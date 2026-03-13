// TAG size O.K
//------------------------------------------------------------------------------
//	컴퓨터 자체에 내장되어 있는 rs-232 의 통신을 담당한다..
//
//	프로그램 유의사항.
//	NT에서는 OVERLAPP을 사용하면 통신이 잘되지 않아서 OVERLAPP을 뺐다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <conio.h> 
#include <string.h>
#include <dos.h>
#include <time.h> 

#include <tools.h>
#include <dataswap.h>

#include "commmain.h"
#include "..\..\catlib.src\NetWorkProtocol.h"
 
static SHARE_PLCSCAN_NETWORK *sharePlcscanNetclient;
static SharedMemory shareClass;
static int depth = 0;

int PlcDeviceInitNetClient(HWND hwnd, DEVICE_STRUCT_NETCLIENT *net, CommaBlockString *comma)
{
	if(sharePlcscanNetclient == NULL) {		// 다른 포트의 디바이스에서도 사용할 수 있다.
		shareClass.Init("SharePlcscanNetClient", sizeof(SHARE_PLCSCAN_NETWORK));
		sharePlcscanNetclient = (SHARE_PLCSCAN_NETWORK*)shareClass.ptr;
	}

	depth++;

	return 1;
}

int PlcDeviceWriteContinueNetClient(DEVICE_STRUCT_NETCLIENT *net, char *buf, int count)
{
	//HANDLE hEvent = OpenEvent(EVENT_MODIFY_STATE, FALSE, "EventPlcScanAndNetClient");
	int i;

	/*
	if(hEvent == NULL) {
		MessageDisplay("PlcDeviceWriteContinueNetClient() - OpenEvent Is Failed\nNetClnt.exe is not running.");
		return 0;	// Net Client 프로그램이 실행중이 아니다.
	}
	*/

	TimeOutClass timeout;
	while(sharePlcscanNetclient->bEvent) {
		Sleep(1);
		if(timeout.IsTimeOut(5))	{
			MessageDisplay("PlcDeviceWriteContinueNetClient() - bEvent Is Already set by another");
			break;
		}
	}
 
	sharePlcscanNetclient->size = count;
	memcpy(sharePlcscanNetclient->buf, buf, count);
	sharePlcscanNetclient->bEvent = 1;
	//SetEvent(hEvent);
	
	// 항목의 출력을 받았는가를 검사한다.
	//TimeOutClass timeout;
	timeout.Reset();
	while(1) {
		Sleep(1);
		if(timeout.IsTimeOut(5))	{
			MessageDisplay("PlcDeviceWriteContinueNetClient() - SetEvent Is TimeOuted");
			break;
		}
		if(sharePlcscanNetclient->bEvent == 0)	break;
		//if(WaitForSingleObject(hEvent, 1) != WAIT_OBJECT_0)	break;
	}
	
	//CloseHandle(hEvent);

	for(i = 0; i < count; i++) DisplaySendCode(net->port_no, buf[i]);

	return 1;
}

int PlcDeviceUnInitNetClient(DEVICE_STRUCT_NETCLIENT *net)
{
	if(depth <= 1) {
		if(sharePlcscanNetclient) {
			shareClass.Uninit();
			sharePlcscanNetclient = NULL;
		}
	}

	depth--;

	return 1;
}

