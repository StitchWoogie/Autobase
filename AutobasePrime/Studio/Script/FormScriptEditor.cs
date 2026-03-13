using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphicModule;

namespace Studio.Script
{
    class FormScriptEditor
    {
        bool bNewScript = AutoLib.ConfigStudio.bUseNewScriptEditor;
        System.Windows.Forms.Form form;

        public FormScriptEditor()
        {
            if (bNewScript)
                form = new FormScriptEditorSimpleNew();
            else
                form = new FormScriptEditorSimpleOld();
        }

        public void EnableScanTimeUse(bool flag)
        {
            if (bNewScript)
                ((FormScriptEditorSimpleNew)form).EnableScanTimeUse(flag);
            else
                ((FormScriptEditorSimpleOld)form).EnableScanTimeUse(flag);
        }

        public void SetDescription(string des)
        {
            if (bNewScript)
                ((FormScriptEditorSimpleNew)form).SetDescription(des);
            else
                ((FormScriptEditorSimpleOld)form).SetDescription(des);
        }

        public void SetScript(string filename)
        {
            if (bNewScript)
                ((FormScriptEditorSimpleNew)form).SetScript(filename);
            else
                ((FormScriptEditorSimpleOld)form).SetScript(filename);
        }

        public DialogResult ShowDialog()
        {
            if (bNewScript)
                return ((FormScriptEditorSimpleNew)form).ShowDialog(SharedStudio.formMain);
            else
                return ((FormScriptEditorSimpleOld)form).ShowDialog(SharedStudio.formMain);
        }

        public ScriptClass GetScript()
        {
            if (bNewScript)
                return ((FormScriptEditorSimpleNew)form).GetScript();
            else
                return ((FormScriptEditorSimpleOld)form).GetScript();
        }

        public void SetScript(string title, ScriptClass script)
        {
            if (bNewScript)
                ((FormScriptEditorSimpleNew)form).SetScript(title, script);
            else
                ((FormScriptEditorSimpleOld)form).SetScript(title, script);
        }

        public void SetScript(ScriptClass script)
        {
            if (bNewScript)
                ((FormScriptEditorSimpleNew)form).SetScript(script);
            else
                ((FormScriptEditorSimpleOld)form).SetScript(script);
        }

        public bool bEnableOptionUseThread
        {
            set
            {
                if (bNewScript)
                    ((FormScriptEditorSimpleNew)form).bEnableOptionUseThread = value;
                else
                    ((FormScriptEditorSimpleOld)form).bEnableOptionUseThread = value;
            }
        }
    }
}
