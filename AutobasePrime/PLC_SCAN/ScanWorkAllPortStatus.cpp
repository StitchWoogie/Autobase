// english O.K
#include "stdafx.h"
#include <stdio.h>
#include <string.h>
 
#include <tools.h>
#include <glib.h>
#include <menubutn.h>

#include "plc_scan.h"
#include "resource.h"
#include "device\commmain.h"

#include "..\catlib.src\totalcfg.h"

#define	CAT_COLOR_TEXT		RGB(255, 255, 255)
#define	CAT_COLOR_BACK		RGB(0, 0, 0x80)

#define	IDC_SCANBUF_BUTTON_VIEWMAIN				7000
#define	IDC_SCANBUF_BUTTON_CLOSE				7001
#define IDC_SCANBUF_BUTTON_CLEAR_ERROR			7002
#define IDC_SCANBUF_BUTTON_HAND_CONNECTION		7003
#define IDC_SCANBUF_BUTTON_HAND_DISCONNECTION	7004

extern	char szViewScanBufPortStatusChildClass [];
extern  HWND hwndScanBufPortStatusChild;

typedef struct {
	COMM_COUNT_STRUCT device[2];
	//struct date dLastCall;
	SYSTEMTIME tLastCall;
	SYSTEMTIME tCountDown;
	int    nModemErrorNo;
} SHOW_COMPARE;

typedef struct {
		int		nScrollVerPos;
		int		nScrollVerHap;
		char	bScrollVer;
		int		ylimit;					// 한 화면에 보일수 있는 라인수
		int		nCursorY;
		//SHOW_COMPARE compare[MAX_PORT];
		SHOW_COMPARE *compare;	// 초기화 시 할당한다. 
} WORK_SCAN_DATA;

static int	cxChar, cyChar;

#define SIZEY	(cyChar*3+12)

static void DrawTextAndCls(HDC hdc, int x, int y, int gabx, int ypos, char *buf)
{
	int x1;
	int y1;
	int x2;
	int y2;

	x1 = x;
	y1 = y+2+(cyChar+3)*ypos;//ypos  y+cyChar+2 : y+2;
	//y1 = ypos ? y+cyChar+5 : y+2;
	x2 = x+cxChar*gabx;
	y2 = y1+cyChar+1;

	gcls(hdc, x1, y1, x2, y2, GetBkColor(hdc));

	RECT r;

	r.left = x1+1;
	r.top  = y1+1;
	r.right = x2;
	r.bottom = y2;

	DrawText(hdc, buf, strlen(buf), &r, DT_LEFT | DT_VCENTER);
}

static void DrawCountCommTry(HDC hdc, WORK_SCAN_DATA *wk, int port_num, int dev_num)
{
	GLOBAL_PORT_STRUCT *port = &portBuf[port_num];

	int x = cxChar*50;
	int y = SIZEY*(port_num-wk->nScrollVerPos);
	char buf[80];

	if(dev_num == port->cDualCurrentActiveDevice)
		SetTextColor(hdc, CAT_COLOR_TEXT);
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	SetBkColor(hdc,   CAT_COLOR_BACK);
	sprintf(buf, "%lu", port->countDevice[dev_num].Total.lCountCommTry);
	DrawTextAndCls(hdc, x, y, 8, dev_num, buf); // 2015-7-18 7->8으로 변경 4천만까지 통신 가능
}

static void DrawCountTimeOut(HDC hdc, WORK_SCAN_DATA *wk, int port_num, int dev_num)
{
	GLOBAL_PORT_STRUCT *port = &portBuf[port_num];

	int x = cxChar*60;
	int y = SIZEY*(port_num-wk->nScrollVerPos);
	char buf[80];

	if(dev_num == port->cDualCurrentActiveDevice)
		SetTextColor(hdc, CAT_COLOR_TEXT);
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	SetBkColor(hdc,   CAT_COLOR_BACK);
	sprintf(buf, "%lu", port->countDevice[dev_num].Total.lCountTimeOut);
	DrawTextAndCls(hdc, x, y, 7, dev_num, buf);
}

static void DrawCountCodeBad(HDC hdc, WORK_SCAN_DATA *wk, int port_num, int dev_num)
{
	GLOBAL_PORT_STRUCT *port = &portBuf[port_num];

	int x = cxChar*70;
	int y = SIZEY*(port_num-wk->nScrollVerPos);
	char buf[80];

	if(dev_num == port->cDualCurrentActiveDevice)
		SetTextColor(hdc, CAT_COLOR_TEXT);
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	SetBkColor(hdc,   CAT_COLOR_BACK);
	sprintf(buf, "%lu", port->countDevice[dev_num].Total.lCountCodeBad);
	DrawTextAndCls(hdc, x, y, 7, dev_num, buf);
}

void PlcDeviceGetLastCallTime(DEVICE_STRUCT *device, SYSTEMTIME *t);

