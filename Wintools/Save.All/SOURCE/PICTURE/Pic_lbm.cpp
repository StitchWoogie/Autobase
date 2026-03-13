#include "stdafx.h"
#include <glib.h>
#include <lbmtool.h>
#include <gcursor.h>
#include <dataswap.h>

HPICTURE PictureLoadLbm(HWND hwnd, char *filename)
{
	lbmClass lbm;
	int i;
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL)	return 0;

	if(!lbm.ReadOpen(hwnd, filename)) {
		return NULL;
	}
	lbm.GetDac(dac.data);
   lbm.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, lbm.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로 그림을 읽어올 수 없습니다.", "메모리 부족", MB_OK);
		return NULL;
	}

	hPic = PictureLoadNew(hwnd, width, height, lbm.GetColor(), dac.data);

	if(hPic == NULL) {
		delete buf;
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!lbm.GetOneLine(buf, width))	break;
		PicturePutOneLine(pic, i, buf);
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	delete buf;

	lbm.ReadClose();

	return hPic;
}

/*
int PictureSaveBmp(HWND hwnd, HPICTURE hPic, char *filename)
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
*/

