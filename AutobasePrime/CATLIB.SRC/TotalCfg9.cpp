// TAG size O.K
//#include "stdafx.h"
//#include <stdio.h>
//#include <string.h>
#include <afxwin.h>
#include <io.h>
#include <atlbase.h>

#include <compiler.hpp>
#include <tools.h>
#include <glib.h>
#include <dataswap.h>

#include "totalcfg.h"
#include "cversion.h"

extern char sDirProgramm[MAXPATH];

static const char *sectionLibrary = "Library";
static const char *sectionUserLibrary = "UserLibrary";
static const char *sectionRun =	  "Run";
static const char *keyDirectory = "Directory";
static const char *keyTestMode = "TestMode";
static const char *keyEditEnable = "EditEnable";

bool IsOsVersionVistaOrHigher()
{
	OSVERSIONINFO osvi;
	ZeroMemory(&osvi, sizeof(OSVERSIONINFO));
	osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&osvi);
	if(osvi.dwMajorVersion >= 6)	return true;	// Vista는 6이다
	return false;
}

static HKEY GetRootRegistryKey()
{
     // 6 = Vista
	if (IsOsVersionVistaOrHigher()) return HKEY_CURRENT_USER;
    else							return HKEY_LOCAL_MACHINE;
}

static void KillEndSlash(char *buf)
{
	if(strlen(buf) < 3)	return;
	int hap = (int)strlen(buf);
	if(buf[hap-1] == '\\' && buf[hap-2] != ':') {
		buf[hap-1] = 0;		
	}
}

static void AutoBaseIniGetIniPath(char *path, int size)
{
	GetWindowsDirectory(path, size);

	strcat(path, "\\autobase.ini");
}

void AutoBaseIniGetConfigDirectory(char *directory)
{
	char win_dir[MAXPATH];

	GetWindowsDirectory(win_dir, MAXPATH);
	sprintf(directory, "%s\\autobase.cfg", win_dir);
	MakeDirectory(directory);
}

void AutoBaseIniGetConfigDirectory(CString &directory)
{
	char win_dir[MAXPATH];

	GetWindowsDirectory(win_dir, MAXPATH);
	directory.Format("%s\\autobase.cfg", win_dir);
	MakeDirectory(directory);
}

void AutoBaseIniGetProjectDirectory(char *directory)
{
	LoadRegAutoBaseConfig("Work Project", NULL, "Directory", "", directory, MAXPATH);

	if(strlen(directory) > 0)	return;	// Registry에 존재한다.

	// 레지스트리에 존재하지 않으면 기존의 환경을 참조한다.
	// 8.3.0 이전의 환경
	char path[MAXPATH];
	const char *default_dir = "c:\\autobase";

	//AutoBaseIniGetIniPath(path, MAXPATH);
	GetWindowsDirectory(path, MAXPATH);
	strcat(path, "\\autobase.ini");

	directory[0] = 0;
	GetPrivateProfileString("Work Project", keyDirectory, NULL, directory, MAXPATH, path);

	if(directory[0] == NULL) {
		strcpy(directory, default_dir);
		return;
	}

	KillEndSlash(directory);

	if(access(directory, 0) != 0) {	// 디렉토리가 존재하지 않을 때도 default디렉토리로 바꾼다.
		strcpy(directory, default_dir);
		return;
	}
}

void AutoBaseIniGetProjectDirectory(CString &str)
{
	StackChar dir(1000);
	AutoBaseIniGetProjectDirectory(dir.data);
	str = dir.data;
}

static void AutoBaseIniGetDataDirectory(char *directory)
{
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	directory[0] = 0;
	GetPrivateProfileString("Data", "sDirData", "C:\\CATDATA", directory, MAXPATH, path);

	KillEndSlash(directory);
}

