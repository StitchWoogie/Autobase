//------------------------------------------------------------------------------
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <afx.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>
#include <ddeml.h> 

#include <compiler.hpp>
#include <totaldef.h>
#include <tools.h>
#include <glib.h>
#include <dataswap.h>

#include "..\plc_scan.h"
#include "pro_lib.h"
#include "pro_main.h"
#include "pro_dde.h"

#define	MAX_TIME_OUT	2

static char bFlagDDE = OFF;
static DWORD idInst = 0;

typedef struct {
	char	topic[80];
	HSZ		hszTopic;
} TOPIC_LIST_STRUCT;	// 하나의 idInst에서는 같은 topic hsz을 사용한다.

Block blockTopicList(sizeof(TOPIC_LIST_STRUCT));

typedef struct {
	HSZ		hszTopic;
	HCONV	hconvTopic;
} TOPIC_HCONV_STRUCT;		// hconv는 service마다 다르므로 각 포트에 따로 정의한다.

typedef struct {
	HSZ	hszItem;
	HSZ	hszTopic;
	HCONV	hconvTopic;		// topic이 성공적으로 connect되었다.
	char	bItemInstall;	// item을 설치하는데 모든 동작이 끝나면 ON.
} LOCAL_ITEM_DDE;

//-----------------------------------------
// DDE에서는 디지털이 보통 글자로 나타난다.
//-----------------------------------------

typedef struct {
	char	*on;
	char	*off;
} DIGITAL_DDE_STRING;

#define MAX_DIGITAL_DDE 10

DIGITAL_DDE_STRING digitalDDE[MAX_DIGITAL_DDE] = {
									{ "HEAT",	"COOL"  },
									{ "I",		"O"	  },
									{ "OPEN",	"CLOSE" },
									{ "ON",		"OFF"	  },
									{ "RUN",		"STOP"  },
									{ "AUTO",	"MAN"	  },
									{ "AU",		"MA"	  },
									{ "AUT",		"MAN"	  },
									{ "OPE",		"CLO"	  },
									{ "RUN",		"STO"	  },

};

static void KillEndSpace(char *buf)
{
	int i;

	for(i = strlen(buf)-1; i >= 0; i--) {
		if(buf[i] == '\t') {
			buf[i] = 0;
		}
		else {
			break;
		}
	}
}

static void DisplayDigitalStringError(char *topic, char *item, char *buf)
{
	StackChar msg(1000);

	if(msg.data == NULL) {
		return;
	}
	
	sprintf(msg.data, "TOPIC[%s] ITEM[%s] 에서 알 수 없는 DDE 디지털 응답(%s)", topic, item, buf);
	MessageDisplay(msg.data);
}

static void ChangeBufToValue(LOCAL_PORT_STRUCT *port, SCAN_METHOD_STRUCT *sm, char *data)
{
	int i;
	CommaBlockString comma;
	char buf[80];
	
	comma.Set(data);
	comma.GetString(buf, sizeof(buf));

	int address = sm->target;

	if(address >= port->nBufSizeWORD+2) {	// buf overflow
		return;
	}

	if(sm->address == 3) {
		float value = (float)atof(buf);
		PokeValue(port, sm, address, value);
	}
	else if(sm->address == 9) {	// Auto Digital Check
		for(i = 0; i < MAX_DIGITAL_DDE; i++) {
			if(strcmp(buf, digitalDDE[i].on) == 0) {	
				if(sm->size == 1) {	// 반대.
					PokeValue(port, sm, address, 0);
				}
				else {
					PokeValue(port, sm, address, 1);
				}
				return;
			}
			else if(strcmp(buf, digitalDDE[i].off) == 0) {	
				if(sm->size == 1) {	// 반대.
					PokeValue(port, sm, address, 1);
				}
				else {
					PokeValue(port, sm, address, 0);
				}
				return;
			}
			else;
		}

		DisplayDigitalStringError(sm->sStation, sm->type, buf);
	}

	// 50번 부터는 특수한 응답일 경우 해당된다.
	else if(sm->address == 50) {	// I/O 응답 8bit	Trend의 Comms943 Server는 연속적으로 8bit의 디지탈 상태를 알려준다. 예)IIIIIIII<HT>
		for(i = 0; i < 8; i++) {
			if(buf[i] == 'I') {	// on
				//PeekWORD
				PokeWORD(port, address, PeekValueWORD(port, address) | WORD_MASK[i]);
			}
			else if(buf[i] == 'O') {
				PokeWORD(port, address, PeekValueWORD(port, address) & (0xFFFF-WORD_MASK[i]));
			}
			else {
				DisplayDigitalStringError(sm->sStation, sm->type, buf);
				break;
			}
		}
	}
	else {		// WORD
		PokeValue(port, sm, address, atoi(buf));
	}
}

