using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoLibLocal;

namespace Studio.RemoteProjectEditor
{
    public class FormRemoteProjectEditor : Form
    {
        #region Controls

        // Connection
        private GroupBox groupBoxConnection;
        private Label labelHost;
        private TextBox textBoxHost;
        private Label labelPort;
        private TextBox textBoxPort;
        private Label labelApiKey;
        private TextBox textBoxApiKey;
        private Button buttonConnect;
        private Button buttonDisconnect;
        private Label labelConnectionStatus;

        // Project
        private GroupBox groupBoxProject;
        private Button buttonDownloadProject;
        private Button buttonUploadProject;
        private Button buttonOpenProjectFolder;
        private Label labelProjectStatus;
        private ProgressBar progressBar;

        // LocalMain Control
        private GroupBox groupBoxLocalMain;
        private Button buttonLocalMainStart;
        private Button buttonLocalMainStop;
        private Button buttonLocalMainRestart;
        private Label labelLocalMainStatus;
        private Button buttonRefreshStatus;

        // Screen Monitor
        private GroupBox groupBoxScreen;
        private PictureBox pictureBoxScreen;
        private Button buttonCaptureScreen;
        private CheckBox checkBoxAutoRefresh;
        private ComboBox comboBoxRefreshInterval;
        private Timer timerAutoRefresh;

        // Page Navigation
        private GroupBox groupBoxPages;
        private ListBox listBoxPages;
        private Button buttonNavigatePage;
        private Button buttonRefreshPages;

        // Log
        private TextBox textBoxLog;

        #endregion

        private RemoteProjectClient client;
        private string localProjectDir;
        private bool isConnected;

        public FormRemoteProjectEditor()
        {
            InitializeComponent();
            LoadSettings();
        }

        #region InitializeComponent

