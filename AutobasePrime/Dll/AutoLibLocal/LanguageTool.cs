using System;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AutoLibLocal
{
	public enum EnumLanguage
	{
		Auto,
		Chinese,
		English,
		Japanese,
		Korean,
        Vietnamese,
        Russian,
	}
	/// <summary>
	/// Summary description for LanguageTool.
	/// </summary>
	public class LanguageTool
	{
		public LanguageTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static EnumLanguage GetLanguage()
		{
			//string lang = TotalConfig.LoadRegAutoBaseConfig("Language", null, "Language", EnumLanguage.Auto.ToString());
			string filename = System.Windows.Forms.Application.StartupPath+"\\Config\\Program.ini";
			string lang = "";
			
			if(String.Compare(Application.ExecutablePath, 0, "http", 0, 4, true) == 0)	// http 실행
			{
				lang = TotalConfig.LoadRegAutoBaseConfig("WebClient", null, "Language", EnumLanguage.Auto.ToString());	
			}
			else 
			{
				NetTools.Profile.GetPrivateProfileStringA("Language", "Lang", "Auto", ref lang, filename);
			}

			EnumLanguage e;
			
			try 
			{
				e = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), lang);
			}
			catch 
			{
				e = EnumLanguage.Auto;
			}

			return e;
		}

		public static void SetLanguage(string lan)
		{
            //TotalConfig.SaveRegAutoBaseConfig("Language", null, "Language", lan);

			string filename = System.Windows.Forms.Application.StartupPath+"\\Config";
			if(!Directory.Exists(filename))	Directory.CreateDirectory(filename);
			filename = System.Windows.Forms.Application.StartupPath+"\\Config\\Program.ini";
			
			NetTools.Profile.WritePrivateProfileStringA("Language", "Lang", lan, filename);
		}

		public static void SetLanguageWebClient(string lan)
		{
			TotalConfig.SaveRegAutoBaseConfig("WebClient", null, "Language", lan);
		}

		public static EnumLanguage GetLanguageWebClient()
		{
			string lang = TotalConfig.LoadRegAutoBaseConfig("WebClient", null, "Language", EnumLanguage.Auto.ToString());

			EnumLanguage e;
			
			try 
			{
				e = (EnumLanguage)Enum.Parse(typeof(EnumLanguage), lang);
			}
			catch 
			{
				e = EnumLanguage.Auto;
			}

			return e;
		}

        public static void ChangeUICulture(bool web)
        {
            EnumLanguage e;
            
            if(web) 
                e = GetLanguageWebClient();
            else
                e = GetLanguage();

            if (e == EnumLanguage.Korean)
            {
                // ko는 중립이므로 ko-kr을 사용해야 한다. 리소스는 ko만 만들어도 됨
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ko-KR");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ko-KR"); //251103 PSU 수정 
            }
            else if (e == EnumLanguage.Japanese)
            {
                // ja는 중립이므로 ja-JP를 사용해야 한다. 리소스는 ja만 만들어도 됨
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ja-JP"); //251103 PSU 수정 
            }
            else if (e == EnumLanguage.Chinese)
            {
                // zh-CHS는 중립이므로 zh-CN을 사용
               // Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("zh-CN"); //251103 PSU 수정 
            }
            else if (e == EnumLanguage.English)
            {
                // en은 중립
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US"); //251103 PSU 수정 
            }
            else if (e == EnumLanguage.Vietnamese)
            {
                // en은 중립
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("vi-VN"); //251103 PSU 수정 
            }
            else if (e == EnumLanguage.Russian)
            {
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU"); //251103 PSU 수정 
            }
            else
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture; //251119 PSU 추가 
            }
        }

		public static void ChangeUICulture()
		{
            ChangeUICulture(false);
		}
	}
}
