// english O.K
//------------------------------------------------------------------------------
//	UDP/IP 의 통신을 담당한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <tools.h>
#include <dataswap.h>

//#include <winsock.h>
 
#include "..\plc_scan.h"
#include "commmain.h" 

//#define	VERSION_MAJOR	1
//#define	VERSION_MINOR	0 

typedef struct {
	SOCKADDR_IN dest_sin;	// DESTination Socket INternet 
	BYTE    *ring;
	WORD	pos_total;
	WORD	pos_curr;
	//int		udp_port_send;
	//int		udp_port_recv;
	int		udp_port;
	int		nConnectNo;
} LOCAL_STRUCT;

//static LOCAL_STRUCT localStruct[256];
static LOCAL_STRUCT *localStruct = NULL;	// 2011-11-21

typedef struct {
	SOCKET	socket;
	int		udp_port;
	int		depth;
} CONNECT_STRUCT;

static Block blockConnect(sizeof(CONNECT_STRUCT));

static void MakeErrorString(char *buf, int err)
{
	switch(err) {
		case WSANOTINITIALISED:
			sprintf(buf, "WSANOTINITIALISED-A successful WSAStartup must occur before using this function.\n(ErrorCode-%d)", err);
			break;
		case WSANO_DATA:
			sprintf(buf, "WSANO_DATA.\n(ErrorCode-%d)", err);
			break;
		case WSAEADDRNOTAVAIL:        
			sprintf(buf, "WSAEADDRNOTAVAIL-The specified address is not available from the local computer.\n(ErrorCode-%d)", err);
			break;
		case WSAECONNREFUSED:
			sprintf(buf, "WSAECONNREFUSED-The attempt to connect was forcefully rejected.\n(ErrorCode-%d)", err);
			break;
		default:
			sprintf(buf, "WSAERROR (ErrorCode=%d)", err);
	}
}

static void DisplayError(HWND hwnd, char *title, int err)
{
	StackChar buf(5000);

	MakeErrorString(buf.data, err);
	strcat(buf.data, "\n");
	strcat(buf.data, title);

	MessageDisplay(buf.data);
}

static int IsNumber(char *string)
{
	int i;

	for(i = 0; i < (int)strlen(string); i++) {
		if(string[i] >= '0' && string[i] <= '9')	continue;
		if(string[i] == '.')	continue;
		return 0;
	}

	return 1;
}

static int FillAddr(HWND hwnd, PSOCKADDR_IN psin, DEVICE_STRUCT_UDPIP *udpip)
{
	PHOSTENT phe;
	int retn;
	char title[80];

	psin->sin_family = AF_INET;

	if(IsNumber(udpip->ip)) {
		psin->sin_addr.s_addr = inet_addr(udpip->ip);
	}
	else {
		phe = gethostbyname(udpip->ip);
   
		if (phe == NULL) {
			retn = WSAGetLastError();
			sprintf(title, "gethostbyname() error HostName(%s)", udpip->ip);
			DisplayError(hwnd, title, retn);

			return 0;
		}
		memcpy((char FAR *)&(psin->sin_addr), phe->h_addr,
				 phe->h_length);
	}

	psin->sin_port = htons(udpip->port);        // Convert to network ordering 

	return 1;
}

static void FillAddrServer(DEVICE_STRUCT_UDPIP *udpip, PSOCKADDR_IN psin)
{
	psin->sin_family = AF_INET;
	psin->sin_addr.s_addr = INADDR_ANY;
	psin->sin_port = htons(udpip->port);        // Convert to network ordering
}

static void GlowUdpRecvBuf(SOCKET socket)
{
	if(socket == INVALID_SOCKET)	return;

	int opt_size = 4;
	char opt_value[80];
	int opt_recv_size = 60000;

	memcpy(opt_value, &opt_recv_size, 4);

	setsockopt(socket, SOL_SOCKET, SO_RCVBUF, opt_value, opt_size);
	int ex = getsockopt(socket, SOL_SOCKET, SO_RCVBUF, opt_value, &opt_size);
	if(opt_size == 4) {
		memcpy(&opt_size, opt_value, 4);
	}
}