static void XtypAdvData(HCONV hConv, HSZ hszTopic, HSZ hszItem, HDDEDATA hData)
{
	int i, j;
	GLOBAL_PORT_STRUCT *port;
	SCAN_METHOD_STRUCT *sm;
	char buf[80];

	if(hData == 0)	return;
		
	DdeGetData(hData, (BYTE*)buf, sizeof(buf), 0);

	//DisplayRecvString(buf);

	KillEndSpace(buf);

	for(i = 0; i < nPortHap; i++) {
		port = &portBuf[i];

		if(port->nScanProtocol != PROTOCOL_DDE) 	continue;
		if(port->local.hLocalProtocol == NULL)				continue;

		for(j = 0; j < port->local.nScanMethodHap; j++) {
			
			sm = &port->local.scanMethod[j];

			if(sm->local == NULL)	continue;

			LOCAL_ITEM_DDE *local;

			local = (LOCAL_ITEM_DDE*) sm->local;

			if(local->bItemInstall == OFF)	continue;	// item 이 연결되지 않았다.

			if(hszTopic == local->hszTopic &&
				hszItem  == local->hszItem &&
				hConv == local->hconvTopic ) {	// yes. topic item matched

				ChangeBufToValue(&port->local, sm, buf);
//				DdeFreeDataHandle(hData);
				return;
			}
		}
	}

	// DdeFreeDataHandle(hData);
}

static void DeleteMatchHCONV(LOCAL_PORT_STRUCT *pt, HCONV hConvDel)
{
	DWORD l;
	TOPIC_HCONV_STRUCT hconv;
	LOCAL_PROTOCOL_DDE *dde;

	if(pt->hLocalProtocol == NULL)	return;

	dde = (LOCAL_PROTOCOL_DDE*)GlobalLock(pt->hLocalProtocol);

	if(dde->blockHCONV == NULL) {	
		GlobalUnlock(pt->hLocalProtocol);
		return;
	}
	
	for(l = 0; l < dde->blockHCONV->GetBlockCount(); l++) {
		dde->blockHCONV->GetBlock((BYTE*)&hconv, l);
		if(hConvDel == hconv.hconvTopic) {
			dde->blockHCONV->DeleteBlock(l);	
			break;
		}
	}

	GlobalUnlock(pt->hLocalProtocol);
}

static void DisConnectHCONV(HCONV hConv)
{
	int i, j;
	GLOBAL_PORT_STRUCT *port;
	SCAN_METHOD_STRUCT *sm;

	for(i = 0; i < nPortHap; i++) {
		port = &portBuf[i];

		if(port->nScanProtocol != PROTOCOL_DDE) 	continue;
		if(port->local.hLocalProtocol == NULL)				continue;

		for(j = 0; j < port->local.nScanMethodHap; j++) {
			
			sm = &port->local.scanMethod[j];

			if(sm->local == NULL)	continue;

			LOCAL_ITEM_DDE *local;

			local = (LOCAL_ITEM_DDE*) sm->local;

			if(local->bItemInstall == OFF)	continue;	// item 이 연결되지 않았다.
			if(local->hconvTopic == 0)			continue;	// 이미 클리어 되어 있다.

			if(local->hconvTopic == hConv) {
				local->hconvTopic	= 0;	
				local->bItemInstall = OFF;
				DeleteMatchHCONV(&port->local, hConv);

			}
		}
	}
}

// REQUEST 통신 방식일때 Server에서 data를 보내주기전에 또 보내주면 server쪽에서 오류가 발생하므로
// 데이터가 들어올 때까지 기다렸다가 보내준다.

static char  bRequestingFlag = OFF;
static HCONV hConvRequesting;
static HSZ   hszRequestingItem;

// transaction processing structure - this structure is associated with
// infoctrl control windows.  A handle to this structure is placed into
// the first window word of the control.

