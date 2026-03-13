#include "stdafx.h"
#include <glib.h>
#include <ch_buf.h>

void ConvertBuf1600To32768(BYTE *tbuf, BYTE *sbuf)
{
   int cs, ct;
   int *pt;

   pt = (int*) tbuf;
   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
      pt[cs] = rgb(sbuf[ct+2] >> 3,
		   sbuf[ct+1] >> 3,
		   sbuf[ct] >> 3);
   }
}

void ConvertBuf256To32768(BYTE *tbuf, BYTE *sbuf)
{
   int cs;
   int *pt;

   pt = (int*) tbuf;
   for(cs = 0; cs < cbSizeX; cs++) {
      pt[cs] = rgb(dacConvertBuf[sbuf[cs]*3] >> 3,
		   dacConvertBuf[sbuf[cs]*3+1] >> 3,
		   dacConvertBuf[sbuf[cs]*3+2] >> 3);
   }
}

void ConvertBuf16To32768(BYTE *tbuf, BYTE *sbuf)
{
   int cs, ct;
   int *pt;
   long color;

   pt = (int*) tbuf;
   for(cs = 0, ct = 0; cs < cbSizeX; cs++, ct+=3) {
      color = GetColorAllBuf16(sbuf, cs);
      color*=3;
      pt[cs] = rgb(dacConvertBuf[(WORD)color] >> 3,
		   dacConvertBuf[(WORD)color+1] >> 3,
		   dacConvertBuf[(WORD)color+2] >> 3);
   }
}

void ConvertBuf2To32768(BYTE *tbuf, BYTE *sbuf)
{
   int cs;
   int *pt;

   pt = (int*) tbuf;
   for(cs = 0; cs < cbSizeX; cs++) {
     pt[cs] = sbuf[cs>>3] & BIT_MASK[cs%8] ? 32767u : 0;
   }
}
