#include "stdafx.h"
#include <glib.h>

HPALETTE MakePaletteWithDac(BYTE *dac, int pal_count)
{
	LOGPALETTE *plgpl;
	HLOCAL hLocal;
	HPALETTE hPal;
	int i;

	if(pal_count == 0)	return NULL;
	if(pal_count > 256)	return NULL;

	hLocal = LocalAlloc(LMEM_MOVEABLE,
		 sizeof(LOGPALETTE) + pal_count * sizeof(PALETTEENTRY));

	if(hLocal == NULL)	return NULL;

	plgpl = (LOGPALETTE*) LocalLock(hLocal);

	plgpl->palNumEntries = pal_count;
	plgpl->palVersion = 0x300;

	for (i = 0; i < pal_count; i++) {
		 plgpl->palPalEntry[i].peRed =   dac[i*3+0];
		 plgpl->palPalEntry[i].peGreen = dac[i*3+1];
		 plgpl->palPalEntry[i].peBlue =  dac[i*3+2];
		 plgpl->palPalEntry[i].peFlags = 0;
	}

	hPal = CreatePalette(plgpl);
	LocalUnlock(hLocal);
	LocalFree(hLocal);

	return hPal;
}
