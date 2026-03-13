// TAG SIZE OK
#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <dos.h>
#include <string.h>    
#include <math.h>

#include <tools.h>
#include <glib.h>
#include <dataswap.h>
#include "..\catlib\cattag.h"
#include "..\catlib\GroupTag.h"

extern char sDirWorkProject[MAXPATH];

TERMINAL_STRUCT *terminalStruct;

#if	defined (COMPILE_HANGUL)
static char *sFileInvalid = "파일에 이상이 있습니다.";
static char *sMsgError = "오류";
#else
static char *sFileInvalid = "File wrong.";
static char *sMsgError = "Error";
#endif

double GetDisplayValue(TAG_AI_STRUCT *ai, double value)
{
	if(ai->file.nCalculateFilter == 1 || ai->file.nCalculateFilter == 3 || ai->file.nCalculateFilter == 4) {
		
		float real_gab = ai->file.full-ai->file.base;
		float view_gab = (float)fabs(ai->file.view_full);

		if(value < 0) {
			value = (float)(real_gab*fabs(value)/view_gab);
			value = value-ai->file.full;
		}
		else {
			value = (float)(real_gab*fabs(value)/view_gab);
			value = ai->file.full-value;
		}

		//if(value < ai->file.view_base)	value = ai->file.view_base;
		//if(value > ai->file.view_full)	value = ai->file.view_full;
	}

	return value;
}

//------------------------------------------------------------------
//	AI 현재값을 format대로 스트링으로 만들어 준다.
//	buf는 최소한 21개 이상은 되어야 한다.
//  모든 형식에 따른다.
//	실제 "   0.11" 형태가 된다.
//------------------------------------------------------------------

void AiValueToString(char *buf, TAG_AI_STRUCT *ai, double value)
{
	CString format_string;

	if(ai->file.fDisplayFormat > 30)
		ai->file.fDisplayFormat = 10.2f;

	format_string.Format("%%%.1ff", ai->file.fDisplayFormat);

	sprintf(buf, format_string, value);
}

void AiValueToString(CString &buf, TAG_AI_STRUCT *ai, double value)
{
	CString format_string;

	if(ai->file.fDisplayFormat > 30)
		ai->file.fDisplayFormat = 10.2f;

	format_string.Format("%%%.1ff", ai->file.fDisplayFormat);

	buf.Format(format_string, value);
}

void ValueToStringByFormat(char *buf, float format, double value)
{
	CString format_string;

	if(format > 30)
		format = 10.2f;

	format_string.Format("%%%.1ff", format);

	sprintf(buf, format_string, value);
}

//------------------------------------------------------------------
//	AI 현재값을 format대로 스트링으로 만들어 준다.
//	buf는 최소한 21개 이상은 되어야 한다.
//  소수점 이하만 형식에 따른다.
//	실제 "0.11" 형태가 된다.
//------------------------------------------------------------------

void AiValueToStringOnlyPoint(char *buf, TAG_AI_STRUCT *ai, double value)
{
	CString format_string;

	format_string.Format("%.0f", ai->file.fDisplayFormat*10);

	int point = atoi(format_string);
	point %= 10;

	format_string.Format("%%.%df", point);

	sprintf(buf, format_string, value);
}

void AiValueToStringOnlyPoint(CString &buf, TAG_AI_STRUCT *ai, double value)
{
	CString format_string;

	format_string.Format("%.0f", ai->file.fDisplayFormat*10);

	int point = atoi(format_string);
	point %= 10;

	format_string.Format("%%.%df", point);

	buf.Format(format_string, value);
}

void AiValueToStringOnlyPoint(char *buf, AI_FILE_STRUCT *ai, double value)
{
	CString format_string;

	format_string.Format("%.0f", ai->fDisplayFormat*10);

	int point = atoi(format_string);
	point %= 10;

	format_string.Format("%%.%df", point);

	sprintf(buf, format_string, value);
}

//--------------------------------------------------------------------------------------
// 두 개의 태그 이름을 비교한다.
// 태그 이름의 뒷 부분에 빈 칸이나 탭 문자은 제거하고 비교한다.
//--------------------------------------------------------------------------------------

int TagCompare(char *str1, char *str2)
{
	int hap1;
	int hap2;
	int i;

	hap1 = strlen(str1);
	hap2 = strlen(str2);

	for(i = hap1-1; i >= 0; i--) {
		if(str1[i] == 32 || str1[i] == '\t') {
			continue;
		}
		else {
			hap1 = i+1;
			goto ok1;
		}
	}
    hap1 = 0;
   ok1:

	for(i = hap2-1; i >= 0; i--) {
		if(str2[i] == 32 || str2[i] == '\t') {
			continue;
		}
		else {
			hap2 = i+1;
			goto ok2;
		}
	}
    hap2 = 0;
   ok2:

	if(hap1 == 0 || hap2 == 0)	return -1;
	if(hap1 != hap2)			return -1;

	return strncmp(str1, str2, hap1);
}

int GetTagPosAO(TERMINAL_STRUCT *ter, const char *tag_org, short &pos)
{
	int i;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(i = 0; i < ter->nAnalogOutputHap; i++) {
		if(strcmp(tag, ter->analogOutput[i].file.tag) == 0) {
			pos = i;
			return 1;
		}
	}
	pos = 0;
	return 0;
}

int GetTagPosDI(TERMINAL_STRUCT *ter, const char *tag_org, short &pos)
{
	int i;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(i = 0; i < ter->nDigitalInputHap; i++) {
		if(strcmp(tag, ter->digitalInput[i].file.tag) == 0) {
			pos = i;
			return 1;
		}
	}
	pos = 0;
	return 0;
}

int GetTagPosDO(TERMINAL_STRUCT *ter, const char *tag_org, short &pos)
{
	int i;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(i = 0; i < ter->nDigitalOutputHap; i++) {
		if(strcmp(tag, ter->digitalOutput[i].file.tag) == 0) {
			pos = i;
			return 1;
		}
	}
	pos = 0;
	return 0;
}

int GetTagPosGDO(TERMINAL_STRUCT *ter, const char *tag_org, short &pos)
{
	DWORD l;
	TAG_GDO_STRUCT grp;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(l = 0; l < blockGroupDO.GetCount(); l++) {
   		blockGroupDO.GetBlock(&grp, l);
		if(strcmp(tag, grp.tag) == 0) {
			pos = (short)l;
			return 1;
		}
	}

	pos = 0;
	return 0;
}

int GetTagPosST(TERMINAL_STRUCT *ter, const char *tag_org, short &pos)
{
	int i;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(i = 0; i < ter->nStringTagHap; i++) {
		if(strcmp(tag, ter->stringTag[i].file.tag) == 0) {
			pos = i;
			return 1;
		}
	}
	pos = 0;
	return 0;
}

int GetTagTypeAndPos(TERMINAL_STRUCT *ter, const char *tag_org, int &type, short &pos)
{
	int i;
	DWORD l;
	TAG_GDO_STRUCT grp;
	char tag[80];

	strcpy(tag, tag_org);
	KillEndSpace(tag);

	for(i = 0; i < ter->nAnalogInputHap; i++) {
		if(strcmp(tag, ter->analogInput[i].file.tag) == 0) {
			pos = i;
			type = 0;	// AI
			return 1;
		}
	}
	for(i = 0; i < ter->nAnalogOutputHap; i++) {
		if(strcmp(tag, ter->analogOutput[i].file.tag) == 0) {
			pos = i;
			type = 1;	// AO
			return 1;
		}
	}

	for(i = 0; i < ter->nDigitalInputHap; i++) {
		if(strcmp(tag, ter->digitalInput[i].file.tag) == 0) {
			pos = i;
			type = 2;	// DI
			return 1;
		}
	}
	for(i = 0; i < ter->nDigitalOutputHap; i++) {
		if(strcmp(tag, ter->digitalOutput[i].file.tag) == 0) {
			pos = i;
			type = TAG_TYPE_DO;	// DO
			return 1;
		}
	}
	for(i = 0; i < ter->nStringTagHap; i++) {
		if(strcmp(tag, ter->stringTag[i].file.tag) == 0) {
			pos = i;
			type = TAG_TYPE_ST;	// STRING tag
			return 1;
		}
	}
	for(l = 0; l < blockGroupDO.GetCount(); l++) {
   		blockGroupDO.GetBlock(&grp, l);
		if(strcmp(tag, grp.tag) == 0) {
			pos = (short)l;
			type = TAG_TYPE_GDO;
			return 1;
		}
	}

	type = -1;
	pos = 0;
	return 0;
}

int GetTagTypeAndPos(int terminal, const char *tag, int &type, short &pos)
{
	TERMINAL_STRUCT *ter = &terminalStruct[terminal];

	return GetTagTypeAndPos(ter, tag, type, pos);
}

