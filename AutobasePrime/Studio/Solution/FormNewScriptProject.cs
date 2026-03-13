using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoLibLocal;
using System.IO;

namespace Studio.Solution
{
    public partial class FormNewScriptProject : Form
    {
        public FormNewScriptProject()
        {
            InitializeComponent();
        }

        string SeekEmptyProjectName()
        {
            string root = this.textBoxLocation.Text;

            string dir;
            int no = 1;
            string project_name;
            
            while (true)
            {
                project_name = String.Format("ClassLibrary{0}", no);
                dir = String.Format("{0}\\{1}", root, project_name);

                if (!Directory.Exists(dir)) break;

                no++;
            }

            return project_name;
        }

        private void FormNewScriptProject_Load(object sender, EventArgs e)
        {
            this.textBoxLocation.Text = String.Format("{0}\\Script", TotalConfig.sDirWorkProject);
            this.textBoxName.Text = SeekEmptyProjectName();
        }

        string MakeProjectFolderString()
        {
            return String.Format("{0}\\{1}", this.textBoxLocation.Text, this.textBoxName.Text);
        }

        void UpdateLabelRealPath()
        {
            this.labelRealPath.Text = MakeProjectFolderString();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelRealPath();
        }

        private void textBoxLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelRealPath();
        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.SelectedPath = this.textBoxLocation.Text;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxLocation.Text = dialog.SelectedPath;
                this.textBoxName.Text = SeekEmptyProjectName();
            }
        }

        void MakeDefaultProject(string project_file)
        {
            string dir = Path.GetDirectoryName(project_file);

            Directory.CreateDirectory(dir);

            TextWriter writer = new StreamWriter(project_file);
            writer.Close();
        }

        public string sSelectedProject;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxName.Text.Length == 0)
            {
                MessageBox.Show("Input the project name.", "Name Required");
                return;
            }

            string project_folder = MakeProjectFolderString();

            string project_filename = String.Format("{0}\\{1}.ScriptProject", project_folder, this.textBoxName.Text);

            if (File.Exists(project_filename))
            {
                MessageBox.Show("Script Project already exists.", "Project");
                return;
            }

            MakeDefaultProject(project_filename);

            sSelectedProject = project_filename;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
