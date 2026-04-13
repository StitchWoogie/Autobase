# PLC_SCAN 쓰기(Write) 이벤트 소실/중복 문제 분석

## 1. 문제 현상

### 1.1 보고된 증상

| 증상 | 설명 |
|------|------|
| **이벤트 값 중복** | 1~50 순차 출력 시 50이 50번 출력됨 (보고 기준) |
| **이벤트 소실** | 동시 다발 이벤트에서 중간 이벤트 누락 |
| **Sleep 필수** | 출력 사이 Sleep(400) 삽입해야 정상 동작 |
| **패킷별 딜레이 다름** | 100ms~1초로 제각각 |
| **Sleep 시 UI Hang** | Sleep이 메인 스레드 블로킹 |
| **새 엔진 미동작** | 새 스크립트 엔진에서 멀티출력/WriteBlock 불가 |

> **주의:** "50이 50번 출력" 현상은 현재 분석된 코드 경로로는 직접 설명되지 않습니다.
> 본 문서의 PLC_SCAN 큐 분석이 직접 설명하는 현상은 **"1~49 소실, 마지막 값(50)만 1회 남음"** 에 가깝습니다.
> "50번 중복 출력" 시나리오는 별도 재현 로그로 추가 확인이 필요합니다.

### 1.3 분석 신뢰도 구분

본 문서의 각 원인 항목은 아래 신뢰도로 분류합니다:

- **[확정]** — 코드로 직접 확인되는 강한 근거. 즉시 수정 후보.
- **[가능성 있음]** — 코드 경로상 문제 가능성은 있으나 현재 증상의 주원인인지 추가 검증 필요.
- **[재검증 필요]** — 단독으로 현상을 설명하지 못하거나 다른 경로로 회피되는 가설. 재현 로그 확보 전까지 수정 보류 권장.

### 1.2 영향 범위

SECS Host/Equipment 드라이버만의 문제가 아니라, 쓰기 큐 구조 자체의 문제:
- SECS Host/Equipment — 이벤트 보고 소실/중복
- MELSEC ENET 3E — 멀티출력
- OMRON PLC — @PlcScanWriteBlock 함수

---

## 2. 쓰기 데이터 흐름

```
[스크립트/태그 출력]
      │
      ├─ PLC_SCAN 내부 호출 (Plcwrite.cpp)
      │   └─ AddWaitWriteAnalogOut() / AddWaitWriteDigitalOut()
      │       └─ InsertWriteWaitOne()  ← 직접 큐 삽입
      │
      ├─ LocalMain (감시 프로그램) — 별도 프로세스
      │   └─ PlcScan.cs → AddWriteList() → Win32Common.dll
      │       └─ 공유메모리 "SHARE_MAIN_PLCSCAN" 링 버퍼에 기록
      │           └─ ProcEventRecv 스레드가 Sleep(1) 폴링으로 수신
      │               └─ InsertWriteWaitFromRing()
      │                   └─ InsertWriteWaitOne()  ← 큐 삽입
      │
      ├─ NetworkServer (원격 클라이언트)
      │   └─ ScanServerStatus.cpp:283-290
      │       └─ AddWaitWriteAnalogOut() / AddWaitWriteDigitalOut()
      │           └─ InsertWriteWaitOne()  ← 큐 삽입
      │
      └─ DLL 내부 콜백 (프로토콜 DLL이 다른 포트에 쓰기)
          └─ Pro_main.cpp:1300-1305 — SetProc으로 등록된 콜백
              └─ AddWaitWriteAnalogOut() / AddWaitWriteDigitalOut()
                  └─ InsertWriteWaitOne()  ← 큐 삽입
                                │
                                v
                  ┌─ 포트별 링 버퍼 큐 ──────────────────────┐
                  │  WRITE_WAIT_STRUCT                        │
                  │  ring_current ──→ ring_target             │
                  │  최대 4000개 (MAX_SCAN_WRITE_LOCAL_ITEM)  │
                  │  bPushing / bPoping char 플래그 동기화     │
                  └──────────────┬────────────────────────────┘
                                 │
                  ┌──────────────v────────────────────────────┐
                  │  RunWriteWait() — 포트 스레드에서 실행      │
                  │                                            │
                  │  큐에서 1개 팝 → 프로토콜 Write 호출       │
                  │  → return;  ← **1개 처리 후 즉시 리턴**   │
                  │                                            │
                  │  다음 사이클(Sleep(1)+Read) 후 또 1개 처리  │
                  └──────────────┬────────────────────────────┘
                                 │
                                 v
                  DLL: ProtocolWriteWord / WriteBit / WriteBlock
```

---

## 3. 근본 원인 분석

### 원인 1: 값 덮어쓰기 — `bUseNewValueOnAnalogOut` (핵심 원인) [확정]

**위치:** `Scanstat.cpp:1553-1580`, 기본값 ON: `Scanconf.cpp:68`

