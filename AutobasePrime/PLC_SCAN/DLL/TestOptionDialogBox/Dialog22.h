#pragma once


// CDialog22 dialog

class CDialog22 : public CDialog
{
	DECLARE_DYNAMIC(CDialog22)

public:
	CDialog22(CWnd* pParent = NULL);   // standard constructor
	virtual ~CDialog22();

// Dialog Data
	enum { IDD = IDD_DIALOG2 };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support

	DECLARE_MESSAGE_MAP()
};
