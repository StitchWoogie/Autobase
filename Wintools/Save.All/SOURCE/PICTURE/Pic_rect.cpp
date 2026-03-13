#include "stdafx.h"
#include <math.h>
#include <stdlib.h>
#if	defined (__BORLANDC__)
#include <mem.h>
#else
#include <memory.h>
#endif

#include <tools.h>
#include <glib.h>
#include <picture.h>

void PictureOneLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);

void PictureRectangle(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int i;

	if(x1 > x2)	Temp(x1, x2);
	if(y1 > y2)	Temp(y1, y2);

	x1 -= pic->nLineThick/2;
	y1 -= pic->nLineThick/2;
	x2 += pic->nLineThick/2;
	y2 += pic->nLineThick/2;

	for(i = 0; i < pic->nLineThick; i++, x1++, y1++, x2--, y2--) {
		PictureOneLine(pic, x1, y1, x2, y1);
		PictureOneLine(pic, x1, y1, x1, y2);
		PictureOneLine(pic, x2, y1, x2, y2);
		PictureOneLine(pic, x1, y2, x2, y2);
	}
}

void PictureRoundRectangle(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int rate=5;
	int x, y;
	int rx, ry;

	if(x1 > x2) 	Temp(x1, x2);
	if(y1 > y2)  	Temp(y1, y2);

	rx = (int)fabs((float)(x2-x1)*(float)(1.0/(float)rate));
	ry = (int)fabs((float)(y2-y1)*(float)(1.0/(float)rate));

	if(rx == 0 || ry == 0)	return;

	PictureLine(pic, x1, y1+ry, x1, y2-ry);
   PictureLine(pic, x2, y1+ry, x2, y2-ry);

	int oldx[4], oldy[4];

	for(y = 0; y <= ry; y++) {
		x = (int)sqrt( fabs( (1.0-(float)y*y/(ry*ry))*rx*rx) );
		if(y == 0) {
			oldx[0] = x1+rx-x;	oldy[0] = y1+ry-y;
			oldx[1] = x2-rx+x;	oldy[1] = y1+ry-y;
			oldx[2] = x1+rx-x;	oldy[2] = y2-ry+y;
			oldx[3] = x2-rx+x;	oldy[3] = y2-ry+y;
		}
		PictureLine(pic, oldx[0], oldy[0], x1+rx-x, y1+ry-y);
		PictureLine(pic, oldx[1], oldy[1], x2-rx+x, y1+ry-y);
		PictureLine(pic, oldx[2], oldy[2], x1+rx-x, y2-ry+y);
		PictureLine(pic, oldx[3], oldy[3], x2-rx+x, y2-ry+y);

		oldx[0] = x1+rx-x;	oldy[0] = y1+ry-y;
		oldx[1] = x2-rx+x;	oldy[1] = y1+ry-y;
		oldx[2] = x1+rx-x;	oldy[2] = y2-ry+y;
		oldx[3] = x2-rx+x;	oldy[3] = y2-ry+y;
	}

	PictureLine(pic, x1+rx-x, y1, x2-rx+x, y1);
	PictureLine(pic, x1+rx-x, y2, x2-rx+x, y2);
}

void PictureRoundRectangleFill(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int rate=5;
	int x, y;
	int rx, ry;

	if(x1 > x2) 	Temp(x1, x2);
	if(y1 > y2)  	Temp(y1, y2);

	rx = (int)fabs((float)(x2-x1)*(float)(1.0/(float)rate));
	ry = (int)fabs((float)(y2-y1)*(float)(1.0/(float)rate));

	if(rx == 0 || ry == 0)	return;

	PictureBar(pic, x1, y1+ry, x2, y2-ry);

	for(y = 0; y <= ry; y++) {
		x = (int)sqrt( fabs( (1.0-(float)y*y/(ry*ry))*rx*rx) );
		PictureBar(pic, x1+rx-x, y1+ry-y, x2-rx+x, y1+ry-y);
		PictureBar(pic, x1+rx-x, y2-ry+y, x2-rx+x, y2-ry+y);
	}
	if(pic->bFillOutLine)
		PictureRoundRectangle(pic, x1, y1, x2, y2);
}


