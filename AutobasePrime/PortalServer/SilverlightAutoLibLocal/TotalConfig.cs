using System;
using System.IO;
using Microsoft.Win32;
using NetTools;


namespace AutoLibLocal
{
	public enum EnumDefineMode 
	{
		MODE_RUN,
		MODE_EDIT,
	}
	/// <summary>
	/// Summary description for TotalConfig.
	/// </summary>
	public class TotalConfig
	{
		public static EnumDefineMode defineMode = EnumDefineMode.MODE_RUN;
		public static string sDirWorkProject = null;
		public static bool bClientFlag = false;
		public static bool bLocalMain = false;		// LocalMain에 의해서 실행되었다.
		//public static System.Windows.Forms.Form formMain;

        //static Version OsVersion = null;

        /*
		static TotalConfig()
		{
			//
			// TODO: Add constructor logic here
			//

            OsVersion = GetOsVersion();
            sDirWorkProject = GetProjectDirectory();
		}

        static Version GetOsVersion()
        {
            return Environment.OSVersion.Version;
        }

        public static RegistryKey GetRootRegistryKey()
        {
            // 6 = Vista
            if (OsVersion.Major >= 6) return Registry.CurrentUser;
            else
            {
                return Registry.LocalMachine;
            }
        }

		static string GetProjectDirectory()
		{
			string directory = "c:\\AutoBase";

            RegistryKey key1 = GetRootRegistryKey();
			RegistryKey key2;
			object obj;

			key2 = key1.OpenSubKey("Software\\AutoBase\\Work Project");

			if(key2 != null) 
			{
				obj = key2.GetValue("Directory");
				if(obj != null) 
				{
					directory = obj.ToString();
				}
				key2.Close();
			}
			key1.Close();

			directory = directory.TrimEnd('\\');

			return directory;
		}

		public static string GetConfigAscii(string work_dir, string section, string item, string default_val)
		{
			string filename;
			string dsn="";

			filename = String.Format("{0}\\Config\\ConfigA.ini", work_dir);
			Profile.GetPrivateProfileString(section, item, default_val, ref dsn, filename);

			return dsn;
		}

		public static void SetConfigAscii(string work_dir, string section, string item, string val)
		{
			string filename;

			filename = String.Format("{0}\\Config\\ConfigA.ini", work_dir);
			Profile.WritePrivateProfileString(section, item, val, filename);
		}

        public static string LoadRegAutoBaseConfig(RegistryKey key1, string root_name, string sub_name, string item, string default_value)
        {
            RegistryKey key2;
            object obj;
            string get_string = default_value;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\AutoBase\\{0}", root_name);
            else
                key_name = String.Format("Software\\AutoBase\\{0}\\{1}", root_name, sub_name);

            key2 = key1.OpenSubKey(key_name);
            if (key2 != null)
            {
                obj = key2.GetValue(item);
                if (obj != null)
                {
                    get_string = obj.ToString();
                }
                key2.Close();
            }
            key1.Close();

            return get_string;
        }

		public static string LoadRegAutoBaseConfig(string root_name, string sub_name, string item, string default_value)
		{
            RegistryKey key1 = GetRootRegistryKey();
            return LoadRegAutoBaseConfig(key1, root_name, sub_name, item, default_value);
		}

		public static byte[] LoadRegAutoBaseConfig(string root_name, string sub_name, string item, byte[] default_value)
		{
            RegistryKey key1 = GetRootRegistryKey();
			RegistryKey key2;
			object obj = null;

			string key_name;
			
			if(sub_name == null) 
				key_name = String.Format("Software\\AutoBase\\{0}", root_name);
			else
				key_name = String.Format("Software\\AutoBase\\{0}\\{1}", root_name, sub_name);

			key2 = key1.OpenSubKey(key_name);
			if(key2 != null) 
			{
				obj = key2.GetValue(item);
				if(obj != null) 
				{
					//get_string = obj.ToString();
				}
				key2.Close();
			}
			key1.Close();

			if(obj == null)	return default_value;
			else 
			{
				if(obj.GetType() == typeof(byte[]))
					return (byte[])obj;
				else
					return default_value;
			}
		}

        public static void SaveRegAutoBaseConfig(RegistryKey key1, string root_name, string sub_name, string item, object set_value)
        {
            if (set_value == null) return;	// 값이 설정되어 있지 않을 때는 return한다.

            RegistryKey key2;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\AutoBase\\{0}", root_name);
            else
                key_name = String.Format("Software\\AutoBase\\{0}\\{1}", root_name, sub_name);

            key2 = key1.CreateSubKey(key_name);
            if (key2 != null)
            {
                // DWORD로 저장하고 싶으면 int로 변수를 보내야 한다.
                key2.SetValue(item, set_value);
                key2.Close();
            }
            key1.Close();
        }

		public static void SaveRegAutoBaseConfig(string root_name, string sub_name, string item, object set_value)
		{
			if(set_value == null)	return;	// 값이 설정되어 있지 않을 때는 return한다.
            RegistryKey key1 = GetRootRegistryKey();
            SaveRegAutoBaseConfig(key1, root_name, sub_name, item, set_value);
		}

		public static int LoadRegAutoBaseConfig(string root_name, string sub_name, string item, int default_value)
		{
			string val = LoadRegAutoBaseConfig(root_name, sub_name, item, default_value.ToString());

			return ConvertTool.ToInt32(val);
		}

        public static int LoadRegAutoBaseConfig(RegistryKey key1, string root_name, string sub_name, string item, int default_value)
        {
            string val = LoadRegAutoBaseConfig(key1, root_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToInt32(val);
        }

		public static bool LoadRegAutoBaseConfig(string root_name, string sub_name, string item, bool default_value)
		{
			string val = LoadRegAutoBaseConfig(root_name, sub_name, item, default_value.ToString());

			return ConvertTool.ToBoolean(val);
		}

		public static uint LoadRegAutoBaseConfig(string root_name, string sub_name, string item, uint default_value)
		{
			string val = LoadRegAutoBaseConfig(root_name, sub_name, item, default_value.ToString());

			return ConvertTool.ToUInt32(val);
		}

		public static string LoadRegAutoBaseProjectConfig(string work_dir, string item, string default_value)
		{
			return LoadRegAutoBaseConfig("Project", work_dir, item, default_value);
		}

		public static bool LoadRegAutoBaseProjectConfig(string work_dir, string item, bool default_value)
		{
			return LoadRegAutoBaseConfig("Project", work_dir, item, default_value);
		}

		public static int LoadRegAutoBaseProjectConfig(string work_dir, string item, int default_value)
		{
			return LoadRegAutoBaseConfig("Project", work_dir, item, default_value);
		}

		public static void SaveRegAutoBaseProjectConfig(string work_dir, string item, object set_value)
		{
			SaveRegAutoBaseConfig("Project", work_dir, item, set_value);
		}

        public static string sWebServiceDataDirBasic = null;   // 웹 서비스에서 설정된
        public static string sWebServiceDataDirLog = null;

        public static string GetProjectDataDirectory(string work_dir)
        {
            string directory = "";

            if (ConfigVarTotal.bRunByWebService && sWebServiceDataDirBasic != null)
                directory = sWebServiceDataDirBasic;
            else
                directory = LoadRegAutoBaseProjectConfig(work_dir, "DirData", "C:\\CATDATA");

            return directory;
        }

		public static string GetProjectDataDirectory()
		{
			return GetProjectDataDirectory(TotalConfig.sDirWorkProject);
		}

        public static string GetProjectDataLogDirectory(string work_dir)
        {
            string directory = "";

            if (ConfigVarTotal.bRunByWebService && sWebServiceDataDirLog != null)
                directory = sWebServiceDataDirLog;
            else
                directory = LoadRegAutoBaseProjectConfig(work_dir, "DirDataLog", "C:\\CATDATA");

            return directory;
        }

		public static string GetProjectDataLogDirectory()
		{
			return GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);
		}

		public static void SetProjectDataDirectory(string work_dir, string dir)
		{
			SaveRegAutoBaseProjectConfig(work_dir, "DirData", dir);
		}

		public static void SetProjectDataLogDirectory(string work_dir, string dir)
		{
			SaveRegAutoBaseProjectConfig(work_dir, "DirDataLog", dir);
		}

		static string KillEndSlash(string buf)
		{
			if(buf.Length < 3)	return buf;
			int hap = buf.Length;
			if(buf[hap-1] == '\\' && buf[hap-2] != ':') 
			{
				return buf.Substring(0, hap-1);
			}
			return buf;
		}

		public static string AutoBaseIniGetOemSupervisorName()
		{
		
			return "ADMIN";
		}

		public static string AutoBaseIniGetOemProgramName()
		{
			return TotalConfig.LoadRegAutoBaseConfig("OEM", null, "program name", "AutoBase");
		}

		public static string AutoBaseIniGetOemCompanyName()
		{
			
			return TotalConfig.LoadRegAutoBaseConfig("OEM", null, "Company name", NetTools.Tools.IsLangKorean() ? "(주)오토베이스" : "Hansol Tech");
		}

		public static bool GetAutoBaseTestMode()
		{
			string buf = LoadRegAutoBaseConfig("Run", null, "TestMode", "ON");

			if(buf == "ON") 
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		const string keyTestMode = "TestMode";

		public static void SetAutoBaseTestMode(bool mode)
		{
			if(mode)
				SaveRegAutoBaseConfig(sectionRun, null, keyTestMode, "ON");	
			else
				SaveRegAutoBaseConfig(sectionRun, null, keyTestMode, "OFF");	
		}

		public static TextReader OpenOldNew(string sub_dir, string ascii_file, string unicode_file)
		{
			string filename = String.Format("{0}\\{1}\\{2}", sDirWorkProject, sub_dir, unicode_file);

			if(File.Exists(filename)) 
			{
				return new StreamReader(filename);
			}
			else 
			{
				filename = String.Format("{0}\\{1}\\{2}", sDirWorkProject, sub_dir, ascii_file);	
				if(File.Exists(filename)) 
				{
					return new StreamReader(filename, System.Text.Encoding.Default);
				}
			}

			return null;
		}

		public static string FileOldNew(string sub_dir, string ascii_file, string unicode_file)
		{
			string filename = String.Format("{0}\\{1}\\{2}", sDirWorkProject, sub_dir, unicode_file);

			if(!File.Exists(filename)) {
				string filename2 = String.Format("{0}\\{1}\\{2}", sDirWorkProject, sub_dir, ascii_file);	
				if(File.Exists(filename2))
					return filename = filename2;
			}

			return filename;
		}

		public static void AutoBaseMainListCtrlConfigLoad(ListView m_list, string section)
		{
			AutoBaseListCtrlConfigLoad(m_list, "AutoBaseMain", section);
		}

		public static void AutoBaseListCtrlConfigLoad(ListView m_list, string filename, string section)
		{
			string cfg_dir;
			string filepath;
			string title = "";
			int val = 0;
			int i;
			ColumnHeader col;
			CommaBlockString comma = new CommaBlockString();
	
			cfg_dir = AutoBaseIniGetConfigDirectory();
			filepath = String.Format("{0}\\ListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

			if(!File.Exists(filepath))	return;

			string buf;

			TextReader reader = new StreamReader(filepath);
			if(reader == null)	return;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				comma.Set(buf);
				comma.GetString(ref title);

				for(i = 0; i < m_list.Columns.Count; i++) 
				{
					col = m_list.Columns[i];
					if(title == col.Text) 
					{
						comma.GetInt(ref val);			
						if(val != 0) 
						{
							if(val > 1000)	val = 50;
							if(val < 10)	val = 10;
							col.Width = val;
						}
						break;
					}
				}
			}
			reader.Close();
		}


		public static void AutoBaseListCtrlConfigLoad(ControlListView m_list, string filename, string section)
		{
			string cfg_dir;
			string filepath;
			string title = "";
			int val = 0;
			int i;
			ControlListViewHeader col;
			CommaBlockString comma = new CommaBlockString();
	
			cfg_dir = AutoBaseIniGetConfigDirectory();
			filepath = String.Format("{0}\\ListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

			if(!File.Exists(filepath))	return;

			string buf;

			TextReader reader = new StreamReader(filepath);
			if(reader == null)	return;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				comma.Set(buf);
				comma.GetString(ref title);

				for(i = 0; i < m_list.header.Count; i++) 
				{
					col = (ControlListViewHeader)m_list.header[i];
					if(title == col.text) 
					{
						comma.GetInt(ref val);			
						if(val != 0) 
						{
							if(val > 1000)	val = 50;
							if(val < 10)	val = 10;
							col.width = val;
						}
						break;
					}
				}
			}
			reader.Close();
		}

		public static void AutoBaseListCtrlConfigLoad(ref int[] length, string filename, string section)
		{
			if(length == null || length.Length <= 0) return;

			string cfg_dir;
			string filepath;
			int val = 0;
			CommaBlockString comma = new CommaBlockString();
	
			cfg_dir = AutoBaseIniGetConfigDirectory();
			filepath = String.Format("{0}\\ListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

			if(!File.Exists(filepath))	return;

			string buf;

			TextReader reader = new StreamReader(filepath);
			if(reader == null)	return;

			int			pos = 0;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				comma.Set(buf);
				comma.GetInt(ref val);
				if(val != 0) 
				{
					if(val > 1000)	val = 50;
					if(val < 10)	val = 10;
					length[pos] = val;
				}
				pos ++;
				if(pos >= length.Length) break;
			}
			reader.Close();
		}

		public static string AutoBaseIniGetConfigDirectory()
		{
			string path = Application.LocalUserAppDataPath;
			path = path.Substring(0, path.Length-Application.ProductVersion.Length);
			path += "Config";

			Directory.CreateDirectory(path);

			return path;
		}

		public static void AutoBaseMainListCtrlConfigSave(ListView m_list, string section)
		{
			AutoBaseListCtrlConfigSave(m_list, "AutoBaseMain", section);
		}

		public static void AutoBaseListCtrlConfigSave(ListView m_list, string filename, string section)
		{
			string cfg_dir;
			string filepath;
			int i;
			ColumnHeader col;
			CommaBlockString comma = new CommaBlockString();
	
			cfg_dir = AutoBaseIniGetConfigDirectory();

			filepath = String.Format("{0}\\ListCtrlConfig", cfg_dir);
			Directory.CreateDirectory(filepath);
			filepath = String.Format("{0}\\ListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

			TextWriter writer = new StreamWriter(filepath);
			if(writer == null)	return;
			
			for(i = 0; i < m_list.Columns.Count; i++) 
			{
				col = m_list.Columns[i];
				writer.WriteLine("{0},{1},", col.Text, col.Width);
			}

			writer.Close();
			
		}

		public static void AutoBaseListCtrlConfigSave(ControlListView m_list, string filename, string section)
		{
			string cfg_dir;
			string filepath;
			int i;
			ControlListViewHeader col;
			CommaBlockString comma = new CommaBlockString();
	
			cfg_dir = AutoBaseIniGetConfigDirectory();

			filepath = String.Format("{0}\\ListCtrlConfig", cfg_dir);
			Directory.CreateDirectory(filepath);
			filepath = String.Format("{0}\\ListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

			TextWriter writer = new StreamWriter(filepath);
			if(writer == null)	return;
			
			for(i = 0; i < m_list.header.Count; i++) 
			{
				col = (ControlListViewHeader)m_list.header[i];
				writer.WriteLine("{0},{1},", col.text, col.width);
			}

			writer.Close();
			
		}

		public static void AutoBaseListCtrlConfigSave(ref int[] length, string filename, string section)
		{
			if(length == null || length.Length <= 0) return;

			string cfg_dir;
			string filepath;
			int i;
			CommaBlockString comma = new CommaBlockString();
	
			cfg_dir = AutoBaseIniGetConfigDirectory();

			filepath = String.Format("{0}\\ListCtrlConfig", cfg_dir);
			Directory.CreateDirectory(filepath);
			filepath = String.Format("{0}\\ListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

			TextWriter writer = new StreamWriter(filepath);
			if(writer == null)	return;
			
			for(i = 0; i < length.Length; i++) 
			{
				writer.WriteLine("{0},", length[i]);
			}

			writer.Close();
			
		}

		public static int AutoBaseIniGetKeyLockWait()
		{
			int val = LoadRegAutoBaseConfig("KeyLock", null, "Wait", 0);
			return val;
		}

		const string sectionRun =	  "Run";
		const string keyEditEnable = "EditEnable";

		public static bool GetAutoBaseEditEnable()
		{
			string buf = LoadRegAutoBaseConfig(sectionRun, null, keyEditEnable, "ON");
			
			if(String.Compare(buf, "ON") == 0) 
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		public static void SetAutoBaseEditEnable(bool mode)
		{
			if(mode)
				SaveRegAutoBaseConfig(sectionRun, null, keyEditEnable, "ON");
			else
				SaveRegAutoBaseConfig(sectionRun, null, keyEditEnable, "OFF");
		}

		public static bool AutoBaseIniGetPlcScanWriteTest()
		{
			int val;
			val = LoadRegAutoBaseConfig("PlcScan", null, "bPlcScanWriteTest", 1);

			if(val == 1)	return true;
			else			return false;
		}

		public static void AutoBaseIniSetPlcScanWriteTest(bool state)
		{
			SaveRegAutoBaseConfig("PlcScan", null, "bPlcScanWriteTest", state ? 1 : 0);
		}*/
	}
}

