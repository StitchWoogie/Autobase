# Excel Worker Quickstart

## 웹서버와 LocalMain PC의 의미
- `웹서버`는 IIS가 돌아가는 서버를 뜻합니다.
- `LocalMain PC`는 사용자가 LocalMain을 실행하는 현장 PC입니다.
- `Excel Worker`는 원칙적으로 `웹서버`와 분리된 별도 Windows PC 또는 VM에 두는 것을 권장합니다.

## 권장 배치
### 1. 가장 권장
- IIS 웹서버 1대
- Excel Worker 전용 PC 또는 VM 1대 이상

장점:
- IIS와 Excel 충돌을 줄일 수 있습니다.
- Excel 팝업, 좀비 프로세스, 세션 문제를 Worker 쪽에 격리할 수 있습니다.

### 2. 소규모 임시 운영
- 웹서버와 Excel Worker를 같은 PC에서 같이 운영

가능은 하지만:
- IIS와 Excel이 같은 장비 자원을 공유합니다.
- 장애 원인 분리가 어렵습니다.

### 3. LocalMain PC를 Worker로 사용
- 기술적으로는 가능합니다.
- 다만 해당 PC가 항상 켜져 있어야 하고, 로그인 세션과 Excel 상태가 안정적으로 유지되어야 합니다.

권장 상황:
- 파일럿
- 테스트
- 단일 사이트 소규모 운영

## 지금 구조에서의 의미
- 웹 사용자가 보고서를 요청하면 IIS가 직접 Excel을 돌리는 것이 아닙니다.
- IIS는 작업만 등록합니다.
- Excel Worker가 그 작업을 가져가 실제 Excel을 실행합니다.

## 기본 실행 파일
- [ExcelWorkerHost.exe](D:/Autobase48/AutobasePrime/PortalServer/ExcelWorkerHost/bin/Debug/ExcelWorkerHost.exe)

## 시작 스크립트
- [start-excel-worker.cmd](D:/Autobase48/AutobasePrime/PortalServer/ExcelWorkerHost/start-excel-worker.cmd)

## start-excel-worker.cmd 설정값
- `WORK_DIR`
  - 웹 프로젝트의 `AutoWeb\Project` 경로
- `BASE_URL`
  - 결과 파일을 여는 웹 주소
- `RUNTIME_DIR`
  - `OfficeExcelLibrary.dll` 이 있는 `AutoWeb\Runtime` 경로
- `WORKER_NAME`
  - 작업 로그에 남길 Worker 이름
- `POLL_SECONDS`
  - 큐 확인 주기

## 예시 1: 웹서버와 Worker가 같은 PC
```cmd
set "WORK_DIR=D:\EXE\AUTOBASE.Prime\WebServer\AutoWeb\Project"
set "BASE_URL=http://localhost"
set "RUNTIME_DIR=D:\EXE\AUTOBASE.Prime\WebServer\AutoWeb\Runtime"
```

## 예시 2: Worker 전용 PC가 별도로 있고, 결과는 웹서버가 제공
```cmd
set "WORK_DIR=\\WEB-SERVER\AutoWebShare\Project"
set "BASE_URL=http://webserver"
set "RUNTIME_DIR=D:\EXE\AUTOBASE.Prime\WebServer\AutoWeb\Runtime"
```

주의:
- 이 경우 `WORK_DIR` 는 Worker PC에서 접근 가능한 공유 경로여야 합니다.
- `Report`, `Result`, `ExcelWorker` 폴더에 읽기/쓰기 권한이 필요합니다.

## 실행 방법
```cmd
D:\Autobase48\AutobasePrime\PortalServer\ExcelWorkerHost\start-excel-worker.cmd
```

## 운영 권장사항
- Worker 전용 Windows 계정을 사용합니다.
- Excel이 설치되어 있어야 합니다.
- 해당 계정으로 한 번 이상 로그인해서 Excel 초기 실행을 완료해둡니다.
- 화면 잠금은 가능하지만, 세션이 완전히 종료되지 않도록 운영합니다.
- Excel 자동 업데이트나 팝업이 작업을 막지 않게 관리합니다.

## 다음 단계
- 시작 프로그램 등록 또는 서비스 래퍼 등록
- Worker 상태 모니터링
- 실패 작업 재시도 정책 추가
