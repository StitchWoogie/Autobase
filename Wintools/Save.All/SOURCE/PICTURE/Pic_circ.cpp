#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>
#else
#include <memory.h>
#endif

#include <tools.h>
#include <glib.h>
#include <picture.h>

void PictureOneLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2);

static void (*PictureCircleExtend)(PICTURE_STRUCT *pic, int x, int y, int xc, int yc);

static void PictureCircleExtendLine(PICTURE_STRUCT *pic, int x, int y, int xc, int yc)
{
	PicturePutPixel(pic, xc-x,  yc+y, pic->lColorLine);
	PicturePutPixel(pic, xc+x,  yc+y, pic->lColorLine);
	PicturePutPixel(pic, xc-x,  yc-y, pic->lColorLine);
	PicturePutPixel(pic, xc+x,  yc-y, pic->lColorLine);
}

static void PictureCircleExtendThickLine1(PICTURE_STRUCT *pic, int x, int y, int xc, int yc)
{
	PictureOneLine(pic, xc-x,  yc-y, xc+x, yc-y);
	PictureOneLine(pic, xc-x,  yc+y, xc+x, yc+y);
}

static void PictureCircleExtendThickLine2(PICTURE_STRUCT *pic, int x, int y, int xc, int yc, int ix)
{
	PictureOneLine(pic, xc-x,  yc-y, xc-ix, yc-y);
	PictureOneLine(pic, xc+ix,  yc-y, xc+x, yc-y);
	PictureOneLine(pic, xc-x,  yc+y, xc-ix, yc+y);
	PictureOneLine(pic, xc+ix,  yc+y, xc+x, yc+y);
}

static void PictureCircleExtendFill(PICTURE_STRUCT *pic, int x, int y, int xc, int yc)
{
	PictureBar(pic, xc-x, yc+y, xc+x, yc+y);
	PictureBar(pic, xc-x, yc-y, xc+x, yc-y);
}

static void PictureCircleStandard(PICTURE_STRUCT *pic, int cx, int cy, int rx, int ry)
{
	register int x = 0, y;
	long a, b, d, dx = 0, dy;
	long Asquared, TwoAsquared, Bsquared, TwoBsquared;

	ry = (ry < 0) ? -ry : ry;
	rx = (rx < 0) ? -rx : rx;

	a = rx, b = ry, y = ry;
	Asquared = a*a, TwoAsquared = 2*Asquared;
	Bsquared = b*b, TwoBsquared = 2*Bsquared;

	d = Bsquared - Asquared*b + Asquared / 4l;
	dy = TwoAsquared * b;

	while(dx < dy) {
		PictureCircleExtend(pic, x, y, cx, cy);	 	// fill circle

		if(d > 0l) {
			--y;
			dy -= TwoAsquared;
			d -= dy;
		}
		++x;
		dx += TwoBsquared;
		d += Bsquared + dx;
	}

	d += (3l * (Asquared-Bsquared)/2l - (dx+dy)) / 2l;

	while(y >= 0) {
		PictureCircleExtend(pic, x, y, cx, cy);
		if(d < 0l) {
			++x;
			dx += TwoBsquared;
			d += dx;
		}
		--y;
		dy -= TwoAsquared;
		d += Asquared - dy;
	}
}

static int PictureCircleSeekInsidePos(int rx, int ry, int &ix, int seeky)
{
	register int x = 0, y;
	long a, b, d, dx = 0, dy;
	long Asquared, TwoAsquared, Bsquared, TwoBsquared;

	ry = abs(ry);
	rx = abs(rx);

	a = rx, b = ry, y = ry;
	Asquared = a*a, TwoAsquared = 2*Asquared;
	Bsquared = b*b, TwoBsquared = 2*Bsquared;

	d = Bsquared - Asquared*b + Asquared / 4l;
	dy = TwoAsquared * b;

	while(dx < dy) {
		if(seeky == y) {
			ix = x;
			return 1;
		}

		if(d > 0l) {
			--y;
			dy -= TwoAsquared;
			d -= dy;
		}
		++x;
		dx += TwoBsquared;
		d += Bsquared + dx;
	}

	d += (3l * (Asquared-Bsquared)/2l - (dx+dy)) / 2l;

	while(y >= 0) {
		if(seeky == y) {
			ix = x;
			return 1;
		}
		if(d < 0l) {
			++x;
			dx += TwoBsquared;
			d += dx;
		}
		--y;
		dy -= TwoAsquared;
		d += Asquared - dy;
	}

	return 0;
}

static void PictureCircleThickLine(PICTURE_STRUCT *pic, int cx, int cy, int rx, int ry)
{
	register int x = 0, y;
	long a, b, d, dx = 0, dy;
	long Asquared, TwoAsquared, Bsquared, TwoBsquared;
	int rx1, ry1, rx2, ry2;
	int ix;

	ry1 = ry2 = abs(ry);
	rx1 = rx2 = abs(rx);

	rx1 += pic->nLineThick/2;
	ry1 += pic->nLineThick/2;
	rx2 -= pic->nLineThick/2;
	ry2 -= pic->nLineThick/2;

	a = rx1, b = ry1, y = ry1;
	Asquared = a*a, TwoAsquared = 2*Asquared;
	Bsquared = b*b, TwoBsquared = 2*Bsquared;

	d = Bsquared - Asquared*b + Asquared / 4l;
	dy = TwoAsquared * b;

	while(dx < dy) {
		if(PictureCircleSeekInsidePos(rx2, ry2, ix, y)) {
			PictureCircleExtendThickLine2(pic, x, y, cx, cy, ix);	 	// fill circle
		}
		else {
			PictureCircleExtendThickLine1(pic, x, y, cx, cy);	 	// fill circle
		}

		if(d > 0l) {
			--y;
			dy -= TwoAsquared;
			d -= dy;
		}
		++x;
		dx += TwoBsquared;
		d += Bsquared + dx;
	}

	d += (3l * (Asquared-Bsquared)/2l - (dx+dy)) / 2l;

	while(y >= 0) {
		if(PictureCircleSeekInsidePos(rx2, ry2, ix, y)) {
			PictureCircleExtendThickLine2(pic, x, y, cx, cy, ix);	 	// fill circle
		}
		else {
			PictureCircleExtendThickLine1(pic, x, y, cx, cy);	 	// fill circle
		}

		if(d < 0l) {
			++x;
			dx += TwoBsquared;
			d += dx;
		}
		--y;
		dy -= TwoAsquared;
		d += Asquared - dy;
	}
}

void PictureCircle(PICTURE_STRUCT *pic, int cx, int cy, int rx, int ry)
{
	if(pic->nLineThick == 1) {
		PictureCircleExtend = PictureCircleExtendLine;
		PictureCircleStandard(pic, cx, cy, abs(rx), abs(ry));
	}
	else {
		PictureCircleThickLine(pic, cx, cy, abs(rx), abs(ry));
	}
}

void PictureCircleFill(PICTURE_STRUCT *pic, int cx, int cy, int rx, int ry)
{
	PictureCircleExtend = PictureCircleExtendFill;
	PictureCircleStandard(pic, cx, cy, rx, ry);

	if(pic->bFillOutLine == ON)
	   PictureCircle(pic, cx, cy, abs(rx), abs(ry));
}



