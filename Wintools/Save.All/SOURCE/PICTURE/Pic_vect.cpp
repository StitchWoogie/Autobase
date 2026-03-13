#include "stdafx.h"
#include <picture.h>
#include <tools.h>
#include <dataswap.h>
#include <glib.h>
#include <math_lib.h>

#include <ttftool.h>

enum {
	START,
	CONNECT,
	LINE,
};

typedef struct {
	SHORT x;
	SHORT y;
	char  flag;
} VECTOR_POINT;

#define	MAX_VECTOR_POINT	10000

#define	MAX_POLY_BUF	15900

static int ycomp( const void *comp1, const void *comp2)
{
   register SHORT *c1, *c2;

   c1 = (SHORT*) comp1;
   c2 = (SHORT*) comp2;

   if(c1[1] > c2[1])			return 1;
   else if(c1[1] < c2[1])	return -1;
   else							return 0;
}

static int xcomp( const void *comp1, const void *comp2)
{
   register SHORT *c1, *c2;

   c1 = (SHORT*) comp1;
   c2 = (SHORT*) comp2;

   if(c1[0] > c2[0])			return 1;
   else if(c1[0] < c2[0])	return -1;
   else							return 0;
}

#define sign(x) ((x) > 0 ? 1 : ((x) == 0 ? 0 : (-1)))

static void MakePoint(int *point, int &element, SHORT *array, char new_flag)
{
   int ix, iy, i, inc, x, y, dx, dy, plot, plotx, ploty;

   dx = point[2]-point[0];
   dy = point[3]-point[1];
   ix = abs(dx)+1;
   iy = abs(dy)+1;
   inc = max(ix, iy);

   plotx = point[0];
   ploty = point[1];
   x =y = 0;

   if(new_flag == ON) {
      array[element*2] = plotx;
      array[element*2+1] =  ploty;
		element++;
   }

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
			if(array[(element-1)*2+1] != ploty) {
				if(element == 1) {goto dol;}
				else if(((array[(element-1)*2+1]-ploty) + (array[(element-2)*2+1]-array[(element-1)*2+1])) == 0) {
					array[element*2]   = array[(element-1)*2];
					array[element*2+1] = array[(element-1)*2+1];
					element = element+1;
					array[element*2] = plotx;
					array[element*2+1] = ploty;
					element = element+1;
				}
				else {
					dol:
					array[element*2]   = plotx;
					array[element*2+1] = ploty;
					element = element+1;
				}
			};
		}
   }

   if(array[(element-1)*2+1] != point[3]) {
      array[(element)*2] = point[2];
      array[(element)*2+1] = point[3];
      element = element+1;
   }
}

