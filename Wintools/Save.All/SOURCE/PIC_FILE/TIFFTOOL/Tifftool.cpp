#include "stdafx.h"
//#include <iostream.h>
#include <conio.h>

#include <compiler.hpp>
#include <tools.h>
#include <glib.h>

#include "tifftool.h"

/*--------------------------------------------------------------------------

	Image File Header
       ---------------------------------
	0-1 	II or MM
	2-3	version
	4-7     offset of first IFD
       ---------------------------------

	Image File Directory (IFD)
       ---------------------------------
	2 byte  number of directory entries
			 +
	0-1	The tag that identified the field                   +
	2-3	The field type                                      |
	4-7	The number of values, Count of the indicated Type   |  12byte
	8-11	The Value Offset                                    +
       ---------------------------------
			 +
			...   entries
			 +
	4 byte	next IFD offset (0 means none)
---------------------------------------------------------------------------*/

tifClass :: tifClass() : pictureFileClass()
{
   head.spSoftware = NULL;
   head.wpGrayResponseCurve = NULL;
   head.lpStripOffsets = NULL;
   head.lpStripByteCounts = NULL;
   head.wpBitsPerSample = NULL;
   head.wpMaxSampleValue = NULL;
   head.wpMinSampleValue = NULL;
   head.spMake = NULL;
}

tifClass :: ~tifClass()
{
   if(head.spSoftware != NULL)		delete head.spSoftware;
   if(head.wpGrayResponseCurve != NULL) delete head.wpGrayResponseCurve;
   if(head.lpStripOffsets != NULL)	delete head.lpStripOffsets;
   if(head.lpStripByteCounts != NULL)	delete head.lpStripByteCounts;
   if(head.wpBitsPerSample != NULL)	delete head.wpBitsPerSample;
   if(head.wpMaxSampleValue != NULL)	delete head.wpMaxSampleValue;
   if(head.wpMinSampleValue != NULL)	delete head.wpMinSampleValue;
   if(head.spMake != NULL)		delete head.spMake;
}

void tifClass :: GetHeader(TIFHEADER& h)
{
   memcpy(&h, &head, sizeof(TIFHEADER));
}

