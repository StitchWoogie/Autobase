/*
#if	!defined (__WINSOCK_H)
#include <winsock.h>
#endif
*/

#include "..\plc_scan.h"
#include "PlcScanSharedMemory.h"

// 고유번호는 바꾸지 않도록 한다. Pcs_Sbus.dll에서 이번호를 사용한다. 2004-6-9
enum {
	DEVICE_TYPE_NULL=0,
	DEVICE_TYPE_NONE=1,			// Device를 사용하지 않는다.
	DEVICE_TYPE_RS232=2,
	DEVICE_TYPE_MODEM=3,		// 전화거는 모뎀을 말한다.
	DEVICE_TYPE_TCPIP=4,		// TCP/IP
	DEVICE_TYPE_UDPIP=5,		// UDP/IP
    DEVICE_TYPE_TCP_SERVER=6,	// TCP Server(9.5.3부터 사용) 8까지는 DDE client이었다
	DEVICE_TYPE_NETCLIENT=7,	// NetClnt.exe 와 통신한다.
	DEVICE_TYPE_TELE=8,			// 모뎀-Device ~~~ Device-모뎀의 형식을 말한다.
	DEVICE_TYPE_SHAREDMEMORY=9,	// 공유 메모리를 사용한다.
};

enum TxMETHOD {
	TX_METHOD_ON,	// 항상 송신 포트를 열어둔다. 일반적인 232상태
	TX_METHOD_RTS,	// 3FC 의 1번째 비트(RTS)가 ON일 때 포트를 열어 송신한후 출구를 닫는다.
					// 보통 이것은 422, 485에서 multi-drop이나 2Wire 방식의 연결을 할때 사용한다.
	TX_METHOD_DTR,	// 3FC 의 1번째 비트(RTS)가 ON일 때 포트를 열어 송신한후 출구를 닫는다.
					// 보통 이것은 422, 485에서 multi-drop이나 2Wire 방식의 연결을 할때 사용한다.
};

enum RxMETHOD {
	RX_METHOD_ON,	// 항상 수신 포트를 열어둔다. 일반적인 232상태
	RX_METHOD_ECHO,	// 항상 수신 포트를 열어둔다.
					// 이 상황은 2Wire 연결에서 송신된 신호가 그대로 수신된
					// 경우이다. 이 경우 반송된 메세지를 제거해야 한다.
};

// 스트럭쳐에서 빼거나 넣지 않도록 한다. 뺏을 때는 reserved시키고 더했을때는 마지막에 붙이도록 한다.
// 사이즈가 현재보다 작아지면 위험하다. PCD_SBUS.DLL에서 사용한다. (2004-6-9)
typedef struct {
	int		port_no;
	HANDLE	id;			// 232-port의 ID
	int		txMethod;	// serial send method
	int		rxMethod;	// serial receive method
	int		port;
	long	baud;
	int		parity;
	int		data;
	int		stop;
	int		nEndDelayReadRTS;		// RTS 시그널은 tx할때 사용한다. 이시간은 다 보낸후에 어느정도 지연하고 RTS를 clear한다.
	int		nEndDelayWriteRTS;		// RTS 시그널은 tx할때 사용한다. 이시간은 다 보낸후에 어느정도 지연하고 RTS를 clear한다.
	int		nStartDelayReadRTS;		// RTS 시그널은 tx할때 사용한다. RTS를 열고 기다리는 시간.
	int		nStartDelayWriteRTS;	// RTS 시그널은 tx할때 사용한다. RTS를 열고 기다리는 시간.
	int		reserved;				// nEchoRemain; 2007-11-9 reserved시킴
	int     nRtsMethod;				// 0 = RTS_ENABLE, 1 = RTS_TOGGLE,
	CryptoCommunication *pCC;		// PORT_STRUCT 에서 할당된 암호화 설정.
} DEVICE_STRUCT_RS232;

int PlcDeviceInitRS232(HWND hwnd, DEVICE_STRUCT_RS232 *rs232, CommaBlockString *comma);
int PlcDeviceReadContinueRS232(DEVICE_STRUCT_RS232 *rs232, char *buf, int count);
int PlcDeviceWriteContinueRS232(DEVICE_STRUCT_RS232 *rs232, char *buf, int count);
int PlcDeviceClearRS232(DEVICE_STRUCT_RS232 *rs232);
int PlcDeviceEnableRS232(DEVICE_STRUCT_RS232 *rs232, char flag);
int PlcDeviceUnInitRS232(DEVICE_STRUCT_RS232 *rs232);
int PlcDeviceGetCommErrorRS232(DEVICE_STRUCT_RS232 *rs232, COMSTAT *comStat);
int PlcDeviceSetCommBreakRS232(DEVICE_STRUCT_RS232 *rs232);
int PlcDeviceClearCommBreakRS232(DEVICE_STRUCT_RS232 *rs232);
void PlcDeviceGetCommModemStatusRS232(DEVICE_STRUCT_RS232 *rs232);

