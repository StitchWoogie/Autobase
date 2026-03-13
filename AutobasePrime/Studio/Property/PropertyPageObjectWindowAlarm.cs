using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using Studio.Property;
using System.Collections.Generic;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectWindowAlarm.
	/// </summary>
	public class PropertyPageObjectWindowAlarm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonIncludeMethod0;
		private System.Windows.Forms.RadioButton radioButtonIncludeMethod1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonIncludeMethod2;
		private System.Windows.Forms.CheckBox checkBoxDisplayTitle;
		private System.Windows.Forms.CheckBox checkBoxDisplayColumnHeader;
        private CheckBox checkBoxScrollVert;
        private CheckBox checkBoxScrollHorz;
        private RadioButton radioButtonIncludeMethod3;
        private Button buttonColumn;
        private RadioButton radioButtonIncludeMethod4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectWindowAlarm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectWindowAlarm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonIncludeMethod4 = new System.Windows.Forms.RadioButton();
            this.radioButtonIncludeMethod3 = new System.Windows.Forms.RadioButton();
            this.radioButtonIncludeMethod2 = new System.Windows.Forms.RadioButton();
            this.radioButtonIncludeMethod1 = new System.Windows.Forms.RadioButton();
            this.radioButtonIncludeMethod0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxScrollVert = new System.Windows.Forms.CheckBox();
            this.checkBoxScrollHorz = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayColumnHeader = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayTitle = new System.Windows.Forms.CheckBox();
            this.buttonColumn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonIncludeMethod4);
            this.groupBox1.Controls.Add(this.radioButtonIncludeMethod3);
            this.groupBox1.Controls.Add(this.radioButtonIncludeMethod2);
            this.groupBox1.Controls.Add(this.radioButtonIncludeMethod1);
            this.groupBox1.Controls.Add(this.radioButtonIncludeMethod0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonIncludeMethod4
            // 
            this.radioButtonIncludeMethod4.AccessibleDescription = null;
            this.radioButtonIncludeMethod4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonIncludeMethod4, "radioButtonIncludeMethod4");
            this.radioButtonIncludeMethod4.BackgroundImage = null;
            this.radioButtonIncludeMethod4.Font = null;
            this.radioButtonIncludeMethod4.Name = "radioButtonIncludeMethod4";
            // 
            // radioButtonIncludeMethod3
            // 
            this.radioButtonIncludeMethod3.AccessibleDescription = null;
            this.radioButtonIncludeMethod3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonIncludeMethod3, "radioButtonIncludeMethod3");
            this.radioButtonIncludeMethod3.BackgroundImage = null;
            this.radioButtonIncludeMethod3.Font = null;
            this.radioButtonIncludeMethod3.Name = "radioButtonIncludeMethod3";
            // 
            // radioButtonIncludeMethod2
            // 
            this.radioButtonIncludeMethod2.AccessibleDescription = null;
            this.radioButtonIncludeMethod2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonIncludeMethod2, "radioButtonIncludeMethod2");
            this.radioButtonIncludeMethod2.BackgroundImage = null;
            this.radioButtonIncludeMethod2.Font = null;
            this.radioButtonIncludeMethod2.Name = "radioButtonIncludeMethod2";
            this.radioButtonIncludeMethod2.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // radioButtonIncludeMethod1
            // 
            this.radioButtonIncludeMethod1.AccessibleDescription = null;
            this.radioButtonIncludeMethod1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonIncludeMethod1, "radioButtonIncludeMethod1");
            this.radioButtonIncludeMethod1.BackgroundImage = null;
            this.radioButtonIncludeMethod1.Font = null;
            this.radioButtonIncludeMethod1.Name = "radioButtonIncludeMethod1";
            // 
            // radioButtonIncludeMethod0
            // 
            this.radioButtonIncludeMethod0.AccessibleDescription = null;
            this.radioButtonIncludeMethod0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonIncludeMethod0, "radioButtonIncludeMethod0");
            this.radioButtonIncludeMethod0.BackgroundImage = null;
            this.radioButtonIncludeMethod0.Font = null;
            this.radioButtonIncludeMethod0.Name = "radioButtonIncludeMethod0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxScrollVert);
            this.groupBox2.Controls.Add(this.checkBoxScrollHorz);
            this.groupBox2.Controls.Add(this.checkBoxDisplayColumnHeader);
            this.groupBox2.Controls.Add(this.checkBoxDisplayTitle);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxScrollVert
            // 
            this.checkBoxScrollVert.AccessibleDescription = null;
            this.checkBoxScrollVert.AccessibleName = null;
            resources.ApplyResources(this.checkBoxScrollVert, "checkBoxScrollVert");
            this.checkBoxScrollVert.BackgroundImage = null;
            this.checkBoxScrollVert.Font = null;
            this.checkBoxScrollVert.Name = "checkBoxScrollVert";
            // 
            // checkBoxScrollHorz
            // 
            this.checkBoxScrollHorz.AccessibleDescription = null;
            this.checkBoxScrollHorz.AccessibleName = null;
            resources.ApplyResources(this.checkBoxScrollHorz, "checkBoxScrollHorz");
            this.checkBoxScrollHorz.BackgroundImage = null;
            this.checkBoxScrollHorz.Font = null;
            this.checkBoxScrollHorz.Name = "checkBoxScrollHorz";
            // 
            // checkBoxDisplayColumnHeader
            // 
            this.checkBoxDisplayColumnHeader.AccessibleDescription = null;
            this.checkBoxDisplayColumnHeader.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDisplayColumnHeader, "checkBoxDisplayColumnHeader");
            this.checkBoxDisplayColumnHeader.BackgroundImage = null;
            this.checkBoxDisplayColumnHeader.Font = null;
            this.checkBoxDisplayColumnHeader.Name = "checkBoxDisplayColumnHeader";
            this.checkBoxDisplayColumnHeader.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBoxDisplayTitle
            // 
            this.checkBoxDisplayTitle.AccessibleDescription = null;
            this.checkBoxDisplayTitle.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDisplayTitle, "checkBoxDisplayTitle");
            this.checkBoxDisplayTitle.BackgroundImage = null;
            this.checkBoxDisplayTitle.Font = null;
            this.checkBoxDisplayTitle.Name = "checkBoxDisplayTitle";
            // 
            // buttonColumn
            // 
            this.buttonColumn.AccessibleDescription = null;
            this.buttonColumn.AccessibleName = null;
            resources.ApplyResources(this.buttonColumn, "buttonColumn");
            this.buttonColumn.BackgroundImage = null;
            this.buttonColumn.Font = null;
            this.buttonColumn.Name = "buttonColumn";
            this.buttonColumn.UseVisualStyleBackColor = true;
            this.buttonColumn.Click += new System.EventHandler(this.buttonColumn_Click);
            // 
            // PropertyPageObjectWindowAlarm
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.buttonColumn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectWindowAlarm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void radioButton3_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		private void checkBox2_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

        void FillListBox()
        {
            //this.checkedListBoxColumn.Items.Add("경보날짜");
            //this.checkedListBoxColumn.Items.Add("경보시간");
            //this.checkedListBoxColumn.Items.Add("태그명");
            //this.checkedListBoxColumn.Items.Add("태그설명");
            //this.checkedListBoxColumn.Items.Add("경보내용");
            //this.checkedListBoxColumn.Items.Add("우선순위");
        }

		public ObjectArgsWindowAlarm ObjectArgs 
		{
			set 
			{
				this.radioButtonIncludeMethod0.Checked = (value.cIncludeMethod == 0);
				this.radioButtonIncludeMethod1.Checked = (value.cIncludeMethod == 1);
				this.radioButtonIncludeMethod2.Checked = (value.cIncludeMethod == 2);
                this.radioButtonIncludeMethod3.Checked = (value.cIncludeMethod == 3);
                this.radioButtonIncludeMethod4.Checked = (value.cIncludeMethod == 4);

				this.checkBoxDisplayTitle.Checked = (value.bFlagWindowCaption == 1);
				this.checkBoxDisplayColumnHeader.Checked = (value.bFlagUseColumnHeader == 1);

                this.checkBoxScrollHorz.Checked = value.bFlagUseScrollHorz;
                this.checkBoxScrollVert.Checked = value.bFlagUseScrollVert;

                arrayColumns = value.arrayColumns;

                FillListBox();
			}
			get 
			{
				ObjectArgsWindowAlarm args = new ObjectArgsWindowAlarm();

				if(this.radioButtonIncludeMethod0.Checked)		args.cIncludeMethod = 0;
				else if(this.radioButtonIncludeMethod1.Checked)	args.cIncludeMethod = 1;
				else if(this.radioButtonIncludeMethod2.Checked)	args.cIncludeMethod = 2;
                else if (this.radioButtonIncludeMethod3.Checked) args.cIncludeMethod = 3;
                else if (this.radioButtonIncludeMethod4.Checked) args.cIncludeMethod = 4;
				else											args.cIncludeMethod = 0;

				args.bFlagWindowCaption = this.checkBoxDisplayTitle.Checked ? (sbyte)1 : (sbyte)0;
				args.bFlagUseColumnHeader = this.checkBoxDisplayColumnHeader.Checked ? (sbyte)1 : (sbyte)0;

                args.bFlagUseScrollHorz = this.checkBoxScrollHorz.Checked;
                args.bFlagUseScrollVert = this.checkBoxScrollVert.Checked;

                args.arrayColumns = this.arrayColumns;

				return args;
			}
		}

        List<AlarmEventColumn> arrayColumns;

        private void buttonColumn_Click(object sender, EventArgs e)
        {
            PropertyPageObjectWindowAlarmColumn dialog = new PropertyPageObjectWindowAlarmColumn();

            dialog.arrayColumns = arrayColumns;
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                arrayColumns = dialog.arrayColumns;
            }
        }
	}
}
