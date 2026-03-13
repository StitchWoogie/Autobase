#include "stdafx.h"
#include <glib.h>

int ExecuteDialogBox(HINSTANCE hinst, LPCTSTR lpszDlgTemp, HWND hwnd, DLGPROC dlgproc)
{
	DLGPROC lpfnDlgProc;
	int retn;

	lpfnDlgProc = (DLGPROC)MakeProcInstance(dlgproc, hinst);
	retn = DialogBox(hinst, lpszDlgTemp, hwnd, lpfnDlgProc);
	FreeProcInstance(lpfnDlgProc);

	return retn;
}



