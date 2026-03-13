using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AutobaseRESTAPIMonitor.FormRestApiMonitor;

namespace AutobaseRESTAPIMonitor
{
    public partial class FormRequestHeader : Form
    {
        private List<(string Name, string Value, bool Encrypt)> headers = new List<(string Name, string Value, bool Encrypt)>();
        public List<(string Name, string Value, bool Encrypt)> GetHeaders()
        {
            headers.Clear();

            foreach (DataGridViewRow row in dgvHeaders.Rows)
            {
                if (row.Cells["HeaderName"].Value != null && row.Cells["StoredValue"].Value != null)
                {
                    string name = row.Cells["HeaderName"].Value.ToString();
                    string storedValue = row.Cells["StoredValue"].Value.ToString(); // 암호화된 값
                    bool encrypt = row.Cells["Encrypt"].Value != null && Convert.ToBoolean(row.Cells["Encrypt"].Value);

                    headers.Add((name, storedValue, encrypt)); // 암호화된 값을 추가
                }
            }

            return headers;
        }

        private void LoadHeadersToGrid()
        {
            dgvHeaders.Rows.Clear(); // 기존 행 초기화

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    dgvHeaders.Rows.Add(
                        header.Name,
                        header.Encrypt ? "********" : header.Value, // 암호화 시 마스킹된 값
                        header.Value, // StoredValue: 암호화된 실제 값 저장
                        header.Encrypt // 암호화 여부
                    );
                }
            }
        }

        public FormRequestHeader(List<(string Name, string Value, bool Encrypt)> existingHeaders = null)
        {
            InitializeComponent();
            InitializeRequestHeaderForm();
            headers = existingHeaders ?? new List<(string Name, string Value, bool Encrypt)>();
            LoadHeadersToGrid();
        }

        private void InitializeRequestHeaderForm()
        {
            // DataGridView 설정
            dgvHeaders.Columns.Add("HeaderName", "Header Name");
            dgvHeaders.Columns.Add("HeaderValue", "Header Value");
            dgvHeaders.Columns.Add("StoredValue", "Stored Value"); // 내부 저장값 컬럼

            // StoredValue는 사용자에게 보이지 않도록 설정
            dgvHeaders.Columns["StoredValue"].Visible = false;

            // Encrypt 컬럼 추가
            var encryptColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Encrypt",
                HeaderText = "Encrypt",
                Width = 50,
                ReadOnly = true 
            };
            dgvHeaders.Columns.Add(encryptColumn);

            dgvHeaders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Header Name ComboBox 설정
            cboHeaderName.Items.AddRange(new string[] { "Authorization", "Content-Type", "Accept", "User-Agent" });
            cboHeaderName.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string headerName = cboHeaderName.Text;
                string headerValue = txtHeaderValue.Text;
                bool encrypt = chkEncrypt.Checked;

                if (string.IsNullOrWhiteSpace(headerName) || string.IsNullOrWhiteSpace(headerValue))
                {
                    MessageBox.Show(GlobalSettings.IsKorean ? "헤더 이름과 헤더 값을 작성해주세요" : "Header Name and Value cannot be empty.");
                    return;
                }

                // 암호화 처리
                string storedValue = encrypt ? EncryptionHelper.Encrypt(headerValue) : headerValue;

                // DataGridView에 추가 또는 수정
                bool exists = false;
                foreach (DataGridViewRow row in dgvHeaders.Rows)
                {
                    if (row.Cells["HeaderName"].Value != null && row.Cells["HeaderName"].Value.ToString() == headerName)
                    {
                        row.Cells["HeaderValue"].Value = headerValue; // 평문 표시
                        row.Cells["StoredValue"].Value = storedValue;
                        row.Cells["Encrypt"].Value = encrypt;
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    dgvHeaders.Rows.Add(headerName, headerValue, storedValue, encrypt);
                }

                // 입력 필드 초기화
                cboHeaderName.Text = "";
                txtHeaderValue.Text = "";
                chkEncrypt.Checked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRequestHeader] btnAddOrUpdate_Click error: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvHeaders.SelectedRows)
                {
                    dgvHeaders.Rows.Remove(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRequestHeader] btnDelete_Click error: {ex.Message}");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                headers.Clear();

                foreach (DataGridViewRow row in dgvHeaders.Rows)
                {
                    if (row.Cells["HeaderName"].Value != null && row.Cells["StoredValue"].Value != null)
                    {
                        string name = row.Cells["HeaderName"].Value.ToString();
                        string storedValue = row.Cells["StoredValue"].Value.ToString();
                        bool encrypt = row.Cells["Encrypt"].Value != null && Convert.ToBoolean(row.Cells["Encrypt"].Value);

                        headers.Add((name, storedValue, encrypt));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRequestHeader] btnOK_Click error: {ex.Message}");
            }
        }

        private void dgvHeaders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dgvHeaders.Rows.Count)
                {
                    DataGridViewRow selectedRow = dgvHeaders.Rows[e.RowIndex];

                    // 선택된 행의 데이터를 ComboBox와 TextBox에 설정
                    if (selectedRow.Cells["HeaderName"].Value != null)
                    {
                        cboHeaderName.Text = selectedRow.Cells["HeaderName"].Value.ToString();
                    }
                    else
                    {
                        cboHeaderName.Text = ""; // 값이 없으면 초기화
                    }

                    if (selectedRow.Cells["HeaderValue"].Value != null)
                    {
                        txtHeaderValue.Text = selectedRow.Cells["HeaderValue"].Value.ToString();
                    }
                    else
                    {
                        txtHeaderValue.Text = ""; // 값이 없으면 초기화
                    }

                    if (selectedRow.Cells["Encrypt"].Value != null)
                    {
                        chkEncrypt.Checked = Convert.ToBoolean(selectedRow.Cells["Encrypt"].Value);
                    }
                    else
                    {
                        chkEncrypt.Checked = false; // 기본값
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[FormRequestHeader] dgvHeaders_CellClick error: {ex.Message}");
            }
        }
    }
}
