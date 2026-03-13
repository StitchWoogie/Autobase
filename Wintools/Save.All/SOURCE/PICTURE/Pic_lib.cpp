#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>
#else
#include <memory.h>
#endif

#include <tools.h>
#include <glib.h>
#include <picture.h>

//------------------------------------------------------------------------------
//	스트럭쳐에 새로운 DAC로 바꾼다.
//------------------------------------------------------------------------------

void PictureSetDac(PICTURE_STRUCT *pic, BYTE *dac)
{
	PictureBitmapSetDac(&pic->back, dac); 
}

//------------------------------------------------------------------------------
//	주어진 정보로 새 그림을 만든다.
//------------------------------------------------------------------------------

HPICTURE PictureLoadNew(HWND hwnd, int width, int height, int bitsperpixel, BYTE *dac)
{
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	hPic = GlobalAlloc(GMEM_MOVEABLE, sizeof(PICTURE_STRUCT));
	if(hPic == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로\n그림을 만들 수 없습니다.", "메모리 부족", MB_OK);
		return NULL;
	}

	pic = (PICTURE_STRUCT*) GlobalLock(hPic);

	memset(pic, 0, sizeof(PICTURE_STRUCT));

	if(!PictureBitmapLoadNew(hwnd, &pic->back, width, height, bitsperpixel, dac)) {
		GlobalUnlock(hPic);
		GlobalFree(hPic);
		return NULL;
	}

	// 각종 변수 초기화
	pic->lColorLine = 0L;
	pic->nLineThick = 1;
	pic->nStartX = 0;			// move to 했을때 움직이는 포인트
	pic->nStartY = 0;

	pic->nFillStyle = 0;		  				// pattern fill
	pic->bFillOutLine = ON;					// 채울때 테두리선의 표시 여부

	pic->fillPat.lColorL = WHITE_COLOR;
	pic->fillPat.lColorR = DARK_COLOR;
	pic->fillPat.nForm = 0;					// 보통
	memset(pic->fillPat.pattern, 255, 8);		// 기본 패턴을 초기화 한다.

	pic->fillGrad.nMethod = 0;					// 위에서 아래 방향
	pic->fillGrad.nCount  = 1;					// 반복 횟수
	pic->fillGrad.lColorL = WHITE_COLOR;
	pic->fillGrad.lColorR = DARK_COLOR;
	pic->fillGrad.hLocalImsi = NULL;			// 그라데이션에서 쓰는 임시버퍼

	GlobalUnlock(hPic);

	return hPic;
}

//------------------------------------------------------------------------------
//	주어진 비트맵 파레트를 가지고 파레트 핸들을 만든다.
//------------------------------------------------------------------------------

HPALETTE PictureMakePalette(PICTURE_STRUCT *pic)
{
	return PictureBitmapMakePalette(&pic->back);
}


//------------------------------------------------------------------------------
//	그래픽을 그리기전에 메모리를 록한다.
//------------------------------------------------------------------------------

PICTURE_STRUCT *PictureLock(HPICTURE hPic)
{
	PICTURE_STRUCT *pic;

	pic = (PICTURE_STRUCT*) GlobalLock(hPic);
	PictureBitmapLock(&pic->back);

	return pic;
}

//------------------------------------------------------------------------------
//	그림을 다 그렸으면 메모리를 UnLock 한다.
//------------------------------------------------------------------------------

void PictureUnlock(HPICTURE hPic, PICTURE_STRUCT *pic)
{
	PictureBitmapUnlock(&pic->back);
	GlobalUnlock(hPic);
}

void PictureDelete(HPICTURE hPic)
{
	PICTURE_STRUCT *pic;

	pic = (PICTURE_STRUCT*)  GlobalLock(hPic);
	PictureBitmapDelete(&pic->back);

	if(pic->fillGrad.hLocalImsi != NULL) {
		LocalFree(pic->fillGrad.hLocalImsi);
		pic->fillGrad.hLocalImsi = NULL;			// 그라데이션에서 쓰는 임시버퍼
	}

/*
	if(pic->bmp.hPicture != NULL)	{			// 속을 채우는 그림
		PictureDelete(pic->bmp.hPicture);
		pic->bmp.hPicture = NULL;
	}
*/

	GlobalUnlock(hPic);

	GlobalFree(hPic);
}

