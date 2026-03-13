using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Reporting.Engine.IO;
using Reporting.Engine.Model;
using Reporting.Viewer.Controls;

namespace Reporting.Viewer
{
    /// <summary>
    /// 리포트 뷰어 폼
    /// 리포트 실행 결과(WorkbookModel)를 읽기 전용으로 표시하고 내보내기 기능을 제공한다.
    /// </summary>
    public partial class FormReportViewer : Form
    {
        private WorkbookModel _workbook;
        private string _filePath;

        public FormReportViewer()
        {
            InitializeComponent();
            SetupEvents();
        }

        /// <summary>
        /// 파일 경로로 열기
        /// </summary>
        public FormReportViewer(string filePath) : this()
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                OpenFile(filePath);
            }
        }

        /// <summary>
        /// WorkbookModel 직접 표시
        /// </summary>
        public FormReportViewer(WorkbookModel workbook, string title = null) : this()
        {
            _workbook = workbook;
            BindToSheet();
            Text = title ?? "리포트 결과";
        }

        private void SetupEvents()
        {
            _sheetView.SelectionChanged += SheetView_SelectionChanged;
            _sheetTab.ActiveSheetChanged += SheetTab_ActiveSheetChanged;
        }

        #region 데이터 바인딩

        /// <summary>
        /// 워크북 설정 (외부에서 결과 전달용)
        /// </summary>
        public void SetWorkbook(WorkbookModel workbook, string title = null)
        {
            _workbook = workbook;
            BindToSheet();
            if (title != null)
                Text = title;
        }

        private void BindToSheet()
        {
            if (_workbook == null) return;

            _sheetView.Sheet = _workbook.ActiveSheet;

            var names = _workbook.Sheets.Select(s => s.Name).ToList();
            _sheetTab.SetSheets(names, _workbook.ActiveSheetIndex);

            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            string addr = _sheetView.SelectedCellAddress;
            string val = _sheetView.SelectedCellText;
            _statusCellInfo.Text = string.IsNullOrEmpty(addr) ? "" : $"{addr}: {val}";
        }

        #endregion

        #region 파일 I/O

        public void OpenFile(string filePath)
        {
            try
            {
                _workbook = XlsxReader.Read(filePath);
                _filePath = filePath;
                BindToSheet();
                Text = Path.GetFileName(filePath) + " - 리포트 뷰어";
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일 열기 실패: " + ex.Message, "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 이벤트 핸들러

        private void SheetView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void SheetTab_ActiveSheetChanged(object sender, int index)
        {
            if (_workbook == null || index < 0 || index >= _workbook.Sheets.Count)
                return;

            _workbook.ActiveSheetIndex = index;
            _sheetView.Sheet = _workbook.ActiveSheet;
            UpdateStatusBar();
        }

        private void BtnExportXlsx_Click(object sender, EventArgs e)
        {
            if (_workbook == null) return;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel 파일 (*.xlsx)|*.xlsx";
                dlg.DefaultExt = "xlsx";
                if (!string.IsNullOrEmpty(_filePath))
                    dlg.FileName = Path.GetFileNameWithoutExtension(_filePath) + "_export.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XlsxWriter.Write(_workbook, dlg.FileName);
                        _statusLabel.Text = "XLSX 내보내기 완료: " + Path.GetFileName(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("XLSX 내보내기 실패: " + ex.Message, "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            if (_workbook == null) return;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV 파일 (*.csv)|*.csv";
                dlg.DefaultExt = "csv";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CsvWriter.Write(_workbook, dlg.FileName);
                        _statusLabel.Text = "CSV 내보내기 완료: " + Path.GetFileName(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("CSV 내보내기 실패: " + ex.Message, "오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion
    }
}
