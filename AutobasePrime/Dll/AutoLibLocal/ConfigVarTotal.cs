using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using NetTools;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Data;
using System.Text;


namespace AutoLibLocal
{
    public enum EnumServiceType
    {
        WebService, // 10.2.7.3까지 전용으로 사용한 웹서버를 통한 서비스 방식 asmx
        WcfService, // WCF를 이용한 NetTcp 직접 접속과 HTTP 접속을 동시에 할 후 있는 새로운 방식
    }

    public enum EnumDataGateBindingType
    {
        NetTcp = 0,
        BasicHttp = 1,
    }

	/// <summary>
	/// Summary description for ConfigVarTotal.
	/// </summary>
	public class ConfigVarTotal
	{
        static public bool bLocalFlag = true;

        static public string sDirConfigUser;		// 각 프로젝트별 사용자 폴더
        static public string sDirConfigTempProject;	// 각 프로젝트별 공통 폴더 (버전뺌)
        static public string sDirConfigCommon;      // ViewMain 공통 폴더

        static public string sSiteRootName;         // http를 뺀 이름

        // 세션을 일치 시키고 싶으면 서비스를 호출할 때 service.CookieContainer = ConfigVarTotal.cookieContainer; 를 지정해야 한다.
        public static System.Net.CookieContainer cookieContainer = null;    // 오류가 나는 경우가 있어서 생성자에서 Try Catch로 잡는다.

        // 웹서버에서만 사용함 WebService에서 세션을 지정하고 기타함수를 호출할 것
        public static System.Web.Services.WebService webService;

        public static bool bRunByWebService = false;
        public static string[] varKeys = null;
        public static string[] varValues = null;

       // public delegate bool DeleSendAndGetData(System.Web.Services.WebService webservice, EnumMultiBlockCommand command, string data, out string recv_data);
        public delegate bool DeleSendAndGetData(
    System.Web.HttpContext context,
    EnumMultiBlockCommand command,
    string data,
    out string recv_data
);
        static DeleSendAndGetData procSendAndGetData = null;

        public static Version serverVersion = null; // null은 서버 정보가 없는 경우이다. 서버 정보 버전은 10.2 부터 있다.
        public static int nWebServerSecurityLevel = 1;         // 정보가 없는 것은 1로 한다.
        public static long nTickGab = 0;

        public static bool bSSL = false;
        public static EnumServiceType eServiceType = EnumServiceType.WebService;
        public static EnumDataGateBindingType eBindType = EnumDataGateBindingType.BasicHttp;
        public static int nServicePort = 80;

        public static bool bWebServiceAlive = true;      // 사이트 최초 접속시 CheckServiceAlive() 를 호출하여 성공하면 true 그렇지 않으면 false가 된다. false가 되었을 때는 다른 함수를
                                                    // 호출해도 다운되므로 호출하지 않도록 처리하도록 한다. 2017-4-28

        static ConfigVarTotal()
        {
            //
            // TODO: Add constructor logic here
            //
            try
            {
                cookieContainer = new System.Net.CookieContainer(); // 오류가 나는 경우가 있다. 
            }
            catch
            {
                cookieContainer = null;
            }
        }

        public static bool CallSendAndGetData(
     EnumMultiBlockCommand cmd,
     string data,
     out string recv)
        {
            recv = "";

            var ctx = System.Web.HttpContext.Current;
            if (ctx == null) return false;

            if (procSendAndGetData != null)
                return procSendAndGetData(ctx, cmd, data, out recv);

            return false;
        }

