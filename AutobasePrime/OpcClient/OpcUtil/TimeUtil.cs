using System;
using NetTools.OldDefine;
using System.Runtime.InteropServices;

namespace NetTools
{
	/// <summary>
	/// Summary description for TimeUtil.
	/// </summary>
	public class TimeUtil
	{
		public TimeUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Exception이 발생되지 않게 존재하는 날짜를 만든다.
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static DateTime MakeDateTime(int year, int month, int day, int hour, int minute, int second)
		{
			if(year < 1)	year = 1;
			if(year > 9999)	year = 9999;

			if(month < 1)	month = 1;
			if(month > 12)	month = 12;

			if(day < 1)		day = 1;
			if(day > getmonthlimit(year, month))	day = getmonthlimit(year, month);

			if(hour < 0)	hour = 0;
			if(hour > 23)	hour = hour%24;

			if(minute < 0)	minute = 0;
			if(minute > 59)	minute = minute%60;

			if(second < 0)	second = 0;
			if(second > 59)	second = second%60;

			return new DateTime(year, month, day, hour, minute, second);
		}

		public static DateTime MinusMin(DateTime org, int val)
		{
			int year = org.Year;
			int mon = org.Month;
			int day = org.Day;
			int hour = org.Hour;
			int min = org.Minute;
			for(int i = 0; i < val; i++) 
			{
				if(min <= 0) 
				{
					min = 59;
					if(hour <= 0) 
					{
						hour = 23;
						if(day <= 1) 
						{
							if(mon <= 1) 
							{	// 1 월 1 일 일때
								year--;
								mon = 12;
								day = 31;
							}
							else 
							{                    // ? 월 1 일 일때
								mon--;
								day = getmonthlimit(year, mon);
							}
						}
						else 
						{
							day--;
						}
					}
					else 
					{
						hour--;
					}
				}
				else 
				{
					min--;
				}
			}
			return new DateTime(year, mon, day, hour, min, org.Second);
		}

		public static void MinusSecond(ref int year, ref int mon, ref int day, ref int hour, ref int min, ref int sec)
		{
			if(sec <= 0) 
			{
				sec = 59;
				MinusMin(ref year, ref mon, ref day, ref hour, ref min);
			}
			else 
			{
				sec--;
			}
		}

		public static void MinusMin(ref int year, ref int mon, ref int day, ref int hour, ref int min)
		{
			int val = 1;

			for(int i = 0; i < val; i++) 
			{
				if(min <= 0) 
				{
					min = 59;
					if(hour <= 0) 
					{
						hour = 23;
						if(day <= 1) 
						{
							if(mon <= 1) 
							{	// 1 월 1 일 일때
								year--;
								mon = 12;
								day = 31;
							}
							else 
							{                    // ? 월 1 일 일때
								mon--;
								day = getmonthlimit(year, mon);
							}
						}
						else 
						{
							day--;
						}
					}
					else 
					{
						hour--;
					}
				}
				else 
				{
					min--;
				}
			}
		}

 
		public static DateTime MinusHour(DateTime org, int val)
		{
			int year = org.Year;
			int mon = org.Month;
			int day = org.Day;
			int hour = org.Hour;
			for(int i = 0; i < val; i++) 
			{
				if(hour <= 0) 
				{
					hour = 23;
					if(day <= 1) 
					{
						if(mon <= 1) 
						{	// 1 월 1 일 일때
							year--;
							mon = 12;
							day = 31;
						}
						else 
						{                    // ? 월 1 일 일때
							mon--;
							day = getmonthlimit(year, mon);
						}
					}
					else 
					{
						day--;
					}
				}
				else 
				{
					hour--;
				}
			}
			return new DateTime(year, mon, day, hour, org.Minute, org.Second);
		}

		public static void MinusHour(ref int year, ref int mon, ref int day, ref int hour)
		{
			int val = 1;
			for(int i = 0; i < val; i++) 
			{
				if(hour <= 0) 
				{
					hour = 23;
					if(day <= 1) 
					{
						if(mon <= 1) 
						{	// 1 월 1 일 일때
							year--;
							mon = 12;
							day = 31;
						}
						else 
						{                    // ? 월 1 일 일때
							mon--;
							day = getmonthlimit(year, mon);
						}
					}
					else 
					{
						day--;
					}
				}
				else 
				{
					hour--;
				}
			}
		}

		public static DateTime MinusDay(DateTime org, int val)
		{
			int year = org.Year;
			int mon = org.Month;
			int day = org.Day;
			for(int i = 0; i < val; i++) 
			{
				if(day <= 1) 
				{
					if(mon <= 1) 
					{	// 1 월 1 일 일때
						year--;
						mon = 12;
						day = 31;
					}
					else 
					{                    // ? 월 1 일 일때
						mon--;
						day = getmonthlimit(year, mon);
					}
				}
				else 
				{
					day--;
				}
			}
			return new DateTime(year, mon, day, org.Hour, org.Minute, org.Second);
		}

