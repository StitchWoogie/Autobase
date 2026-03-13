/*--------------------------------------------------------
	HELLOWIN.C -- Displays "Hello, Windows" in client area
					  (c) Charles Petzold, 1992
  --------------------------------------------------------*/
#include "stdafx.h"
#include <stdio.h>
#include <string.h>

#include <compiler.hpp>
#include <tools.h>
#include <glib.h>
#include <gclass.h>
#include <dataswap.h>

#include <ttftool.h>

ttfClass :: ttfClass()
{
	hDC = NULL;
	xCoordinates = NULL;
	yCoordinates = NULL;
	endPtsOfContours = NULL;
	flags = NULL;
}

void ttfClass :: FreeAllBuf()
{
	if(xCoordinates != NULL)		delete xCoordinates;
	if(yCoordinates != NULL)		delete yCoordinates;
	if(endPtsOfContours != NULL)	delete endPtsOfContours;
	if(flags != NULL)					delete flags;

	xCoordinates = NULL;
	yCoordinates = NULL;
	endPtsOfContours = NULL;
	flags = NULL;
}

ttfClass :: ~ttfClass()
{
	FreeAllBuf();
}


DWORD GetTtfTable(char *buf)
{
	DWORD value;

	value = (DWORD)     ((DWORD)buf[3] << 24)
					  + ((DWORD)buf[2] << 16)
					  + ((DWORD)buf[1] <<  8)
					  + ((DWORD)buf[0]);
	return value;
}

void ttfClass :: PrepareTableNAME(HDC hdc)
{
	typedef struct {
		USHORT	formatSelector;
		USHORT	numberOfNameRecords;
		USHORT	offsetToStartOfStringStorage;
	} NAME_TABLE_HEAD;

	typedef struct {
		USHORT	platformID;
		USHORT	platformEncodingID;
		USHORT	languageID;
		USHORT	nameID;
		USHORT	stringLength;
		USHORT	stringOffset;
	} NAME_RECORD_STRUCT;

	NAME_TABLE_HEAD head;
	NAME_RECORD_STRUCT record;
	int i;

	bUniCode = OFF;

	if(GetFontData(hdc, GetTtfTable("name"), 0, &head, sizeof(NAME_TABLE_HEAD)) == -1) {
		return;
	}

	head.formatSelector = MOTO2IBM(head.formatSelector);
	head.numberOfNameRecords = MOTO2IBM(head.numberOfNameRecords);
	head.offsetToStartOfStringStorage = MOTO2IBM(head.offsetToStartOfStringStorage);

	for(i = 0; i < head.numberOfNameRecords; i++) {
		if(GetFontData(hdc, GetTtfTable("name"), sizeof(NAME_TABLE_HEAD)+sizeof(NAME_RECORD_STRUCT)*i, &record, sizeof(NAME_RECORD_STRUCT)) == -1) {
			return;
		}	
		record.platformEncodingID = MOTO2IBM(record.platformEncodingID);
		if(record.platformEncodingID == 1) {
			bUniCode = ON;
			return;
		}
	}
}

