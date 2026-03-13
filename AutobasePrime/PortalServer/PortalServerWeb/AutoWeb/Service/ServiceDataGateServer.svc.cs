using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AutoLibLocal;
using System.Web;
using AutoLibLocal.KeyLock;
using System.IO;
using System.ServiceModel.Channels;
using System.Web.Hosting;
using NetTools;
using System.Data;
using System.Reflection;
using NetTools.Hash;
using System.Threading.Tasks;
using PortalServerWeb.Library;
using System.Diagnostics;

// 사용하지 않는 클래스인듯 하다.

namespace PortalServerWeb.AutoWeb.Service
{
    // 서비스 타입 구현
    //[ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall,ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class ServiceDataGateServer : IServiceDataGateServer
    {
        public int CommonMethod(List<byte[]> args, out List<byte[]> result)
        {
            try
            {
                // WebServiceDataGateServer.asmx 와 같은 클래스를 사용한다.
                ClassDataGateServer cdgs = new ClassDataGateServer();

                return cdgs.CommonMethod(args, out result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CommonMethod exception :" + ex.Message);
                result = null;
                return -1;
            }
        }
    }
}
