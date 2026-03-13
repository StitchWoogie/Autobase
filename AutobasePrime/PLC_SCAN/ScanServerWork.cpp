// english O.K
#include "stdafx.h"
#include <stdlib.h>
#include <string.h>

#include <tools.h>
#include <gclass.h>
#include <gatelib.h>
#include <glib.h>

#include "resource.h"
#include "plc_scan.h"
#include "ScanServer.h"

extern HWND hwndScanServer;				// 네트워크 서버 상황

static int	cxChar, cyChar;
static int	nScrollPos = 0;
static int  nCursorY = 0;

SCAN_SERVER_LIST scanServerList[MAX_SCAN_SERVER_LIST];
static char bNeedScreenUpdate;

void ConnectStatusChanged()
{
	bNeedScreenUpdate = ON;
	//if(hwndScanServer == NULL)	return;

	//InvalidateRect(hwndScanServer, NULL, TRUE);
}

static void WmPaint(HWND hwnd)
{
	PAINTSTRUCT ps;
	HDC hdc;
	RECT rect;
	int y, pos;
	CString buf("Not include if");
	CString buf_connect;
	CString imsi;
	char info[80];
	SCAN_SERVER_LIST *conn;

	GetClientRect(hwnd, &rect);

	hdc = BeginPaint(hwnd, &ps);
	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);

	for(pos = nScrollPos, y = 0; y < rect.bottom && pos < MAX_SCAN_SERVER_LIST; y+=cyChar, pos++) {
		conn = &scanServerList[pos];
		if(conn->nDeviceType == 0) {
			if(IsLangKorean()) {
				buf.Format("접속%3d: 사용안함", pos, info);
			}
			else if(IsLangChinese()) {
				GetResourceString(IDS_Connection, buf_connect);
				GetResourceString(IDS_NotUsed, imsi);
				buf.Format("%s%3d: %s", buf_connect, pos, imsi);
			}
			else {
				buf.Format("Connect%3d: Not used", pos, info);
			}
			SetTextColor(hdc, WHITE_GRAY_COLOR);
		}
		else {
			if(conn->bInitialFlag == OFF) {
				if(IsLangKorean()) {
					buf.Format("접속%3d: 접속 초기화 실패 (Try Count=%d)", pos, conn->nTryInitCount);
				} 
				else if(IsLangChinese()) {
					GetResourceString(IDS_Connection, buf_connect);
					GetResourceString(IDS_FailConnectionInitial, imsi);
					buf.Format("%s%3d: %s (Try Count=%d)", buf_connect, pos, imsi, conn->nTryInitCount);
				}
				else {
					buf.Format("Connect%3d: Connection initial failed", pos);
				}
				SetTextColor(hdc, RGB(0x80, 0, 0));
			}
			else {
				if(conn->hGate) {
					GateGetInformationString(conn->hGate, info);
					if(conn->bConnectFlag == OFF) {			// 전화의 상태
						if(IsLangKorean()) {
							buf.Format("접속%3d: 접속 대기 중(%s)", pos, info);
						} 
						else if(IsLangChinese()) {
							GetResourceString(IDS_Connection, buf_connect);
							GetResourceString(IDS_ConnectionReady, imsi);
							buf.Format("%s%3d: %s (%s)", buf_connect, pos, imsi, info);
						}
						else {
							buf.Format("Connect%3d: Ready(%s)", pos, info);
						}
						SetTextColor(hdc, DARK_COLOR);
					}
					else {
						int count = 0;
						int port_one;

						for(int j = 0; j < 256; j++) {
							if(conn->wCastPort[j/16] & WORD_MASK[j%16]) {
								count++;
								port_one = j;
							}
						}

						if(count == 1) {
							if(IsLangKorean()) {
								buf.Format("접속%3d: 접속 중 (%s) BroadCastPort=%d", pos, info, port_one);
							}
							else if(IsLangChinese()) {
								GetResourceString(IDS_Connection, buf_connect);
								GetResourceString(IDS_Connecting, imsi);
								buf.Format("%s%3d: %s (%s) BroadCastPort=%d", buf_connect, pos, imsi, info, port_one);
							}
							else {
								buf.Format("Connect%3d: Connecting(%s) BroadCastPort=%d", pos, info, port_one);
							}
						}
						else {
							if(IsLangKorean()) {
								buf.Format("접속%3d: 접속 중 (%s) BroadCastPorts=%d개", pos, info, count);
							}
							else if(IsLangChinese()) {
								GetResourceString(IDS_Connection, buf_connect);
								GetResourceString(IDS_Connecting, imsi);
								buf.Format("%s%3d: %s (%s) BroadCastPorts=%d", buf_connect, pos, imsi, info, count);
							}
							else {
								buf.Format("Connect%3d: Connecting(%s) BroadCastPorts=count %d", pos, info, count);
							}
						}

						SetTextColor(hdc, RGB(0, 0, 255));
					}
				}
				else {
					if(conn->nDeviceType == 3) {
						if(IsLangKorean()) {
							buf.Format("접속%3d: 접속 대기 (TCP/IP Port:%d listen)", pos, conn->tcpipPort);
						}
						else if(IsLangChinese()) {
							GetResourceString(IDS_Connection, buf_connect);
							GetResourceString(IDS_ConnectionReady, imsi);
							buf.Format("%s%3d: %s (TCP/IP Port:%d listen)", buf_connect, pos, imsi, conn->tcpipPort);
						}
						else {
							buf.Format("Connect%3d: Ready (TCP/IP Port:%d listen)", pos, conn->tcpipPort);
						}
						SetTextColor(hdc, DARK_GRAY_COLOR);
					}
				}
			}
		}

		if(nCursorY == pos) 
			gcls(hdc, 0, y, rect.right, y+cyChar-1, RGB(255, 255, 0));
		else
			gcls(hdc, 0, y, rect.right, y+cyChar-1, WHITE_COLOR);

		SetBkMode(hdc, TRANSPARENT);
		TextOut(hdc, 0, y, buf, strlen(buf));
	}
	gcls(hdc, 0, y, rect.right, rect.bottom, WHITE_COLOR);
	EndPaint(hwnd, &ps);
}