void LoadRegAutoBaseConfigCurrentUser(const char *root_name, const char *sub_name, const char *item, const char *default_value, char *val, int limit)
{
	CRegKey reg;
	CString main_key_path;
	char  svalue[256];
	DWORD size;
	
	if(sub_name == NULL || strlen(sub_name) == 0) 
		main_key_path.Format("Software\\AutoBase\\%s", root_name); 
	else
		main_key_path.Format("Software\\AutoBase\\%s\\%s", root_name, sub_name); 

	if(reg.Open(HKEY_CURRENT_USER, main_key_path) == ERROR_SUCCESS) {
	
		size = sizeof(svalue);

		if(reg.QueryStringValue(item, svalue, &size) == ERROR_SUCCESS)
		{
			if((int)strlen(svalue) >= limit) {
				strncpy(val, svalue, limit-1);
				val[limit-1] = 0;
			}
			else {
				strcpy(val, svalue);
			}
		}
		else
			strcpy(val, default_value);

		reg.Close();
	}
	else {
		strcpy(val, default_value);
	}
}

void LoadRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const char *default_value, char *val, int limit)
{
	CRegKey reg;
	CString main_key_path;
	char  svalue[256];
	DWORD size;
	
	if(sub_name == NULL || strlen(sub_name) == 0) 
		main_key_path.Format("Software\\AutoBase\\%s", root_name); 
	else
		main_key_path.Format("Software\\AutoBase\\%s\\%s", root_name, sub_name); 

	if(reg.Open(GetRootRegistryKey(), main_key_path) == ERROR_SUCCESS) {
	
		size = sizeof(svalue);

		if(reg.QueryStringValue(item, svalue, &size) == ERROR_SUCCESS)
		{
			if((int)strlen(svalue) >= limit) {
				strncpy(val, svalue, limit-1);
				val[limit-1] = 0;
			}
			else {
				strcpy(val, svalue);
			}
		}
		else
			strcpy(val, default_value);

		reg.Close();
	}
	else {
		strcpy(val, default_value);
	}
}

void LoadRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const DWORD default_value, DWORD &val)
{
	CRegKey reg;
	CString main_key_path;
	DWORD dvalue;
	
	if(sub_name == NULL) 
		main_key_path.Format("Software\\AutoBase\\%s", root_name); 
	else
		main_key_path.Format("Software\\AutoBase\\%s\\%s", root_name, sub_name); 

	if(reg.Open(GetRootRegistryKey(), main_key_path) == ERROR_SUCCESS) {
	
		if(reg.QueryDWORDValue(item, dvalue) == ERROR_SUCCESS)
			val = dvalue;
		else
			val = default_value;

		reg.Close();
	}
	else {
		val = default_value;
	}
}

void LoadRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const int default_value, int &val)
{
	DWORD dword_val;
	LoadRegAutoBaseConfig(root_name, sub_name, item, default_value, dword_val);
	val = dword_val;
}

void SaveRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const char *val)
{
	CRegKey reg;
	CString main_key_path;
	
	if(sub_name == NULL)
		main_key_path.Format("Software\\AutoBase\\%s", root_name);
	else
		main_key_path.Format("Software\\AutoBase\\%s\\%s", root_name, sub_name);

	reg.Create(GetRootRegistryKey(), main_key_path);
	
	reg.SetStringValue(item, val);

	reg.Close();
}

void SaveRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const DWORD val)
{
	CRegKey reg;
	CString main_key_path;
	
	if(sub_name == NULL)
		main_key_path.Format("Software\\AutoBase\\%s", root_name);
	else
		main_key_path.Format("Software\\AutoBase\\%s\\%s", root_name, sub_name);

	reg.Create(GetRootRegistryKey(), main_key_path);
	
	reg.SetDWORDValue(item, val);

	reg.Close();
}

static void LoadRegAutoBaseProjectConfig(const char *work_dir, const char *item, const char *default_value, char *val, int limit)
{
	LoadRegAutoBaseConfig("Project", work_dir, item, default_value, val, limit);
}

static void SaveRegAutoBaseProjectConfig(const char *work_dir, const char *item, const char *val)
{
	SaveRegAutoBaseConfig("Project", work_dir, item, val);
}

void AutoBaseIniGetProjectDataDirectory(const char *work_dir, char *directory)
{
	LoadRegAutoBaseProjectConfig(work_dir, "DirData", "", directory, MAXPATH);
	if(strlen(directory) == 0) {	// 7.90 이전의 환경
		AutoBaseIniGetDataDirectory(directory);
	}
}

