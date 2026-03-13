#include "stdafx.h"
#include <glib.h>
#include <ch_buf.h>

extern BYTE cbSourceBit, cbTargetBit, cbConvertFlag;
extern int  cbByteX, cbSizeX;
//extern int  cbLineY;	// 디더링 할때 현재의 라인수가 필요하다.

void ConvertBuf1600To256(BYTE *tbuf, BYTE *sbuf)
{
   int cs, ct;

   for(cs = 0, ct = 0; ct < cbSizeX; ct++, cs+=3)
      tbuf[ct] = (sbuf[cs+2] & 0xE0) | ((sbuf[cs+1] & 0xE0) >> 3) | (sbuf[cs] >> 6);
}

void ConvertBuf32768To256(BYTE *tbuf, BYTE *sbuf)
{
   int ct;
   int *ps;

   ps = (int*) sbuf;
   for(ct = 0; ct < cbSizeX; ct++)
      tbuf[ct] = ((getrvalue(ps[ct]) & 0x1C) << 3) |
		 ((getgvalue(ps[ct]) & 0x1C)) |
		  (getbvalue(ps[ct]) >> 3);
}

void ConvertBuf16To256(BYTE *tbuf, BYTE *sbuf)
{
   int cs;

   for(cs = 0; cs < cbSizeX; cs++)
      tbuf[cs] = GetColorAllBuf16(sbuf, cs);
}

void ConvertBuf2To256(BYTE *tbuf, BYTE *sbuf)
{
   int ct;

   for(ct = 0; ct < cbSizeX; ct++)
      tbuf[ct] = (BIT_MASK[ct%8] & sbuf[ct/8]) ? 255 : 0 ;
}

