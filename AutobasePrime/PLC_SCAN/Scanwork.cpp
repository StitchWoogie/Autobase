
#include "stdafx.h"
#include <stdio.h>
#include <string.h>

#include <tools.h>
#include <glib.h>
#include <menubutn.h>

#include "plc_scan.h"
#include "resource.h"

#include "..\catlib.src\totalcfg.h"

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

extern	char szViewScanBufMemoryChildClass [];
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

		int	   nScrollHorPos;
		int	   nScrollHorHap;
		char   bScrollHor;
		int	   xlimit;					// 한 화면에 보일수 있는 X라인수
} WORK_SCAN_DATA;

static int	cxChar, cyChar;
int  nSaveViewPort = 0;
static int  nSaveViewVar = 0;

//------------------------------------------------------------------------------
//	WORD memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufWORD(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeWORD)	{
		bell(100);
		return;
	}

	char buf[80];

	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkMode(hdc, TRANSPARENT);
	
	gcls(hdc, x, y, x+cxChar*16, y+cyChar, CAT_COLOR_BACK);

	wsprintf(buf, "%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	if(portBuf[wk->nPortNum].local.bufWORD[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	wsprintf(buf, "%5u", portBuf[wk->nPortNum].local.bufWORD[pos].value);
	TextOut(hdc, x+cxChar*5, y, buf, strlen(buf));
	
	if(portBuf[wk->nPortNum].local.bufWORD[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);
	
	wsprintf(buf, "%04X", portBuf[wk->nPortNum].local.bufWORD[pos].value);
	TextOut(hdc, x+cxChar*11, y, buf, strlen(buf));

	gcls(hdc, x+cxChar*16, y, x+cxChar*16+cxChar*16, y+cyChar, DARK_COLOR);

	WORD value = portBuf[wk->nPortNum].local.bufWORD[pos].value;

	for(int j = 0; j < 16; j++) {
		sprintf(buf, "%X", 15-j);
		if((value >> (15-j)) & 1) {
			SetTextColor(hdc, RGB(255, 0, 0));
			TextOut(hdc, x+cxChar*16+cxChar*j, y, buf, 1);
		}
		else {
			SetTextColor(hdc, DARK_GRAY_COLOR);
			TextOut(hdc, x+cxChar*16+cxChar*j, y, buf, 1);
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

	CString buf;	//char buf[80];	  2.0E+74을 테스트 하는 경우가 있어서 CString으로 변경함. 2023-5-25. 실제 이 오류는 double테스트 도중에 났지만 예방차원에서 수정함.
	RECT r;

	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, x, y, x+cxChar*32-1, y+cyChar, CAT_COLOR_BACK);

	buf.Format("%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	buf.Format("%.4f", portBuf[wk->nPortNum].local.bufFLOAT[pos].value);

	if(portBuf[wk->nPortNum].local.bufFLOAT[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	r.left = x+cxChar*5;
	r.top  = y;
	r.right = x+cxChar*28;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufFLOAT[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	buf.Format("%6.3E", portBuf[wk->nPortNum].local.bufFLOAT[pos].value);
	r.left = x+cxChar*30;
	r.top  = y;
	r.right = x+cxChar*42;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);
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

	char buf[80];
	RECT r;

	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, x, y, x+cxChar*38-1, y+cyChar, CAT_COLOR_BACK);

	wsprintf(buf, "%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	if(portBuf[wk->nPortNum].local.bufDWORD[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	sprintf(buf, "%10lu", portBuf[wk->nPortNum].local.bufDWORD[pos].value);
	r.left = x+cxChar*5;
	r.top  = y;
	r.right = x+cxChar*16;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufDWORD[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	sprintf(buf, "%08X", portBuf[wk->nPortNum].local.bufDWORD[pos].value);
	r.left = x+cxChar*17;
	r.top  = y;
	r.right = x+cxChar*26;
	r.bottom = y+cyChar;

	TextOut(hdc, x+cxChar*17, y, buf, strlen(buf));

	if(portBuf[wk->nPortNum].local.bufDWORD[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	sprintf(buf, "%d", portBuf[wk->nPortNum].local.bufDWORD[pos].value);
	r.left = x+cxChar*27;
	r.top  = y;
	r.right = x+cxChar*38;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);
}


//------------------------------------------------------------------------------
//	STRING memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufSTRING(HDC hdc, WORK_SCAN_DATA *wk, int pos)
{
	if(pos < 0 || pos >= portBuf[wk->nPortNum].local.nBufSizeSTRING)	{
		return;
	}

	char buf[256];
	RECT r;
	STRING_BUF *str = &portBuf[wk->nPortNum].local.bufSTRING[pos];
	
	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, x, y, x+cxChar*32-1, y+cyChar, CAT_COLOR_BACK);

	sprintf(buf, "%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	if(str->flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	memcpy(buf, str->value, 255);
	buf[255] = 0;

	r.left   = x+cxChar*5;
	r.top    = y;
	r.right  = x+r.left+cxChar*256;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_LEFT);

	
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

	CString buf;	//char buf[80];	  2.0E+74을 테스트 하는 경우가 있어서 CString으로 변경함. 2023-5-25. E+74
	RECT r;

	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, x, y, x+cxChar*32-1, y+cyChar, CAT_COLOR_BACK);

	buf.Format("%03d", pos);//wsprintf(buf, "%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	buf.Format("%.4f", portBuf[wk->nPortNum].local.bufDOUBLE[pos].value);//sprintf(buf, "%.4f", portBuf[wk->nPortNum].local.bufDOUBLE[pos].value);

	if(portBuf[wk->nPortNum].local.bufDOUBLE[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	r.left = x+cxChar*5;
	r.top  = y;
	r.right = x+cxChar*28;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufDOUBLE[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	buf.Format("%6.3E", portBuf[wk->nPortNum].local.bufDOUBLE[pos].value);//sprintf(buf, "%6.3E", portBuf[wk->nPortNum].local.bufDOUBLE[pos].value);
	r.left = x+cxChar*30;
	r.top  = y;
	r.right = x+cxChar*42;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);
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

	char buf[80];
	RECT r;

	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkColor(hdc, CAT_COLOR_BACK);

	gcls(hdc, x, y, x+cxChar*63-1, y+cyChar, CAT_COLOR_BACK);

	wsprintf(buf, "%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	if(portBuf[wk->nPortNum].local.bufINT64[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	sprintf(buf, "%19I64u", portBuf[wk->nPortNum].local.bufINT64[pos].value);
	r.left = x+cxChar*5;
	r.top  = y;
	r.right = x+cxChar*25;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);

	if(portBuf[wk->nPortNum].local.bufINT64[pos].flag)
		SetTextColor(hdc, RGB(0, 255, 255));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	sprintf(buf, "%016I64X", portBuf[wk->nPortNum].local.bufINT64[pos].value);
	//r.left = cxChar*26;
	//r.top  = y;
	//r.right = cxChar*43;
	//r.bottom = y+cyChar;

	TextOut(hdc, x+cxChar*26, y, buf, strlen(buf));

	if(portBuf[wk->nPortNum].local.bufINT64[pos].flag)
		SetTextColor(hdc, RGB(255, 255, 0));
	else
		SetTextColor(hdc, DARK_GRAY_COLOR);

	sprintf(buf, "%I64d", portBuf[wk->nPortNum].local.bufINT64[pos].value);
	r.left = x+cxChar*43;
	r.top  = y;
	r.right = x+cxChar*63;
	r.bottom = y+cyChar;

	DrawText(hdc, buf, strlen(buf), &r, DT_RIGHT);
}

//------------------------------------------------------------------------------
//	SYSTEM memory 한줄을 그린다.
//------------------------------------------------------------------------------

static void DrawOnlyBufSYSTEM(HDC hdc, WORK_SCAN_DATA *wk, int pos, bool bWmPaint)
{
	if(pos < 0 || pos >= MAX_ATTR_WORD)	{
		bell(100);
		return;
	}

	char buf[80];

	int x = -wk->nScrollHorPos*cxChar;
	int y = (pos-wk->nScrollVerPos)*cyChar;

	SetTextColor(hdc, CAT_COLOR_TEXT);
	SetBkMode(hdc, TRANSPARENT);

	gcls(hdc, x, y, x+cxChar*15, y+cyChar, CAT_COLOR_BACK);

	wsprintf(buf, "%03d", pos);
	TextOut(hdc, x, y, buf, strlen(buf));

	SetTextColor(hdc, RGB(255, 255, 0));

	wsprintf(buf, "%5u", portBuf[wk->nPortNum].bufSYSTEM[pos].value);
	TextOut(hdc, x+cxChar*4, y, buf, strlen(buf));
	
	SetTextColor(hdc, RGB(0, 255, 255));
	
	wsprintf(buf, "%04X", portBuf[wk->nPortNum].bufSYSTEM[pos].value);
	TextOut(hdc, x+cxChar*10, y, buf, strlen(buf));

	gcls(hdc, x+cxChar*15, y, x+cxChar*15+cxChar*16, y+cyChar, DARK_COLOR);

	WORD value = portBuf[wk->nPortNum].bufSYSTEM[pos].value;

	for(int j = 0; j < 16; j++) {
		sprintf(buf, "%X", 15-j);
		if((value >> (15-j)) & 1) {
			SetTextColor(hdc, RGB(255, 0, 0));
			TextOut(hdc, x+cxChar*15+cxChar*j, y, buf, 1);
		}
		else {
			SetTextColor(hdc, DARK_GRAY_COLOR);
			TextOut(hdc, x+cxChar*15+cxChar*j, y, buf, 1);
		}
	}

#define MAX_SYSTEM_DES 34
	const char *des[MAX_SYSTEM_DES] = {
		"b0-TimeOut1, b1-TimeOut5, b2-TimeOut10, b3-TimeOut20, b4-TimeOut30 (ON=OK,OFF=Fail)",
		"b0-ModemConnected, b1-Auto Connection, bC-CTS, bD-DSR, bE-RING, bF-RLSD",
		"b0-Device no of line duplex (OFF=Primary,ON=Secondary),b1-active,b8-pause",
		"b0-TimeOut on write, b1-CodeBad on write (ON=Error, OFF=OK)",
		"PC Duplex(b0-Another pc active, b1-This pc master, b2-Another pc master, b3-this pc Timeout, b4-another pc timeout",
		"Communication Success (%*100)",
		"Communication Fail (%*100)",
		"Communication Success of Primary Device (%*100)",
		"Communication Success of Secondary Device (%*100)",
		"Write item remain count", 
		"Total Communication Count (LOWORD)",
		"Total Communication Count (HIWORD)",
		"Total Communication Timeout (LOWORD)",
		"Total Communication Timeout (HIWORD)",
		"Total Communication Codebad (LOWORD)",
		"Total Communication Codebad (HIWORD)",
		"Read Communication Count (LOWORD)",
		"Read Communication Count (HIWORD)",
		"Read Communication Timeout (LOWORD)",
		"Read Communication Timeout (HIWORD)",
		"Read Communication Codebad (LOWORD)",
		"Read Communication Codebad (HIWORD)",
		"Bit write Communication Count (LOWORD)",
		"Bit write Communication Count (HIWORD)",
		"Bit write Communication Timeout (LOWORD)",
		"Bit write Communication Timeout (HIWORD)",
		"Bit write Communication Codebad (LOWORD)",
		"Bit write Communication Codebad (HIWORD)",
		"Word write Communication Count (LOWORD)",
		"Word write Communication Count (HIWORD)",
		"Word write Communication Timeout (LOWORD)",
		"Word write Communication Timeout (HIWORD)",
		"Word write Communication Codebad (LOWORD)",
		"Word write Communication Codebad (HIWORD)",
	};

	if(bWmPaint) {	// WmPaint일때만 그려준다.  그렇지 않을 때도 그리면 부드러운 글자인 경우 글씨가 겹쳐보인다.
		if(pos < MAX_SYSTEM_DES) {
			SetTextColor(hdc, DARK_GRAY_COLOR);
			TextOut(hdc, x+cxChar*33, y, des[pos], strlen(des[pos]));
		}
	}
}

//------------------------------------------------------------------------------
//	WM_PAINT 메세지
//------------------------------------------------------------------------------

static void DrawOnlyBuf(HDC hdc, WORK_SCAN_DATA *wk, int pos, bool bWmPaint)
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
		DrawOnlyBufSYSTEM(hdc, wk, pos, bWmPaint);
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
	else							return MAX_ATTR_WORD;
}

static void WmPaint(HWND hwnd)
{
	PAINTSTRUCT ps;
	HDC hdc;
	int x, y;
	HGLOBAL     hGlobal ;
	WORK_SCAN_DATA		 *wk;
	RECT rect;
	int i;
	char buf[80];

	hGlobal = (HGLOBAL) GetWindowLong(hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	hdc = BeginPaint(hwnd, &ps);

	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);

	gcls(hdc, &ps.rcPaint, CAT_COLOR_BACK);

	if(nPortHap == 0) {
		GetClientRect(hwnd, &rect);
		SetTextColor(hdc, CAT_COLOR_TEXT);

		strcpy(buf, "no scan file.");
		DrawText(hdc, buf, strlen(buf), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
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
			TextOut(hdc, 0, 0, text, strlen(text));
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
			TextOut(hdc, 0, 0, text, strlen(text));
		}
		else if(pt->nScanProtocol == 0) {
			CString text;
			if(IsLangKorean()) {
				text = "Protocol 미 설정 상태이므로 통신을 할 수 없습니다.";
			} else {
				text = "Unable communication because Protocol not defined.";
			}
			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
			TextOut(hdc, 0, 0, text, strlen(text));
		}
		else if(GetBufSize(wk) == 0) {
			char text[80];
			if(IsLangKorean()) {
				if(wk->cViewVarType == 0)		strcpy(text, "WORD 메모리 크기 = 0");
				else if(wk->cViewVarType == 1)	strcpy(text, "FLOAT 메모리 크기 = 0");
				else if(wk->cViewVarType == 2)  strcpy(text, "DWORD 메모리 크기 = 0");
				else if(wk->cViewVarType == 3)  strcpy(text, "STRING 메모리 크기 = 0");
				else if(wk->cViewVarType == 4)	strcpy(text, "DOUBLE 메모리 크기 = 0");
				else if(wk->cViewVarType == 5)  strcpy(text, "INT64 메모리 크기 = 0");
				else							strcpy(text, "SYSTEM 메모리 크기 = 0");
			}else {
				if(wk->cViewVarType == 0)		strcpy(text, "WORD Memory Size = 0");
				else if(wk->cViewVarType == 1)	strcpy(text, "FLOAT Memory Size = 0");
				else if(wk->cViewVarType == 2)	strcpy(text, "DWORD Memory Size = 0");
				else if(wk->cViewVarType == 3)	strcpy(text, "STRING Memory Size = 0");
				else if(wk->cViewVarType == 4)	strcpy(text, "DOUBLE Memory Size = 0");
				else if(wk->cViewVarType == 5)	strcpy(text, "INT64 Memory Size = 0");
				else							strcpy(text, "SYSTEM Memory Size = 0");
			}

			SetTextColor(hdc, WHITE_COLOR);
			SetBkColor(hdc, CAT_COLOR_BACK);
			DrawText(hdc, text, strlen(text), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
		}
		else {
			SCAN_METHOD_STRUCT *sm;

			int nBufSize = GetBufSize(wk);

			for(i = wk->nScrollVerPos; y < rect.bottom && i < nBufSize; i++, y+=cyChar) {
				DrawOnlyBuf(hdc, wk, i, true);
			}

			x = -wk->nScrollHorPos*cxChar;

			for(i = 0; i < pt->local.nScanMethodHap; i++) {
				sm = &pt->local.scanMethod[i];

				if(wk->cViewVarType != sm->cVarType)	continue;
				if(sm->target > wk->nScrollVerPos+wk->ylimit)	continue;

				if(sm->active)
					SetTextColor(hdc, CAT_COLOR_TEXT);
				else
					SetTextColor(hdc, DARK_GRAY_COLOR);

				if(wk->cViewVarType == 1)	// float
					PlcProtocolDrawMethod(hdc, x+cxChar*44, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else if(wk->cViewVarType == 2)	// DWORD
					PlcProtocolDrawMethod(hdc, x+cxChar*40, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else if(wk->cViewVarType == 4)	// DOUBLE
					PlcProtocolDrawMethod(hdc, x+cxChar*44, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else if(wk->cViewVarType == 5)	// INT64
					PlcProtocolDrawMethod(hdc, x+cxChar*40, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
				else
					PlcProtocolDrawMethod(hdc, x+cxChar*34, (sm->target-wk->nScrollVerPos)*cyChar, pt, sm);
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
				DrawText(hdc, text, strlen(text), &rect, DT_CENTER | DT_VCENTER | DT_SINGLELINE);
			}
		}
	}
	EndPaint(hwnd, &ps);

	GlobalUnlock (hGlobal);
}

static void ScrollUpdate(HWND hwnd, WORK_SCAN_DATA *wk)
{
	static bool flag = false;

	if(flag)	return;	// 연속 스크롤 Update를 막는다.

	flag = true;

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

	wk->nScrollHorHap = 120;

	wk->xlimit = rect.right/cxChar;
	if(wk->xlimit <= 0)	wk->xlimit = 1;

	if(wk->nScrollHorHap > wk->xlimit) {
		if(wk->bScrollHor == OFF) {
			ShowScrollBar(hwnd, SB_HORZ, TRUE);
			wk->bScrollHor = ON;
		}
		if(wk->nScrollHorPos > wk->nScrollHorHap)
			wk->nScrollHorPos = wk->nScrollHorHap;
		if(wk->nScrollHorPos < 0) {
			bell();
		}
		SetScrollRange(hwnd, SB_HORZ, 0, wk->nScrollHorHap-1, FALSE);
		SetScrollPos(hwnd, SB_HORZ, wk->nScrollHorPos, TRUE);
	}
	else {
		if(wk->bScrollHor == ON) {
			ShowScrollBar(hwnd, SB_HORZ, FALSE);
			wk->bScrollHor = OFF;
		}
		wk->nScrollHorPos = 0;
		wk->nScrollHorHap = 0;
	}

	flag = false;
}

//------------------------------------------------------------------------------
//	PCL WM_SIZE 메세지
//------------------------------------------------------------------------------

static void WmSize(HWND hwnd, LPARAM /*lParam*/)
{
	HGLOBAL     hGlobal ;
	WORK_SCAN_DATA		*wk;
	//RECT rect;
	HDC hdc;
	TEXTMETRIC tm;

	hdc = GetDC(hwnd);
	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
	GetTextMetrics(hdc, &tm);
	ReleaseDC(hwnd, hdc);

	cxChar = tm.tmAveCharWidth;
	cyChar = tm.tmHeight+tm.tmExternalLeading;
	cxChar = cyChar/2;

	//GetClientRect(hwnd, &rect);

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0) ;
	wk = (WORK_SCAN_DATA*) GlobalLock (hGlobal) ;

	ScrollUpdate(hwnd, wk);
	/*
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
	*/
	GlobalUnlock (hGlobal);
}

static void MessageHScroll(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	RECT rect;
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;

	//GetClientRect(hwnd, &rect);
	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	switch(LOWORD(wParam)) {
	  case SB_LINEUP:
				if(wk->nScrollHorPos <= 0)	break;
				wk->nScrollHorPos --;
				//ScrollWindow(hwnd, 0, cyChar, &rect, &rect);
				//rect.bottom = cyChar;
				InvalidateRect(hwnd, NULL, TRUE);

				GetClientRect(GetParent(hwnd), &rect);
				rect.bottom = cyChar*2;
				InvalidateRect(GetParent(hwnd), &rect, TRUE);
				break;
	  case SB_LINEDOWN:
				if(wk->nScrollHorPos >= wk->nScrollHorHap-1)	break;
				wk->nScrollHorPos++;
				//ScrollWindow(hwnd, 0, -cyChar, &rect, &rect);
				//rect.top = rect.bottom-cyChar;
				InvalidateRect(hwnd, NULL, TRUE);

				GetClientRect(GetParent(hwnd), &rect);
				rect.bottom = cyChar*2;
				InvalidateRect(GetParent(hwnd), &rect, TRUE);
				break;
	  case SB_PAGEUP:
				if(wk->nScrollHorPos <= 0)	break;
				wk->nScrollHorPos -= wk->xlimit;
				if(wk->nScrollHorPos < 0)
					wk->nScrollHorPos = 0;
				InvalidateRect(hwnd, NULL, TRUE);

				GetClientRect(GetParent(hwnd), &rect);
				rect.bottom = cyChar*2;
				InvalidateRect(GetParent(hwnd), &rect, TRUE);
				break;
	  case SB_PAGEDOWN:
				if(wk->nScrollHorPos >= wk->nScrollHorHap-1)	break;
				wk->nScrollHorPos += wk->xlimit;
				if(wk->nScrollHorPos >= wk->nScrollHorHap)
					wk->nScrollHorPos = wk->nScrollHorHap-1;
				InvalidateRect(hwnd, NULL, TRUE);

				GetClientRect(GetParent(hwnd), &rect);
				rect.bottom = cyChar*2;
				InvalidateRect(GetParent(hwnd), &rect, TRUE);
				break;
	  case SB_THUMBPOSITION:
				//wk->nScrollVerPos = HIWORD(wParam);
				SCROLLINFO info;
				info.cbSize = sizeof(SCROLLINFO);
				info.fMask = SIF_TRACKPOS;

				GetScrollInfo(hwnd, SB_HORZ, &info);
				wk->nScrollHorPos = info.nTrackPos;

				InvalidateRect(hwnd, NULL, TRUE);

				GetClientRect(GetParent(hwnd), &rect);
				rect.bottom = cyChar*2;
				InvalidateRect(GetParent(hwnd), &rect, TRUE);
				break;
	}

	GlobalUnlock(hGlobal);
	SetScrollPos(hwnd, SB_HORZ, wk->nScrollHorPos, TRUE);
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
				ScrollWindow(hwnd, 0, cyChar, &rect, &rect);
				rect.bottom = cyChar;
				InvalidateRect(hwnd, &rect, TRUE);
				break;
	  case SB_LINEDOWN:
				if(wk->nScrollVerPos >= wk->nScrollVerHap-1)	break;
				wk->nScrollVerPos++;
				ScrollWindow(hwnd, 0, -cyChar, &rect, &rect);
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
				//wk->nScrollVerPos = HIWORD(wParam);
				SCROLLINFO info;
				info.cbSize = sizeof(SCROLLINFO);
				info.fMask = SIF_TRACKPOS;

				GetScrollInfo(hwnd, SB_VERT, &info);
				wk->nScrollVerPos = info.nTrackPos;

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
	char buf[160];
	char imsi[80];

	if(IsLangKorean()) {
		sprintf(buf, "통신 메모리 ");
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		GetResourceString(IDS_ScanMemory, buf, sizeof(buf));	
		strcat(buf, " ");
	}
	else {
		sprintf(buf, "Memory ");
	}

	if(memory_type == 0) {
		strcat(buf, "WORD");
	}
	else if(memory_type == 1) {
		strcat(buf, "FLOAT");
	}
	else if(memory_type == 2) {
		strcat(buf, "DWORD");
	}
	else if(memory_type == 3) {
		strcat(buf, "STRING");
	}
	else if(memory_type == 4) {
		strcat(buf, "DOUBLE");
	}
	else if(memory_type == 5) {
		strcat(buf, "INT64");
	}
	else {
		strcat(buf, "SYSTEM");
	}

	sprintf(imsi, ":Port %d", port);

	strcat(buf, imsi);
	
	if(portBuf[port].sTitle[0] != 0) {
		strcat(buf, " (");
		strcat(buf, portBuf[port].sTitle);
		strcat(buf, ")");
	}

	char device[80];

	PlcDeviceGetInfoString(&portBuf[port].local.device, device);

	strcat(buf, " ");
	strcat(buf, device);

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
	if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
	if(wk->cViewVarType == 0) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeWORD; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufWORD[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufWORD[i].flag ) {
				wk->saveBuf[j].value  = pt->local.bufWORD[i].value;
				wk->saveBuf[j].flag   = pt->local.bufWORD[i].flag;
				DrawOnlyBuf(hdc, wk, i, false);
			}
		}
	}
	else if(wk->cViewVarType == 1) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeFLOAT; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufFLOAT[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufFLOAT[i].flag ) {
				wk->saveBuf[j].value  = pt->local.bufFLOAT[i].value;
				wk->saveBuf[j].flag   = pt->local.bufFLOAT[i].flag;
				DrawOnlyBuf(hdc, wk, i, false);
			}
		}
	}
	else if(wk->cViewVarType == 2) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeDWORD; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufDWORD[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufDWORD[i].flag) {
				wk->saveBuf[j].value  = pt->local.bufDWORD[i].value;
				wk->saveBuf[j].flag   = pt->local.bufDWORD[i].flag;
				DrawOnlyBuf(hdc, wk, i, false);
			}
		}
	}
	else if(wk->cViewVarType == 3) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeSTRING; i++, j++) {
			if( strcmp(wk->saveBufString[j].value, pt->local.bufSTRING[i].value) != 0 ||
				wk->saveBufString[j].flag != pt->local.bufSTRING[i].flag) {
				strcpy(wk->saveBufString[j].value, pt->local.bufSTRING[i].value);
				wk->saveBufString[j].flag = pt->local.bufSTRING[i].flag;
				DrawOnlyBuf(hdc, wk, i, false);
			}
		}
	}
	else if(wk->cViewVarType == 4) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeDOUBLE; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufDOUBLE[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufDOUBLE[i].flag ) {
				wk->saveBuf[j].value  = pt->local.bufDOUBLE[i].value;
				wk->saveBuf[j].flag   = pt->local.bufDOUBLE[i].flag;
				DrawOnlyBuf(hdc, wk, i, false);
			}
		}
	}
	else if(wk->cViewVarType == 5) {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < pt->local.nBufSizeINT64; i++, j++) {
			if( wk->saveBuf[j].value != pt->local.bufINT64[i].value ||
				wk->saveBuf[j].flag  != pt->local.bufINT64[i].flag) {
				wk->saveBuf[j].value  = (double)pt->local.bufINT64[i].value;
				wk->saveBuf[j].flag   = pt->local.bufINT64[i].flag;
				DrawOnlyBuf(hdc, wk, i, false);
			}
		}
	}
	else  {
		for(i = wk->nScrollVerPos, j = 0; j <= wk->ylimit && j < 100 && i < MAX_ATTR_WORD; i++, j++) {
			if( wk->saveBuf[j].value != pt->bufSYSTEM[i].value) {
				wk->saveBuf[j].value  = pt->bufSYSTEM[i].value;
				DrawOnlyBuf(hdc, wk, i, false);
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
	wk->nScrollHorPos = 0;
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
	char buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*12;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	count = GetCountStruct(wk, type);
	
	if(IsLangKorean()) 
	{
		sprintf(buf, "통신횟수-%-10ld", count->lCountCommTry);
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_CommCount, imsi);	
		sprintf(buf, "%s-%-10ld", imsi, count->lCountCommTry);
	}
	else {
		sprintf(buf, "Comm Try-%-10ld", count->lCountCommTry);
	}
	TextOut(hdc, x, y, buf, strlen(buf));
	GlobalUnlock(hGlobal);
}

static void DrawCountTimeOut(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	char buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*34;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);

	count = GetCountStruct(wk, type);
	if(IsLangKorean()) {
		sprintf(buf, "시간초과-%-10ld", count->lCountTimeOut);
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_TimeOut, imsi);	
		sprintf(buf, "%s-%-10ld", imsi, count->lCountTimeOut);
	}
	else {
		sprintf(buf, "Time Out-%-10ld", count->lCountTimeOut);
	}
	TextOut(hdc, x, y, buf, strlen(buf));
	GlobalUnlock(hGlobal);
}

