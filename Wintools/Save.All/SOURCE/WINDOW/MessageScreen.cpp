// TAG size O.K
#include "stdafx.h"

#include <glib.h>
#include <dataswap.h>

#include "..\catlib.src\totalcfg.h" //20241010 PSU

static HWND hwndMainFrame;

static char   szMessageClass  [] = "MessageScreenWindow";

static HWND hwndMessageBox;
static char sMessageString[200] = "message";
static char sConfigName[80] = "Default";
static int  nMessageWindowShowSec = 0;			// 메세지 윈도우가 화면에 떠있은 시간
static char cMsgBoxOldSec;						// 바로이전의 초
static int  nAlarmScreenTime = 0;				// 5초동안 화면에 떠있음.
static POINT pMessageBox = { 100, 300 };

static HINSTANCE hInst;
static int	cxChar, cyChar;

static int  nAlarmScreenShowMethod = SW_SHOWNORMAL;
static char bAlarmScreenTopMost    = 0;

//static void MessageScreenLocal(const char *title, const char *string)
//{
//	struct time t;
//	int line = 1;
//	int i;
//
//	strncpy(sMessageString, string, 199);
//	sMessageString[199] = 0;
//
//	for(i = 0; i < (int)strlen(sMessageString); i++) {
//		if(sMessageString[i] == '\n')	line++;
//	}
//
//	if(hwndMessageBox == NULL) {
//		int maxx = GetSystemMetrics(SM_CXSCREEN);
//		int maxy = GetSystemMetrics(SM_CYSCREEN);
//		int captiony = GetSystemMetrics(SM_CYCAPTION);
//		int x, y;
//		TEXTMETRIC tm;
//		HDC hdc;
//
//		hdc = GetDC(hwndMainFrame);
//		GetTextMetrics(hdc, &tm);
//		ReleaseDC(hwndMainFrame, hdc);
//		cxChar = tm.tmAveCharWidth;
//		cyChar = tm.tmHeight+tm.tmExternalLeading;
//
//		x = pMessageBox.x;
//		y = pMessageBox.y;
//
//		if(x < 0)		x = 0;
//		if(y < 0)		y = 0;
//		if(x > maxx)	x = maxx-100;
//		if(y > maxy)	y = maxy-100;
//
//		hwndMessageBox = CreateWindow(szMessageClass,
//												" ",
//												WS_SYSMENU,
//												x, y,
//												630, captiony+7+cyChar*line,
//												hwndMainFrame, NULL, hInst, NULL);
//
//		if(bAlarmScreenTopMost)
//			SetWindowPos(hwndMessageBox, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
//		
//		ShowWindow(hwndMessageBox, nAlarmScreenShowMethod);
//	}
//	else {
//
//	}
//
//	SetWindowText(hwndMessageBox, title);
//
//	InvalidateRect(hwndMessageBox, NULL, TRUE);
//	UpdateWindow( hwndMessageBox );
//
//	gettime(&t);
//	nMessageWindowShowSec = 0;	// 메세지 윈도우가 화면에 떠있은 시간
//	cMsgBoxOldSec = t.ti_sec;
//}