typedef struct {
	int port_no;
	SOCKET	socket;			// TCP/IP UDP/IP socket
	char	bConnect;		// 현재 연결중인가?
	char	ip[40];			// host ip address 1.2.3.4 or plc3    string으로 쓸 때는 반드시 HOSTS파일속에 이름이 정의되어 있어야 한다.
	WORD	port[8];			// 사용 Port가 8개인 이유는 한포트가 에러가 날때 계속해서 다른 포트를 사용하고자 할때 사용한다.
	int		nCurrPort;		// 현재 사용중인 port
	int		error_count;
	char	old_sec;
	SYSTEMTIME tConnect;	// 접속된 시작 시간 2012-5-23
	CryptoCommunication *pCC;		// PORT_STRUCT 에서 할당된 암호화 설정.
} DEVICE_STRUCT_TCPIP;

int PlcDeviceInitTCPIP(HWND hwnd, DEVICE_STRUCT_TCPIP *tcpip, CommaBlockString *comma);
int PlcDeviceReadContinueTCPIP(DEVICE_STRUCT_TCPIP *tcpip, char *buf, int count);
int PlcDeviceWriteContinueTCPIP(DEVICE_STRUCT_TCPIP *tcpip, char *buf, int count);
int PlcDeviceClearTCPIP(DEVICE_STRUCT_TCPIP *tcpip);
int PlcDeviceEnableTCPIP(DEVICE_STRUCT_TCPIP *tcpip, char flag);
int PlcDeviceUnInitTCPIP(DEVICE_STRUCT_TCPIP *tcpip);
int PlcDeviceGetCommErrorTCPIP(DEVICE_STRUCT_TCPIP *tcpip, COMSTAT *comStat);
int PlcDeviceGetInfoTCPIP(DEVICE_STRUCT_TCPIP *tcpip, int item, BYTE *buf, int size);

#define MAX_UDPIP_RECV_BUF	4096

typedef struct {
	int port_no;
	//SOCKET	socket;			// UDP/IP UDP/IP socket
	char	ip[40];			// host ip address 1.2.3.4 or plc3    string으로 쓸 때는 반드시 HOSTS파일속에 이름이 정의되어 있어야 한다.
	WORD	port;
	/*
	WORD	port[8];		// 사용 Port가 8개인 이유는 한포트가 에러가 날때 계속해서 다른 포트를 사용하고자 할때 사용한다.
	int		nCurrPort;		// 현재 사용중인 port
	int		error_count;
	*/
	SOCKADDR_IN dest_sin;	// DESTination Socket INternet 

	//char    ring[MAX_UDPIP_RECV_BUF];
	//WORD	pos_total;
	//WORD	pos_curr;
	//int		nLocalNo;
	
	//CryptoCommunication *pCC;		// UDPIP는 지원하기 힘든 구조이다.
} DEVICE_STRUCT_UDPIP;

int PlcDeviceInitUDPIP(HWND hwnd, DEVICE_STRUCT_UDPIP *udpip, CommaBlockString *comma);
int PlcDeviceReadContinueUDPIP(DEVICE_STRUCT_UDPIP *udpip, char *buf, int count);
int PlcDeviceWriteContinueUDPIP(DEVICE_STRUCT_UDPIP *udpip, char *buf, int count);
int PlcDeviceClearUDPIP(DEVICE_STRUCT_UDPIP *udpip);
int PlcDeviceUnInitUDPIP(DEVICE_STRUCT_UDPIP *udpip);
int PlcDeviceGetCommErrorUDPIP(DEVICE_STRUCT_UDPIP *udpip, COMSTAT *comStat);

typedef struct {
	int port_no;
} DEVICE_STRUCT_NETCLIENT;

int PlcDeviceInitNetClient(HWND hwnd, DEVICE_STRUCT_NETCLIENT *net, CommaBlockString *comma);
//int PlcDeviceReadContinueNetClient(DEVICE_STRUCT_OSI *osi, char *buf, int count);
int PlcDeviceWriteContinueNetClient(DEVICE_STRUCT_NETCLIENT *net, char *buf, int count);
//int PlcDeviceClearOSI(DEVICE_STRUCT_OSI *osi);
int PlcDeviceUnInitNetClient(DEVICE_STRUCT_NETCLIENT *net);
//int PlcDeviceGetCommErrorOSI(DEVICE_STRUCT_OSI *osi, COMSTAT *comStat);

typedef struct {
	int port_no;
	PlcScanSharedMemory *sharedMemory;
} DEVICE_STRUCT_SHAREDMEMORY;

