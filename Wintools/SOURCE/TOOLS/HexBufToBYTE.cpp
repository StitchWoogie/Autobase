#include "stdafx.h"
#include <tools.h>

BYTE HexBufToBYTE(char *buf)
{
	return (BYTE)HexBufToValue(buf, 2);
	/*
	BYTE val = 0;

	if(buf[0] >= '0' && buf[0] <= '9')	val |= ((buf[0]-'0') << 4);
	else                                val |= ((buf[0]-'A'+10) << 4);

	if(buf[1] >= '0' && buf[1] <= '9')	val |= ((buf[1]-'0') << 0);
	else                                val |= ((buf[1]-'A'+10) << 0);

	return val;
	*/
}





