// DialogMultiConfig.cpp : implementation file
//

#include "stdafx.h"

#include "plc_scan.h"
#include "protocol\pro_main.h"

#include "resource.h"
#include "DialogMultiConfig.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogMultiConfig dialog


CDialogMultiConfig::CDialogMultiConfig(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogMultiConfig::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogMultiConfig)
	m_nPortFrom = 255;
	m_nPortTo = 255;
	m_sProtocolOption = _T("");
	m_sRead = _T("");
	m_nSizeDWORD = 0;
	m_nSizeFLOAT = 0;
	m_nSizeWORD = 100;
	m_bThread = TRUE;
	m_sDevice = _T("None");
	m_bActive = TRUE;
	m_nReadCycle = 0;
	m_nWriteCycle = 0;
	m_nThreadCycle = 0;
	//}}AFX_DATA_INIT
}


void CDialogMultiConfig::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogMultiConfig)
	DDX_Control(pDX, IDC_MultiConfig_COMBO_PROTOCOL, m_combo);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_PORT_FROM, m_nPortFrom);
	DDV_MinMaxInt(pDX, m_nPortFrom, 0, 9999);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_PORT_TO, m_nPortTo);
	DDV_MinMaxInt(pDX, m_nPortTo, 0, 9999);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_PROTOCOL_OPTION, m_sProtocolOption);
	DDV_MaxChars(pDX, m_sProtocolOption, 80);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_READ, m_sRead);
	DDV_MaxChars(pDX, m_sRead, 30000);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_SIZE_DWORD, m_nSizeDWORD);
	DDV_MinMaxInt(pDX, m_nSizeDWORD, 0, 100000);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_SIZE_FLOAT, m_nSizeFLOAT);
	DDV_MinMaxInt(pDX, m_nSizeFLOAT, 0, 100000);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_SIZE_WORD, m_nSizeWORD);
	DDV_MinMaxInt(pDX, m_nSizeWORD, 0, 100000);
	DDX_Check(pDX, IDC_MultiConfig_CHECK_THREAD, m_bThread);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_DEVICE, m_sDevice);
	DDV_MaxChars(pDX, m_sDevice, 80);
	DDX_Check(pDX, IDC_MultiConfig_CHECK_ACTIVE, m_bActive);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_READ_CYCLE, m_nReadCycle);
	DDV_MinMaxInt(pDX, m_nReadCycle, 0, 30000);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_WRITE_CYCLE, m_nWriteCycle);
	DDV_MinMaxInt(pDX, m_nWriteCycle, 0, 30000);
	DDX_Text(pDX, IDC_MultiConfig_EDIT_THREAD_CYCLE, m_nThreadCycle);
	DDV_MinMaxInt(pDX, m_nThreadCycle, 0, 30000);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogMultiConfig, CDialog)
	//{{AFX_MSG_MAP(CDialogMultiConfig)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_MultiConfig_CHECK_THREAD, &CDialogMultiConfig::OnBnClickedMulticonfigCheckThread)
	ON_BN_CLICKED(ID_ButtonProtocolOption, &CDialogMultiConfig::OnBnClickedButtonprotocoloption)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogMultiConfig message handlers

void MultiConfig()
{
	CDialogMultiConfig dialog;

	dialog.DoModal();
}

CString ConvertCommandString(const char *source, int start_port_no, int current_port_no)
{
	CString target;
	CString command;
	bool command_start = false;

	for(int i = 0; i < (int)strlen(source); i++) {
		if(source[i] == '#') {
			if(command_start) {
				command_start = false;
				command += source[i];
				
				if(strncmp(command, "#AutoCount", 10) == 0) {
					command = command.Right(strlen(command)-10);
					int no = atoi(command);
					no = no+(current_port_no-start_port_no);
					command.Format("%d", no);
					target += command;
				}
				else {
					target += command;
				}

				command = "";
			}
			else {
				command_start = true;
				command += source[i];
			}
		}
		else {
			if(command_start)
				command += source[i];
			else 
				target += source[i];
		}
	}

	if(strlen(command) != 0) {
		target += command;
	}

	return target;
}

