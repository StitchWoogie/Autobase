#include "stdafx.h"
#include <io.h>

#include <tools.h>
#include <compiler.hpp>


//------------------------------------------------------------------------------
//	파일의 갯수를 알아본다.
//------------------------------------------------------------------------------

int GetFileHap(char *filename)
{
	struct ffblk32 ffblk;
	int done;
	int count = 0;
	FindFileClass find;

	done = find.findfirst(filename, &ffblk,0);
	while (!done)
	{
		count ++;
		done = find.findnext(&ffblk);
	}

	return count;
}