static void DrawLastCallTime(HDC hdc, WORK_SCAN_DATA *wk, int port_num)
{
	GLOBAL_PORT_STRUCT *port = &portBuf[port_num];

	int y = SIZEY*(port_num-wk->nScrollVerPos);
	char buf[80];
	SYSTEMTIME t;

	PlcDeviceGetLastCallTime(&port->local.device, &t);

	SetTextColor(hdc, RGB(0, 255, 255));
	SetBkColor(hdc,   CAT_COLOR_BACK);

//	if(count == 0)	SetTextColor(hdc, CAT_COLOR_TEXT);
//	else			SetTextColor(hdc, RGB(255, 0, 0));

	sprintf(buf, "%02d/%02d/%02d", t.wYear, t.wMonth, t.wDay);
	DrawTextAndCls(hdc, cxChar*48, y, 11, 2, buf);

	sprintf(buf, "%02d:%02d", t.wHour, t.wMinute);
	DrawTextAndCls(hdc, cxChar*60, y, 6, 2, buf);
}

static void DrawCountDown(HDC hdc, WORK_SCAN_DATA *wk, int port_num)
{
	GLOBAL_PORT_STRUCT *port = &portBuf[port_num];

	int y = SIZEY*(port_num-wk->nScrollVerPos);
	char buf[80];

	SYSTEMTIME t;

	PlcDeviceGetCountDown(&port->local.device, &t);

	SetTextColor(hdc, RGB(255, 255, 0));
	SetBkColor(hdc,   CAT_COLOR_BACK);

	sprintf(buf, "%02d:%02d:%02d", t.wHour, t.wMinute, t.wSecond); 
	DrawTextAndCls(hdc, cxChar*67, y, 9, 2, buf);
}

void PlcDeviceGetErrorString(DEVICE_STRUCT *device, char *string);
int  PlcDeviceGetErrorCount(DEVICE_STRUCT *device);

static void DrawCountModemError(HDC hdc, WORK_SCAN_DATA *wk, int port_num)
{
	GLOBAL_PORT_STRUCT *port = &portBuf[port_num];

	int y = SIZEY*(port_num-wk->nScrollVerPos);
	char buf[80];
	int count = PlcDeviceGetErrorCount(&port->local.device);

	SetBkColor(hdc,   CAT_COLOR_BACK);

	if(count == 0)	SetTextColor(hdc, CAT_COLOR_TEXT);
	else			SetTextColor(hdc, RGB(255, 0, 0));

	sprintf(buf, "%d",  count);
	DrawTextAndCls(hdc, cxChar*77, y, 3, 2, buf);

	PlcDeviceGetErrorString(&port->local.device, buf);
	sprintf(buf, "%s", buf);
	DrawTextAndCls(hdc, cxChar*81, y, 30, 2, buf);
}

static void DrawTextAndBox(HDC hdc, int x, int y, int gabx, int ypos, char *buf)
{
	int x1;
	int y1;
	int x2;
	int y2;

	x1 = x;
	y1 = y+2+(cyChar+3)*ypos;//ypos  y+cyChar+2 : y+2;
	x2 = x+cxChar*gabx;
	y2 = y1+cyChar+1;

	PushBox2(hdc, x1, y1, x2, y2, GetBkColor(hdc));

	RECT r;

	r.left = x1+1;
	r.top  = y1+1;
	r.right = x2;
	r.bottom = y2;

	DrawText(hdc, buf, strlen(buf), &r, DT_LEFT | DT_VCENTER);
}

