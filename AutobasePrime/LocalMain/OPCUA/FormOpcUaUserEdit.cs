using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain.OPCUA
{
    public partial class FormOpcUaUserEdit : Form
    {
        public OpcUaUserRecord Result { get; private set; }
        public string NewPassword { get; private set; }
        private readonly bool _isEdit;
        private readonly OpcUaUserRecord _origin;

        TextBox txtUserName;
        TextBox txtDisplayName;
        TextBox txtDescription;
        CheckBox chkEnabled;

        TextBox txtPassword;

        TextBox txtCertThumb;
        TextBox txtCertSubject;
        Button btnImportCert;

        CheckBox chkConnect;
        CheckBox chkBrowse;
        CheckBox chkRead;
        CheckBox chkWrite;

        Button btnOk;
        Button btnCancel;

        public FormOpcUaUserEdit(OpcUaUserRecord origin)
        {

            InitializeComponent();
            _origin = origin;
            _isEdit = origin != null;

            BuildUi();
            LoadFromOrigin();
        }

        private void BuildUi()
        {
            Text = _isEdit ? "Edit OPC UA User" : "Add OPC UA User";
            ClientSize = new Size(520, 460);
            StartPosition = FormStartPosition.CenterParent;

            int y = 15;

            Label L(string t, int yy)
                => new Label { Text = t, Left = 10, Top = yy + 4, Width = 120 };

            TextBox T(int yy)
                => new TextBox { Left = 140, Top = yy, Width = 350 };

            Controls.Add(L("User Name", y));
            txtUserName = T(y);
            Controls.Add(txtUserName);
            y += 30;

            Controls.Add(L("Display Name", y));
            txtDisplayName = T(y);
            Controls.Add(txtDisplayName);
            y += 30;

            Controls.Add(L("Description", y));
            txtDescription = T(y);
            Controls.Add(txtDescription);
            y += 30;

            chkEnabled = new CheckBox
            {
                Text = "Enabled",
                Left = 140,
                Top = y
            };
            Controls.Add(chkEnabled);
            y += 35;

            // ===== Authentication =====
            Controls.Add(new Label
            {
                Text = "Authentication",
                Font = new Font(Font, FontStyle.Bold),
                Left = 10,
                Top = y
            });
            y += 25;

            Controls.Add(L("Password", y));
            txtPassword = T(y);
            txtPassword.PasswordChar = '*';
            Controls.Add(txtPassword);
            y += 30;

            Controls.Add(L("Cert Thumbprint", y));
            txtCertThumb = T(y);
            txtCertThumb.ReadOnly = true;
            Controls.Add(txtCertThumb);
            y += 30;

            Controls.Add(L("Cert Subject", y));
            txtCertSubject = T(y);
            txtCertSubject.ReadOnly = true;
            Controls.Add(txtCertSubject);
            y += 30;

            btnImportCert = new Button
            {
                Text = "Import Certificate",
                Left = 140,
                Top = y,
                Width = 160
            };
            btnImportCert.Click += ImportCertificate;
            Controls.Add(btnImportCert);
            y += 40;

            // ===== Permissions =====
            Controls.Add(new Label
            {
                Text = "Permissions",
                Font = new Font(Font, FontStyle.Bold),
                Left = 10,
                Top = y
            });
            y += 25;

            chkConnect = new CheckBox { Text = "Connect", Left = 140, Top = y };
            chkBrowse = new CheckBox { Text = "Browse", Left = 260, Top = y };
            y += 25;
            chkRead = new CheckBox { Text = "Read", Left = 140, Top = y };
            chkWrite = new CheckBox { Text = "Write", Left = 260, Top = y };

            Controls.AddRange(new Control[]
            {
        chkConnect, chkBrowse, chkRead, chkWrite
            });

            // ===== Buttons =====
            btnOk = new Button { Text = "OK", Left = 300, Width = 90, Top = 410 };
            btnCancel = new Button { Text = "Cancel", Left = 400, Width = 90, Top = 410 };

            btnOk.Click += OnOk;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { btnOk, btnCancel });

            if (_isEdit)
                txtUserName.ReadOnly = true;
        }

        private void LoadFromOrigin()
        {
            if (!_isEdit)
            {
                chkEnabled.Checked = true;
                chkConnect.Checked = chkBrowse.Checked = chkRead.Checked = true;
                return;
            }

            txtUserName.Text = _origin.UserName;
            txtDisplayName.Text = _origin.DisplayName;
            txtDescription.Text = _origin.Description;
            chkEnabled.Checked = _origin.IsEnabled;

            txtCertThumb.Text = _origin.CertificateThumbprint;
            txtCertSubject.Text = _origin.CertificateSubject;

            chkConnect.Checked = _origin.Permissions.HasFlag(OpcUaUserPermissions.Connect);
            chkBrowse.Checked = _origin.Permissions.HasFlag(OpcUaUserPermissions.Browse);
            chkRead.Checked = _origin.Permissions.HasFlag(OpcUaUserPermissions.Read);
            chkWrite.Checked = _origin.Permissions.HasFlag(OpcUaUserPermissions.Write);
        }

        private void ImportCertificate(object sender, EventArgs e)
        {
            SelectFromTrustedStore();
        }

        private void SelectFromTrustedStore()
        {
            var certs = LoadTrustedCertificates();
            if (certs.Count == 0)
            {
                MessageBox.Show("No trusted certificates found.");
                return;
            }

            using (var dlg = new FormSelectCertificate(certs))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                var cert = dlg.SelectedCertificate;

                txtCertThumb.Text = cert.Thumbprint;
                txtCertSubject.Text = cert.Subject;
            }
        }

        private List<X509Certificate2> LoadTrustedCertificates()
        {
            var list = new List<X509Certificate2>();

            string path = OPCUAServerMain.config.SecurityConfiguration.TrustedPeerCertificates.StorePath;
            if (!Directory.Exists(path))
                return list;


            try
            {
                foreach (var file in Directory.GetFiles(path, "*.der",
            SearchOption.AllDirectories))
            {
                try
                {
                    list.Add(new X509Certificate2(file));
                }
                catch
                {
                    // skip broken cert
                }
            }

            foreach (var file in Directory.GetFiles(path, "*.cer",
            SearchOption.AllDirectories))
            {
                try
                {
                    list.Add(new X509Certificate2(file));
                }
                catch { }
            }
            }
            catch (UnauthorizedAccessException)
            {
                // 접근 권한 없는 폴더가 있을 수 있음
            }

            return list;
        }

        private void OnOk(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("UserName required.");
                return;
            }

            // Always create a new record so the caller decides
            // when to apply changes to the original.
            Result = new OpcUaUserRecord();
            Result.UserName = txtUserName.Text.Trim();
            Result.DisplayName = txtDisplayName.Text.Trim();
            Result.Description = txtDescription.Text.Trim();
            Result.IsEnabled = chkEnabled.Checked;

            Result.CertificateThumbprint = txtCertThumb.Text;
            Result.CertificateSubject = txtCertSubject.Text;

            Result.Permissions =
                (chkConnect.Checked ? OpcUaUserPermissions.Connect : 0) |
                (chkBrowse.Checked ? OpcUaUserPermissions.Browse : 0) |
                (chkRead.Checked ? OpcUaUserPermissions.Read : 0) |
                (chkWrite.Checked ? OpcUaUserPermissions.Write : 0);

            // Preserve existing password hash when password not changed
            if (_isEdit && string.IsNullOrEmpty(txtPassword.Text))
            {
                Result.SaltBase64 = _origin.SaltBase64;
                Result.HashBase64 = _origin.HashBase64;
                Result.Iterations = _origin.Iterations;
            }

            NewPassword = txtPassword.Text;

            DialogResult = DialogResult.OK;
        }

    }
}
