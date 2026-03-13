using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

namespace PortalServerWeb.AutoWeb.Service
{
    public enum ResultType
    {
        failed = 0,
        matched = 1,
        download = 2,
        notfound = 3,
    }

    /// <summary>
    /// Summary description for ServiceDownLoadProject
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceDownLoadProject : System.Web.Services.WebService
    {

        /// <summary>
        /// return 값으로 실제 파일을 돌려준다.
        /// 파일이 클 경우 다운로드가 제대로 안된다. (Framework 1.0에서만 테스트) 
        /// </summary>
        [WebMethod]
        public byte[] DownLoadWithData(string dir, string filename, bool compare_flag, ref DateTime file_time, long file_size, ref ResultType result)
        {
            string path;
            DateTime dt;

            string work_dir = ProjectLib.GetWorkDir(Context.Request);

            path = String.Format("{0}\\{1}\\{2}", work_dir, dir, filename);
            if (!File.Exists(path))
            {
                result = ResultType.notfound;
                return null;
            }

            dt = File.GetLastWriteTime(path);
            FileInfo info = new FileInfo(path);

            if (compare_flag)
            {
                if (file_time == dt && file_size == info.Length)
                {
                    result = ResultType.matched;
                    return null;
                }
            }

            if (info.Length == 0)	// file size 0
            {
                file_time = dt;
                result = ResultType.download;
                return null;
            }

            FileStream fs = File.OpenRead(path);

            if (fs == null)
            {
                result = ResultType.failed;
                return null;
            }
            byte[] ex = new byte[info.Length];
            fs.Read(ex, 0, (int)info.Length);
            fs.Close();

            file_time = dt;

            result = ResultType.download;

            return ex;
        }

        [WebMethod]
        public ResultType DownLoad(string dir, string filename, bool compare_flag, ref DateTime file_time, long file_size)
        {
            string path;
            DateTime dt;

            string work_dir = Context.Request.PhysicalApplicationPath + "AutoWeb\\Project";

            path = String.Format("{0}\\{1}\\{2}", work_dir, dir, filename);
            if (!File.Exists(path))
            {
                return ResultType.notfound;
            }

            dt = File.GetLastWriteTime(path);
            FileInfo info = new FileInfo(path);

            if (compare_flag)
            {
                if (file_time == dt && file_size == info.Length)
                {
                    return ResultType.matched;
                }
            }

            if (info.Length == 0)	// file size 0
            {
                return ResultType.download;
            }

            file_time = dt;

            return ResultType.download;
        }

        /// <summary>
        /// ref ResultType result 을 ref int result 로 변경했다. 2010.10-8
        /// </summary>
        [WebMethod]
        public byte[] DownLoadWithData2(string dir, string filename, bool compare_flag, ref DateTime file_time, long file_size, ref int result)
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
                if (file_time == dt && file_size == info.Length)
                {
                    result = (int)ResultType.matched;
                    return null;
                }
            }

            if (info.Length == 0)	// file size 0
            {
                file_time = dt;
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

            file_time = dt;

            result = (int)ResultType.download;

            return ex;
        }
    }
}
