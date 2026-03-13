using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExportServer
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            this.checkBoxHideOnStartup.Checked = ExportServerConfig.bHideOnStartUp;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ExportServerConfig.bHideOnStartUp = this.checkBoxHideOnStartup.Checked;
            ExportServerConfig.Save();

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
