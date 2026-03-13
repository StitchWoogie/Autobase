using System;
using System.IO;
using ClosedXML.Excel;
using Reporting.Engine.Model;

namespace Reporting.Engine.IO
{
    /// <summary>
    /// ClosedXML 기반 .xlsx 파일 읽기
    /// </summary>
    public static class XlsxReader
    {
        /// <summary>
        /// .xlsx 파일을 읽어 WorkbookModel로 변환
        /// </summary>
        public static WorkbookModel Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("파일을 찾을 수 없습니다.", filePath);

            using (var xlWorkbook = new XLWorkbook(filePath))
            {
                return ConvertFromXL(xlWorkbook);
            }
        }

        /// <summary>
        /// Stream에서 읽기
        /// </summary>
        public static WorkbookModel Read(Stream stream)
        {
            using (var xlWorkbook = new XLWorkbook(stream))
            {
                return ConvertFromXL(xlWorkbook);
            }
        }

        private static WorkbookModel ConvertFromXL(XLWorkbook xlWorkbook)
        {
            var wb = new WorkbookModel();

            foreach (var xlSheet in xlWorkbook.Worksheets)
            {
                var sheet = new SheetModel(xlSheet.Name);
                ReadSheetProperties(xlSheet, sheet);
                ReadCells(xlSheet, sheet);
                ReadMergedRanges(xlSheet, sheet);
                ReadColumnWidths(xlSheet, sheet);
                ReadRowHeights(xlSheet, sheet);
                ReadPageSetup(xlSheet, sheet);
                wb.Sheets.Add(sheet);
            }

            if (wb.Sheets.Count == 0)
                wb.Sheets.Add(new SheetModel("Sheet1"));

            return wb;
        }

        private static void ReadSheetProperties(IXLWorksheet xlSheet, SheetModel sheet)
        {
            sheet.DefaultColumnWidth = xlSheet.ColumnWidth;
            sheet.DefaultRowHeight = xlSheet.RowHeight;
        }

        private static void ReadCells(IXLWorksheet xlSheet, SheetModel sheet)
        {
            var range = xlSheet.RangeUsed();
            if (range == null) return;

            foreach (var xlCell in range.CellsUsed(XLCellsUsedOptions.All))
            {
                int row = xlCell.Address.RowNumber;
                int col = xlCell.Address.ColumnNumber;

                var cell = new CellModel();

                // 수식
                if (xlCell.HasFormula)
                {
                    cell.Formula = "=" + xlCell.FormulaA1;
                    cell.DataType = CellDataType.Formula;
                }

                // 값
                ReadCellValue(xlCell, cell);

                // 서식
                cell.Style = ReadCellStyle(xlCell.Style);

                sheet.SetCell(row, col, cell);
            }
        }

        private static void ReadCellValue(IXLCell xlCell, CellModel cell)
        {
            try
            {
                switch (xlCell.DataType)
                {
                    case XLDataType.Number:
                        cell.Value = xlCell.GetDouble();
                        if (!cell.HasFormula)
                            cell.DataType = CellDataType.Number;
                        break;
                    case XLDataType.DateTime:
                        cell.Value = xlCell.GetDateTime();
                        if (!cell.HasFormula)
                            cell.DataType = CellDataType.DateTime;
                        break;
                    case XLDataType.Boolean:
                        cell.Value = xlCell.GetBoolean();
                        if (!cell.HasFormula)
                            cell.DataType = CellDataType.Boolean;
                        break;
                    case XLDataType.Text:
                        cell.Value = xlCell.GetString();
                        if (!cell.HasFormula)
                            cell.DataType = CellDataType.String;
                        break;
                    default:
                        if (!xlCell.IsEmpty())
                            cell.Value = xlCell.GetString();
                        break;
                }
            }
            catch
            {
                // 값 읽기 실패 시 문자열로 fallback
                try { cell.Value = xlCell.GetString(); } catch { }
            }
        }

        private static StyleModel ReadCellStyle(IXLStyle xlStyle)
        {
            var style = new StyleModel();

            // 폰트
            var xlFont = xlStyle.Font;
            style.Font.Name = xlFont.FontName;
            style.Font.Size = xlFont.FontSize;
            style.Font.Bold = xlFont.Bold;
            style.Font.Italic = xlFont.Italic;
            style.Font.Underline = xlFont.Underline != XLFontUnderlineValues.None;
            style.Font.Strikethrough = xlFont.Strikethrough;
            if (xlFont.FontColor.HasValue)
                style.Font.Color = ColorFromXL(xlFont.FontColor);

            // 채우기
            var xlFill = xlStyle.Fill;
            if (xlFill.PatternType != XLFillPatternValues.None)
            {
                style.Fill.PatternType = FillPattern.Solid;
                if (xlFill.BackgroundColor.HasValue)
                    style.Fill.BackgroundColor = ColorFromXL(xlFill.BackgroundColor);
            }

            // 보더
            style.Border.Left = ReadBorderEdge(xlStyle.Border.LeftBorder, xlStyle.Border.LeftBorderColor);
            style.Border.Top = ReadBorderEdge(xlStyle.Border.TopBorder, xlStyle.Border.TopBorderColor);
            style.Border.Right = ReadBorderEdge(xlStyle.Border.RightBorder, xlStyle.Border.RightBorderColor);
            style.Border.Bottom = ReadBorderEdge(xlStyle.Border.BottomBorder, xlStyle.Border.BottomBorderColor);

            // 정렬
            style.Alignment.Horizontal = ConvertHAlign(xlStyle.Alignment.Horizontal);
            style.Alignment.Vertical = ConvertVAlign(xlStyle.Alignment.Vertical);
            style.Alignment.WrapText = xlStyle.Alignment.WrapText;
            style.Alignment.TextRotation = xlStyle.Alignment.TextRotation;

            // 숫자 형식
            style.NumberFormat = xlStyle.NumberFormat.Format;
            if (string.IsNullOrEmpty(style.NumberFormat))
                style.NumberFormat = "General";

            return style;
        }

