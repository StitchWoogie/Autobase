using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools
{
    public class DeveloperVersion
    {
        public static bool bDebugSpeed = false;

        static DeveloperVersion()
        {
            bDebugSpeed = IsAblePreview("DebugSpeed");
        }

        static bool IsAblePreview(string item)
        {
            string retn = RegistryTool.LoadConfig("AutoBaseInc", "DeveloperVersion", item, "false");

            return (String.Compare(retn, "true", true) == 0);
        }

        
    }
}
