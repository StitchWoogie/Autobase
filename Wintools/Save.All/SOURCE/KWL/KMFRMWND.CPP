#include "kmfrmwnd.h"

HANDLE hInstApplication = NULL;
HANDLE hPrevInstApplication = NULL;

KMdiFrameWindow :: KMdiFrameWindow() : KFrameWindow()
{
	hMenuWindow = NULL;
	idFirstChild = 999;
}

KMdiFrameWindow :: ~KMdiFrameWindow()
{

}

LRESULT KMdiFrameWindow :: ElseWindowProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	return DefFrameProc (hwnd, hwndClientThis, msg, wParam, lParam);
}

void KMdiFrameWindow :: OnCreate()
{
	CLIENTCREATESTRUCT clientcreate;
	
	clientcreate.hWindowMenu  = hMenuWindow;
	clientcreate.idFirstChild = idFirstChild;

	hwndClientThis = CreateWindow ("MDICLIENT", NULL,
										 WS_CHILD | WS_CLIPCHILDREN | WS_VISIBLE | WS_VSCROLL | WS_HSCROLL,
										 0, 0, 0, 0, hwndThis, (HMENU)1, hInstApplication,
										 (LPVOID) (&clientcreate)) ;
}

void KMdiFrameWindow :: MdiSetMenu(HMENU hMenu, HMENU hmenuwindow)
{
	hMenuWindow	= hmenuwindow;

	if(hwndClientThis != NULL) {
		SendMessage(hwndClientThis, WM_MDISETMENU, (WPARAM)hMenu, (LPARAM)hmenuwindow);	
	}
}

void KMdiFrameWindow :: SetFirstChildID(UINT id)
{
	idFirstChild = id;
}

