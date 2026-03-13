//------------------------------------------------------------------------------
//	ASR LA250 Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
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

void PlcScanDrawMethodTitleLA250(HDC hdc, int x, int y)
{
	char *string = "station, type, slot address, buf address, (LA250)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodLA250(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

//	wsprintf(buf, "%3d", sm->extra1);		// size word of read
//	TextOut(hdc, x+cxChar*15, y, buf, strlen(buf));
}

static void SetErrorCode(int slot, char code)
{
	char string[100];

	switch(code) {
		case '1':
			sprintf(string, "Slot%d번[ErrCode-1 : Local을 위해 SET되지 않음(RS)]", slot);
			break;
		case '2':
			sprintf(string, "Slot%d번[ErrCode-2 : Slot 이 없음]", slot);
			break;
		case '3':
			sprintf(string, "Slot%d번[ErrCode-3 : 통신 이상(RS, RM)]", slot);
			break;
		case '4':
			sprintf(string, "Slot%d번[ErrCode-4 : 수치오류(RS, RM)]", slot);
			break;
		case '5':
			sprintf(string, "Slot%d번[ErrCode-5 : 통신 이상(RS, RM)]", slot);
			break;
		case '6':
		case '7':
			sprintf(string, "Slot%d번[ErrCode-%c]", slot, code);
			break;
		case '8':
			sprintf(string, "Slot%d번[ErrCode-8 : buffer over (255 over)]", slot);
			break;
		default:
			sprintf(string, "Slot%d번[ErrCode-%c : 미확인 오류 code]", slot, code);
			break;
	}

	PlcScanSetErrorString(string);
}



//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadLA250Extend(LOCAL_PORT_STRUCT *pt, char *type, int address, int target)
{
	int  i;
	int  count;
	char imsi[10];
	char end_flag;
	float value;
	CommaBlockString commaBuf;

	if(pt->bReadingFlag == OFF) {
		PlcDeviceClear(&pt->device);

		// send code example RQ00 0101+LF	slot 번호 1번부터 1개slot만 설정치를 읽어온다.

		pt->commSendBuf[0] = 'R';				// plc address
		if(strcmp(type, "PV") == 0)
			pt->commSendBuf[1] = 'P';				// 현재값 의 확인
		else if(strcmp(type, "SV") == 0)
			pt->commSendBuf[1] = 'Q';				// 설정 상태의 확인
		else  		// DI
			pt->commSendBuf[1] = 'M';				// 운전과 remote/local 상태의 확인
		pt->commSendBuf[2] = '0';
		pt->commSendBuf[3] = '0';
		pt->commSendBuf[4] = 32;					// space
		sprintf(imsi, "%02d", address);	// slot 번호
		pt->commSendBuf[5] = imsi[0];
		pt->commSendBuf[6] = imsi[1];

		pt->commSendBuf[7] = imsi[0];	// end slot
		pt->commSendBuf[8] = imsi[1];	//	end slot

		pt->commSendBuf[9] = LF;		// 0x0A		- end command

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 10);

		pt->commCountNeed = MAX_RECV_BUF; // 읽어올수 있는 값은 스트링으로 항상 가변이다.
		pt->commCountSend = 10;
		pt->commCountCurr = 0;

		pt->timeout->Reset();

		if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

		pt->bReadingFlag = ON;

		return COMMUNICATION_WAITING;
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;

		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);	

		if(count == 0)	return COMMUNICATION_WAITING;

		pt->commCountCurr += count;

		end_flag = OFF;
		for(i = 0; i < pt->commCountCurr; i++) {
			if(pt->commRecvBuf[i] == LF) {
				pt->commRecvBuf[i] = 0;
				end_flag = ON;
				break;
			}
		}

		// 데이터 끝남 코드 LF를 받았다.
		if(end_flag) {
			// 처음 코드는 항상 'A'이다.
			if(pt->commRecvBuf[0] != 'A')
				return COMMUNICATION_CODE_BAD;

			// 보내준 명령어와 같은 address로 돌아온다.
			if(strncmp((char*)&pt->commRecvBuf[1], (char*)&pt->commSendBuf[1], 8) != 0)
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[9] != 32)	// space
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[10] != '0') {	// 통신 status가 정상이 아니다.
				SetErrorCode(address, pt->commRecvBuf[10]);
				return COMMUNICATION_ERR_STRING;
			}
 
			if(pt->commRecvBuf[11] != 32) 	// space
				return COMMUNICATION_CODE_BAD;

			commaBuf.Set((char*)&pt->commRecvBuf[12]);

			if(strcmp(type, "PV") == 0) {	// 현재치
				int address = target;
				int int_value;

				// 현재치
				commaBuf.GetFloat(value);
				PokeFLOAT(pt, address, value);

				// 제어출력 Hi
				commaBuf.GetInt(int_value);
				if(int_value == 1)
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) | 0x0004);
				else
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) & 0xFFFB);
				// 제어출력 Lo
				commaBuf.GetInt(int_value);
				if(int_value == 1)
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) | 0x0008);
				else
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) & 0xFFF7);
				// Alarm Hi
				commaBuf.GetInt(int_value);
				if(int_value == 1)
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) | 0x0010);
				else
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) & 0xFFEF);
				// Alarm Lo
				commaBuf.GetInt(int_value);
				if(int_value == 1)
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) | 0x0020);
				else
					PokeWORD(pt, address+5, PeekValueWORD(pt, address+5) & 0xFFDF);

			}
			else if(strcmp(type, "SV") == 0) {
				for(i = 0; i < 17; i++) {
					int address = target+(i*2);
					commaBuf.GetFloat(value);
					PokeFLOAT(pt, address, value);
				}
			}
			else {	// remote/local 운전	"DI"
				int address = target;

				if(address <= pt->nBufSizeWORD-1) {
					int int_value;
					commaBuf.GetInt(int_value);
					if(int_value == 1)
						PokeWORD(pt, address, PeekValueWORD(pt, address) | 0x0002);
					else
						PokeWORD(pt, address, PeekValueWORD(pt, address) & 0xFFFD);

					commaBuf.GetInt(int_value);
					if(int_value == 1)
						PokeWORD(pt, address, PeekValueWORD(pt, address) | 0x0001);
					else
						PokeWORD(pt, address, PeekValueWORD(pt, address) & 0xFFFE);
				}
			}

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
// 한슬롯당 40워드를 차지하고 처음
// 0~1 WORD = Slot 현재치
// 5 WORD   = 운전 상태 비트  ( 	0 - Control bit
//                               1 - Remote / local Bit
//											2 - Hi 출력
//											3 - Lo 출력
//											4 - Hi Alarm
//											5 - Lo Alarm
//	6~39 WORD = 17개의 설정값.
//------------------------------------------------------------------------------

