#include "stdafx.h"
#include <glib.h>

void PopRectangle2(HDC hdc, int x1, int y1, int x2, int y2)
{
	HPEN hWhitePen, hOldPen, hGrayPen;
	POINT p;

	hWhitePen = CreatePen(PS_SOLID, 1, WHITE_COLOR);
	hGrayPen  = CreatePen(PS_SOLID, 1, DARK_GRAY_COLOR);

	hOldPen = (HPEN)SelectObject(hdc, hWhitePen);
	MoveToEx(hdc, x1, y2, &p);
	LineTo(hdc, x1, y1);
	LineTo(hdc, x2, y1);

	SelectObject(hdc, hGrayPen);
	LineTo(hdc, x2, y2);
	LineTo(hdc, x1, y2);

	SelectObject(hdc, hOldPen);
	DeleteObject(hWhitePen);
	DeleteObject(hGrayPen);
}

void PopRectangle2(HDC hdc, RECT *rect)
{
	PopRectangle2(hdc, rect->left, rect->top, rect->right, rect->bottom);
}






