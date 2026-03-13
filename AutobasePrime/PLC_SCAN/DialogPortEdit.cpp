// DialogPortEdit.cpp : implementation file
//

#include "stdafx.h"

#include <tools.h>

#include "..\catlib.src\totalcfg.h"
#include "..\catdll\catdll.hpp"

#include "resource.h"
#include "DialogPortEdit.h"
#include "plc_scan.h" 
#include "protocol\pro_main.h"
#include "DialogConfigTelOption.h"
#include "DialogConfigDevice.h"
#include "DialogConfigDualOption.h"
#include "DialogComputerDualOption.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogPortEdit dialog

CDialogPortEdit::CDialogPortEdit(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogPortEdit::IDD, pParent)
	, m_bComputerDualActive(FALSE)
	, m_bUseStationInfo(FALSE)
	, wBufLengthDOUBLE(0)
	, wBufLengthINT64(0)
	, wBufLengthWORD(0)
	, m_bUseDeviceInfo(FALSE)
{
	//{{AFX_DATA_INIT(CDialogPortEdit)
	bActiveFlag = FALSE;
	nPort = 0;
	wBufLengthDWORD = 0;
	wBufLengthFLOAT = 0;
	wBufLengthWORD = 100;
	sDescription = _T("");
	sDevice = _T("COM1, 9600, 0, 8, 1,");
	sProtocolOption = _T("");
	wTimeOutRead = 3;
	wTimeOutWrite = 3;
	m_nReadScanTime = 0;
	m_nWriteScanTime = 0;
	bDualActive = FALSE;
	m_bActiveThread = FALSE;
	m_nThreadCycle = 0;
	wBufLengthSTRING = 0;
	//}}AFX_DATA_INIT

	sProtocol[0] = 0;
	sTelNumber[0] = 0;
	bTelAutoConnection = ON;

	editBuf = new char[30000];
	if(editBuf) {
		memset(editBuf, 0, 30000);
	}

	sDualDevice[0] = 0;
	nDualCauseTimeOut = 5;
	nDualCauseCodeBad = 5;
}

CDialogPortEdit::~CDialogPortEdit()
{
	if(editBuf)	delete editBuf;
}


void CDialogPortEdit::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogPortEdit)
	DDX_Check(pDX, IDC_PortEdit_CHKBOX_PORT_ACTIVE, bActiveFlag);
	DDX_Text(pDX, IDC_PortEdit_EDIT_PORT, nPort);
	DDV_MinMaxInt(pDX, nPort, 0, 9999);
	DDX_Text(pDX, IDC_PortEdit_EDIT_LENGTH_DWORD, wBufLengthDWORD);
	DDV_MinMaxInt(pDX, wBufLengthDWORD, 0, 100000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_LENGTH_FLOAT, wBufLengthFLOAT);
	DDV_MinMaxInt(pDX, wBufLengthFLOAT, 0, 100000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_LENGTH_WORD, wBufLengthWORD);
	DDV_MinMaxInt(pDX, wBufLengthWORD, 0, 100000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_DESCRIPTION, sDescription);
	DDX_Text(pDX, IDC_PortEdit_EDIT_DEVICE, sDevice);
	DDX_Text(pDX, IDC_PortEdit_EDIT_PROTOCOL_OPTION, sProtocolOption);
	DDX_Text(pDX, IDC_PortEdit_EDIT_READ_TIMEOUT, wTimeOutRead);
	DDV_MinMaxInt(pDX, wTimeOutRead, 100, 7200000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_WRITE_TIMEOUT, wTimeOutWrite);
	DDV_MinMaxInt(pDX, wTimeOutWrite, 100, 7200000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_READ_SCANTIME, m_nReadScanTime);
	DDV_MinMaxInt(pDX, m_nReadScanTime, 0, 3600000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_WRITE_SCANTIME, m_nWriteScanTime);
	DDV_MinMaxInt(pDX, m_nWriteScanTime, 0, 3600000);
	DDX_Check(pDX, IDC_PortEdit_CHKBOX_DUAL, bDualActive);
	DDX_Check(pDX, IDC_PortEdit_CHKBOX_THREAD_ACTIVE, m_bActiveThread);
	DDX_Text(pDX, IDC_PortEdit_EDIT_THREAD_CYCLE, m_nThreadCycle);
	DDV_MinMaxInt(pDX, m_nThreadCycle, 0, 30000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_LENGTH_STRING, wBufLengthSTRING);
	DDV_MinMaxInt(pDX, wBufLengthSTRING, 0, 30000);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_PortEdit_CHKBOX_PC_DUAL, m_bComputerDualActive);
	DDX_Check(pDX, IDC_PortEdit_CHKBOX_USE_STATION_INFO, m_bUseStationInfo);
	DDX_Text(pDX, IDC_PortEdit_EDIT_LENGTH_DOUBLE, wBufLengthDOUBLE);
	DDV_MinMaxInt(pDX, wBufLengthDOUBLE, 0, 100000);
	DDX_Text(pDX, IDC_PortEdit_EDIT_LENGTH_INT64, wBufLengthINT64);
	DDV_MinMaxInt(pDX, wBufLengthINT64, 0, 100000);
	DDX_Check(pDX, IDC_PortEdit_CHKBOX_USE_DEVICE_INFO, m_bUseDeviceInfo);
}


