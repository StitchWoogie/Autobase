// MODBUS_RTU2.h : main header file for the MODBUS_RTU DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols


// CMODBUS_RTUApp
// See MODBUS_RTU2.cpp for the implementation of this class
//

class CMODBUS_RTUApp : public CWinApp
{
public:
	CMODBUS_RTUApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
