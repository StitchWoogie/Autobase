using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;

namespace AutoLib
{
    public class ConfigWebView
    {
        public static bool bCheckProjectEveryConnection;

        static ConfigWebView()
        {
            Load();
        }

        static void Load()
        {
            bCheckProjectEveryConnection = TotalConfig.LoadRegAutoBaseConfig("WebView", "Config", "bCheckProjectEveryConnection", true);
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("WebView", "Config", "bCheckProjectEveryConnection", bCheckProjectEveryConnection);
        }
    }
}
