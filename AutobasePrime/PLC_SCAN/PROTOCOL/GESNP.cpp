//------------------------------------------------------------------------------
//	GE interface
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

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

typedef struct {
	char com_status;
	char byte_count;	
	char bit_type;
	//char mail_count;
	WORD wait_time;
	WORD addr;
	WORD size;
	BOOL attach_write_flag;
	int  nTimerUseMax;
	int  nTimerUseCurr;
} LOCAL_PROTOCOL_STRUCT;

void PlcProtocolInitGeSnp(LOCAL_PORT_STRUCT *pt)
{
	WORD	i;
	CommaBlockString comma;
	char			 buf[50];

	pt->hLocalProtocol = GlobalAlloc(GMEM_MOVEABLE, sizeof(LOCAL_PROTOCOL_STRUCT));

	if(pt->hLocalProtocol == NULL)	return;

	LocalProtocolClass lclass(pt->hLocalProtocol);
	LOCAL_PROTOCOL_STRUCT *local = (LOCAL_PROTOCOL_STRUCT *)lclass.GetPoint();
	if(local == NULL)	return;
	memset(local, 0, sizeof(LOCAL_PROTOCOL_STRUCT));
	local->bit_type = 0;
	//local->mail_count = 0;
	local->wait_time = 5000;
	local->attach_write_flag = FALSE;
	local->nTimerUseCurr = 0;

	comma.Set(pt->sScanProtocolOption);
	comma.GetWORD(i);
	if(i >= 1000 && i <= 60000) local->wait_time = i;
	comma.GetWORD(i);
	if(i == 1) local->attach_write_flag = TRUE;
	comma.GetString(buf, 50);
	if(strncmp(buf, "bit", 3) == 0 || strncmp(buf, "BIT", 3) == 0) local->bit_type = 1;
	
	comma.GetInt(local->nTimerUseMax);
	if(local->nTimerUseMax < 0 || local->nTimerUseMax > 100) {
		local->nTimerUseMax = 1;	
	}
}

void PlcProtocolUnInitGeSnp(LOCAL_PORT_STRUCT *pt)
{
	if(pt->hLocalProtocol == NULL)	return;

	GlobalFree(pt->hLocalProtocol);
	pt->hLocalProtocol = NULL;
}


void PlcScanDrawMethodTitleGESNP(HDC hdc, int x, int y)
{
	char *string = "ADR, type, register address, buf address, word number";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodGESNP(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

static BYTE MakeCRC(BYTE *buf, int pos, int size)
{
	short crc = 0;
	int i;
		
	for(i = pos; i < size; i++) {
		crc ^= (short)buf[i];
		crc <<= 1;
		if (crc & 0x100) crc |= 0x01;		
	}
	return (crc&0xFF);
}


static BYTE GetDeviceType(char *type, BOOL byte)
{
	switch(byte) {
		case 1 :   // BYTE 
			if	 (strcmp(type, "R") == 0)		return 8;
			else if(strcmp(type, "AI") == 0)	return 10;
			else if(strcmp(type, "AQ") == 0)	return 12;			
			else if(strcmp(type, "I") == 0)		return 16;
			else if(strcmp(type, "Q") == 0)		return 18;
			else if(strcmp(type, "T") == 0)		return 20;
			else if(strcmp(type, "M") == 0)		return 22;
			else if(strcmp(type, "SA") == 0)	return 24;
			else if(strcmp(type, "SB") == 0)	return 26;
			else if(strcmp(type, "SC") == 0)	return 28;
			else if(strcmp(type, "S") == 0)		return 30;
			else if(strcmp(type, "G") == 0)		return 56;	
			else 								return 0;
		case 0 :			
			if(strcmp(type, "I") == 0)			return 70;
			else if(strcmp(type, "Q") == 0)		return 72;
			else if(strcmp(type, "T") == 0)		return 74;
			else if(strcmp(type, "M") == 0)		return 76;
			else if(strcmp(type, "SA") == 0)	return 78;
			else if(strcmp(type, "SB") == 0)	return 80;
			else if(strcmp(type, "SC") == 0)	return 82;			
			else if(strcmp(type, "G") == 0)		return 86;	
			else 								return 0;		
	}
	return 0;
}

static BYTE GetDeviceWord(char *type)
{
	if	 (strcmp(type, "R") == 0)		return 1;
	else if(strcmp(type, "AI") == 0)	return 1;
	else if(strcmp(type, "AQ") == 0)	return 1;		
	else								return 0;	
}


static void gefAttachMessage(LOCAL_PORT_STRUCT *pt)
{
	pt->commSendBuf[0] = 0x1B;
	pt->commSendBuf[1] = 0x41;
	memset((char*)&pt->commSendBuf[2], 0, 8);	
	pt->commSendBuf[10] = 0x30;
	pt->commSendBuf[11] = 0x35;//0x41;
	pt->commSendBuf[12] = 0x30;
	pt->commSendBuf[13] = 0x30;
	pt->commSendBuf[14] = 0x30;
	pt->commSendBuf[15] = 0x20;
	pt->commSendBuf[16] = 0x20;
	pt->commSendBuf[17] = 0x20;
	pt->commSendBuf[18] = 0x17;
	pt->commSendBuf[19] = 0x00;
	pt->commSendBuf[20] = 0x00;
	pt->commSendBuf[21] = 0x00;
	pt->commSendBuf[22] = 0x42;//0x31;
	pt->commSendBuf[23] = 0x39;//0x41;
	
	PlcDeviceSetCommBreak(&pt->device);
	Sleep(5);
	PlcDeviceClearCommBreak(&pt->device);	
	Sleep(50);
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], 24);	
}


