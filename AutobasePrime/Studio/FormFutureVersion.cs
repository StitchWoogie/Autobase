using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal.KeyLock;
using AutoLibLocal;
using System.IO;

namespace Studio
{
    public partial class FormFutureVersion : Form
    {
        public string sSelectedText;

        public FormFutureVersion()
        {
            InitializeComponent();
        }

        public static string FutureVersionGo()
        {
            //if (!BetaTest.IsNextVersionBetaTester()) return null;

            FormFutureVersion dialog = new FormFutureVersion();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.sSelectedText;
            }

            return null;
        }

        private void FormFutureVersion_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            sSelectedText = button1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