/*
//--------------------------------------------------------------------------------------------
//	TAG 이름은 10자로 구성되어야 하는데 10자가 되지 않을 때는 뒤에 space로 채워준다.
//--------------------------------------------------------------------------------------------

static void FillSpaceToTag(char *tag)
{
	int i, j;

	for(i = 0; i < 10; i++) {
		if(tag[i] == NULL) {
			for(j = i; j < 10; j++) {
				tag[j] = 0x20;
			}
			tag[10] = 0;
			return;
		}
	}
}
*/

static void CommaBufToAiFileStruct(HWND /*hwnd*/, AI_FILE_STRUCT *ai, char *buf)
{
	int number;
	CommaBlockString commaBuf;

	commaBuf.Set(buf);

	commaBuf.GetInt(number);
	commaBuf.GetString(ai->tag, sizeof(ai->tag));
	if(strncmp(ai->tag, "AMD3019", 7) == 0) {
		number = 0;
	}
	KillEndSpace(ai->tag);
	commaBuf.GetString(ai->description, sizeof(ai->description));
	commaBuf.GetInt(ai->port);
	commaBuf.GetInt(ai->station);
	commaBuf.GetWORD(ai->address);
	commaBuf.GetInt(ai->fn);
	commaBuf.GetString(ai->unit, sizeof(ai->unit));
	commaBuf.GetFloat(ai->base);
	commaBuf.GetFloat(ai->mid);
	commaBuf.GetFloat(ai->full);
	commaBuf.GetFloat(ai->ratio);
	if(ai->ratio == 0.0) {
		ai->ratio = (float)1.0;
	}
	float sp;
	commaBuf.GetFloat(sp);
	commaBuf.GetFloat(ai->hihi);
	commaBuf.GetFloat(ai->high);
	commaBuf.GetFloat(ai->low);
	commaBuf.GetFloat(ai->lolo);
	commaBuf.GetChar(ai->act);
	commaBuf.GetChar(ai->alarm);
	commaBuf.GetChar(ai->bFileSave);	// 데이터 파일을 저장할 것이냐?

	commaBuf.GetString(ai->sSubOutDigitalHiHi, MAX_TAG_NAME);
	KillEndSpace(ai->sSubOutDigitalHiHi);
	commaBuf.GetString(ai->sSubOutDigitalLoLo, MAX_TAG_NAME);
	KillEndSpace(ai->sSubOutDigitalHiHi);
	commaBuf.GetString(ai->sSubOutAnalog, MAX_TAG_NAME);
	KillEndSpace(ai->sSubOutDigitalHiHi);

	commaBuf.GetFloat(ai->fDisplayFormat);		// 디스프레이 할 방법 선택
	if(ai->fDisplayFormat == 0)	ai->fDisplayFormat = (float)10.2;
	commaBuf.GetChar(ai->cAlarmType);			// 알람 조건
	commaBuf.GetString(ai->sGraphicFile, sizeof(ai->sGraphicFile));			// 이 태그가 있는 그래픽 파일은?
	commaBuf.GetString(ai->sAlarmWaveFile, sizeof(ai->sAlarmWaveFile));		// 경보 발생시 사용할 그래픽 파일
	commaBuf.GetInt(ai->nCalculateFilter);		// 특별한 계산식을 정한다. 0 - 일반

	commaBuf.GetString(ai->sSubOutAnalogSP, MAX_TAG_NAME);		// Analog Set Point 값.
	KillEndSpace(ai->sSubOutAnalogSP);

	WORD reserved;
	commaBuf.GetWORD(reserved);		// 이전(7.0) 에는 ai->wAlarmBitON로 사용

	commaBuf.GetChar(ai->bCutOverValue);	// 아날로그 값의 계산치가 RANGE를 벗어났을 때는 값을 최대 최소로 잘라주는 옵션.
	commaBuf.GetFloat(ai->view_full);		// 보여주는 범위 high
	commaBuf.GetFloat(ai->view_base);		// 보여주는 범위 low
	commaBuf.GetInt(ai->nCalcDelay);			// 계산시 지난값과 평균하여 사용할 횟수.
	commaBuf.GetWORD(ai->wAlarmPriority); 	// 경보울리는 방법.
	commaBuf.GetChar(ai->cTagType);		// Dde를 태그로 사용할것인가?
	commaBuf.GetString(ai->sDdeService, sizeof(ai->sDdeService));
	commaBuf.GetString(ai->sDdeTopic,   sizeof(ai->sDdeTopic));
	commaBuf.GetString(ai->sDdeItem,    sizeof(ai->sDdeItem));
	commaBuf.GetChar(ai->cConfirmCount);	// 제어시 확인할 count
	commaBuf.GetChar(ai->bBcdValue);		// 메모리에서 BCD로 읽을 것인가?
	commaBuf.GetFloat(ai->fAlarmReturnGab);	// 알람에서 복귀할때 gab만큼 낮추거나 높인 수치에서 복귀한다.
	commaBuf.GetWORD(ai->wRateOfChangeLimit);	// 이전값과 비교해서 지정해 놓은 값 이상이 차이나면 경보를 울린다. 0 = 사용안함.
	commaBuf.GetHexWORD(ai->wProtectFlags);			// 0 = 자동, 1 = 수동 기입중.
	commaBuf.GetBYTE(ai->cAlarmProtectOnBigChangePercent);
	commaBuf.GetBYTE(ai->cAlarmProtectOnBigChangeSecond);
	commaBuf.GetBYTE(ai->bDdeRequest);
	commaBuf.GetWORD(ai->wScanTime);
	commaBuf.GetChar(ai->bLocalTag);

	if(ai->view_full <= ai->view_base) {	// 보여주는 range가 이상할때는 다시 초기화 해 준다.
		ai->view_full = ai->full;
		ai->view_base = ai->base;
	}

	if(ai->fDisplayFormat > 30)	ai->fDisplayFormat = 10.2f;

}

void CommaBufToTagStructAI(HWND hwnd, ANALOG_INPUT_STRUCT *ai, char *buf)
{
	CommaBufToAiFileStruct(hwnd, &ai->file, buf);
	
	ai->curr = (float)0.0;
	ai->old =  (float)0.0;
	ai->real_old = 0;
	ai->real_curr = 0;
	ai->nScanCount = 0;			// 적산을 하기위해 1분동안 변한 아나로그 횟수
	ai->fMinHap = (float)0;   			// 적산을 1분동안 더해 놓은값
	ai->fMinMin = (float)0;   			// 1분동안의 최소값
	ai->fMinMax = (float)0;   			// 1분동안의 최대값
	ai->fSumTotal = (float)0;			// 현재까지 적산치
	ai->fSumPart = (float)0; 			// 부분적인 적산치
	getdate(&ai->dSumTotal);
	getdate(&ai->dSumPart);
	gettime(&ai->tSumTotal);
	gettime(&ai->tSumPart);

	ai->cAlarmLevelStatus = LEVEL_NORMAL;		// 현재의 아나로그 레벨 상태를 알려준다.
	ai->cSubCheckLevelStatus = LEVEL_NORMAL;	// 현재의 아나로그 레벨 상태를 알려준다.
	ai->fAlarmMinValue = (float)0.0;
	ai->fAlarmMaxValue = (float)0.0;
	ai->bTagChangeFlag = ON;	// 값이 바뀌지 않는한은 다시 표시할 필요가 없다.
	ai->assign.tag[0] = NULL;	// 간접 태그일 때 사용하는 부분
	ai->assign.pos = -1;		// 태그를 Loading하면 Default는 -1이다. (not assigned)

	if(ai->file.bCutOverValue) {	//아날로그 값의 계산치가 RANGE를 벗어났을 때는 값을 최대 최소로 잘라주는 옵션.
		ai->curr = ai->file.base;
	}

	
}

static void TagAIRead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	ANALOG_INPUT_STRUCT ai;
	Block block(sizeof(TAG_AI_STRUCT));

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\AI.TAG)");
		return;
	}

#if	defined(USE_SERVMAIN)
	SmPrepareFileTag(ter->terminal, filename, "AI.TAG");
#else
	wsprintf(filename, "%s\\TAG\\AI.TAG", sDirWorkProject);
#endif

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLine(in, buf.data, 10000)) 	break;
		if(buf.data[0] == ';')	continue;
		if(strlen(buf.data) == 0)	continue;
		if(block.GetCount() >= MAX_ANALOG_INPUT) {
			break;
		}

		memset(&ai, 0, sizeof(TAG_AI_STRUCT));

		ai.nTerminal = ter->terminal;
		ai.buf_pos = (int)block.GetCount();
		
		CommaBufToTagStructAI(hwnd, &ai, buf.data);
		block.AddBlock(&ai);
	}
	fclose(in);

	if(block.GetCount() == 0)	return;	// not exist

	ter->analogInput = new TAG_AI_STRUCT[block.GetCount()];

	if(ter->analogInput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\AI.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		//block.GetBlock(&ai, l);
		memcpy(&ter->analogInput[l], block.GetPtr(l), sizeof(TAG_AI_STRUCT));
	}

	ter->nAnalogInputHap = block.GetCount();
}

