#pragma once


// CDialogComputerDualOption dialog

class CDialogComputerDualOption : public CDialog
{
	DECLARE_DYNAMIC(CDialogComputerDualOption)

public:
	CDialogComputerDualOption(CWnd* pParent = NULL);   // standard constructor
	virtual ~CDialogComputerDualOption();

// Dialog Data
	enum { IDD = IDD_COMPUTER_DUAL_OPTION };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
public:
	CString m_sIP;
	int m_nPort;
	BOOL m_bThread;
	int m_nTimeOut;
};