static void DrawCountCodeBad(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	char buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*56;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);
	count = GetCountStruct(wk, type);
	if(IsLangKorean()) {
		sprintf(buf, "코드불량-%-10ld", count->lCountCodeBad);
	} 
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_CodeBad, imsi);
		sprintf(buf, "%s-%-10ld", imsi, count->lCountCodeBad);
	}
	else {
		sprintf(buf, "Code Bad-%-10ld", count->lCountCodeBad);
	}
	TextOut(hdc, x, y, buf, strlen(buf));
	GlobalUnlock(hGlobal);
}

static void DrawSuccessPercent(HWND hwnd, HDC hdc, int y, int type)
{
	HGLOBAL    hGlobal ;
	WORK_SCAN_DATA 	*wk;
	char buf[80];
	COMM_COUNT_STRUCT *count;

	int x = cxChar*78;
	y += cyChar*type;

	if(nPortHap == 0)	return;

	hGlobal = (HGLOBAL)GetWindowLong(hwnd, 0);
	wk = (WORK_SCAN_DATA*) GlobalLock(hGlobal);
	count = GetCountStruct(wk, type);
	if(IsLangKorean()) {
		sprintf(buf, "성공률-%.2f%%    ",  count->wSuccessPercent/100.0);
	}
	else if(IsLangChinese() || IsLangJapanese()) {
		CString imsi;
		GetResourceString(IDS_SuccessPercent, imsi);	
		sprintf(buf, "%s-%.2f%%    ",  imsi, count->wSuccessPercent/100.0);
	}
	else {
		sprintf(buf, "Success-%.2f%%    ", count->wSuccessPercent/100.0);
	}
	TextOut(hdc, x, y, buf, strlen(buf));
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
				MsgBoxLocalMemoryLow(hwnd, "Local Alloc");
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

			return 0 ;
		case WM_SIZE:
			WmSize(hwnd, lParam);
			break;
		case WM_PAINT:
			WmPaint(hwnd);
			return 0;
		case WM_HSCROLL:
			MessageHScroll(hwnd, wParam, lParam);
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
			return 0 ;
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
	int x;

	switch (message) {
		case WM_CREATE:
			hwndScanBufMemoryViewChild = hwnd;	// 통신 메모리보기 윈도우.

			hdc = GetDC(hwnd);
			if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
			GetTextMetrics(hdc, &tm);
			ReleaseDC(hwnd, hdc);

			cxChar = tm.tmAveCharWidth;
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

			wk->hwndChild = CreateWindow (szViewScanBufMemoryChildClass, "",
				  WS_CHILD | WS_VSCROLL | WS_HSCROLL,
				  0, 0,
				  200, 200,
				  hwnd, NULL, hInst, NULL) ;
			ShowWindow(wk->hwndChild, SW_SHOW);

			wk->menuButton.Init(hwnd, ((LPCREATESTRUCT) lParam) -> hInstance);
			
			if(IsLangKorean()) {
				wk->menuButton.Insert("감시 프로그램", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert("닫기[ESC]", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert("이전 포트",   IDC_SCANBUF_BUTTON_MINUS);
				wk->menuButton.Insert("다음 포트",   IDC_SCANBUF_BUTTON_PLUS);
				wk->menuButton.Insert("다음 메모리", IDC_SCANBUF_BUTTON_VAR_TYPE);
			} 
			else if(IsLangChinese() || IsLangJapanese()) 
			{
				CString imsi;
				GetResourceString(IDS_ViewMainProgram, buf);
				wk->menuButton.Insert(buf, IDC_SCANBUF_BUTTON_VIEWMAIN);
				GetResourceString(IDS_Close, buf);
				imsi.Format("%s[ESC]", buf);
				wk->menuButton.Insert(imsi, IDC_SCANBUF_BUTTON_CLOSE);
				GetResourceString(IDS_PrevPort, buf);
				wk->menuButton.Insert(buf,   IDC_SCANBUF_BUTTON_MINUS);
				GetResourceString(IDS_NextPort, buf);
				wk->menuButton.Insert(buf,   IDC_SCANBUF_BUTTON_PLUS);
				wk->menuButton.Insert("Next Memory", IDC_SCANBUF_BUTTON_VAR_TYPE);	
			}
			else {
				wk->menuButton.Insert("ViewMain", IDC_SCANBUF_BUTTON_VIEWMAIN);
				wk->menuButton.Insert("Close[ESC]", IDC_SCANBUF_BUTTON_CLOSE);
				wk->menuButton.Insert("Prev Port",   IDC_SCANBUF_BUTTON_MINUS);
				wk->menuButton.Insert("Next Port",   IDC_SCANBUF_BUTTON_PLUS);
				wk->menuButton.Insert("Next Memory",   IDC_SCANBUF_BUTTON_VAR_TYPE);
			}
			wk->menuButton.Insert("Word",   IDC_SCANBUF_BUTTON_VAR_WORD);
			wk->menuButton.Insert("Float",   IDC_SCANBUF_BUTTON_VAR_FLOAT);
			wk->menuButton.Insert("Dword",   IDC_SCANBUF_BUTTON_VAR_DWORD);
			if(eOemType == OEM_TYPE_SBAS) 
			{

			}
			else {
				wk->menuButton.Insert("String",   IDC_SCANBUF_BUTTON_VAR_STRING);
				wk->menuButton.Insert("Double",   IDC_SCANBUF_BUTTON_VAR_DOUBLE);
				wk->menuButton.Insert("Int64",   IDC_SCANBUF_BUTTON_VAR_INT64);
			}
			wk->menuButton.Insert("System",   IDC_SCANBUF_BUTTON_VAR_SYSTEM);

			if(nPortHap <= 1) {
				wk->menuButton.Enable(IDC_SCANBUF_BUTTON_MINUS, FALSE);
				wk->menuButton.Enable(IDC_SCANBUF_BUTTON_PLUS, FALSE);
			}

			if(hFontMain != NULL) wk->menuButton.SetFont(hFontMain);

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

			wk->nStatusBoxY = rect.bottom-(wk->menuButton.GetHeight()+cyChar+10)-cyChar*3;

			PopBox2(hdc, 0, wk->nStatusBoxY, rect.right, rect.bottom, WHITE_GRAY_COLOR);

			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			if(IsLangKorean()) {
				TextOut(hdc, 3, wk->nStatusBoxY+3,          "전체통신", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar,   "읽기통신", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*2, "비트쓰기", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*3, "워드쓰기", 8); 
			} 
			else if(IsLangChinese() || IsLangJapanese()) 
			{
				GetResourceString(IDS_TotalComm, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3,          buf, strlen(buf)); 
				GetResourceString(IDS_ReadComm, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar,   buf, strlen(buf)); 
				GetResourceString(IDS_BitWrite, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*2, buf, strlen(buf)); 
				GetResourceString(IDS_WordWrite, buf);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*3, buf, strlen(buf)); 
			}
			else {
				TextOut(hdc, 3, wk->nStatusBoxY+3,          "Total", 5);
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar,   "ReadComm", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*2, "BitWrite", 8); 
				TextOut(hdc, 3, wk->nStatusBoxY+3+cyChar*3, "WordWrite", 9); 
			}

			for(i = 0; i < 4; i++) {
				DrawCountTotal  (wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
				DrawCountTimeOut(wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
				DrawCountCodeBad(wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
				DrawSuccessPercent(wk->hwndChild, hdc, wk->nStatusBoxY+3, i);
			}

			PopBox2(hdc, 0, 0, rect.right, cyChar+10, WHITE_GRAY_COLOR);
			SetTextColor(hdc, DARK_COLOR);
			SetBkColor(hdc, WHITE_GRAY_COLOR);

			hGlobalChild = (HGLOBAL)GetWindowLong(wk->hwndChild, 0);
			workChild = (WORK_SCAN_DATA*) GlobalLock (hGlobalChild);

			x = -workChild->nScrollHorPos*cxChar;
			
			if(workChild->cViewVarType == 1)	{ // float view
				buf.Format("No");
				TextOut(hdc, x+cxChar/2,  5, buf, strlen(buf));
				buf.Format("Float");
				TextOut(hdc, x+cxChar*24,  5, buf, strlen(buf));
				buf.Format("Exponential");
				TextOut(hdc, x+cxChar*32, 5, buf, strlen(buf));

				DrawMethodTitle(wk->hwndChild, hdc, x+cxChar*45, 5);
			}
			else if(workChild->cViewVarType == 2)	{ // DWORD
				TextOut(hdc, x+cxChar/2,  5, "No", 2);
				TextOut(hdc, x+cxChar*12,  5, "Dec", 3);
				TextOut(hdc, x+cxChar*20, 5, "Hex", 3);
				TextOut(hdc, x+cxChar*34, 5, "Long", 4);
				DrawMethodTitle(wk->hwndChild, hdc, x+cxChar*40, 5);
			}
			else if(workChild->cViewVarType == 3)	{ // STRING
				TextOut(hdc, x+cxChar/2,  5, "No", 2);
				TextOut(hdc, x+cxChar*15,  5, "String", 6);
				DrawMethodTitle(wk->hwndChild, hdc, x+cxChar*34, 5);
			}
			else if(workChild->cViewVarType == 4)	{ // double view
				buf.Format("No");
				TextOut(hdc, x+cxChar/2,  5, buf, strlen(buf));
				buf.Format("Double");
				TextOut(hdc, x+cxChar*24,  5, buf, strlen(buf));
				buf.Format("Exponential");
				TextOut(hdc, x+cxChar*32, 5, buf, strlen(buf));
				//TextOut(hdc, x+cxChar*15, 5, "Binary", 6);
				DrawMethodTitle(wk->hwndChild, hdc, x+cxChar*45, 5);
			}
			else if(workChild->cViewVarType == 5)	{ // INT64
				TextOut(hdc, x+cxChar/2,  5, "No", 2);
				TextOut(hdc, x+cxChar*20,  5, "Dec", 3);
				TextOut(hdc, x+cxChar*34, 5, "Hex", 3);
				TextOut(hdc, x+cxChar*50, 5, "INT64", 5);
				DrawMethodTitle(wk->hwndChild, hdc, x+cxChar*64, 5);
			}
			else if(workChild->cViewVarType == 6) { // SYSTEM
				TextOut(hdc, x+cxChar/2,  5, "No", 2);
				TextOut(hdc, x+cxChar*4,  5, "Dec", 3);
				TextOut(hdc, x+cxChar*10, 5, "Hex", 3);
				TextOut(hdc, x+cxChar*15, 5, "Binary", 6);
				buf = "Description";
				TextOut(hdc, x+cxChar*33, 5, buf, strlen(buf));
			}
			else {
				TextOut(hdc, x+cxChar/2,  5, "No", 2);
				TextOut(hdc, x+cxChar*5,  5, "Dec", 3);
				TextOut(hdc, x+cxChar*11, 5, "Hex", 3);
				TextOut(hdc, x+cxChar*16, 5, "Binary", 6);
				DrawMethodTitle(wk->hwndChild, hdc, x+cxChar*34, 5);
			}

			GlobalUnlock(hGlobalChild);

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
				case IDC_CHANGE_COUNT_TOTAL:
							hdc = GetDC(hwnd);
							if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
							DrawCountTotal(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				case IDC_CHANGE_COUNT_TIMEOUT:
							hdc = GetDC(hwnd);
							if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
							DrawCountTimeOut(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				case IDC_CHANGE_COUNT_CODEBAD:
							hdc = GetDC(hwnd);
							if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
							DrawCountCodeBad(wk->hwndChild, hdc, wk->nStatusBoxY+3, lParam);
							ReleaseDC(hwnd, hdc);
							break;
				case IDC_CHANGE_SUCCESS_PERCENT:
							hdc = GetDC(hwnd); 
							if(hFontMain != NULL)	SelectObject(hdc, hFontMain);
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
		case WM_MDIACTIVATE:
			return 0 ;
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
			return 0;
	}
	// Pass unprocessed message to DefMDIChildProc
	return DefMDIChildProc (hwnd, message, wParam, lParam) ;
}