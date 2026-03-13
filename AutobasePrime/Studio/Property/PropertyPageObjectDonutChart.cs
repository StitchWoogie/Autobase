using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using DialogTag;

using AutoLibLocal;

namespace Studio
{
    /// <summary>
    /// Summary description for PropertyPageObjectDonutChart.
    /// </summary>
    public class PropertyPageObjectDonutChart : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;



        static bool bTagMultiSelection = true;
        public ArrayList arrayTemp = new ArrayList();

        Color[] preparedColor = new Color[10] { Color.Red, Color.Blue, Color.Green, Color.Cyan, Color.Magenta, Color.Yellow, Color.Purple, Color.Lime, Color.Orange, Color.Violet };
        int nPreparedColor = 0;
        int old_pos = -1;
        private GroupBox groupBox3;
        private Button buttonOutsideColor;
        private Label label14;
        private Button buttonInsideColor;
        private Label label13;
        private GroupBox groupBox6;
        private GroupBox groupBox11;
        private Label label18;
        private TextBox textBoxDisplayFormat;
        private Button buttonTotalColor;
        private CheckBox checkBoxTotalValue;
        private NumericUpDown numericUpDownTotalSize;
        private Label label15;
        private GroupBox groupBox10;
        private Label label17;
        private Label label16;
        private NumericUpDown numericUpDownSpaceInside;
        private Label label10;
        private NumericUpDown numericUpDownSpaceOutside;
        private Label label8;
        private GroupBox groupBox9;
        private NumericUpDown numericUpDownPercent;
        private RadioButton radioButtonPercent2;
        private Label label7;
        private RadioButton radioButtonPercent1;
        private RadioButton radioButtonPercent0;
        private GroupBox groupBox8;
        private NumericUpDown numericUpDownValue;
        private RadioButton radioButtonValue2;
        private Label label6;
        private RadioButton radioButtonValue1;
        private RadioButton radioButtonValue0;
        private GroupBox groupBox7;
        private NumericUpDown numericUpDownTagName;
        private RadioButton radioButtonTagName2;
        private Label label5;
        private RadioButton radioButtonTagName1;
        private RadioButton radioButtonTagName0;
        private Button buttonEdit;
        private Button buttonDown;
        private Button buttonUp;
        private Button buttonDelete;
        private Button buttonAdd;
        private ListView m_list;
        private ColumnHeader columnHeader1;
        private GroupBox groupBox5;
        private CheckBox checkBoxDisplayGuideLine;
        private NumericUpDown numericUpDownGuideSpace;
        private Label label11;
        private NumericUpDown numericUpDownGuideSize;
        private Label label4;
        private Button buttonTagColor;
        private Label label9;
        private GroupBox groupBox2;
        private RadioButton radioButtonBarDir1;
        private RadioButton radioButtonBarDir0;
        private GroupBox groupBox1;
        private NumericUpDown numericUpDownSpaceAngle;
        private Label label2;
        private GroupBox groupBox4;
        private Label label3;
        private Label label12;
        private NumericUpDown numericUpDownLineThick;
        private Label label1;
        private NumericUpDown numericUpDownStartAngle;
        bool bDeleting = false;

