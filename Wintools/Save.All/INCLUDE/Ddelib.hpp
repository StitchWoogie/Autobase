#if	!defined (__DDEML_H)
#include <ddeml.h>
#endif

void DdeLibClientInit(HWND hwnd);
void DdeLibClientUninit();
int  DdeLibLinkItemAdvise(char *service, char *topic, char *item, DWORD &pos_s, DWORD &pos_t, DWORD &pos_i);
int  DdeLibLinkItemPoke  (char *service, char *topic, char *item, DWORD &pos_s, DWORD &pos_t, DWORD &pos_i);
//void DdeLibGetData(DWORD service, DWORD topic, DWORD item, char *data);
int  DdeLibGetPokeHandle(DWORD pos_s, DWORD pos_t, DWORD pos_i, HCONV &hconv, HSZ &hszTopic, HSZ &hszItem);
void DdeLibConnectTry();
DWORD DdeLibGetInst();

int DdeLibRequestItem(DWORD service, DWORD topic, DWORD item, char *data, int data_size);


// Debug에 사용되는 함수들
void DdeLibDebugWindowRegister(HINSTANCE hInstance);
void DdeLibDebugWindowShow(HWND hwndFrame, HINSTANCE hInst);

typedef void (CALLBACK* LPFNCALLBACK)(DWORD s, DWORD t, DWORD i, char *data);
void DdeLibSetCallBack(LPFNCALLBACK proc);




