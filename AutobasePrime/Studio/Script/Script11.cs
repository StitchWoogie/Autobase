using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibEdit.Debugger;
using ScriptLibRun.Debugger;
using System.Windows;
using NetTools;
using ScriptLibRun;
using ScriptLibEdit;

namespace Studio.Script
{
    class Script11
    {
        public static void Init()
        {
            EditScriptConfig.formMainEditor = SharedStudio.formMain;

            DebuggerHostEdit.Init();

            DebuggerEditService.procGotoCursor = new DebuggerEditService.DelegateGotoCursor(GotoCursor);
        }

        public static void UnInit()
        {
            DebuggerHostEdit.UnInit();
        }

        static void GotoCursor(string filename, int col, int row, ClassDataStack class_stack, ClassDataStack data_stack)
        {
            // ScriptLibEdit.Editor.TextArea.DebugStart(); // 감시에서 디버거 신호가 들어오면 자동으로 디버거 모드로 바꿔준다.
            ScriptLibEdit.Editor.UserControlScriptEditor.DebugStart(); // 감시에서 디버거 신호가 들어오면 자동으로 디버거 모드로 바꿔준다.


            string file = filename;//TotalConfig.sDirWorkProject + "\\" + node.FullPath;

            FormNewScriptEditor form = SharedStudio.formMain.OpenScript(file);

            form.formChild.panelEditor.GotoBreakPoint(col, row);

            SharedStudio.formMain.Activate();

            FormMessagePanel.formThis.FillLocals(class_stack, data_stack);
        }

        
    }
}
