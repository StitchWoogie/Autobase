using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoLibLocal
{
    public class Log
    {
   
        //public delegate void DelegateLog(EnumEventID eid, string format, params object[] args);
        public delegate void DelegateLog(LogLevel level, int category, string format, params object[] args); 
        public static DelegateLog procLog = null;

        public static void Write(LogLevel level, int category, string format, params object[] args)
        {
            if (procLog != null)
            {
                procLog(level, category, format, args);
            }
        }
        
    }
}
