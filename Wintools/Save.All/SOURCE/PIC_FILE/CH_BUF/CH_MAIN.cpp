#include "stdafx.h"
#include <stdlib.h>
#include <math.h>
//#include <iostream.h>

#include <compiler.hpp>
#include <tools.h>
#include <glib.h>
#include <ch_buf.h>

BYTE cbSourceBit, cbTargetBit, cbConvertFlag;
int  cbByteX, cbSizeX;
int  cbLineY;		// 디더링 할때 현재의 라인수가 필요하다.

void ConvertBufSame(BYTE *tbuf, BYTE *sbuf)	// same buf
{
   memcpy(tbuf, sbuf, cbByteX);
}

void PutColorAllBuf16(BYTE *buf, int x, int color)
{
	// packed pixel buf 일때

	if(x%2) {	// 홀수는 하위 nibble
		buf[x/2] = (buf[x/2] & 0xF0) | color;
	}
	else {		// 짝수는 상위 nibble
		buf[x/2] = (buf[x/2] & 0x0F) | (color << 4);
	}
}

int GetColorAllBuf16(BYTE *buf, int x)
{
	// packed pixel buf 일때

	BYTE color;

	if(x%2) {	// 홀수는 하위 4bit
		color = buf[x/2] & 0x0F;
	}
	else {		// 짝수는 상위 4bit
		color = (buf[x/2] >> 4) & 0x0F;
	}

	return color;
}


/*
//--------------------------------------------------------------------------
//	256칼라의 버퍼로 되어있더라도 16칼라만 사용했을시
//	버퍼를 16칼라버퍼로 바꾸어줄 때 사용한다.
//--------------------------------------------------------------------------

void ChangeBuf256To16(BYTE *tbuf, BYTE *sbuf, int sizex)
{
   register i;
   BYTE back;
   int  xbyte = (sizex+7)/8;
   int k,l;

   memset(tbuf, 0, xbyte*4);

   for(i = 0; i < sizex; i++) {
      back = sbuf[i];

      k = i/8;
      l = i%8;

      if(BIT_MASK[4] & back)	tbuf[k]         += BIT_MASK[l];
      if(BIT_MASK[5] & back)	tbuf[xbyte+k]   += BIT_MASK[l];
      if(BIT_MASK[6] & back)	tbuf[xbyte*2+k] += BIT_MASK[l];
      if(BIT_MASK[7] & back)	tbuf[xbyte*3+k] += BIT_MASK[l];
   }
}
*/


/*
//---------------------------------------------------------------------------
//	한줄짜리 16칼라 버퍼에서 주어진 OFFSET 의 COLOR 를 읽어온다.
//---------------------------------------------------------------------------

int GetColorAllBuf16(int x, int allx, unsigned char *buf)
{
	// packed pixel buf 일때

	BYTE color;

	if(x%2) {	// 홀수는 하위 4bit
		color = buf[x/2] & 0x0F;
	}
	else {		// 짝수는 상위 4bit
		color = (buf[x/2] >> 4) & 0x0F;
	}

//plane buf일 때 
// unsigned  int x1 = x/8, x2 = (allx+7)/8;
// unsigned char bit=x%8, color=0;

//	color |= ((buf[x1]      & BIT_MASK[bit]) == BIT_MASK[bit]) ? 8 : 0;
//   color |= ((buf[x1+x2]   & BIT_MASK[bit]) == BIT_MASK[bit]) ? 4 : 0;
//   color |= ((buf[x1+x2*2] & BIT_MASK[bit]) == BIT_MASK[bit]) ? 2 : 0;
//   color |= ((buf[x1+x2*3] & BIT_MASK[bit]) == BIT_MASK[bit]) ? 1 : 0;
	

   return (color);
}

//---------------------------------------------------------------------------
//	한줄짜리 16칼라 버퍼에 주어진 OFFSET 의 COLOR 를 쓴다.
//---------------------------------------------------------------------------

void PutColorAllBuf16(int x, int allx, unsigned char *buf, int color)
{
	// packed pixel buf 일때

	if(x%2) {	// 홀수는 하위 nibble
		//buf[x/2] = (buf[x/2] & 0xF0) | color;
	}
	else {		// 짝수는 상위 nibble
		buf[x/2] = (buf[x/2] & 0x0F) | (color << 4);
	}

//		plane buf일 때 
//	unsigned  int x1 = x/8, x2 = (allx+7)/8;
//   unsigned char bit=x%8;

//   buf[x1]      |= color & 8 ? BIT_MASK[bit] : 0;
//   buf[x1+x2]   |= color & 4 ? BIT_MASK[bit] : 0;
//   buf[x1+x2*2] |= color & 2 ? BIT_MASK[bit] : 0;
//   buf[x1+x2*3] |= color & 1 ? BIT_MASK[bit] : 0;

}
*/

