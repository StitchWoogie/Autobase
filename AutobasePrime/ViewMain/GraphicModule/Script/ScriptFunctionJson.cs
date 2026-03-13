using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphicModule
{
    public partial class ScriptFunctionJson
    {
        /// <summary>
        /// JSON 문자열을 스크립트 상에서 바로 작성할 수 없어서 JSON 트리 구조를 이용하여 생성
        /// winform 트리노드로 개발 완료 후 JToken 을 사용하면 더 간단하게 구현할 수 있어서 변경.
        /// </summary>
        [ThreadStatic]
        private static Dictionary<string, JToken> jsonObjects;
        //private static Dictionary<string, JToken> jsonObjects = new Dictionary<string, JToken>();
        //private static readonly object counterLock = new object();
        [ThreadStatic]
        private static int objectCounter;
        [ThreadStatic]
        private static int maxObjects;
        [ThreadStatic]
        private static int resetThreshold;

        //JSON 객체 증가 메서드 : JsonStructNew, JsonFromString, JsonGet

        private static string GenerateObjectId()
        {
            if (jsonObjects == null)
            {
                jsonObjects = new Dictionary<string, JToken>();
                maxObjects = 1000;
                resetThreshold = 100000;
            }

            if (jsonObjects.Count > maxObjects)
            {
                jsonObjects.Clear();
                objectCounter = 0;
                string errorMsg = Tools.IsLangKorean() ?
                   String.Format("최대 JSON 객체 수({0})를 초과하여 전체 초기화되었습니다. JsonClear 호출이 누락된 스크립트가 있는지 확인하세요.", maxObjects) :
                   String.Format("Exceeded maximum JSON objects({0}). All objects cleared. Check for missing JsonClear calls in scripts.", maxObjects);
                throw new InvalidOperationException(errorMsg);
            }

            if (objectCounter > resetThreshold)
            {
                objectCounter = 0;
            }

            return String.Format("{0}", objectCounter++);
        }

        // JSON 객체/배열 생성
        public static string JsonStructNew(string type)
        {
            try
            {
                string objectId = String.Format("JsonStruct_{0}", GenerateObjectId());

                if (string.IsNullOrEmpty(type))
                {
                    throw new ArgumentNullException("type",
                        Tools.IsLangKorean() ? "JSON 타입이 null이거나 비어있습니다." : "The JSON type is null or empty.");
                }

                if (type == "{}")
                {
                    jsonObjects[objectId] = new JObject();
                }
                else if (type == "[]")
                {
                    jsonObjects[objectId] = new JArray();
                }
                else
                {
                    throw new ArgumentException(
                        Tools.IsLangKorean() ? "유효하지 않은 JSON 타입입니다. {} 또는 []를 사용하세요." :
                        "Invalid JSON type. Use {} or [].");
                }

                return objectId;
            }
            catch
            {
                return "";
            }
        }

        // 배열에 값 추가
        public static void JsonAppend(string objectId, object value, string dataType)
        {
            if (!jsonObjects.ContainsKey(objectId))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "유효하지 않은 JSON 객체 ID입니다." : "Invalid JSON object ID.");
            }

            JToken arrayToken = jsonObjects[objectId];
            if (!(arrayToken is JArray))
            {
                throw new InvalidOperationException(
                    Tools.IsLangKorean() ? "배열 타입의 JSON 객체에만 Append가 가능합니다." :
                    "Append is only possible for JSON array objects.");
            }

            JArray array = (JArray)arrayToken;

            // 다른 JSON 객체를 참조하는 경우
            if (value is string && jsonObjects.ContainsKey(value.ToString()))
            {
                array.Add(jsonObjects[value.ToString()].DeepClone());
            }
            else
            {
                array.Add(ConvertToJToken(value, dataType));
            }
        }

        // 객체에 키-값 쌍 추가
        public static void JsonSet(string objectId, string key, object value, string dataType)
        {
            if (!jsonObjects.ContainsKey(objectId))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "유효하지 않은 JSON 객체 ID입니다." : "Invalid JSON object ID.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "키가 없습니다." : "Key is missing.");
            }

            JToken objToken = jsonObjects[objectId];
            if (!(objToken is JObject))
            {
                throw new InvalidOperationException(
                    Tools.IsLangKorean() ? "객체 타입의 JSON에만 Set이 가능합니다." :
                    "Set is only possible for JSON object type.");
            }

            JObject obj = (JObject)objToken;

            // 다른 JSON 객체를 참조하는 경우
            if (value is string && jsonObjects.ContainsKey(value.ToString()))
            {
                obj[key] = jsonObjects[value.ToString()].DeepClone();
            }
            else
            {
                obj[key] = ConvertToJToken(value, dataType);
            }
        }

        // 키에 해당하는 값 조회 하여 ojectId 생성하여 반환.
        public static string JsonGet(string objectId, string key)
        {
            if (!jsonObjects.ContainsKey(objectId))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "유효하지 않은 JSON 객체 ID입니다." : "Invalid JSON object ID.");
            }

            JToken objToken = jsonObjects[objectId];
            if (!(objToken is JObject))
            {
                throw new InvalidOperationException(
                    Tools.IsLangKorean() ? "객체 타입의 JSON에만 Get이 가능합니다." :
                    "Get is only possible for JSON object type.");
            }

            JObject obj = (JObject)objToken;
            if (!obj.ContainsKey(key))
            {
                throw new KeyNotFoundException(
                    Tools.IsLangKorean() ? "지정된 키를 찾을 수 없습니다." : "Specified key not found.");
            }

            // 키에 해당하는 값을 가져와서 새로운 objectId 생성
            string newObjectId = String.Format("JsonStruct_{0}", GenerateObjectId());
            jsonObjects[newObjectId] = obj[key];
            return newObjectId;
        }

        // JSON 문자열로 변환
        public static string GetJson(string objectId)
        {
            if (!jsonObjects.ContainsKey(objectId))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "유효하지 않은 JSON 객체 ID입니다." : "Invalid JSON object ID.");
            }

            return jsonObjects[objectId].ToString(Formatting.Indented);
        }

        // 객체 제거
        public static void JsonClear(string objectId)
        {
            if (jsonObjects.ContainsKey(objectId))
            {
                jsonObjects.Remove(objectId);

                // jsonObjects가 비어있으면 카운터 초기화
                if (jsonObjects.Count == 0)
                {
                    ResetObjectCounter();
                }
            }
        }

        private static void ResetObjectCounter()
        {
            objectCounter = 0;
        }

        private static class DataTypeParser
        {
            public struct TypeInfo
            {
                public string BaseType;
                public string Format;
            }

            public static TypeInfo Parse(string dataType)
            {
                TypeInfo result = new TypeInfo();
                if (string.IsNullOrEmpty(dataType))
                {
                    result.BaseType = "string";
                    return result;
                }

                string[] parts = dataType.Split(new char[] { ':' }, 2);
                result.BaseType = parts[0].ToLower();
                result.Format = parts.Length > 1 ? parts[1] : string.Empty;
                return result;
            }

        }
        // 값을 JToken으로 변환
        private static JToken ConvertToJToken(object value, string dataType)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return JValue.CreateNull();

            string strValue = value.ToString().Trim();
            var typeInfo = DataTypeParser.Parse(dataType);

            try
            {
                switch (typeInfo.BaseType)
                {
                    case "null":
                        return JValue.CreateNull();
                    case "bool":
                        return new JValue(strValue == "1" || bool.Parse(strValue));
                    case "int":
                        return new JValue(int.Parse(strValue));
                    case "long":
                        return new JValue(long.Parse(strValue));
                    case "float":
                        return new JValue(float.Parse(strValue));
                    case "double":
                        return new JValue(double.Parse(strValue));
                    case "decimal":
                        return new JValue(decimal.Parse(strValue));
                    case "datetime":
                        DateTime dateValue;
                        if (DateTime.TryParse(strValue, out dateValue))
                        {
                            string dateFormat = !string.IsNullOrEmpty(typeInfo.Format) ?
                                              typeInfo.Format : "yyyy-MM-dd HH:mm:ss";
                            try
                            {
                                return new JValue(dateValue.ToString(dateFormat));
                            }
                            catch
                            {
                                return new JValue(dateValue.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                        return new JValue(string.Empty);
                    default:
                        return new JValue(strValue);
                }
            }
            catch
            {
                switch (dataType.ToLower())
                {
                    case "null":
                        return JValue.CreateNull();
                    case "bool":
                        return new JValue(false);
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                    case "decimal":
                        return new JValue(0);
                    default:
                        return new JValue(string.Empty);
                }
            }
        }


        public static int Run_JsonStructNew(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = ""; //20250618 PSU null-> ""
            try
            {
                string type = args[0].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                val = JsonStructNew(type);
                return 1;
            }
            catch (Exception ex)
            {
                val = "";
                scriptClass.ErrorMessage(String.Format("@JsonStructNew Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        public static int Run_JsonAppend(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string objectId = args[0].ToString();
                object value = args[1];
                //string dataType = args.Length < 3 ? "string" : args[2].ToString();
                string dataType = "string";   //20250618 PSU null 체크.
                if (args.Length >= 3 && args[2] != null)
                {
                    dataType = args[2].ToString();
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                JsonAppend(objectId, value, dataType);
                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@JsonAppend Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        //최상위 레벨 키-값 설정
        public static int Run_JsonSet(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string objectId = args[0].ToString();
                string key = args[1].ToString();
                object value = args[2];
                //string dataType = args.Length < 4 ? "string" : args[3].ToString();
                string dataType = "string";   //20250618 PSU null 체크.
                if (args.Length >= 4 && args[3] != null)
                {
                    dataType = args[3].ToString();
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                JsonSet(objectId, key, value, dataType);
                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@JsonSet Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        public static int Run_JsonToString(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                string objectId = args[0].ToString();
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU
                val = GetJson(objectId);
                return 1;
            }
            catch (Exception ex)
            {
                val = "";
                scriptClass.ErrorMessage(String.Format("@JsonToString Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        public static int Run_JsonClear(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string objectId = args[0].ToString();
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU
                JsonClear(objectId);
                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                val = 0;
                scriptClass.ErrorMessage(String.Format("@JsonClear Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        public static int Run_JsonGet(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                string objectId = args[0].ToString();
                string key = args[1].ToString();
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU
                val = JsonGet(objectId, key);
                return 1;
            }
            catch (Exception ex)
            {
                val = "";
                scriptClass.ErrorMessage(String.Format("@JsonGet Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        // JToken 객체에서 JsonPath로 값 변경
        public static void JsonSetPathValue(string objectId, string jsonPath, object newValue, string dataType)
        {
            if (!jsonObjects.ContainsKey(objectId))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "유효하지 않은 JSON 객체 ID입니다." : "Invalid JSON object ID.");
            }

            JToken token = jsonObjects[objectId];
            JToken target = token.SelectToken(jsonPath);

            if (target != null)
            {
                target.Replace(ConvertToJToken(newValue, dataType));
            }
            else
            {
                throw new Exception(
                    Tools.IsLangKorean() ? "지정된 경로를 찾을 수 없습니다." : "Specified path not found.");
            }
        }

        // JToken 객체에서 JsonPath로 값 읽기
        public static string JsonGetPathValue(string objectId, string jsonPath)
        {
            if (!jsonObjects.ContainsKey(objectId))
            {
                throw new ArgumentException(
                    Tools.IsLangKorean() ? "유효하지 않은 JSON 객체 ID입니다." : "Invalid JSON object ID.");
            }

            JToken token = jsonObjects[objectId];
            //JToken result = token.SelectToken(jsonPath);
            IEnumerable<JToken> resultTokens = token.SelectTokens(jsonPath);
            List<string> results = new List<string>();

            foreach (JToken resultToken in resultTokens)
            {
                results.Add(resultToken.ToString());
            }

            if (results.Count == 0)
            {
                throw new Exception(
                    Tools.IsLangKorean() ? "지정된 경로를 찾을 수 없습니다." : "Specified path not found.");
            }
            return results.Count == 1 ? results[0] : string.Join(",", results.ToArray());
        }

        // JsonPath 관련 Run 함수들
        public static int Run_JsonSetPathValue(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                string objectId = args[0].ToString();
                string jsonPath = args[1].ToString();
                object newValue = args[2];
                //string dataType = args.Length < 4 ? "string" : args[3].ToString();

                string dataType = "string";   //20250612 PSU null 체크.
                if (args.Length >= 4 && args[3] != null)
                {
                    dataType = args[3].ToString();
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                JsonSetPathValue(objectId, jsonPath, newValue, dataType);
                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(String.Format("@JsonSetValue Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        public static int Run_JsonGetPathValue(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                string objectId = args[0].ToString();
                string jsonPath = args[1].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                val = JsonGetPathValue(objectId, jsonPath);
                return 1;
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(String.Format("@JsonGetValue Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }



        // JSON 문자열을 JToken으로 변환하여 저장
        public static string JsonFromString(string jsonContent)
        {
            try
            {
                string objectId = String.Format("JsonStruct_{0}", GenerateObjectId());

                // BOM 제거
                if (jsonContent.StartsWith("\ufeff"))
                {
                    jsonContent = jsonContent.Replace("\ufeff", "");
                }

                JToken parsedJson = JToken.Parse(jsonContent);
                jsonObjects[objectId] = parsedJson;

                return objectId;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int Run_JsonFromString(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                string jsonContent = args[0].ToString();

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                val = JsonFromString(jsonContent);
                return 1;
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(String.Format("@JsonFromString Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        public static int Run_JsonClearAll(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (jsonObjects != null)
                {
                    jsonObjects.Clear();
                    objectCounter = 0;
                }
                val = 1;
                return 1;
            }
            catch (Exception ex)
            {
                scriptClass.ErrorMessage(String.Format("@JsonClearAll Error\n{0}", ex.Message));
                val = 1;
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        static int Run_JsonStringGetValue(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                if (args.Length < 2)
                {
                    throw new ArgumentException(Tools.IsLangKorean() ?
                        "인자가 부족합니다." :
                        "Insufficient arguments.");
                }

                string jsonString = args[0].ToString();
                string jsonPath = args[1].ToString();

                if (string.IsNullOrEmpty(jsonString))
                {
                    throw new ArgumentException(Tools.IsLangKorean() ?
                        "JSON 문자열이 비어있습니다." :
                        "JSON string is null or empty.");
                }

                if (string.IsNullOrEmpty(jsonPath))
                {
                    throw new ArgumentException(Tools.IsLangKorean() ?
                        "JSONPath가 비어있습니다." :
                        "JSONPath is null or empty.");
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                // BOM 제거
                if (jsonString.StartsWith("\ufeff"))
                {
                    jsonString = jsonString.Replace("\ufeff", "");
                }

                JToken json = JToken.Parse(jsonString);
                IEnumerable<JToken> resultTokens = json.SelectTokens(jsonPath);

                List<string> results = new List<string>();
                foreach (JToken token in resultTokens)
                {
                    results.Add(token.ToString());
                }

                if (results.Count == 0)
                {
                    val = "";
                }
                else if (results.Count == 1)
                {
                    val = results[0];
                }
                else
                {
                    val = string.Join(",", results.ToArray());
                }
                return 1;
            }
            catch (Exception ex)
            {
                val = "";
                scriptClass.ErrorMessage(String.Format("@JsonStringGetValue Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }

        static int Run_JsonStringSetValue(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
            try
            {
                if (args.Length < 3)
                {
                    throw new ArgumentException(Tools.IsLangKorean() ?
                        "인자가 부족합니다." :
                        "Insufficient arguments.");
                }

                string jsonString = args[0].ToString();
                string jsonPath = args[1].ToString();
                object newValue = args[2];
                //string dataType = args.Length > 3 ? args[3].ToString() : "string";

                string dataType = "string";   //20250618 PSU null 체크.
                if (args.Length >= 4 && args[3] != null)
                {
                    dataType = args[3].ToString();
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                if (string.IsNullOrEmpty(jsonString))
                {
                    throw new ArgumentException(Tools.IsLangKorean() ?
                        "JSON 문자열이 비어있습니다." :
                        "JSON string is null or empty.");
                }

                if (string.IsNullOrEmpty(jsonPath))
                {
                    throw new ArgumentException(Tools.IsLangKorean() ?
                        "JSONPath가 비어있습니다." :
                        "JSONPath is null or empty.");
                }

                // BOM 제거
                if (jsonString.StartsWith("\ufeff"))
                {
                    jsonString = jsonString.Replace("\ufeff", "");
                }

                JToken json = JToken.Parse(jsonString);
                JToken target = json.SelectToken(jsonPath);

                if (target == null)
                {
                    throw new Exception(Tools.IsLangKorean() ?
                        "지정된 경로를 찾을 수 없습니다." :
                        "Specified path not found.");
                }

                // 기존 ConvertToJToken 메서드 활용
                JToken newToken = ScriptFunctionJson.ConvertToJToken(newValue, dataType);
                target.Replace(newToken);

                //val = json.ToString(Formatting.Indented);
                args[0] = json.ToString(Formatting.Indented);
                val = 1; //성공
                return 1;
            }
            catch (Exception ex)
            {
                val = 0; //실패
                scriptClass.ErrorMessage(String.Format("@JsonStringSetValue Error\n{0}", ex.Message));
                return -1;  //20250618 PSU 0 -> -1
            }
        }


        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            if (!TotalConfig.GetOemTypeRight(EnumOemTypeRight.Json)) return;  //20250226 PSU
            string prename = "Json";

            prepare.AddMethod(prename, "JsonStructNew", "object", new ScriptExternalRun.DeleMethod(Run_JsonStructNew), "in:string:type");
            prepare.AddMethod(prename, "JsonAppend", "int", new ScriptExternalRun.DeleMethod(Run_JsonAppend), "in:string:objectId", "in:object:value", "params:object:dataType");
            prepare.AddMethod(prename, "JsonSet", "int", new ScriptExternalRun.DeleMethod(Run_JsonSet), "in:string:objectId", "in:string:key", "in:object:value", "params:object:dataType");
            prepare.AddMethod(prename, "JsonToString", "string", new ScriptExternalRun.DeleMethod(Run_JsonToString), "in:string:objectId");
            prepare.AddMethod(prename, "JsonClear", "int", new ScriptExternalRun.DeleMethod(Run_JsonClear), "in:string:objectId");
            prepare.AddMethod(prename, "JsonGet", "string", new ScriptExternalRun.DeleMethod(Run_JsonGet), "in:string:objectId", "in:string:key");
            prepare.AddMethod(prename, "JsonFromString", "string", new ScriptExternalRun.DeleMethod(Run_JsonFromString), "in:string:jsonContent");
            prepare.AddMethod(prename, "JsonSetValue", "int", new ScriptExternalRun.DeleMethod(Run_JsonSetPathValue), "in:string:objectId", "in:string:jsonPath", "in:object:value", "params:object:dataType");
            prepare.AddMethod(prename, "JsonGetValue", "string", new ScriptExternalRun.DeleMethod(Run_JsonGetPathValue), "in:string:objectId", "in:string:jsonPath");
            prepare.AddMethod(prename, "JsonClearAll", "int", new ScriptExternalRun.DeleMethod(Run_JsonClearAll));

            prepare.AddMethod(prename, "JsonStringGetValue", "string", new ScriptExternalRun.DeleMethod(Run_JsonStringGetValue), "in:string:source", "in:string:jsonPath");
            prepare.AddMethod(prename, "JsonStringSetValue", "int", new ScriptExternalRun.DeleMethod(Run_JsonStringSetValue), "ref:string:source", "in:string:jsonPath", "in:object:value");

            //}  //  prename 앞부분 중복되면 구엔진 스크립트에서 에러. 긴 이름을 먼저 하면 되는 듯?
            //public static void PrepareMethod(ScriptExternalRun prepare)
            //{
            //    string prename = "JsonTemplate";

            prepare.AddMethod(prename, "JsonTemplateLoad", "string", new ScriptExternalRun.DeleMethod(ScriptFunctionJsonTemplate.Run_JsonLoadTemplate), "in:string:templateId");
            //JSON 템플릿 읽어와서 문자열 반환
            prepare.AddMethod(prename, "JsonTemplateReplacePlaceholder", "int", new ScriptExternalRun.DeleMethod(ScriptFunctionJsonTemplate.Run_JsonReplacePlaceholder), "in:string:templateId",
              "in:string:placeholder", "in:object:value", "params:string:dataType");
            //JSON 템플릿에 변경할 플레이스홀더 값을 리스트에 등록
            prepare.AddMethod(prename, "JsonTemplateReplaceDone", "string", new ScriptExternalRun.DeleMethod(ScriptFunctionJsonTemplate.Run_JsonReplaceDone), "in:string:templateId");
            //플레이스홀더 변경 리스트를 템플렛에 적용
            prepare.AddMethod(prename, "JsonTemplateReplaceMissing", "int", new ScriptExternalRun.DeleMethod(ScriptFunctionJsonTemplate.Run_JsonReplaceMissingPlaceholders), "ref:string:jsonContent");
            //JSON 템플릿에 플레이스홀더 남은 부분 null로 변경
            prepare.AddMethod(prename, "JsonTemplateTagSet", "int", new ScriptExternalRun.DeleMethod(ScriptFunctionJsonTemplate.Run_JsonTemplateTagSet), "ref:string:jsonContent");
            //JSON 템플릿에 태그값 모두 적용

        }

    }
}
