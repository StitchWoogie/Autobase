using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Reporting.Engine.Model;
using ReportBasicLib;

namespace Reporting.Engine.Convert
{
    /// <summary>
    /// REPORT_STRUCT (.rptx) → WorkbookModel 변환기
    /// 레거시 자체리포트 형식을 새 엔진 모델로 변환한다.
    /// </summary>
    public static class RptxToWorkbook
    {
        // 레거시 기본 셀 크기 (픽셀 단위)
        private const int DefaultCellWidthPx = 80;
        private const int DefaultCellHeightPx = 25;

        // 픽셀→문자폭 변환 (Excel 기본 폰트 기준 약 7.5px/char)
        private const double PixelsPerCharWidth = 7.5;

        // 픽셀→포인트 변환 (1pt ≈ 1.333px at 96dpi)
        private const double PixelsPerPoint = 1.333;

        /// <summary>
        /// REPORT_STRUCT를 WorkbookModel로 변환
        /// </summary>
        /// <param name="report">레거시 리포트 구조체</param>
        /// <returns>변환된 WorkbookModel</returns>
        public static WorkbookModel Convert(REPORT_STRUCT report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            var workbook = new WorkbookModel();

            // 테이블이 없으면 빈 시트 1개 생성
            if (report.tableBuf == null || report.tableBuf.Count == 0)
            {
                workbook.Sheets.Add(new SheetModel("Sheet1"));
                return workbook;
            }

            // 각 TABLE_STRUCT → SheetModel
            for (int t = 0; t < report.tableBuf.Count; t++)
            {
                var table = report.tableBuf[t] as TABLE_STRUCT;
                if (table == null) continue;

                string sheetName = report.tableBuf.Count == 1
                    ? "Sheet1"
                    : "Table" + (t + 1);
                var sheet = new SheetModel(sheetName);

                // 페이지 설정 (첫 번째 시트에만 적용, 나머지는 복사)
                sheet.PageSetup = ConvertPageSetup(report);

                // 셀 변환
                ConvertCells(table, sheet);

                workbook.Sheets.Add(sheet);
            }

            workbook.ActiveSheetIndex = 0;
            return workbook;
        }

        /// <summary>
        /// REPORT_STRUCT의 마진/방향 → PageSetupModel
        /// </summary>
        private static PageSetupModel ConvertPageSetup(REPORT_STRUCT report)
        {
            var setup = new PageSetupModel();

            // 레거시 마진은 mm 단위 (동일)
            setup.MarginLeft = report.margin_left;
            setup.MarginRight = report.margin_right;
            setup.MarginTop = report.margin_top;
            setup.MarginBottom = report.margin_bottom;

            // 방향: 0=세로, 1=가로
            setup.Orientation = report.bOrientation == 1
                ? PageOrientation.Landscape
                : PageOrientation.Portrait;

            // 배율
            if (report.wOpticRate > 0 && report.wOpticRate != 100)
                setup.ScalePercent = report.wOpticRate;

            return setup;
        }

