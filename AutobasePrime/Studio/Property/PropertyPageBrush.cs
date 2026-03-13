using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using GraphicModule;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageColor.
	/// </summary>
	public class PropertyPageBrush : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelColor;
        PropertyPageBrushPublic formChild = new PropertyPageBrushPublic();

        public void SetSelectedBrush(BrushPublic val, bool first_flag)
		{
			formChild.SetSelectedBrush(val, first_flag);
		}

        public void GetSelectedBrush(ref BrushPublic val)
		{
			formChild.GetSelectedBrush(ref val);
		}

        /*
		public bool IsMultiSelected()
		{
			return formChild.IsMultiSelected();
		}*/

        public PropertyPageBrush(string tag, string text)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			this.Tag = tag;
			if(text != null)	this.Text = text;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageBrush));
            this.panelColor = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelColor
            // 
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.Name = "panelColor";
            // 
            // PropertyPageBrush
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panelColor);
            this.Name = "PropertyPageBrush";
            this.Load += new System.EventHandler(this.PropertyPageColor_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageColor_Load(object sender, System.EventArgs e)
		{
			formChild.TopLevel = false;

			panelColor.Controls.Add(formChild);
			formChild.Show();
		}

	}
}
