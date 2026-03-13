// english O.K
#include "stdafx.h"
#include <glib.h>
#include <pcxtool.h>
#include <gcursor.h>
#include <dataswap.h>

HPICTURE PictureLoadPcx(HWND hwnd, char *filename)
{
	pcxClass pcx;
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

	if(!pcx.ReadOpen(hwnd, filename)) {
		return NULL;
	}
	pcx.GetDac(dac.data);
	pcx.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, pcx.GetColor());

	hPic = PictureLoadNew(hwnd, width, height, pcx.GetColor(), dac.data);

	if(hPic == NULL) {
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!pcx.GetOneLine(buf.data, width))	break;
		PicturePutOneLine(pic, i, buf.data);
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	pcx.ReadClose();

	return hPic;
}

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
	if(!pcx.WriteOpen(hwnd, filename)) {
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

	pcx.WriteClose();

	return 1;
}


