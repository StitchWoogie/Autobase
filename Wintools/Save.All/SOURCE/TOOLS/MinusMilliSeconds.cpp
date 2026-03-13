#include "stdafx.h"
#include <dos.h>

#include <tools.h>

void MinusMilliSeconds(SYSTEMTIME *t, int milli)
{
	int remain = (int)t->wMilliseconds-milli;

	while(remain < 0) 
	{
		MinusSecond(t);
		remain += 1000;
	}

	t->wMilliseconds = remain;
}


