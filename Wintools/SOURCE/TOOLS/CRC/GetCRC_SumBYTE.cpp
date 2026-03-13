// english O.K
#include "stdafx.h"
#include <compiler.hpp>
#include <crc.hpp>

//--------------------------------------------------------------
// 주어진 버퍼를 8bit PLUS overflow로 계산한다.
//--------------------------------------------------------------

BYTE GetCRC_SumBYTE(void *buf, int length)
{
	int i;
	BYTE crc = 0;
					      
	for(i = 0; i < length; i++)
		crc += ((BYTE*)buf)[i];

	return crc;
}

BYTE GetCRC_SUM8(BYTE *buf, int size)
{
	return GetCRC_SumBYTE(buf, size);
}

