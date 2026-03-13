using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptLibRun;
using ScriptLibEdit;
using AutoLibLocal;
using NetTools;
using Studio.Solution;
using System.IO;
using ScriptLibRun.Debugger;

namespace Studio.Script
{
    public partial class FormMessagePanel : Form
    {
        public static FormMessagePanel formThis;

        public FormMessagePanel()
        {
            InitializeComponent();

            formThis = this;

            DebuggerEditService.procDebugWriteLine = new DebuggerEditService.DelegateDebugWriteLine(ProcWriteLine);
        }

        static void ProcWriteLine(string msg)
        {
            if (formThis == null) return;

            formThis.MessageOutput(msg);
        }

        public void MessageOutput(string format, params object[] args)
        {
            string msg = String.Format(format, args);

            this.textBoxOutput.Text += msg + "\r\n";
        }

        public void FillErrorListView(EditScriptLibMain script)
        {
            this.listViewErrorList.Items.Clear();

            string root;

            ScriptProject project;
            ErrorMessageItem emc;
            for (int i = 0; i < script.arrayError.Count; i++)
            {
                emc = script.arrayError[i];
                project = ScriptSolution.SeekProjectByName(emc.projectname);

                root = Path.GetDirectoryName(project.sProjectFilename) + "\\";

                ListViewItem lvi = new ListViewItem("", emc.bError ? 0 : 1);

                lvi.SubItems.Add(emc.message);

                if (emc.sourcefile == null)
                    emc.sourcefile = "null";

                if (String.Compare(emc.sourcefile, 0, root, 0, root.Length, true) == 0)
                {
                    lvi.SubItems.Add(emc.sourcefile.Substring(root.Length));
                }
                else
                {
                    lvi.SubItems.Add(emc.sourcefile);
                }
                lvi.SubItems.Add((emc.row + 1).ToString());
                lvi.SubItems.Add((emc.col + 1).ToString());
                lvi.SubItems.Add(emc.projectname);

                this.listViewErrorList.Items.Add(lvi);
            }
        }

        private void listViewErrorList_DoubleClick(object sender, EventArgs e)
        {
            if (this.listViewErrorList.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listViewErrorList.SelectedItems[0];

            string filename = lvi.SubItems[2].Text;
            int cy = ConvertTool.ToInt32(lvi.SubItems[3].Text) - 1;
            int cx = ConvertTool.ToInt32(lvi.SubItems[4].Text) - 1;
            string projectname = lvi.SubItems[5].Text;

            ScriptProject project = ScriptSolution.SeekProjectByName(projectname);

            string fullpath = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), filename);

            FormNewScriptEditor form = SharedStudio.formMain.OpenScript(fullpath);

            form.formChild.panelEditor.GotoViewCursor(cx, cy);
            form.Activate();
            form.formChild.Focus();
        }

        private void listViewErrorList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormMessagePanel_Load(object sender, EventArgs e)
        {
            //TotalConfig.AutoBaseStudioListCtrlConfigLoad(this.listViewErrorList, "ErrorMessage");
        }

        private void FormMessagePanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            //TotalConfig.AutoBaseStudioListCtrlConfigSave(this.listViewErrorList, "ErrorMessage");
        }

        private void listViewErrorList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {

        }

        private void listViewErrorList_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }

        void RecurseFillLocals(ClassDataStack data_stack)
        {
            if (data_stack.parentStack != null)
            {
                RecurseFillLocals(data_stack.parentStack);
            }

            ListViewItem lvi;
            ItemDataStack ids;

            for (int i = 0; i < data_stack.arrayData.Count; i++)
            {
                ids = data_stack.arrayData[i];
                lvi = new ListViewItem(ids.pVar.sVarName);

                if (ids.valueToStudio == null)
                    lvi.SubItems.Add("null");
                else
                {
                    string buf;

                    if (ids.pVar.eVarType == EnumVarType.TypeChar)
                        buf = String.Format("{0} '{1}'", ids.valueToStudio.ToString(), (char)ConvertTool.ToInt32(ids.valueToStudio.ToString()));
                    else
                        buf = ids.valueToStudio.ToString();

                    lvi.SubItems.Add(buf);
                }

                this.listViewLocals.Items.Add(lvi);
            }
        }

        public void FillLocals(ClassDataStack class_stack, ClassDataStack data_stack)
        {
            this.listViewLocals.Items.Clear();

            RecurseFillLocals(class_stack);
            RecurseFillLocals(data_stack);
        }

        private void textBoxOutput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
