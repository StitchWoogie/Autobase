#include "stdafx.h"
#include <tools.h>
#include <io.h>

#if defined(__BORLANDC__)
FindFileClass :: FindFileClass()
{

}

FindFileClass :: ~FindFileClass()
{

}

int FindFileClass :: findfirst(char *pathname, struct ffblk *blk, int attrib)
{
	return ::findfirst(pathname, blk, attrib);
}

int FindFileClass :: findnext(struct ffblk *blk)
{
	return ::findnext(blk);
}
#else
FindFileClass :: FindFileClass()
{
	handle = -1;
}

FindFileClass :: ~FindFileClass()
{
	if(handle != -1)	_findclose(handle);
}

//#define _A_NORMAL	0x00	// Normal file - No read/write restrictions 
//#define _A_RDONLY	0x01	// Read only file 
//#define _A_HIDDEN	0x02	// Hidden file 
//#define _A_SYSTEM	0x04	// System file 
//#define _A_SUBDIR	0x10	// Subdirectory 
//#define _A_ARCH 	0x20	// Archive file 

//--------------------------------------------------------------------------------------------
// ymdhms ¸¦ WORD·Î ¹Ù²Û´Ù.
//--------------------------------------------------------------------------------------------

void FillDosDateTime(WORD *date, WORD *time, int year, int month, int day, int hour, int min, int sec)
{
	*date = ((WORD)(year-1980) << 9) | ((WORD)month << 5) | (WORD)day;
	*time = ((WORD)hour << 11) | ((WORD)min << 5) | ((WORD)(sec/2));

	year = ((*date) >> 9)+1980;
	month = ((*date) >> 5) & 0x000F;
	day = ((*date) >> 0) & 0x001F;
	hour = ((*time) >> 11) & 0x001F;
	min = ((*time) >> 5) & 0x003F;
	sec = ((*time) ) & 0x001F;
	sec = sec*2;
}

static void finddata2ffblk(struct ffblk32 *blk, struct _finddata_t *data)
{
	memset(blk, 0, sizeof(struct ffblk32));

	strcpy(blk->ff_name, data->name);
	blk->ff_fsize = data->size;

	if(data->attrib & _A_SUBDIR)	blk->ff_attrib |= FA_DIREC;
	//if(data->attrib & _A_SUBDIR)	ffblk->ff_attrib |= FA_DIREC;
	//if(data->attrib & _A_SUBDIR)	ffblk->ff_attrib |= FA_DIREC;
	//if(data->attrib & _A_SUBDIR)	ffblk->ff_attrib |= FA_DIREC;
	//if(data->attrib & _A_SUBDIR)	ffblk->ff_attrib |= FA_DIREC;
	//if(data->attrib & _A_SUBDIR)	ffblk->ff_attrib |= FA_DIREC;

	if(data->time_write == -1) {
		data->time_write = 0;
	}
	
	CTime time(data->time_write);

	FillDosDateTime(&blk->ff_fdate, &blk->ff_ftime,
		            time.GetYear(), time.GetMonth(), time.GetDay(), time.GetHour(), time.GetMinute(), time.GetSecond());
}

int FindFileClass :: findfirst(const char *pathname, struct ffblk32 *blk, int attrib)
{
	struct _finddata_t c_file;

	handle = _findfirst((char*)pathname, &c_file);

	if(handle == -1)	return -1;

	finddata2ffblk(blk, &c_file);
	return 0;
}

int FindFileClass :: findnext(struct ffblk32 *blk)
{
	struct _finddata_t c_file;
	int retn;

	retn = _findnext(handle, &c_file);

	if(retn == -1)	return -1;

	finddata2ffblk(blk, &c_file);

	return 0;
}
#endif
 