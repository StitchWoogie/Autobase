// english O.K
//------------------------------------------------------------------------------
//	AOJ2C214 Mitsubishi Protocol
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

#include <totaldef.h>
#include <tools.h>
#include <crc.hpp>

#include "..\plc_scan.h"
#include "pro_lib.h"
//#include "alarm.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleAOJ2C214(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (Aoj2C214)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodAOJ2C214(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadAOJ2C214(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;

	if(!CheckMelsecDevice(sm->type))	{
		char message[160];
		if(IsLangKorean()) {
			sprintf(message, "Scan.%d의 READ %d번에 설정된 type [%s]는 MelSec Plc에 없는 디바이스입니다.",
								pt->no, pos, sm->type);
		}
		else {
			sprintf(message, "type not exist Scan.%d, Read:%d, type:[%s]",	pt->no, pos, sm->type);
		}
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	if(pt->bReadingFlag == OFF) {

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = ENQ;
		wsprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);	//
		wsprintf((char*)&pt->commSendBuf[3], "FF");				// PC 번호는 항상 0xFF일 것
		wsprintf((char*)&pt->commSendBuf[5], "WR");							// Word Read
		wsprintf((char*)&pt->commSendBuf[7], "0");							// wait count
		if(strlen(sm->type) == 1) {
			pt->commSendBuf[8] = sm->type[0];		// device X, Y etc
			if(IsAddressHex(sm->type))
				wsprintf((char*)&pt->commSendBuf[9], "%04X", sm->address);							// wait count
			else
				wsprintf((char*)&pt->commSendBuf[9], "%04d", sm->address);							// wait count
		}
		else {
			pt->commSendBuf[8] = sm->type[0];  	// device TS, TC ...
			pt->commSendBuf[9] = sm->type[1];
			wsprintf((char*)&pt->commSendBuf[10], "%03X", sm->address);							// wait count
		}
		wsprintf((char*)&pt->commSendBuf[13], "%02X", sm->size);
		wsprintf((char*)&pt->commSendBuf[15], "%02X", GetCRC_SUM8(&pt->commSendBuf[1], 14));

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 17);

		pt->commCountNeed = 6+sm->size*4;	// STX+STATION(2)+PCnum(2)+data+ETX+CRC(2)
		pt->commCountSend = 17;
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		DisplaySendCodeNextLine(pt->no);

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))		return COMMUNICATION_TIME_OUT;

		//need = pt->commCountNeed-pt->commCountCurr;
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {

			if(pt->commRecvBuf[0] != STX)
				return COMMUNICATION_CODE_BAD;
			if(strncmp((char*)&pt->commSendBuf[1], (char*)&pt->commRecvBuf[1], 4) != 0)
				return COMMUNICATION_CODE_BAD;

			int address = sm->target;

			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, HexBufToWORD((char*)&pt->commRecvBuf[5+i*4]));
			}

			pt->commSendBuf[0] = ACK;
			wsprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);	//
			wsprintf((char*)&pt->commSendBuf[3], "FF");						// PC 번호는 항상 0xFF일 것

			PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 5);

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitAOJ2C214(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  	 count;
	TimeOutClass timeout;

	if(!CheckMelsecDevice(device))	{
		char message[160];
		sprintf(message, "extra1 [%s] is not mesec device.", device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	wsprintf((char*)&pt->commSendBuf[1], "%02X", station);	//
	wsprintf((char*)&pt->commSendBuf[3], "FF");				// PC 번호는 항상 0xFF일 것
	wsprintf((char*)&pt->commSendBuf[5], "BW");				// BIT write
	wsprintf((char*)&pt->commSendBuf[7], "0");					// wait count
	if(strlen(device) == 1) {
		pt->commSendBuf[8] = device[0];							// device X, Y etc
		if(IsAddressHex(device))
			wsprintf((char*)&pt->commSendBuf[9], "%04X", address);							// wait count
		else
			wsprintf((char*)&pt->commSendBuf[9], "%04d", address);							// wait count
	}
	else {
		pt->commSendBuf[8] = device[0];  	// device TS, TC ...
		pt->commSendBuf[9] = device[1];
		wsprintf((char*)&pt->commSendBuf[10], "%03X", address);							// wait count
	}
	wsprintf((char*)&pt->commSendBuf[13], "01");					// bit count
	wsprintf((char*)&pt->commSendBuf[15], "%1d", flag);			// ON/OFF
	wsprintf((char*)&pt->commSendBuf[16], "%02X", GetCRC_SUM8(&pt->commSendBuf[1], 15));

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 18);

	pt->commCountNeed = 5;	// STX+STATION(2)+PCnum(2)
	pt->commCountSend = 18;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK)
				return COMMUNICATION_CODE_BAD;
			if(strncmp((char*)&pt->commSendBuf[1], (char*)&pt->commRecvBuf[1], 4) != 0)
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordAOJ2C214(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	int  	 count;
//	int  	 need;
	TimeOutClass timeout;

	if(!CheckMelsecDevice(device))	{
		char message[160];
		if(IsLangKorean()) {
				sprintf(message, "AO.TAG 에 설정된 extra1 [%s]은 MelSec Plc에 없는 디바이스입니다.",
										device);
		}else {
				sprintf(message, "extra1 [%s] is not MelSec Plc file.",
										device);
		}
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	wsprintf((char*)&pt->commSendBuf[1], "%02X", station);	//
	wsprintf((char*)&pt->commSendBuf[3], "FF");				// PC 번호는 항상 0xFF일 것
	wsprintf((char*)&pt->commSendBuf[5], "WW");							// Word write
	wsprintf((char*)&pt->commSendBuf[7], "0");							// wait count
	if(strlen(device) == 1) {
		pt->commSendBuf[8] = device[0];		// device X, Y etc
		if(IsAddressHex(device))
			wsprintf((char*)&pt->commSendBuf[9], "%04X", address);							// wait count
		else
			wsprintf((char*)&pt->commSendBuf[9], "%04d", address);							// wait count
	}
	else {
		pt->commSendBuf[8] = device[0];  	// device TS, TC ...
		pt->commSendBuf[9] = device[1];
		wsprintf((char*)&pt->commSendBuf[10], "%03X", address);							// wait count
	}
	wsprintf((char*)&pt->commSendBuf[13], "01");					// word size
	wsprintf((char*)&pt->commSendBuf[15], "%04X", value);		// word value
	wsprintf((char*)&pt->commSendBuf[19], "%02X", GetCRC_SUM8(&pt->commSendBuf[1], 18));

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 21);

	pt->commCountNeed = 5;	// STX+STATION(2)+PCnum(2)
	pt->commCountSend = 21;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
//		need = pt->commCountNeed-pt->commCountCurr;
		if(pt->commCountNeed >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK)
				return COMMUNICATION_CODE_BAD;
			if(strncmp((char*)&pt->commSendBuf[1], (char*)&pt->commRecvBuf[1], 4) != 0)
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





