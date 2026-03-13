using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using GraphicModule;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using AutoLibLocal;

namespace Studio
{
    /// <summary>
    /// Summary description for PropertyPageColor.
    /// </summary>
    public class PropertyPageBrushPublic : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Panel panelZone;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.NumericUpDown numericUpDownR;
        private System.Windows.Forms.NumericUpDown numericUpDownG;
        private System.Windows.Forms.NumericUpDown numericUpDownB;
        private System.Windows.Forms.NumericUpDown numericUpDownA;

        Color m_color;
        int nSelectColor;
        bool bEditToColorFlag = false;
        private System.Windows.Forms.GroupBox groupBox1;
        bool bSolidColorMultiFlag = false;  // 단색 채움의 다중 선택일 때 매치가 되지않는 경우
        private GroupBox groupBox2;
        private RadioButton radioButtonBrushType2;
        private RadioButton radioButtonBrushType1;
        private RadioButton radioButtonBrushType0;	// 다중 색상이 등록되어 있다.

        BrushPublic tempBrush = new BrushPublic();
        private NumericUpDown numericUpDownStartX;
        private NumericUpDown numericUpDownStartY;
        private GroupBox groupBoxStart;
        private Label label5;
        private Label label6;
        private GroupBox groupBoxEnd;
        private Label label7;
        private Label label8;
        private NumericUpDown numericUpDownEndX;
        private NumericUpDown numericUpDownEndY;

        MultiSelectRadioButton multiSelectBrushType = new MultiSelectRadioButton();
        MultiSelectNumericUpDown multiSelectStartX = new MultiSelectNumericUpDown();
        MultiSelectNumericUpDown multiSelectStartY = new MultiSelectNumericUpDown();
        MultiSelectNumericUpDown multiSelectEndX = new MultiSelectNumericUpDown();
        private GroupBox groupBoxSpread;
        private RadioButton radioButtonSpread2;
        private RadioButton radioButtonSpread1;
        private RadioButton radioButtonSpread0;
        MultiSelectNumericUpDown multiSelectEndY = new MultiSelectNumericUpDown();
        private Panel panelBlend;
        private Button buttonToRight;
        private Button buttonToDown;
        private Button buttonToDiagRight;
        private Button buttonToDiagLeft;
        private TextBox textBoxHex;
        private Label label9;

        MultiSelectRadioButton multiSelectSpread = new MultiSelectRadioButton();

        public PropertyPageBrushPublic()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            multiSelectBrushType.Add(radioButtonBrushType0, radioButtonBrushType1, radioButtonBrushType2);
            multiSelectStartX.Add(numericUpDownStartX);
            multiSelectStartY.Add(numericUpDownStartY);
            multiSelectEndX.Add(numericUpDownEndX);
            multiSelectEndY.Add(numericUpDownEndY);
            multiSelectSpread.Add(radioButtonSpread0, radioButtonSpread1, radioButtonSpread2);

