using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;

namespace AniEditLib
{
    public class AniEditOnStudio
    {
        public static FormAniEditMain formAniEditor = null;

        public static void Run(System.Windows.Forms.Form parent)
        {
            if (formAniEditor != null)
            {
                formAniEditor.Activate();
                return;
            }

            formAniEditor = new FormAniEditMain();
            formAniEditor.Owner = parent;
            formAniEditor.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            formAniEditor.Show(); //250828 PSU parent 제거
        }

        public static void Stop()
        {
            formAniEditor.Close();
        }

        public static void Insert(Bitmap bitmap)
        {
            formAniEditor.Insert(bitmap);
        }

    }
}
