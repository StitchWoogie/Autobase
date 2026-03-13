using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetTools
{
    public class ConsoleTool
    {
        // Form프로그램에서 이 함수를 호출한 다음 Console.WriteLine을 하면 메시지를 표시할 수 있다.
        // 비쥬얼 스튜디오 디버그 상에서는 Output화면에 나타나지만 단독 실행 상태에서는 콘솔 화면이 나타난다.
        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void DebugMessage(string format, params object[] args)
        {
            ShowConsoleWindow();

            Console.Write("{0} ", DateTime.Now);
            Console.WriteLine(format, args);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
    }
}
