// DialogConfigScanServer.cpp : implementation file
//

#include "stdafx.h"
#include <glib.h>
//#include <windll.h>

#include "resource.h"
#include "plc_scan.h"
#include "scanserver.h"
#include "DialogConfigScanServer.h"
#include "DialogConfigDevice.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigScanServer dialog

CDialogConfigScanServer::CDialogConfigScanServer(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogConfigScanServer::IDD, pParent)
	, m_nBlockSize(0)
{
	//{{AFX_DATA_INIT(CDialogConfigScanServer)
	m_modem_init_command = _T("");
	m_tcpip_port = 0;
	m_com_port = _T("");
	m_bThread = FALSE;
	m_nSendDelay = 0;
	m_bSendOnlyChange = FALSE;
	//}}AFX_DATA_INIT
}


void CDialogConfigScanServer::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogConfigScanServer)
	DDX_Text(pDX, IDC_ConfigScanServer_EDIT_MODEM_INIT_COMMAND, m_modem_init_command);
	DDX_Text(pDX, IDC_ConfigScanServer_EDIT_TCPIP_PORT, m_tcpip_port);
	DDX_Text(pDX, IDC_ConfigScanServer_EDIT_COM_PORT, m_com_port);
	DDX_Check(pDX, IDC_ConfigScanServer_CHECK_THREAD, m_bThread);
	DDX_Text(pDX, IDC_ConfigScanServer_EDIT_SEND_DELAY, m_nSendDelay);
	DDV_MinMaxInt(pDX, m_nSendDelay, 0, 5000);
	DDX_Check(pDX, IDC_ConfigScanServer_CHECK_SEND_ONLY_CHANGE, m_bSendOnlyChange);
	//}}AFX_DATA_MAP
	DDX_Text(pDX, IDC_ConfigScanServer_EDIT_BLOCK_SIZE, m_nBlockSize);
	DDV_MinMaxInt(pDX, m_nBlockSize, 1, 500);
}

BEGIN_MESSAGE_MAP(CDialogConfigScanServer, CDialog)
	//{{AFX_MSG_MAP(CDialogConfigScanServer)
	ON_BN_CLICKED(IDC_ConfigScanServer_BUTTON_COM_PORT, OnConfigScanServerBUTTONCOMPORT)
	ON_BN_CLICKED(IDC_ConfigScanServer_RADIO_METHOD0, OnConfigScanServerRADIOMETHOD0)
	ON_BN_CLICKED(IDC_ConfigScanServer_RADIO_METHOD1, OnConfigScanServerRADIOMETHOD1)
	ON_BN_CLICKED(IDC_ConfigScanServer_RADIO_METHOD2, OnConfigScanServerRADIOMETHOD2)
	ON_BN_CLICKED(IDC_ConfigScanServer_RADIO_METHOD3, OnConfigScanServerRADIOMETHOD3)
	ON_BN_CLICKED(IDC_ConfigScanServer_RADIO_METHOD4, OnConfigScanServerRADIOMETHOD4)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void ScanServerInitOneDevice(SCAN_SERVER_LIST *conn);
void ScanServerUnInitOneDevice(SCAN_SERVER_LIST *conn);

