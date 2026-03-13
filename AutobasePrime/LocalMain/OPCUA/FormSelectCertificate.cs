using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalMain.OPCUA
{
    public partial class FormSelectCertificate : Form
    {
        private ListView lv;
        private Button btnOk;
        private Button btnCancel;

        private readonly List<X509Certificate2> _certs;

        public X509Certificate2 SelectedCertificate { get; private set; }

        public FormSelectCertificate(List<X509Certificate2> certificates)
        {
            if (certificates == null)
                throw new ArgumentNullException(nameof(certificates));

            _certs = certificates;

            InitializeComponent();
            InitializeUI();
            PopulateList();
        }

        private void InitializeUI()
        {
            Text = "Select Trusted Certificate";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(820, 420);

            lv = new ListView
            {
                Left = 10,
                Top = 10,
                Width = 800,
                Height = 340,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false
            };

            lv.Columns.Add("Subject", 300);
            lv.Columns.Add("Thumbprint", 260);
            lv.Columns.Add("Valid From", 120);
            lv.Columns.Add("Valid To", 120);

            lv.DoubleClick += (s, e) => AcceptSelection();

            btnOk = new Button
            {
                Text = "OK",
                Left = 610,
                Top = 365,
                Width = 90
            };
            btnOk.Click += (s, e) => AcceptSelection();

            btnCancel = new Button
            {
                Text = "Cancel",
                Left = 720,
                Top = 365,
                Width = 90
            };
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

            Controls.Add(lv);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
        }

        private void PopulateList()
        {
            lv.BeginUpdate();
            lv.Items.Clear();

            foreach (var cert in _certs.OrderBy(c => c.Subject, StringComparer.OrdinalIgnoreCase))
            {
                var item = new ListViewItem(cert.Subject);
                item.SubItems.Add(cert.Thumbprint);
                item.SubItems.Add(cert.NotBefore.ToString("yyyy-MM-dd"));
                item.SubItems.Add(cert.NotAfter.ToString("yyyy-MM-dd"));
                item.Tag = cert;

                if (DateTime.Now < cert.NotBefore || DateTime.Now > cert.NotAfter)
                {
                    item.ForeColor = Color.Gray; // expired or not yet valid
                }

                lv.Items.Add(item);
            }

            lv.EndUpdate();
        }

        private void AcceptSelection()
        {
            if (lv.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "Please select a certificate.", "Select Certificate",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedCertificate = lv.SelectedItems[0].Tag as X509Certificate2;
            if (SelectedCertificate == null)
                return;

            DialogResult = DialogResult.OK;
        }
    }
}
