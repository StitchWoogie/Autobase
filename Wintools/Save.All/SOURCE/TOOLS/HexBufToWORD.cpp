#include "stdafx.h"
#include <tools.h>

WORD HexBufToWORD(char *buf)
{
	return (WORD)HexBufToValue(buf, 4);
}




