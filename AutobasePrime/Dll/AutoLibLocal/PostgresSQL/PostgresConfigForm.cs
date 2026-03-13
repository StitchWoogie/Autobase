using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;
using NetTools;

namespace AutoLibLocal.PostgresSQL
{
    public partial class PostgresConfigForm : Form
    {
        private Label lblHost;
        private Label lblPort;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblDatabase;
        private Label lblTimezone;

        private TextBox txtHost;
        private TextBox txtPort;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtDatabase;
        private ComboBox cmbTimezone;

        private Button btnSave;
        private Button btnCancel;
        private Button btnTest;

        // PostgreSQL 설정 변수들
        private string sPostgresHost;
        private string sPostgresPort;
        private string sPostgresUsername;
        private string sPostgresPassword;
        private string sPostgresDatabase;
        private string sPostgresTimezone;
        private int sOperationalDataRetentionDays;

        // 기존 DB명 저장 (변경 감지용)
        private string sOriginalDatabase;

        public bool ConfigSaved { get; private set; }

        // 암호화 키 (실제 환경에서는 안전한 방법으로 관리해야 함)
        private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("AutoBaseDBpasswd"); // 16, 24, or 32 bytes
        private static readonly byte[] EncryptionIV = Encoding.UTF8.GetBytes("passwdDBAutoBase"); // 16 bytes

        public PostgresConfigForm()
        {
            InitializeComponent();
            if(Tools.IsLangKorean())
            {
                this.Text = "PostgreSQL 설정";
                lblHost.Text = "호스트";
                lblPort.Text = "포트";
                lblUsername.Text = "사용자명";
                lblPassword.Text = "비밀번호";
                lblPasswordConfirm.Text = "비밀번호 확인";
                lblDatabase.Text = "데이터베이스명";
                lblTimezone.Text = "타임존";
                btnSave.Text = "저장";
                btnCancel.Text = "취소";
                btnTest.Text = "연결 테스트";
            }

            SetDefaultValues();
           // LoadConfigFromFile();
            ConfigSaved = false;
        }


        /// <summary>
        /// 기존 설정값을 폼에 로드 (설정 변경 시 사용)
        /// </summary>
        /// <param name="loadExisting">true면 기존 설정 로드, false면 기본값 사용</param>
        public PostgresConfigForm(bool loadExisting) : this()
        {
            if (loadExisting)
            {
                LoadExistingConfig();
            }
            else LoadConfigFromFile();


        }

        /// <summary>
        /// ConfigDataDB의 현재 설정값을 폼에 로드
        /// </summary>
        private void LoadExistingConfig()
        {
            if (!string.IsNullOrEmpty(ConfigDataDB.sPostgresHost))
                txtHost.Text = ConfigDataDB.sPostgresHost;

            if (!string.IsNullOrEmpty(ConfigDataDB.sPostgresPort))
                txtPort.Text = ConfigDataDB.sPostgresPort;

            if (!string.IsNullOrEmpty(ConfigDataDB.sPostgresUsername))
                txtUsername.Text = ConfigDataDB.sPostgresUsername;

            if (!string.IsNullOrEmpty(ConfigDataDB.sPostgresPassword))
                txtPassword.Text = ConfigDataDB.sPostgresPassword;

            if (!string.IsNullOrEmpty(ConfigDataDB.sPostgresDatabase))
            {
                txtDatabase.Text = ConfigDataDB.sPostgresDatabase;
                sOriginalDatabase = ConfigDataDB.sPostgresDatabase;
            }

            if (!string.IsNullOrEmpty(ConfigDataDB.sPostgresTimezone))
            {
                int index = cmbTimezone.FindStringExact(ConfigDataDB.sPostgresTimezone);
                if (index >= 0)
                    cmbTimezone.SelectedIndex = index;
            }

            //if (!string.IsNullOrEmpty(ConfigDataDB.sOperationalDataRetentionDays))
            //{
            //    if (int.TryParse(ConfigDataDB.sOperationalDataRetentionDays, out int days))
            //    {
            //        numericUpDownRetetionDay.Value = days;
            //    }
            //}
        }
        private void SetDefaultValues()
        {
            // 기본값 설정
            txtHost.Text = "localhost";
            txtPort.Text = "5432";
            cmbTimezone.Text = "Asia/Seoul";
            //numericUpDownRetetionDay.Value = 90;
        }


