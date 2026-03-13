//------------------------------------------------------------------------------
// protocol main programm
// protocal main 은 밑의 디렉토리에 있는 서로 다른 모든 프로토콜을 불러준다.
//------------------------------------------------------------------------------
#include "stdafx.h"

#include <winbase.h>
#include <string.h>
#include <io.h>

#include <dataswap.h>

#include "..\plc_scan.h"
#include "pro_main.h" 
#include "pro_dde.h"
#include "pro_lib.h" 

PROTOCOL_NAME_DEFINE protocolNameDefine[MAX_PROTOCOL_NAME_DEFINE] = {
		{ "GANTNER",	PROTOCOL_GANTNER },
		{ "SPC-300",	PROTOCOL_SPC_300 },
		{ "MF2",		PROTOCOL_MF2	 },
		{ "AOJ2-C214",	PROTOCOL_AOJ2_C214},
		{ "WR3380",		PROTOCOL_WR3380 },
		{ "DPM2",		PROTOCOL_DPM2 },
		{ "GLOFA",		PROTOCOL_GLOFA},
		{ "MASTER-K",	PROTOCOL_MASTER_K},
		{ "ABPLC5",		PROTOCOL_ABPLC5 },
		{ "LA250",		PROTOCOL_LA250 },
		{ "BL-2300",	PROTOCOL_BL2300},
		{ "PM-170E",	PROTOCOL_PM170E},
		{ "MJ71E71",	PROTOCOL_MJ71E71},
		{ "MODICON",	PROTOCOL_MODICON},
		{ "TMTC",		PROTOCOL_TMTC},
		{ "HOST-LINK",	PROTOCOL_OMRON},
		{ "PM-B",		PROTOCOL_PM_B},
		{ "PT-L",		PROTOCOL_PT_L},
		{ "RLINK",		PROTOCOL_RLINK},
		{ "ADAM",		PROTOCOL_ADAM},
		{ "SP30",		PROTOCOL_SP30},
		{ "GMPC",		PROTOCOL_GMPC},
		{ "PCD",		PROTOCOL_PCD },
		{ "PCD-SBUS",	PROTOCOL_PCD_SBUS },
		{ "MASTER-K10/60/200",	PROTOCOL_MASTER_K_10 },
		{ "MASTER-K30/50",	PROTOCOL_MASTER_K_30 },
		{ "GE-SNP",		PROTOCOL_GESNP },
		{ "Network Client Multi",		PROTOCOL_NETWORK_CLIENT_MULTI },
		{ "Network Client Virtual",		PROTOCOL_NETWORK_CLIENT_VIRTUAL },
};

//-------------------------------------------------------------------------------
//  protocol Version
//	1.0 1996~
//-------------------------------------------------------------------------------

void WaitThreadProtocolDrawWorking(GLOBAL_PORT_STRUCT *pt)
{
	TimeOutClass timeout;

	while(1) {
		if(timeout.IsTimeOut(30))	break;
		if(pt->bThreadProtocolDrawWorking == OFF)	break;
		Sleep(1);
	}

	pt->bThreadProtocolDrawWorking = OFF;
}

void PlcProtocolDrawMethodTitle(GLOBAL_PORT_STRUCT *pt, HDC hdc, int x, int y)
{
	switch(pt->nScanProtocol) {
		case PROTOCOL_DLL:
			WaitThreadProtocolDrawWorking(pt);
			pt->bThreadProtocolDrawWorking = ON;
			pt->dll.ProtocolDrawMethodTitle(hdc, x, y);
			pt->bThreadProtocolDrawWorking = OFF;
			break;		
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			PlcScanDrawMethodTitleNetClientMulti(hdc, x, y);
			break;
		

		case PROTOCOL_GANTNER:
			PlcScanDrawMethodTitleGantner(hdc, x, y);
			break;
		case PROTOCOL_SPC_300:
			PlcScanDrawMethodTitleSpc300(hdc, x, y);
			break;
		case PROTOCOL_MF2:
			PlcScanDrawMethodTitleMF2(hdc, x, y);
			break;
		case PROTOCOL_AOJ2_C214:
			PlcScanDrawMethodTitleAOJ2C214(hdc, x, y);
			break;
		case PROTOCOL_WR3380:
			PlcScanDrawMethodTitleWR3380(hdc, x, y);
			break;
		case PROTOCOL_DPM2:
			PlcScanDrawMethodTitleDPM2(hdc, x, y);
			break;
		case PROTOCOL_GLOFA:
			PlcScanDrawMethodTitleGlofa(hdc, x, y);
			break;
		case PROTOCOL_MASTER_K:
			PlcScanDrawMethodTitleMasterK(hdc, x, y);
			break;
		case PROTOCOL_ABPLC5:
			PlcScanDrawMethodTitleAbplc5(hdc, x, y);
			break;
		case PROTOCOL_LA250:
			PlcScanDrawMethodTitleLA250(hdc, x, y);
			break;
		case PROTOCOL_BL2300:
			PlcScanDrawMethodTitleBL2300(hdc, x, y);
			break;
		case PROTOCOL_PM170E:
			PlcScanDrawMethodTitlePM170E(hdc, x, y);
			break;
		case PROTOCOL_MJ71E71:
			PlcScanDrawMethodTitleMJ71E71(hdc, x, y);
			break;
		case PROTOCOL_MODICON:
			PlcScanDrawMethodTitleModicon(hdc, x, y);
			break;
		case PROTOCOL_TMTC:
			PlcScanDrawMethodTitleTMTC(hdc, x, y);
			break;
		case PROTOCOL_OMRON:
			PlcScanDrawMethodTitleOMRON(hdc, x, y);
			break;
		case PROTOCOL_PM_B:
			PlcScanDrawMethodTitlePM_B(hdc, x, y);
			break;
		case PROTOCOL_PT_L:
			PlcScanDrawMethodTitlePT_L(hdc, x, y);
			break;
		case PROTOCOL_RLINK:
			PlcScanDrawMethodTitleRLINK(hdc, x, y);
			break;
		case PROTOCOL_ADAM:
			PlcScanDrawMethodTitleADAM(hdc, x, y);
			break;
		case PROTOCOL_SP30:
			PlcScanDrawMethodTitleSP30(hdc, x, y);
			break;
		case PROTOCOL_GMPC:
			PlcScanDrawMethodTitleGMPC(hdc, x, y);
			break;
		case PROTOCOL_PCD:
			PlcScanDrawMethodTitlePCD(hdc, x, y);
			break;
		case PROTOCOL_PCD_SBUS:
			PlcScanDrawMethodTitlePCD_SBUS(hdc, x, y);
			break;
		case PROTOCOL_MASTER_K_10:
			PlcScanDrawMethodTitleMasterK10(hdc, x, y);
			break;
		case PROTOCOL_MASTER_K_30:
			PlcScanDrawMethodTitleMasterK30(hdc, x, y);
			break;
		case PROTOCOL_GESNP:
			PlcScanDrawMethodTitleGESNP(hdc, x, y);
			break;
		
	}
}

