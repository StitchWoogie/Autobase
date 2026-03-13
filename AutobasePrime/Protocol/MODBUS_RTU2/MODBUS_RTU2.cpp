// MODBUS_RTU2.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "MODBUS_RTU2.h"
#include <stdlib.h>
#include <string.h>
#include <totaldef.h>
#include <tools.h>
#include <glib.h>
#include <crc.hpp>
#include "..\..\PLC_SCAN\plc_scan.h"
#include "../dll_lib/dll_lib.h"
#include "..\..\catlib.src\totalcfg.h"
#include "DialogProtocolOption.h"
//#include "..\..\catdll\catdll.hpp"    //delete 2025-07-09
#include "..\..\catlib.src\CatTag9.h"		// add 2007-08-30
#include <comutil.h>

#define MAX_MODBUS_RTU_SEND_BUF	2048   // add 2011-04-27

char sWorkDir[MAXPATH];

typedef struct {
	char	tag[41];
} TAG_ONE;



typedef struct {	
	bool	bCheckCrc;
	bool	bReadSizeflag;
	bool	bFloatData;
	bool	bReadValueFloat;
	bool	bTcp;			// add 2014-10-06
	bool	bUseDigitalMemory;// add 2015-03-20 for 1, 2 function tag
	BYTE	cStation;
	//BYTE	cMasterStation;
	int		nStartAddr;
	int		nReadSize;
	WORD	nCommand;	
	TAG_ONE *tagOne;//[MAX_SEND_TAG_COUNT];// changed 2015-03-20 tagOne[MAX_SEND_TAG_COUNT] -> *tagOne
	TAG_ONE *tagOne2;					// add 2015-03-20
	WORD	nReadTagSize;
	WORD	nReadTagSize2;				// add 2015-03-20
	bool	bVersion9;			// autobase 9.xx 이상인가?, add 2007-08-30
	//bool	bAddrDouble;		// float/dword 데이터일때 address *2 를 할 것인가를 설정, add 2010-05-27
	WORD	nSendStartAddr;		// 보낼 modbus 시작 주소, add 2011-04-27	
	WORD	nSendStartAddrCoil;	// add 2015-03-20 for 1, 2 function tag
	BYTE	sendBuf[MAX_MODBUS_RTU_SEND_BUF];// add 2011-04-27
} LOCAL_VARS_STRUCT;

#define localVars ((LOCAL_VARS_STRUCT*)pt->hLocalProtocol)


int Tag9GetCurr(const char *tag, CString &curr);		// add 2007-08-30
bool TagSetCurr(const char *tag, double val);			// add 2015-03-09

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//	Note!
//
//		If this DLL is dynamically linked against the MFC
//		DLLs, any functions exported from this DLL which
//		call into MFC must have the AFX_MANAGE_STATE macro
//		added at the very beginning of the function.
//
//		For example:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// normal function body here
//		}
//
//		It is very important that this macro appear in each
//		function, prior to any calls into MFC.  This means that
//		it must appear as the first statement within the 
//		function, even before any object variable declarations
//		as their constructors may generate calls into the MFC
//		DLL.
//
//		Please see MFC Technical Notes 33 and 58 for additional
//		details.
//

// CMODBUS_RTUApp

BEGIN_MESSAGE_MAP(CMODBUS_RTUApp, CWinApp)
END_MESSAGE_MAP()


// CMODBUS_RTUApp construction

CMODBUS_RTUApp::CMODBUS_RTUApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}


// The one and only CMODBUS_RTUApp object

CMODBUS_RTUApp theApp;


// CMODBUS_RTUApp initialization

BOOL CMODBUS_RTUApp::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

void ProcProtocolGetDriverTitle(char *title)
{
	strcpy(title, "MODBUS-RTU Mode 2");
}


int getRegisteredTagLineCount(BYTE ptNo, int memPos)	// add 2015-03-20
{
	FILE				*in;
	CommaBlockString	comma;
	char				buf[200], imsi[41], filename[MAXPATH];
	int					pos;

	if(memPos == 1)
		sprintf(filename, "%s\\SCAN\\READTAG_COIL%03d.txt", sWorkDir, ptNo);
	else
		sprintf(filename, "%s\\SCAN\\READTAG%03d.txt", sWorkDir, ptNo);
	in = fopen(filename, "rb");
	if(in == NULL) return 0;

	pos = 0;
	while(TextGetOneLine(in, buf, sizeof(buf))) {
		comma.Set(buf);
		comma.GetString(imsi, sizeof(imsi));
		comma.GetString(imsi, sizeof(imsi));
		if(strlen(imsi) <= 0) continue;		// 태그명이 없으면 ....

		pos++;
		if(pos > MAX_SEND_TAG_COUNT) break;
	}
	fclose(in);	
	return pos;	
}

void readRegisteredTag(LOCAL_PORT_STRUCT *pt)
{	
	localVars->nReadTagSize = getRegisteredTagLineCount((BYTE)pt->no, 0);	// add 2015-03-20
	if(localVars->nReadTagSize <= 0) return;								// add 2015-03-20
	localVars->tagOne = new TAG_ONE[localVars->nReadTagSize];				// add 2015-03-20

	FILE				*in;
	CommaBlockString	comma;
	int					pos;
	char				buf[200], imsi[41], filename[MAXPATH];

	sprintf(filename, "%s\\SCAN\\READTAG%03d.txt", sWorkDir, (BYTE)pt->no);
	in = fopen(filename, "rb");
	if(in == NULL) {
		localVars->nReadTagSize = 0;									// add 2015-03-20
		return;
	}

	pos = 0;
	while(TextGetOneLine(in, buf, sizeof(buf))) {
		comma.Set(buf);
		comma.GetString(imsi, sizeof(imsi));
		comma.GetString(imsi, sizeof(imsi));
		if(strlen(imsi) <= 0) continue;		// 태그명이 없으면 ....
		strcpy(localVars->tagOne[pos].tag, imsi);
		pos++;
		if(pos >= localVars->nReadTagSize) break;						// add 2015-03-20
		if(pos >= MAX_SEND_TAG_COUNT) break;	// 1000 -> MAX_SEND_TAG_COUNT 로 수정, 2005-09-13, changed 2015-03-20 > -> >=
	}
	fclose(in);	
	//localVars->nReadTagSize = pos;	// deleted 2015-03-20
}

