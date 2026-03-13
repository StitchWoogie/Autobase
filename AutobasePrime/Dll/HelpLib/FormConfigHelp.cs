using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;

namespace HelpLib
{
    public partial class FormConfigHelp : Form
    {
        public FormConfigHelp()
        {
            InitializeComponent();
        }

        string sOldLanguage;

        private void FormConfigHelp_Load(object sender, EventArgs e)
        {
            sOldLanguage = ConfigHelp.sLanguage;
            this.listBoxLanguage.SelectedItem = ConfigHelp.sLanguage;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ConfigHelp.sLanguage = (string)this.listBoxLanguage.SelectedItem;
            ConfigHelp.Save();

            if (sOldLanguage != ConfigHelp.sLanguage)
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("프로그램을 다시 시작할 때 변경한 도움말 언어가 적용됩니다.", "도움말 언어 변경");
                else
                    MessageBox.Show("When you restart the program, changed language applies.", "Help Language");
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            ConfigHelp.sLanguage = (string)this.listBoxLanguage.SelectedItem;
            ConfigHelp.Save();

            FormInstallHelp.GoUpdate();
        }
    }
}