void PlcProtocolDrawMethod(HDC hdc, int x, int y, GLOBAL_PORT_STRUCT *pt, SCAN_METHOD_STRUCT *sm)
{
	if(y < -30000)	return;

	switch(pt->nScanProtocol) {
		case PROTOCOL_DLL:
			WaitThreadProtocolDrawWorking(pt);
			pt->bThreadProtocolDrawWorking = ON;

			pt->dll.ProtocolDrawMethod(&pt->local, hdc, x, y, sm);

			pt->bThreadProtocolDrawWorking = OFF;
			break;
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			PlcScanDrawMethodNetClientMulti(hdc, x, y, sm);
			break;
		
		case PROTOCOL_GANTNER:
			PlcScanDrawMethodGantner(hdc, x, y, sm);
			break;
		case PROTOCOL_SPC_300:
			PlcScanDrawMethodSpc300(hdc, x, y, sm);
			break;
		case PROTOCOL_MF2:
			PlcScanDrawMethodMF2(hdc, x, y, sm);
			break;
		case PROTOCOL_AOJ2_C214:
			PlcScanDrawMethodAOJ2C214(hdc, x, y, sm);
			break;
		case PROTOCOL_WR3380:
			PlcScanDrawMethodWR3380(hdc, x, y, sm);
			break;
		case PROTOCOL_DPM2:
			PlcScanDrawMethodDPM2(hdc, x, y, sm);
			break;
		case PROTOCOL_GLOFA:
			PlcScanDrawMethodGlofa(hdc, x, y, sm);
			break;
		case PROTOCOL_MASTER_K:
			PlcScanDrawMethodMasterK(hdc, x, y, sm);
			break;
		case PROTOCOL_ABPLC5:
			PlcScanDrawMethodAbplc5(hdc, x, y, sm);
			break;
		case PROTOCOL_LA250:
			PlcScanDrawMethodLA250(hdc, x, y, sm);
			break;
		case PROTOCOL_BL2300:
			PlcScanDrawMethodBL2300(hdc, x, y, sm);
			break;
		case PROTOCOL_PM170E:
			PlcScanDrawMethodPM170E(hdc, x, y, sm);
			break;
		case PROTOCOL_MJ71E71:
			PlcScanDrawMethodMJ71E71(hdc, x, y, sm);
			break;
		case PROTOCOL_MODICON:
			PlcScanDrawMethodModicon(hdc, x, y, sm);
			break;
		case PROTOCOL_TMTC:
			PlcScanDrawMethodTMTC(hdc, x, y, sm);
			break;
		case PROTOCOL_OMRON:
			PlcScanDrawMethodOMRON(hdc, x, y, sm);
			break;
		case PROTOCOL_PM_B:
			PlcScanDrawMethodPM_B(hdc, x, y, sm);
			break;
		case PROTOCOL_PT_L:
			PlcScanDrawMethodPT_L(hdc, x, y, sm);
			break;
		case PROTOCOL_RLINK:
			PlcScanDrawMethodRLINK(hdc, x, y, sm);
			break;
		case PROTOCOL_ADAM:
			PlcScanDrawMethodADAM(hdc, x, y, sm);
			break;
		case PROTOCOL_SP30:
			PlcScanDrawMethodSP30(hdc, x, y, sm);
			break;
		case PROTOCOL_GMPC:
			PlcScanDrawMethodGMPC(hdc, x, y, sm);
			break;
		case PROTOCOL_PCD:
			PlcScanDrawMethodPCD(hdc, x, y, sm);
			break;
		case PROTOCOL_PCD_SBUS:
			PlcScanDrawMethodPCD_SBUS(hdc, x, y, sm);
			break;
		case PROTOCOL_MASTER_K_10:
			PlcScanDrawMethodMasterK10(hdc, x, y, sm);
			break;
		case PROTOCOL_MASTER_K_30:
			PlcScanDrawMethodMasterK30(hdc, x, y, sm);
			break;
		case PROTOCOL_GESNP:
			PlcScanDrawMethodGESNP(hdc, x, y, sm);
			break;
		
	}
}

int PlcProtocolRead(GLOBAL_PORT_STRUCT *pt, int pos)
{
	int retn; 

	switch(pt->nScanProtocol) { 
		case PROTOCOL_DLL:
			retn = pt->dll.ProtocolRead(&pt->local, pos);
			if(retn == COMMUNICATION_ERR_STRING ||
				retn == COMMUNICATION_ERR_STRING_AND_CODEBAD ||
				retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
				StackChar message(1000);
				if(pt->dll.ProtocolGetErrMsg2)
					pt->dll.ProtocolGetErrMsg2(&pt->local, message.data);
				else
					pt->dll.ProtocolGetErrMsg(message.data);
				PlcScanSetErrorString(message.data);
			}
			break;
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			retn = PlcScanReadNetClientMulti(&pt->local, pos);
			break;
		
		case PROTOCOL_GANTNER:
			retn = PlcScanReadGantner(&pt->local, pos);
			break;
		case PROTOCOL_SPC_300:
			retn = PlcScanReadSpc300(&pt->local, pos);
			break;
		case PROTOCOL_MF2:
			retn = PlcScanReadMF2(&pt->local, pos);
			break;
		case PROTOCOL_AOJ2_C214:
			retn = PlcScanReadAOJ2C214(&pt->local, pos);
			break;
		case PROTOCOL_WR3380:
			retn = PlcScanReadWR3380(&pt->local, pos);
			break;
		case PROTOCOL_DPM2:
			retn = PlcScanReadDPM2(&pt->local, pos);
			break;
		case PROTOCOL_GLOFA:
			retn = PlcScanReadGlofa(&pt->local, pos);
			break;
		case PROTOCOL_MASTER_K:
			retn = PlcScanReadMasterK(&pt->local, pos);
			break;
		case PROTOCOL_ABPLC5:
			retn = PlcScanReadAbplc5(&pt->local, pos);
			break;
		case PROTOCOL_LA250:
			retn = PlcScanReadLA250(&pt->local, pos);
			break;
		case PROTOCOL_BL2300:
			retn = PlcScanReadBL2300(&pt->local, pos);
			break;
		case PROTOCOL_PM170E:
			retn = PlcScanReadPM170E(&pt->local, pos);
			break;
		case PROTOCOL_MJ71E71:
			retn = PlcScanReadMJ71E71(&pt->local, pos);
			break;
		case PROTOCOL_MODICON:
			retn = PlcScanReadModicon(&pt->local, pos);
			break;
		case PROTOCOL_TMTC:
			retn = PlcScanReadTMTC(&pt->local, pos);
			break;
		case PROTOCOL_OMRON:
			retn = PlcScanReadOMRON(&pt->local, pos);
			break;
		case PROTOCOL_PM_B:
			retn = PlcScanReadPM_B(&pt->local, pos);
			break;
		case PROTOCOL_PT_L:
			retn = PlcScanReadPT_L(&pt->local, pos);
			break;
		case PROTOCOL_RLINK:
			retn = PlcScanReadRLINK(&pt->local, pos);
			break;
		case PROTOCOL_ADAM:
			retn = PlcScanReadADAM(&pt->local, pos);
			break;
		case PROTOCOL_SP30:
			retn = PlcScanReadSP30(&pt->local, pos);
			break;
		case PROTOCOL_GMPC:
			retn = PlcScanReadGMPC(&pt->local, pos);
			break;
		case PROTOCOL_PCD:
			retn = PlcScanReadPCD(&pt->local, pos);
			break;
		case PROTOCOL_PCD_SBUS:
			retn = PlcScanReadPCD_SBUS(&pt->local, pos);
			break;
		case PROTOCOL_MASTER_K_10:
			retn = PlcScanReadMasterK10(&pt->local, pos);
			break;
		case PROTOCOL_MASTER_K_30:
			retn = PlcScanReadMasterK30(&pt->local, pos);
			break;
		case PROTOCOL_GESNP:
			retn = PlcScanReadGESNP(&pt->local, pos);
			break;
		
		default:
			retn = COMMUNICATION_UNDEFINED_PROTOCOL;
			break;
	}

	return retn;
}

