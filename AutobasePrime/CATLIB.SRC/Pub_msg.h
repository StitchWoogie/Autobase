// TAG size O.K
#if	!defined (__COMPILER_HPP)
#include <compiler.hpp>
#endif

#pragma pack(push, 1)
// AutoBase Programm끼리 서로 통신을 하기위해 사용하는 ID
// ID	800~ ID 899 까지는 모든 프로그램에서 공통으로 사용하는 ID로 정한다.

#define	IDM_PUBLIC_EVENT_MESSAGE		800		// lParam-message
#define	IDM_PUBLIC_EVENT_ALARM			801		// lParam=ALARM string
#define	IDM_PUBLIC_EVENT_AI				802
#define	IDM_PUBLIC_EVENT_AO				803
#define	IDM_PUBLIC_EVENT_DI				804
#define	IDM_PUBLIC_EVENT_DO				805
#define	IDM_PUBLIC_EVENT_AI_fSumTotal	806
#define	IDM_PUBLIC_EVENT_ST				807

#define	IDM_PUBLIC_NAVIGATOR_CHANGE_POS_SIZE		808		// navigator에서 사용자가 영역을 설정했을 때
												// LPARAM = RECT*
#define	IDM_PUBLIC_NAVIGATOR_NEED_YOUR_SIZE		809		// navigator에서 사용자가 영역을 설정했을 때
												// LPARAM = RECT*

// 분, 시간, 날, 주, 달, 년이 바뀌었을 때 발생하는 메세지
#define	IDM_PUBLIC_EVENT_CHANGE_MIN	810
#define	IDM_PUBLIC_EVENT_CHANGE_HOUR	811
#define	IDM_PUBLIC_EVENT_CHANGE_DAY	812
#define	IDM_PUBLIC_EVENT_CHANGE_WEEK	813
#define	IDM_PUBLIC_EVENT_CHANGE_MONTH	814
#define	IDM_PUBLIC_EVENT_CHANGE_YEAR	815
#define IDM_PUBLIC_EVENT_NEED_RELOAD_DATA	816		// 데이터를 다시 읽어야 할 필요가 있다.

#define IDM_PUBLIC_EVENT_CONTROL_KEY_RETURN		820		

//#define	IDM_PUBLIC_REMOTE_GET_FILE_DATA	 		823
//#define	IDM_PUBLIC_REMOTE_GET_FILE_SYSTEM 		824

//#define	IDM_PUBLIC_REMOTE_WANT_ALL_TAG_INITIAL	830	// Tag의 초기치를 세팅할 값을 보내줄것을 부탁한다.
																		// LOWORD(lParam)=terminal
//#define	IDM_PUBLIC_REMOTE_CONNECT		835	// 하나가 접속 되었다. lParam REMO=,NAME=
//#define	IDM_PUBLIC_REMOTE_DISCONNECT	836	// 하나가 빠져 나갔다. lParam REMO=,NAME=
//#define	IDM_PUBLIC_REGISTERED_VIEWMAIN_HWND	837	// main handle이 등록되었다. 9.0부터 필요없어짐

#define	IDM_PUBLIC_FILE_FIND_FIRST		840   // lParam - find first exchage
#define	IDM_PUBLIC_FILE_FIND_NEXT		841

#define	IDM_PUBLIC_DESTROY_WINDOW		845			// lParam = 12345678   이상한 프로그램에서 종료할 수 없도록 특수한 코드를 부여
#define IDM_PUBLIC_MDI_SET_VIEW_RATIO		846		// lParam = ratio
#define IDM_PUBLIC_MDI_SET_SCROLL_POS		847		// lParam = direction
#define	IDM_PUBLIC_ACTIVE_DOCUMENT		848			// 예) 감시 프로그램에서 스튜디오에 해당 문서를 열어주라는 의미
#define IDM_PUBLIC_PLC_SCAN_PORT_RESTART  849       // 해당 포트를 재시작한다.      lParam = 5000+portnum;

#define IDM_PUBLIC_PLC_SCAN_SCAN_SERVER_RESTART 850			// 해당 메모리 서버를 재시작한다.   lParam = 5000+portnum;

#define IDM_PUBLIC_PLC_SCAN_PORT_ACTIVE 851			// 해당 포트를 활성/비활성한다. lParam = portnum*10000+active;

