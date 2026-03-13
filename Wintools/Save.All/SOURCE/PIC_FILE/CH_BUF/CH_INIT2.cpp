// print convert init

#include <math.h>
#include <iostream.h>
#include <mem.h>

#include <tools.h>
#include <glib.h>

#include <ch_buf.h>

convertBuf :: convertBuf()
{
   table = NULL;	//
   gammaVector = NULL;
   err1 =  NULL;

   cerr1 = NULL;
   yerr1 = NULL;
   merr1 = NULL;
   kerr1 = NULL;

   threshold = 127;
}

convertBuf :: ~convertBuf()
{
   if(table != NULL)	delete table;
   if(gammaVector != NULL)	delete gammaVector;
   if(err1 != NULL)	delete err1;

   if(cerr1 != NULL)	delete cerr1;
   if(yerr1 != NULL)	delete yerr1;
   if(merr1 != NULL)	delete merr1;
   if(kerr1 != NULL)	delete kerr1;
}

//----------------------------------------------------------------------------
//	0.0 < gammaVal <= 10.0
//	0 < numSteps <= 256
//----------------------------------------------------------------------------

void MakeGammaTable(BYTE *table, double gammaVal, int numSteps)
{
   int i;
   double numSteps1;

   numSteps1 = (double)(numSteps-1);

   for(i = 0; i < numSteps; i++) {
      table[i] = (BYTE) (numSteps1*pow((double)i/numSteps1, gammaVal) + (double)0.5);
   }
}