HDDEDATA CALLBACK DdeCallbackProtocol(
	 UINT   uType,			// transaction type
	 UINT   uFmt,			// clipboard data format
	 HCONV  hconv,			// handle to the conversation
	 HSZ    hsz1,			// handle to a string
	 HSZ    hsz2,			// handle to a string
	 HDDEDATA  hdata,		// handle to a global memory object
	 DWORD  dwData1,		// transaction-specific data
	 DWORD  dwData2 		// transaction-specific data
	)
{
	switch ( uType ) {
		case XTYP_ADVDATA:
			XtypAdvData(hconv, hsz1, hsz2, hdata);
			return (HDDEDATA)DDE_FACK;
		case XTYP_XACT_COMPLETE:
			XtypAdvData(hconv, hsz1, hsz2, hdata);
			if(bRequestingFlag) {
				if(hconv == hConvRequesting &&
					hsz2  == hszRequestingItem) {
					bRequestingFlag = OFF;
				}
			}
			break;
		case XTYP_DISCONNECT:
			//DisplayRecvString("XTYP_DISCONNECT received. ");
			DisConnectHCONV(hconv);
			break;
		default:
			{
				/*
				char message[80];
				sprintf(message, "XTYP Message %d, 0x%04X", uType, uType);
				MessageDisplay(message);
				*/
			}
			break;
	}

	return ( (HDDEDATA) NULL );
}

static void PlcProtocolInitTotalDDE()
{
	if(bFlagDDE == ON)	return;	// 한번만 초기화되면 된다.
	
	if ( DdeInitialize ( (LPDWORD)&idInst, (PFNCALLBACK)DdeCallbackProtocol, APPCMD_CLIENTONLY, 0L ) != DMLERR_NO_ERROR) {
		MessageDisplay("PLC_SCAN.EXE DdeInitialize() Error");
		return;
	}

	bFlagDDE = ON;
}

void PlcProtocolUnInitTotalDDE()
{
	if(bFlagDDE == OFF)	return;

	DWORD l;
	TOPIC_LIST_STRUCT list;

	for(l = 0; l < blockTopicList.GetBlockCount(); l++) {
		blockTopicList.GetBlock((BYTE*)&list, l);
		DdeFreeStringHandle(idInst, list.hszTopic);
	}
	blockTopicList.DeleteAllBlock();

	DdeUninitialize(idInst);

	bFlagDDE = OFF;
}

int PlcProtocolInitDDE(LOCAL_PORT_STRUCT *pt)
{
	PlcProtocolInitTotalDDE();
	
	if(pt->hLocalProtocol != NULL) {
		LOCAL_PROTOCOL_DDE *dde;

		dde = (LOCAL_PROTOCOL_DDE*)GlobalLock(pt->hLocalProtocol);

		dde->hszService = DdeCreateStringHandle(idInst, dde->sService,  CP_WINANSI);
		dde->blockHCONV = new Block(sizeof(TOPIC_HCONV_STRUCT));

		GlobalUnlock(pt->hLocalProtocol);
	}

	return 1;
}

int PlcProtocolUnInitDDE(LOCAL_PORT_STRUCT *pt)
{
	SCAN_METHOD_STRUCT *sm;
	int i;
	LOCAL_ITEM_DDE *local;
	DWORD l;
	TOPIC_HCONV_STRUCT hconv;

	for(i = 0; i < pt->nScanMethodHap; i++) {
		sm = &pt->scanMethod[i];

		if(sm->local) {
			local = (LOCAL_ITEM_DDE*) sm->local;
			if(local->hszItem != 0) {
				if(PlcDeviceGetDdeMethod(&pt->device) == 1) {	// DDE advise 방식
					if(local->bItemInstall) {	// ADVSTART가 설치되었을때.
						DWORD dwResult;
						DdeClientTransaction(NULL, 0, local->hconvTopic, local->hszItem, CF_TEXT, XTYP_ADVSTOP, TIMEOUT_ASYNC, &dwResult);										
					}
				}
				DdeFreeStringHandle(idInst, local->hszItem);
			}
			delete (LOCAL_ITEM_DDE*)sm->local;
		}
	}
	
	if(pt->hLocalProtocol != NULL) {
		LOCAL_PROTOCOL_DDE *dde;

		dde = (LOCAL_PROTOCOL_DDE*)GlobalLock(pt->hLocalProtocol);
		DdeFreeStringHandle(idInst, dde->hszService);
		for(l = 0; l < dde->blockHCONV->GetBlockCount(); l++) {
			dde->blockHCONV->GetBlock((BYTE*)&hconv, l);
			DdeDisconnect(hconv.hconvTopic);
		}
		delete dde->blockHCONV;

		GlobalUnlock(pt->hLocalProtocol);
	}

	return 1;
}

