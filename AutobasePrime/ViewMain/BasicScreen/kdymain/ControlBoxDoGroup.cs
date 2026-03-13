using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ControlBoxDigitalOutput.
	/// </summary>
	public class ControlBoxDoGroup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOFF;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonON;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		string		sTag;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView listViewDoGroup;
		int[]		nTagPos = new int[1];

		public ControlBoxDoGroup(string tag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			sTag = tag;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlBoxDoGroup));
            this.buttonOFF = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonON = new System.Windows.Forms.Button();
            this.listViewDoGroup = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // buttonOFF
            // 
            resources.ApplyResources(this.buttonOFF, "buttonOFF");
            this.buttonOFF.Name = "buttonOFF";
            this.buttonOFF.Click += new System.EventHandler(this.buttonOFF_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // buttonON
            // 
            resources.ApplyResources(this.buttonON, "buttonON");
            this.buttonON.Name = "buttonON";
            this.buttonON.Click += new System.EventHandler(this.buttonON_Click);
            // 
            // listViewDoGroup
            // 
            this.listViewDoGroup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewDoGroup.FullRowSelect = true;
            this.listViewDoGroup.HideSelection = false;
            resources.ApplyResources(this.listViewDoGroup, "listViewDoGroup");
            this.listViewDoGroup.MultiSelect = false;
            this.listViewDoGroup.Name = "listViewDoGroup";
            this.listViewDoGroup.UseCompatibleStateImageBehavior = false;
            this.listViewDoGroup.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // ControlBoxDoGroup
            // 
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.listViewDoGroup);
            this.Controls.Add(this.buttonOFF);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonON);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ControlBoxDoGroup";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ControlBoxDigitalOutput_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ControlBoxDigitalOutput_Load(object sender, System.EventArgs e)
		{
			TagDoGroupClass gdo = TagLib.GetStructDoGroup(sTag, ref nTagPos);
			//dout = TagLib.GetDirectTag(dout);

			//labelOutputTag.Text = dout.tag;
			//labelOutputDes.Text = dout.description;

			DoGroupMember member;
			ListViewItem lvi;
			TagPublicClass tp;
				
			for(int i = 0; i < gdo.member.Count; i++) 
			{
				member = (DoGroupMember)gdo.member[i];
				lvi = new ListViewItem(member.tag);
				tp = TagLib.GetStructPublic(member.tag, ref member.pos);
				lvi.SubItems.Add(tp.description);
				this.listViewDoGroup.Items.Add(lvi);
			}

			if(gdo.curr == 0)	buttonON.Select();
			else				buttonOFF.Select();
		}

		private void buttonON_Click(object sender, System.EventArgs e)
		{
			TagDoGroupClass gdo = TagLib.GetStructDoGroup(sTag, ref nTagPos);
			//gdo = TagLib.GetDirectTag(dout);
			TagWrite.WriteCurrDoGroup(gdo, 1, true);
			Close();
		}

		private void buttonOFF_Click(object sender, System.EventArgs e)
		{
			TagDoGroupClass dout = TagLib.GetStructDoGroup(sTag, ref nTagPos);
			//dout = TagLib.GetDirectTag(dout);
			TagWrite.WriteCurrDoGroup(dout, 0, true);
			Close();
		}

	}

}
