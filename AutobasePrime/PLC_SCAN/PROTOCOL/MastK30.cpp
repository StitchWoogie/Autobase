//------------------------------------------------------------------------------
//	MASTER-K30/50 Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
//  commmain.lib 파일을 함께 링크한다.
//	Mast-K30/50과 프로토콜은 같다. 절대영역의 크기가 다르다.
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


void PlcScanDrawMethodTitleMasterK30(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (Master-K30/50)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodMasterK30(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

//----------------------------------------------------------------------------
//	Master500K PLC의 기본 디바이스 인가를 검사한다.
//----------------------------------------------------------------------------

static WORD CalcAddress(char *type, WORD org_address)
{
	WORD address;

	if	 (strcmp(type, "HSC") == 0)	address = 0x8C00+org_address;
	else if(strcmp(type, "M") == 0)	address = 0x9000+(org_address%16)+((org_address/16)*0x100);
	else if(strcmp(type, "K") == 0)	address = 0x9400+(org_address%16)+((org_address/16)*0x100);
	else if(strcmp(type, "T") == 0)	address = 0x9600+org_address;
	else if(strcmp(type, "C") == 0)	address = 0x9700+org_address;
	else if(strcmp(type, "F") == 0)	address = 0x9800+org_address;
	else if(strcmp(type, "P") == 0) address = 0x9900+org_address;
	else if(strcmp(type, "D") == 0) address = 0x9A00+org_address;
	else if(strcmp(type, "TR") == 0)address = 0x9B00+org_address;
	else if(strcmp(type, "CR") == 0)address = 0x9C00+org_address;
	else if(strcmp(type, "S") == 0) address = 0x9D00+org_address;
	else if(strcmp(type, "TS") == 0)address = 0x9E00+org_address;
	else if(strcmp(type, "CS") == 0)address = 0x9F00+org_address;
	else {
		address = org_address;	// 직접 address 사용
	}

	return address;
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadMasterK30(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  count;
	BYTE crc;
	int  buf_pos;

	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		buf_pos = 0;
		
		pt->commSendBuf[buf_pos] = ENQ;
		buf_pos += 1;

		pt->commSendBuf[buf_pos] = STX;
		buf_pos += 1;														
		
		wsprintf((char*)&pt->commSendBuf[buf_pos], "g");				// r- read bit Command With BCC
		buf_pos += 1;														

		// 절대 Address설정
		wsprintf((char*)&pt->commSendBuf[buf_pos], "%04X", CalcAddress(sm->type, (WORD)sm->address*2));
		buf_pos += 4;

		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", sm->size*2);
		buf_pos += 2;

		pt->commSendBuf[buf_pos] = EOT;
		buf_pos += 1;

		crc = GetCRC_SUM8(&pt->commSendBuf[2], buf_pos-2);

		wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
		buf_pos += 2;

		//PlcDeviceWriteContinue(pt->no, (char*)&pt->commSendBuf[0], 1);
		//Sleep(100);
		PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], buf_pos);


		DisplaySendCodeNextLine(pt->no);

		pt->commCountNeed = 6+sm->size*4;	// ACK+st(2)+r+..+EOT+BCC(2)
		pt->commCountSend = buf_pos;
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))		return COMMUNICATION_TIME_OUT;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

		if(pt->commRecvBuf[0] == NAK) {
			PlcScanSetErrorString("MASTER-K30/50 에서 NAK 신호 반송");
			return COMMUNICATION_ERR_STRING;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[1] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[2] != STX) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[3], sm->size*4+1) != HexBufToBYTE((char*)&pt->commRecvBuf[sm->size*4+4])) {
				return COMMUNICATION_CODE_BAD;
			}
			
			int address = sm->target;
			WORD value;

			for(i = 0; i < sm->size; i++) {
				value = MAKEWORD(HexBufToBYTE((char*)&pt->commRecvBuf[3+i*4]), HexBufToBYTE((char*)&pt->commRecvBuf[5+i*4]));
				PokeValue(pt, sm, address+i, value);
			}
			
			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	한워드를 읽는다.
//------------------------------------------------------------------------------

