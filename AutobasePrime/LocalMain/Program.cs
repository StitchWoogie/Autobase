//#define USE_EXCEPTION_REPORT 

using System;
using System.Collections.Generic;

using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLibLocal;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LocalMain
{
    static class Program
    {
        // Debug모드인지를 검사한다.
        static void CheckDebugMode(string[] args)
        {
            if (!AutoLibLocal.NextVersion.bScript11) return;

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (String.Compare(args[i], 0, "Debug=", 0, 6, true) == 0)
                    {
                        int method = ConvertTool.ToInt32(args[i].Substring(6));

                        if (method > 0)
                        {
                            FormLocalMain.bDebugMode = true;
                            ScriptLibRun.ScriptLibMain.bDebugMode = true;

                            if (method == 2)    // step into or step over
                            {
                                ScriptLibRun.CommandPublic.bBreakMustNext = true;
                            }
                        }
                        return; 
                    }
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            //if (!AutoLibLocal.KeyLock.BetaTest.IsPrimeBetaTester()) return;

#if USE_EXCEPTION_REPORT

            try
            {
                //  1. UI 스레드(메인 스레드) 예외 핸들러 251028 PSU
                Application.ThreadException += OnThreadException;
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                //  2. 전역 예외 핸들러 등록 (백그라운드 스레드) 251028 PSU
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                // 3. Task 예외 (관찰되지 않은)  251028 PSU
                TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

                AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
                {
                    Debug.WriteLine("FirstChance: " + e.Exception);
                };

#endif
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);


                FormLocalMain.mainArgs = args;
                ScriptFunctionCommandLine.mainArgs = args;

                CheckDebugMode(args);

                LanguageTool.ChangeUICulture();

                IntPtr hwnd_viewmain = Win32Function.FindWindow("ViewMainProgrammMainFrame", null);

                if (hwnd_viewmain != IntPtr.Zero)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("구 버전의 AutoBase.exe 가 이미 실행중입니다.\nLocalMain.exe 는 구버전과 함께 사용될 수 없습니다.", "구버전 실행 중");
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("正在运行旧版本 AutoBase.exe.\n LocalMain.exe跟旧版本不能一起使用. ", "正在运行旧版本");
                    else
                        MessageBox.Show("Old version (AutoBase.exe) is already running.", "Old version Running");

                    return;
                }

                bool createdNew = false;
                Mutex gM1 = new Mutex(true, "AutoBaseLocalMainMutex", out createdNew);

                if (createdNew)
                {
                    //Application.EnableVisualStyles(); 
                    //Application.SetCompatibleTextRenderingDefault(false);
                    Application.AddMessageFilter(new MessageFilter());
                    Application.Run(new FormLocalMain());
                }
                else
                {
                    Process p = Tools.GetPreviousProcess();
                    if (p != null && p.MainWindowHandle != IntPtr.Zero)
                    {
                        Win32Function.SetForegroundWindow(p.MainWindowHandle);
                    }
                }

#if USE_EXCEPTION_REPORT
            }
            catch (Exception exception)
            {
                //SmLog.MessageWithException(LogLevel.FATAL, LogCategory.SYSTEM, exception, "type: {0}, error: {1}", exception.GetType().FullName, exception.Message);

                //ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);

                //dialog.ShowDialog();

                //Process.GetCurrentProcess().Kill();

                // 메인 스레드 초기화 중 예외
                HandleFatalException(exception);
            }
#endif
        }

        // 공통 예외 처리 메서드
        public static void HandleFatalException(Exception exception)
        {
            SmLog.MessageWithException(LogLevel.FATAL, LogCategory.SYSTEM, exception,
                "type: {0}, error: {1}", exception.GetType().FullName, exception.Message);

            ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);
            dialog.ShowDialog();

            Process.GetCurrentProcess().Kill();
        }


        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                HandleFatalException(exception);
            }
            else
            {
                // Exception이 아닌 다른 객체가 throw된 경우 (매우 드묾)
                SmLog.Message(LogLevel.FATAL, LogCategory.SYSTEM,
                    "Non-exception object thrown: {0}", e.ExceptionObject?.ToString() ?? "null");
                MessageBox.Show("An unknown error occurred.\nExit the program.", "Non-Exception Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// 이벤트 핸들러 내에서 발생하는 예외 처리
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleFatalException(e.Exception);
        }


        /// <summary>
        /// Task 예외 핸들러
        /// Task가 GC될 때 발생
        /// .NET 4.5+ 에서는 기본적으로 앱을 종료시키지 않음
        /// </summary>
        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved(); // 예외를 관찰했다고 표시 (앱 종료 방지)

            if (e.Exception != null)
            {
                foreach (var ex in e.Exception.InnerExceptions)
                {
                    SmLog.Message(LogLevel.FATAL, LogCategory.SYSTEM, "TaskException object thrown: {0}", ex.GetType().FullName, ex.Message);
                    MessageDisplay.Show($"TaskException.\n" +
                    $"Type: {ex.GetType().Name}\n" +
                    $"Message: {ex.Message}");
                }
            }
        }
    }
}
