#include "stdafx.h"
#include <dos.h>

#include <tools.h>

TimeOutClass :: TimeOutClass()
{
	Reset();
} 

void TimeOutClass :: SetTime(int sec)
{
	SYSTEMTIME t;

	GetLocalTime(&t);
	old_sec = (char)t.wSecond;
	nCurrCount = sec;
}

void TimeOutClass :: Reset()
{
	SetTime(0);
}

int TimeOutClass :: IsTimeOut(int time_out)
{
	SYSTEMTIME t;

	GetLocalTime(&t);

	if((char)t.wSecond != old_sec) {
		if((char)t.wSecond < old_sec)
			nCurrCount += t.wSecond+60-old_sec;	
		else
			nCurrCount += t.wSecond-old_sec;	

		old_sec = (char)t.wSecond;
//		nCurrCount++;
	}

	if(nCurrCount >= time_out)	return 1;	// 원래 이 비교문은 if 문장속에 들어 있었는데 
											// IsTimeOut을 연속해서 여러번 부를때는 이 문장까지 들어오지 못하므로 
											// 비교문만 밖으로 나왔다.
	//Sleep(1);	Sleep을 여기서 주면 안됨
	// TimeOut을 사용하는 Method에서 따로처리할 것
	return 0;
}





