using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using AutoLibLocal;

namespace SMS.SmsFunc
{
    // CE를 위해서 추가로 환경을 저장해 준다.
    class SmsConfigCe
    {
        static string GetIniFilename()
        {
            string path = String.Format("{0}\\Sms\\SmsConfigCe.inix", TotalConfig.sDirWorkProject);
            return path;
        }

        public static void SaveConfig(string section, string item, string value)
        {
            Profile.WritePrivateProfileStringW(section, item, value, GetIniFilename());
        }
    }
}