void PlcScanDrawMethodTitleDDE(HDC hdc, int x, int y)
{
	int pos = 0;
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;

	wsprintf(buf, "Topic,");
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
	pos += 17; 

	wsprintf(buf, "Item,");
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
	pos += 15; 

	wsprintf(buf, "Data Type,");
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
	pos += 12; 

	wsprintf(buf, "buf,");
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
	pos += 5; 

	wsprintf(buf, "[DDE Client]");
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
}

void PlcScanDrawMethodDDE(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;
	int pos = 0;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;
	wsprintf(buf, "%-s", sm->sStation);
	TextOut(hdc, x, y, buf, strlen(buf));
	pos += 17;

	wsprintf(buf, "%-s", sm->type);
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
	pos += 15; 

	wsprintf(buf, "%-d", sm->address);
	if(sm->address == 1) {
		strcat(buf, "=HiBYTE");
	}
	else if(sm->address == 2) {
		strcat(buf, "=LoBYTE");
	}
	else if(sm->address == 3) {
		strcat(buf, "=Float");
	}
	else if(sm->address == 9) {
		strcat(buf, "=Digital");
	}
	else if(sm->address == 50) {
		strcat(buf, "=I/O 8bit");
	}
	else {
		strcat(buf, "=WORD");
	}
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));

	pos += 12;

	wsprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*pos, y, buf, strlen(buf));
}

static HSZ TopicInsert(char *topic)
{
	DWORD l;
	TOPIC_LIST_STRUCT list;

	for(l = 0; l < blockTopicList.GetBlockCount(); l++) {
		blockTopicList.GetBlock((BYTE*)&list, l);
		if(strcmp(topic, list.topic) == 0)	return list.hszTopic;
	}

	strcpy(list.topic, topic);
	list.hszTopic = DdeCreateStringHandle(idInst, topic, CP_WINANSI);	

	if(list.hszTopic == 0)	return 0;

	blockTopicList.AddBlock((BYTE*)&list);

	return list.hszTopic;
}

static HCONV TopicConnection(LOCAL_PORT_STRUCT *pt, HSZ topic)
{
	DWORD l;
	TOPIC_HCONV_STRUCT hconv;
	LOCAL_PROTOCOL_DDE *dde;

	if(pt->hLocalProtocol == NULL)	return 0;

	dde = (LOCAL_PROTOCOL_DDE*)GlobalLock(pt->hLocalProtocol);

	if(dde->blockHCONV == NULL) {	
		GlobalUnlock(pt->hLocalProtocol);
		return 0;
	}
	
	for(l = 0; l < dde->blockHCONV->GetBlockCount(); l++) {
		dde->blockHCONV->GetBlock((BYTE*)&hconv, l);
		if(topic == hconv.hszTopic) {
			GlobalUnlock(pt->hLocalProtocol);
			return hconv.hconvTopic;
		}
	}

	hconv.hszTopic = topic;
	hconv.hconvTopic = DdeConnect(idInst, dde->hszService, topic, NULL);
	
	if(hconv.hconvTopic == 0) {
		GlobalUnlock(pt->hLocalProtocol);
		return 0;
	}

	dde->blockHCONV->AddBlock((BYTE*)&hconv);

	GlobalUnlock(pt->hLocalProtocol);

	return hconv.hconvTopic;
}

//----------------------------------------------------------------------------------------
//	Scan Method에 있는 방법대로 읽어서 버퍼에 저장한다.
//	DDE방식이 REQUEST와 ADVISE  DDE에서는 읽는것이 아니고 topic과, item을 설치하기만 한다.
//----------------------------------------------------------------------------------------

