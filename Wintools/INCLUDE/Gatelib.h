#if	!defined(__GATELIB_H)
#define __GATELIB_H

#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#if	!defined (__MODEMLIB_H)
#include <modemlib.h>
#endif

#if	!defined (__TCPIPLIB_H)
#include <tcpiplib.h>
#endif

#if	!defined (__UDPIPLIB_H)
#include <udpiplib.h>
#endif

#pragma pack(push, 1)

enum {
	GATE_TYPE_NULL,				// 아무것도 연결되어 있지 않다.
	GATE_TYPE_MODEM,				// modem type
	GATE_TYPE_TCPIP,				// tcp/ip type
	GATE_TYPE_UDPIP,				// udp/ip type
};

typedef HGLOBAL HGATE;

int GateGetConnectType(HGATE hGate);
int GateGetStruct(HGATE hGate, TCPIP_STRUCT *tcpip);
HGATE GateRegister(MODEM_STRUCT *modem);
HGATE GateRegister(TCPIP_STRUCT *tcpip);
HGATE GateRegister(UDPIP_STRUCT *udpip);
void GateUninstall(HGATE hGate);
int  GateWrite(HGATE hGate, char ch);
int  GateWriteContinue(HGATE hGate, char *buf, int size);
int  GateWriteString(HGATE hGate, LPSTR string, ...);
int  GateReadContinue (HGATE hGate, char *string, int size);
int  GateHangUp(HGATE hGate);
void GateGetInformationString(HGATE hGate, char *info);
void GateClear(HGATE hGate);
int  GateWaitString(HGATE hGate, char *string);

#pragma pack(pop)

#endif

