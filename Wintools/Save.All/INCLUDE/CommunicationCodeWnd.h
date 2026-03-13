#if !defined(AFX_COMMUNICATIONCODEWND_H__B47C52C1_A27B_11D3_809A_0000E8C33021__INCLUDED_)
#define AFX_COMMUNICATIONCODEWND_H__B47C52C1_A27B_11D3_809A_0000E8C33021__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// CommunicationCodeWnd.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCommunicationCodeWnd window

class CCommunicationCodeWnd : public CWnd
{
	int cxChar;
	int cyChar;
	int nPosX;
	int nPosY;
	RECT rClient;
	
	char cFirstCommunicationFlag;
	void DisplayOneChar(int ch);
	void NextLine(CDC *dc);
// Construction
public:
	char cDisplayMethod; // 0 = hex, 1 = ascii, 2 = decimal
	char bPause;

	CCommunicationCodeWnd();
	BOOL Create(CWnd* pParentWnd, int id);
	BOOL CreateWithTitle(CWnd* pParentWnd, char *title, int id);
	BOOL CreateWithTitle(HWND hwndParent, char *title, int id);
	void DisplayCode(const char *buf, int size);
	void DisplayNextLine();
// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCommunicationCodeWnd)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CCommunicationCodeWnd();

	// Generated message map functions
protected:
	//{{AFX_MSG(CCommunicationCodeWnd)
	afx_msg void OnPaint();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COMMUNICATIONCODEWND_H__B47C52C1_A27B_11D3_809A_0000E8C33021__INCLUDED_)
