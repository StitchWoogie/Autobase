using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using System.ServiceModel;

namespace SilverlightAutoLibLocal
{
    public class ServiceLib
    {
        public static System.ServiceModel.EndpointAddress GetEndPoint(string service_name)
        {
            string url = AutoLibLocal.ConfigVarTotal.GetServicePath(service_name);
            return new System.ServiceModel.EndpointAddress(url);
        }

        static System.ServiceModel.BasicHttpBinding MakeDefaultBinding()
        {
            System.ServiceModel.BasicHttpBinding binding;

            //if (ConfigVarTotal.bSSL)
            //    binding = new System.ServiceModel.BasicHttpBinding(BasicHttpSecurityMode.Transport);
            //else
                binding = new System.ServiceModel.BasicHttpBinding();
 
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;

            return binding;
        }

        public static ServiceReferenceDataTag2.WebServiceDataTag2SoapClient GetServiceDataTag2()
        {
            System.ServiceModel.BasicHttpBinding binding = MakeDefaultBinding();

            SilverlightAutoLibLocal.ServiceReferenceDataTag2.WebServiceDataTag2SoapClient service = new SilverlightAutoLibLocal.ServiceReferenceDataTag2.WebServiceDataTag2SoapClient(binding, GetEndPoint("WebServiceDataTag2.asmx"));

            return service;
        }

        public static ServiceReferenceDataSet2.WebServiceDataSet2SoapClient GetServiceDataSet2()
        {
            System.ServiceModel.BasicHttpBinding binding = MakeDefaultBinding();

            SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = new SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient(binding, GetEndPoint("WebServiceDataSet2.asmx"));

            return service;
        }
    }
}