static void gefAckMessage(LOCAL_PORT_STRUCT *pt)
{
	pt->commSendBuf[0] = 6;
	pt->commSendBuf[1] = 0;	
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], 2);
}


static void gefTimerMessage(LOCAL_PORT_STRUCT *pt, WORD wait_time)
{
	pt->commSendBuf[0] = 0x1B;
	pt->commSendBuf[1] = 0x4D;
	memset((char*)&pt->commSendBuf[2], 0, 6);
	pt->commSendBuf[8] = 0x00;		
	pt->commSendBuf[9] = 0xC0;
	pt->commSendBuf[10] = 0x10;
	pt->commSendBuf[11] = 0x3A;
	pt->commSendBuf[12] = 0x00;
	pt->commSendBuf[13] = 0x00;
	pt->commSendBuf[14] = 0x10;	
	pt->commSendBuf[15] = 0x3E;		
	pt->commSendBuf[16] = 0x00;
	pt->commSendBuf[17] = 0x00;
	pt->commSendBuf[18] = 0x01;
	pt->commSendBuf[19] = 0x01;
	pt->commSendBuf[20] = 0x00;	
	pt->commSendBuf[21] = 0x00; // 0x1388 = 5,000
	pt->commSendBuf[22] = (BYTE)wait_time;
	pt->commSendBuf[23] = (BYTE)(wait_time/256);	
	pt->commSendBuf[24] = 0xA8;
	pt->commSendBuf[25] = 0x61; // 0x61A8 = 25,000
	pt->commSendBuf[26] = 0xE8;
	pt->commSendBuf[27] = 0x03; // 0x03E8 = 1,000
	pt->commSendBuf[28] = 0x01;
	memset((char*)&pt->commSendBuf[29], 0, 5);	
	pt->commSendBuf[34] = 0x17;
	memset((char*)&pt->commSendBuf[35], 0, 4);	
	pt->commSendBuf[39] = MakeCRC((BYTE *)pt->commSendBuf, 0, 39);
		
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], 40);
}


static void gefEtcMessage(LOCAL_PORT_STRUCT *pt)
{
	pt->commSendBuf[0] = 0x1B;
	pt->commSendBuf[1] = 0x4D;
	memset((char*)&pt->commSendBuf[2], 0, 6);	
	pt->commSendBuf[8] = 0x0D;
	pt->commSendBuf[9] = 0xC0;
	pt->commSendBuf[10] = 0x10;
	pt->commSendBuf[11] = 0x3A;
	pt->commSendBuf[12] = 0x00;
	pt->commSendBuf[13] = 0x00;
	pt->commSendBuf[14] = 0x10;	
	pt->commSendBuf[15] = 0x0A;		
	pt->commSendBuf[16] = 0x00;
	pt->commSendBuf[17] = 0x00;
	pt->commSendBuf[18] = 0x01;
	pt->commSendBuf[19] = 0x01;
	pt->commSendBuf[20] = 0x21;	
	pt->commSendBuf[21] = 0xFF;	
	memset((char*)&pt->commSendBuf[22], 0, 12);	
	pt->commSendBuf[34] = 0x17;
	memset((char*)&pt->commSendBuf[35], 0, 4);
	pt->commSendBuf[39] = MakeCRC((BYTE *)pt->commSendBuf, 0, 39);
		
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], 40);
}

