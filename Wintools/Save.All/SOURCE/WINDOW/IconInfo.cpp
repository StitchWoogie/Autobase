#include "stdafx.h"
#include <tools.h>
#include <glib.h>

static HWND hwndIconInformation;
long  CALLBACK WndProcIconInformationString( HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam );
static char  *szClassNameIconInformation = "ClassIconInformationString";
static HINSTANCE hInstanceProgramm;
static HWND hwndMainFrame;

#define IDM_CHANGE_STRING 12345

void IconInfoSetParentHWND(HWND hwnd)
{
	hwndMainFrame = hwnd;
}

//------------------------------------------------------------------------------
//	Icon 도움말 윈도우를 등록한다.
//------------------------------------------------------------------------------

void IconInfoStringRegisterClass(HINSTANCE hInstance)
{
	WNDCLASS wndclass ;

	hInstanceProgramm = hInstance;

	// Register the frame window class
	wndclass.style         = CS_HREDRAW | CS_VREDRAW;
	wndclass.lpfnWndProc   = WndProcIconInformationString;
	wndclass.cbClsExtra    = 0 ;
	wndclass.cbWndExtra    = 0 ;
	wndclass.hInstance     = hInstance ;
	wndclass.hIcon         = NULL;
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW);
	wndclass.hbrBackground = NULL;//COLOR_APPWORKSPACE + 1 ;
	wndclass.lpszMenuName  = NULL;
	wndclass.lpszClassName = szClassNameIconInformation;

	RegisterClass (&wndclass);
}

static char *sInfoString = NULL;

static void CreateIconInfoStringBox()
{
	hwndIconInformation = CreateWindow(szClassNameIconInformation,  "",
				  WS_POPUP | WS_BORDER,
				  0, 0, 0, 0,
				  hwndMainFrame, NULL, hInstanceProgramm, NULL);

	ShowWindow( hwndIconInformation, SW_SHOWNA);
	UpdateWindow( hwndIconInformation );
}

static void DeleteInfoString()
{
	if(sInfoString) {
		delete sInfoString;
		sInfoString = NULL;
	}
}

void IconInfoStringClose()
{
	if(hwndIconInformation) {
		DestroyWindow(hwndIconInformation);
	}
}

static void InfoStringNew(char *text)
{
	if(text == NULL)	return;
	if(text[0] == 0)	return;

	DeleteInfoString();

	sInfoString = new char[strlen(text)+1];
	if(sInfoString) {
		strcpy(sInfoString, text);
	}
}

static int mxIconInfo, myIconInfo;
static HWND hwndCurrent;

int IconInfoStringCheck(HWND hwnd, int x1, int y1, int x2, int y2, LPARAM lParam, char *text) 
{
	hwndCurrent = hwnd;

	int mx, my;

	mx = LOWORD(lParam);
	my = HIWORD(lParam);

	if(mx < x1 || mx > x2 || my < y1 || my > y2)	return 0;
	
	if(text == NULL || text[0] == 0) {
		IconInfoStringClose();
		return 0;
	}
	
	if(sInfoString != NULL && strcmp(text, sInfoString) == 0) {
		if(hwndIconInformation)	return 1;

		InfoStringNew(text);
		mxIconInfo = mx;
		myIconInfo = my;
		CreateIconInfoStringBox();			
		return 1;
	}
	else {
		InfoStringNew(text);

		if(hwndIconInformation) {
			mxIconInfo = mx;
			myIconInfo = my;
			SendMessage(hwndIconInformation, WM_COMMAND, IDM_CHANGE_STRING, 0L);
			InvalidateRect(hwndIconInformation, NULL, TRUE);
		}
		else {
			mxIconInfo = mx;
			myIconInfo = my;
			CreateIconInfoStringBox();			
		}

		return 1;
	}
}

static void MoveStringWindow(HWND hwnd)
{
	HDC	hdc;
	SIZE size;
	POINT p;
	
	hdc = GetDC(hwnd);
#if	defined (_WIN32)
	GetTextExtentPoint32(hdc, sInfoString, strlen(sInfoString), &size);
#else
	TEXTMETRIC tm;
	GetTextMetrics(hdc, &tm);
	size.cx = (int)GetTextExtent(hdc, sInfoString, strlen(sInfoString));
	size.cy = tm.tmExternalLeading+tm.tmHeight;
#endif
	ReleaseDC(hwnd, hdc);

	p.x = mxIconInfo;
	p.y = myIconInfo;
	ClientToScreen(hwndCurrent, &p);

	MoveWindow(hwnd, p.x+5, p.y+10, size.cx+2, size.cy+2, TRUE);
}

long  CALLBACK WndProcIconInformationString( HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam )
{
	HDC			hdc;
	PAINTSTRUCT	ps;
	POINT p;

	switch( message )    {
		case WM_CREATE:
			SetTimer(hwnd, 1, 0, NULL);
			MoveStringWindow(hwnd);
			return 0;
		case WM_TIMER:
			GetCursorPos(&p);
			if(WindowFromPoint(p) != hwndCurrent) {
				DestroyWindow(hwnd);		
			}
			return 0;
		case WM_COMMAND:
			switch(wParam) {
				case IDM_CHANGE_STRING:
					MoveStringWindow(hwnd);
					break;
			}
			return 0;
		case WM_PAINT:
			hdc = BeginPaint( hwnd, &ps);
			TextOut(hdc, 0, 0, sInfoString, strlen(sInfoString));
			EndPaint(hwnd, &ps);
			return 0;
		case WM_DESTROY:
			KillTimer(hwnd, 1);
			hwndIconInformation = NULL;
			DeleteInfoString();
			return 0;
	}
	return DefWindowProc( hwnd, message, wParam, lParam );
}