void CDialogMultiConfig::OnOK() 
{
	// TODO: Add extra validation here

	if(!UpdateData())	return;

	if(MessageBox("이 설정을 실행하면 선택한 포트의 모든 설정이 동일하게 바뀝니다.\n설정을 적용할까요?", "주의", MB_YESNO) 
		!= IDYES)	return;

	CString filename;
	FILE *out;

	//char bActiveFlag = 1;
	//char sDevice[80] = "None";
	WORD wTimeOutRead = 3;
	WORD wTimeOutWrite = 3;
	//char sProtocol[80] = "";
	CString sProtocol;

	m_combo.GetWindowText(sProtocol);

	int i;
	int count = 0;

	for(i = m_nPortFrom; i <= m_nPortTo; i++) {
		filename.Format("%s\\SCAN\\SCAN.%03d", sDirWorkProject, i);

		out = fopen(filename, "w");
		if(out == NULL)	continue;

		fprintf(out, "ACTIVE, %s\r\n", m_bActive ? "ON" : "OFF");
		//fprintf(out, "TITLE, %s,\r\n", sDescription);
		fprintf(out, "BUF_LENGTH, %d,\r\n", m_nSizeWORD);
		fprintf(out, "BUF_LENGTH_FLOAT, %d,\r\n", m_nSizeFLOAT);
		fprintf(out, "BUF_LENGTH_DWORD, %d,\r\n", m_nSizeDWORD);

		// DWORD를 100000개로 잡으면 나머지 버퍼도 최대로 잡는다. 2020-1-23 테스트를 위해서 추가.
		if(m_nSizeDWORD == 100000) {
			fprintf(out, "BUF_LENGTH_STRING, %d,\r\n", 30000);
			fprintf(out, "BUF_LENGTH_DOUBLE, %d,\r\n", 100000);
			fprintf(out, "BUF_LENGTH_INT64, %d,\r\n", 100000);
		}

		fprintf(out, "DEVICE, %s\r\n", ConvertCommandString(m_sDevice, m_nPortFrom, i));
		fprintf(out, "PROTOCOL, %s,%s\r\n", sProtocol, ConvertCommandString(m_sProtocolOption, m_nPortFrom, i));
		fprintf(out, "MAX_TIME_OUT_READ, %d,\r\n", wTimeOutRead);
		fprintf(out, "MAX_TIME_OUT_WRITE, %d,\r\n", wTimeOutWrite);
		//fprintf(out, "TELNUMBER, %s,\r\n", sTelNumber);// , 저장 문제점
		//fprintf(out, "TelConnectCicle, %d,\r\n", nTelConnectCicle);
		//fprintf(out, "TelConnectingTime, %d,\r\n", nTelConnectingTime);
		//fprintf(out, "TelAutoConnection, %d,\r\n", bTelAutoConnection);
		fprintf(out, "ScanTimeRead, %d,\r\n", m_nReadCycle);
		fprintf(out, "ScanTimeWrite, %d,\r\n", m_nWriteCycle);
		//fprintf(out, "DualActive, %d,\r\n", bDualActive);
		//fprintf(out, "DualDevice, %s\r\n", sDualDevice);
		//fprintf(out, "DualCauseTimeOut, %d,\r\n",  nDualCauseTimeOut);
		//fprintf(out, "DualUseProtocol,%d,\r\n", bDualUseProtocol);
		//fprintf(out, "DualProtocol,%s,\r\n", sDualProtocol);

		fprintf(out, "ActiveThread, %d,\r\n",  m_bThread);
		fprintf(out, "ThreadCycle, %d,\r\n",  m_nThreadCycle);

		fprintf(out, "%s", m_sRead);

		fclose(out);

		count++;
	}

	CString msg;

	msg.Format("%d개의 포트 환경을 변경했습니다.\n통신 프로그램을 다시 시작해야 변경된 포트가 적용됩니다.", count);

	MessageBox(msg, "포트 변경");
	
	// CDialog::OnOK();
}

void AddProtocolListToComboBox(HWND hwnd, HWND hwndCombo);

BOOL CDialogMultiConfig::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	int i;

	for(i = 0; i < MAX_PROTOCOL_NAME_DEFINE; i++) {
		m_combo.AddString(protocolNameDefine[i].name);
	}
	AddProtocolListToComboBox(m_hWnd, m_combo.m_hWnd);
	
	m_combo.SetWindowText("Network Client Virtual");
	//::SetWindowText(::GetDlgItem(m_hWnd, IDC_PortEdit_COMBO_PROTOCOL), sProtocol);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogMultiConfig::OnBnClickedMulticonfigCheckThread()
{
	// TODO: Add your control notification handler code here
}

void CDialogMultiConfig::OnBnClickedButtonprotocoloption()
{
	// TODO: Add your control notification handler code here
	char protocol_name[80];
	char protocol_option[80];
	int nPort = 0;

	int ConfigProtocolOption(HWND hwnd, HWND hwndEdit, int port, char *protocol_name, char *protocol_option);
	GetDlgItem(IDC_MultiConfig_COMBO_PROTOCOL)->GetWindowText(protocol_name, sizeof(protocol_name));
	GetDlgItem(IDC_MultiConfig_EDIT_PROTOCOL_OPTION)->GetWindowText(protocol_option, sizeof(protocol_option));
	if(ConfigProtocolOption(m_hWnd, ::GetDlgItem(m_hWnd, IDC_MultiConfig_EDIT_READ), nPort, protocol_name, protocol_option)) {
		GetDlgItem(IDC_MultiConfig_EDIT_PROTOCOL_OPTION)->SetWindowText(protocol_option);
	}
}
