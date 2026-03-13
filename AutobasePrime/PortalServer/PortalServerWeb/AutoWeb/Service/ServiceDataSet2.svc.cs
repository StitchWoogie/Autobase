using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "ServiceDataSet2"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 ServiceDataSet2.svc나 ServiceDataSet2.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ServiceDataSet2 : IServiceDataSet2
    {
        public string GetDataSetFromMdb(string filename, string command, out string error)
        {
            try
            {
                string work_dir = ProjectLib.GetWorkDir(System.Web.HttpContext.Current.Request);

                DataLocal local = new DataLocal();
                TotalConfig.sDirWorkProject = work_dir;

                var ds = local.GetDataSetFromMdb(filename, command, out error);
                return ds?.GetXml();
            }
             catch (Exception ex)
            {
                error = "GetDataSetFromMdb Exception: " + ex.Message;
                return null;
            }
        }

        public string GetDataSetFromDsn(string dsn, string command, out string error)
        {
            try
            {
                string work_dir = ProjectLib.GetWorkDir(System.Web.HttpContext.Current.Request);

                DataLocal local = new DataLocal();
                TotalConfig.sDirWorkProject = work_dir;

                var ds = local.GetDataSetFromDsn(dsn, command, out error);
                return ds?.GetXml();
            }
            catch (Exception ex)
            {
                error = "GetDataSetFromDsn Exception: " + ex.Message;
                return null;
            }
        }

        public bool DataSetCommand(string dsn, string command, out string error, string guid)
        {
            try
            {
                if(!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    error = "Client not connected or heartbeat timeout.";
                    return false;
                }

                string work_dir = ProjectLib.GetWorkDir(System.Web.HttpContext.Current.Request);

                DataLocal local = new DataLocal();
                TotalConfig.sDirWorkProject = work_dir;
                return local.DataSetCommand(dsn, command, out error);
            }
            catch (Exception ex)
            {
                error = "DataSetCommand Exception: " + ex.Message;
                return false;
            }
        }

        public bool GetConnectionStringDbType(string dsn, out int dbtype)
        {
            try
            {
                string work_dir = ProjectLib.GetWorkDir(System.Web.HttpContext.Current.Request);

                TotalConfig.sDirWorkProject = work_dir;

                EnumDbType type;
                bool retn = DbTool.GetConnectionStringDbType(dsn, out type);
                dbtype = (int)type;
                return retn;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetConnectionStringDbType Exception: " + ex.Message);
                dbtype = (int)EnumDbType.Normal;
                return false;
            }
        }
    }
}
