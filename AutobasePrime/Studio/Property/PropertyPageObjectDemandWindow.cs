using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLib;
using AutoLibLocal;

namespace Studio
{
    /// <summary>
    /// Summary description for PropertyPageObjectDemandWindow.
    /// </summary>
    public class PropertyPageObjectDemandWindow : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxDemandName;
        private System.Windows.Forms.VScrollBar vScrollBarThick;
        private System.Windows.Forms.Panel panelThick;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private RadioButton radioButtonPos0;
        private RadioButton radioButtonPos1;
        private Button buttonColorPredictionPower;
        private Button buttonColorExcessedPower;
        private Button buttonColorTargetPower;
        private Button buttonStatusBack;
        private Button buttonStatusFill;
        private Button buttonStatusValue;
        private Button buttonTargetValue;
        private Button buttonPreValue;
        private Button buttonExValue;
        private GroupBox groupBox2;
        private GroupBox groupBox3;

        MultiSelectRadioButton multiSelectStatusBarPos = new MultiSelectRadioButton();
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private RadioButton radioButtonPos2;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public PropertyPageObjectDemandWindow()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            multiSelectStatusBarPos.Add(this.radioButtonPos0,
                                    this.radioButtonPos1, this.radioButtonPos2);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDemandWindow));
            this.comboBoxDemandName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vScrollBarThick = new System.Windows.Forms.VScrollBar();
            this.panelThick = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.radioButtonPos0 = new System.Windows.Forms.RadioButton();
            this.radioButtonPos1 = new System.Windows.Forms.RadioButton();
            this.buttonColorPredictionPower = new System.Windows.Forms.Button();
            this.buttonColorExcessedPower = new System.Windows.Forms.Button();
            this.buttonColorTargetPower = new System.Windows.Forms.Button();
            this.buttonStatusBack = new System.Windows.Forms.Button();
            this.buttonStatusFill = new System.Windows.Forms.Button();
            this.buttonStatusValue = new System.Windows.Forms.Button();
            this.buttonTargetValue = new System.Windows.Forms.Button();
            this.buttonPreValue = new System.Windows.Forms.Button();
            this.buttonExValue = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonPos2 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDemandName
            // 
            resources.ApplyResources(this.comboBoxDemandName, "comboBoxDemandName");
            this.comboBoxDemandName.Name = "comboBoxDemandName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vScrollBarThick);
            this.groupBox1.Controls.Add(this.panelThick);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // vScrollBarThick
            // 
            resources.ApplyResources(this.vScrollBarThick, "vScrollBarThick");
            this.vScrollBarThick.Maximum = 29;
            this.vScrollBarThick.Minimum = 1;
            this.vScrollBarThick.Name = "vScrollBarThick";
            this.vScrollBarThick.Value = 1;
            this.vScrollBarThick.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarThick_Scroll);
            // 
            // panelThick
            // 
            resources.ApplyResources(this.panelThick, "panelThick");
            this.panelThick.Name = "panelThick";
            this.panelThick.Paint += new System.Windows.Forms.PaintEventHandler(this.panelThick_Paint);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
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
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // radioButtonPos0
            // 
            resources.ApplyResources(this.radioButtonPos0, "radioButtonPos0");
            this.radioButtonPos0.Name = "radioButtonPos0";
            this.radioButtonPos0.TabStop = true;
            this.radioButtonPos0.UseVisualStyleBackColor = true;
            // 
            // radioButtonPos1
            // 
            resources.ApplyResources(this.radioButtonPos1, "radioButtonPos1");
            this.radioButtonPos1.Name = "radioButtonPos1";
            this.radioButtonPos1.TabStop = true;
            this.radioButtonPos1.UseVisualStyleBackColor = true;
            // 
            // buttonColorPredictionPower
            // 
            resources.ApplyResources(this.buttonColorPredictionPower, "buttonColorPredictionPower");
            this.buttonColorPredictionPower.Name = "buttonColorPredictionPower";
            this.buttonColorPredictionPower.Click += new System.EventHandler(this.buttonColorPredictionPower_Click);
            // 
            // buttonColorExcessedPower
            // 
            resources.ApplyResources(this.buttonColorExcessedPower, "buttonColorExcessedPower");
            this.buttonColorExcessedPower.Name = "buttonColorExcessedPower";
            this.buttonColorExcessedPower.Click += new System.EventHandler(this.buttonColorExcessedPower_Click);
            // 
            // buttonColorTargetPower
            // 
            resources.ApplyResources(this.buttonColorTargetPower, "buttonColorTargetPower");
            this.buttonColorTargetPower.Name = "buttonColorTargetPower";
            this.buttonColorTargetPower.Click += new System.EventHandler(this.buttonColorTargetPower_Click);
            // 
            // buttonStatusBack
            // 
            resources.ApplyResources(this.buttonStatusBack, "buttonStatusBack");
            this.buttonStatusBack.Name = "buttonStatusBack";
            this.buttonStatusBack.Click += new System.EventHandler(this.buttonStatusBack_Click);
            // 
            // buttonStatusFill
            // 
            resources.ApplyResources(this.buttonStatusFill, "buttonStatusFill");
            this.buttonStatusFill.Name = "buttonStatusFill";
            this.buttonStatusFill.Click += new System.EventHandler(this.buttonStatusFill_Click);
            // 
            // buttonStatusValue
            // 
            resources.ApplyResources(this.buttonStatusValue, "buttonStatusValue");
            this.buttonStatusValue.Name = "buttonStatusValue";
            this.buttonStatusValue.Click += new System.EventHandler(this.buttonStatusValue_Click);
            // 
            // buttonTargetValue
            // 
            resources.ApplyResources(this.buttonTargetValue, "buttonTargetValue");
            this.buttonTargetValue.Name = "buttonTargetValue";
            this.buttonTargetValue.Click += new System.EventHandler(this.buttonTargetValue_Click);
            // 
            // buttonPreValue
            // 
            resources.ApplyResources(this.buttonPreValue, "buttonPreValue");
            this.buttonPreValue.Name = "buttonPreValue";
            this.buttonPreValue.Click += new System.EventHandler(this.buttonPreValue_Click);
            // 
            // buttonExValue
            // 
            resources.ApplyResources(this.buttonExValue, "buttonExValue");
            this.buttonExValue.Name = "buttonExValue";
            this.buttonExValue.Click += new System.EventHandler(this.buttonExValue_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.buttonColorPredictionPower);
            this.groupBox2.Controls.Add(this.buttonColorExcessedPower);
            this.groupBox2.Controls.Add(this.buttonColorTargetPower);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButtonPos2);
            this.groupBox5.Controls.Add(this.radioButtonPos1);
            this.groupBox5.Controls.Add(this.radioButtonPos0);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonExValue);
            this.groupBox4.Controls.Add(this.buttonPreValue);
            this.groupBox4.Controls.Add(this.buttonTargetValue);
            this.groupBox4.Controls.Add(this.buttonStatusValue);
            this.groupBox4.Controls.Add(this.buttonStatusFill);
            this.groupBox4.Controls.Add(this.buttonStatusBack);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // radioButtonPos2
            // 
            resources.ApplyResources(this.radioButtonPos2, "radioButtonPos2");
            this.radioButtonPos2.Name = "radioButtonPos2";
            this.radioButtonPos2.TabStop = true;
            this.radioButtonPos2.UseVisualStyleBackColor = true;
            // 
            // PropertyPageObjectDemandWindow
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDemandName);
            this.Name = "PropertyPageObjectDemandWindow";
            this.Load += new System.EventHandler(this.PropertyPageObjectDemandWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        public int nLineThick;

        private void vScrollBarThick_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            nLineThick = 21 - this.vScrollBarThick.Value;
            if (nLineThick < 1) nLineThick = 1;
            if (nLineThick > 20) nLineThick = 20;
            panelThick.Invalidate();
        }

        bool bMultiSelectThick = false;

        private void panelThick_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            int pos = panelThick.ClientRectangle.Top + panelThick.ClientSize.Height / 2 - nLineThick / 2;
            Color color;
            if (bMultiSelectThick) color = Color.DarkGray;
            else color = Color.Black;
            DrawClass.gcls(e.Graphics, panelThick.ClientRectangle.Left + 2, pos, panelThick.ClientRectangle.Right - 2, pos + nLineThick - 1, color);
        }

        void FillComboBox()
        {
            ArrayList blockDemand = new ArrayList();
            FUNCTION_BLOCK_DEMAND_CONTROL demand;
            int l;

            DemandControl.FunctionBlockDemandControlLoad(blockDemand);

            this.comboBoxDemandName.Items.Clear();

            for (l = 0; l < blockDemand.Count; l++)
            {
                demand = (FUNCTION_BLOCK_DEMAND_CONTROL)blockDemand[l];
                this.comboBoxDemandName.Items.Add(demand.title);
            }
        }

        private void PropertyPageObjectDemandWindow_Load(object sender, System.EventArgs e)
        {
            this.vScrollBarThick.Value = 21 - nLineThick;
            FillComboBox();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (FormConfigDemandControl.FunctionBlockDemandControl(this))
            {
                FillComboBox();
            }
        }

        public ObjectArgsDemandWindow ObjectArgs
        {
            set
            {
                this.comboBoxDemandName.Text = value.demand_name;
                this.nLineThick = value.thick_target;

                buttonColorExcessedPower.BackColor = value.IColorExcessedPower;
                buttonColorPredictionPower.BackColor = value.IColorPredictionPower;
                buttonColorTargetPower.BackColor = value.IColorTargetPower;
                buttonExValue.BackColor = value.IColorExValue;
                buttonPreValue.BackColor = value.IColorPreValue;
                buttonStatusBack.BackColor = value.IColorStatusBack;
                buttonStatusFill.BackColor = value.IColorStatusFill;
                buttonStatusValue.BackColor = value.IColorStatusValue;
                buttonTargetValue.BackColor = value.IColorTargetValue;

                multiSelectStatusBarPos.Set(value.nStatusBarPos);

            }
            get
            {
                ObjectArgsDemandWindow args = new ObjectArgsDemandWindow();

                args.demand_name = this.comboBoxDemandName.Text;
                args.thick_target = this.nLineThick;

                args.IColorExcessedPower = buttonColorExcessedPower.BackColor;
                args.IColorPredictionPower = buttonColorPredictionPower.BackColor;
                args.IColorTargetPower = buttonColorTargetPower.BackColor;
                args.IColorExValue = buttonExValue.BackColor;
                args.IColorPreValue = buttonPreValue.BackColor;
                args.IColorStatusBack = buttonStatusBack.BackColor;
                args.IColorStatusFill = buttonStatusFill.BackColor;
                args.IColorStatusValue = buttonStatusValue.BackColor;
                args.IColorTargetValue = buttonTargetValue.BackColor;

                multiSelectStatusBarPos.Get(ref args.nStatusBarPos);

                return args;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void buttonColorPredictionPower_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonColorPredictionPower.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColorPredictionPower.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonColorExcessedPower_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonColorExcessedPower.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColorExcessedPower.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonColorTargetPower_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonColorTargetPower.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonColorTargetPower.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonStatusBack_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonStatusBack.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonStatusBack.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonStatusFill_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonStatusFill.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonStatusFill.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonStatusValue_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonStatusValue.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonStatusValue.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonTargetValue_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonTargetValue.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonTargetValue.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonPreValue_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonPreValue.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonPreValue.BackColor = dialog.GetSelectedColor();
            }
        }

        private void buttonExValue_Click(object sender, EventArgs e)
        {
            FormColorDialog dialog = new FormColorDialog();

            dialog.SetSelectedColor(this.buttonExValue.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.buttonExValue.BackColor = dialog.GetSelectedColor();
            }
        }
    }
}