int SocketPrepare(HWND hwnd, DEVICE_STRUCT_UDPIP *udpip, LOCAL_STRUCT *local, CONNECT_STRUCT *conn)
{
	int retn;

	conn->socket = socket(PF_INET, SOCK_DGRAM, 0);
	
	if(conn->socket == INVALID_SOCKET)	{
		retn = WSAGetLastError();
		DisplayError(hwnd, "socket() error", retn);
		return 0;
	}

	/*
	if (!FillAddr(hwnd, &udpip->dest_sin, udpip, msg_flag)) {
		closesocket(conn->socket);
		conn->socket = INVALID_SOCKET;
		return 0;
	}
	*/

	//memcpy(&local->dest_sin, &udpip->dest_sin, sizeof(SOCKADDR_IN));

	// 아래 Bind 부분은 Advantech 에서 나온 232->udp/ip conveter에서는 꼭 필요한 부분이다.
	// Glofa나 Melsec PLC는 아래부분이 없어도 통신이 잘된다.
	// 원래 Bind는 한포트당 하나의 Bind밖에 할 수 없다.
	// 그래서 오류처리는 하지 않았다.
	{
		SOCKADDR_IN dest_sin;  // DESTination Socket INternet

		FillAddrServer(udpip, &dest_sin);
		if (bind(conn->socket, (struct sockaddr*)&dest_sin, sizeof(dest_sin)) == SOCKET_ERROR) {
			retn = WSAGetLastError();
			DisplayError(hwnd, "bind() error", retn);
			closesocket(conn->socket);
			conn->socket = INVALID_SOCKET;

			return 0;
		}
	}

	GlowUdpRecvBuf(conn->socket);	

	conn->depth = 1;
	conn->udp_port = udpip->port;

	return 1;
}

void SocketUnPrepare(CONNECT_STRUCT *conn)
{
	if(conn->socket == INVALID_SOCKET)	return;

	if(conn->depth <= 1) {
		closesocket(conn->socket);
		conn->socket = INVALID_SOCKET;
		conn->depth = 0;
	}
	else {
		conn->depth--;
	}
}

static ThreadLock lockBlockConnect;

int InitConnect(HWND hwnd, DEVICE_STRUCT_UDPIP *udpip, LOCAL_STRUCT *local)
{
	DWORD l;
	CONNECT_STRUCT conn;

	// 먼저 이미 할당된 부분을 찾는다.
	for(l = 0; l < blockConnect.GetCount(); l++) {
		blockConnect.GetBlock(&conn, l);

		if(conn.socket != INVALID_SOCKET && conn.udp_port == udpip->port) {	// 이미 할당된 곳을 찾았다.
			conn.depth++;
			blockConnect.SetBlock(&conn, l);
			return l;
		}
	}

	for(l = 0; l < blockConnect.GetCount(); l++) {
		blockConnect.GetBlock(&conn, l);

		if(conn.socket == INVALID_SOCKET) {	// 비어있는곳을 찾았다.
			if(!SocketPrepare(hwnd, udpip, local, &conn))	return -1;
			blockConnect.SetBlock(&conn, l);
			return l;
		}
	}

	ZeroMemory(&conn, sizeof(CONNECT_STRUCT));
	if(!SocketPrepare(hwnd, udpip, local, &conn))	return -1;

	int insert_pos;

	// 여러쓰레드에서 UDP를 호출할 수 있어서 Lock을 사용한다. 2020-3-31 추가. 실제로 여러포트에서 Thread를 사용하고 UDP 포트를 다르게 사용해서 blockConnect.AddBlock() 에서 delete시 오류가 난다.
	lockBlockConnect.Lock();
	blockConnect.AddBlock(&conn);
	insert_pos = blockConnect.GetCount()-1;
	lockBlockConnect.Unlock();

	return insert_pos;

}

void PlcDeviceInitAllUDPIP()
{
	localStruct = new LOCAL_STRUCT[MAX_PORT];
	for(int i = 0; i < MAX_PORT; i++) {
		ZeroMemory(&localStruct[i], sizeof(LOCAL_STRUCT));
	}
}

void PlcDeviceUnInitAllUDPIP()
{
	delete localStruct;
	localStruct = NULL;
}

int PlcDeviceInitUDPIP(HWND hwnd, DEVICE_STRUCT_UDPIP *udpip, CommaBlockString *comma)
{
	LOCAL_STRUCT *local = &localStruct[udpip->port_no];

	// 스트럭쳐 전체를 초기화 한다.
	comma->GetString(udpip->ip, sizeof(udpip->ip));
	comma->GetWORD(udpip->port);

	local->pos_total = 0;	// ring buf 를 초기화 한다.
	local->pos_curr  = 0;	
	local->udp_port = udpip->port;
	if(local->ring == NULL) {
		local->ring = new BYTE[MAX_UDPIP_RECV_BUF];
	}

	FillAddr(hwnd, &udpip->dest_sin, udpip);
	memcpy(&local->dest_sin, &udpip->dest_sin, sizeof(SOCKADDR_IN));

	local->nConnectNo = InitConnect(hwnd, udpip, local);	

	return 1;
}

