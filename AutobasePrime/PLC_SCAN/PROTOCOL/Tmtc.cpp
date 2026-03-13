//------------------------------------------------------------------------------
//	대청 엔지니어링 TM/TC Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//
// MELSEC에서는 많은 종류의 프로토콜이 있으나 여기서는 제어형식 1을
// 지원하므로 통신 모듈의 모드 SW를 제어 형식 1로 반드시 맞추도록 한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <compiler.hpp>
#include <totaldef.h>
#include <tools.h>
#include <crc.hpp>

#include "..\plc_scan.h" 
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleTMTC(HDC hdc, int x, int y)
{
	char *string = "id, type, address, buf address, read size (TMTC)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodTMTC(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;
	wsprintf(buf, "%3d", sm->station);
	TextOut(hdc, x, y, buf, strlen(buf));

	wsprintf(buf, "%s", sm->type);
	TextOut(hdc, x+cxChar*4, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->address);
	TextOut(hdc, x+cxChar*7, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*11, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->size);		// size word of read
	TextOut(hdc, x+cxChar*15, y, buf, strlen(buf));
}

//----------------------------------------------------------------------------
//	TMTC의 디바이스 종류를 검사한다.
//----------------------------------------------------------------------------

static int CheckTmtcDevice(char *type)
{
	if		 (strcmp(type, "AI") == 0)	return 1;
	else if(strcmp(type, "AO") == 0)	return 1;
	else if(strcmp(type, "DI") == 0)	return 1;
	else if(strcmp(type, "DO") == 0)	return 1;
	else {
		return 0;
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadTMTC(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;

	if(!CheckTmtcDevice(sm->type))	{
		char message[160];
		sprintf(message, "Scan.%d의 READ %d번에 설정된 type [%s]는 TMTC에 없는 디바이스입니다.",
								pt->no, pos, sm->type);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	if(pt->bReadingFlag == OFF) {

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = DLE;
		pt->commSendBuf[1] = STX;
		pt->commSendBuf[2] = (BYTE)sm->station;
		if(strcmp(sm->type, "DI") == 0) {
			pt->commSendBuf[3] = 0x30;	// RDI
		}
		else if(strcmp(sm->type, "DO") == 0) {
			pt->commSendBuf[3] = 0x32;	// RDO
		}
		else if(strcmp(sm->type, "AI") == 0) {
			pt->commSendBuf[3] = 0x34;	// RAD
		}
		else { 	// AO
			pt->commSendBuf[3] = 0x36;	// RDA
		}
		pt->commSendBuf[4] = (BYTE)sm->address;		//
		pt->commSendBuf[5] = (BYTE)sm->size;

		pt->commSendBuf[6] = GetCRC_SUM8(&pt->commSendBuf[2], 4);
		pt->commSendBuf[7] = DLE;
		pt->commSendBuf[8] = ETX;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 9);

		pt->commCountNeed = 9+sm->size*2;	// DLE+STX+ID+RDI+addr+length+(...)+sum+DLE+ETX
		pt->commCountSend = 9;
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		DisplaySendCodeNextLine(pt->no);

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))		return COMMUNICATION_TIME_OUT;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {

			if(pt->commRecvBuf[0] != DLE)
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != STX)
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[pt->commCountNeed-2] != DLE)
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[pt->commCountNeed-1] != ETX)
				return COMMUNICATION_CODE_BAD;

			int address = sm->target;

			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, MAKEWORD(pt->commRecvBuf[6+i*2], pt->commRecvBuf[7+i*2]));
			}

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitTMTC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  	 count;
	TimeOutClass timeout;

	if(strcmp(device, "DI") != 0 && strcmp(device, "DO") != 0)	{
		char message[160];
		sprintf(message, "DO.TAG 에 설정된 extra1 [%s]은 TMTC에 없는 디바이스입니다.",
								device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = DLE;
	pt->commSendBuf[1] = STX;
	pt->commSendBuf[2] = station;		// 자국은 FF번
	if(strcmp(device, "DI") == 0) {
		if(flag)
			pt->commSendBuf[3] = 0x40;			//	BSETDI
		else
			pt->commSendBuf[3] = 0x41;			//	BCLRDI
	}
	else {		// DO
		if(flag)
			pt->commSendBuf[3] = 0x42;			//	BSETDO
		else
			pt->commSendBuf[3] = 0x43;			//	BCLRDO
	}
	pt->commSendBuf[4] = address/16;	// address
	pt->commSendBuf[5] = address%16;	// bit

	pt->commSendBuf[6] = GetCRC_SUM8(&pt->commSendBuf[2], 4);
	pt->commSendBuf[7] = DLE;
	pt->commSendBuf[8] = ETX;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 9);

	pt->commCountNeed = 7;	// DLE+STX+ID+ACK+SUM+DLE+ETX
	pt->commCountSend = 9;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != DLE ||
				pt->commRecvBuf[1] != STX ||
				pt->commRecvBuf[2] != station ||
				pt->commRecvBuf[3] != ACK ||
				pt->commRecvBuf[5] != DLE ||
				pt->commRecvBuf[6] != ETX)
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordTMTC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	int  	 count;
	TimeOutClass timeout;

	if(strcmp(device, "AI") != 0 && strcmp(device, "AO") != 0)	{
		char message[160];
		sprintf(message, "AO.TAG 에 설정된 extra1 [%s]은 TMTC에 없는 디바이스입니다.",
								device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

   pt->commSendBuf[0] = DLE;
	pt->commSendBuf[1] = STX;
	pt->commSendBuf[2] = station;		// 자국은 FF번
	if(strcmp(device, "AI") == 0) {
		pt->commSendBuf[3] = 0x35;			//	WAD
	}
	else {		// AO
		pt->commSendBuf[3] = 0x37;			//	WDA
	}
	pt->commSendBuf[4] = (BYTE)address;	// address
	pt->commSendBuf[5] = 1;			// size

	pt->commSendBuf[6] = LOBYTE(value);	// size
	pt->commSendBuf[7] = HIBYTE(value);	// size

	pt->commSendBuf[8] =  GetCRC_SUM8(&pt->commSendBuf[2], 6);
	pt->commSendBuf[9] =  DLE;
	pt->commSendBuf[10] = ETX;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 11);

	pt->commCountNeed = 7;	// DLE+STX+ID+WDA+SUM+DLE+ETX
	pt->commCountSend = 11;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
      	if(pt->commRecvBuf[0] != DLE ||
				pt->commRecvBuf[1] != STX ||
				pt->commRecvBuf[2] != station ||
				pt->commRecvBuf[3] != ACK ||
				pt->commRecvBuf[5] != DLE ||
				pt->commRecvBuf[6] != ETX)
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





