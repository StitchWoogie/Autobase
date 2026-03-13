#include "stdafx.h"
#include <glib.h>
#include <spttool.h>
#include <gcursor.h>
#include <dataswap.h>

HPICTURE PictureLoadSpt(HWND hwnd, char *filename)
{
	sptClass spt;
	int i; 
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL)	return 0;

	if(!spt.ReadOpen(filename)) {
		return NULL;
	}
	spt.GetDac(dac.data);
	spt.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, spt.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로 그림을 읽어올 수 없습니다.", "메모리 부족", MB_OK);
		return NULL;
	}

	hPic = PictureLoadNew(hwnd, width, height, spt.GetColor(), dac.data);

	if(hPic == NULL) {
		delete buf;
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!spt.GetOneLine(buf, width))	break;
		PicturePutOneLine(pic, i, buf);
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	delete buf;

	spt.ReadClose();

	return hPic;
}

int PictureSaveSpt(HWND hwnd, HPICTURE hPic, char *filename)
{
	int  i;
	StackBYTE stack(64000u);
	PICTURE_STRUCT *pic;
	sptClass spt;

	if(stack.data == NULL)	return 0;

	pic = PictureLock(hPic);
	spt.SetSize(pic->back.nWidth, pic->back.nHeight);
	spt.SetColor(pic->back.nBitsPerPixel);
	spt.SetDac(pic->back.dac);
	if(!spt.WriteOpen(filename)) {
		PictureUnlock(hPic, pic);
		return 0;
	}
	
	WaitCursorStart(0, pic->back.nHeight);
	for(i = 0; i < pic->back.nHeight; i++) {
		WaitCursorStatus(i);
		PictureGetOneLine(pic, i, stack.data);
		if( !spt.PutOneLine(stack.data) )		break;
	}
	WaitCursorEnd();

	spt.WriteClose();

	PictureUnlock(hPic, pic);

	return 1;
}



