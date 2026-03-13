using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using NetTools;

namespace WebTools
{
    public class UsernameTool
    {
        public static bool CheckNormalReturn(Page pageParent, out string username, out string err_msg)
        {
            username = pageParent.Request["Param1"];
            string seed = pageParent.Request["Param2"];
            string hash_string = pageParent.Request["Param3"];
            byte[] hash = HashTool.ConvertHexaString(hash_string);

            if (!HashTool.CompareHash(username + seed + "UsernameReturn", hash))
            {
                err_msg = "정상적인 호출이 아닙니다. code=3";
                return false;
            }

            err_msg = "";
            username = StringHash.Decode(username, seed);
            return true;
        }
    }
}
