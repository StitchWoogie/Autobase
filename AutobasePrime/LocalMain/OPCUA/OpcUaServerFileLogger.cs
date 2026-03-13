using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalMain.OPCUA
{
    /// <summary>
    /// File-based persistent logger with daily rotation.
    /// Log path: {sDirWorkProject}\OpcData\Server\Logs\opcua_server_{yyyy-MM-dd}.log
    /// </summary>
    public sealed class OpcUaServerFileLogger : IDisposable
    {
        private readonly string _logDirectory;
        private readonly object _writeLock = new object();
        private StreamWriter _writer;
        private string _currentDate;
        private const int MaxLogDays = 30;

        public OpcUaServerFileLogger(string logDirectory)
        {
            _logDirectory = logDirectory;

            try
            {
                Directory.CreateDirectory(_logDirectory);
            }
            catch
            {
                // ignore — WriteLog will retry
            }
        }

        /// <summary>
        /// Writes a log entry to the daily log file (thread-safe).
        /// Registered as a sink via OpcUaServerLogBridge.Attach().
        /// </summary>
        public void WriteLog(ServerLogEntry entry)
        {
            if (entry == null) return;

            lock (_writeLock)
            {
                try
                {
                    EnsureWriter();
                    if (_writer == null) return;

                    string line = FormatLogLine(entry);
                    _writer.WriteLine(line);
                    _writer.Flush();
                }
                catch
                {
                    // logging should never crash the server
                }
            }
        }

        private static string FormatLogLine(ServerLogEntry entry)
        {
            return string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] [{1}] [{2}] {3}",
                entry.Time,
                entry.Level ?? "INFO",
                entry.Category ?? "",
                entry.Message ?? "");
        }

        private void EnsureWriter()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            if (_currentDate == today && _writer != null)
                return;

            // close previous day's writer
            _writer?.Close();
            _writer = null;

            try
            {
                Directory.CreateDirectory(_logDirectory);

                string filePath = Path.Combine(_logDirectory,
                    string.Format("opcua_server_{0}.log", today));

                _writer = new StreamWriter(filePath, append: true, encoding: Encoding.UTF8);
                _currentDate = today;

                CleanOldLogs();
            }
            catch
            {
                _writer = null;
            }
        }

        private void CleanOldLogs()
        {
            try
            {
                var cutoff = DateTime.Now.AddDays(-MaxLogDays);
                var files = Directory.GetFiles(_logDirectory, "opcua_server_*.log");

                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime < cutoff)
                    {
                        fi.Delete();
                    }
                }
            }
            catch
            {
                // non-critical — skip cleanup
            }
        }

        public void Dispose()
        {
            lock (_writeLock)
            {
                _writer?.Close();
                _writer = null;
            }
        }
    }
}
