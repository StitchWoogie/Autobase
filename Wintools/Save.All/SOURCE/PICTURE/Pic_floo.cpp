#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>            
#else
#include <memory.h>
#endif

#include <picture.h>
#include <dataswap.h>
#include <tools.h>

typedef	struct {
	short  x1, x2;
	short  y;
	char flag;
} FLOOD_STRUCT_HORZ;

typedef	struct {
	short  y1, y2;
	short  x;
	char flag;
} FLOOD_STRUCT_VERT;

#define	MAX_FLOOD	8000

//void AddFloodFillArray(int x1, int x2, int y, int pos, int *arr);
//void gfloodfillplus_grad(int x, int y, int *arr);
//int  CheckLine_grad(PICTURE_STRUCT *pic, int *x, int *y, COLORREF color);

static int on_count = 0;

static void AddFloodFillArrayHorz(int x1, int x2, int y, int pos, FLOOD_STRUCT_HORZ *arr)
{
	arr[pos].x1 = x1;
	arr[pos].x2 = x2;
	arr[pos].y  = y;
	arr[pos].flag = ON;
	on_count ++;
}

static void AddFloodFillArrayVert(int y1, int y2, int x, int pos, FLOOD_STRUCT_VERT *arr)
{
	arr[pos].y1 = y1;
	arr[pos].y2 = y2;
	arr[pos].x  = x;
	arr[pos].flag = ON;
	on_count ++;
}

static int LineExistHorz(FLOOD_STRUCT_HORZ *array, int *x, int *y, int hap)
{
	int i;

	for(i = 0; i < hap; i++) {
		if(*y != array[i].y)	continue;
		if(*x >= array[i].x1 && *x <= array[i].x2) {
			*x = array[i].x1;
			*y = array[i].x2;
			return 1;
		}
	}
	return 0;
}

static int LineExistVert(FLOOD_STRUCT_VERT *array, int *x, int *y, int hap)
{
	int i;

	for(i = 0; i < hap; i++) {
		if(*x != array[i].x)	continue;
		if(*y >= array[i].y1 && *y <= array[i].y2) {
			*x = array[i].y1;
			*y = array[i].y2;
			return 1;
		}
	}
	return 0;
}

//---------------------------------------------------------------------------
//	x, y로 주어지나 return은 x1, x2값이 return 된다.
//---------------------------------------------------------------------------

static int CheckLineHorz(PICTURE_STRUCT *pic, int *x, int *y, COLORREF color)
{
//   int i;
	COLORREF bkcolor = PictureGetPixel(pic, *x, *y);
	int x1=*x, x2=*x;

	if(bkcolor != color)	return 0;

	while(1) {
		if(PictureGetPixel(pic, x1, *y) != color) {
			x1++;
			break;
		}
		//if(x1 <= _VIEW_PORT_X1)	break;
		if(x1 <= 0)	break;
		x1--;
	}

	while(1) {
		if(PictureGetPixel(pic, x2, *y) != color) {
			x2--;
			break;
		}
		//if(x2 >= _VIEW_PORT_X2)	break;
		if(x2 >= pic->back.nWidth-1)	break;
		x2++;
	}
	*x = x1;
	*y = x2;
	return 1;
}

//---------------------------------------------------------------------------
//	x, y로 주어지나 return은 y1, y2값이 return 된다.
//---------------------------------------------------------------------------

static int CheckLineVert(PICTURE_STRUCT *pic, int *x, int *y, COLORREF color)
{
//   int i;
	COLORREF bkcolor = PictureGetPixel(pic, *x, *y);
	int y1=*y, y2=*y;

	if(bkcolor != color)	return 0;

	while(1) {
		if(PictureGetPixel(pic, *x, y1) != color) {
			y1++;
			break;
		}
		//if(x1 <= _VIEW_PORT_X1)	break;
		if(y1 <= 0)	break;
		y1--;
	}

	while(1) {
		if(PictureGetPixel(pic, *x, y2) != color) {
			y2--;
			break;
		}
		//if(x2 >= _VIEW_PORT_X2)	break;
		if(y2 >= pic->back.nHeight-1)	break;
		y2++;
	}
	*x = y1;
	*y = y2;
	return 1;
}

