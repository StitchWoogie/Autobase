using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// 요청-응답 상관관계 관리.
    /// OpcUaClient.Ipc.Client.RequestManager 패턴 적용.
    /// </summary>
    internal sealed class PythonAiRequestManager
    {
        private readonly Dictionary<string, TaskCompletionSource<PythonAiMessage>> _pending
            = new Dictionary<string, TaskCompletionSource<PythonAiMessage>>();
        private readonly object _lock = new object();

        /// <summary>요청 전송 후 응답 대기</summary>
        public async Task<PythonAiMessage> SendAsync(
            PythonAiTcpConnection connection,
            PythonAiMessage request,
            int timeoutMs)
        {
            var tcs = new TaskCompletionSource<PythonAiMessage>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            lock (_lock)
            {
                _pending[request.Id] = tcs;
            }

            try
            {
                await connection.SendAsync(request).ConfigureAwait(false);

                using (var cts = new CancellationTokenSource(timeoutMs))
                {
                    cts.Token.Register(() =>
                    {
                        tcs.TrySetException(new TimeoutException(
                            String.Format("Python AI request timeout ({0}ms): {1}", timeoutMs, request.Service)));
                    });

                    return await tcs.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                lock (_lock)
                {
                    _pending.Remove(request.Id);
                }
            }
        }

        /// <summary>응답 수신 처리</summary>
        public void OnResponse(PythonAiMessage response)
        {
            if (response == null || string.IsNullOrEmpty(response.Id))
                return;

            TaskCompletionSource<PythonAiMessage> tcs;
            lock (_lock)
            {
                if (!_pending.TryGetValue(response.Id, out tcs))
                    return;
            }

            tcs.TrySetResult(response);
        }

        /// <summary>모든 보류 요청 취소</summary>
        public void CancelAll(string reason)
        {
            List<TaskCompletionSource<PythonAiMessage>> list;
            lock (_lock)
            {
                list = new List<TaskCompletionSource<PythonAiMessage>>(_pending.Values);
                _pending.Clear();
            }

            var ex = new OperationCanceledException(
                String.Format("Python AI Engine: {0}", reason));
            foreach (var tcs in list)
            {
                tcs.TrySetException(ex);
            }
        }

        /// <summary>보류 요청 수</summary>
        public int PendingCount
        {
            get { lock (_lock) { return _pending.Count; } }
        }
    }
}
