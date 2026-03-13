using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace PortalServerWeb.Library
{
    public class ConfigApplication
    {
        public static int GetInt32(HttpApplicationState app, string item)
        {
            object obj = app[item];

            if (obj == null)
                return 0;

            return NetTools.ConvertTool.ToInt32(obj.ToString());
        }

        public static DateTime GetDateTime(HttpApplicationState app, string item)
        {
            object obj = app[item];

            if (obj == null)
                return new DateTime(2000, 1, 1);

            if (obj.GetType() == typeof(DateTime))
            {
                return (DateTime)obj;
            }

            return new DateTime(2000, 1, 1);
        }

        public static void SetValue(HttpApplicationState app, string item, object value)
        {
            app[item] = value;
        }
    }
}
