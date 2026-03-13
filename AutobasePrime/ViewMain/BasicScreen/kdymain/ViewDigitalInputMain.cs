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
	/// Summary description for ViewDigitalInputMain.
	/// </summary>
	public class ViewDigitalInputMain : System.Windows.Forms.Form	//AnalogDigitalCommonDrawClass
	{
		public System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.ToolBarButton toolBarButton8;
		private System.Windows.Forms.ToolBarButton toolBarButton9;
		private System.Windows.Forms.ContextMenu contextMenuDI;
		private System.Windows.Forms.MenuItem menuItem_diDetail;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour1;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour8;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour24;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour48;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_hour72;
		private System.Windows.Forms.MenuItem menuItem_DiTrend_day30;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem_DiData_min;
		private System.Windows.Forms.MenuItem menuItem_DiData_hour;
		private System.Windows.Forms.MenuItem menuItem_DiData_day;
		private System.Windows.Forms.MenuItem menuItem_DiData_week;
		private System.Windows.Forms.MenuItem menuItem_DiData_month;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem_di_curr_change;
		private System.Windows.Forms.MenuItem menuItem_TagProperityModify;
		private System.Windows.Forms.MenuItem menuItem_HandInput;
        private System.Windows.Forms.MenuItem menuItem_TagSearch;
		private System.Windows.Forms.MenuItem menuItem3;

		TagListStruct[]			tagList;
		//int[]					headWidth = new int[9];
		ControlListView			list = new ControlListView();
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Timer timer1;
        private MenuItem menuItem5;
        private MenuItem menuItem6;

        TagListViewColumn tlvc = new TagListViewColumn();

		public ViewDigitalInputMain(string tag)
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
			headWidth[1] = 100;
			headWidth[2] = 120;
			headWidth[3] = 130;
			headWidth[4] = 50;
			headWidth[5] = 50;
			headWidth[6] = 50;
			headWidth[7] = 80;
			headWidth[8] = 80;*/
		}

		void getTagListAndLength()
		{
			tagList = TagLib.GetTagList(EnumTagType.DI);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputMain));
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
            this.contextMenuDI = new System.Windows.Forms.ContextMenu();
            this.menuItem_diDetail = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour1 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour8 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour24 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour48 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_hour72 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiTrend_day30 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_min = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_hour = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_day = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_week = new System.Windows.Forms.MenuItem();
            this.menuItem_DiData_month = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem_di_curr_change = new System.Windows.Forms.MenuItem();
            this.menuItem_TagProperityModify = new System.Windows.Forms.MenuItem();
            this.menuItem_HandInput = new System.Windows.Forms.MenuItem();
            this.menuItem_TagSearch = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            this.toolBarButton9});
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
            // contextMenuDI
            // 
            this.contextMenuDI.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_diDetail,
            this.menuItem4,
            this.menuItem_DiTrend_hour1,
            this.menuItem_DiTrend_hour8,
            this.menuItem_DiTrend_hour24,
            this.menuItem_DiTrend_hour48,
            this.menuItem_DiTrend_hour72,
            this.menuItem_DiTrend_day30,
            this.menuItem2,
            this.menuItem_DiData_min,
            this.menuItem_DiData_hour,
            this.menuItem_DiData_day,
            this.menuItem_DiData_week,
            this.menuItem_DiData_month,
            this.menuItem1,
            this.menuItem_di_curr_change,
            this.menuItem_TagProperityModify,
            this.menuItem_HandInput,
            this.menuItem_TagSearch,
            this.menuItem5,
            this.menuItem6,
            this.menuItem3});
            resources.ApplyResources(this.contextMenuDI, "contextMenuDI");
            // 
            // menuItem_diDetail
            // 
            resources.ApplyResources(this.menuItem_diDetail, "menuItem_diDetail");
            this.menuItem_diDetail.Index = 0;
            this.menuItem_diDetail.Click += new System.EventHandler(this.menuItem_diDetail_Click);
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 1;
            // 
            // menuItem_DiTrend_hour1
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour1, "menuItem_DiTrend_hour1");
            this.menuItem_DiTrend_hour1.Index = 2;
            this.menuItem_DiTrend_hour1.Click += new System.EventHandler(this.menuItem_DiTrend_hour1_Click);
            // 
            // menuItem_DiTrend_hour8
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour8, "menuItem_DiTrend_hour8");
            this.menuItem_DiTrend_hour8.Index = 3;
            this.menuItem_DiTrend_hour8.Click += new System.EventHandler(this.menuItem_DiTrend_hour8_Click);
            // 
            // menuItem_DiTrend_hour24
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour24, "menuItem_DiTrend_hour24");
            this.menuItem_DiTrend_hour24.Index = 4;
            this.menuItem_DiTrend_hour24.Click += new System.EventHandler(this.menuItem_DiTrend_hour24_Click);
            // 
            // menuItem_DiTrend_hour48
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour48, "menuItem_DiTrend_hour48");
            this.menuItem_DiTrend_hour48.Index = 5;
            this.menuItem_DiTrend_hour48.Click += new System.EventHandler(this.menuItem_DiTrend_hour48_Click);
            // 
            // menuItem_DiTrend_hour72
            // 
            resources.ApplyResources(this.menuItem_DiTrend_hour72, "menuItem_DiTrend_hour72");
            this.menuItem_DiTrend_hour72.Index = 6;
            this.menuItem_DiTrend_hour72.Click += new System.EventHandler(this.menuItem_DiTrend_hour72_Click);
            // 
            // menuItem_DiTrend_day30
            // 
            resources.ApplyResources(this.menuItem_DiTrend_day30, "menuItem_DiTrend_day30");
            this.menuItem_DiTrend_day30.Index = 7;
            this.menuItem_DiTrend_day30.Click += new System.EventHandler(this.menuItem_DiTrend_day30_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 8;
            // 
            // menuItem_DiData_min
            // 
            resources.ApplyResources(this.menuItem_DiData_min, "menuItem_DiData_min");
            this.menuItem_DiData_min.Index = 9;
            this.menuItem_DiData_min.Click += new System.EventHandler(this.menuItem_DiData_min_Click);
            // 
            // menuItem_DiData_hour
            // 
            resources.ApplyResources(this.menuItem_DiData_hour, "menuItem_DiData_hour");
            this.menuItem_DiData_hour.Index = 10;
            this.menuItem_DiData_hour.Click += new System.EventHandler(this.menuItem_DiData_hour_Click);
            // 
            // menuItem_DiData_day
            // 
            resources.ApplyResources(this.menuItem_DiData_day, "menuItem_DiData_day");
            this.menuItem_DiData_day.Index = 11;
            this.menuItem_DiData_day.Click += new System.EventHandler(this.menuItem_DiData_day_Click);
            // 
            // menuItem_DiData_week
            // 
            resources.ApplyResources(this.menuItem_DiData_week, "menuItem_DiData_week");
            this.menuItem_DiData_week.Index = 12;
            this.menuItem_DiData_week.Click += new System.EventHandler(this.menuItem_DiData_week_Click);
            // 
            // menuItem_DiData_month
            // 
            resources.ApplyResources(this.menuItem_DiData_month, "menuItem_DiData_month");
            this.menuItem_DiData_month.Index = 13;
            this.menuItem_DiData_month.Click += new System.EventHandler(this.menuItem_DiData_month_Click);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Index = 14;
            // 
            // menuItem_di_curr_change
            // 
            resources.ApplyResources(this.menuItem_di_curr_change, "menuItem_di_curr_change");
            this.menuItem_di_curr_change.Index = 15;
            this.menuItem_di_curr_change.Click += new System.EventHandler(this.menuItem_di_curr_change_Click);
            // 
            // menuItem_TagProperityModify
            // 
            resources.ApplyResources(this.menuItem_TagProperityModify, "menuItem_TagProperityModify");
            this.menuItem_TagProperityModify.Index = 16;
            this.menuItem_TagProperityModify.Click += new System.EventHandler(this.menuItem_TagProperityModify_Click);
            // 
            // menuItem_HandInput
            // 
            resources.ApplyResources(this.menuItem_HandInput, "menuItem_HandInput");
            this.menuItem_HandInput.Index = 17;
            this.menuItem_HandInput.Click += new System.EventHandler(this.menuItem_HandInput_Click);
            // 
            // menuItem_TagSearch
            // 
            resources.ApplyResources(this.menuItem_TagSearch, "menuItem_TagSearch");
            this.menuItem_TagSearch.Index = 18;
            this.menuItem_TagSearch.Click += new System.EventHandler(this.menuItem_TagSearch_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 19;
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem6
            // 
            resources.ApplyResources(this.menuItem6, "menuItem6");
            this.menuItem6.Index = 20;
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 21;
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
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
            // ViewDigitalInputMain
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "ViewDigitalInputMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ViewDigitalInputMain_Load);
            this.SizeChanged += new System.EventHandler(this.ViewDigitalInputMain_SizeChanged);
            this.Closed += new System.EventHandler(this.ViewDigitalInputMain_Closed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ViewDigitalInputMain_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public static AutoLibLocal.CatWindowRing ringForm = new AutoLibLocal.CatWindowRing();

		private void ViewDigitalInputMain_Load(object sender, System.EventArgs e)
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
			            			
			//TotalConfig.AutoBaseListCtrlConfigLoad(ref headWidth, "BasicScreen", "DigitalInputMain");
			
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
            }
		}

		void listHeaderFill()
		{
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.No, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Tag, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Description, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Value, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Data, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Alarm, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Out1, true);
            tlvc.AddColumn(TagListViewColumn.EnumTagListColumn.Out2, true);
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

            list.ConfigLoad(TotalConfig.AutoBaseIniGetConfigDirectory(), "BasicScreen", "DigitalInputMain");

            /*
			ControlListViewHeader	head;
			string[]				text = new string[9];
						
			if(Tools.IsLangKorean()) 
			{
				text[0] = "순서";
				text[1] = "태그이름";
				text[2] = "설명";
				text[3] = "현재 값";
				text[4] = "자료";
				text[5] = "경보";
				text[6] = "Out1";
				text[7] = "Out2";
				text[8] = "수동기입";
			}
			else if(Tools.IsLangJapanese()) 
			{
				text[0] = "No";
				text[1] = "タグ名";
				text[2] = "説明";
				text[3] = "現在値";
				text[4] = "データ";
                text[5] = "警報";
				text[6] = "Out1";
				text[7] = "Out2";
				text[8] = "手動記入";
			}
			else if(Tools.IsLangChinese()) 
			{
				text[0] = "顺序";
				text[1] = "标记名";
				text[2] = "标记描述";
				text[3] = "现在值";
				text[4] = "资料";
				text[5] = "警报";
				text[6] = "Out1";
				text[7] = "Out2";
				text[8] = "手动输入";
			}
            else if (Tools.IsLangVietnamese())
            {
                text[0] = "Số";
                text[1] = "Tên Tag";
                text[2] = "Mô tả";
                text[3] = "Giá trị hiện tại";
                text[4] = "Data";
                text[5] = "Alarm";
                text[6] = "Out1";
                text[7] = "Out2";
                text[8] = "Hand Input";
            }
			else 
			{
				text[0] = "No";
				text[1] = "Tag Name";
				text[2] = "Description";
				text[3] = "Current Value";
				text[4] = "Data";
				text[5] = "Alarm";
				text[6] = "Out1";
				text[7] = "Out2";
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
				head.color = SharedData.colorTotal.TEXT;// 기본으로 사용할 색상,
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
                list.contextMenu = this.contextMenuDI;
            }
		}

		void fillListDataAll()
		{
			if(tagList == null) list.listHap = 0;
			else list.listHap = tagList.Length;
			list.listItemChanged();			
			setDetailButtonEnableDisable();
		}

		void getCurrDataAlarmString(TagDiClass di, ref string data, ref string alarm)
		{
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
		}

		string getOut1String(TagDiClass di)
		{
			if(di.sSubOutDigital1 != null && di.sSubOutDigital1.Length >= 1) return di.sSubOutDigital1;
			return "";
		}

		string getOut2String(TagDiClass di)
		{
			if(di.sSubOutDigital2 != null && di.sSubOutDigital2.Length >= 1) return di.sSubOutDigital2;
			return "";
		}

		string getHandInputUseString(TagDiClass di)
		{
			if(((ushort)di.wProtectFlags & (ushort)EnumProtectFlag.SCAN) != 0) 
			{
				if(Tools.IsLangKorean()) return "수동기입";
				else if(Tools.IsLangJapanese()) return "手動記入";
				else if(Tools.IsLangJapanese()) return "手动输入";
				else return "Hand Input";
			}
			return "";
		}

		void setTrendDataButtonMenuEnableDisable(bool flag)
		{
			toolBarButton2.Enabled = flag;					
			toolBarButton3.Enabled = flag;
			toolBarButton4.Enabled = flag;
			toolBarButton5.Enabled = flag;
			menuItem_diDetail.Enabled = flag;
			menuItem_DiTrend_hour1.Enabled = flag;
			menuItem_DiTrend_hour8.Enabled = flag;
			menuItem_DiTrend_hour24.Enabled = flag;
			menuItem_DiTrend_hour48.Enabled = flag;
			menuItem_DiTrend_hour72.Enabled = flag;
			menuItem_DiTrend_day30.Enabled = flag;
			menuItem_DiData_min.Enabled = flag;
			menuItem_DiData_hour.Enabled = flag;
			menuItem_DiData_day.Enabled = flag;
			menuItem_DiData_week.Enabled = flag;
			menuItem_DiData_month.Enabled = flag;
			menuItem_di_curr_change.Enabled = flag;
			menuItem_HandInput.Enabled = flag;
		}

		void setDefaultMenuEnableDisable(bool flag)
		{
			menuItem_TagProperityModify.Enabled = flag;
			menuItem_TagSearch.Enabled = flag;			
			toolBarButton6.Enabled = flag;		// Alarm?
			toolBarButton7.Enabled = flag;		// Data?
			toolBarButton8.Enabled = flag;		// act?
			toolBarButton9.Enabled = flag;		// tag 속성수정
		}

		void setDetailButtonEnableDisable()		// 상세 버턴을 Enable/Disable
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) 
			{
				setTrendDataButtonMenuEnableDisable(false);
				setDefaultMenuEnableDisable(false);
				return;
			}
			setDefaultMenuEnableDisable(true);
			setTrendDataButtonMenuEnableDisable((di.act == 1) ? true : false);
            toolBarButton6.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_ALARM_ACTIVE);//alarm?
            toolBarButton7.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_DATASAVE);//data?
            toolBarButton8.Enabled = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE);//act?
		}

		int getSelectedTagPos()
		{
			return list.currPos;
		}

		public void callDetailWindow()
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;
			BasicScreenTool.callDetailWindow((TagPublicClass)di);
		}

		void setTagActiveInActive()		// Tag Active/Inactive
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_CHANGE)) return;

			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;

			di.act = ( di.act == 1 ) ? (sbyte)0 : (sbyte)1;
			list.oneLineInvalidate(list.currPos);
			setDetailButtonEnableDisable();			// active가 바뀌었으므로 버턴을 활성/비활성
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해

			LibComNetServer.SendCommandToNetWorkTagMemberChanged(di.tag, EnumTagMember.TAG_MEMBER_act, di.act);
		}

		void setTagAlarmProperty()		// Tag Alarm property setting
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_ALARM_ACTIVE)) return;

			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;

			di.alarm = ( di.alarm == 1 ) ? (sbyte)0 : (sbyte)1;
			list.oneLineInvalidate(list.currPos);
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해

			LibComNetServer.SendCommandToNetWorkTagMemberChanged(di.tag, EnumTagMember.TAG_MEMBER_alarm, di.alarm);
		}

		void setTagDataProperty()		// Tag Data property setting
		{
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_DATASAVE)) return;

			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;

			di.bFileSave = ( di.bFileSave == 1 ) ? (sbyte)0 : (sbyte)1;			
			list.oneLineInvalidate(list.currPos);
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해

			LibComNetServer.SendCommandToNetWorkTagMemberChanged(di.tag, EnumTagMember.TAG_MEMBER_bFileSave, di.bFileSave);

            TagLib.bNeedFileSaveList = true;   // 파일저장 목록을 새로 만들어야 한다.
		}

		public void callTrendWindows(int hour)
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;
			
			ArrayList arr = new ArrayList();
			multiTrendTagStruct multi = new multiTrendTagStruct();
			multi.tag = di.tag;
			multi.color = Color.Black;
			multi.edge = 0;
			arr.Add(multi);

			ViewDigitalInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputTrendMain(hour, arr, Color.White), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		void callDataWindows(eDataTime dataTime)
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;

			ArrayList arr = new ArrayList();
			multiTrendTagStruct multi = new multiTrendTagStruct();
			multi.tag = di.tag;
			multi.color = Color.Black;
			multi.edge = 0;
			arr.Add(multi);

			ViewDigitalInputDataMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputDataMain(dataTime, arr, Color.White, (int)eDataDispType.DECIMAL), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		TagDiClass getSelectedDiTagClass()
		{
			if(list.currPos < 0 || list.currPos >= list.listHap || tagList == null) return null;
			if(tagList.Length <= list.currPos) return null;

			return TagLib.GetStructDI(tagList[list.currPos]);
		}

		void callValueChangeWindows()
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;
			if(BasicScreenTool.callValueChangeWindows(this, (TagPublicClass)di))
				list.oneLineInvalidate(list.currPos);

		}

		
		void CallSelectTagPropertyWindows()
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;
			if(DialogTag.TagEditor.Editor.ByViewMain(di)) 
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
		
		void callHandInputWindows()
		{
			TagDiClass di = getSelectedDiTagClass();
			if(di == null) return;
			ViewDigitalInputHandInputDlg dialogDi = new ViewDigitalInputHandInputDlg(di);
            dialogDi.StartPosition = FormStartPosition.CenterParent;
			dialogDi.ShowDialog(this);
			if(dialogDi.bElementChanged)
			{
				list.oneLineInvalidate(list.currPos);
				TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
			}
		}
		
		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			setDetailButtonEnableDisable();
		}

		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			callDetailWindow();
		}

		private void menuItem_diDetail_Click(object sender, System.EventArgs e)
		{
			callDetailWindow();
		}

		private void menuItem_DiTrend_hour1_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(1);
		}

		private void menuItem_DiTrend_hour8_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(8);
		}

		private void menuItem_DiTrend_hour24_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(24);
		}

		private void menuItem_DiTrend_hour48_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(48);
		}

		private void menuItem_DiTrend_hour72_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(72);
		}

		private void menuItem_DiTrend_day30_Click(object sender, System.EventArgs e)
		{
			callTrendWindows(720);
		}

		private void menuItem_DiData_min_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.MIN);
		}

		private void menuItem_DiData_hour_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.HOUR);
		}

		private void menuItem_DiData_day_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.DAY);
		}

		private void menuItem_DiData_week_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.WEEK);
		}

		private void menuItem_DiData_month_Click(object sender, System.EventArgs e)
		{
			callDataWindows(eDataTime.MONTH);
		}

		private void menuItem_di_curr_change_Click(object sender, System.EventArgs e)
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
			TagSearchNameOrPos();
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
		}

		private void ViewDigitalInputMain_SizeChanged(object sender, System.EventArgs e)
		{
			this.panel1.Height = this.toolBar1.Height;
		}

        private void OnEventTagChanged(TagPublicClass tagevent)
		{
			if(tagevent.enumTagType != EnumTagType.DI) return;

			TagDiClass	di;

            for (int i = list.startPos; i < list.listHap && i <= list.startPos + list.pageLineCount; i++) 
			{
				di = TagLib.GetStructDI(tagList[i]);
				if(di == null || di.tag != tagevent.tag) continue;
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
			if(tp.enumTagType != EnumTagType.DI) return;
			TagDiClass	di;
			for(int i = 0; i < list.listHap; i++) 
			{
				di = TagLib.GetStructDI(tagList[i]);
				if(di == null || di != (TagDiClass)tp) continue;
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

		void oneLineDraw(Graphics g, TagDiClass di, int i, int x, int y, int xGap)
		{
            Color color_etc = (di.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;

            ControlListViewHeader head;
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;

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
                    Color color = (di.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, di.tag, color, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Description)
                {
                    Color color = (di.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, di.description, color, list.backColor, list.font, format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Value)
                {
                    BasicScreenTool.DiCurrValAndStatusButtonDraw(g, di, x, y, head.width, list.fontY, list.font);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Data)
                {
                    string data = "", alarm = "";
                    getCurrDataAlarmString(di, ref data, ref alarm);
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, data, color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Alarm)
                {
                    string data = "", alarm = "";
                    getCurrDataAlarmString(di, ref data, ref alarm);
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, alarm, color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Out1)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, getOut1String(di), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Out2)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, getOut2String(di), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.HandInput)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, getHandInputUseString(di), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Port)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, di.port.ToString(), color_etc, list.backColor, list.font, head.format);
                }
                else if (head.nID == (int)TagListViewColumn.EnumTagListColumn.Address)
                {
                    DrawClass.WinDrawText(g, x + xGap, y + 1, head.width - xGap * 2, (int)list.fontY, di.address_word.ToString() + "." + di.address_bit.ToString("X"), color_etc, list.backColor, list.font, head.format);
                }

                x += head.width;
            }

            /*
			Color					color;
			ControlListViewHeader	head = (ControlListViewHeader)list.header[0];
			StringFormat			format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			
			color = (di.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y, head.width-xGap*2, (int)list.fontY, i.ToString(), color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[1];
			color = (di.act == 1) ? SharedData.colorTotal.TAG : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, di.tag, color, list.backColor, list.font, format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[2];
			color = (di.act == 1) ? SharedData.colorTotal.DESCRIPTION : SharedData.colorTotal.INACTIVE;
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, di.description, color, list.backColor, list.font, format);
			
			x += head.width;
			head = (ControlListViewHeader)list.header[3];
			color = (di.act == 1) ? SharedData.colorTotal.TEXT : SharedData.colorTotal.INACTIVE;
			BasicScreenTool.DiCurrValAndStatusButtonDraw(g, di, x, y, head.width, list.fontY, list.font);
			
			string		data = "", alarm = "";
			getCurrDataAlarmString(di, ref data, ref alarm);
			x += head.width;
			head = (ControlListViewHeader)list.header[4];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, data, color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[5];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, alarm, color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[6];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getOut1String(di), color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[7];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getOut2String(di), color, list.backColor, list.font, head.format);

			x += head.width;
			head = (ControlListViewHeader)list.header[8];
			DrawClass.WinDrawText(g, x+xGap, y+1, head.width-xGap*2, (int)list.fontY, getHandInputUseString(di), color, list.backColor, list.font, head.format);*/
		}
		

		void DrawData(Graphics g, Rectangle r)
		{
			if(r.Bottom < list.headHeight) return;
			if(r.Top >= list.headHeight) DrawClass.gcls(g, r, list.backColor);
			else				    DrawClass.gcls(g, 0, list.headHeight, Width, r.Bottom, list.backColor);
			if(list.pageLineCount <= 0 || list.fontY <= 0 || list.listHap <= 0) return;
			
			int						pos, x, y = list.headHeight, xGap = (int)(list.fontX*0.25), endPos;
			TagDiClass				di;

			endPos = list.pageLineCount+list.startPos+1;		// 1줄 더 그린다
			if(endPos > list.listHap) endPos = list.listHap;
			
			for(pos = list.startPos; pos < endPos; pos++, y += list.fontY) 
			{
				if(y > r.Bottom) break;
				if(y+list.fontY < r.Top) continue;

				di = TagLib.GetStructDI(tagList[pos]);
				if(di == null) continue;
				x = list.startX;
				oneLineDraw(g, di, pos+1, x, y, xGap);
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

			TagDiClass		di;
			for(int i = list.startPos; i < list.listHap; i++) 
			{
				if(i > list.startPos + list.pageLineCount + 1) return;
				di = TagLib.GetStructDI(tagList[i]);
				if(di == null || di.act == 0) continue;
				di.NeedDataCurr = true;
			}
		}
		

		private void ViewDigitalInputMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
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
				case Keys.F11: CallSelectTagPropertyWindows(); return;
			}

			//ringForm.pop(this);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 
			//preventing drawing background by overriding parent OnPaintBackground method 
			//with empty method. 
		}

		private void ViewDigitalInputMain_Closed(object sender, System.EventArgs e)
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

            list.ConfigSave(TotalConfig.AutoBaseIniGetConfigDirectory(), "BasicScreen", "DigitalInputMain");

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
			if(bChange) TotalConfig.AutoBaseListCtrlConfigSave(ref headWidth, "BasicScreen", "DigitalInputMain");
             */
            ringForm.pop(this);            
		}

        private void menuItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            list.DialogConfigColumn();
        }
		
		
	}
}
