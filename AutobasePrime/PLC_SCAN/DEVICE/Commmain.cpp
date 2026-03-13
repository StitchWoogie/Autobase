// english O.K
//------------------------------------------------------------------------------
//	comm main programm
// comm main 은 밑의 디렉토리에 있는 서로 다른 모든 통신 드라이브의 함수를 관리한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <string.h>
#include <stdlib.h>
  
#include <tools.h>

#include "commmain.h"  
#include "..\plc_scan.h"

extern char cFirstCommunicationFlag;

int  PlcDeviceInit(HWND hwnd, DEVICE_STRUCT *dev, char *argument_string, GLOBAL_PORT_STRUCT *pt)
{
	CommaBlockString comma;
	char device[20];
//	char argument[20];

	comma.Set(argument_string);
	comma.GetString(device, sizeof(device));

	dev->flag = OFF;
	dev->nDeviceStyle = DEVICE_TYPE_NULL;
	dev->pData = NULL;

	if(strnicmp(device, "COM", 3) == 0) {
		DEVICE_STRUCT_RS232 *rs232;
		int retn;

		rs232 = new DEVICE_STRUCT_RS232;
		if(rs232 == NULL) {
			MessageDisplay("232 initial unable because memory insufficent");
			return 0;
		}

		memset(rs232, 0, sizeof(DEVICE_STRUCT_RS232));
 
		rs232->port_no = pt->local.no;
		rs232->port = atoi(&device[3])-1;
		//strcpy(rs232->sPort, device);

		retn = PlcDeviceInitRS232(hwnd, rs232, &comma);

		// if retn < 0	than error
		if(retn == 0) {
			delete rs232;
			return 0;
		}

		rs232->pCC = pt->pCC;

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_RS232;
		dev->pData = (char*)rs232;
		
		return 1;
	}
	else if(strnicmp(device, "MODEM", 5) == 0) {
		DEVICE_STRUCT_MODEM *modem;
		int retn;

		modem = new DEVICE_STRUCT_MODEM;
		if(modem == NULL) {
			MessageDisplay("modem initial unable because memory insufficent");
			return 0;
		}

		memset(modem, 0, sizeof(DEVICE_STRUCT_MODEM));
 
		modem->port_no = pt->local.no;
		modem->rs232.port_no = pt->local.no;
		//sprintf(modem->rs232.sPort, "\\\\.\\COM%
		modem->rs232.port = atoi(&device[5])-1;
		modem->rs232.pCC = pt->pCC;						// 2022-3-8 추가함. TeleDevice에서 다운되어서 MODEM도 추가했다. 10.3.2버전 암호화 탑재부터 생긴 문제

		retn = PlcDeviceInitModem(hwnd, modem, &comma, &pt->tel);

		// if retn < 0	than error
		if(retn == 0) {
			delete modem;
			return 0;
		}

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_MODEM;
		dev->pData = (char*)modem;

		return 1;
	}
	else if(stricmp(device, "TCP/IP") == 0) {	// TCP/IP Protocol
		DEVICE_STRUCT_TCPIP *tcpip;
		int retn;

		tcpip = new DEVICE_STRUCT_TCPIP;
		if(tcpip == NULL) {
			MessageDisplay("TCP/IP initial unable because memory insufficent.");
			return 0;
		}

		retn = PlcDeviceInitTCPIP(hwnd, tcpip, &comma);

		tcpip->port_no = pt->local.no;

		if(retn == 0) {
			delete tcpip;
			return 0;
		}

		tcpip->pCC = pt->pCC;

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_TCPIP;
		dev->pData = (char*)tcpip;

		return 1;
	}
	else if(stricmp(device, "UDP/IP") == 0) {	// UDP/IP Protocol
		DEVICE_STRUCT_UDPIP *udpip;
		int retn;

		udpip = new DEVICE_STRUCT_UDPIP;
		if(udpip == NULL) {
			MessageDisplay( "UDP/IP initial unable because memory insufficent.");
			return 0;
		}

		udpip->port_no = pt->local.no;
		
		retn = PlcDeviceInitUDPIP(hwnd, udpip, &comma);

		if(retn == 0) {
			delete udpip;
			return 0;
		}

		// udpip->pCC = pt->pCC; // UDPIP는 지원하기 힘든 구조이다.

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_UDPIP;
		dev->pData = (char*)udpip;

		return 1;
	}
	else if(stricmp(device, "TCP-Server") == 0) {	// 8 까지는 DDE Client이었고 9.5.3부터는 TCP Server로 바뀌었다.
		DEVICE_STRUCT_TCP_SERVER *tcpserv;
		int retn;

		tcpserv = new DEVICE_STRUCT_TCP_SERVER;
		if(tcpserv == NULL) {
			MessageDisplay("TCP/IP initial unable because memory insufficent.");
			return 0;
		}

		retn = PlcDeviceInitTcpServer(hwnd, tcpserv, &comma);

		tcpserv->port_no = pt->local.no;

		if(retn == 0) {
			delete tcpserv;
			return 0;
		}

		tcpserv->pCC = pt->pCC;

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_TCP_SERVER;
		dev->pData = (char*)tcpserv;

		return 1;
	}
	else if(strnicmp(device, "TeleDevice", 10) == 0) {
		DEVICE_STRUCT_TELE *tele;
		int retn;

		tele = new DEVICE_STRUCT_TELE;
		if(tele == NULL) {
			MessageDisplay("TeleDevice initial unable because memory insufficent");
			return 0;
		}

		memset(tele, 0, sizeof(DEVICE_STRUCT_TELE));
 
		tele->port_no = pt->local.no;
		//sprintf(modem->rs232.sPort, "\\\\.\\COM%
		tele->rs232.port = atoi(&device[10])-1;
		tele->rs232.pCC = pt->pCC;						// 2022-3-8 추가함. TeleDevice도 232를 사용한다. 이부분이 빠져서 다운되었다. 10.3.2버전 암호화 탑재부터 생긴 문제

		retn = PlcDeviceInitTele(hwnd, tele, &comma);

		// if retn < 0	than error
		if(retn == 0) {
			delete tele;
			return 0;
		}

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_TELE;
		dev->pData = (char*)tele;

		return 1;
	}
	else if(stricmp(device, "NetClient") == 0) {	// NetClnt.exe와 통신
		DEVICE_STRUCT_NETCLIENT *net;

		net = new DEVICE_STRUCT_NETCLIENT;
		if(net == NULL) {
			MessageDisplay("NetClient initial unable because memory insufficent.");
			return 0;
		}

		memset(net, 0, sizeof(DEVICE_STRUCT_NETCLIENT));

		int retn = PlcDeviceInitNetClient(hwnd, net, &comma);

		net->port_no = pt->local.no;

		if(retn == 0) {
			delete net;
			return 0;
		}

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_NETCLIENT;
		dev->pData = (char*)net;
		
		return 1;
	}

	else if(stricmp(device, "SharedMemory") == 0) {	// NetClnt.exe와 통신
		DEVICE_STRUCT_SHAREDMEMORY *net;

		net = new DEVICE_STRUCT_SHAREDMEMORY;
		if(net == NULL) {
			MessageDisplay("SharedMemory initial unable because memory insufficent.");
			return 0;
		}

		memset(net, 0, sizeof(DEVICE_STRUCT_SHAREDMEMORY));

		int retn = PlcDeviceInitSharedMemory(hwnd, net, &comma);

		net->port_no = pt->local.no;

		if(retn == 0) {
			delete net;
			return 0;
		}

		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_SHAREDMEMORY;
		dev->pData = (char*)net;
		
		return 1;
	}
	else if(stricmp(device, "NONE") == 0) {	// Device를 사용하지 않는다.
		dev->flag = ON;
		dev->nDeviceStyle = DEVICE_TYPE_NONE;
		dev->pData = NULL;

		return 1;
	}
	else {
		return 0;
	}
}

