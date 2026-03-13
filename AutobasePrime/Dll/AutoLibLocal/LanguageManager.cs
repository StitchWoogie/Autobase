using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetTools;

namespace AutoLibLocal
{
    /// <summary>
    /// SCADA 다국어 지원 관리자
    /// @{key} 형식으로 다국어 키 지정
    /// </summary>
    public class LanguageManager
    {
        private static LanguageManager _instance;
        private static readonly object _lock = new object();

        private Dictionary<string, Dictionary<string, string>> _languageTable;
        private string _currentLanguage;
        private readonly string _dataFilePath;

        // @{key} 형식을 찾는 정규식
        private static readonly Regex KeyPattern = new Regex(@"@\{([^}]+)\}", RegexOptions.Compiled);

        // 언어 변경 이벤트 델리게이트
        public delegate void OnLanguageChanged(string newLanguageCode);

        // 언어 변경 이벤트
        public static event OnLanguageChanged EventLanguageChanged;

        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LanguageManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        private LanguageManager()
        {
            _languageTable = new Dictionary<string, Dictionary<string, string>>();
            // .dat 대신 .csv로 변경
            _dataFilePath = Path.Combine(TotalConfig.sDirWorkProject, "Language", "LanguageData.csv");

            if (Tools.IsLangKorean()) _currentLanguage = "ko";
            else if (Tools.IsLangJapanese()) _currentLanguage = "ja";
            else if (Tools.IsLangChinese()) _currentLanguage = "zh-CN";
            else if (Tools.IsLangVietnamese()) _currentLanguage = "vt";
            else _currentLanguage = "en";

            LoadLanguageData();
        }

        /// <summary>
        /// 언어 코드 정규화 (ko-KR -> ko, en-US -> en 등)
        /// </summary>
        private string NormalizeLanguageCode(string langCode)
        {
            if (string.IsNullOrWhiteSpace(langCode))
                return string.Empty;

            langCode = langCode.Trim();

            // 이미 정규화된 형식이면 그대로 반환
            switch (langCode.ToLower())
            {
                case "ko":
                case "ko-kr":
                    return "ko";

                case "ja":
                case "ja-jp":
                case "jp":
                    return "ja";

                case "zh":
                case "zh-cn":
                case "zh-chs":
                    return "zh-CN";

                case "en":
                case "en-us":
                    return "en";

                case "vi":
                case "vt":
                case "vi-vn":
                    return "vi";

                case "ru":
                case "ru-ru":
                    return "ru";

                case "value": // 기본값 컬럼
                    return "value";

                default:
                    return langCode;
            }
        }

        /// <summary>
        /// 언어 데이터 로드
        /// </summary>
        private void LoadLanguageData()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return;
                }

                // UTF-8 BOM으로 읽기
                string[] lines = File.ReadAllLines(_dataFilePath, Encoding.UTF8);

                if (lines.Length < 2) // 헤더 + 최소 1개 데이터 행
                {
                    return;
                }

                // 헤더 파싱 (첫 번째 줄)
                string[] headers = ParseCSVLine(lines[0]);

                if (headers.Length < 2 || headers[0].ToLower() != "name")
                {
                    throw new Exception("CSV 형식 오류: 첫 번째 열은 'name'이어야 합니다.");
                }

                // 언어 테이블 초기화
                _languageTable.Clear();

                // 헤더에서 언어 코드 추출 (name, value 제외한 나머지)
                List<string> languageCodes = new List<string>();
                for (int i = 1; i < headers.Length; i++)
                {
                    string originalLangCode = headers[i].Trim();
                    if (!string.IsNullOrWhiteSpace(originalLangCode))
                    {
                        // 정규화된 언어 코드 사용
                        string normalizedLangCode = NormalizeLanguageCode(originalLangCode);
                        languageCodes.Add(normalizedLangCode);

                        // 기존에 없으면 새로 생성
                        if (!_languageTable.ContainsKey(normalizedLangCode))
                        {
                            _languageTable[normalizedLangCode] = new Dictionary<string, string>();
                        }
                    }
                }

