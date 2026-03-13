using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoLibLocal
{
    public enum EnumIdmPublic
    {
        //#define	IDM_PUBLIC_FILE_FIND_FIRST		840     // lParam - find first exchage
        //#define	IDM_PUBLIC_FILE_FIND_NEXT		841
        IDM_PUBLIC_DESTROY_WINDOW =	845,
        //#define IDM_PUBLIC_MDI_SET_VIEW_RATIO		846		// lParam = ratio
        //#define IDM_PUBLIC_MDI_SET_SCROLL_POS		847		// lParam = direction
        IDM_PUBLIC_ACTIVE_DOCUMENT = 848,
        IDM_PUBLIC_PLC_SCAN_PORT_RESTART = 849,             // 해당 포트를 재시작한다.          lParam = 5000+portnum;
        IDM_PUBLIC_PLC_SCAN_SCAN_SERVER_RESTART = 850,       // 해당 메모리 서버를 재시작한다.   lParam = 5000+portnum;
        IDM_PUBLIC_PLC_SCAN_PORT_ACTIVE = 851,              // 해당 포트를 활성/비활성한다. lParam = portnum*10000+active;
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
    }
}
