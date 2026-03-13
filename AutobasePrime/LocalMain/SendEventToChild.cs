using System;
using AutoLibLocal;
using AutoLib;
using System.Threading.Tasks;

namespace LocalMain
{
	/// <summary>
	/// Summary description for SendEventToChild.
	/// </summary>
	public class SendEventToChild
	{
		public SendEventToChild()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void SendEventAIToChild(TagAiClass ai)
		{
            

			//COMM_EVENT_STRUCT e = new COMM_EVENT_STRUCT();
			//e.tag = ai.tag;
			//e.tag_type = ai.enumTagType;
			//e.message_type = 0;
			//e.tp = ai;
			SharedViewMain.EventGoTagChanged(ai);
		}

		public static void SendEventAOToChild(TagAoClass ao)
		{
			//COMM_EVENT_STRUCT e = new COMM_EVENT_STRUCT();
			//e.tag = ao.tag;
			//e.tag_type = ao.enumTagType;
			//e.message_type = 0;
			//e.tp = ao;
			SharedViewMain.EventGoTagChanged(ao);
		}

		public static void SendEventDIToChild(TagDiClass di)
		{
			//COMM_EVENT_STRUCT e = new COMM_EVENT_STRUCT();
			//e.tag = di.tag;
			//e.tag_type = di.enumTagType;
			//e.message_type = 0;
			//e.tp = di;
			SharedViewMain.EventGoTagChanged(di);
		}

		public static void SendEventDOToChild(TagDoClass dout)
		{
			//COMM_EVENT_STRUCT e = new COMM_EVENT_STRUCT();
			//e.tag = dout.tag;
			//e.tag_type = dout.enumTagType;
			//e.message_type = 0;
			//e.tp = dout;
			SharedViewMain.EventGoTagChanged(dout);
		}

		public static void SendEventSTToChild(TagStClass st)
		{
			//COMM_EVENT_STRUCT e = new COMM_EVENT_STRUCT();
			//e.tag = st.tag;
			//e.tag_type = st.enumTagType;
			//e.message_type = 0;
			//e.tp = st;
			SharedViewMain.EventGoTagChanged(st);
		}

        
		public static void SendEventDoGroupToChild(TagDoGroupClass gdo)
		{
			//COMM_EVENT_STRUCT e = new COMM_EVENT_STRUCT();
			//e.tag = gdo.tag;
			//e.tag_type = gdo.enumTagType;
			//e.message_type = 0;
			//e.tp = gdo;
			SharedViewMain.EventGoTagChanged(gdo);
		}

		public static async Task SendEventChangeMinToChild()
		{
			await SharedViewMain.EventGoMinuteChanged();
		}

		public static void SendEventChangeHourToChild()
		{
			SharedViewMain.EventGoHourChanged();
		}

		public static void SendEventChangeDayToChild()
		{
			SharedViewMain.EventGoDayChanged();
		}

		public static void SendEventChangeMonthToChild()
		{
			SharedViewMain.EventGoMonthChanged();
		}

		public static void SendEventChangeYearToChild()
		{
			SharedViewMain.EventGoYearChanged();	
		}

		public static void SendEventAlarmToChild(ALARM_FILE_STRUCT alarm)
		{
			SharedViewMain.EventGoNewAlarm(alarm);	
		}

		public static void SendEventChangeUserToChild()
		{
			SharedViewMain.EventGoUserChanged();
		}

		public static void SendEventChangeColorToChild()
		{
			SharedViewMain.EventGoColorChanged();
		}

		public static void SendEventChangeFontToChild()
		{
			SharedViewMain.EventGoMainFontChanged();
		}

		public static void SendPaintToChild()
		{
			TotalConfig.formMain.Invalidate(true);
		}
	}
}
