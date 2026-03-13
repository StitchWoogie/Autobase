#include <kwl\kapplica.h>

KApplication :: KApplication()
{

}

KApplication :: ~KApplication()
{

}

int KApplication :: run()
{
	return 0;
}

int PASCAL WinMain (HANDLE hInstance, HANDLE hPrevInstance,
						  LPSTR lpszCmdLine, int /*nCmdShow*/)
{
/*	
	HANDLE   hAccel ;
	MSG      msg ;
	WNDCLASS wndclass ;
	char		title[80];

	// 프로그램이 이미 실행 중이면 실행중인 프로그램으로 표시해 준다.
	if(hPrevInstance)	{
		hwndMainFrame = FindWindow(szFrameClass, NULL);

		SetActiveWindow(hwndMainFrame);
		if(IsIconic(hwndMainFrame)) {
			ShowWindow(hwndMainFrame, SW_RESTORE);
		}
		return 0;
	}

	GetProgrammDirectory(hInstance, sDirProgramm, sizeof(sDirProgramm));

	hInst = hInstance ;

	if (!hPrevInstance) {
		// Register the frame window class

		wndclass.style         = CS_HREDRAW | CS_VREDRAW ;
		wndclass.lpfnWndProc   = FrameWndProc ;
		wndclass.cbClsExtra    = 0;
		wndclass.cbWndExtra    = 0;
		wndclass.hInstance     = hInstance ;
		wndclass.hIcon         = LoadIcon (hInstance, MAKEINTRESOURCE(IDI_MAIN_FRAME)) ;
		wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW) ;
		wndclass.hbrBackground = (HBRUSH)(COLOR_APPWORKSPACE + 1);
		wndclass.lpszMenuName  = NULL ;
		wndclass.lpszClassName = szFrameClass;

		RegisterClass (&wndclass) ;

		// Register the Hello child window class

		wndclass.style         = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS;
		wndclass.lpfnWndProc   = WorkWndProc ;
		wndclass.cbClsExtra    = 0 ;
		wndclass.cbWndExtra    = sizeof (LOCALHANDLE);
		wndclass.hInstance     = hInstance ;
		wndclass.hIcon         = LoadIcon (NULL, IDI_APPLICATION) ;
		wndclass.hCursor       = LoadCursor (NULL, IDC_ARROW) ;
		wndclass.hbrBackground = GetStockObject(NULL_BRUSH);
		wndclass.lpszMenuName  = NULL ;
		wndclass.lpszClassName = szHelloClass;

		RegisterClass (&wndclass) ;


		
	}

	// Obtain handles to three possible menus & submenus

	hMenuInit  = LoadMenu (hInst, MAKEINTRESOURCE(IDR_MENU_INIT));
	hMenuWork =  LoadMenu (hInst, MAKEINTRESOURCE(IDR_MENU_WORK));

	hMenuInitWindow  = GetSubMenu (hMenuInit,   INIT_MENU_POS);
	hMenuWorkWindow  = GetSubMenu (hMenuWork,   WORK_MENU_POS);

	// Load accelerator table

	hAccel = LoadAccelerators (hInst, "MdiAccel") ;

	// Create the frame window

	sprintf(title, "AutoBase Report 편집기 %s.%s for Windows95", "1", "0");

	hwndMainFrame = CreateWindow (szFrameClass, title,
									 WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN,
									 CW_USEDEFAULT, CW_USEDEFAULT,
									 CW_USEDEFAULT, CW_USEDEFAULT,
									 NULL, hMenuInit, hInstance, NULL) ;

	hwndMainClient = GetWindow (hwndMainFrame, GW_CHILD);

	ShowWindow (hwndMainFrame, SW_SHOWMAXIMIZED);
	UpdateWindow (hwndMainFrame);

	LoadConfig();
	
	SendMessage(hwndMainFrame, WM_COMMAND, IDM_NEW, 0L);

	// Enter the modified message loop
	while (GetMessage (&msg, NULL, 0, 0)) {
		if(!TranslateMDISysAccel (hwndMainClient, &msg) &&
			!TranslateAccelerator (hwndMainFrame, hAccel, &msg)) {
			TranslateMessage (&msg) ;
			DispatchMessage (&msg) ;
		}
	}
	

	// Clean up by deleting unattached menus

	DestroyMenu (hMenuWork) ;

	SaveConfig();

	return msg.wParam ;
	*/
	return 0;
}


