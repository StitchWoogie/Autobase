#if !defined(AFX_DIALOGCONFIGTELEDEVICE_H__8FD02601_B3D7_11D3_809A_0000E8C33021__INCLUDED_)
#define AFX_DIALOGCONFIGTELEDEVICE_H__8FD02601_B3D7_11D3_809A_0000E8C33021__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogConfigTeleDevice.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogConfigTeleDevice dialog

class CDialogConfigTeleDevice : public CDialog
{
// Construction
public:
	CDialogConfigTeleDevice(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDialogConfigTeleDevice)
	enum { IDD = IDD_CONFIG_TELE_DEVICE };
	CString	m_ConnectCommand;
	CString	m_InitCommand;
	int		m_ConnectWaitTimeOut;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogConfigTeleDevice)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogConfigTeleDevice)
	afx_msg void OnHelp();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGCONFIGTELEDEVICE_H__8FD02601_B3D7_11D3_809A_0000E8C33021__INCLUDED_)