void InsertWriteWaitOne(int out_port, SCAN_WRITE_EXCHANGE_ITEM *item);

int PlcProtocolWriteWord(GLOBAL_PORT_STRUCT *pt, int port, int ss, DWORD address_org, char *extra1, WORD extra2, double value)
{
	int retn;

	char buf[80];
	char sAddress[80];

	DWORD address = address_org;

	if(pt->nScanProtocol != PROTOCOL_DLL) {	// DLL이 아닐때는 4d의 형식의 주소를 사용해야 한다.(옛날 방식 3.05이전버전)
		sprintf(buf, "%04X", address);
		address = atoi(buf);
	}

	sprintf(sAddress, "%04X", address_org);

	switch(pt->nScanProtocol) {
		case PROTOCOL_DLL:
			retn = pt->dll.ProtocolWriteWord(&pt->local, ss, address, value, extra1, extra2, sAddress);
			if(retn == COMMUNICATION_ERR_STRING ||
				retn == COMMUNICATION_ERR_STRING_AND_CODEBAD ||
				retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
				StackChar message(1000);
				if(pt->dll.ProtocolGetErrMsg2)
					pt->dll.ProtocolGetErrMsg2(&pt->local, message.data);
				else
					pt->dll.ProtocolGetErrMsg(message.data);
				PlcScanSetErrorString(message.data);
			}
			break;
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			retn = PlcScanWriteWordNetClientMulti(&pt->local, ss, address_org, value, extra1, extra2, port);
			break;
		case PROTOCOL_NETWORK_CLIENT_VIRTUAL:
			{
				int real_no = atoi(pt->sScanProtocolOption[pt->cDualCurrentActiveDevice]);

				if(real_no == pt->local.no)	return COMMUNICATION_OK;	// Multi와 Virtual이 같을수는 없다

				SCAN_WRITE_EXCHANGE_ITEM item;

				ZeroMemory(&item, sizeof(SCAN_WRITE_EXCHANGE_ITEM));
				item.command = 1;	// AO;
				item.port = pt->local.no;
				item.station = ss;
				item.address = address_org;
				strcpy(item.sExtraAddr, extra1);
				item.wExtraAddr = extra2;
				item.value = value;

				InsertWriteWaitOne(real_no, &item);
				retn = COMMUNICATION_OK;
			}
			break;
		

		case PROTOCOL_GANTNER:
			retn = PlcScanWriteWordGantner(&pt->local, ss, (WORD)address, (WORD)value);
			break;
		case PROTOCOL_SPC_300:
			retn = PlcScanWriteWordSpc300(&pt->local, ss, (WORD)address, (WORD)value);
			break;
		case PROTOCOL_MF2:
			retn = PlcScanWriteWordMF2(&pt->local, ss, (WORD)address, (WORD)value);
			break;
		case PROTOCOL_AOJ2_C214:
			retn = PlcScanWriteWordAOJ2C214(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_WR3380:
			retn = PlcScanWriteWordWR3380(&pt->local, ss, (WORD)address, (WORD)value);
			break;
		case PROTOCOL_DPM2:
			retn = PlcScanWriteWordDPM2(&pt->local, ss, (WORD)address, (float)value, extra1, extra2);
			break;
		case PROTOCOL_GLOFA:
			retn = PlcScanWriteWordGlofa(&pt->local, ss, (WORD)address, (WORD)value, extra1, extra2);
			break;
		case PROTOCOL_MASTER_K:
			retn = PlcScanWriteWordMasterK(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_ABPLC5:
			retn = PlcScanWriteWordAbplc5(&pt->local, ss, (WORD)address, (WORD)value, extra1, extra2);
			break;
		case PROTOCOL_LA250:
			retn = PlcScanWriteWordLA250(&pt->local, ss, (WORD)address, (float)value);
			break;
		case PROTOCOL_BL2300:
			retn = PlcScanWriteWordBL2300(&pt->local, ss, (WORD)address, (float)value);
			break;
		case PROTOCOL_PM170E:
			retn = PlcScanWriteWordPM170E(&pt->local, ss, (WORD)address, (float)value, extra1);
			break;
		case PROTOCOL_MJ71E71:
			retn = PlcScanWriteWordMJ71E71(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_MODICON:
			retn = PlcScanWriteWordModicon(&pt->local, ss, (WORD)address, (float)value, extra1);
			break;
		case PROTOCOL_TMTC:
			retn = PlcScanWriteWordTMTC(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_OMRON:
			retn = PlcScanWriteWordOMRON(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_PM_B:
			retn = PlcScanWriteWordPM_B(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_PT_L:
			retn = PlcScanWriteWordPT_L(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_RLINK:
			retn = PlcScanWriteWordRLINK(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_ADAM:
			retn = PlcScanWriteWordADAM(&pt->local, ss, (WORD)address, (float)value, extra1);
			break;
		case PROTOCOL_SP30:
			retn = PlcScanWriteWordSP30(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break; 
		case PROTOCOL_GMPC:
			retn = PlcScanWriteWordGMPC(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_PCD:
			retn = PlcScanWriteWordPCD(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_PCD_SBUS:
			retn = PlcScanWriteWordPCD_SBUS(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_MASTER_K_10:
			retn = PlcScanWriteWordMasterK10(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_MASTER_K_30:
			retn = PlcScanWriteWordMasterK30(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		case PROTOCOL_GESNP:
			retn = PlcScanWriteWordGESNP(&pt->local, ss, (WORD)address, (WORD)value, extra1);
			break;
		
		default:
			retn = COMMUNICATION_UNDEFINED_PROTOCOL;
			break;
	}

	return retn;
}


int PlcProtocolWriteBit(GLOBAL_PORT_STRUCT *pt, int port, int ss, DWORD address_org, char *extra1, WORD extra2, WORD value)
{
	char buf[80];
	char sAddress[80];
	DWORD address;

	address = address_org;
	if(pt->nScanProtocol != PROTOCOL_DLL) {	// DLL이 아닐때는 3d1x 형식의 주소를 사용해야 한다.(옛날 방식 3.05이전버전)
		sprintf(buf, "%03X", address/16);
		address = atoi(buf)*16+address%16;
	}

	sprintf(sAddress, "%04X", address_org);

	int retn;

	switch(pt->nScanProtocol) {
		case PROTOCOL_DLL:
			retn = pt->dll.ProtocolWriteBit(&pt->local, ss, address, value, extra1, extra2, sAddress);
			if(retn == COMMUNICATION_ERR_STRING ||
				retn == COMMUNICATION_ERR_STRING_AND_CODEBAD ||
				retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
				StackChar message(1000);
				if(pt->dll.ProtocolGetErrMsg2)
					pt->dll.ProtocolGetErrMsg2(&pt->local, message.data);
				else
					pt->dll.ProtocolGetErrMsg(message.data);
				PlcScanSetErrorString(message.data);
			}
			break;
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			retn = PlcScanWriteBitNetClientMulti(&pt->local, ss, address_org, value, extra1, extra2, port);
			break;
		case PROTOCOL_NETWORK_CLIENT_VIRTUAL:
			{
				int real_no = atoi(pt->sScanProtocolOption[pt->cDualCurrentActiveDevice]);

				if(real_no == pt->local.no)	return COMMUNICATION_OK;	// Multi와 Virtual이 같을수는 없다

				SCAN_WRITE_EXCHANGE_ITEM item;

				ZeroMemory(&item, sizeof(SCAN_WRITE_EXCHANGE_ITEM));
				item.command = 0;	// DO;
				item.port = pt->local.no;
				item.station = ss;
				item.address = address_org;
				strcpy(item.sExtraAddr, extra1);
				item.wExtraAddr = extra2;
				item.value = value;

				InsertWriteWaitOne(real_no, &item);

				retn = COMMUNICATION_OK;
			}
			break;
		
		case PROTOCOL_GANTNER:
			retn = PlcScanWriteBitGantner(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_SPC_300:
			retn = PlcScanWriteBitSpc300(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_MF2:
			retn = PlcScanWriteBitMF2(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_AOJ2_C214:
			retn = PlcScanWriteBitAOJ2C214(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_WR3380:
			retn = PlcScanWriteBitWR3380(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_DPM2:
			retn = PlcScanWriteBitDPM2(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_GLOFA:
			retn = PlcScanWriteBitGlofa(&pt->local, ss, (WORD)address, value, extra1, extra2);
			break;
		case PROTOCOL_MASTER_K:
			retn = PlcScanWriteBitMasterK(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_ABPLC5:
			retn = PlcScanWriteBitAbplc5(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_LA250:
			retn = PlcScanWriteBitLA250(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_BL2300:
			retn = PlcScanWriteBitBL2300(&pt->local, ss, (WORD)address, value);
			break;
		case PROTOCOL_PM170E:
			retn = PlcScanWriteBitPM170E(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_MJ71E71:
			retn = PlcScanWriteBitMJ71E71(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_MODICON:
			retn = PlcScanWriteBitModicon(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_TMTC:
			retn = PlcScanWriteBitTMTC(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_OMRON:
			retn = PlcScanWriteBitOMRON(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_PM_B:
			retn = PlcScanWriteBitPM_B(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_PT_L:
			retn = PlcScanWriteBitPT_L(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_RLINK:
			retn = PlcScanWriteBitRLINK(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_ADAM:
			retn = PlcScanWriteBitADAM(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_SP30:
			retn = PlcScanWriteBitSP30(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_GMPC:
			retn = PlcScanWriteBitGMPC(&pt->local, ss, (WORD)address, value, extra1, extra2);
			break;
		case PROTOCOL_PCD:
			retn = PlcScanWriteBitPCD(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_PCD_SBUS:
			retn = PlcScanWriteBitPCD_SBUS(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_MASTER_K_10:
			retn = PlcScanWriteBitMasterK10(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_MASTER_K_30:
			retn = PlcScanWriteBitMasterK30(&pt->local, ss, (WORD)address, value, extra1);
			break;
		case PROTOCOL_GESNP:
			retn = PlcScanWriteBitGESNP(&pt->local, ss, (WORD)address, value, extra1);
			break;
		
		default:
			retn = COMMUNICATION_UNDEFINED_PROTOCOL;
			break;
	}

	return retn;
}

int PlcProtocolWriteBlock(GLOBAL_PORT_STRUCT *pt, int port, int ss, DWORD address_org, char *extra1, WORD extra2, BYTE *value, short array_size, BYTE array_type)
{
	int retn;
	char sAddress[80];
	
	sprintf(sAddress, "%04X", address_org);

	if(pt->dll.ProtocolWriteBlock != NULL) {
		retn = pt->dll.ProtocolWriteBlock(&pt->local, ss, address_org, value, extra1, extra2, sAddress, array_size, array_type);
		if(retn == COMMUNICATION_ERR_STRING ||
			retn == COMMUNICATION_ERR_STRING_AND_CODEBAD ||
			retn == COMMUNICATION_ERR_STRING_AND_TIMEOUT) {
			StackChar message(1000);
			if(pt->dll.ProtocolGetErrMsg2)
				pt->dll.ProtocolGetErrMsg2(&pt->local, message.data);
			else
				pt->dll.ProtocolGetErrMsg(message.data);
			PlcScanSetErrorString(message.data);
		}
	}
	else {
		//retn = COMMUNICATION_UNDEFINED_PROTOCOL;

		CString buf;
		CString atype;

		if(array_type == 1)			atype = "byte";
		else if(array_type == 4)	atype = "ushort";
		else if(array_type == 6)	atype = "uint";
		else if(array_type == 8)	atype = "ulong";
		else if(array_type == 9)	atype = "float";
		else if(array_type == 10)	atype = "double";
		else if(array_type == 11)	atype = "string";
		else						atype = "unknown type";

		buf.Format("This protocol is not supported BLOCK Write. (block_size=%d, block_type=%d(%s))", array_size, array_type, atype);

		PlcScanSetErrorString(buf);
		
		retn = COMMUNICATION_ERR_STRING;
	}

	return retn;
}


Block blockProtocolList(sizeof(DLL_PROTOCOL_LIST));

#define	PLCSCAN_VERSION	8	//	3 - visual C++ 6.0
							//		recv, send code를 port별로 볼수 있게 포트번호를 추가
							//  8 - Visual C++ 7.0  (AutoBase 8.0부터 적용)

void MessageBoxLastError(HWND hwnd, const char *title)
{
	GetLastErrorClass error;

	MessageBox(hwnd, error.GetString(), title, MB_OK);
}

void SaveProtocolList(HWND hwnd)
{
	CString filename;

	filename.Format("%s\\protocol\\protocol.lst", sDirProgramm);
	FILE *out;
	//StackChar buf(5000);
	CommaBlockString comma;
	DLL_PROTOCOL_LIST item;
	DWORD l;

	out = fopen(filename, "w");
	if(out == NULL) {
		CString msg;
		msg.Format("Can't write file.\n%s", filename);
		MessageBox(hwnd, msg, "File Open Error", MB_OK);
		return;
	}
	
	for(l = 0; l < blockProtocolList.GetCount(); l++) {
		blockProtocolList.GetBlock(&item, l);
		fprintf(out, "%20s,", item.filename);
		fprintf(out, "%08X,", item.ft.dwLowDateTime);
		fprintf(out, "%08X,", item.ft.dwHighDateTime);
		fprintf(out, "%10d,", item.size);
		fprintf(out, "%d,", item.bProtocol);
		fprintf(out, "%s,", item.title);
		fprintf(out, "%d,", item.VersionMajor);
		fprintf(out, "%d,", item.VersionMinor);
		fprintf(out, "\n");
	}
	fclose(out);
}

static int CompareProtocolOne(DLL_PROTOCOL_LIST *item)
{
	DWORD l;
	DLL_PROTOCOL_LIST *src;
	
	for(l = 0; l < blockProtocolList.GetCount(); l++) {
		src = (DLL_PROTOCOL_LIST *)blockProtocolList.GetPtr(l);
		if(stricmp(src->filename, item->filename) == 0) {
			if(item->size != src->size)	return 0;
			if(item->ft.dwLowDateTime != src->ft.dwLowDateTime)		return 0;
			if(item->ft.dwHighDateTime != src->ft.dwHighDateTime)	return 0;

			item->bProtocol = src->bProtocol;
			strcpy(item->title, src->title);
			item->VersionMajor = src->VersionMajor;
			item->VersionMinor = src->VersionMinor;

			blockProtocolList.DeleteBlock(l);
			return 1;
		}
	}

	return 0;
}

void CompareDllProtocolList(HWND hwnd, char all_update)
{
	char	change_flag = OFF;
	CString filename;
	CString msg;
	HINSTANCE hdll;

typedef void (WINAPI* LPFNPROTOCOLGETDRIVERTITLE) (char *title);
typedef WORD (WINAPI* LPFNPROTOCOLGETDRIVERVERSIONMAJOR) ();
typedef WORD (WINAPI* LPFNPROTOCOLGETDRIVERVERSIONMINOR) ();
	LPFNPROTOCOLGETDRIVERTITLE ProtocolGetDriverTitle;
	LPFNPROTOCOLGETDRIVERVERSIONMAJOR ProtocolGetDriverVersionMajor;
	LPFNPROTOCOLGETDRIVERVERSIONMINOR ProtocolGetDriverVersionMinor;
	DLL_PROTOCOL_LIST item;
	//WORD dll_version;

	CFileFind finder;
	Block block(sizeof(DLL_PROTOCOL_LIST));

	filename.Format("%s\\protocol\\*.dll", sDirProgramm);

	BOOL bWorking = finder.FindFile(filename);
	while (bWorking) {
		
		bWorking = finder.FindNextFile();

		ZeroMemory(&item, sizeof(DLL_PROTOCOL_LIST));

		strcpy(item.filename, finder.GetFileName());
		strcpy(item.title, "Not protocol file");
		item.bProtocol = OFF;
		item.size = (DWORD)finder.GetLength();
		finder.GetLastWriteTime(&item.ft);

		if(all_update == FALSE && CompareProtocolOne(&item)) {
			goto next;
		}

		change_flag = ON;

		hdll = LoadLibrary(finder.GetFilePath());
		if(hdll == NULL) {
			msg.Format("LoadLibrary failed (%s file)", finder.GetFileName());
			MessageBoxLastError(hwnd, msg); 
			goto next;
		}
		ProtocolGetDriverTitle = (LPFNPROTOCOLGETDRIVERTITLE) GetProcAddress(hdll, "ProtocolGetDriverTitle");
		if(ProtocolGetDriverTitle == NULL) {
			FreeLibrary(hdll);
			goto next;
		}

		strcpy(item.filename, finder.GetFileName());
		ProtocolGetDriverTitle(item.title);

		ProtocolGetDriverVersionMajor = (LPFNPROTOCOLGETDRIVERVERSIONMAJOR) GetProcAddress(hdll, "ProtocolGetDriverVersionMajor");
		if(ProtocolGetDriverVersionMajor == NULL) {
			msg.Format("DLL Major Version information not found.\nfilename=%s", finder.GetFileName());
			MessageBox(hwnd, msg, "Unknown Version", MB_OK);

			FreeLibrary(hdll);
			goto next;
		}

		item.VersionMajor = ProtocolGetDriverVersionMajor();

		if(item.VersionMajor != PLCSCAN_VERSION) {
			msg.Format("DLL Major Version mismatched.\nPLC_SCAN Version = %d\n%s file Version = %d", PLCSCAN_VERSION, finder.GetFileName(), item.VersionMajor);
			MessageBox(hwnd, msg, "Major Version Mismatched", MB_OK);
			FreeLibrary(hdll);
			goto next;
		}

		ProtocolGetDriverVersionMinor = (LPFNPROTOCOLGETDRIVERVERSIONMINOR) GetProcAddress(hdll, "ProtocolGetDriverVersionMinor");
		if(ProtocolGetDriverVersionMinor == NULL) {
			msg.Format("DLL Minor Version information not found.\nfilename=%s", finder.GetFileName());
			MessageBox(hwnd, msg, "Unknown Version", MB_OK);

			FreeLibrary(hdll);
			goto next;
		}

		item.VersionMinor = ProtocolGetDriverVersionMinor();

		FreeLibrary(hdll);

		item.bProtocol = ON;

		next:

		block.AddBlock(&item);
	}

	//Block blockProtocolList(sizeof(DLL_PROTOCOL_LIST));
	blockProtocolList.DeleteAllBlock();
	BlockCopy(&blockProtocolList, &block);

	if(change_flag) {
		SaveProtocolList(hwnd);		
	}
}

void LoadProtocolList()
{
	CString filename;

	filename.Format("%s\\protocol\\protocol.lst", sDirProgramm);
	FILE *in;
	StackChar buf(5000);
	CommaBlockString comma;
	DLL_PROTOCOL_LIST item;

	in = fopen(filename, "rb");
	if(in != NULL) {
		while(1) {
			if(!TextGetOneLine(in, buf.data, 5000))	break;
			if(strlen(buf.data) == 0)	continue;
			comma.Set(buf.data);
			ZeroMemory(&item, sizeof(DLL_PROTOCOL_LIST));
			comma.GetString(item.filename, sizeof(item.filename));
			comma.GetHexDWORD(item.ft.dwLowDateTime);
			comma.GetHexDWORD(item.ft.dwHighDateTime);
			comma.GetDWORD(item.size);
			comma.GetChar(item.bProtocol);
			comma.GetString(item.title, sizeof(item.title));
			comma.GetInt(item.VersionMajor);
			comma.GetInt(item.VersionMinor);
			blockProtocolList.AddBlock(&item);
		}
		fclose(in);
	}

	CompareDllProtocolList(hwndMainFrame, FALSE);
}

void AddProtocolListToComboBox(HWND hwnd, HWND hwndCombo)
{
	DWORD l;
	char buf[80];
	DLL_PROTOCOL_LIST item;
	
	for(l = 0; l < blockProtocolList.GetCount(); l++) {
		blockProtocolList.GetBlock(&item, l);
		if(item.bProtocol) {
			sprintf(buf, "DLL-%s", item.title);
			SendMessage(hwndCombo, CB_ADDSTRING, 0, (LPARAM)buf);
		}
	}
}

static void MessageBoxNoConfig(HWND hwnd, char *protocol)
{
	if(IsLangKorean()) {
		MessageBox(hwnd, "이 프로토콜은 프로토콜 설정 대화상자를\n지원하지 않습니다.", protocol, MB_OK);
	}else {
		MessageBox(hwnd, "This protocol is not supported Config Dialog.", protocol, MB_OK);
	}
	return;
}

int ConfigProtocolOption(HWND hwnd, HWND hwndEdit, int port, char *protocol_name, char *protocol_option)
{
	// 이미 "Protocol Option" 창이 열려있는지 확인 250828 PSU 추가
	try {
		HWND hExistingWnd = FindWindow(NULL, "Protocol Option");
		if(hExistingWnd && IsWindow(hExistingWnd)) {
			// 이미 열린 창을 앞으로 가져오기
			if(IsIconic(hExistingWnd)) {
				ShowWindow(hExistingWnd, SW_RESTORE);
			}

			// 강제로 최상위에 표시
			SetWindowPos(hExistingWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
			SetWindowPos(hExistingWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
			SetForegroundWindow(hExistingWnd);
			BringWindowToTop(hExistingWnd);

			return 1;
		}
	} catch(...) {
		// 오류 발생 시 무시하고 계속 진행
	}


	if(strncmp(protocol_name, "DLL-", 4) != 0) {
		MessageBoxNoConfig(hwnd, protocol_name);
		return 0;
	}
	
	char filename[MAXPATH];
	char msg[160];
	int retn;
	HINSTANCE hdll;

	typedef int (WINAPI* LPFNPROTOCOLCONFIGOPTION) (HWND hwnd, HWND hwndEdit, int port, char *string);
	LPFNPROTOCOLCONFIGOPTION ProtocolConfigOption;

	DLL_PROTOCOL_LIST item;
	DWORD l;
	CString caller;

	for(l = 0; l < blockProtocolList.GetCount(); l++) {
		blockProtocolList.GetBlock(&item, l);		
		if(strcmp(&protocol_name[4], item.title) != 0)	continue;	// not seek

		sprintf(filename, "%s\\protocol\\%s", sDirProgramm, item.filename);
		hdll = LoadLibrary(filename);
		if(hdll == NULL) {
			sprintf(msg, "LoadLibrary failed (%s file)", filename);
			MessageBox(hwnd, msg, "DLL Error", MB_OK);
			return 0;
		}

		/*
		// DLL 속에서 할수도 있지만 여기서 할수도 있다.
		//HINSTANCE hInstOld = AfxGetResourceHandle();
		//AfxSetResourceHandle(hdll);
		caller.Format("%s\\CallProtocol_%d_%d.exe", sDirProgramm, item.VersionMajor, item.VersionMinor);
		if(access(caller, 0) == 0) {
			caller.Format("%s\\CallProtocol_%d_%d.exe %s", sDirProgramm, item.VersionMajor, item.VersionMinor, item.filename);
			return 0;
		}
		*/

		ProtocolConfigOption = (LPFNPROTOCOLCONFIGOPTION) GetProcAddress(hdll, "ProtocolConfigOption_8_2");
		if(ProtocolConfigOption) {	
			retn = ProtocolConfigOption(hwnd, hwndEdit, port, protocol_option);
		}
		else {
			FreeLibrary(hdll);
			//AfxSetResourceHandle(hInstOld);
			MessageBoxNoConfig(hwnd, protocol_name);
			return 0;
		}
		
		FreeLibrary(hdll);
		//AfxSetResourceHandle(hInstOld);
		if(retn == IDOK)	return 1;
		return 0;
	}

	MessageBoxNoConfig(hwnd, protocol_name);
	return 0;
}

static void NoSeekProcMessage(HWND hwnd, char *filename, char *proc)
{
	char buf[160];
	if(IsLangKorean()) {
		sprintf(buf, "%s\nProc을 찾을 수 없습니다.\nProtocol 파일이 아니거나 Version이 다름", proc);
	} else {
		sprintf(buf, "%s\nProc not found.\nIt's not Protocol file or Version mismatched", proc);
	}
	MessageBox(hwnd, buf, filename, MB_OK);
}

static int SetProtocolProc(HWND hwnd, HINSTANCE hdll, char *filename, char *sProc, FARPROC procToSet)
{
	LPFNSETPROC proc;
	
	proc = (LPFNSETPROC) GetProcAddress(hdll, sProc);
	if(proc == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(hdll);
		return 0;
	}
	proc(procToSet);

	return 1;
}

static int SetProtocolProcNoMessage(HWND hwnd, HINSTANCE hdll, char *filename, char *sProc, FARPROC procToSet)
{
	LPFNSETPROC proc;
	
	proc = (LPFNSETPROC) GetProcAddress(hdll, sProc);
	if(proc != NULL) {
		proc(procToSet);
	}

	return 1;
}

static int  ByDriverPokeValuePortWORD (int port, int address, WORD val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufWORD == NULL)	return 0;
	PokeNewWORD(&portBuf[port].local, address, val);
	return 1;
}

static int  ByDriverPokeValuePortDWORD (int port, int address, DWORD val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufDWORD == NULL)	return 0;
	PokeNewDWORD(&portBuf[port].local, address, val);
	return 1;
}

static int  ByDriverPokeValuePortFLOAT (int port, int address, double val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufFLOAT == NULL)	return 0;
	PokeNewFLOAT(&portBuf[port].local, address, (float)val);
	return 1;
}

static int  ByDriverPokeValuePortSTRING (int port, int address, char* val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufSTRING == NULL)	return 0;
	PokeNewSTRING(&portBuf[port].local, address, val);
	return 1;
}

static int  ByDriverPokeValuePortDOUBLE (int port, int address, double val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufDOUBLE == NULL)	return 0;
	PokeNewDOUBLE(&portBuf[port].local, address, val);
	return 1;
}

static int  ByDriverPokeValuePortINT64 (int port, int address, __int64 val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufINT64 == NULL)	return 0;
	PokeNewINT64(&portBuf[port].local, address, val);
	return 1;
}

static WORD ByDriverPeekValuePortWORD (int port, int address) 
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufWORD == NULL)	return 0;
	return PeekValueWORD(&portBuf[port].local, address);
}

static DWORD ByDriverPeekValuePortDWORD (int port, int address)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufDWORD == NULL)	return 0;
	return PeekValueDWORD(&portBuf[port].local, address);
}

static double ByDriverPeekValuePortFLOAT (int port, int address)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufFLOAT == NULL)	return 0;
	return PeekValueFLOAT(&portBuf[port].local, address);
}

static int  ByDriverPeekValuePortSTRING (int port, int address, char* val)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufSTRING == NULL)	return 0;
	return PeekValueSTRING(&portBuf[port].local, address, val);
}

static double ByDriverPeekValuePortDOUBLE (int port, int address)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufDOUBLE == NULL)	return 0;
	return PeekValueDOUBLE(&portBuf[port].local, address);
}

static __int64  ByDriverPeekValuePortINT64 (int port, int address)
{
	if(port >= nPortHap)	return 0;
	if(portBuf[port].local.bufINT64 == NULL)	return 0;
	return PeekValueINT64(&portBuf[port].local, address);
}

int AddWaitWriteDigitalOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr, WORD flag);
int AddWaitWriteAnalogOut(int port, int station, DWORD address, char *sExtraAddr, WORD wExtraAddr,  double value);


static int GetDllProtocol(HWND hwnd, GLOBAL_PORT_STRUCT *pt, const char *protocol)
{
	//ReadyDllProtocolList(hwnd);
	
	char filename[MAXPATH];

	DLL_PROTOCOL_LIST item;
	DWORD l;

	for(l = 0; l < blockProtocolList.GetCount(); l++) {
		blockProtocolList.GetBlock(&item, l);
		if(stricmp(protocol, item.title) == 0 && item.bProtocol)	goto ok_seek;
	}
	return 0;
ok_seek:
	sprintf(filename, "%s\\protocol\\%s", sDirProgramm, item.filename);

	pt->dll.hInst = LoadLibrary(filename);
	if(pt->dll.hInst == NULL) {
		MessageBox(hwnd, filename, "Protocol 파일을 찾을 수 없습니다.", MB_OK);
		return 0;
	}

	char sProc[80];

	strcpy(sProc, "ProtocolInit");
	pt->dll.ProtocolInit = (LPFNPROTOCOLINIT) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolInit == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}
	strcpy(sProc, "ProtocolUnInit");
	pt->dll.ProtocolUnInit = (LPFNPROTOCOLUNINIT) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolUnInit == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}
	strcpy(sProc, "ProtocolDrawMethodTitle");
	pt->dll.ProtocolDrawMethodTitle = (LPFNPROTOCOLDRAWMETHODTITLE) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolDrawMethodTitle == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}

	strcpy(sProc, "ProtocolDrawMethod");
	pt->dll.ProtocolDrawMethod = (LPFNPROTOCOLDRAWMETHOD) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolDrawMethod == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}
	strcpy(sProc, "ProtocolRead");
	pt->dll.ProtocolRead = (LPFNPROTOCOLREAD) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolRead == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}
	strcpy(sProc, "ProtocolWriteBit");
	pt->dll.ProtocolWriteBit = (LPFNPROTOCOLWRITEBIT) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolWriteBit == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}
	
	strcpy(sProc, "ProtocolWriteWord");
	pt->dll.ProtocolWriteWord = (LPFNPROTOCOLWRITEWORD) GetProcAddress(pt->dll.hInst, sProc);
	if(pt->dll.ProtocolWriteBit == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);
		FreeLibrary(pt->dll.hInst);
		return 0;
	}

	// 다음 프로토콜 버전에서는 삭제되어야 한다.
	strcpy(sProc, "ProtocolGetErrMsg");		// 8.4.3이전에 개발된 프로토콜에는 있고 이후개발된 프로토콜에는 정보가 비어있다.
	pt->dll.ProtocolGetErrMsg = (LPFNPROTOCOLGETERRMSG) GetProcAddress(pt->dll.hInst, sProc);

	strcpy(sProc, "ProtocolGetErrMsg2");	// 8.4.3에서 추가되었다. pt인자가 추가되었다. 이전 프로토콜은 없다.
	pt->dll.ProtocolGetErrMsg2 = (LPFNPROTOCOLGETERRMSG2) GetProcAddress(pt->dll.hInst, sProc);

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcDisplaySendCode", 
						(FARPROC)DisplaySendCode) == 0)				return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcDisplaySendCodeNextLine", 
						(FARPROC)DisplaySendCodeNextLine) == 0)		return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcDisplayRecvCodeNextLine", 
						(FARPROC)DisplayRecvCodeNextLine) == 0)		return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcDisplayRecvCode", 
						(FARPROC)DisplayRecvCode) == 0)				return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPlcDeviceReadContinue", 
						(FARPROC)PlcDeviceReadContinue) == 0)		return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPlcDeviceWriteContinue", 
						(FARPROC)PlcDeviceWriteContinue) == 0)		return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPlcDeviceClear", 
						(FARPROC)PlcDeviceClear) == 0)				return 0;

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPlcDeviceEnable", 
						(FARPROC)PlcDeviceEnable) == 0)				return 0;

	/*
	// Protocol 4에서 정식 등록 (AutoBase 8.0)
	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPlcDeviceEnable", 
						(FARPROC)PlcDeviceEnable);
	*/
	

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPokeWORD", 
						(FARPROC)PokeWORD) == 0)					return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPokeDWORD", 
						(FARPROC)PokeDWORD) == 0)					return 0;
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPokeFLOAT", 
						(FARPROC)PokeFLOAT) == 0)					return 0;

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcAddWaitWriteDigitalOut", 
						(FARPROC)AddWaitWriteDigitalOut) == 0)		return 0;

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcAddWaitWriteAnalogOut", 
						(FARPROC)AddWaitWriteAnalogOut) == 0)		return 0;
 
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcMessageDisplay", 
						(FARPROC)MessageDisplay) == 0)				return 0;

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcCodeStartEnd", 
						(FARPROC)ProtocolSetCodeStartEnd) == 0)		return 0;

	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcCodeMode", 
						(FARPROC)ProtocolSetCodeMode) == 0)			return 0;

	// Protocol 8에서 정식 등록 (AutoBase 8.0)
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPlcDeviceSetCommState", 
						(FARPROC)PlcDeviceSetCommState) == 0)			return 0;

	// Protocol 8에서 정식 등록 (AutoBase 8.0)
	if(SetProtocolProc(hwnd, pt->dll.hInst, filename,
						"ProtocolSetProcPokeSTRING", 
						(FARPROC)PokeSTRING) == 0)			return 0;

	// 다음 버전에서 정식 등록 할 것
	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPlcDeviceSetCommBreak",
						(FARPROC)PlcDeviceSetCommBreak); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPlcDeviceClearCommBreak",
						(FARPROC)PlcDeviceClearCommBreak); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPokeValuePortWORD",
						(FARPROC)ByDriverPokeValuePortWORD); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPokeValuePortDWORD",
						(FARPROC)ByDriverPokeValuePortDWORD); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPokeValuePortFLOAT",
						(FARPROC)ByDriverPokeValuePortFLOAT); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPokeValuePortSTRING",
						(FARPROC)ByDriverPokeValuePortSTRING); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPeekValuePortWORD",
						(FARPROC)ByDriverPeekValuePortWORD); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPeekValuePortDWORD",
						(FARPROC)ByDriverPeekValuePortDWORD); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPeekValuePortFLOAT",
						(FARPROC)ByDriverPeekValuePortFLOAT); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPeekValuePortSTRING",
						(FARPROC)ByDriverPeekValuePortSTRING); 

	// Protocol Version 8.7 에서 추가 double int64

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPokeValuePortDOUBLE",
						(FARPROC)ByDriverPokeValuePortDOUBLE); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPokeValuePortINT64",
						(FARPROC)ByDriverPokeValuePortINT64); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPeekValuePortDOUBLE",
						(FARPROC)ByDriverPeekValuePortDOUBLE); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPeekValuePortINT64",
						(FARPROC)ByDriverPeekValuePortINT64); 

	// 8.9 에서 추가 2012-5-23
	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPlcDeviceGetInfo",
						(FARPROC)PlcDeviceGetInfo); 

	SetProtocolProcNoMessage(hwnd, pt->dll.hInst, filename, 
						"ProtocolSetProcPlcDeviceSetInfo",
						(FARPROC)PlcDeviceSetInfo); 

	// Protocol Version 8.8 에서 추가 문자열을 쓰기 위한 부분 2010-12-16
	strcpy(sProc, "ProtocolWriteBlock");	
	pt->dll.ProtocolWriteBlock = (LPFNPROTOCOLWRITEBLOCK) GetProcAddress(pt->dll.hInst, sProc);

	// struct를 비교한다.
	LPFNPROTOCOLCHECKSTRUCT procCheckStruct;
	strcpy(sProc, "ProtocolCheckStruct");
	procCheckStruct = (LPFNPROTOCOLCHECKSTRUCT) GetProcAddress(pt->dll.hInst, sProc);
	if(procCheckStruct == NULL) {
		NoSeekProcMessage(hwnd, filename, sProc);	
		FreeLibrary(pt->dll.hInst);
		return 0;
	}
	int retn = procCheckStruct(sizeof(LOCAL_PORT_STRUCT), sizeof(SCAN_METHOD_STRUCT), sizeof(DEVICE_STRUCT));
	if(retn == 0) {
		// o.k size match
	}
	else if(retn == 1) {
		MessageBox(hwnd, "LOCAL_PORT_STRUCT Size가 서로 맞지않습니다.\nDLL과 plc_scan의 버전이 서로 다름", filename, MB_OK);
		FreeLibrary(pt->dll.hInst);
		return 0;	
	}
	else if(retn == 2) {
		MessageBox(hwnd, "SCAN_METHOD_STRUCT Size가 서로 맞지않습니다.\nDLL과 plc_scan의 버전이 서로 다름", filename, MB_OK);
		FreeLibrary(pt->dll.hInst);
		return 0;	
	}
	else if(retn == 3) {
		MessageBox(hwnd, "DEVICE_STRUCT Size가 서로 맞지않습니다.\nDLL과 plc_scan의 버전이 서로 다름", filename, MB_OK);
		FreeLibrary(pt->dll.hInst);
		return 0;	
	}
	else {
		MessageBox(hwnd, "STRUCT Size가 서로 맞지않습니다.\nDLL과 plc_scan의 버전이 서로 다름", filename, MB_OK);
		FreeLibrary(pt->dll.hInst);
		return 0;	
	}
	
	return 1;
}

