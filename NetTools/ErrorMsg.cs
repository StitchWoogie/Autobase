using System;
using System.Windows.Forms;

namespace NetTools
{
	/// <summary>
	/// Summary description for ErrorMsg.
	/// </summary>
	public class ErrorMsg
	{
		public ErrorMsg()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static string MakeAdditionalString(Exception exception)
		{
			string msg = "\n\nError Message:";
			msg+=exception.Message;
			msg += "\n\nStack Trace:\n";
			msg += exception.StackTrace;
			return msg;
		}

		public static void Show(Exception exception, string text, string caption)
		{
			string msg = MakeAdditionalString(exception);
			MessageBox.Show(text+msg, caption);
		}
	}
}
