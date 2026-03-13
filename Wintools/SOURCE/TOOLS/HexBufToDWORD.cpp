#include "stdafx.h"
#include <tools.h>

DWORD HexBufToDWORD(char *buf)
{
	DWORD val;

	val = HexBufToWORD(buf)*0x10000L+HexBufToWORD(&buf[4]);

	return val;
}




