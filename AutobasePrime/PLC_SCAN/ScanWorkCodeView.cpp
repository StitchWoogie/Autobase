// english O.K
#include "stdafx.h"
#include <stdio.h>
#include <string.h>

#include <tools.h>
#include <glib.h>
#include <menubutn.h>
#include <totaldef.h> 

#include "plc_scan.h"
#include "resource.h"

#define	CAT_COLOR_TEXT		RGB(255, 255, 255)
#define	CAT_COLOR_BACK		RGB(0, 0, 0x80)

#define	IDC_SCANBUF_BUTTON_VIEWMAIN		7000
#define	IDC_SCANBUF_BUTTON_CLOSE		7001
#define	IDC_SCANBUF_BUTTON_HEX_ASCII	7002
#define	IDC_SCANBUF_BUTTON_PAUSE		7003

extern	char  szViewScanBufAsciiSendChildClass [];
extern	char  szViewScanBufAsciiRecvChildClass [];
extern  HWND  hwndScanBufCodeViewChild;	// 통신 코드보기 윈도우.

typedef struct {
		int		posx;
		int		posy;
		int		ylimit;						// 한 화면에 보일수 있는 라인수
		RECT	rect;
} WORK_SCAN_DATA;

static int nViewPort = 0;

static int	cxChar, cyChar;
static char bHexOrAscii = 0;

static HWND hwndScanBufSend = NULL;		// 통신 코드보기 윈도우.
static HWND hwndScanBufRecv = NULL;		// 통신 코드보기 윈도우.
static char bPause = OFF;				// 통신 코드보기 잠시 대기 상태.

char cFirstCommunicationFlag = OFF;

void ProtocolSetCodeStartEnd(LOCAL_PORT_STRUCT *pt, int s_code, int e_code)
{
	s_code = s_code;
}

void ProtocolSetCodeMode(LOCAL_PORT_STRUCT *pt, int mode)
{
	mode = mode;
}

void ChangeCodeMode(char type)
{
	bHexOrAscii = type;
}

typedef struct {
	BYTE send_recv;	// 0 = send, 1 = recv, 2 = send next line, 3 = recv next line
	BYTE code;
} CODE_DATA;

#define MAX_CODE_DATA	20000
CODE_DATA codeData[MAX_CODE_DATA];
int nCodeDataTarget = 0;
int nCodeDataCurrent = 0;

static void CodeDataPlus(char send_recv, BYTE code)
{
	nCodeDataTarget++;
	nCodeDataTarget%=MAX_CODE_DATA;
	CODE_DATA *item = &codeData[nCodeDataTarget];
	item->send_recv = send_recv;
	item->code = code;
}

void DisplaySendCode(int port, BYTE code)
{
	if(hwndScanBufSend == NULL)	return;
	if(bPause)			return;
	if(nViewPort != port)	return;

	if(portBuf[port].bActiveThread)
		CodeDataPlus(0, code);
	else
		SendMessage(hwndScanBufSend, WM_COMMAND, IDM_DISPLAY_SEND_CODE, MAKELONG(code, 0));
}

void DisplaySendCodeNextLine(int port)
{
	if(hwndScanBufSend == NULL)	return;
	if(bPause)	return;
	if(nViewPort != port)	return;

	if(portBuf[port].bActiveThread)
		CodeDataPlus(2, 0);
	else
		SendMessage(hwndScanBufSend, WM_COMMAND, IDM_DISPLAY_SEND_CODE, MAKELONG(0, 1));
}

void DisplaySendString(int port, const char *buf)
{
	unsigned i;

	for(i = 0; i < strlen(buf); i++) {
		DisplaySendCode(port, buf[i]);
	}
}

void DisplayRecvCode(int port, BYTE code)
{
	if(hwndScanBufRecv == NULL)	return;
	if(bPause)	return;		
	if(nViewPort != port)	return;

	if(portBuf[port].bActiveThread)
		CodeDataPlus(1, code);
	else
		SendMessage(hwndScanBufRecv, WM_COMMAND, IDM_DISPLAY_RECV_CODE, code);
}

void DisplayRecvCodeNextLine(int port)
{
	if(hwndScanBufRecv == NULL)	return;
	if(bPause)	return;
	if(nViewPort != port)	return;

	if(portBuf[port].bActiveThread)
		CodeDataPlus(3, 0);
	else
		SendMessage(hwndScanBufRecv, WM_COMMAND, IDM_DISPLAY_RECV_CODE, MAKELONG(0, 1));
}

