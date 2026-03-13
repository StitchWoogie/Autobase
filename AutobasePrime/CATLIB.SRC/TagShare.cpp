// TAG size O.K
#include "stdafx.h"

#include <tools.h>
#include <dataswap.h>

#include "cattag.h"
#include "totalcfg.h"
#include "TagShare.h"
#include "grouptag.h"

TERMINAL_STRUCT *terminalStruct;

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
	
	WORD reserved;
	commaBuf.GetWORD(reserved);		// 이전(7.83) 이전에는 ai->sp 로 사용

	commaBuf.GetFloat(ai->hihi);
	commaBuf.GetFloat(ai->high);
	commaBuf.GetFloat(ai->low);
	commaBuf.GetFloat(ai->lolo);
	commaBuf.GetChar(ai->act);
	commaBuf.GetChar(ai->alarm);
	commaBuf.GetChar(ai->bFileSave);	// 데이타 파일을 저장할 것이냐?

	commaBuf.GetString(ai->sSubOutDigitalHiHi, MAX_TAG_NAME);
	KillEndSpace(ai->sSubOutDigitalHiHi);
	commaBuf.GetString(ai->sSubOutDigitalLoLo, MAX_TAG_NAME);
	KillEndSpace(ai->sSubOutDigitalLoLo);
	commaBuf.GetString(ai->sSubOutAnalog, MAX_TAG_NAME);
	KillEndSpace(ai->sSubOutAnalog);

	commaBuf.GetFloat(ai->fDisplayFormat);		// 디스프레이 할 방법 선택
	if(ai->fDisplayFormat == 0)	ai->fDisplayFormat = (float)10.2;
	commaBuf.GetChar(ai->cAlarmType);			// 알람 조건
	commaBuf.GetString(ai->sGraphicFile, sizeof(ai->sGraphicFile));			// 이 태그가 있는 그래픽 파일은?
	commaBuf.GetString(ai->sAlarmWaveFile, sizeof(ai->sAlarmWaveFile));		// 경보 발생시 사용할 그래픽 파일
	commaBuf.GetInt(ai->nCalculateFilter);		// 특별한 계산식을 정한다. 0 - 일반

	commaBuf.GetString(ai->sSubOutAnalogSP, MAX_TAG_NAME);		// Analog Set Point 값.
	KillEndSpace(ai->sSubOutAnalogSP);

	reserved;
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

	if(ai->view_full <= ai->view_base) {	// 보여주는 range가 이상할때는 다시 초기화 해 준다.
		ai->view_full = ai->full;
		ai->view_base = ai->base;
	}

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

static void MsgBoxGlobalMemoryLow(HWND hwnd, char *des)
{
	MessageBox(hwnd, des, "Insufficent Memory Size", MB_OK);
}

