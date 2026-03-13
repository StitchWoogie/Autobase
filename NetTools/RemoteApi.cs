using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace NetTools
{
    public class RemoteApi
    {
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeCloseHandle(IntPtr hObject);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeCopyFile(string lpExistingFileName, string lpNewFileName, int bFailIfExists);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeCreateDirectory(string lpPathName, uint lpSecurityAttributes);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CeCreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, int lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, int hTemplateFile);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeCreateProcess(string pszImageName, string pszCmdLine, IntPtr psaProcess, IntPtr psaThread, int fInheritHandles, int fdwCreate, IntPtr pvEnvironment, IntPtr pszCurDir, IntPtr psiStartInfo, out PROCESS_INFORMATION pi);

        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeDeleteFile(string lpFileName);
                
        internal struct CE_FIND_DATA
        {
            public int dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwOID;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]  // 이전에는 이코드를 사용했다. IEC1000모델에서 사용할 때 스튜디오가 다운되는 현상 발생. 260*2 로 수정하니 IEC1000에서 잘됨  2013-6-14 
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]    // 이코드는 인터넷 예제에서 찾을 수 있는데 문자열이 첫글자만 들어와서 배열로 읽어서 문자열로 바꾸는 방법을 사용. 2013-6-14 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260*2)]  
            public byte[] cFileName;
        } 
        

        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeFindClose(IntPtr hFindFile);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CeFindFirstFile(string lpFileName, ref CE_FIND_DATA lpFindFileData);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeFindNextFile(IntPtr hFindFile, ref CE_FIND_DATA lpFindFileData);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeGetDesktopDeviceCaps(int nIndex);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint CeGetFileAttributes(string lpFileName);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern uint CeGetFileSize(IntPtr hFile, ref uint lpFileSizeHigh);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeGetFileTime(IntPtr hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeGetLastError();
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeGetSpecialFolderPath(int nFolder, uint nBufferLength, StringBuilder lpBuffer);
        //[DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern int CeGetStoreInformation(out STORE_INFORMATION lpsi);
        //[DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern int CeGetSystemInfo(out SYSTEM_INFO pSI);
        //[DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern bool CeGetSystemPowerStatusEx(out SYSTEM_POWER_STATUS_EX pStatus, bool fUpdate);
        //[DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern bool CeGetVersionEx(out OSVERSIONINFO lpVersionInformation);
        //[DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //internal static extern void CeGlobalMemoryStatus(out MEMORYSTATUS msce);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeMoveFile(string lpExistingFileName, string lpNewFileName);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeRapiGetError();
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeRapiInit();

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAPIINIT
        {
            public int cbsize;
            public IntPtr heRapiInit;
            public UInt32 hrRapiInit;
        };

        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeRapiInitEx([MarshalAs(UnmanagedType.Struct)] ref RAPIINIT pRapiInit);

        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeRapiInvoke(string pDllPath, string pFunctionName, uint cbInput, IntPtr pInput, out uint pcbOutput, out IntPtr ppOutput, IntPtr ppIRAPIStream, uint dwReserved);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeRapiUninit();
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfbytesToRead, ref int lpNumberOfbytesRead, int lpOverlapped);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeRemoveDirectory(string lpPathName);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeSetEndOfFile(int hFile);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeSetFileAttributes(string lpFileName, uint dwFileAttributes);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeSetFileTime(IntPtr hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeSHCreateShortcut(string pShortcutName, string pTarget);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int CeSHGetShortcutTarget(string lpszShortcut, string lpszTarget, int cbMax);
        [DllImport("rapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int CeWriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfbytesToWrite, ref int lpNumberOfbytesWritten, int lpOverlapped);

        public bool Init(bool err_msg)
        {
            RAPIINIT r = new RAPIINIT();
            r.cbsize = Marshal.SizeOf(r);
            int retn;

            try
            {
                retn = CeRapiInitEx(ref r); // return은 항상 0이다.
            }
            catch (Exception exception)
            {
                if (err_msg)
                {
                    MessageBox.Show(exception.Message, "Remote Init Error");
                }
                return false;
            }

            if (r.hrRapiInit == 0) return true;

            if (err_msg)
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("CE 장치가 연결되지 않았거나 오류가 있습니다.", "연결 오류");
                else
                    MessageBox.Show("CE Device not connedted.", "CE Device error");
            }

            CeRapiUninit();     // 실패하더라도 Unit을 하지 않으면 다음에 들어올 때 이상하다.
            return false;
            /*  
            int result = CeRapiInit();  // 연결이 안된경우 무한 루프가 발생한다.

            if (result == 0) return true;

            return false;*/
        }

        public void Uninit()
        {
            CeRapiUninit();
        }

        const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        const short INVALID_HANDLE_VALUE = -1;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint CREATE_NEW = 1;
        const uint CREATE_ALWAYS = 2;
        const uint OPEN_EXISTING = 3; 

        /*
        /// <summary>
        /// 컴퓨터의 파일을 CE로 복사해 준다.
        /// 파일이 한번에 복사하니 잘 안되는 경우가 있어서 밑에 있는 CopyFileToDevice() 함수를 사용하니 잘 됨
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CopyFile(string source, string target)
        {
            //int result = CeCopyFile(source, "\\hello.txt", 0); CeCopyFile은 디바이스간 복사만 되는듯 하다.
            byte[] buffer = System.IO.File.ReadAllBytes(source);
            int written = 0;

            //long tc = File.GetCreationTime(source).ToFileTime();
            //long tw = File.GetLastWriteTime(source).ToFileTime();
            //long ta = File.GetLastAccessTime(source).ToFileTime();
            //long empty = 0;

            IntPtr ptr = CeCreateFile(target, GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);

            if (ptr.ToInt32() == -1)
            {
                return false;
            }

            CeWriteFile(ptr, buffer, buffer.Length, ref written, 0);

            //CeSetFileTime(ptr, ref tc, ref ta, ref tw); // 여기서 하면 잘안된다.
            CeCloseHandle(ptr);

            SetFileTime(target, File.GetCreationTime(source), File.GetLastAccessTime(source), File.GetLastWriteTime(source));

            return true;
        }*/

        public bool CopyFileToDevice(string source, string target)
        {
            bool ReturnValue = false;

            if (!File.Exists(source))
                return false;

            BinaryReader DTFile = new BinaryReader(File.Open(source, FileMode.Open));

            System.IntPtr RemoteFile = CeCreateFile(target, GENERIC_READ | GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);

            if (RemoteFile.ToInt32() == -1)
            {
                DTFile.Close();
                return false;
            }

            try
            {
                byte[] Buffer = new byte[4096];

                bool Done = false;

                int BytesWritten = 0;

                int BytesRead;

                while (!Done)
                {
                    BytesRead = DTFile.Read(Buffer, 0, Buffer.Length);

                    if (BytesRead == 0)
                    {
                        Done = true;
                    }
                    else
                    {
                        CeWriteFile(RemoteFile, Buffer, BytesRead, ref BytesWritten, 0);
                    }
                }

                ReturnValue = true;
            }

            finally
            {
                CeCloseHandle(RemoteFile);

                DTFile.Close();
            }

            SetFileTime(target, File.GetCreationTime(source), File.GetLastAccessTime(source), File.GetLastWriteTime(source));

            return ReturnValue;
        }

        /// <summary>
        /// 파일을 만든 다음 닫고 다시 열어서 적용해야 날짜가 잘 만들어 진다.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tc"></param>
        /// <param name="ta"></param>
        /// <param name="tw"></param>
        /// <returns></returns>
        public bool SetFileTime(string filename, DateTime tc, DateTime ta, DateTime tw)
        {
            long lc = tc.ToFileTime();
            long lw = tw.ToFileTime();
            long la = ta.ToFileTime();

            IntPtr ptr = CeCreateFile(filename, GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0);
            if (ptr.ToInt32() == -1)
            {
                return false;
            }
            CeSetFileTime(ptr, ref lc, ref la, ref lw);
            CeCloseHandle(ptr);

            return true;
        }

        /// <summary>
        /// CE에 새 폴더를 만든다.
        /// </summary>
        /// <param name="directory"></param>
        public void CreateDirectory(string directory)
        {
            CeCreateDirectory(directory, 0);
        }

        /// <summary>
        /// CE 디바이스의 실행 파일을 실행해 준다.
        /// </summary>
        /// <param name="programname"></param>
        /// <param name="cmdline"></param>
        /// <returns></returns>
        public PROCESS_INFORMATION CreateProcess(string programname, string cmdline)
        {
            PROCESS_INFORMATION Information = new PROCESS_INFORMATION();

            CeCreateProcess(programname, cmdline, IntPtr.Zero, IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out Information);

            return Information;
        }

        /*
        public void CloseHandle(IntPtr handle)
        {
            CeCloseHandle(handle);
        }*/

        void RecurseRemoveDirectory(string dir)
        {
            CE_FIND_DATA xFileInfo = new CE_FIND_DATA();
            IntPtr xHandle = CeFindFirstFile(dir+"\\*.*", ref xFileInfo);

            const int FILE_ATTRIBUTE_DIRECTORY = 0x0010;

            if(xHandle.ToInt32() != -1) {
                while (true)
                {
                    string xFileName = Encoding.Unicode.GetString(xFileInfo.cFileName);

                    int index = xFileName.IndexOf('\0');

                    if (index != -1)
                    {
                        xFileName = xFileName.Substring(0, index);
                    }

                    if ((xFileInfo.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) > 0)
                    {
                        RecurseRemoveDirectory(dir + "\\" + xFileName);
                    }
                    else
                    {
                        CeDeleteFile(dir+"\\"+xFileName);
                    }

                    if(CeFindNextFile(xHandle, ref xFileInfo) == 0)
                        break;
                }
            }

            CeFindClose(xHandle);

            CeRemoveDirectory(dir);
        }
               
        /// <summary>
        /// CE 폴더안에있는 모든 파일과 서버 폴더를 삭제한다.
        /// </summary>
        /// <param name="dir"></param>
        public void RemoveDirectory(string dir)
        {
            RecurseRemoveDirectory(dir);
        }

        /// <summary>
        /// CE 디바이스에서 파일을 읽어서 배열로 만든다.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public byte[] ReadAllBytes(string filename)
        {
            IntPtr remote_file_ptr = CeCreateFile(filename, GENERIC_READ, 0,   0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);

            if (remote_file_ptr.ToInt32() == INVALID_HANDLE_VALUE)
            {
                return null;
            }

            uint file_size_high = 0;
            uint file_size = CeGetFileSize(remote_file_ptr, ref file_size_high);
            byte[] data = new byte[file_size];
            int read = 0;

            CeReadFile(remote_file_ptr, data, (int)file_size, ref read, 0);
            CeCloseHandle(remote_file_ptr);

            return data;
        }

        void RecurseGetFilesList(List<FileInfoCE> array, string dir)
        {
            CE_FIND_DATA xFileInfo = new CE_FIND_DATA();
            IntPtr xHandle = CeFindFirstFile(dir + "\\*.*", ref xFileInfo);

            const int FILE_ATTRIBUTE_DIRECTORY = 0x0010;

            if (xHandle.ToInt32() != -1)
            {
                while (true)
                {
                    string xFileName = Encoding.Unicode.GetString(xFileInfo.cFileName);

                    int index = xFileName.IndexOf('\0');

                    if (index != -1)
                    {
                        xFileName = xFileName.Substring(0, index);
                    }

                    if ((xFileInfo.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) > 0)
                    {
                        RecurseGetFilesList(array, dir + "\\" + xFileName);
                    }
                    else
                    {
                        FileInfoCE fi = new FileInfoCE();
                        fi.Name = xFileName;
                        fi.FullName = dir + "\\" + xFileName;
                        array.Add(fi);
                    }

                    if (CeFindNextFile(xHandle, ref xFileInfo) == 0)
                        break;
                }
            }

            CeFindClose(xHandle);
        }

        /// <summary>
        /// 폴더안에 있는 모든 파일의 리스트를 가져온다.
        /// </summary>
        /// <param name="dir"></param>
        public List<FileInfoCE> GetFilesList(string dir)
        {
            List<FileInfoCE> array = new List<FileInfoCE>();

            RecurseGetFilesList(array, dir);

            return array;
        }

        public bool DirectoryExists(string directory)
        {
            uint attr = CeGetFileAttributes(directory);

            if ((int)attr == -1)
                return false;

            return true;
        }
    }

    public class FileInfoCE
    {
        public string Name;
        public string FullName;
    }
}

