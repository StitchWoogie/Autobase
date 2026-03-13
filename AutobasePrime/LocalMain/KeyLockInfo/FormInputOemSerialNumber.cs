using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalMain.KeyLockInfo
{
    public partial class FormInputOemSerialNumber : Form
    {
        public FormInputOemSerialNumber()
        {
            InitializeComponent();
        }

        string sSerialNumber;

        public string Get()
        {
            return sSerialNumber;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            sSerialNumber = this.textBox1.Text.Trim();

            if (sSerialNumber.Length != 14)
            {
                MessageBox.Show("The SerialNumber must be 14 characters.");
                return;
            }
            
            DialogResult = DialogResult.OK;
        }
    }
}
