// english O.K
#include "stdafx.h"
#include <glib.h>

//--------------------------------------------------------------
//	주어진 버퍼를 16bit PLUS overflow로 계산한다.
//--------------------------------------------------------------

unsigned short int GetCRC16(BYTE *buf, int length)
{
	int i;
	unsigned short int crc = 0;
                                              
	for(i = 0; i < length; i++)
		crc += buf[i];

	return crc;
}

WORD GetCRC_SumWORD(void *buf, int length)
{
	return GetCRC16((BYTE*)buf, length);
}