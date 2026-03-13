#include "stdafx.h"
#include <tools.h>
#include <glib.h>
#include <picture.h>
#include <bmptool.h>
#include <gcursor.h>
#include <dataswap.h>
#include <ch_buf.h>
#include <compiler.hpp>

int PictureBitmapIsPaintableZone(PICTURE_BITMAP_STRUCT *bmp, int x, int y);

int PictureBitmapLoadBmp(HWND hwnd, PICTURE_BITMAP_STRUCT *pic, char *filename)
{
	bmpClass bmp;
	int i;
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);

	if(dac.data == NULL)	return 0;

	if(!bmp.ReadOpen(hwnd, filename)) {
		return 0;
	}
	bmp.GetDac(dac.data);
	bmp.GetSize(width, height);

	widthbyte = BmpWidthToByte(width, bmp.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로 그림을 읽어올 수 없습니다.", "메모리 부족", MB_OK);
		return 0;
	}

	if(!PictureBitmapLoadNew(hwnd, pic, width, height, bmp.GetColor(), dac.data)) {
		delete buf;
		return 0;
	}

	PictureBitmapLock(pic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!bmp.GetOneLine(buf, width))	break;
		PictureBitmapPutOneLine(pic, height-1-i, buf);
	}
	WaitCursorEnd();
	PictureBitmapUnlock(pic);

	delete buf;

	bmp.ReadClose();

	return 1;
}

//------------------------------------------------------------------------------
//	주어진 크기와 색상을 가지고 비트맵 정보의 핸들을 만든다.
//------------------------------------------------------------------------------

HGLOBAL MakeBitmapInfo(int width, int height, int color, BYTE *dac)
{
	HGLOBAL hglobal;
	PBITMAPINFO pbmi;

	int palcount;

	switch(color) {
		case 1:	palcount = 2;		break;
		case 4:	palcount = 16;    break;
		case 8:	palcount = 256;  	break;
		default:	palcount = 0;    	break;
	}

	hglobal = GlobalAlloc(GMEM_ZEROINIT | GMEM_MOVEABLE,
							sizeof(BITMAPINFOHEADER) + (sizeof(RGBQUAD) * palcount));

	if(hglobal == NULL)	return NULL;

	pbmi = (PBITMAPINFO) GlobalLock(hglobal);

	pbmi->bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	pbmi->bmiHeader.biWidth =  width;
	pbmi->bmiHeader.biHeight = height;
	pbmi->bmiHeader.biPlanes = 1;

	pbmi->bmiHeader.biBitCount = color;
	pbmi->bmiHeader.biCompression = BI_RGB;

	for(int i = 0; i < palcount; i++) {
		pbmi->bmiColors[i].rgbRed = dac[i*3+0];
		pbmi->bmiColors[i].rgbGreen = dac[i*3+1];
		pbmi->bmiColors[i].rgbBlue = dac[i*3+2];
		pbmi->bmiColors[i].rgbReserved = 0;
	}
	GlobalUnlock(hglobal);

	return hglobal;
}

//------------------------------------------------------------------------------
//	스트럭쳐에 새로운 DAC로 바꾼다.
//------------------------------------------------------------------------------

void PictureBitmapSetDac(PICTURE_BITMAP_STRUCT *bmp, BYTE *dac)
{
	BITMAPINFO *pbmi;
	int palcount;

	switch(bmp->nBitsPerPixel) {
		case 1:	palcount = 2;		break;
		case 4:	palcount = 16;    break;
		case 8:	palcount = 256;  	break;
		default:	palcount = 0;    	return;
	}

	pbmi = (PBITMAPINFO) GlobalLock(bmp->hInfo);

	for(int i = 0; i < palcount; i++) {
		pbmi->bmiColors[i].rgbRed =   bmp->dac[i*3+0] = dac[i*3+0];
		pbmi->bmiColors[i].rgbGreen = bmp->dac[i*3+1] = dac[i*3+1];
		pbmi->bmiColors[i].rgbBlue =  bmp->dac[i*3+2] = dac[i*3+2];
		pbmi->bmiColors[i].rgbReserved = 0;
	}

	GlobalUnlock(bmp->hInfo);

	if(bmp->hPalette != NULL) {
		DeleteObject(bmp->hPalette);
		bmp->hPalette = NULL;
	}
	bmp->hPalette = PictureBitmapMakePalette(bmp);
}

//------------------------------------------------------------------------------
//	주어진 정보로 새 그림을 만든다.
//------------------------------------------------------------------------------

