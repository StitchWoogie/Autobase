#include "stdafx.h"
#include <glib.h>

void PushBox3(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color)
{
	gcls(hdc, x1+2, y1+2, x2-2, y2-2, color);
	PushRectangle3(hdc, x1, y1, x2, y2);
}

