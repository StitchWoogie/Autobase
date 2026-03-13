using System;
using System.IO;
using Microsoft.Win32;
using NetTools;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using NetTools.Hash;
using System.Threading;

namespace AutoLibLocal
{
	public enum EnumDefineMode 
	{
		MODE_RUN,
		MODE_EDIT,
	}

    // 고유한 번호이므로 순서를 임의대로 바꾸면 안된다. Modx 파일에 고유 Oem번호를 저장한다.
    public enum EnumOemType
    {
        Normal = 0,
        ZIoT = 1,       // KD POWER Z loT
        UYeG_GS = 2,    // 아이티공간 UYeG GS_인증용
        OPEN_SCADA = 3, // 종성 OPEN SCADA
        SBAS = 4,       // (주)삼원씨앤지 셈스 SAMS (Samwon Automation Management Software) -> SBAS(Samwon Building Automation System Software), Ver.10.2
        FiveTek = 5,    // (주)파이브텍 통합관리시스템 2016-4-11,  2921-9-9 인공지능 기반 바이오가스 통합.
        UYeG_Normal = 6,// 아이티공간 UYeG 버전이나 오토베이스와 같은 메뉴를 가진다.
        MBSENGSCADA = 7,    // (주)엠비에스 엔지니어링
        KobasAI = 8,    // (주)코젠 OEM 2024-4-11 개발 시작.
        SCADA_LITE = 9,    // 24-12-12 추가. Soft 키락에도 적용되었으나 SCADA Lite 전용 배포파일을 사용할 때 이 Type을 사용한다.  
    }

    public enum EnumOemTypeRight
    {
        Report = 1,
        Database = 2,
        ExcelReport = 3,
        MilliData = 4,
        WebServer = 5,
        OnOffList = 6,
        Http = 7,  //20250206 PSU 추가
        Json = 8,
    }

    // OEM 전용키락의 번호를 지정한다. 고유한 번호이므로 순서를 임의대로 바꾸면 안된다. 
    // 이것은 키락이 서로 달라서 인식못하는 키락인 경우이다.
    public enum EnumOemKeylockType
    {
        // 24-12-12 부터 POEM과 같이 번호를 사용한다.
        Normal = 0,
        SCADA = 65,      // 위의 0번과 같다.
        UYeG = 66,       // 아이티공간 UYeG_GS 와 UYeG_Normal 에서 함께 사용
        ScadaLite = 67,  // 24-12-11 SCADA Lite용으로 추가했다. SoftLock에서만 사용한다.  10.3.6.23 버전부터 이키락을 인식한다.

        // 24-12-12 이전 define
        //Normal = 0,
        //UYeG = 2,       // 아이티공간 UYeG_GS 와 UYeG_Normal 에서 함께 사용
        //ScadaLite = 3,  // 24-12-11 SCADA Lite용으로 추가했다. SoftLock에서만 사용한다.
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
		public static System.Windows.Forms.Form formMain;
        public static System.Threading.Thread threadMain;

        // 현재 스래드가 메인스레인가를 검사한다.
        public static bool IsMainThread()
        {
            return (threadMain == Thread.CurrentThread);
        }

        public static string sCurrentComputer;      // 현재의 컴퓨터 이름

        static Version OsVersion = null;

        public static bool bVersionCertificationSmallBusinessProducts = false;    // 중소기업 제품 인증용 특별 버전 10.2 로 표기되어야 한다. true로 하고 컴파일한다.

        public static EnumOemType eOemType = EnumOemType.Normal;
        public static EnumOemKeylockType eOemKeylockType = EnumOemKeylockType.Normal;

        public static string sOemRootFolder;                                            // 기본 OEM 폴더명을 말한다. 일반적으로 AutoBase이지만 각 Oem별로 폴더명을 달리할 수도 있다.
        public static readonly string sRootRegistryName = "AutoBase";                   // 기본 레지스트리 ROOT 명을 말한다. 이것을 바꾸면 통신 DLL등 모든 관련파일을 컴파일해 주어야 한다.
                                                                                        // 바꾸는 것은 좋지 않다.
                                                                                        
