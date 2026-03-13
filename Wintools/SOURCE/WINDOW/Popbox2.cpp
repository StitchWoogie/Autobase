#include "stdafx.h"
#include <glib.h>

void PopBox2(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color)
{
	gcls(hdc, x1+1, y1+1, x2-1, y2-1, color);
	PopRectangle2(hdc, x1, y1, x2, y2);
}

void PopBox2(HDC hdc, RECT *rect, COLORREF color)
{
	gcls(hdc, rect, color);
	PopRectangle2(hdc, rect);
}


