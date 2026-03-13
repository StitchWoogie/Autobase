using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExcelReportPublic
{
    public partial class PropertyEtcStCurr : Form
    {
        public PropertyEtcStCurr()
        {
            InitializeComponent();
        }

        public void Set(string var)
        {
            this.textBoxTag.Text = var;
        }

        public void Get(out string var)
        {
            var = this.textBoxTag.Text;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            DialogTag.SelectTag dialog = new DialogTag.SelectTag();

            dialog.bUseTagST = true;

            if (dialog.Run(this) == DialogResult.OK)
            {
                this.textBoxTag.Text = dialog.sTag;
            }
        }
    }
}