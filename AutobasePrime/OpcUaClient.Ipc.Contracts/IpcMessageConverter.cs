using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace OpcUaClient.Ipc.Contracts
{
    internal class IpcMessageConverter : JsonConverter<IpcMessage>
    {
        public override IpcMessage ReadJson(JsonReader reader, Type objectType, IpcMessage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var msg = new IpcMessage();

            // 기본 필드 역직렬화
            msg.ProtocolVersion = jObject["ProtocolVersion"]?.Value<int>() ?? 1;
            msg.Type = jObject["Type"]?.ToObject<IpcMessageType>() ?? IpcMessageType.Request;
            msg.Command = jObject["Command"]?.ToObject<IpcCommand>() ?? IpcCommand.Ping;
            msg.RequestId = jObject["RequestId"]?.ToObject<Guid>() ?? Guid.Empty;
            msg.ResultCode = jObject["ResultCode"]?.ToObject<IpcResultCode>() ?? IpcResultCode.Ok;
            msg.Error = jObject["Error"]?.Value<string>();

            // Payload 타입 결정
            var payloadToken = jObject["Payload"];
            if (payloadToken != null && payloadToken.Type != JTokenType.Null)
            {
                msg.Payload = DeserializePayload(msg.Command, msg.Type, payloadToken);
            }

            return msg;
        }

        private object DeserializePayload(IpcCommand command, IpcMessageType msgType, JToken token)
        {
            try
            {
                // 응답 메시지
                if (msgType == IpcMessageType.Response)
                {
                    switch (command)
                    {
                        case IpcCommand.GetRuntimeTree:
                            return token.ToObject<GetRuntimeTreeResponse>();
                        case IpcCommand.ReadItem:
                            return token.ToObject<ReadItemResponse>();
                        case IpcCommand.UpdateItem:
                            return token.ToObject<UpdateItemResponse>();
                        case IpcCommand.GetServers:
                            return token.ToObject<GetServersResponse>();
                        case IpcCommand.GetGroups:
                            return token.ToObject<GetGroupsResponse>();
                        case IpcCommand.GetItems:
                            return token.ToObject<GetItemsResponse>();
                        case IpcCommand.Ping:
                            return token.ToObject<string>();
                    }
                }
                // 이벤트 메시지
                else if (msgType == IpcMessageType.Event)
                {
                    switch (command)
                    {
                        case IpcCommand.TagValueChanged:
                            return token.ToObject<TagValueChangedEvent>();
                        case IpcCommand.ServerStateChanged:
                            return token.ToObject<ServerStateChangedEvent>();
                        case IpcCommand.RuntimeLog:
                            return token.ToObject<RuntimeLogEvent>();
                    }
                }
                // 요청 메시지
                else if (msgType == IpcMessageType.Request)
                {
                    switch (command)
                    {
                        case IpcCommand.ReadItem:
                            return token.ToObject<ReadItemRequest>();
                        case IpcCommand.WriteItem:
                            return token.ToObject<WriteItemRequest>();
                        case IpcCommand.UpdateItem:
                            return token.ToObject<UpdateItemRequest>();
                        case IpcCommand.GetGroups:
                            return token.ToObject<GetGroupsRequest>();
                        case IpcCommand.GetItems:
                            return token.ToObject<GetItemsRequest>();
                    }
                }

                // 기본: JToken 그대로 반환
                return token.ToObject<object>();
            }
            catch
            {
                // 역직렬화 실패 시 JToken 그대로
                return token.ToObject<object>();
            }
        }

        public override void WriteJson(JsonWriter writer, IpcMessage value, JsonSerializer serializer)
        {
            // 기본 직렬화 사용
            var jObject = new JObject
            {
                ["ProtocolVersion"] = value.ProtocolVersion,
                ["Type"] = (int)value.Type,
                ["Command"] = (int)value.Command,
                ["RequestId"] = value.RequestId,
                ["ResultCode"] = (int)value.ResultCode
            };

            if (value.Payload != null)
                jObject["Payload"] = JToken.FromObject(value.Payload, serializer);

            if (value.Error != null)
                jObject["Error"] = value.Error;

            jObject.WriteTo(writer);
        }
    }
}