static int PlcScanReadDDERequest(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	HDDEDATA hData;
	char message[160];
	LOCAL_ITEM_DDE *local;

	if(sm->local == NULL) {
		sm->local = (HANDLE)new LOCAL_ITEM_DDE[1];
		if(sm->local == NULL) {
			PlcScanSetErrorString("DDE local 변수 할당 도중 메모리 부족");
			return COMMUNICATION_ERR_STRING;
		}
		local = (LOCAL_ITEM_DDE*) sm->local;

		memset(local, 0, sizeof(LOCAL_ITEM_DDE));
		local->hszItem = 0;
		local->hszTopic = 0;
		local->hconvTopic = 0;
		local->bItemInstall = OFF;
	}

	local = (LOCAL_ITEM_DDE*) sm->local;

	if(local->bItemInstall) {	// item이 성공적으로 설치되어 있으므로 통신을 해본다.	
		if(pt->bReadingFlag) {
			if(bRequestingFlag) { 
				if(pt->timeout->IsTimeOut(60)) {
					pt->timeout->Reset();
					bRequestingFlag = OFF;
					pt->bReadingFlag = OFF;

					sprintf(message, "Topic[%s] Item[%s] REQUEST timeout", sm->sStation, sm->type);
					MessageDisplay(message);

					return COMMUNICATION_OK;
				}
				return COMMUNICATION_WAITING;	// 통신 대기중이다.
			}
			else {
				pt->bReadingFlag = OFF;
				pt->timeout->Reset();
				return COMMUNICATION_OK;
			}
		}

		sprintf(message, "REQUEST %s|%s...", sm->sStation, sm->type);
		//DisplaySendString(message);

		DWORD dwResult;
	
		hData = DdeClientTransaction(NULL, 0, local->hconvTopic, local->hszItem, CF_TEXT, XTYP_REQUEST, TIMEOUT_ASYNC, &dwResult);

		if(hData == 0) {
			sprintf(message, "Topic[%s] Item[%s] REQUEST return 0", sm->sStation, sm->type);	
			MessageDisplay(message);

			return COMMUNICATION_OK;
		}
		else {
			pt->bReadingFlag = ON;
			bRequestingFlag = ON;
			hConvRequesting = local->hconvTopic;
			hszRequestingItem = local->hszItem;

			return COMMUNICATION_WAITING;
		}
	}

	if(local->hszItem == 0) {
		local->hszItem = DdeCreateStringHandle(idInst, sm->type, CP_WINANSI);	
		if(local->hszItem == 0) {
			PlcScanSetErrorString("DDE item 할당 도중 메모리 부족");
			return COMMUNICATION_ERR_STRING;
		}
	}
	
	if(local->hszTopic == 0) {
		local->hszTopic = TopicInsert(sm->sStation);
		if(local->hszTopic == 0) {
			PlcScanSetErrorString("DDE Topic 할당 도중 메모리 부족");
			return COMMUNICATION_ERR_STRING;
		}
	}

	if(local->hconvTopic == 0) {
		local->hconvTopic = TopicConnection(pt, local->hszTopic);
		if(local->hconvTopic == 0) {
			PlcScanSetErrorString("DDE Topic[%s] 접속 불가 (DDE Server not found.)", sm->sStation);
			return COMMUNICATION_ERR_STRING;
		}
	}
	
	local->bItemInstall = ON;

	sprintf(message, "Topic[%s] Item[%s] Install O.K", sm->sStation, sm->type);

	//DisplaySendString(message);
	
	return COMMUNICATION_OK;
}

static int PlcScanReadDDEAdvise(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	HDDEDATA hData;
	DWORD dwResult;
	char message[80];
	LOCAL_ITEM_DDE *local;

	if(sm->local == NULL) {
		sm->local = (HANDLE)new LOCAL_ITEM_DDE[1];
		if(sm->local == NULL) {
			PlcScanSetErrorString("DDE local 변수 할당 도중 메모리 부족");
			return COMMUNICATION_ERR_STRING;
		}
		local = (LOCAL_ITEM_DDE*) sm->local;

		memset(local, 0, sizeof(LOCAL_ITEM_DDE));
		local->hszItem = 0;
		local->hszTopic = 0;
		local->hconvTopic = 0;
		local->bItemInstall = OFF;
	}

	local = (LOCAL_ITEM_DDE*) sm->local;

	if(local->bItemInstall)	return COMMUNICATION_OK;	// item이 성공적으로 설치되어 있다.

	if(local->hszItem == 0) {
		local->hszItem = DdeCreateStringHandle(idInst, sm->type, CP_WINANSI);	
		if(local->hszItem == 0) {
			PlcScanSetErrorString("DDE item 할당 도중 메모리 부족");
			return COMMUNICATION_ERR_STRING;
		}
	}
	
	if(local->hszTopic == 0) {
		local->hszTopic = TopicInsert(sm->sStation);
		if(local->hszTopic == 0) {
			PlcScanSetErrorString("DDE Topic 할당 도중 메모리 부족");
			return COMMUNICATION_ERR_STRING;
		}
	}

	if(local->hconvTopic == 0) {
		local->hconvTopic = TopicConnection(pt, local->hszTopic);
		if(local->hconvTopic == 0) {
			PlcScanSetErrorString("DDE Topic[%s] 접속 불가 (DDE Server not found.)", sm->sStation);
			return COMMUNICATION_ERR_STRING;
		}
	}
	
	hData = DdeClientTransaction(NULL, 0, local->hconvTopic, local->hszItem, CF_TEXT, XTYP_ADVSTART, TIMEOUT_ASYNC, &dwResult);

	if(hData == 0) {
		PlcScanSetErrorString("DDE Item [%s] ADVSTART UnSucess.", sm->type);
		return COMMUNICATION_ERR_STRING;
	}

	local->bItemInstall = ON;

	sprintf(message, "Waiting XTYP_ADVDATA of item [%s]. ", sm->type);

	// DisplaySendString(message);
	
	return COMMUNICATION_OK;
}

