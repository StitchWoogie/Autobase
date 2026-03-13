# PLC_SCAN 코드 리뷰 보고서

**검토일**: 2026-03-13
**검토자**: SCADA 전문 코드 리뷰
**대상**: AutobasePrime/PLC_SCAN 전체 소스코드

---

## 요약

PLC_SCAN은 31개 산업용 프로토콜을 지원하는 성숙한 SCADA 통신 프레임워크입니다.
전체 약 87개 C++ 소스 파일(~17,800 라인)을 분석한 결과, **치명적 메모리 안전성 문제**, **스레드 동기화 결함**, **버퍼 오버플로우 위험** 등을 발견했습니다.

### 심각도 분류
- CRITICAL: 즉시 수정 필요 (시스템 크래시, 메모리 손상 가능)
- HIGH: 조기 수정 권장 (잠재적 장애 원인)
- MEDIUM: 개선 권장 (성능/유지보수성)

---

## CRITICAL-1: `new[]`로 할당 후 `delete`로 해제 (정의되지 않은 동작)

**심각도**: CRITICAL
**영향**: 메모리 손상, 힙 커럽션, 프로세스 크래시

C++ 표준에 따르면 `new[]`로 할당한 배열은 반드시 `delete[]`로 해제해야 합니다.
`delete`로 해제하면 **정의되지 않은 동작(Undefined Behavior)**이 발생하며, 힙 메타데이터 손상으로 언제든지 크래시할 수 있습니다.

### 발견 위치 (총 15건 이상)

| 파일 | 라인 | 코드 |
|------|------|------|
| `DEVICE/Comtcpip.cpp` | 306 | `delete data_enc;` (288행에서 `new char[remain]`) |
| `DEVICE/Com-232.cpp` | 310 | `delete data_enc;` (293행에서 `new char[remain]`) |
| `DEVICE/ComDeviceTcpServer.cpp` | 181 | `delete data_enc;` (162행에서 `new char[remain]`) |
| `DEVICE/Comudpip.cpp` | 256 | `delete localStruct;` (248행에서 `new LOCAL_STRUCT[MAX_PORT]`) |
| `DEVICE/Comudpip.cpp` | 416 | `delete local->ring;` (272행에서 `new BYTE[MAX_UDPIP_RECV_BUF]`) |
| `ScanServerStatus.cpp` | 448 | `delete block;` (416행에서 `new ...[MAX_BLOCK_SEND_WORD]`) |
| `ScanServerStatus.cpp` | 561 | `delete block;` (529행에서 `new ...[MAX_BLOCK_SEND_FLOAT]`) |
| `ScanServerStatus.cpp` | 674 | `delete block;` (642행에서 `new ...[MAX_BLOCK_SEND_DWORD]`) |
| `ScanServerStatus.cpp` | 911 | `delete block;` (879행에서 `new ...[MAX_BLOCK_SEND_DOUBLE]`) |
| `ScanServerStatus.cpp` | 1024 | `delete block;` (992행에서 `new ...[MAX_BLOCK_SEND_INT64]`) |
| `ScanServerStatus.cpp` | 1289-1294 | `delete conn->nReadPosWORD;` 등 6건 (1233-1238행에서 `new WORD[MAX_PORT]`) |
| `ComputerDual.cpp` | 516 | `delete block;` (484행에서 `new ...[MAX_BLOCK_SEND_WORD]`) |
| `Scanfile.cpp` | 745 | `delete pt->local.scanMethod;` (568행에서 `new SCAN_METHOD_STRUCT[...]`) |
| `Scanfile.cpp` | 750 | `delete pt->blockWriteWait->item;` (618행에서 `new SCAN_WRITE_EXCHANGE_ITEM[...]`) |
| `Scanfile.cpp` | 774 | `delete portBuf;` (709행에서 `new GLOBAL_PORT_STRUCT[MAX_PORT]`) |
| `PROTOCOL/Pro_lib.cpp` | 59,82 | `delete sPlcScanErrorString;` (67행에서 `new char[...]`) |

### 수정 방법
```cpp
// 변경 전
delete data_enc;
// 변경 후
delete[] data_enc;
```

---

## CRITICAL-2: WRITE_WAIT_STRUCT 링버퍼 경쟁 조건 (Race Condition)

