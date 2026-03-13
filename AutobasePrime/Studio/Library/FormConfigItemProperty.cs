using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace Studio.Library
{
    public partial class FormConfigItemProperty : Form
    {
        public FormConfigItemProperty()
        {
            InitializeComponent();
        }

        public void Set(bool isweb, string keywords, int price, bool web_share, string comment)
        {
            this.textBoxKeywords.Text = keywords;

            this.checkBoxShareOnWeb.Checked = web_share;
            this.numericUpDownPrice.Value = price;
            this.textBoxComment.Text = comment;

            this.groupBoxWebLibrary.Enabled = isweb;
        }

        public void Get(out string keywords, out int price, out bool web_share, out string comment)
        {
            keywords = this.textBoxKeywords.Text;

            price = (int)this.numericUpDownPrice.Value;
            web_share = this.checkBoxShareOnWeb.Checked;
            comment = this.textBoxComment.Text;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