```cpp
else if(item->command == 1) {   // Word write
    if(config.bUseNewValueOnAnalogOut) {
        SetPushing(pt);
        // 큐에서 같은 주소의 기존 항목 검색
        while(true) {
            if(wait->command == 1 &&
               wait->port == item->port &&
               wait->station == item->station &&
               wait->address == item->address &&
               strcmp(wait->sExtraAddr, item->sExtraAddr) == 0) {

                wait->value = item->value;   // ← 기존 값을 최신값으로 덮어씀!
                ResetPushing(pt);
                return;                       // ← 새 항목 추가하지 않고 리턴
            }
            ...
        }
    }
}
```

**동작:**
```
스크립트: for(i=1; i<=50; i++) { @PlcScanWriteWord(port, addr, i); }

큐 상태:
  i=1  → 큐: [addr=100, value=1]       ← 새로 추가
  i=2  → 큐 검색: addr=100 발견!
         → [addr=100, value=2]          ← 1을 2로 덮어씀
  i=3  → [addr=100, value=3]           ← 2를 3으로 덮어씀
  ...
  i=50 → [addr=100, value=50]          ← 49를 50으로 덮어씀

포트 스레드 처리 시: value=50 하나만 출력
→ "50이 50번 출력" 현상은 아니지만 "1~49 소실" 발생
```

**"최종값으로 출력" 해제하면?**
큐에 50개가 모두 들어감. 하지만 원인 2로 인해 별도 문제 발생.

### 원인 2: 1사이클 1쓰기 병목 [확정]

**위치:** `Scanstat.cpp:1706-1765` (1건 처리 후 return은 `Scanstat.cpp:1760`, 예외는 `COMMUNICATION_NEXT_WRITE_GO`)
**포트 스레드 루프:** `PortThread.cpp:29` → `CommStatusLocalOne()` → `Scanstat.cpp:774`에서 `RunWriteWait()` 호출

```cpp
void RunWriteWait(GLOBAL_PORT_STRUCT *pt)
{
    for(int i = 0; i < MAX_SCAN_WRITE_LOCAL_ITEM_COUNT; i++) {
        // 큐에서 1개 팝
        // 프로토콜 Write 실행
        
        return;   // ← 1개 처리 후 즉시 리턴!
        // 주석: "하나만 보내고 돌아간다."
    }
}
```

**1760번 줄의 `return;`이 핵심 병목입니다.**

포트 스레드 사이클:
```
Sleep(1ms) → ReadScan → RunWriteWait(1건만 처리) → 다음 사이클
```

| 항목 | 값 |
|------|---|
| 사이클 시간 | Sleep(1) + Read 시간 = 수 ms ~ 수십 ms |
| 쓰기 처리량 | **사이클당 1건** |
| 50건 처리 시간 | 50 × 사이클 시간 = 수백 ms |
| 스크립트 투입 속도 | for 루프 = 마이크로초 단위로 50건 즉시 투입 |

**결과:** 큐에 50건이 쌓이는데, 처리는 사이클당 1건 → 큐 적체.
큐 적체 중에 `bUseNewValueOnAnalogOut=ON`이면 후속 쓰기가 기존 항목 덮어씀.

### 원인 3: char 플래그 동기화 경쟁 조건 [가능성 있음 — 위험 요소이나 현재 증상의 주원인 근거는 부족]

**위치:** `Scanstat.cpp:1477-1507`, 플래그 정의 `plc_scan.h:264`

```cpp
static void SetPushing(GLOBAL_PORT_STRUCT *pt)
{
    TimeOutClass timeout;
    while(pt->blockWriteWait->bPoping) {   // volatile 아님!
        Sleep(1);
        if(timeout.IsTimeOut(3)) break;     // 3초 후 강제 진행!
    }
    pt->blockWriteWait->bPushing = 1;       // 원자적 연산 아님!
}
```

| 문제 | 설명 |
|------|------|
| `char` 타입 | CPU 캐시로 인해 다른 스레드에서 변경을 못 볼 수 있음 |
| `volatile` 없음 | 컴파일러 최적화로 읽기 자체를 생략할 수 있음 |
| 원자적 연산 아님 | 두 스레드가 동시에 SetPushing 진입 가능 |
| 3초 타임아웃 후 **무시하고 진행** | 경합 시 데이터 손실/손상 |

#### bPushing/bPoping은 어떤 스레드끼리 경합하는가?

**포트별 큐(`blockWriteWait`)는 포트마다 독립**입니다. 따라서 SECS 포트의 bPoping은 다른 DLL(Melsec, Modbus 등) 스레드와 경합하지 않습니다.

경합하는 스레드는:

```
[Port N의 blockWriteWait 큐]

Push하는 스레드들 (SetPushing):
  ├─ ProcEventRecv 스레드 — LocalMain 공유메모리 폴링
  │   (Scanstat.cpp:1810-1811, InsertWriteWaitFromRing)
  │
  ├─ ProcEventRecv 스레드 — NetworkServer 이벤트
  │   (Scanstat.cpp:1814-1823, InsertWriteCommand)
  │
  ├─ PLC_SCAN 메인 스레드 — UI 수동 출력 (Plcwrite.cpp:206)
  │
  └─ 다른 포트의 DLL 스레드 — DLL 콜백으로 이 포트에 쓰기
      (Pro_main.cpp:433, InsertWriteWaitOne)

Pop하는 스레드 (SetPoping):
  └─ Port N의 포트 스레드 — RunWriteWait()
      (Scanstat.cpp:1722, 1750)
```

