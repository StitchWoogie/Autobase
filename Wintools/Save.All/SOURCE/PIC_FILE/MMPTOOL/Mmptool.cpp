#include "stdafx.h"
#include <string.h>

#include "mmptool.h"




mmpClass :: mmpClass() : pictureFileClass()
{
}

mmpClass :: ~mmpClass()
{

}

int mmpClass :: ReadOpen(char *filename)
{
   MMPHEADER head;

   if(dac == NULL)	return 0;

   if(filename != NULL) {
      in = fopen(filename, "rb");
      if(in == NULL)	return 0;
   }

   fread(&head, 1, sizeof (MMPHEADER), in);

   if(strncmp(head.mhIDString, MMP_HDR_STRING_12, 6) == 0) {	// "YUV12C" 4:1:1:
      yuvForm = YUV_411;
   }
   else {
      yuvForm = YUV_422;
   }

   nWidth = head.mhWidth;
   nHeight = head.mhHeight;
   nBitsPerPixel = COLOR_1600;
   nBytesPerLine = (nWidth)*3;

   return 1;
}

//----------------------------------------------------------------------------
//	MMP Get One line to 1600 true color buffer
//----------------------------------------------------------------------------

int  b8( WORD y, BYTE u );
int  r8( WORD y, BYTE v );
int  g8( WORD y, BYTE u, BYTE v );

int mmpClass :: GetOneLine411(BYTE *buf, int /*mamx*/)
{
   int i, j;
   BYTE U, V;
   WORD word3[4];
   BYTE Y[4];

   for(i = 0; i < nWidth; i+=4) {
      fread(&word3[0], 1, 6, in);

      Y[0] = (word3[0] >> 4) & 0xFE;
      Y[1] = (word3[1] >> 8) & 0xFE;
      Y[2] = ((word3[1] << 4) & 0xF0) | ((word3[2] >> 12) & 0x0E);
      Y[3] = word3[2] & 0xFE;

//?  旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴?컴컴컴컴컴컴컴??
//?Word 0?15  14  13 12  11  10   9   8   7   6   5  4  3  2  1  0  ??
//?  ?U6  U5  V6 V5 Y06  Y05 Y04 Y03 Y02 Y01 Y00 XX U4 U3 V4 V3 ??
//?  읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
//?  旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
//?Word 1?15  14  13  12  11  10   9  8  7  6  5  4   3   2   1   0 ??
//?  ?Y16 Y15 Y14 Y13 Y12 Y11 Y10 XX U2 U1 V2 V1 Y26 Y25 Y24 Y23??
//?  읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
//?  旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
//?Word 2?15  14  13  12 11 10 9  8   7   6   5   4   3   2   1  0  ??
//?  ?Y22 Y21 Y20 XX U0 XX V0 XX Y36 Y35 Y34 Y33 Y32 Y31 Y30 XX ??
//?  읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
      U = ((word3[0] >> 8) & 0xC0) | ((word3[0] << 2) & 0x30) | ((word3[1] >> 4) & 0x0C) | ((word3[2] >> 10) & 0x02);
      V = ((word3[0] >> 6) & 0xC0) | ((word3[0] << 4) & 0x30) | ((word3[1] >> 2) & 0x0C) | ((word3[2] >> 8) & 0x02);

      for(j = 0; j < 4; j++) {
			buf[(i+j)*3]   = b8(Y[j], U);
			buf[(i+j)*3+1] = g8(Y[j], U, V);
			buf[(i+j)*3+2] = r8(Y[j], V);
      }
   }
   return 1;
}

int mmpClass :: GetOneLine422(BYTE *buf, int /*mamx*/)
{
   int i, j;
   BYTE U, V;
   BYTE imsi[4];
//   BYTE Y[4];

   for(i = 0; i < nWidth; i+=2) {
      fread(&imsi, 1, 4, in);
      U = imsi[1]-0x80;
      V = imsi[3]-0x80;

      for(j = 0; j < 2; j++) {
			buf[(i+j)*3]   = b8(imsi[j*2], U);
			buf[(i+j)*3+1] = g8(imsi[j*2], U, V);
			buf[(i+j)*3+2] = r8(imsi[j*2], V);
      }
   }
   return 1;
}

int mmpClass :: GetOneLine(BYTE *buf, int maxx)
{
   switch(yuvForm) {
      case YUV_411:	return(GetOneLine411(buf, maxx));
      case YUV_422:	return(GetOneLine422(buf, maxx));
   }

   return 0;
}

int  b8( WORD y, BYTE u )
{
	return 0;   
}

int  r8( WORD y, BYTE v )
{
	return 0;
}

int  g8( WORD y, BYTE u, BYTE v )
{
	return 0;
}


