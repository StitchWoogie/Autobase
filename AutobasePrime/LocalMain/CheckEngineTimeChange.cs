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
		//     	�ð�(��/��/��/��/��/��)�� ���� �Ǿ������� �˻��Ѵ�.
		//----------------------------------------------------------------------------
		
		public static async Task CheckTimeChange()
		{
			DateTime t = DateTime.Now;

			if(tOld.Second != t.Second) 
			{
				SecCheckAI();	// ���÷� �Ƴ��α� �Է� ���� �о�ξ 1�е����� �ڷ�� ����.
			}
			if(tOld.Minute != t.Minute) 
			{
				CheckEngineMinuteChanged.WorkOnBeforeChangeMin(tOld);

				await SendEventToChild.SendEventChangeMinToChild();

                GC.Collect();   // 2014-12-26 �߰���. MessageDisplay���� Label ���� GDI+������ ���� �߰��� ��
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
			if(tOld.Month != t.Month)
			{
				SendEventToChild.SendEventChangeMonthToChild();
				CheckEngineSchedule.ScheduleChangeOnDateChanged();
			}
			if(tOld.Year != t.Year)
			{
				SendEventToChild.SendEventChangeYearToChild();
				CheckEngineSchedule.ScheduleChangeOnDateChanged();
			}

			tOld = t;
		}

		//------------------------------------------------------------------------------
		//	���÷� �Ƴ��α� �Է� ���� �о� �ξ 1�� ������ �ڷ�� ����.
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

                if (ai.nScanCount == 0)  //ó�� ���� �� ���۰��� �����صд�. 20250225 PSU;
                    ai.fMinInitial = ai.curr;

				if(ai.IsPowerFactorTag()) 
				{	// ������ ��
					ai.fMinHap += Math.Abs(ai.curr);	// ���� ����� �д�.
					if(Math.Abs(ai.curr) < Math.Abs(ai.fMinMin))	ai.fMinMin = ai.curr;	// �ּҰ��� �˾Ƴ���.
					if(Math.Abs(ai.curr) > Math.Abs(ai.fMinMax))	ai.fMinMax = ai.curr;	// �ִ밪�� �˾Ƴ���.
				}
				else 
				{
					ai.fMinHap += ai.curr;	// ���� ����� �д�.
					if(ai.curr < ai.fMinMin)	ai.fMinMin = ai.curr;	// �ּҰ��� �˾Ƴ���.
					if(ai.curr > ai.fMinMax)	ai.fMinMax = ai.curr;	// �ִ밪�� �˾Ƴ���.
				}

				ai.nScanCount ++;			// ī��Ʈ�� ���� ��Ų��.
			}
		}

	}
}