void DisplayRecvString(int port, const char *buf)
{
	unsigned i;

	for(i = 0; i < strlen(buf); i++) {
		DisplayRecvCode(port, buf[i]);
	}
}

//------------------------------------------------------------------------------
//	PCL WM_PAINT 메세지
//------------------------------------------------------------------------------

static void WmPaint(HWND hwnd, const char *title)
{
	PAINTSTRUCT ps;
	HDC hdc;
	HGLOBAL hGlobal;
	WORK_SCAN_DATA		 *wk;
	RECT rect;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal);

	hdc = BeginPaint(hwnd, &ps);

	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);

	PopBox2(hdc,  0, 0, wk->rect.right-1, wk->rect.bottom-1, WHITE_GRAY_COLOR);
	PushBox2(hdc, 3, 3, wk->rect.right-4, wk->rect.bottom-4, CAT_COLOR_BACK);
	PopBox2(hdc,  4, 4, wk->rect.right-5, 4+cyChar, WHITE_GRAY_COLOR);
	rect.left = 4;
	rect.top  = 4;
	rect.right = wk->rect.right-4;
	rect.bottom = rect.top+cyChar;
	SetTextColor(hdc, DARK_COLOR);
	SetBkMode(hdc, TRANSPARENT);
	DrawText(hdc, title, strlen(title), &rect, DT_CENTER|DT_VCENTER|DT_SINGLELINE);
	EndPaint(hwnd, &ps);

	GlobalUnlock (hGlobal);
}

//------------------------------------------------------------------------------
//	PCL WM_SIZE 메세지
//------------------------------------------------------------------------------

static void WmSize(HWND hwnd, LPARAM /*lParam*/)
{
	HGLOBAL hGlobal;
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

	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	wk->ylimit = rect.bottom/cyChar;
	if(wk->ylimit <= 0)	wk->ylimit = 1;
	GetClientRect(hwnd, &wk->rect);
	wk->posx = 5;
	wk->posy = 5+cyChar;

	GlobalUnlock (hGlobal);
}

//---------------------------------------------------------------------------
//	윈도우 타이틀을 만든다.
//---------------------------------------------------------------------------

static void SetScanBufTitle(HWND hwnd)
{
	char buf[80];

	if(IsLangKorean()) {
		sprintf(buf, "통신 코드 보기");
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		GetResourceString(IDS_ViewCommCode, buf, sizeof(buf));
	}
	else {
		sprintf(buf, "Communication Code View");
	}
	SetWindowText(hwnd, buf);
}

static void NextLine(HDC hdc, WORK_SCAN_DATA *wk)
{
	wk->posx = 5;
	if(wk->posy+cyChar*2 > wk->rect.bottom-3) {
		RECT r;

		r.left = 5;
		r.top  = 5+cyChar;
		r.right = wk->rect.right-5;
		r.bottom = wk->rect.bottom-5;

		ScrollDC(hdc, 0, -cyChar, &r, &r, NULL, NULL);
		gcls(hdc, 5, wk->posy, wk->rect.right-5, wk->posy+cyChar, CAT_COLOR_BACK);
	}
	else {
		wk->posy += cyChar;
	}
}

