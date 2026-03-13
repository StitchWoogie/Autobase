using System.Threading.Tasks;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 매니저 인터페이스.
    /// 테스트 및 DI 용도.
    /// </summary>
    public interface IPythonManager
    {
        /// <summary>현재 연결 상태</summary>
        PythonAiState State { get; }

        /// <summary>연결 여부</summary>
        bool IsConnected { get; }

        /// <summary>서비스 호출 (기본 옵션)</summary>
        Task<PythonAiMessage> CallAsync(string service, object payload, int timeoutMs = 0);

        /// <summary>서비스 호출 (확장 옵션)</summary>
        Task<PythonAiMessage> CallAsync(string service, object payload, PythonCallOptions options);

        /// <summary>Ping 호출</summary>
        Task<bool> PingAsync();

        /// <summary>시스템 상태 조회</summary>
        Task<PythonAiMessage> GetStatusAsync();

        /// <summary>시스템 메트릭 조회</summary>
        Task<PythonAiMessage> GetMetricsAsync();

        /// <summary>모델 목록 조회</summary>
        Task<PythonAiMessage> GetModelsAsync();

        /// <summary>건강 상태 조회</summary>
        Task<PythonAiMessage> GetHealthAsync();
    }
}