static int ReadWord(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD &value, char *device)
{
	int  count;
	BYTE crc;
	int  buf_pos;
	int  word_size = 1;

	PlcDeviceClear(&pt->device);

	buf_pos = 0;
	
	pt->commSendBuf[buf_pos] = ENQ;
	buf_pos += 1;

	pt->commSendBuf[buf_pos] = STX;
	buf_pos += 1;														
	
	wsprintf((char*)&pt->commSendBuf[buf_pos], "g");				// r- read bit Command With BCC
	buf_pos += 1;														

	// 절대 Address설정
	wsprintf((char*)&pt->commSendBuf[buf_pos], "%04X", CalcAddress(device, address*2));
	buf_pos += 4;

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", word_size*2);
	buf_pos += 2;

	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(&pt->commSendBuf[2], buf_pos-2);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	//PlcDeviceWriteContinue(pt->no, (char*)&pt->commSendBuf[0], 1);
	//Sleep(100);
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], buf_pos);


	DisplaySendCodeNextLine(pt->no);

	pt->commCountNeed = 6+word_size*4;	// ACK+st(2)+r+..+EOT+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	pt->timeout->Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))		return COMMUNICATION_TIME_OUT;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		if(pt->commRecvBuf[0] == NAK) {
			PlcScanSetErrorString("MASTER-K30/50 에서 NAK 신호 반송");
			return COMMUNICATION_ERR_STRING;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[1] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[2] != STX) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[3], word_size*4+1) != HexBufToBYTE((char*)&pt->commRecvBuf[word_size*4+4])) {
				return COMMUNICATION_CODE_BAD;
			}
			
			value = MAKEWORD(HexBufToBYTE((char*)&pt->commRecvBuf[3]), HexBufToBYTE((char*)&pt->commRecvBuf[5]));
			
			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------------
//	한워드를 쓴다.
//------------------------------------------------------------------------------------

static int WriteWord(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	int  	 count;
	TimeOutClass timeout;
	int    buf_pos;
	BYTE	 crc;

	PlcDeviceClear(&pt->device);

	pt->commSendBuf[0] = ENQ;
	buf_pos = 1;
	pt->commSendBuf[buf_pos] = STX;
	buf_pos += 1;
	wsprintf((char*)&pt->commSendBuf[buf_pos], "h");					// h- write bit with BCC
	buf_pos += 1;

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%04X", CalcAddress(device, address*2));	
	buf_pos += 4;

	sprintf((char*)&pt->commSendBuf[buf_pos], "%02X", 2);	// 2BYTE
	buf_pos += 2;

	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(&pt->commSendBuf[2], buf_pos-2);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

	pt->commCountNeed = 7;    // ACK+st(2)+h+EOT+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[1] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[2] != STX) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[3], 2) != HexBufToBYTE((char*)&pt->commRecvBuf[5])) {
				return COMMUNICATION_CODE_BAD;
			}
			break;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}

	pt->commSendBuf[0] = ENQ;
	buf_pos = 1;
	pt->commSendBuf[buf_pos] = STX;
	buf_pos += 1;

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X%02X", LOBYTE(value), HIBYTE(value));
	buf_pos += 4;

	pt->commSendBuf[buf_pos] = EOT;
	buf_pos += 1;

	crc = GetCRC_SUM8(&pt->commSendBuf[2], buf_pos-2);

	wsprintf((char*)&pt->commSendBuf[buf_pos], "%02X", crc);
	buf_pos += 2;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, buf_pos);

	pt->commCountNeed = 7;    // ACK+st(2)+h+EOT+BCC(2)
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[1] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}
			if(pt->commRecvBuf[2] != STX) {
				return COMMUNICATION_CODE_BAD;
			}
			if(GetCRC_SUM8(&pt->commRecvBuf[3], 2) != HexBufToBYTE((char*)&pt->commRecvBuf[5])) {
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitMasterK30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	WORD value;
	int  retn;

	retn = ReadWord(pt, station, address/16, value, device);

	if(retn != COMMUNICATION_OK)	return retn;

	if(flag)	value |= WORD_MASK[address%16];
	else		value &= (0xFFFF - WORD_MASK[address%16]);

	return WriteWord(pt, station, address/16, value, device);
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordMasterK30(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	return WriteWord(pt, station, address, value, device);
}