**경합 시나리오:**
- ProcEventRecv 스레드가 LocalMain에서 받은 쓰기를 큐에 넣는 중 (SetPushing)
- 동시에 포트 스레드가 큐에서 꺼내는 중 (SetPoping)
- 동시에 NetworkServer가 원격 쓰기를 큐에 넣으려 함 (SetPushing)

→ 3개 스레드가 **같은 포트의 같은 큐**에 동시 접근.

### 원인 4: 공유메모리 폴링 `Sleep(1)` [가능성 있음]

**위치:** `Scanstat.cpp:1802-1812`

```cpp
static DWORD WINAPI ProcEventRecv(LPVOID)
{
    while(bThreadFlag) {
        Sleep(1);                            // ← 1ms 간격 폴링
        if(share_Main_PlcScan) {
            InsertWriteWaitFromRing(share_Main_PlcScan);  // 공유메모리 → 포트별 큐
        }
    }
}
```

LocalMain에서 공유메모리에 쓰기를 넣어도, PLC_SCAN이 **1ms 간격 폴링**으로 가져감.
공유메모리 링 버퍼(`SCAN_WRITE_EXCHANGE_INFO`)의 `ring_current`/`ring_target`은 `short` 타입이고 원자적 연산 없음 → 프로세스 간 경쟁 조건 존재.

### 원인 5: 큐 Full 시 조용한 유실 [확정]

**위치:** 내부 큐 `Scanstat.cpp:1588-1598` (MessageDisplay 후 drop), 공유메모리 링 `FuturePlcScan.cpp:314` (조용히 drop)

```cpp
next_pos = (pt->blockWriteWait->ring_target+1) % MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;

if(next_pos == pt->blockWriteWait->ring_current) {
    // 큐 꽉 참!
    MessageDisplay("Write items are too many >= 1000.");
    return;   // ← 쓰기 명령 버림!
}
```

로그 메시지만 남기고 쓰기를 **조용히 버립니다.**

---

## 4. Sleep(400)이 "해결"되는 이유

```
Sleep(400) 삽입 시:
  Write(value=1) → Sleep(400) → Write(value=2) → Sleep(400) → ...
  
  400ms 동안 포트 스레드가 ~수백 사이클 실행
  → 큐의 value=1이 처리 완료됨
  → value=2 삽입 시 큐에 같은 주소 항목 없음
  → 덮어쓰기 발생 안 함 ✓
  
  하지만:
  → 스크립트 스레드 블로킹 → UI 멈춤
  → 시간당 수천 건 이벤트 × 400ms = 처리 불가능
```

---

## 5. 새 스크립트 엔진에서 더 심각한 이유

### 구 엔진 vs 새 엔진 경로

| 항목 | 구 엔진 | 새 엔진 (`bUseNewEngine`) |
|------|--------|--------------------------|
| 실행 위치 | 같은 프로세스 내 | 같은 프로세스 (delegate 경유) |
| 쓰기 경로 | ScriptFunction → delegate → AddWriteList() → 공유메모리 | 동일 |
| 실행 속도 | 인터프리터 오버헤드로 약간 느림 | 더 빠르게 실행 가능 |
| 결과 | 암묵적 지연이 있어 일부 동작 | **더 빠른 투입 → 덮어쓰기 빈번** |

두 엔진 모두 동일한 공유메모리 경로를 사용:
```
ScriptFunctionPlcScan.cs → PlcScan.cs → AddWriteList()
  → Win32Common.dll (P/Invoke)
    → 공유메모리 SHARE_MAIN_PLCSCAN 링 버퍼
      → ProcEventRecv Sleep(1) 폴링
        → InsertWriteWaitFromRing() → InsertWriteWaitOne()
```

새 엔진이 더 빠르게 쓰기를 투입하면:
1. 공유메모리 링 버퍼에 빠르게 쌓임
2. ProcEventRecv가 1ms 간격으로 가져와서 포트별 큐에 삽입
3. 삽입 시 `bUseNewValueOnAnalogOut` 로직으로 값 덮어쓰기
4. RunWriteWait는 1사이클 1건만 처리 → 적체 심화

---

## 6. 이전 PLC_SCAN 분석과의 연관

| 이전 분석 항목 (NETWORK_PORT_ANALYSIS.md) | 쓰기 문제와의 연관 |
|------------------------------------------|-------------------|
| **`connect()` 블로킹** (Comtcpip.cpp:182) | 연결 실패 시 Write 타임아웃 → 큐 적체 → 후속 쓰기 유실 |
| **1사이클 = Sleep(1) + Read + Write** (PortThread.cpp:29-33) | Read가 느리면 Write 처리 간격도 벌어짐 |
| **bThreadProtocolDrawWorking 잠금** (Pro_main.cpp:55-66) | UI 그리기와 쓰기가 같은 잠금 경쟁 → 쓰기 지연 |
| **ScanServerPause 전체 중단** (ScanEdit.cpp:20) | 편집 중 쓰기도 함께 중단되어 큐 적체 |
| **메인 스레드 Hang** (시나리오 1, 2) | Hang 중 ProcEventRecv 스레드 자체는 동작하나, UI 출력 불가 |

