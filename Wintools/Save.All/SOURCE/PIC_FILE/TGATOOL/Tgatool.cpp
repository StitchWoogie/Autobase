#include "stdafx.h"
#include <math.h>

#include <tools.h>
#include <glib.h>
#include <compiler.hpp>

#include "tgatool.h"

// pcx 의 각종 그림을 256칼라 버퍼에 옮긴다.
int TgaGetOneLine1600To1600( FILE *in, TGAHEADER head, BYTE *buf, int maxx);
int TgaGetOneLine32768To32768( FILE *in, TGAHEADER head, BYTE *buf, int maxx);
int TgaGetOneLine256To256( FILE *in, TGAHEADER head, BYTE *buf, int maxx);
int TgaGetOneLine16To16( FILE *in, TGAHEADER head, BYTE *buf, int maxx);
int TgaGetOneLine2To2( FILE *in, TGAHEADER head, BYTE *buf, int maxx);

tgaClass :: tgaClass() : pictureFileClass()
{
	nRemainCount = 0;	// 일단 다른색상은 이상한 점을 발견하지 못했으나. 32768색상에서 압축이
							// 한라인에서 끝나는 것이 아니고 계속이어지는 파일이 있다.
							// 압축을 덜 풀었을때는 다음라인까지 계속된다.
}

tgaClass :: ~tgaClass()
{

}

int tgaClass :: ReadOpen(char *filename)
{
	nRemainCount = 0;	// 일단 다른색상은 이상한 점을 발견하지 못했으나. 32768색상에서 압축이
							// 한라인에서 끝나는 것이 아니고 계속이어지는 파일이 있다.
							// 압축을 덜 풀었을때는 다음라인까지 계속된다.

   if(dac == NULL)	return 0;

   in = fopen(filename, "rb");
   if(in == NULL)	return 0;
   fread(&head, 1, sizeof(TGAHEADER), in);

   nWidth = head.xsize;
   nHeight = head.ysize;
   nBitsPerPixel = head.bitsperpixel;
   if(nBitsPerPixel == 32) {
      nBitsPerPixel = COLOR_1600;
   }
	
	if(nBitsPerPixel == COLOR_32768 || nBitsPerPixel == COLOR_65536) {
		nBitsPerPixel = COLOR_1600;
	}
	
   // 0  -  No image data included.
   // 1  -  Uncompressed, color-mapped images.
   // 2  -  Uncompressed, RGB images.
   // 3  -  Uncompressed, black and white images.
   // 9  -  Runlength encoded color-mapped images.
   //10  -  Runlength encoded RGB images.
   //11  -  Compressed, black and white images.
   //32  -  Compressed color-mapped data, using Huffman, Delta, and
   //	    runlength encoding.
   //33  -  Compressed color-mapped data, using Huffman, Delta, and
   //	    runlength encoding.  4-pass quadtree-type process.

   image_type = head.imagetype;

   // Image Descriptor Byte.                                    |
   // Bits 3-0 - number of attribute bits associated with each  |
   //	   	pixel.                                         |
   // Bit 4    - reserved.  Must be set to 0.                   |
   // Bit 5    - screen origin bit.                             |
   //	   	0 = Origin in lower left-hand corner.          |
   //	   	1 = Origin in upper left-hand corner.          |
   //	   	Must be 0 for Truevision images.               |
   //Bits 7-6 - Data storage interleaving flag.                |
   //	   	00 = non-interleaved.                          |
   //	   	01 = two-way (even/odd) interleaving.          |
   //	   	10 = four way interleaving.                    |
   //	   	11 = reserved.                                 |

   image_order = head.imagedesc & 0x20 ? IMAGE_ORDER_DOWN : IMAGE_ORDER_UP;

   switch(head.bitsperpixel) {
      case COLOR_2:		nBytesPerLine = (nWidth+7)/8;	break;
      case COLOR_16:    nBytesPerLine = (nWidth);	break;
      case COLOR_256:	nBytesPerLine = nWidth;		break;
      case COLOR_32768:
      case COLOR_65536: nBytesPerLine = nWidth*2; 	break;
      case COLOR_1600:  nBytesPerLine = nWidth*3;	break;
		case 32:				nBytesPerLine = nWidth*4;	break;
      default:		fclose(in);
			return 0;
   }

   BYTE temp;
   int i;

   fseek(in, head.idlen, SEEK_CUR);

   switch( head.bitsperpixel ) {
      case COLOR_256:
			fread(dac, 1, head.cmlen*3, in);
			for(i = 0; i < head.cmlen; i++) {	// 3 color map B,G,R
				temp = dac[i*3];
				dac[i*3] = dac[i*3+2];
				dac[i*3+2] = temp;
			}
			break;
      case COLOR_16:
			fread(dac, 1, 48, in);
			break;
		case COLOR_2:
			for(i = 0; i < 3; i++) dac[i] = 0;
			for(i = 3; i < 6; i++) dac[i] = 255;
			break;
   }

   // RGB image ingnore Color Map Table
   // 보통 cmbits가 24이나 65536칼라일 때는 16이다.
   if(head.cmtype)
      fseek(in, (long)sizeof(TGAHEADER)+head.idlen+head.cmlen*((head.cmbits+7)/8), SEEK_SET);
   else
      fseek(in, (long)sizeof(TGAHEADER)+head.idlen, SEEK_SET);

   return 1;
}