int PictureBitmapLoadNew(HWND hwnd, PICTURE_BITMAP_STRUCT *bmp, int width, int height, int bitsperpixel, BYTE *dacInfo)
{
	StackBYTE dac(768);

	if(dac.data == NULL)	return NULL;

	bmp->nWidth = width;
	bmp->nHeight = height;
	bmp->nDrawZoneX1 = 0;
	bmp->nDrawZoneY1 = 0;
	bmp->nDrawZoneX2 = width-1;
	bmp->nDrawZoneY2 = height-1;
	bmp->nBitsPerPixel = bitsperpixel;
	bmp->lBytesPerLine = BmpWidthToByte(width, bitsperpixel);

	bmp->hPalette = NULL;
	bmp->hInfo = NULL;

	memcpy(dac.data, dacInfo, 768);

	if(bitsperpixel == COLOR_2) {
		memcpy(dac.data, DEFAULT_RGB, 768);
		dac.data[0] = 0;
		dac.data[1] = 0;
		dac.data[2] = 0;
		dac.data[3] = 255;
		dac.data[4] = 255;
		dac.data[5] = 255;
	}
	else if(bitsperpixel == COLOR_1600) {
		memcpy(dac.data, DEFAULT_RGB, 768);
	}
	else if(bitsperpixel == COLOR_32BIT) {
		memcpy(dac.data, DEFAULT_RGB, 768);
	}
	else;
	
	bmp->hData = GlobalAlloc(GMEM_MOVEABLE, (long)bmp->nHeight*bmp->lBytesPerLine);

	if(bmp->hData == NULL) {
		MessageBox(hwnd, "확장 메모리 부족으로\n그림을 만들 수 없습니다.", "메모리 부족", MB_OK);
		return 0;
	}

	bmp->hInfo = MakeBitmapInfo(width, height, bitsperpixel, dac.data);
	if(bmp->hInfo == NULL) {
		MessageBox(hwnd, "글로벌 메모리 부족으로\nInformation을 만들 수 없습니다.", "메모리 부족", MB_OK);
		GlobalFree(bmp->hData);
		bmp->hData = NULL;
		return 0;
	}

	memcpy(bmp->dac, dac.data, 768);

	bmp->hPalette = PictureBitmapMakePalette(bmp);
	
	return 1;
}

//------------------------------------------------------------------------------
//	주어진 비트맵 파레트를 가지고 파레트 핸들을 만든다.
//------------------------------------------------------------------------------

HPALETTE PictureBitmapMakePalette(PICTURE_BITMAP_STRUCT *bmp)
{
	int pal_count;

	switch(bmp->nBitsPerPixel) {
		case 1:		pal_count = 256;	break;
		case 4:		pal_count = 256;	break;
		case 8:		pal_count = 256;	break;
		case 24:	pal_count = 256;	break;
		case 32:	pal_count = 256;	break;
		default:	return NULL;		// 16색상이나 256색상 이외에는 파레트가 없다.
	}
	
	HPALETTE hpal;
	
	if(bmp->nBitsPerPixel == 24) {
		hpal = MakePaletteWithDac(DEFAULT_RGB, pal_count);	
	}
	else if(bmp->nBitsPerPixel == 32) {
		hpal = MakePaletteWithDac(DEFAULT_RGB, pal_count);	
	}
	else {
		hpal = MakePaletteWithDac(bmp->dac, pal_count);
	}
	return hpal;
}

//------------------------------------------------------------------------------
//	그래픽을 그리기 전에 메모리를 록한다.
//------------------------------------------------------------------------------

void PictureBitmapLock(PICTURE_BITMAP_STRUCT *bmp)
{
	bmp->data = (BYTE *) GlobalLock(bmp->hData);
}

//------------------------------------------------------------------------------
//	그림을 다 그렸으면 메모리를 UnLock 한다.
//------------------------------------------------------------------------------

void PictureBitmapUnlock(PICTURE_BITMAP_STRUCT *bmp)
{
	GlobalUnlock(bmp->hData);
}

void PictureBitmapDelete(PICTURE_BITMAP_STRUCT *bmp)
{
	if(bmp->hData != NULL) {
		GlobalFree(bmp->hData);
		bmp->hData = NULL;
	}
	if(bmp->hInfo != NULL) {
		GlobalFree (bmp->hInfo);
		bmp->hInfo = NULL;
	}
	if(bmp->hPalette != NULL) {
		DeleteObject(bmp->hPalette);
		bmp->hPalette = NULL;
	}
}

