// english O.K
//------------------------------------------------------------------------------
//	SAMSUNG BRAIN SPC-300 Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <tools.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleSpc300(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (SPC-300)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodSpc300(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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
// Polynomal = 1
//------------------------------------------------------------------------------

static WORD GetCRC16_1(BYTE *buf, int size)
{
	WORD crc = 0xFFFF;
	int i, j;

	for(i = 0; i < size; i++) {
		crc = crc ^ (buf[i] & 0x00FF);
		for(j = 0; j < 8; j++) {
			if((crc & 0x0001) == 0x0001) crc = (crc>>1) ^ 0xA001;
			else crc = crc >> 1;
		}
	}

	return crc;
}

static int GetReadWordCount(SCAN_METHOD_STRUCT *sm)
{
	if(strcmp(sm->type, "AI") == 0)
		return 2;
	else
		return sm->size;	
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadSpc300(LOCAL_PORT_STRUCT *pt, int pos)
{
	WORD crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
	// TimeOutClass timeout;
	// int	read_word;

	//---------------------------------------------------------------------------
	// AI는 4개 채널이 하나의 카드로 되어있다.
	// 4개의 채널은 값은 어드레스를 공유한다. (2워드)
	// 하나의 채널은 값(WORD)+채널(WORD)로 되어있다.
	// 채널은 PC에서 설정할 수 있는것이 아니라, PLC 프로그램에서 채널값을 0~3까지
	// 반복적으로 바꾸어야 한다.
	// PC에서 값을 읽었을 때 채널값을 참조하여 적당한 위치에 삽입하도록 한다.
	// 0-4번째채널, 1-3번째채널, 2-2번째 채널, 3-1번째 채널
	//--------------------------------------------------------------------------

	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		pt->commSendBuf[0] = (BYTE)sm->station;
		pt->commSendBuf[1] = 191;	// computer address
		pt->commSendBuf[2] = 3;		// function code : read word
		pt->commSendBuf[3] = 3;		// information length : always 3 byte
		pt->commSendBuf[4] = LOBYTE(sm->address);
		pt->commSendBuf[5] = HIBYTE(sm->address);
		pt->commSendBuf[6] = GetReadWordCount(sm);

		crc =GetCRC16_1(pt->commSendBuf, 7);

		pt->commSendBuf[7] = LOBYTE(crc);
		pt->commSendBuf[8] = HIBYTE(crc);

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 9);

		// query ack code
		pt->commCountNeed = 7;	//
		pt->commCountSend = 9;
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

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(GetCRC16_1(pt->commRecvBuf, 5) != pt->commRecvBuf[5]+pt->commRecvBuf[6]*256u)
				return COMMUNICATION_CODE_BAD;

			break;
			//return COMMUNICATION_OK;
		}
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = (BYTE)sm->station;
	pt->commSendBuf[1] = 191;	// computer address
	pt->commSendBuf[2] = 0;		// function code : read word
	pt->commSendBuf[3] = 1;		// information length : always 3 byte
	pt->commSendBuf[4] = 0;

	crc =GetCRC16_1(pt->commSendBuf, 5);

	pt->commSendBuf[5] = LOBYTE(crc);
	pt->commSendBuf[6] = HIBYTE(crc);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 7);

	pt->commCountNeed = 6+GetReadWordCount(sm)*2;	// DA+SA+0x83+L+sm->size*2+CRC(2byte)
	pt->commCountSend = 7;
	pt->commCountCurr = 0;

	pt->timeout->Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(GetCRC16_1(pt->commRecvBuf, pt->commCountNeed-2) != pt->commRecvBuf[pt->commCountNeed-2]+pt->commRecvBuf[pt->commCountNeed-1]*256u)
				return COMMUNICATION_CODE_BAD;
			//if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))	return COMMUNICATION_CODE_BAD;

			if(strcmp(sm->type, "AI") == 0) {
				int address = sm->target;
				int ai_num = pt->commRecvBuf[6]+pt->commRecvBuf[7]*256u;

				ai_num %= 4;

				ai_num = (3-abs(ai_num));

				PokeValue(pt, sm, address+ai_num, pt->commRecvBuf[4]+pt->commRecvBuf[5]*256u);
			}
			else {
				int address = sm->target;

				for(i = 0; i < sm->size; i++) {
					PokeValue(pt, sm, address+i, pt->commRecvBuf[4+i*2]+pt->commRecvBuf[5+i*2]*256u);
				}
			}
			return COMMUNICATION_OK;
		}

		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitSpc300(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = station;
	pt->commSendBuf[1] = 191;	// computer address
	pt->commSendBuf[2] = 2;		// function code : write bit
	pt->commSendBuf[3] = 3;		// byte size L = N*2+2
	pt->commSendBuf[4] = LOBYTE(address);
	pt->commSendBuf[5] = HIBYTE(address);
	pt->commSendBuf[6] = (BYTE) flag;

	crc = GetCRC16_1(pt->commSendBuf, 7);

	pt->commSendBuf[7] = LOBYTE(crc);
	pt->commSendBuf[8] = HIBYTE(crc);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 9);

	// query ack code
	pt->commCountNeed = 7;		// DA+SA+0x80+0x01+0x00+CRC(2)
	pt->commCountSend = 9;	//	DA+SA+0x04+L+BASE(LO)+BASE(HI)+VALUE(2)+CRC(2)
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(GetCRC16_1(pt->commRecvBuf, 5) != pt->commRecvBuf[5]+pt->commRecvBuf[6]*256u)
				return COMMUNICATION_CODE_BAD;

			break;
			// return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = station;
	pt->commSendBuf[1] = 191;	// computer address
	pt->commSendBuf[2] = 0;		// always 0
	pt->commSendBuf[3] = 1;		// always 1
	pt->commSendBuf[4] = 0;     // always 0

	crc =GetCRC16_1(pt->commSendBuf, 5);

	pt->commSendBuf[5] = LOBYTE(crc);
	pt->commSendBuf[6] = HIBYTE(crc);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 7);

	pt->commCountNeed = 7;	// DA+SA+0x84+1+0+CRC(2byte)
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
			if(GetCRC16_1(pt->commRecvBuf, pt->commCountNeed-2) != pt->commRecvBuf[pt->commCountNeed-2]+pt->commRecvBuf[pt->commCountNeed-1]*256u)
				return COMMUNICATION_CODE_BAD;
			//if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))	return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordSpc300(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value)
{
	WORD crc = 0;
	int  	 count;
	TimeOutClass timeout;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = station;
	pt->commSendBuf[1] = 191;	// computer address
	pt->commSendBuf[2] = 4;		// function code : write word
	pt->commSendBuf[3] = 2+2;	// byte size L = N*2+2
	pt->commSendBuf[4] = LOBYTE(address);
	pt->commSendBuf[5] = HIBYTE(address);
	pt->commSendBuf[6] = LOBYTE(value);
	pt->commSendBuf[7] = HIBYTE(value);

	crc =GetCRC16_1(pt->commSendBuf, 8);

	pt->commSendBuf[8] = LOBYTE(crc);
	pt->commSendBuf[9] = HIBYTE(crc);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 10);

	// query ack code
	pt->commCountNeed = 7;		// DA+SA+0x80+0x01+0x00+CRC(2)
	pt->commCountSend = 8+2;	//	DA+SA+0x04+L+BASE(LO)+BASE(HI)+VALUE(2)+CRC(2)
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(GetCRC16_1(pt->commRecvBuf, 5) != pt->commRecvBuf[5]+pt->commRecvBuf[6]*256u)
				return COMMUNICATION_CODE_BAD;

			break;
			//return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = station;
	pt->commSendBuf[1] = 191;	// computer address
	pt->commSendBuf[2] = 0;		// always 0
	pt->commSendBuf[3] = 1;		// always 1
	pt->commSendBuf[4] = 0;     // always 0

	crc =GetCRC16_1(pt->commSendBuf, 5);

	pt->commSendBuf[5] = LOBYTE(crc);
	pt->commSendBuf[6] = HIBYTE(crc);

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 7);

	pt->commCountNeed = 7;	// DA+SA+0x84+1+0+CRC(2byte)
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
			if(GetCRC16_1(pt->commRecvBuf, pt->commCountNeed-2) != pt->commRecvBuf[pt->commCountNeed-2]+pt->commRecvBuf[pt->commCountNeed-1]*256u)
				return COMMUNICATION_CODE_BAD;
			//if(!CheckCRC(pt->commRecvBuf, pt->commCountNeed))	return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}







