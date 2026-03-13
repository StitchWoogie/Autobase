#include "stdafx.h"
#include <stdlib.h>
#include <math.h>
//#include <iostream.h>
#include <compiler.hpp>

#include <tools.h>
#include <glib.h>
#include <ch_buf.h>

void ConvertBufSame(BYTE *tbuf, BYTE *sbuf);

void ConvertBuf32BitTo1600(BYTE *tbuf, BYTE *sbuf);

void ConvertBuf1600To32768(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf1600To256(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf1600To16(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf1600To2(BYTE *tbuf, BYTE *sbuf);

void ConvertBuf32768To1600(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf32768To256(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf32768To16(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf32768To2(BYTE *tbuf, BYTE *sbuf);

void ConvertBuf256To1600(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf256To32768(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf256To16(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf256To2(BYTE *tbuf, BYTE *sbuf);

void ConvertBuf16To1600(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf16To32768(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf16To256(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf16To2(BYTE *tbuf, BYTE *sbuf);

void ConvertBuf2To1600(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf2To32768(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf2To256(BYTE *tbuf, BYTE *sbuf);
void ConvertBuf2To16(BYTE *tbuf, BYTE *sbuf);

BYTE dacConvertBuf[768];

//----------------------------------------------------------------------------
//	버퍼를 바꾸기전에 한번 실행해야 한다.
//	tBit - target color
//	sBit - source color
//	xsize - 바꿀 X 픽셀 크기
//	convertform - 바꾸는 방식
//	dac - 파레트
//----------------------------------------------------------------------------

void ConvertBufInit(int tBit, int sBit, int xsize, int /*Convertform*/, BYTE *dac)
{
	int i, j, r, g, b;

	cbSourceBit = sBit;
	cbTargetBit = tBit;
	cbSizeX = xsize;
	cbLineY = 0;      	// 현재의 라인을 Y 로 세팅한다.

	switch(sBit) {
	  case COLOR_32BIT:	cbByteX = xsize*4;			break;
	  case COLOR_1600:	cbByteX = xsize*3;			break;
	  case COLOR_65536:
	  case COLOR_32768: cbByteX = xsize*2;			break;
	  case COLOR_256:	cbByteX = xsize;			break;
	  case COLOR_16:	cbByteX = (xsize+7)/8*4;	break;
	  case COLOR_2:		cbByteX = (xsize+7)/8;		break;
	}

	switch(tBit) {
	  case COLOR_1600:
		  switch(sBit) {
			  case COLOR_256:	
				  memcpy(dacConvertBuf, dac, 768);
				  break;
			  case COLOR_16:	
				  memcpy(dacConvertBuf, dac, 48);
				  break;
		  }
		  break;

	  case COLOR_65536:
	  case COLOR_32768:
		  switch(sBit) {
			  case COLOR_256:	for(i = 0; i < 768; i++) dacConvertBuf[i] = dac[i];
				  break;
			  case COLOR_16:	memcpy(dacConvertBuf, dac, 48);
				  break;
		  }
		  break;
	  case COLOR_256:

		  switch(sBit) {
	  case COLOR_32BIT:
	  case COLOR_1600:
	  case COLOR_65536:
	  case COLOR_32768:
		  for(i = 0, r = 0; r < 8; r++)
			  for(g = 0; g < 8; g++)
				  for(b = 0; b < 4; b++, i+=3) {
					  dacConvertBuf[i] = r << 5;
					  dacConvertBuf[i+1] = g << 5;
					  dacConvertBuf[i+2] = b << 6;
				  }
				  break;
	  case COLOR_256:	memcpy(dacConvertBuf, dac, 768);	break;
	  case COLOR_16:   
		  memcpy(dacConvertBuf, dac, 768);
		  memcpy(dacConvertBuf, dac, 48);
		  break;
	  case COLOR_2:	memcpy(dacConvertBuf, DEFAULT_RGB, 768);
		  break;
		  }
		  break;
	  case COLOR_16:
		  switch(sBit) {
	  case COLOR_1600:
	  case COLOR_65536:
	  case COLOR_32768:
	  case COLOR_256:
		  memcpy(dacConvertBuf, dac, 768);
		  break;
	  case COLOR_16:	
		  memcpy(dacConvertBuf, dac, 48);
		  break;
	  case COLOR_2:       
		  memcpy(dacConvertBuf, DEFAULT_RGB, 48);
		  break;
		  }
		  break;
	  case COLOR_2:
		  switch(sBit) {
	  case COLOR_1600:
	  case COLOR_65536:
	  case COLOR_32768:
		  break;
	  case COLOR_256:     for(i = 0; i < 768; i+=3) {
		  j = (dac[i]>>2)*30 + (dac[i+1]>>2)*59 + (dac[i+2]>>2)*11;
		  dacConvertBuf[i/3] = j/100;
						  }
						  break;
	  case COLOR_16:      for(i = 0; i < 48; i+=3) {
		  j = (dac[i]>>2)*30 + (dac[i+1]>>2)*59 + (dac[i+2]>>2)*11;
		  dacConvertBuf[i/3] = j/100;
						  }
						  break;
	  case COLOR_2:	break;
		  }
		  break;
	}
}

void ConvertBufOneLine(BYTE *t, BYTE *s)
{
	if(cbTargetBit == cbSourceBit) {
		ConvertBufSame(t, s);
		return;
	}
   // set pointer to
   switch(cbTargetBit) {
      case COLOR_1600:
			switch(cbSourceBit) {
				case COLOR_32BIT:		ConvertBuf32BitTo1600(t, s);	break;
				case COLOR_65536:
				case COLOR_32768:		ConvertBuf32768To1600(t, s);	break;
				case COLOR_256:			ConvertBuf256To1600(t, s);		break;
				case COLOR_16:			ConvertBuf16To1600(t, s);		break;
				case COLOR_2:			ConvertBuf2To1600(t, s);		break;
			}
			break;
      case COLOR_65536:
      case COLOR_32768:
			switch(cbSourceBit) {
				case COLOR_1600:		ConvertBuf1600To32768(t, s);	break;
				case COLOR_256:			ConvertBuf256To32768(t, s);		break;
				case COLOR_16:			ConvertBuf16To32768(t, s);		break;
				case COLOR_2:			ConvertBuf2To32768(t, s);		break;
			}
			break;
      case COLOR_256:
			switch(cbSourceBit) {
				case COLOR_1600:		ConvertBuf1600To256(t, s);		break;
				case COLOR_32768:
				case COLOR_65536:		ConvertBuf32768To256(t, s);	break;
				case COLOR_16:			ConvertBuf16To256(t, s);		break;
				case COLOR_2:			ConvertBuf2To256(t, s);			break;
			}
			break;
      case COLOR_16:
			switch(cbSourceBit) {
				case COLOR_1600:		ConvertBuf1600To16(t, s);		break;
				case COLOR_32768:		
				case COLOR_65536:		ConvertBuf32768To16(t, s);		break;
				case COLOR_256:      ConvertBuf256To16(t, s);		break;
				case COLOR_2:			ConvertBuf2To16(t, s);			break;
			}
			break;
      case COLOR_2:
			switch(cbSourceBit) {
				case COLOR_1600:		ConvertBuf1600To2(t, s);		break;
				case COLOR_32768:
				case COLOR_65536:		ConvertBuf32768To2(t, s);		break;
				case COLOR_256:		ConvertBuf256To2(t, s);			break;
				case COLOR_16:			ConvertBuf16To2(t, s);			break;
			}
			break;
      default:		
			MessageBox(NULL, "Convert Buf Error: t Buf unknowned", "convertBuf error", MB_OK);
   }
}

//----------------------------------------------------------------------------
//	버퍼를 바꾸고 난 후 바뀐 파레트 값을 얻는다.
//----------------------------------------------------------------------------

void ConvertBufGetDac(BYTE *dac)
{
   cbLineY = 0;		// 현재의 라인을 0으로 세팅한다.

   switch(cbTargetBit) {
      case COLOR_1600:	
			memcpy(dac, DEFAULT_RGB, 768);
			break;
      case COLOR_256:	
			memcpy(dac, dacConvertBuf, 768);
			break;
      case COLOR_16:
			switch(cbSourceBit) {
				case COLOR_1600:
				case COLOR_65536:
				case COLOR_32768:
				case COLOR_256:
					memcpy(dac, DEFAULT_RGB, 768);
					break;
				case COLOR_16:
				case COLOR_2:
					memcpy(dac, dacConvertBuf, 48);
			}
			break;
		case COLOR_2:
			memcpy(dac, DEFAULT_RGB, 768);
			dac[0] = 0;
			dac[1] = 0;
			dac[2] = 0;
			dac[3] = 255;
			dac[4] = 255;
			dac[5] = 255;
			break;
   }
}