            // Gradation Brush는 CE에서는 지원하지 않는다.
            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                this.radioButtonBrushType2.Enabled = false;
            }
        }

        int nListStopMultiCount = 0;    // listStop이 2이상이면 2개 이상의 오브젝트의 Gradient가 설정되어 있으므로 Stop영역에 마우스를 대기전까지는 리스트를 바꾸지 않는다.

        public void SetSelectedBrush(BrushPublic val, bool first_flag)
        {
            if (first_flag)
            {
                bEditToColorFlag = false;
                m_color = val.basic_color;
                numericUpDownA.Value = val.basic_color.A;
                numericUpDownR.Value = val.basic_color.R;
                numericUpDownG.Value = val.basic_color.G;
                numericUpDownB.Value = val.basic_color.B;
                bEditToColorFlag = true;

                UpdateHexFromColor(); //20241010
                SeekSelectColorNumber(val.basic_color.A, val.basic_color.R, val.basic_color.G, val.basic_color.B);
            }
            else // 다중 등록일 때 
            {
                if (val.basic_color != m_color)
                {
                    this.bSolidColorMultiFlag = true;
                    nSelectColor = -1;
                }
            }

            multiSelectBrushType.Set(val.brush_type);

            if (val.brush_type == 2)
            {
                BrushLinearGradient brush = (BrushLinearGradient)val;
                multiSelectStartX.Set((decimal)brush.pStart.X);
                multiSelectStartY.Set((decimal)brush.pStart.Y);
                multiSelectEndX.Set((decimal)brush.pEnd.X);
                multiSelectEndY.Set((decimal)brush.pEnd.Y);

                multiSelectSpread.Set(brush.spread_method);

                this.listStops = (List<BrushGradientStop>)Tools.CopyObject(brush.stops);

                nListStopMultiCount++;
            }
        }

        public void GetSelectedBrush(ref BrushPublic val)
        {
            if (multiSelectBrushType.IsMultipleSelected()) return;  // 여러종류가 선택된 상태는 바꿀수가 없다.

            int type = val.brush_type;
            multiSelectBrushType.Get(ref type);

            if (type == 1)
            {
                if (!bSolidColorMultiFlag)
                {
                    BrushSolid brush = new BrushSolid();
                    brush.basic_color = m_color;
                    val = brush;
                }
            }
            else if (type == 2)
            {
                if (nListStopMultiCount > 1) return;    // listStop이 2이상이면 2개 이상의 오브젝트의 Gradient가 설정되어 있으므로 Stop영역에 마우스를 대기전까지는 리스트를 바꾸지 않는다.

                BrushLinearGradient brush = new BrushLinearGradient();
                brush.basic_color = m_color;

                float x = 0, y = 0;

                multiSelectStartX.Get(ref x);
                multiSelectStartY.Get(ref y);
                brush.pStart = new PointF(x, y);
                multiSelectEndX.Get(ref x);
                multiSelectEndY.Get(ref y);
                brush.pEnd = new PointF(x, y);

                if (brush.pStart == brush.pEnd)
                {
                    brush.pStart = new PointF(0, 0);
                    brush.pEnd = new Point(1, 0);
                }

                multiSelectSpread.Get(ref brush.spread_method);

                brush.stops = (List<BrushGradientStop>)Tools.CopyObject(listStops);
                SortGradientsStops(brush.stops);

                val = brush;

            }
            else
            {
                BrushPublic brush = new BrushPublic();
                brush.basic_color = m_color;
                val = brush;
            }

            val.brush_type = type;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageBrushPublic));
            this.panelZone = new System.Windows.Forms.Panel();
            this.numericUpDownR = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownG = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownB = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownA = new System.Windows.Forms.NumericUpDown();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonBrushType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonBrushType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBrushType0 = new System.Windows.Forms.RadioButton();
            this.numericUpDownStartX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownStartY = new System.Windows.Forms.NumericUpDown();
            this.groupBoxStart = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxEnd = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownEndX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownEndY = new System.Windows.Forms.NumericUpDown();
            this.groupBoxSpread = new System.Windows.Forms.GroupBox();
            this.radioButtonSpread2 = new System.Windows.Forms.RadioButton();
            this.radioButtonSpread1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSpread0 = new System.Windows.Forms.RadioButton();
            this.panelBlend = new System.Windows.Forms.Panel();
            this.buttonToRight = new System.Windows.Forms.Button();
            this.buttonToDown = new System.Windows.Forms.Button();
            this.buttonToDiagRight = new System.Windows.Forms.Button();
            this.buttonToDiagLeft = new System.Windows.Forms.Button();
            this.textBoxHex = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownA)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartY)).BeginInit();
            this.groupBoxStart.SuspendLayout();
            this.groupBoxEnd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndY)).BeginInit();
            this.groupBoxSpread.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelZone
            // 
            resources.ApplyResources(this.panelZone, "panelZone");
            this.panelZone.Name = "panelZone";
            this.panelZone.Tag = "";
            this.panelZone.Paint += new System.Windows.Forms.PaintEventHandler(this.panelZone_Paint);
            this.panelZone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelZone_MouseDown);
            // 
            // numericUpDownR
            // 
            resources.ApplyResources(this.numericUpDownR, "numericUpDownR");
            this.numericUpDownR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownR.Name = "numericUpDownR";
            this.numericUpDownR.TextChanged += new System.EventHandler(this.numericUpDownR_TextChanged);
            this.numericUpDownR.ValueChanged += new System.EventHandler(this.numericUpDownR_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownG
            // 
            resources.ApplyResources(this.numericUpDownG, "numericUpDownG");
            this.numericUpDownG.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownG.Name = "numericUpDownG";
            this.numericUpDownG.TextChanged += new System.EventHandler(this.numericUpDownG_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownB
            // 
            resources.ApplyResources(this.numericUpDownB, "numericUpDownB");
            this.numericUpDownB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownB.Name = "numericUpDownB";
            this.numericUpDownB.TextChanged += new System.EventHandler(this.numericUpDownB_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownA
            // 
            resources.ApplyResources(this.numericUpDownA, "numericUpDownA");
            this.numericUpDownA.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownA.Name = "numericUpDownA";
            this.numericUpDownA.TextChanged += new System.EventHandler(this.numericUpDownA_TextChanged);
            // 
            // panelPreview
            // 
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPreview_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelPreview);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonBrushType2);
            this.groupBox2.Controls.Add(this.radioButtonBrushType1);
            this.groupBox2.Controls.Add(this.radioButtonBrushType0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonBrushType2
            // 
            resources.ApplyResources(this.radioButtonBrushType2, "radioButtonBrushType2");
            this.radioButtonBrushType2.Name = "radioButtonBrushType2";
            this.radioButtonBrushType2.TabStop = true;
            this.radioButtonBrushType2.UseVisualStyleBackColor = true;
            this.radioButtonBrushType2.CheckedChanged += new System.EventHandler(this.radioButtonBrushType2_CheckedChanged);
            // 
            // radioButtonBrushType1
            // 
            resources.ApplyResources(this.radioButtonBrushType1, "radioButtonBrushType1");
            this.radioButtonBrushType1.Name = "radioButtonBrushType1";
            this.radioButtonBrushType1.TabStop = true;
            this.radioButtonBrushType1.UseVisualStyleBackColor = true;
            this.radioButtonBrushType1.CheckedChanged += new System.EventHandler(this.radioButtonBrushType1_CheckedChanged);
            // 
            // radioButtonBrushType0
            // 
            resources.ApplyResources(this.radioButtonBrushType0, "radioButtonBrushType0");
            this.radioButtonBrushType0.Name = "radioButtonBrushType0";
            this.radioButtonBrushType0.TabStop = true;
            this.radioButtonBrushType0.UseVisualStyleBackColor = true;
            this.radioButtonBrushType0.CheckedChanged += new System.EventHandler(this.radioButtonBrushType0_CheckedChanged);
            // 
            // numericUpDownStartX
            // 
            this.numericUpDownStartX.DecimalPlaces = 2;
            this.numericUpDownStartX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.numericUpDownStartX, "numericUpDownStartX");
            this.numericUpDownStartX.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownStartX.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDownStartX.Name = "numericUpDownStartX";
            // 
            // numericUpDownStartY
            // 
            this.numericUpDownStartY.DecimalPlaces = 2;
            this.numericUpDownStartY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.numericUpDownStartY, "numericUpDownStartY");
            this.numericUpDownStartY.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownStartY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDownStartY.Name = "numericUpDownStartY";
            // 
            // groupBoxStart
            // 
            this.groupBoxStart.Controls.Add(this.label6);
            this.groupBoxStart.Controls.Add(this.label5);
            this.groupBoxStart.Controls.Add(this.numericUpDownStartX);
            this.groupBoxStart.Controls.Add(this.numericUpDownStartY);
            resources.ApplyResources(this.groupBoxStart, "groupBoxStart");
            this.groupBoxStart.Name = "groupBoxStart";
            this.groupBoxStart.TabStop = false;
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
            // groupBoxEnd
            // 
            this.groupBoxEnd.Controls.Add(this.label7);
            this.groupBoxEnd.Controls.Add(this.label8);
            this.groupBoxEnd.Controls.Add(this.numericUpDownEndX);
            this.groupBoxEnd.Controls.Add(this.numericUpDownEndY);
            resources.ApplyResources(this.groupBoxEnd, "groupBoxEnd");
            this.groupBoxEnd.Name = "groupBoxEnd";
            this.groupBoxEnd.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // numericUpDownEndX
            // 
            this.numericUpDownEndX.DecimalPlaces = 2;
            this.numericUpDownEndX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.numericUpDownEndX, "numericUpDownEndX");
            this.numericUpDownEndX.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownEndX.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDownEndX.Name = "numericUpDownEndX";
            this.numericUpDownEndX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownEndY
            // 
            this.numericUpDownEndY.DecimalPlaces = 2;
            this.numericUpDownEndY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.numericUpDownEndY, "numericUpDownEndY");
            this.numericUpDownEndY.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownEndY.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDownEndY.Name = "numericUpDownEndY";
            // 
            // groupBoxSpread
            // 
            this.groupBoxSpread.Controls.Add(this.radioButtonSpread2);
            this.groupBoxSpread.Controls.Add(this.radioButtonSpread1);
            this.groupBoxSpread.Controls.Add(this.radioButtonSpread0);
            resources.ApplyResources(this.groupBoxSpread, "groupBoxSpread");
            this.groupBoxSpread.Name = "groupBoxSpread";
            this.groupBoxSpread.TabStop = false;
            // 
            // radioButtonSpread2
            // 
            resources.ApplyResources(this.radioButtonSpread2, "radioButtonSpread2");
            this.radioButtonSpread2.Name = "radioButtonSpread2";
            this.radioButtonSpread2.TabStop = true;
            this.radioButtonSpread2.UseVisualStyleBackColor = true;
            // 
            // radioButtonSpread1
            // 
            resources.ApplyResources(this.radioButtonSpread1, "radioButtonSpread1");
            this.radioButtonSpread1.Name = "radioButtonSpread1";
            this.radioButtonSpread1.TabStop = true;
            this.radioButtonSpread1.UseVisualStyleBackColor = true;
            // 
            // radioButtonSpread0
            // 
            resources.ApplyResources(this.radioButtonSpread0, "radioButtonSpread0");
            this.radioButtonSpread0.Name = "radioButtonSpread0";
            this.radioButtonSpread0.TabStop = true;
            this.radioButtonSpread0.UseVisualStyleBackColor = true;
            // 
            // panelBlend
            // 
            resources.ApplyResources(this.panelBlend, "panelBlend");
            this.panelBlend.Name = "panelBlend";
            this.panelBlend.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBlend_Paint);
            this.panelBlend.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelBlend_MouseMove);
            this.panelBlend.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBlend_MouseDown);
            this.panelBlend.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelBlend_MouseUp);
            // 
            // buttonToRight
            // 
            resources.ApplyResources(this.buttonToRight, "buttonToRight");
            this.buttonToRight.Name = "buttonToRight";
            this.buttonToRight.UseVisualStyleBackColor = true;
            this.buttonToRight.Click += new System.EventHandler(this.buttonToRight_Click);
            // 
            // buttonToDown
            // 
            resources.ApplyResources(this.buttonToDown, "buttonToDown");
            this.buttonToDown.Name = "buttonToDown";
            this.buttonToDown.UseVisualStyleBackColor = true;
            this.buttonToDown.Click += new System.EventHandler(this.buttonToDown_Click);
            // 
            // buttonToDiagRight
            // 
            resources.ApplyResources(this.buttonToDiagRight, "buttonToDiagRight");
            this.buttonToDiagRight.Name = "buttonToDiagRight";
            this.buttonToDiagRight.UseVisualStyleBackColor = true;
            this.buttonToDiagRight.Click += new System.EventHandler(this.buttonToDiagRight_Click);
            // 
            // buttonToDiagLeft
            // 
            resources.ApplyResources(this.buttonToDiagLeft, "buttonToDiagLeft");
            this.buttonToDiagLeft.Name = "buttonToDiagLeft";
            this.buttonToDiagLeft.UseVisualStyleBackColor = true;
            this.buttonToDiagLeft.Click += new System.EventHandler(this.buttonToDiagLeft_Click);
            // 
            // textBoxHex
            // 
            resources.ApplyResources(this.textBoxHex, "textBoxHex");
            this.textBoxHex.Name = "textBoxHex";
            this.textBoxHex.TextChanged += new System.EventHandler(this.textBoxHex_TextChanged);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // PropertyPageBrushPublic
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.textBoxHex);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.buttonToDiagLeft);
            this.Controls.Add(this.buttonToDiagRight);
            this.Controls.Add(this.buttonToDown);
            this.Controls.Add(this.buttonToRight);
            this.Controls.Add(this.panelBlend);
            this.Controls.Add(this.groupBoxSpread);
            this.Controls.Add(this.groupBoxEnd);
            this.Controls.Add(this.groupBoxStart);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownG);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownR);
            this.Controls.Add(this.panelZone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PropertyPageBrushPublic";
            this.Load += new System.EventHandler(this.PropertyPageBrushPublic_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownA)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartY)).EndInit();
            this.groupBoxStart.ResumeLayout(false);
            this.groupBoxStart.PerformLayout();
            this.groupBoxEnd.ResumeLayout(false);
            this.groupBoxEnd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEndY)).EndInit();
            this.groupBoxSpread.ResumeLayout(false);
            this.groupBoxSpread.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void panelZone_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // TODO: Add your message handler code here
            Rectangle rect = panelZone.ClientRectangle;
            int x1, y1, x2, y2;
            int pos = 0;
            int i, j;
            Color color;

            x1 = 0;
            y1 = 0;
            x2 = 0;
            for (i = 0; i < 16; i++)
            {
                y1 = 0;
                for (j = 0; j < 16; j++, pos++)
                {
                    x2 = rect.Right * (i + 1) / 16;
                    y2 = rect.Bottom * (j + 1) / 16;

                    color = Color.FromArgb(DEFAULT_RGB.dac[pos * 3 + 0], DEFAULT_RGB.dac[pos * 3 + 1], DEFAULT_RGB.dac[pos * 3 + 2]);

                    DrawClass.gcls(g, x1, y1, x2 - 1, y2 - 1, color);

                    if (pos == nSelectColor)
                    {
                        DrawClass.PushRectangle3(g, x1, y1, x2 - 1, y2 - 1);
                    }

                    y1 = y2;
                }
                x1 = x2;
            }
        }

        private void panelZone_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Rectangle rect = panelZone.ClientRectangle;
            int x1, y1, x2, y2;
            int pos = 0;
            int i, j;

            x1 = 0;
            y1 = 0;
            x2 = 0;
            for (i = 0; i < 16; i++)
            {
                y1 = 0;
                for (j = 0; j < 16; j++, pos++)
                {
                    x2 = rect.Right * (i + 1) / 16;
                    y2 = rect.Bottom * (j + 1) / 16;

                    if (e.X >= x1 && e.X < x2 && e.Y >= y1 && e.Y < y2)
                    {
                        bSolidColorMultiFlag = false;
                        nSelectColor = pos;
                        m_color = Color.FromArgb(DEFAULT_RGB.dac[pos * 3 + 0], DEFAULT_RGB.dac[pos * 3 + 1], DEFAULT_RGB.dac[pos * 3 + 2]);
                        bEditToColorFlag = false;
                        numericUpDownA.Value = 255;
                        numericUpDownR.Value = DEFAULT_RGB.dac[pos * 3 + 0];
                        numericUpDownG.Value = DEFAULT_RGB.dac[pos * 3 + 1];
                        numericUpDownB.Value = DEFAULT_RGB.dac[pos * 3 + 2];
                        bEditToColorFlag = true;

                        // 그라데이션 모드일때만 Update 2010.4.15 추가
                        if (radioButtonBrushType2.Checked)
                        {
                            if (nCursorPos < listStops.Count)
                            {
                                listStops[nCursorPos].color = m_color;
                            }
                            panelBlend.Invalidate();    
                        }

                        UpdateHexFromColor(); //20241010
                        panelZone.Invalidate();
                        panelPreview.Invalidate();

                        return;
                    }

                    y1 = y2;
                }
                x1 = x2;
            }
        }

        private void panelPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //if (nSelectColor != -1)
            //{
                DrawClass.gcls(e.Graphics, panelPreview.ClientRectangle, m_color);
            //}
        }

        void SeekSelectColorNumber(int a, int r, int g, int b)
        {
            m_color = Color.FromArgb(a, r, g, b);

            int i;

            for (i = 0; i < 256; i++)
            {
                if (r == DEFAULT_RGB.dac[i * 3 + 0] &&
                    g == DEFAULT_RGB.dac[i * 3 + 1] &&
                    b == DEFAULT_RGB.dac[i * 3 + 2])
                {
                    nSelectColor = i;
                    return;
                }
            }

            nSelectColor = -1;
        }

        void EditToColor()
        {
            if (!bEditToColorFlag) return;	// 지금은 하지 않는다.

            bSolidColorMultiFlag = false;

            int a = ConvertTool.ToInt32(numericUpDownA.Value);
            int r = ConvertTool.ToInt32(numericUpDownR.Value);
            int g = ConvertTool.ToInt32(numericUpDownG.Value);
            int b = ConvertTool.ToInt32(numericUpDownB.Value);

            // panelZone의 화면이 떠는것을 방지하기위해 코드 추가
            int old_color = nSelectColor;
            SeekSelectColorNumber(a, r, g, b);
            if (old_color != nSelectColor)
            {
                panelZone.Invalidate();
            }

            panelPreview.Invalidate();

            if (nCursorPos < listStops.Count)
            {
                listStops[nCursorPos].color = m_color;
            }
            UpdateHexFromColor(); //20241010
            panelBlend.Invalidate();
        }

        private void numericUpDownR_TextChanged(object sender, System.EventArgs e)
        {
            EditToColor();
        }

        private void numericUpDownG_TextChanged(object sender, System.EventArgs e)
        {
            EditToColor();
        }

        private void numericUpDownB_TextChanged(object sender, System.EventArgs e)
        {
            EditToColor();
        }

        private void numericUpDownA_TextChanged(object sender, System.EventArgs e)
        {
            EditToColor();
        }

        private void PropertyPageBrushPublic_Load(object sender, EventArgs e)
        {

        }

        List<BrushGradientStop> listStops = new List<BrushGradientStop>();

        void SortColorBlend(ColorBlend blend)
        {
            Color temp_color;
            float temp_offset;
            int small_pos;

            for (int i = 0; i < blend.Positions.Length - 1; i++)
            {
                small_pos = i;
                for (int j = i + 1; j < blend.Positions.Length; j++)
                {
                    if (blend.Positions[j] < blend.Positions[small_pos])
                    {
                        small_pos = j;
                    }
                }

                if (small_pos != i)
                {
                    temp_color = blend.Colors[i];
                    blend.Colors[i] = blend.Colors[small_pos];
                    blend.Colors[small_pos] = temp_color;

                    temp_offset = blend.Positions[i];
                    blend.Positions[i] = blend.Positions[small_pos];
                    blend.Positions[small_pos] = temp_offset;
                }
            }
        }

        void SortGradientsStops(List<BrushGradientStop> stops)
        {
            int small_pos;

            for (int i = 0; i < stops.Count - 1; i++)
            {
                small_pos = i;
                for (int j = i + 1; j < stops.Count; j++)
                {
                    if (stops[j].offset < stops[small_pos].offset)
                    {
                        small_pos = j;
                    }
                }

                if (small_pos != i)
                {
                    BrushGradientStop temp;

                    temp = stops[i];
                    stops[i] = stops[small_pos];
                    stops[small_pos] = temp;
                }
            }
        }

        private void panelBlend_Paint(object sender, PaintEventArgs e)
        {
            int x1, y1, x2, y2;
            GetGradationBarZone(out x1, out y1, out x2, out y2);

            LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(x1, y1), new Point(x2, y2), Color.Blue, Color.Red);

            ColorBlend blend = ObjectRectangle.MakeColorBlend(listStops);
            SortColorBlend(blend);

            brush.InterpolationColors = blend;

            DrawClass.gcls(e.Graphics, x1, y1+1, x2, y2-1, brush);
            DrawClass.grect(e.Graphics, x1-1, y1, x2+1, y2, Color.DarkGray);

            int x;

            for (int i = 0; i < listStops.Count; i++)
            {
                x = (int)(x1 + (x2 - x1) * listStops[i].offset);

                DrawPosCursor(e.Graphics, x1, y1, x2, y2, x, listStops[i].color, nCursorPos == i);
            }
        }

        int nCursorPos = 0;

        void GetGradationBarZone(out int x1, out int y1, out int x2, out int y2)
        {
            x1 = 3;
            y1 = 0;
            x2 = panelBlend.ClientRectangle.Right - 4;
            y2 = panelBlend.ClientRectangle.Height / 2;
        }

        void DrawPosCursor(Graphics g, int x1, int y1, int x2, int y2, int x, Color color, bool cursor_flag)
        {
            Point[] polygon = new Point[5];

            polygon[0].X = x; polygon[0].Y = y2;
            polygon[1].X = x - 3; polygon[1].Y = y2 + 3;
            polygon[2].X = x - 3; polygon[2].Y = panelBlend.ClientRectangle.Bottom - 1;
            polygon[3].X = x + 3; polygon[3].Y = panelBlend.ClientRectangle.Bottom - 1;
            polygon[4].X = x + 3; polygon[4].Y = y2 + 3;

            Brush brush = new SolidBrush(color);
            g.FillPolygon(brush, polygon);

            Pen pen = new Pen(cursor_flag ? Color.Black : Color.DarkGray, 1);
            g.DrawPolygon(pen, polygon);
        }

        bool bMouseCapture = false;

        private void panelBlend_MouseDown(object sender, MouseEventArgs e)
        {
            int x1, y1, x2, y2;
            GetGradationBarZone(out x1, out y1, out x2, out y2);

            if (e.X >= x1 && e.Y >= y1 && e.X <= x2 && e.Y <= y2)
            {
                BrushGradientStop stop = new BrushGradientStop();
                stop.color = Color.White;
                stop.offset = (float)(e.X - x1) / (x2 - x1);

                // 2012-04-18 받은 오류메시지 중 아래 메시지가 있어서 offset이 범위를 넘지 않나해서 아래 코드를 추가함
                // ColorBlend object that was set is not valid.Position's last element must be equal to 1.0. ColorBlend objects must be constructed with the same number of positions and color values. Positions must be between 0.0 and 1.0, 1.0 indicating the last element in the array.
                if (stop.offset < 0) stop.offset = 0;
                if (stop.offset > 1) stop.offset = 1;

                listStops.Add(stop);

                nCursorPos = listStops.Count - 1;

                panelBlend.Invalidate();

                nListStopMultiCount = 1;

                return;
            }

            if (e.X >= x1 && e.Y > y2 && e.X <= x2 && e.Y < panelBlend.Bottom)
            {
                for (int i = 0; i < listStops.Count; i++)
                {
                    int x = (int)(x1 + (x2 - x1) * listStops[i].offset);

                    if (e.X >= x - 3 && e.X <= x + 3)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            listStops.RemoveAt(i);
                        }
                        else
                        {
                            nCursorPos = i;
                            panelBlend.Capture = true;
                            bMouseCapture = true;

                            Color color = listStops[nCursorPos].color;
                            bEditToColorFlag = false;
                            m_color = color;
                            numericUpDownA.Value = color.A;
                            numericUpDownR.Value = color.R;
                            numericUpDownG.Value = color.G;
                            numericUpDownB.Value = color.B;
                            bEditToColorFlag = true;

                            SeekSelectColorNumber(color.A, color.R, color.G, color.B);
                            panelZone.Invalidate();
                            panelPreview.Invalidate();
                        }
                        panelBlend.Invalidate();

                        nListStopMultiCount = 1;

                        return;
                    }
                }
            }
        }

        private void panelBlend_MouseMove(object sender, MouseEventArgs e)
        {
            int x1, y1, x2, y2;
            GetGradationBarZone(out x1, out y1, out x2, out y2);

            if (!bMouseCapture)
            {
                if (e.X >= x1 && e.Y > y2 && e.X <= x2 && e.Y < panelBlend.Bottom)
                {
                    for (int i = 0; i < listStops.Count; i++)
                    {
                        int x = (int)(x1 + (x2 - x1) * listStops[i].offset);

                        if (e.X >= x - 3 && e.X <= x + 3)
                        {
                            Cursor = Cursors.SizeWE;
                            return;
                        }
                    }
                }
                Cursor = Cursors.Arrow;
                return;
            }

            Cursor = Cursors.SizeWE;

            float offset = (float)(e.X - x1) / (x2 - x1);
            if (offset < 0) offset = 0;
            if (offset > 1) offset = 1;

            listStops[nCursorPos].offset = offset;

            panelBlend.Invalidate();
        }

        private void panelBlend_MouseUp(object sender, MouseEventArgs e)
        {
            if (!bMouseCapture) return;

            bMouseCapture = false;
            panelBlend.Capture = false;
            Cursor = Cursors.Arrow;
            panelBlend.Cursor = Cursors.Arrow;
        }

        void EnableTypes()
        {
            int type;

            if (radioButtonBrushType0.Checked) type = 0;
            else if (radioButtonBrushType1.Checked) type = 1;
            else if (radioButtonBrushType2.Checked) type = 2;
            else type = 1;

            bool flag_grad = (type == 2);

            this.groupBoxStart.Enabled = flag_grad;
            this.groupBoxEnd.Enabled = flag_grad;
            this.groupBoxSpread.Enabled = flag_grad;
            this.panelBlend.Enabled = flag_grad;
        }

        private void radioButtonBrushType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableTypes();
        }

        private void radioButtonBrushType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableTypes();
        }

        private void radioButtonBrushType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableTypes();

            if (listStops.Count == 0)
            {
                BrushGradientStop stop = new BrushGradientStop();
                stop.offset = 0;
                stop.color = m_color;
                listStops.Add(stop);
                stop = new BrushGradientStop();
                stop.offset = 1;
                stop.color = Color.Black;
                listStops.Add(stop);

                panelBlend.Invalidate();
            }
        }

        private void buttonToRight_Click(object sender, EventArgs e)
        {
            this.numericUpDownStartX.Value = 0;
            this.numericUpDownStartY.Value = 0;
            this.numericUpDownEndX.Value = 1;
            this.numericUpDownEndY.Value = 0;

            if (!this.radioButtonBrushType2.Checked) this.radioButtonBrushType2.Checked = true;

            nListStopMultiCount = 1;
        }

        private void buttonToDown_Click(object sender, EventArgs e)
        {
            this.numericUpDownStartX.Value = 0;
            this.numericUpDownStartY.Value = 0;
            this.numericUpDownEndX.Value = 0;
            this.numericUpDownEndY.Value = 1;

            if (!this.radioButtonBrushType2.Checked) this.radioButtonBrushType2.Checked = true;

            nListStopMultiCount = 1;
        }

        private void buttonToDiagRight_Click(object sender, EventArgs e)
        {
            this.numericUpDownStartX.Value = 0;
            this.numericUpDownStartY.Value = 0;
            this.numericUpDownEndX.Value = 1;
            this.numericUpDownEndY.Value = 1;

            if (!this.radioButtonBrushType2.Checked) this.radioButtonBrushType2.Checked = true;

            nListStopMultiCount = 1;
        }

        private void buttonToDiagLeft_Click(object sender, EventArgs e)
        {
            this.numericUpDownStartX.Value = 1;
            this.numericUpDownStartY.Value = 0;
            this.numericUpDownEndX.Value = 0;
            this.numericUpDownEndY.Value = 1;

            if (!this.radioButtonBrushType2.Checked) this.radioButtonBrushType2.Checked = true;

            nListStopMultiCount = 1;
        }

        private void numericUpDownR_ValueChanged(object sender, EventArgs e)
        {

        }

        // 16진수 RRGGBB 추가 20241010
        private void UpdateHexFromColor()
        {
            textBoxHex.Text = string.Format("{0:X2}{1:X2}{2:X2}", m_color.R, m_color.G, m_color.B);
        }

        private void UpdateColorFromHex(string hexColor)
        {
            if (hexColor.Length == 6)
            {
                try
                {
                    int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

                    bEditToColorFlag = false;
                    numericUpDownR.Value = r;
                    numericUpDownG.Value = g;
                    numericUpDownB.Value = b;
                    bEditToColorFlag = true;

                    EditToColor();
                }
                catch
                {
                    // Invalid hex color, do nothing
                }
            }
        }

        private void textBoxHex_TextChanged(object sender, EventArgs e)
        {
            if (!bEditToColorFlag) return;
            UpdateColorFromHex(textBoxHex.Text);
        }

    }
}

