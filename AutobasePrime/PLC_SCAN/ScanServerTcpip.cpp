#include "stdafx.h"
#include <compiler.hpp>
//#include <winsock.h>
#include <winsock2.h>

#include <tools.h>        
#include <glib.h>
#include <tcpiplib.h>
#include <gatelib.h>

#include "plc_scan.h"
#include "ScanServer.h"

void ConnectStatusChanged();
void SetConnectFlag(SCAN_SERVER_LIST *conn, char flag);
//void TerminalDisplayString(char *string, ...);

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
		case WSAEADDRINUSE:
			sprintf(buf, "WSAEADDRINUSE-Already used.\n(ErrorCode-%d)", err);
			break;
		default:
			sprintf(buf, "WSAERROR (ErrorCode=%d)", err);
	}
}

static void DisplayError(const char *title, int err)
{
	char buf[320];

	MakeErrorString(buf, err);
	
	CString buf2;
	buf2.Format("PlcScan Network Memory Server TCPIP error\n%s - %s", title, buf);

	MessageDisplay(buf2);
}

static int FillAddrServer(SCAN_SERVER_LIST *conn, PSOCKADDR_IN psin)
{
	psin->sin_family = AF_INET;
	psin->sin_addr.s_addr = INADDR_ANY;
	psin->sin_port = htons(conn->tcpipPort);        // Convert to network ordering

	return 1;
}

static int PrepareSocket(SCAN_SERVER_LIST *conn)
{
	int  retn;

	// 스트럭처 전체를 초기화 한다.
	conn->sock_listen = INVALID_SOCKET;

	conn->sock_listen = socket(PF_INET, SOCK_STREAM, 0);

	if(conn->sock_listen == INVALID_SOCKET)	{
		retn = WSAGetLastError();
		DisplayError("socket() error", retn);
		return 0;
	}

	SOCKADDR_IN dest_sin;  // DESTination Socket INternet

	if (!FillAddrServer(conn, &dest_sin)) {
		closesocket(conn->sock_listen);
		conn->sock_listen = INVALID_SOCKET;
		return 0;
	}

	if (bind(conn->sock_listen, (struct sockaddr*)&dest_sin, sizeof(dest_sin)) == SOCKET_ERROR) {
		retn = WSAGetLastError();
		closesocket(conn->sock_listen);
		conn->sock_listen = INVALID_SOCKET;
		//retn = WSAGetLastError();
		CString msg;
		msg.Format("bind() error - tcp_port:%d", conn->tcpipPort);
		DisplayError(msg, retn);

		return 0;
	}

	if(listen (conn->sock_listen, 1) == SOCKET_ERROR) {
		retn = WSAGetLastError();
		closesocket(conn->sock_listen);
		conn->sock_listen = INVALID_SOCKET;
		DisplayError("listen() error", retn);
		return 0;
	}
	WSAAsyncSelect (conn->sock_listen, hwndMainFrame, WM_SOCKET_MSG, FD_ACCEPT);

//	TerminalDisplayString("One TCP/IP Socket listen ready (port-%d).\n\r", conn->tcpipPort);

	return 1;
}

int OpenListenSocket(SCAN_SERVER_LIST *conn)
{
	int retn;

	retn = PrepareSocket(conn);

	if(retn)	conn->bInitialFlag = ON;
	else		conn->bInitialFlag = OFF;

	return retn;
}

//-------------------------------------
//
//-------------------------------------

void CloseListenSocket(SCAN_SERVER_LIST *conn)
{
	if(conn->sock_listen != INVALID_SOCKET) {
		shutdown(conn->sock_listen, 0);
		closesocket(conn->sock_listen);
		conn->sock_listen = INVALID_SOCKET;
	}
}

void WmSocketMessageScanServer(HWND /*hwnd*/, WPARAM wParam, LPARAM lParam)
{
//	SERVER_SOCKET_STRUCT sock;
	int i;
	TCPIP_STRUCT tcpip;
	SCAN_SERVER_LIST *conn;

	switch (WSAGETSELECTEVENT(lParam)) {
		// 수동 연결 접수 가능 Event
		case FD_ACCEPT: {
			for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
         	conn = &scanServerList[i];

				if(wParam != conn->sock_listen) 	continue;

				SOCKADDR_IN sin;

				int addrlen = sizeof(sin);

				// 현재 상태 검사

				if (WSAGETSELECTERROR(lParam)) {
//					TerminalDisplayString("listen() - Accept() error\n\r");
					return;
				}

				memset(&tcpip, 0, sizeof(TCPIP_STRUCT));

				// accept()하고 수동 연결을 위한 LISTEN 소켓을 닫음
				tcpip.socket = accept(conn->sock_listen,
											(struct sockaddr *)&sin, &addrlen);

				if(tcpip.socket == INVALID_SOCKET)	return;

				strcpy(tcpip.ip, inet_ntoa(sin.sin_addr));
				tcpip.port = conn->tcpipPort;
//				TerminalDisplayString("터미날 IP:%s 와 TCP/IP Socket 접속.\n\r", tcpip.ip);

				// 대화 상태로 들어감
				// FD_READ, FD_WRITE, FD_CLOSE 메시지 수신 지정
				WSAAsyncSelect (tcpip.socket, hwndMainFrame, WM_SOCKET_MSG, FD_CLOSE);

				conn->hGate = GateRegister(&tcpip);

				CloseListenSocket(conn);

				ConnectStatusChanged();	// 현재의 connect display상태를 갱신한다.
				return;
			}
		}
		case FD_READ:
			break;
		case FD_CLOSE: {
			for(i = 0; i < MAX_SCAN_SERVER_LIST; i++) {
				conn = &scanServerList[i];
				if(conn->hGate) {
					if(GateGetConnectType(conn->hGate) == GATE_TYPE_TCPIP) {
						GateGetStruct(conn->hGate, &tcpip);
						if(tcpip.socket == (SOCKET)wParam) {

							conn->bSignal_FD_CLOSE = true;	// TCP socket Close 가 들어오면 이 플래그를 살려준다.
							
							/*
							GateUninstall(conn->hGate);
							conn->hGate = NULL;
							conn->bInitialFlag = OFF;
							SetConnectFlag(conn, OFF);

//							TerminalDisplayString("Socket (Terminal IP:%s) closed.\n\r", tcpip.ip);
							OpenListenSocket(conn);
							*/

							return;
						}
					}
				}
			}
			break;
		}
	}
}




