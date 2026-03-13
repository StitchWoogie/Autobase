// english O.K
//------------------------------------------------------------------------------
//	도구 박스를 만들고 없애는 부분.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <tools.h>
#include <glib.h>

static char bLeftFlag = OFF;
static int  nGabX, nGabY;

static void LButtonDown(HWND hwnd, LPARAM lParam)
{
	int mx, my;
	RECT r;

	GetClientRect(hwnd, &r);

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	//if(mx >= 0 && my >= 0 && mx <= r.right &&  my <= 15) {
	bLeftFlag = ON;
	SetCapture(hwnd);
	nGabX = mx;
	nGabY = my;
	//}
}

static LRESULT MouseMove(HWND hwnd, LPARAM lParam)
{
	if(bLeftFlag == OFF)	return 0L;	// 마우스가 눌러져 있지 않다.

	short mx, my;
	POINT pt;
	RECT rect;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	pt.x = mx-nGabX;
	pt.y = my-nGabY;
	ClientToScreen(hwnd, &pt);
	GetClientRect(hwnd, &rect);

	MoveWindow(hwnd, pt.x, pt.y, rect.right, rect.bottom, TRUE);

	return 1L;
}

static void LButtonUp(HWND /*hwnd*/)
{
	if(bLeftFlag == OFF)	return;
	bLeftFlag = OFF;
	ReleaseCapture();
}

void FloatBoxPaint(HWND hwnd, HDC hdc)
{
	RECT rect;
	HPEN hPen, hOldPen;

	GetClientRect(hwnd, &rect);

	hPen = CreatePen(PS_SOLID, 1, DARK_COLOR);
	hOldPen = (HPEN)SelectObject(hdc, hPen);
	Rectangle(hdc, rect.left, rect.top, rect.right, rect.bottom);
	SelectObject(hdc, hOldPen);
	DeleteObject(hPen);

	PopBox2(hdc, rect.left+1, rect.top+1, rect.right-2, rect.bottom-2, WHITE_GRAY_COLOR);
	PushRectangle2(hdc, rect.left+3, rect.top+15, rect.right-4, rect.bottom-4);

	PopBox2(hdc, rect.left+3, rect.top+3, rect.left+20, rect.top+13, WHITE_GRAY_COLOR);
	gcls(hdc, rect.left+6, rect.top+7, rect.left+17, rect.top+9, DARK_COLOR);
	PushBox2(hdc, rect.left+23, rect.top+3, rect.right-4, rect.top+13, RGB(0, 0x80, 0x80));
}

void FloatBoxLButtonDown(HWND hwnd, LPARAM lParam, HMENU hMenu)
{
	POINT p;
	int mx, my;
	mx = LOWORD(lParam), my = HIWORD(lParam);

	if(hMenu != NULL) {
		if(mx >= 3 && my >= 3 &&
			mx <= 20 && my <= 13) {
			p.x = 0;
			p.y = 13;
			ClientToScreen(hwnd, &p);
			TrackPopupMenu(hMenu, TPM_LEFTALIGN, p.x, p.y, 0, hwnd, NULL);
			return;
		}
	}

	LButtonDown(hwnd, lParam);
}

LRESULT FloatBoxMouseMove(HWND hwnd, LPARAM lParam)
{
	return MouseMove(hwnd, lParam);
}

void FloatBoxLButtonUp(HWND hwnd)
{
	LButtonUp(hwnd);
}




