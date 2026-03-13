using System;
using System.Collections.Generic;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI 서비스 호출 옵션.
    /// Phase 2-4: 타임아웃, 우선순위, 컨텍스트, Fire-and-Forget 지원.
    /// </summary>
    public sealed class PythonCallOptions
    {
        /// <summary>요청 타임아웃 (ms). 0이면 기본값 사용.</summary>
        public int TimeoutMs { get; set; }

        /// <summary>요청 우선순위 (높을수록 우선)</summary>
        public PythonCallPriority Priority { get; set; }

        /// <summary>호출 사용자 ID (감사 로그용)</summary>
        public string User { get; set; }

        /// <summary>스테이션 식별자 (감사 로그용)</summary>
        public string Station { get; set; }

        /// <summary>
        /// Fire-and-Forget 모드.
        /// true이면 응답을 기다리지 않고 즉시 반환.
        /// </summary>
        public bool FireAndForget { get; set; }

        /// <summary>재시도 횟수 (0이면 재시도 안함)</summary>
        public int RetryCount { get; set; }

        /// <summary>재시도 간격 (ms)</summary>
        public int RetryDelayMs { get; set; }

        /// <summary>
        /// 요청 권한 목록 (Phase 5+).
        /// CreateRequest에서 context.permissions로 전달됨.
        /// 예: ["predict.execute"], ["script.execute", "script.tag_write"]
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>기본 옵션</summary>
        public static PythonCallOptions Default
        {
            get
            {
                return new PythonCallOptions
                {
                    TimeoutMs = 0,
                    Priority = PythonCallPriority.Normal,
                    User = null,
                    Station = null,
                    FireAndForget = false,
                    RetryCount = 0,
                    RetryDelayMs = 1000
                };
            }
        }

        /// <summary>배치 분석용 옵션 (긴 타임아웃, 낮은 우선순위)</summary>
        public static PythonCallOptions BatchAnalysis
        {
            get
            {
                return new PythonCallOptions
                {
                    TimeoutMs = 60000,
                    Priority = PythonCallPriority.Low,
                    FireAndForget = false,
                    RetryCount = 1,
                    RetryDelayMs = 2000,
                    Permissions = new List<string> { "analysis.execute" }
                };
            }
        }

        /// <summary>실시간 예측용 옵션 (짧은 타임아웃, 높은 우선순위)</summary>
        public static PythonCallOptions RealtimePredict
        {
            get
            {
                return new PythonCallOptions
                {
                    TimeoutMs = 5000,
                    Priority = PythonCallPriority.High,
                    FireAndForget = false,
                    RetryCount = 0,
                    Permissions = new List<string> { "predict.execute" }
                };
            }
        }

        /// <summary>학습 작업용 옵션 (매우 긴 타임아웃, Fire-and-Forget)</summary>
        public static PythonCallOptions Training
        {
            get
            {
                return new PythonCallOptions
                {
                    TimeoutMs = 300000,
                    Priority = PythonCallPriority.Low,
                    FireAndForget = true,
                    RetryCount = 0,
                    Permissions = new List<string> { "train.execute" }
                };
            }
        }

        /// <summary>스크립트 실행 옵션 (보통 타임아웃)</summary>
        public static PythonCallOptions ScriptExecution
        {
            get
            {
                return new PythonCallOptions
                {
                    TimeoutMs = 30000,
                    Priority = PythonCallPriority.Normal,
                    FireAndForget = false,
                    RetryCount = 0,
                    Permissions = new List<string> { "script.execute" }
                };
            }
        }
    }

    /// <summary>
    /// 호출 우선순위
    /// </summary>
    public enum PythonCallPriority
    {
        /// <summary>낮은 우선순위 (배치, 학습)</summary>
        Low = 0,
        /// <summary>보통 우선순위 (기본)</summary>
        Normal = 1,
        /// <summary>높은 우선순위 (실시간 예측)</summary>
        High = 2,
        /// <summary>긴급 (시스템 제어)</summary>
        Critical = 3
    }
}
