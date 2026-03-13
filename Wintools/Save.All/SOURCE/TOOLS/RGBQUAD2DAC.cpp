#include "stdafx.h"

#include <glib.h>

//--------------------------------------------------------------------------------------------
//	RGBQUAD 구조체를 DAC 구조체로 바꾼다.
//--------------------------------------------------------------------------------------------

void RGBQUAD2DAC(RGBQUAD *quad, BYTE *dac, int count)
{
	int i;

	for(i = 0; i < count; i++) {
		dac[i*3+0] = quad[i].rgbRed;
		dac[i*3+1] = quad[i].rgbGreen;
		dac[i*3+2] = quad[i].rgbBlue;
	}
}