                // 데이터 행 파싱 (두 번째 줄부터)
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] values = ParseCSVLine(lines[i]);

                    if (values.Length == 0 || string.IsNullOrWhiteSpace(values[0]))
                        continue;

                    string key = values[0].Trim();

                    // 각 언어별 값 저장
                    for (int j = 0; j < languageCodes.Count && j + 1 < values.Length; j++)
                    {
                        string langCode = languageCodes[j];
                        string value = values[j + 1]; // name 다음부터가 언어 값들

                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            _languageTable[langCode][key] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"언어 데이터 로드 실패: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// CSV 라인 파싱 (큰따옴표 처리 포함)
        /// </summary>
        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            StringBuilder current = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // 연속된 따옴표는 하나의 따옴표로
                        current.Append('"');
                        i++; // 다음 따옴표 건너뛰기
                    }
                    else
                    {
                        // 따옴표 시작/끝
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // 쉼표이고 따옴표 밖이면 필드 구분
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            // 마지막 필드 추가
            result.Add(current.ToString());

            return result.ToArray();
        }

        /// <summary>
        /// 기본 언어 데이터 초기화
        /// </summary>
        private void InitializeDefaultLanguageData()
        {
            _languageTable.Clear();

            // 한국어
            _languageTable["ko"] = new Dictionary<string, string>
            {
                { "app.title", "SCADA 시스템" },
                { "menu.file", "파일" },
                { "menu.view", "보기" },
                { "menu.tools", "도구" },
                { "menu.help", "도움말" },
                { "btn.start", "시작" },
                { "btn.stop", "정지" },
                { "btn.reset", "리셋" },
                { "status.ready", "준비" },
                { "status.running", "실행 중" },
                { "status.stopped", "정지됨" }
            };

            // 영어
            _languageTable["en"] = new Dictionary<string, string>
            {
                { "app.title", "SCADA System" },
                { "menu.file", "File" },
                { "menu.view", "View" },
                { "menu.tools", "Tools" },
                { "menu.help", "Help" },
                { "btn.start", "Start" },
                { "btn.stop", "Stop" },
                { "btn.reset", "Reset" },
                { "status.ready", "Ready" },
                { "status.running", "Running" },
                { "status.stopped", "Stopped" }
            };

            // 일본어
            _languageTable["ja"] = new Dictionary<string, string>
            {
                { "app.title", "SCADAシステム" },
                { "menu.file", "ファイル" },
                { "menu.view", "表示" },
                { "menu.tools", "ツール" },
                { "menu.help", "ヘルプ" },
                { "btn.start", "開始" },
                { "btn.stop", "停止" },
                { "btn.reset", "リセット" },
                { "status.ready", "準備完了" },
                { "status.running", "実行中" },
                { "status.stopped", "停止" }
            };
        }

        /// <summary>
        /// 언어 데이터 저장 (CSV 형식, UTF-8 BOM)
        /// </summary>
        private void SaveLanguageData()
        {
            try
            {
                string directory = Path.GetDirectoryName(_dataFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // UTF-8 BOM으로 저장 (엑셀 호환)
                using (StreamWriter writer = new StreamWriter(_dataFilePath, false, new UTF8Encoding(true)))
                {
                    // 헤더 작성
                    List<string> headers = new List<string> { "name" };
                    List<string> sortedLangCodes = _languageTable.Keys.OrderBy(k => k).ToList();
                    headers.AddRange(sortedLangCodes);

                    writer.WriteLine(string.Join(",", headers.Select(h => EscapeCSV(h))));

                    // 모든 키 수집 (중복 제거)
                    HashSet<string> allKeys = new HashSet<string>();
                    foreach (var langDict in _languageTable.Values)
                    {
                        foreach (var key in langDict.Keys)
                        {
                            allKeys.Add(key);
                        }
                    }

                    // 키별로 데이터 행 작성
                    foreach (string key in allKeys.OrderBy(k => k))
                    {
                        List<string> rowValues = new List<string> { EscapeCSV(key) };

                        foreach (string langCode in sortedLangCodes)
                        {
                            string value = string.Empty;

                            if (_languageTable[langCode].ContainsKey(key))
                            {
                                value = _languageTable[langCode][key];
                            }

                            rowValues.Add(EscapeCSV(value));
                        }

                        writer.WriteLine(string.Join(",", rowValues));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"언어 데이터 저장 실패: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// CSV 값 이스케이프 처리
        /// </summary>
        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // 쉼표, 큰따옴표, 개행 문자가 있으면 큰따옴표로 감싸기
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                // 큰따옴표는 두 번 써서 이스케이프
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        /// <summary>
        /// 언어 변경 이벤트 발생
        /// </summary>
        /// <param name="newLanguageCode">새로운 언어 코드</param>
        private void RaiseLanguageChangedEvent(string newLanguageCode)
        {
            if (EventLanguageChanged == null) return;

            try
            {
                // 모든 구독자에게 순차적으로 이벤트 전달
                Delegate[] delegateList = EventLanguageChanged.GetInvocationList();
                for (int i = 0; i < delegateList.Length; i++)
                {
                    try
                    {
                        ((OnLanguageChanged)delegateList[i])(newLanguageCode);
                    }
                    catch (Exception ex)
                    {
                        // 개별 이벤트 핸들러 오류는 무시하고 계속 진행
                        System.Diagnostics.Debug.WriteLine($"언어 변경 이벤트 처리 오류: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"언어 변경 이벤트 발생 오류: {ex.Message}");
            }
        }


        /// <summary>
        /// 현재 언어 설정
        /// </summary>
        /// <param name="langCode">언어 코드 (예: ko-KR, en-US, ja-JP)</param>
        public string SetLang(string langCode)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                return "langCode is null or empty.";
            }
            // 언어 코드 정규화
            string normalizedLangCode = NormalizeLanguageCode(langCode);

            // 언어가 실제로 변경되었을 때만 이벤트 발생
            if (_currentLanguage != normalizedLangCode)
            {
                _currentLanguage = normalizedLangCode;

                CultureInfo newCulture = GetCultureInfo(normalizedLangCode);

                // 앱 전역 Culture 설정. (새로운 폼에 적용됨)
                CultureInfo.DefaultThreadCurrentUICulture = newCulture;

                // 언어 변경 이벤트 발생
                RaiseLanguageChangedEvent(langCode);
            }
            return "";
        }
        public CultureInfo GetCultureInfo(string langCode)
        {
            switch (langCode)
            {
                case "ko":
                case "ko-KR":
                    return new CultureInfo("ko-KR");
                case "ja":
                case "ja-JP":
                case "jp":
                    return new CultureInfo("ja-JP");
                case "zh-CHS":
                case "zh":
                case "zh-CN":
                    return new CultureInfo("zh-CN");
                case "en":
                case "en-US":
                    return new CultureInfo("en-US");
                case "vi":
                case "vt":
                case "vi-VN":
                    return new CultureInfo("vi-VN");
                case "ru":
                case "ru-RU":
                    return new CultureInfo("ru-RU");
                default:
                    return new CultureInfo("en-US");
            }
        }


        /// <summary>
        /// 현재 언어 코드 반환
        /// </summary>
        /// <returns>현재 설정된 언어 코드</returns>
        public string GetLang()
        {
            return _currentLanguage;
        }

        /// <summary>
        /// 키에 해당하는 다국어 문자열 반환
        /// </summary>
        /// <param name="key">언어 키</param>
        /// <returns>다국어 문자열</returns>
        public string Lang(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            // 1. 현재 언어에서 키 검색
            if (_languageTable.ContainsKey(_currentLanguage) &&
                _languageTable[_currentLanguage].ContainsKey(key))
            {
                string value = _languageTable[_currentLanguage][key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            // 2. 현재 언어에 값이 없으면 "value" 컬럼(기본값)에서 검색
            // FormConfigGlobalization에서 저장한 기본 value 사용
            if (_languageTable.ContainsKey("value") &&
                _languageTable["value"].ContainsKey(key))
            {
                string value = _languageTable["value"][key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            // 3. value도 없으면 키 자체를 반환
            return $"{key}";

        }

        /// <summary>
        /// 텍스트에서 @{key} 형식을 파싱하여 다국어 문자열로 변환
        /// </summary>
        /// <param name="text">원본 텍스트</param>
        /// <returns>변환된 텍스트</returns>
        public string ParseText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            // @{key} 패턴을 찾아서 치환
            return KeyPattern.Replace(text, match =>
            {
                string key = match.Groups[1].Value;
                return Lang(key);
            });
        }

        /// <summary>
        /// 컨트롤의 Text 속성을 다국어로 변환
        /// </summary>
        /// <param name="control">변환할 컨트롤</param>
        public void ApplyLanguage(Control control)
        {
            if (control == null)
                return;

            // 현재 컨트롤의 Text 변환
            if (!string.IsNullOrWhiteSpace(control.Text))
            {
                control.Text = ParseText(control.Text);
            }

            // 특정 컨트롤 타입별 처리
            if (control is Form form)
            {
                form.Text = ParseText(form.Text);
            }
            else if (control is ToolStrip toolStrip)
            {
                ApplyLanguageToToolStrip(toolStrip);
            }
            else if (control is MenuStrip menuStrip)
            {
                ApplyLanguageToToolStrip(menuStrip);
            }
            else if (control is StatusStrip statusStrip)
            {
                ApplyLanguageToToolStrip(statusStrip);
            }

            // 자식 컨트롤들에 재귀적으로 적용
            foreach (Control child in control.Controls)
            {
                ApplyLanguage(child);
            }
        }

        /// <summary>
        /// ToolStrip 계열 컨트롤의 아이템들에 다국어 적용
        /// </summary>
        /// <param name="toolStrip">ToolStrip 컨트롤</param>
        private void ApplyLanguageToToolStrip(ToolStrip toolStrip)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Text))
                {
                    item.Text = ParseText(item.Text);
                }

                // ToolTip도 변환
                if (!string.IsNullOrWhiteSpace(item.ToolTipText))
                {
                    item.ToolTipText = ParseText(item.ToolTipText);
                }

                // DropDown 아이템 처리
                if (item is ToolStripDropDownItem dropDownItem)
                {
                    ApplyLanguageToDropDownItems(dropDownItem);
                }
            }
        }

        /// <summary>
        /// DropDown 아이템들에 재귀적으로 다국어 적용
        /// </summary>
        /// <param name="dropDownItem">DropDown 아이템</param>
        private void ApplyLanguageToDropDownItems(ToolStripDropDownItem dropDownItem)
        {
            foreach (ToolStripItem item in dropDownItem.DropDownItems)
            {
                if (!string.IsNullOrWhiteSpace(item.Text))
                {
                    item.Text = ParseText(item.Text);
                }

                if (!string.IsNullOrWhiteSpace(item.ToolTipText))
                {
                    item.ToolTipText = ParseText(item.ToolTipText);
                }

                if (item is ToolStripDropDownItem subDropDownItem)
                {
                    ApplyLanguageToDropDownItems(subDropDownItem);
                }
            }
        }

        /// <summary>
        /// 사용 가능한 언어 목록 반환
        /// </summary>
        /// <returns>언어 코드 리스트</returns>
        public List<string> GetAvailableLanguages()
        {
            return _languageTable.Keys.ToList();
        }

        /// <summary>
        /// 특정 언어에 키-값 추가 또는 업데이트
        /// </summary>
        /// <param name="langCode">언어 코드</param>
        /// <param name="key">키</param>
        /// <param name="value">값</param>
        public void AddOrUpdateKey(string langCode, string key, string value)
        {
            if (!_languageTable.ContainsKey(langCode))
            {
                _languageTable[langCode] = new Dictionary<string, string>();
            }

            _languageTable[langCode][key] = value;
            SaveLanguageData();
        }

        /// <summary>
        /// 언어 데이터 새로고침 (파일에서 다시 로드)
        /// </summary>
        public void Refresh()
        {
            LoadLanguageData();

            // 새로고침 후 현재 언어로 이벤트 발생
            RaiseLanguageChangedEvent(_currentLanguage);
        }
    }


    /// <summary>
    /// 간편 사용을 위한 전역 헬퍼 클래스
    /// </summary>
    public static class Lang
    {
        /// <summary>
        /// 다국어 문자열 가져오기
        /// </summary>
        /// <param name="key">키</param>
        /// <returns>다국어 문자열</returns>
        public static string Get(string key)
        {
            return LanguageManager.Instance.Lang(key);
        }

        /// <summary>
        /// 텍스트 파싱
        /// </summary>
        /// <param name="text">원본 텍스트</param>
        /// <returns>파싱된 텍스트</returns>
        public static string Parse(string text)
        {
            return LanguageManager.Instance.ParseText(text);
        }

        /// <summary>
        /// 현재 언어 설정
        /// </summary>
        /// <param name="langCode">언어 코드</param>
        public static void Set(string langCode)
        {
            LanguageManager.Instance.SetLang(langCode);
        }

        /// <summary>
        /// 현재 언어 가져오기
        /// </summary>
        /// <returns>언어 코드</returns>
        public static string Current
        {
            get { return LanguageManager.Instance.GetLang(); }
        }
    }
}
