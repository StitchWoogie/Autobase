#include "stdafx.h"

#include <glib.h>

#include "plc_scan.h"
#include "device\commmain.h"

void PokeWordSYSTEM(GLOBAL_PORT_STRUCT *pt, WORD address, WORD value)
{
	if(pt->local.device.nDeviceStyle == DEVICE_TYPE_NETCLIENT)	return;

	pt->bufSYSTEM[address].value = value;
}

void PokeBitSYSTEM(GLOBAL_PORT_STRUCT *pt, WORD address, char flag)
{
	if(pt->local.device.nDeviceStyle == DEVICE_TYPE_NETCLIENT)	return;

	WORD word_pos = address/16;
	WORD bit_pos = address%16;

	if(flag)	pt->bufSYSTEM[word_pos].value |= WORD_MASK[bit_pos];
	else		pt->bufSYSTEM[word_pos].value &= (0xFFFF-WORD_MASK[bit_pos]);
}

void PokeBitSYSTEM(int port, WORD address, char flag)
{
	if(port >= nPortHap)	return;

	GLOBAL_PORT_STRUCT *pt = &portBuf[port];

	PokeBitSYSTEM(pt, address, flag);
}