static void CommaBufToAoFileStruct(AO_FILE_STRUCT *ao, char *buf)
{
	int number;
	CommaBlockString commaBuf;

	commaBuf.Set(buf);

	commaBuf.GetInt(number);
	commaBuf.GetString(ao->tag, sizeof(ao->tag));
	KillEndSpace(ao->tag);
	commaBuf.GetString(ao->description, sizeof(ao->description));
	commaBuf.GetInt(ao->port);
	commaBuf.GetInt(ao->station);
	commaBuf.GetHexDWORD(ao->address);
	commaBuf.GetString(ao->sExtraAddr, sizeof(ao->sExtraAddr));
	commaBuf.GetWORD(ao->wExtraAddr);

	commaBuf.GetInt(ao->fn);
	commaBuf.GetString(ao->unit, sizeof(ao->unit));
	commaBuf.GetFloat(ao->base);
	commaBuf.GetFloat(ao->full);

	commaBuf.GetChar(ao->act);

	char reserved;
	commaBuf.GetChar(reserved);

	commaBuf.GetInt(ao->nCalculateFilter);
	commaBuf.GetFloat(ao->plc_base);
	commaBuf.GetFloat(ao->plc_full);

	commaBuf.GetChar(reserved);		// 이전(7.0)에는 wWriteRetryTime

	commaBuf.GetChar(ao->cTagType);		// Dde를 태그로 사용할것인가?
	commaBuf.GetString(ao->sDdeService, sizeof(ao->sDdeService));
	commaBuf.GetString(ao->sDdeTopic,   sizeof(ao->sDdeTopic));
	commaBuf.GetString(ao->sDdeItem,    sizeof(ao->sDdeItem));

	commaBuf.GetChar(ao->cDdeDataFormat);
	commaBuf.GetChar(ao->bBcdValue);
	commaBuf.GetChar(ao->bCutOverValue);
	commaBuf.GetChar(ao->bLocalTag);
}

void CommaBufToTagStructAO(ANALOG_OUTPUT_STRUCT *ao, char *buf)
{
	CommaBufToAoFileStruct(&ao->file, buf);

	ao->curr = (float)0.0;
	ao->old  = (float)0.0;
	ao->real_old  = 0;
	ao->real_curr = 0;
//	ao->wWriteRetryTimeCurr = 0;
	ao->assign.tag[0] = NULL;	// 간접 태그일 때 사용하는 부분
	ao->assign.pos = -1;		// 태그를 Loading하면 Default는 -1이다. (not assigned)
}

static void TagAORead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	ANALOG_OUTPUT_STRUCT ao;
	Block block(sizeof(TAG_AO_STRUCT));

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\AO.TAG)");
		return;
	}

#if	defined(USE_SERVMAIN)
	SmPrepareFileTag(ter->terminal, filename, "AO.TAG");
#else
	wsprintf(filename, "%s\\TAG\\AO.TAG", sDirWorkProject);
#endif

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLine(in, buf.data, 10000)) 	break;
		if(buf.data[0] == ';')	continue;
		if(strlen(buf.data) == 0)	continue;
		if(block.GetCount() >= MAX_ANALOG_OUTPUT) {
			break;
		}

		memset(&ao, 0, sizeof(TAG_AO_STRUCT));

		ao.nTerminal = ter->terminal;
		ao.buf_pos = (int)block.GetCount();
		CommaBufToTagStructAO(&ao, buf.data);
		block.AddBlock(&ao);
	}
	fclose(in);

	if(block.GetCount() == 0)	return;	// not exist

	ter->analogOutput = new TAG_AO_STRUCT[block.GetCount()];

	if(ter->analogOutput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\AO.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		//block.GetBlock(&ao, l);
		memcpy(&ter->analogOutput[l], block.GetPtr(l), sizeof(TAG_AO_STRUCT));
	}

	ter->nAnalogOutputHap = block.GetCount();
}

void CommaBufToDiFileStruct(HWND /*hwnd*/, DI_FILE_STRUCT *di, char *buf)
{
	int number;
	CommaBlockString commaBuf;
	DWORD address;
	char imsi[80];

	commaBuf.Set(buf);

	commaBuf.GetInt(number);
	commaBuf.GetString(di->tag, sizeof(di->tag));
	KillEndSpace(di->tag);
	commaBuf.GetString(di->description, sizeof(di->description));
	commaBuf.GetInt(di->port);
	commaBuf.GetInt(di->station);
	commaBuf.GetHexDWORD(address);
	sprintf(imsi, "%X", address/16);
	di->address = atoi(imsi)*16+(address%16);

	commaBuf.GetInt(di->fn);
	commaBuf.GetString(di->desON, sizeof(di->desON));
	commaBuf.GetString(di->desOFF, sizeof(di->desOFF));
	commaBuf.GetChar(di->act);
	commaBuf.GetChar(di->alarm);

	commaBuf.GetString(di->sSubOutDigital1, MAX_TAG_NAME);
	KillEndSpace(di->sSubOutDigital1);
	commaBuf.GetString(di->sSubOutDigital2, MAX_TAG_NAME);
	KillEndSpace(di->sSubOutDigital2);
	commaBuf.GetString(di->sSubOutDigitalOnTag, MAX_TAG_NAME);
	KillEndSpace(di->sSubOutDigitalOnTag);
	commaBuf.GetString(di->sSubOutDigitalOffTag, MAX_TAG_NAME);
	KillEndSpace(di->sSubOutDigitalOffTag);

	commaBuf.GetChar(di->bFileSave);	// 데이터 파일을 저장할 것이냐?
	commaBuf.GetChar(di->cAlarmType);	// 데이터 파일을 저장할 것이냐?
	commaBuf.GetString(di->sGraphicFile, sizeof(di->sGraphicFile));		// 이 태그가 있는 그래픽 파일은?
	commaBuf.GetString(di->sAlarmWaveFile, sizeof(di->sAlarmWaveFile));		// 경보 발생시 사용할 그래픽 파일
	commaBuf.GetWORD(di->wAlarmPriority); 	// 경보울리는 방법.
	commaBuf.GetChar(di->cTagType);		// Dde를 태그로 사용할것인가?
	commaBuf.GetString(di->sDdeService, sizeof(di->sDdeService));
	commaBuf.GetString(di->sDdeTopic,   sizeof(di->sDdeTopic));
	commaBuf.GetString(di->sDdeItem,    sizeof(di->sDdeItem));	
	commaBuf.GetChar(di->cOutLinkMethod);	// out1, out2 선택
	commaBuf.GetChar(di->cConfirmCount);	// 제어시 확인할 count
	commaBuf.GetHexWORD(di->wProtectFlags);		// 0 = 자동, 1 = 수동 기입중.
	commaBuf.GetChar(di->bReverse);			// 0 = 정상, 1 = 반전 태그.
	commaBuf.GetBYTE(di->bDdeRequest);
	commaBuf.GetWORD(di->wScanTime);
	commaBuf.GetChar(di->bLocalTag);
}

void CommaBufToTagStructDI(HWND hwnd, DIGITAL_INPUT_STRUCT *di, char *buf)
{
	CommaBufToDiFileStruct(hwnd, &di->file, buf);

	di->curr = OFF;
	di->assign.tag[0] = NULL;	// 간접 태그일 때 사용하는 부분
	di->assign.pos = -1;		// 태그를 Loading하면 Default는 -1이다. (not assigned)
	//di->old_hour = t.ti_hour;
	//di->old_min  = t.ti_min;

}

static void TagDIRead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	DIGITAL_INPUT_STRUCT di;
	Block block(sizeof(TAG_DI_STRUCT));

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\DI.TAG)");
		return;
	}

#if	defined(USE_SERVMAIN)
	SmPrepareFileTag(ter->terminal, filename, "DI.TAG");
#else
	wsprintf(filename, "%s\\TAG\\DI.TAG", sDirWorkProject);
#endif

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLine(in, buf.data, 10000)) 	break;
		if(buf.data[0] == ';')	continue;
		if(strlen(buf.data) == 0)	continue;
		if(block.GetCount() >= MAX_DIGITAL_INPUT) {
			break;
		}

		memset(&di, 0, sizeof(TAG_DI_STRUCT));

		di.nTerminal = ter->terminal;
		di.buf_pos = (int)block.GetCount();
		CommaBufToTagStructDI(hwnd, &di, buf.data);
		block.AddBlock(&di);
	}
	fclose(in);

	if(block.GetCount() == 0)	return;	// not exist

	ter->digitalInput = new TAG_DI_STRUCT[block.GetCount()];

	if(ter->digitalInput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\DI.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		//block.GetBlock(&di, l);
		memcpy(&ter->digitalInput[l], block.GetPtr(l), sizeof(TAG_DI_STRUCT));
	}

	ter->nDigitalInputHap = block.GetCount();
}

