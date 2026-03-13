#include "stdafx.h"
#include <dos.h>

#include <tools.h>

void PlusHour(struct date *d, struct time *t)
{
	if(t->ti_hour >= 23) {
		t->ti_hour = 0;
		PlusDay(d);
	}
	else {
		t->ti_hour++;
	}
}

void PlusHour(int &year, int &mon, int &day, int &hour)
{
	struct date d;
	struct time t;

	d.da_year = year;
	d.da_mon  = mon;
	d.da_day  = day;
	t.ti_hour = hour;
	
	PlusHour(&d, &t);

	year = d.da_year;
	mon  = d.da_mon ;
	day  = d.da_day ;
	hour = t.ti_hour;
}

void PlusHour(SYSTEMTIME *t)
{
	if(t->wHour >= 23) {
		t->wHour = 0;
		PlusDay(t);
	}
	else {
		t->wHour++;
	}
}