int ttfClass :: SetHDC(HDC hdc)
{
	int i;
	
	hDC = hdc;

	if(GetFontData(hdc, GetTtfTable("head"), 0, &headFont, sizeof(FONT_HEADER)) == -1) {
		return 0;
	}
	headFont.version = MOTO2IBM(headFont.version);
	headFont.fontRevision = MOTO2IBM(headFont.fontRevision);
	headFont.checkSumAdjustment = MOTO2IBM(headFont.checkSumAdjustment);
	headFont.magicNumber = MOTO2IBM(headFont.magicNumber);
	headFont.flags = MOTO2IBM(headFont.flags);
	headFont.unitsPerEm = MOTO2IBM(headFont.unitsPerEm);
	//headFont.created = MOTO2IBM(headFont.created);
	//headFont.modified = MOTO2IBM(headFont.modified);
	headFont.xMin = MOTO2IBM(headFont.xMin);
	headFont.yMin = MOTO2IBM(headFont.yMin);
	headFont.xMax = MOTO2IBM(headFont.xMax);
	headFont.yMax = MOTO2IBM(headFont.yMax);
	headFont.macStyle = MOTO2IBM(headFont.macStyle);
	headFont.lowestRecPPEM = MOTO2IBM(headFont.lowestRecPPEM);
	headFont.fontDirectionHint = MOTO2IBM(headFont.fontDirectionHint);
	headFont.indexToLocFormat = MOTO2IBM(headFont.indexToLocFormat);
	headFont.glyphDataFormat = MOTO2IBM(headFont.glyphDataFormat);

	if((int)GetFontData(hdc, GetTtfTable("hhea"), 0, &headHorizontal, sizeof(HORIZONTAL_HEADER)) == -1) {
		return 0;
	}

	headHorizontal.version =  MOTO2IBM(headHorizontal.version);
	headHorizontal.Ascender = MOTO2IBM(headHorizontal.Ascender);
	headHorizontal.Descender = MOTO2IBM(headHorizontal.Descender);
	headHorizontal.LineGap = MOTO2IBM(headHorizontal.LineGap);
	headHorizontal.advanceWidthMax = MOTO2IBM(headHorizontal.advanceWidthMax);
	headHorizontal.minLeftSideBearing = MOTO2IBM(headHorizontal.minLeftSideBearing);
	headHorizontal.minRightSideBearing = MOTO2IBM(headHorizontal.minRightSideBearing);
	headHorizontal.xMaxExtent = MOTO2IBM(headHorizontal.xMaxExtent);
	headHorizontal.caretSlopeRise = MOTO2IBM(headHorizontal.caretSlopeRise);
	headHorizontal.caretSlopeRun = MOTO2IBM(headHorizontal.caretSlopeRun);
	headHorizontal.reserved1 = MOTO2IBM(headHorizontal.reserved1);
	headHorizontal.reserved2 = MOTO2IBM(headHorizontal.reserved2);
	headHorizontal.reserved3 = MOTO2IBM(headHorizontal.reserved3);
	headHorizontal.reserved4 = MOTO2IBM(headHorizontal.reserved4);
	headHorizontal.reserved5 = MOTO2IBM(headHorizontal.reserved5);
	headHorizontal.metricDataFormat = MOTO2IBM(headHorizontal.metricDataFormat);
	headHorizontal.numberOfHMetrics = MOTO2IBM(headHorizontal.numberOfHMetrics);

	if((int)GetFontData(hdc, GetTtfTable("maxp"), 0, &headMaxp, sizeof(MAXP_HEADER)) == -1) {
		return 0;
	}
	headMaxp.version = MOTO2IBM(headMaxp.version);
	headMaxp.numGlyphs = MOTO2IBM(headMaxp.numGlyphs);

	if((int)GetFontData(hdc, GetTtfTable("OS/2"), 0, &headOS2, sizeof(OS2_HEADER)) == -1) {
		return 0;
	}
	headOS2.version = MOTO2IBM(headOS2.version);
	headOS2.xAvgCharWidth = MOTO2IBM(headOS2.xAvgCharWidth);
	headOS2.usWeightClass = MOTO2IBM(headOS2.usWeightClass);
	headOS2.usWidthClass = MOTO2IBM(headOS2.usWidthClass);
	headOS2.fsType = MOTO2IBM(headOS2.fsType);
	headOS2.ySubscriptXSize = MOTO2IBM(headOS2.ySubscriptXSize);
	headOS2.ySubscriptYSize = MOTO2IBM(headOS2.ySubscriptYSize);
	headOS2.ySubscriptXOffset = MOTO2IBM(headOS2.ySubscriptXOffset);
	headOS2.ySubscriptYOffset = MOTO2IBM(headOS2.ySubscriptYOffset);
	headOS2.ySuperscriptXSize = MOTO2IBM(headOS2.ySuperscriptXSize);
	headOS2.ySuperscriptYSize = MOTO2IBM(headOS2.ySuperscriptYSize);
	headOS2.ySuperscriptXOffset = MOTO2IBM(headOS2.ySuperscriptXOffset);
	headOS2.ySuperscriptYOffset = MOTO2IBM(headOS2.ySuperscriptYOffset);
	headOS2.yStrikeoutSize = MOTO2IBM(headOS2.yStrikeoutSize);
	headOS2.yStrikeoutPosition = MOTO2IBM(headOS2.yStrikeoutPosition);
	headOS2.sFamilyClass = MOTO2IBM(headOS2.sFamilyClass);
//	headOS2.panose =      MOTO2IBM(headOS2.panose);		BYTE[10] 이므로 변환할 필요가 없다.
	for(i = 0; i < 4; i++) {
	   headOS2.ulCharRange[i] = MOTO2IBM(headOS2.ulCharRange[i]);
	}
//	headOS2.achVendID =   MOTO2IBM(headOS2.achVendID);	char[4] 이므로 변환할 필요가 없다.
	headOS2.fsSelection = MOTO2IBM(headOS2.fsSelection);
	headOS2.usFirstCharIndex = MOTO2IBM(headOS2.usFirstCharIndex);
	headOS2.usLastCharIncex = MOTO2IBM(headOS2.usLastCharIncex);
	headOS2.sTypoAscender = MOTO2IBM(headOS2.sTypoAscender);
	headOS2.sTypoDescender = MOTO2IBM(headOS2.sTypoDescender);
	headOS2.sTypoLineGap = MOTO2IBM(headOS2.sTypoLineGap);
	headOS2.usWinAscent = MOTO2IBM(headOS2.usWinAscent);
	headOS2.usWinDescent = MOTO2IBM(headOS2.usWinDescent);
 
	if(!PrepareIndexMappingTable()) {
		return 0;
	}

	PrepareTableNAME(hdc);

	return 1;
}

