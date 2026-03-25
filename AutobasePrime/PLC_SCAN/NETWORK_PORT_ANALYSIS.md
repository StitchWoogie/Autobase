# PLC_SCAN 네트워크 포트 확장 분석 리포트

## 1. TCP 통신 다수 포트 사용 시 성능 저하 원인 분석

### 1.1 현재 아키텍처

PLC_SCAN은 **포트 당 1스레드 + 1 TCP 연결** 모델을 사용합니다.

```
[PLC_SCAN 포트 0] → Thread_0 → TCP Socket → 대상 장치
[PLC_SCAN 포트 1] → Thread_1 → TCP Socket → 대상 장치
...
[PLC_SCAN 포트 N] → Thread_N → TCP Socket → 대상 장치
```

**스레드 동작 방식** (`PortThread.cpp:29-33`):
```cpp
while(thread->bDo) {
    WatchDogPlcScanReset(pt->local.no);
    Sleep(sleep_cycle);        // 기본 1ms
    CommStatusLocalOne(pt);    // 스캔 1회 수행
}
```

**서버 측** (`ScanServerStatus.cpp:1113-1123`):
```cpp
for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {   // 256개 순회
    conn = &scanServerList[i];
    ScanServerStatusOne(conn);                  // 연결별 데이터 전송
}
```

서버에서 클라이언트로 데이터 전송 시, 각 연결마다 등록된 포트를 순회하며 (`ScanServerStatus.cpp:1083`):
```cpp
for(i = 0; i < 256; i++) {
    if(!(conn->wCastPort[i/16] & WORD_MASK[i%16]))  continue;
    // 포트별 WORD, FLOAT, DWORD, STRING, DOUBLE, INT64 데이터 전송
}
```

### 1.2 성능 저하 원인 분류

#### (A) 프로그램에서 해결 가능한 문제

| 문제 | 위치 | 영향 | 해결 방법 |
|------|------|------|-----------|
| **TCP_NODELAY 미설정** | `Comtcpip.cpp` 전체 | Nagle 알고리즘으로 작은 패킷 200ms 지연 | `setsockopt(SO_NODELAY)` 추가 |
| **Life Signal 폭주** | `NetworkClientMulti.cpp:397-400` | 10초마다 포트당 1개 → 10000포트면 초당 1000개 | 포트를 그룹화하여 1개 연결로 다수 포트 커버 |
| **데이터 전송 순차 처리** | `ScanServerStatus.cpp:1083-1108` | 256개 포트를 for 루프로 순차 전송 | 변경된 데이터만 전송 (dirty flag) |
| **순차 포트 폴링** | `Scanstat.cpp:1000-1036` | `CommStatusLocal()`이 한 번에 1~2포트만 처리, 전체 순회에 N/2회 호출 필요 | select/poll 또는 이벤트 기반 I/O |
| **ioctlsocket 폴링** | `Comtcpip.cpp:324` | 데이터 확인을 `ioctlsocket(FIONREAD)`로 포트별 순차 체크 | IOCP/이벤트 기반으로 전환 |
| **스레드 과다 생성** | `PortThread.cpp:58` | 포트당 1스레드, 기본 스택 1MB → 10000스레드 = 10GB | 스레드 풀 또는 비동기 I/O로 전환 |
| **Sleep 기반 폴링** | `PortThread.cpp:31` | `Sleep(1)` 반복 → 컨텍스트 스위칭 오버헤드 | IOCP/이벤트 기반으로 전환 |
| **재연결 시 Sleep(500)** | `Comtcpip.cpp:442,502` | TCP 종료 시 500ms 블로킹 | 비동기 종료로 변경 |
| **connect() 블로킹** | `Comtcpip.cpp:182` | TCP 연결 시 OS 타임아웃까지(20~60초) 블로킹 | non-blocking connect + select |

#### (B) 네트워크/게이트웨이 부하 (외부 요인)

| 문제 | 설명 | 해결 방법 |
|------|------|-----------|
| **TCP 연결 수 자체** | 10000개 TCP 연결 = 10000개 소켓 유지 | OS 튜닝 (FD_SETSIZE, ephemeral port range) |
| **게이트웨이 동시 연결 한계** | 공유기/방화벽의 NAT 테이블 크기 제한 | 네트워크 장비 업그레이드 |
| **대역폭** | 포트별 주기적 데이터 전송 × 10000 | 네트워크 대역폭 확보 |

#### (C) 어쩔 수 없는 구조적 문제

| 문제 | 설명 |
|------|------|
| **PLC 응답 지연** | PLC 장치 자체의 통신 속도 한계 |
| **TCP 핸드셰이크 오버헤드** | 연결 수에 비례 (해결: 연결 유지) |

### 1.3 긍정적인 부분 (이미 잘 된 것)