int tifClass :: ReadPrepare()
{
   long offset;
   TIFF_FIELD_HEADER field;
// typedef	struct tagtiffIFDfield {
//	WORD	fieldTag;
//	WORD	fieldType;
//	long	fieldLength;
//	long	fieldOffset;
//   }	TIFF_FIELD_HEADER;

   int size;
   short int fieldnumber;
   int i = 0;

   memset(&head, 0, sizeof(head));

   head.wSamplesPerPixel = 1;    	// default - mono image
   head.wPhotometricInterpretation = 1;	// default - black is zero

   fread(&head.wHardWare, 1, sizeof(head.wHardWare), in);
   fread(&head.wVersion, 1, sizeof(head.wVersion), in);
   fread(&offset, 1, sizeof(offset), in);
   fseek(in, offset, SEEK_SET);		// goto first IFD offset

   fread(&fieldnumber, 1, 2, in); 	// read number of directory entries

   for(i = 0; i < fieldnumber; i++) {
      size = fread(&field, 1, sizeof(TIFF_FIELD_HEADER), in);
      if(size != sizeof(TIFF_FIELD_HEADER))	break;

      switch(field.fieldTag) {
			case TF_NEW_SUB_FILE_TYPE:	// 0xFE
				head.lNewSubfileType = Value(field);		break;
			case TF_OLD_SUB_FILE_TYPE:	// 0xFF
				head.wOldSubfileType = (WORD)Value(field);		break;
			case TF_IMAGE_WIDTH:       	// 0x100
				head.lImageWidth = Value(field);			break;
			case TF_IMAGE_LENGTH:        	// 0x101
				head.lImageLength = Value(field);	 		break;
			case TF_BITS_PER_SAMPLE:     	// 0x102
				head.wpBitsPerSample = ReadWordPointer(field);	break;
			case TF_COMPRESSION:           // 0x103
				head.wCompression = (WORD)Value(field);	 	break;
			case TF_PHOTOMETRIC_INTERPRETATION: // 0x106
				head.wPhotometricInterpretation = (WORD)Value(field);break;
			case TF_THRESHOLDING:		// 0x107
				head.wThresholding = (WORD)Value(field);		break;
			case TF_MAKE:			// 0x10F
				head.spMake = ReadAsciiString(field);           	break;
			case TF_STRIP_OFFSETS:		// 0x111
				head.lpStripOffsets = ReadLongPointer(field);       break;
			case TF_SAMPLES_PER_PIXEL:	// 0x115
				head.wSamplesPerPixel = (WORD)Value(field);		break;
			case TF_ROWS_PER_STRIP:	// 0x116
				head.lRowsPerStrip = Value(field);			break;
			case TF_STRIP_BYTE_COUNTS: 	// 0x117
				head.lpStripByteCounts = ReadLongPointer(field);   	break;
			case TF_MIN_SAMPLE_VALUE:   	// 0x118
				head.wpMinSampleValue = ReadWordPointer(field);	break;
			case TF_MAX_SAMPLE_VALUE:   	// 0x119
				head.wpMaxSampleValue = ReadWordPointer(field);	break;
			case TF_X_RESOLUTION:		// 0x11A
				ReadRational(field, head.rXResolution);		break;
			case TF_Y_RESOLUTION:       	// 0x11B
				ReadRational(field, head.rYResolution);		break;
			case TF_PLANAR_CONFIGURATION: 	// 0x11C
				head.wPlanarConfiguration = (WORD)Value(field);	break;
			case TF_GRAY_RESPONSE_UNIT:  	// 0x122
				head.wGrayResponseUnit = (WORD)Value(field);	break;
			case TF_GRAY_RESPONSE_CURVE:  	// 0x123
				//printf("\nhalftone hints - %u, %u ", head.wHalftoneHints[0], head.wHalftoneHints[1]);
				ReadGrayResponseCurve(field);			break;
			case TF_GROUP_3_OPTIONS:				// 0x124 T4 Encoding Option 이라고도 한다.
				head.lGroup3Options = Value(field);	 		break;
			case TF_RESOLUTION_UNIT: 	// 0x128
				head.wResolutionUnit = (WORD)Value(field);		break;
			case TF_SOFTWARE:		// 0x131
				head.spSoftware = ReadAsciiString(field);		break;
			case TF_PREDICTOR:		// 0x13D
				head.wPredictor = (WORD)Value(field);		break;
			case TF_COLOR_MAP: 		// 0x140
				ReadColorMap(field);				break;
			case TF_HALFTONE_HINTS:	// 0x141
				head.wHalftoneHints[1] = (WORD)(field.fieldOffset >> 16);
				head.wHalftoneHints[0] = (WORD)(field.fieldOffset & 0x0000FFFFL);
				//printf("\n0x141 halftone hints - %u, %u ", head.wHalftoneHints[0], head.wHalftoneHints[1]);
				break;

			default:       bell();
				char imsi[100];
				sprintf(imsi, "\ntag-0x%X, type-%d, length-%lu, offset-0x%lX", field.fieldTag, field.fieldType, field.fieldLength, field.fieldOffset);
				MessageBox(NULL, imsi, "코드 보충 필요 tifClass :: ReadPrepare", MB_OK);
				break;
				//		getch();				break;
      }
   }

   if(head.wpBitsPerSample == NULL) {
       head.wpBitsPerSample = MakeWordDefault(1);
   }
   MakePaletteDefault();

   return 1;
}

LONG tifClass :: Value(TIFF_FIELD_HEADER field)
{
   LONG value;

   value = field.fieldOffset;

   switch(field.fieldType) {
       case TYPE_WORD:	value = value & 0x0000FFFF;	break;
       case TYPE_BYTE:	value = value & 0x000000FF;	break;
   }

   return value;
}

void tifClass :: ReadRational(TIFF_FIELD_HEADER field, LONG *val)
{
   fpos_t pos;

   fgetpos(in, &pos);
   fseek(in, field.fieldOffset, SEEK_SET);
   fread(val, 1, 8, in);
   fsetpos(in, &pos);
}

