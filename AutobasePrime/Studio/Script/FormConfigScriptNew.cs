using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptLibEdit;

namespace Studio.Script
{
    public partial class FormConfigScriptNew : Form
    {
        public FormConfigScriptNew()
        {
            InitializeComponent();
        }

        private void FormConfigScriptNew_Load(object sender, EventArgs e)
        {
            this.checkBoxAutomaticallyFormat.Checked = ConfigScriptEditor.bAutomaticallyFormat;
            this.checkBoxDisplayLineEndingGlyph.Checked = ConfigScriptEditor.bDisplayLineEndingGlyph;
            this.checkBoxDrawingByMemory.Checked = ConfigScriptEditor.bDrawingByMemory;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ConfigScriptEditor.bAutomaticallyFormat = this.checkBoxAutomaticallyFormat.Checked;
            ConfigScriptEditor.bDisplayLineEndingGlyph = this.checkBoxDisplayLineEndingGlyph.Checked;
            ConfigScriptEditor.bDrawingByMemory = this.checkBoxDrawingByMemory.Checked;

            ConfigScriptEditor.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
