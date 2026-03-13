//------------------------------------------------------------------------------
//	OMRON Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <tools.h>
#include <totaldef.h>
#include <glib.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleOMRON(HDC hdc, int x, int y)
{
	char *string = "unit NO, type, address, buf address, read size (OMRON)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodOMRON(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

static BYTE GetCRC(BYTE *buf, int size)
{
	BYTE crc;
   int i;

	crc = buf[0];

	for(i = 1; i < size; i++) {
		crc = crc ^ buf[i];
	}

	return crc;
}

static int CheckDeviceOmron(char *type)
{
	if(strcmp(type, "IR") == 0)	return 1;
	if(strcmp(type, "HR") == 0)	return 1;
	if(strcmp(type, "AR") == 0)	return 1;
	if(strcmp(type, "LR") == 0)	return 1;
	if(strcmp(type, "TC") == 0)	return 1;
	if(strcmp(type, "DM") == 0)	return 1;
	if(strcmp(type, "FMI") == 0)	return 1;
	if(strcmp(type, "FMD") == 0)	return 1;
	if(strcmp(type, "PV") == 0)	return 1;
   if(strcmp(type, "STAT") == 0)	return 1;

	PlcScanSetErrorString("TYPE[%s] 는 OMRON PLC Plc에 없는 디바이스 영역", type);

	return 0;
}

static int TypeToReadCommand(char *retn, char *type)
{
	if(strcmp(type, "IR") == 0) {
		retn[0] = 'R';	retn[1] = 'R';
	}
	else if(strcmp(type, "HR") == 0) {
		retn[0] = 'R';	retn[1] = 'H';
	}
	else if(strcmp(type, "AR") == 0) {
		retn[0] = 'R';	retn[1] = 'J';
	}
	else if(strcmp(type, "LR") == 0) {
		retn[0] = 'R';	retn[1] = 'L';
	}
	else if(strcmp(type, "DM") == 0) {
		retn[0] = 'R';	retn[1] = 'D';
	}
	else if(strcmp(type, "PV") == 0) {
		retn[0] = 'R';	retn[1] = 'C';
	}
	else {
		PlcScanSetErrorString("Omron Protocol 미완성 type[%s]", type);
		return 0;
	}

	return 1;
}

static int TypeToWriteCommand(char *retn, char *type)
{
	if(strcmp(type, "IR") == 0) {
		retn[0] = 'W';	retn[1] = 'R';
	}
	else if(strcmp(type, "HR") == 0) {
		retn[0] = 'W';	retn[1] = 'H';
	}
	else if(strcmp(type, "AR") == 0) {
		retn[0] = 'W';	retn[1] = 'J';
	}
	else if(strcmp(type, "LR") == 0) {
		retn[0] = 'W';	retn[1] = 'L';
	}
	else if(strcmp(type, "DM") == 0) {
		retn[0] = 'W';	retn[1] = 'D';
	}
	else if(strcmp(type, "PV") == 0) {
		retn[0] = 'W';	retn[1] = 'C';
	}
	else if(strcmp(type, "STAT") == 0) {
		retn[0] = 'S';	retn[1] = 'C';
	}
	else {
		PlcScanSetErrorString("Omron Protocol 미완성 type[%s]", type);
		return 0;
	}

	return 1;
}

static void SetErrorCode(BYTE code)
{
	switch(code) {
		case 0x01:
			PlcScanSetErrorString("Omron Plc Returned Error Code-%02X (Run중에는 사용 불가)", code);
			break;
		case 0x13:
			PlcScanSetErrorString("Omron Plc Returned Error Code-%02X (checksum error)", code);
			break;
		default:
			PlcScanSetErrorString("Omron Plc Returned Error Code-%02X", code);
			break;
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadOMRON(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
//	TimeOutClass timeout;
	char command[10];

	if(!CheckDeviceOmron(sm->type))	return COMMUNICATION_ERR_STRING;
	if(!TypeToReadCommand(command, sm->type))	return COMMUNICATION_ERR_STRING;

	if(pt->bReadingFlag == OFF) {

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = '@';
		wsprintf((char*)&pt->commSendBuf[1], "%02d", sm->station);	// read WORD command
		pt->commSendBuf[3] = command[0];
		pt->commSendBuf[4] = command[1];
		wsprintf((char*)&pt->commSendBuf[5], "%04d", sm->address);	// read WORD command
		wsprintf((char*)&pt->commSendBuf[9], "%04d", sm->size);			// read WORD command
		wsprintf((char*)&pt->commSendBuf[13], "%02X", GetCRC(pt->commSendBuf, 13));
		pt->commSendBuf[15] = '*';
		pt->commSendBuf[16] = CR;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 17);

		pt->commCountNeed = 11+sm->size*4;	// @+unit(2)+R+R+responsecode(2)+...+FCS(2)+*+CR
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

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

		// 에러 코드가 들어왔는지를 검사한다.
		if(pt->commCountCurr >= 11 && pt->commRecvBuf[10] == CR) {
			if(pt->commRecvBuf[5] != '0' || pt->commRecvBuf[6] != '0') {
				SetErrorCode(HexBufToBYTE((char*)&pt->commRecvBuf[5]));
				return COMMUNICATION_ERR_STRING;
			}
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[pt->commCountNeed-1] != CR)	return COMMUNICATION_CODE_BAD;

			int address = sm->target;

			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, HexBufToWORD((char*)&pt->commRecvBuf[7+i*4]));
			}

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	지정된 영역에서 한 워드만 읽어온다.
//------------------------------------------------------------------------------

static int ReadWordOMRON(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD &value, char *type)
{
	int  	 count;
	TimeOutClass timeout;
	char command[10];
   int    read_size = 1;

	if(!CheckDeviceOmron(type))	return COMMUNICATION_ERR_STRING;
	if(!TypeToReadCommand(command, type))	return COMMUNICATION_ERR_STRING;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = '@';
	wsprintf((char*)&pt->commSendBuf[1], "%02d", station);	// read WORD command
	pt->commSendBuf[3] = command[0];
	pt->commSendBuf[4] = command[1];
	wsprintf((char*)&pt->commSendBuf[5], "%04d", address);	// read WORD command
	wsprintf((char*)&pt->commSendBuf[9], "%04d", read_size);			// read WORD command
	wsprintf((char*)&pt->commSendBuf[13], "%02X", GetCRC(pt->commSendBuf, 13));
	pt->commSendBuf[15] = '*';
	pt->commSendBuf[16] = CR;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 17);

	pt->commCountNeed = 11+read_size*4;	// @+unit(2)+R+R+responsecode(2)+...+FCS(2)+*+CR
	pt->commCountSend = 17;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 에러 코드가 들어왔는지를 검사한다.
		if(pt->commCountCurr >= 11 && pt->commRecvBuf[10] == CR) {
			if(pt->commRecvBuf[5] != '0' || pt->commRecvBuf[6] != '0') {
				SetErrorCode(HexBufToBYTE((char*)&pt->commRecvBuf[5]));
				return COMMUNICATION_ERR_STRING;
			}
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[pt->commCountNeed-1] != CR)	return COMMUNICATION_CODE_BAD;
			value = HexBufToWORD((char*)&pt->commRecvBuf[7]);
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordOMRON(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *type)
{
	int  	 	count;
	TimeOutClass	timeout;
	char command[10];

	if(!CheckDeviceOmron(type))						return COMMUNICATION_ERR_STRING;
	if(!TypeToWriteCommand(command, type))	return COMMUNICATION_ERR_STRING;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = '@';
	wsprintf((char*)&pt->commSendBuf[1], "%02d", station);	// read WORD command
	pt->commSendBuf[3] = command[0];
	pt->commSendBuf[4] = command[1];
	if(strcmp(type, "STAT") == 0) {
		wsprintf((char*)&pt->commSendBuf[5], "%02X", value);	// read WORD command
		wsprintf((char*)&pt->commSendBuf[7], "%02X", GetCRC(pt->commSendBuf, 7));
		pt->commSendBuf[9] = '*';
		pt->commSendBuf[10] = CR;
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 11);
      pt->commCountSend = 11;
	}
	else {
		wsprintf((char*)&pt->commSendBuf[5], "%04d", address);	// read WORD command
		wsprintf((char*)&pt->commSendBuf[9], "%04X", value);			// read WORD command
		wsprintf((char*)&pt->commSendBuf[13], "%02X", GetCRC(pt->commSendBuf, 13));
		pt->commSendBuf[15] = '*';
		pt->commSendBuf[16] = CR;
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 17);
		pt->commCountSend = 17;
	}

	pt->commCountNeed = 11;	// @+unit(2)+R+R+responsecode(2)+FCS(2)+*+CR
//	pt->commCountSend = 17;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 에러 코드가 들어왔는지를 검사한다.
		if(pt->commCountCurr >= 11) {
			if(pt->commRecvBuf[pt->commCountNeed-1] != CR)	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[5] != '0' || pt->commRecvBuf[6] != '0') {	// response code
				SetErrorCode(HexBufToBYTE((char*)&pt->commRecvBuf[5]));
				return COMMUNICATION_ERR_STRING;
			}

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitOMRON(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	WORD   value;
	int retn;

	retn = ReadWordOMRON(pt, station, address/16, value, device);
	if(retn != COMMUNICATION_OK)	return retn;
	if(flag) {
		value |= WORD_MASK[address%16];
	}
	else {
		value &= 0xFFFF-WORD_MASK[address%16];
	}
	return PlcScanWriteWordOMRON(pt, station, address/16, value, device);
}



