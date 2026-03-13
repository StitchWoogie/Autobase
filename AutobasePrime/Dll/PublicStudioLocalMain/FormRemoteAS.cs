using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PublicStudioLocalMain
{
    public partial class FormRemoteAS : Form
    {
        public FormRemoteAS()
        {
            InitializeComponent();
        }

        public void Set(string url)
        {
            this.webBrowser1.Url = new Uri(url);
        }
    }
}