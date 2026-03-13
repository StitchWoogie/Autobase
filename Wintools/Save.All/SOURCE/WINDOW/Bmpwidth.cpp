#include "stdafx.h"
#include <glib.h>
#include <tools.h>

int BmpWidthToByte(int width, int bits)
{

	int imsi;

	if(bits == 1) {
		imsi = (int)((((width+7)/8)+3)/4*4);
	}
	else if(bits == 4) {
		imsi = (int)(( ((width+1)/2)+3)/4*4);
	}
	else if(bits == 8) {
		imsi = ((width+3)/4)*4;
	}
	else if(bits == 16) {
		imsi = ((width*2+3)/4)*4;
	}
	else if(bits == 32) {
		imsi = width*4;
	}
	else {
		imsi = (int)( (width*3+3)/4*4);
	}
	/*
	switch(bits)
	{
		case 1:
			imsi = (int)((((width+7)/8)+3)/4*4);
			break;
		case 4:
			imsi = (int)(( ((width+1)/2)+3)/4*4);
			break;
		case 8:
			imsi = ((width+3)/4)*4;
			break;
		case 24:
			imsi = (int)( (width*3+3)/4*4);
			break;
		default:
			bell();
			break;
	}
	*/

	return imsi;
}

