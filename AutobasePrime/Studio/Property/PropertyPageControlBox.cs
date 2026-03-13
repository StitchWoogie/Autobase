using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using AutoLibLocal;
using NetTools;

namespace Studio
{
    public partial class PropertyPageControlBox : Form
    {
        public PropertyPageControlBox()
        {
            InitializeComponent();
        }

        public void SetProp(MOUSE_RESPONSE_STRUCT response)
        {
            this.checkBoxUseModule.Checked = (response.cUserControlBoxUse == 1);
            this.textBoxModuleName.Text = response.sUserControlBoxModuleFile;
            this.textBoxTitle.Text = response.sUserControlBoxTitle;
            this.textBoxDescription.Text = response.sUserControlBoxDescription;

            EnableDisable();
        }

        public void GetProp(MOUSE_RESPONSE_STRUCT response)
        {
            response.cUserControlBoxUse = this.checkBoxUseModule.Checked ? (sbyte)1 : (sbyte)0;
            response.sUserControlBoxModuleFile = this.textBoxModuleName.Text;
            response.sUserControlBoxTitle = this.textBoxTitle.Text;
            response.sUserControlBoxDescription = this.textBoxDescription.Text;
        }

        private void checkBoxUseModule_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        void EnableDisable()
        {
            bool flag = this.checkBoxUseModule.Checked;

            this.textBoxDescription.Enabled = flag;
            this.textBoxModuleName.Enabled = flag;
            this.textBoxTitle.Enabled = flag;
            this.buttonSearchModule.Enabled = flag;
            this.buttonSearchLibrary.Enabled = flag;
        }

        private void buttonSearchModule_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Module files (*.modx)|*.modx";

            dialog.InitialDirectory = TotalConfig.sDirWorkProject + "\\graphic";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string my_dir = TotalConfig.sDirWorkProject + "\\graphic";
                if (String.Compare(my_dir, 0, dialog.FileName, 0, my_dir.Length, true) != 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("프로젝트 폴더에 있는 모듈 파일만 사용할 수 있습니다.", "폴더 오류");
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("只可使用工程项目文件夹中的模块文件。", "文件夹错误");
                    else
                        MessageBox.Show("Use module file on project directory.", "Directory Error");

                    return;
                }

                this.textBoxModuleName.Text = dialog.FileName.Substring(my_dir.Length + 1);
            }
        }

        private void buttonSearchLibrary_Click(object sender, EventArgs e)
        {
            Library.FormSelectControlBoxFromLibrary dialog = new Studio.Library.FormSelectControlBoxFromLibrary();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string my_dir = TotalConfig.sDirWorkProject + "\\graphic";
                this.textBoxModuleName.Text = dialog.sFileName.Substring(my_dir.Length + 1);
            }
        }
    }
}