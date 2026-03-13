#include "stdafx.h"

bool IsOsVersionNtPlatform()
{
	OSVERSIONINFO osvi;
	ZeroMemory(&osvi, sizeof(OSVERSIONINFO));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&osvi);
	if(osvi.dwPlatformId == VER_PLATFORM_WIN32_NT)	return true;
	return false;
}
