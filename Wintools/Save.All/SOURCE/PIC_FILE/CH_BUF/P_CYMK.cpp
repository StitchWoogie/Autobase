#include <mem.h>

#include <tools.h>
#include <glib.h>
#include <ch_buf.h>

extern BYTE DITHER_TABLE[64][8];

void convertBuf :: Convert1600ToCYMKPrint(BYTE *tbuf, BYTE *sbuf)
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

      rFlag = (DITHER_TABLE[(gammaVector[sbuf[k*3+2]]>>2)][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[(gammaVector[sbuf[k*3+1]]>>2)][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[(gammaVector[sbuf[k*3]]>>2)][i] & BIT_MASK[l]) ? 1 : 0;

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

void convertBuf :: Convert32768ToCYMKPrint(BYTE *tbuf, BYTE *sbuf)
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

      rFlag = (DITHER_TABLE[gammaVector[getrvalue(intbuf[k])<<3] >> 2][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[gammaVector[getgvalue(intbuf[k])<<3] >> 2][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[gammaVector[getbvalue(intbuf[k])<<3] >> 2][i] & BIT_MASK[l]) ? 1 : 0;

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

void convertBuf :: Convert256ToCYMKPrint(BYTE *tbuf, BYTE *sbuf)
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

      rFlag = (DITHER_TABLE[table[back*3]][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[table[back*3+1]][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[table[back*3+2]][i] & BIT_MASK[l]) ? 1 : 0;

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

void convertBuf :: Convert16ToCYMKPrint(BYTE *tbuf, BYTE *sbuf)
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

      rFlag = (DITHER_TABLE[table[back*3]][i] & BIT_MASK[l]) ? 1 : 0;
      gFlag = (DITHER_TABLE[table[back*3+1]][i] & BIT_MASK[l]) ? 1 : 0;
      bFlag = (DITHER_TABLE[table[back*3+2]][i] & BIT_MASK[l]) ? 1 : 0;

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
//	CYMK 프린터에서 흑백이미지는 K 만 세팅한다.
//----------------------------------------------------------------------------

void convertBuf :: Convert2ToCYMKPrint(BYTE *tbuf, BYTE *sbuf)
{
   int i;
   int pos;

   memset(tbuf, 0, byteX*3);	// set CYM plane to 0

   for(i = 0, pos = byteX*3; i < byteX; i++, pos++) {	// fill K plane
      tbuf[pos] = ~sbuf[i];
   }
}

void convertBuf :: ConvertCYMKPrintDiffusionExpand(BYTE *tbuf)
{
   int i;
   char c, y, m, k;
   static char flag;

   flag ++;
   flag %= 2;

   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int ystart = xbyteplane;
   int mstart = xbyteplane*2;
   int kstart = xbyteplane*3;

   memset(tbuf, 0, xbyteplane*4);

   if(flag) {
      c = (cerr1[0] > threshold) ? ON : OFF;
      y = (yerr1[0] > threshold) ? ON : OFF;
      m = (merr1[0] > threshold) ? ON : OFF;
      k = (kerr1[0] > threshold) ? ON : OFF;

      if(k || (c&&y&&m)) {	// K
	 tbuf[kstart] |= BIT_MASK[0];
      }
      else {
	 if(c)	tbuf[0] |= BIT_MASK[0];
	 if(y)	tbuf[ystart] |= BIT_MASK[0];
	 if(m)	tbuf[mstart] |= BIT_MASK[0];
      }

      for(i = 1; i < sizeX-1; i++) {
	 c = (cerr1[i] > threshold) ? ON : OFF;
	 y = (yerr1[i] > threshold) ? ON : OFF;
	 m = (merr1[i] > threshold) ? ON : OFF;
	 k = (kerr1[i] > threshold) ? ON : OFF;

	 int cerr, yerr, merr, kerr;

	 if(k == ON) {
	   cerr = cerr1[i];
	   yerr = yerr1[i];
	   merr = merr1[i];
	   kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];
	 }
	 else {
	   cerr = cerr1[i] > threshold ? cerr1[i]-255 : cerr1[i];
	   yerr = yerr1[i] > threshold ? yerr1[i]-255 : yerr1[i];
	   merr = merr1[i] > threshold ? merr1[i]-255 : merr1[i];
	   kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];
	 }

	 //cerr = cerr1[i] > threshold ? cerr1[i]-255 : cerr1[i];
	 //yerr = yerr1[i] > threshold ? yerr1[i]-255 : yerr1[i];
	 //merr = merr1[i] > threshold ? merr1[i]-255 : merr1[i];
	 //kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];

	 if(k || (c&&y&&m)) {	// K
	    tbuf[kstart+i/8] |= BIT_MASK[i%8];
	 }
	 else {
	    if(c)	tbuf[i/8] |= BIT_MASK[i%8];
	    if(y)	tbuf[ystart+i/8] |= BIT_MASK[i%8];
	    if(m) 	tbuf[mstart+i/8] |= BIT_MASK[i%8];
	 }

	 // diffuse red error
	 cerr1[i+1] += (cerr*7) >> 4;
	 cerr2[i-1] += (cerr*3) >> 4;
	 cerr2[i]   += (cerr*5) >> 4;
	 cerr2[i+1] += (cerr)   >> 4;

	 // diffuse green error
	 yerr1[i+1] += (yerr*7) >> 4;
	 yerr2[i-1] += (yerr*3) >> 4;
	 yerr2[i]   += (yerr*5) >> 4;
	 yerr2[i+1] += (yerr)   >> 4;

	 // diffuse blue error
	 merr1[i+1] += (merr*7) >> 4;
	 merr2[i-1] += (merr*3) >> 4;
	 merr2[i]   += (merr*5) >> 4;
	 merr2[i+1] += (merr)   >> 4;

	 // diffuse blue error
	 kerr1[i+1] += (kerr*7) >> 4;
	 kerr2[i-1] += (kerr*3) >> 4;
	 kerr2[i]   += (kerr*5) >> 4;
	 kerr2[i+1] += (kerr)   >> 4;
      }

      c = (cerr1[sizeX-1] > threshold) ? ON : OFF;
      y = (yerr1[sizeX-1] > threshold) ? ON : OFF;
      m = (merr1[sizeX-1] > threshold) ? ON : OFF;
      k = (kerr1[sizeX-1] > threshold) ? ON : OFF;

      if(k || (c&&y&&m)) {	// K
	 tbuf[kstart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
      }
      else {
	 if(c)	tbuf[0+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
	 if(y)	tbuf[ystart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
	 if(m)	tbuf[mstart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
      }
   }
   else {
      c = (cerr1[sizeX-1] > threshold) ? ON : OFF;
      y = (yerr1[sizeX-1] > threshold) ? ON : OFF;
      m = (merr1[sizeX-1] > threshold) ? ON : OFF;
      k = (kerr1[sizeX-1] > threshold) ? ON : OFF;

      if(k || (c&&y&&m)) {	// K
	 tbuf[kstart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
      }
      else {
	 if(c)	tbuf[0+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
	 if(y)	tbuf[ystart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
	 if(m)	tbuf[mstart+(sizeX-1)/8] |= BIT_MASK[(sizeX-1)%8];
      }

      for(i = sizeX-2; i >= 1; i--) {
	 //int cerr = cerr1[i] > threshold ? cerr1[i]-255 : cerr1[i];
	 //int yerr = yerr1[i] > threshold ? yerr1[i]-255 : yerr1[i];
	 //int merr = merr1[i] > threshold ? merr1[i]-255 : merr1[i];
	 //int kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];

	 c = (cerr1[i] > threshold) ? ON : OFF;
	 y = (yerr1[i] > threshold) ? ON : OFF;
	 m = (merr1[i] > threshold) ? ON : OFF;
	 k = (kerr1[i] > threshold) ? ON : OFF;

	 int cerr, yerr, merr, kerr;

	 if(k == ON) {
	   cerr = cerr1[i];
	   yerr = yerr1[i];
	   merr = merr1[i];
	   kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];
	 }
	 else {
	   cerr = cerr1[i] > threshold ? cerr1[i]-255 : cerr1[i];
	   yerr = yerr1[i] > threshold ? yerr1[i]-255 : yerr1[i];
	   merr = merr1[i] > threshold ? merr1[i]-255 : merr1[i];
	   kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];
	 }
	 //cerr = cerr1[i] > threshold ? cerr1[i]-255 : cerr1[i];
	 //yerr = yerr1[i] > threshold ? yerr1[i]-255 : yerr1[i];
	 //merr = merr1[i] > threshold ? merr1[i]-255 : merr1[i];
	 //kerr = kerr1[i] > threshold ? kerr1[i]-255 : kerr1[i];

	 if(k || (c&&y&&m) ) {	// K
	    tbuf[kstart+i/8] |= BIT_MASK[i%8];
	 }
	 else {
	    if(c)	tbuf[i/8] |= BIT_MASK[i%8];
	    if(y)	tbuf[ystart+i/8] |= BIT_MASK[i%8];
	    if(m) 	tbuf[mstart+i/8] |= BIT_MASK[i%8];
	 }

	 // diffuse red error
	 cerr1[i-1] += (cerr*7) >> 4;
	 cerr2[i+1] += (cerr*3) >> 4;
	 cerr2[i]   += (cerr*5) >> 4;
	 cerr2[i-1] += (cerr)   >> 4;

	 // diffuse green error
	 yerr1[i-1] += (yerr*7) >> 4;
	 yerr2[i+1] += (yerr*3) >> 4;
	 yerr2[i]   += (yerr*5) >> 4;
	 yerr2[i-1] += (yerr)   >> 4;

	 // diffuse blue error
	 merr1[i-1] += (merr*7) >> 4;
	 merr2[i+1] += (merr*3) >> 4;
	 merr2[i]   += (merr*5) >> 4;
	 merr2[i-1] += (merr)   >> 4;

	 // diffuse blue error
	 kerr1[i-1] += (kerr*7) >> 4;
	 kerr2[i+1] += (kerr*3) >> 4;
	 kerr2[i]   += (kerr*5) >> 4;
	 kerr2[i-1] += (kerr)   >> 4;
      }

      c = (cerr1[0] > threshold) ? ON : OFF;
      y = (yerr1[0] > threshold) ? ON : OFF;
      m = (merr1[0] > threshold) ? ON : OFF;
      k = (kerr1[0] > threshold) ? ON : OFF;

      if(k || (c&&y&&m)) {	// K
	 tbuf[kstart] |= BIT_MASK[0];
      }
      else {
	 if(c)	tbuf[0] |= BIT_MASK[0];
	 if(y)	tbuf[ystart] |= BIT_MASK[0];
	 if(m)	tbuf[mstart] |= BIT_MASK[0];
      }
   }
}

void convertBuf :: Convert1600ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   int i, j;
   int c, m, y, k;
   //int mid;
   //int maxPart, minPart;

   for(i = 0, j = 0; i < sizeX; i++, j+=3) {
      c = 255-gammaVector[sbuf[j+2]];
      m = 255-gammaVector[sbuf[j+1]];
      y = 255-gammaVector[sbuf[j+0]];
      //c = 255-sbuf[j+2];
      //m = 255-sbuf[j+1];
      //y = 255-sbuf[j+0];

      /*
      mid = (c+y+m)/3;
      //mid = 127;

      c += (c-mid)*0.2;
      y += (y-mid)*0.2;
      m += (m-mid)*0.15;

      c = min(c, 255);
      y = min(y, 255);
      m = min(m, 255);

      c = max(c, 0);
      y = max(y, 0);
      m = max(m, 0);

      */
      k = min(c, m);
      k = min(y, k);

      //c -= k;
      //y -= k;
      //m -= k;

      //c = 255-gammaVector[255-c];
      //y = 255-gammaVector[255-y];
      //m = 255-gammaVector[255-m];
      //k = 255-gammaVector[255-k];

      //c *= 1.2;
      //y *= 1.2;
      //m *= 1.2;

      cerr1[i] = cerr2[i]+c-k;
      merr1[i] = merr2[i]+m-k;
      yerr1[i] = yerr2[i]+y-k;
      kerr1[i] = kerr2[i]+k;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
      kerr2[i] = 0;
   }

   ConvertCYMKPrintDiffusionExpand(tbuf);
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert32768ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   int  *intbuf = (int*) sbuf;
   int i, k;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      r_ = 255 - gammaVector[getrvalue(intbuf[i])<<3];
      g_ = 255 - gammaVector[getgvalue(intbuf[i])<<3];
      b_ = 255 - gammaVector[getbvalue(intbuf[i])<<3];

      k = min(r_, g_);
      k = min(k, b_);

      cerr1[i] = cerr2[i]+r_-k;
      merr1[i] = merr2[i]+g_-k;
      yerr1[i] = yerr2[i]+b_-k;
      kerr1[i] = kerr2[i]+k;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
      kerr2[i] = 0;
   }

   ConvertCYMKPrintDiffusionExpand(tbuf);
}

//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert256ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i, k;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      back = *(sbuf+i);
      r_ = 255-table[back*3+0];
      g_ = 255-table[back*3+1];
      b_ = 255-table[back*3+2];

      k = min(r_, g_);
      k = min(k, b_);

      cerr1[i] = cerr2[i]+r_-k;
      merr1[i] = merr2[i]+g_-k;
      yerr1[i] = yerr2[i]+b_-k;
      kerr1[i] = kerr2[i]+k;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
      kerr2[i] = 0;
   }

   ConvertCYMKPrintDiffusionExpand(tbuf);
}



//----------------------------------------------------------------------------
//	버퍼를 프린터 버퍼중에서 CYM 버퍼로 바꿔준다.
//	Red   = Magenta + Yellow
//	Green = Magenta  + Cyan
//	Blue  = Cyan    + Yellow
//----------------------------------------------------------------------------

void convertBuf :: Convert16ToCYMKPrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i, k;
   BYTE r_, g_, b_;

   for(i = 0; i < sizeX; i++) {
      back = GetColorAllBuf16(i, sizeX, sbuf);
      r_ = 255-table[back*3+0];
      g_ = 255-table[back*3+1];
      b_ = 255-table[back*3+2];

      k = min(r_, g_);
      k = min(k, b_);

      cerr1[i] = cerr2[i]+r_-k;
      merr1[i] = merr2[i]+g_-k;
      yerr1[i] = yerr2[i]+b_-k;
      kerr1[i] = kerr2[i]+k;

      cerr2[i] = 0;
      merr2[i] = 0;
      yerr2[i] = 0;
      kerr2[i] = 0;
   }

   ConvertCYMKPrintDiffusionExpand(tbuf);
}