static void TagAIRead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char share_name[80];

	sprintf(share_name, "AutoBaseTerminal%03dTagAI", ter->terminal);

	if(ter->tagHeader) {	// 다른 프로그램에서 이미 Tag를 만들었다.
		ter->hmmfAI = OpenFileMapping(FILE_MAP_WRITE, FALSE, share_name);
		if(ter->hmmfAI != NULL) {
			ter->nAnalogInputHap = ter->tagHeader->nHapAI;
			ter->analogInput = (TAG_AI_STRUCT*)MapViewOfFile(ter->hmmfAI, FILE_MAP_WRITE, 0, 0, 0);
			return;
		}
	}
	
	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	ANALOG_INPUT_STRUCT ai;
	Block block(sizeof(TAG_AI_STRUCT));
	StackChar workdir(MAXPATH);

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\AI.TAG)");
		return;
	}

	AutoBaseIniGetProjectDirectory(workdir.data);
	wsprintf(filename, "%s\\TAG\\AI.TAG", workdir.data);

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

	ter->hmmfAI = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(TAG_AI_STRUCT)*block.GetCount(), share_name);
	if(ter->hmmfAI == NULL)	return;	//할당할 수 없다.

	ter->analogInput = (TAG_AI_STRUCT*)MapViewOfFile(ter->hmmfAI, FILE_MAP_WRITE, 0, 0, 0);

	if(ter->analogInput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\AI.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		block.GetBlock(&ai, l);
		memcpy(&ter->analogInput[l], &ai, sizeof(TAG_AI_STRUCT));
	}

	ter->nAnalogInputHap = block.GetCount();
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
	char share_name[80];

	sprintf(share_name, "AutoBaseTerminal%03dTagAO", ter->terminal);

	if(ter->tagHeader) {	// 다른 프로그램에서 이미 Tag를 만들었다.
		ter->hmmfAO = OpenFileMapping(FILE_MAP_WRITE, FALSE, share_name);
		if(ter->hmmfAO != NULL) {
			ter->nAnalogOutputHap = ter->tagHeader->nHapAO;
			ter->analogOutput = (TAG_AO_STRUCT*)MapViewOfFile(ter->hmmfAO, FILE_MAP_WRITE, 0, 0, 0);
			return;
		}
	}

	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	ANALOG_OUTPUT_STRUCT ao;
	Block block(sizeof(TAG_AO_STRUCT));
	StackChar workdir(MAXPATH);

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\AO.TAG)");
		return;
	}

	AutoBaseIniGetProjectDirectory(workdir.data);
	wsprintf(filename, "%s\\TAG\\AO.TAG", workdir.data);

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

	ter->hmmfAO = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(TAG_AO_STRUCT)*block.GetCount(), share_name);
	if(ter->hmmfAO == NULL)	return;	//할당할 수 없다.

	ter->analogOutput = (TAG_AO_STRUCT*)MapViewOfFile(ter->hmmfAO, FILE_MAP_WRITE, 0, 0, 0);

	if(ter->analogOutput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\AO.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		block.GetBlock(&ao, l);
		memcpy(&ter->analogOutput[l], &ao, sizeof(TAG_AO_STRUCT));
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
	KillEndSpace(di->sSubOutDigital1);
	commaBuf.GetString(di->sSubOutDigitalOnTag, MAX_TAG_NAME);
	KillEndSpace(di->sSubOutDigital1);
	commaBuf.GetString(di->sSubOutDigitalOffTag, MAX_TAG_NAME);
	KillEndSpace(di->sSubOutDigital1);

	commaBuf.GetChar(di->bFileSave);	// 데이타 파일을 저장할 것이냐?
	commaBuf.GetChar(di->cAlarmType);	// 데이타 파일을 저장할 것이냐?
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
	char share_name[80];

	sprintf(share_name, "AutoBaseTerminal%03dTagDI", ter->terminal);

	if(ter->tagHeader) {	// 다른 프로그램에서 이미 Tag를 만들었다.
		ter->hmmfDI = OpenFileMapping(FILE_MAP_WRITE, FALSE, share_name);
		if(ter->hmmfDI != NULL) {
			ter->nDigitalInputHap = ter->tagHeader->nHapDI;
			ter->digitalInput = (TAG_DI_STRUCT*)MapViewOfFile(ter->hmmfDI, FILE_MAP_WRITE, 0, 0, 0);
			return;
		}
	}

	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	DIGITAL_INPUT_STRUCT di;
	Block block(sizeof(TAG_DI_STRUCT));
	StackChar workdir(MAXPATH);

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\DI.TAG)");
		return;
	}

	AutoBaseIniGetProjectDirectory(workdir.data);
	wsprintf(filename, "%s\\TAG\\DI.TAG", workdir.data);

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

	ter->hmmfDI = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(TAG_DI_STRUCT)*block.GetCount(), share_name);
	if(ter->hmmfDI == NULL)	return;	//할당할 수 없다.

	ter->digitalInput = (TAG_DI_STRUCT*)MapViewOfFile(ter->hmmfDI, FILE_MAP_WRITE, 0, 0, 0);

	if(ter->digitalInput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\DI.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		block.GetBlock(&di, l);
		memcpy(&ter->digitalInput[l], &di, sizeof(TAG_DI_STRUCT));
	}

	ter->nDigitalInputHap = block.GetCount();
}

