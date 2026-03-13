//------------------------------------------------------------------------------
//	SAIA PCD_SBUS Protocol
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
#include <crc.hpp>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitlePCD_SBUS(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (PCD_SBUS)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodPCD_SBUS(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

enum {
	TYPE_NONE,
	TYPE_BIT,
	TYPE_DWORD,
};

static int GetType(char *type)
{
	if(type[0] == 'I')		return TYPE_BIT;
	else if(type[0] == 'O')	return TYPE_BIT;
	else if(type[0] == 'F')	return TYPE_BIT;
	else if(type[0] == 'R')	return TYPE_DWORD;
	else if(type[0] == 'T')	return TYPE_DWORD;
	else if(type[0] == 'C')	return TYPE_DWORD;
	else					return TYPE_NONE;
}

static char ReadTypeToCode(char *type)
{
	if(type[0] == 'I')		return 0x03;
	else if(type[0] == 'O')	return 0x05;
	else if(type[0] == 'F')	return 0x02;
	else if(type[0] == 'R')	return 0x06;	// register
	else if(type[0] == 'T')	return 0x07;
	else if(type[0] == 'C')	return 0x00;
	else					return -1;
}

static char WriteTypeToCode(char *type)
{
	if(type[0] == 'O')		return 0x0D;
	else if(type[0] == 'F')	return 0x0B;
	else if(type[0] == 'R')	return 0x0E;	// register
	else if(type[0] == 'T')	return 0x0F;
	else if(type[0] == 'C')	return 0x0A;
	else					return -1;
}

static void DelayAfterAddressSend(LOCAL_PORT_STRUCT *pt)
{
	CommaBlockString comma;
	int mili_sec;

	comma.Set(pt->sScanProtocolOption);
	comma.GetInt(mili_sec);

	if(mili_sec < 0)	mili_sec = 0;
	if(mili_sec < 100)	mili_sec = 100;

	Sleep(mili_sec);
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadPCD_SBUS(LOCAL_PORT_STRUCT *pt, int pos)
{
	WORD crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  	 count;
	int		 type;
	
	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		type = GetType(sm->type);

		if(type == TYPE_BIT) {	// I, O, F
			pt->commSendBuf[0] =  (BYTE)sm->station;	
			pt->commSendBuf[1] =  ReadTypeToCode(sm->type);		// command
			pt->commSendBuf[2] =  sm->size*16-1;			// 0 = 1bit, F = 16 register
			pt->commSendBuf[3] =  ((WORD)sm->address*16)/256;	
			pt->commSendBuf[4] =  ((WORD)sm->address*16)%256;	
			crc = GetCRC_16_12_5_1(&pt->commSendBuf[0], 5);
			pt->commSendBuf[5] =  crc/256;	
			pt->commSendBuf[6] =  crc%256;	

			pt->commCountSend = 7;
		}
		else if(type == TYPE_DWORD) {	// R, T, C
			pt->commSendBuf[0] =  (BYTE)sm->station;	
			pt->commSendBuf[1] =  ReadTypeToCode(sm->type);		// command
			pt->commSendBuf[2] =  (BYTE)sm->size-1;	
			pt->commSendBuf[3] =  ((WORD)sm->address)/256;	
			pt->commSendBuf[4] =  ((WORD)sm->address)%256;	

			crc = GetCRC_16_12_5_1(&pt->commSendBuf[0], 5);
			pt->commSendBuf[5] =  crc/256;	
			pt->commSendBuf[6] =  crc%256;	

			pt->commCountSend = 7;
		}
		else {
			PlcScanSetErrorString("PCD_SBUS에는 없는 영역 [%s]", sm->type);
			return COMMUNICATION_ERR_STRING;
			/*
			pt->commSendBuf[0] =  STX;	// STX
			pt->commSendBuf[1] =  'R';	// STX
			pt->commSendBuf[2] =  'K';	// STX
			pt->commSendBuf[3] =  ETX;	// STX
			pt->commSendBuf[4] =  0x1A;	// STX
			*/
		}

		PlcDeviceSetParity(&pt->device, 3);	// mark
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 1);
		DelayAfterAddressSend(pt);		
		PlcDeviceSetParity(&pt->device, 4);	// space
		PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[1], pt->commCountSend-1);

		pt->commCountCurr = 0;
		if(type == TYPE_BIT)		pt->commCountNeed = (sm->size*2)+2;	// ACK
		else if(type == TYPE_DWORD)	pt->commCountNeed = sm->size*4+2;
		else						pt->commCountNeed = 100;	

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		DisplaySendCodeNextLine(pt->no);

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0) {
			return COMMUNICATION_WAITING;
		}

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			crc = MAKEWORD(pt->commRecvBuf[pt->commCountCurr-1], pt->commRecvBuf[pt->commCountCurr-2]);
			if(crc != GetCRC_16_12_5_1(&pt->commRecvBuf[0], pt->commCountCurr-2)) 
				return COMMUNICATION_CODE_BAD;

			type = GetType(sm->type);
			
			if(type == TYPE_BIT) {
				WORD value;
				int  i;

				for(i = 0; i < sm->size; i++) {
					value = MAKEWORD(pt->commRecvBuf[i*2+0], pt->commRecvBuf[i*2+1]);
					PokeValue(pt, sm, sm->target+i, value);
				}
			}
			else if(type == TYPE_DWORD) {
				WORD hi, lo;
				DWORD value;
				int  i;

				for(i = 0; i < sm->size; i++) {
					lo = MAKEWORD(pt->commRecvBuf[i*4+3], pt->commRecvBuf[i*4+2]);
					hi = MAKEWORD(pt->commRecvBuf[i*4+1], pt->commRecvBuf[i*4+0]);
					value = MAKELONG(lo, hi);
					PokeValue(pt, sm, sm->target+i, value);
				}
			}
			else {

			}

			return COMMUNICATION_OK;
		}
	}
}