static void WmPaint(HWND hwnd)
{
	PAINTSTRUCT ps;
	HDC hdc;
	int y;
	HGLOBAL     hGlobal;
	WORK_SCAN_DATA		 *wk;
	RECT rect;
	int i;
	char buf[80];
	GLOBAL_PORT_STRUCT *port;

	hGlobal = (HGLOBAL) GetWindowLong(hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	hdc = BeginPaint(hwnd, &ps);
	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
	gcls(hdc, &ps.rcPaint, WHITE_GRAY_COLOR);

	if(nPortHap == 0) {
		GetClientRect(hwnd, &rect);
		SetTextColor(hdc, CAT_COLOR_TEXT);

		strcpy(buf, "no scan file.");
		DrawText(hdc, buf, strlen(buf), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
	}
	else {
		GetClientRect(hwnd, &rect);
		y = 0;

		for(i = wk->nScrollVerPos; i < MAX_PORT && y < rect.bottom; i++, y+=SIZEY) {
			port = &portBuf[i];

			if(wk->nCursorY == i) {
				PopBox2(hdc, 0, y, rect.right-1, y+SIZEY-1, CAT_COLOR_BACK);
			}
			else {
				PopRectangle2(hdc, 0, y, rect.right-1, y+SIZEY-1);
			}

			if(port->bActiveFlag)	SetTextColor(hdc, WHITE_COLOR);
			else					SetTextColor(hdc, DARK_GRAY_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
						
			PushBox2(hdc, 2, y+2, cxChar*4, y+SIZEY-3, CAT_COLOR_BACK);
			sprintf(buf, "%03d", i);
			TextOut(hdc, 3+1, y+SIZEY/2-cyChar/2, buf, strlen(buf));	// 원래 x위치가 3으로 해도 맞아야 하는데 안티에이라이징 때문인지 1칸 오른쪽으로 밀어야 박스가 침범되지 않는다. 2015-7-8

			if(wk->nCursorY == i) {	
				SetTextColor(hdc, WHITE_COLOR);
				SetBkColor(hdc, CAT_COLOR_BACK);
			}
			else {
				SetTextColor(hdc, DARK_COLOR);
				SetBkColor(hdc, WHITE_GRAY_COLOR);
			}

			//SetBkColor(hdc, WHITE_GRAY_COLOR);
			sprintf(buf, "%s",  port->sTitle);
			DrawTextAndBox(hdc, cxChar*5, y, 19, 0, buf);

			sprintf(buf, "%s",  port->sScanProtocol);
			DrawTextAndBox(hdc, cxChar*5, y, 19, 1, buf);

			if(port->local.device.nDeviceStyle == DEVICE_TYPE_MODEM) {
				sprintf(buf, "%s",  port->tel.sTelNumber);
				DrawTextAndBox(hdc, cxChar*5, y, 19, 2, buf);

				sprintf(buf, "%dmin",  port->tel.nConnectCicle);
				DrawTextAndBox(hdc, cxChar*25, y, 8, 2, buf);

				sprintf(buf, "%dsec",  port->tel.nConnectingTime);
				DrawTextAndBox(hdc, cxChar*34, y, 7, 2, buf);
			}

			sprintf(buf, "%s",  port->sScanDevice);
			DrawTextAndCls(hdc, cxChar*25, y, 24, 0, buf);

			sprintf(buf, "%s",  port->sDualDevice);
			DrawTextAndCls(hdc, cxChar*25, y, 24, 1, buf);

			if(port->bActiveFlag == OFF)	continue;
			
			DrawCountCommTry(hdc, wk, i, 0);
			DrawCountTimeOut(hdc, wk, i, 0);
			DrawCountCodeBad(hdc, wk, i, 0);

			DrawCountCommTry(hdc, wk, i, 1);
			DrawCountTimeOut(hdc, wk, i, 1);
			DrawCountCodeBad(hdc, wk, i, 1);

			if(port->local.device.nDeviceStyle == DEVICE_TYPE_MODEM) {
				DrawLastCallTime(hdc, wk, i);
				DrawCountDown(hdc, wk, i);
				DrawCountModemError(hdc, wk, i);
			}
		}
	}
	EndPaint(hwnd, &ps);

	GlobalUnlock (hGlobal);
}

//------------------------------------------------------------------------------
//	PCL WM_SIZE 메세지
//------------------------------------------------------------------------------

static void WmSize(HWND hwnd, LPARAM /*lParam*/)
{
	HGLOBAL     hGlobal ;
	WORK_SCAN_DATA		*wk;
	RECT rect;
	HDC hdc;
	TEXTMETRIC tm;

	hdc = GetDC(hwnd);
	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
	GetTextMetrics(hdc, &tm);
	ReleaseDC(hwnd, hdc);

	cxChar = tm.tmAveCharWidth+1;
	cyChar = tm.tmHeight+tm.tmExternalLeading;
	cxChar = cyChar/2;

	GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	wk->nScrollVerHap = MAX_PORT;
	
	wk->ylimit = rect.bottom/SIZEY;
	if(wk->ylimit <= 0)	wk->ylimit = 1;

	if(wk->nScrollVerHap > wk->ylimit) {
		if(wk->bScrollVer == OFF) {
			ShowScrollBar(hwnd, SB_VERT, TRUE);
			wk->bScrollVer = ON;
		}
		if(wk->nScrollVerPos > wk->nScrollVerHap)
			wk->nScrollVerPos = wk->nScrollVerHap;
		if(wk->nScrollVerPos < 0) {
			bell();
		}
		SetScrollRange(hwnd, SB_VERT, 0, wk->nScrollVerHap-1, FALSE);
		SetScrollPos(hwnd, SB_VERT, wk->nScrollVerPos, TRUE);
	}
	else {
		if(wk->bScrollVer == ON) {
			ShowScrollBar(hwnd, SB_VERT, FALSE);
			wk->bScrollVer = OFF;
		}
		wk->nScrollVerPos = 0;
		wk->nScrollVerHap = 0;
	}
	GlobalUnlock (hGlobal);
}

static void MessageVScroll(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	RECT rect;
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;

	GetClientRect(hwnd, &rect);
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	switch(LOWORD(wParam)) {
	  case SB_LINEUP:
				if(wk->nScrollVerPos <= 0)	break;
				wk->nScrollVerPos --;
				ScrollWindow(hwnd, 0, SIZEY, &rect, &rect);
				rect.bottom = SIZEY;
				InvalidateRect(hwnd, &rect, FALSE);
				break;
	  case SB_LINEDOWN:
				if(wk->nScrollVerPos >= wk->nScrollVerHap-1)	break;
				wk->nScrollVerPos++;
				ScrollWindow(hwnd, 0, -SIZEY, &rect, &rect);
				rect.top = rect.bottom-SIZEY;
				InvalidateRect(hwnd, &rect, FALSE);
				break;
	  case SB_PAGEUP:
				if(wk->nScrollVerPos <= 0)	break;
				wk->nScrollVerPos -= wk->ylimit;
				if(wk->nScrollVerPos < 0)
					wk->nScrollVerPos = 0;
				InvalidateRect(hwnd, NULL, TRUE);
				break;
	  case SB_PAGEDOWN:
				if(wk->nScrollVerPos >= wk->nScrollVerHap-1)	break;
				wk->nScrollVerPos += wk->ylimit;
				if(wk->nScrollVerPos >= wk->nScrollVerHap)
					wk->nScrollVerPos = wk->nScrollVerHap-1;
				InvalidateRect(hwnd, NULL, TRUE);
				break;
	  case SB_THUMBPOSITION:
				wk->nScrollVerPos = HIWORD(wParam);
				InvalidateRect(hwnd, NULL, TRUE);
				break;

	}

	GlobalUnlock(hGlobal);
	SetScrollPos(hwnd, SB_VERT, wk->nScrollVerPos, TRUE);
}

static void WmMouseWheel(HWND hwnd, WPARAM wParam)
{
	RECT rect;
	HGLOBAL    hGlobal;
	WORK_SCAN_DATA 	*wk;

	GetClientRect(hwnd, &rect);
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	if(wk->nScrollVerHap > 0) {

		short gab = (short)HIWORD(wParam)/WHEEL_DELTA*2;

		wk->nScrollVerPos += (-gab);

		if(wk->nScrollVerPos < 0)
			wk->nScrollVerPos = 0;
		if(wk->nScrollVerPos >= wk->nScrollVerHap)
			wk->nScrollVerPos = wk->nScrollVerHap-1;

		SetScrollPos(hwnd, SB_VERT, wk->nScrollVerPos, TRUE);

		InvalidateRect(hwnd, NULL, TRUE);
	}

	GlobalUnlock(hGlobal);
}

void InvalidateOnePort(HWND hwnd, WORK_SCAN_DATA *wk, int port)
{
	RECT r;

	GetClientRect(hwnd, &r);
	
	if(port < wk->nScrollVerPos)	return;
	
	r.top = (port-wk->nScrollVerPos)*SIZEY;
	r.bottom = r.top+SIZEY;

	InvalidateRect(hwnd, &r, FALSE);
}

static void WmKeyDown(HWND hwnd, WPARAM wParam)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	RECT rect;

	GetClientRect(hwnd, &rect);
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	switch(wParam) {
		case VK_ESCAPE:
			PostMessage(GetParent(hwnd), WM_COMMAND, IDC_SCANBUF_BUTTON_CLOSE, 0L);
			break;
		case VK_UP:
			if(wk->nCursorY <= 0)	break;
			if(wk->nCursorY < wk->nScrollVerPos) {
				wk->nScrollVerPos --;
				//ScrollWindow(hwnd, 0, SIZEY, &rect, &rect);
				//rect.bottom = SIZEY;
				InvalidateRect(hwnd, NULL, FALSE);			
			}
			else if(wk->nCursorY == wk->nScrollVerPos) {
				wk->nScrollVerPos --;
				wk->nCursorY--;
				//ScrollWindow(hwnd, 0, SIZEY, &rect, &rect);
				//rect.bottom = SIZEY;
				InvalidateRect(hwnd, NULL, FALSE);			
			}
			else {
				InvalidateOnePort(hwnd, wk, wk->nCursorY);
				wk->nCursorY--;
				InvalidateOnePort(hwnd, wk, wk->nCursorY);
			}
			break;
		case VK_DOWN:
			if(wk->nCursorY >= MAX_PORT-1)	break;
			if(wk->nCursorY > wk->nScrollVerPos+wk->ylimit-1) {
				wk->nScrollVerPos ++;
				//ScrollWindow(hwnd, 0, SIZEY, &rect, &rect);
				//rect.bottom = SIZEY;
				InvalidateRect(hwnd, NULL, FALSE);			
			}
			else if(wk->nCursorY == wk->nScrollVerPos+wk->ylimit-1) {
				wk->nScrollVerPos ++;
				wk->nCursorY++;
				//ScrollWindow(hwnd, 0, SIZEY, &rect, &rect);
				//rect.bottom = SIZEY;
				InvalidateRect(hwnd, NULL, FALSE);			
			}
			else {
				InvalidateOnePort(hwnd, wk, wk->nCursorY);
				wk->nCursorY++;
				InvalidateOnePort(hwnd, wk, wk->nCursorY);
			}
			break;
	}

	GlobalUnlock(hGlobal);
}

void PlcDeviceResetErrorCount(DEVICE_STRUCT *device);

static void WmCommand(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
//	HDC hdc;
	GLOBAL_PORT_STRUCT *port;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	switch(wParam) {
		/*
		case IDM_EVENT_COUNT_COMM_TRY:
			{
				hdc = GetDC(hwnd);
				DrawCountCommTry(hdc, wk, LOWORD(lParam), HIWORD(lParam));
				ReleaseDC(hwnd, hdc);
			}
			break;
		case IDM_EVENT_COUNT_COMM_TIME_OUT:
			{
				hdc = GetDC(hwnd);
				DrawCountTimeOut(hdc, wk, LOWORD(lParam), HIWORD(lParam));
				ReleaseDC(hwnd, hdc);
			}
			break;
		case IDM_EVENT_COUNT_COMM_CODE_BAD:
			{
				hdc = GetDC(hwnd);
				DrawCountCodeBad(hdc, wk, LOWORD(lParam), HIWORD(lParam));
				ReleaseDC(hwnd, hdc);
			}
			break;

		case IDM_EVENT_COUNT_MODEM_ERROR:
			{
				hdc = GetDC(hwnd);
				DrawCountModemError(hdc, wk, lParam);
				ReleaseDC(hwnd, hdc);
			}
			break;
		case IDM_EVENT_COUNT_LAST_CALL_TIME:
			{
				hdc = GetDC(hwnd);
				DrawLastCallTime(hdc, wk, lParam);
				ReleaseDC(hwnd, hdc);
			}
			break;
		case IDM_EVENT_COUNT_COUNTDOWN:
			{
				hdc = GetDC(hwnd);
				DrawCountDown(hdc, wk, lParam);
				ReleaseDC(hwnd, hdc);
			}
			break;
		*/
		case IDC_SCANBUF_BUTTON_CLEAR_ERROR:
			{
				GLOBAL_PORT_STRUCT *port = &portBuf[wk->nCursorY];
				PlcDeviceResetErrorCount(&port->local.device);
			}
			break;
		case IDC_SCANBUF_BUTTON_HAND_CONNECTION:
			{
				char buf[80];
				port = &portBuf[wk->nCursorY];

				if(port->bActiveFlag == OFF) {
					if(IsLangKorean()) {
						MessageBox(hwnd, "사용하지 않는 포트입니다.\n수동접속을 할 수 없습니다.", "포트 사용안함", MB_OK);
					} else {
						MessageBox(hwnd, "Inactive port.\nCan't connection.", "Port Inactive", MB_OK);
					}
					break;
				}

				if(!PlcDeviceIsModemDevice(&port->local.device)) {
					if(IsLangKorean()) {
						MessageBox(hwnd, "모뎀 장치만 수동으로 접속할 수 있습니다.", "모뎀 장치 아님", MB_OK);
					}else {
						MessageBox(hwnd, "Port is not modem device.", "not modem", MB_OK);
					}
					break;
				}

				if(IsLangKorean()) {
					sprintf(buf, "%s\nTel:%s 에\n접속할까요?", port->sTitle, port->tel.sTelNumber);
				} else {
					sprintf(buf, "%s\nTel:%s\nConnect?", port->sTitle, port->tel.sTelNumber);
				}

				if(MessageBox(hwnd, buf, "Connect", MB_YESNO) == IDYES) {
					PlcDeviceSetHandConnection(&port->local.device);
				}
			}
			break;
		case IDC_SCANBUF_BUTTON_HAND_DISCONNECTION:
			port = &portBuf[wk->nCursorY];
			PlcDeviceSetHandDisConnection(&port->local.device);
			break;
	}
	GlobalUnlock(hGlobal);
}

static void WmTimer(HWND hwnd)
{
	static char order;

	order ++;
	order %= 2;

	if(order) {
		return;
	}

	GLOBAL_PORT_STRUCT *pt;
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	int i, j;
	HDC hdc;
	SYSTEMTIME t;
	RECT rect;

	GetClientRect(hwnd, &rect);

	hdc = GetDC(hwnd);

	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	int y = 0;

	// MAX_PORT 까지 모두 그리는 것 보다 현재 화면에 있는 것만 하는 것이 더 빠르다. 2011-12-9
	for(i = wk->nScrollVerPos; i < MAX_PORT && y < rect.bottom; i++, y+=SIZEY) {

		pt = &portBuf[i];
		if(pt->bActiveFlag == OFF)	continue;
		
		for(j = 0; j < 2; j++) {
			if(wk->compare[i].device[j].lCountCommTry != pt->countDevice[j].Total.lCountCommTry) {
				DrawCountCommTry(hdc, wk, i, j);
				wk->compare[i].device[j].lCountCommTry = pt->countDevice[j].Total.lCountCommTry;
			}
			if(wk->compare[i].device[j].lCountTimeOut != pt->countDevice[j].Total.lCountTimeOut) {
				DrawCountTimeOut(hdc, wk, i, j);
				wk->compare[i].device[j].lCountTimeOut = pt->countDevice[j].Total.lCountTimeOut;
			}
			if(wk->compare[i].device[j].lCountCodeBad != pt->countDevice[j].Total.lCountCodeBad) {
				DrawCountCodeBad(hdc, wk, i, j);
				wk->compare[i].device[j].lCountCodeBad = pt->countDevice[j].Total.lCountCodeBad;
			}
		}

		if(pt->local.device.nDeviceStyle == DEVICE_TYPE_MODEM) {
			PlcDeviceGetLastCallTime(&pt->local.device, &t);
			if(memcmp(&wk->compare[i].tLastCall, &t, sizeof(SYSTEMTIME)) != 0) {
				DrawLastCallTime(hdc, wk, i);
				memcpy(&wk->compare[i].tLastCall, &t, sizeof(SYSTEMTIME));
			}

			PlcDeviceGetCountDown(&pt->local.device, &t);
			if(memcmp(&wk->compare[i].tCountDown, &t, sizeof(SYSTEMTIME)) != 0) {
				DrawCountDown(hdc, wk, i);
				memcpy(&wk->compare[i].tCountDown, &t, sizeof(SYSTEMTIME));
			}

			int count = PlcDeviceGetErrorCount(&pt->local.device);
			if(wk->compare[i].nModemErrorNo != count) {
				DrawCountModemError(hdc, wk, i);
				wk->compare[i].nModemErrorNo = count;
			}
		}

	}

	GlobalUnlock(hGlobal);

	ReleaseDC(hwnd, hdc);
}

static void WmLButtonDown(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	int mx = LOWORD(lParam), my = HIWORD(lParam);
	RECT rect;
	int i, y;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	GetClientRect(hwnd, &rect);
	y = 0;

	for(i = wk->nScrollVerPos; i < MAX_PORT && y < rect.bottom; i++, y+=SIZEY) {
		if(my >= y && my <= y+SIZEY) {
			if(wk->nCursorY == i)	break;
			InvalidateOnePort(hwnd, wk, wk->nCursorY);
			wk->nCursorY = i;
			InvalidateOnePort(hwnd, wk, wk->nCursorY);
		}
	}

	GlobalUnlock(hGlobal);
}



long FAR PASCAL EXPORT WndProcViewScanBufPortStatusChild (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL     hGlobal;
	WORK_SCAN_DATA		 *wk;

	switch (message) {
		case WM_CREATE:
			// Allocate memory for window private data
			hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof (WORK_SCAN_DATA)) ;
			SetWindowLong(hwnd, 0, (LONG)hGlobal) ;

			if(hGlobal == NULL) {
				MsgBoxLocalMemoryLow(hwnd, "Local Alloc");
				DestroyWindow(GetParent(hwnd));
				return 0;
			}

			wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
			wk->bScrollVer = ON;
			wk->nScrollVerPos = 0;
			wk->nScrollVerHap = 0;
			wk->ylimit = 5;
			wk->nCursorY = 0;
			wk->compare = new SHOW_COMPARE[MAX_PORT];

			GlobalUnlock (hGlobal);

			SetTimer(hwnd, 1, 0, NULL);

			return 0 ;
		case WM_SIZE:
			WmSize(hwnd, lParam);
			break;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_VSCROLL:
			MessageVScroll(hwnd, wParam, lParam);
			return 0;
		case WM_MOUSEWHEEL:
			WmMouseWheel(hwnd, wParam);
			return 0;
		case WM_KEYDOWN:
			WmKeyDown(hwnd, wParam);
			return 0;
		case WM_COMMAND:
			WmCommand(hwnd, wParam, lParam);
			return 0;
		case WM_LBUTTONDOWN:
			WmLButtonDown(hwnd, wParam, lParam);
			return 0;
		case WM_TIMER:
			WmTimer(hwnd);
			return 0;
		case WM_DESTROY:
			KillTimer(hwnd, 1);
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0) ;
			if(hGlobal != NULL) {
				wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
				delete wk->compare;
				GlobalUnlock(hGlobal);
				GlobalFree  (hGlobal);
			}
			return 0;
	}
	// Pass uwkrocessed message to DefMDIChildProc
	return DefWindowProc (hwnd, message, wParam, lParam) ;
}