void CommaBufToDoFileStruct(DO_FILE_STRUCT *dout, char *buf)
{
	int number;
	CommaBlockString commaBuf;

	commaBuf.Set(buf);

	commaBuf.GetInt(number);
	commaBuf.GetString(dout->tag, sizeof(dout->tag));
	KillEndSpace(dout->tag);
	commaBuf.GetString(dout->description, sizeof(dout->description));
	commaBuf.GetInt(dout->port);
	commaBuf.GetInt(dout->station);
	commaBuf.GetHexDWORD(dout->address);
	commaBuf.GetString(dout->sExtraAddr, sizeof(dout->sExtraAddr));
	commaBuf.GetWORD(dout->wExtraAddr);
	commaBuf.GetString(dout->desON, sizeof(dout->desON));
	commaBuf.GetString(dout->desOFF, sizeof(dout->desOFF));
	commaBuf.GetChar(dout->act);
	commaBuf.GetChar(dout->cRelayType);
	commaBuf.GetWORD(dout->wRelaySecTarget);
	commaBuf.GetChar(dout->bReverse);		// 이전(7.0)에는 wWriteRetryTime

	commaBuf.GetChar(dout->cTagType);		// Dde를 태그로 사용할것인가?
	commaBuf.GetString(dout->sDdeService, sizeof(dout->sDdeService));
	commaBuf.GetString(dout->sDdeTopic,   sizeof(dout->sDdeTopic));
	commaBuf.GetString(dout->sDdeItem,    sizeof(dout->sDdeItem));
	commaBuf.GetChar(dout->bLocalTag);
}

void CommaBufToTagStructDO(DIGITAL_OUTPUT_STRUCT *dout, char *buf)
{
	CommaBufToDoFileStruct(&dout->file, buf);
	
	dout->curr = OFF;
	//dout->wWriteRetryTimeCurr = 0;
	dout->assign.tag[0] = NULL;	// 간접 태그일 때 사용하는 부분
	dout->assign.pos = -1;		// 태그를 Loading하면 Default는 -1이다. (not assigned)
}

static void TagDORead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	DIGITAL_OUTPUT_STRUCT dout;
	Block block(sizeof(TAG_DO_STRUCT));

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\DO.TAG)");
		return;
	}

#if	defined(USE_SERVMAIN)
	SmPrepareFileTag(ter->terminal, filename, "DO.TAG");
#else
	wsprintf(filename, "%s\\TAG\\DO.TAG", sDirWorkProject);
#endif

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLine(in, buf.data, 10000)) 	break;
		if(buf.data[0] == ';')	continue;
		if(strlen(buf.data) == 0)	continue;
		if(block.GetCount() >= MAX_DIGITAL_OUTPUT) {
			break;
		}

		memset(&dout, 0, sizeof(TAG_DO_STRUCT));

		dout.nTerminal = ter->terminal;
		dout.buf_pos = (int)block.GetCount();
		CommaBufToTagStructDO(&dout, buf.data);
		block.AddBlock(&dout);
	}
	fclose(in);

	if(block.GetCount() == 0)	return;	// not exist

	ter->digitalOutput = new TAG_DO_STRUCT[block.GetCount()];

	if(ter->digitalOutput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\DO.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		//block.GetBlock(&dout, l);
		memcpy(&ter->digitalOutput[l], block.GetPtr(l), sizeof(TAG_DO_STRUCT));
	}

	ter->nDigitalOutputHap = block.GetCount();
}

void CommaBufToStFileStruct(TAG_ST_FILE_STRUCT *st, char *buf)
{
	int number;
	CommaBlockString commaBuf;

	commaBuf.Set(buf);

	commaBuf.GetInt(number);
	commaBuf.GetString(st->tag, sizeof(st->tag));
	KillEndSpace(st->tag);
	commaBuf.GetString(st->description, sizeof(st->description));

	commaBuf.GetChar(st->act);

	if(commaBuf.IsEOS()) {
		st->cTagType = 2;	// 메모리 태그
	}
	else {
		commaBuf.GetChar(st->cTagType);		
		commaBuf.GetInt(st->port);
		commaBuf.GetWORD(st->address);
		commaBuf.GetBYTE(st->read_size);
		commaBuf.GetBYTE(st->read_method);
		commaBuf.GetChar(st->cMemoryType);
		commaBuf.GetString(st->sDdeService, sizeof(st->sDdeService));
		commaBuf.GetString(st->sDdeTopic,   sizeof(st->sDdeTopic));
		commaBuf.GetString(st->sDdeItem,    sizeof(st->sDdeItem));	
		commaBuf.GetBYTE(st->bDdeRequest);
		commaBuf.GetChar(st->bLocalTag);
	}

	if(st->read_size <= 0)	st->read_size = 1;
}

void CommaBufToTagStructST(TAG_ST_STRUCT *st, char *buf)
{
	CommaBufToStFileStruct(&st->file, buf);
	
	st->curr[0] = 0;
}

static void TagSTRead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	TAG_ST_STRUCT st;
	Block block(sizeof(TAG_ST_STRUCT));

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\ST.TAG)");
		return;
	}

#if	defined(USE_SERVMAIN)
	SmPrepareFileTag(ter->terminal, filename, "ST.TAG");
#else
	wsprintf(filename, "%s\\TAG\\ST.TAG", sDirWorkProject);
#endif

	in = fopen(filename, "rb");
	if(in == NULL) 	return;

	while(1) {
		if(!TextGetOneLine(in, buf.data, 10000)) 	break;
		if(buf.data[0] == ';')	continue;
		if(strlen(buf.data) == 0)	continue;
		if(block.GetCount() >= MAX_STRING_TAG) {
			break;
		}

		memset(&st, 0, sizeof(TAG_ST_STRUCT));

		st.nTerminal = ter->terminal;
		st.buf_pos = (int)block.GetCount();
		CommaBufToTagStructST(&st, buf.data);
		block.AddBlock(&st);
	}
	fclose(in);

	if(block.GetCount() == 0)	return;	// not exist

	ter->stringTag = new TAG_ST_STRUCT[block.GetCount()];

	if(ter->stringTag == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\ST.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		//block.GetBlock(&st, l);
		memcpy(&ter->stringTag[l], block.GetPtr(l), sizeof(TAG_ST_STRUCT));
	}

	ter->nStringTagHap = block.GetCount();
}

#if	defined (MODE_RUN)
void PrepareSubTagAI(HWND hwnd, int terminal, int pos)
{
	ANALOG_INPUT_STRUCT *ai;
	CString message;
	TERMINAL_STRUCT *ter = &terminalStruct[terminal];

	ai = &ter->analogInput[pos];

	ai->nSubOutAnalog = -1;
	ai->nSubOutDigitalHiHi = -1;
	ai->nSubOutDigitalLoLo = -1;
	ai->nSubOutAnalogSP = -1;

	if(strlen(ai->file.sSubOutAnalog) > 0) {
		if(GetTagPosAO(ter, ai->file.sSubOutAnalog, ai->nSubOutAnalog)) {
		}
		else {
#if	defined (COMPILE_HANGUL)
			message.Format("AI 태그(%s)에 설정된 OutAnalogTag-(%s)와 일치 태그 없음", ai->file.tag, ai->file.sSubOutAnalog);
#else
			message.Format("OutAnalogTag-(%s) tag not found at AI.TAG(%s)", ai->file.sSubOutAnalog, ai->file.tag);
#endif
			MessageBox(hwnd, message, sMsgError, MB_OK);
			ai->nSubOutAnalog = -1;
		}
	}

	if(strlen(ai->file.sSubOutDigitalHiHi) > 0) {
		if(GetTagPosDO(ter, ai->file.sSubOutDigitalHiHi, ai->nSubOutDigitalHiHi)) {
			ai->cTypeSubOutDigitalHiHi = TAG_TYPE_DO;
		}
		else {
			if(GetTagPosGDO(ter, ai->file.sSubOutDigitalHiHi, ai->nSubOutDigitalHiHi)) {
				ai->cTypeSubOutDigitalHiHi = TAG_TYPE_GDO;
			}
			else {
	#if	defined (COMPILE_HANGUL)
				message.Format("AI(%s) 태그에 설정된 OutDigitalHi-(%s)와 일치 태그 없음", ai->file.tag, ai->file.sSubOutDigitalHiHi);
	#else
				message.Format("OutDigitalHi-(%s) tag is not found at AI.TAG(%s)", ai->file.sSubOutDigitalHiHi, ai->file.tag);
	#endif
				MessageBox(hwnd, message, sMsgError, MB_OK);
				ai->nSubOutDigitalHiHi = -1;
				ai->cTypeSubOutDigitalHiHi = -1;
			}
		}
	}

	if(strlen(ai->file.sSubOutDigitalLoLo) > 0) {
		if(GetTagPosDO(ter, ai->file.sSubOutDigitalLoLo, ai->nSubOutDigitalLoLo)) {
			ai->cTypeSubOutDigitalLoLo	   = TAG_TYPE_DO;
		}
		else {
			if(GetTagPosGDO(ter, ai->file.sSubOutDigitalLoLo, ai->nSubOutDigitalLoLo)) {
				ai->cTypeSubOutDigitalLoLo = TAG_TYPE_GDO;
			}
			else {
	#if	defined (COMPILE_HANGUL)
				message.Format("AI 태그에 설정된 OutDigitalLo-(%s)와 일치 태그 없음", ai->file.sSubOutDigitalLoLo);
	#else
				message.Format("OutDigitalLo-(%s) tag is not found at AI.TAG", ai->file.sSubOutDigitalLoLo);
	#endif
				MessageBox(hwnd, message, sMsgError, MB_OK);
				ai->nSubOutDigitalLoLo = -1;
				ai->cTypeSubOutDigitalLoLo = -1;
			}
		}
	}
	if(strlen(ai->file.sSubOutAnalogSP) > 0) {
		if(GetTagPosAO(ter, ai->file.sSubOutAnalogSP, ai->nSubOutAnalogSP)) {
		}
		else {
#if	defined (COMPILE_HANGUL)
			message.Format("AI(%s) 태그에 설정된 OutAnalogSP-(%s)와 일치 태그 없음", ai->file.tag, ai->file.sSubOutAnalogSP);
#else
			message.Format("OutAnalogSP-(%s) tag not found at AI.TAG(%s)", ai->file.sSubOutAnalogSP, ai->file.tag);
#endif
			MessageBox(hwnd, message, sMsgError, MB_OK);
			ai->nSubOutAnalogSP = -1;
		}
	}
}

