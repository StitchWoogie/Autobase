// DialogComputerDualOption.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "DialogComputerDualOption.h"


// CDialogComputerDualOption dialog

IMPLEMENT_DYNAMIC(CDialogComputerDualOption, CDialog)
CDialogComputerDualOption::CDialogComputerDualOption(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogComputerDualOption::IDD, pParent)
	, m_sIP(_T(""))
	, m_nPort(0)
	, m_bThread(FALSE)
	, m_nTimeOut(0)
{
}

CDialogComputerDualOption::~CDialogComputerDualOption()
{
}

void CDialogComputerDualOption::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_ComputerDualOption_EDIT_IP, m_sIP);
	DDX_Text(pDX, IDC_ComputerDualOption_EDIT_PORT, m_nPort);
	DDX_Check(pDX, IDC_ComputerDualOption_CHKBOX_THREAD, m_bThread);
	DDX_Text(pDX, IDC_ComputerDualOption_EDIT_TIMEOUT, m_nTimeOut);
}


BEGIN_MESSAGE_MAP(CDialogComputerDualOption, CDialog)
END_MESSAGE_MAP()


// CDialogComputerDualOption message handlers