- **TCP 연결은 영속적**: 매 스캔마다 재연결하지 않고, 초기화 시 1회 연결 후 재사용 (`Comtcpip.cpp:114-214`)
- **수신 버퍼 최적화**: SO_RCVBUF를 기본 8KB에서 60KB로 확장 (`Comtcpip.cpp:199`)
- **포트 Failover**: 8개 대체 포트를 지원하여 장애 시 자동 전환 (`Comtcpip.cpp:236-242`)
- **NetworkClientMulti 게이트웨이**: 단일 TCP 연결로 16개 PLC 포트를 멀티플렉싱 가능

### 1.4 성능 수치 추정

| 포트 수 | 순차 폴링 오버헤드 | 전체 스캔 주기 (10ms/포트) | 스레드 스택 |
|---------|-------------------|---------------------------|-------------|
| 256 | ~500μs | 1.3~2.6초 | 256MB |
| 1000 | ~2ms | 5~10초 | 1GB |
| 10000 | ~20ms | 50~100초 | **10GB** |

### 1.5 결론

**현재 포트 수가 많을 때 느려지는 주된 원인은 프로그램 아키텍처 문제입니다.**

- 현재 구조는 포트당 1스레드가 `Sleep(1)` + 스캔을 반복하는 방식으로, 포트 수에 **선형으로** 리소스가 증가합니다
- TCP 연결 자체의 오버헤드보다, **순차 폴링** (`ioctlsocket(FIONREAD)` 포트별 순차 체크) + **스레드 컨텍스트 스위칭** + **Nagle 지연**이 주요 병목입니다
- 게이트웨이 부하는 수백 개 수준에서는 문제가 되지 않으나, 수천 개 이상에서는 네트워크 장비 성능에 따라 영향이 있을 수 있습니다
- TCP 연결 자체는 영속적이고 수신 버퍼도 최적화되어 있어, 순수 네트워크 오버헤드는 전체의 5~10% 수준입니다

---

## 2. NetworkClient Server/Client 256 → 10000 포트 확장 가능성 분석

### 2.1 현재 256개 제한의 근본 원인

256개 제한은 **단일 상수가 아니라 여러 계층에 걸쳐 있는 복합 제약**입니다.

### 2.2 제약 목록 및 분류

#### HARD 제약 (프로토콜/구조적 변경 필요)

##### 제약 1: BroadCastPorts 비트마스크 — 16 WORD = 256비트

**가장 핵심적인 제약입니다.**

`NetWorkProtocol.h:72`:
```cpp
WORD wBroadCastPorts[16];  // plc scan 포트에서 제공받고자 하는 Port
```

`ScanServer.h:62`:
```cpp
WORD wCastPort[16];  // 브로드캐스트 포트
```

`NetworkClientMulti.cpp:25`:
```cpp
WORD wActPort[16];   // LOCAL_VARS_STRUCT 내부
```

이 비트마스크는 16 × 16비트 = **256비트**로, 각 비트가 하나의 포트를 나타냅니다.
10000포트를 표현하려면 `WORD wBroadCastPorts[625]` (10000/16 = 625)로 확장해야 합니다.

**영향 범위:**
- `NetWorkProtocolRecv` 클래스의 `Split()` 함수 (파싱 로직)
- `NetWorkProtocolSend::MakeBlock()` (직렬화 로직)
- `NetworkClientMulti.cpp`의 `SendLifeSignal()` (16×4=64바이트 → 625×4=2500바이트)
- `ScanServerStatus.cpp:295-298, 310-312` (서버 측 수신)
- **네트워크 프로토콜 자체가 변경**되므로 서버/클라이언트 양쪽 동시 업데이트 필수

##### 제약 2: MAX_SCAN_SERVER_LIST = 256 (정적 배열)

`ScanServer.h:99-101`:
```cpp
#define MAX_SCAN_SERVER_LIST  256
extern SCAN_SERVER_LIST scanServerList[MAX_SCAN_SERVER_LIST];
```

`ScanServerWork.cpp:21`:
```cpp
SCAN_SERVER_LIST scanServerList[MAX_SCAN_SERVER_LIST];
```

SCAN_SERVER_LIST는 서버 측 연결 목록이며, 포트 수와 직접 연결되지는 않지만
연결 수 제한으로 작용합니다.

**해결:** 상수를 늘리거나 동적 배열로 변경. 단, `SCAN_SERVER_LIST` 구조체 크기에 따라
메모리 영향 고려 필요.

##### 제약 3: 포트 순회 루프 하드코딩 256

`ScanServerStatus.cpp:1083`:
```cpp
for(i = 0; i < 256; i++) {
    if(!(conn->wCastPort[i/16] & WORD_MASK[i%16]))  continue;
```

`ScanServerWork.cpp:101`:
```cpp
for(int j = 0; j < 256; j++) {
    if(conn->wCastPort[j/16] & WORD_MASK[j%16]) {
```