//----------------------------------------------------------------------------
//	Read Color Map  	3*(2**BitsPerSample)
//		    (ex 256 col) All Red(256) + All Green(256) + All Blue(256)
// attention - WORD value	dark - 0, 0, 0
//				   white 65535, 65535, 65535
//----------------------------------------------------------------------------

void tifClass :: ReadColorMap(TIFF_FIELD_HEADER field)
{
   fpos_t fpos;
   int i, j;
   int color;
   WORD ch;

   fgetpos(in, &fpos);

   fseek(in, field.fieldOffset, SEEK_SET);
   if(field.fieldLength > 768)	field.fieldLength = 768;
   color = (int)(field.fieldLength/3);
   for(i = 0; i < 3; i++) {
      for(j = 0; j < color; j++) {
			fread(&ch, 1, sizeof(ch), in);
			dac[j*3+i] = ch >> 8;
      }
   }
   fsetpos(in, &fpos);
}

void tifClass :: ReadGrayResponseCurve(TIFF_FIELD_HEADER field)
{
   fpos_t fpos;
   int i;
   WORD ch;
   double val;
   int dacpos;

   switch(head.wGrayResponseUnit) {
      case 1:	val = 10.; 		break;
      case 2:	val = 100.;    break;
      case 3:	val = 1000.;   break;
      case 4:	val = 10000.;  break;
      case 5:	val = 100000.; break;
      default:	val = 100.;    break;
   }

   fgetpos(in, &fpos);

   fseek(in, field.fieldOffset, SEEK_SET);
   if(field.fieldLength > 256)	field.fieldLength = 256;
   for(i = 0; i < field.fieldLength; i++) {
      fread(&ch, 1, sizeof(ch), in);
      if(head.wPhotometricInterpretation == 1)
			dacpos = i*3;
      else {
			dacpos = (int)((field.fieldLength-1-i)*3);
      }
      dac[dacpos+0] = dac[dacpos+1] = dac[dacpos+2] = (BYTE)((ch/val)*255.0/2.0);
   }
   fsetpos(in, &fpos);
}

char *tifClass :: ReadAsciiString(TIFF_FIELD_HEADER field)
{
   fpos_t fpos;
   char *ascii;

   ascii = new char[(WORD)(field.fieldLength+1)];
   if(ascii == NULL)	return NULL;

   fgetpos(in, &fpos);

   fseek(in, field.fieldOffset, SEEK_SET);
   fread(ascii, 1, (size_t)field.fieldLength, in);
   ascii[(WORD)field.fieldLength] = 0;

   fsetpos(in, &fpos);

   //cout << ascii;
   //getch();

   return ascii;
}

WORD *tifClass :: ReadWordPointer(TIFF_FIELD_HEADER field)
{
   fpos_t fpos;
   WORD *pointer;
   BYTE imsi;
   int i;

   pointer = new WORD[(WORD)field.fieldLength];
   if(pointer == NULL)	return NULL;

   if(field.fieldLength == 1) {
      pointer[0] = (WORD)field.fieldOffset;
      return pointer;
   }

   fgetpos(in, &fpos);

   fseek(in, field.fieldOffset, SEEK_SET);
   for(i = 0; i < field.fieldLength; i++) {
      if(field.fieldType == TYPE_WORD) {
			fread(&pointer[i], sizeof (WORD), 1, in);
      }
      else {
			fread(&imsi, sizeof(BYTE), 1, in);
			pointer[i] = imsi;
      }
   }

   fsetpos(in, &fpos);

   return pointer;
}

LONG *tifClass :: ReadLongPointer(TIFF_FIELD_HEADER field)
{
   fpos_t fpos;
   LONG *pointer;
   WORD imsi;
   int i;

   pointer = new LONG[(WORD)field.fieldLength];
   if(pointer == NULL)	return NULL;

   if(field.fieldLength == 1) {
      pointer[0] = (WORD)field.fieldOffset;
      return pointer;
   }

   fgetpos(in, &fpos);

   fseek(in, field.fieldOffset, SEEK_SET);
   for(i = 0; i < field.fieldLength; i++) {
      if(field.fieldType == TYPE_LONG)
			fread(&pointer[i], sizeof(LONG), 1, in);
      else {
			fread(&imsi, sizeof(WORD), 1, in);
			pointer[i] = imsi;
      }
   }
   fsetpos(in, &fpos);

   return pointer;
}