---

## 7. 해결 방향

### 즉시 적용 가능 (DLL 변경 없음)

| 순서 | 개선 | 수정 위치 | 수정량 | 효과 |
|------|------|-----------|--------|------|
| **1** | **RunWriteWait()에서 N건 연속 처리** | `Scanstat.cpp:1760` | 1줄: `return;` → `continue;` 또는 `if(count >= N) return;` | 쓰기 처리량 N배 증가 |
| **2** | **bUseNewValueOnAnalogOut 기본값 OFF** | `config` 초기화 | 1줄 | 값 덮어쓰기 방지 (모든 쓰기 큐잉) |
| **3** | **char → InterlockedExchange** | `Scanstat.cpp:1477-1507` | ~10줄 | 경쟁 조건 제거 |
| **4** | **공유메모리 폴링 → Event 기반** | `Scanstat.cpp:1807-1812` | ~5줄 | Sleep(1) 폴링 → 즉시 반응 |
| **5** | **큐 Full 시 대기 또는 확장** | `Scanstat.cpp:1590` | ~5줄 | 조용한 유실 방지 |

### 1번 수정의 구체적 방법

```cpp
// 현재 (Scanstat.cpp:1706-1765)
void RunWriteWait(GLOBAL_PORT_STRUCT *pt)
{
    for(int i = 0; i < MAX_SCAN_WRITE_LOCAL_ITEM_COUNT; i++) {
        // ... 큐에서 1개 팝 → Write 실행 ...
        
        if(retn == COMMUNICATION_NEXT_WRITE_GO) goto next_write;
        VipScanRegister(&wait);
        return;   // ← 1개 후 리턴
    }
}

// 개선안: 최대 N건까지 연속 처리
void RunWriteWait(GLOBAL_PORT_STRUCT *pt)
{
    int maxBatch = 16;  // 한 사이클에 최대 16건
    
    for(int i = 0; i < maxBatch; i++) {
        SetPoping(pt);
        if(pt->blockWriteWait->ring_current == pt->blockWriteWait->ring_target) break;
        // ... 팝 & Write 실행 ...
        
        pt->blockWriteWait->ring_current = next_pos;
        ResetPoping(pt);
        SetWriteWaitCount(pt);
        
        if(retn == COMMUNICATION_TIME_OUT) {
            // 타임아웃 시 리트라이 로직 유지
            break;
        }
        VipScanRegister(&wait);
        // return 제거 → continue로 다음 건 처리
    }
}
```

### 중기 개선

| 순서 | 개선 | 효과 |
|------|------|------|
| **6** | 비동기 쓰기 완료 콜백 (스크립트에서 Sleep 대신 대기) | Sleep 제거, UI Hang 방지 |
| **7** | 쓰기 전용 스레드 분리 (Read/Write 독립) | Read 지연이 Write에 영향 안 줌 |
| **8** | SECS/GEM 전용 이벤트 큐 (순서 보장, 값 덮어쓰기 금지) | 반도체 공정 무결성 보장 |

---

## 8. SECS/GEM에서 특히 심각한 이유

SECS/GEM 이벤트 보고(S6F11)는 **순서와 무결성이 필수**:

```
[장비]                          [Host]
  S6F11 (CEID=1, 기판 투입)  →
  S6F11 (CEID=2, 공정 시작)  →   ← 시간당 수천 건
  S6F11 (CEID=3, 공정 완료)  →
  S6F11 (CEID=4, 기판 배출)  →
```

현재 구조에서 발생하는 문제:
- CEID=1 출력 큐잉 → CEID=2 출력 시 같은 주소면 값 덮어쓰기 → **CEID=1 소실**
- Sleep으로 우회하면 → 이벤트 처리 지연 → 장비 통신 타임아웃
- MES(Manufacturing Execution System)에 잘못된 이벤트 보고 → **공정 추적 불가**

---

## 9. SECS Host DLL 내부 분석

### 9.1 DLL 구조 개요

SECS_Host_2.zip 소스 분석 결과, PLC_SCAN 쓰기 큐 문제와 별개로 DLL 자체에도 일부 개선이 필요한 지점이 있습니다.
다만 당초 작성된 여러 시나리오 중 상당수는 코드 재확인 결과 **성립하지 않거나 약한 가설**로 재분류되었습니다 (9.13 참조).

```
SECS_Host.cpp      — 메인 DLL (ProtocolRead/WriteWord/WriteBit)
SECS_Hsms.cpp      — HSMS TCP 통신 (WriteWordHsms, ReadHsms)
SECS_HostDef.h     — 데이터 구조 (LOCAL_VARS_STRUCT)
SECS_Tools.cpp     — 데이터 디코딩 (readUserDataToMemory, PokeValueAll)
makeSecsFunctionMessage.cpp — SECS 메시지 생성
```

### 9.2 DLL 내부 문제 1: 단일 수신 버퍼 [재검증 결과: 약함 — 즉시 overwrite 버그 아님]

**위치:** `SECS_HostDef.h:200-202`