void readRegisteredTag2(LOCAL_PORT_STRUCT *pt)// add 2015-03-20
{
	localVars->nReadTagSize2 = getRegisteredTagLineCount((BYTE)pt->no, 1);
	if(localVars->nReadTagSize2 <= 0) return;
	localVars->tagOne2 = new TAG_ONE[localVars->nReadTagSize2];

	FILE				*in;
	CommaBlockString	comma;
	int					pos;
	char				buf[200], imsi[41], filename[MAXPATH];

	sprintf(filename, "%s\\SCAN\\READTAG_COIL%03d.txt", sWorkDir, (BYTE)pt->no);
	in = fopen(filename, "rb");
	if(in == NULL) {
		localVars->nReadTagSize2 = 0;
		return;
	}

	pos = 0;
	while(TextGetOneLine(in, buf, sizeof(buf))) {
		comma.Set(buf);
		comma.GetString(imsi, sizeof(imsi));
		comma.GetString(imsi, sizeof(imsi));
		if(strlen(imsi) <= 0) continue;		// 태그명이 없으면 ....
		strcpy(localVars->tagOne2[pos].tag, imsi);
		pos++;
		if(pos >= localVars->nReadTagSize2) break;
		if(pos >= MAX_SEND_TAG_COUNT) break;
	}
	fclose(in);
}


static void getInitParameter(LOCAL_PORT_STRUCT *pt)
{
	CommaBlockString	comma;
	BYTE				i;

	if(strlen(pt->sScanProtocolOption) > 0) {
		comma.Set(pt->sScanProtocolOption);

		comma.GetBYTE(localVars->cStation);
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);
		localVars->bFloatData = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);
		localVars->bReadValueFloat = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);								// add 2007-08-30
		localVars->bVersion9 =  true; // ( i == 1) ? true : false;   //  2025-07-09
		if(comma.IsEOS()) return;

		comma.GetWORD(localVars->nSendStartAddr);		// add 2011-04-27
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);								// add 2014-10-06
		localVars->bTcp = ( i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);								// add 2015-03-20 for 1, 2 function tag
		localVars->bUseDigitalMemory = ( i == 1) ? true : false;
		if(comma.IsEOS()) return;		

		comma.GetWORD(localVars->nSendStartAddrCoil);	// add 2015-03-20 for 1, 2 function tag
		if(comma.IsEOS()) return;
	}
}

void ProcProtocolInit(HWND hwnd, LOCAL_PORT_STRUCT *pt)
{
	//CommaBlockString	comma;
	//BYTE				i;

	pt->hLocalProtocol = (HGLOBAL) new LOCAL_VARS_STRUCT;
	memset(localVars, 0, sizeof(LOCAL_VARS_STRUCT));

	AutoBaseIniGetProjectDirectory(sWorkDir);
	pt->commCountCurr = 0;
	pt->commCountNeed = (localVars->bTcp) ? 12 : 8;// add 2014-10-07, localVars->bTcp
	localVars->bReadSizeflag = false;
	localVars->cStation = 0;
	localVars->nReadTagSize = 0;
	localVars->nReadTagSize2 = 0;	// add 2015-03-20
	localVars->bFloatData = false;
	localVars->bReadValueFloat = false;
	//localVars->bAddrDouble = false;// add 2010-05-27
	localVars->nSendStartAddr = 0; // add 2011-04-27
	localVars->bTcp = false;		// add 2014-10-06
	localVars->bUseDigitalMemory = false;// add 2015-03-20 for 1, 2 function tag	
	localVars->nSendStartAddrCoil = 0;	// add 2015-03-20 for 1, 2 function tag
	localVars->tagOne = NULL;			// add 2015-03-20
	localVars->tagOne2 = NULL;			// add 2015-03-20
	readRegisteredTag(pt);
	//localVars->cMasterStation = 255;

	getInitParameter(pt);			// add 2007-08-30
	if(localVars->bUseDigitalMemory) readRegisteredTag2(pt); // add 2015-03-20
	/*if(strlen(pt->sScanProtocolOption) > 0) {// delete 2007-08-30
	comma.Set(pt->sScanProtocolOption);		
	comma.GetBYTE(localVars->cStation);
	comma.GetBYTE(i);
	localVars->bFloatData = (i == 1) ? true : false;
	comma.GetBYTE(i);
	localVars->bReadValueFloat = (i == 1) ? true : false;
	}*/
}

void ProcProtocolUnInit(LOCAL_PORT_STRUCT *pt)
{
	if (pt == NULL || pt->hLocalProtocol == NULL) {   //add 2025-07-09 PSU
		return;
	}

	if(localVars->tagOne != NULL) {
		delete localVars->tagOne;			// add 2015-03-20
		localVars->tagOne = NULL;  //add 2025-07-09 PSU
	}
	if(localVars->tagOne2 != NULL) {
		delete localVars->tagOne2;		// add 2015-03-20
		localVars->tagOne2 = NULL;  //add 2025-07-09 PSU
	}
	delete localVars;
	pt->hLocalProtocol = NULL;   //add 2025-07-09 PSU
}

void ProcProtocolDrawMethodTitle(HDC hdc, int x, int y)
{
	char *string = "station, read data type, address, buf address, read size (MODBUS RTU 2)";

	TextOut(hdc, x, y, string, (int)strlen(string));
}

