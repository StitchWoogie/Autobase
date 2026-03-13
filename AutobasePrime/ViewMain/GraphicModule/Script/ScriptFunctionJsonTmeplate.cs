using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace GraphicModule
{
    public class ScriptFunctionJsonTemplate
    {
        /// <summary>
        /// JSON Template 관련 클래스
        /// 1. JSON Template을 스튜디오에서 미리 작성하여 스크립트 상에서 Load하여 사용.
        ///   1-1. Template은 JSON 문법에 맞추어 작성하여야 하며, #xxx# 형태의 문자열로 작성하여 PlaceHolder 작성.($태그멤버 사용가능)
        ///   1-2. PlaceHolder는 해당 PlaceHolder와 변경할 값을 template 이름으로 저장하고 마지막에 Done 실행 시 최종값을 적용.
        ///   1-3. 변경되지 않은 PlaceHolder는 Null로 처리.
        ///   1-4. 태그값은 현재값을 적용하기 위해 JSON문자열에 SetTag 하여 변경된 JSON 문자열 반환.
        ///  2. Template 함수 실행 시 오류가 발생하면 "" 또는 0 값이 아닌 원본 템플릿 또는 정상적으로 변경된 최종 템플릿 반환.
        /// </summary>
        public class JsonTemplate
        {
            public string TemplateId { get; set; }
            public string TemplateContent { get; set; }

            public JsonTemplate(string id, string content)
            {
                TemplateId = id;
                TemplateContent = content;
            }
        }

        public class JsonTemplateManager
        {
            public static void SaveTemplate(string templateId, string jsonContent)
            {
                string fullPath = Path.Combine(TotalConfig.sDirWorkProject, "JSON");
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                string fileName = Path.Combine(fullPath, templateId + ".json");
                File.WriteAllText(fileName, jsonContent, Encoding.UTF8);
            }

            public static string LoadTemplate(string templateId)
            {
                string fileName = MakeFilePath.Project("JSON", templateId + ".json");
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException("Template File not found.");
                }

                return File.ReadAllText(fileName, Encoding.UTF8);
            }
        }

        // 플레이스홀더 값을 저장할 클래스
        private class PlaceholderValue
        {
            private string placeholder;
            private string value;
            private string dataType;

            public string Placeholder
            {
                get { return placeholder; }
                set { placeholder = value; }
            }

            public string Value
            {
                get { return value; }
                set { this.value = value; }
            }

            public string DataType
            {
                get { return dataType; }
                set { dataType = value; }
            }
        }

        private class TemplateManager
        {
            [ThreadStatic]
            private Dictionary<string, string> templates;  // templateId별 원본 템플릿 저장

            private Dictionary<string, List<PlaceholderValue>> templatePlaceholders;

            public TemplateManager()
            {
                this.templates = new Dictionary<string, string>();
                this.templatePlaceholders = new Dictionary<string, List<PlaceholderValue>>();
            }

            public void AddTemplate(string templateId, string template)
            {
                if (string.IsNullOrEmpty(templateId))
                {
                    throw new ArgumentException("Template ID is empty.");
                }

                // 기존 템플릿이 있다면 교체하고 관련 플레이스홀더도 초기화
                this.templates[templateId] = template;
                if (this.templatePlaceholders.ContainsKey(templateId))
                {
                    this.templatePlaceholders[templateId].Clear();
                }
            }

            public bool StorePlaceholder(string templateId, string placeholder, string value, string dataType)
            {
                try
                {
                    // 해당 템플릿이 존재하는지 확인
                    if (!this.templates.ContainsKey(templateId))
                    {
                        return false;
                    }

                    if (!this.templatePlaceholders.ContainsKey(templateId))
                    {
                        this.templatePlaceholders[templateId] = new List<PlaceholderValue>();
                    }

                    // 기존 플레이스홀더가 있는지 확인
                    PlaceholderValue existing = null;
                    foreach (PlaceholderValue pv in this.templatePlaceholders[templateId])
                    {
                        if (pv.Placeholder == placeholder)
                        {
                            existing = pv;
                            break;
                        }
                    }

                    // 있으면 값 업데이트, 없으면 새로 추가
                    if (existing != null)
                    {
                        existing.Value = value;
                        existing.DataType = dataType;
                    }
                    else
                    {
                        PlaceholderValue newValue = new PlaceholderValue();
                        newValue.Placeholder = placeholder;
                        newValue.Value = value;
                        newValue.DataType = dataType;
                        this.templatePlaceholders[templateId].Add(newValue);
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public string ApplyPlaceholders(string templateId)
            {
                try
                {
                    // 템플릿 존재 확인
                    if (!this.templates.ContainsKey(templateId))
                    {
                        throw new Exception("Template not found: " + templateId);
                    }

                    string result = this.templates[templateId];

                    // 해당 템플릿의 플레이스홀더가 없으면 원본 반환
                    if (!this.templatePlaceholders.ContainsKey(templateId))
                    {
                        return result;
                    }

                    // 저장된 플레이스홀더 값들 적용
                    foreach (PlaceholderValue pv in this.templatePlaceholders[templateId])
                    {
                        try
                        {
                            string quotedPlaceholder = String.Format("\"{0}\"", pv.Placeholder);
                            string replacementValue;

                            if (pv.DataType == "int" || pv.DataType == "long" || pv.DataType == "float" ||
                                pv.DataType == "double" || pv.DataType == "decimal" ||
                                pv.DataType == "bool" || pv.DataType == "null")
                            {
                                replacementValue = pv.Value;
                            }
                            else
                            {
                                replacementValue = String.Format("\"{0}\"", pv.Value);
                            }
                            result = result.Replace(quotedPlaceholder, replacementValue);

                            // 따옴표 없는 버전도 처리 (배열 내부 등)
                            if (pv.DataType == "string")
                            {
                                result = result.Replace(pv.Placeholder, pv.Value);
                            }
                            else
                            {
                                result = result.Replace(pv.Placeholder, replacementValue);
                            }
                        }
                        catch (Exception)
                        {
                            // 개별 플레이스홀더 처리 실패 시 다음으로 계속 진행
                            continue;
                        }
                    }

                    return result;
                }
                catch (Exception)
                {
                    return this.templates[templateId]; // 오류 시 원본 반환
                }
            }

            public void RemoveTemplate(string templateId)
            {
                if (this.templates.ContainsKey(templateId))
                {
                    this.templates.Remove(templateId);
                }
                if (this.templatePlaceholders.ContainsKey(templateId))
                {
                    this.templatePlaceholders.Remove(templateId);
                }
            }
        }

        /// <summary>
        /// JSON 템플릿의 플레이스홀더 값을 포맷팅합니다.
        /// </summary>
        private static string FormatTemplateValue(object value, string dataType)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return "null";
            }

            try
            {
                string strValue = value.ToString().Trim();

                if (String.IsNullOrEmpty(strValue))
                {
                    return "\"\"";
                }

                // datetime format 분리 처리
                string format = string.Empty;
                string baseDataType = dataType;
                if (dataType.StartsWith("datetime:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = dataType.Split(new char[] { ':' }, 2);
                    if (parts.Length > 1)
                    {
                        baseDataType = parts[0];
                        format = parts[1];
                    }
                }

                string dataTypeLower = baseDataType.ToLower();

                switch (dataTypeLower)
                {
                    case "null":
                        return "null";

                    case "string":
                    case "object":
                        return strValue;  // JsonConvert 사용하지 않음

                    case "int":
                        int intValue;
                        if (int.TryParse(strValue, out intValue))
                            return intValue.ToString();
                        return "0";

                    case "long":
                        long longValue;
                        if (long.TryParse(strValue, out longValue))
                            return longValue.ToString();
                        return "0";

                    case "float":
                        float floatValue;
                        if (float.TryParse(strValue, out floatValue))
                            return floatValue.ToString();
                        return "0";

                    case "double":
                        double doubleValue;
                        if (double.TryParse(strValue, out doubleValue))
                            return doubleValue.ToString();
                        return "0";

                    case "decimal":
                        decimal decimalValue;
                        if (decimal.TryParse(strValue, out decimalValue))
                            return decimalValue.ToString();
                        return "0";

                    case "bool":
                        if (strValue == "1") strValue = "true";
                        bool boolValue;
                        if (bool.TryParse(strValue, out boolValue))
                            return boolValue.ToString().ToLower();
                        return "false";

                    case "datetime":
                        DateTime dateValue;
                        if (DateTime.TryParse(strValue, out dateValue))
                        {
                            // 기본 형식
                            string defaultFormat = "yyyy-MM-dd HH:mm:ss";

                            // format이 지정된 경우 처리 (예: "datetime:yyyy-MM-dd" 형식)
                            if (!string.IsNullOrEmpty(format))
                            {
                                try
                                {
                                    return dateValue.ToString(format);
                                }
                                catch
                                {
                                    // 잘못된 format이 입력된 경우 기본 형식으로 fallback
                                    return dateValue.ToString(defaultFormat);
                                }

                            }
                            return dateValue.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        return "";

                    default:
                        return strValue;  // JsonConvert 사용하지 않음
                }
            }
            catch
            {
                string dataTypeLower = dataType.ToLower();
                if (dataTypeLower == "null")
                    return "null";

                if (dataTypeLower == "int" ||
                     dataTypeLower == "long" ||
                     dataTypeLower == "float" ||
                     dataTypeLower == "double" ||
                     dataTypeLower == "decimal")
                {
                    return "0";
                }
                else if (dataType.ToLower() == "bool")
                {
                    return "false";
                }
                else
                {
                    return "";
                }
            }
        }


        private static TemplateManager templateManager = new TemplateManager();

        public static int Run_JsonLoadTemplate(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = null;
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250624 PSU

                string templateId = args[0].ToString();
                if (string.IsNullOrEmpty(templateId))
                {
                    string msg = Tools.IsLangKorean() ?
                        "템플릿 ID가 비어있습니다." :
                        "Template ID is empty.";
                    throw new ArgumentException(msg);
                }

                string templateContent = JsonTemplateManager.LoadTemplate(templateId);
                templateManager.AddTemplate(templateId, templateContent);

                val = templateContent;
                return 1;
            }
            catch (Exception ex)
            {
                val = "";
                scriptClass.ErrorMessage(String.Format("@JsonLoadTemplate Error\n{0}", ex.Message));
                return -1; //20250624 PSU 0 -> -1
            }
        }

        public static int Run_JsonReplacePlaceholder(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string templateId = args[0].ToString();
                string placeholder = args[1].ToString();
                object value = args[2];
                string dataType = (args.Length > 3) ? args[3].ToString() : "string";

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250624 PSU

                // templateId 확인
                if (string.IsNullOrEmpty(templateId))
                {
                    throw new Exception(Tools.IsLangKorean() ?
                        "템플릿 ID가 비어있습니다." :
                        "Template ID is empty.");
                }

                // placeholder 확인
                if (string.IsNullOrEmpty(placeholder))
                {
                    throw new Exception(Tools.IsLangKorean() ?
                        "플레이스홀더가 비어있습니다." :
                        "Placeholder is empty.");
                }

                string valueStr = FormatTemplateValue(value, dataType);

                // 플레이스홀더 저장
                if (templateManager.StorePlaceholder(templateId, placeholder, valueStr, dataType))
                {
                    val = 1;
                    return 1;
                }

                throw new Exception(Tools.IsLangKorean() ?
                    String.Format("템플릿 '{0}'에 플레이스홀더를 저장하는데 실패했습니다.", templateId) :
                    String.Format("Failed to store placeholder for template '{0}'.", templateId));
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@JsonTemplateReplacePlaceholder Error\n{0}", ex.Message));
                return -1; //20250624 PSU 0 -> -1
            }
        }

        public static int Run_JsonReplaceDone(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            string result = string.Empty;
            try
            {
                string templateId = args[0].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                // templateId 확인
                if (string.IsNullOrEmpty(templateId))
                {
                    throw new Exception(Tools.IsLangKorean() ?
                        "템플릿 ID가 비어있습니다." :
                        "Template ID is empty.");
                }

                // 플레이스홀더 적용
                result = templateManager.ApplyPlaceholders(templateId);

                try
                {
                    // JSON 유효성 검사
                    JToken.Parse(result);
                }
                catch (Exception)
                {
                    throw new Exception(Tools.IsLangKorean() ?
                        "유효하지 않은 JSON 형식입니다." :
                        "Invalid JSON format.");
                }

                //args[0] = result;
                val = result;
                return 1;
            }
            catch (Exception ex)
            {
                val = result; //실패시 원본 반환.
                scriptClass.ErrorMessage(String.Format("@JsonTemplateReplaceDone Error\n{0}", ex.Message));
                return -1; //20250624 PSU 0 -> -1
            }
        }


        public static int Run_JsonReplaceMissingPlaceholders(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string jsonContent = args[0].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (string.IsNullOrEmpty(jsonContent))
                {
                    string msg = Tools.IsLangKorean() ?
                        "JSON 내용이 비어있습니다." :
                        "JSON content is empty.";
                    throw new ArgumentException(msg);
                }

                // 정규식으로 "#문자#" 패턴 찾기
                string pattern = "\"#[^#]+#\"";
                Regex regex = new Regex(pattern);

                // 모든 미치환 플레이스홀더를 "none"으로 변경
                string result = regex.Replace(jsonContent, "null");

                // 배열 내부의 플레이스홀더도 처리 ("#value#" → none)
                pattern = "#[^#]+#";
                regex = new Regex(pattern);
                result = regex.Replace(result, "none");

                args[0] = result; //성공 시 null처리된 문자열 반환

                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                //실패 시 변경없음.
                scriptClass.ErrorMessage(String.Format("@JsonTemplateReplaceMissing Error\n{0}", ex.Message));
                return -1; //20250624 PSU 0 -> -1
            }
        }

        public static int Run_JsonTemplateTagSet(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string jsonContent = args[0].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250624 PSU

                if (string.IsNullOrEmpty(jsonContent))
                {
                    string msg = Tools.IsLangKorean() ?
                        "JSON 내용이 비어있습니다." :
                        "JSON content is empty.";
                    throw new ArgumentException(msg);
                }

                // #$로 시작하고 #로 끝나는 패턴 찾기
                string pattern = "#\\$[^#]+#";
                Regex regex = new Regex(pattern);

                // 모든 매치 찾기
                MatchCollection matches = regex.Matches(jsonContent);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string placeholder = match.Value;
                    // #$ 와 마지막 #를 제거하여 실제 태그 이름 추출
                    string tagName = placeholder.Substring(2, placeholder.Length - 3);

                    // GetTagValue 호출을 위한 준비
                    object tagValue = null;
                    object[] tagArgs = new object[] { tagName };

                    // GetTagValue 호출
                    int result = ScriptFunctionGet.Run_GetTagValue(scriptClass, "GetTagValue", out tagValue, tagArgs);

                    if (result == 1 && tagValue != null)
                    {
                        // 태그값이 숫자인지 확인
                        double numericValue;
                        bool isNumeric = double.TryParse(tagValue.ToString(), out numericValue);

                        if (isNumeric)
                        {
                            // 숫자값인 경우 따옴표 없이 치환
                            jsonContent = jsonContent.Replace("\"" + placeholder + "\"", tagValue.ToString());
                            // 배열 내부의 경우도 처리
                            jsonContent = jsonContent.Replace(placeholder, tagValue.ToString());
                        }
                        else
                        {
                            // 문자열인 경우 JSON 문자열로 적절히 이스케이프
                            string escapedValue = JsonConvert.ToString(tagValue.ToString());
                            // 이미 따옴표로 둘러싸인 경우 처리
                            jsonContent = jsonContent.Replace("\"" + placeholder + "\"", escapedValue);
                            // 배열 내부의 경우도 처리
                            jsonContent = jsonContent.Replace(placeholder, escapedValue);
                        }
                    }
                }

                // 결과 JSON의 유효성 검증
                try
                {
                    JToken.Parse(jsonContent);
                    args[0] = jsonContent;
                    val = 1;
                }
                catch (JsonReaderException)
                {
                    string msg = Tools.IsLangKorean() ?
                        "유효하지 않은 JSON 형식입니다." :
                        "Invalid JSON format.";
                    throw new JsonReaderException(msg);
                }

                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@JsonTemplateTagSet Error\n{0}", ex.Message));
                return -1; //20250624 PSU 0 -> -1
            }
        }

        //Json 클래스에서 prepare.
        //public static void PrepareMethod(ScriptExternalRun prepare)
        //{
        //    string prename = "JsonTemplate";

        //    prepare.AddMethod(prename, "JsonTemplateLoad", "string", new ScriptExternalRun.DeleMethod(Run_JsonLoadTemplate), "in:string:templateId");
        //    //JSON 템플릿 읽어와서 문자열 반환
        //    prepare.AddMethod(prename, "JsonTemplateReplacePlaceholder", "int", new ScriptExternalRun.DeleMethod(Run_JsonReplacePlaceholder), "in:string:templateId",
        //      "in:string:placeholder", "in:object:value", "params:string:dataType");
        //    //JSON 템플릿에 변경할 플레이스홀더 값을 리스트에 등록
        //    prepare.AddMethod(prename, "JsonTemplateReplaceDone", "string", new ScriptExternalRun.DeleMethod(Run_JsonReplaceDone), "in:string:templateId");
        //    //플레이스홀더 변경 리스트를 템플렛에 적용
        //    prepare.AddMethod(prename, "JsonTemplateReplaceMissing", "int", new ScriptExternalRun.DeleMethod(Run_JsonReplaceMissingPlaceholders), "ref:string:jsonContent");
        //    //JSON 템플릿에 플레이스홀더 남은 부분 null로 변경
        //    prepare.AddMethod(prename, "JsonTemplateTagSet", "int", new ScriptExternalRun.DeleMethod(Run_JsonTemplateTagSet), "ref:string:jsonContent");
        //    //JSON 템플릿에 태그값 모두 적용

        //}

    }
}
