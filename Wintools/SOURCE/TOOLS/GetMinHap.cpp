#include "stdafx.h"
#include <dos.h>

#include <tools.h>

DWORD GetMinHap(int year, int month, int day, int hour, int min)
{
	return GetHourHap(year, month, day, hour)*60+min;
}

DWORD GetMinHap(struct date *d, struct time *t)
{
	return GetMinHap(d->da_year, d->da_mon, d->da_day, t->ti_hour, t->ti_min);
}

DWORD GetMinHap(SYSTEMTIME *t)
{
	return GetMinHap(t->wYear, t->wMonth, t->wDay, t->wHour, t->wMinute);
}

