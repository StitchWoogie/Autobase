#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 날짜 구조체에서 한달을 더한다.
//----------------------------------------------------------------------------

void PlusMonth(struct date *d)
{
	if(d->da_mon >= 12) {	// 12월 31일 일때
		d->da_year++;
		d->da_mon = 1;
	}
    else {                    // ? 월 31 일 일때
		d->da_mon++;
    }
}

void PlusMonth(SYSTEMTIME *t)
{
	if(t->wMonth >= 12) {	// 12월 31일 일때
		t->wYear++;
		t->wMonth = 1;
	}
    else {                    // ? 월 31 일 일때
		t->wMonth++;
    }
}

void PlusMonth(int &year, int &month)
{
	struct date d;

	d.da_year = year;
	d.da_mon = month;
	PlusMonth(&d);
	year = d.da_year;
	month = d.da_mon;
}