**심각도**: CRITICAL
**영향**: 쓰기 명령 유실, 데이터 손상, PLC 제어 오류

`WRITE_WAIT_STRUCT` (plc_scan.h:261-270)에서 링버퍼의 동기화를 `bPushing`/`bPoping` char 플래그로 처리하고 있습니다.

```cpp
typedef struct {
    char    bPushing;           // 넣는 중이다
    char    bPoping;            // 꺼내는 중이다
    short   ring_current;
    short   ring_target;
    SCAN_WRITE_EXCHANGE_ITEM *item;
} WRITE_WAIT_STRUCT;
```

**문제점**:
- `char` 플래그는 **원자적(atomic) 연산이 아닙니다**
- 멀티스레드 환경에서 읽기-수정-쓰기 사이에 다른 스레드가 개입 가능
- CPU 캐시 일관성 보장이 없음 (volatile도 아님)
- ring_current/ring_target 동시 접근 시 인덱스 꼬임 발생 가능

### 수정 방법
```cpp
// CriticalSection 또는 Mutex 사용
CRITICAL_SECTION csWriteWait;
// 또는 InterlockedExchange 사용
InterlockedExchange(&bPushing, 1);
```

---

## CRITICAL-3: 스레드 변수 동기화 부재

**심각도**: CRITICAL
**영향**: 스레드 간 데이터 비가시성, 이중화 절체 실패

`PortThread.cpp`에서 `thread->bDo`와 `thread->bEnd`가 `volatile`로 선언되지 않아, 컴파일러 최적화에 의해 스레드 간 변경이 반영되지 않을 수 있습니다.

```cpp
// PortThread.cpp:29
while(thread->bDo) {        // 컴파일러가 레지스터에 캐시할 수 있음
    WatchDogPlcScanReset(pt->local.no);
    Sleep(sleep_cycle);
    CommStatusLocalOne(pt);
}
```

`THREAD_PORT_STRUCT`의 `bEnd`, `bDo` 필드가 일반 `char`로 선언되어 있어, Release 빌드에서 최적화가 적용되면 스레드가 영원히 종료되지 않을 수 있습니다.

### 수정 방법
```cpp
typedef struct {
    HANDLE  handle;
    DWORD   id;
    volatile char bEnd;    // volatile 추가
    volatile char bDo;     // volatile 추가
} THREAD_PORT_STRUCT;
```

---

## HIGH-1: sprintf/strcpy 버퍼 오버플로우 위험

**심각도**: HIGH
**영향**: 스택 손상, 원격 코드 실행 가능성

프로젝트 전체에서 **sprintf 842건**, **strcpy 143건**이 경계 검사 없이 사용되고 있습니다.

### 위험한 사례

**Comtcpip.cpp:91** - 외부 입력(IP 주소)이 고정 크기 버퍼에 직접 입력:
```cpp
char title[80];
sprintf(title, "gethostbyname() error HostName(%s)", tcpip->ip);
// tcpip->ip가 80바이트 이상이면 스택 오버플로우
```

**ScanServerStatus.cpp:70-71** - GetPrivateProfileString 후 strcpy:
```cpp
GetPrivateProfileString("Modem", "Initial Command", "AT &C1 B0", buf, 20, filename);
strcpy(conn->sModemInitCommand, buf);  // sModemInitCommand 크기 미검증
```

**Comtcpip.cpp:256-260** - 에러 메시지 포맷팅:
```cpp
char message[160];
sprintf(message, "Tcp/ip connect() error - ErrorCode(%d) IP:%s, Port:%d",
        err, tcpip->ip, tcpip->port[tcpip->nCurrPort]);
// IP 주소가 길면 160바이트 초과 가능
```

### 수정 방법
```cpp
// sprintf -> _snprintf 또는 snprintf_s
_snprintf(title, sizeof(title)-1, "gethostbyname() error HostName(%s)", tcpip->ip);
title[sizeof(title)-1] = '\0';

// strcpy -> strncpy
strncpy(conn->sModemInitCommand, buf, sizeof(conn->sModemInitCommand)-1);
conn->sModemInitCommand[sizeof(conn->sModemInitCommand)-1] = '\0';
```

---

## HIGH-2: TCP 수신 버퍼 오버플로우

**심각도**: HIGH
**영향**: 힙 손상, 원격 공격 벡터

