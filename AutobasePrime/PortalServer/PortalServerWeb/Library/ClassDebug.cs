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
/// Summary description for ClassDebug
/// </summary>
public class ClassDebug
{
    static string sDebugByShortMessage = ConfigurationManager.AppSettings["DebugShortMessage"];
        
    public ClassDebug()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    static void SendShortMessage(string msg)
    {
        //kr.co.username.www.ServiceShortMessage service = new kr.co.username.www.ServiceShortMessage();
        //service.SendShortMessage("kks@autobase.biz", msg);
    }

    public static void Debug(string format, params object[] param)
    {
        if (sDebugByShortMessage != "1") return;

        string message = String.Format(format, param);
        SendShortMessage(message);
    }
}
