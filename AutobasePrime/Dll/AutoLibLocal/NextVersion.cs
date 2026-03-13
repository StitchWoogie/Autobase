using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoLibLocal
{
    public class NextVersion
    {
        public static bool bScript11 = false;
        public static bool bScriptList = false;
        public static bool bAutobase11 = false;
        public static bool bUseNewScriptInScriptClass = false;
        public static bool bIgnoreOemKeyOnStudio = false;

        static NextVersion()
        {
            bScript11 = IsAblePreview("Script11");
            bScriptList = IsAblePreview("ScriptList");
            bAutobase11 = IsAblePreview("Autobase11");
            bUseNewScriptInScriptClass = IsAblePreview("UseNewScriptInScriptClass");

            bIgnoreOemKeyOnStudio = IsAblePreview("IgnoreOemKeyOnStudio");
        }

        static bool IsAblePreview(string item)
        {
            return TotalConfig.LoadRegAutoBaseConfig("NextVersion", null, item, false);
        }

        /*
        public static bool Script11()
        {
            return IsAblePreview("Script11");
        }

        public static bool ScriptList()
        {
            return IsAblePreview("ScriptList");
        }

        public static bool IsAutobase11
        {
            get
            {
                return IsAblePreview("Autobase11");
            }
        }

        public static bool IsAutobase11
        {
            get
            {
                return IsAblePreview("Autobase11");
            }
        }*/

        /*
        public static bool DataGridView()
        {
            return IsAblePreview("DataGridView");
        }*/
    }
}
