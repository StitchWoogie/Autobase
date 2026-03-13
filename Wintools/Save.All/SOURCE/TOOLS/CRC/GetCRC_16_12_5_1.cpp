// english O.K
/************************************************************************
*									*
*	PACKAGE     SAIA PCD COMMUNICATIONS DLL FOR MS-WINDOWS		*
*	MODULE	    WCOMCRC.C						*
*	AUTHOR	    Matt Harvey 					*
*									*
*	Copyright (C) SAIA AG, CH-3280 Murten, 1993			*
*									*
*************************************************************************

REVISION HISTORY
29-Jun-93   Initial edit.
15-Jun-95   CrcCalc() now initializes the table if it's not
	    already initialized.

DESCRIPTION
Computes CRC-CCITT using the polynomial:  X^16 + X^12 + X^5 + 1

A fast look-up table method is used. This requires that the look-up
table is initialized using a call to CrcInit() BEFORE CrcCalc()
can be called.
*/
#include "stdafx.h"
#include <windows.h>


static WORD CrcTable[256];	/* Look-up table */
static BYTE zero;
static BOOL bInit;		/* TRUE=CrcTable has been initialized */

/* Compute CRC using standard method (for look-up table only) */

WORD PASCAL _CrcCalc(
WORD     crc, 
PBYTE    bp, 
int      len)
         {
         int       i;

         while (len--) 
            {
	        crc ^= (BYTE)(*bp++) << 8;
	        for (i = 0; i < 8; ++i) 
		        {
	            if (crc & 0x8000)
		           crc = LOWORD((crc << 1) ^ 0x1021);
	            else
		           crc <<= 1;
	            }
            }
         return(crc);
         }


/* Initialize the look-up table */

void PASCAL CrcInit(void)
         {
         int       i;

         bInit = TRUE;
         zero = 0;
         for (i = 0; i < 256; ++i)
	         CrcTable[i] = _CrcCalc(LOWORD(i << 8), &zero, 1);
         }


/* Compute CRC using the look-up table */

WORD GetCRC_16_12_5_1(unsigned char *bp, int len)
         {
		 WORD crc = 0;

         if (!bInit)
	        CrcInit();
         while(len--)
	          crc = LOWORD(CrcTable[(crc >> 8) ^ (*bp++)] ^ (crc << 8));
         return(crc);
         }

WORD GetCRC_16_12_5_1_FFFF(unsigned char *bp, int len)
         {
		 WORD crc = 0xFFFF;

         if (!bInit)
	        CrcInit();
         while(len--)
	          crc = LOWORD(CrcTable[(crc >> 8) ^ (*bp++)] ^ (crc << 8));
         return(crc);
         }
/* End WCOMCRC.C */


/*
// 원래는 이것으로 했으나 CRC 계산이 정확하지 않으므로 PCD library에 있는 것을 사용했다.
WORD GetCRC16_12_5_1(unsigned char *buf, int size)
{
	WORD crc = 0;
	int i, j;
	WORD input;
	WORD quotient;
	WORD data;

	for(j = 0; j < size; j++) {
		data = buf[j];
		for(i = 0; i < 8; i++) {

			if(buf[j] & BIT_MASK[i]) 	input = 1;		
			else               	    	input = 0;

			quotient = crc & 0x8000;

			crc = (crc << 1) & 0xFFFE;

			if(input ^ quotient)	crc ^= 0x1021;
		}
	}

	return crc;
}
*/
