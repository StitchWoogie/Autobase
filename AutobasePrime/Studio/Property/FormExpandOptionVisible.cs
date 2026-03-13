using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace Studio
{
    public partial class FormExpandOptionVisible : Form
    {
        public FormExpandOptionVisible()
        {
            InitializeComponent();

            PropertyPageObjectTagAnimation.TagConditionPlayFillToCombo(this.comboBoxCondition);
        }

        public void SetExpand(object obj)
        {
            if (obj == null) return;

            GraphicModule.ExpandBasicVisible basic = (GraphicModule.ExpandBasicVisible)obj;

            // 저장은 CommaTextWriter를 사용하고 불러오기는 TextReader를 사용하면 저장시 " 를 저장하면 계속 """가 붙어서 엄청나게 커진다.
            if (basic.sTag.Length < 0 || basic.sTag.Length > 1000)
            {
                
            }

            this.textBoxTag.Text = basic.sTag;
            this.textBoxValue.Text = basic.value;
            this.comboBoxCondition.SelectedIndex = basic.condition;
        }

        public object GetExpand()
        {
            GraphicModule.ExpandBasicVisible basic = new GraphicModule.ExpandBasicVisible();

            basic.sTag = this.textBoxTag.Text;
            basic.value = this.textBoxValue.Text;
            basic.condition = this.comboBoxCondition.SelectedIndex;

            return basic;
        }

        private void buttonTag_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (DialogTag.SelectTag.SelectAiDiSt(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxTag.Text = tag;
            }
        }

        void EnableDisableValue()
        {
            bool flag_value;


            if (this.comboBoxCondition.SelectedIndex >= 0 && this.comboBoxCondition.SelectedIndex <= 4)
            {
                flag_value = true;
            }
            else if (this.comboBoxCondition.SelectedIndex == 12)
                flag_value = true;
            else
                flag_value = false;

            this.textBoxValue.Enabled = flag_value;
        }

        private void comboBoxCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisableValue();
        }
    }
}
