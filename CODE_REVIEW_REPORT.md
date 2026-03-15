# Autobase SCADA 시스템 종합 코드 리뷰 보고서

**검토일**: 2026-03-15
**검토 범위**: 전체 프로젝트 (3,079개 소스 파일, 8개 서브시스템)
**목적**: 산업용 SCADA 안정화, 성능 개선, 편의성 증대, 기능 추가를 위한 심층 분석

---

## 목차

1. [종합 평가 요약](#1-종합-평가-요약)
2. [PLC_SCAN 및 통신 프로토콜](#2-plc_scan-및-통신-프로토콜)
3. [LocalMain 핵심 런타임](#3-localmain-핵심-런타임)
4. [네트워크 통신 (NetClient/NetServer)](#4-네트워크-통신)
5. [OPC UA 클라이언트](#5-opc-ua-클라이언트)
6. [ScriptLib 스크립트 엔진](#6-scriptlib-스크립트-엔진)
7. [Python AI Engine](#7-python-ai-engine)
8. [Studio/ViewMain HMI](#8-studioviewmain-hmi)
9. [유틸리티 (Launcher, WatchDog, NetTools 등)](#9-유틸리티)
10. [종합 개선 로드맵](#10-종합-개선-로드맵)

---

## 1. 종합 평가 요약

### 서브시스템별 점수표

| 서브시스템 | 안정성 | 성능 | 보안 | 기능 | 편의성 | 종합 |
|-----------|--------|------|------|------|--------|------|
| PLC_SCAN / 프로토콜 | 6/10 | 7/10 | **4/10** | 7/10 | 6/10 | 6.0 |
| LocalMain 런타임 | 6/10 | 6/10 | 5/10 | 6/10 | 5/10 | 5.6 |
| 네트워크 통신 | 5/10 | 5/10 | **3/10** | 5/10 | 4/10 | 4.4 |
| OPC UA 클라이언트 | 7/10 | 6/10 | 5/10 | 5/10 | 6/10 | 5.8 |
| ScriptLib 엔진 | 5/10 | 5/10 | **4/10** | 5/10 | 5/10 | 4.8 |
| Python AI Engine | 5.5/10 | 5/10 | **3/10** | 4.5/10 | 4.5/10 | 4.5 |
| Studio/ViewMain HMI | 6/10 | 6/10 | 6/10 | 6/10 | 6/10 | 6.0 |
| 유틸리티 | 6/10 | 6/10 | **4/10** | 6/10 | 6/10 | 5.6 |
| **전체 평균** | **5.8** | **5.8** | **4.3** | **5.6** | **5.3** | **5.3** |

### 핵심 발견사항 (Critical Findings)

총 **47건의 심각한 이슈** 발견:
- **보안 취약점**: 18건 (암호화 결함, 인증 부재, 샌드박스 탈출 등)
- **안정성 결함**: 15건 (스레드 안전성, 메모리 누수, 리소스 미해제)
- **성능 병목**: 8건 (동기 블로킹, 버퍼 미관리, 비효율 폴링)
- **기능 부재**: 6건 (HDA 미지원, 자동 복구 미흡, 모니터링 부족)

---

## 2. PLC_SCAN 및 통신 프로토콜

### 분석 범위
- `AutobasePrime/PLC_SCAN/` (C++ 네이티브 코드)
- `AutobasePrime/ExportDll/` (ModBus, Cnet, PcdSbus 등 C# 드라이버)
- `AutobasePrime/Protocol/`

### 2.1 안정성 이슈

#### [심각] 스레드 안전성 문제
- **ScanServerStatus.cpp:248-336** - `conn->recvBuf` 동시 접근 가능, `conn->nRecvHap`이 동기화 없이 수정됨
- **PortThread.cpp:58** - `CreateThread()` 후 스레드 핸들 검증 없음
- **Scanwork.cpp:564,790,832** - `GlobalLock()` 사용 후 `GlobalUnlock()` 호출이 명확하지 않음

#### [심각] 메모리 관리 결함
- **Scanfile.cpp:235-269** - `new` 할당 후 NULL 체크 부재 (NULL 역참조 위험)
- **ScanServerStatus.cpp:416,529,642,879,992** - `new NETWORK_PROTOCOL_BLOCK_WORD[]` 할당 후 반환값 미검증
- **Scanfile.cpp:1233-1238** - 배열 할당 실패 시 초기화 루프에서 크래시 가능

#### [중간] 타임아웃 처리
- **ScanServerStatus.cpp:1042-1052** - 초기화 재시도 5회 실패 후 조용히 반환, 에러 로깅/알림 없음
- **ScanServerStatus.cpp:1057-1064** - 20초 연결 타임아웃 후 자동 재초기화 중 데이터 손실 가능

### 2.2 보안 이슈

#### [심각] 버퍼 오버플로우
- **ScanServerStatus.cpp:71** - `strcpy()` 사용, 버퍼 크기 검증 없음
- **ScanServerStatus.cpp:386-387,499,612** - `strcat()` 사용, StackChar 크기 초과 가능
- **ExportModBus/ClassExportGate2.cs:515-519** - 버퍼 오버플로우 시 데이터 손실 및 동기화 오류

#### [심각] 입력 검증 부재
- **Plcwrite.cpp:42-51** - UI 입력 범위 검증 없음
- **Scanfile.cpp:395-526** - 설정 파일 읽기 시 입력 검증 약함 (음수 크기 가능)

#### [심각] 인증 메커니즘 전무
- 통신 프로토콜에 인증 메커니즘이 없음 → SCADA 시스템에 부적절

### 2.3 성능 이슈
- **ScanServerStatus.cpp:254-335** - 1바이트씩 읽는 비효율적 busy-wait 폴링, CPU 사용률 높음
- **ScanServerStatus.cpp:1083-1108** - 항상 256회 반복 (활성 포트가 적어도 불필요 반복)

### 2.4 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | `strcpy` → `strncpy`, `strcat` → `strncat` 변경 | ScanServerStatus.cpp |
| 긴급 | 모든 `new` 후 NULL 체크 추가 | Scanfile.cpp, ScanServerStatus.cpp |
| 긴급 | `conn->recvBuf` CRITICAL_SECTION 보호 | ScanServerStatus.cpp |
| 높음 | 지수 백오프 재시도 전략 | ScanServerStatus.cpp |
| 높음 | IEC 62351 기반 인증/암호화 | 전체 프로토콜 |
| 중간 | 비동기 I/O 도입, 배치 처리 확대 | ScanServerStatus.cpp |

---

## 3. LocalMain 핵심 런타임

### 분석 범위
- `AutobasePrime/LocalMain/` (Alarm, DataSave, Schedule, Script, OPCUA, Recipe, Mail, PythonAi 등)

### 3.1 안정성 이슈

#### [심각] 스레드 안전성
- **DatabaseHealthMonitor.cs:74-85** - 이중 확인 락에 `volatile` 키워드 누락 → 메모리 가시성 문제
- **RESTAPIMonitor/ClassClient.cs:21-32** - `_cancellationTokens`, `_parsingTokens` 딕셔너리 동기화 부족
- **TrendSaveManager.cs:220** - `GetResult()` 동기 호출 → 스레드 풀 데드락 위험

#### [심각] 리소스 관리
- **RESTAPIMonitor/ClassClient.cs:114** - `HttpClient`를 매번 생성 → 소켓 고갈 위험
- **PythonAiTcpConnection.cs:21** - `SemaphoreSlim` 미해제 가능성
- **Program.cs:191** - 로깅 포맷 문자열 오류 (`{0}` 1개, 인자 2개)

### 3.2 보안 이슈

#### [심각] 경로 순회 (Path Traversal)
- **Mail/C_mail.cs:28** - `username` 미검증, `..\\` 경로 이스케이프 가능
  ```csharp
  filename = String.Format("{0}\\user\\{1}\\mail*.txt", dir, username);
  ```

#### [심각] 통신 보안
- **PythonAiTcpConnection.cs:40-44** - TCP 연결에 TLS/SSL 없음 → MITM 공격 취약

#### [중간] 자격증명
- **DatabaseHealthMonitor.cs:115** - 연결 문자열 메모리 평문 저장

### 3.3 성능 이슈
- **TrendSaveManager.cs:398-416** - 매 호출마다 전체 태그 배열 순회 O(n)
- **TrendSaveManager.cs:71** - `_processedHourKeys` HashSet 무한 증가 가능 (정리 실패 시)
- **AlarmProcessor.cs:121** - 매 배치마다 리스트 복사 → GC 압력

### 3.4 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | `volatile` 추가 | DatabaseHealthMonitor.cs:78 |
| 긴급 | 딕셔너리 접근 동기화 | RESTAPIMonitor/ClassClient.cs |
| 긴급 | TLS 암호화 추가 | PythonAiTcpConnection.cs |
| 긴급 | Path.Combine() + 경로 검증 | Mail/C_mail.cs |
| 높음 | HttpClient 싱글톤화 | RESTAPIMonitor/ClassClient.cs |
| 높음 | `GetResult()` → `await` 변경 | TrendSaveManager.cs |
| 중간 | 태그 검색 딕셔너리 캐싱 | TrendSaveManager.cs |
| 중간 | 지수 백오프 DB 재시도 전략 | TrendSaveManager.cs:436-464 |

---

## 4. 네트워크 통신

### 분석 범위
- `AutobasePrime/NetClient/`, `NetServer/`, `Dll/NetCommon/`, `Dll/AutoLibLocal/`

### 4.1 안정성 이슈

#### [심각] 소켓 처리
- **NetLib.cs:54-55** - `socketNetwork1`, `socketNetwork2` static 전역 선언, 동기화 없음
- **NetLib.cs:75-79** - `SendTo()` 에 try-catch 없음, 전송 실패 시 처리 방법 없음
- **NetLib.cs:341-348** - `Bind()` 실패 시 빈 catch 블록 (silent failure)

#### [심각] 연결 복구 부재
- **NetLib.cs:207-232** - ACK 대기 5초 고정 타임아웃, 재시도 로직 없음
- `Thread.Sleep(1)` busy-wait 패턴 → CPU 낭비

#### [심각] 버퍼 오버플로우
- **NetWorkProtocolRecv.cs:196-200** - `nBlockSize` 범위 검증 없이 배열 접근

### 4.2 보안 이슈

#### [심각] 인증/암호화 전무
- IP 기반 식별만 존재, 클라이언트 인증 없음
- 모든 데이터 평문 전송, TLS/SSL 없음
- 중간자(MITM) 공격, IP 스푸핑 취약

#### [심각] DoS 취약성
- **NetLib.cs:113-131** - 모든 노드에 무조건 브로드캐스트, 속도 제한 없음

### 4.3 성능 이슈

#### [심각] 동기 ACK 대기
- **NetLib.cs:133-165** - 각 노드마다 순차적 ACK 대기 → 최악 5초 x N 노드 지연
- 비동기 처리 미지원

#### [중간] 버퍼 풀링 없음
- **NetWorkProtocolSend.cs:84-109** - 매 메시지마다 새 바이트 배열 할당, 문자열 반복 연결

### 4.4 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | static 소켓에 Lock/Monitor 추가 | NetLib.cs:54-55 |
| 긴급 | 모든 socket 작업에 try-catch | NetLib.cs:75-79 |
| 긴급 | TLS/SSL 도입 | 전체 |
| 긴급 | 토큰 기반 인증 | 전체 |
| 높음 | 비동기 ACK 대기 (Task/async-await) | NetLib.cs:133-165 |
| 높음 | ObjectPool 패턴 버퍼 풀 | NetWorkProtocolSend.cs |
| 중간 | Rate limiting 추가 | NetLib.cs:113-131 |

---

## 5. OPC UA 클라이언트

### 분석 범위
- `AutobasePrime/OPCUA.Client.Core/`, `OpcUa.Client.Host/`, `OpcUa.Client.Ipc.*`, `OPCUA.Client.UI/`

### 5.1 안정성 이슈

#### [중간] 세션 상태 불일치
- **OPCUAMember_server.cs:29,80-89** - `bServerAlive` 속성이 3곳에서 관리됨 → 경쟁 상태 위험

#### [중간] 재연결 메커니즘
- **OPCUAClientMain.cs:93-96** - WinForms Timer 사용, UI 스레드 차단 위험
- 재연결 실패 누적에 대한 백오프 전략 부재

### 5.2 보안 이슈

#### [심각] 인증서 자동 수락
- **OpcUaHost.cs:281-286** - `AutoTrustStore` 기본값 `true` → 모든 미서명 인증서 자동 수락
- **OPCUAMember_server.cs:446-461** - 모든 인증서 오류(체인 검증 실패, 만료 등) 전부 수락
- **중간자 공격(MITM) 위험**: 프로덕션 환경에서 심각

#### [심각] 호스트명 자동 변경
- **OPCUAMember_server.cs:296-311** - 서버 반환 URL을 사용자 호스트명으로 덮어씀 → DNS 스푸핑 취약

#### [중간] IPC 통신 보안
- **IpcMessage.cs** - JSON 평문 전송, 같은 머신 다른 프로세스 접근 가능

### 5.3 기능 부재
| 기능 | 상태 | 영향 | 우선순위 |
|------|------|------|---------|
| 이력 데이터 접근 (HDA) | 미구현 | 데이터 분석/감사 불가 | 높음 |
| 알람 & 이벤트 | 미구현 | 실시간 이벤트 감지 불가 | 높음 |
| 노드 브라우징 | 미구현 | AddressSpace 탐색 불가 | 중간 |
| 배치 쓰기 | 부분 구현 | 다중 쓰기 성능 저하 | 중간 |
| Write 재시도 | 미구현 | OpcUaWriteService.cs:14-75 | 중간 |

### 5.4 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | AutoTrustStore 기본값 `false` 변경 | OpcUaHost.cs:281-286 |
| 긴급 | 호스트명 자동 변경 옵션화 | OPCUAMember_server.cs:296-311 |
| 긴급 | IPC 통신 암호화 | IpcMessage.cs |
| 높음 | 인증서 체인/유효기간 검증 | OpcUaIdentityBuilder.cs |
| 높음 | Write 재시도 로직 (지수 백오프) | OpcUaWriteService.cs |
| 중간 | HDA 지원 추가 | 신규 개발 |
| 중간 | 알람 & 이벤트 구독 | 신규 개발 |

---

## 6. ScriptLib 스크립트 엔진

### 분석 범위
- `ScriptLib/ScriptLibEdit/`, `ScriptLib/ScriptLibRun/`, `AutobasePrime/LocalMain/Script/`

### 6.1 안정성 이슈

#### [심각] 무한루프 타임아웃 전무
- **CommandWhile.cs, CommandFor.cs** - 루프 시간 제한 없음 → 무한루프 시 애플리케이션 행(hang) 상태
- CancellationToken, 스택 깊이 제한 등 전혀 미구현

#### [심각] 예외 처리 부재
- **CommandBlock.cs:42-264** - `RunAsync`에 try-catch 없음
- **CommandMethod.cs:59-71** - 외부 메서드 호출 실패 시 예외 전파 무시

#### [심각] 리소스 누수
- **ScriptLibMain.cs:110-121,135-163** - StreamReader try-finally 없이 사용
- IDisposable 패턴 미구현 (ClassDataStack, CommandBlock 등)

### 6.2 보안 이슈

#### [심각] 샌드박싱 부재
- **ScriptExternalClass.cs:49-76** - 임의 C# 메서드 실행 가능, 파일 I/O/네트워크/프로세스 생성 제한 없음
- **CommandMethod.cs:75-140** - 리플렉션 기반 메서드 호출, 검증 없음

### 6.3 성능 이슈
- **CommandSubstitution.cs:23** - 단일 변수 할당도 `Task<(bool, object)>` 반환 → 불필요한 async 오버헤드
- 모든 메서드가 async only → 간단한 계산도 Task 스케줄링 비용

### 6.4 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | CancellationToken + 타임아웃 추가 | CommandWhile.cs, CommandFor.cs |
| 긴급 | StreamReader using 블록 | ScriptLibMain.cs:110,135 |
| 긴급 | RunAsync try-catch 추가 | CommandBlock.cs |
| 높음 | 스레드 안전성 (ConcurrentCollections) | ScriptLibMain.cs |
| 높음 | API 접근 제어 화이트리스트 | ScriptExternalClass.cs |
| 중간 | 동기 실행 경로 추가 (성능) | CommandSubstitution.cs |

---

## 7. Python AI Engine

### 분석 범위
- `python_ai_engine/` (gateway, models, monitoring, plugins, router, sandbox, scheduler, services, workers)

### 7.1 안정성 이슈
- **gateway/server.py:39-46** - 일반 `Exception` catch로 모든 에러 무시
- **models/cache.py:131-136** - 모델 언로드 실패 시 메모리 누수
- **main.py:243-246** - 리소스 정리 순서 위험 (플러그인 언로드 중 모델 사용 가능)
- **로깅 모듈 미구현** - `logs/audit_logger.py`, `logs/structured_logger.py` 불존재 → import 실패

### 7.2 보안 이슈

#### [심각] 샌드박스 탈출 위험
- **sandbox/policy.py:24** - numpy/pandas 허용 → ctypes 접근으로 시스템 명령 실행 가능
  ```python
  import numpy as np
  libc = np.ctypes.CDLL('libc.so.6')
  libc.system("echo pwned")
  ```
- **sandbox/runner.py:221-222** - AST 검사 없이 `exec(compiled, script_globals)` 실행
- **sandbox/runner.py:49-62** - 패턴 기반 차단 → 인코딩 회피 가능

#### [심각] 모델 무결성 미검증
- **models/registry.py:102-117** - metadata.json 서명 검증 없음
- **models/loaders/onnx_loader.py:29** - ONNX 모델 체크섬 미검증

#### [중간] 입력 검증
- **services/predict_service.py:211-214** - `current_kw` 음수 허용, `history` 크기 제한 없음
- **router/validator.py:41-77** - 리스트 내부 원소 타입 검증 없음

### 7.3 성능 이슈
- **workers/worker_pool.py:78-88** - 실제 워커 풀 미구현, ProcessPoolExecutor 주석처리
- GPU 활용 코드 전무 (ONNX CPU 모드만)
- 모델 프리로딩/워밍업 미지원

### 7.4 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | numpy/pandas ctypes 접근 차단 | sandbox/policy.py |
| 긴급 | AST 기반 코드 검증 추가 | sandbox/runner.py |
| 긴급 | 로깅 모듈 구현 | logs/ 디렉토리 |
| 긴급 | 모델 서명 검증 (HMAC-SHA256) | models/registry.py |
| 높음 | 입력 범위 검증 강화 | router/validator.py |
| 높음 | ProcessPoolExecutor 활성화 | workers/worker_pool.py |
| 중간 | 모델 프리로딩 + 워밍업 | services/predict_service.py |
| 중간 | GPU 지원 (ONNX CUDA) | models/loaders/ |

---

## 8. Studio/ViewMain HMI

### 분석 범위
- `AutobasePrime/Studio/`, `ViewMain/`, `AniEdit/`, `Dll/ViewMainPublic/`, `Dll/DialogCommon/`, `Dll/AutoLib/`

### 8.1 안정성 이슈

#### [심각] GDI 리소스 누수
- **ClassEditObjectText.cs:86** - `CreateGraphics()` 후 Dispose 없음
- **ClassStudioEdit.cs:2790** - `form.CreateGraphics()` Dispose 미호출
- **ClassEditObjectPoly.cs:120** - `new Pen(Color.Black)` 해제 없음
- **ClassEditObjectText.cs:59** - `textBox.Dispose()` 주석 처리됨

#### [중간] async void 사용
- **OpcUaHost.cs:613** - `OnReconnectTick()` async void → 예외 발생 시 크래시

### 8.2 성능 이슈
- **ClassEditObjectPoly.cs:103-104** - MouseMove마다 `form.Invalidate()` 전체 화면 재구성
- **AnimationPlay.cs:147-170** - `DateTime.Now` 사용 (±15ms 해상도) → `Stopwatch` 권장
- **ClassStudioEdit.cs:55-85** - 메모리 1GB 임계값 고정, 사용자 설정 불가

### 8.3 보안 이슈
- **DataGate.cs:181-200** - 암호화 메커니즘 존재 (양호), 해시 기반 무결성 검증

### 8.4 기능 개선
- DPI 인식(DPI-Aware) 고해상도 모니터 지원 확인 필요
- 다크 모드 지원 (ColorTotalClass 기반 테마 시스템은 존재)
- 다국어 지원 양호 (한국어, 중국어, 영어, 일본어)

### 8.5 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | CreateGraphics() using 블록 | ClassEditObjectText.cs:86, ClassStudioEdit.cs:2790 |
| 긴급 | Pen using 블록 | ClassEditObjectPoly.cs:120 |
| 긴급 | textBox.Dispose() 주석 해제 | ClassEditObjectText.cs:59 |
| 높음 | async void → async Task | OpcUaHost.cs:613 |
| 중간 | Dirty rectangle Invalidate | ClassEditObjectPoly.cs |
| 중간 | Stopwatch 기반 애니메이션 | AnimationPlay.cs |

---

## 9. 유틸리티

### 분석 범위
- `Launcher/`, `WebTools/`, `NetTools/`, `ATP_UTIL/`, `WatchDog/`, `SMS/`, `ExceptionReport/`

### 9.1 보안 이슈

#### [심각] 암호화 알고리즘 결함
- **NetTools/Crypto.cs:26** - **ECB 모드** 사용 (NIST 권장 안함, 동일 평문 → 동일 암호문)
- **NetTools/Crypto.cs:23** - **MD5** 해시 사용 (충돌 가능, RFC 6151)
- **NetTools/Crypto.cs** - **TripleDES** 사용 (NIST 폐지 예정)
- **CryptoTextReader.cs:59-60** - **하드코딩된 암호화 키/IV** → 소스 공개 시 전체 해독 가능
  ```csharp
  byte[] key = { 0x11, 0x63, 0x43, 0x54, 0x64, 0x70, 0x78, 0x89, ... };
  byte[] IV = { 0x10, 0x62, 0x03, 0x04, 0x54, 0x60, 0x75, 0x83, ... };
  ```
- **CryptoAES.cs:49** - `RijndaelManaged` (obsolete) 사용

#### [중간] SMS 보안
- **SmsServerClientProc.cs:71** - UDP 평문 통신, 인증 없음

### 9.2 안정성 이슈
- **Launcher/MainWindow.axaml.cs:196-213** - StreamReader using 블록 없음
- **WatchDog/FormWatchDogMain.cs:846** - `p.Kill()` 무조건 강제 종료 → 데이터 손실
- **WatchDog/FormWatchDogMain.cs:707-714** - 빈 catch 블록 다수

### 9.3 개선 권장사항
| 우선순위 | 항목 | 파일 |
|---------|------|------|
| 긴급 | TripleDES → AES-256, ECB → CBC/GCM | NetTools/Crypto.cs |
| 긴급 | MD5 → SHA-256 | NetTools/Crypto.cs |
| 긴급 | 하드코딩 키 제거 → DPAPI/HSM | CryptoTextReader.cs |
| 긴급 | RijndaelManaged → AesCryptoServiceProvider | CryptoAES.cs |
| 높음 | SMS UDP → TLS TCP 마이그레이션 | SmsServerClientProc.cs |
| 높음 | WatchDog graceful shutdown | FormWatchDogMain.cs |
| 중간 | 빈 catch 블록 로깅 추가 | 전체 |

---

## 10. 종합 개선 로드맵

### Phase 1: 긴급 보안 패치 (1-2주)

| # | 항목 | 서브시스템 | 위험도 |
|---|------|-----------|--------|
| 1 | 암호화 알고리즘 업그레이드 (AES-256-CBC/GCM) | NetTools | 심각 |
| 2 | 하드코딩된 키/IV 제거 → DPAPI | NetTools | 심각 |
| 3 | OPC UA AutoTrustStore 기본값 false | OPC UA | 심각 |
| 4 | Python AI 샌드박스 numpy/pandas ctypes 차단 | AI Engine | 심각 |
| 5 | strcpy/strcat → strncpy/strncat | PLC_SCAN | 심각 |
| 6 | 네트워크 소켓 동기화 추가 | NetCommon | 심각 |
| 7 | 경로 순회 방지 (Path.Combine + 검증) | LocalMain | 심각 |
| 8 | GDI 리소스 누수 수정 (using 블록) | Studio/ViewMain | 심각 |

### Phase 2: 안정성 강화 (2-4주)

| # | 항목 | 서브시스템 |
|---|------|-----------|
| 9 | DatabaseHealthMonitor volatile 추가 | LocalMain |
| 10 | HttpClient 싱글톤화 | LocalMain |
| 11 | 스크립트 엔진 타임아웃/CancellationToken | ScriptLib |
| 12 | StreamReader using 블록 (전체) | 다수 |
| 13 | PLC_SCAN 메모리 할당 NULL 체크 | PLC_SCAN |
| 14 | async void → async Task 변경 | OPC UA, HMI |
| 15 | 네트워크 ACK 비동기 처리 | NetCommon |
| 16 | AI Engine 로깅 모듈 구현 | AI Engine |

### Phase 3: 성능 최적화 (4-8주)

| # | 항목 | 서브시스템 |
|---|------|-----------|
| 17 | TLS/SSL 전체 통신 암호화 | 네트워크, PythonAi, SMS |
| 18 | 토큰 기반 인증 메커니즘 | 네트워크 |
| 19 | 버퍼 풀링 (ObjectPool 패턴) | NetCommon |
| 20 | 태그 검색 딕셔너리 캐싱 | LocalMain |
| 21 | PLC_SCAN 비동기 I/O | PLC_SCAN |
| 22 | AI Engine ProcessPoolExecutor 활성화 | AI Engine |
| 23 | Dirty rectangle 렌더링 | Studio |

### Phase 4: 기능 확장 (2-3개월)

| # | 항목 | 서브시스템 |
|---|------|-----------|
| 24 | OPC UA HDA (이력 데이터) 지원 | OPC UA |
| 25 | OPC UA 알람 & 이벤트 구독 | OPC UA |
| 26 | WatchDog 메모리/CPU 감시 | 유틸리티 |
| 27 | AI Engine 모델 무결성 검증 | AI Engine |
| 28 | AI Engine GPU 지원 | AI Engine |
| 29 | 구조화된 로깅 시스템 (전체) | 전체 |
| 30 | 자동 스케일링 (AI Engine) | AI Engine |

### Phase 5: 코드 현대화 (3-6개월)

| # | 항목 | 서브시스템 |
|---|------|-----------|
| 31 | .NET Framework → .NET Core/.NET 8 마이그레이션 검토 | 전체 |
| 32 | GlobalLock/Unlock → 현대 동기화 API | PLC_SCAN |
| 33 | GetPrivateProfileString → JSON/XML 설정 | PLC_SCAN |
| 34 | 단위 테스트 프레임워크 구축 | 전체 |
| 35 | CI/CD 파이프라인 구축 | 전체 |

---

## 부록: 심각도별 전체 이슈 목록

### 심각 (Critical) - 18건

| # | 서브시스템 | 파일 | 라인 | 이슈 |
|---|-----------|------|------|------|
| 1 | NetTools | Crypto.cs | 26 | ECB 모드 암호화 |
| 2 | NetTools | CryptoTextReader.cs | 59-60 | 하드코딩 키/IV |
| 3 | NetTools | Crypto.cs | 23 | MD5 해시 사용 |
| 4 | OPC UA | OpcUaHost.cs | 281-286 | AutoTrustStore 기본 true |
| 5 | OPC UA | OPCUAMember_server.cs | 296-311 | 호스트명 자동 변경 |
| 6 | AI Engine | sandbox/policy.py | 24 | numpy ctypes 샌드박스 탈출 |
| 7 | AI Engine | sandbox/runner.py | 221-222 | AST 검증 없이 exec |
| 8 | AI Engine | models/registry.py | 102-117 | 모델 무결성 미검증 |
| 9 | PLC_SCAN | ScanServerStatus.cpp | 71 | strcpy 버퍼 오버플로우 |
| 10 | PLC_SCAN | Scanfile.cpp | 235-269 | new 후 NULL 미체크 |
| 11 | 네트워크 | NetLib.cs | 54-55 | static 소켓 미동기화 |
| 12 | 네트워크 | NetLib.cs | 75-79 | SendTo 예외 처리 없음 |
| 13 | 네트워크 | 전체 | - | 인증/암호화 전무 |
| 14 | LocalMain | C_mail.cs | 28 | Path Traversal 취약점 |
| 15 | LocalMain | PythonAiTcpConnection.cs | 40 | TLS 미지원 |
| 16 | LocalMain | DatabaseHealthMonitor.cs | 78-80 | volatile 누락 |
| 17 | ScriptLib | CommandWhile/For.cs | - | 무한루프 타임아웃 전무 |
| 18 | HMI | ClassEditObjectText.cs | 86 | GDI 리소스 누수 |

### 높음 (High) - 15건

| # | 서브시스템 | 파일 | 이슈 |
|---|-----------|------|------|
| 1 | PLC_SCAN | ScanServerStatus.cpp | 스레드 동기화 부재 |
| 2 | PLC_SCAN | 전체 | 통신 인증 메커니즘 없음 |
| 3 | LocalMain | RESTAPIMonitor/ClassClient.cs | 딕셔너리 동기화 부족 |
| 4 | LocalMain | RESTAPIMonitor/ClassClient.cs | HttpClient 매번 생성 |
| 5 | LocalMain | TrendSaveManager.cs | GetResult() 동기 호출 |
| 6 | 네트워크 | NetLib.cs | 동기 ACK 대기 |
| 7 | 네트워크 | NetLib.cs | DoS 취약 (Rate limiting 없음) |
| 8 | OPC UA | OpcUaWriteService.cs | Write 재시도 미구현 |
| 9 | OPC UA | OpcUaIdentityBuilder.cs | 인증서 체인 검증 미흡 |
| 10 | ScriptLib | ScriptLibMain.cs | StreamReader 리소스 누수 |
| 11 | ScriptLib | ScriptExternalClass.cs | API 접근 제어 없음 |
| 12 | AI Engine | router/validator.py | 입력 검증 불완전 |
| 13 | AI Engine | workers/worker_pool.py | 워커 풀 미구현 |
| 14 | HMI | ClassEditObjectPoly.cs | Pen 리소스 미해제 |
| 15 | 유틸리티 | SmsServerClientProc.cs | UDP 평문 통신 |

---

*본 보고서는 Autobase SCADA 시스템의 산업용 안정화 및 보안 강화를 위한 코드 리뷰 결과입니다.*
*IEC 62443 (산업 사이버 보안) 및 IEC 62351 (전력 시스템 보안) 표준 준수를 권장합니다.*
