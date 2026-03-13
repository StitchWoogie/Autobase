//------------------------------------------------------------------------------
//	BL2300 Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>
#include <math.h>

#include <totaldef.h>
#include <tools.h>
#include <glib.h>
#include <dataswap.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"

#define	MAX_TIME_OUT		3

enum {
	RECV_CODE_INIT,	// 초기화 코드가 들오엄
	RECV_CODE_READ,
	RECV_CODE_ADJ1,
	RECV_CODE_ADJ2,
	RECV_CODE_ADJ3,
	RECV_CODE_ADJ4,
	RECV_CODE_ADJ5,
	RECV_CODE_ADJ6,
	RECV_CODE_ADJ7,
	RECV_CODE_ACK,
	RECV_CODE_BCK,
	RECV_CODE_CCK,

	RECV_CODE_CODE_BAD,
	RECV_CODE_TIME_OUT,
};

void PlcScanDrawMethodTitleBL2300(HDC hdc, int x, int y)
{
	char *string = "READ, (BL2300)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodBL2300(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;
	wsprintf(buf, "%3d", sm->station);
	TextOut(hdc, x, y, buf, strlen(buf));

	wsprintf(buf, "%s",  sm->type);
	TextOut(hdc, x+cxChar*4, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->address);
	TextOut(hdc, x+cxChar*7, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*11, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->size);		// size word of read
	TextOut(hdc, x+cxChar*15, y, buf, strlen(buf));
}

static char bInitFailFlag = OFF;

static int BL2300ReadBlock(LOCAL_PORT_STRUCT *pt)
{
	TimeOutClass timeout;
	char message[160];
	int count;
	COMSTAT comStat;

	timeout.Reset();

	pt->commCountCurr = 0;

	while(1) {             
		PlcDeviceGetCommError(&pt->device, &comStat);	//

		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], MAX_RECV_BUF+pt->commCountCurr-1);

		if(pt->commCountCurr+count >= MAX_RECV_BUF)	return 0;

		if(count > 0) {
			pt->commCountCurr += count;
			if(pt->commRecvBuf[pt->commCountCurr-1] == CR) {
				return 1;
			}
		}

		if(timeout.IsTimeOut(3)) {
			sprintf(message, "Port %d번의 BL-2300 통신 초과.", pt->no);
			MessageDisplay(message);
			return 0;
		}
	}
}

static int BL2300ReadMode(LOCAL_PORT_STRUCT *pt)
{
	char message[160];

	if(!BL2300ReadBlock(pt))	return RECV_CODE_TIME_OUT;

	if(strncmp((char*)pt->commRecvBuf, "DI", 2) == 0) {
		return RECV_CODE_INIT;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D0", 2) == 0) {
		return RECV_CODE_READ;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D1",  2) == 0) {
		return RECV_CODE_ADJ1;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D2",  2) == 0) {
		return RECV_CODE_ADJ2;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D3",  2) == 0) {
		return RECV_CODE_ADJ3;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D4",  2) == 0) {
		return RECV_CODE_ADJ4;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D5",  2) == 0) {
		return RECV_CODE_ADJ5;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D6",  2) == 0) {
		return RECV_CODE_ADJ6;
	}
	else if(strncmp((char*)pt->commRecvBuf, "D7",  2) == 0) {
		return RECV_CODE_ADJ7;
	}
	else if(strncmp((char*)pt->commRecvBuf, "AC", 2) == 0) {
		return RECV_CODE_ACK;
	}
	else if(strncmp((char*)pt->commRecvBuf, "BC", 2) == 0) {
		return RECV_CODE_BCK;
	}
	else if(strncmp((char*)pt->commRecvBuf, "CC", 2) == 0) {
		return RECV_CODE_CCK;
	}
	else {
		sprintf(message, "Port %d번의 BL-2300 통신 코드 불량.", pt->no);
		MessageDisplay(message);
		return RECV_CODE_CODE_BAD;
	}
}

static void SendACK(LOCAL_PORT_STRUCT *pt)
{
	pt->commSendBuf[0] = 'A';		// plc address
	pt->commSendBuf[1] = 'C';		// my address
	pt->commSendBuf[2] = CR;  		// read word
	pt->commSendBuf[3] = LF;  		// read word

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 4);
}

//--------------------------------------------------------------------------
// 그룹 접점 출력 데이터 파일을 읽어온다.
//--------------------------------------------------------------------------

