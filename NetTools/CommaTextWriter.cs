using System;
using System.IO;
using System.Text;

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

        public CommaTextWriter(System.IO.Stream stream, System.Text.Encoding encoding)
        {
            //
            // TODO: Add constructor logic here
            //
            writer = new StreamWriter(stream, encoding);
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
            StringBuilder make = new StringBuilder("\"");
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                {
                    make.Append(text[i]);
                }

                make.Append(text[i]);
            }
            make.Append('"');

            return make.ToString();

			/*
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

			return make;*/
		}

		public void WriteLine()
		{
			writer.WriteLine();
		}

        /// <summary>
        /// 앞쪽에 탭을 넣어서 소스를 보기좋게 정렬한다.
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLineWithTab(int tab, string format, params object[] args)
        {
            if (tab > 0)
            {
                StringBuilder s = new StringBuilder();
                for (int i = 0; i < tab; i++)
                {
                    s.Append('\t');
                }
                writer.Write(s.ToString());
            }

            WriteLine(format, args);
        }

        // " 이나 , 가 있는 경우 " " 로 보호한다.
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

            writer.WriteLine(String.Format(CultureTool.ciKR, format, args));   // 기본형식으로 저장한다. 2018-3-23 변경 //writer.WriteLine(format, args);
			
		}

        /*
        // 저장은 CommaTextWriter를 사용하고 불러오기는 TextReader를 사용하면 저장시 " 를 저장하면 계속 """가 붙어서 엄청나게 커진다.
        // TextReader 로 읽는 부분은 TextWriter로 저장해야 한다.
        public void WriteLineOriginal(string format, params object[] args)
        {
            writer.WriteLine(format, args);
        }*/

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

            writer.Write(String.Format(CultureTool.ciKR, format, args));    // 기본형식으로 저장한다. 2018-3-23 변경 //writer.Write(format, args);
		}

		public void Write(char val)
		{
			writer.Write(val);
		}

        public void Flush()
        {
            writer.Flush();
        }
	}
}
