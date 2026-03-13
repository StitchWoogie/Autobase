// english O.K
#include "stdafx.h"
#include <string.h>
#include <stdlib.h>
                                  
#include <tools.h>

CommaBlockString :: CommaBlockString(TCHAR *string) 
{ 
	scanBufPos = 0;
	scanBufHap = 0;
	scanBuf = NULL;
	cBlockCode = ',';

	if(string != NULL)	Set(string);
}

CommaBlockString :: ~CommaBlockString()
{
	if(scanBuf != NULL) {
		delete scanBuf;
		scanBuf = NULL;
	}
}

void CommaBlockString :: Set(const TCHAR *string)
{
	if(scanBuf != NULL) {
		delete scanBuf;
		scanBuf = NULL;
	}

	scanBuf = new TCHAR[_tcslen(string)+1];

	scanBufPos = 0;
	_tcscpy(scanBuf, string);
	scanBufHap = (int)_tcslen(scanBuf);
}

void CommaBlockString :: GetString(TCHAR *string, int limit)
{
	int hap = 0;

	if(scanBuf == NULL) {
		string[0] = 0;
		return;
	}

	while(1) {
		if(scanBufPos >= scanBufHap) {
			if(hap > 0) {
				string[hap] = 0;
			}
			else {
				memset(string, 0, limit);
			}
			return;
		}
		if(scanBuf[scanBufPos] == cBlockCode || scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r') {
			scanBufPos++;
			if(hap > 0) {
				string[hap] = 0;
			}
			else {
				memset(string, 0, limit);
			}
			return;
		}
		if(hap < limit-1) {
			if(hap == 0 && (scanBuf[scanBufPos] == ' '  || scanBuf[scanBufPos] == '\t' ||
								 scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')) {
			}
			else {
				string[hap] = scanBuf[scanBufPos];
				hap++;
			}
		}
		scanBufPos++;
	}
}

void CommaBlockString :: GetString(CString &str)
{
	int hap = 0;
	str = "";

	if(scanBuf == NULL) {
		return;
	}

	while(1) {
		if(scanBufPos >= scanBufHap) {
			return;
		}
		if(scanBuf[scanBufPos] == cBlockCode || scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r') {
			scanBufPos++;
			return;
		}

		if(hap == 0 && (scanBuf[scanBufPos] == ' '  || scanBuf[scanBufPos] == '\t' ||
								scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')) {
		}
		else {
			str += scanBuf[scanBufPos];
			hap++;
		}

		scanBufPos++;
	}
}

void CommaBlockString :: GetStringTotalRemain(TCHAR *string, int limit)
{
	int hap = 0;

	if(scanBuf == NULL) {
		string[0] = 0;
		return;
	}

	while(1) {
		if(scanBufPos >= scanBufHap) {
			if(hap > 0) {
				string[hap] = 0;
			}
			else {
				memset(string, 0, limit);
			}
			return;
		}
		if(hap < limit-1) {
			if(hap == 0 && (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t')) {

			}
			else {
				string[hap] = scanBuf[scanBufPos];
				hap++;
			}
		}
		scanBufPos++;
	}
}

void CommaBlockString :: GetStringTotalRemain(CString &str)
{
	str = "";

	if(scanBuf == NULL) {
		return;
	}

	while(1) {
		if(scanBufPos >= scanBufHap) {
			return;
		}

		if(_tcslen(str)==0 && (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t')) {

		}
		else {
			str += scanBuf[scanBufPos];
		}
		
		scanBufPos++;
	}
}


void CommaBlockString :: GetInt(int &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = _tstoi(buf);
}

void CommaBlockString :: GetWORD(WORD &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = (WORD)_tstoi(buf);
}

//------------------------------------------------------------------------------
//
//------------------------------------------------------------------------------

void CommaBlockString :: GetHexDWORD(DWORD &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	_tcsupr(buf);

	int pos;
	unsigned u;
	DWORD imsi;
	DWORD gob = 1;

	val = 0;

	for(u = 0, pos = (int)_tcslen(buf)-1; u < _tcslen(buf); u++, pos--) {
		imsi = 0;
		if(buf[pos] >= 'A' && buf[pos] <= 'F') {
			imsi = buf[pos]-'A'+10;
		}
		else if(buf[pos] >= '0' && buf[pos] <= '9') {
			imsi = buf[pos]-'0';
		}
		imsi = imsi*gob;

		gob *= 0x10;

		val += imsi;
	}
}

void CommaBlockString :: GetHexUINT64(unsigned __int64 &val)
{
	TCHAR buf[30];
	GetString(buf, 30);
	_tcsupr(buf);

	int pos;
	unsigned u;
	unsigned __int64 imsi;
	unsigned __int64 gob = 1;

	val = 0;

	for(u = 0, pos = (int)_tcslen(buf)-1; u < _tcslen(buf); u++, pos--) {
		imsi = 0;
		if(buf[pos] >= 'A' && buf[pos] <= 'F') {
			imsi = buf[pos]-'A'+10;
		}
		else if(buf[pos] >= '0' && buf[pos] <= '9') {
			imsi = buf[pos]-'0';
		}
		imsi = imsi*gob;

		gob *= 0x10;

		val += imsi;
	}
}

void CommaBlockString :: GetHexWORD(WORD &val)
{
	DWORD retn;

	GetHexDWORD(retn);

	val = (WORD)retn;
}

void CommaBlockString :: GetInt(short &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = (WORD)_tstoi(buf);
}


void CommaBlockString :: GetChar(char &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = _tstoi(buf);
}

void CommaBlockString :: GetBYTE(BYTE &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = (BYTE)_tstoi(buf);
}

void CommaBlockString :: GetLong(long &val)
{
	TCHAR buf[20];
	GetString(buf, 20);
	if(buf[0] == 0)	{
		val = 0L;
		return;
	}

	val = _tstol(buf);
}

void CommaBlockString :: GetDWORD(DWORD &val)
{
	TCHAR buf[20];
	GetString(buf, 20);
	if(buf[0] == 0)	{
		val = 0L;
		return;
	}

	val = _tstol(buf);
}

//----------------------------------------------------------------------------
//	DI address 는 앞세자리는 10진수, 뒤의 한자리는 16진수로 구성된다.
//----------------------------------------------------------------------------

void CommaBlockString :: GetAddressDI(WORD &val)
{
	TCHAR buf[10];
	GetString(buf, 9);

	if(buf[0] == 0)	{
		val = 0;
		return;
	}
	val = 0;

	if(buf[3] >= '0' && buf[3] <= '9') {
		val |= ((((unsigned)(buf[3]-'0'))) & 0x000F);
	}
	else if(buf[3] >= 'A' && buf[3] <= 'F') {
		val |= ((((unsigned)(buf[3]-'A')+0x0A)) & 0x000F);
	}
	else;

	buf[3] = 0;
	val += (_tstoi(buf)*16);
}

void CommaBlockString :: GetFloat(float &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	if(buf[0] == 0)	{
		val = (float)0.0;
		return;
	}
	val = (float)_tstof(buf);
}

void CommaBlockString :: GetDouble(double &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	if(buf[0] == 0)	{
		val = (float)0.0;
		return;
	}
	val = (double)_tstof(buf);
}

void CommaBlockString :: GetLongDouble(long double &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	if(buf[0] == 0)	{
		val = (long double)0.0;
		return;
	}
#if	defined (WIN32)
	val = (float)_tstof(buf);
#else
	val = (float)_atold(buf);
#endif
}

void CommaBlockString :: GetCOLORREF(COLORREF &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	val = _tstol(buf);
}

void CommaBlockString :: GetDate(struct date *d)
{
	TCHAR buf[20];
	TCHAR imsi[20];

	GetString(buf, 19);

	_tcsncpy(imsi, &buf[0], 4);
	imsi[4] = 0;
	d->da_year = _tstoi(imsi);

	_tcsncpy(imsi, &buf[5], 2);
	imsi[2] = 0;
	d->da_mon = _tstoi(imsi);

	_tcsncpy(imsi, &buf[8], 2);
	imsi[2] = 0;
	d->da_day = _tstoi(imsi);
}

void CommaBlockString :: GetTime(struct time *t)
{
	TCHAR buf[20];
	TCHAR imsi[20];

	GetString(buf, 19);

	_tcsncpy(imsi, &buf[0], 2);
	imsi[2] = 0;
	t->ti_hour = _tstoi(imsi);

	_tcsncpy(imsi, &buf[3], 2);
	imsi[2] = 0;
	t->ti_min = _tstoi(imsi);

	_tcsncpy(imsi, &buf[6], 2);
	imsi[2] = 0;
	t->ti_sec = _tstoi(imsi);
}

void CommaBlockString :: GetDateTime(SYSTEMTIME *t)
{
	TCHAR buf[80];
	TCHAR imsi[20];

	ZeroMemory(t, sizeof(SYSTEMTIME));

	GetString(buf, sizeof(buf));

	_tcsncpy(imsi, &buf[0], 4);
	imsi[4] = 0;
	t->wYear = _tstoi(imsi);

	_tcsncpy(imsi, &buf[5], 2);
	imsi[2] = 0;
	t->wMonth = _tstoi(imsi);

	_tcsncpy(imsi, &buf[8], 2);
	imsi[2] = 0;
	t->wDay = _tstoi(imsi);

	_tcsncpy(imsi, &buf[11], 2);
	imsi[2] = 0;
	t->wHour = _tstoi(imsi);

	_tcsncpy(imsi, &buf[14], 2);
	imsi[2] = 0;
	t->wMinute = _tstoi(imsi);

	_tcsncpy(imsi, &buf[17], 2);
	imsi[2] = 0;
	t->wSecond = _tstoi(imsi);
}

void CommaBlockString :: GetBool(bool &val)
{
	CString buf;

	GetString(buf);

	if(_tcsicmp(buf, _T("TRUE")) == 0)	
		val = true;
	else							
		val = false;

	/*
	TCHAR buf[80];

	GetString(buf, sizeof(buf));

	if(_tcsicmp(buf, _T("TRUE")) == 0)	val = true;
	else							val = false;*/
}

void CommaBlockString :: Skip()
{
	CString s;

	GetString(s);
}


