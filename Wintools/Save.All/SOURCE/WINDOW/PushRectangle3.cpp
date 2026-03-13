#include "stdafx.h"
#include <glib.h>

void PushRectangle3(HDC hdc, int x1, int y1, int x2, int y2)
{
	HPEN hWhitePen, hOldPen, hWhiteGrayPen, hDarkGrayPen, hDarkPen;
	POINT p;

	hWhitePen = CreatePen(PS_SOLID, 1, WHITE_COLOR);
	hDarkGrayPen  = CreatePen(PS_SOLID, 1, DARK_GRAY_COLOR);
	hWhiteGrayPen  = CreatePen(PS_SOLID, 1, WHITE_GRAY_COLOR);
	hDarkPen  = CreatePen(PS_SOLID, 1, DARK_COLOR);

	hOldPen = (HPEN)SelectObject(hdc, hDarkGrayPen);
	MoveToEx(hdc, x1, y2, &p);
	LineTo(hdc, x1, y1);
	LineTo(hdc, x2, y1);

	SelectObject(hdc, hWhitePen);
	LineTo(hdc, x2, y2);
	LineTo(hdc, x1, y2);

	SelectObject(hdc, hDarkPen);
	MoveToEx(hdc, x1+1, y2-1, &p);
	LineTo(hdc, x1+1, y1+1);
	LineTo(hdc, x2-1, y1+1);

	SelectObject(hdc, hWhiteGrayPen);
	LineTo(hdc, x2-1, y2-1);
	LineTo(hdc, x1+1, y2-1);

	SelectObject(hdc, hOldPen);
	DeleteObject(hWhitePen);
	DeleteObject(hWhiteGrayPen);
	DeleteObject(hDarkGrayPen);
	DeleteObject(hDarkPen);
}


