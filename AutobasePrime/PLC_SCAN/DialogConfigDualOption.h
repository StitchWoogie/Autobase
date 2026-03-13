#if !defined(AFX_DIALOGCONFIGDUALOPTION_H__BB7FA741_B241_11D3_809A_0000E8C33021__INCLUDED_)
#define AFX_DIALOGCONFIGDUALOPTION_H__BB7FA741_B241_11D3_809A_0000E8C33021__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogConfigDualOption.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigDualOption dialog

class CDialogConfigDualOption : public CDialog
{
// Construction
public:
	CDialogConfigDualOption(CWnd* pParent = NULL);   // standard constructor

	//CString sProtocol;

	void EnableDisable();

// Dialog Data
	//{{AFX_DATA(CDialogConfigDualOption)
	enum { IDD = IDD_CONFIG_DUAL_OPTION };
	CComboBox	m_combo;
	CString	m_DualDevice;
	int		m_DualCauseTimeOut;
	int		m_DualCauseCodeBad;
	BOOL	m_bUseProtocol;
	CString	m_sProtocol;
	
	//}}AFX_DATA

	CString m_sProtocolOption;
	int nPort;


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogConfigDualOption)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogConfigDualOption)
	afx_msg void OnConfigDualOptionBTNDEVICE();
	virtual void OnOK();
	afx_msg void OnHelp();
	virtual BOOL OnInitDialog();
	afx_msg void OnConfigDualOptionCHKBOXUSEPROTOCOL();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedConfigdualoptionBtnProtocoloption();
	afx_msg void OnEnChangeConfigdualoptionEditProtocoloption();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGCONFIGDUALOPTION_H__BB7FA741_B241_11D3_809A_0000E8C33021__INCLUDED_)
