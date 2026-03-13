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

int PlcDeviceInitSharedMemory(HWND hwnd, DEVICE_STRUCT_SHAREDMEMORY *net, CommaBlockString *comma)
{
	char name[80];
	comma->GetString(name, sizeof(name));

	net->sharedMemory = new PlcScanSharedMemory();
	net->sharedMemory->Open(name);
	
	return 1;
}

int PlcDeviceWriteContinueSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *net, char *buf, int count)
{
	if(!net->sharedMemory->IsOpen())	return 0;

	net->sharedMemory->WriteBytes((unsigned char*)buf, count);
	for(int i = 0; i < count; i++) DisplaySendCode(net->port_no, buf[i]);

	return 1;
}

int PlcDeviceClearSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *net)
{
	if(!net->sharedMemory->IsOpen())	return 0;
	net->sharedMemory->ClearRecvBuf();
	return 1;
}

int PlcDeviceReadContinueSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *net, char *buf, int count)
{
	if(!net->sharedMemory->IsOpen())	return 0;

	int retn_size = net->sharedMemory->ReadBytes((unsigned char*)buf, count);

	for(int i = 0; i < retn_size; i++) DisplayRecvCode(net->port_no, buf[i]);

	return retn_size;
}

int PlcDeviceUnInitSharedMemory(DEVICE_STRUCT_SHAREDMEMORY *net)
{
	if(net->sharedMemory) {
		net->sharedMemory->Close();
		delete net->sharedMemory;
		net->sharedMemory = NULL;
	}
	
	return 1;
}

