using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NetTools;
using System.Threading;
using System.Security.Cryptography;
using AutoLibLocal;

namespace AutoLib
{
    public class ServiceLibDataGate
    {
        ServiceLibSvcDataGate serviceSvc = new ServiceLibSvcDataGate();
        ServiceLibWebDataGate serviceWeb = new ServiceLibWebDataGate();

        public int Command(string command, params object[] param)
        {
            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                return serviceSvc.Command(command, param);
            else
                return serviceWeb.Command(command, param);
        }

        public string GetResultString(int index)
        {
            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                return serviceSvc.GetResultString(index);
            else
                return serviceWeb.GetResultString(index);
        }

        public byte[] GetResultBytes(int index)
        {
            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                return serviceSvc.GetResultBytes(index);
            else
                return serviceWeb.GetResultBytes(index);
        }

        public List<string> GetResultListString(int index)
        {
            string source = GetResultString(index);

            List<string> array = new List<string>();

            CommaTextReader comma = new CommaTextReader();

            comma.Set(source);
            while (true)
            {
                if (comma.IsEOS()) break;
                array.Add(comma.GetString());
            }

            return array;
        }

        public void PrepareArg1(string arg1)
        {
            if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                serviceSvc.PrepareArg1(arg1);
            else
                serviceWeb.PrepareArg1(arg1);
        }

        public string sErrorMessage
        {
            get
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                    return serviceSvc.sErrorMessage;
                else
                    return serviceWeb.sErrorMessage;
            }
        }
    }

}
