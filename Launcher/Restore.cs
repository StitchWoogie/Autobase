using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;



namespace LauncherMain
{

    public class Restore
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_RESTORE = 9;

        public static void BringToFront(string processName)
        {
            var procs = Process.GetProcessesByName(processName);

            foreach (var p in procs)
            {
                IntPtr handle = p.MainWindowHandle;

                if (handle == IntPtr.Zero)
                {
                    // Restore if minimized
                    ShowWindow(handle, SW_RESTORE);

                    // Bring to front
                    SetForegroundWindow(handle);
                }
            }
        }
    }
}
