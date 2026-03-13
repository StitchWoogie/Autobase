using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using NetTools.OldDefine;
using System.Threading;
using DialogHoliday;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionComboBox.
	/// </summary>
	public class ScriptFunctionTime
	{
        /*		
        public ScriptFunctionTime()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        
		static int Function_TimePlusSec(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour, smin, ssec;
			int year, mon, day, hour, min, sec;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out shour);
			if(!scriptClass.GetValueRecurse(shour, shour.Length, out imsi))	return -1;
			hour = (int)imsi;
			arg.GetArgument(out smin);
			if(!scriptClass.GetValueRecurse(smin, smin.Length, out imsi))	return -1;
			min = (int)imsi;
			arg.GetArgument(out ssec);
			if(!scriptClass.GetValueRecurse(ssec, ssec.Length, out imsi))	return -1;
			sec = (int)imsi;

			TimeUtil.PlusSecond(ref year, ref mon, ref day, ref hour, ref min, ref sec);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimePlusSec() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimePlusSec() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimePlusSec() arg3"))	return -1;	
			if(!scriptClass.ChangeNumberVar(shour, hour, "@TimePlusSec() arg4"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smin,  min, "@TimePlusSec() arg5"))	return -1;	
			if(!scriptClass.ChangeNumberVar(ssec,  sec, "@TimePlusSec() arg6"))	return -1;	
			
			return 1;
		}

		static int Function_TimePlusMin(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour, smin;
			int year, mon, day, hour, min;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out shour);
			if(!scriptClass.GetValueRecurse(shour, shour.Length, out imsi))	return -1;
			hour = (int)imsi;
			arg.GetArgument(out smin);
			if(!scriptClass.GetValueRecurse(smin, smin.Length, out imsi))	return -1;
			min = (int)imsi;

			TimeUtil.PlusMin(ref year, ref mon, ref day, ref hour, ref min);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimePlusMin() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimePlusMin() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimePlusMin() arg3"))	return -1;	
			if(!scriptClass.ChangeNumberVar(shour, hour, "@TimePlusMin() arg4"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smin,  min, "@TimePlusMin() arg5"))	return -1;	
			
			return 1;
		}

		static int Function_TimePlusHour(ScriptClass scriptClass, string command, string argument, out object val)
		{ 
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour;
			int year, mon, day, hour;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out shour);
			if(!scriptClass.GetValueRecurse(shour, shour.Length, out imsi))	return -1;
			hour = (int)imsi;

			TimeUtil.PlusHour(ref year, ref mon, ref day, ref hour);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimePlusHour() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimePlusHour() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimePlusHour() arg3"))	return -1;	
			if(!scriptClass.ChangeNumberVar(shour, hour, "@TimePlusHour() arg4"))	return -1;	
			
			return 1;
		}

		static int Function_TimePlusDay(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday;
			int year, mon, day;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;

			TimeUtil.PlusDay(ref year, ref mon, ref day);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimePlusDay() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimePlusDay() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimePlusDay() arg3"))	return -1;	
			
			return 1;
		}

		static int Function_TimePlusMon(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon;
			int year, mon;
			double imsi=0;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;

			TimeUtil.PlusMonth(ref year, ref mon);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimePlusMon() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimePlusMon() arg2"))	return -1;	
			
			return 1;
		}

		static int Function_TimeMinusSec(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour, smin, ssec;
			int year, mon, day, hour, min, sec;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out shour);
			if(!scriptClass.GetValueRecurse(shour, shour.Length, out imsi))	return -1;
			hour = (int)imsi;
			arg.GetArgument(out smin);
			if(!scriptClass.GetValueRecurse(smin, smin.Length, out imsi))	return -1;
			min = (int)imsi;
			arg.GetArgument(out ssec);
			if(!scriptClass.GetValueRecurse(ssec, ssec.Length, out imsi))	return -1;
			sec = (int)imsi;

			TimeUtil.MinusSecond(ref year, ref mon, ref day, ref hour, ref min, ref sec);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimeMinusSec() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimeMinusSec() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimeMinusSec() arg3"))	return -1;	
			if(!scriptClass.ChangeNumberVar(shour, hour, "@TimeMinusSec() arg4"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smin,  min, "@TimeMinusSec() arg5"))	return -1;	
			if(!scriptClass.ChangeNumberVar(ssec,  sec, "@TimeMinusSec() arg6"))	return -1;	
			
			return 1;
		}

		static int Function_TimeMinusMin(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour, smin;
			int year, mon, day, hour, min;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out shour);
			if(!scriptClass.GetValueRecurse(shour, shour.Length, out imsi))	return -1;
			hour = (int)imsi;
			arg.GetArgument(out smin);
			if(!scriptClass.GetValueRecurse(smin, smin.Length, out imsi))	return -1;
			min = (int)imsi;

			TimeUtil.MinusMin(ref year, ref mon, ref day, ref hour, ref min);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimeMinusMin() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimeMinusMin() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimeMinusMin() arg3"))	return -1;	
			if(!scriptClass.ChangeNumberVar(shour, hour, "@TimeMinusMin() arg4"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smin,  min, "@TimeMinusMin() arg5"))	return -1;	
			
			return 1;
		}

		static int Function_TimeMinusHour(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour;
			int year, mon, day, hour;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out shour);
			if(!scriptClass.GetValueRecurse(shour, shour.Length, out imsi))	return -1;
			hour = (int)imsi;

			TimeUtil.MinusHour(ref year, ref mon, ref day, ref hour);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimeMinusHour() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimeMinusHour() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimeMinusHour() arg3"))	return -1;	
			if(!scriptClass.ChangeNumberVar(shour, hour, "@TimeMinusHour() arg4"))	return -1;	
			
			return 1;
		}

		static int Function_TimeMinusDay(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday;
			int year, mon, day;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out sday);
			if(!scriptClass.GetValueRecurse(sday, sday.Length, out imsi))	return -1;
			day = (int)imsi;

			TimeUtil.MinusDay(ref year, ref mon, ref day);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimeMinusDay() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimeMinusDay() arg2"))	return -1;	
			if(!scriptClass.ChangeNumberVar(sday,  day, "@TimeMinusDay() arg3"))	return -1;	
			
			return 1;
		}

		static int Function_TimeMinusMon(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon;
			int year, mon;
			double imsi;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out syear);
			if(!scriptClass.GetValueRecurse(syear, syear.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out smon);
			if(!scriptClass.GetValueRecurse(smon, smon.Length, out imsi))	return -1;
			mon = (int)imsi;

			TimeUtil.MinusMonth(ref year, ref mon);
			
			if(!scriptClass.ChangeNumberVar(syear, year, "@TimeMinusMon() arg1"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smon,  mon, "@TimeMinusMon() arg2"))	return -1;	
			
			return 1;
		}

		static int Function_TimeGetSunRiseSet(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			double imsi;
			string buf;
			double longitude;
			double latitude;
			int gm;
			string rHour, rMin, rSec, sHour, sMin, sSec;
			SYSTEMTIME tR = new SYSTEMTIME();
			SYSTEMTIME tS = new SYSTEMTIME();
			int year, mon, day;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out imsi))	return -1;
			year = (int)imsi;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out imsi))	return -1;
			mon = (int)imsi;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out imsi))	return -1;
			day = (int)imsi;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out imsi))	return -1;
			longitude = (double)imsi;	// 경도.
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out imsi))	return -1;
			latitude = (double)imsi;	// 위도.
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out imsi))	return -1;
			gm = (int)imsi;	// 위도.

			arg.GetArgument(out rHour);
			arg.GetArgument(out rMin);
			arg.GetArgument(out rSec);
			arg.GetArgument(out sHour);
			arg.GetArgument(out sMin);
			arg.GetArgument(out sSec);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				SunRiseSet.GetTime(year, mon, day, longitude, latitude, gm, tR, tS);
			}
			else 
			{
                tR.Set(DateTimeServer.Now);
                tS.Set(DateTimeServer.Now);
			}
				
			if(!scriptClass.ChangeNumberVar(rHour, tR.wHour, "@TimeGetSunRiseSet() arg7"))	return -1;
			if(!scriptClass.ChangeNumberVar(rMin,  tR.wMinute, "@TimeGetSunRiseSet() arg8"))	return -1;
			if(!scriptClass.ChangeNumberVar(rSec,  tR.wSecond, "@TimeGetSunRiseSet() arg9"))	return -1;
			if(!scriptClass.ChangeNumberVar(sHour, tS.wHour, "@TimeGetSunRiseSet() arg10"))	return -1;
			if(!scriptClass.ChangeNumberVar(sMin,  tS.wMinute, "@TimeGetSunRiseSet() arg11"))	return -1;
			if(!scriptClass.ChangeNumberVar(sSec,  tS.wSecond, "@TimeGetSunRiseSet() arg12"))	return -1;
			
			return 1;
		}

		static int Function_TimeGetWaiting(ScriptClass scriptClass, string command, string argument, out object val)
		{
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            int type;

			val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out type)) return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				DateTime t = DateTime.Now;

                if (type == 1)
                {
                    long old = TimeUtil.GetSecHap(SharedData.dtLastMouseMove);
                    long cur = TimeUtil.GetSecHap(t);

                    val = cur - old;
                }
                else
                {
                    long old = TimeUtil.GetMinHap(SharedData.dtLastMouseMove);
                    long cur = TimeUtil.GetMinHap(t);

                    val = cur - old;
                }
			}

			return 1;
		}

		static int Function_IsHoliday(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			double year=0, month=0, day=0;
			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out year))		return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out month))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out day))		return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				string holi_text;

				val = ConvertTool.ToDouble(Holiday.IsHoliday((int)year, (int)month, (int)day, out holi_text));
			}

			return 1;
		}
        
		static int Function_clock(ScriptClass scriptClass, string command, string argument, out object val)
		{
			val = Environment.TickCount;

			return 1;
		}

		static int Function_Sleep(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf;
			double mili;
			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, buf.Length, out mili))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				Thread.Sleep((int)mili);
			}

			return 1;
		}

		static int Function_TimeFromString(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, shour, smin, ssec;
			//int year, mon, day, hour, min, sec;
			string buf, sdt;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetArgumentString(buf, out sdt))	return -1;
			arg.GetArgument(out syear);
			//if(!scriptClass.GetValueRecurse(syear, ref year))	return -1;
			arg.GetArgument(out smon);
			//if(!scriptClass.GetValueRecurse(smon, ref mon))	return -1;
			arg.GetArgument(out sday);
			//if(!scriptClass.GetValueRecurse(sday, ref day))	return -1;
			arg.GetArgument(out shour);
			//if(!scriptClass.GetValueRecurse(shour, ref hour))	return -1;
			arg.GetArgument(out smin);
			//if(!scriptClass.GetValueRecurse(smin, ref min))	return -1;
			arg.GetArgument(out ssec);
			//if(!scriptClass.GetValueRecurse(smin, ref sec))	return -1;

			DateTime t;
			try 
			{
				t = ConvertTool.ToDateTime(sdt);
			}
			catch 
			{
				t = new DateTime(2000,1,1);
			}
                            
			if(!scriptClass.ChangeNumberVar(syear, t.Year, "@TimeFromString() arg2"))	return -1;
			if(!scriptClass.ChangeNumberVar(smon,  t.Month, "@TimeFromString() arg3"))	return -1;
			if(!scriptClass.ChangeNumberVar(sday,  t.Day, "@TimeFromString() arg4"))	return -1;
			if(!scriptClass.ChangeNumberVar(shour, t.Hour, "@TimeFromString() arg5"))	return -1;	
			if(!scriptClass.ChangeNumberVar(smin,  t.Minute, "@TimeFromString() arg6"))	return -1;	
			if(!scriptClass.ChangeNumberVar(ssec,  t.Second, "@TimeFromString() arg7"))	return -1;
            			
			return 1;
		}


		static int Function_TimeSolarToLunar(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string syear, smon, sday, sleap;
			int year, mon, day;
			string buf;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out year))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out mon))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out day))	return -1;

			arg.GetArgument(out syear);
			arg.GetArgument(out smon);
			arg.GetArgument(out sday);
			arg.GetArgument(out sleap);

			int lyear, lmon, lday, leap;

			SolarLunar.ConvertSolarToLunar(year, mon, day, out lyear, out lmon, out lday, out leap); 

			if(!scriptClass.ChangeNumberVar(syear, lyear, "@TimeSolarToLunar() arg4"))	return -1;
			if(!scriptClass.ChangeNumberVar(smon,  lmon, "@TimeSolarToLunar() arg5"))	return -1;
			if(!scriptClass.ChangeNumberVar(sday,  lday, "@TimeSolarToLunar() arg6"))	return -1;
			if(!scriptClass.ChangeNumberVar(sleap, leap, "@TimeSolarToLunar() arg7"))	return -1;	
            			
			return 1;
		}

		static int Function_TimeSetLocalTime(ScriptClass scriptClass, string command, string argument, out object val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			int year, mon, day, hour,min,sec;
			string buf;

			val = 0;

			arg.Set(argument);

			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out year))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out mon))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out day))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out hour))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out min))	return -1;
			arg.GetArgument(out buf);
			if(!scriptClass.GetValueRecurse(buf, out sec))	return -1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				val = TimeUtil.SetLocalTime(year, mon, day, hour, min, sec);
			}

			return 1;
		}

		//---------------------------------------------------------------------------------
		//	시간과 날짜에 관한 함수.
		//---------------------------------------------------------------------------------

		public static int Function_Time(ScriptClass scriptClass, string command, string argument, out object val)
		{
			if(String.Compare(command, 0, "Time", 0, 4) == 0) 
			{
				/*if(command == "TimePlusMin") 
				{
					return Function_TimePlusMin(scriptClass, command, argument, out val);
				}
				else if(command == "TimePlusSec") 
				{
					return Function_TimePlusSec(scriptClass, command, argument, out val);
				}
				else if(command == "TimePlusHour") 
				{
					return Function_TimePlusHour(scriptClass, command, argument, out val);
				}
				else if(command == "TimePlusDay") 
				{
					return Function_TimePlusDay(scriptClass, command, argument, out val);
				}
				else if(command == "TimePlusMon") 
				{
					return Function_TimePlusMon(scriptClass, command, argument, out val);
				}
				else if(command == "TimeMinusSec") 
				{
					return Function_TimeMinusSec(scriptClass, command, argument, out val);
				}
				else if(command == "TimeMinusMin") 
				{
					return Function_TimeMinusMin(scriptClass, command, argument, out val);
				}
				else if(command == "TimeMinusHour") 
				{
					return Function_TimeMinusHour(scriptClass, command, argument, out val);
				}
				else if(command == "TimeMinusDay") 
				{
					return Function_TimeMinusDay(scriptClass, command, argument, out val);
				}
				else if(command == "TimeMinusMon") 
				{
					return Function_TimeMinusMon(scriptClass, command, argument, out val);
				}
				else if(command == "TimeGetSunRiseSet") 
				{
					return Function_TimeGetSunRiseSet(scriptClass, command, argument, out val);
				}
				else if(command == "TimeGetWaiting") 
				{
					return Function_TimeGetWaiting(scriptClass, command, argument, out val);
				}
				else if(command == "TimeFromString") 
				{
					return Function_TimeFromString(scriptClass, command, argument, out val);
				}
				else if(command == "TimeSolarToLunar") 
				{
					return Function_TimeSolarToLunar(scriptClass, command, argument, out val);
				}
				else if(command == "TimeSetLocalTime") 
				{
					return Function_TimeSetLocalTime(scriptClass, command, argument, out val);
				}
				else 
				{
					if(Tools.IsLangKorean())
						scriptClass.ErrorMessage(String.Format("없는 Time함수 (@{0})", command));
					else
						scriptClass.ErrorMessage(String.Format("Unknowned Time??? method(@{0})", command));

					val = 0;

					return -1;
				}
			}
			else if(command == "IsHoliday") 
			{
				return Function_IsHoliday(scriptClass, command, argument, out val);
			}
			else if(command == "clock") 
			{
				return Function_clock(scriptClass, command, argument, out val);
			}
			else if(command == "Sleep") 
			{
				return Function_Sleep(scriptClass, command, argument, out val);
			}
			else {}

			val = 0;

			return 0;
		}*/

        static int Run_TimeMinusSec(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int minute = (int)args[4];
            int second = (int)args[5];

            TimeUtil.MinusSecond(ref year, ref month, ref day, ref hour, ref minute, ref second);

            args[0] = year;
            args[1] = month;
            args[2] = day;
            args[3] = hour;
            args[4] = minute;
            args[5] = second;

            return 1;
        }

        static int Run_TimeMinusMin(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int minute = (int)args[4];

            TimeUtil.MinusMin(ref year, ref month, ref day, ref hour, ref minute);

            args[0] = year;
            args[1] = month;
            args[2] = day;
            args[3] = hour;
            args[4] = minute;

            return 1;
        }

        static int Run_TimeMinusHour(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];

            TimeUtil.MinusHour(ref year, ref month, ref day, ref hour);

            args[0] = year;
            args[1] = month;
            args[2] = day;
            args[3] = hour;

            return 1;
        }

        static int Run_TimeMinusDay(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];

            TimeUtil.MinusDay(ref year, ref month, ref day);

            args[0] = year;
            args[1] = month;
            args[2] = day;

            return 1;
        }

        static int Run_TimeMinusMon(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];

            TimeUtil.MinusMonth(ref year, ref month);

            args[0] = year;
            args[1] = month;

            return 1;
        }

        static int Run_TimePlusSec(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int minute = (int)args[4];
            int second = (int)args[5];

            TimeUtil.PlusSecond(ref year, ref month, ref day, ref hour, ref minute, ref second);

            args[0] = year;
            args[1] = month;
            args[2] = day;
            args[3] = hour;
            args[4] = minute;
            args[5] = second;

            return 1;
        }

        static int Run_TimePlusMin(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int minute = (int)args[4];

            TimeUtil.PlusMin(ref year, ref month, ref day, ref hour, ref minute);

            args[0] = year;
            args[1] = month;
            args[2] = day;
            args[3] = hour;
            args[4] = minute;

            return 1;
        }

        static int Run_TimePlusHour(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];

            TimeUtil.PlusHour(ref year, ref month, ref day, ref hour);

            args[0] = year;
            args[1] = month;
            args[2] = day;
            args[3] = hour;

            return 1;
        }

        static int Run_TimePlusDay(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];
            int day = (int)args[2];

            TimeUtil.PlusDay(ref year, ref month, ref day);

            args[0] = year;
            args[1] = month;
            args[2] = day;

            return 1;
        }

        static int Run_TimePlusMon(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            int year = (int)args[0];
            int month = (int)args[1];

            TimeUtil.PlusMonth(ref year, ref month);

            args[0] = year;
            args[1] = month;

            return 1;
        }

        static int Run_TimeGetSunRiseSet(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            int year = (int)args[0];
            int mon = (int)args[1];
            int day = (int)args[2];

            double longitude = (double)args[3];
            double latitude = (double)args[4];
            int gm = (int)args[5];

            SYSTEMTIME tR = new SYSTEMTIME();
            SYSTEMTIME tS = new SYSTEMTIME();
            
            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                SunRiseSet.GetTime(year, mon, day, longitude, latitude, gm, tR, tS);
            }
            else
            {
                tR.Set(DateTimeServer.Now);
                tS.Set(DateTimeServer.Now);
            }

            args[6] = tR.wHour;
            args[7] = tR.wMinute;
            args[8] = tR.wSecond;

            args[9] = tS.wHour;
            args[10] = tS.wMinute;
            args[11] = tS.wSecond;

            return 1;
        }

        static int Run_TimeGetWaiting(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            int type = (int)args[0];

            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                DateTime t = DateTime.Now;

                if (type == 1)
                {
                    long old = TimeUtil.GetSecHap(SharedData.dtLastMouseMove);
                    long cur = TimeUtil.GetSecHap(t);

                    val = cur - old;
                }
                else
                {
                    long old = TimeUtil.GetMinHap(SharedData.dtLastMouseMove);
                    long cur = TimeUtil.GetMinHap(t);

                    val = cur - old;
                }
            }

            return 1;
        }

        static int Run_TimeFromString(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string sdt = (string)args[0];

			val = 0;

			DateTime t;

			try 
			{
				t = ConvertTool.ToDateTime(sdt);
			}
			catch 
			{
				t = new DateTime(2000,1,1);
			}

            args[1] = t.Year;
            args[2] = t.Month;
            args[3] = t.Day;
            args[4] = t.Hour;
            args[5] = t.Minute;
            args[6] = t.Second;
            			
			return 1;
        }

        static int Run_TimeSolarToLunar(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            int year = (int)args[0];
            int mon = (int)args[1];
            int day = (int)args[2];

            val = 0;

            int lyear, lmon, lday, leap;

            SolarLunar.ConvertSolarToLunar(year, mon, day, out lyear, out lmon, out lday, out leap);

            args[3] = lyear;
            args[4] = lmon;
            args[5] = lday;
            args[6] = leap;

            return 1;
        }

        static int Run_TimeSetLocalTime(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            int year = (int)args[0];
            int mon = (int)args[1];
            int day = (int)args[2];
            int hour = (int)args[3];
            int min = (int)args[4];
            int sec = (int)args[5];

            val = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                // 윈도우즈에 따라서 관리자 권한으로 실행해야 되는 경우도 있다.
                val = TimeUtil.SetLocalTime(year, mon, day, hour, min, sec);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Time";

            prepare.AddMethod(prename, "TimeMinusMon", "void", new ScriptExternalRun.DeleMethod(Run_TimeMinusMon), "ref:int:year", "ref:int:month");
            prepare.AddMethod(prename, "TimeMinusDay", "void", new ScriptExternalRun.DeleMethod(Run_TimeMinusDay), "ref:int:year", "ref:int:month", "ref:int:day");
            prepare.AddMethod(prename, "TimeMinusHour", "void", new ScriptExternalRun.DeleMethod(Run_TimeMinusHour), "ref:int:year", "ref:int:month", "ref:int:day", "ref:int:hour");
            prepare.AddMethod(prename, "TimeMinusMin", "void", new ScriptExternalRun.DeleMethod(Run_TimeMinusMin), "ref:int:year", "ref:int:month", "ref:int:day", "ref:int:hour", "ref:int:minute");
            prepare.AddMethod(prename, "TimeMinusSec", "void", new ScriptExternalRun.DeleMethod(Run_TimeMinusSec), "ref:int:year", "ref:int:month", "ref:int:day", "ref:int:hour", "ref:int:minute", "ref:int:second");

            prepare.AddMethod(prename, "TimePlusMon", "void", new ScriptExternalRun.DeleMethod(Run_TimePlusMon), "ref:int:year", "ref:int:month");
            prepare.AddMethod(prename, "TimePlusDay", "void", new ScriptExternalRun.DeleMethod(Run_TimePlusDay), "ref:int:year", "ref:int:month", "ref:int:day");
            prepare.AddMethod(prename, "TimePlusHour", "void", new ScriptExternalRun.DeleMethod(Run_TimePlusHour), "ref:int:year", "ref:int:month", "ref:int:day", "ref:int:hour");
            prepare.AddMethod(prename, "TimePlusMin", "void", new ScriptExternalRun.DeleMethod(Run_TimePlusMin), "ref:int:year", "ref:int:month", "ref:int:day", "ref:int:hour", "ref:int:minute");
            prepare.AddMethod(prename, "TimePlusSec", "void", new ScriptExternalRun.DeleMethod(Run_TimePlusSec), "ref:int:year", "ref:int:month", "ref:int:day", "ref:int:hour", "ref:int:minute", "ref:int:second");

            prepare.AddMethod(prename, "TimeGetSunRiseSet", "void", new ScriptExternalRun.DeleMethod(Run_TimeGetSunRiseSet), "in:int:year", "in:int:month", "in:int:day", "in:double:longitude", "in:double:latitude", "in:int:gmh", "out:int:rise_hour", "out:int:rise_minute", "out:int:rise_second", "out:int:set_hour", "out:int:set_minute", "out:int:set_second");
            prepare.AddMethod(prename, "TimeGetWaiting", "int", new ScriptExternalRun.DeleMethod(Run_TimeGetWaiting), "in:int:time_type");
            prepare.AddMethod(prename, "TimeFromString", "void", new ScriptExternalRun.DeleMethod(Run_TimeFromString), "in:string:buffer", "out:int:year", "out:int:month", "out:int:day", "out:int:hour", "out:int:minute", "out:int:second");
            prepare.AddMethod(prename, "TimeSolarToLunar", "void", new ScriptExternalRun.DeleMethod(Run_TimeSolarToLunar), "in:int:year", "in:int:month", "in:int:day", "out:int:lyear", "out:int:lmonth", "out:int:lday", "out:int:leap");
            prepare.AddMethod(prename, "TimeSetLocalTime", "int", new ScriptExternalRun.DeleMethod(Run_TimeSetLocalTime), "in:int:year", "in:int:month", "in:int:day", "in:int:hour", "in:int:minute", "in:int:second");

        }
	}
}
