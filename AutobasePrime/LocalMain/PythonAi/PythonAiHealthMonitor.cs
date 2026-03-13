using AutoLibLocal;
using System;
using System.Diagnostics;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 상태
    /// </summary>
    public enum PythonAiState
    {
        /// <summary>미시작 또는 완전 정지</summary>
        Disconnected,
        /// <summary>프로세스 시작 중, TCP 연결 시도 중</summary>
        Connecting,
        /// <summary>정상 연결</summary>
        Connected,
        /// <summary>연속 실패 발생, 아직 재시도 중</summary>
        Degraded,
        /// <summary>Circuit Open - 재시도 대기</summary>
        Error
    }

    /// <summary>
    /// Python AI Engine 상태 변경 이벤트 Args
    /// </summary>
    public class PythonAiStateChangedEventArgs : EventArgs
    {
        public PythonAiState PreviousState { get; set; }
        public PythonAiState CurrentState { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Python AI Engine Circuit Breaker / Health Monitor.
    /// DatabaseHealthMonitor 패턴 적용.
    /// </summary>
    internal class PythonAiHealthMonitor
    {
        // Circuit Breaker 임계값
        private const int FAILURE_THRESHOLD = 5;
        private const int CIRCUIT_OPEN_DURATION_SEC = 30;

        private PythonAiState _currentState = PythonAiState.Disconnected;
        private int _consecutiveFailures = 0;
        private DateTime _lastSuccessTime = DateTime.MinValue;
        private DateTime _circuitOpenTime = DateTime.MinValue;
        private readonly object _stateLock = new object();

        /// <summary>상태 변경 이벤트</summary>
        public event EventHandler<PythonAiStateChangedEventArgs> StateChanged;

        /// <summary>현재 상태</summary>
        public PythonAiState CurrentState
        {
            get { lock (_stateLock) { return _currentState; } }
        }

        /// <summary>연속 실패 횟수</summary>
        public int ConsecutiveFailures
        {
            get { lock (_stateLock) { return _consecutiveFailures; } }
        }

        /// <summary>성공 보고 → failures 리셋, Connected 전환</summary>
        public void OnSuccess()
        {
            lock (_stateLock)
            {
                _consecutiveFailures = 0;
                _lastSuccessTime = DateTime.Now;
                ChangeState(PythonAiState.Connected, "Python AI Engine 연결 정상");
            }
        }

        /// <summary>실패 보고 → 임계값 초과 시 Circuit Open</summary>
        public void OnFailure(string detail = null)
        {
            lock (_stateLock)
            {
                _consecutiveFailures++;

                if (_consecutiveFailures >= FAILURE_THRESHOLD)
                {
                    _circuitOpenTime = DateTime.Now;
                    ChangeState(PythonAiState.Error,
                        String.Format("Python AI Engine 연속 {0}회 실패. Circuit Open. {1}",
                            _consecutiveFailures, detail ?? ""));
                }
                else
                {
                    ChangeState(PythonAiState.Degraded,
                        String.Format("Python AI Engine 실패 ({0}/{1}). {2}",
                            _consecutiveFailures, FAILURE_THRESHOLD, detail ?? ""));
                }
            }
        }

        /// <summary>연결 시도 상태로 전환</summary>
        public void OnConnecting()
        {
            lock (_stateLock)
            {
                ChangeState(PythonAiState.Connecting, "Python AI Engine 연결 시도 중");
            }
        }

        /// <summary>연결 끊김 보고</summary>
        public void OnDisconnected()
        {
            lock (_stateLock)
            {
                ChangeState(PythonAiState.Disconnected, "Python AI Engine 연결 끊김");
            }
        }

        /// <summary>
        /// 재시도 가능 여부 확인.
        /// Error 상태에서는 CIRCUIT_OPEN_DURATION 경과 후에만 허용 (Half-Open).
        /// </summary>
        public bool CanAttempt()
        {
            lock (_stateLock)
            {
                if (_currentState != PythonAiState.Error)
                    return true;

                // Half-Open: 대기 시간 경과 시 재시도 허용
                double elapsed = (DateTime.Now - _circuitOpenTime).TotalSeconds;
                return elapsed >= CIRCUIT_OPEN_DURATION_SEC;
            }
        }

        /// <summary>상태 리셋</summary>
        public void Reset()
        {
            lock (_stateLock)
            {
                _consecutiveFailures = 0;
                _circuitOpenTime = DateTime.MinValue;
                ChangeState(PythonAiState.Disconnected, "Python AI Engine 상태 리셋");
            }
        }

        private void ChangeState(PythonAiState newState, string message)
        {
            // _stateLock 안에서 호출됨
            if (_currentState == newState) return;

            var oldState = _currentState;
            _currentState = newState;

            // 이벤트 발생
            try
            {
                StateChanged?.Invoke(this, new PythonAiStateChangedEventArgs
                {
                    PreviousState = oldState,
                    CurrentState = newState,
                    Message = message
                });
            }
            catch { }

            // 로깅
            var logLevel = (newState == PythonAiState.Connected) ? LogLevel.INFO : LogLevel.WARNING;
            SmLog.Message(logLevel, LogCategory.SYSTEM, message);

            Debug.WriteLine(String.Format("PythonAi: {0} → {1}: {2}", oldState, newState, message));
        }
    }
}
