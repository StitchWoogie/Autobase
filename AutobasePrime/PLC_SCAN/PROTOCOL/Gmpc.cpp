// english O.K
//------------------------------------------------------------------------------
//	GMPC Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
//  commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <tools.h>
#include <totaldef.h>
#include <glib.h>
#include <crc.hpp>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleGMPC(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodGMPC(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
	TextOut(hdc, x+cxChar*10, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*14, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->size);		// size word of read
	TextOut(hdc, x+cxChar*18, y, buf, strlen(buf));
}

// X.X로 표기

static double ByteToX_X(BYTE value)
{
	return (value/16)+((value%16)*0.1);
}

// XX로 표기

static double ByteToXX(BYTE value)
{
	return (value/16)*10+(value%16);
}

// 0X로 표기

static double ByteTo0X(BYTE value)
{
	return (value%16);
}

// .XX로 표기

static double ByteTo_XX(BYTE value)
{
	return ((value/16)*10+(value%16))*0.01;
}

// XX.XX로 표기

static double TwoByteToXX_XX(BYTE value1, BYTE value2)
{
	return ((value1/16)*10+(value1%16))+((value2/16)*10+(value2%16))*0.01;
}

// 0X.XX로 표기

static double TwoByteTo0X_XX(BYTE value1, BYTE value2)
{
	return (value1%16)+((value2/16)*10+(value2%16))*0.01;
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadGMPC(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  	 count;
	BYTE crc;
	
	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		pt->commCountSend = 0;

		if(stricmp(sm->type, "uRTU") == 0) {
			pt->commSendBuf[0] = 0xFF;	// preamble byte 1
			pt->commSendBuf[1] = 0x00;	// preamble byte 2
			pt->commSendBuf[2] = (BYTE)sm->station;	// 0~0xFF
			pt->commSendBuf[3] = 1;		// 7bit = error bit - 0
										// 5~6bit = reserved - 00
										// 0~4 = data byte size
			pt->commSendBuf[4] = 0x10 | ((BYTE)sm->address & 0x0F);	// hi nibble 1 = 데이터 요청 명령 
																		// lo nibble  command code
			pt->commSendBuf[5] = GetCRC_SUM8(&pt->commSendBuf[2], 3);

			pt->commCountSend = 6;
			pt->commCountCurr = 0;
			pt->commCountNeed = 4+9+1;
		}
		else {
			if(sm->address < 16) {
				pt->commSendBuf[0] = 0xFF;	// preamble byte 1
				pt->commSendBuf[1] = 0x00;	// preamble byte 2
				pt->commSendBuf[2] = (BYTE)sm->station;	// 0~0xFF
				pt->commSendBuf[3] = 1;		// 7bit = error bit - 0
											// 5~6bit = reserved - 00
											// 0~4 = data byte size
				pt->commSendBuf[4] = 0x10 | ((BYTE)sm->address & 0x0F);	// hi nibble 1 = 데이터 요청 명령 
																		// lo nibble  command code
				pt->commSendBuf[5] = GetCRC_SUM8(&pt->commSendBuf[2], 3);

				pt->commCountSend = 6;
				pt->commCountCurr = 0;
				pt->commCountNeed = 4+16+1;	 
			}
			else {
				pt->commSendBuf[0] = 0xFF;	// preamble byte 1
				pt->commSendBuf[1] = 0x00;	// preamble byte 2
				pt->commSendBuf[2] = (BYTE)sm->station;	// 0~0xFF
				pt->commSendBuf[3] = 2;		// 7bit = error bit - 0
										// 5~6bit = reserved - 00
										// 0~4 = data byte size
				pt->commSendBuf[4] = 0x1D;
				pt->commSendBuf[5] = ((WORD)sm->address-20);	//
				pt->commSendBuf[6] = GetCRC_SUM8(&pt->commSendBuf[2], 4);

				pt->commCountSend = 7;
				pt->commCountCurr = 0;
				pt->commCountNeed = 4+16+1;
			}
		}

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

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
			if(pt->commRecvBuf[0] != 0xFF)	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != 0x00)	return COMMUNICATION_CODE_BAD;

			crc = pt->commRecvBuf[pt->commCountNeed-1];
			
			if(GetCRC_SUM8(&pt->commRecvBuf[2], pt->commCountNeed-3) != crc)	return COMMUNICATION_CODE_BAD;
			
			int address = sm->target;

			if(stricmp(sm->type, "uRTU") == 0) {
				if(sm->address == 0) {
					PokeValue(pt, sm, address+0, pt->commRecvBuf[6]);
					PokeValue(pt, sm, address+1, pt->commRecvBuf[7]);
					PokeValue(pt, sm, address+2, pt->commRecvBuf[8]);
					PokeValue(pt, sm, address+3, MAKEWORD(pt->commRecvBuf[9], pt->commRecvBuf[10]));
					PokeValue(pt, sm, address+4, MAKEWORD(pt->commRecvBuf[11], pt->commRecvBuf[12]));
				}
				else {
					PokeValue(pt, sm, address+0, MAKEWORD(pt->commRecvBuf[7], pt->commRecvBuf[6]));
					PokeValue(pt, sm, address+1, MAKEWORD(pt->commRecvBuf[9], pt->commRecvBuf[8]));
					PokeValue(pt, sm, address+2, MAKEWORD(pt->commRecvBuf[11], pt->commRecvBuf[10]));
					PokeValue(pt, sm, address+3, MAKEWORD(pt->commRecvBuf[13], pt->commRecvBuf[12]));
				}

				return COMMUNICATION_OK;
			}
			
			if(sm->address == 0) {	// status data
				int j;
				WORD mask = 0;

				for(j = 0; j < 14 && j < sm->size; j++) {	
					PokeValue(pt, sm, address+j, pt->commRecvBuf[6+j]);
				}
			}
			else if(sm->address == 20) {	// 계측부 Setting
				PokeValue(pt, sm, address+0, pt->commRecvBuf[6]);	// 정격 전류
				PokeValue(pt, sm, address+1, pt->commRecvBuf[7]);	// 정격 전압
				PokeValue(pt, sm, address+2, pt->commRecvBuf[8]);	// 결선 방식
				PokeValue(pt, sm, address+3, pt->commRecvBuf[9]);	// 선택된 계전요소
				PokeValue(pt, sm, address+4, MAKEWORD(pt->commRecvBuf[11], pt->commRecvBuf[10]));	// NO CB
				PokeValue(pt, sm, address+5, MAKEWORD(pt->commRecvBuf[13], pt->commRecvBuf[12]));	// NO CB
				PokeValue(pt, sm, address+6, MAKEWORD(pt->commRecvBuf[15], pt->commRecvBuf[14]));	// NO CB
				PokeValue(pt, sm, address+7, pt->commRecvBuf[16]);	// NO CB
			}
			else if(sm->address == 21) {	// OCR/OCGR SETTING
				PokeValue(pt, sm, address+0, ByteToX_X(pt->commRecvBuf[6]));	// OCR 한시 전류
				PokeValue(pt, sm, address+1, ByteToXX(pt->commRecvBuf[7]));	// OCR 한시 전류
				PokeValue(pt, sm, address+2, ByteTo0X(pt->commRecvBuf[8]));	// OCR 한시 전류
				PokeValue(pt, sm, address+3, TwoByteTo0X_XX(pt->commRecvBuf[9], pt->commRecvBuf[10]));	// OCR 동작시간
				PokeValue(pt, sm, address+4, ByteTo_XX(pt->commRecvBuf[11]));	
				PokeValue(pt, sm, address+5, ByteToX_X(pt->commRecvBuf[12]));	
				PokeValue(pt, sm, address+6, ByteTo0X(pt->commRecvBuf[13]));	
				PokeValue(pt, sm, address+7, TwoByteTo0X_XX(pt->commRecvBuf[14], pt->commRecvBuf[15]));	// OCR 동작시간
				PokeValue(pt, sm, address+8, ByteTo0X(pt->commRecvBuf[16]));	
			}
			else if(sm->address == 22) {	// OCR/OCGR SETTING
				PokeValue(pt, sm, address+0, TwoByteTo0X_XX(pt->commRecvBuf[6], pt->commRecvBuf[7]));	// OCR 동작시간
				PokeValue(pt, sm, address+1, TwoByteToXX_XX(pt->commRecvBuf[8], pt->commRecvBuf[9]));	// OCR 동작시간
				PokeValue(pt, sm, address+2, ByteTo0X(pt->commRecvBuf[10]));	// OCR 한시 전류
				PokeValue(pt, sm, address+3, ByteTo_XX(pt->commRecvBuf[11]));	
				PokeValue(pt, sm, address+4, TwoByteToXX_XX(pt->commRecvBuf[12], pt->commRecvBuf[13]));	// OCR 동작시간
				PokeValue(pt, sm, address+5, ByteTo0X(pt->commRecvBuf[14]));	// OCR 한시 전류
			}
			else if(sm->address == 23) {	// OCR/OCGR SETTING
				PokeValue(pt, sm, address+0, ByteTo_XX(pt->commRecvBuf[6]));	
				PokeValue(pt, sm, address+1, ByteTo_XX(pt->commRecvBuf[7]));	
				PokeValue(pt, sm, address+2, TwoByteTo0X_XX(pt->commRecvBuf[8], pt->commRecvBuf[9]));	// OCR 동작시간
				PokeValue(pt, sm, address+3, ByteTo0X(pt->commRecvBuf[10]));	// OCR 한시 전류
				PokeValue(pt, sm, address+4, ByteToX_X(pt->commRecvBuf[11]));	
				PokeValue(pt, sm, address+5, ByteTo_XX(pt->commRecvBuf[12]));	
				PokeValue(pt, sm, address+6, TwoByteToXX_XX(pt->commRecvBuf[13], pt->commRecvBuf[14]));	// OCR 동작시간
				PokeValue(pt, sm, address+7, ByteTo0X(pt->commRecvBuf[15]));	// OCR 한시 전류
			}
			else {	// analog
				int j;
				WORD mask = 0;
				static float value = 1;

				//value *= 1.1;

				for(j = 0; j < 3 && j < sm->size; j++) {	
					//memcpy(&value, &pt->commRecvBuf[5+j*4], 4);
					((BYTE*)&value)[0] = pt->commRecvBuf[5+j*4+3];
					((BYTE*)&value)[1] = pt->commRecvBuf[5+j*4+2];
					((BYTE*)&value)[2] = pt->commRecvBuf[5+j*4+1];
					((BYTE*)&value)[3] = pt->commRecvBuf[5+j*4+0];
					PokeValue(pt, sm, address+j, value);
				}
			}

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitGMPC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device, WORD pannel)
{
//	int  	 count;
//	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	pt->commCountSend = 0;

	pt->commSendBuf[0] = 0xFF;	// preamble byte 1
	pt->commSendBuf[1] = 0x00;	// preamble byte 2
	pt->commSendBuf[2] = (BYTE)station;	// 0~0xFF
	pt->commSendBuf[3] = 2;		// 7bit = error bit - 0
								// 5~6bit = reserved - 00
								// 0~4 = data byte size
	
	pt->commSendBuf[4] = 0x20;
	if(stricmp(device, "uRTU") == 0) {
		pt->commSendBuf[5] = (BYTE)address;
		if(pannel == 1) {
			pt->commSendBuf[5] |= 0x80;
		}
		else {
			if(flag)	pt->commSendBuf[5] |= 0x40;
		}
	}
	else {
		pt->commSendBuf[5] = (BYTE)address;	// hi nibble 1 = 데이터 요청 명령 
											// lo nibble  command code
	}
	pt->commSendBuf[6] = GetCRC_SUM8(&pt->commSendBuf[2], 4);


	pt->commCountNeed = 4+16+1;//16+sm->size*4;	// STX+COMMAND+STATION+ADDRESS+SIZE+DATA+CRC+ETX
	pt->commCountSend = 7;
	pt->commCountCurr = 0;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	//if(address >= 0x40)	// GIMAC
	Sleep(100);
	return COMMUNICATION_OK;	// return code가 없다.
/*
	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(pt->no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
	*/
	
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordGMPC(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	PlcScanSetErrorString("GMPC 에서는 아날로그 출력을 할 수 없습니다.");
	return COMMUNICATION_ERR_STRING;

	/*
	int  	 count;
	TimeOutClass timeout;

	PlcDeviceClear(pt->no);

	pt->commSendBuf[0] = SOH;	// SOH
	pt->commSendBuf[1] = SOH;	// SOH
	pt->commSendBuf[2] = 0x88;	
	wsprintf((char*)&pt->commSendBuf[3], "%02d", station);	// destination
	wsprintf((char*)&pt->commSendBuf[5], "%02d", 98);	// source
	pt->commSendBuf[7] = STX;	
	
	wsprintf((char*)&pt->commSendBuf[8],  "%03d", 11);	// data length
	wsprintf((char*)&pt->commSendBuf[11], "%02d", 24);	// op code

	wsprintf((char*)&pt->commSendBuf[13], "%02d", address/100);	// module address
	wsprintf((char*)&pt->commSendBuf[15], "%d",   0);			// module id
	wsprintf((char*)&pt->commSendBuf[16], "%02d", (address%100)+1);	// point address

	wsprintf((char*)&pt->commSendBuf[18], "+%03d.%01d", value/10, value%10);	
	pt->commSendBuf[24] = 0x04;	
	wsprintf((char*)&pt->commSendBuf[25], "%02X", GetCRC_SUM8(&pt->commSendBuf[3], 28-7));	// CRC
	pt->commSendBuf[27] = 0x0D;	

	pt->commCountNeed = 10;//16+sm->size*4;	// STX+COMMAND+STATION+ADDRESS+SIZE+DATA+CRC+ETX
	pt->commCountSend = 28;

	PlcDeviceWriteContinue(pt->no, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(pt->no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
	*/
}



