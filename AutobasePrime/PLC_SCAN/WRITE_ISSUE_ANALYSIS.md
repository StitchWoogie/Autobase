# PLC_SCAN 쓰기(Write) 이벤트 소실/중복 문제 분석

## 1. 문제 현상

### 1.1 보고된 증상

| 증상 | 설명 |
|------|------|
| **이벤트 값 중복** | 1~50 순차 출력 시 50이 50번 출력됨 |
| **이벤트 소실** | 동시 다발 이벤트에서 중간 이벤트 누락 |
| **Sleep 필수** | 출력 사이 Sleep(400) 삽입해야 정상 동작 |
| **패킷별 딜레이 다름** | 100ms~1초로 제각각 |
| **Sleep 시 UI Hang** | Sleep이 메인 스레드 블로킹 |
| **새 엔진 미동작** | 새 스크립트 엔진에서 멀티출력/WriteBlock 불가 |

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

### 원인 1: 값 덮어쓰기 — `bUseNewValueOnAnalogOut` (핵심 원인)

**위치:** `Scanstat.cpp:1553-1580`

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

### 원인 2: 1사이클 1쓰기 병목

**위치:** `Scanstat.cpp:1706-1765`

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

### 원인 3: char 플래그 동기화 경쟁 조건

**위치:** `Scanstat.cpp:1477-1507`

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

### 원인 4: 공유메모리 폴링 `Sleep(1)`

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

### 원인 5: 큐 Full 시 조용한 유실

**위치:** `Scanstat.cpp:1588-1598`

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

SECS_Host_2.zip 소스 분석 결과, **PLC_SCAN 쓰기 큐 문제와 별개로 DLL 자체에도 심각한 문제**가 있습니다.

```
SECS_Host.cpp      — 메인 DLL (ProtocolRead/WriteWord/WriteBit)
SECS_Hsms.cpp      — HSMS TCP 통신 (WriteWordHsms, ReadHsms)
SECS_HostDef.h     — 데이터 구조 (LOCAL_VARS_STRUCT)
SECS_Tools.cpp     — 데이터 디코딩 (readUserDataToMemory, PokeValueAll)
makeSecsFunctionMessage.cpp — SECS 메시지 생성
```

### 9.2 DLL 내부 문제 1: 단일 수신 버퍼 — 이벤트 덮어쓰기

**위치:** `SECS_HostDef.h:200-202`

```cpp
typedef struct {
    ...
    BYTE sendBuf[MAX_SECS_SEND_BUF];    // 송신 버퍼 — 1개
    BYTE saveBuf[MAX_SECS_SAVE_BUF];    // 저장 버퍼 — 1개
    BYTE recvBuf[MAX_SECS_RECV_BUF];    // 수신 버퍼 — 1개
    READ_DATA_STRUCT readDataSt;         // 수신 패킷 정보 — 1개
    ...
} LOCAL_VARS_STRUCT;
```

**이벤트 큐가 없습니다.** 모든 수신 데이터가 단일 `recvBuf`에 덮어씌워집니다.

**`SECS_Host.cpp:665`:**
```cpp
memcpy(&localVars->recvBuf[0], &pt->commRecvBuf[0], dataLen + 11);
```

이벤트 A(S6F11) 처리 중에 이벤트 B(S6F11)가 도착하면:
→ `recvBuf`가 이벤트 B 데이터로 덮어씌워짐
→ 이벤트 A의 데이터 처리가 이벤트 B의 데이터로 진행됨
→ **이벤트 A 소실 + 이벤트 B 중복 출력**

### 9.3 DLL 내부 문제 2: saveBuf 미초기화 — 이전 데이터 잔존

**위치:** `SECS_Host.cpp:443-458`

```cpp
if(localVars->nBlockNo == 1) {
    getStreamFunctionData(device, localVars->cStream, localVars->cFunction);
    localVars->nSendSave = makeSfMemoryDataToBuf(pt, localVars->saveBuf);
}
```

