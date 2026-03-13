using System;
using System.Collections.Generic;
using System.Text;

namespace UsernameLibrary
{
    public class Config
    {
        public static string sUserName;
        public static byte[] aPassCode = new byte[1];
        public static bool bSaveUsername = false;
        public static bool bSavePassword = false;
        public static bool bAutoLogin = false;

        static Config()
        {
            //
            // TODO: Add constructor logic here
            //
            LoadConfig();
        }

        public static void LoadConfig()
        {
            sUserName = TotalConfig.LoadRegConfig("Username", "Config", "Username", "");
            aPassCode = TotalConfig.LoadRegConfig("Username", "Config", "PassCode", aPassCode);

            bSaveUsername = TotalConfig.LoadRegConfig("Username", "Config", "bSaveUsername", false);
            bSavePassword = TotalConfig.LoadRegConfig("Username", "Config", "bSavePassword", false);
            bAutoLogin = TotalConfig.LoadRegConfig("Username", "Config", "bAutoLogin", false);
        }

        public static void SaveConfig()
        {
            TotalConfig.SaveRegConfig("Username", "Config", "Username", sUserName);
            TotalConfig.SaveRegConfig("Username", "Config", "PassCode", aPassCode);

            TotalConfig.SaveRegConfig("Username", "Config", "bSaveUsername", bSaveUsername);
            TotalConfig.SaveRegConfig("Username", "Config", "bSavePassword", bSavePassword);
            TotalConfig.SaveRegConfig("Username", "Config", "bAutoLogin", bAutoLogin);
        }
    }
}
