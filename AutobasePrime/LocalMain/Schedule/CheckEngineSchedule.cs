using System;
using System.Collections;
using System.Drawing;
using AutoLib;
using AutoLibLocal;
using GraphicModule;
using NetTools;
using System.IO;
using NetTools.OldDefine;
using DialogHoliday;
using PublicStudioLocalMain.Schedule;
using System.Threading.Tasks;

namespace LocalMain
{
	public class SCHEDULE_GO
	{
		public int pos_model;
		public int pos_item;
		public int hour;
		public int min;
		public bool bDone;	// ON = 운전을 했다.
		public string str;
		public string model;
	}

	/// <summary>
	/// Summary description for CheckEngineSchedule.
	/// </summary>
	public class CheckEngineSchedule
	{
		static CheckEngineSchedule()
		{
			//
			// TODO: Add constructor logic here
			//
			ScheduleLoad();
		}

		public static ArrayList blockScheduleGo = new ArrayList();
        public static bool bActiveSchedule = true;
		
		public static async Task CheckScheduleStatus()
		{
            if (bActiveSchedule == false) return;   

			int l;
			SCHEDULE_GO go;
			DateTime t;

			t = DateTime.Now;
	
			for(l = 0; l < blockScheduleGo.Count; l++) 
			{
				go = (SCHEDULE_GO)blockScheduleGo[l];
				if(go.bDone)	continue;
				if((t.Hour*60+t.Minute) >= (go.hour*60+go.min)) 
				{

					await PlayModelItem(go);
					go.bDone = true;
					FormScheduleToday.UpdateTodayScheduleGoList();
				}
			}
		}

		static async Task PlayModelItem(SCHEDULE_GO go)
		{
			SCHEDULE_MODEL_STRUCT model;
			SCHEDULE_MODEL_ITEM_STRUCT item;

			if(go.pos_model >= Schedule.blockScheduleModel.Count)	return;
			model = (SCHEDULE_MODEL_STRUCT)Schedule.blockScheduleModel[go.pos_model];

			if(go.pos_item >= model.blockItem.Count)	return;

			item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[go.pos_item];

			if(item.blockTag != null) 
			{
				int l;
				SCHEDULE_TAG_VALUE_STRUCT tagValue;

				for(l = 0; l < item.blockTag.Count; l++) 
				{
					tagValue = (SCHEDULE_TAG_VALUE_STRUCT)item.blockTag[l];
					await PlcScan.SetTagValue(tagValue.tag, tagValue.val, false);
				}
			}

			if(item.script != null) 
			{
				ScriptClass control;

				control = new ScriptClass();

				if(control != null) 
				{
					control.SetBuf(item.script);
					await control.RunAsync(TotalConfig.formMain, null);
					if(control.IsError()) 
					{
						string message;
						string sm;
						message = control.GetError();
						sm = String.Format("Schedule Script Error Model={0}, Item={1:00}:{2:00}\n{3}", go.model, go.hour, go.min, message);
						MessageDisplay.Show(sm);
						SmLog.LogError(LogCategory.SYSTEM, sm);
					}
					
				}
			}
		}

		
		public static void ScheduleGoPrepare()
		{
			DateTime t = DateTime.Now;

			blockScheduleGo.Clear();
	
			int  week = TimeUtil.GetWeekDay(t.Year, t.Month, t.Day);

			string text_holiday;
			string text_special;
			SCHEDULE_STRUCT sc;
			SCHEDULE_MODEL_STRUCT model;
			int l;
			int model_pos;
	
			bool bHoliday = Holiday.IsHoliday(t.Year, t.Month, t.Day, out text_holiday);
			bool bSpecial = Holiday.IsSpecialDay(t.Year, t.Month, t.Day, out text_special);

			bool retn = Schedule.ScheduleGetDayStructure(t.Year, t.Month, t.Day, out sc, week, bHoliday, bSpecial);

			if(retn) 
			{
				NAME_STRUCT name;

				for(l = 0; l < sc.blockName.Count; l++) 
				{
					name = (NAME_STRUCT)sc.blockName[l];
					if(!Schedule.GetModelStructure(name.title, out model, out model_pos))	continue;
					AddModelToGoBlock(blockScheduleGo, model, model_pos, t);
				}
			}

			SCHEDULE_ADDITIONAL add;
	
			for(l = 0; l < Schedule.blockScheduleAdditional.Count; l++) 
			{
				add = (SCHEDULE_ADDITIONAL)Schedule.blockScheduleAdditional[l];
				if(Schedule.IsDayInclude(t, add, week, bHoliday, bSpecial)) 
				{
					if(!Schedule.GetModelStructure(add.model, out model, out model_pos))	continue;
					AddModelToGoBlock(blockScheduleGo, model, model_pos, t);
				}
			}
 
			Tools.BlockSort(blockScheduleGo, new Tools.BlockSortFunction(sort_function));

			FormScheduleToday.UpdateTodayScheduleGoList();
		}