static void DisplayOneChar(HWND hwnd, LPARAM lParam)
{
	HGLOBAL hGlobal;
	WORK_SCAN_DATA		*wk;
	HDC hdc;
	char buf[10];
	SIZE size;
	COLORREF lBackColor;

	hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	hdc = GetDC(hwnd);
	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
	if(HIWORD(lParam) == 1) {
		NextLine(hdc, wk);
	}
	else {

		if(bHexOrAscii == 0) {
			if(cFirstCommunicationFlag) {
      		cFirstCommunicationFlag = OFF;
				lBackColor = RGB(0x80, 0, 0);
			}
			else
				lBackColor = CAT_COLOR_BACK;

			sprintf(buf, "%02X ", lParam);
		}
		else {
			lBackColor = RGB(0x80, 0, 0);
			switch(lParam) {
				case 0:		lBackColor = RGB(0x80, 0x80, 0x80);
							strcpy(buf, "<NULL>");
							break;							
				case SOH:	lBackColor = RGB(0x80, 0x40, 0x80);
							strcpy(buf, "<SOH>");
							break;							
				case STX:   lBackColor = RGB(0x80, 0, 0);
								strcpy(buf, "<STX>");
								break;
				case ETX:   lBackColor = RGB(0, 0x80, 0);
								strcpy(buf, "<ETX>");
								break;
				case EOT:	lBackColor = RGB(0x80, 0x80, 0);
								strcpy(buf, "<EOT>");
								break;
				case ENQ:	lBackColor = RGB(0x80, 0, 0x80);
								strcpy(buf, "<ENQ>");
								break;
				case ACK:	lBackColor = RGB(0, 0x80, 0x80);
								strcpy(buf, "<ACK>");
								break;
				case LF:		strcpy(buf, "<LF>");		break;
				case CR:		strcpy(buf, "<CR>");		break;
				case DLE:	strcpy(buf, "<DLE>");	break;
				case NAK:	strcpy(buf, "<NAK>");	break;
				case 0x09:	strcpy(buf, "<HT>");		break;
				default:
							lBackColor = CAT_COLOR_BACK;
							sprintf(buf, "%c", lParam);
							break;
			}
		}

		GetTextExtentPoint32(hdc, buf, strlen(buf), &size);

		if(size.cx+wk->posx > wk->rect.right-5) {
			NextLine(hdc, wk);
		}

		SetTextColor(hdc, WHITE_COLOR);
		SetBkColor(hdc, lBackColor);
		TextOut(hdc, wk->posx, wk->posy, buf, strlen(buf));

		wk->posx += size.cx;
	}

	ReleaseDC(hwnd, hdc);

	GlobalUnlock (hGlobal);
}

long FAR PASCAL EXPORT WndProcViewScanBufAsciiSendChild (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL         hGlobal;
	WORK_SCAN_DATA		 *wkLocalData;

	switch (message) {
		case WM_CREATE:
			// Allocate memory for window private data
			hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof (WORK_SCAN_DATA)) ;
			SetWindowLong (hwnd, 0, (LONG)hGlobal) ;

			if(hGlobal == NULL) {
				MsgBoxLocalMemoryLow(hwnd, "Local Alloc");
				DestroyWindow(GetParent(hwnd));
				return 0;
			}

			wkLocalData = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
			wkLocalData->ylimit = 5;

			GlobalUnlock (hGlobal);

			return 0 ;
		case WM_SIZE:
			WmSize(hwnd, lParam);
			break;
		case WM_PAINT:
			if(IsLangKorean())
				WmPaint(hwnd, "송신 코드");
			else if(IsLangChinese() || IsLangJapanese()) {
				CString buf;
				GetResourceString(IDS_SendCode, buf);
				WmPaint(hwnd, buf);
			}
			else
				WmPaint(hwnd, "Send Code");

			return 0;
		case WM_COMMAND:
			switch(wParam) {
				case IDM_DISPLAY_SEND_CODE:
					DisplayOneChar(hwnd, lParam);
					break;
			}
			return 0;
		case WM_DESTROY:
			hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0) ;
			if(hGlobal != NULL) {
				wkLocalData = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
				GlobalUnlock(hGlobal);
				GlobalFree  (hGlobal);
			}
			return 0 ;
	}
	// Pass uwkrocessed message to DefMDIChildProc
	return DefWindowProc (hwnd, message, wParam, lParam) ;
}

long FAR PASCAL EXPORT WndProcViewScanBufAsciiRecvChild (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL         hGlobal;
	WORK_SCAN_DATA		 *wkLocalData;

	switch (message) {
		case WM_CREATE:
			// Allocate memory for window private data
			hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof (WORK_SCAN_DATA)) ;
			SetWindowLong (hwnd, 0, (LONG)hGlobal) ;

			if(hGlobal == NULL) {
				MsgBoxLocalMemoryLow(hwnd, "Local Alloc");
				DestroyWindow(GetParent(hwnd));
				return 0;
			}

			wkLocalData = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
			wkLocalData->ylimit = 5;

			GlobalUnlock (hGlobal);

			return 0 ;
		case WM_SIZE:
			WmSize(hwnd, lParam);
			break;
		case WM_PAINT:
			if(IsLangKorean()) 
				WmPaint(hwnd, "수신 코드");
			else if(IsLangChinese() || IsLangJapanese()) {
				CString buf;
				GetResourceString(IDS_RecvCode, buf);
				WmPaint(hwnd, buf);
			}
			else
				WmPaint(hwnd, "Receive Code");
			return 0;
		case WM_COMMAND:
      	switch(wParam) {
				case IDM_DISPLAY_RECV_CODE:
					DisplayOneChar(hwnd, lParam);
					break;
			}
			return 0;
		case WM_DESTROY:
			hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0) ;
			if(hGlobal != NULL) {
				wkLocalData = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
				GlobalUnlock(hGlobal);
				GlobalFree  (hGlobal);
			}
			return 0 ;
	}
	// Pass uwkrocessed message to DefMDIChildProc
	return DefWindowProc (hwnd, message, wParam, lParam) ;
}