**ScanServerStatus.cpp:265-271** - 수신 버퍼 경계 검사 불완전:
```cpp
conn->recvBuf[conn->nRecvHap] = imsi[0];
conn->nRecvHap += count;

if(conn->nRecvHap >= MAX_RECV_BUF) {
    conn->nRecvHap = 0;      // 리셋하지만 이미 쓴 데이터는 복구 불가
}
```

**문제점**: `conn->nRecvHap`이 `MAX_RECV_BUF-1`일 때 `count`가 1 이상이면, 배열 범위를 초과한 후에야 검사가 실행됩니다. 쓰기가 먼저 발생하고 그 다음 경계 검사를 하므로, 1바이트 오버플로우가 발생할 수 있습니다.

### 수정 방법
```cpp
if(conn->nRecvHap >= MAX_RECV_BUF - 1) {
    conn->nRecvHap = 0;
    continue;  // 데이터 버림
}
conn->recvBuf[conn->nRecvHap] = imsi[0];
conn->nRecvHap += count;
```

---

## HIGH-3: TCP/IP 소켓 재연결 로직 결함

**심각도**: HIGH
**영향**: 연결 끊김 후 통신 복구 실패, 소켓 리소스 누수

**Comtcpip.cpp:217-273** `RetryConnect()` 함수:

```cpp
if(tcpip->error_count > 1) {
    closesocket(tcpip->socket);
    tcpip->bConnect = OFF;
    tcpip->socket = socket(PF_INET, SOCK_STREAM, 0);  // 새 소켓 생성

    if (!FillAddr(NULL, &dest_sin, tcpip)) {
        return 0;   // 소켓이 열린 상태로 반환 - 리소스 누수!
    }
```

**문제점**:
1. `FillAddr` 실패 시 새로 생성한 소켓이 닫히지 않음 (소켓 누수)
2. `connect()` 실패 시에도 소켓이 열린 상태로 유지됨
3. 반복 실패 시 소켓 핸들 고갈 가능

### 수정 방법
```cpp
if (!FillAddr(NULL, &dest_sin, tcpip)) {
    closesocket(tcpip->socket);
    tcpip->socket = INVALID_SOCKET;
    return 0;
}
```

---

## HIGH-4: TCP Clear 함수 성능 문제

**심각도**: HIGH
**영향**: CPU 과부하, 통신 지연

**Comtcpip.cpp:406-424** `PlcDeviceClearTCPIP()`:
```cpp
char imsi[10];
for(i = 0; i < (int)remain; i++) {
    retn = recv(tcpip->socket, imsi, 1, 0);  // 1바이트씩 수신!
    if(retn != 1) return 1;
}
```

**문제점**: 버퍼에 잔여 데이터가 많을 때 1바이트씩 읽으면 시스템 콜이 수천 번 발생합니다. remain이 10,000이면 recv()가 10,000번 호출됩니다.

### 수정 방법
```cpp
char discard_buf[4096];
while(remain > 0) {
    int to_read = min((int)remain, sizeof(discard_buf));
    retn = recv(tcpip->socket, discard_buf, to_read, 0);
    if(retn <= 0) return 1;
    remain -= retn;
}
```

---

## MEDIUM-1: PortThread.cpp 256개 하드코딩 스레드 함수

**심각도**: MEDIUM
**영향**: 유지보수성 극히 낮음, 코드 비대화

`PortThread.cpp`에서 256개의 거의 동일한 스레드 함수가 하드코딩되어 있습니다 (148-405행).
또한 Init_000_049, Init_050_099 등의 함수에서 256개의 if-else 분기가 반복됩니다 (407-500행 이후 계속).

```cpp
static DWORD WINAPI PortThread_0(LPVOID)   { PortThread_Common(0);   return 0; }
static DWORD WINAPI PortThread_1(LPVOID)   { PortThread_Common(1);   return 0; }
// ... 254개 더 ...
static DWORD WINAPI PortThread_255(LPVOID) { PortThread_Common(255); return 0; }
```

**참고**: 같은 파일 상단(11-45행)에 이미 `PortThread_Common(LPVOID lpparam)` 함수가 있고, `PortThreadInit()`(47-59행)에서 이를 직접 사용하고 있습니다. 즉 **이미 올바른 구현이 존재하는데**, 아래의 256개 하드코딩 함수는 구 버전의 레거시 코드로 보입니다.

