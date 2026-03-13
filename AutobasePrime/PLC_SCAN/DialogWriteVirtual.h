#if !defined(AFX_DIALOGWRITEVIRTUAL_H__2706C2A1_B6C6_11D2_AA12_00001B319F36__INCLUDED_)
#define AFX_DIALOGWRITEVIRTUAL_H__2706C2A1_B6C6_11D2_AA12_00001B319F36__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogWriteVirtual.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogWriteVirtual dialog

class CDialogWriteVirtual : public CDialog
{
// Construction
public:
	CDialogWriteVirtual(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDialogWriteVirtual)
	enum { IDD = IDD_WRITE_VIRTUAL };
	CSpinButtonCtrl	m_SpinAddress;
	CSpinButtonCtrl	m_SpinPort;
	int		m_port;
	int	m_address;
	BOOL	m_bHex;
	CString	m_sValue;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogWriteVirtual)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogWriteVirtual)
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	afx_msg void OnHelp();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGWRITEVIRTUAL_H__2706C2A1_B6C6_11D2_AA12_00001B319F36__INCLUDED_)
