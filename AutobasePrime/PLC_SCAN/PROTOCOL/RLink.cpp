//------------------------------------------------------------------------------
//	RLINK Protocol
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
//#include "alarm.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleRLINK(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (RLINK)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodRLINK(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

static BYTE GetCRC(BYTE *buf, int count)
{
	BYTE crc = 0x80;
	int  i;

	for(i = 0; i < count; i++) {
		crc += HexBufToBYTE((char*)&buf[i*2]);
	}

	crc = 0x0100-crc;

	return crc;
}

static void MakeBinaryString(char *buf, BYTE value)
{
	int i;
	
	for(i = 0; i < 8; i++) {
		if(WORD_MASK[i] & value)	buf[7-i] = '1';
		else								buf[7-i] = '0';
	}
}

static void PutDataInScanList(LOCAL_PORT_STRUCT *pt)
{
	SCAN_METHOD_STRUCT *sm;
	int i, j;
	int station;

	station = HexBufToBYTE((char*)&pt->commRecvBuf[4]);
	station += (HexBufToBYTE((char*)&pt->commRecvBuf[2]) & 0x03)*256;

	for(j = 0; j < pt->nScanMethodHap; j++) {
		sm = &pt->scanMethod[j];

		if(sm->station == station) {
			int address = sm->target;
			WORD value;
			for(i = 0; i < 3; i++) {
				value = HexBufToBYTE((char*)&pt->commRecvBuf[6+i*4])+
						HexBufToBYTE((char*)&pt->commRecvBuf[8+i*4])*256;
				PokeValue(pt, sm, address+i, value);
				// pt->bufWORD[address+i] = HexBufToBYTE((char*)&pt->commRecvBuf[6+i*4])+
				//							HexBufToBYTE((char*)&pt->commRecvBuf[8+i*4])*256;
			}	
			return;
		}
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadRLINK(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  	 count;

	if(pt->bReadingFlag == OFF) {
		pt->bReadingFlag = ON;

		BYTE imsi;

		sprintf((char*)&pt->commSendBuf[0],  "%02X", 0x14);	// command
		
		imsi = 0 << 2;
		imsi |= sm->station/256;
		sprintf((char*)&pt->commSendBuf[2],  "%02X", imsi);				// address
		sprintf((char*)&pt->commSendBuf[4],  "%02X", sm->station%256);	// sub-address
		if(strcmp(sm->type, "F") == 0)
			sprintf((char*)&pt->commSendBuf[6],  "%02X", 0x0E);	// failure status
		else 
			sprintf((char*)&pt->commSendBuf[6],  "%02X", 0x11);	// relay status

		sprintf((char*)&pt->commSendBuf[8],  "%02X", 0x00);	// relay status

		sprintf((char*)&pt->commSendBuf[10], "%02X", GetCRC(pt->commSendBuf, 5));
		pt->commSendBuf[12] = CR;

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 13);

		pt->commCountSend = 13;
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		return COMMUNICATION_WAITING;
	}
	else {
next_read:	
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	{
			pt->bReadingFlag = OFF;
			return COMMUNICATION_OK;
		}
		
		COMSTAT comStat;
		comStat.cbInQue = 0;
		PlcDeviceGetCommError(&pt->device, &comStat);
		if(comStat.cbInQue == 0)	return COMMUNICATION_WAITING;
		
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		if(pt->commCountCurr > 100) {	// 불량코드 over
			pt->bReadingFlag = OFF;
			return COMMUNICATION_CODE_BAD;
		}
		
		if(pt->commRecvBuf[pt->commCountCurr-1] == CR) {	// end code
			if(pt->commCountCurr == 2) {
				if(pt->commRecvBuf[0] == 'R') {
					PlcScanSetErrorString("RLink Send Error Code.");
					return COMMUNICATION_ERR_STRING;
				}
				if(pt->commRecvBuf[0] == 'O') {	// O.K 신호일 때 다음신호 대기.
					pt->commCountCurr = 0;
					return COMMUNICATION_WAITING;
				}
			}
			if(pt->commCountCurr == 21) {	// data 갯수가 맞다.
				if(pt->commRecvBuf[0] != '2' ||
					pt->commRecvBuf[1] != '5') {
					pt->bReadingFlag = OFF;
					return COMMUNICATION_CODE_BAD;
				}
				
				if(sm->target >= pt->nBufSizeWORD+3) {

				}
				else {
					PutDataInScanList(pt);					
				}
				pt->bReadingFlag = OFF;
				return COMMUNICATION_OK;
			}
		}
		goto next_read;
		// return COMMUNICATION_WAITING;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitRLINK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	TimeOutClass timeout;

	address += 1;

	// PlcDeviceClear(&pt->device);

	BYTE imsi;

	sprintf((char*)&pt->commSendBuf[0],  "%02X", 0x07);	// command
	imsi = address << 2;
	imsi |= station/256;
	sprintf((char*)&pt->commSendBuf[2],  "%02X", imsi);				// address
	sprintf((char*)&pt->commSendBuf[4],  "%02X", station%256);	// sub-address
	if(flag) 
		sprintf((char*)&pt->commSendBuf[6],  "%02X", 0x13);	// flag(0001)+network(0011b)
	else 
		sprintf((char*)&pt->commSendBuf[6],  "%02X", 0x03);	// flag(0000)+network(0011b)

	sprintf((char*)&pt->commSendBuf[8], "%02X", GetCRC(pt->commSendBuf, 4));
	pt->commSendBuf[10] = CR;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 11);

	pt->commCountNeed = 2;	// O+CR
	pt->commCountSend = 11;
	//pt->commCountCurr = 0;

	/*
	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(port_no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != 'O')
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != CR)
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(MAX_TIME_OUT))	return COMMUNICATION_TIME_OUT;
	}
	*/

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordRLINK(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	PlcScanSetErrorString("RLink는 WORD Write 명령어가 없습니다.");
	return COMMUNICATION_ERR_STRING;
}