int ttfClass :: GetWidth(USHORT glyph_num)
{
	typedef struct {
		uFWord advanceWidth;
		FWord	 lsb;
	} longHorMetric_HEADER;

	longHorMetric_HEADER longHorMetric;

	if(glyph_num >= headMaxp.numGlyphs)	return 0;	// zone over
	if(glyph_num < headHorizontal.numberOfHMetrics) {	// numberOfHMetrics안에 있을때
		if((int)GetFontData(hDC, GetTtfTable("hmtx"), (long)glyph_num*sizeof(longHorMetric), &longHorMetric, sizeof(longHorMetric)) == -1) {
			return 0;
		}
		longHorMetric.advanceWidth = MOTO2IBM(longHorMetric.advanceWidth);
		longHorMetric.lsb = MOTO2IBM(longHorMetric.lsb);

		return longHorMetric.advanceWidth;
	}
	else {
		if((int)GetFontData(hDC, GetTtfTable("hmtx"), (long)(headHorizontal.numberOfHMetrics-1)*sizeof(longHorMetric), &longHorMetric, sizeof(longHorMetric)) == -1) {
			return 0;
		}
		longHorMetric.advanceWidth = MOTO2IBM(longHorMetric.advanceWidth);
		longHorMetric.lsb = MOTO2IBM(longHorMetric.lsb);
		//if((int)GetFontData(hdc, GetTtfTable("hmtx"), (long)(headHorizontal.numberOfHMetrics)*sizeof(longHorMetric), &longHorMetric, sizeof(longHorMetric) == -1) {
		//	return 0;
		//}
		return longHorMetric.advanceWidth;
	}
}

//------------------------------------------------------------------------------
// 주어진 글자를 빨리 찾기 위해서 Index Table을 미리 준비한다.
//------------------------------------------------------------------------------

int ttfClass :: PrepareIndexMappingTable()
{

typedef struct {
	USHORT version;
	USHORT numberOfEncodingTable;
} CMAP_HEADER;

typedef struct {
	USHORT platformID;
	USHORT platformSpecificEncodingID;
	ULONG  offsetFromBeginningOfTable;
} CMAP_ACTUAL_SUBTABLE_STRUCT;

/*
typedef struct {
	USHORT format;		// 0, 2, 4, 6의 6가지 형식이 있으나 처음구조의 6바이트는 같다.
	USHORT length;
	USHORT version;
	USHORT segCount;
	USHORT searchRange;
	USHORT entrySelector;
	USHORT rangeShift;
} CMAP_INDEX_MAPPING_TABLE_FORMAT4;
*/

	CMAP_HEADER headCmap;
	CMAP_ACTUAL_SUBTABLE_STRUCT *headActualSubtable;
	USHORT format;
	int    format4;
	int i;

	if((int)GetFontData(hDC, GetTtfTable("cmap"), 0, &headCmap, sizeof(CMAP_HEADER)) == -1) {
		return 0;
	}

	headCmap.version = MOTO2IBM(headCmap.version);
	headCmap.numberOfEncodingTable = MOTO2IBM(headCmap.numberOfEncodingTable);

	if(headCmap.numberOfEncodingTable == 0)	return 0;
	if(headCmap.numberOfEncodingTable > 10)	return 0;

	headActualSubtable = new CMAP_ACTUAL_SUBTABLE_STRUCT[headCmap.numberOfEncodingTable];
	if(headActualSubtable == NULL)	return 0;

	format4 = -1;

	for(i = 0; i < headCmap.numberOfEncodingTable; i++) {
		if((int)GetFontData(hDC, GetTtfTable("cmap"), sizeof(CMAP_HEADER)+sizeof(CMAP_ACTUAL_SUBTABLE_STRUCT)*i, &headActualSubtable[i], sizeof(CMAP_ACTUAL_SUBTABLE_STRUCT)) == -1) {
			continue;
//			return 0;
		}
		headActualSubtable[i].platformID = MOTO2IBM(headActualSubtable[i].platformID);
		headActualSubtable[i].platformSpecificEncodingID = MOTO2IBM(headActualSubtable[i].platformSpecificEncodingID);
		headActualSubtable[i].offsetFromBeginningOfTable = MOTO2IBM(headActualSubtable[i].offsetFromBeginningOfTable);
		if((int)GetFontData(hDC, GetTtfTable("cmap"), headActualSubtable[i].offsetFromBeginningOfTable, &format, sizeof(USHORT)) == -1) {
//			return 0;
			continue;
		}
		format = MOTO2IBM(format);
		if(format == 4) {
			format4 = i;
		}
	}

	if(format4 == -1)	{
		delete headActualSubtable;
		return 0;
	}

	if((int)GetFontData(hDC, GetTtfTable("cmap"), headActualSubtable[format4].offsetFromBeginningOfTable+sizeof(USHORT)*3, &usIndexMappingSegCount, sizeof(USHORT)) == -1) {
		delete headActualSubtable;
		return 0;
	}

	usIndexMappingSegCount = MOTO2IBM(usIndexMappingSegCount)/2;
	ulIndexOffset = headActualSubtable[format4].offsetFromBeginningOfTable;

	delete headActualSubtable;

	return 1;
}

