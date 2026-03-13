using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Studio.Library
{
    public partial class FormCommentWrite : Form
    {
        public FormCommentWrite()
        {
            InitializeComponent();
        }

        public void Set(string url)
        {
            this.webBrowser1.Url = new Uri(url); 
        }
    }
}
