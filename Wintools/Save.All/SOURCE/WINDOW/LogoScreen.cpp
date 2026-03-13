// TAG size O.K
// english O.K
#include "stdafx.h"
#include <io.h>

#include <glib.h>
#include <dataswap.h>
#include <pic_tool.h>

static HWND hwndMainFrame;

static char   szLogoClass  [] = "LogoScreenWindow";

static HWND hwndLogoBox;
static int  nMessageWindowShowSec = 0;			// 메세지 윈도우가 화면에 떠있은 시간
static char cMsgBoxOldSec;						// 바로이전의 초
static int  nAlarmScreenTime = 5;				// 5초동안 화면에 떠있음.

static HINSTANCE hInst;
static HGLOBAL hLogoDIB = NULL;

void LogoScreen(const char *filename)
{
	struct time t;
	gettime(&t);

	if(hwndLogoBox == NULL) {
		if(access(filename, 0) != 0)	return;	// logo file not found

		hLogoDIB = PictureToolLoadToBufDIB(hwndMainFrame, (char*)filename);
		if(hLogoDIB == NULL)	return;

		BYTE *lpDib = (BYTE*)GlobalLock(hLogoDIB);
		int width   = GetDibWidth (lpDib);
		int height  = GetDibHeight(lpDib);
		GlobalUnlock(hLogoDIB);

		//BYTE huge * GetDibBitsAddr (BYTE huge * lpDib);

		int maxx = GetSystemMetrics(SM_CXSCREEN);
		int maxy = GetSystemMetrics(SM_CYSCREEN);
		int x, y;

		x = maxx/2-width/2;
		y = maxy/2-height/2;

		if(x < 0)		x = 0;
		if(y < 0)		y = 0;

		hwndLogoBox = CreateWindow(szLogoClass,
												" ",
												WS_POPUP|WS_BORDER,
												x, y,
												width, height,
												hwndMainFrame, NULL, hInst, NULL);

		SetWindowPos(hwndLogoBox, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
		
		ShowWindow(hwndLogoBox, SW_SHOWNA);
	}
	else {

	}

	InvalidateRect(hwndLogoBox, NULL, TRUE);
	UpdateWindow( hwndLogoBox );

	nMessageWindowShowSec = 0;	// 메세지 윈도우가 화면에 떠있은 시간
	cMsgBoxOldSec = t.ti_sec;
}

void LogoScreenHide()
{
	DestroyWindow(hwndLogoBox);
}

static void WmTimer()
{
	struct time t;

	if(nAlarmScreenTime == 0)	return;	// 보여주는 시간이 0일때는 사용자가 없애기 전까지는 계속 보여준다.
	{
		gettime(&t);
		if(t.ti_sec != cMsgBoxOldSec) {
			
			if(t.ti_sec > cMsgBoxOldSec) {
				nMessageWindowShowSec += (t.ti_sec-cMsgBoxOldSec);
			}
			else {
				nMessageWindowShowSec += (t.ti_sec+60-cMsgBoxOldSec);
			}
			cMsgBoxOldSec = t.ti_sec;
			
			if(nMessageWindowShowSec >= nAlarmScreenTime) {
				LogoScreenHide();
				return;
			}
		}
	}
}

static void WmPaint(HWND hwnd)
{
	HDC hdc;
	PAINTSTRUCT ps;
	RECT rect;
	
	GetClientRect(hwnd, &rect);
	hdc = BeginPaint(hwnd, &ps);

	BYTE *lpDib = (BYTE*)GlobalLock(hLogoDIB);
	int width   = GetDibWidth (lpDib);
	int height  = GetDibHeight(lpDib);
	BYTE *lpBits = GetDibBitsAddr (lpDib);
	SetDIBitsToDevice(hdc, 0, 0, width, height, 0, 0, 0, height, lpBits, (BITMAPINFO*)lpDib, DIB_RGB_COLORS);
	GlobalUnlock(hLogoDIB);

	EndPaint(hwnd, &ps);
}

long FAR PASCAL EXPORT WndProcLogo (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
		case WM_CREATE:
			SetTimer(hwnd, 1, 1000, NULL);
			return 0;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_TIMER:
			WmTimer();
			return 0;
		case WM_DESTROY :
			KillTimer(hwnd, 1);
			hwndLogoBox = NULL;
			if(hLogoDIB) {
				GlobalFree(hLogoDIB);
				hLogoDIB = NULL;
			}
			return 0;
	}
	return DefWindowProc( hwnd, message, wParam, lParam );
}

//------------------------------------------------------------------------------
//	메세지 윈도우를 등록한다.
//------------------------------------------------------------------------------

void LogoScreenRegisterClass(HWND hwnd, HINSTANCE hInstance)
{
	WNDCLASS wndclass;

	// Register the Message window class
	wndclass.style         = CS_HREDRAW | CS_VREDRAW;
	wndclass.lpfnWndProc   = WndProcLogo;
	wndclass.cbClsExtra    = 0;
	wndclass.cbWndExtra    = 0;
	wndclass.hInstance     = hInstance;
	wndclass.hIcon         = LoadIcon (NULL, IDI_APPLICATION);
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW);
	wndclass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
	wndclass.lpszMenuName  = NULL;
	wndclass.lpszClassName = szLogoClass;

	RegisterClass (&wndclass);

	hInst = hInstance;
	hwndMainFrame = hwnd;
	//strcpy(sConfigName, config_name);
}

//void LogoScreenSetLifeTime(int time)
//{
//	nAlarmScreenTime = time;
//}



