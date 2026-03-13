using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetTools
{
    public class TextBoxTool
    {
        public static bool CheckTextBoxLimitOver(TextBox tb, int limit)
        {
            int size = tb.Text.Length;

            if (size > limit)
            {
                string msg;
                if (Tools.IsLangKorean())
                {
                    tb.Select(0, size);
                    tb.Focus();

                    msg = String.Format("최대 입력할 수 있는 글자수는 {0} 개입니다.\n현재 {1}글자", limit, size);
                    MessageBox.Show(msg, "입력 글자수 초과");

                    return true;
                }
                else
                {
                    tb.Focus();
                    tb.Select();

                    msg = String.Format("Text limit length = {0}.\ncurrent = {1}", limit, size);
                    MessageBox.Show(msg, "TextBox limit length");

                    return true;
                }
            }

            return false;
        }
    }
}