void ProcProtocolDrawMethod(LOCAL_PORT_STRUCT *pt, HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	//cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;
	cxChar = (tm.tmHeight+tm.tmExternalLeading)/2;
	wsprintf(buf, "%3d", sm->station);
	TextOut(hdc, x, y, buf, (int)strlen(buf));

	wsprintf(buf, "%s", sm->type);
	TextOut(hdc, x+cxChar*8, y, buf, (int)strlen(buf));

	wsprintf(buf, "%3d", sm->address);
	TextOut(hdc, x+cxChar*20, y, buf, (int)strlen(buf));

	wsprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*28, y, buf, (int)strlen(buf));

	wsprintf(buf, "%3d", sm->size);		// size word of read
	TextOut(hdc, x+cxChar*38, y, buf, (int)strlen(buf));
}


static bool isWriteDataPacket(LOCAL_PORT_STRUCT *pt)
{
	BYTE	func = (localVars->bTcp) ? pt->commRecvBuf[7] : pt->commRecvBuf[1];	// add 2014-10-07
	//switch(pt->commRecvBuf[1]) {												// deleted 2014-10-07
	switch(func) {																// add 2014-10-07
		case 5 :																// add 2015-03-09
		case 6 :
		case 0x10 : return true;
		default : 	return false;
	}	
}

static int addTcpHeaderCheckAndAdd(LOCAL_PORT_STRUCT *pt, int buf_pos, bool bError) // add 2014-10-06
{
	BYTE		func = (localVars->bTcp) ? pt->commRecvBuf[7] : pt->commRecvBuf[1];

	if(localVars->bTcp) {
		localVars->sendBuf[buf_pos++] = pt->commRecvBuf[0];//HIBYTE(localVars->nTns);
		localVars->sendBuf[buf_pos++] = pt->commRecvBuf[1];//LOBYTE(localVars->nTns);
		localVars->sendBuf[buf_pos++] = 0x00;
		localVars->sendBuf[buf_pos++] = 0x00;
		localVars->sendBuf[buf_pos++] = 0x00;
		localVars->sendBuf[buf_pos++] = 0x00;
	}
	localVars->sendBuf[buf_pos++] = localVars->cStation;
	if(bError) localVars->sendBuf[buf_pos++] = func | 0x80;
	else	   localVars->sendBuf[buf_pos++] = func;
	return buf_pos;
}

static int addTcpTailCodeAdd(LOCAL_PORT_STRUCT *pt, int buf_pos) // add 2014-10-06
{
	if(localVars->bTcp) {
		localVars->sendBuf[4] = HIBYTE(buf_pos-6);
		localVars->sendBuf[5] = LOBYTE(buf_pos-6);
	}
	else {
		WORD		crc;
		crc = GetCRC_16_15_2_1(localVars->sendBuf, buf_pos);
		localVars->sendBuf[buf_pos++] = HIBYTE(crc);		// CRC
		localVars->sendBuf[buf_pos++] = LOBYTE(crc);
	}
	return buf_pos;
}


static void MakeAndSendErrorCodeData(LOCAL_PORT_STRUCT *pt, BYTE code)
{
	int			buf_pos = 0;
	//WORD		crc;

	buf_pos = addTcpHeaderCheckAndAdd(pt, buf_pos, true);							// add 2014-10-06
	//localVars->sendBuf[buf_pos++] = localVars->cStation;							// deleted 2014-10-07
	//localVars->sendBuf[buf_pos++] = pt->commRecvBuf[1] | 0x80;					// deleted 2014-10-07
	localVars->sendBuf[buf_pos++] = code;

	buf_pos = addTcpTailCodeAdd(pt, buf_pos);										// add 2014-10-06
	//crc = GetCRC_16_15_2_1(localVars->sendBuf, buf_pos);							// deleted 2014-10-06
	//localVars->sendBuf[buf_pos++] = HIBYTE(crc);		// CRC						// deleted 2014-10-06
	//localVars->sendBuf[buf_pos++] = LOBYTE(crc);									// deleted 2014-10-06

	PlcDeviceWriteContinue(&pt->device, (char*)&localVars->sendBuf, buf_pos);	
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;
	pt->timeout->Reset();
	DisplaySendCodeNextLine(pt->no);
}

static void MakeAndSendAckData(LOCAL_PORT_STRUCT *pt)
{
	int			buf_pos = 0;
	int			nTcpAddPos = (localVars->bTcp) ? 6 : 0;								// add 2015-03-09
	BYTE		func = (localVars->bTcp) ? pt->commRecvBuf[7] : pt->commRecvBuf[1];	// add 2015-03-09

	buf_pos = addTcpHeaderCheckAndAdd(pt, buf_pos, false);							// add 2014-10-06
	//localVars->sendBuf[buf_pos++] = localVars->cStation;							// deleted 2014-10-07
	//localVars->sendBuf[buf_pos++] = pt->commRecvBuf[1];							// deleted 2014-10-07

	localVars->sendBuf[buf_pos++] = HIBYTE(localVars->nStartAddr);
	localVars->sendBuf[buf_pos++] = LOBYTE(localVars->nStartAddr);
	if(func == 0x05 || func == 0x06 || func == 0x10) {								// add 2015-03-09
		localVars->sendBuf[buf_pos++] = pt->commRecvBuf[4+nTcpAddPos];
		localVars->sendBuf[buf_pos++] = pt->commRecvBuf[5+nTcpAddPos];
	}
	else {
		localVars->sendBuf[buf_pos++] = 0;
		localVars->sendBuf[buf_pos++] = 0;
	}
	buf_pos = addTcpTailCodeAdd(pt, buf_pos);										// add 2014-10-06
	//crc = GetCRC_16_15_2_1(localVars->sendBuf, buf_pos);							// deleted 2014-10-06
	//localVars->sendBuf[buf_pos++] = HIBYTE(crc);		// CRC						// deleted 2014-10-06
	//localVars->sendBuf[buf_pos++] = LOBYTE(crc);									// deleted 2014-10-06

	PlcDeviceWriteContinue(&pt->device, (char*)&localVars->sendBuf, buf_pos);	
	pt->commCountSend = buf_pos;	
	pt->timeout->Reset();
	DisplaySendCodeNextLine(pt->no);
}

