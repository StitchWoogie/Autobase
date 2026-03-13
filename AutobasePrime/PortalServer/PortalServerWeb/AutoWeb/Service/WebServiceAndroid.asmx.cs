using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AutoLibLocal;
using System.Collections;
using System.Web.UI.WebControls;
using PortalServerWeb.Library;
using NetTools;
using System.Security.Cryptography;
using System.IO;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for WebServiceAndroid
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceAndroid : System.Web.Services.WebService
    {

        [WebMethod]
        public string[] GetTagGroupLists()
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;

            List<WebTagList> array = WebGroupList.Load();
            WebTagList list;
            // ListItem item;

            string[] lists = new string[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                list = (WebTagList)array[i];
                //item = new ListItem();

                lists[i] = list.name;
                
                /*
                if (list.description.Length > 0)
                    item.Text = String.Format("{0}  ({1})", list.name, list.description);
                else
                    item.Text = String.Format("{0}", list.name);

                item.Value = String.Format("WebTagListItem.aspx?name={0}&des={1}", Server.UrlEncode(list.name), Server.UrlEncode(list.description));

                this.List1.Items.Add(item);*/
            }

            return lists;
        }

        [WebMethod]
        public List<WebTagList> GetTagGroupListArrays()
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;

            List<WebTagList> array = WebGroupList.Load();

            return array;
        }

        [WebMethod]
        public string[] GetWebTagLists(string group_name)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;

            List<WebTagList> array = WebGroupList.Load();
            WebTagList list;
            //ListItem item;

            for (int i = 0; i < array.Count; i++)
            {
                list = (WebTagList)array[i];

                if (group_name == list.name)
                {
                    string[] lists = new string[list.member.Count];

                    for (int j = 0; j < lists.Length; j++)
                    {
                        lists[j] = (string)list.member[j];
                    }

                    return lists;
                }
            }

            return null;
        }

        static string GetTagValueListFromTcpXDocument(WebService webservice, string tags)
        {
            string recv_data;

            if (!ServiceDataTag.SendAndGetData(webservice, EnumMultiBlockCommand.TagValueList, tags, out recv_data))
                return null;

            return recv_data;
        }

        [WebMethod(EnableSession = true)]
        public string GetTagValueLists(string tags)
        {
            ServiceLib.SetCommonVars(this);
            
            return GetTagValueListFromTcpXDocument(this, tags);

        }

        [WebMethod]
        public List<WebTagInfo> GetWebTagsInfo(string group_name)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;

            List<WebTagList> array = WebGroupList.Load();
            List<WebTagInfo> retn = new List<WebTagInfo>();

            for (int i = 0; i < array.Count; i++)
            {
                if (group_name == array[i].name)
                {
                    TagFile file = new TagFile();
                    string filename = String.Format("{0}\\Tag\\local.tagx", work_dir);

                    for (int j = 0; j < array[i].member.Count; j++)
                    {
                        WebTagInfo wti = new WebTagInfo();
                        wti.tag = array[i].member[j];

                        TagPublicClass tp = file.LoadTagOne(filename, wti.tag);
                        wti.description = tp.description;
                        wti.tag_type = tp.enumTagType.ToString();

                        wti.unit = "";
                        wti.desON = "";
                        wti.desOFF = "";

                        if (tp.enumTagType == EnumTagType.AI)
                        {
                            TagAiClass ai = (TagAiClass)tp;
                            wti.unit = ai.unit;
                        }
                        else if (tp.enumTagType == EnumTagType.AO)
                        {
                            TagAoClass ao = (TagAoClass)tp;
                            wti.unit = ao.unit;
                        }
                        else if (tp.enumTagType == EnumTagType.DI)
                        {
                            TagDiClass di = (TagDiClass)tp;
                            wti.desON = di.desON;
                            wti.desOFF = di.desOFF;
                        }
                        else if (tp.enumTagType == EnumTagType.DO)
                        {
                            TagDoClass dout = (TagDoClass)tp;
                            wti.desON = dout.desON;
                            wti.desOFF = dout.desOFF;
                        }
                        else
                        {
                            
                        }

                        retn.Add(wti);
                    }

                    return retn;
                }
            }

            return null;
        }

        public static byte[] StringToBytes(string buf)
        {
            byte[] b = new byte[buf.Length * 2];

            for (int i = 0; i < buf.Length; i++)
            {
                b[i * 2 + 0] = (byte)(buf[i] / 256);
                b[i * 2 + 1] = (byte)(buf[i] % 256);
            }

            return b;
        }

        bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

        // 아무곳에서 출력할 수 없도록 Hash해서 사용한다.
        [WebMethod(Description = "WriteCurr Session", EnableSession = true)]
        public void WriteCurr(string username, string tag, string val, byte[] hash)
        {
            string err_msg;
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                return;
            }

            byte[] hash_s = StringToBytes("WriteCurr"+username+tag+val);

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(hash_s);

            bool match = CompareBytes(result, hash);

            if (!match) return;

            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TotalConfig.sDirWorkProject = work_dir;

            string clientip = Context.Request.UserHostAddress;

            string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, username, clientip, "WebServer");

            string recv_data;

            ServiceDataTag.SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
        }


        /// <summary>
        /// ref ResultType result 을 ref int result 로 변경했다. 2010.10-8
        /// </summary>
        [WebMethod]
        public byte[] DownLoadWithData2(string dir, string filename, out int result)
        {
            string path;
            //DateTime dt;

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            path = String.Format("{0}\\{1}\\{2}", work_dir, dir, filename);
            if (!File.Exists(path))
            {
                result = (int)ResultType.notfound;
                return null;
            }

            
            //dt = File.GetLastWriteTime(path);
            
            FileInfo info = new FileInfo(path);

            /*
            if (compare_flag)
            {
                if (file_time == dt && file_size == info.Length)
                {
                    result = (int)ResultType.matched;
                    return null;
                }
            }*/

            if (info.Length == 0)	// file size 0
            {
                result = (int)ResultType.download;
                return null;
            }

            FileStream fs = File.OpenRead(path);

            if (fs == null)
            {
                result = (int)ResultType.failed;
                return null;
            }
            byte[] ex = new byte[info.Length];
            fs.Read(ex, 0, (int)info.Length);
            fs.Close();

            //file_time = dt;

            result = (int)ResultType.download;

            return ex;
        }

        /// <summary>
        /// ref ResultType result 을 ref int result 로 변경했다. 2010.10-8
        /// </summary>
        [WebMethod]
        public byte[] DownLoadWithData3(string dir, string filename, bool compare_flag, 
            ref int DateTimeYear, ref int DateTimeMonth, ref int DateTimeDay, 
            ref int DateTimeHour, ref int DateTimeMinute, ref int DateTimeSecond, long file_size, ref int result)
        {
            string path;
            DateTime dt;

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            path = String.Format("{0}\\{1}\\{2}", work_dir, dir, filename);
            if (!File.Exists(path))
            {
                result = (int)ResultType.notfound;
                return null;
            }

            dt = File.GetLastWriteTime(path);
            FileInfo info = new FileInfo(path);

            if (compare_flag)
            {
                if (DateTimeYear == dt.Year &&
                    DateTimeMonth == dt.Month &&
                    DateTimeDay == dt.Day &&
                    DateTimeHour == dt.Hour &&
                    DateTimeMinute == dt.Minute &&
                    DateTimeSecond == dt.Second &&
                    file_size == info.Length)
                {
                    result = (int)ResultType.matched;
                    return null;
                }
            }

            DateTimeYear = dt.Year;
            DateTimeMonth = dt.Month;
            DateTimeDay = dt.Day;
            DateTimeHour = dt.Hour;
            DateTimeMinute = dt.Minute;
            DateTimeSecond = dt.Second;

            if (info.Length == 0)	// file size 0
            {
                
                result = (int)ResultType.download;
                return null;
            }

            FileStream fs = File.OpenRead(path);

            if (fs == null)
            {
                result = (int)ResultType.failed;
                return null;
            }
            byte[] ex = new byte[info.Length];
            fs.Read(ex, 0, (int)info.Length);
            fs.Close();

            result = (int)ResultType.download;

            return ex;
        }

        void RecurseProjectFilesInfo(string root_dir, string dir, List<ProjectFilesInfo> array)
        {
            DirectoryInfo infodir = new DirectoryInfo(dir);

            foreach (DirectoryInfo di in infodir.GetDirectories())
            {
                RecurseProjectFilesInfo(root_dir, di.FullName, array);
            }

            foreach (FileInfo fi in infodir.GetFiles("*.*"))
            {
                string ext = fi.Extension;
                if (String.Compare(ext, ".ANI", true) == 0) continue;
                if (String.Compare(ext, ".PCX", true) == 0) continue;
                if (String.Compare(ext, ".GIF", true) == 0) continue;
                if (String.Compare(ext, ".BMP", true) == 0) continue;
                if (String.Compare(ext, ".TIF", true) == 0) continue;
                if (String.Compare(ext, ".JPG", true) == 0) continue;
                if (String.Compare(ext, ".WMF", true) == 0) continue;
                if (String.Compare(ext, ".EMF", true) == 0) continue;
                
                ProjectFilesInfo file = new ProjectFilesInfo();
                file.filename = fi.FullName.Substring(root_dir.Length+1);
                file.filesize = fi.Length;
                array.Add(file);
            }
        }

        /// <summary>
        /// 스마트폰에서 필요한 파일만을 다운로드 하기위한 목록을 만든다.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<ProjectFilesInfo> GetProjectFilesInfo()
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            TotalConfig.sDirWorkProject = work_dir;

            List<ProjectFilesInfo> array = new List<ProjectFilesInfo>();

            RecurseProjectFilesInfo(work_dir, work_dir, array);

            return array;
        }
    }

    public class ProjectFilesInfo
    {
        public long filesize;
        public String filename;
    }

    public class WebTagInfo
    {
        public String tag;
        public String description;
        public String tag_type;
        public String value;
        public String unit;
        public String desON;
        public String desOFF;
    }
}