/*
1.          [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
2.        internal static extern int CeCloseHandle(IntPtr hObject);   
3.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
4.        internal static extern int CeCopyFile(string lpExistingFileName, string lpNewFileName, int bFailIfExists);   
5.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
6.        internal static extern int CeCreateDirectory(string lpPathName, uint lpSecurityAttributes);   
7.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
8.        internal static extern IntPtr CeCreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);   
9.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
10.        internal static extern int CeCreateProcess(string pszImageName, IntPtr pszCmdLine, IntPtr psaProcess, IntPtr psaThread, int fInheritHandles, int fdwCreate, IntPtr pvEnvironment, IntPtr pszCurDir, IntPtr psiStartInfo, out PROCESS_INFORMATION pi);   
11.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
12.        internal static extern int CeCreateProcess(string pszImageName, string pszCmdLine, IntPtr psaProcess, IntPtr psaThread, int fInheritHandles, int fdwCreate, IntPtr pvEnvironment, IntPtr pszCurDir, IntPtr psiStartInfo, out PROCESS_INFORMATION pi);   
13.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
14.        internal static extern int CeDeleteFile(string lpFileName);   
15.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
16.        internal static extern int CeFindClose(IntPtr hFindFile);   
17.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
18.        internal static extern IntPtr CeFindFirstFile(string lpFileName, byte[] lpFindFileData);   
19.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
20.        internal static extern int CeFindNextFile(IntPtr hFindFile, byte[] lpFindFileData);   
21.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
22.        internal static extern int CeGetDesktopDeviceCaps(int nIndex);   
23.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
24.        internal static extern uint CeGetFileAttributes(string lpFileName);   
25.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
26.        internal static extern uint CeGetFileSize(IntPtr hFile, ref uint lpFileSizeHigh);   
27.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
28.        internal static extern int CeGetFileTime(IntPtr hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);   
29.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
30.        internal static extern int CeGetLastError();   
31.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
32.        internal static extern int CeGetSpecialFolderPath(int nFolder, uint nBufferLength, StringBuilder lpBuffer);   
33.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
34.        internal static extern int CeGetStoreInformation(out STORE_INFORMATION lpsi);   
35.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
36.        internal static extern int CeGetSystemInfo(out SYSTEM_INFO pSI);   
37.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
38.        internal static extern bool CeGetSystemPowerStatusEx(out SYSTEM_POWER_STATUS_EX pStatus, bool fUpdate);   
39.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
40.        internal static extern bool CeGetVersionEx(out OSVERSIONINFO lpVersionInformation);   
41.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
42.        internal static extern void CeGlobalMemoryStatus(out MEMORYSTATUS msce);   
43.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
44.        internal static extern int CeMoveFile(string lpExistingFileName, string lpNewFileName);   
45.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
46.        internal static extern int CeRapiGetError();   
47.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
48.        internal static extern int CeRapiInit();   
49.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
50.        internal static extern int CeRapiInitEx([MarshalAs(UnmanagedType.Struct)] ref RAPIINIT pRapiInit);   
51.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
52.        internal static extern int CeRapiInvoke(string pDllPath, string pFunctionName, uint cbInput, IntPtr pInput, out uint pcbOutput, out IntPtr ppOutput, IntPtr ppIRAPIStream, uint dwReserved);   
53.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
54.        internal static extern int CeRapiUninit();   
55.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
56.        internal static extern int CeReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfbytesToRead, ref int lpNumberOfbytesRead, int lpOverlapped);   
57.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
58.        internal static extern int CeRemoveDirectory(string lpPathName);   
59.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
60.        internal static extern int CeSetEndOfFile(int hFile);   
61.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
62.        internal static extern int CeSetFileAttributes(string lpFileName, uint dwFileAttributes);   
63.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
64.        internal static extern int CeSetFileTime(IntPtr hFile, ref long lpCreationTime, ref long lpLastAccessTime, ref long lpLastWriteTime);   
65.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
66.        internal static extern int CeSHCreateShortcut(string pShortcutName, string pTarget);   
67.        [DllImport("rapi.dll", CharSet=CharSet.Unicode, SetLastError=true)]   
68.        internal static extern int CeSHGetShortcutTarget(string lpszShortcut, string lpszTarget, int cbMax);   
69.        [DllImport("rapi.dll", CharSet=CharSet.Unicode)]   
70.        internal static extern int CeWriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfbytesToWrite, ref int lpNumberOfbytesWritten, int lpOverlapped);  
*/