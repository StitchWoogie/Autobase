using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using GraphicModule;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageButtonModule3D.
	/// </summary>
	public class PropertyPageObjectDate : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonDisplay0;
		private System.Windows.Forms.RadioButton radioButtonDisplay1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonWeek1;
		private System.Windows.Forms.RadioButton radioButtonWeek0;
		private System.Windows.Forms.RadioButton radioButtonWeek2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectDate()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDate));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisplay1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplay0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonWeek2 = new System.Windows.Forms.RadioButton();
            this.radioButtonWeek1 = new System.Windows.Forms.RadioButton();
            this.radioButtonWeek0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonDisplay1);
            this.groupBox1.Controls.Add(this.radioButtonDisplay0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonDisplay1
            // 
            resources.ApplyResources(this.radioButtonDisplay1, "radioButtonDisplay1");
            this.radioButtonDisplay1.Name = "radioButtonDisplay1";
            // 
            // radioButtonDisplay0
            // 
            resources.ApplyResources(this.radioButtonDisplay0, "radioButtonDisplay0");
            this.radioButtonDisplay0.Name = "radioButtonDisplay0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonWeek2);
            this.groupBox2.Controls.Add(this.radioButtonWeek1);
            this.groupBox2.Controls.Add(this.radioButtonWeek0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonWeek2
            // 
            resources.ApplyResources(this.radioButtonWeek2, "radioButtonWeek2");
            this.radioButtonWeek2.Name = "radioButtonWeek2";
            // 
            // radioButtonWeek1
            // 
            resources.ApplyResources(this.radioButtonWeek1, "radioButtonWeek1");
            this.radioButtonWeek1.Name = "radioButtonWeek1";
            // 
            // radioButtonWeek0
            // 
            resources.ApplyResources(this.radioButtonWeek0, "radioButtonWeek0");
            this.radioButtonWeek0.Name = "radioButtonWeek0";
            // 
            // PropertyPageObjectDate
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "PropertyPageObjectDate";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		

		public ObjectArgsDate ObjectArgs 
		{
			set 
			{
				//this.textBoxText.Text = value.text;
				int val = value.type%10;

				this.radioButtonDisplay0.Checked = (val == 0);
				this.radioButtonDisplay1.Checked = (val == 1);

				val = (value.type/10)%10;
				this.radioButtonWeek0.Checked = (val == 0);
				this.radioButtonWeek1.Checked = (val == 1);
				this.radioButtonWeek2.Checked = (val == 2);
			}
			get 
			{
				ObjectArgsDate args = new ObjectArgsDate();
				//args.text = this.textBoxText.Text;

				int val = 0;

				if(this.radioButtonDisplay0.Checked)	val = 0;
				else									val = 1;

				args.type = val;

				if(this.radioButtonWeek0.Checked)		val = 0;
				else if(this.radioButtonWeek1.Checked)	val = 1;
				else									val = 2;

				args.type = args.type+val*10;

				return args;
			}
		}
	}
}
