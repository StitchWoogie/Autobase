#if !defined(AFX_DIALOGMULTICONFIG_H__7FA8C3E0_6EE8_4BF0_B08C_A2EF7F865AA1__INCLUDED_)
#define AFX_DIALOGMULTICONFIG_H__7FA8C3E0_6EE8_4BF0_B08C_A2EF7F865AA1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DialogMultiConfig.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogMultiConfig dialog

class CDialogMultiConfig : public CDialog
{
// Construction
public:
	CDialogMultiConfig(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDialogMultiConfig)
	enum { IDD = IDD_MULTI_CONFIG };
	CComboBox	m_combo;
	int		m_nPortFrom;
	int		m_nPortTo;
	CString	m_sProtocolOption;
	CString	m_sRead;
	int		m_nSizeDWORD;
	int		m_nSizeFLOAT;
	int		m_nSizeWORD;
	BOOL	m_bThread;
	CString	m_sDevice;
	BOOL	m_bActive;
	int		m_nReadCycle;
	int		m_nWriteCycle;
	int		m_nThreadCycle;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogMultiConfig)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogMultiConfig)
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedMulticonfigCheckThread();
	afx_msg void OnBnClickedButtonprotocoloption();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGMULTICONFIG_H__7FA8C3E0_6EE8_4BF0_B08C_A2EF7F865AA1__INCLUDED_)
