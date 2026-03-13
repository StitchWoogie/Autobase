//------------------------------------------------------------------------------
//	MF2 Protocol
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

#define	MAX_TIME_OUT		2

void PlcScanDrawMethodTitleMF2(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (MF2)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodMF2(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
// 통신을 읽으면 워드단위가 채널하나에 해당한다.
// BL-7000은 일반 아날로그와는 달리 상위 바이트는 상태를 나타내고,
// 하위 바이트는 실제 계측치를 나타낸다.
// BIT-15	0 - 통상
// BIT-14	0 - 측정중, 1 - 조정
// BIT-13	0 - 측정중, 1 - SKIP
// BIT-8~12 미정의	(실제로는 자국번호가 측정된다.)
// Low Byte 계측치 (0x32 ~ 0xFA)
//
// 통신 데이터값이 	"----" = 미 접속
//                   "****" = 자국 에러시
//							"??00" = trouble 시
//
//	AutoBase 에서는 "----"나 "****"을 메모리 내부로 읽어들일 수 없으므로
//	통신중 위 값을 받았을 때는 BIT-12, 11, 10을 각각 ON시켜서 AutoBase에서 알수 있도록
// 한다.
//------------------------------------------------------------------------------

int MF2ReadContinue(DEVICE_STRUCT *dev, char *buf, int size)
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

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(curr >= size) {
			return 1;
		}

		// check time out
		gettime(&t);
		if(t.ti_sec != sec_old) {
			sec_old = t.ti_sec;
			sec_curr++;
			if(sec_curr >= MAX_TIME_OUT) {
				return 0;		// 시간초과에 걸렸다.
			}
		}
	}
}

//------------------------------------------------------------------------------
// MF2 통신 모듈을 초기화 한다.
//------------------------------------------------------------------------------

static int ResetMF2(DEVICE_STRUCT *dev)
{
	char buf[10];
	int i;

	PlcDeviceClear(dev);

	// reset MF2
	PlcDeviceWriteContinue(dev, "RE", 2);
	PlcDeviceWrite(dev, CR);	// 0x0d
	if(!MF2ReadContinue(dev, buf, 8))	return 0;	// "RE,0000CR"

	WaitSec(4);		// wait 3 sec

	// set control
	for(i = 1; i <= 0x7F; i++) {
		PlcDeviceWriteContinue(dev, "SC,", 3);
		wsprintf(buf, "%02X", i);
		PlcDeviceWriteContinue(dev, buf, 2);
		PlcDeviceWriteContinue(dev, ",01,FF,FF", 9);
		PlcDeviceWrite(dev, CR);	// 0x0d

		if(!MF2ReadContinue(dev, buf, 8))		return 0; 	//	"SC,0000CR"
	}

	// enable
	PlcDeviceWriteContinue(dev, "SC,FF,1", 7);
	PlcDeviceWrite(dev, CR);	// 0x0d
	if(!MF2ReadContinue(dev, buf, 8))		return 0; 	//	"SC,0000CR"

	return 1;	// ok reset MF2
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadMF2(LOCAL_PORT_STRUCT *pt, int pos)
{
	static char reset_flag = OFF;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
	char imsi[10];
	TimeOutClass timeout;

	if(reset_flag == OFF) {
		HCURSOR hCursorOld;
		int retn;
		char message[80];

		hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));
		sprintf(message, "Port %d번의 MF2통신 모듈 초기화중...", pt->no);
		MessageDisplay(message);
		retn = ResetMF2(&pt->device);
		SetCursor(hCursorOld);

		if(retn == 0)	return COMMUNICATION_ERR_RESET;
		reset_flag = ON;

		return COMMUNICATION_OK;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 'R';		// plc address
	pt->commSendBuf[1] = 'D';		// my address
	pt->commSendBuf[2] = ',';  		// read word
	sprintf(imsi, "%02X", sm->address+1);
	pt->commSendBuf[3] = imsi[0];	// information length
	pt->commSendBuf[4] = imsi[1];	// information length
	pt->commSendBuf[5] = 0x0d;		// information length

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 6);

	pt->commCountNeed = 8;			// "RD,????CR"
	pt->commCountSend = 6;
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
			if(pt->commRecvBuf[0] != 'R')	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[1] != 'D')	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[2] != ',')	return COMMUNICATION_CODE_BAD;
			if(pt->commRecvBuf[7] != 0x0d)	return COMMUNICATION_CODE_BAD;

			if(sm->target >= pt->nBufSizeWORD) {

			}
			else {
				int address = sm->target;

				for(i = 0; i < 1; i++) {
					if(strncmp((char*)&pt->commRecvBuf[3+i*4], "----", 4) == 0) {	// 미 접속시
						PokeWORD(pt, address+i, WORD_MASK[12]);
					}
					else if(strncmp((char*)&pt->commRecvBuf[3+i*4], "****", 4) == 0) {	// 자국 에러
						PokeWORD(pt, address+i, WORD_MASK[11]);
					}
					else if(strncmp((char*)&pt->commRecvBuf[3+i*4+2], "00", 2) == 0) { // --00 -> Trouble 시
						PokeWORD(pt, address+i, WORD_MASK[10]);
					}
					else {
						WORD imsi = HexBufToWORD((char*)&pt->commRecvBuf[3+i*4]);
						imsi &= 0xE3FF;	// bit 12, 11, 10 off
						PokeWORD(pt, address+i, imsi);
					}
				}
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(MAX_TIME_OUT))	return COMMUNICATION_TIME_OUT;
	}

}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitMF2(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD /*address*/, WORD /*flag*/)
{
	/*
	WORD crc = 0;
	struct time t;
	int  	 count;
	int i;

	PlcDeviceClear(port_no);

	pt->commSendBuf[0] = 2;	// STX
	wsprintf((char*)&pt->commSendBuf[1], "%02X", 0x11);	// read WORD command
	wsprintf((char*)&pt->commSendBuf[3], "%02X", station);
	wsprintf((char*)&pt->commSendBuf[5], "%04X", address/16);
	wsprintf((char*)&pt->commSendBuf[9], "%01X",  address%16);
	wsprintf((char*)&pt->commSendBuf[10], "%01X", flag);
	for(i = 1; i <= 10; i++) {
		crc += pt->commSendBuf[i];
	}
	wsprintf((char*)&pt->commSendBuf[11], "%04X", crc);
	pt->commSendBuf[15] = 3;	// ETX

	PlcDeviceWriteContinue(port_no, (char*)pt->commSendBuf, 16);

	pt->commCountNeed = 14;	// STX+COMMAND+STATION+ADDRESS+CRC+ETX
	pt->commCountSend = 16;
	pt->commCountCurr = 0;
	cCommSecCurr = 0;
	gettime(&t);
	cCommOldSec = t.ti_sec;

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(port_no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

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

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordMF2(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD /*address*/, WORD /*value*/)
{
	/*
	WORD 		crc = 0;
	struct 	time t;
	int  	 	count;
	int 	 	i;

	PlcDeviceClear(port_no);

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

	PlcDeviceWriteContinue(port_no, (char*)pt->commSendBuf, 16);

	pt->commCountNeed = 14;	// STX+COMMAND+STATION+ADDRESS+CRC+ETX
	pt->commCountSend = 18;
	pt->commCountCurr = 0;
	cCommSecCurr = 0;
	gettime(&t);
	cCommOldSec = t.ti_sec;

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		  count = PlcDeviceReadContinue(port_no, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

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



