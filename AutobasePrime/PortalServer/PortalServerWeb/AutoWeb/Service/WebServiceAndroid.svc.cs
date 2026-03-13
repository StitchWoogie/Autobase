using AutoLib.ServiceReferenceDownLoadProject;
using AutoLibLocal;
using PortalServerWeb.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WcfWebServiceAndroid"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WcfWebServiceAndroid.svc나 WcfWebServiceAndroid.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfWebServiceAndroid : IWebServiceAndroid
    {
        HttpContext Ctx => HttpContext.Current;

        private void Init()
        {
            ServiceLib.SetCommonVars();
            TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(Ctx.Request);
        }

        public string[] GetTagGroupLists()
        {
            try
            {
                Init();

                List<WebTagList> array = WebGroupList.Load();
                string[] lists = new string[array.Count];

                for (int i = 0; i < array.Count; i++)
                    lists[i] = array[i].name;

                return lists;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"GetTagGroupLists error: {ex.Message}");
                return null;
            }
        }

        public List<WebTagList> GetTagGroupListArrays()
        {
            try
            {
                Init();
                return WebGroupList.Load();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetTagGroupListArrays error: {ex.Message}");
                return null;
            }
        }

        public string[] GetWebTagLists(string group_name)
        {
            try
            {
                Init();

                var array = WebGroupList.Load();

                foreach (var list in array)
                {
                    if (group_name == list.name)
                    {
                        string[] lists = new string[list.member.Count];
                        for (int j = 0; j < list.member.Count; j++)
                            lists[j] = (string)list.member[j];
                        return lists;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetWebTagLists error: {ex.Message}");
                return null;
            }
        }

        // SendAndGetData wrapper 그대로 사용
        static string GetTagValueListFromTcpXDocument(string tags)
        {
            string recv_data;
            if (!ServiceDataTagStatic.SendAndGetData(HttpContext.Current, EnumMultiBlockCommand.TagValueList, tags, out recv_data))
                return null;

            return recv_data;
        }

        public string GetTagValueLists(string tags)
        {
            try
            {
                Init();
                return GetTagValueListFromTcpXDocument(tags);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetTagValueLists error: {ex.Message}");
                return null;
            }
        }

        public List<WebTagInfo> GetWebTagsInfo(string group_name)
        {
            try
            {
                Init();

                var array = WebGroupList.Load();
                string work_dir = TotalConfig.sDirWorkProject;

                foreach (var group in array)
                {
                    if (group_name == group.name)
                    {
                        string filename = $"{work_dir}\\Tag\\local.tagx";
                        TagFile file = new TagFile();

                        List<WebTagInfo> list = new List<WebTagInfo>();

                        foreach (string tag in group.member)
                        {
                            TagPublicClass tp = file.LoadTagOne(filename, tag);

                            WebTagInfo w = new WebTagInfo
                            {
                                tag = tag,
                                description = tp.description,
                                tag_type = tp.enumTagType.ToString(),
                                unit = "",
                                desON = "",
                                desOFF = ""
                            };

                            switch (tp.enumTagType)
                            {
                                case EnumTagType.AI:
                                    w.unit = ((TagAiClass)tp).unit;
                                    break;
                                case EnumTagType.AO:
                                    w.unit = ((TagAoClass)tp).unit;
                                    break;
                                case EnumTagType.DI:
                                    var di = (TagDiClass)tp;
                                    w.desON = di.desON;
                                    w.desOFF = di.desOFF;
                                    break;
                                case EnumTagType.DO:
                                    var dout = (TagDoClass)tp;
                                    w.desON = dout.desON;
                                    w.desOFF = dout.desOFF;
                                    break;
                            }

                            list.Add(w);
                        }

                        return list;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetWebTagsInfo error: {ex.Message}");
                return null;
            }
        }

        public bool WriteCurr(string username, string tag, string val, byte[] hash, string guid)
        {
            try
            {
                Init();

                string err_msg;

                if( !GuidHeartbeatManager.IsClientConnected(guid))
                {
                    return false;
                }

                if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
                    return false;

                byte[] hash_s = StringToBytes("WriteCurr" + username + tag + val);

                SHA1 sha = new SHA1CryptoServiceProvider();
                byte[] result = sha.ComputeHash(hash_s);

                if (!CompareBytes(result, hash)) return false;

                string clientip = Ctx.Request.UserHostAddress;
                string buf = $"{tag},{val},{username},{clientip},WebServer";

                string recv_data;
                ServiceDataTagStatic.SendAndGetData(Ctx, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WriteCurr error: {ex.Message}");
                return false;
            }
        }

        // Helpers
        public static byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];
            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }
            return b;
        }

        bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        public byte[] DownLoadWithData2(string dir, string filename, out int result)
        {
            try
            {
                Init();
                string work_dir = TotalConfig.sDirWorkProject;

                string path = $"{work_dir}\\{dir}\\{filename}";
                if (!File.Exists(path))
                {
                    result = (int)ResultType.notfound;
                    return null;
                }

                FileInfo info = new FileInfo(path);
                if (info.Length == 0)
                {
                    result = (int)ResultType.download;
                    return null;
                }

                byte[] data = File.ReadAllBytes(path);
                result = (int)ResultType.download;
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownLoadWithData2 error: {ex.Message}");
                result = (int)ResultType.failed;
                return null;
            }
        }

        public byte[] DownLoadWithData3(string dir, string filename, bool compare_flag,
            ref int DateYear, ref int DateMonth, ref int DateDay,
            ref int DateHour, ref int DateMinute, ref int DateSecond,
            long file_size, ref int result)
        {
            try
            {
                Init();
                string work_dir = TotalConfig.sDirWorkProject;
                string path = $"{work_dir}\\{dir}\\{filename}";

                if (!File.Exists(path))
                {
                    result = (int)ResultType.notfound;
                    return null;
                }

                FileInfo info = new FileInfo(path);
                DateTime dt = info.LastWriteTime;

                if (compare_flag &&
                    DateYear == dt.Year &&
                    DateMonth == dt.Month &&
                    DateDay == dt.Day &&
                    DateHour == dt.Hour &&
                    DateMinute == dt.Minute &&
                    DateSecond == dt.Second &&
                    file_size == info.Length)
                {
                    result = (int)ResultType.matched;
                    return null;
                }

                DateYear = dt.Year; DateMonth = dt.Month; DateDay = dt.Day;
                DateHour = dt.Hour; DateMinute = dt.Minute; DateSecond = dt.Second;

                byte[] data = File.ReadAllBytes(path);
                result = (int)ResultType.download;
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownLoadWithData3 error: {ex.Message}");
                result = (int)ResultType.failed;
                return null;
            }
        }

        void RecurseProjectFilesInfo(string root_dir, string dir, List<ProjectFilesInfo> array)
        {
            foreach (var di in new DirectoryInfo(dir).GetDirectories())
                RecurseProjectFilesInfo(root_dir, di.FullName, array);

            foreach (var fi in new DirectoryInfo(dir).GetFiles())
            {
                string ext = fi.Extension.ToUpper();

                string[] skipExts = new[]
                {
            ".ANI", ".PCX", ".GIF", ".BMP", ".TIF", ".JPG", ".WMF", ".EMF"
        };

                if (skipExts.Contains(ext))
                    continue;

                array.Add(new ProjectFilesInfo
                {
                    filename = fi.FullName.Substring(root_dir.Length + 1),
                    filesize = fi.Length
                });
            }
        }

        public List<ProjectFilesInfo> GetProjectFilesInfo()
        {
            try
            {
                Init();
                string work_dir = TotalConfig.sDirWorkProject;

                List<ProjectFilesInfo> array = new List<ProjectFilesInfo>();
                RecurseProjectFilesInfo(work_dir, work_dir, array);

                return array;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetProjectFilesInfo error: {ex.Message}");
                return null;
            }
        }
        public class ProjectFilesInfo { public long filesize; public String filename; }
        public class WebTagInfo { public String tag; public String description; public String tag_type; public String value; public String unit; public String desON; public String desOFF; }
    }
}
