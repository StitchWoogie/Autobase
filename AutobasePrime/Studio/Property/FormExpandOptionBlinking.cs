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
    public partial class FormExpandOptionBlinking : Form
    {
        public FormExpandOptionBlinking()
        {
            InitializeComponent();

            PropertyPageObjectTagAnimation.TagConditionPlayFillToCombo(this.comboBoxCondition);
        }

        public void SetExpand(object obj)
        {
            

            if (obj == null) return;

            GraphicModule.ExpandBasicBlinking basic = (GraphicModule.ExpandBasicBlinking)obj;

            this.textBoxTag.Text = basic.sTag;
            this.textBoxValue.Text = basic.value;
            this.comboBoxCondition.SelectedIndex = basic.condition;
            Tools.SetNumericUpDownValue(this.numericUpDownCycle, basic.cycle);
        }

        public object GetExpand()
        {
            GraphicModule.ExpandBasicBlinking basic = new GraphicModule.ExpandBasicBlinking();

            basic.sTag = this.textBoxTag.Text;
            basic.value = this.textBoxValue.Text;
            basic.condition = this.comboBoxCondition.SelectedIndex;
            basic.cycle = (int)this.numericUpDownCycle.Value;

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
