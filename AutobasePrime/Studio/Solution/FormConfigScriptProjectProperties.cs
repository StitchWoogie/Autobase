using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Studio.Solution
{
    public partial class FormConfigScriptProjectProperties : Form
    {
        public FormConfigScriptProjectProperties()
        {
            InitializeComponent();
        }

        ScriptProject tempProject;

        public void Set(ScriptProject project)
        {
            this.textBoxDefaultNamespace.Text = project.sDefaultNamespace;
            this.textBoxProjectPath.Text = project.sProjectFilename;

            tempProject = project;
        }

        public void Get(ScriptProject project)
        {
            project.sDefaultNamespace = this.textBoxDefaultNamespace.Text;
            project.arrayReference = tempProject.arrayReference;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        void FillListReference()
        {
            this.listViewReference.Items.Clear();

            ListViewItem lvi;
            for (int i = 0; i < tempProject.arrayReference.Count; i++)
            {
                lvi = new ListViewItem(tempProject.arrayReference[i].sReferenceName);

                this.listViewReference.Items.Add(lvi);
            }
        }

        private void FormConfigScriptProjectProperties_Load(object sender, EventArgs e)
        {
            FillListReference();
        }

        private void textBoxProjectPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonAddReference_Click(object sender, EventArgs e)
        {
            FormAddReference dialog = new FormAddReference();
            dialog.Set(tempProject);

            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                dialog.Get(tempProject);
                FillListReference();
            }
        }
    }
}
