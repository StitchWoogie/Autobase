// TAG size O.K
// english O.K
#include "stdafx.h"
#include <compiler.hpp>
#include <stdlib.h>
#include <string.h>
#include <dos.h>
//#include <winsock.h>

#include <tools.h>
#include <udpiplib.h>
#include <dataswap.h>

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

static int FillAddr(HWND hwnd, PSOCKADDR_IN psin, UDPIP_STRUCT *udpip)
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

	psin->sin_port = htons(udpip->port);        /* Convert to network ordering */

	return 1;
}

static int FillAddrServer(UDPIP_STRUCT *udpip, PSOCKADDR_IN psin)
{
	psin->sin_family = AF_INET;
	psin->sin_addr.s_addr = INADDR_ANY;
	psin->sin_port = htons(udpip->port);        // Convert to network ordering

	return 1;
}

//------------------------------------------------------------------------------
//	TCP/IP를 설치한다.
//------------------------------------------------------------------------------

int UdpipInstall(HWND hwnd, UDPIP_STRUCT *udpip, char message_flag)
{
	int retn;
//	char title[80];

	// 스트럭쳐 천체를 초기화 한다.
	udpip->socket = INVALID_SOCKET;
	udpip->pos_total = 0;	// ring buf 를 초기화 한다.
	udpip->pos_curr  = 0;	
	//udpip->bConnect = OFF;
//	udpip->error_count = 0;
		
	udpip->socket = socket(PF_INET, SOCK_DGRAM, 0);

	if(udpip->socket == INVALID_SOCKET)	{
		retn = WSAGetLastError();
		if(message_flag)	DisplayError(hwnd, "socket() error", retn);
		return 0;
	}

	/*
	SOCKADDR_IN dest_sin;  // DESTination Socket INternet

	
	if (!FillAddr( hwnd, &dest_sin, udpip)) {
		closesocket(udpip->socket);
		udpip->socket = INVALID_SOCKET;
		return 0;
	}
	*/

	SOCKADDR_IN dest_sin;  // DESTination Socket INternet

	if (!FillAddrServer(udpip, &dest_sin)) {
		closesocket(udpip->socket);
		udpip->socket = INVALID_SOCKET;
		//closesocket(conn->sock_listen);
		//conn->sock_listen = INVALID_SOCKET;
		return 0;
	}

	if (bind(udpip->socket, (struct sockaddr*)&dest_sin, sizeof(dest_sin)) == SOCKET_ERROR) {
		// closesocket(conn->sock_listen);
		// conn->sock_listen = INVALID_SOCKET;
		closesocket(udpip->socket);
		udpip->socket = INVALID_SOCKET;
		// retn = WSAGetLastError();
		// DisplayError("bind() error", retn);

		return 0;
	}

	return 1;
}

int UdpipReadContinue(UDPIP_STRUCT *udpip, char *buf, int count)
{
	if(udpip->socket == INVALID_SOCKET)	return 0;

	int retn;
	u_long space;
	int i;
	u_long remain = 0;

	// 비어있는 공간이 있으면 ring 버퍼에 담는다.
	if(ioctlsocket(udpip->socket, FIONREAD, &remain) == 0) {
		space = (udpip->pos_curr+MAX_UDPIP_RECV_BUF)-udpip->pos_total-2;
		if(remain > 0 && remain <= space) {
			StackChar imsi(remain);
			if(imsi.data != NULL) {
				SOCKADDR_IN dest_sin;  // DESTination Socket INternet
				int addr_size = sizeof(dest_sin);
				retn = recvfrom(udpip->socket, imsi.data, remain, 0, (PSOCKADDR) &dest_sin, &addr_size);
				//dest_sin.sin_port = ntohs(dest_sin.sin_port);
				if(retn != SOCKET_ERROR) {
					sprintf(udpip->ip, "%d.%d.%d.%d", 
						dest_sin.sin_addr.S_un.S_un_b.s_b1, 
						dest_sin.sin_addr.S_un.S_un_b.s_b2,
						dest_sin.sin_addr.S_un.S_un_b.s_b3,
						dest_sin.sin_addr.S_un.S_un_b.s_b4);
														
					for(i = 0; i < (int)remain; i++) {
						udpip->ring[udpip->pos_total] = imsi.data[i];
						udpip->pos_total++;
						udpip->pos_total%=MAX_UDPIP_RECV_BUF;
					}
				}
			}
		}
	}

	if(udpip->pos_total == udpip->pos_curr)	return 0;

	for(i = 0; i < count && udpip->pos_total != udpip->pos_curr; i++) {
		buf[i] = udpip->ring[udpip->pos_curr];
		udpip->pos_curr++;
		udpip->pos_curr%=MAX_UDPIP_RECV_BUF;
	}

	return i;
}

int UdpipWriteContinue(UDPIP_STRUCT *udpip, char *buf, int count)
{
	// if(udpip->bConnect == OFF)	return 0;
	if(udpip->socket == INVALID_SOCKET)	return 0;

	SOCKADDR_IN dest_sin;  // DESTination Socket INternet
	int addr_size = sizeof(dest_sin);
	int retn;

	if (!FillAddr(NULL, &dest_sin, udpip)) {
		closesocket(udpip->socket);
		udpip->socket = INVALID_SOCKET;
		return 0;
	}

	retn = sendto(udpip->socket, buf, count, 0, (PSOCKADDR) &dest_sin, sizeof(dest_sin));
	if(retn == SOCKET_ERROR) {
		int retn = WSAGetLastError();
		char title[160];
		sprintf(title, "gethostbyname() error HostName(%s)", udpip->ip);
		// DisplayError(NULL, title, retn);
		return 0;
	}

	return 1;
}

void UdpipClear(UDPIP_STRUCT *udpip)
{
	char buf[10];
	TimeOutClass timeout;

	timeout.Reset();

	while(1) {
		if(timeout.IsTimeOut(2))	break;
		if(UdpipReadContinue(udpip, buf, 1) == 0)	break;
	}

	return;
}

int UdpipUninstall(UDPIP_STRUCT *udpip)
{
	if(udpip->socket != INVALID_SOCKET)	{
		// shutdown(udpip->socket, 1);
		// udpip->bConnect = OFF;

		if(closesocket(udpip->socket) == SOCKET_ERROR) {
			//int err = WSAGetLastError();
		}
		udpip->socket = INVALID_SOCKET;
	}

	return 1;
}

