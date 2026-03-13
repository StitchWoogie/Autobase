// TAG size O.K
#include "stdafx.h"

bool IsOsVersion98()
{
	OSVERSIONINFO osvi;
	ZeroMemory(&osvi, sizeof(OSVERSIONINFO));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&osvi);
	if(osvi.dwMajorVersion != 4)	return false;// 4 = 95,98,me,nt4
	if(osvi.dwMinorVersion != 10)	return false;// 10 = 98
	return true;
}
