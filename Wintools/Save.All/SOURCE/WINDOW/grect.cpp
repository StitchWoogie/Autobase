#include "stdafx.h"
#include <glib.h>

void grect(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color)
{
	HPEN hPen, hOldPen;

	hPen = CreatePen(PS_SOLID, 1, color);

	hOldPen = (HPEN)SelectObject(hdc, hPen);
	MoveToEx(hdc, x1, y1, NULL);
	LineTo(hdc, x2, y1);
	LineTo(hdc, x2, y2);
	LineTo(hdc, x1, y2);
	LineTo(hdc, x1, y1);

	SelectObject(hdc, hOldPen);
	DeleteObject(hPen);
}







