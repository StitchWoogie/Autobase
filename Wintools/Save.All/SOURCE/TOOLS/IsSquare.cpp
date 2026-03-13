// ¿µ¹® O.K
#include "stdafx.h"
#include <tools.h>

int IsSquare(DWORD value)
{
	DWORD seed = 1;
   int i;

	for(i = 1; i <= 30; i++) {
		if(seed == value)	return 1;
		if(seed > value)	return 0;
      seed *= 2;
	}

   return 0;
}