// ¿µ¹® O.K                    
#include "stdafx.h"
#include <glib.h>

void PushBox(HDC hdc, int x1, int y1, int x2, int y2, COLORREF color)
{
	gcls(hdc, x1, y1, x2, y2, color);
	PushRectangle(hdc, x1, y1, x2, y2);
}

