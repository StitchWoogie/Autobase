// DialogProtocolInformation.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogProtocolInformation.h"
#include "protocol\pro_main.h"

#include "..\catlib.src\totalcfg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDialogProtocolInformation dialog


CDialogProtocolInformation::CDialogProtocolInformation(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogProtocolInformation::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDialogProtocolInformation)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CDialogProtocolInformation::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDialogProtocolInformation)
	DDX_Control(pDX, IDC_LIST_PROTOCOL, m_list);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDialogProtocolInformation, CDialog)
	//{{AFX_MSG_MAP(CDialogProtocolInformation)
	ON_BN_CLICKED(IDC_BUTTON_RELOAD, OnButtonReload)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDialogProtocolInformation message handlers

void ViewProtocolInformation()
{
	CDialogProtocolInformation dialog;

	dialog.DoModal();
}

BOOL CDialogProtocolInformation::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	if(IsLangKorean()) {
		m_list.InsertColumn(0, "번호", LVCFMT_LEFT, 40);
		m_list.InsertColumn(1, "파일명", LVCFMT_LEFT, 100);
		m_list.InsertColumn(2, "프로토콜", LVCFMT_LEFT, 140);
		m_list.InsertColumn(3, "파일날짜", LVCFMT_LEFT, 100);
		m_list.InsertColumn(4, "파일크기", LVCFMT_RIGHT, 60);
		m_list.InsertColumn(5, "버전", LVCFMT_RIGHT, 40);
	}
	else {
		m_list.InsertColumn(0, "No", LVCFMT_LEFT, 40);
		m_list.InsertColumn(1, "filename", LVCFMT_LEFT, 100);
		m_list.InsertColumn(2, "protocol", LVCFMT_LEFT, 140);
		m_list.InsertColumn(3, "DateTime", LVCFMT_LEFT, 100);
		m_list.InsertColumn(4, "Size", LVCFMT_RIGHT, 60);
		m_list.InsertColumn(5, "Version", LVCFMT_RIGHT, 40);
	}

	AutoBasePlcScanListCtrlConfigLoad(m_list, "Dialog Protocol Information");

	FillList();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogProtocolInformation::FillList() 
{
	m_list.DeleteAllItems();	
	
	DWORD l;
	DLL_PROTOCOL_LIST item;
	CString buf;
	
	for(l = 0; l < blockProtocolList.GetCount(); l++) {
		blockProtocolList.GetBlock(&item, l);

		m_list.InsertItem(l, "");
		
		buf.Format("%d", l+1);
		m_list.SetItemText(l, 0, buf);
		m_list.SetItemText(l, 1, item.filename);
		m_list.SetItemText(l, 2, item.title);

		CTime time(item.ft);
		buf.Format("%04d-%02d-%02d %02d:%02d:%02d", time.GetYear(), time.GetMonth(), time.GetDay(), time.GetHour(), time.GetMinute(), time.GetSecond());
		m_list.SetItemText(l, 3, buf);
		buf.Format("%d", item.size);
		m_list.SetItemText(l, 4, buf);
		buf.Format("%d.%d", item.VersionMajor, item.VersionMinor);
		m_list.SetItemText(l, 5, buf);
	}

	#define	MAX_PROTOCOL_NAME_DEFINE	29

	extern PROTOCOL_NAME_DEFINE protocolNameDefine[MAX_PROTOCOL_NAME_DEFINE];
	PROTOCOL_NAME_DEFINE *pre;
	int pos;

	for(l = 0; l < MAX_PROTOCOL_NAME_DEFINE; l++) {
		pre = &protocolNameDefine[l];

		pos = l+blockProtocolList.GetCount();

		m_list.InsertItem(pos, "");
		
		buf.Format("%d", pos+1);
		m_list.SetItemText(pos, 0, buf);
		m_list.SetItemText(pos, 1, "내장형");
		m_list.SetItemText(pos, 2, pre->name);
	}
}

void CDialogProtocolInformation::OnButtonReload() 
{
	// TODO: Add your control notification handler code here
	void CompareDllProtocolList(HWND hwnd, char all_update);

	m_list.DeleteAllItems();
	CompareDllProtocolList(m_hWnd, TRUE);

	FillList();

	CString msg;

	if(IsLangKorean()) {
		msg.Format("총 %d개의 DLL 파일을 찾았습니다.", blockProtocolList.GetCount());
		MessageBox(msg, "목록 갱신");
	}else {
		msg.Format("DLL file found.(Total:%d)", blockProtocolList.GetCount());
		MessageBox(msg, "Update list");
	}
}

void CDialogProtocolInformation::OnCancel() 
{
	// TODO: Add extra cleanup here

	AutoBasePlcScanListCtrlConfigSave(m_list, "Dialog Protocol Information");
	
	CDialog::OnCancel();
}
