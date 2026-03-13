// english O.K
//------------------------------------------------------------------------------
//	S-CON DPM 2 interface
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
#include <totaldef.h>
#include <crc.hpp>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleDPM2(HDC hdc, int x, int y)
{
	char *string = "station, type(AI,DI,CT), card address, buf address";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodDPM2(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

int PlcScanReadDPM2(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;

	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = 0x02;	// STX
		pt->commSendBuf[1] = 7; 		// size (국번부터 CheckSum까지의 BYTE수)
		pt->commSendBuf[2] = (BYTE)sm->station;	// 0x00~0xFF
		pt->commSendBuf[3] = (BYTE)sm->address;	// card address (0x00~0xFE)
		pt->commSendBuf[4] = 0x4F;			// read command
		if(strcmp(sm->type, "AI") == 0) {
			pt->commSendBuf[5] = 0x2C;			//	AI(2) all channel (C)
		}
		else if(strcmp(sm->type, "DI") == 0) {
			pt->commSendBuf[5] = 0x0C;			//	DI(0) all channel (C)
		}
		else {
			pt->commSendBuf[5] = 0x4C;			//	COUNT(4) all channel (C)
		}
		pt->commSendBuf[6] = (BYTE)sm->address;	// card channel num (모든 채널을 읽을때는 해당 채널 번호)
		pt->commSendBuf[7] = 0x03;			// ETX
		pt->commSendBuf[8] = GetCRC_SUM8(&pt->commSendBuf[2], 6);	// crc

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 9);

		if(strcmp(sm->type, "AI") == 0 ||
			strcmp(sm->type, "DI") == 0) {
			pt->commCountNeed = 9+32;	// STX+size+st+addr+r/w+command+ch+...+ETX+CRC
		}
		else {		// count card
			pt->commCountNeed = 9+24;	// STX+size+st+addr+r/w+command+ch+...+ETX+CRC
		}
		pt->commCountSend = 9;
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

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
      	if(pt->commRecvBuf[0] != STX)	return COMMUNICATION_CODE_BAD;

			BYTE crc;

			crc = GetCRC_SUM8(&pt->commRecvBuf[2], pt->commCountNeed-3);
			if(crc != (BYTE)pt->commRecvBuf[pt->commCountNeed-1]) {
				return COMMUNICATION_CODE_BAD;
			}

			if(strcmp(sm->type, "AI") == 0) {
				if(sm->target+16 >= pt->nBufSizeWORD) {

				}
				else {
					int address = sm->target;

					for(i = 0; i < 16; i++) {
						PokeValue(pt, sm, address+i, (BYTE)pt->commRecvBuf[7+i*2+0]+(BYTE)pt->commRecvBuf[7+i*2+1]*256);
					}
				}
			}
			else if(strcmp(sm->type, "DI") == 0) {
				int address = sm->target;

				PokeWORD(pt, address, 0);
				for(i = 0; i < 16; i++) {
					if(pt->commRecvBuf[7+i*2+0] == 0xFF) {
						PokeWORD(pt, address, PeekValueWORD(pt, address) | WORD_MASK[i]);
					}
				}
			}
			else {		// counter
				if(sm->target+12 >= pt->nBufSizeWORD) {

				}
				else {
					int address = sm->target;

					for(i = 0; i < 12; i++) {
						PokeWORD(pt, address+i, pt->commRecvBuf[7+i*2+0]+(BYTE)pt->commRecvBuf[7+i*2+1]*256);
					}
				}
			}
			return COMMUNICATION_OK;
		}
	}
}

// 한 워드를 통신을 통해 쓴다.

static int DPM2WriteWord(LOCAL_PORT_STRUCT *pt, int station, WORD address, char *type, WORD channel, WORD value)
{
	int count;
	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 0x02;				// STX;
	pt->commSendBuf[1] = 0x09;				// data size (st~CRC)
	pt->commSendBuf[2] = station;

	pt->commSendBuf[3] = (BYTE)address;	// DPM card address

	pt->commSendBuf[4] = 0x47;				// write command

	if(strcmp(type, "DO") == 0) 			pt->commSendBuf[5] = 0x1B;
	else if(strcmp(type, "AO") == 0) 	pt->commSendBuf[5] = 0x3B;
	else                          		pt->commSendBuf[5] = 0x4B;	// CNT card

	pt->commSendBuf[6] = (BYTE)channel;			// card channel
	pt->commSendBuf[7] = LOBYTE(value);
	pt->commSendBuf[8] = HIBYTE(value);
	pt->commSendBuf[9] = 0x03;
	pt->commSendBuf[10] = GetCRC_SUM8(&pt->commSendBuf[2], 8);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 11);

	pt->commCountNeed = 3;	// ADDRESS+Function+ADDR(2)+DATA(2)+CRC(2)
	pt->commCountSend = 11;
	pt->commCountCurr = 0;

   timeout.Reset();

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
// 한 워드를 통신을 통해 쓴다.
// S-800S에 COMMAND를 송신할때.
//------------------------------------------------------------------------------

