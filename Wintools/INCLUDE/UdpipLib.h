#if	!defined (__UDPIPLIB_H)
#define __UDPIPLIB_H

/*
#if	!defined (__WINSOCK_H)
#include <winsock.h>
#endif
*/
#include <afxsock.h>

#pragma pack(push, 1)

#define MAX_UDPIP_RECV_BUF	4096

typedef struct {
	SOCKET	socket;			// TCP/IP의 socket
	char 	ip[80];			// 128.1.9.1  or hansol.com.co.kr
	WORD	port;			// service port
	char    ring[MAX_UDPIP_RECV_BUF];
	WORD	pos_total;
	WORD	pos_curr;
} UDPIP_STRUCT;

int UdpipInstall(HWND hwnd, UDPIP_STRUCT *udpip, char message_flag);
int UdpipUninstall(UDPIP_STRUCT *udpip);

int UdpipWriteContinue(UDPIP_STRUCT *udpip, char *string, int size);
int UdpipReadContinue (UDPIP_STRUCT *udpip, char *string, int size);
int UdpipWriteString(UDPIP_STRUCT *udpip, LPSTR string, ...);

int UdpipWrite(UDPIP_STRUCT *udpip, char ch);
int UdpipStatus(UDPIP_STRUCT *udpip);

int UdpipWaitOK(UDPIP_STRUCT *udpip);
int UdpipWaitString(UDPIP_STRUCT *udpip, char *string);
void UdpipClear(UDPIP_STRUCT *udpip);

//int UdpipHangUp(UDPIP_STRUCT *udpip);

#pragma pack(pop)

#endif



