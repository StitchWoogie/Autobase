using NetTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Studio.Script
{
    public partial class FormScriptFind : Form
    {
        public string FindText { get; private set; }
        public bool MatchCase { get; private set; }
        public bool FindNext { get; private set; }
        public bool FindPrevious { get; private set; }

        private TextBox textBoxFind;
        private CheckBox checkBoxMatchCase;
        private Button buttonFindNext;
        private Button buttonFindPrevious;
        private Button buttonClose;
        private Label labelFind;
        private FormScriptEditorSimpleNew parentForm;


        public FormScriptFind(FormScriptEditorSimpleNew parent)
        {
            parentForm = parent;
            InitializeComponent();
            SetupLocalizedText();
        }

        private void SetupLocalizedText()
        {
            if (Tools.IsLangKorean())
            {
                this.Text = "찾기";
                this.labelFind.Text = "찾을 내용:";
                this.checkBoxMatchCase.Text = "대/소문자 구분";
                this.buttonFindNext.Text = "다음 찾기";
                this.buttonFindPrevious.Text = "이전 찾기";
                this.buttonClose.Text = "닫기";
            }
            else if (Tools.IsLangChinese())
            {
                this.Text = "查找";
                this.labelFind.Text = "查找内容:";
                this.checkBoxMatchCase.Text = "区分大小写";
                this.buttonFindNext.Text = "查找下一个";
                this.buttonFindPrevious.Text = "查找上一个";
                this.buttonClose.Text = "关闭";
            }
            else
            {
                this.Text = "Find";
                this.labelFind.Text = "Find what:";
                this.checkBoxMatchCase.Text = "Match case";
                this.buttonFindNext.Text = "Find Next";
                this.buttonFindPrevious.Text = "Find Previous";
                this.buttonClose.Text = "Close";
            }
        }

        private void TextBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (e.Shift)
                    ButtonFindPrevious_Click(sender, e);
                else
                    ButtonFindNext_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Hide(); // Close() 대신 Hide() 사용
            }
        }

        private void ButtonFindNext_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFind.Text))
                return;

            // 직접 부모 폼의 FindNext 호출
            parentForm?.FindNext(textBoxFind.Text, checkBoxMatchCase.Checked);
        }

        private void ButtonFindPrevious_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFind.Text))
                return;


            // 직접 부모 폼의 FindPrevious 호출
            parentForm?.FindPrevious(textBoxFind.Text, checkBoxMatchCase.Checked);
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Hide(); // Close() 대신 Hide() 사용
        }

        public void SetFindText(string text)
        {
            textBoxFind.Text = text;
            textBoxFind.SelectAll();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            textBoxFind.Focus();
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            
            {
                e.Handled = true;

                // 빈 텍스트면 무시
                if (string.IsNullOrEmpty(textBoxFind.Text))
                    return;

                if (e.Shift)
                    ButtonFindPrevious_Click(sender, e);
                else
                    ButtonFindNext_Click(sender, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

    }
}
