using AutoLibLocal;
using NetTools;
using Opc.Ua;
using Opc.Ua.Configuration;
using OPCUA.Client.Core;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain
{
    public partial class FormCertificateSettings : Form
    {
        private readonly ApplicationConfiguration _uaConfig;

        private string _certWorkingDir;

        public FormCertificateSettings(ApplicationConfiguration uaConfig)
        {
            InitializeComponent();

            _uaConfig = uaConfig;

            InitListView();

            // 초기 로딩
            _ = LoadCertsAsync();
            _ = UpdateServerCertInfoAsync();
            _ = UpdateServerCertStatusAsync();
        }

        private void InitListView()
        {
            m_list_certs.View = View.Details;
            m_list_certs.FullRowSelect = true;
            m_list_certs.MultiSelect = false;
            m_list_certs.HideSelection = false;

            m_list_certs.Columns.Clear();
            m_list_certs.Columns.Add("CN", 200);
            m_list_certs.Columns.Add("Subject / AppURI", 320);
            m_list_certs.Columns.Add("Trust", 90);
            m_list_certs.Columns.Add("Thumbprint", 260);

            buttonTrustReject.Enabled = false;
            buttonRemove.Enabled = false;
        }


        private async Task<X509Certificate2> GetServerCertificateAsync()
        {
            var appCert = _uaConfig.SecurityConfiguration.ApplicationCertificate;
            return await appCert.FindAsync(true);
        }

        private async Task UpdateServerCertInfoAsync()
        {
            var cert = await GetServerCertificateAsync();
            if (cert == null)
            {
                richTextBoxClientCert.Text = "Server certificate not found.";
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine("=== OPC UA Server Certificate ===");
            sb.AppendLine($"Subject       : {cert.Subject}");
            sb.AppendLine($"Issuer        : {cert.Issuer}");
            sb.AppendLine($"Thumbprint    : {cert.Thumbprint}");
            sb.AppendLine($"Serial Number : {cert.SerialNumber}");
            sb.AppendLine($"Valid From    : {cert.NotBefore:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Valid To      : {cert.NotAfter:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Key Size      : {cert.PublicKey.Key.KeySize}");
            sb.AppendLine($"Signature Alg : {cert.SignatureAlgorithm.FriendlyName}");

            var appUri = cert.GetNameInfo(X509NameType.UrlName, false);
            if (!string.IsNullOrEmpty(appUri))
                sb.AppendLine($"ApplicationURI: {appUri}");

            richTextBoxClientCert.Text = sb.ToString();
        }

        private async Task UpdateServerCertStatusAsync()
        {
            var cert = await GetServerCertificateAsync();

            if (cert == null)
            {
                labelCertStatusText.Text = "Not Installed";
                labelCertStatusText.ForeColor = Color.Red;
                return;
            }

            var now = DateTime.UtcNow;

            if (now > cert.NotAfter)
            {
                labelCertStatusText.Text = "Expired";
                labelCertStatusText.ForeColor = Color.Red;
            }
            else if ((cert.NotAfter - now).TotalDays < 30)
            {
                labelCertStatusText.Text = $"Expiring Soon ({cert.NotAfter:yyyy-MM-dd})";
                labelCertStatusText.ForeColor = Color.Orange;
            }
            else
            {
                labelCertStatusText.Text = "Valid";
                labelCertStatusText.ForeColor = Color.Green;
            }
        }

        // -----------------------------
        // List Load (Store 기반)
        // -----------------------------
        private async Task LoadCertsAsync()
        {
            try
            {
                m_list_certs.BeginUpdate();
                m_list_certs.Items.Clear();

                // Trusted
                await LoadStoreAsync(_uaConfig.SecurityConfiguration.TrustedPeerCertificates, "Trusted")
                    .ConfigureAwait(false);

                // Rejected
                await LoadStoreAsync(_uaConfig.SecurityConfiguration.RejectedCertificateStore, "Rejected")
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Certificates Error");
            }
            finally
            {
                if (m_list_certs.IsHandleCreated)
                {
                    m_list_certs.EndUpdate();
                }

                // UI 스레드에서 버튼 상태 초기화
                BeginInvoke((Action)(() =>
                {
                    buttonTrustReject.Enabled = false;
                    buttonRemove.Enabled = false;
                }));
            }
        }

        private async Task LoadStoreAsync(CertificateStoreIdentifier storeId, string state)
        {
            using (var store = storeId.OpenStore(null))
            {
                // SDK에 따라 EnumerateAsync/Enumerate 둘 다 있을 수 있음
                X509Certificate2Collection certs;
                try
                {
                    certs = await store.EnumerateAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {                  
                    MessageBox.Show(ex.Message, "Load Certificates Error");
                    return;
                }

                foreach (X509Certificate2 cert in certs)
                {
                    AddCertToListView(cert, state);
                }
            }
        }

        private void AddCertToListView(X509Certificate2 cert, string state)
        {
            string cn = cert.GetNameInfo(X509NameType.SimpleName, false);
            string subject = cert.Subject ?? "";

            var item = new ListViewItem(cn);
            item.SubItems.Add(subject);
            item.SubItems.Add(state);
            item.SubItems.Add(cert.Thumbprint);

            // 🔑 Store 기반에서는 Thumbprint가 유일 키
            item.Tag = cert.Thumbprint;

            item.ForeColor = (state == "Trusted") ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            // WinForms UI 스레드에서만 Items.Add 해야 안전
            if (m_list_certs.InvokeRequired)
            {
                m_list_certs.BeginInvoke((Action)(() => m_list_certs.Items.Add(item)));
            }
            else
            {
                m_list_certs.Items.Add(item);
            }
        }

        // -----------------------------
        // Selection Helpers
        // -----------------------------
        private string GetSelectedThumbprint()
        {
            if (m_list_certs.SelectedItems.Count == 0)
                return null;

            return m_list_certs.SelectedItems[0].Tag as string;
        }

        private string GetSelectedState()
        {
            if (m_list_certs.SelectedItems.Count == 0)
                return null;

            return m_list_certs.SelectedItems[0].SubItems[2].Text; // Trust column
        }

        private CertificateStoreIdentifier GetSelectedStoreId()
        {
            var state = GetSelectedState();
            if (state == "Trusted")
                return _uaConfig.SecurityConfiguration.TrustedPeerCertificates;

            return _uaConfig.SecurityConfiguration.RejectedCertificateStore;
        }

        private void m_list_certs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_list_certs.SelectedItems.Count == 0)
            {
                buttonTrustReject.Enabled = false;
                buttonRemove.Enabled = false;
                return;
            }

            buttonTrustReject.Enabled = true;
            buttonRemove.Enabled = true;

            var state = GetSelectedState();
            if (state == "Trusted")
            {
                buttonTrustReject.Text = "Reject";
                buttonTrustReject.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                buttonTrustReject.Text = "Trust";
                buttonTrustReject.BackColor = System.Drawing.Color.Green;
            }
        }

        // -----------------------------
        // Trust ↔ Reject (Store 기반)
        // -----------------------------
        private async void buttonTrustReject_Click(object sender, EventArgs e)
        {

            string thumbprint = GetSelectedThumbprint();
            if (string.IsNullOrEmpty(thumbprint))
                return;

            var state = GetSelectedState();
            bool isTrust = (state == "Rejected");

            string actionText = isTrust ? "Trust" : "Reject";
            string targetText = isTrust ? "신뢰(Trusted)" : "거부(Rejected)";

            // 1️⃣ 사전 확인 메시지
            var confirm = MessageBox.Show(
                $"선택한 인증서를 [{targetText}] 상태로 변경하시겠습니까?\n\n" +
                $"Thumbprint:\n{thumbprint}",
                $"Confirm {actionText}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                if (isTrust)
                {
                    await OpcUaCertificateService
                        .TrustAsync(_uaConfig, thumbprint)
                        .ConfigureAwait(false);
                }
                else
                {
                    await OpcUaCertificateService
                        .RejectAsync(_uaConfig, thumbprint)
                        .ConfigureAwait(false);
                }

                await LoadCertsAsync().ConfigureAwait(false);

                BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(
                        $"인증서가 성공적으로 [{targetText}] 처리되었습니다.",
                        "Certificate Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Certificate Error");
            }
        }

        // -----------------------------
        // Remove (Store 기반)
        // -----------------------------
        private async void buttonRemove_Click(object sender, EventArgs e)
        {
            string thumbprint = GetSelectedThumbprint();
            if (string.IsNullOrEmpty(thumbprint))
                return;

            var result = MessageBox.Show(
                "이 인증서를 삭제하면 다시 복구할 수 없습니다.\n계속하시겠습니까?",
                "Delete Certificate",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result != DialogResult.OK)
                return;

            try
            {
                var storeId = GetSelectedStoreId();
                using (var store = storeId.OpenStore(null))
                {
                    try
                    {
                        await store.DeleteAsync(thumbprint).ConfigureAwait(false);
                    }
                    catch ( Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Delete Error");
                        return;
                    }
                }

                await LoadCertsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        // -----------------------------
        // Export (Store 기반)
        // -----------------------------
        private async void buttonExport_Click(object sender, EventArgs e)
        {
            string thumbprint = GetSelectedThumbprint();
            if (string.IsNullOrEmpty(thumbprint))
                return;

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "Certificate (*.cer)|*.cer|DER (*.der)|*.der";
                dlg.FileName = thumbprint + ".cer";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var storeId = GetSelectedStoreId();
                    X509Certificate2 cert;

                    using (var store = storeId.OpenStore(null))
                    {
                        cert = await store.GetSingleByThumbprintAsync(thumbprint).ConfigureAwait(false);
                    }

                    if (cert == null)
                    {
                        MessageBox.Show("Certificate not found in store.", "Export");
                        return;
                    }

                    File.WriteAllBytes(dlg.FileName, cert.Export(X509ContentType.Cert));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Error");
                }
            }
        }

        // -----------------------------
        // Import (Rejected로 넣는 게 정석)
        // -----------------------------
        private async void buttonImport_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Certificate (*.cer;*.der)|*.cer;*.der";
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var cert = new X509Certificate2(dlg.FileName);

                    var rejected = _uaConfig.SecurityConfiguration.RejectedCertificateStore;
                    using (var store = rejected.OpenStore(null))
                    {
                        try
                        {
                            await store.AddAsync(cert).ConfigureAwait(false);
                        }
                        catch(Exception ex)
                        {
                           MessageBox.Show(ex.Message, "Import Error");
                            return;
                        }
                    }

                    await LoadCertsAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Error");
                }
            }
        }

        private readonly string _pkiRoot =
        Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OPCUA", "Client", "pki");

        private void buttonBrowseDer_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Certificate (*.der;*.cer)|*.der;*.cer|All Files (*.*)|*.*";
                dlg.Title = "Select DER Certificate";
                dlg.InitialDirectory = GetDefaultCertPath();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textDerPath.Text = dlg.FileName;
                }
            }
        }

        private void buttonBrowsePrivateKey_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Private Key (*.pem;*.key;*.pfx)|*.pem;*.key;*.pfx|All Files (*.*)|*.*";
                dlg.Title = "Select Private Key";
                dlg.InitialDirectory = GetDefaultCertPath();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textPrivateKeyPath.Text = dlg.FileName;
                }
            }
        }


        private async void buttonRegisterCert_Click(object sender, EventArgs e)
        {
            string derPath = textDerPath.Text.Trim();
            string keyPath = textPrivateKeyPath.Text.Trim();
            string password = textPassword.Text;

            if (string.IsNullOrEmpty(derPath) || !File.Exists(derPath))
            {
                MessageBox.Show("DER 인증서 파일을 선택하세요.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(keyPath) || !File.Exists(keyPath))
            {
                MessageBox.Show("개인키 파일을 선택하세요.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = MessageBox.Show(
                "서버 인증서를 변경하려면 서버를 일시 정지합니다.\n" +
                "변경 후 자동으로 재시작됩니다.\n\n계속하시겠습니까?",
                "Server Certificate Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (res != DialogResult.Yes)
                return;

            try
            {
                await OPCUAServerMain.Uninit();
                await RegisterServerCertificateAsync(derPath, keyPath, password);
                await OPCUAServerMain.Init();

                MessageBox.Show(
                    "Client certificate registered successfully.\n" +
                    "All OPC UA connections will be re-established.",
                    "Certificate Registered",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await UpdateServerCertInfoAsync();
                await UpdateServerCertStatusAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RegisterServerCertificateAsync(
       string derPath,
       string privateKeyPath,
       string password)
        {
            var appCert = _uaConfig.SecurityConfiguration.ApplicationCertificate;

            // 1. 기존 인증서 삭제
            var oldCert = await appCert.FindAsync(true);
            if (oldCert != null)
            {
                using (var store = appCert.OpenStore(null))
                {
                    await store.DeleteAsync(oldCert.Thumbprint);
                }
            }

            // 2. 인증서 + 개인키 결합
            X509Certificate2 cert = LoadCertificateWithPrivateKey(
                derPath,
                privateKeyPath,
                password);

            if (!cert.HasPrivateKey)
            {
                throw new Exception("Failed to load private key.");
            }

            await OPCUAServerMain.ImportServerCertificateAsync(
                cert);
        }

        private X509Certificate2 LoadCertificateWithPrivateKey(
            string derPath,
            string privateKeyPath,
            string password)
        {
            string ext = Path.GetExtension(privateKeyPath).ToLowerInvariant();

            // Case 1: PFX (인증서 + 개인키 포함)
            if (ext == ".pfx")
            {
                return new X509Certificate2(
                    privateKeyPath,
                    password,
                    X509KeyStorageFlags.MachineKeySet |
                    X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.Exportable);
            }

            // Case 2: DER + PEM 개인키 결합
            if (ext == ".pem" || ext == ".key")
            {
                byte[] derBytes = File.ReadAllBytes(derPath);
                X509Certificate2 cert = new X509Certificate2(derBytes);

                // PEM 개인키 로드 및 결합
                return CombineCertificateWithPemKey(cert, privateKeyPath, password);
            }

            throw new NotSupportedException($"Unsupported private key format: {ext}");
        }

        private X509Certificate2 CombineCertificateWithPemKey(
            X509Certificate2 cert,
            string pemPath,
            string password)
        {
            string pemContent = File.ReadAllText(pemPath);

            // 암호화된 PEM 체크
            bool isEncrypted = pemContent.Contains("ENCRYPTED");

            if (isEncrypted && string.IsNullOrEmpty(password))
            {
                throw new Exception("Encrypted PEM requires password.");
            }

            // BouncyCastle 사용
            return CombineWithBouncyCastle(cert, pemContent, password);
        }

        private X509Certificate2 CombineWithBouncyCastle(
            X509Certificate2 cert,
            string pemContent,
            string password)
        {
            try
            {
                using (var reader = new StringReader(pemContent))
                {
                    var pemReader = string.IsNullOrEmpty(password)
                        ? new PemReader(reader)
                        : new PemReader(reader, new PasswordFinder(password));

                    object pemObject = pemReader.ReadObject();

                    AsymmetricKeyParameter privateKey = null;

                    if (pemObject is AsymmetricCipherKeyPair keyPair)
                    {
                        privateKey = keyPair.Private;
                    }
                    else if (pemObject is AsymmetricKeyParameter key)
                    {
                        privateKey = key;
                    }
                    else
                    {
                        throw new Exception($"Unsupported PEM object: {pemObject?.GetType().Name}");
                    }

                    if (privateKey == null || !privateKey.IsPrivate)
                        throw new Exception("Failed to load private key from PEM");

                    // BouncyCastle → .NET 변환
                    var bcCert = DotNetUtilities.FromX509Certificate(cert);

                    var store = new Pkcs12StoreBuilder().Build();
                    var certEntry = new X509CertificateEntry(bcCert);

                    store.SetCertificateEntry("cert", certEntry);
                    store.SetKeyEntry(
                        "key",
                        new AsymmetricKeyEntry(privateKey),
                        new[] { certEntry });

                    using (var ms = new MemoryStream())
                    {
                        store.Save(ms, new char[0], new SecureRandom());

                        return new X509Certificate2(
                            ms.ToArray(),
                            "",
                            X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to combine certificate with PEM: {ex.Message}", ex);
            }
        }

        // BouncyCastle 암호 제공자
        private class PasswordFinder : Org.BouncyCastle.OpenSsl.IPasswordFinder
        {
            private readonly string _password;

            public PasswordFinder(string password)
            {
                _password = password;
            }

            public char[] GetPassword()
            {
                return _password?.ToCharArray();
            }
        }


        private string GetDefaultCertPath()
        {
            return _pkiRoot;
        }


        private async void btnClientCertExport_Click(object sender, EventArgs e)
        {
            var cert = await GetServerCertificateAsync();
            if (cert == null)
            {
                MessageBox.Show("Server certificate not found.", "Export");
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.InitialDirectory = GetDefaultCertPath();
                dlg.Filter = "Certificate (*.cer)|*.cer|DER (*.der)|*.der";
                dlg.FileName = $"client_{cert.Thumbprint}.cer";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    File.WriteAllBytes(
                        dlg.FileName,
                        cert.Export(X509ContentType.Cert));

                    MessageBox.Show("Server certificate exported successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Error");
                }
            }
        }


        private void buttonPwToggle_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (textPassword.UseSystemPasswordChar)
            {
                textPassword.UseSystemPasswordChar = false;
                buttonPwToggle.Text = "Hide";
            }
            else
            {
                textPassword.UseSystemPasswordChar = true;
                buttonPwToggle.Text = "Show";
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OPCUAServerMain.bAutoTrustStore = this.chkAutoTrustStore.Checked;
            

            TotalConfig.SaveRegAutoBaseConfig("Config", "Start", "OpcServerAutoTrustStore", OPCUAServerMain.bAutoTrustStore);
            Close();
        }
    }
}
