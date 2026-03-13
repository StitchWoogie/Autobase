using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;

namespace ExportServer
{
    class ExportServerConfig
    {
        public static bool bHideOnStartUp;

        public static void Load()
        {
            bHideOnStartUp = TotalConfig.LoadRegAutoBaseConfig("ExportServer", "Config", "bHideOnStartUp", false);
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("ExportServer", "Config", "bHideOnStartUp", bHideOnStartUp);
        }
    }
}