static void gefMailBoxMessage(LOCAL_PORT_STRUCT *pt, int pos)
{
	WORD		i;

	LocalProtocolClass lclass(pt->hLocalProtocol);
	LOCAL_PROTOCOL_STRUCT *local = (LOCAL_PROTOCOL_STRUCT *)lclass.GetPoint();
	if(local == NULL)	return;
	
	//if(local->mail_count == 1) {
	//	gefEtcMessage(pt);
	//	local->mail_count = 2;
	//	return;
	//}
	//if(local->mail_count == 0) {		
		//gefTimerMessage(pt, local->wait_time);
		//local->mail_count = 1;
		//return;
	//}

	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];	
	//local->mail_count = 3;
	
	pt->commSendBuf[0] = 0x1B;
	pt->commSendBuf[1] = 0x4D;
	memset((char*)&pt->commSendBuf[2], 0, 6);
	pt->commSendBuf[8] = 0x0C;
	pt->commSendBuf[9] = 0xC0;
	pt->commSendBuf[10] = 0x10;
	pt->commSendBuf[11] = 0x3A;
	pt->commSendBuf[12] = 0x00;
	pt->commSendBuf[13] = 0x00;
	pt->commSendBuf[14] = 0x10;	
	pt->commSendBuf[15] = 0x0A;
	pt->commSendBuf[16] = 0x00;
	pt->commSendBuf[17] = 0x00;
	pt->commSendBuf[18] = 0x01;
	pt->commSendBuf[19] = 0x01;
	pt->commSendBuf[20] = 0x04;	
	pt->commSendBuf[21] = GetDeviceType(sm->type, 1);
	if(local->bit_type && GetDeviceWord(sm->type) == 0) {
		local->addr = (WORD)sm->address * 10 / 8;
		pt->commSendBuf[22] = (BYTE)LOWORD(local->addr);
		pt->commSendBuf[23] = (BYTE)HIWORD(local->addr);
		local->size = sm->size * 10 / 8;
		i = ((WORD)sm->address*10) % 8;
		i = (8 - i) % 8;
		if(i+local->size*8 < sm->size*10) local->size++;
		if(i != 0) local->size ++;
		pt->commSendBuf[24] = (BYTE)LOWORD(local->size);
	}
	else {
		pt->commSendBuf[22] = (BYTE)sm->address;
		pt->commSendBuf[23] = (BYTE)(sm->address/256);
		pt->commSendBuf[24] = (BYTE)sm->size;
	}
	memset((char*)&pt->commSendBuf[25], 0, 9);
	pt->commSendBuf[34] = 0x17;
	memset((char*)&pt->commSendBuf[35], 0, 4);
	pt->commSendBuf[39] = MakeCRC((BYTE *)pt->commSendBuf, 0, 39);
		
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], 40);
}

enum {
	ATTACH,
	SET_TIMER,
//	SET_ETC,
	MAIL_BOX,
	WAIT_MAIL_TEXT,
};