void PlcDeviceGetInfoString(DEVICE_STRUCT *device, char *buf)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232: 
			{
				DEVICE_STRUCT_RS232 *rs232;
				rs232 = (DEVICE_STRUCT_RS232*)device->pData;
				sprintf(buf, "COM%d, %lu, %d, %d, %d", rs232->port+1, rs232->baud, rs232->parity, rs232->data, rs232->stop);
				if(rs232->pCC->bUseEncryption) 
				{
					CString info_crypto;
					info_crypto.Format(" (%s)", rs232->pCC->sEngine);
					strcat(buf, info_crypto);
				}
				break;
			}
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem;
				modem = (DEVICE_STRUCT_MODEM*)device->pData;
				sprintf(buf, "Modem%d, %lu, %d, %d, %d", modem->rs232.port+1, modem->rs232.baud, modem->rs232.parity, modem->rs232.data, modem->rs232.stop);
				break;
			}
		case DEVICE_TYPE_TELE:
			{
				DEVICE_STRUCT_TELE *tele;
				tele = (DEVICE_STRUCT_TELE*)device->pData;
				sprintf(buf, "TeleDevice%d, %lu, %d, %d, %d", tele->rs232.port+1, tele->rs232.baud, tele->rs232.parity, tele->rs232.data, tele->rs232.stop);
				break;
			}
		case DEVICE_TYPE_TCPIP:
			{
				DEVICE_STRUCT_TCPIP *tcpip;
				tcpip = (DEVICE_STRUCT_TCPIP*)device->pData;
				sprintf(buf, "TCP/IP, HostIP:%s, Port:%d", tcpip->ip, tcpip->port[tcpip->nCurrPort]);

				if(tcpip->pCC->bUseEncryption) 
				{
					CString info_crypto;
					info_crypto.Format(" (%s)", tcpip->pCC->sEngine);
					strcat(buf, info_crypto);
				}
			}
			break;
		case DEVICE_TYPE_UDPIP:
			{
				DEVICE_STRUCT_UDPIP *udpip;
				udpip = (DEVICE_STRUCT_UDPIP*)device->pData;
				sprintf(buf, "UDP/IP, HostIP:%s, Port:%d", udpip->ip, udpip->port);

				
			}
			break;
		case DEVICE_TYPE_TCP_SERVER:
			{
				DEVICE_STRUCT_TCP_SERVER *tcpip;
				tcpip = (DEVICE_STRUCT_TCP_SERVER*)device->pData;
				sprintf(buf, "TCP-Server, Port:%d, TO:%d", tcpip->tcp_port, tcpip->nTimeoutReset);

				if(tcpip->pCC->bUseEncryption) 
				{
					CString info_crypto;
					info_crypto.Format(" (%s)", tcpip->pCC->sEngine);
					strcat(buf, info_crypto);
				}
			}
			break;
		case DEVICE_TYPE_NONE:
			{
				sprintf(buf, "Device Not Used");
			}
			break;
		case DEVICE_TYPE_NETCLIENT:
			{
				sprintf(buf, "NetClient Device");
			}
			break;
		case DEVICE_TYPE_SHAREDMEMORY:
			{
				sprintf(buf, "SharedMemory Device");
			}
			break;
		default:
			if(IsLangKorean()) {
						strcpy(buf, "Device 미설정 상태");
			}
			else {
						strcpy(buf, "Device not prepared");
			}
			break;
	}
}

