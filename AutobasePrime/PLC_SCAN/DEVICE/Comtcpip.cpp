// english O.K
//------------------------------------------------------------------------------
//	TCP/IP 의 통신을 담당한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <tools.h>
#include <dataswap.h>

#include <winsock2.h>

#include "..\plc_scan.h"
#include "commmain.h" 

#define	VERSION_MAJOR	1
#define	VERSION_MINOR	0

static void MakeErrorString(char *buf, int err)
{
	// GetLastErrorClass error;

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
		case WSAETIMEDOUT://            (WSABASEERR+60)
			sprintf(buf, "WSAETIMEDOUT-Attempt to connect timed out without establishing a connection.\n(ErrorCode-%d)", err);
			break;
		default:
			//error.GetErrorString(buf, 160);
			
			sprintf(buf, "WSAERROR (ErrorCode=%d)", err);
			break;
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

static int FillAddr(HWND hwnd, PSOCKADDR_IN psin, DEVICE_STRUCT_TCPIP *tcpip)
{
	PHOSTENT phe;
	int retn;
	char title[80];

	psin->sin_family = AF_INET;

	if(IsNumber(tcpip->ip)) {
		psin->sin_addr.s_addr = inet_addr(tcpip->ip);
	}
	else {
		phe = gethostbyname(tcpip->ip);
   
		if (phe == NULL) {
			retn = WSAGetLastError();
			sprintf(title, "gethostbyname() error HostName(%s)", tcpip->ip);
			DisplayError(hwnd, title, retn);

			return 0;
		}
		memcpy((char FAR *)&(psin->sin_addr), phe->h_addr,
				 phe->h_length);
	}

	psin->sin_port = htons(tcpip->port[tcpip->nCurrPort]);        // Convert to network ordering 

	return 1;
}

static int FillAddrServer(DEVICE_STRUCT_TCPIP *tcpip, PSOCKADDR_IN psin)
{
	psin->sin_family = AF_INET;
	psin->sin_addr.s_addr = INADDR_ANY;
	psin->sin_port = htons(tcpip->port[tcpip->nCurrPort]);        // Convert to network ordering

	return 1;
}

int PlcDeviceInitTCPIP(HWND hwnd, DEVICE_STRUCT_TCPIP *tcpip, CommaBlockString *comma)
{
	int retn;
	char title[80];
	int i;

	// 스트럭쳐 전체를 초기화 한다.
	tcpip->socket = INVALID_SOCKET;
	tcpip->bConnect = OFF;
	comma->GetString(tcpip->ip, sizeof(tcpip->ip));
	for(i = 0; i < 8; i++) {
		comma->GetWORD(tcpip->port[i]);
	}
	tcpip->nCurrPort = 0;
	tcpip->error_count = 0;
		
	tcpip->socket = socket(PF_INET, SOCK_STREAM, 0);

	if(tcpip->socket == INVALID_SOCKET)	{
		retn = WSAGetLastError();
		DisplayError(hwnd, "socket() error", retn);
		return 0;
	}

	GetLocalTime(&tcpip->tConnect);

	/*
	char timeout_size;
	int  size = 80;

	retn = getsockopt(tcpip->socket, SOL_SOCKET, SO_RCVTIMEO, &timeout_size, &size);

	if(retn == INVALID_SOCKET) {
		retn = WSAGetLastError();
		if(msg_flag)
			DisplayError(hwnd, "getsockopt() error", retn);	
		closesocket(tcpip->socket);
		tcpip->socket = INVALID_SOCKET;
		return 0;
	}
	*/

	SOCKADDR_IN dest_sin;  // DESTination Socket INternet 

	/*
	if (!FillAddrServer(tcpip, &dest_sin)) {
		closesocket(tcpip->socket);
		tcpip->socket = INVALID_SOCKET;
		return 0;
	}

	if (bind(tcpip->socket, (struct sockaddr*)&dest_sin, sizeof(dest_sin)) == SOCKET_ERROR){ 
		closesocket(tcpip->socket);
		tcpip->socket = INVALID_SOCKET;
		retn = WSAGetLastError();
		if(msg_flag)
			DisplayError(hwnd, title, retn);
		//DisplayError("bind() error", retn);
		return 0;
	}
	*/

	if (!FillAddr(hwnd, &dest_sin, tcpip)) {
		closesocket(tcpip->socket);
		tcpip->socket = INVALID_SOCKET;
		return 0;
	}
				
	if (connect( tcpip->socket, (PSOCKADDR) &dest_sin, sizeof( dest_sin)) == SOCKET_ERROR) {
		retn = WSAGetLastError();
	
		sprintf(title, "connect() error - Host(%s),Port(%d)", tcpip->ip, tcpip->port[tcpip->nCurrPort]);
		DisplayError(hwnd, title, retn);
		
		//tcpip->socket = INVALID_SOCKET;
		tcpip->bConnect = OFF;	// connect 가 되지 않더라도 계속 시도한다.
		return 1;
	}

	GetLocalTime(&tcpip->tConnect);
	tcpip->bConnect = ON;	// 서버에 연결된 상태이다.

	{
		int opt_size = 4;
		char opt_value[80];
		int opt_recv_size = 60000;

		memcpy(opt_value, &opt_recv_size, 4);

		// default SO_RCVBUF = 8192

		int ex = getsockopt(tcpip->socket, SOL_SOCKET, SO_RCVBUF, opt_value, &opt_size);

		setsockopt(tcpip->socket, SOL_SOCKET, SO_RCVBUF, opt_value, opt_size);
		ex = getsockopt(tcpip->socket, SOL_SOCKET, SO_RCVBUF, opt_value, &opt_size);
		if(opt_size == 4) {
			memcpy(&opt_size, opt_value, 4);
		}
	}
	
	return 1;
}

static int RetryConnect(DEVICE_STRUCT_TCPIP *tcpip)
{
	if(tcpip->socket == INVALID_SOCKET)	return 0;

	if(tcpip->bConnect == OFF) {
		struct time t;

		gettime(&t);
		if(t.ti_sec != tcpip->old_sec) {
			tcpip->old_sec = t.ti_sec;
			tcpip->error_count++; 	
		}
	}

	char message[160];
	int err;

	if(tcpip->error_count > 1) {   // 접속이 된 상태에서 끊어졌다. 이경우는 PLC reset인 경우이므로 다시 접속을 시도한다.  복구가 늦는것 같아서 횟수를 1로 바꾸었다.2009.10.23
		// 에러가 날때는 port를 바꾸어서 계속해 본다.
		int savePort = tcpip->nCurrPort;
		while(1) {
			tcpip->nCurrPort ++;
			tcpip->nCurrPort %= 8;
			if(tcpip->nCurrPort == savePort)	break;		// 다시 원 위치로 돌아왔을때는 그냥 break한다.
			if(tcpip->port[tcpip->nCurrPort] != 0)	break;	// 번호가 0이 아니면 그 포트를 사용한다.
		}

		tcpip->error_count = 0;
		
		SOCKADDR_IN dest_sin;  // DESTination Socket INternet 

		closesocket(tcpip->socket);
		tcpip->bConnect = OFF;
		tcpip->socket = socket(PF_INET, SOCK_STREAM, 0);

		if (!FillAddr( NULL, &dest_sin, tcpip)) {
			return 0;
		}
			
		sprintf(message, "Tcp/ip connect() Try - IP:%s, Port:%d", tcpip->ip, tcpip->port[tcpip->nCurrPort]);

		if (connect(tcpip->socket, (PSOCKADDR) &dest_sin, sizeof( dest_sin)) == SOCKET_ERROR) {
			err = WSAGetLastError();
			sprintf(message, "Tcp/ip connect() error - ErrorCode(%d) IP:%s, Port:%d", err, tcpip->ip, tcpip->port[tcpip->nCurrPort]);
			MessageDisplay(message);
			tcpip->bConnect = OFF;
			return 0;
		}

		GetLocalTime(&tcpip->tConnect);
		tcpip->bConnect = ON;

		return 1;
	}

	return 1;	
}

static int PlcDeviceReadContinueTCPIP_Crypto(DEVICE_STRUCT_TCPIP *tcpip, char *buf, int count)
{
	if(RetryConnect(tcpip) == 0)	return 0;

	int retn = tcpip->pCC->GetDecryptionData((BYTE*)buf, count);
	if(retn > 0) return retn;	// 남아있는 Decoding 이 있으면 그것을 돌려준다.

	unsigned long remain;

	if(ioctlsocket(tcpip->socket, FIONREAD, &remain) != 0)	return 0;

	if((int)remain == 0)	return 0;

	char *data_enc = new char[remain];

	retn = recv(tcpip->socket, data_enc, remain, 0);

	if(retn < 0)	retn = 0;

	if(retn) {
		for(int i = 0; i < retn; i++) {
			DisplayRecvCode(tcpip->port_no, data_enc[i]);

			tcpip->pCC->SetEncryptedData(data_enc[i]);	// 데이터를 넣는다.
		}

		DisplayRecvCodeNextLine(tcpip->port_no);

		retn = tcpip->pCC->GetDecryptionData((BYTE*)buf, count);		// 
	}

	delete data_enc;

	return retn;
}

int PlcDeviceReadContinueTCPIP(DEVICE_STRUCT_TCPIP *tcpip, char *buf, int count)
{
	// 기존에 잘되는 루틴이 문제가 생길 수 있어 암호화 할때만 새로만든 함수를 호출해서 사용한다. 
	// ??_Crypto 함수를 그대로 대치해도 된다. 2016-10-28
	if(tcpip->pCC->bUseEncryption) {
		return PlcDeviceReadContinueTCPIP_Crypto(tcpip, buf, count);
	}

	if(RetryConnect(tcpip) == 0)	return 0;

	unsigned long remain;
	int retn;

	if(ioctlsocket(tcpip->socket, FIONREAD, &remain) != 0)	return 0;

	if((int)remain == 0)	return 0;

	if((int)remain > count) {
		remain = count;
	}
	 
	retn = recv(tcpip->socket, buf, remain, 0);

	if(retn < 0)	retn = 0;

	if(retn) {
		for(int i = 0; i < retn; i++) {
			DisplayRecvCode(tcpip->port_no, buf[i]);
		}
	}
	
	return retn;
}

static int PlcDeviceWriteContinueTCPIP_Crypto(DEVICE_STRUCT_TCPIP *tcpip, char *source_buffer, int source_count)
{
	if(RetryConnect(tcpip) == 0)	return 1;
	
	if(tcpip->bConnect == OFF)		return 1;

	int count;
	BYTE *buf = tcpip->pCC->GetEncryptionData((BYTE*)source_buffer, source_count, count);

	if(send(tcpip->socket, (char*)buf, count, 0) == SOCKET_ERROR) {
		tcpip->error_count ++;

		int err = WSAGetLastError();
		char message[160];

		sprintf(message, "TCP/IP send() error - ErrorCode(%d) IP:%s, Port:%d", err, tcpip->ip, tcpip->port[tcpip->nCurrPort]);
		MessageDisplay(message);

		return 0;
	}
	else {
		tcpip->error_count = 0; 
	}

	for(int i = 0; i < count; i++) DisplaySendCode(tcpip->port_no, buf[i]);

	return 1;
}

int PlcDeviceWriteContinueTCPIP(DEVICE_STRUCT_TCPIP *tcpip, char *buf, int count)
{
	// 기존에 잘되는 루틴이 문제가 생길 수 있어 암호화 할때만 새로만든 함수를 호출해서 사용한다. 
	// ??_Crypto 함수를 그대로 대치해도 된다. 2016-10-28
	if(tcpip->pCC->bUseEncryption) {
		return PlcDeviceWriteContinueTCPIP_Crypto(tcpip, buf, count);
	}

	if(RetryConnect(tcpip) == 0)	return 1;
	
	if(tcpip->bConnect == OFF)		return 1;

	if(send(tcpip->socket, (char*)buf, count, 0) == SOCKET_ERROR) {
		tcpip->error_count ++;

		int err = WSAGetLastError();
		char message[160];

		sprintf(message, "TCP/IP send() error - ErrorCode(%d) IP:%s, Port:%d", err, tcpip->ip, tcpip->port[tcpip->nCurrPort]);
		MessageDisplay(message);

		return 0;
	}
	else {
		tcpip->error_count = 0; 
	}

	for(int i = 0; i < count; i++) DisplaySendCode(tcpip->port_no, buf[i]);

	return 1;
}

int PlcDeviceClearTCPIP(DEVICE_STRUCT_TCPIP *tcpip)
{
	if(tcpip->bConnect == OFF)	return 1;

	unsigned long remain;
	int retn;

	if(ioctlsocket(tcpip->socket, FIONREAD, &remain) != 0)	return 1;

	char imsi[10];
	int i;

	for(i = 0; i < (int)remain; i++) {
		retn = recv(tcpip->socket, imsi, 1, 0);
		if(retn != 1)	return 1;
	}

	return 1;
}

/*
static SOCKET socketToClose = INVALID_SOCKET;

void CheckTcpDeviceClose(WPARAM wParam, LPARAM lParam)
{
	if(wParam == socketToClose && LOWORD(lParam) == FD_CLOSE)
		socketToClose = INVALID_SOCKET;
}
*/



int PlcDeviceUnInitTCPIP(DEVICE_STRUCT_TCPIP *tcpip)
{
	if(tcpip->socket != INVALID_SOCKET)	{
		shutdown(tcpip->socket, SD_SEND);
		Sleep(500);

		/*
		//
		WSAAsyncSelect(tcpip->socket, hwndMainFrame, WM_SOCKET_MSG, FD_CLOSE);
		shutdown(tcpip->socket, SD_SEND);
		
		TimeOutMiliSecClass timeout;
		socketToClose = tcpip->socket;
		while(1) {
			if(timeout.IsTimeOut(5000))	break;
			if(socketToClose != tcpip->socket)	break;
			Sleep(1);
		}
		PlcDeviceClearTCPIP(tcpip);
		//
		*/
		
		tcpip->bConnect = OFF;

		if(closesocket(tcpip->socket) == SOCKET_ERROR) {
			int err = WSAGetLastError();
		}
		tcpip->socket = INVALID_SOCKET;
	}
	
	return 1;
}

int PlcDeviceEnableTCPIP(DEVICE_STRUCT_TCPIP *tcpip, char flag)
{
	if(flag) {
		if(tcpip->socket == INVALID_SOCKET) {
			tcpip->socket = socket(PF_INET, SOCK_STREAM, 0);

			SOCKADDR_IN dest_sin;  // DESTination Socket INternet 
			
			if (!FillAddr( NULL, &dest_sin, tcpip)) {
				return 0;
			}
				
			//CString msg;
			//sprintf(message, "Tcp/ip connect() Try - IP:%s, Port:%d", tcpip->ip, tcpip->port[tcpip->nCurrPort]);

			if (connect(tcpip->socket, (PSOCKADDR) &dest_sin, sizeof( dest_sin)) == SOCKET_ERROR) {
				//err = WSAGetLastError();
				//sprintf(message, "Tcp/ip connect() error - ErrorCode(%d) IP:%s, Port:%d", err, tcpip->ip, tcpip->port[tcpip->nCurrPort]);
				//MessageDisplay(msg);
				tcpip->bConnect = OFF;
				return 0;
			}

			GetLocalTime(&tcpip->tConnect);
			tcpip->bConnect = ON;			
		}
		return 1;
	}
	else {
		if(tcpip->socket != INVALID_SOCKET)	{
			shutdown(tcpip->socket, SD_SEND);
			Sleep(500);
			
			tcpip->bConnect = OFF;

			if(closesocket(tcpip->socket) == SOCKET_ERROR) {
				int err = WSAGetLastError();
			}

			tcpip->socket = INVALID_SOCKET;
		}
	}
	
	return 1;
}

int PlcDeviceGetCommErrorTCPIP(DEVICE_STRUCT_TCPIP *tcpip, COMSTAT *comStat)
{
	return 1;
}

int PlcDeviceGetInfoTCPIP(DEVICE_STRUCT_TCPIP *tcpip, int item, BYTE *buf, int size)
{
	if(item == DEVICE_INFO_TCPIP_CONNECT_FLAG) {
		buf[0] = tcpip->bConnect;
		return 1;
	}
	else if(item == DEVICE_INFO_TCPIP_CONNECT_TIME) {
		buf[0] = (BYTE)(tcpip->tConnect.wYear%256);
		buf[1] = (BYTE)(tcpip->tConnect.wYear/256);
		buf[2] = (BYTE)(tcpip->tConnect.wMonth);
		buf[3] = (BYTE)(tcpip->tConnect.wDay);
		buf[4] = (BYTE)(tcpip->tConnect.wHour);
		buf[5] = (BYTE)(tcpip->tConnect.wMinute);
		buf[6] = (BYTE)(tcpip->tConnect.wSecond);
		buf[7] = (BYTE)(tcpip->tConnect.wMilliseconds)%256;
		buf[8] = (BYTE)(tcpip->tConnect.wMilliseconds)/256;
		
		return 9;
	}

	return 0;
}

