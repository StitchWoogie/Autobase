// DialogConfigDevice.cpp : implementation file
//

#include "stdafx.h"

#include <tools.h>
#include <glib.h>

#include "..\catlib.src\totalcfg.h"

#include "resource.h"
#include "DialogConfigDevice.h"
#include "DialogConfigTeleDevice.h"
#include "plc_scan.h"
#include "device\commmain.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigDevice dialog


CDialogConfigDevice::CDialogConfigDevice(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogConfigDevice::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogConfigDevice)
	m_ComReadDelay = 0;
	m_ComWriteDelay = 0;
	m_ipAddress = _T("192.168.0.1");
	m_ipPort = 2004;
	m_StartRtsReadDelay = 0;
	m_StartRtsWriteDelay = 0;
	m_sShareName = _T("SharedName");
	//}}AFX_DATA_INIT

	nDeviceType = 0;
	nComPort = 1;
	nComBaud = 9600;
	nComParity = 0;
	nComDataBit = 0;
	nComStopBit = 0;
	nComTxMethod = 0;
	nComRxMethod = 0;
	nComRtsMethod = 0;
	sExtraString[0] = 0;
}

void CDialogConfigDevice::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogConfigDevice)
	DDX_Control(pDX, IDC_ConfigDevice_COMBO_COM_BAUD, m_comboComBaud);
	DDX_Control(pDX, IDC_ConfigDevice_COMBO_COM_PORT, m_comboComPort);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_COM_READ_DELAY, m_ComReadDelay);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_COM_WRITE_DELAY, m_ComWriteDelay);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_IP_ADDRESS, m_ipAddress);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_IP_PORT, m_ipPort);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_COM_READ_START_DELAY, m_StartRtsReadDelay);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_COM_WRITE_START_DELAY, m_StartRtsWriteDelay);
	DDX_Text(pDX, IDC_ConfigDevice_EDIT_SHARE_NAME, m_sShareName);
	DDV_MaxChars(pDX, m_sShareName, 79);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CDialogConfigDevice, CDialog)
	//{{AFX_MSG_MAP(CDialogConfigDevice)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE0, OnConfigDeviceRADIODEVICETYPE0)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE1, OnConfigDeviceRADIODEVICETYPE1)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE2, OnConfigDeviceRADIODEVICETYPE2)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE3, OnConfigDeviceRADIODEVICETYPE3)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE4, OnConfigDeviceRADIODEVICETYPE4)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE5, OnConfigDeviceRADIODEVICETYPE5)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE6, OnConfigDeviceRADIODEVICETYPE6)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE7, OnConfigDeviceRADIODEVICETYPE7)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE8, OnConfigDeviceRADIODEVICETYPE8)
	ON_BN_CLICKED(IDC_ConfigDevice_RADIO_DEVICE_TYPE9, OnConfigDeviceRADIODEVICETYPE9)
	ON_BN_CLICKED(IDHELP, OnHelp)
	ON_BN_CLICKED(IDC_ConfigDevice_BTN_TELEDEVICE_OPTION, OnConfigDeviceBTNTELEDEVICEOPTION)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

#define	MAX_COM_BAUD 13

static int nSampleBaudType[MAX_COM_BAUD] = { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 56000, 57600, 115200, 128000, 256000 };

static char *sTxMethod[3] = { "TxON", "TxRTS", "TxDTR" };
static char *sRxMethod[3] = { "RxON", "RxECHO", "RxETC" };

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigDevice message handlers