void PictureVectorFillPoly(PICTURE_STRUCT *pic, int num, VECTOR_POINT *point)
{
   int element = 0, sort=0, i;
   int count;
   int xy[4];
   int lastx, lasty;
   int startx, starty;
   char new_flag = OFF;
	StackShort array((MAX_POLY_BUF+4)*2);

	if(array.data == NULL)	return;

   if(num < 4)		return;

   for(i = 0; i < num; i++) {
//      gprintf(0, 100, 0, WHITE_COLOR, "%d   ", point[i].flag);
//      getch();
      switch(point[i].flag) {
			case START:	
				startx = lastx = xy[0] = point[i].x;
				starty = lasty = xy[1] = point[i].y;
				new_flag = ON;
				break;

			case LINE:     
				if(lastx == point[i].x && lasty == point[i].y)	break;
				xy[2] = point[i].x;
				xy[3] = point[i].y;

				MakePoint(xy, element, array.data, new_flag);
				new_flag = OFF;
				lastx = xy[0] = xy[2];
				lasty = xy[1] = xy[3];
				break;
			case CONNECT:
				xy[2] = startx;
				xy[3] = starty;
				//	if(lastx != point[i].x || lasty != point[i].y)
				MakePoint(xy, element, array.data, new_flag);
				new_flag = OFF;
				xy[0] = xy[2];
				xy[1] = xy[3];
				if(element%2 == 1) {
					array.data[element*2] =   array.data[element*2-2];
					array.data[element*2+1] = array.data[element*2-1];
					element++;
				}
				break;
			default:	
				bell();
				break;
      }
   }

   qsort((void*) array.data, element, 4, ycomp);

   sort = 0;
   count = 1;
   for(i = 1; i < element; i++) {
      if(array.data[i*2+1] != array.data[sort*2+1]) {
			if(count > 1) {
				qsort( (void*)(&array.data[sort*2]), count, 4, xcomp);
			}
			sort = i;
			count = 1;
      }
      else {
			count ++;
      }
   }

   if(count > 1) {
      qsort( (void*)(&array.data[sort*2]), count, 4, xcomp);
   }

   for (i = 0; i < element; i +=2) {
		PictureBar(pic, array.data[i*2], array.data[(i+1)*2+1], array.data[(i+1)*2], array.data[(i+1)*2+1]);
      //gbar(array.data[i*2], array.data[(i+1)*2+1], array.data[(i+1)*2], array.data[(i+1)*2+1]);
   }

	if(!pic->bFillOutLine)	return;

   for(i = 0; i < num; i++) {
      switch(point[i].flag) {
			case START:	
				startx = lastx = point[i].x;
				starty = lasty = point[i].y;
				break;
			case LINE:
				PictureLine(pic, lastx, lasty, point[i].x, point[i].y);
				lastx = point[i].x;
				lasty = point[i].y;
			case CONNECT:  //if(startx == lastx && starty == lasty)	break;
				PictureLine(pic, lastx, lasty, point[i].x, point[i].y);
				lastx = point[i].x;
				lasty = point[i].y;
				break;
      }
   }
}

class classGetPoint {
		int x1, y1, x2, y2, x3, y3, x4, y4;
		int Height, Width;
	public:
		classGetPoint(int xx1, int yy1, int xx2, int yy2, int xx3, int yy3, int xx4, int yy4, int width, int height);
		int GetX(int posx, int posy);
		int GetY(int posx, int posy);
};

classGetPoint :: classGetPoint(int xx1, int yy1, int xx2, int yy2, int xx3, int yy3, int xx4, int yy4, int width, int height)
{
	x1 = xx1;
	y1 = yy1;
	x2 = xx2;
	y2 = yy2;
	x3 = xx3;
	y3 = yy3;
	x4 = xx4;
	y4 = yy4;
	Width = width;
	Height = height;
}

int classGetPoint :: GetX(int posx, int posy)
{
   double c1, c2, c3;

   c1 = (double)(x3-x1) / Height * (double)posy + (double)x1;
   c2 = (double)(x4-x2) / Height * (double)posy + (double)x2;

   c3 = c1+ (c2-c1)/Width* (double)posx;
   if( c3-(int)c3 > 0.5)	return( (int)c3+1);
   else			return( (int)c3 );
}

int classGetPoint :: GetY(int posx, int posy)
{
   double c1, c2, c3;
   c1 = (float)(y2-y1) / Width * (float)posx + (float)y1;
   c2 = (float)(y4-y3) / Width * (float)posx + (float)y3;

   c3 = c1+ (c2-c1)/Height* (float)posy;
   if( c3-(int)c3 > 0.5)	return( (int)c3 +1);
   else			return( (int)c3 );
}

double c(int n, int i);
void   bezierFcn (double *x, double *y, double u, double coeff[], int n, int *p);

void BezierFillVectorPoint(void *p, int npts, int segments, VECTOR_POINT *seg, int &seg_count)
{
   register int i;
   double u, x, y;
   double coeff[20];
   int *pp = (int*) p;

   for( i = 0; i < npts; i++)	
      coeff[i] = c(npts - 1, i);
	for(i = 0; i <= segments; i++) {
		u = (float) i / segments;
		bezierFcn(&x, &y, u, coeff, npts - 1, pp);

		x+=0.5;
		y+=0.5;
		if( i == 0) {
			;
		}
		else {
			if(seg[seg_count-1].x != (int)x || seg[seg_count-1].y != (int)y) {
				seg[seg_count].flag = LINE;
				seg[seg_count].x = (int)x;
				seg[seg_count].y = (int)y;
				seg_count++;
			}
		}
	}
}