```cpp
typedef struct {
    ...
    BYTE sendBuf[MAX_SECS_SEND_BUF];    // 송신 버퍼 — 1개
    BYTE saveBuf[MAX_SECS_SAVE_BUF];    // 저장 버퍼 — 1개
    BYTE recvBuf[MAX_SECS_RECV_BUF];    // 수신 버퍼 — 1개
    READ_DATA_STRUCT readDataSt;        // 수신 패킷 정보 — 1개
    ...
} LOCAL_VARS_STRUCT;
```

**재검증 결과:**
`recvBuf`가 단일 버퍼인 것은 사실이지만, 현재 구현은 `PlcDeviceReadContinue(..., 1)`로 한 메시지를
**1바이트씩 순차적으로 읽고**, 한 패킷을 완료한 뒤 `readDataToMemoryHsms()`로 메모리에 반영하고 나서야
다음 바이트 수신으로 진행합니다 (`SECS_Hsms.cpp:247, 303` 참조).
즉, 이벤트 A 처리 도중에 이벤트 B가 같은 `recvBuf`를 **동시에 덮어쓰는** 구조는 아닙니다.

→ **"단일 recvBuf가 곧바로 이벤트 간 overwrite를 만든다"는 원래 주장은 코드와 맞지 않으므로 철회합니다.**

남는 실제 위험:
- 한 메시지 처리 중 TCP 소켓 버퍼에 backlog가 쌓이는 **지연/적체** 문제 — 이는 `recvBuf` 단일성이 아니라
  `checkWaitOkSignal` 블로킹(9.4)과 DLL 전체의 블로킹 read 루프 구조로 설명하는 게 맞습니다.
- 고속 이벤트 흐름에서 프레이밍/싱크 오류가 났을 때 복구가 어렵다는 일반 위험은 존재하나, 현재 증상의
  주원인으로 올리기에는 근거가 부족합니다.

### 9.3 DLL 내부 문제 2: saveBuf 미초기화 [재검증 결과: 성립하지 않음]

**위치:** `SECS_Host.cpp:443-458`, `makeSecsFunctionMessage.cpp:271-285`, `SECS_Hsms.cpp:36-49`

```cpp
// makeSecsFunctionMessage.cpp:271
int makeSfMemoryDataToBuf(LOCAL_PORT_STRUCT *pt, BYTE *data)
{
    int buf_pos = 0, ...
    ...
    return buf_pos;   // ← 현재 메시지 길이를 새로 계산해 반환
}

// SECS_Hsms.cpp:40-46
getStreamFunctionData(device, localVars->cStream, localVars->cFunction);
makeSecsHeaderHsms(pt, localVars->saveBuf, station, bReadRequest);
localVars->nSendSave = makeSfMemoryDataToBuf(pt, &localVars->saveBuf[HSMS_HEADER_SIZE]);
buf_pos = HSMS_HEADER_SIZE + localVars->nSendSave;
...
if(buf_pos > 0) PlcDeviceWriteContinue(&pt->device, (char*)localVars->saveBuf, buf_pos);
                                                                         // ↑ buf_pos 길이만큼만 전송
```

실제 전송은 항상 **새로 계산된 `buf_pos` 길이**만큼만 수행됩니다 (`PlcDeviceWriteContinue(..., buf_pos)`).
이전 메시지가 100바이트, 새 메시지가 50바이트라도, 전송되는 것은 **새 메시지의 50바이트**뿐입니다.
saveBuf의 나머지 영역에 이전 데이터가 남아 있어도 TCP 스트림에 나가지 않습니다.

→ **"이전 이벤트 데이터가 섞여서 전송"이라는 원래 주장은 코드와 맞지 않으므로 철회합니다.**

### 9.4 DLL 내부 문제 3: 블로킹 대기 루프 — checkWaitOkSignal [확정]

**위치:** `SECS_Host.cpp:805-831`

```cpp
void checkWaitOkSignal(LOCAL_PORT_STRUCT *pt)
{
    TimeOutClass timeout;
    bool flag = false;
    char curr[256];
    
    while(1) {
        if(timeout.IsTimeOut(localVars->nResponseCheckTimeout)) return;
        if(Tag9GetCurr(localVars->sCurrWaitOkTag, curr) == false) return;
        val = atoi(curr);
        if(val == 1) flag = true;
        if(flag && val != 1) {
            return;
        }
    }
}
```

외부 프로그램의 OK 신호를 **타임아웃 있는 busy-wait**(`while(1)` + `IsTimeOut(nResponseCheckTimeout)`)로 대기합니다.
무한 대기는 아니지만 타임아웃(기본 2000ms) 전까지는 `Sleep` 없이 `Tag9GetCurr`만 반복 호출하므로,
이 동안 ProtocolRead 스레드가 사실상 **완전히 블로킹** → 새 이벤트 수신 불가.

### 9.5 DLL 내부 문제 4: nBlockNo 미리셋 [재검증 결과: 성립하지 않음]

**위치:** `SECS_Host.cpp:858` (블록 전송 완료 후 리셋), `SECS_Host.cpp:350-358` (`setStartInitFlag`에서 nBlockNo=1 초기화), `SECS_Host.cpp:960-...` (`WriteWordSerial`)

