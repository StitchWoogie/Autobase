using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace Studio.Layer
{
    public partial class FormConfigLayer : Form
    {
        public FormConfigLayer()
        {
            InitializeComponent();
        }

        public void Set(GraphicModule.ObjectLayer layer)
        {
            this.textBoxTitle.Text = layer.objGeneral.sOnStudioTitle;
            this.buttonColor.BackColor = layer.LayerColor;
        }

        public void Get(GraphicModule.ObjectLayer layer)
        {
            layer.objGeneral.sOnStudioTitle = this.textBoxTitle.Text.Trim();
            layer.LayerColor = this.buttonColor.BackColor;

            if (layer.objGeneral.sOnStudioTitle.Length == 0)
            {
                //layer.sOnStudioTitle = "Layer";
            }
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();

            dialog.Color = this.buttonColor.BackColor;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColor.BackColor = dialog.Color;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
           
            DialogResult = DialogResult.OK;

            Close();
        }

        private void FormConfigLayer_Load(object sender, EventArgs e)
        {
            this.textBoxTitle.Select();
        }
    }
}
