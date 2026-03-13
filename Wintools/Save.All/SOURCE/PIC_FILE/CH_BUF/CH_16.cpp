#include "stdafx.h"
#include <glib.h>
#include <compiler.hpp>
#include <ch_buf.h>

void ConvertBuf1600To16(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k;
   //t xbyteplane = (cbSizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int cs;
   BYTE max;
	int color;

   i = cbLineY%8;     		// 디더링하기위한 y 의 좌표상의 위치.

   for(j = 0, cs = 0; j < cbSizeX; j++,cs+=3) {
      k = j%8;
		color = 0;
 
      max = sbuf[cs];
      if(sbuf[cs+1] > max)	max = sbuf[cs+1];
      if(sbuf[cs+2] > max)	max = sbuf[cs+2];

      if((DITHER_TABLE[max >> 2][i]        & BIT_MASK[k]))		 color |= WORD_MASK[3];
      if((DITHER_TABLE[sbuf[cs+2] >> 2][i] & BIT_MASK[k]))      color |= WORD_MASK[0];
      if((DITHER_TABLE[sbuf[cs+1] >> 2][i] & BIT_MASK[k]))      color |= WORD_MASK[1];
      if((DITHER_TABLE[sbuf[cs+0] >> 2][i] & BIT_MASK[k]))      color |= WORD_MASK[2];

		PutColorAllBuf16(tbuf, j, color);	
   }

   cbLineY++;
}

void ConvertBuf32768To16(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k, l;
   int xbyteplane = (cbSizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int rStart, gStart, bStart;
   int *ps = (int*) sbuf;
   BYTE max;

   i = cbLineY%8;     		// 디더링하기 위한 y 의 좌표상의 위치.

   memset(tbuf, 0, xbyteplane*4);

   rStart = xbyteplane*3;  	// start point of r plane
   gStart = xbyteplane*2;       // start point of g plane
   bStart = xbyteplane;         // start point of b plane

   for(j = 0; j < cbSizeX; j++) {
      k = j%8; l = j/8;

      max = getrvalue(ps[j]);
      if(getgvalue(ps[j]) > max)	max = getgvalue(ps[j]);
      if(getbvalue(ps[j]) > max)	max = getbvalue(ps[j]);

      if((DITHER_TABLE[max << 1][i] & BIT_MASK[k]))							tbuf[l] |= BIT_MASK[k];
      if((DITHER_TABLE[getrvalue(ps[j]) << 1][i] & BIT_MASK[k]))      	tbuf[rStart+l] |= BIT_MASK[k];
      if((DITHER_TABLE[getgvalue(ps[j]) << 1][i] & BIT_MASK[k]))      	tbuf[gStart+l] |= BIT_MASK[k];
      if((DITHER_TABLE[getbvalue(ps[j]) << 1][i] & BIT_MASK[k]))      	tbuf[bStart+l] |= BIT_MASK[k];
   }
   cbLineY++;
}

void ConvertBuf256To16(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k;
   BYTE back;
   BYTE max;
	int color;

   i = cbLineY%8;     		// 디더링하기위한 y 의 좌표상의 위치.

   for(j = 0; j < cbSizeX; j++) {
      back = *(sbuf+j);
      k = j%8;
		color = 0;

      max = dacConvertBuf[back*3+0];
      if(dacConvertBuf[back*3+1] > max)	max = dacConvertBuf[back*3+1];
      if(dacConvertBuf[back*3+2] > max)	max = dacConvertBuf[back*3+2];

      if((DITHER_TABLE[max >> 2][i]                     & BIT_MASK[k]))		  color |= WORD_MASK[3];
      if((DITHER_TABLE[dacConvertBuf[back*3+0] >> 2][i] & BIT_MASK[k]))      color |= WORD_MASK[0];
      if((DITHER_TABLE[dacConvertBuf[back*3+1] >> 2][i] & BIT_MASK[k]))      color |= WORD_MASK[1];
      if((DITHER_TABLE[dacConvertBuf[back*3+2] >> 2][i] & BIT_MASK[k]))      color |= WORD_MASK[2];

		PutColorAllBuf16(tbuf, j, color);	
   }

   cbLineY++;
}

void ConvertBuf2To16(BYTE *tbuf, BYTE *sbuf)
{
	int i;

	for(i = 0; i < cbSizeX; i++) {
		if(sbuf[i/8] & BIT_MASK[i%8])			PutColorAllBuf16(tbuf, i, 15);	
		else											PutColorAllBuf16(tbuf, i, 0);
   }
}
