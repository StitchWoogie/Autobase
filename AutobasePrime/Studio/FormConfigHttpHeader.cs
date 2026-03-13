using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NetTools;
using AutoLibLocal;
using System.Security.Cryptography;
using GraphicModule;

namespace Studio
{
    public partial class FormConfigHttpHeader : Form
    {

        private class HeaderItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public bool IsEncrypted { get; set; }
        }

        private List<HeaderItem> headerItems = new List<HeaderItem>();
        private bool _isLangKorean = false;
        private int modifiedFlag = 0;
        private string originalTitle;

        private string colHeaderName;
        private string colHeaderValue;
        private string colEncrypted;


        public FormConfigHttpHeader()
        {
            InitializeComponent();
            _isLangKorean = Tools.IsLangKorean();
            originalTitle = this.Text;

            listView.View = View.Details;
            listView.FullRowSelect = true;  // 전체 행 선택 가능
            listView.MultiSelect = false;   // 단일 선택만 허용

            colHeaderName = _isLangKorean ? "헤더 이름" : "Header Name";
            colHeaderValue = _isLangKorean ? "헤더 값" : "Header Value";
            colEncrypted = _isLangKorean ? "암호화" : "Encrypted";

            if (_isLangKorean)
            {
                btnAdd.Text = "추가";
                btnDelete.Text = "선택 삭제";
                btnSave.Text = "저장";
                btnLoad.Text = "불러오기";
                btnWarnigs.Text = "주의사항";

                lbApiName.Text = "헤더설정 파일명";
                lbName.Text = "헤더 이름";
                lbValue.Text = "헤더 값";
                chkEncrypt.Text = "암호화 사용";
            }

            listView.Columns.Add(colHeaderName, 150);
            listView.Columns.Add(colHeaderValue, 250);
            listView.Columns.Add(colEncrypted, 70);

            // 기본 헤더 이름 추가
            cmbHeaderName.Items.AddRange(new string[] {
            "Authorization",
            "Content-Type",
            "Accept",
            "User-Agent",
            "Accept-Language",
            "Accept-Encoding",
            "Cache-Control",
            "Origin",
            "X-Requested-With"
        });

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbHeaderName.Text))
            {
                MessageBox.Show(
                    _isLangKorean ? "헤더이름을 입력하세요." : "Please enter a header name.",
                    _isLangKorean ? "알림" : "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 중복 체크
            if (headerItems.Any(x => x.Name == cmbHeaderName.Text))
            {
                MessageBox.Show(
                    _isLangKorean ? "이미 존재하는 헤더이름입니다." : "Header name already exists.",
                    _isLangKorean ? "오류" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string value = txtValue.Text;
            if (chkEncrypt.Checked)
            {
                value = GraphicModule.ScriptFunctionHttp.EncryptAES(value);
            }

            var item = new HeaderItem
            {
                Name = cmbHeaderName.Text,
                Value = value,
                IsEncrypted = chkEncrypt.Checked
            };
            headerItems.Add(item);
            UpdateListView();
            SetModified();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtApiName.Text))
            {
                MessageBox.Show(
                    _isLangKorean ? "헤더 설정 파일명을 입력하세요." : "Please enter Header Config Filename.",
                    _isLangKorean ? "알림" : "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                _isLangKorean ? "저장하시겠습니까?" : "Do you want to save?",
                _isLangKorean ? "저장 확인" : "Save Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string dirPath = Path.Combine(TotalConfig.sDirWorkProject, "API");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                string filePath = Path.Combine(dirPath, txtApiName.Text + ".header");
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (var item in headerItems)
                    {
                        sw.WriteLine(String.Format("{0},{1},{2}", item.Name, item.Value, (item.IsEncrypted ? "1" : "0")));
                    }
                }
                modifiedFlag = 0;
                this.Text = originalTitle + " - " + filePath;
            }
        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                int index = listView.SelectedItems[0].Index;
                headerItems.RemoveAt(index);
                UpdateListView();
                SetModified();
            }
        }

        private void SetModified()
        {
            if (modifiedFlag == 0)
            {
                modifiedFlag = 1;
                this.Text = this.Text + " *";
            }
        }

        private void UpdateListView()
        {
            listView.Items.Clear();
            foreach (var item in headerItems)
            {
                var lvi = new ListViewItem(new string[] 
                { 
                    item.Name, 
                    item.Value, 
                    item.IsEncrypted ? "Y" : "N" 
                });
                listView.Items.Add(lvi);
            }
        }


        private void FormConfigHttpHeader_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (modifiedFlag == 1)
            {
                var result = MessageBox.Show(
                    _isLangKorean ? "변경된 내용을 저장하시겠습니까?" : "Do you want to save the changes?",
                    _isLangKorean ? "저장 확인" : "Save Confirmation",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Path.Combine(TotalConfig.sDirWorkProject, "API");
                openFileDialog.Filter = "Header files (*.header)|*.header|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!openFileDialog.FileName.EndsWith(".header", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show(
                            _isLangKorean ? "헤더파일이 아닙니다." : "Not a header file.",
                            _isLangKorean ? "오류" : "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    txtApiName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    LoadHeaders(openFileDialog.FileName);
                    this.Text = originalTitle + " - " + openFileDialog.FileName;
                    modifiedFlag = 0;
                }
            }
        }


        private void LoadHeaders(string filePath)
        {
            try
            {
                headerItems.Clear();
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new char[] { ',' }, 3);
                        if (parts.Length >= 2)
                        {
                            bool isEncrypted = parts.Length > 2 && parts[2] == "1";
                            headerItems.Add(new HeaderItem
                            {
                                Name = parts[0],
                                Value = parts[1],
                                IsEncrypted = isEncrypted
                            });
                        }
                    }
                }
                UpdateListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    _isLangKorean ? "파일을 읽는 중 오류가 발생했습니다.\n" + ex.Message :
                                   "Error while reading file.\n" + ex.Message,
                    _isLangKorean ? "오류" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                int index = listView.SelectedItems[0].Index;
                HeaderItem item = headerItems[index];

                cmbHeaderName.Text = item.Name;
                txtValue.Text = item.Value;
            }
        }

        private void btnWarnigs_Click(object sender, EventArgs e)
        {
            using (Form warningForm = new Form())
            {
                warningForm.Text = _isLangKorean ? "주의사항" : "Warnings";
                warningForm.Size = new Size(500, 200);
                warningForm.StartPosition = FormStartPosition.CenterParent;
                warningForm.MinimizeBox = false;
                warningForm.MaximizeBox = false;
                warningForm.FormBorderStyle = FormBorderStyle.FixedDialog;

                Label lblWarnings = new Label();
                lblWarnings.Text = _isLangKorean ?
                    "1. 암호화된 값은 복호화할 수 없습니다.\n   원본 값을 반드시 별도로 보관하세요.\n\n" +
                    "2. 스크립트에서 @HttpLoadHeaderConfig 함수로 헤더 설정을 불러오면\n   설정한 모든 헤더가 적용됩니다.  사용하지 않는 헤더는 삭제하세요." :
                    "1. Encrypted values cannot be decrypted.\n   Make sure to keep original values separately.\n\n" +
                    "2. If you use the @HttpLoadHeaderConfig function in your script\n   to load the header settings, all the headers you set will be applied.\n   Remove any unused headers.";

                lblWarnings.Location = new Point(20, 20);
                lblWarnings.AutoSize = true;

                warningForm.Controls.Add(lblWarnings);
                warningForm.ShowDialog();
            }
        }


    }
}


