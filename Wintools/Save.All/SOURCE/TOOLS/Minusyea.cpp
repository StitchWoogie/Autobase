#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 시간에서 1시간을 뺀다.
//----------------------------------------------------------------------------

void MinusYear(int &year)
{
	if(year <= 0)
		year = 9999;
	else
		year--;
}

void MinusYear(struct date *d)
{
	if(d->da_year <= 0)
		d->da_year = 9999;
	else
		d->da_year--;
}


