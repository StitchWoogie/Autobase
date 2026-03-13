using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AutoLibLocal
{
	public enum EnumEventCategory : short
	{
		NONE = 0,
		SCRIPT = 1,
	}

	// 0~65535 까지만 허용됨
	public enum EnumEventID
	{
		None = 0,

		ProgramStart = 1,
		ProgramEnd = 2,
        SystemStart = 20,
        SystemStop = 21,

        // 사용자
        UserLogin = 10,
        UserLogout = 11,

        // 데이터
        DataSave = 30,
        DataDelete = 31,
        DataExport = 32,
        DataImport = 33,

        // 통신
        CommError = 40,
        CommRecover = 41,
        DeviceError = 42,
        DeviceRecover = 43,

        // 레벨
        Warning = 100,
        Error = 101,
        Critical = 102,

       // Error = 50000,	// 모든 오류는 50000으로 시작

		// 51000~51999,
		ErrorScript = 51000,	// 스크립트 오류
	}
	/// <summary>
	/// Summary description for EventSave.
	/// </summary>
	public class EventSave
	{
		static string sSource;
		static bool bEventLog = TotalConfig.LoadRegAutoBaseConfig("EventLog", null, "Active", false);

		static EventSave()
		{
			//
			// TODO: Add constructor logic here
			//

			if(bEventLog) 
			{
    			string oem = TotalConfig.AutoBaseIniGetOemProgramName();

                oem += " Hey";

				sSource = oem+" "+Application.ProductName;

                try
                {
                    if (!EventLog.SourceExists(sSource))
                    {
                        EventLog.CreateEventSource(sSource, oem);
                    }
                }
                catch
                {
                    bFail = true;
                }
			}
		}

		static bool bFail = false;

		static void Save(string s, EventLogEntryType type, EnumEventID id, EnumEventCategory c)
		{
			if(!bEventLog)	return;
			if(bFail)		return;	// 한번 이벤트 오류가 발생하면 계속 발생할 수 있으므로 돌아간다.

			EventLog myLog = new EventLog();
			myLog.Source = sSource;
        
			try 
			{
				myLog.WriteEntry(s, type, (int)id, (short)c);
			}
			catch
			{
				bFail = true;
			}
		}

		public static void Error(string s)
		{
			Save(s, EventLogEntryType.Error, EnumEventID.Error, EnumEventCategory.NONE);
		}

		public static void Error(string s, EnumEventID id)
		{
			Save(s, EventLogEntryType.Error, id, EnumEventCategory.NONE);
		}

		public static void Error(string s, EnumEventCategory c)
		{
			Save(s, EventLogEntryType.Error, EnumEventID.Error, c);
		}

		public static void Error(string s, EnumEventID id, EnumEventCategory c)
		{
			Save(s, EventLogEntryType.Error, id, c);
		}

		public static void Log(string s)
		{
			Save(s, EventLogEntryType.Information, EnumEventID.None, EnumEventCategory.NONE);
		}

		public static void Log(string s, EnumEventID id)
		{
			Save(s, EventLogEntryType.Information, id, EnumEventCategory.NONE);
		}
	}
}
