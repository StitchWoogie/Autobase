using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using System.IO;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectAnalogStatus.
	/// </summary>
	public class PropertyPageObjectAnalogStatus : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ComboBox comboBoxCondition;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label Condition;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.Button buttonFilename;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ColumnHeader columnHeader3;

		bool bWaitUpdate = false;

		public PropertyPageObjectAnalogStatus()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAnalogStatus));
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.comboBoxCondition = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Condition = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.buttonFilename = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2});
            this.m_list.Font = null;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // comboBoxCondition
            // 
            this.comboBoxCondition.AccessibleDescription = null;
            this.comboBoxCondition.AccessibleName = null;
            resources.ApplyResources(this.comboBoxCondition, "comboBoxCondition");
            this.comboBoxCondition.BackgroundImage = null;
            this.comboBoxCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBoxCondition.Font = null;
            this.comboBoxCondition.Name = "comboBoxCondition";
            this.comboBoxCondition.SelectedIndexChanged += new System.EventHandler(this.comboBoxCondition_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // Condition
            // 
            this.Condition.AccessibleDescription = null;
            this.Condition.AccessibleName = null;
            resources.ApplyResources(this.Condition, "Condition");
            this.Condition.Font = null;
            this.Condition.Name = "Condition";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.AccessibleDescription = null;
            this.textBoxFilename.AccessibleName = null;
            resources.ApplyResources(this.textBoxFilename, "textBoxFilename");
            this.textBoxFilename.BackgroundImage = null;
            this.textBoxFilename.Font = null;
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.TextChanged += new System.EventHandler(this.textBoxFilename_TextChanged);
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
            // PropertyPageObjectAnalogStatus
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.buttonFilename);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Condition);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxCondition);
            this.Controls.Add(this.m_list);
            this.Icon = null;
            this.Name = "PropertyPageObjectAnalogStatus";
            this.Load += new System.EventHandler(this.PropertyPageObjectAnalogStatus_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		readonly string[] sCondition = 
			{ 
				"No Screen",
				"Bit 0 ON", "Bit 1 ON", "Bit 2 ON", "Bit 3 ON",
				"Bit 4 ON", "Bit 5 ON", "Bit 6 ON", "Bit 7 ON",
				"Bit 8 ON", "Bit 9 ON", "Bit A ON", "Bit B ON",
				"Bit C ON", "Bit D ON", "Bit E ON", "Bit F ON",
				"Bit 0 OFF", "Bit 1 OFF", "Bit 2 OFF", "Bit 3 OFF",
				"Bit 4 OFF", "Bit 5 OFF", "Bit 6 OFF", "Bit 7 OFF",
				"Bit 8 OFF", "Bit 9 OFF", "Bit A OFF", "Bit B OFF",
				"Bit C OFF", "Bit D OFF", "Bit E OFF", "Bit F OFF",
				">= HIHI", ">= HIGH", ">= LOW", ">= LOLO",
				"<= HIHI", "<= HIGH", "<= LOW", "<= LOLO",
				"Normal",
			};

		private void PropertyPageObjectAnalogStatus_Load(object sender, System.EventArgs e)
		{
			for(int i = 0; i < sCondition.Length; i++) 
			{
				this.comboBoxCondition.Items.Add(sCondition[i]);
			}
		}

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(bWaitUpdate)	return;
			
			if(m_list.SelectedItems.Count == 0)	return;

			bWaitUpdate = true;

			ListViewItem item = m_list.SelectedItems[0];

			this.textBoxFilename.Text = item.SubItems[2].Text;
			comboBoxCondition.SelectedIndex = (int)item.Tag;

			bWaitUpdate = false;
		}

		void UpdateTextToList()
		{
			if(bWaitUpdate)	return;

			if(m_list.SelectedItems.Count == 0)	return;

			bWaitUpdate = true;

			ListViewItem item = m_list.SelectedItems[0];

			item.SubItems[2].Text = this.textBoxFilename.Text;
			if(comboBoxCondition.SelectedIndex != -1) 
			{
				item.Tag = comboBoxCondition.SelectedIndex;
				item.SubItems[1].Text = sCondition[comboBoxCondition.SelectedIndex];
			}

			bWaitUpdate = false;
		}

		private void textBoxFilename_TextChanged(object sender, System.EventArgs e)
		{
			UpdateTextToList();
		}

		private void comboBoxCondition_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateTextToList();
		}

		private void buttonFilename_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Picture Files(*.*)|*.ani;*.png;*.bmp;*.pcx;*.gif;*.tif;*.jpg;*.wmf;*.emf|Animation Files (*.ani)|*.ani|Bitmap Files (png,bmp,pcx,gif,tif,jpg,wmf,emf)|*.png;*.bmp;*.pcx;*.gif;*.tif;*.jpg;*.wmf;*.emf";

			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				this.textBoxFilename.Text = Path.GetFileName(dialog.FileName);
			}
		}

		public ArrayList Member
		{
			set 
			{
				ANALOG_STATUS_STRUCT status;
				for(int i = 0; i < value.Count; i++) 
				{
					status = (ANALOG_STATUS_STRUCT)value[i];
					ListViewItem item = new ListViewItem();
					item.Text = i.ToString();
					
					item.Tag  = status.type;
					item.SubItems.Add(sCondition[status.type]);
					item.SubItems.Add(status.filename);
					m_list.Items.Add(item);
				}
			}
			get 
			{
				ArrayList list = new ArrayList();

				ANALOG_STATUS_STRUCT status;
				for(int i = 0; i < m_list.Items.Count; i++) 
				{
					status = new ANALOG_STATUS_STRUCT();
					ListViewItem item = m_list.Items[i];
					status.type = (int)item.Tag;
					status.filename = item.SubItems[2].Text;
					list.Add(status);
				}

				return list;
			}
		}
	}
}
