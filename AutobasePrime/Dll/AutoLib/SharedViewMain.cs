using System;
using AutoLibLocal;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AutoLib
{
	/// <summary>
	/// ViewMain에서만 사용하는 환경 설정 파일 들
	/// </summary>
	public class SharedViewMain
	{
		public SharedViewMain()
		{
			//
			// TODO: Add constructor logic here
			//
			
		}

		public static void Prepare()
		{
			
		}

		public delegate void OnEventTagChanged(TagPublicClass tagevent);
		public static event OnEventTagChanged EventListTagChanged;

        public static void EventGoTagChanged(TagPublicClass tagevent)
		{
            if (!TotalConfig.IsMainThread())
            {
                tagevent.bNeedSendEventToChild = true;
                return;
            }

			if(EventListTagChanged == null)	return;

			Delegate[] DelegateList = EventListTagChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
                ((OnEventTagChanged)DelegateList[i])(tagevent);
			}
		}

		public delegate Task OnEventTimer();
		public static event OnEventTimer EventListTimer;

		public static void EventGoTimer()
		{
			if (EventListTimer == null) return;

			Delegate[] DelegateList = EventListTimer.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++)
			{
				((OnEventTimer)DelegateList[i])();
			}
		}

		//public static async Task EventGoTimerAsync()
		//{
		//	if (EventListTimer == null) return;
		//	Delegate[] DelegateList = EventListTimer.GetInvocationList();
		//	var tasks = new List<Task>();

		//	for (int i = 0; i < DelegateList.Length; i++)
		//	{
		//		var handler = (OnEventTimer)DelegateList[i];
		//		// 각 핸들러 실행 전에 안전성 검사
		//		try
		//		{
		//			tasks.Add(handler());
		//		}
		//		catch (ObjectDisposedException)
		//		{
		//			// Dispose된 객체의 핸들러는 제외
		//			continue;
		//		}
		//		catch (InvalidOperationException)
		//		{
		//			// 무효한 상태의 핸들러는 제외
		//			continue;
		//		}
		//	}

		//	if (tasks.Count > 0)
		//	{
		//		await Task.WhenAll(tasks);
		//	} // 모든 핸들러가 완료될 때까지 대기
		//}

		// 병렬처리 대신 순차처리로 변경 , 전역변수(태그값) 등 UI 스레드 경합 및 동시접근 문제 발생 가능.
		public static async Task EventGoTimerAsync()
		{
			if (EventListTimer == null) return;

			foreach (OnEventTimer handler in EventListTimer.GetInvocationList())
			{
				try
				{
					await handler().ConfigureAwait(false);
				}
				catch
				{
					continue;
				}
			}
		}

		public static int GetCountEventGoTimer()
		{
			if(EventListTimer == null)	return 0;
			Delegate[] DelegateList = EventListTimer.GetInvocationList();
			return DelegateList.Length;
		}

		public delegate Task OnEventMinuteChanged();
		public delegate void OnEventHourChanged();
		public delegate void OnEventDayChanged();
		public delegate void OnEventMonthChanged();
		public delegate void OnEventYearChanged();

		public static event OnEventMinuteChanged EventListMinuteChanged;
		public static event OnEventHourChanged   EventListHourChanged;
		public static event OnEventDayChanged    EventListDayChanged;
		public static event OnEventMonthChanged  EventListMonthChanged;
		public static event OnEventYearChanged   EventListYearChanged;

		public static async Task EventGoMinuteChanged()
		{
			if(EventListMinuteChanged == null)	return;

			Delegate[] DelegateList = EventListMinuteChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				await ((OnEventMinuteChanged) DelegateList[i])();
			}
		}

		public static void EventGoHourChanged()
		{
			if(EventListHourChanged == null)	return;

			Delegate[] DelegateList = EventListHourChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((OnEventHourChanged) DelegateList[i])();
			}
		}

		public static void EventGoDayChanged()
		{
			if(EventListDayChanged == null)	return;

			Delegate[] DelegateList = EventListDayChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((OnEventDayChanged) DelegateList[i])();
			}
		}

		public static void EventGoMonthChanged()
		{
			if(EventListMonthChanged == null)	return;

			Delegate[] DelegateList = EventListMonthChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((OnEventMonthChanged) DelegateList[i])();
			}
		}

		public static void EventGoYearChanged()
		{
			if(EventListYearChanged == null)	return;

			Delegate[] DelegateList = EventListYearChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((OnEventYearChanged) DelegateList[i])();
			}
		}

		public delegate void OnEventMainFontChanged();
		public static event OnEventMainFontChanged EventListMainFontChanged;

		public static void EventGoMainFontChanged()
		{
			if(EventListMainFontChanged == null)	return;

			Delegate[] DelegateList = EventListMainFontChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((OnEventMainFontChanged) DelegateList[i])();
			}
		}


		public delegate void DelegatePublic();

		public static event DelegatePublic EventListColorChanged;

		public static void EventGoColorChanged()
		{
			if(EventListColorChanged == null)	return;

			Delegate[] DelegateList = EventListColorChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((DelegatePublic) DelegateList[i])();
			}
		}

		public delegate void DelegateTagPropertyChanged(TagPublicClass tp);
		public static event DelegateTagPropertyChanged EventListTagPropertyChanged;

		public static void EventGoTagPropertyChanged(TagPublicClass tp)
		{
			if(EventListTagPropertyChanged == null)	return;

			Delegate[] DelegateList = EventListTagPropertyChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((DelegateTagPropertyChanged) DelegateList[i])(tp);
			}
		}

		public static event DelegatePublic EventListTagListChanged;

		public static void EventGoTagListChanged()
		{
			if(EventListTagListChanged == null)	return;

			Delegate[] DelegateList = EventListTagListChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((DelegatePublic) DelegateList[i])();
			}
		}


		public static event DelegatePublic EventListUserChanged;

		public static void EventGoUserChanged()
		{
			if(EventListUserChanged == null)	return;

			Delegate[] DelegateList = EventListUserChanged.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				((DelegatePublic) DelegateList[i])();
			}
		}


		public delegate Task DelegateNewAlarm(ALARM_FILE_STRUCT alarm);
		public static event DelegateNewAlarm EventListNewAlarm = null;

		public static async Task EventGoNewAlarm(ALARM_FILE_STRUCT alarm)
		{
			if(EventListNewAlarm == null)	return;

			Delegate[] DelegateList = EventListNewAlarm.GetInvocationList();
			for (int i = 0; i < DelegateList.Length; i++) 
			{
				await ((DelegateNewAlarm) DelegateList[i])(alarm);
			}
		}

		public delegate void DelegateVoidProcAi(TagAiClass ai);
		public static DelegateVoidProcAi procSaveTrendRemainAI = null;

		public static void SaveTrendRemainAI(TagAiClass ai)
		{
			if(procSaveTrendRemainAI == null)	return;
			procSaveTrendRemainAI(ai);
		}

		public delegate void DelegateVoidProcDi(TagDiClass di);
		public static DelegateVoidProcDi procSaveTrendRemainDI = null;

		public static void SaveTrendRemainDI(TagDiClass di)
		{
			if(procSaveTrendRemainDI == null)	return;
			procSaveTrendRemainDI(di);
		}


        public delegate void DelegateAlarmDisplayAI(TagAiClass ai, string message, EnumAlarmType alarm_type, bool retn_or_event, string user, string ip, string computer);
		public static DelegateAlarmDisplayAI procAlarmDisplayAI = null;

		public static void AlarmDisplayAI(TagAiClass ai, string message, EnumAlarmType alarm_type, bool retn_or_event, string user, string ip, string computer)
		{
			if(procAlarmDisplayAI == null)	return;
			procAlarmDisplayAI(ai, message, alarm_type, retn_or_event, user, ip, computer);
		}

        public delegate void DelegateAlarmDisplayDI(TagDiClass di, string message, EnumAlarmType alarm_type, bool retn_or_event, string user, string ip, string computer);
		public static DelegateAlarmDisplayDI procAlarmDisplayDI = null;

        public static void AlarmDisplayDI(TagDiClass di, string message, EnumAlarmType alarm_type, bool retn_or_event, string user, string ip, string computer)
		{
			if(procAlarmDisplayDI == null)	return;
			procAlarmDisplayDI(di, message, alarm_type, retn_or_event, user, ip, computer);
		}


	}

	
}
