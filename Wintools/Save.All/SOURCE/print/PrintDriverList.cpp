// TAG size O.K
#include "stdafx.h"

#include <print.hpp>
#include <dataswap.h>

class GetStringToChar {
		int  pos;
		char *String;
	public:	
		GetStringToChar();
		~GetStringToChar();
		void Set(char *buf);
		void Get(char *buf, char ch);
};

GetStringToChar :: GetStringToChar()
{
	String = NULL;
	pos = 0;
}

GetStringToChar :: ~GetStringToChar()
{
	if(String != NULL)	delete String;
}

void GetStringToChar :: Set(char *buf)
{
	if(String != NULL) {
		delete String;
		String = NULL;
	}

	String = new char[strlen(buf)+1];
	strcpy(String, buf);
	pos = 0;
}

void GetStringToChar :: Get(char *buf, char ch)
{
	int hap = 0;
	int i;

	for(i = pos; i < (int)strlen(String); i++) {
		if(String[i] == NULL)	break;	
		if(String[i] == ch)		break;
		buf[hap] = String[i];
		hap++;
	}

	buf[hap] = 0;
	pos = i+1;
}

static void ConvertBufToPrintDriver(PRINT_DRIVER_LIST *item, char *buf)
{
	GetStringToChar tok;

	tok.Set(buf);
	tok.Get(item->sDevice, '=');
	tok.Get(item->sDriver, ',');
	tok.Get(item->sPort,   ',');
}

void PrintDriverFillList(Block *block)
{
	if(block == NULL)	return;
		
	char buf[80];
	int  i;
	StackChar stack(32000);
	int retn;
	PRINT_DRIVER_LIST item;
	
	if(stack.data == NULL)	return;

	retn = GetProfileSection("Devices", stack.data, 32000);

	for(i = 0; i < retn; i++) {
		if(i == 0 || stack.data[i-1] == NULL) {
			ConvertBufToPrintDriver(&item, &stack.data[i]);
			sprintf(buf, "%s on %s", item.sDevice, item.sPort);
			block->AddBlock((BYTE*)&item);
		}
	}
}

void PrintDriverFillComboBox(HWND hwndCombo, Block *block, PRINT_DRIVER_LIST *defaultDriver)
{
	DWORD l;
	char buf[80];
	PRINT_DRIVER_LIST item;
	
	for(l = 0; l < block->GetBlockCount(); l++) {
		block->GetBlock((BYTE*)&item, l);
		sprintf(buf, "%s on %s", item.sDevice, item.sPort);
		SendMessage(hwndCombo, CB_ADDSTRING, 0, (LONG)(LPSTR)buf);		
	}

	if(defaultDriver == NULL) {
		if(SendMessage(hwndCombo, CB_SETCURSEL, 0, (LONG)(LPSTR)buf) == CB_ERR) {
		}
	}
	else {
		sprintf(buf, "%s on %s", defaultDriver->sDevice, defaultDriver->sPort);
		if(SendMessage(hwndCombo, CB_SELECTSTRING, 0, (LONG)(LPSTR)buf) == CB_ERR) {
		}
	}
	
}

void PrintDriverGetSelectedDriver(HWND hwndCombo, Block *block, PRINT_DRIVER_LIST *driver)
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


void PrintDriverFillDefault(PRINT_DRIVER_LIST *driver)
{
	StackChar stack(32000);
	
	GetProfileString("windows", "device", ",,,", stack.data, 32000);
	GetStringToChar tok;

	tok.Set(stack.data);
	tok.Get(driver->sDevice, ',');
	tok.Get(driver->sDriver, ',');
	tok.Get(driver->sPort,   ',');
}

