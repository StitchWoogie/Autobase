using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for edit_AiMinListDlg.
	/// </summary>
	public class edit_AiMinListDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button button_FieldDelete;
		private System.Windows.Forms.Button button_FieldAdd;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radio_MIN_DATA;
		private System.Windows.Forms.RadioButton radio_HOUR_DATA;
        private System.Windows.Forms.GroupBox groupBox4;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public bool bOkFlag = false;
		public int	nDataTypePos;
		public int	nMinListColumnCount = 6;
		public System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		public System.Windows.Forms.ComboBox comboBox_Data_Type;
		private System.Windows.Forms.Button button_FieldModify;
		private System.Windows.Forms.Button button_FieldInsert;	// 평균, 최소, 최대, 적산, 최대값차이
		public string[] sMinListColumnName = {"AVE", "MIN", "MAX", "SUM", "SUB", "MOMENT"};
        public TextBox textBox_DATA_PERIOD;
		string[] OrderString = { "StartOrder", "EndOrder" };

		public edit_AiMinListDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_AiMinListDlg));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button_FieldInsert = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.button_FieldDelete = new System.Windows.Forms.Button();
            this.button_FieldModify = new System.Windows.Forms.Button();
            this.button_FieldAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_Data_Type = new System.Windows.Forms.ComboBox();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radio_HOUR_DATA = new System.Windows.Forms.RadioButton();
            this.radio_MIN_DATA = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox_DATA_PERIOD = new System.Windows.Forms.TextBox();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button_FieldInsert);
            this.groupBox5.Controls.Add(this.listView1);
            this.groupBox5.Controls.Add(this.button_FieldDelete);
            this.groupBox5.Controls.Add(this.button_FieldModify);
            this.groupBox5.Controls.Add(this.button_FieldAdd);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // button_FieldInsert
            // 
            resources.ApplyResources(this.button_FieldInsert, "button_FieldInsert");
            this.button_FieldInsert.Name = "button_FieldInsert";
            this.button_FieldInsert.Click += new System.EventHandler(this.button_FieldInsert_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.FullRowSelect = true;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // button_FieldDelete
            // 
            resources.ApplyResources(this.button_FieldDelete, "button_FieldDelete");
            this.button_FieldDelete.Name = "button_FieldDelete";
            this.button_FieldDelete.Click += new System.EventHandler(this.button_FieldDelete_Click);
            // 
            // button_FieldModify
            // 
            resources.ApplyResources(this.button_FieldModify, "button_FieldModify");
            this.button_FieldModify.Name = "button_FieldModify";
            this.button_FieldModify.Click += new System.EventHandler(this.button_FieldModify_Click);
            // 
            // button_FieldAdd
            // 
            resources.ApplyResources(this.button_FieldAdd, "button_FieldAdd");
            this.button_FieldAdd.Name = "button_FieldAdd";
            this.button_FieldAdd.Click += new System.EventHandler(this.button_FieldAdd_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_Data_Type);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboBox_Data_Type
            // 
            resources.ApplyResources(this.comboBox_Data_Type, "comboBox_Data_Type");
            this.comboBox_Data_Type.Name = "comboBox_Data_Type";
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radio_HOUR_DATA);
            this.groupBox3.Controls.Add(this.radio_MIN_DATA);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radio_HOUR_DATA
            // 
            resources.ApplyResources(this.radio_HOUR_DATA, "radio_HOUR_DATA");
            this.radio_HOUR_DATA.Name = "radio_HOUR_DATA";
            // 
            // radio_MIN_DATA
            // 
            resources.ApplyResources(this.radio_MIN_DATA, "radio_MIN_DATA");
            this.radio_MIN_DATA.Name = "radio_MIN_DATA";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox_DATA_PERIOD);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // textBox_DATA_PERIOD
            // 
            resources.ApplyResources(this.textBox_DATA_PERIOD, "textBox_DATA_PERIOD");
            this.textBox_DATA_PERIOD.Name = "textBox_DATA_PERIOD";
            // 
            // edit_AiMinListDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_AiMinListDlg";
            this.Load += new System.EventHandler(this.edit_AiMinListDlg_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void edit_AiMinListDlg_Load(object sender, System.EventArgs e)
		{
			if(nDataTypePos == 1) this.radio_HOUR_DATA.Checked = true;
			else this.radio_MIN_DATA.Checked = true;		
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(this.comboBox_Data_Type.Text.Length <= 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("명렁인식 문장을 설정해야 합니다.", "설정오류");
				else
					MessageBox.Show("Please Select Command Name.", "Select Error");
				return;
			}
			if(this.listView1.Items.Count <= 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("적어도 하나 이상의 자료목록을 입력해야 합니다.", "입력오류");
				else
					MessageBox.Show("Please Input Tag Data Object.", "Input Error");
				return;
			}

			if(this.radio_HOUR_DATA.Checked)	this.nDataTypePos = 1;
			else								this.nDataTypePos = 0;

			bOkFlag = true;
			Close();
		}

		private bool listCountIsMaxCountAndShowMessage()
		{
			if(this.listView1.Items.Count >= 128) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("등록된 Column 이름 수가 128개를 초과 하였습니다.", "입력오류");
				else
					MessageBox.Show("Registed Column Count >= 128.", "Input error");
				return true;
			}
			return false;
		}

		private void button_FieldAdd_Click(object sender, System.EventArgs e)
		{
			if(listCountIsMaxCountAndShowMessage()) return;

			ListViewItem item;
			edit_AiMinListAddTagDlg dialog = new edit_AiMinListAddTagDlg();

			dialog.nDataTypePos = 0;	// 평균값
			dialog.nOrderTypePos = 0;	// 시작시간
			dialog.ShowDialog();
			if(dialog.bOkFlag == true) 
			{
				item = new ListViewItem();
				if(dialog.checkBox1.Checked) 
				{
					item.Text = OrderString[dialog.comboBox_Order.SelectedIndex % 2];
					item.SubItems.Add(OrderString[dialog.comboBox_Order.SelectedIndex % 2]);
				}
				else 
				{
					item.Text = dialog.textBox_Tag.Text;
					item.SubItems.Add(this.sMinListColumnName[dialog.comboBox_TagType.SelectedIndex % this.nMinListColumnCount]);
				}
				listView1.Items.Add(item);				
			}
		}

		private void button_FieldInsert_Click(object sender, System.EventArgs e)
		{
			if(listCountIsMaxCountAndShowMessage()) return;

			ListViewItem item;
			edit_AiMinListAddTagDlg dialog = new edit_AiMinListAddTagDlg();

			dialog.checkBox1.Checked = false;
			dialog.nDataTypePos = 0;	// 평균값
			dialog.nOrderTypePos = 0;	// 시작시간
			dialog.ShowDialog();
			if(dialog.bOkFlag == true) 
			{
				item = new ListViewItem();
				if(dialog.checkBox1.Checked) 
				{
					item.Text = OrderString[dialog.comboBox_Order.SelectedIndex % 2];
					item.SubItems.Add(OrderString[dialog.comboBox_Order.SelectedIndex % 2]);
				}
				else 
				{
					item.Text = dialog.textBox_Tag.Text;
					item.SubItems.Add(this.sMinListColumnName[dialog.comboBox_TagType.SelectedIndex % this.nMinListColumnCount]);
				}
				if(listView1.Items.Count <= 0 || listView1.SelectedItems.Count <= 0) 
					listView1.Items.Add(item);
				else
					listView1.Items.Insert(listView1.SelectedIndices[0], item);
			}		
		}

		int dataTypeToPos(string dataType)
		{
			int		i;

			for(i = 0; i < this.nMinListColumnCount; i++) 
			{
				if(this.sMinListColumnName[i] == dataType) return i;
			}
			return 0;
		}

		void modifyCurrentListView() 
		{
			if(listView1.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("수정하고 싶은 항목을 선택한 후 다시 하세요.", "선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要修改的项。", "选择错误");
				else
					MessageBox.Show("Please Select Item. (Modify)", "Select Error");
				return;
			}

			ListViewItem		item;
			int					index = this.listView1.SelectedItems[0].Index;

			item = listView1.Items[index];
			edit_AiMinListAddTagDlg dialog = new edit_AiMinListAddTagDlg();

			if(item.SubItems[0].Text == OrderString[0] && item.SubItems[1].Text == OrderString[0])
			{
				dialog.checkBox1.Checked = true;
				dialog.nOrderTypePos = 0;	// 시작시간
			}
			else if(item.SubItems[0].Text == OrderString[1] && item.SubItems[1].Text == OrderString[1])
			{
				dialog.checkBox1.Checked = true;
				dialog.nOrderTypePos = 1;	// 끝시간
			}
			else 
			{
				dialog.textBox_Tag.Text = item.SubItems[0].Text;
				dialog.nDataTypePos = dataTypeToPos(item.SubItems[1].Text) % this.nMinListColumnCount;
			}
			dialog.ShowDialog();
			if(dialog.bOkFlag == true) 
			{
				if(dialog.checkBox1.Checked) 
				{
					item.Text = OrderString[dialog.comboBox_Order.SelectedIndex % 2];
					item.SubItems[1].Text = (OrderString[dialog.comboBox_Order.SelectedIndex % 2]);
				}
				else 
				{
					item.Text = dialog.textBox_Tag.Text;
					//item.SubItems.Add(this.sMinListColumnName[dialog.comboBox_TagType.SelectedIndex % this.nMinListColumnCount]);			
					item.SubItems[1].Text = (this.sMinListColumnName[dialog.comboBox_TagType.SelectedIndex % this.nMinListColumnCount]);			
				}
			}
		}


		private void button_FieldDelete_Click(object sender, System.EventArgs e)
		{
			if(this.listView1.SelectedItems.Count == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("삭제하고 싶은 항목을 선택한 후 다시 하세요.", "선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选择要删除的项。", "选择错误");
				else 
					MessageBox.Show("Please Select Item. (Delete)", "Select Error");
				return;
			}

			int index = this.listView1.SelectedItems[0].Index;

			listView1.Items.RemoveAt(index);			
		}
		

		private void button_FieldModify_Click(object sender, System.EventArgs e)
		{
			modifyCurrentListView();
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			modifyCurrentListView();
		}

		
	}
}
