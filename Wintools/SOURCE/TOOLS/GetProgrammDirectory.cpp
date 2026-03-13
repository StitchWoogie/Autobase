#include "stdafx.h"

#include <tchar.h>

#include <tools.h>

void GetProgrammDirectory(HINSTANCE hInstance, LPTSTR dir, int limit)
{
	GetModuleFileName(hInstance, dir, limit);
	for(int i = _tcslen(dir)-1; i > 0; i--) {
		if(dir[i] == '\\') {
			dir[i] = 0;
			break;
		}
	}
}