WORD *tifClass :: MakeWordDefault(WORD val)
{
   WORD *pointer;

   pointer = new WORD[1];
   if(pointer == NULL)	return NULL;

   pointer[0] = val;

   return pointer;
}

void tifClass :: MakePaletteDefault()
{
   int i;

   if(head.wGrayResponseUnit != 0)	return;

   if(head.wPhotometricInterpretation == 0) {
      switch(head.wpBitsPerSample[0]) {
			case 4:        
				for(i = 0; i < 16; i++) {
					dac[i*3] = dac[i*3+1] = dac[i*3+2] = (BYTE)(15-(long)255*i/15);
				}
				break;
			case 8:        
				for(i = 0; i < 256; i++) {
					dac[i*3] = dac[i*3+1] = dac[i*3+2] = 255-i;
				}
				break;
      }
   }
   else if(head.wPhotometricInterpretation == 1) {
      switch(head.wpBitsPerSample[0]) {
			case 4:        
				for(i = 0; i < 16; i++) {
					dac[i*3] = dac[i*3+1] = dac[i*3+2] = (BYTE)((long)255*i/15);
				}
				break;
			case 8:        
				for(i = 0; i < 256; i++) {
					dac[i*3] = dac[i*3+1] = dac[i*3+2] = i;
				}
				break;
      }
   }
   else;
}

//----------------------------------------------------------------------------
//	Read Open Ready
//----------------------------------------------------------------------------

int tifClass :: ReadOpen(char *filename)
{
   if(dac == NULL)	return 0;	// dac not ready

   in = fopen(filename, "rb");
   if(in == NULL)	return 0;	// file open error

   if(!ReadPrepare()) {
      fclose(in);
		in = NULL;
      return 0;
   }

	switch(head.wCompression) {
		case 1:
		case 32773u:
		case 3:
			break;
		default:
			char message[80];
			sprintf(message, "이 압축방식은 지원하지 않습니다. (압축-%u)", head.wCompression);
			MessageBox(NULL, message, "Tiff Read Open", MB_OK);
			fclose(in);
			in = NULL;
			return 0;
	}

   nWidth =  (int)head.lImageWidth;
   nHeight = (int)head.lImageLength;
   nBitsPerPixel = head.wSamplesPerPixel*head.wpBitsPerSample[0];

   switch(nBitsPerPixel) {
      case COLOR_2:	nBytesPerLine = (nWidth+7)/8;	break;
      case COLOR_16:	nBytesPerLine = (nWidth+1)/2;	break;
      case COLOR_256:	nBytesPerLine = nWidth;		break;
      case COLOR_32768:
      case COLOR_65536: nBytesPerLine = nWidth*2;	break;
      case COLOR_1600:	nBytesPerLine = nWidth*3;	break;
   }
   fseek(in, head.lpStripOffsets[0], SEEK_SET);
   lCurrentPosY = 0L;
   return 1;
}

int tifClass :: GetOneLine(BYTE *buf, int maxx)
{
   int retn = 1;

   //if(lCurrentPosY%head.lRowsPerStrip == 0L) {
   //   if(head.lRowsPerStrip != 0)
   //	 fseek(in, head.lpStripOffsets[lCurrentPosY/head.lRowsPerStrip], SEEK_SET);
   //}
   switch(head.wCompression) {
      case 1:	   
			retn = GetOneLineComp1(buf, maxx);
			break;
      //case 2:   retn = GetOneLineComp2(buf, maxx);
      //		break;
		case 3:	
			retn = GetOneLineComp3(buf, maxx);
			break;
      case 32773u:
			retn = GetOneLineComp32773(buf, maxx);
			break;
   }
   lCurrentPosY++;
   return retn;
}