static int PictureViewVecOneGlyph(PICTURE_STRUCT *pic, int glyph_num, ttfClass *ttf, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
{
	int i;
	int curve_point;
	int posx, posy;
	int startx, starty;
	char start_flag;
	int endpos;
	int curve[20];
	VECTOR_POINT *seg;
	int seg_count = 0;
	classGetPoint calc(x1, y1, x2, y2, x3, y3, x4, y4, ttf->GetWidth(glyph_num), ttf->GetHeight());

	seg = new VECTOR_POINT[MAX_VECTOR_POINT];

	if(seg == NULL)	return 0;

	i = 0;
	curve_point = 0;

	startx = posx = ttf->xCoordinates[0];
	starty = posy = ttf->yCoordinates[0];
	start_flag = ON;
	endpos = 0;
	//MoveTo(hdc, GetPointX(x1,x2,x3,x4,posx,posy), GetPointY(y1,y2,y3,y4,posx,posy));
	//PutPoint(hdc, posx/config.optic, posy/config.optic);
	seg[seg_count].flag = START;
	seg[seg_count].x = curve[0] = calc.GetX(posx,posy);
	seg[seg_count].y = curve[1] = calc.GetY(posx,posy);
	seg_count++;

	curve_point = 1;

	for(i = 1; i < ttf->numCoordinates; i++) {
		posx = ttf->xCoordinates[i];
		posy = ttf->yCoordinates[i];

		if(start_flag == OFF) {
			startx = posx;
			starty = posy;
			start_flag = ON;
			//MoveTo(hdc, GetPointX(x1,x2,x3,x4,posx,posy), GetPointY(y1,y2,y3,y4,posx,posy));
			//MoveTo(hdc, (startx)/config.optic, (starty)/config.optic);
			//PutPoint(hdc, (startx)/config.optic, (starty)/config.optic);

			seg[seg_count].flag = START;
			seg[seg_count].x = curve[0] = calc.GetX(posx,posy);
			seg[seg_count].y = curve[1] = calc.GetY(posx,posy);
			seg_count++;
			curve_point = 1;
		}
		else {
			//PutPoint(hdc, (posx)/config.optic, (posy)/config.optic);

			if((ttf->flags[i] & 1) == 0) {
				if(curve_point < 10) {
					curve[curve_point*2+0] = calc.GetX(posx,posy);
					curve[curve_point*2+1] = calc.GetY(posx,posy);
					curve_point ++;
				}
			}
			else {
				if(curve_point > 1) {
					curve[curve_point*2+0] = calc.GetX(posx,posy);
					curve[curve_point*2+1] = calc.GetY(posx,posy);
					curve_point ++;
					BezierFillVectorPoint(curve, curve_point, 20, seg, seg_count);
				}
				else {
					seg[seg_count].flag = LINE;
					seg[seg_count].x = calc.GetX(posx,posy);
					seg[seg_count].y = calc.GetY(posx,posy);
					seg_count++;
					//LineTo(hdc, GetPointX(x1,x2,x3,x4,posx,posy), GetPointY(y1,y2,y3,y4,posx,posy));
					//LineTo(hdc, (posx)/config.optic, (posy)/config.optic);
				}
				curve[0] = calc.GetX(posx,posy);
				curve[1] = calc.GetY(posx,posy);
				curve_point = 1;
			}
		}

		if(i == ttf->endPtsOfContours[endpos]) {
			endpos++;

			if(curve_point > 1) {
				curve[curve_point*2+0] = calc.GetX(startx,starty);
				curve[curve_point*2+1] = calc.GetY(startx,starty);
				curve_point ++;
				BezierFillVectorPoint(curve, curve_point, 20, seg, seg_count);
			}
			else {
				seg[seg_count].flag = LINE;
				seg[seg_count].x = calc.GetX(startx,starty);
				seg[seg_count].y = calc.GetY(startx,starty);
				seg_count++;
			}

			seg[seg_count].flag = CONNECT;
			seg[seg_count].x = calc.GetX(startx,starty);
			seg[seg_count].y = calc.GetY(startx,starty);
			seg_count++;

			//MoveTo(hdc, (startx)/config.optic, (starty)/config.optic);
			//PutPoint(hdc, (startx)/config.optic, (starty)/config.optic);
			start_flag = OFF;
		}
	}

	//LineTo(hdc, (startx)/config.optic, (starty)/config.optic);

	PictureVectorFillPoly(pic, seg_count, seg);

	delete seg;

	return 1;
}

void PictureViewVec(HDC hdc, PICTURE_STRUCT *pic, 
						  int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4,
						  WORD glyph_num, VECTOR_FORM *form, ttfClass *ttf)
{
	int i;
	int glyph_hap;

	glyph_hap = ttf->GetData(glyph_num);

	// 원래의 Pattern을 저장해 둔다.
	int  nSaveFillStyle = pic->nFillStyle;			// 채우는 방법, 0-pattern, 1-gradation, 2-bitmap
	char bSaveFillOutLine = pic->bFillOutLine;	// 채울때 테두리선의 표시 여부
	COLORREF lSaveColorLine = pic->lColorLine;	// 선색상
	FILL_PATTERN_STRUCT 	 	saveFillPat;
	int  poly[10];

	memcpy(&saveFillPat, &pic->fillPat, sizeof(FILL_PATTERN_STRUCT));

	if(form->writeform == 1) {	// 반전 글자.
		PictureSetFillOutLine(pic, OFF);
		PictureSetLineThick(pic, 1);
		PictureSetColor(pic, pic->fillPat.lColorL);

		poly[0] = x1;
		poly[1] = y1;
		poly[2] = x2;
		poly[3] = y2;
		poly[4] = x4;
		poly[5] = y4;
		poly[6] = x3;
		poly[7] = y3;
		PictureFillPoly(pic, 4, poly);

		PictureSetFillStyle(pic, 0);
		PictureSetFillOutLine(pic, ON);
		PictureSetFillPattern(pic, 0, lSaveColorLine);
		PictureSetColor(pic, lSaveColorLine);
	}

	if(glyph_hap == 0) {
//		strcpy(msg, "ttf.GetData return 0");
//		TextOut(hdc, 0, 0, msg, strlen(msg));
		goto out;
		//return;
	}


	if(glyph_hap == 1) 	PictureViewVecOneGlyph(pic, glyph_num, ttf, x1, y1, x2, y2, x3, y3, x4, y4);
	else {
		for(i = 0; i < glyph_hap; i++) {
			if(ttf->GetData(ttf->compositeGlyphIndex[i]) != 1) 	continue;
			PictureViewVecOneGlyph(pic, glyph_num, ttf, x1, y1, x2, y2, x3, y3, x4, y4);
		}
	}

out:	
	// 원래의 Pattern으로 복귀한다.
	PictureSetFillStyle(pic, nSaveFillStyle);
	PictureSetFillOutLine(pic, bSaveFillOutLine);
	memcpy(&pic->fillPat, &saveFillPat, sizeof(FILL_PATTERN_STRUCT));
	PictureSetColor(pic, lSaveColorLine);
}

DWORD GetTtfTotalWidth(ttfClass *ttf, char *s)
{
	int i;
	DWORD total_width = 0L;
	int strhap = strlen(s);
	WORD glyph_num;
	WORD font_number;

	for( i=0; i< strhap; i++ ) {
		if((BYTE)s[i] > 127)	{ 
			font_number = (BYTE)s[i]*256u + (BYTE)s[i+1];
			i++;
		}
		else									
			font_number = s[i];

		glyph_num = ttf->GetGlyphPosition(font_number);

		total_width += ttf->GetWidth(glyph_num);
	}

	return total_width;
}

void PictureVectorPoly(HDC hdc, PICTURE_STRUCT *pic, int px1, int py1, int px2, int py2, int px3, int py3, int px4, int py4, char *s, VECTOR_FORM *form) 
{
   int i;
   //int point[10];
	ttfClass ttf;

   int x1, x2, x3, x4;
   int y1, y2, y3, y4;
   int strhap = strlen(s);
	WORD font_number;
	WORD glyph_num;
	DWORD total_width;
	int width;
	int curr_pos;

	if(!ttf.SetHDC(hdc)) {
//		strcpy(msg, "ttf.SetHDC return 0");
//		TextOut(hdc, 0, 0, msg, strlen(msg));
		return;
	}

	total_width = GetTtfTotalWidth(&ttf, s);
	curr_pos = 0;

   for( i=0; i< strhap; i++ ) {
		if((BYTE)s[i] > 127)	{
			font_number = ((BYTE)s[i])*256u + (BYTE)s[i+1];
			i++;
		}
		else									
			font_number = s[i];

		glyph_num = ttf.GetGlyphPosition(font_number);
		width = ttf.GetWidth(glyph_num);

		x1 = (int)((long double)(px2-px1)*(curr_pos)/total_width + px1);
	   x2 = (int)((long double)(px2-px1)*(curr_pos+width)/total_width + px1);
	   x3 = (int)((long double)(px4-px3)*(curr_pos)/total_width + px3);
	   x4 = (int)((long double)(px4-px3)*(curr_pos+width)/total_width + px3);

	   y1 = (int)((long double)(py2-py1)*(curr_pos)/total_width + py1);
	   y2 = (int)((long double)(py2-py1)*(curr_pos+width)/total_width + py1);
	   y3 = (int)((long double)(py4-py3)*(curr_pos)/total_width + py3);
	   y4 = (int)((long double)(py4-py3)*(curr_pos+width)/total_width + py3);

	   PictureViewVec(hdc, pic, x1, y1, x2, y2, x3, y3, x4, y4, glyph_num, form, &ttf);
		curr_pos += width;
	}
}

int gputvangleplus(int angledir, int sangle, int anglehap, int strhap, int strpos)
{
   if(angledir == 0)
      return ((int) (sangle - (long)anglehap * strpos / strhap));
   else
      return ((int) (sangle + (long)anglehap * strpos / strhap));
}


void PictureVectorCircle(HDC hdc, PICTURE_STRUCT *pic, int cx, int cy, int rx1, int ry1, int rx2, int ry2,
	    int startangle, int endangle, int angledirection, int rightform, char *s, VECTOR_FORM *form)
{
   register int i;
   WORD font_number;
	WORD glyph_num;
	ttfClass ttf;
	DWORD total_width;
	int width;

   int x1, x2, x3, x4;
   int y1, y2, y3, y4;

   int strhap = strlen(s);
   register int anglehap = 0, anglepos;
   int fit_size;
	int curr_pos;
	
	if(!ttf.SetHDC(hdc)) {
//		strcpy(msg, "ttf.SetHDC return 0");
//		TextOut(hdc, 0, 0, msg, strlen(msg));
		return;
	}

	total_width = GetTtfTotalWidth(&ttf, s);

   if(strhap == 0)	return;

  	// get anglehap 
   if(angledirection == 1) {                // non-watch direction 
      if(endangle <= startangle)
			endangle += 360;
      anglehap = endangle-startangle;
   }
   else {                                   // watch direction 
      if(endangle >= startangle)
			endangle -= 360;
      anglehap = startangle-endangle;
   }

   rx1 = rx1 ? rx1 : rx1+1;                 // protect divide by 0 
   ry1 = ry1 ? ry1 : ry1+1;
   rx2 = rx2 ? rx2 : rx2+1;
   ry2 = ry2 ? ry2 : ry2+1;

	curr_pos = 0;

   for( i=0; i < strhap; i++ ) {
		if((BYTE)s[i] > 127)	{
			font_number = ((BYTE)s[i])*256u + (BYTE)s[i+1];
			i++;
		}
		else									
			font_number = s[i];

		glyph_num = ttf.GetGlyphPosition(font_number);
		width = ttf.GetWidth(glyph_num);

		anglepos = gputvangleplus(angledirection, startangle, anglehap, total_width, curr_pos);
	   MathGetEllipsePoint(cx,cy,(float)rx1,(float)ry1, (float)anglepos, &x1, &y1);
	   MathGetEllipsePoint(cx,cy,(float)rx2,(float)ry2, (float)anglepos, &x3, &y3);

	   anglepos = gputvangleplus(angledirection, startangle, anglehap, total_width, curr_pos+width);
	   MathGetEllipsePoint(cx,cy,(float)rx1,(float)ry1, (float)anglepos, &x2, &y2);
	   MathGetEllipsePoint(cx,cy,(float)rx2,(float)ry2, (float)anglepos, &x4, &y4);

	   fit_size = abs(rx2-rx1);

	   if(rightform == 1)
			PictureViewVec(hdc, pic, x1-fit_size, y1-fit_size, x1+fit_size, y1-fit_size,
		       x1-fit_size, y1+fit_size, x1+fit_size, y1+fit_size, glyph_num, form, &ttf);
		else
			PictureViewVec(hdc, pic, x1,y1, x2, y2, x3, y3, x4, y4, glyph_num, form, &ttf);

		curr_pos += width;
	}
}


//double c(int n, int i);
//void   bezierFcn (double *x, double *y, double u, double coeff[], int n, int *p);

void BezierGetxy(void *p, int npts, int segments, int pos, int& getx, int& gety)
{
   register int i;
   double u, x, y;
   double coeff[20];
   int *pp = (int*) p;


   if(npts <= 1) {
      getx = pp[0];
      gety = pp[1];
      return;
   }

   if(pos == 0) {
      getx = pp[0];
      gety = pp[1];
      return;
   }

   if(pos >= npts-1) {
      getx = pp[pos*2];
      gety = pp[pos*2+1];
   }

   for( i = 0; i < npts; i++)
      coeff[i] = c(npts - 1, i);
   for(i = 0; i <= segments; i++) {
      u = (float) i / segments;
      bezierFcn(&x, &y, u, coeff, npts - 1, pp);

      x+=0.5;
      y+=0.5;

      if(i == pos) {
			getx = (int)x;
			gety = (int)y;
      }
   }
}


void PictureVectorBezier(HDC hdc, PICTURE_STRUCT *pic, int *p1, int *p2, char *s, VECTOR_FORM *form) 
{
   int i;
   //int point[10];
	ttfClass ttf;

   int x1, y1, x2, y2, x3, y3, x4, y4;
	int strhap = strlen(s);
	WORD font_number;
	WORD glyph_num;
	DWORD total_width;
	int width;
	int curr_pos;
	
	if(!ttf.SetHDC(hdc)) {
//		strcpy(msg, "ttf.SetHDC return 0");
//		TextOut(hdc, 0, 0, msg, strlen(msg));
		return;
	}

	total_width = GetTtfTotalWidth(&ttf, s);
	curr_pos = 0;

	BezierGetxy(p1, 4, strhap, 0, x1, y1);
   BezierGetxy(p2, 4, strhap, 0, x3, y3);

   for( i=0; i < strhap; i++ ) {
		if((BYTE)s[i] > 127)	{
			font_number = ((BYTE)s[i])*256u + (BYTE)s[i+1];
			i++;
		}
		else									
			font_number = s[i];

		glyph_num = ttf.GetGlyphPosition(font_number);
		width = ttf.GetWidth(glyph_num);

		BezierGetxy(p1, 4, strhap, i+1, x2, y2);
		BezierGetxy(p2, 4, strhap, i+1, x4, y4);

	   PictureViewVec(hdc, pic, x1, y1, x2, y2, x3, y3, x4, y4, glyph_num, form, &ttf);
		curr_pos += width;

		x1 = x2;
      y1 = y2;
      x3 = x4;
      y3 = y4;
	}
}