현재 `PortThreadInit()`은 상단의 올바른 함수를 사용하므로, 하위 256개 함수(주석 처리된 코드 포함)는 삭제 가능합니다.

---

## MEDIUM-2: ScanServer 인증 없음

**심각도**: MEDIUM (네트워크 환경에 따라 HIGH)
**영향**: 무단 PLC 제어 접근

`ScanServerStatus.cpp`의 네트워크 프로토콜에 인증 메커니즘이 없습니다:
- 수신된 WRITE_BIT/WRITE_WORD 명령이 인증 없이 즉시 실행됨 (282-290행)
- Life Signal만으로 연결 상태를 관리
- 평문 TCP/IP 통신 (암호화 선택 사항)

산업 현장에서는 네트워크 세그먼트가 분리되어 있어 위험이 낮을 수 있지만,
IEC 62443 등 산업보안 표준을 고려하면 개선이 필요합니다.

---

## MEDIUM-3: ScanServerStatus 블록 전송 메모리 비효율

**심각도**: MEDIUM
**영향**: 불필요한 힙 할당/해제 반복

`ScanServerStatusSendWORD_Block()` 등의 함수에서 매 호출마다 `new`/`delete`로 블록 버퍼를 할당합니다:

```cpp
// ScanServerStatus.cpp:416
block = new NETWORK_PROTOCOL_BLOCK_WORD[MAX_BLOCK_SEND_WORD];
// ... 사용 ...
delete block;  // delete[] 여야 함 (CRITICAL-1 참조)
```

이 함수는 통신 루프에서 반복적으로 호출되므로, 정적 버퍼 또는 멤버 변수로 재사용하는 것이 효율적입니다.

---

## MEDIUM-4: 이중화 절체 시 프로토콜 DrawMethod 경쟁 조건

**심각도**: MEDIUM
**영향**: 이중화 절체 중 UI 크래시

`plc_scan.h:355`의 주석에서도 언급:
```cpp
char bThreadProtocolDrawWorking;  // 이것은 스레드 사용시 각 프로토콜의 그리는 함수에서
                                  // 점유할 때 ON을 사용한다.
                                  // 이중화 절체시 이 플래그를 사용하지 않으면 다운된다.
```

이 플래그 역시 `char` 타입으로 원자적 연산이 보장되지 않으며, UI 스레드와 통신 스레드 간의 적절한 동기화가 필요합니다.

---

## CRITICAL-4: VIP 스캔 위치 공유 변수 경쟁 조건

**심각도**: CRITICAL
**영향**: 안전 중요 데이터의 스캔 누락

**Scanstat.cpp:952** - `CommStatusLocalOne()` 함수 내부의 static 변수:
```cpp
static int nScanPosBeforeVipScan = -1;
```

이 변수가 `static`으로 선언되어 **모든 포트가 단일 값을 공유**합니다.
서로 다른 스레드에서 다른 포트를 스캔할 때 VIP 스캔 위치가 서로 덮어쓰기됩니다.
VIP 스캔은 일반적으로 안전 중요(safety-critical) 고속 폴링 항목에 사용되므로,
이 버그로 인해 **중요 데이터의 스캔이 누락될 수 있습니다**.

### 수정 방법
```cpp
// static 제거하고 포트별 변수로 이동
// GLOBAL_PORT_STRUCT에 nScanPosBeforeVipScan 필드 추가
```

---

## CRITICAL-5: 재진입 방지 플래그 비원자적 연산

**심각도**: CRITICAL
**영향**: 쓰기 처리 함수의 동시 실행으로 데이터 손상

**Scanstat.cpp:1041-1049**:
```cpp
static char flag = OFF;
if(flag) return;  // void stack overflow
flag = ON;
// ... 처리 ...
flag = OFF;
```

멀티코어 시스템에서 두 스레드가 동시에 `OFF`를 읽고 나서 둘 다 `ON`으로 설정할 수 있어,
재진입 방지가 실패합니다. 쓰기 명령 처리 함수에서 이 문제가 발생하면
PLC에 중복 쓰기 또는 잘못된 값이 전달될 수 있습니다.

### 수정 방법
```cpp
// InterlockedCompareExchange 사용
if(InterlockedCompareExchange((LONG*)&flag, ON, OFF) != OFF) return;
```

---