BEGIN_MESSAGE_MAP(CDialogPortEdit, CDialog)
	//{{AFX_MSG_MAP(CDialogPortEdit)
	ON_BN_CLICKED(IDC_PortEdit_CHKBOX_PORT_ACTIVE, OnPortEditCHKBOXPORTACTIVE)
	ON_BN_CLICKED(IDHELP, OnHelp)
	ON_BN_CLICKED(IDC_PortEdit_BTN_PROTOCOL_OPTION, OnPortEditBTNPROTOCOLOPTION)
	ON_BN_CLICKED(IDC_PortEdit_BUTTON_TEL_OPTION, OnPortEditBUTTONTELOPTION)
	ON_BN_CLICKED(IDC_PortEdit_BTN_DEVICE, OnPortEditBTNDEVICE)
	ON_BN_CLICKED(IDC_PortEdit_BUTTON_DUAL_OPTION, OnPortEditBUTTONDUALOPTION)
	ON_BN_CLICKED(IDC_PortEdit_CHKBOX_DUAL, OnPortEditCHKBOXDUAL)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_PortEdit_CHKBOX_PC_DUAL, OnBnClickedPorteditChkboxPcDual)
	ON_BN_CLICKED(IDC_PortEdit_BUTTON_PC_DUAL_OPTION, OnBnClickedPorteditButtonPcDualOption)
	ON_BN_CLICKED(IDC_PortEdit_CHKBOX_THREAD_ACTIVE, OnBnClickedPorteditChkboxThreadActive)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogPortEdit message handlers

void AddProtocolListToComboBox(HWND hwnd, HWND hwndCombo);

BOOL CDialogPortEdit::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	int  i;

	for(i = 0; i < MAX_PROTOCOL_NAME_DEFINE; i++) {
		::SendMessage(::GetDlgItem(m_hWnd, IDC_PortEdit_COMBO_PROTOCOL), CB_ADDSTRING, 0, (LPARAM)protocolNameDefine[i].name);
	}
	AddProtocolListToComboBox(m_hWnd, ::GetDlgItem(m_hWnd, IDC_PortEdit_COMBO_PROTOCOL));
	::SetWindowText(::GetDlgItem(m_hWnd, IDC_PortEdit_COMBO_PROTOCOL), sProtocol);

	if(editBuf == NULL) {
		MessageBox("Can't alloc editBuf", "memory insufficent", MB_OK);
	}
	else {
		::SetWindowText(::GetDlgItem(m_hWnd, IDC_PortEdit_EDIT_READ), editBuf);	
	}

	::SendMessage(::GetDlgItem(m_hWnd, IDC_PortEdit_EDIT_READ), WM_SETFONT, (WPARAM)hFontEdit, 0L);

	EnableItem();

	if(!ShareProtectHaveRights(RIGHT_PLCSCAN_EDIT)) {
		GetDlgItem(IDOK)->EnableWindow(FALSE);
	}

	if(eOemType == OEM_TYPE_SBAS) {
		
	}

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogPortEdit::EnableDualItem() 
{
	char flag;

	if(IsDlgButtonChecked(IDC_PortEdit_CHKBOX_DUAL) == 0 ||
	   IsDlgButtonChecked(IDC_PortEdit_CHKBOX_PORT_ACTIVE) == 0)
		flag = FALSE;
	else
		flag = TRUE;

	GetDlgItem(IDC_PortEdit_BUTTON_DUAL_OPTION)->EnableWindow(flag);
}

