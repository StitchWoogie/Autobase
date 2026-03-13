using System;
using System.Runtime.Serialization;

namespace AutoLibLocal
{
	public class BUTTON_DOUT_STRUCT 
	{
		public string tag;
		public int[]  tag_pos = new int[1];
	}

	public class CONFIG_MULTI_TREND
	{
		public int  wShowUnit = 24;
		public byte cTimeZone = 1;

		public DateTime dtStart = DateTime.Now;
		public sbyte cStartMethod = 0;

		public bool bAutoRange=false;		// 자동으로 Range를 조정해서 본다.
		public bool bAutoGuideLine=false;	// 자동으로 Guide라인을 계산해 준다.
		public bool bDisplayPointDate=false;// 자료시점의 날짜를 표시해 준다.
		public bool bUseLocalRange=false;	// View Range를 각 그래프마다 다른 Range를 사용한다.
		public int  nDataGab = 1;
	}

	public class CONFIG_DATABASE_TREND
	{
		//public int  wShowUnit = 24;
		//public byte cTimeZone = 1;

		public DateTime dtStart = DateTime.Now;
		public sbyte cStartMethod = 0;

		public bool bAutoRange=false;		// 자동으로 Range를 조정해서 본다.
		public bool bAutoGuideLine=false;	// 자동으로 Guide라인을 계산해 준다.
		public bool bDisplayPointDate=false;// 자료시점의 날짜를 표시해 준다.
		public bool bUseLocalRange=false;	// View Range를 각 그래프마다 다른 Range를 사용한다.
		//public int  nDataGab = 1;
		public bool bShowToolTip = false;
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