int PlcScanReadDDE(LOCAL_PORT_STRUCT *pt, int pos)
{
	if(PlcDeviceGetDdeMethod(&pt->device) == 0) {
		return PlcScanReadDDERequest(pt, pos);		
	}
	else {
		return PlcScanReadDDEAdvise(pt, pos);		
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitDDE(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD address, WORD flag, char *device)
{
/*
	int j;
	PORT_STRUCT *port;
	SCAN_METHOD_STRUCT *sm;
	char buf[80];
	char msg[160];
	CommaBlockString comma;

	port = &portBuf[port_no];

	for(j = 0; j < port->nScanMethodHap; j++) {
		sm = &port->scanMethod[j];

		if((address/16)*10+(address%16) != sm->address)  continue;	// write는 address와 scanbuf의 adress가 일치하여야 한다.
		
		if(sm->local == NULL)	continue;

		HDDEDATA hData, hDataRetn;
		DWORD dwResult;
		//UINT retn;

		if(flag)	strcpy(buf, "ON");
		else		strcpy(buf, "OFF");

		hData = DdeCreateDataHandle(idInst, (BYTE*)buf, strlen(buf), 0, 0, 0, 0);
		hDataRetn = DdeClientTransaction((LPBYTE)hData, 0xFFFFFFFFL, hConvPoint, (HSZ)sm->local, CF_TEXT, XTYP_POKE, TIMEOUT_ASYNC, &dwResult);
		DdeFreeDataHandle(hData);

		if(hDataRetn == 0) {
			sprintf(msg, "DDE DDE Item Write Bit [%s] UnSucess", sm->type);
			MessageDisplay(msg);
			return COMMUNICATION_OK;
		}

		return COMMUNICATION_OK;
	}
	
*/
	return COMMUNICATION_OK;
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordDDE(LOCAL_PORT_STRUCT *pt, int /*station*/, WORD address, float value, char *device)
{
/*
	int j;
	PORT_STRUCT *port;
	SCAN_METHOD_STRUCT *sm;
	char buf[80];
	char msg[160];
	CommaBlockString comma;

	port = &portBuf[port_no];

	for(j = 0; j < port->nScanMethodHap; j++) {
		sm = &port->scanMethod[j];

		if((address/16)*10+(address%16) != sm->address)  continue;	// write는 address와 scanbuf의 adress가 일치하여야 한다.
		if(sm->local == NULL)	continue;

		HDDEDATA hData, hDataRetn;
		DWORD dwResult;
		//UINT retn;

		sprintf(buf, "%f", value);

		hData = DdeCreateDataHandle(idInst, (BYTE*)buf, strlen(buf), 0, (HSZ)sm->local, CF_TEXT, HDATA_APPOWNED);
//		if(DdeClientTransaction((LPBYTE)hDDE, 0xFFFFFFFF, hConvWrite, 0, 0, XTYP_EXECUTE, TIMEOUT_ASYNC, &dwResult) == 0) {
		hDataRetn = DdeClientTransaction((LPBYTE)hData, -1, hConvPoint, (HSZ)sm->local, CF_TEXT, XTYP_POKE, TIMEOUT_ASYNC, &dwResult);
		DdeFreeDataHandle(hData);

		if(hDataRetn == 0) {
			sprintf(msg, "DDE DDE Item Write Word [%s] UnSucess error-%04Xh", sm->type, DdeGetLastError(idInst));
			MessageDisplay(msg);
			return COMMUNICATION_OK;
		}

      return COMMUNICATION_OK;
	}
*/

	return COMMUNICATION_OK;
}






