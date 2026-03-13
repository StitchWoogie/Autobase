#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>            
#else
#include <memory.h>
#endif
#include <string.h>

#include <gclass.h>

static COLORREF aclrCust[16];

GColorDialog :: GColorDialog(HWND hwnd)
{
	choose = new CHOOSECOLOR[1];

	if(choose == NULL)	return;

	COLORREF clr;

	// Initialize clr to black.

	clr = RGB(0, 0, 0);

	// Set all structure fields to zero.

	memset(choose, 0, sizeof(CHOOSECOLOR));

	// Initialize the necessary CHOOSECOLOR members.

	choose->lStructSize = sizeof(CHOOSECOLOR);

	choose->hwndOwner = hwnd;
	choose->rgbResult = clr;
	choose->lpCustColors = aclrCust;		// custom color 
	choose->Flags = CC_RGBINIT;
}

GColorDialog :: ~GColorDialog()
{
	if(choose != NULL) 	delete choose;
}

BOOL GColorDialog :: Execute(COLORREF init)
{
	BOOL retn;

	if(choose != NULL) {
      choose->rgbResult = init;
	}

	retn = ChooseColor(choose);
	if(retn == 0L) {	// error
		ErrorMessage();
	}
	return retn;
}