//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitPCD_SBUS(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;
	int type;

	type = GetType(device);

	if(type != TYPE_BIT) {
		PlcScanSetErrorString("PCD_SBUS Bit 종류 (O,F) 에만 출력할 수 있습니다.");				
		return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] =  station;	
	pt->commSendBuf[1] =  WriteTypeToCode(device);	
	pt->commSendBuf[2] =  3;	// w-count
	pt->commSendBuf[3] =  address/256;	
	pt->commSendBuf[4] =  address%256;	
	pt->commSendBuf[5] =  0;	// fio-count
	pt->commSendBuf[6] =  (BYTE)flag;	// fio-byte
	crc = GetCRC_16_12_5_1(pt->commSendBuf, 7);
	pt->commSendBuf[7] =  crc/256;	// 
	pt->commSendBuf[8] =  crc%256;	// 

	pt->commCountSend = 9;

	PlcDeviceSetParity(&pt->device, 3);	// mark
	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 1);
	DelayAfterAddressSend(pt);		
	PlcDeviceSetParity(&pt->device, 4);	// space
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[1], pt->commCountSend-1);

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

int PlcScanWriteWordPCD_SBUS(LOCAL_PORT_STRUCT *pt, int station, WORD address, double value, char *device)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;
	int type;

	type = GetType(device);

	if(type != TYPE_DWORD) {
		PlcScanSetErrorString("PCD_SBUS DWORD 종류 (R,T,C) 에만 출력할 수 있습니다.");				
		return COMMUNICATION_ERR_STRING;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] =  station;	
	pt->commSendBuf[1] =  WriteTypeToCode(device);	
	pt->commSendBuf[2] =  5;	// w-count
	pt->commSendBuf[3] =  address/256;	
	pt->commSendBuf[4] =  address%256;	
	pt->commSendBuf[5] =  HIBYTE(HIWORD((DWORD)value));
	pt->commSendBuf[6] =  LOBYTE(HIWORD((DWORD)value));
	pt->commSendBuf[7] =  HIBYTE(LOWORD((DWORD)value));
	pt->commSendBuf[8] =  LOBYTE(LOWORD((DWORD)value));
	crc = GetCRC_16_12_5_1(pt->commSendBuf, 9);
	pt->commSendBuf[9] =  crc/256;	// 
	pt->commSendBuf[10] =  crc%256;	// 

	pt->commCountSend = 11;

	PlcDeviceSetParity(&pt->device, 3);	// mark
	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 1);
	DelayAfterAddressSend(pt);		
	PlcDeviceSetParity(&pt->device, 4);	// space
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[1], pt->commCountSend-1);

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



