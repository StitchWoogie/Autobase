using System;
using System.Drawing;
using System.IO;

namespace NetTools
{
	/// <summary>
	/// Summary description for OldDefine.
	/// </summary>
	namespace OldDefine
	{
		[Serializable]
		public class POINT
		{
			public int x;
			public int y;
		}

		[Serializable]
		public class SIZE
		{
			public int cx;
			public int cy;
		}

		[Serializable]
		public class RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public void Set(Rectangle r)
			{
				left = r.Left;
				top = r.Top;
				right = r.Right;
				bottom = r.Bottom;
			}
		}

		[Serializable]
		public class LOGFONT
		{
			public string lfFaceName;
			public float  lfHeight;
			public FontStyle style;
			//public int top;
			//public int right;
			//public int bottom;
			
		}

		enum a
		{
			OFF = 0,
			ON = 1,
		}

		[Serializable]
		public class SYSTEMTIME // size is 18
		{
			public ushort wYear;
			public ushort wMonth;
			public ushort wDayOfWeek;
			public ushort wDay;
			public ushort wHour;
			public ushort wMinute;
			public ushort wSecond;
			public ushort wMilliseconds;

			public SYSTEMTIME()
			{

			}

			public SYSTEMTIME(SYSTEMTIME s)
			{
				wYear = s.wYear;
				wMonth = s.wMonth;
				wDayOfWeek = s.wDayOfWeek;
				wDay = s.wDay;
				wHour = s.wHour;
				wMinute = s.wMinute;
				wSecond = s.wSecond;
				wMilliseconds = s.wMilliseconds;
			}

			public void LoadFromFile(BinaryReader reader)
			{
				wYear = reader.ReadUInt16();
				wMonth = reader.ReadUInt16();
				wDayOfWeek = reader.ReadUInt16();
				wDay = reader.ReadUInt16();
				wHour = reader.ReadUInt16();
				wMinute = reader.ReadUInt16();
				wSecond = reader.ReadUInt16();
				wMilliseconds = reader.ReadUInt16();
			}

			public DateTime ToDateTime()
			{
                try
                {
                    return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
                }
                catch
                {
                    return DateTime.Now;
                }
			}

			public void GetLocalTime()
			{
				DateTime dt = DateTime.Now;

				Set(dt);
			}

			public void Set(DateTime t)
			{
				wYear = (ushort)t.Year;
				wMonth = (ushort)t.Month;
				wDay = (ushort)t.Day;
				wHour = (ushort)t.Hour;
				wMinute = (ushort)t.Minute;
				wSecond = (ushort)t.Second;
				wMilliseconds = (ushort)t.Millisecond;
				wDayOfWeek = (ushort)t.DayOfWeek;
			}

			public static void memcpy(SYSTEMTIME tar, SYSTEMTIME src)
			{
				tar.wYear = src.wYear;
				tar.wMonth = src.wMonth;
				tar.wDay = src.wDay;
				tar.wHour = src.wHour;
				tar.wMinute = src.wMinute;
				tar.wSecond = src.wSecond;
				tar.wMilliseconds = src.wMilliseconds;
				tar.wDayOfWeek = src.wDayOfWeek;
			}

            public static int CompareTime(SYSTEMTIME fr, SYSTEMTIME to)
            {
                if (fr.wYear != to.wYear) return fr.wYear - to.wYear;
                if (fr.wMonth != to.wMonth) return fr.wMonth - to.wMonth;
                if (fr.wDay != to.wDay) return fr.wDay - to.wDay;
                if (fr.wHour != to.wHour) return fr.wHour - to.wHour;
                if (fr.wMinute != to.wMinute) return fr.wMinute - to.wMinute;
                if (fr.wSecond != to.wSecond) return fr.wSecond - to.wSecond;
                if (fr.wMilliseconds != to.wMilliseconds) return fr.wMilliseconds - to.wMilliseconds;

                return 0;
            }

            public long GetMilliSecHap()
            {
                return TimeUtil.GetMilliSecHap(new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds));
            }

		}

		[Serializable]
		public class datetime // size is 18
		{
			public int da_year;
			public int da_mon;
			public int da_day;
			public int ti_hour;
			public int ti_min;
			public int ti_sec;
		}
	}
}
