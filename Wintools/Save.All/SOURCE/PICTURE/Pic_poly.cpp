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
#include <dataswap.h>

#define sign(x) ((x) > 0 ? 1 : ((x) == 0 ? 0 : (-1)))
#define	MAX_POLY_BUF	15900

void x_line2(int x1, int y1, int x2, int y2);
void MakePoint(int *point, int *element, int *array);
 
int ycomp( const void *comp1, const void *comp2)
{
	register int *c1, *c2;

	c1 = (int*) comp1;
	c2 = (int*) comp2;

	if(c1[1] > c2[1])	return 1;
	else if(c1[1] < c2[1]) return -1;
	else			return 0;
}

//--------------------------------------------------------------------
//	패턴으로 외곽선을 그린다.
//--------------------------------------------------------------------

static void PictureFillPolyDrawOutLine(PICTURE_STRUCT *pic, int x1, int y1, int x2, int y2)
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

	PictureBar(pic, plotx, ploty, plotx, ploty);

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
			PictureBar(pic, plotx, ploty, plotx, ploty);
		}
	}
}


void PictureFillPoly(PICTURE_STRUCT *pic, int num, int *point)
{
	int startx, starty;
	int i;
	int element = 0, sort=0, new_start=1, startform;
	int xy[4];
	StackInt array((MAX_POLY_BUF+3)*2);

	if(array.data == NULL)	return;

	if(num <= 2)		return;

	startform = 0;

	for(i = 0; i < num; i++) {
		if(startform == 0) {
			startx = point[i*2];
			starty = point[i*2+1];
			xy[0] = point[i*2];
			xy[1] = point[i*2+1];
			startform = 1;
			continue;
		}
		else if(point[i*2] == startx && point[i*2+1] == starty) {
			xy[2] = startx;
			xy[3] = starty;
			startform = 0;
			MakePoint(xy, &element, array.data);

			if(element%2 == 1) {
				if(element >= MAX_POLY_BUF)	return;	// buf overflow
				array.data[element*2] = startx;
				array.data[element*2+1] = starty;
				element++;
			}
			continue;
		}
		else;

		xy[2] = point[i*2];
		xy[3] = point[i*2+1];
		MakePoint(xy, &element, array.data);
		xy[0] = xy[2];
		xy[1] = xy[3];
	}

	if(point[num*2-2] != startx || point[num*2-1] != starty) {
		xy[2] = startx;
		xy[3] = starty;
		xy[0] = point[num*2-2];
		xy[1] = point[num*2-1];
		MakePoint(xy, &element, array.data);
	}

	if(element%2 == 1) {
		if(element >= MAX_POLY_BUF)	return;	// buf overflow
		array.data[element*2] = startx;
		array.data[element*2+1] = starty;
		element++;
	}

	qsort((void*) array.data, element, sizeof(int)*2, ycomp);

	sort = 0;

	while( !sort ) {
		sort = 1;

		for(i = new_start; i < element; i++) {
			if( array.data[(i-1)*2+1] == array.data[i*2+1] ) {
				if(array.data[(i-1)*2] > array.data[i*2] ) {
					Temp(array.data[(i-1)*2+1], array.data[i*2+1] );
					Temp(array.data[(i-1)*2], array.data[i*2] );
					sort = 0;
				}
			}
			else {
				if( sort == 1) new_start = i+1;
				sort = 0;
				break;
			}
		}
	}

	for (i = 0; i < element; i +=2) {
		//if(_LINE_THICK == 1)
		//	gline(array[i*2], array[(i+1)*2+1], array[(i+1)*2], array[(i+1)*2+1]);
		//else
		PictureBar(pic, array.data[i*2], array.data[(i+1)*2+1], array.data[(i+1)*2], array.data[(i+1)*2+1]);
	}

	startx = point[0], starty =point[1];

	if(pic->bFillOutLine) {
		for(i = 0; i <= num-2; i++) {
			PictureLine(pic, point[i*2], point[i*2+1], point[i*2+2], point[i*2+3]);

			if(startx == point[i*2+2] && starty == point[i*2+3]) {
				if(i+1 > num-2)	break;
				startx = point[i*2+4];
				starty = point[i*2+5];
				i++;
			}
		}

		PictureLine(pic, point[(num-1)*2], point[(num-1)*2+1], startx, starty);
	}
	else {
		for(i = 0; i <= num-2; i++) {
			PictureFillPolyDrawOutLine(pic, point[i*2], point[i*2+1], point[i*2+2], point[i*2+3]);

			if(startx == point[i*2+2] && starty == point[i*2+3]) {
				if(i+1 > num-2)	break;
				startx = point[i*2+4];
				starty = point[i*2+5];
				i++;
			}
		}

		PictureFillPolyDrawOutLine(pic, point[(num-1)*2], point[(num-1)*2+1], startx, starty);
	}
}

