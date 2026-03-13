#if !defined(AFX_DIALOGMODEMOPTION_H__0A7515C4_3B33_11D2_AF9B_00001B319F36__INCLUDED_)
#define AFX_DIALOGMODEMOPTION_H__0A7515C4_3B33_11D2_AF9B_00001B319F36__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogModemOption.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogModemOption dialog

class CDialogModemOption : public CDialog
{
// Construction
	int nModemNo;
public:
	CDialogModemOption(int modem_no, CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDialogModemOption)
	enum { IDD = IDD_MODEM_OPTION };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogModemOption)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogModemOption)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGMODEMOPTION_H__0A7515C4_3B33_11D2_AF9B_00001B319F36__INCLUDED_)