## HIGH-5: `strcpy`를 통한 네트워크 입력 버퍼 오버플로우 (원격 공격 벡터)

**심각도**: HIGH
**영향**: 원격 코드 실행 가능성

**Scanstat.cpp:1646,1666,1786** - 네트워크 수신 데이터를 경계 검사 없이 복사:
```cpp
strcpy(item.sExtraAddr, sExtraAddr);    // line 1646
strcpy(item.sExtraAddr, recv->sExtra1); // line 1786
```

`sExtraAddr`/`recv->sExtra1`은 네트워크 프로토콜에서 파싱된 데이터이며,
`item.sExtraAddr`의 크기 제한 없이 복사됩니다.
악의적인 패킷으로 스택 버퍼 오버플로우를 유발할 수 있습니다.

---

## HIGH-6: `static int port` 공유 변수 (멀티스레드 포트 충돌)

**심각도**: HIGH
**영향**: 멀티포트 운용 시 스캔 순서 꼬임

**Scanstat.cpp:1000**:
```cpp
static int port = 0;  // CommStatusLocal() 내부
```

이 변수가 현재 스캔 포트를 추적하는데, 여러 스레드가 동시에 접근하면
포트 번호가 꼬여 잘못된 포트를 스캔하거나 특정 포트를 건너뛸 수 있습니다.

---

## HIGH-7: ScanServer 스레드 핸들 누수

**심각도**: HIGH
**영향**: 장기 운용 시 커널 핸들 고갈

**ScanServerStatus.cpp:1154** - `CreateThread()` 호출 후 핸들 미반환:
```cpp
conn->hThread = CreateThread(NULL, 0, ServerThreadFunc, conn, 0, &conn->idThread);
```

스레드 종료 시 `CloseHandle()`이 호출되지 않고 핸들이 NULL로만 설정됩니다 (1172행).
서버 재시작 시마다 커널 핸들이 누수되어, 장기 운용 시 시스템 리소스가 고갈됩니다.

---

## HIGH-8: TCP 연결 타임아웃 미설정 (블로킹 connect)

**심각도**: HIGH
**영향**: PLC 장애 시 통신 스레드 무한 대기

**Comtcpip.cpp:130-191** - `connect()` 호출 전 SO_RCVTIMEO/SO_SNDTIMEO 미설정:
```cpp
if (connect(tcpip->socket, (PSOCKADDR)&dest_sin, sizeof(dest_sin)) == SOCKET_ERROR) {
```

원격 PLC가 응답하지 않으면 `connect()` 블로킹 호출이 OS 기본 타임아웃(수십 초~수 분)까지
대기하며, 해당 포트의 모든 통신이 중단됩니다.

---

## HIGH-9: PlcDeviceUnInit 초기화되지 않은 반환값

**심각도**: HIGH
**영향**: 정의되지 않은 동작

**Commmain.cpp:539-581** - switch 문에서 매칭되지 않는 경우:
```cpp
int PlcDeviceUnInit(DEVICE_STRUCT *device)
{
    int retn;
    switch(device->nDeviceStyle) {
        // ... 각 case에서 retn 설정 ...
    }
    return retn;  // 매칭 안 되면 초기화되지 않은 값 반환!
}
```

`DEVICE_TYPE_NONE`이나 예상치 못한 값일 때 `retn`이 초기화되지 않은 스택 값을 반환하며,
디바이스 `pData` 메모리도 해제되지 않아 메모리 누수가 발생합니다.

---

## HIGH-10: RS-232 Busy-Wait 스핀 루프 (CPU 100%)

**심각도**: HIGH (성능)
**영향**: CPU 코어 독점, 다른 포트 통신 지연

**Com-232.cpp:360-373** - RTS/DTR 토글 타이밍을 빈 루프로 처리:
```cpp
for(i = 0; i < rs232->nEndDelayReadRTS; i++) {
    for(int j = 0; j < 100; j++);   // 빈 루프!
}
```

이 방식은 해당 CPU 코어를 100% 사용하며, 다른 통신 스레드를 기아(starvation) 상태로 만듭니다.
고해상도 타이머(`QueryPerformanceCounter`) 또는 `Sleep(1)` 사용이 권장됩니다.

---

## HIGH-11: PlcDeviceGetInfoString 크기 제한 없는 버퍼 쓰기

**심각도**: HIGH
**영향**: 호출자 버퍼 오버플로우