void MakePoint(int *point, int *element, int *array)
{
	register int ix, iy, i, inc, x, y, dx, dy, plot, plotx, ploty;

	dx = point[2]-point[0];
	dy = point[3]-point[1];
	ix = abs(dx)+1;
	iy = abs(dy)+1;
	inc = max(ix, iy);

	plotx = point[0];
	ploty = point[1];
	x =y = 0;


	if(*element == 0) {
		array[(*element)*2] = plotx;
		array[(*element)*2+1] =  ploty;
		*element = *element+1;
	}
	else if(array[(*element-1)*2+1] != point[1]) {
		if(*element >= MAX_POLY_BUF)	return;	// buf overflow
		array[(*element)*2] = plotx;
		array[(*element)*2+1] = ploty;
		*element = *element+1;
	}
	else;


	for(i = 0; i < inc; i++) {
		x += ix;
		y += iy;
		plot = 0;

		if(x > inc) {
			plot = 1;
			x -= inc;
			plotx += sign(dx);
		}

		if(y > inc) {
			plot = 1;
			y -= inc;
			ploty += sign(dy);
		}

		if(plot) {
			if(array[(*element-1)*2+1] != ploty) {
				if(*element == 1) {goto dol;}
				else if(((array[(*element-1)*2+1]-ploty) + (array[(*element-2)*2+1]-array[(*element-1)*2+1])) == 0) {
					if(*element >= MAX_POLY_BUF)	return;	// buf overflow
					array[(*element)*2] = array[(*element-1)*2];
					array[(*element)*2+1] = array[(*element-1)*2+1];
					*element = *element+1;
					array[(*element)*2] = plotx;
					array[(*element)*2+1] = ploty;
					*element = *element+1;
				}
				else {
				  dol:
					if(*element >= MAX_POLY_BUF)	return;	// buf overflow
					array[(*element)*2] = plotx;
					array[(*element)*2+1] = ploty;
					*element = *element+1;
				}
			};
		}
	}

	if(array[(*element-1)*2+1] != point[3]) {
		if(*element >= MAX_POLY_BUF)	return;	// buf overflow
		array[(*element)*2] = point[2];
		array[(*element)*2+1] = point[3];
		*element = *element+1;
	}
}

/*
#define sign(x) ((x) > 0 ? 1 : ((x) == 0 ? 0 : (-1)))

void x_line2(int x1, int y1, int x2, int y2)
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

//  Write_Pixel(plotx, ploty, color);

  for(i = 0; i < inc; ++i) {

	 x += ix;
	 y += iy;
	 plot = 0;

	 if(x > inc)
		{
		plot = -1;
		x -= inc;
		plotx += sign(dx);
		}

	 if(y > inc)
		{
		plot = -1;
		y -= inc;
		ploty += sign(dy);
		}

	 if(plot) {
	  if(TestViewPort(plotx, ploty))
		gputpixel(plotx, ploty, _COLOR_NUM); }
	 }
  }
*/









