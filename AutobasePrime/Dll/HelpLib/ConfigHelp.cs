using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;

namespace HelpLib
{
    class ConfigHelp
    {
        public static string sLanguage;

        // static으로 선언해야 이 클래스가 최초호출될때 생성자가 자동호출된다.
        static ConfigHelp()
        {
            Load();
        }

        static void Load()
        {
            sLanguage = TotalConfig.LoadRegAutoBaseConfig("Help", "Config", "Language", "Auto");
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("Help", "Config", "Language", sLanguage);
        }
    }
}
