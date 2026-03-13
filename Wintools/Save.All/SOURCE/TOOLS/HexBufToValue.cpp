#include "stdafx.h"
#include <tools.h>

DWORD HexBufToValue(char *buf, int count)
{
	DWORD val = 0;
	int i;

	for(i = 0; i < count; i++) {
		if(i != 0)	val = val << 4;
		if(buf[i] >= '0' && buf[i] <= '9')		val |= (buf[i]-'0');
		else if(buf[i] >= 'A' && buf[i] <= 'F')	val |= (buf[i]-'A'+10);
		else;
	}

	return val;
}





