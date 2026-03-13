using System;
using System.Collections.Generic;
using System.Text;
using AutoLibLocal;

namespace ExcelReportData
{
    class GlobalVar
    {
        public static string GetVarValue(string varname)
        {
            if (ConfigVarTotal.bRunByWebService)
            {
                string s;
                if (GetGlobalVarFromSession(varname, out s))
                {
                    return s;
                }
                return "";
            }

            return AutoLibLocal.TotalConfig.LoadRegAutoBaseConfig("GlobalVars", null, varname, "");
        }

        static bool GetGlobalVarFromStringArray(string name, out string val)
        {
            name = "GlobalVar-" + name;

            if (ConfigVarTotal.varKeys == null)
            {
                val = "";
                return false;
            }

            for (int i = 0; i < ConfigVarTotal.varKeys.Length; i++)
            {
                // 로컬 레지스트리나 세션은 대소문자 구분이 없다.
                if (String.Compare(ConfigVarTotal.varKeys[i], name, true) == 0)
                {
                    val = ConfigVarTotal.varValues[i];
                    return true;
                }
            }

            val = "";
            return false;
        }

        static bool GetGlobalVarFromSession(string name, out string val)
        {
            if (ConfigVarTotal.varKeys != null)
            {
                return GetGlobalVarFromStringArray(name, out val);
            }

            val = "";

            object obj = ConfigVarTotal.GetFromSession("GlobalVar-" + name);

            if (obj == null)
            {
                return false;
            }

            val = Convert.ToString(obj);

            return true;
        }
    }
}
