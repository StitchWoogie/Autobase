// ¿µ¹® O.K
#include "stdafx.h"
#include <glib.h>
void PushRectangle(HDC hdc, int x1, int y1, int x2, int y2)
{
	HPEN hWhitePen, hOldPen, hGrayPen, hDarkPen;
	POINT p;

	hWhitePen = CreatePen(PS_SOLID, 1, WHITE_COLOR);
	hGrayPen  = CreatePen(PS_SOLID, 1, DARK_GRAY_COLOR);
	hDarkPen  = CreatePen(PS_SOLID, 1, DARK_COLOR);

	hOldPen = (HPEN)SelectObject(hdc, hGrayPen);
	MoveToEx(hdc, x1,y2, &p);
	LineTo(hdc, x1,y1);
	LineTo(hdc, x2,y1);

	SelectObject(hdc, hWhitePen);
	LineTo(hdc, x2,y2);
	LineTo(hdc, x1,y2);

	SelectObject(hdc, hGrayPen);
	MoveToEx(hdc, x1+1,y2-1, &p);
	LineTo(hdc, x1+1,y1+1);
	LineTo(hdc, x2-1,y1+1);

	SelectObject(hdc, hWhitePen);
	LineTo(hdc, x2-1,y2-1);
	LineTo(hdc, x1+1,y2-1);

	SelectObject(hdc, hDarkPen);
	MoveToEx(hdc, x1+2, y1+2, &p);
	LineTo(hdc, x2-2, y1+2);
	LineTo(hdc, x2-2, y2-2);
	LineTo(hdc, x1+2, y2-2);
	LineTo(hdc, x1+2, y1+2);

//	gsetcolor(DARK_COLOR);
//	grectangle(x1, y1, x2, y2);

	SelectObject(hdc, hOldPen);
	DeleteObject(hWhitePen);
	DeleteObject(hGrayPen);
	DeleteObject(hDarkPen);
}

