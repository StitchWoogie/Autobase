using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using PublicStudioLocalMain.Schedule;
using NetTools.OldDefine;
using DialogTag;
using DialogHoliday;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigScheduleDesignerModelEditor.
	/// </summary>
	public class FormConfigScheduleDesignerModelEditor : System.Windows.Forms.Form
    {
        private IContainer components;

		public FormConfigScheduleDesignerModelEditor()
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScheduleDesignerModelEditor));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxModelOne = new System.Windows.Forms.GroupBox();
            this.button_ModelOne_New = new System.Windows.Forms.Button();
            this.labelModelOne = new System.Windows.Forms.Label();
            this.button_ModelOne_Delete = new System.Windows.Forms.Button();
            this.m_list_ModelOne = new System.Windows.Forms.ListView();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBoxConfigModel = new System.Windows.Forms.GroupBox();
            this.buttonConfigModelModify = new System.Windows.Forms.Button();
            this.m_list_ConfigModel = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.buttonConfigModelDelete = new System.Windows.Forms.Button();
            this.buttonConfigModelAdd = new System.Windows.Forms.Button();
            this.buttonConfigModelCopy = new System.Windows.Forms.Button();
            this.buttonConfigModelPaste = new System.Windows.Forms.Button();
            this.groupBoxModelItem = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ModelItem_radioButtonTimeType2 = new System.Windows.Forms.RadioButton();
            this.ModelItem_radioButtonTimeType1 = new System.Windows.Forms.RadioButton();
            this.ModelItem_radioButtonTimeType0 = new System.Windows.Forms.RadioButton();
            this.groupBoxSunControl = new System.Windows.Forms.GroupBox();
            this.buttonLocation_ModelItem = new System.Windows.Forms.Button();
            this.textBoxLocation_ModelItem = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownSunAfterBefore_ModelItem = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_ModelOne_Add = new System.Windows.Forms.Button();
            this.ModelItem_buttonAdd = new System.Windows.Forms.Button();
            this.ModelItem_textBoxScript = new System.Windows.Forms.TextBox();
            this.ModelItem_buttonDelete = new System.Windows.Forms.Button();
            this.m_list_ModelItem = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.ModelItem_buttonModify = new System.Windows.Forms.Button();
            this.groupBoxSpecifiedTime = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_comboMinute_ModelItem = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_comboHour_ModelItem = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBoxModelOne.SuspendLayout();
            this.groupBoxConfigModel.SuspendLayout();
            this.groupBoxModelItem.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxSunControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSunAfterBefore_ModelItem)).BeginInit();
            this.groupBoxSpecifiedTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // groupBoxModelOne
            // 
            this.groupBoxModelOne.Controls.Add(this.button_ModelOne_New);
            this.groupBoxModelOne.Controls.Add(this.labelModelOne);
            this.groupBoxModelOne.Controls.Add(this.button_ModelOne_Delete);
            this.groupBoxModelOne.Controls.Add(this.m_list_ModelOne);
            this.groupBoxModelOne.ForeColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.groupBoxModelOne, "groupBoxModelOne");
            this.groupBoxModelOne.Name = "groupBoxModelOne";
            this.groupBoxModelOne.TabStop = false;
            // 
            // button_ModelOne_New
            // 
            this.button_ModelOne_New.ForeColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.button_ModelOne_New, "button_ModelOne_New");
            this.button_ModelOne_New.Name = "button_ModelOne_New";
            this.button_ModelOne_New.Click += new System.EventHandler(this.button_ModelOne_New_Click);
            // 
            // labelModelOne
            // 
            resources.ApplyResources(this.labelModelOne, "labelModelOne");
            this.labelModelOne.Name = "labelModelOne";
            // 
            // button_ModelOne_Delete
            // 
            this.button_ModelOne_Delete.ForeColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.button_ModelOne_Delete, "button_ModelOne_Delete");
            this.button_ModelOne_Delete.Name = "button_ModelOne_Delete";
            this.button_ModelOne_Delete.Click += new System.EventHandler(this.button_ModelOne_Delete_Click);
            // 
            // m_list_ModelOne
            // 
            this.m_list_ModelOne.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7});
            this.m_list_ModelOne.FullRowSelect = true;
            this.m_list_ModelOne.HideSelection = false;
            resources.ApplyResources(this.m_list_ModelOne, "m_list_ModelOne");
            this.m_list_ModelOne.MultiSelect = false;
            this.m_list_ModelOne.Name = "m_list_ModelOne";
            this.m_list_ModelOne.SmallImageList = this.imageList1;
            this.m_list_ModelOne.UseCompatibleStateImageBehavior = false;
            this.m_list_ModelOne.View = System.Windows.Forms.View.Details;
            this.m_list_ModelOne.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.m_list_ModelOne_ItemSelectionChanged);
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "");
            this.imageList1.Images.SetKeyName(9, "");
            this.imageList1.Images.SetKeyName(10, "");
            this.imageList1.Images.SetKeyName(11, "");
            this.imageList1.Images.SetKeyName(12, "");
            this.imageList1.Images.SetKeyName(13, "");
            this.imageList1.Images.SetKeyName(14, "");
            this.imageList1.Images.SetKeyName(15, "");
            this.imageList1.Images.SetKeyName(16, "");
            this.imageList1.Images.SetKeyName(17, "");
            this.imageList1.Images.SetKeyName(18, "");
            this.imageList1.Images.SetKeyName(19, "");
            this.imageList1.Images.SetKeyName(20, "");
            this.imageList1.Images.SetKeyName(21, "");
            this.imageList1.Images.SetKeyName(22, "");
            this.imageList1.Images.SetKeyName(23, "");
            this.imageList1.Images.SetKeyName(24, "");
            this.imageList1.Images.SetKeyName(25, "");
            this.imageList1.Images.SetKeyName(26, "");
            // 
            // groupBoxConfigModel
            // 
            this.groupBoxConfigModel.Controls.Add(this.buttonConfigModelModify);
            this.groupBoxConfigModel.Controls.Add(this.m_list_ConfigModel);
            this.groupBoxConfigModel.Controls.Add(this.buttonConfigModelDelete);
            this.groupBoxConfigModel.Controls.Add(this.buttonConfigModelAdd);
            this.groupBoxConfigModel.Controls.Add(this.buttonConfigModelCopy);
            this.groupBoxConfigModel.Controls.Add(this.buttonConfigModelPaste);
            resources.ApplyResources(this.groupBoxConfigModel, "groupBoxConfigModel");
            this.groupBoxConfigModel.Name = "groupBoxConfigModel";
            this.groupBoxConfigModel.TabStop = false;
            // 
            // buttonConfigModelModify
            // 
            resources.ApplyResources(this.buttonConfigModelModify, "buttonConfigModelModify");
            this.buttonConfigModelModify.Name = "buttonConfigModelModify";
            this.buttonConfigModelModify.Click += new System.EventHandler(this.buttonConfigModelModify_Click);
            // 
            // m_list_ConfigModel
            // 
            this.m_list_ConfigModel.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.m_list_ConfigModel.FullRowSelect = true;
            this.m_list_ConfigModel.HideSelection = false;
            resources.ApplyResources(this.m_list_ConfigModel, "m_list_ConfigModel");
            this.m_list_ConfigModel.MultiSelect = false;
            this.m_list_ConfigModel.Name = "m_list_ConfigModel";
            this.m_list_ConfigModel.UseCompatibleStateImageBehavior = false;
            this.m_list_ConfigModel.View = System.Windows.Forms.View.Details;
            this.m_list_ConfigModel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_list_ConfigModel_MouseDoubleClick);
            this.m_list_ConfigModel.SelectedIndexChanged += new System.EventHandler(this.m_list_ConfigModel_SelectedIndexChanged);
            this.m_list_ConfigModel.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.m_list_ConfigModel_ItemSelectionChanged);
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // buttonConfigModelDelete
            // 
            resources.ApplyResources(this.buttonConfigModelDelete, "buttonConfigModelDelete");
            this.buttonConfigModelDelete.Name = "buttonConfigModelDelete";
            this.buttonConfigModelDelete.Click += new System.EventHandler(this.buttonConfigModelDelete_Click);
            // 
            // buttonConfigModelAdd
            // 
            resources.ApplyResources(this.buttonConfigModelAdd, "buttonConfigModelAdd");
            this.buttonConfigModelAdd.Name = "buttonConfigModelAdd";
            this.buttonConfigModelAdd.Click += new System.EventHandler(this.buttonConfigModelAdd_Click);
            // 
            // buttonConfigModelCopy
            // 
            resources.ApplyResources(this.buttonConfigModelCopy, "buttonConfigModelCopy");
            this.buttonConfigModelCopy.Name = "buttonConfigModelCopy";
            this.buttonConfigModelCopy.Click += new System.EventHandler(this.buttonConfigModelCopy_Click);
            // 
            // buttonConfigModelPaste
            // 
            resources.ApplyResources(this.buttonConfigModelPaste, "buttonConfigModelPaste");
            this.buttonConfigModelPaste.Name = "buttonConfigModelPaste";
            this.buttonConfigModelPaste.Click += new System.EventHandler(this.buttonConfigModelPaste_Click);
            // 
            // groupBoxModelItem
            // 
            this.groupBoxModelItem.Controls.Add(this.groupBox2);
            this.groupBoxModelItem.Controls.Add(this.groupBoxSunControl);
            this.groupBoxModelItem.Controls.Add(this.label3);
            this.groupBoxModelItem.Controls.Add(this.button_ModelOne_Add);
            this.groupBoxModelItem.Controls.Add(this.ModelItem_buttonAdd);
            this.groupBoxModelItem.Controls.Add(this.ModelItem_textBoxScript);
            this.groupBoxModelItem.Controls.Add(this.ModelItem_buttonDelete);
            this.groupBoxModelItem.Controls.Add(this.m_list_ModelItem);
            this.groupBoxModelItem.Controls.Add(this.ModelItem_buttonModify);
            this.groupBoxModelItem.Controls.Add(this.groupBoxSpecifiedTime);
            resources.ApplyResources(this.groupBoxModelItem, "groupBoxModelItem");
            this.groupBoxModelItem.Name = "groupBoxModelItem";
            this.groupBoxModelItem.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ModelItem_radioButtonTimeType2);
            this.groupBox2.Controls.Add(this.ModelItem_radioButtonTimeType1);
            this.groupBox2.Controls.Add(this.ModelItem_radioButtonTimeType0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // ModelItem_radioButtonTimeType2
            // 
            resources.ApplyResources(this.ModelItem_radioButtonTimeType2, "ModelItem_radioButtonTimeType2");
            this.ModelItem_radioButtonTimeType2.Name = "ModelItem_radioButtonTimeType2";
            this.ModelItem_radioButtonTimeType2.TabStop = true;
            this.ModelItem_radioButtonTimeType2.UseVisualStyleBackColor = true;
            this.ModelItem_radioButtonTimeType2.CheckedChanged += new System.EventHandler(this.ModelItem_radioButtonTimeType2_CheckedChanged);
            // 
            // ModelItem_radioButtonTimeType1
            // 
            resources.ApplyResources(this.ModelItem_radioButtonTimeType1, "ModelItem_radioButtonTimeType1");
            this.ModelItem_radioButtonTimeType1.Name = "ModelItem_radioButtonTimeType1";
            this.ModelItem_radioButtonTimeType1.TabStop = true;
            this.ModelItem_radioButtonTimeType1.UseVisualStyleBackColor = true;
            this.ModelItem_radioButtonTimeType1.CheckedChanged += new System.EventHandler(this.ModelItem_radioButtonTimeType1_CheckedChanged);
            // 
            // ModelItem_radioButtonTimeType0
            // 
            resources.ApplyResources(this.ModelItem_radioButtonTimeType0, "ModelItem_radioButtonTimeType0");
            this.ModelItem_radioButtonTimeType0.Checked = true;
            this.ModelItem_radioButtonTimeType0.Name = "ModelItem_radioButtonTimeType0";
            this.ModelItem_radioButtonTimeType0.TabStop = true;
            this.ModelItem_radioButtonTimeType0.UseVisualStyleBackColor = true;
            this.ModelItem_radioButtonTimeType0.CheckedChanged += new System.EventHandler(this.ModelItem_radioButtonTimeType0_CheckedChanged);
            // 
            // groupBoxSunControl
            // 
            this.groupBoxSunControl.Controls.Add(this.buttonLocation_ModelItem);
            this.groupBoxSunControl.Controls.Add(this.textBoxLocation_ModelItem);
            this.groupBoxSunControl.Controls.Add(this.label6);
            this.groupBoxSunControl.Controls.Add(this.label5);
            this.groupBoxSunControl.Controls.Add(this.numericUpDownSunAfterBefore_ModelItem);
            this.groupBoxSunControl.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBoxSunControl, "groupBoxSunControl");
            this.groupBoxSunControl.Name = "groupBoxSunControl";
            this.groupBoxSunControl.TabStop = false;
            // 
            // buttonLocation_ModelItem
            // 
            resources.ApplyResources(this.buttonLocation_ModelItem, "buttonLocation_ModelItem");
            this.buttonLocation_ModelItem.Name = "buttonLocation_ModelItem";
            this.buttonLocation_ModelItem.Click += new System.EventHandler(this.buttonLocation_ModelItem_Click);
            // 
            // textBoxLocation_ModelItem
            // 
            resources.ApplyResources(this.textBoxLocation_ModelItem, "textBoxLocation_ModelItem");
            this.textBoxLocation_ModelItem.Name = "textBoxLocation_ModelItem";
            this.textBoxLocation_ModelItem.ReadOnly = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownSunAfterBefore_ModelItem
            // 
            resources.ApplyResources(this.numericUpDownSunAfterBefore_ModelItem, "numericUpDownSunAfterBefore_ModelItem");
            this.numericUpDownSunAfterBefore_ModelItem.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.numericUpDownSunAfterBefore_ModelItem.Minimum = new decimal(new int[] {
            1440,
            0,
            0,
            -2147483648});
            this.numericUpDownSunAfterBefore_ModelItem.Name = "numericUpDownSunAfterBefore_ModelItem";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // button_ModelOne_Add
            // 
            resources.ApplyResources(this.button_ModelOne_Add, "button_ModelOne_Add");
            this.button_ModelOne_Add.Name = "button_ModelOne_Add";
            this.button_ModelOne_Add.Click += new System.EventHandler(this.button_ModelOne_Add_Click);
            // 
            // ModelItem_buttonAdd
            // 
            resources.ApplyResources(this.ModelItem_buttonAdd, "ModelItem_buttonAdd");
            this.ModelItem_buttonAdd.Name = "ModelItem_buttonAdd";
            this.ModelItem_buttonAdd.Click += new System.EventHandler(this.ModelItem_buttonAdd_Click);
            // 
            // ModelItem_textBoxScript
            // 
            this.ModelItem_textBoxScript.AcceptsReturn = true;
            this.ModelItem_textBoxScript.AcceptsTab = true;
            resources.ApplyResources(this.ModelItem_textBoxScript, "ModelItem_textBoxScript");
            this.ModelItem_textBoxScript.Name = "ModelItem_textBoxScript";
            // 
            // ModelItem_buttonDelete
            // 
            resources.ApplyResources(this.ModelItem_buttonDelete, "ModelItem_buttonDelete");
            this.ModelItem_buttonDelete.Name = "ModelItem_buttonDelete";
            this.ModelItem_buttonDelete.Click += new System.EventHandler(this.ModelItem_buttonDelete_Click);
            // 
            // m_list_ModelItem
            // 
            this.m_list_ModelItem.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.m_list_ModelItem.FullRowSelect = true;
            this.m_list_ModelItem.HideSelection = false;
            resources.ApplyResources(this.m_list_ModelItem, "m_list_ModelItem");
            this.m_list_ModelItem.MultiSelect = false;
            this.m_list_ModelItem.Name = "m_list_ModelItem";
            this.m_list_ModelItem.UseCompatibleStateImageBehavior = false;
            this.m_list_ModelItem.View = System.Windows.Forms.View.Details;
            this.m_list_ModelItem.DoubleClick += new System.EventHandler(this.m_list_ModelItem_DoubleClick);
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
            // ModelItem_buttonModify
            // 
            resources.ApplyResources(this.ModelItem_buttonModify, "ModelItem_buttonModify");
            this.ModelItem_buttonModify.Name = "ModelItem_buttonModify";
            this.ModelItem_buttonModify.Click += new System.EventHandler(this.ModelItem_buttonModify_Click);
            // 
            // groupBoxSpecifiedTime
            // 
            this.groupBoxSpecifiedTime.Controls.Add(this.label2);
            this.groupBoxSpecifiedTime.Controls.Add(this.m_comboMinute_ModelItem);
            this.groupBoxSpecifiedTime.Controls.Add(this.label1);
            this.groupBoxSpecifiedTime.Controls.Add(this.m_comboHour_ModelItem);
            resources.ApplyResources(this.groupBoxSpecifiedTime, "groupBoxSpecifiedTime");
            this.groupBoxSpecifiedTime.Name = "groupBoxSpecifiedTime";
            this.groupBoxSpecifiedTime.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // m_comboMinute_ModelItem
            // 
            this.m_comboMinute_ModelItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboMinute_ModelItem, "m_comboMinute_ModelItem");
            this.m_comboMinute_ModelItem.Name = "m_comboMinute_ModelItem";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // m_comboHour_ModelItem
            // 
            this.m_comboHour_ModelItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboHour_ModelItem, "m_comboHour_ModelItem");
            this.m_comboHour_ModelItem.Name = "m_comboHour_ModelItem";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // FormConfigScheduleDesignerModelEditor
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBoxModelOne);
            this.Controls.Add(this.groupBoxConfigModel);
            this.Controls.Add(this.groupBoxModelItem);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScheduleDesignerModelEditor";
            this.Load += new System.EventHandler(this.FormConfigScheduleDesignerModelEditor_Load);
            this.groupBoxModelOne.ResumeLayout(false);
            this.groupBoxConfigModel.ResumeLayout(false);
            this.groupBoxModelItem.ResumeLayout(false);
            this.groupBoxModelItem.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxSunControl.ResumeLayout(false);
            this.groupBoxSunControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSunAfterBefore_ModelItem)).EndInit();
            this.groupBoxSpecifiedTime.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion



        //ConfigModel

        public int bConfigOrSelect = 0;
        ArrayList blockTemp = new ArrayList();
        public string m_SelectTitle;
        public bool bChangeFlag;

        SCHEDULE_MODEL_STRUCT modelClipboard = null;
        private Button buttonCancel;
        private GroupBox groupBoxModelOne;
        private Button button_ModelOne_New;
        private Label labelModelOne;
        private Button button_ModelOne_Delete;
        private ListView m_list_ModelOne;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private GroupBox groupBoxConfigModel;
        private Button buttonConfigModelModify;
        private ListView m_list_ConfigModel;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private Button buttonConfigModelDelete;
        private Button buttonConfigModelAdd;
        private Button buttonConfigModelCopy;
        private Button buttonConfigModelPaste;
        private GroupBox groupBoxModelItem;
        private GroupBox groupBox2;
        private RadioButton ModelItem_radioButtonTimeType2;
        private RadioButton ModelItem_radioButtonTimeType1;
        private RadioButton ModelItem_radioButtonTimeType0;
        private GroupBox groupBoxSunControl;
        private Button buttonLocation_ModelItem;
        private TextBox textBoxLocation_ModelItem;
        private Label label6;
        private Label label5;
        private NumericUpDown numericUpDownSunAfterBefore_ModelItem;
        private Label label4;
        private Label label3;
        private Button button_ModelOne_Add;
        private Button ModelItem_buttonAdd;
        public TextBox ModelItem_textBoxScript;
        private Button ModelItem_buttonDelete;
        private ListView m_list_ModelItem;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private Button ModelItem_buttonModify;
        private GroupBox groupBoxSpecifiedTime;
        private Label label2;
        private ComboBox m_comboMinute_ModelItem;
        private Label label1;
        private ComboBox m_comboHour_ModelItem;
        private Button buttonOK;
        private ImageList imageList1;
        bool bCopyFlag;

        private void FormConfigScheduleDesignerModelEditor_Load(object sender, EventArgs e)
        {
            //ConfigModel

            bChangeFlag = false;

            bCopyFlag = false;


            this.groupBoxModelItem.Enabled = false;
            this.groupBoxModelOne.Enabled = false;

            blockTemp = ScheduleLib.ModelLoad();



            if (Tools.IsLangKorean())
                this.groupBoxConfigModel.Text = "일일 운전모델 설정";
            else if (Tools.IsLangJapanese())
                this.groupBoxConfigModel.Text = "日モデルの設定";
            else if (Tools.IsLangChinese())
                this.groupBoxConfigModel.Text = "设置一日的操作模型";
            else if (Tools.IsLangVietnamese())
                this.groupBoxConfigModel.Text = "Cấu hình kiểu ngày";
            else
                this.groupBoxConfigModel.Text = "Day model config";



            TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list_ConfigModel, "DllDialogConfigModel");

            //m_list.SetImageList(&image, LVSIL_SMALL);

            SCHEDULE_MODEL_STRUCT model;
            int l;
            ListViewItem item;

            for (l = 0; l < blockTemp.Count; l++)
            {
                model = (SCHEDULE_MODEL_STRUCT)blockTemp[l];
                item = new ListViewItem(model.title, 25);
                item.SubItems.Add(model.description);
                m_list_ConfigModel.Items.Add(item);
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCHEDULE_SETUP))
                this.buttonOK.Enabled = false;



            // 
        }

        private void buttonConfigModelAdd_Click(object sender, EventArgs e)
        {
            int pos = blockTemp.Count;

            string buf = "";
            bool bCheck = true;
            int i = 0;

            if (m_list_ModelOne.Items.Count > 0)
            {
                if (m_list_ModelOne.SelectedItems.Count > 0)
                {
                    if (pos_item != -1)
                    {
                        SaveModelItemModify();

                    }
                    else
                    {
                        SaveModelItemNew();
                    }
                }
            }
            // 날아가는거 방지를 위하여 추가 hsjeong 25-03-25

            while (bCheck)
            {
                bCheck = false;
                buf = "Model_" + String.Format("{0:D4}", i);


                for (int j = 0; j < m_list_ConfigModel.Items.Count; j++)
                {
                    string temp = m_list_ConfigModel.Items[j].SubItems[0].Text;


                    if (buf == temp)
                    {
                        i++;
                        bCheck = true;
                        break;
                    }


                }


            }

            SCHEDULE_MODEL_STRUCT model = new SCHEDULE_MODEL_STRUCT();
            model.title = buf;
            model.description = DateTime.Now.ToString();


            blockTemp.Add(model);
            ListViewItem item = m_list_ConfigModel.Items.Insert(pos, model.title, 25);
            item.SubItems.Add(model.description);
            this.m_list_ConfigModel.Items[m_list_ConfigModel.Items.Count - 1].Selected = true;
            this.m_list_ConfigModel.EnsureVisible(m_list_ConfigModel.Items.Count - 1);
            bChangeFlag = true;
        }

        private void buttonConfigModelModify_Click(object sender, EventArgs e)
        {
            ModifyConfigModel(); ;
        }

        private void buttonConfigModelDelete_Click(object sender, EventArgs e)
        {
            int retn = FormConfigModel.ListCtrlGetDeleteItem(m_list_ConfigModel);
            if (retn == -1) return;

            m_list_ConfigModel.Items.RemoveAt(retn);
            blockTemp.RemoveAt(retn);
            bChangeFlag = true;
        }



        static int ListCtrlGetSelect(ListView list)
        {
            if (list.SelectedItems.Count == 0) return -1;

            return list.SelectedItems[0].Index;
        }

        // Model One

        public SCHEDULE_MODEL_STRUCT model_one;
        ArrayList arraySchedule = null;
        int indexSchedule = -1;

        private void m_list_ConfigModel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void ChangeOneItem(ListViewItem item, SCHEDULE_MODEL_ITEM_STRUCT model_item)
        {
            string buf;

            item.ImageIndex = model_item.hour;
            if (model_item.nTimeType == 1)
            {
                item.SubItems[0].Text = String.Format("Sunrise:{0}", model_item.nSunBeforeAfterMinutes);
            }
            else if (model_item.nTimeType == 2)
            {
                item.SubItems[0].Text = String.Format("Sunset:{0}", model_item.nSunBeforeAfterMinutes);
            }
            else
            {
                item.SubItems[0].Text = String.Format("{0:00}:{1:00}", model_item.hour, model_item.minute);
            }

            ScheduleLib.MakeViewString(model_item, out buf);
            item.SubItems[1].Text = buf;
        }


        private void m_list_ConfigModel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ModifyConfigModel();
        }

        private void ModifyConfigModel()
        {
            int retn = FormConfigModel.ListCtrlGetModifyItem(m_list_ConfigModel);
            if (retn == -1) return;

            FormConfigScheduleDesignerModelModify dialog = new FormConfigScheduleDesignerModelModify();
            SCHEDULE_MODEL_STRUCT model;

            model = (SCHEDULE_MODEL_STRUCT)blockTemp[retn];
            dialog.model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(model);
            if (Tools.IsLangKorean())
                dialog.Text = "운전 모델 수정";
            else if (Tools.IsLangJapanese())
                dialog.Text = "モデル修正";
            else
                dialog.Text = "Modify Model";

            dialog.Set(blockTemp, retn);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(dialog.model);
                blockTemp[retn] = model;
                ListViewItem item = m_list_ConfigModel.Items[retn];
                item.SubItems[0].Text = model.title;
                item.SubItems[1].Text = model.description;
                bChangeFlag = true;
                m_list_ConfigModel.Items[retn].Selected = false;
                m_list_ConfigModel.Items[retn].Selected = true;
                //CheckSameName();
            }
        }



        private void m_list_ConfigModel_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (m_list_ConfigModel.SelectedItems.Count > 0)
            {
                this.m_list_ModelOne.Items.Clear();

                this.groupBoxModelOne.Enabled = true;


                int i = 0;
                string buf = "";
                for (i = 0; i < 24; i++)
                {
                    buf = String.Format("{0:00}", i);
                    this.m_comboHour_ModelItem.Items.Add(buf);
                }

                for (i = 0; i < 60; i++)
                {
                    buf = String.Format("{0:00}", i);
                    this.m_comboMinute_ModelItem.Items.Add(buf);
                }

                int retn = FormConfigModel.ListCtrlGetModifyItem(m_list_ConfigModel);
                if (retn == -1) return;

                SCHEDULE_MODEL_STRUCT model_temp;
                model_one = new SCHEDULE_MODEL_STRUCT();

                model_temp = (SCHEDULE_MODEL_STRUCT)blockTemp[retn];
                model_one = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(model_temp);

                arraySchedule = blockTemp;
                indexSchedule = retn;

                TotalConfig.AutoBaseMainListCtrlConfigLoad(m_list_ModelOne, "DllDialogConfigModelOne");

                SCHEDULE_MODEL_ITEM_STRUCT item;
                int l;

                this.groupBoxModelOne.Text = model_one.title;
                this.labelModelOne.Text = model_one.description;

                for (l = 0; l < model_one.blockItem.Count; l++)
                {
                    item = (SCHEDULE_MODEL_ITEM_STRUCT)model_one.blockItem[l];
                    ListViewItem lvi = new ListViewItem("");
                    lvi.SubItems.Add("");
                    ChangeOneItem(lvi, item);
                    this.m_list_ModelOne.Items.Add(lvi);
                }


            }
            else
            {

                if (Tools.IsLangKorean())
                {
                    this.groupBoxModelOne.Text = "운전모델을 선택하세요 !";
                    this.labelModelOne.Text = "선택된 운전모델이 없습니다 !";
                }
                else
                {
                    this.groupBoxModelOne.Text = "Select Model from the list !";
                    this.labelModelOne.Text = "No Model is selected !";
                }
                this.m_list_ModelOne.Items.Clear();

                this.groupBoxModelOne.Enabled = false;


                this.m_comboHour_ModelItem.Items.Clear();
                this.m_comboMinute_ModelItem.Items.Clear();
                this.m_list_ModelItem.Items.Clear();
                this.ModelItem_textBoxScript.Text = "";

                if (Tools.IsLangKorean())
                {
                    this.groupBoxModelItem.Text = "모델 아이템";
                }
                else
                {
                    this.groupBoxModelItem.Text = "Model Item";
                }

                pos_item = -1;
                this.groupBoxModelItem.Enabled = false;


            }






        }



        // ModelItem

        static bool bTagMultiSelection = true;

        public bool bChangeItemFlag = false;

        public ArrayList blockTemp_Item = new ArrayList();
        public int m_nHour;
        public int m_nMinute;
        public int pos_item = -1;

        private void ConfigModelItem_Load()
        {

            string buf;
            int i;

            this.m_comboHour_ModelItem.Items.Clear();
            this.m_comboMinute_ModelItem.Items.Clear();
            this.m_list_ModelItem.Items.Clear();


            for (i = 0; i < 24; i++)
            {
                buf = String.Format("{0:00}", i);
                this.m_comboHour_ModelItem.Items.Add(buf);
            }

            for (i = 0; i < 60; i++)
            {
                buf = String.Format("{0:00}", i);
                this.m_comboMinute_ModelItem.Items.Add(buf);
            }

            TotalConfig.AutoBaseMainListCtrlConfigLoad(this.m_list_ModelItem, "DllDialogConfigModelItem");



            SCHEDULE_TAG_VALUE_STRUCT tagValue;
            int l;
            if (pos_item != -1)
            {

                for (l = 0; l < blockTemp_Item.Count; l++)
                {
                    tagValue = (SCHEDULE_TAG_VALUE_STRUCT)blockTemp_Item[l];
                    ListViewItem item = new ListViewItem("");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    this.m_list_ModelItem.Items.Add(item);
                    ChangeOneModelItem(item, tagValue.tag, tagValue.val);
                }

                this.m_comboHour_ModelItem.SelectedIndex = this.m_nHour;
                this.m_comboMinute_ModelItem.SelectedIndex = this.m_nMinute;

                EnableTimeType();

                RecalcSunContolTime();

                if (TotalConfig.eOemType == EnumOemType.SBAS)
                {
                    groupBox2.Visible = false;
                    groupBoxSunControl.Visible = false;
                    groupBoxSpecifiedTime.Left = groupBox2.Left;

                    this.m_list_ModelItem.Top -= 88;
                    this.m_list_ModelItem.Top -= 88;
                    this.m_list_ModelItem.Top -= 88;
                    this.m_list_ModelItem.Top -= 88;

                    this.label3.Visible = false;
                    this.ModelItem_textBoxScript.Visible = false;
                    this.Height -= (144 + 88);

                    this.ModelItem_radioButtonTimeType0.Checked = true;
                }

            }
            else
            {
                this.ModelItem_textBoxScript.Text = "";


            }
        }

        void ChangeOneModelItem(ListViewItem item, string tag, string val)
        {
            TagPublicClass tp;
            int[] tag_pos = new int[1];

            tp = TagLib.GetStructPublic(tag, ref tag_pos);

            item.SubItems[0].Text = tag;
            item.SubItems[1].Text = tp.description;
            item.SubItems[2].Text = val;
        }

        void EnableTimeType()
        {
            int type = GetRadioTimeType();
            bool flag_sun_control = false;
            bool flag_specifid_control = false;

            if (type == 1 || type == 2)
            {
                flag_sun_control = true;
            }
            else
            {
                flag_specifid_control = true;
            }

            this.groupBoxSpecifiedTime.Enabled = flag_specifid_control;
            this.groupBoxSunControl.Enabled = flag_sun_control;
        }

        int GetRadioTimeType()
        {
            int type;

            if (this.ModelItem_radioButtonTimeType0.Checked) type = 0;
            else if (this.ModelItem_radioButtonTimeType1.Checked) type = 1;
            else if (this.ModelItem_radioButtonTimeType2.Checked) type = 2;
            else type = 0;

            return type;
        }

        // 일출/일몰 일때는 제어 시간을 계산해서 디스프레이 해준다.
        void RecalcSunContolTime()
        {
            int time_type = GetRadioTimeType();

            if (time_type == 0) return;

            double longitude, latitude;

            LocationLib.CalcLocationInfomationByTitle(LocationLib.arrayLocation, this.textBoxLocation_ModelItem.Text, out latitude, out longitude);
            DateTime t = DateTime.Now;
            SYSTEMTIME tRise = new SYSTEMTIME(), tSet = new SYSTEMTIME();
            SunRiseSet.GetTime(t.Year, t.Month, t.Day, longitude, latitude, 0, tRise, tSet);

            if (time_type == 1)
            {
                t = new DateTime(t.Year, t.Month, t.Day, tRise.wHour, tRise.wMinute, 0);
            }
            else if (time_type == 2)
            {
                t = new DateTime(t.Year, t.Month, t.Day, tSet.wHour, tSet.wMinute, 0);
            }

            t = t.AddMinutes(ConvertTool.ToInt32(this.numericUpDownSunAfterBefore_ModelItem.Value));

            t = t.ToLocalTime();
            this.m_comboHour_ModelItem.Text = t.Hour.ToString("00");
            this.m_comboMinute_ModelItem.Text = t.Minute.ToString("00");
        }

        private void m_list_ModelOne_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ModifyItem();
        }



        private void ModifyItem()
        {
            //int retn = -1;
            //if (this.m_list_ModelItem.SelectedItems.Count > 0)
            //{
            //    retn = this.m_list_ModelItem.SelectedItems[0].Index;
            //}
            //else return;
            if (m_list_ModelOne.SelectedItems.Count > 0)
            {
                this.groupBoxModelItem.Enabled = true;

                int retn = FormConfigModel.ListCtrlGetModifyItem(m_list_ModelOne);
                if (retn == -1) return;

                pos_item = retn;

                if (Tools.IsLangKorean())
                {
                    this.groupBoxModelItem.Text = "아이템 수정 중...";
                }
                else
                {
                    this.groupBoxModelItem.Text = "Model Item Modifying...";
                }

                SCHEDULE_MODEL_ITEM_STRUCT item;

                item = (SCHEDULE_MODEL_ITEM_STRUCT)model_one.blockItem[retn];



                m_nHour = item.hour;
                m_nMinute = item.minute;
                this.ModelItem_textBoxScript.Text = item.script;

                blockTemp_Item = (ArrayList)Tools.CopyObject(item.blockTag);

                SetStruct(item);


                ConfigModelItem_Load();

            }
            else
            {
                pos_item = -1;

                if (Tools.IsLangKorean())
                {
                    this.groupBoxModelItem.Text = "모델 아이템이 선택되지 않았습니다 !";
                }
                else
                {
                    this.groupBoxModelItem.Text = "No Model Item is selected !";
                }

                this.groupBoxModelItem.Enabled = false;

                ConfigModelItem_Load();
                //SaveModelItem();

            }


        }

        private void SaveModelItemNew()
        {

            SCHEDULE_TAG_VALUE_STRUCT tagValue;
            int i;
            ListViewItem item;

            blockTemp_Item.Clear();

            for (i = 0; i < m_list_ModelItem.Items.Count; i++)
            {
                tagValue = new SCHEDULE_TAG_VALUE_STRUCT();
                item = m_list_ModelItem.Items[i];

                tagValue.tag = item.SubItems[0].Text;
                tagValue.val = item.SubItems[2].Text;
                blockTemp_Item.Add(tagValue);
            }

            this.m_nHour = ConvertTool.ToInt32(this.m_comboHour_ModelItem.Text);
            this.m_nMinute = ConvertTool.ToInt32(this.m_comboMinute_ModelItem.Text);

            int pos = model_one.blockItem.Count;
            SCHEDULE_MODEL_ITEM_STRUCT item_new;

            item_new = new SCHEDULE_MODEL_ITEM_STRUCT();

            GetStruct(item_new);

            item_new.hour = this.m_nHour;
            item_new.minute = this.m_nMinute;

            item_new.script = this.ModelItem_textBoxScript.Text;

            item_new.blockTag = (ArrayList)Tools.CopyObject(blockTemp_Item);

            model_one.blockItem.Add(item_new);
            ListViewItem lvi = new ListViewItem("");
            lvi.SubItems.Add("");
            m_list_ModelOne.Items.Add(lvi);
            ChangeOneItem(lvi, item_new);
            bChangeItemFlag = true;

        }

        private void SaveModelItemModify()
        {

            SCHEDULE_TAG_VALUE_STRUCT tagValue;
            int i;
            ListViewItem item;

            blockTemp_Item.Clear();

            for (i = 0; i < m_list_ModelItem.Items.Count; i++)
            {
                tagValue = new SCHEDULE_TAG_VALUE_STRUCT();
                item = m_list_ModelItem.Items[i];

                tagValue.tag = item.SubItems[0].Text;
                tagValue.val = item.SubItems[2].Text;
                blockTemp_Item.Add(tagValue);
            }

            this.m_nHour = ConvertTool.ToInt32(this.m_comboHour_ModelItem.Text);
            this.m_nMinute = ConvertTool.ToInt32(this.m_comboMinute_ModelItem.Text);

            int pos = model_one.blockItem.Count;
            SCHEDULE_MODEL_ITEM_STRUCT item_modify;

            item_modify = (SCHEDULE_MODEL_ITEM_STRUCT)model_one.blockItem[pos_item];

            GetStruct(item_modify);

            item_modify.hour = this.m_nHour;
            item_modify.minute = this.m_nMinute;

            item_modify.script = this.ModelItem_textBoxScript.Text;

            item_modify.blockTag = (ArrayList)Tools.CopyObject(blockTemp_Item);

            //model_one.blockItem.Add(item_modify);
            //ListViewItem lvi = new ListViewItem("");
            //lvi.SubItems.Add("");
            //m_list_ModelOne.Items.Add(lvi);

            ChangeOneItem(m_list_ModelOne.Items[pos_item], item_modify);
            bChangeItemFlag = true;

            int retn = FormConfigModel.ListCtrlGetModifyItem(m_list_ConfigModel);
            if (retn == -1) return;

            blockTemp[retn] = model_one;



        }

        public void SetStruct(SCHEDULE_MODEL_ITEM_STRUCT item)
        {
            this.ModelItem_radioButtonTimeType0.Checked = (item.nTimeType == 0);
            this.ModelItem_radioButtonTimeType1.Checked = (item.nTimeType == 1);
            this.ModelItem_radioButtonTimeType2.Checked = (item.nTimeType == 2);

            this.numericUpDownSunAfterBefore_ModelItem.Value = item.nSunBeforeAfterMinutes;
            this.textBoxLocation_ModelItem.Text = item.sLocation;
        }

        public void GetStruct(SCHEDULE_MODEL_ITEM_STRUCT item)
        {
            item.nTimeType = GetRadioTimeType();
            item.nSunBeforeAfterMinutes = ConvertTool.ToInt32(this.numericUpDownSunAfterBefore_ModelItem.Value);
            item.sLocation = this.textBoxLocation_ModelItem.Text;
        }

        private void m_list_ModelOne_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ModifyItem();
        }

        private void ModelItem_radioButtonTimeType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeType();
            RecalcSunContolTime();
        }

        private void ModelItem_radioButtonTimeType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeType();
            RecalcSunContolTime();
        }

        private void ModelItem_radioButtonTimeType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeType();
            RecalcSunContolTime();
        }

        private void buttonLocation_ModelItem_Click(object sender, EventArgs e)
        {
            FormConfigLocation dialog = new FormConfigLocation();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.textBoxLocation_ModelItem.Text = dialog.sSelectedCity;

                RecalcSunContolTime();
            }
        }

        private void ModelItem_buttonAdd_Click(object sender, EventArgs e)
        {
            FormSelectTag dialog = new FormSelectTag();
            dialog.bUseTagAI = true;
            dialog.bUseTagDI = true;
            dialog.bUseTagST = true;
            dialog.bUseTagAO = true;
            dialog.bUseTagDO = true;
            dialog.bUseTagGDO = true;
            dialog.bUseMultiSelection = true;
            dialog.checkBoxMultiSelect.Checked = bTagMultiSelection;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (dialog.checkBoxMultiSelect.Checked == false || dialog.m_list.SelectedItems.Count <= 1)
                {
                    AddOneTag(dialog.sTag, dialog.sDes);
                }
                else
                {
                    ListViewItem lvi;
                    string tag;
                    int[] tag_pos = new int[1];
                    TagPublicClass tp;

                    for (int i = 0; i < dialog.m_list.SelectedItems.Count; i++)
                    {
                        lvi = dialog.m_list.SelectedItems[i];

                        tag = dialog.GetFullTagName(lvi.SubItems[0].Text);

                        tp = TagLib.GetStructPublic(tag, ref tag_pos);

                        AddOneTag(tag, tp.description);
                    }
                }

                bTagMultiSelection = dialog.checkBoxMultiSelect.Checked;
            }
        }

        private void AddOneTag(string tag, string des)
        {
            ListViewItem item;
            for (int i = 0; i < this.m_list_ModelItem.Items.Count; i++)
            {
                item = this.m_list_ModelItem.Items[i];
                if (String.Compare(item.Text, tag, true) == 0)
                {
                    item.Selected = true;
                    return;	// already registerd
                }
            }

            item = new ListViewItem(tag);
            item.SubItems.Add(des);
            item.SubItems.Add("0");
            this.m_list_ModelItem.Items.Add(item);
            this.m_list_ModelItem.EnsureVisible(this.m_list_ModelItem.Items.Count - 1);
            item.Selected = true;
        }

        private void ModifyTag()
        {
            int retn = FormConfigModel.ListCtrlGetModifyItem(m_list_ModelItem);
            if (retn == -1) return;

            FormModelChangeValue dialog = new FormModelChangeValue();

            ListViewItem item = m_list_ModelItem.Items[retn];

            dialog.textBoxTag.Text = item.SubItems[0].Text;
            dialog.textBoxDescription.Text = item.SubItems[1].Text;
            dialog.textBoxValue.Text = item.SubItems[2].Text;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                item.SubItems[0].Text = dialog.textBoxTag.Text;
                item.SubItems[2].Text = dialog.textBoxValue.Text;
            }
        }

        private void ModelItem_buttonModify_Click(object sender, EventArgs e)
        {
            ModifyTag();
        }

        private void ModelItem_buttonDelete_Click(object sender, EventArgs e)
        {
            int retn = FormConfigModel.ListCtrlGetDeleteItem(m_list_ModelItem);
            if (retn == -1) return;

            m_list_ModelItem.Items.RemoveAt(retn);
        }

        private void m_list_ModelItem_DoubleClick(object sender, EventArgs e)
        {
            ModifyTag();
        }




        private void button_ModelOne_Add_Click(object sender, EventArgs e)
        {
            if (pos_item != -1)
            {
                SaveModelItemModify();

            }
            else
            {
                SaveModelItemNew();
            }
        }

        private void button_ModelOne_New_Click(object sender, EventArgs e)
        {
            if (m_list_ModelOne.Items.Count > 0)
            {
                if (m_list_ModelOne.SelectedItems.Count > 0)
                {
                    if (pos_item != -1)
                    {
                        SaveModelItemModify();

                    }
                    else
                    {
                        SaveModelItemNew();
                    }
                }
            }
            // 날아가는거 방지를 위하여 추가 25-03-25 hsjeong

            int retn = FormConfigModel.ListCtrlGetModifyItem(m_list_ConfigModel);
            if (retn == -1) return;



            SCHEDULE_MODEL_ITEM_STRUCT item = new SCHEDULE_MODEL_ITEM_STRUCT();
            model_one.blockItem.Add(item);

            blockTemp[retn] = model_one;

            int l = 0;
            this.m_list_ModelOne.Items.Clear();
            for (l = 0; l < model_one.blockItem.Count; l++)
            {
                item = (SCHEDULE_MODEL_ITEM_STRUCT)model_one.blockItem[l];
                ListViewItem lvi = new ListViewItem("");
                lvi.SubItems.Add("");
                ChangeOneItem(lvi, item);
                this.m_list_ModelOne.Items.Add(lvi);
            }


            int curr = m_list_ModelOne.Items.Count - 1;
            this.m_list_ModelOne.Items[curr].Selected = true;
            this.m_list_ModelOne.EnsureVisible(curr);

            bChangeItemFlag = true;



        }

        private void button_ModelOne_Delete_Click(object sender, EventArgs e)
        {
            int retn = FormConfigModel.ListCtrlGetDeleteItem(m_list_ModelOne);
            if (retn == -1) return;

            int retn2 = FormConfigModel.ListCtrlGetModifyItem(m_list_ConfigModel);
            if (retn2 == -1) return;

            m_list_ModelOne.Items.RemoveAt(retn);
            model_one.blockItem.RemoveAt(retn);

            blockTemp[retn2] = model_one;

            bChangeItemFlag = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (m_list_ModelOne.Items.Count > 0)
            {
                if (m_list_ModelOne.SelectedItems.Count > 0)
                {
                    if (pos_item != -1)
                    {
                        SaveModelItemModify();

                    }
                    else
                    {
                        SaveModelItemNew();
                    }
                }
            }

            //날아가는거 방지를 위하여 추가 hsjeong 25-03-25
            
            TotalConfig.AutoBaseMainListCtrlConfigSave(m_list_ConfigModel, "DllDialogConfigModel");

            ScheduleLib.ModelSave(blockTemp);

            DialogResult = DialogResult.OK;
            Close();


        }

        private void buttonConfigModelCopy_Click(object sender, EventArgs e)
        {
            int retn = ListCtrlGetSelect(m_list_ConfigModel);
            if (retn == -1) return;

            SCHEDULE_MODEL_STRUCT model;

            model = (SCHEDULE_MODEL_STRUCT)blockTemp[retn];

            bCopyFlag = true;
            modelClipboard = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(model);

            this.buttonConfigModelPaste.Enabled = true;
        }

        private void buttonConfigModelPaste_Click(object sender, EventArgs e)
        {
            if (bCopyFlag == false) return;

            SCHEDULE_MODEL_STRUCT model;
            model = (SCHEDULE_MODEL_STRUCT)Tools.CopyObject(modelClipboard);

            if (Tools.IsLangKorean())
                model.title += "-복사본";
            else if (Tools.IsLangJapanese())
                model.title += "-コピー";
            else
                model.title += "-Copy";

            int pos = blockTemp.Count;

            int count = 0;
            string title_temp = model.title;

            for (int i = 0; i < blockTemp.Count; i++)
            {


                SCHEDULE_MODEL_STRUCT model2 = (SCHEDULE_MODEL_STRUCT)blockTemp[i];
                if (title_temp == model2.title)
                {
                    title_temp = model.title + count.ToString();
                    count++;
                    i = 0;

                }

            }
            model.title = title_temp;

            blockTemp.Add(model);
            ListViewItem item = m_list_ConfigModel.Items.Insert(pos, model.title, 25);
            item.SubItems.Add(model.description);

            m_list_ConfigModel.Items[m_list_ConfigModel.Items.Count - 1].Selected = true;
            m_list_ConfigModel.EnsureVisible(m_list_ConfigModel.Items.Count - 1);

            bChangeFlag = true;
        }

        
        

		

	}
}

