// TestDoubleInt64Memory.h : main header file for the TestDoubleInt64Memory DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CTestDoubleInt64MemoryApp
// See TestDoubleInt64Memory.cpp for the implementation of this class
//

class CTestDoubleInt64MemoryApp : public CWinApp
{
public:
	CTestDoubleInt64MemoryApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