        public PropertyPageObjectDonutChart()
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDonutChart));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonOutsideColor = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.buttonInsideColor = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBoxDisplayFormat = new System.Windows.Forms.TextBox();
            this.buttonTotalColor = new System.Windows.Forms.Button();
            this.checkBoxTotalValue = new System.Windows.Forms.CheckBox();
            this.numericUpDownTotalSize = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.numericUpDownSpaceInside = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownSpaceOutside = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.numericUpDownPercent = new System.Windows.Forms.NumericUpDown();
            this.radioButtonPercent2 = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButtonPercent1 = new System.Windows.Forms.RadioButton();
            this.radioButtonPercent0 = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.numericUpDownValue = new System.Windows.Forms.NumericUpDown();
            this.radioButtonValue2 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButtonValue1 = new System.Windows.Forms.RadioButton();
            this.radioButtonValue0 = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.numericUpDownTagName = new System.Windows.Forms.NumericUpDown();
            this.radioButtonTagName2 = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButtonTagName1 = new System.Windows.Forms.RadioButton();
            this.radioButtonTagName0 = new System.Windows.Forms.RadioButton();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxDisplayGuideLine = new System.Windows.Forms.CheckBox();
            this.numericUpDownGuideSpace = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDownGuideSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonTagColor = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonBarDir1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBarDir0 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownSpaceAngle = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownLineThick = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownStartAngle = new System.Windows.Forms.NumericUpDown();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalSize)).BeginInit();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpaceInside)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpaceOutside)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPercent)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTagName)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSize)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpaceAngle)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineThick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonOutsideColor);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.buttonInsideColor);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.buttonEdit);
            this.groupBox3.Controls.Add(this.buttonDown);
            this.groupBox3.Controls.Add(this.buttonUp);
            this.groupBox3.Controls.Add(this.buttonDelete);
            this.groupBox3.Controls.Add(this.buttonAdd);
            this.groupBox3.Controls.Add(this.m_list);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.buttonTagColor);
            this.groupBox3.Controls.Add(this.label9);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // buttonOutsideColor
            // 
            resources.ApplyResources(this.buttonOutsideColor, "buttonOutsideColor");
            this.buttonOutsideColor.Name = "buttonOutsideColor";
            this.buttonOutsideColor.Click += new System.EventHandler(this.buttonOutsideColor_Click);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // buttonInsideColor
            // 
            resources.ApplyResources(this.buttonInsideColor, "buttonInsideColor");
            this.buttonInsideColor.Name = "buttonInsideColor";
            this.buttonInsideColor.Click += new System.EventHandler(this.buttonInsideColor_Click);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox11);
            this.groupBox6.Controls.Add(this.groupBox10);
            this.groupBox6.Controls.Add(this.groupBox9);
            this.groupBox6.Controls.Add(this.groupBox8);
            this.groupBox6.Controls.Add(this.groupBox7);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.label18);
            this.groupBox11.Controls.Add(this.textBoxDisplayFormat);
            this.groupBox11.Controls.Add(this.buttonTotalColor);
            this.groupBox11.Controls.Add(this.checkBoxTotalValue);
            this.groupBox11.Controls.Add(this.numericUpDownTotalSize);
            this.groupBox11.Controls.Add(this.label15);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // textBoxDisplayFormat
            // 
            resources.ApplyResources(this.textBoxDisplayFormat, "textBoxDisplayFormat");
            this.textBoxDisplayFormat.Name = "textBoxDisplayFormat";
            // 
            // buttonTotalColor
            // 
            resources.ApplyResources(this.buttonTotalColor, "buttonTotalColor");
            this.buttonTotalColor.Name = "buttonTotalColor";
            this.buttonTotalColor.Click += new System.EventHandler(this.buttonTotalColor_Click);
            // 
            // checkBoxTotalValue
            // 
            resources.ApplyResources(this.checkBoxTotalValue, "checkBoxTotalValue");
            this.checkBoxTotalValue.Name = "checkBoxTotalValue";
            // 
            // numericUpDownTotalSize
            // 
            resources.ApplyResources(this.numericUpDownTotalSize, "numericUpDownTotalSize");
            this.numericUpDownTotalSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTotalSize.Name = "numericUpDownTotalSize";
            this.numericUpDownTotalSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.label17);
            this.groupBox10.Controls.Add(this.label16);
            this.groupBox10.Controls.Add(this.numericUpDownSpaceInside);
            this.groupBox10.Controls.Add(this.label10);
            this.groupBox10.Controls.Add(this.numericUpDownSpaceOutside);
            this.groupBox10.Controls.Add(this.label8);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // numericUpDownSpaceInside
            // 
            resources.ApplyResources(this.numericUpDownSpaceInside, "numericUpDownSpaceInside");
            this.numericUpDownSpaceInside.Name = "numericUpDownSpaceInside";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // numericUpDownSpaceOutside
            // 
            resources.ApplyResources(this.numericUpDownSpaceOutside, "numericUpDownSpaceOutside");
            this.numericUpDownSpaceOutside.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownSpaceOutside.Name = "numericUpDownSpaceOutside";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.numericUpDownPercent);
            this.groupBox9.Controls.Add(this.radioButtonPercent2);
            this.groupBox9.Controls.Add(this.label7);
            this.groupBox9.Controls.Add(this.radioButtonPercent1);
            this.groupBox9.Controls.Add(this.radioButtonPercent0);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // numericUpDownPercent
            // 
            resources.ApplyResources(this.numericUpDownPercent, "numericUpDownPercent");
            this.numericUpDownPercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPercent.Name = "numericUpDownPercent";
            this.numericUpDownPercent.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonPercent2
            // 
            resources.ApplyResources(this.radioButtonPercent2, "radioButtonPercent2");
            this.radioButtonPercent2.Name = "radioButtonPercent2";
            this.radioButtonPercent2.TabStop = true;
            this.radioButtonPercent2.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // radioButtonPercent1
            // 
            resources.ApplyResources(this.radioButtonPercent1, "radioButtonPercent1");
            this.radioButtonPercent1.Name = "radioButtonPercent1";
            this.radioButtonPercent1.TabStop = true;
            this.radioButtonPercent1.UseVisualStyleBackColor = true;
            // 
            // radioButtonPercent0
            // 
            resources.ApplyResources(this.radioButtonPercent0, "radioButtonPercent0");
            this.radioButtonPercent0.Name = "radioButtonPercent0";
            this.radioButtonPercent0.TabStop = true;
            this.radioButtonPercent0.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.numericUpDownValue);
            this.groupBox8.Controls.Add(this.radioButtonValue2);
            this.groupBox8.Controls.Add(this.label6);
            this.groupBox8.Controls.Add(this.radioButtonValue1);
            this.groupBox8.Controls.Add(this.radioButtonValue0);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // numericUpDownValue
            // 
            resources.ApplyResources(this.numericUpDownValue, "numericUpDownValue");
            this.numericUpDownValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownValue.Name = "numericUpDownValue";
            this.numericUpDownValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonValue2
            // 
            resources.ApplyResources(this.radioButtonValue2, "radioButtonValue2");
            this.radioButtonValue2.Name = "radioButtonValue2";
            this.radioButtonValue2.TabStop = true;
            this.radioButtonValue2.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // radioButtonValue1
            // 
            resources.ApplyResources(this.radioButtonValue1, "radioButtonValue1");
            this.radioButtonValue1.Name = "radioButtonValue1";
            this.radioButtonValue1.TabStop = true;
            this.radioButtonValue1.UseVisualStyleBackColor = true;
            // 
            // radioButtonValue0
            // 
            resources.ApplyResources(this.radioButtonValue0, "radioButtonValue0");
            this.radioButtonValue0.Name = "radioButtonValue0";
            this.radioButtonValue0.TabStop = true;
            this.radioButtonValue0.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.numericUpDownTagName);
            this.groupBox7.Controls.Add(this.radioButtonTagName2);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Controls.Add(this.radioButtonTagName1);
            this.groupBox7.Controls.Add(this.radioButtonTagName0);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // numericUpDownTagName
            // 
            resources.ApplyResources(this.numericUpDownTagName, "numericUpDownTagName");
            this.numericUpDownTagName.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTagName.Name = "numericUpDownTagName";
            this.numericUpDownTagName.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonTagName2
            // 
            resources.ApplyResources(this.radioButtonTagName2, "radioButtonTagName2");
            this.radioButtonTagName2.Name = "radioButtonTagName2";
            this.radioButtonTagName2.TabStop = true;
            this.radioButtonTagName2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // radioButtonTagName1
            // 
            resources.ApplyResources(this.radioButtonTagName1, "radioButtonTagName1");
            this.radioButtonTagName1.Name = "radioButtonTagName1";
            this.radioButtonTagName1.TabStop = true;
            this.radioButtonTagName1.UseVisualStyleBackColor = true;
            // 
            // radioButtonTagName0
            // 
            resources.ApplyResources(this.radioButtonTagName0, "radioButtonTagName0");
            this.radioButtonTagName0.Name = "radioButtonTagName0";
            this.radioButtonTagName0.TabStop = true;
            this.radioButtonTagName0.UseVisualStyleBackColor = true;
            // 
            // buttonEdit
            // 
            resources.ApplyResources(this.buttonEdit, "buttonEdit");
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonDown
            // 
            resources.ApplyResources(this.buttonDown, "buttonDown");
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // buttonUp
            // 
            resources.ApplyResources(this.buttonUp, "buttonUp");
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxDisplayGuideLine);
            this.groupBox5.Controls.Add(this.numericUpDownGuideSpace);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.numericUpDownGuideSize);
            this.groupBox5.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBoxDisplayGuideLine
            // 
            resources.ApplyResources(this.checkBoxDisplayGuideLine, "checkBoxDisplayGuideLine");
            this.checkBoxDisplayGuideLine.Name = "checkBoxDisplayGuideLine";
            // 
            // numericUpDownGuideSpace
            // 
            resources.ApplyResources(this.numericUpDownGuideSpace, "numericUpDownGuideSpace");
            this.numericUpDownGuideSpace.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownGuideSpace.Name = "numericUpDownGuideSpace";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // numericUpDownGuideSize
            // 
            resources.ApplyResources(this.numericUpDownGuideSize, "numericUpDownGuideSize");
            this.numericUpDownGuideSize.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownGuideSize.Name = "numericUpDownGuideSize";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // buttonTagColor
            // 
            resources.ApplyResources(this.buttonTagColor, "buttonTagColor");
            this.buttonTagColor.Name = "buttonTagColor";
            this.buttonTagColor.Click += new System.EventHandler(this.buttonTagColor_Click);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonBarDir1);
            this.groupBox2.Controls.Add(this.radioButtonBarDir0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonBarDir1
            // 
            resources.ApplyResources(this.radioButtonBarDir1, "radioButtonBarDir1");
            this.radioButtonBarDir1.Name = "radioButtonBarDir1";
            // 
            // radioButtonBarDir0
            // 
            resources.ApplyResources(this.radioButtonBarDir0, "radioButtonBarDir0");
            this.radioButtonBarDir0.Name = "radioButtonBarDir0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownSpaceAngle);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownStartAngle);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownSpaceAngle
            // 
            resources.ApplyResources(this.numericUpDownSpaceAngle, "numericUpDownSpaceAngle");
            this.numericUpDownSpaceAngle.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDownSpaceAngle.Name = "numericUpDownSpaceAngle";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.numericUpDownLineThick);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // numericUpDownLineThick
            // 
            resources.ApplyResources(this.numericUpDownLineThick, "numericUpDownLineThick");
            this.numericUpDownLineThick.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLineThick.Name = "numericUpDownLineThick";
            this.numericUpDownLineThick.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDownStartAngle
            // 
            resources.ApplyResources(this.numericUpDownStartAngle, "numericUpDownStartAngle");
            this.numericUpDownStartAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownStartAngle.Name = "numericUpDownStartAngle";
            // 
            // PropertyPageObjectDonutChart
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectDonutChart";
            this.groupBox3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalSize)).EndInit();
            this.groupBox10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpaceInside)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpaceOutside)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPercent)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTagName)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSize)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpaceAngle)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLineThick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAngle)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void label6_Click(object sender, System.EventArgs e)
        {

        }

        public ObjectArgsDonutChart ObjectArgs
        {
            set
            {
                Tools.SetNumericUpDownValue(this.numericUpDownStartAngle, (decimal)value.fStartAngle);
                Tools.SetNumericUpDownValue(this.numericUpDownSpaceAngle, (decimal)value.fSpaceAngle);
                Tools.SetNumericUpDownValue(this.numericUpDownLineThick, value.nLineThick);

                this.radioButtonBarDir0.Checked = (value.nBarDir == 0);
                this.radioButtonBarDir1.Checked = (value.nBarDir == 1);

                this.checkBoxDisplayGuideLine.Checked = (value.bDisplayGuideLine == 1);

                Tools.SetNumericUpDownValue(this.numericUpDownGuideSize, (decimal)value.nDisplayGuideLineSize);
                Tools.SetNumericUpDownValue(this.numericUpDownGuideSpace, (decimal)value.nDisplayGuideLineSpace);

                Tools.SetNumericUpDownValue(this.numericUpDownSpaceInside, (decimal)value.nSpaceInside);
                Tools.SetNumericUpDownValue(this.numericUpDownSpaceOutside, (decimal)value.nSpaceOutside);

                Tools.SetNumericUpDownValue(this.numericUpDownTagName, (decimal)value.nTagNameSize);
                this.radioButtonTagName0.Checked = (value.nTagNameOption == 0);
                this.radioButtonTagName1.Checked = (value.nTagNameOption == 1);
                this.radioButtonTagName2.Checked = (value.nTagNameOption == 2);

                Tools.SetNumericUpDownValue(this.numericUpDownValue, (decimal)value.nValueSize);
                this.radioButtonValue0.Checked = (value.nValueOption == 0);
                this.radioButtonValue1.Checked = (value.nValueOption == 1);
                this.radioButtonValue2.Checked = (value.nValueOption == 2);

                Tools.SetNumericUpDownValue(this.numericUpDownPercent, (decimal)value.nPercentSize);
                this.radioButtonPercent0.Checked = (value.nPercentOption == 0);
                this.radioButtonPercent1.Checked = (value.nPercentOption == 1);
                this.radioButtonPercent2.Checked = (value.nPercentOption == 2);

                this.checkBoxTotalValue.Checked = (value.bDisplayTotalValue == 1);
                Tools.SetNumericUpDownValue(this.numericUpDownTotalSize, (decimal)value.nDisplayTotalSize);
                this.buttonTotalColor.BackColor = value.color_total;
                this.textBoxDisplayFormat.Text = value.sDisplayTotalFormat;

            }
            get
            {
                ObjectArgsDonutChart args = new ObjectArgsDonutChart();

                args.fStartAngle = (float)ConvertTool.ToDouble(this.numericUpDownStartAngle.Value);
                args.fSpaceAngle = (float)ConvertTool.ToDouble(this.numericUpDownSpaceAngle.Value);
                args.nLineThick = ConvertTool.ToInt32(this.numericUpDownLineThick.Value);

                if (this.radioButtonBarDir0.Checked) args.nBarDir = 0;
                else if (this.radioButtonBarDir1.Checked) args.nBarDir = 1;

                if (this.checkBoxDisplayGuideLine.Checked) args.bDisplayGuideLine = 1;
                else args.bDisplayGuideLine = 0;

                args.nDisplayGuideLineSize = ConvertTool.ToInt32(this.numericUpDownGuideSize.Value);
                args.nDisplayGuideLineSpace = ConvertTool.ToInt32(this.numericUpDownGuideSpace.Value);

                args.nSpaceInside = ConvertTool.ToInt32(this.numericUpDownSpaceInside.Value);
                args.nSpaceOutside = ConvertTool.ToInt32(this.numericUpDownSpaceOutside.Value);

                if (this.checkBoxTotalValue.Checked) args.bDisplayTotalValue = 1;
                else args.bDisplayTotalValue = 0;
                args.nDisplayTotalSize = ConvertTool.ToInt32(this.numericUpDownTotalSize.Value);
                args.sDisplayTotalFormat = this.textBoxDisplayFormat.Text;

                args.color_total = this.buttonTotalColor.BackColor;

                args.nTagNameSize = ConvertTool.ToInt32(this.numericUpDownTagName.Value);
                if (this.radioButtonTagName0.Checked) args.nTagNameOption = 0;
                else if (this.radioButtonTagName1.Checked) args.nTagNameOption = 1;
                else if (this.radioButtonTagName2.Checked) args.nTagNameOption = 2;

                args.nValueSize = ConvertTool.ToInt32(this.numericUpDownValue.Value);
                if (this.radioButtonValue0.Checked) args.nValueOption = 0;
                else if (this.radioButtonValue1.Checked) args.nValueOption = 1;
                else if (this.radioButtonValue2.Checked) args.nValueOption = 2;

                args.nPercentSize = ConvertTool.ToInt32(this.numericUpDownPercent.Value);
                if (this.radioButtonPercent0.Checked) args.nPercentOption = 0;
                else if (this.radioButtonPercent1.Checked) args.nPercentOption = 1;
                else if (this.radioButtonPercent2.Checked) args.nPercentOption = 2;




                return args;
            }
        }

        public ArrayList ChartMember
        {
            set
            {
                DONUT_CHART_TAG_MEMBER member;

                for (int i = 0; i < value.Count; i++)
                {
                    arrayTemp.Add(Tools.CopyObject(value[i]));
                }

                int[] pos = new int[1];
                TagPublicClass tp;

                for (int i = 0; i < arrayTemp.Count; i++)
                {
                    member = (DONUT_CHART_TAG_MEMBER)arrayTemp[i];

                    ListViewItem item = new ListViewItem();
                    item.Text = member.tag;
                    tp = AutoLibLocal.TagLib.GetStructPublic(member.tag, ref pos);

                    m_list.Items.Add(item);
                }
            }
            get
            {
                DialogToArray();
                return arrayTemp;
            }
        }




        private void buttonAdd_Click(object sender, EventArgs e)
        {
            FormSelectTag dialog = new FormSelectTag();
            dialog.bUseTagAI = true;

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
                    for (int i = 0; i < dialog.m_list.SelectedItems.Count; i++)
                    {
                        lvi = dialog.m_list.SelectedItems[i];
                        tag = dialog.GetFullTagName(lvi.SubItems[0].Text);
                        string des = lvi.SubItems[1].Text; // 설명을가져옵니다. 필요에따라조정하세요.
                        AddOneTag(tag, des);
                    }
                }
                bTagMultiSelection = dialog.checkBoxMultiSelect.Checked;
            }
        }

        private void AddOneTag(string tag, string des)
        {
            ListViewItem item = new ListViewItem(tag);
            item.SubItems.Add(des);
            m_list.Items.Add(item);

            DONUT_CHART_TAG_MEMBER member = new DONUT_CHART_TAG_MEMBER();

            member.color_tag = preparedColor[nPreparedColor];
            member.color_outside = preparedColor[nPreparedColor];
            nPreparedColor++;
            nPreparedColor %= preparedColor.Length;
            member.color_inside = Color.Black;

            member.tag = tag;
            arrayTemp.Add(member);

            item.Selected = true;
            item.EnsureVisible();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0 || m_list.SelectedItems[0].Index == 0)
                return;

            bDeleting = true;

            int selectedIndex = m_list.SelectedItems[0].Index;
            ListViewItem item = m_list.SelectedItems[0];

            // ListView에서항목이동
            m_list.Items.RemoveAt(selectedIndex);
            m_list.Items.Insert(selectedIndex - 1, item);

            // arrayTemp에서해당항목이동
            object member = arrayTemp[selectedIndex];
            arrayTemp.RemoveAt(selectedIndex);
            arrayTemp.Insert(selectedIndex - 1, member);

            m_list.Items[selectedIndex - 1].Selected = true;
            m_list.Select();

            old_pos = -1;
            bDeleting = false;

            EnsureVisible(selectedIndex - 1);
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0 || m_list.SelectedItems[0].Index == m_list.Items.Count - 1)
                return;

            bDeleting = true;

            int selectedIndex = m_list.SelectedItems[0].Index;
            ListViewItem item = m_list.SelectedItems[0];

            // ListView에서항목이동
            m_list.Items.RemoveAt(selectedIndex);
            m_list.Items.Insert(selectedIndex + 1, item);

            // arrayTemp에서해당항목이동
            object member = arrayTemp[selectedIndex];
            arrayTemp.RemoveAt(selectedIndex);
            arrayTemp.Insert(selectedIndex + 1, member);

            m_list.Items[selectedIndex + 1].Selected = true;
            m_list.Select();

            old_pos = -1;
            bDeleting = false;

            EnsureVisible(selectedIndex + 1);
        }

        private void EnsureVisible(int index)
        {
            if (index >= 0 && index < m_list.Items.Count)
            {
                m_list.EnsureVisible(index);
            }
        }

        private void buttonDelete_Click(object sender, System.EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select item to delete", "Select Error");
                return;
            }

            bDeleting = true;
            int Index = m_list.SelectedItems[0].Index;

            arrayTemp.RemoveAt(Index);
            m_list.Items.RemoveAt(Index);
            old_pos = -1;
            bDeleting = false;
        }

        private void buttonTagColor_Click(object sender, System.EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonTagColor.BackColor, true);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonTagColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (bDeleting == true) return;
            DialogToArray();
            ArrayToDialog();
            if (m_list.SelectedItems.Count > 0)
                old_pos = m_list.SelectedItems[0].Index;
            else
                old_pos = -1;
        }

        void DialogToArray()
        {
            if (old_pos == -1) return;

            int Index = old_pos;

            if (Index >= arrayTemp.Count) return;

            DONUT_CHART_TAG_MEMBER member;
            member = (DONUT_CHART_TAG_MEMBER)arrayTemp[Index];

            member.color_tag = this.buttonTagColor.BackColor;
            member.color_inside = this.buttonInsideColor.BackColor;
            member.color_outside = this.buttonOutsideColor.BackColor;




        }

        void ArrayToDialog()
        {
            if (m_list.SelectedItems.Count == 0) return;

            int Index = m_list.SelectedItems[0].Index;

            DONUT_CHART_TAG_MEMBER member;
            member = (DONUT_CHART_TAG_MEMBER)arrayTemp[Index];



            this.buttonTagColor.BackColor = member.color_tag;
            this.buttonInsideColor.BackColor = member.color_inside;
            this.buttonOutsideColor.BackColor = member.color_outside;


        }



        private void buttonTagNameColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonTagColor.BackColor, true);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonTagColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonInsideColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonInsideColor.BackColor, true);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonInsideColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonOutsideColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonOutsideColor.BackColor, true);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonOutsideColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonTotalColor_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonTotalColor.BackColor, true);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonTotalColor.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0)
                return;

            bDeleting = true;

            int selectedIndex = m_list.SelectedItems[0].Index;
            FormSelectTag tag = new FormSelectTag();


            tag.bUseTagAI = true;

            if (tag.ShowDialog(this) == DialogResult.OK)
            {
                DONUT_CHART_TAG_MEMBER member;
                member = (DONUT_CHART_TAG_MEMBER)arrayTemp[selectedIndex];

                member.tag = tag.sTag;
                member.tag_description = tag.sDes;

                m_list.Items[selectedIndex].Text = tag.sTag;


                arrayTemp[selectedIndex] = member;


            }


            bDeleting = false;
        }





















    }
}
