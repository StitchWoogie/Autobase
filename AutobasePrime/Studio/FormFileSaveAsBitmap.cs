using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using AutoLibLocal;
using NetTools.OldDefine;
using NetTools;
using System.Drawing.Drawing2D;

namespace Studio
{
	/// <summary>
	/// Summary description for FormFileSaveAsBitmap.
	/// </summary>
	public class FormFileSaveAsBitmap : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxHorz;
		private System.Windows.Forms.TextBox textBoxVert;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBoxSaveZone;
		private System.Windows.Forms.RadioButton radioButtonZone0;
		private System.Windows.Forms.RadioButton radioButtonZone1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox checkBoxSaveOnlySelectedObject;

		ObjectRoot objRoot;
        private CheckBox checkBoxSaveAsTransparentBackground;
        private GroupBox groupBox2;
        private NumericUpDown numericUpDownMarginBottom;
        private Label label6;
        private NumericUpDown numericUpDownMarginRight;
        private Label label5;
        private NumericUpDown numericUpDownMarginTop;
        private Label label4;
        private NumericUpDown numericUpDownMarginLeft;
        private Label label3;
        private CheckBox checkBoxUseSameMargin;
        private CheckBox checkBoxUseSmooth;
		FormEditGraphic formParent;

		public FormFileSaveAsBitmap(FormEditGraphic form, ObjectRoot obj)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			formParent = form;
			objRoot = obj;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFileSaveAsBitmap));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxVert = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxHorz = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxSaveZone = new System.Windows.Forms.GroupBox();
            this.radioButtonZone1 = new System.Windows.Forms.RadioButton();
            this.radioButtonZone0 = new System.Windows.Forms.RadioButton();
            this.checkBoxSaveOnlySelectedObject = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveAsTransparentBackground = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseSameMargin = new System.Windows.Forms.CheckBox();
            this.numericUpDownMarginBottom = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownMarginRight = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownMarginTop = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownMarginLeft = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxUseSmooth = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBoxSaveZone.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginLeft)).BeginInit();
            this.SuspendLayout();
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
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxVert);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxHorz);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxVert
            // 
            this.textBoxVert.AccessibleDescription = null;
            this.textBoxVert.AccessibleName = null;
            resources.ApplyResources(this.textBoxVert, "textBoxVert");
            this.textBoxVert.BackgroundImage = null;
            this.textBoxVert.Font = null;
            this.textBoxVert.Name = "textBoxVert";
            this.textBoxVert.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // textBoxHorz
            // 
            this.textBoxHorz.AccessibleDescription = null;
            this.textBoxHorz.AccessibleName = null;
            resources.ApplyResources(this.textBoxHorz, "textBoxHorz");
            this.textBoxHorz.BackgroundImage = null;
            this.textBoxHorz.Font = null;
            this.textBoxHorz.Name = "textBoxHorz";
            this.textBoxHorz.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // groupBoxSaveZone
            // 
            this.groupBoxSaveZone.AccessibleDescription = null;
            this.groupBoxSaveZone.AccessibleName = null;
            resources.ApplyResources(this.groupBoxSaveZone, "groupBoxSaveZone");
            this.groupBoxSaveZone.BackgroundImage = null;
            this.groupBoxSaveZone.Controls.Add(this.radioButtonZone1);
            this.groupBoxSaveZone.Controls.Add(this.radioButtonZone0);
            this.groupBoxSaveZone.Font = null;
            this.groupBoxSaveZone.Name = "groupBoxSaveZone";
            this.groupBoxSaveZone.TabStop = false;
            // 
            // radioButtonZone1
            // 
            this.radioButtonZone1.AccessibleDescription = null;
            this.radioButtonZone1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonZone1, "radioButtonZone1");
            this.radioButtonZone1.BackgroundImage = null;
            this.radioButtonZone1.Font = null;
            this.radioButtonZone1.Name = "radioButtonZone1";
            this.radioButtonZone1.CheckedChanged += new System.EventHandler(this.radioButtonZone1_CheckedChanged);
            // 
            // radioButtonZone0
            // 
            this.radioButtonZone0.AccessibleDescription = null;
            this.radioButtonZone0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonZone0, "radioButtonZone0");
            this.radioButtonZone0.BackgroundImage = null;
            this.radioButtonZone0.Checked = true;
            this.radioButtonZone0.Font = null;
            this.radioButtonZone0.Name = "radioButtonZone0";
            this.radioButtonZone0.TabStop = true;
            this.radioButtonZone0.CheckedChanged += new System.EventHandler(this.radioButtonZone0_CheckedChanged);
            // 
            // checkBoxSaveOnlySelectedObject
            // 
            this.checkBoxSaveOnlySelectedObject.AccessibleDescription = null;
            this.checkBoxSaveOnlySelectedObject.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveOnlySelectedObject, "checkBoxSaveOnlySelectedObject");
            this.checkBoxSaveOnlySelectedObject.BackgroundImage = null;
            this.checkBoxSaveOnlySelectedObject.Font = null;
            this.checkBoxSaveOnlySelectedObject.Name = "checkBoxSaveOnlySelectedObject";
            this.checkBoxSaveOnlySelectedObject.CheckedChanged += new System.EventHandler(this.checkBoxSaveOnlySelectedObject_CheckedChanged);
            // 
            // checkBoxSaveAsTransparentBackground
            // 
            this.checkBoxSaveAsTransparentBackground.AccessibleDescription = null;
            this.checkBoxSaveAsTransparentBackground.AccessibleName = null;
            resources.ApplyResources(this.checkBoxSaveAsTransparentBackground, "checkBoxSaveAsTransparentBackground");
            this.checkBoxSaveAsTransparentBackground.BackgroundImage = null;
            this.checkBoxSaveAsTransparentBackground.Font = null;
            this.checkBoxSaveAsTransparentBackground.Name = "checkBoxSaveAsTransparentBackground";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxUseSameMargin);
            this.groupBox2.Controls.Add(this.numericUpDownMarginBottom);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numericUpDownMarginRight);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericUpDownMarginTop);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numericUpDownMarginLeft);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxUseSameMargin
            // 
            this.checkBoxUseSameMargin.AccessibleDescription = null;
            this.checkBoxUseSameMargin.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseSameMargin, "checkBoxUseSameMargin");
            this.checkBoxUseSameMargin.BackgroundImage = null;
            this.checkBoxUseSameMargin.Checked = true;
            this.checkBoxUseSameMargin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseSameMargin.Font = null;
            this.checkBoxUseSameMargin.Name = "checkBoxUseSameMargin";
            this.checkBoxUseSameMargin.UseVisualStyleBackColor = true;
            this.checkBoxUseSameMargin.CheckedChanged += new System.EventHandler(this.checkBoxUseSameMargin_CheckedChanged);
            // 
            // numericUpDownMarginBottom
            // 
            this.numericUpDownMarginBottom.AccessibleDescription = null;
            this.numericUpDownMarginBottom.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownMarginBottom, "numericUpDownMarginBottom");
            this.numericUpDownMarginBottom.Font = null;
            this.numericUpDownMarginBottom.Name = "numericUpDownMarginBottom";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // numericUpDownMarginRight
            // 
            this.numericUpDownMarginRight.AccessibleDescription = null;
            this.numericUpDownMarginRight.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownMarginRight, "numericUpDownMarginRight");
            this.numericUpDownMarginRight.Font = null;
            this.numericUpDownMarginRight.Name = "numericUpDownMarginRight";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // numericUpDownMarginTop
            // 
            this.numericUpDownMarginTop.AccessibleDescription = null;
            this.numericUpDownMarginTop.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownMarginTop, "numericUpDownMarginTop");
            this.numericUpDownMarginTop.Font = null;
            this.numericUpDownMarginTop.Name = "numericUpDownMarginTop";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // numericUpDownMarginLeft
            // 
            this.numericUpDownMarginLeft.AccessibleDescription = null;
            this.numericUpDownMarginLeft.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownMarginLeft, "numericUpDownMarginLeft");
            this.numericUpDownMarginLeft.Font = null;
            this.numericUpDownMarginLeft.Name = "numericUpDownMarginLeft";
            this.numericUpDownMarginLeft.ValueChanged += new System.EventHandler(this.numericUpDownMarginLeft_ValueChanged);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // checkBoxUseSmooth
            // 
            this.checkBoxUseSmooth.AccessibleDescription = null;
            this.checkBoxUseSmooth.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseSmooth, "checkBoxUseSmooth");
            this.checkBoxUseSmooth.BackgroundImage = null;
            this.checkBoxUseSmooth.Font = null;
            this.checkBoxUseSmooth.Name = "checkBoxUseSmooth";
            // 
            // FormFileSaveAsBitmap
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxUseSmooth);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxSaveAsTransparentBackground);
            this.Controls.Add(this.checkBoxSaveOnlySelectedObject);
            this.Controls.Add(this.groupBoxSaveZone);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFileSaveAsBitmap";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormFileSaveAsBitmap_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxSaveZone.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMarginLeft)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        void SetPictureSize()
		{
			int radio = 0;

			if(this.checkBoxSaveOnlySelectedObject.Checked && group != null) 
			{
				int x=0, y=0;
				group.GetGroupRealSize(ref x, ref y);
				this.textBoxHorz.Text = x.ToString();
				this.textBoxVert.Text = y.ToString();
			}
			else 
			{
				if(this.radioButtonZone0.Checked)		radio = 0;
				else if(this.radioButtonZone1.Checked)	radio = 1;
				else									radio = 0;

				if(radio == 0)	
				{
					int x, y;
					objRoot.GetModuleSize(out x, out y);
					this.textBoxHorz.Text = x.ToString();
					this.textBoxVert.Text = y.ToString();
				}
				else 
				{
					int x=0, y=0;
					objRoot.GetModuleSizeByObject(ref x, ref y);
					this.textBoxHorz.Text = x.ToString();
					this.textBoxVert.Text = y.ToString();
				}
			}
		}

		ObjectGroup group;

        static bool bTransparentBackground = false;

		private void FormFileSaveAsBitmap_Load(object sender, System.EventArgs e)
		{
			group = ClassStudioEdit.SelectedObjectToGroup(formParent, true);
			SetPictureSize();

			this.checkBoxSaveOnlySelectedObject.Checked = (group != null);
            this.checkBoxSaveOnlySelectedObject.Enabled = (group != null);

            this.checkBoxSaveAsTransparentBackground.Checked = bTransparentBackground;
            this.checkBoxUseSmooth.Checked = objRoot.cSmoothingMode == 1;

            if (group != null)
            {
                int thick = 0;

                group.GetMaxBorderThick(ref thick);

                this.numericUpDownMarginLeft.Value = thick / 2;
            }

            EnableDisable();
            EnableMargin();
		}

        void EnableDisable()
        {
            if (this.checkBoxSaveOnlySelectedObject.Checked && group != null)
            {
                this.groupBoxSaveZone.Enabled = false;
            }
            else
            {
                this.groupBoxSaveZone.Enabled = true;
            }
        }

		private void radioButtonZone0_CheckedChanged(object sender, System.EventArgs e)
		{
			SetPictureSize();
		}

		private void radioButtonZone1_CheckedChanged(object sender, System.EventArgs e)
		{
			SetPictureSize();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();

			dialog.InitialDirectory = String.Format("{0}\\graphic", TotalConfig.sDirWorkProject);
            dialog.Filter = "Bitmap Files (*.png)|*.png|Bitmap Files (*.bmp)|*.bmp";

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
                bool ispng = (String.Compare(System.IO.Path.GetExtension(dialog.FileName), ".png", true) == 0);

				int width = ConvertTool.ToInt32(this.textBoxHorz.Text);
				int height = ConvertTool.ToInt32(this.textBoxVert.Text);

                int gab_x1 = (int)numericUpDownMarginLeft.Value;
                int gab_y1 = (int)numericUpDownMarginTop.Value;
                int gab_x2 = (int)numericUpDownMarginRight.Value;
                int gab_y2 = (int)numericUpDownMarginBottom.Value;

                int bwidth = width;     // 비트맵의 크기
                int bheight = height;   // 비트맵의 크기

                if (this.checkBoxSaveOnlySelectedObject.Checked)
                {
                    bwidth += gab_x1 + gab_x2;
                    bheight += gab_y1 + gab_y2;
                }

				Bitmap bitmap = new Bitmap(bwidth, bheight, ispng ? System.Drawing.Imaging.PixelFormat.Format32bppArgb : System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				Graphics g = Graphics.FromImage(bitmap);

                if (checkBoxUseSmooth.Checked)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
                else
                {
                    g.SmoothingMode = SmoothingMode.HighSpeed;
                    g.CompositingMode = CompositingMode.SourceOver;
                }

				Rectangle rect = new Rectangle(0, 0, bwidth, bheight);
				Rectangle clip = new Rectangle(0, 0, bwidth, bheight);
				
				// 선택한 영역만 저장
				if(this.checkBoxSaveOnlySelectedObject.Checked)
				{
					RECT r = new RECT();

					r.left = gab_x1;
					r.top  = gab_y1;
					r.right = r.left+width-1;
					r.bottom = r.top+height-1;

                    group.SetBasePoint(0, 0);
                    group.SetOpticRate(100);

					group.UpdateZone(formParent, r.left, r.top, r.right, r.bottom);
					group.SetZoneAtPercent100(group.sizeGroup, r);

					Color color = formParent.workThis.obj.GetBackGroundColor();

                    if (checkBoxSaveAsTransparentBackground.Checked && ispng)
                        g.FillRectangle(new SolidBrush(Color.Transparent), rect);
                    else
                    {
                        g.FillRectangle(new SolidBrush(color), rect);
                    }

					group.Display(g, rect, 0, 0);
				}
				else 
				{
                    Point point = new Point(0, 0);
					objRoot.Display(g, rect, clip, point);
				}

				try 
				{
                    if(ispng)
					    bitmap.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    else
                        bitmap.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
				}
				catch (Exception exp)
				{
					MessageBox.Show(exp.Message, "Can't save the file");
				}
			}

            bTransparentBackground = this.checkBoxSaveAsTransparentBackground.Checked;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void checkBoxSaveOnlySelectedObject_CheckedChanged(object sender, System.EventArgs e)
		{
            EnableDisable();
			SetPictureSize();
		}

        void EnableMargin()
        {
            bool flag = this.checkBoxUseSameMargin.Checked;

            this.numericUpDownMarginTop.Enabled = !flag;
            this.numericUpDownMarginRight.Enabled = !flag;
            this.numericUpDownMarginBottom.Enabled = !flag;
        }

        void SetAsLeftMargin()
        {
            decimal val = this.numericUpDownMarginLeft.Value;
            this.numericUpDownMarginTop.Value = val;
            this.numericUpDownMarginRight.Value = val;
            this.numericUpDownMarginBottom.Value = val;
        }

        private void checkBoxUseSameMargin_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxUseSameMargin.Checked)
            {
                SetAsLeftMargin();
            }

            EnableMargin();
        }

        private void numericUpDownMarginLeft_ValueChanged(object sender, EventArgs e)
        {
            if (this.checkBoxUseSameMargin.Checked)
            {
                SetAsLeftMargin();
            }
        }
	}
}