void PictureBitmapGetSize(PICTURE_BITMAP_STRUCT *bmp, int &width, int &height)
{
	width  = bmp->nWidth;
	height = bmp->nHeight;
}

int PictureBitmapGetColor(PICTURE_BITMAP_STRUCT *bmp)
{
	return bmp->nBitsPerPixel;
}

void PictureBitmapPutOneLine(PICTURE_BITMAP_STRUCT *bmp, int y, BYTE *buf)
{
	if(y < 0 || y >= bmp->nHeight) {
		//MessageBox(NULL, "y < 0 || y >= height", "range over", MB_OK);
		return;
	}

	long start_pos = bmp->lBytesPerLine*y;

	for(long l = 0L; l < bmp->lBytesPerLine; l++) {
		*(bmp->data+start_pos+l) = buf[(unsigned)l];
	}
}

void PictureBitmapPutBuf(PICTURE_BITMAP_STRUCT *bmp, BYTE *buf, int x, int y, int width)
{
	if(y < 0 || y >= bmp->nHeight) {
//		MessageBox(NULL, "y < 0 || y >= height", "range over", MB_OK);
		return;
	}

	long start_pos;

	if(bmp->nBitsPerPixel == COLOR_1600) {
		start_pos = bmp->lBytesPerLine*y;

		for(int i = 0; i < width; i++) {
			if(i+x >= bmp->nWidth)	break;
			if(i+x < 0)					continue;
			*(bmp->data+start_pos+(x+i)*3+0) = buf[i*3+0];
			*(bmp->data+start_pos+(x+i)*3+1) = buf[i*3+1];
			*(bmp->data+start_pos+(x+i)*3+2) = buf[i*3+2];
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_256) {
		start_pos = bmp->lBytesPerLine*y+x;

		for(int i = 0; i < width; i++) {
			if(i+x >= bmp->nWidth)	break;
			if(i+x < 0)					continue;
			*(bmp->data+start_pos+i) = buf[i];
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_16) {
		start_pos = bmp->lBytesPerLine*y;
		int nibble;

		for(int i = 0; i < width; i++) {
			if(i+x >= bmp->nWidth)	break;
			if(i+x < 0)					continue;

			if(i%2)		nibble = (buf[i/2] >> 0) & 0x0F;		
			else			nibble = (buf[i/2] >> 4) & 0x0F;

			if((i+x)%2) bmp->data[start_pos+(i+x)/2] = (bmp->data[start_pos+(i+x)/2] & 0xF0) | (nibble);
			else			bmp->data[start_pos+(i+x)/2] = (bmp->data[start_pos+(i+x)/2] & 0x0F) | (nibble << 4);
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_2) {
		start_pos = bmp->lBytesPerLine*y;

		for(int i = 0; i < width; i++) {
			if(i+x >= bmp->nWidth)	break;
			if(i+x < 0)					continue;

			if(buf[i/8] & BIT_MASK[i%8])
				bmp->data[start_pos+(i+x)/8] |= BIT_MASK[(i+x)%8];
			else {
				bmp->data[start_pos+(i+x)/8] &= 255-BIT_MASK[(i+x)%8];
			}
		}
	}
	else {

	}
}

void PictureBitmapGetOneLine(PICTURE_BITMAP_STRUCT *bmp, int y, BYTE *buf)
{
	if(y < 0 || y >= bmp->nHeight) {
		//MessageBox(NULL, "y < 0 || y >= height", "range over", MB_OK);
		return;
	}

	long start_pos = bmp->lBytesPerLine*y;
 
	for(long l = 0L; l < bmp->lBytesPerLine; l++) {
		buf[(unsigned)l] = *(bmp->data+start_pos+l); 
	}
}

void PictureBitmapGetBuf(PICTURE_BITMAP_STRUCT *bmp, BYTE *buf, int x, int y, int width)
{
	if(y < 0 || y >= bmp->nHeight) {
		//MessageBox(NULL, "range over y < 0 || y >= height", "PictureBitmapGetBuf", MB_OK);
		return;
	}
	if(x < 0 || x >= bmp->nWidth) {
		//MessageBox(NULL, "range over x < 0 || x >= width", "PictureBitmapGetBuf", MB_OK);
		return;
	}

	long start_pos;

	if(bmp->nBitsPerPixel == COLOR_1600) {
		start_pos = bmp->lBytesPerLine*y;

		for(int i = 0; i < width; i++) {
			if(i+x > bmp->nWidth)	break;
			buf[i*3+0] = bmp->data[start_pos+(x+i)*3+0];
			buf[i*3+1] = bmp->data[start_pos+(x+i)*3+1];
			buf[i*3+2] = bmp->data[start_pos+(x+i)*3+2];
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_256) {
		start_pos = bmp->lBytesPerLine*y;

		for(int i = 0; i < width; i++) {
			if(i+x > bmp->nWidth)	break;
			buf[i] = *(bmp->data+start_pos+(x+i));
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_16) {
		start_pos = bmp->lBytesPerLine*y;
		int nibble;

		for(int i = 0; i < width; i++) {
			if(i+x >= bmp->nWidth)	break;
			if(i+x < 0)					continue;

			if(i%2)		nibble = (buf[i/2] >> 0) & 0x0F;		
			else		nibble = (buf[i/2] >> 4) & 0x0F;

			if((i+x)%2) nibble = (bmp->data[start_pos+(i+x)/2] >> 0) & 0x0F;
			else		nibble = (bmp->data[start_pos+(i+x)/2] >> 4) & 0x0F;

			if(i%2)		buf[i/2] = (buf[i/2] & 0xF0) | (nibble);
			else		buf[i/2] = (buf[i/2] & 0x0F) | (nibble << 4);
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_2) {
		start_pos = bmp->lBytesPerLine*y;

		memset(buf, 0, (width+7)/8);

		for(int i = 0; i < width; i++) {
			if(i+x >= bmp->nWidth)	break;
			if(i+x < 0)					continue;

			if(bmp->data[start_pos+(i+x)/8] & BIT_MASK[(i+x)%8])
				buf[i/8] |= BIT_MASK[i%8];
		}
	}
	else {
		ProgrammError("PictureBitmapGetBuf(), else detected");
	}
}

void PictureBitmapGetOneLineVertical(PICTURE_BITMAP_STRUCT *bmp, int x, BYTE *buf)
{
	if(x < 0 || x >= bmp->nWidth) {
		//MessageBox(NULL, "x < 0 || x >= width", "range over", MB_OK);
		return;
	}

	int yline;
	int i;

	if(bmp->nBitsPerPixel == COLOR_1600) {
		int x_x_3 = x*3;

		for(i = 0, yline = bmp->nHeight-1; i < bmp->nHeight; i++, yline--) {
			buf[i*3+0] = *(bmp->data+bmp->lBytesPerLine*yline+x_x_3+0);
			buf[i*3+1] = *(bmp->data+bmp->lBytesPerLine*yline+x_x_3+1);
			buf[i*3+2] = *(bmp->data+bmp->lBytesPerLine*yline+x_x_3+2);
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_256) {
		for(i = 0, yline = bmp->nHeight-1; i < bmp->nHeight; i++, yline--) {
			buf[i] = *(bmp->data+bmp->lBytesPerLine*yline+x);
		}
	}
	else if(bmp->nBitsPerPixel == COLOR_16) {
		int nibble;
		for(i = 0, yline = bmp->nHeight-1; i < bmp->nHeight; i++, yline--) {
			if(x%2)		nibble = (bmp->data[bmp->lBytesPerLine*yline+x/2] >> 0) & 0x0F;
			else			nibble = (bmp->data[bmp->lBytesPerLine*yline+x/2] >> 4) & 0x0F;

			if(i%2)		buf[i/2] = (buf[i/2] & 0xF0) | (nibble);
			else			buf[i/2] = (buf[i/2] & 0x0F) | (nibble << 4);
		}					
	}
	else if(bmp->nBitsPerPixel == COLOR_2) {
		memset(buf, 0, (bmp->nHeight+7)/8); 
		for(i = 0, yline = bmp->nHeight-1; i < bmp->nHeight; i++, yline--) {
			if(bmp->data[bmp->lBytesPerLine*yline+x/8] & BIT_MASK[x%8]) 
				buf[i/8] |= BIT_MASK[i%8];
		}					
	}
	else {
		ProgrammError("PictureBitmapGetOneLineVertical(), else detected");
	}
}

void PictureBitmapPutImage(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x, int y)
{
	BYTE *buf = new BYTE[(unsigned)bmp->lBytesPerLine];

	if(buf == NULL)	return;

	BITMAPINFO *info = (BITMAPINFO*)GlobalLock(bmp->hInfo);
	int i;
	long pos = 0L;

	for(i = 0; i < bmp->nHeight; i++, pos+=bmp->lBytesPerLine) {
		SetDIBitsToDevice(hdc, x, y+i, bmp->nWidth, 1,
									  0, 0, 0, 1,
									  //buf,
									  (LPSTR) (bmp->data+pos),
									  info,
									  DIB_RGB_COLORS);
	}
	GlobalUnlock(bmp->hInfo);

	delete buf;
}

void PictureBitmapPutImageViewRect(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x, int y, RECT *rect)
{
	BYTE *buf = new BYTE[(unsigned)bmp->lBytesPerLine];

	if(buf == NULL)	return;

	BITMAPINFO *info = (BITMAPINFO*)GlobalLock(bmp->hInfo);
	int i;
	long pos = 0L;

	/*
	SetDIBitsToDevice(hdc, x, y, bmp->nWidth, bmp->nHeight,
									  0, 0, 0, bmp->nHeight,
									  //buf,
									  (LPSTR) (bmp->data+pos),
									  info,
									  DIB_RGB_COLORS);
	*/

	info->bmiHeader.biHeight = 1;

	for(i = 0; i < bmp->nHeight; i++, pos+=bmp->lBytesPerLine) {
		if(y+i < rect->top)		continue;
		if(y+i > rect->bottom)	continue;

		SetDIBitsToDevice(hdc, x, y+i, bmp->nWidth, 1,
									  0, 0, 0, 1,
									  //buf,
									  (LPSTR) (bmp->data+pos),
									  info,
									  DIB_RGB_COLORS);
	}

	GlobalUnlock(bmp->hInfo);

	delete buf;
}

void PictureBitmapPutImage(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x1, int y1, int x2, int y2)
{
	if(x1 > x2)	Temp(x1, x2);
	if(y1 > y2)	Temp(y1, y2);
	
	int opt_width = x2-x1+1;
	int opt_height = y2-y1+1;

	StackBYTE buf(BmpWidthToByte(opt_width, bmp->nBitsPerPixel));
	opticBuf optic;
	HGLOBAL hInfo;

	if(buf.data == NULL)	return;
	if(!optic.Init(opt_width, bmp->nWidth, bmp->nBitsPerPixel))	return;

	hInfo = MakeBitmapInfo(opt_width, 1, bmp->nBitsPerPixel, bmp->dac);
	if(hInfo == NULL)	return;

	BITMAPINFO *info = (BITMAPINFO*)GlobalLock(hInfo);
	int i;
	int orgline;
	int oldline = -1;

	for(i = 0; i < opt_height; i++) {
		orgline = (int)((long)i*bmp->nHeight/opt_height);
		if(orgline != oldline) {
			optic.OneLine(buf.data, bmp->data+(long)orgline*bmp->lBytesPerLine);
			oldline = orgline;
		}
		SetDIBitsToDevice(hdc, x1, y1+i, opt_width, 1,
									  0, 0, 0, 1,
									  (LPSTR)buf.data,
									  info,
									  DIB_RGB_COLORS);
	}
	GlobalUnlock(hInfo);
	GlobalFree(hInfo);
}

void PictureBitmapPutImageViewRect(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x1, int y1, int x2, int y2, RECT *rect)
{
	int opt_width = x2-x1+1;
	int opt_height = y2-y1+1;

	StackBYTE buf(BmpWidthToByte(opt_width, bmp->nBitsPerPixel));
	opticBuf optic;
	HGLOBAL hInfo;

	if(buf.data == NULL)	return;
	if(!optic.Init(opt_width, bmp->nWidth, bmp->nBitsPerPixel))	return;

	hInfo = MakeBitmapInfo(opt_width, 1, bmp->nBitsPerPixel, bmp->dac);
	if(hInfo == NULL)	return;

	BITMAPINFO *info = (BITMAPINFO*)GlobalLock(hInfo);
	int i;
	int orgline;
	int oldline = -1;

	for(i = 0; i < opt_height; i++) {
		if(y1+i < rect->top)		continue;
		if(y1+i > rect->bottom)	continue;
		orgline = (int)((__int64)i*bmp->nHeight/opt_height);
		if(orgline != oldline) {
			optic.OneLine(buf.data, bmp->data+(long)orgline*bmp->lBytesPerLine);
			oldline = orgline;
		}
		SetDIBitsToDevice(hdc, x1, y1+i, opt_width, 1,
									  0, 0, 0, 1,
									  (LPSTR)buf.data,
									  info,
									  DIB_RGB_COLORS);
	}
	GlobalUnlock(hInfo);
	GlobalFree(hInfo);
}

//-------------------------------------------------------------------------------------
// 주어진 영역만을 복구한다.
//-------------------------------------------------------------------------------------

void PictureBitmapRestoreBack(HDC hdc, PICTURE_BITMAP_STRUCT *bmp, int x, int y, int x1, int y1, int x2, int y2)
{
	BYTE *buf = new BYTE[(unsigned)bmp->lBytesPerLine];

	if(buf == NULL)	return;

	BITMAPINFO *info = (BITMAPINFO*)GlobalLock(bmp->hInfo);
	
	SetDIBitsToDevice(hdc, x, y, x2-x1+1, y2-y1+1,
									  x1, bmp->nHeight-y1-1, y1, y2-y1+1,
									  (LPSTR) (bmp->data),
									  info,
									  DIB_RGB_COLORS);
	GlobalUnlock(bmp->hInfo);

	delete buf;
}

void PictureBitmapPutPixel(PICTURE_BITMAP_STRUCT *bmp, int x, int y, COLORREF color)
{
	if(x < 0)								return;
	if(y < 0)								return;
	if(x >= bmp->nWidth)					return;
	if(y >= bmp->nHeight)					return;
	if(!PictureBitmapIsPaintableZone(bmp, x, y))	return;

	switch(bmp->nBitsPerPixel) {
		case 1:
			if(color == 0) {
				bmp->data[bmp->lBytesPerLine*y+x/8] &= (255-BIT_MASK[x%8]);
			}
			else {
				bmp->data[bmp->lBytesPerLine*y+x/8] |= BIT_MASK[x%8];
			}
			break;
		case 4:	
			{
				int pos = bmp->lBytesPerLine*y+x/2;
				if((x%2) == 0) 
					bmp->data[pos] = ((bmp->data[pos]) & 0x0F) | ((BYTE)color << 4);
				else 
					bmp->data[pos] = ((bmp->data[pos]) & 0xF0) | (BYTE)color;
			}
			break;
		case 8:
			bmp->data[bmp->lBytesPerLine*y+x] = (BYTE)color;
			break;
		case 24:
			{
				long pos = bmp->lBytesPerLine*y+x*3;
				bmp->data[pos+0] = GetBValue(color);
				bmp->data[pos+1] = GetGValue(color);
				bmp->data[pos+2] = GetRValue(color);
			}
			break;
		case 32: 
			{
				long pos = bmp->lBytesPerLine*y+x*4;
				bmp->data[pos+0] = GetBValue(color);
				bmp->data[pos+1] = GetGValue(color);
				bmp->data[pos+2] = GetRValue(color);
				bmp->data[pos+3] = 0;		// GetAValue(color);	일단 값을 채운다.
			}
			break;
	}
}

COLORREF PictureBitmapGetPixel(PICTURE_BITMAP_STRUCT *bmp, int x, int y)
{
	if(x < 0)	return 0;
	if(y < 0)	return 0;
	if(x >= bmp->nWidth)	return 0;
	if(y >= bmp->nHeight)   return 0;

	switch(bmp->nBitsPerPixel) {
		case 1:
			if(bmp->data[bmp->lBytesPerLine*y+x/8] & BIT_MASK[x%8])
				return 1;
			else
				return 0;
		case 4:	
			{
				int pos = bmp->lBytesPerLine*y+x/2;

				if((x%2) == 0) 
					return (bmp->data[pos] >> 4);
				else 
					return (bmp->data[pos] & 0x0F);
			}
			break;
		case 8:
			return bmp->data[bmp->lBytesPerLine*y+x];
		case 24:
			{
				long pos = bmp->lBytesPerLine*y+x*3;
				return RGB(bmp->data[pos+2],
							  bmp->data[pos+1],
							  bmp->data[pos+0]);
			}
		case 32:
			{
				long pos = bmp->lBytesPerLine*y+x*4;
				return ARGB(	bmp->data[pos+3],
							bmp->data[pos+2],
							bmp->data[pos+1],
							bmp->data[pos+0]);
			}
		default:
			return 0;
	}
}

void PictureBitmapCopy(PICTURE_BITMAP_STRUCT *target, PICTURE_BITMAP_STRUCT *source)
{
#if	defined (_WIN32)
	for(long l = 0; l < source->nHeight; l++) {
		
		CopyMemory(&target->data[l*source->lBytesPerLine], &source->data[l*source->lBytesPerLine], source->lBytesPerLine);
	}
#else 
	for(long l = 0; l < source->nHeight*source->lBytesPerLine; l++) {
		target->data[l] = source->data[l];
	}
#endif
}






