using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using GraphicModule;
using NetTools;
using DialogTag;
using AutoLibLocal;
using System.Diagnostics;
using HelpLib;
using ScriptLibEdit;
using ScriptLibRun;
using Studio.Script;
using ScriptLibEdit.Editor;
using System.Threading.Tasks;

namespace Studio
{
	/// <summary>
	/// Summary description for FormScriptEditor.
	/// </summary>
	public class FormScriptEditorSimpleNew : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label label1; 
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Button buttonCompile;
		private System.Windows.Forms.Button buttonTag;
		private System.Windows.Forms.Button buttonDataDel;
		private System.Windows.Forms.Button buttonDataAdd;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button buttonDataModify;
		private System.Windows.Forms.ListView listViewData;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private IContainer components;
		private System.Windows.Forms.NumericUpDown numericUpDownScanTime;

		string sPrepareDescription = null;
		private System.Windows.Forms.MainMenu mainMenuScript;
		private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItemConfigFont;
		bool	bScanTimeUse = false;

		static bool bSaveFlag = false;
		static int nSaveWidth = 0;
		static int nSaveHeight = 0;
		static int nSaveX = 0;
		static int nSaveY = 0;
		private System.Windows.Forms.MenuItem menuItemHelp;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemScriptCopy;
		private System.Windows.Forms.MenuItem menuItemScriptPaste;
		private System.Windows.Forms.Label labelSec;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemEditInsertRGB;
		private System.Windows.Forms.ContextMenu contextMenuScript;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.MenuItem menuItemPaste;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemUndo;
		private System.Windows.Forms.MenuItem menuItemRedo;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemSelectAll;
		static FormWindowState nSaveState;
        private GroupBox groupBoxScanTime;
        private MenuItem menuItemInsertTag;
        private Button buttonMethod;
        private CheckBox checkBoxUseThread;
        public bool bUseOkCancelButton = true;
        private CheckBox checkBoxUseNewEngine;
        private UserControlScriptEditor userControlScriptEditor1;
        private MenuItem menuItemConfig;
        public bool bEnableOptionUseThread = false; // 스레드 옵션을 보이지 않도록 한다.

        // 찾기 관련 필드 추가 250731PSU
        private FormScriptFind formFind;
        private string lastFindText = "";
        private bool lastMatchCase = false;
        private MenuItem menuItemFind;
        private MenuItem menuItem4;
        private MenuItem menuItemFindNext;
        private MenuItem menuItemFindPrevious;
        private Panel panel2;
        private MenuItem menuItem6;
        private SplitContainer splitContainer1;
        private Panel panel6;
        private Panel panel4;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelPosition;
        private int lastFindPosition = -1;

        public void SetDescription(string des)
		{
			sPrepareDescription = des;
		}

		public void EnableScanTimeUse(bool flag)
		{
			bScanTimeUse = flag;
		}

        public bool CompactMode // 확장 스크립트 편집 여부. true이면 확장 스크립트 편집, false이면 일반 스크립트 편집. 20260303 PSU 추가.
        {
            set
            {
                if (value)
                {                   
                    buttonCompile.Location = new Point(10, 3);
                    buttonTag.Location = new Point(10, 32);
                    buttonDataAdd.Location = new Point(10, 90);
                    buttonDataDel.Location = new Point(10, 120);
                    buttonDataModify.Location = new Point(10, 150);
                    buttonMethod.Location = new Point(10, 200);
                    checkBoxUseNewEngine.Location = new Point(10 , 260);
                }
            }
        }

        public FormScriptEditorSimpleNew()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            this.buttonCompile.Click += async (sender, e) => await this.buttonCompile_Click(sender, e); //250731 PSU 디자이너코드 밖으로 이동.
            SetFindMenuText();
            // UserControl 이벤트 연결
            this.userControlScriptEditor1.procCallBackOnEvent = OnEditorEvent;

            string positionText;
            if (Tools.IsLangKorean())
            {
                positionText = $"줄 0, 열 0";
            }
            else if (Tools.IsLangChinese())
            {
                positionText = $"行 0, 列 0";
            }
            else
            {
                positionText = $"Line 0, Col 0";
            }

