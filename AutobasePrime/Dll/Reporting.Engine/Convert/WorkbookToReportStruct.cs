using System;
using System.Collections;
using System.Drawing;
using Reporting.Engine.Model;
using ReportBasicLib;

namespace Reporting.Engine.Convert
{
    /// <summary>
    /// WorkbookModel → REPORT_STRUCT 변환기
    /// 새 리포트 모델을 레거시 실행 엔진(MakeRunReport)이 처리할 수 있는 구조로 변환한다.
    /// </summary>
    public static class WorkbookToReportStruct
    {
        // 문자폭→픽셀 변환 (RptxToWorkbook 역)
        private const double PixelsPerCharWidth = 7.5;
        private const double PixelsPerPoint = 1.333;
        private const int DefaultCellWidthPx = 80;
        private const int DefaultCellHeightPx = 25;

        /// <summary>
        /// WorkbookModel을 REPORT_STRUCT로 변환
        /// </summary>
        /// <param name="workbook">새 리포트 모델</param>
        /// <returns>레거시 REPORT_STRUCT</returns>
        public static REPORT_STRUCT Convert(WorkbookModel workbook)
        {
            if (workbook == null)
                throw new ArgumentNullException(nameof(workbook));

            var report = new REPORT_STRUCT();

            // 첫 시트의 페이지 설정으로 리포트 설정
            if (workbook.Sheets.Count > 0)
            {
                var setup = workbook.Sheets[0].PageSetup;
                if (setup != null)
                {
                    report.margin_left = (int)Math.Round(setup.MarginLeft);
                    report.margin_right = (int)Math.Round(setup.MarginRight);
                    report.margin_top = (int)Math.Round(setup.MarginTop);
                    report.margin_bottom = (int)Math.Round(setup.MarginBottom);
                    report.bOrientation = (sbyte)(setup.Orientation == PageOrientation.Landscape ? 1 : 0);
                    report.wOpticRate = setup.ScalePercent;
                }
            }

            // 각 시트 → TABLE_STRUCT
            for (int i = 0; i < workbook.Sheets.Count; i++)
            {
                var sheet = workbook.Sheets[i];
                var table = ConvertSheet(sheet, i);
                if (table != null)
                    report.tableBuf.Add(table);
            }

            return report;
        }

        /// <summary>
        /// SheetModel → TABLE_STRUCT
        /// </summary>
        private static TABLE_STRUCT ConvertSheet(SheetModel sheet, int tableIndex)
        {
            int maxRow = sheet.UsedRowCount;
            int maxCol = sheet.UsedColumnCount;

            if (maxRow == 0 || maxCol == 0)
                return null;

            var table = new TABLE_STRUCT();
            table.no = tableIndex;
            table.cell_x = maxCol;
            table.cell_y = maxRow;
            table.gab_left = 0;
            table.gab_top = 2;

            // 행 우선 순서로 셀 생성 (레거시 형식: pos = cell_x * y + x)
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    int modelRow = row + 1; // 1-based
                    int modelCol = col + 1;

                    var cellModel = sheet.GetCell(modelRow, modelCol);
                    var cell = new CELL_STRUCT();
                    FillDefaultCell(cell);
                    cell.x = col;
                    cell.y = row;

                    if (cellModel != null)
                    {
                        // 텍스트/수식
                        if (cellModel.HasFormula)
                        {
                            cell.text = cellModel.Formula ?? "";
                        }
                        else
                        {
                            cell.text = cellModel.Value?.ToString() ?? "";
                        }

                        // 스타일 변환
                        if (cellModel.Style != null)
                        {
                            ConvertStyleToCell(cellModel.Style, cell);
                        }

                        // 숫자 포맷
                        if (cellModel.Style?.NumberFormat != null &&
                            cellModel.Style.NumberFormat != "General")
                        {
                            cell.format.sUserFormat = cellModel.Style.NumberFormat;
                            cell.format.cType = 1;
                        }
                    }

                    // 열 너비 (문자폭 → 픽셀)
                    double charWidth = sheet.GetColumnWidth(modelCol);
                    cell.width = (int)Math.Round(charWidth * PixelsPerCharWidth);
                    if (cell.width < 1) cell.width = DefaultCellWidthPx;

                    // 행 높이 (포인트 → 픽셀)
                    double points = sheet.GetRowHeight(modelRow);
                    cell.height = (int)Math.Round(points * PixelsPerPoint);
                    if (cell.height < 1) cell.height = DefaultCellHeightPx;

                    // 병합 처리
                    var merged = sheet.FindMergedRange(modelRow, modelCol);
                    if (merged != null)
                    {
                        if (merged.IsOrigin(modelRow, modelCol))
                        {
                            cell.cGroup = 1; // 마스터
                            cell.nGroupX = col;
                            cell.nGroupY = row;
                        }
                        else
                        {
                            cell.cGroup = 2; // 숨김
                            cell.nGroupX = merged.FirstCol - 1; // 0-based 마스터 좌표
                            cell.nGroupY = merged.FirstRow - 1;
                            cell.text = ""; // 숨김 셀은 텍스트 없음
                        }
                    }

                    table.cellBuf.Add(cell);
                }
            }

