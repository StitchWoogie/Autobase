#if	!defined (__PLC_SCAN_H)
#define	__PLC_SCAN_H



#include "..\catlib.src\main_scan.h"

#if	!defined (__COMPILER_HPP)
#include "..\..\Wintools\INCLUDE\compiler.hpp"
#endif

#if	!defined (__TOOLS_H)
#include "..\..\Wintools\INCLUDE\tools.h"
#endif

#include "..\..\Wintools\INCLUDE\Crypto.h"

#include <afxsock.h>

#pragma pack(push, 1)

#define	WM_SOCKET_MSG	(WM_USER+1)

#define	MENU_POS_WINDOW	3

enum {
	COMMUNICATION_OK = 1,			// communication end end
	COMMUNICATION_WAITING = 2,		// 통신이 지금 진행중이다.
	COMMUNICATION_OK_NOCOUNT = 3,	// 통신이 정상적으로 종료되었다. 다음 통신 카운트는 하지 않는다.

	// error code
	COMMUNICATION_CODE_BAD = 1000,		// 통신에 코드 불량 발생하다.
	COMMUNICATION_TIME_OUT = 1001,		// 통신에 시간 초과 발생하다.

	COMMUNICATION_ERR_BAD_COMMAND = 2000,	// 컴퓨터에서 알수 없는 명령을  보냈다.
	COMMUNICATION_ERR_BAD_STATION = 2001,	// 컴퓨터에서 알수 없는 스테이션을 보냈다.
	COMMUNICATION_ERR_BAD_ADDRESS = 2002,	// 컴퓨터에서 알수 없는 주소를 요구했다.
	COMMUNICATION_ERR_SIZE_TOO_BIG = 2003,	// 한번에 읽는 크기가 너무 크다.
	COMMUNICATION_ERR_BAD_CRC = 2004,		// 컴퓨터에서 보낸 명령어의 CRC가 틀리다.
	COMMUNICATION_ERR_RESET = 2005,			// 콘트롤라의 RESET에 실패했다.
	COMMUNICATION_ERR_STRING = 2006,		// 콘트롤라의 특성에 맞는 오류 메세지.
	COMMUNICATION_ERR_STRING_AND_CODEBAD = 2007,	// 콘트롤라의 특성에 맞는 오류 메세지와 코드불량처리.
	COMMUNICATION_ERR_STRING_AND_TIMEOUT = 2008,	// 콘트롤라의 특성에 맞는 오류 메세지와 시간초과처리.


	COMMUNICATION_ERR_CONTROLLER = 2100,	// 콘트롤러에 이상이 생겼다.

	// programm error code
	COMMUNICATION_UNDEFINED_DEVICE 	= 10000,			// 디바이스가 모두 완성되지 않았다. (PROGRAMM Error)
	COMMUNICATION_UNDEFINED_PROTOCOL = 10001,			// 프로토콜이 모두 완성되지 않았다. (PROGRAMM Error)
	COMMUNICATION_PROGRAMM_NOT_MAKED = 10002,			// 이부분의 프로그램이 완성되지 않았다.

	COMMUNICATION_LOCAL_PROTOCOL_MEMORY_NOT_ALLOCATED,	// local 프로토콜 메모리가 할당되지 않았다.

	COMMUNICATION_UNDEFINED_ERROR = 32761,				// 지정되지 않은 오류번호가 발생 했다.
	COMMUNICATION_NEXT_WRITE_GO = -1,					// 다음에 있는 쓰기를 바로 실행한다.
};

// 주의: 아래의 station 구조체의 크기를 바꿀때는 통신 DLL 함수들과 연관이 있으므로
//  		통신 DLL도 체크할 수 있도록 한다.

