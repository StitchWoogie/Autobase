#pragma once


// CDialogProtocolOption dialog

class CDialogProtocolOption : public CDialog
{
	DECLARE_DYNAMIC(CDialogProtocolOption)

public:
	CDialogProtocolOption(CWnd* pParent = NULL);   // standard constructor
	virtual ~CDialogProtocolOption();

// Dialog Data
	enum { IDD = IDD_DIALOG1 };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
};
