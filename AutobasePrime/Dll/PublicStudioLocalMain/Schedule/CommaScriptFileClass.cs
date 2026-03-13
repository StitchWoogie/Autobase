using System;
using System.IO;
using NetTools;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for CommaScriptFileClass.
	/// </summary>
	public class CommaScriptFileClass
	{
		public CommaScriptFileClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected TextReader in_pointer;
		protected string bufOneLine;

		public bool Execute(TextReader reader, string start_command, CommaBlockString comma_start)
		{
			in_pointer = reader;
			string buf;
			string command="";
			CommaBlockString comma = new CommaBlockString();

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				bufOneLine = buf;
				comma.Set(buf);
				comma.GetString(ref command);
				if(start_command == command) 
				{
					comma.GetString(ref command);
					if(String.Compare(command, "End", true) == 0) 
					{
						return true;
					}
				}
				else 
				{
					OnCommand(command, comma);
				}
			}
			return false;
		}

		protected virtual void OnCommand(string command, CommaBlockString comma) 
		{
		}
	}
}
