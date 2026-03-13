using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScriptLibEdit.Editor
{
    public partial class FormScriptErrorMessage : Form
    {
        public FormScriptErrorMessage()
        {
            InitializeComponent();
        }

        private void FormErrorMessage_SizeChanged(object sender, EventArgs e)
        {
            this.buttonClose.Left = this.ClientRectangle.Width / 2 - buttonClose.Width / 2;
        }

        public void FillList(EditScriptLibMain eslm)
        {
            this.userControlErrorMessage1.FillList(eslm);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public bool UseColumnFile
        {
            set
            {
                userControlErrorMessage1.bColumnFile = value;
            }
        }

        public bool UseColumnProject
        {
            set
            {
                userControlErrorMessage1.bColumnProject = value;
            }
        }

        public void SetProcGotoError(ScriptLibEdit.Editor.UserControlErrorMessage.DelegateGotoError proc)
        {
            userControlErrorMessage1.SetProcGotoError(proc);
        }
    }
}
