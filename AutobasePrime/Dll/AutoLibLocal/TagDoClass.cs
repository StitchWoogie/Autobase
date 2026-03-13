using System;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TagDoClass.
	/// </summary>
	/// 
	[Serializable]
	public class TagDoClass : TagPublicClass
	{
		
		public uint 		address;
		public string 		sExtraAddr;		// PLC 기종에 따라 틀리다, FUJI(B, M, D...) MELSEC(X, Y, ...)
		public ushort		wExtraAddr;		// 어드레스 이외에 통신에 필요할 수 있는 값을 넣을수 있게 했다.
		public string 		desON;
		public string 		desOFF;

		public sbyte 		cRelayType;		// 0 - latch, 1 - pulse,
		public ushort		wRelaySecTarget;// relay type 이 pulse 일때 지연시간.

		public sbyte		bReverse;		// 8.10부터추가 반전, 7.0이전에는-wWriteRetryTime으로 사용(지정한 시간이 경과한 후 데이터를 써 준다. 0 - disable , sec)
		public short		nDelaySecON;	// ON 지연시간   8.5.3/9.0.5 부터 지원
		public short		nDelaySecOFF;	// OFF 지연시간  8.5.3/9.0.5 부터 지원

		public sbyte		curr;				// flag ON/OFF;
		public char			reserved;			// 이전에는 count_on_off 로 사용했다.

		public sbyte		cDelayOutputMethod;	// 지정시간(초)뒤에 출력을 한다. 0 - none, 1 - ON, 2 - OFF.
		//public short		nDelayOutputSec;    // 지정시간(초)뒤에 출력을 할 시간.

        public DateTime? DelayExecuteAtUtc;         //  실행 예정 시각 nDelayOutputSec 를 시각으로 변경. 251222 PSU 추가

        public TagDoClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.DO;
		}

        public override object GetCurr()
        {
            return curr;
        }
	}
}
