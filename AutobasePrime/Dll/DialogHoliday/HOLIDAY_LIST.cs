using System;

namespace DialogHoliday
{
	/// <summary>
	/// Summary description for HOLIDAY_LIST.
	/// </summary>
	/// 
	[Serializable]
	public class HOLIDAY_LIST
	{
		public string 	title;

		public sbyte	type;	// 0 = 날짜, 1 = 요일
		public short	year;	// 0 = 매년
		public sbyte   month;
		public sbyte	day = 1;
		public sbyte	bSunOrMoon;	// 0 = 양력, 1 = 음력
	
		public sbyte	week;	// 주 0 = 매주
		public sbyte	weekday;	// 요일 0 = 일요일
	}
}
