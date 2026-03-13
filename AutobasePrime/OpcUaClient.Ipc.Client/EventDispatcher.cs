using Newtonsoft.Json.Linq;
using OpcUaClient.Ipc.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaClient.Ipc.Client
{
    internal sealed class EventDispatcher
    {
        public event EventHandler<TagValueChangedEvent> TagValueChanged;
        public event EventHandler<ServerStateChangedEvent> ServerStateChanged;
        public event EventHandler<RuntimeLogEvent> RuntimeLog;

        public void Dispatch(IpcMessage msg)
        {
            switch (msg.Command)
            {
                case IpcCommand.TagValueChanged:
                    {
                        var e = SafeCast<TagValueChangedEvent>(msg.Payload);
                        if (e != null) TagValueChanged?.Invoke(this, e);
                    }
                    break;

                case IpcCommand.ServerStateChanged:
                    {
                        var e = SafeCast<ServerStateChangedEvent>(msg.Payload);
                        if (e != null) ServerStateChanged?.Invoke(this, e);
                    }
                    break;

                case IpcCommand.RuntimeLog:
                    {
                        var e = SafeCast<RuntimeLogEvent>(msg.Payload);
                        if (e != null) RuntimeLog?.Invoke(this, e);
                    }
                    break;
            }
        }

        /// <summary>
        /// 역직렬화 결과가 JObject일 수 있으므로 안전하게 변환.
        /// </summary>
        private static T SafeCast<T>(object payload) where T : class
        {
            if (payload == null) return null;

            if (payload is T typed)
                return typed;

            // IpcMessageConverter 실패 시 JObject로 올 수 있음
            if (payload is JObject jObj)
            {
                try { return jObj.ToObject<T>(); }
                catch { return null; }
            }

            return null;
        }
    }
}
