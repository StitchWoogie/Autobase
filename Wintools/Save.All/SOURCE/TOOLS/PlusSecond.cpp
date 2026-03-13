#include "stdafx.h"
#include <dos.h>

#include <tools.h>

void PlusSecond(SYSTEMTIME *t)
{
	if(t->wSecond >= 59) {
		PlusMin(t);
		t->wSecond = 0;
	}
	else {
		t->wSecond ++;
	}
}
