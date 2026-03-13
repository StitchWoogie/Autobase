#include "stdafx.h"
#include <string.h>

#include <glib.h>

#include "menubutn.h"

void MenuButtonClass :: Init(HWND hwnd, HINSTANCE hInstance)
{
	hWnd = hwnd;
	hInst = hInstance;
	nButtonHap = 0;
}

MenuButtonClass :: ~MenuButtonClass()
{

}

void MenuButtonClass :: Insert(const TCHAR *title, unsigned id)
{
	if(nButtonHap == MAX_BUTTON) {
		MessageBox(hWnd, "Too many button.", "MenuButtonClass :: Insert", MB_OK);
		return;
	}

	HWND hwnd;

	hwnd = CreateWindow("button", title,
							  WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
							  0, 0, 0, 0,
							  hWnd, (HMENU)id, hInst, NULL);
	button[nButtonHap].hwnd = hwnd;
	button[nButtonHap].id = id;
	button[nButtonHap].sHap = strlen(title);

	nButtonHap++;
}

void MenuButtonClass :: DisableAll()
{
	int i;

	for(i = 0; i < nButtonHap; i++) {
		EnableWindow(button[i].hwnd, FALSE);
	}
}

BOOL MenuButtonClass :: Enable(unsigned id, int flag)
{
	int i;

	for(i = 0; i < nButtonHap; i++) {
		if(id == button[i].id)	return(EnableWindow(button[i].hwnd, flag));
	}

	return 0;
}

void MenuButtonClass :: SetText(unsigned id, char *text)
{
	int i;

	for(i = 0; i < nButtonHap; i++) {
		if(id == button[i].id)	SetWindowText(button[i].hwnd, text);
	}
}

void MenuButtonClass :: SetFont(HFONT hFont)
{
	int i;

	for(i = 0; i < nButtonHap; i++) {
		SendMessage(button[i].hwnd, WM_SETFONT, (WPARAM)hFont, 0L);
	}
}

#define BUTTON_EXTRA	2	// 버턴 스트링에 더하는 여분의 글자수

void MenuButtonClass :: Move(int sx, int sy)
{
	nStartX = sx;
	nStartY = sy;

	int gab;
	int x = sx;
	int y = sy;
	int i;

	gab = (button[0].sHap+BUTTON_EXTRA)*cxChar+6;
	MoveWindow(button[0].hwnd, x, y, gab, cyChar+10, TRUE);
	x += gab;

	for(i = 1; i < nButtonHap; i++) {
		gab = (button[i].sHap+BUTTON_EXTRA)*cxChar+6;
		if(gab+x > nWidth+sx) {
			y += cyChar+10;
			MoveWindow(button[i].hwnd, sx, y, gab, cyChar+10, TRUE);
			x = sx+gab;
		}
		else {
			MoveWindow(button[i].hwnd, x, y, gab, cyChar+10, TRUE);
			x += gab;
		}
	}
}

int  MenuButtonClass :: GetNeedSizeY(int width)
{
	nWidth = width;

	int gab;
	int line = 1;
	int x = 0;
	int i;

	gab = (button[0].sHap+BUTTON_EXTRA)*cxChar+6;
	x = gab;

	for(i = 1; i < nButtonHap; i++) {
		gab = (button[i].sHap+BUTTON_EXTRA)*cxChar+6;
		if(gab+x > nWidth) {
			line++;
			x = gab;
		}
		else {
			x += gab;
		}
	}

	nHeight = (cyChar+10)*line;

	return nHeight;
}

void MenuButtonClass :: Paint(HDC hdc)
{
	gcls(hdc, nStartX, nStartY, nStartX+nWidth, nStartY+nHeight, WHITE_GRAY_COLOR);
}