void ConfigScanServer(HWND hwnd, int port)
{
	CDialogConfigScanServer dialog;
	char buf[80];

	SCAN_SERVER_LIST *server = &scanServerList[port];

	dialog.m_tcpip_port = server->tcpipPort;
	dialog.m_modem_init_command = server->sModemInitCommand;
	dialog.m_device_type = server->nDeviceType;
	ConvertModemStructToBuf(buf, &server->modem);
	dialog.m_com_port = buf;
	dialog.m_bThread = server->bThreadFlag;
	dialog.m_nSendDelay = server->nSendDelay;
	dialog.m_bSendOnlyChange = server->bSendOnlyChange;
	dialog.m_nBlockSize = server->nSendBlockSize;

	//sprintf(buf, "PlcScan NetworkServer (%d)", port+1);
	//dialog.SetWindowText(buf);

	if(dialog.DoModal() == IDOK) {
		ScanServerUnInitOneDevice(&scanServerList[port]);
		server->tcpipPort = dialog.m_tcpip_port;
		strcpy(server->sModemInitCommand, dialog.m_modem_init_command);
		server->nDeviceType = dialog.m_device_type;
		ConvertBufToModemStruct(dialog.m_com_port, &server->modem);
		server->bThreadFlag = dialog.m_bThread;
		server->nSendDelay = dialog.m_nSendDelay;
		server->bSendOnlyChange = dialog.m_bSendOnlyChange;
		server->nSendBlockSize = dialog.m_nBlockSize;

		ScanServerListSaveOne(port);

		ScanServerInitOneDevice(&scanServerList[port]);

		InvalidateRect(hwnd, NULL, FALSE);
	}
}

void CDialogConfigScanServer::EnableDisable() 
{
	int type = GetRadioPosition(m_hWnd, IDC_ConfigScanServer_RADIO_METHOD0, 5);

	char flag_com;
	char flag_tcpip;
	char flag_modem;

	if(type == 1) {
		flag_com = ON;
		flag_modem = OFF;
		flag_tcpip = OFF;
	}
	else if(type == 2) {
		flag_com = ON;
		flag_modem = ON;
		flag_tcpip = OFF;
	}
	else if(type == 3) {
		flag_com = OFF;
		flag_modem = OFF;
		flag_tcpip = ON;
	}
	else if(type == 4) {
		flag_com = OFF;
		flag_modem = OFF;
		flag_tcpip = ON;
	}
	else {
		flag_com = OFF;
		flag_modem = OFF;
		flag_tcpip = OFF;
	}

	GetDlgItem(IDC_ConfigScanServer_BUTTON_COM_PORT)->EnableWindow(flag_com);
	GetDlgItem(IDC_ConfigScanServer_EDIT_MODEM_INIT_COMMAND)->EnableWindow(flag_modem);
	GetDlgItem(IDC_ConfigScanServer_EDIT_TCPIP_PORT)->EnableWindow(flag_tcpip);
}

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigScanServer message handlers

BOOL CDialogConfigScanServer::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	SetRadioPosition(m_hWnd, IDC_ConfigScanServer_RADIO_METHOD0, 5, m_device_type);

	EnableDisable();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogConfigScanServer::OnOK() 
{
	// TODO: Add extra validation here
	m_device_type = GetRadioPosition(m_hWnd, IDC_ConfigScanServer_RADIO_METHOD0, 5);
	
	CDialog::OnOK();
}

void CDialogConfigScanServer::OnConfigScanServerBUTTONCOMPORT() 
{
	// TODO: Add your control notification handler code here
	UpdateData();

	CDialogConfigDevice dialog;

	dialog.bUseOnlyCom = true;
	strcpy(dialog.sDeviceString,m_com_port);
	
	if(dialog.DoModal() == IDOK) {
		m_com_port = dialog.sDeviceString;
		UpdateData(FALSE);
	}
	/*
	MODEM_STRUCT modem;
	char buf[80];

	UpdateData();

	ConvertBufToModemStruct((LPCSTR)m_com_port, &modem);
	ConfigPortRS232(m_hWnd, &modem.cPort, &modem.lBaud, &modem.cParity, &modem.cData, &modem.cStop);
	ConvertModemStructToBuf(buf, &modem);
	m_com_port = buf;

	UpdateData(FALSE);*/
}

void CDialogConfigScanServer::OnConfigScanServerRADIOMETHOD0() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();	
}

void CDialogConfigScanServer::OnConfigScanServerRADIOMETHOD1() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();	
}

void CDialogConfigScanServer::OnConfigScanServerRADIOMETHOD2() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigScanServer::OnConfigScanServerRADIOMETHOD3() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigScanServer::OnConfigScanServerRADIOMETHOD4() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}
