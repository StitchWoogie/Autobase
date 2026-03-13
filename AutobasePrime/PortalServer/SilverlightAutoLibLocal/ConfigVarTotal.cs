using System;
using Microsoft.Win32;
using System.IO;
using NetTools;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO.IsolatedStorage;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for ConfigVarTotal.
	/// </summary>
	public class ConfigVarTotal
	{
        
        static public bool bLocalFlag = true;
        
        //static public string sDirConfigUser;		// 각 프로젝트별 사용자 폴더
        static public string sDirConfigTempProject;	// 각 프로젝트별 공통 폴더 (버전뺌)
        
        static public string sSiteRootName;         // http를 뺀 이름
        //public static bool bSSL;                    // SSL 일단 실버라이트는 잘안되어서 보류 했다.

        /*
        // 세션을 일치 시키고 싶으면 서비스를 호출할 때 service.CookieContainer = ConfigVarTotal.cookieContainer; 를 지정해야 한다.
        public static System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();

        // 웹서버에서만 사용함 WebService에서 세션을 지정하고 기타함수를 호출할 것
        public static System.Web.Services.WebService webService;

        public static bool bRunByWebService = false;

        public delegate bool DeleSendAndGetData(System.Web.Services.WebService webservice, EnumMultiBlockCommand command, string data, out string recv_data);
        static DeleSendAndGetData procSendAndGetData = null;

        public ConfigVarTotal()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        */
        public static void Init(string argument)
        {
            //bSSL = false;

            if (String.Compare(argument, 0, "http", 0, 4, StringComparison.CurrentCultureIgnoreCase) == 0) 
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
                comma.Set(argument);
                while (true)
                {
                    if (comma.IsEOS())
                    {
                        root = "";
                        break;
                    }

                    comma.GetString(ref root);
                    if (root.Length == 0) continue;

                    if (String.Compare(root, 0, "https", 0, 5, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        //bSSL = true;
                        continue;
                    }
                    if (String.Compare(root, 0, "http", 0, 4, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        continue;
                    }

                    /*
                    if (String.Compare(root, 0, "localhost", 0, 9, StringComparison.CurrentCultureIgnoreCase) == 0) // localhost 인 경우는 Sub도 포함시킨다.
                    {
                        string sub = "";
                        comma.GetString(ref sub);
                        root += "/" + sub;
                    }*/

                    break;
                }

                if (root.Length == 0)
                {
                    root = "HttpUnknown";
                }
            }

            /*
            // local에서 직접 Site를 실행해 볼 수 있게 한다.
            if (bLocalFlag)
            {
                if (argument != null)
                {
                    if (String.Compare(argument, 0, "Site=", 0, 5, true) == 0)
                    {
                        bLocalFlag = false;
                        root = argument.Substring(5);
                    }
                }
            }*/

            sSiteRootName = root;

            /*
            // http://www.ex.com:2003 처럼 포트번호를 따로 사용할 때 포트번호를 제거하지않으면 일반 경로명에 맞지 않는다.
            int index = root.IndexOf(':');
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
            sDirConfigTempProject += root;*/
        }

        public static string GetServicePath(string service)
        {
            //if (bSSL)
            //    return "https://" + sSiteRootName + "/AutoWeb/Service/" + service;
            //else
                return "http://" + sSiteRootName + "/AutoWeb/Service/" + service;
        }
        /*
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

            if (!procSendAndGetData(webService, EnumMultiBlockCommand.TagValueList, tag, out recv_data))
                return false;

            val = recv_data;
            return true;
        }

		*/

        public static IsolatedStorageFile isolatedStorageFile;

        public static void InitSilverlight()
        {
            isolatedStorageFile = IsolatedStorageFile.GetUserStoreForSite();
        }
	}
}
