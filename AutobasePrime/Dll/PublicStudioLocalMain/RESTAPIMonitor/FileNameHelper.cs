using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutobaseRESTAPIMonitor
{
    public static class FileNameHelper
    {
        public static string GetSafeFileName(string fileName)
        {
            // 파일 이름으로 사용할 수 없는 문자들을 제거하거나 대체
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            string safe = fileName;

            foreach (char c in invalid)
            {
                safe = safe.Replace(c, '_');
            }

            // URL에서 자주 사용되는 특수문자들도 처리
             safe = fileName.Replace("\\", "_")
                         .Replace("/", "_")
                         .Replace(":", "_")
                         .Replace("*", "_")
                         .Replace("?", "_")
                         .Replace("\"", "_")
                         .Replace("<", "_")
                         .Replace(">", "_")
                         .Replace("|", "_");

            // 파일명 길이 제한 (Windows의 경우 260자 제한이 있음)
            if (safe.Length > 200)  // 확장자와 경로를 고려하여 여유있게 설정
            {
                safe = safe.Substring(0, 200);
            }

            return safe;
        }
    }
}