`saveBuf`에 `memset(0)` 없이 바로 데이터를 씁니다.
이전 이벤트가 100바이트, 새 이벤트가 50바이트면 → 뒤쪽 50바이트에 이전 데이터 잔존
→ SECS 메시지에 **이전 이벤트 데이터가 섞여서 전송**

### 9.4 DLL 내부 문제 3: 블로킹 대기 루프 — checkWaitOkSignal

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

외부 프로그램의 OK 신호를 **무한 폴링**으로 대기합니다.
이 동안 ProtocolRead 스레드가 **완전히 블로킹** → 새 이벤트 수신 불가.

### 9.5 DLL 내부 문제 4: nBlockNo 미리셋 — Sleep 필요 원인

**위치:** `SECS_Host.cpp:858, 428-436`

```cpp
// 블록 전송 완료 후 리셋
localVars->nBlockNo = 1;  // line 858
```

```cpp
// 다음 블록 데이터 계산
start = (localVars->nBlockNo - 1) * MAX_ONE_PACKET_DATA;  // 244바이트/블록
```

WriteWord 호출 시 `nBlockNo`가 아직 리셋 안 되어 있으면:
→ 새 데이터가 이전 메시지의 **연속 블록**으로 처리됨
→ 패킷 연결(concatenation) 오류

**이것이 Sleep() 크기가 패킷마다 다른 이유:**
- 작은 패킷(1블록): `nBlockNo` 리셋이 빠름 → Sleep(100) 충분
- 큰 패킷(다중 블록): 여러 Read 사이클 필요 → Sleep(1000) 필요

### 9.6 DLL 내부 문제 5: 플래그 동기화 없음

```cpp
// 상태 플래그들 (SECS_HostDef.h)
bool bSendDataPacket;      // 데이터 전송 중
bool bReadRequest;         // 읽기 요청 중
bool bReadWriteRequest;    // 읽기/쓰기 요청 중
bool bReadDone;            // 읽기 완료
bool bRespRequire;         // 응답 필요
bool bWaitSendOkSignal;    // OK 신호 대기 중
```

이 플래그들은 mutex/critical section **없이** 사용됩니다.
ProtocolRead와 WriteWord가 같은 포트 스레드에서 순차 실행되므로 이론상 충돌은 없지만,
`checkWaitOkSignal()`의 블로킹 루프 중에 플래그 변경이 발생하면 불일치 가능.

### 9.7 문제 계층 정리

```
[문제 발생 계층]

계층 1: SECS Host DLL 내부 (프로토콜 레벨)
  ├─ 단일 recvBuf → 이벤트 간 덮어쓰기
  ├─ saveBuf 미초기화 → 이전 데이터 잔존
  ├─ nBlockNo 리셋 타이밍 → Sleep 크기 의존
  └─ checkWaitOkSignal 블로킹 → 이벤트 수신 불가

계층 2: PLC_SCAN 쓰기 큐 (본체 레벨)
  ├─ bUseNewValueOnAnalogOut → 같은 주소 값 덮어쓰기
  ├─ 1사이클 1쓰기 → 처리 속도 병목
  ├─ char 플래그 동기화 → 경쟁 조건
  └─ 큐 Full 시 유실 → 조용한 데이터 손실

두 계층의 문제가 복합적으로 작용하여 증상이 심화됨
```

| 증상 | DLL 원인 | PLC_SCAN 원인 |
|------|---------|--------------|
| **이벤트 소실** | recvBuf 덮어쓰기, TNS 미검증 | bUseNewValueOnAnalogOut |
| **이벤트 중복** | saveBuf 미초기화 | — |
| **Sleep 필수** | nBlockNo 리셋 타이밍 | 1사이클 1쓰기 |
| **Sleep 크기 다름** | 다중 블록 패킷 크기 차이 | — |
| **UI Hang** | checkWaitOkSignal 블로킹 | 메인 스레드 Sleep |

