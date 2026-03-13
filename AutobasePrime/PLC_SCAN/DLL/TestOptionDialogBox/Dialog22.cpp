// Dialog22.cpp : implementation file
//

#include "stdafx.h"
#include "TestOptionDialogBox.h"
#include "Dialog22.h"


// CDialog22 dialog

IMPLEMENT_DYNAMIC(CDialog22, CDialog)

CDialog22::CDialog22(CWnd* pParent /*=NULL*/)
	: CDialog(CDialog22::IDD, pParent)
{

}

CDialog22::~CDialog22()
{
}

void CDialog22::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CDialog22, CDialog)
END_MESSAGE_MAP()


// CDialog22 message handlers