//20241010 PSU
static void MessageScreenLocal(const char *title, const char *string)
{
    //InitializeStartMonitorIndex();

    struct time t;
    int line = 1;
    int i;

    strncpy(sMessageString, string, 199);
    sMessageString[199] = 0;

    for(i = 0; i < (int)strlen(sMessageString); i++) {
        if(sMessageString[i] == '\n')   line++;
    }

    TEXTMETRIC tm;
    HDC hdc = GetDC(hwndMainFrame);
    GetTextMetrics(hdc, &tm);
    ReleaseDC(hwndMainFrame, hdc);
    cxChar = tm.tmAveCharWidth;
    cyChar = tm.tmHeight + tm.tmExternalLeading;

    // 주프레임윈도우가있는모니터정보가져오기
    HMONITOR hMonitor = MonitorFromWindow(hwndMainFrame, MONITOR_DEFAULTTONEAREST);
    MONITORINFO monitorInfo = { sizeof(MONITORINFO) };
    GetMonitorInfo(hMonitor, &monitorInfo);

    // 메시지박스의크기계산
    int captiony = GetSystemMetrics(SM_CYCAPTION);
    int width = 630;
    int height = captiony + 7 + cyChar * line;

    // 메시지박스의위치계산(모니터의작업영역내에서)
    int margin = 120; // 화면가장자리로부터의여백
    int x = monitorInfo.rcWork.left + margin;
    int y = monitorInfo.rcWork.top + margin;

    if (hwndMessageBox == NULL) {
        hwndMessageBox = CreateWindow(szMessageClass,
                                      " ",
                                      WS_SYSMENU,
                                      x, y,
                                      width, height,
                                      hwndMainFrame, NULL, hInst, NULL);

        if(bAlarmScreenTopMost)
            SetWindowPos(hwndMessageBox, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        
        ShowWindow(hwndMessageBox, nAlarmScreenShowMethod);
    }
    else {
        // 기존메시지박스가있다면, 위치를업데이트합니다.
        //SetWindowPos(hwndMessageBox, NULL, x, y, width, height, SWP_NOZORDER);
    }

    SetWindowText(hwndMessageBox, title);

    InvalidateRect(hwndMessageBox, NULL, TRUE);
    UpdateWindow(hwndMessageBox);

    gettime(&t);
    nMessageWindowShowSec = 0;  // 메세지윈도우가화면에떠있은시간
    cMsgBoxOldSec = t.ti_sec;
}


void MessageScreen(const char *title, const char *string, ...)
{
	va_list ap;
	StackChar imsi(1000);

	if(imsi.data != NULL) {
		va_start(ap, string);
		vsprintf(imsi.data, (const char*)string, ap);
		va_end(ap);
		MessageScreenLocal(title, imsi.data);
	}
}

void MessageScreenHide()
{
	DestroyWindow(hwndMessageBox);
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
				MessageScreenHide();
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
	StackChar buf(1000);
	int count;
	int i;
	int line = 0;
	
	GetClientRect(hwnd, &rect);
	hdc = BeginPaint(hwnd, &ps);
	SetTextColor(hdc, RGB(0, 0, 0));
	SetBkColor(hdc, RGB(255, 255, 255));

	count = 0;
	for(i = 0; i < (int)strlen(sMessageString); i++) {
		if(sMessageString[i] == '\n') {	
			buf.data[count] = 0;
			rect.top = line*cyChar;
			rect.bottom = rect.top+cyChar;
			DrawText(hdc, buf.data, strlen(buf.data), &rect, DT_CENTER|DT_VCENTER|DT_SINGLELINE);
			count = 0;
			line++;
		}
		else {
			if(count < 990) {
				buf.data[count] = sMessageString[i];
				count++;
			}
		}
	}

	if(count > 0) {		// display last line
		buf.data[count] = 0;
		rect.top = line*cyChar;
		rect.bottom = rect.top+cyChar;
		DrawText(hdc, buf.data, strlen(buf.data), &rect, DT_CENTER|DT_VCENTER|DT_SINGLELINE);
	}

	EndPaint(hwnd, &ps);
}

long FAR PASCAL EXPORT WndProcMessage (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	RECT rect;

	switch (message)
	{
		case WM_CREATE:
			SetTimer(hwnd, 1, 1000, NULL);
			return 0;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_MOVE:
			GetWindowRect(hwnd, &rect);
			pMessageBox.x = rect.left;
			pMessageBox.y = rect.top;
			return 0;
		case WM_TIMER:
			WmTimer();
			return 0;
		case WM_DESTROY :
			KillTimer(hwnd, 1);
			hwndMessageBox = NULL;
			return 0;
	}
	return DefWindowProc( hwnd, message, wParam, lParam );
}

//------------------------------------------------------------------------------
//	메세지 윈도우를 등록한다.
//------------------------------------------------------------------------------

void MessageScreenRegisterClass(HWND hwnd, HINSTANCE hInstance)
{
	WNDCLASS wndclass;

	// Register the Message window class
	wndclass.style         = CS_HREDRAW | CS_VREDRAW;
	wndclass.lpfnWndProc   = WndProcMessage ;
	wndclass.cbClsExtra    = 0;
	wndclass.cbWndExtra    = 0;
	wndclass.hInstance     = hInstance;
	wndclass.hIcon         = LoadIcon (NULL, IDI_APPLICATION);
	wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW);
	wndclass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
	wndclass.lpszMenuName  = NULL;
	wndclass.lpszClassName = szMessageClass;

	RegisterClass (&wndclass);

	hInst = hInstance;
	hwndMainFrame = hwnd;
	//strcpy(sConfigName, config_name);
}

void MessageScreenSetLifeTime(int time)
{
	nAlarmScreenTime = time;
}

void MessageScreenSetParentHWND(HWND hwnd)
{
	hwndMainFrame = hwnd;
}

void MessageScreenSetShowMethod(int method, char bTopMost)
{
	nAlarmScreenShowMethod = method;
	bAlarmScreenTopMost = bTopMost;
}

/*
class {
		HANDLE hThread;
		DWORD dwID;
	public:
		ClassMessageScreen();
		~ClassMessageScreen();
} ClassMessageScreen;

static DWORD WINAPI PortThread_0(LPVOID)
{	
	PortThread_Common(0);
	return 0;	
}

ClassMessageScreen::ClassMessageScreen()
{
	hThread = CreateThread(NULL, 0, PortThread_0, NULL, 0, &dwID);
}

ClassMessageScreen::ClassMessageScreen()
{

}
*/

