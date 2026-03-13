#include "stdafx.h"
//---------------------------------------------------------------------------
//	주어진 달의 마지막 날을 구한다.
//--------------------------------------------------------------------------

int getmonthlimit(int year, int month)
{
   char limit[12] = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

   if(month != 2)	return(limit[month-1]);

   if(year%400 == 0)	return 29;
   if(year%100 == 0)	return 28;
   if(year%4 == 0)	return 29;
   return 28;
}

