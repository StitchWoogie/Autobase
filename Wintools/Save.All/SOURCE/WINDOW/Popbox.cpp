#include "stdafx.h"
#include <glib.h>

void PopBox(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color)
{
	gcls(hdc, x1, y1, x2, y2, color);
	PopRectangle(hdc, x1, y1, x2, y2);
}







