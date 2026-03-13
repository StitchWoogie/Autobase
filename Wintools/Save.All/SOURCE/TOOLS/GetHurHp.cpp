#include "stdafx.h"
#include <dos.h>

#include <tools.h>

DWORD GetHourHap(int year, int month, int day, int hour)
{
	return GetDayHap(year, month, day)*24+hour;
}

DWORD GetHourHap(struct date *d, struct time *t)
{
	return GetHourHap(d->da_year, d->da_mon, d->da_day, t->ti_hour);
}

