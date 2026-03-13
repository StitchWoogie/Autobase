using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLib;
using NetTools;
using System.IO;

namespace DialogTag.TagEditor
{
	/// <summary>
	/// Summary description for FormTagEditor.
	/// </summary>
	public class FormTagEditor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ContextMenu contextMenu1;  
		private System.Windows.Forms.MenuItem menuItemAddTag;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemTagProperties;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.MenuItem menuItemPaste;
        private System.Windows.Forms.MenuItem menuItemSelectAll;
        private IContainer components = null;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TreeView treeViewGroup;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.Button buttonCopy;
		private System.Windows.Forms.Button buttonPaste;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.Button buttonMoveDown;

		bool bChangedFlag = false;
		private System.Windows.Forms.Button buttonInsert;
		private System.Windows.Forms.MenuItem menuItemInsertTag;
        private System.Windows.Forms.Button buttonRunLocalMain;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem importFromCSVFileToolStripMenuItem;
        private ToolStripMenuItem exportToCSVFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem saveWithColumndescriptionToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem;
        private ToolStripMenuItem replaceToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem tagDescriptionAutoNumbringToolStripMenuItem;
		ControlListView userList = new ControlListView();

        int KeyLockGetTotalTagUsed(TagGrClass gr)
        {
            int count = 0;
            TagPublicClass tp;

            for (int i = 0; i < gr.arrayTag.Count; i++)
            {
                tp = (TagPublicClass)gr.arrayTag[i];

                if (tp.enumTagType == EnumTagType.GR)
                {
                    count += KeyLockGetTotalTagUsed((TagGrClass)tp);
                }
                else
                {
                    if (tp.act == 1)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

		void DrawTitle()
		{
			//this.Text = "Local.tagx";
            if (Tools.IsLangKorean())
            {
                this.Text = "태그 편집";
            }
            else
            {
                this.Text = "Tag Editor";
            }

			if(bChangedFlag)
				this.Text += " *";

            int count = KeyLockGetTotalTagUsed(tagTemp);

            if (Tools.IsLangKorean())
            {
                this.Text += String.Format(" (사용된 태그수:{0})", count);
            }
            else
            {
                this.Text += String.Format(" ({0} Tags used.)", count);
            }
		}

		void SetChangedFlag(bool flag)
		{
			bChangedFlag = flag;
			DrawTitle();
		}

		public FormTagEditor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.userList.SmallImageList = TotalResource.res.imageListTagType;

			this.treeViewGroup.Dock = DockStyle.Fill;
			//this.userList.do.Dock = DockStyle.Fill;

			if(bSaveFlag) 
			{
				this.WindowState = nSaveState;
				if(this.WindowState == FormWindowState.Normal) 
				{
					this.Left = nSaveX;
					this.Top = nSaveY;
					this.Width = nSaveWidth;
					this.Height = nSaveHeight;
				}
			}
		}

		static bool bSaveFlag = false;
		static int nSaveWidth = 0;
		static int nSaveHeight = 0;
		static int nSaveX = 0;
		static int nSaveY = 0;
		static FormWindowState nSaveState;

		void SaveEditorCoordinate()
		{
			bSaveFlag = true; 
			nSaveX = this.Left;
			nSaveY = this.Top;
			nSaveWidth = this.Width;
			nSaveHeight = this.Height;
			nSaveState = this.WindowState;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTagEditor));
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemAddTag = new System.Windows.Forms.MenuItem();
            this.menuItemInsertTag = new System.Windows.Forms.MenuItem();
            this.menuItemTagProperties = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemSelectAll = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemPaste = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeViewGroup = new System.Windows.Forms.TreeView();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonRunLocalMain = new System.Windows.Forms.Button();
            this.buttonInsert = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromCSVFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToCSVFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveWithColumndescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tagDescriptionAutoNumbringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAddTag,
            this.menuItemInsertTag,
            this.menuItemTagProperties,
            this.menuItem4,
            this.menuItemSelectAll,
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemDelete,
            this.menuItem1,
            this.menuItem3});
            resources.ApplyResources(this.contextMenu1, "contextMenu1");
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // menuItemAddTag
            // 
            resources.ApplyResources(this.menuItemAddTag, "menuItemAddTag");
            this.menuItemAddTag.Index = 0;
            this.menuItemAddTag.Click += new System.EventHandler(this.menuItemAddTag_Click);
            // 
            // menuItemInsertTag
            // 
            resources.ApplyResources(this.menuItemInsertTag, "menuItemInsertTag");
            this.menuItemInsertTag.Index = 1;
            this.menuItemInsertTag.Click += new System.EventHandler(this.menuItemInsertTag_Click);
            // 
            // menuItemTagProperties
            // 
            resources.ApplyResources(this.menuItemTagProperties, "menuItemTagProperties");
            this.menuItemTagProperties.Index = 2;
            this.menuItemTagProperties.Click += new System.EventHandler(this.menuItemTagProperties_Click);
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 3;
            // 
            // menuItemSelectAll
            // 
            resources.ApplyResources(this.menuItemSelectAll, "menuItemSelectAll");
            this.menuItemSelectAll.Index = 4;
            this.menuItemSelectAll.Click += new System.EventHandler(this.menuItemSelectAll_Click);
            // 
            // menuItemCopy
            // 
            resources.ApplyResources(this.menuItemCopy, "menuItemCopy");
            this.menuItemCopy.Index = 5;
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemPaste
            // 
            resources.ApplyResources(this.menuItemPaste, "menuItemPaste");
            this.menuItemPaste.Index = 6;
            this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
            // 
            // menuItemDelete
            // 
            resources.ApplyResources(this.menuItemDelete, "menuItemDelete");
            this.menuItemDelete.Index = 7;
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 8;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 9;
            // 
            // panel3
            // 
            this.panel3.AccessibleDescription = null;
            this.panel3.AccessibleName = null;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackgroundImage = null;
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.splitter1);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Font = null;
            this.panel3.Name = "panel3";
            // 
            // panel4
            // 
            this.panel4.AccessibleDescription = null;
            this.panel4.AccessibleName = null;
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.BackgroundImage = null;
            this.panel4.Font = null;
            this.panel4.Name = "panel4";
            // 
            // splitter1
            // 
            this.splitter1.AccessibleDescription = null;
            this.splitter1.AccessibleName = null;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.BackgroundImage = null;
            this.splitter1.Font = null;
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.treeViewGroup);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // treeViewGroup
            // 
            this.treeViewGroup.AccessibleDescription = null;
            this.treeViewGroup.AccessibleName = null;
            resources.ApplyResources(this.treeViewGroup, "treeViewGroup");
            this.treeViewGroup.BackgroundImage = null;
            this.treeViewGroup.Font = null;
            this.treeViewGroup.FullRowSelect = true;
            this.treeViewGroup.HideSelection = false;
            this.treeViewGroup.ItemHeight = 14;
            this.treeViewGroup.Name = "treeViewGroup";
            this.treeViewGroup.PathSeparator = ".";
            this.treeViewGroup.ShowNodeToolTips = true;
            this.treeViewGroup.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGroup_AfterSelect);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.buttonRunLocalMain);
            this.panel1.Controls.Add(this.buttonInsert);
            this.panel1.Controls.Add(this.buttonMoveDown);
            this.panel1.Controls.Add(this.buttonMoveUp);
            this.panel1.Controls.Add(this.buttonPaste);
            this.panel1.Controls.Add(this.buttonCopy);
            this.panel1.Controls.Add(this.buttonModify);
            this.panel1.Controls.Add(this.buttonDelete);
            this.panel1.Controls.Add(this.buttonAdd);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // buttonRunLocalMain
            // 
            this.buttonRunLocalMain.AccessibleDescription = null;
            this.buttonRunLocalMain.AccessibleName = null;
            resources.ApplyResources(this.buttonRunLocalMain, "buttonRunLocalMain");
            this.buttonRunLocalMain.BackgroundImage = null;
            this.buttonRunLocalMain.Font = null;
            this.buttonRunLocalMain.Name = "buttonRunLocalMain";
            this.buttonRunLocalMain.Click += new System.EventHandler(this.buttonRunLocalMain_Click);
            // 
            // buttonInsert
            // 
            this.buttonInsert.AccessibleDescription = null;
            this.buttonInsert.AccessibleName = null;
            resources.ApplyResources(this.buttonInsert, "buttonInsert");
            this.buttonInsert.BackgroundImage = null;
            this.buttonInsert.Font = null;
            this.buttonInsert.Name = "buttonInsert";
            this.buttonInsert.Click += new System.EventHandler(this.buttonInsert_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.AccessibleDescription = null;
            this.buttonMoveDown.AccessibleName = null;
            resources.ApplyResources(this.buttonMoveDown, "buttonMoveDown");
            this.buttonMoveDown.BackgroundImage = null;
            this.buttonMoveDown.Font = null;
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.AccessibleDescription = null;
            this.buttonMoveUp.AccessibleName = null;
            resources.ApplyResources(this.buttonMoveUp, "buttonMoveUp");
            this.buttonMoveUp.BackgroundImage = null;
            this.buttonMoveUp.Font = null;
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.AccessibleDescription = null;
            this.buttonPaste.AccessibleName = null;
            resources.ApplyResources(this.buttonPaste, "buttonPaste");
            this.buttonPaste.BackgroundImage = null;
            this.buttonPaste.Font = null;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.AccessibleDescription = null;
            this.buttonCopy.AccessibleName = null;
            resources.ApplyResources(this.buttonCopy, "buttonCopy");
            this.buttonCopy.BackgroundImage = null;
            this.buttonCopy.Font = null;
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonModify
            // 
            this.buttonModify.AccessibleDescription = null;
            this.buttonModify.AccessibleName = null;
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.BackgroundImage = null;
            this.buttonModify.Font = null;
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.AccessibleDescription = null;
            this.buttonDelete.AccessibleName = null;
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.BackgroundImage = null;
            this.buttonDelete.Font = null;
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.AccessibleDescription = null;
            this.buttonAdd.AccessibleName = null;
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.BackgroundImage = null;
            this.buttonAdd.Font = null;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.AccessibleDescription = null;
            this.menuStrip1.AccessibleName = null;
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.BackgroundImage = null;
            this.menuStrip1.Font = null;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.AccessibleDescription = null;
            this.fileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.BackgroundImage = null;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFromCSVFileToolStripMenuItem,
            this.exportToCSVFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveWithColumndescriptionToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // importFromCSVFileToolStripMenuItem
            // 
            this.importFromCSVFileToolStripMenuItem.AccessibleDescription = null;
            this.importFromCSVFileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.importFromCSVFileToolStripMenuItem, "importFromCSVFileToolStripMenuItem");
            this.importFromCSVFileToolStripMenuItem.BackgroundImage = null;
            this.importFromCSVFileToolStripMenuItem.Name = "importFromCSVFileToolStripMenuItem";
            this.importFromCSVFileToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.importFromCSVFileToolStripMenuItem.Click += new System.EventHandler(this.importFromCSVFileToolStripMenuItem_Click);
            // 
            // exportToCSVFileToolStripMenuItem
            // 
            this.exportToCSVFileToolStripMenuItem.AccessibleDescription = null;
            this.exportToCSVFileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.exportToCSVFileToolStripMenuItem, "exportToCSVFileToolStripMenuItem");
            this.exportToCSVFileToolStripMenuItem.BackgroundImage = null;
            this.exportToCSVFileToolStripMenuItem.Name = "exportToCSVFileToolStripMenuItem";
            this.exportToCSVFileToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.exportToCSVFileToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // saveWithColumndescriptionToolStripMenuItem
            // 
            this.saveWithColumndescriptionToolStripMenuItem.AccessibleDescription = null;
            this.saveWithColumndescriptionToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.saveWithColumndescriptionToolStripMenuItem, "saveWithColumndescriptionToolStripMenuItem");
            this.saveWithColumndescriptionToolStripMenuItem.BackgroundImage = null;
            this.saveWithColumndescriptionToolStripMenuItem.Checked = true;
            this.saveWithColumndescriptionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveWithColumndescriptionToolStripMenuItem.Name = "saveWithColumndescriptionToolStripMenuItem";
            this.saveWithColumndescriptionToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.saveWithColumndescriptionToolStripMenuItem.Click += new System.EventHandler(this.saveWithColumndescriptionToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.AccessibleDescription = null;
            this.editToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.BackgroundImage = null;
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.toolStripSeparator2,
            this.tagDescriptionAutoNumbringToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.AccessibleDescription = null;
            this.findToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.findToolStripMenuItem, "findToolStripMenuItem");
            this.findToolStripMenuItem.BackgroundImage = null;
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.AccessibleDescription = null;
            this.replaceToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.replaceToolStripMenuItem, "replaceToolStripMenuItem");
            this.replaceToolStripMenuItem.BackgroundImage = null;
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AccessibleDescription = null;
            this.toolStripSeparator2.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // tagDescriptionAutoNumbringToolStripMenuItem
            // 
            this.tagDescriptionAutoNumbringToolStripMenuItem.AccessibleDescription = null;
            this.tagDescriptionAutoNumbringToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.tagDescriptionAutoNumbringToolStripMenuItem, "tagDescriptionAutoNumbringToolStripMenuItem");
            this.tagDescriptionAutoNumbringToolStripMenuItem.BackgroundImage = null;
            this.tagDescriptionAutoNumbringToolStripMenuItem.Name = "tagDescriptionAutoNumbringToolStripMenuItem";
            this.tagDescriptionAutoNumbringToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.tagDescriptionAutoNumbringToolStripMenuItem.Click += new System.EventHandler(this.tagDescriptionAutoNumbringToolStripMenuItem_Click);
            // 
            // FormTagEditor
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = null;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "FormTagEditor";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormTagEditor_Load);
            this.SizeChanged += new System.EventHandler(this.FormTagEditor_SizeChanged_1);
            this.Closed += new System.EventHandler(this.FormTagEditor_Closed);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormTagEditor_Closing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormTagEditor_KeyDown);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		int nTempPos=0;

		TagGrClass tagTemp;

		void OnDoubleClick()
		{
			Modify();
		}

        //public static bool bTagFileChangedByOutside = false;

        void LoadTempTag()
        {
            if (Editor.checkTagFileChanged.IsChanged())
            {
                string msg;
                DialogResult result;

                if (Tools.IsLangKorean())
                {
                    msg = String.Format("{0}\n\n태그 파일이 스튜디오 외부에서 변경되었습니다.\n다시 불러올까요?", "Local.tagx");
                    result = MessageBox.Show(msg, "File Changed", MessageBoxButtons.YesNo);
                }
                else
                {
                    msg = String.Format("{0}\n\nThe TAG file has been modified outside of the Studio.\nDo you want to reload it?", "Local.tagx");
                    result = MessageBox.Show(msg, "File Changed", MessageBoxButtons.YesNo);
                }

                if (result == DialogResult.Yes)
                {
                    TerminalClass.Init();
                }

                Editor.checkTagFileChanged.Reset();
            }

            tagTemp = (TagGrClass)TagLib.groupRoot.CopyObjectOnStudio();
        }

		private void FormTagEditor_Load(object sender, System.EventArgs e)
		{
			userList.bMultiRowSelect = true;
			userList.bOwnerDraw = true;
			userList.TopLevel = false;
			userList.font = this.Font;
			userList.contextMenu = this.contextMenu1;
			userList.doubleClick = new ControlListView.OnEventDoubleClick(OnDoubleClick);
            userList.onEventColumnHeaderClick = new ControlListView.OnEventColumnHeaderClick(OnColumnHeaderClick);
            userList.HeaderStyle = ColumnHeaderStyle.Clickable;
            //userList.Dock = DockStyle.Fill;

			this.panel4.Controls.Add(userList);

			ControlListViewHeader header;

			StringFormat center = new StringFormat();
			center.Alignment = StringAlignment.Center;

			header = new ControlListViewHeader();
			if(Tools.IsLangKorean())
				header.text = "순서";
			else if(Tools.IsLangJapanese())
				header.text = "番号";
			else if(Tools.IsLangChinese())
				header.text = "顺序";
            else if (Tools.IsLangVietnamese())
                header.text = "Số";
			else
				header.text = "No";
			header.format = DrawClass.StringFormatLeft;
			header.width = 75;
            header.bClickable = false;
			userList.header.Add(header);

			header = new ControlListViewHeader();
			if(Tools.IsLangKorean())
				header.text = "태그";
			else if(Tools.IsLangJapanese())
				header.text = "タグ";
			else if(Tools.IsLangChinese())
				header.text = "标记";
			else
				header.text = "Tag";
			header.format = DrawClass.StringFormatLeft;
			header.width = 100;
            header.bClickable = true;
			userList.header.Add(header);

			header = new ControlListViewHeader();
			if(Tools.IsLangKorean())
				header.text = "설명";
			else if(Tools.IsLangJapanese())
				header.text = "説明";
			else if(Tools.IsLangChinese())
				header.text = "标记描述";
            else if (Tools.IsLangVietnamese())
                header.text = "Mô tả";
			else
				header.text = "Description";

			header.format = DrawClass.StringFormatLeft;
            header.bClickable = true;
			header.width = 100;

			userList.header.Add(header);

			header = new ControlListViewHeader();
			if(Tools.IsLangKorean())
				header.text = "종류";
			else if(Tools.IsLangJapanese())
				header.text = "タイプ";
			else if(Tools.IsLangChinese())
				header.text = "类型";
            else if (Tools.IsLangVietnamese())
                header.text = "Kiểu";
			else
				header.text = "Type";

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
            }
            else
                header.format = center;

			header.width = 40;
            header.bClickable = true;

			userList.header.Add(header);

			header = new ControlListViewHeader();
			if(Tools.IsLangKorean())
				header.text = "사용";
			else if(Tools.IsLangJapanese())
				header.text = "使用";
			else if(Tools.IsLangChinese())
				header.text = "启用";
            else if (Tools.IsLangVietnamese())
                header.text = "Kích hoạt";
			else
				header.text = "Act";
            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
            }
            else
                header.format = center;
			header.width = 40;
            header.bClickable = true;
			userList.header.Add(header);

			header = new ControlListViewHeader();
			if(Tools.IsLangKorean())
				header.text = "연결";
			else if(Tools.IsLangJapanese())
				header.text = "連結";
			else if(Tools.IsLangChinese())
				header.text = "连接";
            else if (Tools.IsLangVietnamese())
                header.text = "Liên kết";
			else
				header.text = "Link";

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
            }
            else
                header.format = center;

			header.width = 80;
            header.bClickable = true;
			userList.header.Add(header);

            string[] etc_header = { "포트", "스테이션", "주소", "Extra1", "Extra2" };

			for(int i = 0; i < 5; i++) 
			{
				header = new ControlListViewHeader();
                if(TotalConfig.eOemType == EnumOemType.UYeG_GS)
                    header.text = etc_header[i];
                else
                    header.text = "";
				header.format = center;
				header.width = 100;
                header.bClickable = true;
				userList.header.Add(header);
			}

			userList.paintMessage = new ControlListView.OnEventPaintMessage(OnListPaint);

			userList.Show();

			//tagTemp = (TagGrClass)Tools.CopyObject(TagLib.groupRoot); 속도가 느리다.
            LoadTempTag();
            
			            
			TagLib.FillGroupTree(tagTemp, treeViewGroup);
			treeViewGroup.Nodes[0].Expand();
			treeViewGroup.SelectedNode = treeViewGroup.Nodes[0];
			
			/*
			if(nTempPos < m_list.Items.Count) 
			{
				ListViewItem item = m_list.Items[nTempPos];
				item.Selected = true;
				item.EnsureVisible();
			}
			*/
			userList.currPos = nTempPos;

			DrawTitle();

			TotalConfig.AutoBaseListCtrlConfigLoad(this.userList, "TagEditor", "TagList");

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {
                if (Tools.IsLangKorean())
                    this.buttonModify.Text = "태그 수정";
            }
		}

		protected void OnListPaint(System.Drawing.Graphics g, Rectangle r)
		{
			if(r.Bottom < userList.headHeight) return;
			if(r.Top >= userList.headHeight) DrawClass.gcls(g, r, userList.backColor);
			else				    DrawClass.gcls(g, 0, userList.headHeight, Width, r.Bottom, userList.backColor);
			if(userList.pageLineCount <= 0 || userList.fontY <= 0 || userList.listHap <= 0) return;
			
			int						pos, x, y = userList.headHeight, xGap = (int)(userList.fontX*0.25), endPos;
			TagPublicClass				tp;

			string path = GetGroupName();
			TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);

			endPos = userList.pageLineCount+userList.startPos+1;		// 1줄 더 그린다
			if(endPos > userList.listHap) endPos = userList.listHap;
			
			for(pos = userList.startPos; pos < endPos; pos++, y += userList.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+userList.fontY < r.Top) continue;

				tp = (TagPublicClass)gr.arrayTag[pos];

				if(tp == null) continue;
				x = userList.startX;
				oneLineDraw(g, tp, pos+1, x, y, xGap);
			}
		}

		void oneLineDraw(Graphics g, TagPublicClass tp, int i, int x, int y, int xGap)
		{
			Color					color;
			ControlListViewHeader	head = (ControlListViewHeader)userList.header[0];
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			string buf;

			int ImageIndex;


			if(tp.enumTagType == EnumTagType.AI)		ImageIndex = (int)TotalResource.ImageTagType.AI;
			else if(tp.enumTagType == EnumTagType.AO)	ImageIndex = (int)TotalResource.ImageTagType.AO;
			else if(tp.enumTagType == EnumTagType.DI)	ImageIndex = (int)TotalResource.ImageTagType.DI;
			else if(tp.enumTagType == EnumTagType.DO)	ImageIndex = (int)TotalResource.ImageTagType.DO;
			else if(tp.enumTagType == EnumTagType.ST)	ImageIndex = (int)TotalResource.ImageTagType.ST;
			else if(tp.enumTagType == EnumTagType.GR)	ImageIndex = (int)TotalResource.ImageTagType.GR;
			else if(tp.enumTagType == EnumTagType.GDO)	ImageIndex = (int)TotalResource.ImageTagType.GDO;
			else 										ImageIndex = (int)TotalResource.ImageTagType.Unknown;

			Image image = TotalResource.res.imageListTagType.Images[ImageIndex];

			g.DrawImageUnscaled(image, x+xGap, y); 
						
			color = tp.act == 1 ? Color.Black : Color.LightGray;
			DrawClass.WinDrawText(g, x+xGap+image.Width, y, head.width-xGap*2-image.Width, (int)userList.fontY, i.ToString(), color, userList.backColor, userList.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)userList.header[1];
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.name, color, userList.backColor, userList.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)userList.header[2];
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.description, color, userList.backColor, userList.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)userList.header[3];
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.enumTagType.ToString(), color, userList.backColor, userList.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)userList.header[4];
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.act.ToString(), color, userList.backColor, userList.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)userList.header[5];

			if(tp.cTagLinkType == 0)		buf = "PlcScan";
			else if(tp.cTagLinkType == 1)	buf = "DDE";
			else if(tp.cTagLinkType == 2)	buf = "Memory";
			else if(tp.cTagLinkType == 3)	buf = "Indirect";
			else if(tp.cTagLinkType == 4)	buf = "SYSTEM";
			else if(tp.cTagLinkType == 5)	buf = "OPC";
			else 							buf = "?";

			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, buf, color, userList.backColor, userList.font, head.format);

			if(tp.cTagLinkType == 1)	// DDE
			{
				x += head.width;
				head = (ControlListViewHeader)userList.header[6];
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.sDdeService, color, userList.backColor, userList.font, head.format);

				x += head.width;
				head = (ControlListViewHeader)userList.header[7];
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.sDdeTopic, color, userList.backColor, userList.font, head.format);

				x += head.width;
				head = (ControlListViewHeader)userList.header[8];
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.sDdeItem, color, userList.backColor, userList.font, head.format);
			}
			else if(tp.cTagLinkType == 5) // OPC
			{
				x += head.width;
				head = (ControlListViewHeader)userList.header[6];
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.sOpcServer, color, userList.backColor, userList.font, head.format);

				x += head.width;
				head = (ControlListViewHeader)userList.header[7];
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.sOpcGroup, color, userList.backColor, userList.font, head.format);

				x += head.width;
				head = (ControlListViewHeader)userList.header[8];
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, tp.sOpcItem, color, userList.backColor, userList.font, head.format);
			}
			else 
			{
				if(tp.enumTagType == EnumTagType.AI) 
				{
					TagAiClass ai = (TagAiClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						x += head.width;
						head = (ControlListViewHeader)userList.header[6];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ai.port.ToString(), color, userList.backColor, userList.font, head.format);

                        x += head.width;
                        head = (ControlListViewHeader)userList.header[7];
                        DrawClass.WinDrawText(g, x + xGap, y, head.width - xGap * 2, (int)userList.fontY, ai.station.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[8];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ai.address.ToString(), color, userList.backColor, userList.font, head.format);
					}
				}
				else if(tp.enumTagType == EnumTagType.AO) 
				{
					TagAoClass ao = (TagAoClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						x += head.width;
						head = (ControlListViewHeader)userList.header[6];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ao.port.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[7];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ao.station.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[8];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ao.address.ToString("X"), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[9];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ao.sExtraAddr, color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[10];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, ao.wExtraAddr.ToString(), color, userList.backColor, userList.font, head.format);
					}
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					TagDiClass di = (TagDiClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						x += head.width;
						head = (ControlListViewHeader)userList.header[6];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, di.port.ToString(), color, userList.backColor, userList.font, head.format);

                        x += head.width;
                        head = (ControlListViewHeader)userList.header[7];
                        DrawClass.WinDrawText(g, x + xGap, y, head.width - xGap * 2, (int)userList.fontY, di.station.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[8];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, di.address_word.ToString()+"."+di.address_bit.ToString("X"), color, userList.backColor, userList.font, head.format);
					}
				}
				else if(tp.enumTagType == EnumTagType.DO) 
				{
					TagDoClass dout = (TagDoClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						x += head.width;
						head = (ControlListViewHeader)userList.header[6];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, dout.port.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[7];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, dout.station.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[8];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, dout.address.ToString("X"), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[9];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, dout.sExtraAddr, color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[10];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, dout.wExtraAddr.ToString(), color, userList.backColor, userList.font, head.format);
					}
				}
				else if(tp.enumTagType == EnumTagType.ST) 
				{
					TagStClass st = (TagStClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						x += head.width;
						head = (ControlListViewHeader)userList.header[6];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, st.port.ToString(), color, userList.backColor, userList.font, head.format);

						x += head.width;
						head = (ControlListViewHeader)userList.header[8];
						DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)userList.fontY, st.address.ToString(), color, userList.backColor, userList.font, head.format);
					}
				}
			}

		}

		private void treeViewGroup_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			FillListBox();
		}

		string GetGroupName()
		{
            return GetGroupName(this.treeViewGroup.SelectedNode);
		}

        string GetGroupName(TreeNode node)
        {
            string path;

            if (node == null)
            {
                path = "";
            }
            else
            {
                path = node.FullPath;
                int index = path.IndexOf('.');
                if (index == -1)
                    path = "";
                else
                    path = path.Substring(path.IndexOf('.') + 1);	// local.을 제거한다.
            }

            return path;
        }

		void AddGroupToTree(TagGrClass gr)
		{
			if(treeViewGroup.SelectedNode == null) 
			{
				
			}
			else 
			{
				TreeNode node = new TreeNode(gr.name);
				treeViewGroup.SelectedNode.Nodes.Add(node);
				TagLib.RecurseGroupTree(node, gr.arrayTag);
				treeViewGroup.SelectedNode.Expand();
			}
		}

		void FillListBox()
		{
			string path = GetGroupName();

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);

			userList.selectIndices.Clear();
			userList.listHap = gr.arrayTag.Count;
			userList.listItemChanged();
		}

		/*
		void ChangeViewItem(TagPublicClass tp, ListViewItem lvi)
		{
			if(tp.act == 0)	lvi.ForeColor = Color.DarkGray;
			else			lvi.ForeColor = Color.Black;

			if(tp.enumTagType == EnumTagType.AI)		lvi.ImageIndex = (int)TotalResource.ImageTagType.AI;
			else if(tp.enumTagType == EnumTagType.AO)	lvi.ImageIndex = (int)TotalResource.ImageTagType.AO;
			else if(tp.enumTagType == EnumTagType.DI)	lvi.ImageIndex = (int)TotalResource.ImageTagType.DI;
			else if(tp.enumTagType == EnumTagType.DO)	lvi.ImageIndex = (int)TotalResource.ImageTagType.DO;
			else if(tp.enumTagType == EnumTagType.ST)	lvi.ImageIndex = (int)TotalResource.ImageTagType.ST;
			else if(tp.enumTagType == EnumTagType.GR)	lvi.ImageIndex = (int)TotalResource.ImageTagType.GR;
			else if(tp.enumTagType == EnumTagType.GDO)	lvi.ImageIndex = (int)TotalResource.ImageTagType.GDO;
			else 										lvi.ImageIndex = (int)TotalResource.ImageTagType.Unknown;
			
			lvi.SubItems[0].Text = tp.name;
			lvi.SubItems[1].Text = tp.description;
			lvi.SubItems[2].Text = tp.enumTagType.ToString();
			lvi.SubItems[3].Text = tp.act.ToString();

			if(tp.cTagLinkType == 0)		lvi.SubItems[4].Text = "PlcScan";
			else if(tp.cTagLinkType == 1)	lvi.SubItems[4].Text = "DDE";
			else if(tp.cTagLinkType == 2)	lvi.SubItems[4].Text = "Memory";
			else if(tp.cTagLinkType == 3)	lvi.SubItems[4].Text = "Indirect";
			else if(tp.cTagLinkType == 4)	lvi.SubItems[4].Text = "SYSTEM";
			else if(tp.cTagLinkType == 5)	lvi.SubItems[4].Text = "OPC";
			else 							lvi.SubItems[4].Text = "?";

			if(tp.cTagLinkType == 1)	// DDE
			{
				lvi.SubItems[5].Text = tp.sDdeService;
				lvi.SubItems[6].Text = tp.sDdeTopic;
				lvi.SubItems[7].Text = tp.sDdeItem;
			}
			else if(tp.cTagLinkType == 5) // OPC
			{
				lvi.SubItems[5].Text = tp.sOpcServer;
				lvi.SubItems[6].Text = tp.sOpcGroup;
				lvi.SubItems[7].Text = tp.sOpcItem;
			}
			else 
			{
				if(tp.enumTagType == EnumTagType.AI) 
				{
					TagAiClass ai = (TagAiClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						lvi.SubItems[5].Text = ai.port.ToString();
						lvi.SubItems[7].Text = ai.address.ToString();
					}
				}
				else if(tp.enumTagType == EnumTagType.AO) 
				{
					TagAoClass ao = (TagAoClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						lvi.SubItems[5].Text = ao.port.ToString();
						lvi.SubItems[6].Text = ao.station.ToString();
						lvi.SubItems[7].Text = ao.address.ToString("X");
						lvi.SubItems[8].Text = ao.sExtraAddr;
						lvi.SubItems[9].Text = ao.wExtraAddr.ToString();
					}
				}
				else if(tp.enumTagType == EnumTagType.DI) 
				{
					TagDiClass di = (TagDiClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						lvi.SubItems[5].Text = di.port.ToString();
						lvi.SubItems[7].Text = di.address_word.ToString()+"."+di.address_bit.ToString();
					}
				}
				else if(tp.enumTagType == EnumTagType.DO) 
				{
					TagDoClass dout = (TagDoClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						lvi.SubItems[5].Text = dout.port.ToString();
						lvi.SubItems[6].Text = dout.station.ToString();
						lvi.SubItems[7].Text = dout.address.ToString("X");
						lvi.SubItems[8].Text = dout.sExtraAddr;
						lvi.SubItems[9].Text = dout.wExtraAddr.ToString();
					}
				}
				else if(tp.enumTagType == EnumTagType.ST) 
				{
					TagStClass st = (TagStClass)tp;
					if(tp.cTagLinkType == 0) 
					{
						lvi.SubItems[5].Text = st.port.ToString();
						lvi.SubItems[7].Text = st.address.ToString();
					}
				}
			}
		}
		*/

		void AddTag()
		{
			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());
			FormTagProperty dialog = new FormTagProperty(gr.arrayTag);//this.m_list);

			if(Tools.IsLangKorean())		dialog.sMainText = "태그 추가";
			else if(Tools.IsLangJapanese())	dialog.sMainText = "タグ追加";
			else if(Tools.IsLangChinese())	dialog.sMainText = "添加标记";
			else							dialog.sMainText = "Add Tag";

			//dialog.procPrevNext = new DialogTag.TagEditor.FormTagProperty.DelegatePrevNext(this.OnPrevNext);
			dialog.bEditTagName = true;
			dialog.bModeAdd = true;
			dialog.sGroupName = GetGroupName();
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				TagPublicClass tp = dialog.NewTag();
				gr.arrayTag.Add(tp);

				userList.listHap = gr.arrayTag.Count; 
				userList.listItemChanged();
				userList.selectIndices.Clear();

				userList.currPos = userList.listHap-1;
				userList.listItemChanged();

				if(tp.enumTagType == EnumTagType.GR) 
				{
					AddGroupToTree((TagGrClass)tp);
				}

				SetChangedFlag(true);
			}
		}

		private void menuItemAddTag_Click(object sender, System.EventArgs e)
		{
			AddTag();
		}


        /// <summary>
        /// 왼쪽의 트리의 그룹 이름을 바꾸어 준다.
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="old_name"></param>
		void ChangeGroupFromTree(TagGrClass gr, string old_name)
		{
			if(treeViewGroup.SelectedNode == null) 
			{
				
			}
			else 
			{
				TreeNode parent_node = treeViewGroup.SelectedNode;

				TreeNode node;
				for(int i = 0; i < parent_node.Nodes.Count; i++) 
				{
					node = parent_node.Nodes[i];
					if(String.Compare(node.Text, old_name, true) == 0) 
					{
						node.Text = gr.name;
						return;
					}
				}
			}
		}

		bool ModifyRetry()
		{
			if(this.userList.listHap == 0)				return false;
			if(this.userList.selectIndices.Count == 0)	return false;

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());
			FormTagProperty dialog = new FormTagProperty(gr.arrayTag); 
			//dialog.procPrevNext = new DialogTag.TagEditor.FormTagProperty.DelegatePrevNext(this.OnPrevNext);
			dialog.bEditTagName = true;
			dialog.sGroupName = GetGroupName();

			int lvi;
			TagPublicClass tp;
			int[] pos = new int[1];
			dialog.MultiSelect = (this.userList.selectIndices.Count > 1);

			if(dialog.MultiSelect == false)
				dialog.listViewOwnerPos = (int)(this.userList.selectIndices[0]);
			
			for(int i = 0; i < userList.selectIndices.Count; i++) 
			{
				lvi = (int)userList.selectIndices[i];
				tp = (TagPublicClass)gr.arrayTag[lvi];
				dialog.SetTag(tp);
			}

			string old_name;

            dialog.StartPosition = FormStartPosition.CenterParent;
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				for(int i = 0; i < userList.selectIndices.Count; i++) 
				{
					lvi = (int)userList.selectIndices[i];
					tp = (TagPublicClass)gr.arrayTag[lvi];
					old_name = tp.name;
					dialog.GetTag(tp);
					if(!dialog.MultiSelect && tp.enumTagType == EnumTagType.GR && old_name != tp.name) 
					{
						ChangeGroupFromTree((TagGrClass)tp, old_name);
                        RecurseCalcTagName((TagGrClass)tp);
                        //ChangeGroupFromTree((TagGrClass)tp, old_name);
					}
				}

				SetChangedFlag(true);

				if(dialog.nEndMethod == 1) 
				{
					int index = (int)this.userList.selectIndices[0];
					if(index == 0)	return false;

					this.userList.selectIndices.Clear();
					index--;
					this.userList.currPos = index;
					this.userList.selectIndices.Add(index);
					this.userList.listItemChanged();

					return true;
				}
				else if(dialog.nEndMethod == 2) 
				{
					int index = (int)this.userList.selectIndices[0];
					if(index >= userList.listHap-1)	return false;

					this.userList.selectIndices.Clear();

					index++;
					this.userList.currPos = index;
					this.userList.selectIndices.Add(index);
					this.userList.listItemChanged();

					return true;
				}
				else 
				{

				}
			}

			return false;
		}

		void Modify()
		{
			while(true) 
			{
				if(!ModifyRetry())	break;
			}

			
		}

		/*
		void Modify()
		{
			retry:

			if(this.userList.listHap == 0)			return;
			if(this.userList.selectIndices.Count == 0)	return;

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());
			FormTagProperty dialog = new FormTagProperty(gr.arrayTag); 
			//dialog.procPrevNext = new DialogTag.TagEditor.FormTagProperty.DelegatePrevNext(this.OnPrevNext);
			dialog.bEditTagName = true;
			dialog.sGroupName = GetGroupName();

			int lvi;
			TagPublicClass tp;
			int[] pos = new int[1];
			dialog.MultiSelect = (this.userList.selectIndices.Count > 1);

			if(dialog.MultiSelect == false)
				dialog.listViewOwnerPos = (int)(this.userList.selectIndices[0]);
			
			for(int i = 0; i < userList.selectIndices.Count; i++) 
			{
				lvi = (int)userList.selectIndices[i];
				tp = (TagPublicClass)gr.arrayTag[lvi];
				dialog.SetTag(tp);
			}

			string old_name;
			
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				for(int i = 0; i < userList.selectIndices.Count; i++) 
				{
					lvi = (int)userList.selectIndices[i];
					tp = (TagPublicClass)gr.arrayTag[lvi];
					old_name = tp.name;
					dialog.GetTag(tp);
					if(!dialog.MultiSelect && tp.enumTagType == EnumTagType.GR && old_name != tp.name) 
					{
						ChangeGroupFromTree((TagGrClass)tp, old_name);
					}
				}

				SetChangedFlag(true);

				if(dialog.nEndMethod == 1) 
				{
					int index = (int)this.userList.selectIndices[0];
					if(index == 0)	return;

					this.userList.selectIndices.Clear();
					index--;
					this.userList.currPos = index;
					this.userList.selectIndices.Add(index);
					this.userList.listItemChanged();

					goto retry;
				}
				else if(dialog.nEndMethod == 2) 
				{
					int index = (int)this.userList.selectIndices[0];
					if(index >= userList.listHap-1)	return;

					this.userList.selectIndices.Clear();

					index++;
					this.userList.currPos = index;
					this.userList.selectIndices.Add(index);
					this.userList.listItemChanged();

					goto retry;
				}
				else 
				{

				}
			}
		}
		*/

		private void menuItemTagProperties_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

		static ArrayList arrayCopy = new ArrayList();

		void CopyGo()
		{
			if(this.userList.selectIndices.Count == 0)	return;
			if(this.userList.listHap == 0)				return;

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());

			arrayCopy.Clear();

			int index;

			for(int i = 0; i < this.userList.selectIndices.Count; i++) 
			{
				index = (int)userList.selectIndices[i];
                object obj = ((TagPublicClass)gr.arrayTag[index]).CopyObjectOnStudio();
				arrayCopy.Add(obj);
			}
		}

		private void menuItemCopy_Click(object sender, System.EventArgs e)
		{
			CopyGo();
		}

        /// <summary>
        /// 유일한 새 태그 이름을 만든다. 
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="source"></param>
        /// <param name="new_number">번호가 부여되었을 때 태그 설명에도 사용할 수 있게 번호를 돌려준다.</param>
        /// <returns></returns>
		public static string MakeNewTagName(ArrayList gr, string source, out int new_number)
		{
			string tag;
			int start = 0;
			int number_size = 0;
			int start_pos;

			// 이름끝에 숫자가 붙은 개수와 값을 찾는다.
			for(start_pos = source.Length-1; start_pos > 0; start_pos--) 
			{
				if(source[start_pos] >= '0' && source[start_pos] <= '9') 
				{
					number_size++;
				}
				else 
				{
					break;
				}
			}

			string target;
			string format;
			string left_string;	// 숫자 앞쪽의 문자열
			
			if(number_size == 0) // 뒤쪽에 숫자 부분이 없을 경우 앞쪽부터 숫자가 있는가를 검사한다.
			{
				// 숫자가 붙은 개수와 값을 찾는다.
				bool number_start = false;
				for(int i = 0; i < source.Length; i++) 
				{
					if(source[i] >= '0' && source[i] <= '9') 
					{
						if(number_start == false) 
						{
							number_start = true;
							start_pos = i;
						}
						number_size++;
					}
					else 
					{
						if(number_start == true) 
						{
							break;
						}
					}
				}

				if(number_start) // 앞쪽에 숫자를 찾았다.
				{
					start = ConvertTool.ToInt32(source.Substring(start_pos, number_size));

					left_string = source.Substring(0, start_pos);			// 숫자 앞쪽의 문자열
					string right_string = source.Substring(start_pos+number_size);	// 숫자 뒤쪽의 문자열

					while(true) 
					{
						target = left_string;
						format = "{0:";
						for(int i = 0; i < number_size; i++) 
						{
							format += "0";
						}
						format += "}";
						target += String.Format(format, start);
						target += right_string;

						for(int i = 0; i < gr.Count; i++) 
						{
							tag = ((TagPublicClass)gr[i]).name;
							if(String.Compare(target, tag, true) == 0)	goto next; 
						}

                        new_number = start;

						return target;

					next:
						start++;
						if(number_size == 0)	number_size = 1;		
					}
				}
			}

			start_pos++;

			if(number_size == 0) 
			{
				start = 0;
			}
			else 
			{
				start = ConvertTool.ToInt32(source.Substring(start_pos));
			}

			left_string = source.Substring(0, start_pos);

			while(true) 
			{
				if(number_size == 0)	// 다른 그룹에 복사할 수 있으므로 처음에는 원본이름을 가지고 한다.
				{
					target = source;
				}
				else 
				{
					target = left_string;
					format = "{0:";
					for(int i = 0; i < number_size; i++) 
					{
						format += "0";
					}
					format += "}";
					target += String.Format(format, start);
				}

				for(int i = 0; i < gr.Count; i++) 
				{
					tag = ((TagPublicClass)gr[i]).name;
					if(String.Compare(target, tag, true) == 0)	goto next; 
				}

                new_number = start;

				return target;

			next:
				start++;
				if(number_size == 0)	number_size = 1;		
			}
		}

        /// <summary>
        /// 태그 설명에 태그명과 같은 번호를 부여한다.
        /// </summary>
        public static string MakeNumberingString(string source, int new_number)
        {
            if (new_number == 0) return source;

            int start = new_number;
            int number_size = 0;
            int start_pos;

            // 이름끝에 숫자가 붙은 개수와 값을 찾는다.
            for (start_pos = source.Length - 1; start_pos > 0; start_pos--)
            {
                if (source[start_pos] >= '0' && source[start_pos] <= '9')
                {
                    number_size++;
                }
                else
                {
                    break;
                }
            }

            string target;
            string format;
            string left_string;	// 숫자 앞쪽의 문자열

            if (number_size == 0) // 뒤쪽에 숫자 부분이 없을 경우 앞쪽부터 숫자가 있는가를 검사한다.
            {
                // 숫자가 붙은 개수와 값을 찾는다.
                bool number_start = false;
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] >= '0' && source[i] <= '9')
                    {
                        if (number_start == false)
                        {
                            number_start = true;
                            start_pos = i;
                        }
                        number_size++;
                    }
                    else
                    {
                        if (number_start == true)
                        {
                            break;
                        }
                    }
                }

                if (number_start) // 앞쪽에 숫자를 찾았다.
                {
                    //start = new_number; // ConvertTool.ToInt32(source.Substring(start_pos, number_size));

                    left_string = source.Substring(0, start_pos);			// 숫자 앞쪽의 문자열
                    string right_string = source.Substring(start_pos + number_size);	// 숫자 뒤쪽의 문자열

                    target = left_string;
                    format = "{0:";
                    for (int i = 0; i < number_size; i++)
                    {
                        format += "0";
                    }

                    format += "}";
                    target += String.Format(format, start);
                    target += right_string;

                    return target;

                }
            }

            start_pos++;

            left_string = source.Substring(0, start_pos);


            if (number_size == 0)
            {
                number_size = 1;
            }

            target = left_string;
            format = "{0:";
            for (int i = 0; i < number_size; i++)
            {
                format += "0";
            }
            format += "}";
            target += String.Format(format, start);

            return target;
        }

        
		/// <summary>
        /// name으로 tagname을 그룹에 맞추어서 다시 명명한다.
		/// </summary>
		/// <param name="gr"></param>
		public static void RecurseCalcTagName(TagGrClass gr)
		{
			TagPublicClass tp;
			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				tp = (TagPublicClass)gr.arrayTag[i];
				tp.tag = gr.tag+"."+tp.name;
				if(tp.enumTagType == EnumTagType.GR) 
				{
					RecurseCalcTagName((TagGrClass)tp);
				}
			}
		}

		void PasteGo()
		{
			if(arrayCopy.Count == 0)	return;

			string group_name = GetGroupName();
			TagGrClass gr = TagLib.GetGroupArray(tagTemp, group_name);
			TagPublicClass tp;

			this.userList.selectIndices.Clear();

			string new_name;
            int new_number;

			for(int i = 0; i < arrayCopy.Count; i++) 
			{
                tp = (TagPublicClass)(((TagPublicClass)arrayCopy[i]).CopyObjectOnStudio());
				new_name = tp.name = MakeNewTagName(gr.arrayTag, tp.name, out new_number);

				if(group_name.Length == 0)
					tp.tag = tp.name;
				else
					tp.tag = group_name+"."+tp.name;

                if (this.tagDescriptionAutoNumbringToolStripMenuItem.Checked)
                    tp.description = MakeNumberingString(tp.description, new_number);

				gr.arrayTag.Add(tp);

				userList.selectIndices.Add(gr.arrayTag.Count-1);
				userList.currPos = gr.arrayTag.Count-1;

				if(tp.enumTagType == EnumTagType.GR) 
				{
					AddGroupToTree((TagGrClass)tp);
					RecurseCalcTagName((TagGrClass)tp); // 그룹속에 있는 태그들도 그룹명에 맞추어 다시 명명해야 한다. 2007.9.18
				}

				// 아래 부분은 다음에 붙여넣기 할때 찾는 속도를 증가 시킨다.
				// 그러나 ID가 바뀌어지면 다른 그룹으로 복사할 때는 같은 이름을 만들기 어렵다.
				tp = (TagPublicClass)arrayCopy[i];
				tp.name = new_name;			
			}

			userList.listHap = gr.arrayTag.Count;
			userList.listItemChanged();

			SetChangedFlag(true);
		}

		private void menuItemPaste_Click(object sender, System.EventArgs e)
		{
			PasteGo();
		}

		private void contextMenu1_Popup(object sender, System.EventArgs e)
		{
			this.menuItemCopy.Enabled = (this.userList.selectIndices.Count > 0);
			this.menuItemPaste.Enabled = (arrayCopy.Count > 0);
			this.menuItemDelete.Enabled = (this.userList.selectIndices.Count > 0);
		}

		private void menuItemSelectAll_Click(object sender, System.EventArgs e)
		{
			this.userList.selectIndices.Clear();

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());

			for(int i = 0; i < gr.arrayTag.Count; i++) 
			{
				this.userList.selectIndices.Add(i);
			}

			this.userList.Invalidate();
		}

		private void FormTagEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(bChangedFlag == true) 
			{
				DialogResult result;
				
				if(Tools.IsLangKorean())
					result = MessageBox.Show("태그파일을 저장하지 않았습니다.\n저장할까요?", "저장확인", MessageBoxButtons.YesNoCancel);
				else if(Tools.IsLangChinese())
					result = MessageBox.Show("文件还没保存。\n想保存文件吗？", "保存确认", MessageBoxButtons.YesNoCancel);
				else
					result = MessageBox.Show("Tagfile is not saved.\nSave the tag to file?", "File not saved", MessageBoxButtons.YesNoCancel);
				
				if(result == DialogResult.Cancel)  
				{
					e.Cancel = true;
					return;	
				}
				if(result == DialogResult.Yes) 
				{
					if(!Save()) 
					{
						e.Cancel = true;
						return;	
					}

                    Editor.checkTagFileChanged.Reset(); // 2017-1-12 추가. 원래 있어야 함.
				}
			}
		}

		void RemoveGroupFromTree(TagGrClass gr)
		{
			if(treeViewGroup.SelectedNode == null) 
			{
				
			}
			else 
			{
				TreeNode parent_node = treeViewGroup.SelectedNode;

				TreeNode node;
				for(int i = 0; i < parent_node.Nodes.Count; i++) 
				{
					node = parent_node.Nodes[i];
					if(String.Compare(node.Text, gr.name, true) == 0) 
					{
						parent_node.Nodes.RemoveAt(i);
						return;
					}
				}
			}
		}

		void DeleteTag()
		{
			if(this.userList.listHap == 0)				return;	// 삭제할 태그가 없다.
			if(this.userList.selectIndices.Count == 0)	return;

			if(Tools.IsLangKorean()) 
			{
				if(MessageBox.Show("선택한 태그를 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					return;
			}
			else if(Tools.IsLangChinese()) 
			{
				if(MessageBox.Show("要删除选择的标记吗？", "删除确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					return;
			}
			else 
			{
				if(MessageBox.Show("Are you sure you want to delete the selected tag?", "Delete Tag", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					return;
			}


			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());

			TagPublicClass tp;
			int pos;

			for(int i = this.userList.selectIndices.Count-1; i >= 0; i--) 
			{
				pos = (int)this.userList.selectIndices[i];

				if(pos >= gr.arrayTag.Count) 
				{
					MessageBox.Show("Delete position is invalid.", "DeleteTag()");
					break;	// over pos (이 부분이 걸리면 안된다. ListView 클래스를 다시 만들어야 할것으로 보임)
				}

				tp = (TagPublicClass)gr.arrayTag[pos];
				if(tp.enumTagType == EnumTagType.GR) 
				{
					RemoveGroupFromTree((TagGrClass)tp);
				}

				gr.arrayTag.RemoveAt(pos);
			}

			this.userList.selectIndices.Clear();
			this.userList.listHap = gr.arrayTag.Count;
			this.userList.listItemChanged();

			SetChangedFlag(true);
		}

		private void menuItemDelete_Click(object sender, System.EventArgs e)
		{
			DeleteTag();
		}

		bool Save()
		{
			bool retn = TerminalClass.SaveTag(tagTemp);

			if(retn) 
			{
				SetChangedFlag(false);
				//TagLib.groupRoot = (TagGrClass)Tools.CopyObject(tagTemp);
                TagLib.groupRoot = (TagGrClass)tagTemp.CopyObjectOnStudio();
                TagLib.ChangeTagNotFound();
                TagLib.ReMakeTagList();
			}

			return retn;
		}

		private void menuItemFileSaveTag_Click(object sender, System.EventArgs e)
		{
			Save();		
		}

		private void m_list_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyData == Keys.Enter)
				Modify();
		}

		/*
		public static bool OpenTagEditor(Form form)
		{
			FormTagEditor dialog = new FormTagEditor();
			if(dialog.ShowDialog(form) == DialogResult.OK)	return true;
			else return false;
		}
		*/

		void OK()
		{
			this.SaveEditorCoordinate();
			if(bChangedFlag == true) 
			{
				if(!Save()) 
				{
					MessageBox.Show("Can't save the Tag file");
					return;
				}

                Editor.checkTagFileChanged.Reset();
			}
			
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			OK();
		}

		private void FormTagEditor_SizeChanged_1(object sender, System.EventArgs e)
		{
			this.buttonOK.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonCancel.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonAdd.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonDelete.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonModify.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonCopy.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonPaste.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonMoveUp.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonMoveDown.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonInsert.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;
			this.buttonRunLocalMain.Left = this.ClientRectangle.Width-8-this.buttonOK.Width;

			this.panel3.Width = this.buttonOK.Left - 16;
            this.panel3.Height = this.panel1.ClientRectangle.Height - 16;   // 2016-10-12 수정  this.panel3.Height = this.ClientRectangle.Height - 16;
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.SaveEditorCoordinate();
		}

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			AddTag();
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			DeleteTag();
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void buttonCopy_Click(object sender, System.EventArgs e)
		{
			CopyGo();
		}

		private void buttonPaste_Click(object sender, System.EventArgs e)
		{
			PasteGo();
		}

		private void buttonMoveUp_Click(object sender, System.EventArgs e)
		{
			if(this.userList.selectIndices.Count == 0)	return;

			if(((int)userList.selectIndices[0]) == 0)		// 커서가 제일 상단에 있으면 아무 것도 하지 않는다.
			{
				return;
			}

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());

			int pos;
			object obj;
			for(int i = 0; i < userList.selectIndices.Count; i++) 
			{
				pos = (int)userList.selectIndices[i];
				obj = gr.arrayTag[pos];
				gr.arrayTag.Insert(pos-1, obj);
				gr.arrayTag.RemoveAt(pos+1);

				userList.selectIndices[i] = pos-1;
				userList.currPos = pos-1;
			}

			userList.listItemChanged();
			userList.Invalidate();

			SetChangedFlag(true);
		}

		private void buttonMoveDown_Click(object sender, System.EventArgs e)
		{
			if(this.userList.selectIndices.Count == 0)	return;

			int hap = this.userList.selectIndices.Count;

			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());

			if(((int)userList.selectIndices[hap-1]) >= gr.arrayTag.Count-1)		// 커서가 제일 상단에 있으면 아무 것도 하지 않는다.
			{
				return;
			}

			int pos;
			object obj;
			for(int i = userList.selectIndices.Count-1; i >=0; i--) 
			{
				pos = (int)userList.selectIndices[i];
				obj = gr.arrayTag[pos];
				gr.arrayTag.Insert(pos+2, obj);
				gr.arrayTag.RemoveAt(pos);

				userList.selectIndices[i] = pos+1;
				userList.currPos = pos+1;
			}

			userList.listItemChanged();
			userList.Invalidate();

			SetChangedFlag(true);
		}

		void Insert()
		{
			TagGrClass gr = TagLib.GetGroupArray(tagTemp, GetGroupName());
			FormTagProperty dialog = new FormTagProperty(gr.arrayTag);//this.m_list);

			if(Tools.IsLangKorean())		dialog.sMainText = "태그 삽입";
			else if(Tools.IsLangJapanese())	dialog.sMainText = "タグ挿入";
			else if(Tools.IsLangChinese())	dialog.sMainText = "插入标记";
			else							dialog.sMainText = "Insert Tag";

			dialog.bEditTagName = true;
			dialog.bModeAdd = true;
			dialog.sGroupName = GetGroupName();

			int pos;

			pos = (int)this.userList.selectIndices[0];
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				TagPublicClass tp = dialog.NewTag();
				gr.arrayTag.Insert(pos, tp);

				userList.listHap = gr.arrayTag.Count; 
				userList.listItemChanged();
				userList.selectIndices.Clear();

				userList.currPos = pos;
				userList.listItemChanged();

				if(tp.enumTagType == EnumTagType.GR)
				{
					AddGroupToTree((TagGrClass)tp);
				}

				SetChangedFlag(true);
			}
		}

		private void buttonInsert_Click(object sender, System.EventArgs e)
		{
			Insert();
		}

		private void menuItemInsertTag_Click(object sender, System.EventArgs e)
		{
			Insert();
		}

		private void FormTagEditor_Closed(object sender, System.EventArgs e)
		{
			TotalConfig.AutoBaseListCtrlConfigSave(this.userList, "TagEditor", "TagList");
		}

		public bool bRunLocalMain = false;

		private void buttonRunLocalMain_Click(object sender, System.EventArgs e)
		{
			bRunLocalMain = true;
			OK();
		}

		private void FormTagEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.F12) 
			{
				bRunLocalMain = true;
				OK();
			}

            if (e.KeyCode == Keys.F11)
            {
                FormCheckTag dialog = new FormCheckTag();

                dialog.Set(tagTemp);

                dialog.ShowDialog(this);
            }
		}

        private void importFromCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Csv files(*.csv)|*.csv";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (Tools.IsLangKorean())
                {
                    if (MessageBox.Show("CSV 파일에서 가져온 내용으로 모든 태그가 바뀝니다. 계속 진행할까요?", "가져오기 확인", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                }
                else if (Tools.IsLangChinese())
                {
                    if (MessageBox.Show("因从CSV文件导入的内容，所以所有标就要变。要继续进行吗？", "导入确认", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                }
                else
                {
                    if (MessageBox.Show("Change all tag as CSV file?", "Tag import", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                }

                TagFile tagfile = new TagFile();
                TagGrClass gr = new TagGrClass();

                if (tagfile.LoadTag(dialog.FileName, gr, System.Text.Encoding.Default, true))
                {
                    tagTemp = gr;

                    treeViewGroup.Nodes.Clear();
                    TagLib.FillGroupTree(tagTemp, treeViewGroup);
                    treeViewGroup.Nodes[0].Expand();
                    treeViewGroup.SelectedNode = treeViewGroup.Nodes[0];

                    if (Tools.IsLangKorean())
                        MessageBox.Show("가져오기를 완료 하였습니다.", dialog.FileName);
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("导入完成", dialog.FileName);
                    else
                        MessageBox.Show("Import O.K.", dialog.FileName);

                    SetChangedFlag(true);
                }
                else
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("파일을 읽을 수 없습니다.", dialog.FileName);
                    else
                        MessageBox.Show("Cannot read the file.", dialog.FileName);
                }
            }
        }

        private void exportToCSVFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Csv files(*.csv)|*.csv";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // 다이얼로그 대화 상자에서 존재하는지 묻는다.
                /*
                if (File.Exists(dialog.FileName))
                {
                    if (Tools.IsLangKorean())
                    {
                        if (MessageBox.Show("같은 이름의 파일이 존재합니다. 덮어쓸까요?", dialog.FileName, MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    }
                    else if (Tools.IsLangChinese())
                    {
                        if (MessageBox.Show("同样的文件名已存在。是否替换现有的？", dialog.FileName, MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    }
                    else
                    {
                        if (MessageBox.Show("Same file is already exists. Overwrite?", dialog.FileName, MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    }
                }*/

                TagFile tagfile = new TagFile();
                if (tagfile.SaveTag(dialog.FileName, this.tagTemp, System.Text.Encoding.Default, true, this.saveWithColumndescriptionToolStripMenuItem.Checked))
                {
                    if (Tools.IsLangKorean())
                    {
                        MessageBox.Show("내보내기를 완료 하였습니다.", "내보내기 완료");
                    }
                    else if (Tools.IsLangChinese())
                    {
                        MessageBox.Show("导出完成", "导出完成");
                    }
                    else
                    {
                        MessageBox.Show("File successfully saved to csv file.", "Export");
                    }
                }
            }
        }

        private void saveWithColumndescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveWithColumndescriptionToolStripMenuItem.Checked = !this.saveWithColumndescriptionToolStripMenuItem.Checked;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        int nSortPos = 0;
        int nSortDirection;

        int ArrayListCompare(object obj1, object obj2)
        {
            TagPublicClass tp1 = (TagPublicClass)obj1;
            TagPublicClass tp2 = (TagPublicClass)obj2;

            if (nSortPos == 1) return String.Compare(tp1.tag, tp2.tag, true);
            else if (nSortPos == 2) return String.Compare(tp1.description, tp2.description, true);
            else if (nSortPos == 3) return tp1.enumTagType - tp2.enumTagType;
            else if (nSortPos == 4) return tp1.act - tp2.act;
            else if (nSortPos == 5) return tp1.cTagLinkType - tp2.cTagLinkType;

            else if (nSortPos >= 6 && nSortPos <= 10) {
                if (tp1.cTagLinkType != tp2.cTagLinkType)
                {
                    return tp1.cTagLinkType - tp2.cTagLinkType;
                }
                else
                {
                    if (tp1.cTagLinkType == 1)  // DDE
                    {
                        if (nSortPos == 6) return String.Compare(tp1.sDdeService, tp2.sDdeService, true);
                        else if (nSortPos == 7) return String.Compare(tp1.sDdeTopic, tp2.sDdeTopic, true);
                        else if (nSortPos == 8) return String.Compare(tp1.sDdeItem, tp2.sDdeItem, true);
                        else return 0;
                    }
                    else if (tp1.cTagLinkType == 5) // OPC
                    {
                        if (nSortPos == 6) return String.Compare(tp1.sOpcServer, tp2.sOpcServer, true);
                        else if (nSortPos == 7) return String.Compare(tp1.sOpcGroup, tp2.sOpcGroup, true);
                        else if (nSortPos == 8) return String.Compare(tp1.sOpcItem, tp2.sOpcItem, true);
                        else return 0;
                    }
                    else if (tp1.cTagLinkType == 0) // PLC_SCAN
                    {
                        if (nSortPos == 6)      return tp1.port - tp2.port;
                        else if (nSortPos == 7) return tp1.station - tp2.station;
                        else { }

                        // 같은 태그 종류만 비교할 것
                        if (tp1.enumTagType != tp2.enumTagType) return tp1.enumTagType - tp2.enumTagType;

                        if (tp1.enumTagType == EnumTagType.AI)
                        {
                            TagAiClass ai1 = (TagAiClass)tp1;
                            TagAiClass ai2 = (TagAiClass)tp2;

                            if (nSortPos == 8) return ai1.address - ai2.address;
                            else return 0;
                        }
                        else if (tp1.enumTagType == EnumTagType.AO)
                        {
                            TagAoClass ao1 = (TagAoClass)tp1;
                            TagAoClass ao2 = (TagAoClass)tp2;

                            if (nSortPos == 8) return (int)ao1.address - (int)ao2.address;
                            else if (nSortPos == 9) return String.Compare(ao1.sExtraAddr, ao2.sExtraAddr, true);
                            else if (nSortPos == 10) return ao1.wExtraAddr - ao2.wExtraAddr;
                            else return 0;
                        }
                        else if (tp1.enumTagType == EnumTagType.DI)
                        {
                            TagDiClass di1 = (TagDiClass)tp1;
                            TagDiClass di2 = (TagDiClass)tp2;
                            
                            if (nSortPos == 8)
                            {
                                if (di1.address_word == di2.address_word)
                                {
                                    return di1.address_bit - di2.address_bit;
                                }

                                return (int)di1.address_word - (int)di2.address_word;
                            }
                            else return 0;
                        }
                        else if (tp1.enumTagType == EnumTagType.DO)
                        {
                            TagDoClass do1 = (TagDoClass)tp1;
                            TagDoClass do2 = (TagDoClass)tp2;
                            
                            if (nSortPos == 8) return (int)do1.address - (int)do2.address;
                            else if (nSortPos == 9) return String.Compare(do1.sExtraAddr, do2.sExtraAddr, true);
                            else if (nSortPos == 10) return do1.wExtraAddr - do2.wExtraAddr;
                            else return 0;
                        }
                        else if (tp1.enumTagType == EnumTagType.ST)
                        {
                            TagStClass st1 = (TagStClass)tp1;
                            TagStClass st2 = (TagStClass)tp2;
                            
                            if (nSortPos == 8) return (int)st1.address - (int)st2.address;
                            else return 0;
                        }
                    }
                }
            }

            else return 0;

            return 0;
        }

        void OnColumnHeaderClick(int pos)
        {
            /*
            if (pos != 1 && pos != 3 && pos != 4 && pos != 5)
            {
                return;
            }*/

            string path = GetGroupName();
            TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);

            if (nSortPos != pos)
            {
                nSortDirection = 0;
                nSortPos = pos;
            }
            else
            {
                nSortDirection = nSortDirection == 1 ? 0 : 1;
            }

            NetTools.Sort.ArrayListSort(gr.arrayTag, new Sort.DelegateArrayListCompare(ArrayListCompare) , nSortDirection == 1, true);

            this.userList.Invalidate();

            SetChangedFlag(true);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearch dialog = new FormSearch(this, false);
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearch dialog = new FormSearch(this, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            dialog.ShowDialog(this);
        }

        int MyCompareIndex(string search, string text, bool case_sens)
        {
            if (case_sens)
                return text.IndexOf(search, StringComparison.CurrentCulture);
            else
                return text.IndexOf(search, StringComparison.CurrentCultureIgnoreCase);       
        }

        bool MyCompare(string search, string text, bool case_sens)
        {
            return MyCompareIndex(search, text, case_sens) != -1;
        }

        void SearchFindNextAtOneGroup(string text, bool case_sens)
        {
            string path = GetGroupName();
            TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);
            TagPublicClass tp;

            int pos = (int)this.userList.selectIndices[0];

            for (int i = 0; i < gr.arrayTag.Count; i++)
            {
                pos++;
                pos %= gr.arrayTag.Count;
                tp = (TagPublicClass)gr.arrayTag[pos];

                if (MyCompare(text, tp.name, case_sens))
                {
                    userList.selectIndices.Clear();

                    userList.currPos = pos;
                    userList.listItemChanged();
                    break;
                }
            }
        }

        // 먼저 전체태그에서 찾을때는 
        // 현재 위치의 다음부터 트리의 끝까지 검색한다.
        bool RecurseSearchPosToEnd(TreeNode seek_node, TreeNode node, int pos, string text, bool case_sens, ref bool node_seeked)
        {
            if (!node_seeked)
            {
                if (seek_node == node)
                {
                    node_seeked = true;
                }
            }

            // 현재 리스트의 노드를 찾았을 때
            if (node_seeked)
            {
                string path = GetGroupName(node);
                TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);
                TagPublicClass tp;

                for (int i = pos; i < gr.arrayTag.Count; i++)
                {
                    tp = (TagPublicClass)gr.arrayTag[i];

                    if (MyCompare(text, tp.name, case_sens))
                    //if (String.Compare(text, 0, tp.name, 0, text.Length, !case_sens) == 0)
                    {
                        this.treeViewGroup.SelectedNode = node;

                        userList.selectIndices.Clear();
                        userList.currPos = i;
                        userList.listItemChanged();
                        return true;
                    }

                }
            }

            if (node.Nodes.Count > 0)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if (RecurseSearchPosToEnd(seek_node, node.Nodes[i], node_seeked ? 0 : pos, text, case_sens, ref node_seeked)) return true;
                }
            }

            return false;
        }

        // 그리고 트리의
        // 처음부터 현재위치까지 검색한다.
        bool RecurseSearchStartToPos(TreeNode start_node, TreeNode node, int start_pos, string text, bool case_sens)
        {
            string path = GetGroupName(node);
            TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);
            TagPublicClass tp;

            for (int i = 0; i < gr.arrayTag.Count; i++)
            {
                if (node == start_node && i == start_pos)
                {
                    MessageBox.Show("입력한 항목을 찾을 수 없습니다.");
                    return true;
                }

                tp = (TagPublicClass)gr.arrayTag[i];

                if (MyCompare(text, tp.name, case_sens))
                {
                    this.treeViewGroup.SelectedNode = node;

                    userList.selectIndices.Clear();
                    userList.currPos = i;
                    userList.listItemChanged();
                    return true;
                }

            }

            if (node.Nodes.Count > 0)
            {
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    if (RecurseSearchStartToPos(start_node, node.Nodes[i], 0, text, case_sens)) return true;
                }
            }

            return false;
        }

        void SearchFindNextAtAllGroup(string text, bool case_sens)
        {
            TreeNode start_node = this.treeViewGroup.SelectedNode;
            int start_pos;

            start_pos = (int)this.userList.selectIndices[0];
            int pos = (int)this.userList.selectIndices[0] + 1;

            TreeNode node = start_node;

            bool node_seeked = false;

            // while 루프보다는 트리를 두번 검색하는게 더 명확할 것 같다.
            // 커서위치부터 마지막 트리까지 검색하고 없을 경우 아래에서
            if (RecurseSearchPosToEnd(this.treeViewGroup.SelectedNode, this.treeViewGroup.Nodes[0], pos, text, case_sens, ref node_seeked)) 
                return;
            // 트리의 처음부터 커서 위치까지 다시 검색한다.
            if (RecurseSearchStartToPos(this.treeViewGroup.SelectedNode, this.treeViewGroup.Nodes[0], pos, text, case_sens))
                return;
        }

        public void SearchFindNext(string text, bool case_sens, bool search_at_all)
        {
            if (search_at_all)
                SearchFindNextAtAllGroup(text, case_sens);
            else
                SearchFindNextAtOneGroup(text, case_sens);
        }

        public void SearchReplace(string text, bool case_sens, bool search_at_all, string replace_text)
        {
            ReplaceAtOneGroup(text, case_sens, replace_text);
        }

        void ReplaceAtOneGroup(string text, bool case_sens, string replace_text)
        {
            string path = GetGroupName();
            TagGrClass gr = TagLib.GetGroupArray(tagTemp, path);
            TagPublicClass tp;

            int pos = (int)this.userList.selectIndices[0];

            tp = (TagPublicClass)gr.arrayTag[pos];

            // 현재의 커서 위치에 바꿀 항목이 존재하면 바꾼다.
            int index = MyCompareIndex(text, tp.name, case_sens);
            if (index != -1)
            {
                string imsi = "";
                imsi = tp.name.Substring(0, index);
                imsi += replace_text;
                imsi += tp.name.Substring(index+text.Length);

                for (int i = 0; i < gr.arrayTag.Count; i++)
                {
                    TagPublicClass tp_exist = (TagPublicClass)gr.arrayTag[i];

                    if (i == pos) continue; // 현재바꾸는 태그
                    if (String.Compare(imsi, tp_exist.name, true) == 0)
                    {
                        MessageBox.Show("바꾸려고 하는 태그명이 이미 존재합니다.", "태그명 중복");
                        return;
                    }
                }

                tp.tag = tp.tag.Substring(0, tp.tag.Length - tp.name.Length);
                tp.tag += imsi;
                tp.name = imsi;

                SetChangedFlag(true);
                this.userList.Invalidate();
            }

            // 바꾼 다음에는 다음 항목으로 이동한다.
            for (int i = 0; i < gr.arrayTag.Count; i++)
            {
                pos++;
                pos %= gr.arrayTag.Count;
                tp = (TagPublicClass)gr.arrayTag[pos];

                if (MyCompare(text, tp.name, case_sens))
                {
                    userList.selectIndices.Clear();

                    userList.currPos = pos;
                    userList.listItemChanged();
                    
                    break;
                }
            }
        }

        private void tagDescriptionAutoNumbringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tagDescriptionAutoNumbringToolStripMenuItem.Checked = !this.tagDescriptionAutoNumbringToolStripMenuItem.Checked;
        }



    }
}