typedef struct {
	WORD struct_size;		// structure size
	char cVarType;			// 0 - WORD, 1 - FLOAT
	WORD station;   		// station
	char sStation[20];		// 위의 station과 같은 값을 사용한다. 즉 string으로도 읽고 WORD로도 동시에 변환시켜 놓는다. (sStation은 DDE에서 topic으로 활용한다.)
	char type[40];		  	// AI, AO, DI, DO, or B,M,CR(fuji), or X,Y,M(melsel),
							// 위와 같이 PLC 고유 특정 주소의 구분으로 쓸 수 있다.
							// 현재 30으로 되어있다. 30인 이유는 CCIM DDE Item이 거의 30바이트 정도 차지하므로.
	DWORD address;   	  	// read position
	WORD target;			// save to buf positon
	WORD size;
	DWORD extra2;
	DWORD extra3;
	char active;

	//char reserved1;

	HANDLE local;			// 각 라인별 특성인데 프로토콜 마다 다르게 설정해서 사용한다. 
							// 이전에는 DWORD로 선언되어 있었는데 2002-1-13 HANDLE로 바꾸었다.

	int	 nTimeOutSkipTime;	// timeout이 발생했을 때 현재 지연중인 시간.
	int	 nTimeOutCount;		// timeout이 발생한 횟수.
	char cTimeOutOldSec;	// timeout count

	char bHexStation;		// 스테이션 설정이 00h방식이다.
	char bHexAddress;		// ADDRESS 설정이 00h방식이다.
	char read_delay_target;	// 각 스캔별 Delay Time을 추가할 수 있다.
	char read_delay_curr;	
	char reserved[80];		// 나중에 추가될 부분을 위하여.
} SCAN_METHOD_STRUCT;

//#define	MAX_PORT		256				
extern int 	MAX_PORT;

#define	MAX_ATTR_WORD	1000		// 포트 특성을 저장하는 공간.
#define	MAX_SEND_BUF	256
#define	MAX_RECV_BUF	1024

typedef struct {
	char	 flag;			// 현재 디바이스를 사용중인가?
	short 	 nDeviceStyle;	// 사용중인 디바이스
	char     *pData;
} DEVICE_STRUCT;

typedef struct {
	WORD struct_size;					// struct size
	SCAN_METHOD_STRUCT *scanMethod;
	int	 nScanMethodHap;

	WORD_BUF  *bufWORD;					// WORD buf
	int  nBufSizeWORD;

	FLOAT_BUF *bufFLOAT;				// FLOAT buf
	int  nBufSizeFLOAT;

	DWORD_BUF *bufDWORD;				// DWORD buf
	int  nBufSizeDWORD;

	int	 no;							// 현재 스트럭쳐 구조체의 pos 값
	int	 nScanPos;						// 현재 scan중인 위치.

	HLOCAL hLocalProtocol;				// 각 프로토콜마다 틀린 설정이 있을때는 이것을 
										// 할당하여 사용한다.
	char sScanProtocolOption[80];		// scan protocol	

	//int		reserved2;//					nUserProtocolID;				// protocol이 user-일때 사용하는 user protocol의 id

	char	bReadingFlag;				// 현재 읽기를 하고 있는 중이다.
	int		commCountNeed;				// 읽기 원하는 count수
	int		commCountCurr;				// 현재까지 읽은 개수
	BYTE	commRecvBuf[MAX_RECV_BUF];	// 받는 버퍼
	BYTE	commSendBuf[MAX_SEND_BUF];	// 보내는 버퍼
	int		commCountSend;				// 보낸 통신 버퍼 크기

	TimeOutClass *timeout;				// timeout class
	int		MAX_TIME_OUT_READ;			// 읽기 시  사용하는 MAX_TIME_OUT  최소 2초이상은 되어야 한다. 
	int		MAX_TIME_OUT_WRITE;			// 쓰기 시  사용하는 MAX_TIME_OUT  최소 2초이상은 되어야 한다. 

	DEVICE_STRUCT device;				// 

	STRING_BUF  *bufSTRING;				// STRING buf
	int  nBufSizeSTRING;

	int   nLocalReadScanTime;			// 각 Local Read ScanTime		읽기마다 지연시간
	int   nLocalWriteScanTime;			// 각 Local Write ScanTime		쓰기마다 지연시간
	
	char	*sPlcScanErrorString;		// DLL에 static으로 존재했었는데 이후에 바뀌었다. 8.4.3부터 추가 되었다.

	DOUBLE_BUF  *bufDOUBLE;				// DOUBLE buf
	int  nBufSizeDOUBLE;

	INT64_BUF  *bufINT64;				// INT64 buf
	int  nBufSizeINT64;

	int		TIMEOUT_MILLI_READ;			// 읽기 시  사용하는 Milli Sec Timeout  10.2 부터 지원 Milli Sec 를 지원하려면 통신 드라이버에서 다시 지원해야 한다.
	int		TIMEOUT_MILLI_WRITE;		// 쓰기 시  사용하는 Milli Sec Timeout  10.2 부터 지원 Milli Sec 를 지원하려면 통신 드라이버에서 다시 지원해야 한다.
	
	char	reserved[52];				// 나중을 위하여 예약
} LOCAL_PORT_STRUCT;

