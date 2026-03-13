//------------------------------------------------------------------------------
//	MASTER 500-1000K Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
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
#include "pro_main.h"

void PlcScanDrawMethodTitleMasterK(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (Master-K500/1000)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodMasterK(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
//	Master500K PLC의 기본 디바이스 인가를 검사한다.
//----------------------------------------------------------------------------

static int CheckMasterKDevice(char *type)
{
	if		 (strcmp(type, "P") == 0)	return 1;
	else if(strcmp(type, "M") == 0)	return 1;
	else if(strcmp(type, "L") == 0)	return 1;
	else if(strcmp(type, "K") == 0)	return 1;
	else if(strcmp(type, "F") == 0)	return 1;
	else if(strcmp(type, "T") == 0)	return 1;	// WORD 만 가능.
	else if(strcmp(type, "C") == 0)	return 1;	// WORD 만 가능.
	else if(strcmp(type, "D") == 0)	return 1;
	else if(strcmp(type, "S") == 0)	return 1;
	else              					return 0;
}

//------------------------------------------------------------
//	Master500K 에서 발생하는 에러를 스트링으로 만든다.
//------------------------------------------------------------

static void SetErrorCode(BYTE err)
{
	char string[100];

	sprintf(string, "ErrorCode:(%02Xh)", err);

	PlcScanSetErrorString(string);
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadMasterK(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
	int  buf_pos = 0;
	BYTE crc;

	if(!CheckMasterKDevice(sm->type)) {
		PlcScanSetErrorString("Master-K 에 없는 Device영역이 설정됨(%s).", sm->type);
		return COMMUNICATION_ERR_STRING;
	}

	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = ENQ;
		buf_pos = 1;
		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", sm->station);	//	station
		buf_pos += 2;

		wsprintf((char*)&pt->commSendBuf[buf_pos], "r");	// r- read Command With BCC
		buf_pos += 1;										// SB - System label I, Q, M

		// M0001 방식의 Address설정
		wsprintf((char*)&pt->commSendBuf[buf_pos], "%c%04d", sm->type[0], sm->address);
		buf_pos += 5;

		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", sm->size);
		buf_pos += 2;

		pt->commSendBuf[buf_pos] = EOT;
		buf_pos += 1;

		crc = GetCRC_SUM8(&pt->commSendBuf[1], buf_pos-1);

		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
		buf_pos += 2;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

		DisplaySendCodeNextLine(pt->no);

		pt->commCountNeed = 7+sm->size*4;	// ACK+st(2)+r+..+EOT+BCC(2)
		pt->commCountSend = buf_pos;
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))		return COMMUNICATION_TIME_OUT;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

		if(pt->commCountCurr >= 8 && pt->commRecvBuf[0] == NAK) {
			SetErrorCode(HexBufToBYTE((char*)&pt->commRecvBuf[4]));
			return COMMUNICATION_ERR_STRING;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[1], sm->size*4+4) != HexBufToBYTE((char*)&pt->commRecvBuf[sm->size*4+5])) {
				return COMMUNICATION_CODE_BAD;
			}

			int address = sm->target;

			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, HexBufToWORD((char*)&pt->commRecvBuf[4+i*4]));
			}

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitMasterK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  	 count;
	TimeOutClass timeout;
	int    buf_pos;
	BYTE	 crc;

	if(!CheckMasterKDevice(device)) {
		PlcScanSetErrorString("WriteBit Error: Master-K 에 없는 Device영역이 설정됨(%s).", device);
		return COMMUNICATION_ERR_STRING;
	}

	switch(device[0]) {
		case 'P':
		case 'M':
		case 'L':
		case 'K':	break;
		default:
			PlcScanSetErrorString("Master-K (%s)디바이스는 비트 쓰기가 불가능.", device);
			return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	buf_pos = 1;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", station);	//	station
	buf_pos += 2;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "h");					// h- write bit with BCC
	buf_pos += 1;

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%c%03d%01X", device[0], address/16, address%16);	// PC 번호는 항상 0xFF일 것
	buf_pos += 5;

	sprintf((char*)&pt->commSendBuf[buf_pos], "01");	// size
	buf_pos += 2;

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%01X", flag);
	buf_pos += 1;

	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(&pt->commSendBuf[1], buf_pos-1);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

	pt->commCountNeed = 7;    // ACK+st(2)+h+EOT+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] == NAK) {	// NAK 가 나올때는 데이터가 더 길기 때문에 부득이 여기서 검사한다.
				SetErrorCode(HexBufToBYTE((char*)&pt->commRecvBuf[3]));
				return COMMUNICATION_ERR_STRING;
			}
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[1], 4) != HexBufToBYTE((char*)&pt->commRecvBuf[5])) {
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordMasterK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	int  	 count;
	TimeOutClass timeout;
	int	 buf_pos = 0;
	BYTE	 crc;

	if(!CheckMasterKDevice(device)) {
		PlcScanSetErrorString("WriteBit Error: Master-K 에 없는 Device영역이 설정됨(%s).", device);
		return COMMUNICATION_ERR_STRING;
	}

	if(device[0] == 'F') {
		PlcScanSetErrorString("Master-K (%s)디바이스는 워드 쓰기가 불가능.", device);
		return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	buf_pos = 1;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", station);	//	station
	buf_pos += 2;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "w");	// r- read Command With BCC
	buf_pos += 1;														// SB - System label I, Q, M
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%c%04d", device[0], address);
	buf_pos += 5;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "01");	// word 개수
	buf_pos += 2;

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%04X", value);
	buf_pos += 4;

	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(&pt->commSendBuf[1], buf_pos-1);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

	pt->commCountNeed = 7;	// ACK+st(2)+h+EOT+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] == NAK) {	// NAK 가 나올때는 데이터가 더 길기 때문에 부득이 여기서 검사한다.
				SetErrorCode(HexBufToBYTE((char*)&pt->commRecvBuf[3]));
				return COMMUNICATION_ERR_STRING;
			}
         if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[1], 4) != HexBufToBYTE((char*)&pt->commRecvBuf[5])) {
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