### 9.8 DLL 내부 문제 6: 응답 매칭에 Transaction ID(TNS) 미사용

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

### 9.9 DLL 내부 문제 7: setInitRecvStatus()의 조기 버퍼 초기화

**위치:** `SECS_Host.cpp:341-348`, `SECS_Hsms.cpp:268,275`

```cpp
void setInitRecvStatus(LOCAL_PORT_STRUCT *pt) {
    pt->commCountCurr = 0;   // ← 수신 버퍼 위치 초기화!
    localVars->bSize = false;
}
```

응답 처리 후 `setInitRecvStatus()` → `continue`로 다시 루프:
- 응답 #1의 데이터 부분이 아직 소켓 버퍼에 남아있어도 `commCountCurr=0`으로 리셋
- 다음 루프에서 응답 #2의 헤더를 위치 0부터 읽기 시작
- 응답 #1의 남은 데이터는 **영구 유실**

고속 이벤트에서 파이프라인 응답이 겹칠 때 특히 심각.

### 9.10 DLL 내부 문제 8: WriteWord 2회차부터 전송 자체가 안 됨 (HSMS)

**위치:** `SECS_Hsms.cpp:292-293`, `SECS_Host.cpp:467`

```cpp
// WriteWordHsms() — SECS_Hsms.cpp:287-330
int WriteWordHsms(LOCAL_PORT_STRUCT *pt, int station, char *device)
{
    sendControlCodeOrdReadWriteCommand(pt, station, device);
    
    // ↓ 이미 전송 중이고 응답 불필요하면 → 즉시 리턴!
    if(localVars->bSendDataPacket && localVars->bReadRequest == false)
        return COMMUNICATION_OK;   // ← SUCCESS 반환하지만 실제로는 전송 안 함!
    ...
}
```

```cpp
// sendReadWriteRequestDataReal() — SECS_Host.cpp:460-471
localVars->bSendDataPacket = true;   // line 467 — 첫 번째 Write에서 설정
localVars->bReadRequest = ...;       // 함수번호가 홀수면 true
```

**시나리오:**
```
Write #1: bSendDataPacket=false → makeDataReadBufHsms() 실행 → 실제 전송 ✓
          → bSendDataPacket=true로 설정

Write #2: bSendDataPacket=true 확인 → 즉시 COMMUNICATION_OK 반환
          → 실제로는 아무것도 전송하지 않음!
          → PLC_SCAN은 성공으로 인식 → "보냈다고 거짓말"

Write #3: 동일 → 전송 안 됨
...
(ProtocolRead가 응답을 수신하여 bSendDataPacket=false로 리셋할 때까지)
```

**이것이 "50번 출력하면 50이 50번 출력"의 DLL 레벨 원인입니다.**
PLC_SCAN 쓰기 큐에서 1건씩 꺼내 WriteWord를 호출하지만, DLL이 첫 번째만 실제 전송하고
나머지는 SUCCESS를 반환하면서 무시합니다. 큐에서 꺼낸 49건은 전송되었다고 표시되지만 실제로는 유실됩니다.

### 9.11 DLL 내부 문제 9: checkWaitOkSignal 블로킹 중 이벤트 수신 불가

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

### 9.12 문제 계층 최종 정리

```
[계층 1: SECS Host DLL — 프로토콜 레벨]

  치명적:
  ├─ WriteWord 2회차부터 전송 안 됨 (bSendDataPacket 조기 리턴)
  ├─ 응답 매칭에 TNS 미사용 (Stream/Function만 비교)
  ├─ 단일 recvBuf — 이벤트 간 덮어쓰기
  └─ checkWaitOkSignal 블로킹 — 이벤트 수신 중단

  높음:
  ├─ saveBuf 미초기화 — 이전 데이터 잔존
  ├─ nBlockNo 리셋 타이밍 — Sleep 크기 의존
  └─ setInitRecvStatus 조기 호출 — 파이프라인 응답 유실

[계층 2: PLC_SCAN 본체 — 쓰기 큐 레벨]

  치명적:
  ├─ bUseNewValueOnAnalogOut — 같은 주소 값 덮어쓰기
  └─ 1사이클 1쓰기 — 처리 속도 병목

  높음:
  ├─ char 플래그 동기화 — 경쟁 조건
  ├─ 공유메모리 Sleep(1) 폴링 — 지연
  └─ 큐 Full 시 유실 — 조용한 데이터 손실
```