void PictureFloodFill(PICTURE_STRUCT *pic, int x, int y)
{
	register int x1=x, x2=x;
	register int i, j, hap;
	COLORREF bkcolor = PictureGetPixel(pic, x, y);
	StackBYTE block(sizeof(FLOOD_STRUCT_HORZ)*MAX_FLOOD);
	FLOOD_STRUCT_HORZ *array;

	if(x < 0 || y < 0)									return;	// zone over
	if(x >= pic->back.nWidth || y >= pic->back.nHeight)	return;	// zone over

	if(block.data == NULL)	return;

	on_count = 0;

	array = (FLOOD_STRUCT_HORZ*) block.data;

	x2 = y;
	x1 = x;

	CheckLineHorz(pic, &x1, &x2, bkcolor);
	//PictureBar(pic, x1, y, x2, y);
	//if(bkcolor == PictureGetPixel(pic, x, y))	return;
	
	AddFloodFillArrayHorz(x1, x2, y, 0, array);

	hap = 1;
	while(1) {
		if(on_count == 0)	break;		// yes o.k end flood fill

		for(i = 0; i < hap; i++) {
			if(array[i].flag == OFF)	continue;

			// 현재 라인에서 위로 통과 되는가를 검사한다.
			if(array[i].y > 0) {
				for(j = array[i].x1; j <= array[i].x2; j++) {
					x1 = j;
					x2 = array[i].y-1;
					if(LineExistHorz(array, &x1, &x2, hap)) {
						j = x2;
					}
					else if(CheckLineHorz(pic, &x1, &x2, bkcolor)) {
						AddFloodFillArrayHorz(x1, x2, array[i].y-1, hap, array);
						hap++;
						if(hap >= MAX_FLOOD)	goto draw_routine;
						j = x2;
					}
					else;
				}
			}
			// 현재 라인에서 아래로 통과 되는가를 검사한다.
			if(array[i].y < pic->back.nHeight-1) {
				for(j = array[i].x1; j <= array[i].x2; j++) {
					x1 = j;
					x2 = array[i].y+1;
					if(LineExistHorz(array, &x1, &x2, hap)) {
						j = x2;
					}
					else if(CheckLineHorz(pic, &x1, &x2, bkcolor)) {
						AddFloodFillArrayHorz(x1, x2, array[i].y+1, hap, array);
						hap++;
						if(hap >= MAX_FLOOD)	goto draw_routine;
						j = x2;
					}
				}
			}

			array[i].flag = OFF;
			on_count--;
		}
	}

  draw_routine:
	for(i = 0; i < hap; i++) {
		PictureBar(pic, array[i].x1, array[i].y, array[i].x2, array[i].y);
	}
}

//----------------------------------------------------------------------------------------
//	채울 공간의 테두리만 채워준다.
//----------------------------------------------------------------------------------------

static int FillStructHorz(PICTURE_STRUCT *pic, int x, int y, FLOOD_STRUCT_HORZ *array_horz)
{
	register int x1=x, x2=x;
	register int i, j, hap;
	COLORREF bkcolor = PictureGetPixel(pic, x, y);

	hap = 0;

	if(x < 0 || y < 0)									return 0;	// zone over
	if(x >= pic->back.nWidth || y >= pic->back.nHeight)	return 0;	// zone over

	on_count = 0;

	x2 = y;
	x1 = x;

	CheckLineHorz(pic, &x1, &x2, bkcolor);
	//PictureBar(pic, x1, y, x2, y);
	//if(bkcolor == PictureGetPixel(pic, x, y))	return;
	
	AddFloodFillArrayHorz(x1, x2, y, 0, array_horz);

	hap = 1;
	while(1) {
		if(on_count == 0)	return hap;		// yes o.k end flood fill

		for(i = 0; i < hap; i++) {
			if(array_horz[i].flag == OFF)	continue;

			// 현재 라인에서 위로 통과 되는가를 검사한다.
			if(array_horz[i].y > 0) {
				for(j = array_horz[i].x1; j <= array_horz[i].x2; j++) {
					x1 = j;
					x2 = array_horz[i].y-1;
					if(LineExistHorz(array_horz, &x1, &x2, hap)) {
						j = x2;
					}
					else if(CheckLineHorz(pic, &x1, &x2, bkcolor)) {
						AddFloodFillArrayHorz(x1, x2, array_horz[i].y-1, hap, array_horz);
						hap++;
						if(hap >= MAX_FLOOD)	return hap;
						j = x2;
					}
					else;
				}
			}
			// 현재 라인에서 아래로 통과 되는가를 검사한다.
			if(array_horz[i].y < pic->back.nHeight-1) {
				for(j = array_horz[i].x1; j <= array_horz[i].x2; j++) {
					x1 = j;
					x2 = array_horz[i].y+1;
					if(LineExistHorz(array_horz, &x1, &x2, hap)) {
						j = x2;
					}
					else if(CheckLineHorz(pic, &x1, &x2, bkcolor)) {
						AddFloodFillArrayHorz(x1, x2, array_horz[i].y+1, hap, array_horz);
						hap++;
						if(hap >= MAX_FLOOD)	return hap;
						j = x2;
					}
				}
			}

			array_horz[i].flag = OFF;
			on_count--;
		}
	}
}