static int BL2300ReadGroupTableData(int port, int group, int *data)
{
	char filename[MAXPATH];
	char buf[200];
	FILE *in;
	int  number;
	int  pos;
	char message[MAXPATH];
	int  i;
	CommaBlockString commaBuf;

	sprintf(filename, "%s\\scan\\group%d.%03d", sDirWorkProject, group, port);
	in = fopen(filename, "rb");
	if(in == NULL) {
		sprintf(message, "Port %d번의 BL-2300 Group%d 초기화 파일이 없음.", port, group);
		MessageDisplay(message);
		return 0;
	}

	pos = 0;

	while(1) {		// 데이터 파일을 읽는다.
		TextGetOneLine(in, buf, sizeof(buf));

		commaBuf.Set(buf);
		commaBuf.GetInt(number);	// 첫번째의 참고 라인.
		for(i = 0; i < 10; i++, pos++) {
			if(pos >= 250)	break;
			commaBuf.GetInt(data[pos]);
		}
		if(pos >= 250)	break;
	}

	fclose(in);

	return 1;
}

//--------------------------------------------------------------------------
// ZRT, SPPT, SPVT 값을 파일에서 읽어온다.
//--------------------------------------------------------------------------

static int BL2300ReadValueFile(LOCAL_PORT_STRUCT *pt)
{
	char filename[MAXPATH];
	char buf[200];
	FILE *in;
	int  pos;
	char message[MAXPATH];
	int  skip;
	CommaBlockString commaBuf;

	sprintf(filename, "%s\\scan\\value.%03d", sDirWorkProject, pt->no);
	in = fopen(filename, "rb");
	if(in == NULL) {
		sprintf(message, "Port %d번의 BL-2300 설정값 초기화 파일이 없음.", pt->no);
		MessageDisplay(message);
		return 0;
	}

	pos = 0;

	TextGetOneLine(in, buf, sizeof(buf));	// 제목줄은 skip 한다.
	while(1) {		// 데이터 파일을 읽는다.
		if(pos >= 281)	break;
		if(pos >= 224) {
			PokeWORD(pt, 300+pos*5+2, 2048);	// ZRT
			PokeWORD(pt, 300+pos*5+3, 3200);	// SPPT
			PokeWORD(pt, 300+pos*5+4, 255);	// SPVT
		}
		else {
			WORD imsi;
			TextGetOneLine(in, buf, sizeof(buf));
			commaBuf.Set(buf);
			commaBuf.GetInt(skip);
			commaBuf.GetWORD(imsi);	// ZRT
			PokeWORD(pt, 300+pos*5+2, imsi);
			commaBuf.GetWORD(imsi);	// SPPT
			PokeWORD(pt, 300+pos*5+3, imsi);
			commaBuf.GetWORD(imsi);	// SPVT
			PokeWORD(pt, 300+pos*5+4, imsi);
		}
		pos++;
	}

	fclose(in);

	return 1;
}

//--------------------------------------------------------------------------
// LTT0~5 의 값을 파일에서 읽어온다.
//--------------------------------------------------------------------------

static int BL2300ReadLttFile(int port, int *data)
{
	char filename[MAXPATH];
	char buf[200];
	FILE *in;
	int  pos;
	char message[MAXPATH];
	int  number;
	CommaBlockString commaBuf;

	sprintf(filename, "%s\\scan\\LLT.%03d", sDirWorkProject, port);
	in = fopen(filename, "rb");
	if(in == NULL) {
		sprintf(message, "Port %d번의 BL-2300 LLT 설정값 파일이 없음.", port);
		MessageDisplay(message);
		return 0;
	}

	pos = 0;

	TextGetOneLine(in, buf, sizeof(buf));	// 제목줄은 skip 한다.
	while(1) {		// 데이터 파일을 읽는다.
		if(pos >= 256)	break;
		TextGetOneLine(in, buf, sizeof(buf));
		commaBuf.Set(buf);
		commaBuf.GetInt(number);
		commaBuf.GetInt(data[pos]);	// ZRT

		if(number != pos+1) {
			sprintf(message, "Port %d번의 BL-2300 LLT 설정값 파일이 깨져 있습니다.(pos=%d)", port, pos+1);
			MessageDisplay(message);
			fclose(in);
			return 0;
		}

		pos++;
	}

	fclose(in);

	return 1;
}