		public static void MinusDay(ref int year, ref int mon, ref int day)
		{
			int val = 1;
			for(int i = 0; i < val; i++) 
			{
				if(day <= 1) 
				{
					if(mon <= 1) 
					{	// 1 월 1 일 일때
						year--;
						mon = 12;
						day = 31;
					}
					else 
					{                    // ? 월 1 일 일때
						mon--;
						day = getmonthlimit(year, mon);
					}
				}
				else 
				{
					day--;
				}
			}
		}

		public static DateTime MinusMonth(DateTime org, int val)
		{
			int year = org.Year;
			int mon = org.Month;
			for(int i = 0; i < val; i++) 
			{
				if(mon <= 1) 
				{	// 1 월 1 일 일때
					year--;
					mon = 12;
				}
				else 
				{                    // ? 월 1 일 일때
					mon--;
				}
			}
			return new DateTime(year, mon, org.Day, org.Hour, org.Minute, org.Second);
		}

		public static void MinusMonth(ref int year, ref int mon)
		{
			int val = 1;
			for(int i = 0; i < val; i++) 
			{
				if(mon <= 1) 
				{	// 1 월 1 일 일때
					year--;
					mon = 12;
				}
				else 
				{                    // ? 월 1 일 일때
					mon--;
				}
			}
		}

		public static void MinusMonth(SYSTEMTIME t)
		{
			int val = 1;
			for(int i = 0; i < val; i++) 
			{
				if(t.wMonth <= 1) 
				{	// 1 월 1 일 일때
					t.wYear--;
					t.wMonth = 12;
				}
				else 
				{                    // ? 월 1 일 일때
					t.wMonth--;
				}
			}
		}

		public static void MinusYear(ref int year)
		{
			year--;
		}

		public static void PlusMilliSecond(ref int year, ref int mon, ref int day, ref int hour, ref int min, ref int sec, ref int milli)
		{
			if(milli >= 999) 
			{
				milli = 0;
				PlusSecond(ref year, ref mon, ref day, ref hour, ref min, ref sec);
			}
			else 
			{
				milli++;
			}
		}

		public static void PlusSecond(ref int year, ref int mon, ref int day, ref int hour, ref int min, ref int sec)
		{
			if(sec >= 59) 
			{
				sec = 0;
				PlusMin(ref year, ref mon, ref day, ref hour, ref min);
			}
			else 
			{
				sec++;
			}
		}

		public static void PlusMin(ref int year, ref int mon, ref int day, ref int hour, ref int min)
		{
			if(min >= 59) 
			{
				min = 0;
				PlusHour(ref year, ref mon, ref day, ref hour);
			}
			else 
			{
				min++;
			}
		}

		public static void PlusHour(ref int year, ref int mon, ref int day, ref int hour)
		{
			if(hour >= 23) 
			{
				hour = 0;
				PlusDay(ref year, ref mon, ref day);
			}
			else 
			{
				hour++;
			}
		}

		public static void PlusDay(ref int year, ref int mon, ref int day)
		{
			if(day >= getmonthlimit(year, mon)) 
			{	 // 그 달의 마지막 날
				if(mon >= 12) 
				{	// 12월 31일 일때
					year++;
					mon = 1;
					day = 1;
				}
				else 
				{                    // ? 월 31 일 일때
					mon++;
					day = 1;
				}
			}
			else 
			{
				day++;
			}
		}

		public static void PlusMonth(ref int year, ref int mon)
		{
			if(mon >= 12) 
			{	// 12월 31일 일때
				year++;
				mon = 1;
			}
			else 
			{                    // ? 월 31 일 일때
				mon++;
			}

		}

		public static void PlusMonth(SYSTEMTIME t)
		{
			if(t.wMonth >= 12) 
			{	// 12월 31일 일때
				t.wYear++;
				t.wMonth = 1;
			}
			else 
			{                    // ? 월 31 일 일때
				t.wMonth++;
			}

		}

		public static void PlusYear(ref int year)
		{
			year++;
		}

		public static long GetMonHap(int year, int month)
		{
			return (year-1)*12+month;
		}

		public static long GetDayHap(int year, int month, int day)
		{
			int[] limit = {31,28,31,30,31,30,31,31,30,31,30,31};	
			int i;
			long dayhap;

			dayhap = (((long)year-1)*365)+((year-1)/4)-((year-1)/100)+((year-1)/400);

			if((year%400) == 0)         limit[1] = 29;
			else if((year%100) == 0 )	limit[1] = 28;
			else if((year%4) == 0)		limit[1] = 29;
			else						limit[1] = 28;

			if(month != 1)
				for(i = 0; i < month-1; i++)
					dayhap += limit[i%12];

			dayhap += day;

			return dayhap;
		}

