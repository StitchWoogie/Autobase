using System;
//using System.Data;
using System.IO;
using NetTools;
using System.Collections;
using AutoLibLocal;

namespace AutoLib
{
	public struct COMM_EVENT_STRUCT
	{
		public string tag;
		public EnumTagType tag_type;	// 0- AI, 1 - AO, 2 - DI, 3 - DO
		//public int tag_pos;
		public int message_type;		// 메세지의 형태
										// 0 - 현재치, 1 - 적산치.
		public TagPublicClass tp;
	}

	/// <summary>
	/// Summary description for ShareData.
	/// </summary>
	public class SharedData
	{
		//TerminalClass terminalStruct;

		
		//public static System.Windows.Forms.Form formNavigator=null;
		//public static MessageDisplay formMessageDisplay = null;

		public static UserInfoStruct userInfo = new UserInfoStruct();

		//public static ColorTotalClass colorTotal = new ColorTotalClass();
		//public static AlarmClass alarmClass = new AlarmClass();

		public static DateTime dtLastMouseMove = DateTime.Now;

		//static public string sDirWorkProject;

		public SharedData()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		
	}
}

