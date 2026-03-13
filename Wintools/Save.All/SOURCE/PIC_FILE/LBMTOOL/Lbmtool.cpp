#include "stdafx.h"
#include <conio.h>
//#include <iostream.h>
#include <string.h>

#include <compiler.hpp>
#include <tools.h>
#include <lbmtool.h>

lbmClass :: lbmClass() : pictureFileClass()
{

}

lbmClass :: ~lbmClass()
{

}


//----------------------------------------------------------------------------
//	LBM의 바이트 배열은 MOTOROLA 형식이므로 INTEL 형식으로 바꿔줄 필요가
//	있다.
//----------------------------------------------------------------------------

void lbmClass :: HeaderConvertToIBM()
{
   head.fileSize = MOTO2IBM(head.fileSize);

   head.headSize = MOTO2IBM(head.headSize);

   head.width = MOTO2IBM(head.width);
   head.height = MOTO2IBM(head.height);
   head.xoff = MOTO2IBM(head.xoff);
   head.yoff = MOTO2IBM(head.yoff);
   head.tansparent = MOTO2IBM(head.tansparent);
   head.screenWidth = MOTO2IBM(head.screenWidth);
   head.screenHeight = MOTO2IBM(head.screenHeight);
}

//----------------------------------------------------------------------------
//	RGB 정보를 읽어온다.
//	RGB 정보는 "CMAP" 다음에 4바이트의 길이에 따라 plane*3 의 갯수가 온다.
//----------------------------------------------------------------------------

void lbmClass :: ReadRGB()
{
   DWORD rgbsize;
   BYTE id[4];
   int size;

	fread(&id, 1, 4, in);
   if(strncmp((char*)id, "CMAP", 4) != 0)	return;

   fread(&rgbsize, 1, 4, in);

   rgbsize = MOTO2IBM(rgbsize);

   fread(dac, 1, (size_t)rgbsize, in);

	if(nBitsPerPixel == COLOR_2) {
		dac[0] = 0;
		dac[1] = 0;
		dac[2] = 0;
		dac[3] = 255;
		dac[4] = 255;
		dac[5] = 255;
	}

   //****************************************************************
   // 각 정보별로 CMAP(RGB), FORM, GRAB, TINY(약식저장), DPPS, CRNG,
   // 등이 많으나 실제의 이미지는 "BODY" 다음에 오므로 "BODY" 가
   // 나올때까지 포인트를 이동시킨다.
   //****************************************************************

   while(1) {
      size = fread(&id, 1, 4, in);
      if(size != 4)	return;
      if(strncmp((char*)id, "BODY", 4) == 0)	break; 	// 데이타가 시작되면 나간다.

      fread(&rgbsize, 1, 4, in); 		// 각정보의 길이
      rgbsize = MOTO2IBM(rgbsize);
      rgbsize = (rgbsize+1)/2*2;			// 길이가 홀수일때는 짝수로 저장하는것
													// 같다.
      fseek(in, rgbsize, SEEK_CUR);
   }

   size = fread(&rgbsize, 1, 4, in);	// 이미지 데이타의 길이
   rgbsize = MOTO2IBM(rgbsize);
}

int lbmClass :: ReadOpen(HWND hwnd, char *filename)
{
   if(dac == NULL)	return 0;

   in = fopen(filename, "rb");
   if(in == NULL)	{
		ReadOpenError(hwnd, filename);
		return 0;
   }
   if(sizeof(LBMHEADER) != fread(&head, 1, sizeof(LBMHEADER), in)) {
		ReadSizeError(hwnd, filename);
		fclose(in);
		return 0;
   }

   if(strncmp(head.msg1, "FORM", 4) != 0) {
		MessageBox(NULL, "LBM 파일이 아닙니다.", "파일 형식 틀림", MB_OK);
		fclose(in);
		return 0;
   }

	HeaderConvertToIBM();	// MOTO 형식을 IBM형식으로 바꿔준다.	

   nWidth =  head.width;
   nHeight = head.height;

	nBitsPerPixel = head.planes;

   switch(nBitsPerPixel) {
		case COLOR_2:
		case COLOR_16:
		case COLOR_256:
		case COLOR_1600:
			break;
		default:
			char message[256];
			wsprintf(message, "지원되지 않는 색상을 가진 그림입니다.\nhead.nplanes=%d",
							      head.planes);
			MessageBox(NULL, message, "LBM 색상 이상", MB_OK);
			return 0;
   }

	ReadRGB();	// RGB 정보를 읽어온다.
   
   return 1;
}