int PlcScanReadLA250(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int retn;

	if(strcmp(sm->type, "PV") == 0) {	// 현재치
		retn = PlcScanReadLA250Extend(pt, sm->type, sm->address, sm->target);
	}
	else if(strcmp(sm->type, "SV") == 0) {
		retn = PlcScanReadLA250Extend(pt, sm->type, sm->address, sm->target);
	}
	else {	// DI
		retn = PlcScanReadLA250Extend(pt, sm->type, sm->address, sm->target);
	}

	return retn;
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitLA250(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD address, WORD flag)
{
	int  	 	count;
	int 	 	i;
	TimeOutClass timeout;
	char		end_flag;
	int		slot;

	PlcDeviceClear(&pt->device);

	slot = address/16;

	if(slot < 1 || slot > 99) {
		PlcScanSetErrorString("01 <= slot <= 99 를 만족안함");
		return COMMUNICATION_ERR_STRING;
	}

	pt->commSendBuf[0] = 'R';	// START code
	pt->commSendBuf[1] = 'M';	// mode 절환
	pt->commSendBuf[2] = '0';
	pt->commSendBuf[3] = '0';
	pt->commSendBuf[4] = 32;		// space
	sprintf((char*)&pt->commSendBuf[5], "%02d", slot);	// start slot
	sprintf((char*)&pt->commSendBuf[7], "%02d", slot);	// end   slot
	pt->commSendBuf[9] = 32;		// space

	pt->commSendBuf[10] = '1';	// remote
	pt->commSendBuf[11] = ',';
	if(flag == ON)	pt->commSendBuf[12] = '1';	// control ON
	else			pt->commSendBuf[12] = '0';	// control OFF
	pt->commSendBuf[13] = LF;	// 0x0a	종료 코드

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 14);

	pt->commCountSend = 14;
	pt->commCountNeed = pt->commCountSend+2;	// 이상이 없다면 보낸 코드보다 2byte(status) 큰 값이 들어온다.
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		end_flag = OFF;
		for(i = 0; i < pt->commCountCurr; i++) {
			if(pt->commRecvBuf[i] == LF) {
				pt->commRecvBuf[i] = 0;
				end_flag = ON;
				break;
			}
		}

		// 데이터 끝남 코드 LF를 받았다.
		if(end_flag) {
			// 처음 코드는 항상 'A'이다.
			if(pt->commRecvBuf[0] != 'A')
				return COMMUNICATION_CODE_BAD;

			// 보내준 명령어와 같은 address로 돌아온다.
			if(strncmp((char*)&pt->commRecvBuf[1], (char*)&pt->commSendBuf[1], 8) != 0)
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[9] != 32)	// space
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[10] != '0') {	// 통신 status가 정상이 아니다.
				SetErrorCode(slot, pt->commRecvBuf[10]);
				return COMMUNICATION_ERR_STRING;
			}

			if(pt->commRecvBuf[11] != 32) 	// space
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 값을 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordLA250(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD address, float value)
{
	int  	 	count;
	int 	 	i;
	int 		slot;
	char		imsi[80];
	TimeOutClass timeout;
	char		end_flag;

	PlcDeviceClear(&pt->device);

	slot = address/100;
	address = address%100;

	if(address < 3 || address > 19) {
		PlcScanSetErrorString("??03 <= address <= ??19 를 만족안함");
		return COMMUNICATION_ERR_STRING;
	}

	pt->commSendBuf[0] = 'R';	// START code
	pt->commSendBuf[1] = 'S';	// set value command
	pt->commSendBuf[2] = '0';
	pt->commSendBuf[3] = '0';
	pt->commSendBuf[4] = 32;		// space
	sprintf((char*)&pt->commSendBuf[5], "%02d", slot);	// start slot
	sprintf((char*)&pt->commSendBuf[7], "%02d", slot);	// end   slot
	pt->commSendBuf[9] = 32;		// space

	pt->commCountSend = 10;

	for(i = 3; i < address; i++) {	// 해당되지 않는 address는 공백으로 남겨둔다.
		pt->commSendBuf[pt->commCountSend] = ',';
		pt->commCountSend++;
	}
	sprintf(imsi, "%.2f", value);
	strncpy((char*)&pt->commSendBuf[pt->commCountSend], imsi, strlen(imsi));
	pt->commCountSend += strlen(imsi);
	for(i = address+1; i <= 19; i++) {	// 해당되지 않는 address는 공백으로 남겨둔다.
		pt->commSendBuf[pt->commCountSend] = ',';
		pt->commCountSend++;
	}
	pt->commSendBuf[pt->commCountSend] = LF;	// 0x0a	종료 코드
	pt->commCountSend++;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	pt->commCountNeed = MAX_RECV_BUF;	// 전체의 값이 모두 들어오므로 크기가 얼마인지는 알 수 없다.
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);

		pt->commCountCurr += count;

		end_flag = OFF;
		for(i = 0; i < pt->commCountCurr; i++) {
			if(pt->commRecvBuf[i] == LF) {
				pt->commRecvBuf[i] = 0;
				end_flag = ON;
				break;
			}
		}

		// 데이터 끝남 코드 LF를 받았다.
		if(end_flag) {
			// 처음 코드는 항상 'A'이다.
			if(pt->commRecvBuf[0] != 'A')
				return COMMUNICATION_CODE_BAD;

			// 보내준 명령어와 같은 address로 돌아온다.
			if(strncmp((char*)&pt->commRecvBuf[1], (char*)&pt->commSendBuf[1], 8) != 0)
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[9] != 32)	// space
				return COMMUNICATION_CODE_BAD;

			if(pt->commRecvBuf[10] != '0') {	// 통신 status가 정상이 아니다.
				SetErrorCode(slot, pt->commRecvBuf[10]);
				return COMMUNICATION_ERR_STRING;
			}

			if(pt->commRecvBuf[11] != 32) 	// space
				return COMMUNICATION_CODE_BAD;

			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}



