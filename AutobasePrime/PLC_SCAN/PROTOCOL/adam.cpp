// english O.K
//------------------------------------------------------------------------------
//	ADAM Protocol
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

void PlcScanDrawMethodTitleADAM(HDC hdc, int x, int y)
{
	char *string = "station, type, channel, buf address (ADAM)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodADAM(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

//----------------------------------------------------------------------------
//	MelSec PLC의 디바이스 종류를 검사한다.
//----------------------------------------------------------------------------

static int CheckDeviceADAM(char *type)
{
	if		 (strcmp(type, "X") == 0)	return 1;
	else if(strcmp(type, "Y") == 0)	return 1;
	else if(strcmp(type, "M") == 0)	return 1;
	else if(strcmp(type, "D") == 0)	return 1;
	else {
		return 0;
	}
}

static BYTE GetCRC(BYTE *buf, int size)
{
	BYTE crc = 0;
	int  i;

	//crc = buf[0];

	for(i = 0; i < size; i++) {
		crc ^= buf[i];
	}

	return crc;
}

BYTE GetCRC_NP(BYTE *buf, int size)
{
	BYTE crc = 0;
	int i;

	for(i = 0; i < size; i++) {
		crc ^= buf[i];
	}

	return crc;
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadADAM(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  count;
	//int  need;

	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		if(strcmp(sm->type, "AI") == 0) {
			pt->commSendBuf[0] = '#';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = CR;
			pt->commCountSend = 4;
		}
		else if(strcmp(sm->type, "AIN") == 0) {
			pt->commSendBuf[0] = '#';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = (BYTE)((sm->address%10)+'0');
			pt->commSendBuf[4] = CR;
			pt->commCountSend = 5;
		}
		else if(strcmp(sm->type, "DI") == 0 ||
				strcmp(sm->type, "DO") == 0) {
			pt->commSendBuf[0] = '$';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = '6';
			pt->commSendBuf[4] = CR;
			pt->commCountSend = 5;
		}
		else if(strcmp(sm->type, "AO") == 0) {
			pt->commSendBuf[0] = '$';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = '6';
			pt->commSendBuf[4] = CR;
			pt->commCountSend = 5;
		}
		else if(strcmp(sm->type, "AOF") == 0) {
			pt->commSendBuf[0] = '$';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = '8';
			pt->commSendBuf[4] = CR;
			pt->commCountSend = 5;
		}
		else if(strncmp(sm->type, "SP-", 3) == 0) {
			pt->commSendBuf[0] = '{';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = 'A';	
			pt->commSendBuf[4] = sm->type[3];	
			pt->commSendBuf[5] = LF;	
			pt->commSendBuf[6] = CR;
			pt->commCountSend = 7;
		}
		else if(strncmp(sm->type, "NP-", 3) == 0) {
			pt->commSendBuf[0] = '{';
			sprintf((char*)&pt->commSendBuf[1], "%02X", sm->station);
			pt->commSendBuf[3] = '0';	
			pt->commSendBuf[4] = '0';	
			pt->commSendBuf[5] = sm->type[3];	
			pt->commSendBuf[6] = sm->type[4];	
			
			BYTE crc = GetCRC_NP(&pt->commSendBuf[5], 2);

			sprintf((char*)&pt->commSendBuf[7], "%02X", crc);
			pt->commSendBuf[9] = CR;
			pt->commCountSend = 10;
		}
		else {
			PlcScanSetErrorString("ADAM Unknown device type [%s]", sm->type);
			return COMMUNICATION_ERR_STRING;		
		}
				
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

		pt->commCountNeed = 8;	// STX+STATION(2)+PCnum(2)+data+ETX+CRC(2)
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;

		//need = pt->commCountNeed-pt->commCountCurr;
		
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		if(count == 1) {
			if(pt->commRecvBuf[pt->commCountCurr] == CR) {
				if( strcmp(sm->type, "AI") == 0 ||
					strcmp(sm->type, "AIN") == 0 ) {

					if(pt->commRecvBuf[0] != '>') 
						return COMMUNICATION_CODE_BAD;
										
					int address = sm->target;
					float value;

					pt->commRecvBuf[pt->commCountCurr] = 0;

					value = (float)atof((char*)&pt->commRecvBuf[1]);
					PokeValue(pt, sm, address, value);
				}
				else if(strcmp(sm->type, "AO") == 0 ||
						strcmp(sm->type, "AOF") == 0 ) {

					if(pt->commRecvBuf[0] != '!') 
						return COMMUNICATION_CODE_BAD;
										
					int address = sm->target;
					float value;

					pt->commRecvBuf[pt->commCountCurr] = 0;

					value = (float)atof((char*)&pt->commRecvBuf[3]);
					PokeValue(pt, sm, address, value);
				}
				else if(strncmp(sm->type, "SP-", 3) == 0) {
					int address = sm->target;
					float value;

					pt->commRecvBuf[pt->commCountCurr] = 0;

					value = (float)atof((char*)pt->commRecvBuf);
					PokeValue(pt, sm, address, value);
				}
				else if(strncmp(sm->type, "NP-", 3) == 0) {
					
					int address = sm->target;
					WORD value;

					pt->commRecvBuf[pt->commCountCurr] = 0;

					for(int i = 0; i < sm->size && i < 100; i++) {
						value = HexBufToWORD((char*)&pt->commRecvBuf[4+i*4]);
						PokeValue(pt, sm, address+i, value);
					}
				}
				else {	// DI or DO
					if(pt->commRecvBuf[0] != '!') 
						return COMMUNICATION_CODE_BAD;
					
					int address = sm->target;

					PokeValue(pt, sm, address, HexBufToWORD((char*)&pt->commRecvBuf[1]));
				}
				return COMMUNICATION_OK;
			}

			else if(pt->commRecvBuf[pt->commCountCurr] == LF) {
				if(strncmp(sm->type, "SP-", 3) == 0) {
					int address = sm->target;
					float value;

					pt->commRecvBuf[pt->commCountCurr] = 0;

					value = (float)atof((char*)pt->commRecvBuf);
					PokeValue(pt, sm, address, value);
				}
				else if(strncmp(sm->type, "NP-", 3) == 0) {
					
					int address = sm->target;
					WORD value;

					pt->commRecvBuf[pt->commCountCurr] = 0;

					for(int i = 0; i < sm->size && i < 100; i++) {
						value = HexBufToWORD((char*)&pt->commRecvBuf[4+i*4]);
						PokeValue(pt, sm, address+i, value);
					}
				}
				return COMMUNICATION_OK;
			}

			else {

			}

			pt->commCountCurr++;

			if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;		
		}

		
	}
	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitADAM(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  	 count;
	//int  	 need;
	TimeOutClass timeout;
	char dle_flag = OFF;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = '#';
	sprintf((char*)&pt->commSendBuf[1], "%02X", station);
	pt->commSendBuf[3] = '1';		// one channel write 
	sprintf((char*)&pt->commSendBuf[4], "%1X", address%16);
	pt->commSendBuf[5] = '0';		// 0 - single channel write
	pt->commSendBuf[6] = flag ? '1' : '0';		// flag
	pt->commSendBuf[7] = CR;

	pt->commCountSend = 8;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 2;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
//		need = pt->commCountNeed-pt->commCountCurr;
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != '>')
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != CR)	// Error
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordADAM(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device)
{
	int  	 count;
	//int  	 need;
	TimeOutClass timeout;
	char dle_flag = OFF;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = '#';
	sprintf((char*)&pt->commSendBuf[1], "%02X", station);
	sprintf((char*)&pt->commSendBuf[3], "%06.3f%c", value, CR);

	pt->commCountSend = strlen((char*)pt->commSendBuf);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 2;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
//		need = pt->commCountNeed-pt->commCountCurr;
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != '>')
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != CR)	// Error
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}

	return COMMUNICATION_OK;
}





