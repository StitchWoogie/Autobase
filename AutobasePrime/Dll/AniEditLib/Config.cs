using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using AutoLibLocal;
using System.Windows.Forms;

namespace AniEditLib
{
    public class Config
    {
        public static string sBitmapEditor;

        static Config()
        {
            Load();   
        }

        static void Load()
        {
            string filename = Application.StartupPath + "\\SU30.EXE";

            sBitmapEditor = TotalConfig.LoadRegAutoBaseConfig("AniStudio", null, "Bitmap Editor", filename);
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("AniStudio", null, "Bitmap Editor", sBitmapEditor);
        }
    }
}
