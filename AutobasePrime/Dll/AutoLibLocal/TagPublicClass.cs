using System;
using System.Collections;

namespace AutoLibLocal
{
	[Flags]
	public enum EnumProtectFlag : ushort
	{
		SCAN = 0x0001,
		CONTROL = 0x0002,
		ALARM_DATA = 0x0004,
		ALARM_EVENT = 0x0008,
	}

	public enum EnumTagType
	{
		none = -1,
		AI = 0,
		AO = 1,
		DI = 2,
		DO = 3,
		ST = 9,
		GDO = 10,
		GR = 11,	// Group tag
        SYSTEM = 20,    // __Control_Box Tag와 같이 미리 정해진 태그형식
	}

	/// <summary>
	/// Summary description for TagPublicClass.
	/// </summary>
	/// 

	[Serializable]
	public class TagPublicClass
	{
		public string	tag;
		public string	name;                   // 그룹이 포함될때는 그룹명을 제외한 태그이름
		public string	description;
		public sbyte   	act;

		bool		    bNeedDataCurr;			// 주기적으로 true를 만든다.
		public bool		bChangedDataCurr;		// 스레드에서 결과를 돌려준다.  ViewMain에서 태그 이벤트를 주기위해서 존재한다.

		public EnumTagType enumTagType = EnumTagType.none;

		public byte		cTagLinkType; 				// 0=PLC_SCAN, 1=DDE, 2=가상태그.
		public sbyte	bLocalTag;					// 지역태그로 사용
		// public uint		hSharedTag;				// Shared memory 9.0.11 부터 삭제

        public short    port;
        public short    station;				// plc station

		public string	sDdeService;		// dde service
		public string	sDdeTopic;			// dde Topic
		public string	sDdeItem;			// dde item
		public byte		bDdeRequest;

		public bool		bDdeLinkFlag;				// DdeTag가 접속되었느냐?
		public uint		dwDdeService;				// dde pos
		public uint		dwDdeTopic;					// dde pos
		public uint		dwDdeItem;					// dde pos

		public string	sOpcServer;
		public string   sOpcGroup;
		public string   sOpcItem;
		public int		nOpcItemPos;				// 여러 Array중에서 선택

		public sbyte	bUseAsOutput=0;	            // 출력으로 사용

        // 다음 버전에서 태그 속성이 추가 되었을 때 이전버전에서 태그를 불러서 저장하면 태그속성이 손실될 수 있기 때문에 reserved로 보관하여 다시 저장해 준다.
        // 9.3.3 부터 상위 태그파일 원형보존 지원
        // 9.3.4 부터 뒤는 다시 보류
        public string sScriptTagEvent;              // pbReserved02; 2009-5-6 추가
        public uint flagsOpcServer = 0x0007;        // Reserved03;  2016-4-12 추가. OpcServer가 아니더라도 사용할 수 있다. Bit0-Visible, Bit1-Read, Bit2-Write
        public byte flagOpcUAClient = 0;   // pbReserved04; // OPC UA Flag 24-09-02 추가 hsjeong
        public string pbReserved05;
        public string pbReserved06;
        public string pbReserved07;
        public string pbReserved08;
        public string pbReserved09;
        public string pbReserved10;
        
        // public string pbReservedLast;            마지막에 이상하게 들어가서 일단 보류 9.3.4부터

		public ASSIGN_TAG_STRUCT assign = null;		// 태그가 간접 태그일때만 사용한다.

		public char		bRightOperation;			// 사용자가 이 태그를 운전할 수 있는 권한이 있는냐?

		public bool		bWriteWait = false;			// 외부의 출력 명령 
		public string	sWriteWaitValue;			// 출력 명령 문자열
		public double   fWriteWaitValue;			// 출력 명령 실수
        public string   sWriteUser;
        public string   sWriteIP;
        public string   sWriteComputer;

        public bool bRecurseWrite;                  // 출력이 Recurse반복 되는것을 막기 위해 SetTagValueDelaySec에서 사용한다.
                                                    // 이것이 True이면 Recurse되고 있다는 뜻이므로 더이상 진행하면 안된다.

        public object scriptTagEvent;               // NULL이 아니면 스크립트 클래스이다.

        EnumDeviceQuality eDeviceQuality = EnumDeviceQuality.Good;
        byte eDeviceSubStatus;
        EnumDeviceLimit eDeviceLimit;
        public bool bDevideStatusChanged = false;  // Quality 속성이 변경되었다.

