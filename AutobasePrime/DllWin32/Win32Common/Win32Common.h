// Win32Common.h : main header file for the Win32Common DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CWin32CommonApp
// See Win32Common.cpp for the implementation of this class
//

class CWin32CommonApp : public CWinApp
{
public:
	CWin32CommonApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
