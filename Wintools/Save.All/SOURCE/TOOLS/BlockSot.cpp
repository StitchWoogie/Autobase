#include "stdafx.h"
#include <tools.h>
#include <dataswap.h>

void BlockSort(Block *block, LPFNBLOCKSORTFUNCTION function)
{
	// sort
	DWORD size = block->GetBlockSize();
	StackBYTE data1(size);
	StackBYTE data2(size);

	if(data1.data == NULL)	return;
	if(data2.data == NULL)	return;

	int retn;
	DWORD pos;
	DWORD l;
	DWORD m;

	for(l = 0; l < block->GetCount(); l++) {
		block->GetBlock(data1.data, l);
		pos = l;
		for(m = l+1; m < block->GetCount(); m++) {
			block->GetBlock(data2.data, m);
			retn = function(data1.data, data2.data);
			if(retn < 0) {
				pos = m;
				memcpy(data1.data, data2.data, size);
			}
		}
		if(pos != l) {
			block->GetBlock(data1.data, l);	
			block->GetBlock(data2.data, pos);
			block->SetBlock(data1.data, pos);	
			block->SetBlock(data2.data, l);
		}
	}	
}