int tgaClass :: GetOneLineType10_32768(BYTE *buf, int maxx)
{
   int ch;
   int x = 0;
	BYTE r, g, b;

	if(nRemainCount > 0) {	// 이전라인에서 압축이 연결될 경우
		nRemainCount--;
		if(bRemainSameOrDifferent == 0)	goto same;
		else										goto diffrent;
	}
	
   while(1) {
      if(x >= nWidth)	break;
      ch = fgetc(in);
      if(ch == EOF)	return 0;
      if(ch >= 128) {	// same count;
			bRemainSameOrDifferent = 0;
			nRemainCount = ((BYTE)ch & 0x7F)+1;//1-(char)ch;	// key point.
			fread(&wRemainWord, 1, 2, in);
			r = getrvalue(wRemainWord) << 3;
			g = getgvalue(wRemainWord) << 3;
			b = getbvalue(wRemainWord) << 3;
			while(nRemainCount-- > 0) {
			  same:
				if(x < maxx) {
					buf[x*3+0] = b;
					buf[x*3+1] = g;
					buf[x*3+2] = r;
				}
				x++;
				if(x >= nWidth)	return 1;
			}
      }
      else {
			bRemainSameOrDifferent = 1;
			nRemainCount = ch+1;	//1+ch;
			while(nRemainCount-- > 0) {
			  diffrent:
				fread(&wRemainWord, 1, 2, in);
				if(x < maxx) {
				//	fread(&imsi, 1, 2, in);
					r = getrvalue(wRemainWord) << 3;
					g = getgvalue(wRemainWord) << 3;
					b = getbvalue(wRemainWord) << 3;
					buf[x*3+0] = b;
					buf[x*3+1] = g;
					buf[x*3+2] = r;
				}
				else {
				//	if(x < nWidth)	fseek(in, 2, SEEK_CUR);
				}
				x++;
				if(x >= nWidth)	return 1;
			}
      }
   }

	//fseek(in, -1, SEEK_CUR);

   return 1;
}

int tgaClass :: GetOneLineType10_1600(BYTE *buf, int maxx)
{
   int ch;
   int count;
   int x = 0;
   int maxbyte = maxx*3;
   BYTE imsi[5];

   while(1) {
      if(x >= nBytesPerLine)	break;
      ch = fgetc(in);
      if(ch == EOF)	return 0;
      if(ch >= 128) {	// same count;
			count = ((BYTE)ch & 0x7F) + 1;	// key point.
			fread(&imsi, 1, 3, in);
			while(count-- > 0) {
				if(x < maxbyte) {
					buf[x] = imsi[0];
					buf[x+1] = imsi[1];
					buf[x+2] = imsi[2];
				}
				x+=3;
			}
      }
      else {
			count = ch+1;
			while(count-- > 0) {
				if(x < maxbyte) {
					fread(buf+x, 1, 3, in);
				}
				else {
					fseek(in, 3, SEEK_CUR);
				}
				x+=3;
			}
      }
   }

   return 1;
}