static WORD DEC_MASK[10] = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadGESNP(LOCAL_PORT_STRUCT *pt, int pos)
{
	WORD crc = 0;
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i, j;	
	WORD  	count, val;

	LocalProtocolClass lclass(pt->hLocalProtocol);
	LOCAL_PROTOCOL_STRUCT *local = (LOCAL_PROTOCOL_STRUCT *)lclass.GetPoint();

	if(local == NULL)	return COMMUNICATION_LOCAL_PROTOCOL_MEMORY_NOT_ALLOCATED;
	
	if(GetDeviceWord(sm->type)) local->byte_count = 2;
	else                        local->byte_count = 1;

	if(sm->size*local->byte_count > 220) {
		PlcScanSetErrorString("Read size too long. (read size : 1 ~ 110(word), 1~220(byte))");
		return COMMUNICATION_ERR_STRING;
	}

	if(pt->bReadingFlag == OFF) {		
		PlcDeviceClear(&pt->device);
		switch(local->com_status) {
			case ATTACH :
				gefAttachMessage(pt);
				pt->commCountNeed = 24;
				pt->commCountSend = 24;
				//local->mail_count = 0;
				break;
			case MAIL_BOX :				
				gefMailBoxMessage(pt, pos);
				pt->commCountNeed = 42;
				pt->commCountSend = 40;	
				break;			
		}
		pt->timeout->Reset();
		pt->commCountCurr = 0;
		DisplaySendCodeNextLine(pt->no);
		pt->bReadingFlag = ON;
		return COMMUNICATION_WAITING;
	}
	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ)) {
			local->com_status = ATTACH;
			return COMMUNICATION_TIME_OUT;
		}
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;
		
		pt->commCountCurr += count;		
		if(pt->commCountCurr >= pt->commCountNeed) {// 필요한 바이트 수만큼 데이터를 모두 받았다.
			int		address = sm->target, buf_pos = 24, bit_pos, bit_count;

			switch(local->com_status) {
				case ATTACH :					
					PlcDeviceClear(&pt->device);
					if(local->nTimerUseCurr >= local->nTimerUseMax) {
						gefTimerMessage(pt, local->wait_time);					
						pt->commCountNeed = 42;
						pt->commCountSend = 40;	
						pt->commCountCurr = 0;
						pt->timeout->Reset();
						DisplaySendCodeNextLine(pt->no);
						local->com_status = SET_TIMER;
						local->nTimerUseCurr = 0;
					}
					else {
						gefMailBoxMessage(pt, pos);
						pt->commCountNeed = 42;
						pt->commCountSend = 40;
						pt->commCountCurr = 0;
						pt->timeout->Reset();
						DisplaySendCodeNextLine(pt->no);
						local->com_status = MAIL_BOX;
						local->nTimerUseCurr++;
					}

					return COMMUNICATION_WAITING;
				case SET_TIMER :
					gefAckMessage(pt);
					PlcDeviceClear(&pt->device);
					gefMailBoxMessage(pt, pos);
					pt->commCountNeed = 42;
					pt->commCountSend = 40;
					pt->commCountCurr = 0;
					pt->timeout->Reset();
					DisplaySendCodeNextLine(pt->no);
					local->com_status = MAIL_BOX;
					return COMMUNICATION_WAITING;
				case MAIL_BOX :					
					if(pt->commRecvBuf[0] != 0x06 || pt->commRecvBuf[1] != 0x00 ||
						pt->commRecvBuf[2] != 0x1B || pt->commRecvBuf[3] != 0x4D) {
						local->com_status = ATTACH;	
						return COMMUNICATION_CODE_BAD;
					}
					crc = MakeCRC((BYTE*)pt->commRecvBuf, 2, pt->commCountNeed-1);
					if(crc != pt->commRecvBuf[pt->commCountNeed-1]) {
						local->com_status = ATTACH;
						return COMMUNICATION_CODE_BAD;
					}

					if(pt->commRecvBuf[37] == 0x54) {
						local->com_status = WAIT_MAIL_TEXT;
						gefAckMessage(pt);						
						pt->commCountNeed = (WORD)pt->commRecvBuf[38]+(WORD)(pt->commRecvBuf[39]*256);
						pt->commCountSend = 2;
						pt->timeout->Reset();
						pt->commCountCurr = 0;	
						DisplaySendCodeNextLine(pt->no);
						return COMMUNICATION_WAITING;
					}					
					break;
				case WAIT_MAIL_TEXT :
					if(pt->commRecvBuf[0] != 0x1B || pt->commRecvBuf[1] != 0x54) {
						local->com_status = ATTACH;
						return COMMUNICATION_CODE_BAD;
					}
					crc = MakeCRC((BYTE*)pt->commRecvBuf, 0, pt->commCountNeed-1);
					if(crc != pt->commRecvBuf[pt->commCountNeed-1]) {
						local->com_status = ATTACH;
						return COMMUNICATION_CODE_BAD;
					}
					buf_pos = 2;
					local->com_status = MAIL_BOX;
					break;
			}
			if(local->bit_type && local->byte_count == 1) { // 10 진수영역으로 설정한 bit 영역
				bit_pos = (sm->address*10) % 8;
				bit_pos = 8 - bit_pos;
				for(i = 0, val = 0; i < sm->size; i++) {					
					for(j = bit_pos, bit_count = 0; j > 0; bit_count++) {
						j--;
						if(pt->commRecvBuf[buf_pos] & BIT_MASK[j]) val += DEC_MASK[bit_count];
					}
					buf_pos ++;
					for(j = 8; j > 0; bit_count++) {
						j--;
						if(pt->commRecvBuf[buf_pos] & BIT_MASK[j]) val += DEC_MASK[bit_count];						
						if(bit_count >= 9) break;
					}
					if(bit_count >= 9) {
						bit_pos = j;
						if(j == 0) { 
							bit_pos = 8;
							buf_pos ++;
						}						
						PokeWORD(pt, address+i, val & 1023);
						if(val & 1024) val = 1;
						else           val = 0;
						continue;
					}
					buf_pos ++;
					for(j = 8; j > 0; bit_count++) {
						j--;
						if(pt->commRecvBuf[buf_pos] & BIT_MASK[j]) val += DEC_MASK[bit_count];						
						if(bit_count >= 9) break;
					}
					bit_pos = j;
					if(j == 0) { 
						bit_pos = 8;
						buf_pos ++;
					}					
					PokeWORD(pt, address+i, val & 1023);
					if(val & 1024) val = 1;
					else           val = 0;
				}
				PokeWORD(pt, address+i, val);				
			}
			else {
				for(i = 0; i < sm->size; i++) {
					if(local->byte_count == 1)
						PokeValue(pt, sm, address+i, (BYTE)pt->commRecvBuf[buf_pos+i]);
					else 
						PokeValue(pt, sm, address+i, (WORD)pt->commRecvBuf[buf_pos+i*2]+(WORD)(pt->commRecvBuf[buf_pos+i*2+1]*256));
				}
			}
			gefAckMessage(pt);
			return COMMUNICATION_OK;
		}
	}
}



