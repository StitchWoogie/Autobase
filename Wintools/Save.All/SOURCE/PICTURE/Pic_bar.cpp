#include "stdafx.h"
#include <stdlib.h>
#if	defined (__BORLANDC__)
#include <mem.h>
#else 
#include <memory.h>
#endif

#include <tools.h>
#include <glib.h>
#include <picture.h>
#include <dataswap.h>
#include <ch_buf.h>

//------------------------------------------------------------------------------
//	사각형 영역을 무늬로 채운다.
//------------------------------------------------------------------------------

static void PictureBarExtendPattern(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int x, y;

	if(pic->fillPat.nForm == 0) {	// 보통
		for(y = y1; y <= y2; y++) {
			for(x = x1; x <= x2; x++) {
				if(BIT_MASK[x%8] & pic->fillPat.pattern[y%8])
					PicturePutPixel(pic, x, y, pic->fillPat.lColorL);
				else
					PicturePutPixel(pic, x, y, 0);
			}
		}
	}
	else if(pic->fillPat.nForm == 1) {	// 투명
		for(y = y1; y <= y2; y++) {
			for(x = x1; x <= x2; x++) {
				if(BIT_MASK[x%8] & pic->fillPat.pattern[y%8])
					PicturePutPixel(pic, x, y, pic->fillPat.lColorL);
			}
		}
	}
	else {
		for(y = y1; y <= y2; y++) {
			for(x = x1; x <= x2; x++) {
				if(BIT_MASK[x%8] & pic->fillPat.pattern[y%8])
					PicturePutPixel(pic, x, y, pic->fillPat.lColorL);
				else
					PicturePutPixel(pic, x, y, pic->fillPat.lColorR);
			}
		}
	}
}

//------------------------------------------------------------------------------
//	사각형 영역을 그라데이션으로 채운다.
//------------------------------------------------------------------------------

static void PictureBarExtendGradation(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	void ggradation_line(PICTURE_STRUCT *pic, int x1, int x2, int y);

	for(int y = y1; y <= y2; y++)
		ggradation_line(pic, x1, x2, y);
}

//---------------------------------------------------------------------------------------
//	그림 패턴을 배경에 뿌릴때 16칼라 256칼라등은 미리 Table을 만들어 두면 속도가 증가된다.
//---------------------------------------------------------------------------------------

static void PictureBarExtendBitmapMakeTable(PICTURE_STRUCT *pic, int source_num, int target_num)
{
	int i;
	
	if((pic->fillBmp.bmp.nBitsPerPixel != pic->fillBmp.colorSource)  ||
		(pic->back.nBitsPerPixel != pic->fillBmp.colorTarget)) {
		goto make_table;
	}
	if(memcmp(pic->fillBmp.bmp.dac, pic->fillBmp.dacSource, source_num*3) != 0 ||
		memcmp(pic->back.dac, pic->fillBmp.dacTarget, target_num*3) != 0) { 
		goto make_table;
	}
	return;	// table이 이전의 그림과 같다.

make_table:
	for(i = 0; i < source_num; i++) {
		pic->fillBmp.dacTable[i] = SeekFitRGB(pic->back.dac, pic->fillBmp.bmp.dac[i*3+0],
 																			  pic->fillBmp.bmp.dac[i*3+1],
																			  pic->fillBmp.bmp.dac[i*3+2],
																			  target_num);
	}

	memcpy(pic->fillBmp.dacSource, pic->fillBmp.bmp.dac, source_num*3);
	memcpy(pic->fillBmp.dacTarget, pic->back.dac, target_num*3);
	pic->fillBmp.colorSource = pic->fillBmp.bmp.nBitsPerPixel;
	pic->fillBmp.colorTarget = pic->back.nBitsPerPixel;
}

//------------------------------------------------------------------------------
//	사각형 영역을 그림으로 채운다.
//------------------------------------------------------------------------------

