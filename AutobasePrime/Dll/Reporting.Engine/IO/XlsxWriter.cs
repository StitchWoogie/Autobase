using System;
using System.IO;
using ClosedXML.Excel;
using Reporting.Engine.Model;

namespace Reporting.Engine.IO
{
    /// <summary>
    /// ClosedXML 기반 .xlsx 파일 쓰기
    /// </summary>
    public static class XlsxWriter
    {
        /// <summary>
        /// WorkbookModel을 .xlsx 파일로 저장
        /// </summary>
        public static void Write(WorkbookModel workbook, string filePath)
        {
            if (workbook == null)
                throw new ArgumentNullException(nameof(workbook));

            using (var xlWorkbook = ConvertToXL(workbook))
            {
                xlWorkbook.SaveAs(filePath);
            }
        }

        /// <summary>
        /// Stream으로 저장
        /// </summary>
        public static void Write(WorkbookModel workbook, Stream stream)
        {
            if (workbook == null)
                throw new ArgumentNullException(nameof(workbook));

            using (var xlWorkbook = ConvertToXL(workbook))
            {
                xlWorkbook.SaveAs(stream);
            }
        }

        private static XLWorkbook ConvertToXL(WorkbookModel workbook)
        {
            var xlWorkbook = new XLWorkbook();

            foreach (var sheet in workbook.Sheets)
            {
                var xlSheet = xlWorkbook.Worksheets.Add(sheet.Name);
                WriteSheetProperties(sheet, xlSheet);
                WriteCells(sheet, xlSheet);
                WriteMergedRanges(sheet, xlSheet);
                WriteColumnWidths(sheet, xlSheet);
                WriteRowHeights(sheet, xlSheet);
                WritePageSetup(sheet, xlSheet);
            }

            return xlWorkbook;
        }

        private static void WriteSheetProperties(SheetModel sheet, IXLWorksheet xlSheet)
        {
            xlSheet.ColumnWidth = sheet.DefaultColumnWidth;
            xlSheet.RowHeight = sheet.DefaultRowHeight;
        }

        private static void WriteCells(SheetModel sheet, IXLWorksheet xlSheet)
        {
            foreach (var kv in sheet.Cells)
            {
                var addr = kv.Key;
                var cell = kv.Value;
                var xlCell = xlSheet.Cell(addr.Row, addr.Col);

                // 수식
                if (cell.HasFormula)
                {
                    string formula = cell.Formula;
                    if (formula.StartsWith("="))
                        formula = formula.Substring(1);
                    try
                    {
                        xlCell.FormulaA1 = formula;
                    }
                    catch
                    {
                        // 수식 설정 실패 시 값으로 대체
                        WriteCellValue(xlCell, cell);
                    }
                }
                else
                {
                    WriteCellValue(xlCell, cell);
                }

                // 서식
                if (cell.Style != null)
                    ApplyCellStyle(xlCell, cell.Style);
            }
        }

        private static void WriteCellValue(IXLCell xlCell, CellModel cell)
        {
            if (cell.Value == null) return;

            switch (cell.DataType)
            {
                case CellDataType.Number:
                    if (cell.Value is double d)
                        xlCell.Value = d;
                    else if (double.TryParse(cell.Value.ToString(), out double parsed))
                        xlCell.Value = parsed;
                    else
                        xlCell.Value = cell.Value.ToString();
                    break;

                case CellDataType.DateTime:
                    if (cell.Value is DateTime dt)
                        xlCell.Value = dt;
                    else
                        xlCell.Value = cell.Value.ToString();
                    break;

                case CellDataType.Boolean:
                    if (cell.Value is bool b)
                        xlCell.Value = b;
                    else
                        xlCell.Value = cell.Value.ToString();
                    break;

                default:
                    xlCell.Value = cell.Value.ToString();
                    break;
            }
        }

        private static void ApplyCellStyle(IXLCell xlCell, StyleModel style)
        {
            // 폰트
            if (style.Font != null)
            {
                var xlFont = xlCell.Style.Font;
                if (!string.IsNullOrEmpty(style.Font.Name))
                    xlFont.FontName = style.Font.Name;
                xlFont.FontSize = style.Font.Size;
                xlFont.Bold = style.Font.Bold;
                xlFont.Italic = style.Font.Italic;
                xlFont.Underline = style.Font.Underline
                    ? XLFontUnderlineValues.Single : XLFontUnderlineValues.None;
                xlFont.Strikethrough = style.Font.Strikethrough;
                if (style.Font.Color != System.Drawing.Color.Black)
                    xlFont.FontColor = XLColor.FromColor(style.Font.Color);
            }

            // 채우기
            if (style.Fill != null && style.Fill.HasFill)
            {
                xlCell.Style.Fill.PatternType = XLFillPatternValues.Solid;
                xlCell.Style.Fill.BackgroundColor = XLColor.FromColor(style.Fill.BackgroundColor);
            }

            // 보더
            if (style.Border != null && style.Border.HasAnyBorder)
            {
                ApplyBorderEdge(xlCell.Style.Border, style.Border.Left, "Left");
                ApplyBorderEdge(xlCell.Style.Border, style.Border.Top, "Top");
                ApplyBorderEdge(xlCell.Style.Border, style.Border.Right, "Right");
                ApplyBorderEdge(xlCell.Style.Border, style.Border.Bottom, "Bottom");
            }

            // 정렬
            if (style.Alignment != null)
            {
                xlCell.Style.Alignment.Horizontal = ConvertHAlign(style.Alignment.Horizontal);
                xlCell.Style.Alignment.Vertical = ConvertVAlign(style.Alignment.Vertical);
                xlCell.Style.Alignment.WrapText = style.Alignment.WrapText;
                xlCell.Style.Alignment.TextRotation = style.Alignment.TextRotation;
            }

            // 숫자 형식
            if (!string.IsNullOrEmpty(style.NumberFormat) && style.NumberFormat != "General")
            {
                xlCell.Style.NumberFormat.Format = style.NumberFormat;
            }
        }

