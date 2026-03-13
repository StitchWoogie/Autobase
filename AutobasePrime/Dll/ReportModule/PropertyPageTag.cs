using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLib;
using DialogTag;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageTag.
	/// </summary>
	public class PropertyPageTag : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TextBox textBoxTag;
		public System.Windows.Forms.TextBox textBoxDes;
		private System.Windows.Forms.Button buttonFindTag;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public bool bUseAI = false;
		public bool bUseAO = false;
		public bool bUseDI = false;
		public bool bUseDO = false;
		public bool bUseST = false;

		public PropertyPageTag()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageTag));
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.buttonFindTag = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxDes = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTag
            // 
            this.textBoxTag.AccessibleDescription = null;
            this.textBoxTag.AccessibleName = null;
            resources.ApplyResources(this.textBoxTag, "textBoxTag");
            this.textBoxTag.BackgroundImage = null;
            this.textBoxTag.Font = null;
            this.textBoxTag.Name = "textBoxTag";
            // 
            // buttonFindTag
            // 
            this.buttonFindTag.AccessibleDescription = null;
            this.buttonFindTag.AccessibleName = null;
            resources.ApplyResources(this.buttonFindTag, "buttonFindTag");
            this.buttonFindTag.BackgroundImage = null;
            this.buttonFindTag.Font = null;
            this.buttonFindTag.Name = "buttonFindTag";
            this.buttonFindTag.Click += new System.EventHandler(this.buttonFindTag_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxTag);
            this.groupBox1.Controls.Add(this.buttonFindTag);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.textBoxDes);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxDes
            // 
            this.textBoxDes.AccessibleDescription = null;
            this.textBoxDes.AccessibleName = null;
            resources.ApplyResources(this.textBoxDes, "textBoxDes");
            this.textBoxDes.BackgroundImage = null;
            this.textBoxDes.Font = null;
            this.textBoxDes.Name = "textBoxDes";
            this.textBoxDes.ReadOnly = true;
            // 
            // PropertyPageTag
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageTag";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetTag(string tag, bool first)
		{
			if(first) 
			{
				textBoxTag.Text = tag;
				int[] tag_pos = new int[1];
				TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);
				textBoxDes.Text = tp.description;
			}
			else 
			{
				if(String.Compare(textBoxTag.Text, tag, true) != 0) 
				{
					textBoxTag.Text = "";
					textBoxDes.Text = "";
				}
			}
		}

		private void buttonFindTag_Click(object sender, System.EventArgs e)
		{
			FormSelectTag tag = new FormSelectTag();

			tag.TopMost = true;

			tag.bUseTagAI = bUseAI;
			tag.bUseTagAO = bUseAO;
			tag.bUseTagDI = bUseDI;
			tag.bUseTagDO = bUseDO;
			tag.bUseTagST = bUseST;

            tag.StartPosition = FormStartPosition.CenterParent;

			if(tag.ShowDialog(this) == DialogResult.OK) 
			{
				textBoxTag.Text = tag.sTag;
				textBoxDes.Text = tag.sDes;
			}
		}
	}
}
