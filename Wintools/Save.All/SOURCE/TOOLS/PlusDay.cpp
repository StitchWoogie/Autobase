#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 날짜 구조체에서 하루를 더한다.
//----------------------------------------------------------------------------

void PlusDay(struct date *d)
{
	if(d->da_day >= getmonthlimit(d->da_year, d->da_mon)) {	 // 그 달의 마지막 날
		if(d->da_mon >= 12) {	// 12월 31일 일때
			d->da_year++;
			d->da_mon = 1;
			d->da_day = 1;
		}
		else {                    // ? 월 31 일 일때
			d->da_mon++;
			d->da_day = 1;
		}
	}
	else {
		d->da_day++;
	}
}

void PlusDay(int &year, int &mon, int &day)
{
	struct date d;

	d.da_year = year;
	d.da_mon = mon;
	d.da_day = day;

	PlusDay(&d);

	year = d.da_year;
	mon  = d.da_mon;
	day  = d.da_day;
}

void PlusDay(SYSTEMTIME *t)
{
	if(t->wDay >= getmonthlimit(t->wYear, t->wMonth)) {	 // 그 달의 마지막 날
		if(t->wMonth >= 12) {	// 12월 31일 일때			
			t->wYear++;
			t->wMonth = 1;
			t->wDay = 1;
		}
		else {                    // ? 월 31 일 일때
			t->wMonth++;
			t->wDay = 1;
		}
	}
	else {
		t->wDay++;
	}
}