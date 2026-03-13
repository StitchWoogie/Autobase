using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using System.IO;
using AutoLibLocal;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectAnalogRotate.
	/// </summary>
	public class PropertyPageObjectAnalogRotate : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.NumericUpDown numericUpDownStartAngle;
		private System.Windows.Forms.NumericUpDown numericUpDownEndAngle;
		private System.Windows.Forms.RadioButton radioButtonDirection0;
		private System.Windows.Forms.RadioButton radioButtonDirection1;
		private System.Windows.Forms.RadioButton radioButtonBackground1;
		private System.Windows.Forms.RadioButton radioButtonBackground0;
		private System.Windows.Forms.RadioButton radioButtonBackground2;
		private System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.Button buttonFilename;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectNumericUpDown multiSelectStartAngle = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiSelectEndAngle = new MultiSelectNumericUpDown();
		MultiSelectRadioButton multiSelectDirection = new MultiSelectRadioButton();
		MultiSelectRadioButton multiSelectBackground = new MultiSelectRadioButton();
		MultiSelectTextBox multiSelectFilename = new MultiSelectTextBox();

		public PropertyPageObjectAnalogRotate()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectStartAngle.Add(this.numericUpDownStartAngle);
			multiSelectEndAngle.Add(this.numericUpDownEndAngle);
			multiSelectDirection.Add(	this.radioButtonDirection0, 
										this.radioButtonDirection1);
			multiSelectBackground.Add(	this.radioButtonBackground0,
										this.radioButtonBackground1,
										this.radioButtonBackground2);
			multiSelectFilename.Add(this.textBoxFilename);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAnalogRotate));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownEndAngle = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownStartAngle = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonDirection1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDirection0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonBackground2 = new System.Windows.Forms.RadioButton();
            this.radioButtonBackground1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBackground0 = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonFilename = new System.Windows.Forms.Button();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAngle)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownEndAngle);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownStartAngle);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownEndAngle
            // 
            this.numericUpDownEndAngle.AccessibleDescription = null;
            this.numericUpDownEndAngle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownEndAngle, "numericUpDownEndAngle");
            this.numericUpDownEndAngle.Font = null;
            this.numericUpDownEndAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownEndAngle.Name = "numericUpDownEndAngle";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownStartAngle
            // 
            this.numericUpDownStartAngle.AccessibleDescription = null;
            this.numericUpDownStartAngle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownStartAngle, "numericUpDownStartAngle");
            this.numericUpDownStartAngle.Font = null;
            this.numericUpDownStartAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownStartAngle.Name = "numericUpDownStartAngle";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButtonDirection1);
            this.groupBox2.Controls.Add(this.radioButtonDirection0);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonDirection1
            // 
            this.radioButtonDirection1.AccessibleDescription = null;
            this.radioButtonDirection1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDirection1, "radioButtonDirection1");
            this.radioButtonDirection1.BackgroundImage = null;
            this.radioButtonDirection1.Font = null;
            this.radioButtonDirection1.Name = "radioButtonDirection1";
            // 
            // radioButtonDirection0
            // 
            this.radioButtonDirection0.AccessibleDescription = null;
            this.radioButtonDirection0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDirection0, "radioButtonDirection0");
            this.radioButtonDirection0.BackgroundImage = null;
            this.radioButtonDirection0.Font = null;
            this.radioButtonDirection0.Name = "radioButtonDirection0";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonBackground2);
            this.groupBox3.Controls.Add(this.radioButtonBackground1);
            this.groupBox3.Controls.Add(this.radioButtonBackground0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonBackground2
            // 
            this.radioButtonBackground2.AccessibleDescription = null;
            this.radioButtonBackground2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBackground2, "radioButtonBackground2");
            this.radioButtonBackground2.BackgroundImage = null;
            this.radioButtonBackground2.Font = null;
            this.radioButtonBackground2.Name = "radioButtonBackground2";
            // 
            // radioButtonBackground1
            // 
            this.radioButtonBackground1.AccessibleDescription = null;
            this.radioButtonBackground1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBackground1, "radioButtonBackground1");
            this.radioButtonBackground1.BackgroundImage = null;
            this.radioButtonBackground1.Font = null;
            this.radioButtonBackground1.Name = "radioButtonBackground1";
            // 
            // radioButtonBackground0
            // 
            this.radioButtonBackground0.AccessibleDescription = null;
            this.radioButtonBackground0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBackground0, "radioButtonBackground0");
            this.radioButtonBackground0.BackgroundImage = null;
            this.radioButtonBackground0.Font = null;
            this.radioButtonBackground0.Name = "radioButtonBackground0";
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.buttonFilename);
            this.groupBox4.Controls.Add(this.textBoxFilename);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // buttonFilename
            // 
            this.buttonFilename.AccessibleDescription = null;
            this.buttonFilename.AccessibleName = null;
            resources.ApplyResources(this.buttonFilename, "buttonFilename");
            this.buttonFilename.BackgroundImage = null;
            this.buttonFilename.Font = null;
            this.buttonFilename.Name = "buttonFilename";
            this.buttonFilename.Click += new System.EventHandler(this.buttonFilename_Click);
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.AccessibleDescription = null;
            this.textBoxFilename.AccessibleName = null;
            resources.ApplyResources(this.textBoxFilename, "textBoxFilename");
            this.textBoxFilename.BackgroundImage = null;
            this.textBoxFilename.Font = null;
            this.textBoxFilename.Name = "textBoxFilename";
            // 
            // PropertyPageObjectAnalogRotate
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Icon = null;
            this.Name = "PropertyPageObjectAnalogRotate";
            this.Load += new System.EventHandler(this.PropertyPageObjectAnalogRotate_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAngle)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public static bool SelectRotateFile(ref string filename)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Rotate Files (*.rot)|*.rot";
			dialog.InitialDirectory = TotalConfig.sDirWorkProject+"\\graphic";

            if (dialog.ShowDialog(SharedStudio.formMain) == DialogResult.OK) 
			{
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(ClassEditProperty.formEditor, dialog.FileName, out filename);
				return true;
			}
			return false;
		}

		private void buttonFilename_Click(object sender, System.EventArgs e)
		{
			string filename = "";

			if(SelectRotateFile(ref filename)) 
			{
				this.textBoxFilename.Text = filename;
			}
		}

		private void groupBox4_Enter(object sender, System.EventArgs e)
		{
		
		}

		public void SetObjectArgs(ObjectArgsAnalogRotate args) 
		{
			this.multiSelectDirection.Set(args.bAngleDirection);
			this.multiSelectBackground.Set(args.nMethod);
			this.multiSelectStartAngle.Set(args.nStartAngle);
			this.multiSelectEndAngle.Set(args.nEndAngle);
			this.multiSelectFilename.Set(args.sFileName);
		}

		public ObjectArgsAnalogRotate GetObjectArgs(ObjectArgsAnalogRotate org) 
		{
			ObjectArgsAnalogRotate args = (ObjectArgsAnalogRotate)Tools.CopyObject(org);

			this.multiSelectDirection.Get(ref args.bAngleDirection);
			this.multiSelectBackground.Get(ref args.nMethod);
			this.multiSelectStartAngle.Get(ref args.nStartAngle);
			this.multiSelectEndAngle.Get(ref args.nEndAngle);
			this.multiSelectFilename.Get(ref args.sFileName);

			return args;
		}

		private void PropertyPageObjectAnalogRotate_Load(object sender, System.EventArgs e)
		{
		
		}
		/*

		public ObjectArgsAnalogRotate ObjectArgs 
		{
			set 
			{
				this.radioButtonDirection0.Checked = (value.bAngleDirection == 0);	
				this.radioButtonDirection1.Checked = (value.bAngleDirection == 1);	

				this.numericUpDownEndAngle.Value = value.nEndAngle;
				this.numericUpDownStartAngle.Value = value.nStartAngle;
				
				this.radioButtonBackground0.Checked = (value.nMethod == 0);
				this.radioButtonBackground1.Checked = (value.nMethod == 1);
				this.radioButtonBackground2.Checked = (value.nMethod == 2);

				this.textBoxFilename.Text = value.sFileName;
			}

			get 
			{
				ObjectArgsAnalogRotate args = new ObjectArgsAnalogRotate();

				if(this.radioButtonDirection0.Checked)			args.bAngleDirection = 0;
				else if(this.radioButtonDirection1.Checked)		args.bAngleDirection = 1;
				else											args.bAngleDirection = 0;

				args.nEndAngle = ConvertTool.ToInt32(this.numericUpDownEndAngle.Value);
				args.nStartAngle = ConvertTool.ToInt32(this.numericUpDownStartAngle.Value);

				if(this.radioButtonBackground0.Checked)			args.nMethod = 0;
				else if(this.radioButtonBackground1.Checked)	args.nMethod = 1;
				else if(this.radioButtonBackground2.Checked)	args.nMethod = 2;
				else											args.nMethod = 0;

				args.sFileName = this.textBoxFilename.Text;

				return args;
			}
		}
		*/

	}
}

/*

GOpenDialog dialog(hwndDlg);
				dialog.SetFilter("Rotate Files (*.rot)|*.rot|");
#if	defined (COMPILE_HANGUL)
				dialog.SetTitle("회전할 때 사용할 파일 고르기");
#else
				dialog.SetTitle("Select rotate filename (*.rot)");
#endif
				if(dialog.Execute())	{
					dialog.GetFileName(filename);
					GetOnlyFileName(filename);
					SetWindowText(GetDlgItem(IDC_AnalogRotate_EDIT_FILENAME), filename);
				}

*/