int convertBuf :: Init(int tBit, int sBit, int xsize, int Convertform, BYTE *dac, float gamma)
{
   int i, j;

   table = new BYTE[768];
   if(table == NULL)	return 0;
   gammaVector = new BYTE[256];
   if(gammaVector == NULL)	return 0;

   //MakeGammaTable(gammaVector, 0.35, 256);
   MakeGammaTable(gammaVector, gamma, 256);

   if(Convertform == 1) {	// error diffusion dither
      switch(tBit) {
	 case COLOR_PRINT_MONO:
	    err1 = new int[xsize*2];
	    if(err1 == NULL)	return 0;
	    err2 = err1 + xsize;
	    memset(err1, 0, xsize*2*sizeof(int));
	    break;
	 case COLOR_PRINT_CYMK_ALL:
	 case COLOR_PRINT_CYMK_LOSS:
	 case COLOR_PRINT_CYM_ALL:
	 case COLOR_PRINT_CYM_LOSS:
	    cerr1 = new int[xsize*2];
	    yerr1 = new int[xsize*2];
	    merr1 = new int[xsize*2];
	    kerr1 = new int[xsize*2];

	    if(cerr1 == NULL)	return 0;
	    if(yerr1 == NULL)	return 0;
	    if(merr1 == NULL)	return 0;
	    if(kerr1 == NULL)	return 0;

	    cerr2 = cerr1 + xsize;
	    yerr2 = yerr1 + xsize;
	    merr2 = merr1 + xsize;
	    kerr2 = kerr1 + xsize;

	    memset(cerr1, 0, xsize*2*sizeof(int));
	    memset(yerr1, 0, xsize*2*sizeof(int));
	    memset(merr1, 0, xsize*2*sizeof(int));
	    memset(kerr1, 0, xsize*2*sizeof(int));
	    break;
      }
   }

   sourceBit = sBit;
   targetBit = tBit;
   sizeX = xsize;
   currY = 0;      		// 현재의 라인을 Y 로 세팅한다.
   ditherMethod = Convertform;	// convert form

   switch(sBit) {
      case COLOR_1600:	byteX = xsize*3;		break;
      case COLOR_65536:
      case COLOR_32768: byteX = xsize*2;		break;
      case COLOR_256:	byteX = xsize;		break;
      case COLOR_16:	byteX = (xsize+7)/8*4;	break;
      case COLOR_2:	byteX = (xsize+7)/8;		break;
   }

   if(ditherMethod == 0) {	// pattern dither
      switch(tBit) {
	 case COLOR_PRINT_MONO:
	    switch(sBit) {
	       case COLOR_256:     for(i = 0; i < 768; i+=3) {
				      j = (gammaVector[dac[i]]>>2)*30 + (gammaVector[dac[i+1]]>>2)*59 + (gammaVector[dac[i+2]]>>2)*11;
				      if(j > 6300)	j = 6300;
				      table[i/3] = j/100;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i+=3) {
				      j = (gammaVector[dac[i]]>>2)*30 + (gammaVector[dac[i+1]]>>2)*59 + (gammaVector[dac[i+2]]>>2)*11;
				      if(j > 6300)	j = 6300;
				      table[i/3] = j/100;
				   }
				   break;
	    }
	    break;
	 case COLOR_PRINT_RGB:
	    switch(sBit) {
	       case COLOR_256:     for(i = 0; i < 768; i++) {
				      table[i] = (dac[i]>>2);
				      if(table[i] >= 63)	table[i] = 63;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i++) {
				      table[i] = dac[i]>>2;
				   }
				   break;
	    }
	    break;
	 case COLOR_PRINT_CYM_ALL:
	 case COLOR_PRINT_CYM_LOSS:
	    switch(sBit) {
	       case COLOR_256:     //*******************************************
				   // C = Green + Blue
				   // Y = Red   + Green
				   // M = Red   + Blue
				   //*******************************************
				   for(i = 0; i < 256; i++) {
				      table[i*3] = dac[i*3] >> 2;
				      table[i*3+1] = dac[i*3+1] >> 2;
				      table[i*3+2] = dac[i*3+2] >> 2;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i++) {
				      table[i] = dac[i]>>2;
				   }
				   break;
	    }
	    break;
	 case COLOR_PRINT_CYMK_ALL:
	    switch(sBit) {
	       case COLOR_256:     //*******************************************
				   // C = Green + Blue
				   // Y = Red   + Green
				   // M = Red   + Blue
				   //*******************************************
				   for(i = 0; i < 768; i++) {
				      table[i] = gammaVector[dac[i]] >> 2;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i++) {
				      table[i] = gammaVector[dac[i]] >> 2;
				   }
				   break;
	    }
	    break;
	 case COLOR_PRINT_CYMK_LOSS:
	    switch(sBit) {
	       case COLOR_256:     //*******************************************
				   // C = Green + Blue
				   // Y = Red   + Green
				   // M = Red   + Blue
				   //*******************************************
				   for(i = 0; i < 256; i++) {
				      table[i*3] = dac[i*3] >> 3;
				      table[i*3+1] = dac[i*3+1] >> 3;
				      table[i*3+2] = dac[i*3+2] >> 3;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i++) {
				      table[i] = dac[i] >> 3;
				   }
				   break;
	    }
	    break;
      }
   }
   else {	// diffusion dither
      switch(tBit) {
	 case COLOR_PRINT_MONO:
	    switch(sBit) {
	       case COLOR_256:     for(i = 0; i < 768; i+=3) {
				      j = (gammaVector[dac[i]])*30 + (gammaVector[dac[i+1]])*59 + (gammaVector[dac[i+2]])*11;
				      if(j > 25500)	j = 25500;
				      table[i/3] = j/100;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i+=3) {
				      j = (gammaVector[dac[i]])*30 + (gammaVector[dac[i+1]])*59 + (gammaVector[dac[i+2]])*11;
				      if(j > 25500)	j = 25500;
				      table[i/3] = j/100;
				   }
				   break;
	    }
	    break;
	 case COLOR_PRINT_RGB:
	    switch(sBit) {
	       case COLOR_256:     for(i = 0; i < 768; i++) {
				      table[i] = (dac[i]>>2);
				      if(table[i] >= 63)	table[i] = 63;
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i++) {
				      table[i] = dac[i]>>2;
				   }
				   break;
	    }
	    break;
	 case COLOR_PRINT_CYM_ALL:
	 case COLOR_PRINT_CYM_LOSS:
	 case COLOR_PRINT_CYMK_ALL:
	 case COLOR_PRINT_CYMK_LOSS:
	    switch(sBit) {
	       case COLOR_256:     for(i = 0; i < 768; i++) {
				      table[i] = gammaVector[dac[i]];
				   }
				   break;
	       case COLOR_16:      for(i = 0; i < 48; i++) {
				      table[i] = gammaVector[dac[i]];
				   }
				   break;
	    }
	    break;
      }
   }

   return 1;
}

