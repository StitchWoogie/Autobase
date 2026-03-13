// Test2.h : main header file for the Test2 DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CTest2App
// See Test2.cpp for the implementation of this class
//

class CTest2App : public CWinApp
{
public:
	CTest2App();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
