//------------------------------------------------------------------------------
//	SAIA PCD Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
//  commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <totaldef.h>
#include <tools.h>
#include <glib.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitlePCD(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (PCD)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodPCD(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
	BYTE crc = 0;
	int i;

	for(i = 0; i < size; i++) {
		crc ^= buf[i];
	}

	return crc;
}

enum {
	TYPE_NONE,
	TYPE_BIT,
	TYPE_DWORD,
	TYPE_STATUS,
};

static int GetType(char *type)
{
	if(type[0] == 'I')		return TYPE_BIT;
	else if(type[0] == 'O')	return TYPE_BIT;
	else if(type[0] == 'F')	return TYPE_BIT;
	else if(type[0] == 'R')	return TYPE_DWORD;
	else if(type[0] == 'T')	return TYPE_DWORD;
	else if(type[0] == 'C')	return TYPE_DWORD;
	else if(type[0] == 'S')	return TYPE_STATUS;
	else					return TYPE_NONE;
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadPCD(LOCAL_PORT_STRUCT *pt, int pos)
{
	WORD crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  	 count;
	int		 type;
	
	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		type = GetType(sm->type);

		if(type == TYPE_BIT) {	// I, O, F
			pt->commSendBuf[0] =  STX;	
			pt->commSendBuf[1] =  'R';	
			pt->commSendBuf[2] =  sm->type[0];	
			pt->commSendBuf[3] =  'F';
			sprintf((char*)&pt->commSendBuf[4], "%04X", sm->address*16);
			pt->commSendBuf[8] =  ETX;	
			pt->commSendBuf[9] =  GetCRC(&pt->commSendBuf[1], 8);	

			pt->commCountSend = 10;
		}
		else if(type == TYPE_DWORD) {	// R, T, C
			if(sm->size > 16)	sm->size = 16;

			pt->commSendBuf[0] =  STX;	// STX
			pt->commSendBuf[1] =  'R';	// STX
			pt->commSendBuf[2] =  sm->type[0];	// STX
			sprintf((char*)&pt->commSendBuf[3], "%01X", sm->size-1);
			sprintf((char*)&pt->commSendBuf[4], "%03X", sm->address);
			pt->commSendBuf[7] =  ETX;	// STX
			pt->commSendBuf[8] =  GetCRC(&pt->commSendBuf[1], 7);	

			pt->commCountSend = 9;
		}
		else if(type == TYPE_STATUS) {
			//if(sm->size > 16)	sm->size = 16;

			pt->commSendBuf[0] =  STX;	// STX
			pt->commSendBuf[1] =  'R';	// STX
			pt->commSendBuf[2] =  sm->type[0];	// STX
			pt->commSendBuf[3] =  sm->type[1];	// STX
			if(sm->type[1] < '0' || sm->type[1] > '7')
			pt->commSendBuf[3] =  '7';	// STX
			pt->commSendBuf[4] =  ETX;	// STX
			pt->commSendBuf[5] =  GetCRC(&pt->commSendBuf[1], 4);	

			pt->commCountSend = 6;
		}
		else {
			PlcScanSetErrorString("PCD에는 없는 영역 [%s]", sm->type);
			return COMMUNICATION_ERR_STRING;
			/*
			pt->commSendBuf[0] =  STX;	// STX
			pt->commSendBuf[1] =  'R';	// STX
			pt->commSendBuf[2] =  'K';	// STX
			pt->commSendBuf[3] =  ETX;	// STX
			pt->commSendBuf[4] =  0x1A;	// STX
			*/
		}

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

		pt->commCountNeed = 1;	// ACK
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

		if(pt->commRecvBuf[0] == ACK)	break;
		
		return COMMUNICATION_CODE_BAD;
	}

	PlcDeviceWrite(&pt->device, ENQ);

	type = GetType(sm->type);

	if(type == TYPE_BIT)		pt->commCountNeed = 19;	
	else if(type == TYPE_DWORD)	pt->commCountNeed = 3+sm->size*8;
	else if(type == TYPE_STATUS)pt->commCountNeed = 4;	// STX+S+ETX+CRC
	else						pt->commCountNeed = 100;	

	pt->commCountSend = 1;
	pt->commCountCurr = 0;

	DisplaySendCodeNextLine(pt->no);

	pt->timeout->Reset();

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != STX)	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[pt->commCountCurr-1] != GetCRC(&pt->commRecvBuf[1], pt->commCountCurr-2)) 
				return COMMUNICATION_CODE_BAD;

			type = GetType(sm->type);
			
			if(type == TYPE_BIT) {
				WORD mask = 0;
				int  i;

				for(i = 0; i < 16; i++) {
					if(pt->commRecvBuf[1+i] == '1')		mask |= WORD_MASK[i];
				}

				PokeValue(pt, sm, sm->target, mask);
			}
			else if(type == TYPE_DWORD) {
				DWORD value;
				int  i;

				for(i = 0; i < sm->size; i++) {
					value = MAKELONG(HexBufToWORD((char*)&pt->commRecvBuf[1+i*8+4]), 
									 HexBufToWORD((char*)&pt->commRecvBuf[1+i*8+0]));		             
					PokeValue(pt, sm, sm->target+i, value);
				}
			}
			else if(type == TYPE_STATUS) {
				if(pt->commRecvBuf[1] == 'R')		PokeValue(pt, sm, sm->target, 1);
				else if(pt->commRecvBuf[1] == 'H')	PokeValue(pt, sm, sm->target, 2);
				else if(pt->commRecvBuf[1] == 'C')	PokeValue(pt, sm, sm->target, 4);
				else if(pt->commRecvBuf[1] == 'D')	PokeValue(pt, sm, sm->target, 8);
				else								PokeValue(pt, sm, sm->target, 0);
			}
			else {

			}

			PlcDeviceWrite(&pt->device, ACK);	// ACK를 보낸다.

			return COMMUNICATION_OK;
		}

		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitPCD(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;
	int type;

	type = GetType(device);

	if(device[0] == 'S') {
		PlcDeviceClear(&pt->device);

		char cpu;
		if(device[1] < '0' || device[1] > '7') {
			cpu = '7';
		}
		else 
			cpu = device[1];

		pt->commSendBuf[0] =  STX;	
		pt->commSendBuf[1] =  flag ? 'G' : 'S';	
		pt->commSendBuf[2] =  'P';	
		pt->commSendBuf[3] =  cpu;	
		pt->commSendBuf[4] =  ETX;	
		pt->commSendBuf[5] =  GetCRC(&pt->commSendBuf[1], 4);	

		pt->commCountSend = 6;
	}
	else {
		if(type != TYPE_BIT) {
			PlcScanSetErrorString("PCD Bit 종류 (O,F,S) 에만 출력할 수 있습니다.");
			return COMMUNICATION_ERR_STRING;
		}

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] =  STX;	
		pt->commSendBuf[1] =  'W';	
		pt->commSendBuf[2] =  device[0];	
		sprintf((char*)&pt->commSendBuf[3], "%04X", address);
		pt->commSendBuf[7] =  flag ? '1' : '0';	
		pt->commSendBuf[8] =  ETX;	
		pt->commSendBuf[9] =  GetCRC(&pt->commSendBuf[1], 8);

		pt->commCountSend = 10;
	}

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 1;	// ACK
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK)	return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}

}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordPCD(LOCAL_PORT_STRUCT *pt, int station, WORD address, double value, char *device)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;
	int type;

	type = GetType(device);

	if(type != TYPE_DWORD) {
		PlcScanSetErrorString("PCD DWORD 종류 (R,T,C) 에만 출력할 수 있습니다.");				
		return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] =  STX;	
	pt->commSendBuf[1] =  'W';	
	pt->commSendBuf[2] =  device[0];	
	sprintf((char*)&pt->commSendBuf[3], "%03X", address);
	sprintf((char*)&pt->commSendBuf[6], "%08X", (DWORD)value);
	pt->commSendBuf[14] =  ETX;	
	pt->commSendBuf[15] =  GetCRC(&pt->commSendBuf[1], 14);	

	pt->commCountSend = 16;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 1;	// ACK
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK)	return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}



