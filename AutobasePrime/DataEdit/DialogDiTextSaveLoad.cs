using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Threading.Tasks;

namespace DataEdit
{
	/// <summary>
	/// Summary description for DialogAiMinDataEdit.
	/// </summary>
	public class DialogDiTextSaveLoad : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_Exit;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label_No;
		private System.Windows.Forms.Label label_TagName;
		private System.Windows.Forms.Label label_Description;
		private System.Windows.Forms.Button button_PrevTag;
		private System.Windows.Forms.Button button_NextTag;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown numericUpDown_Year;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDown_Month;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDown_Day;
		private System.Windows.Forms.NumericUpDown numericUpDown_Hour;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDown_Minute;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.ComponentModel.IContainer components = null;

		public TagListStruct[]					tagListDi;
		string									tagName;
		int										currPos = -1;
		private TagDiClass						di;
		private System.Windows.Forms.TextBox textBox_Filename;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button button_TextFile;
		private System.Windows.Forms.Button button_WriteToText;
		private System.Windows.Forms.RadioButton radioButton_HourDataType;
		private System.Windows.Forms.RadioButton radioButton_MinDataType;
		private System.Windows.Forms.NumericUpDown numericUpDown_DataCount;
		private System.Windows.Forms.Button button_ReadText;
		
