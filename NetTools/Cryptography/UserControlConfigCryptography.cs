using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetTools.Cryptography
{
    public partial class UserControlConfigCryptography : UserControl
    {
        public UserControlConfigCryptography()
        {
            InitializeComponent();
        }

        void EnableDisableEncryption()
        {
            bool flag = this.checkBoxUseEncryption.Checked;

            this.radioButtonEngine0.Enabled = flag;
            this.radioButtonEngine1.Enabled = flag;

            this.textBoxKey.Enabled = flag;
            this.textBoxIV.Enabled = flag;
            this.comboBoxCipherMode.Enabled = flag;
            this.comboBoxPaddingMode.Enabled = flag;
            this.comboBoxFrameMode.Enabled = flag;
        }

        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
            labelInfoKey.Text = String.Format("Current: {0} bits", textBoxKey.Text.Length * 4);
        }

        private void textBoxIV_TextChanged(object sender, EventArgs e)
        {
            labelInfoIV.Text = String.Format("Current: {0} bits", textBoxIV.Text.Length * 4);
        }

        private void checkBoxUseEncryption_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableEncryption();
        }

        public void Set(CryptoCommunication ec)
        {
            this.checkBoxUseEncryption.Checked = ec.bUseEncryption;
            this.textBoxKey.Text = ec.sKey;
            this.textBoxIV.Text = ec.sIV;
            this.comboBoxCipherMode.Text = ec.sCipherMode;
            this.comboBoxPaddingMode.Text = ec.sPaddingMode;

            this.radioButtonEngine0.Checked = (ec.sEngine == "AES");
            this.radioButtonEngine1.Checked = (ec.sEngine == "ARIA");

            if(ec.nFrameMode < this.comboBoxFrameMode.Items.Count)
                this.comboBoxFrameMode.SelectedIndex = ec.nFrameMode;

            EnableDisableEncryption();
        }

        public bool GetWithMessage(CryptoCommunication ec)
        {
            if (this.checkBoxUseEncryption.Checked)
            {
                string text = this.textBoxKey.Text.Trim();

                if (text.Length == 32 || text.Length == 48 || text.Length == 64)
                {
                }
                else
                {
                    MessageBox.Show("Key size must be 128/192/256 bits.", "Key Size Error");
                    return false;
                }

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] >= '0' && text[i] <= '9') continue;
                    if (text[i] >= 'a' && text[i] <= 'f') continue;
                    if (text[i] >= 'A' && text[i] <= 'F') continue;

                    MessageBox.Show("Key character must be 0~9 or a~f or A~F", "Key character error");
                    return false;
                }

                text = this.textBoxIV.Text.Trim();

                if (text.Length != 32)
                {
                    MessageBox.Show("IV size must be 128 bits.", "IV Size Error");
                    return false;
                }

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] >= '0' && text[i] <= '9') continue;
                    if (text[i] >= 'a' && text[i] <= 'f') continue;
                    if (text[i] >= 'A' && text[i] <= 'F') continue;

                    MessageBox.Show("IV character must be 0~9 or a~f or A~F", "IV character error");
                    return false;
                }

                if (this.radioButtonEngine0.Checked && this.comboBoxCipherMode.Text == "OFB")
                {
                    MessageBox.Show("CipherMode.OFB is not supported in the AES Engine.", "Not Supported");
                    return false;
                }
                else if (this.radioButtonEngine0.Checked && this.comboBoxCipherMode.Text == "CTS")
                {
                    MessageBox.Show("CipherMode.CTS is not supported in the AES Engine.", "Not Supported");
                    return false;
                }
            }

            ec.bUseEncryption = this.checkBoxUseEncryption.Checked;
            ec.sKey = this.textBoxKey.Text.Trim().ToUpper();
            ec.sIV = this.textBoxIV.Text.Trim().ToUpper();
            ec.sCipherMode = this.comboBoxCipherMode.Text;
            ec.sPaddingMode = this.comboBoxPaddingMode.Text;

            if (this.radioButtonEngine1.Checked)
                ec.sEngine = "ARIA";
            else
                ec.sEngine = "AES";

            ec.nFrameMode = this.comboBoxFrameMode.SelectedIndex;

            return true;
        }
    }
}
