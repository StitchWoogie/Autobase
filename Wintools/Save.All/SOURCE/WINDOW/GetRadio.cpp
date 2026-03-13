#include "stdafx.h"
#include <glib.h>

//------------------------------------------------------------------------------
// 0부터 시작하는 연속되는 radio button에서 현재 선택된 값을 읽어온다.
//------------------------------------------------------------------------------

int GetRadioPosition(HWND hDlg, int start_id, int count)
{
	int i;
	for(i = 0; i < count; i++) {
		if(IsDlgButtonChecked(hDlg, start_id+i)) {
			return i;
		}
	}
	return -1;
}
