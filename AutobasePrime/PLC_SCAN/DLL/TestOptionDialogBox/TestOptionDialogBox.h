// TestOptionDialogBox.h : main header file for the TestOptionDialogBox DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CTestOptionDialogBoxApp
// See TestOptionDialogBox.cpp for the implementation of this class
//

class CTestOptionDialogBoxApp : public CWinApp
{
public:
	CTestOptionDialogBoxApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
