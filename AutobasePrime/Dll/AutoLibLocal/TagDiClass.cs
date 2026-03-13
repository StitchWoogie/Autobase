using System;
using NetTools;

namespace AutoLibLocal
{
	public struct DI_TREND_REMAIN
	{
		public DateTime t;
		public TREND_DI_STRUCT trend;
	} 
	/// <summary>
	/// Summary description for TagDiClass.
	/// </summary>
	/// 
	[Serializable]
	public class TagDiClass : TagPublicClass
	{
		
		public uint 	address_word;
		public sbyte	address_bit;		// bit address
		public short 	fn;
		public string 	desON="";
		public string 	desOFF="";

		public sbyte	alarm;
		public string 	sSubOutDigital1;	// 입력태그의 신호가 물려있는 출력태그
		public string 	sSubOutDigital2;
		public string 	sSubOutDigitalOnTag;// 입력이 ON이 되었을때 출력할 디지털 태그
		public string 	sSubOutDigitalOffTag; // 입력이 OFF가 되었을 때 출력할 디지털 태그
		public sbyte	bFileSave;				// 데이터 파일을 저장할것이냐???
		public sbyte	cAlarmType;				// 경보를 울리면 어떤 조건에서 울리느냐?
		public string 	sGraphicFile;	 	// 주어진 태그를 가장 잘 확인 할 수 있는 그래픽모듈파일?
		public string 	sAlarmWaveFile;		// 경보 발생 시 사용할 음성 파일
		public ushort	wAlarmPriority; 		// 경보 우선권

		public sbyte	cOutLinkMethod;			// out1, out2로의 출력방법을 정한다.
		public sbyte	cConfirmCount;			// confirm을 할 count default=0 confirm을 하지 않는다.
		public EnumProtectFlag	wProtectFlags;			// 각종 금지 플래그들 
		public sbyte	bReverse;				// I/O 가 반전되어 있다.
		
		public ushort	wScanTime = TagLib.DEFAULT_SCANTIME;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.

		public sbyte		curr;					// flag ON/OFF;
		public short		count_on_off;			// 1분동안 디지털이 ON/OFF 된 횟수 >> ON 변경 횟수로 수정 20241219 PSU
		public sbyte		startOnSec;				// 디지털이 ON될 당시의 초
		//public sbyte		cOnTime;				// 1분동안 총 ON된 시간(초)
        public ushort cOnTime;         // 1분동안 총 ON된 시간 (밀리초 단위) uint16  20241219 PSU
        public DateTime prevTime = DateTime.Now;     // 이전 상태 변경 시간          20241219 PSU
        
		public bool		bCurrentAlarmStatus;	// 현재 경보상태
		public DateTime tEvent;				// event 가 발생한 시간.
		public bool	bNeedAlarmConfirm;		// 경보가 발생하면 이 플래그가 살고 사용자가 확인해 주거나 경보가 복귀되면 이 플래그는 죽는다.

		public int[] nSubOutDigital1 = new int[1];		// 입력태그의 신호가 물려있는 출력태그
		public int[] nSubOutDigital2 = new int[1];
		public int[] nSubOutDigitalOnTag = new int[1];  // 입력이 ON이 되었을때 출력할 디지털 태그
		public int[] nSubOutDigitalOffTag = new int[1]; // 입력이 OFF가 되었을 때 출력할 디지털 태그

		public int			cTrendRemainCount;	// 남아있는 trend count
		[NonSerialized]
		public DI_TREND_REMAIN[]	remain = new DI_TREND_REMAIN[10];			// 남아있는 trend 구조체

		public char cTypeSubOutDigital1;		// 입력태그의 신호가 물려있는 출력태그			DO or GDO
		public char cTypeSubOutDigital2;		//												DO or GDO
		public char cTypeSubOutDigitalOnTag;	// 입력이 ON이 되었을때 출력할 디지털 태그		DO or GDO
		public char cTypeSubOutDigitalOffTag;	// 입력이 OFF가 되었을 때 출력할 디지털 태그	DO or GDO
		public ushort		wScanTimeCurr;				// 0 - 일반 스캔, 그 이외의 값은 지정한 스캔 Time에 따라 움직임.
		public	int		nScanTimeOldMiliSec;

        public DoOnDiClass writeDo = new DoOnDiClass();

        [NonSerialized]
        public PreviewOutputResultDigital previewOutputResult = null;
		
		public TagDiClass()
		{
			//
			// TODO: Add constructor logic here
			//
			this.enumTagType = EnumTagType.DI;
		}

        /// <summary>
        /// NetTools.Tools.CopyObject를 사용하면 편리하나 속도가 느리다.
        /// MemberwiseClone는 속도는 빠르나 클래스 변수는 복사가 되지 않는다.
        /// 해당 클래스 변수는 복사시 원본과 같은 포인트를 가지므로 복사해 주어야 한다.
        /// </summary>
        /// <returns></returns>
        public override object CopyObjectOnStudio()
        {
            TagDiClass obj = (TagDiClass)this.MemberwiseClone();
            obj.writeDo = (DoOnDiClass)obj.writeDo.CopyObjectOnStudio();
            return obj;
        }

        public override object GetCurr()
        {
            return curr;
        }
	}

    [Serializable]
    public class DoOnDiClass
    {
        public uint   address;
        public string sExtraAddr;		// PLC 기종에 따라 틀리다, FUJI(B, M, D...) MELSEC(X, Y, ...)
        public ushort wExtraAddr;		// 어드레스 이외에 통신에 필요할 수 있는 값을 넣을수 있게 했다.

        public sbyte  cRelayType;		// 0 - latch, 1 - pulse,
        public ushort wRelaySecTarget;// relay type 이 pulse 일때 지연시간.

        public short nDelaySecON;	// ON 지연시간   8.5.3/9.0.5 부터 지원
        public short nDelaySecOFF;	// OFF 지연시간  8.5.3/9.0.5 부터 지원

        public sbyte curr;				// flag ON/OFF;

        public sbyte cDelayOutputMethod;	// 지정시간(초)뒤에 출력을 한다. 0 - none, 1 - ON, 2 - OFF.
        //public short nDelayOutputSec;       // 지정시간(초)뒤에 출력을 할 시간.

        public DateTime? DelayExecuteAtUtc;         //  실행 예정 시각 nDelayOutputSec 를 시각으로 변경. 251222 PSU 추가

        public virtual object CopyObjectOnStudio()
        {
            return this.MemberwiseClone();
        }
    }

    public class PreviewOutputResultDigital
    {
        public TimeOutMiliSecClass timeout = new TimeOutMiliSecClass();
        public sbyte new_value;
        public sbyte old_value;
    }
}
