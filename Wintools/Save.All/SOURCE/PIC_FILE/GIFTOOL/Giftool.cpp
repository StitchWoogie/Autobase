#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>
#endif
#include <giftool.h>
#include <dataswap.h>
#include <ch_buf.h>

#define COLORMAPFLAG	0x80	// mask for bit signifying colormap presence 
#define INTERLACE	0x40		// mask for bit signifying interlaced image 

const char interlace_start[4] = { 0, 4, 2, 1};	// 비월주사 방식 시작위치
const char interlace_plus[4] =  { 8, 8, 4, 2};	// 비원주사 방식 건너뛰기.

gifClass :: gifClass() : pictureFileClass()
{
	lzwRead = new lzwReadClass;
	lzwWrite = new lzwWriteClass;
	nInterlace = 0;
}

gifClass :: ~gifClass()
{
	delete lzwRead;
	delete lzwWrite;
}

int gifClass :: ReadByte()
// Read next byte from GIF file 
{
	int c;

	c = getc(in);
    
	return c;
}

int gifClass :: ReadOK(char *buf, int len)
{
	if(fread(buf, 1, len, in) != (unsigned)len)	return 0;
	else														return 1;
}

int gifClass :: GetDataBlock (char *buf)
// Read a GIF data block, which has a leading count byte 
// A zero-length block marks the end of a data block sequence 
{
  int count;

  count = ReadByte();
  if (count > 0) {
    if (!ReadOK(buf, count))
		 return 0;
  }
  return count;
}

void gifClass :: ReadColorMap (int cmaplen)
// Read a GIF colormap 
{
	int i;

	memcpy(dac, DEFAULT_RGB, 768);

	for(i = 0; i < cmaplen; i++) {
		dac[i*3+0] = ReadByte();
		dac[i*3+1] = ReadByte();
		dac[i*3+2] = ReadByte();
	}
}

void gifClass :: DoExtension ()
// Process an extension block 
// Currently we ignore 'em all 
{
	int extlabel;
	int count;
	int i;

	// Read extension label byte
	extlabel = ReadByte();
	//TRACEMS1(sinfo->cinfo, 1, JTRC_GIF_EXTENSION, extlabel);
	// Skip the data block(s) associated with the extension 
	//SkipDataBlocks(sinfo);
	count = ReadByte();
	for(i = 0; i < count; i++) {
		ReadByte();
	}
}

int gifClass :: ReadOpen(HWND hwnd, char *filename)
{
	GIFHEADER head;
	GIFHEADER_LOCAL local;
	int colormaplen;
	int aspectRatio;
	int c;
	int input_code_size;

   if(dac == NULL)	return 0;

   in = fopen(filename, "rb");
   if(in == NULL)	{
		ReadOpenError(hwnd, filename);
		return 0;
   }

	if(sizeof(GIFHEADER) != fread(&head, 1, sizeof(GIFHEADER), in)) {
		ReadSizeError(hwnd, filename);
		fclose(in);
		return 0;
   }

	if(head.maker[0] != 'G' || head.maker[1] != 'I' || head.maker[2] != 'F') {
		MessageBox(NULL, "GIF 파일이 아닙니다.", "파일 형식 틀림", MB_OK);
		fclose(in);
		return 0;
   }

	if((head.version[0] != '8' || head.version[1] != '7' || head.version[2] != 'a') &&
		(head.version[0] != '8' || head.version[1] != '9' || head.version[2] != 'a')) {
		MessageBox(NULL, "GIF 버전이 틀립니다.", "파일 버전 틀림", MB_OK);
		fclose(in);
		return 0;
   }

	nWidth =  head.xres;
	nHeight = head.yres;

	if((head.colorBit & 0x07) == 0)
		colormaplen = 0;
	else
		colormaplen = 2 << (head.colorBit & 0x07);
	
	nBitsPerPixel = ((head.colorBit >> 4) & 0x07)+1;

	// 16칼라나 256칼라가 아닐 수 있으므로 새로 정의한다. (예:4칼라, 8칼라)
	if(nBitsPerPixel == 1) {
		nBitsPerPixel = COLOR_2;
	}
	else if(nBitsPerPixel >= 2 && nBitsPerPixel <= 4) {	 
		nBitsPerPixel = COLOR_16;
	}
	else {
		nBitsPerPixel = COLOR_256;
	}

   // we ignore the color resolution, sort flag, and background color index 
   aspectRatio = head.etc & 0xFF;
   
	//if (aspectRatio != 0 && aspectRatio != 49)
   // TRACEMS(cinfo, 1, JTRC_GIF_NONSQUARE);

	// Read global colormap if header indicates it is present 
	if (head.colorBit & COLORMAPFLAG)
		ReadColorMap(colormaplen);

	// Scan until we reach start of desired image.
   // We don't currently support skipping images, but could add it easily.
   
	for (;;) {
		c = ReadByte();

		if (c == ';')			// GIF terminator?? 
			return 0;
			//ERREXIT(cinfo, JERR_GIF_IMAGENOTFOUND);

		if (c == '!') {		// Extension 
			DoExtension();
			continue;
		}
    
		if (c != ',') {		// Not an image separator? 
			//WARNMS1(cinfo, JWRN_GIF_CHAR, c);
			continue;
		}

		if(sizeof(GIFHEADER_LOCAL) != fread(&local, 1, sizeof(GIFHEADER_LOCAL), in)) {
			return 0;
		}
		
		// we ignore top/left position info, also sort flag 
		nWidth =  local.width;
		nHeight = local.height;

		if(local.colorBit & INTERLACE)	nInterlace = 1;

		// Read local colormap if header indicates it is present
		// Note: if we wanted to support skipping images, 
		// we'd need to skip rather than read colormap for ignored images 

		if (local.colorBit & COLORMAPFLAG) {
			colormaplen = 2 << (local.colorBit & 0x07);
			ReadColorMap(colormaplen);
		}

		input_code_size = ReadByte(); // get min-code-size byte 

		if (input_code_size < 2 || input_code_size >= MAX_LZW_BITS)
			return 0;
		// ERREXIT1(cinfo, JERR_GIF_CODESIZE, source->input_code_size);

		// Reached desired image, so break out of loop 
		// If we wanted to skip this image, 
		// we'd call SkipDataBlocks and then continue the loop 
		break;
	}

	lzwRead->SetInputCodeSize(input_code_size);
	lzwRead->Init();
	lzwRead->SetFile(in);
	
   return 1;
}