int tgaClass :: GetOneLineType10_32bit(BYTE *buf, int maxx)
{
   int ch;
   int count;
   int x = 0;
   BYTE imsi[5];

   while(1) {
      if(x >= nWidth)	break;
      ch = fgetc(in);
      if(ch == EOF)	return 0;
      if(ch >= 128) {	// same count;
			count = ((BYTE)ch & 0x7F)+1;	// key point.
			fread(&imsi, 1, 4, in);
			while(count-- > 0) {
				if(x < maxx) {
					buf[x*3+0] = imsi[0];
					buf[x*3+1] = imsi[1];
					buf[x*3+2] = imsi[2];
				}
				x++;
			}
      }
      else {
			count = 1+ch;
			while(count-- > 0) {
				if(x < maxx) {
					fread(buf+x*3, 1, 3, in);
					fseek(in, 1, SEEK_CUR);
				}
				else {
					fseek(in, 4, SEEK_CUR);
				}
				x++;
			}
      }
   }

   return 1;
}

//----------------------------------------------------------------------------
//      Run length encoded RGB images
//----------------------------------------------------------------------------
int tgaClass :: GetOneLineType10(BYTE *buf, int maxx)
{
   switch(head.bitsperpixel) {
      case COLOR_32768:
      case COLOR_65536:	return GetOneLineType10_32768(buf, maxx);
      case COLOR_1600:	return GetOneLineType10_1600(buf, maxx);
      case 32:				return GetOneLineType10_32bit(buf, maxx);
   }
   return 1;
}

int tgaClass :: GetOneLine(BYTE *buf, int maxx)
{
   switch(image_type) {
      case 0:	return 1;	// no image data included
      case 10: return(GetOneLineType10(buf, maxx));
      default:	fread(buf, 1, nBytesPerLine, in);
		// if(screen_order == SCREEN_BOTTOM_TOP)
		// fseek(in, -(nBytesPerLine*2), SEEK_CUR);
   }
   return 1;
}

//---------------------------------------------------------------------------
//	TGA 파일에서 RGB 정보를 쓴다.
//	여기서는 256 칼라일때만 유용하므로 나머지는 필요없다.
//	파레트는 2,3,4 바이트 세가지가 있지만
//	여기서는 항상 3바이트 BGR 파레트를 쓴다.
//	2 byte - ARRRRRGG GGGBBBBB
//	3 byte - Blue Green Red
//	4 byte - Blue Green Red Attribute
//---------------------------------------------------------------------------

void tgaClass :: PutRGB(TGAHEADER *head, BYTE *dac)
{
   int i;

   if(nBitsPerPixel != 8) {
      return;
   }

   if(head->cmlen == 2) {
      fputc(0, out);	fputc(0, out);		fputc(0, out);
      fputc(255, out);	fputc(255, out);	fputc(255, out);
      return;
   }

   for(i = 0; i < head->cmlen; i++) {	// because BGR color map
      fputc(dac[i*3+2], out);
      fputc(dac[i*3+1], out);
      fputc(dac[i*3+0], out);
   }
}


