using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetTools;
using ScriptLibRun;

namespace ScriptLibEdit.Editor
{
    public partial class UserControlErrorMessage : UserControl
    {
        public bool bColumnFile = true;
        public bool bColumnProject = true;

        public UserControlErrorMessage()
        {
            InitializeComponent();
        }

        public delegate void DelegateGotoError(int cx, int cy, string file, string project);
        DelegateGotoError procGotoError = null;

        public void SetProcGotoError(DelegateGotoError proc)
        {
            procGotoError = proc;
        }
        
        private void listViewErrorList_DoubleClick(object sender, EventArgs e)
        {
            if (this.listViewErrorList.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listViewErrorList.SelectedItems[0];

            int cy = ConvertTool.ToInt32(lvi.SubItems[2].Text) - 1;
            int cx = ConvertTool.ToInt32(lvi.SubItems[3].Text) - 1;

            int pos = 4;
            string filename = "";

            if(bColumnFile)
                filename = lvi.SubItems[pos++].Text;

            string projectname = "";
            
            if(bColumnProject)
                projectname = lvi.SubItems[pos++].Text;

            if (procGotoError != null)
                procGotoError(cx, cy, filename, projectname);

            /*
            ScriptProject project = ScriptSolution.SeekProjectByName(projectname);

            string fullpath = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), filename);

            FormNewScriptEditor form = SharedStudio.formMain.OpenScript(fullpath);

            form.formChild.panelEditor.GotoViewCursor(cx, cy);
            form.formChild.panelEditor.Select();*/
        }

        public void FillList(EditScriptLibMain script)
        {
            if (!bColumnFile) 
                this.listViewErrorList.Columns.Remove(columnHeaderFile);
            if(!bColumnProject)
                this.listViewErrorList.Columns.Remove(columnHeaderProject);

            this.listViewErrorList.Items.Clear();

            //string root;

            //ScriptProject project;
            ErrorMessageItem emc;
            for (int i = 0; i < script.arrayError.Count; i++)
            {
                emc = script.arrayError[i];
                //project = ScriptSolution.SeekProjectByName(emc.projectname);

                //root = Path.GetDirectoryName(project.sProjectFilename) + "\\";

                ListViewItem lvi = new ListViewItem("", emc.bError ? 0 : 1);

                lvi.SubItems.Add(emc.message);

                lvi.SubItems.Add((emc.row + 1).ToString());
                lvi.SubItems.Add((emc.col + 1).ToString());

                if (emc.sourcefile == null)
                    emc.sourcefile = "null";

                /*
                if (String.Compare(emc.sourcefile, 0, root, 0, root.Length, true) == 0)
                {
                    lvi.SubItems.Add(emc.sourcefile.Substring(root.Length));
                }
                else
                {*/
                if(bColumnFile)
                    lvi.SubItems.Add(emc.sourcefile);
                //}
                

                if(bColumnProject)
                    lvi.SubItems.Add(emc.projectname);

                this.listViewErrorList.Items.Add(lvi);
            }
        }
    }
}