        public static void Init(string argument)
        {
            bLocalFlag = true;
            bSSL = false;

            // 웹상에서 바로 접속한 경우 http이거나 https이다.
            if (String.Compare(Application.ExecutablePath, 0, "http", 0, 4, true) == 0) 
            {
                bLocalFlag = false;
            }

            string root = "";

            if (bLocalFlag)
            {
                root = "local";
            }
            else
            {
                CommaBlockString comma = new CommaBlockString();
                comma.SetBlockCode('/');
                comma.Set(Application.ExecutablePath);
                while (true)
                {
                    if (comma.IsEOS())
                    {
                        root = "";
                        break;
                    }

                    comma.GetString(ref root);
                    if (root.Length == 0) continue;
                    if (String.Compare(root, 0, "https", 0, 5, true) == 0)
                    {
                        bSSL = true;
                        continue;
                    }
                    if (String.Compare(root, 0, "http", 0, 4, true) == 0)
                    {
                        continue;
                    }
                    break;
                }

                if (root.Length == 0)
                {
                    root = "HttpUnknown";
                }
            }

            // local에서 직접 Site를 실행해 볼 수 있게 한다.
            if (bLocalFlag)
            {
                if (argument != null)
                {
                    if (String.Compare(argument, 0, "Site=", 0, 5, true) == 0)
                    {
                        bLocalFlag = false;
                        root = argument.Substring(5);

                        if (String.Compare(root, 0, "http://", 0, 7, true) == 0)
                        {
                            root = root.Substring(7);

                            eBindType = EnumDataGateBindingType.BasicHttp;
                        }
                        else if (String.Compare(root, 0, "https://", 0, 8, true) == 0)
                        {
                            root = root.Substring(8);

                            eBindType = EnumDataGateBindingType.BasicHttp;
                            
                            bSSL = true;
                        }
                        else if (String.Compare(root, 0, "net.tcp://", 0, 10, true) == 0)
                        {
                            root = root.Substring(10);

                            eBindType = EnumDataGateBindingType.NetTcp;
                            eServiceType = EnumServiceType.WcfService;      // net.tcp 를 사용하면 WCF 서비스이다.  WebService는 http만을 사용했다.
                        }
                    }
                }
            }

            sSiteRootName = root;

            int index = sSiteRootName.IndexOf(':');
            if (index == -1)
            {
                if(bSSL)
                    nServicePort = 443;  
                else
                    nServicePort = 80;  
            }
            else
            {
                nServicePort = ConvertTool.ToInt32(sSiteRootName.Substring(index + 1));
                sSiteRootName = sSiteRootName.Substring(0, index);
            }

            // www.ex.com:2003 처럼 포트번호를 따로 사용할 때 포트번호를 제거하지않으면 일반 경로명에 맞지 않는다.
            // www.ex.com-2003 으로 변경한다.
            index = root.IndexOf(':');
            if (index != -1)
            {
                string tar = "";
                for (int i = 0; i < root.Length; i++)
                {
                    if (root[i] == ':')
                        tar += '-';
                    else
                        tar += root[i];
                }
                root = tar;
            }

            sDirConfigUser = Application.LocalUserAppDataPath + "\\" + root;

            string path = Application.LocalUserAppDataPath; 
            sDirConfigTempProject = path.Substring(0, path.Length - Application.ProductVersion.Length);
            sDirConfigTempProject += root;

            sDirConfigCommon = Application.LocalUserAppDataPath + "\\ViewMainCommon";
        }

        // root의 전체 url을 나타낸다.

        public static string MakeRootUrl()
        {
            StringBuilder s = new StringBuilder();

            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
            {
                if (ConfigVarTotal.eBindType == EnumDataGateBindingType.NetTcp)
                    s.AppendFormat("net.tcp");
                else
                    s.AppendFormat("http");
            }
            else
            {
                if (ConfigVarTotal.bSSL)
                    s.AppendFormat("https");
                else 
                    s.AppendFormat("http");
            }

            s.AppendFormat("://{0}", sSiteRootName);

            if (ConfigVarTotal.nServicePort != 80)
                s.AppendFormat(":{0}", nServicePort);

            return s.ToString();
        }

        public static string GetServicePath(string service)
        {
            if(bSSL)
                return String.Format("https://{0}:{1}/AutoWeb/Service/{2}", sSiteRootName, nServicePort, service);
            else
                return String.Format("http://{0}:{1}/AutoWeb/Service/{2}", sSiteRootName, nServicePort, service);
        }

        public static object GetFromSession(string name)
        {
            if (webService.Session == null) return null;

            return webService.Session[name];
        }

        public static void SetProcSendAndGetData(DeleSendAndGetData dele)
        {
            procSendAndGetData = dele;
        }

