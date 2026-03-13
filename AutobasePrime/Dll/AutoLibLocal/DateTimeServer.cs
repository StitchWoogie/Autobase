using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoLibLocal
{
    // 서버의 시간에 따라갈 수 있도록 현재 시간을 조절하는 역할을 한다.
    public class DateTimeServer
    {
        //static int nMyTimeZoneSeconds;
        static string sMyTimeZoneID;

        //public static int nServerTimeZoneSeconds;
        static string sServerTimeZoneID;

        static TimeZoneInfo tziServer = null;

        static DateTimeServer()
        {
            TimeSpan ts = TimeZoneInfo.Local.BaseUtcOffset;
            //nServerTimeZoneSeconds = (int)ts.TotalSeconds;
            sServerTimeZoneID = sMyTimeZoneID = TimeZoneInfo.Local.Id;
        }

        public static bool IsSameZone
        {
            get
            {
                return (tziServer == null);
            }
        }

        public static string ServierTimeZoneID
        {
            set
            {
                sServerTimeZoneID = value;

                if (sServerTimeZoneID == sMyTimeZoneID) return;

                try
                {
                    tziServer = TimeZoneInfo.FindSystemTimeZoneById(sServerTimeZoneID);
                }
                catch
                {
                    tziServer = null;
                }
            }
        }

        public static DateTime Now
        {
            get
            {
                if (tziServer == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    DateTime t = DateTime.UtcNow;
                    return TimeZoneInfo.ConvertTimeFromUtc(t, tziServer);
                }
            }
        }

        public static DateTime ConvertTime(DateTime src)
        {
            if (tziServer == null)
            {
                return src;
            }
            else
            {
                return TimeZoneInfo.ConvertTime(src, tziServer);
            }
        }
    }
}
