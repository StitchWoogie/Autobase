using System;
using System.Collections.Generic;
using System.Text;
using AutoLibLocal;
using System.Drawing;
using System.Windows.Forms;

namespace AutoLib
{
    public class ConfigStudio
    {
        public static int nUndoCount = 20;
        public static bool bSaveLayerLockStatus = false;
        public static bool bSaveLayerShowStatus = false;

        public static bool bGuideLineFit = false;
        public static int nGuideLineUnitX = 20;
        public static int nGuideLineUnitY = 20;
        public static int nGuideLineDisplayX = 1;
        public static int nGuideLineDisplayY = 1;
        public static int nGuideLineType = 0;
        public static Color lGuideLineColor = Color.Gray;
        public static bool bGuideLineMatchUnit = true;
        public static bool bGuideLineMatchDisplay = true;

        public static int nPasteX = 0;
        public static int nPasteY = 0;

        public static string sEditFileName;				// 사용할 비트맵 에디터는?

        public static bool bUseNewScriptEditor;
        public static bool bUseMonacoEditor;        // true: WebView2+Monaco 사용, false: 레거시 TextArea 사용 20260301 PSU

        /*
        public StudioConfig()
        {
            sEditFileName = Application.StartupPath + "\\SU30.EXE";
        }*/

        static ConfigStudio()
        {
            Load();   
        }

        static void Load()
        {
            nUndoCount = TotalConfig.LoadRegAutoBaseConfig("Studio", "Undo", "nUndoCount", 20);
            bSaveLayerLockStatus = TotalConfig.LoadRegAutoBaseConfig("Studio", "Layer", "bSaveLayerLockStatus", false);
            bSaveLayerShowStatus = TotalConfig.LoadRegAutoBaseConfig("Studio", "Layer", "bSaveLayerShowStatus", false);

            bGuideLineFit = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "bGuideLineFit", false);
            nGuideLineUnitX = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineUnitX", 20);
            nGuideLineUnitY = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineUnitY", 20);
            nGuideLineDisplayX = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineDisplayX", 1);
            nGuideLineDisplayY = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineDisplayY", 1);
            nGuideLineType = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineType", 0);
            lGuideLineColor = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "lGuideLineColor", Color.Gray);
            bGuideLineMatchUnit = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "bGuideLineMatchUnit", true);
            bGuideLineMatchDisplay = TotalConfig.LoadRegAutoBaseConfig("Studio", "GuideLine", "bGuideLineMatchDisplay", true);

            string path = Application.StartupPath + "\\SU30.EXE";
            sEditFileName = TotalConfig.LoadRegAutoBaseConfig("Studio", "ExternalTools", "sEditFileName", path);

            nPasteX = TotalConfig.LoadRegAutoBaseConfig("Studio", "Paste", "X", 10);
            nPasteY = TotalConfig.LoadRegAutoBaseConfig("Studio", "Paste", "Y", 10);

            bUseNewScriptEditor = TotalConfig.LoadRegAutoBaseConfig("Studio", "Script", "bUseNewScriptEditor", true); //10.4 부터 true로 변경 250801 PSU 
            bUseMonacoEditor = TotalConfig.LoadRegAutoBaseConfig("Studio", "Script", "bUseSmartScriptEditor", true);// 20260301 PSU
        }

        public static void Save()
        {
            TotalConfig.SaveRegAutoBaseConfig("Studio", "Undo", "nUndoCount", nUndoCount);

            TotalConfig.SaveRegAutoBaseConfig("Studio", "Layer", "bSaveLayerLockStatus", bSaveLayerLockStatus);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "Layer", "bSaveLayerShowStatus", bSaveLayerShowStatus);

            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "bGuideLineFit", bGuideLineFit);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineUnitX", nGuideLineUnitX);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineUnitY", nGuideLineUnitY);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineDisplayX", nGuideLineDisplayX);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineDisplayY", nGuideLineDisplayY);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "nGuideLineType", nGuideLineType);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "lGuideLineColor", lGuideLineColor);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "bGuideLineMatchUnit", bGuideLineMatchUnit);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "GuideLine", "bGuideLineMatchDisplay", bGuideLineMatchDisplay);

            TotalConfig.SaveRegAutoBaseConfig("Studio", "ExternalTools", "sEditFileName", sEditFileName);

            TotalConfig.SaveRegAutoBaseConfig("Studio", "Paste", "X", nPasteX);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "Paste", "Y", nPasteY);

            TotalConfig.SaveRegAutoBaseConfig("Studio", "Script", "bUseNewScriptEditor", bUseNewScriptEditor);
            TotalConfig.SaveRegAutoBaseConfig("Studio", "Script", "bUseSmartScriptEditor", bUseMonacoEditor); //20260301 PSU
        }
    }
}