            return table;
        }

        /// <summary>
        /// StyleModel → CELL_STRUCT 스타일 필드
        /// </summary>
        private static void ConvertStyleToCell(StyleModel style, CELL_STRUCT cell)
        {
            // 폰트
            if (style.Font != null)
            {
                cell.fontName = style.Font.Name ?? "Gulim";
                cell.fontSize = (float)style.Font.Size;
                cell.tcolor = style.Font.Color;

                FontStyle fs = FontStyle.Regular;
                if (style.Font.Bold) fs |= FontStyle.Bold;
                if (style.Font.Italic) fs |= FontStyle.Italic;
                if (style.Font.Underline) fs |= FontStyle.Underline;
                if (style.Font.Strikethrough) fs |= FontStyle.Strikeout;
                cell.fontStyle = fs;
            }

            // 배경색
            if (style.Fill != null && style.Fill.HasFill)
            {
                cell.bcolor = style.Fill.BackgroundColor;
            }

            // 정렬
            if (style.Alignment != null)
            {
                switch (style.Alignment.Horizontal)
                {
                    case HorizontalAlign.Left: cell.cAlignHorz = 0; break;
                    case HorizontalAlign.Center: cell.cAlignHorz = 1; break;
                    case HorizontalAlign.Right: cell.cAlignHorz = 2; break;
                    default: cell.cAlignHorz = 1; break;
                }
                switch (style.Alignment.Vertical)
                {
                    case VerticalAlign.Top: cell.cAlignVert = 0; break;
                    case VerticalAlign.Middle: cell.cAlignVert = 1; break;
                    case VerticalAlign.Bottom: cell.cAlignVert = 2; break;
                    default: cell.cAlignVert = 1; break;
                }
            }

            // 테두리
            if (style.Border != null)
            {
                ConvertBorderEdgeToStruct(style.Border.Left, ref cell.border[0]);
                ConvertBorderEdgeToStruct(style.Border.Top, ref cell.border[1]);
                ConvertBorderEdgeToStruct(style.Border.Right, ref cell.border[2]);
                ConvertBorderEdgeToStruct(style.Border.Bottom, ref cell.border[3]);
            }
        }

        /// <summary>
        /// BorderEdge → BORDER_STRUCT 변환
        /// </summary>
        private static void ConvertBorderEdgeToStruct(BorderEdge edge, ref BORDER_STRUCT border)
        {
            if (edge == null || edge.Style == BorderLineStyle.None)
            {
                border.type = 0;
                return;
            }

            border.color = edge.Color;

            switch (edge.Style)
            {
                case BorderLineStyle.Thin:
                    border.type = 1;
                    border.thick = 1;
                    break;
                case BorderLineStyle.Medium:
                    border.type = 1;
                    border.thick = 2;
                    break;
                case BorderLineStyle.Thick:
                    border.type = 1;
                    border.thick = 3;
                    break;
                case BorderLineStyle.Double:
                    border.type = 2;
                    border.thick = 1;
                    break;
                case BorderLineStyle.Dashed:
                    border.type = 1;
                    border.thick = 1;
                    break;
                case BorderLineStyle.Dotted:
                    border.type = 1;
                    border.thick = 1;
                    break;
                default:
                    border.type = 1;
                    border.thick = 1;
                    break;
            }
        }

        /// <summary>
        /// 기본 셀 값으로 초기화 (FillDefaultCell 재현)
        /// </summary>
        private static void FillDefaultCell(CELL_STRUCT cell)
        {
            cell.tcolor = Color.Black;
            cell.bcolor = Color.White;
            cell.width = DefaultCellWidthPx;
            cell.height = DefaultCellHeightPx;
            cell.cAlignHorz = 1;
            cell.cAlignVert = 1;
            cell.fontSize = 9;
            cell.fontName = "Gulim";
            cell.text = "";
            cell.format.cUnderPoint = 2;

            for (int i = 0; i < 6; i++)
            {
                cell.border[i].color = Color.Black;
                cell.border[i].thick = 1;
            }
            for (int i = 0; i < 4; i++)
            {
                cell.border[i].type = 1;
            }
        }
    }
}