BOOL CDialogConfigDevice::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	CommaBlockString comma;
	char buf[80];
	int  value;
	int  i;

	comma.Set(sDeviceString);
	comma.GetString(buf, sizeof(buf));
	if(strnicmp(buf, "COM", 3) == 0) {	// com port
		nDeviceType = 1;
		nComPort = atoi(&buf[3]);
		comma.GetInt(nComBaud);
		comma.GetInt(nComParity);
		comma.GetInt(value);
		if(value == 7)	nComDataBit = 1;
		else			nComDataBit = 0;
		comma.GetInt(value);
		if(value == 2)	nComStopBit = 1;
		else			nComStopBit = 0;
		
		comma.GetString(buf, sizeof(buf));
		if(stricmp(buf, "TxRTS") == 0)	nComTxMethod = 1;
		else if(stricmp(buf, "TxDTR") == 0)	nComTxMethod = 2;
		else								nComTxMethod = 0;	

		comma.GetString(buf, sizeof(buf));
		if(stricmp(buf, "RxECHO") == 0)	nComRxMethod = 1;
		else							nComRxMethod = 0;	

		comma.GetInt(m_ComReadDelay);
		comma.GetInt(m_ComWriteDelay);
		comma.GetInt(m_StartRtsReadDelay);
		comma.GetInt(m_StartRtsWriteDelay);
		comma.GetInt(nComRtsMethod);
	}
	else if(strnicmp(buf, "MODEM", 5) == 0) {	// modem
		nDeviceType = 2;
		nComPort = atoi(&buf[5]);
		comma.GetInt(nComBaud);
		comma.GetInt(nComParity);
		comma.GetInt(value);
		if(value == 7)	nComDataBit = 1;
		else			nComDataBit = 0;
		comma.GetInt(value);
		if(value == 2)	nComStopBit = 1;
		else			nComStopBit = 0;
	}
	else if(stricmp(buf, "TCP/IP") == 0) {	// tcp/ip
		nDeviceType = 3;

		comma.GetString(buf, sizeof(buf));
		m_ipAddress = buf;
		comma.GetInt(m_ipPort);
		comma.GetStringTotalRemain(sExtraString, sizeof(sExtraString));
	}	
	else if(stricmp(buf, "UDP/IP") == 0) {	// udp/ip
		nDeviceType = 4;

		comma.GetString(buf, sizeof(buf));
		m_ipAddress = buf;
		comma.GetInt(m_ipPort);
	}
	else if(stricmp(buf, "TCP-Server") == 0) {	// 
		nDeviceType = 5;

		comma.GetInt(m_ipPort);
		comma.GetStringTotalRemain(sExtraString, sizeof(sExtraString));
	}	
	else if(strnicmp(buf, "TeleDevice", 10) == 0) {	// Tele Device
		nDeviceType = 7;

		nComPort = atoi(&buf[10]);
		comma.GetInt(nComBaud);
		comma.GetInt(nComParity);
		comma.GetInt(value);
		if(value == 7)	nComDataBit = 1;
		else			nComDataBit = 0;
		comma.GetInt(value);
		if(value == 2)	nComStopBit = 1;
		else			nComStopBit = 0;
	}
	else if(stricmp(buf, "NetClient") == 0) {		// Net Client Device
		nDeviceType = 8;

		//comma.GetString(buf, sizeof(buf));
		//m_ipAddress = buf;
		//comma.GetInt(m_ipPort);
	}
	else if(stricmp(buf, "SharedMemory") == 0) {	// Net Client Device
		nDeviceType = 9;

		comma.GetString(buf, sizeof(buf));
		m_sShareName = buf;
	}
	else {
		nDeviceType = 0;
	}

	SetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_DEVICE_TYPE0, 10, nDeviceType);
	SetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_PARITY0, 3, nComParity);
	SetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_DATA0, 2, nComDataBit);
	SetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_STOP0, 2, nComStopBit);
	SetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_TX0, 3, nComTxMethod);
	SetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_RX0, 2, nComRxMethod);

	CheckDlgButton(IDC_ConfigDevice_CHECK_RTS_METHOD, nComRtsMethod); 

	for(i = 0; i < 256; i++) {
		sprintf(buf, "COM%d", i+1);
		m_comboComPort.AddString(buf);
	}
	sprintf(buf, "COM%d", nComPort);
	m_comboComPort.SelectString(-1, buf);
 
	for(i = 0; i < MAX_COM_BAUD; i++) {
		sprintf(buf, "%d", nSampleBaudType[i]);
		m_comboComBaud.AddString(buf);
	}
	sprintf(buf, "%d", nComBaud);
	m_comboComBaud.SelectString(-1, buf);

	UpdateData(FALSE);
	EnableDisable();

	if(eOemType == OEM_TYPE_SBAS) {
		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE2)->EnableWindow(FALSE);
		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE4)->EnableWindow(FALSE);
		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE5)->EnableWindow(FALSE);
		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE6)->EnableWindow(FALSE);

		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE7)->EnableWindow(FALSE);
		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE8)->EnableWindow(FALSE);
		GetDlgItem(IDC_ConfigDevice_RADIO_DEVICE_TYPE9)->EnableWindow(FALSE);
	}
		
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogConfigDevice::EnableDisable()
{
	int pos = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_DEVICE_TYPE0, 10);	
	
	char flag_com = FALSE;
	char flag_485 = FALSE;
	char flag_ip = FALSE;
	bool flag_port = false;
	char flag_tele = FALSE;
	bool flag_share = false;
	
	int  i;

	if(pos == 1) {
		flag_com = TRUE;
		flag_485 = TRUE;
	}
	else if(pos == 2) {
		flag_com = TRUE;
	}
	else if(pos == 3) {
		flag_ip = TRUE;
		flag_port = true;
	}
	else if(pos == 4) {
		flag_ip = TRUE;
		flag_port = true;
	}
	else if(pos == 5) {
		flag_ip = false;
		flag_port = true;
	}
	else if(pos == 7) {
		flag_com = TRUE;
		flag_tele = TRUE;
	}
	else if(pos == 9) {
		flag_share = true;
	}
	else;

	GetDlgItem(IDC_ConfigDevice_COMBO_COM_PORT)->EnableWindow(flag_com);
	GetDlgItem(IDC_ConfigDevice_COMBO_COM_BAUD)->EnableWindow(flag_com);

	for(i = 0; i < 3; i++) {
		GetDlgItem(IDC_ConfigDevice_RADIO_COM_PARITY0+i)->EnableWindow(flag_com);
	}
	for(i = 0; i < 2; i++) {
		GetDlgItem(IDC_ConfigDevice_RADIO_COM_DATA0+i)->EnableWindow(flag_com);
	}
	for(i = 0; i < 2; i++) {
		GetDlgItem(IDC_ConfigDevice_RADIO_COM_STOP0+i)->EnableWindow(flag_com);
	}
	for(i = 0; i < 3; i++) {
		GetDlgItem(IDC_ConfigDevice_RADIO_COM_TX0+i)->EnableWindow(flag_485);
	}
	for(i = 0; i < 2; i++) {
		GetDlgItem(IDC_ConfigDevice_RADIO_COM_RX0+i)->EnableWindow(flag_485);
	}
	GetDlgItem(IDC_ConfigDevice_EDIT_COM_READ_DELAY)->EnableWindow(flag_485);
	GetDlgItem(IDC_ConfigDevice_EDIT_COM_WRITE_DELAY)->EnableWindow(flag_485);
	GetDlgItem(IDC_ConfigDevice_EDIT_COM_READ_START_DELAY)->EnableWindow(flag_485);
	GetDlgItem(IDC_ConfigDevice_EDIT_COM_WRITE_START_DELAY)->EnableWindow(flag_485);
	
	GetDlgItem(IDC_ConfigDevice_CHECK_RTS_METHOD)->EnableWindow(flag_485);


	GetDlgItem(IDC_ConfigDevice_EDIT_IP_ADDRESS)->EnableWindow(flag_ip);
	GetDlgItem(IDC_ConfigDevice_EDIT_IP_PORT)->EnableWindow(flag_port);

	GetDlgItem(IDC_ConfigDevice_BTN_TELEDEVICE_OPTION)->EnableWindow(flag_tele);

	GetDlgItem(IDC_ConfigDevice_EDIT_SHARE_NAME)->EnableWindow(flag_share);
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE0() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE1() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE2() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE3() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE4() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE5() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE6() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE7() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE8() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnConfigDeviceRADIODEVICETYPE9() 
{
	// TODO: Add your control notification handler code here
	EnableDisable();
}

