using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Client
{
    internal sealed class RequestManager
    {
        private readonly Dictionary<Guid, TaskCompletionSource<IpcMessage>> _pending
            = new Dictionary<Guid, TaskCompletionSource<IpcMessage>>();

        public async Task<T> SendAsync<T>(PipeConnection conn, IpcMessage msg)
        {
            var tcs = new TaskCompletionSource<IpcMessage>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_pending)
                _pending[msg.RequestId] = tcs;

            await conn.SendAsync(msg).ConfigureAwait(false);

            return await AwaitResponse<T>(msg.RequestId, tcs).ConfigureAwait(false);
        }

        private async Task<T> AwaitResponse<T>(
            Guid requestId,
            TaskCompletionSource<IpcMessage> tcs)
        {
            using (var cts = new CancellationTokenSource(3000))
            {
                using (cts.Token.Register(() =>
                    tcs.TrySetException(new TimeoutException("IPC timeout"))))
                {
                    try
                    {
                        var res = await tcs.Task;

                        if (res.ResultCode != IpcResultCode.Ok)
                            throw new Exception(res.Error);

                        return ConvertPayload<T>(res.Payload);
                    }
                    finally
                    {
                        lock (_pending)
                            _pending.Remove(requestId);
                    }
                }
            }
        }

        private static T ConvertPayload<T>(object payload)
        {
            if (payload == null)
                return default(T);

            // 이미 올바른 타입이면 그대로 반환
            if (payload is T typedPayload)
                return typedPayload;

            // JObject나 JArray인 경우 재직렬화
            var payloadType = payload.GetType();
            if (payloadType.FullName != null &&
                payloadType.FullName.StartsWith("Newtonsoft.Json.Linq"))
            {
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                }
                catch
                {
                    // 재직렬화 실패 시 직접 캐스팅 시도
                }
            }

            // 최후 수단: 직접 캐스팅
            return (T)payload;
        }

        public void OnResponse(IpcMessage msg)
        {
            TaskCompletionSource<IpcMessage> tcs;

            lock (_pending)
            {
                if (!_pending.TryGetValue(msg.RequestId, out tcs))
                    return;
            }

            tcs.TrySetResult(msg);
        }

        public void CancelAll(string reason)
        {
            lock (_pending)
            {
                foreach (var p in _pending.Values)
                    p.TrySetException(new Exception(reason));

                _pending.Clear();
            }
        }
    }
}
