#if	!defined (__TCPIPLIB_H)
#define __TCPIPLIB_H
/*
#if	!defined (__WINDOWS_H)
#include <windows.h>
#endif

#if	!defined (__WINSOCK_H)
#include <winsock.h>
#endif
*/
#include <afxsock.h>

typedef struct {
	SOCKET	socket;	// TCP/IP의 socket
	char 		ip[80];	// 128.1.9.1  or hansol.com.co.kr
	WORD		port;		// service port
	char 		bConnect;	// 현재 접속중인가 ?
} TCPIP_STRUCT;

int TcpipInstall(HWND hwnd, TCPIP_STRUCT *tcpip, char message_flag);
int TcpipUninstall(TCPIP_STRUCT *tcpip);

int TcpipWriteContinue(TCPIP_STRUCT *tcpip, char *string, int size);
int TcpipReadContinue (TCPIP_STRUCT *tcpip, char *string, int size);
int TcpipWriteString(TCPIP_STRUCT *tcpip, LPSTR string, ...);

int TcpipWrite(TCPIP_STRUCT *tcpip, char ch);
int TcpipStatus(TCPIP_STRUCT *tcpip);

int TcpipWaitOK(TCPIP_STRUCT *tcpip);
int TcpipWaitString(TCPIP_STRUCT *tcpip, char *string);
void TcpipClear(TCPIP_STRUCT *tcpip);

//int TcpipHangUp(TCPIP_STRUCT *tcpip);

#endif



