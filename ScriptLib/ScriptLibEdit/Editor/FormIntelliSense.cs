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
    public partial class FormIntelliSense : Form
    {
        public static FormIntelliSense formThis = null;

        public FormIntelliSense()
        {
            InitializeComponent();

            formThis = this;
        }

        private void FormIntelliSense_FormClosed(object sender, FormClosedEventArgs e)
        {
            formThis = null;
        }

        private void FormIntelliSense_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                this.listView1.Items.Add(i.ToString());

                this.listBox1.Items.Add(i.ToString());
            }
        }
    }
}
