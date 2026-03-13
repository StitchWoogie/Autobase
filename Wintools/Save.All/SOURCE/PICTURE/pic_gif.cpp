#include "stdafx.h"
#include <glib.h>
#include <giftool.h>
#include <gcursor.h>
#include <dataswap.h>

HPICTURE PictureLoadGif(HWND hwnd, char *filename)
{
	gifClass gif;
	int i;
	WORD widthbyte;
	StackBYTE buf(64000L);
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL ||
		buf.data == NULL)	{
		return 0;
	}

	if(!gif.ReadOpen(hwnd, filename)) {
		return NULL;
	}

	gif.GetDac(dac.data);
	gif.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, gif.GetColor());

	hPic = PictureLoadNew(hwnd, width, height, gif.GetColor(), dac.data);

	if(hPic == NULL) {
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);

	if(gif.GetInterlace()) {
		int pos = 0;
		int j;

		for(j = 0; j < 4; j++) {
			for(i = interlace_start[j]; i < height; i += interlace_plus[j]) {
				if(pos >= height)	break;
				WaitCursorStatus(pos);
				if(!gif.GetOneLine(buf.data, width))	break;
				PicturePutOneLine(pic, i, buf.data);
				pos++;
			}
		}
	}
	else {
		for(i = 0; i < height; i++) {
			WaitCursorStatus(i);
			if(!gif.GetOneLine(buf.data, width))	break;
			PicturePutOneLine(pic, i, buf.data);
		}
	}

	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	gif.ReadClose();

	return hPic;
}

int PictureSaveGif(HWND hwnd, HPICTURE hPic, char *filename)
{
	int  i;
	StackBYTE stack(64000u);
	PICTURE_STRUCT *pic;
	gifClass gif;

	if(stack.data == NULL)	return 0;

	pic = PictureLock(hPic);

	if(pic->back.nBitsPerPixel == COLOR_1600) {
		MessageBox(hwnd, "24 Bit 색상은 GIF파일로 저장할 수 없습니다.\n다른 파일 형식으로 저장하세요.", filename, MB_OK);
		PictureUnlock(hPic, pic);
		return 0;
	}

	gif.SetSize(pic->back.nWidth, pic->back.nHeight);
	gif.SetColor(pic->back.nBitsPerPixel);
	gif.SetDac(pic->back.dac);
	if(!gif.WriteOpen(hwnd, filename)) {
		PictureUnlock(hPic, pic);
		return 0;
	}
	
	WaitCursorStart(0, pic->back.nHeight);
	for(i = 0; i < pic->back.nHeight; i++) {
		WaitCursorStatus(i);
		PictureGetOneLine(pic, i, stack.data);
		if( !gif.PutOneLine(stack.data) )		break;
	}
	WaitCursorEnd();

	PictureUnlock(hPic, pic);

	gif.WriteClose();

	return 1;
}



