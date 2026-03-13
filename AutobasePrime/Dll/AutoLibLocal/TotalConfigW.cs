using System;
using NetTools;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for TotalConfigW.
	/// </summary>
	public class TotalConfigW
	{
		static TotalConfigW()
		{
			//
			// TODO: Add constructor logic here
			//
            //ReReadProjectPlatform();
		}

        /*
        public static string sConfigFileNameOnWebView = null; // 웹 뷰일때의 파일명

        public static string GetProjectConfigFilename()
        {
            string filename;
            
            if(ConfigVarTotal.bLocalFlag)
                filename = TotalConfig.sDirWorkProject + "\\Config\\ProjectConfig.inix";
            else
                filename = sConfigFileNameOnWebView;

            return filename;
        }

        // 각 프로젝트에만 해당되는 고유 설정
        public static string LoadConfig(string section, string sub, string item, string defaultvalue)
        {
            string filename = GetProjectConfigFilename();
            string retn = "";

            Profile.GetPrivateProfileStringW(section + "_" + sub, item, defaultvalue, ref retn, filename);

            return retn;
        }

        public static void SaveConfig(string section, string sub, string item, string value)
        {
            string filename = GetProjectConfigFilename();

            Profile.WritePrivateProfileStringW(section + "_" + sub, item, value, filename);
        }

        public static EnumProjectPlatform eProjectPlatform = EnumProjectPlatform.Win;

        public static void ReReadProjectPlatform()
        {
            string filename = GetProjectConfigFilename();

            int platform = Profile.GetPrivateProfileIntW("Config", "Platform", 0, filename);

            if(platform == 1)  eProjectPlatform = EnumProjectPlatform.CE;
            else               eProjectPlatform = EnumProjectPlatform.Win;
        }

        public static bool LoadConfig(string section, string sub, string item, bool defaultvalue)
        {
            string retn;

            retn = LoadConfig(section, sub, item, defaultvalue.ToString());

            return ConvertTool.ToBoolean(retn);
        }

        public static void SaveConfig(string section, string sub, string item, bool value)
        {
            SaveConfig(section, sub, item, value.ToString());
        }

        public static int LoadConfig(string section, string sub, string item, int defaultvalue)
        {
            string retn;

            retn = LoadConfig(section, sub, item, defaultvalue.ToString());

            return ConvertTool.ToInt32(retn);
        }

        public static void SaveConfig(string section, string sub, string item, int value)
        {
            SaveConfig(section, sub, item, value.ToString());
        }*/

		public static string GetDefaultUser(string work_dir)
		{
			string filename = work_dir+"\\Config\\ConfigW.ini";
			string username="";

			Profile.GetPrivateProfileStringW("User", "DefaultUser", "", ref username, filename);
			return username;
		}

		public static void SetDefaultUser(string work_dir, string username)
		{
			string filename = work_dir+"\\Config\\ConfigW.ini";

			Profile.WritePrivateProfileStringW("User", "DefaultUser", username, filename);
		}
	}

    
}
