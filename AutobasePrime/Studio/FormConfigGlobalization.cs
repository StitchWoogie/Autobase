using AutoLibLocal;
using NetTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace Studio
{
    public partial class FormConfigGlobalization : Form
    {
        private DataTable _dataTable;
        private List<string> _languageColumns;
        private const string COL_NAME = "name";
        private const string COL_VALUE = "value";
        private bool _isModified = false;
        private string _originalTitle;
        private bool _initialized = false;
        public FormConfigGlobalization()
        {
            InitializeComponent();
            InitializeDataTable();
            LoadLanguageData();

            // 원본 제목 저장
            _originalTitle = this.Text;
            _initialized = true;
        }

        /// <summary>
        /// DataTable 초기화
        /// </summary>
        private void InitializeDataTable()
        {
            _dataTable = new DataTable();
            _languageColumns = new List<string>();

            // 기본 컬럼
            _dataTable.Columns.Add(COL_NAME, typeof(string));
            _dataTable.Columns.Add(COL_VALUE, typeof(string));

            // 기본 언어 컬럼
            string[] defaultLanguages = {"en", "ko" };
            foreach (string lang in defaultLanguages)
            {
                AddLanguageColumn(lang);
            }

            dgvLanguageTable.DataSource = _dataTable;

            //  선택 색상 변경 (연한 파란색)
            dgvLanguageTable.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
            dgvLanguageTable.DefaultCellStyle.SelectionForeColor = Color.Black;

            // ✅ 선택된 행 헤더 색상
            dgvLanguageTable.RowHeadersDefaultCellStyle.SelectionBackColor = Color.CornflowerBlue;
            dgvLanguageTable.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // ✅ 선택된 열 헤더 색상
            dgvLanguageTable.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.CornflowerBlue;
            dgvLanguageTable.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // name 컬럼은 읽기 전용으로 설정 (키 역할)
            dgvLanguageTable.Columns[COL_NAME].ReadOnly = false;
            dgvLanguageTable.Columns[COL_NAME].DefaultCellStyle.BackColor = Color.LightGray;

            // 컬럼 너비 설정
            dgvLanguageTable.Columns[COL_NAME].FillWeight = 80;
            dgvLanguageTable.Columns[COL_VALUE].FillWeight = 100;
        }

        /// <summary>
        /// 언어 컬럼 추가
        /// </summary>
        /// <param name="langCode">언어 코드</param>
        private void AddLanguageColumn(string langCode)
        {
            if (!_languageColumns.Contains(langCode))
            {
                _dataTable.Columns.Add(langCode, typeof(string));
                _languageColumns.Add(langCode);

                // 컬럼 너비 설정
                if (dgvLanguageTable.Columns[langCode] != null)
                {
                    dgvLanguageTable.Columns[langCode].FillWeight = 100;
                }
            }
        }

        private string Msg(string ko, string en)
        {
            return Tools.IsLangKorean() ? ko : en;
        }

        /// <summary>
        /// 언어 데이터 로드
        /// </summary>
        private void LoadLanguageData()
        {
            try
            {
                string dataFilePath = Path.Combine(TotalConfig.sDirWorkProject, "Language", "LanguageData.csv");

                if (!File.Exists(dataFilePath))
                {
                    MessageBox.Show( Msg("언어 데이터 파일이 없습니다.", "Language data file not found."), Msg("정보", "Information"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _dataTable.Rows.Clear();

                string[] lines = File.ReadAllLines(dataFilePath, Encoding.UTF8);

                if (lines.Length < 2) // 헤더 + 최소 1개 데이터 행
                {
                    MessageBox.Show( Msg("언어 데이터 파일이 비어있습니다.", "Wrong format") , Msg("정보", "Information"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 헤더 파싱
                string headerLine = lines[0];
                string[] headers = ParseCSVLine(headerLine);

                if (headers.Length< 2 || headers[0] != COL_NAME || headers[1] != COL_VALUE)
                {
                    MessageBox.Show(Msg("언어 데이터 파일 형식이 올바르지 않습니다.", "Wrong format"), Msg("오류","Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 언어 컬럼 추가 (헤더의 3번째부터는 언어 컬럼)
                for (int i = 2; i < headers.Length; i++)
                {
                    string langName = headers[i].Trim();
                    if (!string.IsNullOrWhiteSpace(langName) && !_languageColumns.Contains(langName))
                    {
                        AddLanguageColumn(langName);
                    }
                }

                // 데이터 행 파싱
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] values = ParseCSVLine(line);

                    if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0]))
                        continue;

                    DataRow row = _dataTable.NewRow();

                    // 각 컬럼에 값 설정
                    for (int j = 0; j < Math.Min(values.Length, headers.Length); j++)
                    {
                        string columnName = headers[j];
                        if (_dataTable.Columns.Contains(columnName))
                        {
                            row[columnName] = values[j];
                        }
                    }

                    _dataTable.Rows.Add(row);
                }

                // 로드 후 변경사항 플래그 초기화
                if (_initialized) SetModified(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Msg($"언어 데이터 로드 실패: {ex.Message}", $"Failed to load data: {ex.Message}"), Msg("오류","Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 언어 데이터 저장
        /// </summary>
        private void SaveLanguageData()
        {
            try
            {
                // 중복 키 검사
                if (!ValidateUniqueKeys())
                {
                    return;
                }

                string dataFilePath = Path.Combine(TotalConfig.sDirWorkProject, "Language", "LanguageData.csv");
                string directory = Path.GetDirectoryName(dataFilePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // UTF-8 BOM으로 저장 (엑셀 호환)
                using (StreamWriter writer = new StreamWriter(dataFilePath, false, new UTF8Encoding(true)))
                {
                    // 헤더 작성
                    List<string> columnNames = new List<string>();
                    columnNames.Add(COL_NAME);
                    columnNames.Add(COL_VALUE);
                    columnNames.AddRange(_languageColumns);

                    writer.WriteLine(string.Join(",", columnNames.Select(c => EscapeCSV(c))));

                    // 데이터 행 작성
                    foreach (DataRow row in _dataTable.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted ||
                            string.IsNullOrWhiteSpace(row[COL_NAME]?.ToString()))
                            continue;

                        List<string> values = new List<string>();

                        foreach (string colName in columnNames)
                        {
                            string value = row[colName]?.ToString() ?? string.Empty;
                            values.Add(EscapeCSV(value));
                        }

                        writer.WriteLine(string.Join(",", values));
                    }
                }

                // LanguageManager 새로고침
                LanguageManager.Instance.Refresh();

                MessageBox.Show(Msg("언어 데이터가 저장되었습니다.","Save complete"), Msg("정보", "Information"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 저장 후 변경사항 플래그 초기화
                SetModified(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Msg($"언어 데이터 저장 실패: {ex.Message}", $"Failed to save data: {ex.Message}"), Msg("오류", "Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 중복 키 검증
        /// </summary>
        /// <returns>유효하면 true, 중복이 있으면 false</returns>
        private bool ValidateUniqueKeys()
        {
            bool ko = Tools.IsLangKorean();

            HashSet<string> keySet = new HashSet<string>();
            List<string> duplicateKeys = new List<string>();

            foreach (DataRow row in _dataTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                string key = row[COL_NAME]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (keySet.Contains(key))
                {
                    if (!duplicateKeys.Contains(key))
                    {
                        duplicateKeys.Add(key);
                    }
                }
                else
                {
                    keySet.Add(key);
                }
            }

            if (duplicateKeys.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ko? "다음 키가 중복되었습니다:": "The following key is duplicated:");
                sb.AppendLine();
                foreach (string key in duplicateKeys)
                {
                    sb.AppendLine($"  - {key}");
                }
                sb.AppendLine();
                sb.AppendLine(ko ? "중복된 키를 수정한 후 다시 저장하세요.": "Please correct the duplicate key and save again.");

                MessageBox.Show(sb.ToString(), Msg("중복 키 오류", "Duplicate ket error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 변경사항 플래그 설정
        /// </summary>
        /// <param name="modified">변경 여부</param>
        private void SetModified(bool modified)
        {
            _isModified = modified;

            if (_isModified)
            {
                if (!this.Text.EndsWith(" *"))
                {
                    this.Text = _originalTitle + " *";
                }
            }
            else
            {
                this.Text = _originalTitle;
            }
        }

        /// <summary>
        /// CSV 가져오기
        /// </summary>
        private void ImportFromCSV()
        {
            try
            {
                bool ko = Tools.IsLangKorean();

                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = ko? "CSV 파일|*.csv|모든 파일|*.*": "CSV files|*.csv|All files|*.*";
                    ofd.Title = ko ?  "CSV 파일 선택": "Select the CSV file";

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;

                    string[] lines = File.ReadAllLines(ofd.FileName, Encoding.UTF8);

                    if (lines.Length == 0)
                    {
                        MessageBox.Show(Msg("파일이 비어있습니다.", "The file is empty."), Msg("오류","Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 헤더 파싱
                    string[] headers = ParseCSVLine(lines[0]);

                    if (headers.Length < 2 || headers[0] != COL_NAME)
                    {
                        MessageBox.Show(
                            Msg("CSV 형식이 올바르지 않습니다.\n첫 번째 열은 'name'이어야 합니다.", "The CSV format is incorrect.\r\nThe first column must be 'name'.") , 
                            Msg("오류", "Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 기존 데이터 초기화
                    _dataTable.Rows.Clear();

                    // 새로운 언어 컬럼 추가
                    for (int i = 2; i < headers.Length; i++)
                    {
                        if (!_dataTable.Columns.Contains(headers[i]))
                        {
                            AddLanguageColumn(headers[i]);
                        }
                    }

                    // 데이터 행 파싱
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        string[] values = ParseCSVLine(lines[i]);

                        if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0]))
                            continue;

                        DataRow row = _dataTable.NewRow();

                        for (int j = 0; j < Math.Min(values.Length, headers.Length); j++)
                        {
                            if (_dataTable.Columns.Contains(headers[j]))
                            {
                                row[headers[j]] = values[j];
                            }
                        }

                        _dataTable.Rows.Add(row);
                    }

                    //MessageBox.Show($"총 {_dataTable.Rows.Count}개의 항목을 가져왔습니다.", "가져오기 완료",
                    //    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 가져오기 후 변경사항 표시
                    SetModified(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Msg($"CSV 가져오기 실패: {ex.Message}", $"Failed to import CSV file: {ex.Message}"), Msg("오류", "Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// CSV 내보내기
        /// </summary>
        private void ExportToCSV()
        {
            try
            {
                bool ko = Tools.IsLangKorean();

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = ko ? "CSV 파일|*.csv|모든 파일|*.*" : "CSV files|*.csv|All files|*.*";
                    sfd.Title = ko ? "CSV 파일 저장" : "Save CSV file";

                    sfd.FileName = $"LanguageData_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    using (StreamWriter writer = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // 헤더 작성
                        List<string> columnNames = new List<string>();
                        columnNames.Add(COL_NAME);
                        columnNames.Add(COL_VALUE);
                        columnNames.AddRange(_languageColumns);

                        writer.WriteLine(string.Join(",", columnNames.Select(c => EscapeCSV(c))));

                        // 데이터 행 작성
                        foreach (DataRow row in _dataTable.Rows)
                        {
                            if (row.RowState == DataRowState.Deleted ||
                                string.IsNullOrWhiteSpace(row[COL_NAME]?.ToString()))
                                continue;

                            List<string> values = new List<string>();

                            foreach (string colName in columnNames)
                            {
                                string value = row[colName]?.ToString() ?? string.Empty;
                                values.Add(EscapeCSV(value));
                            }

                            writer.WriteLine(string.Join(",", values));
                        }
                    }

                    MessageBox.Show(Msg("CSV 파일로 내보내기가 완료되었습니다.", "Export to CSV file has been completed")
                        , Msg("내보내기 완료", "Export complete"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Msg($"CSV 내보내기 실패: {ex.Message}", $"Failed to export CSV file: {ex.Message}"), 
                    Msg("오류","Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// CSV 라인 파싱 (따옴표 처리 포함)
        /// </summary>
        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            StringBuilder currentValue = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // 이스케이프된 따옴표 ("")
                        currentValue.Append('"');
                        i++; // 다음 따옴표 건너뛰기
                    }
                    else
                    {
                        // 따옴표 시작 또는 종료
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // 필드 구분자
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            // 마지막 필드 추가
            result.Add(currentValue.ToString());

            return result.ToArray();
        }

        /// <summary>
        /// CSV 값 이스케이프 처리
        /// </summary>
        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // 쉼표, 큰따옴표, 개행 문자가 있으면 큰따옴표로 감싸기
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                // 큰따옴표는 두 번 써서 이스케이프
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        // 이벤트 핸들러들
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            bool ko = Tools.IsLangKorean();

            DataRow newRow = _dataTable.NewRow();
            newRow[COL_NAME] = "new.key";
            newRow[COL_VALUE] = ko ? "새 항목": "New item";
            _dataTable.Rows.Add(newRow);
            SetModified(true);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvLanguageTable.SelectedRows.Count == 0)
            {
                MessageBox.Show(Msg("삭제할 행을 선택하세요.", "Please select the row to delete"), 
                    Msg("알림", "Notice"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(
                Msg($"선택한 {dgvLanguageTable.SelectedRows.Count}개의 행을 삭제하시겠습니까?", 
                $"Are you sure you want to delete the { dgvLanguageTable.SelectedRows.Count} selected row(s)?"),
                Msg("삭제 확인", "Confirm deletion"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvLanguageTable.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        dgvLanguageTable.Rows.Remove(row);
                    }
                }
                SetModified(true);
            }
        }

        private void BtnAddColumn_Click(object sender, EventArgs e)
        {
            bool ko = Tools.IsLangKorean();
            using (FormInputDialog dialog = new FormInputDialog(ko ? "언어 코드 입력": "Enter language code", ko? "언어 코드:": "Language code:"))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string langCode = dialog.InputValue.Trim();

                    if (string.IsNullOrWhiteSpace(langCode))
                    {
                        MessageBox.Show(
                            Msg("언어 코드를 입력하세요.", "Please enter the language code.") ,
                            Msg("오류", "Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (_languageColumns.Contains(langCode))
                    {
                        MessageBox.Show(
                            Msg("이미 존재하는 언어 코드입니다.", "This language code already exists."),
                            Msg("오류", "Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    AddLanguageColumn(langCode);
                    MessageBox.Show(
                        Msg($"언어 '{langCode}'가 추가되었습니다.", $"The language '{langCode}' has been added"),
                        Msg("완료", "Add complete"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetModified(true);
                }
            }
        }

        private void BtnDeleteColumn_Click(object sender, EventArgs e)
        {
            if (_languageColumns.Count == 0)
            {
                MessageBox.Show(
                    Msg("삭제할 언어가 없습니다.", "There is no language to delete.") , Msg("알림", "Notice"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FormSelectLanguage dialog = new FormSelectLanguage(_languageColumns))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedLang = dialog.SelectedLanguage;

                    if (MessageBox.Show(
                        Msg($"언어 '{selectedLang}' 컬럼을 삭제하시겠습니까?", $"Are you sure you want to delete the '{selectedLang}' column?"),
                        Msg("삭제 확인","Confirm deletion"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _dataTable.Columns.Remove(selectedLang);
                        _languageColumns.Remove(selectedLang);

                        MessageBox.Show(
                            Msg($"언어 '{selectedLang}'가 삭제되었습니다." , $"The language '{selectedLang}' has been deleted."),
                            Msg("완료","Completed"),
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SetModified(true);
                    }
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            ImportFromCSV();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveLanguageData();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (_isModified)
            {
                DialogResult result = MessageBox.Show(
                    Msg("저장하지 않은 변경사항이 있습니다.\n계속하시겠습니까?", "There are unsaved changes.\nDo you want to continue?"),
                    Msg( "새로고침","Reload"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            LoadLanguageData();
        }

        private void FormConfigGlobalization_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isModified)
            {
                DialogResult result = MessageBox.Show(
                    Msg("저장하지 않은 변경사항이 있습니다.\n저장하시겠습니까?", "You have unsaved changes.\nDo you want to save them?"),
                    Msg("확인","Confirm"),
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // 저장
                    SaveLanguageData();

                    // 저장 실패 시 (중복 키 등) 폼 닫기 취소
                    if (_isModified)
                    {
                        e.Cancel = true;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    // 취소 - 폼 닫기 취소
                    e.Cancel = true;
                }
                // No 선택 시 그냥 닫기
            }
        }

        private void dgvLanguageTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SetModified(true);
        }

        private void dgvLanguageTable_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            SetModified(true);
        }

        private void dgvLanguageTable_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //if(_initialized)
            //    SetModified(true);
        }

        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportFromCSV();
        }

        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLanguageData();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// 입력 다이얼로그
    /// </summary>
    internal class FormInputDialog : Form
    {
        private TextBox txtInput;
        private Button btnOK;
        private Button btnCancel;

        public string InputValue
        {
            get { return txtInput.Text; }
        }

        public FormInputDialog(string title, string label)
        {
            bool ko = Tools.IsLangKorean();

            this.Text = ko ? title : "Enter Language Code";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblPrompt = new Label();
            lblPrompt.Text = ko ? label : "Language code:";
            lblPrompt.Location = new Point(20, 20);
            lblPrompt.Size = new Size(350, 20);

            txtInput = new TextBox();
            txtInput.Location = new Point(20, 45);
            txtInput.Size = new Size(350, 25);

            // ✅ 기본 지원 언어 안내 라벨 추가
            Label lblDefaultLanguages = new Label();
            lblDefaultLanguages.Text = ko
                ? "기본 지원 언어: en, ko, ja, zh-CN, vt"
                : "Supported default languages: en, ko, ja, zh-CN, vt";
            lblDefaultLanguages.Location = new Point(20, 75);
            lblDefaultLanguages.Size = new Size(350, 20);
            lblDefaultLanguages.ForeColor = Color.Black;

            btnOK = new Button();
            btnOK.Text = ko ? "확인" : "OK";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(200, 110); // ✅ 위치 조정
            btnOK.Size = new Size(80, 30);

            btnCancel = new Button();
            btnCancel.Text = ko ? "취소" : "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(290, 110); // ✅ 위치 조정
            btnCancel.Size = new Size(80, 30);

            this.Controls.AddRange(new Control[] { lblPrompt, txtInput, lblDefaultLanguages, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }

    /// <summary>
    /// 언어 선택 다이얼로그
    /// </summary>
    internal class FormSelectLanguage : Form
    {
        private ComboBox cmbLanguages;
        private Button btnOK;
        private Button btnCancel;

        public string SelectedLanguage
        {
            get { return cmbLanguages.SelectedItem?.ToString() ?? string.Empty; }
        }

        public FormSelectLanguage(List<string> languages)
        {
            bool ko = Tools.IsLangKorean();

            this.Text = ko ? "언어 선택" : "Select Language";
            this.Size = new Size(350, 160);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblPrompt = new Label();
            lblPrompt.Text = ko
                ? "삭제할 언어를 선택하세요:"
                : "Select the language to delete:";

            lblPrompt.Location = new Point(20, 20);
            lblPrompt.Size = new Size(300, 20);

            cmbLanguages = new ComboBox();
            cmbLanguages.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguages.Location = new Point(20, 45);
            cmbLanguages.Size = new Size(300, 25);
            cmbLanguages.DataSource = languages;

            btnOK = new Button();
            btnOK.Text = ko ? "확인" : "OK";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(150, 80);
            btnOK.Size = new Size(80, 30);

            btnCancel = new Button();
            btnCancel.Text = ko ? "취소" : "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(240, 80);
            btnCancel.Size = new Size(80, 30);

            this.Controls.AddRange(new Control[] { lblPrompt, cmbLanguages, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    
}
}
