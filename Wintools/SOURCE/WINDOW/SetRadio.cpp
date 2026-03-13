#include "stdafx.h"
#include <glib.h>      
 
 
//------------------------------------------------------------------------------
// 0부터 시작하는 연속되는 radio button에서 현재 선택된 값을 읽어온다.
//------------------------------------------------------------------------------
 
void SetRadioPosition(HWND hDlg, int start_id, int count, int value)
{
	CheckRadioButton(hDlg, start_id, start_id+count-1, value+start_id);
}
