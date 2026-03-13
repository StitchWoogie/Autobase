#if !defined(AFX_DIALOGCONFIGTELOPTION_H__6913D462_2AFF_11D2_98CB_00001B319F36__INCLUDED_)
#define AFX_DIALOGCONFIGTELOPTION_H__6913D462_2AFF_11D2_98CB_00001B319F36__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogConfigTelOption.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigTelOption dialog

class CDialogConfigTelOption : public CDialog
{
// Construction
public:
	CDialogConfigTelOption(int cicle, int time, char *tel, CWnd* pParent = NULL);   // standard constructor
	void EnableDisable();

// Dialog Data
	//{{AFX_DATA(CDialogConfigTelOption)
	enum { IDD = IDD_CONFIG_TEL_OPTION };
	int		m_cicle;
	int		m_time;
	CString	m_tel_num;
	BOOL	m_autoConnection;
	BOOL	m_bAutoDisconnectOnManual;
	int		m_timeOnManual;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogConfigTelOption)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogConfigTelOption)
	virtual void OnOK();
	afx_msg void OnConfigTelOptionCHKBOXAUTOCONNECT();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGCONFIGTELOPTION_H__6913D462_2AFF_11D2_98CB_00001B319F36__INCLUDED_)
