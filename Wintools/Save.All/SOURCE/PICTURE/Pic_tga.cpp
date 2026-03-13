#include "stdafx.h"
#include <glib.h>
#include <tgatool.h>
#include <gcursor.h>
#include <dataswap.h>


HPICTURE PictureLoadTga(HWND hwnd, char *filename)
{
	tgaClass tga;
	int i;
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL)	return 0;

	if(!tga.ReadOpen(filename)) {
		return NULL;
	}
	tga.GetDac(dac.data);
	tga.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, tga.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로 그림을 읽어올 수 없습니다.", "메모리 부족", MB_OK);
		return NULL;
	}

	hPic = PictureLoadNew(hwnd, width, height, tga.GetColor(), dac.data);

	if(hPic == NULL) {
		delete buf;
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);

	if(tga.ImageOrder() == tga.IMAGE_ORDER_DOWN) {
		for(i = 0; i < height; i++) {
			WaitCursorStatus(i);
			if(!tga.GetOneLine(buf, width))	break;
			PicturePutOneLine(pic, i, buf);
		}
	}
	else {	// IMAGE_ORDER_UP
		for(i = 0; i < height; i++) {
			WaitCursorStatus(i);
			if(!tga.GetOneLine(buf, width))	break;
			PicturePutOneLine(pic, height-1-i, buf);
		}
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	delete buf;

	tga.ReadClose();

	return hPic;
}

int PictureSaveTga(HWND hwnd, HPICTURE hPic, char *filename)
{
	int  i;
	StackBYTE stack(64000u);
	PICTURE_STRUCT *pic;
	tgaClass tga;

	if(stack.data == NULL)	return 0;

	pic = PictureLock(hPic);
	tga.SetSize(pic->back.nWidth, pic->back.nHeight);
	tga.SetColor(pic->back.nBitsPerPixel);
	tga.SetDac(pic->back.dac);
	if(!tga.WriteOpen(filename)) {
		PictureUnlock(hPic, pic);
		return 0;
	}
	
	WaitCursorStart(0, pic->back.nHeight);

	// TGA image default인 아래에서 위 방향으로 이미지를 저장한다.
	for(i = 0; i < pic->back.nHeight; i++) {
		WaitCursorStatus(i);
		PictureGetOneLine(pic, pic->back.nHeight-1-i, stack.data);
		if( !tga.PutOneLine(stack.data) )		break;
	}
	WaitCursorEnd();

	PictureUnlock(hPic, pic);

	tga.WriteClose();

	return 1;
}


