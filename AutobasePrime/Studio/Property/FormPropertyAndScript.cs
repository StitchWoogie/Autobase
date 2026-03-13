using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GraphicModule;

namespace Studio
{
    public partial class FormPropertyAndScript : Form
    {
        Form formProperty;
        FormScriptEditorSimpleNew formScript;

        public FormPropertyAndScript(ExpandBasicAndScript expand, string default_description, Form form_property)
        {
            InitializeComponent();

            formScript = new FormScriptEditorSimpleNew();

            if (expand != null)
            {
                if (expand.script != null)
                {
                    formScript.SetScript(expand.script);
                }
                else
                {
                    formScript.SetDescription(default_description);
                }
            }

            formScript.TopLevel = false;
            formScript.FormBorderStyle = FormBorderStyle.None;
            formScript.Dock = DockStyle.Fill;
            formScript.bUseOkCancelButton = false;
            formScript.CompactMode = true;  //20260303 PSU 추가. 

            this.groupBoxScript.Controls.Add(formScript);
            //this.panelScript.Controls.Add(dialog);
            formScript.Show();

            formProperty = form_property;

            if (expand != null)
            {
                if (formProperty.GetType() == typeof(FormExpandOptionSizeWidth))
                {
                    ((FormExpandOptionSizeWidth)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionSizeHeight))
                {
                    ((FormExpandOptionSizeHeight)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionLocationX))
                {
                    ((FormExpandOptionLocationX)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionLocationY))
                {
                    ((FormExpandOptionLocationY)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionAnimationSpeed))
                {
                    ((FormExpandOptionAnimationSpeed)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionBlinking))
                {
                    ((FormExpandOptionBlinking)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionColor))
                {
                    ((FormExpandOptionColor)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionLineThick))
                {
                    ((FormExpandOptionLineThick)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionVisible))
                {
                    ((FormExpandOptionVisible)formProperty).SetExpand(expand.basic);
                }
                else if (formProperty.GetType() == typeof(FormExpandOptionRotate))
                {
                    ((FormExpandOptionRotate)formProperty).SetExpand(expand.basic);
                }
                else
                {
                    MessageBox.Show("FormPropertyAndScript.FormPropertyAndScript에서 알수없는 Type", formProperty.ToString());
                }
            }

            formProperty.TopLevel = false;
            formProperty.FormBorderStyle = FormBorderStyle.None;
            formProperty.Dock = DockStyle.Fill;

            this.groupBoxBasic.Controls.Add(formProperty);
            //this.panelScript.Controls.Add(dialog);
            formProperty.Show();

            if (expand != null)
            {
                if (expand.eExpandType == EnumExpandType.Basic)
                {
                    this.radioButtonPropertyType0.Checked = true;
                }
                else
                {
                    this.radioButtonPropertyType1.Checked = true;
                }

            }

            this.Text = form_property.Text;
        }

        void EnableDisableType()
        {
            bool flag_basic = false;
            bool flag_script = false;

            if (this.radioButtonPropertyType0.Checked) flag_basic = true;
            if (this.radioButtonPropertyType1.Checked) flag_script = true;

            this.groupBoxBasic.Enabled = flag_basic;
            this.groupBoxScript.Enabled = flag_script;
        }

        void EnableDisableMenu()
        {
            this.menuStrip1.Enabled = this.radioButtonPropertyType1.Checked;
        }

        private void FormPropertyAndScript_Load(object sender, EventArgs e)
        {
            EnableDisableType();
            EnableDisableMenu();
        }

        private void FormPropertyAndScript_SizeChanged(object sender, EventArgs e)
        {
            this.groupBoxScript.Width = this.ClientRectangle.Width - (this.groupBoxScript.Left * 2);
            this.groupBoxScript.Height = (this.ClientRectangle.Bottom - this.groupBoxScript.Top - this.groupBoxScript.Left);
        }

        private void radioButtonPropertyType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableType();
            EnableDisableMenu();
        }

        private void radioButtonPropertyType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableType();
            EnableDisableMenu();
        }

        public ExpandBasicAndScript expandBasicAndScript;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            expandBasicAndScript = new ExpandBasicAndScript();

            if (this.radioButtonPropertyType1.Checked) expandBasicAndScript.eExpandType = EnumExpandType.Script;
            else
            {
                expandBasicAndScript.eExpandType = EnumExpandType.Basic;
            }

            if (formProperty.GetType() == typeof(FormExpandOptionSizeWidth))
            {
                expandBasicAndScript.basic = ((FormExpandOptionSizeWidth)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionSizeHeight))
            {
                expandBasicAndScript.basic = ((FormExpandOptionSizeHeight)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionLocationX))
            {
                expandBasicAndScript.basic = ((FormExpandOptionLocationX)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionLocationY))
            {
                expandBasicAndScript.basic = ((FormExpandOptionLocationY)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionAnimationSpeed))
            {
                expandBasicAndScript.basic = ((FormExpandOptionAnimationSpeed)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionBlinking))
            {
                expandBasicAndScript.basic = ((FormExpandOptionBlinking)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionColor))
            {
                expandBasicAndScript.basic = ((FormExpandOptionColor)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionLineThick))
            {
                expandBasicAndScript.basic = ((FormExpandOptionLineThick)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionVisible))
            {
                expandBasicAndScript.basic = ((FormExpandOptionVisible)formProperty).GetExpand();
            }
            else if (formProperty.GetType() == typeof(FormExpandOptionRotate))
            {
                expandBasicAndScript.basic = ((FormExpandOptionRotate)formProperty).GetExpand();
            }
            else
            {
                MessageBox.Show("FormPropertyAndScript.buttonOK_Click에서 알수없는 Type", formProperty.ToString());
                expandBasicAndScript.basic = null;
            }


            expandBasicAndScript.script = new ScriptClass();
            formScript.DialogToScript(expandBasicAndScript.script);

            DialogResult = DialogResult.OK;

            Close();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formScript.MenuItemConfigFontGo();
        }

        private void scriptCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formScript.MenuItemScriptCopyGo();
        }

        private void scriptPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formScript.MenuItemScriptPasteGo();
        }

        private void insertRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formScript.MenuItemInsertRGBGo();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formScript.MenuItemHelpGo();
        }
    }
}
