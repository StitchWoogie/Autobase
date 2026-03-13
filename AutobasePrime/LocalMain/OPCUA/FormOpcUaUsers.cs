using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain.OPCUA
{
    public partial class FormOpcUaUsers : Form
    {
        private readonly string _storePath =
           Path.Combine(
                TotalConfig.sDirWorkProject,
                "OpcData", "Server", "users.json");

        private readonly OpcUaUserStore _store;
        private List<OpcUaUserRecord> _users;

        private ListView lv;
        private Button btnAdd, btnEdit, btnDel, btnSave, btnClose;

        public event EventHandler<OpcUaUsersChangedEventArgs> UsersSaved;

        public FormOpcUaUsers()
        {
            InitializeComponent();
            _store = new OpcUaUserStore(_storePath);
            BuildUi();
            LoadUsers();
            RefreshList();
        }

        private void BuildUi()
        {
            Text = "OPC UA Users";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(760, 420);

            lv = new ListView
            {
                Left = 10,
                Top = 10,
                Width = 740,
                Height = 330,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false
            };

            lv.Columns.Add("UserName", 200);
            lv.Columns.Add("DisplayName", 220);
            lv.Columns.Add("Description", 220);
            lv.Columns.Add("Permissions", 160);
            lv.Columns.Add("Enabled", 80);
            lv.Columns.Add("Auth", 100);

            btnAdd = new Button { Left = 10, Top = 355, Width = 90, Text = "Add" };
            btnEdit = new Button { Left = 110, Top = 355, Width = 90, Text = "Edit" };
            btnDel = new Button { Left = 210, Top = 355, Width = 90, Text = "Delete" };
            btnSave = new Button { Left = 560, Top = 355, Width = 90, Text = "Save" };
            btnClose = new Button { Left = 660, Top = 355, Width = 90, Text = "Close" };

            btnAdd.Click += (s, e) => AddUser();
            btnEdit.Click += (s, e) => EditUser();
            btnDel.Click += (s, e) => DeleteUser();
            btnSave.Click += (s, e) => SaveUsers();
            btnClose.Click += (s, e) => Close();

            Controls.Add(lv);
            Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDel, btnSave, btnClose });
        }

        private void LoadUsers()
        {
            _users = _store.Load();
        }

        private void RefreshList()
        {
            lv.BeginUpdate();
            lv.Items.Clear();

            foreach (var u in _users.OrderBy(x => x.UserName, StringComparer.OrdinalIgnoreCase))
            {
                var item = new ListViewItem(u.UserName);
                item.SubItems.Add(u.DisplayName ?? "");
                item.SubItems.Add(u.Description ?? "");
                item.SubItems.Add(u.Permissions.ToString());
                item.SubItems.Add(u.IsEnabled ? "Yes" : "No");

                string auth =
                    (!string.IsNullOrEmpty(u.HashBase64) ? "Password " : "") +
                    (!string.IsNullOrEmpty(u.CertificateThumbprint) ? "Cert" : "");

                item.SubItems.Add(auth.Trim());

                item.Tag = u.UserName;

                if (!u.IsEnabled)
                    item.ForeColor = Color.Gray;

                lv.Items.Add(item);
            }

            lv.EndUpdate();
        }

        private OpcUaUserRecord FindSelected()
        {
            if (lv.SelectedItems.Count == 0)
                return null;

            string user = lv.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(user))
                return null;

            return _users.FirstOrDefault(x => string.Equals(x.UserName, user, StringComparison.OrdinalIgnoreCase));
        }

        private void AddUser()
        {
            var dlg = new FormOpcUaUserEdit(null);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            var rec = dlg.Result;

            if (_users.Any(x => string.Equals(x.UserName, rec.UserName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("UserName already exists.");
                return;
            }

            if (!string.IsNullOrEmpty(dlg.NewPassword))
                OpcUaUserStore.SetPassword(rec, dlg.NewPassword);

            _users.Add(rec);
            RefreshList();
        }

        private void EditUser()
        {
            var selected = FindSelected();
            if (selected == null)
                return;

            var dlg = new FormOpcUaUserEdit(selected);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            var edited = dlg.Result;

            // ===== Basic fields =====
            selected.UserName = edited.UserName;
            selected.DisplayName = edited.DisplayName;
            selected.Description = edited.Description;
            selected.IsEnabled = edited.IsEnabled;

            // ===== Certificate =====
            selected.CertificateThumbprint = edited.CertificateThumbprint;
            selected.CertificateSubject = edited.CertificateSubject;

            // ===== Permissions =====
            selected.Permissions = edited.Permissions;

            // ===== Password =====
            if (!string.IsNullOrEmpty(dlg.NewPassword))
            {
                OpcUaUserStore.SetPassword(selected, dlg.NewPassword);
            }
            else
            {
                // Preserve existing password hash (already copied by FormOpcUaUserEdit)
                selected.SaltBase64 = edited.SaltBase64;
                selected.HashBase64 = edited.HashBase64;
                selected.Iterations = edited.Iterations;
            }

            RefreshList();
        }

        private void DeleteUser()
        {
            var selected = FindSelected();
            if (selected == null)
                return;

            var ok = MessageBox.Show(
                "Delete this user?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (ok != DialogResult.Yes)
                return;

            _users.RemoveAll(x => string.Equals(x.UserName, selected.UserName, StringComparison.OrdinalIgnoreCase));
            RefreshList();
        }

        private void SaveUsers()
        {
            try
            {
                _store.Save(_users);
                UsersSaved?.Invoke(this, new OpcUaUsersChangedEventArgs(_users));

                MessageBox.Show("Saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error");
            }
        }
    }
    public sealed class OpcUaUsersChangedEventArgs : EventArgs
    {
        public List<OpcUaUserRecord> Users { get; private set; }
        public OpcUaUsersChangedEventArgs(List<OpcUaUserRecord> users)
        {
            Users = users;
        }
    }

}
