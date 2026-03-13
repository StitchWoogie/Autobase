using System;
using NetTools;

namespace AutoLibLocal
{
	public enum EnumTagMember
	{
		TAG_MEMBER_tag = 0,
		TAG_MEMBER_description = 1,
		TAG_MEMBER_port = 2,
		TAG_MEMBER_station = 3,
		TAG_MEMBER_address = 4,
		TAG_MEMBER_fn = 5,
		TAG_MEMBER_unit = 6,
		TAG_MEMBER_desON = 7,
		TAG_MEMBER_desOFF = 8,
		TAG_MEMBER_base = 9,
		TAG_MEMBER_full = 11,
		TAG_MEMBER_sp = 13,
		TAG_MEMBER_hihi = 14,
		TAG_MEMBER_high = 15,
		TAG_MEMBER_low = 16,
		TAG_MEMBER_lolo = 17,
		TAG_MEMBER_act = 18,
		TAG_MEMBER_alarm = 19,
		TAG_MEMBER_bFileSave = 20,
		TAG_MEMBER_fDisplayFormat = 21,
		TAG_MEMBER_cAlarmType = 22,
		TAG_MEMBER_sGraphicFile = 23,
		TAG_MEMBER_sAlarmWaveFile = 24,
		TAG_MEMBER_nCalculateFilter = 25,
	
		TAG_MEMBER_curr = 26,
		TAG_MEMBER_fSumTotal = 27,		// fSumTotal;	현재까지 세어진 유량.

		TAG_MEMBER_viewbase = 28,
		TAG_MEMBER_viewfull = 29,
		TAG_MEMBER_extra1 = 30,
		TAG_MEMBER_extra2 = 31,

		TAG_MEMBER_assign = 32,				// indirect 태그를 assign해 준다.
		TAG_MEMBER_NeedAlarmConfirm = 33,	// 알람 confirm이 필요하다.
		TAG_MEMBER_ProtectScan = 34,		// 스캔 금지중인가의 여부.
		TAG_MEMBER_ProtectControl = 35,		// 콘트롤 금지중인가의 여부.
		TAG_MEMBER_ProtectAlarmEvent = 36,		// 이벤트 금지중인가의 여부.
		TAG_MEMBER_ProtectAlarmData = 37,		// 경보 금지중인가의 여부.

		TAG_MEMBER_fSumPart = 38,		// fSumTotal;	현재까지 세어진 부분 유량.
		TAG_MEMBER_bBcdValue = 39,		// AI/AO		
		TAG_MEMBER_cConfirmCount = 40,	// AI/DI		
		TAG_MEMBER_bCutOverValue = 41,	// AI/AO
		TAG_MEMBER_cTagType = 42,		// AI/AO/DI/DO
		TAG_MEMBER_fAlarmReturnGab = 43,
		TAG_MEMBER_nCalcDelay = 44,
		TAG_MEMBER_sDdeItem = 45,
		TAG_MEMBER_sDdeService = 46,
		TAG_MEMBER_sDdeTopic = 47,

		TAG_MEMBER_sSubOutAnalog = 48,
		TAG_MEMBER_sSubOutAnalogSP = 49,
		TAG_MEMBER_sSubOutDigitalHiHi = 50,
		TAG_MEMBER_sSubOutDigitalLoLo = 51,

		TAG_MEMBER_wAlarmPriority = 52,
		TAG_MEMBER_wProtectFlags = 53,
		TAG_MEMBER_wRateOfChangeLimit = 54,

		TAG_MEMBER_bReverse = 55,
		TAG_MEMBER_cOutLinkMethod = 56,
		TAG_MEMBER_sSubOutDigital1 = 57,
		TAG_MEMBER_sSubOutDigital2 = 58,
		TAG_MEMBER_sSubOutDigitalOffTag = 59,
		TAG_MEMBER_sSubOutDigitalOnTag = 60,

		TAG_MEMBER_cRelayType = 61,
		TAG_MEMBER_wRelaySecTarget = 62,
		//TAG_MEMBER_wWriteRetryTime = 63,

		TAG_MEMBER_cDdeDataFormat = 64,
		TAG_MEMBER_plc_base = 65,
		TAG_MEMBER_plc_full = 66,