//----------------------------------------------------------
//	압축을 하지 않는 저장 방식.
//----------------------------------------------------------

int tifClass :: GetOneLineComp1(BYTE *buf, int maxx)
{
   switch(nBitsPerPixel) {
      case COLOR_2:
			fread(buf, 1, nBytesPerLine, in);
			if(head.wPhotometricInterpretation == 0) {
				for(int i = 0; i < nBytesPerLine; i++) 	buf[i] = ~buf[i];
			}
			break;
      case COLOR_16: 
			{
			register int plane;
			register int xbyte;
			int value;
			int ch;
			int targetbyte = (maxx+7)/8;
			memset(buf, 0, targetbyte*4);

			for(xbyte = 0; xbyte < nBytesPerLine; xbyte++) {
				ch = getc(in);
				value = ch >> 4;
				if(xbyte*2 >= maxx)	continue;
				for(plane = 3; plane >= 0; plane--)
					if(value & BIT_MASK[plane+4])
						buf[targetbyte*plane+xbyte/4] += BIT_MASK[xbyte%4*2];
				value = ch & 0x0F;
				for(plane = 3; plane >= 0; plane--)
					if(value & BIT_MASK[plane+4])
						buf[targetbyte*plane+xbyte/4] += BIT_MASK[xbyte%4*2+1];
			}
			break;
			}
      case COLOR_1600:
			fread(buf, 1, nBytesPerLine, in);
			if(head.wPhotometricInterpretation == 2) { // RGB
				BYTE temp;
				for(int i = 0; i < nBytesPerLine; i+=3) {
					temp = buf[i];
					buf[i] = buf[i+2];
					buf[i+2] = temp;
				}
			}
			else {

			}
			break;
      default:  
			fread(buf, 1, nBytesPerLine, in);
			break;
   }
   return 1;
}

int tifClass :: GetOneLineComp32773(BYTE *buf, int maxx)
{
   switch(nBitsPerPixel) {
      case COLOR_2:  return(GetOneLineComp32773_2(buf, maxx));
      case COLOR_16:	return(GetOneLineComp32773_16(buf, maxx));
   }
   return 0;
}

int tifClass :: GetOneLineComp32773_16(BYTE *buf, int maxx)
{
   int ch, count;
   int x = 0;
   int bufByte = (maxx+7)/8;
   memset(buf, 0, bufByte*4);
   int plane;
   int value1, value2;

   x = 0;
   while(1) {
      ch = fgetc(in);
      if(ch == EOF)	return 0;

      if(ch & 0x80) {	//
			count = -1*(char)ch+1;
			ch = fgetc(in);
			value1 = ch >> 4;
			value2 = ch & 0x000F;
			while(count-- > 0) {
				if(x*2 < maxx) {
					for(plane = 3; plane >= 0; plane--)
						if(value1 & BIT_MASK[plane+4])
							buf[bufByte*plane+x/4] += BIT_MASK[x%4*2];
					for(plane = 3; plane >= 0; plane--)
						if(value2 & BIT_MASK[plane+4])
							buf[bufByte*plane+x/4] += BIT_MASK[x%4*2+1];
				}
				x++;
			}
      }
      else {
			count = ch+1;
			while(count-- > 0) {
				ch = fgetc(in);
				if(x*2 < maxx) {
					value1 = ch >> 4;
					for(plane = 3; plane >= 0; plane--)
						if(value1 & BIT_MASK[plane+4])
							buf[bufByte*plane+x/4] += BIT_MASK[x%4*2];
					value2 = ch & 0x000F;
					for(plane = 3; plane >= 0; plane--)
						if(value2 & BIT_MASK[plane+4])
							buf[bufByte*plane+x/4] += BIT_MASK[x%4*2+1];
				}
				x++;
			}
      }
      if(x >= nBytesPerLine)  break;
   }
   return 1;
}

