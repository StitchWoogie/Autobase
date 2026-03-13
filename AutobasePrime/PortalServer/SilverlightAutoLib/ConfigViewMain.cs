using System;
using AutoLibLocal;
using NetTools;

namespace AutoLib
{
	/// <summary>
	/// ViewMain에서만 사용하는 환경 설정 파일 들
	/// </summary>
	//[Serializable]
	public class ConfigViewMain
	{
		public static bool bFitToWindow = true;
		//public static Font fontMain = new Font("Gulim", 10);
		public static bool bDisplayToolTip = true;
		public static bool bResponseMouseLeftOnGraphic = true;
		public static bool bResponseMouseRightOnGraphic = true;
		public static bool bUseMenuButtonOnGraphic = true;
		public static int  nMdiCountOnGraphic = 5;
		public static int  nMdiCountOnBasicScreen = 2;
		public static bool bShowMainTitle = true;
		public static bool bShowMainMenu = true;
		public static int  nForLoopTimeout = 5;
		public static string PrinterOnScriptPrintModule;
		public static bool bDisplayLogInBoxOnStart = false;

        public static bool bUseNotifyIcon = false;
        public static bool bUseTestModeMessage = false;

		public static bool bNaviSaveFlag;
		public static int  nNaviSaveX;
		public static int  nNaviSaveY;
		public static int  nNaviSaveWidth;
		public static int  nNaviSaveHeight;

        public static bool bBasciScreenDataViewWhiteBackground;

        public static bool bUseDeviceQuality;

        public static bool bUserControlBoxUseAnalogModule;
        public static bool bUserControlBoxUseDigitalModule;
        public static bool bUserControlBoxUseStringModule;
        public static string sUserControlBoxAnalogModule;
        public static string sUserControlBoxDigitalModule;
        public static string sUserControlBoxStringModule;
		
		static ConfigViewMain()
		{
            /*
			if(NetTools.Tools.IsLangJapanese())
				fontMain = new Font("MS UI Gothic", 10);
			else if(NetTools.Tools.IsLangChinese())
				fontMain = new Font("SimSun", 10);
			else {}

			ConfigViewMain.Load();*/
		}

        /*
		public static void Load()
		{
			bDisplayToolTip = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bDisplayTagInfoOnMouseMove", true);
			bFitToWindow = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bDisplayFitWindowSize", false);
			bResponseMouseLeftOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bLMouseResponseOnGraphic", true);
			bResponseMouseRightOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bRMouseResponseOnGraphic", true);
			bUseMenuButtonOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bMenuButtonOnGraphic", true);
			nMdiCountOnGraphic = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMaxMdiScreenGraphic", 5);
			nMdiCountOnBasicScreen = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nMdiCountOnBasicScreen", 2);

            bShowMainTitle = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bShowMainTitle", true);
            bShowMainMenu = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bShowMainMenu", true);

			nForLoopTimeout = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "nForLoopTimeout", 5);
			PrinterOnScriptPrintModule = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "PrinterOnScriptPrintModule", "");

			string font_string = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "Font", "");
			if(font_string.Length != 0) 
			{
				CommaBlockString comma = new CommaBlockString();
				comma.Set(font_string);

				int imsi = 0;
				string fname = "arial";
				float  fheight = 10;
				FontStyle fstyle = 0;

				comma.GetString(ref fname);
				comma.GetFloat(ref fheight);

				comma.GetInt(ref imsi);
				if(imsi == 1) fstyle |= FontStyle.Bold;
				comma.GetInt(ref imsi);
				if(imsi == 1) fstyle |= FontStyle.Italic;
				comma.GetInt(ref imsi);
				if(imsi == 1) fstyle |= FontStyle.Strikeout;
				comma.GetInt(ref imsi);
				if(imsi == 1) fstyle |= FontStyle.Underline;			

				fontMain = new Font(fname, fheight, fstyle);
			}

			bNaviSaveFlag = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "bNaviSaveFlag", false);
			nNaviSaveX = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveX", 0);
			nNaviSaveY = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveY", 0);
			nNaviSaveWidth = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveWidth", 100);
			nNaviSaveHeight = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveHeight", 100);

			bDisplayLogInBoxOnStart = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config\\LogIn", "bDisplayLogInBoxOnStart", false);

            bUseNotifyIcon = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bUseNotifyIcon", false);
            bUseTestModeMessage = TotalConfig.LoadRegAutoBaseConfig("RunMain", "Config", "bUseTestModeMessage", true);

            bBasciScreenDataViewWhiteBackground = TotalConfig.LoadRegAutoBaseConfig("RunMain", "BasicScreen", "bBasciScreenDataViewWhiteBackground", false);
		}

		public static void Save()
		{
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bDisplayTagInfoOnMouseMove", bDisplayToolTip);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bDisplayFitWindowSize", bFitToWindow);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bLMouseResponseOnGraphic", bResponseMouseLeftOnGraphic);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bRMouseResponseOnGraphic", bResponseMouseRightOnGraphic);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bMenuButtonOnGraphic", bUseMenuButtonOnGraphic);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMaxMdiScreenGraphic", nMdiCountOnGraphic);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nMdiCountOnBasicScreen", nMdiCountOnBasicScreen);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bShowMainTitle", bShowMainTitle);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bShowMainMenu", bShowMainMenu);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "nForLoopTimeout", nForLoopTimeout);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "PrinterOnScriptPrintModule", PrinterOnScriptPrintModule);

			string font_string = String.Format("{0},", fontMain.Name);
			font_string += String.Format("{0},", fontMain.Size);
			font_string += String.Format("{0},", (fontMain.Style & FontStyle.Bold) > 0 ? 1 : 0);
			font_string += String.Format("{0},", (fontMain.Style & FontStyle.Italic) > 0 ? 1 : 0);
			font_string += String.Format("{0},", (fontMain.Style & FontStyle.Strikeout) > 0 ? 1 : 0);
			font_string += String.Format("{0},", (fontMain.Style & FontStyle.Underline) > 0 ? 1 : 0);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "Font", font_string);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "bNaviSaveFlag", bNaviSaveFlag);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveX", nNaviSaveX);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveY", nNaviSaveY);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveWidth", nNaviSaveWidth);
			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\Navigation", "nNaviSaveHeight", nNaviSaveHeight);

			TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config\\LogIn", "bDisplayLogInBoxOnStart", bDisplayLogInBoxOnStart);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bUseNotifyIcon", bUseNotifyIcon);
            TotalConfig.SaveRegAutoBaseConfig("RunMain", "Config", "bUseTestModeMessage", bUseTestModeMessage);

            TotalConfig.SaveRegAutoBaseConfig("RunMain", "BasicScreen", "bBasciScreenDataViewWhiteBackground", bBasciScreenDataViewWhiteBackground);
		}*/
	}
}