static void PictureBarExtendBitmap(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int x, y;
	int val;
	BYTE r, g, b;
	COLORREF color;

	// ConvertBuf는 구조적으로 맞지 않다. 함수 전체를 새로구성하는 것이 좋다.

	if(pic->fillBmp.bmp.hData == NULL)	return;

	PictureBitmapLock(&pic->fillBmp.bmp);

	// 바탕이 1600만 색상이고 패턴그림이 256이나 16색상일때.
	if(pic->back.nBitsPerPixel == COLOR_1600 && 
		(pic->fillBmp.bmp.nBitsPerPixel == COLOR_256 ||
		 pic->fillBmp.bmp.nBitsPerPixel == COLOR_16 ||
		 pic->fillBmp.bmp.nBitsPerPixel == COLOR_2)) {

		for(y = y1; y <= y2; y++) {
			for(x = x1; x <= x2; x++) {
				val = (int)PictureBitmapGetPixel(&pic->fillBmp.bmp,
													 x%pic->fillBmp.bmp.nWidth,
													 y%pic->fillBmp.bmp.nHeight);
				PictureBitmapPutPixel(&pic->back, x, y,
											 RGB(pic->fillBmp.bmp.dac[val*3+0],
												  pic->fillBmp.bmp.dac[val*3+1],
												  pic->fillBmp.bmp.dac[val*3+2]) );
			}
		}
	}
	else if(pic->back.nBitsPerPixel == COLOR_2) {	// 배경이 흑백일때.
		if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_1600) {
			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					val = PictureBitmapGetPixel(&pic->fillBmp.bmp, 
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					r = GetRValue(val);
					g = GetGValue(val);
					b = GetBValue(val);
					val = DITHER_TABLE[((r>>2)*30 + (g>>2)*59 + (b>>2)*11)/100][y%8] & BIT_MASK[x%8] 
						   ? 1 : 0;

					PictureBitmapPutPixel(&pic->back, x, y, val);
				}
			}
		}
		else if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_256 ||
			     pic->fillBmp.bmp.nBitsPerPixel == COLOR_16) {
			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					val = PictureBitmapGetPixel(&pic->fillBmp.bmp, 
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					r = pic->fillBmp.bmp.dac[val*3+0];
					g = pic->fillBmp.bmp.dac[val*3+1];
					b = pic->fillBmp.bmp.dac[val*3+2];
					val = DITHER_TABLE[((r>>2)*30 + (g>>2)*59 + (b>>2)*11)/100][y%8] & BIT_MASK[x%8] 
						   ? 1 : 0;

					PictureBitmapPutPixel(&pic->back, x, y, val);
				}
			}
		}
		else 
			goto default_if;
	}
	else if(pic->back.nBitsPerPixel == COLOR_16) {	// 배경이 16색상일때.
		if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_2) {
			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					color = PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					if(color == 1)	color = 15;
					PictureBitmapPutPixel(&pic->back, x, y, color);
				}
			}
		}	
		else if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_16) {
			PictureBarExtendBitmapMakeTable(pic, 16, 16);

			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					color = PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					PictureBitmapPutPixel(&pic->back, x, y, pic->fillBmp.dacTable[color]);
				}
			}
		}
		else if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_256) {
			PictureBarExtendBitmapMakeTable(pic, 256, 16);

			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					color = PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					PictureBitmapPutPixel(&pic->back, x, y, pic->fillBmp.dacTable[color]);
				}
			}
		}	
		else {
			goto default_if;
		}
	}
	else if(pic->back.nBitsPerPixel == COLOR_256) {	// 배경이 256색상일때.
		if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_2) {
			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					color = PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					if(color == 1)	color = 255;
					PictureBitmapPutPixel(&pic->back, x, y, color);
				}
			}
		}	
		else if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_16) {
			PictureBarExtendBitmapMakeTable(pic, 16, 256);

			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					color = PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					PictureBitmapPutPixel(&pic->back, x, y, pic->fillBmp.dacTable[color]);
				}
			}
		}
		else if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_256) {
			PictureBarExtendBitmapMakeTable(pic, 256, 256);

			for(y = y1; y <= y2; y++) {
				for(x = x1; x <= x2; x++) {
					color = PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight);
					PictureBitmapPutPixel(&pic->back, x, y, pic->fillBmp.dacTable[color]);
				}
			}
		}	
		else {
			goto default_if;
		}
	}
	else {
default_if:
		for(y = y1; y <= y2; y++) {
			for(x = x1; x <= x2; x++) {
				PictureBitmapPutPixel(&pic->back, x, y,
											 PictureBitmapGetPixel(&pic->fillBmp.bmp,
																		  x%pic->fillBmp.bmp.nWidth,
																		  y%pic->fillBmp.bmp.nHeight));
			}
		}
	}

	PictureBitmapUnlock(&pic->fillBmp.bmp);
}