void AutoBaseIniGetProjectDataLogDirectory(const char *work_dir, char *directory)
{
	LoadRegAutoBaseProjectConfig(work_dir, "DirDataLog", "", directory, MAXPATH);
	if(strlen(directory) == 0) {
		AutoBaseIniGetProjectDataDirectory(work_dir, directory);
	}
}

void AutoBaseIniSetProjectDataDirectory(const char *work_dir, char *directory)
{
	SaveRegAutoBaseProjectConfig(work_dir, "DirData", directory);
}

void AutoBaseIniSetProjectDataLogDirectory(const char *work_dir, char *directory)
{
	SaveRegAutoBaseProjectConfig(work_dir, "DirDataLog", directory);
}



void AutoBaseIniGetDuplexDataStruct(const char *work_dir, DUPLEX_DIR_STRUCT *dir)
{
	char imsi[80];
	LoadRegAutoBaseProjectConfig(work_dir, "DuplexDirUse", "0", imsi, sizeof(imsi));
	dir->bUse = atoi(imsi);
	LoadRegAutoBaseProjectConfig(work_dir, "DuplexDirPrimary", "C:\\PRIMARY", dir->sPrimary, sizeof(dir->sPrimary));
	LoadRegAutoBaseProjectConfig(work_dir, "DuplexDirSecondary", "C:\\SECONDARY", dir->sSecondary, sizeof(dir->sSecondary));
}

void AutoBaseIniSetDuplexDataStruct(const char *work_dir, DUPLEX_DIR_STRUCT *dir)
{
	CString buf;
	buf.Format("%d", dir->bUse);
	SaveRegAutoBaseProjectConfig(work_dir, "DuplexDirUse", buf);
	SaveRegAutoBaseProjectConfig(work_dir, "DuplexDirPrimary", dir->sPrimary);
	SaveRegAutoBaseProjectConfig(work_dir, "DuplexDirSecondary", dir->sSecondary);
}

void GetLibraryDirectory(char *directory)
{
	LoadRegAutoBaseConfig(sectionLibrary, NULL, keyDirectory, "", directory, MAXPATH);

	if(strlen(directory) > 0)	return;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];
	AutoBaseIniGetIniPath(path, MAXPATH);

	directory[0] = 0;
	GetPrivateProfileString(sectionLibrary, keyDirectory, "NULL", directory, MAXPATH, path);

	// 이름이 설치 되지 않았을 때는 default디렉토리로 바꾼다.
	if(strcmp(directory, "NULL") == 0) {
		strcpy(directory, "c:\\autobase.lib");
		return;
	}

	KillEndSlash(directory);
}

void SetLibraryDirectory(char *directory)
{
	SaveRegAutoBaseConfig(sectionLibrary, NULL, keyDirectory, directory);

	// 8.3.0 이전의 환경
	//char path[MAXPATH];
	//AutoBaseIniGetIniPath(path, MAXPATH);
	//WritePrivateProfileString(sectionLibrary, keyDirectory, directory, path);
}

void AutoBaseIniGetUserLibraryDirectory(char *directory)
{
	LoadRegAutoBaseConfig(sectionUserLibrary, NULL, keyDirectory, "", directory, MAXPATH);

	if(strlen(directory) > 0)	return;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	directory[0] = 0;
	GetPrivateProfileString(sectionUserLibrary, keyDirectory, "NULL", directory, MAXPATH, path);

	// 이름이 설치 되지 않았을 때는 default디렉토리로 바꾼다.
	if(strcmp(directory, "NULL") == 0) {
		strcpy(directory, "c:\\AutoUser.lib");
		return;
	}

	KillEndSlash(directory);
}

void AutoBaseIniSetUserLibraryDirectory(char *directory)
{
	SaveRegAutoBaseConfig(sectionUserLibrary, NULL, keyDirectory, directory);

	// 8.3.0 이전의 환경
	//char path[MAXPATH];

	//AutoBaseIniGetIniPath(path, MAXPATH);

	//WritePrivateProfileString(sectionUserLibrary, keyDirectory, directory, path);
}

