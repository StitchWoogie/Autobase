#include <afxwin.h>
#include <string.h>
#include <io.h>
#include <tchar.h>

#include <dataswap.h>

int MakeDirectory(LPCTSTR path)
{
//#if	defined (_WIN32)
//	if(_access(path, 0) == 0)	return 1;
//#else
	if(_taccess(path, 0) == 0)	return 1;
//#endif

	StackChar imsi((unsigned int) (_tcslen(path)+1));
	int i;
	int count = 0;

	if(imsi.data == NULL)	return 0;

	if(path[1] == ':')	i = 3;
	else				i = 1;

	for(;i < (int)_tcslen(path); i++) {
		if(path[i] == '\\' ||
			path[i] == '/') {
			if(count > 0) {
				count = 0;
				_tcscpy(imsi.data, path);
				imsi.data[i] = 0;

				if(_taccess(imsi.data, 0) != 0) {
					if(CreateDirectory(imsi.data, NULL) == 0)	return 0;
				}

			}
		}
		else {
			count++;
		}
	}

	if(count > 0) {
		_tcscpy(imsi.data, path);
		imsi.data[i] = 0;

		if(_taccess(imsi.data, 0) != 0) {
			if(CreateDirectory(imsi.data, NULL) == 0)	return 0;
		}

	}

	return 1;
}

