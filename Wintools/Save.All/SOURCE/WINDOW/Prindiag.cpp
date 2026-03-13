// english O.K
#include "stdafx.h"
#include <string.h>

#include <compiler.hpp>

#if	!defined(_WIN32)
#include <print.h>
#endif

#include <gclass.h>

GPrintDialog :: GPrintDialog(HWND hwnd)
{
	//hDevMode = GlobalAlloc(GMEM_MOVEABLE, sizeof(DEVMODE));
	choose = new PRINTDLG[1];

	if(choose != NULL) {
		memset(choose, 0, sizeof(PRINTDLG));

		choose->lStructSize = sizeof(PRINTDLG);
		choose->hwndOwner = hwnd;
		choose->hDevMode = NULL;
		choose->hDevNames = NULL;
		choose->hDC = NULL;
		choose->Flags = PD_RETURNDC;
	}
}

GPrintDialog :: ~GPrintDialog()
{
	if (choose->hDevMode != NULL)		GlobalFree(choose->hDevMode);
	if (choose->hDevNames != NULL)   GlobalFree(choose->hDevNames);
	if (choose->hDC != NULL)			DeleteDC(choose->hDC);

	if(choose != NULL) 	delete choose;
}

BOOL GPrintDialog :: Execute()
{     
	BOOL retn;

	retn = PrintDlg(choose);
	if(retn == 0L) {	// error
		ErrorMessage();
	}

	return retn;
}

void GPrintDialog :: GetStructure(PRINTDLG *st)
{
	memcpy(st, choose, sizeof(PRINTDLG));
}

HDC GPrintDialog :: GetDC()
{
	if(choose == NULL)	return NULL;

   return choose->hDC;
}


