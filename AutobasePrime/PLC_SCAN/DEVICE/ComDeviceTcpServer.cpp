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
		case WSAETIMEDOUT://            (WSABASEERR+60)
			sprintf(buf, "WSAETIMEDOUT-Attempt to connect timed out without establishing a connection.\n(ErrorCode-%d)", err);
			break;
		default:
			sprintf(buf, "WSAERROR (ErrorCode=%d)", err);
			break;
	}
}

static void DisplayError(HWND hwnd, const char *title, int err)
{
	StackChar buf(5000);

	MakeErrorString(buf.data, err);

	strcat(buf.data, "\n");
	strcat(buf.data, title);

	MessageDisplay(buf.data);
	
}

static int FillAddrServer(DEVICE_STRUCT_TCP_SERVER *tcpip, PSOCKADDR_IN psin)
{
	psin->sin_family = AF_INET;
	psin->sin_addr.s_addr = INADDR_ANY;
	psin->sin_port = htons(tcpip->tcp_port);        // Convert to network ordering

	return 1;
}

static int PrepareListenSocket(HWND hwnd, DEVICE_STRUCT_TCP_SERVER *conn)
{
	int  retn;

	// 스트럭처 전체를 초기화 한다.
	conn->sock_listen = INVALID_SOCKET;

	conn->sock_listen = socket(PF_INET, SOCK_STREAM, 0);

	if(conn->sock_listen == INVALID_SOCKET)	{
		retn = WSAGetLastError();
		DisplayError(hwnd, "socket() error", retn);
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
		CString msg;
		msg.Format("bind() error - tcp_port:%d", conn->tcp_port);
		DisplayError(hwnd, msg, retn);

		return 0;
	}

	if(listen (conn->sock_listen, 1) == SOCKET_ERROR) {
		retn = WSAGetLastError();
		closesocket(conn->sock_listen);
		conn->sock_listen = INVALID_SOCKET;
		DisplayError(hwnd, "listen() error", retn);
		return 0;
	}
	WSAAsyncSelect (conn->sock_listen, hwndMainFrame, WM_SOCKET_MSG, FD_ACCEPT);

	return 1;
}

int PlcDeviceInitTcpServer(HWND hwnd, DEVICE_STRUCT_TCP_SERVER *tcpip, CommaBlockString *comma)
{
	// 스트럭쳐 전체를 초기화 한다.
	tcpip->socket = INVALID_SOCKET;
	tcpip->bConnect = false;
	comma->GetWORD(tcpip->tcp_port);
	tcpip->sec_hap = 0;

	comma->GetInt(tcpip->nTimeoutReset);
	if(tcpip->nTimeoutReset <= 0)	
		tcpip->nTimeoutReset = 60;

	return PrepareListenSocket(hwnd, tcpip);
}

static bool RetryConnect(DEVICE_STRUCT_TCP_SERVER *tcpip)
{
	struct time t;

	gettime(&t);
	if(t.ti_sec != tcpip->old_sec) {
		tcpip->old_sec = t.ti_sec;
		tcpip->sec_hap++; 

		if(tcpip->sec_hap >= tcpip->nTimeoutReset) {
			tcpip->sec_hap = 0;
			PlcDeviceUnInitTcpServer(tcpip);
			PrepareListenSocket(hwndMainFrame, tcpip);

			return false;
		}
	}

	if(tcpip->bConnect == false)	return false;

	
	return true;	
}

static int PlcDeviceReadContinueTcpServer_Crypto(DEVICE_STRUCT_TCP_SERVER *tcpip, char *buf, int count)
{
	if(RetryConnect(tcpip) == false)	return 0;

	int retn = tcpip->pCC->GetDecryptionData((BYTE*)buf, count);
	if(retn > 0) return retn;	// 남아있는 Decoding 이 있으면 그것을 돌려준다.

	unsigned long remain;

	if(ioctlsocket(tcpip->socket, FIONREAD, &remain) != 0)	return 0;

	if((int)remain == 0)	return 0;

	char *data_enc = new char[remain];

	retn = recv(tcpip->socket, data_enc, remain, 0);

	if(retn < 0)	retn = 0;

	if(retn > 0) {
		for(int i = 0; i < retn; i++) {
			DisplayRecvCode(tcpip->port_no, buf[i]);
			tcpip->pCC->SetEncryptedData(data_enc[i]);	// 데이터를 넣는다.
		}

		DisplayRecvCodeNextLine(tcpip->port_no);

		retn = tcpip->pCC->GetDecryptionData((BYTE*)buf, count);		// 

		tcpip->sec_hap = 0;
	}

	delete data_enc;
	
	return retn;
}

int PlcDeviceReadContinueTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, char *buf, int count)
{
	// 기존에 잘되는 루틴이 문제가 생길 수 있어 암호화 할때만 새로만든 함수를 호출해서 사용한다. 
	// ??_Crypto 함수를 그대로 대치해도 된다. 2016-10-28
	if(tcpip->pCC->bUseEncryption) {
		return PlcDeviceReadContinueTcpServer_Crypto(tcpip, buf, count);
	}

	if(RetryConnect(tcpip) == false)	return 0;

	unsigned long remain;
	int retn;

	if(ioctlsocket(tcpip->socket, FIONREAD, &remain) != 0)	return 0;

	if((int)remain == 0)	return 0;

	if((int)remain > count) {
		remain = count;
	}
	 
	retn = recv(tcpip->socket, buf, remain, 0);

	if(retn < 0)	retn = 0;

	if(retn > 0) {
		for(int i = 0; i < retn; i++) {
			DisplayRecvCode(tcpip->port_no, buf[i]);
		}

		tcpip->sec_hap = 0;
	}
	
	return retn;
}

void CloseListenSocket(DEVICE_STRUCT_TCP_SERVER *conn)
{
	if(conn->sock_listen != INVALID_SOCKET) {
		shutdown(conn->sock_listen, 0);
		closesocket(conn->sock_listen);
		conn->sock_listen = INVALID_SOCKET;
	}
}

static int PlcDeviceWriteContinueTcpServer_Crypto(DEVICE_STRUCT_TCP_SERVER *tcpip, char *source_buffer, int source_count)
{
	if(RetryConnect(tcpip) == false)	return 1;
	
	if(tcpip->bConnect == false)		return 1;

	int count;
	BYTE *buf = tcpip->pCC->GetEncryptionData((BYTE*)source_buffer, source_count, count);

	if(send(tcpip->socket, (char*)buf, count, 0) == SOCKET_ERROR) {
		PlcDeviceUnInitTcpServer(tcpip);
		PrepareListenSocket(hwndMainFrame, tcpip);

		return 0;
	}

	for(int i = 0; i < count; i++) DisplaySendCode(tcpip->port_no, buf[i]);

	return 1;
}

int PlcDeviceWriteContinueTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, char *buf, int count)
{
	// 기존에 잘되는 루틴이 문제가 생길 수 있어 암호화 할때만 새로만든 함수를 호출해서 사용한다. 
	// ??_Crypto 함수를 그대로 대치해도 된다. 2016-10-28
	if(tcpip->pCC->bUseEncryption) {
		return PlcDeviceWriteContinueTcpServer_Crypto(tcpip, buf, count);
	}

	if(RetryConnect(tcpip) == false)	return 1;
	
	if(tcpip->bConnect == false)		return 1;

	if(send(tcpip->socket, buf, count, 0) == SOCKET_ERROR) {
		PlcDeviceUnInitTcpServer(tcpip);
		PrepareListenSocket(hwndMainFrame, tcpip);

		return 0;
	}

	for(int i = 0; i < count; i++) DisplaySendCode(tcpip->port_no, buf[i]);

	return 1;
}

int PlcDeviceClearTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip)
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

int PlcDeviceUnInitTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip)
{
	CloseListenSocket(tcpip);

	if(tcpip->socket != INVALID_SOCKET)	{
		shutdown(tcpip->socket, SD_SEND);
		Sleep(500);
		
		tcpip->bConnect = false;

		if(closesocket(tcpip->socket) == SOCKET_ERROR) {
			int err = WSAGetLastError();
		}
		tcpip->socket = INVALID_SOCKET;
	}
	
	return 1;
}

int PlcDeviceEnableTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, char flag)
{
	return 1;
}

int PlcDeviceGetCommErrorTcpServer(DEVICE_STRUCT_TCP_SERVER *tcpip, COMSTAT *comStat)
{
	return 1;
}



