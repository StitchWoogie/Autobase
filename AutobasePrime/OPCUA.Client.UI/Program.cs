using NetTools;
using OpcUa.Client.Host;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OPCUA.Client.UI
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew = false;
            Mutex gM1 = new Mutex(true, "AutobaseOPCUAClientMutext", out createdNew);
            if (!createdNew)
            {
                Process p = Tools.GetPreviousProcess();
                if (p != null && p.MainWindowHandle != IntPtr.Zero)
                {
                    Win32Function.SetForegroundWindow(p.MainWindowHandle);
                }
            }
                Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new FormOpcUaClient());

            OpcUaHost host = null;

            try
            { 
                //  Host 생성
                var logger = new OpcUaUiLogger("Host");
                host = new OpcUaHost(logger);
                host.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "OPC UA Host initialization failed:\n" + ex.Message,
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                host?.Dispose();
                return;
            }

            try
            {
                // 3UI 실행
                Application.Run(new FormOpcUaClient(host));
            }
            finally
            {
                // 4️⃣ UI 종료 → Host 종료
                host.Dispose();
            }
        }

        public static Process GetPreviousProcess()
        {
            // studio에서 실행하면 .vhost가 붙는다. 이 부분을 잘라내야 테스트가 가능하다.
            string process_name = Process.GetCurrentProcess().ProcessName;

            int index = process_name.IndexOf(".vshost");

            if (index != -1)
            {
                process_name = process_name.Substring(0, index);
            }

            string name;

            foreach (Process p in Process.GetProcesses())
            {
                if (p.Id == Process.GetCurrentProcess().Id) continue;

                name = p.ProcessName;
                index = name.IndexOf(".vshost");

                if (index != -1)
                {
                    name = name.Substring(0, index);
                }

                if (String.Compare(process_name, name, true) != 0) continue;

                return p;
            }

            return null;
        }
    }
}
