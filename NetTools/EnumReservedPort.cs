using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetTools
{
    /// <summary>
    /// IANA 제안에 근거함
    /// 공식적인 포트와 비공식적인 포트가 있다. 
    /// 0~49151 포트는 할당받아서 사용하는 것이 좋다.
    /// 49152~65535 포트는 동적 또는 개인포트로 사용
    /// 
    /// System Ports (0~1023)
    /// User Ports (1024-49151)
    /// Dynamic and/or Private Ports (49152-65535)
    /// 
    /// 참고 iana  http://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.xml
    /// 참고 위키피디아 http://en.wikipedia.org/wiki/List_of_TCP_and_UDP_port_numbers 
    /// </summary>
    public enum EnumReservedPort
    {
        HTTP = 80,

        AutoBasePlcScanMemoryServer0 = 6000,        // Autobase PLC_SCAN 메모리 서버 Port0                  IANA 예약포트: X11—used between an X client and server over the network
        AutoBasePlcScanMemoryServer255 = 6255,      // Autobase PLC_SCAN 메모리 서버 Port255                IANA 예약포트:

        AutoBasePlcScanComputerDual0 = 6700,        // Autobase PLC_SCAN 컴퓨터 이중화 Port0                IANA 예약포트:
        AutoBasePlcScanComputerDual255 = 6955,      // Autobase PLC_SCAN 컴퓨터 이중화 Port255              IANA 예약포트: 

        AutoBaseNetworkServerClientUDP = 7000,      // Autobase 네트워크 서버/클라이언트 UDP                IANA 예약포트: Default for Vuze's built in HTTPS Bittorrent Tracker 와 Avira Server Management Console
        AutoBaseNetworkServerClientTCP = 7001,      // Autobase 네트워크 서버/클라이언트 TCP 9.0 부터 예약  IANA 예약포트: Avira Server Management Console

        AutoBaseDataSync = 7050,                    // Autobase DataSync 10.2 부터 예약                     IANA 예약포트:

        AutoBaseSmsServerClient = 7100,             // Autobase SMS 서버/클라이언트                         IANA 예약포트:
        AutoBaseSmsWebService = 7110,               // Autobase SMS Web Service                             IANA 예약포트:

        AutoBaseShareServerTCP = 7200,              // Autobase Share Server TCP                            IANA 예약포트: FODMS FLIP
        AutoBaseShareServerUDP = 7201,              // Autobase Share Server TCP                            IANA 예약포트: DLIP

        AutoBaseProgramLocalMain = 7210,            // Autobase Program LocalMain                           IANA 예약포트: not assigned
        AutoBaseProgramStudio = 7212,               // Autobase Program LocalMain                           IANA 예약포트: not assigned

        AutoBaseExportServer = 7300,                // Autobase Export Server  7300~                        IANA 예약포트: The Swiss Exchange  7300~7359
        AutoBaseExportServer255 = 7555,             // Autobase Export Server  7300~                        IANA 예약포트: The Swiss Exchange  7300~7359
    }
}