USHORT ttfClass :: GetGlyphPosition(USHORT font_number)
{
	USHORT endCount, startCount, idRangeOffset, idDelta;
	ULONG offset;
	USHORT glyphPos;
	int i;

#if	defined (_WIN32) 	// 32 bit 에서만 unicode를 사용할 수 있다. 
	if(bUniCode && font_number > 255) {		// unicode 일 때는 적절한 한글 코드로 바꿔준다.
		char imsi[10];
		wchar_t ex[10];

		imsi[0] = HIBYTE(font_number);
		imsi[1] = LOBYTE(font_number);
		imsi[2] = 0;

		MultiByteToWideChar(CP_ACP, MB_COMPOSITE, imsi, 2, ex, 5);

		font_number = ex[0];
	}
#endif
		
	for(i = 0; i < usIndexMappingSegCount; i++) {

		// endCount 를 읽는다.
		offset = ulIndexOffset+sizeof(USHORT)*(7+i);
		if((int)GetFontData(hDC, GetTtfTable("cmap"), offset, &endCount, sizeof(USHORT)) == -1) {
			return 0;
		}
		endCount = MOTO2IBM(endCount);

		if(endCount == 0xFFFF)	break;

		if(font_number <= endCount) {
			// startCount를 읽는다.
			offset = ulIndexOffset+sizeof(USHORT)*(7+usIndexMappingSegCount+1+i);
			if((int)GetFontData(hDC, GetTtfTable("cmap"), offset, &startCount, sizeof(USHORT)) == -1) {
				return 0;
			}
			startCount = MOTO2IBM(startCount);

			if(startCount == 0xFFFF)	break;

			if(font_number >= startCount) {	// yes i seek fit seg
				// Range Offset을 읽는다.
				offset = ulIndexOffset+sizeof(USHORT)*(8+usIndexMappingSegCount*3+i);
				if((int)GetFontData(hDC, GetTtfTable("cmap"), offset, &idRangeOffset, sizeof(USHORT)) == -1) {
					return 0;
				}
				idRangeOffset = MOTO2IBM(idRangeOffset);

				if(idRangeOffset == 0) {
					// idDelta값을 읽는다.
					offset = ulIndexOffset+sizeof(USHORT)*(8+usIndexMappingSegCount*2+i);
					if((int)GetFontData(hDC, GetTtfTable("cmap"), offset, &idDelta, sizeof(USHORT)) == -1) {
						return 0;
					}
					idDelta = MOTO2IBM(idDelta);
					glyphPos = (idDelta + font_number);
//					glyphPos -= (font_number;
				}
				else {
					offset = ulIndexOffset+sizeof(USHORT)*(8+usIndexMappingSegCount*3+i)+idRangeOffset+(font_number-startCount)*sizeof(USHORT);
					if((int)GetFontData(hDC, GetTtfTable("cmap"), offset, &glyphPos, sizeof(USHORT)) == -1) {
						return 0;
					}
					glyphPos = MOTO2IBM(glyphPos);
				}

				return glyphPos;
			}
			else {	// 찾는 폰트는 빠져있다.
				return 0;
			}
		}
	}

	return 0;
}