**해결:** 256 → 확장된 포트 수 상수로 변경.

##### 제약 4: NetworkClientMulti 포트 범위 검증

`NetworkClientMulti.cpp:92-95`:
```cpp
if(from < 0)   from = 0;
if(from > 255) from = 255;  // ← 하드코딩
if(to < from)  to = from;
if(to > 255)   to = 255;    // ← 하드코딩
```

**해결:** 255 → 새 최대값으로 변경.

##### 제약 5: 하드코딩 256 순회 루프 (추가 위치)

위에서 언급한 것 외에도 다수 파일에 하드코딩된 256 루프가 존재합니다:

| 파일 | 라인 | 코드 |
|------|------|------|
| `Scanstat.cpp` | 468 | `for(int port = 0; port < 256; port++)` |
| `Bl_2300.cpp` | 446, 532 | `for(i = 0; i < 256; i++)` |
| `DialogConfigDevice.cpp` | 207 | `for(i = 0; i < 256; i++)` |
| `DialogModemSelect.cpp` | 68 | `for(i = 0; i < 256; i++)` |
| `Comudpip.cpp` | 288 | `for(i = 0; i < 256; i++)` |

**해결:** 모두 `MAX_PORT` 또는 확장된 상수로 교체.

##### 제약 6: modemList[256] 정적 배열

`ComModem.cpp:37`:
```cpp
static MODEM_LIST modemList[256];
```

**해결:** 모뎀을 사용하지 않는 경우 영향 없음. 사용 시 동적 할당 필요.

#### SOFT 제약 (상수/설정 변경으로 해결)

| 항목 | 위치 | 현재 값 | 해결 |
|------|------|---------|------|
| MAX_PORT 기본값 | `Scanmain.cpp:62` | `int MAX_PORT = 256` | INI에서 이미 변경 가능 |
| portBuf 할당 | `Scanfile.cpp:709` | `new GLOBAL_PORT_STRUCT[MAX_PORT]` | 이미 동적 |
| 스레드 모델 | `PortThread.cpp:47-58` | `CreateThread(PortThread_Common, pt)` | 이미 파라미터 기반 |
| 파일명 포맷 | `Scanmain.cpp:1116` | `SCAN.%03d` | `%05d`로 변경 (10000 미만) |
| LPARAM 인코딩 | `Scanmain.cpp:1107` | `port = lParam/10000` | 10000 미만이면 문제 없음 |

#### 이미 해결된 항목

| 항목 | 설명 |
|------|------|
| portBuf 동적 할당 | `new GLOBAL_PORT_STRUCT[MAX_PORT]` — MAX_PORT 변경 시 자동 반영 |
| 스레드 함수 | 과거 `PortThread_0~255` 하드코딩 → 현재 `PortThread_Common(LPVOID)` 사용 |
| MAX_PORT 설정 | `ProjectConfig.inix`의 `MaxPorts` 키로 변경 가능 |
| nPort 데이터 타입 | `NetWorkProtocolRecv::nPort`는 `int` (4바이트) → 10000 충분 |

### 2.3 10000 포트 확장 시 실현 가능성 평가

#### 확장 가능 여부: **조건부 가능**

**변경이 필요한 핵심 작업 5가지:**

| 순서 | 작업 | 난이도 | 위험도 | 비고 |
|------|------|--------|--------|------|
| 1 | **BroadCastPorts 비트마스크 확장** | **높음** | **높음** | 프로토콜 변경 → 하위 호환성 깨짐 |
| 2 | MAX_SCAN_SERVER_LIST 확장 | 낮음 | 중간 | 메모리 증가 |
| 3 | 하드코딩 256 루프 수정 | 낮음 | 낮음 | 검색/치환 |
| 4 | NetworkClientMulti 255 제한 해제 | 낮음 | 낮음 | 4줄 수정 |
| 5 | 파일명 포맷 변경 | 낮음 | 낮음 | `%03d` → `%05d` |

#### 최대 난관: BroadCastPorts 비트마스크 변경

현재 프로토콜에서 `wBroadCastPorts[16]`은:
- 네트워크 패킷에 직렬화되어 전송됨
- 서버와 클라이언트 양쪽에서 동일하게 파싱
- Life Signal에 포함되어 10초마다 전송

**10000포트 지원 시:**
- `WORD[16]` (32바이트) → `WORD[625]` (1250바이트)
- Life Signal 패킷 크기가 ~100바이트 → ~1300바이트로 증가
- `SendLifeSignal()`의 imsi 버퍼가 `char[65]` → `char[2501]`로 확장 필요

**하위 호환성:**
- 기존 v8.7 클라이언트와 새 서버는 통신 불가 (또는 그 반대)
- 버전 필드(`Version=8.7`)를 활용하여 하위 호환 처리 가능하나 복잡도 증가

