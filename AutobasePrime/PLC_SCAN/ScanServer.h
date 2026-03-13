#if	!defined (__GATELIB_H)
#include <gatelib.h>
#endif

/*
typedef struct {
	int  connect_num;

	int  nConnectProtocol; 		// 연결 방식.

	// 접속 중 아무 반응이 없을때 접속을 해제할 수 있는 타이머 변수.
	int  nCurrSec;
	char cOldSec;

	// 태그의 현재치를 보내줄 때 한번에 보내면 시간이 걸리므로 시간이 날때마다 하나씩 보낸다.
	Block *blockCurr;

	int   nRecvHap;
	char  recvBuf[20];

	MODEM_STRUCT modem;
	char  sModemInitCommand[20];

	int	tcpipPort;
	

	// COMMAND_SEND_ANYTHING 를 보낸후에 신호를 기다리기 위한 변수.
	int	WaitStartIdCount;
	TimeOutClass timeoutWaitStartID;
	char  bAnythingCommandSendFlag;		// ON 이면 신호를 보내고 대답을 기다리는 중이다.

	int   nPosAI_fSumTotal;				// 적산값은 1분마다 달라지므로 수시로 보내준다.
} CONNECT_LIST;
*/

typedef struct {
	int pos;
} STATUS_SEND;

typedef struct {
	int  struct_no;
	int  nDeviceType;				// 연결 장치 종류.
	char bInitialFlag;				// 초기화가 되었느냐?
	
	char	bThreadFlag;				// Thread를 사용하느냐?
	HANDLE	hThread;
	DWORD	idThread;
	char	bThreadEnd;
	char	bThreadDo;
	char	bThreadPause;			// thread를 잠시정지한다.
	char	bThreadPauseACK;		// thread function이 thread가 pause되었을 때 ON을 보내준다.

	int		nSendDelay;
	char	bSendOnlyChange;
	int		nSendBlockSize;			// 한번에 보내는 개수
	TimeOutMiliSecClass timeoutSendDelay;

	HGATE hGate;
	
	char bConnectFlag;				// 상태편과 접속되었느냐?
	//int  nConnectPort;				// 서비스할 Port
	WORD	wCastPort[16];			// 공급해줄 포트.
	TimeOutClass timeoutDisconnect;	

	TimeOutClass timeoutTryInit;
	int  nTryInitCount;

	SOCKET sock_listen;				// tcp/ip 응답용 socket
	WORD tcpipPort;
	
	MODEM_STRUCT modem;
	char  sModemInitCommand[20];

	int		nRecvHap;
	char	recvBuf[MAX_RECV_BUF];

	WORD	*nReadPosWORD;
	WORD	*nReadPosFLOAT;
	WORD	*nReadPosDWORD;
	WORD	*nReadPosSTRING;
	WORD	*nReadPosDOUBLE;
	WORD	*nReadPosINT64;

	//WORD	nReadPosWORD[256];
	//WORD	nReadPosFLOAT[256];
	//WORD	nReadPosDWORD[256];
	//WORD	nReadPosSTRING[256];
	//WORD	nReadPosDOUBLE[256];
	//WORD	nReadPosINT64[256];

	bool	bSignal_FD_CLOSE;	// TCP socket Close 가 들어오면 이 플래그를 살려준다.

	short nVersionMajor;		// 상대편의 통신 드라이버 버전
	short nVersionMinor;		// 상대편의 통신 드라이버 버전

	//int		nBlockSize;
} SCAN_SERVER_LIST;

#define	MAX_SCAN_SERVER_LIST	256

extern SCAN_SERVER_LIST scanServerList[MAX_SCAN_SERVER_LIST];

void ScanServerListLoad();
void ScanServerListSaveOne(int connect);
void ScanServerUnInitAllDevice();
void ScanServerInitAllDevice(HWND hwnd);
void ScanServerStatus();
