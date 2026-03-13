#pragma once
#include "afxcmn.h"
#include "afxwin.h"

#define MAX_SEND_TAG_COUNT	10000		// 2005-09-13 ¼öÁ¤, 1000 -> 10000


// CDialogProtocolOption dialog

class CDialogProtocolOption : public CDialog
{
	DECLARE_DYNAMIC(CDialogProtocolOption)

public:
	CDialogProtocolOption(CWnd* pParent = NULL);   // standard constructor
	virtual ~CDialogProtocolOption();

// Dialog Data
	enum { IDD = IDD_DIALOG_OPTION };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
public:
	void GetRegisteredTag();
	int  GetSelectPosition();
	void saveRegisteredTag();
	void checkAndSetTagPos();
	bool tagCountIsMaxCount();

	BYTE m_port;


	virtual BOOL OnInitDialog();
protected:
	virtual void OnOK();
public:
	afx_msg void OnBnClickedButtonInsert();
	CListCtrl m_list;
	afx_msg void OnBnClickedButtonAddData();
	afx_msg void OnBnClickedButtonDelete();
	BYTE m_station;
	CSpinButtonCtrl m_spinStation;
	BOOL m_bFloatData;
	BOOL m_bReadValueFloat;
	
	int	 m_currPos;		// add 2007-08-30
	int	 m_tagType;		// add 2007-08-30, 0 = AI, 1 = AO, 2 = DI,...
	int  m_memoryType;	// add 2015-03-20 for Coil/Input status 0 = Analog, 1 = digital
	bool m_bChangeData;	// add 2015-03-20
	BOOL m_bVersion9;
	CSpinButtonCtrl m_spinSendStartAddr;
	UINT m_nSendStartPos;
	BOOL m_bUseTcp;
	BOOL m_bUseDigitalMemory;
	DWORD m_nSendStartPosCoil;
	CSpinButtonCtrl m_spinSendStartPosCoil;
	afx_msg void OnBnClickedCheckUseCoilInputMemory();
	afx_msg void OnBnClickedRadioAnalog();
	afx_msg void OnBnClickedRadioDigital();
};