void CDialogConfigDevice::OnOK() 
{
	// TODO: Add extra validation here
	char buf[80];

	if(!UpdateData())	return;

	nDeviceType = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_DEVICE_TYPE0, 10);
	nComParity = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_PARITY0, 3);
	nComDataBit = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_DATA0, 2);
	nComStopBit = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_STOP0, 2);
	nComTxMethod = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_TX0, 3);
	nComRxMethod = GetRadioPosition(m_hWnd, IDC_ConfigDevice_RADIO_COM_RX0, 2);
	m_comboComPort.GetWindowText(buf, sizeof(buf));
	nComPort = atoi(&buf[3]);
	m_comboComBaud.GetWindowText(buf, sizeof(buf));
	nComBaud = atoi(buf);

	nComRtsMethod = IsDlgButtonChecked(IDC_ConfigDevice_CHECK_RTS_METHOD);
	
	if(nDeviceType == 1) {
		sprintf(sDeviceString, "COM%d,%d,%d,%d,%d,", nComPort, nComBaud, nComParity, nComDataBit == 1 ? 7 : 8, nComStopBit == 1 ? 2 : 1);
		if(nComTxMethod != 0 || nComRxMethod != 0) {
			sprintf(buf, "%s,%s,", sTxMethod[nComTxMethod], sRxMethod[nComRxMethod]);
			strcat(sDeviceString, buf);

			//if(m_ComReadDelay != 0 || m_ComWriteDelay != 0) {
				sprintf(buf, "%d,%d,", m_ComReadDelay, m_ComWriteDelay);
				strcat(sDeviceString, buf);
			//}

			//if(m_StartRtsReadDelay != 0 || m_StartRtsWriteDelay != 0) {
				sprintf(buf, "%d,%d,", m_StartRtsReadDelay, m_StartRtsWriteDelay);
				strcat(sDeviceString, buf);
			//}
			sprintf(buf, "%d,", nComRtsMethod);
			strcat(sDeviceString, buf);
		}
	}
	else if(nDeviceType == 2) {
		sprintf(sDeviceString, "MODEM%d,%d,%d,%d,%d,", nComPort, nComBaud, nComParity, nComDataBit == 1 ? 7 : 8, nComStopBit == 1 ? 2 : 1);
	}
	else if(nDeviceType == 3) {
		sprintf(sDeviceString, "TCP/IP, %s, %d,", (const char*)m_ipAddress, m_ipPort);
		strcat(sDeviceString, sExtraString);
	}
	else if(nDeviceType == 4) {
		sprintf(sDeviceString, "UDP/IP, %s, %d,", (const char*)m_ipAddress, m_ipPort);
	}
	else if(nDeviceType == 5) {
		sprintf(sDeviceString, "TCP-Server, %d,", m_ipPort);
	}
	else if(nDeviceType == 7) {
		sprintf(sDeviceString, "TeleDevice%d,%d,%d,%d,%d,", nComPort, nComBaud, nComParity, nComDataBit == 1 ? 7 : 8, nComStopBit == 1 ? 2 : 1);
	}
	else if(nDeviceType == 8) {
		sprintf(sDeviceString, "NetClient");
	}
	else if(nDeviceType == 9) {
		sprintf(sDeviceString, "SharedMemory,%s", m_sShareName);
	}
	else {
		sprintf(sDeviceString, "None");
	}
	
	CDialog::OnOK();
}

