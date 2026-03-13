using System;
using System.Collections.Generic;
using System.Text;

namespace LocalMain.Barcode
{
	/// <summary>
	/// QR 코드 데이터 파싱/생성 유틸리티.
	/// 산업 환경에서 사용하는 구조화된 QR 데이터 형식을 처리한다.
	///
	/// 지원 형식:
	/// 1. Key-Value: LOT=230501;ITEM=VALVE;LINE=3
	/// 2. JSON: {"lot":"230501","item":"VALVE","line":"3"}
	/// 3. PlainText: 단순 텍스트
	/// </summary>
	public static class QRDataParser
	{
		/// <summary>
		/// Key-Value 형식의 QR 데이터를 파싱한다.
		/// 구분자: 세미콜론(;), 키-값 구분: 등호(=)
		/// </summary>
		public static Dictionary<string, string> ParseKeyValue(string data)
		{
			var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (string.IsNullOrEmpty(data))
				return result;

			string[] pairs = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string pair in pairs)
			{
				int eqIdx = pair.IndexOf('=');
				if (eqIdx > 0)
				{
					string key = pair.Substring(0, eqIdx).Trim();
					string value = pair.Substring(eqIdx + 1).Trim();
					result[key] = value;
				}
			}

			return result;
		}

		/// <summary>
		/// Dictionary를 Key-Value 형식 문자열로 변환한다.
		/// </summary>
		public static string ToKeyValueString(Dictionary<string, string> data)
		{
			if (data == null || data.Count == 0)
				return "";

			var sb = new StringBuilder();
			bool first = true;
			foreach (var kvp in data)
			{
				if (!first) sb.Append(';');
				sb.Append(kvp.Key).Append('=').Append(kvp.Value);
				first = false;
			}
			return sb.ToString();
		}

		/// <summary>
		/// 간단한 JSON 파싱 (Newtonsoft.Json에 의존하지 않는 경량 구현).
		/// 중첩 객체는 지원하지 않으며 단일 수준 key:value만 처리한다.
		/// </summary>
		public static Dictionary<string, string> ParseSimpleJson(string json)
		{
			var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (string.IsNullOrEmpty(json))
				return result;

			json = json.Trim();
			if (json.StartsWith("{") && json.EndsWith("}"))
			{
				json = json.Substring(1, json.Length - 2);
			}

			// 간단한 상태 머신 기반 파싱
			int i = 0;
			while (i < json.Length)
			{
				// 키 찾기
				string key = ReadJsonString(json, ref i);
				if (key == null) break;

				// 콜론 건너뛰기
				SkipWhitespace(json, ref i);
				if (i >= json.Length || json[i] != ':') break;
				i++; // ':'

				// 값 찾기
				SkipWhitespace(json, ref i);
				string value = ReadJsonValue(json, ref i);
				if (value == null) break;

				result[key] = value;

				// 쉼표 건너뛰기
				SkipWhitespace(json, ref i);
				if (i < json.Length && json[i] == ',')
					i++;
			}

			return result;
		}

		/// <summary>
		/// Dictionary를 간단한 JSON 문자열로 변환한다.
		/// </summary>
		public static string ToJsonString(Dictionary<string, string> data)
		{
			if (data == null || data.Count == 0)
				return "{}";

			var sb = new StringBuilder();
			sb.Append('{');
			bool first = true;
			foreach (var kvp in data)
			{
				if (!first) sb.Append(',');
				sb.Append('"').Append(EscapeJsonString(kvp.Key)).Append('"');
				sb.Append(':');
				sb.Append('"').Append(EscapeJsonString(kvp.Value)).Append('"');
				first = false;
			}
			sb.Append('}');
			return sb.ToString();
		}

		/// <summary>
		/// 바코드 데이터에서 특정 키의 값을 추출한다.
		/// Key-Value 형식과 JSON 형식을 모두 시도한다.
		/// </summary>
		public static string ExtractValue(string data, string key)
		{
			if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(key))
				return null;

			// Key-Value 형식 먼저 시도
			var kvResult = ParseKeyValue(data);
			if (kvResult.ContainsKey(key))
				return kvResult[key];

			// JSON 형식 시도
			if (data.TrimStart().StartsWith("{"))
			{
				var jsonResult = ParseSimpleJson(data);
				if (jsonResult.ContainsKey(key))
					return jsonResult[key];
			}

			return null;
		}

		/// <summary>
		/// 데이터 형식을 자동 감지한다.
		/// </summary>
		public static QRDataFormat DetectFormat(string data)
		{
			if (string.IsNullOrEmpty(data))
				return QRDataFormat.PlainText;

			string trimmed = data.Trim();
			if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
				return QRDataFormat.JSON;

			if (trimmed.Contains("=") && (trimmed.Contains(";") || !trimmed.Contains(" ")))
				return QRDataFormat.KeyValue;

			return QRDataFormat.PlainText;
		}

		#region JSON 파싱 헬퍼

		private static void SkipWhitespace(string s, ref int i)
		{
			while (i < s.Length && char.IsWhiteSpace(s[i]))
				i++;
		}

		private static string ReadJsonString(string s, ref int i)
		{
			SkipWhitespace(s, ref i);
			if (i >= s.Length || s[i] != '"')
				return null;

			i++; // opening quote
			var sb = new StringBuilder();
			while (i < s.Length && s[i] != '"')
			{
				if (s[i] == '\\' && i + 1 < s.Length)
				{
					i++;
					switch (s[i])
					{
						case '"': sb.Append('"'); break;
						case '\\': sb.Append('\\'); break;
						case '/': sb.Append('/'); break;
						case 'n': sb.Append('\n'); break;
						case 'r': sb.Append('\r'); break;
						case 't': sb.Append('\t'); break;
						default: sb.Append(s[i]); break;
					}
				}
				else
				{
					sb.Append(s[i]);
				}
				i++;
			}
			if (i < s.Length) i++; // closing quote
			return sb.ToString();
		}

		private static string ReadJsonValue(string s, ref int i)
		{
			SkipWhitespace(s, ref i);
			if (i >= s.Length) return null;

			if (s[i] == '"')
				return ReadJsonString(s, ref i);

			// 숫자, boolean, null
			var sb = new StringBuilder();
			while (i < s.Length && s[i] != ',' && s[i] != '}' && !char.IsWhiteSpace(s[i]))
			{
				sb.Append(s[i]);
				i++;
			}
			return sb.ToString();
		}

		private static string EscapeJsonString(string s)
		{
			if (string.IsNullOrEmpty(s)) return "";
			return s
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t");
		}

		#endregion
	}
}
