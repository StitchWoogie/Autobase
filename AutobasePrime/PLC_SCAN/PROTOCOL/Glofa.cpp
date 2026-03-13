// english O.K
//------------------------------------------------------------------------------
//	Glofa Mitsubishi Protocol
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


void PlcScanDrawMethodTitleGlofa(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (Glofa)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodGlofa(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

	wsprintf(buf, "%3d", sm->extra2);		// size word of read
	TextOut(hdc, x+cxChar*18, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->extra3);		// size word of read
	TextOut(hdc, x+cxChar*21, y, buf, strlen(buf));
}

//----------------------------------------------------------------------------
//	Glofa PLC의 기본 디바이스 인가를 검사한다.
//----------------------------------------------------------------------------

static int CheckGlofaBaseDevice(char *type)
{
	if		 (strcmp(type, "M") == 0)	return 1;
	else if(strcmp(type, "I") == 0)	return 1;
	else if(strcmp(type, "Q") == 0)	return 1;
	else              					return 0;
}

//------------------------------------------------------------
//	Glofa 에서 발생하는 에러를 스트링으로 만든다.
//------------------------------------------------------------

static void SetErrorCode(WORD err)
{
	char string[100];

	sprintf(string, "ErrorCode:(%04Xh)", err);

   PlcScanSetErrorString(string);
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadGlofa(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
	char   flag_base_device = CheckGlofaBaseDevice(sm->type);
	int  buf_pos = 0;
	char buf[20];
	BYTE crc;
	//int  read_size;

	if(pt->bReadingFlag == OFF) {

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = ENQ;
		buf_pos = 1;
		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", sm->station);	//	station
		buf_pos += 2;

		if(flag_base_device) {
			wsprintf((char*)&pt->commSendBuf[buf_pos], "rSB");	// r- read Command With BCC
			buf_pos += 3;														// SB - System label I, Q, M

			if(sm->type[0] == 'M') {
				sprintf(buf, "%%%sW%02d", sm->type, sm->address);
				wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
				buf_pos += 2;
				strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
				buf_pos += strlen(buf);
			}
			else {
				sprintf(buf, "%%%sW%d.%d.%d", sm->type, sm->extra2, sm->extra3, sm->address);
				wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
				buf_pos += 2;
				strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
				buf_pos += strlen(buf);
			}

			//read_size = sm->size;

			wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", sm->size);
			buf_pos += 2;
		}
		/*
		else {	// name label	read
			wsprintf((char*)&pt->commSendBuf[buf_pos], "r%02X", sm->extra2); // r- read Command With BCC
			buf_pos += 3;														       // SB - System label I, Q, M

			wsprintf((char*)&pt->commSendBuf[buf_pos], "01");	// block count
			buf_pos += 2;

			sprintf(buf, "%s", sm->type, sm->address);
			wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
			buf_pos += 2;
			strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
			buf_pos += strlen(buf);

			if(sm->extra2 >= 0x15) {	// array 변수
				read_size = sm->size;
				sprintf((char*)&pt->commSendBuf[buf_pos], "%02X", read_size);	// Word Read
				buf_pos += 2;
			}
			else {
				read_size = 1;
			}
		}
		*/

		pt->commSendBuf[buf_pos] = EOT;
		buf_pos += 1;

		crc = GetCRC_SUM8(pt->commSendBuf, buf_pos);

		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
		buf_pos += 2;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

		pt->commCountNeed = 13+sm->size*4;	// ACK+st(2)+r+SB+block_size(2)+size(2)+..+ETX+BCC(2)
		pt->commCountSend = buf_pos;
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

		if(pt->commCountCurr >= 13 && pt->commRecvBuf[0] == NAK) {
			SetErrorCode(HexBufToWORD((char*)&pt->commRecvBuf[6]));
			return COMMUNICATION_ERR_STRING;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}

			crc = GetCRC_SUM8(pt->commRecvBuf, pt->commCountNeed-2);
			if(crc != HexBufToBYTE((char*)&pt->commRecvBuf[pt->commCountNeed-2])) {
				return COMMUNICATION_CODE_BAD;
			}

			int address = sm->target;

			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, HexBufToWORD((char*)&pt->commRecvBuf[10+i*4]));
			}
			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitGlofa(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device, WORD pannel)
{
	int  	 count;
	TimeOutClass timeout;
	char   flag_base_device = CheckGlofaBaseDevice(device);
	int    buf_pos;
	char	 buf[20];
	BYTE	 crc;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	buf_pos = 1;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", station);	//	station
	buf_pos += 2;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "wSS");	// r- read Command With BCC
	buf_pos += 3;														// SB - System label I, Q, M
	wsprintf((char*)&pt->commSendBuf[buf_pos], "01");	// block count
	buf_pos += 2;
	if(flag_base_device) {
		if(device[0] == 'M') {
			sprintf(buf, "%%%sX%03d%01X", device, address/16, address%16);
			wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
			buf_pos += 2;
			strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
			buf_pos += strlen(buf);
		}
		else {
			char imsi[10];
			sprintf(imsi, "%03d%01X", address/16, address%16);
			sprintf(buf, "%%%sX%d.%c%c.%c%c", device, pannel, imsi[0], imsi[1], imsi[2], imsi[3]);
			wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
			buf_pos += 2;
			strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
			buf_pos += strlen(buf);
		}
	}
	else {	// name label	read
		sprintf(buf, "%s", device);
		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
		buf_pos += 2;
		strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
		buf_pos += strlen(buf);
	}
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", flag);
	buf_pos += 2;
	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(pt->commSendBuf, buf_pos);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

	pt->commCountNeed = 13;    // NAK일때는 13개 ACK일때는 9개
									// ACK+STATION(2)+w+SS+ETC+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		if(pt->commRecvBuf[0] == ACK && pt->commCountCurr >= 9) {	// ACK 가 나올때는 9글자가 온다.
			return COMMUNICATION_OK;
		}
		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] == NAK) {	// NAK 가 나올때는 데이터가 더 길기 때문에 부득이 여기서 검사한다.
				SetErrorCode(HexBufToWORD((char*)&pt->commRecvBuf[6]));
				return COMMUNICATION_ERR_STRING;
			}
			return COMMUNICATION_CODE_BAD;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordGlofa(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device, WORD pannel)
{
	int  	 count;
	TimeOutClass timeout;
	char   flag_base_device = CheckGlofaBaseDevice(device);
	int	 buf_pos = 0;
	char	 buf[20];
	BYTE	 crc;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	buf_pos = 1;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", station);	//	station
	buf_pos += 2;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "wSS");	// r- read Command With BCC
	buf_pos += 3;														// SB - System label I, Q, M
	wsprintf((char*)&pt->commSendBuf[buf_pos], "01");	// block count
	buf_pos += 2;
	if(flag_base_device) {
		if(device[0] == 'M') {
			sprintf(buf, "%%%sW%d", device, address);
			wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
			buf_pos += 2;
			strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
			buf_pos += strlen(buf);
		}
		else {
			char imsi[10];
			sprintf(imsi, "%04d", address);
			sprintf(buf, "%%%sW%d.%c%c.%c%c", device, pannel, imsi[0], imsi[1], imsi[2], imsi[3]);
			wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
			buf_pos += 2;
			strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
			buf_pos += strlen(buf);
		}
	}
	else {	// name label	read 일반 라벨명일때는 주소값이 필요없슴
		sprintf(buf, "%s", device);
		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", strlen(buf));	// PC 번호는 항상 0xFF일 것
		buf_pos += 2;
		strncpy((char*)&pt->commSendBuf[buf_pos], buf, strlen(buf));	// Word Read
		buf_pos += strlen(buf);
	}

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%04X", value);
	buf_pos += 4;
	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(pt->commSendBuf, buf_pos);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

	pt->commCountNeed = 9;	// ACK+STATION(2)+w+SS+ETC+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

      if(pt->commRecvBuf[0] == ACK && pt->commCountCurr >= 9) {	// ACK 가 나올때는 9글자가 온다.
			return COMMUNICATION_OK;
		}
		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] == NAK) {	// NAK 가 나올때는 데이터가 더 길기 때문에 부득이 여기서 검사한다.
				SetErrorCode(HexBufToWORD((char*)&pt->commRecvBuf[6]));
				return COMMUNICATION_ERR_STRING;
			}
			return COMMUNICATION_CODE_BAD;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