		TAG_MEMBER_cAlarmLevelStatus = 70,

		TAG_MEMBER_cAlarmProtectOnBigChangePercent = 71,
		TAG_MEMBER_cAlarmProtectOnBigChangeSecond = 72,
		TAG_MEMBER_bDdeRequest = 73,
		TAG_MEMBER_wScanTime = 74,
		TAG_MEMBER_ID = 75,

		TAG_MEMBER_name = 76,			// tag이름 중에서 . 의 마지막 이름 zone1.ai0000 이면 name=ai0000, tag=zone1.ai0000

        TAG_MEMBER_DeviceQuality = 77,
        TAG_MEMBER_DeviceSubStatus = 78,
        TAG_MEMBER_DeviceLimit = 79,

        TAG_MEMBER_RelayOnDelay = 80,
        TAG_MEMBER_RelayOffDelay = 81,
        TAG_MEMBER_RelayPulseTime = 82,
	}

	// 멤버의 형태
	public enum EnumTagMemberVar
	{
		MEMBER_VAR_int,
		MEMBER_VAR_float,
		MEMBER_VAR_string,
		MEMBER_VAR_double,
        MEMBER_VAR_variable,        // 무슨 형인지 모른다.  2009.6.2
	}

	public enum EnumAnalogLevel
	{
		NORMAL = 0,
		LOLO = 1,
		LOW = 2,
		HIGH = 3,
		HIHI = 4,
        MIDDLE = 5, // low <= curr <= high 같은 중간값이 되었을 때 경보가 되는 경우.
	}

	[Serializable]
	public class ASSIGN_TAG_STRUCT 
	{
		public string	tag; 
		public int[]	pos = new int[1];
	}

