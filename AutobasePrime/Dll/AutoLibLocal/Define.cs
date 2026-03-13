using System;
using System.Runtime.Serialization;

namespace AutoLibLocal
{
	[Serializable]
	public class BUTTON_DOUT_STRUCT 
	{
		public string tag;
		public int[]  tag_pos = new int[1];
	}

	[Serializable]
	public class CONFIG_MULTI_TREND
	{
        public int wShowUnit = 60; //24 >> 60 20250306 PSU 변경.
        public byte cTimeZone = 0; // 1 >> 0   20250306 PSU 시간에서 분으로 변경.

		public DateTime dtStart = DateTimeServer.Now;
		public sbyte cStartMethod = 0;

		public bool bAutoRange=false;		// 자동으로 Range를 조정해서 본다.
		public bool bAutoGuideLine=false;	// 자동으로 Guide라인을 계산해 준다.
		public bool bDisplayPointDate=false;// 자료시점의 날짜를 표시해 준다.
		public bool bUseLocalRange=false;	// View Range를 각 그래프마다 다른 Range를 사용한다.
		public int  nDataGab = 1;

        public int nMovePeriod = 1;  //앞주기,뒤주기 이동개수 20250306 PSU 추가
	}

	[Serializable]
	public class CONFIG_DATABASE_TREND
	{
		//public int  wShowUnit = 24;
		//public byte cTimeZone = 1;

        public DateTime dtStart = DateTimeServer.Now;
		public sbyte cStartMethod = 0;

		public bool bAutoRange=false;		// 자동으로 Range를 조정해서 본다.
		public bool bAutoGuideLine=false;	// 자동으로 Guide라인을 계산해 준다.
		public bool bDisplayPointDate=false;// 자료시점의 날짜를 표시해 준다.
		public bool bUseLocalRange=false;	// View Range를 각 그래프마다 다른 Range를 사용한다.
		//public int  nDataGab = 1;
		public bool bShowToolTip = false;
        public int nMovePeriod = 1;  //앞주기,뒤주기 이동개수  20250306 PSU 추가
	}

	/// <summary>
	/// Summary description for Define.
	/// </summary>
	public class Define
	{
		public Define()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
