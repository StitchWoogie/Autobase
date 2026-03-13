using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using GraphicModule;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageButtonModule3D.
	/// </summary>
	public class PropertyPageObjectModule : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TextBox textBoxModule;
		private System.Windows.Forms.Button buttonModule;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectModule()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectModule));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonModule = new System.Windows.Forms.Button();
            this.textBoxModule = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.buttonModule);
            this.groupBox2.Controls.Add(this.textBoxModule);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonModule
            // 
            this.buttonModule.AccessibleDescription = null;
            this.buttonModule.AccessibleName = null;
            resources.ApplyResources(this.buttonModule, "buttonModule");
            this.buttonModule.BackgroundImage = null;
            this.buttonModule.Font = null;
            this.buttonModule.Name = "buttonModule";
            this.buttonModule.Click += new System.EventHandler(this.buttonModule_Click);
            // 
            // textBoxModule
            // 
            this.textBoxModule.AccessibleDescription = null;
            this.textBoxModule.AccessibleName = null;
            resources.ApplyResources(this.textBoxModule, "textBoxModule");
            this.textBoxModule.BackgroundImage = null;
            this.textBoxModule.Font = null;
            this.textBoxModule.Name = "textBoxModule";
            // 
            // PropertyPageObjectModule
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectModule";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonModule_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Module files (*.modx) unicode|*.modx|Module files (*.mod) ascii|*.mod";

			dialog.InitialDirectory = TotalConfig.sDirWorkProject+"\\graphic";
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				this.textBoxModule.Text = Path.GetFileName(dialog.FileName);
			}
		}

		public ObjectArgsModule ObjectArgs 
		{
			set 
			{
				this.textBoxModule.Text = value.filename;
			}
			get 
			{
				ObjectArgsModule args = new ObjectArgsModule();
				args.filename = this.textBoxModule.Text;
				return args;
			}
		}
	}
}
