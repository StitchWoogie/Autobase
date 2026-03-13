// english O.K
#include "stdafx.h"
#include <io.h>
#include <dos.h>
#include <direct.h>

#include <compiler.hpp>
#include <tools.h>

#if	defined (_WIN32)
//------------------------------------------------------------------------------
//	주어진 디렉토리와 안에 존재하는 모든 파일을 지운다.
//------------------------------------------------------------------------------

DWORD DeleteDirAndFile(char *path)
{
	char filename[MAXPATH];
	struct ffblk32 ffblk;
	int done;
	DWORD count = 0;
	FindFileClass find;

	if(access(path, 0) != 0)	return 0;

	sprintf(filename, "%s\\*.*", path);
	done = find.findfirst(filename,&ffblk,0);

	while (!done) {
		sprintf(filename, "%s\\%s", path, ffblk.ff_name);

		if(ffblk.ff_name[0] == '.') {

		}
		else if(ffblk.ff_attrib & FA_DIREC) {
			count += DeleteDirAndFile(filename);
		}
		else {
			if(unlink(filename) == -1) {

			}
			else {
				count++;
			}
		}
		done = find.findnext(&ffblk);
	}

	_rmdir(path);

	return count;
}
#else
//------------------------------------------------------------------------------
//	주어진 디렉토리와 안에 존재하는 모든 파일을 지운다.
//------------------------------------------------------------------------------

DWORD DeleteDirAndFile(char *path)
{
	char filename[MAXPATH];
	struct ffblk ffblk;
	int done;
	DWORD count = 0;
	FindFileClass find;

	if(access(path, 0) != 0)	return 0;

	sprintf(filename, "%s\\*.*", path);
	done = findfirst(filename,&ffblk,0);

	while (!done) {
		sprintf(filename, "%s\\%s", path, ffblk.ff_name);

		if(ffblk.ff_name[0] == '.') {

		}
		else if(ffblk.ff_attrib & FA_DIREC) {
			count += DeleteDirAndFile(filename);
		}
		else {
			if(unlink(filename) == -1) {

			}
			else {
				count++;
			}
		}
		done = findnext(&ffblk);
	}

	rmdir(path);

	return count;
}
#endif