typedef void (WINAPI* LPFNPROTOCOLDRAWMETHODTITLE) (HDC hdc, int x, int y);
//typedef void (WINAPI* LPFNPROTOCOLDRAWMETHODOLD) (HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
typedef void (WINAPI* LPFNPROTOCOLDRAWMETHOD) (LOCAL_PORT_STRUCT *pt, HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm);
typedef int  (WINAPI* LPFNPROTOCOLREAD) (LOCAL_PORT_STRUCT *pt, int pos);
typedef int  (WINAPI* LPFNPROTOCOLWRITEWORD) (LOCAL_PORT_STRUCT *pt, int ss, DWORD address, long double value, char *extra1, DWORD extra2, char *sAddress);
typedef int  (WINAPI* LPFNPROTOCOLWRITEBIT)  (LOCAL_PORT_STRUCT *pt, int ss, DWORD address, WORD flag, char *extra1, DWORD extra2, char *sAddress);
typedef int  (WINAPI* LPFNPROTOCOLWRITEBLOCK)  (LOCAL_PORT_STRUCT *pt, int ss, DWORD address, BYTE *value, char *extra1, DWORD extra2, char *sAddress, short arraysize, BYTE array_type); // 10.2.0 부터 추가 Protocol 8.8 부터 사용 가능
typedef int  (WINAPI* LPFNPROTOCOLINIT)  (HWND hwnd, LOCAL_PORT_STRUCT *pt);
typedef int  (WINAPI* LPFNPROTOCOLUNINIT)  (LOCAL_PORT_STRUCT *pt);
typedef int  (WINAPI* LPFNPROTOCOLGETERRMSG) (char *err);
typedef int  (WINAPI* LPFNPROTOCOLGETERRMSG2) (LOCAL_PORT_STRUCT *pt, char *err);
typedef int  (WINAPI* LPFNSETPROC) (FARPROC proc);
typedef int  (WINAPI* LPFNPROTOCOLCHECKSTRUCT) (WORD local_port_size, WORD scan_method_size, WORD device_struct_size);

typedef struct {
	HINSTANCE hInst;			// dll의 핸들
	//typedef DWORD (WINAPI* LPFNCOMMDLGEXTENDEDERROR) (void);
	LPFNPROTOCOLDRAWMETHODTITLE ProtocolDrawMethodTitle;
	//LPFNPROTOCOLDRAWMETHODOLD   ProtocolDrawMethodOld;
	LPFNPROTOCOLDRAWMETHOD		ProtocolDrawMethod;
	LPFNPROTOCOLREAD            ProtocolRead;
	LPFNPROTOCOLWRITEWORD       ProtocolWriteWord;
	LPFNPROTOCOLWRITEBIT        ProtocolWriteBit;
	LPFNPROTOCOLINIT            ProtocolInit;
	LPFNPROTOCOLUNINIT          ProtocolUnInit;
	LPFNPROTOCOLGETERRMSG       ProtocolGetErrMsg;
	LPFNPROTOCOLGETERRMSG2       ProtocolGetErrMsg2;
	LPFNPROTOCOLWRITEBLOCK      ProtocolWriteBlock;
} DLL_PROTOCOL_PROC;			// DLL protocol에서 사용하는 구조체.

typedef struct {
	char	sTelNumber[20];			// Tel Number
	int		nConnectCicle;			// 접속 주기
	int		nConnectingTime;		// 접속시간.
	char	bAutoConnection;		// 자동/수동접속 여부
	int     nConnectingTimeOnManual;// 수동 접속시 접속시간.
} TELEPHONE_STRUCT;

