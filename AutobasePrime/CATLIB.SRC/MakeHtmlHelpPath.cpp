// TAG size O.K
#include "stdafx.h"
#include <stdio.h>
#include <string.h>
#include <io.h>
#include <atlbase.h>

#include <compiler.hpp>
#include <tools.h>
#include <glib.h>
#include <dataswap.h>

#include "..\catlib.src\totalcfg.h"
#include "..\catlib.src\cversion.h"

extern char sDirProgramm[MAXPATH];

CString GetFitLanguageName()
{
	char value[MAXPATH];
	LoadRegAutoBaseConfigCurrentUser("Help", "Config", "Language", "Auto", value, MAX_PATH);	

    CString sLan = value;

    if (stricmp(sLan, "Auto") == 0)
    {
        if (IsLangKorean())
        {
            sLan = "Korean";
        }
        else if (IsLangJapanese())
        {
            sLan = "Japanese";
        }
        else
        {
            sLan = "English";
        }
    }

    return sLan;
}

CString GetHelpFolder(const char *language)
{
    CString directory;

    CString default_path;
	
	default_path.Format("C:\\AutoBase\\Help\\%s", language);

	CString item;
	item.Format("Path%s", language);
	char value[MAXPATH];

    LoadRegAutoBaseConfigCurrentUser("Help", NULL, item, default_path, value, MAX_PATH);

	directory = value;

    return directory;
}

void AutoBaseMakeHtmlHelpPath(char *path, char *file)
{
	// 새 도움말 구조 autobase/help/korean 형식의 폴더에 있는 구조
	CString sLan = GetFitLanguageName();

    sprintf(path, "%s\\%s", (const char*)GetHelpFolder(sLan), file);

	/* 10.2.1 까지의 도움말 구조 Sub폴더에 있는 구조
	CString default_string;
	char directory[MAXPATH];
	char buf[80];

	double version_program;
	double version_help;

	LoadRegAutoBaseConfig("Help", NULL, "Version", "0", buf, sizeof(buf));
	version_help = atof(buf);
	sprintf(buf, "%d.%d.%d", VERSION_Major, VERSION_Minor, VERSION_Build);
	version_program = atof(buf);

	if(version_program > version_help) {
		sprintf(path, "%s\\help\\%s", sDirProgramm, file);
		if(access(path, 0) == 0)	return;
	}

	LoadRegAutoBaseConfig("Help", NULL, "Directory", "C:\\AutoHelp", directory, sizeof(directory));
	sprintf(path, "%s\\%s", directory, file);*/

	
}










