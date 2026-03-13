
#include "stdafx.h"
#include <stdio.h>
#include <string.h>

#include <tools.h>
#include <glib.h>
#include <menubutn.h>

#include "plc_scan.h"
#include "resource.h"

#define	CAT_COLOR_TEXT		RGB(255, 255, 255)
#define	CAT_COLOR_BACK		RGB(0, 0, 0x80)

#define	IDC_SCANBUF_BUTTON_VIEWMAIN	7000
#define	IDC_SCANBUF_BUTTON_CLOSE		7001
#define	IDC_SCANBUF_BUTTON_MINUS		7002
#define	IDC_SCANBUF_BUTTON_PLUS			7003
#define	IDC_SCANBUF_BUTTON_VAR_TYPE		7004
#define	IDC_SCANBUF_BUTTON_VAR_WORD		7005
#define	IDC_SCANBUF_BUTTON_VAR_FLOAT	7006
#define	IDC_SCANBUF_BUTTON_VAR_DWORD	7007
#define	IDC_SCANBUF_BUTTON_VAR_STRING	7008
#define	IDC_SCANBUF_BUTTON_VAR_DOUBLE	7009
#define	IDC_SCANBUF_BUTTON_VAR_INT64	7010
#define	IDC_SCANBUF_BUTTON_VAR_SYSTEM	7011

#define  IDC_CHANGE_COUNT_TOTAL			7020
#define  IDC_CHANGE_COUNT_TIMEOUT		7021
#define  IDC_CHANGE_COUNT_CODEBAD		7022
#define  IDC_CHANGE_SUCCESS_PERCENT		7023

extern	TCHAR szViewScanBufMemoryChildClass [];
extern  HWND hwndScanBufMemoryViewChild;

typedef struct {
	double value;
	char	flag;
} SAVE_BUF;

typedef struct {
	char	flag;
	char	value[256];
} SAVE_BUF_STRING;

typedef struct {
		char   cViewVarType;			// 0 - word view, 1 - float view
		int	   nScrollVerPos;
		int	   nScrollVerHap;
		char   bScrollVer;
		int	   ylimit;					// 한 화면에 보일수 있는 라인수
		int    nPortNum;
		long   lCountTotal[4];
		long   lCountCodeBad[4];
		long   lCountTimeOut[4];
		WORD   wSuccessPercent[4];
		SAVE_BUF saveBuf[100];		// 통신을 하고 난 뒤 버퍼의 내용이 바뀌어졌을 때를 대비해서 원래 버퍼와 현재 버퍼가 같은가를 감시한다.
		SAVE_BUF_STRING saveBufString[100];		// 통신을 하고 난 뒤 버퍼의 내용이 바뀌어졌을 때를 대비해서 원래 버퍼와 현재 버퍼가 같은가를 감시한다.
		int		nScanDevice;
} WORK_SCAN_DATA;

static int	cxChar, cyChar;
static int  nSaveViewPort = 0;
static int  nSaveViewVar = 0;

void TextOut(HDC hdc, int x, int y, const TCHAR *s, int size)
{
	ExtTextOut(hdc, x, y, 0, NULL, s, size, NULL);
}

