#include "stdafx.h"
#include <tools.h>

DWORD MOTO2IBM(DWORD value)
{
	DWORD retn = 0L;

	retn |= (value << 24) & (DWORD)0xFF000000L;
	retn |= (value <<  8) & (DWORD)0x00FF0000L;
	retn |= (value >>  8) & (DWORD)0x0000FF00L;
	retn |= (value >> 24) & (DWORD)0x000000FFL;

	return retn;
}

int MOTO2IBM(int value)
{
	int retn = 0L;

	retn |= (value << 24) & (int)0xFF000000L;
	retn |= (value <<  8) & (int)0x00FF0000L;
	retn |= (value >>  8) & (int)0x0000FF00L;
	retn |= (value >> 24) & (int)0x000000FFL;

	return retn;
}

USHORT MOTO2IBM(USHORT value)
{
	USHORT retn = 0;

	retn |= (value << 8) & 0xFF00;
	retn |= (value >> 8) & 0x00FF;

	return retn;
}

SHORT MOTO2IBM(SHORT value)
{
	SHORT retn = 0;

	retn |= (value << 8) & 0xFF00;
	retn |= (value >> 8) & 0x00FF;

	return retn;
}