        public bool bNeedSharedTagUpdate = false;   // 값이 변경되었다는 표시. 값이 변경되었을 경우 시간이 오래걸리는 것을 CheckEngineTagChangeThread에서 처리하기 위해서 사용한다.
                                                    // CheckEngineTagChangeThread 에서 처리하고 flag를 false로 바꾸어 준다. 주로 SharedTag에서 레지스트리에 쓸 때 시간이 많이 걸리므로 이때 사용한다.

        public bool bNeedSendEventToChild = false;      // 스레드에서 차일드에 이벤트를 발생하면 위험이 있어서 플래그만 살려놓고 메인에서 발생시킨다.

        public int nIndexOfStruct;                  // 감시프로그램에서 TagLib.MakeTagList()사용시  TagListStruct[]를 만들어 놓을때 태그의 Struct Index를 만들어서 SharedTag에서 사용한다.

        [NonSerialized]
        public SharedRegistryClass sharedRegistry = null;

        public EnumDeviceQuality DeviceQuality
        {
            get { return eDeviceQuality; }
            set
            {
                if (eDeviceQuality != value)
                {
                    eDeviceQuality = value;
                    bDevideStatusChanged = true;
                }
            }
        }

        public byte DeviceSubStatus
        {
            get { return eDeviceSubStatus; }
            set
            {
                if (eDeviceSubStatus != value)
                {
                    eDeviceSubStatus = value;
                    bDevideStatusChanged = true;
                }
            }
        }

        public EnumDeviceLimit DeviceLimit
        {
            get { return eDeviceLimit; }
            set
            {
                if (eDeviceLimit != value)
                {
                    eDeviceLimit = value;
                    bDevideStatusChanged = true;
                }
            }
        }

        public virtual object CopyObjectOnStudio()
		{
			return this.MemberwiseClone();              // 이것은 간단 변수만 복사되고 클래스 변수는 같은 참조를 하기 때문에 클래스 변수는 복사하는 루틴을 추가로 작성해 주어야 한다.
            //return NetTools.Tools.CopyObject(this);   속도가 너무 느리다.
		}

        public bool NeedDataCurr
        {
            set
            {
                bNeedDataCurr = value;

                if (cTagLinkType == 3 && assign != null && value == true) // 간접 태그일 경우는 연결된 태그도 같이 요구한다.
                {
                    TagPublicClass tp = TagLib.GetStructPublic(assign.tag, ref assign.pos);
                    tp.bNeedDataCurr = value;
                }
            }
            get
            {
                return bNeedDataCurr;
            }
        }

        public virtual object GetCurr()
        {
            return 0;
        }

	}

    public enum EnumDeviceQuality : byte
    {
        // 0~3 은 OPC에서 예약 OPC가 아닌 경우는 정의 시 4이상의 값을 사용할 것
        Bad = 0,
        Uncertain = 1,
        Good = 3,
    }

    public enum EnumDeviceSubStatusBad : byte
    {
        // 0~15는 OPC에서 예약. OPC가 아닌 경우는 정의 시 16이상의 값을 사용할 것

        // Quality가 Bad인 경우
        NonSpecific = 0,
        ConfigurationError = 1,
        NotConnected = 2,
        DeviceFailure = 3,
        SensorFailure = 4,
        LastKnownValue = 5,
        CommFailure = 6,
        OutOfService = 7,
        WaitingForInitialData = 8,
    }

    public enum EnumDeviceSubStatusUncertain : byte
    {
        // 0~15는 OPC에서 예약. OPC가 아닌 경우는 정의 시 16이상의 값을 사용할 것
        // Quality가 Uncertain인 경우
        NonSpecific = 0,
        LastUsableValue = 1,
        SensorNotAccurate = 4,
        EngineeringUnitsExceeded = 5,
        SubNormal = 6,

        PlcScanNotRunning = 16,
    }

    public enum EnumDeviceSubStatusGood : byte
    {
        // 0~15는 OPC에서 예약. OPC가 아닌 경우는 정의 시 16이상의 값을 사용할 것
        // Quality가 Good인 경우
        Nonspecific = 0,
        LocalOverride = 6,  // 수동기입
    }

    public enum EnumDeviceLimit : byte
    {
        // 0~3 은 OPC에서 예약 OPC가 아닌 경우는 정의 시 4이상의 값을 사용할 것
        NotLimited = 0,
        LowLimited = 1,
        HighLimited = 2,
        Contant = 3,
    }
}