static BYTE GetSum8(const char *s)
{
	BYTE c = 0;

	CStringW cs(s);

	int count = (int)wcslen(cs);

	for(int i = 0; i < count; i++)
	{
		c += (byte)(cs[i]%256);
		c += (byte)(cs[i]/256);
	}

	return c;
}

static bool GetCurr(char *tag, char *curr, int limit)
{
	byte sum = GetSum8(tag);

	CString sub_name;

	sub_name.Format("%d\\%s", sum, tag);

	LoadRegAutoBaseConfig("TagShare", sub_name, "Value", "", curr, limit);

	return true;
}


WORD getStartModbusAddr(LOCAL_PORT_STRUCT *pt, int func)// add 2015-03-20
{
	if(localVars->bUseDigitalMemory == true && (func == 1 || func == 2 || func == 5)) return localVars->nSendStartAddrCoil;// add 2015-03-26 || func == 5
	return localVars->nSendStartAddr;
}

WORD getReadedTagSize(LOCAL_PORT_STRUCT *pt, int func)// add 2015-03-20
{
	if(localVars->bUseDigitalMemory == true && (func == 1 || func == 2 || func == 5)) return localVars->nReadTagSize2;// add 2015-03-26 || func == 5
	return localVars->nReadTagSize;
}

char *getTagNameCurrPos(LOCAL_PORT_STRUCT *pt, int func, int pos)// add 2015-03-20
{
	if(localVars->bUseDigitalMemory == true && (func == 1 || func == 2 || func == 5)) return (char *)localVars->tagOne2[pos].tag;
	return (char *)localVars->tagOne[pos].tag;
}

BYTE _BYTE_MASK[8] = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };

static int addDigitalInputDataToBuf(LOCAL_PORT_STRUCT *pt, int buf_pos)	// add 2015-03-09
{
	int			i, pos, nSize;
	BYTE		sendImsi = 0, val=0;
	CString		str;
	//char		curr[50];														// delete 2025-07-14
	char		*tag;																// add 2015-03-20

	nSize = localVars->nReadSize/8;
	if(nSize <= 0) nSize = 1;
	localVars->sendBuf[buf_pos++] = LOBYTE(nSize);

	for(i = 0; i < localVars->nReadSize; i++) {
		if(buf_pos+2 >= MAX_MODBUS_RTU_SEND_BUF) break;// size check
		pos = (i+(localVars->nStartAddr-getStartModbusAddr(pt, 1))) % MAX_SEND_TAG_COUNT;// changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, 1)
		if(pos < 0 || pos >= getReadedTagSize(pt, 1)) val = 0;							 // changed 2015-03-20 localVars->nReadTagSize -> getReadedTagSize(pt, 1)
		else {
			tag = getTagNameCurrPos(pt, 1, pos);											 // add 2015-03-20
			if(localVars->bVersion9) {
				if(Tag9GetCurr(tag, str)) val = (BYTE)(atoi(str));						 // version 9.xx, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
				else val = 0;				
			}
			//8.xx 이하 버전 삭제 2025-07-09
			//else { 
			//	if(SharedTagGetCurr(tag, curr)) val = (BYTE)(atoi(curr));				 // version 8.xx 이하, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
			//	else if(GetCurr(tag, curr, sizeof(curr))) val = (BYTE)(atoi(curr));		 // version 9.xx, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
			//	else val = 0;
			//}
		}
		if(val & 0x01) sendImsi |= _BYTE_MASK[i % 8];
		if((i % 8) == 7) {
			localVars->sendBuf[buf_pos++] = sendImsi;
			sendImsi = 0x0;
		}
	}
	if((i % 8) != 0) localVars->sendBuf[buf_pos++] = sendImsi;
	if(localVars->bTcp) localVars->sendBuf[8] = LOBYTE(buf_pos-9);
	else				localVars->sendBuf[2] = LOBYTE(buf_pos-3);
	return buf_pos;
}

