// english O.K
//------------------------------------------------------------------------------
//	Modicon interface
//	만들어진 lib 파일을 protocol main 과 링크 시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <tools.h>
#include <glib.h>
#include <crc.hpp>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleModicon(HDC hdc, int x, int y)
{
	char *string = "ADR, type, register address, buf address, word number";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodModicon(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

static void DisplayErrorCode(BYTE code)
{
	char message[160];

	switch(code) {
		case 0x01:
			sprintf(message, "Modicon return Error Code(%02Xh) - Illegal function", code);
			break;
		case 0x02:
			sprintf(message, "Modicon return Error Code(%02Xh) - Illegal data address", code);
			break;
		case 0x03:
			sprintf(message, "Modicon return Error Code(%02Xh) - Illegal data value", code);
			break;
		case 0x04:
			sprintf(message, "Modicon return Error Code(%02Xh) - Slave Device Failure", code);
			break;
		case 0x05:
			sprintf(message, "Modicon return Error Code(%02Xh) - Acknowledge", code);
			break;
		case 0x06:
			sprintf(message, "Modicon return Error Code(%02Xh) - Slave device busy", code);
			break;
		case 0x07:
			sprintf(message, "Modicon return Error Code(%02Xh) - Negative acknowledge", code);
			break;
		case 0x08:
			sprintf(message, "Modicon return Error Code(%02Xh) - memory parity error", code);
			break;
		default:
			if(IsLangKorean()) {
			sprintf(message, "Modecon return Error Code(%02Xh) - 새로운 오류번호 Modicon 책자 Exception codes 참고", code);
			}else {
			sprintf(message, "Modecon return Error Code(%02Xh) - New Error : See Exception codes of modicon manual.", code);
			}
			break;
	}

	MessageDisplay(message);
}

static int IsBitDevice(char *type)
{
	if(strcmp(type, "0") == 0 ||
		strcmp(type, "1") == 0) {
		return ON;
	}
	else {
		return OFF;
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadModicon(LOCAL_PORT_STRUCT *pt, int pos)
{
	WORD crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
	//char  bit_device = OFF;

	if(pt->bReadingFlag == OFF) {

		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = (BYTE)sm->station;				//sm->station;
		if(strcmp(sm->type, "0") == 0) {
			pt->commSendBuf[1] = 0x01;						// read holding register
		}
		else if(strcmp(sm->type, "1") == 0) {
			pt->commSendBuf[1] = 0x02;						// read holding register
		}
		else if(strcmp(sm->type, "3") == 0) {
			pt->commSendBuf[1] = 0x04;						// read holding register
		}
		else if(strcmp(sm->type, "4") == 0) {
			pt->commSendBuf[1] = 0x03;						// read holding register
		}
		else {
			MessageDisplay("Read Type must Only 0,1, 3,4");
			return COMMUNICATION_OK;
		}

		pt->commSendBuf[2] = HIBYTE(sm->address);	// start address to be read
		pt->commSendBuf[3] = LOBYTE(sm->address);

		if(IsBitDevice(sm->type)) {
			pt->commSendBuf[4] = HIBYTE(sm->size*16);		// number of register to be read
			pt->commSendBuf[5] = LOBYTE(sm->size*16);
		}
		else {
			pt->commSendBuf[4] = HIBYTE(sm->size);		// number of register to be read
			pt->commSendBuf[5] = LOBYTE(sm->size);
		}

		crc = GetCRC_16_15_2_1(pt->commSendBuf, 6);

		pt->commSendBuf[6] = HIBYTE(crc);		// CRC
		pt->commSendBuf[7] = LOBYTE(crc);

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 8);

		pt->commCountNeed = 5+sm->size*2;	// ADDRESS+Function+ByteNum+data(sm->size*2)+CRC(2)
		pt->commCountSend = 8;
		pt->commCountCurr = 0;

		pt->timeout->Reset();
	
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

		// 에러 발생시.
		if((pt->commCountCurr == 5) && (pt->commRecvBuf[1] & 0x80)) {
			DisplayErrorCode(pt->commRecvBuf[2]);
			return COMMUNICATION_OK;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			WORD crc;

			crc = GetCRC_16_15_13_1((BYTE*)pt->commRecvBuf, pt->commCountNeed-2);
			if(crc != (BYTE)pt->commRecvBuf[pt->commCountNeed-2]+pt->commRecvBuf[pt->commCountNeed-1]*256u) {
				return COMMUNICATION_CODE_BAD;
			}

			if(IsBitDevice(sm->type)) {
				int address = sm->target;
				for(i = 0; i < sm->size; i++) {
					PokeValue(pt, sm, address+i, pt->commRecvBuf[3+i*2+1]*256u+(BYTE)pt->commRecvBuf[3+i*2+0]);
				}
			}
			else {
				int address = sm->target;
				for(i = 0; i < sm->size; i++) {
					PokeValue(pt, sm, address+i, pt->commRecvBuf[3+i*2+0]*256u+(BYTE)pt->commRecvBuf[3+i*2+1]);
				}
			}

			return COMMUNICATION_OK;
		}
	}
}

// 한 워드를 통신을 통해 쓴다.

static int ModiconWriteWord(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = station;				// ISM address;
	if(strcmp(device, "0") == 0) {
		pt->commSendBuf[1] = 0x05;					// preset single register
	}
	else if(strcmp(device, "4") == 0) {
		pt->commSendBuf[1] = 0x06;					// preset single register
	}

	pt->commSendBuf[2] = HIBYTE(address);		// address of the Register to be Write
	pt->commSendBuf[3] = LOBYTE(address);

	pt->commSendBuf[4] = HIBYTE(value);		// dataword
	pt->commSendBuf[5] = LOBYTE(value);

	crc = GetCRC_16_15_2_1(pt->commSendBuf, 6);

	pt->commSendBuf[6] = HIBYTE(crc);			// CRC
	pt->commSendBuf[7] = LOBYTE(crc);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 8);

	pt->commCountNeed = 8;	// ADDRESS+Function+ADDR(2)+DATA(2)+CRC(2)
	pt->commCountSend = 8;
	pt->commCountCurr = 0;

	timeout.Reset();

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 에러 발생시.
		if((pt->commCountCurr == 5) && (pt->commRecvBuf[1] & 0x80)) {
			DisplayErrorCode(pt->commRecvBuf[2]);
			return COMMUNICATION_OK;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//	하나의 디지털이 하나의 워드를 사용하므로 WORD 쓰는 방식과 동일한다.
// ON 일때는 0xFFFF, OFF 일때는 0x0000을 주소에 쓰면 된다.
//------------------------------------------------------------------------------

int PlcScanWriteBitModicon(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	if(flag)
		return ModiconWriteWord(pt, station, (address/16)*10+(address%16)-1, 0xFF00, "0");
	else
		return ModiconWriteWord(pt, station, (address/16)*10+(address%16)-1, 0x0000, "0");
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordModicon(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device)
{
	return ModiconWriteWord(pt, station, address-1, (WORD)value, "4");
}



