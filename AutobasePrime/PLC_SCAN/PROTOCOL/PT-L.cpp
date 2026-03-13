//------------------------------------------------------------------------------
//	PT_L Mitsubishi Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//
// MELSEC에서는 많은 종류의 프로토콜이 있으나 여기서는 제어형식 1을
// 지원하므로 통신 모듈의 모드 SW를 제어 형식 1로 반드시 맞추도록 한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <totaldef.h>
#include <tools.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
//#include "alarm.h"
#include "pro_main.h"

void PlcScanDrawMethodTitlePT_L(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (PT_L)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodPT_L(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
//------------------------------------------------------------------------------

int PlcScanReadPT_L(LOCAL_PORT_STRUCT *pt, int pos)
{
	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitPT_L(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  	 count;
	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	if(strcmp(device, "PWR") == 0) {
		if(flag == ON) {
			pt->commSendBuf[0] = STX;
			strncpy((char*)&pt->commSendBuf[1], "PON", 3);
			pt->commSendBuf[4] = ETX;
			pt->commCountSend = 5;
		}
		else {
			pt->commSendBuf[0] = STX;
			strncpy((char*)&pt->commSendBuf[1], "POF", 3);
			pt->commSendBuf[4] = ETX;
			pt->commCountSend = 5;
		}
	}
	else if(strcmp(device, "AMT") == 0) {
		pt->commSendBuf[0] = STX;
		strncpy((char*)&pt->commSendBuf[1], device, 3);
		pt->commSendBuf[4] = ':';
		pt->commSendBuf[5] = '0'+flag;
		pt->commSendBuf[6] = ETX;
		
		pt->commCountSend = 7;
	}
	else if(strcmp(device, "VID") == 0 ||
		     strcmp(device, "RG1") == 0 ||
			  strcmp(device, "RG2") == 0 ) {
		pt->commSendBuf[0] = STX;
		strncpy((char*)&pt->commSendBuf[1], "IIS", 3);
		pt->commSendBuf[4] = ':';
		strncpy((char*)&pt->commSendBuf[5], device, 3);
		pt->commSendBuf[8] = ETX;
		
		pt->commCountSend = 9;
	}
	else {
		char message[160];
		sprintf(message, "DO.TAG 에 설정된 extra1 [%s]은 PT-L 계기에 없는 영역입니다.",
								device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = pt->commCountSend;	// STX+STATION(2)+PCnum(2)
	pt->commCountCurr = 0;

	//return COMMUNICATION_OK;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		if(pt->commCountCurr >= 3 && strncmp((char*)&pt->commRecvBuf[2], "ER", 2) == 0) {
			return COMMUNICATION_CODE_BAD;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			//if(pt->commRecvBuf[0] != ACK)
			//	return COMMUNICATION_CODE_BAD;
			//if(strncmp((char*)&pt->commSendBuf[1], (char*)&pt->commRecvBuf[1], 4) != 0)
			//	return COMMUNICATION_CODE_BAD;

			if(pt->no < nPortHap) {
				//LOCAL_PORT_STRUCT *pt = &portBuf[pt->no].local;

				if(pt->nBufSizeWORD > 10) {
					if(strcmp(device, "PWR") == 0) {
						PokeWORD(pt, 0, flag);
					}
					else if(strcmp(device, "AMT") == 0) {
						PokeWORD(pt, 1, flag);
					}
					else if(strcmp(device, "VID") == 0) {
						PokeWORD(pt, 2, 0x0001);
					}
					else if(strcmp(device, "RG1") == 0) {
						PokeWORD(pt, 2, 0x0002);
					}
					else if(strcmp(device, "RG2") == 0) {
						PokeWORD(pt, 2, 0x0004);
					}
				}
			}

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordPT_L(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	int  	 count;
	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	if(strcmp(device, "AVL") == 0) {
		pt->commSendBuf[0] = STX;
		strncpy((char*)&pt->commSendBuf[1], device, 3);
		pt->commSendBuf[4] = ':';
		sprintf((char*)&pt->commSendBuf[5], "%03d", value);
		pt->commSendBuf[8] = ETX;
		
		pt->commCountSend = 9;
	}
	else {
		char message[160];
		sprintf(message, "AO.TAG 에 설정된 extra1 [%s]은 PT-L 계기에 없는 영역입니다.",
								device);
		MessageDisplay(message);

		return COMMUNICATION_OK;
	}

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = pt->commCountSend;	// STX+STATION(2)+PCnum(2)
	pt->commCountCurr = 0;

	//return COMMUNICATION_OK;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		if(pt->commCountCurr >= 3 && strncmp((char*)&pt->commRecvBuf[2], "ER", 2) == 0) {
			return COMMUNICATION_CODE_BAD;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			// if(pt->commRecvBuf[0] != ACK)
			//	return COMMUNICATION_CODE_BAD;
			// if(strncmp((char*)&pt->commSendBuf[1], (char*)&pt->commRecvBuf[1], 4) != 0)
			//	return COMMUNICATION_CODE_BAD;
			if(pt->no < nPortHap) {
				//LOCAL_PORT_STRUCT *pt = &portBuf[pt->no].local;

				PokeWORD(pt, 10, value);
			}

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





