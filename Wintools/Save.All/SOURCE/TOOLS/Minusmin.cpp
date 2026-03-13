#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 분에서 1분을 뺀다.
//----------------------------------------------------------------------------

void MinusMin(int &year, int &mon, int &day, int &hour, int &min)
{
	if(min <= 0) {
		min = 59;
		MinusHour(year, mon, day, hour);
	}
	else {
		min--;
	}
}

void MinusMinute(SYSTEMTIME *t)
{
	if(t->wMinute <= 0) {
		t->wMinute = 59;
		MinusHour(t);
	}
	else {
		t->wMinute--;
	}
}

//----------------------------------------------------------------------------
//	주어진 분에서 1분을 뺀다.
//----------------------------------------------------------------------------

void MinusMin(struct date *d, struct time *t)
{
	int year = d->da_year;
	int mon  = d->da_mon;
	int day  = d->da_day;
	int hour = t->ti_hour;
	int min  = t->ti_min;

	MinusMin(year, mon, day, hour, min);
	
	d->da_year = year;
	d->da_mon  = mon;
	d->da_day  = day;
	t->ti_hour = hour;
	t->ti_min  = min;
}


