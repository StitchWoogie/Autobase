using System;
using AutoLibLocal;
using NetTools;
using System.Windows.Forms;

namespace LocalMain
{
	/// <summary>
	/// Summary description for C_mail.
	/// </summary>
	public class C_mail
	{
		public C_mail()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//------------------------------------------------------------------------------
		//	사용자의 메일개수를 읽어온다.
		//------------------------------------------------------------------------------

		public static int GetMyMailCount(string username)
		{
			string filename;

			filename = String.Format("{0}\\user\\{1}\\mail*.txt", TotalConfig.AutoBaseIniGetConfigDirectory(), username);

			return Tools.GetFileHap(filename);
		}

		//------------------------------------------------------------------------------
		//	사용자에게 온 메일을 읽어본다.
		//------------------------------------------------------------------------------

		public static void ReadMail()
		{
			Mail.FormReadMail dialog = new LocalMain.Mail.FormReadMail();

            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(FormLocalMain.formMain);
		}

		public static void SendMail()
		{
			Mail.FormSendMail dialog = new LocalMain.Mail.FormSendMail();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(FormLocalMain.formMain);
		}
	}
}

