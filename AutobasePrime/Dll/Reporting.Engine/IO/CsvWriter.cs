using System;
using System.IO;
using System.Text;
using Reporting.Engine.Model;

namespace Reporting.Engine.IO
{
    /// <summary>
    /// CSV 출력
    /// </summary>
    public static class CsvWriter
    {
        /// <summary>
        /// 활성 시트를 CSV로 저장
        /// </summary>
        public static void Write(WorkbookModel workbook, string filePath, string delimiter = ",")
        {
            if (workbook == null)
                throw new ArgumentNullException(nameof(workbook));

            var sheet = workbook.ActiveSheet;
            if (sheet == null)
                throw new InvalidOperationException("활성 시트가 없습니다.");

            WriteSheet(sheet, filePath, delimiter);
        }

        /// <summary>
        /// 지정 시트를 CSV로 저장
        /// </summary>
        public static void WriteSheet(SheetModel sheet, string filePath, string delimiter = ",")
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            int maxRow = sheet.UsedRowCount;
            int maxCol = sheet.UsedColumnCount;

            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
            {
                for (int row = 1; row <= maxRow; row++)
                {
                    var sb = new StringBuilder();
                    for (int col = 1; col <= maxCol; col++)
                    {
                        if (col > 1) sb.Append(delimiter);

                        var cell = sheet.GetCell(row, col);
                        if (cell != null)
                        {
                            string text = cell.GetDisplayText();
                            // CSV 이스케이프: 구분자, 따옴표, 줄바꿈 포함 시 따옴표로 감싸기
                            if (text.Contains(delimiter) || text.Contains("\"") || text.Contains("\n") || text.Contains("\r"))
                            {
                                text = "\"" + text.Replace("\"", "\"\"") + "\"";
                            }
                            sb.Append(text);
                        }
                    }
                    writer.WriteLine(sb.ToString());
                }
            }
        }
    }
}
