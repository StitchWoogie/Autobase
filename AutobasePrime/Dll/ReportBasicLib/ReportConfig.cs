using System;
using AutoLibLocal;
using NetTools;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for ReportConfig.
	/// </summary>
	public class ReportConfig
	{
		public static string sNoData = "***";
		public static bool bCalcNoData = true;

		public static bool bRunByIIS = false;
		
		static ReportConfig()
		{
			//
			// TODO: Add constructor logic here
			//
			sNoData = NoData;
			bCalcNoData = CalcNoData;

            DateTime t = DateTimeServer.Now;

			frMultiPrint.year = t.Year;
			frMultiPrint.mon = t.Month;
			frMultiPrint.day = t.Day;
			frMultiPrint.hour = t.Hour;
			frMultiPrint.min = t.Minute;
			frMultiPrint.sec = t.Second;

			toMultiPrint.year = t.Year;
			toMultiPrint.mon = t.Month;
			toMultiPrint.day = t.Day;
			toMultiPrint.hour = t.Hour;
			toMultiPrint.min = t.Minute;
			toMultiPrint.sec = t.Second;

		}

		public static string NoData
		{
			get 
			{
				return TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "NoData", "***");
			}
			set 
			{
				TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "NoData", value);
			}
		}	


		public static string ReportPrintFunctionPrinter
		{
			get 
			{
				return TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "ReportPrintFunctionPrinter", "");
			}
			set 
			{
				TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "ReportPrintFunctionPrinter", value);
			}
		}	


		public static bool CalcNoData
		{
			get 
			{
				return TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "CalcNoData", true);
			}
			set 
			{
				TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "CalcNoData", value);
			}
		}	

		public static int InitCellWidth
		{
			get 
			{
				return TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "InitCellWidth", 80);
			}
			set 
			{
				TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "InitCellWidth", value);
			}
		}

		public static int InitCellHeight
		{
			get 
			{
				return TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "InitCellHeight", 25);
			}
			set 
			{
				TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "InitCellHeight", value);
			}
		}

        public static DateTime tMinListTimeFrAtIIS = DateTimeServer.Now;
        public static DateTime tMinListTimeToAtIIS = DateTimeServer.Now;

		public static void GetMinListTimeFr(USER_SELECT_TIME time)
		{
			if(bRunByIIS) 
			{
				time.year = tMinListTimeFrAtIIS.Year;
				time.mon  = tMinListTimeFrAtIIS.Month;
				time.day  = tMinListTimeFrAtIIS.Day;
				time.hour = tMinListTimeFrAtIIS.Hour;
				time.min  = tMinListTimeFrAtIIS.Minute;
				time.sec  = tMinListTimeFrAtIIS.Second;
				return;
			}

            DateTime t = DateTimeServer.Now;

            string default_buf = String.Format("{0},{1},{2},{3},{4},", t.Year, t.Month, t.Day, t.Hour, t.Minute);

			string val = TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "MinListFrom", default_buf);
			
			CommaBlockString comma = new CommaBlockString(); 

			comma.Set(val);
			comma.GetInt(ref time.year);
			comma.GetInt(ref time.mon);
			comma.GetInt(ref time.day);
			comma.GetInt(ref time.hour);
			comma.GetInt(ref time.min);
			time.sec = t.Second;

			if(!TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
			{
				time.year = t.Year;
				time.mon = t.Month;
				time.day = t.Day;
			}
		}

		public static DateTime GetMinListTimeFr()
		{
			USER_SELECT_TIME time = new USER_SELECT_TIME();
			GetMinListTimeFr(time);
			return new DateTime(time.year, time.mon, time.day, time.hour, time.min, time.sec);
		}

		public static DateTime GetMinListTimeTo()
		{
			USER_SELECT_TIME time = new USER_SELECT_TIME();
			GetMinListTimeTo(time);
			return new DateTime(time.year, time.mon, time.day, time.hour, time.min, time.sec);
		}

		public static void GetMinListTimeTo(USER_SELECT_TIME time)
		{
			if(bRunByIIS) 
			{
				time.year = tMinListTimeToAtIIS.Year;
				time.mon  = tMinListTimeToAtIIS.Month;
				time.day  = tMinListTimeToAtIIS.Day;
				time.hour = tMinListTimeToAtIIS.Hour;
				time.min  = tMinListTimeToAtIIS.Minute;
				time.sec  = tMinListTimeToAtIIS.Second;
				return;
			}

            DateTime t = DateTimeServer.Now;

			string default_buf = String.Format("{0},{1},{2},{3},{4},", t.Year, t.Month, t.Day, t.Hour, t.Minute);

			string val = TotalConfig.LoadRegAutoBaseConfig("Config", "Reporter", "MinListTo", default_buf);
			
			CommaBlockString comma = new CommaBlockString(); 

			comma.Set(val);
			comma.GetInt(ref time.year);
			comma.GetInt(ref time.mon);
			comma.GetInt(ref time.day);
			comma.GetInt(ref time.hour);
			comma.GetInt(ref time.min);
			time.sec = t.Second;

			if(!TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
			{
				time.year = t.Year;
				time.mon = t.Month;
				time.day = t.Day;
			}
		}

		public static void SetMinListTimeFr(USER_SELECT_TIME t)
		{
			if(bRunByIIS) 
			{
				tMinListTimeFrAtIIS = new DateTime(t.year, t.mon, t.day, t.hour, t.min, t.sec);
				return;
			}

			string default_buf = String.Format("{0},{1},{2},{3},{4},", t.year, t.mon, t.day, t.hour, t.min);

			TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "MinListFrom", default_buf);
		}

		public static void SetMinListTimeFr(DateTime t)
		{
			if(bRunByIIS) 
			{
				tMinListTimeFrAtIIS = new DateTime(t.Ticks);
				return;
			}

			string default_buf = String.Format("{0},{1},{2},{3},{4},", t.Year, t.Month, t.Day, t.Hour, t.Minute);

			TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "MinListFrom", default_buf);
		}

		public static void SetMinListTimeTo(USER_SELECT_TIME t)
		{
			if(bRunByIIS) 
			{
				tMinListTimeToAtIIS = new DateTime(t.year, t.mon, t.day, t.hour, t.min, t.sec);
				return;
			}

			string default_buf = String.Format("{0},{1},{2},{3},{4},", t.year, t.mon, t.day, t.hour, t.min);

			TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "MinListTo", default_buf);
		}

		public static void SetMinListTimeTo(DateTime t)
		{
			if(bRunByIIS) 
			{
				tMinListTimeToAtIIS = new DateTime(t.Ticks);
				return;
			}

			string default_buf = String.Format("{0},{1},{2},{3},{4},", t.Year, t.Month, t.Day, t.Hour, t.Minute);

			TotalConfig.SaveRegAutoBaseConfig("Config", "Reporter", "MinListTo", default_buf);
		}

        public static DateTime tHandReportTime = DateTimeServer.Now;
        public static DateTime tAutoReportTime = DateTimeServer.Now;

		public static void GetHandSelectTime(USER_SELECT_TIME time)
		{
			time.year = tHandReportTime.Year;
			time.mon = tHandReportTime.Month;
			time.day = tHandReportTime.Day;
			time.hour = tHandReportTime.Hour;
			time.min = tHandReportTime.Minute;
			time.sec = tHandReportTime.Second;
		}

		public static void GetAutoSelectTime(USER_SELECT_TIME time)
		{
			time.year = tAutoReportTime.Year;
			time.mon = tAutoReportTime.Month;
			time.day = tAutoReportTime.Day;
			time.hour = tAutoReportTime.Hour;
			time.min = tAutoReportTime.Minute;
			time.sec = tAutoReportTime.Second;
		}

		public static void SetAutoSelectTime(USER_SELECT_TIME time)
		{
			tAutoReportTime = new DateTime(time.year, time.mon, time.day, time.hour, time.min, time.sec);
		}

		public static USER_SELECT_TIME frMultiPrint = new USER_SELECT_TIME();
		public static USER_SELECT_TIME toMultiPrint = new USER_SELECT_TIME();
	}
}
