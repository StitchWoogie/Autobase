using AutoLibLocal;
using NetTools;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace LocalMain.OPCUA
{
    public partial class FormOpcUaServer : Form
    {
        private bool _running;

        private Timer timer1 = new Timer();
        // ObservableCollection<OPCUA_Certificate_Member> Model_Lists = new ObservableCollection<OPCUA_Certificate_Member>();
        string sTrustFolder;
        string sRejectFolder;
        //OPCUA_Certificate_Member MemberTemp;

        private const int MaxLogLines = 1000;

        public FormOpcUaServer()
        {
            InitializeComponent();

            SetupUI();

            if (OPCUAServerMain.config != null)
            {
                this.sTrustFolder = OPCUAServerMain.config.SecurityConfiguration.TrustedPeerCertificates.StorePath;
                this.sRejectFolder = OPCUAServerMain.config.SecurityConfiguration.RejectedCertificateStore.StorePath;

                sTrustFolder = Path.Combine(sTrustFolder, "certs");
                sRejectFolder = Path.Combine(sRejectFolder, "certs");
            }

            timer1.Interval = 1000;
            timer1.Tick += timer1_tick;

            this.chkServerEnable.Checked = OPCUAServerMain.bEnabled;
            chkServerEnable_CheckedChanged(this, EventArgs.Empty);

            if (this.checkBoxNone.Checked == false && this.checkBoxSha256.Checked == false)
            {
                this.checkBoxNone.Checked = true;
            }

            this.chkAnonymousToken.Checked = OPCUAServerMain.bUserTokenAnonymous;
            this.chkCertificateToken.Checked = OPCUAServerMain.bUserTokenCertificate;
            this.chkUsernameToken.Checked = OPCUAServerMain.bUserTokenUserName;

            this.checkBoxNone.Checked = OPCUAServerMain.bNone;
            this.checkBoxSha256.Checked = OPCUAServerMain.bSecurity;
        }

        private void timer1_tick(object sender, EventArgs e)
        {
            SetupUI();
        }

        private void SetupUI()
        {
            if (OPCUAServerMain._server is null || OPCUAServerMain._application is null)
            {
                this.lblStatus.Text = "Stopped";

                this.btnStart.Enabled = true;
                this.btnStop.Enabled = false;
                this.numericUpDownPort.Enabled = true;
                this.checkBoxNone.Enabled = true;
                this.checkBoxSha256.Enabled = true;
                this.chkAnonymousToken.Enabled = true;
                this.chkCertificateToken.Enabled = true;
                this.chkUsernameToken.Enabled = true;

            }
            else
            {
                if (OPCUAServerMain.bLoad)
                {
                    this.lblStatus.Text = "Running";
                    this.lblStatus.ForeColor = System.Drawing.Color.Blue;
                    this.btnStart.Enabled = false;
                    this.btnStop.Enabled = true;
                    this.numericUpDownPort.Enabled = false;
                    this.checkBoxNone.Enabled = false;
                    this.checkBoxSha256.Enabled = false;
                    this.buttonReloadIP.Enabled = false;
                    this.chkAnonymousToken.Enabled = false;
                    this.chkCertificateToken.Enabled = false;
                    this.chkUsernameToken.Enabled = false;


                    if (OPCUAServerMain.bIpv4error)
                    {
                        this.lblStatus.Text = "IPv4 Error";
                        this.lblStatus.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        this.lblStatus.Text = "Running";
                        this.lblStatus.ForeColor = System.Drawing.Color.Blue;
                    }

                }
                else
                {
                    this.lblStatus.Text = "Stopped";
                    this.lblStatus.ForeColor = System.Drawing.Color.Red;
                    this.btnStart.Enabled = true;
                    this.btnStop.Enabled = false;
                    this.numericUpDownPort.Enabled = true;
                    this.checkBoxNone.Enabled = true;
                    this.checkBoxSha256.Enabled = true;
                    this.buttonReloadIP.Enabled = true;
                    this.chkAnonymousToken.Enabled = true;
                    this.chkCertificateToken.Enabled = true;
                    this.chkUsernameToken.Enabled = true;
                }
            }
        }

        private void FormOpcUaServer_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Stopped";
            _running = OPCUAServerMain.bLoad;

            // Setup log display
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.BackColor = System.Drawing.Color.White;
            OpcUaServerLogBridge.Attach(AddLog);

            timer1.Start();
            this.numericUpDownPort.Value = OPCUAServerMain.nPort;
            LoadUI();
            LoadAvailableIPs();
            //SetMenuLang();
            //LoadCerts();
        }

        private void LoadUI()
        {
            this.numericUpDownPort.Value = OPCUAServerMain.nPort;
            this.labelIp.Text = OPCUAServerMain.buf_ip;
            this.checkBoxNone.Checked = OPCUAServerMain.bNone;
            this.checkBoxSha256.Checked = OPCUAServerMain.bSecurity;

            this.comboBoxSecurity.SelectedIndex = 0;
        }


        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_running) return;

            try
            {
                if (!String.IsNullOrEmpty(this.comboBoxIP.Text))
                    OPCUAServerMain.buf_ip = this.comboBoxIP.Text;
                labelIp.Text = OPCUAServerMain.buf_ip;

                OPCUAServerMain.nPort = Convert.ToInt32(numericUpDownPort.Value);
                OPCUAServerMain.bNone = this.checkBoxNone.Checked;
                OPCUAServerMain.bSecurity = this.checkBoxSha256.Checked;

                OPCUAServerMain.bUserTokenAnonymous = this.chkAnonymousToken.Checked;
                OPCUAServerMain.bUserTokenCertificate = this.chkCertificateToken.Checked;
                OPCUAServerMain.bUserTokenUserName = this.chkUsernameToken.Checked;

                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerSetIP", OPCUAServerMain.buf_ip);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerPortNumber", OPCUAServerMain.nPort);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerNone", OPCUAServerMain.bNone);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerSecurity", OPCUAServerMain.bSecurity);
                // UserToken 정책 저장
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenAnonymous", OPCUAServerMain.bUserTokenAnonymous);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenCertificate", OPCUAServerMain.bUserTokenCertificate);
                TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerUserTokenUserName", OPCUAServerMain.bUserTokenUserName);

                await OPCUAServerMain.Init();

                _running = true;
                lblStatus.Text = "Running";
                lblStatus.ForeColor = System.Drawing.Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Start Error");
            }
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            if (!_running) return;

            try
            {
                await OPCUAServerMain.Uninit();
                _running = false;

                lblStatus.Text = "Stopped";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Stop Error");
            }
        }

        private void checkBoxSha256_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxNone.Checked == false && this.checkBoxSha256.Checked == false)
            {
                this.checkBoxNone.Checked = true;
            }
        }

        private void buttonReloadIP_Click(object sender, EventArgs e)
        {
            LoadAvailableIPs();
        }

        private void LoadAvailableIPs()
        {
            try
            {
                comboBoxIP.Items.Clear();

                // 사용 가능한 IP 목록 가져오기
                var availableIPs = GetAvailableIPv4Addresses();

                if (availableIPs.Count == 0)
                {
                    MessageBox.Show("사용 가능한 네트워크 인터페이스를 찾을 수 없습니다.",
                        "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 콤보박스에 IP 추가
                foreach (var ip in availableIPs)
                {
                    comboBoxIP.Items.Add(ip);
                }

                if (!string.IsNullOrEmpty(OPCUAServerMain.buf_ip))
                {
                    // 설정된 IP가 목록에 있으면 선택
                    int index = comboBoxIP.Items.IndexOf(OPCUAServerMain.buf_ip);
                    if (index >= 0)
                    {
                        comboBoxIP.SelectedIndex = index;
                    }
                    else
                    {
                        if (comboBoxIP.Items.Count > 0)
                            comboBoxIP.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (comboBoxIP.Items.Count > 0)
                        comboBoxIP.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"IP 목록 로드 중 오류 발생:\n{ex.Message}",
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 사용 가능한 IPv4 주소 목록 조회
        /// </summary>
        public static List<string> GetAvailableIPv4Addresses()
        {
            var addresses = new List<string>();

            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var ni in interfaces)
                {
                    // 활성화된 인터페이스만
                    if (ni.OperationalStatus != OperationalStatus.Up)
                        continue;

                    // 가상 어댑터 제외 (옵션 - 필요시 주석 처리)
                    if (IsVirtualAdapter(ni))
                        continue;

                    var ipProps = ni.GetIPProperties();
                    foreach (var addr in ipProps.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(addr.Address))
                        {
                            addresses.Add(addr.Address.ToString());
                        }
                    }
                }

                // IP 주소 정렬 (선택사항)
                addresses.Sort((a, b) =>
                {
                    var partsA = a.Split('.').Select(int.Parse).ToArray();
                    var partsB = b.Split('.').Select(int.Parse).ToArray();

                    for (int i = 0; i < 4; i++)
                    {
                        if (partsA[i] != partsB[i])
                            return partsA[i].CompareTo(partsB[i]);
                    }
                    return 0;
                });
            }
            catch (Exception ex)
            {
                // 로깅
                System.Diagnostics.Debug.WriteLine($"IP 주소 조회 오류: {ex.Message}");
            }

            return addresses;
        }

        private static bool IsVirtualAdapter(NetworkInterface ni)
        {
            string desc = ni.Description.ToLower();
            string name = ni.Name.ToLower();

            return desc.Contains("virtual") ||
                   desc.Contains("vmware") ||
                   desc.Contains("vbox") ||
                   desc.Contains("virtualbox") ||
                   desc.Contains("hyper-v") ||
                   desc.Contains("tap-windows") ||
                   name.Contains("virtual");
        }

        private void setupPKIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCertificateSettings formSetupPki = new FormCertificateSettings(OPCUAServerMain.config);
            formSetupPki.ShowDialog();
        }

        private void menuCreateCert_Click(object sender, EventArgs e)
        {
            FormCreateCert formCreateCert = new FormCreateCert();
            formCreateCert.ShowDialog();
        }

        private void chkServerEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkServerEnable.Checked)
            {
                groupBoxServer.Enabled = true;
                groupBoxSecurity.Enabled = true;
                groupBoxUser.Enabled = true;
            }
            else 
            {                
                groupBoxServer.Enabled = false;
                groupBoxSecurity.Enabled = false;
                groupBoxUser.Enabled = false;
                _ = OPCUAServerMain.Uninit();
            }
        }

        private void FormOpcUaServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            OpcUaServerLogBridge.Detach(AddLog);

            OPCUAServerMain.bEnabled = this.chkServerEnable.Checked;
            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "EnableOpcUAServer", OPCUAServerMain.bEnabled);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuUserSettings_Click(object sender, EventArgs e)
        {
            var form = new FormOpcUaUsers();
            form.UsersSaved += (s, args) =>
            {
                OPCUAServerMain.ReloadUsers(args.Users);
            };
            form.ShowDialog(this);
        }

        #region Server Log

        /// <summary>
        /// Adds a log entry to the RichTextBox with color coding.
        /// Thread-safe via InvokeRequired / BeginInvoke.
        /// </summary>
        private void AddLog(ServerLogEntry entry)
        {
            if (entry == null) return;

            if (IsDisposed || !IsHandleCreated) return;

            if (richTextBoxLog.InvokeRequired)
            {
                try
                {
                    richTextBoxLog.BeginInvoke(new Action<ServerLogEntry>(AddLog), entry);
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            try
            {
                // Trim old lines to prevent memory issues
                if (richTextBoxLog.Lines.Length > MaxLogLines)
                {
                    int removeUpTo = richTextBoxLog.GetFirstCharIndexFromLine(
                        richTextBoxLog.Lines.Length - MaxLogLines);
                    if (removeUpTo > 0)
                    {
                        richTextBoxLog.Select(0, removeUpTo);
                        richTextBoxLog.SelectedText = "";
                    }
                }

                // Format: [HH:mm:ss] [LEVEL] [Category] Message
                string text = string.Format("[{0:HH:mm:ss}] [{1}] [{2}] {3}\n",
                    entry.Time,
                    entry.Level ?? "INFO",
                    entry.Category ?? "",
                    entry.Message ?? "");

                // Color coding
                System.Drawing.Color color;
                switch (entry.Level)
                {
                    case "ERROR":
                        color = System.Drawing.Color.Red;
                        break;
                    case "WARN":
                        color = System.Drawing.Color.DarkOrange;
                        break;
                    default:
                        color = System.Drawing.Color.Black;
                        break;
                }

                richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
                richTextBoxLog.SelectionLength = 0;
                richTextBoxLog.SelectionColor = color;
                richTextBoxLog.AppendText(text);
                richTextBoxLog.ScrollToCaret();
            }
            catch
            {
                // logging should never crash the UI
            }
        }

        #endregion

        private void autoSetUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new FormAutoSetup())
            {
                // 현재 설정 반영
                dlg.UseReconnect = OPCUAServerMain.AutoRestart;
                dlg.ReconnectIntervalSec = OPCUAServerMain.AutoRestartIntervalSec;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    OPCUAServerMain.SetAutoRestart(
                        dlg.UseReconnect,
                        dlg.ReconnectIntervalSec);
                }
            }
        }
    }

}
