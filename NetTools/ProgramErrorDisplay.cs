using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetTools
{
    public class ProgramErrorDisplay
    {
        public static void Show(string text)
        {
            MessageBox.Show(text);
        }

        public static void Show(string text, string caption)
        {
            MessageBox.Show(text, caption);
        }
    }
}
