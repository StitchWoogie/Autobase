using AutoLibLocal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace PortalServerWeb
{
    public class Global : System.Web.HttpApplication
    {
        //20250709 PSU OpenSilver 및 Http 요청 OPTIONS 200 응답 추가.
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture; //defaultCulture는 beginRequest 에서 계속 설정해야함. 251119 PSU 추가 

            //string path = Request.Path.ToLower();

            //// WebAPI 요청은 CORS 처리에서 제외 
            //if (path.StartsWith("/api/"))
            //    return;

            //HttpContext context = HttpContext.Current;
            //// OPTIONS 요청이면 즉시 200으로 응답 종료
            //if (context.Request.HttpMethod == "OPTIONS")
            //{
            //    context.Response.StatusCode = 200;
            //    context.Response.End();
            //}
        }
    }
}