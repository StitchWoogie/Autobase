#include "stdafx.h"
#include <glib.h>

WaitCursorClass :: WaitCursorClass()
{
	hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));
}

WaitCursorClass :: ~WaitCursorClass()
{
	Restore();
}

void WaitCursorClass :: Restore()
{
	if(hCursorOld == NULL)	return;
	SetCursor(hCursorOld);
   hCursorOld = NULL;
}

