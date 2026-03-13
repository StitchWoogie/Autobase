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

namespace Studio.Script
{
    public partial class FormSelectNewClass : Form
    {
        string sInsertFolder;

        public FormSelectNewClass(string insert_folder)
        {
            InitializeComponent();

            sInsertFolder = insert_folder;
        }

        static int nNewScriptNo = 1;

        private void FormSelectNewClass_Load(object sender, EventArgs e)
        {
            string path;
            string filename;

            while (true)
            {
                filename = String.Format("Class{0}.cs", nNewScriptNo++);
                path = String.Format("{0}\\{1}", sInsertFolder, filename);

                if (!File.Exists(path)) break;
            }

            this.textBoxFilename.Text = filename;
            this.textBoxFilename.Select();
            this.textBoxFilename.Focus();
        }

        public string sFileName;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string filename = this.textBoxFilename.Text.Trim();

            if (filename.Length == 0)
            {
                MessageBox.Show("Input the filename");
                return;
            }

            if (filename.IndexOf('.') == -1)
                filename += ".cs";

            string path = String.Format("{0}\\{1}", sInsertFolder, filename);

            if (File.Exists(path))
            {
                MessageBox.Show("Same filename already exists.", filename);
                return;
            }

            sFileName = path;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