//----------------------------------------------------------------------------------------
//	채울 공간의 테두리만 채워준다.
//----------------------------------------------------------------------------------------

static int FillStructVert(PICTURE_STRUCT *pic, int x, int y, FLOOD_STRUCT_VERT *array_vert)
{
	register int y1=y, y2=y;
	register int i, j, hap;
	COLORREF bkcolor = PictureGetPixel(pic, x, y);

	hap = 0;

	if(x < 0 || y < 0)									return 0;	// zone over
	if(x >= pic->back.nWidth || y >= pic->back.nHeight)	return 0;	// zone over

	on_count = 0;

	y2 = y;
	y1 = x;

	CheckLineVert(pic, &y1, &y2, bkcolor);
	//PictureBar(pic, x1, y, x2, y);
	//if(bkcolor == PictureGetPixel(pic, x, y))	return;
	
	AddFloodFillArrayVert(y1, y2, x, 0, array_vert);

	hap = 1;
	while(1) {
		if(on_count == 0)	return hap;		// yes o.k end flood fill

		for(i = 0; i < hap; i++) {
			if(array_vert[i].flag == OFF)	continue;

			// 현재 라인에서 위로 통과 되는가를 검사한다.
			if(array_vert[i].x > 0) {
				for(j = array_vert[i].y1; j <= array_vert[i].y2; j++) {
					y1 = array_vert[i].x-1;
					y2 = j;
					if(LineExistVert(array_vert, &y1, &y2, hap)) {
						j = y2;
					}
					else if(CheckLineVert(pic, &y1, &y2, bkcolor)) {
						AddFloodFillArrayVert(y1, y2, array_vert[i].x-1, hap, array_vert);
						hap++;
						if(hap >= MAX_FLOOD)	return hap;
						j = y2;
					}
					else;
				}
			}
			// 현재 라인에서 아래로 통과 되는가를 검사한다.
			if(array_vert[i].x < pic->back.nWidth-1) {
				for(j = array_vert[i].y1; j <= array_vert[i].y2; j++) {
					y1 = array_vert[i].x+1;
					y2 = j;
					if(LineExistVert(array_vert, &y1, &y2, hap)) {
						j = y2;
					}
					else if(CheckLineVert(pic, &y1, &y2, bkcolor)) {
						AddFloodFillArrayVert(y1, y2, array_vert[i].x+1, hap, array_vert);
						hap++;
						if(hap >= MAX_FLOOD)	return hap;
						j = y2;
					}
					else;
				}
			}

			array_vert[i].flag = OFF;
			on_count--;
		}
	}
}


void PictureFloodFillOffset(PICTURE_STRUCT *pic, int x, int y, COLORREF color)
{
	register int i, hap_horz = 0, hap_vert = 0;
	StackBYTE block_horz(sizeof(FLOOD_STRUCT_HORZ)*MAX_FLOOD);
	StackBYTE block_vert(sizeof(FLOOD_STRUCT_VERT)*MAX_FLOOD);
	FLOOD_STRUCT_HORZ *array_horz;
	FLOOD_STRUCT_VERT *array_vert;

	if(x < 0 || y < 0)									return;	// zone over
	if(x >= pic->back.nWidth || y >= pic->back.nHeight)	return;	// zone over

	if(block_horz.data == NULL)	return;
	if(block_vert.data == NULL)	return;

	array_horz = (FLOOD_STRUCT_HORZ*) block_horz.data;
	array_vert = (FLOOD_STRUCT_VERT*) block_vert.data;

	hap_horz = FillStructHorz(pic, x, y, array_horz);
	hap_vert = FillStructVert(pic, x, y, array_vert);

	for(i = 0; i < hap_horz; i++) {
		//PictureBar(pic, array_horz[i].x1, array_horz[i].y, array_horz[i].x2, array_horz[i].y);
		PicturePutPixel(pic, array_horz[i].x1, array_horz[i].y, color);
		PicturePutPixel(pic, array_horz[i].x2, array_horz[i].y, color);
	}
	for(i = 0; i < hap_vert; i++) {
		//PictureBar(pic, array_vert[i].x, array_vert[i].y1, array_vert[i].x, array_vert[i].y2);
		PicturePutPixel(pic, array_vert[i].x, array_vert[i].y1, color);
		PicturePutPixel(pic, array_vert[i].x, array_vert[i].y2, color);
	}
}



