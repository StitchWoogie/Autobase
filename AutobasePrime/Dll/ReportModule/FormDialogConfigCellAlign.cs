using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormDialogConfigCellAlign.
	/// </summary>
	public class FormDialogConfigCellAlign : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonHorz0;
		private System.Windows.Forms.RadioButton radioButtonHorz1;
		private System.Windows.Forms.RadioButton radioButtonHorz2;
		private System.Windows.Forms.RadioButton radioButtonHorz3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonVert3;
		private System.Windows.Forms.RadioButton radioButtonVert2;
		private System.Windows.Forms.RadioButton radioButtonVert1;
		private System.Windows.Forms.RadioButton radioButtonVert0;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDialogConfigCellAlign()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogConfigCellAlign));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonHorz3 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorz2 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorz1 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorz0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonVert3 = new System.Windows.Forms.RadioButton();
            this.radioButtonVert2 = new System.Windows.Forms.RadioButton();
            this.radioButtonVert1 = new System.Windows.Forms.RadioButton();
            this.radioButtonVert0 = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
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
            this.groupBox1.Controls.Add(this.radioButtonHorz3);
            this.groupBox1.Controls.Add(this.radioButtonHorz2);
            this.groupBox1.Controls.Add(this.radioButtonHorz1);
            this.groupBox1.Controls.Add(this.radioButtonHorz0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonHorz3
            // 
            this.radioButtonHorz3.AccessibleDescription = null;
            this.radioButtonHorz3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorz3, "radioButtonHorz3");
            this.radioButtonHorz3.BackgroundImage = null;
            this.radioButtonHorz3.Font = null;
            this.radioButtonHorz3.Name = "radioButtonHorz3";
            // 
            // radioButtonHorz2
            // 
            this.radioButtonHorz2.AccessibleDescription = null;
            this.radioButtonHorz2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorz2, "radioButtonHorz2");
            this.radioButtonHorz2.BackgroundImage = null;
            this.radioButtonHorz2.Font = null;
            this.radioButtonHorz2.Name = "radioButtonHorz2";
            // 
            // radioButtonHorz1
            // 
            this.radioButtonHorz1.AccessibleDescription = null;
            this.radioButtonHorz1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorz1, "radioButtonHorz1");
            this.radioButtonHorz1.BackgroundImage = null;
            this.radioButtonHorz1.Font = null;
            this.radioButtonHorz1.Name = "radioButtonHorz1";
            // 
            // radioButtonHorz0
            // 
            this.radioButtonHorz0.AccessibleDescription = null;
            this.radioButtonHorz0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorz0, "radioButtonHorz0");
            this.radioButtonHorz0.BackgroundImage = null;
            this.radioButtonHorz0.Font = null;
            this.radioButtonHorz0.Name = "radioButtonHorz0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButtonVert3);
            this.groupBox2.Controls.Add(this.radioButtonVert2);
            this.groupBox2.Controls.Add(this.radioButtonVert1);
            this.groupBox2.Controls.Add(this.radioButtonVert0);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonVert3
            // 
            this.radioButtonVert3.AccessibleDescription = null;
            this.radioButtonVert3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVert3, "radioButtonVert3");
            this.radioButtonVert3.BackgroundImage = null;
            this.radioButtonVert3.Font = null;
            this.radioButtonVert3.Name = "radioButtonVert3";
            // 
            // radioButtonVert2
            // 
            this.radioButtonVert2.AccessibleDescription = null;
            this.radioButtonVert2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVert2, "radioButtonVert2");
            this.radioButtonVert2.BackgroundImage = null;
            this.radioButtonVert2.Font = null;
            this.radioButtonVert2.Name = "radioButtonVert2";
            this.radioButtonVert2.CheckedChanged += new System.EventHandler(this.radioButtonVert2_CheckedChanged);
            // 
            // radioButtonVert1
            // 
            this.radioButtonVert1.AccessibleDescription = null;
            this.radioButtonVert1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVert1, "radioButtonVert1");
            this.radioButtonVert1.BackgroundImage = null;
            this.radioButtonVert1.Font = null;
            this.radioButtonVert1.Name = "radioButtonVert1";
            // 
            // radioButtonVert0
            // 
            this.radioButtonVert0.AccessibleDescription = null;
            this.radioButtonVert0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVert0, "radioButtonVert0");
            this.radioButtonVert0.BackgroundImage = null;
            this.radioButtonVert0.Font = null;
            this.radioButtonVert0.Name = "radioButtonVert0";
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
            // FormDialogConfigCellAlign
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDialogConfigCellAlign";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetItem(FormReportChild form)
		{
			REPORT_STRUCT report = form.GetReportStruct();

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
			if(report.cursor_table >= report.TableCount)	return;
			CELL_STRUCT cell = (CELL_STRUCT)table.cellBuf[report.cursor_y1*table.cell_x+report.cursor_x1];


			this.radioButtonHorz0.Checked = (cell.cAlignHorz == 0);
			this.radioButtonHorz1.Checked = (cell.cAlignHorz == 1);
			this.radioButtonHorz2.Checked = (cell.cAlignHorz == 2);
			this.radioButtonHorz3.Checked = (cell.cAlignHorz == 3);

			this.radioButtonVert0.Checked = (cell.cAlignVert == 0);
			this.radioButtonVert1.Checked = (cell.cAlignVert == 1);
			this.radioButtonVert2.Checked = (cell.cAlignVert == 2);
			this.radioButtonVert3.Checked = (cell.cAlignVert == 3);
		}

		public void GetItem(FormReportChild form)
		{
			REPORT_STRUCT report = form.GetReportStruct();

			sbyte horz, vert;

			if(this.radioButtonHorz0.Checked)		horz = 0;
			else if(this.radioButtonHorz1.Checked)	horz = 1;
			else if(this.radioButtonHorz2.Checked)	horz = 2;
			else if(this.radioButtonHorz3.Checked)	horz = 3;
			else									horz = 0;

			if(this.radioButtonVert0.Checked)		vert = 0;
			else if(this.radioButtonVert1.Checked)	vert = 1;
			else if(this.radioButtonVert2.Checked)	vert = 2;
			else if(this.radioButtonVert3.Checked)	vert = 3;
			else									vert = 0;

			SelectedCell.SelectedCellSetAlign(report, horz, vert);

			form.Invalidate();
			form.SetChangeFlag();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void radioButtonVert2_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}

