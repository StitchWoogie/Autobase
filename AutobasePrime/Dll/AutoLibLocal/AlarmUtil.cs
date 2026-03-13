using System;
using System.Threading.Tasks;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for AlarmUtil.
	/// </summary>
	public class AlarmUtil
	{
		public AlarmUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public delegate void DelegateAlarmDataSave(TagPublicClass tp, string msg, EnumAlarmType type, string user, string ip, string computer);
		public static DelegateAlarmDataSave procAlarmDataSave = null;

        public static void AlarmDataSave(TagPublicClass tp, string msg, EnumAlarmType type, string user, string ip, string computer)
		{
			if(procAlarmDataSave == null)	return;

			procAlarmDataSave(tp, msg, type, user, ip, computer);
		}
	}
}
