#include "stdafx.h"
#include <tools.h>
#include <glib.h>
#include <kwl\kdialog.h>

static char bDialogInitFlag = OFF;

BOOL CALLBACK StandardDialogProc( HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam )
{
	switch( message ) {
		case WM_INITDIALOG:
			SetWindowLong(hDlg, DWL_USER, lParam);
			bDialogInitFlag = ON;
			break;
		default:
			if(bDialogInitFlag == OFF)	return FALSE;
			break;
	}

	KDialog *dialog = (KDialog*)GetWindowLong(hDlg, DWL_USER);

	return dialog->DialogProc(hDlg, message, wParam, lParam);
}

KDialog :: KDialog()
{
	bCenterFlag = OFF;
	hwndDlg = NULL;
	hwndOwner = NULL;
}

int KDialog :: run(HWND hwnd, WORD id, HINSTANCE hInstance)
{
	bDialogInitFlag = OFF;
	hwndOwner = hwnd;
#if	defined (_WIN32)
	return DialogBoxParam(hInstance, MAKEINTRESOURCE(id), hwnd, (DLGPROC)StandardDialogProc, (LPARAM)this);
#else
	int retn;
	FARPROC far_proc = MakeProcInstance((FARPROC)StandardDialogProc, hInstance);
	retn = DialogBoxParam(hInstance, MAKEINTRESOURCE(id), hwnd, far_proc, (LPARAM)this);
	FreeProcInstance(far_proc);
	return retn;
#endif
}

BOOL KDialog :: WmCommand()
{
	switch(wParamThis) {
		case  IDOK :
		case  IDCANCEL :
			EndDialog(0);	// 1 - return
			return TRUE;
	}

	return FALSE;
}

BOOL KDialog :: WmInitDialog()
{
	return TRUE;
}

BOOL KDialog :: WmDrawItem()
{
	return TRUE;
}

BOOL KDialog :: WmTimer()
{
	return TRUE;
}

BOOL KDialog :: WmVScroll()
{
	return TRUE;
}

BOOL KDialog :: WmHScroll()
{
	return TRUE;
}

BOOL KDialog :: DialogProc( HWND hdlg, UINT message, WPARAM wParam, LPARAM lParam )
{
	hwndDlg = hdlg;
	wParamThis = wParam;
	lParamThis = lParam;

	switch( message )	{
		case WM_INITDIALOG :
			if(bCenterFlag) {
				MoveWindowCenter(hwndDlg);
			}
			return WmInitDialog(); 
		case WM_COMMAND :
			return WmCommand();
		case WM_DRAWITEM:
			return WmDrawItem();
		case WM_TIMER:
			return WmTimer();
		case WM_HSCROLL:
			return WmHScroll();
		case WM_VSCROLL:
			return WmVScroll();
	}
	return FALSE;
}

