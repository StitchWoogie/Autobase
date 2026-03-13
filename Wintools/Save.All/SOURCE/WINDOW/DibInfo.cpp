#include "stdafx.h"
#include <glib.h>

static DWORD GetDibInfoHeaderSize (BYTE  * lpDib)
{
	return ((BITMAPINFOHEADER  *) lpDib)->biSize ;
}

WORD GetDibWidth (BYTE  * lpDib)
{
	if (GetDibInfoHeaderSize (lpDib) == sizeof (BITMAPCOREHEADER))
		return (WORD) (((BITMAPCOREHEADER  *) lpDib)->bcWidth) ;
	else
		return (WORD) (((BITMAPINFOHEADER  *) lpDib)->biWidth) ;
}

WORD GetDibHeight (BYTE  * lpDib)
{
	if (GetDibInfoHeaderSize (lpDib) == sizeof (BITMAPCOREHEADER))
		return (WORD) (((BITMAPCOREHEADER  *) lpDib)->bcHeight) ;
	else
		return (WORD) (((BITMAPINFOHEADER  *) lpDib)->biHeight) ;
}

BYTE  * GetDibBitsAddr (BYTE  * lpDib)
{
	DWORD dwNumColors, dwColorTableSize ;
	WORD  wBitCount ;

	if (GetDibInfoHeaderSize (lpDib) == sizeof (BITMAPCOREHEADER))	{
		wBitCount = ((BITMAPCOREHEADER  *) lpDib)->bcBitCount ;

		if (wBitCount != 24)
			dwNumColors = 1L << wBitCount;
		else
			dwNumColors = 0;

		dwColorTableSize = dwNumColors * sizeof (RGBTRIPLE);
	}
	else {
		wBitCount = ((BITMAPINFOHEADER  *) lpDib)->biBitCount;

		if (GetDibInfoHeaderSize (lpDib) >= 36)
			dwNumColors = ((BITMAPINFOHEADER *) lpDib)->biClrUsed;
		else
			dwNumColors = 0;

		if (dwNumColors == 0) {
			if (wBitCount != 24)
				dwNumColors = 1L << wBitCount;
			else
				dwNumColors = 0 ;
		}

		dwColorTableSize = dwNumColors * sizeof (RGBQUAD) ;
	}

	return lpDib + GetDibInfoHeaderSize (lpDib) + dwColorTableSize ;
}
