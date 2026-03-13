using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLib;
using AutoLibLocal;
using DialogTag;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewRegisteredGroupDataInputDlg.
	/// </summary>
	public class ViewRegisteredGroupDataInputDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1;
		public  System.Windows.Forms.TextBox textBox_GroupName;
		private System.Windows.Forms.Button button_MoveUp;
		private System.Windows.Forms.Button button_MoveDown;
		private System.Windows.Forms.Button button_TagInsert;
		private System.Windows.Forms.Button button_TagAdd;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button_Insert;
		private System.Windows.Forms.Button button_Add;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public  System.Windows.Forms.ComboBox comboBox_TagType;
		public  System.Windows.Forms.ComboBox comboBox_Element;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		private System.Windows.Forms.Button button_TagDelete;
		private System.Windows.Forms.TextBox textBox_Condition;
		public  groupShowTagGroupMember	grBuf = new groupShowTagGroupMember();
		//groupShowTagMember		item = new groupShowTagMember();
		TagListStruct[]			tagList;

		int[]					headWidth = new int[3];
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox3;
		ControlListView			list = new ControlListView();

		public ViewRegisteredGroupDataInputDlg(groupShowTagGroupMember group)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.panel1.Controls.Add(list);
			basicElementSetting();
			headWidth[0] = 60;
			headWidth[1] = 150;
			headWidth[2] = 80;

			if(group == null) grBuf.tag = new ArrayList();
			else 
			{
				//grBuf = (groupShowTagGroupMember)Tools.CopyObject(group);	// 속도가 늦기 때문에 사용하지 않는다
				grBuf.name = group.name;
				groupShowTagMember		item;				
				grBuf.tag = new ArrayList();
				for(int i = 0; i < group.tag.Count; i++) 
				{
					item = (groupShowTagMember)group.tag[i];
					grBuf.tag.Add(item);
				}
				grBuf.tagCount = group.tag.Count;
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewRegisteredGroupDataInputDlg));
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_GroupName = new System.Windows.Forms.TextBox();
            this.button_MoveUp = new System.Windows.Forms.Button();
            this.button_MoveDown = new System.Windows.Forms.Button();
            this.button_TagInsert = new System.Windows.Forms.Button();
            this.button_TagAdd = new System.Windows.Forms.Button();
            this.button_TagDelete = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_Element = new System.Windows.Forms.ComboBox();
            this.comboBox_TagType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Condition = new System.Windows.Forms.TextBox();
            this.button_Insert = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_cancel, "button_cancel");
            this.button_cancel.Name = "button_cancel";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_GroupName);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBox_GroupName
            // 
            resources.ApplyResources(this.textBox_GroupName, "textBox_GroupName");
            this.textBox_GroupName.Name = "textBox_GroupName";
            // 
            // button_MoveUp
            // 
            resources.ApplyResources(this.button_MoveUp, "button_MoveUp");
            this.button_MoveUp.Name = "button_MoveUp";
            this.button_MoveUp.Click += new System.EventHandler(this.button_MoveUp_Click);
            // 
            // button_MoveDown
            // 
            resources.ApplyResources(this.button_MoveDown, "button_MoveDown");
            this.button_MoveDown.Name = "button_MoveDown";
            this.button_MoveDown.Click += new System.EventHandler(this.button_MoveDown_Click);
            // 
            // button_TagInsert
            // 
            resources.ApplyResources(this.button_TagInsert, "button_TagInsert");
            this.button_TagInsert.Name = "button_TagInsert";
            this.button_TagInsert.Click += new System.EventHandler(this.button_TagInsert_Click);
            // 
            // button_TagAdd
            // 
            resources.ApplyResources(this.button_TagAdd, "button_TagAdd");
            this.button_TagAdd.Name = "button_TagAdd";
            this.button_TagAdd.Click += new System.EventHandler(this.button_TagAdd_Click);
            // 
            // button_TagDelete
            // 
            resources.ApplyResources(this.button_TagDelete, "button_TagDelete");
            this.button_TagDelete.Name = "button_TagDelete";
            this.button_TagDelete.Click += new System.EventHandler(this.button_TagDelete_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_Element);
            this.groupBox2.Controls.Add(this.comboBox_TagType);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button_Add);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox_Condition);
            this.groupBox2.Controls.Add(this.button_Insert);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboBox_Element
            // 
            this.comboBox_Element.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox_Element, "comboBox_Element");
            this.comboBox_Element.Items.AddRange(new object[] {
            resources.GetString("comboBox_Element.Items"),
            resources.GetString("comboBox_Element.Items1"),
            resources.GetString("comboBox_Element.Items2")});
            this.comboBox_Element.Name = "comboBox_Element";
            // 
            // comboBox_TagType
            // 
            this.comboBox_TagType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox_TagType, "comboBox_TagType");
            this.comboBox_TagType.Items.AddRange(new object[] {
            resources.GetString("comboBox_TagType.Items"),
            resources.GetString("comboBox_TagType.Items1"),
            resources.GetString("comboBox_TagType.Items2"),
            resources.GetString("comboBox_TagType.Items3"),
            resources.GetString("comboBox_TagType.Items4"),
            resources.GetString("comboBox_TagType.Items5")});
            this.comboBox_TagType.Name = "comboBox_TagType";
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
            // button_Add
            // 
            resources.ApplyResources(this.button_Add, "button_Add");
            this.button_Add.Name = "button_Add";
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox_Condition
            // 
            resources.ApplyResources(this.textBox_Condition, "textBox_Condition");
            this.textBox_Condition.Name = "textBox_Condition";
            // 
            // button_Insert
            // 
            resources.ApplyResources(this.button_Insert, "button_Insert");
            this.button_Insert.Name = "button_Insert";
            this.button_Insert.Click += new System.EventHandler(this.button_Insert_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // ViewRegisteredGroupDataInputDlg
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_TagDelete);
            this.Controls.Add(this.button_TagAdd);
            this.Controls.Add(this.button_TagInsert);
            this.Controls.Add(this.button_MoveDown);
            this.Controls.Add(this.button_MoveUp);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewRegisteredGroupDataInputDlg";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewRegisteredGroupDataInputDlg_Load);
            this.Closed += new System.EventHandler(this.ViewRegisteredGroupDataInputDlg_Closed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void ViewRegisteredGroupDataInputDlg_Load(object sender, System.EventArgs e)
		{
			list.paintMessage += new ControlListView.OnEventPaintMessage(onPaintMessage);
			//ShowCurrentRegisteredGroupTag();
			//SetListItemSelection(0);
			list.bOwnerDraw = true;
			list.bMultiRowSelect = true;
			listHeaderFill();
			list.Show();
			fillListDataAll();
			this.list.Select();				// list view 로 포커스를 이동
		}

		void listHeaderFill()
		{
			ControlListViewHeader	head;
			string[]				text = new string[3];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "순서";
				text[1] = "태그이름";
				text[2] = "태그종류";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "番号";
				text[1] = "タグ名";
				text[2] = "タグタイプ";
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "顺序";
				text[1] = "标记名";
				text[2] = "标记类型";
			}
			else 
			{
				text[0] = "No";
				text[1] = "Tag Name";
				text[2] = "Tag Type";
			}			

			for(int i = 0; i < 3; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();
				head.format.Alignment = StringAlignment.Center;
				list.header.Add(head);
			}
		}

		void basicElementSetting()
		{
			list.font = ConfigViewMain.fontMain;
			list.backColor = Color.White;//SharedData.colorTotal.BACK;
			list.textColor = Color.Black;////SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용			
			//list.colorFocus = Color.FromArgb(127, Color.LightBlue);
		}

		void fillListDataAll()
		{
			list.listHap = grBuf.tagCount;
			list.listItemChanged();
		}

		/*void ArrangeListItemPosNo(int pos, int end)
		{
			ListViewItem		viewItem;

			if(end > this.m_list.Items.Count) end = this.m_list.Items.Count;
			for(int i = pos; i < end; i++) 
			{
				viewItem = this.m_list.Items[i];
				if(viewItem == null) continue;
				viewItem.SubItems[0].Text = string.Format("{0}", i+1);
			}
		}

		void SetListItemSelection(int pos)
		{
			if(m_list.Items.Count <= 0) return;
			if(m_list.SelectedIndices.Count >= 1) return;
			if(pos >= m_list.Items.Count) pos = m_list.Items.Count - 1;

			try 
			{
				m_list.Items[pos].Selected = true;			
			}
			catch{}
		}

		void ShowCurrentRegisteredGroupTag()
		{
			int					i;
			ListViewItem		ViewItem;
			groupShowTagMember	item;

			m_list.Items.Clear();
			m_list.Hide();//.Visible = false;
			for(i = 0; i < grBuf.tagCount; i++)
			{
				item = (groupShowTagMember)grBuf.tag[i];
				ViewItem = new ListViewItem();

				ViewItem.Text = string.Format("{0}", i+1);
				ViewItem.SubItems.Add(item.tag);
				ViewItem.SubItems.Add(getTagTypeText(item.tagType));
				m_list.Items.Add(ViewItem);
			}
			m_list.Visible = true;
		}


		bool AddInsertViewList(int pos, string tag, EnumTagType tagType, bool bAdd, bool bMessage)
		{
			if(checkRegisteredTagCount(bMessage) == false) return false;

			ListViewItem		ViewItem = new ListViewItem();
			
			if(bAdd) ViewItem.Text = string.Format("{0}", this.m_list.Items.Count+1);
			else	 ViewItem.Text = string.Format("{0}", pos+1);
			ViewItem.SubItems.Add(tag);
			ViewItem.SubItems.Add(getTagTypeText(tagType));
			if(bAdd) m_list.Items.Add(ViewItem);
			else	 m_list.Items.Insert(pos, ViewItem);
			return true;
		}*/

		int getSelectedListStartPos()
		{
			int		pos;
			if(list.selectIndices.Count <= 0) pos = list.currPos;	// 선택된 리스트가 없으면 0번 위치에 삽입
			else pos = (int)list.selectIndices[0];
			if(pos <= 0) pos = 0;
			else if(pos >= grBuf.tagCount) pos = grBuf.tagCount-1;
			return pos;
		}

		string getTagTypeText(EnumTagType tagType)
		{
			switch(tagType) 
			{
				case AutoLibLocal.EnumTagType.AI : return "AI";					
				case AutoLibLocal.EnumTagType.AO : return "AO";					
				case AutoLibLocal.EnumTagType.DI : return "DI";					
				case AutoLibLocal.EnumTagType.DO : return "DO";					
				case AutoLibLocal.EnumTagType.ST : return "ST";					
				default :						   return "Tag Error";
			}
		}

		bool TagListAddInsert(string tag, bool bAdd, int listPos)
		{
			groupShowTagMember	item = new groupShowTagMember();
			item.tag = tag;
			if(TagLib.GetTagTypeAndPos(tag, ref item.tagType, ref item.tagPos) == false) return false;

			if(bAdd) grBuf.tag.Add(item);			
			else 	 grBuf.tag.Insert(listPos, item);			
			grBuf.tagCount++;
			return true;
		}

		void callTagDialog(bool bAdd, int listPos)
		{
			SelectTag dialog = new SelectTag();

			dialog.bUseTagAI = true;
			dialog.bUseTagAO = true;
			dialog.bUseTagDI = true;
			dialog.bUseTagDO = true;
			dialog.bUseTagST = true;


            if (dialog.Run(Form.ActiveForm) == DialogResult.OK) 
			{
                if (!CheckSameTag(dialog.sTag)) return;

				if(checkRegisteredTagCount(true) == false) return;
				if(TagListAddInsert(dialog.sTag, bAdd, listPos) == false) return;

				//AddInsertViewList(listPos, dialog.sTag, dialog.tagType, bAdd, false);
				if(bAdd == false) // Insert 시
				{
					//ArrangeListItemPosNo(listPos, m_list.Items.Count);
					//SetListItemSelection(listPos);
					//list.dataAreaInvalidate();
				}
				fillListDataAll();
			}
		}

		bool checkRegisteredTagCount(bool bMessage)
		{
			if(grBuf.tagCount < 10000) return true;
			if(bMessage == false) return false;
			if(Tools.IsLangKorean()) 
			{
				MessageBox.Show("등록된 태그가 10000개를 넘었습니다. 다른 그룹으로 사용해 주세요.");
			}
			else 
			{
				MessageBox.Show("Registered Tag Count > 10000.");
			}
			return false;
		}

        bool CheckSameTag(string tag)
        {
            groupShowTagMember item;
            for (int i = 0; i < grBuf.tag.Count; i++)
            {
                item = (groupShowTagMember)grBuf.tag[i];
                if (tag == item.tag)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("같은 태그가 이미 등록되어 있습니다.", "태그 중복");
                    else
                        MessageBox.Show("Same tag already added.", "Same tag exists.");
                    return false;
                }
            }
            return true;
        }

		private void button_TagInsert_Click(object sender, System.EventArgs e)
		{
			if(checkRegisteredTagCount(true) == false) return;
			callTagDialog(false, getSelectedListStartPos());
		}

		private void button_TagAdd_Click(object sender, System.EventArgs e)
		{
			if(checkRegisteredTagCount(true) == false) return;
			callTagDialog(true, 0);			// add 이므로 Pos 는 상관없음
		}

		
		private void button_TagDelete_Click(object sender, System.EventArgs e)
		{
			if(grBuf.tagCount <= 0 || list.listHap <= 0) return;

			int			i, pos;
			for(i = list.selectIndices.Count-1; i >= 0; i--) 
			{
				if(grBuf.tagCount <= 0) break;
				pos = (int)list.selectIndices[i];
				grBuf.tag.RemoveAt(pos);				
				grBuf.tagCount--;
			}
			list.selectIndices.Clear();
			fillListDataAll();
			//ShowCurrentRegisteredGroupTag();		// 삭제는 새로 그리는 것이 빠르다
			//SetListItemSelection(listPos);
		}

		void setSelectionItemChange(int pos)
		{
			list.selectIndices.Clear();
			list.currPos = pos;
			if(pos < list.startPos) 
			{
				list.startPos = pos;
			}
			else if(pos >= list.startPos + list.pageLineCount) 
			{
				if(list.pageLineCount <= 1) list.startPos = pos;
				else						list.startPos = pos-list.pageLineCount+1;
			}
			list.dataAreaInvalidate();
		}

		private void button_MoveUp_Click(object sender, System.EventArgs e)
		{
			int			listPos = getSelectedListStartPos();
			if(listPos == 0 || listPos >= grBuf.tag.Count) return;

			groupShowTagMember item = (groupShowTagMember) grBuf.tag[listPos];			
			grBuf.tag.RemoveAt(listPos);
			//m_list.Items.RemoveAt(listPos);
			grBuf.tag.Insert(listPos-1, item);
			//AddInsertViewList(listPos-1, item.tag, item.tagType, false, false);
			setSelectionItemChange(listPos-1);
			list.listItemChanged();
			//ArrangeListItemPosNo(listPos, listPos+1);
			//SetListItemSelection(listPos-1);
		}

		private void button_MoveDown_Click(object sender, System.EventArgs e)
		{
			int			listPos = getSelectedListStartPos();;
			if(listPos < 0 || listPos >= grBuf.tag.Count-1) return;

			groupShowTagMember item = (groupShowTagMember) grBuf.tag[listPos];
			grBuf.tag.RemoveAt(listPos);
			grBuf.tag.Insert(listPos+1, item);
			//AddInsertViewList(listPos+1, item.tag, item.tagType, false, false);
			setSelectionItemChange(listPos+1);
			list.listItemChanged();
			//ArrangeListItemPosNo(listPos, listPos+1);
			//SetListItemSelection(listPos+1);		
		}
		

		int portElementRegisterAI(int port, bool bAdd, int listPos, bool bArrange)
		{
			int							i, pos, count = 0;
			TagAiClass					ai;

			tagList = TagLib.GetTagList(EnumTagType.AI);
			if(tagList == null) return 0;
			for(i = 0; i < tagList.Length; i++)
			{
				if(bAdd) pos = i;					// 추가이면 처음 부터
				else     pos = tagList.Length-i-1;	// 삽입이면 뒤에서 부터
				ai = TagLib.GetStructAI(tagList[pos]);
				if(ai.port == port) 
				{
					if(checkRegisteredTagCount(true) == false) break;
					if(TagListAddInsert(ai.tag, bAdd, listPos) == false) continue;
					count++;
					//if(AddInsertViewList(listPos, ai.tag, EnumTagType.AI, bAdd, true)) count++;
					//else break;
				}
			}
			if(bAdd == false) // Insert 시
			{
				//if(bArrange) ArrangeListItemPosNo(listPos, m_list.Items.Count);
				//SetListItemSelection(listPos+count);
				setSelectionItemChange(listPos+count);
			}
			return count;
		}

		int portElementRegisterAO(int port, bool bAdd, int listPos, bool bArrange)
		{
			int							i, pos, count = 0;
			TagAoClass					ao;

			tagList = TagLib.GetTagList(EnumTagType.AO);
			if(tagList == null) return 0;
			for(i = 0; i < tagList.Length; i++)
			{
				if(bAdd) pos = i;					// 추가이면 처음 부터
				else     pos = tagList.Length-i-1;	// 삽입이면 뒤에서 부터
				ao = TagLib.GetStructAO(tagList[pos]);
				if(ao.port == port) 
				{
					if(checkRegisteredTagCount(true) == false) break;
					if(TagListAddInsert(ao.tag, bAdd, listPos) == false) continue;
					count++;
					//if(AddInsertViewList(listPos, ao.tag, EnumTagType.AO, bAdd, true)) count++;
					//else break;
				}
			}
			if(bAdd == false) // Insert 시 
			{
				//if(bArrange) ArrangeListItemPosNo(listPos, m_list.Items.Count);
				//SetListItemSelection(listPos+count);
				setSelectionItemChange(listPos+count);
			}
			return count;
		}

		int portElementRegisterDI(int port, bool bAdd, int listPos, bool bArrange)
		{
			int							i, pos, count = 0;
			TagDiClass					di;

			tagList = TagLib.GetTagList(EnumTagType.DI);
			if(tagList == null) return 0;
			for(i = 0; i < tagList.Length; i++)
			{
				if(bAdd) pos = i;					// 추가이면 처음 부터
				else     pos = tagList.Length-i-1;	// 삽입이면 뒤에서 부터
				di = TagLib.GetStructDI(tagList[pos]);
				if(di.port == port) 
				{
					if(checkRegisteredTagCount(true) == false) break;
					if(TagListAddInsert(di.tag, bAdd, listPos) == false) continue;
					count++;
					//if(AddInsertViewList(listPos, di.tag, EnumTagType.DI, bAdd, true)) count ++;
					//else break;
				}
			}
			if(bAdd == false) // Insert 시 
			{
				//if(bArrange) ArrangeListItemPosNo(listPos, m_list.Items.Count);
				//SetListItemSelection(listPos+count);
				setSelectionItemChange(listPos+count);
			}
			return count;
		}

		int portElementRegisterDO(int port, bool bAdd, int listPos, bool bArrange)
		{
			int							i, pos, count = 0;
			TagDoClass					dout;

			tagList = TagLib.GetTagList(EnumTagType.DO);
			if(tagList == null) return 0;
			for(i = 0; i < tagList.Length; i++)
			{
				if(bAdd) pos = i;					// 추가이면 처음 부터
				else     pos = tagList.Length-i-1;	// 삽입이면 뒤에서 부터
				dout = TagLib.GetStructDO(tagList[pos]);
				if(dout.port == port) 
				{
					if(checkRegisteredTagCount(true) == false) break;
					if(TagListAddInsert(dout.tag, bAdd, listPos) == false) continue;
					count++;
					//if(AddInsertViewList(listPos, dout.tag, EnumTagType.DO, bAdd, true)) count++;
					//else break;
				}
			}
			if(bAdd == false) // Insert 시 
			{
				//if(bArrange) ArrangeListItemPosNo(listPos, m_list.Items.Count);
				//SetListItemSelection(listPos+count);
				setSelectionItemChange(listPos+count);
			}
			return count;
		}

		int portElementRegisterST(int port, bool bAdd, int listPos, bool bArrange)
		{
			int							i, pos, count = 0;
			TagStClass					st;

			tagList = TagLib.GetTagList(EnumTagType.ST);
			if(tagList == null) return 0;
			for(i = 0; i < tagList.Length; i++)
			{
				if(bAdd) pos = i;					// 추가이면 처음 부터
				else     pos = tagList.Length-i-1;	// 삽입이면 뒤에서 부터
				st = TagLib.GetStructST(tagList[pos]);
				if(st.port == port) 
				{
					if(checkRegisteredTagCount(true) == false) break;
					if(TagListAddInsert(st.tag, bAdd, listPos) == false) continue;
					count++;
					//if(AddInsertViewList(listPos, st.tag, EnumTagType.ST, bAdd, true)) count ++;
					//else break;
				}
			}
			if(bAdd == false) // Insert 시 
			{
				//if(bArrange) ArrangeListItemPosNo(listPos, m_list.Items.Count);
				//SetListItemSelection(listPos+count);
				setSelectionItemChange(listPos+count);
			}
			return count;
		}

		void portElementRegister(int port, int tagType, bool bAdd, int listPos)
		{
			switch(tagType) 
			{
				case 0 : portElementRegisterAI(port, bAdd, listPos, true); break;	// AI
				case 1 : portElementRegisterAO(port, bAdd, listPos, true); break;	// AO
				case 2 : portElementRegisterDI(port, bAdd, listPos, true); break;	// DI
				case 3 : portElementRegisterDO(port, bAdd, listPos, true); break;	// DO
				case 4 : portElementRegisterST(port, bAdd, listPos, true); break;	// ST
				case 5 :															// ALL
					int			pos = listPos;
					listPos += portElementRegisterAI(port, bAdd, listPos, false);
					listPos += portElementRegisterAO(port, bAdd, listPos, false);
					listPos += portElementRegisterDI(port, bAdd, listPos, false);
					listPos += portElementRegisterDO(port, bAdd, listPos, false);
					listPos += portElementRegisterST(port, bAdd, listPos, false); 
					//ArrangeListItemPosNo(pos, m_list.Items.Count); 
					break;
				default: break;
			}
		}

		bool checkElementStringEqual(string name)
		{
			string		buf;
			int			i;
			bool		bWild = false;

			buf = this.textBox_Condition.Text.Trim();
			
			if(buf == "*") return true;		// * 문자이면 모든 string
			if(buf == "*.") return true;	// * 문자이면 모든 string
			if(buf == "*.*") return true;	// * 문자이면 모든 string

			for(i = buf.Length-1; i >= 0; i--) 
			{
				if(buf[i] == '*') 
				{
					buf = buf.Substring(0, i);
					bWild = true;
					break;
				}
			}
			if(buf.Length <= 0) return false;

			if(bWild) 
			{				
				if(name.Length < buf.Length) return false;
				if(buf == name.Substring(0, buf.Length)) return true;// 마지막에 * 존재하면
			}
			else 
			{
				if(buf == name) return true;						// 실제 태그, 설명
			}
			return false;
		}

		int stringElementRegisterTP(bool bAdd, int listPos, bool bArrange, EnumTagType tagType)
		{
			int							i, pos, count = 0;
			//TagAiClass					ai;
			TagPublicClass				tp;

			tagList = TagLib.GetTagList(tagType);
			if(tagList == null) return 0;
			for(i = 0; i < tagList.Length; i++)
			{
				if(bAdd) pos = i;					// 추가이면 처음 부터
				else     pos = tagList.Length-i-1;	// 삽입이면 뒤에서 부터
				tp = TagLib.GetStructPublic(tagList[pos]);
				switch(comboBox_Element.SelectedIndex) 
				{
					case 1 : 
						if(checkElementStringEqual(tp.tag)) 
						{
							if(checkRegisteredTagCount(true) == false) goto end_loop;
							if(TagListAddInsert(tp.tag, bAdd, listPos) == false) continue;
							count++;
							//if(AddInsertViewList(listPos, ai.tag, EnumTagType.AI, bAdd, true)) count++;
							//else goto end_loop;
						}
						break;
					case 2 :
						if(checkElementStringEqual(tp.description)) 
						{
							if(checkRegisteredTagCount(true) == false) goto end_loop;
							if(TagListAddInsert(tp.tag, bAdd, listPos) == false) continue;
							count++;
							//if(AddInsertViewList(listPos, ai.tag, EnumTagType.AI, bAdd, true)) count++;
							//else goto end_loop;
						}
						break;
				}				
			}
	
			end_loop:
			if(bAdd == false) 
			{
				//if(bArrange) ArrangeListItemPosNo(listPos, m_list.Items.Count);
				//SetListItemSelection(listPos+count);
				setSelectionItemChange(listPos+count);
			}
			return count;
		}		

		void stringElementRegister(int tagType, bool bAdd, int listPos)
		{
			switch(tagType) 
			{
				case 0 : stringElementRegisterTP(bAdd, listPos, true, EnumTagType.AI); break;	// AI
				case 1 : stringElementRegisterTP(bAdd, listPos, true, EnumTagType.AO); break;	// AO
				case 2 : stringElementRegisterTP(bAdd, listPos, true, EnumTagType.DI); break;	// DI
				case 3 : stringElementRegisterTP(bAdd, listPos, true, EnumTagType.DO); break;	// DO
				case 4 : stringElementRegisterTP(bAdd, listPos, true, EnumTagType.ST); break;	// ST
				case 5 :														// ALL
					int			pos = listPos;
					listPos += stringElementRegisterTP(bAdd, listPos, false, EnumTagType.AI);
					listPos += stringElementRegisterTP(bAdd, listPos, false, EnumTagType.AO);
					listPos += stringElementRegisterTP(bAdd, listPos, false, EnumTagType.DI);
					listPos += stringElementRegisterTP(bAdd, listPos, false, EnumTagType.DO);
					listPos += stringElementRegisterTP(bAdd, listPos, false, EnumTagType.ST); 
					//ArrangeListItemPosNo(pos, m_list.Items.Count); 
					break;
				default: break;
			}
		}

		void conditionGroupTagAddInsert(bool bAdd, int listPos)
		{
			int port = 0, tagType;

			if(textBox_Condition.Text.Length <= 0) return;
			try 
			{
				tagType = comboBox_Element.SelectedIndex;	// 선택된 요소가 있는지 확인
				tagType = comboBox_TagType.SelectedIndex;
			}
			catch 
			{
				return;
			}

			switch(comboBox_Element.SelectedIndex) 
			{
				case 0 :			// Port
					try 
					{
						port = ConvertTool.ToInt32(textBox_Condition.Text);
					}
					catch {}
					portElementRegister(port, tagType, bAdd, listPos);
					break;
				default :
					stringElementRegister(tagType, bAdd, listPos);
					break;
			}
			fillListDataAll();
		}

		private void button_Insert_Click(object sender, System.EventArgs e)
		{
			conditionGroupTagAddInsert(false, getSelectedListStartPos());
		}

		private void button_Add_Click(object sender, System.EventArgs e)
		{
			conditionGroupTagAddInsert(true, getSelectedListStartPos());
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBox_GroupName, TextBoxLimit.MAX_GroupName)) return;
            if (TextBoxTool.CheckTextBoxLimitOver(textBox_Condition, TextBoxLimit.MAX_GroupSearchFilter)) return;

            if (grBuf.tagCount < 1)
            {
                if (Tools.IsLangKorean())
                {
                    MessageBox.Show("태그를 한개 이상 등록해야 합니다.");
                }
                else
                {
                    MessageBox.Show("You must add at least one tag.");
                }

                return;
            }

            string name = textBox_GroupName.Text.Trim();

			if(name.Length <= 0) 
			{
				if(Tools.IsLangKorean()) 
				{
					MessageBox.Show("그룹 이름을 입력해야 합니다.");
				}
				else if(Tools.IsLangChinese()) 
				{
					MessageBox.Show("请输入组名。");
				}
				else 
				{
					MessageBox.Show("Please Input Registered Group Name.");
				}
				return;
			}
			grBuf.name = name;
			this.DialogResult = DialogResult.OK;
			Close();
		}

		void oneLineDraw(Graphics g, groupShowTagMember item, int i, int x, int y, int xGap)
		{
			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];			
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, i.ToString(), Color.Black, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, item.tag, Color.Black, list.backColor, list.font, format);

			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getTagTypeText(item.tagType), Color.Black, list.backColor, list.font, head.format);
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);

			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;			
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
			groupShowTagMember		item;

			endPos = list.pageLineCount+list.startPos;//+1;		// 기본 리스트 이므로 딱 맞게 그린다.
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;
				if(pos >= grBuf.tagCount) break;
				item = (groupShowTagMember)grBuf.tag[pos];

				x = list.startX;
				oneLineDraw(g, item, pos+1, x, y, xGap);
			}
		}

		void onPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}

		private void ViewRegisteredGroupDataInputDlg_Closed(object sender, System.EventArgs e)
		{
			list.paintMessage -= new ControlListView.OnEventPaintMessage(onPaintMessage);
		}
	}
}
