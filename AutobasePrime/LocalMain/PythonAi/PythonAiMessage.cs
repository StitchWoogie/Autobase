using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// Python AI Engine 요청/응답 메시지 DTO.
    /// Phase 1: Id, Service, Payload → Ok, Result, Error
    /// Phase 2-4: + context, priority, protocolVersion, durationMs
    /// </summary>
    public sealed class PythonAiMessage
    {
        /// <summary>현재 C# 클라이언트가 사용하는 메시지 스키마 버전</summary>
        public const int CURRENT_MESSAGE_VERSION = 1;
        /// <summary>요청 ID (상관관계 매칭용)</summary>
        [JsonProperty("Id")]
        public string Id { get; set; }

        /// <summary>서비스 경로 (예: "predict/power", "system/ping")</summary>
        [JsonProperty("Service", NullValueHandling = NullValueHandling.Ignore)]
        public string Service { get; set; }

        /// <summary>요청 payload (JSON 직렬화 가능 객체)</summary>
        [JsonProperty("Payload", NullValueHandling = NullValueHandling.Ignore)]
        public object Payload { get; set; }

        /// <summary>응답: 성공 여부</summary>
        [JsonProperty("Ok", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Ok { get; set; }

        /// <summary>응답: 결과 데이터</summary>
        [JsonProperty("Result", NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; set; }

        /// <summary>응답: 에러 메시지</summary>
        [JsonProperty("Error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        // ==================== Phase 2-4 확장 필드 ====================

        /// <summary>프로토콜 버전 (Phase 2+): 와이어 형식 버전</summary>
        [JsonProperty("protocolVersion", NullValueHandling = NullValueHandling.Ignore)]
        public int? ProtocolVersion { get; set; }

        /// <summary>
        /// 메시지 스키마 버전: 메시지 필드 구조 버전 관리.
        /// Version 1: Id, Service, Payload, Ok, Result, Error, protocolVersion, context, priority, durationMs.
        /// 새 필드 추가 시 버전을 올리고, Python 측에서 호환성 분기 가능.
        /// </summary>
        [JsonProperty("messageVersion", NullValueHandling = NullValueHandling.Ignore)]
        public int? MessageVersion { get; set; }

        /// <summary>
        /// 지원되는 메시지 버전 목록 (협상용).
        /// system/ping 응답에서 Python 측이 지원 가능한 버전 목록을 반환하면
        /// C# 측에서 가장 높은 공통 버전을 선택.
        /// </summary>
        [JsonProperty("supportedMessageVersions", NullValueHandling = NullValueHandling.Ignore)]
        public int[] SupportedMessageVersions { get; set; }

        /// <summary>
        /// 요청 컨텍스트 (Phase 2+): 사용자, 스테이션, 세션, 권한 정보.
        /// Python 측 auth, audit에서 활용.
        /// Phase 5: object 타입으로 확장 (permissions 배열 지원).
        /// </summary>
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public object Context { get; set; }

        /// <summary>요청 우선순위 (Phase 2+): "high", "normal", "low"</summary>
        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public string Priority { get; set; }

        /// <summary>응답: 처리 시간 (ms)</summary>
        [JsonProperty("durationMs", NullValueHandling = NullValueHandling.Ignore)]
        public double? DurationMs { get; set; }

        // ==================== Factory Methods ====================

        /// <summary>요청 메시지 생성 (기본)</summary>
        public static PythonAiMessage CreateRequest(string service, object payload)
        {
            return new PythonAiMessage
            {
                Id = Guid.NewGuid().ToString("N"),
                Service = service,
                Payload = payload,
                ProtocolVersion = 2,
                MessageVersion = CURRENT_MESSAGE_VERSION
            };
        }

        /// <summary>요청 메시지 생성 (확장 옵션)</summary>
        public static PythonAiMessage CreateRequest(string service, object payload, PythonCallOptions options)
        {
            var msg = new PythonAiMessage
            {
                Id = Guid.NewGuid().ToString("N"),
                Service = service,
                Payload = payload,
                ProtocolVersion = 2,
                MessageVersion = CURRENT_MESSAGE_VERSION
            };

            if (options != null)
            {
                // 우선순위 설정
                switch (options.Priority)
                {
                    case PythonCallPriority.High:
                    case PythonCallPriority.Critical:
                        msg.Priority = "high";
                        break;
                    case PythonCallPriority.Low:
                        msg.Priority = "low";
                        break;
                    default:
                        msg.Priority = "normal";
                        break;
                }

                // 컨텍스트 설정 (Phase 5: user, station, permissions 배열 지원)
                var ctx = new JObject();
                if (!string.IsNullOrEmpty(options.User))
                    ctx["user"] = options.User;
                if (!string.IsNullOrEmpty(options.Station))
                    ctx["station"] = options.Station;
                if (options.Permissions != null && options.Permissions.Count > 0)
                {
                    var perms = new JArray();
                    for (int i = 0; i < options.Permissions.Count; i++)
                        perms.Add(options.Permissions[i]);
                    ctx["permissions"] = perms;
                }
                if (ctx.Count > 0)
                    msg.Context = ctx;
            }

            return msg;
        }

        // ==================== Result Helpers ====================

        /// <summary>Result를 지정 타입으로 변환</summary>
        public T GetResult<T>()
        {
            if (Result == null)
                return default(T);

            if (Result is T typed)
                return typed;

            if (Result is JToken jt)
                return jt.ToObject<T>();

            return default(T);
        }

        /// <summary>Result에서 특정 필드 추출</summary>
        public T GetResultField<T>(string fieldName)
        {
            if (Result == null)
                return default(T);

            JObject obj = null;

            if (Result is JObject jObj)
                obj = jObj;
            else if (Result is JToken jt)
                obj = jt as JObject;

            if (obj == null)
                return default(T);

            JToken val;
            if (obj.TryGetValue(fieldName, out val))
                return val.ToObject<T>();

            return default(T);
        }

        /// <summary>성공 응답인지 확인</summary>
        public bool IsSuccess
        {
            get { return Ok.HasValue && Ok.Value; }
        }

        /// <summary>에러 응답인지 확인</summary>
        public bool IsError
        {
            get { return Ok.HasValue && !Ok.Value; }
        }

        /// <summary>처리 시간 (ms, 없으면 -1)</summary>
        public double ElapsedMs
        {
            get { return DurationMs.HasValue ? DurationMs.Value : -1; }
        }
    }
}
