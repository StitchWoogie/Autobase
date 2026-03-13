using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using System.IO;

namespace DialogTag.TagEditor
{
	/// <summary>
	/// Summary description for FormTagProperty.
	/// </summary>
	public class FormTagProperty : System.Windows.Forms.Form
	{ 
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxDdeItem;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxDdeTopic;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxDdeService;
		private System.Windows.Forms.CheckBox checkBoxDdeRequest;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonOK;

		public string sGroupName;

		MultiSelectRadioButton multiCommonConnectionType = new MultiSelectRadioButton();
		MultiSelectCheckBox multiCommonLocalTag = new MultiSelectCheckBox();
		MultiSelectTextBox multiCommonDescription = new MultiSelectTextBox();
		MultiSelectCheckBox multiCommonAct = new MultiSelectCheckBox();


		private System.Windows.Forms.TabPage tabPageDDE;

		MultiSelectTextBox multiDdeService = new MultiSelectTextBox();
		MultiSelectTextBox multiDdeTopic = new MultiSelectTextBox();
		MultiSelectTextBox multiDdeItem = new MultiSelectTextBox();
		MultiSelectCheckBox multiDdeRequest = new MultiSelectCheckBox();

		MultiSelectTextBox multiOpcServer = new MultiSelectTextBox();
		MultiSelectTextBox multiOpcGroup = new MultiSelectTextBox();
		MultiSelectTextBox multiOpcItem = new MultiSelectTextBox();
        MultiSelectCheckBox multiOpcUAClient = new MultiSelectCheckBox();  // OPC UA 24-09-02 추가 hsjeong
		MultiSelectNumericUpDown multiOpcItemPos = new MultiSelectNumericUpDown();

        MultiSelectCheckBox multiOpcServerVisible = new MultiSelectCheckBox();
        MultiSelectCheckBox multiOpcServerReadable = new MultiSelectCheckBox();
        MultiSelectCheckBox multiOpcServerWritable = new MultiSelectCheckBox();

        MultiSelectTextBox multiScriptTagEvent = new MultiSelectTextBox();

		ArrayList listViewParent = null;
		private System.Windows.Forms.GroupBox groupBox34;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.TextBox textBoxOpcItem;
		private System.Windows.Forms.Label label56;
		private System.Windows.Forms.TextBox textBoxOpcGroup;
		private System.Windows.Forms.Label label59;
		private System.Windows.Forms.TextBox textBoxOpcServer;
		private System.Windows.Forms.Button buttonOpc;
		private System.Windows.Forms.Label label60;
		private MyNumericUpDown numericUpDownOpcArray;
		private System.Windows.Forms.Button buttonPrev;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxActive;
		private System.Windows.Forms.RadioButton radioButtonTagType5;
		private System.Windows.Forms.RadioButton radioButtonTagType6;
		private System.Windows.Forms.RadioButton radioButtonTagType4;
		private System.Windows.Forms.RadioButton radioButtonTagType3;
		private System.Windows.Forms.RadioButton radioButtonTagType2;
		private System.Windows.Forms.RadioButton radioButtonTagType1;
		private System.Windows.Forms.RadioButton radioButtonTagType0;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonConnectionType5;
		private System.Windows.Forms.RadioButton radioButtonConnectionType4;
		private System.Windows.Forms.RadioButton radioButtonConnectionType3;
		private System.Windows.Forms.RadioButton radioButtonConnectionType2;
		private System.Windows.Forms.RadioButton radioButtonConnectionType1;
		private System.Windows.Forms.RadioButton radioButtonConnectionType0;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.TextBox textBoxTag;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxLocalTag;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxFullName;
        private GroupBox groupBox4;
        private Button buttonScriptTagEvent;
        private TextBox textBoxScriptTagEvent;
        private GroupBox groupBox5;
        private CheckBox checkBoxOpcServerWritable;
        private CheckBox checkBoxOpcServerReadable;
        private CheckBox checkBoxOpcServerVisible;
        private CheckBox checkBoxOPCUAClient;
        private Label labelOPC;
        public int listViewOwnerPos = -1;

		public FormTagProperty(ArrayList listview)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			listViewParent = listview;

			multiCommonConnectionType.Add(this.radioButtonConnectionType0, radioButtonConnectionType1,
				radioButtonConnectionType2, radioButtonConnectionType3,radioButtonConnectionType4,radioButtonConnectionType5);
			multiCommonLocalTag.Add(this.checkBoxLocalTag);
			multiCommonDescription.Add(this.textBoxDescription);
			multiCommonAct.Add(this.checkBoxActive);

			// DDE
			multiDdeService.Add(this.textBoxDdeService);
			multiDdeTopic.Add(this.textBoxDdeTopic);
			multiDdeItem.Add(this.textBoxDdeItem);
			multiDdeRequest.Add(this.checkBoxDdeRequest);

			multiOpcServer.Add(this.textBoxOpcServer);
			multiOpcGroup.Add(this.textBoxOpcGroup);
			multiOpcItem.Add(this.textBoxOpcItem);
			multiOpcItemPos.Add(this.numericUpDownOpcArray);
            multiOpcUAClient.Add(this.checkBoxOPCUAClient); //OPC UA 24-09-02 추가 hsjeong

            multiOpcServerVisible.Add(this.checkBoxOpcServerVisible);
            multiOpcServerReadable.Add(this.checkBoxOpcServerReadable);
            multiOpcServerWritable.Add(this.checkBoxOpcServerWritable);

            multiScriptTagEvent.Add(this.textBoxScriptTagEvent);

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

