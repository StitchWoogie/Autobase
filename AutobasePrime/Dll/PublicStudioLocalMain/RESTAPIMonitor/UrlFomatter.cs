using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutobaseRESTAPIMonitor
{
    public class UrlFormatter
    {
        // URL에서 사용할 수 있는 포맷 문자열 정의
        private static readonly string[] ValidFormats = new string[]
        {
            @"\{yyyy([-+]\d+[hd])?\}", @"\{MM([-+]\d+[hd])?\}", @"\{dd([-+]\d+[hd])?\}",           // 날짜 포맷
            @"\{HH([-+]\d+[hd])?\}", @"\{mm([-+]\d+[hd])?\}", @"\{ss([-+]\d+[hd])?\}",             // 시간 포맷
            @"\{yyyyMMdd([-+]\d+[hd])?\}", @"\{HHmmss([-+]\d+[hd])?\}",           // 조합 포맷
            @"\{yyyy-MM-dd([-+]\d+[hd])?\}", @"\{HH:mm:ss([-+]\d+[hd])?\}",       // 구분자 포함 포맷
            @"\{utc:yyyy([-+]\d+[hd])?\}", @"\{utc:MM([-+]\d+[hd])?\}", @"\{utc:dd([-+]\d+[hd])?\}", // UTC 날짜 포맷
            @"\{utc:HH([-+]\d+[hd])?\}", @"\{utc:mm([-+]\d+[hd])?\}", @"\{utc:ss([-+]\d+[hd])?\}"    // UTC 시간 포맷

        };

        // 시간 및 날짜 오프셋을 적용
        private static DateTime ApplyOffset(DateTime dateTime, string offsetStr)
        {
            if (string.IsNullOrEmpty(offsetStr)) return dateTime;

            // 오프셋 파싱 (예: "-1h", "+2h", "-1d", "+2d")
            var match = Regex.Match(offsetStr, @"([-+])(\d+)([hd])");
            if (!match.Success) return dateTime;

            int value = int.Parse(match.Groups[2].Value);
            if (match.Groups[1].Value == "-")
                value = -value;

            string unit = match.Groups[3].Value;

            if (unit == "h")
                return dateTime.AddHours(value);
            else if (unit == "d")
                return dateTime.AddDays(value);

            return dateTime;
        }

        // URL에서 포맷 문자열을 실제 날짜/시간으로 대체
        public static string FormatUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;

            DateTime now = DateTime.Now;
            DateTime utcNow = DateTime.UtcNow;

            string result = url;

            // 포맷 패턴과 매칭되는 모든 문자열 찾기
            var matches = Regex.Matches(result, @"\{[^}]+\}");
            foreach (Match match in matches)
            {
                string format = match.Value;
                string replacement = "";

                bool isDateTimeFormat = false;

                // 날짜/시간 포맷인지 확인
                foreach (string validFormat in ValidFormats)
                {
                    if (Regex.IsMatch(format, "^" + validFormat + "$"))
                    {
                        isDateTimeFormat = true;
                        break;
                    }
                }
                if (isDateTimeFormat)
                {
                    // 날짜/시간
                    string offsetStr = "";
                    var offsetMatch = Regex.Match(format, @"([-+]\d+[hd])}$");
                    if (offsetMatch.Success)
                    {
                        offsetStr = offsetMatch.Groups[1].Value;
                        format = format.Substring(0, format.Length - offsetMatch.Groups[1].Value.Length - 1) + "}";
                    }

                    // 기준 시간 결정
                    DateTime baseTime = format.StartsWith("{utc:") ? utcNow : now;

                    // 오프셋 적용
                    if (!string.IsNullOrEmpty(offsetStr))
                    {
                        baseTime = ApplyOffset(baseTime, offsetStr);
                    }

                    // 포맷에 따라 날짜/시간 문자열 생성
                    switch (format)
                    {
                        case "{yyyy}":
                        case "{utc:yyyy}":
                            replacement = baseTime.ToString("yyyy");
                            break;
                        case "{MM}":
                        case "{utc:MM}":
                            replacement = baseTime.ToString("MM");
                            break;
                        case "{dd}":
                        case "{utc:dd}":
                            replacement = baseTime.ToString("dd");
                            break;
                        case "{HH}":
                        case "{utc:HH}":
                            replacement = baseTime.ToString("HH");
                            break;
                        case "{mm}":
                        case "{utc:mm}":
                            replacement = baseTime.ToString("mm");
                            break;
                        case "{ss}":
                        case "{utc:ss}":
                            replacement = baseTime.ToString("ss");
                            break;
                        case "{yyyyMMdd}":
                            replacement = baseTime.ToString("yyyyMMdd");
                            break;
                        case "{HHmmss}":
                            replacement = baseTime.ToString("HHmmss");
                            break;
                        case "{yyyy-MM-dd}":
                            replacement = baseTime.ToString("yyyy-MM-dd");
                            break;
                        case "{HH:mm:ss}":
                            replacement = baseTime.ToString("HH:mm:ss");
                            break;
                    }

                    if (!string.IsNullOrEmpty(replacement))
                    {
                        result = result.Replace(match.Value, replacement);
                    }
                }
                else
                {
                    // 날짜/시간 포맷이 아닌 경우 자동으로 인코딩
                    string valueToEncode = format.Substring(1, format.Length - 2); // { } 제거
                    replacement = Uri.EscapeDataString(valueToEncode);
                }
                if (!string.IsNullOrEmpty(replacement))
                {
                    result = result.Replace(match.Value, replacement);
                }
            }

                return result;
        }
        // URL에서 사용된 포맷 문자열이 유효한지 검사
        public static bool ValidateUrlFormat(string url, out string invalidFormat)
        {
            invalidFormat = string.Empty;
            if (string.IsNullOrEmpty(url)) return true;

            // 중괄호로 둘러싸인 모든 문자열 찾기
            var matches = Regex.Matches(url, @"\{[^}]+\}");
            foreach (Match match in matches)
            {
                // 날짜/시간 포맷인지 확인
                bool isDateTimeFormat = false;
                foreach (string formatPattern in ValidFormats)
                {
                    if (Regex.IsMatch(match.Value, "^" + formatPattern + "$"))
                    {
                        isDateTimeFormat = true;
                        break;
                    }
                }

                // 날짜/시간 포맷이 아니면 일반 텍스트로 간주
                if (!isDateTimeFormat)
                {
                    // 중괄호 안의 텍스트가 비어있는지 확인
                    string innerText = match.Value.Substring(1, match.Value.Length - 2);
                    if (string.IsNullOrWhiteSpace(innerText))
                    {
                        invalidFormat = match.Value;
                        return false;
                    }
                    // 중첩된 중괄호가 있는지 확인
                    if (innerText.Contains("{") || innerText.Contains("}"))
                    {
                        invalidFormat = match.Value;
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
