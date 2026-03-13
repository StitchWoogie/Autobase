#include "stdafx.h"
#include <glib.h>
#include <ch_buf.h>

void ConvertBuf32BitTo1600(BYTE *tbuf, BYTE *sbuf)
{ 
   int cs, ct;

   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
      tbuf[ct]   = sbuf[cs*4+0];
      tbuf[ct+1] = sbuf[cs*4+1];
      tbuf[ct+2] = sbuf[cs*4+2];
   }
}

void ConvertBuf32768To1600(BYTE *tbuf, BYTE *sbuf)
{ 
   int cs, ct;
   int *ps;

   ps = (int*) sbuf;
   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
      tbuf[ct]   = getbvalue(ps[cs]) << 3;
      tbuf[ct+1] = getgvalue(ps[cs]) << 3;
      tbuf[ct+2] = getrvalue(ps[cs]) << 3;
   }
}

void ConvertBuf256To1600(BYTE *tbuf, BYTE *sbuf)
{
   int cs, ct;
	int color;

   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
		color = sbuf[cs];
      tbuf[ct+2] = dacConvertBuf[color*3+0];
      tbuf[ct+1] = dacConvertBuf[color*3+1];
      tbuf[ct+0] = dacConvertBuf[color*3+2];
   }
}

void ConvertBuf16To1600(BYTE *tbuf, BYTE *sbuf)
{
   int cs, ct;
   long color;

   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
      color = GetColorAllBuf16(sbuf, cs);
      color *= 3;
      tbuf[ct+2] = dacConvertBuf[(WORD)color];
      tbuf[ct+1] = dacConvertBuf[(WORD)color+1];
      tbuf[ct+0]   = dacConvertBuf[(WORD)color+2];
   }
}

void ConvertBuf2To1600(BYTE *tbuf, BYTE *sbuf)
{
   int cs, ct;
   BYTE color;

   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
      color = sbuf[cs>>3] & BIT_MASK[cs%8] ? 255 : 0;
      tbuf[ct+0]   = color;
      tbuf[ct+1] = color;
      tbuf[ct+2] = color;
   }
}

