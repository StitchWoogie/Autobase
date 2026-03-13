using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalMain.SystemInfo
{
    public partial class FormModify : Form
    {
        public FormModify()
        {
            InitializeComponent();
        }

        public void Get(out string min, out string max)
        {
            min = this.textBoxLowAlarm.Text;
            max = this.textBoxHighAlarm.Text;
        }

        public void Set(string min, string max)
        {
            this.textBoxLowAlarm.Text = min;
            this.textBoxHighAlarm.Text = max;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
