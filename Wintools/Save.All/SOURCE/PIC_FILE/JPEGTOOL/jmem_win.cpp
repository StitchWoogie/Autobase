#include "stdafx.h"
#include <compiler.hpp>
#include <tools.h>
 
typedef struct {
	HGLOBAL hGlobal;
	DWORD offset;
	DWORD size;
} HARD_MEMORY_STRUCT;

static Block blockHard(sizeof(HARD_MEMORY_STRUCT)); 

short far jdos_open(short far * handle, char *filename)
{ 
	HARD_MEMORY_STRUCT block;
	HARD_MEMORY_STRUCT imsi;
	DWORD l;
	
	memset(&block, 0, sizeof(HARD_MEMORY_STRUCT));
	
	block.size = 100;
	block.hGlobal = GlobalAlloc(GMEM_MOVEABLE, block.size);
	if(block.hGlobal == NULL) {
		return -1;
	}
	block.offset = 0L;

	for(l = 0; l < blockHard.GetBlockCount(); l++) {
		blockHard.GetBlock((BYTE*)&imsi, l);
		if(imsi.hGlobal == NULL) {
			blockHard.SetBlock((BYTE*)&block, l);
			*handle = (short)l;
			return 0;
		}
	}

	if(blockHard.AddBlock((BYTE*)&block)) {
		*handle = (short)blockHard.GetBlockCount()-1;		
		return 0;
	}
	return -1;
}

short far jdos_close(short handle)
{
	HARD_MEMORY_STRUCT block;

	if((DWORD)handle >= blockHard.GetBlockCount())	return 0;

	blockHard.GetBlock((BYTE*)&block, handle);
	GlobalFree(block.hGlobal);
	block.hGlobal = NULL;
	blockHard.SetBlock((BYTE*)&block, handle);	

	return 0;	
}

short far jdos_seek(short handle, long offset)
{
	HARD_MEMORY_STRUCT block;

	if((DWORD)handle >= blockHard.GetBlockCount())	return 0;

	blockHard.GetBlock((BYTE*)&block, handle);
	block.offset = offset;
	blockHard.SetBlock((BYTE*)&block, handle);	

	return 0;	
}

short far jdos_read(short handle, void far * buffer, unsigned short count)
{
	HARD_MEMORY_STRUCT block;
	BYTE *buf;

	if((DWORD)handle >= blockHard.GetBlockCount())	return 0;

	blockHard.GetBlock((BYTE*)&block, handle);
	buf = (BYTE*)GlobalLock(block.hGlobal);
	memcpy(buffer, buf+block.offset, count);
	GlobalUnlock(block.hGlobal);
	block.offset+=count;
	blockHard.SetBlock((BYTE*)&block, handle);	

	return 0;	
}

short far jdos_write(short handle, void far * buffer, unsigned short count)
{
	HARD_MEMORY_STRUCT block;
	BYTE *buf;

	if((DWORD)handle >= blockHard.GetBlockCount())	return 0;

	blockHard.GetBlock((BYTE*)&block, handle);

	if(block.offset+count > block.size) {
		HGLOBAL hGlobal;
		hGlobal = GlobalReAlloc(block.hGlobal, block.offset+count, GMEM_MOVEABLE);	

		if(hGlobal == NULL)	return 0;
		block.hGlobal = hGlobal;
		block.size = block.offset+count;
	}
	buf = (BYTE*)GlobalLock(block.hGlobal);
	memcpy(buf+block.offset, buffer, count);
	GlobalUnlock(block.hGlobal);
	block.offset+=count;
	blockHard.SetBlock((BYTE*)&block, handle);	

	return 0;	
}
