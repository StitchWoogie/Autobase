#if !defined(AFX_DIALOGMODEMSELECT_H__0A7515C3_3B33_11D2_AF9B_00001B319F36__INCLUDED_)
#define AFX_DIALOGMODEMSELECT_H__0A7515C3_3B33_11D2_AF9B_00001B319F36__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogModemSelect.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogModemSelect dialog

class CDialogModemSelect : public CDialog
{
// Construction
public:
	CDialogModemSelect(CWnd* pParent = NULL);   // standard constructor

	int nSelectItem;

// Dialog Data
	//{{AFX_DATA(CDialogModemSelect)
	enum { IDD = IDD_MODEM_SELECT };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogModemSelect)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogModemSelect)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnDblclkModemSelectLISTMODEM();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedOk();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGMODEMSELECT_H__0A7515C3_3B33_11D2_AF9B_00001B319F36__INCLUDED_)