			if(Tools.IsLangKorean())		this.sMainText = "태그 수정";
			else if(Tools.IsLangJapanese()) this.sMainText = "タグ修正";
			else if(Tools.IsLangChinese()) this.sMainText = "修改标记";
			else							this.sMainText = "Tag Properties";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTagProperty));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonScriptTagEvent = new System.Windows.Forms.Button();
            this.textBoxScriptTagEvent = new System.Windows.Forms.TextBox();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonConnectionType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnectionType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnectionType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnectionType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnectionType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonConnectionType0 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonTagType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagType6 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagType0 = new System.Windows.Forms.RadioButton();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTag = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxLocalTag = new System.Windows.Forms.CheckBox();
            this.textBoxFullName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox34 = new System.Windows.Forms.GroupBox();
            this.checkBoxOPCUAClient = new System.Windows.Forms.CheckBox();
            this.numericUpDownOpcArray = new AutoLibLocal.MyNumericUpDown();
            this.label60 = new System.Windows.Forms.Label();
            this.buttonOpc = new System.Windows.Forms.Button();
            this.label49 = new System.Windows.Forms.Label();
            this.textBoxOpcItem = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.textBoxOpcGroup = new System.Windows.Forms.TextBox();
            this.label59 = new System.Windows.Forms.Label();
            this.textBoxOpcServer = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDdeItem = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDdeTopic = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDdeService = new System.Windows.Forms.TextBox();
            this.checkBoxDdeRequest = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDDE = new System.Windows.Forms.TabPage();
            this.labelOPC = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxOpcServerWritable = new System.Windows.Forms.CheckBox();
            this.checkBoxOpcServerReadable = new System.Windows.Forms.CheckBox();
            this.checkBoxOpcServerVisible = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox34.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOpcArray)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageDDE.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.checkBoxActive);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.textBoxDescription);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.textBoxTag);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.checkBoxLocalTag);
            this.tabPage1.Controls.Add(this.textBoxFullName);
            this.tabPage1.Controls.Add(this.label3);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonScriptTagEvent);
            this.groupBox4.Controls.Add(this.textBoxScriptTagEvent);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // buttonScriptTagEvent
            // 
            resources.ApplyResources(this.buttonScriptTagEvent, "buttonScriptTagEvent");
            this.buttonScriptTagEvent.Name = "buttonScriptTagEvent";
            this.buttonScriptTagEvent.UseVisualStyleBackColor = true;
            this.buttonScriptTagEvent.Click += new System.EventHandler(this.buttonScriptTagEvent_Click);
            // 
            // textBoxScriptTagEvent
            // 
            resources.ApplyResources(this.textBoxScriptTagEvent, "textBoxScriptTagEvent");
            this.textBoxScriptTagEvent.Name = "textBoxScriptTagEvent";
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.Checked = true;
            this.checkBoxActive.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkBoxActive, "checkBoxActive");
            this.checkBoxActive.Name = "checkBoxActive";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonConnectionType5);
            this.groupBox2.Controls.Add(this.radioButtonConnectionType4);
            this.groupBox2.Controls.Add(this.radioButtonConnectionType3);
            this.groupBox2.Controls.Add(this.radioButtonConnectionType2);
            this.groupBox2.Controls.Add(this.radioButtonConnectionType1);
            this.groupBox2.Controls.Add(this.radioButtonConnectionType0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonConnectionType5
            // 
            resources.ApplyResources(this.radioButtonConnectionType5, "radioButtonConnectionType5");
            this.radioButtonConnectionType5.Name = "radioButtonConnectionType5";
            this.radioButtonConnectionType5.CheckedChanged += new System.EventHandler(this.radioButtonConnectionType5_CheckedChanged);
            // 
            // radioButtonConnectionType4
            // 
            resources.ApplyResources(this.radioButtonConnectionType4, "radioButtonConnectionType4");
            this.radioButtonConnectionType4.Name = "radioButtonConnectionType4";
            this.radioButtonConnectionType4.CheckedChanged += new System.EventHandler(this.radioButtonConnectionType4_CheckedChanged);
            // 
            // radioButtonConnectionType3
            // 
            resources.ApplyResources(this.radioButtonConnectionType3, "radioButtonConnectionType3");
            this.radioButtonConnectionType3.Name = "radioButtonConnectionType3";
            this.radioButtonConnectionType3.CheckedChanged += new System.EventHandler(this.radioButtonConnectionType3_CheckedChanged);
            // 
            // radioButtonConnectionType2
            // 
            resources.ApplyResources(this.radioButtonConnectionType2, "radioButtonConnectionType2");
            this.radioButtonConnectionType2.Name = "radioButtonConnectionType2";
            this.radioButtonConnectionType2.CheckedChanged += new System.EventHandler(this.radioButtonConnectionType2_CheckedChanged);
            // 
            // radioButtonConnectionType1
            // 
            resources.ApplyResources(this.radioButtonConnectionType1, "radioButtonConnectionType1");
            this.radioButtonConnectionType1.Name = "radioButtonConnectionType1";
            this.radioButtonConnectionType1.CheckedChanged += new System.EventHandler(this.radioButtonConnectionType1_CheckedChanged);
            // 
            // radioButtonConnectionType0
            // 
            this.radioButtonConnectionType0.Checked = true;
            resources.ApplyResources(this.radioButtonConnectionType0, "radioButtonConnectionType0");
            this.radioButtonConnectionType0.Name = "radioButtonConnectionType0";
            this.radioButtonConnectionType0.TabStop = true;
            this.radioButtonConnectionType0.CheckedChanged += new System.EventHandler(this.radioButtonConnectionType0_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonTagType5);
            this.groupBox1.Controls.Add(this.radioButtonTagType6);
            this.groupBox1.Controls.Add(this.radioButtonTagType4);
            this.groupBox1.Controls.Add(this.radioButtonTagType3);
            this.groupBox1.Controls.Add(this.radioButtonTagType2);
            this.groupBox1.Controls.Add(this.radioButtonTagType1);
            this.groupBox1.Controls.Add(this.radioButtonTagType0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonTagType5
            // 
            resources.ApplyResources(this.radioButtonTagType5, "radioButtonTagType5");
            this.radioButtonTagType5.Name = "radioButtonTagType5";
            this.radioButtonTagType5.CheckedChanged += new System.EventHandler(this.radioButtonTagType5_CheckedChanged);
            // 
            // radioButtonTagType6
            // 
            resources.ApplyResources(this.radioButtonTagType6, "radioButtonTagType6");
            this.radioButtonTagType6.Name = "radioButtonTagType6";
            this.radioButtonTagType6.CheckedChanged += new System.EventHandler(this.radioButtonTagType6_CheckedChanged);
            // 
            // radioButtonTagType4
            // 
            resources.ApplyResources(this.radioButtonTagType4, "radioButtonTagType4");
            this.radioButtonTagType4.Name = "radioButtonTagType4";
            this.radioButtonTagType4.CheckedChanged += new System.EventHandler(this.radioButtonTagType4_CheckedChanged);
            // 
            // radioButtonTagType3
            // 
            resources.ApplyResources(this.radioButtonTagType3, "radioButtonTagType3");
            this.radioButtonTagType3.Name = "radioButtonTagType3";
            this.radioButtonTagType3.CheckedChanged += new System.EventHandler(this.radioButtonTagType3_CheckedChanged);
            // 
            // radioButtonTagType2
            // 
            resources.ApplyResources(this.radioButtonTagType2, "radioButtonTagType2");
            this.radioButtonTagType2.Name = "radioButtonTagType2";
            this.radioButtonTagType2.CheckedChanged += new System.EventHandler(this.radioButtonTagType2_CheckedChanged);
            // 
            // radioButtonTagType1
            // 
            resources.ApplyResources(this.radioButtonTagType1, "radioButtonTagType1");
            this.radioButtonTagType1.Name = "radioButtonTagType1";
            this.radioButtonTagType1.CheckedChanged += new System.EventHandler(this.radioButtonTagType1_CheckedChanged);
            // 
            // radioButtonTagType0
            // 
            resources.ApplyResources(this.radioButtonTagType0, "radioButtonTagType0");
            this.radioButtonTagType0.Name = "radioButtonTagType0";
            this.radioButtonTagType0.CheckedChanged += new System.EventHandler(this.radioButtonTagType0_CheckedChanged);
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxTag
            // 
            resources.ApplyResources(this.textBoxTag, "textBoxTag");
            this.textBoxTag.Name = "textBoxTag";
            this.textBoxTag.TextChanged += new System.EventHandler(this.textBoxTag_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxLocalTag
            // 
            resources.ApplyResources(this.checkBoxLocalTag, "checkBoxLocalTag");
            this.checkBoxLocalTag.Name = "checkBoxLocalTag";
            // 
            // textBoxFullName
            // 
            resources.ApplyResources(this.textBoxFullName, "textBoxFullName");
            this.textBoxFullName.Name = "textBoxFullName";
            this.textBoxFullName.ReadOnly = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox34
            // 
            this.groupBox34.Controls.Add(this.checkBoxOPCUAClient);
            this.groupBox34.Controls.Add(this.numericUpDownOpcArray);
            this.groupBox34.Controls.Add(this.label60);
            this.groupBox34.Controls.Add(this.buttonOpc);
            this.groupBox34.Controls.Add(this.label49);
            this.groupBox34.Controls.Add(this.textBoxOpcItem);
            this.groupBox34.Controls.Add(this.label56);
            this.groupBox34.Controls.Add(this.textBoxOpcGroup);
            this.groupBox34.Controls.Add(this.label59);
            this.groupBox34.Controls.Add(this.textBoxOpcServer);
            resources.ApplyResources(this.groupBox34, "groupBox34");
            this.groupBox34.Name = "groupBox34";
            this.groupBox34.TabStop = false;
            // 
            // checkBoxOPCUAClient
            // 
            resources.ApplyResources(this.checkBoxOPCUAClient, "checkBoxOPCUAClient");
            this.checkBoxOPCUAClient.Name = "checkBoxOPCUAClient";
            // 
            // numericUpDownOpcArray
            // 
            resources.ApplyResources(this.numericUpDownOpcArray, "numericUpDownOpcArray");
            this.numericUpDownOpcArray.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownOpcArray.Name = "numericUpDownOpcArray";
            this.numericUpDownOpcArray.SampleProperty = 0;
            // 
            // label60
            // 
            resources.ApplyResources(this.label60, "label60");
            this.label60.Name = "label60";
            // 
            // buttonOpc
            // 
            resources.ApplyResources(this.buttonOpc, "buttonOpc");
            this.buttonOpc.Name = "buttonOpc";
            this.buttonOpc.Click += new System.EventHandler(this.buttonOpc_Click);
            // 
            // label49
            // 
            resources.ApplyResources(this.label49, "label49");
            this.label49.Name = "label49";
            // 
            // textBoxOpcItem
            // 
            resources.ApplyResources(this.textBoxOpcItem, "textBoxOpcItem");
            this.textBoxOpcItem.Name = "textBoxOpcItem";
            // 
            // label56
            // 
            resources.ApplyResources(this.label56, "label56");
            this.label56.Name = "label56";
            // 
            // textBoxOpcGroup
            // 
            resources.ApplyResources(this.textBoxOpcGroup, "textBoxOpcGroup");
            this.textBoxOpcGroup.Name = "textBoxOpcGroup";
            // 
            // label59
            // 
            resources.ApplyResources(this.label59, "label59");
            this.label59.Name = "label59";
            // 
            // textBoxOpcServer
            // 
            resources.ApplyResources(this.textBoxOpcServer, "textBoxOpcServer");
            this.textBoxOpcServer.Name = "textBoxOpcServer";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBoxDdeItem);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBoxDdeTopic);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBoxDdeService);
            this.groupBox3.Controls.Add(this.checkBoxDdeRequest);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textBoxDdeItem
            // 
            resources.ApplyResources(this.textBoxDdeItem, "textBoxDdeItem");
            this.textBoxDdeItem.Name = "textBoxDdeItem";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxDdeTopic
            // 
            resources.ApplyResources(this.textBoxDdeTopic, "textBoxDdeTopic");
            this.textBoxDdeTopic.Name = "textBoxDdeTopic";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxDdeService
            // 
            resources.ApplyResources(this.textBoxDdeService, "textBoxDdeService");
            this.textBoxDdeService.Name = "textBoxDdeService";
            // 
            // checkBoxDdeRequest
            // 
            resources.ApplyResources(this.checkBoxDdeRequest, "checkBoxDdeRequest");
            this.checkBoxDdeRequest.Name = "checkBoxDdeRequest";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPageDDE);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.TabIndexChanged += new System.EventHandler(this.tabControl1_TabIndexChanged);
            // 
            // tabPageDDE
            // 
            this.tabPageDDE.Controls.Add(this.labelOPC);
            this.tabPageDDE.Controls.Add(this.groupBox5);
            this.tabPageDDE.Controls.Add(this.groupBox3);
            this.tabPageDDE.Controls.Add(this.groupBox34);
            resources.ApplyResources(this.tabPageDDE, "tabPageDDE");
            this.tabPageDDE.Name = "tabPageDDE";
            // 
            // labelOPC
            // 
            resources.ApplyResources(this.labelOPC, "labelOPC");
            this.labelOPC.Name = "labelOPC";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxOpcServerWritable);
            this.groupBox5.Controls.Add(this.checkBoxOpcServerReadable);
            this.groupBox5.Controls.Add(this.checkBoxOpcServerVisible);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBoxOpcServerWritable
            // 
            resources.ApplyResources(this.checkBoxOpcServerWritable, "checkBoxOpcServerWritable");
            this.checkBoxOpcServerWritable.Name = "checkBoxOpcServerWritable";
            // 
            // checkBoxOpcServerReadable
            // 
            resources.ApplyResources(this.checkBoxOpcServerReadable, "checkBoxOpcServerReadable");
            this.checkBoxOpcServerReadable.Name = "checkBoxOpcServerReadable";
            // 
            // checkBoxOpcServerVisible
            // 
            resources.ApplyResources(this.checkBoxOpcServerVisible, "checkBoxOpcServerVisible");
            this.checkBoxOpcServerVisible.Name = "checkBoxOpcServerVisible";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonPrev
            // 
            resources.ApplyResources(this.buttonPrev, "buttonPrev");
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonNext
            // 
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // FormTagProperty
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonPrev);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTagProperty";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormTagProperty_Load);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox34.ResumeLayout(false);
            this.groupBox34.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOpcArray)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageDDE.ResumeLayout(false);
            this.tabPageDDE.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		void OK()
		{
			this.textBoxTag.Text = this.textBoxTag.Text.Trim();

			if(this.bEditTagName) 
			{	// 입력모드일 때만 태그명 검사
				if(this.textBoxTag.Text.Length == 0) 
				{
					MessageBox.Show("You must input the tag name", "Input error");
					this.tabPage1.Show();
					this.textBoxTag.Select();
					return;
				}
				char[] char_unable = { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '.' }; 
				if(this.textBoxTag.Text.IndexOfAny(char_unable) != -1) 
				{
					this.tabPage1.Show();
					this.textBoxTag.Select();
					if(Tools.IsLangKorean()) 
					{
						string msg = "아래의 문자들은 태그명으로 사용할 수 없습니다.\n\n";
						for(int i = 0; i < char_unable.Length; i++) 
						{
							msg += String.Format("{0}  ", char_unable[i]);
						}
						MessageBox.Show(msg, "태그명 오류");
					}
					else 
					{
						string msg = "A Tag name cannot contain any of the following characters.\n\n";
						for(int i = 0; i < char_unable.Length; i++) 
						{
							msg += String.Format("{0}  ", char_unable[i]);
						}
						MessageBox.Show(msg, "Invalid Tagname");
					
					}
					return;
				}

				char[] char_able = { '`', '~', '!', '@', '#', '$', '%', '^', '&', '(', 
									   ')', '-', '+', '=', '{', '}', '[', ']', ';', '\'', 
									   ',', 
				}; // , '.' 제거 250801 PSU

                if (this.textBoxTag.Text.IndexOfAny(char_able) != -1) 
				{
					this.tabPage1.Show();
					this.textBoxTag.Select();
					if(Tools.IsLangKorean()) 
					{
						string msg = "아래의 특수 문자들은 태그명으로 사용할 수는 있으나 다른 문자로 바꾸는 것이 좋습니다.\n\n";
						for(int i = 0; i < char_able.Length; i++) 
						{
							msg += String.Format("{0} ", char_able[i]);
						}
						msg += "\n\n추천 문자 (특수문자_, 한글,영문 대/소문자,기타 언어문자)";
						MessageBox.Show(msg, "태그명 확인");
					}
					else if(Tools.IsLangChinese()) 
					{
						string msg = "]标记名能包含下列特殊字符，不过最好改为别的字符。\n\n";
						for(int i = 0; i < char_able.Length; i++) 
						{
							msg += String.Format("{0} ", char_able[i]);
						}
						msg += "\n\n推荐字符(特殊字符_, 韩文，英文 大写/小写字符，其他语言字符)";
						MessageBox.Show(msg, "标记名确认");
					}
					else 
					{
						string msg = "A Tag name can contain any of the following characters.\nBut change the character to alphabet or numeric or language characters.\n\n";
						for(int i = 0; i < char_able.Length; i++) 
						{
							msg += String.Format("{0} ", char_able[i]);
						}
						MessageBox.Show(msg, "Tagname");
					}
				}

				if(TagUtil.IsTagMember(this.textBoxTag.Text))
				{
					this.tabPage1.Show();
					this.textBoxTag.Select();
					if(Tools.IsLangKorean()) 
					{
						string msg = "사용한 태그명은 태그의 멤버로 예약된 이름이므로\n다른 이름을 사용하시는 것이 좋습니다.";
						MessageBox.Show(msg, "태그명 확인");
					}
					else if(Tools.IsLangChinese()) 
					{
						string msg = "要使用的标记名已使用于标记成员,\n请使用别的名称.";
						MessageBox.Show(msg, "标记名确认");
					}
					else 
					{
						string msg = "Used tag name is reserved as Tag Member. Use another name";
						MessageBox.Show(msg, "Tagname warning");
					}
				}
			}

			if(CheckSameName(this.textBoxTag.Text)) 
			{
				this.tabPage1.Show();
				this.textBoxTag.Select();

				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("같은 태그명이 이미 존재합니다.", "태그명 중복");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("存在着同样的标记名。", "标记名冗余性");
				}
				else 
				{
					MessageBox.Show("Same tag name already exists.", "Tag Error");
				}
				return;
			}

            if (this.textBoxFullName.Text.Length > 80)
            {
                if(Tools.IsLangKorean())
                    MessageBox.Show("전체 태그 이름이 80글자를 넘었습니다.", "태그명 길이 초과");
                else
                    MessageBox.Show("Tag Name length is over 80 characters.", "TagName Length over");
                return;
            }
            if (this.textBoxDescription.Text.Length > 80)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("태그 설명이 80글자를 넘었습니다.", "태그설명 길이 초과");
                else
                    MessageBox.Show("Tag Description length is over 80 characters.", "Tag Description Length over");
                return;
            }

			this.SaveEditorCoordinate();
			
			DialogResult = DialogResult.OK;
			Close();
		}

		public int nEndMethod = 0;

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			nEndMethod = 0;
			OK();
		}

		bool CheckSameName(string text)
		{
			if(listViewParent == null)	return false;
			TagPublicClass tp;

			for(int i = 0; i < listViewParent.Count; i++) 
			{
				if(i == listViewOwnerPos)	continue;	// 현재변경하고 있는 태그
				tp = (TagPublicClass)listViewParent[i];
				if(String.Compare(tp.name, text, true) == 0)
					return true;
			}

			return false;
		}

        void EnableDisablePrevNextButton()
        {
            if (listViewOwnerPos == -1) return;
            if (listViewParent == null) return;

            if (listViewOwnerPos <= 0) 
                this.buttonPrev.Enabled = false;
            if (listViewOwnerPos >= listViewParent.Count - 1)
                this.buttonNext.Enabled = false;
        }

		static string sSelectedTagPage = "";
		bool bLoading = false;
		public bool bEditTagName = false;

		System.Windows.Forms.TabPage tabPageAI = null;
        System.Windows.Forms.TabPage tabPageAIO = null;
		System.Windows.Forms.TabPage tabPageAO = null;
		System.Windows.Forms.TabPage tabPageDI = null;
        System.Windows.Forms.TabPage tabPageDIO = null;
		System.Windows.Forms.TabPage tabPageDO = null;
		System.Windows.Forms.TabPage tabPageST = null;
		System.Windows.Forms.TabPage tabPageDoGroup = null;

		private void FormTagProperty_Load(object sender, System.EventArgs e)
		{
			bLoading = true;

			if(bFirst)
				this.radioButtonTagType0.Checked = true;

			if(!bModeAdd) 
			{
				this.radioButtonTagType0.Enabled = false;
				this.radioButtonTagType1.Enabled = false;
				this.radioButtonTagType2.Enabled = false;
				this.radioButtonTagType3.Enabled = false;
				this.radioButtonTagType4.Enabled = false;
				this.radioButtonTagType5.Enabled = false;
				this.radioButtonTagType6.Enabled = false;
			}

			this.textBoxTag.Enabled = bEditTagName;
			if(bEditTagName == false) 
			{
				this.buttonPrev.Visible = false;
				this.buttonNext.Visible = false;
			}

            if (tabPageAI != null)
            {
                this.tabControl1.Controls.Add(tabPageAI);
                this.tabControl1.Controls.Add(tabPageAIO);
            }

			if(tabPageAO != null)	this.tabControl1.Controls.Add(tabPageAO);

            if (tabPageDI != null)
            {
                this.tabControl1.Controls.Add(tabPageDI);
                this.tabControl1.Controls.Add(tabPageDIO);
            }
			if(tabPageDO != null)	this.tabControl1.Controls.Add(tabPageDO);
			if(tabPageST != null)	this.tabControl1.Controls.Add(tabPageST);
			if(tabPageDoGroup != null)	this.tabControl1.Controls.Add(tabPageDoGroup);

			if(bModeAdd) 
			{
				this.tabControl1.TabPages.Remove(this.tabPageDDE);
				this.buttonNext.Visible = false;
				this.buttonPrev.Visible = false;
			}
			
			for(int i = 0; i < this.tabControl1.TabPages.Count; i++) 
			{
				if(this.tabControl1.TabPages[i].Text == sSelectedTagPage) 
				{
					this.tabControl1.SelectedIndex = i;
					break;
				}
			}

			EnableDisableConnectionType();

            EnableDisablePrevNextButton();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE))
				this.buttonOK.Enabled = false;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                this.buttonOK.Enabled = false;
            }

			bLoading = false;
		}

		// Title에 태그명을 넣기 위해서 폼의 타이틀을 보관한다.
		public string sMainText = "";

		private void textBoxTag_TextChanged(object sender, System.EventArgs e)
		{
			this.textBoxFullName.Text = sGroupName;
			if(this.textBoxTag.Text.Length > 0) 
			{
				if(sGroupName.Length == 0)
					this.textBoxFullName.Text += this.textBoxTag.Text;
				else
					this.textBoxFullName.Text += "."+this.textBoxTag.Text;
			}

			this.Text = sMainText;
			if(this.textBoxFullName.Text.Length > 0)
				this.Text += String.Format(" ({0})", this.textBoxFullName.Text);
		}

		bool bMultiSelect = false;
		public bool bModeAdd = false;

		void TextBoxMulti(TextBox textbox)
		{
			textbox.Enabled = false;
			if(Tools.IsLangKorean()) 
				textbox.Text = "여러 태그가 선택됨";
			else if(Tools.IsLangChinese()) 
				textbox.Text = "选定着许多标记";
			else
				textbox.Text = "Multi Tag Selected";

		}

		public bool MultiSelect 
		{
			set 
			{
				bMultiSelect = value;
				if(bMultiSelect) 
				{
					TextBoxMulti(this.textBoxTag);
					//TextBoxMulti(this.textBoxDescription);
					bEditTagName = false;
				}
			}
			get 
			{
				return bMultiSelect;
			}
		}

		bool bFirst = true;

		void SetTagCommon(TagPublicClass tp) 
		{
			multiCommonConnectionType.Set(tp.cTagLinkType);
			multiCommonLocalTag.Set(tp.bLocalTag);
			multiCommonDescription.Set(tp.description);
			multiCommonAct.Set(tp.act);
			multiOpcServer.Set(tp.sOpcServer);
			multiOpcGroup.Set(tp.sOpcGroup);
			multiOpcItem.Set(tp.sOpcItem);
			multiOpcItemPos.Set(tp.nOpcItemPos);
            multiOpcUAClient.Set(tp.flagOpcUAClient); // OPC UA 24-09-02 추가 hsjeong

            multiOpcServerVisible.SetFlag(tp.flagsOpcServer, 0);
            multiOpcServerReadable.SetFlag(tp.flagsOpcServer, 1);
            multiOpcServerWritable.SetFlag(tp.flagsOpcServer, 2);

			multiDdeService.Set(tp.sDdeService);
			multiDdeTopic.Set(tp.sDdeTopic);
			multiDdeItem.Set(tp.sDdeItem);
			multiDdeRequest.Set(tp.bDdeRequest);

            multiScriptTagEvent.Set(tp.sScriptTagEvent);
		}

		void GetTagCommon(TagPublicClass tp) 
		{
			multiCommonConnectionType.Get(ref tp.cTagLinkType);
			multiCommonLocalTag.Get(ref tp.bLocalTag);
			multiCommonDescription.Get(ref tp.description);
			multiCommonAct.Get(ref tp.act);
			multiOpcServer.Get(ref tp.sOpcServer);
			multiOpcGroup.Get(ref tp.sOpcGroup);
			multiOpcItem.Get(ref tp.sOpcItem);
			multiOpcItemPos.Get(ref tp.nOpcItemPos);
            multiOpcUAClient.Get(ref tp.flagOpcUAClient); // OPC UA 24-09-02 추가 hsjeong

            multiOpcServerVisible.GetFlag(ref tp.flagsOpcServer, 0);
            multiOpcServerReadable.GetFlag(ref tp.flagsOpcServer, 1);
            multiOpcServerWritable.GetFlag(ref tp.flagsOpcServer, 2);
                        
			multiDdeService.Get(ref tp.sDdeService);
			multiDdeTopic.Get(ref tp.sDdeTopic);
			multiDdeItem.Get(ref tp.sDdeItem);
			multiDdeRequest.Get(ref tp.bDdeRequest);

            multiScriptTagEvent.Get(ref tp.sScriptTagEvent);
		}

		PropertyTagAI propAI;
        PropertyTagAIO propAIO;
		PropertyTagAO propAO;
		PropertyTagDI propDI;
        PropertyTagDIO propDIO;
		PropertyTagDO propDO;
		PropertyTagST propST;
		PropertyTagDoGroup propDoGroup;

		void CommonOnCreateProperty(Form form)
		{
			form.TopLevel = false;
			form.FormBorderStyle = FormBorderStyle.None;
			form.Visible = true;
		}

		void SetTagAI(TagAiClass ai)
		{
			if(this.tabPageAI == null) 
			{
				this.tabPageAI = new TabPage("AI");
				
				propAI = new PropertyTagAI();
				CommonOnCreateProperty(propAI);
				this.tabPageAI.Controls.Add(propAI);
			}

            if (this.tabPageAIO == null)
            {
                this.tabPageAIO = new TabPage("AIO");

                propAIO = new PropertyTagAIO();
                CommonOnCreateProperty(propAIO);
                this.tabPageAIO.Controls.Add(propAIO);
            }

			this.propAI.SetTagAI(ai);
            this.propAIO.SetTagAO(ai);

		}

		void GetTagAI(TagAiClass ai)
		{
			if(this.propAI != null)
				this.propAI.GetTagAI(ai);

            if (this.propAIO != null)
                this.propAIO.GetTagAO(ai);
		}

		void SetTagAO(TagAoClass ao)
		{
			if(this.tabPageAO == null) 
			{
				this.tabPageAO = new TabPage("AO");
				propAO = new PropertyTagAO();
				CommonOnCreateProperty(propAO);
				this.tabPageAO.Controls.Add(propAO);
			}

			this.propAO.SetTagAO(ao);

		}

		void GetTagAO(TagAoClass ao)
		{
			if(this.propAO != null)
				this.propAO.GetTagAO(ao);
		}

		void SetTagDI(TagDiClass di)
		{
			if(this.tabPageDI == null) 
			{
				this.tabPageDI = new TabPage("DI");
				propDI = new PropertyTagDI();
				CommonOnCreateProperty(propDI);
				this.tabPageDI.Controls.Add(propDI);
			}

            if (this.tabPageDIO == null)
            {
                this.tabPageDIO = new TabPage("DIO");
                propDIO = new PropertyTagDIO();
                CommonOnCreateProperty(propDIO);
                this.tabPageDIO.Controls.Add(propDIO);
            }

			this.propDI.SetTagDI(di);
            this.propDIO.SetTagDO(di);

		}

		void GetTagDI(TagDiClass di)
		{
			if(this.propDI != null)
				this.propDI.GetTagDI(di);

            if (this.propDIO != null)
                this.propDIO.GetTagDO(di);
		}

		void SetTagDO(TagDoClass dout)
		{
			if(this.tabPageDO == null) 
			{
				this.tabPageDO = new TabPage("DO");
				propDO = new PropertyTagDO();
				CommonOnCreateProperty(propDO);
				this.tabPageDO.Controls.Add(propDO);
			}

			this.propDO.SetTagDO(dout);

		}

		void GetTagDO(TagDoClass dout)
		{
			if(this.propDO != null)
				this.propDO.GetTagDO(dout);
		}

		void SetTagST(TagStClass st)
		{
			if(this.tabPageST == null) 
			{
				this.tabPageST = new TabPage("ST");
				propST = new PropertyTagST();
				CommonOnCreateProperty(propST);
				this.tabPageST.Controls.Add(propST);
			}

			this.propST.SetTagST(st);

		}

		void GetTagST(TagStClass st)
		{
			if(this.propST != null)
				this.propST.GetTagST(st);
		}

		void SetTagDoGroup(TagDoGroupClass gdo)
		{
			if(this.tabPageDoGroup == null) 
			{
				this.tabPageDoGroup = new TabPage("DoGroup");
				propDoGroup = new PropertyTagDoGroup();
				CommonOnCreateProperty(propDoGroup);
				this.tabPageDoGroup.Controls.Add(propDoGroup);
			}

			this.propDoGroup.SetTagDoGroup(gdo);

		}

		void GetTagDoGroup(TagDoGroupClass gdo)
		{
			if(this.propDoGroup != null)
				this.propDoGroup.GetTagDoGroup(gdo);
		}

		void SetTagGR(TagGrClass gr)
		{
		}

		void GetTagGR(TagGrClass gr)
		{

		}

		public void SetTag(TagPublicClass tp)
		{
			if(bFirst) 
			{
				this.radioButtonTagType0.Checked = (tp.enumTagType == EnumTagType.AI);
				this.radioButtonTagType1.Checked = (tp.enumTagType == EnumTagType.AO);
				this.radioButtonTagType2.Checked = (tp.enumTagType == EnumTagType.DI);
				this.radioButtonTagType3.Checked = (tp.enumTagType == EnumTagType.DO);
				this.radioButtonTagType4.Checked = (tp.enumTagType == EnumTagType.ST);
				this.radioButtonTagType5.Checked = (tp.enumTagType == EnumTagType.GDO);
				this.radioButtonTagType6.Checked = (tp.enumTagType == EnumTagType.GR);
			}

			if(bMultiSelect)
			{
				
			}
			else 
			{
				this.textBoxTag.Text = tp.name;
				//this.textBoxDescription.Text = tp.description;
			}

			bFirst = false;

			SetTagCommon(tp);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				SetTagAI((TagAiClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				SetTagAO((TagAoClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				SetTagDI((TagDiClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				SetTagDO((TagDoClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				SetTagST((TagStClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.GDO) 
			{
				SetTagDoGroup((TagDoGroupClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.GR) 
			{
				SetTagGR((TagGrClass)tp);
			}
		}

		public void GetTag(TagPublicClass tp)
		{
			if(bMultiSelect)
			{
				
			}
			else 
			{
				tp.tag = sGroupName;
				if(sGroupName.Length == 0)
					tp.tag += this.textBoxTag.Text;
				else
					tp.tag += "."+this.textBoxTag.Text;

				tp.name = this.textBoxTag.Text;
				//tp.description = this.textBoxDescription.Text;
			}

			GetTagCommon(tp);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				GetTagAI((TagAiClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.AO) 
			{
				GetTagAO((TagAoClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.DI) 
			{
				GetTagDI((TagDiClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.DO) 
			{
				GetTagDO((TagDoClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.ST) 
			{
				GetTagST((TagStClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.GDO) 
			{
				GetTagDoGroup((TagDoGroupClass)tp);
			}
			else if(tp.enumTagType == EnumTagType.GR) 
			{
				GetTagGR((TagGrClass)tp);
			}
		}

		public TagPublicClass NewTag()
		{
			TagPublicClass tp;
			if(this.radioButtonTagType0.Checked) 
			{
				TagAiClass ai = new TagAiClass();
				ai.unit = "";	// 태그추가 후 Unit을 사용오브젝트 추가시 다운되는 경우가 있음
				tp = (TagPublicClass)ai;
			}
			else if(this.radioButtonTagType1.Checked) 
			{
				tp = (TagPublicClass)new TagAoClass();
			}
			else if(this.radioButtonTagType2.Checked) 
			{
				tp = (TagPublicClass)new TagDiClass();
			}
			else if(this.radioButtonTagType3.Checked) 
			{
				tp = (TagPublicClass)new TagDoClass();
			}
			else if(this.radioButtonTagType4.Checked) 
			{
				tp = (TagPublicClass)new TagStClass();
			}
			else if(this.radioButtonTagType5.Checked) 
			{
				tp = (TagPublicClass)new TagDoGroupClass();
			}
			else if(this.radioButtonTagType6.Checked) 
			{
				tp = (TagPublicClass)new TagGrClass();
			}
			else 
				tp = null;

			GetTag(tp);

			return tp;
		}

		public static bool SelectAlarmWaveFile(out string filename)
		{
			filename = "";

			OpenFileDialog dialog = new OpenFileDialog();

			filename = String.Format("{0}\\sound", TotalConfig.sDirWorkProject);
			dialog.InitialDirectory = filename;
			dialog.Filter = "Sound Files (*.wav) |*.wav";
			if(Tools.IsLangKorean()) 
			{
				dialog.Title = "경보발생 시 사용할 음성파일";
			}
			else if(Tools.IsLangChinese()) 
			{
				dialog.Title = "发生警报时，要使用的声音文件";
			}
			else 
			{
				dialog.Title = "Select the alarm wave file";
			}

			if(dialog.ShowDialog() == DialogResult.OK)	
			{
				filename = Path.GetFileName(dialog.FileName);
				return true;
			}

			return false;
		}

		public static bool SelectAlarmModuleFile(out string filename)
		{
			filename = "";

			OpenFileDialog dialog = new OpenFileDialog();

			filename = String.Format("{0}\\Graphic", TotalConfig.sDirWorkProject);
			dialog.InitialDirectory = filename;
			dialog.Filter = "Unicode Module Files (*.modx) |*.modx|Ascii Module Files (*.mod) |*.mod";
			if(Tools.IsLangKorean()) 
			{
				dialog.Title = "경보 발생 시 표시할 그래픽모듈 파일";
			}
			else if(Tools.IsLangChinese()) 
			{
				dialog.Title = "发生警报时，要显示的图形模块文件";
			}
			else 
			{
				dialog.Title = "Select the module file";
			}

			if(dialog.ShowDialog() == DialogResult.OK)	
			{
                string my_dir = TotalConfig.sDirWorkProject + "\\graphic";
                if (String.Compare(my_dir, 0, dialog.FileName, 0, my_dir.Length, true) != 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("프로젝트 폴더에 있는 모듈 파일만 사용할 수 있습니다.", "폴더 오류");
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("只可使用工程项目文件夹中的模块文件。", "文件夹错误");
                    else
                        MessageBox.Show("Use module file on project directory.", "Directory Error");

                    return false;
                }

                filename = dialog.FileName.Substring(my_dir.Length + 1);

				//filename = Path.GetFileName(dialog.FileName);
				return true;
			}

			return false;
		}

		private void tabControl1_TabIndexChanged(object sender, System.EventArgs e)
		{
			
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(bLoading)	return;
			sSelectedTagPage = this.tabControl1.SelectedTab.Text;
		}


        //20241010 PSU Form owner 추가.
		public delegate bool DelegateOpcList(Form owner, out string servername, out string groupname, out string itemname);
		public static DelegateOpcList procOpcList = null;

        public static DelegateOpcList
            procOpcListUA = null; 


		private void buttonOpc_Click(object sender, System.EventArgs e)
		{
			if(procOpcList == null)	return;

			string servername;
			string groupname;
			string itemname;
            //if(!procOpcList(out servername, out groupname, out itemname))	return;
            // OPC UA 24-09-02 추가 hsjeong
            if (this.checkBoxOPCUAClient.Checked)
            {
                if (procOpcListUA == null) return;


                if (!procOpcListUA(this, out servername, out groupname, out itemname)) return;
            }
            else
            {
                if (procOpcList == null) return;


                if (!procOpcList(this, out servername, out groupname, out itemname)) return;
            }


			this.textBoxOpcServer.Text = servername;
			this.textBoxOpcGroup.Text = groupname;
			this.textBoxOpcItem.Text = itemname;
		}

		void EnableDisableConnectionType()
		{
			bool flag_opc = false;
			bool flag_dde = false;

            int type = 0;
            if (this.radioButtonConnectionType0.Checked) type = 0;
            else if (this.radioButtonConnectionType1.Checked) type = 1;
            else if (this.radioButtonConnectionType2.Checked) type = 2;
            else if (this.radioButtonConnectionType3.Checked) type = 3;
            else if (this.radioButtonConnectionType4.Checked) type = 4;
            else if (this.radioButtonConnectionType5.Checked) type = 5;
            else type = -1;

			if(type == 1) 
			{
				flag_dde = true;
			}
			else if(type == 5)
			{
				flag_opc = true;
			}

			this.textBoxOpcServer.Enabled = flag_opc;
			this.textBoxOpcGroup.Enabled = flag_opc;
			this.textBoxOpcItem.Enabled = flag_opc;
			this.buttonOpc.Enabled = flag_opc;
            this.checkBoxOPCUAClient.Enabled = flag_opc; // OPC UA 24-09-02 추가 hsjeong
            this.numericUpDownOpcArray.Enabled = flag_opc;


			this.textBoxDdeService.Enabled = flag_dde;
			this.textBoxDdeTopic.Enabled = flag_dde;
			this.textBoxDdeItem.Enabled = flag_dde;
			this.checkBoxDdeRequest.Enabled = flag_dde;

			if(this.propAI != null)			this.propAI.EnableDisableConnectionType(type);
            if (this.propAIO != null)       this.propAIO.EnableDisableConnectionType(type);
			if(this.propAO != null)			this.propAO.EnableDisableConnectionType(type);
			if(this.propDI != null)			this.propDI.EnableDisableConnectionType(type);
            if (this.propDIO != null)       this.propDIO.EnableDisableConnectionType(type);
			if(this.propDO != null)			this.propDO.EnableDisableConnectionType(type);
			if(this.propST != null)			this.propST.EnableDisableConnectionType(type);
			if(this.propDoGroup != null)	this.propDoGroup.EnableDisableConnectionType(type);
		}

		private void radioButtonConnectionType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableConnectionType();
		}

		private void radioButtonConnectionType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableConnectionType();
		}

		private void radioButtonConnectionType2_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableConnectionType();
		}

		private void radioButtonConnectionType3_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableConnectionType();
		}

		private void radioButtonConnectionType4_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableConnectionType();
		}

		private void radioButtonConnectionType5_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableConnectionType();
		}

		//public delegate void DelegatePrevNext(FormTagProperty dialog, sbyte prev_next);
		//public DelegatePrevNext procPrevNext = null;

		void Prev()
		{
			// 실행모드이거나 태그추가의 경우는 Next, Prev 버튼이 없다.
			if(bEditTagName == false || bModeAdd)	return;

			nEndMethod = 1;
			OK();
		}

		void Next()
		{
			// 실행모드이거나 태그추가의 경우는 Next, Prev 버튼이 없다.
			if(bEditTagName == false || bModeAdd)	return;

			nEndMethod = 2;
			OK();
		}

		private void buttonPrev_Click(object sender, System.EventArgs e)
		{
			Prev();
		}

		private void buttonNext_Click(object sender, System.EventArgs e)
		{
			Next();
		}

		private void radioButtonTagType0_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(0);
		}

		private void radioButtonTagType1_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(1);
		}

		private void radioButtonTagType2_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(2);
		}

		private void radioButtonTagType3_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(3);
		}

		private void radioButtonTagType4_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(4);
		}

		private void radioButtonTagType5_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(5);
		}

		private void radioButtonTagType6_CheckedChanged(object sender, System.EventArgs e)
		{
            TagTypeClicked(6);
		}

        void TagTypeClicked(int type)
        {
            FillNewName(type);

            bool flag_tag_event_script = false;

            if (type == 0) flag_tag_event_script = true;
            else if (type == 2) flag_tag_event_script = true;
            else if (type == 4) flag_tag_event_script = true;
            

            this.textBoxScriptTagEvent.Enabled = flag_tag_event_script;
            this.buttonScriptTagEvent.Enabled = flag_tag_event_script;
        }

		void FillNewName(int type)
		{
			if(this.bEditTagName == false)	return;	// 태그이름을 바꾸는 모드가 아니다.

			string tag = this.textBoxTag.Text;

			if(	tag.Length == 0 ||
				String.Compare(tag, 0, "AI_", 0, 3, true) == 0 ||
				String.Compare(tag, 0, "AO_", 0, 3, true) == 0 ||
				String.Compare(tag, 0, "DI_", 0, 3, true) == 0 ||
				String.Compare(tag, 0, "DO_", 0, 3, true) == 0 ||
				String.Compare(tag, 0, "ST_", 0, 3, true) == 0 ||
				String.Compare(tag, 0, "GDO_", 0, 4, true) == 0 ||
				String.Compare(tag, 0, "GR_", 0, 3, true) == 0 ||
				String.Compare(tag, 0, "TAG_", 0, 4, true) == 0) 
			{
				
			}
			else	// 사용자가 다른 이름을 작성하였으므로 채우지 않는다.
			{
				return;
			}

			if(type == 0) 
			{
				tag = "AI_0000";
			}
			else if(type == 1) 
			{
				tag = "AO_0000";
			}
			else if(type == 2) 
			{
				tag = "DI_0000";
			}
			else if(type == 3) 
			{
				tag = "DO_0000";
			}
			else if(type == 4) 
			{
				tag = "ST_0000";
			}
			else if(type == 5) 
			{
				tag = "GDO_0000";
			}
			else if(type == 6) 
			{
				tag = "GR_0000";
			}
			else 
			{
				tag = "TAG_0000";
			}

            int new_number;
			string name = FormTagEditor.MakeNewTagName(listViewParent, tag, out new_number);

			this.textBoxTag.Text = name;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(msg.Msg == 0x100)	// key Down, 방향키는 ProcessCmdKey에서만 들어온다
			{
				if(msg.WParam.ToInt32() == (int)(Keys.PageUp)) 
				{
					Prev();
				}
				if(msg.WParam.ToInt32() == (int)(Keys.PageDown)) 
				{
					Next();
				}
			}

			return base.ProcessCmdKey (ref msg, keyData);
		}

        void SelectScriptFile()
        {
            string script_path;

            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Script Files (*.ctlx)|*.ctlx";
            script_path = String.Format("{0}\\Control\\TagEvent", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(script_path);
            dialog.InitialDirectory = script_path;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxScriptTagEvent.Text = Path.GetFileName(dialog.FileName);
            }
        }

        public delegate bool DeleEditScript(string filename);
        public static DeleEditScript procEditScript = null;

        private void buttonScriptTagEvent_Click(object sender, EventArgs e)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                if (this.textBoxScriptTagEvent.Text.Length == 0)
                {
                    SelectScriptFile();
                }
                else
                {
                    string path = String.Format("{0}\\Control\\TagEvent\\{1}", TotalConfig.sDirWorkProject, this.textBoxScriptTagEvent.Text);

                    if (procEditScript != null)
                    {
                        if (procEditScript(path))
                        {
                            // 혹시 CTL파일일 경우는 CTLX로 이름을 바꾸어야 한다.
                            textBoxScriptTagEvent.Text = Path.GetFileNameWithoutExtension(textBoxScriptTagEvent.Text) + ".ctlx";
                        }
                    }
                }

            }
            else
            {
                SelectScriptFile();
            }
        }

	}
}
