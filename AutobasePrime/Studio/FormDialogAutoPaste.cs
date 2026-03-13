using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;

namespace Studio
{
    public partial class FormDialogAutoPaste : Form
    {
        public int nObjheight = 0;
        public int nObjwidth = 0;
        public int nModulewidth = 0;
        public int nModuleheight = 0;

        public int nColumnCount = 1;
        public int nColumnspace = 0;
        public int nRowCount = 1;
        public int nRowspace = 0;

        private int nTotalobjwidth = 0;
        private int nTotalobjheight = 0;

        public FormDialogAutoPaste()
        {
            InitializeComponent();

            numericUpDownColumn.Select();
            TotalCalc();

        }



        private void buttonOk_Click(object sender, EventArgs e)
        {
            OK();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        public void SetValue(int objw, int objh, int modulew, int moduleh)
        {
            nObjwidth = objw;
            nObjheight = objh;
            nModulewidth = modulew;
            nModuleheight = moduleh;

            TotalCalc();

        }

        private void TotalCalc()
        {
            int nBuf = 0;
            nColumnCount = ConvertTool.ToInt32(this.numericUpDownColumn.Value);
            nColumnspace = ConvertTool.ToInt32(this.numericUpDownColumnSpace.Value);
            nRowCount = ConvertTool.ToInt32(this.numericUpDownRow.Value);
            nRowspace = ConvertTool.ToInt32(this.numericUpDownRowSpace.Value);

            nBuf = (nObjwidth + nColumnspace) * (nColumnCount - 1) + nObjwidth;
            this.labelTotalWidth.Text = nBuf.ToString();
            nTotalobjwidth = nBuf;

            nBuf = (nObjheight + nRowspace) * (nRowCount - 1) + nObjheight;
            this.labelTotalHeight.Text = nBuf.ToString();
            nTotalobjheight = nBuf;

            this.labelModuleWidth.Text = nModulewidth.ToString();
            this.labelModuleHeight.Text = nModuleheight.ToString();

        }

        private void OK()
        {
            TotalCalc();

            if (nTotalobjwidth > nModulewidth)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("붙여넣으려는 가로크기가 모듈의 가로크기보다 큽니다..", "가로 크기 오류");
                else MessageBox.Show("You can't paste bigger width size than the module width size.", "Width size Error");

                return;

            }
            if (nTotalobjheight > nModuleheight)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("붙여넣으려는 세로크기가 모듈의 세로크기보다 큽니다..", "세로 크기 오류");
                else MessageBox.Show("You can't paste bigger height size than the module height size.", "Height size Error");

                return;

            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void numericUpDownColumn_ValueChanged(object sender, EventArgs e)
        {
            TotalCalc();
        }

        private void numericUpDownColumnSpace_ValueChanged(object sender, EventArgs e)
        {
            TotalCalc();
        }

        private void numericUpDownRow_ValueChanged(object sender, EventArgs e)
        {
            TotalCalc();
        }

        private void numericUpDownRowSpace_ValueChanged(object sender, EventArgs e)
        {
            TotalCalc();
        }

        private void FormDialogAutoPaste_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) this.OK();
        }

        private void numericUpDownColumn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) this.OK();
        }

        private void numericUpDownColumnSpace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) this.OK();
        }

        private void numericUpDownRow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) this.OK();
        }

        private void numericUpDownRowSpace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();

            if (e.KeyCode == Keys.Enter) this.OK();
        }

        private void buttonOk_Click_1(object sender, EventArgs e)
        {
            OK();
        }
    }


}