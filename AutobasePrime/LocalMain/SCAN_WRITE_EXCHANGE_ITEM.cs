using System;
using System.Runtime.InteropServices;

namespace LocalMain
{
	/// <summary>
	/// Summary description for SCAN_WRITE_EXCHANGE_ITEM.
	/// </summary>
	[ StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi, Pack=1)]
	public class SCAN_WRITE_EXCHANGE_ITEM
	{
		public sbyte	command;//char	command;	// 0 = DO, 1 = AO, 100 = Read scan delay,
		[ MarshalAs( UnmanagedType.ByValTStr, SizeConst=40 )]
		public String   tag;//char    tag[40];
		public short	port;
		public short	station;
		public UInt32   address;//DWORD	address;
		[ MarshalAs( UnmanagedType.ByValTStr, SizeConst=40 )]
		public String	sExtraAddr;//char	sExtraAddr[40];
		public ushort   wExtraAddr;//WORD	wExtraAddr;
		public double	val;        // 어레이 출력일 때는 byte의 크기로 사용한다.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=512)]
        public byte[] array;
        public byte array_type;     // 출력할 어레이의 종류

		public sbyte	bVipScanFlag;//char	bVipScanFlag;			// MAIN에서 수동으로 운전했을 때
		public ushort   wVipScanMemoryPort;
		public sbyte	cVipScanMemoryType;		// 수동으로 운전했을 때 관련 입력의 메모리 종류
		public int	    nVipScanMemoryPos;		// 수동으로 운전했을 때 관련 입력의 메모리 위치

		/*
			char	command;	// 0 = DO, 1 = AO, 100 = Read scan delay,
			char    tag[40];
			short	port;
			short	station;
			DWORD	address;
			char	sExtraAddr[40];
			WORD	wExtraAddr;
			double	value;

			char	bVipScanFlag;			// MAIN에서 수동으로 운전했을 때
			WORD    wVipScanMemoryPort;
			char	cVipScanMemoryType;		// 수동으로 운전했을 때 관련 입력의 메모리 종류
			WORD	wVipScanMemoryPos;		// 수동으로 운전했을 때 관련 입력의 메모리 위치
			*/
	}

	/*
typedef struct tagOFN { 
  DWORD         lStructSize; 
  HWND          hwndOwner; 
  HINSTANCE     hInstance; 
  LPCTSTR       lpstrFilter; 
  LPTSTR        lpstrCustomFilter; 
  DWORD         nMaxCustFilter; 
  DWORD         nFilterIndex; 
  LPTSTR        lpstrFile; 
  DWORD         nMaxFile; 
  LPTSTR        lpstrFileTitle; 
  DWORD         nMaxFileTitle; 
  LPCTSTR       lpstrInitialDir; 
  LPCTSTR       lpstrTitle; 
  DWORD         Flags; 
  WORD          nFileOffset; 
  WORD          nFileExtension; 
  LPCTSTR       lpstrDefExt; 
  LPARAM        lCustData; 
  LPOFNHOOKPROC lpfnHook; 
  LPCTSTR       lpTemplateName; 
#if (_WIN32_WINNT >= 0x0500)
  void *        pvReserved;
  DWORD         dwReserved;
  DWORD         FlagsEx;
#endif // (_WIN32_WINNT >= 0x0500)
} OPENFILENAME, *LPOPENFILENAME; 

	[ StructLayout( LayoutKind.Sequential, CharSet=CharSet.Auto )]  
	public class OpenFileName 
	{
		public int		structSize = 0;
		public IntPtr	dlgOwner = IntPtr.Zero; 
		public IntPtr	instance = IntPtr.Zero;
    
		public String	filter = null;
		public String	customFilter = null;
		public int		maxCustFilter = 0;
		public int		filterIndex = 0;
    
		public String	file = null;
		public int		maxFile = 0;
    
		public String	fileTitle = null;
		public int		maxFileTitle = 0;
	
		public String	initialDir = null;
    
		public String	title = null;   
    
		public int		flags = 0; 
		public short	fileOffset = 0;
		public short	fileExtension = 0;
    
		public String	defExt = null; 
    
		public IntPtr	custData = IntPtr.Zero;  
		public IntPtr	hook = IntPtr.Zero;  
    
		public String	templateName = null; 
    
		public IntPtr	reservedPtr = IntPtr.Zero; 
		public int		reservedInt = 0;
		public int		flagsEx = 0;
	}
	*/
}