        static string sOemRegistryName = "AutoBase";                                    // OEM이 적용된 레지스트리. 호환성을 위해서 새로 생기는 환경변수는 이것을 이용해서 저장해도 된다.

		static TotalConfig()
		{
			//
			// TODO: Add constructor logic here
			//

            OsVersion = GetOsVersion();

            string oem_type = AutoBaseIniGetOemType();
            if (oem_type == "ZIoT")
                eOemType = EnumOemType.ZIoT;
            else if (oem_type == "U-YE-G")
                eOemType = EnumOemType.UYeG_GS;
            else if (oem_type == "U-YE-G Normal")
                eOemType = EnumOemType.UYeG_Normal;
            else if (oem_type == "OPEN_SCADA")
                eOemType = EnumOemType.OPEN_SCADA;
            else if (oem_type == "SBAS")
                eOemType = EnumOemType.SBAS;
            else if (oem_type == "FiveTek")
                eOemType = EnumOemType.FiveTek;
            else if (oem_type == "MBSENGSCADA")
                eOemType = EnumOemType.MBSENGSCADA;
            else if (oem_type == "KobasAI")
                eOemType = EnumOemType.KobasAI;
            else if (oem_type == "SCADA-Lite")
                eOemType = EnumOemType.SCADA_LITE;
            else
            {

            }

            if (eOemType == EnumOemType.ZIoT)
            {
                sOemRootFolder = "Z_loT_Program";
                sOemRegistryName = "Z_loT";
            }
            else if (TotalConfig.eOemType == EnumOemType.UYeG_GS || TotalConfig.eOemType == EnumOemType.UYeG_Normal)
            {
                sOemRootFolder = "UYeG";
                sOemRegistryName = "UYeG";
                eOemKeylockType = EnumOemKeylockType.UYeG;
            }
            else if (eOemType == EnumOemType.SBAS)
            {
                sOemRootFolder = "SBAS";
                sOemRegistryName = "SBAS";
            }
            else if (eOemType == EnumOemType.MBSENGSCADA)
            {
                sOemRootFolder = "MBSENGSCADA";
                sOemRegistryName = "MBSENGSCADA";
            }
            else if (eOemType == EnumOemType.KobasAI)
            {
                sOemRootFolder = "KobasAI";
                sOemRegistryName = "KobasAI";
            }
            else
            {
                sOemRootFolder = "AutoBase";
                sOemRegistryName = "AutoBase";
            }

            sDirWorkProject = GetProjectDirectory();

            try
            {
                sCurrentComputer = Dns.GetHostName();
            }
            catch   // 중국어 OS 2008R2 에서 Dns.GetHostName에서 오류가 발생하는 컴퓨터가 있다. 2013-7-1
            {
                sCurrentComputer = "Unknown";
            }
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

        public static string MakePathByWindowsDisk(string dir)
        {
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string root = Path.GetPathRoot(windir);

            string path = root + dir;
            return path;
        }

		static string GetProjectDirectory()
		{
			string directory = MakePathByWindowsDisk(sOemRootFolder+"\\Project\\Default");

            RegistryKey key1 = GetRootRegistryKey();
			RegistryKey key2;
			object obj;

            key2 = key1.OpenSubKey(String.Format("Software\\{0}\\Work Project", sRootRegistryName));

			if(key2 != null) 
			{
				obj = key2.GetValue("Directory");
				if(obj != null) 
				{
					directory = obj.ToString();
				}
				key2.Close();
			}
			//key1.Close(); root 키를 굳이 닫을 필요는 없다. 2010-4-19

			directory = directory.TrimEnd('\\');

			return directory;
		}

        /*
		public static string GetConfigAscii(string work_dir, string section, string item, string default_val)
		{
			string filename;
			string dsn="";

			filename = String.Format("{0}\\Config\\ConfigA.ini", work_dir);
			Profile.GetPrivateProfileStringA(section, item, default_val, ref dsn, filename);

			return dsn;
		}

		public static void SetConfigAscii(string work_dir, string section, string item, string val)
		{
			string filename;

			filename = String.Format("{0}\\Config\\ConfigA.ini", work_dir);
			Profile.WritePrivateProfileStringA(section, item, val, filename);
		}*/

        public static RegistryKey GetRegKeyAutoBaseConfig(string root_name, string sub_name)
        {
            RegistryKey key1 = GetRootRegistryKey();
            RegistryKey key2;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\{0}\\{1}", sRootRegistryName, root_name);
            else
                key_name = String.Format("Software\\{0}\\{1}\\{2}", sRootRegistryName, root_name, sub_name);

            key2 = key1.CreateSubKey(key_name);

            return key2;
        }

        static string LoadRegConfigBasic(string oem_root, RegistryKey key1, string root_name, string sub_name, string item, string default_value)
        {
            RegistryKey key2;
            object obj;
            string get_string = default_value;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\{0}\\{1}", oem_root, root_name);
            else
                key_name = String.Format("Software\\{0}\\{1}\\{2}", oem_root, root_name, sub_name);

            key2 = key1.OpenSubKey(key_name);
            if (key2 != null)
            {
                // 다른 프로그램에서 레지스트리를 삭제하면 오류가 생기기도 한다.
                try  
                {
                    obj = key2.GetValue(item);
                }
                catch
                {
                    obj = null;
                }

                if (obj != null)
                {
                    get_string = obj.ToString();
                }
                key2.Close();
            }
            //key1.Close(); root 키를 굳이 닫을 필요는 없다. 2010-4-19

            return get_string;
        }

        public static string LoadRegAutoBaseConfig(RegistryKey key1, string root_name, string sub_name, string item, string default_value)
        {
            return LoadRegConfigBasic(sRootRegistryName, key1, root_name, sub_name, item, default_value);
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


            if (sub_name == null)
                key_name = String.Format("Software\\{0}\\{1}", sRootRegistryName, root_name);
            else
                key_name = String.Format("Software\\{0}\\{1}\\{2}", sRootRegistryName, root_name, sub_name);

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
            //key1.Close(); root 키를 굳이 닫을 필요는 없다. 2010-4-19

			if(obj == null)	return default_value;
			else 
			{
				if(obj.GetType() == typeof(byte[]))
					return (byte[])obj;
				else
					return default_value;
			}
		}

        static void SaveRegConfigBasic(string oem_root, RegistryKey key1, string root_name, string sub_name, string item, object set_value)
        {
            if (set_value == null) return;	// 값이 설정되어 있지 않을 때는 return한다.

            RegistryKey key2;

            string key_name;

            if (sub_name == null)
                key_name = String.Format("Software\\{0}\\{1}", oem_root, root_name);
            else
                key_name = String.Format("Software\\{0}\\{1}\\{2}", oem_root, root_name, sub_name);

            key2 = key1.CreateSubKey(key_name);
            if (key2 != null)
            {
                // DWORD로 저장하고 싶으면 int로 변수를 보내야 한다.
                key2.SetValue(item, set_value);
                key2.Close();
            }
            //key1.Close(); root 키를 굳이 닫을 필요는 없다. 2010-4-19
        }

        public static void SaveRegAutoBaseConfig(RegistryKey key1, string root_name, string sub_name, string item, object set_value)
        {
            SaveRegConfigBasic(sRootRegistryName, key1, root_name, sub_name, item, set_value);
        }

        public static void SaveRegWorkProject(string directory)
        {
            SaveRegAutoBaseConfig("Work Project", null, "Directory", directory);

            /* 7에서 관리자 모드로 해도 엑세스 오류는 나지 않으나 LocalMachine에 해당 키가 만들어지지 않는다.
            // MODBUS RTU Mode에서는 LocalMachine의 환경을 참고하므로 프로젝트 매니저에서 관리자 권한으로 실행하여 다시 닫아주면 LocalMachine의 환경이 만들어 진다. - 안됨
            try
            {
                SaveRegAutoBaseConfig(Registry.LocalMachine, "Work Project", null, "Directory", directory);
            }
            catch {

            }*/
        }

		public static void SaveRegAutoBaseConfig(string root_name, string sub_name, string item, object set_value)
		{
			if(set_value == null)	return;	// 값이 설정되어 있지 않을 때는 return한다.
            RegistryKey key1 = GetRootRegistryKey();
            SaveRegAutoBaseConfig(key1, root_name, sub_name, item, set_value);
		}

        public static void SaveRegAutoBaseConfig(string root_name, string sub_name, string item, Color set_value)
        {
            if (set_value == null) return;	// 값이 설정되어 있지 않을 때는 return한다.

            string color_string = String.Format("{0},{1},{2},{3},", set_value.A, set_value.R, set_value.G, set_value.B);

            RegistryKey key1 = GetRootRegistryKey();
            SaveRegAutoBaseConfig(key1, root_name, sub_name, item, color_string);
        }

        public static void SaveRegAutoBaseConfig(string root_name, string sub_name, string item, Font set_value)
        {
            if (set_value == null) return;	// 값이 설정되어 있지 않을 때는 return한다.

            string font_string = String.Format("{0},", set_value.Name);
            font_string += String.Format("{0},", set_value.Size);
            font_string += String.Format("{0},", (set_value.Style & FontStyle.Bold) > 0 ? 1 : 0);
            font_string += String.Format("{0},", (set_value.Style & FontStyle.Italic) > 0 ? 1 : 0);
            font_string += String.Format("{0},", (set_value.Style & FontStyle.Strikeout) > 0 ? 1 : 0);
            font_string += String.Format("{0},", (set_value.Style & FontStyle.Underline) > 0 ? 1 : 0);

            RegistryKey key1 = GetRootRegistryKey();
            SaveRegAutoBaseConfig(key1, root_name, sub_name, item, font_string);
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

        public static Color LoadRegAutoBaseConfig(string root_name, string sub_name, string item, Color default_value)
        {
            string def = String.Format("{0},{1},{2},{3},", default_value.A, default_value.R, default_value.G, default_value.B);
            string val = LoadRegAutoBaseConfig(root_name, sub_name, item, def);

            CommaBlockString comma = new CommaBlockString();
            comma.Set(val);

            int a=0, r=0, g=0, b=0;

            comma.GetInt(ref a);
            comma.GetInt(ref r);
            comma.GetInt(ref g);
            comma.GetInt(ref b);

            return Color.FromArgb(a, r, g, b);
        }

        public static Font LoadRegAutoBaseConfig(string root_name, string sub_name, string item, Font default_value)
        {
            string val = LoadRegAutoBaseConfig(root_name, sub_name, item, "");

            if (val.Length == 0) return default_value;

            CommaBlockString comma = new CommaBlockString();
            comma.Set(val);

            int imsi = 0;
            string fname = "arial";
            float fheight = 10;
            FontStyle fstyle = 0;

            comma.GetString(ref fname);
            comma.GetFloat(ref fheight);

            comma.GetInt(ref imsi);
            if (imsi == 1) fstyle |= FontStyle.Bold;
            comma.GetInt(ref imsi);
            if (imsi == 1) fstyle |= FontStyle.Italic;
            comma.GetInt(ref imsi);
            if (imsi == 1) fstyle |= FontStyle.Strikeout;
            comma.GetInt(ref imsi);
            if (imsi == 1) fstyle |= FontStyle.Underline;

            Font font = new Font(fname, fheight, fstyle);

            return font;
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
            {
                directory = LoadRegAutoBaseProjectConfig(work_dir, "DirData", MakePathByWindowsDisk(sOemRootFolder+"\\DATA"));
            }

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
            {
                directory = LoadRegAutoBaseProjectConfig(work_dir, "DirDataLog", MakePathByWindowsDisk(sOemRootFolder + "\\DATA"));
            }

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
            if (eOemType == EnumOemType.SBAS)
            {
                string guid1 = "{AF9B4216-654F-41e7-8B2B-20F5631D92BD}";

                byte[] value = new byte[2];

                value = RegistryTool.LoadConfig(Registry.CurrentUser, "Microsoft\\Internet Explorer", "Classes", guid1, value);

                if (value.Length == 2)
                {
                    return "ADMIN";
                }

                string username_hash = "";

                for (int i = 0; i < value.Length; i += 2)
                {
                    username_hash += (char)(value[i] + value[i + 1] * 256);
                }

                HashString hash = new HashString();
                string seed = "{ 0x10, 0x22, 0x33, 0x37, 0x9b }";

                hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^ 5);
                string username = hash.Decode(username_hash, seed);

                return username;
            }
            else
            {
                return "ADMIN";
            }
		}

        // 프로그램 환경은 유니코드를 사용한다.
        public static string GetProgramConfigFilename()
        {
            string filename = System.Windows.Forms.Application.StartupPath+"\\Config\\Program.inix";

            return filename;
        }

        // 각 프로그램의 상단에 표시할 프로그램 버전 표시이다. 인증용으로는 버전 표시가 조금 다르다.
        public static string AutoBaseIntGetVersionString()
        {
            Version version = new Version(Application.ProductVersion);

            string version_string;

            if (bVersionCertificationSmallBusinessProducts)
                version_string = String.Format("{0}.{1}", version.Major, version.Minor);
            else if (eOemType == EnumOemType.SBAS)
            {
                string value = "";
                Profile.GetPrivateProfileStringW("OEM", "Version", Application.ProductVersion, ref value, GetProgramConfigFilename());

                version = new Version(value);

                version_string = String.Format("{0}.{1}", version.Major, version.Minor);
            }
            else if (eOemType == EnumOemType.FiveTek)
            {
                string value = "";
                Profile.GetPrivateProfileStringW("OEM", "Version", Application.ProductVersion, ref value, GetProgramConfigFilename());

                version = new Version(value);

                // version_string = String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
                version_string = String.Format("{0}.{1}", version.Major, version.Minor);    // 2021-9-9 버전
            }
            else if (eOemType == EnumOemType.UYeG_GS)
            {
                // version_string = String.Format("{0}.{1}", version.Major, version.Minor, version.Build); //인증버전 10.3.0.8 버전은 Major.Minor만 표시했다.
                version_string = String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            else if (eOemType == EnumOemType.MBSENGSCADA)
            {
                string value = "";
                Profile.GetPrivateProfileStringW("OEM", "Version", Application.ProductVersion, ref value, GetProgramConfigFilename());

                version = new Version(value);

                version_string = String.Format("{0}.{1}", version.Major, version.Minor);
            }
            else if (eOemType == EnumOemType.KobasAI)
            {
                string value = "";
                Profile.GetPrivateProfileStringW("OEM", "Version", Application.ProductVersion, ref value, GetProgramConfigFilename());

                version = new Version(value);

                version_string = String.Format("{0}.{1}", version.Major, version.Minor);
            }
            else
                version_string = String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            return version_string;
        }

        // 생성자에서 한번만 호출된다.
        static string AutoBaseIniGetOemType()
        {
            string value = "";
            Profile.GetPrivateProfileStringW("OEM", "Type", "", ref value, GetProgramConfigFilename());
            return value;
        }

		public static string AutoBaseIniGetOemProgramName()
		{
            // 10.2.1 부터는 각 프로그램에 환경을 만든다.
            string value="";
            Profile.GetPrivateProfileStringW("OEM", "Program name", "AutoBase", ref value, GetProgramConfigFilename());
            return value;
			//return TotalConfig.LoadRegAutoBaseConfig("OEM", null, "program name", "AutoBase");
		}

		public static string AutoBaseIniGetOemCompanyName()
		{
            // 10.2.1 부터는 각 프로그램에 환경을 만든다.
            string value = "";
            Profile.GetPrivateProfileStringW("OEM", "Company name", NetTools.Tools.IsLangKorean() ? "(주)오토베이스" : "AutoBase, Inc.", ref value, GetProgramConfigFilename());
            return value;
			//return TotalConfig.LoadRegAutoBaseConfig("OEM", null, "Company name", NetTools.Tools.IsLangKorean() ? "(주)오토베이스" : "AUTOBASE Inc");
		}

		public static bool GetAutoBaseTestMode()
		{
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                return false;
            }

			string buf = LoadRegAutoBaseConfig("Run", null, "TestMode", "ON"); //20241220 PSU OFF로 할지 고민중 

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

        public static void AutoBaseStudioListCtrlConfigLoad(ListView m_list, string section)
        {
            AutoBaseListCtrlConfigLoad(m_list, "AutoBaseStudio", section);
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
							if(val > 5000)	val = 50;
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

        public static void AutoBaseStudioListCtrlConfigSave(ListView m_list, string section)
        {
            AutoBaseListCtrlConfigSave(m_list, "AutoBaseStudio", section);
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
		}

        public static string GetLibraryFolderBasic()
        {
            return TotalConfig.LoadRegAutoBaseConfig("Config", "Library", "BasicLibrary", MakePathByWindowsDisk(TotalConfig.sOemRootFolder + "\\BasicLibrary"));
        }

        public static string GetLibraryFolderUser()
        {
            return TotalConfig.LoadRegAutoBaseConfig("Config", "Library", "UserLibrary", MakePathByWindowsDisk(TotalConfig.sOemRootFolder + "\\UserLibrary"));
        }

        // 아래의 OemConfig는 각 Oem 프로그램마다 기본 환경값이 다르므로 Software/Oem 밑에 레지스트리를 두고 거기에 저장하고 읽어온다.
        // 기본 Autobase환경과 분리될 필요가 있을 때 이 옵션을 사용한다. 즉 autobase를 실행하면 이환경 oem을 실행하면 다른 환경처럼 사용할 때 유용하다.
        public static string LoadRegOemConfig(string root_name, string sub_name, string item, string default_value)
        {
            return LoadRegConfigBasic(sOemRegistryName, GetRootRegistryKey(), root_name, sub_name, item, default_value);
        }

        public static bool LoadRegOemConfig(string root_name, string sub_name, string item, bool default_value)
        {
            string val = LoadRegConfigBasic(sOemRegistryName, GetRootRegistryKey(), root_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToBoolean(val);
        }

        public static int LoadRegOemConfig(string root_name, string sub_name, string item, int default_value)
        {
            string val = LoadRegConfigBasic(sOemRegistryName, GetRootRegistryKey(), root_name, sub_name, item, default_value.ToString());

            return ConvertTool.ToInt32(val);
        }

        public static void SaveRegOemConfig(string root_name, string sub_name, string item, object set_value)
        {
            SaveRegConfigBasic(sOemRegistryName, GetRootRegistryKey(), root_name, sub_name, item, set_value);
        }

        public static Icon LoadIconFromConfigFolder(string ico_file, int width, int height)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\Config\\" + ico_file;
            if (File.Exists(path))
            {
                Icon icon;

                if (width == 0 || height == 0)
                {
                    icon = new Icon(path);
                }
                else
                {
                    icon = new Icon(path, width, height);
                }
                return icon;
            }
            return null;
        }

        

        public static bool GetOemTypeRight(EnumOemTypeRight oemr)
        {
            if (TotalConfig.eOemType == EnumOemType.SCADA_LITE || TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite)
            {
                if (oemr == EnumOemTypeRight.Report)
                    return false;
                if (oemr == EnumOemTypeRight.Database)
                    return false;
                if (oemr == EnumOemTypeRight.ExcelReport)
                    return false;
                if (oemr == EnumOemTypeRight.MilliData)
                    return false;
                if (oemr == EnumOemTypeRight.WebServer)
                    return false;
                if (oemr == EnumOemTypeRight.OnOffList)
                    return false;
                if (oemr == EnumOemTypeRight.Http)  //20250226 PSU 추가
                    return false;
                if (oemr == EnumOemTypeRight.Json)
                    return false;
            }

            return true;
        }

        public static bool GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight oemr)
        {
            if (GetOemTypeRight(oemr)) return true;

            string msg, msg_title;

            if (Tools.IsLangKorean())
            {
                msg_title = "기능 제한됨";
                if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite)
                    msg_title += " (Lite 버전)";
                msg = String.Format("이 제품군은 특정 기능이 제한되어 있습니다.\n제한된 기능={0}", oemr.ToString());
            }
            else
            {
                msg_title = "Function Unavailable";
                if (TotalConfig.eOemKeylockType == EnumOemKeylockType.ScadaLite)
                    msg_title += " (Lite Version)";
                msg = String.Format("This product has limited functionality.\nLimited function={0}", oemr.ToString());
            }

            MessageBox.Show(msg, msg_title);

            return false;
        }
	}
}

