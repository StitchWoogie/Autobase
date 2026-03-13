using System;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 로그 항목
    /// </summary>
    public class PythonAiLogEntry
    {
        public DateTime Time { get; set; }
        /// <summary>"INFO", "WARN", "ERROR"</summary>
        public string Level { get; set; }
        /// <summary>"Engine", "Connection", "Predict", "Training", "Script", "Model"</summary>
        public string Category { get; set; }
        public string Message { get; set; }

        public PythonAiLogEntry()
        {
            Time = DateTime.Now;
            Level = "INFO";
            Category = "";
            Message = "";
        }
    }

    /// <summary>
    /// Python AI Engine 로그 브릿지.
    /// OpcUaServerLogBridge 패턴: 정적 싱크 라우팅.
    /// </summary>
    public static class PythonAiLogBridge
    {
        private static Action<PythonAiLogEntry> _onLog;
        private static readonly object _lock = new object();

        public static void Write(PythonAiLogEntry entry)
        {
            Action<PythonAiLogEntry> handler;
            lock (_lock)
            {
                handler = _onLog;
            }
            if (handler != null)
            {
                try { handler(entry); } catch { }
            }
        }

        public static void Write(string level, string category, string message)
        {
            Write(new PythonAiLogEntry
            {
                Level = level,
                Category = category,
                Message = message
            });
        }

        public static void Info(string category, string message)
        {
            Write("INFO", category, message);
        }

        public static void Warn(string category, string message)
        {
            Write("WARN", category, message);
        }

        public static void Error(string category, string message)
        {
            Write("ERROR", category, message);
        }

        public static void Attach(Action<PythonAiLogEntry> handler)
        {
            lock (_lock)
            {
                _onLog += handler;
            }
        }

        public static void Detach(Action<PythonAiLogEntry> handler)
        {
            lock (_lock)
            {
                _onLog -= handler;
            }
        }
    }
}