void convertBuf :: OneLine(BYTE *tbuf, BYTE *sbuf)
{
   if(ditherMethod == 0) {	// pattern dither
      switch(targetBit) {
	 case COLOR_PRINT_MONO:
	   switch(sourceBit) {
	      case COLOR_1600:     Convert1600To2Print(tbuf, sbuf);	break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768To2Print(tbuf, sbuf);	break;
	      case COLOR_256:      Convert256To2Print(tbuf, sbuf); 	break;
	      case COLOR_16:       Convert16To2Print(tbuf, sbuf);		break;
	      case COLOR_2:        Convert2To2Print(tbuf, sbuf);		break;
	   }
	   break;
	 case COLOR_PRINT_CYM_LOSS:
	   switch(sourceBit) {
	      case COLOR_1600:     Convert1600ToCYMPrintLoss(tbuf, sbuf);	break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768ToCYMPrintLoss(tbuf, sbuf);	break;
	      case COLOR_256:      Convert256ToCYMPrintLoss(tbuf, sbuf); 	break;
	      case COLOR_16:       Convert16ToCYMPrintLoss(tbuf, sbuf);	break;
	      case COLOR_2:        //memcpy(target, source, cbByteX);
				   break;
	   }
	   break;
	 case COLOR_PRINT_CYMK_LOSS:
	   switch(sourceBit) {
	      case COLOR_1600:     Convert1600ToCYMKPrintLoss(tbuf, sbuf);	break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768ToCYMKPrintLoss(tbuf,sbuf);	break;
	      case COLOR_256:      Convert256ToCYMKPrintLoss(tbuf, sbuf);	break;
	      case COLOR_16:       Convert16ToCYMKPrintLoss(tbuf, sbuf);	break;
	      case COLOR_2:        Convert2ToCYMKPrint(tbuf, sbuf);	break;
	   }
	   break;
	 case COLOR_PRINT_CYMK_ALL:
	   switch(sourceBit) {
	      case COLOR_1600:	Convert1600ToCYMKPrint(tbuf, sbuf);	break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768ToCYMKPrint(tbuf, sbuf);	break;
	      case COLOR_256:	Convert256ToCYMKPrint(tbuf, sbuf);	break;
	      case COLOR_16:	Convert16ToCYMKPrint(tbuf, sbuf);	break;
	      case COLOR_2:        Convert2ToCYMKPrint(tbuf, sbuf);	break;
	   }
	   break;
	 default:		exiterr("Convert Buf Error: t Buf unknowned");
      }
   }
   else {	// error diffusion dither
      switch(targetBit) {
	 case COLOR_PRINT_MONO:
	   switch(sourceBit) {
	      case COLOR_1600:     Convert1600To2PrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768To2PrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_256:      Convert256To2PrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_16:       Convert16To2PrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_2:        Convert2To2Print(tbuf, sbuf);		break;
	   }
	   break;
	 case COLOR_PRINT_CYM_LOSS:
	 case COLOR_PRINT_CYM_ALL:
	   switch(sourceBit) {
	      case COLOR_1600:     Convert1600ToCYMPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768ToCYMPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_256:      Convert256ToCYMPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_16:       Convert16ToCYMPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_2:        //memcpy(target, source, cbByteX);
				   break;
	   }
	   break;
	 case COLOR_PRINT_CYMK_LOSS:
	 case COLOR_PRINT_CYMK_ALL:
	   switch(sourceBit) {
	      case COLOR_1600:     Convert1600ToCYMKPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_65536:
	      case COLOR_32768:    Convert32768ToCYMKPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_256:      Convert256ToCYMKPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_16:       Convert16ToCYMKPrintDiffusion(tbuf, sbuf);
				   break;
	      case COLOR_2:        Convert2ToCYMKPrint(tbuf, sbuf);
				   break;
	   }
	   break;
	 default:		exiterr("Convert Buf Error: t Buf unknowned");
      }
   }
}