		public static void AddModelToGoBlock(ArrayList go_block, SCHEDULE_MODEL_STRUCT model, int pos_model, DateTime t_target)
		{
			int m;
			SCHEDULE_MODEL_ITEM_STRUCT item;
			SCHEDULE_GO go;
			DateTime t_today;

			t_today = DateTime.Now;
	
			for(m = 0; m < model.blockItem.Count; m++) 
			{
				item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[m];

                Schedule.CalculateToSunControlTime(item, t_target);

				go = new SCHEDULE_GO();

				go.pos_model = pos_model;
				go.pos_item = m;
				go.hour = item.hour;
				go.min = item.minute;

                if (t_today.Hour == 0 && t_today.Minute == 0)
                {
                    go.bDone = false;   // 현재시간보다 높은 설정값이어야 제어가 적용되므로 0시0분에 동작하기 위해서 0:0 일때는 제어를 안한것으로 한다.
                }
                else
                {
                    if ((item.hour * 60 + item.minute) > (t_today.Hour * 60 + t_today.Minute)) // 시간이 지난것들은 
                        go.bDone = false;
                    else
                        go.bDone = true;
                }

				Schedule.ModelMakeString(item, out go.str);

				go.model = model.title;

				go_block.Add(go);
			}
		}

		static int sort_function(object b1, object b2)
		{
			SCHEDULE_GO go1 = (SCHEDULE_GO)b1;
			SCHEDULE_GO go2 = (SCHEDULE_GO)b2;

			return (go2.hour*60+go2.min)-(go1.hour*60+go1.min);
		}

		static void ScheduleLoad()
		{
			Schedule.blockScheduleFixed = ScheduleLib.ScheduleLoadFixed();
			Schedule.blockScheduleAdditional = ScheduleLib.ScheduleLoadAdditional();
            Schedule.blockScheduleModel = ScheduleLib.ModelLoad();
	
			Schedule.scheduleWeek = ScheduleLib.ScheduleLoadWeek();

			Schedule.FindFixedScheduleAtWeek();
		}

		// 년, 월, 일 이 바뀌었을 때 이 함수를 불러준다.

		static SYSTEMTIME tOldOnDateChanged = new SYSTEMTIME();

		public static void ScheduleChangeOnDateChanged()
		{
			SYSTEMTIME t = new SYSTEMTIME();

			t.GetLocalTime();

			if(t.wYear == tOldOnDateChanged.wYear && t.wMonth == tOldOnDateChanged.wMonth && t.wDay == tOldOnDateChanged.wDay)	return;

			tOldOnDateChanged.wYear = t.wYear;
			tOldOnDateChanged.wMonth = t.wMonth;
			tOldOnDateChanged.wDay = t.wDay;

			ScheduleGoPrepare();
		}
	}
}

