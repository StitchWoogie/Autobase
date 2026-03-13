// TAG size O.K
#include "stdafx.h"

#include <io.h>

#include <tools.h>

bool IsFileExists(const TCHAR *filename)
{
	if(access(filename, 0) == 0)	return true;

	return false;
}