        /// <summary>
        /// TABLE_STRUCT의 셀 목록 → SheetModel
        /// </summary>
        private static void ConvertCells(TABLE_STRUCT table, SheetModel sheet)
        {
            if (table.cellBuf == null || table.cellBuf.Count == 0)
                return;

            // 병합 셀 추적: (masterX, masterY) → (maxX, maxY)
            var mergeMap = new Dictionary<long, MergeInfo>();

            // 열 너비 / 행 높이 추적
            var colWidths = new Dictionary<int, int>();  // col → max width px
            var rowHeights = new Dictionary<int, int>(); // row → max height px

            foreach (var item in table.cellBuf)
            {
                var cell = item as CELL_STRUCT;
                if (cell == null) continue;

                // 레거시 좌표는 0-based, 모델은 1-based
                int row = cell.y + 1;
                int col = cell.x + 1;

                // cGroup: 0=일반, 1=병합 시작(마스터), 2=병합 내 숨김
                if (cell.cGroup == 2)
                {
                    // 숨김 셀: 마스터 셀의 병합 범위 확장
                    int masterCol = cell.nGroupX + 1;
                    int masterRow = cell.nGroupY + 1;
                    long masterKey = ((long)masterRow << 32) | (uint)masterCol;

                    if (mergeMap.TryGetValue(masterKey, out var info))
                    {
                        if (row > info.MaxRow) info.MaxRow = row;
                        if (col > info.MaxCol) info.MaxCol = col;
                    }
                    else
                    {
                        mergeMap[masterKey] = new MergeInfo
                        {
                            MasterRow = masterRow,
                            MasterCol = masterCol,
                            MaxRow = row,
                            MaxCol = col
                        };
                    }
                    continue; // 숨김 셀은 데이터 변환 스킵
                }

                // 셀 모델 생성
                var cellModel = new CellModel();

                // 텍스트 / 수식 판별
                string text = cell.text ?? string.Empty;
                if (text.StartsWith("="))
                {
                    cellModel.Formula = text;
                    cellModel.DataType = CellDataType.Formula;
                }
                else
                {
                    cellModel.Value = ParseCellValue(text);
                    cellModel.DataType = DetectDataType(cellModel.Value);
                }

                // 스타일 변환
                cellModel.Style = ConvertStyle(cell);

                // 숫자 포맷 변환
                string numFmt = ConvertDisplayFormat(cell.format);
                if (numFmt != null)
                    cellModel.Style.NumberFormat = numFmt;

                sheet.SetCell(row, col, cellModel);

                // 열 너비 추적 (같은 열에서 최대값)
                if (cell.width > 0)
                {
                    if (!colWidths.TryGetValue(col, out int existing) || cell.width > existing)
                        colWidths[col] = cell.width;
                }

                // 행 높이 추적 (같은 행에서 최대값)
                if (cell.height > 0)
                {
                    if (!rowHeights.TryGetValue(row, out int existing) || cell.height > existing)
                        rowHeights[row] = cell.height;
                }

                // 병합 마스터 셀 등록
                if (cell.cGroup == 1)
                {
                    long masterKey = ((long)row << 32) | (uint)col;
                    if (!mergeMap.ContainsKey(masterKey))
                    {
                        mergeMap[masterKey] = new MergeInfo
                        {
                            MasterRow = row,
                            MasterCol = col,
                            MaxRow = row,
                            MaxCol = col
                        };
                    }
                }
            }

            // 열 너비 설정 (픽셀 → 문자폭)
            foreach (var kv in colWidths)
            {
                double charWidth = kv.Value / PixelsPerCharWidth;
                if (charWidth < 1) charWidth = 1;
                sheet.ColumnWidths[kv.Key] = Math.Round(charWidth, 2);
            }

            // 행 높이 설정 (픽셀 → 포인트)
            foreach (var kv in rowHeights)
            {
                double points = kv.Value / PixelsPerPoint;
                if (points < 1) points = 1;
                sheet.RowHeights[kv.Key] = Math.Round(points, 2);
            }

            // 병합 범위 등록
            foreach (var kv in mergeMap)
            {
                var info = kv.Value;
                // 실제 병합인 경우만 (1셀 이상 확장)
                if (info.MaxRow > info.MasterRow || info.MaxCol > info.MasterCol)
                {
                    sheet.MergedRanges.Add(new MergedRange(
                        info.MasterRow, info.MasterCol,
                        info.MaxRow, info.MaxCol));
                }
            }
        }

        /// <summary>
        /// CELL_STRUCT → StyleModel 변환
        /// </summary>
        private static StyleModel ConvertStyle(CELL_STRUCT cell)
        {
            var style = new StyleModel();

            // 폰트
            style.Font.Name = string.IsNullOrEmpty(cell.fontName) ? "Gulim" : cell.fontName;
            style.Font.Size = cell.fontSize > 0 ? cell.fontSize : 9;
            style.Font.Color = cell.tcolor;
            style.Font.Bold = (cell.fontStyle & FontStyle.Bold) != 0;
            style.Font.Italic = (cell.fontStyle & FontStyle.Italic) != 0;
            style.Font.Underline = (cell.fontStyle & FontStyle.Underline) != 0;
            style.Font.Strikethrough = (cell.fontStyle & FontStyle.Strikeout) != 0;

            // 배경색
            if (cell.bcolor != Color.White && cell.bcolor != Color.Empty)
            {
                style.Fill.BackgroundColor = cell.bcolor;
                style.Fill.PatternType = FillPattern.Solid;
            }

            // 정렬
            // 레거시: cAlignHorz 0=Left, 1=Center, 2=Right
            switch (cell.cAlignHorz)
            {
                case 0: style.Alignment.Horizontal = HorizontalAlign.Left; break;
                case 1: style.Alignment.Horizontal = HorizontalAlign.Center; break;
                case 2: style.Alignment.Horizontal = HorizontalAlign.Right; break;
                default: style.Alignment.Horizontal = HorizontalAlign.Center; break;
            }
            // 레거시: cAlignVert 0=Top, 1=Center, 2=Bottom
            switch (cell.cAlignVert)
            {
                case 0: style.Alignment.Vertical = VerticalAlign.Top; break;
                case 1: style.Alignment.Vertical = VerticalAlign.Middle; break;
                case 2: style.Alignment.Vertical = VerticalAlign.Bottom; break;
                default: style.Alignment.Vertical = VerticalAlign.Middle; break;
            }

            // 테두리 (border[0]=left, [1]=top, [2]=right, [3]=bottom, [4,5]=대각선 무시)
            if (cell.border != null && cell.border.Length >= 4)
            {
                style.Border.Left = ConvertBorderEdge(cell.border[0]);
                style.Border.Top = ConvertBorderEdge(cell.border[1]);
                style.Border.Right = ConvertBorderEdge(cell.border[2]);
                style.Border.Bottom = ConvertBorderEdge(cell.border[3]);
            }

            return style;
        }