int ttfClass :: GetFontOffsetAndSize(int font_number, ULONG &offset, DWORD &size)
{
	int one_block;
	BYTE buf[10];
//	char msg[80];
	USHORT value_ushort;
	ULONG  value_ulong;

	// 0 - short, 1 - long offset
	if(headFont.indexToLocFormat == 1)	one_block = 4;
	else											one_block = 2;

	// 먼저 해당 글자의 위치를 얻는다.
	if((int)GetFontData(hDC, GetTtfTable("loca"), font_number*one_block, buf, one_block) == -1) {
		//sprintf(msg, "loca GetFontData return -1");
		//TextOut(hDC, 0, 0, msg, strlen(msg));
		return 0;
	}

	if(headFont.indexToLocFormat == 1) {
		memcpy(&value_ulong, buf, 4);
		offset = MOTO2IBM(value_ulong);
	}
	else {
		memcpy(&value_ushort, buf, 2);
		offset = MOTO2IBM(value_ushort);
	}

	// 다음 글자의 위치를 찾는다.
	// 이 위치에서 원하는 글자의 위치를 빼면 폰트의 크기를 얻을 수 있다.
	if((int)GetFontData(hDC, GetTtfTable("loca"), (font_number+1)*one_block, buf, one_block) == -1) {
		//sprintf(msg, "loca GetFontData(loca) return -1");
		//TextOut(hDC, 0, 0, msg, strlen(msg));
		return 0;
	}

	if(headFont.indexToLocFormat == 1) {
		memcpy(&value_ulong, buf, 4);
		size = MOTO2IBM(value_ulong)-offset;
	}
	else {
		memcpy(&value_ushort, buf, 2);
		size = MOTO2IBM(value_ushort)-offset;
		size *= 2;		// USHORT방식의 offset일때는 실제 크기에 *2를 하면 된다.
		offset *= 2;
	}

	if(size == 0) {
		//sprintf(msg, "this font size = 0");
		//TextOut(hDC, 0, 0, msg, strlen(msg));
		return 0;
	}

	return 1;
}

