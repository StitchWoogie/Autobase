using System;
using System.Collections;
using NetTools;
using AutoLib;
using AutoLibLocal;
using System.IO;
using DialogHoliday;
using PublicStudioLocalMain.Schedule;
using System.Windows.Forms;
using NetTools.OldDefine;

namespace LocalMain
{
	/// <summary>
	/// Summary description for Schedule.
	/// </summary>
	public class Schedule
	{
		public Schedule()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static ArrayList blockScheduleFixed = new ArrayList();
		public static ArrayList blockScheduleModel = new ArrayList();
		public static ArrayList blockScheduleAdditional = new ArrayList();

		public static SCHEDULE_WEEK_STRUCT[] scheduleWeek = new SCHEDULE_WEEK_STRUCT[ScheduleLib.MAX_SCHEDULE_WEEK];

		public static bool ScheduleGetDayStructure(int year, int mon, int day, out SCHEDULE_STRUCT sc, int week, bool bHoliday, bool bSpecial)
		{
			sc = null;
			if(scheduleWeek[8].block_pos != -1) 
			{
				if(bSpecial) 
				{
					sc = (SCHEDULE_STRUCT)blockScheduleFixed[scheduleWeek[8].block_pos];
					return true;
				}
			}
			if(scheduleWeek[7].block_pos != -1) 
			{
				if(bHoliday) 
				{
					sc = (SCHEDULE_STRUCT)blockScheduleFixed[scheduleWeek[7].block_pos];
					return true;
				}
			}
			if(scheduleWeek[week].block_pos != -1) 
			{
				sc = (SCHEDULE_STRUCT)blockScheduleFixed[scheduleWeek[week].block_pos];
				return true;
			}

			return false;
		}

		public static bool GetModelStructure(string title, out SCHEDULE_MODEL_STRUCT model, out int pos)
		{
			model = null;
			pos = 0;

			int l;
	
			for(l = 0; l < blockScheduleModel.Count; l++) 
			{
				model = (SCHEDULE_MODEL_STRUCT)blockScheduleModel[l];
				if(model.title == title) 
				{
					pos = l;
					return true;
				}
			}

			return false;
		}

		public static bool IsDayInclude(DateTime t, SCHEDULE_ADDITIONAL add, int week, bool bHoliday, bool bSpecial)
		{
			if(add.type == 0) 
			{	// 특정일 운전
				if(bSpecial)	return true;
			}
			else if(add.type == 1) 
			{	// 공휴일 운전
				if(bHoliday)	return true;
			}
            else if (add.type == 3) // 매일 운전
            {	
                return true;
            }
			else 
			{	// 사용자 지정.
				DATE_SOLAR_LUNAR date = new DATE_SOLAR_LUNAR();
				date.syear = t.Year;
				date.smon = (char)t.Month;
				date.sday = (char)t.Day;
				SolarLunar.ConvertSolarToLunar(date);
				return Holiday.IsDayInclude(date, add.user);
			}

			return false;	
		}

		public static bool ModelMakeString(SCHEDULE_MODEL_ITEM_STRUCT item, out string buf)
		{
			ScheduleLib.MakeViewString(item, out buf);
			return true;
		}

        public static void ModelSave()
        {
            ScheduleLib.ModelSave(blockScheduleModel);
        }

        public static object LocalScriptCallBack(string method_name, object[] args)
        {
            if (method_name == "ScheduleSetActive")
            {
                bool old_value = CheckEngineSchedule.bActiveSchedule;
                CheckEngineSchedule.bActiveSchedule = ((int)args[0] == 1);

                FormLocalMain.formMain.Invalidate(true);

                if (old_value == false && CheckEngineSchedule.bActiveSchedule == true)
                {
                    FormSchedule.OnScheduleStructChanged();
                }
            }
            else if (method_name == "ScheduleGetActive")
            {
                return CheckEngineSchedule.bActiveSchedule;
            }
            else
            {
                string msg = String.Format("{0} method not founded. 추가 지원이 필요합니다.", method_name);
                MessageBox.Show(msg, "Method error");
            }

            return 0;
        }

        public static int ByScriptModelGetTime(string model_title, int item_pos, out int hour, out int minute)
        {
            hour = 0;
            minute = 0;

            SCHEDULE_MODEL_STRUCT model = null;
            SCHEDULE_MODEL_ITEM_STRUCT item = null;

            int i;

            for (i = 0; i < blockScheduleModel.Count; i++)
            {
                model = (SCHEDULE_MODEL_STRUCT)blockScheduleModel[i];
                if (model.title == model_title)
                {
                    if (item_pos >= 0 && item_pos < model.blockItem.Count)
                    {
                        item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[item_pos];
                        hour = item.hour;
                        minute = item.minute;
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return 0;
        }

        public static int ByScriptModelSetTime(string model_title, int item_pos, int hour, int minute)
        {
            SCHEDULE_MODEL_STRUCT model = null;
            SCHEDULE_MODEL_ITEM_STRUCT item = null;

            int i;

            for (i = 0; i < blockScheduleModel.Count; i++)
            {
                model = (SCHEDULE_MODEL_STRUCT)blockScheduleModel[i];
                if (model.title == model_title)
                {
                    if (item_pos >= 0 && item_pos < model.blockItem.Count)
                    {
                        item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[item_pos];
                        item.hour = hour;
                        item.minute = minute;

                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            return 0;
        }

        public static void FindFixedScheduleAtWeek()
        {
            int i;
            int l;
            SCHEDULE_STRUCT sc;

            for (i = 0; i < ScheduleLib.MAX_SCHEDULE_WEEK; i++)
            {
                Schedule.scheduleWeek[i].block_pos = -1;
                for (l = 0; l < Schedule.blockScheduleFixed.Count; l++)
                {
                    sc = (SCHEDULE_STRUCT)Schedule.blockScheduleFixed[l];
                    if (Schedule.scheduleWeek[i].title == sc.title)
                    {
                        Schedule.scheduleWeek[i].color = sc.color;
                        Schedule.scheduleWeek[i].block_pos = l;
                        break;
                    }
                }
            }
        }

        // hour와 minute를 Sunrist/Sunset 변환된 시간으로 다시 계산한다.
        public static void CalculateToSunControlTime(SCHEDULE_MODEL_ITEM_STRUCT item, DateTime t)
        {
            if (item.nTimeType == 0) return;

            double longitude, latitude;

            LocationLib.CalcLocationInfomationByTitle(LocationLib.arrayLocation, item.sLocation, out latitude, out longitude);
            SYSTEMTIME tRise = new SYSTEMTIME(), tSet = new SYSTEMTIME();
            SunRiseSet.GetTime(t.Year, t.Month, t.Day, longitude, latitude, 0, tRise, tSet);

            if (item.nTimeType == 1)
            {
                t = new DateTime(t.Year, t.Month, t.Day, tRise.wHour, tRise.wMinute, 0);
            }
            else if (item.nTimeType == 2)
            {
                t = new DateTime(t.Year, t.Month, t.Day, tSet.wHour, tSet.wMinute, 0);
            }

            t = t.AddMinutes(item.nSunBeforeAfterMinutes);

            t = t.ToLocalTime();

            item.hour = t.Hour;
            item.minute = t.Minute;
        }
	}
}