static void MakeAndSendWantedData(LOCAL_PORT_STRUCT *pt)
{
	int			i, pos, buf_pos = 0, nSize;
	WORD		val=0;
	float		fVal=0;
	CString		str;										// add 2007-08-30
	//char		curr[50];								// delete 2025-07-14
    char		imsi[50];
	char		*tag;										// add 2015-03-20
	int			nTcpAddPos = (localVars->bTcp) ? 6 : 0;		// add 2015-03-09

	int func = pt->commRecvBuf[1+nTcpAddPos];									// add 2015-03-20
	if(localVars->nStartAddr < getStartModbusAddr(pt, func)) {//  add 2011-04-27, 읽을 데이터 주소가 보내줄 시작주소보다 작다 // changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, func)
		return MakeAndSendErrorCodeData(pt, 2);
	}
	if(localVars->nStartAddr >= getStartModbusAddr(pt, func)+getReadedTagSize(pt, func)) {// add 2011-04-27 // changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, func), // changed 2015-03-20 localVars->nReadTagSize -> getReadedTagSize(pt, func), add 2015-03-26 =
		return MakeAndSendErrorCodeData(pt, 2);
	}
	//if(localVars->nStartAddr+localVars->nReadSize/2 > localVars->nReadTagSize) {// deleted 2011-04-27
	//	return MakeAndSendErrorCodeData(pt, 2);
	//}
	if(localVars->nReadSize <= 0) {
		return MakeAndSendErrorCodeData(pt, 3);
	}

	buf_pos = addTcpHeaderCheckAndAdd(pt, buf_pos, false);						// add 2014-10-06	
	//localVars->sendBuf[buf_pos++] = localVars->cStation;						// deleted 2014-10-07
	//localVars->sendBuf[buf_pos++] = pt->commRecvBuf[1];						// deleted 2014-10-07

	//int func = pt->commRecvBuf[1+nTcpAddPos];									// add 2015-03-09, deleted 2015-03-20
	if(func == 1 || func == 2) {												// add 2015-03-09
		buf_pos = addDigitalInputDataToBuf(pt, buf_pos);
	}
	else {
		localVars->sendBuf[buf_pos++] = LOBYTE(localVars->nReadSize*2);
		nSize = localVars->nReadSize;
		if(localVars->bFloatData) {
			nSize /= 2;
		}

		for(i = 0; i < nSize; i++) {
			if(buf_pos+2 >= MAX_MODBUS_RTU_SEND_BUF) break;// add 2011-04-27, size check
			pos = (i+(localVars->nStartAddr-getStartModbusAddr(pt, func))) % MAX_SEND_TAG_COUNT;// -localVars->nSendStartPos, 2011-04-27 add // changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, func)
			if(pos < 0 || pos >= getReadedTagSize(pt, func)) {// add 2011-04-27, // changed 2015-03-20 localVars->nReadTagSize -> getReadedTagSize(pt, func)
				val = 0;
				fVal = 0.0;
			}
			else {														// add 2011-04-27
				tag = getTagNameCurrPos(pt, func, pos);					// add 2015-03-20
				if(localVars->bVersion9) {								// add 2007-08-30
					if(Tag9GetCurr(tag, str)) {							// version 9.xx, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
						val = atoi(str);
						fVal = (float)atof(str);
					}
					else {
						val = 0;
						fVal = 0.0;
					}
				}
				//8.xx 이하 버전 삭제 2025-07-09
				//else {
				//	if(SharedTagGetCurr(tag, curr)) {					// version 8.xx 이하, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
				//		val = atoi(curr);
				//		fVal = (float)atof(curr);
				//	}
				//	else if(GetCurr(tag, curr, sizeof(curr))) {			// version 9.xx, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
				//		val = atoi(curr);
				//		fVal = (float)atof(curr);
				//	}
				//	else {
				//		val = 0;
				//		fVal = 0.0;
				//	}
				//}
			}
			if(localVars->bFloatData) {
				memcpy(imsi, &fVal, 4);
				localVars->sendBuf[buf_pos++] = imsi[3];				// float
				localVars->sendBuf[buf_pos++] = imsi[2];
				localVars->sendBuf[buf_pos++] = imsi[1];
				localVars->sendBuf[buf_pos++] = imsi[0];			
			}
			else {
				localVars->sendBuf[buf_pos++] = HIBYTE(val);
				localVars->sendBuf[buf_pos++] = LOBYTE(val);
			}
		}
		if(i != nSize) {
			if(localVars->bTcp) localVars->sendBuf[8] = LOBYTE(buf_pos-9);// add 2014-10-06
			else				localVars->sendBuf[2] = LOBYTE(buf_pos-3);// add 2011-04-27
		}
	}

	buf_pos = addTcpTailCodeAdd(pt, buf_pos);					// add 2014-10-06
	//crc = GetCRC_16_15_2_1(localVars->sendBuf, buf_pos);		// deleted 2014-10-06
	//localVars->sendBuf[buf_pos++] = HIBYTE(crc);		// CRC	// deleted 2014-10-06
	//localVars->sendBuf[buf_pos++] = LOBYTE(crc);				// deleted 2014-10-06

	PlcDeviceWriteContinue(&pt->device, (char*)&localVars->sendBuf, buf_pos);	
	pt->commCountSend = buf_pos;
	pt->commCountCurr = 0;
	pt->timeout->Reset();
	DisplaySendCodeNextLine(pt->no);
}



static void killStartCode(LOCAL_PORT_STRUCT *pt)
{	
	if(pt->commCountCurr <= 1) pt->commCountCurr = 0;	
	else {
		pt->commCountCurr--;
		for(int i = 0; i < pt->commCountCurr;i++) {
			pt->commRecvBuf[i] = pt->commRecvBuf[i+1];
		}		
	}	
}

static void checkStartStation(LOCAL_PORT_STRUCT *pt)
{
	if(localVars->bTcp) {											// added 2014-10-07
		if(pt->commCountCurr > 6) {
			if(pt->commRecvBuf[6] != localVars->cStation) killStartCode(pt);
		}
		return;
	}
	if(pt->commRecvBuf[0] != localVars->cStation) killStartCode(pt);
}


static bool IsOnePacketCode(LOCAL_PORT_STRUCT *pt)
{
	if(localVars->bTcp) {														// add 2014-10-07
		if(pt->commCountCurr < 6) return false;
		pt->commCountNeed = pt->commRecvBuf[4] * 0x100 + pt->commRecvBuf[5] + 6;
		if(pt->commCountCurr < pt->commCountNeed) return false;
		return true;
	}

	if(pt->commCountCurr >= 2) {
		switch(pt->commRecvBuf[1]) {
			case 0x10 : break;
			default : pt->commCountNeed = 8; break;
		}
	}	
	if(pt->commCountCurr < pt->commCountNeed) return false;

	int		nSize;
	WORD	crc = 0;

	switch(pt->commRecvBuf[1]) {
		case 6 :
			break;
		case 0x10 :
			if(localVars->bReadSizeflag == false) {
				nSize = pt->commRecvBuf[6];
				pt->commCountNeed = nSize + 9;
				localVars->bReadSizeflag = true;
				pt->timeout->Reset();
				return false;
			}
			break;
	}	
	localVars->bReadSizeflag = false;
	crc = GetCRC_16_15_2_1(pt->commRecvBuf, pt->commCountNeed-2);
	if(crc == pt->commRecvBuf[pt->commCountNeed-2] * 256 + pt->commRecvBuf[pt->commCountNeed-1]) return true;	
	killStartCode(pt);
	return false;
}


