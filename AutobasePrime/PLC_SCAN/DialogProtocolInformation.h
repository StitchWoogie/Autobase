#if !defined(AFX_DIALOGPROTOCOLINFORMATION_H__DADCBC64_A1B1_40F7_A356_D11C651DB557__INCLUDED_)
#define AFX_DIALOGPROTOCOLINFORMATION_H__DADCBC64_A1B1_40F7_A356_D11C651DB557__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DialogProtocolInformation.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogProtocolInformation dialog

class CDialogProtocolInformation : public CDialog
{
// Construction
public:
	CDialogProtocolInformation(CWnd* pParent = NULL);   // standard constructor

	void FillList();

// Dialog Data
	//{{AFX_DATA(CDialogProtocolInformation)
	enum { IDD = IDD_PROTOCOL_INFORMATION };
	CListCtrl	m_list;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogProtocolInformation)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogProtocolInformation)
	virtual BOOL OnInitDialog();
	afx_msg void OnButtonReload();
	virtual void OnCancel();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGPROTOCOLINFORMATION_H__DADCBC64_A1B1_40F7_A356_D11C651DB557__INCLUDED_)