/*
int GetAutoBaseTestMode()
{
	char buf[80];

	LoadRegAutoBaseConfig(sectionRun, NULL, keyTestMode, "", buf, sizeof(buf));

	if(strlen(buf) == 0) {
		// 8.3.0 이전의 환경
		char path[MAXPATH];

		AutoBaseIniGetIniPath(path, MAXPATH);

		buf[0] = 0;
		GetPrivateProfileString(sectionRun, keyTestMode, "ON", buf, sizeof(buf), path);
	}

	if(strcmp(buf, "ON") == 0) {
		return 1;
	}
	else {
		return 0;
	}
}

void SetAutoBaseTestMode(int mode)
{
	if(mode)
		SaveRegAutoBaseConfig(sectionRun, NULL, keyTestMode, "ON");	
	else
		SaveRegAutoBaseConfig(sectionRun, NULL, keyTestMode, "OFF");	


}

int GetAutoBaseEditEnable()
{
	char buf[80];

	LoadRegAutoBaseConfig(sectionRun, NULL, keyEditEnable, "", buf, sizeof(buf));

	if(strlen(buf) == 0) {
		// 8.3.0 이전의 환경
		char path[MAXPATH];
		AutoBaseIniGetIniPath(path, MAXPATH);
		buf[0] = 0;
		GetPrivateProfileString(sectionRun, keyEditEnable, "ON", buf, sizeof(buf), path);
	}

	if(strcmp(buf, "ON") == 0) {
		return 1;
	}
	else {
		return 0;
	}
}

void SetAutoBaseEditEnable(int mode)
{
	if(mode)
		SaveRegAutoBaseConfig(sectionRun, NULL, keyEditEnable, "ON");
	else
		SaveRegAutoBaseConfig(sectionRun, NULL, keyEditEnable, "OFF");


}*/

//------------------------------------------
// [OEM] section
// program name = oem program name
//------------------------------------------

static const char *sectionOEM = "OEM";

int eOemType = OEM_TYPE_AUTOBASE;

void GetProgramConfigFilename(CString &path)
{
	path.Format("%s\\Config\\Program.inix", sDirProgramm);
}

void AutoBaseIniGetOemType()
{
	CString path;

	GetProgramConfigFilename(path);

	CString name;
	GetProfileStringFromUTF8(sectionOEM, "Type", "", name, path);

	if(name == "ZIoT")
		eOemType = OEM_TYPE_ZIoT;
	else if(name == "SBAS")
		eOemType = OEM_TYPE_SBAS;
	else if(name == "FiveTek")
		eOemType = OEM_TYPE_FiveTek;
	else if(name == "U-YE-G")
		eOemType = OEM_TYPE_U_YE_G;
	else if(name == "MBSENGSCADA")
		eOemType = OEM_TYPE_MBSENGSCADA;
	else if(name == "KobasAI")
		eOemType = OEM_TYPE_KOBASAI;
	else
		eOemType = OEM_TYPE_AUTOBASE;
}

void AutoBaseIniGetVersionString(CString &version)
{
	CString path;

	GetProgramConfigFilename(path);
	
	bool bVersionCertificationSmallBussinessProducts = false;	// 중소기업 제품 인증 버전이면 true

	if(bVersionCertificationSmallBussinessProducts || eOemType == OEM_TYPE_SBAS) 
		version.Format("%d.%d", VERSION_Major, VERSION_Minor);
	else 
		version.Format("%d.%d.%d", VERSION_Major, VERSION_Minor, VERSION_Build);

	if(eOemType == OEM_TYPE_SBAS) {
		GetProfileStringFromUTF8(sectionOEM, "Version", version, version, path);
	}
	else if(eOemType == OEM_TYPE_FiveTek) {
		GetProfileStringFromUTF8(sectionOEM, "Version", version, version, path);
	}
	else if(eOemType == OEM_TYPE_U_YE_G) {
		GetProfileStringFromUTF8(sectionOEM, "Version", version, version, path);
	}
}

void AutoBaseIniGetOemProgramName(CString &name)
{
	CString path;

	GetProgramConfigFilename(path);

	GetProfileStringFromUTF8(sectionOEM, "Program name", "AutoBase", name, path);
}

