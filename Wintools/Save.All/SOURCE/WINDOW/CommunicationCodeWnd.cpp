// TAG size O.K
// CommunicationCodeWnd.cpp : implementation file
//
#include "stdafx.h"

#include <glib.h>
#include <tools.h>
#include <totaldef.h>
#include <CommunicationCodeWnd.h>

#define	TEXT_COLOR		RGB(255, 255, 255)
#define	BACK_COLOR		RGB(0, 0, 0x80)

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCommunicationCodeWnd

CCommunicationCodeWnd::CCommunicationCodeWnd()
{
	cDisplayMethod = 1;
	cFirstCommunicationFlag = 0;
	bPause = OFF;
}

CCommunicationCodeWnd::~CCommunicationCodeWnd()
{
}


BEGIN_MESSAGE_MAP(CCommunicationCodeWnd, CWnd)
	//{{AFX_MSG_MAP(CCommunicationCodeWnd)
	ON_WM_PAINT()
	ON_WM_CREATE()
	ON_WM_SIZE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CCommunicationCodeWnd message handlers

BOOL CCommunicationCodeWnd::Create(CWnd* pParentWnd, int id)
{
	LPCTSTR class_name = AfxRegisterWndClass(0);

	RECT rect;

	rect.left = 0;
	rect.top = 0;
	rect.right = 0;
	rect.bottom = 0;

	return CWnd::Create(class_name, "Communication code view window", WS_CHILD|WS_BORDER|WS_VISIBLE, rect, pParentWnd, id);
}

BOOL CCommunicationCodeWnd::CreateWithTitle(HWND hwndParent, char *title, int id)
{
	LPCTSTR class_name = AfxRegisterWndClass(0);

	RECT rect;

	rect.left = 0;
	rect.top = 0;
	rect.right = 0;
	rect.bottom = 0;

	return CWnd::CreateEx(0, class_name, title, WS_CHILD|WS_CAPTION|WS_VISIBLE, 0, 0, 0, 0, hwndParent, (HMENU)id, NULL);

	//return CWnd::Create(class_name, "Communication code view window", WS_CHILD|WS_BORDER|WS_VISIBLE, rect, pParentWnd, nID);
}

BOOL CCommunicationCodeWnd::CreateWithTitle(CWnd* pParentWnd, char *title, int id)
{
	LPCTSTR class_name = AfxRegisterWndClass(0);

	RECT rect;

	rect.left = 0;
	rect.top = 0;
	rect.right = 0;
	rect.bottom = 0;

	return CWnd::Create(class_name, title, WS_CHILD|WS_VISIBLE|WS_CAPTION, rect, pParentWnd, id);
}

void CCommunicationCodeWnd::OnPaint()
{
	CPaintDC dc(this); // device context for painting
	
	// TODO: Add your message handler code here
	gcls(dc.m_hDC, 0, 0, rClient.right-1, rClient.bottom-1, BACK_COLOR);
	
	// Do not call CWnd::OnPaint() for painting messages
}

void CCommunicationCodeWnd::DisplayCode(const char *buf, int size) 
{
	int i;

	for(i = 0; i < size; i++) {
		DisplayOneChar(buf[i]);
	}
}

int CCommunicationCodeWnd::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CWnd::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	// TODO: Add your specialized creation code here
	CClientDC dc(this);
	TEXTMETRIC tm;

	dc.GetTextMetrics(&tm);

	cxChar = tm.tmAveCharWidth;
	cyChar = tm.tmHeight+tm.tmExternalLeading;
	
	return 0;
}

void CCommunicationCodeWnd::OnSize(UINT nType, int cx, int cy) 
{
	CWnd::OnSize(nType, cx, cy);
	
	// TODO: Add your message handler code here
	GetClientRect(&rClient);
	nPosX = 0;
	nPosY = 0;	
}

void CCommunicationCodeWnd::NextLine(CDC *dc)
{
	if(bPause == ON)	return;
	
	nPosX = 0;

	if(nPosY+cyChar*2 >= rClient.bottom) {
		RECT r;

		r.left   = 0;
		r.top    = 0;
		r.right  = rClient.right;
		r.bottom = rClient.bottom;

		//ScrollWindow(0, -cyChar);
		dc->ScrollDC(0, -cyChar, &r, &r, NULL, NULL);
		gcls(dc->m_hDC, 0, nPosY, rClient.right-1, rClient.bottom-1, BACK_COLOR);
	}
	else {
		nPosY += cyChar;
	}
}

void CCommunicationCodeWnd::DisplayOneChar(int ch)
{
	if(bPause == ON)	return;
	
	CClientDC dc(this);
	
	char buf[10];
	SIZE size;
	COLORREF lBackColor;

	if(cDisplayMethod == 0) {
		if(cFirstCommunicationFlag) {
      		cFirstCommunicationFlag = OFF;
			lBackColor = RGB(0x80, 0, 0);
		}
		else
			lBackColor = BACK_COLOR;

		sprintf(buf, "%02X ", (BYTE)ch);
	}
	else if(cDisplayMethod == 2) {
		if(cFirstCommunicationFlag) {
      		cFirstCommunicationFlag = OFF;
			lBackColor = RGB(0x80, 0, 0);
		}
		else
			lBackColor = BACK_COLOR;

		sprintf(buf, "%03d ", (BYTE)ch);
	}
	else {
		lBackColor = RGB(0x80, 0, 0);
		switch(ch) {
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
			case LF:	strcpy(buf, "<LF>");	break;
			case CR:	strcpy(buf, "<CR>");	break;
			case DLE:	strcpy(buf, "<DLE>");	break;
			case NAK:	strcpy(buf, "<NAK>");	break;
			case 0x09:	strcpy(buf, "<HT>");	break;
			default:
						lBackColor = BACK_COLOR;
						sprintf(buf, "%c", ch);
						break;
		}
	}

	GetTextExtentPoint32(dc.m_hDC, buf, strlen(buf), &size);

	if(size.cx+nPosX > rClient.right) {
		NextLine(&dc);
	}

	dc.SetTextColor(WHITE_COLOR);
	dc.SetBkColor(lBackColor);
	dc.TextOut(nPosX, nPosY, buf, strlen(buf));

	nPosX += size.cx;
}

void CCommunicationCodeWnd::DisplayNextLine()
{
	CClientDC dc(this);

	NextLine(&dc);
}
