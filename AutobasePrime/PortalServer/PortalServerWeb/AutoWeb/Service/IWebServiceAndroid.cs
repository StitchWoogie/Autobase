using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using static PortalServerWeb.AutoWeb.Service.WcfWebServiceAndroid;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWcfWebServiceAndroid"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IWebServiceAndroid
    {
        [OperationContract]
        string[] GetTagGroupLists();

        [OperationContract]
        List<WebTagList> GetTagGroupListArrays();

        [OperationContract]
        string[] GetWebTagLists(string group_name);

        [OperationContract]
        string GetTagValueLists(string tags);

        [OperationContract]
        List<WebTagInfo> GetWebTagsInfo(string group_name);

        [OperationContract]
        bool WriteCurr(string username, string tag, string val, byte[] hash, string guid);

        [OperationContract]
        byte[] DownLoadWithData2(string dir, string filename, out int result);

        [OperationContract]
        byte[] DownLoadWithData3(string dir, string filename, bool compare_flag,
            ref int DateTimeYear, ref int DateTimeMonth, ref int DateTimeDay,
            ref int DateTimeHour, ref int DateTimeMinute, ref int DateTimeSecond,
            long file_size, ref int result);

        [OperationContract]
        List<ProjectFilesInfo> GetProjectFilesInfo();
    }
}
