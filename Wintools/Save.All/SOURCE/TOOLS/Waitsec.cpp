//------------------------------------------------------------------------------
//	지정된 시간동안 가다린다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <dos.h>

#include <tools.h>

void WaitSec(int sec)
{
	struct time t;
	int old, curr=0;

	gettime(&t);

	old = t.ti_sec;

	while(1) {
		gettime(&t);
		if(t.ti_sec != old) {
			old = t.ti_sec;
			curr++;
			if(curr >= sec) {
				return;
			}
		}
	}
}


