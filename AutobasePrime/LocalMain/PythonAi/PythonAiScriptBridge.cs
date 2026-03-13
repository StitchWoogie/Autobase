using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoLibLocal;
using GraphicModule;

namespace LocalMain.PythonAi
{
    /// <summary>
    /// PythonAiManager → ScriptFunctionPythonAi Delegate Bridge.
    /// Phase 5: 태그 프리로드 + pending_writes 처리.
    /// C_init.ViewProgrammStart()에서 RegisterCallbacks(),
    /// C_init.ViewProgrammEnd()에서 UnregisterCallbacks() 호출.
    /// </summary>
    internal static class PythonAiScriptBridge
    {
        // ==================== Registration ====================

        public static void RegisterCallbacks()
        {
            ScriptFunctionPythonAi.procIsConnected = IsConnected;
            ScriptFunctionPythonAi.procExecuteScriptAsync = ExecuteScriptAsync;
            ScriptFunctionPythonAi.procCallAsync = CallAsync;
        }

        public static void UnregisterCallbacks()
        {
            ScriptFunctionPythonAi.procIsConnected = null;
            ScriptFunctionPythonAi.procExecuteScriptAsync = null;
            ScriptFunctionPythonAi.procCallAsync = null;
        }

        // ==================== Delegate Implementations ====================

        private static bool IsConnected()
        {
            return PythonAiManager.IsConnected;
        }

        /// <summary>
        /// 스크립트 실행 with 태그 프리로드 (public, PythonAiMessage 반환).
        /// 대시보드, 외부 모듈에서 직접 호출 가능.
        /// _read_tags → _tag_snapshot, _allow_tag_write → 권한, _pending_writes → 태그 반영.
        /// </summary>
        public static async Task<PythonAiMessage> ExecuteWithPreloadAsync(string code, JObject payloadObj)
        {
            if (payloadObj == null)
                payloadObj = new JObject();

            // 1. 태그 프리로드: _read_tags → _tag_snapshot
            JToken readTagsToken;
            if (payloadObj.TryGetValue("_read_tags", out readTagsToken) && readTagsToken is JArray readTags)
            {
                var snapshot = new JObject();
                for (int i = 0; i < readTags.Count; i++)
                {
                    string tagName = readTags[i].ToString();
                    object val = ReadTagValue(tagName);
                    if (val != null)
                        snapshot[tagName] = JToken.FromObject(val);
                }
                payloadObj["_tag_snapshot"] = snapshot;
                payloadObj.Remove("_read_tags");
            }

            // 히스토리 프리로드: _read_history → _history_snapshot (구조만 준비, 실제 이력 조회는 미래 확장)
            // 현재는 pass — 역방향 IPC 없이는 이력 조회 불가

            // 2. _allow_tag_write 추출 — tag_write는 명시적 요청 시에만 부여
            bool tagWriteRequested = false;
            JToken allowWriteToken;
            if (payloadObj.TryGetValue("_allow_tag_write", out allowWriteToken))
            {
                tagWriteRequested = allowWriteToken.Type == JTokenType.Boolean
                    && allowWriteToken.Value<bool>();
                payloadObj.Remove("_allow_tag_write"); // Python으로 전달 불필요
            }

            // 3. Build script payload
            var scriptPayload = new JObject();
            scriptPayload["code"] = code;
            scriptPayload["payload"] = payloadObj;

            // 4. context: permissions 전달 (tag_write는 조건부)
            var contextObj = new JObject();
            contextObj["user"] = "script";
            var permsArray = new JArray();
            permsArray.Add("script.execute");
            if (tagWriteRequested)
            {
                permsArray.Add("script.tag_write");
            }
            contextObj["permissions"] = permsArray;
            scriptPayload["context"] = contextObj;

            // 5. PythonAiManager.CallAsync로 전송 (script.execute 서비스)
            var response = await PythonAiManager.CallAsync(
                "script/execute", scriptPayload, PythonCallOptions.ScriptExecution)
                .ConfigureAwait(false);

            // 6. pending_writes 처리
            if (response != null && response.IsSuccess && response.Result != null)
            {
                await ApplyPendingWritesAsync(response).ConfigureAwait(false);
            }

            return response;
        }

        /// <summary>
        /// 스크립트 실행 (delegate용 — string 인터페이스).
        /// ExecuteWithPreloadAsync를 내부적으로 호출.
        /// </summary>
        private static async Task<string> ExecuteScriptAsync(string code, string payloadJson)
        {
            JObject payloadObj = null;
            if (!string.IsNullOrEmpty(payloadJson))
            {
                try { payloadObj = JObject.Parse(payloadJson); }
                catch { payloadObj = new JObject(); }
            }

            var response = await ExecuteWithPreloadAsync(code, payloadObj).ConfigureAwait(false);
            return SerializeResponse(response);
        }

        /// <summary>
        /// WCF 서비스에서 호출 가능한 범용 서비스 호출 (internal).
        /// ServiceDataGateServer → V2_PythonAiCall에서 사용.
        /// </summary>
        internal static async Task<string> CallServiceAsync(string service, string payloadJson, int timeoutMs)
        {
            return await CallAsync(service, payloadJson, timeoutMs).ConfigureAwait(false);
        }

