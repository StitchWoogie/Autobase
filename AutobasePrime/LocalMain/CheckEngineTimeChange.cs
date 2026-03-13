using System;
using NetTools;
using AutoLib;
using AutoLibLocal;
using System.Threading.Tasks;

namespace LocalMain
{
	/// <summary>
	/// Summary description for CheckEngineTimeChange.
	/// </summary>
	public class CheckEngineTimeChange
	{
		public CheckEngineTimeChange()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static DateTime tOld = DateTime.Now;

		//----------------------------------------------------------------------------
		//     	시간(초/분/시/일/월/년)이 변경 되었는지를 검사한다.
		//----------------------------------------------------------------------------
		
		public static async Task CheckTimeChange()
		{
			DateTime t = DateTime.Now;

			if(tOld.Second != t.Second) 
			{
				SecCheckAI();	// 수시로 아나로그 입력 값을 읽어두어서 1분동안의 자료로 쓴다.
			}
			if(tOld.Minute != t.Minute) 
			{
				CheckEngineMinuteChanged.WorkOnBeforeChangeMin(tOld);

				await SendEventToChild.SendEventChangeMinToChild();

                GC.Collect();   // 2014-12-26 추가함. MessageDisplay에서 Label 에서 GDI+오류가 나서 추가해 봄
			}
			if(tOld.Hour != t.Hour) 
			{
				CheckEngineHourChanged.WorkOnBeforeChangeHour(tOld);
				SendEventToChild.SendEventChangeHourToChild();
			}
			if(tOld.Day != t.Day) 
			{
				SendEventToChild.SendEventChangeDayToChild();
				await CheckEngineDayChanged.WorkOnBeforeChangeDay(tOld);
			}
			if(tOld.Month != tOld.Month) 
			{
				SendEventToChild.SendEventChangeMonthToChild();
				CheckEngineSchedule.ScheduleChangeOnDateChanged();
			}
			if(tOld.Year != tOld.Year) 
			{
				SendEventToChild.SendEventChangeYearToChild();
				CheckEngineSchedule.ScheduleChangeOnDateChanged();
			}

			tOld = t;
		}

		//------------------------------------------------------------------------------
		//	수시로 아나로그 입력 값을 읽어 두어서 1분 동안의 자료로 쓴다.
		//------------------------------------------------------------------------------

		static void SecCheckAI()
		{
			TagAiClass ai;
			int i;

			for(i = 0; i < TagLib.tagListAll.Length; i++) 
			{
				if(TagLib.tagListAll[i].type != EnumTagType.AI)	continue;

				ai = TagLib.GetStructAI(TagLib.tagListAll[i]);

				if(ai.act == 0)	continue;

                if (ai.nScanCount == 0)  //처음 시작 시 시작값을 저장해둔다. 20250225 PSU;
                    ai.fMinInitial = ai.curr;

				if(ai.IsPowerFactorTag()) 
				{	// 역률일 때
					ai.fMinHap += Math.Abs(ai.curr);	// 합을 계산해 둔다.
					if(Math.Abs(ai.curr) < Math.Abs(ai.fMinMin))	ai.fMinMin = ai.curr;	// 최소값을 알아낸다.
					if(Math.Abs(ai.curr) > Math.Abs(ai.fMinMax))	ai.fMinMax = ai.curr;	// 최대값을 알아낸다.
				}
				else 
				{
					ai.fMinHap += ai.curr;	// 합을 계산해 둔다.
					if(ai.curr < ai.fMinMin)	ai.fMinMin = ai.curr;	// 최소값을 알아낸다.
					if(ai.curr > ai.fMinMax)	ai.fMinMax = ai.curr;	// 최대값을 알아낸다.
				}

				ai.nScanCount ++;			// 카운트를 증가 시킨다.
			}
		}

	}
}