void AutoBaseIniGetOemCompanyName(CString &name)
{
	CString path;

	GetProgramConfigFilename(path);

	if(IsLangKorean()) {
		GetProfileStringFromUTF8(sectionOEM, "Company name", "(주)오토베이스", name, path);
	} else {
		GetProfileStringFromUTF8(sectionOEM, "Company name", "AUTOBASE, INC.", name, path);
	}
}

/*
void AutoBaseIniGetOemSupervisorName(char *name, int size)
{
	strcpy(name, "ADMIN");

}

void AutoBaseIniGetOemExeNameViewMain(char *name, int size)
{
	LoadRegAutoBaseConfig(sectionOEM, NULL, "Exe ViewMain", "", name, size);
	if(strlen(name) > 0)	return;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	GetPrivateProfileString(sectionOEM, "Exe ViewMain", "AUTOBASE.EXE", name, size, path);
}

int AutoBaseIniGetKeyLockAllCheck()
{
	char val[80];
	LoadRegAutoBaseConfig("KeyLock", NULL, "AllCheck", "9999", val, sizeof(val));

	if(strcmp(val, "9999") != 0) {
		if(stricmp(val, "true") == 0)	return 1;
		else return 0;
		// Registry에 존재한다.
	}

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	return GetPrivateProfileInt("KeyLock", "AllCheck", 0, path);
}

int AutoBaseIniGetKeyLockWait()
{
	DWORD val;
	LoadRegAutoBaseConfig("KeyLock", NULL, "Wait", 9999, val);
	if(val != 9999)	return val;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	return GetPrivateProfileInt("KeyLock", "Wait", 0, path);
}

void AutoBaseIniGetDdeServiceName(char *name, int limit)
{
	LoadRegAutoBaseConfig("DDE", NULL, "Service", "", name, limit);
	if(strlen(name) > 0)	return;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	GetPrivateProfileString("DDE", "Service", "AUTOBASE", name, limit, path);
	strupr(name);
}

int  AutoBaseIniGetLoadBarState()
{
	DWORD val;
	LoadRegAutoBaseConfig("ToolBar", NULL, "LoadBarState", 9999, val);
	if(val != 9999)	return val;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	return GetPrivateProfileInt("ToolBar", "LoadBarState", 1, path);
}

void AutoBaseIniSetLoadBarState(int state)
{
	SaveRegAutoBaseConfig("ToolBar", NULL, "LoadBarState", state);

	// 8.3.0 이전의 환경
	//char path[MAXPATH];

	//AutoBaseIniGetIniPath(path, MAXPATH);

	//WritePrivateProfileInt("ToolBar", "LoadBarState", state, path);
}*/

int  AutoBaseIniGetPlcScanWriteTest()
{
	DWORD val;
	LoadRegAutoBaseConfig("PlcScan", NULL, "bPlcScanWriteTest", 9999, val);
	if(val != 9999)	return val;	// Registry에 존재한다.

	// 8.3.0 이전의 환경
	char path[MAXPATH];

	AutoBaseIniGetIniPath(path, MAXPATH);

	return GetPrivateProfileInt("PlcScan", "bPlcScanWriteTest", 1, path);
}

void AutoBaseIniSetPlcScanWriteTest(int state)
{
	SaveRegAutoBaseConfig("PlcScan", NULL, "bPlcScanWriteTest", state);
}

void AutoBaseListCtrlConfigLoad(CListCtrl &m_list, const char *filename, char *section)
{
	StackChar cfg_dir(MAX_PATH);
	CString filepath;
	CString item;
	UINT value;
	int i;
	LVCOLUMN lv;
	
	AutoBaseIniGetConfigDirectory(cfg_dir.data);
	filepath.Format("%s\\ListCtrlConfig\\%s", cfg_dir.data, filename);

	ZeroMemory(&lv, sizeof(LVCOLUMN));
	lv.mask = LVCF_WIDTH;

	for(i = 0; m_list.GetColumn(i, &lv); i++) {
		item.Format("Item%d", i);
		value = GetPrivateProfileInt(section, item, 0, filepath);
		if(value != 0) {
			if(value > 1000)	value = 50;
			if(value < 10)		value = 10;
			m_list.SetColumnWidth(i, value);
		}
	}
}

