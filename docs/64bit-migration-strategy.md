# Autobase 64bit 마이그레이션 전략 분석

## 1. 현재 32bit 아키텍처 분석

### 1.1 메모리 호출 구조 현황

현재 localmain(C#)과 plcscan(C++) 프로젝트는 다음과 같은 32bit 의존적 구조를 사용합니다:

#### 핵심 구조체 (plc_scan.h)
```
SCAN_METHOD_STRUCT
├── HANDLE local          ← 32bit: 4바이트, 64bit: 8바이트 (크기 변동)
├── DWORD address         ← 항상 4바이트 (변동 없음)
└── WORD struct_size      ← 항상 2바이트 (변동 없음)

LOCAL_PORT_STRUCT
├── SCAN_METHOD_STRUCT *scanMethod  ← 포인터: 32bit=4B, 64bit=8B
├── WORD_BUF *bufWORD               ← 포인터: 크기 변동
├── FLOAT_BUF *bufFLOAT             ← 포인터: 크기 변동
├── HLOCAL hLocalProtocol            ← 핸들: 크기 변동
├── TimeOutClass *timeout            ← 포인터: 크기 변동
├── char *sPlcScanErrorString        ← 포인터: 크기 변동
└── char reserved[52]                ← 고정

GLOBAL_PORT_STRUCT
├── LOCAL_PORT_STRUCT local          ← 내부 포인터들로 인해 크기 변동
├── HANDLE hmmfWORD ~ hmmfINT64     ← 핸들 7개: 각각 크기 변동
├── DLL_PROTOCOL_PROC dll           ← 함수 포인터들: 크기 변동
├── CryptoCommunication *pCC        ← 포인터: 크기 변동
└── BYTE reserved[1431]             ← 고정
```

#### IPC 공유 메모리 구조 (main_scan.h)
```
SCAN_WRITE_EXCHANGE_ITEM    ← 포인터 없음, 고정 크기 (안전)
SCAN_WRITE_EXCHANGE_INFO    ← 포인터 없음, 고정 크기 (안전)
PLC_SCAN_PORT_INFO          ← 포인터 없음, 고정 크기 (안전)
WORD_BUF, FLOAT_BUF 등     ← 포인터 없음, 고정 크기 (안전)
```

#### 주요 32bit 의존 코드

| 위치 | 문제점 | 영향도 |
|------|--------|--------|
| `Scanfile.cpp` | `CreateFileMapping((HANDLE)0xFFFFFFFF, ...)` | **높음** - `INVALID_HANDLE_VALUE` 사용 필요 |
| `plc_scan.h` | `HANDLE local` in SCAN_METHOD_STRUCT | **높음** - 구조체 크기/오프셋 변동 |
| `plc_scan.h` | 포인터 멤버들 in LOCAL_PORT_STRUCT | **높음** - 구조체 크기/오프셋 변동 |
| `Scanwork.cpp` 등 | `SetWindowLong/GetWindowLong` (68건) | **높음** - `SetWindowLongPtr` 필요 |
| `PROTOCOL/*.cpp` | `GlobalAlloc` → HGLOBAL 캐스팅 | **중간** - 포인터 크기 변동 |
| `compiler.hpp` | `SetWindowLong(hwnd, 0, (LONG)hGlobal)` | **높음** - LONG_PTR 필요 |

---

## 2. 64bit 마이그레이션 방법

### 2.1 단계별 수정 전략

#### Phase 1: 즉시 수정 가능한 항목 (하위 호환 유지)

**A. `INVALID_HANDLE_VALUE` 매크로 사용**
```cpp
// 변경 전 (32bit 전용)
CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, ...)

// 변경 후 (32/64bit 호환)
CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, ...)
```
- 영향 파일: `Scanfile.cpp`, `PlcScanSharedMemory.cpp`, `Scanstat.cpp` 등 (약 69건)
- `INVALID_HANDLE_VALUE`는 32bit에서 0xFFFFFFFF, 64bit에서 0xFFFFFFFFFFFFFFFF로 자동 확장

**B. `SetWindowLong` → `SetWindowLongPtr` 전환**
```cpp
// 변경 전
SetWindowLong(hwnd, 0, (LONG)hGlobal);
HGLOBAL hg = (HGLOBAL)GetWindowLong(hwnd, 0);

// 변경 후
SetWindowLongPtr(hwnd, 0, (LONG_PTR)hGlobal);
HGLOBAL hg = (HGLOBAL)GetWindowLongPtr(hwnd, 0);
```
- `GWL_USERDATA` → `GWLP_USERDATA` 변경 필요
- 영향: PLC_SCAN 프로젝트 전체 68건

**C. 정수-포인터 캐스팅 수정**
```cpp
// 변경 전
DWORD ptr_val = (DWORD)some_pointer;

// 변경 후
DWORD_PTR ptr_val = (DWORD_PTR)some_pointer;
// 또는
uintptr_t ptr_val = (uintptr_t)some_pointer;
```

#### Phase 2: 구조체 크기 변동 처리

**핵심 문제**: `#pragma pack(1)`로 바이트 정렬된 구조체에 포인터/핸들이 포함되어 있어, 32bit와 64bit에서 구조체 크기가 달라집니다.

**A. 프로세스 내부 전용 구조체 (수정 용이)**

`LOCAL_PORT_STRUCT`, `GLOBAL_PORT_STRUCT`는 프로세스 내부에서만 사용되므로 재컴파일만으로 해결:
```cpp
// reserved 배열 크기를 조정하여 총 구조체 크기 유지 (선택사항)
// 64bit에서 포인터가 커지므로 reserved를 줄여 총 크기를 맞출 수 있음
#ifdef _WIN64
    char reserved[XX];  // 64bit용 예약 크기
#else
    char reserved[52];  // 기존 32bit 크기
#endif
```

**B. IPC 공유 구조체 (이미 안전)**

`main_scan.h`의 공유 구조체들 (`PLC_SCAN_PORT_INFO`, `WORD_BUF`, `FLOAT_BUF`, `SCAN_WRITE_EXCHANGE_ITEM` 등)은 포인터 멤버가 없고 고정 크기 타입(`int`, `DWORD`, `WORD`, `short`)만 사용하므로 **32bit/64bit 간 공유 메모리 호환이 이미 보장**됩니다.

#### Phase 3: DLL 인터페이스 정비

**현재 상태**: LocalMain(C#)에서 이미 `PlcScan32.cs` / `PlcScan64.cs`로 분리되어 있고, `IntPtr.Size`로 런타임 선택 중.

```
LocalMain (C#, AnyCPU)
├── IntPtr.Size == 8 → PlcScan64 → Win64Common.dll
└── IntPtr.Size == 4 → PlcScan32 → Win32Common.dll
```

필요 작업:
- `Win32Common.vcxproj`에서 x64 빌드 설정 확인 (이미 존재)
- PLC_SCAN.vcxproj에 x64 플랫폼 구성 추가 (현재 Win32만 존재)
- 64bit 빌드 시 출력 DLL명 확인 (`Win64Common.dll`)

#### Phase 4: PLC_SCAN 프로젝트 x64 빌드 구성 추가

```xml
<!-- PLC_SCAN.vcxproj에 추가 -->
<PropertyGroup Condition="'$(Platform)'=='x64'">
  <TargetMachine>MachineX64</TargetMachine>
</PropertyGroup>
```
- 현재 PLC_SCAN은 Win32 구성만 존재 (Debug|Win32, Release|Win32 등)
- x64 Platform을 추가하고 `/MACHINE:X64` 링크 옵션 설정 필요

---

## 3. 32bit/64bit 동시 지원 가능 여부

### 결론: **가능합니다. 실제로 부분적으로 이미 구현되어 있습니다.**

### 3.1 현재 이미 구현된 부분

| 컴포넌트 | 32bit 지원 | 64bit 지원 | 상태 |
|----------|-----------|-----------|------|
| LocalMain (C#) | PlcScan32.cs → Win32Common.dll | PlcScan64.cs → Win64Common.dll | **완료** |
| Win32Common DLL | Win32 빌드 구성 존재 | x64 빌드 구성 존재 | **완료** |
| PLC_SCAN.exe | Win32 빌드 구성 존재 | x64 빌드 구성 **없음** | **미완료** |
| C# DDE Tag | C_DdeTag32.cs | C_DdeTag64.cs | **완료** |

### 3.2 듀얼 지원 아키텍처

```
┌─────────────────────────────────────────────┐
│         LocalMain.exe (C#, AnyCPU)          │
│                                              │
│   if(IntPtr.Size == 8)     if(IntPtr.Size==4)│
│         │                        │           │
│    PlcScan64              PlcScan32          │
│         │                        │           │
│   ┌─────┴─────┐          ┌──────┴──────┐    │
│   │Win64Common│          │Win32Common  │    │
│   │   .dll    │          │   .dll      │    │
│   └─────┬─────┘          └──────┬──────┘    │
│         │                        │           │
│   공유메모리 접근           공유메모리 접근   │
└─────────┼────────────────────────┼───────────┘
          │                        │
    ┌─────┴──────┐          ┌──────┴──────┐
    │ PLC_SCAN   │          │ PLC_SCAN    │
    │  (x64)     │          │  (x86)      │
    └────────────┘          └─────────────┘
```

### 3.3 핵심 조건

32bit/64bit 동시 지원이 가능한 이유:
1. **IPC 공유 구조체에 포인터가 없음** → 프로세스 비트 수가 달라도 공유 메모리 레이아웃 동일
2. **C# 측에서 런타임 감지 로직 완비** → `IntPtr.Size`로 자동 선택
3. **DLL 이름 분리 완료** → `Win32Common.dll` vs `Win64Common.dll`
4. **Named Shared Memory 사용** → 이름 기반 접근이므로 비트 수 무관

**주의**: 32bit PLC_SCAN과 64bit LocalMain(또는 반대) 조합도 가능합니다. 공유 메모리의 데이터 구조체(`WORD_BUF`, `FLOAT_BUF` 등)에 포인터가 없으므로 크로스 비트 IPC가 안전합니다.

---

## 4. 64bit 지원 시 장점 및 단점

### 4.1 장점

| 항목 | 설명 |
|------|------|
| **메모리 확장** | 프로세스당 4GB → 사실상 무제한 (8TB 이상). 대규모 태그/포트 운용 시 메모리 부족 해소 |
| **성능 향상** | 64bit 레지스터 활용, DOUBLE/INT64 연산이 네이티브 크기로 처리되어 부동소수점 연산 성능 향상 |
| **향후 호환성** | Windows의 64bit 전용 API 및 최신 보안 기능(ASLR 확장, DEP 강화) 활용 가능 |
| **대규모 시스템** | 포트 수(현재 MAX_PORT=256) 확장 시 메모리 여유, 더 많은 스캔 메소드/버퍼 지원 |
| **보안 강화** | 64bit 주소 공간의 ASLR이 32bit 대비 훨씬 효과적 |
| **최신 라이브러리** | 일부 서드파티 라이브러리가 64bit만 지원하는 추세 |
| **장기 유지보수** | Microsoft가 32bit Windows 지원을 점진적으로 축소 중 |

### 4.2 단점

| 항목 | 설명 |
|------|------|
| **마이그레이션 비용** | `SetWindowLong` 68건, `(HANDLE)0xFFFFFFFF` 69건 등 코드 수정 필요 |
| **메모리 사용량 증가** | 포인터/핸들이 4B→8B로 증가, `GLOBAL_PORT_STRUCT` 크기 증가 |
| **구조체 크기 변동** | `LOCAL_PORT_STRUCT`, `GLOBAL_PORT_STRUCT`의 크기가 달라짐 (reserved 재조정 필요) |
| **프로토콜 DLL 재빌드** | 모든 통신 프로토콜 DLL을 64bit로 재빌드 필요 |
| **테스트 범위** | 모든 프로토콜(Modbus, GESNP, DDE 등)을 64bit에서 재검증 필요 |
| **배포 복잡성** | 32bit/64bit 동시 지원 시 DLL 2벌 관리 필요 |

### 4.3 현재 코드베이스 기준 구체적 수정량 추정

```
수정 필요 항목                     예상 수정 건수
─────────────────────────────────────────────────
(HANDLE)0xFFFFFFFF 치환             ~69건
SetWindowLong/GetWindowLong 전환    ~68건
PLC_SCAN.vcxproj x64 구성 추가     1건
DWORD→DWORD_PTR 포인터 캐스팅      조사 필요
LONG→LONG_PTR 캐스팅               조사 필요
reserved[] 크기 재조정              2-3개 구조체
프로토콜 DLL x64 빌드              각 프로토콜별
─────────────────────────────────────────────────
```

---

## 5. 권장 마이그레이션 순서

```
1단계: (HANDLE)0xFFFFFFFF → INVALID_HANDLE_VALUE 치환 (32bit 호환 유지)
    ↓
2단계: SetWindowLong → SetWindowLongPtr 전환 (32bit 호환 유지)
    ↓
3단계: DWORD/LONG 포인터 캐스팅 → DWORD_PTR/LONG_PTR 전환
    ↓
4단계: PLC_SCAN.vcxproj에 x64 플랫폼 구성 추가
    ↓
5단계: 64bit 빌드 및 컴파일 오류 수정
    ↓
6단계: 각 프로토콜 DLL 64bit 빌드
    ↓
7단계: 통합 테스트 (32bit PLC_SCAN + 64bit LocalMain, 역방향 등)
```

**1~3단계는 32bit 빌드에 영향을 주지 않으므로 안전하게 선행 가능합니다.**
