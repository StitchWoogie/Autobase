using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Studio.Solution;
using ScriptLibRun;
using System.IO;
using NetTools;
using AutoLibLocal;

namespace Studio.Script
{
    public partial class FormNewScriptErrorMessage : Form
    {
        public FormNewScriptErrorMessage()
        {
            InitializeComponent();
        }

        Form formParent = null;

        public void Set(Form parent, ScriptLibRun.ScriptLibMain main)
        {
            formParent = parent;
            /*
            for (int i = 0; i < main.arrayError.Count; i++)
            {
                this.listBox1.Items.Add(main.arrayError[i].MakeErrorString());
            }*/

            this.listViewErrorList.Items.Clear();

            //string root;
            
            //ScriptProject project;
            ErrorMessageItem emc;
            for (int i = 0; i < main.arrayError.Count; i++)
            {
                emc = main.arrayError[i];
                //project = ScriptSolution.SeekProjectByName(emc.projectname);

                //root = Path.GetDirectoryName(project.sProjectFilename)+"\\";

                ListViewItem lvi = new ListViewItem("", emc.bError ? 0 : 1);

                lvi.SubItems.Add(emc.message);

                /*
                if (String.Compare(emc.sourcefile, 0, root, 0, root.Length, true) == 0)
                {
                    lvi.SubItems.Add(emc.sourcefile.Substring(root.Length));
                }
                else
                {
                    lvi.SubItems.Add(emc.sourcefile);
                }*/
                lvi.SubItems.Add((emc.row + 1).ToString());
                lvi.SubItems.Add((emc.col + 1).ToString());
                //lvi.SubItems.Add(emc.projectname);

                this.listViewErrorList.Items.Add(lvi);
            }
        
        }

        private void FormNewScriptErrorMessage_Load(object sender, EventArgs e)
        {
            TotalConfig.AutoBaseStudioListCtrlConfigLoad(listViewErrorList, "FormNewScriptErrorMessage");
        }

        private void listViewErrorList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listViewErrorList.SelectedItems.Count == 0) return;

            ListViewItem lvi = this.listViewErrorList.SelectedItems[0];

            //string filename = lvi.SubItems[2].Text;
            int cy = ConvertTool.ToInt32(lvi.SubItems[2].Text) - 1;
            int cx = ConvertTool.ToInt32(lvi.SubItems[3].Text) - 1;
            //string projectname = lvi.SubItems[5].Text;

            //ScriptProject project = ScriptSolution.SeekProjectByName(projectname);

            //string fullpath = String.Format("{0}\\{1}", Path.GetDirectoryName(project.sProjectFilename), filename);

            //FormNewScriptEditor form = SharedStudio.formMain.OpenScript(fullpath);

            //form.formChild.panelEditor.GotoViewCursor(cx, cy);

            // RichTextBox 는 커서를 찾기가 쉽지 않다.
            if (formParent.GetType() == typeof(FormScriptEditorSimpleNew))
            {
                FormScriptEditorSimpleNew form = (FormScriptEditorSimpleNew)formParent;

                form.GotoViewCursor(cx, cy);
            }
        }

        private void listViewErrorList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormNewScriptErrorMessage_SizeChanged(object sender, EventArgs e)
        {
            this.buttonClose.Left = this.ClientRectangle.Width / 2 - buttonClose.Width / 2;
        }

        private void FormNewScriptErrorMessage_FormClosed(object sender, FormClosedEventArgs e)
        {
            TotalConfig.AutoBaseStudioListCtrlConfigSave(listViewErrorList, "FormNewScriptErrorMessage");
        }
    }
}
