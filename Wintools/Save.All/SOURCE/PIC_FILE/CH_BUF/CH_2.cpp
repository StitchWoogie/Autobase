#include "stdafx.h"
#include <glib.h>
#include <compiler.hpp>
#include <ch_buf.h>

void ConvertBuf1600To2(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k, l;
   int xbyteplane = (cbSizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int cs;

   i = cbLineY%8;     		// 디더링하기위한 y 의 좌표상의 위치.

   memset(tbuf, 0, xbyteplane);

   for(j = 0, cs = 0; j < cbSizeX; j++,cs+=3) {
      k = j%8; l = j/8;
      tbuf[l] |= DITHER_TABLE
		 [((sbuf[cs+2]>>2)*30 + (sbuf[cs+1]>>2)*59 + (sbuf[cs]>>2)*11)
		 /100][i] & BIT_MASK[k] ? BIT_MASK[k] : 0;
   }

   cbLineY++;
}

void ConvertBuf32768To2(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k, l;
   int xbyteplane = (cbSizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int *ps = (int*) sbuf;

   i = cbLineY%8;     		// 디더링하기위한 y 의 좌표상의 위치.

   memset(tbuf, 0, xbyteplane);

   for(j = 0; j < cbSizeX; j++) {
      k = j%8; l = j/8;
      tbuf[l] |= DITHER_TABLE
		 [( ( getrvalue(ps[j])<<1 )*30 +
		    ( getgvalue(ps[j])<<1 )*59 +
		    ( getbvalue(ps[j])<<1 )*11)
		 /100][i] & BIT_MASK[k] ? BIT_MASK[k] : 0;
   }
   cbLineY++;
}

void ConvertBuf256To2(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i, j;

   memset(tbuf, 0, (cbSizeX+7)/8); 	// clear target buffer

   i = cbLineY%8;

   for(j = 0; j < cbSizeX; j++) {
      back = *(sbuf+j);
      tbuf[j/8] |= (((DITHER_TABLE[dacConvertBuf[back]][i]& BIT_MASK[j%8])==0) ? 0 : BIT_MASK[j%8]);
   }

   cbLineY++;
}

void ConvertBuf16To2(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i, j;
   int xbyte = (cbSizeX+7)/8;

   memset(tbuf, 0, xbyte); 	// clear target buffer

   i = cbLineY%8;

   for(j = 0; j < cbSizeX; j++) {
      back = GetColorAllBuf16(sbuf, j);
      tbuf[j/8] |= (((DITHER_TABLE[dacConvertBuf[back]][i]& BIT_MASK[j%8])==0) ? 0 : BIT_MASK[j%8]);
   }

   cbLineY++;
}

