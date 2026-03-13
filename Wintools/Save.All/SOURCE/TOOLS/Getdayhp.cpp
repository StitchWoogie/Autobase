#include "stdafx.h"
#include <tools.h>
#include <dos.h>

DWORD GetDayHap(int year, int month, int day)
{
	 int limit[] = {31,28,31,30,31,30,31,31,30,31,30,31};
	 int i;
	 long dayhap;

	 dayhap = (((long)year-1)*365)+((year-1)/4)-((year-1)/100)+((year-1)/400);

	 if((year%400) == 0)         limit[1] = 29;
	 else if((year%100) == 0 )	limit[1] = 28;
	 else if((year%4) == 0)	limit[1] = 29;
	 else			limit[1] = 28;

	 if(month != 1)
		 for(i = 0; i < month-1; i++)
	  dayhap += limit[i];
	 dayhap += day;
	 return dayhap;
}

DWORD GetDayHap(struct date *d)
{
	return GetDayHap(d->da_year, d->da_mon, d->da_day);
}

