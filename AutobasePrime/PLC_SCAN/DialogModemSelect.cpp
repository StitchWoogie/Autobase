// DialogModemSelect.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogModemSelect.h"
#include "DialogModemOption.h"
#include ".\dialogmodemselect.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogModemSelect dialog


CDialogModemSelect::CDialogModemSelect(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogModemSelect::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogModemSelect)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CDialogModemSelect::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogModemSelect)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogModemSelect, CDialog)
	//{{AFX_MSG_MAP(CDialogModemSelect)
	ON_LBN_DBLCLK(IDC_ModemSelect_LIST_MODEM, OnDblclkModemSelectLISTMODEM)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDOK, OnBnClickedOk)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogModemSelect message handlers


void ConfigModem(HWND hwnd)
{
	CDialogModemSelect dialog;

	if(dialog.DoModal() == IDOK) {
		CDialogModemOption option(dialog.nSelectItem);	

		option.DoModal();
	}	
}

BOOL CDialogModemSelect::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	char buf[80];
	int i;

	for(i = 0; i < 256; i++) {
		sprintf_s(buf, _countof(buf), "Modem%03d", i+1);
		::SendMessage(::GetDlgItem(m_hWnd, IDC_ModemSelect_LIST_MODEM), LB_ADDSTRING, 0, (LPARAM)buf);
	}
	::SendMessage(::GetDlgItem(m_hWnd, IDC_ModemSelect_LIST_MODEM), LB_SETCURSEL, 0, 0);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogModemSelect::OnOK() 
{
	// TODO: Add extra validation here
	LRESULT retn;
	retn = ::SendMessage(::GetDlgItem(m_hWnd, IDC_ModemSelect_LIST_MODEM), LB_GETCURSEL, 0, 0);

	if(retn == LB_ERR) {
		MessageBox("Modem not selected.", "Select Error");
		return;
	}
	nSelectItem = retn;
	
	CDialog::OnOK();
}

void CDialogModemSelect::OnDblclkModemSelectLISTMODEM() 
{
	// TODO: Add your control notification handler code here
	SendMessage(WM_COMMAND, IDOK, 0L);
}

void CDialogModemSelect::OnBnClickedOk()
{
	// TODO: Add your control notification handler code here
	OnOK();
}