//---------------------------------------------------------------------------
//	LBM 화일에서 한라인의 그림을 읽어온다..
//	maxx 는 버퍼에서 제한하는 최대값이다.
//	bufcolor 는 채울버퍼의 칼라수를 포함한다.
//---------------------------------------------------------------------------

int lbmClass :: GetOneLine(BYTE *buf, int maxx)
{
   int retn = 0;

   switch ( nBitsPerPixel ) {
      case COLOR_2:
			retn = GetOneLinePlaneTo2(buf, maxx);
	      break;
      case COLOR_16:
	      retn = GetOneLinePlaneTo16(buf, maxx);
	      break;
      case COLOR_256:
	      if( strncmp(head.msg2, "ILBMBMHD", 8) == 0)
				retn = GetOneLinePlaneTo256(buf, maxx);
	      else
				retn = GetOneLinePixelTo256(buf, maxx);
	      break;
		case COLOR_1600:
			retn = GetOneLinePixelTo1600(buf, maxx);
			break;
   }

   return ( retn );
}

//----------------------------------------------------------------------------
//	그림이 플랜방식인 화일을 256버퍼에 읽어온다.
//----------------------------------------------------------------------------

int lbmClass :: GetOneLinePlaneTo256(BYTE *buf, int maxx)
{
   register int xx=0, xb, ch, i;
   register int plane = 0;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head.width+7)/8;

   memset(buf, 0, limitbyte*8);

   if(head.compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
			if(xx < limitbyte) {
				for(i = 0; i < 8; i++) {
					if(ch & BIT_MASK[i])	buf[xx*8+i] += BIT_MASK[7-plane];
				}
				xx++;
				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
			rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
			data = getc(in);
			while ( rc-- > 0) {
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					for( i = 0; i < 8; i++) {
						if(data & BIT_MASK[i]) 	buf[xx*8+i] += BIT_MASK[7-plane];
					}
				}
				xx++;
				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
      else {
			rc = ch+1;
			while ( rc-- > 0) {
				data = fgetc(in);
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					for( i = 0; i < 8; i++) {
						if(data & BIT_MASK[i]) 	buf[xx*8+i] += BIT_MASK[7-plane];
					}
				}
				xx++;

				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 픽셀방식인 화일을 256버퍼에 읽어온다.
//----------------------------------------------------------------------------

int lbmClass :: GetOneLinePixelTo256(BYTE *buf, int maxx)
{
   register int xx=0, xb, ch;
   register int  rc = 0;
   BYTE data;

   int limitbyte = maxx;

   xb = head.width;

   if(head.compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
			if(xx < limitbyte) {
				buf[xx] = ch;
			}
			xx++;
			if(xx >= xb) 		return 1;
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
			rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
			data = getc(in);

			while ( rc-- > 0) {
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					buf[xx] = data;
				}
				xx++;
				if(xx >= xb)	return 1;
			}
      }
      else {
			rc = ch+1;
			while ( rc-- > 0) {
				data = fgetc(in);
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					buf[xx] = data;
				}
				xx++;

				if(xx >= xb) 	return 1;
			}
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 픽셀방식인 화일을 1600버퍼에 읽어온다.
//----------------------------------------------------------------------------

int lbmClass :: GetOneLinePixelTo1600(BYTE *buf, int maxx)
{
	/*
   register int xx=0, xb, ch, i;
   register int plane = 0;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head.width+7)/8;

	memset(buf, 0, limitbyte*24);

   if(head.compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
			if(xx < limitbyte) {
				for(i = 0; i < 8; i++) {
					if(ch & BIT_MASK[i])	buf[xx*24+i] += BIT_MASK[7-plane];
				}
				xx++;
				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
			rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
			data = getc(in);
			while ( rc-- > 0) {
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을 때
					for( i = 0; i < 8; i++) {
						if(data & BIT_MASK[i]) 	buf[xx*8+(plane%3)*24+i] += BIT_MASK[7-plane];
					}
				}
				xx++;
				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
      else {
			rc = ch+1;
			while ( rc-- > 0) {
				data = fgetc(in);
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을 때
					for( i = 0; i < 8; i++) {
						if(data & BIT_MASK[i]) 	buf[xx*24+(plane/3)*8+i] += BIT_MASK[7-plane];
					}
				}
				xx++;

				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
   }
	*/
   return (0);

}


//----------------------------------------------------------------------------
//	그림이 플랜 방식인 화일을 16버퍼에 읽어온다.
//----------------------------------------------------------------------------

int lbmClass :: GetOneLinePlaneTo16(BYTE *buf, int maxx)
{
   register int xx=0, xb, ch;
   register int plane = 0;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head.width+7)/8;

   memset(buf, 0, limitbyte*4);

   if(head.compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
			if(xx < limitbyte) {
				for(int i = 0; i < 8; i+=2) {	// packed memory buffer
					buf[xx*4+i/2] |= ch & BIT_MASK[i]   ? BIT_MASK[plane]   : 0;
					buf[xx*4+i/2] |= ch & BIT_MASK[i+1] ? BIT_MASK[plane+4] : 0;
				}	
				// plane memory
				//	buf[(3-plane)*xb+xx] = ch;
			}
			xx++;
			if(xx >= xb) {
				if(plane >= head.planes-1)	return 1;
				xx = 0;
				plane++;
			}
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
			rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
			data = getc(in);
			while ( rc-- > 0) {
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					for(int i = 0; i < 8; i+=2) {	// packed memory buffer
						buf[xx*4+i/2] |= data & BIT_MASK[i]   ? BIT_MASK[plane]   : 0;
						buf[xx*4+i/2] |= data & BIT_MASK[i+1] ? BIT_MASK[plane+4] : 0;
					}	
					// plane memory buffer
					//buf[(3-plane)*xb+xx] = data;
				}
				xx++;
				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
      else {
			rc = ch+1;
			while ( rc-- > 0) {
				data = fgetc(in);
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					for(int i = 0; i < 8; i+=2) {	// packed memory buffer
						buf[xx*4+i/2] |= data & BIT_MASK[i]   ? BIT_MASK[plane]   : 0;
						buf[xx*4+i/2] |= data & BIT_MASK[i+1] ? BIT_MASK[plane+4] : 0;
					}
					// plane memory buffer
					// buf[(3-plane)*xb+xx] = data;
				}
				xx++;

				if(xx >= xb) {
					if(plane >= head.planes-1)	return 1;
					xx = 0;
					plane++;
				}
			}
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 플랜방식인 화일을 2버퍼에 읽어온다.
//----------------------------------------------------------------------------

int lbmClass :: GetOneLinePlaneTo2(BYTE *buf, int maxx)
{
   register int xx=0, xb, ch;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head.width+7)/8;

   if(head.compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
	 if(xx < limitbyte) {
	    buf[xx] = ch;
	 }
	 xx++;
	 if(xx >= xb) 	return 1;
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
			rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
			data = getc(in);
			while ( rc-- > 0) {
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					buf[xx] = data;
				}
				xx++;
				if(xx >= xb) 	return 1;
			}
      }
      else {
			rc = ch+1;
			while ( rc-- > 0) {
				data = fgetc(in);
				if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
					buf[xx] = data;
				}
				xx++;

				if(xx >= xb) 	return 1;
			}
      }
   }
   return (0);
}

