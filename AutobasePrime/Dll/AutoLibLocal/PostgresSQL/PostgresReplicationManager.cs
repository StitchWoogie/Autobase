using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoLibLocal.PostgresSQL
{
    /// <summary>
    /// PostgreSQL 이중화(Replication) 서버 상태
    /// </summary>
    public enum ReplicationServerRole
    {
        Primary,
        Standby,
        Unknown
    }

    /// <summary>
    /// 이중화 서버 연결 상태
    /// </summary>
    public enum ReplicationConnectionStatus
    {
        Connected,
        Disconnected,
        Connecting,
        Failed
    }

    /// <summary>
    /// 이중화 모드
    /// </summary>
    public enum ReplicationMode
    {
        None,               // 이중화 미사용
        StreamingReplication // Streaming Replication (기본)
    }

    /// <summary>
    /// 이중화 서버 정보
    /// </summary>
    public class ReplicationServerInfo
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Timezone { get; set; }
        public ReplicationServerRole Role { get; set; }
        public ReplicationConnectionStatus Status { get; set; }
        public DateTime LastHealthCheck { get; set; }
        public long ReplicationLagBytes { get; set; }

        public string BuildConnectionString()
        {
            var builder = new StringBuilder();
            builder.Append($"Host={Host};");
            builder.Append($"Port={Port};");
            builder.Append($"Username={Username};");
            builder.Append($"Password={Password};");
            builder.Append($"Database={Database};");
            if (!string.IsNullOrEmpty(Timezone))
                builder.Append($"Timezone={Timezone};");
            builder.Append("Timeout=10;");
            builder.Append("CommandTimeout=30;");
            return builder.ToString();
        }
    }

    /// <summary>
    /// PostgreSQL 이중화(Replication) 관리자
    /// Primary-Standby 구성의 Streaming Replication을 관리한다.
    ///
    /// 주요 기능:
    /// 1. Primary/Standby 서버 상태 모니터링
    /// 2. 자동 Failover (Primary 장애 시 Standby 승격)
    /// 3. 읽기 부하 분산 (Read: Standby, Write: Primary)
    /// 4. Replication Lag 모니터링
    /// </summary>
    public class PostgresReplicationManager : IDisposable
    {
        private static PostgresReplicationManager _instance;
        private static readonly object _instanceLock = new object();

        private ReplicationServerInfo _primaryServer;
        private ReplicationServerInfo _standbyServer;
        private ReplicationMode _replicationMode;
        private bool _useReadWriteSplit;
        private bool _autoFailover;

        private System.Timers.Timer _healthCheckTimer;
        private readonly int _healthCheckIntervalMs;
        private readonly int _failoverTimeoutMs;
        private int _consecutiveFailures;
        private readonly int _maxConsecutiveFailures;

        private bool _isFailoverInProgress;
        private readonly object _failoverLock = new object();

        private bool _disposed;

        /// <summary>
        /// 이중화 활성화 여부
        /// </summary>
        public bool IsReplicationEnabled => _replicationMode != ReplicationMode.None
                                            && _standbyServer != null
                                            && !string.IsNullOrEmpty(_standbyServer.Host);

        /// <summary>
        /// 현재 Primary 서버 연결 문자열
        /// </summary>
        public string PrimaryConnectionString => _primaryServer?.BuildConnectionString();

        /// <summary>
        /// 현재 Standby 서버 연결 문자열 (읽기 전용)
        /// </summary>
        public string StandbyConnectionString => _standbyServer?.BuildConnectionString();

        /// <summary>
        /// 쓰기용 연결 문자열 (항상 Primary)
        /// </summary>
        public string WriteConnectionString => PrimaryConnectionString;

        /// <summary>
        /// 읽기용 연결 문자열 (Read/Write Split 사용 시 Standby, 아니면 Primary)
        /// </summary>
        public string ReadConnectionString
        {
            get
            {
                if (_useReadWriteSplit && IsReplicationEnabled
                    && _standbyServer.Status == ReplicationConnectionStatus.Connected)
                {
                    return StandbyConnectionString;
                }
                return PrimaryConnectionString;
            }
        }

        /// <summary>
        /// Failover 발생 시 이벤트
        /// </summary>
        public event EventHandler<FailoverEventArgs> OnFailover;

        /// <summary>
        /// 서버 상태 변경 시 이벤트
        /// </summary>
        public event EventHandler<ServerStatusChangedEventArgs> OnServerStatusChanged;

        public static PostgresReplicationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                            _instance = new PostgresReplicationManager();
                    }
                }
                return _instance;
            }
        }

        private PostgresReplicationManager()
        {
            _replicationMode = ReplicationMode.None;
            _useReadWriteSplit = false;
            _autoFailover = true;
            _healthCheckIntervalMs = 5000;  // 5초
            _failoverTimeoutMs = 30000;      // 30초
            _maxConsecutiveFailures = 3;
            _consecutiveFailures = 0;
            _isFailoverInProgress = false;
        }

        /// <summary>
        /// 이중화 설정 초기화
        /// </summary>
        public void Initialize(
            ReplicationServerInfo primary,
            ReplicationServerInfo standby,
            ReplicationMode mode,
            bool useReadWriteSplit,
            bool autoFailover)
        {
            _primaryServer = primary ?? throw new ArgumentNullException(nameof(primary));
            _standbyServer = standby;
            _replicationMode = mode;
            _useReadWriteSplit = useReadWriteSplit;
            _autoFailover = autoFailover;

            if (_primaryServer.Role == ReplicationServerRole.Unknown)
                _primaryServer.Role = ReplicationServerRole.Primary;

            if (_standbyServer != null && _standbyServer.Role == ReplicationServerRole.Unknown)
                _standbyServer.Role = ReplicationServerRole.Standby;

            Debug.WriteLine($"[Replication] 이중화 초기화 - 모드: {mode}, R/W Split: {useReadWriteSplit}, Auto Failover: {autoFailover}");
        }

        /// <summary>
        /// 헬스체크 타이머 시작
        /// </summary>
        public void StartHealthCheck()
        {
            if (!IsReplicationEnabled) return;

            StopHealthCheck();

            _healthCheckTimer = new System.Timers.Timer(_healthCheckIntervalMs);
            _healthCheckTimer.Elapsed += async (s, e) => await PerformHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();

            Debug.WriteLine($"[Replication] 헬스체크 시작 (간격: {_healthCheckIntervalMs}ms)");
        }

        /// <summary>
        /// 헬스체크 타이머 중지
        /// </summary>
        public void StopHealthCheck()
        {
            if (_healthCheckTimer != null)
            {
                _healthCheckTimer.Stop();
                _healthCheckTimer.Dispose();
                _healthCheckTimer = null;
            }
        }

        /// <summary>
        /// 서버 연결 상태 확인
        /// </summary>
        private async Task<bool> CheckServerConnection(ReplicationServerInfo server)
        {
            if (server == null) return false;

            try
            {
                using (var conn = new NpgsqlConnection(server.BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    {
                        await cmd.ExecuteScalarAsync();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Replication] 서버 연결 실패 ({server.Host}:{server.Port}): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 서버의 실제 역할(Primary/Standby) 확인
        /// </summary>
        private async Task<ReplicationServerRole> DetectServerRole(ReplicationServerInfo server)
        {
            if (server == null) return ReplicationServerRole.Unknown;

            try
            {
                using (var conn = new NpgsqlConnection(server.BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand("SELECT pg_is_in_recovery()", conn))
                    {
                        bool isInRecovery = (bool)await cmd.ExecuteScalarAsync();
                        return isInRecovery ? ReplicationServerRole.Standby : ReplicationServerRole.Primary;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Replication] 역할 감지 실패 ({server.Host}:{server.Port}): {ex.Message}");
                return ReplicationServerRole.Unknown;
            }
        }

        /// <summary>
        /// Replication Lag 확인 (바이트 단위)
        /// </summary>
        public async Task<long> GetReplicationLagBytes()
        {
            if (!IsReplicationEnabled) return -1;

            try
            {
                using (var conn = new NpgsqlConnection(_primaryServer.BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    // pg_stat_replication에서 lag 확인
                    string sql = @"
                        SELECT COALESCE(
                            pg_wal_lsn_diff(pg_current_wal_lsn(), sent_lsn), 0
                        )::bigint AS lag_bytes
                        FROM pg_stat_replication
                        LIMIT 1";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            long lagBytes = Convert.ToInt64(result);
                            if (_standbyServer != null)
                                _standbyServer.ReplicationLagBytes = lagBytes;
                            return lagBytes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Replication] Replication lag 확인 실패: {ex.Message}");
            }

            return -1;
        }

        /// <summary>
        /// Replication 상태 상세 정보 조회
        /// </summary>
        public async Task<ReplicationStatusInfo> GetReplicationStatus()
        {
            var status = new ReplicationStatusInfo
            {
                Mode = _replicationMode,
                IsEnabled = IsReplicationEnabled,
                AutoFailover = _autoFailover,
                UseReadWriteSplit = _useReadWriteSplit
            };

            if (_primaryServer != null)
            {
                status.PrimaryHost = $"{_primaryServer.Host}:{_primaryServer.Port}";
                status.PrimaryStatus = _primaryServer.Status;
                status.PrimaryRole = await DetectServerRole(_primaryServer);
            }

            if (_standbyServer != null)
            {
                status.StandbyHost = $"{_standbyServer.Host}:{_standbyServer.Port}";
                status.StandbyStatus = _standbyServer.Status;
                status.StandbyRole = await DetectServerRole(_standbyServer);
                status.ReplicationLagBytes = await GetReplicationLagBytes();
            }

            return status;
        }

        /// <summary>
        /// 주기적 헬스체크 수행
        /// </summary>
        private async Task PerformHealthCheck()
        {
            if (_isFailoverInProgress) return;

            try
            {
                // Primary 서버 체크
                bool primaryOk = await CheckServerConnection(_primaryServer);
                var oldPrimaryStatus = _primaryServer.Status;
                _primaryServer.Status = primaryOk
                    ? ReplicationConnectionStatus.Connected
                    : ReplicationConnectionStatus.Disconnected;
                _primaryServer.LastHealthCheck = DateTime.Now;

                if (oldPrimaryStatus != _primaryServer.Status)
                {
                    OnServerStatusChanged?.Invoke(this, new ServerStatusChangedEventArgs
                    {
                        Server = _primaryServer,
                        OldStatus = oldPrimaryStatus,
                        NewStatus = _primaryServer.Status
                    });
                }

                // Standby 서버 체크
                if (_standbyServer != null)
                {
                    bool standbyOk = await CheckServerConnection(_standbyServer);
                    var oldStandbyStatus = _standbyServer.Status;
                    _standbyServer.Status = standbyOk
                        ? ReplicationConnectionStatus.Connected
                        : ReplicationConnectionStatus.Disconnected;
                    _standbyServer.LastHealthCheck = DateTime.Now;

                    if (oldStandbyStatus != _standbyServer.Status)
                    {
                        OnServerStatusChanged?.Invoke(this, new ServerStatusChangedEventArgs
                        {
                            Server = _standbyServer,
                            OldStatus = oldStandbyStatus,
                            NewStatus = _standbyServer.Status
                        });
                    }

                    // Replication lag 업데이트
                    if (primaryOk && standbyOk)
                    {
                        await GetReplicationLagBytes();
                    }
                }

                // Primary 장애 감지 및 Failover
                if (!primaryOk)
                {
                    _consecutiveFailures++;
                    Debug.WriteLine($"[Replication] Primary 연결 실패 ({_consecutiveFailures}/{_maxConsecutiveFailures})");

                    if (_autoFailover && _consecutiveFailures >= _maxConsecutiveFailures)
                    {
                        await ExecuteFailover();
                    }
                }
                else
                {
                    _consecutiveFailures = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Replication] 헬스체크 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// Failover 실행 - Standby를 새로운 Primary로 승격
        /// </summary>
        private async Task ExecuteFailover()
        {
            lock (_failoverLock)
            {
                if (_isFailoverInProgress) return;
                _isFailoverInProgress = true;
            }

            try
            {
                Debug.WriteLine("[Replication] === Failover 시작 ===");

                if (_standbyServer == null ||
                    _standbyServer.Status != ReplicationConnectionStatus.Connected)
                {
                    Debug.WriteLine("[Replication] Standby 서버가 사용 불가능하여 Failover 중단");
                    return;
                }

                // Standby 서버가 실제로 접근 가능한지 확인
                bool standbyReachable = await CheckServerConnection(_standbyServer);
                if (!standbyReachable)
                {
                    Debug.WriteLine("[Replication] Standby 서버 연결 불가, Failover 중단");
                    return;
                }

                // Standby 역할 확인
                var standbyRole = await DetectServerRole(_standbyServer);
                Debug.WriteLine($"[Replication] Standby 서버 현재 역할: {standbyRole}");

                // Primary/Standby 역할 교체
                var oldPrimary = _primaryServer;
                var oldStandby = _standbyServer;

                _primaryServer = oldStandby;
                _primaryServer.Role = ReplicationServerRole.Primary;

                _standbyServer = oldPrimary;
                _standbyServer.Role = ReplicationServerRole.Standby;
                _standbyServer.Status = ReplicationConnectionStatus.Disconnected;

                // ConfigDataDB 업데이트
                ConfigDataDB.sPostgresHost = _primaryServer.Host;
                ConfigDataDB.sPostgresPort = _primaryServer.Port;

                // 이중화 Standby 정보 업데이트
                ConfigDataDB.sStandbyHost = _standbyServer.Host;
                ConfigDataDB.sStandbyPort = _standbyServer.Port;

                // 연결 문자열 재구성
                ConfigDataDB.RebuildConnectionString();

                _consecutiveFailures = 0;

                Debug.WriteLine($"[Replication] Failover 완료 - 새 Primary: {_primaryServer.Host}:{_primaryServer.Port}");

                OnFailover?.Invoke(this, new FailoverEventArgs
                {
                    OldPrimary = oldPrimary,
                    NewPrimary = _primaryServer,
                    FailoverTime = DateTime.Now,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Replication] Failover 실패: {ex.Message}");

                OnFailover?.Invoke(this, new FailoverEventArgs
                {
                    FailoverTime = DateTime.Now,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
            finally
            {
                lock (_failoverLock)
                {
                    _isFailoverInProgress = false;
                }
            }
        }

        /// <summary>
        /// 수동 Failover 실행
        /// </summary>
        public async Task<bool> ManualFailover()
        {
            if (!IsReplicationEnabled)
            {
                Debug.WriteLine("[Replication] 이중화가 활성화되지 않아 Failover 불가");
                return false;
            }

            _consecutiveFailures = _maxConsecutiveFailures; // 강제 트리거
            await ExecuteFailover();
            return _primaryServer.Status == ReplicationConnectionStatus.Connected;
        }

        /// <summary>
        /// 연결 가능한 연결 문자열 반환 (Failover 고려)
        /// </summary>
        public string GetAvailableConnectionString(bool forWrite = true)
        {
            if (!IsReplicationEnabled)
                return PrimaryConnectionString;

            if (forWrite)
                return WriteConnectionString;

            return ReadConnectionString;
        }

        /// <summary>
        /// Standby 서버로 연결 테스트
        /// </summary>
        public async Task<bool> TestStandbyConnection()
        {
            if (_standbyServer == null) return false;
            return await CheckServerConnection(_standbyServer);
        }

        /// <summary>
        /// Primary 서버로 연결 테스트
        /// </summary>
        public async Task<bool> TestPrimaryConnection()
        {
            if (_primaryServer == null) return false;
            return await CheckServerConnection(_primaryServer);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            StopHealthCheck();
            _instance = null;
        }
    }

    #region Event Args

    public class FailoverEventArgs : EventArgs
    {
        public ReplicationServerInfo OldPrimary { get; set; }
        public ReplicationServerInfo NewPrimary { get; set; }
        public DateTime FailoverTime { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ServerStatusChangedEventArgs : EventArgs
    {
        public ReplicationServerInfo Server { get; set; }
        public ReplicationConnectionStatus OldStatus { get; set; }
        public ReplicationConnectionStatus NewStatus { get; set; }
    }

    #endregion

    #region Replication Status DTO

    public class ReplicationStatusInfo
    {
        public ReplicationMode Mode { get; set; }
        public bool IsEnabled { get; set; }
        public bool AutoFailover { get; set; }
        public bool UseReadWriteSplit { get; set; }

        public string PrimaryHost { get; set; }
        public ReplicationConnectionStatus PrimaryStatus { get; set; }
        public ReplicationServerRole PrimaryRole { get; set; }

        public string StandbyHost { get; set; }
        public ReplicationConnectionStatus StandbyStatus { get; set; }
        public ReplicationServerRole StandbyRole { get; set; }

        public long ReplicationLagBytes { get; set; }

        public string ReplicationLagDisplay
        {
            get
            {
                if (ReplicationLagBytes < 0) return "N/A";
                if (ReplicationLagBytes < 1024) return $"{ReplicationLagBytes} B";
                if (ReplicationLagBytes < 1024 * 1024) return $"{ReplicationLagBytes / 1024.0:F1} KB";
                return $"{ReplicationLagBytes / (1024.0 * 1024.0):F1} MB";
            }
        }
    }

    #endregion
}
