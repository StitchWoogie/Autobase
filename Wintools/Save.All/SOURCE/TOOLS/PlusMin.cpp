#include "stdafx.h"
#include <dos.h>

#include <tools.h>

void PlusMin(struct date *d, struct time *t)
{
	if(t->ti_min >= 59) {
		t->ti_min = 0;
		PlusHour(d, t);
	}
	else {
		t->ti_min++;
	}
}

void PlusMin(int &year, int &mon, int &day, int &hour, int &min)
{
	if(min >= 59) {
		min = 0;
		PlusHour(year, mon, day, hour);
	}
	else {
		min++;
	}
}

void PlusMin(SYSTEMTIME *t)
{
	if(t->wMinute >= 59) {
		t->wMinute = 0;
		PlusHour(t);
	}
	else {
		t->wMinute++;
	}
}