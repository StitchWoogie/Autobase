#include "stdafx.h"
#include <compiler.hpp>

#include <pcxtool.h>
#include <dataswap.h>
#if	defined (__BORLANDC__)
#include <mem.h>
#endif

pcxClass :: pcxClass() : pictureFileClass()
{

}

pcxClass :: ~pcxClass()
{

}

int pcxClass :: ReadOpen(HWND hwnd, const TCHAR *filename)
{
   if(dac == NULL)	return 0;

   in = _tfopen(filename, _T("rb"));
   if(in == NULL)	{
		ReadOpenError(hwnd, filename);
		return 0;
   }
   if(sizeof(PCXHEADER) != fread(&head, 1, sizeof(PCXHEADER), in)) {
		ReadSizeError(hwnd, filename);
		fclose(in);
		return 0;
   }

   if(head.maker != 10) {
		MessageBox(NULL, _T("PCX 파일이 아닙니다."), _T("파일 형식 틀림"), MB_OK);
		fclose(in);
		return 0;
   }

   nWidth = head.x2-head.x1+1;
   nHeight = head.y2-head.y1+1;

   //---------------------------------------------------------------------------
   //	주어진 그림에서 사용된 색상수를 얻는다
   //---------------------------------------------------------------------------
   if(head.nplanes == 3 && head.bitperpixel == 8)
		nBitsPerPixel = COLOR_1600;
   else if( (head.nplanes == 1) && head.bitperpixel == 8)
		nBitsPerPixel = COLOR_256;
   else if( (head.nplanes == 1 || head.nplanes == 2) && head.bitperpixel == 1)
		nBitsPerPixel = COLOR_2;
   else if( head.nplanes == 4 && head.bitperpixel == 1)
		nBitsPerPixel = COLOR_16;
   else {
		TCHAR message[256];
		_stprintf(message, _T("지원되지 않는 색상을 가진 그림입니다.\nhead.nplanes=%d\nhead.bitperpixel=%d"),
							   head.nplanes, head.bitperpixel);
		MessageBox(NULL, message, _T("PCX 색상쩠 이상"), MB_OK);
		return 0;
   }

   //----------------------------------------------------------------------------
   //	RGB 정보를 읽어온다.
   // RGB 정보는 256일때는 화일의 맨뒤에 768바이트가 붙는다.
   //----------------------------------------------------------------------------

   register int ch, i;

   switch( nBitsPerPixel ) {
      case COLOR_256:	fseek(in, -769, 2);
			ch = fgetc(in);
			if(ch == 12) {
			   fread(dac, 1, 768, in);
			}
			else {
			   fseek(in, 16, 0);
			   fread(dac, 1, 48, in);
			   dac[765] = dac[766] = dac[767] = 255;
			}
			break;
      case COLOR_16:
			memcpy(dac, head.rgb, 48);
			break;
      case COLOR_2:
			for(i = 0; i < 3; i++) dac[i] = 0;
			for(i = 3; i < 6; i++) dac[i] = 255;
			break;
   }
   fseek(in, sizeof(PCXHEADER),  SEEK_SET);

   return 1;
}

//---------------------------------------------------------------------------
//	PCX 화일에서 한라인의 그림을 읽어온다..
//	maxx 는 버퍼에서 제한하는 최대값이다.
//	bufcolor 는 채울버퍼의 칼윕쩠를 큉함한다.
//---------------------------------------------------------------------------

int pcxClass :: GetOneLine(BYTE far *buf, int maxx)
{
   int retn = 0;

   switch ( nBitsPerPixel ) {
      case COLOR_2:
		retn = GetOneLine2To2(buf, maxx);	break;
      case COLOR_16:
		retn = GetOneLine16To16(buf, maxx);	break;
      case COLOR_256:
		retn = GetOneLine256To256(buf, maxx);	break;
      case COLOR_1600:
		retn = GetOneLine1600To1600(buf, maxx);	break;
   }
   return ( retn );
}

//----------------------------------------------------------------------------
//	그림이 1600인 화일을 1600좼퍼찌 읽어온다.
//----------------------------------------------------------------------------