```cpp
// SECS_Host.cpp:350
void setStartInitFlag(LOCAL_PORT_STRUCT *pt) {
    ...
    localVars->bSendDataPacket = false;
    ...
    localVars->nBlockNo = 1;   // ← Write 진입 시 초기화
}

// SECS_Host.cpp:960 WriteWordSerial
static int WriteWordSerial(LOCAL_PORT_STRUCT *pt, int station, char *device) {
    ...
    setStartInitFlag(pt);                 // ← 진입 시 nBlockNo=1 리셋
    localVars->nStatus = READ_EOT_STATUS;
    ...
    while(1) { ... }                      // ← 완료/타임아웃까지 블로킹
}
```

Serial 경로는 `WriteWordSerial` 진입 시 `setStartInitFlag`로 `nBlockNo=1` 즉시 리셋되고, 블로킹 루프로 완료까지 진행합니다.
HSMS 경로도 `checkConnectAndSendSelectRequest` → `setStartInitFlag`로 진입 시 리셋됩니다.

→ **"nBlockNo 리셋 타이밍이 Sleep 필요의 원인"이라는 주장은 현재 코드와 맞지 않으므로 철회합니다.**
Sleep 크기가 패킷마다 다른 현상은 재현 로그 확보 전까지 원인 보류.

### 9.6 DLL 내부 문제 5: 플래그 동기화 없음 [재검증 결과: 현재 구조상 과장]

```cpp
// 상태 플래그들 (SECS_HostDef.h)
bool bSendDataPacket;      // 데이터 전송 중
bool bReadRequest;         // 읽기 요청 중
bool bReadWriteRequest;    // 읽기/쓰기 요청 중
bool bReadDone;            // 읽기 완료
bool bRespRequire;         // 응답 필요
bool bWaitSendOkSignal;    // OK 신호 대기 중
```

이 플래그들은 mutex/critical section **없이** 사용되는 것은 사실입니다.
그러나 이 DLL은 **PLC_SCAN의 해당 포트 스레드가 순차적으로** `ProtocolRead`/`WriteWord`를 호출합니다 (`PortThread.cpp:29`, `Scanstat.cpp:774`).
즉 동일 포트에서 read/write 여러 스레드가 동시에 내부 플래그를 건드리는 구조가 아니므로, 원래 문서가 묘사한 "동시 충돌" 시나리오는 현재 구조상 과장된 면이 있습니다.
위험 요소로 기록하되, 현재 증상의 주원인 근거로 쓰기는 부적절합니다.

### 9.7 (구) 문제 계층 정리 — 재검증 후 9.13으로 대체

> 본 절의 초기 계층 정리는 재검증 결과 일부 항목이 성립하지 않아 **9.13 "재검증 후 계층 정리"로 대체**합니다.
> 원 표는 혼선을 피하기 위해 제거했습니다.

### 9.8 DLL 내부 문제 6: 응답 매칭에 Transaction ID(TNS) 미사용 [확정 — 프로토콜 정합성 허점]

**위치:** `SECS_Hsms.cpp:210`

```cpp
// 응답 매칭 — Stream/Function만 비교, TNS 비교 없음!
localVars->bReadDone = (localVars->cSendStream == cStream &&
    (BYTE)(localVars->cSendFunction + 1) == cFunction &&
    localVars->bEqualStation) ? true : false;
```

SECS/GEM에서 Transaction Number(TNS)는 요청-응답을 정확히 매칭하는 **필수 식별자**입니다.
현재 코드는 Stream/Function 번호만 비교하므로:

- Write #1 (S1F13, TNS=100) 전송
- Write #2 (S1F13, TNS=101) 전송
- Response #1 (S1F14, TNS=100) 도착 → `bReadDone = true`
- Write #1의 응답이 Write #2도 만족시킴 → **Write #2의 실제 응답은 무시됨**

TNS는 `SECS_Hsms.cpp:31`에서 `localVars->dTns++`로 증가하고, 수신 시 `SECS_Hsms.cpp:163`에서 읽지만 **비교에 사용되지 않습니다.**

### 9.9 DLL 내부 문제 7: setInitRecvStatus()의 조기 버퍼 초기화 [재검증 결과: 약함]

**위치:** `SECS_Host.cpp:341-348`, `SECS_Hsms.cpp:268,275`

```cpp
void setInitRecvStatus(LOCAL_PORT_STRUCT *pt) {
    pt->commCountCurr = 0;   // 로컬 위치 인덱스만 0으로 리셋
    localVars->bSize = false;
}
```

`SECS_Hsms.cpp`의 수신 루프는 `PlcDeviceReadContinue(&pt->device, ..., 1)`로 **1바이트씩** 읽습니다 (line 247, 303).
즉 응답 #1 다음에 아직 읽지 않은 #2 데이터는 **장치/TCP 버퍼에 그대로 남아** 있고, `commCountCurr=0`은 `localVars->recvBuf`에서의 기록 위치를 다시 0으로 두는 것뿐입니다.
→ "응답 #1의 남은 데이터가 영구 유실"이라는 원래 주장은 성립하지 않습니다.

다만 파이프라인/backlog 상황에서 헤더 싱크 감지가 복잡해질 수 있다는 수준의 위험 요소로는 남습니다. 수정 보류.