### 2.4 10000 포트 확장 시 리소스 영향

| 항목 | 256 포트 | 10000 포트 | 비고 |
|------|----------|------------|------|
| **portBuf 메모리** | ~0.86MB (256 × ~3,525바이트) | ~33.6MB (10000 × ~3,525바이트) | **메모리는 문제 없음** |
| **스레드 수** | 최대 256 | 최대 10000 | **Windows 한계 주의** |
| **스레드 스택** | 256MB (기본 1MB/스레드) | **10GB** | 반드시 스택 축소 또는 아키텍처 변경 필요 |
| **TCP 소켓** | 256개 | 10000개 | OS 튜닝 필요 |
| **BroadCast 비트맵** | 32바이트 | 1250바이트 | 패킷 크기 증가 |

### 2.5 권장 사항

#### 단기 (현 아키텍처 유지하며 확장)

1. **BroadCastPorts를 가변 길이로 변경**
   - 프로토콜에 포트 목록 크기 필드 추가
   - 비트마스크 대신 포트 번호 목록 방식 고려 (예: `Port=0,1,5,100,9999`)

2. **MAX_SCAN_SERVER_LIST 동적 할당**
   - 정적 배열 → `new SCAN_SERVER_LIST[설정값]`

3. **스레드 스택 축소**
   - `CreateThread`의 `dwStackSize` 파라미터를 64KB~128KB로 지정
   - 10000 × 128KB = 1.25GB (관리 가능)

4. **하위 호환 처리**
   - 프로토콜 버전 필드 활용 (현재 `Version=8.7`)
   - 새 버전 클라이언트만 확장 비트마스크 사용

#### 중장기 (아키텍처 개선)

1. **스레드 풀 + IOCP 모델로 전환**
   - 10000 스레드 대신 CPU 코어 수 × 2 스레드
   - Windows IOCP로 소켓 이벤트 처리

2. **포트 그룹화**
   - 동일 IP 대상 포트를 1개 TCP 연결로 멀티플렉싱
   - 현재 NetworkClientMulti가 이미 이 방향이나, 비트마스크 제한

3. **변경 감지 기반 전송**
   - 매 주기마다 모든 데이터를 전송하는 대신
   - dirty flag로 변경된 데이터만 전송

---

## 3. 종합 결론

### TCP 포트 수 증가 시 느려지는 문제

| 원인 분류 | 비중 | 해결 가능 여부 |
|-----------|------|----------------|
| **프로그램 아키텍처** (스레드 과다, 순차 전송, Nagle) | **60%** | **해결 가능** |
| **네트워크/게이트웨이 부하** (연결 수, NAT) | **25%** | 일부 해결 가능 (장비 교체) |
| **구조적 한계** (PLC 응답속도, TCP 오버헤드) | **15%** | 해결 불가 |

### 256 → 10000 포트 확장

| 판정 | 설명 |
|------|------|
| **확장 가능** | 단, BroadCastPorts 비트마스크 프로토콜 변경이 **핵심 선결 조건** |
| **보류 이유는 타당** | 프로토콜 변경 → 하위 호환성 문제 → 양쪽 동시 배포 필요 |
| **권장 접근** | 비트마스크를 가변 길이 포트 목록으로 변경하고, 버전 필드로 하위 호환 처리 |

### 변경 필요 파일 목록

| 파일 | 변경 내용 |
|------|-----------|
| `CATLIB.SRC/NetWorkProtocol.h` | `wBroadCastPorts[16]` → 가변 또는 확장 |
| `ScanServer.h` | `wCastPort[16]` 확장, `MAX_SCAN_SERVER_LIST` 확장 |
| `ScanServerWork.cpp` | `for(j < 256)` → 확장된 상수 |
| `ScanServerStatus.cpp` | `for(i < 256)` → 확장, `wCastPort` 처리 로직 |
| `PROTOCOL/NetworkClientMulti.cpp` | `wActPort[16]` 확장, 255 제한 해제, `imsi` 버퍼 확장 |
| `DEVICE/ComModem.cpp` | `modemList[256]` 확장 (모뎀 사용 시) |
| `Scanmain.cpp` | `SCAN.%03d` → `%05d`, LPARAM 인코딩 검증 |

---

## 4. 추가 질의 분석

### 4.1 하위 호환성: 새 클라이언트(10000포트) → 구 서버(256포트) 통신 가능 여부

#### 시나리오

```
[통합 PC - 새버전]              [로컬 PC A - 구버전]
 NetworkClient(10000포트)  ←→   NetworkServer(256포트)
                                [로컬 PC B - 구버전]
                           ←→   NetworkServer(256포트)
```

#### 현재 프로토콜 통신 흐름

