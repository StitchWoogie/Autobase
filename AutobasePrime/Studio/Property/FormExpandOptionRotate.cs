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
    public partial class FormExpandOptionRotate : Form
    {
        public FormExpandOptionRotate()
        {
            InitializeComponent();
        }

        public void SetExpand(object obj)
        {
            if (obj == null) return;

            GraphicModule.ExpandBasicRotate basic = (GraphicModule.ExpandBasicRotate)obj;

            this.textBoxTag.Text = basic.sTag;
            this.textBoxSourceMin.Text = basic.fBasicSourceMin.ToString();
            this.textBoxSourceMax.Text = basic.fBasicSourceMax.ToString();
            this.textBoxTargetMin.Text = basic.fBasicTargetMin.ToString();
            this.textBoxTargetMax.Text = basic.fBasicTargetMax.ToString();

            this.radioButtonDirection0.Checked = (basic.nDirection == 0);
            this.radioButtonDirection1.Checked = (basic.nDirection == 1);

            this.radioButtonAxis0.Checked = (basic.nRotateAxis == 0);
            this.radioButtonAxis1.Checked = (basic.nRotateAxis == 1);
            this.radioButtonAxis2.Checked = (basic.nRotateAxis == 2);
            this.radioButtonAxis3.Checked = (basic.nRotateAxis == 3);
            this.radioButtonAxis4.Checked = (basic.nRotateAxis == 4);
        }

        public object GetExpand()
        {
            GraphicModule.ExpandBasicRotate basic = new GraphicModule.ExpandBasicRotate();

            basic.sTag = this.textBoxTag.Text;
            basic.fBasicSourceMin = ConvertTool.ToSingle(this.textBoxSourceMin.Text);
            basic.fBasicSourceMax = ConvertTool.ToSingle(this.textBoxSourceMax.Text);
            basic.fBasicTargetMin = ConvertTool.ToSingle(this.textBoxTargetMin.Text);
            basic.fBasicTargetMax = ConvertTool.ToSingle(this.textBoxTargetMax.Text);

            if (this.radioButtonDirection0.Checked) basic.nDirection = 0;
            else if (this.radioButtonDirection1.Checked) basic.nDirection = 1;
            else basic.nDirection = 0;

            if (this.radioButtonAxis0.Checked) basic.nRotateAxis = 0;
            else if (this.radioButtonAxis1.Checked) basic.nRotateAxis = 1;
            else if (this.radioButtonAxis2.Checked) basic.nRotateAxis = 2;
            else if (this.radioButtonAxis3.Checked) basic.nRotateAxis = 3;
            else if (this.radioButtonAxis4.Checked) basic.nRotateAxis = 4;
            else basic.nRotateAxis = 0;

            return basic;
        }

        private void buttonTag_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (DialogTag.SelectTag.SelectAi(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxTag.Text = tag;
            }
        }
    }
}