void CommaBufToDoFileStruct(DO_FILE_STRUCT *dout, char *buf)
{
	int number;
	char reserved;
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
	commaBuf.GetChar(reserved);	// 이전(7.0)에는 wWriteRetryTime

	commaBuf.GetChar(dout->cTagType);		// Dde를 태그로 사용할것인가?
	commaBuf.GetString(dout->sDdeService, sizeof(dout->sDdeService));
	commaBuf.GetString(dout->sDdeTopic,   sizeof(dout->sDdeTopic));
	commaBuf.GetString(dout->sDdeItem,    sizeof(dout->sDdeItem));
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
	char share_name[80];

	sprintf(share_name, "AutoBaseTerminal%03dTagDO", ter->terminal);

	if(ter->tagHeader) {	// 다른 프로그램에서 이미 Tag를 만들었다.
		ter->hmmfDO = OpenFileMapping(FILE_MAP_WRITE, FALSE, share_name);
		if(ter->hmmfDO != NULL) {
			ter->nDigitalOutputHap = ter->tagHeader->nHapDO;
			ter->digitalOutput = (TAG_DO_STRUCT*)MapViewOfFile(ter->hmmfDO, FILE_MAP_WRITE, 0, 0, 0);
			return;
		}
	}

	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	DIGITAL_OUTPUT_STRUCT dout;
	Block block(sizeof(TAG_DO_STRUCT));
	StackChar workdir(MAXPATH);

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\DO.TAG)");
		return;
	}

	AutoBaseIniGetProjectDirectory(workdir.data);
	wsprintf(filename, "%s\\TAG\\DO.TAG", workdir.data);


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

	ter->hmmfDO = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(TAG_DO_STRUCT)*block.GetCount(), share_name);
	if(ter->hmmfDO == NULL)	return;	//할당할 수 없다.

	ter->digitalOutput = (TAG_DO_STRUCT*)MapViewOfFile(ter->hmmfDO, FILE_MAP_WRITE, 0, 0, 0);

	if(ter->digitalOutput == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\DO.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		block.GetBlock(&dout, l);
		memcpy(&ter->digitalOutput[l], &dout, sizeof(TAG_DO_STRUCT));
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

	//commaBuf.GetInt(di->port);
	//commaBuf.GetInt(di->station);
	//commaBuf.GetAddressDI(di->address);
	//commaBuf.GetInt(di->fn);
	//commaBuf.GetString(di->desON, sizeof(di->desON));
	//commaBuf.GetString(di->desOFF, sizeof(di->desOFF));
	commaBuf.GetChar(st->act);
	//commaBuf.GetChar(di->alarm);
}

void CommaBufToTagStructST(TAG_ST_STRUCT *st, char *buf)
{
	CommaBufToStFileStruct(&st->file, buf);
	
	st->curr[0] = 0;
}

static void TagSTRead(HWND hwnd, TERMINAL_STRUCT *ter)
{
	char share_name[80];

	sprintf(share_name, "AutoBaseTerminal%03dTagST", ter->terminal);

	if(ter->tagHeader) {	// 다른 프로그램에서 이미 Tag를 만들었다.
		ter->hmmfST = OpenFileMapping(FILE_MAP_WRITE, FALSE, share_name);
		if(ter->hmmfST != NULL) {
			ter->nStringTagHap = ter->tagHeader->nHapST;
			ter->stringTag = (TAG_ST_STRUCT*)MapViewOfFile(ter->hmmfST, FILE_MAP_WRITE, 0, 0, 0);
			return;
		}
	}

	char filename[MAXPATH];
	FILE *in;
	StackChar buf(10000);
	TAG_ST_STRUCT st;
	Block block(sizeof(TAG_ST_STRUCT));
	StackChar workdir(MAXPATH);

	if(buf.data == NULL) {
		MsgBoxGlobalMemoryLow(hwnd, "reading (TAG\\ST.TAG)");
		return;
	}

	AutoBaseIniGetProjectDirectory(workdir.data);
	wsprintf(filename, "%s\\TAG\\ST.TAG", workdir.data);

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

	ter->hmmfST = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(TAG_ST_STRUCT)*block.GetCount(), share_name);
	if(ter->hmmfST == NULL)	return;	//할당할 수 없다.

	ter->stringTag = (TAG_ST_STRUCT*)MapViewOfFile(ter->hmmfST, FILE_MAP_WRITE, 0, 0, 0);

	if(ter->stringTag == NULL) {
		MessageBox(hwnd, "Can't alloc memory.", "reading (TAG\\ST.TAG)", MB_OK);
		return;
	}

	for(DWORD l = 0; l < block.GetCount(); l++) {
		block.GetBlock(&st, l);
		memcpy(&ter->stringTag[l], &st, sizeof(TAG_ST_STRUCT));
	}

	ter->nStringTagHap = block.GetCount();
}

//------------------------------------------------------------------------------
//	태그 파일을 읽어서 준비한다.
//	터미널 하나가 접속 되었을때도 이부분을 불러준다.
//------------------------------------------------------------------------------

