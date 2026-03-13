using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace PortalServerWeb.AutoWeb.Service
{
    // 서비스 Contract 선언
    [ServiceContract]
    public interface IServiceDataGate
    {
        [OperationContract]
        string GetServerVersion();

        [OperationContract]
        int Connect();

        [OperationContract]
        void DisConnect(int id);

        [OperationContract]
        bool DefaultUserCheck(int id, string hash, out string username, out string err_msg);

        [OperationContract]
        bool CheckUserNameWithVersion(int id, string username, string password, string hash, out string err_msg);

        [OperationContract]
        List<ProjectFilesInfo2> GetProjectFilesInfo(int id, string hash);

        [OperationContract]
        byte[] DownLoadWithData(int id, string filename, string hash);

        [OperationContract]
        string[] GetTagValues(int id, string[] tags, string hash);

        [OperationContract]
        void WriteCurr(int id, string username, string computer, string tag, string val, string hash);

        [OperationContract]
        List<ClassDataGateAlarmFileInfo> GetAlarmLists(int id, string hash);

        [OperationContract]
        string GetAlarmFile(int id, string filename, string hash);

        [OperationContract]
        string GetLogLists(int id, string hash);

        [OperationContract]
        string GetLogFile(int id, string filename, string hash);
    }

    [DataContract]
    public class ProjectFilesInfo2
    {
        [DataMember]
        public long filesize;
        [DataMember]
        public String filename;
        [DataMember]
        public DateTime filetime;
    }

    [DataContract]
    public class ClassDataGateAlarmFileInfo
    {
        [DataMember]
        public string filename;
        [DataMember]
        public int alarm_count;
    }
    
}