static bool readPacketStartAddrAndReadSize(LOCAL_PORT_STRUCT *pt)
{
	int			nTcpAddPos = (localVars->bTcp) ? 6 : 0;							// add 2014-10-07

	//switch(pt->commRecvBuf[1]) {												// deleted 2014-10-07
	switch(pt->commRecvBuf[1+nTcpAddPos]) {										// added 2014-10-07
		case 1 :																// added 2015-03-09
		case 2 :																// added 2015-03-09
		case 3 :
		case 4 : 
		case 5 :																// added 2015-03-09
		case 6 :
		case 0x10 : break;
		default :
			MakeAndSendErrorCodeData(pt, 1);	// Illegal function
			return false;
	}	
	localVars->nStartAddr = pt->commRecvBuf[2+nTcpAddPos] * 256 + pt->commRecvBuf[3+nTcpAddPos];
	if(pt->commRecvBuf[1+nTcpAddPos] == 6) localVars->nReadSize = 1;
	else								   localVars->nReadSize = pt->commRecvBuf[4+nTcpAddPos] * 256 + pt->commRecvBuf[5+nTcpAddPos];

	return true;
}

static int sendErrorCodeAndValReset(LOCAL_PORT_STRUCT *pt)	// added 2015-03-09
{
	MakeAndSendErrorCodeData(pt, 2);
	pt->commCountCurr = 0;
	pt->commCountNeed = (localVars->bTcp) ? 12 : 8;
	return COMMUNICATION_OK;
}