        private static BorderEdge ReadBorderEdge(XLBorderStyleValues xlStyle, XLColor xlColor)
        {
            var edge = new BorderEdge();
            edge.Style = ConvertBorderStyle(xlStyle);
            if (xlColor != null && xlColor.HasValue)
                edge.Color = ColorFromXL(xlColor);
            return edge;
        }

        private static System.Drawing.Color ColorFromXL(XLColor xlColor)
        {
            try
            {
                if (xlColor.ColorType == XLColorType.Color)
                    return xlColor.Color;
                if (xlColor.ColorType == XLColorType.Theme)
                    return xlColor.Color;
            }
            catch { }
            return System.Drawing.Color.Black;
        }

        private static BorderLineStyle ConvertBorderStyle(XLBorderStyleValues xlStyle)
        {
            switch (xlStyle)
            {
                case XLBorderStyleValues.Thin: return BorderLineStyle.Thin;
                case XLBorderStyleValues.Medium: return BorderLineStyle.Medium;
                case XLBorderStyleValues.Thick: return BorderLineStyle.Thick;
                case XLBorderStyleValues.Dashed:
                case XLBorderStyleValues.MediumDashed: return BorderLineStyle.Dashed;
                case XLBorderStyleValues.Dotted: return BorderLineStyle.Dotted;
                case XLBorderStyleValues.Double: return BorderLineStyle.Double;
                default: return BorderLineStyle.None;
            }
        }

        private static HorizontalAlign ConvertHAlign(XLAlignmentHorizontalValues xlAlign)
        {
            switch (xlAlign)
            {
                case XLAlignmentHorizontalValues.Left: return HorizontalAlign.Left;
                case XLAlignmentHorizontalValues.Center: return HorizontalAlign.Center;
                case XLAlignmentHorizontalValues.Right: return HorizontalAlign.Right;
                default: return HorizontalAlign.General;
            }
        }

        private static VerticalAlign ConvertVAlign(XLAlignmentVerticalValues xlAlign)
        {
            switch (xlAlign)
            {
                case XLAlignmentVerticalValues.Top: return VerticalAlign.Top;
                case XLAlignmentVerticalValues.Center: return VerticalAlign.Middle;
                case XLAlignmentVerticalValues.Bottom: return VerticalAlign.Bottom;
                default: return VerticalAlign.Bottom;
            }
        }

        private static void ReadMergedRanges(IXLWorksheet xlSheet, SheetModel sheet)
        {
            foreach (var merged in xlSheet.MergedRanges)
            {
                sheet.MergedRanges.Add(new MergedRange(
                    merged.FirstRow().RowNumber(),
                    merged.FirstColumn().ColumnNumber(),
                    merged.LastRow().RowNumber(),
                    merged.LastColumn().ColumnNumber()));
            }
        }

        private static void ReadColumnWidths(IXLWorksheet xlSheet, SheetModel sheet)
        {
            foreach (var xlCol in xlSheet.ColumnsUsed())
            {
                double w = xlCol.Width;
                if (Math.Abs(w - xlSheet.ColumnWidth) > 0.01)
                    sheet.ColumnWidths[xlCol.ColumnNumber()] = w;
            }
        }

        private static void ReadRowHeights(IXLWorksheet xlSheet, SheetModel sheet)
        {
            foreach (var xlRow in xlSheet.RowsUsed())
            {
                double h = xlRow.Height;
                if (Math.Abs(h - xlSheet.RowHeight) > 0.01)
                    sheet.RowHeights[xlRow.RowNumber()] = h;
            }
        }

        private static void ReadPageSetup(IXLWorksheet xlSheet, SheetModel sheet)
        {
            var ps = xlSheet.PageSetup;
            var model = sheet.PageSetup;

            model.Orientation = ps.PageOrientation == XLPageOrientation.Landscape
                ? PageOrientation.Landscape : PageOrientation.Portrait;

            var margins = ps.Margins;
            // ClosedXML 여백은 인치 단위이므로 mm로 변환
            model.MarginLeft = margins.Left * 25.4;
            model.MarginRight = margins.Right * 25.4;
            model.MarginTop = margins.Top * 25.4;
            model.MarginBottom = margins.Bottom * 25.4;

            // 머리글/바닥글
            var hf = ps.Header;
            if (hf != null)
            {
                model.HeaderLeft = hf.Left?.GetText(XLHFOccurrence.AllPages);
                model.HeaderCenter = hf.Center?.GetText(XLHFOccurrence.AllPages);
                model.HeaderRight = hf.Right?.GetText(XLHFOccurrence.AllPages);
            }
            var ff = ps.Footer;
            if (ff != null)
            {
                model.FooterLeft = ff.Left?.GetText(XLHFOccurrence.AllPages);
                model.FooterCenter = ff.Center?.GetText(XLHFOccurrence.AllPages);
                model.FooterRight = ff.Right?.GetText(XLHFOccurrence.AllPages);
            }
        }
    }
}