        private void BtnTest_Click(object sender, EventArgs e)
        {

            if (!ValidateInput())
                return;

            // UI 비활성화
            lblStatus.ForeColor = Color.Blue;
            lblStatus.Text = Tools.IsLangKorean() ? "연결 테스트 중...": "Testing connection...";
            Application.DoEvents();

            string connectionString = BuildConnectionString();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // TimescaleDB 확장 확인
                    using (NpgsqlCommand cmd = new NpgsqlCommand(
                        "SELECT COUNT(*) FROM pg_extension WHERE extname = 'timescaledb'", conn))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            lblStatus.ForeColor = Color.Green;
                            lblStatus.Text = Tools.IsLangKorean() ? "✓ 연결 성공!" : "✓ Connection successful! ";
                        }
                        else
                        {
                            lblStatus.ForeColor = Color.Orange;
                            lblStatus.Text = Tools.IsLangKorean() ? "✓ 연결 성공! 단, TimescaleDB 확장이 설치되어 있지 않습니다." 
                                : "✓ Connection successful! However, TimescaleDB extension is not installed." ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = Tools.IsLangKorean() ? $"✗ 연결 실패: {ex.Message}" : $"✗ Connection failed: {ex.Message}";
            }

        }


        private void BtnSave_Click(object sender, EventArgs e)
        {

            // 입력값 검증
            if (!ValidateInput())
                return;

            // 비밀번호 일치 확인 (추가)
            if (txtPassword.Text != txtPasswordConfirm.Text)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("비밀번호가 일치하지 않습니다.\n비밀번호를 다시 확인해주세요.", "비밀번호 불일치",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Passwords do not match.\nPlease check your password again.", "Password Mismatch",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                txtPasswordConfirm.Focus();
                txtPasswordConfirm.SelectAll();
                return;
            }

            // 기존 DB명과 다를 경우 경고 (추가)
            string newDatabase = txtDatabase.Text.Trim();
            if (!string.IsNullOrEmpty(sOriginalDatabase) &&
                !sOriginalDatabase.Equals(newDatabase, StringComparison.OrdinalIgnoreCase))
            {
                DialogResult result;
                if (Tools.IsLangKorean())
                {
                     result = MessageBox.Show(
                    $"데이터베이스명이 변경되었습니다.\n\n" +
                    $"기존 DB: {sOriginalDatabase}\n" +
                    $"새로운 DB: {newDatabase}\n\n" +
                    $"데이터베이스를 변경하면 기존 데이터에 접근할 수 없게 됩니다.\n" +
                    $"계속 진행하시겠습니까?",
                    "데이터베이스 변경 경고",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                } else {
                     result = MessageBox.Show(
                    $"The database name has been changed.\n\n" +
                    $"Old DB: {sOriginalDatabase}\n" +
                    $"New DB: {newDatabase}\n\n" +
                    $"Changing the database will prevent access to existing data.\n" +
                    $"Do you want to continue?",
                    "Database Change Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                }

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                // ConfigDataDB에 값 설정
                ConfigDataDB.sPostgresHost = txtHost.Text.Trim();
                ConfigDataDB.sPostgresPort = txtPort.Text.Trim();
                ConfigDataDB.sPostgresUsername = txtUsername.Text.Trim();
                ConfigDataDB.sPostgresPassword = txtPassword.Text.Trim();
                ConfigDataDB.sPostgresDatabase = txtDatabase.Text.Trim();
                ConfigDataDB.sPostgresTimezone = cmbTimezone.SelectedItem.ToString();
               // ConfigDataDB.sOperationalDataRetentionDays = numericUpDownRetetionDay.Value.ToString();

                // 설정 파일 저장
                SaveConfigFile();

                ConfigSaved = true;
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = Tools.IsLangKorean() ? "✓ 설정이 저장되었습니다." : "✓Settings have been saved.";

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = Tools.IsLangKorean()? $"✗ 저장 실패: {ex.Message}" : $"✗ Save failed: {ex.Message}";
                if(Tools.IsLangKorean())
                    MessageBox.Show($"설정 저장 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"An error occurred while saving settings:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadConfigFromFile()
        {
            try
            {
                string dbConfigFile = String.Format("{0}\\Config\\pgDB.inix", TotalConfig.sDirWorkProject);

                if (!File.Exists(dbConfigFile))
                    return;

                string[] lines = File.ReadAllLines(dbConfigFile, Encoding.UTF8);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("[") || line.StartsWith("#"))
                        continue;

                    string[] parts = line.Split(new char[] { '=' }, 2);
                    if (parts.Length != 2)
                        continue;

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key.ToLower())
                    {
                        case "host":
                            if (!string.IsNullOrWhiteSpace(value))
                                txtHost.Text = value;
                            break;
                        case "port":
                            if (!string.IsNullOrWhiteSpace(value))
                                txtPort.Text = value;
                            break;
                        case "username":
                            if (!string.IsNullOrWhiteSpace(value))
                                txtUsername.Text = value;
                            break;
                        case "password":
                            if (!string.IsNullOrWhiteSpace(value))
                                txtPassword.Text = ConfigDataDB.DecryptPassword(value);
                            break;
                        case "database":
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                txtDatabase.Text = value;
                                sPostgresDatabase = value; //원래 DB명을 기억.
                            }

                            break;
                        case "timezone":
                            if (!string.IsNullOrWhiteSpace(value))
                                cmbTimezone.Text = value;
                            break;
                        //case "dataretentiondays":
                        //    if (int.TryParse(value, out int days))
                        //        numericUpDownRetetionDay.Value = days;
                        //    break;
                    }
                }
            }
            catch (Exception ex)
            {
               if(Tools.IsLangKorean())
                    MessageBox.Show($"설정 파일 로드 중 오류가 발생했습니다: {ex.Message}", "파일 로드 오류",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else MessageBox.Show($"An error occurred while loading the configuration file: {ex.Message}", "File Load Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveConfigFile()
        {
            try
            {
                string dbConfigFile = String.Format("{0}\\Config\\pgDB.inix", TotalConfig.sDirWorkProject);
                string configDir = System.IO.Path.GetDirectoryName(dbConfigFile);

                if (!System.IO.Directory.Exists(configDir))
                {
                    System.IO.Directory.CreateDirectory(configDir);
                }

                // 비밀번호 암호화 (추가)
                string encryptedPassword = ConfigDataDB.EncryptPassword(txtPassword.Text);

                StringBuilder configContent = new StringBuilder();
                configContent.AppendLine("[PostgreSQL Database Configuration]");
                configContent.AppendLine($"Host={txtHost.Text.Trim()}");
                configContent.AppendLine($"Port={txtPort.Text.Trim()}");
                configContent.AppendLine($"Username={txtUsername.Text.Trim()}");
                configContent.AppendLine($"Password={encryptedPassword}");
                configContent.AppendLine($"Database={txtDatabase.Text.Trim()}");
                configContent.AppendLine($"Timezone={cmbTimezone.SelectedItem?.ToString() ?? "Asia/Seoul"}");
                //configContent.AppendLine($"DataRetentionDays={numericUpDownRetetionDay.Value}");
                configContent.AppendLine($"# Last Updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                File.WriteAllText(dbConfigFile, configContent.ToString(), Encoding.UTF8);

                // 현재 설정 업데이트
                sPostgresHost = txtHost.Text.Trim();
                sPostgresPort = txtPort.Text.Trim();
                sPostgresUsername = txtUsername.Text.Trim();
                sPostgresPassword = txtPassword.Text;
                sPostgresDatabase = txtDatabase.Text.Trim();
                sPostgresTimezone = cmbTimezone.SelectedItem?.ToString() ?? "Asia/Seoul";
                //sOperationalDataRetentionDays = (int)numericUpDownRetetionDay.Value;

                // 기존 DB명 업데이트
                sOriginalDatabase = sPostgresDatabase;
            }
            catch (Exception ex)
            {
                if(Tools.IsLangKorean())
                    throw new Exception($"설정 파일 저장 실패: {ex.Message}");
                else
                    throw new Exception($"Failed to save configuration file: {ex.Message}");
            }
        }


        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtHost.Text))
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("호스트를 입력해주세요.", "입력 확인",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Please enter the host.", "Input Check",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHost.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPort.Text))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("포트를 입력해주세요.", "입력 확인",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Please enter the port.", "Input Check",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPort.Focus();
                return false;
            }

            if (!int.TryParse(txtPort.Text, out int port) || port <= 0 || port > 65535)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("올바른 포트 번호를 입력해주세요. (1-65535)", "입력 확인",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Please enter a valid port number. (1-65535)", "Input Check",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtPort.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("사용자명을 입력해주세요.", "입력 확인",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Please enter the username.", "Input Check",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("데이터베이스명을 입력해주세요.", "입력 확인",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Please enter the database name.", "Input Check",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDatabase.Focus();
                return false;
            }

            if (cmbTimezone.SelectedIndex < 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("타임존을 선택해주세요.", "입력 확인",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show("Please select the timezone.", "Input Check",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);

                cmbTimezone.Focus();
                return false;
            }

            return true;
        }



        private string BuildConnectionString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Host={txtHost.Text.Trim()};");
            builder.Append($"Port={txtPort.Text.Trim()};");
            builder.Append($"Username={txtUsername.Text.Trim()};");
            builder.Append($"Password={txtPassword.Text.Trim()};");
            builder.Append($"Database={txtDatabase.Text.Trim()};");
            builder.Append($"Timezone={cmbTimezone.SelectedItem};");
            builder.Append("Timeout=30;");
            builder.Append("CommandTimeout=30;");
           // builder.Append("Options=-c lc_messages=en_US.UTF8;");
           builder.Append("Options=-c lc_messages=C;");

            return builder.ToString();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {

        }

        private void buttonImport_Click(object sender, EventArgs e)
        {

        }


    }
}