static int PlcProtocolGetProtocol(HWND hwnd, GLOBAL_PORT_STRUCT *pt, const char *string)
{
	int value = 0;	// debug mode
	int i;

	for(i = 0; i < MAX_PROTOCOL_NAME_DEFINE; i++) {
		if(strcmp(string, protocolNameDefine[i].name) == 0) {
			value = protocolNameDefine[i].protocol;
			return value;
		}
	}

	if(strncmp(string, "DLL-", 4) == 0) {
		if(GetDllProtocol(hwnd, pt, &string[4])) {
			value = PROTOCOL_DLL;
		}
		else {
			value = PROTOCOL_NULL;
		}
	}
	else {
		value = PROTOCOL_NULL;
	}

//	nProtocol = value;

	return value;
}

int PlcProtocolInitDDE(LOCAL_PORT_STRUCT *port);
int PlcProtocolUnInitDDE(LOCAL_PORT_STRUCT *port);

void PlcProtocolInitGeSnp(LOCAL_PORT_STRUCT *port);
void PlcProtocolUnInitGeSnp(LOCAL_PORT_STRUCT *port);

void PlcProtocolInitNetClientMulti(LOCAL_PORT_STRUCT *port);
void PlcProtocolUnInitNetClientMulti(LOCAL_PORT_STRUCT *port);