        /// <summary>
        /// BORDER_STRUCT → BorderEdge 변환
        /// </summary>
        private static BorderEdge ConvertBorderEdge(BORDER_STRUCT border)
        {
            var edge = new BorderEdge();
            edge.Color = border.color;

            // type: 0=없음, 1=실선, 2=이중선
            switch (border.type)
            {
                case 0:
                    edge.Style = BorderLineStyle.None;
                    break;
                case 1:
                    // thick에 따라 세분화
                    if (border.thick >= 3)
                        edge.Style = BorderLineStyle.Thick;
                    else if (border.thick >= 2)
                        edge.Style = BorderLineStyle.Medium;
                    else
                        edge.Style = BorderLineStyle.Thin;
                    break;
                case 2:
                    edge.Style = BorderLineStyle.Double;
                    break;
                default:
                    edge.Style = BorderLineStyle.Thin;
                    break;
            }

            return edge;
        }

        /// <summary>
        /// DISPLAY_FORMAT_STRUCT → 엑셀 NumberFormat 문자열
        /// </summary>
        private static string ConvertDisplayFormat(DISPLAY_FORMAT_STRUCT format)
        {
            // cType 0 = 기본(서식 없음)
            if (format.cType == 0 && string.IsNullOrEmpty(format.sUserFormat))
                return null;

            // 사용자 정의 포맷이 있으면 우선 사용
            if (!string.IsNullOrEmpty(format.sUserFormat))
                return format.sUserFormat;

            // 숫자 서식 생성
            // cType: 숫자 관련
            string decimalPart = "";
            if (format.cUnderPoint > 0)
            {
                decimalPart = "." + new string('0', format.cUnderPoint);
            }

            string integerPart = format.bThousandComma != 0 ? "#,##0" : "0";

            // 날짜/시간 서식
            if (format.cDateTime > 0)
            {
                return ConvertDateTimeFormat(format.cDateTime, format.bWeekAdd != 0);
            }

            // 경과 시간
            if (format.cTimeCount > 0)
            {
                return ConvertTimeCountFormat(format.cTimeCount);
            }

            if (format.cType != 0 || format.cUnderPoint > 0 || format.bThousandComma != 0)
            {
                return integerPart + decimalPart;
            }

            return null;
        }

        /// <summary>
        /// 날짜/시간 포맷 변환
        /// </summary>
        private static string ConvertDateTimeFormat(sbyte cDateTime, bool weekAdd)
        {
            string weekSuffix = weekAdd ? " (ddd)" : "";
            switch (cDateTime)
            {
                case 1: return "yyyy-MM-dd" + weekSuffix;
                case 2: return "yyyy/MM/dd" + weekSuffix;
                case 3: return "yy-MM-dd" + weekSuffix;
                case 4: return "MM-dd" + weekSuffix;
                case 5: return "HH:mm:ss";
                case 6: return "HH:mm";
                case 7: return "yyyy-MM-dd HH:mm:ss";
                case 8: return "yyyy-MM-dd HH:mm";
                default: return "yyyy-MM-dd HH:mm:ss";
            }
        }

        /// <summary>
        /// 경과 시간 포맷 변환
        /// </summary>
        private static string ConvertTimeCountFormat(sbyte cTimeCount)
        {
            switch (cTimeCount)
            {
                case 1: return "[h]:mm:ss";
                case 2: return "[h]:mm";
                case 3: return "[m]:ss";
                default: return "[h]:mm:ss";
            }
        }

        /// <summary>
        /// 텍스트 값을 적절한 타입으로 파싱
        /// </summary>
        private static object ParseCellValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            // 숫자 시도
            if (double.TryParse(text, out double dval))
                return dval;

            // 날짜 시도
            if (DateTime.TryParse(text, out DateTime dtval))
                return dtval;

            return text;
        }

        /// <summary>
        /// 값으로부터 데이터 타입 감지
        /// </summary>
        private static CellDataType DetectDataType(object value)
        {
            if (value == null) return CellDataType.String;
            if (value is double) return CellDataType.Number;
            if (value is DateTime) return CellDataType.DateTime;
            if (value is bool) return CellDataType.Boolean;
            return CellDataType.String;
        }

        /// <summary>병합 셀 정보 임시 저장</summary>
        private class MergeInfo
        {
            public int MasterRow;
            public int MasterCol;
            public int MaxRow;
            public int MaxCol;
        }
    }
}
