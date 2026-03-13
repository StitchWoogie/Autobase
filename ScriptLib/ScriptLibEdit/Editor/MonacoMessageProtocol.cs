using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace ScriptLibEdit.Editor
{
    /// <summary>
    /// Monaco Editor ↔ .NET 통신 메시지 프로토콜.
    /// JavaScriptSerializer 기반 JSON 파싱 (.NET Framework 4.8.1 내장).
    /// </summary>
    public static class MonacoMessageProtocol
    {
        // --- JS → .NET Message Parsing ---

        public class MonacoMessage
        {
            public string type;
            public int line;
            public int column;
            public bool isDirty;
            public bool canUndo;
            public bool canRedo;
            public bool hasSelection;
            public bool isInsert;
            public string requestId;
            public string content;
            public string text;
            public int position;
        }

        // JavaScriptSerializer 인스턴스 (thread-safe read operations)
        private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer()
        {
            MaxJsonLength = int.MaxValue  // 대용량 content 지원
        };

        /// <summary>
        /// JSON 문자열을 MonacoMessage로 파싱한다.
        /// JavaScriptSerializer로 모든 JSON 엣지 케이스를 안전하게 처리한다.
        /// </summary>
        public static MonacoMessage ParseMessage(string json)
        {
            var msg = new MonacoMessage();

            if (string.IsNullOrEmpty(json)) return msg;

            try
            {
                var dict = _serializer.Deserialize<Dictionary<string, object>>(json);
                if (dict == null) return msg;

                if (dict.ContainsKey("type")) msg.type = Convert.ToString(dict["type"]);
                if (dict.ContainsKey("line")) msg.line = Convert.ToInt32(dict["line"]);
                if (dict.ContainsKey("column")) msg.column = Convert.ToInt32(dict["column"]);
                if (dict.ContainsKey("isDirty")) msg.isDirty = Convert.ToBoolean(dict["isDirty"]);
                if (dict.ContainsKey("canUndo")) msg.canUndo = Convert.ToBoolean(dict["canUndo"]);
                if (dict.ContainsKey("canRedo")) msg.canRedo = Convert.ToBoolean(dict["canRedo"]);
                if (dict.ContainsKey("hasSelection")) msg.hasSelection = Convert.ToBoolean(dict["hasSelection"]);
                if (dict.ContainsKey("isInsert")) msg.isInsert = Convert.ToBoolean(dict["isInsert"]);
                if (dict.ContainsKey("requestId")) msg.requestId = Convert.ToString(dict["requestId"]);
                if (dict.ContainsKey("content")) msg.content = Convert.ToString(dict["content"]);
                if (dict.ContainsKey("text")) msg.text = Convert.ToString(dict["text"]);
                if (dict.ContainsKey("position")) msg.position = Convert.ToInt32(dict["position"]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[MonacoProtocol] JSON parse error: " + ex.Message);
            }

            return msg;
        }

        // --- .NET → JS JSON Encoding ---

        /// <summary>
        /// 문자열을 JavaScript 안전한 형태로 이스케이프한다.
        /// ExecuteScriptAsync에서 사용.
        /// </summary>
        public static string EscapeForJs(string text)
        {
            if (text == null) return "null";

            var sb = new StringBuilder(text.Length + 20);
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\'': sb.Append("\\'"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default:
                        if (c < 0x20)
                            sb.AppendFormat("\\u{0:X4}", (int)c);
                        else
                            sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