static int saveToReceivedDataAndSendAck(LOCAL_PORT_STRUCT *pt)
{
	int			i, nSize;
	WORD		lo_word, hi_word;
	DWORD		value;
	float		f_value;
	int			nTcpAddPos = (localVars->bTcp) ? 6 : 0;							// add 2014-10-07
	int			pos;															// added 2015-03-09
	char		*tag;															// add 2015-03-20
	CString	 buf; //20250714 PSU 추가

	int func = pt->commRecvBuf[1+nTcpAddPos];									// add 2015-03-20
	if(localVars->nStartAddr < getStartModbusAddr(pt, func)) return sendErrorCodeAndValReset(pt);  // add 2015-03-09, 읽을 데이터 주소가 보내줄 시작주소보다 작다 // changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, func)
	if(localVars->nStartAddr >= getStartModbusAddr(pt, func)+getReadedTagSize(pt, func)) return sendErrorCodeAndValReset(pt); // add 2015-03-09 // changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, func), // changed 2015-03-20 localVars->nReadTagSize -> getReadedTagSize(pt, func), add 2015-03-26 =
	pos = localVars->nStartAddr-getStartModbusAddr(pt, func);					// add 2015-03-09 // changed 2015-03-20 localVars->nSendStartAddr -> getStartModbusAddr(pt, func)

	tag = getTagNameCurrPos(pt, func, pos);										// add 2015-03-20, changed 2015-03-24 1 -> func
	//if(pt->commRecvBuf[1] == 6) {												// deleted 2014-10-07
	if(pt->commRecvBuf[1+nTcpAddPos] == 6) {									// added 2014-10-07
		if(localVars->bReadValueFloat) {
			lo_word = pt->commRecvBuf[6+nTcpAddPos]*256+pt->commRecvBuf[7+nTcpAddPos];
			hi_word = pt->commRecvBuf[4+nTcpAddPos]*256+pt->commRecvBuf[5+nTcpAddPos];
			value = MAKELONG(lo_word, hi_word);

			memcpy(&f_value, &value, 4);
			if(pos >= 0 && pos < MAX_SEND_TAG_COUNT) {							// added 2015-03-09
				//PokeFLOAT(pt, localVars->nStartAddr, f_value);				// deleted 2015-03-09
				PokeFLOAT(pt, pos, f_value);									// added 2015-03-09
				//TagSetCurr(tag, f_value);										// added 2015-03-09, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
				if(!TagSetCurr(tag, f_value))  //20250714 PSU 에러 추가.
				{
					pt->commCountCurr = 0;
					pt->commCountNeed = (localVars->bTcp) ? 12 : 8;
					buf.Format("TagSetCurr Error : %s", tag );
					PlcScanSetErrorString(pt, buf);
					return COMMUNICATION_ERR_STRING;
				}
			}
		}
		else {
			if(pos >= 0 && pos < MAX_SEND_TAG_COUNT) {							// added 2015-03-09
				//PokeWORD(pt, localVars->nStartAddr, pt->commRecvBuf[4+nTcpAddPos]*256u+(BYTE)pt->commRecvBuf[5+nTcpAddPos]);// deleted 2015-03-09
				value = pt->commRecvBuf[4+nTcpAddPos]*256u+(BYTE)pt->commRecvBuf[5+nTcpAddPos];// added 2015-03-09
				PokeWORD(pt, pos, (WORD)value);									// added 2015-03-09
				//TagSetCurr(tag, value);											// added 2015-03-09, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
				if(!TagSetCurr(tag, value))  //20250714 PSU 에러 추가.
				{
					pt->commCountCurr = 0;
					pt->commCountNeed = (localVars->bTcp) ? 12 : 8;
					buf.Format("TagSetCurr Error : %s", tag );
					PlcScanSetErrorString(pt, buf);
					return COMMUNICATION_ERR_STRING;
				}
			}
		}
	}
	else if(pt->commRecvBuf[1+nTcpAddPos] == 5) {								// added 2015-03-09
		value = pt->commRecvBuf[4+nTcpAddPos]*256u+(BYTE)pt->commRecvBuf[5+nTcpAddPos];
		value = (value == 0xFF00) ? 1 : 0;		
		if(pos >= 0 && pos < MAX_SEND_TAG_COUNT) {
			PokeWORD(pt, pos, (WORD)value);
			//TagSetCurr(tag, value);												// changed 2015-03-20 localVars->tagOne[pos].tag -> tag
			if(!TagSetCurr(tag, value))  //20250714 PSU 에러 추가.
			{
				pt->commCountCurr = 0;
				pt->commCountNeed = (localVars->bTcp) ? 12 : 8;
				buf.Format("TagSetCurr Error : %s", tag );
				PlcScanSetErrorString(pt, buf);
				return COMMUNICATION_ERR_STRING;
			}
		}
	}
	else {
		nSize = localVars->nReadSize;
		if(localVars->bReadValueFloat) nSize /= 2;

		for(i = 0; i < nSize; i++) {
			if(localVars->bReadValueFloat) {
				lo_word = pt->commRecvBuf[9+i*4+nTcpAddPos]*256+pt->commRecvBuf[10+i*4+nTcpAddPos];
				hi_word = pt->commRecvBuf[7+i*4+nTcpAddPos]*256+pt->commRecvBuf[8+i*4+nTcpAddPos];
				value = MAKELONG(lo_word, hi_word);

				memcpy(&f_value, &value, 4);
				if(pos + i >= 0 && pos + i < MAX_SEND_TAG_COUNT) {					// added 2015-03-09, changed 2016-12-15 pos -> pos + i
					//PokeFLOAT(pt, localVars->nStartAddr+i, f_value);				// deleted 2015-03-09
					PokeFLOAT(pt, pos+i, f_value);									// added 2015-03-09
					tag = getTagNameCurrPos(pt, func, pos+i);						// add 2016-12-12 for multi write from server
					//TagSetCurr(tag, f_value);										// added 2015-03-09, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag
					if(!TagSetCurr(tag, f_value))  //20250714 PSU 에러 추가.
					{
						pt->commCountCurr = 0;
						pt->commCountNeed = (localVars->bTcp) ? 12 : 8;
						buf.Format("TagSetCurr Error : %s", tag );
						PlcScanSetErrorString(pt, buf);
						return COMMUNICATION_ERR_STRING;
					}
				}
			}
			else {
				if(pos + i >= 0 && pos + i < MAX_SEND_TAG_COUNT) {					// added 2015-03-09, changed 2016-12-15 pos -> pos + i
					//PokeWORD(pt, localVars->nStartAddr+i, pt->commRecvBuf[7+i*2+nTcpAddPos]*256u+(BYTE)pt->commRecvBuf[8+i*2+nTcpAddPos]);// deleted 2015-03-09
					value = pt->commRecvBuf[7+i*2+nTcpAddPos]*256u+(BYTE)pt->commRecvBuf[8+i*2+nTcpAddPos];// added 2015-03-09
					PokeWORD(pt, pos+i, (WORD)value);								// added 2015-03-09
					tag = getTagNameCurrPos(pt, func, pos+i);						// add 2016-12-12 for multi write from server
					//TagSetCurr(tag, value);											// added 2015-03-09, // changed 2015-03-20 localVars->tagOne[pos].tag -> tag.
					if(!TagSetCurr(tag, value))  //20250714 PSU 에러 추가.
					{
						pt->commCountCurr = 0;
						pt->commCountNeed = (localVars->bTcp) ? 12 : 8;
						buf.Format("TagSetCurr Error : %s", tag );
						PlcScanSetErrorString(pt, buf);
						return COMMUNICATION_ERR_STRING;
					}
				}
			}
		}
	}
	MakeAndSendAckData(pt);
	pt->commCountCurr = 0;
	pt->commCountNeed = (localVars->bTcp) ? 12 : 8;// add 2014-10-07, localVars->bTcp
	return COMMUNICATION_OK;
}


/*static bool bTestFlag = true;	// add 2012-02-17 for test
static void testReceivedDataInput(LOCAL_PORT_STRUCT *pt)	// add 2012-02-17 for test
{
int			buf_pos = 0;

pt->commRecvBuf[buf_pos++] = 0x01;
pt->commRecvBuf[buf_pos++] = 0x04;
pt->commRecvBuf[buf_pos++] = 0x07;
pt->commRecvBuf[buf_pos++] = 0xD0;
pt->commRecvBuf[buf_pos++] = 0x00;
pt->commRecvBuf[buf_pos++] = 0x04;
pt->commRecvBuf[buf_pos++] = 0xF1;
pt->commRecvBuf[buf_pos++] = 0x44;
pt->commCountCurr = buf_pos;
}*/


//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int ProcProtocolRead(LOCAL_PORT_STRUCT *pt, int pos)
{	
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];	
	int					count;

	if(pt->bReadingFlag == OFF) {
		pt->bReadingFlag = ON;
		pt->timeout->Reset();		
		return COMMUNICATION_WAITING;		
	}

	while(1) {
		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ)) {
			pt->commCountCurr = 0;
			pt->commCountNeed = (localVars->bTcp) ? 12 : 8;// add 2014-10-07, localVars->bTcp
			localVars->bReadSizeflag = false;			
			return COMMUNICATION_OK;
			//return COMMUNICATION_TIME_OUT;
		}
		if(pt->commCountCurr >= MAX_RECV_BUF) {
			pt->commCountCurr = 0;
			return COMMUNICATION_ERR_SIZE_TOO_BIG;
		}

		/*if(bTestFlag) {					// add 2012-02-17 for test
		testReceivedDataInput(pt);	// add 2012-02-17 for test
		count = 0;					// add 2012-02-17 for test
		}
		else {*/							// add 2012-02-17 for test
		count = PlcDeviceReadContinue(&pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], 1);
		if(count == 0)	return COMMUNICATION_WAITING;
		//}

		pt->timeout->Reset();
		pt->commCountCurr += count;
		checkStartStation(pt);

		if(IsOnePacketCode(pt)) {
			DisplayRecvCodeNextLine(pt->no);
			readPacketStartAddrAndReadSize(pt);
			if(isWriteDataPacket(pt)) {
				return saveToReceivedDataAndSendAck(pt);
			}			
			MakeAndSendWantedData(pt);
			return COMMUNICATION_OK;
		}
		else {
			//bell();			// 2007-04-13 필요없는 코드라서 삭제
		}
	}
}