void PrepareSubTagDI(HWND hwnd, int terminal, int pos)
{
	DIGITAL_INPUT_STRUCT *di;
	CString message;
//	int type;
    TERMINAL_STRUCT *ter = &terminalStruct[terminal];

	di = &ter->digitalInput[pos];

	di->nSubOutDigital1 = -1;
	di->nSubOutDigital2 = -1;
	di->nSubOutDigitalOnTag = -1;
	di->nSubOutDigitalOffTag = -1;

	// 입력과 연결된 출력 태그 값으로서 이출력값을 바꾸면 입력의 변화가 생긴다.
	if(strlen(di->file.sSubOutDigital1) > 0) {
		if(GetTagPosDO(ter, di->file.sSubOutDigital1, di->nSubOutDigital1)) {
			di->cTypeSubOutDigital1 = TAG_TYPE_DO;
		}
		else {
			if(GetTagPosGDO(ter, di->file.sSubOutDigital1, di->nSubOutDigital1)) {
				di->cTypeSubOutDigital1 = TAG_TYPE_GDO;
			}
			else {
#if	defined (COMPILE_HANGUL)
				message.Format("DI 태그에 설정된 OutDigital1-(%s)와 일치 태그 없음", di->file.sSubOutDigital1);
#else
				message.Format("OutDigital1-(%s) tag not found at DI.TAG", di->file.sSubOutDigital1);
#endif
				MessageBox(hwnd, message, di->file.tag, MB_OK);
				di->nSubOutDigital1 = -1;
				di->cTypeSubOutDigital1 = -1;
			}
		}
	}

	// 입력과 연결된 출력 태그 값으로서 이출력값을 바꾸면 입력의 변화가 생긴다.
	if(strlen(di->file.sSubOutDigital2) > 0) {
		if(GetTagPosDO(ter, di->file.sSubOutDigital2, di->nSubOutDigital2)) {
			di->cTypeSubOutDigital2 = TAG_TYPE_DO;
		}
		else {
			if(GetTagPosGDO(ter, di->file.sSubOutDigital2, di->nSubOutDigital2)) {
				di->cTypeSubOutDigital2 = TAG_TYPE_GDO;
			}
			else {
	#if	defined (COMPILE_HANGUL)
				message.Format("DI 태그에 설정된 OutDigital2-(%s)와 일치 태그 없음", di->file.sSubOutDigital2);
	#else
				message.Format("OutDigital2-(%s) tag not found at DI.TAG", di->file.sSubOutDigital2);
	#endif
				MessageBox(hwnd, message, di->file.tag, MB_OK);
				di->nSubOutDigital2 = -1;
				di->cTypeSubOutDigital2 = -1;
			}
		}
	}

	// curr 값이 ON 일때 출력할 디지털 태그
	if(strlen(di->file.sSubOutDigitalOnTag) > 0) {
		if(GetTagPosDO(ter, di->file.sSubOutDigitalOnTag, di->nSubOutDigitalOnTag)) {
			di->cTypeSubOutDigitalOnTag = TAG_TYPE_DO;
		}
		else {
			if(GetTagPosGDO(ter, di->file.sSubOutDigitalOnTag, di->nSubOutDigitalOnTag)) {
				di->cTypeSubOutDigitalOnTag = TAG_TYPE_GDO;
			}
			else {
#if	defined (COMPILE_HANGUL)
				message.Format("DI 태그에 설정된 OutDigitalOnTag-(%s)와 일치 태그 없음", di->file.sSubOutDigitalOnTag);
#else
				message.Format("OutDigitalOnTag-(%s) tag not found at DI.TAG", di->file.sSubOutDigitalOnTag);
#endif
				MessageBox(hwnd, message, di->file.tag, MB_OK);
				di->nSubOutDigitalOnTag = -1;
				di->cTypeSubOutDigitalOnTag = -1;
			}
		}
	}

	// curr 값이 OFF 일때 출력할 디지털 태그
	if(strlen(di->file.sSubOutDigitalOffTag) > 0) {
		if(GetTagPosDO(ter, di->file.sSubOutDigitalOffTag, di->nSubOutDigitalOffTag)) {
			di->cTypeSubOutDigitalOffTag = TAG_TYPE_DO;
		}
		else {
			if(GetTagPosGDO(ter, di->file.sSubOutDigitalOffTag, di->nSubOutDigitalOffTag)) {
				di->cTypeSubOutDigitalOffTag = TAG_TYPE_GDO;
			}
			else {
#if	defined (COMPILE_HANGUL)
				message.Format("DI 태그에 설정된 OutDigitalOFFTag-(%s)와 일치 태그 없음", di->file.sSubOutDigitalOffTag);
#else
				message.Format("OutDigitalOFFTag-(%s) tag not found at DI.TAG", di->file.sSubOutDigitalOffTag);
#endif
				MessageBox(hwnd, message, di->file.tag, MB_OK);
				di->nSubOutDigitalOffTag = -1;
				di->cTypeSubOutDigitalOffTag = -1;
			}
		}
	}
}


#if	defined (COMPILE_HANGUL)
static char *tag_same_error_title = "오류:태그 이름 중복 사용";
#else
static char *tag_same_error_title = "Error:Same tag used";
#endif

static char *sReserved = "reserved";