// WRITE FUNCTION

// Write Message Packet
static void gefMailBoxWriteMessage(LOCAL_PORT_STRUCT *pt, WORD address, char *device, WORD value, BOOL byte)
{
	pt->commSendBuf[0] = 0x1B;
	pt->commSendBuf[1] = 0x4D;
	memset((char*)&pt->commSendBuf[2], 0, 6);
	pt->commSendBuf[8] = 0x02;//count;
	pt->commSendBuf[9] = 0xC0;
	pt->commSendBuf[10] = 0x10;
	pt->commSendBuf[11] = 0x3A;
	pt->commSendBuf[12] = 0x00;
	pt->commSendBuf[13] = 0x00;
	pt->commSendBuf[14] = 0x10;
	pt->commSendBuf[15] = 0x0A;
	pt->commSendBuf[16] = 0x00;
	pt->commSendBuf[17] = 0x00;
	pt->commSendBuf[18] = 0x01;
	pt->commSendBuf[19] = 0x01;
	pt->commSendBuf[20] = 0x07;	
	pt->commSendBuf[21] = GetDeviceType(device, byte); //sm->type	
	pt->commSendBuf[22] = LOBYTE(address);
	pt->commSendBuf[23] = HIBYTE(address);
	pt->commSendBuf[24] = 0x01;
	pt->commSendBuf[25] = 0x00;
	pt->commSendBuf[26] = LOBYTE(value);
	pt->commSendBuf[27] = HIBYTE(value);
	memset((char*)&pt->commSendBuf[28], 0, 6);	
	pt->commSendBuf[34] = 0x17;
	memset((char*)&pt->commSendBuf[35], 0, 4);	
	pt->commSendBuf[39] = MakeCRC((BYTE *)pt->commSendBuf, 0, 39);
		
	PlcDeviceWriteContinue(&pt->device, (char*)&pt->commSendBuf[0], 40);
}


// Write Attach
static int GefSendAttach(LOCAL_PORT_STRUCT *pt)
{
	int				count;
	TimeOutClass	timeout;

	PlcDeviceClear(&pt->device);
	gefAttachMessage(pt);
	pt->commCountNeed = 24;
	pt->commCountSend = 24;
	pt->commCountCurr = 0;
	DisplaySendCodeNextLine(pt->no);

	timeout.Reset();

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);
		pt->commCountCurr += count;
		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) break;
		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE)) return COMMUNICATION_TIME_OUT;
	}
	return COMMUNICATION_OK;
}


