// DialogWriteVirtual.cpp : implementation file
//

#include "stdafx.h"

#include <glib.h>

#include "resource.h"
#include "DialogWriteVirtual.h"
#include "plc_scan.h"
#include "protocol\\pro_lib.h"

#include "..\catlib.src\totalcfg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogWriteVirtual dialog


CDialogWriteVirtual::CDialogWriteVirtual(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogWriteVirtual::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogWriteVirtual)
	m_port = 0;
	m_address = 0;
	m_bHex = FALSE;
	m_sValue = _T("");
	//}}AFX_DATA_INIT
}


void CDialogWriteVirtual::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogWriteVirtual)
	DDX_Control(pDX, IDC_WriteVirtual_SPIN_ADDRESS, m_SpinAddress);
	DDX_Control(pDX, IDC_WriteVirtual_SPIN_PORT, m_SpinPort);
	DDX_Text(pDX, IDC_WriteVirtual_EDIT_PORT, m_port);
	DDV_MinMaxInt(pDX, m_port, 0, 255);
	DDX_Text(pDX, IDC_WriteVirtual_EDIT_ADDRESS, m_address);
	DDV_MinMaxInt(pDX, m_address, 0, 99999);
	DDX_Check(pDX, IDC_CHECK_TO_HEX, m_bHex);
	DDX_Text(pDX, IDC_WriteVirtual_EDIT_VALUE, m_sValue);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CDialogWriteVirtual, CDialog)
	//{{AFX_MSG_MAP(CDialogWriteVirtual)
	ON_BN_CLICKED(IDHELP, OnHelp)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

bool CheckWordValue(double value);

/////////////////////////////////////////////////////////////////////////////
// CDialogWriteVirtual message handlers

void CDialogWriteVirtual::OnOK() 
{
	// TODO: Add extra validation here
	if(UpdateData() == 0)	return;	// error

	

	if(m_port >= nPortHap) {
		MessageBox("포트가 설정되지 않았습니다.", "포트 번호 오류");
		return;
	}
	int pos;
	LOCAL_PORT_STRUCT *pt = &portBuf[m_port].local;
	long double value = 0;
	unsigned __int64 value_64;

	if(m_bHex) {
		CString imsi;

		CommaBlockString comma;
		comma.Set(m_sValue);
		comma.GetHexUINT64(value_64);
		value = (double)value_64;
	}
	else {
		value = atof(m_sValue);
		value_64 = _atoi64(m_sValue);
	}

	if(!CheckWordValue((double)value))	return;

	pos = GetRadioPosition(m_hWnd, IDC_WriteVirtual_RADIO_MEMORY0, 6);
	if(pos == 0) {
		PokeNewWORD(pt, m_address, (WORD)value);
	}
	else if(pos == 1) {
		PokeNewFLOAT(pt, m_address, (float)value);
	}
	else if(pos == 2) {
		PokeNewDWORD(pt, m_address, (DWORD)value);
	}
	else if(pos == 3) {
		PokeNewSTRING(pt, m_address, m_sValue);
	}
	else if(pos == 4) {
		PokeNewDOUBLE(pt, m_address, value);
	}
	else if(pos == 5) {
		PokeNewINT64(pt, m_address, value_64);
	}
	else;
	

	//CDialog::OnOK();
}

void ExecuteWriteVirtualDialog(HWND hwnd)
{
	CDialogWriteVirtual dialog;

	dialog.DoModal();
}

BOOL CDialogWriteVirtual::OnInitDialog() 
{
	CDialog::OnInitDialog();
	m_SpinPort.SetRange(0, 255);
	m_SpinAddress.SetRange32(0, 99999);
	
	// TODO: Add extra initialization here
	SetRadioPosition(m_hWnd, IDC_WriteVirtual_RADIO_MEMORY0, 4, 0);


	if(eOemType == OEM_TYPE_SBAS) {
		GetDlgItem(IDHELP)->ShowWindow(SW_HIDE);

		GetDlgItem(IDC_WriteVirtual_EDIT_PORT)->SendMessage(EM_SETREADONLY ,1 ,0);
		GetDlgItem(IDC_WriteVirtual_EDIT_ADDRESS)->SendMessage(EM_SETREADONLY ,1 ,0);

		GetDlgItem(IDC_WriteVirtual_RADIO_MEMORY3)->ShowWindow(SW_HIDE);
		GetDlgItem(IDC_WriteVirtual_RADIO_MEMORY4)->ShowWindow(SW_HIDE);
		GetDlgItem(IDC_WriteVirtual_RADIO_MEMORY5)->ShowWindow(SW_HIDE);
		//::SendMessage(GetDlgItem(IDC_WriteVirtual_EDIT_PORT)->GetSafeHwnd(), EM_SETREADONLY, !ES_READONLY, 0); 
		//GetDlgItem(IDC_WriteVirtual_EDIT_PORT)->
		
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogWriteVirtual::OnHelp() 
{
	// TODO: Add your control notification handler code here
	PlcScanHelp(m_hWnd, "PLC_SCANVirtualWrite.htm");	
}
