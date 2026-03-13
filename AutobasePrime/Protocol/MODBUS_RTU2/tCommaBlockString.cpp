// english O.K
#include "stdafx.h"
#include <string.h>
#include <stdlib.h>
#include <tchar.h>
                                  
#include <tools.h>

tCommaBlockString :: tCommaBlockString(TCHAR *string)
{
	scanBufPos = 0;
	scanBufHap = 0;
	scanBuf = NULL;
	cBlockCode = ',';

	if(string != NULL)	Set(string);
}

tCommaBlockString :: ~tCommaBlockString()
{
	if(scanBuf) {
		delete scanBuf;
		scanBuf = NULL;
	}
}

void tCommaBlockString :: Set(LPCTSTR string)
{
	if(scanBuf) {
		delete scanBuf;
		scanBuf = NULL;
	}

	scanBuf = new TCHAR[_tcslen(string)+1];
	if(scanBuf == NULL) {
		bell(50);
		return;
	}

	scanBufPos = 0;
	_tcscpy(scanBuf, string);
	scanBufHap = _tcslen(scanBuf);
}

void tCommaBlockString :: GetString(TCHAR *string, int limit)
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

void tCommaBlockString :: GetStringTotalRemain(TCHAR *string, int limit)
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


void tCommaBlockString :: GetInt(int &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = _ttoi(buf);
}

void tCommaBlockString :: GetWORD(WORD &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = (WORD)_ttoi(buf);
}

//------------------------------------------------------------------------------
//
//------------------------------------------------------------------------------

void tCommaBlockString :: GetHexDWORD(DWORD &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	
	_tcsupr(buf);

	int pos;
	unsigned u;
	DWORD imsi;
	DWORD gob = 1;

	val = 0;

	for(u = 0, pos = _tcslen(buf)-1; u < _tcslen(buf); u++, pos--) {
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

void tCommaBlockString :: GetHexWORD(WORD &val)
{
	DWORD retn;

	GetHexDWORD(retn);

	val = (WORD)retn;
}

void tCommaBlockString :: GetInt(short &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = (WORD)_ttoi(buf);
}


void tCommaBlockString :: GetChar(char &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = _ttoi(buf);
}

void tCommaBlockString :: GetBYTE(BYTE &val)
{
	TCHAR buf[10];
	GetString(buf, 10);
	if(buf[0] == 0)	{
		val = 0;
		return;
	}

	val = (BYTE)_ttoi(buf);
}

void tCommaBlockString :: GetLong(long &val)
{
	TCHAR buf[20];
	GetString(buf, 20);
	if(buf[0] == 0)	{
		val = 0L;
		return;
	}

	val = _ttol(buf);
}

void tCommaBlockString :: GetDWORD(DWORD &val)
{
	TCHAR buf[20];
	GetString(buf, 20);
	if(buf[0] == 0)	{
		val = 0L;
		return;
	}

	val = _ttol(buf);
}

//----------------------------------------------------------------------------
//	DI address 는 앞세자리는 10진수, 뒤의 한자리는 16진수로 구성된다.
//----------------------------------------------------------------------------

void tCommaBlockString :: GetAddressDI(WORD &val)
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
	val += (_ttoi(buf)*16);
}

long double tcstofloat(TCHAR *buf)
{
	char ex[100];

	wcstombs(ex, buf, 100);

	return atof(ex);
}

void tCommaBlockString :: GetFloat(float &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	if(buf[0] == 0)	{
		val = (float)0.0;
		return;
	}
	val = (float)tcstofloat(buf);
}

void tCommaBlockString :: GetDouble(double &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	if(buf[0] == 0)	{
		val = (float)0.0;
		return;
	}
	val = (double)tcstofloat(buf);
}

void tCommaBlockString :: GetLongDouble(long double &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	if(buf[0] == 0)	{
		val = (long double)0.0;
		return;
	}

	val = (long double)tcstofloat(buf);
}

void tCommaBlockString :: GetCOLORREF(COLORREF &val)
{
	TCHAR buf[20];
	GetString(buf, 19);

	val = _ttol(buf);
}

void tCommaBlockString :: GetDate(struct date *d)
{
	TCHAR buf[20];
	TCHAR imsi[20];

	GetString(buf, 19);

	_tcsncpy(imsi, &buf[0], 4);
	imsi[4] = 0;
	d->da_year = _ttoi(imsi);

	_tcsncpy(imsi, &buf[5], 2);
	imsi[2] = 0;
	d->da_mon = _ttoi(imsi);

	_tcsncpy(imsi, &buf[8], 2);
	imsi[2] = 0;
	d->da_day = _ttoi(imsi);
}

void tCommaBlockString :: GetTime(struct time *t)
{
	TCHAR buf[20];
	TCHAR imsi[20];

	GetString(buf, 19);

	_tcsncpy(imsi, &buf[0], 2);
	imsi[2] = 0;
	t->ti_hour = _ttoi(imsi);

	_tcsncpy(imsi, &buf[3], 2);
	imsi[2] = 0;
	t->ti_min = _ttoi(imsi);

	_tcsncpy(imsi, &buf[6], 2);
	imsi[2] = 0;
	t->ti_sec = _ttoi(imsi);
}


