#include "stdafx.h"
#include <glib.h>

void MsgBoxLocalMemoryLow(HWND hwnd, char *string)
{
#if	defined(COMPILE_ENGLISH)
	MessageBox(hwnd, "로컬 메모리가 부족합니다.", string, MB_OK);
#else
	MessageBox(hwnd, "Local memory Insufficient.", string, MB_OK);
#endif
}

void MsgBoxGlobalMemoryLow(HWND hwnd, char *string)
{
#if	defined(COMPILE_ENGLISH)
	MessageBox(hwnd, "글로벌 메모리가 부족합니다.", string, MB_OK);
#else
	MessageBox(hwnd, "Global memory Insufficient.", string, MB_OK);
#endif
}