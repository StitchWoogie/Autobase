using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    public class ErrorMessageItem
    {
        public bool bError;         // true=error, false=warning
        public string message;      // 메시지 내용
        public int col;             // 해당 위치 -1이면 위치정보를 사용하지 않는 경우
        public int row;             // 해당 위치 -1이면 위치정보를 사용하지 않는 경우
        public string sourcefile;   // 해당 소스파일
        public string projectname;  // Project 이름
        public EnumScriptErrorType et;  // 2023-8-31 추가.

        public string MakeErrorString()
        {
            string msg = "";
            msg = String.Format("Line={0}, Col={1}, Message={2}", row+1, col+1, message); // 실제 row, col은 +을 해주어야 한다.

            if (sourcefile != null && sourcefile.Length > 0)
            {
                msg += String.Format(", File={0}", sourcefile);
            }

            if (projectname != null && projectname.Length > 0)
            {
                msg += String.Format(", Project={0}", projectname);
            }

            return msg;
        }
    }
}