static void TagLoadTerminal(HWND hwnd, TERMINAL_STRUCT *ter)
{
	/*
	char message[80];

	// 아래 부분은 태그 구조체의 크기가 2의 승수인가를 검사한다.
	// Global 메모리에서는 구조체의 크기가 2,4,8,16... 의 크기가 되어야 한다.
	if(!IsSquare(sizeof(ANALOG_INPUT_STRUCT))) {
		wsprintf(message, "must ANALOG INPUT struct not 2**(current-%d)", sizeof(ANALOG_INPUT_STRUCT));
		MessageBox(hwnd, message, "Programm error", MB_OK);
		return;
	}
	if(!IsSquare(sizeof(ANALOG_OUTPUT_STRUCT))) {
		wsprintf(message, "must ANALOG OUTPUT struct not 2**(current-%d)", sizeof(ANALOG_OUTPUT_STRUCT));
		MessageBox(hwnd, message, "Programm error", MB_OK);
		return;
	}
	if(!IsSquare(sizeof(DIGITAL_INPUT_STRUCT))) {
		wsprintf(message, "must DIGITAL INPUT struct not 2**(current-%d)", sizeof(DIGITAL_INPUT_STRUCT));
		MessageBox(hwnd, message, "Programm error", MB_OK);
		return;
	}
	if(!IsSquare(sizeof(DIGITAL_OUTPUT_STRUCT))) {
		wsprintf(message, "must DIGITAL OUTPUT struct not 2**(current-%d)", sizeof(DIGITAL_OUTPUT_STRUCT));
		MessageBox(hwnd, message, "Programm error", MB_OK);
		return;
	}
	if(!IsSquare(sizeof(TAG_ST_STRUCT))) {
		wsprintf(message, "must STRING TAG struct not 2**(current-%d)", sizeof(TAG_ST_STRUCT));
		MessageBox(hwnd, message, "Programm error", MB_OK);
		return;
	}
	*/

	char share_name[80];

	sprintf(share_name, "AutoBaseTerminal%03dTagInfo", ter->terminal);

	ter->hmmfHeader = OpenFileMapping(FILE_MAP_WRITE, FALSE, share_name);
	if(ter->hmmfHeader != NULL) {
		ter->tagHeader = (TAG_MAP_HEADER*)MapViewOfFile(ter->hmmfHeader, FILE_MAP_WRITE, 0, 0, 0);
		if(ter->tagHeader) {
			if(ter->tagHeader->struct_size != sizeof(TAG_MAP_HEADER)) {
				MessageBox(hwnd, "TAG_MAP_HEADER size mismatched", "Error", MB_OK);
				UnmapViewOfFile(ter->tagHeader);
				CloseHandle(ter->hmmfHeader);
				ter->tagHeader = NULL;
				return;
			}
		}
		return;
	}

	TagAIRead(hwnd, ter);
	TagAORead(hwnd, ter);
	TagDIRead(hwnd, ter);
	TagDORead(hwnd, ter);
	TagSTRead(hwnd, ter);

/*
#if	defined (MODE_RUN)
	CheckSameTagAI(hwnd, ter);	// 중복 사용된 태그가 있는가를 검사한다.
	CheckSameTagAO(hwnd, ter);
	CheckSameTagDI(hwnd, ter);
	CheckSameTagDO(hwnd, ter);
	CheckSameTagST(hwnd, ter);

	int i;

	for(i = 0; i < ter->nAnalogInputHap; i++)	PrepareSubTagAI(hwnd, ter->terminal, i);
	for(i = 0; i < ter->nDigitalInputHap; i++)	PrepareSubTagDI(hwnd, ter->terminal, i);
#endif
*/

	ter->hmmfHeader = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL, PAGE_READWRITE, 0, sizeof(TAG_MAP_HEADER), share_name);
	if(ter->hmmfHeader == NULL)	return;	//할당할 수 없다.

	ter->tagHeader = (TAG_MAP_HEADER*)MapViewOfFile(ter->hmmfHeader, FILE_MAP_WRITE, 0, 0, 0);

	ZeroMemory(ter->tagHeader, sizeof(TAG_MAP_HEADER));
	ter->tagHeader->struct_size = sizeof(TAG_MAP_HEADER);
	ter->tagHeader->nHapAI = ter->nAnalogInputHap;
	ter->tagHeader->nHapAO = ter->nAnalogOutputHap;
	ter->tagHeader->nHapDI = ter->nDigitalInputHap;
	ter->tagHeader->nHapDO = ter->nDigitalOutputHap;
	ter->tagHeader->nHapST = ter->nStringTagHap;


}

