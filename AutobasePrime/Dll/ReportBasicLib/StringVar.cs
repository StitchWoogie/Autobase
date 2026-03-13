using System;
using System.Collections;
using AutoLibLocal;
using System.IO;
using NetTools;
using AutoLib;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for StringVar.
	/// </summary>
	public class StringVar
	{
		public static ArrayList blockStringVar = new ArrayList(); 

		static StringVar()
		{
			//
			// TODO: Add constructor logic here
			//
			LoadStringVars();
		}

		static void LoadStringVars()
		{
			string filename;
			TextReader reader;
			string buf;
			CommaBlockString comma = new CommaBlockString();
			STRING_VAR_STRUCT var;

			filename = String.Format("{0}\\report\\StrVars.lstx", TotalConfig.sDirWorkProject);
			if(File.Exists(filename)) 
			{
				reader = new StreamReader(filename);		
			}
			else 
			{
				filename = String.Format("{0}\\report\\StrVars.lst", TotalConfig.sDirWorkProject);
				if(!File.Exists(filename))	return;
				reader = new StreamReader(filename, System.Text.Encoding.Default);
			}
			if(reader == null)	return;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				var = new STRING_VAR_STRUCT();
				comma.Set(buf);
				comma.GetString(ref var.name);
				comma.GetStringTotalRemain(ref var.val);
				if(var.name.Length == 0)	continue;
				blockStringVar.Add(var);
			}
			reader.Close();
		}

		public static void SaveStringVars()
		{
			string filename;
			TextWriter writer;
			STRING_VAR_STRUCT var;

			filename = String.Format("{0}\\report", TotalConfig.sDirWorkProject);
			Directory.CreateDirectory(filename);

			filename = String.Format("{0}\\report\\StrVars.lstx", TotalConfig.sDirWorkProject);

			writer = new StreamWriter(filename);		

			for(int i = 0; i < blockStringVar.Count; i++) 
			{
				var = (STRING_VAR_STRUCT)blockStringVar[i];

				writer.WriteLine("{0},{1}", var.name, var.val);
			}
			writer.Close();
		}

        static bool GetStringVarFromStringArray(string name, out string val)
        {
            name = "ReportVar-" + name;

            if (ConfigVarTotal.varKeys == null)
            {
                val = "";
                return false;
            }

            for (int i = 0; i < ConfigVarTotal.varKeys.Length; i++)
            {
                // 로컬 레지스트리나 세션은 대소문자 구분이 없다.
                if (String.Compare(ConfigVarTotal.varKeys[i], name, true) == 0)
                {
                    val = ConfigVarTotal.varValues[i];
                    return true;
                }
            }

            val = "";
            return false;
        }

        static bool GetStringVarFromSession(string name, out string val)
        {
            if (ConfigVarTotal.varKeys != null)
            {
                return GetStringVarFromStringArray(name, out val);
            }

            val = "";

            object obj = ConfigVarTotal.GetFromSession("ReportVar-"+name);

            if (obj == null)
            {
                return false;
            }

            val = Convert.ToString(obj);

            return true;
        }

		public static bool GetStringVar(string name, out double val)
		{
            val = 0;

            if (ConfigVarTotal.bRunByWebService)
            {
                string s;
                if (GetStringVarFromSession(name, out s))
                {
                    val = ConvertTool.ToDouble(s);
                    return true;
                }
                return false;
            }

			STRING_VAR_STRUCT var;
			int l;

			for(l = 0; l < blockStringVar.Count; l++) 
			{
				var = (STRING_VAR_STRUCT)blockStringVar[l];
				if(String.Compare(var.name, name, true) == 0) 
				{
					val = ConvertTool.ToDouble(var.val);
					return true;
				}
			}
            
			return false;
		}

		public static bool GetStringVar(string name, out string str)
		{
            if (ConfigVarTotal.bRunByWebService)
            {
                return GetStringVarFromSession(name, out str);
            }

			STRING_VAR_STRUCT var;
			int l;

			for(l = 0; l < blockStringVar.Count; l++) 
			{
				var = (STRING_VAR_STRUCT)blockStringVar[l];
				if(String.Compare(var.name, name, true) == 0) 
				{
					str = var.val;
					return true;
				}
			}
            str = "";
			return false;
		}


		public static bool SetStringVar(string name, string val)
		{
            if (!ConfigVarTotal.bLocalFlag)
            {
                if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 3, 2, 1))
                {
                    ServiceLib.dicVars["ReportVar-" + name] = val;
                }
                else
                {
                    var service = new ReportBasicLib.ServiceReferenceReport.ServiceReportClient();
                    //service.Url = AutoLibLocal.ConfigVarTotal.GetServicePath("ServiceReport.asmx");
                   // service.CookieContainer = ConfigVarTotal.cookieContainer;
                    
                    service.ReportSetVar("ReportVar-"+name, val);
                }
            }
			
            STRING_VAR_STRUCT var;
			int l;

			for(l = 0; l < blockStringVar.Count; l++) 
			{
				var = (STRING_VAR_STRUCT)blockStringVar[l];
				if(String.Compare(var.name, name, true) == 0) 
				{
					var.val = val;
					return true;
				}
			}
			return false;
		}

		
	}

	public class STRING_VAR_STRUCT 
	{
		public string name;
		public string val;
	}
}
