using System;
using AutoLibLocal;


namespace WatchDog
{
	/// <summary>
	/// Summary description for WatchDogItem.
	/// </summary>
	public class WatchDogItem
	{
		public string sProcessName;
		public string sFileName;
		public string sTitle;
		public int nTimer = 10;
		public EnumWatchDogInfo addr;
		public DateTime dtStart;
		public bool bRunning = false;
		public bool bActive = false;
		public int  nOldSec;
	}
}