int PlcDeviceReadContinue(DEVICE_STRUCT *device, char *buf, int count)
{
	int retn = 0;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceReadContinueRS232((DEVICE_STRUCT_RS232*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_MODEM:
			retn = PlcDeviceReadContinueModem((DEVICE_STRUCT_MODEM*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_TCPIP:
			retn = PlcDeviceReadContinueTCPIP((DEVICE_STRUCT_TCPIP*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_UDPIP:
			retn = PlcDeviceReadContinueUDPIP((DEVICE_STRUCT_UDPIP*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_TCP_SERVER:
			retn = PlcDeviceReadContinueTcpServer((DEVICE_STRUCT_TCP_SERVER*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_TELE:
			retn = PlcDeviceReadContinueTele((DEVICE_STRUCT_TELE*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_SHAREDMEMORY:
			retn = PlcDeviceReadContinueSharedMemory((DEVICE_STRUCT_SHAREDMEMORY*)device->pData, buf, count);
			break;
	}

	return retn;
}

int PlcDeviceWriteContinue(DEVICE_STRUCT *device, char *buf, int count)
{
	int retn = 0;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceWriteContinueRS232((DEVICE_STRUCT_RS232*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_MODEM:
			retn = PlcDeviceWriteContinueModem((DEVICE_STRUCT_MODEM*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_TCPIP:
			retn = PlcDeviceWriteContinueTCPIP((DEVICE_STRUCT_TCPIP*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_UDPIP:
			retn = PlcDeviceWriteContinueUDPIP((DEVICE_STRUCT_UDPIP*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_TCP_SERVER:
			retn = PlcDeviceWriteContinueTcpServer((DEVICE_STRUCT_TCP_SERVER*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_TELE:
			retn = PlcDeviceWriteContinueTele((DEVICE_STRUCT_TELE*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_NETCLIENT:
			retn = PlcDeviceWriteContinueNetClient((DEVICE_STRUCT_NETCLIENT*)device->pData, buf, count);
			break;
		case DEVICE_TYPE_SHAREDMEMORY:
			retn = PlcDeviceWriteContinueSharedMemory((DEVICE_STRUCT_SHAREDMEMORY*)device->pData, buf, count);
			break;
	}

	return retn;
}

int PlcDeviceWrite(DEVICE_STRUCT *dev, char ch)
{
	char buf[2];

	buf[0] = ch;
	return PlcDeviceWriteContinue(dev, buf, 1);
}

int PlcDeviceClear(DEVICE_STRUCT *device)
{
	cFirstCommunicationFlag = ON;

	int retn = 0;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceClearRS232((DEVICE_STRUCT_RS232*)device->pData);
			break;
		case DEVICE_TYPE_MODEM:
			retn = PlcDeviceClearModem((DEVICE_STRUCT_MODEM*)device->pData);
			break;
		case DEVICE_TYPE_TCPIP:
			retn = PlcDeviceClearTCPIP((DEVICE_STRUCT_TCPIP*)device->pData);
			break;
		case DEVICE_TYPE_UDPIP:
			retn = PlcDeviceClearUDPIP((DEVICE_STRUCT_UDPIP*)device->pData);
			break;
		case DEVICE_TYPE_TCP_SERVER:
			retn = PlcDeviceClearTcpServer((DEVICE_STRUCT_TCP_SERVER*)device->pData);
			break;
		case DEVICE_TYPE_TELE:
			retn = PlcDeviceClearTele((DEVICE_STRUCT_TELE*)device->pData);
			break;
		case DEVICE_TYPE_SHAREDMEMORY:
			retn = PlcDeviceClearSharedMemory((DEVICE_STRUCT_SHAREDMEMORY*)device->pData);
			break;
		default:
			retn = 1;
	}

	return retn;
}

int PlcDeviceEnable(DEVICE_STRUCT *device, char flag)
{
	int retn = 0;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceEnableRS232((DEVICE_STRUCT_RS232*)device->pData, flag);
			break;
		/*
		case DEVICE_TYPE_MODEM:
			retn = PlcDeviceClearModem((DEVICE_STRUCT_MODEM*)device->pData);
			break;
		*/
		case DEVICE_TYPE_TCPIP:
			retn = PlcDeviceEnableTCPIP((DEVICE_STRUCT_TCPIP*)device->pData, flag);
			break;
		/*
		case DEVICE_TYPE_UDPIP:
			retn = PlcDeviceClearUDPIP((DEVICE_STRUCT_UDPIP*)device->pData);
			break;
		case DEVICE_TYPE_TELE:
			retn = PlcDeviceClearTele((DEVICE_STRUCT_TELE*)device->pData);
			break;
		*/
		default:
			retn = 1;
	}

	return retn;
}

int PlcDeviceSetParityRS232( DEVICE_STRUCT_RS232 *rs232, char parity);

int PlcDeviceSetParity(DEVICE_STRUCT *device, char parity)
{
	int retn;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceSetParityRS232((DEVICE_STRUCT_RS232*)device->pData, parity);
			break;
		default:
			retn = 1;
			break;
	}

	return retn;
}

int PlcDeviceSetCommStateRS232( DEVICE_STRUCT_RS232 *rs232, DWORD baud, char parity, char data, char stop);

int PlcDeviceSetCommState(DEVICE_STRUCT *device, DWORD baud, char parity, char data, char stop)
{
	int retn;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceSetCommStateRS232((DEVICE_STRUCT_RS232*)device->pData, baud, parity, data, stop);
			break;
		default:
			retn = 1;
			break;
	}

	return retn;
}

int PlcDeviceUnInit(DEVICE_STRUCT *device)
{
	int retn;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceUnInitRS232((DEVICE_STRUCT_RS232*)device->pData);
			delete (DEVICE_STRUCT_RS232*)device->pData;
			break;
		case DEVICE_TYPE_MODEM:
			retn = PlcDeviceUnInitModem((DEVICE_STRUCT_MODEM*)device->pData);
			delete (DEVICE_STRUCT_MODEM*)device->pData;
			break;
		case DEVICE_TYPE_TCPIP:
			retn = PlcDeviceUnInitTCPIP((DEVICE_STRUCT_TCPIP*)device->pData);
			delete (DEVICE_STRUCT_TCPIP*)device->pData;
			break;
		case DEVICE_TYPE_UDPIP:
			retn = PlcDeviceUnInitUDPIP((DEVICE_STRUCT_UDPIP*)device->pData);
			delete (DEVICE_STRUCT_UDPIP*)device->pData;
			break;
		case DEVICE_TYPE_TCP_SERVER:
			retn = PlcDeviceUnInitTcpServer((DEVICE_STRUCT_TCP_SERVER*)device->pData);
			delete (DEVICE_STRUCT_TCP_SERVER*)device->pData;
			break;
		case DEVICE_TYPE_TELE:
			retn = PlcDeviceUnInitTele((DEVICE_STRUCT_TELE*)device->pData);
			delete (DEVICE_STRUCT_TELE*)device->pData;
			break;
		case DEVICE_TYPE_NETCLIENT:
			retn = PlcDeviceUnInitNetClient((DEVICE_STRUCT_NETCLIENT*)device->pData);
			delete (DEVICE_STRUCT_NETCLIENT*)device->pData;
			break;
		case DEVICE_TYPE_SHAREDMEMORY:
			retn = PlcDeviceUnInitSharedMemory((DEVICE_STRUCT_SHAREDMEMORY*)device->pData);
			delete (DEVICE_STRUCT_SHAREDMEMORY*)device->pData;
			break;
	}
	
	device->nDeviceStyle = DEVICE_TYPE_NULL;

	return retn;
}

int PlcDeviceGetCommError(DEVICE_STRUCT *device, COMSTAT *comStat)
{
	int retn;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			retn = PlcDeviceGetCommErrorRS232((DEVICE_STRUCT_RS232*)device->pData, comStat);
			break;
		case DEVICE_TYPE_TCPIP:
			retn = PlcDeviceGetCommErrorTCPIP((DEVICE_STRUCT_TCPIP*)device->pData, comStat);
			break;
		case DEVICE_TYPE_UDPIP:
			retn = PlcDeviceGetCommErrorUDPIP((DEVICE_STRUCT_UDPIP*)device->pData, comStat);
			break;
		default:
			retn = 1;
			break;
	}

	return retn;
}

/*
void PlcDeviceServerSocketClose(SOCKET socket)
{
	DEVICE_STRUCT_TCPIP *tcpip;
	DEVICE_STRUCT_UDPIP  *udpip;
	DEVICE_STRUCT *device;
	int i;

	for(i = 0; i < MAX_PORT; i++) {
		device = &portBuf[i].local.device;
		if(device->nDeviceStyle == DEVICE_TYPE_TCPIP) {
			tcpip = (DEVICE_STRUCT_TCPIP*)device->pData;
			if(tcpip->socket == socket) {
				tcpip->bConnect = OFF;
				return;
			}
		}
		else if(device->nDeviceStyle == DEVICE_TYPE_UDPIP) {
			udpip = (DEVICE_STRUCT_UDPIP*)device->pData;
			if(udpip->socket == socket) {
				return;
			}
		}
		else;
	}
}
*/

//-----------------------------------------------------------------------------------------
//	모뎀 접속일때는 먼저 접속중인가를 검사해야 한다.
//  접속중이 아닐때는 접속한다.
//  모뎀 이외의 경우는 return 1 한다. (현재 접속중이라는 뜻)
//-----------------------------------------------------------------------------------------

int PlcDeviceCheckConnecting(DEVICE_STRUCT *device, TELEPHONE_STRUCT *tel)
{
	int retn;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			retn = PlcDeviceCheckConnectingModem((DEVICE_STRUCT_MODEM*)device->pData, tel);
			break;
		default:
			retn = 1;
			break;
	}

	return retn;
}

int PlcDeviceGetErrorCount(DEVICE_STRUCT *device)
{
	int count = 0;

	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;
				count = modem->connect_error_count;
				//int  connect_error_count;	// 접속 시 발생한 error count
				//char connect_error_msg[80];	// 접속 시 발생한 error message
			}
			break;
		default:
			//strcpy(string, "통신중");
			//retn = 1;
			break;
	}

	return count;
}

void PlcDeviceGetErrorString(DEVICE_STRUCT *device, char *string)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;
				strcpy(string, modem->connect_error_msg);
				//count = modem->connect_error_count;
				//int  connect_error_count;	// 접속 시 발생한 error count
				//char connect_error_msg[80];	// 접속 시 발생한 error message
			}
			break;
		default:
			if(IsLangKorean()) {
						strcpy(string, "정상");
			}
			else {

						strcpy(string, "No Error");
			}
			//retn = 1;
			break;
	}
}

void PlcDeviceGetLastCallTime(DEVICE_STRUCT *device, SYSTEMTIME *t)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;

				//memcpy(d, &modem->dLastCall, sizeof(struct date));
				memcpy(t, &modem->tLastCall, sizeof(SYSTEMTIME));
			}
			break;
		default:
			ZeroMemory(t, sizeof(SYSTEMTIME));
			break;
	}
}

void PlcDeviceResetErrorCount(DEVICE_STRUCT *device)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			PlcDeviceResetErrorCountModem((DEVICE_STRUCT_MODEM*)device->pData);
			break;
		default:
			//retn = 1;
			break;
	}
}

void PlcDeviceGetCountDown(DEVICE_STRUCT *device, SYSTEMTIME *t)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;

				memcpy(t, &modem->tCountDown, sizeof(SYSTEMTIME));
			}			
			break;
		default:
			ZeroMemory(t, sizeof(SYSTEMTIME));
			//t->wHour = 0;
			//t->ti_min = 0;
			//t->ti_sec = 0;
			//retn = 1;
			break;
	}
}

void PlcDeviceSetHandConnection(DEVICE_STRUCT *device)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;
				modem->bHandConnection = ON;
			}			
			break;
		default:
			break;
	}
}