static int CheckSameTagAtAO(HWND hwnd, TERMINAL_STRUCT *ter, char *tag, int pos, char *tag_file)
{
	int i;
	ANALOG_OUTPUT_STRUCT *ao;
	CString message;
	
	for(i = 0; i < ter->nAnalogOutputHap; i++) {
		ao = &ter->analogOutput[i];
		if(strcmp(tag, ao->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
			message.Format("%s의 %d번째 [%s]태그와\nAO.TAG의 %d번째 [%s]태그가 일치합니다.", tag_file, pos+1, tag, i+1, ao->file.tag);
#else
			message.Format("line %d of %s [%s] tag and\nline %d of AO.TAG [%s] tag is same name.", pos+1, tag_file, tag, i+1, ao->file.tag);
#endif
			MessageBox(hwnd, message, tag_same_error_title, MB_OK);
			return 1;
		}
	}	

	return 0;
}

static int CheckSameTagAtDI(HWND hwnd, TERMINAL_STRUCT *ter, char *tag, int pos, char *tag_file)
{
	int i;
	DIGITAL_INPUT_STRUCT *di;
	
	
	for(i = 0; i < ter->nDigitalInputHap; i++) {
		di = &ter->digitalInput[i];
		if(strcmp(tag, di->file.tag) == 0) {
			CString message;
#if	defined (COMPILE_HANGUL)
			message.Format("%s의 %d번째 [%s]태그와\nDI.TAG의 %d번째 [%s]태그가 일치합니다.", tag_file, pos+1, tag, i+1, di->file.tag);
#else
			message.Format("line %d of %s [%s] tag and\nline %d of DI.TAG [%s] tag is same name.", pos+1, tag_file, tag, i+1, di->file.tag);
#endif
			MessageBox(hwnd, message, tag_same_error_title, MB_OK);
			return 1;
		}
	}	

	return 0;
}

static int CheckSameTagAtDO(HWND hwnd, TERMINAL_STRUCT *ter, char *tag, int pos, char *tag_file)
{
	int i;
	DIGITAL_OUTPUT_STRUCT *dout;
	
	
	for(i = 0; i < ter->nDigitalOutputHap; i++) {
		dout = &ter->digitalOutput[i];
		if(strcmp(tag, dout->file.tag) == 0) {
			CString message;
#if	defined (COMPILE_HANGUL)
			message.Format("%s의 %d번째 [%s]태그와\nDO.TAG의 %d번째 [%s]태그가 일치합니다.", tag_file, pos+1, tag, i+1, dout->file.tag);
#else
			message.Format("line %d of %s [%s] tag and\nline %d of DO.TAG [%s] tag is same name.", pos+1, tag_file, tag, i+1, dout->file.tag);
#endif
			MessageBox(hwnd, message, tag_same_error_title, MB_OK);
			return 1;
		}
	}	

	return 0;
}

static int CheckSameTagAtST(HWND hwnd, TERMINAL_STRUCT *ter, char *tag, int pos, char *tag_file)
{
	int i;
	TAG_ST_STRUCT *st;
	CString message;
	
	for(i = 0; i < ter->nStringTagHap; i++) {
		st = &ter->stringTag[i];
		if(strcmp(tag, st->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
			message.Format("%s의 %d번째 [%s]태그와\nST.TAG의 %d번째 [%s]태그가 일치합니다.", tag_file, pos+1, tag, i+1, st->file.tag);
#else
			message.Format("line %d of %s [%s] tag and\nline %d of ST.TAG [%s] tag is same name.", pos+1, tag_file, tag, i+1, st->file.tag);
#endif
			MessageBox(hwnd, message, tag_same_error_title, MB_OK);
			return 1;
		}
	}	

	return 0;
}

//------------------------------------------------------------------------------
//	중복 사용된 태그가 있는가 검사한다.
//------------------------------------------------------------------------------

void CheckSameTagAI(HWND hwnd, TERMINAL_STRUCT *ter)
{
	ANALOG_INPUT_STRUCT *ai1, *ai2;

	CString message;
	int i, j;

	for(i = 0; i < ter->nAnalogInputHap; i++) {
		ai1 = &ter->analogInput[i];
		if(strnicmp(ai1->file.tag, sReserved, 8) == 0) {	// "reserved" or "RESERVED"
			ai1->file.act = OFF;
			continue;
		}
		for(j = i+1; j < ter->nAnalogInputHap; j++) {
			ai2 = &ter->analogInput[j];
			if(strcmp(ai1->file.tag, ai2->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
				message.Format("AI.TAG의 %d번째 [%s]태그와\nAI.TAG의 %d번째 [%s]태그가 일치합니다.", i+1, ai1->file.tag, j+1, ai2->file.tag);
#else
				message.Format("line %d of AI.TAG [%s] tag and\nline %d of AI.TAG [%s] tag is same name.", i+1, ai1->file.tag, j+1, ai2->file.tag);
#endif
				MessageBox(hwnd, message, tag_same_error_title, MB_OK);
				return;
			}
		}

		if(CheckSameTagAtAO(hwnd, ter, ai1->file.tag, i, "AI.TAG"))	return;
		if(CheckSameTagAtDI(hwnd, ter, ai1->file.tag, i, "AI.TAG"))	return;
		if(CheckSameTagAtDO(hwnd, ter, ai1->file.tag, i, "AI.TAG"))	return;
		if(CheckSameTagAtST(hwnd, ter, ai1->file.tag, i, "AI.TAG"))	return;
	}
}

//------------------------------------------------------------------------------
//	중복 사용된 태그가 있는가 검사한다.
//------------------------------------------------------------------------------

void CheckSameTagAO(HWND hwnd, TERMINAL_STRUCT *ter)
{
	ANALOG_OUTPUT_STRUCT *ao1, *ao2;

	CString message;
	int i, j;

	for(i = 0; i < ter->nAnalogOutputHap; i++) {
		ao1 = &ter->analogOutput[i];
		if(strnicmp(ao1->file.tag, sReserved, 8) == 0 ) {	// "reserved" or "RESERVED"
			ao1->file.act = OFF;
			continue;
		}
		for(j = i+1; j < ter->nAnalogOutputHap; j++) {
			ao2 = &ter->analogOutput[j];
			if(strcmp(ao1->file.tag, ao2->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
				message.Format("AO.TAG의 %d번째 [%s]태그와\nAO.TAG의 %d번째 [%s]태그가 일치합니다.", i+1, ao1->file.tag, j+1, ao2->file.tag);
#else
				message.Format("line %d of AO.TAG [%s] tag and\nline %d of AO.TAG [%s] tag is same name.", i+1, ao1->file.tag, j+1, ao2->file.tag);
#endif
				MessageBox(hwnd, message, tag_same_error_title, MB_OK);
				return;
			}
		}
		if(CheckSameTagAtDI(hwnd, ter, ao1->file.tag, i, "AO.TAG"))	return;
		if(CheckSameTagAtDO(hwnd, ter, ao1->file.tag, i, "AO.TAG"))	return;
		if(CheckSameTagAtST(hwnd, ter, ao1->file.tag, i, "AO.TAG"))	return;
	}
}

//------------------------------------------------------------------------------
//	중복 사용된 태그가 있는가 검사한다.
//------------------------------------------------------------------------------

void CheckSameTagDI(HWND hwnd, TERMINAL_STRUCT *ter)
{
	DIGITAL_INPUT_STRUCT *di1, *di2;

	CString message;
	int i, j;

	for(i = 0; i < ter->nDigitalInputHap; i++) {
		di1 = &ter->digitalInput[i];
		if(strnicmp(di1->file.tag, sReserved, 8) == 0 ) {	// "reserved" or "RESERVED"
			di1->file.act = OFF;
			continue;
		}
		for(j = i+1; j < ter->nDigitalInputHap; j++) {
			di2 = &ter->digitalInput[j];
			if(strcmp(di1->file.tag, di2->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
				message.Format("DI.TAG의 %d번째 [%s]태그와\nDI.TAG의 %d번째 [%s]태그가 일치합니다.", i+1, di1->file.tag, j+1, di2->file.tag);
#else
				message.Format("line %d of DI.TAG [%s] tag and\nline %d of DI.TAG [%s] tag is same name.", i+1, di1->file.tag, j+1, di2->file.tag);
#endif
				MessageBox(hwnd, message, tag_same_error_title, MB_OK);
				return;
			}
		}
		if(CheckSameTagAtDO(hwnd, ter, di1->file.tag, i, "DI.TAG"))	return;
		if(CheckSameTagAtST(hwnd, ter, di1->file.tag, i, "DI.TAG"))	return;
	}
}

//------------------------------------------------------------------------------
//	중복 사용된 태그가 있는가 검사한다.
//------------------------------------------------------------------------------

void CheckSameTagDO(HWND hwnd, TERMINAL_STRUCT *ter)
{
	DIGITAL_OUTPUT_STRUCT *dout1, *dout2;

	CString message;
	int i, j;

	for(i = 0; i < ter->nDigitalOutputHap; i++) {
		dout1 = &ter->digitalOutput[i];
		if(strnicmp(dout1->file.tag, sReserved, 8) == 0 ) {	// "reserved" or "RESERVED"
			dout1->file.act = OFF;
			continue;
		}
		for(j = i+1; j < ter->nDigitalOutputHap; j++) {
			dout2 = &ter->digitalOutput[j];
			if(strcmp(dout1->file.tag, dout2->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
				message.Format("DO.TAG의 %d번째 [%s]태그와\nDO.TAG의 %d번째 [%s]태그가 일치합니다.", i+1, dout1->file.tag, j+1, dout2->file.tag);
#else
				message.Format("line %d of DO.TAG [%s] tag and\nline %d of DO.TAG [%s] tag is same name.", i+1, dout1->file.tag, j+1, dout2->file.tag);
#endif
				MessageBox(hwnd, message, tag_same_error_title, MB_OK);
				return;
			}
		}
		if(CheckSameTagAtST(hwnd, ter, dout1->file.tag, i, "DO.TAG"))	return;
	}
}

//------------------------------------------------------------------------------
//	중복 사용된 태그가 있는가 검사한다.
//------------------------------------------------------------------------------

void CheckSameTagST(HWND hwnd, TERMINAL_STRUCT *ter)
{
	TAG_ST_STRUCT *st1, *st2;

	CString message;
	int i, j;

	for(i = 0; i < ter->nStringTagHap; i++) {
		st1 = &ter->stringTag[i];
		if(strnicmp(st1->file.tag, sReserved, 8) == 0 ) {	// "reserved" or "RESERVED"
			st1->file.act = OFF;
			continue;
		}
		for(j = i+1; j < ter->nStringTagHap; j++) {
			st2 = &ter->stringTag[j];
			if(strcmp(st1->file.tag, st2->file.tag) == 0) {
#if	defined (COMPILE_HANGUL)
				message.Format("ST.TAG의 %d번째 [%s]태그와\nST.TAG의 %d번째 [%s]태그가 일치합니다.", i+1, st1->file.tag, j+1, st2->file.tag);
#else
				message.Format("line %d of ST.TAG [%s] tag and\nline %d of ST.TAG [%s] tag is same name.", i+1, st1->file.tag, j+1, st2->file.tag);
#endif
				MessageBox(hwnd, message, tag_same_error_title, MB_OK);
				return;
			}
		}
	}
}
#endif

//------------------------------------------------------------------------------
//	태그 파일을 읽어서 준비한다.
//	터미널 하나가 접속 되었을때도 이부분을 불러준다.
//------------------------------------------------------------------------------

void TagLoadTerminal(HWND hwnd, TERMINAL_STRUCT *ter)
{

	CString msg;

	if(ter->tag_read_flag == ON)	return;

//	TagFree();

	ter->analogInput = NULL;
	ter->digitalInput = NULL;
	ter->analogOutput = NULL;
	ter->digitalOutput = NULL;
	ter->stringTag = NULL;

	ter->nAnalogInputHap = 0;
	ter->nAnalogOutputHap = 0;
	ter->nDigitalInputHap = 0;
	ter->nDigitalOutputHap = 0;
	ter->nStringTagHap = 0;

	// 아래 부분은 태그 구조체의 크기가 2의 승수인가를 검사한다.
	// Global 메모리에서는 구조체의 크기가 2,4,8,16... 의 크기가 되어야 한다.
	
	/*
	if(!IsSquare(sizeof(ANALOG_INPUT_STRUCT))) {
		msg.Format("must ANALOG INPUT struct not 2**(current-%d)", sizeof(ANALOG_INPUT_STRUCT));
		MessageBox(hwnd, msg, "Programm error", MB_OK);
		return;
	}

	if(!IsSquare(sizeof(ANALOG_OUTPUT_STRUCT))) {
		msg.Format("must ANALOG OUTPUT struct not 2**(current-%d)", sizeof(ANALOG_OUTPUT_STRUCT));
		MessageBox(hwnd, msg, "Programm error", MB_OK);
		return;
	}
	if(!IsSquare(sizeof(DIGITAL_INPUT_STRUCT))) {
		msg.Format("must DIGITAL INPUT struct not 2**(current-%d)", sizeof(DIGITAL_INPUT_STRUCT));
		MessageBox(hwnd, msg, "Programm error", MB_OK);
		return;
	}
	if(!IsSquare(sizeof(DIGITAL_OUTPUT_STRUCT))) {
		msg.Format("must DIGITAL OUTPUT struct not 2**(current-%d)", sizeof(DIGITAL_OUTPUT_STRUCT));
		MessageBox(hwnd, msg, "Programm error", MB_OK);
		return;
	}

	if(!IsSquare(sizeof(TAG_ST_STRUCT))) {
		msg.Format("must STRING TAG struct not 2**(current-%d)", sizeof(TAG_ST_STRUCT));
		MessageBox(hwnd, msg, "Programm error", MB_OK);
		return;
	}
	*/

	TagAIRead(hwnd, ter);
	TagAORead(hwnd, ter);
	TagDIRead(hwnd, ter);
	TagDORead(hwnd, ter);
	TagSTRead(hwnd, ter);

	if(ter->nAnalogInputHap == 0) {	// 태그가 없을때는 자동으로 하나를 만든다.
		ter->analogInput = new TAG_AI_STRUCT[1];
		if(ter->analogInput) {
			ter->nAnalogInputHap = 1;
			TAG_AI_STRUCT *ai = &ter->analogInput[0];
			ZeroMemory(ai, sizeof(TAG_AI_STRUCT));
			strcpy(ai->file.tag, "AI_0000");
		}
	}
	if(ter->nAnalogOutputHap == 0) {	// 태그가 없을때는 자동으로 하나를 만든다.
		ter->analogOutput = new TAG_AO_STRUCT[1];
		if(ter->analogOutput) {
			ter->nAnalogOutputHap = 1;
			TAG_AO_STRUCT *ao = &ter->analogOutput[0];
			ZeroMemory(ao, sizeof(TAG_AO_STRUCT));
			strcpy(ao->file.tag, "AO_0000");
		}
	}
	if(ter->nDigitalInputHap == 0) {	// 태그가 없을때는 자동으로 하나를 만든다.
		ter->digitalInput = new TAG_DI_STRUCT[1];
		if(ter->digitalInput) {
			ter->nDigitalInputHap = 1;
			TAG_DI_STRUCT *di = &ter->digitalInput[0];
			ZeroMemory(di, sizeof(TAG_DI_STRUCT));
			strcpy(di->file.tag, "DI_0000");
		}
	}
	if(ter->nDigitalOutputHap == 0) {	// 태그가 없을때는 자동으로 하나를 만든다.
		ter->digitalOutput = new TAG_DO_STRUCT[1];
		if(ter->digitalOutput) {
			ter->nDigitalOutputHap = 1;
			TAG_DO_STRUCT *dout = &ter->digitalOutput[0];
			ZeroMemory(dout, sizeof(TAG_DO_STRUCT));
			strcpy(dout->file.tag, "DO_0000");
		}
	}
	if(ter->nStringTagHap == 0) {	// 태그가 없을때는 자동으로 하나를 만든다.
		ter->stringTag = new TAG_ST_STRUCT[1];
		if(ter->stringTag) {
			ter->nStringTagHap = 1;
			TAG_ST_STRUCT *st = &ter->stringTag[0];
			ZeroMemory(st, sizeof(TAG_ST_STRUCT));
			strcpy(st->file.tag, "ST_0000");
		}
	}

	GroupTagLoad(hwnd);					// 그룹 태그를 읽어온다.

#if	defined (MODE_RUN)
	CheckSameTagAI(hwnd, ter);	// 중복 사용된 태그가 있는가를 검사한다.
	CheckSameTagAO(hwnd, ter);
	CheckSameTagDI(hwnd, ter);
	CheckSameTagDO(hwnd, ter);
	CheckSameTagST(hwnd, ter);

	int i;

	for(i = 0; i < ter->nAnalogInputHap; i++)	PrepareSubTagAI(hwnd, ter->terminal, i);
	for(i = 0; i < ter->nDigitalInputHap; i++)	PrepareSubTagDI(hwnd, ter->terminal, i);

	TAG_AI_STRUCT *ai;
	CString name;

	for(i = 0; i < ter->nAnalogInputHap; i++) {
		ai = &terminalStruct[0].analogInput[i];

		name.Format("TAG_%s", ai->file.tag);

		ai->pSharedTag = NULL;
		ai->hSharedTag = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_TAG_AI), name);
		if(ai->hSharedTag == NULL) {
			//MessageBox(hwnd, "NULL", "Error", MB_OK);
			continue;

		}
		ai->pSharedTag = (SHARED_TAG_AI*)MapViewOfFile(ai->hSharedTag, FILE_MAP_WRITE, 0, 0, 0);
		ai->pSharedTag->pub.tag_type = TAG_TYPE_AI;

		if(GetLastError() != ERROR_ALREADY_EXISTS) {
			//MessageBox(hwnd, "ERROR_ALREADY_EXISTS", "Error", MB_OK);
			ai->pSharedTag->pub.struct_size = sizeof(SHARED_TAG_AI);	
		}
		else {

		}
	}

	TAG_AO_STRUCT *ao;

	for(i = 0; i < ter->nAnalogOutputHap; i++) {
		ao = &terminalStruct[0].analogOutput[i];

		name.Format("TAG_%s", ao->file.tag);

		ao->pSharedTag = NULL;
		ao->hSharedTag = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_TAG_AO), name);
		if(ao->hSharedTag == NULL)	continue;
		ao->pSharedTag = (SHARED_TAG_AO*)MapViewOfFile(ao->hSharedTag, FILE_MAP_WRITE, 0, 0, 0);
		ao->pSharedTag->pub.tag_type = TAG_TYPE_AO;

		if(GetLastError() != ERROR_ALREADY_EXISTS) {
			ao->pSharedTag->pub.struct_size = sizeof(SHARED_TAG_AO);
		}
	}
	
	TAG_DI_STRUCT *di;

	for(i = 0; i < ter->nDigitalInputHap; i++) {
		di = &terminalStruct[0].digitalInput[i];

		name.Format("TAG_%s", di->file.tag);

		di->pSharedTag = NULL;
		di->hSharedTag = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_TAG_DI), name);
		if(di->hSharedTag == NULL)	continue;
		di->pSharedTag = (SHARED_TAG_DI*)MapViewOfFile(di->hSharedTag, FILE_MAP_WRITE, 0, 0, 0);
		di->pSharedTag->pub.tag_type = TAG_TYPE_DI;

		if(GetLastError() != ERROR_ALREADY_EXISTS) {
			di->pSharedTag->pub.struct_size = sizeof(SHARED_TAG_DI);	
		}
	}

	TAG_DO_STRUCT *dout;

	for(i = 0; i < ter->nDigitalOutputHap; i++) {
		dout = &terminalStruct[0].digitalOutput[i];

		name.Format("TAG_%s", dout->file.tag);

		dout->pSharedTag = NULL;
		dout->hSharedTag = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_TAG_DO), name);
		if(dout->hSharedTag == NULL)	continue;
		dout->pSharedTag = (SHARED_TAG_DO*)MapViewOfFile(dout->hSharedTag, FILE_MAP_WRITE, 0, 0, 0);
		dout->pSharedTag->pub.tag_type = TAG_TYPE_DO;

		if(GetLastError() != ERROR_ALREADY_EXISTS) {
			dout->pSharedTag->pub.struct_size = sizeof(SHARED_TAG_DO);	
		}
	}

	TAG_ST_STRUCT *st;

	for(i = 0; i < ter->nStringTagHap; i++) {
		st = &terminalStruct[0].stringTag[i];

		name.Format("TAG_%s", st->file.tag);

		st->pSharedTag = NULL;
		st->hSharedTag = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(SHARED_TAG_ST), name);
		if(st->hSharedTag == NULL)	continue;
		st->pSharedTag = (SHARED_TAG_ST*)MapViewOfFile(st->hSharedTag, FILE_MAP_WRITE, 0, 0, 0);
		st->pSharedTag->pub.tag_type = TAG_TYPE_ST;

		if(GetLastError() != ERROR_ALREADY_EXISTS) {
			st->pSharedTag->pub.struct_size = sizeof(SHARED_TAG_ST);
		}
	}

