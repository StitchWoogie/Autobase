// DialogPortSelect.cpp : implementation file
//

#include "stdafx.h"

#include <glib.h>

#include "resource.h"
#include "DialogPortSelect.h"

#include "plc_scan.h"
#include "..\catlib.src\totalcfg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogPortSelect dialog


CDialogPortSelect::CDialogPortSelect(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogPortSelect::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogPortSelect)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CDialogPortSelect::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogPortSelect)
	DDX_Control(pDX, IDC_PortSelect_LIST_PORT, m_list);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogPortSelect, CDialog)
	//{{AFX_MSG_MAP(CDialogPortSelect)
	ON_NOTIFY(NM_DBLCLK, IDC_PortSelect_LIST_PORT, OnDblclkPortSelectLISTPORT)
	ON_WM_DRAWITEM()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogPortSelect message handlers

int PortSelect(int &port)
{
	CDialogPortSelect dialog;

	dialog.m_nPort = port;

	if(dialog.DoModal() != IDOK)	return 0;

	port = dialog.m_nPort;

	if(port >= MAX_PORT)	port = MAX_PORT-1;

	return 1;
}

BOOL CDialogPortSelect::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	if(IsLangKorean()) {
		m_list.InsertColumn(0, "번호",    LVCFMT_LEFT, 40);
		m_list.InsertColumn(1, "포트설명",    LVCFMT_LEFT, 100);
		m_list.InsertColumn(2, "디바이스",      LVCFMT_LEFT, 150);
		m_list.InsertColumn(3, "프로토콜",      LVCFMT_LEFT, 120);
	}
	else if(IsLangChinese()) {
		CString buf;
		GetResourceString(IDS_No, buf);
		m_list.InsertColumn(0, buf,    LVCFMT_LEFT, 40);
		GetResourceString(IDS_PortDescription, buf);
		m_list.InsertColumn(1, buf,    LVCFMT_LEFT, 100);
		GetResourceString(IDS_Device, buf);
		m_list.InsertColumn(2, buf,      LVCFMT_LEFT, 150);
		GetResourceString(IDS_Protocol, buf);
		m_list.InsertColumn(3, buf,      LVCFMT_LEFT, 120);
	}
	else {
		m_list.InsertColumn(0, "No",    LVCFMT_LEFT, 40);
		m_list.InsertColumn(1, "Port description",    LVCFMT_LEFT, 100);
		m_list.InsertColumn(2, "Device",      LVCFMT_LEFT, 150);
		m_list.InsertColumn(3, "Protocol",      LVCFMT_LEFT, 120);
	}

	AutoBasePlcScanListCtrlConfigLoad(m_list, "Port Select");

	CString sTitle;
	CString sDevice;
	CString sProtocol;

	int i;
	GLOBAL_PORT_STRUCT *pt;
	
	for(i = 0; i < MAX_PORT; i++) {
		pt = &portBuf[i];

		sTitle = pt->sTitle;

		if(pt->bActiveFlag) {
			sTitle = pt->sTitle;

			char device[80];
			
			PlcDeviceGetInfoString(&pt->local.device, device);
			sDevice = device;

			sProtocol = pt->sScanProtocol;
		}
		else {
			if(IsLangKorean()) {
				sDevice = "사용안함";
			} else {
				sDevice = "Not used";
			}
			sTitle = pt->sTitle;
			//sDevice = "";
			sProtocol = "";
		}

		m_list.InsertItem(i, "");
		ChangeOneItem(i, sTitle, sDevice, sProtocol);
	}

	m_list.SetItemState(m_nPort, LVIS_SELECTED, LVIS_SELECTED);
	m_list.EnsureVisible(m_nPort, FALSE);
	//SendMessage(GetDlgItem(IDC_PortSelect_LIST_PORT), LB_SETCURSEL, nPort, 0L);
		
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogPortSelect::OnOK() 
{
	// TODO: Add extra validation here

	int retn = GetSelectedPosition();

	if(retn == -1) {
		if(IsLangKorean()) {
			MessageBox("수정할 포트를 하나 선택하세요.", "선택오류");
		}else {
			MessageBox("Select port to modify.", "Selection error");
		}
		return;
	}

	m_nPort = retn;

	AutoBasePlcScanListCtrlConfigSave(m_list, "Port Select");
	
	CDialog::OnOK();
}

void CDialogPortSelect::ChangeOneItem(int pos, const char *title, const char *device, const char *protocol)
{
	int cursor = pos;
	LV_ITEM item;
	char buf[MAXPATH];

	memset(&item, 0, sizeof(LV_ITEM));

	sprintf(buf, "%03d", pos);
	item.mask = LVIF_TEXT; 
	item.iItem = cursor;
	item.iSubItem = 0;
	item.pszText = buf;
	m_list.SetItem(&item);

	item.mask = LVIF_TEXT; 
	item.iItem = cursor;
	item.iSubItem = 1;
	item.pszText = buf;
	strcpy(buf, title);
	m_list.SetItem(&item);

	item.mask = LVIF_TEXT; 
	item.iItem = cursor;
	item.iSubItem = 2;
	item.pszText = buf;
	strcpy(buf, device);
	m_list.SetItem(&item);

	item.mask = LVIF_TEXT; 
	item.iItem = cursor;
	item.iSubItem = 3;
	item.pszText = buf;
	strcpy(buf, protocol);
	m_list.SetItem(&item);
}

int CDialogPortSelect::GetSelectedPosition()
{
	int i;

	for(i = 0; i < m_list.GetItemCount(); i++) {
		if(m_list.GetItemState(i, LVIS_SELECTED))	return i;
	}

	return -1;
}

void CDialogPortSelect::OnDblclkPortSelectLISTPORT(NMHDR* pNMHDR, LRESULT* pResult) 
{
	// TODO: Add your control notification handler code here
	OnOK();
	
	*pResult = 0;
}

void CDialogPortSelect::OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct) 
{
	// TODO: Add your message handler code here and/or call default
	if(nIDCtl == IDC_PortSelect_LIST_PORT)
		DrawUserListCtrl(lpDrawItemStruct, m_list);
	
	//CDialog::OnDrawItem(nIDCtl, lpDrawItemStruct);
}
