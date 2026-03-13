#include "stdafx.h"
#include <glib.h>

void gcls(HDC hdc, RECT *rect, COLORREF color)
{
	HBRUSH hBrush;

	hBrush = CreateSolidBrush(color);
	FillRect(hdc, rect, hBrush);
	DeleteObject(hBrush);
}

void gcls(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color)
{
	RECT rect;

	rect.left = x1;
	rect.top  = y1;
	rect.right = x2+1;
	rect.bottom = y2+1;
	gcls(hdc, &rect, color);
}