typedef struct {
	long lCountTimeOut;				// timeout count
	long lCountCodeBad;   			// code error count
	long lCountCommTry;    			// total communication count
	WORD wSuccessPercent;			// 통신 성공률

	// 9.5.1 부터 추가되었다. 이 변수는 일단 Station Info에서만 사용한다.
	short	nContinuedTimeOut;		// 연속적인 시간초과 회수
	short	nContinuedCodeBad;		// 연속적인 코드불량 회수
	WORD	flagsTimeOut;			// 연속 시간초과 마스크 0bit=1회, 1=5회, 2=10회, 3=20회, 4=30회
	WORD	flagsCodeBad;			// 연속 코드불량 마스크 0bit=1회, 1=5회, 2=10회, 3=20회, 4=30회
} COMM_COUNT_STRUCT;

typedef struct {
	COMM_COUNT_STRUCT Total;
	COMM_COUNT_STRUCT Read;
	COMM_COUNT_STRUCT WriteBit;
	COMM_COUNT_STRUCT WriteWord;	
} DEVICE_COUNT_STRUCT;

typedef struct {
	char sIP[80];
	int  nPort;
	char bThread;
	char bActive;
	int  nTimeOut;
} COMPUTER_DUAL_FILE;	// 저장되는 부분

typedef struct {
	HANDLE	hThread;
	DWORD	idThread;
	char	bThreadEnd;
	char	bThreadDo;

	CSocket *socket;		// UDP socket
	char    bSocketCreate;	// Socket Create
	TimeOutClass *timeoutLifeSignal;
	TimeOutClass *timeoutAnotherLife;
	TimeOutClass *timeoutChange;

	char	bConnect;
	char	bActiveI;
	char	bActiveYou;
	char	bErrorI;
	char	bErrorYou;

	char	bChangeStart;

	int		nReadPosWORD;
	int		nReadPosDWORD;
	int		nReadPosFLOAT;
	int		nReadPosSTRING;
	int		nReadPosDOUBLE;
	int		nReadPosINT64;

} COMPUTER_DUAL_RUN;	// 메모리 사용 부분

//#define MAX_SCAN_WRITE_LOCAL_ITEM_COUNT	4000	// 통신 프로그램에서 각 포트별로 쌓아서 대기하는 Write 갯수

extern int 	MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;		// 2021-6-23 변수로 변경

typedef struct {
	//char	BitOrWord;	// Bit냐 WORD냐?
	//SCAN_WRITE_EXCHANGE_ITEM item;
	char	bPushing;				// 넣는 중이다. 
	char	bPoping;				// 꺼내는중이다.

	short	ring_current;
	short	ring_target;
	SCAN_WRITE_EXCHANGE_ITEM *item;//SCAN_WRITE_EXCHANGE_ITEM item[MAX_SCAN_WRITE_LOCAL_ITEM_COUNT];	// 변수로 바뀌면서 포인터로 변경 2021-6-23
} WRITE_WAIT_STRUCT;

typedef struct {
	WORD	station_no;
	DEVICE_COUNT_STRUCT count_info;

	
} STATION_INFO_ONE_STRUCT;

typedef struct {
	char	bUse;			// 스테이션 상태 정보를 사용한다. 시간 초과/코드 불량 등
	short	station_count;
	STATION_INFO_ONE_STRUCT *stationBuf;
} STATION_INFO_STRUCT;

typedef struct {
	HANDLE	handle;
	DWORD	id;
	char	bEnd;
	char	bDo;
} THREAD_PORT_STRUCT;