void PlcDeviceSetHandDisConnection(DEVICE_STRUCT *device)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			PlcDeviceSetHandDisConnectModem((DEVICE_STRUCT_MODEM*)device->pData);
			break;
		default:
			break;
	}
}

void PlcDeviceSetAutoConnection(DEVICE_STRUCT *device, TELEPHONE_STRUCT *tel, char type)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_MODEM:
			{
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;
				tel->bAutoConnection = type;
				PokeBitSYSTEM(modem->port_no, POKE_BIT_MODEM_AUTO_CONNECTION, tel->bAutoConnection);
			}			
			break;
		default:
			break;
	}
}

int PlcDeviceIsModemDevice(DEVICE_STRUCT *device)
{
	if(device->nDeviceStyle == DEVICE_TYPE_MODEM)	return 1;
	
	return 0;
}



int PlcDeviceSetCommBreak(DEVICE_STRUCT *device)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			return PlcDeviceSetCommBreakRS232((DEVICE_STRUCT_RS232*)device->pData);			
		default:
			return 0;			
	}	
}

int PlcDeviceClearCommBreak(DEVICE_STRUCT *device)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			return PlcDeviceClearCommBreakRS232((DEVICE_STRUCT_RS232*)device->pData);
		default:
			return 0;			
	}	
}