/*
#define	IDM_PUBLIC_GET_TAG_MEMBER_AI	850	// lParam - struct
#define	IDM_PUBLIC_GET_TAG_MEMBER_AO	851	// lParam - struct
#define	IDM_PUBLIC_GET_TAG_MEMBER_DI	852	// lParam - struct
#define	IDM_PUBLIC_GET_TAG_MEMBER_DO	853	// lParam - struct

#define	IDM_PUBLIC_SET_TAG_MEMBER_AI	854	// lParam - struct
#define	IDM_PUBLIC_SET_TAG_MEMBER_AO	855	// lParam - struct
#define	IDM_PUBLIC_SET_TAG_MEMBER_DI	856	// lParam - struct
#define	IDM_PUBLIC_SET_TAG_MEMBER_DO	857	// lParam - struct

#define	IDM_PUBLIC_SET_DO_ON_OFF		858	// 디지탈 출력을 ON/OFF 한다.
															// main 에서 connect에게 send message
#define	IDM_PUBLIC_GET_TAG_COUNT_AI	859
#define	IDM_PUBLIC_GET_TAG_COUNT_AO	860
#define	IDM_PUBLIC_GET_TAG_COUNT_DI	861
#define	IDM_PUBLIC_GET_TAG_COUNT_DO	862

#define	IDM_PUBLIC_SET_AO_VALUE			863	// 아날로그 출력값을 바꿔준다.
															// main 에서 connect에게 send message
#define	IDM_PUBLIC_GET_AI_TAG_POS		864
#define	IDM_PUBLIC_GET_DI_TAG_POS		865

#define	IDM_PUBLIC_SET_DI_ON_OFF		866	// 디지탈 출력을 ON/OFF 한다.
#define	IDM_PUBLIC_SET_AI_VALUE			867	// 디지탈 출력을 ON/OFF 한다.*/

//#define	IDM_PUBLIC_GET_TREND_AI_DATA	868	// 아날로그 TREND를 가져온다.(lParam-exchange)
//#define	IDM_PUBLIC_GET_TREND_DI_DATA	869	// 디지탈   TREND를 가져온다.(lParam-exchange)

// 아래는 ViewMain과 PLC_SCAN 프로그램이 서로정보를 교환하기 위해서 존재하는 ID

/* 9.0에서는 더이상 필요하지 않다.
#define	IDM_PUBLIC_SCAN_GET_PORT		872
#define	IDM_PUBLIC_SCAN_REGISTER_HWND 874	// plc scan 프로그램의 핸들을 main에 등록한다.
#define	IDM_PUBLIC_SCAN_GET_COMM_TRY 		875
#define	IDM_PUBLIC_SCAN_GET_COMM_TIMEOUT 876
#define	IDM_PUBLIC_SCAN_GET_COMM_CODEBAD 877
#define	IDM_PUBLIC_SCAN_RESET_COMM_COUNT 878
*/

//#define	IDM_PUBLIC_SCAN_GET_WORD_START		880	// 32비트로 프로그램이 바뀌면서 추가 start 중에는 plc_scan timer가 동작하지 않도록 한다.
//#define	IDM_PUBLIC_SCAN_GET_WORD_END		881	// 

#define	IDM_PUBLIC_LINE_PRINTER_REGISTER_HWND	883	// line printer hwnd register command
#define	IDM_PUBLIC_LINE_PRINTER_MSG				884	// line printer msg command from 32 bit

#define	IDM_PUBLIC_BEEPER_MANAGER_REGISTER_HWND	885	// beep manager hwnd register command
#define	IDM_PUBLIC_BEEPER_MANAGER_MSG_16		886	// beep manager msg command from 16 bit
#define	IDM_PUBLIC_BEEPER_MANAGER_MSG_32		887	// beep manager msg command from 32 bit

#define	IDM_PUBLIC_SMS_MANAGER_REGISTER_HWND	888	// line printer hwnd register command
#define	IDM_PUBLIC_SMS_MANAGER_MSG				889	// line printer msg command from 32 bit

//#define IDM_PUBLIC_REPORT_SET_PRINT_TIME		890	// report start time


enum {
	FIND_FILE_TYPE_DATA,    
	FIND_FILE_TYPE_SYSTEM,
};

/*
// 이 스트럭쳐가 lParam
typedef struct {
	WORD 	struct_size;
	WORD	terminal;
	WORD	attrib;				// FA_ARCH | FA_DIREC
	char	find_file_type;		// FIND_FIND_???
	char	path[MAXPATH];		// seek file path	( if alarm - alarm\19960101.arm)
	struct	ffblk blk;			// if success ffblk filled
} EXCHANGE_FIND_FIRST_NEXT_STRUCT;
*/

#pragma pack(pop)