//--------------------------------------------------------------------------
// ZRT, SPPT, SPVT 값을 파일에 저장한다.
//--------------------------------------------------------------------------

static int BL2300SaveValueFile(LOCAL_PORT_STRUCT *pt)
{
	char filename[MAXPATH];
	FILE *out;
	char message[MAXPATH];
	int  i;

	sprintf(filename, "%s\\scan\\value.%03d", sDirWorkProject, pt->no);
	out = fopen(filename, "w");
	if(out == NULL) {
		sprintf(message, "Port %d번의 BL-2300 설정값 파일을 저장할 수 없음.", pt->no);
		MessageDisplay(message);
		return 0;
	}

	fprintf(out, "  NO  ,  ZRT, SPPT, SPVT,\n");
	for(i = 0; i < 224; i++) {
		if(pt->nBufSizeWORD <= 305+i*5)	break;	// buf 초과.
		fprintf(out, "NO:%03d, %4d, %4d, %4d,\n", i+1, PeekValueWORD(pt, 300+i*5+2), PeekValueWORD(pt, 300+i*5+3), PeekValueWORD(pt, 300+i*5+4));
	}

	fclose(out);

	return 1;
}

void PlcDeviceEscapeCommFunction(int port, int function);


static void WaitEndSec()
{
	TimeOutClass timeout;
	MSG msg;

	timeout.Reset();

	while(1) {
		if(timeout.IsTimeOut(1))	break;
		if(PeekMessage (&msg, NULL, 0, 0, PM_REMOVE)) {
			TranslateMessage (&msg);
			DispatchMessage (&msg);
		}
	}
}

