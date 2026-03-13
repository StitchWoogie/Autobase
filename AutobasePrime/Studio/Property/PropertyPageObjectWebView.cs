using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;

namespace Studio
{
    public partial class PropertyPageObjectWebView : Form
    {
        MultiSelectTextBox multiSelectUrl = new MultiSelectTextBox();

        public PropertyPageObjectWebView()
        {
            InitializeComponent();

            // Load가 호출되지 않으므로 여기서 등록
            this.multiSelectUrl.Add(this.textBoxUrl);
        }

        private void PropertyPageObjectWebView_Load(object sender, EventArgs e)
        {

        }

        public void Set(ObjectArgsWebView obj)
        {
            this.multiSelectUrl.Set(obj.url);
        }

        public void Get(ObjectArgsWebView obj)
        {
            this.multiSelectUrl.Get(ref obj.url);
        }
    }
}