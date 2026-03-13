using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Data;
using System.Drawing.Drawing2D;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewRegisterGroupDetailMain.
	/// </summary>
	public class ViewRegisterGroupDetailMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		public System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.ToolBarButton toolBarButton8;
		private System.Windows.Forms.ToolBarButton toolBarButton9;
		private System.Windows.Forms.ToolBarButton toolBarButton10;		
		private System.Windows.Forms.ContextMenu contextMenuGroupList;
		private System.Windows.Forms.MenuItem menuItem_ViewDetail;
		private System.Windows.Forms.MenuItem menuItem_AiTotalView;
		private System.Windows.Forms.MenuItem menuItem_DiTotalView;
		private System.Windows.Forms.MenuItem menuItem_DoTotalView;
		private System.Windows.Forms.MenuItem menuItem_StTotalView;
		private System.Windows.Forms.MenuItem menuItem_1Trend;
		private System.Windows.Forms.MenuItem menuItem_8Trend;
		private System.Windows.Forms.MenuItem menuItem_24Trend;
		private System.Windows.Forms.MenuItem menuItem_48Trend;
		private System.Windows.Forms.MenuItem menuItem_72Trend;
		private System.Windows.Forms.MenuItem menuItem_30Trend;
		private System.Windows.Forms.MenuItem menuItem_MinData;
		private System.Windows.Forms.MenuItem menuItem_HourData;
		private System.Windows.Forms.MenuItem menuItem_DayData;
		private System.Windows.Forms.MenuItem menuItem_WeekData;
		private System.Windows.Forms.MenuItem menuItem_MonthData;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem_ValueChange;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;
		private System.Windows.Forms.MenuItem menuItem_TagSearch;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem_AoTotalView;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;

		groupShowTagGroupMember	grShow = new groupShowTagGroupMember();
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		//int						TagHap;

		ControlListView list = new ControlListView();
        private MenuItem menuItem5;
		//int[]					headWidth = new int[9];

        TagListViewColumn tlvc = new TagListViewColumn();

		public ViewRegisterGroupDetailMain(groupShowTagGroupMember group)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			
			if(Tools.IsLangKorean()) this.Text = string.Format("{0} 등록된 그룹 상세보기", group.name);
			else if(Tools.IsLangJapanese()) this.Text = string.Format("{0} 登録されたグループの詳細表示", group.name);
			else if(Tools.IsLangChinese()) this.Text = string.Format("{0} 详细地查看已登记的组", group.name);
			else					 this.Text = string.Format("{0} View Registered Group Detail", group.name);

			this.panel2.Controls.Add(list);
			basicElementSetting();

            /*
			headWidth[0] = 80;
			headWidth[1] = 100;
			headWidth[2] = 120;
			headWidth[3] = 130;
			headWidth[4] = 50;
			headWidth[5] = 50;
			headWidth[6] = 50;
			headWidth[7] = 80;
			headWidth[8] = 80;*/

			grShow = group;
			if(grShow == null || grShow.tagCount <= 0) list.listHap = 0;
			else list.listHap = grShow.tag.Count;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewRegisterGroupDetailMain));
            this.contextMenuGroupList = new System.Windows.Forms.ContextMenu();
            this.menuItem_ViewDetail = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem_AiTotalView = new System.Windows.Forms.MenuItem();
            this.menuItem_AoTotalView = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTotalView = new System.Windows.Forms.MenuItem();
            this.menuItem_DoTotalView = new System.Windows.Forms.MenuItem();
            this.menuItem_StTotalView = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem_1Trend = new System.Windows.Forms.MenuItem();
            this.menuItem_8Trend = new System.Windows.Forms.MenuItem();
            this.menuItem_24Trend = new System.Windows.Forms.MenuItem();
            this.menuItem_48Trend = new System.Windows.Forms.MenuItem();
            this.menuItem_72Trend = new System.Windows.Forms.MenuItem();
            this.menuItem_30Trend = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_MinData = new System.Windows.Forms.MenuItem();
            this.menuItem_HourData = new System.Windows.Forms.MenuItem();
            this.menuItem_DayData = new System.Windows.Forms.MenuItem();
            this.menuItem_WeekData = new System.Windows.Forms.MenuItem();
            this.menuItem_MonthData = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_ValueChange = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem_TagSearch = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton7 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton8 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton9 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton10 = new System.Windows.Forms.ToolBarButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuGroupList
            // 
            this.contextMenuGroupList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_ViewDetail,
            this.menuItem6,
            this.menuItem_AiTotalView,
            this.menuItem_AoTotalView,
            this.menuItem_DiTotalView,
            this.menuItem_DoTotalView,
            this.menuItem_StTotalView,
            this.menuItem7,
            this.menuItem_1Trend,
            this.menuItem_8Trend,
            this.menuItem_24Trend,
            this.menuItem_48Trend,
            this.menuItem_72Trend,
            this.menuItem_30Trend,
            this.menuItem1,
            this.menuItem_MinData,
            this.menuItem_HourData,
            this.menuItem_DayData,
            this.menuItem_WeekData,
            this.menuItem_MonthData,
            this.menuItem2,
            this.menuItem_ValueChange,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem_TagSearch,
            this.menuItem5,
            this.menuItem3,
            this.menuItem4});
            resources.ApplyResources(this.contextMenuGroupList, "contextMenuGroupList");
            // 
            // menuItem_ViewDetail
            // 
            resources.ApplyResources(this.menuItem_ViewDetail, "menuItem_ViewDetail");
            this.menuItem_ViewDetail.Index = 0;
            this.menuItem_ViewDetail.Click += new System.EventHandler(this.menuItem_ViewDetail_Click);
            // 
            // menuItem6
            // 
            resources.ApplyResources(this.menuItem6, "menuItem6");
            this.menuItem6.Index = 1;
            // 
            // menuItem_AiTotalView
            // 
            resources.ApplyResources(this.menuItem_AiTotalView, "menuItem_AiTotalView");
            this.menuItem_AiTotalView.Index = 2;
            this.menuItem_AiTotalView.Click += new System.EventHandler(this.menuItem_AiTotalView_Click);
            // 
            // menuItem_AoTotalView
            // 
            resources.ApplyResources(this.menuItem_AoTotalView, "menuItem_AoTotalView");
            this.menuItem_AoTotalView.Index = 3;
            this.menuItem_AoTotalView.Click += new System.EventHandler(this.menuItem_AoTotalView_Click);
            // 
            // menuItem_DiTotalView
            // 
            resources.ApplyResources(this.menuItem_DiTotalView, "menuItem_DiTotalView");
            this.menuItem_DiTotalView.Index = 4;
            this.menuItem_DiTotalView.Click += new System.EventHandler(this.menuItem_DiTotalView_Click);
            // 
            // menuItem_DoTotalView
            // 
            resources.ApplyResources(this.menuItem_DoTotalView, "menuItem_DoTotalView");
            this.menuItem_DoTotalView.Index = 5;
            this.menuItem_DoTotalView.Click += new System.EventHandler(this.menuItem_DoTotalView_Click);
            // 
            // menuItem_StTotalView
            // 
            resources.ApplyResources(this.menuItem_StTotalView, "menuItem_StTotalView");
            this.menuItem_StTotalView.Index = 6;
            this.menuItem_StTotalView.Click += new System.EventHandler(this.menuItem_StTotalView_Click);
            // 
            // menuItem7
            // 
            resources.ApplyResources(this.menuItem7, "menuItem7");
            this.menuItem7.Index = 7;
            // 
            // menuItem_1Trend
            // 
            resources.ApplyResources(this.menuItem_1Trend, "menuItem_1Trend");
            this.menuItem_1Trend.Index = 8;
            this.menuItem_1Trend.Click += new System.EventHandler(this.menuItem_1Trend_Click);
            // 
            // menuItem_8Trend
            // 
            resources.ApplyResources(this.menuItem_8Trend, "menuItem_8Trend");
            this.menuItem_8Trend.Index = 9;
            this.menuItem_8Trend.Click += new System.EventHandler(this.menuItem_8Trend_Click);
            // 
            // menuItem_24Trend
            // 
            resources.ApplyResources(this.menuItem_24Trend, "menuItem_24Trend");
            this.menuItem_24Trend.Index = 10;
            this.menuItem_24Trend.Click += new System.EventHandler(this.menuItem_24Trend_Click);
            // 
            // menuItem_48Trend
            // 
            resources.ApplyResources(this.menuItem_48Trend, "menuItem_48Trend");
            this.menuItem_48Trend.Index = 11;
            this.menuItem_48Trend.Click += new System.EventHandler(this.menuItem_48Trend_Click);
            // 
            // menuItem_72Trend
            // 
            resources.ApplyResources(this.menuItem_72Trend, "menuItem_72Trend");
            this.menuItem_72Trend.Index = 12;
            this.menuItem_72Trend.Click += new System.EventHandler(this.menuItem_72Trend_Click);
            // 
            // menuItem_30Trend
            // 
            resources.ApplyResources(this.menuItem_30Trend, "menuItem_30Trend");
            this.menuItem_30Trend.Index = 13;
            this.menuItem_30Trend.Click += new System.EventHandler(this.menuItem_30Trend_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 14;
            // 
            // menuItem_MinData
            // 
            resources.ApplyResources(this.menuItem_MinData, "menuItem_MinData");
            this.menuItem_MinData.Index = 15;
            this.menuItem_MinData.Click += new System.EventHandler(this.menuItem_MinData_Click);
            // 
            // menuItem_HourData
            // 
            resources.ApplyResources(this.menuItem_HourData, "menuItem_HourData");
            this.menuItem_HourData.Index = 16;
            this.menuItem_HourData.Click += new System.EventHandler(this.menuItem_HourData_Click);
            // 
            // menuItem_DayData
            // 
            resources.ApplyResources(this.menuItem_DayData, "menuItem_DayData");
            this.menuItem_DayData.Index = 17;
            this.menuItem_DayData.Click += new System.EventHandler(this.menuItem_DayData_Click);
            // 
            // menuItem_WeekData
            // 
            resources.ApplyResources(this.menuItem_WeekData, "menuItem_WeekData");
            this.menuItem_WeekData.Index = 18;
            this.menuItem_WeekData.Click += new System.EventHandler(this.menuItem_WeekData_Click);
            // 
            // menuItem_MonthData
            // 
            resources.ApplyResources(this.menuItem_MonthData, "menuItem_MonthData");
            this.menuItem_MonthData.Index = 19;
            this.menuItem_MonthData.Click += new System.EventHandler(this.menuItem_MonthData_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 20;
            // 
            // menuItem_ValueChange
            // 
            resources.ApplyResources(this.menuItem_ValueChange, "menuItem_ValueChange");
            this.menuItem_ValueChange.Index = 21;
            this.menuItem_ValueChange.Click += new System.EventHandler(this.menuItem_ValueChange_Click);
            // 
            // menuItem_TagProperityModify
            // 
            resources.ApplyResources(this.menuItem_TagProperityModify, "menuItem_TagProperityModify");
            this.menuItem_TagProperityModify.Index = 22;
            this.menuItem_TagProperityModify.Click += new System.EventHandler(this.menuItem_TagProperityModify_Click);
            // 
            // menuItem_HandInput
            // 
            resources.ApplyResources(this.menuItem_HandInput, "menuItem_HandInput");
            this.menuItem_HandInput.Index = 23;
            this.menuItem_HandInput.Click += new System.EventHandler(this.menuItem_HandInput_Click);
            // 
            // menuItem_TagSearch
            // 
            resources.ApplyResources(this.menuItem_TagSearch, "menuItem_TagSearch");
            this.menuItem_TagSearch.Index = 24;
            this.menuItem_TagSearch.Click += new System.EventHandler(this.menuItem_TagSearch_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 25;
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 26;
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 27;
            this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
            // 
            // toolBar1
            // 
            this.toolBar1.AccessibleDescription = null;
            this.toolBar1.AccessibleName = null;
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.BackgroundImage = null;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4,
            this.toolBarButton5,
            this.toolBarButton6,
            this.toolBarButton7,
            this.toolBarButton8,
            this.toolBarButton9,
            this.toolBarButton10});
            this.toolBar1.Font = null;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButton1
            // 
            resources.ApplyResources(this.toolBarButton1, "toolBarButton1");
            this.toolBarButton1.Name = "toolBarButton1";
            // 
            // toolBarButton2
            // 
            resources.ApplyResources(this.toolBarButton2, "toolBarButton2");
            this.toolBarButton2.Name = "toolBarButton2";
            // 
            // toolBarButton3
            // 
            resources.ApplyResources(this.toolBarButton3, "toolBarButton3");
            this.toolBarButton3.Name = "toolBarButton3";
            // 
            // toolBarButton4
            // 
            resources.ApplyResources(this.toolBarButton4, "toolBarButton4");
            this.toolBarButton4.Name = "toolBarButton4";
            // 
            // toolBarButton5
            // 
            resources.ApplyResources(this.toolBarButton5, "toolBarButton5");
            this.toolBarButton5.Name = "toolBarButton5";
            // 
            // toolBarButton6
            // 
            resources.ApplyResources(this.toolBarButton6, "toolBarButton6");
            this.toolBarButton6.Name = "toolBarButton6";
            // 
            // toolBarButton7
            // 
            resources.ApplyResources(this.toolBarButton7, "toolBarButton7");
            this.toolBarButton7.Name = "toolBarButton7";
            // 
            // toolBarButton8
            // 
            resources.ApplyResources(this.toolBarButton8, "toolBarButton8");
            this.toolBarButton8.Name = "toolBarButton8";
            // 
            // toolBarButton9
            // 
            resources.ApplyResources(this.toolBarButton9, "toolBarButton9");
            this.toolBarButton9.Name = "toolBarButton9";
            // 
            // toolBarButton10
            // 
            resources.ApplyResources(this.toolBarButton10, "toolBarButton10");
            this.toolBarButton10.Name = "toolBarButton10";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.toolBar1);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // ViewRegisterGroupDetailMain
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "ViewRegisterGroupDetailMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ViewRegisterGroupDetailMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewRegisterGroupDetailMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewRegisterGroupDetailMain_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewRegisterGroupDetailMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();

		private void ViewRegisterGroupDetailMain_Load(object sender, System.EventArgs e)
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

			//TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "RegisteredGroupTag");
			
			list.bOwnerDraw = true;
			listHeaderFill();
			list.Show();
			fillListDataAll();
			this.panel1.Height = this.toolBar1.Height;			// 초기크기를 설정

			ringForm.push(this);

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.toolBarButton5.Visible = false;
                this.toolBarButton6.Visible = false;
                this.toolBarButton7.Visible = false;
                this.toolBarButton8.Visible = false;
                this.toolBarButton9.Visible = false;
                this.toolBarButton10.Visible = false;

            }
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
                list.contextMenu = this.contextMenuGroupList;
            }
		}

		void listHeaderFill()
		{
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.No, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Tag, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Description, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Value, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Unit, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Data, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Alarm, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.SV_Out1, true);
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.HandInput, false);
            }
            else
            {
                tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.HandInput, true);
            }
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

            list.ConfigLoad(TotalConfig.AutoBaseIniGetConfigDirectory(), "BasicScreen", "RegisteredGroupTag");

            /*
			ControlListViewHeader	head;
			string[]				text = new string[9];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "순서";
				text[1] = "태그이름";
				text[2] = "설명";
				text[3] = "현재 값";
				text[4] = "단위";
				text[5] = "자료";
				text[6] = "경보";
				text[7] = "SV/Out1";
				text[8] = "수동기입";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "番号";
				text[1] = "タグ名";
				text[2] = "説明";
				text[3] = "現在値";
				text[4] = "単位";
				text[5] = "データ";
                text[6] = "警報";
				text[7] = "SV/Out1";
				text[8] = "手動記入";
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "顺序";
				text[1] = "标记名";
				text[2] = "标记描述";
				text[3] = "现在值";
				text[4] = "单位";
				text[5] = "资料";
				text[6] = "警报";
				text[7] = "SV/Out1";
				text[8] = "手动输入";
			}
			else 
			{
				text[0] = "No";
				text[1] = "Tag Name";
				text[2] = "Description";
				text[3] = "Current Value";
				text[4] = "Unit";
				text[5] = "Data";
				text[6] = "Alarm";
				text[7] = "SV/Out1";
				text[8] = "Hand Input";
			}

            if (TotalConfig.eOemType == EnumOemType.SBAS)
                text[8] = "";

			for(int i = 0; i < 9; i++) 
			{
				head = new ControlListViewHeader();
				head.width = headWidth[i];
				head.text = text[i];
				head.format = new StringFormat();

				head.format.Alignment = StringAlignment.Center;
				list.header.Add(head);
			}*/
		}

		void fillListDataAll()
		{
			if(grShow == null || grShow.tagCount <= 0) list.listHap = 0;
			else list.listHap = grShow.tag.Count;
			list.listItemChanged();
			setDetailButtonEnableDisable();
		}

		void getCurrUnitDataAlarm(TagPublicClass tp, ref string unit, ref string data, ref string alarm)
		{
			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					TagAiClass ai = (TagAiClass)tp;
					unit = ai.unit;
					if(ai.bFileSave == 1) 
					{
						if(Tools.IsLangKorean()) data = "자료";
						else if(Tools.IsLangJapanese()) data = "データ";
						else if(Tools.IsLangChinese()) data = "资料";
						else data = "Data";
					}
					if(ai.alarm == 1) 
					{
						if(Tools.IsLangKorean()) alarm = "경보";
                        else if (Tools.IsLangJapanese()) alarm = "警報";
						else if(Tools.IsLangChinese()) alarm = "警报";
						else alarm = "Alarm";
					}
					break;
				case EnumTagType.AO :
					TagAoClass ao = (TagAoClass)tp;
					unit = ao.unit;
					break;
				case EnumTagType.DI :
					TagDiClass di = (TagDiClass)tp;
					if(di.bFileSave == 1) 
					{
						if(Tools.IsLangKorean()) data = "자료";
						else if(Tools.IsLangJapanese()) data = "データ";
						else if(Tools.IsLangChinese()) data = "资料";
						else data = "Data";
					}
					if(di.alarm == 1) 
					{
						if(Tools.IsLangKorean()) alarm = "경보";
                        else if (Tools.IsLangJapanese()) alarm = "警報";
						else if(Tools.IsLangChinese()) alarm = "警报";
						else alarm = "Alarm";
					}
					break;
			}
		}

		string getAoSvOut1Tag(TagPublicClass tp)
		{
			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					TagAiClass ai = (TagAiClass)tp;
					if(ai.sSubOutAnalogSP == null) return "";
					return ai.sSubOutAnalogSP;					
				case EnumTagType.DI :
					TagDiClass di = (TagDiClass)tp;
					if(di.sSubOutDigital1 == null) return "";
					return di.sSubOutDigital1;					
			}
			return "";
		}

		string getHandInputUseString(TagPublicClass tp)
		{
			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					TagAiClass ai = (TagAiClass)tp;
					if((ai.wProtectFlags & EnumProtectFlag.SCAN) > 0) 
					{
						if(Tools.IsLangKorean()) return "수동기입";
						else if(Tools.IsLangJapanese()) return "手動記入";
						else if(Tools.IsLangChinese()) return "手动输入";
						else return "Hand Input";
					}
					break;					
				case EnumTagType.DI :
					TagDiClass di = (TagDiClass)tp;
					if((di.wProtectFlags & EnumProtectFlag.SCAN) > 0) 
					{
						if(Tools.IsLangKorean()) return "수동기입";
						else if(Tools.IsLangJapanese()) return "手動記入";
						else if(Tools.IsLangChinese()) return "手动输入";
						else return "Hand Input";
					}
					break;
			}
			return "";
		}

		int getSelectedTagPos()
		{
			return list.currPos;
		}

		TagPublicClass getSelectedTagPublicClass()
		{
			if(list.currPos < 0 || list.currPos >= list.listHap) return null;

			groupShowTagMember	gs;
			gs = (groupShowTagMember)grShow.tag[list.currPos];
			if(gs == null) return null;
			return TagLib.GetStructPublic(gs.tag, ref gs.tagPos);
		}

		public void callDetailWindow()
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) return;
			if(tp.enumTagType == EnumTagType.ST || tp.enumTagType == EnumTagType.GR) CallSelectTagPropertyWindows();
			else BasicScreenTool.callDetailWindow(tp);
		}

		void callValueChangeWindows()
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) return;
			if(BasicScreenTool.callValueChangeWindows(this, tp))
				list.oneLineInvalidate(list.currPos);
		}

		void CallSelectTagPropertyWindows()
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) return;
			if(DialogTag.TagEditor.Editor.ByViewMain(tp)) 
			{
				list.oneLineInvalidate(list.currPos);

				setDetailButtonEnableDisable();				// act 가 변경될 수도 있으므로
			}
		}

		void CallTagAlarmLevelChangeWindows()
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null || tp.enumTagType != EnumTagType.AI) return;

			TagAiClass ai = (TagAiClass)tp;
			ViewAnalogInputAlarmLevelDlg dialog = new ViewAnalogInputAlarmLevelDlg(ai);
			dialog.ShowDialog(this);
		}
		
		void setTrendDataButtonMenuEnableDisable(bool flag)
		{
			toolBarButton4.Enabled = flag;
			toolBarButton5.Enabled = flag;
			toolBarButton6.Enabled = flag;
			toolBarButton7.Enabled = flag;
			this.menuItem_1Trend.Enabled = flag;
			this.menuItem_8Trend.Enabled = flag;
			this.menuItem_24Trend.Enabled = flag;
			this.menuItem_48Trend.Enabled = flag;
			this.menuItem_72Trend.Enabled = flag;
			this.menuItem_30Trend.Enabled = flag;
			this.menuItem_MinData.Enabled = flag;
			this.menuItem_HourData.Enabled = flag;
			this.menuItem_DayData.Enabled = flag;
			this.menuItem_WeekData.Enabled = flag;
			this.menuItem_MonthData.Enabled = flag;
			this.menuItem_HandInput.Enabled = flag;
		}

		void setDefaultMenuEnableDisable(bool flag)
		{
			menuItem_TagProperityModify.Enabled = flag;
			menuItem_TagSearch.Enabled = flag;
			toolBarButton8.Enabled = flag;		// Act
			toolBarButton9.Enabled = flag;		// tag 속성수정
		}

		void setDetailButtonEnableDisable()		// 상세 버턴을 Enable/Disable
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) 
			{
				setTrendDataButtonMenuEnableDisable(false);
				toolBarButton2.Enabled = false;
				toolBarButton3.Enabled = false;
				toolBarButton10.Enabled = false;
				menuItem_ViewDetail.Enabled = false;				
				menuItem_ValueChange.Enabled = false;				
				setDefaultMenuEnableDisable(false);
				return;
			}
			setDefaultMenuEnableDisable(true);
            toolBarButton8.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE);//act
			bool		bFlag = (tp.act == 1) ? true : false;
			switch(tp.enumTagType)
			{
				case EnumTagType.AI :
				case EnumTagType.DI :
					setTrendDataButtonMenuEnableDisable(bFlag);
                    toolBarButton6.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_ALARM_ACTIVE);
                    toolBarButton7.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_DATASAVE);
					break;
				default :
					setTrendDataButtonMenuEnableDisable(false);
					break;
			}
			switch(tp.enumTagType)
			{
				case EnumTagType.AI :
				case EnumTagType.AO :
				case EnumTagType.DI :
				case EnumTagType.DO : 
					this.menuItem_ViewDetail.Enabled = bFlag;
					toolBarButton2.Enabled = bFlag;					
					break;
				case EnumTagType.ST :
				default : 
					this.menuItem_ViewDetail.Enabled = false;
					toolBarButton2.Enabled = false;
					break;
			}
			if(tp.enumTagType != EnumTagType.GR) 
			{
				toolBarButton3.Enabled = bFlag;			// 출력버턴 활성화
				menuItem_ValueChange.Enabled = bFlag;
			}
			else 
			{
				toolBarButton3.Enabled = false;
				menuItem_ValueChange.Enabled = false;
			}
			if(tp.enumTagType == EnumTagType.AI) this.toolBarButton10.Enabled = true;// 경보레벨 버턴 활성화
			else this.toolBarButton10.Enabled = false;
		}


		void setTagActiveInActive()		// Tag Active/Inactive
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE)) return;

			TagPublicClass tp = getSelectedTagPublicClass();			
			if(tp == null) return;

			tp.act = ( tp.act == 1 ) ? (sbyte)0 : (sbyte)1;
			list.oneLineInvalidate(list.currPos);
			setDetailButtonEnableDisable();			// active가 바뀌었으므로 버턴을 활성/비활성
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
		}

		void setTagAlarmProperty()		// Tag Alarm property setting
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_ALARM_ACTIVE)) return;

			TagPublicClass tp = getSelectedTagPublicClass();			
			if(tp == null) return;

			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					TagAiClass ai = (TagAiClass)tp;
					ai.alarm = ( ai.alarm == 1 ) ? (byte)0 : (byte)1;
					break;
				case EnumTagType.DI :
					TagDiClass di = (TagDiClass)tp;
					di.alarm = ( di.alarm == 1 ) ? (sbyte)0 : (sbyte)1;
					break;
				default : return;
			}
			list.oneLineInvalidate(list.currPos);
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
		}

		void setTagDataProperty()		// Tag Data property setting
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_DATASAVE)) return;

			TagPublicClass tp = getSelectedTagPublicClass();			
			if(tp == null) return;

			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					TagAiClass ai = (TagAiClass)tp;
					ai.bFileSave = ( ai.bFileSave == 1 ) ? (byte)0 : (byte)1;
                    TagLib.bNeedFileSaveList = true;   // 파일저장 목록을 새로 만들어야 한다.
					break;
				case EnumTagType.DI :
					TagDiClass di = (TagDiClass)tp;
					di.bFileSave = ( di.bFileSave == 1 ) ? (sbyte)0 : (sbyte)1;
                    TagLib.bNeedFileSaveList = true;   // 파일저장 목록을 새로 만들어야 한다.
					break;
				default : return;
			}
			list.oneLineInvalidate(list.currPos);
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
		}

		public void callTrendWindows(int hour)
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) return;
			if(tp.enumTagType != EnumTagType.AI && tp.enumTagType != EnumTagType.DI) return;

			ArrayList arr = new ArrayList();
			multiTrendTagStruct multi = new multiTrendTagStruct();
			multi.tag = tp.tag;
			multi.color = Color.Black;
			multi.edge = 0;
			arr.Add(multi);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				ViewAnalogInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputTrendMain(hour, arr, Color.White, 14), ConfigViewMain.nMdiCountOnBasicScreen);
			}
			if(tp.enumTagType == EnumTagType.DI) 
			{
				ViewDigitalInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputTrendMain(hour, arr, Color.White), ConfigViewMain.nMdiCountOnBasicScreen);
			}
		}

		void callDataWindows(eDataTime dataTime)
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) return;
			if(tp.enumTagType != EnumTagType.AI && tp.enumTagType != EnumTagType.DI) return;

			ArrayList arr = new ArrayList();
			multiTrendTagStruct multi = new multiTrendTagStruct();
			multi.tag = tp.tag;
			multi.color = Color.Black;
			multi.edge = 0;
			arr.Add(multi);

			if(tp.enumTagType == EnumTagType.AI) 
			{
				ViewAnalogInputDataMain.ringViewAnalogInputDataMain.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDataMain(dataTime, arr, Color.White, (int)eDataDispType.DECIMAL), ConfigViewMain.nMdiCountOnBasicScreen);
			}
			if(tp.enumTagType == EnumTagType.DI) 
			{
				ViewDigitalInputDataMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputDataMain(dataTime, arr, SharedData.colorTotal.BACK, (int)eDataDispType.DECIMAL), ConfigViewMain.nMdiCountOnBasicScreen);
			}
		}

		public void callAiWindows()
		{
			ViewAnalogInputMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void callAoWindows()
		{
			ViewAnalogOutputMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogOutputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void callDiWindows()
		{
			ViewDigitalInputMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void callDoWindows()
		{
			ViewDigitalOutputMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalOutputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public void callStWindows()
		{
			ViewStringTagMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewStringTagMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		void groupListTagSearchNameOrPos()
		{
			if(list.listHap <= 1) return;

			TagGrClass				gr = new TagGrClass();
			groupShowTagMember		gs;
			TagPublicClass			tp;

			for(int i = 0; i < list.listHap; i++) 
			{
				gs = (groupShowTagMember)grShow.tag[i];
				if(gs == null) continue;
				tp = TagLib.GetStructPublic(gs.tag, ref gs.tagPos);
				if(tp == null) continue;
				gr.AddTag(tp, false);
			}			
			int			pos = getSelectedTagPos();
			ViewTagNamePosSearchDlg dialog = new ViewTagNamePosSearchDlg(pos, list.listHap);
			dialog.gr = gr;
			dialog.bTagList = false;
			dialog.ShowDialog(this);
			if(dialog.bTagChanged && pos != dialog.currPos)		// 값이 변경되었다.
			{
				list.listSelectedPosChange(dialog.currPos);
				setDetailButtonEnableDisable();
			}
		}

		void callHandInputWindows()
		{
			TagPublicClass tp = getSelectedTagPublicClass();
			if(tp == null) return;
			if(tp.enumTagType != EnumTagType.AI && tp.enumTagType != EnumTagType.DI) return;
			
			if(tp.enumTagType == EnumTagType.AI) 
			{
				TagAiClass ai = (TagAiClass)tp;
				ViewAnalogInputHandInputDlg dialogAi = new ViewAnalogInputHandInputDlg(ai);
				dialogAi.ShowDialog(this);				
				if(dialogAi.bElementChanged) list.oneLineInvalidate(list.currPos);
				TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
				
			}
			if(tp.enumTagType == EnumTagType.DI) 
			{
				TagDiClass di = (TagDiClass)tp;
				ViewDigitalInputHandInputDlg dialogDi = new ViewDigitalInputHandInputDlg(di);
				dialogDi.ShowDialog(this);
				if(dialogDi.bElementChanged) list.oneLineInvalidate(list.currPos);
				TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
			}
		}


        private void OnEventTagChanged(TagPublicClass tagevent)
		{
			TagPublicClass		tp;
			groupShowTagMember	gs;

			for(int i = 0; i < list.listHap; i++) 
			{
				gs = (groupShowTagMember)grShow.tag[i];
				if(gs == null) continue;
				tp = TagLib.GetStructPublic(gs.tag, ref gs.tagPos);
				if(tp == null || tp.tag != tagevent.tag) continue;
				list.oneLineInvalidate(i);
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
			TagPublicClass		tp2;
			groupShowTagMember	gs;
			for(int i = 0; i < list.listHap; i++) 
			{
				gs = (groupShowTagMember)grShow.tag[i];
				if(gs == null) continue;
				tp2 = TagLib.GetStructPublic(gs.tag, ref gs.tagPos);
				if(tp2 == null || tp2 != tp) continue;
				list.oneLineInvalidate(i);

			}
		}

		void OnTagListChanged()
		{
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

		int drawStartIcon(Graphics g, int x, int y, int index)
		{
			int			gap = list.fontY-TotalResource.res.imageListTagType.Images[index].Height;

			gap = (gap <= 0) ? 0 : gap/2;
			g.DrawImageUnscaled(TotalResource.res.imageListTagType.Images[index], x, y+gap);
			return TotalResource.res.imageListTagType.Images[index].Width;
		}

		void oneLineDraw(Graphics g, TagPublicClass	tp, int i, int x, int y, int xGap)
		{
            Color color_etc = (tp.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;

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
                    int ImageIndex = 9999;

                    if (TotalResource.res.imageListTagType != null)
                    {
                        switch (tp.enumTagType)
                        {
                            case EnumTagType.AI: ImageIndex = (int)TotalResource.ImageTagType.AI; break;
                            case EnumTagType.AO: ImageIndex = (int)TotalResource.ImageTagType.AO; break;
                            case EnumTagType.DI: ImageIndex = (int)TotalResource.ImageTagType.DI; break;
                            case EnumTagType.DO: ImageIndex = (int)TotalResource.ImageTagType.DO; break;
                            case EnumTagType.GR: ImageIndex = (int)TotalResource.ImageTagType.GR; break;
                            case EnumTagType.ST: ImageIndex = (int)TotalResource.ImageTagType.ST; break;
                            case EnumTagType.GDO: ImageIndex = (int)TotalResource.ImageTagType.GDO; break;
                        }

                        if (ImageIndex < TotalResource.res.imageListTagType.Images.Count)
                        {
                            int width = drawStartIcon(g, x, y, ImageIndex);
                            if (head.width - width > 0)
                                DrawClass.WinDrawText(g, x + xGap + width, y, head.width - xGap * 2 - width, (int)list.fontY, i.ToString(), color_etc, list.backColor, list.font, format);
                        }
                    }
                    else
                        DrawClass.WinDrawText(g, x + xGap, y, head.width - xGap * 2, (int)list.fontY, i.ToString(), color_etc, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Tag)
                {
                    Color color = (tp.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, tp.tag, color, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Description)
                {
                    Color color = (tp.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, tp.description, color, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Value)
                {
                    string value_string;

                    switch (tp.enumTagType)
                    {
                        case EnumTagType.AI:
                            BasicScreenTool.AiCurrValAndProgBarDraw(g, (TagAiClass)tp, x + xGap, y, head.width, list.fontY, xGap, color_etc, list.backColor, list.font);
                            break;
                        case EnumTagType.AO:
                            TagAoClass ao = (TagAoClass)tp;
                            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, ao.curr.ToString(), color_etc, list.backColor, list.font, format_far);
                            break;
                        case EnumTagType.DI:
                            BasicScreenTool.DiCurrValAndStatusButtonDraw(g, (TagDiClass)tp, x, y, head.width, list.fontY, list.font);
                            break;
                        case EnumTagType.DO:
                            BasicScreenTool.DoCurrValAndStatusButtonDraw(g, (TagDoClass)tp, x, y, head.width, list.fontY, list.font);
                            break;
                        case EnumTagType.ST:
                            TagStClass st = (TagStClass)tp;
                            value_string = TagUtil.ApplyDeviceQuality(st, st.curr);
                            DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, value_string, color_etc, list.backColor, list.font, format);
                            break;
                        case EnumTagType.GDO:
                            BasicScreenTool.DoGroupCurrValAndStatusButtonDraw(g, (TagDoGroupClass)tp, x, y, head.width, list.fontY, list.font);
                            break;
                        default:
                            break;
                    }
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Unit)
                {
                    string unit = "", data = "", alarm = "";
                    getCurrUnitDataAlarm(tp, ref unit, ref data, ref alarm);

                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, unit, color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Data)
                {
                    string unit = "", data = "", alarm = "";
                    getCurrUnitDataAlarm(tp, ref unit, ref data, ref alarm);

                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, data, color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Alarm)
                {
                    string unit = "", data = "", alarm = "";
                    getCurrUnitDataAlarm(tp, ref unit, ref data, ref alarm);

                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, alarm, color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.SV_Out1)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, getAoSvOut1Tag(tp), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.HandInput)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, getHandInputUseString(tp), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Port)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, tp.port.ToString(), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Address)
                {
                    string address;
                    if (tp.enumTagType == EnumTagType.AI)
                    {
                        address = ((TagAiClass)tp).address.ToString();
                    }
                    else if (tp.enumTagType == EnumTagType.AO)
                    {
                        address = ((TagAoClass)tp).address.ToString("X");
                    }
                    else if (tp.enumTagType == EnumTagType.DI)
                    {
                        TagDiClass di = (TagDiClass)tp;
                        address = di.address_word.ToString() + "." + di.address_bit.ToString("X");
                    }
                    else if (tp.enumTagType == EnumTagType.DO)
                    {
                        address = ((TagDoClass)tp).address.ToString("X");
                    }
                    else if (tp.enumTagType == EnumTagType.ST)
                    {
                        address = ((TagStClass)tp).address.ToString();
                    }
                    else
                    {
                        address = "";
                    }

                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, address, color_etc, list.backColor, list.font, head.format);
                }

                x += head.width;
            }

            /*
			Color					color;
			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];
			int						ImageIndex = 9999;
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;

			switch(tp.enumTagType) 
			{
				case EnumTagType.AI : ImageIndex = (int)TotalResource.ImageTagType.AI; break;
				case EnumTagType.AO : ImageIndex = (int)TotalResource.ImageTagType.AO; break;
				case EnumTagType.DI : ImageIndex = (int)TotalResource.ImageTagType.DI; break;
				case EnumTagType.DO : ImageIndex = (int)TotalResource.ImageTagType.DO; break;
				case EnumTagType.GR : ImageIndex = (int)TotalResource.ImageTagType.GR; break;
				case EnumTagType.ST : ImageIndex = (int)TotalResource.ImageTagType.ST; break;
				case EnumTagType.GDO: ImageIndex = (int)TotalResource.ImageTagType.GDO;break;
			}

			color = (tp.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			if(TotalResource.res.imageListTagType != null) 
			{
				if(ImageIndex < TotalResource.res.imageListTagType.Images.Count) 
				{
					int		width = drawStartIcon(g, x, y, ImageIndex);
					if(head.width-width > 0)
						DrawClass.WinDrawText(g, x+xGap+width, y, head.width-xGap*2-width, (int)list.fontY, i.ToString(), color, list.backColor, list.font, format);
				}
			}
			else
				DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, i.ToString(), color, list.backColor, list.font, format);

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, tp.tag, color, list.backColor, list.font, format);

			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, tp.description, color, list.backColor, list.font, format);

			x += head.width;
			head = (ControlListViewHeader)list.header[3];
			switch(tp.enumTagType) 
			{
				case EnumTagType.AI :
					BasicScreenTool.AiCurrValAndProgBarDraw(g, (TagAiClass)tp, x+xGap, y, head.width, list.fontY, xGap, color, list.backColor, list.font);
					break;
				case EnumTagType.AO :
					format.Alignment = StringAlignment.Far;
					TagAoClass ao = (TagAoClass)tp;
					DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, ao.curr.ToString(), color, list.backColor, list.font, format);
					return;
				case EnumTagType.DI :
					BasicScreenTool.DiCurrValAndStatusButtonDraw(g, (TagDiClass)tp, x, y, head.width, list.fontY, list.font);
					break;
				case EnumTagType.DO :
					BasicScreenTool.DoCurrValAndStatusButtonDraw(g, (TagDoClass)tp, x, y, head.width, list.fontY, list.font);
					return;
				case EnumTagType.ST :
					TagStClass st = (TagStClass)tp;
					DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, st.curr, color, list.backColor, list.font, format);
					return;
				case EnumTagType.GDO :
					BasicScreenTool.DoGroupCurrValAndStatusButtonDraw(g, (TagDoGroupClass)tp, x, y, head.width, list.fontY, list.font);
					return;
				default :
					return;
			}			

			string		unit = "", data = "", alarm = "";
			getCurrUnitDataAlarm(tp, ref unit, ref data, ref alarm);
			x += head.width;
			head = (ControlListViewHeader)list.header[4];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, unit, color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[5];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data, color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[6];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, alarm, color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[7];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getAoSvOut1Tag(tp), color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[8];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getHandInputUseString(tp), color, list.backColor, list.font, head.format);*/
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);
			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
			groupShowTagMember		gs;
			TagPublicClass			tp;

			endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;
				
				gs = (groupShowTagMember)grShow.tag[pos];
				if(gs == null) continue;
				tp = TagLib.GetStructPublic(gs.tag, ref gs.tagPos);
				if(tp == null) continue;

				x = list.startX;
				oneLineDraw(g, tp, pos+1, x, y, xGap);
			}
		}

		void onPaintMessage(Graphics g, Rectangle r)
		{
			DrawData(g, r);
		}

		private void ViewRegisterGroupDetailMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
		}

		private void ViewRegisterGroupDetailMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Escape : Close(); return;
				case Keys.Enter : callDetailWindow(); return;
				case Keys.F3 : callValueChangeWindows(); return;
				case Keys.F4 : callTrendWindows(1); return;
				case Keys.F6 : callDataWindows(eDataTime.HOUR); return;
				case Keys.F7 : setTagAlarmProperty(); return;
				case Keys.F8 : setTagDataProperty(); return;
				case Keys.F9 : setTagActiveInActive(); return;
				case Keys.F11 : CallSelectTagPropertyWindows(); return;
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

		private void menuItem_ViewDetail_Click(object sender, System.EventArgs e)
		{
			callDetailWindow();
		}

		private void menuItem_AiTotalView_Click(object sender, System.EventArgs e)
		{			
			callAiWindows();
		}


		private void menuItem_AoTotalView_Click(object sender, System.EventArgs e)
		{
			callAoWindows();
		}		

		private void menuItem_DiTotalView_Click(object sender, System.EventArgs e)
		{
			callDiWindows();
		}
		
		private void menuItem_DoTotalView_Click(object sender, System.EventArgs e)
		{
			callDoWindows();
		}

		private void menuItem_StTotalView_Click(object sender, System.EventArgs e)
		{
			callStWindows();
		}

		private void menuItem_1Trend_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(1);
		}

		private void menuItem_8Trend_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(8);
		}

		private void menuItem_24Trend_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(24);
		}

		private void menuItem_48Trend_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(48);
		}

		private void menuItem_72Trend_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(72);
		}

		private void menuItem_30Trend_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(720);
		}

		private void menuItem_MinData_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.MIN);
		}

		private void menuItem_HourData_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.HOUR);
		}

		private void menuItem_DayData_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.DAY);
		}

		private void menuItem_WeekData_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.WEEK);
		}

		private void menuItem_MonthData_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.MONTH);
		}

		private void menuItem_ValueChange_Click(object sender, System.EventArgs e)
		{
			callValueChangeWindows();
		}
				
		private void menuItem_TagProperityModify_Click(object sender, System.EventArgs e)
		{
			CallSelectTagPropertyWindows();
		}

		private void menuItem_HandInput_Click(object sender, System.EventArgs e)
		{
			callHandInputWindows();
		}

		private void menuItem_TagSearch_Click(object sender, System.EventArgs e)
		{
			groupListTagSearchNameOrPos();
		}

		bool	bTimerTick = false;

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(list.listHap <= 0) return;

			bTimerTick = !bTimerTick;
			if(bTimerTick == false) return;				// timer를 유연하게 하기 위해

			TagPublicClass		tp;
			groupShowTagMember	gs;
			for(int i = list.startPos; i < list.listHap; i++) 
			{
				if(i > list.startPos + list.pageLineCount) return;

				gs = (groupShowTagMember)grShow.tag[i];
				if(gs == null) continue;
				tp = TagLib.GetStructPublic(gs.tag, ref gs.tagPos);
				if(tp == null || tp.act == 0) continue;
				tp.NeedDataCurr = true;
			}
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if(e.Button == toolBarButton1) this.Close();
			else if(e.Button == toolBarButton2) callDetailWindow();
			else if(e.Button == toolBarButton3) callValueChangeWindows();
			else if(e.Button == toolBarButton4) callTrendWindows(1);
			else if(e.Button == toolBarButton5) callDataWindows(eDataTime.HOUR);
			else if(e.Button == toolBarButton6) setTagAlarmProperty();
			else if(e.Button == toolBarButton7) setTagDataProperty();
			else if(e.Button == toolBarButton8) setTagActiveInActive();
			else if(e.Button == toolBarButton9) CallSelectTagPropertyWindows();
			else if(e.Button == toolBarButton10) CallTagAlarmLevelChangeWindows();
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewRegisterGroupDetailMain_Closed(object sender, System.EventArgs e)
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

            list.ConfigSave(TotalConfig.AutoBaseIniGetConfigDirectory(), "BasicScreen", "RegisteredGroupTag");

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
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "RegisteredGroupTag");*/

			ringForm.pop(this);
		}

        private void menuItem4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            list.DialogConfigColumn();
        }		
		
		

		
		
		

	}
}
