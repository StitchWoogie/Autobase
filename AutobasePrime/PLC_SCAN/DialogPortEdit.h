#if !defined(AFX_DIALOGPORTEDIT_H__72D91881_344E_11D3_B241_00C026CAD466__INCLUDED_)
#define AFX_DIALOGPORTEDIT_H__72D91881_344E_11D3_B241_00C026CAD466__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// DialogPortEdit.h : header file
//

#include "plc_scan.h"

/////////////////////////////////////////////////////////////////////////////
// CDialogPortEdit dialog

class CDialogPortEdit : public CDialog
{
// Construction
public:
	CDialogPortEdit(CWnd* pParent = NULL);   // standard constructor
	~CDialogPortEdit();   // standard constructor
	char sProtocol[80];
	char *editBuf;
	char sTelNumber[20];
	int  nTelConnectCicle;
	int  nTelConnectingTime;
	char bTelAutoConnection;
	int  nTelConnectingTimeOnManual;

	char sDualDevice[80];
	char sDualProtocol[80];
	CString sDualProtocolOption;
	char bDualUseProtocol;


	int	 nDualCauseTimeOut;	// 통신 시간 초과가 연속해서 ?회 이상 발생하면 듀얼 Device 작동.
	int	 nDualCauseCodeBad;	// 통신 시간 초과가 연속해서 ?회 이상 발생하면 듀얼 Device 작동.

	void EnableItem(); 
	void EnableDualItem();
	void EnableDualComputerItem();
	void EnableThreadItem();
	int  SavePort();

	COMPUTER_DUAL_FILE pcDual;

// Dialog Data
	//{{AFX_DATA(CDialogPortEdit)
	enum { IDD = IDD_PORTEDIT };
	BOOL	bActiveFlag;
	int		nPort;
	int		wBufLengthDWORD;
	int		wBufLengthFLOAT;
	int		wBufLengthWORD;
	CString	sDescription;
	CString	sDevice;
	CString	sProtocolOption;
	int		wTimeOutRead;
	int		wTimeOutWrite;
	int		m_nReadScanTime;
	int		m_nWriteScanTime;
	BOOL	bDualActive;
	BOOL	m_bActiveThread;
	int		m_nThreadCycle;
	int		wBufLengthSTRING;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDialogPortEdit)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CDialogPortEdit)
	virtual BOOL OnInitDialog();
	afx_msg void OnPortEditCHKBOXPORTACTIVE();
	afx_msg void OnHelp();
	afx_msg void OnPortEditBTNPROTOCOLOPTION();
	afx_msg void OnPortEditBUTTONTELOPTION();
	virtual void OnOK();
	virtual void OnCancel(); //250828 PSU 추가
	afx_msg void OnPortEditBTNDEVICE();
	afx_msg void OnPortEditBUTTONDUALOPTION();
	afx_msg void OnPortEditCHKBOXDUAL();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedPorteditChkboxPcDual();
	BOOL m_bComputerDualActive;
	afx_msg void OnBnClickedPorteditButtonPcDualOption();
	afx_msg void OnBnClickedPorteditChkboxThreadActive();
	BOOL m_bUseStationInfo;
	int wBufLengthDOUBLE;
	int wBufLengthINT64;
	BOOL m_bUseDeviceInfo;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DIALOGPORTEDIT_H__72D91881_344E_11D3_B241_00C026CAD466__INCLUDED_)
