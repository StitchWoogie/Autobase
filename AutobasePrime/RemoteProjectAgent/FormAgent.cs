using System;
using System.Drawing;
using System.Windows.Forms;

namespace RemoteProjectAgent
{
    public class FormAgent : Form
    {
        private NotifyIcon notifyIcon;
        private TextBox textBoxLog;
        private TextBox textBoxApiKey;
        private Label labelStatus;
        private Label labelPort;
        private Label labelProjectDir;
        private Button buttonStart;
        private Button buttonStop;
        private CheckBox checkBoxShowKey;
        private RemoteProjectServer server;
        private int port;
        private string projectDir;
        private string apiKey;

        public FormAgent(int port, string projectDir, string apiKey = "")
        {
            this.port = port;
            this.projectDir = projectDir;
            this.apiKey = apiKey;
            InitializeComponent();
            StartServer();
        }

        private void InitializeComponent()
        {
            this.Text = "Remote Project Agent";
            this.Size = new Size(600, 460);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var labelPortTitle = new Label { Text = "Port:", Location = new Point(12, 15), AutoSize = true };
            labelPort = new Label { Text = port.ToString(), Location = new Point(100, 15), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };

            var labelDirTitle = new Label { Text = "Project Dir:", Location = new Point(12, 38), AutoSize = true };
            labelProjectDir = new Label { Text = projectDir, Location = new Point(100, 38), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };

            var labelKeyTitle = new Label { Text = "API Key:", Location = new Point(12, 63), AutoSize = true };
            textBoxApiKey = new TextBox
            {
                Location = new Point(100, 60),
                Size = new Size(200, 22),
                Text = apiKey,
                UseSystemPasswordChar = true
            };
            textBoxApiKey.TextChanged += (s, e) => apiKey = textBoxApiKey.Text.Trim();

            checkBoxShowKey = new CheckBox { Text = "Show", Location = new Point(308, 62), AutoSize = true };
            checkBoxShowKey.CheckedChanged += (s, e) => textBoxApiKey.UseSystemPasswordChar = !checkBoxShowKey.Checked;

            var labelKeyHint = new Label
            {
                Text = "(empty = no authentication)",
                Location = new Point(370, 63),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font(Font.FontFamily, 8)
            };

            labelStatus = new Label { Text = "Status: Stopped", Location = new Point(12, 90), AutoSize = true, ForeColor = Color.Red };

            buttonStart = new Button { Text = "Start", Location = new Point(400, 12), Size = new Size(80, 28) };
            buttonStart.Click += (s, e) => StartServer();

            buttonStop = new Button { Text = "Stop", Location = new Point(490, 12), Size = new Size(80, 28) };
            buttonStop.Click += (s, e) => StopServer();

            textBoxLog = new TextBox
            {
                Location = new Point(12, 115),
                Size = new Size(560, 290),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9)
            };

            this.Controls.AddRange(new Control[]
            {
                labelPortTitle, labelPort, labelDirTitle, labelProjectDir,
                labelKeyTitle, textBoxApiKey, checkBoxShowKey, labelKeyHint,
                labelStatus, buttonStart, buttonStop, textBoxLog
            });

            // System tray icon
            notifyIcon = new NotifyIcon
            {
                Text = "Remote Project Agent",
                Icon = SystemIcons.Application,
                Visible = true
            };

            var trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            trayMenu.Items.Add("Exit", null, (s, e) => { this.Close(); });
            notifyIcon.ContextMenuStrip = trayMenu;
            notifyIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };

            this.Resize += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    notifyIcon.ShowBalloonTip(1000, "Remote Project Agent", "Running in system tray.", ToolTipIcon.Info);
                }
            };
        }

        private void StartServer()
        {
            if (server != null && server.IsRunning) return;

            server = new RemoteProjectServer(port, projectDir, apiKey, AppendLog);
            server.Start();

            labelStatus.Text = $"Status: Running on port {port}";
            labelStatus.ForeColor = Color.Green;
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            textBoxApiKey.Enabled = false;

            string authStatus = string.IsNullOrEmpty(apiKey) ? "No authentication" : "API Key authentication enabled";
            AppendLog($"Server started on port {port}, project dir: {projectDir}");
            AppendLog(authStatus);
        }

        private void StopServer()
        {
            if (server != null)
            {
                server.Stop();
                server = null;
            }

            labelStatus.Text = "Status: Stopped";
            labelStatus.ForeColor = Color.Red;
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            textBoxApiKey.Enabled = true;
            AppendLog("Server stopped.");
        }

        private void AppendLog(string message)
        {
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            string line = $"[{DateTime.Now:HH:mm:ss}] {message}\r\n";
            textBoxLog.AppendText(line);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopServer();
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            base.OnFormClosing(e);
        }
    }
}
