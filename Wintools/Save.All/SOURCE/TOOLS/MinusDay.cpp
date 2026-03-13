#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 날짜 구조체에서 하루를 뺀다.
//----------------------------------------------------------------------------

void MinusDay(int &year, int &mon, int &day)
{
	if(day <= 1) {		// 1일 일때
		if(mon <= 1) {	// 1 월 1 일 일때
			year--;
			mon = 12;
			day = 31;
		}
		else {                    // ? 월 1 일 일때
			mon--;
			day = getmonthlimit(year, mon);
		}
	}
	else {
		day--;
	}
}


void MinusDay(SYSTEMTIME *t)
{
	if(t->wDay <= 1) {		// 1일 일때
		if(t->wMonth <= 1) {	// 1 월 1 일 일때
			t->wYear--;
			t->wMonth = 12;
			t->wDay = 31;
		}
		else {                    // ? 월 1 일 일때
			t->wMonth--;
			t->wDay = getmonthlimit(t->wYear, t->wMonth);
		}
	}
	else {
		t->wDay--;
	}
}

//----------------------------------------------------------------------------
//	주어진 날짜 구조체에서 하루를 뺀다.
//----------------------------------------------------------------------------

void MinusDay(struct date *d)
{
	int day, mon, year;

	year = d->da_year;
	mon  = d->da_mon;
	day  = d->da_day;

	MinusDay(year, mon, day);

	d->da_year = year;
	d->da_mon  = mon;
   d->da_day  = day;
}
