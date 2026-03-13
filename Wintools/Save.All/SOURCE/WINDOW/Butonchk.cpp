
//------------------------------------------------------------------------------
//	버턴을 흉내낸다.
//------------------------------------------------------------------------------
#include "stdafx.h"
#include <tools.h>
#include <glib.h>

static char bCaptureFlag = OFF;
static char bMouseInFlag = OFF;
static int  nSaveX1, nSaveY1, nSaveX2, nSaveY2;
static int  nSaveID;

//------------------------------------------------------------------------------
//	return 값이 1이면 다른버턴을 검사하지 않고 윈도우 proc 을 벗어나도록 한다.
//------------------------------------------------------------------------------

int ButtonCheckDown2(HWND hwnd, LPARAM lParam, int x1, int y1, int x2, int y2, int id)
{
	int mx, my;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	if(mx >= x1 && my >= y1 && mx <= x2 && my <= y2) {
		RECT rect;
		HDC hdc;
		rect.left = x1+1;
		rect.top  = y1+1;
		rect.right = x2-1;
		rect.bottom = y2-1;
		ScrollWindow(hwnd, 1, 1, &rect, &rect);
		hdc = GetDC(hwnd);
		PushRectangle2(hdc, x1, y1, x2, y2);
		ReleaseDC(hwnd, hdc);
		bCaptureFlag = ON;
		bMouseInFlag = ON;		// 마우스가 버턴속에 있다.
		SetCapture(hwnd);
		nSaveX1 = x1;
		nSaveY1 = y1;
		nSaveX2 = x2;
		nSaveY2 = y2;
		nSaveID = id;
		return 1;
	}
	return 0;
}

//------------------------------------------------------------------------------------------
//	return 1 = 버튼 체크가 현재 진행중에 있다.
//------------------------------------------------------------------------------------------

int ButtonCheckMove2(HWND hwnd, LPARAM lParam)
{
	if(bCaptureFlag == OFF)	return 0;	// 마우스가 눌러져 있지 않다.

	int mx, my;
	int x1, y1, x2, y2;
	char in;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	x1 = nSaveX1;
	y1 = nSaveY1;
	x2 = nSaveX2;
	y2 = nSaveY2;

	if(mx >= x1 && my >= y1 && mx <= x2 && my <= y2) 	in = ON;
	else																in = OFF;

	if(bMouseInFlag == in)	return 1;			// 마우스 움직임에 변화를 줄필요가 없다.

	RECT rect;
	HDC hdc;
	rect.left = x1+1;
	rect.top = y1+1;
	rect.right = x2-1;
	rect.bottom = y2-1;
	hdc = GetDC(hwnd);
	if(bMouseInFlag == OFF && in == ON) {	// 마우스가 버턴 바깥에서 안으로 들어왔다.
		ScrollWindow(hwnd, 1, 1, &rect, &rect);
		PushRectangle2(hdc, x1, y1, x2, y2);
	}
	else {			// 마우스가 버턴 안에서 바깥으로 나갔다.
		ScrollWindow(hwnd, -1, -1, &rect, &rect);
		PopRectangle2(hdc, x1, y1, x2, y2);
	}
	ReleaseDC(hwnd, hdc);

	bMouseInFlag = in;

	return 1;
}

//------------------------------------------------------------------------------
//	return - (-1) 체크된 버턴이 없다.
// return - else (체크된 ID)
//------------------------------------------------------------------------------

int ButtonCheckUp2(HWND hwnd)
{
	if(bCaptureFlag == OFF)	return -1;
	bCaptureFlag = OFF;
	ReleaseCapture();
	if(bMouseInFlag == OFF)	return -1;

	RECT rect;

	rect.left   = nSaveX1;
	rect.top    = nSaveY1;
	rect.right  = nSaveX2+1;
	rect.bottom = nSaveY2+1;
	InvalidateRect(hwnd, &rect, FALSE);
	UpdateWindow(hwnd);
	bCaptureFlag = OFF;
	bMouseInFlag = OFF;

	return nSaveID;
}


