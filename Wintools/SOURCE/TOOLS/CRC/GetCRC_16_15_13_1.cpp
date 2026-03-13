// english O.K
#include "stdafx.h"

WORD GetCRC_16_15_13_1(unsigned char *buf, int size)
{
	unsigned crc = 0xFFFF;
	//unsigned crc = 0;
	int i, j;
	static unsigned int bitmask[8] = { 1, 2, 4, 8, 16, 32, 64, 128 };
	unsigned int a, b, c, input;

	for(j = 0; j < size; j++) {
		for(i = 0; i < 8; i++) {
			if(buf[j] & bitmask[i]) 	input = 1;
			else               	    	input = 0;

			if(crc & 1) c = 1;
			else	  c = 0;
			c = c ^ input;

			if(crc & 2) b = 1;
			else        b = 0;
			b = c ^ b;

			if(crc & 0x4000) a = 1;
			else	       	  a = 0;
			a = a ^ c;

			crc = crc >> 1;
			crc = (crc & 0x7FFF) + (c << 15);//(c*0x8000;
			crc = (crc & 0xDFFF) + (a << 13);//a*0x2000;
			crc = (crc & 0xFFFE) + b;
		}
	}
	return crc;
}

WORD GetCRC_16_15_13_1_0000(unsigned char *buf, int size)
{
	unsigned crc = 0;
	int i, j;
	static unsigned int bitmask[8] = { 1, 2, 4, 8, 16, 32, 64, 128 };
	unsigned int a, b, c, input;

	for(j = 0; j < size; j++) {
		for(i = 0; i < 8; i++) {
			if(buf[j] & bitmask[i]) 	input = 1;
			else               	    	input = 0;

			if(crc & 1) c = 1;
			else	  c = 0;
			c = c ^ input;

			if(crc & 2) b = 1;
			else        b = 0;
			b = c ^ b;

			if(crc & 0x4000) a = 1;
			else	       	  a = 0;
			a = a ^ c;

			crc = crc >> 1;
			crc = (crc & 0x7FFF) + (c << 15);//(c*0x8000;
			crc = (crc & 0xDFFF) + (a << 13);//a*0x2000;
			crc = (crc & 0xFFFE) + b;
		}
	}
	return crc;
}
