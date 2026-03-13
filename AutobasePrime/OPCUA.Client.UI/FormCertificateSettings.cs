using NetTools;
using Opc.Ua;
using Opc.Ua.Configuration;
using OpcUa.Client.Host;
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
using static OpcUa.Client.Host.OpcUaCertificateService;

namespace OPCUA.Client.UI
{
    public partial class FormCertificateSettings : Form
    {
        private readonly ApplicationConfiguration _uaConfig;
        private readonly OpcUaHost _host;

        private string _certWorkingDir;

        public FormCertificateSettings(ApplicationConfiguration uaConfig, OpcUaHost host)
        {
            InitializeComponent();

            _uaConfig = uaConfig;
            _host = host;

            InitListView();

            // 초기 로딩
            _ = LoadCertsAsync();
            _ = UpdateClientCertInfoAsync();
            _ = UpdateClientCertStatusAsync();
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


        private async Task<X509Certificate2> GetClientCertificateAsync()
        {
            var appCert = _uaConfig.SecurityConfiguration.ApplicationCertificate;
            return await appCert.FindAsync(true);
        }

        private async Task UpdateClientCertInfoAsync()
        {
            var cert = await GetClientCertificateAsync();
            if (cert == null)
            {
                richTextBoxClientCert.Text = "Client certificate not found.";
                return;
            }

            var sb = new StringBuilder();

            sb.AppendLine("=== OPC UA Client Certificate ===");
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

        private async Task UpdateClientCertStatusAsync()
        {
            var cert = await GetClientCertificateAsync();

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

            try
            {
                await RegisterClientCertificateAsync(derPath, keyPath, password);

                MessageBox.Show(
                    "Client certificate registered successfully.\n" +
                    "All OPC UA connections will be re-established.",
                    "Certificate Registered",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                await UpdateClientCertInfoAsync();
                await UpdateClientCertStatusAsync();
                await _host?.OnClientCertificateRegenerated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RegisterClientCertificateAsync(
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

            // 3. Store에 저장
            using (var store = appCert.OpenStore(null))
            {
                await store.AddAsync(cert);
            }

            // 4. ApplicationCertificate 재바인딩
            appCert.Certificate = cert;
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

        private void buttonValidatePem_Click(object sender, EventArgs e)
        {
            string keyPath = textPrivateKeyPath.Text.Trim();

            if (string.IsNullOrEmpty(keyPath) || !File.Exists(keyPath))
            {
                MessageBox.Show("개인키 파일을 선택하세요.", "Error");
                return;
            }

            try
            {
                string content = File.ReadAllText(keyPath);
                var sb = new StringBuilder();

                sb.AppendLine("=== PEM 파일 분석 ===");
                sb.AppendLine();

                // 형식 확인
                if (content.Contains("-----BEGIN RSA PRIVATE KEY-----"))
                    sb.AppendLine("✓ PKCS#1 RSA Private Key (암호화 안됨)");
                else if (content.Contains("-----BEGIN PRIVATE KEY-----"))
                    sb.AppendLine("✓ PKCS#8 Private Key (암호화 안됨)");
                else if (content.Contains("-----BEGIN ENCRYPTED PRIVATE KEY-----"))
                    sb.AppendLine("✓ PKCS#8 Encrypted Private Key (암호 필요)");
                else if (content.Contains("Proc-Type: 4,ENCRYPTED"))
                    sb.AppendLine("✓ Encrypted PEM (암호 필요)");
                else
                    sb.AppendLine("✗ 인식할 수 없는 형식");

                sb.AppendLine();
                sb.AppendLine($"파일 크기: {new FileInfo(keyPath).Length:N0} bytes");
                sb.AppendLine($"줄 수: {content.Split('\n').Length}");

                MessageBox.Show(sb.ToString(), "PEM 검증");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private Org.BouncyCastle.Crypto.AsymmetricKeyParameter ParseAsn1Sequence(
            Org.BouncyCastle.Asn1.Asn1Sequence sequence,
            string password)
        {
            try
            {
                // PKCS#1 RSA Private Key 시도
                var rsaPrivateKey = Org.BouncyCastle.Asn1.Pkcs.RsaPrivateKeyStructure
                    .GetInstance(sequence);

                return new Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters(
                    rsaPrivateKey.Modulus,
                    rsaPrivateKey.PublicExponent,
                    rsaPrivateKey.PrivateExponent,
                    rsaPrivateKey.Prime1,
                    rsaPrivateKey.Prime2,
                    rsaPrivateKey.Exponent1,
                    rsaPrivateKey.Exponent2,
                    rsaPrivateKey.Coefficient);
            }
            catch
            {
                // PKCS#8 시도
                var privateKeyInfo = Org.BouncyCastle.Asn1.Pkcs.PrivateKeyInfo
                    .GetInstance(sequence);

                return Org.BouncyCastle.Security.PrivateKeyFactory
                    .CreateKey(privateKeyInfo);
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

        private static void BackupClientCertificateToDir(
        X509Certificate2 cert,
        string baseDir)
        {
            string dir = Path.Combine(
            baseDir,
            DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Directory.CreateDirectory(dir);

            File.WriteAllBytes(
                Path.Combine(dir, "client.cer"),
                cert.Export(X509ContentType.Cert));

            if (cert.HasPrivateKey)
            {
                File.WriteAllBytes(
                    Path.Combine(dir, "client.pfx"),
                    cert.Export(X509ContentType.Pfx));
            }

            // 메타 정보
            File.WriteAllText(
                Path.Combine(baseDir, "info.txt"),
                $"Subject    : {cert.Subject}\r\n" +
                $"Thumbprint : {cert.Thumbprint}\r\n" +
                $"NotBefore  : {cert.NotBefore}\r\n" +
                $"NotAfter   : {cert.NotAfter}\r\n");
        }



        private async void btnClientCertExport_Click(object sender, EventArgs e)
        {
            var cert = await GetClientCertificateAsync();
            if (cert == null)
            {
                MessageBox.Show("Client certificate not found.", "Export");
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

                    MessageBox.Show("Client certificate exported successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Export Error");
                }
            }
        }

        private async void btnClientCertImport_Click(object sender, EventArgs e)
        {

        }

        private async Task ImportClientPfxAsync(string pfxPath)
        {
            // 1. PFX 로드 (암호 없는 백업 기준)
            var cert = new X509Certificate2(
                pfxPath,
                (string)null,
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet |
                X509KeyStorageFlags.Exportable);

            if (!cert.HasPrivateKey)
                throw new Exception("PFX does not contain a private key.");

            var appCert = _uaConfig.SecurityConfiguration.ApplicationCertificate;

            // 2. 기존 인증서 삭제 (선택 정책)
            var oldCert = await appCert.FindAsync(true);
            if (oldCert != null)
            {
                using (var store = appCert.OpenStore(null))
                {
                    await store.DeleteAsync(oldCert.Thumbprint);
                }
            }

            // 3. Store에 추가
            using (var store = appCert.OpenStore(null))
            {
                await store.AddAsync(cert);
            }

            // 4. ApplicationCertificate 재바인딩
            appCert.Certificate = cert;
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
    }
}
