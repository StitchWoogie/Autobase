using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DialogTag.TagEditor
{
    public partial class FormSearch : Form
    {
        FormTagEditor formEditor;
        bool bReplace;
        public FormSearch(FormTagEditor form, bool replace)
        {
            formEditor = form;
            bReplace = replace;
            InitializeComponent();
        }

        void AddNewHistory(ComboBox combo, string text)
        {
            if (text.Length == 0) return;

            // 중복되는 항목은 없앤다.
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if ((string)combo.Items[i] == text)
                {
                    if (i == 0) return; // 교체할 이유가 없다.
                    combo.Items.RemoveAt(i);
                    break;
                }
            }

            combo.Items.Insert(0, text);
            combo.Text = text;
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            string text = this.comboBoxSearchText.Text;

            text = text.Trim();

            if (text.Length == 0)
            {
                MessageBox.Show("찾을 단어를 입력하세요.", "입력 오류");
                return;
            }

            AddNewHistory(this.comboBoxSearchText, text);
            formEditor.SearchFindNext(text, checkBoxMatchCase.Checked, checkBoxSearchAtAll.Checked);
        }

        

        private void FormSearch_Load(object sender, EventArgs e)
        {
            this.comboBoxReplaceText.Enabled = bReplace;
            this.buttonReplace.Enabled = bReplace;

            this.checkBoxSearchAtAll.Enabled = !bReplace;

            LoadHistoryText(this.comboBoxSearchText, "SearchText");
            LoadHistoryText(this.comboBoxReplaceText, "ReplaceText");

            this.comboBoxSearchText.Select();
        }

        private void comboBoxSearchText_TextChanged(object sender, EventArgs e)
        {
            /*
            if (!checkBoxSearchOnTextChanged.Checked) return;

            string text = this.comboBoxSearchText.Text;

            text = text.Trim();

            if (text.Length == 0)
            {
                return;
            }

            formEditor.SearchFindNext(text, checkBoxMatchCase.Checked, checkBoxSearchAtAll.Checked);
             */
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            string text = this.comboBoxSearchText.Text;

            text = text.Trim();

            if (text.Length == 0)
            {
                MessageBox.Show("바꿀 단어를 입력하세요.", "입력 오류");
                return;
            }

            AddNewHistory(this.comboBoxSearchText, text);
            AddNewHistory(this.comboBoxReplaceText, this.comboBoxReplaceText.Text);

            formEditor.SearchReplace(text, checkBoxMatchCase.Checked, checkBoxSearchAtAll.Checked, this.comboBoxReplaceText.Text);
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {

        
        }

        void LoadHistoryText(ComboBox combo, string item)
        {
            string buf = AutoLibLocal.TotalConfig.LoadRegAutoBaseConfig("TagEditor", "SearchReplace", item, "");

            NetTools.CommaBlockString comma = new NetTools.CommaBlockString();
            comma.Set(buf);

            string imsi = "";

            for (int i = 0; i < 20; i++)
            {
                if (comma.IsEOS()) break;
                comma.GetString(ref imsi);
                if (imsi.Length > 0)
                {
                    combo.Items.Add(imsi);
                }
            }
        }

        void SaveHistoryText(ComboBox combo, string item)
        {
            string buf = "";
            for (int i = 0; i < combo.Items.Count; i++)
            {
                buf += combo.Items[i]+",";
            }

            AutoLibLocal.TotalConfig.SaveRegAutoBaseConfig("TagEditor", "SearchReplace", item, buf);
        }

        private void FormSearch_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveHistoryText(this.comboBoxSearchText, "SearchText");
            SaveHistoryText(this.comboBoxSearchText, "ReplaceText");
        }
    }
}