/**********
;旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴커
;?								?
;?YUV data is stored in 4:1:1 YUV format.  For each pixel there is	?
;?one 7 bit unsigned luminance value, Y.  The two chroma components, 	?
;?U (B-Y) and V (R-Y), are 7 bit signed values.  Since chroma is	?
;?sampled at one fourth the rate of luminance, there is one value of	?
;?each chominance component for every four pixels.  In an IBM MMotion	?
;?file these bits are distributed over three words as shown below:	?
;?								?
;?  旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴?컴컴컴컴컴컴컴??
;?Word 0 ?15  14  13 12  11  10   9   8   7   6   5  4  3  2  1  0  ??
;?  ?U6  U5  V6 V5 Y06  Y05 Y04 Y03 Y02 Y01 Y00 XX U4 U3 V4 V3 ??
;?  읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
;?  旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
;?Word 1 ?15  14  13  12  11  10   9  8  7  6  5  4   3   2   1   0 ??
;?  ?Y16 Y15 Y14 Y13 Y12 Y11 Y10 XX U2 U1 V2 V1 Y26 Y25 Y24 Y23??
;?  읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
;?  旼컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
;?Word 2 ?15  14  13  12 11 10 9  8   7   6   5   4   3   2   1  0  ??
;?  ?Y22 Y21 Y20 XX U0 XX V0 XX Y36 Y35 Y34 Y33 Y32 Y31 Y30 XX ??
;?  읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴??
;?								?
;?U and V are also scaled by 127/179 and 127/226, respectively, to	?
;?fit in 8 bits, and the LSB must be truncated to fit in 7 bits.	?
;?								?
;?Y = 0.299*R + 0.587*G + 0.114*B					?
;?								?
;?V = R-Y = R - 0.299*R - 0.587*G - 0.114*B			?
;?V = 0.701*R - 0.587*G - 0.114*B					?
;?Vmax = 0.701*255 - 0.587*0 - 0.114*0 = 178.76			?
;?Vmin = 0.701*0 - 0.587*255 - 0.114*255 = -178.76		?
;?-> V is scaled by 127/179 to fit in 8 bits.			?
;?								?
;?U = B-Y = -0.299*R - 0.587*G + B - 0.114*B			?
;?U = -0.299*R - 0.587*G + 0.866*B				?
;?Umax = -0.299*0 - 0.587*0 + 0.866*255 = 225.93			?
;?Umin = -0.299*255 - 0.587*255 + 0.866*0 = -225.93		?
;?-> U is scaled by 127/226 to fit in 8 bits.			?
;?								?
;?To convert from YUV to RGB, the U and V values must first be 	?
;?unscaled by multiplying by 226/127 and 179/127.  The following	?
;?formulas are used in the conversion to RGB:				?
;?								?
;?R = V + Y	B = U + Y	 				?
;?G = 1.706*Y - 0.509*R - 0.194*B					?
;읕컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴컴켸
******************/


/* ASM code of r8, g8, b8
.MODEL large

ifdef ??version 	;for Turbo Assembler
	MASM51
	QUIRKS
endif
 
.286P

.code

public C b8

b8 PROC y, u:BYTE
		push bp
		mov bp, sp
		mov	al, u
		cbw					;//AX = scaled U (sign extended)
		mov	dx,456			;//(456/256 = 226/127)
		imul	dx
		mov	al,ah
		mov	ah,dl
		add ax, y
		jge  @F
		xor ax, ax
		pop bp
		ret

	@@:
		cmp ax, 255
		jg  @F
		pop bp
		ret
	@@:
		mov ax, 255
		pop bp
		ret
							;//AX = unscaled U
ENDP

public C r8
r8 PROC y, v:BYTE
	push bp
	mov bp, sp
		mov	al, v
		cbw					;//AX = scaled V (sign extended)
		mov	dx,361			;//(361/256 = 179/127)
		imul	dx
		mov	al, ah
		mov	ah, dl
		add ax, y
		jge  @F
		xor ax, ax
		pop bp
		ret

	@@:
		cmp ax, 255
		jg  @F
		pop bp
		ret

	@@:
		mov ax, 255
		pop bp
		ret						;//AX = unscaled V
ENDP
;g8( y, u, v ) = min(255,max(0,(436L*y-130L*r8(y,v)-50L*b8(y,u))>>8))
;green=1.706L*y-.509L*r8(y,v)-.194L*b8(y,u)

public C g8

g8 PROC y, u:BYTE, v:BYTE
	push bp
	mov bp, sp

		mov dx, y
		mov ax, 218
		mul dx
		shr ax, 7
		mov cx, ax

		push word ptr v
		push y
		call r8
		add sp, 4

		mov  dx, 65
		mul  dx
		shr ax, 7
		mov bx, ax

		push word ptr u
		push y
		call b8
		add sp, 4

		mov dx, 25
		mul dx
		shr ax, 7

		neg ax
		add ax, cx
		sub ax, bx

		cmp ax, 0
		jge @F
		xor ax, ax
		pop bp
		ret

	@@:
		cmp ax, 255
		jg  @F
		pop bp
		ret

	@@:
		mov ax, 255
		pop bp
		ret
endp

end

*/