void PictureBar(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	if(x1 > x2)	Temp(x1, x2);
	if(y1 > y2)	Temp(y1, y2);

	if(x2 < 0)  return;
	if(y2 < 0)	return;
	if(x1 >= pic->back.nWidth)	return;
	if(y1 >= pic->back.nHeight)	return;

	if(x1 < 0)	x1 = 0;
	if(y1 < 0)	y1 = 0;
	if(x2 >= pic->back.nWidth)		x2 = pic->back.nWidth-1;
	if(y2 >= pic->back.nHeight)	y2 = pic->back.nHeight-1;

	if(pic->nFillStyle == 0)
		PictureBarExtendPattern(pic, x1, y1, x2, y2);
	else if(pic->nFillStyle == 1)
		PictureBarExtendGradation(pic, x1, y1, x2, y2);
	else
		PictureBarExtendBitmap(pic, x1, y1, x2, y2);
}

void PictureSetFillPattern(PICTURE_STRUCT *pic, int pattern, COLORREF color)
{
	PictureSetFillPattern(pic, FILL_PATTERN_SYSTEM[pattern], color);
}

void PictureSetFillPattern(PICTURE_STRUCT *pic, int pattern, COLORREF colorl, COLORREF colorr)
{
	PictureSetFillPattern(pic, FILL_PATTERN_SYSTEM[pattern], colorl, colorr);
}

void PictureSetFillPattern(PICTURE_STRUCT *pic, BYTE *pattern, COLORREF color)
{
	pic->fillPat.lColorL = color;

	memcpy(pic->fillPat.pattern, pattern, 8);
}

void PictureSetFillPattern(PICTURE_STRUCT *pic, BYTE *pattern, COLORREF colorl, COLORREF colorr)
{
	pic->fillPat.lColorL = colorl;
	pic->fillPat.lColorR = colorr;

	memcpy(pic->fillPat.pattern, pattern, 8);
}

void PictureSetFillPatternForm(PICTURE_STRUCT *pic, int form)
{
	pic->fillPat.nForm = form;
}

int PictureBitmapLoadBmp(HWND hwnd, PICTURE_BITMAP_STRUCT *pic, char *filename);

void PictureSetFillBitmap(PICTURE_STRUCT *pic, char *filename)
{
	if(strcmp(filename, pic->fillBmp.filename) == 0)	return;

	PictureBitmapDelete(&pic->fillBmp.bmp);
	if(!PictureBitmapLoadBmp(NULL, &pic->fillBmp.bmp, filename))	return;

	if(pic->back.nBitsPerPixel == COLOR_256) {
		if(pic->fillBmp.bmp.nBitsPerPixel == COLOR_256) {
			StackInt  table(256);
			if(table.data != NULL) {
				for(int i = 0; i < 256; i++) {
					table.data[i] = SeekFitRGB(pic->back.dac, pic->fillBmp.bmp.dac[i*3+0],
																			pic->fillBmp.bmp.dac[i*3+1],
																			pic->fillBmp.bmp.dac[i*3+2], 256);
				}
				PictureBitmapLock(&pic->fillBmp.bmp);
				for(int i = 0; i < pic->fillBmp.bmp.nHeight; i++) {
					for(int j = 0; j < pic->fillBmp.bmp.nWidth; j++) {
						PictureBitmapPutPixel(&pic->fillBmp.bmp,
													 j, i, 
													 table.data[PictureBitmapGetPixel(&pic->fillBmp.bmp, j, i)]);
					}
				}
				PictureBitmapUnlock(&pic->fillBmp.bmp);
			}
		}
	}

	strcpy(pic->fillBmp.filename, filename);
}

int  mask_exchange(PICTURE_STRUCT *pic, int pos, int num, char mode_rgb);
int  mask_exchange_256(int pos);

//------------------------------------------------------------------------------
//	그라데이션을 초기화 한다.
//------------------------------------------------------------------------------