//---------------------------------------------------------------------------
//	PCX 화일에서 한라인의 그림을 읽어온다..
//	maxx 는 버퍼에서 제한하는 최대값이다.
//---------------------------------------------------------------------------

int gifClass :: GetOneLine(BYTE *buf, int maxx)
{
	int c;
	int i;

	if(nBitsPerPixel == COLOR_2) {
		memset(buf, 0, (maxx+7)/8);
		for(i = 0; i < nWidth; i++) {
			c = lzwRead->ReadByte();
			if(i < maxx) {
				if(c)	buf[i/8] |= BIT_MASK[i%8];
			}
		}
	}
	else if(nBitsPerPixel == COLOR_16) {
		for(i = 0; i < nWidth; i++) {
			c = lzwRead->ReadByte();
			if(i < maxx) {
				PutColorAllBuf16(buf, i, c);
			}
		}
	}
	else if(nBitsPerPixel == COLOR_256) {	
		for(i = 0; i < nWidth; i++) {
			c = lzwRead->ReadByte();
			if(i < maxx) {
				buf[i] = c;
			}
		}
	}
	else {

	}

	return 1;
}

int gifClass :: WriteHeader()
{
	int colormapsize;
	GIFHEADER head;
	int i;

	if(nBitsPerPixel == COLOR_2) {
		colormapsize = 0;
	}
	else if(nBitsPerPixel == COLOR_16) {
		colormapsize = 16;
	}
	else if(nBitsPerPixel == COLOR_256) {
		colormapsize = 256;
	}
	else {
		colormapsize = 0;
	}
	
	head.maker[0] = 'G';
	head.maker[1] = 'I';
	head.maker[2] = 'F';
	head.version[0] = '8';
	head.version[1] = '7';
	head.version[2] = 'a';

	head.xres = nWidth;
	head.yres = nHeight;

	if(colormapsize == 0) {
		head.colorBit = 0;							// Yes, there is a global color table 
	}
	else
		head.colorBit = 0x80;							// Yes, there is a global color table 

	head.colorBit |= (nBitsPerPixel-1) << 4;	// color resolution 
   head.colorBit |= (nBitsPerPixel-1);			// size of global color table 
	head.backColor = 0; // Background color index 
	head.etc = 0; // Reserved (aspect ratio in GIF89) 	

	fwrite(&head, 1, sizeof(head), out);

	// write palette
	for(i = 0; i < colormapsize; i++) {
		// Normal case: RGB color map 
		fputc(dac[i*3+0], out);
		fputc(dac[i*3+1], out);
		fputc(dac[i*3+2], out);
	} 

	// Write image separator and Image Descriptor 
	putc(',', out); // separator 

	GIFHEADER_LOCAL local;

	local.start_x = 0;		// left/top offset
	local.start_y = 0;
	local.width = nWidth;	// image size
	local.height = nHeight;
	local.colorBit = 0;	// flag byte: not interlaced, no local color map 

	fwrite(&local, 1, sizeof(local), out);

	int InitCodeSize;

	if (nBitsPerPixel <= 1)
		InitCodeSize = 2;
	else
		InitCodeSize = nBitsPerPixel;
	
	putc(InitCodeSize, out); // Write Initial Code Size byte 

	lzwWrite->SetFile(out);
	lzwWrite->Init(InitCodeSize+1);

  	return 1;
}

int gifClass :: WriteOpen(HWND hwnd, char *filename)
{
	if(nBitsPerPixel == COLOR_32BIT) {
		MessageBox(NULL, "지원되지 않는 색상", "Gif File", MB_OK);
		return 0;
	}

	if(dac == NULL)	return 0;

	out = fopen(filename, "wb");
	if(out == NULL) {
		WriteOpenError(hwnd, filename);
		return 0;
	}

	WriteHeader();			// header write

	return 1;
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

int  gifClass :: PutOneLine(BYTE *buf)
{
	register int col;

	if(nBitsPerPixel == COLOR_2) {
		for (col = 0; col < nWidth; col++) {
			lzwWrite->compress_byte((buf[col/8] & BIT_MASK[col%8]) ? 1 : 0);
		}
	}
	else if(nBitsPerPixel == COLOR_16) {
		for (col = 0; col < nWidth; col++) {
			lzwWrite->compress_byte(GetColorAllBuf16(buf, col));
		}
	}
	else if(nBitsPerPixel == COLOR_256) {
		for (col = 0; col < nWidth; col++) {
			lzwWrite->compress_byte(buf[col]);
		}
	}
	else {

	}

	return 1;
}

void gifClass :: WriteClosePrepare()
{
	// Flush LZW mechanism 
	lzwWrite->compress_term();
	// Write a zero-length data block to end the series 
	putc(0, out);
	// Write the GIF terminator mark 
	putc(';', out);
	// Make sure we wrote the output file OK 
	fflush(out);
	if (ferror(out)) {
		//ERREXIT(cinfo, JERR_FILE_WRITE);
	}
}
