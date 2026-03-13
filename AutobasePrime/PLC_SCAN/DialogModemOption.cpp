// DialogModemOption.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogModemOption.h"
#include "plc_scan.h"
#include "device\commmain.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogModemOption dialog


CDialogModemOption::CDialogModemOption(int modem_no, CWnd* pParent /*=NULL*/)
	: CDialog(CDialogModemOption::IDD, pParent)
{
	nModemNo = modem_no;
	//{{AFX_DATA_INIT(CDialogModemOption)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}

void CDialogModemOption::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogModemOption)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CDialogModemOption, CDialog)
	//{{AFX_MSG_MAP(CDialogModemOption)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void ModemOptionFileRead(int modem_no, MODEM_FILE_OPTION *option)
{
	char ini_path[_MAX_PATH];
	
	sprintf(ini_path, "%s\\scan\\modem%03d.ini", sDirWorkProject, modem_no);

	GetPrivateProfileString("Init", "Command", "AT &C1 B0", option->init_command, sizeof(option->init_command), ini_path);
}

void ModemOptionFileSave(int modem_no, MODEM_FILE_OPTION *option)
{
	char ini_path[MAXPATH];
	
	sprintf(ini_path, "%s\\scan\\modem%03d.ini", sDirWorkProject, modem_no);

	WritePrivateProfileString("Init", "Command", option->init_command, ini_path);
}

/////////////////////////////////////////////////////////////////////////////
// CDialogModemOption message handlers

BOOL CDialogModemOption::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	MODEM_FILE_OPTION option;
	ModemOptionFileRead(nModemNo, &option);
	::SetWindowText(::GetDlgItem(m_hWnd, IDC_ModemOption_EDIT_INIT_COMMAND), option.init_command);

	char buf[80];
	if(IsLangKorean()) {
		sprintf(buf, "모뎀 설정 ( Modem%03d )", nModemNo+1);
	}
	else {
		sprintf(buf, "Modem Option ( Modem%03d )", nModemNo+1);
	}
	SetWindowText(buf);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogModemOption::OnOK() 
{
	// TODO: Add extra validation here
	MODEM_FILE_OPTION option;
	::GetWindowText(::GetDlgItem(m_hWnd, IDC_ModemOption_EDIT_INIT_COMMAND), option.init_command, sizeof(option.init_command));
	ModemOptionFileSave(nModemNo, &option);
	
	CDialog::OnOK();
}
