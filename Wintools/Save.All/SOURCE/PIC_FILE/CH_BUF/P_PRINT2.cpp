#include <stdlib.h>
#include <mem.h>

#include <glib.h>
#include <ch_buf.h>

extern BYTE DITHER_TABLE[64][8];

void convertBuf :: Convert1600To2Print(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k, l;
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int cs;
   int val;

   i = currY%8;     		// 디더링하기위한 y 의 좌표상의 위치.

   memset(tbuf, 0, xbyteplane);

   for(j = 0, cs = 0; j < sizeX; j++,cs+=3) {
      k = j%8; l = j/8;
      val = ((sbuf[cs+2]>>2)*30 + (sbuf[cs+1]>>2)*59 + (sbuf[cs]>>2)*11);
      val += 500;
      if(val > 6300)	val = 6300;
      tbuf[l] |= DITHER_TABLE[val/100][i] & BIT_MASK[k] ? 0 : BIT_MASK[k];
   }

   currY++;
}

void convertBuf :: Convert32768To2Print(BYTE *tbuf, BYTE *sbuf)
{
   register WORD i, j, k, l;
   int xbyteplane = (sizeX+7)/8; 	// 한플랜에 해당하는 바이트수
   int *ps = (int*) sbuf;
   int val;

   i = currY%8;     		// 디더링하기위한 y 의 좌표상의 위치.

   memset(tbuf, 0, xbyteplane);

   for(j = 0; j < sizeX; j++) {
      k = j%8; l = j/8;
      val = ( ( getrvalue(ps[j])<<1 )*30 +
	      ( getgvalue(ps[j])<<1 )*59 +
	      ( getbvalue(ps[j])<<1 )*11);
      val += 500;
      if(val > 6300)	val = 6300;
      tbuf[l] |= DITHER_TABLE
		 [val/100][i] & BIT_MASK[k] ? 0 : BIT_MASK[k];
   }
   currY++;
}

void convertBuf :: Convert256To2Print(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i, j;

   memset(tbuf, 0, (sizeX+7)/8); 	// clear target buffer

   i = currY%8;

   for(j = 0 ; j < sizeX; j++) {
      back = *(sbuf+j);
      tbuf[j/8] |= (((DITHER_TABLE[table[back]][i]& BIT_MASK[j%8]) == 0) ? BIT_MASK[j%8] : 0);
   }

   currY++;
}

void convertBuf :: Convert16To2Print(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int i, j;

   memset(tbuf, 0, (sizeX+7)/8); 	// clear target buffer

   i = currY%8;

   for(j = 0; j < sizeX; j++) {
      back = GetColorAllBuf16(j, sizeX, sbuf);
      tbuf[j/8] |= (((DITHER_TABLE[table[back]][i]& BIT_MASK[j%8]) == 0) ? BIT_MASK[j%8] : 0);
   }

   currY++;
}

void convertBuf :: Convert2To2Print(BYTE *tbuf, BYTE *sbuf)
{
   int ct, i;

   for(ct = 0; ct < byteX; ct++)
      tbuf[ct] = ~sbuf[ct];

   tbuf[byteX-1] = 0;
   for(i = byteX*8; i < sizeX; i++) {	// last byte empty is 0
      tbuf[byteX-1] |= sbuf[byteX-1] & BIT_MASK[i%8] ? 0 : BIT_MASK[i%8];
   }
}

void convertBuf :: Convert2PrintDiffusionExpand(BYTE *tbuf)
{
   int i;
   static char flag;

   flag++;
   flag %= 2;

   memset(tbuf, 0, (sizeX+7)/8);

   if(flag) {
      tbuf[0] = (err1[0] > threshold) ? 0 : BIT_MASK[0];
      for(i = 1; i < sizeX-1; i++) {
	 int err = err1[i] > threshold ? err1[i]-255 : err1[i];
	 tbuf[i/8] |= (err1[i] > threshold) ? 0 : BIT_MASK[i%8];
	 err1[i+1] += (err*7) >> 4;
	 err2[i-1] += (err*3) >> 4;
	 err2[i]   += (err*5) >> 4;
	 err2[i+1] += (err)   >> 4;
      }
      tbuf[(sizeX-1)/8] += (err1[sizeX-1] > threshold) ? 0 : BIT_MASK[(sizeX-1)%8];
   }
   else {
      tbuf[(sizeX-1)/8] = (err1[sizeX-1] > threshold) ? 0 : BIT_MASK[(sizeX-1)%8];
      for(i = sizeX-2; i >= 1; i--) {
	 int err = err1[i] > threshold ? err1[i]-255 : err1[i];
	 tbuf[i/8] |= (err1[i] > threshold) ? 0 : BIT_MASK[i%8];
	 err1[i-1] += (err*7) >> 4;
	 err2[i+1] += (err*3) >> 4;
	 err2[i]   += (err*5) >> 4;
	 err2[i-1] += (err)   >> 4;
      }
      tbuf[0] += (err1[0] > threshold) ? 0 : BIT_MASK[0];
   }
}

void convertBuf :: Convert1600To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   register WORD j;
   int cs;
   int val;

   for(j = 0, cs = 0; j < sizeX; j++,cs+=3) {
      val = sbuf[cs+2]*30 + sbuf[cs+1]*59 + sbuf[cs]*11;
      err1[j] = err2[j]+val/100;
      err2[j] = 0;
   }
   Convert2PrintDiffusionExpand(tbuf);
}

void convertBuf :: Convert32768To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   register WORD j;
   int *ps = (int*) sbuf;
   int val;

   for(j = 0; j < sizeX; j++) {
      val = ( ( getrvalue(ps[j])<<3 )*30 +
	      ( getgvalue(ps[j])<<3 )*59 +
	      ( getbvalue(ps[j])<<3 )*11);
      err1[j] = err2[j]+val/100;
      err2[j] = 0;
   }
   Convert2PrintDiffusionExpand(tbuf);
}

void convertBuf :: Convert256To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int j;

   for(j = 0; j < sizeX; j++) {
      back = *(sbuf+j);
      err1[j] = err2[j]+table[back];
      err2[j] = 0;
   }
   Convert2PrintDiffusionExpand(tbuf);
}

void convertBuf :: Convert16To2PrintDiffusion(BYTE *tbuf, BYTE *sbuf)
{
   BYTE back;
   int j;

   for(j = 0; j < sizeX; j++) {
      back = GetColorAllBuf16(j, sizeX, sbuf);
      err1[j] = err2[j]+table[back];
      err2[j] = 0;
   }
   Convert2PrintDiffusionExpand(tbuf);
}


