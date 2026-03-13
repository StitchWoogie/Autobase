//------------------------------------------------------------------------------
//	National WR3380-82 Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//
//	WR-3380 에서는 약간의 통신오류가 생길 수 있다.
// Write신호를 연속적으로 여러개 보낸 후 곧바로 Read를 하면 Write를 처리하는
// 시간이 약간 걸리므로 통신시간 초과가 종종 생긴다. 이것은 Write는 오류는 없느나
// Read가 약간 늦어지는 원인이 된다.
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

void WaitSec(int sec);

void PlcScanDrawMethodTitleWR3380(HDC hdc, int x, int y)
{
	char *string = "station, type, T/U address, buf address, T/U 개수(1<=num<=4)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodWR3380(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
// 통신을 읽으면 워드단위가 채널 하나에 해당한다.
//------------------------------------------------------------------------------

int ReadContinueSec(DEVICE_STRUCT *dev, char *buf, int size, int sec)
{
	int  	 count, curr;
	int 	 sec_old, sec_curr;
	struct time t;

	curr = 0;
	sec_curr = 0;
	gettime(&t);
	sec_old = t.ti_sec;

	while(1) {
		count = PlcDeviceReadContinue(dev, &buf[curr], 1);

		curr += count;

		// 필요한 바이트 수 만큼 데이터를 모두 받았다.
		if(curr >= size) {
			return 1;
		}

		// check time out
		gettime(&t);
		if(t.ti_sec != sec_old) {
			sec_old = t.ti_sec;
			sec_curr++;
			if(sec_curr >= sec) {
				return 0;		// 시간초과에 걸렸다.
			}
		}
	}
}

//------------------------------------------------------------------------------
// WR3380 통신 모듈을 초기화 한다.
//------------------------------------------------------------------------------

static int ResetWR3380(DEVICE_STRUCT *dev)
{
	char buf[10];

	PlcDeviceClear(dev);

	// reset WR3380
	PlcDeviceWrite(dev, '#');
	if(!ReadContinueSec(dev, buf, 2, 20))	return 0;	// "*"+CR
	if(buf[0] != '*')	return 0;
	if(buf[1] != 13)	return 0;

	PlcDeviceWrite(dev, 'R');
	PlcDeviceWrite(dev, '3');
	PlcDeviceWrite(dev, '0');
	PlcDeviceWrite(dev, '2');
	PlcDeviceWrite(dev, 13);		// CR

	PlcDeviceWrite(dev, '@');
	if(!ReadContinueSec(dev, buf, 2, 3))	return 0;	// "&"
	if(buf[0] != '&')	return 0;
	if(buf[1] != 13)	return 0;

	return 1;	// ok reset WR3380
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadWR3380(LOCAL_PORT_STRUCT *pt, int pos)
{
	static char reset_flag = OFF;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i, j;
	int  	 count;
	char imsi[10];
	int read_tu;
	BYTE change;
	static int error_count = 0;
	TimeOutClass timeout;

	/*
	if(reset_flag == OFF) {
		HCURSOR hCursorOld;
		int retn;
		char message[80];

		hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));
		sprintf(message, "Port %d번의 National WR3380-82 통신 모듈 초기화 중...", pt->no);
		MessageDisplay(message);
		retn = ResetWR3380(pt->no);
		MessageDisplayHide();
		SetCursor(hCursorOld);

		if(retn == 0)	return COMMUNICATION_ERR_RESET;
		reset_flag = ON;

		return COMMUNICATION_OK;
	}
	*/

	PlcDeviceClear(&pt->device);

	read_tu = sm->size;
	if(read_tu > 4)	read_tu = 4;
	if(read_tu < 1)	read_tu = 1;

	pt->commSendBuf[0] = 'W';	// Request of relay status data in area
	pt->commSendBuf[1] = '2';	// Request of relay status in area
	sprintf(imsi, "%02X", sm->address+0x80);
	pt->commSendBuf[2] = imsi[0];  	// T/U Start High ch
	pt->commSendBuf[3] = imsi[1];		// T/U start Low  ch
	sprintf(imsi, "%02X", sm->address+0x80+read_tu-1);
	pt->commSendBuf[4] = imsi[0];  	// T/U END High ch
	pt->commSendBuf[5] = imsi[1];		// T/U END Low  ch
	pt->commSendBuf[6] = 13;				// CR

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 7);

	pt->commCountNeed = 7+read_tu*4;	// "M2"+start(H)+start(L)+end(H)+end(L)+data...+CR
	pt->commCountSend = 7;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			//if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[0] != 'M')	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != '2')	return COMMUNICATION_CODE_BAD;

			if(sm->target >= pt->nBufSizeWORD) {

			}
			else {
				int address = sm->target;

				for(i = 0; i < read_tu; i++) {
					for(j = 0; j < 4; j++) {
						change = pt->commRecvBuf[6+i*4+j];
						if(change >= 'A' && change <= 'F')
							change = change-'A'+10;
						else
							change = change-'0';
						if(change & 0x08) {
							PokeWORD(pt, address, PeekValueWORD(pt,address) | WORD_MASK[i*4+j]);
						}
						else {
							PokeWORD(pt, address, PeekValueWORD(pt,address) & (0xFFFF-WORD_MASK[i*4+j]));
						}
					}
				}
			}

         error_count = 0;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_READ)) {
			error_count++;
			if((reset_flag == OFF) ||
				(reset_flag == ON && error_count > 10)) {

				error_count = 0;

				HCURSOR hCursorOld;
				int retn;
				char message[80];

				hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));
				sprintf(message, "Port %d번의 National WR3380-82 통신 모듈 초기화 중...", pt->no);
				MessageDisplay(message);
				retn = ResetWR3380(&pt->device);
