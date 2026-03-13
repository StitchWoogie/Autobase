using System;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for CommaScriptFileModelScript.
	/// </summary>
	public class CommaScriptFileModelScript : CommaScriptFileClass
	{
		public CommaScriptFileModelScript()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string str = "";

		protected override void OnCommand(string command, NetTools.CommaBlockString comma)
		{
			base.OnCommand (command, comma);

			if(str.Length > 0) 
			{
				str += "\r\n";
			}

			str += bufOneLine;
		}

	}


}