### 9.10 DLL 내부 문제 8: WriteWord 2회차부터 전송 자체가 안 됨 (HSMS) [재검증 결과: 성립하지 않음]

**위치:** `SECS_Hsms.cpp:124-137` (`checkConnectAndSendSelectRequest`), `SECS_Host.cpp:350-358` (`setStartInitFlag`)

```cpp
// SECS_Hsms.cpp:124
bool checkConnectAndSendSelectRequest(LOCAL_PORT_STRUCT *pt, bool bRead)
{
    setStartInitFlag(pt);                        // ← 진입 시 호출
    ...
}

// SECS_Host.cpp:350
void setStartInitFlag(LOCAL_PORT_STRUCT *pt)
{
    localVars->bReadRequest = false;
    localVars->bReadOkFlag = false;
    localVars->bSendDataPacket = false;          // ← 매 write 진입 시 false로 리셋
    localVars->bSendControlCode = false;
    localVars->cSendHsmsControl = HSMS_DATA_CODE;
    localVars->nBlockNo = 1;
}
```

`WriteWordHsms` → `sendControlCodeOrdReadWriteCommand` → `checkConnectAndSendSelectRequest` → `setStartInitFlag`의 경로로,
**매 write 진입마다 `bSendDataPacket`이 false로 초기화**됩니다.
따라서 "2회차부터 이전 write의 true 상태가 남아 조기 리턴되어 실제 전송되지 않는다"는 원래 시나리오는 **현재 코드와 맞지 않으므로 철회합니다.**

참고: `WriteWordHsms:293`의 `if(bSendDataPacket && bReadRequest == false) return COMMUNICATION_OK;`는,
이번 write가 "응답 불필요(짝수 function)" 종류일 때 **실제 전송을 끝낸 뒤 read 루프로 내려가지 않고 즉시 OK 리턴**하기 위한 정상 경로입니다.
bSendDataPacket은 바로 직전 `sendReadWriteRequestDataReal`에서 방금 true로 설정된 것이며, "이전 write의 잔류값"이 아닙니다.

### 9.11 DLL 내부 문제 9: checkWaitOkSignal 블로킹 중 이벤트 수신 불가 [가능성 있음 — 옵션 켜진 경우]

```
S6F11 이벤트 도착
  → readDataToMemoryHsms() → 메모리에 기록
  → bRespRequire = true
  → checkWaitOkSignal() 진입 — 최대 2000ms 블로킹!
    │
    │  이 동안 TCP 소켓 버퍼에 S6F11 #2, #3, #4 쌓임
    │  하지만 ProtocolRead 루프가 블로킹 중이므로 수신 불가
    │
  → 타임아웃 또는 OK 신호 수신
  → S6F12 응답 전송
  → 루프 재개 → S6F11 #2 처리 시작
    (이미 수백~수천 ms 지연됨)
```

시간당 수천 건이면 이벤트 간격 < 1초. `checkWaitOkSignal()` 타임아웃이 2초면 → **이벤트 적체 → TCP 버퍼 오버플로 → 연결 끊김.**

### 9.12 (구) 최종 계층 정리 — 9.13으로 대체

### 9.13 재검증 후 문제 계층 최종 정리

코드 재확인을 반영하여 신뢰도별로 재분류합니다.

```
[계층 1: SECS Host DLL — 프로토콜 레벨]

  [확정]
  ├─ 응답 매칭에 TNS 미사용 (Stream/Function + station만 비교) — SECS_Hsms.cpp:210
  └─ checkWaitOkSignal 블로킹 루프 (옵션 켜진 경우) — SECS_Host.cpp:805

  [가능성 있음]
  └─ checkWaitOkSignal 중 backlog로 인한 지연 악화 — 옵션/타임아웃 설정에 의존

  [재검증 결과 철회 또는 약함]
  ├─ 단일 recvBuf → 이벤트 덮어쓰기  (순차 처리로 곧바로 버그는 아님)
  ├─ saveBuf 미초기화 → 이전 데이터 전송  (송신 길이 재계산으로 미발생)
  ├─ nBlockNo 리셋 타이밍 → Sleep 필요  (setStartInitFlag에서 진입 시 리셋)
  ├─ bSendDataPacket 조기 리턴 → 2회차 미전송  (setStartInitFlag에서 초기화)
  ├─ setInitRecvStatus → 응답 유실  (1바이트 read로 TCP 버퍼에 데이터 잔존)
  └─ DLL 내부 플래그 비동기화 → 충돌  (동일 포트 순차 호출 구조로 과장)

[계층 2: PLC_SCAN 본체 — 쓰기 큐 레벨]

  [확정]
  ├─ bUseNewValueOnAnalogOut — 같은 주소 값 덮어쓰기 (Scanstat.cpp:1553, 기본값 ON Scanconf.cpp:68)
  ├─ 1사이클 1쓰기 — RunWriteWait return (Scanstat.cpp:1760)
  └─ 큐 Full 시 drop — 내부 큐(MessageDisplay) + 공유링(조용히)

  [가능성 있음]
  ├─ char 플래그 비동기화 — 위험 요소이나 현재 증상 주원인 근거 부족
  └─ 공유메모리 Sleep(1) 폴링 — 지연 기여
```

