// DialogConfigDualOption.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogConfigDualOption.h"
#include "DialogConfigDevice.h"
#include "plc_scan.h"
#include "protocol\\pro_main.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigDualOption dialog

CDialogConfigDualOption::CDialogConfigDualOption(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogConfigDualOption::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogConfigDualOption)
	m_DualDevice = _T("");
	m_DualCauseTimeOut = 0;
	m_DualCauseCodeBad = 0;
	m_bUseProtocol = FALSE;
	m_sProtocol = _T("");
	//}}AFX_DATA_INIT
}

void CDialogConfigDualOption::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogConfigDualOption)
	DDX_Control(pDX, IDC_ConfigDualOption_COMBO_PROTOCOL, m_combo);
	DDX_Text(pDX, IDC_ConfigDualOption_EDIT_DEVICE, m_DualDevice);
	DDV_MaxChars(pDX, m_DualDevice, 80);
	DDX_Text(pDX, IDC_ConfigDualOption_EDIT_CAUSE_TIMEOUT, m_DualCauseTimeOut);
	DDV_MinMaxInt(pDX, m_DualCauseTimeOut, 0, 20);
	DDX_Text(pDX, IDC_ConfigDualOption_EDIT_CAUSE_CODEBAD, m_DualCauseCodeBad);
	DDV_MinMaxInt(pDX, m_DualCauseCodeBad, 0, 20);
	DDX_Check(pDX, IDC_ConfigDualOption_CHKBOX_USE_PROTOCOL, m_bUseProtocol);
	DDX_CBString(pDX, IDC_ConfigDualOption_COMBO_PROTOCOL, m_sProtocol);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogConfigDualOption, CDialog)
	//{{AFX_MSG_MAP(CDialogConfigDualOption)
	ON_BN_CLICKED(IDC_ConfigDualOption_BTN_DEVICE, OnConfigDualOptionBTNDEVICE)
	ON_BN_CLICKED(IDHELP, OnHelp)
	ON_BN_CLICKED(IDC_ConfigDualOption_CHKBOX_USE_PROTOCOL, OnConfigDualOptionCHKBOXUSEPROTOCOL)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_ConfigDualOption_BTN_ProtocolOption, &CDialogConfigDualOption::OnBnClickedConfigdualoptionBtnProtocoloption)
	ON_EN_CHANGE(IDC_ConfigDualOption_EDIT_ProtocolOption, &CDialogConfigDualOption::OnEnChangeConfigdualoptionEditProtocoloption)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigDualOption message handlers

void CDialogConfigDualOption::OnConfigDualOptionBTNDEVICE() 
{
	// TODO: Add your control notification handler code here
	CDialogConfigDevice dialog;

	GetDlgItem(IDC_ConfigDualOption_EDIT_DEVICE)->GetWindowText(dialog.sDeviceString, sizeof(dialog.sDeviceString));
	
	if(dialog.DoModal() == IDOK) {
		GetDlgItem(IDC_ConfigDualOption_EDIT_DEVICE)->SetWindowText(dialog.sDeviceString);
	}	
}

void CDialogConfigDualOption::OnOK() 
{
	// TODO: Add extra validation here
	if(!UpdateData())	return;

	if(strlen(m_DualDevice) == 0) {
		if(IsLangKorean()) {
			MessageBox("예비 Device를 입력해야 합니다.", "입력 오류");
		}else {
			MessageBox("Must input Secondary Device.", "Input Error");
		}
		return;
	}

	GetDlgItemText(IDC_ConfigDualOption_EDIT_ProtocolOption, m_sProtocolOption);
	
	CDialog::OnOK();
}

void CDialogConfigDualOption::OnHelp() 
{
	// TODO: Add your control notification handler code here
	PlcScanHelp(m_hWnd, "PLC_SCANDualLine.htm");
}

void AddProtocolListToComboBox(HWND hwnd, HWND hwndCombo);


BOOL CDialogConfigDualOption::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	int  i;

	for(i = 0; i < MAX_PROTOCOL_NAME_DEFINE; i++) {
		m_combo.AddString(protocolNameDefine[i].name);
	}
	AddProtocolListToComboBox(m_hWnd, ::GetDlgItem(m_hWnd, IDC_ConfigDualOption_COMBO_PROTOCOL));
	//::SetWindowText(::GetDlgItem(m_hWnd, IDC_ConfigDualOption_COMBO_PROTOCOL), sProtocol);

	//m_sProtocolOption =  GetDlgItemText(IDC_ConfigDualOption_EDIT_ProtocolOption, m_sProtocolOption);
	SetDlgItemText(IDC_ConfigDualOption_EDIT_ProtocolOption, m_sProtocolOption);

	EnableDisable();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogConfigDualOption::EnableDisable()
{
	int flag = IsDlgButtonChecked(IDC_ConfigDualOption_CHKBOX_USE_PROTOCOL);

	if(flag)	
	{
		m_combo.EnableWindow(TRUE);
	}
	else		
	{
		m_combo.EnableWindow(FALSE);
	}

	GetDlgItem(IDC_ConfigDualOption_EDIT_ProtocolOption)->EnableWindow(flag);
	GetDlgItem(IDC_ConfigDualOption_BTN_ProtocolOption)->EnableWindow(flag);
}

void CDialogConfigDualOption::OnConfigDualOptionCHKBOXUSEPROTOCOL() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDualOption::OnBnClickedConfigdualoptionBtnProtocoloption()
{
	// TODO: Add your control notification handler code here

	char protocol_name[80];
	char protocol_option[80];

	int ConfigProtocolOption(HWND hwnd, HWND hwndEdit, int port, char *protocol_name, char *protocol_option);
	GetDlgItem(IDC_ConfigDualOption_COMBO_PROTOCOL)->GetWindowText(protocol_name, sizeof(protocol_name));
	GetDlgItem(IDC_ConfigDualOption_EDIT_ProtocolOption)->GetWindowText(protocol_option, sizeof(protocol_option));
	if(ConfigProtocolOption(m_hWnd, ::GetDlgItem(m_hWnd, IDC_PortEdit_EDIT_READ), nPort, protocol_name, protocol_option)) {
		GetDlgItem(IDC_ConfigDualOption_EDIT_ProtocolOption)->SetWindowText(protocol_option);
	}	
}

void CDialogConfigDualOption::OnEnChangeConfigdualoptionEditProtocoloption()
{
	// TODO:  If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.

	// TODO:  Add your control notification handler code here
}