            toolStripStatusLabelPosition.Text = positionText;
        }

        // 에디터 이벤트 처리
        private void OnEditorEvent(EnumEditorEventType type, params object[] param)
        {
            switch (type)
            {
                case EnumEditorEventType.CursorPositionChanged:
                    UpdateCursorPosition();
                    break;
            }
        }

        // 커서 위치 업데이트
        private void UpdateCursorPosition()
        {
            if (userControlScriptEditor1?.panelEditor != null)
            {
                // TextArea에서 현재 커서 위치 가져오기
                int line = userControlScriptEditor1.panelEditor.ViewCursorY + 1; // 1-based
                int column = userControlScriptEditor1.panelEditor.ViewCursorX + 1; // 1-based

                string positionText;
                if (Tools.IsLangKorean())
                {
                    positionText = $"줄 {line}, 열 {column}";
                }
                else if (Tools.IsLangChinese())
                {
                    positionText = $"行 {line}, 列 {column}";
                }
                else
                {
                    positionText = $"Line {line}, Col {column}";
                }

                toolStripStatusLabelPosition.Text = positionText;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                if (formFind != null && !formFind.IsDisposed)
                {
                    formFind.Dispose();
                }

                if (components != null)
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScriptEditorSimpleNew));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.userControlScriptEditor1 = new ScriptLibEdit.Editor.UserControlScriptEditor();
            this.listViewData = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuScript = new System.Windows.Forms.ContextMenu();
            this.menuItemUndo = new System.Windows.Forms.MenuItem();
            this.menuItemRedo = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemPaste = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItemSelectAll = new System.Windows.Forms.MenuItem();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonCompile = new System.Windows.Forms.Button();
            this.buttonTag = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonDataDel = new System.Windows.Forms.Button();
            this.buttonDataAdd = new System.Windows.Forms.Button();
            this.buttonDataModify = new System.Windows.Forms.Button();
            this.numericUpDownScanTime = new System.Windows.Forms.NumericUpDown();
            this.mainMenuScript = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemConfigFont = new System.Windows.Forms.MenuItem();
            this.menuItemConfig = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemScriptCopy = new System.Windows.Forms.MenuItem();
            this.menuItemScriptPaste = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemEditInsertRGB = new System.Windows.Forms.MenuItem();
            this.menuItemInsertTag = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemFind = new System.Windows.Forms.MenuItem();
            this.menuItemFindNext = new System.Windows.Forms.MenuItem();
            this.menuItemFindPrevious = new System.Windows.Forms.MenuItem();
            this.menuItemHelp = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.labelSec = new System.Windows.Forms.Label();
            this.groupBoxScanTime = new System.Windows.Forms.GroupBox();
            this.buttonMethod = new System.Windows.Forms.Button();
            this.checkBoxUseThread = new System.Windows.Forms.CheckBox();
            this.checkBoxUseNewEngine = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelPosition = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanTime)).BeginInit();
            this.groupBoxScanTime.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.userControlScriptEditor1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewData);
            // 
            // userControlScriptEditor1
            // 
            resources.ApplyResources(this.userControlScriptEditor1, "userControlScriptEditor1");
            this.userControlScriptEditor1.Name = "userControlScriptEditor1";
            // 
            // listViewData
            // 
            this.listViewData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            resources.ApplyResources(this.listViewData, "listViewData");
            this.listViewData.FullRowSelect = true;
            this.listViewData.HideSelection = false;
            this.listViewData.MultiSelect = false;
            this.listViewData.Name = "listViewData";
            this.listViewData.UseCompatibleStateImageBehavior = false;
            this.listViewData.View = System.Windows.Forms.View.Details;
            this.listViewData.SelectedIndexChanged += new System.EventHandler(this.listViewData_SelectedIndexChanged);
            this.listViewData.DoubleClick += new System.EventHandler(this.listViewData_DoubleClick);
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
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // contextMenuScript
            // 
            this.contextMenuScript.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemUndo,
            this.menuItemRedo,
            this.menuItem5,
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemDelete,
            this.menuItemSelectAll});
            this.contextMenuScript.Popup += new System.EventHandler(this.contextMenuScript_Popup);
            // 
            // menuItemUndo
            // 
            this.menuItemUndo.Index = 0;
            resources.ApplyResources(this.menuItemUndo, "menuItemUndo");
            this.menuItemUndo.Click += new System.EventHandler(this.menuItemUndo_Click);
            // 
            // menuItemRedo
            // 
            this.menuItemRedo.Index = 1;
            resources.ApplyResources(this.menuItemRedo, "menuItemRedo");
            this.menuItemRedo.Click += new System.EventHandler(this.menuItemRedo_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 2;
            resources.ApplyResources(this.menuItem5, "menuItem5");
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Index = 3;
            resources.ApplyResources(this.menuItemCopy, "menuItemCopy");
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemPaste
            // 
            this.menuItemPaste.Index = 4;
            resources.ApplyResources(this.menuItemPaste, "menuItemPaste");
            this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Index = 5;
            resources.ApplyResources(this.menuItemDelete, "menuItemDelete");
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // menuItemSelectAll
            // 
            this.menuItemSelectAll.Index = 6;
            resources.ApplyResources(this.menuItemSelectAll, "menuItemSelectAll");
            this.menuItemSelectAll.Click += new System.EventHandler(this.menuItemSelectAll_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonCompile
            // 
            resources.ApplyResources(this.buttonCompile, "buttonCompile");
            this.buttonCompile.Name = "buttonCompile";
            // 
            // buttonTag
            // 
            resources.ApplyResources(this.buttonTag, "buttonTag");
            this.buttonTag.Name = "buttonTag";
            this.buttonTag.Click += new System.EventHandler(this.buttonTag_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // buttonDataDel
            // 
            resources.ApplyResources(this.buttonDataDel, "buttonDataDel");
            this.buttonDataDel.Name = "buttonDataDel";
            this.buttonDataDel.Click += new System.EventHandler(this.buttonDataDel_Click);
            // 
            // buttonDataAdd
            // 
            resources.ApplyResources(this.buttonDataAdd, "buttonDataAdd");
            this.buttonDataAdd.Name = "buttonDataAdd";
            this.buttonDataAdd.Click += new System.EventHandler(this.buttonDataAdd_Click);
            // 
            // buttonDataModify
            // 
            resources.ApplyResources(this.buttonDataModify, "buttonDataModify");
            this.buttonDataModify.Name = "buttonDataModify";
            this.buttonDataModify.Click += new System.EventHandler(this.buttonDataModify_Click);
            // 
            // numericUpDownScanTime
            // 
            resources.ApplyResources(this.numericUpDownScanTime, "numericUpDownScanTime");
            this.numericUpDownScanTime.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownScanTime.Name = "numericUpDownScanTime";
            // 
            // mainMenuScript
            // 
            this.mainMenuScript.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItemHelp,
            this.menuItem6});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemConfigFont,
            this.menuItemConfig});
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // menuItemConfigFont
            // 
            this.menuItemConfigFont.Index = 0;
            resources.ApplyResources(this.menuItemConfigFont, "menuItemConfigFont");
            this.menuItemConfigFont.Click += new System.EventHandler(this.menuItemConfigFont_Click);
            // 
            // menuItemConfig
            // 
            this.menuItemConfig.Index = 1;
            resources.ApplyResources(this.menuItemConfig, "menuItemConfig");
            this.menuItemConfig.Click += new System.EventHandler(this.menuItemConfig_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemScriptCopy,
            this.menuItemScriptPaste,
            this.menuItem3,
            this.menuItemEditInsertRGB,
            this.menuItemInsertTag,
            this.menuItem4,
            this.menuItemFind,
            this.menuItemFindNext,
            this.menuItemFindPrevious});
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Popup += new System.EventHandler(this.menuItem2_Popup);
            // 
            // menuItemScriptCopy
            // 
            this.menuItemScriptCopy.Index = 0;
            resources.ApplyResources(this.menuItemScriptCopy, "menuItemScriptCopy");
            this.menuItemScriptCopy.Click += new System.EventHandler(this.menuItemScriptCopy_Click);
            // 
            // menuItemScriptPaste
            // 
            this.menuItemScriptPaste.Index = 1;
            resources.ApplyResources(this.menuItemScriptPaste, "menuItemScriptPaste");
            this.menuItemScriptPaste.Click += new System.EventHandler(this.menuItemScriptPaste_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            // 
            // menuItemEditInsertRGB
            // 
            this.menuItemEditInsertRGB.Index = 3;
            resources.ApplyResources(this.menuItemEditInsertRGB, "menuItemEditInsertRGB");
            this.menuItemEditInsertRGB.Click += new System.EventHandler(this.menuItemEditInsertRGB_Click);
            // 
            // menuItemInsertTag
            // 
            this.menuItemInsertTag.Index = 4;
            resources.ApplyResources(this.menuItemInsertTag, "menuItemInsertTag");
            this.menuItemInsertTag.Click += new System.EventHandler(this.menuItemInsertTag_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 5;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // menuItemFind
            // 
            this.menuItemFind.Index = 6;
            resources.ApplyResources(this.menuItemFind, "menuItemFind");
            this.menuItemFind.Click += new System.EventHandler(this.menuItemFind_Click);
            // 
            // menuItemFindNext
            // 
            this.menuItemFindNext.Index = 7;
            resources.ApplyResources(this.menuItemFindNext, "menuItemFindNext");
            this.menuItemFindNext.Click += new System.EventHandler(this.MenuItemFindNext_Click);
            // 
            // menuItemFindPrevious
            // 
            this.menuItemFindPrevious.Index = 8;
            resources.ApplyResources(this.menuItemFindPrevious, "menuItemFindPrevious");
            this.menuItemFindPrevious.Click += new System.EventHandler(this.MenuItemFindPrevious_Click);
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.Index = 2;
            resources.ApplyResources(this.menuItemHelp, "menuItemHelp");
            this.menuItemHelp.Click += new System.EventHandler(this.menuItemHelp_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 3;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // labelSec
            // 
            resources.ApplyResources(this.labelSec, "labelSec");
            this.labelSec.Name = "labelSec";
            // 
            // groupBoxScanTime
            // 
            this.groupBoxScanTime.Controls.Add(this.labelSec);
            this.groupBoxScanTime.Controls.Add(this.numericUpDownScanTime);
            resources.ApplyResources(this.groupBoxScanTime, "groupBoxScanTime");
            this.groupBoxScanTime.Name = "groupBoxScanTime";
            this.groupBoxScanTime.TabStop = false;
            // 
            // buttonMethod
            // 
            resources.ApplyResources(this.buttonMethod, "buttonMethod");
            this.buttonMethod.Name = "buttonMethod";
            this.buttonMethod.Click += new System.EventHandler(this.buttonMethod_Click);
            // 
            // checkBoxUseThread
            // 
            resources.ApplyResources(this.checkBoxUseThread, "checkBoxUseThread");
            this.checkBoxUseThread.Name = "checkBoxUseThread";
            this.checkBoxUseThread.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseNewEngine
            // 
            resources.ApplyResources(this.checkBoxUseNewEngine, "checkBoxUseNewEngine");
            this.checkBoxUseNewEngine.Name = "checkBoxUseNewEngine";
            this.checkBoxUseNewEngine.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxDescription);
            this.panel2.Controls.Add(this.label1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.buttonOK);
            this.panel6.Controls.Add(this.checkBoxUseNewEngine);
            this.panel6.Controls.Add(this.buttonCompile);
            this.panel6.Controls.Add(this.checkBoxUseThread);
            this.panel6.Controls.Add(this.buttonTag);
            this.panel6.Controls.Add(this.buttonMethod);
            this.panel6.Controls.Add(this.buttonCancel);
            this.panel6.Controls.Add(this.groupBoxScanTime);
            this.panel6.Controls.Add(this.buttonDataAdd);
            this.panel6.Controls.Add(this.buttonDataModify);
            this.panel6.Controls.Add(this.buttonDataDel);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.statusStrip1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelPosition});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.SizingGrip = false;
            // 
            // toolStripStatusLabelPosition
            // 
            this.toolStripStatusLabelPosition.Name = "toolStripStatusLabelPosition";
            this.toolStripStatusLabelPosition.Padding = new System.Windows.Forms.Padding(30, 0, 0, 0);
            resources.ApplyResources(this.toolStripStatusLabelPosition, "toolStripStatusLabelPosition");
            // 
            // FormScriptEditorSimpleNew
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel6);
            this.Menu = this.mainMenuScript;
            this.Name = "FormScriptEditorSimpleNew";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormScriptEditor_Load);
            this.SizeChanged += new System.EventHandler(this.FormScriptEditor_SizeChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanTime)).EndInit();
            this.groupBoxScanTime.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        bool bOldUseThread;

		public void DialogToScript(ScriptClass script)
		{
            script.SetBuf(this.userControlScriptEditor1.GetSourceString());
			script.Description = this.textBoxDescription.Text;
			script.SetScanTime(ConvertTool.ToInt32(this.numericUpDownScanTime.Value));

            script.bUseThread = this.checkBoxUseThread.Checked;

            script.bUseNewEngine = this.checkBoxUseNewEngine.Checked;

			ListDataToVar(script);
		}

        void SaveEditorCoordinate()
        {
            bSaveFlag = true;
            nSaveX = this.Left;
            nSaveY = this.Top;
            nSaveWidth = this.Width;
            nSaveHeight = this.Height;
            nSaveState = this.WindowState;
        }

        void LoadEditorCoordinate()
        {
            if (bSaveFlag)
            {
                if (nSaveState == FormWindowState.Maximized)
                {
                    this.WindowState = nSaveState;
                }
                
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Left = nSaveX;
                    this.Top = nSaveY;
                    this.Width = nSaveWidth;
                    this.Height = nSaveHeight;
                }
            }
        }

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            if (!bOldUseThread && checkBoxUseThread.Checked)
            {
                string msg;
                if (Tools.IsLangKorean())
                {
                    msg = String.Format("스크립트에서 스레드를 사용할 때는 메모리 연산만 사용하시기 바랍니다.\n화면관련 동작을 하게되면 프로그램이 정상적으로 동작하지 않습니다.");
                    MessageBox.Show(msg, "스레드 사용 주의");
                }
                else
                {
                    msg = String.Format("Use only memory operation in option 'Use Thread'.");
                    MessageBox.Show(msg, "Thread option warning");
                }
            }

			DialogToScript(scriptInclude);

			if(sFileName != null)	// 파일로 저장해 주어야 한다.
			{
				if(String.Compare(Path.GetExtension(sFileName), ".CTLX", true) != 0) 
				{
					string target;
					target = String.Format("{0}\\{1}.CTLX", Path.GetDirectoryName(sFileName), Path.GetFileNameWithoutExtension(sFileName));
					string msg;
					if(Tools.IsLangKorean()) 
					{
						msg = String.Format("스크립트 파일은 확장자가 *.ctlx 로 저장되어야 합니다.\n파일을 {0}이름으로 저장합니다.", target);
						MessageBox.Show(msg, "저장 확인");
					}
					else if(Tools.IsLangChinese()) 
					{
						msg = String.Format("脚本文件的扩展名该用*.ctlx 保存。\n将文件保存为{0}名", target);
						MessageBox.Show(msg, "保存确认");
					}
					else 
					{
						msg = String.Format("Script file must save as *.ctlx.\nThis file will be saved as {0}.", target);
						MessageBox.Show(msg, "Save as");
					}
					sFileName = target;
				}
				scriptInclude.SaveFileToMODX(sFileName);
			}

			SaveEditorCoordinate();

			DialogResult = DialogResult.OK;
			Close();
		}

		ScriptClass scriptInclude = new ScriptClass();
		string sFileName = null;

		void SetScriptPublic()
		{
            this.userControlScriptEditor1.SetSourceString(scriptInclude.Script);
			this.textBoxDescription.Text = scriptInclude.Description;
		}

		public ScriptClass GetScript()
		{
			return scriptInclude;
		}

		public void SetScript(string title, ScriptClass script)
		{
			scriptInclude = script;
			if(scriptInclude == null)	
				scriptInclude = new ScriptClass();
			this.Text = title;
			SetScriptPublic();
		}

		public void SetScript(ScriptClass script)
		{
			scriptInclude = script;
			if(scriptInclude == null)	
				scriptInclude = new ScriptClass();
			SetScriptPublic();
		}

		private void contextMenuScript_Popup(object sender, System.EventArgs e)
		{
            /*
			this.menuItemUndo.Enabled = this.richTextBoxScript.CanUndo;
			this.menuItemRedo.Enabled = this.richTextBoxScript.CanRedo;

			this.menuItemCopy.Enabled = (this.richTextBoxScript.SelectedText.Length > 0);
			this.menuItemPaste.Enabled = this.richTextBoxScript.CanPaste(DataFormats.GetFormat(DataFormats.Rtf));
			this.menuItemDelete.Enabled = (this.richTextBoxScript.SelectedText.Length > 0);*/
		}

		private void menuItemDelete_Click(object sender, System.EventArgs e)
		{
			//this.richTextBoxScript.Cut();
		}

		private void menuItemSelectAll_Click(object sender, System.EventArgs e)
		{
			//this.richTextBoxScript.SelectAll();
		}

		private void menuItemUndo_Click(object sender, System.EventArgs e)
		{
			//this.richTextBoxScript.Undo();
		}

		private void menuItemRedo_Click(object sender, System.EventArgs e)
		{
			//this.richTextBoxScript.Redo();
		}

		private void menuItemCopy_Click(object sender, System.EventArgs e)
		{
            this.userControlScriptEditor1.Copy();
		}

		private void menuItemPaste_Click(object sender, System.EventArgs e)
		{
            this.userControlScriptEditor1.Paste();
		}

		static Color tempColor = Color.Black;

        public void MenuItemInsertRGBGo()
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(tempColor, true);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                Color color = dialog.GetSelectedColor();

                tempColor = color;

                if (color.A == 255)
                {
                    Clipboard.SetDataObject(String.Format("@RGB({0},{1},{2})", color.R, color.G, color.B));
                }
                else
                {
                    Clipboard.SetDataObject(String.Format("@ARGB({0},{1},{2},{3})", color.A, color.R, color.G, color.B));
                }

                this.userControlScriptEditor1.Paste();

                this.userControlScriptEditor1.Select();
                this.userControlScriptEditor1.Focus();
            }
        }

		private void menuItemEditInsertRGB_Click(object sender, System.EventArgs e)
		{
            MenuItemInsertRGBGo();
		}

		private void listViewData_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void listViewData_DoubleClick(object sender, System.EventArgs e)
		{
			DataModify();
		}

		private void menuItem2_Popup(object sender, System.EventArgs e)
		{
			this.menuItemScriptPaste.Enabled = (scriptCopy != null);		
		}

        public void MenuItemScriptPasteGo()
        {
            if (scriptCopy == null) return;

            if (Tools.IsLangKorean())
            {
                if (MessageBox.Show("붙여넣기를 하시겠습니까?", "붙여넣기 확인", MessageBoxButtons.YesNo)
                    != DialogResult.Yes) return;
            }
            else if (Tools.IsLangChinese())
            {
                if (MessageBox.Show("要粘贴吗？", "确认粘贴", MessageBoxButtons.YesNo)
                    != DialogResult.Yes) return;
            }
            else
            {
                if (MessageBox.Show("Paste script of clipboard?", "Paste", MessageBoxButtons.YesNo)
                    != DialogResult.Yes) return;
            }

            ScriptClass script = (ScriptClass)Tools.CopyObject(scriptCopy);

            SetScript(this.Text, script);
            FillDataList();
        }

		private void menuItemScriptPaste_Click(object sender, System.EventArgs e)
		{
            MenuItemScriptPasteGo();
		}

        public void MenuItemScriptCopyGo()
        {
            scriptCopy = new ScriptClass();
            DialogToScript(scriptCopy);
        }

		private void menuItemScriptCopy_Click(object sender, System.EventArgs e)
		{
            MenuItemScriptCopyGo();
		}

        public void MenuItemHelpGo()
        {
            ClassHelp.ShowHelp(this, "Script.chm", "ScriptStart.htm", false);
        }

		private void menuItemHelp_Click(object sender, System.EventArgs e)
		{
            MenuItemHelpGo();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			SaveEditorCoordinate();
		}

		private void FormScriptEditor_SizeChanged(object sender, System.EventArgs e)
		{
			//int button_pos = this.ClientRectangle.Width-this.buttonOK.Width-10;

			//this.buttonOK.Left = button_pos;
			//this.buttonCancel.Left = button_pos;
			//this.buttonCompile.Left = button_pos;
			//this.buttonDataAdd.Left = button_pos;
			//this.buttonDataDel.Left = button_pos;
			//this.buttonDataModify.Left = button_pos;
			//this.buttonTag.Left = button_pos;
   //         this.buttonMethod.Left = button_pos;
			//this.groupBoxScanTime.Left = button_pos;

   //         this.checkBoxUseThread.Left = button_pos;
   //         this.checkBoxUseNewEngine.Left = button_pos;

			//this.textBoxDescription.Width = (button_pos-this.textBoxDescription.Left)-10;
            
   //         int height_gab = this.ClientRectangle.Height - this.userControlScriptEditor1.Top - 15;
   //         //this.userControlScriptEditor1.Size = new Size(button_pos - 25, height_gab * 2 / 3);
   //         this.userControlScriptEditor1.Width = button_pos - 25;
   //         this.userControlScriptEditor1.Height = height_gab * 2 / 3;

			//this.listViewData.Top = this.ClientRectangle.Bottom-height_gab/3-10;
   //         this.listViewData.Width = button_pos - 25;
			//this.listViewData.Height = height_gab/3;
            
            this.userControlScriptEditor1.OnSize();
		}

        public void MenuItemConfigFontGo()
        {
            FontDialog dialog = new FontDialog();
            dialog.Font = this.userControlScriptEditor1.Font;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.userControlScriptEditor1.SetFont(dialog.Font);
                TotalConfig.SaveRegAutoBaseConfig("Studio\\ScriptEditor", "Font", "Name", dialog.Font.Name);
                TotalConfig.SaveRegAutoBaseConfig("Studio\\ScriptEditor", "Font", "sSize", dialog.Font.Size.ToString(CultureTool.ciKR));    // 소숫점을 기본형식으로 저장한다.
            }
        }

		private void menuItemConfigFont_Click(object sender, System.EventArgs e)
		{
            MenuItemConfigFontGo();
		}

		static ScriptClass scriptCopy = null;


		void FillDataList()
		{
			int l;
			VAR_STRUCT var;
			string imsi;
			ScriptClass control = scriptInclude;

			listViewData.Items.Clear();

			for(l = 0; l < control.arrayVar.Count; l++) 
			{
				var = (VAR_STRUCT)control.arrayVar[l];

				imsi = control.VarTypeToString(var.type);

				ListViewItem item = new ListViewItem(imsi);
				item.SubItems.Add(var.name);
				item.SubItems.Add(var.size.ToString());
				item.SubItems.Add(ScriptClass.MakeVarValueString(var));
				listViewData.Items.Add(item);
			}	
		}

		private void FormScriptEditor_Load(object sender, System.EventArgs e)
		{
            LoadEditorCoordinate(); // 생성자에서 설정이 되지 않아서 Load로 옮김
                        
            FillDataList();
			if(sPrepareDescription != null) 
			{
				this.textBoxDescription.Text = sPrepareDescription;
			}

			this.numericUpDownScanTime.Enabled = bScanTimeUse;
            this.groupBoxScanTime.Visible = bScanTimeUse;

			Tools.SetNumericUpDownValue(this.numericUpDownScanTime, scriptInclude.GetScanTime());

            // 저장된 폰트를 가져온다.
            string font_name = TotalConfig.LoadRegAutoBaseConfig("Studio\\ScriptEditor", "Font", "Name", this.userControlScriptEditor1.Font.Name);
            string font_size = TotalConfig.LoadRegAutoBaseConfig("Studio\\ScriptEditor", "Font", "sSize", this.userControlScriptEditor1.Font.Size.ToString());

            Font font = new Font(font_name, ConvertTool.ToSingle(CultureTool.ciKR, font_size));
            this.userControlScriptEditor1.SetFont(font);

            if (!bUseOkCancelButton)
            {
                this.buttonCancel.Visible = false;
                this.buttonOK.Visible = false;
            }

            this.checkBoxUseThread.Visible = bEnableOptionUseThread;

            this.checkBoxUseThread.Checked = bOldUseThread = scriptInclude.bUseThread;

            this.checkBoxUseNewEngine.Checked = scriptInclude.bUseNewEngine;

            this.userControlScriptEditor1.OnSize(); // 여기서 호출을 하지 않으니 상하/좌우 스크롤바의 위치가 갱신이 되지 않는다.

            this.userControlScriptEditor1.Select();
            this.userControlScriptEditor1.Focus();

            // Monaco 자동완성용 태그/메서드 데이터 전달
            PopulateCompletionData();
        }

        /// <summary>
        /// Monaco 자동완성용 태그 및 메서드 데이터를 구축하여 에디터에 전달한다.
        /// </summary>
        void PopulateCompletionData()
        {
            try
            {
                // 태그 목록 구축
                var tagSb = new System.Text.StringBuilder("[");
                bool firstTag = true;
                CollectTagsRecursive(TagLib.groupRoot, tagSb, ref firstTag);
                tagSb.Append("]");
                this.userControlScriptEditor1.SetCompletionTags(tagSb.ToString());

                // 메서드 목록 구축
                var methodList = ScriptExternalRun.scriptExternal.GetMethodCompletionList();
                var methodSb = new System.Text.StringBuilder("[");
                for (int i = 0; i < methodList.Count; i++)
                {
                    if (i > 0) methodSb.Append(",");
                    string name = EscapeJsonString(methodList[i][0]);
                    string retn = EscapeJsonString(methodList[i][1]);
                    string sig = EscapeJsonString(methodList[i][2]);
                    methodSb.AppendFormat("{{\"name\":\"{0}\",\"returnType\":\"{1}\",\"signature\":\"{2}\"}}", name, retn, sig);
                }
                methodSb.Append("]");
                this.userControlScriptEditor1.SetCompletionMethods(methodSb.ToString());
            }
            catch { }
        }

        void CollectTagsRecursive(TagGrClass group, System.Text.StringBuilder sb, ref bool first)
        {
            if (group == null) return;

            for (int i = 0; i < group.arrayTag.Count; i++)
            {
                var tag = (TagPublicClass)group.arrayTag[i];

                if (tag is TagGrClass subGroup)
                {
                    CollectTagsRecursive(subGroup, sb, ref first);
                }
                else
                {
                    if (!first) sb.Append(",");
                    first = false;

                    string tagName = EscapeJsonString(tag.tag ?? "");
                    string tagType = tag.enumTagType.ToString();
                    string tagDesc = EscapeJsonString(tag.description ?? "");
                    sb.AppendFormat("{{\"name\":\"{0}\",\"type\":\"{1}\",\"description\":\"{2}\"}}", tagName, tagType, tagDesc);
                }
            }
        }

        static string EscapeJsonString(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }


        int CheckSameVarExist(string var)
		{
			ListViewItem item;			
			for(int i = 0; i < listViewData.Items.Count; i++) 
			{
				item = listViewData.Items[i];
				if(String.Compare(item.SubItems[1].Text, var, true) == 0) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("같은 이름의 변수가 이미 존재합니다.", var);
					else if(Tools.IsLangChinese())
						MessageBox.Show("同名的变量已经存在。", var);
					else
						MessageBox.Show("Same variable is already exist.", var);

					return i;
				}
			}

			return -1;
		}

		private void buttonDataAdd_Click(object sender, System.EventArgs e)
		{
			FormScriptEditorDataModify dialog = new FormScriptEditorDataModify();

			if(Tools.IsLangKorean())
				dialog.Text = "변수 추가";
			else if(Tools.IsLangJapanese())
				dialog.Text = "変数の追加";
			else if(Tools.IsLangChinese())
				dialog.Text = "添加变量";
            else if (Tools.IsLangVietnamese())
                dialog.Text = "Thêm biến";
			else
				dialog.Text = "Add Variable";

            dialog.StartPosition = FormStartPosition.CenterParent;
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				int exist_pos = CheckSameVarExist(dialog.textBoxName.Text);
				if(exist_pos != -1) 
				{
					listViewData.Items[exist_pos].Selected = true;
					listViewData.Items[exist_pos].EnsureVisible();
					return;
				}

				ListViewItem item = new ListViewItem(FormScriptEditorDataModify.sVarType);
				item.SubItems.Add(dialog.textBoxName.Text);
				item.SubItems.Add(dialog.numericUpDownSize.Value.ToString());
				item.SubItems.Add(dialog.textBoxInitValue.Text);
				listViewData.Items.Add(item);
				item.Selected = true;
				item.EnsureVisible();
			}
		}

		private void buttonDataDel_Click(object sender, System.EventArgs e)
		{
			if(this.listViewData.SelectedItems.Count == 0)	
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("삭제할 항목을 선택하세요.", "삭제오류");
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请选择要删除的项。", "删除错误");
				}
				else
					MessageBox.Show("Select item to delete", "Delete Error");

				return;
			}

			listViewData.Items.RemoveAt(listViewData.SelectedItems[0].Index);
		}

		void DataModify()
		{
			if(this.listViewData.SelectedItems.Count == 0)	
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("수정할 항목을 선택하세요.", "수정오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else
					MessageBox.Show("Select item to modify", "Select Error");

				return;
			}

			ListViewItem item = listViewData.SelectedItems[0];

			FormScriptEditorDataModify dialog = new FormScriptEditorDataModify();

			if(Tools.IsLangKorean())
				dialog.Text = "변수 수정";
			else if(Tools.IsLangJapanese())
				dialog.Text = "変数の修正";
			else if(Tools.IsLangChinese())
				dialog.Text = "修改变量";
			else
				dialog.Text = "Modify Variable";

			FormScriptEditorDataModify.sVarType = item.SubItems[0].Text;
			dialog.textBoxName.Text = item.SubItems[1].Text;
			dialog.numericUpDownSize.Value = ConvertTool.ToDecimal(item.SubItems[2].Text);
			dialog.textBoxInitValue.Text = item.SubItems[3].Text;
            dialog.StartPosition = FormStartPosition.CenterParent;
            
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				item.SubItems[0].Text = FormScriptEditorDataModify.sVarType;
				item.SubItems[1].Text = dialog.textBoxName.Text;
				item.SubItems[2].Text = dialog.numericUpDownSize.Value.ToString();
				item.SubItems[3].Text = dialog.textBoxInitValue.Text;
			}
		}

		private void buttonDataModify_Click(object sender, System.EventArgs e)
		{
			DataModify();
		}

        void InsertTag()
        {
            FormSelectTag dialog = new FormSelectTag();

            dialog.bUseTagAI = true;
            dialog.bUseTagAO = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagDO = true;
            dialog.bUseTagST = true;
            dialog.bUseTagGDO = true;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                //Clipboard.SetDataObject("$" + dialog.sTag);
                //this.userControlScriptEditor1.Paste();
                this.userControlScriptEditor1.InsertText("$" + dialog.sTag);

                this.userControlScriptEditor1.Select();
                this.userControlScriptEditor1.Focus();
            }
        }

		private void buttonTag_Click(object sender, System.EventArgs e)
		{
            InsertTag();
		}

		void ListDataToVar(ScriptClass script)
		{
			script.arrayVar.Clear();

			ListViewItem item;
			string var_type;
			string var_name;
			int var_size;
			string var_val;

			for(int i = 0; i < this.listViewData.Items.Count; i++) 
			{
				item = this.listViewData.Items[i];
				
				var_type = item.SubItems[0].Text;
				var_name = item.SubItems[1].Text;
				var_size = ConvertTool.ToInt32(item.SubItems[2].Text);
				var_val = item.SubItems[3].Text;
				script.AddOneVarFromModX(var_type, var_name, var_size, var_val); 
			}
		}

		private async Task buttonCompile_Click(object sender, System.EventArgs e)
		{
            ScriptClass script = new ScriptClass();
            script.SetBuf(this.userControlScriptEditor1.GetSourceString());
            ListDataToVar(script);

            // 새버전도 위의 ListDataToVar(script); 부분이 있어야 한다.
            if (checkBoxUseNewEngine.Checked)
            {
                if (!CompileNewScript(script)) return;
                return;
            }

			await script.RunAsync(this, null);

			if(script.IsError()) 
			{
				string message = script.GetError();
				if(Tools.IsLangKorean())
					MessageBox.Show(message, "스크립트 문장 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show(message, "脚本文章错误");
				else
					MessageBox.Show(message, "Programm Error");
				return;
			}	
			else 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("문장 오류가 없습니다.", "O.K");
				else if(Tools.IsLangChinese())
					MessageBox.Show("没有文章错误.", "O.K");
				else
					MessageBox.Show("Compile Success.", "O.K");	
			}
		}

        bool CompileNewScript(ScriptClass script)
        {
            EditScriptLibMain slm = new EditScriptLibMain();
            slm.Prepare();
            EditScriptLibFile slf = new EditScriptLibFile();
            slf.AddUsing(slm, "System", 0, 0);
            EditScriptLibLibrary sll = new EditScriptLibLibrary();
            EditScriptLibNamespace sln = new EditScriptLibNamespace(slm, sll);
            EditScriptLibClass slc = new EditScriptLibClass(sln, slf);
            EditScriptLibMethod method = new EditScriptLibMethod(slc);
            
            slm.scriptExternal = new ScriptExternalRun();

            string bin_folder = String.Format("{0}\\Bin", TotalConfig.sDirWorkProject);
            slm.LoadAllLibrary(bin_folder);

            string slc_body = script.VarsToCSSource();

            if (!slc.Split(slm, slf, slc_body, 0, 0))
            {
                Studio.Script.FormNewScriptErrorMessage dialog = new Studio.Script.FormNewScriptErrorMessage();
                dialog.Set(this, slm);
                dialog.ShowDialog(this);
                return false;
            }
            slc.Compile(slm);
            if (slm.bErrorFlag)
            {
                Studio.Script.FormNewScriptErrorMessage dialog = new Studio.Script.FormNewScriptErrorMessage();
                dialog.Set(this, slm);
                dialog.ShowDialog(this);
                return false;
            }

            if (!method.Split(slm, slf, this.userControlScriptEditor1.GetSourceString(), 0, 0))
            {
                Studio.Script.FormNewScriptErrorMessage dialog = new Studio.Script.FormNewScriptErrorMessage();
                dialog.Set(this, slm);
                dialog.ShowDialog(this);
                return false;
            }
            method.Compile(slm, slc, 0, 0);
            if(slm.bErrorFlag)
            {
                Studio.Script.FormNewScriptErrorMessage dialog = new Studio.Script.FormNewScriptErrorMessage();
                dialog.Set(this, slm);
                dialog.ShowDialog(this);
                return false;
            }

            method.sNameMethod = "Run";
            slc.arrayMethod.Add(method);

            string result_debug = slc.MakeDecompiledFile(0);

            if (NextVersion.bScript11)
            {
                FormCompileResultForDebug dialog = new FormCompileResultForDebug();

                dialog.Text = "Comiple O.K";
                dialog.Set(result_debug);
                dialog.Show(this);
                //MessageBox.Show(result_debug, "Comiple O.K"); 
            }
            else
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("문장 오류가 없습니다.", "Compile O.K");
                else
                    MessageBox.Show("Compile succeeded.", "Compile O.K");
            }

            return true;
        }

		public void SetScript(string filename)
		{
			sFileName = filename;
			if(Tools.IsLangKorean())
				this.Text = String.Format("스크립트 편집기 ({0})", filename);
			else if(Tools.IsLangJapanese())
				this.Text = String.Format("スクリプト エディター ({0})", filename);
			else if(Tools.IsLangChinese())
				this.Text = String.Format("脚本编辑器 ({0})", filename);
			else
				this.Text = String.Format("Script Editor ({0})", filename);
			
			if(File.Exists(filename)) 
			{
				scriptInclude.LoadFromFile(filename);
			}

			SetScriptPublic();
		}

        private void menuItemInsertTag_Click(object sender, EventArgs e)
        {
            InsertTag();
        }

        private void buttonMethod_Click(object sender, EventArgs e)
        {
            FormSelectMethod dialog = new FormSelectMethod();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                //Clipboard.SetDataObject("@"+dialog.sSelectedMethod);
                //this.userControlScriptEditor1.Paste();
                this.userControlScriptEditor1.InsertText("@" + dialog.sSelectedMethod);

                this.userControlScriptEditor1.Select();
                this.userControlScriptEditor1.Focus();
            }
        }

        public void GotoViewCursor(int x, int y)
        {
            userControlScriptEditor1.panelEditor.GotoViewCursor(x, y);
        }

        private void menuItemConfig_Click(object sender, EventArgs e)
        {
            FormConfigScriptNew dialog = new FormConfigScriptNew();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.userControlScriptEditor1.OnConfigurationChanged();
            }
        }

        private void SetFindMenuText()
        {
            if (Tools.IsLangKorean())
            {
                this.menuItemFind.Text = "찾기";
                this.menuItemFindNext.Text = "다음 찾기";
                this.menuItemFindPrevious.Text = "이전 찾기";
            }
            else if (Tools.IsLangChinese())
            {
                this.menuItemFind.Text = "查找";
                this.menuItemFindNext.Text = "查找下一个";
                this.menuItemFindPrevious.Text = "查找上一个";
            }
            else
            {
                this.menuItemFind.Text = "Find";
                this.menuItemFindNext.Text = "Find Next";
                this.menuItemFindPrevious.Text = "Find Previous";
            }
        }

        // 키보드 단축키 처리
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.F:
                    ShowFindDialog();
                    return true;
                case Keys.F3:
                    FindNext();
                    return true;
                case Keys.Shift | Keys.F3:
                    FindPrevious();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void menuItemFind_Click(object sender, EventArgs e)
        {
            ShowFindDialog();
        }

        private void MenuItemFindNext_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void MenuItemFindPrevious_Click(object sender, EventArgs e)
        {
            FindPrevious();
        }

        // 다음 찾기 (단축키용 - F3)
        private void FindNext()
        {
            if (string.IsNullOrEmpty(lastFindText))
            {
                ShowFindDialog();
                return;
            }

            FindNext(lastFindText, lastMatchCase);
        }

        // 이전 찾기 (단축키용 - Shift+F3)
        private void FindPrevious()
        {
            if (string.IsNullOrEmpty(lastFindText))
            {
                ShowFindDialog();
                return;
            }

            FindPrevious(lastFindText, lastMatchCase);
        }

        // 찾기 다이얼로그 표시 (모달리스 방식)
        private void ShowFindDialog()
        {
            if (formFind == null || formFind.IsDisposed)
            {
                formFind = new FormScriptFind(this);
            }

            // 선택된 텍스트가 있으면 찾기 텍스트로 설정
            string selectedText = userControlScriptEditor1.GetSelectedText();
            if (!string.IsNullOrEmpty(selectedText))
            {
                formFind.SetFindText(selectedText);
            }
            else if (!string.IsNullOrEmpty(lastFindText))
            {
                formFind.SetFindText(lastFindText);
            }

            // 모달리스로 표시
            if (!formFind.Visible)
            {
                formFind.Show(this);
            }
            else
            {
                formFind.Activate(); // 이미 열려있으면 포커스만 이동
            }
        }

        // 다음 찾기
        public void FindNext(string findText, bool matchCase)
        {
            lastFindText = findText;
            lastMatchCase = matchCase;

            int position = FindTextInEditor(findText, matchCase, true);
            if (position == -1)
            {
                ShowFindNotFoundMessage();
            }
        }

        // 이전 찾기
        public void FindPrevious(string findText, bool matchCase)
        {
            lastFindText = findText;
            lastMatchCase = matchCase;

            int position = FindTextInEditor(findText, matchCase, false);
            if (position == -1)
            {
                ShowFindNotFoundMessage();
            }
        }

        // 에디터에서 텍스트 찾기
        private int FindTextInEditor(string findText, bool matchCase, bool findNext)
        {
            string editorText = userControlScriptEditor1.GetSourceString();
            if (string.IsNullOrEmpty(editorText))
                return -1;

            StringComparison comparison = matchCase ?
                StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            int currentPosition = userControlScriptEditor1.GetCursorPosition();
            int foundPosition = -1;

            if (findNext)
            {
                // 현재 위치부터 끝까지 찾기
                foundPosition = editorText.IndexOf(findText, currentPosition, comparison);

                // 못 찾으면 처음부터 현재 위치까지 찾기
                if (foundPosition == -1 && currentPosition > 0)
                {
                    foundPosition = editorText.IndexOf(findText, 0, currentPosition, comparison);
                }
            }
            else
            {
                // 이전 찾기: 현재 선택된 텍스트가 있으면 그 길이만큼 제외
                int searchStartPos = currentPosition;

                // 현재 선택된 텍스트가 찾는 텍스트와 같으면 선택 영역을 제외하고 검색
                string selectedText = userControlScriptEditor1.GetSelectedText();
                if (!string.IsNullOrEmpty(selectedText) &&
                    string.Equals(selectedText, findText, comparison))
                {
                    // 선택된 텍스트의 시작 위치로 이동
                    searchStartPos = currentPosition - selectedText.Length;
                }

                // 처음부터 searchStartPos까지의 범위에서 마지막 발생 위치 찾기
                if (searchStartPos > 0)
                {
                    foundPosition = editorText.LastIndexOf(findText, searchStartPos - 1, comparison);
                }

                // 못 찾으면 끝에서부터 찾기 (순환 검색)
                if (foundPosition == -1 && searchStartPos < editorText.Length)
                {
                    // 전체 텍스트에서 마지막 발생 위치 찾기
                    int lastOccurrence = editorText.LastIndexOf(findText, comparison);

                    // 마지막 발생 위치가 현재 검색 시작 위치보다 뒤에 있으면 순환 검색 결과로 사용
                    if (lastOccurrence != -1 && lastOccurrence >= searchStartPos)
                    {
                        foundPosition = lastOccurrence;
                    }
                }
            }

            if (foundPosition != -1)
            {
                // 찾은 텍스트 선택 및 스크롤
                userControlScriptEditor1.SetSelection(foundPosition, findText.Length);
                userControlScriptEditor1.ScrollToCursor();
                lastFindPosition = foundPosition;
            }

            return foundPosition;
        }

        // 찾을 수 없음 메시지
        private void ShowFindNotFoundMessage()
        {
            string message;
            string title;

            if (Tools.IsLangKorean())
            {
                message = $"'{lastFindText}'을(를) 찾을 수 없습니다.";
                title = "찾기";
            }
            else if (Tools.IsLangChinese())
            {
                message = $"找不到 '{lastFindText}'。";
                title = "查找";
            }
            else
            {
                message = $"Cannot find '{lastFindText}'.";
                title = "Find";
            }

            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
