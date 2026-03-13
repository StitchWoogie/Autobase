#include "stdafx.h"

#include <tools.h>

int IsDayExist(int year, int mon, int day)
{
	int limit = getmonthlimit(year, mon);
	if(day > limit)	return 0;
	if(day <= 0)		return 0;
	return 1;
}