typedef struct {
	HWND hwndChild;
	MenuButtonClass menuButton;
} GLOBAL_SCANBUF_FRAME;

static void SizeRebuild(HWND hwnd, GLOBAL_SCANBUF_FRAME *wk)
{
	int x, y;
	RECT rect;
	int needsize;

	GetClientRect(hwnd, &rect);
	x = rect.right-rect.left;
	y = rect.bottom-rect.top;

	wk->menuButton.SetFontSize(cxChar, cyChar);
	needsize = wk->menuButton.GetNeedSizeY(x);

	MoveWindow(wk->hwndChild, 0, cyChar*3+10,   rect.right, y-((cyChar*3+10)+needsize), TRUE);
	wk->menuButton.Move(0, y-needsize);
}

long FAR PASCAL EXPORT WndProcViewScanBufPortStatus (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL hGlobal;
	GLOBAL_SCANBUF_FRAME *wk;
	HDC hdc;
	TEXTMETRIC tm;
	PAINTSTRUCT ps;
	RECT rect;
	HWND hwndChild;
	char buf[80];

	switch (message) {
		case WM_CREATE:
			hwndScanBufPortStatusChild = hwnd;	// 통신 메모리보기 윈도우.

			hdc = GetDC(hwnd);
			if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
			GetTextMetrics(hdc, &tm);
			ReleaseDC(hwnd, hdc);

			cxChar = tm.tmAveCharWidth+1;
			cyChar = tm.tmHeight+tm.tmExternalLeading;
			cxChar = cyChar/2;

			hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof (GLOBAL_SCANBUF_FRAME));
			SetWindowLong(hwnd, 0, (LONG)hGlobal);

			if(hGlobal == NULL) {
				MsgBoxLocalMemoryLow(hwnd, "hGraphicData Alloc");
				DestroyWindow(hwnd);
				return 0;
			}

			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;

			wk->hwndChild = CreateWindow (szViewScanBufPortStatusChildClass, "",
				  WS_CHILD | WS_VSCROLL,
				  0, 0,
				  200, 200,
				  hwnd, NULL, hInst, NULL) ;
			ShowWindow(wk->hwndChild, SW_SHOW);

			wk->menuButton.Init(hwnd, ((LPCREATESTRUCT) lParam) -> hInstance);
			if(IsLangKorean()) {
				wk->menuButton.Insert("감시 프로그램", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert("닫기[ESC]", IDC_SCANBUF_BUTTON_CLOSE);
				
				if(eOemType != OEM_TYPE_SBAS) {
					wk->menuButton.Insert("모뎀오류 초기화", IDC_SCANBUF_BUTTON_CLEAR_ERROR);
					wk->menuButton.Insert("수동 접속", IDC_SCANBUF_BUTTON_HAND_CONNECTION);
					wk->menuButton.Insert("접속 끊기", IDC_SCANBUF_BUTTON_HAND_DISCONNECTION);
				}
			} 
			else if(IsLangChinese() || IsLangJapanese()) 
			{
				CString imsi;
				GetResourceString(IDS_ViewMainProgram, buf, sizeof(buf));
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_VIEWMAIN);
				GetResourceString(IDS_Close, buf, sizeof(buf));
				imsi.Format("%s[ESC]", buf);
				wk->menuButton.Insert(imsi, IDC_SCANBUF_BUTTON_CLOSE);
				GetResourceString(IDS_ClearModemError, buf, sizeof(buf));
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_CLEAR_ERROR);
				GetResourceString(IDS_ManualConnection, buf, sizeof(buf));
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_HAND_CONNECTION);
				GetResourceString(IDS_ManualDisconnection, buf, sizeof(buf));
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_HAND_DISCONNECTION);
			}
			else {
				wk->menuButton.Insert("ViewMain", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert("Close[ESC]", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert("Clear Error", IDC_SCANBUF_BUTTON_CLEAR_ERROR);
				wk->menuButton.Insert("Hand Connection", IDC_SCANBUF_BUTTON_HAND_CONNECTION);
				wk->menuButton.Insert("Hand Disconnection", IDC_SCANBUF_BUTTON_HAND_DISCONNECTION);
			}

			if(hFontMain != NULL)	wk->menuButton.SetFont(hFontMain);

			SetFocus(wk->hwndChild);
			GlobalUnlock(hGlobal);

			return 0 ;
		case WM_SIZE:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
			SizeRebuild(hwnd, wk);
			GlobalUnlock(hGlobal);
			break;
		case WM_PAINT:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;

			GetClientRect(hwnd, &rect);
			hdc = BeginPaint(hwnd, &ps);
			if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			PopBox2(hdc, 0, 0, rect.right, cyChar*3+10, WHITE_GRAY_COLOR);
			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			if(IsLangKorean()) {
				TextOut(hdc, cxChar/2,  5, "번호", 4);
				strcpy(buf, "제목");
				TextOut(hdc, cxChar*5,  5, buf, strlen(buf));
				strcpy(buf, "디바이스1");
				TextOut(hdc, cxChar*25,  5, buf, strlen(buf));
				strcpy(buf, "통신횟수");
				TextOut(hdc, cxChar*50, 5, buf, strlen(buf));
				strcpy(buf, "시간초과");
				TextOut(hdc, cxChar*60, 5, buf, strlen(buf));
				strcpy(buf, "코드불량");
				TextOut(hdc, cxChar*70, 5, buf, strlen(buf));

				strcpy(buf, "프로토콜");
				TextOut(hdc, cxChar*5,  5+cyChar, buf, strlen(buf));
				strcpy(buf, "디바이스2");
				TextOut(hdc, cxChar*25,  5+cyChar, buf, strlen(buf));
				strcpy(buf, "통신횟수");
				TextOut(hdc, cxChar*50, 5+cyChar, buf, strlen(buf));
				strcpy(buf, "시간초과");
				TextOut(hdc, cxChar*60, 5+cyChar, buf, strlen(buf));
				strcpy(buf, "코드불량");
				TextOut(hdc, cxChar*70, 5+cyChar, buf, strlen(buf));
				
				if(eOemType != OEM_TYPE_SBAS) {
					strcpy(buf, "전화번호");
					TextOut(hdc, cxChar*5,  5+cyChar*2, buf, strlen(buf));
				}

				strcpy(buf, "접속주기");
				TextOut(hdc, cxChar*25,  5+cyChar*2, buf, strlen(buf));
				strcpy(buf, "접속시간");
				TextOut(hdc, cxChar*34,  5+cyChar*2, buf, strlen(buf));
				
				strcpy(buf, "최종 호출 시간");
				TextOut(hdc, cxChar*48, 5+cyChar*2, buf, strlen(buf));
				strcpy(buf, "카운트다운");
				TextOut(hdc, cxChar*65, 5+cyChar*2, buf, strlen(buf));
				strcpy(buf, "오류코드");
				TextOut(hdc, cxChar*77, 5+cyChar*2, buf, strlen(buf));				
			}
			else if(IsLangChinese()) {

				GetResourceString(IDS_No, buf, sizeof(buf));
				TextOut(hdc, cxChar/2,  5, buf, strlen(buf));
				GetResourceString(IDS_Title, buf, sizeof(buf));
				TextOut(hdc, cxChar*5,  5, buf, strlen(buf));
				GetResourceString(IDS_Device1, buf, sizeof(buf));
				TextOut(hdc, cxChar*25,  5, buf, strlen(buf));
				GetResourceString(IDS_CommCount, buf, sizeof(buf));
				TextOut(hdc, cxChar*50, 5, buf, strlen(buf));
				GetResourceString(IDS_TimeOut, buf, sizeof(buf));
				TextOut(hdc, cxChar*60, 5, buf, strlen(buf));
				GetResourceString(IDS_CodeBad, buf, sizeof(buf));
				TextOut(hdc, cxChar*70, 5, buf, strlen(buf));

				GetResourceString(IDS_Protocol, buf, sizeof(buf));
				TextOut(hdc, cxChar*5,  5+cyChar, buf, strlen(buf));
				GetResourceString(IDS_Device2, buf, sizeof(buf));
				TextOut(hdc, cxChar*25,  5+cyChar, buf, strlen(buf));
				GetResourceString(IDS_CommCount, buf, sizeof(buf));
				TextOut(hdc, cxChar*50, 5+cyChar, buf, strlen(buf));
				GetResourceString(IDS_TimeOut, buf, sizeof(buf));
				TextOut(hdc, cxChar*60, 5+cyChar, buf, strlen(buf));
				GetResourceString(IDS_CodeBad, buf, sizeof(buf));
				TextOut(hdc, cxChar*70, 5+cyChar, buf, strlen(buf));
				
				GetResourceString(IDS_TelephoneNumber, buf, sizeof(buf));
				TextOut(hdc, cxChar*5,  5+cyChar*2, buf, strlen(buf));

				GetResourceString(IDS_ConnectionCycle, buf, sizeof(buf));
				TextOut(hdc, cxChar*25,  5+cyChar*2, buf, strlen(buf));
				GetResourceString(IDS_ConnectionTime, buf, sizeof(buf));
				TextOut(hdc, cxChar*34,  5+cyChar*2, buf, strlen(buf));
				
				GetResourceString(IDS_LastCalledTime, buf, sizeof(buf));
				TextOut(hdc, cxChar*48, 5+cyChar*2, buf, strlen(buf));
				GetResourceString(IDS_CountDown, buf, sizeof(buf));
				TextOut(hdc, cxChar*65, 5+cyChar*2, buf, strlen(buf));
				GetResourceString(IDS_ErrorCode, buf, sizeof(buf));
				TextOut(hdc, cxChar*77, 5+cyChar*2, buf, strlen(buf));
			}
			else {
				TextOut(hdc, cxChar/2,  5, "No", 2);
				strcpy(buf, "Title");
				TextOut(hdc, cxChar*5,  5, buf, strlen(buf));
				strcpy(buf, "Device1");
				TextOut(hdc, cxChar*25,  5, buf, strlen(buf));
				strcpy(buf, "Total");
				TextOut(hdc, cxChar*50, 5, buf, strlen(buf));
				strcpy(buf, "TOut");
				TextOut(hdc, cxChar*60, 5, buf, strlen(buf));
				strcpy(buf, "CBad");
				TextOut(hdc, cxChar*70, 5, buf, strlen(buf));

				strcpy(buf, "Protocol");
				TextOut(hdc, cxChar*5,  5+cyChar, buf, strlen(buf));
				strcpy(buf, "Device2");
				TextOut(hdc, cxChar*25,  5+cyChar, buf, strlen(buf));
				strcpy(buf, "Total");
				TextOut(hdc, cxChar*50, 5+cyChar, buf, strlen(buf));
				strcpy(buf, "TOut");
				TextOut(hdc, cxChar*60, 5+cyChar, buf, strlen(buf));
				strcpy(buf, "CBad");
				TextOut(hdc, cxChar*70, 5+cyChar, buf, strlen(buf));
				
				strcpy(buf, "Telephone Number");
				TextOut(hdc, cxChar*5,  5+cyChar*2, buf, strlen(buf));

				strcpy(buf, "Cycle");
				TextOut(hdc, cxChar*25,  5+cyChar*2, buf, strlen(buf));
				strcpy(buf, "Time");
				TextOut(hdc, cxChar*34,  5+cyChar*2, buf, strlen(buf));
				
				strcpy(buf, "Last Call Time");
				TextOut(hdc, cxChar*48, 5+cyChar*2, buf, strlen(buf));
				strcpy(buf, "CountDown");
				TextOut(hdc, cxChar*65, 5+cyChar*2, buf, strlen(buf));
				strcpy(buf, "ModemError");
				TextOut(hdc, cxChar*77, 5+cyChar*2, buf, strlen(buf));
			}

			EndPaint(hwnd, &ps);
			GlobalUnlock(hGlobal);
			return 0;
		case WM_COMMAND:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
			switch(wParam) {
				case IDC_SCANBUF_BUTTON_VIEWMAIN:
							SendMessage(hwndMainFrame, WM_COMMAND, IDM_VIEW_VIEWMAIN, 0L);
                     break;
				case IDC_SCANBUF_BUTTON_CLOSE:
							SendMessage(hwndMainClient, WM_MDIDESTROY, (WPARAM)hwnd, 0L);
							break;
				default:	
							hwndChild = wk->hwndChild;
							GlobalUnlock(hGlobal);
							return SendMessage(hwndChild, WM_COMMAND, wParam, lParam);
			}
			GlobalUnlock(hGlobal);
			return 0;
		case WM_SETFOCUS:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
			SetFocus(wk->hwndChild);
			GlobalUnlock(hGlobal);
			return 0;
		case WM_DESTROY:
	      	hwndScanBufPortStatusChild = NULL;	// 통신 메모리보기 윈도우.
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);

			if(hGlobal != NULL) {
				wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
				DestroyWindow(wk->hwndChild);
				GlobalUnlock(hGlobal);
				GlobalFree (hGlobal);
			}

			return 0;
	}
	// Pass unprocessed message to DefMDIChildProc
	return DefMDIChildProc (hwnd, message, wParam, lParam) ;
}