int tifClass :: GetOneLineComp32773_2(BYTE *buf, int maxx)
{
   int ch, count;
   int x = 0;
   int bufByte = (maxx+7)/8;

   x = 0;
   while(1) {
      ch = fgetc(in);
      if(ch == EOF)	return 0;

      if(ch & 0x80) {	//
			count = -1*(char)ch+1;
			ch = fgetc(in);
			while(count-- > 0) {
				if(x < bufByte)	buf[x] = ch;
				x++;
			}
      }
      else {
			count = ch+1;
			while(count-- > 0) {
				if(x < bufByte)	buf[x] = fgetc(in);
				x++;
			}
      }
      if(x >= nBytesPerLine)  break;
   }

   if(head.wPhotometricInterpretation == 0)
      for(x = 0; x < bufByte; x++) buf[x] = ~buf[x];
   return 1;
}

static T4_RUN_LENGTH_CODE_WORD whiteCode[105] = {
	{    0, "00110101" },
	{    1, "000111" },
	{    2, "0111" },
	{    3, "1000" },
	{    4, "1011" },
	{    5, "1100" },
	{    6, "1110" },
	{    7, "1111" },
	{    8, "10011" },
	{    9, "10100" },
	{   10, "00111" },
	{   11, "01000" },
	{   12, "001000" },
	{   13, "000011" },
	{   14, "110100" },
	{   15, "110101" },
	{   16, "101010" },
	{   17, "101011" },
	{   18, "0100111" },
	{   19, "0001100" },
	{   20, "0001000" },
	{   21, "0010111" },
	{   22, "0000011" },
	{   23, "0000100" },
	{   24, "0101000" },
	{   25, "0101011" },
	{   26, "0010011" },
	{   27, "0100100" },
	{   28, "0011000" },
	{   29, "00000010" },
	{   30, "00000011" },
	{   31, "00011010" },
	{   32, "00011011" },
	{   33, "00010010" },
	{   34, "00010011" },
	{   35, "00010100" },
	{   36, "00010101" },
	{   37, "00010110" },
	{   38, "00010111" },
	{   39, "00101000" },
	{   40, "00101001" },
	{   41, "00101010" },
	{   42, "00101011" },
	{   43, "00101100" },
	{   44, "00101101" },
	{   45, "00000100" },
	{   46, "00000101" },
	{   47, "00001010" },
	{   48, "00001011" },
	{   49, "01010010" },
	{   50, "01010011" },
	{   51, "01010100" },
	{   52, "01010101" },
	{   53, "00100100" },
	{   54, "00100101" },
	{   55, "01011000" },
	{   56, "01011001" },
	{   57, "01011010" },
	{   58, "01011011" },
	{   59, "01001010" },
	{   60, "01001011" },
	{   61, "00110010" },
	{   62, "00110011" },
	{   63, "00110100" },
	{   64, "11011" },
	{  128, "10010" },
	{  192, "010111" },
	{  256, "0110111" },
	{  320, "00110110" },
	{  384, "00110111" },
	{  448, "01100100" },
	{  512, "01100101" },
	{  576, "01101000" },
	{  640, "01100111" },
	{  704, "011001100" },
	{  768, "011001101" },
	{  832, "011010010" },
	{  896, "011010011" },
	{  960, "011010100" },
	{ 1024, "011010101" },
	{ 1088, "011010110" },
	{ 1152, "011010111" },
	{ 1216, "011011000" },
	{ 1280, "011011001" },
	{ 1344, "011011010" },
	{ 1408, "011011011" },
	{ 1472, "010011000" },
	{ 1536, "010011001" },
	{ 1600, "010011010" },
	{ 1664, "011000" },
	{ 1728, "010011011" },
	{ 1792, "00000001000" },
	{ 1856, "00000001100" },
	{ 1920, "00000001101" },
	{ 1984, "000000010010" },
	{ 2048, "000000010011" },
	{ 2112, "000000010100" },
	{ 2176, "000000010101" },
	{ 2240, "000000010110" },
	{ 2304, "000000010111" },
	{ 2368, "000000011100" },
	{ 2432, "000000011101" },
	{ 2496, "000000011110" },
	{ 2560, "000000011111" },
	{ 9999, "000000000001" }};	// end signal


