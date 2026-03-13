using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools.Cryptography;

namespace Studio.Communication
{
    public partial class FormConfigCryptography : Form
    {
        public FormConfigCryptography()
        {
            InitializeComponent();
        }

        private void FormConfigCryptography_Load(object sender, EventArgs e)
        {
            
        }

        public void Set(CryptoCommunication cc)
        {
            this.userControlConfigCryptography1.Set(cc);
        }

        public void Get(CryptoCommunication cc)
        {
            this.userControlConfigCryptography1.GetWithMessage(cc);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            CryptoCommunication cc = new CryptoCommunication();

            if (!this.userControlConfigCryptography1.GetWithMessage(cc)) return;

            DialogResult = DialogResult.OK;

            Close();
        }

        
    }
}
