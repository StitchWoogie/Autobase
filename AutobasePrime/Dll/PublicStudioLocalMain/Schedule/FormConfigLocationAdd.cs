using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;

namespace PublicStudioLocalMain.Schedule
{
    public partial class FormConfigLocationAdd : Form
    {
        public FormConfigLocationAdd()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxTitle, 30)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxLatitude, 30)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxLongitude, 30)) return;

            if (this.textBoxTitle.Text.Length == 0)
            {
                MessageBox.Show("Input the title.", "Input Error");
                return;
            }

            if (this.textBoxLatitude.Text.Length == 0)
            {
                MessageBox.Show("Input the Latitude.", "Input Error");
                return;
            }

            if (this.textBoxLongitude.Text.Length == 0)
            {
                MessageBox.Show("Input the Longitude.", "Input Error");
                return;
            }

            DialogResult = DialogResult.OK;

            Close();
        }

        public void GetStruct(LocationItem item)
        {
            item.sCity = this.textBoxTitle.Text;
            item.sLatitude = this.textBoxLatitude.Text;
            item.sLongitude = this.textBoxLongitude.Text;
        }

        public void SetStruct(LocationItem item)
        {
            this.textBoxTitle.Text = item.sCity;
            this.textBoxLatitude.Text = item.sLatitude;
            this.textBoxLongitude.Text = item.sLongitude;
        }
    }
}
