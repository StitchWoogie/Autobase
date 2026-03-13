using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace NetTools
{
    public class RegistryTool
    {
        public static string LoadConfig(RegistryKey key1, string app_name, string sub_name, string item, string default_value)
        {
            RegistryKey key2;
            object obj;
            string get_string = default_value;

            string key_name;

            key_name = String.Format("Software\\{0}\\{1}", app_name, sub_name);

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
            //key1.Close(); root 키를 굳이 닫을 필요는 없다. 2010-4-19

            return get_string;
        }

        public static byte[] LoadConfig(RegistryKey key1, string app_name, string sub_name, string item, byte[] default_value)
        {
            RegistryKey key2;
            object obj = null;

            string key_name;

            key_name = String.Format("Software\\{0}\\{1}", app_name, sub_name);

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
            //key1.Close(); root 키를 굳이 닫을 필요는 없다. 2010-4-19

            if (obj == null) return default_value;
            else
            {
                if (obj.GetType() == typeof(byte[]))
                    return (byte[])obj;
                else
                    return default_value;
            }
        }

        public static int LoadConfig(RegistryKey key1, string app_name, string sub_name, string item, int default_value)
        {
            return LoadConfig(Registry.CurrentUser, app_name, sub_name, item, default_value);
        }

        public static string LoadConfig(string app_name, string sub_name, string item, string default_value)
        {
            return LoadConfig(Registry.CurrentUser, app_name, sub_name, item, default_value);
        }
        
        public static bool LoadConfig(string app_name, string sub_name, string item, bool default_value)
        {
            string val = LoadConfig(app_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToBoolean(val);
        }

        public static int LoadConfig(string app_name, string sub_name, string item, int default_value)
        {
            string val = LoadConfig(app_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToInt32(val);
        }

        public static void SaveConfig(RegistryKey key1, string app_name, string sub_name, string item, object set_value)
        {
            if (set_value == null) return;	// 값이 설정되어 있지 않을 때는 return한다.

            RegistryKey key2;

            string key_name;

            key_name = String.Format("Software\\{0}\\{1}", app_name, sub_name);

            key2 = key1.CreateSubKey(key_name);
            if (key2 != null)
            {
                // DWORD로 저장하고 싶으면 int로 변수를 보내야 한다.
                key2.SetValue(item, set_value);
                key2.Close();
            }
        }

        public static void SaveConfig(string app_name, string sub_name, string item, object set_value)
        {
            SaveConfig(Registry.CurrentUser, app_name, sub_name, item, set_value);
        }

        
    }
}