int PlcDeviceInitSharedMemory(HWND hwnd, DEVICE_STRUCT_SHAREDMEMORY *share, CommaBlockString *comma);
int PlcDeviceReadContinueSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *share, char *buf, int count);
int PlcDeviceWriteContinueSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *share, char *buf, int count);
int PlcDeviceClearSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *share);
int PlcDeviceUnInitSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *share);

typedef struct {
	int port_no;
	SOCKET	socket;				// TCP/IP UDP/IP socket
	bool	bConnect;			// 현재 연결중인가?
	char	client_ip[40];		// host ip address 1.2.3.4 or plc3    string으로 쓸 때는 반드시 HOSTS파일속에 이름이 정의되어 있어야 한다.
	WORD	tcp_port;			// 사용 Port가 8개인 이유는 한포트가 에러가 날때 계속해서 다른 포트를 사용하고자 할때 사용한다.
	int		sec_hap;
	char	old_sec;
	SOCKET sock_listen;			// tcp/ip 응답용 socket
	int		nTimeoutReset;		// 응답이 없는 경우 지정된 시간이 지나면 다시 리셋해 준다.
	CryptoCommunication *pCC;		// PORT_STRUCT 에서 할당된 암호화 설정.
} DEVICE_STRUCT_TCP_SERVER;

int PlcDeviceInitTcpServer(HWND hwnd, DEVICE_STRUCT_TCP_SERVER *tcpip, CommaBlockString *comma);
int PlcDeviceReadContinueTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, char *buf, int count);
int PlcDeviceWriteContinueTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, char *buf, int count);
int PlcDeviceClearTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip);
int PlcDeviceEnableTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, char flag);
int PlcDeviceUnInitTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip);
int PlcDeviceGetCommErrorTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, COMSTAT *comStat);

typedef struct {
	int port_no;
	DEVICE_STRUCT_RS232 rs232;	// 모뎀은 rs232의 모든 옵션을 수용한다.
	int  connect_error_count;	// 접속 시 발생한 error count
	char connect_error_msg[80];	// 접속 시 발생한 error message
	SYSTEMTIME tLastCall;
	SYSTEMTIME tCountDown;
	//struct date dLastCall;		// 마지막으로 접속된 시간.
	//struct time tLastCall;		// 마지막으로 접속된 시간.
	//struct time tCountDown;		// 마지막으로 접속된 시간.
	char bHandConnection;		// 수동 접속.
} DEVICE_STRUCT_MODEM;

int PlcDeviceInitModem(HWND hwnd, DEVICE_STRUCT_MODEM *modem, CommaBlockString *comma, TELEPHONE_STRUCT *tel);
int PlcDeviceReadContinueModem(DEVICE_STRUCT_MODEM *modem, char *buf, int count);
int PlcDeviceWriteContinueModem(DEVICE_STRUCT_MODEM *modem, char *buf, int count);
int PlcDeviceUnInitModem(DEVICE_STRUCT_MODEM *modem);
int PlcDeviceCheckConnectingModem(DEVICE_STRUCT_MODEM *modem, TELEPHONE_STRUCT *tel);
void PlcDeviceResetErrorCountModem(DEVICE_STRUCT_MODEM *modem);
int PlcDeviceClearModem(DEVICE_STRUCT_MODEM *modem);
void PlcDeviceSetHandDisConnectModem(DEVICE_STRUCT_MODEM *modem);

typedef struct {
	int port_no;
	DEVICE_STRUCT_RS232 rs232;	// 모뎀은 rs232의 모든 옵션을 수용한다.
} DEVICE_STRUCT_TELE;

int PlcDeviceInitTele(HWND hwnd, DEVICE_STRUCT_TELE *modem, CommaBlockString *comma);
int PlcDeviceReadContinueTele(DEVICE_STRUCT_TELE *modem, char *buf, int count);
int PlcDeviceWriteContinueTele(DEVICE_STRUCT_TELE *modem, char *buf, int count);
int PlcDeviceUnInitTele(DEVICE_STRUCT_TELE *modem);
//int PlcDeviceCheckConnectingModem(DEVICE_STRUCT_TELE *modem, TELEPHONE_STRUCT *tel);
//void PlcDeviceResetErrorCountModem(DEVICE_STRUCT_TELE *modem);
int PlcDeviceClearTele(DEVICE_STRUCT_TELE *modem);
//void PlcDeviceSetHandDisConnectModem(DEVICE_STRUCT_TELE *modem);
void PlcDeviceGetCommModemStatus(DEVICE_STRUCT *device);

typedef struct {
	char	init_command[30];
} MODEM_FILE_OPTION;

void ModemOptionFileRead(int modem_no, MODEM_FILE_OPTION *option);

typedef struct {
	char sInitCommand[80];
	char sConnectCommand[80];
	int  nTimeOutWaitConnect;
} TELE_DEVICE_OPTION;

void LoadTeleDeviceOption(int nComPort, TELE_DEVICE_OPTION *opt);






