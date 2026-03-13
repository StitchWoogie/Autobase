using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Reporting.Designer.Controls;
using Reporting.Engine.IO;
using Reporting.Engine.Model;

namespace Reporting.Designer
{
    /// <summary>
    /// 리포트 디자이너 메인 편집기 폼 (Studio MDI 자식)
    /// </summary>
    public partial class FormReportDesigner : Form
    {
        private WorkbookModel _workbook;
        private string _filePath;
        private bool _isDirty;

        // Studio 콜백 (MDI 연동)
        public delegate void CallBackOnMdiActivated(Form form, string filename, object obj);
        public delegate void CallBackOnPopupView();
        private CallBackOnMdiActivated _cbMdiActivated;
        private CallBackOnPopupView _cbPopupView;

        public FormReportDesigner()
        {
            InitializeComponent();
            SetupEvents();
            NewWorkbook();
        }

        public FormReportDesigner(string filePath, StatusBar statusBar,
            CallBackOnMdiActivated cbMdi, CallBackOnPopupView cbPopup)
        {
            InitializeComponent();
            _cbMdiActivated = cbMdi;
            _cbPopupView = cbPopup;
            SetupEvents();

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                OpenFile(filePath);
            }
            else
            {
                _filePath = filePath;
                NewWorkbook();
            }
        }

        private void SetupEvents()
        {
            _spreadsheet.SelectionChanged += Spreadsheet_SelectionChanged;
            _spreadsheet.CellValueChanged += Spreadsheet_CellValueChanged;
            _spreadsheet.SheetModified += Spreadsheet_SheetModified;
            _formulaBar.FormulaCommitted += FormulaBar_FormulaCommitted;
            _formulaBar.AddressNavigated += FormulaBar_AddressNavigated;
            _sheetTab.ActiveSheetChanged += SheetTab_ActiveSheetChanged;
        }

        #region 워크북 관리

        private void NewWorkbook()
        {
            _workbook = WorkbookModel.CreateDefault();
            _isDirty = false;
            BindToSheet();
            UpdateTitle();
        }

        private void BindToSheet()
        {
            if (_workbook == null) return;

            var sheet = _workbook.ActiveSheet;
            _spreadsheet.Sheet = sheet;

            var names = _workbook.Sheets.Select(s => s.Name).ToList();
            _sheetTab.SetSheets(names, _workbook.ActiveSheetIndex);

            UpdateFormulaBar();
        }

        private void UpdateFormulaBar()
        {
            _formulaBar.UpdateAddress(_spreadsheet.CurrentCellAddress);
            _formulaBar.UpdateFormula(_spreadsheet.CurrentCellText);
        }

        private void UpdateTitle()
        {
            string name = string.IsNullOrEmpty(_filePath)
                ? "새 리포트"
                : Path.GetFileName(_filePath);
            Text = _isDirty ? name + " *" : name;
        }

        #endregion

        #region 파일 I/O

        public void OpenFile(string filePath)
        {
            try
            {
                _workbook = XlsxReader.Read(filePath);
                _filePath = filePath;
                _isDirty = false;
                BindToSheet();
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일 열기 실패: " + ex.Message, "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveFile()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                SaveFileAs();
                return;
            }
            SaveToFile(_filePath);
        }

        public void SaveFileAs()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Excel 파일 (*.xlsx)|*.xlsx|CSV 파일 (*.csv)|*.csv|모든 파일 (*.*)|*.*";
                dlg.DefaultExt = "xlsx";
                if (!string.IsNullOrEmpty(_filePath))
                    dlg.FileName = Path.GetFileName(_filePath);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(dlg.FileName).ToLower();
                    if (ext == ".csv")
                    {
                        CsvWriter.Write(_workbook, dlg.FileName);
                    }
                    else
                    {
                        SaveToFile(dlg.FileName);
                    }
                    _filePath = dlg.FileName;
                    UpdateTitle();
                }
            }
        }

        private void SaveToFile(string filePath)
        {
            try
            {
                _spreadsheet.CommitEdit();
                XlsxWriter.Write(_workbook, filePath);
                _isDirty = false;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 실패: " + ex.Message, "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 이벤트 핸들러

        private void Spreadsheet_SelectionChanged(object sender, EventArgs e)
        {
            UpdateFormulaBar();
        }

        private void Spreadsheet_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            _isDirty = true;
            UpdateTitle();
            UpdateFormulaBar();
        }

        private void Spreadsheet_SheetModified(object sender, EventArgs e)
        {
            _isDirty = true;
            UpdateTitle();
        }

        private void FormulaBar_FormulaCommitted(object sender, string text)
        {
            _spreadsheet.SetCurrentCellText(text);
            _spreadsheet.Focus();
            _isDirty = true;
            UpdateTitle();
        }

        private void FormulaBar_AddressNavigated(object sender, string address)
        {
            _spreadsheet.GoToCell(address);
            _spreadsheet.Focus();
        }

        private void SheetTab_ActiveSheetChanged(object sender, int index)
        {
            if (_workbook == null || index < 0 || index >= _workbook.Sheets.Count)
                return;

            _spreadsheet.CommitEdit();
            _workbook.ActiveSheetIndex = index;
            _spreadsheet.Sheet = _workbook.ActiveSheet;
            UpdateFormulaBar();
        }

        #endregion

        #region 메뉴/툴바 핸들러

        private void MenuFileNew_Click(object sender, EventArgs e)
        {
            if (!ConfirmSave()) return;
            _filePath = null;
            NewWorkbook();
        }

        private void MenuFileOpen_Click(object sender, EventArgs e)
        {
            if (!ConfirmSave()) return;

            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Excel 파일 (*.xlsx)|*.xlsx|모든 파일 (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(dlg.FileName);
                }
            }
        }

        private void MenuFileSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void MenuFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private bool ConfirmSave()
        {
            if (!_isDirty) return true;

            var result = MessageBox.Show("변경 사항을 저장하시겠습니까?", "저장 확인",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SaveFile();
                return true;
            }
            return result == DialogResult.No;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!ConfirmSave())
                e.Cancel = true;
            base.OnFormClosing(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _cbMdiActivated?.Invoke(this, _filePath, null);
        }

        #endregion
    }
}
