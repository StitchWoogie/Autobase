#include "stdafx.h"

#include <compiler.hpp>
#include <ddelib.hpp>

#include "Win32Common.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

extern "C" BOOL DLLEXPORT LinkItemAdvise(TCHAR *service, TCHAR *topic, TCHAR *item, DWORD *pos_s, DWORD *pos_t, DWORD *pos_i)
{
	DWORD s,t,i;
	int retn = DdeLibLinkItemAdvise(service, topic, item, s, t, i);

	*pos_s = s;
	*pos_t = t;
	*pos_i = i;

	if(retn)	return TRUE;
	else		return FALSE;
}

extern "C" void DLLEXPORT ClientInit()
{
	DdeLibClientInit(NULL);
}

extern "C" void DLLEXPORT ClientUnInit()
{
	DdeLibClientUninit();
}

extern "C" BOOL DLLEXPORT RequestItem(DWORD service, DWORD topic, DWORD item, LPTSTR data, long data_size)
{
	int retn = DdeLibRequestItem(service, topic, item, data, data_size);

	if(retn)	return TRUE;
	else		return FALSE;
}

extern "C" BOOL DLLEXPORT ChangedItem(DWORD service, DWORD topic, DWORD item, LPTSTR data, long data_size)
{
	int retn = DdeLibRequestItem(service, topic, item, data, data_size);

	if(retn)	return TRUE;
	else		return FALSE;
}

extern "C" void DLLEXPORT ConnectTry()
{
	DdeLibConnectTry();
}

extern "C" BOOL DLLEXPORT Transaction(char *topic, DWORD pos_s, DWORD pos_t, DWORD pos_i, char *buf, char *err_msg)
{
	HDDEDATA retn;
	HCONV hconv;
	HSZ   hszTopic, hszItem;

	if(!DdeLibGetPokeHandle(pos_s, pos_t, pos_i, hconv, hszTopic, hszItem)) 
	{
		sprintf(err_msg, "Can't get handle");
		return FALSE;
	}

	if(hconv == 0) 
	{
		sprintf(err_msg, "Topic[%s] not connected.", topic);
		return FALSE;	
	}

	HDDEDATA hDDE;
	DWORD dwResult;

	hDDE = DdeCreateDataHandle(DdeLibGetInst(), (LPBYTE)buf, strlen(buf)+1, 0, hszItem, CF_TEXT, 0);

	if(hDDE == NULL) 
	{
		sprintf(err_msg, "memory insufficent");
		return FALSE;	
	}

	// 2000은 timeout 시간 2 sec 바로 돌아오기를 원할 때는 TIMEOUT_ASYNC 사용
	retn = DdeClientTransaction((LPBYTE)hDDE, -1, hconv, hszItem, CF_TEXT, XTYP_POKE, TIMEOUT_ASYNC, &dwResult);
	if(retn == FALSE) 
	{
		sprintf(err_msg, "ErrorCode=%d", DdeGetLastError(DdeLibGetInst()));		
	}

	if(retn == (HDDEDATA)TRUE)	return TRUE;
	else						return FALSE;		
}

/*
// 이벤트 C# CallBack은 여러번 실행하면 다운된다.
extern "C" void DLLEXPORT SetCallBack(LPFNCALLBACK proc)
{
	DdeLibSetCallBack(proc);
}
*/








