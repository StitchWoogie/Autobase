#if	!defined (__MODEMLIB_H)
#define __MODEMLIB_H

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#pragma pack(push, 1)

typedef struct {
	HANDLE idDevice;	// comport의 디바이스
	BYTE	cPort;
	DWORD	lBaud;
	char	cData;
	char	cStop;
	char	cParity;
	char    bNullOrModem;		// 0 - NULL Modem
								// 1 - 232 modem
} MODEM_STRUCT;

int ModemInstall(HWND hwnd, MODEM_STRUCT *modem, char message_flag);
int ModemUninstall(MODEM_STRUCT *modem);

int ModemWriteContinue(MODEM_STRUCT *modem, char *string, int size);
int ModemReadContinue (MODEM_STRUCT *modem, char *string, int size);
int ModemWriteString(MODEM_STRUCT *modem, LPSTR string, ...);

int ModemWrite(MODEM_STRUCT *mode, char ch);
int ModemStatus(MODEM_STRUCT *modem);

int  ModemWaitOK(MODEM_STRUCT *modem);
int  ModemWaitString(MODEM_STRUCT *modem, char *string);
void ModemClear(MODEM_STRUCT *modem);

int ModemHangUp(MODEM_STRUCT *modem);

void ConvertModemStructToBuf(char *buf, MODEM_STRUCT *modem);
void ConvertBufToModemStruct(const char *buf, MODEM_STRUCT *modem);

#pragma pack(pop)

#endif



