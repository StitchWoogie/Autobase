//------------------------------------------------------------------------------
//	각 프로토콜 드라이브들이 같이 쓸 수 있는 함수들을 모아 놓았다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <dos.h>
#include <string.h>
#include <stdarg.h>
#include <stdio.h>

#include <dataswap.h>

#include "..\plc_scan.h"
#include "pro_lib.h"

//------------------------------------------------------------------------------
//
//------------------------------------------------------------------------------

int SeekMatch(LOCAL_PORT_STRUCT *pt, int pos, char var_type)
{
	int i;
	SCAN_METHOD_STRUCT *sm;

	for(i = 0; i < pt->nScanMethodHap; i++) {
		sm = &pt->scanMethod[i];

		if(sm->target == pos && sm->cVarType == var_type)	return i;
	}
	return -1;
}
              
void WaitSec(int sec)
{
	struct time t;
	int old, curr=0;

	gettime(&t);

	old = t.ti_sec;

	while(1) {
		gettime(&t);
		if(t.ti_sec != old) {
			old = t.ti_sec;
			curr++;
			if(curr >= sec) {
				return;
			}
		}
	}
}

/*
void PlcScanSetErrorString(char *string, ...)
{
	va_list ap;
	StackChar imsi(1000);

	if(sPlcScanErrorString)	delete sPlcScanErrorString;
	sPlcScanErrorString = NULL;

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);

		sPlcScanErrorString = new char[strlen(imsi.data)+1];
		if(sPlcScanErrorString != NULL) {
			strcpy(sPlcScanErrorString, imsi.data);
		}
	}
}


char *PlcScanGetErrorString()
{
	return sPlcScanErrorString;
}

void PlcScanErrorStringDelete()
{
	if(sPlcScanErrorString)	delete sPlcScanErrorString;
	sPlcScanErrorString = NULL;
}
*/

// 포인터로 사용하니 포트가 많고 메시지가 모든포트에서 발생시 다른 스레드에서 포인트를 이중으로 사용하여 다운되는 경우가 발생해서
// CString으로 바꾸고 테스트 하니 정상동작함 2007.10.5 (CString이 스레드에서 보호가 되면 문제가 없을듯)

// 이전에 8.5.0 에서는 프로토콜 내부 메시지 만들 때 비슷한 문제가 발생하여 각각의 프로토콜마다 메시지 영역을 따로 분리
// 
//static char *sPlcScanErrorString = NULL;

//CWin32Heap g_stringHeap( 0, 0, 0 );
//CAtlStringMgr g_stringMgr( &g_stringHeap ); 
static CString sPlcScanErrorString;

static ThreadLock tlPlcScanSetErrorString;

void PlcScanSetErrorString(const char *string, ...)
{
	va_list ap;
	StackChar imsi(1000);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);

		tlPlcScanSetErrorString.Lock();
		sPlcScanErrorString = imsi.data;
		tlPlcScanSetErrorString.Unlock();
	}
}

void PlcScanGetErrorString(char *buf)
{
	//strcpy(buf, sPlcScanErrorString); 이것을 사용해도 잘되나 어떤 블로그에서 아래와 같이 사용하면 좋다해서 해봄 2007.10.11
	tlPlcScanSetErrorString.Lock();
	strcpy(buf, static_cast<LPCTSTR>(sPlcScanErrorString));
	tlPlcScanSetErrorString.Unlock();
}

void PlcScanErrorStringDelete()
{

}

void PokeNewWORD(LOCAL_PORT_STRUCT *pt, int address, WORD value)
{
	if(pt->bufWORD == NULL)			return;
	if(address >= pt->nBufSizeWORD)	return;

	if(pt->bufWORD[address].value != value) {
		pt->bufWORD[address].value = value;
	}
	pt->bufWORD[address].flag  = ON;
}

void PokeNewFLOAT(LOCAL_PORT_STRUCT *pt, int address, float value)
{
	if(pt->bufFLOAT == NULL)			return;
	if(address >= pt->nBufSizeFLOAT)	return;

	/*
	//-------------------------
	// 이상한 소수점을 막기위해 5짜리까지만 취한다. (이 부분이 Floating error 막을지는 확실하지는 않다.(2002-9-10, Version 7.91 부터 사용)
	CString imsi;
	imsi.Format("%.5f", value);
	value = (float)atof(imsi);
	//-------------------------
	*/

	if(pt->bufFLOAT[address].value != value) {
		pt->bufFLOAT[address].value = value;
	}
	pt->bufFLOAT[address].flag  = ON;
}

void PokeNewDWORD(LOCAL_PORT_STRUCT *pt, int address, DWORD value)
{
	if(pt->bufDWORD == NULL)			return;
	if(address >= pt->nBufSizeDWORD)	return;

	if(pt->bufDWORD[address].value != value) {
		pt->bufDWORD[address].value = value;
	}
	pt->bufDWORD[address].flag  = ON;
}

