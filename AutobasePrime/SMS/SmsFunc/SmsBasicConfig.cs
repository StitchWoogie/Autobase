using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using SMS.SmsComm;

namespace SMS.SmsFunc
{
	/// <summary>
	/// Summary description for SmsBasicConfig.
	/// </summary>
	public class SmsBasic
	{
		static public	ArrayList arrUserSetting = new ArrayList();		// 사용자 정보
		static public	ArrayList arrSendList = new ArrayList();		// 전송정보 파일 리스트
		static public	ArrayList arrSendData = new ArrayList();		// 전송정보 실제 데이터
		static public	ArrayList arrSmsQueue = new ArrayList();		// 전송해야할 문자 메시지

		static public	SMS_MESSAGE_TYPE msgFormat = new SMS_MESSAGE_TYPE();
		static public	smsConfigData smsConfig = new smsConfigData();
		static public	smsUserConfig smsScriptUser = new smsUserConfig();


        //static public string sLineNotifyErrorCode = "";
        static public string sTelegramErrorCode = "";  //20241111 PSU

				
		public SmsBasic()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}

	public class smsUserConfig
	{
		public	bool			bActive;
		public bool				bChange = false;	// 값이 변경되었나	
		public	bool			bReadFlag;
		public	SendSMSData.eCommStatus	eConnectStatus;// 접속상태 저장, 통화중, 선로이상...
		public	int				nUserNo;			// 사용자 순서번호 1 ~ 256
		public	string			userName;		
		public	string			phoneNo;
		public	byte			cManOrAutoConnection;	// 수동 접속이냐?(0), 자동 접속이냐?(1), 반자동?(2)
		public	int				nReadOrder;			// 읽기 순서
		public	bool			bAlarmPriority;
		public	bool			bTagType;
		public	bool			bTagOrder;
		public	bool[]			bPriority = new bool[1000];
		public	int				retry_count;		
		public	int				fail_count;
		public	int				nSendCondition;
		public	string			diTag = "";
		public  int				nSmsType;	// 0 - 문자메시지, 1-음성메시지
        public string           ChatID; //20241111 PSU
	}

	public class sendFileList
	{
		public	bool			bOldData;		// 이전자료 형식인가?
		public	string			filename;
		public	string			text;
	}
	
	public class sendDataList
	{
		public string	pos;
		public string	hour;
		public string	userName;
		public string	phoneNo;	
		public string	data;
        public string   ChatID; //20241111 PSU
	}

	/*public class ALARM_FILE_CLASS
	{
		public DateTime		t;
		public string		tag;// = new byte[40];
		public string		description;// = new byte[80];
		public string		str;// = new byte[80];		
		public UInt16		alarm_type;
		public UInt16		priority; // 경보 우선권 (0~999)
		public UInt16		port;
		public UInt16		station;
		public UInt32		address;
		public UInt16		type;
		public UInt16		crc;
	} */

	public class alarmDataMain
	{	
		//public ALARM_FILE_STRUCT	alarm = new ALARM_FILE_STRUCT();// 자국(로컬)에서 발생된 경보 메세지
		public int		nUserNo;				// 사용자 순서번호 0 ~ 255
		public bool		bManual;				// 수동 = true, 테스트/감시 = false
		public string	userName;
		public string	phoneNo;
		public string	sendPhoneNo = "";		// 보낸 측의 전화번호(로컬은 config에서), 서버에서 보내올 때는 필요하기 때문에, // 빈칸을 입력하여 NULL 인 것을 방지, 웹 서비스 처음 전송시 전송안되는 경우가 발생, 2005-08-31
		public ulong	lParam;					// 발생시 lParam
		public bool		bClient;				// 0 : 자기포트, 1 : client 에서 온 자료, 기존 = char
		public string	dIpAddress;			    // 4자리 클라이언트의 IP 번호
		public string	data;//[MAX_SMS_SEND_DATA];	// 서버에서 올라온 메모리 버퍼, 350 byte
		public int		nSmsType;				// 0-문자,1-음성

        public string ChatID; //20241111 PSU
	}

	public class SMS_MESSAGE_TYPE 
	{
		public bool		bChange = false;	// 값이 변경되었나	
		public bool		bUserDefine;		// 각격 조정 - 0 = 디폴트, 1 = 사용자 정의 (format 에 의해)
		public string	defaultFormat = "&[Time] &[Desc] &[Msg40]";
		public string	format = "&[Time] &[Desc] &[Msg40]";
	}
	

	public class smsConfigData
	{
		//char   sSerialInfo[80];
		public	bool	bChange = false;	// 값이 변경되었나	
		public	int		nPort = 1;
		public	int		nBaud = 115200;
		public	int		nData = 8;
		public	int		nParity = 0;
		public	int		nStop = 1;
		public  int		nRts = 1;
		public  int     nDtr = 1;
		public	string  sTestBuf;
		public	int		nRetryCount = 2;
		public	int		nRetryErrorCount = 3;	
		public	bool	bStopSmsCall;
		public	int		nDelaySiteCall = 3;	// 사용자간
		public	int		nDelayDataCall = 5;	// 
		public	int		nMaxSendChar = 80;	// 보낼 수 있는 최대 문자 수
		public	int		nTimeOutBasic = 5;	// 기본 통신 시간초과
		public	int		nTimeOutMessage = 30;// 메세지 전송확인 시간초과
		public	SendSMSData.eMsgType eMessageType;	// 메세지 전송 형태 보통 = 1, 빠름=2, 긴급=3
		public	string	sSendNumber = "";	// 연락받을 전화번호, 서버에서 보내올 때는 필요하기 때문에, 빈칸을 입력하여 NULL 인 것을 방지, 웹 서비스 처음 전송시 전송안되는 경우가 발생, 2005-08-31 
		//public	int		nSmsType;
		public	SendSMSData.eCdmaType eCdma;// CDMA 종류, LG_MOS, ANY_CALL,...

		public SendSMSData.eConectionType		eConnectType;	//USE_SERVER_STRUCT start
		public bool		bUseServer;
		public byte		cIP1 = 192;
		public byte		cIP2 = 168;
		public byte		cIP3 = 0;
		public byte		cIP4 = 1;
		//public UInt32	dServerAddress;
		public UInt16	wPortNo = 7100;
		public byte		cAckTimeOut = 2; //USE_SERVER_STRUCT server;	// 메세지를 서버로 전송하여 문자메세지 송신

		public	int		nWebServiceErrorCode;// 웹서비스에서 오류 발생시 코드 (오류저장시에 사용)		
		public	Font	fontList = new Font("굴림", 10);
        public byte     cMobiconID = 1;//add 2010-11-18

        public int nMaxCountOfDay = 10000; // 하루에 보낼 수 있는 메시지 총 갯수
		
	};


}