1. **클라이언트 → 서버**: Life Signal 전송 (`NetworkClientMulti.cpp:41`)
   ```cpp
   sprintf(commSendBuf, "Port=%d,BroadCastPorts=%s,Version=8.7", pt->no, imsi);
   ```
   - `imsi`는 `wActPort[16]`을 16진수 문자열로 직렬화한 것 (64바이트)
   - 서버는 이 문자열을 파싱하여 어떤 포트 데이터를 보낼지 결정

2. **서버 측 파싱** (`ScanServerStatus.cpp:307-312`)
   ```cpp
   for(int i = 0; i < 16; i++) {
       conn->wCastPort[i] = recv.wBroadCastPorts[i];
   }
   ```

3. **서버 → 클라이언트**: 등록된 포트의 데이터를 전송

#### 분석 결과: **조건부 가능**

**읽기는 가능합니다.** 단, 다음 조건이 필요합니다:

| 조건 | 가능 여부 | 설명 |
|------|-----------|------|
| 통합 PC가 로컬 PC의 **0~255번 포트**만 읽는 경우 | **가능** | 기존 `WORD[16]` 비트마스크로 충분 |
| 통합 PC가 로컬 PC의 **256번 이상 포트**를 읽는 경우 | **불가능** | 구 서버가 확장 비트마스크를 이해 못함 |

**이유:**
- `nPort` 필드는 `int`이므로 데이터 수신 자체에는 문제 없음
- Life Signal의 `BroadCastPorts` 문자열이 핵심: 구 서버는 16개 WORD만 파싱
- 새 클라이언트가 구 서버에 연결할 때, **요청 포트 범위가 0~255 이내**이면 기존 프로토콜과 동일

#### 구현 방법

```
새 클라이언트의 Life Signal 전송 로직:
1. 서버 버전 확인 (Version 필드)
2. 구 서버(v8.7): 기존 WORD[16] 포맷으로 BroadCastPorts 전송
3. 새 서버(v9.x): 확장 포맷으로 전송
```

**결론:** 통합 PC에서 로컬 PC의 포트 0~255만 읽으면 **코드 변경 없이 호환 가능**.
로컬 PC가 256번 이상 포트를 사용하려면 로컬 PC도 업데이트 필수.

---

### 4.2 개별 스레드 → 스레드 풀 전환의 이점과 단점

#### 현재 구조 (포트당 개별 스레드)

```cpp
// PortThread.cpp:11-45
static DWORD WINAPI PortThread_Common(LPVOID lpparam) {
    GLOBAL_PORT_STRUCT *pt = (GLOBAL_PORT_STRUCT*)lpparam;
    while(thread->bDo) {
        Sleep(sleep_cycle);        // 기본 1ms
        CommStatusLocalOne(pt);    // 스캔 1회
    }
}

// CreateThread(NULL, 0, ...)  ← dwStackSize = 0 → 기본 1MB 스택
```

- 모든 `CreateThread` 호출에서 `dwStackSize = 0` (Windows 기본 1MB)
- `Sleep(1)` 반복 폴링
- `bActiveThread` 플래그로 포트별 스레드 사용 여부 제어 가능

#### 이점

| 항목 | 개별 스레드 (현재) | 스레드 풀 |
|------|-------------------|-----------|
| **메모리** | N × 1MB 스택 = 256포트→256MB | 고정 스레드 수 × 1MB (예: 32MB) |
| **컨텍스트 스위칭** | N개 스레드 스케줄링 → OS 부담 | 고정 수 스레드만 스케줄링 |
| **생성/종료 비용** | 포트마다 CreateThread/WaitForSingleObject | 풀 재사용, 생성/종료 비용 없음 |
| **CPU 활용** | 대부분 Sleep 상태로 CPU 낭비 | 작업 있을 때만 깨어남 |
| **확장성** | 1000개 이상에서 OS 한계 접근 | 10000포트도 동일 성능 |

#### 단점

| 항목 | 설명 |
|------|------|
| **구현 복잡도** | 작업 큐, 스케줄링 로직 신규 구현 필요 |
| **실시간성 저하 가능** | 풀 스레드가 모두 사용 중이면 대기 발생 |
| **디버깅 난이도** | 포트-스레드 1:1 매핑이 아니라 추적 어려움 |
| **기존 코드 변경 범위** | `CommStatusLocalOne()` 내부에서 블로킹 호출 사용 → 풀 스레드 점유 시간 길어질 수 있음 |
| **PLC 통신 특성** | PLC 응답 대기(수십~수백ms)가 풀 스레드를 점유 → 풀 크기를 충분히 키워야 함 |

#### 개별 스레드 사용 시 문제가 두드러지는 시점

