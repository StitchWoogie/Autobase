// english O.K
 
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <totaldef.h>
#include <tools.h>
#include <crc.hpp>
#include <glib.h>
#include <dataswap.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"
#include "..\..\catlib.src\NetworkProtocol.h"


#define	LOCAL_RECV_BUF	5000

typedef struct {
	TimeOutClass timeout;
	WORD	wActPort[16];
	BYTE	recvBuf[LOCAL_RECV_BUF];
} LOCAL_VARS_STRUCT;

#define localVars ((LOCAL_VARS_STRUCT*)pt->hLocalProtocol)

void PlcProtocolInitNetClientMulti(LOCAL_PORT_STRUCT *port);

static void SendLifeSignal(LOCAL_PORT_STRUCT *pt)
{
	char imsi[16*4+1];
	int i;

	for(i = 0; i < 16; i++) {
		sprintf(&imsi[i*4], "%04X", localVars->wActPort[i]);
	}
	sprintf((char*)pt->commSendBuf, "Port=%d,BroadCastPorts=%s,Version=8.7", pt->no, imsi);  // port no

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_PORT_LIFE_SIGNAL_MULTI, 0, (char*)pt->commSendBuf, strlen((char*)pt->commSendBuf));
		
	PlcDeviceWriteContinue(&pt->device, send.bufSend, send.nBufCount);

	DisplaySendCodeNextLine(pt->no);
}

static void SendDisConnectSignal(LOCAL_PORT_STRUCT *pt)
{
	char imsi[16*4+1];
	int i;

	for(i = 0; i < 16; i++) {
		sprintf(&imsi[i*4], "%04X", localVars->wActPort[i]);
	}

	sprintf((char*)pt->commSendBuf, "Port=%d,BroadCastPorts=%s,", pt->no, imsi);  // port no

	NetWorkProtocolSend send;
	send.MakeBlock(COMMAND_PLCSCAN_PORT_DISCONNECT_SIGNAL, 0, (char*)pt->commSendBuf, strlen((char*)pt->commSendBuf));
		
	PlcDeviceWriteContinue(&pt->device, send.bufSend, send.nBufCount);

	DisplaySendCodeNextLine(pt->no);
}

void ProtocolNetClientMultiGetOption(WORD *wActPort, char *option)
{
	ZeroMemory(wActPort, sizeof(WORD)*16);
	
	CommaBlockString comma;
	CommaBlockString devide;
	char imsi[80];
	int from;
	int to;
	int i;

	devide.SetBlockCode(':');
	comma.Set(option);

	while(1) {
		comma.GetString(imsi, sizeof(imsi));
		if(strlen(imsi) == 0)	break;
		
		devide.Set(imsi);
		devide.GetInt(from);
		devide.GetInt(to);

		if(from < 0)	from = 0;
		if(from > 255)	from = 255;
		if(to < from)	to = from;
		if(to > 255)	to = 255;

		for(i = from; i <= to; i++) {
			wActPort[i/16] |= WORD_MASK[i%16];
		}
	}
}

void PlcProtocolInitNetClientMulti(LOCAL_PORT_STRUCT *pt)
{
	pt->hLocalProtocol = (HGLOBAL) new LOCAL_VARS_STRUCT;

	ZeroMemory(localVars, sizeof(LOCAL_VARS_STRUCT));

	ProtocolNetClientMultiGetOption(localVars->wActPort, pt->sScanProtocolOption);

	SendLifeSignal(pt);

	pt->timeout->Reset();
}

void PlcProtocolUnInitNetClientMulti(LOCAL_PORT_STRUCT *pt)
{
	SendDisConnectSignal(pt);
	delete localVars;
}

