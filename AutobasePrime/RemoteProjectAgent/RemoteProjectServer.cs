using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RemoteProjectAgent
{
    public class RemoteProjectServer
    {
        private HttpListener listener;
        private readonly int port;
        private readonly string projectDir;
        private readonly Action<string> log;
        private CancellationTokenSource cts;

        public bool IsRunning { get; private set; }

        public RemoteProjectServer(int port, string projectDir, Action<string> log)
        {
            this.port = port;
            this.projectDir = projectDir;
            this.log = log;
        }

        public void Start()
        {
            if (IsRunning) return;

            cts = new CancellationTokenSource();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{port}/");

            try
            {
                listener.Start();
            }
            catch (HttpListenerException)
            {
                // Fallback to localhost only if no admin rights
                listener = new HttpListener();
                listener.Prefixes.Add($"http://localhost:{port}/");
                listener.Start();
            }

            IsRunning = true;
            Task.Run(() => ListenLoop(cts.Token));
        }

        public void Stop()
        {
            IsRunning = false;
            cts?.Cancel();
            try { listener?.Stop(); } catch { }
            try { listener?.Close(); } catch { }
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested && IsRunning)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequest(context));
                }
                catch (ObjectDisposedException) { break; }
                catch (HttpListenerException) { break; }
                catch (Exception ex)
                {
                    log?.Invoke($"Listener error: {ex.Message}");
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                string path = request.Url.AbsolutePath.ToLower().TrimEnd('/');
                log?.Invoke($"{request.HttpMethod} {path}");

                switch (path)
                {
                    case "/api/project/download":
                        HandleProjectDownload(request, response);
                        break;
                    case "/api/project/upload":
                        HandleProjectUpload(request, response);
                        break;
                    case "/api/project/files":
                        HandleProjectFiles(request, response);
                        break;
                    case "/api/localmain/status":
                        HandleLocalMainStatus(request, response);
                        break;
                    case "/api/localmain/restart":
                        HandleLocalMainRestart(request, response);
                        break;
                    case "/api/localmain/stop":
                        HandleLocalMainStop(request, response);
                        break;
                    case "/api/localmain/start":
                        HandleLocalMainStart(request, response);
                        break;
                    case "/api/screen/capture":
                        HandleScreenCapture(request, response);
                        break;
                    case "/api/screen/navigate":
                        HandleScreenNavigate(request, response);
                        break;
                    case "/api/screen/pages":
                        HandleScreenPages(request, response);
                        break;
                    case "/api/ping":
                        SendJson(response, new { status = "ok", time = DateTime.Now.ToString("o") });
                        break;
                    default:
                        response.StatusCode = 404;
                        SendJson(response, new { error = "Not found" });
                        break;
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"Error: {ex.Message}");
                try
                {
                    response.StatusCode = 500;
                    SendJson(response, new { error = ex.Message });
                }
                catch { }
            }
        }

        #region Project Operations

        private void HandleProjectDownload(HttpListenerRequest request, HttpListenerResponse response)
        {
            log?.Invoke("Packaging project for download...");

            string tempFile = Path.GetTempFileName();
            try
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
                ZipFile.CreateFromDirectory(projectDir, tempFile, CompressionLevel.Fastest, false);

                byte[] data = File.ReadAllBytes(tempFile);
                response.ContentType = "application/zip";
                response.AddHeader("Content-Disposition", "attachment; filename=project.zip");
                response.ContentLength64 = data.Length;
                response.OutputStream.Write(data, 0, data.Length);

                log?.Invoke($"Project downloaded ({data.Length / 1024} KB)");
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }

        private void HandleProjectUpload(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != "POST")
            {
                response.StatusCode = 405;
                SendJson(response, new { error = "POST required" });
                return;
            }

            log?.Invoke("Receiving project upload...");

            string tempFile = Path.GetTempFileName();
            try
            {
                using (var fs = File.Create(tempFile))
                {
                    request.InputStream.CopyTo(fs);
                }

                long size = new FileInfo(tempFile).Length;
                log?.Invoke($"Received {size / 1024} KB, extracting...");

                // Backup current project
                string backupDir = projectDir + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                if (Directory.Exists(projectDir))
                {
                    CopyDirectory(projectDir, backupDir);
                    log?.Invoke($"Backup created: {backupDir}");
                }

                // Extract uploaded project
                string tempExtract = Path.Combine(Path.GetTempPath(), "RemoteProjectAgent_extract_" + Guid.NewGuid().ToString("N"));
                ZipFile.ExtractToDirectory(tempFile, tempExtract);

                // Copy extracted files over project directory
                CopyDirectory(tempExtract, projectDir);

                // Clean up temp extract
                if (Directory.Exists(tempExtract))
                    Directory.Delete(tempExtract, true);

                log?.Invoke("Project uploaded and deployed successfully.");
                SendJson(response, new { status = "ok", message = "Project deployed", backup = backupDir });
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }

        private void HandleProjectFiles(HttpListenerRequest request, HttpListenerResponse response)
        {
            var files = new List<object>();
            if (Directory.Exists(projectDir))
            {
                CollectFiles(projectDir, projectDir, files);
            }
            SendJson(response, new { projectDir, files });
        }

        private void CollectFiles(string baseDir, string dir, List<object> files)
        {
            foreach (var file in Directory.GetFiles(dir))
            {
                var info = new FileInfo(file);
                string relativePath = file.Substring(baseDir.Length).TrimStart('\\', '/');
                files.Add(new
                {
                    path = relativePath,
                    size = info.Length,
                    modified = info.LastWriteTime.ToString("o")
                });
            }

            foreach (var subDir in Directory.GetDirectories(dir))
            {
                CollectFiles(baseDir, subDir, files);
            }
        }

        #endregion

        #region LocalMain Control

        private void HandleLocalMainStatus(HttpListenerRequest request, HttpListenerResponse response)
        {
            var process = FindProcess("LocalMain");
            bool isRunning = process != null;

            SendJson(response, new
            {
                running = isRunning,
                processId = isRunning ? process.Id : -1,
                processName = isRunning ? process.ProcessName : "",
                startTime = isRunning ? process.StartTime.ToString("o") : "",
                projectDir
            });
        }

        private void HandleLocalMainRestart(HttpListenerRequest request, HttpListenerResponse response)
        {
            log?.Invoke("Restarting LocalMain...");

            // Read optional page argument from body
            string page = "";
            if (request.HasEntityBody)
            {
                var body = ReadJsonBody(request);
                if (body.ContainsKey("page"))
                    page = body["page"]?.ToString() ?? "";
            }

            // Stop existing
            StopLocalMainProcess();
            Thread.Sleep(1500);

            // Start new
            string result = StartLocalMainProcess(page);

            SendJson(response, new { status = "ok", message = result });
        }

        private void HandleLocalMainStop(HttpListenerRequest request, HttpListenerResponse response)
        {
            log?.Invoke("Stopping LocalMain...");
            StopLocalMainProcess();
            SendJson(response, new { status = "ok", message = "LocalMain stopped" });
        }

        private void HandleLocalMainStart(HttpListenerRequest request, HttpListenerResponse response)
        {
            string page = "";
            if (request.HasEntityBody)
            {
                var body = ReadJsonBody(request);
                if (body.ContainsKey("page"))
                    page = body["page"]?.ToString() ?? "";
            }

            string result = StartLocalMainProcess(page);
            SendJson(response, new { status = "ok", message = result });
        }

        private void StopLocalMainProcess()
        {
            var process = FindProcess("LocalMain");
            if (process == null)
            {
                log?.Invoke("LocalMain is not running.");
                return;
            }

            try
            {
                // Try graceful close first
                process.CloseMainWindow();
                if (!process.WaitForExit(5000))
                {
                    process.Kill();
                    log?.Invoke("LocalMain force-killed.");
                }
                else
                {
                    log?.Invoke("LocalMain closed gracefully.");
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"Error stopping LocalMain: {ex.Message}");
            }
        }

        private string StartLocalMainProcess(string page)
        {
            // Find LocalMain.exe relative to this agent or in known paths
            string exePath = FindLocalMainExe();
            if (string.IsNullOrEmpty(exePath))
            {
                log?.Invoke("LocalMain.exe not found.");
                return "LocalMain.exe not found";
            }

            var existing = FindProcess("LocalMain");
            if (existing != null)
            {
                return "LocalMain is already running";
            }

            string args = "";
            if (!string.IsNullOrEmpty(page))
                args = $"\"{page}\"";

            Process.Start(exePath, args);
            log?.Invoke($"LocalMain started: {exePath} {args}");
            return $"LocalMain started with args: {args}";
        }

        private string FindLocalMainExe()
        {
            // Check same directory as this agent
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(dir, "LocalMain.exe");
            if (File.Exists(path)) return path;

            // Check parent directories
            var parent = Directory.GetParent(dir);
            if (parent != null)
            {
                path = Path.Combine(parent.FullName, "LocalMain.exe");
                if (File.Exists(path)) return path;
            }

            // Check running processes to get original path
            var process = FindProcess("LocalMain");
            if (process != null)
            {
                try { return process.MainModule.FileName; }
                catch { }
            }

            return null;
        }

        private Process FindProcess(string name)
        {
            var processes = Process.GetProcessesByName(name);
            if (processes.Length > 0) return processes[0];

            processes = Process.GetProcessesByName(name + ".vshost");
            if (processes.Length > 0) return processes[0];

            return null;
        }

        #endregion

        #region Screen Capture & Navigation

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private void HandleScreenCapture(HttpListenerRequest request, HttpListenerResponse response)
        {
            var process = FindProcess("LocalMain");
            if (process == null)
            {
                // Try ViewMain as well
                process = FindProcess("ViewMain");
            }

            if (process == null)
            {
                response.StatusCode = 404;
                SendJson(response, new { error = "LocalMain/ViewMain not running" });
                return;
            }

            try
            {
                IntPtr hwnd = process.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                {
                    response.StatusCode = 404;
                    SendJson(response, new { error = "No main window found" });
                    return;
                }

                GetWindowRect(hwnd, out RECT rect);
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width <= 0 || height <= 0)
                {
                    response.StatusCode = 500;
                    SendJson(response, new { error = "Invalid window size" });
                    return;
                }

                using (var bmp = new Bitmap(width, height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        IntPtr hdc = g.GetHdc();
                        PrintWindow(hwnd, hdc, 0x2); // PW_RENDERFULLCONTENT
                        g.ReleaseHdc(hdc);
                    }

                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        byte[] data = ms.ToArray();

                        response.ContentType = "image/png";
                        response.ContentLength64 = data.Length;
                        response.OutputStream.Write(data, 0, data.Length);
                    }
                }

                log?.Invoke("Screen captured.");
            }
            catch (Exception ex)
            {
                log?.Invoke($"Screen capture error: {ex.Message}");
                response.StatusCode = 500;
                SendJson(response, new { error = ex.Message });
            }
        }

        private void HandleScreenNavigate(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (!request.HasEntityBody)
            {
                response.StatusCode = 400;
                SendJson(response, new { error = "Body required with 'page' field" });
                return;
            }

            var body = ReadJsonBody(request);
            if (!body.ContainsKey("page"))
            {
                response.StatusCode = 400;
                SendJson(response, new { error = "'page' field required" });
                return;
            }

            string page = body["page"]?.ToString() ?? "";
            log?.Invoke($"Navigating to page: {page}");

            // Navigate by restarting LocalMain with the page argument
            // Or use shared memory / IPC mechanism
            // For now, use process restart approach
            StopLocalMainProcess();
            Thread.Sleep(1500);
            string result = StartLocalMainProcess(page);

            SendJson(response, new { status = "ok", page, message = result });
        }

        private void HandleScreenPages(HttpListenerRequest request, HttpListenerResponse response)
        {
            var pages = new List<object>();
            string graphicDir = Path.Combine(projectDir, "graphic");

            if (Directory.Exists(graphicDir))
            {
                foreach (var file in Directory.GetFiles(graphicDir))
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".modx" || ext == ".mod")
                    {
                        var info = new FileInfo(file);
                        pages.Add(new
                        {
                            name = Path.GetFileName(file),
                            size = info.Length,
                            modified = info.LastWriteTime.ToString("o")
                        });
                    }
                }
            }

            SendJson(response, new { pages });
        }

        #endregion

        #region Helpers

        private void SendJson(HttpListenerResponse response, object data)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string json = serializer.Serialize(data);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            response.ContentType = "application/json; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private Dictionary<string, object> ReadJsonBody(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string body = reader.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<Dictionary<string, object>>(body)
                       ?? new Dictionary<string, object>();
            }
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

        #endregion
    }
}