void CDialogPortEdit::EnableDualComputerItem() 
{
	char flag;

	if(IsDlgButtonChecked(IDC_PortEdit_CHKBOX_PC_DUAL) == 0 ||
	   IsDlgButtonChecked(IDC_PortEdit_CHKBOX_PORT_ACTIVE) == 0)
		flag = FALSE;
	else
		flag = TRUE;

	GetDlgItem(IDC_PortEdit_BUTTON_PC_DUAL_OPTION)->EnableWindow(flag);
}

void CDialogPortEdit::EnableThreadItem() 
{
	char flag;

	if(IsDlgButtonChecked(IDC_PortEdit_CHKBOX_THREAD_ACTIVE) == 0 ||
	   IsDlgButtonChecked(IDC_PortEdit_CHKBOX_PORT_ACTIVE) == 0)
		flag = FALSE;
	else
		flag = TRUE;

	GetDlgItem(IDC_PortEdit_EDIT_THREAD_CYCLE)->EnableWindow(flag);
}

void CDialogPortEdit::EnableItem() 
{
	char flag;
	
	if(IsDlgButtonChecked(IDC_PortEdit_CHKBOX_PORT_ACTIVE))
		flag = TRUE;
	else
		flag = FALSE;

	BOOL flag_sbas = flag;

	if(eOemType == OEM_TYPE_SBAS) {
		flag_sbas = false;
	}

	GetDlgItem(IDC_PortEdit_EDIT_DESCRIPTION)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_DEVICE)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_BTN_DEVICE)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_COMBO_PROTOCOL)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_PROTOCOL_OPTION)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_BTN_PROTOCOL_OPTION)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_LENGTH_WORD)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_LENGTH_FLOAT)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_LENGTH_DWORD)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_LENGTH_STRING)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_LENGTH_DOUBLE)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_LENGTH_INT64)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_READ_TIMEOUT)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_WRITE_TIMEOUT)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_READ)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_READ_SCANTIME)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_EDIT_WRITE_SCANTIME)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_BUTTON_TEL_OPTION)->EnableWindow(flag_sbas);

	GetDlgItem(IDC_PortEdit_CHKBOX_DUAL)->EnableWindow(flag_sbas);
	GetDlgItem(IDC_PortEdit_CHKBOX_THREAD_ACTIVE)->EnableWindow(flag);
	GetDlgItem(IDC_PortEdit_CHKBOX_PC_DUAL)->EnableWindow(flag_sbas);

	GetDlgItem(IDC_PortEdit_CHKBOX_USE_STATION_INFO)->EnableWindow(flag_sbas);
	GetDlgItem(IDC_PortEdit_CHKBOX_USE_DEVICE_INFO)->EnableWindow(flag_sbas);

	EnableDualItem();
	EnableDualComputerItem();
	EnableThreadItem();
}

void CDialogPortEdit::OnPortEditCHKBOXPORTACTIVE() 
{
	// TODO: Add your control notification handler code here
	EnableItem();	
}

