using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Management;
using System.Threading.Tasks;


namespace OfficeExcelLibrary
{
    /// <summary>
    /// ServiceExcelReport에서 호출하는 dll 클래스
    /// </summary>
    public class ClassExcel
    {
        public static void MakeResult(string source_file, string target_file, DateTime tData, DateTime tMinListFr, DateTime tMinListTo, bool delete_target_dir)
        {
            string target_path = Path.GetDirectoryName(target_file);

            if (!System.IO.Directory.Exists(target_path))
                System.IO.Directory.CreateDirectory(target_path);

            // 결과 폴더를 삭제한다.
            if (delete_target_dir)
            {
                try
                {
                    File.Delete(target_file);
                }
                catch
                {

                }
                /*
                DirectoryInfo di = new DirectoryInfo(target_path);
                foreach (FileInfo fi in di.GetFiles("*.xls*"))
                {
                    try
                    {
                        File.Delete(fi.FullName);
                    }
                    catch
                    {
                        break; // 삭제가 되지 않으면 무한루프를 돈다.
                    }
                }*/
            }

            try
            {
                System.IO.File.Copy(source_file, target_file, true);
            }
            catch
            {

            }

            Excel.ApplicationClass oExcel = new Microsoft.Office.Interop.Excel.ApplicationClass();
            Excel.Workbook oBook;
            Excel.Workbooks oBooks;

            oExcel.Visible = false;
            oExcel.DisplayAlerts = false;   // 이것을 넣으면 오류메시지 안나올까 해서 추가 9.5.2 

            oBooks = oExcel.Workbooks;

            oBook = oBooks.Open(target_file, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            ConvertData(oExcel, oBook, tData, tMinListFr, tMinListTo);

            // 파일이 저장시 어떤 메시지가 뜨면 웹서비스 상태에서 어떤 메시지가 실행되어 더이상 진행되지 않는 듯 하다. SaveAs도 같은 현상
            oBook.Save();//SaveAs(target_file, Excel.XlFileFormat.xlWorkbookNormal, null, null, null, null, Excel.XlSaveAsAccessMode.xlExclusive, null, null, null, null, null);

            // Close the Workbook object.
            if (oBook != null)
            {
                oBook.Close(false, Type.Missing, Type.Missing);
                oBook = null;
            }

            // Close the ApplicationClass object.
            if (oExcel != null)
            {
                oExcel.Quit();
                oExcel = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            System.Diagnostics.Process[] P = System.Diagnostics.Process.GetProcessesByName("Excel");
            for (int i = 0; i < P.Length; i++)
            {
                if (GetProcessOwner(P[i].Id) == "ASPNET")
                {
                    P[i].Kill();
                }
            }
        }

        static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                    return argList[0];
            }

            return "NO OWNER";
        }

