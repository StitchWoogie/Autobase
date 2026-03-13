using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AniEditLib
{
    public partial class FormProperty : Form
    {
        public FormProperty()
        {
            InitializeComponent();
        }

        public int nWidth;
        public int nHeight;
        public int nFrame;
        public int nRpm;
        public int nColor;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            nWidth = (int)this.numericUpDownWidth.Value;
            nHeight = (int)this.numericUpDownHeight.Value;
            //nFrame = (int)this.numericUpDownFrame.Value;
            nRpm = (int)this.numericUpDownRpm.Value;
            
            int index = this.comboBoxColor.SelectedIndex;

            if (index == 0) nColor = 2;
            else if (index == 1) nColor = 4;
            else if (index == 2) nColor = 8;
            else if (index == 3) nColor = 24;
            else if (index == 4) nColor = 32;
            else
            {
                nColor = 24;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void FormNewAnimation_Load(object sender, EventArgs e)
        {
            this.comboBoxColor.Items.Add("2 Color");
            this.comboBoxColor.Items.Add("16 Color");
            this.comboBoxColor.Items.Add("256 Color");
            this.comboBoxColor.Items.Add("24 bit Color");
            this.comboBoxColor.Items.Add("32 bit Color");

            int index = 4;
            if (nColor == 2) index = 0;
            else if (nColor == 4) index = 1;
            else if (nColor == 8) index = 2;
            else if (nColor == 24) index = 3;
            else if (nColor == 32) index = 4;

            this.comboBoxColor.SelectedIndex = index;

            this.numericUpDownWidth.Value = nWidth;
            this.numericUpDownHeight.Value = nHeight;
            this.numericUpDownFrame.Value = nFrame;
            this.numericUpDownRpm.Value = nRpm;

            UpdateFrameSec();
        }

        void UpdateFrameSec()
        {
            double fps = 60.0 / ((int)numericUpDownFrame.Value * (int)numericUpDownRpm.Value);

            this.textBoxFrameTime.Text = fps.ToString("0.00");
        }

        private void numericUpDownFrame_ValueChanged(object sender, EventArgs e)
        {
            UpdateFrameSec();
        }

        private void numericUpDownRpm_ValueChanged(object sender, EventArgs e)
        {
            UpdateFrameSec();
        }
    }
}
