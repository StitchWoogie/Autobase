#include "stdafx.h"
#include <dos.h>

#include <tools.h>

//----------------------------------------------------------------------------
//	주어진 분에서 1분을 뺀다.
//----------------------------------------------------------------------------

void MinusSecond(SYSTEMTIME *t)
{
	if(t->wSecond <= 0) {
		t->wSecond = 59;
		MinusMinute(t);
	}
	else {
		t->wSecond--;
	}
}