void TagShareInit(HWND hwnd)
{
	int i;
	TERMINAL_STRUCT *ter;

	terminalStruct = new TERMINAL_STRUCT[MAX_TERMINAL];
	if(terminalStruct == NULL)	{
		MessageBox(hwnd, "insufficent memory size", "new terminalStruct", MB_OK);
		return;
	}

	for(i = 0; i < MAX_TERMINAL; i++) {
		ter = &terminalStruct[i];

		memset(ter, 0, sizeof(TERMINAL_STRUCT));
		
		ter->terminal = i;
	}

	TagLoadTerminal(hwnd, &terminalStruct[0]);

	GroupTagLoad(hwnd);
}

//------------------------------------------------------------------------------
//	프로그램을 끝내기 전에 태그 메모리를 모두 풀어준다.
// 약간의 문제가 있다. 이 부분을 포함시키면 약간 이상한 문제가 발생한다. (윈도우 다운)
//------------------------------------------------------------------------------

static void TagFreeTerminal(TERMINAL_STRUCT *ter)
{
	if(ter->tagHeader != NULL)		UnmapViewOfFile(ter->tagHeader);
	ter->tagHeader = NULL;

	if(ter->hmmfHeader)		CloseHandle(ter->hmmfHeader);
	ter->hmmfHeader = NULL;
	
	if(ter->analogInput != NULL)	UnmapViewOfFile(ter->analogInput);
	if(ter->analogOutput != NULL)	UnmapViewOfFile(ter->analogOutput);
	if(ter->digitalInput != NULL)	UnmapViewOfFile(ter->digitalInput);
	if(ter->digitalOutput != NULL)	UnmapViewOfFile(ter->digitalOutput);
	if(ter->stringTag != NULL)		UnmapViewOfFile(ter->stringTag);

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

	if(ter->hmmfAI)		CloseHandle(ter->hmmfAI);
	if(ter->hmmfAO)		CloseHandle(ter->hmmfAO);
	if(ter->hmmfDI)		CloseHandle(ter->hmmfDI);
	if(ter->hmmfDO)		CloseHandle(ter->hmmfDO);
	if(ter->hmmfST)		CloseHandle(ter->hmmfST);

	ter->hmmfAI = NULL;
	ter->hmmfAO = NULL;
	ter->hmmfDI = NULL;
	ter->hmmfDO = NULL;
	ter->hmmfST = NULL;
}

void TagShareUnInit()
{
	GroupTagFree();

	if(terminalStruct == NULL)	return;
	
	TERMINAL_STRUCT *ter;
	int i;
	
	for(i = 0; i < MAX_TERMINAL; i++) {
		ter = &terminalStruct[i];
		TagFreeTerminal(ter);
	}

	delete terminalStruct;
	
	terminalStruct = NULL;	
}

TAG_AI_STRUCT *TagShareGetAI(int pos)
{
	return &terminalStruct[0].analogInput[pos];
}

TAG_AO_STRUCT *TagShareGetAO(int pos)
{
	return &terminalStruct[0].analogOutput[pos];
}

TAG_DI_STRUCT *TagShareGetDI(int pos)
{
	return &terminalStruct[0].digitalInput[pos];
}

TAG_DO_STRUCT *TagShareGetDO(int pos)
{
	return &terminalStruct[0].digitalOutput[pos];
}

TAG_ST_STRUCT *TagShareGetST(int pos)
{
	return &terminalStruct[0].stringTag[pos];
}

TAG_AI_STRUCT *TagShareGetAI(char *tag)
{
	// return &terminalStruct[0].analogInput[pos];
	short pos;

	if(!GetTagPosAI(&terminalStruct[0], tag, pos))	return NULL;

	return &terminalStruct[0].analogInput[pos];;
}

int TagShareGetHapAI()
{
	return terminalStruct[0].nAnalogInputHap;
}

int TagShareGetHapAO()
{
	return terminalStruct[0].nAnalogOutputHap;
}

int TagShareGetHapDI()
{
	return terminalStruct[0].nDigitalInputHap;
}

int TagShareGetHapDO()
{
	return terminalStruct[0].nDigitalOutputHap;
}

int TagShareGetHapST()
{
	return terminalStruct[0].nStringTagHap;
}
