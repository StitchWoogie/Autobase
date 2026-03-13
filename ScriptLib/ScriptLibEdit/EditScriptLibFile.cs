using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    /// <summary>
    /// 
    /// </summary>
    public class EditScriptLibFile
    {
        public string sProjectName;
        public string sSourceFilename;
        public string sDefaultNamespace;
        
        public List<string> arrayUsing = new List<string>();    // 파일마다 Using문이 다르다.

        public void AddUsing(EditScriptLibMain main, string using_namespace, int col_pos, int row_pos)
        {
            for (int i = 0; i < arrayUsing.Count; i++)
            {
                if (arrayUsing[i] == using_namespace)
                {
                    main.SetWarning(sProjectName, sSourceFilename, col_pos, row_pos, EnumScriptErrorType.Else, "The using directive for '{0}' appeared priviously in this namespace", using_namespace);
                    return;    // already exist
                }
            }

            arrayUsing.Add(using_namespace);
        }
    }
}
