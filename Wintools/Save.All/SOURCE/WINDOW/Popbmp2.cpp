// english O.K
#include "stdafx.h"
#include <glib.h>

void DrawBitmapTile(HDC hdc, int x1, int y1, int x2, int y2, HBITMAP hBitmap)
{
	HDC hMemoryDC;
	HBITMAP hOldBitmap;
	int x, y;
	int put_x, put_y;
	int width, height;
	BITMAP bm;

	if(hBitmap == NULL)	return;

	if(GetObject(hBitmap, sizeof(BITMAP), &bm) == 0)	return;	// fail

	width = bm.bmWidth;
	height = bm.bmHeight;

	hMemoryDC = CreateCompatibleDC(hdc);
	hOldBitmap = (HBITMAP)SelectObject(hMemoryDC, hBitmap);

	for(y = y1; y <= y2; y += height) {

		if(y+height > y2+1)		put_y = (y2-y)+1;
		else                    put_y = height;

		for(x = x1; x <= x2; x += width) {

			if(x+width > x2+1)	put_x = (x2-x)+1;
			else                 put_x = width;

			BitBlt(hdc, x, y, put_x, put_y, hMemoryDC, 0, 0, SRCCOPY);
		}
	}

	SelectObject(hMemoryDC, hOldBitmap);
   DeleteDC(hMemoryDC);
}

void PopBoxBitmap2(HDC hdc, int x1, int y1, int x2, int y2, HBITMAP hBitmap)
{
	DrawBitmapTile(hdc, x1+1, y1+1, x2-1, y2-1, hBitmap);
	PopRectangle2(hdc, x1, y1, x2, y2);
}

void PopBoxBitmap(HDC hdc, int x1, int y1, int x2, int y2, HBITMAP hBitmap)
{
	DrawBitmapTile(hdc, x1+3, y1+3, x2-3, y2-3, hBitmap);
	PopRectangle(hdc, x1, y1, x2, y2);
}




