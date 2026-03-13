using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using NetTools;

namespace AutoLibLocal
{
    /// <summary>
    /// 레지스트리를 구해놓고 하면 그렇지 않은 경우보다 속도가 3배정도 더 증가한다.
    /// </summary>
    public class SharedRegistryClass
    {
        RegistryKey key2 = null;
        public void Init(string root_name, string sub_name)
        {
            key2 = TotalConfig.GetRegKeyAutoBaseConfig(root_name, sub_name);
        }

        public void Write(string item, object value)
        {
            if (key2 == null) return;

            // DWORD로 저장하고 싶으면 int로 변수를 보내야 한다.
            key2.SetValue(item, value);
        }

        public string Read(string item, string default_value)
        {
            if (key2 == null)
            {
                return default_value;
            }

            object obj;
            string get_string = default_value;

            obj = key2.GetValue(item);
            if (obj != null)
            {
                get_string = obj.ToString();
            }

            return get_string;
        }

        public int Read(string item, int default_value)
        {
            string val = Read(item, default_value.ToString());

            return ConvertTool.ToInt32(val);
        }

        public void UnInit()
        {
            if (key2 == null) return;

            key2.Close();
            key2 = null;
        }
    }
}
