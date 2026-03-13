//------------------------------------------------------------------------------
//	MJ71E71 Mitsubishi Protocol
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

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleMJ71E71(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (MJ71E71)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodMJ71E71(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
//	MelSec PLC의 디바이스 종류를 검사한다.
//----------------------------------------------------------------------------

static int CheckMelsecDevice(char *type)
{
	if		 (strcmp(type, "X") == 0)	return 1;
	else if(strcmp(type, "Y") == 0)	return 1;
	else if(strcmp(type, "M") == 0)	return 1;
	else if(strcmp(type, "L") == 0)	return 1;
	else if(strcmp(type, "S") == 0)	return 1;
	else if(strcmp(type, "B") == 0)	return 1;
	else if(strcmp(type, "F") == 0)	return 1;
	else if(strcmp(type, "M") == 0)	return 1;
	else if(strcmp(type, "TS") == 0)	return 1;
	else if(strcmp(type, "TC") == 0)	return 1;
	else if(strcmp(type, "CS") == 0)	return 1;
	else if(strcmp(type, "CC") == 0)	return 1;
	else if(strcmp(type, "TN") == 0)	return 1;
	else if(strcmp(type, "CN") == 0)	return 1;
	else if(strcmp(type, "D") == 0)	return 1;
	else if(strcmp(type, "W") == 0)	return 1;
	else if(strcmp(type, "R") == 0)	return 1;
	else {
		return 0;
	}
}

//------------------------------------------------------------------------------
//	MelSel Address구조는 16진수와 10진수를 따로 사용한다.
// X, Y, B, W 는 16진수 어드레스를 사용하고,
// 그외는 10진수를 사용한다.
// CS, CC 등 두글자를 가진 어드레스는 10진수 어드레스를 사용한다.
//------------------------------------------------------------------------------

static int IsAddressHex(char *type)
{
	if(type[0] == 'X')	return 1;
	if(type[0] == 'Y')	return 1;
	if(type[0] == 'B')	return 1;
	if(type[0] == 'W')	return 1;

	return 0;
}

static void DisplayErrorCode(BYTE code)
{
	char message[160];

	sprintf(message, "MelSec Plc 통신오류 코드 %02Xh", code);
	MessageDisplay(message);
}

static void DisplayEndCode(BYTE code)
{
	char message[160];

	sprintf(message, "MelSec Plc 통신종료 코드 %02Xh", code);
	MessageDisplay(message);
}


//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadMJ71E71(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
	char imsi[10];

	if(!CheckMelsecDevice(sm->type))	{
		char message[160];
		sprintf(message, "Scan.%d의 READ %d번에 설정된 type [%s]는 MelSec Plc에 없는 디바이스입니다.",
								pt->no, pos, sm->type);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	if(pt->bReadingFlag == OFF) {

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = 0x01;
		pt->commSendBuf[1] = (BYTE)sm->station;		// 자국은 FF번
		pt->commSendBuf[2] = LOBYTE(10);		// 감시 타이머
		pt->commSendBuf[3] = HIBYTE(10);		// 감시 타이머
		if(strlen(sm->type) == 1) {
			imsi[0] = sm->type[0];		// 감시 타이머
			imsi[1] = 0x20;		// 감시 타이머
		}
		else {
			imsi[0] = sm->type[0];  	// device TS, TC ...
			imsi[1] = sm->type[1];
		}

		if(IsAddressHex(sm->type))
			sprintf(&imsi[2], "%04X", sm->address);							// wait count
		else
			sprintf(&imsi[2], "%04d", sm->address);							// wait count

		pt->commSendBuf[4] = (WORD)sm->address%256;
		pt->commSendBuf[5] = (WORD)sm->address/256;
		pt->commSendBuf[6] = 0;
		pt->commSendBuf[7] = 0;
		pt->commSendBuf[8] = imsi[1];
		pt->commSendBuf[9] = imsi[0];

		pt->commSendBuf[10] = (BYTE)sm->size;
		pt->commSendBuf[11] = 0x00;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 12);

		pt->commCountNeed = 2+sm->size*2;	// STX+STATION(2)+PCnum(2)+data+ETX+CRC(2)
		pt->commCountSend = 12;
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

		if(pt->commCountCurr >= 2) {
			if(pt->commRecvBuf[1] != 0 && pt->commRecvBuf[1] != 0x5B) {
				DisplayEndCode(pt->commRecvBuf[1]);
				return COMMUNICATION_OK;
			}
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {

			if(pt->commRecvBuf[0] != 0x81)
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[1] == 0x5B) {
				DisplayErrorCode(pt->commRecvBuf[2]);
				return COMMUNICATION_OK;
			}

			int address = sm->target;

			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, MAKEWORD(pt->commRecvBuf[2+i*2], pt->commRecvBuf[3+i*2]));
			}

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitMJ71E71(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  	 count;
	TimeOutClass timeout;
	char imsi[10];

	if(!CheckMelsecDevice(device))	{
		char message[160];
		sprintf(message, "DO.TAG 에 설정된 extra1 [%s]은 MelSec Plc에 없는 디바이스입니다.",
								device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 0x02;
	pt->commSendBuf[1] = station;		// 자국은 FF번
	pt->commSendBuf[2] = LOBYTE(10);	// 감시 타이머
	pt->commSendBuf[3] = HIBYTE(10);	// 감시 타이머
	if(strlen(device) == 1) {
		imsi[0] = device[0];		// 감시 타이머
		imsi[1] = 0x20;			// 감시 타이머
	}
	else {
		imsi[0] = device[0];  	// device TS, TC ...
		imsi[1] = device[1];
	}

	if(IsAddressHex(device))
		sprintf(&imsi[2], "%04X", address);							// wait count
	else
		sprintf(&imsi[2], "%04d", address);							// wait count

	pt->commSendBuf[4] = address%256;
	pt->commSendBuf[5] = address/256;
	pt->commSendBuf[6] = 0;
	pt->commSendBuf[7] = 0;
	pt->commSendBuf[8] = imsi[1];
	pt->commSendBuf[9] = imsi[0];

	pt->commSendBuf[10] = 1;
	pt->commSendBuf[11] = 0x00;
	pt->commSendBuf[12] = (BYTE)flag << 4;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 13);

	pt->commCountNeed = 2;	// 82+end code
	pt->commCountSend = 13;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != 0x82)
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != 0x00) {
				DisplayEndCode(pt->commRecvBuf[1]);
			}

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordMJ71E71(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	int  	 count;
	TimeOutClass timeout;
	char imsi[10];

	if(!CheckMelsecDevice(device))	{
		char message[160];
		sprintf(message, "AO.TAG 에 설정된 extra1 [%s]은 MelSec Plc에 없는 디바이스입니다.",
								device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 0x03;
	pt->commSendBuf[1] = station;		// 자국은 FF번
	pt->commSendBuf[2] = LOBYTE(10);	// 감시 타이머
	pt->commSendBuf[3] = HIBYTE(10);	// 감시 타이머
	if(strlen(device) == 1) {
		imsi[0] = device[0];		// 감시 타이머
		imsi[1] = 0x20;			// 감시 타이머
	}
	else {
		imsi[0] = device[0];  	// device TS, TC ...
		imsi[1] = device[1];
	}

	if(IsAddressHex(device)) {
		pt->commSendBuf[4] = (address*16)%256;
		pt->commSendBuf[5] = (address*16)/256;
		pt->commSendBuf[6] = 0;
		pt->commSendBuf[7] = 0;
	}
	else {
		pt->commSendBuf[4] = address%256;
		pt->commSendBuf[5] = address/256;
		pt->commSendBuf[6] = 0;
		pt->commSendBuf[7] = 0;
	}
	pt->commSendBuf[8] = imsi[1];
	pt->commSendBuf[9] = imsi[0];

	pt->commSendBuf[10] = 1;
	pt->commSendBuf[11] = 0x00;
	pt->commSendBuf[12] = LOBYTE(value);
	pt->commSendBuf[13] = HIBYTE(value);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 14);

	pt->commCountNeed = 2;	// 82+end code
	pt->commCountSend = 14;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != 0x83)
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != 0x00) {
				DisplayEndCode(pt->commRecvBuf[1]);
			}

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





