using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ExcelReportData
{
    public partial class PropertyEtcGlobalGetVar : Form
    {
        public PropertyEtcGlobalGetVar()
        {
            InitializeComponent();
        }

        public void Set(string var)
        {
            this.textBoxVarName.Text = var;
        }

        public void Get(out string var)
        {
            var = this.textBoxVarName.Text;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}