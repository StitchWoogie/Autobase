#if !defined(AFX_DIALOGCONFIGSCANSERVER_H__8E2A3341_BF68_11D2_AA12_00001B319F36__INCLUDED_)
#define AFX_DIALOGCONFIGSCANSERVER_H__8E2A3341_BF68_11D2_AA12_00001B319F36__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogConfigScanServer.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigScanServer dialog

class CDialogConfigScanServer : public CDialog
{
// Construction
	void EnableDisable(); 
public:
	int m_device_type;

	CDialogConfigScanServer(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDialogConfigScanServer)
	enum { IDD = IDD_CONFIG_SCAN_SERVER };
	CString	m_modem_init_command;
	UINT	m_tcpip_port;
	CString	m_com_port;
	BOOL	m_bThread;
	int		m_nSendDelay;
	BOOL	m_bSendOnlyChange;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogConfigScanServer)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogConfigScanServer)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnConfigScanServerBUTTONCOMPORT();
	afx_msg void OnConfigScanServerRADIOMETHOD0();
	afx_msg void OnConfigScanServerRADIOMETHOD1();
	afx_msg void OnConfigScanServerRADIOMETHOD2();
	afx_msg void OnConfigScanServerRADIOMETHOD3();
	afx_msg void OnConfigScanServerRADIOMETHOD4();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	int m_nBlockSize;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGCONFIGSCANSERVER_H__8E2A3341_BF68_11D2_AA12_00001B319F36__INCLUDED_)
