using System;
using AutoLibLocal;
using PortalServerWeb.AutoWeb.Service;

namespace PortalServerWeb.Library
{
	/// <summary>
	/// Summary description for ServiceLib.
	/// </summary>
	public class ServiceLib
	{
		public ServiceLib()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        //public static void SetCommonVars(System.Web.Services.WebService service)
        //{
        //    ConfigVarTotal.bRunByWebService = true;
        //    ConfigVarTotal.webService = service;
        //    ConfigVarTotal.SetProcSendAndGetData(new ConfigVarTotal.DeleSendAndGetData(ServiceDataTag.SendAndGetData));

        //    TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(service.Context.Request);
        //}

        public static void SetCommonVars()
        {
            var ctx = System.Web.HttpContext.Current;

            ConfigVarTotal.SetProcSendAndGetData(
                   (System.Web.HttpContext context,
                    EnumMultiBlockCommand cmd,
                    string data,
                    out string recv) =>
                        ServiceDataTagStatic.SendAndGetData(context, cmd, data, out recv)
               );

            TotalConfig.sDirWorkProject = ProjectLib.GetWorkDir(ctx.Request);
        }
    }
}
