
#include <tools.h>
#include <dataswap.h>
                          
StackChar :: StackChar(unsigned int hap)
{
	data = NULL;
	data = new TCHAR[hap];
}

StackChar :: ~StackChar()
{
	if(data != NULL)	delete data;
}

StackBYTE :: StackBYTE(DWORD hap)
{
	data = NULL;
	data = new BYTE[hap];
}

StackBYTE :: ~StackBYTE()
{
	if(data != NULL)	delete data;
}


StackInt :: StackInt(unsigned int hap)
{
	data = NULL;
	data = new int[hap];
}

StackInt :: ~StackInt()
{
	if(data != NULL)	delete data;
}

StackShort :: StackShort(unsigned int hap)
{
	data = NULL;
	data = new SHORT[hap];
}

StackShort :: ~StackShort()
{
	if(data != NULL)	delete data;
}

//------------------------------------------------------------------------------
//	long 형을 마련한다.
//	주로 스택용으로 사용하기 바란다.
//------------------------------------------------------------------------------

StackLong :: StackLong(unsigned int hap)
{
	data = NULL;
	data = new long[hap];
}

StackLong :: ~StackLong()
{
	if(data != NULL)	delete data;
}

//------------------------------------------------------------------------------
//	float 형을 마련한다.
//	주로 스택용으로 사용하기 바란다.
//------------------------------------------------------------------------------

StackFloat :: StackFloat(unsigned int hap)
{
	data = NULL;
	data = new float[hap];
}

StackFloat :: ~StackFloat()
{
	if(data != NULL)	delete data;
}

StackBlock :: StackBlock(unsigned int hap, DWORD one_block_size)
{
	data = NULL;

	// 2의 i승으로 떨어지는 수를 찾는다.
	DWORD val = GetFitGlobalBlockSize(one_block_size);

	dwBlockCount = hap;
	dwBlockSizeOrginal = one_block_size; 	// 실제 Block의 크기
	dwBlockSizeReal = val;						// 조정된 크기의 Block

	data = new BYTE[(DWORD)dwBlockCount*(DWORD)dwBlockSizeReal];
}

void StackBlock :: SetBlock(BYTE *block, int pos)
{
	if(pos < 0 || pos >= (int)dwBlockCount)	return;

	memcpy(data+(DWORD)dwBlockSizeReal*pos, block, dwBlockSizeOrginal);
}

void StackBlock :: GetBlock(BYTE *block, int pos)
{
	if(pos < 0 || pos >= (int)dwBlockCount)	return;

	memcpy(block, data+(DWORD)dwBlockSizeReal*pos, dwBlockSizeOrginal);
}

StackBlock :: ~StackBlock()
{
	if(data)	delete data;
}