void PictureSetFillGrad(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2, COLORREF color1, COLORREF color2, int mode, int num)
{
	int i;
	BYTE value_r[2], value_g[2], value_b[2];
	int val[3];
	BYTE *gimsi;
	int pos;

	if(pic->fillGrad.hLocalImsi != NULL) {
		LocalFree(pic->fillGrad.hLocalImsi);
		pic->fillGrad.hLocalImsi = NULL;
	}

	pic->fillGrad.hLocalImsi = LocalAlloc(LMEM_MOVEABLE, 1030*6);
	if(pic->fillGrad.hLocalImsi == NULL)	return;

	gimsi = (BYTE*)LocalLock(pic->fillGrad.hLocalImsi);

	pic->fillGrad.x1 = (x2 > x1) ? x1 : x2;
	pic->fillGrad.y1 = (y2 > y1) ? y1 : y2;
	pic->fillGrad.x2 = (x2 > x1) ? x2 : x1;
	pic->fillGrad.y2 = (y2 > y1) ? y2 : y1;
	pic->fillGrad.lColorL = color1;
	pic->fillGrad.lColorR = color2;

	value_r[0] = GetRValue(color1);
	value_r[1] = GetRValue(color2);

	value_g[0] = GetGValue(color1);
	value_g[1] = GetGValue(color2);

	value_b[0] = GetBValue(color1);
	value_b[1] = GetBValue(color2);

	// if sign_flag == 0 : minus

	if(mode <= 4) {
		pic->fillGrad.sign_flag[0] = (value_r[0] < value_r[1]) ? (BYTE)1 : (BYTE)0;
		pic->fillGrad.sign_flag[1] = (value_g[0] < value_g[1]) ? (BYTE)1 : (BYTE)0;
		pic->fillGrad.sign_flag[2] = (value_b[0] < value_b[1]) ? (BYTE)1 : (BYTE)0;
	}
	else {
		pic->fillGrad.sign_flag[0] = (value_r[0] > value_r[1]) ? (BYTE)1 : (BYTE)0;
		pic->fillGrad.sign_flag[1] = (value_g[0] > value_g[1]) ? (BYTE)1 : (BYTE)0;
		pic->fillGrad.sign_flag[2] = (value_b[0] > value_b[1]) ? (BYTE)1 : (BYTE)0;

		mode %= 6;
	}

	pic->fillGrad.nMethod = mode;
	if(num < 1) num = 1;
	pic->fillGrad.nCount = num;
	pic->fillGrad.nSizeX = (pic->fillGrad.x2 - pic->fillGrad.x1 + 1);
	if(pic->fillGrad.nSizeX > 1030) pic->fillGrad.nSizeX = 1029;
	pic->fillGrad.nSizeY = (pic->fillGrad.y2 - pic->fillGrad.y1 + 1);

	pic->fillGrad.r_sub = abs(value_r[1] - value_r[0]);
	pic->fillGrad.g_sub = abs(value_g[1] - value_g[0]);
	pic->fillGrad.b_sub = abs(value_b[1] - value_b[0]);

	pic->fillGrad.r1 = value_r[0];
	pic->fillGrad.g1 = value_g[0];
	pic->fillGrad.b1 = value_b[0];

	switch(mode) {
		case 1 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				for(i = 0; i < pic->fillGrad.nSizeX; i++) {
					val[0] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.r_sub * pic->fillGrad.nCount));
					val[1] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.g_sub * pic->fillGrad.nCount));
					val[2] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.b_sub * pic->fillGrad.nCount));

					val[0] = mask_exchange(pic, val[0], pic->fillGrad.r_sub, 0);
					val[1] = mask_exchange(pic, val[1], pic->fillGrad.g_sub, 1);
					val[2] = mask_exchange(pic, val[2], pic->fillGrad.b_sub, 2);
					gimsi[i*3]   = (BYTE)val[0];
					gimsi[i*3+1] = (BYTE)val[1];
					gimsi[i*3+2] = (BYTE)val[2];
				}
			}
			else {
				for(i = 0; i < pic->fillGrad.nSizeX; i++) {
					pos = (int)((float)i / pic->fillGrad.nSizeX * (64.0 *  pic->fillGrad.nCount));
					pos = mask_exchange_256(pos);
					gimsi[i] = (BYTE)pos;
				}
			}
			break;
		case 2 :
		case 3 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				for(i = 0; i < pic->fillGrad.nSizeX; i++) {
					val[0] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.r_sub * pic->fillGrad.nCount) /2.0);
					val[1] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.g_sub * pic->fillGrad.nCount) /2.0);
					val[2] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.b_sub * pic->fillGrad.nCount) /2.0);

					gimsi[i*6]   = (BYTE)(val[0] / 256);
					gimsi[i*6+1] = (BYTE)(val[0] % 256);
					gimsi[i*6+2] = (BYTE)(val[1] / 256);
					gimsi[i*6+3] = (BYTE)(val[1] % 256);
					gimsi[i*6+4] = (BYTE)(val[2] / 256);
					gimsi[i*6+5] = (BYTE)(val[2] % 256);
				}
			}
			else {
				for(i = 0; i < pic->fillGrad.nSizeX; i++) {
					pos = (int)((float)i / pic->fillGrad.nSizeX * (32.0 *  pic->fillGrad.nCount));
					gimsi[i] = (BYTE)pos;
				}
			}
			break;
		case 4 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				for(i = 0; i < pic->fillGrad.nSizeX; i++) {
					val[0] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.r_sub * pic->fillGrad.nCount * 2));
					val[1] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.g_sub * pic->fillGrad.nCount * 2));
					val[2] = (int)((float) i / pic->fillGrad.nSizeX * (pic->fillGrad.b_sub * pic->fillGrad.nCount * 2));

					gimsi[i*6]   = (BYTE)(val[0] / 256);
					gimsi[i*6+1] = (BYTE)(val[0] % 256);
					gimsi[i*6+2] = (BYTE)(val[1] / 256);
					gimsi[i*6+3] = (BYTE)(val[1] % 256);
					gimsi[i*6+4] = (BYTE)(val[2] / 256);
					gimsi[i*6+5] = (BYTE)(val[2] % 256);
				}
			}
			else {
				for(i = 0; i < pic->fillGrad.nSizeX; i++) {
					pos = (int)((float)i / pic->fillGrad.nSizeX * (128.0 *  pic->fillGrad.nCount));
					gimsi[i] = (BYTE)pos;
				}
			}
			break;
		case 5:
			pic->fillGrad.r_sub = (value_r[1] - value_r[0]);
			pic->fillGrad.g_sub = (value_g[1] - value_g[0]);
			pic->fillGrad.b_sub = (value_b[1] - value_b[0]);

			pic->fillGrad.r1 = value_r[0];
			pic->fillGrad.g1 = value_g[0];
			pic->fillGrad.b1 = value_b[0];
			break;
	}

	LocalUnlock(pic->fillGrad.hLocalImsi);
}

