#include "stdafx.h"
#include <kwl\kdialog.h>
#include <dataswap.h>

KDialogPrint :: KDialogPrint()
{
	block = new Block(sizeof(PRINT_DRIVER_LIST));

	PrintDriverFillList(block);
}

KDialogPrint :: ~KDialogPrint()
{
	if(block != NULL)	delete block;
}

void KDialogPrint :: FillPrintDriverComboBox(HWND hwndCombo, PRINT_DRIVER_LIST *defaultDriver)
{
	DWORD l;
	char buf[80];
	PRINT_DRIVER_LIST item;
	
	for(l = 0; l < block->GetBlockCount(); l++) {
		block->GetBlock((BYTE*)&item, l);
		sprintf(buf, "%s on %s", item.sDevice, item.sPort);
		SendMessage(hwndCombo, CB_ADDSTRING, 0, (LONG)(LPSTR)buf);		
	}

	sprintf(buf, "%s on %s", defaultDriver->sDevice, defaultDriver->sPort);
	if(SendMessage(hwndCombo, CB_SELECTSTRING, 0, (LONG)(LPSTR)buf) == CB_ERR) {
		//SendMessage(hwndCombo, CB_SETCURSEL, 0, 0L);
	}
}

void KDialogPrint :: GetSelectedDriver(HWND hwndCombo, PRINT_DRIVER_LIST *driver)
{
	DWORD l;
	char buf1[80];
	char buf2[80];
	PRINT_DRIVER_LIST item;
	int retn;

	retn = SendMessage(hwndCombo, CB_GETCURSEL, 0, 0L);
	SendMessage(hwndCombo, CB_GETLBTEXT, retn, (LPARAM)buf1);
	
	for(l = 0; l < block->GetBlockCount(); l++) {
		block->GetBlock((BYTE*)&item, l);
		sprintf(buf2, "%s on %s", item.sDevice, item.sPort);
		if(strcmp(buf1, buf2) == 0) {
			memcpy(driver, &item, sizeof(PRINT_DRIVER_LIST));
			return;
		}
	}
}



