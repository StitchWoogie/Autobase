#include "stdafx.h"
#include <tools.h>

DWORD GetMonHap(int year, int month)
{
	return ( (long)year*12+month-1 );
}

