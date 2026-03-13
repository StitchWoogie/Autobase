using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Client.Abstractions;
using OpcUa.Client.Host;
using OPCUA.Client.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCUA.Client.UI
{
    public partial class FormAddServer : Form
    {
        private ApplicationConfiguration _config;
        private EndpointDescription _selectedEndpoint;

        public string ServerName { get; private set; }
        public string EndpointUrl { get; private set; }
        public string DiscoveryUrl { get; private set; }  // 사용자가 입력한 원본 URL
        public string AccessName { get; private set; }
        public int EndpointIndex { get; private set; }
        public string ServerDisplayName { get; private set; }

        // Endpoint Security Info (사용자가 선택한 endpoint 보안 정보)
        public string SecurityPolicyUri { get; private set; }
        public int SecurityModeValue { get; private set; }

        // Authentication
        public OpcUaAuthMode AuthMode { get; private set; }
        public string AuthUserName { get; private set; }
        public string AuthPasswordProtected { get; private set; }
        public string AuthCertificateThumbprint { get; private set; }
        public string AuthCertificateFilePath { get; private set; }
        public string AuthCertPasswordProtected { get; private set; }
        public bool SaveCredentials { get; private set; }

        private bool _isModify = false;
        private string _originEndpointUrl;

        /// <summary>비동기 버튼 중복 클릭 방지 플래그</summary>
        private bool _asyncBusy;

        public FormAddServer(ApplicationConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            InitializeComponent();

            cmbAuthMode.SelectedIndex = 0; // Anonymous
        }

        public void SetModify(
                string serverName,
                string endpointUrl,
                string accessName,
                int endpointIndex,
                OpcUaAuthMode authMode = OpcUaAuthMode.Anonymous,
                string authUserName = null,
                string authCertThumbprint = null,
                string authCertFilePath = null,
                string authPasswordProtected = null)
        {
            _isModify = true;

            _originEndpointUrl = endpointUrl;

            txtDiscoveryUrl.Text = serverName;
            txtAccessName.Text = accessName;

            this.Text = "Modify OPC UA Server";
            btnAdd.Text = "Apply";

            // Auth
            cmbAuthMode.SelectedIndex = (int)authMode;
            if (!string.IsNullOrEmpty(authUserName))
                txtAuthUser.Text = authUserName;
            if (!string.IsNullOrEmpty(authCertThumbprint))
                txtCertThumbprint.Text = authCertThumbprint;
            if (!string.IsNullOrEmpty(authCertFilePath))
                txtCertFilePath.Text = authCertFilePath;

            // Restore saved credentials (password)
            if (!string.IsNullOrEmpty(authPasswordProtected))
            {
                try
                {
                    txtAuthPassword.Text = OpcUaCredentialProtection.Unprotect(authPasswordProtected);
                }
                catch
                {
                    txtAuthPassword.Text = string.Empty;
                }
                chkSaveCredentials.Checked = true;
                chkSaveCredentials.Enabled = false; // 이미 저장된 경우 해제 불가
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FromAddServer_Load(object sender, EventArgs e)
        {
            // Modify 모드라도 자동 브라우징 하지 않음
            // 사용자가 수동으로 Find Servers → Get Endpoints 를 클릭해야 함
        }

        private sealed class ServerItem
        {
            public ApplicationDescription Description { get; set; }
            public string DiscoveryUrl { get; set; }

            public override string ToString()
            {
                var name = Description?.ApplicationName?.Text;
                if (string.IsNullOrEmpty(name))
                    name = "(Unknown Server)";

                return $"{name} [{DiscoveryUrl}]";
            }
        }

        private sealed class EndpointItem
        {
            public EndpointDescription Endpoint { get; set; }

            public override string ToString()
            {
                if (Endpoint == null)
                    return "(null)";

                string policyShort = GetPolicyShortName(
                    Endpoint.SecurityPolicyUri);

                return $"{Endpoint.EndpointUrl} [{Endpoint.SecurityMode} / {policyShort}] (Level:{Endpoint.SecurityLevel})";
            }

            private static string GetPolicyShortName(string policyUri)
            {
                if (string.IsNullOrEmpty(policyUri))
                    return "None";

                int idx = policyUri.LastIndexOf('#');
                if (idx >= 0 && idx < policyUri.Length - 1)
                    return policyUri.Substring(idx + 1);

                return policyUri;
            }
        }

        private async Task LoadForModifyAsync()
        {
            cmbServers.Items.Clear();
            cmbEndpoints.Items.Clear();

            try
            {
                var discoveryUri = new Uri(txtDiscoveryUrl.Text.Trim());

                using (var dc = await DiscoveryClient.CreateAsync(
                    _config,
                    discoveryUri,
                    DiagnosticsMasks.None))
                {
                    var servers = await dc.FindServersAsync(null);

                    foreach (var s in servers)
                    {
                        var name = s.ApplicationName?.Text ?? "(Unknown Server)";

                        foreach (var url in s.DiscoveryUrls)
                        {
                            cmbServers.Items.Add(new ServerItem
                            {
                                Description = s,
                                DiscoveryUrl = url
                            });
                        }
                    }

                    if (cmbServers.Items.Count > 0)
                        cmbServers.SelectedIndex = 0;
                }

                for (int i = 0; i < cmbServers.Items.Count; i++)
                {
                    var item = cmbServers.Items[i] as ServerItem;
                    if (item != null && item.DiscoveryUrl == _originEndpointUrl)
                    {
                        cmbServers.SelectedIndex = i;
                        break;
                    }
                }

                if (cmbServers.SelectedItem != null)
                {
                    var serverItem = cmbServers.SelectedItem as ServerItem;
                    if (serverItem == null)
                        return;

                    var userUri = discoveryUri;  // 사용자가 입력한 원본 URL
                    var serverUri = new Uri(serverItem.DiscoveryUrl);

                    using (var dc = await DiscoveryClient.CreateAsync(
                        _config,
                        serverUri,
                        DiagnosticsMasks.None))
                    {
                        var endpoints = await dc.GetEndpointsAsync(null);

                        var sorted = endpoints
                            .OrderByDescending(ep => ep.SecurityLevel)
                            .ThenBy(ep => ep.SecurityMode.ToString())
                            .ThenBy(ep => ep.SecurityPolicyUri);

                        foreach (var ep in sorted)
                        {
                            ep.EndpointUrl = ReplaceHostInUrl(ep.EndpointUrl, userUri);
                            cmbEndpoints.Items.Add(new EndpointItem { Endpoint = ep });
                        }
                    }

                    for (int i = 0; i < cmbEndpoints.Items.Count; i++)
                    {
                        var item = cmbEndpoints.Items[i] as EndpointItem;
                        if (item != null && item.Endpoint.EndpointUrl == _originEndpointUrl)
                        {
                            cmbEndpoints.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Modify load failed:\n" + ex.Message);
            }
        }

        private async void btnFindServers_Click(object sender, EventArgs e)
        {
            if (_asyncBusy) return;
            _asyncBusy = true;
            btnFindServers.Enabled = false;
            try
            {
                cmbServers.Items.Clear();
                cmbEndpoints.Items.Clear();

                var discoveryUri = new Uri(txtDiscoveryUrl.Text.Trim());

                using (var dc = await DiscoveryClient.CreateAsync(
                  _config,
                  discoveryUri,
                  DiagnosticsMasks.None))
                {
                    var servers = await dc.FindServersAsync(null);

                    foreach (var s in servers)
                    {
                        foreach (var url in s.DiscoveryUrls)
                        {
                            cmbServers.Items.Add(new ServerItem
                            {
                                Description = s,
                                DiscoveryUrl = url
                            });
                        }
                    }
                }

                if (cmbServers.Items.Count > 0)
                    cmbServers.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FindServers failed:\n" + ex.Message);
            }
            finally
            {
                btnFindServers.Enabled = true;
                _asyncBusy = false;
            }
        }

        private async void btnGetEndpoints_Click(object sender, EventArgs e)
        {
            if (_asyncBusy) return;
            _asyncBusy = true;
            btnGetEndpoints.Enabled = false;
            try
            {
                cmbEndpoints.Items.Clear();

                var serverItem = cmbServers.SelectedItem as ServerItem;
                if (serverItem == null)
                    return;

                // 사용자가 입력한 원본 URL의 호스트를 기억
                var userUri = new Uri(txtDiscoveryUrl.Text.Trim());
                var serverUri = new Uri(serverItem.DiscoveryUrl);

                using (var dc = await DiscoveryClient.CreateAsync(
                    _config,
                    serverUri,
                    DiagnosticsMasks.None))
                {
                    var endpoints = await dc.GetEndpointsAsync(null);

                    var sorted = endpoints
                        .OrderByDescending(ep => ep.SecurityLevel)
                        .ThenBy(ep => ep.SecurityMode.ToString())
                        .ThenBy(ep => ep.SecurityPolicyUri);

                    foreach (var ep in sorted)
                    {
                        // 서버가 반환한 endpoint URL의 호스트를 사용자가 입력한 호스트로 치환
                        ep.EndpointUrl = ReplaceHostInUrl(ep.EndpointUrl, userUri);
                        cmbEndpoints.Items.Add(new EndpointItem { Endpoint = ep });
                    }
                }

                if (cmbEndpoints.Items.Count > 0)
                    cmbEndpoints.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetEndpoints failed:\n" + ex.Message);
            }
            finally
            {
                btnGetEndpoints.Enabled = true;
                _asyncBusy = false;
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (_asyncBusy) return;
            _asyncBusy = true;
            buttonTestCon.Enabled = false;
            try
            {
                var endpointItem = cmbEndpoints.SelectedItem as EndpointItem;
                if (endpointItem == null)
                {
                    MessageBox.Show("Select an endpoint first.");
                    return;
                }

                var endpoint = endpointItem.Endpoint;

                var endpointConfig = EndpointConfiguration.Create(_config);
                var configuredEndpoint =
                    new ConfiguredEndpoint(null, endpoint, endpointConfig);

                var factory = new DefaultSessionFactory(
               OpcUaClientTelemetry.Telemetry);

                // Build identity from UI
                IUserIdentity testIdentity = BuildIdentityFromUI();

                using (var session = await factory.CreateAsync(
                    _config,
                    configuredEndpoint,
                    true,
                    false,
                    "TestSession",
                    60000,
                    testIdentity,
                    null,
                    CancellationToken.None))
                {
                    MessageBox.Show("Connection OK");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed:\n" + ex.Message);
            }
            finally
            {
                buttonTestCon.Enabled = true;
                _asyncBusy = false;
            }
        }

        private IUserIdentity BuildIdentityFromUI()
        {
            var mode = (OpcUaAuthMode)cmbAuthMode.SelectedIndex;

            switch (mode)
            {
                case OpcUaAuthMode.UserName:
                    var userNameToken = new UserNameIdentityToken
                    {
                        UserName = txtAuthUser.Text.Trim(),
                        DecryptedPassword = System.Text.Encoding.UTF8.GetBytes(txtAuthPassword.Text)
                    };
                    return new UserIdentity(userNameToken);

                case OpcUaAuthMode.Certificate:
                    // Try file-based certificate first (PFX with private key)
                    if (!string.IsNullOrEmpty(txtCertFilePath.Text) &&
                        File.Exists(txtCertFilePath.Text))
                    {
                        try
                        {
                            string certPwd = txtCertPassword.Text;
                            var fileCert = new X509Certificate2(
                                txtCertFilePath.Text,
                                string.IsNullOrEmpty(certPwd) ? (string)null : certPwd,
                                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);

                            if (!fileCert.HasPrivateKey)
                            {
                                MessageBox.Show("Certificate file does not contain a private key.");
                                fileCert.Dispose();
                                return new UserIdentity(new AnonymousIdentityToken());
                            }

                            return new UserIdentity(fileCert);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to load certificate from file:\n" + ex.Message);
                            return new UserIdentity(new AnonymousIdentityToken());
                        }
                    }

                    // Fallback to thumbprint-based lookup
                    if (string.IsNullOrEmpty(txtCertThumbprint.Text))
                    {
                        MessageBox.Show("Select a certificate first.");
                        return new UserIdentity(new AnonymousIdentityToken());
                    }

                    var storeCert = FindCertByThumbprint(txtCertThumbprint.Text.Trim());
                    if (storeCert == null)
                    {
                        MessageBox.Show("Certificate not found in store.");
                        return new UserIdentity(new AnonymousIdentityToken());
                    }
                    return new UserIdentity(storeCert);

                default:
                    return new UserIdentity(new AnonymousIdentityToken());
            }
        }

        private X509Certificate2 FindCertByThumbprint(string thumbprint)
        {
            if (_config == null || string.IsNullOrEmpty(thumbprint))
                return null;

            try
            {
                var storeId = _config.SecurityConfiguration.ApplicationCertificate;
                using (var store = storeId.OpenStore(null))
                {
                    var cert = store.GetSingleByThumbprintAsync(thumbprint)
                        .GetAwaiter().GetResult();
                    if (cert != null)
                        return cert;
                }

                var trustedId = _config.SecurityConfiguration.TrustedPeerCertificates;
                using (var store = trustedId.OpenStore(null))
                {
                    var cert = store.GetSingleByThumbprintAsync(thumbprint)
                        .GetAwaiter().GetResult();
                    if (cert != null)
                        return cert;
                }
            }
            catch { }

            return null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var endpointItem = cmbEndpoints.SelectedItem as EndpointItem;
            if (endpointItem == null)
            {
                MessageBox.Show("Endpoint not selected.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAccessName.Text))
            {
                MessageBox.Show("Access Name is required.");
                return;
            }

            var serverItem = (ServerItem)cmbServers.SelectedItem;
            var endpoint = endpointItem.Endpoint;

            ServerName = serverItem.DiscoveryUrl;
            ServerDisplayName = serverItem.Description.ApplicationName.Text;
            EndpointUrl = endpoint.EndpointUrl;
            DiscoveryUrl = txtDiscoveryUrl.Text.Trim();  // 사용자가 입력한 원본 URL 보존
            AccessName = txtAccessName.Text.Trim();
            EndpointIndex = cmbEndpoints.SelectedIndex;

            // Endpoint Security Info
            SecurityPolicyUri = endpoint.SecurityPolicyUri;
            SecurityModeValue = (int)endpoint.SecurityMode;

            // Authentication
            AuthMode = (OpcUaAuthMode)cmbAuthMode.SelectedIndex;

            switch (AuthMode)
            {
                case OpcUaAuthMode.UserName:
                    AuthUserName = txtAuthUser.Text.Trim();
                    AuthPasswordProtected = OpcUaCredentialProtection.Protect(
                        txtAuthPassword.Text);
                    SaveCredentials = chkSaveCredentials.Checked;
                    AuthCertificateThumbprint = null;
                    break;

                case OpcUaAuthMode.Certificate:
                    AuthUserName = null;
                    AuthPasswordProtected = null;
                    AuthCertificateThumbprint = txtCertThumbprint.Text.Trim();
                    AuthCertificateFilePath = txtCertFilePath.Text.Trim();
                    AuthCertPasswordProtected = !string.IsNullOrEmpty(txtCertPassword.Text)
                        ? OpcUaCredentialProtection.Protect(txtCertPassword.Text)
                        : null;
                    break;

                default: // Anonymous
                    AuthUserName = null;
                    AuthPasswordProtected = null;
                    AuthCertificateThumbprint = null;
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // ===========================
        // Auth mode switching
        // ===========================

        private void cmbAuthMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mode = (OpcUaAuthMode)cmbAuthMode.SelectedIndex;

            bool isUserName = (mode == OpcUaAuthMode.UserName);
            bool isCert = (mode == OpcUaAuthMode.Certificate);

            // Username/Password controls
            lblUserName.Visible = isUserName;
            txtAuthUser.Visible = isUserName;
            lblPassword.Visible = isUserName;
            txtAuthPassword.Visible = isUserName;
            chkSaveCredentials.Visible = isUserName;
            btnShowPassword.Visible = isUserName;

            // Certificate controls
            lblCertThumb.Visible = isCert;
            txtCertThumbprint.Visible = isCert;
            btnBrowseCert.Visible = isCert;
            lblCertFile.Visible = isCert;
            txtCertFilePath.Visible = isCert;
            btnBrowseCertFile.Visible = isCert;
            lblCertPassword.Visible = isCert;
            txtCertPassword.Visible = isCert;
        }

        /// <summary>
        /// 서버가 반환한 endpoint URL의 호스트를 사용자가 입력한 URL의 호스트로 치환.
        /// 예: 서버 반환 "opc.tcp://PSU-DEV:43344/" + 사용자 입력 "opc.tcp://127.0.0.1:43344/"
        ///   → "opc.tcp://127.0.0.1:43344/"
        /// </summary>
        private static string ReplaceHostInUrl(string endpointUrl, Uri userUri)
        {
            if (string.IsNullOrEmpty(endpointUrl) || userUri == null)
                return endpointUrl;

            try
            {
                var epUri = new Uri(endpointUrl);

                if (string.Equals(epUri.Host, userUri.Host, StringComparison.OrdinalIgnoreCase))
                    return endpointUrl;

                var builder = new UriBuilder(epUri)
                {
                    Host = userUri.Host
                };

                return builder.Uri.ToString();
            }
            catch
            {
                return endpointUrl;
            }
        }

        private void btnBrowseCert_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Certificate (public key only)";
                dlg.Filter = "Certificate Files (*.cer;*.crt;*.der)|*.cer;*.crt;*.der|All Files (*.*)|*.*";

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    var cert = new X509Certificate2(dlg.FileName);
                    txtCertThumbprint.Text = cert.Thumbprint;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load certificate:\n" + ex.Message);
                }
            }
        }

        private void btnBrowseCertFile_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Certificate with Private Key (.pfx / .p12)";
                dlg.Filter = "PFX/P12 Files (*.pfx;*.p12)|*.pfx;*.p12|All Files (*.*)|*.*";

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    string password = txtCertPassword.Text;

                    X509Certificate2 cert;
                    if (!string.IsNullOrEmpty(password))
                    {
                        cert = new X509Certificate2(
                            dlg.FileName,
                            password,
                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                    }
                    else
                    {
                        try
                        {
                            cert = new X509Certificate2(
                                dlg.FileName,
                                (string)null,
                                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                        }
                        catch
                        {
                            MessageBox.Show(
                                "This PFX file requires a password.\nPlease enter the password in the 'Cert Password' field and try again.",
                                "Password Required",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            return;
                        }
                    }

                    if (!cert.HasPrivateKey)
                    {
                        MessageBox.Show(
                            "The selected file does not contain a private key.\nPlease select a .pfx or .p12 file that includes the private key.",
                            "No Private Key",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        cert.Dispose();
                        return;
                    }

                    txtCertFilePath.Text = dlg.FileName;
                    txtCertThumbprint.Text = cert.Thumbprint;

                    cert.Dispose();
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    MessageBox.Show(
                        "Invalid password or corrupted PFX file.\nPlease check the password and try again.",
                        "Certificate Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load certificate:\n" + ex.Message);
                }
            }
        }

        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            if (txtAuthPassword.PasswordChar == '*')
            {
                txtAuthPassword.PasswordChar = '\0';
                btnShowPassword.Text = "Hide";
            }
            else
            {
                txtAuthPassword.PasswordChar = '*';
                btnShowPassword.Text = "Show";
            }
        }
    }
}