/*
//----------------------------------------------------------------------------
//	LBM의 바이트 배열은 MOTOROLA 형식이므로 INTEL 형식으로 바꿔줄 필요가
//	있다.
//----------------------------------------------------------------------------

void LbmHeaderConvertToIBM(LBMHEADER& head)
{
   head.fileSize = MOTO2IBM(head.fileSize);

   head.headSize = MOTO2IBM(head.headSize);

   head.width = MOTO2IBM(head.width);
   head.height = MOTO2IBM(head.height);
   head.xoff = MOTO2IBM(head.xoff);
   head.yoff = MOTO2IBM(head.yoff);
   head.tansparent = MOTO2IBM(head.tansparent);
   head.screenWidth = MOTO2IBM(head.screenWidth);
   head.screenHeight = MOTO2IBM(head.screenHeight);
}


// 	pcx 의 각종 그림을 256칼라 버퍼에 옮긴다.

int LbmGetOneLinePlaneTo256( FILE *in, LBMHEADER *head, BYTE *buf, int maxx);
int LbmGetOneLinePixelTo256( FILE *in,  LBMHEADER *head, BYTE *buf, int maxx);

//	pcx의 각종그림을 16칼라버퍼에 옮긴다.

int LbmGetOneLinePlaneTo16( FILE *in, LBMHEADER *head, BYTE *buf, int maxx);

int LbmGetOneLinePlaneTo2( FILE *in, LBMHEADER *head, BYTE *buf, int maxx);

//----------------------------------------------------------------------------
//	RGB 정보를 읽어온다.
//	RGB 정보는 "CMAP" 다음에 4바이트의 길이에 따라 plane*3 의 갯수가 온다.
//----------------------------------------------------------------------------

void LbmGetRGB( FILE *in, LBMHEADER *head, BYTE *dac)
{
   DWORD rgbsize;
   BYTE id[4];
   int size;

   fread(&id, 1, 4, in);
   if(strncmp((char*)id, "CMAP", 4) != 0)	return;

   fread(&rgbsize, 1, 4, in);

   rgbsize = MOTO2IBM(rgbsize);

   fread(dac, 1, (size_t)rgbsize, in);

   //****************************************************************
   // 각 정보별로 CMAP(RGB), FORM, GRAB, TINY(약식저장), DPPS, CRNG,
   // 등이 많으나 실제의 이미지는 "BODY" 다음에 오므로 "BODY" 가
   // 나올때까지 포인트를 이동시킨다.
   //****************************************************************

   while(1) {
      size = fread(&id, 1, 4, in);
      if(size != 4)	return;
      if(strncmp((char*)id, "BODY", 4) == 0)	break; 	// 데이타가 시작되면 나간다.

      fread(&rgbsize, 1, 4, in); 	// 각정보의 길이
      rgbsize = MOTO2IBM(rgbsize);
      rgbsize = (rgbsize+1)/2*2;        // 길이가 홀수일때는 짝수로 저장하는것
					// 같다.
      fseek(in, rgbsize, SEEK_CUR);
   }

   size = fread(&rgbsize, 1, 4, in);	// 이미지데이타의 길이
   rgbsize = MOTO2IBM(rgbsize);
}

//---------------------------------------------------------------------------
//	LBM 화일에서 한라인의 그림을 읽어온다..
//	maxx 는 버퍼에서 제한하는 최대값이다.
//	bufcolor 는 채울버퍼의 칼라수를 포함한다.
//---------------------------------------------------------------------------

int LbmGetOneLine(FILE *in, LBMHEADER *head, BYTE *buf, int maxx)
{
   int retn = 0;

   switch ( head->planes ) {
      case COLOR_2:
	       retn = LbmGetOneLinePlaneTo2(in, head, buf, maxx);
	       break;
      case COLOR_16:
	       retn = LbmGetOneLinePlaneTo16(in, head, buf, maxx);
	       break;
      case COLOR_256:
	       if( strncmp(head->msg2, "ILBMBMHD", 8) == 0)
		  retn = LbmGetOneLinePlaneTo256(in, head, buf, maxx);
	       else
		  retn = LbmGetOneLinePixelTo256(in, head, buf, maxx);
	       break;
   }

   return ( retn );
}

//----------------------------------------------------------------------------
//	그림이 플랜방식인 화일을 256버퍼에 읽어온다.
//----------------------------------------------------------------------------

int LbmGetOneLinePlaneTo256( FILE *in, LBMHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, ch, i;
   register int plane = 0;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head->width+7)/8;

   memset(buf, 0, limitbyte*8);

   if(head->compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
	 if(xx < limitbyte) {
	    for(i = 0; i < 8; i++) {
	       if(ch & BIT_MASK[i])	buf[xx*8+i] += BIT_MASK[7-plane];
	    }
	    xx++;
	    if(xx >= xb) {
	       if(plane >= head->planes-1)	return 1;
	       xx = 0;
	       plane++;
	    }
	 }
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
	 rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);
	 while ( rc-- > 0) {
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       for( i = 0; i < 8; i++) {
		  if(data & BIT_MASK[i]) 	buf[xx*8+i] += BIT_MASK[7-plane];
	       }
	    }
	    xx++;
	    if(xx >= xb) {
	       if(plane >= head->planes-1)	return 1;
	       xx = 0;
	       plane++;
	    }
	 }
      }
      else {
	 rc = ch+1;
	 while ( rc-- > 0) {
	    data = fgetc(in);
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       for( i = 0; i < 8; i++) {
		  if(data & BIT_MASK[i]) 	buf[xx*8+i] += BIT_MASK[7-plane];
	       }
	    }
	    xx++;

	    if(xx >= xb) {
	       if(plane >= head->planes-1)	return 1;
	       xx = 0;
	       plane++;
	    }
	 }
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 픽셀방식인 화일을 256버퍼에 읽어온다.
//----------------------------------------------------------------------------
int LbmGetOneLinePixelTo256( FILE *in, LBMHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, ch;
   register int  rc = 0;
   BYTE data;

   int limitbyte = maxx;

   xb = head->width;

   if(head->compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
	 if(xx < limitbyte) {
	    buf[xx] = ch;
	 }
	 xx++;
	 if(xx >= xb) 		return 1;
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
	 rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);

	 while ( rc-- > 0) {
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       buf[xx] = data;
	    }
	    xx++;
	    if(xx >= xb)	return 1;
	 }
      }
      else {
	 rc = ch+1;
	 while ( rc-- > 0) {
	    data = fgetc(in);
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       buf[xx] = data;
	    }
	    xx++;

	    if(xx >= xb) 	return 1;
	 }
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 플랜방식인 화일을 16버퍼에 읽어온다.
//----------------------------------------------------------------------------

int LbmGetOneLinePlaneTo16( FILE *in, LBMHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, ch;
   register int plane = 0;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head->width+7)/8;

   memset(buf, 0, limitbyte*4);

   if(head->compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
	 if(xx < limitbyte) {
	    buf[(3-plane)*xb+xx] = ch;
	 }
	 xx++;
	 if(xx >= xb) {
	    if(plane >= head->planes-1)	return 1;
	    xx = 0;
	    plane++;
	 }
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
	 rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);
	 while ( rc-- > 0) {
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       buf[(3-plane)*xb+xx] = data;
	    }
	    xx++;
	    if(xx >= xb) {
	       if(plane >= head->planes-1)	return 1;
	       xx = 0;
	       plane++;
	    }
	 }
      }
      else {
	 rc = ch+1;
	 while ( rc-- > 0) {
	    data = fgetc(in);
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       buf[(3-plane)*xb+xx] = data;
	    }
	    xx++;

	    if(xx >= xb) {
	       if(plane >= head->planes-1)	return 1;
	       xx = 0;
	       plane++;
	    }
	 }
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 플랜방식인 화일을 2버퍼에 읽어온다.
//----------------------------------------------------------------------------

int LbmGetOneLinePlaneTo2( FILE *in, LBMHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, ch;
   register int  rc = 0;
   BYTE data;

   int limitbyte = (maxx+7)/8;

   xb = (head->width+7)/8;

   if(head->compression == 0) {	// non compression
      while (( ch = getc(in)) != EOF) {
	 if(xx < limitbyte) {
	    buf[xx] = ch;
	 }
	 xx++;
	 if(xx >= xb) 	return 1;
      }
      return 0;
   }

   while( (ch = getc(in)) != EOF) {
      if( ch & 0x80 ) {  	// 1000 0000
	 rc = 0x100 - ch + 1;    // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);
	 while ( rc-- > 0) {
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       buf[xx] = data;
	    }
	    xx++;
	    if(xx >= xb) 	return 1;
	 }
      }
      else {
	 rc = ch+1;
	 while ( rc-- > 0) {
	    data = fgetc(in);
	    if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
	       buf[xx] = data;
	    }
	    xx++;

	    if(xx >= xb) 	return 1;
	 }
      }
   }
   return (0);
}

*/