        private static void ApplyBorderEdge(IXLBorder xlBorder, BorderEdge edge, string side)
        {
            if (edge == null || edge.Style == BorderLineStyle.None) return;

            var xlStyle = ConvertBorderStyle(edge.Style);
            var xlColor = XLColor.FromColor(edge.Color);

            switch (side)
            {
                case "Left":
                    xlBorder.LeftBorder = xlStyle;
                    xlBorder.LeftBorderColor = xlColor;
                    break;
                case "Top":
                    xlBorder.TopBorder = xlStyle;
                    xlBorder.TopBorderColor = xlColor;
                    break;
                case "Right":
                    xlBorder.RightBorder = xlStyle;
                    xlBorder.RightBorderColor = xlColor;
                    break;
                case "Bottom":
                    xlBorder.BottomBorder = xlStyle;
                    xlBorder.BottomBorderColor = xlColor;
                    break;
            }
        }

        private static XLBorderStyleValues ConvertBorderStyle(BorderLineStyle style)
        {
            switch (style)
            {
                case BorderLineStyle.Thin: return XLBorderStyleValues.Thin;
                case BorderLineStyle.Medium: return XLBorderStyleValues.Medium;
                case BorderLineStyle.Thick: return XLBorderStyleValues.Thick;
                case BorderLineStyle.Dashed: return XLBorderStyleValues.Dashed;
                case BorderLineStyle.Dotted: return XLBorderStyleValues.Dotted;
                case BorderLineStyle.Double: return XLBorderStyleValues.Double;
                default: return XLBorderStyleValues.None;
            }
        }

        private static XLAlignmentHorizontalValues ConvertHAlign(HorizontalAlign align)
        {
            switch (align)
            {
                case HorizontalAlign.Left: return XLAlignmentHorizontalValues.Left;
                case HorizontalAlign.Center: return XLAlignmentHorizontalValues.Center;
                case HorizontalAlign.Right: return XLAlignmentHorizontalValues.Right;
                default: return XLAlignmentHorizontalValues.General;
            }
        }

        private static XLAlignmentVerticalValues ConvertVAlign(VerticalAlign align)
        {
            switch (align)
            {
                case VerticalAlign.Top: return XLAlignmentVerticalValues.Top;
                case VerticalAlign.Middle: return XLAlignmentVerticalValues.Center;
                case VerticalAlign.Bottom: return XLAlignmentVerticalValues.Bottom;
                default: return XLAlignmentVerticalValues.Bottom;
            }
        }

        private static void WriteMergedRanges(SheetModel sheet, IXLWorksheet xlSheet)
        {
            foreach (var mr in sheet.MergedRanges)
            {
                xlSheet.Range(mr.FirstRow, mr.FirstCol, mr.LastRow, mr.LastCol).Merge();
            }
        }

        private static void WriteColumnWidths(SheetModel sheet, IXLWorksheet xlSheet)
        {
            foreach (var kv in sheet.ColumnWidths)
            {
                xlSheet.Column(kv.Key).Width = kv.Value;
            }
        }

        private static void WriteRowHeights(SheetModel sheet, IXLWorksheet xlSheet)
        {
            foreach (var kv in sheet.RowHeights)
            {
                xlSheet.Row(kv.Key).Height = kv.Value;
            }
        }

        private static void WritePageSetup(SheetModel sheet, IXLWorksheet xlSheet)
        {
            var model = sheet.PageSetup;
            if (model == null) return;

            var ps = xlSheet.PageSetup;

            ps.PageOrientation = model.Orientation == PageOrientation.Landscape
                ? XLPageOrientation.Landscape : XLPageOrientation.Portrait;

            // mm를 인치로 변환
            ps.Margins.Left = model.MarginLeft / 25.4;
            ps.Margins.Right = model.MarginRight / 25.4;
            ps.Margins.Top = model.MarginTop / 25.4;
            ps.Margins.Bottom = model.MarginBottom / 25.4;

            // 머리글/바닥글
            if (!string.IsNullOrEmpty(model.HeaderLeft))
                ps.Header.Left.AddText(model.HeaderLeft);
            if (!string.IsNullOrEmpty(model.HeaderCenter))
                ps.Header.Center.AddText(model.HeaderCenter);
            if (!string.IsNullOrEmpty(model.HeaderRight))
                ps.Header.Right.AddText(model.HeaderRight);
            if (!string.IsNullOrEmpty(model.FooterLeft))
                ps.Footer.Left.AddText(model.FooterLeft);
            if (!string.IsNullOrEmpty(model.FooterCenter))
                ps.Footer.Center.AddText(model.FooterCenter);
            if (!string.IsNullOrEmpty(model.FooterRight))
                ps.Footer.Right.AddText(model.FooterRight);
        }
    }
}
