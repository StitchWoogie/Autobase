// ThreadReceiver.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "ThreadReceiver.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CThreadReceiver

IMPLEMENT_DYNCREATE(CThreadReceiver, CWinThread)

CThreadReceiver::CThreadReceiver()
{
}

CThreadReceiver::~CThreadReceiver()
{
}

BOOL CThreadReceiver::InitInstance()
{
	// TODO:  perform and per-thread initialization here
	return TRUE;
}

int CThreadReceiver::ExitInstance()
{
	// TODO:  perform any per-thread cleanup here
	return CWinThread::ExitInstance();
}

BEGIN_MESSAGE_MAP(CThreadReceiver, CWinThread)
	//{{AFX_MSG_MAP(CThreadReceiver)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CThreadReceiver message handlers
