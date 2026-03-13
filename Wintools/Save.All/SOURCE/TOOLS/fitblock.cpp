#include "stdafx.h"
#include <windows.h>

//------------------------------------------------------------------------------
//	2의 i승으로 떨어지는 수를 찾는다.
// Windows 3.1의 Global 메모리는 65536byte 가 넘을때는 블럭을 2의 승수로 맞춘다.
//------------------------------------------------------------------------------

DWORD GetFitGlobalBlockSize(DWORD one_block_size)
{
	// 2의 i승으로 떨어지는 수를 찾는다.
	DWORD val = 0;
	int i;

	for(i = 0; i < 30; i++) {
		if(i == 0)		val = 1;
		else			val *= 2;
		if(val >= one_block_size)	break;
	}

	return val;
}