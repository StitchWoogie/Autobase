using AutoLib.ServiceReferenceDownLoadProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "WcfServiceDownLoadProject"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 WcfServiceDownLoadProject.svc나 WcfServiceDownLoadProject.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfServiceDownLoadProject : IServiceDownLoadProject
    {
        private HttpContext Context => HttpContext.Current;
        private HttpRequest Request => HttpContext.Current?.Request;

        public class DownloadResponse
        {
            public ResultType Result { get; set; }
            public DateTime FileTime { get; set; }
            public long FileSize { get; set; }
            public byte[] Data { get; set; }
        }

        public DownloadResponse Download(string dir, string filename,
                                                  bool compareFlag,
                                                  DateTime fileTime,
                                                  long fileSize)
        {
            var output = new DownloadResponse();

            try
            {
                string workDir = ProjectLib.GetWorkDir(Request);
                string path = Path.Combine(workDir, dir, filename);

                if (!File.Exists(path))
                {
                    output.Result = ResultType.notfound;
                    return output;
                }

                FileInfo info = new FileInfo(path);
                DateTime dt = info.LastWriteTime;

                // 비교
                if (compareFlag && fileTime == dt && fileSize == info.Length)
                {
                    output.Result = ResultType.matched;
                    output.FileTime = dt;
                    output.FileSize = info.Length;
                    return output;
                }

                // 파일 크기 0
                if (info.Length == 0)
                {
                    output.Result = ResultType.download;
                    output.FileTime = dt;
                    output.FileSize = info.Length;
                    return output;
                }

                // 파일 데이터 읽기
                output.Data = File.ReadAllBytes(path);
                output.Result = ResultType.download;
                output.FileTime = dt;
                output.FileSize = info.Length;

                return output;
            }
            catch
            {
                output.Result = ResultType.failed;
                return output;
            }
        }

        //public byte[] DownLoadWithData(string dir, string filename, bool compare_flag,
        //                               ref DateTime file_time, long file_size, ref ResultType result)
        //{
        //    try
        //    {
        //        string work_dir = ProjectLib.GetWorkDir(Request);
        //        string path = $"{work_dir}\\{dir}\\{filename}";

        //        if (!File.Exists(path))
        //        {
        //            result = ResultType.notfound;
        //            return null;
        //        }

        //        FileInfo info = new FileInfo(path);
        //        DateTime dt = info.LastWriteTime;

        //        // 비교 모드
        //        if (compare_flag)
        //        {
        //            if (file_time == dt && file_size == info.Length)
        //            {
        //                result = ResultType.matched;
        //                return null;
        //            }
        //        }

        //        // 파일 사이즈 0
        //        if (info.Length == 0)
        //        {
        //            file_time = dt;
        //            result = ResultType.download;
        //            return null;
        //        }

        //        // 파일 읽기
        //        byte[] buffer = File.ReadAllBytes(path);
        //        file_time = dt;
        //        result = ResultType.download;

        //        return buffer;
        //    }
        //    catch(Exception ex)
        //    {
        //        result = ResultType.failed;
        //        return null;
        //    }
        //}

        //public ResultType DownLoad(string dir, string filename, bool compare_flag,
        //                           ref DateTime file_time, long file_size)
        //{
        //    try
        //    {
        //        string work_dir = ProjectLib.GetWorkDir(Request);
        //        string path = $"{work_dir}\\{dir}\\{filename}";

        //        if (!File.Exists(path))
        //            return ResultType.notfound;

        //        FileInfo info = new FileInfo(path);
        //        DateTime dt = info.LastWriteTime;

        //        if (compare_flag)
        //        {
        //            if (file_time == dt && file_size == info.Length)
        //                return ResultType.matched;
        //        }

        //        if (info.Length == 0)
        //            return ResultType.download;

        //        file_time = dt;

        //        return ResultType.download;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultType.failed;
        //    }
        //}

        //public byte[] DownLoadWithData2(string dir, string filename, bool compare_flag,
        //                                ref DateTime file_time, long file_size, ref int result)
        //{
        //    try
        //    {
        //        ResultType type = ResultType.failed;

        //        byte[] data = DownLoadWithData(dir, filename, compare_flag,
        //                                       ref file_time, file_size, ref type);

        //        result = (int)type;
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = (int)ResultType.failed;
        //        return null;
        //    }
        //}
    }
}
