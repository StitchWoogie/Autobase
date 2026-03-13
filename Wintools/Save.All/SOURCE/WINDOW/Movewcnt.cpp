#include "stdafx.h"
#include <glib.h>

void	MoveWindowCenter(HWND hwnd)
{
	RECT	rc;

	GetWindowRect(hwnd, &rc);
	SetWindowPos(hwnd, NULL,
				 (GetSystemMetrics(SM_CXSCREEN) - (rc.right - rc.left)) / 2,
				 (GetSystemMetrics(SM_CYSCREEN) - (rc.bottom - rc.top)) / 2 + 25,
				 0, 0, SWP_NOSIZE | SWP_NOACTIVATE);
}





