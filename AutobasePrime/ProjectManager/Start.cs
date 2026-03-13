using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using NetTools;
using AutoLibLocal;
using System.Diagnostics;

/// 실행파일이 너무 작아서 바이러스 처리가 되어 UnUsedCode 를 만들어서 사용하지 않는 코드를 포함 시키니 바이러스로 오진하지 않는듯 하다.
/// 실행 파일이 적당히 커야 할 듯 하다. 2016-4-20 
/// Main 사이 사이에 코드를 넣으면 상황은 좋아지나 백신에 따라서 오진하는 경우도 있다.

namespace ProjectSelect
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class StartMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
#if USE_EXCEPTION_REPORT
			try 
			{
#endif
			LanguageTool.ChangeUICulture();
			bool createdNew=false;
			System.Threading.Mutex gM1 = new System.Threading.Mutex(true,"AutoBaseProjectManager", out createdNew);

			if (createdNew) 
			{
				Application.Run(new FormSelectProject());
			}
			else 
			{
				Process p = Tools.GetPreviousProcess();

				if(p != null && p.MainWindowHandle != IntPtr.Zero) 
				{
					Win32Function.SetForegroundWindow(p.MainWindowHandle);
				}
			}
#if USE_EXCEPTION_REPORT
			}
			catch (Exception exception)
			{
				ExceptionReport.FormExceptionReport dialog = new ExceptionReport.FormExceptionReport(exception);

				dialog.ShowDialog();
			}
#endif
		}
	}
}
