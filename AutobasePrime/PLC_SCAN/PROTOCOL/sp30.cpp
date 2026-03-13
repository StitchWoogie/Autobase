//------------------------------------------------------------------------------
//	SP30 Protocol
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
//#include "alarm.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleSP30(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address (SP30)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodSP30(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

	/*
	wsprintf(buf, "%3d", sm->extra1);		// size word of read
	TextOut(hdc, x+cxChar*15, y, buf, strlen(buf));
	*/
}

//----------------------------------------------------------------------------
//	MelSec PLC의 디바이스 종류를 검사한다.
//----------------------------------------------------------------------------

static int CheckDeviceSP30(char *type)
{
	if	   (strcmp(type, "X") == 0)	return 1;
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

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadSP30(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  count;

	if(strcmp(sm->type,  "R") == 0 ||
		strcmp(sm->type, "T") == 0 ||
		strcmp(sm->type, "B") == 0 ||
		strcmp(sm->type, "C") == 0) {
	}
	else {
		PlcScanSetErrorString("SP30 콘트롤러에는 없는 type [%s]", sm->type);
		return COMMUNICATION_ERR_STRING;		
	}

	if(pt->bReadingFlag == OFF) {
		
		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = 'A';
		pt->commSendBuf[1] = sm->type[0];
		pt->commSendBuf[2] = LF;
		pt->commCountSend  = 3;
				
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

		pt->commCountNeed = 8;	// STX+STATION(2)+PCnum(2)+data+ETX+CRC(2)
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

		if(count == 1) {
			if(pt->commRecvBuf[pt->commCountCurr] == LF) {
				/*
				if(pt->commRecvBuf[0] != '>') 
					return COMMUNICATION_CODE_BAD;
				if(pt->commRecvBuf[0] != '>') 
					return COMMUNICATION_CODE_BAD;
				*/

				int address = sm->target;

				pt->commRecvBuf[pt->commCountCurr] = 0;
				PokeValue(pt, sm, address, atol((char*)pt->commRecvBuf));
			
				return COMMUNICATION_OK;
			}

			pt->commCountCurr ++;

			if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;		
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitSP30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	PlcScanSetErrorString("SP30 콘트롤러는 Bit Write 명령어가 없습니다.");
	return COMMUNICATION_ERR_STRING;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordSP30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
/*
	int  	 count;
	TimeOutClass timeout;
	char dle_flag = OFF;

	PlcDeviceClear(port_no);

	pt->commCountSend = 0;
	pt->commSendBuf[pt->commCountSend] = STX;
	pt->commCountSend++;

	AddSendCode(0xA2);
	AddSendCode((BYTE)station);

	// device type으로 인식되는 것은 인식부호에 따라 시작한다.
	if(stricmp(device, "PWR") == 0) {	
		if(flag)		AddSendCode(0x10);
		else			AddSendCode(0x11);
	}
	else {
		AddSendCode((BYTE)address);
		if(address == 0x27) {	// add set
			AddSendCode(station);
		}
	}
	
	pt->commSendBuf[pt->commCountSend] = ETX;
	pt->commCountSend++;
	pt->commSendBuf[pt->commCountSend] = GetCRC(&pt->commSendBuf[0], pt->commCountSend);	
	pt->commCountSend++;

	PlcDeviceWriteContinue(port_no, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = 7;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	dle_flag = OFF;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(port_no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 1 && pt->commRecvBuf[pt->commCountCurr] == 0x10) {
			dle_flag = ON;
		}

		else {
			if(count == 1) {
				if(dle_flag) {
					pt->commRecvBuf[pt->commCountCurr] -= 0x20;	
					dle_flag = OFF;
				}
				pt->commCountCurr += count;
			}
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != STX)
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[4] != 0x00)	// Error
				return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[5] != ETX)
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(MAX_TIME_OUT))	return COMMUNICATION_TIME_OUT;
	}
*/
	return COMMUNICATION_OK;
}





