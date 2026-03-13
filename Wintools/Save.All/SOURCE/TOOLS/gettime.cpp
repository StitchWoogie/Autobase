// english O.K
#include "stdafx.h"
#if	defined (__BORLANDC__)
#else

#include <tools.h>


void gettime(struct time *t)
{
/* 
	typedef struct _SYSTEMTIME {  // st 
    WORD wYear; 
    WORD wMonth; 
    WORD wDayOfWeek; 
    WORD wDay; 
    WORD wHour; 
    WORD wMinute; 
    WORD wSecond; 
    WORD wMilliseconds; 
} SYSTEMTIME; 
*/
	SYSTEMTIME st;

	GetLocalTime(&st);
	t->ti_hour = (char)st.wHour;
	t->ti_min = (char)st.wMinute;
	t->ti_sec = (char)st.wSecond;
	t->ti_hund = st.wMilliseconds/10;
}
#endif