/*
void ConfigCellAlign(CReporterView *view)
{
	CDialogConfigCellAlign dialog;

	REPORT_STRUCT *report = view->GetReportStruct();

	TABLE_STRUCT *table = &report.tableBuf[report.cursor_table];
	if(report.cursor_table >= report.nTableCount)	return;
	CELL_STRUCT *cell = &table.cellBuf[report.cursor_y1*table.cell_x+report.cursor_x1];

	dialog.m_horz = cell->cAlignHorz;
	dialog.m_vert = cell->cAlignVert;
	
	if(dialog.DoModal() == IDOK) {
		void SelectedCellSetAlign(REPORT_STRUCT *report, char horz, char vert);
		SelectedCellSetAlign(report, dialog.m_horz, dialog.m_vert);

		view->InvalidateRect(NULL);
	}
}

BOOL CDialogConfigCellAlign::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	SetRadioPosition(m_hWnd, IDC_ConfigCellAlign_RADIO_HORZ0, 3, m_horz);
	SetRadioPosition(m_hWnd, IDC_ConfigCellAlign_RADIO_VERT0, 4, m_vert);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDialogConfigCellAlign::OnOK() 
{
	// TODO: Add extra validation here
	m_horz = GetRadioPosition(m_hWnd, IDC_ConfigCellAlign_RADIO_HORZ0, 3);
	m_vert = GetRadioPosition(m_hWnd, IDC_ConfigCellAlign_RADIO_VERT0, 4);
	
	CDialog::OnOK();
}

 */