**Commmain.cpp:271-362**:
```cpp
void PlcDeviceGetInfoString(DEVICE_STRUCT *device, char *buf)
{
    sprintf(buf, "COM%d, %lu, %d, %d, %d", ...);
    strcat(buf, info_crypto);  // 크기 제한 없음
}
```

`buf`의 크기 파라미터가 없어, `sprintf`와 `strcat`이 호출자가 제공한 버퍼를 초과할 수 있습니다.

---

## HIGH-12: StackChar(5000) 블록 전송 버퍼 오버플로우 근접

**심각도**: HIGH
**영향**: 블록 크기 증가 시 힙 손상

**ScanServerStatus.cpp:382-387 등 다수**:
```cpp
StackChar buf(5000);
for(i = 0; i < block_size; i++) {
    sprintf(imsi, "%02X", p[i]);
    strcat(buf.data, imsi);   // 각 바이트당 2문자 추가
}
```

`MAX_BLOCK_SEND_WORD=500`, `sizeof(NETWORK_PROTOCOL_BLOCK_WORD)=4`이면
`block_size=2000`, 헥스 문자열=4000바이트 + prefix ≈ 5000바이트 근접.
블록 크기가 조금만 증가하면 즉시 오버플로우됩니다.

---

## CRITICAL-6: MAX_PORT INI 파일 입력값 미검증

**심각도**: CRITICAL
**영향**: 메모리 고갈 또는 비정상 동작

**Scanmain.cpp:~352-356** - INI 파일에서 읽은 값 무검증 사용:
```cpp
MAX_PORT = _wtoi(retn);  // .inix 파일에서 읽음
```

사용자 편집 가능한 `.inix` 파일에서 `MaxPorts` 값을 읽어 검증 없이 사용합니다.
- `999999` 설정 시: `new GLOBAL_PORT_STRUCT[999999]` → 수 GB 메모리 할당 시도
- `0` 또는 음수 설정 시: 반복문 오동작
- `MAX_SCAN_WRITE_LOCAL_ITEM_COUNT`도 동일한 문제

### 수정 방법
```cpp
MAX_PORT = _wtoi(retn);
if(MAX_PORT < 1)   MAX_PORT = 1;
if(MAX_PORT > 256) MAX_PORT = 256;
```

---

## HIGH-13: CreateThread 대신 _beginthreadex 사용 필요

**심각도**: HIGH
**영향**: CRT 메모리 누수

**PortThread.cpp:58**:
```cpp
thread->handle = CreateThread(NULL, 0, PortThread_Common, pt, 0, &thread->id);
```

MFC/CRT 함수를 호출하는 스레드에서 `CreateThread`를 사용하면 CRT 내부의 per-thread 데이터가
해제되지 않아 메모리가 누수됩니다. Microsoft는 CRT를 사용하는 스레드에서
`_beginthreadex`를 사용할 것을 권장합니다.

---

## HIGH-14: WaitForSingleObject 타임아웃 후 핸들 강제 종료

**심각도**: HIGH
**영향**: 스레드 실행 중 핸들 해제 → 정의되지 않은 동작

**PortThread.cpp:80-81**:
```cpp
WaitForSingleObject(thread->handle, 30000);  // 30초 대기
CloseHandle(thread->handle);                  // 타임아웃되어도 무조건 닫음
```

프로토콜 DLL 내부에서 블로킹 호출(TCP 소켓 읽기 등) 중이면 30초 대기가 만료되고,
스레드가 아직 실행 중인 상태에서 핸들이 닫힙니다. 이후 스레드가 `thread->bEnd = ON`을
쓰면 해제된 메모리 접근이 됩니다.

---

## HIGH-15: GetWindowLong 64비트 포인터 잘림

**심각도**: HIGH (64비트 빌드 시)
**영향**: 64비트 환경에서 UI 크래시

**Scanwork.cpp:563, 789 등 다수**:
```cpp
GetWindowLong(hwnd, 0);  // 32비트 값만 반환
```

64비트 빌드에서 `HGLOBAL` 포인터가 잘려 잘못된 메모리를 참조합니다.
`GetWindowLongPtr`로 교체해야 합니다.

---

## MEDIUM-5: #pragma pack(push, 1) 포인터 포함 구조체

