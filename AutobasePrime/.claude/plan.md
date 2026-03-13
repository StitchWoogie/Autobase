# 신 리포트 편집기/실행기 설계안

## 1. 목표

기존 ReportModule + ReportBasicLib 기반의 legacy .rptx 리포터는 유지한다.
별도의 신규 리포트 시스템을 만들어, 사용자 경험은 엑셀처럼 가져가되
런타임/웹/PDF/PNG 출력은 우리 엔진으로 통제한다.

핵심 목표:
- 엑셀식 편집 경험 (행/열/셀 중심, 수식바, 시트 탭, 병합/서식/정렬/숫자형식)
- Excel COM/Office 설치 없이 실행 가능
- 웹에서 직접 .xlsx/.csv/.pdf/.png 생성 가능
- 기존 .rptx 자산을 단계적으로 전환 가능
- 기존 39개 커맨드 실행 로직을 재사용 또는 점진 대체 가능

## 2. 기본 방향

**유지할 것:**
- ExcelReportPublic 기반 기존 Excel COM 리포트
- ReportModule / ReportBasicLib 기반 legacy 리포터

**새로 만들 것:**
- 차세대 리포트 엔진 (Reporting.Engine)
- 엑셀식 편집기 (Reporting.Designer)
- 시트 기반 뷰어/실행기 (Reporting.Viewer)
- .xlsx 중심의 입출력 계층

**중요한 원칙:**
- 내부 문서 모델은 ClosedXML 객체가 아니라 독립 모델이어야 한다
- ClosedXML/OpenXML은 입출력 계층으로만 사용한다
- UI는 DataGridView가 아니라 커스텀 SpreadsheetControl로 간다
- 모델은 처음부터 멀티시트를 지원한다
- 하지만 초기 UI 구현은 활성 시트 1개 편집 중심으로 단순하게 시작한다

## 3. 아키텍처 개요

```
Reporting.Designer → Reporting.Engine, Reporting.Viewer, DialogTag, AutoLib
Reporting.Viewer   → Reporting.Engine
Reporting.Engine   → ReportBasicLib (rptx 변환 및 기존 실행엔진 브리지용)
                   → ClosedXML     (.xlsx 읽기/쓰기)
                   → PdfSharp      (.pdf 출력)
```

## 4. 프로젝트 구조

```
Dll/
├── ReportBasicLib/              [기존 유지]
├── ReportModule/                [기존 유지]
│
├── Reporting.Engine/            [신규] 핵심 엔진
│   ├── Reporting.Engine.csproj
│   ├── Model/
│   │   ├── WorkbookModel.cs
│   │   ├── SheetModel.cs
│   │   ├── CellModel.cs
│   │   ├── StyleModel.cs
│   │   ├── MergedRange.cs
│   │   └── PageSetupModel.cs
│   ├── IO/
│   │   ├── XlsxReader.cs
│   │   ├── XlsxWriter.cs
│   │   ├── CsvWriter.cs
│   │   ├── PdfWriter.cs
│   │   └── PngWriter.cs
│   ├── Convert/
│   │   ├── RptxToWorkbook.cs
│   │   └── WorkbookToReportStruct.cs
│   ├── Formula/
│   │   ├── FormulaEngine.cs
│   │   ├── LegacyCommandAdapter.cs
│   │   └── TagResolver.cs
│   └── Render/
│       ├── SheetRenderer.cs
│       ├── PrintRenderer.cs
│       └── MeasureService.cs
│
├── Reporting.Designer/          [신규] 편집기
│   ├── Reporting.Designer.csproj
│   ├── FormReportDesigner.cs
│   ├── Controls/
│   │   ├── SpreadsheetControl.cs
│   │   ├── FormulaBar.cs
│   │   ├── SheetTabControl.cs
│   │   ├── CellEditorOverlay.cs
│   │   └── ToolbarPanel.cs
│   ├── Dialogs/
│   │   ├── FormCellFormat.cs
│   │   ├── FormInsertTag.cs
│   │   ├── FormInsertFunction.cs
│   │   ├── FormPageSetup.cs
│   │   └── FormImportRptx.cs
│   └── Undo/
│       ├── UndoManager.cs
│       └── Commands/
│
├── Reporting.Viewer/            [신규] 실행 결과 뷰어
│   ├── Reporting.Viewer.csproj
│   ├── FormReportViewer.cs
│   ├── FormReportPreview.cs
│   └── Controls/
│       └── SheetViewControl.cs
```

