#include "stdafx.h"

#include <winbase.h>

#include <tools.h>

// GetSystemWindowsDirectory 함수도 있으나 98에서는 함수를 선언한 것만으로도 Kernel32오류가 발생한다.
// GetSystemDirectory에서 System이나 System32를 제거한다.

void GetWindowsRootDirectory(TCHAR *directory, int limit)
{
	GetSystemDirectory(directory, limit);

	int size = _tcslen(directory);
	for(int i = size-1; i > 0; i--) {
		if(directory[i] == '\\') {
			directory[i] = 0;
			return;
		}
	}
}