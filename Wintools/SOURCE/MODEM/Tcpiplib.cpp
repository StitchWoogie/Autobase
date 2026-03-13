// english O.K
#include "stdafx.h"
#include <compiler.hpp>
#include <stdlib.h>
#include <string.h>
#include <dos.h>
//#include <winsock.h>

#include <tools.h>
#include <tcpiplib.h> 

#define	VERSION_MAJOR	1
#define	VERSION_MINOR	0

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
	char buf[160];

	MakeErrorString(buf, err);

#if	defined (_WIN32)
	MessageBox(hwnd, buf, title, MB_OK | MB_ICONERROR);
#else
	MessageBox(hwnd, buf, title, MB_OK | MB_ICONASTERISK);
#endif
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

static int FillAddr(HWND hwnd, PSOCKADDR_IN psin, TCPIP_STRUCT *tcpip)
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

	psin->sin_port = htons(tcpip->port);        /* Convert to network ordering */

	return 1;
}

//------------------------------------------------------------------------------
//	TCP/IP를 설치한다.
//------------------------------------------------------------------------------

int TcpipInstall(HWND hwnd, TCPIP_STRUCT *tcpip, char message_flag)
{
	int retn;
	char title[80];

	// 스트럭쳐 천체를 초기화 한다.
	tcpip->socket = INVALID_SOCKET;
	tcpip->bConnect = OFF;
//	tcpip->error_count = 0;
		
	tcpip->socket = socket(PF_INET, SOCK_STREAM, 0);

	if(tcpip->socket == INVALID_SOCKET)	{
		retn = WSAGetLastError();
      if(message_flag)	DisplayError(hwnd, "socket() error", retn);
		return 0;
	}

	SOCKADDR_IN dest_sin;  // DESTination Socket INternet

	if (!FillAddr( hwnd, &dest_sin, tcpip)) {
		closesocket(tcpip->socket);
		tcpip->socket = INVALID_SOCKET;
		return 0;
	}
				
	if (connect(tcpip->socket, (PSOCKADDR) &dest_sin, sizeof( dest_sin)) == SOCKET_ERROR) {
		closesocket(tcpip->socket);
		
		retn = WSAGetLastError();
		if(message_flag) {
			sprintf(title, "connect() error - Host(%s),Port(%d)", tcpip->ip, tcpip->port);
			DisplayError(hwnd, title, retn);
		}

		tcpip->socket = INVALID_SOCKET;
		return 0;
	}

    char buf[80];
	int max = 4;

	if(getsockopt(tcpip->socket, SOL_SOCKET, SO_RCVBUF, buf, &max) == 0) {
	   memcpy(&max, buf, 4);
	}


	tcpip->bConnect = ON;	// 서버에 연결된 상태이다.

	return 1;
}

int TcpipReadContinue(TCPIP_STRUCT *tcpip, char far *buf, int count)
{
	if(tcpip->bConnect == OFF)	return 0;

	unsigned long remain;
	int retn;

	if(ioctlsocket(tcpip->socket, FIONREAD, &remain) != 0)	return 0;

	if(remain == 0)	return 0;

	if(remain < (unsigned long)count)	count = (int) remain;

	retn = recv(tcpip->socket, buf, count, 0);

	if(retn < 0)	retn = 0;

	return retn;
}

int TcpipWriteContinue(TCPIP_STRUCT *tcpip, char far *buf, int count)
{
	if(tcpip->bConnect == OFF)	return 0;

	int size;
	TimeOutClass timeout;

	timeout.Reset();
 
	while(1) {
		if(timeout.IsTimeOut(3))	return 0;
		if(count < 80)		size = count;
		else				size = 80;

		size = send(tcpip->socket, buf, count, 0);
		if(size == SOCKET_ERROR)	return 0;

		break;
		/*
		) {
			//bell();
			return 0;
		}

		count -= size;
		if(count <= 0)	break;
		*/
	}
	return 1;
}

void TcpipClear(TCPIP_STRUCT *tcpip)
{
	char buf[10];
	TimeOutClass timeout;

	timeout.Reset();

	while(1) {
		if(timeout.IsTimeOut(2))	break;
		if(TcpipReadContinue(tcpip, buf, 1) == 0)	break;
	}

	return;
}

int TcpipUninstall(TCPIP_STRUCT *tcpip)
{
	if(tcpip->socket != INVALID_SOCKET)	{
		//shutdown(tcpip->socket, 1);
		tcpip->bConnect = OFF;

		if(closesocket(tcpip->socket) == SOCKET_ERROR) {
			//int err = WSAGetLastError();
		}
		tcpip->socket = INVALID_SOCKET;
	}

	return 1;
}

