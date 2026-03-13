using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using DialogTag;
using AutoLib;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectGroup.
	/// </summary>
	public class PropertyPageObjectGroup : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView m_list;
		public System.Windows.Forms.CheckBox checkBoxRestoreToOriginalSize;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectGroup()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectGroup));
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.checkBoxRestoreToOriginalSize = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // checkBoxRestoreToOriginalSize
            // 
            resources.ApplyResources(this.checkBoxRestoreToOriginalSize, "checkBoxRestoreToOriginalSize");
            this.checkBoxRestoreToOriginalSize.Name = "checkBoxRestoreToOriginalSize";
            // 
            // PropertyPageObjectGroup
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxRestoreToOriginalSize);
            this.Controls.Add(this.m_list);
            this.Name = "PropertyPageObjectGroup";
            this.Load += new System.EventHandler(this.PropertyPageObjectGroup_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageObjectGroup_Load(object sender, System.EventArgs e)
		{
		    
		}

		ArrayList arrayTag = new ArrayList();

		public void ObjectToList(ObjectGroup group)
		{
			group.GetMultiSelectTagList(arrayTag);

			MULTI_SELECT_TAG_STRUCT list;

			for(int i = 0; i < arrayTag.Count; i++) 
			{
				list = (MULTI_SELECT_TAG_STRUCT)arrayTag[i];
				ListViewItem item = new ListViewItem(list.tagSource);
				item.SubItems.Add(list.tagTarget);
				item.SubItems.Add(list.tag_type.ToString());
				m_list.Items.Add(item);
			}

			if(arrayTag.Count > 0)
				m_list.Items[0].Selected = true;
		}

		public void ListToObject(ObjectGroup group)
		{
			group.SetMultiSelectTagList(arrayTag);
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0)		return;
			ListViewItem item = m_list.SelectedItems[0];
			MULTI_SELECT_TAG_STRUCT list = (MULTI_SELECT_TAG_STRUCT)arrayTag[item.Index];

			FormSelectTag dialog = new FormSelectTag();

			if(list.tag_type == EnumTagType.AI)			dialog.bUseTagAI = true;
			else if(list.tag_type == EnumTagType.AO)	dialog.bUseTagAO = true;
			else if(list.tag_type == EnumTagType.DI)	dialog.bUseTagDI = true;
			else if(list.tag_type == EnumTagType.DO)	dialog.bUseTagDO = true;
			else if(list.tag_type == EnumTagType.ST)	dialog.bUseTagST = true;
			else 
			{
				dialog.bUseTagAI = true;
				dialog.bUseTagAO = true;
				dialog.bUseTagDI = true;
				dialog.bUseTagDO = true;
				dialog.bUseTagST = true;
			}

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				int[] pos = new int[1];

				list.tagTarget = dialog.sTag;
				TagLib.GetTagTypeAndPos(dialog.sTag, ref list.tag_type, ref pos);	
				
				item.SubItems[1].Text = dialog.sTag;
				item.SubItems[2].Text = list.tag_type.ToString();
			}
		}
	}
}
