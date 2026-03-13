using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Contracts
{
    /// <summary>
    /// IPC 메시지 공통 포맷
    /// </summary>
    [Serializable]
    public sealed class IpcMessage
    {
        public int ProtocolVersion { get; set; } = 1;

        /// <summary>메시지 유형</summary>
        public IpcMessageType Type;

        /// <summary>
        /// 요청/이벤트 명령
        /// (Request/Response/Event 공통)
        /// </summary>
        public IpcCommand Command;

        /// <summary>
        /// 요청 식별자
        /// - Request/Response에서만 사용
        /// - Event에서는 Guid.Empty
        /// </summary>
        public Guid RequestId;

        /// <summary>
        /// 요청 데이터 / 응답 데이터 / 이벤트 데이터
        /// </summary>
        public object Payload;

        /// <summary>
        /// 처리 결과 (Response 전용)
        /// </summary>
        public IpcResultCode ResultCode;

        /// <summary>
        /// 오류 메시지 (Response 전용)
        /// </summary>
        public string Error;

        // ---------- Factory Helpers ----------

        public static IpcMessage CreateRequest(
            IpcCommand command, object payload)
        {
            return new IpcMessage
            {
                Type = IpcMessageType.Request,
                Command = command,
                RequestId = Guid.NewGuid(),
                Payload = payload
            };
        }

        public static IpcMessage CreateResponse(
            IpcMessage request, IpcResultCode result, object payload = null, string error = null)
        {
            return new IpcMessage
            {
                Type = IpcMessageType.Response,
                Command = request.Command,
                RequestId = request.RequestId,
                ResultCode = result,
                Payload = payload,
                Error = error
            };
        }

        public static IpcMessage CreateEvent(
            IpcCommand command, object payload)
        {
            return new IpcMessage
            {
                Type = IpcMessageType.Event,
                Command = command,
                RequestId = Guid.Empty,
                Payload = payload
            };
        }
    }
}
