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

void PictureSetLock(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	if(x1 > x2)	Temp(x1, x2);
	if(y1 > y2)	Temp(y1, y2);

	PICTURE_BITMAP_STRUCT *bmp;

	bmp = &pic->back;

	if(x1 < 0)	x1 = 0;
	if(y1 < 0)	y1 = 0;
	if(x2 >= bmp->nWidth)	x2 = bmp->nWidth-1;
	if(y2 >= bmp->nHeight)	y2 = bmp->nHeight-1;

	bmp->nDrawZoneX1 = x1;
	bmp->nDrawZoneY1 = y1;
	bmp->nDrawZoneX2 = x2;
	bmp->nDrawZoneY2 = y2;
}

int PictureBitmapIsPaintableZone(PICTURE_BITMAP_STRUCT *bmp, int x, int y)
{
	if(x < bmp->nDrawZoneX1)	return 0;
	if(y < bmp->nDrawZoneY1)	return 0;
	if(x > bmp->nDrawZoneX2)	return 0;
	if(y > bmp->nDrawZoneY2)	return 0;

	return 1;
}

int PictureIsPaintableZone(PICTURE_STRUCT *pic, int x, int y)
{
	return PictureBitmapIsPaintableZone(&pic->back, x, y);
}