		public static long GetHourHap(int year, int month, int day, int hour)
		{
			return GetDayHap(year, month, day)*24+hour;
		}

		public static long GetHourHap(DateTime t)
		{
			return GetDayHap(t.Year, t.Month, t.Day)*24+t.Hour;
		}

		public static long GetMinHap(int year, int month, int day, int hour, int min)
		{
			return GetHourHap(year, month, day, hour)*60+min;
		}

		public static long GetMinHap(DateTime dt)
		{
			return GetHourHap(dt.Year, dt.Month, dt.Day, dt.Hour)*60+dt.Minute;
		}

		public static int GetWeekDay(int year, int month, int day)
		{
			long dayhap = GetDayHap(year, month, day);

			while(dayhap > 14000L)	dayhap -= 14000L;

			return (((int)dayhap) % 7); 
		}

		// 지정한 년도의 전체 주 개수를 구하는 함수
		public static int GetWeekCount(int year)	
		{
			int dayhap = 365;

			if(getmonthlimit(year, 2) == 29) dayhap ++;
			return (dayhap+6+GetWeekDay(year, 1, 1))/7;			
		}

		public static void AddMilliSecond(SYSTEMTIME t, int milli)
		{
			int remain = milli+t.wMilliseconds;

			while(remain >= 1000) 
			{
				PlusSecond(t);
				remain -= 1000;
			}

			t.wMilliseconds = (ushort)remain;
		}

		public static void PlusSecond(SYSTEMTIME t)
		{
			if(t.wSecond >= 59) 
			{
				PlusMin(t);
				t.wSecond = 0;
			}
			else 
			{
				t.wSecond ++;
			}
		}

		
		public static void PlusMin(SYSTEMTIME t)
		{
			if(t.wMinute >= 59) 
			{
				t.wMinute = 0;
				PlusHour(t);
			}
			else 
			{
				t.wMinute++;
			}
		}

		public static void PlusHour(SYSTEMTIME t)
		{
			if(t.wHour >= 23) 
			{
				t.wHour = 0;
				PlusDay(t);
			}
			else 
			{
				t.wHour++;
			}
		}

		public static void PlusDay(SYSTEMTIME t)
		{
			if(t.wDay >= getmonthlimit(t.wYear, t.wMonth)) 
			{	 // 그 달의 마지막 날
				if(t.wMonth >= 12) 
				{	// 12월 31일 일때			
					t.wYear++;
					t.wMonth = 1;
					t.wDay = 1;
				}
				else 
				{                    // ? 월 31 일 일때
					t.wMonth++;
					t.wDay = 1;
				}
			}
			else 
			{
				t.wDay++;
			}
		}

		public static bool IsDayExist(int year, int mon, int day)
		{
			int limit = getmonthlimit(year, mon);
			if(day > limit)	return false;
			if(day <= 0)		return false;
			return true;
		}

		public static int getmonthlimit(int year, int month)
		{
			short[] limit = new short[12];
			
			limit[0] = 31;
			limit[1] = 28;
			limit[2] = 31;
			limit[3] = 30;
			limit[4] = 31;
			limit[5] = 30;
			limit[6] = 31;
			limit[7] = 31;
			limit[8] = 30;
			limit[9] = 31;
			limit[10] = 30;
			limit[11] = 31;

			if(month <= 0 || month > 12) return 0;
			if(month != 2)	return(limit[month-1]);

			if(year%400 == 0)	return 29;
			if(year%100 == 0)	return 28;
			if(year%4 == 0)	return 29;
			return 28;
		}
		
		public static void plusOneDay(ref int year, ref int mon, ref int day)
		{
			if(mon == 12 && day == 31) 
			{
				if(year >= 9999) return;
				year += 1;
				mon = 1;
				day = 1;
			}
			else if(day >= getmonthlimit(year, mon)) 
			{
				mon += 1;
				day = 1;
			}
			else day += 1;
		}

		[StructLayout( LayoutKind.Sequential)]
		class SystemTime
		{
			public ushort year;
			public ushort month;
			public ushort dayOfWeek=0;
			public ushort day;
			public ushort hour;
			public ushort minute;
			public ushort second;
			public ushort milliseconds;
		}

		[DllImport("Kernel32.dll")]
		static extern void SetLocalTime([Out]SystemTime st);

		public static void SetLocalTime(int year, int mon, int day, int hour, int min, int sec)
		{
			SystemTime st = new SystemTime();
			st.year = (ushort)year;
			st.month = (ushort)mon;
			st.day = (ushort)day;
			st.hour = (ushort)hour;
			st.minute = (ushort)min;
			st.second = (ushort)sec;
			st.milliseconds = 0;
			SetLocalTime(st);
		}
	}
}

