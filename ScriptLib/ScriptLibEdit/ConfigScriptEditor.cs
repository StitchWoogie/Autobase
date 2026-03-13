using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace ScriptLibEdit
{
    public class ConfigScriptEditor
    {
        public static bool bAutomaticallyFormat;
        public static bool bDisplayLineEndingGlyph; // 라인끝에 엔터를 표시한다.
        public static bool bDrawingByMemory;        // 라인끝에 엔터를 표시한다.
        public static bool bUseMonacoEditor;        // true: WebView2+Monaco 사용, false: 레거시 TextArea 사용 기타환경설정과 같은 값

        static ConfigScriptEditor()
		{
			LoadConfig();
		}

        public static void LoadConfig()
		{
            bAutomaticallyFormat = RegistryTool.LoadConfig("ScriptLibEdit", "Editor", "bAutomaticallyFormat", true);
            bDisplayLineEndingGlyph = RegistryTool.LoadConfig("ScriptLibEdit", "Editor", "bDisplayLineEndingGlyph", false);
            bDrawingByMemory = RegistryTool.LoadConfig("ScriptLibEdit", "Editor", "bDrawingByMemory", true);
            bUseMonacoEditor = RegistryTool.LoadConfig("Autobase", "Studio\\Script", "bUseSmartScriptEditor", true);// 20260301 PSU
        }

        public static void Save()
		{
            RegistryTool.SaveConfig("ScriptLibEdit", "Editor", "bAutomaticallyFormat", bAutomaticallyFormat);
            RegistryTool.SaveConfig("ScriptLibEdit", "Editor", "bDisplayLineEndingGlyph", bDisplayLineEndingGlyph);
            RegistryTool.SaveConfig("ScriptLibEdit", "Editor", "bDrawingByMemory", bDrawingByMemory);
		}
    }
}