typedef struct {
	char bActiveFlag;					// port active
	TCHAR sTitle[40];					// port title

	SYSTEM_BUF *bufSYSTEM;				// port 특성 메모리

	//COMM_COUNT_STRUCT countTotal;
	//COMM_COUNT_STRUCT countRead;
	//COMM_COUNT_STRUCT countWriteBit;
	//COMM_COUNT_STRUCT countWriteWord;
	DEVICE_COUNT_STRUCT countAll;		// 
	DEVICE_COUNT_STRUCT countDevice[2];	// 기본/예비 Device의 통신
	
	char sScanDevice[80];				// scan protocol
	char sScanProtocol[80];				// scan protocol

	// 원래 local에 sScanProtocolOption 이 있었으나 이중화에서 각각 다른 Option이 필요해서 아래를 만들었다. 프로토콜에서는 local.sScanProtocolOption을 사용하므로
	// Proc 호출 시 상황에 맞게 sScanProtocolOption[0/1] 를 local.sScanProtocolOption에 복사한 다음 호출하도록 한다.
	char sScanProtocolOption[2][80];	// 이중화 기본/예비 
	
	int	  nScanProtocol;				// protocol
	int	  nScanDevice;					// communication device

	// int를 short로 바꾸지 말것 over될수가 있음
	
	int   nLocalReadScanTimeCurr;		// 흐른 Read ScanTime
	struct time tLocalReadScanTimeOld;		

	// int를 short로 바꾸지 말것 over될수가 있음
	
	int   nLocalWriteScanTimeCurr;		// 흐른 Write ScanTime
	struct time tLocalWriteScanTimeOld;		

	DLL_PROTOCOL_PROC dll;				// PROTOCOL_DLL 일때만 사용한다. Procedure의 포인터가 있다.
	LOCAL_PORT_STRUCT local;			// 각 프로토콜에서 필요한 부분이 모두 있음

	TELEPHONE_STRUCT tel;				// telephone 을 사용할때 옵션

	short	nVipScanCount;				// VIP SCAN
	short	nVipScanPos;

	char	bDualActive;				// dual 사용여부.
	char	sDualDevice[80];			// dual DEVICE
	int		nDualCauseTimeOut;			// 절체 조건중에서 TimeOut일 때.
	char	bDualUseProtocol;			// 프로토콜도 이중화.
	char	sDualProtocol[80];			// dual Protocol
	int		nDualTimeOutCount;			// 듀얼시스템에서 현재의 TimeOut Count
	char	cDualCurrentActiveDevice;	// 현재 사용중인 Device 0 - device, 1 = 예비 Device

	// 2017-4-3 추가
	// AIC에서 TCP/IP 에서 버퍼밀림 현상이 계속 발생해서 복구가 되지 않는다.  transaction 번호 밀림이 계속되는 현상이 있어서 코드불량에서도 절체를 만들었다.
	int		nDualCauseCodeBad;			// 절체 조건중에서 CodeBad일 때.
	int		nDualCodeBadCount;			// 듀얼시스템에서 현재의 Code Count

	short	nCountContinueTimeOut;		// 연속한 시간 초과

	WRITE_WAIT_STRUCT *blockWriteWait;

	char	bActiveThread;				// Thread 사용함.
	short	nThreadCycle;				// Thread Sleep time
	char	bThreadProtocolDrawWorking;	// 이것은 스래드 사용시 각 프로토콜의 그리는 함수에서 점유할 때 ON을 사용한다.
										// 이중화 절체시 이 플래그를 사용하지 않으면 다운된다.

	bool	bCountNextCommTry;			// default = true, 이전의 통신리턴이 COMMUNICATION_OK_NO_COUNT 일때만 flase 다음 통신의 횟수를 증가시키지 않는다.

	HANDLE  hmmfWORD;
	HANDLE  hmmfDWORD;
	HANDLE  hmmfFLOAT;
	HANDLE  hmmfSTRING;
	HANDLE  hmmfSYSTEM;
	HANDLE  hmmfDOUBLE;
	HANDLE  hmmfINT64;
	BYTE    hmmfNumber;					// 각 포트의 hmmf 넘버 이것은 각 포트의 리셋 시 번호를 할당하여 다른 메모리 맵으로 갈 수있도록 한다.

	COMPUTER_DUAL_FILE pcDualFile;		// PC Dual
	COMPUTER_DUAL_RUN pcDualRun;		// PC Dual

	char	bInitialFlag;				// 포트의 초기화가 모두 끝났다.
										// 포트의 준비가 끝났으므로 다른 함수에서 포트를 엑세스해도 된다.
	char	bDeviceInitialFlag;			// 디바이스의 초기화 여부 
	char	bInvalidateScreen;			// 해당 디스프레이를 다시 그릴 필요가 있다.

	int		retry_count_on_timeout;

	BYTE	bScanPause;
	
	STATION_INFO_STRUCT stationInfo;	 

	char	checkWatchDog;				// WatchDog에서 사용하는 라이프 시그널 체크
	THREAD_PORT_STRUCT threadInfo;		// 쓰레드에서 사용

	char	bUseDeviceInfo;				// 디바이스 상태 정보를 사용한다.
	
	CryptoCommunication  *pCC;			// Crypto Communication;

	BYTE	reserved[1431];				// 구조체를 2의 승수에 맞추기 위한 여분의 데이터
} GLOBAL_PORT_STRUCT;