static void WmVScroll(HWND hwnd, WPARAM wParam, LPARAM /*lParam*/)
{
	RECT rect;

	GetClientRect(hwnd, &rect);

	switch(LOWORD(wParam)) {
		case SB_LINEUP:
			if(nScrollPos <= 0) 	break;
			nScrollPos --;
			SetScrollPos(hwnd, SB_VERT, nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, TRUE);
			break;
		case SB_LINEDOWN:
			if(nScrollPos >= MAX_SCAN_SERVER_LIST-1) 	break;
			nScrollPos ++;
			SetScrollPos(hwnd, SB_VERT, nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, TRUE);
			break;
		case SB_PAGEUP:
			if(nScrollPos <= 0) 	break;
			nScrollPos -= rect.bottom/cyChar;
			if(nScrollPos < 0) 	nScrollPos = 0;
			SetScrollPos(hwnd, SB_VERT, nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, TRUE);
			break;
		case SB_PAGEDOWN:
			if(nScrollPos >= MAX_SCAN_SERVER_LIST-1) 	break;
			nScrollPos += rect.bottom/cyChar;
			if(nScrollPos > MAX_SCAN_SERVER_LIST-1) 	nScrollPos = MAX_SCAN_SERVER_LIST-1;
			SetScrollPos(hwnd, SB_VERT, nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, TRUE);
			break;
		case SB_THUMBTRACK:
		case SB_THUMBPOSITION:
			nScrollPos = HIWORD(wParam);
			if(nScrollPos > MAX_SCAN_SERVER_LIST-1) 	nScrollPos = MAX_SCAN_SERVER_LIST-1;
			SetScrollPos(hwnd, SB_VERT, nScrollPos, TRUE);
			InvalidateRect(hwnd, NULL, TRUE);
			break;
	}
}

static void WmLButtonDown(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	short mx = LOWORD(lParam);
	short my = HIWORD(lParam);

	int y;
	int i;

	for(i = nScrollPos, y = 0; i < MAX_SCAN_SERVER_LIST; y+=cyChar, i++) {
		if(my >= y && my < y+cyChar) {
			if(i == nCursorY)	break;
			nCursorY = i;
			InvalidateRect(hwnd, NULL, FALSE);
			break;
		}
	}
}

static void WmLButtonDblClk(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	short mx = LOWORD(lParam);
	short my = HIWORD(lParam);

	int y;
	int i;

	for(i = nScrollPos, y = 0; i < MAX_SCAN_SERVER_LIST; y+=cyChar, i++) {
		if(my >= y && my < y+cyChar) {
			//if(i == nCursorY)	break;
			nCursorY = i;
			InvalidateRect(hwnd, NULL, FALSE);
			void ConfigScanServer(HWND hwnd, int pos);
			ConfigScanServer(hwnd, nCursorY);
			break;
		}
	}
}

LRESULT CALLBACK WndProcViewScanServer (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	HDC             hdc;
	TEXTMETRIC		tm;

	switch (message) {
		case WM_CREATE:
			hwndScanServer = hwnd;

			hdc = GetDC(hwnd);
			if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
			GetTextMetrics(hdc, &tm);
			ReleaseDC(hwnd, hdc);

			cxChar = tm.tmAveCharWidth+1;
			cyChar = tm.tmHeight+tm.tmExternalLeading;
			cxChar = cyChar/2;

			ShowScrollBar(hwnd, SB_VERT, TRUE);
			SetScrollRange(hwnd, SB_VERT, 0, 255, TRUE);
			SetScrollPos(hwnd, SB_VERT, nScrollPos, TRUE);

			SetTimer(hwnd, 1, 1, NULL);
			return 0 ;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_VSCROLL:
			WmVScroll(hwnd, wParam, lParam);
			return 0;
		case WM_LBUTTONDOWN:
			WmLButtonDown(hwnd, wParam, lParam);
			return 0;
		case WM_LBUTTONDBLCLK:
			WmLButtonDblClk(hwnd, wParam, lParam);
			return 0;
		case WM_QUERYENDSESSION:
		case WM_CLOSE:
			break ;   // ie, call DefMDIChildProc
		case WM_TIMER:
			if(bNeedScreenUpdate) {
				InvalidateRect(hwnd, NULL, TRUE);
				bNeedScreenUpdate = OFF;
			}
			return 0; 
		case WM_DESTROY:
			KillTimer(hwnd, 1);
			hwndScanServer = NULL;
			return 0 ;
	}
	// Pass unprocessed message to DefMDIChildProc
	return DefMDIChildProc (hwnd, message, wParam, lParam);
}