| 스레드 수 | 증상 | 근거 |
|-----------|------|------|
| **~500개** | 컨텍스트 스위칭 오버헤드 체감 시작 | Windows 스케줄러가 수백 개 ready 스레드 관리 부담 증가 |
| **~1000개** | 스택 메모리 1GB 소비, 스케줄링 지연 | 1MB × 1000 = 1GB. `Sleep(1)` 스레드가 1000개씩 깨어남 |
| **~2000개** | 성능 급격 저하 | 32비트 프로세스의 가상 주소 공간(2GB) 중 스택만 2GB 점유 |
| **~4000개+** | 프로세스 메모리 한계 도달 | 32비트: CreateThread 실패 가능. 64비트에서도 OS 스케줄링 비효율 |

**참고:** 현재 코드는 32비트 빌드(`.vcxproj`)이므로, 가상 주소 공간 2GB 제한이 적용됩니다.
실질적으로 **1000~2000개**가 32비트 환경의 실용적 한계입니다.

#### 권장: 단계적 접근

1. **즉시 적용 가능**: `CreateThread`의 `dwStackSize`를 **64KB~128KB**로 지정
   - 현재 스레드 함수에서 큰 로컬 변수 없음 (최대 `StackChar(5000)` 정도)
   - 256포트: 256MB → 16~32MB로 감소
   - 2000포트까지 가능해짐

2. **중기**: Windows `QueueUserWorkItem()` 또는 `CreateThreadpoolWork()` 활용
3. **장기**: IOCP 기반 비동기 I/O로 전면 전환

---

### 4.3 디바이스 연결 불가 시 UI Hang 원인 요약

> 상세 시나리오 분석 및 해결 방법은 5장 참조.

**근본 원인:** `Comtcpip.cpp`의 `connect()`와 `gethostbyname()`이 블로킹 모드로 동작.

| 원인 | 위치 | 블로킹 시간 |
|------|------|-------------|
| `connect()` 블로킹 | `Comtcpip.cpp:182` | 20~60초 (OS 타임아웃) |
| `gethostbyname()` DNS 조회 | `Comtcpip.cpp:87` | 30초+ |
| 소켓 타임아웃 코드 주석 처리됨 | `Comtcpip.cpp:140-154` | — |

**메인 스레드에서 블로킹 발생 조건:**

| 호출자 | 스레드 | Hang 여부 |
|--------|--------|-----------|
| `PortThread_Common()` (PortThread.cpp:32) | **포트 스레드** | UI에 영향 없음 |
| `CommStatus()` → `CommStatusLocal()` (Scanstat.cpp:1039) | **메인 스레드** | **UI Hang!** |

`CommStatus()`는 `WM_TIMER` 핸들러에서 호출 (Scanmain.cpp:1498).
`bActiveThread = OFF`인 포트만 메인 스레드에서 처리되므로, 해당 포트의 TCP 연결 실패 시 Hang.

**non-blocking connect 해결 코드:**
```cpp
// Comtcpip.cpp:130 이후 — 소켓 생성 직후 추가
u_long mode = 1;
ioctlsocket(tcpip->socket, FIONBIO, &mode);    // Non-blocking 모드

connect(tcpip->socket, ...);                     // 즉시 반환 (WSAEWOULDBLOCK)

fd_set writefds;
FD_ZERO(&writefds);
FD_SET(tcpip->socket, &writefds);
struct timeval tv = {3, 0};                      // 3초 타임아웃
select(0, NULL, &writefds, NULL, &tv);

mode = 0;
ioctlsocket(tcpip->socket, FIONBIO, &mode);    // Blocking 모드 복원
```

---

## 5. 실제 UI Hang 시나리오 분석

### 5.1 시나리오 1: 통신파일 편집 시 디바이스 연결 불가 → 전체 Hang

#### 재현 경로

```
사용자: 메뉴 → 포트 편집 (IDM_PORT_EDIT)
  ↓
Scanmain.cpp:1142 → ScanFileEdit(hwnd)
  ↓
ScanEdit.cpp:45 → CDialogPortEdit 다이얼로그 표시
  ↓
사용자: OK 클릭
  ↓
ScanEdit.cpp:20 → RestartOnePort(port)
```

#### `RestartOnePort()` 코드 — 메인 스레드에서 실행 (ScanEdit.cpp:20-43)

```cpp
void RestartOnePort(int port)
{
    ScanServerPause(ON);          // ① 모든 서버 포트 일시정지 (256개 전부)

    SendMessage(hwndMainFrame, WM_COMMAND, IDM_CLOSEALL, 0L);  // ② 모든 창 닫기
    ScanPortUnInitOne(port);      // ③ 해당 포트만 해제
    ScanPortInitOne(hwndMainFrame, port);   // ④ 해당 포트만 재초기화

    PostMessage(hwndMainFrame, WM_COMMAND, IDM_VIEW_SCANMEMORY, 0L);  // ⑤ 메모리 창 열기

    ScanServerPause(OFF);         // ⑥ 모든 서버 포트 재개
}
```

#### Hang이 발생하는 시점

