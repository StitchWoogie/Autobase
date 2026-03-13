using System;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for FUNCTION_BLOCK_DEMAND_CONTROL.
	/// </summary>
	public class DEMAND_TAG 
	{
		public string tag;
		public EnumTagType   tag_type;
		public int[] tag_pos;
	}

	public class FUNCTION_BLOCK_DEMAND_CONTROL 
	{
		public string title;

		public DEMAND_TAG tagCurr = new DEMAND_TAG();
		public DEMAND_TAG tagTarget = new DEMAND_TAG();
		public DEMAND_TAG tagOut = new DEMAND_TAG();
		public DEMAND_TAG tagOut2 = new DEMAND_TAG();
		public DEMAND_TAG tagEcho = new DEMAND_TAG();			// 출력이 되었는가 입력으로 확인한다.
		public DEMAND_TAG tagEOI = new DEMAND_TAG();			// Demand 시작 태그
		public DEMAND_TAG tagStartTarget = new DEMAND_TAG();
		public DEMAND_TAG tagPredictionDisplay = new DEMAND_TAG();		// 예측 전력을 태그로 표시
		public DEMAND_TAG tagPredictionInput = new DEMAND_TAG();		// 예측 전력을 태그에서 가져옴 

		public int  nInclineRingPos;			// 기울기에서 RING pos.
		public sbyte	cOutputType;			// 0 = tagOut으로 ON/OFF출력을 한다. 1 = On은 Tagout으로 OFF = tagout2로 한다.

		public int   nControlTime;
		public int   nControlProtectTime;		// 기계를 너무자주 ON/OFF하게 되면 문제가 생길 수 있으므로 최소한의 금지 시간을 준다.
		public sbyte  bUseIsolation;
		public sbyte  bUseEOI;					// EOI를 사용한다.
		public float fPulseRatio;				// 펄스 정수비
		public double[] data;
		public double[] incline;				// 기울기
		public int   nControlProtectRemain;		// 제어 금지시간이 설정되어 있는경우 0 이 되면 제어금지가 해제된다.

		public int  nCurrentSec;
		public DateTime tStart;		//	Demand Control을 시작한 시간
		public sbyte old_sec;
		public sbyte start_flag;
		public sbyte error_flag;
		public string sErrorMsg;

		public double fPulseOld;
		public double fPulseCount;
		public double fTargetValue;
		public double fValueForecast;
		public double fValueKwh;		// pulse가 정수값으로 환산된 최종값.
		public double fPulseStart;		// 시작할 때의 태그 현재값을 기억해 둔다. 이 변수는 이상한 계기 (값이 떠는 계기)를 보정하기 위해 존재한다.

		public sbyte	bOldEoi;			// 이전의 EOI값.
		public sbyte	bInputAutoReset;	// 전력입력이 자동으로 Reset된다. 

		public int    nPercentY;		// 목표 대비 Y축의 크기.

        public bool   bDatabaseSave;    // 데이터베이스로 저장
        public string sDatabaseDsn;     // Dsn 글자
        public bool bDatabaseTableChecked;  // 데이터베이스 테이블을 체크했다.
	}
}