void PlcScanDrawMethodTitleNetClientMulti(HDC hdc, int x, int y)
{
	char *string = "station, type, address, buf address, read size (Network Client Multi)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodNetClientMulti(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
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

static void PokeValueBlockWORD(LOCAL_PORT_STRUCT *pt, NetWorkProtocolRecv *split)
{
	int j;
	int block_count;

	if(split->nBlockSize == 0)	return;

	block_count = split->nBlockSize/sizeof(NETWORK_PROTOCOL_BLOCK_WORD);

	// if not 0 struct changed
	if((split->nBlockSize%sizeof(NETWORK_PROTOCOL_BLOCK_WORD)) != 0)	return;

	if(block_count == 0)	return;

	NETWORK_PROTOCOL_BLOCK_WORD *block = (NETWORK_PROTOCOL_BLOCK_WORD*)split->Block;

	if(block == NULL) return;

	for(j = 0; j < block_count; j++) {
		PokeWORD(pt, block[j].address,  block[j].value);
	}
}

static void PokeValueBlockDWORD(LOCAL_PORT_STRUCT *pt, NetWorkProtocolRecv *split)
{
	int j;
	int block_count;

	if(split->nBlockSize == 0)	return;

	block_count = split->nBlockSize/sizeof(NETWORK_PROTOCOL_BLOCK_DWORD);

	// if not 0 struct changed
	if((split->nBlockSize%sizeof(NETWORK_PROTOCOL_BLOCK_DWORD)) != 0)	return;

	if(block_count == 0)	return;

	NETWORK_PROTOCOL_BLOCK_DWORD *block = (NETWORK_PROTOCOL_BLOCK_DWORD*)split->Block;

	if(block == NULL) return;

	for(j = 0; j < block_count; j++) {
		PokeDWORD(pt, block[j].address,  block[j].value);
	}
}

static void PokeValueBlockFLOAT(LOCAL_PORT_STRUCT *pt, NetWorkProtocolRecv *split)
{
	int j;
	int block_count;

	if(split->nBlockSize == 0)	return;

	block_count = split->nBlockSize/sizeof(NETWORK_PROTOCOL_BLOCK_FLOAT);

	// if not 0 struct changed
	if((split->nBlockSize%sizeof(NETWORK_PROTOCOL_BLOCK_FLOAT)) != 0)	return;

	if(block_count == 0)	return;

	NETWORK_PROTOCOL_BLOCK_FLOAT *block = (NETWORK_PROTOCOL_BLOCK_FLOAT*)split->Block;

	if(block == NULL) return;

	for(j = 0; j < block_count; j++) {
		PokeFLOAT(pt, block[j].address,  block[j].value);
	}
}

static void PokeValueBlockDOUBLE(LOCAL_PORT_STRUCT *pt, NetWorkProtocolRecv *split)
{
	int j;
	int block_count;

	if(split->nBlockSize == 0)	return;

	block_count = split->nBlockSize/sizeof(NETWORK_PROTOCOL_BLOCK_DOUBLE);

	// if not 0 struct changed
	if((split->nBlockSize%sizeof(NETWORK_PROTOCOL_BLOCK_DOUBLE)) != 0)	return;

	if(block_count == 0)	return;

	NETWORK_PROTOCOL_BLOCK_DOUBLE *block = (NETWORK_PROTOCOL_BLOCK_DOUBLE*)split->Block;

	if(block == NULL) return;

	for(j = 0; j < block_count; j++) {
		PokeDOUBLE(pt, block[j].address,  block[j].value);
	}
}

static void PokeValueBlockINT64(LOCAL_PORT_STRUCT *pt, NetWorkProtocolRecv *split)
{
	int j;
	int block_count;

	if(split->nBlockSize == 0)	return;

	block_count = split->nBlockSize/sizeof(NETWORK_PROTOCOL_BLOCK_INT64);

	// if not 0 struct changed
	if((split->nBlockSize%sizeof(NETWORK_PROTOCOL_BLOCK_INT64)) != 0)	return;

	if(block_count == 0)	return;

	NETWORK_PROTOCOL_BLOCK_INT64 *block = (NETWORK_PROTOCOL_BLOCK_INT64*)split->Block;

	if(block == NULL) return;

	for(j = 0; j < block_count; j++) {
		PokeINT64(pt, block[j].address,  block[j].value);
	}
}

void SendCodeACK(LOCAL_PORT_STRUCT *pt, WORD trans)
{
	NetWorkProtocolSend send;

	send.MakeBlock(COMMAND_ACK, trans, NULL, 0);

	PlcDeviceWriteContinue(&pt->device, send.bufSend, send.nBufCount);

	DisplaySendCodeNextLine(pt->no);
}

static void PokeVariousValue(LOCAL_PORT_STRUCT *pt_gate, NetWorkProtocolRecv &recv)
{
	if(recv.nPort >= nPortHap)	return;	// port over
	
	LOCAL_PORT_STRUCT *pt = &portBuf[recv.nPort].local;

	if(recv.wCommand == COMMAND_PLCSCAN_VALUE_WORD) {
		PokeWORD(pt, (WORD)recv.dwAddress, (WORD)recv.fValue);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_WORD_BLOCK) {
		SendCodeACK(pt_gate, recv.wTransaction);
		PokeValueBlockWORD(pt, &recv);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_DWORD) {	// DWORD
		PokeDWORD(pt, (WORD)recv.dwAddress, (DWORD)recv.fValue);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_DWORD_BLOCK) {
		SendCodeACK(pt_gate, recv.wTransaction);
		PokeValueBlockDWORD(pt, &recv);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_FLOAT) {	// Float
		PokeFLOAT(pt, (WORD)recv.dwAddress, (float)recv.fValue);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_FLOAT_BLOCK) {
		SendCodeACK(pt_gate, recv.wTransaction);
		PokeValueBlockFLOAT(pt, &recv);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_STRING) {	// String
		char buf[256];
		if(recv.nBlockSize < 256) {
			memcpy(buf, recv.Block, recv.nBlockSize);
			buf[recv.nBlockSize] = 0;
			PokeSTRING(pt, (WORD)recv.dwAddress, buf);
		}
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_DOUBLE) {	// double
		PokeDOUBLE(pt, (WORD)recv.dwAddress, recv.fValue);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_DOUBLE_BLOCK) {
		SendCodeACK(pt_gate, recv.wTransaction);
		PokeValueBlockDOUBLE(pt, &recv);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_INT64) {	// INT64
		PokeINT64(pt, (WORD)recv.dwAddress, recv.i64Value);
	}
	else if(recv.wCommand == COMMAND_PLCSCAN_VALUE_INT64_BLOCK) {
		SendCodeACK(pt_gate, recv.wTransaction);
		PokeValueBlockINT64(pt, &recv);
	}
	else;
}


static int StatusRead(LOCAL_PORT_STRUCT *pt)
{
	int count;
	char one_flag = OFF;	// 하나이상의 명령어가 들어오면 ON;
	StackChar read(LOCAL_RECV_BUF);

	count = PlcDeviceReadContinue(&pt->device, read.data, LOCAL_RECV_BUF);

	if(count == 0) {
		pt->bReadingFlag = ON;
		return COMMUNICATION_WAITING;
	}

	for(int i = 0; i < count; i++) {
		if(pt->commCountCurr >= LOCAL_RECV_BUF) {
			pt->commCountCurr = 0;
		}
		if(read.data[i] == STX) {
			pt->commCountCurr = 0;
		}

		localVars->recvBuf[pt->commCountCurr] = read.data[i];

		pt->commCountCurr += 1;

		if(pt->commCountCurr >= LOCAL_RECV_BUF) {
			pt->commCountCurr = 0;
		}

		// 필요한 바이트 수만큼 데이터를 모두 받았다.
		if(read.data[i] == ETX && pt->commCountCurr > 6) {
			DisplayRecvCodeNextLine(pt->no);

			NetWorkProtocolRecv recv;

			if(!recv.Split(localVars->recvBuf, pt->commCountCurr)) {
				pt->commCountCurr = 0;
				continue;
			}
		
			PokeVariousValue(pt, recv);

			pt->commCountCurr = 0;

			pt->timeout->Reset();
			one_flag = ON;
			
		}
	}

	if(one_flag) {
		return COMMUNICATION_OK;
	}
	else {
		pt->bReadingFlag = ON;
		return COMMUNICATION_WAITING;
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadNetClientMulti(LOCAL_PORT_STRUCT *pt, int pos)
{
	if(localVars->timeout.IsTimeOut(10)) {	// 10초 마다 1번씩 connect 시그널을 보낸다.
		localVars->timeout.Reset();
		SendLifeSignal(pt);
	}

	if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ)) {
		pt->timeout->Reset();
		return COMMUNICATION_TIME_OUT;
	}

	return StatusRead(pt);
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitNetClientMulti(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, WORD pannel, int write_port)
{
	char imsi[80];
	WORD trans = GetTransaction();

	//PlcDeviceClear(&pt->device);

	sprintf(imsi, "Port=%d,", write_port);
	strcpy((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Station=%d,", station);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Address=%d,", address);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Value=%d,", flag);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Extra1=%s,", device);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Extra2=%d,", pannel);
	strcat((char*)pt->commSendBuf, imsi);

	NetWorkProtocolSend send;

	send.MakeBlock(COMMAND_PLCSCAN_WRITE_BIT, trans, (char*)pt->commSendBuf, strlen((char*)pt->commSendBuf));

	PlcDeviceWriteContinue(&pt->device, send.bufSend, send.nBufCount);

	pt->commCountNeed = 14; // STX+COMMAND+STATION+ADDRESS+CRC+ETX
	pt->commCountSend = send.nBufCount;
	pt->commCountCurr = 0;

	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordNetClientMulti(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, WORD pannel, int write_port)
{
	char imsi[80];
	WORD trans = GetTransaction();

	//PlcDeviceClear(&pt->device);

	sprintf(imsi, "Port=%d,", write_port);
	strcpy((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Station=%d,", station);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Address=%d,", address);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Value=%.5E,", value);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Extra1=%s,", device);
	strcat((char*)pt->commSendBuf, imsi);
	sprintf(imsi, "Extra2=%d,", pannel);
	strcat((char*)pt->commSendBuf, imsi);

	NetWorkProtocolSend send;

	send.MakeBlock(COMMAND_PLCSCAN_WRITE_WORD, trans, (char*)pt->commSendBuf, strlen((char*)pt->commSendBuf));

	PlcDeviceWriteContinue(&pt->device, send.bufSend, send.nBufCount);

	pt->commCountNeed = 14; // STX+COMMAND+STATION+ADDRESS+CRC+ETX
	pt->commCountSend = send.nBufCount;
	pt->commCountCurr = 0;

	return COMMUNICATION_OK;
}





