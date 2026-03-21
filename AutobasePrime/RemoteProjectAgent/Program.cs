using System;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Threading;

namespace RemoteProjectAgent
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "RemoteProjectAgent_SingleInstance");

        [STAThread]
        static void Main(string[] args)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                MessageBox.Show("RemoteProjectAgent is already running.", "RemoteProjectAgent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int port = 18080;
                string projectDir = "";
                string apiKey = "";

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("/port=", StringComparison.OrdinalIgnoreCase))
                        int.TryParse(args[i].Substring(6), out port);
                    else if (args[i].StartsWith("/project=", StringComparison.OrdinalIgnoreCase))
                        projectDir = args[i].Substring(9).Trim('"');
                    else if (args[i].StartsWith("/apikey=", StringComparison.OrdinalIgnoreCase))
                        apiKey = args[i].Substring(8).Trim('"');
                }

                if (string.IsNullOrEmpty(projectDir))
                {
                    projectDir = GetDefaultProjectDir();
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormAgent(port, projectDir, apiKey));
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        static string GetDefaultProjectDir()
        {
            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Autobase");
                if (key != null)
                {
                    var val = key.GetValue("ProjectDir") as string;
                    if (!string.IsNullOrEmpty(val)) return val;
                }
            }
            catch { }

            string defaultDir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Autobase", "Project");

            if (!System.IO.Directory.Exists(defaultDir))
                System.IO.Directory.CreateDirectory(defaultDir);

            return defaultDir;
        }
    }
}