static T4_RUN_LENGTH_CODE_WORD blackCode[105] = {
	{    0, "0000110111" },
	{    1, "010" },
	{    2, "11" },
	{    3, "10" },
	{    4, "011" },
	{    5, "0011" },
	{    6, "0010" },
	{    7, "00011" },
	{    8, "000101" },
	{    9, "000100" },
	{   10, "0000100" },
	{   11, "0000101" },
	{   12, "0000111" },
	{   13, "00000100" },
	{   14, "00000111" },
	{   15, "000011000" },
	{   16, "0000010111" },
	{   17, "0000011000" },
	{   18, "0000001000" },
	{   19, "00001100111" },
	{   20, "00001101000" },
	{   21, "00001101100" },
	{   22, "00000110111" },
	{   23, "00000101000" },
	{   24, "00000010111" },
	{   25, "00000011000" },
	{   26, "000011001010" },
	{   27, "000011001011" },
	{   28, "000011001100" },
	{   29, "000011001101" },
	{   30, "000001101000" },
	{   31, "000001101001" },
	{   32, "000001101010" },
	{   33, "000001101011" },
	{   34, "000011010010" },
	{   35, "000011010011" },
	{   36, "000011010100" },
	{   37, "000011010101" },
	{   38, "000011010110" },
	{   39, "000011010111" },
	{   40, "000001101100" },
	{   41, "000001101101" },
	{   42, "000011011010" },
	{   43, "000011011011" },
	{   44, "000001010100" },
	{   45, "000001010101" },
	{   46, "000001010110" },
	{   47, "000001010111" },
	{   48, "000001100100" },
	{   49, "000001100101" },
	{   50, "000001010010" },
	{   51, "000001010011" },
	{   52, "000000100100" },
	{   53, "000000110111" },
	{   54, "000000111000" },
	{   55, "000000100111" },
	{   56, "000000101000" },
	{   57, "000001011000" },
	{   58, "000001011001" },
	{   59, "000000101011" },
	{   60, "000000101100" },
	{   61, "000001011010" },
	{   62, "000001100110" },
	{   63, "000001100111" },
	{   64, "0000001111" },
	{  128, "000011001000" },
	{  192, "000011001001" },
	{  256, "000001011011" },
	{  320, "000000110011" },
	{  384, "000000110100" },
	{  448, "000000110101" },
	{  512, "0000001101100" },
	{  576, "0000001101101" },
	{  640, "0000001001010" },
	{  704, "0000001001011" },
	{  768, "0000001001100" },
	{  832, "0000001001101" },
	{  896, "0000001110010" },
	{  960, "0000001110011" },
	{ 1024, "0000001110100" },
	{ 1088, "0000001110101" },
	{ 1152, "0000001110110" },
	{ 1216, "0000001110111" },
	{ 1280, "0000001010010" },
	{ 1344, "0000001010011" },
	{ 1408, "0000001010100" },
	{ 1472, "0000001010101" },
	{ 1536, "0000001011010" },
	{ 1600, "0000001011011" },
	{ 1664, "0000001100100" },
	{ 1728, "0000001100101" },
	{ 1792, "00000001000" },
	{ 1856, "00000001100" },
	{ 1920, "00000001101" },
	{ 1984, "000000010010" },
	{ 2048, "000000010011" },
	{ 2112, "000000010100" },
	{ 2176, "000000010101" },
	{ 2240, "000000010110" },
	{ 2304, "000000010111" },
	{ 2368, "000000011100" },
	{ 2432, "000000011101" },
	{ 2496, "000000011110" },
	{ 2560, "000000011111" },
	{ 9999, "00000000000" }};	// EOF end signal

