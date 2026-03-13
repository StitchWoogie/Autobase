using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.IO;
using AutoLibLocal;
using System.Threading;
using GraphicModule;
using System.Drawing.Printing;

namespace LocalMain
{
	/// <summary>
	/// Summary description for FormConfigAutoPrint.
	/// </summary>
	public class FormConfigAutoPrint : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel; 
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonModify;
		private System.Windows.Forms.ListView m_list;
		private System.Windows.Forms.TextBox textBoxDes;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigAutoPrint()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigAutoPrint));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonModify = new System.Windows.Forms.Button();
            this.m_list = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.textBoxDes = new System.Windows.Forms.TextBox();
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
            // buttonAdd
            // 
            this.buttonAdd.AccessibleDescription = null;
            this.buttonAdd.AccessibleName = null;
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.BackgroundImage = null;
            this.buttonAdd.Font = null;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.AccessibleDescription = null;
            this.buttonDelete.AccessibleName = null;
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.BackgroundImage = null;
            this.buttonDelete.Font = null;
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonModify
            // 
            this.buttonModify.AccessibleDescription = null;
            this.buttonModify.AccessibleName = null;
            resources.ApplyResources(this.buttonModify, "buttonModify");
            this.buttonModify.BackgroundImage = null;
            this.buttonModify.Font = null;
            this.buttonModify.Name = "buttonModify";
            this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
            // 
            // m_list
            // 
            this.m_list.AccessibleDescription = null;
            this.m_list.AccessibleName = null;
            resources.ApplyResources(this.m_list, "m_list");
            this.m_list.BackgroundImage = null;
            this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_list.Font = null;
            this.m_list.FullRowSelect = true;
            this.m_list.HideSelection = false;
            this.m_list.MultiSelect = false;
            this.m_list.Name = "m_list";
            this.m_list.UseCompatibleStateImageBehavior = false;
            this.m_list.View = System.Windows.Forms.View.Details;
            this.m_list.SelectedIndexChanged += new System.EventHandler(this.m_list_SelectedIndexChanged);
            this.m_list.DoubleClick += new System.EventHandler(this.m_list_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // textBoxDes
            // 
            this.textBoxDes.AccessibleDescription = null;
            this.textBoxDes.AccessibleName = null;
            resources.ApplyResources(this.textBoxDes, "textBoxDes");
            this.textBoxDes.BackgroundImage = null;
            this.textBoxDes.Font = null;
            this.textBoxDes.Name = "textBoxDes";
            this.textBoxDes.ReadOnly = true;
            // 
            // FormConfigAutoPrint
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.textBoxDes);
            this.Controls.Add(this.m_list);
            this.Controls.Add(this.buttonModify);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigAutoPrint";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigAutoPrint_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		ArrayList blockTemp = new ArrayList();

		private void buttonAdd_Click(object sender, System.EventArgs e)
		{
			FormConfigAutoPrintAdd dialog = new FormConfigAutoPrintAdd();
			AUTO_PRINT_LIST list;

			if(Tools.IsLangKorean()) 
				dialog.Text = "리포터 자동 인쇄 추가";
			else if(Tools.IsLangJapanese()) 
				dialog.Text = "レポート自動印刷の追加";
			else if(Tools.IsLangChinese()) 
				dialog.Text = "添加自动打印报表";
			else
				dialog.Text = "Add Report Auto Print";

            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				list = new AUTO_PRINT_LIST();

				dialog.GetStruct(list);

				ReportAutoPrint.CalcPrintedFlag(list);

				blockTemp.Add(list);
				ListViewItem lvi = new ListViewItem(list.filename);
				m_list.Items.Add(lvi);
				lvi.Selected = true;
				DrawInfoString();
			}	
		}

		private void buttonDelete_Click(object sender, System.EventArgs e)
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("삭제하고 싶은 항목을 선택한 후 다시 하세요.", "삭제오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else 
				{
					MessageBox.Show("Select the item to delete.", "Delete error");
				}
				return;
			}

			string buf;
			
			ListViewItem lvi = m_list.SelectedItems[0];
			int retn = lvi.Index;

			buf = lvi.Text;

			if(Tools.IsLangKorean()) 
			{
				if(MessageBox.Show(buf, "선택한 항목을 삭제할까요?", MessageBoxButtons.YesNo) != DialogResult.Yes)	return;
			}
			else if(Tools.IsLangChinese()) 
			{
				if(MessageBox.Show(buf, "要删除选择的项吗？", MessageBoxButtons.YesNo) != DialogResult.Yes)	return;
			}
			else 
			{
				if(MessageBox.Show(buf, "Delete selected item?", MessageBoxButtons.YesNo) != DialogResult.Yes)	return;
			}
			blockTemp.RemoveAt(retn);
			m_list.Items.RemoveAt(retn);
			DrawInfoString();
		}

		void ChangeItem(AUTO_PRINT_LIST list, ListViewItem lvi)
		{
			lvi.SubItems[0].Text = list.filename;
		}

		void Modify()
		{
			FormConfigAutoPrintAdd dialog = new FormConfigAutoPrintAdd();
			AUTO_PRINT_LIST list;

			if(m_list.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("수정하고 싶은 항목을 선택한 후 다시 하세요.", "수정오류");
				}
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else 
				{
					MessageBox.Show("Select item to modify.", "Modify error");
				}
				return;
			}

			ListViewItem lvi = m_list.SelectedItems[0];
			int retn = lvi.Index;

			list = (AUTO_PRINT_LIST)blockTemp[retn];

			dialog.SetStruct(list);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				dialog.GetStruct(list);
				ReportAutoPrint.CalcPrintedFlag(list);
				ChangeItem(list, lvi);
				DrawInfoString();
			}	
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			Modify();
		}

		void MakeItemString(out string buf, AUTO_PRINT_LIST list)
		{
			if(Tools.IsLangKorean()) 
			{
				if(list.type == 0) 
				{
					buf = String.Format("{0} (매시 {1:00}분)", list.filename, list.min);
				}
				else if(list.type == 1) 
				{
					buf = String.Format("{0} (매일 {1:00}:{2:00})", list.filename, list.hour, list.min);
				}
				else if(list.type == 2) 
				{
					buf = String.Format("{0} (매월 {1}일 {2:00}:{3:00})", list.filename, list.day, list.hour, list.min);
				}
				else 
				{
					buf = String.Format("{0}", list.filename);
				}
			}
			else if(Tools.IsLangJapanese()) 
			{
				if(list.type == 0) 
				{
					buf = String.Format("{0} (毎時 {1:00}分)", list.filename, list.min);
				}
				else if(list.type == 1) 
				{
					buf = String.Format("{0} (毎日 {1:00}:{2:00})", list.filename, list.hour, list.min);
				}
				else if(list.type == 2) 
				{
					buf = String.Format("{0} (毎月 {1}日 {2:00}:{3:00})", list.filename, list.day, list.hour, list.min);
				}
				else 
				{
					buf = String.Format("{0}", list.filename);
				}
			}
			else if(Tools.IsLangChinese()) 
			{
				if(list.type == 0) 
				{
					buf = String.Format("{0} (按时间 {1:00}分)", list.filename, list.min);
				}
				else if(list.type == 1) 
				{
					buf = String.Format("{0} (按天 {1:00}:{2:00})", list.filename, list.hour, list.min);
				}
				else if(list.type == 2) 
				{
					buf = String.Format("{0} (按月 {1}日 {2:00}:{3:00})", list.filename, list.day, list.hour, list.min);
				}
				else 
				{
					buf = String.Format("{0}", list.filename);
				}
			}
			else 
			{
				if(list.type == 0) 
				{
					buf = String.Format("{0} (Every hour {1:00} minute)", list.filename, list.min);
				}
				else if(list.type == 1) 
				{
					buf = String.Format("{0} (Every day {1:00}:{2:00})", list.filename, list.hour, list.min);
				}
				else if(list.type == 2) 
				{
					buf = String.Format("{0} (Every {1} day {2:00}:{3:00})", list.filename, list.day, list.hour, list.min);
				}
				else 
				{
					buf = String.Format("{0}", list.filename);
				}
			}
		}

		void DrawInfoString()
		{
			if(m_list.SelectedItems.Count == 0) 
			{
				this.textBoxDes.Text = " ";
				return;
			}
			AUTO_PRINT_LIST list;
			int retn = m_list.SelectedItems[0].Index;

			list = (AUTO_PRINT_LIST)blockTemp[retn];

			string buf;

			MakeItemString(out buf, list);

			buf += String.Format("\r\n[{0}]", list.sPrinterName);

			this.textBoxDes.Text = buf;
		}

		void FillList()
		{
			AUTO_PRINT_LIST list;
			int l;

			for(l = 0; l < blockTemp.Count; l++) 
			{
				list = (AUTO_PRINT_LIST)blockTemp[l];
				ListViewItem lvi = new ListViewItem("");
				ChangeItem(list, lvi);
				m_list.Items.Add(lvi);
			}
		}

		private void FormConfigAutoPrint_Load(object sender, System.EventArgs e)
		{
			blockTemp = (ArrayList)Tools.CopyObject(ReportAutoPrint.blockAutoPrint);
			FillList();
			if(m_list.Items.Count > 0) 
			{
				m_list.Items[0].Selected = true;
			}
			DrawInfoString();

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_CONFIG_ETC))
                this.buttonOK.Enabled = false;
		}

		private void m_list_DoubleClick(object sender, System.EventArgs e)
		{
			Modify();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ReportAutoPrint.blockAutoPrint = (ArrayList)Tools.CopyObject(blockTemp);
			ReportAutoPrint.SaveAutoPrintList();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DrawInfoString();	
		}
	}

	[Serializable]
	public class AUTO_PRINT_LIST
	{
		public string filename;
		public int  type;
		public int  mon;
		public int  day;
		public int  hour;
		public int  min;
		public string sPrinterName;
		public bool bPrintedFlag;	// OFF 는 프린터가 되지 않았다. ON = 프린터가 되었다.
		public int  nReportFileType; // 0 - basic, 1 - Excel	
	}

	class ReportAutoPrint
	{
		public static ArrayList blockAutoPrint = new ArrayList();

		static ReportAutoPrint()
		{
			LoadAutoPrintList();
		}

		public static void SaveAutoPrintList()
		{
			string filename;
			TextWriter writer;
			AUTO_PRINT_LIST list;
			int l;

			filename = String.Format("{0}\\report", TotalConfig.sDirWorkProject);
			Directory.CreateDirectory(filename);

			filename = String.Format("{0}\\report\\autoprn.lstx", TotalConfig.sDirWorkProject);
			writer = new StreamWriter(filename);
			if(writer == null)	return;

			for(l = 0; l < blockAutoPrint.Count; l++) 
			{
				list = (AUTO_PRINT_LIST)blockAutoPrint[l];
		
				writer.Write("{0},{1},{2},{3},{4},{5},", list.filename, list.type, list.mon, list.day, list.hour, list.min);
				writer.Write("{0},", list.sPrinterName);
				writer.Write("{0},", list.nReportFileType);
				writer.WriteLine();
			}
			writer.Close();
		}

		// 로드시나 새로 설정 시 시간이 아직 안되었으면 인쇄완료 플래그를 false 지났으면 true로 한다.
		public static void CalcPrintedFlag(AUTO_PRINT_LIST list)
		{
			DateTime t = DateTime.Now;

			if(list.type == 0)	// hour
			{
				list.bPrintedFlag = (t.Minute >= list.min);
			}
			else if(list.type == 1)		// day
			{
				list.bPrintedFlag = (t.Minute+t.Hour*60 >= list.min+list.hour*60);
			}
			else if(list.type == 2)		// month
			{
				list.bPrintedFlag = (t.Minute+t.Hour*60+t.Day*60*24 >= list.min+list.hour*60+list.day*60*24);
			}
			else 
			{
				list.bPrintedFlag = true;
			}
		}

		static void LoadAutoPrintList()
		{
			TextReader reader = null;
			string buf;
			AUTO_PRINT_LIST list;
			CommaBlockString comma = new CommaBlockString();
			bool unicode = false;

			string filename = String.Format("{0}\\Report\\AutoPrn.lstx", TotalConfig.sDirWorkProject);

			if(File.Exists(filename)) 
			{
				unicode = true;
				reader = new StreamReader(filename);
			}
			else 
			{
				filename = String.Format("{0}\\Report\\AutoPrn.lst", TotalConfig.sDirWorkProject);
				if(File.Exists(filename)) 
				{
					reader = new StreamReader(filename, System.Text.Encoding.Default);
				}
			}

			if(reader == null)	return;
			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;
				list = new AUTO_PRINT_LIST();
				comma.Set(buf);
				comma.GetString(ref list.filename);
				comma.GetInt(ref list.type);
				comma.GetInt(ref list.mon);
				comma.GetInt(ref list.day);
				comma.GetInt(ref list.hour);
				comma.GetInt(ref list.min);
				if(unicode) 
				{
					comma.GetString(ref list.sPrinterName);
				}
				else 
				{
					comma.GetString(ref list.sPrinterName);
					comma.Skip();
					comma.Skip();
				}
				comma.GetInt(ref list.nReportFileType);
				CalcPrintedFlag(list);	
				
				blockAutoPrint.Add(list);
			}
			reader.Close();
		}

		static DateTime tOld;
		static bool first_flag = false;

		public static void AutoPrintCheck()
		{
			if(blockAutoPrint.Count == 0)	return;
	
			DateTime t = DateTime.Now;

			if(first_flag == false) 
			{
				first_flag = true;
				tOld = t;
			}

			if(tOld.Minute == t.Minute)	return;	// 시간 변화가 없다.

			AUTO_PRINT_LIST list;
			int l;

			// 시간,일,월이 바뀌면 해당 List의 bPrintedFlag 를 OFF한다.
			if(tOld.Hour != t.Hour) 
			{
				for(l = 0; l < blockAutoPrint.Count; l++) 
				{
					list = (AUTO_PRINT_LIST)blockAutoPrint[l];
					if(list.type == 0) 
					{	// 매시 인쇄
						if(list.bPrintedFlag) 
						{
							list.bPrintedFlag = false;	
						}
					}
				}
			}

			if(tOld.Day != t.Day) 
			{
				for(l = 0; l < blockAutoPrint.Count; l++) 
				{
					list = (AUTO_PRINT_LIST)blockAutoPrint[l];
					if(list.type == 1) 
					{	// 매일 인쇄
						if(list.bPrintedFlag) 
						{
							list.bPrintedFlag = false;
						}
					}
				}
			}

			if(tOld.Month != t.Month) 
			{
				for(l = 0; l < blockAutoPrint.Count; l++) 
				{
					list = (AUTO_PRINT_LIST)blockAutoPrint[l];
					if(list.type == 2) 
					{	// 매월 인쇄
						if(list.bPrintedFlag) 
						{
							list.bPrintedFlag = false;	
						}
					}
				}
			}

			tOld = t;

			for(l = 0; l < blockAutoPrint.Count; l++) 
			{
				list = (AUTO_PRINT_LIST)blockAutoPrint[l];
				if(list.bPrintedFlag)	continue;
				if(list.type == 0) 
				{	// 매시 인쇄
					if(t.Minute >= list.min) 
					{
						AutoPrintGo(list, t);
						list.bPrintedFlag = true;
					}
				}
				else if(list.type == 1) 
				{	// 매일 인쇄
					if(t.Hour >= list.hour && t.Minute >= list.min) 
					{
						AutoPrintGo(list, t);
						list.bPrintedFlag = true;
					}
				}
				else if(list.type == 2) 
				{	// 매월 인쇄
					if(t.Day >= list.day && t.Hour >= list.hour && t.Minute >= list.min) 
					{
						AutoPrintGo(list, t);
						list.bPrintedFlag = true;
					}
				}
				else 
				{
					list.bPrintedFlag = true;
				}
			}
		}

		static void AutoPrintGo(AUTO_PRINT_LIST list, DateTime t)
		{
			if(SystemStatusMemory.GetDI(SSMDI.DuplexControlItemLinePrinter) == 0)	return;

			// 자동 인쇄할 때 사용할 리포터 시간을 계산한다.

			if(list.type == 0) 
			{	// 매시 인쇄

				t = t.AddHours(-1);
			}
			else if(list.type == 1) 
			{	// 매일 인쇄
				t = t.AddDays(-1);
			}
			else if(list.type == 2) 
			{	// 매월 인쇄
				t = t.AddMonths(-1);
			}
			else {
			}

			ReportBasicLib.ReportConfig.tAutoReportTime = t;

			if(list.nReportFileType == 1) 
			{	// excel
				string err_msg = "";
				if(!ScriptFunctionExcel.ExcelReportPrepare(ref err_msg, list.filename, "", 1, 1, t.Year, t.Month, t.Day, t.Hour, 0)) 
				{
					if(Tools.IsLangKorean())
						AutoLibLocal.MessageDisplay.Show("자동인쇄오류\n{0}", err_msg);	
					else if(Tools.IsLangChinese())
						AutoLibLocal.MessageDisplay.Show("自动打印错误\n{0}", err_msg);	
					else
						AutoLibLocal.MessageDisplay.Show("AutoPrint Error\n{0}", err_msg);	

					return;
				}
				ScriptFunctionExcel.ExcelReportRun(ref err_msg);
			}
			else 
			{
				if(!PrintUtil.IsPrinterExist(list.sPrinterName)) 
				{
					if(Tools.IsLangKorean()) 
					{
						AutoLibLocal.MessageDisplay.Show("리포터 자동 인쇄에서 존재하지 않는 프린터를 설정하였습니다.\nPrinterName={0}\nReportFile={1}", list.sPrinterName, list.filename);
					}
					else if(Tools.IsLangChinese()) 
					{
						AutoLibLocal.MessageDisplay.Show("自动打印报表时,您选定了不存在的打印机.\nPrinterName={0}\nReportFile={1}", list.sPrinterName, list.filename);
					}
					else 
					{
						AutoLibLocal.MessageDisplay.Show("Printer not found. Check Report AutoPrint\nPrinterName={0}\nReportFile={1}", list.sPrinterName, list.filename);
					}
					return;
				}

				PrintDocument pd = new PrintDocument();
				PrinterSettings ps = new PrinterSettings();
				ps.PrinterName = list.sPrinterName;
				pd.PrinterSettings = ps;

				ReportModule.ReportPrint print = new ReportModule.ReportPrint();
				print.PrintReportFile("자동인쇄", list.filename, ReportBasicLib.EnumHandAuto.AUTO_MODE, pd);
			}
		}

	}
}