void CDialogPortEdit::OnHelp() 
{
	// TODO: Add your control notification handler code here
	PlcScanHelp(m_hWnd, "PLC_SCANstart.htm");
}

void CDialogPortEdit::OnPortEditBTNPROTOCOLOPTION() 
{
	// TODO: Add your control notification handler code here
	char protocol_name[80];
	char protocol_option[80];

	int ConfigProtocolOption(HWND hwnd, HWND hwndEdit, int port, char *protocol_name, char *protocol_option);
	GetDlgItem(IDC_PortEdit_COMBO_PROTOCOL)->GetWindowText(protocol_name, sizeof(protocol_name));
	GetDlgItem(IDC_PortEdit_EDIT_PROTOCOL_OPTION)->GetWindowText(protocol_option, sizeof(protocol_option));
	if(ConfigProtocolOption(m_hWnd, ::GetDlgItem(m_hWnd, IDC_PortEdit_EDIT_READ), nPort, protocol_name, protocol_option)) {
		GetDlgItem(IDC_PortEdit_EDIT_PROTOCOL_OPTION)->SetWindowText(protocol_option);
	}	
}

void CDialogPortEdit::OnPortEditBUTTONTELOPTION() 
{
	// TODO: Add your control notification handler code here
	CDialogConfigTelOption dialog(nTelConnectCicle, nTelConnectingTime, sTelNumber);

	dialog.m_autoConnection = bTelAutoConnection;
	dialog.m_timeOnManual = nTelConnectingTimeOnManual;

	if(dialog.DoModal() == IDOK) {
		nTelConnectCicle = dialog.m_cicle;
		nTelConnectingTime =  dialog.m_time;
		strcpy(sTelNumber, dialog.m_tel_num);
		bTelAutoConnection = dialog.m_autoConnection;
		nTelConnectingTimeOnManual = dialog.m_timeOnManual;
	}	
}



