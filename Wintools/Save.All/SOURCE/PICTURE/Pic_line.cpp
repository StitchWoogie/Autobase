#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>            
#else
#include <memory.h>
#endif
#include <stdlib.h>

#include <tools.h>
#include <glib.h>
#include <picture.h>

#define sign(x) ((x) > 0 ? 1 : ((x) == 0 ? 0 : (-1)))

//------------------------------------------------------------------------------
//	한선짜리 선을 그린다.
//------------------------------------------------------------------------------

void PictureOneLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int ix, iy, i, inc, x, y, dx, dy, plot, plotx, ploty;

	dx = x2-x1;
	dy = y2-y1;
	ix = abs(dx)+1;
	iy = abs(dy)+1;
	inc = max(ix, iy);

	plotx = x1;
	ploty = y1;
	x =y = 0;

	PicturePutPixel(pic, plotx, ploty, pic->lColorLine);

	for(i = 0; i < inc; ++i) {
		x += ix;
		y += iy;
		plot = 0;

		if(x > inc) {
			plot = -1;
			x -= inc;
			plotx += sign(dx);
		}

		if(y > inc) {
			plot = -1;
			y -= inc;
			ploty += sign(dy);
		}

		if(plot) {
			PicturePutPixel(pic, plotx, ploty, pic->lColorLine);
		}
	}
}

//------------------------------------------------------------------------------
//	굵은 선을 그린다.
//------------------------------------------------------------------------------

void PictureThickLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	int ix, iy, i, inc, x, y, dx, dy, plot, plotx, ploty;
	int r;

	// 원래의 Pattern을 저장해 둔다.
	int  nSaveFillStyle = pic->nFillStyle;			// 채우는 방법, 0-pattern, 1-gradation, 2-bitmap
	char bSaveFillOutLine = pic->bFillOutLine;	// 채울때 테두리선의 표시 여부
	FILL_PATTERN_STRUCT 	 	saveFillPat;
	memcpy(&saveFillPat, &pic->fillPat, sizeof(FILL_PATTERN_STRUCT));

	PictureSetFillStyle(pic, 0);
	PictureSetFillOutLine(pic, OFF);
	PictureSetFillPattern(pic, 0, pic->lColorLine);

	r = pic->nLineThick/2;

	dx = x2-x1;
	dy = y2-y1;
	ix = abs(dx)+1;
	iy = abs(dy)+1;
	inc = max(ix, iy);

	plotx = x1;
	ploty = y1;
	x =y = 0;

	PictureCircleFill(pic, plotx, ploty, r, r);

	for(i = 0; i < inc; ++i) {
		x += ix;
		y += iy;
		plot = 0;

		if(x > inc) {
			plot = -1;
			x -= inc;
			plotx += sign(dx);
		}

		if(y > inc) {
			plot = -1;
			y -= inc;
			ploty += sign(dy);
		}

		if(plot) {
			PictureCircleFill(pic, plotx, ploty, r, r);
		}
	}

	// 원래의 Pattern으로 복귀한다.
	PictureSetFillStyle(pic, nSaveFillStyle);
	PictureSetFillOutLine(pic, bSaveFillOutLine);
	memcpy(&pic->fillPat, &saveFillPat, sizeof(FILL_PATTERN_STRUCT));
}

void PictureLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
{
	if(pic->nLineThick == 1)
		PictureOneLine(pic, x1, y1, x2, y2);
	else
		PictureThickLine(pic, x1, y1, x2, y2);
}

void PictureMoveTo(PICTURE_STRUCT *pic, int x, int y)
{
	pic->nStartX = x;
	pic->nStartY = y;
}

void PictureLineTo(PICTURE_STRUCT *pic, int x, int y)
{
   PictureLine(pic, pic->nStartX, pic->nStartY, x, y);

	pic->nStartX = x;
	pic->nStartY = y;
}