| 증상 | DLL 원인 | PLC_SCAN 원인 |
|------|---------|--------------|
| **이벤트 소실** | bSendDataPacket 조기 리턴 (전송 거짓말) | bUseNewValueOnAnalogOut |
| **이벤트 중복** | saveBuf 미초기화, TNS 미검증 | — |
| **Sleep 필수** | nBlockNo 리셋 타이밍, bSendDataPacket 리셋 대기 | 1사이클 1쓰기 |
| **Sleep 크기 다름** | 다중 블록 패킷 크기 차이 | — |
| **UI Hang** | checkWaitOkSignal 블로킹 | 메인 스레드 Sleep |
| **연결 끊김** | checkWaitOkSignal 중 이벤트 적체 | — |

---

## 10. 종합 해결 방안

### PLC_SCAN 본체 수정 (DLL 변경 없음)

| 순서 | 개선 | 수정 위치 | 수정량 | 효과 |
|------|------|-----------|--------|------|
| **1** | **RunWriteWait()에서 N건 연속 처리** | `Scanstat.cpp:1760` | ~5줄 | 쓰기 처리량 N배 증가 |
| **2** | **bUseNewValueOnAnalogOut 기본값 OFF** | config 초기화 | 1줄 | 값 덮어쓰기 방지 |
| **3** | **char → InterlockedExchange** | `Scanstat.cpp:1477-1507` | ~10줄 | 경쟁 조건 제거 |
| **4** | **공유메모리 폴링 → Event 기반** | `Scanstat.cpp:1807-1812` | ~5줄 | 즉시 반응 |
| **5** | **큐 Full 시 대기/확장** | `Scanstat.cpp:1590` | ~5줄 | 유실 방지 |

### SECS Host DLL 수정

| 순서 | 개선 | 수정 위치 | 수정량 | 효과 |
|------|------|-----------|--------|------|
| **A** | **이벤트 큐 추가** (recvBuf 링 버퍼화) | `SECS_HostDef.h` | ~30줄 | 이벤트 덮어쓰기 방지 |
| **B** | **saveBuf memset(0) 추가** | `SECS_Host.cpp:449` | 1줄 | 이전 데이터 잔존 방지 |
| **C** | **nBlockNo 즉시 리셋** | `SECS_Host.cpp:858` | ~3줄 | Sleep 의존 제거 |
| **D** | **checkWaitOkSignal 비동기화** | `SECS_Host.cpp:805` | ~20줄 | 블로킹 대기 제거 |
| **E** | **응답 매칭에 TNS 비교 추가** | `SECS_Hsms.cpp:210` | ~3줄 | 요청-응답 정확 매칭 |
| **F** | **setInitRecvStatus 호출 시점 조정** | `SECS_Hsms.cpp:268,275` | ~5줄 | 파이프라인 응답 유실 방지 |

### 적용 우선순위

```
[1순위: PLC_SCAN 본체 — 모든 프로토콜에 효과]
  1번(연속 처리) + 2번(덮어쓰기 OFF)
  → SECS 뿐 아니라 MELSEC, OMRON 멀티출력도 개선

[2순위: SECS Host DLL — SECS/GEM 전용]
  B번(saveBuf 초기화) + C번(nBlockNo 리셋)
  → 1줄+3줄 수정으로 Sleep 의존 대폭 감소

[3순위: 근본 해결]
  A번(이벤트 큐) + D번(비동기 대기)
  → 시간당 수천 건 이벤트 무손실 처리
```
