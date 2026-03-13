#include "stdafx.h"
#include <stdio.h>

//---------------------------------------------------------------------------
//	Get One Line In Text File
//	must open "rb"
//---------------------------------------------------------------------------

int tTextGetOneLine(FILE *in, TCHAR *buf, int limit)
{
	wint_t fc;
	int bufpos = 0;

	while( (fc = fgetwc(in))  != WEOF) {
		if(fc == 0x0d || fc == 0x0A) {                     // 개행 문자
			if(fc == 0x0d)	_fgettc(in);
			buf[bufpos] = 0;
			return 1;
		}
		else {   			//
			if(bufpos < limit-1)
			buf[bufpos++] = fc;
		}
	}
	if(bufpos == 0)	return 0;

	buf[bufpos] = 0;

	return 1;
}