//				MessageDisplayHide();
				SetCursor(hCursorOld);

				if(retn == 0)	return COMMUNICATION_ERR_RESET;
				reset_flag = ON;
				return COMMUNICATION_OK;
			}
			return COMMUNICATION_TIME_OUT;
		}
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitWR3380(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD address, WORD flag)
{
	BYTE mask = 0;
	char imsi[10];

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 'T';	// STX
	pt->commSendBuf[1] = '1';
	sprintf(imsi, "%02X", address/16+0x80);
	pt->commSendBuf[2] = imsi[0];  	// T/U Start High ch
	pt->commSendBuf[3] = imsi[1];		// T/U start Low  ch

	if(flag == 1) 		mask |= 0x08;
	else					mask |= 0x04;

	mask |= (address%16)%4;

	sprintf(imsi, "%01X", mask);
	pt->commSendBuf[4] = imsi[0];  	// T/U END High ch
	pt->commSendBuf[5] = 13;				// CR

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 6);

	pt->commCountNeed = 0;	// STX+COMMAND+STATION+ADDRESS+CRC+ETX
	pt->commCountSend = 16;
	pt->commCountCurr = 0;

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordWR3380(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD /*address*/, WORD /*value*/)
{
	/*
	WORD 		crc = 0;
	struct 	time t;
	int  	 	count;
	int 	 	i;

	PlcDeviceClear(pt->no);

	pt->commSendBuf[0] = 2;	// STX
	wsprintf((char*)&pt->commSendBuf[1], "%02X", 0x10);	// read WORD command
	wsprintf((char*)&pt->commSendBuf[3], "%02X", station);
	wsprintf((char*)&pt->commSendBuf[5], "%04X", address);
	wsprintf((char*)&pt->commSendBuf[9], "%04X", value);		// data
	for(i = 1; i <= 12; i++) {
		crc += pt->commSendBuf[i];
	}
	wsprintf((char*)&pt->commSendBuf[13], "%04X", crc);
	pt->commSendBuf[17] = 3;	// ETX

	PlcDeviceWriteContinue(pt->no, (char*)pt->commSendBuf, 16);

	pt->commCountNeed = 14;	// STX+COMMAND+STATION+ADDRESS+CRC+ETX
	pt->commCountSend = 18;
	pt->commCountCurr = 0;
	cCommSecCurr = 0;
	gettime(&t);
	cCommOldSec = t.ti_sec;

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(pt->no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 에러 코드가 들어왔는지를 검사한다.
		if(pt->commCountCurr >= 12 &&
			pt->commRecvBuf[1] == 'F' &&
			pt->commRecvBuf[2] == 'F') {
			if(!CheckCRC(pt->commRecvBuf, 12))	return COMMUNICATION_CODE_BAD;
			return GetErrorCode(&pt->commRecvBuf[3]);
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			return COMMUNICATION_OK;
		}

		if(CheckTimeOut(MAX_TIME_OUT))	return COMMUNICATION_TIME_OUT;
	}
	*/

	return COMMUNICATION_PROGRAMM_NOT_MAKED;
}



