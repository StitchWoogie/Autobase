using System;

namespace LocalMain
{
	/// <summary>
	/// Summary description for CheckEngineHourChanged.
	/// </summary>
	public class CheckEngineHourChanged
	{
		public CheckEngineHourChanged()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void WorkOnBeforeChangeHour(DateTime t)
		{
			//BroadCastToDDEChangedHour(t->ti_hour);	// DDE server에게 시간이 바뀌었다고 알려준다.	
		}
	}
}
