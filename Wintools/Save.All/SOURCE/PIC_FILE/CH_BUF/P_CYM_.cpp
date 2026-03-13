
#include <mem.h>

#include <tools.h>
#include <glib.h>
#include <ch_buf.h>

extern BYTE DITHER_TABLE[64][8];

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert1600ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   int cmFlag;
   int ystart;
   int mstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;

   ystart = xbyteplane;
   mstart = xbyteplane*2;

   memset(tbuf, 0, xbyteplane*3);

   i = currY%8;
   cmFlag = (currY%3);

   for(k = 0; k < sizeX; k++, cmFlag++) {
      cmFlag %= 3;
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[sbuf[k*3+2]>>2][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[sbuf[k*3+1]>>2][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[sbuf[k*3]>>2][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;

      if(rFlag == ON && gFlag == ON && bFlag == ON) {
	 goto go;
      }
      else if(rFlag == ON && gFlag == ON) 	yFlag = ON;
      else if(gFlag == ON && bFlag == ON)	cFlag = ON;
      else if(bFlag == ON && rFlag == ON)	mFlag = ON;
      else if(rFlag == ON) {
	 mFlag = ON;
	 yFlag = ON;
      }
      else if(gFlag == ON) {
	 yFlag = ON;
	 cFlag = ON;
      }
      else if(bFlag == ON) {
	 cFlag = ON;
	 mFlag = ON;
      }
      else {
	 cFlag = ON;
	 yFlag = ON;
	 mFlag = ON;
      };

      go:

      if(cFlag == ON && cmFlag == 0)
	 tbuf[j] |= BIT_MASK[l];
      else if(yFlag == ON && cmFlag == 1)
	 tbuf[ystart+j] |= BIT_MASK[l];
      else if(mFlag == ON && cmFlag == 2)
	 tbuf[mstart+j] |= BIT_MASK[l];
      else;
   }
   currY++;
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert32768ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   int cmFlag;
   int ystart;
   int mstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;
   int  *intbuf = (int*) sbuf;

   ystart = xbyteplane;
   mstart = xbyteplane*2;

   memset(tbuf, 0, xbyteplane*3);

   i = currY%8;
   cmFlag = (currY%3);

   for(k = 0; k < sizeX; k++, cmFlag++) {
      cmFlag %= 3;
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[getrvalue(intbuf[k])<<1][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[getgvalue(intbuf[k])<<1][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[getbvalue(intbuf[k])<<1][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;

      if(rFlag == ON && gFlag == ON && bFlag == ON) {
	 goto go;
      }
      else if(rFlag == ON && gFlag == ON) 	yFlag = ON;
      else if(gFlag == ON && bFlag == ON)	cFlag = ON;
      else if(bFlag == ON && rFlag == ON)	mFlag = ON;
      else if(rFlag == ON) {
	 mFlag = ON;
	 yFlag = ON;
      }
      else if(gFlag == ON) {
	 yFlag = ON;
	 cFlag = ON;
      }
      else if(bFlag == ON) {
	 cFlag = ON;
	 mFlag = ON;
      }
      else {
	 cFlag = ON;
	 yFlag = ON;
	 mFlag = ON;
      };

      go:

      if(cFlag == ON && cmFlag == 0)
	 tbuf[j] |= BIT_MASK[l];
      else if(yFlag == ON && cmFlag == 1)
	 tbuf[ystart+j] |= BIT_MASK[l];
      else if(mFlag == ON && cmFlag == 2)
	 tbuf[mstart+j] |= BIT_MASK[l];
      else;
   }
   currY++;
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert256ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   int cmFlag;
   BYTE back;
   int ystart;
   int mstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;

   ystart = xbyteplane;
   mstart = xbyteplane*2;

   memset(tbuf, 0, xbyteplane*3);

   i = currY%8;
   cmFlag = (currY%3);

   for(k = 0; k < sizeX; k++, cmFlag++) {
      cmFlag %= 3;
      back = *(sbuf+k);
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[table[back*3]][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[table[back*3+1]][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[table[back*3+2]][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;

      if(rFlag == ON && gFlag == ON && bFlag == ON) {
	 goto go;
      }
      else if(rFlag == ON && gFlag == ON) 	yFlag = ON;
      else if(gFlag == ON && bFlag == ON)	cFlag = ON;
      else if(bFlag == ON && rFlag == ON)	mFlag = ON;
      else if(rFlag == ON) {
	 mFlag = ON;
	 yFlag = ON;
      }
      else if(gFlag == ON) {
	 yFlag = ON;
	 cFlag = ON;
      }
      else if(bFlag == ON) {
	 cFlag = ON;
	 mFlag = ON;
      }
      else {
	 cFlag = ON;
	 yFlag = ON;
	 mFlag = ON;
      };

      go:

      if(cFlag == ON && cmFlag == 0)
	 tbuf[j] |= BIT_MASK[l];
      else if(yFlag == ON && cmFlag == 1)
	 tbuf[ystart+j] |= BIT_MASK[l];
      else if(mFlag == ON && cmFlag == 2)
	 tbuf[mstart+j] |= BIT_MASK[l];
      else;
   }
   currY++;
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert16ToCYMPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   int cmFlag;
   BYTE back;
   int ystart;
   int mstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;

   ystart = xbyteplane;
   mstart = xbyteplane*2;

   memset(tbuf, 0, xbyteplane*3);

   i = currY%8;
   cmFlag = (currY%3);

   for(k = 0; k < sizeX; k++, cmFlag++) {
      cmFlag %= 3;
      back = GetColorAllBuf16(k, sizeX, sbuf);
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[table[back*3]][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[table[back*3+1]][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[table[back*3+2]][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;

      if(rFlag == ON && gFlag == ON && bFlag == ON) {
	 goto go;
      }
      else if(rFlag == ON && gFlag == ON) 	yFlag = ON;
      else if(gFlag == ON && bFlag == ON)	cFlag = ON;
      else if(bFlag == ON && rFlag == ON)	mFlag = ON;
      else if(rFlag == ON) {
	 mFlag = ON;
	 yFlag = ON;
      }
      else if(gFlag == ON) {
	 yFlag = ON;
	 cFlag = ON;
      }
      else if(bFlag == ON) {
	 cFlag = ON;
	 mFlag = ON;
      }
      else {
	 cFlag = ON;
	 yFlag = ON;
	 mFlag = ON;
      };

      go:

      if(cFlag == ON && cmFlag == 0)
	 tbuf[j] |= BIT_MASK[l];
      else if(yFlag == ON && cmFlag == 1)
	 tbuf[ystart+j] |= BIT_MASK[l];
      else if(mFlag == ON && cmFlag == 2)
	 tbuf[mstart+j] |= BIT_MASK[l];
      else;
   }
   currY++;
}

void convertBuf :: ConvertCYMPrintDiffusionExpand(BYTE *tbuf)
{
   int i, j, l;
   char c, y, m;
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int ystart = xbyteplane;
   int mstart = xbyteplane*2;

   memset(tbuf, 0, xbyteplane*3);

   c = (cerr1[0] > threshold) ? ON : OFF;
   y = (yerr1[0] > threshold) ? ON : OFF;
   m = (merr1[0] > threshold) ? ON : OFF;

   if(c)	tbuf[0] |= BIT_MASK[0];
   if(y)	tbuf[ystart] |= BIT_MASK[0];
   if(m)	tbuf[mstart] |= BIT_MASK[0];

   for(i = 1; i < sizeX-1; i++) {
      j = i/8;
      l = i%8;

      int cerr = cerr1[i] > threshold ? cerr1[i]-255 : cerr1[i];
      int yerr = yerr1[i] > threshold ? yerr1[i]-255 : yerr1[i];
      int merr = merr1[i] > threshold ? merr1[i]-255 : merr1[i];

      c = (cerr1[i] > threshold) ? ON : OFF;
      y = (yerr1[i] > threshold) ? ON : OFF;
      m = (merr1[i] > threshold) ? ON : OFF;

      if(c)	tbuf[j] |= BIT_MASK[l];
      if(y)	tbuf[ystart+j] |= BIT_MASK[l];
      if(m)	tbuf[mstart+j] |= BIT_MASK[l];

      cerr1[i+1] += (cerr*7) >> 4;
      cerr2[i-1] += (cerr*3) >> 4;
      cerr2[i]   += (cerr*5) >> 4;
      cerr2[i+1] += (cerr)   >> 4;

      yerr1[i+1] += (yerr*7) >> 4;
      yerr2[i-1] += (yerr*3) >> 4;
      yerr2[i]   += (yerr*5) >> 4;
      yerr2[i+1] += (yerr)   >> 4;

      merr1[i+1] += (merr*7) >> 4;
      merr2[i-1] += (merr*3) >> 4;
      merr2[i]   += (merr*5) >> 4;
      merr2[i+1] += (merr)   >> 4;
   }

   c = (cerr1[sizeX-1] > threshold) ? ON : OFF;
   y = (yerr1[sizeX-1] > threshold) ? ON : OFF;
   m = (merr1[sizeX-1] > threshold) ? ON : OFF;

   if(c)	tbuf[(sizeX-1)/8]        |= BIT_MASK[(sizeX-1)%8];
   if(y)	tbuf[ystart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
   if(m)	tbuf[mstart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
}

void convertBuf :: Convert1600ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   int i;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      r_=255-gammaVector[sbuf[i*3+2]];
      g_=255-gammaVector[sbuf[i*3+1]];
      b_=255-gammaVector[sbuf[i*3+0]];

      cerr1[i] = cerr2[i]+r_;
      merr1[i] = merr2[i]+g_;
      yerr1[i] = yerr2[i]+b_;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
   }

   ConvertCYMPrintDiffusionExpand(tbuf);
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert32768ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   int  *intbuf = (int*) sbuf;
   int i;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      r_ = 255 - gammaVector[getrvalue(intbuf[i]) << 3];
      g_ = 255 - gammaVector[getgvalue(intbuf[i]) << 3];
      b_ = 255 - gammaVector[getbvalue(intbuf[i]) << 3];

      cerr1[i] = cerr2[i]+r_;
      merr1[i] = merr2[i]+g_;
      yerr1[i] = yerr2[i]+b_;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
   }

   ConvertCYMPrintDiffusionExpand(tbuf);
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert256ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      back = *(sbuf+i);
      r_ = 255-table[back*3+0];
      g_ = 255-table[back*3+1];
      b_ = 255-table[back*3+2];

      cerr1[i] = cerr2[i]+r_;
      merr1[i] = merr2[i]+g_;
      yerr1[i] = yerr2[i]+b_;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
   }

   ConvertCYMPrintDiffusionExpand(tbuf);
}



//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert16ToCYMPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      back = GetColorAllBuf16(i, sizeX, sbuf);
      r_ = 255-table[back*3+0];
      g_ = 255-table[back*3+1];
      b_ = 255-table[back*3+2];

      cerr1[i] = cerr2[i]+r_;
      merr1[i] = merr2[i]+g_;
      yerr1[i] = yerr2[i]+b_;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
   }

   ConvertCYMPrintDiffusionExpand(tbuf);
}