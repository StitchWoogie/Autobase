using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageColor.
	/// </summary>
	public class PropertyPageColor : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelColor;
		PropertyPageColorPublic formChild = new PropertyPageColorPublic();

		public void SetSelectedColor(Color val, bool first_flag)
		{
			formChild.SetSelectedColor(val, first_flag);
		}

		public Color GetSelectedColor()
		{
			return formChild.GetSelectedColor();
		}

		public bool IsMultiSelected()
		{
			return formChild.IsMultiSelected();
		}

		public PropertyPageColor(string tag, string text)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageColor));
            this.panelColor = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelColor
            // 
            this.panelColor.AccessibleDescription = null;
            this.panelColor.AccessibleName = null;
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.BackgroundImage = null;
            this.panelColor.Font = null;
            this.panelColor.Name = "panelColor";
            // 
            // PropertyPageColor
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panelColor);
            this.Icon = null;
            this.Name = "PropertyPageColor";
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