//------------------------------------------------------------------------------
//	주어진 한 줄을 그라데이션 한다.
//------------------------------------------------------------------------------

void ggradation_line(PICTURE_STRUCT *pic, int x1, int x2, int y)
{
	register int i, sub, ix, width = abs(x2-x1);
	int val[3], ival[3], ival2[3], iy[3], iy2[3], imsi[3], imsi2[3];
	COLORREF color;
	BYTE *gimsi;
	int pos, pos2, pos3, a, b;

	if(pic->fillGrad.hLocalImsi == NULL)	return;

	gimsi = (BYTE*)LocalLock(pic->fillGrad.hLocalImsi);

	ix = (x1 > x2) ? x2 : x1;

	switch(pic->fillGrad.nMethod) {
		case 0 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				iy[0] = iy[1] = iy[2] = y-pic->fillGrad.y1;
				val[0] = (int)((float) iy[0] / pic->fillGrad.nSizeY * pic->fillGrad.r_sub * pic->fillGrad.nCount);
				val[1] = (int)((float) iy[1] / pic->fillGrad.nSizeY * pic->fillGrad.g_sub * pic->fillGrad.nCount);
				val[2] = (int)((float) iy[2] / pic->fillGrad.nSizeY * pic->fillGrad.b_sub * pic->fillGrad.nCount);
				val[0] = mask_exchange(pic, val[0], pic->fillGrad.r_sub, 0);
				val[1] = mask_exchange(pic, val[1], pic->fillGrad.g_sub, 1);
				val[2] = mask_exchange(pic, val[2], pic->fillGrad.b_sub, 2);

				color = RGB(val[0]+pic->fillGrad.r1, val[1]+pic->fillGrad.g1, val[2]+pic->fillGrad.b1);

				for(i = 0; i <= width; i++)
					PicturePutPixel(pic, ix+i, y, color);
			}
			else {
				pos = (int)((float)(y -pic->fillGrad.y1) / pic->fillGrad.nSizeY * (64.0 *pic->fillGrad.nCount));
				pos = mask_exchange_256(pos);
				for(i = 0; i <= width; i++) {
					if(DITHER_TABLE[pos][y%8] & BIT_MASK[(ix+i)%8])	PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorR);
					else															PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorL);
				}
			}
			break;
		case 1 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				sub = abs(ix -pic->fillGrad.x1);
				for(i = 0; i <= width; i++) {
					val[0] = gimsi[(sub+i) * 3];
					val[1] = gimsi[(sub+i) * 3 + 1];
					val[2] = gimsi[(sub+i) * 3 + 2];
					PicturePutPixel(pic, ix+i, y, RGB(val[0]+pic->fillGrad.r1, val[1]+pic->fillGrad.g1, val[2]+pic->fillGrad.b1));

				}
			}
			else {
				sub = abs(ix -pic->fillGrad.x1);
				for(i = 0; i <= width; i++) {
					pos = gimsi[(sub + i) % 1029];
					if(DITHER_TABLE[pos][y%8] & BIT_MASK[(ix+i)%8]) PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorR);
					else                                       		PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorL);
				}
			}
			break;
		case 2 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				sub = abs(ix -pic->fillGrad.x1);
				iy[0] = iy[1] = iy[2] = y-pic->fillGrad.y1;
				ival[0] = (int)((float) iy[0] / pic->fillGrad.nSizeY * (pic->fillGrad.r_sub * pic->fillGrad.nCount) / 2);
				ival[1] = (int)((float) iy[1] / pic->fillGrad.nSizeY * (pic->fillGrad.g_sub * pic->fillGrad.nCount) / 2);
				ival[2] = (int)((float) iy[2] / pic->fillGrad.nSizeY * (pic->fillGrad.b_sub * pic->fillGrad.nCount) / 2);

				for(i = 0; i <= width; i++) {
					val[0] = (gimsi[(sub + i) * 6] << 8) +
								 gimsi[(sub + i) * 6 + 1] + ival[0];
					val[1] = (gimsi[(sub + i) * 6 + 2] << 8) +
								 gimsi[(sub + i) * 6 + 3] + ival[1];

					val[2] = (gimsi[(sub + i) * 6 + 4] << 8) +
								 gimsi[(sub + i) * 6 + 5] + ival[2];

					val[0] = mask_exchange(pic, val[0], pic->fillGrad.r_sub, 0);
					val[1] = mask_exchange(pic, val[1], pic->fillGrad.g_sub, 1);
					val[2] = mask_exchange(pic, val[2], pic->fillGrad.b_sub, 2);

					PicturePutPixel(pic, ix+i, y, RGB(val[0]+pic->fillGrad.r1, val[1]+pic->fillGrad.g1, val[2]+pic->fillGrad.b1));

				}
			}
			else {
				sub = abs(ix -pic->fillGrad.x1);
				pos = (int)((float)(y - pic->fillGrad.y1) / pic->fillGrad.nSizeY * (32.0 * pic->fillGrad.nCount));
				for(i = 0; i <= width; i++) {
					pos2 = gimsi[(sub + i) % 1029] + pos;
					pos2 = mask_exchange_256(pos2);
					if(DITHER_TABLE[pos2][y%8] & BIT_MASK[(ix+i)%8])PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorR);
					else                                        		PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorL);
				}
			}
			break;
		case 3 :
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				sub = abs(ix -pic->fillGrad.x1);

				iy2[0] = iy2[1] = iy2[2] = pic->fillGrad.y2 - y;
				ival[0] = (int)((float) iy2[0] / pic->fillGrad.nSizeY * (pic->fillGrad.r_sub * pic->fillGrad.nCount) / 2.0);
				ival[1] = (int)((float) iy2[1] / pic->fillGrad.nSizeY * (pic->fillGrad.g_sub * pic->fillGrad.nCount) / 2.0);
				ival[2] = (int)((float) iy2[2] / pic->fillGrad.nSizeY * (pic->fillGrad.b_sub * pic->fillGrad.nCount) / 2.0);

				for(i = 0; i <= width; i++) {
					val[0] = (gimsi[(sub + i) * 6] << 8) +
								 gimsi[(sub + i) * 6 + 1] + ival[0];
					val[1] = (gimsi[(sub + i) * 6 + 2] << 8) +
								 gimsi[(sub + i) * 6 + 3] + ival[1];
					val[2] = (gimsi[(sub + i) * 6 + 4] << 8) +
								 gimsi[(sub + i) * 6 + 5] + ival[2];

					val[0] = mask_exchange(pic, val[0], pic->fillGrad.r_sub, 0);
					val[1] = mask_exchange(pic, val[1], pic->fillGrad.g_sub, 1);
					val[2] = mask_exchange(pic, val[2], pic->fillGrad.b_sub, 2);

					PicturePutPixel(pic, ix+i, y, RGB(val[0]+pic->fillGrad.r1, val[1]+pic->fillGrad.g1, val[2]+pic->fillGrad.b1));
				}
			}
			else {
				sub = abs(ix -pic->fillGrad.x1);
				pos = (int)((float)(pic->fillGrad.y2 - y) / pic->fillGrad.nSizeY * (32.0 * pic->fillGrad.nCount));
				for(i = 0; i <= width; i++) {
					pos2 = gimsi[(sub + i) % 1029] + pos;
					pos2 = mask_exchange_256(pos2);
					if(DITHER_TABLE[pos2][y%8] & BIT_MASK[(ix+i)%8])PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorR);
					else                                        		PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorL);
				}
			}
			break;
		case 4:
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				sub = abs(ix -pic->fillGrad.x1);
				iy[0] = iy[1] = iy[2] = y - pic->fillGrad.y1;
				ival[0] = (int)((float) iy[0] / pic->fillGrad.nSizeY * (pic->fillGrad.r_sub * pic->fillGrad.nCount * 2.0));
				ival[1] = (int)((float) iy[1] / pic->fillGrad.nSizeY * (pic->fillGrad.g_sub * pic->fillGrad.nCount * 2.0));
				ival[2] = (int)((float) iy[2] / pic->fillGrad.nSizeY * (pic->fillGrad.b_sub * pic->fillGrad.nCount * 2.0));
				ival[0] = mask_exchange(pic, ival[0], pic->fillGrad.r_sub, 0);
				ival[1] = mask_exchange(pic, ival[1], pic->fillGrad.g_sub, 1);
				ival[2] = mask_exchange(pic, ival[2], pic->fillGrad.b_sub, 2);

				iy2[0] = iy2[1] = iy2[2] = pic->fillGrad.y2 - y;
				ival2[0] = (int)((float) iy2[0] / pic->fillGrad.nSizeY * (pic->fillGrad.r_sub * pic->fillGrad.nCount * 2.0));
				ival2[1] = (int)((float) iy2[1] / pic->fillGrad.nSizeY * (pic->fillGrad.g_sub * pic->fillGrad.nCount * 2.0));
				ival2[2] = (int)((float) iy2[2] / pic->fillGrad.nSizeY * (pic->fillGrad.b_sub * pic->fillGrad.nCount * 2.0));
				ival2[0] = mask_exchange(pic, ival2[0], pic->fillGrad.r_sub, 0);
				ival2[1] = mask_exchange(pic, ival2[1], pic->fillGrad.g_sub, 1);
				ival2[2] = mask_exchange(pic, ival2[2], pic->fillGrad.b_sub, 2);

				imsi[0] = (int)((float) iy[0] * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);
				imsi[1] = (int)((float) iy[1] * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);
				imsi[2] = (int)((float) iy[2] * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);

				imsi2[0] = (int)((float) iy2[0] * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);
				imsi2[1] = (int)((float) iy2[1] * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);
				imsi2[2] = (int)((float) iy2[2] * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);

				for(i = 0; i <= width; i++) {
					if(imsi[0] < (sub+i) && imsi[0] < pic->fillGrad.nSizeX-(sub+i)) {
						val[0] = ival[0];
					}
					else if(imsi2[0] < (sub+i) && imsi2[0] < pic->fillGrad.nSizeX-(sub+i)) {
						val[0] = ival2[0];
					}
					else {
						val[0] = (gimsi[(sub+i) * 6] << 8) +
									 gimsi[(sub+i) * 6 + 1];
						val[0] = mask_exchange(pic, val[0], pic->fillGrad.r_sub, 0);
					}

					if(imsi[1] < (sub+i) && imsi[1] < pic->fillGrad.nSizeX-(sub+i)) {
						val[1] = ival[1];
					}
					else if(imsi2[1] < (sub+i) && imsi2[1] < pic->fillGrad.nSizeX-(sub+i)) {
						val[1] = ival2[1];
					}
					else {
						val[1] = (gimsi[(sub+i) * 6 + 2] << 8) +
									 gimsi[(sub+i) * 6 + 3];
						val[1] = mask_exchange(pic, val[1], pic->fillGrad.g_sub, 1);
					}

					if(imsi[2] < (sub+i) && imsi[2] < pic->fillGrad.nSizeX-(sub+i)) {
						val[2] = ival[2];
					}
					else if(imsi2[2] < (sub+i) && imsi2[2] < pic->fillGrad.nSizeX-(sub+i)) {
						val[2] = ival2[2];
					}
					else {
						val[2] = (gimsi[(sub+i) * 6 + 4] << 8) +
									 gimsi[(sub+i) * 6 + 5];
						val[2] = mask_exchange(pic, val[2], pic->fillGrad.b_sub, 2);
					}
					PicturePutPixel(pic, ix+i, y, RGB(val[0]+pic->fillGrad.r1, val[1]+pic->fillGrad.g1, val[2]+pic->fillGrad.b1));
				}
			}
			else {
				sub = abs(ix -pic->fillGrad.x1);
				pos =  (int)((float)(y - pic->fillGrad.y1) / pic->fillGrad.nSizeY * (128.0 * pic->fillGrad.nCount));
				pos = mask_exchange_256(pos);
				pos2 = (int)((float)(pic->fillGrad.y2 - y) / pic->fillGrad.nSizeY * (128.0 * pic->fillGrad.nCount));
				pos2 = mask_exchange_256(pos2);
				a = (int)((float)(y - pic->fillGrad.y1) * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);
				b = (int)((float)(pic->fillGrad.y2 - y) * pic->fillGrad.nSizeX / pic->fillGrad.nSizeY);
				for(i = 0; i <= width; i++) {
					if(     a < (sub+i)  &&  a < pic->fillGrad.nSizeX-(sub+i)) pos3 = pos;
					else if(b < (sub+i)  &&  b < pic->fillGrad.nSizeX-(sub+i)) pos3 = pos2;
					else {
						pos3 = gimsi[(sub + i) % 1029];
						pos3 = mask_exchange_256(pos3);
					}
					if(DITHER_TABLE[pos3][y%8] & BIT_MASK[(ix+i)%8])PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorR);
					else                                        		PicturePutPixel(pic, ix+i, y, pic->fillGrad.lColorL);
				}
			}
			break;
		case 5:
			if(pic->back.nBitsPerPixel == COLOR_1600) {
				width = x2-x1+1;
				for(i = 0; i < width; i++) {
					PicturePutPixel(pic, x1+i, y, RGB(pic->fillGrad.r1+(int)((long)i*pic->fillGrad.r_sub/width),
																 pic->fillGrad.g1+(int)((long)i*pic->fillGrad.g_sub/width),
																 pic->fillGrad.b1+(int)((long)i*pic->fillGrad.b_sub/width)));
				}
			}
			else {
				width = x2-x1+1;
				for(i = 0; i < width; i++) {
					pos = (int)((float)i / width * (64.0 *  pic->fillGrad.nCount));
					pos = mask_exchange_256(pos);
					if(DITHER_TABLE[pos][y%8] & BIT_MASK[(ix+i)%8])	PicturePutPixel(pic, x1+i, y, pic->fillGrad.lColorR);
					else															PicturePutPixel(pic, x1+i, y, pic->fillGrad.lColorL);
				}
			}
			break;
	}

	LocalUnlock(pic->fillGrad.hLocalImsi);
}

int mask_exchange(PICTURE_STRUCT *pic, int pos, int num, char mode_rgb)
{
	register int i;

	num++;
	i = pos / num;
	pos = pos % num;
	if(i % 2 == 1) pos = num - pos - 1;

	if(mode_rgb == 0)      pos = (pic->fillGrad.sign_flag[0]) ? pos : -pos;
	else if(mode_rgb == 1) pos = (pic->fillGrad.sign_flag[1]) ? pos : -pos;
	else                   pos = (pic->fillGrad.sign_flag[2]) ? pos : -pos;

	return (pos);
}

int mask_exchange_256(int pos)
{
int i;

	i = pos / 64;
	pos %= 64;
	if(i % 2 == 0) return pos;
	else           return (63 - pos);
}