#endif

	ter->tag_read_flag = ON;
}

void TagPrepare(HWND hwnd)
{
	int i;
	TERMINAL_STRUCT *ter;

	terminalStruct = new TERMINAL_STRUCT[MAX_TERMINAL];
	if(terminalStruct == NULL)	{
		MsgBoxGlobalMemoryLow(hwnd, "new terminalStruct");
		return;
	}

	for(i = 0; i < MAX_TERMINAL; i++) {
		ter = &terminalStruct[i];
		memset(ter, 0, sizeof(TERMINAL_STRUCT));
		
		ter->terminal = i;
		
		ter->analogInput = NULL;
		ter->digitalInput = NULL;
		ter->analogOutput = NULL;
		ter->digitalOutput = NULL;

		ter->nAnalogInputHap = 0;
		ter->nAnalogOutputHap = 0;
		ter->nDigitalInputHap = 0;
		ter->nDigitalOutputHap = 0;
	}

	TagLoadTerminal(hwnd, &terminalStruct[0]);
}

//------------------------------------------------------------------------------
//	프로그램을 끝내기 전에 태그 메모리를 모두 풀어준다.
// 약간의 문제가 있다. 이 부분을 포함시키면 약간 이상한 문제가 발생한다. (윈도우 다운)
//------------------------------------------------------------------------------

