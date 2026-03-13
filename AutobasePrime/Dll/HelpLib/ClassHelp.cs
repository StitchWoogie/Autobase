using System;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using AutoLibLocal;
using NetTools;

namespace HelpLib
{
	/// <summary>
	/// Summary description for ClassHelp.
	/// </summary>
	public class ClassHelp
	{
		public ClassHelp()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static string GetFitLanguageName()
        {
            string sLan = ConfigHelp.sLanguage;

            if (String.Compare(ConfigHelp.sLanguage, "Auto", true) == 0)
            {
                if (Tools.IsLangKorean())
                {
                    sLan = "Korean";
                }
                else if (Tools.IsLangJapanese())
                {
                    sLan = "Japanese";
                }
                else if (Tools.IsLangVietnamese())
                {
                    sLan = "Vietnamese";
                }
                else
                {
                    sLan = "English";
                }
            }

            return sLan;
        }

        public static string GetHelpFolder(string language)
        {
            string directory;

            string default_path = String.Format("C:\\AutoBase\\Help\\{0}", language);

            directory = TotalConfig.LoadRegAutoBaseConfig(Registry.CurrentUser, "Help", null, "Path"+language, default_path);

            return directory;
        }

        static string MakeHelpPath(string file)
        {
            string sLan = GetFitLanguageName();

            string path = String.Format("{0}\\{1}", GetHelpFolder(sLan), file);

            return path;
        }

        /// <summary>
        /// 언어가 바뀌면 이전에 도움말을 한번 실행하면 다시 시작하기 전까지 이전 도움말이 디스프레이 된다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="file"></param>
        /// <param name="keyword"></param>
        /// <param name="independent">Child로 소속되지 않고 따로 실행됨 이 경우 keyword는 사용할 수 없음</param>
        public static void ShowHelp(System.Windows.Forms.Control parent, string file, string keyword, bool independent)
        {
            string filename = ClassHelp.MakeHelpPath(file);

            if (!File.Exists(filename))
            {
                string msg;

                if (Tools.IsLangKorean())
                    msg = "도움말 파일을 찾을 수 없습니다.\n도움말을 Setup을 이용해 설치하거나\n인터넷에서 바로 설치 하실 수 있습니다.\n지금 설치할까요?";
                else
                    msg = "Help file can not be found.\nThe help file is installing directly on the Internet or installing using Setup you can.\nInstall it now?";

                if(MessageBox.Show(msg, filename, MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                FormInstallHelp.GoUpdate();

                return;
            }

            if (independent)
                System.Diagnostics.Process.Start(filename);
            else
                Help.ShowHelp(parent, filename, keyword);   // 도움말이 한번 로딩되면 그 도움말이 캐시에 남아 있는것 같음 언어를 바꾸어도 되지 않음
        }

        //20241010 PSU Form owner 추가
        public static void DialogConfigHelp(Form owner)
        {
            FormConfigHelp dialog = new FormConfigHelp();
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(owner);
        }

        /* 도움말이 각 버전별로 /Help 폴더에 위치한 경우의 도움말 10.2.1 버전까지 사용
		static string MakeHelpPath(string file)
		{
			string directory;
			string path;

			string sDirProgramm;
			sDirProgramm = Application.StartupPath;
			path = String.Format("{0}\\help\\{1}", sDirProgramm, file);
			if(File.Exists(path))	return path;

			directory = TotalConfig.LoadRegAutoBaseConfig("Help", null, "Directory", "C:\\AutoHelp");
			path = String.Format("{0}\\{1}", directory, file);

			return path;
		}

		public static void ShowHelp(System.Windows.Forms.Control parent, string file, string keyword)
		{
			string filename = ClassHelp.MakeHelpPath(file);

			if(!File.Exists(filename)) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("도움말 파일을 찾을 수 없습니다.", filename);
				else
					MessageBox.Show("Can't find help file.", filename);
				return;
			}

			Help.ShowHelp(parent, filename, keyword);
		}*/

        
	}
}
