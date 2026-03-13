#include "stdafx.h"
#if	defined (__BORLANDC__)
#include <mem.h>            
#else
#include <memory.h> 
#endif

#include <string.h>

#include <cderr.h>
                          
#include <gclass.h>
#include <tools.h>

GSaveDialog ::
GSaveDialog(HWND hwnd) :
GOpenDialog(hwnd)
{

}

GSaveDialog :: ~GSaveDialog()
{

}

BOOL GSaveDialog :: Execute()
{
	
	BOOL retn;

	retn = GetSaveFileName(ofn);
	if(retn == 0L) {	// error
		ErrorMessage();
	}
	else {
		FnSplit fnsplit;
		char ext[MAXPATH];

		fnsplit.fnsplit(szFile);
		fnsplit.GetExt(ext);
		
		if(ext[0] == NULL) {	// 확장자가 없슴
			int pos = 0;
			int null = 0;
			int index = 1;
			char ok_flag = OFF;

			while(1) {											// 먼저 index의 위치를 찾는다
				if(index >= (int)ofn->nFilterIndex)	break;
				if(szFilter[pos] == NULL) {
					null++;
					if(null >= 2) {
						index++;
						null = 0;
					}
				}
				pos++;
			}
			null = 0;
			while(1) {											// filter 가 시작되는 위치를 찾는다.
				if(null >= 1)	break;
				if(szFilter[pos] == NULL) {
					null++;
				}
				pos++;
			}

			while(1) {											// filter 가 시작되는 위치를 찾는다.
				if(szFilter[pos] == NULL)	break; 
				if(szFilter[pos] == '.') {
					ok_flag = ON;
					break;
				}
				pos++;
			}	
			if(ok_flag) {
				strcat(szFile, &szFilter[pos]);
			}
		}
	}

	return retn;
}

void GSaveDialog::SetOverWritePrompt()
{
	ofn->Flags |= OFN_OVERWRITEPROMPT;
}


