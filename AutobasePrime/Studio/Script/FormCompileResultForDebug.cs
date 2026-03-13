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
    public partial class FormCompileResultForDebug : Form
    {
        public FormCompileResultForDebug()
        {
            InitializeComponent();
        }

        public void Set(string text)
        {
            this.textBox1.Text = text;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormCompileResultForDebug_Load(object sender, EventArgs e)
        {
            this.buttonClose.Select();
        }
    }
}
