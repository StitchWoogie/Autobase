// TAG size O.K
#include "stdafx.h"

void DrawGlobalLockCount(HDC hdc, HGLOBAL hGlobal)
{
	UINT count = LOBYTE(LOWORD(GlobalFlags(hGlobal)));

	if(count <= 1)	return;	// 최소한 한개는 열릴 수 있다. (대화상자나 Popup 메뉴의 등장)

	SetTextColor(hdc, RGB(0, 0, 0));
	SetBkColor(hdc, RGB(255, 255, 255));
	SetBkMode(hdc, OPAQUE); 

	char msg[80];
	
	sprintf(msg, "Global lock count is %d", count);
	TextOut(hdc, 0, 0, msg, strlen(msg));
}
