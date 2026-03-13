#if	!defined(__DATASWAP_H)
#define __DATASWAP_H

#if	!defined(__COMPILER_HPP)
#include "compiler.hpp"
#endif

#pragma pack(push, 1)

class StackChar{
	public:
		TCHAR *data;
		StackChar(unsigned int hap = 1);
		~StackChar();
};

class StackBYTE{
	public:
		BYTE *data;
		StackBYTE(DWORD hap = 1);
		~StackBYTE();
};

class StackInt{
	public:
		int *data;
		StackInt(unsigned int hap = 1);
		~StackInt();
};

class StackShort{
	public:
		SHORT *data;
		StackShort(unsigned int hap = 1);
		~StackShort();
};

class StackLong{
	public:
		long *data;
		StackLong(unsigned int hap = 1);
		~StackLong();
};

class StackFloat{
	public:
		float *data;
		StackFloat(unsigned int hap = 1);
		~StackFloat();
};

class StackBlock{
		DWORD dwBlockCount;
		DWORD dwBlockSizeOrginal; 	// 실제 Block의 크기
		DWORD dwBlockSizeReal;			// 조정된 크기의 Block
	public:
		BYTE *data;
		void SetBlock(BYTE *block, int pos);
		void GetBlock(BYTE *block, int pos);
		StackBlock(unsigned int hap = 1, DWORD one_block_size = 1);
		~StackBlock();
};

#pragma pack(pop)

#endif

