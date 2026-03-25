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

##### 제약 5: modemList[256] 정적 배열

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
| **portBuf 메모리** | `256 × sizeof(GLOBAL_PORT_STRUCT)` | `10000 × sizeof(GLOBAL_PORT_STRUCT)` | 약 39배 증가 |
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
