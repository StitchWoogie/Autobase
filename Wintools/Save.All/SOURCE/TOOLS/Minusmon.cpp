#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 시간에서 1시간을 뺀다.
//----------------------------------------------------------------------------

void MinusMonth(int &year, int &mon)
{
	if(mon <= 1) {
		mon = 12;
		MinusYear(year);
	}
	else {
		mon--;
	}
}

void MinusMonth(struct date *d)
{
	if(d->da_mon <= 1) {
		d->da_mon = 12;
		MinusYear(d);
	}
	else {
		d->da_mon--;
	}
}

void MinusMonth(SYSTEMTIME *t)
{
	if(t->wMonth <= 1) {
		t->wMonth = 12;
		t->wYear--;
	}
	else {
		t->wMonth--;
	}
}