int CDialogPortEdit :: SavePort()
{
	FILE *out;
	char filename[MAXPATH];

	if(editBuf == NULL) {
		MessageBox("Can't alloc editBuf", "Memory insufficent", MB_OK);
		return 0;
	}

	GetDlgItem(IDC_PortEdit_COMBO_PROTOCOL)->GetWindowText(sProtocol, sizeof(sProtocol));
	if(sProtocol[0] == 0) {
		if(IsLangKorean()) {
			MessageBox("Protocol을 설정하지 않았습니다.", "Protocol 오류", MB_OK);
		}else{
			MessageBox("Protocol not selected.", "Protocol Error", MB_OK);
		}
		GetDlgItem(IDC_PortEdit_COMBO_PROTOCOL)->SetFocus();
		return 0;
	}

	GetDlgItem(IDC_PortEdit_EDIT_READ)->GetWindowText(editBuf, 30000);
	
	sprintf(filename, "%s\\scan", sDirWorkProject);
	MakeDirectory(filename);

	sprintf(filename, "%s\\scan\\scan.%03d", sDirWorkProject, nPort);
	
	out = fopen(filename, "wb");
	if(out == NULL) {
		MessageBox(filename, "Can't write file.", MB_OK);
		return 0;
	}

#define	FILE_VERSION_Major	10 
#define	FILE_VERSION_Minor   3
#define	FILE_VERSION_Build   4

	fprintf(out, "FileVersion,%d.%d.%d\r\n", FILE_VERSION_Major, FILE_VERSION_Minor, FILE_VERSION_Build);
	fprintf(out, "ACTIVE, %s\r\n", bActiveFlag ? "ON" : "OFF");
	fprintf(out, "TITLE, %s,\r\n", sDescription);
	fprintf(out, "BUF_LENGTH, %d,\r\n", wBufLengthWORD);
	fprintf(out, "BUF_LENGTH_FLOAT, %d,\r\n", wBufLengthFLOAT);
	fprintf(out, "BUF_LENGTH_DWORD, %d,\r\n", wBufLengthDWORD);
	fprintf(out, "BUF_LENGTH_STRING, %d,\r\n", wBufLengthSTRING);
	fprintf(out, "BUF_LENGTH_DOUBLE, %d,\r\n", wBufLengthDOUBLE);
	fprintf(out, "BUF_LENGTH_INT64, %d,\r\n", wBufLengthINT64);
	fprintf(out, "DEVICE, %s\r\n", sDevice);
	fprintf(out, "PROTOCOL, %s,%s\r\n", sProtocol, sProtocolOption);

	// 10.2 이전 버전을 위하여 초 단위의 TIMEOUT도 같이 기록해 준다. 이 부분이 TIMEOUT_MILLI_???? 보다 앞에 있어야 한다.
	int old_timeout_read = wTimeOutRead/1000;
	if(old_timeout_read < 2)	old_timeout_read = 2;
	fprintf(out, "MAX_TIME_OUT_READ, %d,\r\n", old_timeout_read);
	int old_timeout_write = wTimeOutWrite/1000;
	if(old_timeout_write < 2)	old_timeout_write = 2;
	fprintf(out, "MAX_TIME_OUT_WRITE, %d,\r\n", old_timeout_write);

	fprintf(out, "TIMEOUT_MILLI_READ, %d,\r\n", wTimeOutRead);
	fprintf(out, "TIMEOUT_MILLI_WRITE, %d,\r\n", wTimeOutWrite);

	fprintf(out, "TELNUMBER, %s,\r\n", sTelNumber);// , 저장 문제점
	fprintf(out, "TelConnectCicle, %d,\r\n", nTelConnectCicle);
	fprintf(out, "TelConnectingTime, %d,\r\n", nTelConnectingTime);
	fprintf(out, "TelAutoConnection, %d,\r\n", bTelAutoConnection);
	fprintf(out, "TelConnectingTimeOnManual, %d,\r\n", nTelConnectingTimeOnManual);
	fprintf(out, "ScanTimeRead, %d,\r\n", m_nReadScanTime);
	fprintf(out, "ScanTimeWrite, %d,\r\n", m_nWriteScanTime);
	fprintf(out, "DualActive, %d,\r\n", bDualActive);
	fprintf(out, "DualDevice, %s\r\n", sDualDevice);
	fprintf(out, "DualCauseTimeOut, %d,\r\n",  nDualCauseTimeOut);
	fprintf(out, "DualCauseCodeBad, %d,\r\n",  nDualCauseCodeBad);
	fprintf(out, "DualUseProtocol,%d,\r\n", bDualUseProtocol);
	fprintf(out, "DualProtocol,%s,%s\r\n", sDualProtocol, sDualProtocolOption);

	fprintf(out, "ActiveThread,%d,\r\n",  m_bActiveThread);
	fprintf(out, "ThreadCycle,%d,\r\n",  m_nThreadCycle);

	fprintf(out, "ComputerDual,%d,%d,%s,%d,%d,\r\n",  m_bComputerDualActive, pcDual.bThread, pcDual.sIP, pcDual.nPort, pcDual.nTimeOut);
	fprintf(out, "UseStationInfo,%d,\r\n",  m_bUseStationInfo);
	fprintf(out, "UseDeviceInfo,%d,\r\n",  m_bUseDeviceInfo);

	fprintf(out, "%s", editBuf);

	fclose(out);

	return 1;
}

int CheckSameUdpPort(int my_no, int udpport, char &port_thread)
{
	GLOBAL_PORT_STRUCT *pt;
	CommaBlockString comma;
	char device[80];
	char ip[80];
	int used_udpport;
	int i;

	for(i = 0; i < nPortHap; i++) {
		if(i == my_no)	continue;

		pt = &portBuf[i];

		if(!pt->bActiveFlag)	continue;

		comma.Set(pt->sScanDevice);
		comma.GetString(device, sizeof(device));
		if(stricmp(device, "UDP/IP") == 0) {
			comma.GetString(ip, sizeof(ip));
			comma.GetInt(used_udpport);
			if(used_udpport == udpport) {
				port_thread = pt->bActiveThread;
				return i;				
			}
		}
	}

	return -1;
}

