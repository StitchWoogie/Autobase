#pragma once

#pragma pack(push, 1)
/*
#define	SIZE_PROTECT_STRUCT	500

typedef struct {
	bool bAllRights;				// 관리자가 로그인 하면 모든권한이 생긴다.
	DWORD dwPasswordCRC;
	bool bRightProgrammEnd;			// programm end right
	bool bRightTagChange;			// Tag 수정 권한
	bool bRightConfigAlarm;			// 환경설정 수정 권한.
	bool bRightConfigEtc;			// 기타환경 수정 권한.
	bool bRightTagMemberAlarmLevel;	// 경보 레벨
	bool bRightTagMemberDataSave;
	bool bRightAlarmConfirm;		// 원래 이자리는 디지탈 출력을 ON/OFF하는 권한

	bool bRightTagMemberViewRange;	// View-Full View-Base
	bool bRightConfigData;			// 
	bool bRightScript;				// 
	bool bRightPlcScanEdit;			// 
	bool bRightTagMemberAlarmActive;// 태그의 경보여부

	bool bRightHandInput;			// 수동기입 여부

	char reserved[482];				// 공간
} SHARE_PROTECT_STRUCT;
*/

enum {
	RIGHT_PROGRAMM_END = 0,			// programm end right
	RIGHT_TAG_CHANGE = 1,			// tag의 속성을 바꿀수 있는 권한
	RIGHT_CONFIG_ALARM = 2,
	RIGHT_CONFIG_ETC = 3,
	RIGHT_ALARM_CONFIRM = 4,
	RIGHT_CONFIG_DATA = 5,
	RIGHT_SCRIPT = 6,
	RIGHT_PLCSCAN_EDIT = 7,
	RIGHT_HAND_INPUT = 8,
	
	RIGHT_IS_ADMIN = 999,
	RIGHT_TAG_MEMBER_ALARM_LEVEL = 1000,
	RIGHT_TAG_MEMBER_DATASAVE = 1001,
	RIGHT_TAG_MEMBER_VIEW_RANGE = 1002,
	RIGHT_TAG_MEMBER_ALARM_ACTIVE = 1003,
};

bool ShareProtectHaveRights(int id);

#pragma pack(pop)
