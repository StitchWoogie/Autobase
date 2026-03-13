using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using AutoLib;
using NetTools;
using System.Data;
using AutoLibLocal;
using System.Drawing.Drawing2D; 

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogOutputMain.
	/// </summary>
	public class ViewAnalogOutputMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ContextMenu contextMenuAo;
		private System.Windows.Forms.MenuItem menuItem_aoDetail;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem_aoValueSetting;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
        private System.Windows.Forms.MenuItem menuItem_TagSearch;
		private System.Windows.Forms.MenuItem menuItem3;

		TagListStruct[]		tagList;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Timer timer1;


		//int[]					headWidth = new int[5];
		ControlListView			list = new ControlListView();
        private MenuItem menuItem1;
        private MenuItem menuItem2;

        TagListViewColumn tlvc = new TagListViewColumn();

		public ViewAnalogOutputMain(string tag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.panel2.Controls.Add(list);
			basicElementSetting();

			getTagListAndLength();
			if(tag == null) list.currPos = 0;
			else list.currPos = TagLib.GetTagPosOnlyList(tagList, tag);
			if(list.currPos <= 0 || list.currPos >= list.listHap) list.currPos = 0;
			
            /*
			headWidth[0] = 80;
			headWidth[1] = 180;
			headWidth[2] = 180;
			headWidth[3] = 130;
			headWidth[4] = 80;*/
		}

		void getTagListAndLength()
		{
			tagList = TagLib.GetTagList(EnumTagType.AO);
			if(tagList == null) list.listHap = 0;
			else list.listHap = tagList.Length;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogOutputMain));
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
            this.contextMenuAo = new System.Windows.Forms.ContextMenu();
            this.menuItem_aoDetail = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem_aoValueSetting = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_TagSearch = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4,
            this.toolBarButton5});
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            resources.ApplyResources(this.toolBarButton1, "toolBarButton1");
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            resources.ApplyResources(this.toolBarButton2, "toolBarButton2");
            // 
            // toolBarButton3
            // 
            this.toolBarButton3.Name = "toolBarButton3";
            resources.ApplyResources(this.toolBarButton3, "toolBarButton3");
            // 
            // toolBarButton4
            // 
            this.toolBarButton4.Name = "toolBarButton4";
            resources.ApplyResources(this.toolBarButton4, "toolBarButton4");
            // 
            // toolBarButton5
            // 
            this.toolBarButton5.Name = "toolBarButton5";
            resources.ApplyResources(this.toolBarButton5, "toolBarButton5");
            // 
            // contextMenuAo
            // 
            this.contextMenuAo.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_aoDetail,
            this.menuItem4,
            this.menuItem_aoValueSetting,
            this.menuItem_TagProperityModify,
            this.menuItem_TagSearch,
            this.menuItem1,
            this.menuItem2,
            this.menuItem3});
            // 
            // menuItem_aoDetail
            // 
            this.menuItem_aoDetail.Index = 0;
            resources.ApplyResources(this.menuItem_aoDetail, "menuItem_aoDetail");
            this.menuItem_aoDetail.Click += new System.EventHandler(this.menuItem_aoDetail_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            resources.ApplyResources(this.menuItem4, "menuItem4");
            // 
            // menuItem_aoValueSetting
            // 
            this.menuItem_aoValueSetting.Index = 2;
            resources.ApplyResources(this.menuItem_aoValueSetting, "menuItem_aoValueSetting");
            this.menuItem_aoValueSetting.Click += new System.EventHandler(this.menuItem_aoValueSetting_Click);
            // 
            // menuItem_TagProperityModify
            // 
            this.menuItem_TagProperityModify.Index = 3;
            resources.ApplyResources(this.menuItem_TagProperityModify, "menuItem_TagProperityModify");
            this.menuItem_TagProperityModify.Click += new System.EventHandler(this.menuItem_TagProperityModify_Click);
            // 
            // menuItem_TagSearch
            // 
            this.menuItem_TagSearch.Index = 4;
            resources.ApplyResources(this.menuItem_TagSearch, "menuItem_TagSearch");
            this.menuItem_TagSearch.Click += new System.EventHandler(this.menuItem_TagSearch_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 5;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 6;
            resources.ApplyResources(this.menuItem2, "menuItem2");
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 7;
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolBar1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // ViewAnalogOutputMain
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "ViewAnalogOutputMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ViewAnalogOutputMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewAnalogOutputMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewAnalogOutputMain_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewAnalogOutputMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();

		private void ViewAnalogOutputMain_Load(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListTagChanged += new SharedViewMain.OnEventTagChanged(OnEventTagChanged);
			SharedViewMain.EventListMainFontChanged += new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged += new SharedViewMain.DelegatePublic(OnColorChanged);
			SharedViewMain.EventListTagPropertyChanged += new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
			SharedViewMain.EventListTagListChanged += new SharedViewMain.DelegatePublic(OnTagListChanged);
			SharedViewMain.EventListUserChanged += new SharedViewMain.DelegatePublic(OnUserChanged);

			list.doubleClick += new ControlListView.OnEventDoubleClick(onListMouse_DoubleClicked);
			list.selectedIndexChanged += new ControlListView.OnEventSelectedIndexChanged(onSelectedIndexChanged);
			list.paintMessage += new ControlListView.OnEventPaintMessage(onPaintMessage);
			            			
			//TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "AnalogOutputMain");
			
			list.bOwnerDraw = true;
			listHeaderFill();
			list.Show();
			fillListDataAll();
			this.panel1.Height = this.toolBar1.Height;			// 초기크기를 설정
			ringForm.push(this);

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.toolBarButton4.Visible = false;
                this.toolBarButton5.Visible = false;
            }
		}

		void listHeaderFill()
		{
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.No, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Tag, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Description, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Value, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Unit, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Port, false);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Address, false);

            ControlListViewHeader head;

            BasicScreen.kdymain.TagListViewColumn.TagListColumnItem tlci;

            for (int i = 0; i < tlvc.arrayColumnList.Count; i++)
            {
                tlci = tlvc.arrayColumnList[i];

                head = new ControlListViewHeader();
                head.width = tlci.nWidth;
                head.text = tlci.sColumnName;
                head.format = new StringFormat();
                head.format.Alignment = StringAlignment.Center;
                head.bVisible = tlci.visible;
                head.nID = (int)tlci.eTagListColumn;

                list.header.Add(head);
            }

            list.ConfigLoad(TotalConfig.AutoBaseIniGetConfigDirectory(), "BasicScreen", "AnalogOutputMain");

            /*
			ControlListViewHeader	head;
			string[]				text = new string[5];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "순서";
				text[1] = "태그이름";
				text[2] = "설명";
				text[3] = "현재 값";
				text[4] = "단위";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "No";
				text[1] = "タグ名";
				text[2] = "説明";
				text[3] = "現在値"; 
				text[4] = "単位"; 
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "顺序";
				text[1] = "标记名";
				text[2] = "标记描述";
				text[3] = "现在值"; 
				text[4] = "单位"; 
			}
            else if (Tools.IsLangVietnamese())
            {
                text[0] = "Số";
                text[1] = "Tên Tag";
                text[2] = "Mô tả";
                text[3] = "Giá trị hiện tại";
                text[4] = "Đơn vị";
            }
			else 
			{
				text[0] = "No";
				text[1] = "Tag Name";
				text[2] = "Description";
				text[3] = "Current Value";
				text[4] = "Unit";				
			}			

			for(int i = 0; i < 5; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();
				//if(i < 3) head.format.Alignment = StringAlignment.Near;
				head.format.Alignment = StringAlignment.Center;
				list.header.Add(head);
			}*/
		}

		void basicElementSetting()
		{
			list.font = ConfigViewMain.fontMain;
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {

            }
            else
            {
                list.contextMenu = this.contextMenuAo;
            }
		}

		void fillListDataAll()
		{
			if(tagList == null) list.listHap = 0;
			else list.listHap = tagList.Length;
			list.listItemChanged();			
			setDetailButtonEnableDisable();
		}

		void setActTagMenuEnableDisable(bool flag)
		{
			toolBarButton2.Enabled = flag;
			toolBarButton3.Enabled = flag;		// 출력
			menuItem_aoDetail.Enabled = flag;
			menuItem_aoValueSetting.Enabled = flag;
		}

		void setDefaultMenuEnableDisable(bool flag)
		{
			menuItem_TagProperityModify.Enabled = flag;
			menuItem_TagSearch.Enabled = flag;			
			toolBarButton4.Enabled = flag;		// Act
			toolBarButton5.Enabled = flag;		// tag 속성수정
		}

		void setDetailButtonEnableDisable()		// 상세 버턴을 Enable/Disable
		{
			TagAoClass ao = getSelectedAoTagClass();
			if(ao == null) 
			{
				setActTagMenuEnableDisable(false);
				setDefaultMenuEnableDisable(false);
				return;
			}
			setDefaultMenuEnableDisable(true);

			setActTagMenuEnableDisable((ao.act == 1) ? true : false);
            toolBarButton4.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE);//act?
		}

		int getSelectedTagPos()
		{
			return list.currPos;
			
		}

		public void callDetailWindow()
		{
			TagAoClass ao = getSelectedAoTagClass();
			if(ao == null) return;
			BasicScreenTool.callDetailWindow((TagPublicClass)ao);
		}

		void setTagActiveInActive()		// Tag Active/Inactive
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE)) return;

			TagAoClass ao = getSelectedAoTagClass();			
			if(ao == null) return;

			ao.act = ( ao.act == 1 ) ? (sbyte)0 : (sbyte)1;
			list.oneLineInvalidate(list.currPos);
			setDetailButtonEnableDisable();			// active가 바뀌었으므로 버턴을 활성/비활성
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해

			LibComNetServer.SendCommandToNetWorkTagMemberChanged(ao.tag, EnumTagMember.TAG_MEMBER_act, ao.act);
		}

		TagAoClass getSelectedAoTagClass()
		{
			if(list.currPos < 0 || list.currPos >= list.listHap || tagList == null) return null;
			if(tagList.Length <= list.currPos) return null;
			return TagLib.GetStructAO(tagList[list.currPos]);

			
		}

		void callValueChangeWindows()
		{
			TagAoClass ao = getSelectedAoTagClass();
			if(ao == null) return;
			if(BasicScreenTool.callValueChangeWindows(this, (TagPublicClass)ao))			
				list.oneLineInvalidate(list.currPos);
				
		}

		
		void CallSelectTagPropertyWindows()
		{
			TagAoClass ao = getSelectedAoTagClass();
			if(ao == null) return;
			if(DialogTag.TagEditor.Editor.ByViewMain(ao)) 
			{			
				list.oneLineInvalidate(list.currPos);
				
				setDetailButtonEnableDisable();				// act 가 변경될 수도 있으므로
			}
		}

		void TagSearchNameOrPos()
		{
			if(list.listHap <= 1) return;
			

			int			pos = getSelectedTagPos();
			ViewTagNamePosSearchDlg dialog = new ViewTagNamePosSearchDlg(pos, list.listHap);
			dialog.tagList = tagList;
			dialog.bTagList = true;
            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(this);
			if(dialog.bTagChanged && pos != dialog.currPos)		// 값이 변경되었다.
			{
				
				list.listSelectedPosChange(dialog.currPos);
				setDetailButtonEnableDisable();
			}
		}

		private void menuItem_aoDetail_Click(object sender, System.EventArgs e)
		{
			callDetailWindow();
		}

		private void menuItem_aoValueSetting_Click(object sender, System.EventArgs e)
		{
			callValueChangeWindows();
		}

		private void menuItem_TagProperityModify_Click(object sender, System.EventArgs e)
		{
			CallSelectTagPropertyWindows();
		}

		private void menuItem_TagSearch_Click(object sender, System.EventArgs e)
		{
			TagSearchNameOrPos();
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) 
			{
				this.Close();
				return;
			}
			if(e.Button == toolBarButton2) 
			{
				callDetailWindow();
				return;
			}
			if(e.Button == toolBarButton3) 
			{
				callValueChangeWindows();
				return;
			}
			if(e.Button == toolBarButton4)
			{
				setTagActiveInActive();
				return;
			}
			if(e.Button == toolBarButton5)
			{
				CallSelectTagPropertyWindows();
				return;
			}
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			callDetailWindow();
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			setDetailButtonEnableDisable();
		}

		
		private void ViewAnalogOutputMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
			
		}

        private void OnEventTagChanged(TagPublicClass tagevent)
		{
			if(tagevent.enumTagType != EnumTagType.AO) return;

			TagAoClass	ao;

            for (int i = list.startPos; i < list.listHap && i <= list.startPos + list.pageLineCount; i++) 
			
			{
				ao = TagLib.GetStructAO(tagList[i]);
				if(ao == null || ao.tag != tagevent.tag) continue;
				list.oneLineInvalidate(i);
			
				return;
			}
		}

		private void OnMainFontChanged()
		{
			list.font = ConfigViewMain.fontMain;
			list.fontChanged();
			
		}

		void OnColorChanged()
		{
			this.BackColor = SharedData.colorTotal.BACK;				// 기본화면의 배경색상, list visable =
			list.backColor = SharedData.colorTotal.BACK;
			list.textColor = SharedData.colorTotal.INACTIVE;		// 유효하지 않은태그일 경우의 색상으로 사용
			list.dataAreaInvalidate();

			
		}
		
		
		void OnTagPropertyChanged(TagPublicClass tp)
		{
			if(tp.enumTagType != EnumTagType.AO) return;
			TagAoClass	ao;
			for(int i = 0; i < list.listHap; i++) 
			{
				ao = TagLib.GetStructAO(tagList[i]);
				if(ao == null || ao != (TagAoClass)tp) continue;
				list.oneLineInvalidate(i);
				
				return;
			}
		}

		void OnTagListChanged()
		{
			getTagListAndLength();
			fillListDataAll();
		}

		void OnUserChanged()
		{
			setDetailButtonEnableDisable();
		}

		void onListMouse_DoubleClicked()
		{
			callDetailWindow();
		}

		void onSelectedIndexChanged()
		{
			setDetailButtonEnableDisable();
		}
		
		void oneLineDraw(Graphics g, TagAoClass ao, int i, int x, int y, int xGap)
		{
            Color color_etc = (ao.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;

            ControlListViewHeader head;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            StringFormat format_far = new StringFormat();
            format_far.Alignment = StringAlignment.Far;

            for (int c = 0; c < list.header.Count; c++)
            {
                head = (ControlListViewHeader)list.header[c];

                if (!head.bVisible) continue;

                if (head.nID == (int)TagListViewColumn.EnumTagListColumn.No)
                {
                    DrawClass.WinDrawText(g, x + xGap, y, head.width - xGap * 2, (int)list.fontY, i.ToString(), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Tag)
                {
                    Color color = (ao.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.tag, color, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Description)
                {
                    Color color = (ao.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.description, color, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Value)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.curr.ToString(), color_etc, list.backColor, list.font, format_far);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Unit)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.unit, color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Port)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.port.ToString(), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Address)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.address.ToString("X"), color_etc, list.backColor, list.font, head.format);
                }

                x += head.width;
            }

            /*
			Color					color;
			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			
			color = (ao.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, i.ToString(), color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			color = (ao.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, ao.tag, color, list.backColor, list.font, format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			color = (ao.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, ao.description, color, list.backColor, list.font, format);
			
			format.Alignment = StringAlignment.Far;
			x += head.width;
			head = (ControlListViewHeader)list.header[3];
			color = (ao.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, ao.curr.ToString(), color, list.backColor, list.font, format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[4];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, ao.unit, color, list.backColor, list.font, head.format);
             */
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);
			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
			TagAoClass				ao;

			endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;

				ao = TagLib.GetStructAO(tagList[pos]);
				if(ao == null) continue;
				x = list.startX;
				oneLineDraw(g, ao, pos+1, x, y, xGap);
			}
		}

		void onPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}

		bool bTimerTick = false;

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(list.listHap <= 0 || tagList == null) return;

			bTimerTick = !bTimerTick;
			if(bTimerTick == false) return;				// timer를 유연하게 하기 위해

			TagAoClass				ao;
			for(int i = list.startPos; i < list.listHap; i++) 
			{
				if(i > list.startPos + list.pageLineCount + 1) return;
				ao = TagLib.GetStructAO(tagList[i]);
				if(ao == null || ao.act == 0) continue;
				ao.NeedDataCurr = true;
			}


		}


		private void ViewAnalogOutputMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
				case Keys.Enter : callDetailWindow(); return;
				case Keys.F3 : callValueChangeWindows();; return;
				case Keys.F9 : setTagActiveInActive(); return;
				case Keys.F11 : CallSelectTagPropertyWindows(); return;
			}

		}		

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewAnalogOutputMain_Closed(object sender, System.EventArgs e)
		{
			SharedViewMain.EventListTagChanged -= new SharedViewMain.OnEventTagChanged(OnEventTagChanged);
			SharedViewMain.EventListMainFontChanged -= new SharedViewMain.OnEventMainFontChanged(OnMainFontChanged);
			SharedViewMain.EventListColorChanged -= new SharedViewMain.DelegatePublic(OnColorChanged);
			SharedViewMain.EventListTagPropertyChanged -= new SharedViewMain.DelegateTagPropertyChanged(OnTagPropertyChanged);
			SharedViewMain.EventListTagListChanged -= new SharedViewMain.DelegatePublic(OnTagListChanged);
			SharedViewMain.EventListUserChanged -= new SharedViewMain.DelegatePublic(OnUserChanged);

			list.doubleClick -= new ControlListView.OnEventDoubleClick(onListMouse_DoubleClicked);
			list.selectedIndexChanged -= new ControlListView.OnEventSelectedIndexChanged(onSelectedIndexChanged);
			list.paintMessage -= new ControlListView.OnEventPaintMessage(onPaintMessage);

            list.ConfigSave(TotalConfig.AutoBaseIniGetConfigDirectory(), "BasicScreen", "AnalogOutputMain");
            /*
			ControlListViewHeader head;
			bool					bChange = false;
			for(int i = 0; i < list.header.Count; i++) 
			{
				head = (ControlListViewHeader)list.header[i];
				if(headWidth[i] == head.width) continue;
				headWidth[i] = head.width;
				bChange = true;
			}
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "AnalogOutputMain");
             */
			ringForm.pop(this);
		}

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            list.DialogConfigColumn();
        }
		
		
	}
}
