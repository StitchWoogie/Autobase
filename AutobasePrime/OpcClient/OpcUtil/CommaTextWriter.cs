using System;
using System.IO;

namespace NetTools
{
	/// <summary>
	/// Summary description for CommaTextWriter.
	/// </summary>
	public class CommaTextWriter
	{
		TextWriter writer;

		public CommaTextWriter(string filename)
		{
			//
			// TODO: Add constructor logic here
			//

			writer = new StreamWriter(filename);
		}

		public CommaTextWriter(string filename, System.Text.Encoding encoding)
		{
			//
			// TODO: Add constructor logic here
			//

			writer = new StreamWriter(filename, false, encoding);
		}

		public CommaTextWriter(System.IO.Stream stream)
		{
			//
			// TODO: Add constructor logic here
			//

			writer = new StreamWriter(stream);
		}

		public void Close()
		{
			if(writer != null)
				writer.Close();
		}

		public static bool IsExistBlockCode(string text)
		{
			for(int i = 0; i < text.Length; i++) 
			{
				if(text[i] == '"' || text[i] == ',') 
				{
					return true;
				}
			}

			return false;
		}

		public static string MakeString(string text) 
		{
			string make = "\"";
			for(int i = 0; i < text.Length; i++) 
			{
				if(text[i] == '"')
				{
					make += text[i];
				}

				make += text[i];
			}	
			make += '"';

			return make;
		}

		public void WriteLine()
		{
			writer.WriteLine();
		}

		public void WriteLine(string format, params object[] args)
		{
			for(int i = 0; i < args.Length; i++) 
			{
				if(args[i] != null && args[i].GetType() == Type.GetType("System.String")) 
				{
					if(IsExistBlockCode((string)args[i]))
						args[i] = MakeString((string)args[i]);
				}	
			}

			writer.WriteLine(format, args);
		}

		public void Write(string format, params object[] args)
		{
			for(int i = 0; i < args.Length; i++) 
			{
				if(args[i] != null && args[i].GetType() == Type.GetType("System.String")) 
				{
					if(IsExistBlockCode((string)args[i]))
						args[i] = MakeString((string)args[i]);
				}	
			}

			writer.Write(format, args);
		}

		public void Write(char val)
		{
			writer.Write(val);
		}
	}
}