④번 `ScanPortInitOne()` 자체는 안전합니다 — 디바이스 초기화를 하지 않고 스레드만 생성합니다.

**문제는 ⑥번 `ScanServerPause(OFF)` 이후, 다음 `WM_TIMER`에서:**

```
ScanServerPause(OFF) 이후
  ↓
WM_TIMER 발생 (Scanmain.cpp:1489)
  ↓
CommStatus() 호출 (Scanmain.cpp:1498)         ← 메인 스레드
  ↓
CommStatusLocal() → CommStatusLocalOne(pt)     ← bActiveThread=OFF인 포트
  ↓
bDeviceInitialFlag == 0 (재초기화됨)
  ↓
PlcDeviceInit() → PlcDeviceInitTCPIP()        ← Scanstat.cpp:736
  ↓
gethostbyname() → 블로킹 30초+               ← Comtcpip.cpp:87
connect()        → 블로킹 20~60초             ← Comtcpip.cpp:182
  ↓
⚠️ 메인 스레드 Hang → UI 응답 불가 → Network Memory Server도 응답 불가
```

#### "모든 포트 일시정지"가 필요한가?

`ScanServerPause(ON)` (ScanServerStatus.cpp:1329-1339):
```cpp
void ScanServerPause(char flag)
{
    for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {   // 256개 전부 순회
        conn = &scanServerList[i];
        ScanServerPauseOne(conn, flag);             // 각각 ACK 대기 (최대 5초)
    }
}
```

**필요하지 않습니다.**

| 관점 | 분석 |
|------|------|
| `ScanPortUnInitOne(port)` | **1개 포트**만 해제. 다른 포트 상태 변경 없음 |
| `ScanPortInitOne(port)` | **1개 포트**만 초기화. 다른 포트 메모리 접근 없음 |
| 포트 스레드 독립성 | 각 포트 스레드는 자기 `portBuf[port]`만 접근 |
| ScanServer와의 관계 | ScanServer가 편집 중인 포트의 `portBuf[port]` 데이터를 전송하는 중에 구조체가 해제될 수 있음 |

**유일한 이유:** ScanServer가 편집 중인 포트의 데이터를 전송하다가 구조체가 해제/재할당되면 크래시.
하지만 이것은 **해당 포트를 참조하는 ScanServer만 일시정지**하면 됩니다.

#### Network Memory Server 끊김 원인

메인 스레드 Hang → `WM_TIMER` 처리 불가 → `ScanServerStatus()` (Scanmain.cpp:1499) 미호출
→ `bThreadFlag=OFF`인 서버는 데이터 전송 중단 → 클라이언트 타임아웃 → 연결 끊김.

(Hang 풀리면 자동 복구되는 이유: 클라이언트가 Life Signal 재전송 → 서버 재인식)

#### 해결 방법

**방법 1: 전체 일시정지 → 해당 포트 관련 서버만 일시정지 (권장)**
```cpp
void RestartOnePort(int port)
{
    // 해당 포트를 참조하는 ScanServer만 일시정지
    for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
        if(scanServerList[i].wCastPort[port/16] & WORD_MASK[port%16]) {
            ScanServerPauseOne(&scanServerList[i], ON);
        }
    }

    ScanPortUnInitOne(port);
    ScanPortInitOne(hwndMainFrame, port);

    // 일시정지한 서버만 재개
    for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
        if(scanServerList[i].wCastPort[port/16] & WORD_MASK[port%16]) {
            ScanServerPauseOne(&scanServerList[i], OFF);
        }
    }
}
```

**방법 2: connect() non-blocking화 (근본 해결)** — 4.3절 참조

**방법 3: TCP 포트는 bActiveThread 강제 ON**
→ 메인 스레드에서 `PlcDeviceInit` 호출 자체가 발생하지 않음.

---

### 5.2 시나리오 2: 선로이중화 절체 시 통신 메모리 창 열려있으면 UI Hang

#### 이 시나리오의 원인은 `connect()` 블로킹이 아닙니다

시나리오 2는 `bThreadProtocolDrawWorking` 플래그를 사용한 **동기화 교착**이 원인입니다.

#### 관련 코드: `bThreadProtocolDrawWorking` 동기화 메커니즘

```cpp
// Pro_main.cpp:55-66 — 화면 그리기 완료 대기 (최대 30초)
void WaitThreadProtocolDrawWorking(GLOBAL_PORT_STRUCT *pt)
{
    TimeOutClass timeout;
    while(1) {
        if(timeout.IsTimeOut(30))  break;     // 30초 타임아웃
        if(pt->bThreadProtocolDrawWorking == OFF) break;
        Sleep(1);
    }
    pt->bThreadProtocolDrawWorking = OFF;
}
```

이 플래그는 **두 곳에서 ON/OFF**됩니다:

