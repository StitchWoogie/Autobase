using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "ServiceOdbc"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 ServiceOdbc.svc나 ServiceOdbc.svc.cs를 선택하고 디버깅을 시작하십시오.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ServiceOdbc : IServiceOdbc
    {

        public DataSet GetDataSetFromOdbc(string dsn, string command, out string error)
        {
            try
            {
                string work_dir = ProjectLib.GetWorkDir(HttpContext.Current.Request);

                DataLocal local = new DataLocal();
                TotalConfig.sDirWorkProject = work_dir;
                return local.GetDataSetFromOdbc(dsn, command, out error);
            }
            catch (Exception ex)
            {
                error = $"GetDataSetFromOdbc + {ex.Message}";
                return null;
            }
        }

        public bool DataSetOdbcCommand(string dsn, string command, out string error, string guid)
        {
            try
            {
                if(!GuidHeartbeatManager.IsClientConnected(guid))
                {
                    error = "Client not connected or heartbeat expired.";
                    return false;
                }

                string work_dir = ProjectLib.GetWorkDir(HttpContext.Current.Request);
                ServiceDataTagStatic.TryLogIn(work_dir);

                DataLocal local = new DataLocal();
                TotalConfig.sDirWorkProject = work_dir;
                return local.DataSetOdbcCommand(dsn, command, out error);
            }
            catch (Exception ex)
            {
                error = $"DataSetOdbcCommand + {ex.Message}";
                return false;
            }
        }
    }
}