extern GLOBAL_PORT_STRUCT *portBuf;
extern int nPortHap;	// 실제 enable되어 있는 port의 개수

void PlcProtocolDrawMethodTitle(GLOBAL_PORT_STRUCT *st, HDC hdc, int x, int y);
void PlcProtocolDrawMethod(HDC hdc, int x, int y, GLOBAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm);
int  PlcProtocolRead(GLOBAL_PORT_STRUCT *st, int pos);
int  PlcProtocolWriteWord(GLOBAL_PORT_STRUCT *st, int port, int ss, DWORD addr, char *extra1, WORD extra2, double value);
int  PlcProtocolWriteBit(GLOBAL_PORT_STRUCT *st,  int port, int ss, DWORD addr, char *extra1, WORD extra2, WORD value);
int  PlcProtocolWriteBlock(GLOBAL_PORT_STRUCT *pt, int port, int ss, DWORD address_org, char *extra1, WORD extra2, BYTE *value, short arraysize, BYTE array_type);
int  PlcProtocolGetProtocol(HWND hwnd, GLOBAL_PORT_STRUCT *pt, char *string);

int  PlcDeviceInit(HWND hwnd, DEVICE_STRUCT *device, char *string, GLOBAL_PORT_STRUCT *pt);
int  PlcDeviceReadContinue(DEVICE_STRUCT *device, char *buf, int count);
int  PlcDeviceWriteContinue(DEVICE_STRUCT *device, char *buf, int count);
int  PlcDeviceWrite(DEVICE_STRUCT *device, char ch);
int  PlcDeviceClear(DEVICE_STRUCT *device);
int  PlcDeviceEnable(DEVICE_STRUCT *device, char flag);

int  PlcDeviceUnInit(DEVICE_STRUCT *device);
int  PlcDeviceGetCommError(DEVICE_STRUCT *device, COMSTAT *comStat);
int  PlcDeviceGetDdeMethod(DEVICE_STRUCT *device);
int  PlcDeviceSetParity(DEVICE_STRUCT *device, char parity);
void PlcDeviceGetInfoString(DEVICE_STRUCT *dev, TCHAR *buf);
int  PlcDeviceCheckConnecting(DEVICE_STRUCT *device, TELEPHONE_STRUCT *tel);
void PlcDeviceResetErrorCount(DEVICE_STRUCT *device);
void PlcDeviceGetCountDown(DEVICE_STRUCT *device, SYSTEMTIME *t);
void PlcDeviceSetHandConnection(DEVICE_STRUCT *device);
void PlcDeviceSetHandDisConnection(DEVICE_STRUCT *device);
int  PlcDeviceIsModemDevice(DEVICE_STRUCT *device);
int  PlcDeviceSetCommBreak(DEVICE_STRUCT *device);
int  PlcDeviceClearCommBreak(DEVICE_STRUCT *device);
int  PlcDeviceSetCommState(DEVICE_STRUCT *device, DWORD baud, char parity, char data, char stop);
int  PlcDeviceGetInfo(DEVICE_STRUCT *device, int item, BYTE *buf, int size);
int  PlcDeviceSetInfo(DEVICE_STRUCT *device, int item, BYTE *buf, int size);

void MessageDisplay(LPCSTR string);
//void MessageDisplayHide();
 
void DisplaySendCode(int port, BYTE code);
void DisplaySendCodeNextLine(int port);
void DisplayRecvCode(int port, BYTE code);
void DisplayRecvCodeNextLine(int port);
void DisplaySendString(int port, const char *string);
void DisplayRecvString(int port, const char *string);