void CDialogConfigDevice::OnHelp() 
{
	// TODO: Add your control notification handler code here
	PlcScanHelp(m_hWnd, "PLC_SCANDeviceSet.htm");	
}

void LoadTeleDeviceOption(int nComPort, TELE_DEVICE_OPTION *opt)
{
	char filename[MAXPATH];
	char section[80];

	sprintf(filename, "%s\\scan\\TeleDevi.ini", sDirWorkProject);
	sprintf(section, "COM%d", nComPort);

	GetPrivateProfileString(section, "Init Command", "ATZ", opt->sInitCommand, sizeof(opt->sInitCommand), filename);
	GetPrivateProfileString(section, "Connect Command", "##1", opt->sConnectCommand, sizeof(opt->sConnectCommand), filename);
	opt->nTimeOutWaitConnect = GetPrivateProfileInt(section, "nTimeOutWaitConnect", 30, filename);
}

void SaveTeleDeviceOption(int nComPort, TELE_DEVICE_OPTION *opt)
{
	char filename[MAXPATH];
	char section[80];

	sprintf(filename, "%s\\scan\\TeleDevi.ini", sDirWorkProject);
	sprintf(section, "COM%d", nComPort);

	WritePrivateProfileString(section, "Init Command", opt->sInitCommand, filename);
	WritePrivateProfileString(section, "Connect Command", opt->sConnectCommand, filename);
	WritePrivateProfileInt(section, "nTimeOutWaitConnect", opt->nTimeOutWaitConnect, filename);
}

void CDialogConfigDevice::OnConfigDeviceBTNTELEDEVICEOPTION() 
{
	// TODO: Add your control notification handler code here
	CDialogConfigTeleDevice dialog;

	TELE_DEVICE_OPTION opt;
	char buf[80];

	m_comboComPort.GetWindowText(buf, sizeof(buf));
	nComPort = atoi(&buf[3]);

	LoadTeleDeviceOption(nComPort, &opt);
	dialog.m_InitCommand = opt.sInitCommand;
	dialog.m_ConnectCommand = opt.sConnectCommand;
	dialog.m_ConnectWaitTimeOut = opt.nTimeOutWaitConnect;
	
	if(dialog.DoModal() == IDOK) {
		strcpy(opt.sInitCommand, dialog.m_InitCommand);
		strcpy(opt.sConnectCommand, dialog.m_ConnectCommand);
		opt.nTimeOutWaitConnect = dialog.m_ConnectWaitTimeOut;
		SaveTeleDeviceOption(nComPort, &opt);
	}
}
