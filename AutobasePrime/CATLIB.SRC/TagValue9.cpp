// TAG size O.K
//#include "stdafx.h"
#include <afxwin.h>
#include <stdio.h>

#include <tools.h>
#include <dataswap.h>

#include "cattag.h"
#include "totalcfg.h"

/*
static BYTE GetSum8(const char *s)
{
	BYTE c = 0;

	for(int i = 0; i < (int)strlen(s); i++)
	{
		c += (BYTE)(s[i]%256);
		c += (BYTE)(s[i]/256);
	}

	return c;
}*/

static BYTE GetSum8(const char *s)
{
	CStringW ws;
	ws = s;

	BYTE c = 0;

	for(int i = 0; i < (int)wcslen(ws); i++)
	{
		c += (BYTE)(ws[i]%256);
		c += (BYTE)(ws[i]/256);
	}

	return c;
}

int Tag9GetCurr(const char *tag, CString &curr)
{
	BYTE sum = GetSum8(tag);

	StackChar data(10000);

	CString sub_name;
	sub_name.Format("%d\\%s", sum, tag);

	LoadRegAutoBaseConfig("TagShare", sub_name, "Value", "", data.data, 10000);

	curr = data.data;

	return 1;
}

static unsigned int MakeCode(unsigned int seed1, unsigned int seed2)
{
	unsigned int code = seed1;

	code += 0x7F43;
	code ^= seed2;
	code ^= 0x5678;
	code += seed2;

	return code;
}

__int64 GetTicks(SYSTEMTIME t)
{
	DWORD day_hap = GetDayHap(t.wYear, t.wMonth, t.wDay)-1;
	//DWORD day_hap = GetDayHap(1, 1, 1)-1;

	__int64 ticks;

	ticks = day_hap*24+t.wHour;
	ticks = ticks*60+t.wMinute;
	ticks = ticks*60+t.wSecond;
	ticks = ticks*1000+t.wMilliseconds;
	ticks = ticks*10000;
		
	return ticks;
}

bool TagSetCurr(const char *tag, double val)
{
	BYTE sum = GetSum8(tag);

	DWORD seed1;

	LoadRegAutoBaseConfig("TagShare", NULL, "Code", 0, seed1);

	SYSTEMTIME t;

	GetLocalTime(&t);
	__int64 seed2 = GetTicks(t);
	unsigned int code = MakeCode(seed1, (unsigned int)seed2);

	CString sub_name;
	CString buf;

	sub_name.Format("%d\\%s", sum, tag);
	buf.Format("%I64d,%I32u,%f", seed2, code, val); // !64 format

	SaveRegAutoBaseConfig("TagShare", sub_name, "Write", buf);
	SaveRegAutoBaseConfig("TagShare", sub_name, "bWrite", 1);
	int rand = (int)seed2;
	SaveRegAutoBaseConfig("TagShare", NULL, "RandomID", rand);
	return true;
}