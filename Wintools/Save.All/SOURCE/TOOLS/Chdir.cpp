#include "stdafx.h"
#include <direct.h>

#include <tools.h>

int ChangeDirectory(const TCHAR *directory)
{
	if(_tchdir(directory) == 0)	return 1;
	else						return 0;
}
