// TAG size O.K
#include "stdafx.h"

#include <winspool.h>

#include <print.hpp>

DEVMODE *CreateDevMode(char *sDevice)
{
	HANDLE handle;
	DEVMODE *devMode = NULL;
	HWND hwnd = NULL;

	if(OpenPrinter(sDevice, &handle, NULL) == 0)	return NULL;

	LONG size;
	size = DocumentProperties(hwnd, handle, sDevice, NULL, NULL, 0);
	if(size > 0) {
		devMode = (DEVMODE*)new BYTE[size];
		if(DocumentProperties(hwnd, handle, sDevice, devMode, NULL, DM_OUT_BUFFER) < 0) {
			delete devMode;	
			devMode = NULL;
		}
	}
	
	ClosePrinter(handle);

	return devMode;
}