static void BL2300Init(LOCAL_PORT_STRUCT *pt)
{
	int retn;
	int i;
	char message[MAXPATH];
	StackInt group1(250);
	StackInt group2(250);
	StackInt group3(250);
	StackInt llt(256);

	if(group1.data == NULL ||
		group2.data == NULL ||
		group3.data == NULL ||
		llt.data    == NULL) {
		sprintf(message, "메모리 부족으로 Port %d번의 BL-2300 초기화 불가능.", pt->no);
		MessageDisplay(message);
		return;
	}

	if(!BL2300ReadGroupTableData(pt->no, 1, group1.data))	return;
	if(!BL2300ReadGroupTableData(pt->no, 2, group2.data))	return;
	if(!BL2300ReadGroupTableData(pt->no, 3, group3.data))	return;

	if(!BL2300ReadLttFile(pt->no, llt.data))	return;

	BL2300ReadValueFile(pt);

	sprintf(message, "Port %d번의 BL-2300 초기화중.", pt->no);
	MessageDisplay(message);

	pt->commSendBuf[0] = 'D';		// plc address
	pt->commSendBuf[1] = 'I';		// my address
	pt->commSendBuf[2] = '2';		// plc address
	pt->commSendBuf[3] = '8';		// my address
	pt->commSendBuf[4] = '1';		// plc address
	pt->commSendBuf[5] = CR;  		// read word
	pt->commSendBuf[6] = LF;  		// read word

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 7);

	retn = BL2300ReadMode(pt);  // wait ACCR;

	if(retn != RECV_CODE_ACK)	{
		bInitFailFlag = ON;
		return;
	}

	for(i = 0; i < 281; i++) {
		sprintf(message, "[Port:%d] BL-2300의 %d point 설정치 초기화 중.", pt->no, i+1);
		MessageDisplay(message);

		if((i+1 >= 201 && i+1 <= 208) ||
			(i+1 >= 225 && i+1 <= 281)) {
			sprintf((char*)pt->commSendBuf+ 0, "%04d", 100);		// FST 검지부 full scale
			sprintf((char*)pt->commSendBuf+ 4, "%04d", 0);		// DPT 소수점 위치
			sprintf((char*)pt->commSendBuf+ 8, "%04d", 20);		// DDT 표시 분해능
			sprintf((char*)pt->commSendBuf+12, "%04d", 50);		// LAT 1st alarm
			sprintf((char*)pt->commSendBuf+16, "%04d", 50);		// HAT 2nd alarm
			sprintf((char*)pt->commSendBuf+20, "%04d", 6);		// LLT 검량선 번호
			sprintf((char*)pt->commSendBuf+24, "%04d", 0);		// APT alarm pass
		}
		else {
			sprintf((char*)pt->commSendBuf+ 0, "%04d", 1000);	// FST 검지부 full scale
			sprintf((char*)pt->commSendBuf+ 4, "%04d", 1);		// DPT 소수점 위치
			sprintf((char*)pt->commSendBuf+ 8, "%04d", 5);		// DDT 표시 분해능
			sprintf((char*)pt->commSendBuf+12, "%04d", 250);		// LAT 1st alarm
			sprintf((char*)pt->commSendBuf+16, "%04d", 500);		// HAT 2nd alarm
			sprintf((char*)pt->commSendBuf+20, "%04d", 0);		// LLT 검량선 번호
			sprintf((char*)pt->commSendBuf+24, "%04d", 0);		// APT alarm pass
		}

		sprintf((char*)pt->commSendBuf+28, "%04d", PeekValueWORD(pt, 300+i*5+2));		// ZRT zero시 센서 출력
		sprintf((char*)pt->commSendBuf+32, "%04d", PeekValueWORD(pt, 300+i*5+3));		// SPPT span시 센서 출력
		sprintf((char*)pt->commSendBuf+36, "%04d", PeekValueWORD(pt, 300+i*5+4));		// SPVT span 농도
		sprintf((char*)pt->commSendBuf+40, "%c%c", CR, LF);

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 42);

		retn = BL2300ReadMode(pt);  // wait A+C+CR;
		if(retn != RECV_CODE_ACK) {
			sprintf(message, "[Port:%d] BL-2300의 %d point 초기화 실패. No AC", pt->no, i+1);
			MessageDisplay(message);
			bInitFailFlag = ON;
			return;
		}
	}

	// 검량선 데이터를 초기화 한다.
	for(i = 0; i < 256; i++) {
		sprintf(message, "[Port:%d] BL-2300의 검량선 Data#%d 초기화 중.", pt->no, i+1);
		MessageDisplay(message);

		sprintf((char*)pt->commSendBuf+ 0, "%04d", llt.data[i]);
		sprintf((char*)pt->commSendBuf+ 4, "%04d", llt.data[i]);
		sprintf((char*)pt->commSendBuf+ 8, "%04d", llt.data[i]);
		sprintf((char*)pt->commSendBuf+12, "%04d", llt.data[i]);
		sprintf((char*)pt->commSendBuf+16, "%04d", llt.data[i]);
		sprintf((char*)pt->commSendBuf+20, "%04d", llt.data[i]);

		if(i == 255) {
			sprintf((char*)pt->commSendBuf+24, "%04d", 3);
		}
		else if(i <= 2) {
			sprintf((char*)pt->commSendBuf+24, "%04d", 2);
		}
		else {
			sprintf((char*)pt->commSendBuf+24, "%04d", 0);
		}
		sprintf((char*)pt->commSendBuf+28, "%c%c", CR, LF);
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 30);

		retn = BL2300ReadMode(pt);  // wait B+C+CR;

		if(retn != RECV_CODE_BCK) {
			sprintf(message, "[Port:%d] BL-2300의 %d point 초기화 실패. No BC", pt->no, i+1);
			MessageDisplay(message);
			bInitFailFlag = ON;
			return;
		}
	}

	// 그룹 접점 출력 테이블 데이터를 초기화 한다.
	for(i = 0; i < 250; i++) {
		sprintf(message, "[Port:%d] BL-2300의 Group 접점 출력 #%d 초기화 중.", pt->no, i+1);
		MessageDisplay(message);

		sprintf((char*)pt->commSendBuf+0, "%04d", group1.data[i]);
		sprintf((char*)pt->commSendBuf+4, "%04d", group2.data[i]);
		sprintf((char*)pt->commSendBuf+8, "%04d", group3.data[i]);
		sprintf((char*)pt->commSendBuf+12, "%c%c", CR, LF);
		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 14);

		retn = BL2300ReadMode(pt);  // wait C+C+CR;

		if(retn != RECV_CODE_CCK) {
			sprintf(message, "[Port:%d] BL-2300의 %d point 초기화 실패. No CC", pt->no, i+1);
			MessageDisplay(message);
			bInitFailFlag = ON;
			return;
		}
	}

	WaitEndSec();
	WaitEndSec();
	WaitEndSec();
	WaitEndSec();
}

typedef struct {
	int llt0;
} LLT_STRUCT;

