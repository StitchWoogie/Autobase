using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLibLocal;
using Studio.Script;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageExpandOption.
	/// </summary>
	public class PropertyPageExpandOption : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.GroupBox groupBox9;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		
		private System.Windows.Forms.CheckBox checkBoxSizeHeight;
		private System.Windows.Forms.CheckBox checkBoxSizeWidth;
		private System.Windows.Forms.Button buttonSizeHeight;
		private System.Windows.Forms.Button buttonSizeWidth;
		private System.Windows.Forms.CheckBox checkBoxUse;
		
		private System.Windows.Forms.CheckBox checkBoxLocationY;
		private System.Windows.Forms.CheckBox checkBoxLocationX;
		private System.Windows.Forms.Button buttonLocationY;
		private System.Windows.Forms.Button buttonLocationX;
		private System.Windows.Forms.CheckBox checkBoxEventSelChange;
		private System.Windows.Forms.CheckBox checkBoxEventKeyDown;
		private System.Windows.Forms.Button buttonEventSelChange;
		private System.Windows.Forms.Button buttonEventKeyDown;
		private System.Windows.Forms.CheckBox checkBoxBlinking;
		private System.Windows.Forms.CheckBox checkBoxVisible;
		private System.Windows.Forms.Button buttonBlinking;
		private System.Windows.Forms.Button buttonVisible;
		private System.Windows.Forms.CheckBox checkBoxAnimationSpeed;
		private System.Windows.Forms.Button buttonAnimationSpeed;
		private System.Windows.Forms.CheckBox checkBoxZoneDisplay;
		private System.Windows.Forms.Button buttonZoneDisplay;
		private System.Windows.Forms.CheckBox checkBoxMouseUp;
		private System.Windows.Forms.CheckBox checkBoxMouseDown;
		private System.Windows.Forms.Button buttonMouseUp;
		private System.Windows.Forms.Button buttonMouseDown;
		private System.Windows.Forms.CheckBox checkBoxSliderVert;
		private System.Windows.Forms.CheckBox checkBoxSliderHorz;
		private System.Windows.Forms.Button buttonSliderVert;
		private System.Windows.Forms.Button buttonSliderHorz;
		private System.Windows.Forms.CheckBox checkBoxColorFill;
		private System.Windows.Forms.CheckBox checkBoxColorLine;
		private System.Windows.Forms.Button buttonColorFill;
		private System.Windows.Forms.Button buttonColorLine;
		private System.Windows.Forms.CheckBox checkBoxThickLine;
		private System.Windows.Forms.Button buttonThickLine;

		public bool bUseSizeWidth = false;
		public bool bUseSizeHeight = false;
		public bool bUseLocationX = false;
		public bool bUseLocationY = false;
		public bool bUseEventKeyDown = false;
		public bool bUseEventSelChange = false;
		public bool bUseMouseLeftDown = false;
		public bool bUseMouseLeftUp = false;
        public bool bUseMouseRightDown = false;
        public bool bUseMouseRightUp = false;

        public bool bUseMouseEnter = false;
        public bool bUseMouseLeave = false;
        public bool bUseMouseMove = false;

		public bool bUseZoneDisplay = false;
		public bool bUseVisible = false;
		public bool bUseBlinking = false;
		public bool bUseAnimationSpeed = false;
		public bool bUseSliderHorz = false;
		public bool bUseSliderVert = false;
		public bool bUseColorLine = false;
		public bool bUseColorFill = false;
		public bool bUseColorText = false;
		public bool bUseColorBack = false;
		public bool bUseThickLine = false;
        public bool bUseRotation = true;
		private System.Windows.Forms.CheckBox checkBoxColorText;
		private System.Windows.Forms.Button buttonColorText;
		private System.Windows.Forms.CheckBox checkBoxColorBack;
		private System.Windows.Forms.Button buttonColorBack;
        private CheckBox checkBoxRotate;
        private Button buttonRotate;
        private CheckBox checkBoxMouseRightUp;
        private CheckBox checkBoxMouseRightDown;
        private Button buttonMouseRightUp;
        private Button buttonMouseRightDown;
        private CheckBox checkBoxMouseLeave;
        private Button buttonMouseLeave;
        private CheckBox checkBoxMouseEnter;
        private Button buttonMouseEnter;
        private CheckBox checkBoxMouseMove;
        private Button buttonMouseMove;


		ObjectExpand objExpand = null;

		public PropertyPageExpandOption(ObjectExpand obj)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			objExpand = obj;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageExpandOption));
            this.checkBoxUse = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSizeHeight = new System.Windows.Forms.CheckBox();
            this.checkBoxSizeWidth = new System.Windows.Forms.CheckBox();
            this.buttonSizeHeight = new System.Windows.Forms.Button();
            this.buttonSizeWidth = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxLocationY = new System.Windows.Forms.CheckBox();
            this.checkBoxLocationX = new System.Windows.Forms.CheckBox();
            this.buttonLocationY = new System.Windows.Forms.Button();
            this.buttonLocationX = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxEventSelChange = new System.Windows.Forms.CheckBox();
            this.checkBoxEventKeyDown = new System.Windows.Forms.CheckBox();
            this.buttonEventSelChange = new System.Windows.Forms.Button();
            this.buttonEventKeyDown = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxRotate = new System.Windows.Forms.CheckBox();
            this.buttonRotate = new System.Windows.Forms.Button();
            this.checkBoxBlinking = new System.Windows.Forms.CheckBox();
            this.checkBoxVisible = new System.Windows.Forms.CheckBox();
            this.buttonBlinking = new System.Windows.Forms.Button();
            this.buttonVisible = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxAnimationSpeed = new System.Windows.Forms.CheckBox();
            this.buttonAnimationSpeed = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBoxMouseMove = new System.Windows.Forms.CheckBox();
            this.buttonMouseMove = new System.Windows.Forms.Button();
            this.checkBoxMouseLeave = new System.Windows.Forms.CheckBox();
            this.buttonMouseLeave = new System.Windows.Forms.Button();
            this.checkBoxMouseEnter = new System.Windows.Forms.CheckBox();
            this.buttonMouseEnter = new System.Windows.Forms.Button();
            this.checkBoxMouseRightUp = new System.Windows.Forms.CheckBox();
            this.checkBoxMouseRightDown = new System.Windows.Forms.CheckBox();
            this.buttonMouseRightUp = new System.Windows.Forms.Button();
            this.buttonMouseRightDown = new System.Windows.Forms.Button();
            this.checkBoxZoneDisplay = new System.Windows.Forms.CheckBox();
            this.buttonZoneDisplay = new System.Windows.Forms.Button();
            this.checkBoxMouseUp = new System.Windows.Forms.CheckBox();
            this.checkBoxMouseDown = new System.Windows.Forms.CheckBox();
            this.buttonMouseUp = new System.Windows.Forms.Button();
            this.buttonMouseDown = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkBoxSliderVert = new System.Windows.Forms.CheckBox();
            this.checkBoxSliderHorz = new System.Windows.Forms.CheckBox();
            this.buttonSliderVert = new System.Windows.Forms.Button();
            this.buttonSliderHorz = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxColorBack = new System.Windows.Forms.CheckBox();
            this.buttonColorBack = new System.Windows.Forms.Button();
            this.checkBoxColorText = new System.Windows.Forms.CheckBox();
            this.buttonColorText = new System.Windows.Forms.Button();
            this.checkBoxColorFill = new System.Windows.Forms.CheckBox();
            this.checkBoxColorLine = new System.Windows.Forms.CheckBox();
            this.buttonColorFill = new System.Windows.Forms.Button();
            this.buttonColorLine = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.checkBoxThickLine = new System.Windows.Forms.CheckBox();
            this.buttonThickLine = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxUse
            // 
            this.checkBoxUse.AccessibleDescription = null;
            this.checkBoxUse.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUse, "checkBoxUse");
            this.checkBoxUse.BackgroundImage = null;
            this.checkBoxUse.Font = null;
            this.checkBoxUse.Name = "checkBoxUse";
            this.checkBoxUse.CheckedChanged += new System.EventHandler(this.checkBoxUse_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.checkBoxSizeHeight);
            this.groupBox1.Controls.Add(this.checkBoxSizeWidth);
            this.groupBox1.Controls.Add(this.buttonSizeHeight);
            this.groupBox1.Controls.Add(this.buttonSizeWidth);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxSizeHeight
            // 
            this.checkBoxSizeHeight.AccessibleDescription = null;
            this.checkBoxSizeHeight.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSizeHeight, "checkBoxSizeHeight");
            this.checkBoxSizeHeight.BackgroundImage = null;
            this.checkBoxSizeHeight.Font = null;
            this.checkBoxSizeHeight.Name = "checkBoxSizeHeight";
            // 
            // checkBoxSizeWidth
            // 
            this.checkBoxSizeWidth.AccessibleDescription = null;
            this.checkBoxSizeWidth.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSizeWidth, "checkBoxSizeWidth");
            this.checkBoxSizeWidth.BackgroundImage = null;
            this.checkBoxSizeWidth.Font = null;
            this.checkBoxSizeWidth.Name = "checkBoxSizeWidth";
            // 
            // buttonSizeHeight
            // 
            this.buttonSizeHeight.AccessibleDescription = null;
            this.buttonSizeHeight.AccessibleName = null;
            resources.ApplyResources(this.buttonSizeHeight, "buttonSizeHeight");
            this.buttonSizeHeight.BackgroundImage = null;
            this.buttonSizeHeight.Font = null;
            this.buttonSizeHeight.Name = "buttonSizeHeight";
            this.buttonSizeHeight.Click += new System.EventHandler(this.buttonSizeHeight_Click);
            // 
            // buttonSizeWidth
            // 
            this.buttonSizeWidth.AccessibleDescription = null;
            this.buttonSizeWidth.AccessibleName = null;
            resources.ApplyResources(this.buttonSizeWidth, "buttonSizeWidth");
            this.buttonSizeWidth.BackgroundImage = null;
            this.buttonSizeWidth.Font = null;
            this.buttonSizeWidth.Name = "buttonSizeWidth";
            this.buttonSizeWidth.Click += new System.EventHandler(this.buttonSizeWidth_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxLocationY);
            this.groupBox2.Controls.Add(this.checkBoxLocationX);
            this.groupBox2.Controls.Add(this.buttonLocationY);
            this.groupBox2.Controls.Add(this.buttonLocationX);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxLocationY
            // 
            this.checkBoxLocationY.AccessibleDescription = null;
            this.checkBoxLocationY.AccessibleName = null;
            resources.ApplyResources(this.checkBoxLocationY, "checkBoxLocationY");
            this.checkBoxLocationY.BackgroundImage = null;
            this.checkBoxLocationY.Font = null;
            this.checkBoxLocationY.Name = "checkBoxLocationY";
            // 
            // checkBoxLocationX
            // 
            this.checkBoxLocationX.AccessibleDescription = null;
            this.checkBoxLocationX.AccessibleName = null;
            resources.ApplyResources(this.checkBoxLocationX, "checkBoxLocationX");
            this.checkBoxLocationX.BackgroundImage = null;
            this.checkBoxLocationX.Font = null;
            this.checkBoxLocationX.Name = "checkBoxLocationX";
            // 
            // buttonLocationY
            // 
            this.buttonLocationY.AccessibleDescription = null;
            this.buttonLocationY.AccessibleName = null;
            resources.ApplyResources(this.buttonLocationY, "buttonLocationY");
            this.buttonLocationY.BackgroundImage = null;
            this.buttonLocationY.Font = null;
            this.buttonLocationY.Name = "buttonLocationY";
            this.buttonLocationY.Click += new System.EventHandler(this.buttonLocationY_Click);
            // 
            // buttonLocationX
            // 
            this.buttonLocationX.AccessibleDescription = null;
            this.buttonLocationX.AccessibleName = null;
            resources.ApplyResources(this.buttonLocationX, "buttonLocationX");
            this.buttonLocationX.BackgroundImage = null;
            this.buttonLocationX.Font = null;
            this.buttonLocationX.Name = "buttonLocationX";
            this.buttonLocationX.Click += new System.EventHandler(this.buttonLocationX_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.checkBoxEventSelChange);
            this.groupBox3.Controls.Add(this.checkBoxEventKeyDown);
            this.groupBox3.Controls.Add(this.buttonEventSelChange);
            this.groupBox3.Controls.Add(this.buttonEventKeyDown);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // checkBoxEventSelChange
            // 
            this.checkBoxEventSelChange.AccessibleDescription = null;
            this.checkBoxEventSelChange.AccessibleName = null;
            resources.ApplyResources(this.checkBoxEventSelChange, "checkBoxEventSelChange");
            this.checkBoxEventSelChange.BackgroundImage = null;
            this.checkBoxEventSelChange.Font = null;
            this.checkBoxEventSelChange.Name = "checkBoxEventSelChange";
            // 
            // checkBoxEventKeyDown
            // 
            this.checkBoxEventKeyDown.AccessibleDescription = null;
            this.checkBoxEventKeyDown.AccessibleName = null;
            resources.ApplyResources(this.checkBoxEventKeyDown, "checkBoxEventKeyDown");
            this.checkBoxEventKeyDown.BackgroundImage = null;
            this.checkBoxEventKeyDown.Font = null;
            this.checkBoxEventKeyDown.Name = "checkBoxEventKeyDown";
            // 
            // buttonEventSelChange
            // 
            this.buttonEventSelChange.AccessibleDescription = null;
            this.buttonEventSelChange.AccessibleName = null;
            resources.ApplyResources(this.buttonEventSelChange, "buttonEventSelChange");
            this.buttonEventSelChange.BackgroundImage = null;
            this.buttonEventSelChange.Font = null;
            this.buttonEventSelChange.Name = "buttonEventSelChange";
            this.buttonEventSelChange.Click += new System.EventHandler(this.buttonEventSelChange_Click);
            // 
            // buttonEventKeyDown
            // 
            this.buttonEventKeyDown.AccessibleDescription = null;
            this.buttonEventKeyDown.AccessibleName = null;
            resources.ApplyResources(this.buttonEventKeyDown, "buttonEventKeyDown");
            this.buttonEventKeyDown.BackgroundImage = null;
            this.buttonEventKeyDown.Font = null;
            this.buttonEventKeyDown.Name = "buttonEventKeyDown";
            this.buttonEventKeyDown.Click += new System.EventHandler(this.buttonEventKeyDown_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.checkBoxRotate);
            this.groupBox4.Controls.Add(this.buttonRotate);
            this.groupBox4.Controls.Add(this.checkBoxBlinking);
            this.groupBox4.Controls.Add(this.checkBoxVisible);
            this.groupBox4.Controls.Add(this.buttonBlinking);
            this.groupBox4.Controls.Add(this.buttonVisible);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // checkBoxRotate
            // 
            this.checkBoxRotate.AccessibleDescription = null;
            this.checkBoxRotate.AccessibleName = null;
            resources.ApplyResources(this.checkBoxRotate, "checkBoxRotate");
            this.checkBoxRotate.BackgroundImage = null;
            this.checkBoxRotate.Font = null;
            this.checkBoxRotate.Name = "checkBoxRotate";
            this.checkBoxRotate.CheckedChanged += new System.EventHandler(this.checkBoxRotate_CheckedChanged);
            // 
            // buttonRotate
            // 
            this.buttonRotate.AccessibleDescription = null;
            this.buttonRotate.AccessibleName = null;
            resources.ApplyResources(this.buttonRotate, "buttonRotate");
            this.buttonRotate.BackgroundImage = null;
            this.buttonRotate.Font = null;
            this.buttonRotate.Name = "buttonRotate";
            this.buttonRotate.Click += new System.EventHandler(this.buttonRotate_Click);
            // 
            // checkBoxBlinking
            // 
            this.checkBoxBlinking.AccessibleDescription = null;
            this.checkBoxBlinking.AccessibleName = null;
            resources.ApplyResources(this.checkBoxBlinking, "checkBoxBlinking");
            this.checkBoxBlinking.BackgroundImage = null;
            this.checkBoxBlinking.Font = null;
            this.checkBoxBlinking.Name = "checkBoxBlinking";
            // 
            // checkBoxVisible
            // 
            this.checkBoxVisible.AccessibleDescription = null;
            this.checkBoxVisible.AccessibleName = null;
            resources.ApplyResources(this.checkBoxVisible, "checkBoxVisible");
            this.checkBoxVisible.BackgroundImage = null;
            this.checkBoxVisible.Font = null;
            this.checkBoxVisible.Name = "checkBoxVisible";
            // 
            // buttonBlinking
            // 
            this.buttonBlinking.AccessibleDescription = null;
            this.buttonBlinking.AccessibleName = null;
            resources.ApplyResources(this.buttonBlinking, "buttonBlinking");
            this.buttonBlinking.BackgroundImage = null;
            this.buttonBlinking.Font = null;
            this.buttonBlinking.Name = "buttonBlinking";
            this.buttonBlinking.Click += new System.EventHandler(this.buttonBlinking_Click);
            // 
            // buttonVisible
            // 
            this.buttonVisible.AccessibleDescription = null;
            this.buttonVisible.AccessibleName = null;
            resources.ApplyResources(this.buttonVisible, "buttonVisible");
            this.buttonVisible.BackgroundImage = null;
            this.buttonVisible.Font = null;
            this.buttonVisible.Name = "buttonVisible";
            this.buttonVisible.Click += new System.EventHandler(this.buttonVisible_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = null;
            this.groupBox5.AccessibleName = null;
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.BackgroundImage = null;
            this.groupBox5.Controls.Add(this.checkBoxAnimationSpeed);
            this.groupBox5.Controls.Add(this.buttonAnimationSpeed);
            this.groupBox5.Font = null;
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // checkBoxAnimationSpeed
            // 
            this.checkBoxAnimationSpeed.AccessibleDescription = null;
            this.checkBoxAnimationSpeed.AccessibleName = null;
            resources.ApplyResources(this.checkBoxAnimationSpeed, "checkBoxAnimationSpeed");
            this.checkBoxAnimationSpeed.BackgroundImage = null;
            this.checkBoxAnimationSpeed.Font = null;
            this.checkBoxAnimationSpeed.Name = "checkBoxAnimationSpeed";
            // 
            // buttonAnimationSpeed
            // 
            this.buttonAnimationSpeed.AccessibleDescription = null;
            this.buttonAnimationSpeed.AccessibleName = null;
            resources.ApplyResources(this.buttonAnimationSpeed, "buttonAnimationSpeed");
            this.buttonAnimationSpeed.BackgroundImage = null;
            this.buttonAnimationSpeed.Font = null;
            this.buttonAnimationSpeed.Name = "buttonAnimationSpeed";
            this.buttonAnimationSpeed.Click += new System.EventHandler(this.buttonAnimationSpeed_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.checkBoxMouseMove);
            this.groupBox6.Controls.Add(this.buttonMouseMove);
            this.groupBox6.Controls.Add(this.checkBoxMouseLeave);
            this.groupBox6.Controls.Add(this.buttonMouseLeave);
            this.groupBox6.Controls.Add(this.checkBoxMouseEnter);
            this.groupBox6.Controls.Add(this.buttonMouseEnter);
            this.groupBox6.Controls.Add(this.checkBoxMouseRightUp);
            this.groupBox6.Controls.Add(this.checkBoxMouseRightDown);
            this.groupBox6.Controls.Add(this.buttonMouseRightUp);
            this.groupBox6.Controls.Add(this.buttonMouseRightDown);
            this.groupBox6.Controls.Add(this.checkBoxZoneDisplay);
            this.groupBox6.Controls.Add(this.buttonZoneDisplay);
            this.groupBox6.Controls.Add(this.checkBoxMouseUp);
            this.groupBox6.Controls.Add(this.checkBoxMouseDown);
            this.groupBox6.Controls.Add(this.buttonMouseUp);
            this.groupBox6.Controls.Add(this.buttonMouseDown);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // checkBoxMouseMove
            // 
            this.checkBoxMouseMove.AccessibleDescription = null;
            this.checkBoxMouseMove.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseMove, "checkBoxMouseMove");
            this.checkBoxMouseMove.BackgroundImage = null;
            this.checkBoxMouseMove.Font = null;
            this.checkBoxMouseMove.Name = "checkBoxMouseMove";
            // 
            // buttonMouseMove
            // 
            this.buttonMouseMove.AccessibleDescription = null;
            this.buttonMouseMove.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseMove, "buttonMouseMove");
            this.buttonMouseMove.BackgroundImage = null;
            this.buttonMouseMove.Font = null;
            this.buttonMouseMove.Name = "buttonMouseMove";
            this.buttonMouseMove.Click += new System.EventHandler(this.buttonMouseMove_Click);
            // 
            // checkBoxMouseLeave
            // 
            this.checkBoxMouseLeave.AccessibleDescription = null;
            this.checkBoxMouseLeave.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseLeave, "checkBoxMouseLeave");
            this.checkBoxMouseLeave.BackgroundImage = null;
            this.checkBoxMouseLeave.Font = null;
            this.checkBoxMouseLeave.Name = "checkBoxMouseLeave";
            // 
            // buttonMouseLeave
            // 
            this.buttonMouseLeave.AccessibleDescription = null;
            this.buttonMouseLeave.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseLeave, "buttonMouseLeave");
            this.buttonMouseLeave.BackgroundImage = null;
            this.buttonMouseLeave.Font = null;
            this.buttonMouseLeave.Name = "buttonMouseLeave";
            this.buttonMouseLeave.Click += new System.EventHandler(this.buttonMouseLeave_Click);
            // 
            // checkBoxMouseEnter
            // 
            this.checkBoxMouseEnter.AccessibleDescription = null;
            this.checkBoxMouseEnter.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseEnter, "checkBoxMouseEnter");
            this.checkBoxMouseEnter.BackgroundImage = null;
            this.checkBoxMouseEnter.Font = null;
            this.checkBoxMouseEnter.Name = "checkBoxMouseEnter";
            // 
            // buttonMouseEnter
            // 
            this.buttonMouseEnter.AccessibleDescription = null;
            this.buttonMouseEnter.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseEnter, "buttonMouseEnter");
            this.buttonMouseEnter.BackgroundImage = null;
            this.buttonMouseEnter.Font = null;
            this.buttonMouseEnter.Name = "buttonMouseEnter";
            this.buttonMouseEnter.Click += new System.EventHandler(this.buttonMouseEnter_Click);
            // 
            // checkBoxMouseRightUp
            // 
            this.checkBoxMouseRightUp.AccessibleDescription = null;
            this.checkBoxMouseRightUp.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseRightUp, "checkBoxMouseRightUp");
            this.checkBoxMouseRightUp.BackgroundImage = null;
            this.checkBoxMouseRightUp.Font = null;
            this.checkBoxMouseRightUp.Name = "checkBoxMouseRightUp";
            // 
            // checkBoxMouseRightDown
            // 
            this.checkBoxMouseRightDown.AccessibleDescription = null;
            this.checkBoxMouseRightDown.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseRightDown, "checkBoxMouseRightDown");
            this.checkBoxMouseRightDown.BackgroundImage = null;
            this.checkBoxMouseRightDown.Font = null;
            this.checkBoxMouseRightDown.Name = "checkBoxMouseRightDown";
            // 
            // buttonMouseRightUp
            // 
            this.buttonMouseRightUp.AccessibleDescription = null;
            this.buttonMouseRightUp.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseRightUp, "buttonMouseRightUp");
            this.buttonMouseRightUp.BackgroundImage = null;
            this.buttonMouseRightUp.Font = null;
            this.buttonMouseRightUp.Name = "buttonMouseRightUp";
            this.buttonMouseRightUp.Click += new System.EventHandler(this.buttonMouseRightUp_Click);
            // 
            // buttonMouseRightDown
            // 
            this.buttonMouseRightDown.AccessibleDescription = null;
            this.buttonMouseRightDown.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseRightDown, "buttonMouseRightDown");
            this.buttonMouseRightDown.BackgroundImage = null;
            this.buttonMouseRightDown.Font = null;
            this.buttonMouseRightDown.Name = "buttonMouseRightDown";
            this.buttonMouseRightDown.Click += new System.EventHandler(this.buttonMouseRightDown_Click);
            // 
            // checkBoxZoneDisplay
            // 
            this.checkBoxZoneDisplay.AccessibleDescription = null;
            this.checkBoxZoneDisplay.AccessibleName = null;
            resources.ApplyResources(this.checkBoxZoneDisplay, "checkBoxZoneDisplay");
            this.checkBoxZoneDisplay.BackgroundImage = null;
            this.checkBoxZoneDisplay.Font = null;
            this.checkBoxZoneDisplay.Name = "checkBoxZoneDisplay";
            // 
            // buttonZoneDisplay
            // 
            this.buttonZoneDisplay.AccessibleDescription = null;
            this.buttonZoneDisplay.AccessibleName = null;
            resources.ApplyResources(this.buttonZoneDisplay, "buttonZoneDisplay");
            this.buttonZoneDisplay.BackgroundImage = null;
            this.buttonZoneDisplay.Font = null;
            this.buttonZoneDisplay.Name = "buttonZoneDisplay";
            this.buttonZoneDisplay.Click += new System.EventHandler(this.buttonZoneDisplay_Click);
            // 
            // checkBoxMouseUp
            // 
            this.checkBoxMouseUp.AccessibleDescription = null;
            this.checkBoxMouseUp.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseUp, "checkBoxMouseUp");
            this.checkBoxMouseUp.BackgroundImage = null;
            this.checkBoxMouseUp.Font = null;
            this.checkBoxMouseUp.Name = "checkBoxMouseUp";
            // 
            // checkBoxMouseDown
            // 
            this.checkBoxMouseDown.AccessibleDescription = null;
            this.checkBoxMouseDown.AccessibleName = null;
            resources.ApplyResources(this.checkBoxMouseDown, "checkBoxMouseDown");
            this.checkBoxMouseDown.BackgroundImage = null;
            this.checkBoxMouseDown.Font = null;
            this.checkBoxMouseDown.Name = "checkBoxMouseDown";
            // 
            // buttonMouseUp
            // 
            this.buttonMouseUp.AccessibleDescription = null;
            this.buttonMouseUp.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseUp, "buttonMouseUp");
            this.buttonMouseUp.BackgroundImage = null;
            this.buttonMouseUp.Font = null;
            this.buttonMouseUp.Name = "buttonMouseUp";
            this.buttonMouseUp.Click += new System.EventHandler(this.buttonMouseUp_Click);
            // 
            // buttonMouseDown
            // 
            this.buttonMouseDown.AccessibleDescription = null;
            this.buttonMouseDown.AccessibleName = null;
            resources.ApplyResources(this.buttonMouseDown, "buttonMouseDown");
            this.buttonMouseDown.BackgroundImage = null;
            this.buttonMouseDown.Font = null;
            this.buttonMouseDown.Name = "buttonMouseDown";
            this.buttonMouseDown.Click += new System.EventHandler(this.buttonMouseDown_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.AccessibleDescription = null;
            this.groupBox7.AccessibleName = null;
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.BackgroundImage = null;
            this.groupBox7.Controls.Add(this.checkBoxSliderVert);
            this.groupBox7.Controls.Add(this.checkBoxSliderHorz);
            this.groupBox7.Controls.Add(this.buttonSliderVert);
            this.groupBox7.Controls.Add(this.buttonSliderHorz);
            this.groupBox7.Font = null;
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // checkBoxSliderVert
            // 
            this.checkBoxSliderVert.AccessibleDescription = null;
            this.checkBoxSliderVert.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSliderVert, "checkBoxSliderVert");
            this.checkBoxSliderVert.BackgroundImage = null;
            this.checkBoxSliderVert.Font = null;
            this.checkBoxSliderVert.Name = "checkBoxSliderVert";
            // 
            // checkBoxSliderHorz
            // 
            this.checkBoxSliderHorz.AccessibleDescription = null;
            this.checkBoxSliderHorz.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSliderHorz, "checkBoxSliderHorz");
            this.checkBoxSliderHorz.BackgroundImage = null;
            this.checkBoxSliderHorz.Font = null;
            this.checkBoxSliderHorz.Name = "checkBoxSliderHorz";
            // 
            // buttonSliderVert
            // 
            this.buttonSliderVert.AccessibleDescription = null;
            this.buttonSliderVert.AccessibleName = null;
            resources.ApplyResources(this.buttonSliderVert, "buttonSliderVert");
            this.buttonSliderVert.BackgroundImage = null;
            this.buttonSliderVert.Font = null;
            this.buttonSliderVert.Name = "buttonSliderVert";
            this.buttonSliderVert.Click += new System.EventHandler(this.buttonSliderVert_Click);
            // 
            // buttonSliderHorz
            // 
            this.buttonSliderHorz.AccessibleDescription = null;
            this.buttonSliderHorz.AccessibleName = null;
            resources.ApplyResources(this.buttonSliderHorz, "buttonSliderHorz");
            this.buttonSliderHorz.BackgroundImage = null;
            this.buttonSliderHorz.Font = null;
            this.buttonSliderHorz.Name = "buttonSliderHorz";
            this.buttonSliderHorz.Click += new System.EventHandler(this.buttonSliderHorz_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.AccessibleDescription = null;
            this.groupBox8.AccessibleName = null;
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.BackgroundImage = null;
            this.groupBox8.Controls.Add(this.checkBoxColorBack);
            this.groupBox8.Controls.Add(this.buttonColorBack);
            this.groupBox8.Controls.Add(this.checkBoxColorText);
            this.groupBox8.Controls.Add(this.buttonColorText);
            this.groupBox8.Controls.Add(this.checkBoxColorFill);
            this.groupBox8.Controls.Add(this.checkBoxColorLine);
            this.groupBox8.Controls.Add(this.buttonColorFill);
            this.groupBox8.Controls.Add(this.buttonColorLine);
            this.groupBox8.Font = null;
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // checkBoxColorBack
            // 
            this.checkBoxColorBack.AccessibleDescription = null;
            this.checkBoxColorBack.AccessibleName = null;
            resources.ApplyResources(this.checkBoxColorBack, "checkBoxColorBack");
            this.checkBoxColorBack.BackgroundImage = null;
            this.checkBoxColorBack.Font = null;
            this.checkBoxColorBack.Name = "checkBoxColorBack";
            // 
            // buttonColorBack
            // 
            this.buttonColorBack.AccessibleDescription = null;
            this.buttonColorBack.AccessibleName = null;
            resources.ApplyResources(this.buttonColorBack, "buttonColorBack");
            this.buttonColorBack.BackgroundImage = null;
            this.buttonColorBack.Font = null;
            this.buttonColorBack.Name = "buttonColorBack";
            this.buttonColorBack.Click += new System.EventHandler(this.buttonColorBack_Click);
            // 
            // checkBoxColorText
            // 
            this.checkBoxColorText.AccessibleDescription = null;
            this.checkBoxColorText.AccessibleName = null;
            resources.ApplyResources(this.checkBoxColorText, "checkBoxColorText");
            this.checkBoxColorText.BackgroundImage = null;
            this.checkBoxColorText.Font = null;
            this.checkBoxColorText.Name = "checkBoxColorText";
            // 
            // buttonColorText
            // 
            this.buttonColorText.AccessibleDescription = null;
            this.buttonColorText.AccessibleName = null;
            resources.ApplyResources(this.buttonColorText, "buttonColorText");
            this.buttonColorText.BackgroundImage = null;
            this.buttonColorText.Font = null;
            this.buttonColorText.Name = "buttonColorText";
            this.buttonColorText.Click += new System.EventHandler(this.buttonColorText_Click);
            // 
            // checkBoxColorFill
            // 
            this.checkBoxColorFill.AccessibleDescription = null;
            this.checkBoxColorFill.AccessibleName = null;
            resources.ApplyResources(this.checkBoxColorFill, "checkBoxColorFill");
            this.checkBoxColorFill.BackgroundImage = null;
            this.checkBoxColorFill.Font = null;
            this.checkBoxColorFill.Name = "checkBoxColorFill";
            // 
            // checkBoxColorLine
            // 
            this.checkBoxColorLine.AccessibleDescription = null;
            this.checkBoxColorLine.AccessibleName = null;
            resources.ApplyResources(this.checkBoxColorLine, "checkBoxColorLine");
            this.checkBoxColorLine.BackgroundImage = null;
            this.checkBoxColorLine.Font = null;
            this.checkBoxColorLine.Name = "checkBoxColorLine";
            // 
            // buttonColorFill
            // 
            this.buttonColorFill.AccessibleDescription = null;
            this.buttonColorFill.AccessibleName = null;
            resources.ApplyResources(this.buttonColorFill, "buttonColorFill");
            this.buttonColorFill.BackgroundImage = null;
            this.buttonColorFill.Font = null;
            this.buttonColorFill.Name = "buttonColorFill";
            this.buttonColorFill.Click += new System.EventHandler(this.buttonColorFill_Click);
            // 
            // buttonColorLine
            // 
            this.buttonColorLine.AccessibleDescription = null;
            this.buttonColorLine.AccessibleName = null;
            resources.ApplyResources(this.buttonColorLine, "buttonColorLine");
            this.buttonColorLine.BackgroundImage = null;
            this.buttonColorLine.Font = null;
            this.buttonColorLine.Name = "buttonColorLine";
            this.buttonColorLine.Click += new System.EventHandler(this.buttonColorLine_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.AccessibleDescription = null;
            this.groupBox9.AccessibleName = null;
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.BackgroundImage = null;
            this.groupBox9.Controls.Add(this.checkBoxThickLine);
            this.groupBox9.Controls.Add(this.buttonThickLine);
            this.groupBox9.Font = null;
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // checkBoxThickLine
            // 
            this.checkBoxThickLine.AccessibleDescription = null;
            this.checkBoxThickLine.AccessibleName = null;
            resources.ApplyResources(this.checkBoxThickLine, "checkBoxThickLine");
            this.checkBoxThickLine.BackgroundImage = null;
            this.checkBoxThickLine.Font = null;
            this.checkBoxThickLine.Name = "checkBoxThickLine";
            // 
            // buttonThickLine
            // 
            this.buttonThickLine.AccessibleDescription = null;
            this.buttonThickLine.AccessibleName = null;
            resources.ApplyResources(this.buttonThickLine, "buttonThickLine");
            this.buttonThickLine.BackgroundImage = null;
            this.buttonThickLine.Font = null;
            this.buttonThickLine.Name = "buttonThickLine";
            this.buttonThickLine.Click += new System.EventHandler(this.buttonThickLine_Click);
            // 
            // PropertyPageExpandOption
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBoxUse);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox9);
            this.Icon = null;
            this.Name = "PropertyPageExpandOption";
            this.Load += new System.EventHandler(this.PropertyPageExpandOption_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageExpandOption_Load(object sender, System.EventArgs e)
		{
			EnableAll();
		}

		void Enable(bool condition, CheckBox check, Button button)
		{
			if(!checkBoxUse.Checked) 
			{
				condition = false;
			}
			
			check.Enabled = condition;
			button.Enabled = condition;
		}

		void EnableAll()
		{
			Enable(bUseSizeWidth, checkBoxSizeWidth, buttonSizeWidth);
			Enable(bUseSizeHeight, checkBoxSizeHeight, buttonSizeHeight);

			Enable(bUseLocationX, checkBoxLocationX, buttonLocationX);
			Enable(bUseLocationY, checkBoxLocationY, buttonLocationY);
			Enable(bUseEventKeyDown, checkBoxEventKeyDown, buttonEventKeyDown);
			Enable(bUseEventSelChange, checkBoxEventSelChange, buttonEventSelChange);

			Enable(bUseMouseLeftDown, checkBoxMouseDown, buttonMouseDown);
			Enable(bUseMouseLeftUp, checkBoxMouseUp, buttonMouseUp);
            Enable(bUseMouseRightDown, checkBoxMouseRightDown, buttonMouseRightDown);
            Enable(bUseMouseRightUp, checkBoxMouseRightUp, buttonMouseRightUp);
            Enable(bUseMouseEnter, checkBoxMouseEnter, buttonMouseEnter);
            Enable(bUseMouseLeave, checkBoxMouseLeave, buttonMouseLeave);
            Enable(bUseMouseMove, checkBoxMouseMove, buttonMouseMove);

			Enable(bUseZoneDisplay, checkBoxZoneDisplay, buttonZoneDisplay);
			Enable(bUseVisible, checkBoxVisible, buttonVisible);
			Enable(bUseBlinking, checkBoxBlinking, buttonBlinking);
			Enable(bUseAnimationSpeed, checkBoxAnimationSpeed, buttonAnimationSpeed);
			Enable(bUseSliderHorz, checkBoxSliderHorz, buttonSliderHorz);
			Enable(bUseSliderVert, checkBoxSliderVert, buttonSliderVert);
			Enable(bUseColorLine, checkBoxColorLine, buttonColorLine);
			Enable(bUseColorFill, checkBoxColorFill, buttonColorFill);
			Enable(bUseColorText, checkBoxColorText, buttonColorText);
			Enable(bUseColorBack, checkBoxColorBack, buttonColorBack);
			Enable(bUseThickLine, checkBoxThickLine, buttonThickLine);

            // 회전은 CE버전에서는 사용하지 않는다.
            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                Enable(false, checkBoxRotate, buttonRotate);
            }
            else
            {
                Enable(bUseRotation, checkBoxRotate, buttonRotate);
            }
		}

		private void checkBoxUse_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableAll();
		}

		ScriptClass EditScript(string title, ScriptClass script, CheckBox checkbox, string default_description)
		{
            FormScriptEditor dialog = new FormScriptEditor();

			if(script != null) 
			{
				dialog.SetScript(script);
			}
			else 
			{
				dialog.SetDescription(default_description);
			}

			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				if(script == null)
					script = new ScriptClass();

				script = dialog.GetScript();	

				checkbox.Checked = true;
			}

			return script;	// script가 새로 할당될 수 있으므로 다시 돌려준다.
		}

        ExpandBasicAndScript EditPropertyAndScript(string title, ExpandBasicAndScript expand, CheckBox checkbox, string default_description, Form formProperty)
        {
            FormPropertyAndScript dialog = new FormPropertyAndScript(expand, default_description, formProperty);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                checkbox.Checked = true;

                return dialog.expandBasicAndScript;
            }

            return expand;	// script가 새로 할당될 수 있으므로 다시 돌려준다.

            /*
            FormScriptEditor dialog = new FormScriptEditor();

            if (script != null)
            {
                dialog.SetScript(script);
            }
            else
            {
                dialog.SetDescription(default_description);
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (script == null)
                    script = new ScriptClass();

                script = dialog.GetScript();

                checkbox.Checked = true;
            }

            return script;	// script가 새로 할당될 수 있으므로 다시 돌려준다.
            */
        }

        

		void NumericUpDownSetValue(NumericUpDown control, Decimal value)
		{
			if(value < control.Minimum)	value = control.Minimum;
			if(value > control.Maximum)	value = control.Maximum;

			control.Value = value;
		}

		EXPAND_MOUSE_ZONE EditScript(EXPAND_MOUSE_ZONE script, CheckBox checkbox)
		{
			FormExpandOptionMouseZone dialog = new FormExpandOptionMouseZone();
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(script != null) 
			{
				dialog.checkBoxLock.Checked = (script.bLock == 1);
				NumericUpDownSetValue(dialog.numericUpDownX1, script.x1);
				NumericUpDownSetValue(dialog.numericUpDownY1, script.y1);
				NumericUpDownSetValue(dialog.numericUpDownX2, script.x2);
				NumericUpDownSetValue(dialog.numericUpDownY2, script.y2);
			}

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				if(script == null) 
					script = new EXPAND_MOUSE_ZONE();

				script.bLock = dialog.checkBoxLock.Checked ? (sbyte)1 : (sbyte)0;
				script.x1 = ConvertTool.ToInt32(dialog.numericUpDownX1.Value);
				script.y1 = ConvertTool.ToInt32(dialog.numericUpDownY1.Value);
				script.x2 = ConvertTool.ToInt32(dialog.numericUpDownX2.Value);
				script.y2 = ConvertTool.ToInt32(dialog.numericUpDownY2.Value);

				checkbox.Checked = true;
			}

			return script;	// script가 새로 할당될수 있으므로 다시 돌려준다.
		}

		EXPAND_SLIDER_STRUCT EditScriptSliderHorz(EXPAND_SLIDER_STRUCT script, CheckBox checkbox)
		{
			FormExpandOptionSliderHorz dialog = new FormExpandOptionSliderHorz();

			dialog.SetScript(script);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				script = dialog.GetScript();
				
				checkbox.Checked = true;
			}

			return script;	// script가 새로 할당될수 있으므로 다시 돌려준다.
		}

		EXPAND_SLIDER_STRUCT EditScriptSliderVert(EXPAND_SLIDER_STRUCT script, CheckBox checkbox)
		{
			FormExpandOptionSliderVert dialog = new FormExpandOptionSliderVert();

			dialog.SetScript(script);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				script = dialog.GetScript();
				
				checkbox.Checked = true;
			}

			return script;	// script가 새로 할당될수 있으므로 다시 돌려준다.
		}

		private void buttonSizeWidth_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "가로 크기의 %값을 return";
			else
				description = "return percent(%) value of width";

            FormExpandOptionSizeWidth width = new FormExpandOptionSizeWidth();
			
			objExpand.expandScript.scriptSizeWidth = EditPropertyAndScript("SizeWidth", objExpand.expandScript.scriptSizeWidth, checkBoxSizeWidth, description, width);
		}

		private void buttonSizeHeight_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "세로 크기의 %값을 return";
			else
				description = "return percent(%) value of height";

            FormExpandOptionSizeHeight height = new FormExpandOptionSizeHeight();

            objExpand.expandScript.scriptSizeHeight = EditPropertyAndScript("SizeHeight", objExpand.expandScript.scriptSizeHeight, checkBoxSizeHeight, description, height);
		}

		private void buttonLocationX_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "가로의 위치값을 return";
			else
				description = "return location(x) value of horizontal";

            FormExpandOptionLocationX locx = new FormExpandOptionLocationX();

            objExpand.expandScript.scriptLocationX = EditPropertyAndScript("LocationX", objExpand.expandScript.scriptLocationX, checkBoxLocationX, description, locx);
		}

		private void buttonLocationY_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "세로의 위치값을 return";
			else
				description = "return location(y) value of vertical";

            FormExpandOptionLocationY locy = new FormExpandOptionLocationY();

            objExpand.expandScript.scriptLocationY = EditPropertyAndScript("LocationY", objExpand.expandScript.scriptLocationY, checkBoxLocationY, description, locy);
		}

		private void buttonEventKeyDown_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "키가 눌러졌을 때 사용할 스크립트";
			else
				description = "Script on Key Down"; 


			objExpand.expandScript.scriptEventKeyDown = EditScript("EventKeyDown", objExpand.expandScript.scriptEventKeyDown, checkBoxEventKeyDown, description);					
		}

		private void buttonEventSelChange_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "선택 항목이 바뀌었을 때 사용할 스크립트";
			else
				description = "Script on Item Sel Changed"; 

			objExpand.expandScript.scriptEventSelChange = EditScript("EventSelChange", objExpand.expandScript.scriptEventSelChange, checkBoxEventSelChange, description);
		}

		private void buttonMouseDown_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "마우스를 눌렀을 때 실행할 Script";
			else
				description = "Script when user down mouse button";

            objExpand.expandScript.scriptMouseLeftDown = EditScript("MouseDown", objExpand.expandScript.scriptMouseLeftDown, checkBoxMouseDown, description);
		}

		private void buttonMouseUp_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "마우스를 떼었을 때 실행할 Script";
			else
				description = "Script when user up mouse button";

            objExpand.expandScript.scriptMouseLeftUp = EditScript("MouseUp", objExpand.expandScript.scriptMouseLeftUp, checkBoxMouseUp, description);		
		}

        private void buttonMouseRightDown_Click(object sender, EventArgs e)
        {
            string description;

            if (Tools.IsLangKorean())
                description = "오른쪽 마우스를 눌렀을 때 실행할 Script";
            else
                description = "Script when user down mouse right button";

            objExpand.expandScript.scriptMouseRightDown = EditScript("MouseRightDown", objExpand.expandScript.scriptMouseRightDown, checkBoxMouseRightDown, description);
        }

        private void buttonMouseRightUp_Click(object sender, EventArgs e)
        {
            string description;

            if (Tools.IsLangKorean())
                description = "오른쪽 마우스를 떼었을 때 실행할 Script";
            else
                description = "Script when user up mouse right button";

            objExpand.expandScript.scriptMouseRightUp = EditScript("MouseRightUp", objExpand.expandScript.scriptMouseRightUp, checkBoxMouseRightUp, description);
        }

        private void buttonMouseEnter_Click(object sender, EventArgs e)
        {
            string description;

            if (Tools.IsLangKorean())
                description = "마우스 Enter시 실행할 Script";
            else
                description = "Script when mouse enter";

            objExpand.expandScript.scriptMouseEnter = EditScript("MouseEnter", objExpand.expandScript.scriptMouseEnter, checkBoxMouseEnter, description);
        }

        private void buttonMouseLeave_Click(object sender, EventArgs e)
        {
            string description;

            if (Tools.IsLangKorean())
                description = "마우스 Leave시 실행할 Script";
            else
                description = "Script when mouse leave";

            objExpand.expandScript.scriptMouseLeave = EditScript("MouseLeave", objExpand.expandScript.scriptMouseLeave, checkBoxMouseLeave, description);
        }

        private void buttonMouseMove_Click(object sender, EventArgs e)
        {
            string description;

            if (Tools.IsLangKorean())
                description = "마우스 Move시 실행할 Script";
            else
                description = "Script when mouse move";

            objExpand.expandScript.scriptMouseMove = EditScript("MouseMove", objExpand.expandScript.scriptMouseMove, checkBoxMouseMove, description);
        }

		private void buttonZoneDisplay_Click(object sender, System.EventArgs e)
		{
			objExpand.expandScript.structMouseZone = EditScript(objExpand.expandScript.structMouseZone, checkBoxZoneDisplay);
		}

		private void buttonVisible_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "ON 또는 OFF 값을 return";
			else
				description = "return ON(1) or OFF(0)";

            FormExpandOptionVisible form = new FormExpandOptionVisible();

            objExpand.expandScript.scriptVisible = EditPropertyAndScript("Visible", objExpand.expandScript.scriptVisible, checkBoxVisible, description, form);
		}

		private void buttonBlinking_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "점멸 주기 값을 return (0 = 점멸 없음, 1000 = 1초마다 점멸)";
			else
				description = "return Blink cycle by milisec";

            FormExpandOptionBlinking form = new FormExpandOptionBlinking();

            objExpand.expandScript.scriptBlinking = EditPropertyAndScript("Blinking", objExpand.expandScript.scriptBlinking, checkBoxBlinking, description, form);		
		}

		private void buttonAnimationSpeed_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "애니매이션 의 RPM 값을 return";
			else
				description = "return RPM value of animation";

            FormExpandOptionAnimationSpeed form = new FormExpandOptionAnimationSpeed();

            objExpand.expandScript.scriptAnimationSpeed = EditPropertyAndScript("AnimationSpeed", objExpand.expandScript.scriptAnimationSpeed, checkBoxAnimationSpeed, description, form);
		}

		private void buttonSliderHorz_Click(object sender, System.EventArgs e)
		{
			objExpand.expandScript.sliderHorz = EditScriptSliderHorz(objExpand.expandScript.sliderHorz, this.checkBoxSliderHorz);
		}

		private void buttonSliderVert_Click(object sender, System.EventArgs e)
		{
			objExpand.expandScript.sliderVert = EditScriptSliderVert(objExpand.expandScript.sliderVert, this.checkBoxSliderVert);	
		}

		private void buttonColorLine_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "선 색상의 @RGB(r,g,b) 값을 return";
			else
				description = "return @RGB(r,g,b) value of line color";

            FormExpandOptionColor form = new FormExpandOptionColor();

            objExpand.expandScript.scriptColorLine = EditPropertyAndScript("ColorLine", objExpand.expandScript.scriptColorLine, checkBoxColorLine, description, form);
		}

		private void buttonColorFill_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "채움 색상의 @RGB(r,g,b) 값을 return";
			else
				description = "return @RGB(r,g,b) value of fill color";

            FormExpandOptionColor form = new FormExpandOptionColor();

            objExpand.expandScript.scriptColorFill = EditPropertyAndScript("ColorFill", objExpand.expandScript.scriptColorFill, checkBoxColorFill, description, form);
		}

		private void buttonColorText_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "글자 색상의 @RGB(r,g,b) 값을 return";
			else
				description = "return @RGB(r,g,b) value of text color";

            FormExpandOptionColor form = new FormExpandOptionColor();

            objExpand.expandScript.scriptColorText = EditPropertyAndScript("ColorText", objExpand.expandScript.scriptColorText, checkBoxColorText, description, form);
		}

		private void buttonColorBack_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "배경 색상의 @RGB(r,g,b) 값을 return";
			else
				description = "return @RGB(r,g,b) value of back color";

            FormExpandOptionColor form = new FormExpandOptionColor();

            objExpand.expandScript.scriptColorBack = EditPropertyAndScript("ColorBack", objExpand.expandScript.scriptColorBack, checkBoxColorBack, description, form);		
		}

		private void buttonThickLine_Click(object sender, System.EventArgs e)
		{
			string description;

			if(Tools.IsLangKorean())
				description = "선 굵기 값을 return (1이상)";
			else
				description = "return thick of line";

            FormExpandOptionLineThick form = new FormExpandOptionLineThick();

            objExpand.expandScript.scriptThickLine = EditPropertyAndScript("ThickLine", objExpand.expandScript.scriptThickLine, checkBoxThickLine, description, form);
		}

		//ScriptClass scriptSizeWidth;
		//ScriptClass scriptSizeHeight;

		public void ObjectToDialog(ObjectExpand obj)
		{
			checkBoxUse.Checked = obj.ExpandActive;

			checkBoxSizeWidth.Checked = (objExpand.expandScript.scriptSizeWidth != null);
			checkBoxSizeHeight.Checked = (objExpand.expandScript.scriptSizeHeight != null);

			checkBoxLocationX.Checked = (objExpand.expandScript.scriptLocationX != null);
			checkBoxLocationY.Checked = (objExpand.expandScript.scriptLocationY != null);
			checkBoxEventKeyDown.Checked = (objExpand.expandScript.scriptEventKeyDown != null);
			checkBoxEventSelChange.Checked = (objExpand.expandScript.scriptEventSelChange != null);
			checkBoxMouseDown.Checked = (objExpand.expandScript.scriptMouseLeftDown != null);
			checkBoxMouseUp.Checked = (objExpand.expandScript.scriptMouseLeftUp != null);
            checkBoxMouseRightDown.Checked = (objExpand.expandScript.scriptMouseRightDown != null);
            checkBoxMouseRightUp.Checked = (objExpand.expandScript.scriptMouseRightUp != null);
            checkBoxMouseEnter.Checked = (objExpand.expandScript.scriptMouseEnter != null);
            checkBoxMouseLeave.Checked = (objExpand.expandScript.scriptMouseLeave != null);
            checkBoxMouseMove.Checked = (objExpand.expandScript.scriptMouseMove != null);

			checkBoxZoneDisplay.Checked = (objExpand.expandScript.structMouseZone != null);
			checkBoxVisible.Checked = (objExpand.expandScript.scriptVisible != null);
			checkBoxBlinking.Checked = (objExpand.expandScript.scriptBlinking != null);
			checkBoxAnimationSpeed.Checked = (objExpand.expandScript.scriptAnimationSpeed != null);
			checkBoxSliderHorz.Checked = (objExpand.expandScript.sliderHorz != null);
			checkBoxSliderVert.Checked = (objExpand.expandScript.sliderVert != null);
			checkBoxColorLine.Checked = (objExpand.expandScript.scriptColorLine != null);
			checkBoxColorFill.Checked = (objExpand.expandScript.scriptColorFill != null);
			checkBoxColorText.Checked = (objExpand.expandScript.scriptColorText != null);
			checkBoxColorBack.Checked = (objExpand.expandScript.scriptColorBack != null);
			checkBoxThickLine.Checked = (objExpand.expandScript.scriptThickLine != null);
            checkBoxRotate.Checked = (objExpand.expandScript.scriptRotate != null);
		}

		public void DialogToObject(ObjectExpand obj)
		{
			obj.ExpandActive = checkBoxUse.Checked;

			if(!checkBoxSizeWidth.Checked)	objExpand.expandScript.scriptSizeWidth = null;
			if(!checkBoxSizeHeight.Checked) objExpand.expandScript.scriptSizeHeight = null;
			if(!checkBoxLocationX.Checked)	objExpand.expandScript.scriptLocationX = null;
			if(!checkBoxLocationY.Checked)	objExpand.expandScript.scriptLocationY = null;
			if(!checkBoxEventKeyDown.Checked) objExpand.expandScript.scriptEventKeyDown = null;
			if(!checkBoxEventSelChange.Checked) objExpand.expandScript.scriptEventSelChange = null;
			if(!checkBoxMouseDown.Checked)	objExpand.expandScript.scriptMouseLeftDown = null;
			if(!checkBoxMouseUp.Checked)	objExpand.expandScript.scriptMouseLeftUp = null;
            if (!checkBoxMouseRightDown.Checked) objExpand.expandScript.scriptMouseRightDown = null;
            if (!checkBoxMouseRightUp.Checked) objExpand.expandScript.scriptMouseRightUp = null;
            if (!checkBoxMouseEnter.Checked) objExpand.expandScript.scriptMouseEnter = null;
            if (!checkBoxMouseLeave.Checked) objExpand.expandScript.scriptMouseLeave = null;
            if (!checkBoxMouseMove.Checked) objExpand.expandScript.scriptMouseMove = null;

			if(!checkBoxZoneDisplay.Checked)objExpand.expandScript.structMouseZone = null;
			if(!checkBoxVisible.Checked)	objExpand.expandScript.scriptVisible = null;
			if(!checkBoxBlinking.Checked)	objExpand.expandScript.scriptBlinking = null;
			if(!checkBoxAnimationSpeed.Checked) objExpand.expandScript.scriptAnimationSpeed = null;
			if(!checkBoxSliderHorz.Checked) objExpand.expandScript.sliderHorz = null;
			if(!checkBoxSliderVert.Checked) objExpand.expandScript.sliderVert = null;
			if(!checkBoxColorLine.Checked)	objExpand.expandScript.scriptColorLine = null;
			if(!checkBoxColorFill.Checked)	objExpand.expandScript.scriptColorFill = null;
			if(!checkBoxColorText.Checked)	objExpand.expandScript.scriptColorText = null;
			if(!checkBoxColorBack.Checked)	objExpand.expandScript.scriptColorBack = null;
			if(!checkBoxThickLine.Checked)	objExpand.expandScript.scriptThickLine = null;
            if (!checkBoxRotate.Checked) objExpand.expandScript.scriptRotate = null;

			// 마우스 영역표시는 체크만 해도 기본값으로 할당되어야 한다.
			if(checkBoxZoneDisplay.Checked) 
			{
				if(objExpand.expandScript.structMouseZone == null) 
					objExpand.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			}

		}

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            string description;

            if (Tools.IsLangKorean())
                description = "회전각을 return (-360~360))";
            else
                description = "return rotation angle";

            FormExpandOptionRotate form = new FormExpandOptionRotate();

            objExpand.expandScript.scriptRotate = EditPropertyAndScript("Rotate", objExpand.expandScript.scriptRotate, checkBoxRotate, description, form);
        }

        private void checkBoxRotate_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        

        

        
		
	}
}