void ProtocolSetCodeStartEnd(LOCAL_PORT_STRUCT *pt, int s_code, int e_code);
void ProtocolSetCodeMode(LOCAL_PORT_STRUCT *pt, int mode);

//char *PlcScanGetErrorString();	// 통신 return이 Communication_err_string일 때 스트링의 포인트를 얻을 수 있다.

extern HMENU hMenuInit;
extern HMENU hMenuInitWindow;
extern HINSTANCE hInst;
extern HWND	 hwndMainFrame;			// 메인윈도우의 Framw 핸들
extern HWND   hwndMainClient;			// 메인윈도우의 Client 핸들

extern char	 sDirProgramm[MAXPATH];  	// 프로그램이 시작된 디렉토리
extern char	 sDirWorkProject[MAXPATH]; // 현재 작업이 진행중인 root 디렉토리. 이 디렉토리 밑으로 작업파일을 저장한다.

// Binary 형식으로 저장되어야 할 환경

typedef struct {
	LOGFONT logFontEdit;			// 포트에디터에서 사용하는 폰트.
} CONFIG_BIN_STRUCT;

typedef struct {
	POINT	pMessageBox;
	int		nAlarmScreenTime;
	char	bMessageBoxAllMessage;	// 전체 메세지를 표시할 것이냐?
	char	bMessageBoxTimeOut;		// TimeOut일 때 메세지 상자를 표시할 것이냐?
	char	bMessageBoxCodeBad;		// CodeBad일 때 메세지 상자를 표시할 것이냐?
	int		nScanTime;					// 통신 지연시간.
	int		nSkipTimeOnTimeOut;		// timeout이 5회 이상 발생할 때 통신 대기시간.
	char	bMultiPortMultiTasking;	// 여러 포트를 사용할때 제어방법.
	char	bBufClearOnTimeOut5;	// 시간초과가 5회이상 발행할 때 버퍼 클리어.
	float   fBufClearValueOnTimeOut5;// 시간초과가 5회이상 발행할 때 버퍼 클리어할 값.
	int		nItemTimeoutCount;		// 아이템에서 연속 시간초과가 발생할 때 시간초과 조건 0 = 사용안함


	int		nRetryCountOnWriteTimeOut;	// 쓰기 시 시간초과가 발생할 때 재시도 횟수.

	int		nVipScanTryCount;		// VIP scan 시도할 횟수.

	int		OnPortTimeOut_SetValue_TimeOut; // port 시간초과시 set 조건 (timeout 값)
	int		OnPortTimeOut_SetValue_Value;	// port 시간초과시 set 조건 (set 값)

	char	bUseNewValueOnDigitalOut;	// 비트 쓰기시 중복명령이 있으면 마지막 값 사용
	char	bUseNewValueOnAnalogOut;	// 워드 쓰기시 중복명령이 있으면 마지막 값 사용

	CONFIG_BIN_STRUCT bin;
} CONFIG_STRUCT;

extern CONFIG_STRUCT config;
extern HFONT hFontEdit;				// 포트에디터에서 사용하는 폰트.
extern HFONT hFontMain;



void PlcScanHelp(HWND hwnd, char *sHtml);

void GetResourceString(UINT id, CString &str);
void GetResourceString(UINT id, TCHAR *buf, int limit);

void PokeWordSYSTEM(GLOBAL_PORT_STRUCT *pt, WORD address, WORD value);
void PokeBitSYSTEM(GLOBAL_PORT_STRUCT *pt, WORD address, char flag);
void PokeBitSYSTEM(int port, WORD address, char flag);

#define POKE_BIT_MODEM_CONN_STATUS		0x0010
#define POKE_BIT_MODEM_AUTO_CONNECTION	0x0011

enum {
	DEVICE_INFO_TCPIP_CONNECT_FLAG = 1,	// TCP IP 접속상태				1byte = flag(1)
	DEVICE_INFO_TCPIP_CONNECT_TIME = 2,	// TCP IP 접속된 시작 시간		9byte = year(2)+month(1)+day(1)+hour(1)+minute(1)+second(1)+milisecond(2);
};

#pragma pack(pop)

#endif