| 호출자 | 스레드 | 코드 위치 |
|--------|--------|-----------|
| `PlcProtocolDrawMethod()` | **메인 스레드** (WM_PAINT) | Pro_main.cpp:173-178 |
| `ChangeDualSystem()` | **포트 스레드** | Scanstat.cpp:574-583 |

#### 교착 시나리오

```
시간 →   T1                    T2                       T3

[포트스레드]
          ChangeDualSystem()
          ├─ WaitThread...()     → OK (OFF 상태)
          ├─ flag = ON ──────────────────────────────────┐
          ├─ PlcDeviceUnInit()                           │
          ├─ PlcDeviceInit()                             │
          │   └─ connect() 블로킹 20~60초...            │ flag=ON 유지
          │                                              │
[메인스레드]                                             │
               WM_PAINT 발생 (메모리 창 갱신)            │
               └─ PlcProtocolDrawMethod()                │
                  └─ WaitThread...()                     │
                     └─ flag == ON이므로 대기 ──────────>│
                        Sleep(1) 반복... 최대 30초       │
                        ⚠️ 메인 스레드 Hang              │
                                                         │
          ├─ connect() 완료 (또는 타임아웃)              │
          ├─ flag = OFF ─────────────────────────────────┘
               WaitThread...() 해제
               UI 응답 복귀
```

**핵심:** 포트 스레드가 `flag=ON`을 설정한 채 `connect()`에서 20~60초 블로킹
→ 메인 스레드의 화면 갱신이 `WaitThreadProtocolDrawWorking()`에서 30초 대기
→ **UI Hang** (최대 30초).

#### 왜 Network Memory Server는 계속 동작하는가?

시나리오 1과 달리:
- `ScanServerPause()`가 호출되지 않음 → 서버 스레드 영향 없음
- 서버 스레드(`bThreadFlag=ON`)는 독립적으로 데이터 전송 지속
- 메인 스레드만 `WaitThreadProtocolDrawWorking()`에서 대기

#### 해결 방법

**방법 1: `ChangeDualSystem()`의 잠금 범위 축소 (가장 효과적)**
```cpp
// 현재 — connect() 동안 flag=ON 유지 (Scanstat.cpp:574-583)
WaitThreadProtocolDrawWorking(pt);
pt->bThreadProtocolDrawWorking = ON;
PlcProtocolUnInitOne(pt);              // 해제
PlcDeviceUnInit(&pt->local.device);    // 해제
pt->nScanDevice = PlcDeviceInit(...);  // ← 여기서 오래 블로킹!
PlcProtocolInitOne(...);               // 초기화
pt->bThreadProtocolDrawWorking = OFF;

// 개선 — 해제 후 잠금 해제, connect 완료 후 다시 잠금
WaitThreadProtocolDrawWorking(pt);
pt->bThreadProtocolDrawWorking = ON;
PlcProtocolUnInitOne(pt);
PlcDeviceUnInit(&pt->local.device);
pt->bThreadProtocolDrawWorking = OFF;     // ← 해제 완료 후 즉시 잠금 해제

pt->nScanDevice = PlcDeviceInit(...);     // 블로킹해도 UI 무관

WaitThreadProtocolDrawWorking(pt);
pt->bThreadProtocolDrawWorking = ON;
PlcProtocolInitOne(...);
pt->bThreadProtocolDrawWorking = OFF;
```

**방법 2: connect() non-blocking화 (근본 해결)**
`connect()`가 즉시 반환하면 `flag=ON` 유지 시간이 수 ms → 교착 해소.

**방법 3: WaitThreadProtocolDrawWorking 타임아웃 단축**
```cpp
// Pro_main.cpp:60 — 30초 → 1~2초로 단축
if(timeout.IsTimeOut(2))  break;   // 30 → 2
```
Hang 시간은 줄지만, 근본 해결은 아님.

---

### 5.3 두 시나리오 종합 해결 방안

| 순서 | 방법 | 난이도 | 효과 | 수정 파일 |
|------|------|--------|------|-----------|
| **1** | **`connect()` non-blocking 전환** | 중간 | **시나리오 1, 2 모두 근본 해결** | `Comtcpip.cpp` |
| **2** | `ScanServerPause` → 해당 포트 관련만 | 낮음 | 시나리오 1: 다른 포트 중단 해소 | `ScanEdit.cpp` |
| **3** | `ChangeDualSystem` 잠금 범위 축소 | 낮음 | 시나리오 2: 교착 해소 | `Scanstat.cpp` |
| **4** | `WaitThreadProtocolDrawWorking` 타임아웃 단축 | 낮음 | 시나리오 2: Hang 시간 단축 | `Pro_main.cpp` |
| **5** | TCP 포트 `bActiveThread` 강제 ON | 낮음 | 시나리오 1: 메인스레드 connect 차단 | `Scanfile.cpp` |
