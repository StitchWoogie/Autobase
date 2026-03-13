using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PortalServerWeb.AutoWeb.Service
{
    // 서비스 Contract 선언
    [ServiceContract]
    public interface IServiceDataGateServer
    {
        [OperationContract]
        int CommonMethod(List<byte[]> args, out List<byte[]> result);

    }
}