	public struct AI_TREND_REMAIN
	{
		public DateTime t;
		public TREND_AI_STRUCT trend;
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class TagAiClass : TagPublicClass
	{
		
		public int 	    address;
		public short 	fn;						// 0 - 계산시 ushort 전체값 사용
		// 1 - 계산시 HIBYTE만 사용
		// 2 - 계산시 LOBYTE만 사용
		public string	unit;
		public double 	fBase;
        public double fFull;
        public double fPlcBase;
        public double fPlcFull;
        public double hihi;
        public double high;
        public double low;
        public double lolo;
		
		public byte		alarm;					// 경보를 울릴것이냐 말것이냐?
		public byte		bFileSave;				// 데이터 파일을 저장할것이냐???
		public string	sSubOutDigitalHiHi;		// HiHi일때 출력할 디지털 태그
		public string	sSubOutDigitalLoLo;		// LoLo일때 출력할 디지털 태그
		public string	sSubOutAnalog; 			// 수치가 바뀌면 무조건 결과를 출력하는 아나로그 출력 태그

		public string	sDisplayFormat = "10.2";// 실제 사용자가 입력한 표시 형식의 원본 9.1.2 부터 추가
		public float	fDisplayFormat;			// 10.2f나 10.2E와 같은 앞의 숫자값
		// 0 - default	10.2f
		// 8.2  2.0  등의 수치를 쓸 수 있다.
		public char		cDisplayFormat;			// 10.2f나 10.2E와 같은 뒤의 문자값
												// { 일때는 신형 방식의 표시형식 {0} 와 같은 C#방식 { 일 때는 sDisplayFormat만을 사용한다. 그이외는 이전 방식
												// { 방식은 9.1.2 부터 추가

		public byte		cAlarmType;				// 경보를 울리면 어떤 조건에서 울리느냐?
		public string	sGraphicFile;	 		// 주어진 태그를 가장 잘 확인 할 수 있는 그래픽모듈파일?
		public string	sAlarmWaveFile;			// 경보 발생 시 사용할 음성 파일
		public short	nCalculateFilter;		// 계산 방법, 0 - 보통의 계산
		public string	sSubOutAnalogSP;		// 아날로그 출력(이 아날로그 출력을 변경하면 아날로그 입력도 바뀐다.
		public byte		bCutOverValue;			// 아날로그 값의 계산치가 RANGE를 벗어났을 때는 값을 최대 최소로 잘라주는 옵션.
        public double view_full;				// 보여주는 최고 범위
        public double view_base;              // 보여주는 최소 범위
		public short	nCalcDelay;				// 값을 읽을때 지난값과 평균하여 사용. 0-사용하지 않는다.
		public ushort	wAlarmPriority; 		// 경보 우선권
				
		public byte		cConfirmCount;			// confirm을 할 count default=0 confirm을 하지 않는다.
		public byte		bBcdValue;				// BCD로 읽을 것인가?
		public float	fAlarmReturnGab;		// 알람에서 복귀할때 gab만큼 낮추거나 높인 수치에서 복귀한다. 
		public ushort	wRateOfChangeLimit;		// 이전값과 비교해서 지정해 놓은 값 이상이 차이나면 경보를 울린다. 0 = 사용안함.
		public EnumProtectFlag	wProtectFlags;	// 각종 금지 플래그들
		public byte		cAlarmProtectOnBigChangePercent;	// 과 변화가 되었을 때 경보를 지연시키는 기능
		public short	nAlarmProtectOnBigChangeSecond;		// 과 변화가 되었을 때 경보를 지연시키는 기능   2013-6-28 sbyte에서 short로 변경 (최대 3600으로 변경)
		public ushort	wScanTime = TagLib.DEFAULT_SCANTIME;	// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.
		public float	fScrollUnit = 1;		// 스크롤 바를 사용할 때의 단위 기본은 1으로 설정 9.1.3 부터 지원

        public string   sCalcScript;            // 계산용 스크립트
		
		
		public double		curr;
		public double		old;
        public double       real_curr;      // 비례식이나 각종 계산을 하지 않은 콘트롤러 원시의 값. 2021-8-3 확인해 보니 AnalogStatus 비트 연산에서만 사용하고 있다.
		public EnumAnalogLevel	cAlarmLevelStatus;
		public EnumAnalogLevel	cSubCheckLevelStatus;
		public bool		bNeedAlarmConfirm;		// 경보가 발생하면 이 플래그가 살고 사용자가 확인해 주거나 경보가 복귀되면 이 플래그는 죽는다.

		public int[]  		nSubOutDigitalHiHi = new int[1];		// HiHi일때 출력할 디지털 태그
		public int[]  		nSubOutDigitalLoLo = new int[1];		// LoLo일때 출력할 디지털 태그
		public int[]  		nSubOutAnalog = new int[1]; 			// 수치가 바뀌면 무조건 결과를 출력하는 아나로그 출력 태그
		public int[]  		nSubOutAnalogSP = new int[1];		// 아날로그 출력(이 아날로그 출력을 변경하면 아날로그 입력도 바뀐다.

		public short		nScanCount;					// 적산을 하기위해 1분동안 변한 아나로그 횟수
		public double		fMinHap;   					// 적산을 1분동안 더해 놓은값
		public double		fMinMin;					// 1분동안 측정된값 중에서 최소값
		public double		fMinMax;					// 1분동안 측정된값 중에서 최대값
		public double		fSumTotal;					// 현재까지 세어진 유량
		public double		fSumPart;     				// 부분별로 세어진 유량

        public double       fMinInitial; //초기 값 20250225 PSU
        public byte         bAccumulatedValue; // 누적값 태그 20250225 PSU
        public bool bHasSavedOnce;  //20250717 PSU  자료저장 1회 후 true

        public DateTime		tSumTotal;
		public DateTime		tSumPart;

		public double	fAlarmMaxValue;				// 알람이 발생하고 난 뒤 최고로 올라간 값, 경보출력시 최고값으로 쓴다.
		public double	fAlarmMinValue;				// 알람이 발생하고 난 뒤 최저로 내려간 값, 경보출력시 최저값으로 쓴다.
		public bool		bTagChangeFlag;				// 태그 속성이 바뀌었다(범위나). 그래서 새로운 계산을 화면에 표시해 줄필요가 있다.
		public bool		bCurrentAlarmStatus;		// 현재 경보상태

		public int				nTrendRemainCount;	// 남아있는 trend count
		[NonSerialized]
		public AI_TREND_REMAIN[]		remain = new AI_TREND_REMAIN[10] ;//10			// 남아있는 trend 구조체

		public char  	cTypeSubOutDigitalHiHi;		// HiHi일때 출력할 디지털 태그
		public char  	cTypeSubOutDigitalLoLo;		// LoLo일때 출력할 디지털 태그

		public double	sosu_curr;					// AI 계산시 표시형식과 일치를 사용하게 되면 누적측정이 되지 않는다. 그 문제를 해결하기위한 변수
		public double	sosu_old;					// AI 계산시 표시형식과 일치를 사용하게 되면 누적측정이 되지 않는다. 그 문제를 해결하기위한 변수 2015-9-3 WritePreDisplay 추가하면서 삭제함.

		public short	nAlarmProtectOnBigChangeCount;	// 과 변화가 되었을 때 경보를 지연시키는 기능에서 사용하는 기능 2013-6-28 sbyte에서 short로 변경
		public sbyte	cAlarmProtectOnBigChangeOldSec;	// 과 변화가 되었을 때 경보를 지연시키는 기능에서 사용하는 기능

		public ushort	wScanTimeCurr;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.
		public	int		nScanTimeOldMiliSec;		

		public bool PreReadFlag;
		public uint PreReadReal;
		public double PreReadDouble;

        public short nMemoryTypeSub;    // fn에서 부가적으로 사용할 값 메모리에서 값을 조합할 수 있다.

        public AoOnAiClass writeAo = new AoOnAiClass();

        [NonSerialized]
        public PreviewOutputResultAnalog previewOutputResult = null;
		
		public TagAiClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.AI;

            // 초기값을 현재시간으로 설정한다. Total과 Part를 모두 같게 한다.
            this.tSumTotal = DateTime.Now;
            this.tSumPart = tSumTotal;
		}

