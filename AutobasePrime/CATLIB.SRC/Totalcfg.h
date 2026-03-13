#pragma once

#include "..\catlib.src\DuplexDir.h"

#include <afxcmn.h>

#pragma pack(push, 1)

// TAG size O.K
void AutoBaseIniGetProjectDirectory(char *directory);
void AutoBaseIniGetProjectDirectory(CString &directory);
//void AutoBaseIniGetIniPath(char *path, int size);

// default c:/windows/autobase.cfg directory
void AutoBaseIniGetConfigDirectory(char *directory);
void AutoBaseIniGetConfigDirectory(CString &directory);

void GetLibraryDirectory(char *directory);
void SetLibraryDirectory(char *directory);

int  GetAutoBaseTestMode();
void SetAutoBaseTestMode(int mode);

int  GetAutoBaseEditEnable();
void SetAutoBaseEditEnable(int mode);

enum enumOEMTYPE {
	OEM_TYPE_AUTOBASE = 0,
	OEM_TYPE_ZIoT = 1,
	OEM_TYPE_U_YE_G = 2,		// 아이티공간 U-YE-G
    //OEM_TYPE_OPEN_SCADA = 3,	// 종성 OPEN SCADA
	OEM_TYPE_SBAS = 4,			// (주)삼원씨앤지 셈스 SAMS (Samwon Automation Management Software) -> SBAS(Samwon Building Automation System Software), Ver.10.2
	OEM_TYPE_FiveTek = 5,		// (주)파이브텍 통합관리시스템 2016-4-11
	OEM_TYPE_MBSENGSCADA = 6,	// (주)엠비에스 엔지니어링
	OEM_TYPE_KOBASAI = 7, // (주)코젠 OEM 2024-0411
};

extern int eOemType;

void AutoBaseIniGetOemProgramName(CString &name);
void AutoBaseIniGetOemCompanyName(CString &name);
void AutoBaseIniGetOemSupervisorName(char *name, int size);
void AutoBaseIniGetOemExeNameViewMain(char *name, int size);

//WORD AutoBaseIniGetKeyLockPort();
int  AutoBaseIniGetKeyLockAllCheck();

void AutoBaseIniGetUserLibraryDirectory(char *directory);
void AutoBaseIniSetUserLibraryDirectory(char *directory);

//void AutoBaseIniGetDataDirectory(char *directory);
void AutoBaseIniGetProjectDataDirectory(const char *work_dir, char *directory);
void AutoBaseIniSetProjectDataDirectory(const char *work_dir, char *directory);
void AutoBaseIniGetProjectDataLogDirectory(const char *work_dir, char *directory);
void AutoBaseIniSetProjectDataLogDirectory(const char *work_dir, char *directory);

void AutoBaseIniGetDuplexDataStruct(const char *work_dir, DUPLEX_DIR_STRUCT *dir);
void AutoBaseIniSetDuplexDataStruct(const char *work_dir, DUPLEX_DIR_STRUCT *dir);

void AutoBaseIniGetDdeServiceName(char *name, int limit);

int  AutoBaseIniGetLoadBarState();
void AutoBaseIniSetLoadBarState(int state);

int  AutoBaseIniGetPlcScanWriteTest();
void AutoBaseIniSetPlcScanWriteTest(int state);

void AutoBaseListCtrlConfigLoad(CListCtrl &m_list, const char *filename, char *section);
void AutoBaseListCtrlConfigSave(CListCtrl &m_list, const char *filename, char *section);

void AutoBaseMainListCtrlConfigLoad(CListCtrl &m_list, char *section);
void AutoBaseMainListCtrlConfigSave(CListCtrl &m_list, char *section);

void AutoBaseStudioListCtrlConfigLoad(CListCtrl &m_list, char *section);
void AutoBaseStudioListCtrlConfigSave(CListCtrl &m_list, char *section);

void AutoBasePlcScanListCtrlConfigLoad(CListCtrl &m_list, char *section);
void AutoBasePlcScanListCtrlConfigSave(CListCtrl &m_list, char *section);

void SaveRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const char *val);
void SaveRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const DWORD val);
void LoadRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const char *default_value, char *val, int limit);
void LoadRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const DWORD default_value, DWORD &val);
void LoadRegAutoBaseConfig(const char *root_name, const char *sub_name, const char *item, const int default_value, int &val);

void LoadRegAutoBaseConfigCurrentUser(const char *root_name, const char *sub_name, const char *item, const char *default_value, char *val, int limit);

void AutoBaseMakeHtmlHelpPath(char *path, char *file);

void AutoBaseIniSetMinListFrom(SYSTEMTIME *t);
void AutoBaseIniSetMinListTo(SYSTEMTIME *t);
void AutoBaseIniGetMinListFrom(SYSTEMTIME *t);
void AutoBaseIniGetMinListTo(SYSTEMTIME *t);

void ChangeUICulture();
void ChangeUICultureByPlcScan();

#pragma pack(pop)


