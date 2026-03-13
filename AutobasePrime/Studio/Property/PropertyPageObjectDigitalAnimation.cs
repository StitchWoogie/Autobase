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
	/// Summary description for PropertyPageDigitalAnimation.
	/// </summary>
	public class PropertyPageObjectDigitalAnimation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textBoxOnFile;
		private System.Windows.Forms.Button buttonOnFile;
		private System.Windows.Forms.Button buttonOffFile;
		private System.Windows.Forms.TextBox textBoxOffFile;
		private System.Windows.Forms.CheckBox checkBoxOverlay;
		public System.Windows.Forms.CheckBox checkBoxRestore;
		private System.Windows.Forms.Button buttonFromLibrary;
		private System.Windows.Forms.Button buttonChangeOnOff;
		private System.Windows.Forms.Panel panelOnFile;
		private System.Windows.Forms.Panel panelOffFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		AnimationPlay aniOn = new AnimationPlay();
		AnimationPlay aniOff = new AnimationPlay();

		MultiSelectTextBox multiSelectOnFile = new MultiSelectTextBox();
		MultiSelectTextBox multiSelectOffFile = new MultiSelectTextBox();
		MultiSelectCheckBox multiSelectOverlay = new MultiSelectCheckBox();

		public PropertyPageObjectDigitalAnimation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectOnFile.Add(this.textBoxOnFile);
			multiSelectOffFile.Add(this.textBoxOffFile);
			multiSelectOverlay.Add(this.checkBoxOverlay);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDigitalAnimation));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelOnFile = new System.Windows.Forms.Panel();
            this.buttonOnFile = new System.Windows.Forms.Button();
            this.textBoxOnFile = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonOffFile = new System.Windows.Forms.Button();
            this.textBoxOffFile = new System.Windows.Forms.TextBox();
            this.panelOffFile = new System.Windows.Forms.Panel();
            this.checkBoxOverlay = new System.Windows.Forms.CheckBox();
            this.checkBoxRestore = new System.Windows.Forms.CheckBox();
            this.buttonFromLibrary = new System.Windows.Forms.Button();
            this.buttonChangeOnOff = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.panelOnFile);
            this.groupBox1.Controls.Add(this.buttonOnFile);
            this.groupBox1.Controls.Add(this.textBoxOnFile);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panelOnFile
            // 
            this.panelOnFile.AccessibleDescription = null;
            this.panelOnFile.AccessibleName = null;
            resources.ApplyResources(this.panelOnFile, "panelOnFile");
            this.panelOnFile.BackgroundImage = null;
            this.panelOnFile.Font = null;
            this.panelOnFile.Name = "panelOnFile";
            // 
            // buttonOnFile
            // 
            this.buttonOnFile.AccessibleDescription = null;
            this.buttonOnFile.AccessibleName = null;
            resources.ApplyResources(this.buttonOnFile, "buttonOnFile");
            this.buttonOnFile.BackgroundImage = null;
            this.buttonOnFile.Font = null;
            this.buttonOnFile.Name = "buttonOnFile";
            this.buttonOnFile.Click += new System.EventHandler(this.buttonOnFile_Click);
            // 
            // textBoxOnFile
            // 
            this.textBoxOnFile.AccessibleDescription = null;
            this.textBoxOnFile.AccessibleName = null;
            resources.ApplyResources(this.textBoxOnFile, "textBoxOnFile");
            this.textBoxOnFile.BackgroundImage = null;
            this.textBoxOnFile.Font = null;
            this.textBoxOnFile.Name = "textBoxOnFile";
            this.textBoxOnFile.TextChanged += new System.EventHandler(this.textBoxOnFile_TextChanged);
            this.textBoxOnFile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxOnFile_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.buttonOffFile);
            this.groupBox2.Controls.Add(this.textBoxOffFile);
            this.groupBox2.Controls.Add(this.panelOffFile);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonOffFile
            // 
            this.buttonOffFile.AccessibleDescription = null;
            this.buttonOffFile.AccessibleName = null;
            resources.ApplyResources(this.buttonOffFile, "buttonOffFile");
            this.buttonOffFile.BackgroundImage = null;
            this.buttonOffFile.Font = null;
            this.buttonOffFile.Name = "buttonOffFile";
            this.buttonOffFile.Click += new System.EventHandler(this.buttonOffFile_Click);
            // 
            // textBoxOffFile
            // 
            this.textBoxOffFile.AccessibleDescription = null;
            this.textBoxOffFile.AccessibleName = null;
            resources.ApplyResources(this.textBoxOffFile, "textBoxOffFile");
            this.textBoxOffFile.BackgroundImage = null;
            this.textBoxOffFile.Font = null;
            this.textBoxOffFile.Name = "textBoxOffFile";
            // 
            // panelOffFile
            // 
            this.panelOffFile.AccessibleDescription = null;
            this.panelOffFile.AccessibleName = null;
            resources.ApplyResources(this.panelOffFile, "panelOffFile");
            this.panelOffFile.BackgroundImage = null;
            this.panelOffFile.Font = null;
            this.panelOffFile.Name = "panelOffFile";
            // 
            // checkBoxOverlay
            // 
            this.checkBoxOverlay.AccessibleDescription = null;
            this.checkBoxOverlay.AccessibleName = null;
            resources.ApplyResources(this.checkBoxOverlay, "checkBoxOverlay");
            this.checkBoxOverlay.BackgroundImage = null;
            this.checkBoxOverlay.Font = null;
            this.checkBoxOverlay.Name = "checkBoxOverlay";
            // 
            // checkBoxRestore
            // 
            this.checkBoxRestore.AccessibleDescription = null;
            this.checkBoxRestore.AccessibleName = null;
            resources.ApplyResources(this.checkBoxRestore, "checkBoxRestore");
            this.checkBoxRestore.BackgroundImage = null;
            this.checkBoxRestore.Font = null;
            this.checkBoxRestore.Name = "checkBoxRestore";
            // 
            // buttonFromLibrary
            // 
            this.buttonFromLibrary.AccessibleDescription = null;
            this.buttonFromLibrary.AccessibleName = null;
            resources.ApplyResources(this.buttonFromLibrary, "buttonFromLibrary");
            this.buttonFromLibrary.BackgroundImage = null;
            this.buttonFromLibrary.Font = null;
            this.buttonFromLibrary.Name = "buttonFromLibrary";
            this.buttonFromLibrary.Click += new System.EventHandler(this.buttonFromLibrary_Click);
            // 
            // buttonChangeOnOff
            // 
            this.buttonChangeOnOff.AccessibleDescription = null;
            this.buttonChangeOnOff.AccessibleName = null;
            resources.ApplyResources(this.buttonChangeOnOff, "buttonChangeOnOff");
            this.buttonChangeOnOff.BackgroundImage = null;
            this.buttonChangeOnOff.Font = null;
            this.buttonChangeOnOff.Name = "buttonChangeOnOff";
            this.buttonChangeOnOff.Click += new System.EventHandler(this.buttonChangeOnOff_Click);
            // 
            // PropertyPageObjectDigitalAnimation
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.buttonChangeOnOff);
            this.Controls.Add(this.buttonFromLibrary);
            this.Controls.Add(this.checkBoxRestore);
            this.Controls.Add(this.checkBoxOverlay);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectDigitalAnimation";
            this.Load += new System.EventHandler(this.PropertyPageObjectDigitalAnimation_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageObjectDigitalAnimation_Load(object sender, System.EventArgs e)
		{
			aniOn.TopLevel = false;
			aniOn.FormBorderStyle = FormBorderStyle.None;

			aniOff.TopLevel = false;
			aniOff.FormBorderStyle = FormBorderStyle.None;

			//this.Controls.Add(aniOn);
			//this.Controls.Add(aniOff);

			panelOnFile.Controls.Add(aniOn);
			panelOffFile.Controls.Add(aniOff);

			aniOn.Show();
			aniOff.Show();
		}

		private void buttonChangeOnOff_Click(object sender, System.EventArgs e)
		{
			string temp = textBoxOnFile.Text;
			textBoxOnFile.Text = textBoxOffFile.Text;
			textBoxOffFile.Text = temp;

			aniOn.SetFileName(textBoxOnFile.Text);
			aniOff.SetFileName(textBoxOffFile.Text);
		}

		private void textBoxOnFile_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
		
		}

		private void textBoxOnFile_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void buttonOnFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = TotalConfig.sDirWorkProject+"\\graphic";
			dialog.Filter = PropertyPageObjectBitmap.sFilterAnimation+"|"+PropertyPageObjectBitmap.sFilterBitmap;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				string change;
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(ClassEditProperty.formEditor, dialog.FileName, out change);
				this.textBoxOnFile.Text = change;
				aniOn.SetFileName(textBoxOnFile.Text);
			}
		}

		private void buttonOffFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = TotalConfig.sDirWorkProject+"\\graphic";
			dialog.Filter = PropertyPageObjectBitmap.sFilterBitmap+"|"+PropertyPageObjectBitmap.sFilterAnimation;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				string change;
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(ClassEditProperty.formEditor, dialog.FileName, out change);
				this.textBoxOffFile.Text = change;
				aniOff.SetFileName(textBoxOffFile.Text);
			}
		}

		private void buttonFromLibrary_Click(object sender, System.EventArgs e)
		{
			FormSelectDigitalAnimationFromLibrary dialog = new FormSelectDigitalAnimationFromLibrary();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				string change;
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(ClassEditProperty.formEditor, dialog.sFileNameON, out change);
				this.textBoxOnFile.Text = change;
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(ClassEditProperty.formEditor, dialog.sFileNameOFF, out change);
				this.textBoxOffFile.Text = change;

				aniOn.SetFileName(textBoxOnFile.Text);
				aniOff.SetFileName(textBoxOffFile.Text);
			}
		}

		public void SetObjectArgs(ObjectArgsDigitalAnimation args) 
		{
			multiSelectOnFile.Set(args.sFileOn);
			multiSelectOffFile.Set(args.sFileOff);
			multiSelectOverlay.Set(args.nOverlayMethod);

			aniOn.SetFileName(args.sFileOn);
			aniOff.SetFileName(args.sFileOff);
		}

		public ObjectArgsDigitalAnimation GetObjectArgs(ObjectArgsDigitalAnimation org) 
		{
			ObjectArgsDigitalAnimation args = (ObjectArgsDigitalAnimation)Tools.CopyObject(org);

			multiSelectOnFile.Get(ref args.sFileOn);
			multiSelectOffFile.Get(ref args.sFileOff);
			multiSelectOverlay.Get(ref args.nOverlayMethod);	

			return args;
		}

		/*
		public ObjectArgsDigitalAnimation ObjectArgs 
		{
			set 
			{
				this.checkBoxOverlay.Checked = (value.nOverlayMethod == 1);
				this.textBoxOnFile.Text = value.sFileOn;
				this.textBoxOffFile.Text = value.sFileOff;

				aniOn.SetFileName(value.sFileOn);
				aniOff.SetFileName(value.sFileOff);
			}
			get 
			{
				ObjectArgsDigitalAnimation args = new ObjectArgsDigitalAnimation();
				args.nOverlayMethod = this.checkBoxOverlay.Checked ? 1 : 0;
				args.sFileOn = textBoxOnFile.Text;
				args.sFileOff = textBoxOffFile.Text;
				return args;
			}
		}
		*/
	}
}