static int DPM2WriteWordMosaicCommand(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD command2, WORD value)
{
	TimeOutClass timeout;
	int count;
	
	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 0x02;				// STX;
	pt->commSendBuf[1] = 0x08;				// data size (st~CRC)
	pt->commSendBuf[2] = station;

	pt->commSendBuf[3] = (BYTE)address;	// DPM card address

	pt->commSendBuf[4] = 0x47;				// write command

	pt->commSendBuf[5] = 0x5B;				// 5-Card type : Serial Output,  B-Mosaic Command Setting의 경우

	pt->commSendBuf[6] = (BYTE)command2;	// 1 - 소수점 변경
												// 2 - Polarity 변경
												// 4 - Address변경

	pt->commSendBuf[7] = (BYTE)value;
	pt->commSendBuf[8] = 0x03; 				// ETX
	pt->commSendBuf[9] = GetCRC_SUM8(&pt->commSendBuf[2], 7);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 10);

	pt->commCountNeed = 3;	
	pt->commCountSend = 10;
	pt->commCountCurr = 0;

   timeout.Reset();

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
// 한 워드를 통신을 통해 쓴다.
// S-800S에 Display Data를 송신할때.
//------------------------------------------------------------------------------

static int DPM2WriteWordMosaicDisplay(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD command2, float value)
{
	TimeOutClass timeout;
	int i, j;
	int count;
	
	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = 0x02;				// STX;
	pt->commSendBuf[1] = 0x09;				// data size (st~CRC)
	pt->commSendBuf[2] = station;

	pt->commSendBuf[3] = (BYTE)address;	// DPM card address

	pt->commSendBuf[4] = 0x47;				// write command

	pt->commSendBuf[5] = 0x5D;				// 5-Card type : Serial Output,  B-Mosaic Display Data의 경우

	char buf[20];

	sprintf(buf, "%6.0f", value);

	// 디스프레이 될 수 없는 숫자는 F로 표시한다.
	for(i = 0; i < 6; i++) {
		if(buf[i] == 0x20)	buf[i] = 'F';	// 디스프레이 될수 없는 숫자.
		if(buf[i] == '-')		buf[i] = 'F';	// 디스프레이 될수 없는 숫자.
	}

	// DISPLAY Card 특성상 1.0 이하의 숫자는 표시될 수 없으므로 맨앞에 0을 삽입한다.
	for(i = 0, j = 5; i <= command2 && i < 6; i++, j--) {
		if(buf[j] == 'F') {
			buf[j] = '0';
			break;
		}
	}

	pt->commSendBuf[6] = HexBufToBYTE(&buf[0]);
	pt->commSendBuf[7] = HexBufToBYTE(&buf[2]);
	pt->commSendBuf[8] = HexBufToBYTE(&buf[4]);

	pt->commSendBuf[9] = 0x03; 				// ETX
	pt->commSendBuf[10] = GetCRC_SUM8(&pt->commSendBuf[2], 8);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 11);

	pt->commCountNeed = 3;
	pt->commCountSend = 11;
	pt->commCountCurr = 0;

	timeout.Reset();

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//	하나의 디지털카드에 16개의 채널이 사용된다.
// ON 일때는 0x00FF, OFF 일때는 0x0000을 주소/채널 에 쓰면 된다.
//------------------------------------------------------------------------------

int PlcScanWriteBitDPM2(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag)
{
	return DPM2WriteWord(pt, station, address/16, "DO", address%16, flag ? 0x00FF : 0x0000);
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordDPM2(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *type, WORD channel)
{
	// type이 AO, CT의 두 가지가 있다.
	if(strcmp(type, "CMD") == 0) {
		return DPM2WriteWordMosaicCommand(pt, station, address, channel, (WORD)value);
	}
	else if(strcmp(type, "DISP") == 0) {
		return DPM2WriteWordMosaicDisplay(pt, station, address, channel, value);
	}
	else {
		return DPM2WriteWord(pt, station, address, type, channel, (WORD)value);
	}
}



