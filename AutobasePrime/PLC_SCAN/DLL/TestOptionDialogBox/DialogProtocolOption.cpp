// DialogProtocolOption.cpp : implementation file
//

#include "stdafx.h"
#include "TestOptionDialogBox.h"
#include "DialogProtocolOption.h"


// CDialogProtocolOption dialog

IMPLEMENT_DYNAMIC(CDialogProtocolOption, CDialog)

CDialogProtocolOption::CDialogProtocolOption(CWnd* pParent /*=NULL*/)
	: CDialog(CDialogProtocolOption::IDD, pParent)
{

}

CDialogProtocolOption::~CDialogProtocolOption()
{
}

void CDialogProtocolOption::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CDialogProtocolOption, CDialog)
END_MESSAGE_MAP()


// CDialogProtocolOption message handlers
