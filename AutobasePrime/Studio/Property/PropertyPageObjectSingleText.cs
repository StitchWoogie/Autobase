using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using GraphicModule;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageButtonModule3D.
	/// </summary>
	public class PropertyPageObjectSingleText : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxText;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectTextBox multiSelectText = new MultiSelectTextBox();

		public PropertyPageObjectSingleText()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			multiSelectText.Add(this.textBoxText);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectSingleText));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxText);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxText
            // 
            this.textBoxText.AccessibleDescription = null;
            this.textBoxText.AccessibleName = null;
            resources.ApplyResources(this.textBoxText, "textBoxText");
            this.textBoxText.BackgroundImage = null;
            this.textBoxText.Font = null;
            this.textBoxText.Name = "textBoxText";
            // 
            // PropertyPageObjectSingleText
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectSingleText";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsSingleText args) 
		{
			multiSelectText.Set(args.text);
		}

		public ObjectArgsSingleText GetObjectArgs(ObjectArgsSingleText org) 
		{
			ObjectArgsSingleText args = (ObjectArgsSingleText)Tools.CopyObject(org);

			multiSelectText.Get(ref args.text);

			return args;
		}
	}
}