        public static bool GetTagValueOnWebService(string tag, out string val)
        {
            val = "";

            if (procSendAndGetData == null) return false;

            string recv_data;

            if (!CallSendAndGetData( EnumMultiBlockCommand.TagValueList, tag, out recv_data))
                return false;

            val = recv_data;
            return true;
        }

        public static bool IsWebServerVersionEqualOrHigher(int major, int minor, int build, int revision)
        {
            /* 이것은 잘될것 같다.
            if (serverVersion == null) return false;

            Version version_local = new Version(major, minor, build, revision);

            if (serverVersion >= version_local)
            {
                return true;
            }

            return false;*/

            /* 틀린함수이다.
            if (serverVersion == null) return false;

            if (major > serverVersion.Major) return false;
            if (minor > serverVersion.Minor) return false;
            if (build > serverVersion.Build) return false;
            if (revision > serverVersion.Revision) return false;

            return true;*/

            if (serverVersion == null) return false;

            if (serverVersion.Major > major) return true;
            if (serverVersion.Major < major) return false;
            if (serverVersion.Minor > minor) return true;
            if (serverVersion.Minor < minor) return false;
            if (serverVersion.Build > build) return true;
            if (serverVersion.Build < build) return false;
            if (serverVersion.Revision >= revision) return true;
            
            return false;
        }

		/*
        static public bool bLocalFlag = true;

		static public string sDirConfigUser;		// 각 프로젝트별 사용자 폴더
		static public string sDirConfigTempProject;	// 각 프로젝트별 공통 폴더 (버전뺌)
		
		static public string sSiteRootName;         // http를 뺀 이름
		static int    nHttpPort = 80;

		public ConfigVarTotal()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void Init(string argument)
		{
			if(String.Compare(Application.ExecutablePath, 0, "http", 0, 4, true) == 0) 
			{
				bLocalFlag = false;
			}
			
			string root="";

			if(bLocalFlag) 
			{
				root = "local";
			}
			else 
			{
				CommaBlockString comma = new CommaBlockString();
				comma.SetBlockCode('/');
				comma.Set(Application.ExecutablePath);
				while(true) 
				{
					if(comma.IsEOS()) 
					{
						root = "";
						break;
					}

					comma.GetString(ref root);
					if(root.Length == 0)	continue;
					if(String.Compare(root, 0, "http", 0, 4, true) == 0)	continue;
					break;
				}

				if(root.Length == 0) 
				{
					root = "HttpUnknown";
				}
			}

			
			if(!bLocalFlag) 
			{
				if(String.Compare(root, "localhost", true) == 0)
					root = "localhost/AutoBaseWeb";
			}

			// local에서 직접 Site를 실행해 볼 수 있게 한다.
			if(bLocalFlag)
			{
				if(argument != null) 
				{
					if(String.Compare(argument, 0, "Site=", 0, 5, true) == 0) 
					{
						bLocalFlag = false;
						root = argument.Substring(5);
					}
				}
			}


			sSiteRootName = root;

			// http://www.ex.com:2003 처럼 포트번호를 따로 사용할 때 포트번호를 제거하지않으면 일반 경로명에 맞지 않는다.
			int index = root.IndexOf(':');
			if(index != -1) 
			{
				nHttpPort = ConvertTool.ToInt32(root.Substring(index+1));
				root = root.Substring(0, index);
			}
			
			sDirConfigUser   = Application.LocalUserAppDataPath+"\\"+root; 

			string path = Application.LocalUserAppDataPath;
			sDirConfigTempProject = path.Substring(0, path.Length-Application.ProductVersion.Length);
			sDirConfigTempProject += root;
		}

		public static string GetServicePath(string service)
		{
			return "http://"+sSiteRootName+"/AutoWeb/Service/"+service;
		}
         */
	}

    public static class AppEnvironment
    {
        public static bool IsAspNet()
        {
            // System.Web이 참조된 경우 자동 사용 가능
            try { return System.Web.HttpContext.Current != null; }
            catch { return false; }
        }

        public static bool IsWinForms()
        {
            return Environment.UserInteractive &&
                   Application.MessageLoop;
        }

        public static bool IsConsoleOrService()
        {
            return !Environment.UserInteractive ||
                   !Application.MessageLoop;
        }
    }
}
