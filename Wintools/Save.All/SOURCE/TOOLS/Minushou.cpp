#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 시간에서 1시간을 뺀다.
//----------------------------------------------------------------------------

void MinusHour(int &year, int &mon, int &day, int &hour)
{
	if(hour <= 0) {
		hour = 23;
		MinusDay(year, mon, day);
	}
	else {
		hour--;
	}
}

void MinusHour(struct date *d, struct time *t)
{
	if(t->ti_hour <= 0) {
		t->ti_hour = 23;
		MinusDay(d);
	}
	else {
		t->ti_hour--;
	}
}

void MinusHour(SYSTEMTIME *t)
{
	if(t->wHour <= 0) {
		t->wHour = 23;
		MinusDay(t);
	}
	else {
		t->wHour--;
	}
}