//------------------------------------------------------------------------------
//	WORD memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufWORD(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeWORD)	{
		bell(100);
		return;
	}

	TCHAR buf[80];

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkMode(hdc, TRANSPARENT);

	gcls(hdc, 0, y, cxChar*16, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	if(portBuf[wk->nPortNum].local.bufWORD[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%5u", portBuf[wk->nPortNum].local.bufWORD[pos].value);
	TextOut(hdc, cxChar*5, y, buf, wcslen(buf));
	
	if(portBuf[wk->nPortNum].local.bufWORD[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);
	
	swprintf(buf, L"%04X", portBuf[wk->nPortNum].local.bufWORD[pos].value);
	TextOut(hdc, cxChar*11, y, buf, wcslen(buf));

	gcls(hdc, cxChar*16, y, cxChar*16+cxChar*16, y+cyChar, DARK_COLOR);

	WORD value = portBuf[wk->nPortNum].local.bufWORD[pos].value;

	for(int j = 0; j < 16; j++) {
		swprintf(buf, L"%X", 15-j);
		if((value >> (15-j)) & 1) {
			SetTextColor(hdc, RGB(255, 0, 0));
			TextOut(hdc, cxChar*16+cxChar*j, y, buf, 1);
		}
		else {
			SetTextColor(hdc, DARK_GRAY_COLOR);
			TextOut(hdc, cxChar*16+cxChar*j, y, buf, 1);
		}
	}
}

//------------------------------------------------------------------------------
//	Float memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufFLOAT(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeFLOAT)	{
		bell(100);
		return;
	}

	WCHAR buf[80];
	RECT r;

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, 0, y, cxChar*32-1, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	swprintf(buf, L"%.4f", portBuf[wk->nPortNum].local.bufFLOAT[pos].value);

	if(portBuf[wk->nPortNum].local.bufFLOAT[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	r.left = cxChar*5;
	r.top  = y;
	r.right = cxChar*28;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufFLOAT[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%6.3E", portBuf[wk->nPortNum].local.bufFLOAT[pos].value);
	r.left = cxChar*30;
	r.top  = y;
	r.right = cxChar*42;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);
}

//------------------------------------------------------------------------------
//	DWORD memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufDWORD(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeDWORD)	{
		bell(100);
		return;
	}

	TCHAR buf[80];
	RECT r;

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, 0, y, cxChar*38-1, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	if(portBuf[wk->nPortNum].local.bufDWORD[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%10lu", portBuf[wk->nPortNum].local.bufDWORD[pos].value);
	r.left = cxChar*5;
	r.top  = y;
	r.right = cxChar*16;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufDWORD[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%08X", portBuf[wk->nPortNum].local.bufDWORD[pos].value);
	r.left = cxChar*17;
	r.top  = y;
	r.right = cxChar*26;
	r.bottom = y+cyChar;

	TextOut(hdc, cxChar*17, y, buf, wcslen(buf));


	if(portBuf[wk->nPortNum].local.bufDWORD[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%d", portBuf[wk->nPortNum].local.bufDWORD[pos].value);
	r.left = cxChar*27;
	r.top  = y;
	r.right = cxChar*38;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);
}


//------------------------------------------------------------------------------
//	STRING memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufSTRING(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeSTRING)	{
		return;
	}

	TCHAR buf[256];
	RECT r;
	STRING_BUF *str = &portBuf[wk->nPortNum].local.bufSTRING[pos];
	

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, 0, y, cxChar*32-1, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	if(str->flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	memcpy(buf, str->value, 255);
	buf[255] = 0;

	r.left   = cxChar*5;
	r.top    = y;
	r.right  = r.left+cxChar*256;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_LEFT);

	
}

//------------------------------------------------------------------------------
//	DOUBLE memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufDOUBLE(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeDOUBLE)	{
		bell(100);
		return;
	}

	TCHAR buf[80];
	RECT r;

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, 0, y, cxChar*32-1, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	swprintf(buf, L"%.4f", portBuf[wk->nPortNum].local.bufDOUBLE[pos].value);

	if(portBuf[wk->nPortNum].local.bufDOUBLE[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	r.left = cxChar*5;
	r.top  = y;
	r.right = cxChar*28;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufDOUBLE[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%6.3E", portBuf[wk->nPortNum].local.bufDOUBLE[pos].value);
	r.left = cxChar*30;
	r.top  = y;
	r.right = cxChar*42;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);
}

//------------------------------------------------------------------------------
//	INT64 memory 한줄을 그린다.
// 음수 9223372036854775808에서 양수 9223372036854775807 사이인 정수   19자리를 차지한다. 922경
//------------------------------------------------------------------------------

static void DrawOnlyBufINT64(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeINT64)	{
		bell(100);
		return;
	}

	TCHAR buf[80];
	RECT r;

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, 0, y, cxChar*63-1, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	if(portBuf[wk->nPortNum].local.bufINT64[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%19I64u", portBuf[wk->nPortNum].local.bufINT64[pos].value);
	r.left = cxChar*5;
	r.top  = y;
	r.right = cxChar*25;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufINT64[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%016I64X", portBuf[wk->nPortNum].local.bufINT64[pos].value);
	//r.left = cxChar*26;
	//r.top  = y;
	//r.right = cxChar*43;
	//r.bottom = y+cyChar;

	TextOut(hdc, cxChar*26, y, buf, wcslen(buf));

	if(portBuf[wk->nPortNum].local.bufINT64[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	swprintf(buf, L"%I64d", portBuf[wk->nPortNum].local.bufINT64[pos].value);
	r.left = cxChar*43;
	r.top  = y;
	r.right = cxChar*63;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, wcslen(buf), &r, DT_RIGHT);
}

//------------------------------------------------------------------------------
//	SYSTEM memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufSYSTEM(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= MAX_ATTR_WORD)	{
		bell(100);
		return;
	}

	TCHAR buf[80];

	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkMode(hdc, TRANSPARENT);

	gcls(hdc, 0, y, cxChar*15, y+cyChar, CAT_COLOR_BACK);

	swprintf(buf, L"%03d", pos);
	TextOut(hdc, 0, y, buf, wcslen(buf));

	SetTextColor(hdc, RGB(255, 255, 0));

	swprintf(buf, L"%5u", portBuf[wk->nPortNum].bufSYSTEM[pos].value);
	TextOut(hdc, cxChar*4, y, buf, wcslen(buf));
	
	SetTextColor(hdc, RGB(0, 255, 255));
	
	swprintf(buf, L"%04X", portBuf[wk->nPortNum].bufSYSTEM[pos].value);
	TextOut(hdc, cxChar*10, y, buf, wcslen(buf));

	gcls(hdc, cxChar*15, y, cxChar*15+cxChar*16, y+cyChar, DARK_COLOR);

	WORD value = portBuf[wk->nPortNum].bufSYSTEM[pos].value;

	for(int j = 0; j < 16; j++) {
		swprintf(buf, L"%X", 15-j);
		if((value >> (15-j)) & 1) {
			SetTextColor(hdc, RGB(255, 0, 0));
			TextOut(hdc, cxChar*15+cxChar*j, y, buf, 1);
		}
		else {
			SetTextColor(hdc, DARK_GRAY_COLOR);
			TextOut(hdc, cxChar*15+cxChar*j, y, buf, 1);
		}
	}

#define MAX_SYSTEM_DES 34
	const TCHAR *des[MAX_SYSTEM_DES] = {
		L"b0-TimeOut1, b1-TimeOut5, b2-TimeOut10, b3-TimeOut20, b4-TimeOut30 (ON=OK,OFF=Fail)",
		L"b0-ModemConnect (ON=Connected,OFF=not connected)",
		L"b0-Device no of line duplex (OFF=Primary,ON=Secondary)",
		L"b0-TimeOut on write, b1-CodeBad on write (ON=Error, OFF=OK)",
		L"PC Duplex(b0-Another pc active, b1-This pc master, b2-Another pc master, b3-this pc Timeout, b4-another pc timeout",
		L"Communication Success (%*100)",
		L"Communication Fail (%*100)",
		L"Communication Success of Primary Device (%*100)",
		L"Communication Success of Secondary Device (%*100)",
		L"Write item remain count", 
		L"Total Communication Count (LOWORD)",
		L"Total Communication Count (HIWORD)",
		L"Total Communication Timeout (LOWORD)",
		L"Total Communication Timeout (HIWORD)",
		L"Total Communication Codebad (LOWORD)",
		L"Total Communication Codebad (HIWORD)",
		L"Read Communication Count (LOWORD)",
		L"Read Communication Count (HIWORD)",
		L"Read Communication Timeout (LOWORD)",
		L"Read Communication Timeout (HIWORD)",
		L"Read Communication Codebad (LOWORD)",
		L"Read Communication Codebad (HIWORD)",
		L"Bit write Communication Count (LOWORD)",
		L"Bit write Communication Count (HIWORD)",
		L"Bit write Communication Timeout (LOWORD)",
		L"Bit write Communication Timeout (HIWORD)",
		L"Bit write Communication Codebad (LOWORD)",
		L"Bit write Communication Codebad (HIWORD)",
		L"Word write Communication Count (LOWORD)",
		L"Word write Communication Count (HIWORD)",
		L"Word write Communication Timeout (LOWORD)",
		L"Word write Communication Timeout (HIWORD)",
		L"Word write Communication Codebad (LOWORD)",
		L"Word write Communication Codebad (HIWORD)",
	};

	if(pos < MAX_SYSTEM_DES) {
		SetTextColor(hdc, DARK_GRAY_COLOR);
		TextOut(hdc, cxChar*33, y, des[pos], wcslen(des[pos]));
	}
}

//------------------------------------------------------------------------------
//	WM_PAINT 메세지
//------------------------------------------------------------------------------

static void DrawOnlyBuf(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(wk->cViewVarType == 0) {
		DrawOnlyBufWORD(hdc, wk, pos);
	}
	else if(wk->cViewVarType == 1) {
		DrawOnlyBufFLOAT(hdc, wk, pos);
	}
	else if(wk->cViewVarType == 2) {
		DrawOnlyBufDWORD(hdc, wk, pos);
	}
	else if(wk->cViewVarType == 3) {
		DrawOnlyBufSTRING(hdc, wk, pos);
	}
	else if(wk->cViewVarType == 4) {
		DrawOnlyBufDOUBLE(hdc, wk, pos);
	}
	else if(wk->cViewVarType == 5) {
		DrawOnlyBufINT64(hdc, wk, pos);
	}
	else {
		DrawOnlyBufSYSTEM(hdc, wk, pos);
	}
} 

int SeekMatch(LOCAL_PORT_STRUCT *st, int pos, char var_type);

static int GetBufSize(WORK_SCAN_DATA *wk)
{
	if(wk->nPortNum >= nPortHap)	return 0;

	if(wk->cViewVarType == 0)		return portBuf[wk->nPortNum].local.nBufSizeWORD;
	else if(wk->cViewVarType == 1)	return portBuf[wk->nPortNum].local.nBufSizeFLOAT;
	else if(wk->cViewVarType == 2)	return portBuf[wk->nPortNum].local.nBufSizeDWORD;
	else if(wk->cViewVarType == 3)	return portBuf[wk->nPortNum].local.nBufSizeSTRING;
	else if(wk->cViewVarType == 4)	return portBuf[wk->nPortNum].local.nBufSizeDOUBLE;
	else if(wk->cViewVarType == 5)	return portBuf[wk->nPortNum].local.nBufSizeINT64;
	else;							return MAX_ATTR_WORD;
}

static void WmPaint(HWND hwnd)
{
	PAINTSTRUCT ps;
	HDC hdc;
	int y;
	HGLOBAL     hGlobal ;
	WORK_SCAN_DATA		 *wk;
	RECT rect;
	int i;
	TCHAR buf[80];

	hGlobal = (HGLOBAL) GetWindowLong(hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	hdc = BeginPaint(hwnd, &ps);

	gcls(hdc, &ps.rcPaint, CAT_COLOR_BACK);

	if(nPortHap == 0) {
		GetClientRect(hwnd, &rect);
		SetTextColor(hdc, CAT_COLOR_TEXT);

		wcscpy(buf, L"no scan file.");
		DrawText(hdc, buf, wcslen(buf), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
	}
	else {
		GLOBAL_PORT_STRUCT *pt = &portBuf[wk->nPortNum];

		GetClientRect(hwnd, &rect);
		y = 0;

		if(pt->bActiveFlag == 0) {
			CString text;
			if(IsLangKorean()) {
				text = "이 포트는 사용안함으로 설정되어 있습니다.";
			} else {
				text = "This port defined NOT USED.";
			}
			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
			TextOut(hdc, 0, 0, text, wcslen(text));
		}
		else if(pt->bDeviceInitialFlag == 0) {
			CString text;
			if(IsLangKorean()) {
				text = "디바이스 초기화 중입니다...";
			} else {
				text = "Waiting Device Initial...";
			}
			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
			TextOut(hdc, 0, 0, text, wcslen(text));
		}
		else if(pt->nScanProtocol == 0) {
			CString text;
			if(IsLangKorean()) {
				text = "Protocol 미 설정 상태이므로 통신을 할 수 없습니다.";
			} else {
				text = "Unable communication decause Protocol not defined.";
			}
			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
			TextOut(hdc, 0, 0, text, wcslen(text));
		}
		else if(GetBufSize(wk) == 0) {
			CString text;
			if(IsLangKorean()) {
				if(wk->cViewVarType == 0)		text = "WORD 메모리 크기 = 0";
				else if(wk->cViewVarType == 1)	text = "FLOAT 메모리 크기 = 0";
				else if(wk->cViewVarType == 2)  text = "DWORD 메모리 크기 = 0";
				else if(wk->cViewVarType == 3)  text = "STRING 메모리 크기 = 0";
				else if(wk->cViewVarType == 4)	text = "DOUBLE 메모리 크기 = 0";
				else if(wk->cViewVarType == 5)  text = "INT64 메모리 크기 = 0";
				else							text = "SYSTEM 메모리 크기 = 0";
			}else {
				if(wk->cViewVarType == 0)		text =  "WORD Memory Size = 0";
				else if(wk->cViewVarType == 1)	text = "FLOAT Memory Size = 0";
				else if(wk->cViewVarType == 2)	text = "DWORD Memory Size = 0";
				else if(wk->cViewVarType == 3)	text = "STRING Memory Size = 0";
				else if(wk->cViewVarType == 4)	text =  "DOUBLE Memory Size = 0";
				else if(wk->cViewVarType == 5)	text = "INT64 Memory Size = 0";
				else							text = "SYSTEM Memory Size = 0";
			}

			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
			DrawText(hdc, text, wcslen(text), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
		}
		else {
			SCAN_METHOD_STRUCT *sm;

			int nBufSize = GetBufSize(wk);

			for(i = wk->nScrollVerPos; y < rect.bottom && i < nBufSize; i++, y+=cyChar) {
				DrawOnlyBuf(hdc, wk, i);
			}

			for(i = 0; i < pt->local.nScanMethodHap; i++) {
				sm = &pt->local.scanMethod[i];

				if(wk->cViewVarType != sm->cVarType)	continue;
				if(sm->target > wk->nScrollVerPos+wk->ylimit)	continue;

				if(sm->active)
					SetTextColor(hdc, CAT_COLOR_TEXT);
				else
					SetTextColor(hdc, DARK_GRAY_COLOR);

				if(wk->cViewVarType == 1)	// float
					PlcProtocolDrawMethod(hdc, cxChar*44, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else if(wk->cViewVarType == 2)	// DWORD
					PlcProtocolDrawMethod(hdc, cxChar*40, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else if(wk->cViewVarType == 4)	// DOUBLE
					PlcProtocolDrawMethod(hdc, cxChar*44, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else if(wk->cViewVarType == 5)	// INT64
					PlcProtocolDrawMethod(hdc, cxChar*40, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else
					PlcProtocolDrawMethod(hdc, cxChar*34, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
			}

			if(pt->bScanPause) {
				CString text;
				
				if(IsLangKorean()) {
					text = "포트 일시중지(PAUSE) 상태 (#DO# 0028 ON 상태)";
				}
				else {
					text = "Port is PAUSE status";
				}

				SetTextColor(hdc, RGB(255, 0, 0));
				SetBkColor(hdc, CAT_COLOR_BACK);
				DrawText(hdc, text, wcslen(text), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
			}
		}
	}
	EndPaint(hwnd, &ps);

	GlobalUnlock (hGlobal);
}

static void ScrollUpdate(HWND hwnd, WORK_SCAN_DATA *wk)
{
	RECT rect;

	GetClientRect(hwnd, &rect);

	wk->nScrollVerHap = GetBufSize(wk);

	wk->ylimit = rect.bottom/cyChar;
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
	GetTextMetrics(hdc, &tm);
	ReleaseDC(hwnd, hdc);

	cxChar = tm.tmAveCharWidth;
	cyChar = tm.tmHeight+tm.tmExternalLeading;

	GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	wk->nScrollVerHap = GetBufSize(wk);
	
	wk->ylimit = rect.bottom/cyChar;
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
				//ScrollWindow(hwnd, 0, cyChar, &rect, &rect);
				rect.bottom = cyChar;
				InvalidateRect(hwnd, &rect, TRUE);
				break;
	  case SB_LINEDOWN:
				if(wk->nScrollVerPos >= wk->nScrollVerHap-1)	break;
				wk->nScrollVerPos++;
				//ScrollWindow(hwnd, 0, -cyChar, &rect, &rect);
				rect.top = rect.bottom-cyChar;
				InvalidateRect(hwnd, &rect, TRUE);
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

static void WmKeyDown(HWND hwnd, WPARAM wParam)
{
	switch(wParam) {
		case VK_ESCAPE:
			SendMessage(GetParent(hwnd), WM_COMMAND, IDC_SCANBUF_BUTTON_CLOSE, 0L);
			break;
		case VK_LEFT:
			break;
		case VK_RIGHT:
			break;
		case VK_UP:
			SendMessage(hwnd, WM_VSCROLL, SB_LINEUP, 0L);	break;
		case VK_DOWN:
			SendMessage(hwnd, WM_VSCROLL, SB_LINEDOWN, 0L);	break;
		case VK_PRIOR:
			SendMessage(hwnd, WM_VSCROLL, SB_PAGEUP, 0L);	break;
		case VK_NEXT:
			SendMessage(hwnd, WM_VSCROLL, SB_PAGEDOWN, 0L);	break;
	}
}

//---------------------------------------------------------------------------
//	윈도우 타이틀을 만든다.
//---------------------------------------------------------------------------

static void SetScanBufTitle(HWND hwnd, int port, int memory_type)
{
	TCHAR buf[160];
	TCHAR imsi[80];

	if(IsLangKorean()) {
		swprintf(buf, L"통신 메모리 ");
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		GetResourceString(IDS_ScanMemory, buf, sizeof(buf));	
		wcscat(buf, L" ");
	}
	else {
		swprintf(buf, L"Memory ");
	}

	if(memory_type == 0) {
		wcscat(buf, L"WORD");
	}
	else if(memory_type == 1) {
		wcscat(buf, L"FLOAT");
	}
	else if(memory_type == 2) {
		wcscat(buf, L"DWORD");
	}
	else if(memory_type == 3) {
		wcscat(buf, L"STRING");
	}
	else if(memory_type == 4) {
		wcscat(buf, L"DOUBLE");
	}
	else if(memory_type == 5) {
		wcscat(buf, L"INT64");
	}
	else {
		wcscat(buf, L"SYSTEM");
	}

	swprintf(imsi, L":Port %d", port);

	wcscat(buf, imsi);
	
	if(portBuf[port].sTitle[0] != 0) {
		wcscat(buf, L" (");
		wcscat(buf, portBuf[port].sTitle);
		wcscat(buf, L")");
	}

	TCHAR device[80];

	PlcDeviceGetInfoString(&portBuf[port].local.device, device);

	wcscat(buf, L" ");
	wcscat(buf, device);

	SetWindowText(hwnd, buf);
}

static COMM_COUNT_STRUCT *GetCountStruct(WORK_SCAN_DATA *wk, int type)
{
	if(type == 0)		return &portBuf[wk->nPortNum].countAll.Total;
	else if(type == 1)	return &portBuf[wk->nPortNum].countAll.Read;
	else if(type == 2)	return &portBuf[wk->nPortNum].countAll.WriteBit;
	else				return &portBuf[wk->nPortNum].countAll.WriteWord;
}

static void OnTimer(HWND hwnd, WORK_SCAN_DATA *wk)
{
	if(nPortHap == 0)	return;

	GLOBAL_PORT_STRUCT *pt;		

	pt = &portBuf[wk->nPortNum];

	if(pt->bInvalidateScreen) {
		pt->bInvalidateScreen = 0;
		InvalidateRect(hwnd, NULL, TRUE);
	}

	if(wk->nScanDevice != pt->nScanDevice) {
		wk->nScanDevice = pt->nScanDevice;
		SetScanBufTitle(GetParent(hwnd), wk->nPortNum, wk->cViewVarType);
	}

	int i, j;
	COMM_COUNT_STRUCT *count;

	for(i = 0; i < 4; i++) {
		count = GetCountStruct(wk, i);
		if(wk->lCountTotal[i] !=  count->lCountCommTry) {
			wk->lCountTotal[i] =  count->lCountCommTry;
			PostMessage(GetParent(hwnd), WM_COMMAND, IDC_CHANGE_COUNT_TOTAL, i);
		}
		if(wk->lCountTimeOut[i] != count->lCountTimeOut) {
			wk->lCountTimeOut[i] = count->lCountTimeOut;
			PostMessage(GetParent(hwnd), WM_COMMAND, IDC_CHANGE_COUNT_TIMEOUT, i);
		}
		if(wk->lCountCodeBad[i] != count->lCountCodeBad) {
			wk->lCountCodeBad[i] = count->lCountCodeBad;
			PostMessage(GetParent(hwnd), WM_COMMAND, IDC_CHANGE_COUNT_CODEBAD, i);
		}
		if(wk->wSuccessPercent[i] != count->wSuccessPercent) {
			wk->wSuccessPercent[i] = count->wSuccessPercent;
			PostMessage(GetParent(hwnd), WM_COMMAND, IDC_CHANGE_SUCCESS_PERCENT, i);
		}
	}

	if(pt->bActiveFlag == 0)	return;

	HDC hdc;
				
	hdc = GetDC(hwnd);
	if(wk->cViewVarType == 0) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeWORD; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufWORD[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufWORD[i].flag ) {
				wk->saveBuf[j].value  = pt->local.bufWORD[i].value;
				wk->saveBuf[j].flag   = pt->local.bufWORD[i].flag;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	else if(wk->cViewVarType == 1) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeFLOAT; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufFLOAT[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufFLOAT[i].flag ) {
				wk->saveBuf[j].value  = pt->local.bufFLOAT[i].value;
				wk->saveBuf[j].flag   = pt->local.bufFLOAT[i].flag;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	else if(wk->cViewVarType == 2) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeDWORD; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufDWORD[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufDWORD[i].flag) {
				wk->saveBuf[j].value  = pt->local.bufDWORD[i].value;
				wk->saveBuf[j].flag   = pt->local.bufDWORD[i].flag;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	else if(wk->cViewVarType == 3) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeSTRING; i++, j++) {
			if( strcmp(wk->saveBufString[j].value, pt->local.bufSTRING[i].value) != 0 ||
				wk->saveBufString[j].flag != pt->local.bufSTRING[i].flag) {
				strcpy(wk->saveBufString[j].value, pt->local.bufSTRING[i].value);
				wk->saveBufString[j].flag = pt->local.bufSTRING[i].flag;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	else if(wk->cViewVarType == 4) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeDOUBLE; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufDOUBLE[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufDOUBLE[i].flag ) {
				wk->saveBuf[j].value  = pt->local.bufDOUBLE[i].value;
				wk->saveBuf[j].flag   = pt->local.bufDOUBLE[i].flag;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	else if(wk->cViewVarType == 5) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeINT64; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufINT64[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufINT64[i].flag) {
				wk->saveBuf[j].value  = (double)pt->local.bufINT64[i].value;
				wk->saveBuf[j].flag   = pt->local.bufINT64[i].flag;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	else  {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < MAX_ATTR_WORD; i++, j++) {
			if( wk->saveBuf[j].value != pt->bufSYSTEM[i].value) {
				wk->saveBuf[j].value  = pt->bufSYSTEM[i].value;
				DrawOnlyBuf(hdc, wk, i);
			}
		}
	}
	ReleaseDC(hwnd, hdc);

	static BYTE bOldScanPause;

	if(bOldScanPause != pt->bScanPause) {
		bOldScanPause = pt->bScanPause;
		InvalidateRect(hwnd, NULL, TRUE);
	}
}

static void ChangeMemoryType(HWND hwnd, WORK_SCAN_DATA *wk, char type)
{
	wk->cViewVarType = type;

	SetScanBufTitle(GetParent(hwnd), wk->nPortNum, wk->cViewVarType);
	wk->nScrollVerPos = 0;
	ScrollUpdate(hwnd, wk);
	InvalidateRect(hwnd, NULL, TRUE);
	InvalidateRect(GetParent(hwnd), NULL, TRUE);
}

static void WmCommand(HWND hwnd, WPARAM wParam)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	switch(wParam) {
		case IDC_SCANBUF_BUTTON_MINUS:
			if(nPortHap <= 1)	break;
			if(wk->nPortNum <= 0)	wk->nPortNum = nPortHap-1;
			else					wk->nPortNum --;

			SetScanBufTitle(GetParent(hwnd), wk->nPortNum, wk->cViewVarType);
			wk->nScrollVerPos = 0;
			ScrollUpdate(hwnd, wk);
			InvalidateRect(hwnd, NULL, TRUE);
			InvalidateRect(GetParent(hwnd), NULL, TRUE);
			break;
		case IDC_SCANBUF_BUTTON_PLUS:
			if(nPortHap <= 1)	break;
			if(wk->nPortNum >= nPortHap-1)	wk->nPortNum = 0;
			else							wk->nPortNum ++;

			SetScanBufTitle(GetParent(hwnd), wk->nPortNum, wk->cViewVarType);
			wk->nScrollVerPos = 0;
			ScrollUpdate(hwnd, wk);
			InvalidateRect(hwnd, NULL, TRUE);
			InvalidateRect(GetParent(hwnd), NULL, TRUE);
			break;

		case IDC_SCANBUF_BUTTON_VAR_TYPE:
			wk->cViewVarType++;
			wk->cViewVarType %= 7;

			SetScanBufTitle(GetParent(hwnd), wk->nPortNum, wk->cViewVarType);
			wk->nScrollVerPos = 0;
			ScrollUpdate(hwnd, wk);
			InvalidateRect(hwnd, NULL, TRUE);
			InvalidateRect(GetParent(hwnd), NULL, TRUE);

			break;

		case IDC_SCANBUF_BUTTON_VAR_WORD:
			ChangeMemoryType(hwnd, wk, 0);
			break;
		case IDC_SCANBUF_BUTTON_VAR_FLOAT:
			ChangeMemoryType(hwnd, wk, 1);
			break;
		case IDC_SCANBUF_BUTTON_VAR_DWORD:
			ChangeMemoryType(hwnd, wk, 2);
			break;
		case IDC_SCANBUF_BUTTON_VAR_STRING:
			ChangeMemoryType(hwnd, wk, 3);
			break;
		case IDC_SCANBUF_BUTTON_VAR_DOUBLE:
			ChangeMemoryType(hwnd, wk, 4);
			break;
		case IDC_SCANBUF_BUTTON_VAR_INT64:
			ChangeMemoryType(hwnd, wk, 5);
			break;
		case IDC_SCANBUF_BUTTON_VAR_SYSTEM:
			ChangeMemoryType(hwnd, wk, 6);
			break;

		case IDM_EVENT_TIMER:
			OnTimer(hwnd, wk);
			break;
	}
	GlobalUnlock(hGlobal);
}

static void DrawCountTotal(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal;
	WORK_SCAN_DATA 	*wk;
	TCHAR buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*12;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	count = GetCountStruct(wk, type);
	
	if(IsLangKorean()) 
	{
		swprintf(buf, L"통신횟수-%-10ld", count->lCountCommTry);
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_CommCount, imsi);	
		swprintf(buf, L"%s-%-10ld", imsi, count->lCountCommTry);
	}
	else {
		swprintf(buf, L"Comm Try-%-10ld", count->lCountCommTry);
	}
	TextOut(hdc, x, y, buf, wcslen(buf));
	GlobalUnlock(hGlobal);
}

static void DrawCountTimeOut(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	TCHAR buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*34;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	count = GetCountStruct(wk, type);
	if(IsLangKorean()) {
		swprintf(buf, L"시간초과-%-10ld", count->lCountTimeOut);
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_TimeOut, imsi);	
		swprintf(buf, L"%s-%-10ld", imsi, count->lCountTimeOut);
	}
	else {
		swprintf(buf, L"Time Out-%-10ld", count->lCountTimeOut);
	}
	TextOut(hdc, x, y, buf, wcslen(buf));
	GlobalUnlock(hGlobal);
}

static void DrawCountCodeBad(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	TCHAR buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*56;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);
	count = GetCountStruct(wk, type);
	if(IsLangKorean()) {
		swprintf(buf, L"코드불량-%-10ld", count->lCountCodeBad);
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_CodeBad, imsi);
		swprintf(buf, L"%s-%-10ld", imsi, count->lCountCodeBad);
	}
	else {
		swprintf(buf, L"Code Bad-%-10ld", count->lCountCodeBad);
	}
	TextOut(hdc, x, y, buf, wcslen(buf));
	GlobalUnlock(hGlobal);
}

static void DrawSuccessPercent(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	TCHAR buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*78;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);
	count = GetCountStruct(wk, type);
	if(IsLangKorean()) {
		swprintf(buf, L"성공률-%.2f%%    ",  count->wSuccessPercent/100.0);
	}
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_SuccessPercent, imsi);	
		swprintf(buf, L"%s-%.2f%%    ",  imsi, count->wSuccessPercent/100.0);
	}
	else {
		swprintf(buf, L"Success-%.2f%%    ", count->wSuccessPercent/100.0);
	}
	TextOut(hdc, x, y, buf, wcslen(buf));
	GlobalUnlock(hGlobal);
}

long FAR PASCAL EXPORT WndProcViewScanBufMemoryChild (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL     hGlobal;
	WORK_SCAN_DATA		 *wk;
	int i;

	switch (message) {
		
		case WM_CREATE:
			// Allocate memory for window private data
			
			hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof (WORK_SCAN_DATA)) ;
			SetWindowLong(hwnd, 0, (LONG)hGlobal) ;

			if(hGlobal == NULL) {
				MsgBoxLocalMemoryLow(hwnd, L"Local Alloc");
				DestroyWindow(GetParent(hwnd));
				return 0;
			}

			wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;
			wk->cViewVarType = nSaveViewVar;	// WORD view
			wk->bScrollVer = ON;
			wk->nScrollVerPos = 0;
			wk->nScrollVerHap = 0;
			wk->nPortNum = nSaveViewPort;
			wk->nPortNum %= nPortHap;
			wk->ylimit = 5;
			for(i = 0; i < 4; i++) {
				wk->lCountTotal[i] = 0L;
				wk->lCountTimeOut[i] = 0L;
				wk->lCountCodeBad[i] = 0L;
			}

			SetScanBufTitle(GetParent(hwnd), wk->nPortNum, wk->cViewVarType);

			GlobalUnlock (hGlobal);
			
			return 0 ;/*
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
			WmCommand(hwnd, wParam);
			return 0;
		case WM_DESTROY:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0) ;
			if(hGlobal != NULL) {
				wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal);
				nSaveViewPort = wk->nPortNum;
				nSaveViewVar  = wk->cViewVarType;
				GlobalUnlock(hGlobal);
				GlobalFree  (hGlobal);
			}
			return 0 ;*/
	}
	// Pass uwkrocessed message to DefMDIChildProc
	return DefWindowProc (hwnd, message, wParam, lParam) ;
}

typedef struct {
	HWND hwndChild;
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

	MoveWindow(wk->hwndChild, 0, cyChar+10,   rect.right, y-((cyChar+10)*2+needsize)-cyChar*3, TRUE);
	wk->menuButton.Move(0, y-needsize);
}

static void DrawMethodTitle(HWND hwnd, HDC hdc, int x, int y)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);
	PlcProtocolDrawMethodTitle(&portBuf[wk->nPortNum], hdc, x, y);
	GlobalUnlock(hGlobal);
}

long FAR PASCAL EXPORT WndProcViewScanBufMemory (HWND hwnd, UINT message, UINT wParam, LONG lParam)
{
	HGLOBAL hGlobal;
	HGLOBAL hGlobalChild;
	GLOBAL_SCANBUF_FRAME *wk;
	WORK_SCAN_DATA *workChild;
	HDC hdc;
	TEXTMETRIC tm;
	PAINTSTRUCT ps;
	RECT rect;
	HWND hwndChild;
	CString buf;
	int i;

	switch (message) {
		
		case WM_CREATE:
			hwndScanBufMemoryViewChild = hwnd;	// 통신 메모리보기 윈도우.

			hdc = GetDC(hwnd);
			GetTextMetrics(hdc, &tm);
			ReleaseDC(hwnd, hdc);

			cxChar = tm.tmAveCharWidth;
			cyChar = tm.tmHeight+tm.tmExternalLeading;

			hGlobal = GlobalAlloc (GMEM_MOVEABLE | GMEM_ZEROINIT, sizeof (GLOBAL_SCANBUF_FRAME));
			SetWindowLong(hwnd, 0, (LONG)hGlobal);

			if(hGlobal == NULL) {
				MsgBoxLocalMemoryLow(hwnd, L"hGraphicData Alloc");
				DestroyWindow(hwnd);
				return 0;
			}

			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;

			
			wk->hwndChild = CreateWindow(szViewScanBufMemoryChildClass, L"",
				  WS_CHILD | WS_VSCROLL,
				  0, 0,
				  200, 200,
				  hwnd, NULL, hInst, NULL) ;
			ShowWindow(wk->hwndChild, SW_SHOW);

			wk->menuButton.Init(hwnd, ((LPCREATESTRUCT) lParam) -> hInstance);
			if(IsLangKorean()) {
				wk->menuButton.Insert(L"감시 프로그램", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert(L"닫기[ESC]", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert(L"이전 포트",   IDC_SCANBUF_BUTTON_MINUS);
				wk->menuButton.Insert(L"다음 포트",   IDC_SCANBUF_BUTTON_PLUS);
				wk->menuButton.Insert(L"다음 메모리", IDC_SCANBUF_BUTTON_VAR_TYPE);
			} 
			else if(IsLangChinese() || IsLangJapanese()) 
			{
				CString imsi;
				GetResourceString(IDS_ViewMainProgram, buf);
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_VIEWMAIN);
				GetResourceString(IDS_Close, buf);
				imsi.Format(L"%s[ESC]", buf);
				wk->menuButton.Insert(imsi, IDC_SCANBUF_BUTTON_CLOSE);
				GetResourceString(IDS_PrevPort, buf);
				wk->menuButton.Insert(buf,   IDC_SCANBUF_BUTTON_MINUS);
				GetResourceString(IDS_NextPort, buf);
				wk->menuButton.Insert(buf,   IDC_SCANBUF_BUTTON_PLUS);
				wk->menuButton.Insert(L"Next Memory", IDC_SCANBUF_BUTTON_VAR_TYPE);	
			}
			else {
				wk->menuButton.Insert(L"ViewMain", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert(L"Close[ESC]", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert(L"Prev Port",   IDC_SCANBUF_BUTTON_MINUS);
				wk->menuButton.Insert(L"Next Port",   IDC_SCANBUF_BUTTON_PLUS);
				wk->menuButton.Insert(L"Next Memory",   IDC_SCANBUF_BUTTON_VAR_TYPE);
			}
			wk->menuButton.Insert(L"Word",   IDC_SCANBUF_BUTTON_VAR_WORD);
			wk->menuButton.Insert(L"Float",   IDC_SCANBUF_BUTTON_VAR_FLOAT);
			wk->menuButton.Insert(L"Dword",   IDC_SCANBUF_BUTTON_VAR_DWORD);
			wk->menuButton.Insert(L"String",   IDC_SCANBUF_BUTTON_VAR_STRING);
			wk->menuButton.Insert(L"Double",   IDC_SCANBUF_BUTTON_VAR_DOUBLE);
			wk->menuButton.Insert(L"Int64",   IDC_SCANBUF_BUTTON_VAR_INT64);
			wk->menuButton.Insert(L"System",   IDC_SCANBUF_BUTTON_VAR_SYSTEM);

			if(nPortHap <= 1) {
				wk->menuButton.Enable(IDC_SCANBUF_BUTTON_MINUS, FALSE);
				wk->menuButton.Enable(IDC_SCANBUF_BUTTON_PLUS, FALSE);
			}

			SetFocus(wk->hwndChild);
			GlobalUnlock(hGlobal);
			return 0 ;/*
		case WM_SIZE:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
			SizeRebuild(hwnd, wk);
			GlobalUnlock(hGlobal);
			break;*/
		case WM_PAINT:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;

			GetClientRect(hwnd, &rect);
			hdc = BeginPaint(hwnd, &ps);

			wk->nStatusBoxY = rect.bottom-(wk->menuButton.GetHeight()+cyChar+10)-cyChar*3;

			PopBox2(hdc, 0, wk->nStatusBoxY, rect.right, rect.bottom, WHITE_GRAY_COLOR);

			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			if(IsLangKorean()) {
				TextOut(hdc, 3, wk->nStatusBoxY+3,          L"전체통신", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar,   L"읽기통신", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*2, L"비트쓰기", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*3, L"워드쓰기", 8); 
			} 
			else if(IsLangChinese() || IsLangJapanese()) 
			{
				GetResourceString(IDS_TotalComm, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3,          buf, wcslen(buf)); 
				GetResourceString(IDS_ReadComm, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar,   buf, wcslen(buf)); 
				GetResourceString(IDS_BitWrite, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*2, buf, wcslen(buf)); 
				GetResourceString(IDS_WordWrite, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*3, buf, wcslen(buf)); 
			}
			else {
				TextOut(hdc, 3, wk->nStatusBoxY+3,          L"Total", 5);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar,   L"ReadComm", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*2, L"BitWrite", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*3, L"WordWrite", 9); 
			}

			for(i = 0; i < 4; i++) {
				//DrawCountTotal  (wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
				//DrawCountTimeOut(wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
				//DrawCountCodeBad(wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
				//DrawSuccessPercent(wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
			}

			PopBox2(hdc, 0, 0, rect.right, cyChar+10, WHITE_GRAY_COLOR);
			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			/*			
			hGlobalChild = (HGLOBAL)GetWindowLong(wk->hwndChild, 0);
			workChild = (WORK_SCAN_DATA*) GlobalLock (hGlobalChild);
			
			if(workChild->cViewVarType == 1)	{ // float view
				buf.Format(L"No");
				TextOut(hdc, cxChar/2,  5, buf, wcslen(buf));
				buf.Format(L"Float");
				TextOut(hdc, cxChar*24,  5, buf, wcslen(buf));
				buf.Format(L"Exponential");
				TextOut(hdc, cxChar*32, 5, buf, wcslen(buf));

				DrawMethodTitle(wk->hwndChild, hdc, cxChar*45, 5);
			}
			else if(workChild->cViewVarType == 2)	{ // DWORD
				TextOut(hdc, cxChar/2,  5, L"No", 2);
				TextOut(hdc, cxChar*12,  5, L"Dec", 3);
				TextOut(hdc, cxChar*20, 5, L"Hex", 3);
				TextOut(hdc, cxChar*34, 5, L"Long", 4);
				DrawMethodTitle(wk->hwndChild, hdc, cxChar*40, 5);
			}
			else if(workChild->cViewVarType == 3)	{ // STRING
				TextOut(hdc, cxChar/2,  5, L"No", 2);
				TextOut(hdc, cxChar*15,  5, L"String", 6);
				DrawMethodTitle(wk->hwndChild, hdc, cxChar*34, 5);
			}
			else if(workChild->cViewVarType == 4)	{ // double view
				buf.Format(L"No");
				TextOut(hdc, cxChar/2,  5, buf, wcslen(buf));
				buf.Format(L"Double");
				TextOut(hdc, cxChar*24,  5, buf, wcslen(buf));
				buf.Format(L"Exponential");
				TextOut(hdc, cxChar*32, 5, buf, wcslen(buf));
				//TextOut(hdc, cxChar*15, 5, "Binary", 6);
				DrawMethodTitle(wk->hwndChild, hdc, cxChar*45, 5);
			}
			else if(workChild->cViewVarType == 5)	{ // INT64
				TextOut(hdc, cxChar/2,  5, L"No", 2);
				TextOut(hdc, cxChar*20,  5, L"Dec", 3);
				TextOut(hdc, cxChar*34, 5, L"Hex", 3);
				TextOut(hdc, cxChar*50, 5, L"INT64", 5);
				DrawMethodTitle(wk->hwndChild, hdc, cxChar*64, 5);
			}
			else if(workChild->cViewVarType == 6) { // SYSTEM
				TextOut(hdc, cxChar/2,  5, L"No", 2);
				TextOut(hdc, cxChar*4,  5, L"Dec", 3);
				TextOut(hdc, cxChar*10, 5, L"Hex", 3);
				TextOut(hdc, cxChar*15, 5, L"Binary", 6);
				buf = "Description";
				TextOut(hdc, cxChar*33, 5, buf, wcslen(buf));
			}
			else {
				TextOut(hdc, cxChar/2,  5, L"No", 2);
				TextOut(hdc, cxChar*4,  5, L"Dec", 3);
				TextOut(hdc, cxChar*10, 5, L"Hex", 3);
				TextOut(hdc, cxChar*15, 5, L"Binary", 6);
				DrawMethodTitle(wk->hwndChild, hdc, cxChar*34, 5);
			}

			GlobalUnlock(hGlobalChild);*/

			EndPaint(hwnd, &ps);
			GlobalUnlock(hGlobal);
			return 0;/*
		case WM_COMMAND:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal) ;
			switch(wParam) {
				case IDC_SCANBUF_BUTTON_VIEWMAIN:
							SendMessage(hwndMainFrame, WM_COMMAND, IDM_VIEW_VIEWMAIN, 0L);
                     break;
				case IDC_SCANBUF_BUTTON_CLOSE:
							//SendMessage(hwndMainClient, WM_MDIDESTROY, (WPARAM)hwnd, 0L);
							break;
				case IDC_CHANGE_COUNT_TOTAL:
							hdc = GetDC(hwnd);
							DrawCountTotal(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				case IDC_CHANGE_COUNT_TIMEOUT:
							hdc = GetDC(hwnd);
							DrawCountTimeOut(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				case IDC_CHANGE_COUNT_CODEBAD:
							hdc = GetDC(hwnd);
							DrawCountCodeBad(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				case IDC_CHANGE_SUCCESS_PERCENT:
							hdc = GetDC(hwnd); 
							DrawSuccessPercent(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				default:	
							hwndChild = wk->hwndChild;
							GlobalUnlock(hGlobal);
							return SendMessage(hwndChild, WM_COMMAND, wParam, lParam);
			}
			GlobalUnlock(hGlobal);
			return 0;
		//case WM_MDIACTIVATE:
		//	return 0 ;
		case WM_SETFOCUS:
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			wk = (GLOBAL_SCANBUF_FRAME*) GlobalLock (hGlobal);
			SetFocus(wk->hwndChild);
			GlobalUnlock(hGlobal);
			return 0;
		case WM_DESTROY:
	      	hwndScanBufMemoryViewChild = NULL;	// 통신 메모리보기 윈도우.
			hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
			if(hGlobal != NULL) {
				GlobalFree (hGlobal) ;
			}
			return 0;*/
	}
	// Pass unprocessed message to DefMDIChildProc
	return DefWindowProc (hwnd, message, wParam, lParam) ;
}