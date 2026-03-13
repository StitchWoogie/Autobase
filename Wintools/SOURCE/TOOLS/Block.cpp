
#include <tools.h>
#include <dataswap.h>

Block :: Block(DWORD size)
{
	SetBlockSize(size);
	pBlock = NULL;
	dwBlockCount = 0L;
	dwAllocedCount = 0L;
}

Block :: ~Block()
{
	DeleteAllBlock();
}

void Block :: DeleteAllBlock()
{
	if(pBlock != NULL) {
		delete pBlock;
	}
	pBlock = NULL;
	dwBlockCount = 0L;
	dwAllocedCount = 0L;
}

void Block :: SetBlockSize(DWORD one_block_size)
{
	pBlock = NULL;

	// 2의 i승으로 떨어지는 수를 찾는다.
	DWORD val = GetFitGlobalBlockSize(one_block_size);

	dwBlockSizeOriginal = one_block_size; 		// 실제 Block의 크기
	dwBlockSizeReal = val;						// 조정된 크기의 Block
}

void Block :: SetBlock(void *block, DWORD pos)
{
	if(pos >= dwBlockCount)	return;

	if(pBlock == NULL)	return;

	memcpy(pBlock+(DWORD)dwBlockSizeReal*pos, block, dwBlockSizeOriginal);
}

int Block :: GetBlock(void *block, DWORD pos)
{
	if(pos >= dwBlockCount)	return 0;

	if(pBlock == NULL)		return 0;
 
	memcpy(block, pBlock+(DWORD)dwBlockSizeReal*pos, dwBlockSizeOriginal);

	return 1;
}

int  Block :: AddBlock(void *block)
{
	return InsertBlock(block, GetBlockCount());
}

int  Block :: InsertBlock(void *block, DWORD pos)
{
	if(dwBlockCount >= 0xFFFFFFF0L)	return 0;

	if(dwBlockCount >= dwAllocedCount) {
		int MAX_ALLOC_PREPARE = dwBlockCount/5;
		if(MAX_ALLOC_PREPARE < 1)		MAX_ALLOC_PREPARE = 1;
		//if(MAX_ALLOC_PREPARE > 1000)	MAX_ALLOC_PREPARE = 1000;

		BYTE *new_data;

		new_data = new BYTE[(long)dwBlockSizeReal*(dwAllocedCount+MAX_ALLOC_PREPARE)];

		if(new_data == NULL) {
			return 0;
		}

		if(dwBlockCount == 0) {		// first block insert
			memcpy(new_data, block, dwBlockSizeOriginal);
		}
		else {
			if(pos > 0) {
				memcpy(new_data, pBlock, dwBlockSizeReal*pos);
			}

			memcpy(new_data+pos*dwBlockSizeReal, block, dwBlockSizeOriginal);

			if(pos < dwBlockCount) {
				memcpy(new_data+(pos+1)*dwBlockSizeReal, pBlock+(pos)*dwBlockSizeReal, dwBlockSizeReal*(dwBlockCount-pos));
			}
			/*
			DWORD  l;

			for(l = 0; l < pos; l++) {
				memcpy(new_data+(l)*dwBlockSizeReal, pBlock+(l)*dwBlockSizeReal, dwBlockSizeOriginal);
			}

			memcpy(new_data+pos*dwBlockSizeReal, block, dwBlockSizeOriginal);

			for(l = pos; l < dwBlockCount; l++) {
				memcpy(new_data+(l+1)*dwBlockSizeReal, pBlock+(l)*dwBlockSizeReal, dwBlockSizeOriginal);
			}
			*/
		}

		if(pBlock != NULL) {	// 이전소스 if(pBlock) -> (pBlock != NULL) 로 변경했다. 2020-3-31 혹시 이것때문에 문제인가하여서.
			delete pBlock;
		}

		pBlock = new_data;
		dwAllocedCount += MAX_ALLOC_PREPARE;
	}
	else {
		DWORD l;

		for(l = dwBlockCount; l > pos; l--) {
			memcpy(pBlock+(l)*dwBlockSizeReal, pBlock+(l-1)*dwBlockSizeReal, dwBlockSizeOriginal);
		}

		memcpy(pBlock+pos*dwBlockSizeReal, block, dwBlockSizeOriginal);
	}

	dwBlockCount++;

	return 1;
}

int  Block :: DeleteBlock(DWORD pos)
{
   // 삭제 속도를 개선시켜놓은 상태이다.

	if(dwBlockCount <= 0)	return 0;
	if(pos >= dwBlockCount)	return 0;

	if(dwBlockCount == 1) {
		delete pBlock;
		pBlock = NULL;
		dwBlockCount = 0;
		dwAllocedCount = 0;
		return 1;
	}

	DWORD  l;

	for(l = pos+1; l < dwBlockCount; l++) {
		memcpy(pBlock+(l-1)*dwBlockSizeReal, pBlock+(l)*dwBlockSizeReal, dwBlockSizeOriginal);
	}

	dwBlockCount--;

	return 1;
}

void Block :: TempBlock(DWORD t1, DWORD t2)
{
	StackBYTE buf1(GetBlockSize());
	StackBYTE buf2(GetBlockSize());

	GetBlock(buf1.data, t1);
	GetBlock(buf2.data, t2);
	SetBlock(buf1.data, t2);
	SetBlock(buf2.data, t1);
}

void *Block :: GetPtr(DWORD pos)
{
	if(pos >= dwBlockCount)	return NULL;

	if(pBlock == NULL)		return NULL;

	return &pBlock[pos*dwBlockSizeReal];
}

void BlockCopy(Block *target, Block *source)
{
	target->DeleteAllBlock();
	DWORD l;
	StackBYTE buf(source->GetBlockSize());

	for(l = 0; l < source->GetCount(); l++) {
		source->GetBlock(buf.data, l);
		target->AddBlock(buf.data);
	}
}

