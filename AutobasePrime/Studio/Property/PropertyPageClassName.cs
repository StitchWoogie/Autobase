using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using GraphicModule;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageClassName.
	/// </summary>
	public class PropertyPageClassName : System.Windows.Forms.Form
	{
		public System.Windows.Forms.TextBox textBoxClassName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectTextBox multiSelectDescription = new MultiSelectTextBox();
		private System.Windows.Forms.CheckBox checkBoxUseToolTip;
		private System.Windows.Forms.TextBox textBoxDescription;
        private CheckBox checkBoxResponseOnVisible;
		MultiSelectCheckBox multiSelectUseToolTip = new MultiSelectCheckBox();
        private GroupBox groupBox3;
        private NumericUpDown numericUpDownHeight;
        private Label label3;
        private NumericUpDown numericUpDownWidth;
        private Label label4;
        private GroupBox groupBox4;
        private NumericUpDown numericUpDownY;
        private Label label2;
        private NumericUpDown numericUpDownX;
        private Label label5;

        MultiSelectCheckBox multiSelectResponseOnVisible = new MultiSelectCheckBox();

        MultiSelectNumericUpDown multiSelectX = new MultiSelectNumericUpDown();
        MultiSelectNumericUpDown multiSelectY = new MultiSelectNumericUpDown();
        MultiSelectNumericUpDown multiSelectWidth = new MultiSelectNumericUpDown();
        private NumericUpDown numericUpDownRotate;
        private Label label6;
        MultiSelectNumericUpDown multiSelectHeight = new MultiSelectNumericUpDown();
        private GroupBox groupBox5;
        public TextBox textBoxNameForEdit;

        MultiSelectTextBox multiSelectNameForEdit = new MultiSelectTextBox();

        MultiSelectNumericUpDown multiSelectRotate = new MultiSelectNumericUpDown();

		public PropertyPageClassName()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            // 등록을 Load에서 하면 안됨 Load는 호출되지 않음
			this.multiSelectDescription.Add(textBoxDescription);
			this.multiSelectUseToolTip.Add(checkBoxUseToolTip);
            multiSelectResponseOnVisible.Add(this.checkBoxResponseOnVisible);


            multiSelectX.Add(this.numericUpDownX);
            multiSelectY.Add(this.numericUpDownY);
            multiSelectWidth.Add(this.numericUpDownWidth);
            multiSelectHeight.Add(this.numericUpDownHeight);

            multiSelectRotate.Add(this.numericUpDownRotate);

            multiSelectNameForEdit.Add(textBoxNameForEdit);

            this.numericUpDownRotate.Enabled = (TotalConfigProject.eProjectPlatform != EnumProjectPlatform.CE);
		}

        public void InitPositionSize()
        {
            multiSelectX = new MultiSelectNumericUpDown();
            multiSelectY = new MultiSelectNumericUpDown();
            multiSelectWidth = new MultiSelectNumericUpDown();
            multiSelectHeight = new MultiSelectNumericUpDown();

            multiSelectX.Add(this.numericUpDownX);
            multiSelectY.Add(this.numericUpDownY);
            multiSelectWidth.Add(this.numericUpDownWidth);
            multiSelectHeight.Add(this.numericUpDownHeight);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageClassName));
            this.textBoxClassName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxUseToolTip = new System.Windows.Forms.CheckBox();
            this.checkBoxResponseOnVisible = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numericUpDownY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownX = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownRotate = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxNameForEdit = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotate)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxClassName
            // 
            this.textBoxClassName.AccessibleDescription = null;
            this.textBoxClassName.AccessibleName = null;
            resources.ApplyResources(this.textBoxClassName, "textBoxClassName");
            this.textBoxClassName.BackgroundImage = null;
            this.textBoxClassName.Font = null;
            this.textBoxClassName.Name = "textBoxClassName";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxClassName);
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
            this.groupBox2.Controls.Add(this.textBoxDescription);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.checkBoxUseToolTip);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AccessibleDescription = null;
            this.textBoxDescription.AccessibleName = null;
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.BackgroundImage = null;
            this.textBoxDescription.Font = null;
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // checkBoxUseToolTip
            // 
            this.checkBoxUseToolTip.AccessibleDescription = null;
            this.checkBoxUseToolTip.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseToolTip, "checkBoxUseToolTip");
            this.checkBoxUseToolTip.BackgroundImage = null;
            this.checkBoxUseToolTip.Font = null;
            this.checkBoxUseToolTip.Name = "checkBoxUseToolTip";
            // 
            // checkBoxResponseOnVisible
            // 
            this.checkBoxResponseOnVisible.AccessibleDescription = null;
            this.checkBoxResponseOnVisible.AccessibleName = null;
            resources.ApplyResources(this.checkBoxResponseOnVisible, "checkBoxResponseOnVisible");
            this.checkBoxResponseOnVisible.BackgroundImage = null;
            this.checkBoxResponseOnVisible.Font = null;
            this.checkBoxResponseOnVisible.Name = "checkBoxResponseOnVisible";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.numericUpDownHeight);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownWidth);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.AccessibleDescription = null;
            this.numericUpDownHeight.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownHeight, "numericUpDownHeight");
            this.numericUpDownHeight.Font = null;
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.AccessibleDescription = null;
            this.numericUpDownWidth.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownWidth, "numericUpDownWidth");
            this.numericUpDownWidth.Font = null;
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.numericUpDownY);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.numericUpDownX);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // numericUpDownY
            // 
            this.numericUpDownY.AccessibleDescription = null;
            this.numericUpDownY.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownY, "numericUpDownY");
            this.numericUpDownY.Font = null;
            this.numericUpDownY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDownY.Name = "numericUpDownY";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownX
            // 
            this.numericUpDownX.AccessibleDescription = null;
            this.numericUpDownX.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownX, "numericUpDownX");
            this.numericUpDownX.Font = null;
            this.numericUpDownX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDownX.Name = "numericUpDownX";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // numericUpDownRotate
            // 
            this.numericUpDownRotate.AccessibleDescription = null;
            this.numericUpDownRotate.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownRotate, "numericUpDownRotate");
            this.numericUpDownRotate.Font = null;
            this.numericUpDownRotate.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownRotate.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numericUpDownRotate.Name = "numericUpDownRotate";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // groupBox5
            // 
            this.groupBox5.AccessibleDescription = null;
            this.groupBox5.AccessibleName = null;
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.BackgroundImage = null;
            this.groupBox5.Controls.Add(this.textBoxNameForEdit);
            this.groupBox5.Font = null;
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // textBoxNameForEdit
            // 
            this.textBoxNameForEdit.AccessibleDescription = null;
            this.textBoxNameForEdit.AccessibleName = null;
            resources.ApplyResources(this.textBoxNameForEdit, "textBoxNameForEdit");
            this.textBoxNameForEdit.BackgroundImage = null;
            this.textBoxNameForEdit.Font = null;
            this.textBoxNameForEdit.Name = "textBoxNameForEdit";
            // 
            // PropertyPageClassName
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.numericUpDownRotate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxResponseOnVisible);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageClassName";
            this.Load += new System.EventHandler(this.PropertyPageClassName_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRotate)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void Set(ObjectExpand obj)
		{
			this.multiSelectUseToolTip.Set(obj.objGeneral.bUseToolTip);
			this.multiSelectDescription.Set(obj.objGeneral.sObjectDescription);

            this.multiSelectResponseOnVisible.Set(obj.objGeneral.bResponseOnVisible);

            this.multiSelectRotate.Set((decimal)obj.objGeneral.fRotateAngle);

            this.multiSelectNameForEdit.Set(obj.objGeneral.sOnStudioTitle);

            SetPosSize(obj);
		}

        public void Get(FormEditGraphic form, ObjectExpand obj)
		{
			this.multiSelectUseToolTip.Get(ref obj.objGeneral.bUseToolTip);
			this.multiSelectDescription.Get(ref obj.objGeneral.sObjectDescription);

            this.multiSelectResponseOnVisible.Get(ref obj.objGeneral.bResponseOnVisible);

            this.multiSelectRotate.Get(ref obj.objGeneral.fRotateAngle);

            this.multiSelectNameForEdit.Get(ref obj.objGeneral.sOnStudioTitle);

            GetPosSize(form, obj);
		}

        void SetPosSize(ObjectExpand obj)
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            obj.GetZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            int width = x2 - x1 + 1;
            int height = y2 - y1 + 1;

            multiSelectX.Set(x1);
            multiSelectY.Set(y1);
            multiSelectWidth.Set(width);
            multiSelectHeight.Set(height);
        }

        void GetPosSize(FormEditGraphic form, ObjectExpand obj)
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int ox1, oy1, ox2, oy2;
            obj.GetZone(ref x1, ref y1, ref x2, ref y2);

            ox1 = x1;
            oy1 = y1;
            ox2 = x2;
            oy2 = y2;

            int x = x1;
            int y = y1;

            if (x1 > x2)
            {
                x = x2;
            }
            if (y1 > y2)
            {
                y = y2;
            }

            int width = Math.Abs(x2 - x1) + 1;
            int height = Math.Abs(y2 - y1) + 1;

            int old_width = width;
            int old_height = height;

            int smallox = x;    
            int smalloy = y;

            multiSelectX.Get(ref x);
            multiSelectY.Get(ref y);

            if (!obj.bSetOriginalSizeOnStudio)  // 사이즈가 다른 속성 부분에서 변경되었다. 위치만 적용한다.
            {  
                multiSelectWidth.Get(ref width);
                multiSelectHeight.Get(ref height);
            }
            
            if (x1 > x2)
            {
                x2 = x;
                x1 = x2 + (width - 1);
            }
            else
            {
                x1 = x;
                x2 = x1 + (width - 1);
            }

            if (y1 > y2)
            {
                y2 = y;
                y1 = y2 + (height - 1);
            }
            else
            {
                y1 = y;
                y2 = y1 + (height - 1);
            }

            // 한줄 글자인 경우 크기는 적용하지 않는다. 폰트와 상반되는 기능.
            if (obj.enumObjectType == EnumObjectType.SingleText)
            {
                if (ox1 != x1 || oy1 != y1)
                {
                    int gabx = x1 - ox1;
                    int gaby = y1 - oy1;

                    obj.UpdateZone(form, x1, y1, ox2 + gabx, oy2 + gaby);
                    form.SelectListUpdate();	// 오브젝트의 위치가 바뀌었으므로 다시 리스트의 좌표를 갱신해야 한다.
                    form.workThis.obj.SetZoneAtPercent100();
                }
            }
            else
            {
                if (ox1 != x1 || oy1 != y1 || ox2 != x2 || oy2 != y2)
                {
                    obj.UpdateZone(form, x1, y1, x2, y2);
                    form.SelectListUpdate();	// 오브젝트의 위치가 바뀌었으므로 다시 리스트의 좌표를 갱신해야 한다.
                    form.workThis.obj.SetZoneAtPercent100();
                }
            }
        }

        private void PropertyPageClassName_Load(object sender, EventArgs e)
        {

        }
	}
}