int ttfClass :: GetData(USHORT glyph_number)
{
//	char	msg[160];
	DWORD pos= 0;

	USHORT instructionLength;
	BYTE	 *instructions;
	int  i, j;
	ULONG offset;
	DWORD size;
	int  repeat;
	int  posx, posy;
	USHORT u;

	FreeAllBuf();

	if(glyph_number >= headMaxp.numGlyphs) {
		return 0;
	}

	if(!GetFontOffsetAndSize(glyph_number, offset, size))	return 0;

	StackBYTE stack(size);

	if(stack.data == NULL)	return 0;

	if((int)GetFontData(hDC, GetTtfTable("glyf"), offset, stack.data, size) == -1) {
		//sprintf(msg, "glyf GetFontData return -1");
		//TextOut(hDC, 0, 0, msg, strlen(msg));
		return 0;
	}               

	pos = 0;

	memcpy(&headGlyph, &stack.data[pos], sizeof(GLYPH_HEADER));
	headGlyph.numberOfContours = MOTO2IBM(headGlyph.numberOfContours);
	headGlyph.xMin = MOTO2IBM(headGlyph.xMin);
	headGlyph.yMin = MOTO2IBM(headGlyph.yMin);
	headGlyph.xMax = MOTO2IBM(headGlyph.xMax);
	headGlyph.yMax = MOTO2IBM(headGlyph.yMax);
	pos += sizeof(GLYPH_HEADER);

	if(headGlyph.numberOfContours == -1) {	// 이부분은 보강할 필요가 있다.
		//sprintf(msg, "headGlyph.numberOfContours(보강 필요) == -1");
		//TextOut(hDC, 0, 0, msg, strlen(msg));

		USHORT glyphflags;
		USHORT glyphIndex;
                            
      nCompositeGlyphHap = 0;
		do {
			memcpy(&glyphflags, &stack.data[pos], sizeof(USHORT));
			glyphflags = MOTO2IBM(glyphflags);
			pos += 2;
			memcpy(&glyphIndex, &stack.data[pos], sizeof(USHORT));
			glyphIndex = MOTO2IBM(glyphIndex);
			pos += 2;

			if(nCompositeGlyphHap < MAX_COMPOSITE_GLYPH) {
				compositeGlyphIndex[nCompositeGlyphHap] = glyphIndex;
				nCompositeGlyphHap++;
			}

			if(glyphflags & WORD_MASK[0]) {	// ARG_1_AND_2_ARE_WORDS
				pos += 4;
			}
			else {
				pos += 2;
			}

			if(glyphflags & WORD_MASK[3]) {
				pos += 2;
			}
			else if(glyphflags & WORD_MASK[6]) {
				pos += 4;
			}
			else if(glyphflags & WORD_MASK[7]) {
				pos += 8;
			}
		} while(glyphflags & WORD_MASK[5]);

		return nCompositeGlyphHap;
	}

	endPtsOfContours = new USHORT[headGlyph.numberOfContours];
	for(i = 0; i < headGlyph.numberOfContours; i++) {
		 memcpy(&endPtsOfContours[i], &stack.data[pos], sizeof(USHORT));
		 endPtsOfContours[i] = MOTO2IBM(endPtsOfContours[i]);
		 pos += sizeof(USHORT);
	}

	memcpy(&instructionLength, &stack.data[pos], sizeof(USHORT));
	instructionLength = MOTO2IBM(instructionLength);
	pos += sizeof(USHORT);

	instructions = new BYTE[instructionLength];
	for(u = 0; u < instructionLength; u++) {
		instructions[u] = stack.data[pos];
		pos ++;
	}

	numCoordinates = endPtsOfContours[headGlyph.numberOfContours-1]+1;

	flags = new BYTE[numCoordinates];

	for(i = 0; i < numCoordinates; i++) {
		 flags[i] = stack.data[pos];
		 if(WORD_MASK[3] & flags[i]) {   	// 이비트가 ON시 지금과 같은 flag가 다음에 있는 갯수만큼 온다.
			 repeat = stack.data[pos+1];
			 for(j = 0; j < repeat; j++) {
				 flags[i+j+1] = flags[i];
			 }
			 pos += 2;
			 i += repeat;
		 }
		 else {
			 pos ++;
		 }
	 }

	 xCoordinates = new SHORT[numCoordinates];
	 yCoordinates = new SHORT[numCoordinates];

	 for(i = 0; i < numCoordinates; i++) {
		 if((WORD_MASK[1] & flags[i])) {
			 if(WORD_MASK[4] & flags[i]) {	// positive
				 xCoordinates[i] = stack.data[pos];
				 pos +=1;
			 }
			 else {     							// negative
				 xCoordinates[i] = -stack.data[pos];
				 pos +=1;
			 }
		 }
		 else {
			 if(WORD_MASK[4] & flags[i]) {	// same coordinate
				 xCoordinates[i] = 0;
			 }
			 else {									// 16 bit signed value
				 memcpy(&xCoordinates[i], &stack.data[pos], 2);
				 xCoordinates[i] = MOTO2IBM(xCoordinates[i]);
				 pos += 2;
			 }
		 }
	 }

	 for(i = 0; i < numCoordinates; i++) {
		 if((WORD_MASK[2] & flags[i])) {
			 if(WORD_MASK[5] & flags[i]) {	// positive
				 yCoordinates[i] = stack.data[pos];
				 pos ++;
			 }
			 else {     							// negative
				 yCoordinates[i] = -stack.data[pos];
				 pos ++;
			 }
		 }
		 else {
			 if(WORD_MASK[5] & flags[i]) {	// same coordinate
				 yCoordinates[i] = 0;
			 }
			 else {									// 16 bit signed value
				 memcpy(&yCoordinates[i], &stack.data[pos], 2);
				 yCoordinates[i] = MOTO2IBM(yCoordinates[i]);
				 pos += 2;
			 }
		 }
	}

	// BasePoint를 0으로 하는 수학좌표를 실제 컴퓨터 좌표로 바꾼다.

	posx = xCoordinates[0];
	posy = yCoordinates[0];

	xCoordinates[0] = posx;
	yCoordinates[0] = GetHeight()-(posy+headOS2.usWinDescent);

	for(i = 1; i < numCoordinates; i++) {
		posx += xCoordinates[i];
		posy += yCoordinates[i];

		xCoordinates[i] = posx;
		yCoordinates[i] = GetHeight()-(posy+headOS2.usWinDescent);
	}

	delete instructions;

	return 1;
}