void AutoBaseListCtrlConfigSave(CListCtrl &m_list, const char *filename, char *section)
{
	StackChar cfg_dir(MAX_PATH);
	CString filepath;
	CString item;
	int i;
	LVCOLUMN lv;
	
	AutoBaseIniGetConfigDirectory(cfg_dir.data);
	filepath.Format("%s\\ListCtrlConfig", cfg_dir.data);
	if(!MakeDirectory(filepath))	return;

	filepath.Format("%s\\ListCtrlConfig\\%s", cfg_dir.data, filename);

	ZeroMemory(&lv, sizeof(LVCOLUMN));
	lv.mask = LVCF_WIDTH;

	for(i = 0; m_list.GetColumn(i, &lv); i++) {
		item.Format("Item%d", i);
		WritePrivateProfileInt(section, item, lv.cx, filepath);
	}
}

void AutoBaseMainListCtrlConfigLoad(CListCtrl &m_list, char *section)
{
	AutoBaseListCtrlConfigLoad(m_list, "AutoBaseMain.ini", section);
}

void AutoBaseMainListCtrlConfigSave(CListCtrl &m_list, char *section)
{
	AutoBaseListCtrlConfigSave(m_list, "AutoBaseMain.ini", section);
}

void AutoBaseStudioListCtrlConfigLoad(CListCtrl &m_list, char *section)
{
	AutoBaseListCtrlConfigLoad(m_list, "AutoBaseStudio.ini", section);
}

void AutoBaseStudioListCtrlConfigSave(CListCtrl &m_list, char *section)
{
	AutoBaseListCtrlConfigSave(m_list, "AutoBaseStudio.ini", section);
}

void AutoBasePlcScanListCtrlConfigLoad(CListCtrl &m_list, char *section)
{
	AutoBaseListCtrlConfigLoad(m_list, "AutoBasePlcScan.ini", section);
}

void AutoBasePlcScanListCtrlConfigSave(CListCtrl &m_list, char *section)
{
	AutoBaseListCtrlConfigSave(m_list, "AutoBasePlcScan.ini", section);
}

/*
void AutoBaseIniSetMinListFrom(SYSTEMTIME *t)
{
	CString buf;
	buf.Format("%d,%d,%d,%d,%d,", t->wYear, t->wMonth, t->wDay, t->wHour, t->wMinute);
	SaveRegAutoBaseConfig("Reporter", NULL, "MinListFrom", 	buf);
}

void AutoBaseIniSetMinListTo(SYSTEMTIME *t)
{
	CString buf;
	buf.Format("%d,%d,%d,%d,%d,", t->wYear, t->wMonth, t->wDay, t->wHour, t->wMinute);
	SaveRegAutoBaseConfig("Reporter", NULL, "MinListTo", 	buf);
}

void AutoBaseIniGetMinListFrom(SYSTEMTIME *t)
{
	GetLocalTime(t);

	CString default_buf;
	default_buf.Format("%d,%d,%d,%d,%d,", t->wYear, t->wMonth, t->wDay, t->wHour, t->wMinute);
	char buf[80];
	LoadRegAutoBaseConfig("Reporter", NULL, "MinListFrom", default_buf, buf, sizeof(buf));

	CommaBlockString comma;
	comma.Set(buf);
	comma.GetWORD(t->wYear);
	comma.GetWORD(t->wMonth);
	comma.GetWORD(t->wDay);
	comma.GetWORD(t->wHour);
	comma.GetWORD(t->wMinute);
}

void AutoBaseIniGetMinListTo(SYSTEMTIME *t)
{
	GetLocalTime(t);

	CString default_buf;
	default_buf.Format("%d,%d,%d,%d,%d,", t->wYear, t->wMonth, t->wDay, t->wHour, t->wMinute);
	char buf[80];
	LoadRegAutoBaseConfig("Reporter", NULL, "MinListTo", default_buf, buf, sizeof(buf));

	CommaBlockString comma;
	comma.Set(buf);
	comma.GetWORD(t->wYear);
	comma.GetWORD(t->wMonth);
	comma.GetWORD(t->wDay);
	comma.GetWORD(t->wHour);
	comma.GetWORD(t->wMinute);
}
*/

