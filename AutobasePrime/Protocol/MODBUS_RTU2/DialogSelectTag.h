#pragma once
#include "afxcmn.h"


// CDialogSelectTag dialog

class CDialogSelectTag : public CDialog
{
	DECLARE_DYNAMIC(CDialogSelectTag)

public:
	CDialogSelectTag(CWnd* pParent = NULL);   // standard constructor
	virtual ~CDialogSelectTag();

// Dialog Data
	enum { IDD = IDD_DIALOG_SELECT_TAG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
public:
	CListCtrl m_list;
	virtual BOOL OnInitDialog();
protected:
	virtual void OnOK();
public:
	afx_msg void OnBnClickedRadioAnalogInput();
	afx_msg void OnBnClickedRadioDigitalInput();

	void GetCurrentTag();
	int GetSelectPosition();
	bool isTagTypeMached(int nTagType);
	void checkCurrPos();
	
	int			m_tagType;	// 0 = AI, 1= AO, 2= DI, ...
	int			m_currPos;
	char		mTagName[80];
	char		mTagDesc[80];
	afx_msg void OnNMDblclkListSelectTag(NMHDR *pNMHDR, LRESULT *pResult);
};