        public void MakeResultDebug(string target_file, bool bAutoPrint, bool bAutoClose, bool bAutoSave, DateTime tData, DateTime tMinListFr, DateTime tMinListTo)
        {
            Excel.ApplicationClass oExcel = new Microsoft.Office.Interop.Excel.ApplicationClass();
            Excel.Workbook oBook;
            Excel.Workbooks oBooks;

            oExcel.Visible = true;
            oBooks = oExcel.Workbooks;

            oBook = oBooks.Open(target_file, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            ConvertData(oExcel, oBook, tData, tMinListFr, tMinListTo);

            oBook.Save();

            if (bAutoClose)
            {
                // Close the Workbook object.
                if (oBook != null)
                {
                    oBook.Close(false, Type.Missing, Type.Missing);
                    oBook = null;
                }

                // Close the ApplicationClass object.
                if (oExcel != null)
                {
                    oExcel.Quit();
                    oExcel = null;
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            /*
            Process[] P = Process.GetProcessesByName("Excel");
            for (int i = 0; i < P.Length; i++)
            {
                P[i].Kill();
            }*/
        }

        static void ConvertData(Excel.ApplicationClass application, Excel.Workbook workbook, DateTime tData, DateTime tMinListFr, DateTime tMinListTo)
        {
            if (application.Workbooks.Count == 0) return; // 열린 파일이 없는 경우

            application.ScreenUpdating = false;
            bool bSaved = application.ActiveWorkbook.Saved;

            int nSheetCount = workbook.Sheets.Count;

            Excel.Worksheet sheet1;
            Excel.Worksheet sheet2;

            int nNextSheet;
            bool bVisibleImsi;

            int LastCol;
            int LastRow;
            int rowCount;
            string strCell;
            int nInsertedLine;
            int length;
            int nLine;
            bool bInsertItem;
            string strVal;

            ExcelReportData.ConnectRoot gExternComObject = new ExcelReportData.ConnectRoot();
            gExternComObject.SetDataTime(tData);
            gExternComObject.SetMinListDataTime(tMinListFr, tMinListTo);

            for (int nSheet = 0; nSheet < nSheetCount; nSheet += 2)
            {
                nNextSheet = nSheet + 1;
                if (nNextSheet >= nSheetCount) break;

                sheet1 = (Excel.Worksheet)workbook.Sheets[nSheet + 1];
                sheet2 = (Excel.Worksheet)workbook.Sheets[nNextSheet + 1];

                bVisibleImsi = true;
                Excel.XlSheetVisibility saveVisible = sheet2.Visible;

                if (sheet2.Visible != Excel.XlSheetVisibility.xlSheetVisible)
                {
                    saveVisible = sheet2.Visible;
                    sheet2.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                    bVisibleImsi = false;
                }

                sheet2.Select(true);
                sheet2.Cells.Select();
                sheet2.Cells.Copy(Type.Missing);
                Excel.Range range = (Excel.Range)sheet2.Cells[1, 1];
                range.Select(); // 복사 시 전체가 선택되어 있으므로 첫번째 셀로 커서를 바꾼다.

                sheet1.Select(true);
                sheet1.Cells.Select();
                sheet1.Paste(Type.Missing, Type.Missing);
                range = (Excel.Range)sheet1.Cells[1, 1];
                range.Select(); // 붙여넣기 시 전체가 선택되어 있으므로 첫번째 셀로 커서를 바꾼다.

                application.CutCopyMode = (Microsoft.Office.Interop.Excel.XlCutCopyMode)0; //Excel.XlCutCopyMode. false;

                if (bVisibleImsi == false)
                {
                    sheet2.Visible = saveVisible;
                }

                nInsertedLine = 0;
                Excel.Range LastCell = sheet2.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Excel.XlSpecialCellsValue.xlTextValues);

                LastCol = LastCell.Column + LastCell.Columns.Count - 1;
                LastRow = LastCell.Row + LastCell.Rows.Count - 1;
                rowCount = LastRow;

                for (int y = 1; y <= rowCount; y++)
                {
                    for (int x = 1; x <= LastCol; x++)
                    {
                        strCell = ((Excel.Range)sheet2.Cells[y, x]).Text as string;
                        if (strCell.Length != 0)
                        {
                            length = gExternComObject.getCellDataReadLineCount(strCell);
                            if (length == 1)
                            {
                                strVal = gExternComObject.getCellDataValue(strCell, 0, 0).GetAwaiter().GetResult();  // 한줄 명령일 경우 마지막 2자리는 항상 0으로 설정
                                sheet1.Cells[y + nInsertedLine, x] = strVal;
                            }
                            else if (length >= 2)
                            {
                                if (length == 3)
                                {
                                    bInsertItem = false;
                                }
                                else
                                {
                                    bInsertItem = true;
                                }

                                nLine = makeMultiLineData(gExternComObject, application, sheet1, x, y + nInsertedLine, strCell, bInsertItem);
                                if (length != 3)
                                {
                                    rowCount = rowCount + nLine;
                                    nInsertedLine += nLine;
                                }
                            }
                        }
                    }
                }
            }

            if (bSaved)  // 이전에 파일 변경이 없을 때만 Saved Flag = True
                application.ActiveWorkbook.Saved = true;

            application.ScreenUpdating = true;

            sheet1 = null;
            sheet2 = null;
        }

        static int makeMultiLineData(ExcelReportData.ConnectRoot gExternComObject, Excel.ApplicationClass application, Excel.Worksheet sheet1, int x, int y, string strCell, bool bInsertItem)
        {
            string strVal;
            int rowCount;
            int columnCount;
            int nRow;
            int nColumn;

            rowCount = (int)gExternComObject.getMultiCellDataRowCount(strCell).GetAwaiter().GetResult();
            columnCount = gExternComObject.getMultiCellDataColumnCount();

            if (rowCount == 0)
            {
                strVal = gExternComObject.getCellDataNoneString(strCell);
                if (columnCount == 0)
                {
                    sheet1.Cells[y, x] = strVal;
                }
                else
                {
                    columnCount -= 1;
                    for (nColumn = 0; nColumn <= columnCount; nColumn++)
                    {
                        sheet1.Cells[y, x + nColumn] = strVal;
                    }
                }
                return 0;
            }

            rowCount -= 1;
            columnCount -= 1;

            Excel.Range range;

            for (nRow = 0; nRow <= rowCount; nRow++)
            {
                if (nRow < rowCount && bInsertItem == true)
                {
                    range = sheet1.Rows[y + nRow, Type.Missing] as Excel.Range;
                    range.Select();
                    range.Copy(Type.Missing);

                    range = sheet1.Rows[y + nRow + 1, Type.Missing] as Excel.Range;
                    range.Insert(Excel.XlDirection.xlDown, Type.Missing);

                    application.CutCopyMode = (Microsoft.Office.Interop.Excel.XlCutCopyMode)0; //application.CutCopyMode = false;
                }
                for (nColumn = 0; nColumn <= columnCount; nColumn++)
                {
                    strVal = gExternComObject.getCellDataValue(strCell, nColumn, nRow).GetAwaiter().GetResult();
                    sheet1.Cells[y + nRow, x + nColumn] = strVal;
                }
            }

            range = (Excel.Range)sheet1.Cells[1, 1];    // 첫줄에 커서를 위치
            range.Select();

            return rowCount;

        }


    }

}