static void TagFreeTerminal(TERMINAL_STRUCT *ter)
{
	TAG_AI_STRUCT *ai;
	for(int i = 0; i < ter->nAnalogInputHap; i++) {
		ai = &terminalStruct[0].analogInput[i];

		if(ai->pSharedTag)	UnmapViewOfFile(ai->pSharedTag);
		if(ai->hSharedTag)	CloseHandle(ai->hSharedTag);
	}

	TAG_AO_STRUCT *ao;
	for(int i = 0; i < ter->nAnalogOutputHap; i++) {
		ao = &terminalStruct[0].analogOutput[i];

		if(ao->pSharedTag)	UnmapViewOfFile(ao->pSharedTag);
		if(ao->hSharedTag)	CloseHandle(ao->hSharedTag);
	}

	TAG_DI_STRUCT *di;
	for(int i = 0; i < ter->nDigitalInputHap; i++) {
		di = &terminalStruct[0].digitalInput[i];

		if(di->pSharedTag)	UnmapViewOfFile(di->pSharedTag);
		if(di->hSharedTag)	CloseHandle(di->hSharedTag);
	}

	TAG_DO_STRUCT *dout;
	for(int i = 0; i < ter->nDigitalOutputHap; i++) {
		dout = &terminalStruct[0].digitalOutput[i];

		if(dout->pSharedTag)	UnmapViewOfFile(dout->pSharedTag);
		if(dout->hSharedTag)	CloseHandle(dout->hSharedTag);
	}

	TAG_ST_STRUCT *st;
	for(int i = 0; i < ter->nStringTagHap; i++) {
		st = &terminalStruct[0].stringTag[i];

		if(st->pSharedTag)	UnmapViewOfFile(st->pSharedTag);
		if(st->hSharedTag)	CloseHandle(st->hSharedTag);
	}

	if(ter->analogInput != NULL)	delete ter->analogInput;
	if(ter->analogOutput != NULL)	delete ter->analogOutput;
	if(ter->digitalInput != NULL)	delete ter->digitalInput;
	if(ter->digitalOutput != NULL)	delete ter->digitalOutput;
	if(ter->stringTag != NULL)		delete ter->stringTag;

	ter->nAnalogInputHap = 0;
	ter->nAnalogOutputHap = 0;
	ter->nDigitalInputHap = 0;
	ter->nDigitalOutputHap = 0;
	ter->nStringTagHap = 0;

  	ter->analogInput = NULL;
	ter->digitalInput = NULL;
	ter->analogOutput = NULL;
	ter->digitalOutput = NULL;
	ter->stringTag = NULL;
}

void TagFree()
{
	if(terminalStruct == NULL)	return;
	
	TERMINAL_STRUCT *ter;
	int i;

	
	for(i = 0; i < MAX_TERMINAL; i++) {
		ter = &terminalStruct[i];
		TagFreeTerminal(ter);
	}

	delete terminalStruct;

	GroupTagFree();
	
	terminalStruct = NULL;
}

void GetDesON(TAG_DI_STRUCT *di, char *des)
{
	if(strlen(di->file.desON) == 0)	{
		strcpy(des, "ON");
	}
	else {
		strcpy(des, di->file.desON);
	}
}

void GetDesOFF(TAG_DI_STRUCT *di, char *des)
{
	if(strlen(di->file.desOFF) == 0)	{
		strcpy(des, "OFF");
	}
	else {
		strcpy(des, di->file.desOFF);
	}
}

void GetDesON(TAG_DI_STRUCT *di, CString &des)
{
	if(strlen(di->file.desON) == 0)	{
		des = "ON";
	}
	else {
		des = di->file.desON;
	}
}

void GetDesOFF(TAG_DI_STRUCT *di, CString &des)
{
	if(strlen(di->file.desOFF) == 0)	{
		des = "OFF";
	}
	else {
		des = di->file.desOFF;
	}
}

void GetDigitalStatusString(TAG_DI_STRUCT *di, char *des, char flag)
{
	if(flag)	GetDesON(di, des);
	else		GetDesOFF(di, des);
}

void GetDoDesON(TAG_DO_STRUCT *dout, char *des)
{
	if(strlen(dout->file.desON) == 0)	{
		strcpy(des, "ON");
	}
	else {
		strcpy(des, dout->file.desON);
	}
}

void GetDoDesOFF(TAG_DO_STRUCT *dout, char *des)
{
	if(strlen(dout->file.desOFF) == 0)	{
		strcpy(des, "OFF");
	}
	else {
		strcpy(des, dout->file.desOFF);
	}
}

void GetDoStatusString(TAG_DO_STRUCT *dout, char *des, char flag)
{
	if(flag)	GetDoDesON(dout, des);
	else		GetDoDesOFF(dout, des);
}

TAG_AI_STRUCT *TagShareGetAI(const char *tag)
{
	short pos;

	if(!GetTagPosAI(&terminalStruct[0], tag, pos))	return NULL;

	return &terminalStruct[0].analogInput[pos];;
}

TAG_AI_STRUCT *GetDirectTag(TAG_AI_STRUCT *ai)
{
	if(ai->file.cTagType != 3)	return ai;

	if(ai->assign.pos == -1)	return ai;	// assign 되어 있지 않다.

	return &terminalStruct[0].analogInput[ai->assign.pos];
}

TAG_DI_STRUCT *GetDirectTag(TAG_DI_STRUCT *di)
{
	if(di->file.cTagType != 3)	return di;

	if(di->assign.pos == -1)	return di;	// assign 되어 있지 않다.

	return &terminalStruct[0].digitalInput[di->assign.pos];
}

TAG_AO_STRUCT *GetDirectTag(TAG_AO_STRUCT *ao)
{
	if(ao->file.cTagType != 3)	return ao;

	if(ao->assign.pos == -1)	return ao;	// assign 되어 있지 않다.

	return &terminalStruct[0].analogOutput[ao->assign.pos];
}

TAG_DO_STRUCT *GetDirectTag(TAG_DO_STRUCT *dout)
{
	if(dout->file.cTagType != 3)	return dout;

	if(dout->assign.pos == -1)	return dout;	// assign 되어 있지 않다.

	return &terminalStruct[0].digitalOutput[dout->assign.pos];
}

