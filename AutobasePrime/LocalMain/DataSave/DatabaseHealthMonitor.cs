using AutoLibLocal;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain
{
    #region DB 상태 모니터링

    /// <summary>
    /// DB 연결 상태
    /// </summary>
    public enum DbConnectionState
    {
        Connected,          // 정상 연결
        Connecting,         // 연결 시도 중
        Disconnected,       // 연결 끊김
        Error              // 오류 상태
    }

    /// <summary>
    /// DB 상태 이벤트 Args
    /// </summary>
    public class DbStateChangedEventArgs : EventArgs
    {
        public DbConnectionState PreviousState { get; set; }
        public DbConnectionState CurrentState { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// DB 상태 모니터
    /// </summary>
    public class DatabaseHealthMonitor
    {
        private static DatabaseHealthMonitor _instance;
        private static readonly object _instanceLock = new object();

        private readonly string _connectionString;
        private DbConnectionState _currentState = DbConnectionState.Disconnected;
        private DateTime _lastCheckTime = DateTime.MinValue;
        private DateTime _lastSuccessTime = DateTime.MinValue;
        private int _consecutiveFailures = 0;
        private readonly Timer _healthCheckTimer;
        private readonly object _stateLock = new object();

        public event EventHandler<DbStateChangedEventArgs> StateChanged;

        public DbConnectionState CurrentState
        {
            get { lock (_stateLock) { return _currentState; } }
        }

        public DateTime LastSuccessTime
        {
            get { lock (_stateLock) { return _lastSuccessTime; } }
        }

        public int ConsecutiveFailures
        {
            get { lock (_stateLock) { return _consecutiveFailures; } }
        }

        
        /// <summary>
        /// 싱글톤 인스턴스 (private 생성자 사용)
        /// </summary>
        public static DatabaseHealthMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(
                        "DatabaseHealthMonitor is not initialized.");
                }
                return _instance;
            }
        }

        /// <summary>
        /// 싱글톤 초기화
        /// </summary>
        public static void Initialize(string connectionString)
        {
            if (_instance == null)
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new DatabaseHealthMonitor(connectionString);
                        Debug.WriteLine("DatabaseHealthMonitor 싱글톤 초기화 완료");
                    }
                }
            }
        }

        /// <summary>
        /// 초기화 여부 확인
        /// </summary>
        public static bool IsInitialized
        {
            get { return _instance != null; }
        }

        public DatabaseHealthMonitor(string connectionString)
        {
            _connectionString = connectionString;
            _healthCheckTimer = new Timer(HealthCheckCallback, null,
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
        }

        private async void HealthCheckCallback(object state)
        {
            await CheckHealthAsync();
        }

        /// <summary>
        /// DB 연결 상태 확인
        /// </summary>
        public async Task<bool> CheckHealthAsync()
        {
            _lastCheckTime = DateTime.Now;

            try
            {
                // 짧은 타임아웃으로 연결 문자열 생성
                var builder = new NpgsqlConnectionStringBuilder(_connectionString)
                {
                    Timeout = 5,  // 연결 타임아웃 5초
                    CommandTimeout = 3  // 명령 타임아웃 3초
                };
                using (var connection = new NpgsqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand("SELECT 1", connection))
                    {
                        command.CommandTimeout = 3;
                        var result = await command.ExecuteScalarAsync();

                        if (result != null && (int)result == 1)
                        {
                            lock (_stateLock)
                            {
                                _lastSuccessTime = DateTime.Now;
                                _consecutiveFailures = 0;
                                ChangeState(DbConnectionState.Connected, "DB 연결 정상");
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (_stateLock)
                {
                    _consecutiveFailures++;
                    ChangeState(DbConnectionState.Error,
                        string.Format("DB 연결 실패 (연속 {0}회): {1}", _consecutiveFailures, ex.Message));
                }

                Debug.WriteLine(string.Format("DB Health Check 실패: {0}", ex.Message));
                MessageDisplay.Show($"DB connection failed: {ex.Message}");
            }

            return false;
        }

        private void ChangeState(DbConnectionState newState, string message)
        {
            DbConnectionState oldState;

            lock (_stateLock)
            {
                if (_currentState == newState) return;

                oldState = _currentState;
                _currentState = newState;
            }

            // 상태 변경 이벤트 발생
            StateChanged?.Invoke(this, new DbStateChangedEventArgs
            {
                PreviousState = oldState,
                CurrentState = newState,
                Message = message
            });

            // 로깅
            LogLevel logLevel = newState == DbConnectionState.Connected ?
                LogLevel.INFO : LogLevel.ERROR;
            SmLog.Message(logLevel, LogCategory.DATA_SAVE, message);
        }

        /// <summary>
        /// 싱글톤 정리 (애플리케이션 종료 시)
        /// </summary>
        public static void Shutdown()
        {
            if (_instance != null)
            {
                lock (_instanceLock)
                {
                    if (_instance != null)
                    {
                        _instance.Dispose();
                        _instance = null;
                        Debug.WriteLine("DatabaseHealthMonitor 싱글톤 종료");
                    }
                }
            }
        }

        public void Dispose()
        {
            _healthCheckTimer?.Dispose();
        }
    }

    #endregion
}
