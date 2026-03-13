#include <afxwin.h>

void bell(int hap)
{
	int i;

	for(i = 0; i < hap; i++)	MessageBeep(0xFFFF);
}

