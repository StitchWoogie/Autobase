using System;
using NetTools.OldDefine;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ALARM_CONFIRMATION_STRUCT.
	/// </summary>
	[Serializable]
	public class ALARM_CONFIRMATION_STRUCT
	{
		public string tag;
		public string description;
		public int  port;
		//public int  tag_type;
		//public int  tag_pos;
		public string message;
		public SYSTEMTIME t = new SYSTEMTIME();
		public SYSTEMTIME tReturn = new SYSTEMTIME();
		public string sAlarmWaveFile;
		public sbyte screen_method;
		public sbyte bAlarm;			// 현재 Alarm 진행중에 있다. 사용자가 컨펌하면 꺼진다.
        public sbyte bAlarmTag;         // Filter4를 지원하기 위해 추가했다. 실제 태그의 알람여부를 알기위해 추가했다. 2020-12-30
		public ushort msg_type;
        public ushort msg_sub_type;      // 일반경보(11)인 경우 Sub메시지가 구분되어 있다.
		public ushort priority;
		public sbyte bHandConfirm;		// 
		public sbyte bConfirmMethod;	// 0 = 자동으로 복귀, 1 = 수동으로 확인.
		public bool	 bConfirmedAlarmSound = false;	// 사용자가 음성 ACK했다.

        public int id;          // 경보 발생 시 고유한 번호. 2014-7-22 추가

        
	}
}