		public DialogDiTextSaveLoad(string tag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			tagName = tag;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogDiTextSaveLoad));
            this.button_Exit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_Description = new System.Windows.Forms.Label();
            this.label_TagName = new System.Windows.Forms.Label();
            this.label_No = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_PrevTag = new System.Windows.Forms.Button();
            this.button_NextTag = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_Minute = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Hour = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_Day = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Month = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_Year = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDown_DataCount = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_TextFile = new System.Windows.Forms.Button();
            this.textBox_Filename = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButton_HourDataType = new System.Windows.Forms.RadioButton();
            this.radioButton_MinDataType = new System.Windows.Forms.RadioButton();
            this.button_WriteToText = new System.Windows.Forms.Button();
            this.button_ReadText = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Minute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Year)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DataCount)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Exit
            // 
            resources.ApplyResources(this.button_Exit, "button_Exit");
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_Description);
            this.groupBox1.Controls.Add(this.label_TagName);
            this.groupBox1.Controls.Add(this.label_No);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label_Description
            // 
            resources.ApplyResources(this.label_Description, "label_Description");
            this.label_Description.Name = "label_Description";
            // 
            // label_TagName
            // 
            resources.ApplyResources(this.label_TagName, "label_TagName");
            this.label_TagName.Name = "label_TagName";
            // 
            // label_No
            // 
            resources.ApplyResources(this.label_No, "label_No");
            this.label_No.Name = "label_No";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // button_PrevTag
            // 
            resources.ApplyResources(this.button_PrevTag, "button_PrevTag");
            this.button_PrevTag.Name = "button_PrevTag";
            this.button_PrevTag.Click += new System.EventHandler(this.button_PrevTag_Click);
            // 
            // button_NextTag
            // 
            resources.ApplyResources(this.button_NextTag, "button_NextTag");
            this.button_NextTag.Name = "button_NextTag";
            this.button_NextTag.Click += new System.EventHandler(this.button_NextTag_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDown_Minute);
            this.groupBox2.Controls.Add(this.numericUpDown_Hour);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numericUpDown_Day);
            this.groupBox2.Controls.Add(this.numericUpDown_Month);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericUpDown_Year);
            this.groupBox2.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericUpDown_Minute
            // 
            resources.ApplyResources(this.numericUpDown_Minute, "numericUpDown_Minute");
            this.numericUpDown_Minute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown_Minute.Name = "numericUpDown_Minute";
            // 
            // numericUpDown_Hour
            // 
            resources.ApplyResources(this.numericUpDown_Hour, "numericUpDown_Hour");
            this.numericUpDown_Hour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDown_Hour.Name = "numericUpDown_Hour";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // numericUpDown_Day
            // 
            resources.ApplyResources(this.numericUpDown_Day, "numericUpDown_Day");
            this.numericUpDown_Day.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown_Day.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Day.Name = "numericUpDown_Day";
            this.numericUpDown_Day.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDown_Month
            // 
            resources.ApplyResources(this.numericUpDown_Month, "numericUpDown_Month");
            this.numericUpDown_Month.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown_Month.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Month.Name = "numericUpDown_Month";
            this.numericUpDown_Month.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Month.ValueChanged += new System.EventHandler(this.numericUpDown_Month_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDown_Year
            // 
            resources.ApplyResources(this.numericUpDown_Year, "numericUpDown_Year");
            this.numericUpDown_Year.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_Year.Name = "numericUpDown_Year";
            this.numericUpDown_Year.Value = new decimal(new int[] {
            2005,
            0,
            0,
            0});
            this.numericUpDown_Year.ValueChanged += new System.EventHandler(this.numericUpDown_Year_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDown_DataCount);
            this.groupBox3.Controls.Add(this.label9);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // numericUpDown_DataCount
            // 
            resources.ApplyResources(this.numericUpDown_DataCount, "numericUpDown_DataCount");
            this.numericUpDown_DataCount.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDown_DataCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_DataCount.Name = "numericUpDown_DataCount";
            this.numericUpDown_DataCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button_TextFile);
            this.groupBox4.Controls.Add(this.textBox_Filename);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // button_TextFile
            // 
            resources.ApplyResources(this.button_TextFile, "button_TextFile");
            this.button_TextFile.Name = "button_TextFile";
            this.button_TextFile.Click += new System.EventHandler(this.button_TextFile_Click);
            // 
            // textBox_Filename
            // 
            resources.ApplyResources(this.textBox_Filename, "textBox_Filename");
            this.textBox_Filename.Name = "textBox_Filename";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButton_HourDataType);
            this.groupBox5.Controls.Add(this.radioButton_MinDataType);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // radioButton_HourDataType
            // 
            resources.ApplyResources(this.radioButton_HourDataType, "radioButton_HourDataType");
            this.radioButton_HourDataType.Name = "radioButton_HourDataType";
            this.radioButton_HourDataType.CheckedChanged += new System.EventHandler(this.radioButton_HourDataType_CheckedChanged);
            // 
            // radioButton_MinDataType
            // 
            resources.ApplyResources(this.radioButton_MinDataType, "radioButton_MinDataType");
            this.radioButton_MinDataType.Name = "radioButton_MinDataType";
            this.radioButton_MinDataType.CheckedChanged += new System.EventHandler(this.radioButton_MinDataType_CheckedChanged);
            // 
            // button_WriteToText
            // 
            resources.ApplyResources(this.button_WriteToText, "button_WriteToText");
            this.button_WriteToText.Name = "button_WriteToText";
            this.button_WriteToText.Click += new System.EventHandler(this.button_WriteToText_Click);
            // 
            // button_ReadText
            // 
            resources.ApplyResources(this.button_ReadText, "button_ReadText");
            this.button_ReadText.Name = "button_ReadText";
            this.button_ReadText.Click += new System.EventHandler(this.button_ReadText_Click);
            // 
            // DialogDiTextSaveLoad
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.button_ReadText);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_NextTag);
            this.Controls.Add(this.button_PrevTag);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Exit);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.button_WriteToText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogDiTextSaveLoad";
            this.ShowInTaskbar = false;
            this.Closed += new System.EventHandler(this.DialogDiTextSaveLoad_Closed);
            this.Load += new System.EventHandler(this.DialogAiTextSaveLoad_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Minute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Year)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_DataCount)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		void tagPosAddOrSub(bool bAdd)
		{
			if(this.tagListDi == null || tagListDi.Length <= 1) return;	// 태그가 없거나 1개 일때

			int			pos = currPos;
			if(pos < 0 || pos >= tagListDi.Length) pos = 0;
			if(bAdd) pos = (pos >= tagListDi.Length-1) ? 0 : pos + 1;
			else	 pos = (pos <= 0) ? tagListDi.Length-1 : pos - 1;

			di = TagLib.GetStructDI(tagListDi[pos]);
			if(di == null) return;
			DisplayTagInfo(pos);
		}

		void DisplayTagInfo(int pos)
		{
			currPos = pos;
			this.label_TagName.Text = di.tag;
			this.label_Description.Text = di.description;
			this.label_No.Text = string.Format("{0}", pos+1);
		}

		void checkedAndEnableDateItem(int type)
		{
			if(type == 0)	// 분자료
			{
				this.radioButton_MinDataType.Checked = true;
				this.numericUpDown_Minute.Enabled = true;
			}
			else										// 시간자료
			{
				this.radioButton_HourDataType.Checked = true;
				this.numericUpDown_Minute.Enabled = false;
			}
		}

		private void DialogAiTextSaveLoad_Load(object sender, System.EventArgs e)
		{
			this.numericUpDown_Year.Value = DataEditConfig.dTime.Year;
			this.numericUpDown_Month.Value = DataEditConfig.dTime.Month;
			this.numericUpDown_Day.Value = DataEditConfig.dTime.Day;
			this.numericUpDown_Hour.Value = DataEditConfig.dTime.Hour;
			this.numericUpDown_Minute.Value = DataEditConfig.dTime.Minute;

			this.numericUpDown_DataCount.Value = DataEditConfig.nTextSaveLoadCount;
			checkedAndEnableDateItem(DataEditConfig.nTextSaveLoadType);
			this.textBox_Filename.Text = DataEditConfig.sTextFilename;

			if(this.tagListDi == null) return;

			int			pos = 0;
			pos = TagLib.GetTagPosOnlyList(tagListDi, tagName);
			if(pos < 0 || pos >= tagListDi.Length) return;
			di = TagLib.GetStructDI(tagListDi[pos]);
			if(di == null) return;
			DisplayTagInfo(pos);
		}

		private void button_Exit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void button_PrevTag_Click(object sender, System.EventArgs e)
		{
			tagPosAddOrSub(false);
		}

		private void button_NextTag_Click(object sender, System.EventArgs e)
		{
			tagPosAddOrSub(true);
		}		
		
		private void numericUpDown_Year_ValueChanged(object sender, System.EventArgs e)
		{
			setMaxDayLimit();
			
		}

		private void numericUpDown_Month_ValueChanged(object sender, System.EventArgs e)
		{
			setMaxDayLimit();
		}
		
		void setMaxDayLimit()
		{
			int		year = (int)this.numericUpDown_Year.Value;
			int		month = (int)this.numericUpDown_Month.Value;
			int		day = (int)this.numericUpDown_Day.Value;
			int		limit = TimeUtil.getmonthlimit(year, month);
			
			if(day < 1) day = 1;
			if(day > limit) day = limit;
			this.numericUpDown_Day.Value = day;
			this.numericUpDown_Day.Maximum = limit;
		}

		void readCurrentDateTime()
		{
			int		year = (int)this.numericUpDown_Year.Value;
			int		month = (int)this.numericUpDown_Month.Value;
			int		day = (int)this.numericUpDown_Day.Value;
			int		hour = (int)this.numericUpDown_Hour.Value;
			int		minute = (int)this.numericUpDown_Minute.Value;
			DataEditConfig.dTime = new DateTime(year, month, day, hour, minute, 0);
		}

		private void button_TextFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog	dialog = new OpenFileDialog();
			dialog.FileName = DataEditConfig.sTextFilename;
			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				DataEditConfig.sTextFilename = dialog.FileName;
				this.textBox_Filename.Text = DataEditConfig.sTextFilename;
			}
		}

		private void radioButton_MinDataType_CheckedChanged(object sender, System.EventArgs e)
		{
			int			type = (radioButton_MinDataType.Checked) ? 0 : 1;
			checkedAndEnableDateItem(type);
		}

		private void radioButton_HourDataType_CheckedChanged(object sender, System.EventArgs e)
		{
			int			type = (radioButton_HourDataType.Checked) ? 1 : 0;
			checkedAndEnableDateItem(type);
		}

		bool checkFileExistOverWrite()
		{
			if(File.Exists(this.textBox_Filename.Text)) 
			{
				string		msg;
				if(Tools.IsLangKorean()) 
				{
					msg = string.Format("설정한 파일이 존재합니다. 덮어쓸까요? ({0})", this.textBox_Filename.Text);
					if(MessageBox.Show(msg, "덮어쓰기 확인", MessageBoxButtons.OKCancel) != DialogResult.OK) return false;
				}
				else 
				{
					msg = string.Format("Selected Data File Exist. Overwrite? ({0})", this.textBox_Filename.Text);
					if(MessageBox.Show(msg, "File Overwrite?", MessageBoxButtons.OKCancel) != DialogResult.OK) return false;
				}
			}
			return true;
		}


		async Task diMinDataSaveToFile(Stream fs)
		{
			TextWriter writer = new StreamWriter(fs);
			if(writer == null) 
			{
				FileReadErrorMessage();
				return;
			}

			int					count = (int)this.numericUpDown_DataCount.Value;
			DateTime			d = new DateTime(DataEditConfig.dTime.Year, DataEditConfig.dTime.Month, DataEditConfig.dTime.Day, DataEditConfig.dTime.Hour, DataEditConfig.dTime.Minute, 0);
			TREND_DI_STRUCT		data = new TREND_DI_STRUCT();
			DataLocal			dLocal = new DataLocal();
			bool				bExist;			

			for(int i = 0; i < count; i++) 
			{
				bExist = await dLocal.LoadMinDataStructDI(di.tag, d, data);
				if(bExist) writer.WriteLine("{0,04:d4}-{1,02:d2}-{2,02:d2}, {3,02:d2}:{4,02:d2}, {5,9:d}, {6,9:d}, {7,9:d},", d.Year, d.Month, d.Day, d.Hour, d.Minute, (data.bOnOff == 1) ? 1 : 0, data.nCountOnOff, data.cOnTime);
				else       writer.WriteLine("{0,04:d4}-{1,02:d2}-{2,02:d2}, {3,02:d2}:{4,02:d2},        ..,        ..,        ..,", d.Year, d.Month, d.Day, d.Hour, d.Minute);
				d = d.AddMinutes(1);
			}
			writer.Close();

			string		msg = string.Format("{0}", count);
			if(Tools.IsLangKorean()) MessageBox.Show(msg+" 개의 분 자료 쓰기 완료.", "파일쓰기 완료", MessageBoxButtons.OK);
			else					 MessageBox.Show(msg+" Minutes Data Saved To File.", "File Write Finished", MessageBoxButtons.OK);
		}

		async Task diHourDataSaveToFile(Stream fs)
		{
			TextWriter writer = new StreamWriter(fs);
			if(writer == null) 
			{
				FileReadErrorMessage();
				return;
			}

			int						count = (int)this.numericUpDown_DataCount.Value;
			DateTime				d = new DateTime(DataEditConfig.dTime.Year, DataEditConfig.dTime.Month, DataEditConfig.dTime.Day, DataEditConfig.dTime.Hour, 0, 0);
			HOUR_DATA_DIGITAL_STRUCT	data = new HOUR_DATA_DIGITAL_STRUCT();
			DataLocal				dLocal = new DataLocal();
			bool					bExist;			

			for(int i = 0; i < count; i++) 
			{
				bExist = await dLocal.LoadHourDataStructDI(di.tag, d, data);
				if(bExist) writer.WriteLine("{0,04:d4}-{1,02:d2}-{2,02:d2}, {3,02:d2}, {4,9:d}, {5,9:d},", d.Year, d.Month, d.Day, d.Hour, data.wCountOnOff, data.dwOnTime);
				else       writer.WriteLine("{0,04:d4}-{1,02:d2}-{2,02:d2}, {3,02:d2},        ..,        ..,", d.Year, d.Month, d.Day, d.Hour, d.Minute);
				d = d.AddHours(1);
			}
			writer.Close();
			string		msg = string.Format("{0}", count);
			if(Tools.IsLangKorean()) MessageBox.Show(msg+" 개의 시간 자료 쓰기 완료.", "파일쓰기 완료", MessageBoxButtons.OK);
			else					 MessageBox.Show(msg+" Hours Data Saved To File.", "File Write Finished", MessageBoxButtons.OK);
		}

		private async void button_WriteToText_Click(object sender, System.EventArgs e)
		{
			if(di == null) return;
			if(checkFileExistOverWrite() == false) return;
			
			Stream fs = File.Open(textBox_Filename.Text, FileMode.Create);
			if(fs == null) 
			{
				FileReadErrorMessage();
				return;
			}
			

			readCurrentDateTime();
			if(this.radioButton_MinDataType.Checked) await diMinDataSaveToFile(fs);
			else	await diHourDataSaveToFile(fs);
			fs.Close();
		}

		void FileReadErrorMessage()
		{
			string		msg;
			if(Tools.IsLangKorean()) 
			{
				msg = string.Format("지정한 파일을 열 수 없습니다. ({0})", this.textBox_Filename.Text);
				MessageBox.Show(msg, "파일열기 에러", MessageBoxButtons.OK);
			}
			else 
			{
				msg = string.Format("Selected Data File Open Error. ({0})", this.textBox_Filename.Text);
				MessageBox.Show(msg, "File Open Error", MessageBoxButtons.OK);
			}
			
		}

		bool getHourMinData(string data, ref DateTime d, bool bMin)
		{
			CommaBlockString	comma = new CommaBlockString();
			int					hour = 0, min = 0;

			comma.Set(data);
			comma.SetBlockCode(':');
			if(comma.IsEOS()) return false;
			comma.GetInt(ref hour);
			if(hour < 0 || hour > 23) return false;

			if(bMin)
			{
				if(comma.IsEOS()) return false;
				comma.GetInt(ref min);
				if(min < 0 || min > 59) return false;
				d = new DateTime(d.Year, d.Month, d.Day, hour, min, 0);
			}
			else 
			{				
				d = new DateTime(d.Year, d.Month, d.Day, hour, 0, 0);
			}
			return true;
		}

		void diMinDataTextToFile(FileStream fs)
		{
			TextReader reader = new StreamReader(fs);
			if(reader == null) 
			{
				FileReadErrorMessage();
				return;
			}

			int					i, count = 0, notExistCount;
			DateTime			d = new DateTime(DataEditConfig.dTime.Year, DataEditConfig.dTime.Month, DataEditConfig.dTime.Day, DataEditConfig.dTime.Hour, DataEditConfig.dTime.Minute, 0);
			TREND_DI_STRUCT		data;
			CommaBlockString	comma = new CommaBlockString();
			string				one_line, imsi = "";
			short[]				val;

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null || one_line.Length == 0) break;

				comma.Set(one_line);
				if(comma.IsEOS()) continue;
				d = comma.GetDateTime();
				
				if(comma.IsEOS()) continue;
				comma.GetString(ref imsi);				
				if(getHourMinData(imsi, ref d, true) == false) continue;				

				data = new TREND_DI_STRUCT();
				val = new short[3];
				notExistCount = 0;
				for(i = 0; i < 3; i++, notExistCount = 0) 
				{
					if(comma.IsEOS()) break;
					comma.GetString(ref imsi);					
					if(string.Compare(imsi, 0, "..", 0, 2, true) == 0) 
					{
						notExistCount++;
						val[i] = 0;
					}
					else if(i == 0 && string.Compare(imsi, 0, "ON", 0, 2, true) == 0) 
					{						
						val[i] = 1;
					}
					else 
					{
						val[i] = ConvertTool.ToInt16(imsi);
					}
				}
				if(i < 3) continue;			// 모든 데이터가 있지 않을 때
				data.bOnOff = (byte)val[0];
				data.nCountOnOff = val[1];
				data.cOnTime = (byte)val[2];
				if(notExistCount >= 3) DataSave.SaveMinDataStructDI(di.tag, d, data, false);
				else				   DataSave.SaveMinDataStructDI(di.tag, d, data, true);
				count++;
			}
			reader.Close();

			string		msg = string.Format("{0}", count);
			if(Tools.IsLangKorean()) MessageBox.Show(msg+" 개의 분 자료 읽기 완료.", "파일읽기 완료", MessageBoxButtons.OK);
			else					 MessageBox.Show(msg+" Minutes Text Data Read OK.", "File Read OK", MessageBoxButtons.OK);
		}

		void diHourDataTextToFile(FileStream fs)
		{
			TextReader reader = new StreamReader(fs);
			if(reader == null) 
			{
				FileReadErrorMessage();
				return;
			}

			int					i, count = 0, notExistCount = 0;
			DateTime			d = new DateTime(DataEditConfig.dTime.Year, DataEditConfig.dTime.Month, DataEditConfig.dTime.Day, DataEditConfig.dTime.Hour, DataEditConfig.dTime.Minute, 0);
			HOUR_DATA_DIGITAL_STRUCT		data;
			CommaBlockString	comma = new CommaBlockString();
			string				one_line, imsi = "";			
			uint[]				val;

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null || one_line.Length == 0) break;

				comma.Set(one_line);
				if(comma.IsEOS()) continue;
				d = comma.GetDateTime();
				
				if(comma.IsEOS()) continue;
				comma.GetString(ref imsi);				
				if(getHourMinData(imsi, ref d, false) == false) continue;				

				data = new HOUR_DATA_DIGITAL_STRUCT();
				val = new uint[2];
				notExistCount = 0;
				for(i = 0; i < 2; i++, notExistCount = 0)
				{
					if(comma.IsEOS()) break;
					comma.GetString(ref imsi);					
					if(string.Compare(imsi, 0, "..", 0, 2, true) == 0) 
					{
						notExistCount++;
						val[i] = 0;
					}
					else 
					{
						val[i] = ConvertTool.ToUInt32(imsi);
					}
				}
				if(i < 2) continue;			// 모든 데이터가 있지 않을 때
				data.wCountOnOff = (ushort)val[0];
				data.dwOnTime = val[1];
				if(notExistCount >= 2) data.flag = 0;
				else				   data.flag = 1;
				DataSave.SaveHourDataStructDI(di.tag, d, data);
				count++;
			}
			reader.Close();

			string		msg = string.Format("{0}", count);
			if(Tools.IsLangKorean()) MessageBox.Show(msg+" 개의 시간 자료 읽기 완료.", "파일읽기 완료", MessageBoxButtons.OK);
			else					 MessageBox.Show(msg+" Hours Text Data Read OK.", "File Read OK", MessageBoxButtons.OK);
		}

		bool checkFileExist()
		{
			if(File.Exists(this.textBox_Filename.Text) == false) 
			{
				string		msg;
				if(Tools.IsLangKorean()) 
				{
					msg = string.Format("지정한 파일이 존재하지 않습니다. ({0})", this.textBox_Filename.Text);
					MessageBox.Show(msg, "파일없음", MessageBoxButtons.OK);
					return false;
				}
				else 
				{
					msg = string.Format("Selected Data File Does Not Exist. ({0})", this.textBox_Filename.Text);
					MessageBox.Show(msg, "File Does Not Exist", MessageBoxButtons.OK);
					return false;
				}
			}
			return true;
		}


		private void button_ReadText_Click(object sender, System.EventArgs e)
		{
			if(di == null) return;
			if(checkFileExist() == false) return;
			
			FileStream fs = File.OpenRead(textBox_Filename.Text);			
			if(fs == null)	
			{
				FileReadErrorMessage();
				return;
			}

			readCurrentDateTime();
			if(this.radioButton_MinDataType.Checked) diMinDataTextToFile(fs);
			else									 diHourDataTextToFile(fs);
			fs.Close();
		}

		private void DialogDiTextSaveLoad_Closed(object sender, System.EventArgs e)
		{
			readCurrentDateTime();
			DataEditConfig.nTextSaveLoadCount = (int)this.numericUpDown_DataCount.Value;
			DataEditConfig.nTextSaveLoadType = (radioButton_HourDataType.Checked) ? 1 : 0;
			DataEditConfig.sTextFilename = this.textBox_Filename.Text;
		}		
		
		

	}
}
