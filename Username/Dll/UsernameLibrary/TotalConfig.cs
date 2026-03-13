using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using NetTools;

namespace UsernameLibrary
{
    public enum EnumDefineMode
    {
        MODE_RUN,
        MODE_EDIT,
    }
    /// <summary>
    /// Summary description for TotalConfig.
    /// </summary>
    public class TotalConfig
    {
        public static EnumDefineMode defineMode = EnumDefineMode.MODE_RUN;
        public static bool bClientFlag = false;
        public static bool bLocalMain = false;		// LocalMain에 의해서 실행되었다.
        public static System.Windows.Forms.Form formMain;

        public TotalConfig()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static string LoadRegConfig(string root_name, string sub_name, string item, string default_value)
        {
            RegistryKey key1 = Registry.CurrentUser;
            RegistryKey key2;
            object obj;
            string get_string = default_value;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\Username site\\{0}", root_name);
            else
                key_name = String.Format("Software\\Username site\\{0}\\{1}", root_name, sub_name);

            key2 = key1.OpenSubKey(key_name);
            if (key2 != null)
            {
                obj = key2.GetValue(item);
                if (obj != null)
                {
                    get_string = obj.ToString();
                }
                key2.Close();
            }
            key1.Close();

            return get_string;
        }

        public static byte[] LoadRegConfig(string root_name, string sub_name, string item, byte[] default_value)
        {
            RegistryKey key1 = Registry.CurrentUser;
            RegistryKey key2;
            object obj = null;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\Username site\\{0}", root_name);
            else
                key_name = String.Format("Software\\Username site\\{0}\\{1}", root_name, sub_name);

            key2 = key1.OpenSubKey(key_name);
            if (key2 != null)
            {
                obj = key2.GetValue(item);
                if (obj != null)
                {
                    //get_string = obj.ToString();
                }
                key2.Close();
            }
            key1.Close();

            if (obj == null) return default_value;
            else
            {
                if (obj.GetType() == typeof(byte[]))
                    return (byte[])obj;
                else
                    return default_value;
            }
        }

        public static void SaveRegConfig(string root_name, string sub_name, string item, object set_value)
        {
            if (set_value == null) return;	// 값이 설정되어 있지 않을 때는 return한다.
            RegistryKey key1 = Registry.CurrentUser;
            RegistryKey key2;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\Username site\\{0}", root_name);
            else
                key_name = String.Format("Software\\Username site\\{0}\\{1}", root_name, sub_name);

            key2 = key1.CreateSubKey(key_name);
            if (key2 != null)
            {
                // DWORD로 저장하고 싶으면 int로 변수를 보내야 한다.
                key2.SetValue(item, set_value);
                key2.Close();
            }
            key1.Close();
        }

        public static int LoadRegConfig(string root_name, string sub_name, string item, int default_value)
        {
            string val = LoadRegConfig(root_name, sub_name, item, default_value.ToString());

            try
            {
                return ConvertTool.ToInt32(val);
            }
            catch
            {
                return 0;
            }
        }

        public static bool LoadRegConfig(string root_name, string sub_name, string item, bool default_value)
        {
            string val = LoadRegConfig(root_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToBoolean(val);
        }

        public static uint LoadRegConfig(string root_name, string sub_name, string item, uint default_value)
        {
            string val = LoadRegConfig(root_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToUInt32(val);
        }


    }
}
