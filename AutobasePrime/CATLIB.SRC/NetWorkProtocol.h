// TAG SIZE O.K
#if	!defined (__NetWorkProtocol_H)
#define __NetWorkProtocol_H

#include <tools.h>

#pragma pack(push, 1)

// 맨 상위 비트가 살아 있으면	ACK를 원하는 명령이다.

#define COMMAND_LIFE_SIGNAL_SERVER	0x0001
#define COMMAND_CHANGE_MAIN			0x0002
#define COMMAND_LIFE_SIGNAL_CLIENT	0x0003
#define COMMAND_ACK					0x0004
#define	COMMAND_DUPLEX_HAND			0x0005
#define	COMMAND_DUPLEX_AUTO			0x0006
#define COMMAND_SERVER_REGISTER		0x0007	// NetServer->NetServer/NetClient
#define COMMAND_SERVER_UNREGISTER	0x0008	// NetServer->NetServer/NetClient
#define COMMAND_TIME_SYNC			0x0009	// NetServer->All Network

#define COMMAND_CLIENT_GOODBY		0x0010	// NetClient->All Network

#define	COMMAND_PC_DUAL_LIFE_SIGNAL			0x0050
#define	COMMAND_PC_DUAL_DISCONNECT_SIGNAL	0x0051
#define	COMMAND_PC_DUAL_I_AM_ACTIVATED		0x0052		// 이 신호를 받으면 비활성화 되어야 한다.

#define COMMAND_SMS_DATA			0x0100
#define COMMAND_SMS_CALL_ERROR		0x0101

#define COMMAND_PLCSCAN_WRITE_BIT				0x8100	// PLC_SCAN->NetClient->NetServer->PLC_SCAN
#define COMMAND_PLCSCAN_WRITE_WORD				0x8101	// PLC_SCAN->NetClient->NetServer->PLC_SCAN	
#define COMMAND_PLCSCAN_PORT_LIFE_SIGNAL		0x0102	// PLC_SCAN->NetClient->NetServer
#define COMMAND_PLCSCAN_PORT_DISCONNECT_SIGNAL	0x0103	// PLC_SCAN->NetClient->NetServer
#define	COMMAND_PLCSCAN_VALUE_WORD				0x8104	// NetServer->NetClient	NEED ACK
#define	COMMAND_PLCSCAN_VALUE_DWORD				0x8105	// NetServer->NetClient	NEED ACK
#define	COMMAND_PLCSCAN_VALUE_FLOAT				0x8106	// NetServer->NetClient	NEED ACK
//#define	COMMAND_PLCSCAN_READ_ONE				0x0107	// Plc_scan->Exe comm
#define	COMMAND_PLCSCAN_VALUE_WORD_BLOCK		0x8108	// NetServer->NetClient	NEED ACK
#define	COMMAND_PLCSCAN_VALUE_DWORD_BLOCK		0x8109	// NetServer->NetClient	NEED ACK
#define	COMMAND_PLCSCAN_VALUE_FLOAT_BLOCK		0x810A	// NetServer->NetClient	NEED ACK
#define	COMMAND_PLCSCAN_VALUE_SYSTEM			0x810B	// NetServer->NetClient	NEED ACK
#define COMMAND_PLCSCAN_PORT_LIFE_SIGNAL_MULTI	0x010C	// 
#define	COMMAND_PLCSCAN_VALUE_STRING			0x810D	// NetServer->NetClient	NEED ACK
#define	COMMAND_PLCSCAN_NEED_ALL_DATA			0x010E	// Client에서 시작 시에 한번 발생
#define	COMMAND_PLCSCAN_VALUE_STRING_BLOCK		0x810F	// NetServer->NetClient	NEED ACK

#define	COMMAND_PLCSCAN_VALUE_DOUBLE			0x8110	
#define	COMMAND_PLCSCAN_VALUE_DOUBLE_BLOCK		0x8111	
#define	COMMAND_PLCSCAN_VALUE_INT64				0x8112	
#define	COMMAND_PLCSCAN_VALUE_INT64_BLOCK		0x8113	

#define COMMAND_TAG_PROTECT_FLAG_CHANGE			0x8200
#define COMMAND_ALARM_LIST_CONFIRM_ONE			0x8201	//
#define COMMAND_ALARM_LIST_DELETE_ONE			0x8202	//
#define COMMAND_TAG_VALUE_CHANGE				0x8204
#define	COMMAND_TAG_MEMBER_CHANGED				0x8205

class NetWorkProtocolRecv {
	public:
		WORD wCommand;
		WORD wTransaction;
		int  nNodeType;			// NodeType
		char sNodeName[80];		// NodeName

		//int	 smBufType;
		//char smDeviceType[10];
		//int  smTarget;
		//int  smSize;

		int  nPort;					// plc scan port
		int  nBroadCastPort;		// plc scan 포트에서 제공받고자 하는 Port
		WORD wBroadCastPorts[16];	// plc scan 포트에서 제공받고자 하는 Port
		int  nStation;			// plc scan station
		DWORD dwAddress;		// plc scan address
		char sExtra1[40];		// Plc Scan Extra1
		WORD wExtra2;			// Plc Scan Extra2
		long double fValue;		// value
		__int64 i64Value;
		char sTag[40];			// tag name
		WORD wTagMember;		// TagMember No
		short nBlockSize;		// count
		BYTE *Block;			// block count = nCount를 사용한다.
		char *sString;			// "String" 값.
		SYSTEMTIME tDateTime;	// SYSTEM TIME
		char bServerActive;		// 해당서버가 활성화가 되어있는가를 검사
		char bPlcScanTimeOut;
		char bAnotherProgramExit;
		char bAutoWatch;		// 자동 감시 상태
		short nVersionMajor;
		short nVersionMinor;

		NetWorkProtocolRecv();		
		~NetWorkProtocolRecv();
		void FreeBuf();
		int Split(void *buf, int size);
};

class NetWorkProtocolSend {
	public:
		char *bufSend;
		int  nBufCount;

		NetWorkProtocolSend();		
		~NetWorkProtocolSend();
		void MakeBlock(WORD command, WORD trans, const char *buf, WORD size);
};

WORD GetTransaction();

typedef struct {
	char	bEvent;
	WORD	size;
	BYTE	buf[5000];
} SHARE_PLCSCAN_NETWORK;

int SendEventProgramToNetwork(char *buf, int count);
int SendEventNetworkToViewMain(char *buf, int count);

class RecvEventAndShareMemory {
		SharedMemory *shareClass;
	public:
		RecvEventAndShareMemory(char *name);
		~RecvEventAndShareMemory();
		int IsEvent();
		void ResetEvent();
		void *GetPointer();
};

int SendEventAndShareMemory(char *name, void *buf, int count);

typedef struct {
	WORD address;
	WORD value;
} NETWORK_PROTOCOL_BLOCK_WORD;

typedef struct {
	WORD address;
	DWORD value;
} NETWORK_PROTOCOL_BLOCK_DWORD;

typedef struct {
	WORD address;
	float value;
} NETWORK_PROTOCOL_BLOCK_FLOAT;

typedef struct {
	WORD address;
	double value;
} NETWORK_PROTOCOL_BLOCK_DOUBLE;

typedef struct {
	WORD address;
	__int64 value;
} NETWORK_PROTOCOL_BLOCK_INT64;

#pragma pack(pop)

#endif
