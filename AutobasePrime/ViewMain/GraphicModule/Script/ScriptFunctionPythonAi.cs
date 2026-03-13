using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using AutoLib;
using AutoLibLocal;
using ScriptLibRun;

namespace GraphicModule
{
    /// <summary>
    /// Python AI Engine 스크립트 함수.
    /// Delegate Bridge 패턴: GraphicModule → LocalMain.PythonAi 연결.
    /// C_init.ViewProgrammStart()에서 PythonAiScriptBridge.RegisterCallbacks()로 등록.
    ///
    /// 제공 메서드:
    ///   PythonAiIsConnected()  → int (1=연결, 0=미연결)
    ///   PythonAiScriptEx(code, payloadJson) → string (JSON)
    ///   PythonAiCall(service, payloadJson) → string (JSON)
    ///   PythonAiCallTimeout(service, payloadJson, timeoutMs) → string (JSON)
    /// </summary>
    public class ScriptFunctionPythonAi
    {
        // ==================== Delegate Bridge Definitions ====================

        public delegate bool DelegateIsConnected();
        public static DelegateIsConnected procIsConnected = null;

        public delegate Task<string> DelegateExecuteScriptAsync(string code, string payloadJson);
        public static DelegateExecuteScriptAsync procExecuteScriptAsync = null;

        public delegate Task<string> DelegateCallAsync(string service, string payloadJson, int timeoutMs);
        public static DelegateCallAsync procCallAsync = null;

        // ==================== Script Method Implementations ====================

        /// <summary>PythonAiIsConnected() → int (1=연결됨, 0=미연결)</summary>
        static async Task<(int, object)> Run_PythonAiIsConnected(
            ScriptClass scriptClass, string method_name, object[] args)
        {
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                    return (1, 0);

                if (procIsConnected == null)
                    return (1, 0);

                bool connected = procIsConnected();
                return (1, connected ? 1 : 0);
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(
                    String.Format("@PythonAiIsConnected Error: {0}", ex.Message));
                return (-1, 0);
            }
        }

        /// <summary>PythonAiScriptEx(string code, string payloadJson) → string (JSON result)</summary>
        static async Task<(int, object)> Run_PythonAiScriptEx(
            ScriptClass scriptClass, string method_name, object[] args)
        {
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                    return (1, "");

                if (procExecuteScriptAsync == null)
                {
                    scriptClass.ErrorMessage("@PythonAiScriptEx: Engine not initialized");
                    return (-1, "");
                }

                string code = (string)args[0];
                string payloadJson = (string)args[1];
                string result = await procExecuteScriptAsync(code, payloadJson)
                    .ConfigureAwait(false);
                return (1, result ?? "");
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(
                    String.Format("@PythonAiScriptEx Error: {0}", ex.Message));
                return (-1, "");
            }
        }

        /// <summary>PythonAiCall(string service, string payloadJson) → string (JSON)</summary>
        static async Task<(int, object)> Run_PythonAiCall(
            ScriptClass scriptClass, string method_name, object[] args)
        {
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                    return (1, "");

                if (procCallAsync == null)
                {
                    scriptClass.ErrorMessage("@PythonAiCall: Engine not initialized");
                    return (-1, "");
                }

                string service = (string)args[0];
                string payloadJson = (string)args[1];
                string result = await procCallAsync(service, payloadJson, 0).ConfigureAwait(false);
                return (1, result ?? "");
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(
                    String.Format("@PythonAiCall Error: {0}", ex.Message));
                return (-1, "");
            }
        }

        /// <summary>PythonAiCallTimeout(string service, string payloadJson, int timeoutMs) → string (JSON)</summary>
        static async Task<(int, object)> Run_PythonAiCallTimeout(
            ScriptClass scriptClass, string method_name, object[] args)
        {
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                    return (1, "");

                if (procCallAsync == null)
                {
                    scriptClass.ErrorMessage("@PythonAiCallTimeout: Engine not initialized");
                    return (-1, "");
                }

                string service = (string)args[0];
                string payloadJson = (string)args[1];
                int timeoutMs = Convert.ToInt32(args[2]);
                string result = await procCallAsync(service, payloadJson, timeoutMs)
                    .ConfigureAwait(false);
                return (1, result ?? "");
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(
                    String.Format("@PythonAiCallTimeout Error: {0}", ex.Message));
                return (-1, "");
            }
        }

        // ==================== PrepareMethod Registration ====================

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "PythonAi";

            prepare.AddMethod(prename, "PythonAiIsConnected", "int",
                new ScriptExternalRun.AsyncDeleMethod(Run_PythonAiIsConnected));

            prepare.AddMethod(prename, "PythonAiScriptEx", "string",
                new ScriptExternalRun.AsyncDeleMethod(Run_PythonAiScriptEx),
                "in:string:code", "in:string:payloadJson");

            prepare.AddMethod(prename, "PythonAiCall", "string",
                new ScriptExternalRun.AsyncDeleMethod(Run_PythonAiCall),
                "in:string:service", "in:string:payloadJson");

            prepare.AddMethod(prename, "PythonAiCallTimeout", "string",
                new ScriptExternalRun.AsyncDeleMethod(Run_PythonAiCallTimeout),
                "in:string:service", "in:string:payloadJson", "in:int:timeoutMs");
        }
    }
}
