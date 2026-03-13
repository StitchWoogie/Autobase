using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ViewMain
{
    public partial class FormSelectSiteAdd : Form
    {
        public FormSelectSiteAdd()
        {
            InitializeComponent();
        }

        private void FormSelectSiteAdd_Load(object sender, EventArgs e)
        {

        }

        public void Set(SiteItem si)
        {
            this.textBoxTitle.Text = si.sSiteName;
            this.textBoxURL.Text = si.sSiteUrl;
        }

        public void Get(SiteItem si)
        {
            si.sSiteName = this.textBoxTitle.Text;
            si.sSiteUrl = this.textBoxURL.Text;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxTitle.Text.Length == 0)
            {
                MessageBox.Show("Input the Site Name.", "Site Name required");
                return;
            }

            if (this.textBoxURL.Text.Length == 0)
            {
                MessageBox.Show("Input the Site URL.", "Site URL required");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
