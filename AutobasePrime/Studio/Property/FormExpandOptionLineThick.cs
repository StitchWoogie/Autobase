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
    public partial class FormExpandOptionLineThick : Form
    {
        public FormExpandOptionLineThick()
        {
            InitializeComponent();
        }

        public void SetExpand(object obj)
        {
            if (obj == null) return;

            GraphicModule.ExpandBasicOnlyTag basic = (GraphicModule.ExpandBasicOnlyTag)obj;

            this.textBoxTag.Text = basic.sTag;
        }

        public object GetExpand()
        {
            GraphicModule.ExpandBasicOnlyTag basic = new GraphicModule.ExpandBasicOnlyTag();

            basic.sTag = this.textBoxTag.Text;

            return basic;
        }

        private void buttonTag_Click(object sender, EventArgs e)
        {
            string tag;
            string des;

            if (DialogTag.SelectTag.SelectAiDi(this, out tag, out des) == DialogResult.OK)
            {
                this.textBoxTag.Text = tag;
            }
        }
    }
}