/*
//----------------------------------------------------------------------------
//	버퍼를 바꾸기전에 한번 실행해야 한다.
//	tBit - target color
//	sBit - source color
//	xsize - 바꿀 X 픽셀 크기
//	convertform - 바꾸는 방식
//	dac - 파레트
//----------------------------------------------------------------------------

void ConvertBufPrnInit(int tBit, int sBit, int xsize, int Convertform, BYTE *dac, int *diffusionBuf)
{
   int i, j;

   cbSourceBit = sBit;
   cbTargetBit = tBit;
   cbSizeX = xsize;
   cbLineY = 0;      	// 현재의 라인을 Y 로 세팅한다.
   cbConvertFlag = Convertform;		// convert form
   cbDiffusionBuf = diffusionBuf;

   srand( xsize );

   switch(sBit) {
      case COLOR_1600:	cbByteX = xsize*3;		break;
      case COLOR_65536:
      case COLOR_32768: cbByteX = xsize*2;		break;
      case COLOR_256:	cbByteX = xsize;		break;
      case COLOR_16:	cbByteX = (xsize+7)/8*4;	break;
      case COLOR_2:	cbByteX = (xsize+7)/8;		break;
   }

   switch(tBit) {
      case COLOR_PRINT_MONO:
	 switch(sBit) {
	    case COLOR_256:     for(i = 0; i < 768; i+=3) {
				   j = (dac[i]>>2)*30 + (dac[i+1]>>2)*59 + (dac[i+2]>>2)*11;
				   j += 500;
				   if(j > 6300)	j = 6300;
				   gimsi[i/3] = j/100;
				}
				break;
	    case COLOR_16:      for(i = 0; i < 48; i+=3) {
				   j = (dac[i]>>2)*30 + (dac[i+1]>>2)*59 + (dac[i+2]>>2)*11;
				   j += 500;
				   if(j > 6300)	j = 6300;
				   gimsi[i/3] = j/100;
				}
				break;
	 }
	 break;
      case COLOR_PRINT_RGB:
	 switch(sBit) {
	    case COLOR_256:     for(i = 0; i < 768; i++) {
				   gimsi[i] = (dac[i]>>2);
				   if(gimsi[i] >= 63)	gimsi[i] = 63;
				}
				break;
	    case COLOR_16:      for(i = 0; i < 48; i++) {
				   gimsi[i] = dac[i]>>2;
				}
				break;
	 }
	 break;
      case COLOR_PRINT_CYM_ALL:
      case COLOR_PRINT_CYM_LOSS:
      case COLOR_PRINT_CYMK_ALL:
	 switch(sBit) {
	    case COLOR_256:     //*******************************************
				// C = Green + Blue
				// Y = Red   + Green
				// M = Red   + Blue
				//*******************************************
				for(i = 0; i < 256; i++) {
				   gimsi[i*3] = dac[i*3] >> 2;
				   gimsi[i*3+1] = dac[i*3+1] >> 2;
				   gimsi[i*3+2] = dac[i*3+2] >> 2;
				}
				break;
	    case COLOR_16:      for(i = 0; i < 48; i++) {
				   gimsi[i] = dac[i]>>2;
				}
				break;
	 }
	 break;
      case COLOR_PRINT_CYMK_LOSS:
	 switch(sBit) {
	    case COLOR_256:     //*******************************************
				// C = Green + Blue
				// Y = Red   + Green
				// M = Red   + Blue
				//*******************************************
				for(i = 0; i < 256; i++) {
				   gimsi[i*3] = dac[i*3] >> 3;
				   gimsi[i*3+1] = dac[i*3+1] >> 3;
				   gimsi[i*3+2] = dac[i*3+2] >> 3;
				}
				break;
	    case COLOR_16:      for(i = 0; i < 48; i++) {
				   gimsi[i] = dac[i] >> 3;
				}
				break;
	 }
	 break;
   }

   ConvertBufOneLine = ConvertBufSame;

   switch(cbTargetBit) {
      case COLOR_PRINT_MONO:
	switch(cbSourceBit) {
	   case COLOR_1600:     ConvertBufOneLine = ConvertBuf1600To2Print;
				break;
	   case COLOR_65536:
	   case COLOR_32768:    ConvertBufOneLine = ConvertBuf32768To2Print;
				if(cbConvertFlag == 1)
				   ConvertBufOneLine = ConvertBuf32768To2PrintDiffusion;
				break;
	   case COLOR_256:      ConvertBufOneLine = ConvertBuf256To2Print;
				break;
	   case COLOR_16:       ConvertBufOneLine = ConvertBuf16To2Print;
				if(cbConvertFlag == 1)
				   ConvertBufOneLine = ConvertBuf16To2PrintDiffusion;
				break;
	   case COLOR_2:        ConvertBufOneLine = ConvertBuf2To2Print;
				break;
	}
	break;
      case COLOR_PRINT_CYM_LOSS:
	switch(cbSourceBit) {
	   case COLOR_1600:     ConvertBufOneLine = ConvertBuf1600ToCYMPrintLoss;
				break;
	   case COLOR_65536:
	   case COLOR_32768:    ConvertBufOneLine = ConvertBuf32768ToCYMPrintLoss;
				break;
	   case COLOR_256:      ConvertBufOneLine = ConvertBuf256ToCYMPrintLoss;
				break;
	   case COLOR_16:       ConvertBufOneLine = ConvertBuf16ToCYMPrintLoss;
				break;
	   case COLOR_2:        //memcpy(target, source, cbByteX);
				break;
	}
	break;
      case COLOR_PRINT_CYMK_LOSS:
	switch(cbSourceBit) {

	   case COLOR_1600:     ConvertBufOneLine = ConvertBuf1600ToCYMKPrintLoss;
				break;
	   case COLOR_65536:
	   case COLOR_32768:    ConvertBufOneLine = ConvertBuf32768ToCYMKPrintLoss;
				break;
	   case COLOR_256:      ConvertBufOneLine = ConvertBuf256ToCYMKPrintLoss;
				break;
	   case COLOR_16:       ConvertBufOneLine = ConvertBuf16ToCYMKPrintLoss;
				break;
	   case COLOR_2:        ConvertBufOneLine = ConvertBuf2ToCYMKPrint;
				break;
	}
	break;
      case COLOR_PRINT_CYMK_ALL:
	switch(cbSourceBit) {
	   case COLOR_1600:	ConvertBufOneLine = ConvertBuf1600ToCYMKPrint;
				break;
	   case COLOR_65536:
	   case COLOR_32768:    ConvertBufOneLine = ConvertBuf32768ToCYMKPrint;
				break;
	   case COLOR_256:	ConvertBufOneLine = ConvertBuf256ToCYMKPrint;
				break;
	   case COLOR_16:	ConvertBufOneLine = ConvertBuf16ToCYMKPrint;
				break;
	   case COLOR_2:        ConvertBufOneLine = ConvertBuf2ToCYMKPrint;
				break;
	}
	break;
      default:		exiterr("Convert Buf Error: t Buf unknowned");
   }
}

*/