| 증상 | DLL 원인 (확정만) | PLC_SCAN 원인 (확정만) |
|------|-------------------|------------------------|
| **이벤트 소실 (1~49 누락)** | — | bUseNewValueOnAnalogOut + 1사이클 1쓰기 적체 |
| **응답 오매칭** | TNS 미검증 | — |
| **이벤트 처리 지연/연결 위험** | checkWaitOkSignal 블로킹 (옵션 켜진 경우) | 1사이클 1쓰기 + 큐 적체 |
| **Sleep 필수** | (주원인 미확정 — 재현 로그 필요) | 1사이클 1쓰기 (유입 속도 > drain 속도 시 큐 덮어쓰기 유발) |
| **UI Hang** | — | 메인 스레드 Sleep(400) |
| **"50번 중복 출력"** | (재현 로그 필요) | (현재 코드로는 직접 설명되지 않음) |

---

## 10. 종합 해결 방안 (재검증 반영)

### 10.1 수정 우선순위 (재정렬)

재검증 결과, **우선순위는 PLC_SCAN 본체 쪽에 집중**하고 SECS DLL은 확정된 항목(TNS, checkWaitOkSignal)만 착수, 나머지는 재현 로그 확보 후 진행합니다.

```
[1순위: PLC_SCAN — 근거 확정, 모든 프로토콜에 효과]
  1) same-address overwrite 정책 재검토 — bUseNewValueOnAnalogOut 기본값 OFF 또는 주소별 정책화
  2) RunWriteWait() 배치 처리 — 한 사이클 N건 처리로 drain 속도 확보
  3) 큐 동기화 개선 (InterlockedExchange/CRITICAL_SECTION) + 큐 Full 정책 개선 (drop → wait/확장 + 로그)

[2순위: SECS Host DLL — 확정 근거 있는 항목만]
  4) HSMS 응답 매칭에 TNS 비교 추가 (SECS_Hsms.cpp:210)
  5) checkWaitOkSignal 비동기화/타임아웃 단축 검토 (SECS_Host.cpp:805)

[보류: 재현 로그 확보 전까지]
  - saveBuf memset, nBlockNo 즉시 리셋, bSendDataPacket 조기 리턴
  - setInitRecvStatus 조기 호출, DLL 내부 플래그 락
  → 현재 코드 경로로는 직접 설명되지 않거나 다른 메커니즘으로 회피됨.
     실제 장비 로그 (Wireshark/포트 캡처, 순차 write 실측)로 재현이 잡힐 때 재평가.
```

### 10.2 확정 수정 항목 상세

**PLC_SCAN 본체**

| 순서 | 개선 | 수정 위치 | 수정량 | 효과 |
|------|------|-----------|--------|------|
| **1** | `bUseNewValueOnAnalogOut` 기본값 OFF 또는 주소별 opt-in | `Scanconf.cpp:68` 기본값, `Scanstat.cpp:1553` 정책 분기 | 수 줄 | 같은 주소 값 덮어쓰기로 인한 중간값 소실 방지 |
| **2** | RunWriteWait()에서 N건 연속 처리 | `Scanstat.cpp:1706-1765` (`return` → 배치 루프) | ~10줄 | 사이클당 처리량 N배 향상 |
| **3** | 내부 큐 동기화 개선 + Full 시 정책 변경 | `Scanstat.cpp:1477-1507`, `Scanstat.cpp:1588-1598`, `FuturePlcScan.cpp:314` | ~20줄 | 경쟁 조건 완화 및 조용한 drop 제거 |

**SECS Host DLL**

| 순서 | 개선 | 수정 위치 | 수정량 | 효과 |
|------|------|-----------|--------|------|
| **4** | 응답 매칭에 TNS 비교 추가 | `SECS_Hsms.cpp:210` (기록은 `:163`, 송신은 `:31/115/129`) | ~3줄 | 요청-응답 정확 매칭, 고속 파이프라인 시 오매칭 방지 |
| **5** | `checkWaitOkSignal` 비동기화/타임아웃 단축 | `SECS_Host.cpp:805-831` | ~20줄 | 이벤트 수신 루프 블로킹 최소화 |

### 10.3 보류 항목

| 항목 | 보류 사유 |
|------|-----------|
| saveBuf memset | 송신 길이 재계산으로 잔여 바이트가 전송되지 않음 |
| nBlockNo 즉시 리셋 | `setStartInitFlag`에서 write 진입 시 이미 리셋됨 |
| bSendDataPacket 조기 리턴 (2회차 미전송) | `setStartInitFlag`에서 매 write 진입 시 false로 초기화됨 |
| setInitRecvStatus 조기 호출 | 1바이트 read 구조상 TCP 버퍼에 데이터 잔존, 로컬 리셋이 곧바로 유실을 의미하지 않음 |
| DLL 내부 플래그 락 | 동일 포트에서 PLC_SCAN 포트 스레드가 순차 호출 — 동시 충돌 시나리오 과장 |

> 위 항목들은 **"위험 요소"로는 기록하되**, 별도 재현 로그(예: Wireshark HSMS 캡처, WriteWord 순차 호출 시 실제 송출 패킷 비교)로 확인되기 전까지는 수정 진행을 보류합니다.
