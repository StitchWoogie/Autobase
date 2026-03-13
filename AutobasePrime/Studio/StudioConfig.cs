using System;
using AutoLibLocal;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

namespace Studio
{
    /*
    // 10.0.4 부터는 AutoLib에 있는 ConfigStudio로 이동했다.
    public class StudioConfig
    {
        public static bool bGuideLineFit = false;
        public static int nGuideLineUnitX = 20;
        public static int nGuideLineUnitY = 20;
        public static int nGuideLineDisplayX = 1;
        public static int nGuideLineDisplayY = 1;
        public static int nGuideLineType = 0;
        public static Color lGuideLineColor = Color.Gray;
        public static bool bGuideLineMatchUnit = true;
        public static bool bGuideLineMatchDisplay = true;

        public static string sEditFileName;				// 사용할 비트맵 에디터는?

        public StudioConfig()
        {
            sEditFileName = Application.StartupPath + "\\SU30.EXE";
        }
    }

	/// <summary>
	/// Summary description for StudioConfig.
	/// </summary>
	public class config
	{
		public config()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static ConfigTotal total = new ConfigTotal();

		public static void Load()
		{
			string filename = Application.CommonAppDataPath+"\\Studio.cfg";

			if(File.Exists(filename) == false)	return;
			
			Stream s = File.OpenRead(filename);
			SoapFormatter format = new SoapFormatter();
			try 
			{
				total = (ConfigTotal)format.Deserialize(s);
			}
			catch 
			{
				
			}
			s.Close();
		}

		public static void Save()
		{
			string filename = Application.CommonAppDataPath+"\\Studio.cfg";
			
			Stream s = File.OpenWrite(filename);
			s.Flush();
			SoapFormatter format = new SoapFormatter();
			format.Serialize(s, total);
			s.Close();
		}
	}

	[Serializable]
	public class ConfigTotal
	{
		public bool		bGuideLineFit = false;
		public int		nGuideLineUnitX = 20;
		public int		nGuideLineUnitY = 20;
		public int		nGuideLineDisplayX = 1;
		public int		nGuideLineDisplayY = 1;
		public int		nGuideLineType = 0;
		public Color	lGuideLineColor = Color.Gray;
		public bool		bGuideLineMatchUnit = true;
		public bool		bGuideLineMatchDisplay = true;

		public string	sEditFileName;				// 사용할 비트맵 에디터는?
		//LOGFONT		logFontScript;				// 스크립트에서 사용하는 폰트.
		
	
		//LOGFONT		logFontObjectDefault;		// Object Default Font
		public ConfigTotal()
		{
			sEditFileName = Application.StartupPath+"\\SU30.EXE";
		}
	}

	*/
}
