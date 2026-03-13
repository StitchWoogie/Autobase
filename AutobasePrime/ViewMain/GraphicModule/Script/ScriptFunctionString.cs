using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GraphicModule
{
    /// <summary>
    /// Summary description for ScriptFunctionComboBox.
    /// </summary>
    public class ScriptFunctionString
    {
        /*
        public ScriptFunctionString()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static int Function_Trim(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            val = source.Trim();

            return 1;
        }

        static int Function_TrimEnd(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            val = source.TrimEnd();

            return 1;
        }

        static int Function_TrimStart(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            val = source.TrimStart();

            return 1;
        }

        static int Function_IndexOf(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;
            string search;
            int index;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out search)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out index)) return -1;

            val = source.IndexOf(search, index);

            return 1;
        }

        static int Function_Substring(ScriptClass scriptClass, string command, string argument, out object val)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string source;
            int index;
            int length;

            val = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out source)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out index)) return -1;

            arg.GetArgument(out buf);


            if (index < 0 || index >= source.Length)  // index가 source length 보다 크면 catch 발생
            {
                val = "";
            }
            else
            {
                if (buf.Length == 0)    // length가 없는 경우
                {
                    val = source.Substring(index);
                }
                else
                {
                    if (!scriptClass.GetValueRecurse(buf, out length)) return -1;

                    if (index + length > source.Length)  // 가져오는 크기가 범위를 오버하면 catch발생
                        length = source.Length - index;

                    try
                    {
                        val = source.Substring(index, length);
                    }
                    catch   // 컴파일 시 오류를 방지 2012.12.17
                    {
                        val = "";
                    }
                }
            }

            return 1;
        }

        public static int Function_String(ScriptClass scriptClass, string command, string argument, out object val)
        {
            if (command == "StringTrim")
            {
                return Function_Trim(scriptClass, command, argument, out val);
            }
            else if (command == "StringTrimEnd")
            {
                return Function_TrimEnd(scriptClass, command, argument, out val);
            }
            else if (command == "StringTrimStart")
            {
                return Function_TrimStart(scriptClass, command, argument, out val);
            }
            else if (command == "StringIndexOf")
            {
                return Function_IndexOf(scriptClass, command, argument, out val);
            }
            else if (command == "StringSubstring")
            {
                return Function_Substring(scriptClass, command, argument, out val);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 String 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 String 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined String function ({0})", command));
                }
                val = 0;
                return -1;
            }
        }*/

        static int Run_StringUnicodeToAnsi(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;
                                    
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                byte[] bytes = Tools.StringToBytes((string)args[1]);
                args[0] = bytes;
            }

            return 1;
        }

        static int Run_StringTrim(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string source = (string)args[0];

            val = 0;

            val = source.Trim();

            return 1;
        }

        static int Run_StringTrimEnd(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string source = (string)args[0];

            val = 0;

            val = source.TrimEnd();

            return 1;
        }

        static int Run_StringTrimStart(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string source = (string)args[0];

            val = 0;

            val = source.TrimStart();

            return 1;
        }

        static int Run_StringIndexOf(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string source = (string)args[0];
            string search = (string)args[1];
            int index = (int)args[2];

            val = 0;

            val = source.IndexOf(search, index);

            return 1;
        }

        static int Run_StringSubstring(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            string source = (string)args[0];
            int index = (int)args[1];
            //int length = (int)args[2];

            val = 0;

            if (index < 0 || index >= source.Length)  // index가 source length 보다 크면 catch 발생
            {
                val = "";
            }
            else
            {
                if (args.Length < 3)
                {
                    val = source.Substring(index);
                }
                else
                {
                    int length = scriptClass.GetValueInt(args[2]);

                    if (index + length > source.Length)  // 가져오는 크기가 범위를 오버하면 catch발생
                        length = source.Length - index;

                    try
                    {
                        val = source.Substring(index, length);
                    }
                    catch   // 컴파일 시 오류를 방지 2012.12.17
                    {
                        val = "";
                    }
                }

            }

            return 1;
        }

        //20250107 StirngSplit 추가
        static int Run_StringSplit(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                string source = (string)args[0];

                // 입력 문자열이 null이거나 비어있는 경우
                if (string.IsNullOrEmpty(source))
                {
                    //val = new string[] { };
                    val = "";
                    return 1;
                }

                // 구분자가 없는 경우
                if (args.Length < 2)
                {
                    //val = new string[] { source };
                    val = source;
                    return 1;
                }

                // 구분자 처리
                string separator = scriptClass.GetValueString(args[1]);

                // StringSplitOptions 처리 (옵션: args[2])
                StringSplitOptions options = StringSplitOptions.None;
                if (args.Length > 2)
                {
                    int opt = scriptClass.GetValueInt(args[2]);
                    if (opt == 1)
                        options = StringSplitOptions.RemoveEmptyEntries;
                }

                // Split 실행
                if (args.Length > 3)
                {
                    // count 제한이 있는 경우
                    int count = scriptClass.GetValueInt(args[3]);
                    if (count <= 0)
                    {
                        //val = new string[] { source };
                        val = source;
                    }
                    else
                    {
                        val = source.Split(new string[] { separator }, count, options);
                    }
                }
                else
                {
                    // 기본 Split
                    val = source.Split(new string[] { separator }, options);
                }

                return 1;
            }
            catch
            {
                val = new string[] { };
                return 1;
            }
        }

        //20250107 JSON파싱 추가
        static int Run_StringJson(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                // 입력 인자 확인
                if (args.Length < 2)
                {
                   // val = "Error: Insufficient arguments.";
                    scriptClass.ErrorMessage(String.Format("@StringJson Error\n Insufficient arguments."));
                    return -1; //20250618 PSU 1 -> -1
                }

                string source = (string)args[0];
                string jsonPath = (string)args[1];

                // 입력 JSON 문자열 확인
                if (string.IsNullOrEmpty(source))
                {
                    //val = "Error: JSON source is null or empty.";
                    scriptClass.ErrorMessage(String.Format("@StringJson Error\n JSON source is null or empty."));
                    return -1;  //20250618 PSU 1 -> -1
                }

                // JSONPath 확인
                if (string.IsNullOrEmpty(jsonPath))
                {
                    //val = "Error: JSONPath is null or empty.";
                    scriptClass.ErrorMessage(String.Format("@StringJson  Error\n JSONPath is null or empty."));
                    return -1;  //20250618 PSU 1 -> -1
                }

                // BOM 제거
                if (source.StartsWith("\ufeff"))
                {
                    //source = source.Substring(1);
                    source = source.Replace("\ufeff", ""); //BOM 문자만 정확하게 제거 20250120 PSU 수정
                }

                if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return 1; //20250618 PSU

                // JSON 파싱
                JToken json = JToken.Parse(source);

                // JSONPath를 사용하여 값 추출
                IEnumerable<JToken> resultTokens = json.SelectTokens(jsonPath);

                // 결과 처리
                List<string> results = new List<string>();
                foreach (JToken token in resultTokens)
                {
                    results.Add(token.ToString());
                }

                // 결과 반환 형식 결정
                if (results.Count == 0)
                {
                   // val = "Error: No matching values found for the given JSONPath."; 
                }
                else if (results.Count == 1)
                {
                    val = results[0]; // 값이 한 개일 경우
                }
                else
                {
                    val = string.Join(",", results.ToArray()); // 값이 여러 개일 경우 콤마로 구분
                }

                return 1;
            }
            catch (Exception ex)
            {
                //val = string.Format("Error: {0}", ex.Message);
                scriptClass.ErrorMessage(String.Format("@StringJson Error\n {0}", ex.Message));
                return -1;  //20250618 PSU 1 -> -1
            }
        }


        // 정규식을 이용한 문자열 치환 Run 메서드 20250224 PSU
        static int Run_StringRegexReplace(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";
            try
            {
                // 입력 인자 확인
                if (args.Length < 3)
                {
                    //val = "Error: Insufficient arguments.";
                    scriptClass.ErrorMessage(String.Format("@StringRegexReplace Error\n Insufficient arguments."));
                    return -1;
                }

                string source = (string)args[0];
                string pattern = (string)args[1];
                string replacement = (string)args[2];

                // 입력 문자열 확인
                if (string.IsNullOrEmpty(source))
                {
                    //val = "Error: Source string is null or empty.";
                    scriptClass.ErrorMessage(String.Format("@StringRegexReplace Error\nSource string is null or empty."));  //20250618 PSU
                    return -1;
                }

                // 정규식 패턴 확인
                if (string.IsNullOrEmpty(pattern))
                {
                    //val = "Error: Regex pattern is null or empty.";
                    scriptClass.ErrorMessage(String.Format("@StringRegexReplace Error\nRegex pattern is null or empty."));  //20250618 PSU
                    return -1;
                }

                // 정규식 치환 수행
                try
                {
                    val = System.Text.RegularExpressions.Regex.Replace(source, pattern, replacement ?? string.Empty);
                    return 1;
                }
                catch (ArgumentException ex)
                {
                    //val = string.Format("Error: Invalid regular expression pattern - {0}", ex.Message);
                    val = "";
                    scriptClass.ErrorMessage(String.Format("@StringRegexReplace Error\n{0}", ex.Message));  //20250618 PSU
                    return -1;
                }
            }
            catch (Exception ex)
            {
                //val = string.Format("Error: {0}", ex.Message);
                scriptClass.ErrorMessage(String.Format("@StringRegexReplace Error\n{0}", ex.Message));  //20250618 PSU
                return -1;
            }
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "String";

            prepare.AddMethod(prename, "StringUnicodeToAnsi", "int", new ScriptExternalRun.DeleMethod(Run_StringUnicodeToAnsi), "out:byte[]:target", "in:string:source");

            prepare.AddMethod(prename, "StringTrim", "string", new ScriptExternalRun.DeleMethod(Run_StringTrim), "in:string:source");
            prepare.AddMethod(prename, "StringTrimEnd", "string", new ScriptExternalRun.DeleMethod(Run_StringTrimEnd), "in:string:source");
            prepare.AddMethod(prename, "StringTrimStart", "string", new ScriptExternalRun.DeleMethod(Run_StringTrimStart), "in:string:source");
            prepare.AddMethod(prename, "StringIndexOf", "int", new ScriptExternalRun.DeleMethod(Run_StringIndexOf), "in:string:source", "in:string:search", "in:int:startindex");
            prepare.AddMethod(prename, "StringSubstring", "string", new ScriptExternalRun.DeleMethod(Run_StringSubstring), "in:string:source", "in:int:startindex", "params:object:length");    // params은 object 또는 object[] 로 선언하여야 한다.


            prepare.AddMethod(prename, "StringSplit", "string[]", new ScriptExternalRun.DeleMethod(Run_StringSplit), "in:string:source", "in:string:separator"); 
            prepare.AddMethod(prename, "StringSplit", "string[]", new ScriptExternalRun.DeleMethod(Run_StringSplit), "in:string:source", "in:string:separator", "params:object:count");
            prepare.AddMethod(prename, "StringJson", "string", new ScriptExternalRun.DeleMethod(Run_StringJson), "in:string:source", "in:string:path");

            prepare.AddMethod(prename, "StringRegexReplace", "string", new ScriptExternalRun.DeleMethod(Run_StringRegexReplace), "in:string:source", "in:string:pattern", "in:string:replacement"); //20250224 PSU
        }
    }
}