void CDialogPortEdit::OnCancel() // 250828 PSU 추가
{
    // Protocol Option 창이 열려있는지 확인하고 처리 - 250828 PSU 추가
    try {
        CWnd* pProtocolOptionWnd = CWnd::FindWindow(NULL, _T("Protocol Option"));
        if(pProtocolOptionWnd != NULL) {
            // 최소화되어 있으면 복원
            if(pProtocolOptionWnd->IsIconic()) {
                pProtocolOptionWnd->ShowWindow(SW_RESTORE);
            }
            
            // 창을 앞으로 가져오기
            pProtocolOptionWnd->SetWindowPos(&CWnd::wndTopMost, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            pProtocolOptionWnd->SetWindowPos(&CWnd::wndNoTopMost, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            pProtocolOptionWnd->SetForegroundWindow();
            pProtocolOptionWnd->BringWindowToTop();
            
            return;
        }
    } catch(...) {
        // 오류 발생 시 무시하고 계속 진행
    }

    CDialog::OnCancel();
}

void CDialogPortEdit::OnOK() 
{
	// TODO: Add extra validation here

	// Protocol Option 창이 열려있는지 확인하고 처리 - 250828 PSU 추가
	try {
		CWnd* pProtocolOptionWnd = CWnd::FindWindow(NULL, _T("Protocol Option"));
		if(pProtocolOptionWnd != NULL) {
			// 최소화되어 있으면 복원
			if(pProtocolOptionWnd->IsIconic()) {
				pProtocolOptionWnd->ShowWindow(SW_RESTORE);
			}

			// 창을 앞으로 가져오기
			pProtocolOptionWnd->SetWindowPos(&CWnd::wndTopMost, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
			pProtocolOptionWnd->SetWindowPos(&CWnd::wndNoTopMost, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
			pProtocolOptionWnd->SetForegroundWindow();
			pProtocolOptionWnd->BringWindowToTop();

			return;
		}
	} catch(...) {
		// 오류 발생 시 무시하고 계속 진행
	}

	if(!UpdateData())	return;

	GetDlgItem(IDC_PortEdit_COMBO_PROTOCOL)->GetWindowText(sProtocol, sizeof(sProtocol));

	if(bActiveFlag) {
		CommaBlockString comma;
		char device[80];
		
		comma.Set(sDevice);
		comma.GetString(device, sizeof(device));
		if(stricmp(device, "UDP/IP") == 0) {
			char ip[80];
			int udp_port;
			comma.GetString(ip, sizeof(ip));
			comma.GetInt(udp_port);
			char used_thread;
			int used_port = CheckSameUdpPort(nPort, udp_port, used_thread);
			if(used_port != -1) {
				if(m_bActiveThread) {
					CString msg;
					if(IsLangKorean()) {
						msg.Format("다른 포트(No:%d) 에서 같은 UDP/IP Port번호를 사용하고 있으므로\n독립 Thread를 사용할 수 없습니다.", used_port);
						MessageBox(msg);
					}else{
						msg.Format("Can't use Thread because another port (%d) is used UDP/IP port.", used_port);	
						MessageBox(msg);	
					}
					return;
				}
				else {
					if(used_thread) {
						CString msg;
						if(IsLangKorean()) {
							msg.Format("다른 포트(No:%d) 에서 같은 UDP/IP Port번호를 독립스레드로 사용하고 있으므로\nUDP/IP로 같은 포트번호를 사용할 수 없습니다.", used_port);
							MessageBox(msg);
						}else {
							msg.Format("Can't use UDP/IP port because another port (%d) is used UDP/IP port as thread.", used_port);	
							MessageBox(msg);
						}
						return;
					}
					
				}
			}
		}
		else if(strnicmp(device, "MODEM", 5) == 0 && m_bActiveThread) {
			if(IsLangKorean()) {
				MessageBox("MODEM device는 독립 Thread를 사용할 수 없습니다.");
			}else{
				MessageBox("Can't use Thread on MODEM device.");	
			}
			return;
		}
		else;

		if(stricmp(sProtocol, "DLL-NetWork Client") == 0 || stricmp(sProtocol, "NetWork Client Multi") == 0) {
			CString msg;
			if(m_nReadScanTime != 0) {
				if(IsLangKorean()) {
					msg.Format("[%s] 프로토콜은 읽기 주기를 0으로 설정하시기 바랍니다.", sProtocol);
				} else {
					msg.Format("ReadCycle must set to 0 at [%s] Protocol.", sProtocol);
				}
				MessageBox(msg, "ReadScan Cycle");
				return;
			}
		}
	}

	if(!SavePort())		return;
	
	CDialog::OnOK();
}

void CDialogPortEdit::OnPortEditBTNDEVICE() 
{
	// TODO: Add your control notification handler code here
	CDialogConfigDevice dialog;

	GetDlgItem(IDC_PortEdit_EDIT_DEVICE)->GetWindowText(dialog.sDeviceString, sizeof(dialog.sDeviceString));
	
	if(dialog.DoModal() == IDOK) {
		GetDlgItem(IDC_PortEdit_EDIT_DEVICE)->SetWindowText(dialog.sDeviceString);	
	}
}

void CDialogPortEdit::OnPortEditBUTTONDUALOPTION() 
{
	// TODO: Add your control notification handler code here
	CDialogConfigDualOption dialog;
	
	dialog.m_DualDevice = sDualDevice;
	dialog.m_DualCauseTimeOut = nDualCauseTimeOut;
	dialog.m_DualCauseCodeBad = nDualCauseCodeBad;
	dialog.m_sProtocol = sDualProtocol;
	dialog.m_bUseProtocol = bDualUseProtocol;
	dialog.nPort = nPort;
	dialog.m_sProtocolOption = sDualProtocolOption;

	if(dialog.DoModal() == IDOK) {
		strcpy(sDualDevice, dialog.m_DualDevice);
		nDualCauseTimeOut = dialog.m_DualCauseTimeOut;
		nDualCauseCodeBad = dialog.m_DualCauseCodeBad;
		strcpy(sDualProtocol, dialog.m_sProtocol);
		bDualUseProtocol = dialog.m_bUseProtocol;
		sDualProtocolOption = dialog.m_sProtocolOption;
	}
}

void CDialogPortEdit::OnPortEditCHKBOXDUAL() 
{
	// TODO: Add your control notification handler code here
	EnableDualItem();	
}

void CDialogPortEdit::OnBnClickedPorteditChkboxPcDual()
{
	// TODO: Add your control notification handler code here
	EnableDualComputerItem();
}

void CDialogPortEdit::OnBnClickedPorteditButtonPcDualOption()
{
	// TODO: Add your control notification handler code here
	
	CDialogComputerDualOption dialog;
	
	//dialog.m_DualDevice = sDualDevice;
	//dialog.m_DualCauseTimeOut = nDualCauseTimeOut;
	//dialog.m_sProtocol = sDualProtocol;
	//dialog.m_bUseProtocol = bDualUseProtocol;
	dialog.m_sIP = pcDual.sIP;
	dialog.m_nPort = pcDual.nPort;
	dialog.m_bThread = pcDual.bThread;
	dialog.m_nTimeOut = pcDual.nTimeOut;

	if(dialog.DoModal() == IDOK) {
		strcpy(pcDual.sIP, dialog.m_sIP);
		pcDual.nPort = dialog.m_nPort;
		pcDual.bThread = dialog.m_bThread;
		pcDual.nTimeOut = dialog.m_nTimeOut;
	}
}

void CDialogPortEdit::OnBnClickedPorteditChkboxThreadActive()
{
	// TODO: Add your control notification handler code here
	EnableThreadItem();
}
