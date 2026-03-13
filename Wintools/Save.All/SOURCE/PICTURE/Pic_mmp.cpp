#include "stdafx.h"
#include <glib.h>
#include <mmptool.h>
#include <gcursor.h>
#include <dataswap.h>

HPICTURE PictureLoadMmp(HWND hwnd, char *filename)
{
	mmpClass mmp;
	int i;
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL)	return 0;

	if(!mmp.ReadOpen(filename)) {
		return NULL;
	}
	mmp.GetDac(dac.data);
	mmp.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, mmp.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로 그림을 읽어올 수 없습니다.", "메모리 부족", MB_OK);
		return NULL;
	}

	hPic = PictureLoadNew(hwnd, width, height, mmp.GetColor(), dac.data);

	if(hPic == NULL) {
		delete buf;
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!mmp.GetOneLine(buf, width))	break;
		PicturePutOneLine(pic, i, buf);
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	delete buf;

	mmp.ReadClose();

	return hPic;
}

/*
int PictureSavePcx(HWND hwnd, HPICTURE hPic, char *filename)
{
	int  i;
	StackBYTE stack(64000u);
	PICTURE_STRUCT *pic;
	pcxClass pcx;

	if(stack.data == NULL)	return 0;

	pic = PictureLock(hPic);
	pcx.SetSize(pic->back.nWidth, pic->back.nHeight);
	pcx.SetColor(pic->back.nBitsPerPixel);
	pcx.SetDac(pic->back.dac);
	if(!pcx.WriteOpen(filename)) {
		PictureUnlock(hPic, pic);
		return 0;
	}
	
	WaitCursorStart(0, pic->back.nHeight);
	for(i = 0; i < pic->back.nHeight; i++) {
		WaitCursorStatus(i);
		PictureGetOneLine(pic, i, stack.data);
		if( !pcx.PutOneLine(stack.data) )		break;
	}
	WaitCursorEnd();

	PictureUnlock(hPic, pic);

	return 1;
}
*/


