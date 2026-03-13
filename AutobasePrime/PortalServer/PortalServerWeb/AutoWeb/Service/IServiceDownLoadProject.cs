using AutoLib.ServiceReferenceDownLoadProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using static PortalServerWeb.AutoWeb.Service.WcfServiceDownLoadProject;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWcfServiceDownLoadProject"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IServiceDownLoadProject
    {
        //[OperationContract]
        //byte[] DownLoadWithData(string dir, string filename, bool compare_flag,
        //                        ref DateTime file_time, long file_size, ref ResultType result);

        //[OperationContract]
        //ResultType DownLoad(string dir, string filename, bool compare_flag,
        //                    ref DateTime file_time, long file_size);

        //[OperationContract]
        //byte[] DownLoadWithData2(string dir, string filename, bool compare_flag,
        //                         ref DateTime file_time, long file_size, ref int result);

        [OperationContract]
        DownloadResponse Download(string dir, string filename, bool compare_flag, DateTime file_time, long file_size);

    }
}