		/// <summary>
		/// 역률 태그인가를 검사한다.
		/// </summary>
		/// <returns></returns>
		public bool IsPowerFactorTag()
		{
			if(nCalculateFilter == 1 || nCalculateFilter == 3 || nCalculateFilter == 4)		return true;
			return false;
		}

        /// <summary>
        /// NetTools.Tools.CopyObject를 사용하면 편리하나 속도가 느리다.
        /// MemberwiseClone는 속도는 빠르나 클래스 변수는 복사가 되지 않는다.
        /// 해당 클래스 변수는 복사시 원본과 같은 포인트를 가지므로 복사해 주어야 한다.
        /// </summary>
        /// <returns></returns>
        public override object CopyObjectOnStudio()
        {
            TagAiClass obj = (TagAiClass)this.MemberwiseClone();
            obj.writeAo = (AoOnAiClass)obj.writeAo.CopyObjectOnStudio();
            return obj;
        }

        public override object GetCurr()
        {
            return curr;
        }
	}

    [Serializable]
    public class AoOnAiClass
    {
        //public short port;
        //public short station;		// plc station
        public uint address;
        public string sExtraAddr;	// PLC 기종에 따라 틀리다, FUJI(B, M, D...) MELSEC(X, Y, ...)
        public ushort wExtraAddr;	// 어드레스 이외에 통신에 필요할 수 있는 값을 넣을수 있게 했다.
        //public short fn;
        //public string unit;
        //public float fBase;
        //public float fFull;
        public short nCalculateFilter;		// 계산 방법, 0 - 계산하지 않고 그대로 출력
        //public float plc_base;				// 출력 최소값
        //public float plc_full;				// 출력 최대값

        public sbyte cDdeDataFormat;			// DDE 출력시 데이터 형태
        //public sbyte bBcdValue;				// BCD로 출력할 것인가?
        //public sbyte bCutOverValue;			// 초과치는 자른다.
        public string sCalcScript;            // 계산용 스크립트

        public double curr;
        public double old;
        public ushort real_curr;

        // 이 클래스 속에 클래스 변수가 선언 되면 복제가 되지 않으므로 
        public virtual object CopyObjectOnStudio()
        {
            return this.MemberwiseClone();      
        }
    }

    public class PreviewOutputResultAnalog
    {
        public TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();
        public double new_value;
        public double old_value;
    }
    
}
