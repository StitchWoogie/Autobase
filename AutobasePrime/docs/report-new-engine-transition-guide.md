# Autobase 전용 보고서 신규 전환 가이드

## 방향
- 기존 Excel 리포트는 `Excel Worker`로 유지합니다.
- 신규 보고서는 `ReportModule + ReportBasicLib` 기반으로 전환합니다.
- 즉, 앞으로는 Excel 양식이 아니라 `.rptx` 양식을 표준 보고서 자산으로 키웁니다.

## 왜 이 방향이 맞는가
- `ReportModule`에는 이미 전용 에디터와 뷰어가 있습니다.
- `ReportBasicLib`에는 이미 양식 로드, 실행 엔진, 저장 기능이 있습니다.
- 신규 보고서는 Excel 수식/VBA 호환보다 제품 제어성과 성능이 더 중요합니다.
- 웹, 스케줄러, 로컬 실행을 같은 엔진으로 묶기 쉽습니다.

## 이번에 추가한 시작점
- [ReportRuntimeFacade.cs](D:/Autobase48/AutobasePrime/Dll/ReportBasicLib/ReportRuntimeFacade.cs)
- [ReportExecutionRequest.cs](D:/Autobase48/AutobasePrime/Dll/ReportBasicLib/ReportExecutionRequest.cs)
- [ReportExecutionResult.cs](D:/Autobase48/AutobasePrime/Dll/ReportBasicLib/ReportExecutionResult.cs)

이 facade는 다음 흐름을 하나로 묶습니다.
- `.rptx` 또는 보고서 파일 경로 결정
- 실행 시간/기간 설정
- 문자열 변수 적용
- `MakeRunReport` 호출
- 실행 결과 `REPORT_STRUCT` 반환
- 필요 시 `.rptx` / `.csv` 저장

## 신규 보고서 전환 절차
### 1. 양식 제작
- `ReportModule` 에디터로 `.rptx` 양식을 만듭니다.
- 표, 셀 서식, 명령, 태그, 시간 범위를 전용 방식으로 구성합니다.

### 2. 엔진 실행
- `ReportRuntimeFacade.ExecuteFileAsync(...)` 호출
- 결과는 `REPORT_STRUCT`

### 3. 출력 선택
- 화면 뷰어: 기존 `ReportModule`
- 파일 저장: `ReportFile.Save`, `ReportFile.SaveToCSV`
- 향후 확장: PDF, XLSX export

## 추천 적용 대상
- 신규 일지/일보/월보
- 표 중심 보고서
- 설비 현황 보고서
- 알람 요약 보고서
- 추세/집계 중심의 반복 양식

## 당장 피하는 것이 좋은 대상
- 기존 Excel 수식/VBA 결과와 100% 동일성이 필수인 보고서
- 외부 Excel 연결/피벗/매크로에 의존한 양식

## 다음 구현 우선순위
1. `ReportRuntimeFacade`를 호출하는 샘플 실행 코드 추가
2. 웹에서 `.rptx` 실행 후 결과를 PNG/PDF로 내려주는 서비스 추가
3. `ReportPrint`의 bitmap 출력을 엔진 facade와 연결
4. 신규 보고서용 `Xlsx export` 추가
5. `ArrayList` -> 제네릭 컬렉션 전환

## 실무 적용 원칙
- Excel 리포트 신규 개발 금지
- 신규 보고서는 우선 `ReportModule`로 검토
- Excel 전환이 꼭 필요한 경우만 예외로 승인
