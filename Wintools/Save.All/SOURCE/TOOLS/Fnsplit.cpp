#include "stdafx.h"
#include <tools.h>

FnSplit :: FnSplit()
{
	drive = new TCHAR[MAXDRIVE];
	dir = new TCHAR[MAXDIR];
	name = new TCHAR[MAXFILE];
	ext = new TCHAR[MAXEXT];
}

FnSplit :: ~FnSplit()
{
	if(drive != NULL)	delete drive;
	if(dir   != NULL)	delete dir;
	if(name  != NULL)	delete name;
	if(ext	!= NULL)	delete ext;
}

int FnSplit :: fnsplit(const TCHAR *path)
{
	if(drive == NULL || dir == NULL || name == NULL || ext == NULL) {
		MessageBox(NULL, _T("Local Memory Insufficent"), _T("FnSplit :: fnsplit"), MB_OK);
		return 0;
	}
#if	defined (__BORLANDC__)
	return (::fnsplit(path, drive, dir, name, ext));
#else
	_tsplitpath(path, drive, dir, name, ext);
	return 1;
#endif
}

void FnSplit :: GetNameExt(TCHAR *string)
{
	_tcscpy(string, name);
	_tcscat(string, ext);
}

void FnSplit :: GetDirNameExt(TCHAR *string)
{
	_tcscpy(string, dir);
	_tcscat(string, name);
	_tcscat(string, ext);
}


//--------------------------------
// 드라이브와 디렉토리명을 구한다.
//--------------------------------
void FnSplit :: GetDriveDir(TCHAR *string)
{
	_tcscpy(string, drive);
	_tcscat(string, dir);
}

