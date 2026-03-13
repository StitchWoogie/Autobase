using Avalonia;
using System;
using System.Threading;

namespace LauncherMain
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]

        //public static void Main(string[] args) => BuildAvaloniaApp()
        //    .StartWithClassicDesktopLifetime(args);
        public static void Main(string[] args)
        {
            // LocalMain 및 ScadaX 동시 실행 불가능하게 방지
            bool createdNew = false;
            Mutex gM1 = new Mutex(true, "AutoBaseLauncher", out createdNew);

            if (createdNew)
            {
                
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            else
            {

                return;
            }



        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
