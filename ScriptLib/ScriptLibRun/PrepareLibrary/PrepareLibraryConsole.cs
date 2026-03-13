using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ScriptLibRun
{
    class PrepareLibraryConsole
    {
        public static void AddClassConsole(ScriptLibNamespace sln)
        {
            ScriptLibClass slc = new ScriptLibClass(sln);
            slc.sNameClass = "Console";

            PrepareLibrary.AddMethod(slc, "WriteLine", new ScriptLibMemberMethod.DeleMethod(WriteLine), "in:string:format", "params:object[]:args");

            sln.arrayMember.Add(slc);
        }

        // 주의: Console.WriteLine은 스튜디오 디버그 상에서는 잘 출력되지 않는다.
        static bool WriteLine(ClassDataStack cds, out object retn, object[] args)
        {
            ShowConsoleWindow();

            object[] args2 = new object[args.Length - 1];
            // params는 args[1] 부터 시작한다.
            for (int i = 0; i < args.Length - 1; i++)
            {
                args2[i] = args[i + 1];
            }

            try
            {
                // 주의: Console.WriteLine은 스튜디오 디버그 상에서는 잘 출력되지 않는다.
                if (args.Length == 0)
                    Console.WriteLine();
                if (args.Length == 1)
                    Console.WriteLine(args[0]); // 꼭 문자열이 아니더라도 Int같은 것도 바로 출력할 수 있다.
                else
                    Console.WriteLine((string)args[0], args2);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception:{0}", exception.Message);
            }

            retn = 0;
            return true;
        }

        static void ShowConsoleWindow()
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
