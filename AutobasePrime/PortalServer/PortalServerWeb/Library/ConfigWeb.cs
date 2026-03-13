using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using NetTools;

namespace PortalServerWeb.Library
{
    public class ConfigWeb
    {
        /*
    10.3.1.6 부터 제거됨
    public static bool IsUseWebConfig()
    {
        //if (String.Compare(UseWebConfig, "true", true) == 0) return true;
        //return false;
        bool flag = LoadConfigBool("UseWebConfig", false);

        return flag;
    }*/

        public static string GetLocalIP()
        {
            string ip;

            ip = ConfigurationManager.AppSettings["LocalMainIP"];

            return ip;
        }

        public static string GetLocalPort()
        {
            string port;

            port = ConfigurationManager.AppSettings["LocalMainPort"];

            return port;
        }

        static bool LoadConfig(string item, bool default_value)
        {
            string buf = ConfigurationManager.AppSettings[item];
            bool flag;

            if (buf == null || buf.Length == 0)
            {
                flag = default_value;
            }
            else
            {
                if (String.Compare(buf, "true", true) == 0)
                    flag = true;
                else
                    flag = false;
            }

            return flag;
        }

        static int LoadConfig(string item, int default_value)
        {
            string buf = ConfigurationManager.AppSettings[item];
            int value;

            if (buf == null || buf.Length == 0)
            {
                value = default_value;
            }
            else
            {
                value = ConvertTool.ToInt32(buf);
            }

            return value;
        }

        public static bool LocalVersionIs941OrHigher()
        {
            bool flag;

            flag = LoadConfig("LocalVersionIs941OrHigher", true);

            return flag;
        }

        public static bool LogOnUse()
        {
            bool flag;

            flag = LoadConfig("LogOnUse", false);

            return flag;
        }

        public static string LogOnUsername()
        {
            string data;

            data = ConfigurationManager.AppSettings["LogOnUsername"];

            return data;
        }

        public static string LogOnPassword()
        {
            string data;

            data = ConfigurationManager.AppSettings["LogOnPassword"];

            return data;
        }

        public static string LogOnDomain()
        {
            string data;

            data = ConfigurationManager.AppSettings["LogOnDomain"];

            return data;
        }

        public static string DataFolderBasic()
        {
            string data;

            data = ConfigurationManager.AppSettings["DataFolderBasic"];

            return data;
        }

        public static string DataFolderLog()
        {
            string data;

            data = ConfigurationManager.AppSettings["DataFolderLog"];

            return data;
        }

        public static bool UseLog()
        {
            return LoadConfig("UseLog", false);
        }

        public static string LogDsn()
        {
            return ConfigurationManager.AppSettings["LogDsn"];
        }

        public static int SecurityLevel()
        {
            return LoadConfig("SecurityLevel", 0);
        }

        // 보안레벨이 3미만이면 사용할 수 있는 함수 있다.
        // 보안레벨이 3이상인 웹서버에서는 사용할 수 없도록 해야하는 함수는 이것을 호출한다.
        public static bool CheckBelowSecurityLevel3(out string err_msg)
        {
            if (ConfigWeb.SecurityLevel() >= 3)
            {
                err_msg = String.Format("The SecurityLevel of the Webclient must be 3 or greater.");
                return false;
            }

            err_msg = "";

            return true;
        }

        public static string WebAdminUsername()
        {
            return ConfigurationManager.AppSettings["WebAdminUsername"];
        }

        public static string WebAdminPassword()
        {
            return ConfigurationManager.AppSettings["WebAdminPassword"];
        }
    }
}