// 2012-5-23  이 함수 지원
int  PlcDeviceGetInfo(DEVICE_STRUCT *device, int item, BYTE *buf, int size)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_TCPIP:
			return PlcDeviceGetInfoTCPIP((DEVICE_STRUCT_TCPIP*)device->pData, item, buf, size);
		default:
			return 0;
	}

	return 0;
}

// PlcDeviceGetInfo를 지원하면서 미리 준비함 2012-5-23
int  PlcDeviceSetInfo(DEVICE_STRUCT *device, int item, BYTE *buf, int size)
{
	return 0;
}

void PlcDeviceGetCommModemStatus(DEVICE_STRUCT *device)
{
	switch(device->nDeviceStyle) {
		case DEVICE_TYPE_RS232:
			PlcDeviceGetCommModemStatusRS232((DEVICE_STRUCT_RS232*)device->pData);
			break;
		case DEVICE_TYPE_MODEM: 
			{
				// 기본적으로 닫혀있기 때문에 상태를 읽기가 곤란함. 연결중일때만 해야 할 듯.
				/*
				DEVICE_STRUCT_MODEM *modem = (DEVICE_STRUCT_MODEM*)device->pData;
				PlcDeviceGetCommModemStatusRS232(&modem->rs232);*/
				break;
			}
	}	
}