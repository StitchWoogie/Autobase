#include "stdafx.h"
#include <io.h>

#include <compiler.hpp>
#include <tools.h>

#include <dos.h>

long getfilesize(const char *filename)
{
	int done;
	struct ffblk32 ffblk;
	FindFileClass find;

	done = find.findfirst(filename, &ffblk, FA_ARCH|FA_HIDDEN|FA_SYSTEM);

	if(done)	return (0L);

	return (ffblk.ff_fsize);
}