        private void InitializeComponent()
        {
            this.Text = "Remote Project Editor";
            this.Size = new Size(1050, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new Size(900, 650);

            // ── Connection Group ──
            groupBoxConnection = new GroupBox
            {
                Text = "Remote Connection",
                Location = new Point(12, 12),
                Size = new Size(500, 95)
            };

            labelHost = new Label { Text = "Host:", Location = new Point(10, 25), AutoSize = true };
            textBoxHost = new TextBox { Location = new Point(50, 22), Size = new Size(150, 22), Text = "192.168.0.1" };

            labelPort = new Label { Text = "Port:", Location = new Point(210, 25), AutoSize = true };
            textBoxPort = new TextBox { Location = new Point(250, 22), Size = new Size(60, 22), Text = "18080" };

            labelApiKey = new Label { Text = "API Key:", Location = new Point(10, 50), AutoSize = true };
            textBoxApiKey = new TextBox { Location = new Point(70, 47), Size = new Size(240, 22), UseSystemPasswordChar = true };

            buttonConnect = new Button { Text = "Connect", Location = new Point(320, 20), Size = new Size(80, 28) };
            buttonConnect.Click += ButtonConnect_Click;

            buttonDisconnect = new Button { Text = "Disconnect", Location = new Point(405, 20), Size = new Size(85, 28), Enabled = false };
            buttonDisconnect.Click += ButtonDisconnect_Click;

            labelConnectionStatus = new Label { Text = "● Disconnected", Location = new Point(10, 74), AutoSize = true, ForeColor = Color.Red, Font = new Font(Font, FontStyle.Bold) };

            groupBoxConnection.Controls.AddRange(new Control[] { labelHost, textBoxHost, labelPort, textBoxPort, labelApiKey, textBoxApiKey, buttonConnect, buttonDisconnect, labelConnectionStatus });

            // ── LocalMain Control Group ──
            groupBoxLocalMain = new GroupBox
            {
                Text = "LocalMain Control",
                Location = new Point(520, 12),
                Size = new Size(500, 95)
            };

            buttonLocalMainStart = new Button { Text = "Start", Location = new Point(10, 22), Size = new Size(70, 28), Enabled = false };
            buttonLocalMainStart.Click += ButtonLocalMainStart_Click;

            buttonLocalMainStop = new Button { Text = "Stop", Location = new Point(85, 22), Size = new Size(70, 28), Enabled = false };
            buttonLocalMainStop.Click += ButtonLocalMainStop_Click;

            buttonLocalMainRestart = new Button { Text = "Restart", Location = new Point(160, 22), Size = new Size(75, 28), Enabled = false };
            buttonLocalMainRestart.Click += ButtonLocalMainRestart_Click;

            buttonRefreshStatus = new Button { Text = "Refresh", Location = new Point(240, 22), Size = new Size(70, 28), Enabled = false };
            buttonRefreshStatus.Click += ButtonRefreshStatus_Click;

            labelLocalMainStatus = new Label { Text = "Status: Unknown", Location = new Point(10, 60), AutoSize = true };

            groupBoxLocalMain.Controls.AddRange(new Control[] { buttonLocalMainStart, buttonLocalMainStop, buttonLocalMainRestart, buttonRefreshStatus, labelLocalMainStatus });

            // ── Project Group ──
            groupBoxProject = new GroupBox
            {
                Text = "Project Management",
                Location = new Point(12, 113),
                Size = new Size(500, 80)
            };

            buttonDownloadProject = new Button { Text = "Download Project", Location = new Point(10, 22), Size = new Size(120, 28), Enabled = false };
            buttonDownloadProject.Click += ButtonDownloadProject_Click;

            buttonUploadProject = new Button { Text = "Upload Project", Location = new Point(135, 22), Size = new Size(120, 28), Enabled = false };
            buttonUploadProject.Click += ButtonUploadProject_Click;

            buttonOpenProjectFolder = new Button { Text = "Open Folder", Location = new Point(260, 22), Size = new Size(100, 28), Enabled = false };
            buttonOpenProjectFolder.Click += ButtonOpenProjectFolder_Click;

            labelProjectStatus = new Label { Text = "", Location = new Point(10, 55), AutoSize = true };

            progressBar = new ProgressBar { Location = new Point(370, 22), Size = new Size(120, 28), Style = ProgressBarStyle.Marquee, Visible = false };

            groupBoxProject.Controls.AddRange(new Control[] { buttonDownloadProject, buttonUploadProject, buttonOpenProjectFolder, labelProjectStatus, progressBar });

            // ── Screen Monitor Group ──
            groupBoxScreen = new GroupBox
            {
                Text = "Screen Monitor",
                Location = new Point(520, 113),
                Size = new Size(500, 80)
            };

            buttonCaptureScreen = new Button { Text = "Capture", Location = new Point(10, 22), Size = new Size(80, 28), Enabled = false };
            buttonCaptureScreen.Click += ButtonCaptureScreen_Click;

            checkBoxAutoRefresh = new CheckBox { Text = "Auto Refresh", Location = new Point(100, 26), AutoSize = true };
            checkBoxAutoRefresh.CheckedChanged += CheckBoxAutoRefresh_CheckedChanged;

            comboBoxRefreshInterval = new ComboBox
            {
                Location = new Point(210, 24),
                Size = new Size(80, 22),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBoxRefreshInterval.Items.AddRange(new object[] { "1 sec", "2 sec", "3 sec", "5 sec", "10 sec" });
            comboBoxRefreshInterval.SelectedIndex = 2; // 3 sec default

            groupBoxScreen.Controls.AddRange(new Control[] { buttonCaptureScreen, checkBoxAutoRefresh, comboBoxRefreshInterval });

            // ── PictureBox for screen ──
            pictureBoxScreen = new PictureBox
            {
                Location = new Point(520, 197),
                Size = new Size(500, 355),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };

            // ── Page Navigation Group ──
            groupBoxPages = new GroupBox
            {
                Text = "Page Navigation",
                Location = new Point(12, 197),
                Size = new Size(500, 355)
            };

            listBoxPages = new ListBox
            {
                Location = new Point(10, 22),
                Size = new Size(370, 285)
            };
            listBoxPages.DoubleClick += ListBoxPages_DoubleClick;

            buttonRefreshPages = new Button { Text = "Refresh", Location = new Point(390, 22), Size = new Size(100, 28) };
            buttonRefreshPages.Click += ButtonRefreshPages_Click;

            buttonNavigatePage = new Button { Text = "Navigate", Location = new Point(390, 55), Size = new Size(100, 28) };
            buttonNavigatePage.Click += ButtonNavigatePage_Click;

            groupBoxPages.Controls.AddRange(new Control[] { listBoxPages, buttonRefreshPages, buttonNavigatePage });

            // ── Log ──
            textBoxLog = new TextBox
            {
                Location = new Point(12, 558),
                Size = new Size(1008, 140),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Consolas", 9),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // ── Timer ──
            timerAutoRefresh = new Timer();
            timerAutoRefresh.Tick += TimerAutoRefresh_Tick;

            // ── Add to form ──
            this.Controls.AddRange(new Control[]
            {
                groupBoxConnection, groupBoxLocalMain, groupBoxProject, groupBoxScreen,
                pictureBoxScreen, groupBoxPages, textBoxLog
            });
        }

        #endregion

        #region Connection

        private async void ButtonConnect_Click(object sender, EventArgs e)
        {
            string host = textBoxHost.Text.Trim();
            int port;
            if (!int.TryParse(textBoxPort.Text.Trim(), out port))
            {
                MessageBox.Show("Invalid port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string apiKey = textBoxApiKey.Text.Trim();
            client = new RemoteProjectClient(host, port, apiKey);
            AppendLog($"Connecting to {host}:{port}...");

            buttonConnect.Enabled = false;

            bool pingOk = await Task.Run(() => client.Ping());

            if (!pingOk)
            {
                isConnected = false;
                labelConnectionStatus.Text = "● Connection Failed";
                labelConnectionStatus.ForeColor = Color.Red;
                buttonConnect.Enabled = true;
                AppendLog("Connection failed. Make sure RemoteProjectAgent is running on the remote PC.");
                return;
            }

            // Verify API Key authentication
            string authResult = await Task.Run(() => client.TestAuthentication());

            if (authResult == "unauthorized")
            {
                isConnected = false;
                labelConnectionStatus.Text = "● Authentication Failed";
                labelConnectionStatus.ForeColor = Color.Red;
                buttonConnect.Enabled = true;
                AppendLog("Authentication failed. Check the API Key.");
                MessageBox.Show("API Key가 올바르지 않습니다.\nRemote Agent에 설정된 API Key를 확인하세요.",
                    "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (authResult == "ok")
            {
                isConnected = true;
                labelConnectionStatus.Text = $"● Connected ({host}:{port})";
                labelConnectionStatus.ForeColor = Color.Green;
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
                SetControlsEnabled(true);
                AppendLog("Connected and authenticated successfully.");
                SaveSettings();

                // Auto-refresh status and pages
                ButtonRefreshStatus_Click(null, null);
                ButtonRefreshPages_Click(null, null);
            }
            else
            {
                isConnected = false;
                labelConnectionStatus.Text = "● Connection Error";
                labelConnectionStatus.ForeColor = Color.Red;
                buttonConnect.Enabled = true;
                AppendLog($"Connection error: {authResult}");
            }
        }

        private void ButtonDisconnect_Click(object sender, EventArgs e)
        {
            isConnected = false;
            client = null;
            labelConnectionStatus.Text = "● Disconnected";
            labelConnectionStatus.ForeColor = Color.Red;
            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;
            checkBoxAutoRefresh.Checked = false;
            SetControlsEnabled(false);
            AppendLog("Disconnected.");
        }

        private void SetControlsEnabled(bool enabled)
        {
            buttonDownloadProject.Enabled = enabled;
            buttonUploadProject.Enabled = enabled;
            buttonLocalMainStart.Enabled = enabled;
            buttonLocalMainStop.Enabled = enabled;
            buttonLocalMainRestart.Enabled = enabled;
            buttonRefreshStatus.Enabled = enabled;
            buttonCaptureScreen.Enabled = enabled;
        }

        #endregion

        #region Project Operations

        private async void ButtonDownloadProject_Click(object sender, EventArgs e)
        {
            if (client == null) return;

            // Select local directory to save
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select folder to save the downloaded project";
                dialog.SelectedPath = localProjectDir ?? TotalConfig.sDirWorkProject ?? "";

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                localProjectDir = dialog.SelectedPath;
            }

            AppendLog("Downloading project from remote PC...");
            progressBar.Visible = true;
            buttonDownloadProject.Enabled = false;

            try
            {
                string tempZip = Path.GetTempFileName() + ".zip";

                await Task.Run(() => client.DownloadProject(tempZip));

                long size = new FileInfo(tempZip).Length;
                AppendLog($"Downloaded {size / 1024} KB. Extracting to: {localProjectDir}");

                await Task.Run(() =>
                {
                    string tempDir = Path.Combine(Path.GetTempPath(), "RemoteProjectEditor_" + Guid.NewGuid().ToString("N"));
                    ZipFile.ExtractToDirectory(tempZip, tempDir);
                    CopyDirectory(tempDir, localProjectDir);
                    if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
                    if (File.Exists(tempZip)) File.Delete(tempZip);
                });

                labelProjectStatus.Text = $"Project downloaded to: {localProjectDir}";
                buttonOpenProjectFolder.Enabled = true;
                AppendLog("Project downloaded and extracted successfully.");
            }
            catch (Exception ex)
            {
                AppendLog($"Download error: {ex.Message}");
                MessageBox.Show($"Download failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                buttonDownloadProject.Enabled = true;
            }
        }

        private async void ButtonUploadProject_Click(object sender, EventArgs e)
        {
            if (client == null) return;

            // Select local project directory to upload
            string uploadDir = localProjectDir;
            if (string.IsNullOrEmpty(uploadDir) || !Directory.Exists(uploadDir))
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Select project folder to upload";
                    dialog.SelectedPath = TotalConfig.sDirWorkProject ?? "";

                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    uploadDir = dialog.SelectedPath;
                }
            }

            var confirm = MessageBox.Show(
                $"Upload project from:\n{uploadDir}\n\nThis will overwrite the remote project. Continue?",
                "Confirm Upload",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            AppendLog($"Uploading project from: {uploadDir}");
            progressBar.Visible = true;
            buttonUploadProject.Enabled = false;

            try
            {
                string tempZip = Path.GetTempFileName() + ".zip";

                await Task.Run(() =>
                {
                    if (File.Exists(tempZip)) File.Delete(tempZip);
                    ZipFile.CreateFromDirectory(uploadDir, tempZip, CompressionLevel.Fastest, false);
                });

                long size = new FileInfo(tempZip).Length;
                AppendLog($"Compressed to {size / 1024} KB. Uploading...");

                var result = await Task.Run(() => client.UploadProject(tempZip));

                if (File.Exists(tempZip)) File.Delete(tempZip);

                string msg = result.ContainsKey("message") ? result["message"].ToString() : "OK";
                string backup = result.ContainsKey("backup") ? result["backup"].ToString() : "";

                AppendLog($"Upload complete: {msg}");
                if (!string.IsNullOrEmpty(backup))
                    AppendLog($"Remote backup created: {backup}");

                labelProjectStatus.Text = "Project uploaded successfully.";
            }
            catch (Exception ex)
            {
                AppendLog($"Upload error: {ex.Message}");
                MessageBox.Show($"Upload failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Visible = false;
                buttonUploadProject.Enabled = true;
            }
        }

        private void ButtonOpenProjectFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(localProjectDir) && Directory.Exists(localProjectDir))
            {
                System.Diagnostics.Process.Start("explorer.exe", localProjectDir);
            }
        }

        #endregion

        #region LocalMain Control

        private async void ButtonLocalMainStart_Click(object sender, EventArgs e)
        {
            if (client == null) return;
            AppendLog("Starting LocalMain...");
            try
            {
                var result = await Task.Run(() => client.StartLocalMain());
                string msg = result.ContainsKey("message") ? result["message"].ToString() : "OK";
                AppendLog($"Start result: {msg}");
                await Task.Delay(1000);
                ButtonRefreshStatus_Click(null, null);
            }
            catch (Exception ex)
            {
                AppendLog($"Start error: {ex.Message}");
            }
        }

        private async void ButtonLocalMainStop_Click(object sender, EventArgs e)
        {
            if (client == null) return;

            var confirm = MessageBox.Show("Stop LocalMain on the remote PC?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            AppendLog("Stopping LocalMain...");
            try
            {
                var result = await Task.Run(() => client.StopLocalMain());
                string msg = result.ContainsKey("message") ? result["message"].ToString() : "OK";
                AppendLog($"Stop result: {msg}");
                await Task.Delay(1000);
                ButtonRefreshStatus_Click(null, null);
            }
            catch (Exception ex)
            {
                AppendLog($"Stop error: {ex.Message}");
            }
        }

        private async void ButtonLocalMainRestart_Click(object sender, EventArgs e)
        {
            if (client == null) return;

            string page = listBoxPages.SelectedItem?.ToString() ?? "";
            AppendLog($"Restarting LocalMain (page: {(string.IsNullOrEmpty(page) ? "default" : page)})...");

            try
            {
                var result = await Task.Run(() => client.RestartLocalMain(page));
                string msg = result.ContainsKey("message") ? result["message"].ToString() : "OK";
                AppendLog($"Restart result: {msg}");
                await Task.Delay(2000);
                ButtonRefreshStatus_Click(null, null);
            }
            catch (Exception ex)
            {
                AppendLog($"Restart error: {ex.Message}");
            }
        }

        private async void ButtonRefreshStatus_Click(object sender, EventArgs e)
        {
            if (client == null) return;

            try
            {
                var result = await Task.Run(() => client.GetLocalMainStatus());
                bool running = result.ContainsKey("running") && (bool)result["running"];

                if (running)
                {
                    string pid = result.ContainsKey("processId") ? result["processId"].ToString() : "?";
                    string start = result.ContainsKey("startTime") ? result["startTime"].ToString() : "";
                    labelLocalMainStatus.Text = $"Status: Running (PID: {pid})";
                    labelLocalMainStatus.ForeColor = Color.Green;
                }
                else
                {
                    labelLocalMainStatus.Text = "Status: Stopped";
                    labelLocalMainStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                labelLocalMainStatus.Text = $"Status: Error ({ex.Message})";
                labelLocalMainStatus.ForeColor = Color.Red;
            }
        }

        #endregion

        #region Screen Monitor

        private async void ButtonCaptureScreen_Click(object sender, EventArgs e)
        {
            await CaptureScreenAsync();
        }

        private async Task CaptureScreenAsync()
        {
            if (client == null || !isConnected) return;

            try
            {
                var bmp = await Task.Run(() => client.CaptureScreen());
                if (bmp != null)
                {
                    var old = pictureBoxScreen.Image;
                    pictureBoxScreen.Image = bmp;
                    old?.Dispose();
                }
                else
                {
                    AppendLog("Screen capture returned no image.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Screen capture error: {ex.Message}");
            }
        }

        private void CheckBoxAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoRefresh.Checked && isConnected)
            {
                int[] intervals = { 1000, 2000, 3000, 5000, 10000 };
                int idx = comboBoxRefreshInterval.SelectedIndex;
                if (idx < 0 || idx >= intervals.Length) idx = 2;
                timerAutoRefresh.Interval = intervals[idx];
                timerAutoRefresh.Start();
                AppendLog($"Auto-refresh started ({comboBoxRefreshInterval.SelectedItem}).");
            }
            else
            {
                timerAutoRefresh.Stop();
                if (!checkBoxAutoRefresh.Checked)
                    AppendLog("Auto-refresh stopped.");
            }
        }

        private async void TimerAutoRefresh_Tick(object sender, EventArgs e)
        {
            await CaptureScreenAsync();
        }

        #endregion

        #region Page Navigation

        private async void ButtonRefreshPages_Click(object sender, EventArgs e)
        {
            if (client == null) return;

            try
            {
                var result = await Task.Run(() => client.GetPages());
                listBoxPages.Items.Clear();

                if (result.ContainsKey("pages") && result["pages"] is ArrayList pages)
                {
                    foreach (var page in pages)
                    {
                        if (page is Dictionary<string, object> dict && dict.ContainsKey("name"))
                        {
                            listBoxPages.Items.Add(dict["name"].ToString());
                        }
                    }
                    AppendLog($"Found {listBoxPages.Items.Count} pages.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Page list error: {ex.Message}");
            }
        }

        private async void ButtonNavigatePage_Click(object sender, EventArgs e)
        {
            if (client == null || listBoxPages.SelectedItem == null) return;

            string page = listBoxPages.SelectedItem.ToString();
            AppendLog($"Navigating to: {page}");

            try
            {
                var result = await Task.Run(() => client.NavigateToPage(page));
                string msg = result.ContainsKey("message") ? result["message"].ToString() : "OK";
                AppendLog($"Navigate result: {msg}");

                // Wait and capture screen after navigation
                await Task.Delay(3000);
                await CaptureScreenAsync();
            }
            catch (Exception ex)
            {
                AppendLog($"Navigate error: {ex.Message}");
            }
        }

        private void ListBoxPages_DoubleClick(object sender, EventArgs e)
        {
            ButtonNavigatePage_Click(sender, e);
        }

        #endregion

        #region Settings

        private void LoadSettings()
        {
            try
            {
                textBoxHost.Text = TotalConfigProject.LoadConfig("RemoteProjectEditor", "Connection", "Host", "192.168.0.1");
                textBoxPort.Text = TotalConfigProject.LoadConfig("RemoteProjectEditor", "Connection", "Port", "18080");
                textBoxApiKey.Text = TotalConfigProject.LoadConfig("RemoteProjectEditor", "Connection", "ApiKey", "");
                localProjectDir = TotalConfigProject.LoadConfig("RemoteProjectEditor", "Project", "LocalDir", "");
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                TotalConfigProject.SaveConfig("RemoteProjectEditor", "Connection", "Host", textBoxHost.Text.Trim());
                TotalConfigProject.SaveConfig("RemoteProjectEditor", "Connection", "Port", textBoxPort.Text.Trim());
                TotalConfigProject.SaveConfig("RemoteProjectEditor", "Connection", "ApiKey", textBoxApiKey.Text.Trim());
                if (!string.IsNullOrEmpty(localProjectDir))
                    TotalConfigProject.SaveConfig("RemoteProjectEditor", "Project", "LocalDir", localProjectDir);
            }
            catch { }
        }

        #endregion

        #region Helpers

        private void AppendLog(string message)
        {
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }
            textBoxLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string dest = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dest = Path.Combine(targetDir, Path.GetFileName(dir));
                CopyDirectory(dir, dest);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerAutoRefresh.Stop();
            timerAutoRefresh.Dispose();
            pictureBoxScreen.Image?.Dispose();
            SaveSettings();
            base.OnFormClosing(e);
        }

        #endregion
    }
}