void PictureGetSize(PICTURE_STRUCT *pic, int &width, int &height)
{
	PictureBitmapGetSize(&pic->back, width, height);
}

int PictureGetColor(PICTURE_STRUCT *pic)
{
	return PictureBitmapGetColor(&pic->back);
}

void PicturePutOneLine(PICTURE_STRUCT *pic, int y, BYTE *buf)
{
	PictureBitmapPutOneLine(&pic->back, y, buf);
}

void PicturePutBuf(PICTURE_STRUCT *pic, BYTE *buf, int x, int y, int width)
{
	PictureBitmapPutBuf(&pic->back, buf, x, y, width);
}

void PictureGetOneLine(PICTURE_STRUCT *pic, int y, BYTE *buf)
{
	PictureBitmapGetOneLine(&pic->back, y, buf);
}

void PictureGetBuf(PICTURE_STRUCT *pic, BYTE *buf, int x, int y, int width)
{
	PictureBitmapGetBuf(&pic->back, buf, x, y, width);
}

void PictureGetOneLineVertical(PICTURE_STRUCT *pic, int x, BYTE *buf)
{
	PictureBitmapGetOneLineVertical(&pic->back, x, buf);
}

void PicturePutImage(HDC hdc, PICTURE_STRUCT *pic, int x, int y)
{
	PictureBitmapPutImage(hdc, &pic->back, x, y);
}

void PicturePutImageViewRect(HDC hdc, PICTURE_STRUCT *pic, int x, int y, RECT *rect)
{
	PictureBitmapPutImageViewRect(hdc, &pic->back, x, y, rect);
}

void PicturePutImage(HDC hdc, PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	PictureBitmapPutImage(hdc, &pic->back, x1, y1, x2, y2);
}

void PicturePutImageViewRect(HDC hdc, PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2, RECT *rect)
{
	PictureBitmapPutImageViewRect(hdc, &pic->back, x1, y1, x2, y2, rect);
}

void PictureRestoreBack(HDC hdc, PICTURE_STRUCT *pic, int x, int y, int x1, int y1, int x2, int y2)
{
	PictureBitmapRestoreBack(hdc, &pic->back, x, y, x1, y1, x2, y2);
}

void PicturePutPixel(PICTURE_STRUCT *pic, int x, int y, COLORREF color)
{
	PictureBitmapPutPixel(&pic->back, x, y, color);
}

COLORREF PictureGetPixel(PICTURE_STRUCT *pic, int x, int y)
{
	return PictureBitmapGetPixel(&pic->back, x, y);
}

void PictureCopy(PICTURE_STRUCT *target, PICTURE_STRUCT *source)
{
	PictureBitmapCopy(&target->back, &source->back);
}

void PictureSetFillStyle(PICTURE_STRUCT *pic, int num)
{
	pic->nFillStyle = num;
}

//------------------------------------------------------------------------------
//	속을 채울때 테두리선의 표시 여부.
//------------------------------------------------------------------------------

void PictureSetFillOutLine(PICTURE_STRUCT *pic, char flag)
{
	pic->bFillOutLine = flag;
}

void PictureSetColor(PICTURE_STRUCT *pic, COLORREF color)
{
	pic->lColorLine = color;
}

void PictureSetLineThick(PICTURE_STRUCT *pic, int thick)
{
	pic->nLineThick = thick;
}

void PictureClear(HPICTURE hPic, COLORREF color)
{
	PICTURE_STRUCT *pic;

	pic = PictureLock(hPic);
	PictureSetFillStyle(pic, 0);
	PictureSetFillPattern(pic, 0, color);
	PictureBar(pic, 0, 0, pic->back.nWidth-1, pic->back.nHeight-1);
	PictureUnlock(hPic, pic);
}




