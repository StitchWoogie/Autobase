// DialogSystemInfo.cpp : implementation file
//

#include "stdafx.h"

#include <tools.h>
#include <glib.h>

#include "..\catlib.src\main_scan.h"

#include "plc_scan.h"
#include "resource.h"
#include "DialogSystemInfo.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogSystemInfo dialog


CDialogSystemInfo::CDialogSystemInfo(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogSystemInfo::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogSystemInfo)
	m_wait = 0;
	//}}AFX_DATA_INIT
}


void CDialogSystemInfo::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogSystemInfo)
	DDX_Control(pDX, IDC_SystemInfo_EDIT_WRITE_WAIT, m_edit);
	DDX_Control(pDX, IDC_SystemInfo_BTN_WRITE_WAIT_LIST, m_list);
	DDX_Text(pDX, IDC_SystemInfo_EDIT_WRITE_WAIT, m_wait);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogSystemInfo, CDialog)
	//{{AFX_MSG_MAP(CDialogSystemInfo)
	ON_WM_TIMER()
	ON_WM_DRAWITEM()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogSystemInfo message handlers

void SystemInfo()
{
	CDialogSystemInfo dialog;

	dialog.DoModal();
}

BOOL CDialogSystemInfo::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	SetTimer(0, 1000, NULL);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogSystemInfo::OnCancel() 
{
	// TODO: Add extra cleanup here
	KillTimer(0);
	
	CDialog::OnCancel();
}

static int GetCountWriteWait()
{
	int port;
	GLOBAL_PORT_STRUCT *pt;
	int hap = 0;
	int count;

	for(port = 0; port < nPortHap; port++) {
		pt = &portBuf[port];
		if(pt->bActiveFlag == OFF)	continue;
		count = pt->blockWriteWait->ring_target-pt->blockWriteWait->ring_current;
		if(count < 0)	count += MAX_SCAN_WRITE_LOCAL_ITEM_COUNT;
		hap += count;
	}

	return hap;
}

void CDialogSystemInfo::OnTimer(UINT nIDEvent) 
{
	// TODO: Add your message handler code here and/or call default
	CString buf;
	
	buf.Format("%d", GetCountWriteWait());

	m_edit.SetWindowText(buf);

	m_list.InvalidateRect(NULL);
	
	CDialog::OnTimer(nIDEvent);
}

// extern Block blockWriteWait;

void CDialogSystemInfo::OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpdis) 
{
	// TODO: Add your message handler code here and/or call default
	gcls(lpdis->hDC, &lpdis->rcItem, WHITE_COLOR);

	DWORD l;
	SCAN_WRITE_EXCHANGE_ITEM *wait;
	int y = 0;
	TEXTMETRIC tm;
	int cyChar;
	CString buf;
	int port;
	GLOBAL_PORT_STRUCT *pt;

	GetTextMetrics(lpdis->hDC, &tm);
	cyChar = tm.tmExternalLeading+tm.tmHeight;
	
	SetTextColor(lpdis->hDC, DARK_COLOR);
	SetBkColor(lpdis->hDC, WHITE_COLOR);

	y = 0;
	for(port = 0; port < nPortHap; port++) {
		pt = &portBuf[port];
		if(pt->bActiveFlag == OFF)	continue;

		l = pt->blockWriteWait->ring_current;
		if(l == pt->blockWriteWait->ring_target)	continue;
		for(;y < lpdis->rcItem.bottom; y+=cyChar) {
			l++;
			l %= 1000;
			if(l == pt->blockWriteWait->ring_target)	break;

			wait = &pt->blockWriteWait->item[l];
			buf.Format("Tag=%s, Port=%d, St=%d, address=%04X, extra1=%s, extra2=%d, value=%.2f", 
							wait->tag,
							wait->port, 
							wait->station,
							wait->address,
							wait->sExtraAddr,
							wait->wExtraAddr,
							wait->value);
			TextOut(lpdis->hDC, 0, y, buf, strlen(buf));
		}
	}
}
