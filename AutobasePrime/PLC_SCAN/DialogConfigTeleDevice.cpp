// DialogConfigTeleDevice.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogConfigTeleDevice.h"

#include "plc_scan.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigTeleDevice dialog


CDialogConfigTeleDevice::CDialogConfigTeleDevice(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogConfigTeleDevice::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogConfigTeleDevice)
	m_ConnectCommand = _T("");
	m_InitCommand = _T("");
	m_ConnectWaitTimeOut = 0;
	//}}AFX_DATA_INIT
}


void CDialogConfigTeleDevice::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogConfigTeleDevice)
	DDX_Text(pDX, IDC_ConfigTeleDevice_EDIT_CONNECT_COMMAND, m_ConnectCommand);
	DDV_MaxChars(pDX, m_ConnectCommand, 80);
	DDX_Text(pDX, IDC_ConfigTeleDevice_EDIT_INIT_COMMAND, m_InitCommand);
	DDV_MaxChars(pDX, m_InitCommand, 80);
	DDX_Text(pDX, IDC_ConfigTeleDevice_EDIT_CONNECT_WAIT_TIMEOUT, m_ConnectWaitTimeOut);
	DDV_MinMaxInt(pDX, m_ConnectWaitTimeOut, 5, 60);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogConfigTeleDevice, CDialog)
	//{{AFX_MSG_MAP(CDialogConfigTeleDevice)
	ON_BN_CLICKED(IDHELP, OnHelp)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigTeleDevice message handlers

void CDialogConfigTeleDevice::OnHelp()
{
	// TODO: Add your control notification handler code here
	PlcScanHelp(m_hWnd, "PLC_SCANTeleDevSet.htm");
}