// Write Set Timer Or Etc
static int GefSendTimerEtc(LOCAL_PORT_STRUCT *pt, WORD wait_time, BOOL mode)
{
	int				count;
	TimeOutClass	timeout;
	WORD			crc;

	PlcDeviceClear(&pt->device);
	if(mode) gefTimerMessage(pt, wait_time);
	else	 gefEtcMessage(pt);
	pt->commCountNeed = 42;
	pt->commCountSend = 40;
	pt->commCountCurr = 0;
	DisplaySendCodeNextLine(pt->no);

	timeout.Reset();
	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);
		pt->commCountCurr += count;
		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != 0x06 || pt->commRecvBuf[1] != 0x00 ||
				pt->commRecvBuf[2] != 0x1B || pt->commRecvBuf[3] != 0x4D) {
					return COMMUNICATION_CODE_BAD;
			}					
			crc = MakeCRC((BYTE*)pt->commRecvBuf, 2, pt->commCountNeed-1);
			if(crc != pt->commRecvBuf[pt->commCountNeed-1]) return COMMUNICATION_CODE_BAD;
			break;
		}
		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE)) return COMMUNICATION_TIME_OUT;
	}
	gefAckMessage(pt);
	return COMMUNICATION_OK;
}


// 한 워드를 통신을 통해 쓴다.
static int GESNPWriteWord(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device, BOOL flag)
{
	WORD			crc = 0;
	int				count, i;
	TimeOutClass	timeout;

	LocalProtocolClass lclass(pt->hLocalProtocol);
	LOCAL_PROTOCOL_STRUCT *local = (LOCAL_PROTOCOL_STRUCT *)lclass.GetPoint();
	
	if(local == NULL)	return COMMUNICATION_LOCAL_PROTOCOL_MEMORY_NOT_ALLOCATED;

	i = GetDeviceType(device, flag);
	if(i == 0 || i == 30) {
		PlcScanSetErrorString("PLC Memory Type Error or Read only Memory.");
		return COMMUNICATION_ERR_STRING;
	}
	if(local->attach_write_flag) { // 쓰기 시 attach 할 것인지 ?
		PlcDeviceClear(&pt->device);
		if(GefSendAttach(pt) != COMMUNICATION_OK) return COMMUNICATION_TIME_OUT;
		//i = GefSendTimerEtc(pt, local->wait_time, 1);
		//if(i != COMMUNICATION_OK) return i;
		//i = GefSendTimerEtc(pt, local->wait_time, 0);
		//if(i != COMMUNICATION_OK) return i;
	}

	PlcDeviceClear(&pt->device);
	gefMailBoxWriteMessage(pt, address, device, value, flag);
	pt->commCountNeed = 42;
	pt->commCountSend = 40;	
	pt->commCountCurr = 0;
	
	DisplaySendCodeNextLine(pt->no);

	timeout.Reset();	

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;
		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[0] != 0x06 || pt->commRecvBuf[1] != 0x00 ||
			   pt->commRecvBuf[2] != 0x1B || pt->commRecvBuf[3] != 0x4D) {
				PlcScanSetErrorString("Plc write error.");
				return COMMUNICATION_ERR_STRING;						
			}
			crc = MakeCRC((BYTE*)pt->commRecvBuf, 2, 41);
			if(crc != pt->commRecvBuf[pt->commCountNeed-1]) {
				return COMMUNICATION_CODE_BAD;
			}					
			gefAckMessage(pt);
			return COMMUNICATION_OK;
		}
		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE)) return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//	하나의 디지털이 하나의 워드를 사용하므로 WORD 쓰는 방식과 동일한다.
// ON 일때는 0xFFFF, OFF 일때는 0x0000을 주소에 쓰면 된다.
//------------------------------------------------------------------------------

int PlcScanWriteBitGESNP(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	if(flag)
		return GESNPWriteWord(pt, station, (address/16)*10+(address%16)-1, 0xFF, device, 0);
	else
		return GESNPWriteWord(pt, station, (address/16)*10+(address%16)-1, 0, device, 0);
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordGESNP(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device)
{
	return GESNPWriteWord(pt, station, address-1, (WORD)value, device, 1);
}