void WmSocketMessageDeviceTcpServer(HWND /*hwnd*/, WPARAM wParam, LPARAM lParam)
{
	int i;
	GLOBAL_PORT_STRUCT *pt; 

	switch (WSAGETSELECTEVENT(lParam)) {
		// 수동 연결 접수 가능 Event
		case FD_ACCEPT: 
			{
				for(i = 0; i < MAX_PORT; i++) {
					pt = &portBuf[i];

					if(pt->local.device.nDeviceStyle == DEVICE_TYPE_TCP_SERVER) {
						DEVICE_STRUCT_TCP_SERVER *tcpip;
						tcpip = (DEVICE_STRUCT_TCP_SERVER*)pt->local.device.pData;

						if(tcpip->sock_listen == wParam) {
							SOCKADDR_IN sin;

							int addrlen = sizeof(sin);

							// 현재 상태 검사

							if (WSAGETSELECTERROR(lParam)) {
								return;
							}

							// accept()하고 수동 연결을 위한 LISTEN 소켓을 닫음
							tcpip->socket = accept(tcpip->sock_listen,
														(struct sockaddr *)&sin, &addrlen);

							if(tcpip->socket == INVALID_SOCKET)	return;

							tcpip->bConnect = true;

							strcpy(tcpip->client_ip, inet_ntoa(sin.sin_addr));
							
							// 대화 상태로 들어감
							// FD_READ, FD_WRITE, FD_CLOSE 메시지 수신 지정

							WSAAsyncSelect (tcpip->socket, hwndMainFrame, WM_SOCKET_MSG, FD_CLOSE);

							CloseListenSocket(tcpip);

							CString msg;
							msg.Format("Client Connected. (IP=%s)", tcpip->client_ip);

							DisplaySendString(tcpip->port_no, msg);
							DisplaySendCodeNextLine(tcpip->port_no);

							//ConnectStatusChanged();	// 현재의 connect display상태를 갱신한다.

							return;
						}
					}
					/*
         			conn = &scanServerList[i];

					if(wParam != conn->sock_listen) 	continue;

					SOCKADDR_IN sin;

					int addrlen = sizeof(sin);

					// 현재 상태 검사

					if (WSAGETSELECTERROR(lParam)) {
						return;
					}

					memset(&tcpip, 0, sizeof(TCPIP_STRUCT));

					// accept()하고 수동 연결을 위한 LISTEN 소켓을 닫음
					tcpip.socket = accept(conn->sock_listen,
												(struct sockaddr *)&sin, &addrlen);

					if(tcpip.socket == INVALID_SOCKET)	return;

					strcpy(tcpip.ip, inet_ntoa(sin.sin_addr));
					tcpip.port = conn->tcpipPort;

					// 대화 상태로 들어감
					// FD_READ, FD_WRITE, FD_CLOSE 메시지 수신 지정
					WSAAsyncSelect (tcpip.socket, hwndMainFrame, WM_SOCKET_MSG, FD_CLOSE);

					conn->hGate = GateRegister(&tcpip);

					CloseListenSocket(conn);

					ConnectStatusChanged();	// 현재의 connect display상태를 갱신한다.

					return;
					*/
				}
			}
			break;
		case FD_READ:
			break;
		case FD_CLOSE: {
			for(i = 0; i < MAX_PORT; i++) {
				pt = &portBuf[i];
				if(pt->local.device.nDeviceStyle == DEVICE_TYPE_TCP_SERVER) {
					DEVICE_STRUCT_TCP_SERVER *tcpip;
					tcpip = (DEVICE_STRUCT_TCP_SERVER*)pt->local.device.pData;

					if(tcpip->socket == wParam) {
						tcpip->socket = INVALID_SOCKET;
						tcpip->bConnect = false;
						DisplaySendString(tcpip->port_no, "Client Closed.");
						DisplaySendCodeNextLine(tcpip->port_no);
						PrepareListenSocket(hwndMainFrame, tcpip);
						return;
					}
				}
				/*
				if(conn->hGate) {
					if(GateGetConnectType(conn->hGate) == GATE_TYPE_TCPIP) {
						GateGetStruct(conn->hGate, &tcpip);
						if(tcpip.socket == (SOCKET)wParam) {

							conn->bSignal_FD_CLOSE = true;	// TCP socket Close 가 들어오면 이 플래그를 살려준다.
							
							return;
						}
					}
				}*/
			}
			break;
		}
	}
}