        private static async Task<string> CallAsync(string service, string payloadJson, int timeoutMs)
        {
            object payload = null;
            if (!string.IsNullOrEmpty(payloadJson))
            {
                try { payload = JToken.Parse(payloadJson); }
                catch { payload = payloadJson; }
            }

            // 서비스명에서 필요한 권한을 자동 해석하여 PythonCallOptions 사용
            var options = ResolveCallOptions(service, timeoutMs);
            var response = await PythonAiManager.CallAsync(service, payload, options)
                .ConfigureAwait(false);
            return SerializeResponse(response);
        }

        /// <summary>
        /// 서비스명 접두사로 필요 권한을 자동 판별.
        /// auth.py의 DEFAULT_PERMISSIONS와 동일한 매핑 유지.
        /// </summary>
        private static PythonCallOptions ResolveCallOptions(string service, int timeoutMs)
        {
            var options = new PythonCallOptions();
            options.TimeoutMs = timeoutMs > 0 ? timeoutMs : 0;
            options.Priority = PythonCallPriority.Normal;

            // service prefix → required permission (auth.py DEFAULT_PERMISSIONS 미러)
            if (service.StartsWith("predict/"))
                options.Permissions = new System.Collections.Generic.List<string> { "predict.execute" };
            else if (service.StartsWith("analysis/"))
                options.Permissions = new System.Collections.Generic.List<string> { "analysis.execute" };
            else if (service.StartsWith("script/"))
                options.Permissions = new System.Collections.Generic.List<string> { "script.execute" };
            else if (service.StartsWith("train/"))
                options.Permissions = new System.Collections.Generic.List<string> { "train.execute" };
            else if (service.StartsWith("vision/"))
                options.Permissions = new System.Collections.Generic.List<string> { "vision.execute" };
            // system/* → permissions = null (no auth required)

            return options;
        }

        // ==================== Tag Preload ====================

        /// <summary>SCADA 태그 현재값 읽기 (AI/DI/AO/DO/ST 자동 판별)</summary>
        private static object ReadTagValue(string tagName)
        {
            try
            {
                int[] pos = new int[1];
                TagPublicClass tp = TagLib.GetStructPublic(tagName, ref pos);
                if (tp == null) return null;

                switch (tp.enumTagType)
                {
                    case EnumTagType.AI:
                        return ((TagAiClass)tp).curr;
                    case EnumTagType.DI:
                        return (double)((TagDiClass)tp).curr;
                    case EnumTagType.AO:
                        return ((TagAoClass)tp).curr;
                    case EnumTagType.DO:
                        return (double)((TagDoClass)tp).curr;
                    case EnumTagType.ST:
                        return ((TagStClass)tp).curr;
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        // ==================== Pending Writes ====================

        /// <summary>
        /// 스크립트 응답의 _pending_writes를 실제 SCADA 태그에 반영.
        /// [{tag: "Station1.Alarm", value: 1}, ...]
        /// </summary>
        private static async Task ApplyPendingWritesAsync(PythonAiMessage response)
        {
            try
            {
                JObject resultObj = null;
                if (response.Result is JObject jObj)
                    resultObj = jObj;
                else if (response.Result is JToken jt)
                    resultObj = jt as JObject;

                if (resultObj == null) return;

                JToken pendingToken;
                if (!resultObj.TryGetValue("_pending_writes", out pendingToken))
                    return;

                JArray writes = pendingToken as JArray;
                if (writes == null || writes.Count == 0)
                    return;

                for (int i = 0; i < writes.Count; i++)
                {
                    JObject w = writes[i] as JObject;
                    if (w == null) continue;

                    string tag = w.Value<string>("tag");
                    JToken val = w["value"];
                    if (string.IsNullOrEmpty(tag) || val == null) continue;

                    string valStr = val.ToString();
                    await PlcScan.SetTagValue(tag, valStr, false).ConfigureAwait(false);
                }

                PythonAiLogBridge.Info("ScriptBridge",
                    String.Format("Applied {0} pending tag writes", writes.Count));
            }
            catch (Exception ex)
            {
                PythonAiLogBridge.Error("ScriptBridge",
                    String.Format("ApplyPendingWrites error: {0}", ex.Message));
            }
        }

        // ==================== Helpers ====================

        private static string MakeError(string message)
        {
            var obj = new JObject();
            obj["ok"] = false;
            obj["error"] = message;
            return obj.ToString(Formatting.None);
        }

        /// <summary>
        /// PythonAiMessage → JSON string.
        /// {"ok":bool, "result":..., "error":"...", "elapsed_ms":double}
        /// </summary>
        private static string SerializeResponse(PythonAiMessage msg)
        {
            if (msg == null)
                return "{\"ok\":false,\"error\":\"NULL_RESPONSE\"}";

            var obj = new JObject();
            obj["ok"] = msg.IsSuccess;

            if (msg.Result != null)
            {
                if (msg.Result is JToken jt)
                    obj["result"] = jt;
                else
                    obj["result"] = JToken.FromObject(msg.Result);
            }

            if (msg.Error != null)
                obj["error"] = msg.Error;

            if (msg.DurationMs.HasValue)
                obj["elapsed_ms"] = msg.DurationMs.Value;

            return obj.ToString(Formatting.None);
        }
    }
}
