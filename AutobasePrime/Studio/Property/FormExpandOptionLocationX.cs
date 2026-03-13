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
    public partial class FormExpandOptionLocationX : Form
    {
        public FormExpandOptionLocationX()
        {
            InitializeComponent();
        }

        public void SetExpand(object obj)
        {
            if (obj == null) return;

            GraphicModule.ExpandBasicConversion basic = (GraphicModule.ExpandBasicConversion)obj;

            this.textBoxTag.Text = basic.sTag;
            this.textBoxSourceMin.Text = basic.fBasicSourceMin.ToString();
            this.textBoxSourceMax.Text = basic.fBasicSourceMax.ToString();
            this.textBoxTargetMin.Text = basic.fBasicTargetMin.ToString();
            this.textBoxTargetMax.Text = basic.fBasicTargetMax.ToString();

            this.radioButtonDirection0.Checked = (basic.nBasicConversionType == 0);
            this.radioButtonDirection1.Checked = (basic.nBasicConversionType == 1);
            this.radioButtonDirection2.Checked = (basic.nBasicConversionType == 2);
        }

        public object GetExpand()
        {
            GraphicModule.ExpandBasicConversion basic = new GraphicModule.ExpandBasicConversion();

            basic.sTag = this.textBoxTag.Text;
            basic.fBasicSourceMin = ConvertTool.ToSingle(this.textBoxSourceMin.Text);
            basic.fBasicSourceMax = ConvertTool.ToSingle(this.textBoxSourceMax.Text);
            basic.fBasicTargetMin = ConvertTool.ToSingle(this.textBoxTargetMin.Text);
            basic.fBasicTargetMax = ConvertTool.ToSingle(this.textBoxTargetMax.Text);

            if (this.radioButtonDirection0.Checked) basic.nBasicConversionType = 0;
            else if (this.radioButtonDirection1.Checked) basic.nBasicConversionType = 1;
            else if (this.radioButtonDirection2.Checked) basic.nBasicConversionType = 2;
            else  basic.nBasicConversionType = 0;

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