typedef struct {
	HWND hwndList;
	HWND hwndChildSend;
	HWND hwndChildRecv;
	int  nStatusBoxY;
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

	int remain_x = rect.right-cxChar*7;

	MoveWindow(wk->hwndList, 0, 0, cxChar*7, y-needsize, TRUE);
	MoveWindow(wk->hwndChildSend, cxChar*7, 0, remain_x/2, y-needsize, TRUE);
	MoveWindow(wk->hwndChildRecv, cxChar*7+remain_x/2, 0, remain_x-remain_x/2, y-needsize, TRUE);
	wk->menuButton.Move(0, y-needsize);
}

static void WmTimer()
{
	static char order;

	order++;
	order %= 2;
	if(order)	return;

	for(int i = 0; i < 1000; i++) {
		if(nCodeDataTarget == nCodeDataCurrent)		return;

		nCodeDataCurrent++;
		nCodeDataCurrent%=MAX_CODE_DATA;

		CODE_DATA *item = &codeData[nCodeDataCurrent];

		if(item->send_recv == 0) 
			SendMessage(hwndScanBufSend, WM_COMMAND, IDM_DISPLAY_SEND_CODE, MAKELONG(item->code, 0));
		else if(item->send_recv == 1) 
			SendMessage(hwndScanBufRecv, WM_COMMAND, IDM_DISPLAY_RECV_CODE, MAKELONG(item->code, 0));
		else if(item->send_recv == 2) 
			SendMessage(hwndScanBufSend, WM_COMMAND, IDM_DISPLAY_SEND_CODE, MAKELONG(0, 1));
		else if(item->send_recv == 3) 
			SendMessage(hwndScanBufRecv, WM_COMMAND, IDM_DISPLAY_RECV_CODE, MAKELONG(0, 1));
		else;
	}
}

