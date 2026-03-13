using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WatchDog
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            this.checkBoxHideOnStartup.Checked = WatchDogConfig.bHideOnStartUp;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            WatchDogConfig.bHideOnStartUp = this.checkBoxHideOnStartup.Checked;
            WatchDogConfig.Save();

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