Block blockLLT(sizeof(LLT_STRUCT));

static int GetLLT0(int table)
{
	static char flag = OFF;
	LLT_STRUCT block;

	if(flag)	{
		if(blockLLT.GetBlockCount() == 0)	return 0;
		blockLLT.GetBlock((BYTE*)&block, table);
		return block.llt0 ;
	}

	flag = ON;

	StackInt llt(256);
	int i;

	if(llt.data == NULL)	return 0;

	if(!BL2300ReadLttFile(0, llt.data))	return 0;

	for(i = 0; i < 256; i++) {
		block.llt0 = llt.data[i];
		blockLLT.AddBlock((BYTE*)&block);
	}

	return 0;
}

static void BL2300Adj(LOCAL_PORT_STRUCT *pt, int adj)
{
	static char first_flag = OFF;

	if(first_flag == OFF) {	// 조정 모드가 시작되면 조정 파일을 읽어와야 한다.
   	BL2300ReadValueFile(pt);
		first_flag = ON;
	}

	SendACK(pt);

	int  start_address = 300+(32*5)*(adj-1);
	int  address;
	char buf[80];
	int  element;
	int  i;
	int  retn;
	float value;

	strncpy(buf, (char*)&pt->commRecvBuf[2], 3);
	buf[3] = 0;
	i = atoi(buf);
	if(i >= 1 && i <= 16) {
		PokeWORD(pt, 0, WORD_MASK[i-1]);
	}

	for(i = 0; i < 32; i++) {
		address = 2000+(i*10);

		if(address >= pt->nBufSizeWORD) 	break;	// zone over
		PokeWORD(pt, address, (adj-1)*32+(i+1));
	}

	for(element = 0; element < 5; element++) {
		retn = BL2300ReadBlock(pt);
//		WaitReadyToSend(pt->no);
		SendACK(pt);

		if(retn == 0) 	continue;
		if(pt->commCountCurr != 129)	continue;	// 32*4+CR

		for(i = 0; i < 32; i++) {
			address = start_address+(i*5)+element;

			if(address >= pt->nBufSizeWORD) 	break;	// zone over

			strncpy(buf, (char*)&pt->commRecvBuf[4*i], 4);
			buf[4] = 0;
			PokeWORD(pt, address, atoi(buf));
		}

		for(i = 0; i < 32; i++) {
			address = 2000+(i*10)+element+1;

			if(address >= pt->nBufSizeWORD) 	break;	// zone over

			strncpy(buf, (char*)&pt->commRecvBuf[4*i], 4);
			buf[4] = 0;
			PokeWORD(pt, address, atoi(buf));
		}
	}

	int table;

	for(i = 0; i < 32; i++) {
		if(pt->nBufSizeWORD < 2320) 	break;	// zone over

		address = 2000+(i*10);

		PokeWORD(pt, address+6, (WORD)((pt->bufWORD[address+2].value-2048)/2.05));

		if(pt->bufWORD[address+4].value == pt->bufWORD[address+3].value)
			value = (float)0.0;
		else
			value = ((float)pt->bufWORD[address+1].value-(float)pt->bufWORD[address+3].value) /
					((float)pt->bufWORD[address+4].value-(float)pt->bufWORD[address+3].value);

		value *= pt->bufWORD[address+5].value;

		table = (int)fabs(value);

		table = table-1;

		if(table < 0)		table = 0;
		if(table > 255)	table = 255;

		table = GetLLT0(table);

		WORD imsi;

		if(value < 0)
			imsi = (WORD)(2048-(table*2048.0/1000.0));
		else
			imsi = (WORD)(2048+(table*2048.0/1000.0));

		if(imsi > 4095)	imsi = 4095;
		PokeWORD(pt, address+7, imsi);
	}

	WaitEndSec();
	WaitEndSec();
	WaitEndSec();
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadBL2300(LOCAL_PORT_STRUCT *pt, int /*pos*/)
{
	int  i;
	TimeOutClass timeout;
	int  mode;
	int  retn;
	static char adj_flag = OFF;
	static char init_flag = OFF;

	if(pt->nBufSizeWORD < 2320)	{
		MessageDisplay("BL-2300 모듈 설정 오류 (BUF_LENGTH >= 2320) 일 것");
		return COMMUNICATION_OK;
	}

	if(bInitFailFlag)	{
		MessageDisplay("BL-2300 모듈의 초기화 실패 (BL-2300과 AutoBase를 다시실행할 것");
		return COMMUNICATION_OK;
	}

//	PlcDeviceClear(pt->no);
	pt->commSendBuf[0] = 'D';		// plc address
	pt->commSendBuf[1] = '0';		// my address
	pt->commSendBuf[2] = CR;  		// read word
	pt->commSendBuf[3] = LF;  		// read word

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, 4);

	pt->commCountNeed = 8;			// "RD,????CR"
	pt->commCountSend = 4;
	pt->commCountCurr = 0;

	mode = BL2300ReadMode(pt);

	if(mode == RECV_CODE_CODE_BAD) {
		return COMMUNICATION_CODE_BAD;
	}
	else if(mode == RECV_CODE_TIME_OUT) {
		return COMMUNICATION_TIME_OUT;
	}
	else if(mode == RECV_CODE_INIT) {
		BL2300Init(pt);
		init_flag = ON;
	}
	else if(mode == RECV_CODE_READ) {	// 현재값이 바뀐 모드 Command
//	  retry_read:
		SendACK(pt);
		int point_hap;
		char imsi[10];
		int point;
		float value;

		strncpy(imsi, (char*)&pt->commRecvBuf[2], 3);
		imsi[3] = 0;
		i = atoi(imsi);
		if(i >= 1 && i <= 16) {
			PokeWORD(pt, 0, WORD_MASK[i-1]);
		}

		strncpy(imsi, (char*)&pt->commRecvBuf[5], 3);
		imsi[3] = 0;
		point_hap = atoi(imsi);

		if(point_hap < 0) 	point_hap = 1;
		if(point_hap > 300)	point_hap = 281;

		for(i = 0; i < point_hap; i++) {
			retn = BL2300ReadBlock(pt);
			SendACK(pt);

			if(!retn)	continue;

			strncpy(imsi, (char*)pt->commRecvBuf, 3);
			imsi[3] = 0;
			point = atoi(imsi);

			if(point < 1 || point > 281)		continue;	// point not exist
			if(point >= pt->nBufSizeWORD)  continue;	// ScanBuf 범위 초과.

			PokeWORD(pt, point, 0);

			if(strncmp((char*)&pt->commRecvBuf[3], "P-SKIP", 6) == 0) {	// point-skip
				PokeWORD(pt, point, pt->bufWORD[point].value | WORD_MASK[13]);
				continue;
			}

			if(strncmp((char*)&pt->commRecvBuf[3], "TROUBLE", 7) == 0) {	// trouble
				PokeWORD(pt, point, pt->bufWORD[point].value | WORD_MASK[10]);
				continue;
			}

			strncpy(imsi, (char*)&pt->commRecvBuf[3], 4);
			imsi[4] = 0;
			value = (float)atof(imsi);

			if(point >= 1 && point <= 200) {	// 농도 감지기.
				PokeWORD(pt, point, (WORD)(255.0*value/100.0));
			}
			else {	// 201~281
				if(value <= 1)	PokeWORD(pt, point, 0);
				else			PokeWORD(pt, point, 1);
			}
		}

		if(adj_flag) {
			adj_flag = OFF;
			BL2300SaveValueFile(pt);
		}
		WaitEndSec();
		WaitEndSec();

		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ1) {
		BL2300Adj(pt, 1);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ2) {
		BL2300Adj(pt, 2);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ3) {
		BL2300Adj(pt, 3);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ4) {
		BL2300Adj(pt, 4);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ5) {
		BL2300Adj(pt, 5);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ6) {
		BL2300Adj(pt, 6);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ADJ7) {
		BL2300Adj(pt, 7);
		adj_flag = ON;
		init_flag = ON;
	}
	else if(mode == RECV_CODE_ACK) {
		bInitFailFlag = ON;
	}
	else if(mode == RECV_CODE_BCK) {
		bInitFailFlag = ON;
	}
	else if(mode == RECV_CODE_CCK) {
		bInitFailFlag = ON;
	}
	else {
		SendACK(pt);
	}

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitBL2300(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD /*address*/, WORD /*flag*/)
{
	return COMMUNICATION_PROGRAMM_NOT_MAKED;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordBL2300(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD /*address*/, float /*value*/)
{
	return COMMUNICATION_PROGRAMM_NOT_MAKED;
}