long FAR PASCAL EXPORT WndProcViewScanBufAscii (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL hGlobal;
	GLOBAL_SCANBUF_FRAME *wk;
	HDC hdc;
	TEXTMETRIC tm;
	PAINTSTRUCT ps;
	RECT rect;
	int i;
	char buf[80];

	switch (message) {
		case WM_CREATE:
      		bPause = OFF;

			hwndScanBufCodeViewChild = hwnd;	// 통신 코드보기 윈도우.
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

			wk->hwndList = CreateWindow("ListBox", "ViewPort", WS_CHILD|WS_BORDER|WS_VISIBLE|WS_VSCROLL|LBS_NOINTEGRALHEIGHT|LBS_NOTIFY,
				  0, 0, 0, 0, hwnd, (HMENU)1, hInst, NULL);

			if(nPortHap == 0) {
				nViewPort = 0;
			}

			for(i = 0; i < MAX_PORT; i++) {
				if(portBuf[i].bActiveFlag == OFF)	continue;
				sprintf(buf, "%03d", i);
				SendMessage(wk->hwndList, LB_ADDSTRING, 0, (LPARAM)buf);
				if(nViewPort == i)
					SendMessage(wk->hwndList, LB_SELECTSTRING, 0, (LPARAM)buf);
			}

			//SendMessage(wk->hwndList, LB_SETCURSEL, nViewPort, 0);

			wk->hwndChildSend = CreateWindow (szViewScanBufAsciiSendChildClass, "",
				  WS_CHILD,
				  0, 0,
				  200, 200,
				  hwnd, (HMENU)2, hInst, NULL);
			ShowWindow(wk->hwndChildSend, SW_SHOW);

			wk->hwndChildRecv = CreateWindow (szViewScanBufAsciiRecvChildClass, "",
				  WS_CHILD,
				  0, 0,
				  200, 200,
				  hwnd, (HMENU)3, hInst, NULL);
			ShowWindow(wk->hwndChildRecv, SW_SHOW);

			wk->menuButton.Init(hwnd, ((LPCREATESTRUCT) lParam) -> hInstance);
			if(IsLangKorean()) {
				wk->menuButton.Insert("감시 프로그램", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert("닫기", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert("16진수/ASCII", IDC_SCANBUF_BUTTON_HEX_ASCII);
				wk->menuButton.Insert("보기 일시정지",     IDC_SCANBUF_BUTTON_PAUSE);
			} 
			else if(IsLangChinese() || IsLangJapanese()) 
			{
				CString imsi;
				GetResourceString(IDS_ViewMainProgram, buf, sizeof(buf));
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_VIEWMAIN);
				GetResourceString(IDS_Close, buf, sizeof(buf));
				imsi.Format("%s", buf);
				wk->menuButton.Insert(imsi, IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert("Hex/Ascii", IDC_SCANBUF_BUTTON_HEX_ASCII);
				GetResourceString(IDS_PauseView, buf, sizeof(buf));
				wk->menuButton.Insert(buf,     IDC_SCANBUF_BUTTON_PAUSE);
			}
			else {
				wk->menuButton.Insert("ViewMain", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert("Close", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert("Hex/Ascii", IDC_SCANBUF_BUTTON_HEX_ASCII);
				wk->menuButton.Insert("View Pause",     IDC_SCANBUF_BUTTON_PAUSE);			
			}
			hwndScanBufSend = wk->hwndChildSend;
			hwndScanBufRecv = wk->hwndChildRecv;
			if(hFontMain != NULL)	wk->menuButton.SetFont(hFontMain);
			GlobalUnlock(hGlobal);
			SetScanBufTitle(hwnd);
			SetTimer(hwnd, 1, 0, NULL);
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

			wk->nStatusBoxY = rect.bottom-(wk->menuButton.GetHeight()+cyChar+10);

			PopBox2(hdc, 0, wk->nStatusBoxY, rect.right, rect.bottom, WHITE_GRAY_COLOR);

			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			EndPaint(hwnd, &ps);
			GlobalUnlock(hGlobal);
			return 0;
		case WM_COMMAND:
			if(HIWORD(wParam) == LBN_SELCHANGE) {
				hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
				wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
				LRESULT retn = SendMessage(wk->hwndList, LB_GETCURSEL, 0, 0L);
				if(retn != LB_ERR) {
					char text[80];
					SendMessage(wk->hwndList, LB_GETTEXT, retn, (LPARAM)text);
					nViewPort = atoi(text);
				}
				GlobalUnlock(hGlobal);

				nCodeDataCurrent = nCodeDataTarget;
			}
			switch(LOWORD(wParam)) {
				case IDC_SCANBUF_BUTTON_VIEWMAIN:
							SendMessage(hwndMainFrame, WM_COMMAND, IDM_VIEW_VIEWMAIN, 0L);
							break;
				case IDC_SCANBUF_BUTTON_CLOSE:
							SendMessage(hwndMainClient, WM_MDIDESTROY, (WPARAM)hwnd, 0L);
							break;
				case IDC_SCANBUF_BUTTON_HEX_ASCII:
							bHexOrAscii++;
							bHexOrAscii %= 2;
							break;
				case IDC_SCANBUF_BUTTON_PAUSE:
							bPause++;
							bPause %= 2;
							break;
			}
			return 0;
		case WM_TIMER:
			WmTimer();
			return 0;
		case WM_DESTROY:
			KillTimer(hwnd, 1);
			hGlobal = (HGLOBAL)GetWindowLong (hwnd, 0);
			if(hGlobal != NULL) {
				GlobalFree (hGlobal) ;
			}
			hwndScanBufSend = NULL;
			hwndScanBufRecv = NULL;
			hwndScanBufCodeViewChild = NULL;	// 통신 코드보기 윈도우.
			return 0;
	}
	// Pass unprocessed message to DefMDIChildProc
	return DefMDIChildProc (hwnd, message, wParam, lParam) ;
}