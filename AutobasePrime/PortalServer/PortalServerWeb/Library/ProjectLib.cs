using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ProjectLib
/// </summary>
public class ProjectLib
{
    public ProjectLib()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string GetWorkDir(HttpRequest Request)
    {
        string work_dir;
        
        //if(Request.Url.Host == "localhost") 실버라이트는 Project를 사용하므로 일단 보류
        //    work_dir = Request.PhysicalApplicationPath + "AutoWeb\\Project.localhost";
        //else
            work_dir = Request.PhysicalApplicationPath + "AutoWeb\\Project";

        return work_dir;
    }

    public static string GetRuntimeDir(HttpRequest Request)
    {
        string work_dir;

        work_dir = Request.PhysicalApplicationPath + "AutoWeb\\Runtime";

        return work_dir;
    }
}