void PokeNewSTRING(LOCAL_PORT_STRUCT *pt, int address, const char *value)
{
	if(pt->bufSTRING == NULL)			return;
	if(address >= pt->nBufSizeSTRING)	return;

	if(strlen(value) > 255) {
		strncpy(pt->bufSTRING[address].value, value, 255);
		pt->bufSTRING[address].value[255] = 0;
	}
	else {
		strcpy(pt->bufSTRING[address].value, value);
	}

	pt->bufSTRING[address].flag  = ON;
}

void PokeNewDOUBLE(LOCAL_PORT_STRUCT *pt, int address, double value)
{
	if(pt->bufDOUBLE == NULL)			return;
	if(address >= pt->nBufSizeDOUBLE)	return;

	if(pt->bufDOUBLE[address].value != value) {
		pt->bufDOUBLE[address].value = value;
	}
	pt->bufDOUBLE[address].flag  = ON;
}

void PokeNewINT64(LOCAL_PORT_STRUCT *pt, int address, __int64 value)
{
	if(pt->bufINT64 == NULL)			return;
	if(address >= pt->nBufSizeINT64)	return;

	if(address == 1) {
		int a = 1;
	}

	if(pt->bufINT64[address].value != value) {
		pt->bufINT64[address].value = value;
	}
	pt->bufINT64[address].flag  = ON;
}

void PokeWORD(LOCAL_PORT_STRUCT *pt, WORD address, WORD value)
{
	PokeNewWORD(pt, address, value);
}

void PokeFLOAT(LOCAL_PORT_STRUCT *pt, WORD address, float value)
{
	PokeNewFLOAT(pt, address, value);
}

void PokeDWORD(LOCAL_PORT_STRUCT *pt, WORD address, DWORD value)
{
	PokeNewDWORD(pt, address, value);
}

void PokeSTRING(LOCAL_PORT_STRUCT *pt, WORD address, const char *value)
{
	PokeNewSTRING(pt, address, value);
}

void PokeDOUBLE(LOCAL_PORT_STRUCT *pt, WORD address, double value)
{
	PokeNewDOUBLE(pt, address, value);
}

void PokeINT64(LOCAL_PORT_STRUCT *pt, WORD address, __int64 value)
{
	PokeNewINT64(pt, address, value);
}

void PokeValue(LOCAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm, WORD address, double value)
{
	if(sm->cVarType == 0) {			// 0 - WORD, 1 - FLOAT
		PokeWORD(pt, address, (WORD)value);
	}
	else if(sm->cVarType == 1) {			// 0 - WORD, 1 - FLOAT
		PokeFLOAT(pt, address, (float)value);
	}
	else if(sm->cVarType == 2) {			// 0 - WORD, 1 - FLOAT
		PokeDWORD(pt, address, (DWORD)value);
	}
	else if(sm->cVarType == 4) {			// 0 - WORD, 1 - FLOAT
		PokeDOUBLE(pt, address, value);
	}
	else if(sm->cVarType == 5) {			// 0 - WORD, 1 - FLOAT
		PokeINT64(pt, address, (__int64)value);
	}
	else {
		PokeWORD(pt, address, (WORD)value);
	}
}

WORD PeekValueWORD(LOCAL_PORT_STRUCT *pt, int address)
{
	if(address >= pt->nBufSizeWORD)	return 0;

	return pt->bufWORD[address].value;
}

DWORD PeekValueDWORD(LOCAL_PORT_STRUCT *pt, int address)
{
	if(address >= pt->nBufSizeDWORD)	return 0;
	
	return pt->bufDWORD[address].value;
}

double PeekValueFLOAT(LOCAL_PORT_STRUCT *pt, int address)
{
	if(address >= pt->nBufSizeFLOAT)	return 0;
	
	return pt->bufFLOAT[address].value;
}

int PeekValueSTRING(LOCAL_PORT_STRUCT *pt, int address, char *val)
{
	if(address >= pt->nBufSizeSTRING)	return 0;
	
	strcpy(val, pt->bufSTRING[address].value);
	return 1;
}

double PeekValueDOUBLE(LOCAL_PORT_STRUCT *pt, int address)
{
	if(address >= pt->nBufSizeDOUBLE)	return 0;
	
	return pt->bufDOUBLE[address].value;
}

__int64 PeekValueINT64(LOCAL_PORT_STRUCT *pt, int address)
{
	if(address >= pt->nBufSizeINT64)	return 0;
	
	return pt->bufINT64[address].value;
}


LocalProtocolClass :: LocalProtocolClass(HGLOBAL hglobal)
{
	hGlobal = hglobal;
	local = NULL;

	if(hGlobal == NULL)	return;

	local = (char*)GlobalLock(hGlobal);
}

LocalProtocolClass :: ~LocalProtocolClass()
{
	if(hGlobal == NULL)	return;
	GlobalUnlock(hGlobal);
}


