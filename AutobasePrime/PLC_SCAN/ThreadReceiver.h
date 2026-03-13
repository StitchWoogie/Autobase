#if !defined(AFX_THREADRECEIVER_H__551142A3_7BE8_11D3_8099_0000E8C33021__INCLUDED_)
#define AFX_THREADRECEIVER_H__551142A3_7BE8_11D3_8099_0000E8C33021__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// ThreadReceiver.h : header file
//



/////////////////////////////////////////////////////////////////////////////
// CThreadReceiver thread

class CThreadReceiver : public CWinThread
{
	DECLARE_DYNCREATE(CThreadReceiver)
protected:
	CThreadReceiver();           // protected constructor used by dynamic creation

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CThreadReceiver)
	public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

// Implementation
protected:
	virtual ~CThreadReceiver();

	// Generated message map functions
	//{{AFX_MSG(CThreadReceiver)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_THREADRECEIVER_H__551142A3_7BE8_11D3_8099_0000E8C33021__INCLUDED_)
