using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebTools
{
    public class JavaTool
    {
        //"\\r\\n\\r\\n"
        // \n 문자일때는 \\r\\n을 붙여준다.
        static string ChangeLfToCrLf(string source)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '\n')
                {
                    s.Append("\\r\\n");
                }
                else
                    s.Append(source[i]);
            }

            return s.ToString();
        }

        public static void MessageBox(string strMessage)
        {
            strMessage = ChangeLfToCrLf(strMessage);

            string strHTML;
            //strHTML = "<script language=\"javascript\">\n    <!--\n" +
            //    "        alert('" + strMessage + "');\n//-->\n    </script>\n";
            strHTML = "<script language=\"javascript\">alert('" + strMessage + "');</script>";

            HttpContext.Current.Response.Write(strHTML);
        }

        public static void MessageBoxAndRedirect(string strMessage, string url, string target)
        {
            strMessage = ChangeLfToCrLf(strMessage);

            string script;

            script = String.Format("<script language=\"javascript\">alert('{0}'); window.open(\"{1}\", \"{2}\");  </script>", strMessage, url, target);
            
            HttpContext.Current.Response.Write(script);
        }

        public static void Redirect(string url, string target)
        {
            string script;

            script = String.Format("<script language=\"javascript\"> window.open(\"{0}\", \"{1}\");  </script>", url, target);

            HttpContext.Current.Response.Write(script);
        }

    }
}
