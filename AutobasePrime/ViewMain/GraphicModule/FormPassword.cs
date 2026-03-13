using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphicModule
{
    public partial class FormPassword : Form
    {
        string sPassword = "";

        public FormPassword(string password)
        {
            InitializeComponent();

            sPassword = password;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxPassword.Text != sPassword)
            {
                MessageBox.Show("Password mismatched.", "Password error");
                return;
            }

            DialogResult = DialogResult.OK;

            Close();
        }

        private void FormPassword_Load(object sender, EventArgs e)
        {
            this.textBoxPassword.Select();
        }
    }
}