**심각도**: MEDIUM
**영향**: 성능 저하, 일부 아키텍처에서 비정렬 접근 폴트

**plc_scan.h:20** - 헤더 전체에 1바이트 패킹 적용:
```cpp
#pragma pack(push, 1)
```

`LOCAL_PORT_STRUCT` 내의 포인터 멤버(`scanMethod*`, `timeout*`, `bufWORD*` 등)가
비정렬 상태로 저장되어 성능이 저하됩니다. 프로토콜 통신 버퍼에만 패킹을 적용하고,
포인터 포함 구조체는 기본 정렬을 사용하는 것이 바람직합니다.

---

## MEDIUM-6: PortThreadInit/UnInit 포트 인덱스 범위 미검사

**심각도**: MEDIUM
**영향**: 잘못된 포트 번호로 호출 시 배열 범위 초과 접근

**PortThread.cpp:49, 65**:
```cpp
GLOBAL_PORT_STRUCT *pt = &portBuf[port];  // port 범위 검사 없음
```

`port >= MAX_PORT`인 경우 배열 범위를 벗어나 읽기/쓰기를 수행합니다.

---

## 개선 권장 사항 우선순위

| 순위 | 항목 | 심각도 | 예상 작업량 |
|------|------|--------|-------------|
| 1 | `delete` -> `delete[]` 일괄 수정 | CRITICAL | 1시간 |
| 2 | WRITE_WAIT_STRUCT CriticalSection 적용 | CRITICAL | 2시간 |
| 3 | THREAD_PORT_STRUCT volatile 추가 | CRITICAL | 30분 |
| 4 | VIP 스캔 static 변수 -> 포트별 변수 이동 | CRITICAL | 1시간 |
| 5 | 재진입 플래그 InterlockedCompareExchange 적용 | CRITICAL | 30분 |
| 6 | MAX_PORT INI 입력값 범위 검증 추가 | CRITICAL | 30분 |
| 7 | 네트워크 입력 strcpy 경계 검사 추가 | HIGH | 2시간 |
| 8 | sprintf -> _snprintf 일괄 교체 | HIGH | 4시간 |
| 9 | TCP 수신 버퍼 경계 검사 수정 | HIGH | 1시간 |
| 10 | RetryConnect 소켓 누수 수정 | HIGH | 1시간 |
| 11 | ScanServer 스레드 핸들 CloseHandle 추가 | HIGH | 30분 |
| 12 | TCP connect() 타임아웃 설정 | HIGH | 1시간 |
| 13 | PlcDeviceUnInit 반환값 초기화 | HIGH | 30분 |
| 14 | CreateThread -> _beginthreadex 교체 | HIGH | 1시간 |
| 15 | WaitForSingleObject 타임아웃 후 처리 개선 | HIGH | 1시간 |
| 16 | GetWindowLong -> GetWindowLongPtr 교체 | HIGH | 2시간 |
| 17 | RS-232 Busy-Wait -> 고해상도 타이머 교체 | HIGH | 2시간 |
| 18 | PlcDeviceClearTCPIP 벌크 읽기 | HIGH | 30분 |
| 19 | StackChar(5000) 버퍼 크기 검증/확대 | HIGH | 1시간 |
| 20 | PortThread.cpp 레거시 코드 정리 | MEDIUM | 1시간 |
| 21 | #pragma pack 포인터 구조체 분리 | MEDIUM | 설계 필요 |
| 22 | 포트 인덱스 범위 검사 추가 | MEDIUM | 1시간 |
| 23 | ScanServer 인증 메커니즘 추가 | MEDIUM | 설계 필요 |

---

## 결론

PLC_SCAN은 오랜 기간 산업 현장에서 검증된 안정적인 시스템이지만,
C++ 메모리 안전성 관련 이슈(`delete` vs `delete[]`)와 멀티스레드 동기화 문제가
잠재적으로 **힙 손상 및 통신 장애**를 유발할 수 있습니다.

특히 **CRITICAL-1 (`delete[]` 문제)**는 현재 운이 좋아서 문제가 발생하지 않는 것이며,
컴파일러 버전 변경이나 메모리 레이아웃 변화 시 즉시 크래시로 이어질 수 있으므로
**가장 먼저 수정해야 합니다**.

총 발견 건수: **CRITICAL 6건, HIGH 15건, MEDIUM 6건** = 27건
