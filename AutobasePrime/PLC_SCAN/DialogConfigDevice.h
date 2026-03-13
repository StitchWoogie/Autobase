#if !defined(AFX_DIALOGCONFIGDEVICE_H__56EC2B01_97B1_11D3_B248_00C026CAD466__INCLUDED_)
#define AFX_DIALOGCONFIGDEVICE_H__56EC2B01_97B1_11D3_B248_00C026CAD466__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogConfigDevice.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigDevice dialog

class CDialogConfigDevice : public CDialog
{
// Construction
public:
	CDialogConfigDevice(CWnd* pParent = NULL);   // standard constructor
	char sDeviceString[160];
	char sExtraString[80];

	int nDeviceType;
	int nComPort;
	int nComBaud;
	int nComParity;
	int nComDataBit;
	int nComStopBit;
	int nComTxMethod;
	int nComRxMethod;
	int nComRtsMethod;

	bool bUseOnlyCom;

	void EnableDisable();

// Dialog Data
	//{{AFX_DATA(CDialogConfigDevice)
	enum { IDD = IDD_CONFIG_DEVICE };
	CComboBox	m_comboComBaud;
	CComboBox	m_comboComPort;
	int		m_ComReadDelay;
	int		m_ComWriteDelay;
	CString	m_ipAddress;
	int		m_ipPort;
	int		m_StartRtsReadDelay;
	int		m_StartRtsWriteDelay;
	CString	m_sShareName;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogConfigDevice)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogConfigDevice)
	virtual BOOL OnInitDialog();
	afx_msg void OnConfigDeviceRADIODEVICETYPE0();
	afx_msg void OnConfigDeviceRADIODEVICETYPE1();
	afx_msg void OnConfigDeviceRADIODEVICETYPE2();
	afx_msg void OnConfigDeviceRADIODEVICETYPE3();
	afx_msg void OnConfigDeviceRADIODEVICETYPE4();
	afx_msg void OnConfigDeviceRADIODEVICETYPE5();
	afx_msg void OnConfigDeviceRADIODEVICETYPE6();
	afx_msg void OnConfigDeviceRADIODEVICETYPE7();
	afx_msg void OnConfigDeviceRADIODEVICETYPE8();
	afx_msg void OnConfigDeviceRADIODEVICETYPE9();
	virtual void OnOK();
	afx_msg void OnHelp();
	afx_msg void OnConfigDeviceBTNTELEDEVICEOPTION();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGCONFIGDEVICE_H__56EC2B01_97B1_11D3_B248_00C026CAD466__INCLUDED_)