void tgaClass :: MakeHeader(TGAHEADER *head)
{
   head->idlen = 0;			// number of bytes in Ident Field
   head->cmorg = 0;        // color map origin

	switch(nBitsPerPixel) {
		case COLOR_256:
			head->imagetype = 1;		// color map image
			head->cmtype = 1;  		// color map type (0 for none)
			head->cmlen = 256;      // color map length in # of colors
			head->cmbits = 24;      // number of bits per color map entry
			head->bitsperpixel = 8;
			break;
		case COLOR_16:
			head->imagetype = 1;		// color map image
			head->cmtype = 1;
			head->cmlen = 16;
			head->cmbits = 24;
			head->bitsperpixel = 8;
			break;
		case COLOR_2:
			head->imagetype = 1;		// B/W image type
			head->cmtype = 1;
			head->cmlen = 2;
			head->cmbits = 24;
			head->bitsperpixel = 8;
			break;
		case COLOR_32768:
			head->imagetype = 2;		// RGB image type
			head->cmtype = 0;
			head->cmlen = 0;
			head->cmtype = 0;
			head->bitsperpixel = COLOR_32768;
			break;
		case COLOR_1600:
			head->imagetype = 2;		// RGB image type
			head->cmtype = 0;
			head->cmlen = 0;
			head->cmtype = 0;
			head->bitsperpixel = COLOR_1600;
			break;
	}

   head->xoff = 0;
   head->yoff = 0;
   head->xsize = nWidth;
   head->ysize = nHeight;
   head->imagedesc = 0;          // 0 for TGA and TARGA images
}


int tgaClass :: WriteOpen(char *filename)
{
	if(nBitsPerPixel == COLOR_32BIT) {
		MessageBox(NULL, "지원되지 않는 색상", "Tag File", MB_OK);
		return 0;
	}

	TGAHEADER head;
	
	out = fopen(filename, "wb");

	if(out == NULL) {
		return 0;
	}

	MakeHeader(&head);

	fwrite(&head, sizeof(TGAHEADER), 1, out);
	
	PutRGB(&head, dac);

	return 1;
}

//----------------------------------------------------------------------------
//	TGA 파일에 한라인을 압축한다.
//----------------------------------------------------------------------------

int  tgaClass :: PutOneLine(BYTE *buf)
{
   int onelinebyte;

   switch(nBitsPerPixel) {
      case 1:	
			onelinebyte = (nWidth+7)/8; break;
      case 8:	
			onelinebyte = nWidth;	break;
      case 15:
      case 16:
			onelinebyte = nWidth*2;	break;
      case 24:	
			onelinebyte = nWidth*3;	break;
      case 32:	
			onelinebyte = nWidth*4;	break;
   }

   fwrite(buf, 1, onelinebyte, out);

   return 1;
}


/*

//---------------------------------------------------------------------------
//	TGA 파일에서 RGB 정보를 쓴다.
//	여기서는 256 칼라일때만 유용하므로 나머지는 필요없다.
//	파레트는 2,3,4 바이트 세가지가 있지만
//	여기서는 항상 3바이트 BGR 파레트를 쓴다.
//	2 byte - ARRRRRGG GGGBBBBB
//	3 byte - Blue Green Red
//	4 byte - Blue Green Red Attribute
//---------------------------------------------------------------------------

void TgaPutRGB(FILE *out, TGAHEADER head, BYTE *dac)
{
   int i;

   if(head.bitsperpixel != 8) {
      return;
   }

   if(head.cmlen == 2) {
      fputc(0, out);	fputc(0, out);		fputc(0, out);
      fputc(255, out);	fputc(255, out);	fputc(255, out);
      return;
   }

   for(i = 0; i < head.cmlen; i++) {	// because BGR color map
      fputc(dac[i*3+2], out);
      fputc(dac[i*3+1], out);
      fputc(dac[i*3+0], out);
   }
}

//----------------------------------------------------------------------------
//	TGA 파일에 한라인을 압축한다.
//----------------------------------------------------------------------------

int  TgaPutOneLine(FILE *out, TGAHEADER head, BYTE *buf)
{
   int onelinebyte;

   switch(head.bitsperpixel) {
      case 1:	onelinebyte = (head.xsize+7)/8; break;
      case 8:	onelinebyte = head.xsize;	break;
      case 15:
      case 16:
		onelinebyte = head.xsize*2;	break;
      case 24:	onelinebyte = head.xsize*3;	break;
      case 32:	onelinebyte = head.xsize*4;	break;
      default:  exiterr("Tga put one line error");	break;
   }

   fwrite(buf, 1, onelinebyte, out);

   return 1;
}
*/

