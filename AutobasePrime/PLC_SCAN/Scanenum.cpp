// english O.K
#include "stdafx.h"
#include "plc_scan.h"
#include "resource.h"
 
//------------------------------------------------------------------------------
//	타이머 event 발생 부분
//------------------------------------------------------------------------------

BOOL FAR PASCAL EXPORT TimerEnumProc (HWND hwnd, LPARAM)
{
	if (GetWindow (hwnd, GW_OWNER))         // check for icon title.
		return 1 ;

	SendMessage (hwnd, WM_COMMAND, IDM_EVENT_TIMER, 0L);
	return 1;
}

void SendEventTimerToChild(HWND hwndClient)
{
	WNDENUMPROC            lpfnEnum ;

	lpfnEnum = (WNDENUMPROC)MakeProcInstance ((FARPROC) TimerEnumProc, hInst);
	EnumChildWindows (hwndClient, lpfnEnum, 0L);
	FreeProcInstance (lpfnEnum);
}