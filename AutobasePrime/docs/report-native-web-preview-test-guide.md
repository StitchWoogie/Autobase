# 전용 보고서 웹 미리보기 테스트 가이드

## 목적
- 나중에 신규 전용 보고서의 웹 미리보기 경로를 확인할 때 참고하는 문서다.
- 현재 기준으로는 `ServiceReport -> PNG 페이지 배열 -> 파일 저장/표시` 흐름을 검증한다.

## 준비 상태
- `ReportRuntimeFacade`를 통해 `.rptx` 실행 가능
- `ReportBitmapRenderFacade`를 통해 `REPORT_STRUCT`를 PNG 페이지들로 변환 가능
- `ServiceReport`에 PNG 반환 메서드 추가됨
- `AutoLib.ServiceLib`에 PNG 페이지 수신/저장 헬퍼 추가됨

## 테스트 대상 메서드

### 서버
- `GetReportBitmapPngs(...)`
- `GetReportBitmapPngsWithDic(...)`

### 클라이언트 공용 헬퍼
- `ServiceLib.GetReportBitmapPngsAsync(...)`
- `ServiceLib.GetReportBitmapPngsWithDicAsync(...)`
- `ServiceLib.SaveReportBitmapPngsAsync(...)`
- `ServiceLib.SaveReportBitmapPngsWithDicAsync(...)`

## 기본 확인 절차
1. 테스트용 `.rptx` 파일 1개 준비
2. 로컬 실행으로 결과 확인
3. 같은 조건으로 웹 서비스 호출
4. PNG 파일이 페이지별로 생성되는지 확인
5. 첫 페이지, 중간 페이지, 마지막 페이지 내용 확인

## 확인 포인트
- 긴 보고서가 한 장으로 뭉개지지 않고 여러 페이지로 분리되는지
- 파일명이 `_001`, `_002`, `_003` 형식으로 순서대로 생성되는지
- 문자열 변수 적용 결과가 로컬과 같은지
- 시간 범위가 올바르게 반영되는지
- 표 잘림이나 페이지 누락이 없는지

## 알려진 제약
- 현재 PNG는 미리보기용이다.
- 텍스트 선택, 검색, 확대 시 벡터 품질은 없다.
- 정식 배포/보관 포맷은 향후 PDF 추가가 더 적합하다.

## 다음 테스트 이후 결정할 것
- 웹에서 PNG를 세로 스크롤로 보여줄지
- 페이지네이션 UI를 둘지
- PDF를 기본 다운로드 포맷으로 추가할지
