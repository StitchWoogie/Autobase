using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Studio.Script
{
    public partial class FormErrorDecompileCompare : Form
    {
        public FormErrorDecompileCompare()
        {
            InitializeComponent();
        }

        private void FormErrorDecompileCompare_Load(object sender, EventArgs e)
        {

        }

        public void Set(string d1, string d2)
        {
            this.userControlScriptEditor1.SetSourceString(d1);
            this.userControlScriptEditor2.SetSourceString(d2);
        }

    }
}