**네임스페이스:**
- `Reporting.Engine`
- `Reporting.Designer`
- `Reporting.Viewer`

## 5. 핵심 설계 판단

### 5.1 DataGridView 대신 커스텀 SpreadsheetControl
초기에는 DataGridView가 빨라 보이지만 아래 기능 때문에 결국 한계가 온다:
- 병합 셀
- 엑셀식 보더
- 셀 위 인라인 편집 오버레이
- 수식바 연동
- 가상화 렌더링
- 시트 렌더링과 출력 렌더링 공통화
- 향후 차트/도형/이미지 오버레이

따라서 편집 핵심 컨트롤은 UserControl + custom paint로 구현한다.

### 5.2 내부 모델과 파일 포맷 분리
- 내부 편집/실행 모델: WorkbookModel
- 파일 입출력: XlsxReader / XlsxWriter
- 웹/PNG/PDF 출력: SheetRenderer

즉 .xlsx는 저장 포맷이지 내부 런타임 객체가 아니다.

### 5.3 멀티시트는 모델부터 지원
WorkbookModel은 처음부터 다중 시트를 지원한다.
구현 순서는 단계적으로:
- 1차: 활성 시트 1개 편집
- 2차: 시트 탭 전환
- 3차: 시트 추가/삭제/이름변경

### 5.4 차트는 1차 범위에서 제외
1차 목표: 셀 값, 수식, 서식, 병합, 행/열 크기, 시트, 실행 결과 출력
차트는 2차 확장 항목으로 둔다.

## 6. 데이터 모델

```csharp
public sealed class WorkbookModel {
    public IList<SheetModel> Sheets { get; }
    public int ActiveSheetIndex { get; set; }
    public IDictionary<string, StyleModel> NamedStyles { get; }
}

public sealed class SheetModel {
    public string Name { get; set; }
    public SortedDictionary<CellAddress, CellModel> Cells { get; }
    public Dictionary<int, double> ColumnWidths { get; }
    public Dictionary<int, double> RowHeights { get; }
    public List<MergedRange> MergedRanges { get; }
    public PageSetupModel PageSetup { get; set; }
}

public sealed class CellModel {
    public object Value { get; set; }
    public string Formula { get; set; }
    public StyleModel Style { get; set; }
    public CellDataType DataType { get; set; }
}

public sealed class StyleModel {
    public FontStyleModel Font { get; set; }
    public BorderStyleModel Border { get; set; }
    public FillStyleModel Fill { get; set; }
    public AlignmentStyleModel Alignment { get; set; }
    public string NumberFormat { get; set; }
}
```

메모리 효율을 위해 셀 저장은 희소 구조로 간다.

## 7. 파일 입출력

### 7.1 xlsx 읽기/쓰기
ClosedXML 사용. 지원 범위: 셀 값, 수식, 기본 서식, 병합, 행높이/열너비, 시트이름, 페이지설정 일부

### 7.2 CSV 출력
단일 시트 또는 활성 시트 기준 출력

### 7.3 PDF 출력
PdfSharp 사용. 처음에는 "보기 좋은 표 기반 PDF"를 목표

### 7.4 PNG 출력
SheetRenderer → Bitmap 경로. 웹 미리보기와 공통 렌더링 경로

## 8. legacy .rptx 변환 전략

### 8.1 1차 전략
TABLE_STRUCT 하나 = Sheet 하나 (가장 단순하고 안전)

