using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IServiceOdbc"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IServiceOdbc
    {
        [OperationContract]
        DataSet GetDataSetFromOdbc(string dsn, string command, out string error);

        [OperationContract]
        bool DataSetOdbcCommand(string dsn, string command, out string error, string guid);
    }
}
