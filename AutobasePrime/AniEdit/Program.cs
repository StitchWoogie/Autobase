using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AniStudio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AutoLibLocal.LanguageTool.ChangeUICulture();
            
            Application.Run(new AniEditLib.FormAniEditMain());
        }
    }
}
