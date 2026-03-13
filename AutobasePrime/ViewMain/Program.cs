using System;
using System.Collections.Generic;
using System.Text;
using AutoLibLocal;
using Microsoft.Win32;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace ViewMain
{
    /// <summary>
    /// 웹상에서 실행 시 보안체크에 걸리게 되는데 Main이 Form속에 선언되거나 다른 클래스 파일에 선언되어도 보안 오류를
    /// 체크하지 못한다.
    /// 클래스를 따로빼고 Form의 아래부분에 다음과 같이 사용하여야 보안 오류가 체크된다.
    /// </summary>
    /// 
    class Program
    {
        public static string[] mainArgs;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static void MainCall(string[] args)
        {

#if USE_EXCEPTION_REPORT
			try 
			{
#endif
            LanguageTool.ChangeUICulture(true);

            mainArgs = args;

            FormViewMain form = new FormViewMain();
            if (!form.bErrorSecurity)
                System.Windows.Forms.Application.Run(form);
#if USE_EXCEPTION_REPORT
			}
			catch (Exception exception)
			{
				ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);
				dialog.ShowDialog();
				Process.GetCurrentProcess().Kill();
			}
#endif

        }

        [STAThread]
        static void Main(string[] args)
        {

            if (!CheckSecurity()) return;

            // Main 함수가 호출되면 내부에 선언된 모든 클래스의 Dll이 로딩되는 것 같아서 따로 뺐다.
            // Dll이 로딩되는 것으로도 보안에 걸리는 것 같다.
            MainCall(args);
        }

        static bool CheckSecurity()
        {
            try
            {
                RegistryKey key1 = Registry.CurrentUser;
                RegistryKey key2;

                string key_name;

                key_name = String.Format("Software\\AutoBase\\TestSecurity");

                key2 = key1.CreateSubKey(key_name);
                if (key2 != null)
                {
                    // DWORD로 저장하고 싶으면 int로 변수를 보내야 한다.
                    key2.SetValue("TestItem", "Security test O.K");
                    key2.Close();
                }
                key1.Close();
                return true;
            }
            catch
            {
                if (IsLangKorean())
                    System.Windows.Forms.MessageBox.Show("ViewMain.exe에 대한 보안 설정을 완전 신뢰로 설정하여야 합니다.\n프로그램을 종료합니다.", "보안 오류");
                else if (IsLangChinese())
                    System.Windows.Forms.MessageBox.Show("将ViewMain.exe安全设置设置成完全信任。\n退出程序。", "安全错误");
                else
                    System.Windows.Forms.MessageBox.Show("Security level is too low (Set to Full trust)", "ViewMain.exe Security error");

                return false;
            }
        }

        static bool IsLangKorean()
        {
            //CultureInfo info = Thread.CurrentThread.CurrentUICulture;
            CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //20251111 PSU 수정

            if (String.Compare(info.Name, "ko", true) == 0)
                return true;
            if (String.Compare(info.Name, "ko-kr", true) == 0)
                return true;

            return false;
        }

        static bool IsLangChinese()
        {
           // CultureInfo info = Thread.CurrentThread.CurrentUICulture;
            CultureInfo info = CultureInfo.DefaultThreadCurrentUICulture; //20251111 PSU 수정

            if (String.Compare(info.Name, 0, "zh-", 0, 3, true) == 0)
                return true;

            return false;
        }
    }
}