void PlcProtocolInitOne(HWND hwnd, GLOBAL_PORT_STRUCT *pt, char bBasicOrDual)
{
	if(!pt->bActiveFlag)	return;

	CString sProtocol;

	if(bBasicOrDual == 0) {
		sProtocol = pt->sScanProtocol;
		strcpy(pt->local.sScanProtocolOption,  pt->sScanProtocolOption[0]);
	}
	else {
		if(pt->bDualUseProtocol) {
			sProtocol = pt->sDualProtocol;
			strcpy(pt->local.sScanProtocolOption,  pt->sScanProtocolOption[1]);
		}
		else 
		{
			sProtocol = pt->sScanProtocol;
			strcpy(pt->local.sScanProtocolOption,  pt->sScanProtocolOption[0]);
		}
	}

	pt->nScanProtocol = PlcProtocolGetProtocol(hwnd, pt, sProtocol);

	if(!pt->nScanProtocol) {
		CString message;		
		CString title;

		if(IsLangKorean()) {
			message.Format("알 수 없는 PROTOCOL 종류입니다.\n%s", sProtocol);
		} else {
			message.Format("undefined PROTOCOL type.\n%s", sProtocol);
		}
		title.Format("Port%03d", pt->local.no);
		
		//MessageBox(hwnd, message, title, MB_OK);
		MessageDisplay(message);
		return;
	}

	switch(pt->nScanProtocol) {
		case PROTOCOL_DLL:
			pt->dll.ProtocolInit(hwnd, &pt->local);
			break;
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			PlcProtocolInitNetClientMulti(&pt->local);
			break;
		case PROTOCOL_GESNP:
			PlcProtocolInitGeSnp(&pt->local);
			break;
		
	}
}

void PlcProtocolUnInitOne(GLOBAL_PORT_STRUCT *port)
{
	switch(port->nScanProtocol) {
		case PROTOCOL_DLL:
			port->dll.ProtocolUnInit(&port->local);
			FreeLibrary(port->dll.hInst);
			break;
		case PROTOCOL_NETWORK_CLIENT_MULTI:
			PlcProtocolUnInitNetClientMulti(&port->local);
			break;
		case PROTOCOL_GESNP:
			PlcProtocolUnInitGeSnp(&port->local);
			break;
		
	}
}