int pcxClass :: GetOneLine1600To1600( BYTE far *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
   BYTE data;
   int plane = 2;

   xb = head.byteperline;

   while( (ch = fgetc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0 ) {  	// 1100 0000
	 rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
	 data = fgetc(in);
      }
      else {
	 rc = 1;
	 data = ch;
      }

      while ( rc-- > 0) {
	 if(xx < maxx)        		// 좼퍼의 범챦를 벗어나지 않았을때
	 buf[xx*3+plane] = data;
	 xx++;
	 if(xx >= xb) {                    // 한줄을 모두 읽었을때
	    if(plane <= 0)	return 1;
	    xx = 0;
	    plane--;
	 }
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 256인 화일을 256좼퍼찌 읽어온다.
//----------------------------------------------------------------------------
int pcxClass :: GetOneLine256To256( BYTE far *buf, int maxx)
{
   int xx=0, xb, rc, ch;
   BYTE data;

   xb = head.byteperline;

   while( (ch = fgetc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0 ) {  	// 1100 0000
	 rc = ch & 0x3F;                // 같은 온이타가 rc 만큼 반복한다.
	 data = fgetc(in);
      }
      else {
	 rc = 1;
	 data = ch;
      }

      while ( rc-- > 0) {
	 if(xx < maxx)        		// 좼퍼의 범챦를 벗어나지 않았을때
	 buf[xx] = data;

	 xx++;

	 if(xx >= xb)                     // 한줄을 모두 읽었을때
		 return (1);
      }

      if(xx >= xb)                     // 한줄을 모두 읽었을때
      return (1);
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 16인 화일을 16버퍼에 읽어온다.
//----------------------------------------------------------------------------

int pcxClass :: GetOneLine16To16( BYTE far *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
   unsigned char data;
   register int plane = 4;

   int limitbyte = (maxx+7)/8;

   xb = head.byteperline;

   memset(buf, 0, (maxx+1)/2);

   while( (ch = getc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0 ) {  	// 1100 0000
			rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
			data = getc(in);
      }
      else {
			rc = 1;
			data = ch;
      }

      while ( rc-- > 0) {
			if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
				for(int i = 0; i < 8; i+=2) {	// packed memory buffer
					buf[xx*4+i/2] |= data & BIT_MASK[i]   ? BIT_MASK[(plane-1)]   : 0;
					buf[xx*4+i/2] |= data & BIT_MASK[i+1] ? BIT_MASK[(plane-1)+4] : 0;
				}
			   // buf[limitbyte*(plane-1)+xx] = data;   plane memory buffer
			}
			xx++;

			if(xx >= xb) {
				if(plane <= 1)	return (1);
				xx = 0;
				plane--;
			}
		}
   }
   return (0);
}

int pcxClass :: GetOneLine2To2( BYTE far *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
   unsigned char data;

   int limitbyte = (maxx+7) / 8;

   xb = head.byteperline;

   while( (ch = getc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0) {
			rc = ch & 0x3F;
			data = getc(in);
      }
      else {
			rc = 1;
			data = ch;
      }

      while ( rc-- > 0) {
			if(xx < limitbyte) {
				buf[xx] = data;
			}
			xx++;
			if(xx >= xb) {
				return (1);
			}
      }
      if(xx >= xb) {
			return (1);
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	 PCX 헤더를 채운다.
//----------------------------------------------------------------------------

int pcxClass :: FillPcxHeader(PCXHEADER *head)
{
	memset(head, 0, sizeof(PCXHEADER));
	
	head->maker = 10;			// allways 10
   head->version = 5;
   head->code = 1;  			// 압축
   head->x1 = 0;				// 그림의 시작점 x

   head->y1 =	0;          // 그림의 시작점 y
   head->x2 =	nWidth-1;	//	그림의 끝점   x
   head->y2 =	nHeight-1;  //	그림의 끝점   y
   head->hres = 640;			//	그림그릴때의 수평해상도
   head->vres = 480;			//	그림그릴때의 수직해상도
   head->vmode = 0;        //

	switch(nBitsPerPixel) {
		case COLOR_1600:
			head->nplanes = 3;
			head->byteperline = nWidth;
			head->bitperpixel = 8;
			break;
		case COLOR_256:
			head->nplanes = 1;				// 1 - mono, 256
			head->byteperline = nWidth;	//	bytes per line
			head->bitperpixel = 8;			//	1 - mono, 16color, 8 - 256 color
			break;
		case COLOR_16:
			head->nplanes = 4;            // 1 - mono, 256
			head->byteperline = (nWidth+7)/8; 	//	bytes per line
			head->bitperpixel = 1;        //	1 - mono, 16color, 8 - 256 color
			memcpy(head->rgb, dac, 48);
			break;
		case COLOR_2:
			head->nplanes = 1;            // 1 - mono, 256
			head->byteperline = (nWidth+7)/8; 	//	bytes per line
			head->bitperpixel = 1;        //	1 - mono, 16color, 8 - 256 color
			break;
		default:
			MessageBox(NULL, _T("지원되지 않는 색상"), _T("pcxClass :: FillPcxHeader()"), MB_OK);
			return 0;
	}

	return 1;
}

//---------------------------------------------------------------------------
//	PCX 화일에서 RGB 정보를 쓴다.
//	여기서는 256 칼라일때만 유용하므로 나머지는 필요없다.
// 파일의 맨 뒷부분에 (12)+RGB 가 위치한다.
//---------------------------------------------------------------------------

void pcxClass :: PutRGB()
{
   if(nBitsPerPixel != COLOR_256)	return;
	
   fputc(12, out);
   fwrite(dac, 1, 768, out);
}

int pcxClass :: WriteOpen(HWND hwnd, TCHAR *filename)
{
   if(dac == NULL)	return 0;

	if(!FillPcxHeader(&head))	return 0;

   out = _tfopen(filename, _T("wb"));
   if(out == NULL)	{
		WriteOpenError(hwnd, filename);
		return 0;
   }

   fwrite(&head, 1, sizeof(PCXHEADER), out);

   return 1;
}

//-----------------------------------------------------------
//	파일을 닫기전에 256칼라의 색상판을 파일의 맨뒤에 저장한다. 
//-----------------------------------------------------------

void pcxClass :: WriteClosePrepare()
{
	PutRGB();
}

//----------------------------------------------------------------------------
//	PCX 화일에 한 라인을 압축한다.
//
//	압축 방법은 다음과 같다.
//
//	같은 바이트가 반복되면 반복수, 데이타
//	다른 바이트 반복시는 0xC0 보다 작은 데이타는 그냥써주고,
//	0xC0 보다 크거나 같은 데이타는 0xC1, 데이타를 써준다.
//----------------------------------------------------------------------------

int  pcxClass :: PutOneLine(BYTE *buf)
{
   if(nBitsPerPixel == COLOR_16) {
		return PutOneLine16(buf);
	}
	else {
		return PutOneLineElse(buf);
	}
}

int  pcxClass :: PutOneLineElse(BYTE *buf)
{
	BYTE *p;
   register int i;
   register int count = 0;
   BYTE save;
   register int plane;
   int plus = 1; 	
	// 256칼라 이전까지는 1씩 버퍼위치를 증가시키면
	// 되지만 그 이상의 색상에서는 증가율을 조정한다.

   if(head.nplanes == 3 && head.bitperpixel == 8) {	// 1600 color
      plus = 3;         // 버퍼를 저장할때 R,G,B plane 순으로 저장한다.
   }

   for(plane = head.nplanes-1; plane >= 0; plane--) {
      
		if(head.nplanes == 3 && head.bitperpixel == 8)  // 1600M color
			p = &buf[plane];          // 0, 1, 2 순서
      else
			p = &buf[plane*head.byteperline];	// B,G,R,I plane save

      count = 1;
      save = *p;
      p+=plus;
      for(i = 1; i < head.byteperline; i++, p+=plus) {
			if(*p == save) {
				if(count >= 0x3F) {
					fputc(0xFF, out);
					fputc(save, out);
					count = 0;
				}
				count ++;
			}
			else {      		// 앞바이트와 뒤바이트가 같지 않을때
				if(count > 1) {     // 앞에 헤아린 수가 두개 이상일때
					fputc(0xC0 | (BYTE) count, out);
					fputc(save, out);
				}
				else {              // 앞에 헤아린 수가 하나일때
					if(save >= 0xC0) { 	// 데이타가 0xC0 이상일때
						fputc(0xC1, out);
						fputc(save, out);
					}
					else {                   // 데이타가 0xC0 보다 작을때
						fputc(save, out);
					}
				}
				count = 1;
				save = *p;
			}      // 앞바이트와 뒤바이트가 같지 않을때
		}         // byteperline for loop

		//**********************************
		// 미처 다쓰지 못한 데이타를 쓴다.
		// 여기서 이런이유는 모든데이타의 압축은
		// 한줄단위로 (혹은 한 플랜) 압축되기 때문이다.
		//**********************************

		if(count > 1)
			fputc(0xC0 | (BYTE) count, out);
		else if(save >= 0xC0)
			fputc(0xC1, out);
		else;

		fputc(save, out);
	}    	// plane for loop
	return 1;
}


int  pcxClass :: PutOneLine16(BYTE *buf)
{
	BYTE *p;
   register int i;
   register int count = 0;
   BYTE save;
   register int plane;
   int plus = 1; 	
	int oneplanebyte = (nWidth+7)/8;
	StackBYTE stack(oneplanebyte*4);
	int color;

	if(stack.data == NULL)	return 0;

	memset(stack.data, 0, oneplanebyte*4);
	for(i = 0; i < nWidth; i++) {
		if((i%2) == 0)		color = (buf[i/2] >> 4)	& 0x0F;			
		else					color = (buf[i/2] >> 0)	& 0x0F;			
		
		for(plane = 3; plane >= 0; plane--) {
			stack.data[oneplanebyte*plane+i/8] |= BIT_MASK[plane+4] & color ? BIT_MASK[i%8] : 0;
		}
	}

	for(plane = head.nplanes-1; plane >= 0; plane--) {
      
		if(head.nplanes == 3 && head.bitperpixel == 8)  // 1600M color
			p = &stack.data[plane];          // 0, 1, 2 순서
      else
			p = &stack.data[plane*head.byteperline];	// B,G,R,I plane save

      count = 1;
      save = *p;
      p+=plus;
      for(i = 1; i < head.byteperline; i++, p+=plus) {
			if(*p == save) {
				if(count >= 0x3F) {
					fputc(0xFF, out);
					fputc(save, out);
					count = 0;
				}
				count ++;
			}
			else {      		// 앞바이트와 뒤바이트가 같지 않을때
				if(count > 1) {     // 앞에 헤아린 수가 두개 이상일때
					fputc(0xC0 | (BYTE) count, out);
					fputc(save, out);
				}
				else {              // 앞에 헤아린 수가 하나일때
					if(save >= 0xC0) { 	// 데이타가 0xC0 이상일때
						fputc(0xC1, out);
						fputc(save, out);
					}
					else {                   // 데이타가 0xC0 보다 작을때
						fputc(save, out);
					}
				}
				count = 1;
				save = *p;
			}      // 앞바이트와 뒤바이트가 같지 않을때
		}         // byteperline for loop

		//**********************************
		// 미처 다쓰지 못한 데이타를 쓴다.
		// 여기서 이런이유는 모든데이타의 압축은
		// 한줄단위로 (혹은 한 플랜) 압축되기 때문이다.
		//**********************************

		if(count > 1)
			fputc(0xC0 | (BYTE) count, out);
		else if(save >= 0xC0)
			fputc(0xC1, out);
		else;

		fputc(save, out);
	}    	// plane for loop
	return 1;
}


/*
// 	pcx 의 각종 그림을 256칼라 버퍼에 옮긴다.
int PcxGetOneLine1600To1600( FILE *in, PCXHEADER *head, BYTE *buf, int maxx);
int PcxGetOneLine256To256( FILE *in, PCXHEADER *head, BYTE *buf, int maxx);
int PcxGetOneLine16To16( FILE *in, PCXHEADER *head, BYTE *buf, int maxx);
int PcxGetOneLine2To2( FILE *in, PCXHEADER *head, BYTE *buf, int maxx);

int PcxGetSize(char *filename, int& sizex, int& sizey)
{
   FILE *in;
   PCXHEADER head;
   int size;

   in = fopen(filename, "rb");
   if(in == NULL)	return 0;
   size = fread(&head, 1, sizeof(head), in);
   fclose(in);

   if(size != sizeof(head))	return 0;
   sizex = head.x2-head.x1+1;
   sizey = head.y2-head.y1+1;
   return 1;
}

//---------------------------------------------------------------------------
//	주어진 헤드에서 그림에서 사용된 최대칼라수를 얻는다
//---------------------------------------------------------------------------

int PcxGetMaxColor( PCXHEADER *head )
{
   if(head->nplanes == 3 && head->bitperpixel == 8)
	return ( COLOR_1600);
   else if( (head->nplanes == 1) && head->bitperpixel == 8)
	return ( COLOR_256 );
   else if( (head->nplanes == 1 || head->nplanes == 2) && head->bitperpixel == 1)
	return ( COLOR_2 );
   else if( head->nplanes == 4 && head->bitperpixel == 1)
	return ( COLOR_16 );
   else
	return ( 0 );
}

//----------------------------------------------------------------------------
//	RGB 정보를 읽어온다.
//      RGB 정보는 256일때는 화일의 맨뒤에 768바이트가 붙는다.
//----------------------------------------------------------------------------

void PcxGetRGB( FILE *in, PCXHEADER *head, BYTE *dac)
{
   register int ch, i;

   switch( PcxGetMaxColor(head) ) {
      case COLOR_256:
		fseek(in, -769, 2);
		ch = fgetc(in);
		if(ch == 12) {
		   fread(dac, 768, 1, in);
		}
		else {
		   fseek(in, 16, 0);
		   fread(dac, 48, 1, in);
		   dac[765] = dac[766] = dac[767] = 255;
		}
		break;
      case COLOR_16:
		memcpy(dac, head->rgb, 48);
		break;
      case COLOR_2:
		for(i = 0; i < 3; i++) dac[i] = 0;
		for(i = 3; i < 6; i++) dac[i] = 255;
		break;
   }

   fseek(in, sizeof(PCXHEADER),  SEEK_SET);
}

//---------------------------------------------------------------------------
//	PCX 화일에서 한라인의 그림을 읽어온다..
//	maxx 는 버퍼에서 제한하는 최대값이다.
//	bufcolor 는 채울버퍼의 칼라수를 포함한다.
//---------------------------------------------------------------------------

int PcxGetOneLine(FILE *in, PCXHEADER *head, BYTE *buf, int maxx)
{
   int retn = 0;

   switch ( PcxGetMaxColor(head) ) {
      case COLOR_2:
		retn = PcxGetOneLine2To2(in, head, buf, maxx);
		break;
      case COLOR_16:
		retn = PcxGetOneLine16To16(in, head, buf, maxx);
		break;
		case COLOR_256:
		retn = PcxGetOneLine256To256(in, head, buf, maxx);
		break;
      case COLOR_1600:
		retn = PcxGetOneLine1600To1600(in, head, buf, maxx);
		break;
   }

   return ( retn );
}

//----------------------------------------------------------------------------
//	그림이 1600인 화일을 1600버퍼에 읽어온다.
//----------------------------------------------------------------------------

int PcxGetOneLine1600To1600( FILE *in, PCXHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
   BYTE data;
   int plane = 2;

   xb = head->byteperline;

   while( (ch = getc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0 ) {  	// 1100 0000
	 rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);
      }
      else {
	 rc = 1;
	 data = ch;
      }

      while ( rc-- > 0) {
	 if(xx < maxx)        		// 버퍼의 범위를 벗어나지 않았을때
	    buf[xx*3+plane] = data;
	 xx++;
	 if(xx >= xb) {                    // 한줄을 모두 읽었을때
	    if(plane <= 0)	return 1;
	    xx = 0;
	    plane--;
	 }
      }
   }
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 256인 화일을 256버퍼에 읽어온다.
//----------------------------------------------------------------------------
int PcxGetOneLine256To256( FILE *in, PCXHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
   BYTE data;

   xb = head->byteperline;

   while( (ch = getc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0 ) {  	// 1100 0000
	 rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);
      }
      else {
	 rc = 1;
	 data = ch;
      }

      while ( rc-- > 0) {
	 if(xx < maxx)        		// 버퍼의 범위를 벗어나지 않았을때
	    buf[xx] = data;

	 xx++;

	 if(xx >= xb)                     // 한줄을 모두 읽었을때
	    return (1);
      }

      if(xx >= xb)                     // 한줄을 모두 읽었을때
	 return (1);
	}
   return (0);
}

//----------------------------------------------------------------------------
//	그림이 16인 화일을 16버퍼에 읽어온다.
//----------------------------------------------------------------------------
int PcxGetOneLine16To16( FILE *in, PCXHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
   unsigned char data;
   register int plane = 4;

   int limitbyte = (maxx+7)/8;

   xb = head->byteperline;

   memset(buf, 0, limitbyte*4);

   while( (ch = getc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0 ) {  	// 1100 0000
	 rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
	 data = getc(in);
      }
      else {
	 rc = 1;
	 data = ch;
      }

      while ( rc-- > 0) {

	 if(xx < limitbyte) {       		// 버퍼의 범위를 벗어나지 않았을때
		 buf[limitbyte*(plane-1) +xx] = data;
	 }
	 xx++;

	 if(xx >= xb) {
	    if(plane <= 1)	return (1);
	    xx = 0;
	    plane--;
	 }
      }
   }
   return (0);
}

int PcxGetOneLine2To2( FILE *in, PCXHEADER *head, BYTE *buf, int maxx)
{
   register int xx=0, xb, rc, ch;
	unsigned char data;

   int limitbyte = (maxx+7) / 8;

   xb = head->byteperline;

   while( (ch = getc(in)) != EOF) {
      if( (ch & 0xC0) == 0xC0) {
	 rc = ch & 0x3F;
	 data = getc(in);
      }
      else {
	 rc = 1;
	 data = ch;
      }

      while ( rc-- > 0) {
	 if(xx < limitbyte) {
	    buf[xx] = data;
	 }
	 xx++;
	 if(xx >= xb) {
	    return (1);
	 }
      }
      if(xx >= xb) {
	 return (1);
      }
   }
   return (0);
}


//---------------------------------------------------------------------------
//	PCX 화일에서 RGB 정보를 쓴다.
//	여기서는 256 칼라일때만 유용하므로 나머지는 필요없다.
//---------------------------------------------------------------------------

void PcxPutRGB(FILE *out, PCXHEADER *head, BYTE *dac)
{
   if(head->bitperpixel != 8 || head->nplanes != 1)	return;

   fputc(12, out);
   fwrite(dac, 1, 768, out);
}

//----------------------------------------------------------------------------
//	PCX 화일에 한라인을 압축한다.
//
//	압축 방법은 다음과 같다.
//
//	같은 바이트가 반복되면 반복수, 데이타
//	다른 바이트 반복시는 0xC0 보다 작은 데이타는 그냥써주고,
//	0xC0 보다 크거나 같은 데이타는 0xC1, 데이타를 써준다.
//----------------------------------------------------------------------------

int  PcxPutOneLine(FILE *out, PCXHEADER *head, BYTE *buf)
{
   BYTE *p;
   register int i;
   register int count = 0;
   BYTE save;
   register int plane;
   int plus = 1; 	// 256칼라이전까지는 1씩 버퍼위치를 증가시키면
			// 되지만 그이상의 색상에서는 증가율을 조정한다.

   if(head->nplanes == 3 && head->bitperpixel == 8) {	// 1600 color
      plus = 3;         // 버퍼를 저장할때 R,G,B plane 순으로 저장한다.
   }

   for(plane = head->nplanes-1; plane >= 0; plane--) {
      if(head->nplanes == 3 && head->bitperpixel == 8)  // 1600M color
			p = &buf[plane];          // 0, 1, 2 순서
      else
			p = &buf[plane*head->byteperline];	// B,G,R,I plane save
      count = 1;
      save = *p;
      p+=plus;
      for(i = 1; i < head->byteperline; i++, p+=plus) {
			if(*p == save) {
				if(count >= 0x3F) {
					fputc(0xFF, out);
					fputc(save, out);
					count = 0;
				}
				count ++;
			}
			else {      		// 앞바이트와 뒤바이트가 같지 않을때
			if(count > 1) {     // 앞에 헤아린 수가 두개 이상일때
		fputc(0xC0 | (BYTE) count, out);
		fputc(save, out);
		 }
		 else {              // 앞에 헤아린 수가 하나일때
		if(save >= 0xC0) { 	// 데이타가 0xC0 이상일때
		  fputc(0xC1, out);
		  fputc(save, out);
			 }
			 else {                   // 데이타가 0xC0 보다 작을때
		  fputc(save, out);
			 }
		 }
		 count = 1;
		 save = *p;
	 }      // 앞바이트와 뒤바이트가 같지 않을때
		}         // byteperline for loop

		//**********************************
		// 미처 다쓰지 못한 데이타를 쓴다.
		// 여기서 이런이유는 모든데이타의 압축은
		// 한줄단위로 (혹은 한 플랜) 압축되기 때문이다.
		//**********************************

		if(count > 1)
	 fputc(0xC0 | (BYTE) count, out);
		else if(save >= 0xC0)
	 fputc(0xC1, out);
		else;

		fputc(save, out);
	}    	// plane for loop
	return 1;
}
*/




