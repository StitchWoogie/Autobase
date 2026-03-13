using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Studio.Solution
{
    public partial class FormAddReference : Form
    {
        public FormAddReference()
        {
            InitializeComponent();
        }

        private void FormAddReference_Load(object sender, EventArgs e)
        {

        }

        public void Set(ScriptProject project)
        {
            for (int i = 0; i < ScriptSolution.arrayProjects.Count; i++)
            {
                this.checkedListBoxProject.Items.Add(Path.GetFileNameWithoutExtension(ScriptSolution.arrayProjects[i].sProjectFilename));
            }

            for (int i = 0; i < project.arrayReference.Count; i++)
            {
                if (project.arrayReference[i].type == EnumReferenceType.Project)
                {
                    int index = this.checkedListBoxProject.Items.IndexOf(project.arrayReference[i].sReferenceName);
                    if (index != -1)
                    {
                        this.checkedListBoxProject.SetItemChecked(index, true);
                    }
                }
            }
        }

        public void Get(ScriptProject project)
        {
            project.arrayReference = new List<ReferenceClass>();

            ReferenceClass reference;

            foreach (int indexChecked in checkedListBoxProject.CheckedIndices)
            {
                reference = new ReferenceClass();
                reference.type = EnumReferenceType.Project;
                reference.sReferenceName = (string)checkedListBoxProject.Items[indexChecked];
                project.arrayReference.Add(reference);
            }
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