### 8.2 매핑
- TABLE_STRUCT → SheetModel
- CELL_STRUCT.text → Value 또는 Formula
- fontName/fontSize/fontStyle → Style.Font
- tcolor/bcolor → FontColor / FillColor
- width/height → ColumnWidths / RowHeights
- cGroup → MergedRanges
- format → NumberFormat

### 8.3 제한
일부 특수 보더, 헤더/푸터 라인, legacy 전용 속성은 1차에 완전 재현하지 않고 주석/보조정보 수준으로 둔다.

## 9. 수식/실행 엔진

### 9.1 단기 (브리지)
WorkbookModel → REPORT_STRUCT → MakeRunReport → 결과 REPORT_STRUCT → WorkbookModel
기존 실행 엔진을 브리지로 재사용

### 9.2 장기 (직접 평가)
FormulaEngine에서 직접 legacy 39개 커맨드를 평가
구성: FormulaEngine + LegacyCommandAdapter + TagResolver

## 10. 편집기 UI

### 10.1 FormReportDesigner
구성: 상단 툴바 → 수식바 → SpreadsheetControl → 시트 탭 → 상태바

### 10.2 SpreadsheetControl
핵심: 보이는 셀만 그리기, 셀 선택/범위 선택, 키보드 이동, 인라인 편집,
행/열 리사이즈, 병합 셀 표시, 더블 버퍼링, 컨텍스트 메뉴

### 10.3 FormulaBar
현재 셀 주소 표시, 셀 값/수식 편집, 선택 셀과 양방향 동기화

### 10.4 SheetTabControl
1차: 시트 전환만 / 2차: 추가/삭제/이름변경

## 11. 실행 결과 뷰어

Reporting.Viewer는 읽기 전용 뷰어:
- 실행 결과 시트 보기, 시트 전환, 인쇄 미리보기, PNG/PDF 내보내기
- 편집기와 렌더러를 공유하되 입력 기능만 제거

## 12. 웹 서비스 연동

신 리포트는 Excel Worker 없이 직접 결과를 만든다.
Web/API → Reporting.Engine → WorkbookModel 실행 → 파일 생성 → URL 반환
기존 Excel Worker는 "기존 Excel 양식" 전용으로만 남는다.

## 13. 단계별 구현 계획

### Phase 1: Reporting.Engine 생성
- WorkbookModel, SheetModel, CellModel, StyleModel
- XlsxReader, XlsxWriter

### Phase 2: SpreadsheetControl 1차
- 단일 활성 시트 렌더링, 셀 선택, 인라인 편집, 수식바 연동

### Phase 3: FormReportDesigner
- 파일 열기/저장, 시트 탭, 기본 서식 편집

### Phase 4: .rptx → WorkbookModel 변환기
- 테이블별 시트 변환

### Phase 5: 실행 엔진 브리지
- WorkbookModel → REPORT_STRUCT → MakeRunReport

### Phase 6: Viewer / PNG / PDF / 웹 출력

### Phase 7: 장기 개선
- 직접 FormulaEngine, 차트, xlsx import/export 고도화

## 14. 기술 스택
- .NET Framework 4.8
- ClosedXML
- DocumentFormat.OpenXml
- PdfSharp 1.50
- WinForms + custom GDI+ rendering

## 15. 구현 우선순위

**필수:**
1. Reporting.Engine
2. SpreadsheetControl
3. FormReportDesigner
4. .rptx 변환기
5. 실행 브리지
6. Viewer

**후순위:**
- PDF/PNG 고도화
- 차트
- Workbook → legacy 역변환
- 고급 함수 확장

## 16. 구현 규칙

- Form을 만들 때 InitializeComponent()에서 컨트롤 추가/위치 변경을 적용하여 디자이너에서 보이도록 한다
- InitializeComponent()에는 외부 메서드 및 변수를 추가하지 않는다
- 모든 소스 파일은 UTF-8 BOM으로 저장한다
- 한글 주석 및 한글 문자열이 깨지지 않도록 주의한다
