using System;

namespace LocalMain.OPCUA
{
    /// <summary>
    /// OPC UA Server log entry
    /// </summary>
    public class ServerLogEntry
    {
        public DateTime Time { get; set; } = DateTime.Now;
        /// <summary> "INFO", "WARN", "ERROR" </summary>
        public string Level { get; set; } = "INFO";
        /// <summary> "Server", "Certificate", "Auth", "TagWrite", "TagUpdate" </summary>
        public string Category { get; set; } = "";
        public string Message { get; set; } = "";
    }

    /// <summary>
    /// Static bridge that routes server log entries to registered sinks
    /// (UI display, file logger, etc.).
    /// Pattern: FormOpcUaClient.OpcUaUiLogBridge
    /// </summary>
    public static class OpcUaServerLogBridge
    {
        private static Action<ServerLogEntry> _onLog;
        private static readonly object _lock = new object();

        public static void Write(ServerLogEntry entry)
        {
            Action<ServerLogEntry> handler;
            lock (_lock)
            {
                handler = _onLog;
            }
            handler?.Invoke(entry);
        }

        public static void Write(string level, string category, string message)
        {
            Write(new ServerLogEntry
            {
                Level = level,
                Category = category,
                Message = message
            });
        }

        public static void Info(string category, string message)
            => Write("INFO", category, message);

        public static void Warn(string category, string message)
            => Write("WARN", category, message);

        public static void Error(string category, string message)
            => Write("ERROR", category, message);

        public static void Attach(Action<ServerLogEntry> handler)
        {
            lock (_lock)
            {
                _onLog += handler;
            }
        }

        public static void Detach(Action<ServerLogEntry> handler)
        {
            lock (_lock)
            {
                _onLog -= handler;
            }
        }

        public static void DetachAll()
        {
            lock (_lock)
            {
                _onLog = null;
            }
        }
    }
}
