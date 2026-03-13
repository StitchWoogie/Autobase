#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>
#else
#include <memory.h>
#endif

#include <glib.h>
#include <ch_buf.h>
#include <totaldef.h>

opticBuf :: opticBuf()
{
   table = NULL;
}

opticBuf :: ~opticBuf()
{
   if(table != NULL)	delete table;
	table = NULL;
}

void opticBuf :: Free(void)
{
   if(table != NULL)	delete table;
}

void opticBuf :: OpticMakeTable(void)
{
   int i;

   for(i = 0; i < nTargetX; i++)
		 table[i] = (int) ((long)i*(long)nSourceX/(long) nTargetX);
}

int opticBuf :: Init(int targetx, int sourcex, int bufBit)
{
   nTargetX = targetx;
   nSourceX = sourcex;
   nBit = bufBit;

   if(table != NULL)	delete table;

   table = new int[nTargetX];
   if(table == NULL) {
      nTargetX = 0;
      nSourceX = 0;
      return 0;
   }

   OpticMakeTable();

   return 1;
}

void opticBuf :: OneLine(BYTE *target, BYTE *source)
{
	switch(nBit) {
		case COLOR_1600:	OpticBuf1600(target, source);	return;
		case COLOR_256:   OpticBuf256(target, source);	return;
		case COLOR_16:		OpticBuf16(target, source);	return;
		case COLOR_2:
		default:          OpticBuf2(target, source);		return;
	}
}

void opticBuf :: OpticBuf1600(BYTE *target, BYTE *source)
{
	int x;

	for(x = 0; x < nTargetX; x++) {
		target[x*3] = source[table[x]*3];
		target[x*3+1] = source[1+table[x]*3];
		target[x*3+2] = source[2+table[x]*3];
	}
}

void opticBuf :: OpticBuf32768(BYTE *target, BYTE *source)
{
	int x;
   int *t=(int*)target, *s=(int*)source;

	for(x = 0; x < nTargetX; x++)
      t[x] = s[table[x]];
}

void opticBuf :: OpticBuf256(BYTE *target, BYTE *source)
{
	int x;

	for(x = 0; x < nTargetX; x++)
		target[x] = source[table[x]];
}

void opticBuf :: OpticBuf16(BYTE *target, BYTE *source)
{
	// packed buffer memory
	int x;
	int color;

	memset(target, 0, (nTargetX+1)/2);

	for(x = 0; x < nTargetX; x++) {
		if((table[x]%2) == 0) {
			color = (source[table[x]/2] >> 4) & 0x0F;			
		}
		else {
			color = (source[table[x]/2] >> 0) & 0x0F;			
		}
		if((x%2) == 0) {
			target[x/2] |= color << 4;
		}
		else {
			target[x/2] |= color << 0;
		}
	}

	/*	plane buffer memory
	int x;
	int toneplane = (nTargetX+7)/8;
	int soneplane = (nSourceX+7)/8;

	memset(target, 0, toneplane*4);
	for(x = 0; x < nTargetX; x++) {
		target[x/8]             |= source[table[x]/8]             & BIT_MASK[table[x]%8] ? BIT_MASK[x%8] : 0;
		target[toneplane+x/8]   |= source[soneplane+table[x]/8]   & BIT_MASK[table[x]%8] ? BIT_MASK[x%8] : 0;
		target[toneplane*2+x/8] |= source[soneplane*2+table[x]/8] & BIT_MASK[table[x]%8] ? BIT_MASK[x%8] : 0;
		target[toneplane*3+x/8] |= source[soneplane*3+table[x]/8] & BIT_MASK[table[x]%8] ? BIT_MASK[x%8] : 0;
	}
	*/
}

void opticBuf :: OpticBuf2(BYTE *target, BYTE *source)
{
	int x;

	memset(target, 0, (nTargetX+7)/8);

	for(x = 0; x < nTargetX; x++) {
		target[x/8] |= source[table[x]/8] & BIT_MASK[table[x]%8] ? BIT_MASK[x%8] : 0;
	}
}


