#include "stdafx.h"
#include <tools.h>

// 이 파일은 라이브러리에 포함시키지 말고 직접 연결하여 사용할 것 (영문/한글 문제)
// 1.1 이상인 것을 체크한다.

bool DotNetCheck(HWND hwnd)
{
	char directory[_MAX_PATH];
	GetWindowsRootDirectory(directory, sizeof(directory));
	CString filename;

	filename.Format("%s\\Microsoft.NET\\Framework\\*.*", directory);

	CFileFind finder;
    BOOL bWorking = finder.FindFile(filename); 

	int major, minor, build;
	bool another_flag = false; 
	CommaBlockString comma;
	comma.SetBlockCode('.');

#define	NEED_FRAMEWORK_MAJOR	1
#define NEED_FRAMEWORK_MINOR	1

    while (bWorking)
    {
		bWorking = finder.FindNextFile();
		if(!finder.IsDirectory())	continue;
		strcpy(directory, finder.GetFileName());
		if(directory[0] != 'v')	continue;
		comma.Set(&directory[1]);
		if(comma.IsEOS())	continue;
		comma.GetInt(major);
		if(comma.IsEOS())	continue;
		comma.GetInt(minor);
		if(comma.IsEOS())	continue;
		comma.GetInt(build);

		if(NEED_FRAMEWORK_MAJOR == major) {	// major 버전이 일치해야 한다.
			if(minor >= NEED_FRAMEWORK_MINOR)	return true;	// 상위 버전이거나 같은 버전은 정상
		}

		another_flag = true;
	}

	CString msg;

	if(another_flag) {
#if	defined (COMPILE_HANGUL)
		msg.Format("Microsoft.NET Framework %d.%d 버전을 설치하여야 합니다.\n\n현재 %d.%d 버전이 설치되어 있으나 대체하여 사용할 수는 없습니다.\nFramework를 설치하지 않으면 프로그램의 일부 기능을 이용할 수 없습니다.",  NEED_FRAMEWORK_MAJOR, NEED_FRAMEWORK_MINOR, major, minor);
#else
		msg.Format("Microsoft.NET Framework Version %d.%d not installed.\n\nAnother version of Micsoft.NET Framework is installed.(%d.%d)",  NEED_FRAMEWORK_MAJOR, NEED_FRAMEWORK_MINOR, major, minor);	
#endif
	}
	else {
#if	defined (COMPILE_HANGUL)
		msg.Format("Microsoft.NET Framework %d.%d 버전을 설치하여야 합니다.\nFramework를 설치하지 않으면 프로그램의 일부 기능을 이용할 수 없습니다.", NEED_FRAMEWORK_MAJOR, NEED_FRAMEWORK_MINOR);
#else
		msg.Format("Microsoft.NET Framework Version %d.%d not installed.\nYou must install the Microsoft.NET Framework.", NEED_FRAMEWORK_MAJOR, NEED_FRAMEWORK_MINOR);
#endif
	}

	MessageBox(hwnd, msg, "Microsoft.NET Framework error", MB_OK);
                                       
	return false;
}

