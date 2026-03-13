// DialogConfigTelOption.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogConfigTelOption.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigTelOption dialog


CDialogConfigTelOption::CDialogConfigTelOption(int cicle, int time, char *tel, CWnd* pParent /*=NULL*/)
	: CDialog(CDialogConfigTelOption::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogConfigTelOption)
	m_cicle = cicle;
	m_time = time;
	m_tel_num = _T(tel);
	m_autoConnection = FALSE;
	m_bAutoDisconnectOnManual = FALSE;
	m_timeOnManual = 0;
	//}}AFX_DATA_INIT
}


void CDialogConfigTelOption::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogConfigTelOption)
	DDX_Text(pDX, IDC_ConfigTelOption_EDIT_CONNECT_CICLE, m_cicle);
	DDV_MinMaxInt(pDX, m_cicle, 5, 1440);
	DDX_Text(pDX, IDC_ConfigTelOption_EDIT_CONNECTING_TIME, m_time);
	DDV_MinMaxInt(pDX, m_time, 10, 3600);
	DDX_Text(pDX, IDC_ConfigTelOption_EDIT_TEL_NUMBER, m_tel_num);
	DDV_MaxChars(pDX, m_tel_num, 20);
	DDX_Check(pDX, IDC_ConfigTelOption_CHKBOX_AUTO_CONNECT, m_autoConnection);
	DDX_Text(pDX, IDC_ConfigTelOption_EDIT_CONNECTING_TIME_ON_MANUAL, m_timeOnManual);
	DDV_MinMaxInt(pDX, m_timeOnManual, 0, 36000);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogConfigTelOption, CDialog)
	//{{AFX_MSG_MAP(CDialogConfigTelOption)
	ON_BN_CLICKED(IDC_ConfigTelOption_CHKBOX_AUTO_CONNECT, OnConfigTelOptionCHKBOXAUTOCONNECT)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigTelOption message handlers

void CDialogConfigTelOption::OnOK() 
{
	// TODO: Add extra validation here
	if(UpdateData() == 0)	return;
	CDialog::OnOK();
}

void CDialogConfigTelOption::EnableDisable()
{
	char flag = IsDlgButtonChecked(IDC_ConfigTelOption_CHKBOX_AUTO_CONNECT);

	GetDlgItem(IDC_ConfigTelOption_EDIT_CONNECT_CICLE)->EnableWindow(flag);
}

void CDialogConfigTelOption::OnConfigTelOptionCHKBOXAUTOCONNECT() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

BOOL CDialogConfigTelOption::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	EnableDisable();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
