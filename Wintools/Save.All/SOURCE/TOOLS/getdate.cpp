#include "stdafx.h"
#if	defined (__BORLANDC__)
#else
#include <tools.h>

void getdate(struct date *d)
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
	d->da_year = st.wYear;
	d->da_mon = (char)st.wMonth;
	d->da_day = (char)st.wDay;
}
#endif
