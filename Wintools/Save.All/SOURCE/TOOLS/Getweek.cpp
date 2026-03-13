#include "stdafx.h"
#include <tools.h>

int  GetWeekDay(int year, int month, int day)
{
	long dayhap = GetDayHap(year, month, day);

	while(dayhap > 14000L)	dayhap -= 14000L;

	return (((WORD)dayhap) % 7);
}


