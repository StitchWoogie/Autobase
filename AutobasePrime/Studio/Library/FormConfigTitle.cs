using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace Studio.Library
{
    public partial class FormConfigTitle : Form
    {
        public FormConfigTitle()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void Set(string title)
        {
            this.textBoxTitle.Text = title;
        }

        public string Get()
        {
            return this.textBoxTitle.Text.Trim();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.textBoxTitle.Text = this.textBoxTitle.Text.Trim();

            if (textBoxTitle.Text.Length == 0)
            {
                MessageBox.Show("제목을 입력하세요", "입력오류");
                return;
            }

            if (!NetTools.Tools.IsAbleFilename(this.textBoxTitle.Text))
            {
                MessageBox.Show("제목으로 적당한 형식이 아닙니다.", this.textBoxTitle.Text);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