static bool checkDot(float val)
{
	DWORD	i = (DWORD)(val * 10);

	if(i % 10 == 0)return false;
	return true;
}




static int WriteData(LOCAL_PORT_STRUCT *pt, int station, WORD address, float value, char *device, WORD pannel)
{
	return COMMUNICATION_OK;	
}



//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int ProcProtocolWriteBit(LOCAL_PORT_STRUCT *pt, int station, DWORD address, WORD flag, char *device, WORD pannel, char *sAddress)
{
	return WriteData(pt, station, (WORD)address, (float)flag, device, pannel);

}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int ProcProtocolWriteWord(LOCAL_PORT_STRUCT *pt, int station, DWORD address, long double value, char *device, WORD pannel, char *sAddress)
{
	return WriteData(pt, station, (WORD)address, (float)value, device, pannel);
}

int ProcProtocolWriteBlock(LOCAL_PORT_STRUCT *pt, int station, DWORD address, BYTE *value, short array_size, BYTE array_type, char *device, WORD pannel, char *sAddress)
{
	CString buf;
	CString atype;

	if(array_type == 1)			atype = "byte";
	else if(array_type == 4)	atype = "ushort";
	else if(array_type == 6)	atype = "uint";
	else if(array_type == 8)	atype = "ulong";
	else if(array_type == 9)	atype = "float";
	else if(array_type == 10)	atype = "double";
	else if(array_type == 11)	atype = "string";
	else						atype = "unknown type";

	buf.Format("This protocol is not supported BLOCK Write. (block_size=%d, block_type=%d(%s))", array_size, array_type, (const char*)atype);

	PlcScanSetErrorString(pt, buf);

	return COMMUNICATION_ERR_STRING;
}


class ChangeResource {
	HINSTANCE hInst;
public:
	ChangeResource();
	~ChangeResource();
};

ChangeResource::ChangeResource()
{
	hInst = AfxGetResourceHandle();
	AfxSetResourceHandle(theApp.m_hInstance);
}

ChangeResource::~ChangeResource()
{
	AfxSetResourceHandle(hInst);
}

static void getInitDialogParameter(char *option, CDialogProtocolOption &dialog)
{
	CommaBlockString	comma;
	BYTE				i;
	WORD				wVal;

	if(strlen(option) > 0) {
		comma.Set(option);

		comma.GetBYTE(dialog.m_station);
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);
		dialog.m_bFloatData = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);
		dialog.m_bReadValueFloat = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);							// add 2007-08-30
		dialog.m_bVersion9 = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetWORD(wVal);						// add 2011-04-27
		dialog.m_nSendStartPos = wVal;				// add 2011-04-27
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);							// add 2014-10-07
		dialog.m_bUseTcp = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetBYTE(i);							// add 2015-03-20 for 1, 2 function tag	
		dialog.m_bUseDigitalMemory = (i == 1) ? true : false;
		if(comma.IsEOS()) return;

		comma.GetWORD(wVal);						// add 2015-03-20 for 1, 2 function tag	
		dialog.m_nSendStartPosCoil = wVal;				
		if(comma.IsEOS()) return;
	}
}

int ProcProtocolConfigOption(HWND hwnd, HWND hwndEdit, int port, char *option)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());	// 이것을 쓰니 잘된다. 2018-9-12
	ChangeResource			res;
	int						retn = 0;
	//BYTE					i;
	CDialogProtocolOption	dialog;
	//CommaBlockString		comma;

	dialog.m_port = (BYTE)port;
	dialog.m_station = 0;
	dialog.m_bFloatData = false;
	dialog.m_bReadValueFloat = false;
	dialog.m_bVersion9 = false;					// add 2007-08-30
	dialog.m_nSendStartPos = 0;					// add 2011-04-27
	dialog.m_bUseTcp = false;					// add 2014-10-07
	dialog.m_bUseDigitalMemory = false;			// add 2015-03-20 for 1, 2 function tag	
	dialog.m_nSendStartPosCoil = 0;				// add 2015-03-20 for 1, 2 function tag	

	getInitDialogParameter(option, dialog);		// add 2007-08-30
	TagLoad9();									// add 2007-08-30
	/*if(strlen(option) > 0) {					// delete 2007-08-30
	comma.Set(option);		
	comma.GetBYTE(dialog.m_station);
	comma.GetBYTE(i);
	dialog.m_bFloatData = (i == 1) ? true : false;
	comma.GetBYTE(i);
	dialog.m_bReadValueFloat = (i == 1) ? true : false;
	}*/
	retn = (int)dialog.DoModal();	
	if(retn == IDOK) {
		sprintf(option, "%d,%d,%d,%d,%d,%d,%d,%d,", dialog.m_station, dialog.m_bFloatData, dialog.m_bReadValueFloat, dialog.m_bVersion9, dialog.m_nSendStartPos, dialog.m_bUseTcp, dialog.m_bUseDigitalMemory, dialog.m_nSendStartPosCoil);// add 2007-08-30, dialog.m_bVersion9, add-2011-04-27 dialog.m_nSendStartPos
	}
	return retn;
}