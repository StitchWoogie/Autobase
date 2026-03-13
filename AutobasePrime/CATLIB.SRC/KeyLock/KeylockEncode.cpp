// TAG size O.K
#include "stdafx.h"

#include <dataswap.h>

static void ScrembleBuffer(int *buf, int size, DWORD seed)
{
	int temp;
	int i;
	int pos = 100;

	for(i = 0; i < size; i++) {
		pos += (seed >> (i%23)) & 0xFFFF;
		pos %= size;

		temp = buf[i];
		buf[i] = buf[pos];
		buf[pos] = temp;
	}
}

void KeyLockEncodeData(BYTE *buf, int size, DWORD seed)
{
	int i, j;
	BYTE c = 55;
	int pos;
	StackInt order(size);
	StackBYTE src(size);

	for(i = 0; i < size; i++) {
		order.data[i] = i;
	}

	ScrembleBuffer(order.data, size, seed);

	for(j = 0; j < 11; j++) {
		c = 55;
		memcpy(src.data, buf, size);

		for(i = 0; i < size; i++) {
			pos = order.data[i];
			c ^= src.data[pos];
			c ^= (seed >> (i%23)) & 0xFF;
			buf[i] = c;
		}
	}
}

void KeyLockDecodeData(BYTE *buf, int size, DWORD seed)
{
	int i, j;
	BYTE c;
	int pos;
	StackInt order(size);
	StackBYTE src(size);

	for(i = 0; i < size; i++) {
		order.data[i] = i;
	}

	ScrembleBuffer(order.data, size, seed);

	for(j = 0; j < 11; j++) {
		memcpy(src.data, buf, size);
		for(i = 0; i < size; i++) {
			pos = order.data[i];
			//c = src[i];
			if(i == 0)	c = src.data[i]^55;
			else		c = src.data[i]^src.data[i-1];
			c ^= (seed >> (i%23)) & 0xFF;
			buf[pos] = c;
		}
	}
}
