using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DatabaseConnection;
using NetTools;
using AutoLibLocal;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for ReportDataIdentifyTypeSetting.
	/// </summary>
	public class ReportDataIdentifyTypeSetting : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.ComboBox comboBox_Data_Type;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox textBox_CHAR;
		private System.Windows.Forms.Button button_DB_Connect_Setting;
		public System.Windows.Forms.ComboBox comboBox_DSN_Name;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.ListView m_list_main;
		public	bool bOkFlag;
		public	bool bAddFlag;
		public System.Windows.Forms.TextBox textBox_None_Data_String;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label8;
		public System.Windows.Forms.TextBox textBoxDbTimeColumnName;
		public System.Windows.Forms.TextBox textBoxDbMilliSecondColumnName;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.ComboBox comboBoxTimeColumnFormat;
		private System.Windows.Forms.Label label6;
		ConnectionStringList		dsnList;
		memberConfigStruct			oldMember;
		public memberConfigStruct	newMember = new memberConfigStruct();

		public ReportDataIdentifyTypeSetting(ListView m_list, ConnectionStringList dsn, memberConfigStruct mem, bool bAdd)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//			
			m_list_main = m_list;
			dsnList = dsn;
			oldMember = mem;
			bAddFlag = bAdd;
			bOkFlag = false;			
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportDataIdentifyTypeSetting));
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_CHAR = new System.Windows.Forms.TextBox();
            this.comboBox_Data_Type = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_None_Data_String = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button_DB_Connect_Setting = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_DSN_Name = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDbMilliSecondColumnName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDbTimeColumnName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxTimeColumnFormat = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox_CHAR
            // 
            resources.ApplyResources(this.textBox_CHAR, "textBox_CHAR");
            this.textBox_CHAR.Name = "textBox_CHAR";
            // 
            // comboBox_Data_Type
            // 
            this.comboBox_Data_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox_Data_Type, "comboBox_Data_Type");
            this.comboBox_Data_Type.Name = "comboBox_Data_Type";
            this.comboBox_Data_Type.SelectedIndexChanged += new System.EventHandler(this.comboBox_Data_Type_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_None_Data_String);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button_DB_Connect_Setting);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox_DSN_Name);
            this.groupBox1.Controls.Add(this.comboBox_Data_Type);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_CHAR);
            this.groupBox1.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBox_None_Data_String
            // 
            resources.ApplyResources(this.textBox_None_Data_String, "textBox_None_Data_String");
            this.textBox_None_Data_String.Name = "textBox_None_Data_String";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // button_DB_Connect_Setting
            // 
            resources.ApplyResources(this.button_DB_Connect_Setting, "button_DB_Connect_Setting");
            this.button_DB_Connect_Setting.Name = "button_DB_Connect_Setting";
            this.button_DB_Connect_Setting.Click += new System.EventHandler(this.button_DB_Connect_Setting_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBox_DSN_Name
            // 
            resources.ApplyResources(this.comboBox_DSN_Name, "comboBox_DSN_Name");
            this.comboBox_DSN_Name.Name = "comboBox_DSN_Name";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxDbMilliSecondColumnName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxDbTimeColumnName);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.comboBoxTimeColumnFormat);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textBoxDbMilliSecondColumnName
            // 
            resources.ApplyResources(this.textBoxDbMilliSecondColumnName, "textBoxDbMilliSecondColumnName");
            this.textBoxDbMilliSecondColumnName.Name = "textBoxDbMilliSecondColumnName";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxDbTimeColumnName
            // 
            resources.ApplyResources(this.textBoxDbTimeColumnName, "textBoxDbTimeColumnName");
            this.textBoxDbTimeColumnName.Name = "textBoxDbTimeColumnName";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // comboBoxTimeColumnFormat
            // 
            this.comboBoxTimeColumnFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxTimeColumnFormat, "comboBoxTimeColumnFormat");
            this.comboBoxTimeColumnFormat.Name = "comboBoxTimeColumnFormat";
            // 
            // ReportDataIdentifyTypeSetting
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportDataIdentifyTypeSetting";
            this.Load += new System.EventHandler(this.ReportDataIdentifyTypeSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void comboBox_Data_Type_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private bool checkEqualChar()
		{
			int			i;

			for(i = 0; i < m_list_main.Items.Count; i++) 
			{
				if(m_list_main.Items[i].Text == textBox_CHAR.Text) return true;
			}
			return false;
		}

		bool checkDsnIsNeeded()
		{
			if(comboBox_Data_Type.SelectedIndex >= 0 && comboBox_Data_Type.SelectedIndex < BasicRptTool.data[0].dataTypeMaxCount) return true;
			return false;
		}

		void readCurrentSettingData()
		{
			newMember.dataName = textBox_CHAR.Text;
			newMember.sMainDataTypeBuf = comboBox_Data_Type.Text;
			newMember.sDataTitle = BasicRptTool.getPosFromDataTitle(comboBox_Data_Type.SelectedIndex);			
			newMember.noneDataString = textBox_None_Data_String.Text;
			newMember.dsnName = this.comboBox_DSN_Name.Text;
			newMember.sDateColumnName = textBoxDbTimeColumnName.Text;
			newMember.sMilliSecondColumnName = textBoxDbMilliSecondColumnName.Text;
			newMember.nTimeFormat = comboBoxTimeColumnFormat.SelectedIndex % BasicRptTool.nDbTimeColumnFormatCount;
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(this.comboBox_Data_Type.Text.Length == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("데이터 종류를 선택해야 합니다.", "선택오류");
				else
					MessageBox.Show("Please Select Data Type.", "Select error");
				return;
			}
			if(textBox_CHAR.Text.Length == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("명렁인식 문장을 입력해야 합니다.", "입력오류");
				else
					MessageBox.Show("Please Input Command Name.", "Input Error");
				return;				
			}
			if(checkDsnIsNeeded())
			{
				if(comboBox_DSN_Name.Text.Length == 0) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("DSN 종류를 설정해야 합니다.", "설정오류");
					else
						MessageBox.Show("Please Select DSN Name.", "Select Error");
					return;
				}
				if(this.textBoxDbTimeColumnName.Text.Length == 0) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("DB 시간컬럼 이름을 입력해야 합니다.", "입력오류");
					else
						MessageBox.Show("Please Input DB Time Column Name.", "Input Error");
					return;
				}
				if(this.textBoxDbMilliSecondColumnName.Text.Length == 0) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("DB Milli Second 컬럼 이름을 입력해야 합니다.", "입력오류");
					else
						MessageBox.Show("Please Input DB Milli Time Column Name.", "Input Error");
					return;
				}
				if(this.comboBoxTimeColumnFormat.Text.Length == 0) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("시간컬럼 종류를 설정해야 합니다.", "설정오류");
					else
						MessageBox.Show("Please Select Time Column Format.", "Select Error");
					return;
				}
			}
			if(checkEqualChar() && bAddFlag == true) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("동일한 인식 문자열이 존재합니다. 다른 문자열을 입력하십시요.", "입력오류");
				else
					MessageBox.Show("Identical Command Name Exist. Please Another String.", "Input Error");
				return;
			}
			readCurrentSettingData();
			bOkFlag = true;
			Close();
		}

		void AddFormTitleText()
		{
			if(this.bAddFlag) 
			{
				if(Tools.IsLangKorean()) 
				{
					this.Text += " 추가";
				}
				else 
				{
					this.Text += " Insert";
				}
			}
			else 
			{
				if(Tools.IsLangKorean()) 
				{
					this.Text += " 수정";
				}
				else 
				{
					this.Text += " Modify";
				}
			}
		}

		void EnableDisable()
		{
			bool flag = checkDsnIsNeeded();

			comboBox_DSN_Name.Enabled = flag;
			button_DB_Connect_Setting.Enabled = flag;
			this.groupBox2.Enabled = flag;			
		}

		void UpdateDbTimeColumnFormat()
		{
			this.comboBoxTimeColumnFormat.Items.Clear();

			for(int i = 0; i < BasicRptTool.nDbTimeColumnFormatCount; i++) 
			{
				this.comboBoxTimeColumnFormat.Items.Add(BasicRptTool.sTimeColumnFormat[i]);
			}
		}

		void UpdateCommandType()
		{
			int		i, pos;

			for(pos = 0; pos < 4; pos++) 
			{
				for(i = 0; i < BasicRptTool.data[pos].dataTypeMaxCount; i++)
					comboBox_Data_Type.Items.Add(BasicRptTool.data[pos].sMainDataTypeBuf[i]);
			}
		}

		void setMemberData()
		{
			try 
			{
				if(this.bAddFlag) 
				{
					comboBox_Data_Type.Text = BasicRptTool.data[0].sMainDataTypeBuf[0];
					textBox_CHAR.Text = "DATA";
					textBox_None_Data_String.Text = "***";
					this.textBoxDbTimeColumnName.Text = "DATASAVEDTIME";
					this.textBoxDbMilliSecondColumnName.Text = "MILLISECOND";
					this.comboBoxTimeColumnFormat.SelectedIndex = 0;
					this.comboBox_DSN_Name.SelectedIndex = 0;
				}
				else 
				{
					comboBox_Data_Type.Text = oldMember.sMainDataTypeBuf;
					textBox_CHAR.Text = oldMember.dataName;
					comboBox_DSN_Name.Text = oldMember.dsnName;
					textBox_None_Data_String.Text = oldMember.noneDataString;
					this.textBoxDbTimeColumnName.Text = oldMember.sDateColumnName;
					this.textBoxDbMilliSecondColumnName.Text = oldMember.sMilliSecondColumnName;
					this.comboBoxTimeColumnFormat.SelectedIndex = oldMember.nTimeFormat;
				}
			}
			catch
			{
			}
		}

		private void ReportDataIdentifyTypeSetting_Load(object sender, System.EventArgs e)
		{
			AddFormTitleText();
			EnableDisable();
			UpdateDsnList();
			UpdateDbTimeColumnFormat();
			UpdateCommandType();
			setMemberData();
		}

		void UpdateDsnList()
		{
			ConnectionString dsn;

			this.comboBox_DSN_Name.Items.Clear();

			for(int i = 0; i < dsnList.arrayConnectionString.Count; i++) 
			{
				dsn = (ConnectionString)dsnList.arrayConnectionString[i];
				this.comboBox_DSN_Name.Items.Add(dsn.title);
			}
		}

		private void button_DB_Connect_Setting_Click(object sender, System.EventArgs e)
		{
			FormDatabaseConnection dialog = new FormDatabaseConnection(dsnList);

			if(dialog.ShowDialog() == DialogResult.OK) 
			{
				UpdateDsnList();
			}
		}
	}
}
