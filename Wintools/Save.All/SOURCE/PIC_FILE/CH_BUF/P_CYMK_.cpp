
#include <mem.h>

#include <tools.h>
#include <glib.h>
#include <ch_buf.h>

extern BYTE DITHER_TABLE[64][8];

void convertBuf :: Convert1600ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   int ystart;
   int mstart;
   int kstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;
   BYTE kFlag;

   ystart = xbyteplane;
   mstart = xbyteplane*2;
   kstart = xbyteplane*3;

   memset(tbuf, 0, xbyteplane*4);

   i = currY%8;

   for(k = 0; k < sizeX; k++) {
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[(sbuf[k*3+2]>>3)+32][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[(sbuf[k*3+1]>>3)+32][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[(sbuf[k*3]>>3)+32][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;
      kFlag = OFF;

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
	 kFlag = ON;
      };

      go:

      if(kFlag == ON) {
	 tbuf[kstart+j] |= BIT_MASK[l];
	 continue;
      }
      if(cFlag == ON)
	 tbuf[j] |= BIT_MASK[l];
      if(yFlag == ON)
	 tbuf[ystart+j] |= BIT_MASK[l];
      if(mFlag == ON)
	 tbuf[mstart+j] |= BIT_MASK[l];
   }
   currY++;
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert32768ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   int ystart;
   int mstart;
   int kstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;
   BYTE kFlag;
   int  *intbuf = (int*) sbuf;

   ystart = xbyteplane;
   mstart = xbyteplane*2;
   kstart = xbyteplane*3;

   memset(tbuf, 0, xbyteplane*4);

   i = currY%8;

   for(k = 0; k < sizeX; k++) {
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[getrvalue(intbuf[k])+32][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[getgvalue(intbuf[k])+32][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[getbvalue(intbuf[k])+32][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;
      kFlag = OFF;

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
	 kFlag = ON;
      };

      go:

      if(kFlag == ON) {
	 tbuf[kstart+j] |= BIT_MASK[l];
	 continue;
      }

      if(cFlag == ON)
	 tbuf[j] |= BIT_MASK[l];
      if(yFlag == ON)
	 tbuf[ystart+j] |= BIT_MASK[l];
      if(mFlag == ON)
	 tbuf[mstart+j] |= BIT_MASK[l];
   }
   currY++;
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert256ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   BYTE back;
   int ystart;
   int mstart;
   int kstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;
   BYTE kFlag;

   ystart = xbyteplane;
   mstart = xbyteplane*2;
   kstart = xbyteplane*3;

   memset(tbuf, 0, xbyteplane*4);

   i = currY%8;

   for(k = 0; k < sizeX; k++) {
      back = *(sbuf+k);
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[table[back*3]+32][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[table[back*3+1]+32][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[table[back*3+2]+32][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;
      kFlag = OFF;

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
	 kFlag = ON;
      };

      go:

      if(kFlag == ON) {
	 tbuf[kstart+j] |= BIT_MASK[l];
	 continue;
      }

      if(cFlag == ON)
	 tbuf[j] |= BIT_MASK[l];
      if(yFlag == ON)
	 tbuf[ystart+j] |= BIT_MASK[l];
      if(mFlag == ON)
	 tbuf[mstart+j] |= BIT_MASK[l];
   }
   currY++;
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert16ToCYMKPrintLoss(BYTE *tbuf, BYTE *sbuf)
{
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int i, k, j, l;
   BYTE back;
   int ystart;
   int mstart;
   int kstart;
   BYTE rFlag = 0;
   BYTE gFlag = 0;
   BYTE bFlag = 0;
   BYTE cFlag;
   BYTE yFlag;
   BYTE mFlag;
   BYTE kFlag;

   ystart = xbyteplane;
   mstart = xbyteplane*2;
   kstart = xbyteplane*3;

   memset(tbuf, 0, xbyteplane*4);

   i = currY%8;

   for(k = 0; k < sizeX; k++) {
      back = GetColorAllBuf16(k, sizeX, sbuf);
      j = k/8;
      l = k%8;

      rFlag = (DITHER_TABLE[table[back*3]+32][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[table[back*3+1]+32][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[table[back*3+2]+32][i] & BIT_MASK[l]) ? 1 : 0;

      cFlag = OFF;
      yFlag = OFF;
      mFlag = OFF;
      kFlag = OFF;

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
	 kFlag = ON;
      };

      go:

      if(kFlag == ON) {
	 tbuf[kstart+j] |= BIT_MASK[l];
	 continue;
      }

      if(cFlag == ON)
	 tbuf[j] |= BIT_MASK[l];
      if(yFlag == ON)
	 tbuf[ystart+j] |= BIT_MASK[l];
      if(mFlag == ON)
	 tbuf[mstart+j] |= BIT_MASK[l];
   }
   currY++;
}

