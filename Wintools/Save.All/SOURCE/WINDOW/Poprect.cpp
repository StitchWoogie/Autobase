#include "stdafx.h"
#include <glib.h>

void PopRectangle(HDC hdc, int x1, int y1, int x2, int y2)
{
	HPEN hWhitePen, hOldPen, hGrayPen, hDarkPen;
	POINT old;

	hWhitePen = CreatePen(PS_SOLID, 1, WHITE_COLOR);
	hGrayPen  = CreatePen(PS_SOLID, 1, DARK_GRAY_COLOR);
	hDarkPen  = CreatePen(PS_SOLID, 1, DARK_COLOR);

	hOldPen = (HPEN)SelectObject(hdc, hWhitePen);
	MoveToEx(hdc, x1+1,y2-1, &old);
	LineTo(hdc, x1+1,y1+1);
	LineTo(hdc, x2-1,y1+1);

	SelectObject(hdc, hGrayPen);
	LineTo(hdc, x2-1,y2-1);
	LineTo(hdc, x1+1,y2-1);

	SelectObject(hdc, hWhitePen);
	MoveToEx(hdc, x1+2,y2-2, &old);
	LineTo(hdc, x1+2,y1+2);
	LineTo(hdc, x2-2,y1+2);

	SelectObject(hdc, hGrayPen);
	LineTo(hdc, x2-2,y2-2);
	LineTo(hdc, x1+2,y2-2);

	SelectObject(hdc, hDarkPen);
	MoveToEx(hdc, x1, y1, &old);
	LineTo(hdc, x2, y1);
	LineTo(hdc, x2, y2);
	LineTo(hdc, x1, y2);
	LineTo(hdc, x1, y1);

//	gsetcolor(DARK_COLOR);
//	grectangle(x1, y1, x2, y2);

	SelectObject(hdc, hOldPen);
	DeleteObject(hWhitePen);
	DeleteObject(hGrayPen);
	DeleteObject(hDarkPen);
}