int tifClass :: GetOneLineComp3_Extend(BYTE *buf, int maxx, T4_RUN_LENGTH_CODE_WORD *code)
{
	// code 중에서 제일 작은 갯수는 2개이고 제일 큰갯수는 13개이다.

   int  count;
   int  x = 0;
   int  bufByte = (maxx+7)/8;
	char imsi[20];
	WORD makeup;
	int  i, j;
	int  ch; 
		
	count = 0;
   x = 0;
	makeup = 0;

	while(1) {
      ch = fgetc(in);
      if(ch == EOF)	return 0;

		for(i = 0; i < 8; i++) {
			//for(j = 0; j < count; j++) {
			//	imsi[count] = imsi[count-1];	
			//}

			if(BIT_MASK[i] & ch) {
				imsi[count] = '1';
			}
			else {
				imsi[count] = '0';
			}

			count++;
			imsi[count] = 0;

			if(count > 13)	{
				bell();
				return 0;
			}

			for(j = 104; j >= 0; j--) {	// 일치하는 code word를 찾는다.
				if(strcmp(imsi, code[j].code) == 0) {	// 일치하는 word를 찾았다.
					if(code[j].value == 9999) {	// EOL
						makeup = 0;
						// x = 0;
					}
					else if(code[j].value > 63) {
						makeup += code[j].value;	
					}
					else {
						if(x < bufByte)	buf[x] = LOBYTE(code[j].value+makeup);
						x++;
						if(x < bufByte)	buf[x] = HIBYTE(code[j].value+makeup);
						x++;
						makeup = 0;
						if(x >= nBytesPerLine)  return 1;
					}
					count = 0;
					break;
				}
			}

		}
   }

   return 1;
}

int tifClass :: GetOneLineComp3(BYTE *buf, int maxx)
{
	return GetOneLineComp3_Extend(buf, maxx, whiteCode);
}
	
/*
int tifClass :: WriteOpen(char *filename, int width, int height, int bitsperpixel, BYTE pal)
{
   if(dac == NULL)	return 0;	// dac not ready

   out = fopen(filename, "wb");
   if(out == NULL)	return 0;	// write file open error

   nWidth =  width;
   nHeight = height;
   nBitsPerPixel = bitsperpixel;

   switch(nBitsPerPixel) {
      case COLOR_2:	nBytesPerLine = (nWidth+7)/8;	break;
      case COLOR_16:	nBytesPerLine = (nWidth+1)/2;	break;
      case COLOR_256:	nBytesPerLine = nWidth;		break;
      case COLOR_32768:
      case COLOR_65536: nBytesPerLine = nWidth*2;	break;
      case COLOR_1600:	nBytesPerLine = nWidth*3;	break;
   }
   lCurrentPosY = 0L;

   int entries;
   TIFF_FIELD_HEADER field;
   TIFF_FILE_HEADER prehead;

   prehead.hardware = 0x4949;	// "II"
   prehead.version  = 0x002A;	// version
   prehead.offset   = 8;     	// first IFD offset
   fwrite(&prehead, 1, sizeof(head), out);	// write head

   entries = 10;
   fwrite(&entries, 1, 2, out);		// directory entry number

   field.fieldLength = 1;
   field.fieldOffset = 0;
   field.fieldTag = TF_OLD_SUB_FILE_TYPE;
   field.fieldType = TYPE_WORD;
   fwrite(&field, 1, sizeof(field), out);

   field.fieldLength = 1;
   field.fieldOffset = 0;
   field.fieldTag = TF_OLD_SUB_FILE_TYPE;
   field.fieldType = TYPE_WORD;
   fwrite(&field, 1, sizeof(field), out);

   field.fieldLength = 1;
   field.fieldOffset = width;
   field.fieldTag = TF_IMAGE_WIDTH;
   field.fieldType = TYPE_LONG;
   fwrite(&field, 1, sizeof(field), out);

   field.fieldLength = 1;
   field.fieldOffset = height;
   field.fieldTag = TF_IMAGE_LENGTH;
   field.fieldType = TYPE_LONG;
   fwrite(&field, 1, sizeof(field), out);

   field.fieldLength = 1;
   field.fieldOffset = 32773u;	// run length encoding
   field.fieldTag = TF_COMPRESSION;
   field.fieldType = TYPE_WORD;
   fwrite(&field, 1, sizeof(field), out);

   return 1;
}

void tifClass :: WriteClose()
{
   if(out != NULL)	fclose(out);
}

int  tifClass :: PutOneLine(BYTE *buf)
{
   if(out == NULL)	return 0;
   return 1;
}
*/

