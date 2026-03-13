# Excel Report OpenXML + ClosedXML Architecture

## 목적

기존 Excel DCOM/COM Add-in 기반 보고서 생성 구조를 Excel 비의존(in-process) 방식으로 전환한다.

- 로컬 모드의 `temp txt -> Excel.exe 실행` 흐름 제거
- IIS/WCF 환경의 Excel DCOM 자동화 제거
- `ExcelReportPublic`의 데이터 조회 로직은 최대한 재사용
- Excel Add-in은 보고서 생성 엔진이 아니라 클라이언트/뷰어 역할로 축소

## 현재 구조

### 로컬

`ScriptFunctionExcel`  
-> `ExcelReportPrepare()`  
-> `C:\ExcelReportTemp\AutoPrnXXXX.TXT` 생성  
-> `ExcelReportRun()`  
-> `Excel.exe` 실행  
-> `ExcelReportData` COM Add-in  
-> `ExcelReportPublic.ConnectRoot`

### 웹

`ScriptFunctionExcel`  
-> `ServiceExcelReportClient`  
-> 서버 측 Excel/DCOM  
-> `.xlsx` 생성  
-> URL 반환

## 목표 구조

### 공통 구조

`UI(Client)`  
-> `ReportExportService`  
-> `ReportDataProvider`  
-> `ClosedXmlTemplateRenderer`  
-> `OpenXmlPostProcessor`  
-> `.xlsx` 파일 저장/스트림 반환

### 역할 분리

- `ExcelReportData`
  - Excel Add-in 유지
  - 셀 조회/편집 UX 담당
  - 보고서 생성 엔진 책임 제거
- `ExcelReportPublic`
  - 데이터 조회 규칙, 보고서 파라미터 해석, 공통 도메인 유지
  - OpenXML/ClosedXML 기반 새 엔진 수용
- `GraphicModule`
  - 로컬/웹 실행 진입점만 담당
  - Excel 실행 대신 보고서 생성 서비스 호출

## 권장 레이어

### 1. Contracts

요청/응답/옵션/템플릿 메타데이터를 정의한다.

- `ReportRenderRequest`
- `ReportRenderResult`
- `ReportTemplateDefinition`
- `ReportDataSet`

### 2. Data Provider

기존 `ConnectRoot`, `BasicRptTool`, `run_*.cs`, `DB_Data_Read_Write.cs`를 감싸서
보고서 생성에 필요한 정규화된 데이터를 만든다.

핵심 원칙:

- Excel COM 타입을 절대 반환하지 않는다
- `DataTable`, `DataSet`, `IReadOnlyList<T>` 같은 순수 데이터만 사용한다
- 취소 토큰을 받는다

### 3. Template Renderer

ClosedXML로 템플릿 기반 렌더링을 담당한다.

적합한 책임:

- 템플릿 `.xlsx` 열기
- named range / placeholder 치환
- 표 데이터 바인딩
- 기본 스타일, 병합, 수식 보존
- 페이지 설정/인쇄 옵션 반영

### 4. OpenXML Post Processor

ClosedXML가 약하거나 직접 제어가 필요한 작업만 OpenXML SDK로 후처리한다.

적합한 책임:

- 문서 속성/커스텀 속성
- 정의된 이름, 피벗/캐시 조정
- 대용량 시트 스트리밍
- 시트 구조 직접 수정
- 향후 차트/매크로 관련 세밀 제어

### 5. Delivery Layer

- 로컬: 파일 생성 후 기본 연결 프로그램으로 연다
- 웹: 파일 경로나 URL 대신 파일 스트림/다운로드 응답 반환

## ClosedXML + OpenXML 분담 기준

### ClosedXML 우선

- 10만 행 이하 일반 보고서
- 기존 서식 템플릿 재사용
- 셀/테이블 채우기
- 유지보수성 우선

### OpenXML 직접 처리

- 50만 행 이상 대용량
- 메모리 사용량 최소화 필요
- 문서 구조를 직접 제어해야 함
- ClosedXML가 처리하지 못하는 세부 편집

## 현재 코드 기준 매핑

### 유지/재사용 대상

- `Dll/ExcelReportPublic/Connect.cs`
  - 도메인 진입점 참고용
- `Dll/ExcelReportPublic/BasicExcelReportTools.cs`
  - 보고서 설정/명령 파싱 일부 재사용
- `Dll/ExcelReportPublic/run_*.cs`
  - 데이터 조회 규칙 재사용 후보
- `Dll/ExcelReportPublic/DB_Data_Read_Write.cs`
  - 데이터 액세스 재사용 후보

### 축소/제거 대상

- `ExcelReportData/Connect.cs`
  - 보고서 생성 진입점 역할 제거
- `ViewMain/GraphicModule/Script/ScriptFunctionExcel.cs`
  - `ExcelReportPrepare`, `ExcelReportRun` 제거 대상
  - `ExcelReportRunDirect`는 새 서비스 호출로 대체
- `Dll/OfficeExcelLibrary`
  - Interop 의존 제거 대상

## `ScriptFunctionExcel` 목표 변경

### 기존

- 로컬: 임시파일 생성 후 Excel 실행
- 웹: WCF에서 서버 측 Excel 자동화

### 변경

- 로컬: `ReportExportService.CreateReportFile(...)` 호출 후 결과 파일 열기
- 웹: `ServiceExcelReport`가 `ReportExportService.CreateReportStream(...)` 호출

즉, `ScriptFunctionExcel`은 더 이상 Excel 프로세스나 temp txt 파일을 다루지 않는다.

## 단계별 마이그레이션

### 1단계. 엔진 스캐폴딩

- 요청/응답 모델 추가
- 데이터 제공 인터페이스 추가
- 템플릿 렌더러 인터페이스 추가
- 오케스트레이션 서비스 추가

### 2단계. 로컬 실행 교체

- `ExcelReportRunDirect`의 로컬 분기에서 temp txt/Excel 실행 제거
- 새 `ReportExportService` 호출
- 생성된 파일만 오픈

### 3단계. 웹 서비스 교체

- WCF 서비스 내부 Excel DCOM 제거
- 같은 `ReportExportService`를 서버에서 직접 호출
- URL 반환보다 파일 다운로드/저장 경로 반환을 우선

### 4단계. Add-in 역할 축소

- `ExcelReportData`는 셀 조회/입력 보조 기능에 집중
- 보고서 산출은 Add-in 외부 엔진으로 일원화

## 설계 원칙

- 생성 엔진은 Excel 설치 여부와 무관해야 한다
- 생성 엔진은 STA/COM 컨텍스트에 의존하면 안 된다
- 데이터 조회와 문서 렌더링을 분리한다
- 웹/로컬이 같은 엔진을 사용한다
- 장기적으로 `ExcelReportPublic` 내부의 UI Form 의존을 분리한다

## 1차 구현 권장안

- 패키지
  - `ClosedXML`
  - `DocumentFormat.OpenXml`
- 첫 대상
  - 수식/차트 의존이 낮은 템플릿형 보고서 2~3개
- 성공 기준
  - Excel 미설치 서버에서 보고서 생성 성공
  - 기존 대비 생성 시간 단축
  - temp txt 및 Excel.exe 실행 제거
  - 웹/로컬 동일 출력 확인
