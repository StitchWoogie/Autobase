#include "stdafx.h"
#include <stdlib.h>

#include <glib.h>

//-------------------------------------------------------------------------------
//	주어진 DAC에서 RGB에 가장 접근한 값을 읽어온다.
//-------------------------------------------------------------------------------

int SeekFitRGB(BYTE *dac, int r, int g, int b, int colorhap)
{
	int old_color = 0;
	int old_gab = 255;
	int i;
	int gabr, gabg, gabb;

	for(i = 0; i < colorhap*3; i+=3) {
		/*
		if(dac[i+0] > r)	
			gabr = dac[i+0]-r;
		else
			gabr = r-dac[i+0];

		if(dac[i+1] > g)	
			gabg = dac[i+1]-g;
		else
			gabg = g-dac[i+1];

		if(dac[i+2] > b)	
			gabb = dac[i+2]-b;
		else
			gabb = b-dac[i+2];
		*/
			
		gabr = abs(dac[i+0]-r);
		gabg = abs(dac[i+1]-g);
		gabb = abs(dac[i+2]-b);
		
		if(gabg > gabr)	gabr = gabg;
		if(gabb > gabr)	gabr = gabb;

		if(gabr == 0)	return i/3;	// 똑같은 R,G,B를 찾았다.

		if(gabr < old_gab) {
			old_gab = gabr;
			old_color = i/3;
		}
	}
	return old_color;
	
	/*
	// 옛날에 만든 찾기 함수.
	int i, pos, gab = 0;
   int r1,r2,g1,g2,b1,b2;
   char flag = 0;

   if(r == 0 && g == 0 && b == 0)	flag = 1;

   while(1) {
      r1 = ( (int)r-gab) <   0 ?   0 : (int)r-gab;
      r2 = ( (int)r+gab) > 255 ? 255 : (int)r+gab;

      g1 = ( (int)g-gab) <   0 ?   0 : (int)g-gab;
      g2 = ( (int)g+gab) > 255 ? 255 : (int)g+gab;

      b1 = ( (int)b-gab) <   0 ?   0 : (int)b-gab;
      b2 = ( (int)b+gab) > 255 ? 255 : (int)b+gab;

      if(flag == 1) {
			for(i = 0; i < colorhap; i++) {
				pos = i*3;
				if(dac[pos]   >= r1 && dac[pos]   <= r2 &&
					dac[pos+1] >= g1 && dac[pos+1] <= g2 &&
					dac[pos+2] >= b1 && dac[pos+2] <= b2 )

					return i;
			}
      }
      else {
			for(i = colorhap-1; i >= 0; i--) {
				pos = i*3;
				if(dac[pos]   >= r1 && dac[pos]   <= r2 &&
					dac[pos+1] >= g1 && dac[pos+1] <= g2 &&
					dac[pos+2] >= b1 && dac[pos+2] <= b2 )

					return i;
			}
      }
      gab++;
   }
	*/
}




