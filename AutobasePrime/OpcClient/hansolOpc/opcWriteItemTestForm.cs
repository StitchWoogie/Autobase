using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opc.Da;

namespace OpcClient
{
	/// <summary>
	/// Summary description for opcWriteItemTestForm.
	/// </summary>
	public class opcWriteItemTestForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1; 
		public System.Windows.Forms.TextBox textBox_writeValue;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Label label_serverName;
		private System.Windows.Forms.Label label_groupName;
		private System.Windows.Forms.Label label_itemName;
		public	string hostName = "", serverName, groupName, itemName;
		public	int		nServer, nGroup, nItem;
		public	opcServerReadWriteClass opcServer;
		public	opcItemReadWriteClass opcItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public opcWriteItemTestForm()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(opcWriteItemTestForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_itemName = new System.Windows.Forms.Label();
            this.label_groupName = new System.Windows.Forms.Label();
            this.label_serverName = new System.Windows.Forms.Label();
            this.textBox_writeValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Close = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label_itemName);
            this.groupBox1.Controls.Add(this.label_groupName);
            this.groupBox1.Controls.Add(this.label_serverName);
            this.groupBox1.Controls.Add(this.textBox_writeValue);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label_itemName
            // 
            this.label_itemName.AccessibleDescription = null;
            this.label_itemName.AccessibleName = null;
            resources.ApplyResources(this.label_itemName, "label_itemName");
            this.label_itemName.Font = null;
            this.label_itemName.Name = "label_itemName";
            // 
            // label_groupName
            // 
            this.label_groupName.AccessibleDescription = null;
            this.label_groupName.AccessibleName = null;
            resources.ApplyResources(this.label_groupName, "label_groupName");
            this.label_groupName.Font = null;
            this.label_groupName.Name = "label_groupName";
            // 
            // label_serverName
            // 
            this.label_serverName.AccessibleDescription = null;
            this.label_serverName.AccessibleName = null;
            resources.ApplyResources(this.label_serverName, "label_serverName");
            this.label_serverName.Font = null;
            this.label_serverName.Name = "label_serverName";
            // 
            // textBox_writeValue
            // 
            this.textBox_writeValue.AccessibleDescription = null;
            this.textBox_writeValue.AccessibleName = null;
            resources.ApplyResources(this.textBox_writeValue, "textBox_writeValue");
            this.textBox_writeValue.BackgroundImage = null;
            this.textBox_writeValue.Font = null;
            this.textBox_writeValue.Name = "textBox_writeValue";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // button_Close
            // 
            this.button_Close.AccessibleDescription = null;
            this.button_Close.AccessibleName = null;
            resources.ApplyResources(this.button_Close, "button_Close");
            this.button_Close.BackgroundImage = null;
            this.button_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Close.Font = null;
            this.button_Close.Name = "button_Close";
            // 
            // button_OK
            // 
            this.button_OK.AccessibleDescription = null;
            this.button_OK.AccessibleName = null;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.BackgroundImage = null;
            this.button_OK.Font = null;
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // opcWriteItemTestForm
            // 
            this.AcceptButton = this.button_OK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.button_Close;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "opcWriteItemTestForm";
            this.ShowInTaskbar = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.opcWriteItemTestForm_KeyDown);
            this.Load += new System.EventHandler(this.opcWriteItemTestForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void opcWriteItemTestForm_Load(object sender, System.EventArgs e)
		{
            if(hostName.Length > 0)     // add 2009-01-22
                this.label_serverName.Text += hostName + "." + serverName;
            else
			    this.label_serverName.Text += this.serverName;
			this.label_groupName.Text += this.groupName;
			this.label_itemName.Text += this.itemName;
			this.textBox_writeValue.Text = opcItem.readData.ToString();			
		}

		void writeOneItem(opcServerReadWriteClass opcServer, opcItemReadWriteClass opcItem, object val)
		{
			if(opcServer.m_server == null || opcServer.m_server.IsConnected == false) 
			{
				if(NetTools.Tools.IsLangKorean()) 
					MessageBox.Show("선택한 아이템은 서버에 연결되지 않았습니다.");
				else
					MessageBox.Show("Selected Server does not Connection.");
				return;
			}
			ArrayList				arrItems;
			Opc.Da.ItemValue		oneItem = new ItemValue();
			Opc.Da.ItemValue[]		items;

			arrItems = new ArrayList();
			oneItem.ItemName = opcItem.itemName;
			oneItem.Value = val;
			arrItems.Add(oneItem);
			
			Opc.IdentifiedResult[] results  = null;
			items = (Opc.Da.ItemValue[])arrItems.ToArray(typeof(ItemValue));
            try
            {
                results = opcServer.m_server.Write(items);
            }
            catch
            {
                results = null;
            }
			//results.
			if (results == null)// || results[0].ResultID == null) 
			{
				if(NetTools.Tools.IsLangKorean()) 
					MessageBox.Show("아이템 쓰기 실패. 서버에서 쓰기 에러.");
				else
					MessageBox.Show("Write Item Error.");
				return;
			}
            if (opcBasic.opcClientConfig.bUseDeviceReadMode)        // 2008-10-31 add
            {
                ItemValueResult[] readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, true);     // device 모드로 읽고
                if (readResults != null)
                {
                    opcItem.bNewRead = true;
                    opcItem.reuslt = readResults[0].ResultID;
                    opcItem.timeStamp = readResults[0].Timestamp;
                    opcItem.quality = readResults[0].Quality;
                    if (readResults[0].Value == null)
                    {
                        opcItem.bFlag = false;
                    }
                    else
                    {
                        opcItem.bFlag = true;
                        if (opcItem.readData != (object)readResults[0].Value) // 값이 바뀌었으면
                        {
                            opcItem.readData = (object)readResults[0].Value;
                        }
                    }
                }
            }

			if(opcItem.reuslt != Opc.ResultID.S_OK) //== Opc.ResultID.E_WRITEONLY) modify 2004-07-23
			{
				opcItem.bNewRead = true;
				opcItem.readData = val;	// 쓰기전용일 때는 쓴 값을 대입, 2004-05-03 추가
			}
		}


		int getWriteItemValueObject(object type, out object val)
		{
			val = (int)0;
			if(opcBasic.isValueObjectIsSingleType(type) == false) return 0;
			try
			{
				if(type.GetType() == Opc.Type.INT) 
				{
					val = (int)Convert.ToDouble(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.SBYTE) 
				{
					val = (sbyte)Convert.ToDouble(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.FLOAT) 
				{
					val = (float)Convert.ToDouble(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.DOUBLE) 
				{
					val = Convert.ToDouble(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.STRING) 
				{
					val = (string)this.textBox_writeValue.Text;
					return 1;
				}
				if(type.GetType() == Opc.Type.BYTE) {
					val = Convert.ToByte(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.SHORT) 
				{
					val = Convert.ToInt16(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.USHORT) 
				{
					val = Convert.ToUInt16(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.UINT) 
				{
					val = Convert.ToInt32(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.LONG) 
				{
					val = Convert.ToInt64(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.ULONG) 
				{
					val = Convert.ToUInt64(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.DECIMAL) 
				{
					val = Convert.ToDecimal(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.BOOLEAN) 
				{
					if(String.Compare(textBox_writeValue.Text, 0,  "1", 0, 1, true) == 0) 
					{
						val = Convert.ToBoolean("True");
						return 1;
					}
					if(String.Compare(textBox_writeValue.Text, 0, "0", 0, 1, true) == 0) 
					{
						val = Convert.ToBoolean("False");
						return 1;
					}
					val = Convert.ToBoolean(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.DATETIME) 
				{
					val = Convert.ToDateTime(this.textBox_writeValue.Text);
					return 1;
				}
				if(type.GetType() == Opc.Type.DURATION) 
				{
					val = Convert.ToInt64(this.textBox_writeValue.Text);
					return 1;
				}				
			}
			catch
			{
				return 2;
			}
			return 0;
		}


		void writeCurrentValue()
		{
			object		val;
			if(opcItem.reuslt != Opc.ResultID.S_OK)// == Opc.ResultID.E_WRITEONLY)  modify 2004-07-23
			{
				ItemValue item = new ItemValue();
				item.ItemName = opcItem.itemName;
				opcBasic.GetDefaultValues(opcServer.m_server, new ItemValue[] { item });
				if(item.Value == null) opcItem.readData = 0;			// Null 일 때는 기본으로 Int32로 설정
				else				   opcItem.readData = item.Value;
			}
			int	retn = getWriteItemValueObject(opcItem.readData, out val);
			switch(retn) 
			{
				case 1 : writeOneItem(opcServer, opcItem, val); break;
				case 2 : 
					if(NetTools.Tools.IsLangKorean()) 
						MessageBox.Show("입력한 데이터가 맞지 않습니다. 알맞은 데이터를 입력해 주세요."); 
					else
						MessageBox.Show("Input Data Error. Please Input Correct Data Type."); 
					break;
				case 0 : 
					if(NetTools.Tools.IsLangKorean()) 
						MessageBox.Show("선택한 아이템은 쓰기 테스트를 지원하지 않습니다. 통신/감시 프로그램과 연결하여 사용해 주세요.");
					else
						MessageBox.Show("Unsupported Test Write Item."); 
					break;
			}			
		}


		private void button_OK_Click(object sender, System.EventArgs e)
		{
			writeCurrentValue();
			//this.DialogResult = DialogResult.OK;
			//this.Close();
		}

		private void opcWriteItemTestForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Enter : writeCurrentValue(); return;				
			}
		}
	}
}
