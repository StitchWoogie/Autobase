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
    public partial class PropertyPageObjectWebBrowser : Form
    {
        MultiSelectTextBox multiSelectUrl = new MultiSelectTextBox();
        MultiSelectCheckBox multiSelectStyleNavigation = new MultiSelectCheckBox();
        MultiSelectCheckBox multiSelectStyleContextMenu = new MultiSelectCheckBox();
        MultiSelectCheckBox multiSelectStyleScrollBar = new MultiSelectCheckBox();

        public PropertyPageObjectWebBrowser()
        {
            InitializeComponent();

            // Load가 호출되지 않으므로 여기서 등록
            this.multiSelectUrl.Add(this.textBoxUrl);
            this.multiSelectStyleContextMenu.Add(this.checkBoxStyleContextMenu);
            this.multiSelectStyleNavigation.Add(this.checkBoxStyleAllowNavigation);
            this.multiSelectStyleScrollBar.Add(this.checkBoxStyleScrollBar);
        }

        private void PropertyPageObjectWebBrowser_Load(object sender, EventArgs e)
        {
            
        }

        public void Set(ObjectArgsWebBrowser obj)
        {
            this.multiSelectUrl.Set(obj.url);
            this.multiSelectStyleContextMenu.Set(obj.styleContextMenu);
            this.multiSelectStyleNavigation.Set(obj.styleNavigation);
            this.multiSelectStyleScrollBar.Set(obj.styleScrollBar);
        }

        public void Get(ObjectArgsWebBrowser obj)
        {
            this.multiSelectUrl.Get(ref obj.url);
            this.multiSelectStyleContextMenu.Get(ref obj.styleContextMenu);
            this.multiSelectStyleNavigation.Get(ref obj.styleNavigation);
            this.multiSelectStyleScrollBar.Get(ref obj.styleScrollBar);
        }
    }
}