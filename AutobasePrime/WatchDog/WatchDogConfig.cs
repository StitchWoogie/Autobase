using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoLibLocal;

namespace WatchDog
{
    class WatchDogConfig
    {
        public static bool bHideOnStartUp;

        public static void Load()
        {
            bHideOnStartUp = TotalConfig.LoadRegAutoBaseConfig("WatchDog", "Config", "bHideOnStartUp", false);
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("WatchDog", "Config", "bHideOnStartUp", bHideOnStartUp);
        }
    }
}
