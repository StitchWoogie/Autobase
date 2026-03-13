using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Windows Job Object wrapper.
    /// 부모 프로세스 종료 시 자식 프로세스를 자동으로 함께 종료시킵니다.
    /// 또한 JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE 플래그로 인해
    /// 디버거 중지, Task Manager Kill 등 비정상 종료 시에도 자식이 정리됩니다.
    /// </summary>
    internal sealed class ChildProcessJob : IDisposable
    {
        private IntPtr _jobHandle;
        private bool _disposed;

        public ChildProcessJob()
        {
            _jobHandle = CreateJobObject(IntPtr.Zero, null);
            if (_jobHandle == IntPtr.Zero)
                return;

            var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
            info.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;

            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr infoPtr = Marshal.AllocHGlobal(length);
            try
            {
                Marshal.StructureToPtr(info, infoPtr, false);
                SetInformationJobObject(_jobHandle,
                    JobObjectInfoType.ExtendedLimitInformation,
                    infoPtr, (uint)length);
            }
            finally
            {
                Marshal.FreeHGlobal(infoPtr);
            }
        }

        /// <summary>
        /// 프로세스를 Job Object에 등록합니다.
        /// Job이 닫힐 때(부모 종료 시) 해당 프로세스도 자동 종료됩니다.
        /// </summary>
        public bool AssignProcess(Process process)
        {
            if (_jobHandle == IntPtr.Zero || process == null)
                return false;

            try
            {
                return AssignProcessToJobObject(_jobHandle, process.Handle);
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_jobHandle != IntPtr.Zero)
            {
                CloseHandle(_jobHandle);
                _jobHandle = IntPtr.Zero;
            }
        }

        // ── Win32 P/Invoke ──

        private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(
            IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private enum JobObjectInfoType
        {
            ExtendedLimitInformation = 9
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public uint LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public long Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }
    }
}
