using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTools
{
    public static class ExcelPathFinder
    {
        public static List<string> FindExcelExePaths()
        {
            var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 1) App Paths (가장 우선)
            TryAddFromAppPaths(results, @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\excel.exe");

            // 2) Uninstall 정보에서 추정
            TryAddFromUninstall(results, RegistryHive.LocalMachine);
            TryAddFromUninstall(results, RegistryHive.CurrentUser);

            // 3) ClickToRun 구성에서 root\OfficeXX 패턴 탐색
            TryAddFromClickToRun(results);

            // 4) PATH 환경변수에서 excel.exe 후보 (있을 수도 있음)
            TryAddFromEnvironmentPath(results);

            // 최종: 실제 파일 존재하는 것만, 정렬
            var list = new List<string>();
            foreach (var p in results)
            {
                if (File.Exists(p))
                    list.Add(NormalizePath(p));
            }
            list.Sort(StringComparer.OrdinalIgnoreCase);
            return list;
        }

        private static void TryAddFromAppPaths(HashSet<string> results, string subKeyPath)
        {
            // HKLM/HKCU + 32/64 view 모두 조회
            TryAddFromAppPaths(results, RegistryHive.LocalMachine, RegistryView.Registry64, subKeyPath);
            TryAddFromAppPaths(results, RegistryHive.LocalMachine, RegistryView.Registry32, subKeyPath);
            TryAddFromAppPaths(results, RegistryHive.CurrentUser, RegistryView.Registry64, subKeyPath);
            TryAddFromAppPaths(results, RegistryHive.CurrentUser, RegistryView.Registry32, subKeyPath);
        }

        private static void TryAddFromAppPaths(HashSet<string> results, RegistryHive hive, RegistryView view, string subKeyPath)
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(hive, view))
                using (var key = baseKey.OpenSubKey(subKeyPath, false))
                {
                    if (key == null) return;

                    // (Default) 값이 보통 exe 전체 경로
                    var v = key.GetValue(null) as string;
                    AddIfExcelExe(results, v);

                    // Path 값이 따로 있기도 함
                    var path = key.GetValue("Path") as string;
                    if (!string.IsNullOrEmpty(path))
                    {
                        var candidate = CombineSafe(path, "EXCEL.EXE");
                        AddIfExcelExe(results, candidate);
                    }
                }
            }
            catch { /* ignore */ }
        }

        private static void TryAddFromUninstall(HashSet<string> results, RegistryHive hive)
        {
            TryAddFromUninstall(results, hive, RegistryView.Registry64);
            TryAddFromUninstall(results, hive, RegistryView.Registry32);
        }

        private static void TryAddFromUninstall(HashSet<string> results, RegistryHive hive, RegistryView view)
        {
            const string uninstallKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(hive, view))
                using (var uninstall = baseKey.OpenSubKey(uninstallKeyPath, false))
                {
                    if (uninstall == null) return;

                    foreach (var subName in uninstall.GetSubKeyNames())
                    {
                        using (var sub = uninstall.OpenSubKey(subName, false))
                        {
                            if (sub == null) continue;

                            // Office/Excel 관련 엔트리만 가볍게 필터
                            var displayName = (sub.GetValue("DisplayName") as string) ?? "";
                            if (displayName.IndexOf("Microsoft Office", StringComparison.OrdinalIgnoreCase) < 0 &&
                                displayName.IndexOf("Microsoft 365", StringComparison.OrdinalIgnoreCase) < 0 &&
                                displayName.IndexOf("Excel", StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                continue;
                            }

                            // DisplayIcon: "...\EXCEL.EXE",0 같은 형식이 많음
                            var displayIcon = sub.GetValue("DisplayIcon") as string;
                            AddIfExcelExe(results, ExtractExePathFromCommand(displayIcon));

                            // InstallLocation에서 흔한 후보 경로 생성
                            var installLocation = sub.GetValue("InstallLocation") as string;
                            if (!string.IsNullOrEmpty(installLocation))
                            {
                                // Click-to-Run / MSI 모두를 가정해서 몇 가지 패턴 시도
                                // root\OfficeXX, OfficeXX, Office14/15/16 등
                                AddIfExcelExe(results, CombineSafe(installLocation, "EXCEL.EXE"));
                                AddIfExcelExe(results, CombineSafe(installLocation, @"root\Office16\EXCEL.EXE"));
                                AddIfExcelExe(results, CombineSafe(installLocation, @"Office16\EXCEL.EXE"));
                                AddIfExcelExe(results, CombineSafe(installLocation, @"Office15\EXCEL.EXE"));
                                AddIfExcelExe(results, CombineSafe(installLocation, @"Office14\EXCEL.EXE"));
                            }
                        }
                    }
                }
            }
            catch { /* ignore */ }
        }

        private static void TryAddFromClickToRun(HashSet<string> results)
        {
            // 대표 키:
            // HKLM\SOFTWARE\Microsoft\Office\ClickToRun\Configuration
            // 값: InstallationPath (예: C:\Program Files\Microsoft Office\root)
            TryAddFromClickToRun(results, RegistryView.Registry64);
            TryAddFromClickToRun(results, RegistryView.Registry32);
        }

        private static void TryAddFromClickToRun(HashSet<string> results, RegistryView view)
        {
            const string c2rKeyPath = @"SOFTWARE\Microsoft\Office\ClickToRun\Configuration";
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (var key = baseKey.OpenSubKey(c2rKeyPath, false))
                {
                    if (key == null) return;

                    var installationPath = key.GetValue("InstallationPath") as string; // root 까지 주는 경우도, 그 상위인 경우도 있음
                    if (string.IsNullOrEmpty(installationPath)) return;

                    // root/OfficeXX 조합 탐색
                    AddIfExcelExe(results, CombineSafe(installationPath, @"Office16\EXCEL.EXE"));
                    AddIfExcelExe(results, CombineSafe(installationPath, @"Office15\EXCEL.EXE"));
                    AddIfExcelExe(results, CombineSafe(installationPath, @"Office14\EXCEL.EXE"));

                    AddIfExcelExe(results, CombineSafe(installationPath, @"root\Office16\EXCEL.EXE"));
                    AddIfExcelExe(results, CombineSafe(installationPath, @"root\Office15\EXCEL.EXE"));
                    AddIfExcelExe(results, CombineSafe(installationPath, @"root\Office14\EXCEL.EXE"));
                }
            }
            catch { /* ignore */ }
        }

        private static void TryAddFromEnvironmentPath(HashSet<string> results)
        {
            try
            {
                var path = Environment.GetEnvironmentVariable("PATH") ?? "";
                var parts = path.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var dir in parts)
                {
                    var candidate = CombineSafe(dir.Trim(), "EXCEL.EXE");
                    AddIfExcelExe(results, candidate);
                }
            }
            catch { /* ignore */ }
        }

        private static void AddIfExcelExe(HashSet<string> results, string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            path = path.Trim().Trim('"');

            // 간단히 파일명 확인 + 존재 여부는 최종 단계에서 다시 체크
            if (path.EndsWith(@"\EXCEL.EXE", StringComparison.OrdinalIgnoreCase))
                results.Add(NormalizePath(path));
        }

        private static string ExtractExePathFromCommand(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;

            // 예: "C:\...\EXCEL.EXE",0  또는  C:\...\EXCEL.EXE /something
            var s = raw.Trim();

            // 따옴표로 시작하면 첫 따옴표 쌍
            if (s.StartsWith("\"", StringComparison.Ordinal))
            {
                int end = s.IndexOf('"', 1);
                if (end > 1) return s.Substring(1, end - 1);
            }

            // , 또는 공백 앞까지를 경로로 간주
            int cutComma = s.IndexOf(',');
            int cutSpace = s.IndexOf(' ');
            int cut = -1;
            if (cutComma >= 0) cut = cutComma;
            if (cutSpace >= 0) cut = (cut < 0) ? cutSpace : Math.Min(cut, cutSpace);

            return (cut > 0) ? s.Substring(0, cut) : s;
        }

        private static string CombineSafe(string dir, string file)
        {
            if (string.IsNullOrEmpty(dir)) return null;
            try { return Path.Combine(dir.Trim().Trim('"'), file); }
            catch { return null; }
        }

        private static string NormalizePath(string path)
        {
            try
            {
                // GetFullPath로 표준화
                return Path.GetFullPath(path);
            }
            catch
            {
                return path;
            }
        }
    }
}
