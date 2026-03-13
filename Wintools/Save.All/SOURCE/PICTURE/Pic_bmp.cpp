#include "stdafx.h"
#include <glib.h>
#include <bmptool.h>
#include <gcursor.h>
#include <dataswap.h>

HPICTURE PictureLoadBmp(HWND hwnd, const char *filename)
{
	bmpClass bmp;
	int i;
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL)	return 0;

	if(!bmp.ReadOpen(hwnd, filename)) {
		return NULL;
	}
	bmp.GetDac(dac.data);
	bmp.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, bmp.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "Can't load picture because memory insufficent.", filename, MB_OK);
		return NULL;
	}

	hPic = PictureLoadNew(hwnd, width, height, bmp.GetColor(), dac.data);

	if(hPic == NULL) {
		delete buf;
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!bmp.GetOneLine(buf, width))	break;
		PicturePutOneLine(pic, height-1-i, buf);
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	delete buf;

	bmp.ReadClose();

	return hPic;
}

int PictureSaveBmp(HWND /*hwnd*/, HPICTURE hPic, const char *filename)
{
	int  i, j;
	StackBYTE stack(64000u);
	PICTURE_STRUCT *pic;
	bmpClass bmp;

	if(stack.data == NULL)	return 0;

	pic = PictureLock(hPic);
	bmp.SetSize(pic->back.nWidth, pic->back.nHeight);
	bmp.SetColor(pic->back.nBitsPerPixel);
	bmp.SetDac(pic->back.dac);

	if(!bmp.WriteOpen(filename)) {
		PictureUnlock(hPic, pic);
		return 0;
	}
	
	WaitCursorStart(0, pic->back.nHeight);
	for(i = (int)pic->back.nHeight-1, j = 0; i >= 0; i--, j++) {
		WaitCursorStatus(i);
		PictureGetOneLine(pic, i, stack.data);
		if( !bmp.PutOneLine(stack.data) )		break;
	}
	WaitCursorEnd();

	bmp.WriteClose();

	PictureUnlock(hPic, pic);
	return 1;
}