static int SeekMatchPort(SOCKADDR_IN *dest_sin, int udp_port) 
{
	LOCAL_STRUCT *local;
	int i;

	for(i = 0; i < 256; i++) {
		local = &localStruct[i];
		if(local->ring == NULL)	continue;			// already freed
		if(udp_port != local->udp_port)	continue;	// 통신해서 들어온 포트번호를 참고하지 않는다. 8.4.3부터 적용
		//if(dest_sin->sin_addr.s_addr == local->dest_sin.sin_addr.s_addr &&
		//	dest_sin->sin_port == local->dest_sin.sin_port)		return i;  // 실제 다른 번호를 보내주는 콘트롤러도 있다.
		if(dest_sin->sin_addr.s_addr == local->dest_sin.sin_addr.s_addr)
			return i;
	}

	return -1;
}

void ReadConnectAll()
{
	static char run_flag;

	if(run_flag)	return;

	run_flag = ON;
	
	DWORD l;
	int retn;
	int i;
	u_long remain = 0;
	CONNECT_STRUCT *conn;
	LOCAL_STRUCT *local;

	for(l = 0; l < blockConnect.GetCount(); l++) {
		conn = (CONNECT_STRUCT*)blockConnect.GetPtr(l);

		if(conn->socket == INVALID_SOCKET)	continue;

		// 비어있는 공간이 있으면 ring 버퍼에 담는다.
		if(ioctlsocket(conn->socket, FIONREAD, &remain) == 0) {
			if(remain > 0) {
				StackChar imsi(remain);
				if(imsi.data != NULL) {
					SOCKADDR_IN dest_sin;  // DESTination Socket INternet
					int addr_size = sizeof(dest_sin);
					retn = recvfrom(conn->socket, imsi.data, remain, 0, (PSOCKADDR) &dest_sin, &addr_size);

					if(retn != SOCKET_ERROR) {
						int port = SeekMatchPort(&dest_sin, conn->udp_port);
						
						if(port == 0 && remain != 64) {
							port = port;
						}

						if(port != -1) {
							local = &localStruct[port];
							for(i = 0; i < (int)retn; i++) {
								local->ring[local->pos_total] = imsi.data[i];
								local->pos_total++;
								local->pos_total%=MAX_UDPIP_RECV_BUF;
								DisplayRecvCode(port, imsi.data[i]);
							}
						}
					}
				}
			}
		}
	}

	run_flag = OFF;
}

// UDPIP는 Crypto를 지원하기 힘든 구조이다. Socket하나에 여러개가 들어올 수 있으므로
int PlcDeviceReadContinueUDPIP(DEVICE_STRUCT_UDPIP *udpip, char *buf, int count)
{
	ReadConnectAll();

	LOCAL_STRUCT *local;
	int i;

	local = &localStruct[udpip->port_no];

	if(local->pos_total == local->pos_curr)	return 0;

	for(i = 0; i < count && local->pos_total != local->pos_curr; i++) {
		buf[i] = local->ring[local->pos_curr];
		local->pos_curr++;
		local->pos_curr%=MAX_UDPIP_RECV_BUF;
	}

	return i;
}

int PlcDeviceWriteContinueUDPIP(DEVICE_STRUCT_UDPIP *udpip, char *buf, int count)
{
	LOCAL_STRUCT *local;
	CONNECT_STRUCT *conn;

	local = &localStruct[udpip->port_no];	

	if(local->nConnectNo == -1)	{
		return 0;
	}

	conn = (CONNECT_STRUCT*)blockConnect.GetPtr(local->nConnectNo);

	if(sendto(conn->socket, buf, count, 0, (PSOCKADDR) &udpip->dest_sin, sizeof(udpip->dest_sin)) == SOCKET_ERROR) {

		return 0;
	}

	for(int i = 0; i < count; i++) DisplaySendCode(udpip->port_no, buf[i]);

	return 1;
}

int PlcDeviceClearUDPIP(DEVICE_STRUCT_UDPIP *udpip)
{
	ReadConnectAll();	// 링을 클리어하기 전에 Recv 버퍼에 담는다.
	
	LOCAL_STRUCT *local = &localStruct[udpip->port_no];

	local->pos_total = local->pos_curr;
	
	return 1;
}

int PlcDeviceUnInitUDPIP(DEVICE_STRUCT_UDPIP *udpip)
{
	LOCAL_STRUCT *local = &localStruct[udpip->port_no];
	CONNECT_STRUCT *conn;

	if(local->ring) {
		delete local->ring;
		local->ring = NULL;
	}	

	local->nConnectNo = -1;

	if(local->nConnectNo != -1) {
		conn = (CONNECT_STRUCT*)blockConnect.GetPtr(local->nConnectNo);
		SocketUnPrepare(conn);
	}
	
	return 1;
}

int PlcDeviceGetCommErrorUDPIP(DEVICE_STRUCT_UDPIP *udpip, COMSTAT *comStat)
{
	return 1;
}

