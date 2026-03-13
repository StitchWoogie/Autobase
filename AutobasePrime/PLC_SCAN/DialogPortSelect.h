#if !defined(AFX_DIALOGPORTSELECT_H__E8517BB0_9798_475E_B3EA_AEBD4F1F7C44__INCLUDED_)
#define AFX_DIALOGPORTSELECT_H__E8517BB0_9798_475E_B3EA_AEBD4F1F7C44__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DialogPortSelect.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CDialogPortSelect dialog

class CDialogPortSelect : public CDialog
{
// Construction
public:
	CDialogPortSelect(CWnd* pParent = NULL);   // standard constructor
	void ChangeOneItem(int pos, const char *title, const char *device, const char *protocol);
	int GetSelectedPosition();

	int m_nPort;

// Dialog Data
	//{{AFX_DATA(CDialogPortSelect)
	enum { IDD = IDD_PORT_SELECT };
	CListCtrl	m_list;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogPortSelect)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogPortSelect)
	virtual BOOL OnInitDialog();
	virtual void OnOK();
	afx_msg void OnDblclkPortSelectLISTPORT(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGPORTSELECT_H__E8517BB0_9798_475E_B3EA_AEBD4F1F7C44__INCLUDED_)
