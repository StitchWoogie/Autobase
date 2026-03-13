#if !defined(AFX_DIALOGSYSTEMINFO_H__038F9D29_459A_11D3_B244_00C026CAD466__INCLUDED_)
#define AFX_DIALOGSYSTEMINFO_H__038F9D29_459A_11D3_B244_00C026CAD466__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogSystemInfo.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogSystemInfo dialog

class CDialogSystemInfo : public CDialog
{
// Construction
public:
	CDialogSystemInfo(CWnd* pParent = NULL);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDialogSystemInfo)
	enum { IDD = IDD_SYSTEM_INFO };
	CEdit	m_edit;
	CButton	m_list;
	int		m_wait;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogSystemInfo)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogSystemInfo)
	virtual BOOL OnInitDialog();
	virtual void OnCancel();
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGSYSTEMINFO_H__038F9D29_459A_11D3_B244_00C026CAD466__INCLUDED_)
