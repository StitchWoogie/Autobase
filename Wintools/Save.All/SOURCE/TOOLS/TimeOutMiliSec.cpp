#include "stdafx.h"
#include <dos.h>

#include <tools.h>

TimeOutMiliSecClass :: TimeOutMiliSecClass()
{
	Reset();
} 

void TimeOutMiliSecClass :: SetTime(DWORD milisec)
{
	//typedef struct _SYSTEMTIME {  // st     WORD wYear; WORD wMonth; 
    //WORD wDayOfWeek;     WORD wDay;     WORD wHour;     WORD wMinute; 
    //WORD wSecond;     WORD wMilliseconds; } SYSTEMTIME; 

	GetLocalTime(&old_t);
	dwCurrCount = milisec;
}

void TimeOutMiliSecClass :: Reset()
{
	SetTime(0);
}

int TimeOutMiliSecClass :: IsTimeOut(DWORD time_out)
{
	SYSTEMTIME t;

	GetLocalTime(&t);

	if(t.wSecond == old_t.wSecond) {	// 초가 변함 없다.
		dwCurrCount += (t.wMilliseconds-old_t.wMilliseconds);
	}
	else if(t.wSecond > old_t.wSecond) {	// 초는 변하고 분은 변하지 않았다.
		dwCurrCount += ((t.wSecond*1000L+t.wMilliseconds)-
			            (old_t.wSecond*1000L+old_t.wMilliseconds));
	}
	else {	// 분이 넘어가서 초 단위가 작아졌다.
		dwCurrCount += (((t.wSecond+60)*1000L+t.wMilliseconds)-
			            (old_t.wSecond*1000L+old_t.wMilliseconds));	
	}

	memcpy(&old_t, &t, sizeof(SYSTEMTIME));
	if(dwCurrCount >= time_out)	return 1;

	//Sleep(1);	// Sleep을 여기서 주면 안됨
	// TimeOut을 사용하는 Method에서 따로처리할 것
	return 0;
}
