using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for FormConfigAlarmPriority.
	/// </summary>
	public class FormConfigAlarmPriority : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBoxPrinter;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonScreen0;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBoxLinePrinter;
		private System.Windows.Forms.RadioButton radioButtonScreen1;
		private System.Windows.Forms.RadioButton radioButtonScreen2;
		private System.Windows.Forms.RadioButton radioButtonScreen3;
		private System.Windows.Forms.RadioButton radioButtonScreen4;
		private System.Windows.Forms.RadioButton radioButtonSound4;
		private System.Windows.Forms.RadioButton radioButtonSound3;
		private System.Windows.Forms.RadioButton radioButtonSound2;
		private System.Windows.Forms.RadioButton radioButtonSound1;
		private System.Windows.Forms.RadioButton radioButtonSound0;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigAlarmPriority()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigAlarmPriority));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxPrinter = new System.Windows.Forms.GroupBox();
            this.checkBoxLinePrinter = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonScreen4 = new System.Windows.Forms.RadioButton();
            this.radioButtonScreen3 = new System.Windows.Forms.RadioButton();
            this.radioButtonScreen2 = new System.Windows.Forms.RadioButton();
            this.radioButtonScreen1 = new System.Windows.Forms.RadioButton();
            this.radioButtonScreen0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonSound4 = new System.Windows.Forms.RadioButton();
            this.radioButtonSound3 = new System.Windows.Forms.RadioButton();
            this.radioButtonSound2 = new System.Windows.Forms.RadioButton();
            this.radioButtonSound1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSound0 = new System.Windows.Forms.RadioButton();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.groupBoxPrinter.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            // 
            // groupBoxPrinter
            // 
            this.groupBoxPrinter.Controls.Add(this.checkBoxLinePrinter);
            resources.ApplyResources(this.groupBoxPrinter, "groupBoxPrinter");
            this.groupBoxPrinter.Name = "groupBoxPrinter";
            this.groupBoxPrinter.TabStop = false;
            // 
            // checkBoxLinePrinter
            // 
            resources.ApplyResources(this.checkBoxLinePrinter, "checkBoxLinePrinter");
            this.checkBoxLinePrinter.Name = "checkBoxLinePrinter";
            this.checkBoxLinePrinter.CheckedChanged += new System.EventHandler(this.checkBoxLinePrinter_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonScreen4);
            this.groupBox2.Controls.Add(this.radioButtonScreen3);
            this.groupBox2.Controls.Add(this.radioButtonScreen2);
            this.groupBox2.Controls.Add(this.radioButtonScreen1);
            this.groupBox2.Controls.Add(this.radioButtonScreen0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonScreen4
            // 
            resources.ApplyResources(this.radioButtonScreen4, "radioButtonScreen4");
            this.radioButtonScreen4.Name = "radioButtonScreen4";
            this.radioButtonScreen4.CheckedChanged += new System.EventHandler(this.radioButtonScreen4_CheckedChanged);
            // 
            // radioButtonScreen3
            // 
            resources.ApplyResources(this.radioButtonScreen3, "radioButtonScreen3");
            this.radioButtonScreen3.Name = "radioButtonScreen3";
            this.radioButtonScreen3.CheckedChanged += new System.EventHandler(this.radioButtonScreen3_CheckedChanged);
            // 
            // radioButtonScreen2
            // 
            resources.ApplyResources(this.radioButtonScreen2, "radioButtonScreen2");
            this.radioButtonScreen2.Name = "radioButtonScreen2";
            this.radioButtonScreen2.CheckedChanged += new System.EventHandler(this.radioButtonScreen2_CheckedChanged);
            // 
            // radioButtonScreen1
            // 
            resources.ApplyResources(this.radioButtonScreen1, "radioButtonScreen1");
            this.radioButtonScreen1.Name = "radioButtonScreen1";
            this.radioButtonScreen1.CheckedChanged += new System.EventHandler(this.radioButtonScreen1_CheckedChanged);
            // 
            // radioButtonScreen0
            // 
            resources.ApplyResources(this.radioButtonScreen0, "radioButtonScreen0");
            this.radioButtonScreen0.Name = "radioButtonScreen0";
            this.radioButtonScreen0.CheckedChanged += new System.EventHandler(this.radioButtonScreen0_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonSound4);
            this.groupBox3.Controls.Add(this.radioButtonSound3);
            this.groupBox3.Controls.Add(this.radioButtonSound2);
            this.groupBox3.Controls.Add(this.radioButtonSound1);
            this.groupBox3.Controls.Add(this.radioButtonSound0);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // radioButtonSound4
            // 
            resources.ApplyResources(this.radioButtonSound4, "radioButtonSound4");
            this.radioButtonSound4.Name = "radioButtonSound4";
            this.radioButtonSound4.CheckedChanged += new System.EventHandler(this.radioButtonSound4_CheckedChanged);
            // 
            // radioButtonSound3
            // 
            resources.ApplyResources(this.radioButtonSound3, "radioButtonSound3");
            this.radioButtonSound3.Name = "radioButtonSound3";
            this.radioButtonSound3.CheckedChanged += new System.EventHandler(this.radioButtonSound3_CheckedChanged);
            // 
            // radioButtonSound2
            // 
            resources.ApplyResources(this.radioButtonSound2, "radioButtonSound2");
            this.radioButtonSound2.Name = "radioButtonSound2";
            this.radioButtonSound2.CheckedChanged += new System.EventHandler(this.radioButtonSound2_CheckedChanged);
            // 
            // radioButtonSound1
            // 
            resources.ApplyResources(this.radioButtonSound1, "radioButtonSound1");
            this.radioButtonSound1.Name = "radioButtonSound1";
            this.radioButtonSound1.CheckedChanged += new System.EventHandler(this.radioButtonSound1_CheckedChanged);
            // 
            // radioButtonSound0
            // 
            resources.ApplyResources(this.radioButtonSound0, "radioButtonSound0");
            this.radioButtonSound0.Name = "radioButtonSound0";
            this.radioButtonSound0.CheckedChanged += new System.EventHandler(this.radioButtonSound0_CheckedChanged);
            // 
            // m_list
            // 
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.m_list_MouseUp);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // FormConfigAlarmPriority
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxPrinter);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigAlarmPriority";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigAlarmPriority_Load);
            this.groupBoxPrinter.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigAlarmPriority_Load(object sender, System.EventArgs e)
		{
			AlarmPriority.Load();
	
			// TODO: Add extra initialization here
			string buf;
			int i;
			for(i = 0; i < 1000; i++) 
			{
				buf = String.Format("{0:000}", i);
				ListViewItem item = new ListViewItem(buf);
				m_list.Items.Add(item);
			}

            EnableDisableByListSelected();
		}

		void UpdatePriorityDisplay()
		{
			sbyte bPrint = -1;
			sbyte cScreen = -1;
			sbyte cSound = -1;
			int i;

			for(i = 0; i < m_list.Items.Count; i++) 
			{
				if(m_list.Items[i].Selected) 
				{
					if(bPrint == -1) 
					{	// not init
						bPrint = AlarmPriority.alarmPriority[i].bLinePrint;
					}
					else if(bPrint == 2) 
					{	// selection multiple
						// not work	
					}
					else 
					{	// 0 or 1
						if(bPrint != AlarmPriority.alarmPriority[i].bLinePrint) 
						{
							bPrint = 2;	
						}
					}

					if(cScreen == -1) 
					{	// not init
						cScreen = AlarmPriority.alarmPriority[i].cScreen;
					}
					else if(cScreen == -2) 
					{	// selection multiple
						// not work	
					}
					else 
					{	// 0 or 1
						if(cScreen != AlarmPriority.alarmPriority[i].cScreen) 
						{
							cScreen = -2;	
						}
					}

					if(cSound == -1) 
					{	// not init
						cSound = AlarmPriority.alarmPriority[i].cSound;
					}
					else if(cSound == -2) 
					{	// selection multiple
						// not work	
					}
					else 
					{	// 0 or 1
						if(cSound != AlarmPriority.alarmPriority[i].cSound) 
						{
							cSound = -2;	
						}
					}
				}
			}

			if(bPrint == 0) 
			{
				this.checkBoxLinePrinter.CheckState = CheckState.Unchecked;
			}
			else if(bPrint == 1) 
			{
				this.checkBoxLinePrinter.CheckState = CheckState.Checked;
			}
			else if(bPrint == 2) 
			{
				this.checkBoxLinePrinter.CheckState = CheckState.Indeterminate;
			}
			else 
			{	// list not selected
		
			}

			this.radioButtonScreen0.Checked = (cScreen == 0);
			this.radioButtonScreen1.Checked = (cScreen == 1);
			this.radioButtonScreen2.Checked = (cScreen == 2);
			this.radioButtonScreen3.Checked = (cScreen == 3);
			this.radioButtonScreen4.Checked = (cScreen == 4);

			this.radioButtonSound0.Checked = (cSound == 0);
			this.radioButtonSound1.Checked = (cSound == 1);
			this.radioButtonSound2.Checked = (cSound == 2);
			this.radioButtonSound3.Checked = (cSound == 3);
			this.radioButtonSound4.Checked = (cSound == 4);
		}

		private void checkBoxLinePrinter_CheckedChanged(object sender, System.EventArgs e)
		{
			if(bUpdating)	return;
			CheckState flag = this.checkBoxLinePrinter.CheckState;
			int i;

			if(flag == CheckState.Indeterminate) 
			{
				this.checkBoxLinePrinter.CheckState = CheckState.Unchecked;
				flag = CheckState.Unchecked;
			}

			for(i = 0; i < 1000; i++) 
			{
				if(m_list.Items[i].Selected)
					AlarmPriority.alarmPriority[i].bLinePrint = (sbyte)flag;
			}
		}

		void ScreenChange()
		{
			if(bUpdating)	return;

			sbyte cScreen;
			
			if(this.radioButtonScreen0.Checked)			cScreen = 0;
			else if(this.radioButtonScreen1.Checked)	cScreen = 1;
			else if(this.radioButtonScreen2.Checked)	cScreen = 2;
			else if(this.radioButtonScreen3.Checked)	cScreen = 3;
			else if(this.radioButtonScreen4.Checked)	cScreen = 4;
			else										cScreen = 0;

			int i;

			for(i = 0; i < 1000; i++) 
			{
				if(this.m_list.Items[i].Selected)
					AlarmPriority.alarmPriority[i].cScreen = cScreen;
			}
		}

		private void radioButtonScreen0_CheckedChanged(object sender, System.EventArgs e)
		{
			ScreenChange();
		}

		private void radioButtonScreen1_CheckedChanged(object sender, System.EventArgs e)
		{
			ScreenChange();
		}

		private void radioButtonScreen2_CheckedChanged(object sender, System.EventArgs e)
		{
			ScreenChange();
		}

		private void radioButtonScreen3_CheckedChanged(object sender, System.EventArgs e)
		{
			ScreenChange();
		}

		private void radioButtonScreen4_CheckedChanged(object sender, System.EventArgs e)
		{
			ScreenChange();
		}

		void SoundChange()
		{
			if(bUpdating)	return;

			sbyte cSound;

			if(this.radioButtonSound0.Checked)		cSound = 0;
			else if(this.radioButtonSound1.Checked)	cSound = 1;
			else if(this.radioButtonSound2.Checked)	cSound = 2;
			else if(this.radioButtonSound3.Checked)	cSound = 3;
			else if(this.radioButtonSound4.Checked)	cSound = 4;
			else									cSound = 0;

			int i;

			for(i = 0; i < 1000; i++) 
			{
				if(m_list.Items[i].Selected)
					AlarmPriority.alarmPriority[i].cSound = cSound;
			}
		}

		private void radioButtonSound0_CheckedChanged(object sender, System.EventArgs e)
		{
			SoundChange();
		}

		private void radioButtonSound1_CheckedChanged(object sender, System.EventArgs e)
		{
			SoundChange();
		}

		private void radioButtonSound2_CheckedChanged(object sender, System.EventArgs e)
		{
			SoundChange();
		}

		private void radioButtonSound3_CheckedChanged(object sender, System.EventArgs e)
		{
			SoundChange();
		}

		private void radioButtonSound4_CheckedChanged(object sender, System.EventArgs e)
		{
			SoundChange();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			AlarmPriority.Save();
			DialogResult = DialogResult.OK;
			Close();
		}

		bool bUpdating = false;

        void EnableDisableByListSelected()
        {
            bool flag = this.m_list.SelectedItems.Count > 0;

            this.groupBoxPrinter.Enabled = flag;
            this.groupBox2.Enabled = flag;
            this.groupBox3.Enabled = flag;
        }

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            EnableDisableByListSelected();
			//bUpdating = true;
			//UpdatePriorityDisplay();
			//bUpdating = false;
		}

		private void m_list_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			bUpdating = true;
			UpdatePriorityDisplay();	
			bUpdating = false;
		}

		private void groupBox3_Enter(object sender, System.EventArgs e)
		{
		
		}
	}
}

