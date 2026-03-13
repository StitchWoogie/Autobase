using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using NetTools.OldDefine;
using AutoLib;
using AutoLibLocal;
using NetTools;
using BasicScreen.kdymain;
using DialogControl;
using System.IO;
using System.Threading.Tasks;
using static AutoLib.SharedViewMain;
using System.Globalization;


namespace GraphicModule
{
	public enum EnumViewMode
	{
		CONTROL,
		ZOOM_IN,
		ZOOM_OUT,
		PANNING,
        DIGITAL_SELECT,
	}
	/// <summary>
	/// Summary description for FormGraphic.
	/// </summary>
	public class FormGraphicChild : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public ObjectRoot objectGraphic;
		public string sFileName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.ContextMenu contextMenuMain;
		private System.Windows.Forms.MenuItem menuItemModuleReal;
		private System.Windows.Forms.MenuItem menuItemModuleFitWindow;
		private System.Windows.Forms.MenuItem menuItemModuleFitWindowXY;
		private System.Windows.Forms.MenuItem menuItemViewControl;
		private System.Windows.Forms.MenuItem menuItemViewZoomIn;
		private System.Windows.Forms.MenuItem menuItemViewZoomOut;
		private System.Windows.Forms.MenuItem menuItemViewPanning;
		private System.Windows.Forms.MenuItem menuItemViewPercent50;
		private System.Windows.Forms.MenuItem menuItemViewPercent75;
		private System.Windows.Forms.MenuItem menuItemViewPercent100;
		private System.Windows.Forms.MenuItem menuItemViewPercent150;
		private System.Windows.Forms.MenuItem menuItemViewPercent200;
		private System.Windows.Forms.MenuItem menuItemViewPercent300;
		private System.Windows.Forms.MenuItem menuItemClose;
		public EnumViewMode cViewMode=EnumViewMode.CONTROL;

        /// <summary>
        /// 리사이즈 드래그 중 그래픽 갱신 억제 플래그
        /// </summary>
        private bool _suspendGraphicUpdate = false;

        public void SuspendGraphicUpdate()
        {
            _suspendGraphicUpdate = true;
        }

        public void ResumeGraphicUpdate()
        {
            _suspendGraphicUpdate = false;
            objectGraphic.SetScreenSize(ClientSize.Width, ClientSize.Height);
            ScrollUpdate();
            Invalidate();
            UpdateNavigatorDisplay();
        }

        public void SetViewMode(EnumViewMode mode)  //20250212 PSU 추가
        {
            cViewMode = mode;
            // ViewMode 변경 시 즉시 커서 변경
            switch (mode)
            {
                case EnumViewMode.ZOOM_IN:
                    this.Cursor = cursorZoomIn;
                    break;
                case EnumViewMode.ZOOM_OUT:
                    this.Cursor = cursorZoomOut;
                    break;
                case EnumViewMode.PANNING:
                    this.Cursor = cursorPanning;
                    break;
                case EnumViewMode.DIGITAL_SELECT:
                    this.Cursor = cursorCross;
                    break;
                default:
                    this.Cursor = Cursors.Arrow;
                    break;
            }
            Invalidate(); //20250312 PSU 추가. control enable 갱신.
        }
		
		Cursor cursorPanning = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphicModule.Cursor.Panning.cur"));
		Cursor cursorZoomIn = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphicModule.Cursor.ZoomIn.cur"));
		Cursor cursorZoomOut = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphicModule.Cursor.ZoomOut.cur"));
        Cursor cursorCross = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GraphicModule.Cursor.cross.cur"));

		public string sSelectedTagName;
		public EnumTagType eSelectedTagType;

        public List<string> SelectedTagNameList = new List<string>(); //25-02-05 hsjeong
        bool bMultiSelectFlag = false;	//25-02-05 hsjeong

		private System.Windows.Forms.ContextMenu contextMenuAnalog;
		private System.Windows.Forms.MenuItem menuItemAnalogDetail;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemAnalogTrend1;
		private System.Windows.Forms.MenuItem menuItemAnalogTrend8;
		private System.Windows.Forms.MenuItem menuItemAnalogTrend24;
		private System.Windows.Forms.MenuItem menuItemAnalogTrend48;
		private System.Windows.Forms.MenuItem menuItemAnalogTrend72;
		private System.Windows.Forms.MenuItem menuItemAnalogTrend30;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItemAnalogDataMin;
		private System.Windows.Forms.MenuItem menuItemAnalogDataHour;
		private System.Windows.Forms.MenuItem menuItemAnalogWeek;
		private System.Windows.Forms.MenuItem menuItemAnalogMonth;
		private System.Windows.Forms.MenuItem menuItemAnalogTagList;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuItemCancel;
		private System.Windows.Forms.MenuItem menuItemAnalogSetValue;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.ContextMenu contextMenuDigital;
		private System.Windows.Forms.MenuItem menuItemDigitalDetail;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItemDigitalTrend1;
		private System.Windows.Forms.MenuItem menuItemDigitalTrend8;
		private System.Windows.Forms.MenuItem menuItemDigitalTrend24;
		private System.Windows.Forms.MenuItem menuItemDigitalTrend48;
		private System.Windows.Forms.MenuItem menuItemDigitalTrend72;
		private System.Windows.Forms.MenuItem menuItemDigitalTrend30;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItemDigitalDataMin;
		private System.Windows.Forms.MenuItem menuItemDigitalDataHour;
		private System.Windows.Forms.MenuItem menuItemDigitalDataDay;
		private System.Windows.Forms.MenuItem menuItemDigitalDataWeek;
		private System.Windows.Forms.MenuItem menuItemDigitalDataMonth;
		private System.Windows.Forms.MenuItem menuItemDigitalTagList;
		private System.Windows.Forms.MenuItem menuItem23;
		private System.Windows.Forms.MenuItem menuItemDigitalSetValue;
		private System.Windows.Forms.MenuItem menuItem25;
		private System.Windows.Forms.MenuItem menuItemScreenPrint;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemAnalogDataDay;
		private System.Windows.Forms.MenuItem menuItemAnalogProtectScan;
		private System.Windows.Forms.MenuItem menuItemAnalogProtectSeparator;
		private System.Windows.Forms.MenuItem menuItem26;
		private System.Windows.Forms.MenuItem menuItemDigitalProtectSeparator;
		private System.Windows.Forms.MenuItem menuItemAnalogProtectControl;
		private System.Windows.Forms.MenuItem menuItemAnalogProtectAlarmEvent;
		private System.Windows.Forms.MenuItem menuItemAnalogProtectAlarmData;
		private System.Windows.Forms.MenuItem menuItemDigitalProtectScan;
		private System.Windows.Forms.MenuItem menuItemDigitalProtectControl;
		private System.Windows.Forms.MenuItem menuItemDigitalProtectAlarmEvent;
        private MenuItem menuItemViewDigitalInputSelect;
        private MenuItem menuItemViewPercent80;
        private MenuItem menuItemViewPercent125;
		private System.Windows.Forms.MenuItem menuItemDigitalProtectAlarmData;
		//public int nSelectedTagPos;

		public FormGraphicChild(string filename)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            SetMenuItemNames(); //20251111 PSU 추가.

            this.Load += async (sender, e) => await this.FormGraphic_Load(sender, e); //20250723 PSU 수정.
            // 마우스 휠 이벤트 등록
            this.MouseWheel += new MouseEventHandler(FormGraphic_MouseWheel);  //20250212 PSU 추가

			objectGraphic = new ObjectRoot();
			sFileName = filename;

			this.VScroll = false;

            objectGraphic.Load(this, sFileName);    // 9.5.2 부터 사용자 정의 제어상자를 사용하기 위해서 Load에 있는 것을 생성자로 옮겼다.
            LanguageManager.EventLanguageChanged += OnLanguageChanged;
        }

        #region SetMenuItemNames
        /// <summary>
        /// Name이 자동으로 추가되지 않아 수동기입.
        /// </summary>
        private void SetMenuItemNames()
        {
            // contextMenuMain
            menuItem6.Name = "menuItem6";
            menuItem10.Name = "menuItem10";
            menuItem18.Name = "menuItem18";
            menuItemModuleReal.Name = "menuItemModuleReal";
            menuItemModuleFitWindow.Name = "menuItemModuleFitWindow";
            menuItemModuleFitWindowXY.Name = "menuItemModuleFitWindowXY";
            menuItemViewControl.Name = "menuItemViewControl";
            menuItemViewZoomIn.Name = "menuItemViewZoomIn";
            menuItemViewZoomOut.Name = "menuItemViewZoomOut";
            menuItemViewPanning.Name = "menuItemViewPanning";
            menuItemViewDigitalInputSelect.Name = "menuItemViewDigitalInputSelect";
            menuItemViewPercent50.Name = "menuItemViewPercent50";
            menuItemViewPercent75.Name = "menuItemViewPercent75";
            menuItemViewPercent80.Name = "menuItemViewPercent80";
            menuItemViewPercent100.Name = "menuItemViewPercent100";
            menuItemViewPercent125.Name = "menuItemViewPercent125";
            menuItemViewPercent150.Name = "menuItemViewPercent150";
            menuItemViewPercent200.Name = "menuItemViewPercent200";
            menuItemViewPercent300.Name = "menuItemViewPercent300";
            menuItemScreenPrint.Name = "menuItemScreenPrint";
            menuItem5.Name = "menuItem5";
            menuItemClose.Name = "menuItemClose";

            // contextMenuAnalog
            menuItemAnalogDetail.Name = "menuItemAnalogDetail";
            menuItem2.Name = "menuItem2";
            menuItemAnalogTrend1.Name = "menuItemAnalogTrend1";
            menuItemAnalogTrend8.Name = "menuItemAnalogTrend8";
            menuItemAnalogTrend24.Name = "menuItemAnalogTrend24";
            menuItemAnalogTrend48.Name = "menuItemAnalogTrend48";
            menuItemAnalogTrend72.Name = "menuItemAnalogTrend72";
            menuItemAnalogTrend30.Name = "menuItemAnalogTrend30";
            menuItem11.Name = "menuItem11";
            menuItemAnalogDataMin.Name = "menuItemAnalogDataMin";
            menuItemAnalogDataHour.Name = "menuItemAnalogDataHour";
            menuItemAnalogDataDay.Name = "menuItemAnalogDataDay";
            menuItemAnalogWeek.Name = "menuItemAnalogWeek";
            menuItemAnalogMonth.Name = "menuItemAnalogMonth";
            menuItemAnalogTagList.Name = "menuItemAnalogTagList";
            menuItem19.Name = "menuItem19";
            menuItemAnalogSetValue.Name = "menuItemAnalogSetValue";
            menuItem3.Name = "menuItem3";
            menuItemCancel.Name = "menuItemCancel";
            menuItemAnalogProtectSeparator.Name = "menuItemAnalogProtectSeparator";
            menuItemAnalogProtectScan.Name = "menuItemAnalogProtectScan";
            menuItemAnalogProtectControl.Name = "menuItemAnalogProtectControl";
            menuItemAnalogProtectAlarmEvent.Name = "menuItemAnalogProtectAlarmEvent";
            menuItemAnalogProtectAlarmData.Name = "menuItemAnalogProtectAlarmData";
            menuItem26.Name = "menuItem26";

            // contextMenuDigital
            menuItemDigitalDetail.Name = "menuItemDigitalDetail";
            menuItem4.Name = "menuItem4";
            menuItemDigitalTrend1.Name = "menuItemDigitalTrend1";
            menuItemDigitalTrend8.Name = "menuItemDigitalTrend8";
            menuItemDigitalTrend24.Name = "menuItemDigitalTrend24";
            menuItemDigitalTrend48.Name = "menuItemDigitalTrend48";
            menuItemDigitalTrend72.Name = "menuItemDigitalTrend72";
            menuItemDigitalTrend30.Name = "menuItemDigitalTrend30";
            menuItem14.Name = "menuItem14";
            menuItemDigitalDataMin.Name = "menuItemDigitalDataMin";
            menuItemDigitalDataHour.Name = "menuItemDigitalDataHour";
            menuItemDigitalDataDay.Name = "menuItemDigitalDataDay";
            menuItemDigitalDataWeek.Name = "menuItemDigitalDataWeek";
            menuItemDigitalDataMonth.Name = "menuItemDigitalDataMonth";
            menuItemDigitalTagList.Name = "menuItemDigitalTagList";
            menuItem23.Name = "menuItem23";
            menuItemDigitalSetValue.Name = "menuItemDigitalSetValue";
            menuItem25.Name = "menuItem25";
            menuItemDigitalProtectSeparator.Name = "menuItemDigitalProtectSeparator";
            menuItemDigitalProtectScan.Name = "menuItemDigitalProtectScan";
            menuItemDigitalProtectControl.Name = "menuItemDigitalProtectControl";
            menuItemDigitalProtectAlarmEvent.Name = "menuItemDigitalProtectAlarmEvent";
            menuItemDigitalProtectAlarmData.Name = "menuItemDigitalProtectAlarmData";
        }
        #endregion

        #region Globalization
        /// <summary>
        /// 언어 변경 이벤트 핸들러
        /// </summary>
        /// <param name="newLanguageCode">새로운 언어 코드</param>
        private void OnLanguageChanged(string newLanguageCode)
        {
            // 언어가 변경되면 텍스트 업데이트
            var culture = new CultureInfo(newLanguageCode);
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            ComponentResourceManager resources = new ComponentResourceManager(this.GetType());

            this.SuspendLayout();
            // 특정 ContextMenuStrip들만 처리
            ApplyResourcesToContextMenu(resources, contextMenuDigital, culture);
            ApplyResourcesToContextMenu(resources, contextMenuAnalog, culture);
            ApplyResourcesToContextMenu(resources, contextMenuMain, culture);
            this.ResumeLayout(true);
            this.Refresh();
        }

        private void ApplyResourcesToContextMenu(ComponentResourceManager res, System.Windows.Forms.ContextMenu contextMenu, CultureInfo culture)
        {
            if (contextMenu == null) return;

            // 모든 MenuItem에 리소스 적용
            foreach (MenuItem item in contextMenu.MenuItems)
                ApplyResourcesToMenuItem(res, item, culture);
        }

        private void ApplyResourcesToMenuItem(ComponentResourceManager res, MenuItem item, CultureInfo culture)
        {
            // 현재 MenuItem에 리소스 적용
            res.ApplyResources(item, item.Name, culture); //Name이 지정되어 있어야 한다.

            // 하위 MenuItem 처리
            foreach (MenuItem child in item.MenuItems)
                ApplyResourcesToMenuItem(res, child, culture);
        }

        #endregion Globalization

        /// <summary>
        /// 툴바에서 파일을 바꿀 때 사용할 수 있도록 같은 폼에서 다시 부를 수 있도록 추가하였다. (9.5.2)
        /// </summary>
        /// <param name="filename"></param>
        public void LoadFile(string filename)
        {
            objectGraphic.Close();
            objectGraphic = new ObjectRoot();
            sFileName = filename;
            objectGraphic.Load(this, sFileName);    // 9.5.2 부터 사용자 정의 제어상자를 사용하기 위해서 Load에 있는 것을 생성자로 옮겼다. 
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

            objectGraphic.Dispose();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphicChild));
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuMain = new System.Windows.Forms.ContextMenu();
            this.menuItemModuleReal = new System.Windows.Forms.MenuItem();
            this.menuItemModuleFitWindow = new System.Windows.Forms.MenuItem();
            this.menuItemModuleFitWindowXY = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemViewControl = new System.Windows.Forms.MenuItem();
            this.menuItemViewZoomIn = new System.Windows.Forms.MenuItem();
            this.menuItemViewZoomOut = new System.Windows.Forms.MenuItem();
            this.menuItemViewPanning = new System.Windows.Forms.MenuItem();
            this.menuItemViewDigitalInputSelect = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent50 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent75 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent80 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent100 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent125 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent150 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent200 = new System.Windows.Forms.MenuItem();
            this.menuItemViewPercent300 = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItemScreenPrint = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemClose = new System.Windows.Forms.MenuItem();
            this.contextMenuAnalog = new System.Windows.Forms.ContextMenu();
            this.menuItemAnalogDetail = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTrend1 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTrend8 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTrend24 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTrend48 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTrend72 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTrend30 = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogDataMin = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogDataHour = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogDataDay = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogWeek = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogMonth = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogTagList = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogSetValue = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogProtectScan = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogProtectControl = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogProtectAlarmEvent = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogProtectAlarmData = new System.Windows.Forms.MenuItem();
            this.menuItemAnalogProtectSeparator = new System.Windows.Forms.MenuItem();
            this.menuItemCancel = new System.Windows.Forms.MenuItem();
            this.contextMenuDigital = new System.Windows.Forms.ContextMenu();
            this.menuItemDigitalDetail = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTrend1 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTrend8 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTrend24 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTrend48 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTrend72 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTrend30 = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalDataMin = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalDataHour = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalDataDay = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalDataWeek = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalDataMonth = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalTagList = new System.Windows.Forms.MenuItem();
            this.menuItem23 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalSetValue = new System.Windows.Forms.MenuItem();
            this.menuItem25 = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalProtectScan = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalProtectControl = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalProtectAlarmEvent = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalProtectAlarmData = new System.Windows.Forms.MenuItem();
            this.menuItemDigitalProtectSeparator = new System.Windows.Forms.MenuItem();
            this.menuItem26 = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemModuleReal,
            this.menuItemModuleFitWindow,
            this.menuItemModuleFitWindowXY,
            this.menuItem6,
            this.menuItemViewControl,
            this.menuItemViewZoomIn,
            this.menuItemViewZoomOut,
            this.menuItemViewPanning,
            this.menuItemViewDigitalInputSelect,
            this.menuItem10,
            this.menuItemViewPercent50,
            this.menuItemViewPercent75,
            this.menuItemViewPercent80,
            this.menuItemViewPercent100,
            this.menuItemViewPercent125,
            this.menuItemViewPercent150,
            this.menuItemViewPercent200,
            this.menuItemViewPercent300,
            this.menuItem18,
            this.menuItemScreenPrint,
            this.menuItem5,
            this.menuItemClose});
            resources.ApplyResources(this.contextMenuMain, "contextMenuMain");
            this.contextMenuMain.Popup += new System.EventHandler(this.contextMenuMain_Popup);
            // 
            // menuItemModuleReal
            // 
            resources.ApplyResources(this.menuItemModuleReal, "menuItemModuleReal");
            this.menuItemModuleReal.Index = 0;
            this.menuItemModuleReal.RadioCheck = true;
            this.menuItemModuleReal.Click += new System.EventHandler(this.menuItemModuleReal_Click);
            // 
            // menuItemModuleFitWindow
            // 
            resources.ApplyResources(this.menuItemModuleFitWindow, "menuItemModuleFitWindow");
            this.menuItemModuleFitWindow.Index = 1;
            this.menuItemModuleFitWindow.RadioCheck = true;
            this.menuItemModuleFitWindow.Click += new System.EventHandler(this.menuItemModuleFitWindow_Click);
            // 
            // menuItemModuleFitWindowXY
            // 
            resources.ApplyResources(this.menuItemModuleFitWindowXY, "menuItemModuleFitWindowXY");
            this.menuItemModuleFitWindowXY.Index = 2;
            this.menuItemModuleFitWindowXY.RadioCheck = true;
            this.menuItemModuleFitWindowXY.Click += new System.EventHandler(this.menuItemModuleFitWindowXY_Click);
            // 
            // menuItem6
            // 
            resources.ApplyResources(this.menuItem6, "menuItem6");
            this.menuItem6.Index = 3;
            // 
            // menuItemViewControl
            // 
            resources.ApplyResources(this.menuItemViewControl, "menuItemViewControl");
            this.menuItemViewControl.Index = 4;
            this.menuItemViewControl.RadioCheck = true;
            this.menuItemViewControl.Click += new System.EventHandler(this.menuItemViewControl_Click);
            // 
            // menuItemViewZoomIn
            // 
            resources.ApplyResources(this.menuItemViewZoomIn, "menuItemViewZoomIn");
            this.menuItemViewZoomIn.Index = 5;
            this.menuItemViewZoomIn.RadioCheck = true;
            this.menuItemViewZoomIn.Click += new System.EventHandler(this.menuItemViewZoomIn_Click);
            // 
            // menuItemViewZoomOut
            // 
            resources.ApplyResources(this.menuItemViewZoomOut, "menuItemViewZoomOut");
            this.menuItemViewZoomOut.Index = 6;
            this.menuItemViewZoomOut.RadioCheck = true;
            this.menuItemViewZoomOut.Click += new System.EventHandler(this.menuItemViewZoomOut_Click);
            // 
            // menuItemViewPanning
            // 
            resources.ApplyResources(this.menuItemViewPanning, "menuItemViewPanning");
            this.menuItemViewPanning.Index = 7;
            this.menuItemViewPanning.RadioCheck = true;
            this.menuItemViewPanning.Click += new System.EventHandler(this.menuItemViewPanning_Click);
            // 
            // menuItemViewDigitalInputSelect
            // 
            resources.ApplyResources(this.menuItemViewDigitalInputSelect, "menuItemViewDigitalInputSelect");
            this.menuItemViewDigitalInputSelect.Index = 8;
            this.menuItemViewDigitalInputSelect.Click += new System.EventHandler(this.menuItemViewDigitalInput_Click);
            // 
            // menuItem10
            // 
            resources.ApplyResources(this.menuItem10, "menuItem10");
            this.menuItem10.Index = 9;
            // 
            // menuItemViewPercent50
            // 
            resources.ApplyResources(this.menuItemViewPercent50, "menuItemViewPercent50");
            this.menuItemViewPercent50.Index = 10;
            this.menuItemViewPercent50.RadioCheck = true;
            this.menuItemViewPercent50.Click += new System.EventHandler(this.menuItemViewPercent50_Click);
            // 
            // menuItemViewPercent75
            // 
            resources.ApplyResources(this.menuItemViewPercent75, "menuItemViewPercent75");
            this.menuItemViewPercent75.Index = 11;
            this.menuItemViewPercent75.RadioCheck = true;
            this.menuItemViewPercent75.Click += new System.EventHandler(this.menuItemViewPercent75_Click);
            // 
            // menuItemViewPercent80
            // 
            resources.ApplyResources(this.menuItemViewPercent80, "menuItemViewPercent80");
            this.menuItemViewPercent80.Index = 12;
            this.menuItemViewPercent80.RadioCheck = true;
            this.menuItemViewPercent80.Click += new System.EventHandler(this.menuItemViewPercent80_Click);
            // 
            // menuItemViewPercent100
            // 
            resources.ApplyResources(this.menuItemViewPercent100, "menuItemViewPercent100");
            this.menuItemViewPercent100.Index = 13;
            this.menuItemViewPercent100.RadioCheck = true;
            this.menuItemViewPercent100.Click += new System.EventHandler(this.menuItemViewPercent100_Click);
            // 
            // menuItemViewPercent125
            // 
            resources.ApplyResources(this.menuItemViewPercent125, "menuItemViewPercent125");
            this.menuItemViewPercent125.Index = 14;
            this.menuItemViewPercent125.RadioCheck = true;
            this.menuItemViewPercent125.Click += new System.EventHandler(this.menuItemViewPercent125_Click);
            // 
            // menuItemViewPercent150
            // 
            resources.ApplyResources(this.menuItemViewPercent150, "menuItemViewPercent150");
            this.menuItemViewPercent150.Index = 15;
            this.menuItemViewPercent150.RadioCheck = true;
            this.menuItemViewPercent150.Click += new System.EventHandler(this.menuItemViewPercent150_Click);
            // 
            // menuItemViewPercent200
            // 
            resources.ApplyResources(this.menuItemViewPercent200, "menuItemViewPercent200");
            this.menuItemViewPercent200.Index = 16;
            this.menuItemViewPercent200.RadioCheck = true;
            this.menuItemViewPercent200.Click += new System.EventHandler(this.menuItemViewPercent200_Click);
            // 
            // menuItemViewPercent300
            // 
            resources.ApplyResources(this.menuItemViewPercent300, "menuItemViewPercent300");
            this.menuItemViewPercent300.Index = 17;
            this.menuItemViewPercent300.RadioCheck = true;
            this.menuItemViewPercent300.Click += new System.EventHandler(this.menuItemViewPercent300_Click);
            // 
            // menuItem18
            // 
            resources.ApplyResources(this.menuItem18, "menuItem18");
            this.menuItem18.Index = 18;
            // 
            // menuItemScreenPrint
            // 
            resources.ApplyResources(this.menuItemScreenPrint, "menuItemScreenPrint");
            this.menuItemScreenPrint.Index = 19;
            this.menuItemScreenPrint.Click += new System.EventHandler(this.menuItemScreenPrint_Click);
            // 
            // menuItem5
            // 
            resources.ApplyResources(this.menuItem5, "menuItem5");
            this.menuItem5.Index = 20;
            // 
            // menuItemClose
            // 
            resources.ApplyResources(this.menuItemClose, "menuItemClose");
            this.menuItemClose.Index = 21;
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // contextMenuAnalog
            // 
            this.contextMenuAnalog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAnalogDetail,
            this.menuItem2,
            this.menuItemAnalogTrend1,
            this.menuItemAnalogTrend8,
            this.menuItemAnalogTrend24,
            this.menuItemAnalogTrend48,
            this.menuItemAnalogTrend72,
            this.menuItemAnalogTrend30,
            this.menuItem11,
            this.menuItemAnalogDataMin,
            this.menuItemAnalogDataHour,
            this.menuItemAnalogDataDay,
            this.menuItemAnalogWeek,
            this.menuItemAnalogMonth,
            this.menuItemAnalogTagList,
            this.menuItem19,
            this.menuItemAnalogSetValue,
            this.menuItem3,
            this.menuItemAnalogProtectScan,
            this.menuItemAnalogProtectControl,
            this.menuItemAnalogProtectAlarmEvent,
            this.menuItemAnalogProtectAlarmData,
            this.menuItemAnalogProtectSeparator,
            this.menuItemCancel});
            resources.ApplyResources(this.contextMenuAnalog, "contextMenuAnalog");
            this.contextMenuAnalog.Popup += new System.EventHandler(this.contextMenuAnalog_Popup);
            // 
            // menuItemAnalogDetail
            // 
            resources.ApplyResources(this.menuItemAnalogDetail, "menuItemAnalogDetail");
            this.menuItemAnalogDetail.Index = 0;
            this.menuItemAnalogDetail.Click += new System.EventHandler(this.menuItemAnalogDetail_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Index = 1;
            // 
            // menuItemAnalogTrend1
            // 
            resources.ApplyResources(this.menuItemAnalogTrend1, "menuItemAnalogTrend1");
            this.menuItemAnalogTrend1.Index = 2;
            this.menuItemAnalogTrend1.Click += new System.EventHandler(this.menuItemAnalogTrend1_Click);
            // 
            // menuItemAnalogTrend8
            // 
            resources.ApplyResources(this.menuItemAnalogTrend8, "menuItemAnalogTrend8");
            this.menuItemAnalogTrend8.Index = 3;
            this.menuItemAnalogTrend8.Click += new System.EventHandler(this.menuItemAnalogTrend8_Click);
            // 
            // menuItemAnalogTrend24
            // 
            resources.ApplyResources(this.menuItemAnalogTrend24, "menuItemAnalogTrend24");
            this.menuItemAnalogTrend24.Index = 4;
            this.menuItemAnalogTrend24.Click += new System.EventHandler(this.menuItemAnalogTrend24_Click);
            // 
            // menuItemAnalogTrend48
            // 
            resources.ApplyResources(this.menuItemAnalogTrend48, "menuItemAnalogTrend48");
            this.menuItemAnalogTrend48.Index = 5;
            this.menuItemAnalogTrend48.Click += new System.EventHandler(this.menuItemAnalogTrend48_Click);
            // 
            // menuItemAnalogTrend72
            // 
            resources.ApplyResources(this.menuItemAnalogTrend72, "menuItemAnalogTrend72");
            this.menuItemAnalogTrend72.Index = 6;
            this.menuItemAnalogTrend72.Click += new System.EventHandler(this.menuItemAnalogTrend72_Click);
            // 
            // menuItemAnalogTrend30
            // 
            resources.ApplyResources(this.menuItemAnalogTrend30, "menuItemAnalogTrend30");
            this.menuItemAnalogTrend30.Index = 7;
            this.menuItemAnalogTrend30.Click += new System.EventHandler(this.menuItemAnalogTrend30_Click);
            // 
            // menuItem11
            // 
            resources.ApplyResources(this.menuItem11, "menuItem11");
            this.menuItem11.Index = 8;
            // 
            // menuItemAnalogDataMin
            // 
            resources.ApplyResources(this.menuItemAnalogDataMin, "menuItemAnalogDataMin");
            this.menuItemAnalogDataMin.Index = 9;
            this.menuItemAnalogDataMin.Click += new System.EventHandler(this.menuItemAnalogDataMin_Click);
            // 
            // menuItemAnalogDataHour
            // 
            resources.ApplyResources(this.menuItemAnalogDataHour, "menuItemAnalogDataHour");
            this.menuItemAnalogDataHour.Index = 10;
            this.menuItemAnalogDataHour.Click += new System.EventHandler(this.menuItemAnalogDataHour_Click);
            // 
            // menuItemAnalogDataDay
            // 
            resources.ApplyResources(this.menuItemAnalogDataDay, "menuItemAnalogDataDay");
            this.menuItemAnalogDataDay.Index = 11;
            this.menuItemAnalogDataDay.Click += new System.EventHandler(this.menuItemAnalogDataDay_Click);
            // 
            // menuItemAnalogWeek
            // 
            resources.ApplyResources(this.menuItemAnalogWeek, "menuItemAnalogWeek");
            this.menuItemAnalogWeek.Index = 12;
            this.menuItemAnalogWeek.Click += new System.EventHandler(this.menuItemAnalogWeek_Click);
            // 
            // menuItemAnalogMonth
            // 
            resources.ApplyResources(this.menuItemAnalogMonth, "menuItemAnalogMonth");
            this.menuItemAnalogMonth.Index = 13;
            this.menuItemAnalogMonth.Click += new System.EventHandler(this.menuItemAnalogMonth_Click);
            // 
            // menuItemAnalogTagList
            // 
            resources.ApplyResources(this.menuItemAnalogTagList, "menuItemAnalogTagList");
            this.menuItemAnalogTagList.Index = 14;
            this.menuItemAnalogTagList.Click += new System.EventHandler(this.menuItemAnalogTagList_Click);
            // 
            // menuItem19
            // 
            resources.ApplyResources(this.menuItem19, "menuItem19");
            this.menuItem19.Index = 15;
            // 
            // menuItemAnalogSetValue
            // 
            resources.ApplyResources(this.menuItemAnalogSetValue, "menuItemAnalogSetValue");
            this.menuItemAnalogSetValue.Index = 16;
            this.menuItemAnalogSetValue.Click += new System.EventHandler(this.menuItemAnalogSetValue_Click);
            // 
            // menuItem3
            // 
            resources.ApplyResources(this.menuItem3, "menuItem3");
            this.menuItem3.Index = 17;
            // 
            // menuItemAnalogProtectScan
            // 
            resources.ApplyResources(this.menuItemAnalogProtectScan, "menuItemAnalogProtectScan");
            this.menuItemAnalogProtectScan.Index = 18;
            this.menuItemAnalogProtectScan.Click += new System.EventHandler(this.menuItemAnalogProtectScan_Click);
            // 
            // menuItemAnalogProtectControl
            // 
            resources.ApplyResources(this.menuItemAnalogProtectControl, "menuItemAnalogProtectControl");
            this.menuItemAnalogProtectControl.Index = 19;
            this.menuItemAnalogProtectControl.Click += new System.EventHandler(this.menuItemAnalogProtectControl_Click);
            // 
            // menuItemAnalogProtectAlarmEvent
            // 
            resources.ApplyResources(this.menuItemAnalogProtectAlarmEvent, "menuItemAnalogProtectAlarmEvent");
            this.menuItemAnalogProtectAlarmEvent.Index = 20;
            this.menuItemAnalogProtectAlarmEvent.Click += new System.EventHandler(this.menuItemAnalogProtectAlarmEvent_Click);
            // 
            // menuItemAnalogProtectAlarmData
            // 
            resources.ApplyResources(this.menuItemAnalogProtectAlarmData, "menuItemAnalogProtectAlarmData");
            this.menuItemAnalogProtectAlarmData.Index = 21;
            this.menuItemAnalogProtectAlarmData.Click += new System.EventHandler(this.menuItemAnalogProtectAlarmData_Click);
            // 
            // menuItemAnalogProtectSeparator
            // 
            resources.ApplyResources(this.menuItemAnalogProtectSeparator, "menuItemAnalogProtectSeparator");
            this.menuItemAnalogProtectSeparator.Index = 22;
            // 
            // menuItemCancel
            // 
            resources.ApplyResources(this.menuItemCancel, "menuItemCancel");
            this.menuItemCancel.Index = 23;
            // 
            // contextMenuDigital
            // 
            this.contextMenuDigital.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDigitalDetail,
            this.menuItem4,
            this.menuItemDigitalTrend1,
            this.menuItemDigitalTrend8,
            this.menuItemDigitalTrend24,
            this.menuItemDigitalTrend48,
            this.menuItemDigitalTrend72,
            this.menuItemDigitalTrend30,
            this.menuItem14,
            this.menuItemDigitalDataMin,
            this.menuItemDigitalDataHour,
            this.menuItemDigitalDataDay,
            this.menuItemDigitalDataWeek,
            this.menuItemDigitalDataMonth,
            this.menuItemDigitalTagList,
            this.menuItem23,
            this.menuItemDigitalSetValue,
            this.menuItem25,
            this.menuItemDigitalProtectScan,
            this.menuItemDigitalProtectControl,
            this.menuItemDigitalProtectAlarmEvent,
            this.menuItemDigitalProtectAlarmData,
            this.menuItemDigitalProtectSeparator,
            this.menuItem26});
            resources.ApplyResources(this.contextMenuDigital, "contextMenuDigital");
            this.contextMenuDigital.Popup += new System.EventHandler(this.contextMenuDigital_Popup);
            // 
            // menuItemDigitalDetail
            // 
            resources.ApplyResources(this.menuItemDigitalDetail, "menuItemDigitalDetail");
            this.menuItemDigitalDetail.Index = 0;
            this.menuItemDigitalDetail.Click += new System.EventHandler(this.menuItemDigitalDetail_Click);
            // 
            // menuItem4
            // 
            resources.ApplyResources(this.menuItem4, "menuItem4");
            this.menuItem4.Index = 1;
            // 
            // menuItemDigitalTrend1
            // 
            resources.ApplyResources(this.menuItemDigitalTrend1, "menuItemDigitalTrend1");
            this.menuItemDigitalTrend1.Index = 2;
            this.menuItemDigitalTrend1.Click += new System.EventHandler(this.menuItemDigitalTrend1_Click);
            // 
            // menuItemDigitalTrend8
            // 
            resources.ApplyResources(this.menuItemDigitalTrend8, "menuItemDigitalTrend8");
            this.menuItemDigitalTrend8.Index = 3;
            this.menuItemDigitalTrend8.Click += new System.EventHandler(this.menuItemDigitalTrend8_Click);
            // 
            // menuItemDigitalTrend24
            // 
            resources.ApplyResources(this.menuItemDigitalTrend24, "menuItemDigitalTrend24");
            this.menuItemDigitalTrend24.Index = 4;
            this.menuItemDigitalTrend24.Click += new System.EventHandler(this.menuItemDigitalTrend24_Click);
            // 
            // menuItemDigitalTrend48
            // 
            resources.ApplyResources(this.menuItemDigitalTrend48, "menuItemDigitalTrend48");
            this.menuItemDigitalTrend48.Index = 5;
            this.menuItemDigitalTrend48.Click += new System.EventHandler(this.menuItemDigitalTrend48_Click);
            // 
            // menuItemDigitalTrend72
            // 
            resources.ApplyResources(this.menuItemDigitalTrend72, "menuItemDigitalTrend72");
            this.menuItemDigitalTrend72.Index = 6;
            this.menuItemDigitalTrend72.Click += new System.EventHandler(this.menuItemDigitalTrend72_Click);
            // 
            // menuItemDigitalTrend30
            // 
            resources.ApplyResources(this.menuItemDigitalTrend30, "menuItemDigitalTrend30");
            this.menuItemDigitalTrend30.Index = 7;
            this.menuItemDigitalTrend30.Click += new System.EventHandler(this.menuItemDigitalTrend30_Click);
            // 
            // menuItem14
            // 
            resources.ApplyResources(this.menuItem14, "menuItem14");
            this.menuItem14.Index = 8;
            // 
            // menuItemDigitalDataMin
            // 
            resources.ApplyResources(this.menuItemDigitalDataMin, "menuItemDigitalDataMin");
            this.menuItemDigitalDataMin.Index = 9;
            this.menuItemDigitalDataMin.Click += new System.EventHandler(this.menuItemDigitalDataMin_Click);
            // 
            // menuItemDigitalDataHour
            // 
            resources.ApplyResources(this.menuItemDigitalDataHour, "menuItemDigitalDataHour");
            this.menuItemDigitalDataHour.Index = 10;
            this.menuItemDigitalDataHour.Click += new System.EventHandler(this.menuItemDigitalDataHour_Click);
            // 
            // menuItemDigitalDataDay
            // 
            resources.ApplyResources(this.menuItemDigitalDataDay, "menuItemDigitalDataDay");
            this.menuItemDigitalDataDay.Index = 11;
            this.menuItemDigitalDataDay.Click += new System.EventHandler(this.menuItemDigitalDataDay_Click);
            // 
            // menuItemDigitalDataWeek
            // 
            resources.ApplyResources(this.menuItemDigitalDataWeek, "menuItemDigitalDataWeek");
            this.menuItemDigitalDataWeek.Index = 12;
            this.menuItemDigitalDataWeek.Click += new System.EventHandler(this.menuItemDigitalDataWeek_Click);
            // 
            // menuItemDigitalDataMonth
            // 
            resources.ApplyResources(this.menuItemDigitalDataMonth, "menuItemDigitalDataMonth");
            this.menuItemDigitalDataMonth.Index = 13;
            this.menuItemDigitalDataMonth.Click += new System.EventHandler(this.menuItemDigitalDataMonth_Click);
            // 
            // menuItemDigitalTagList
            // 
            resources.ApplyResources(this.menuItemDigitalTagList, "menuItemDigitalTagList");
            this.menuItemDigitalTagList.Index = 14;
            this.menuItemDigitalTagList.Click += new System.EventHandler(this.menuItemDigitalTagList_Click);
            // 
            // menuItem23
            // 
            resources.ApplyResources(this.menuItem23, "menuItem23");
            this.menuItem23.Index = 15;
            // 
            // menuItemDigitalSetValue
            // 
            resources.ApplyResources(this.menuItemDigitalSetValue, "menuItemDigitalSetValue");
            this.menuItemDigitalSetValue.Index = 16;
            this.menuItemDigitalSetValue.Click += new System.EventHandler(this.menuItemDigitalSetValue_Click);
            // 
            // menuItem25
            // 
            resources.ApplyResources(this.menuItem25, "menuItem25");
            this.menuItem25.Index = 17;
            // 
            // menuItemDigitalProtectScan
            // 
            resources.ApplyResources(this.menuItemDigitalProtectScan, "menuItemDigitalProtectScan");
            this.menuItemDigitalProtectScan.Index = 18;
            this.menuItemDigitalProtectScan.Click += new System.EventHandler(this.menuItemDigitalProtectScan_Click);
            // 
            // menuItemDigitalProtectControl
            // 
            resources.ApplyResources(this.menuItemDigitalProtectControl, "menuItemDigitalProtectControl");
            this.menuItemDigitalProtectControl.Index = 19;
            this.menuItemDigitalProtectControl.Click += new System.EventHandler(this.menuItemDigitalProtectControl_Click);
            // 
            // menuItemDigitalProtectAlarmEvent
            // 
            resources.ApplyResources(this.menuItemDigitalProtectAlarmEvent, "menuItemDigitalProtectAlarmEvent");
            this.menuItemDigitalProtectAlarmEvent.Index = 20;
            this.menuItemDigitalProtectAlarmEvent.Click += new System.EventHandler(this.menuItemDigitalProtectAlarmEvent_Click);
            // 
            // menuItemDigitalProtectAlarmData
            // 
            resources.ApplyResources(this.menuItemDigitalProtectAlarmData, "menuItemDigitalProtectAlarmData");
            this.menuItemDigitalProtectAlarmData.Index = 21;
            this.menuItemDigitalProtectAlarmData.Click += new System.EventHandler(this.menuItemDigitalProtectAlarmData_Click);
            // 
            // menuItemDigitalProtectSeparator
            // 
            resources.ApplyResources(this.menuItemDigitalProtectSeparator, "menuItemDigitalProtectSeparator");
            this.menuItemDigitalProtectSeparator.Index = 22;
            // 
            // menuItem26
            // 
            resources.ApplyResources(this.menuItem26, "menuItem26");
            this.menuItem26.Index = 23;
            // 
            // FormGraphicChild
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormGraphicChild";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormGraphicChild_Closing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormGraphicChild_FormClosed);
            this.SizeChanged += new System.EventHandler(this.FormGraphic_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormGraphic_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormGraphic_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormGraphic_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormGraphic_MouseUp);
            this.ResumeLayout(false);

		}
		#endregion

        async Task PlayScriptWhenModStartEnd(string when)
        {
            ScriptClass control;

            if (String.Compare(when, "ModStart", true) == 0)
                control = objectGraphic.scriptModuleStart;
            else if (String.Compare(when, "ModEnd", true) == 0)
                control = objectGraphic.scriptModuleEnd;
            else
                control = null;

            if (control == null) return;

            await control.RunAsync(this, null);

            if (control.IsError())
            {
                string message;
                message = control.GetError();
                MessageBox.Show(message, when + " Script");
            }
        }

        private OnEventTimer timerHandler;

        private async Task FormGraphic_Load(object sender, System.EventArgs e)
		{
            // 모듈 시작 프로그램을 시작한다.
            await PlayScriptWhenModStartEnd("ModStart");

			objectGraphic.SetScreenSize(ClientSize.Width, ClientSize.Height);

			SharedViewMain.EventListTagChanged += new SharedViewMain.OnEventTagChanged(FormGraphic_EventTag);

            timerHandler = async () => await FormGraphic_EventTimer();
            SharedViewMain.EventListTimer += timerHandler;

            SetGraphicWindowTitle();

			label1.Left = -label1.Width;

            InitializeScrollBars(); //20250317 PSU 추가

			ScrollUpdate();

            // 2007.6.14 OnPaintBitmap 대신 사용할 수 있다. 이 함수로 화면 떨림을 예방할 수 있다. OnPaintBitmap(Memory dc)을 사용하면 화면위에 투명한 윈도우가 오면 화면 떨림이 발생한다.
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
		}

        private VScrollBar vScrollBar;
        private HScrollBar hScrollBar;
        private Point scrollPosition = new Point(0, 0);

        private void InitializeScrollBars()  //20250317 PSU 추가
        {
            // AutoScroll 비활성화
            this.AutoScroll = false;

            // 스크롤바 생성
            vScrollBar = new VScrollBar();
            hScrollBar = new HScrollBar();

            // 스크롤바 속성 설정
            vScrollBar.Dock = DockStyle.Right;
            hScrollBar.Dock = DockStyle.Bottom;

            // 스크롤바 이벤트 연결
            vScrollBar.Scroll += new ScrollEventHandler(VScrollBar_Scroll);
            hScrollBar.Scroll += new ScrollEventHandler(HScrollBar_Scroll);

            // 폼에 스크롤바 추가
            this.Controls.Add(vScrollBar);
            this.Controls.Add(hScrollBar);

            // Z-order를 명시적으로 설정 - 스크롤바를 가장 위로 가져옴
            vScrollBar.BringToFront();
            hScrollBar.BringToFront();
        }

        //20250317 PSU 추가
        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            scrollPosition.Y = e.NewValue;
            Invalidate(); // 화면 갱신
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            scrollPosition.X = e.NewValue;
            Invalidate(); // 화면 갱신

        }

        private void FormGraphic_EventTag(TagPublicClass tagevent)
		{
			objectGraphic.EventTag(this, tagevent);
		}

        async Task CheckModuleAlwaysScript()
        {
            ScriptClass script = objectGraphic.scriptModuleAlways;

            if (script == null) return;

            await script.RunAsync(this, null);

            if (script.IsError())
            {
                string message;
                string title;
                message = script.GetError();
                if (NetTools.Tools.IsLangKorean())
                {
                    title = String.Format("{0}의 Module Script Always에서 오류", sFileName);
                }
                else
                {
                    title = String.Format("Module Script Always Error at {0}", sFileName);
                }
                MessageDisplay.Show("{0} {1}", title, message);
                return;
            }
        }

        private async Task FormGraphic_EventTimer()
        {
            if (this.IsDisposed || !this.IsHandleCreated)
            {
                return;
            }

            try
            {
               await CheckModuleAlwaysScript();
               await objectGraphic.EventTimerAsync(this);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private void FormGraphic_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(this.WindowState == FormWindowState.Minimized)	return;
			if(ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0)	return;

			//int tick = Environment.TickCount;
			//objectGraphic.Display(e.Graphics, this.ClientRectangle, e.ClipRectangle, this.AutoScrollPosition);
            objectGraphic.Display(e.Graphics, this.ClientRectangle, e.ClipRectangle,
                                 new Point(-scrollPosition.X, -scrollPosition.Y)); //20250317 PSU 수정.

			//string text = String.Format("Tick={0}  ", Environment.TickCount-tick);
			//SharedData.formMain.Text = text;
			//e.Graphics.DrawString(text, this.Font, Brushes.Red, 0, 0);
			//SizeF size = e.Graphics.MeasureString(text, this.Font);
			//Rectangle r = new Rectangle(0, 0, (int)size.Width, (int)size.Height);
			//this.Invalidate(r);

            //25-02-05 Digital Input Select Mode 추가
            if (bMultiSelectFlag)
            {
                DrawMutiSelectZone(e.Graphics);
            }
		}

   		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 

		} 

		public void GetNavigatorSize(ref int x1, ref int y1, ref int x2, ref int y2)
		{
			Rectangle rect = ClientRectangle;

			x1 = 0;
			y1 = 0;
			x2 = rect.Right-1;
			y2 = rect.Bottom-1;

			GetPicturePosition(ref x1, ref y1);
			GetPicturePosition(ref x2, ref y2);
		}

		void UpdateNavigatorDisplay()
		{
			int x1=0, y1=0, x2=0, y2=0;

			GetNavigatorSize(ref x1, ref y1, ref x2, ref y2);
			ViewNavigator.UpdateNavigatorRectZone(x1, y1, x2, y2);
		}

		private void FormGraphic_SizeChanged(object sender, System.EventArgs e)
		{
			if(this.IsDisposed)	return;
			if(_suspendGraphicUpdate) return;

			objectGraphic.SetScreenSize(ClientSize.Width, ClientSize.Height);
			ScrollUpdate();
			Invalidate();

			UpdateNavigatorDisplay();
		}


        public int nMouseSleep = 0;    //  밀리초
        public TimeOutMiliSecClass timeoutMouseSleep = null;

        bool IsMouseSleep()
        {
            if (timeoutMouseSleep == null) return false;

            if (!timeoutMouseSleep.IsTimeOut(nMouseSleep)) return true;

            timeoutMouseSleep = null;

            return false;
        }

        #region mouse events

        int nCaptureX, nCaptureY;               // mouse capture 할 당시의 커서 위치값.
        int nCaptureScrollX, nCaptureScrollY;   // mouse capture 할 당시의 커서 위치값.
        bool bMouseCaptureFlag = false;
        int nOldMX, nOldMY;

        private async void FormGraphic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            if (IsMouseSleep())
            {
                return;
            }

			nOldMX = e.X;
			nOldMY = e.Y;

			if(e.Button == MouseButtons.Left) 
			{
				if(ConfigViewMain.bResponseMouseLeftOnGraphic) 
				{	  
					if(cViewMode == EnumViewMode.PANNING) 
					{
						this.Capture = true;
						bMouseCaptureFlag = true;
						nCaptureX = e.X;
						nCaptureY = e.Y;

                        //Point point = this.AutoScrollPosition;
                        //nCaptureScrollX = point.X;
                        //nCaptureScrollY = point.Y;

                        nCaptureScrollX = scrollPosition.X; //20250317 PSU 수정.
                        nCaptureScrollY = scrollPosition.Y;
						return;
					}
					if(cViewMode == EnumViewMode.ZOOM_IN) 
					{
						this.Capture = true;
						bMouseCaptureFlag = true;
						nCaptureX = e.X;
						nCaptureY = e.Y;
                        //Point point = this.AutoScrollPosition;
                        //nCaptureScrollX = point.X;
                        //nCaptureScrollY = point.Y;
                        nCaptureScrollX = scrollPosition.X;  //20250317 PSU 수정.
                        nCaptureScrollY = scrollPosition.Y;
						return;
					}
					if(cViewMode == EnumViewMode.ZOOM_OUT) 
					{
						ZoomOut();
						return;
					}
                    //25-02-05 Digital Input Select Mode 추가
                    if (cViewMode == EnumViewMode.DIGITAL_SELECT)
                    {
                        SelectedTagNameList.Clear();
                        this.Capture = true;
                        bMouseCaptureFlag = true;
                        bMultiSelectFlag = true;
                        nCaptureX = e.X;
                        nCaptureY = e.Y;

                        return;

                    }

					await objectGraphic.WmLeftButtonDown(this, e);
				}
				else 
				{
					if(Tools.IsLangKorean()) 
						MessageBox.Show("왼쪽 마우스 버튼 사용이 금지되어 있습니다.", "사용 제한");
					else if(Tools.IsLangChinese()) 
						MessageBox.Show("禁止使用鼠标左按钮。", "使用限制");
					else
						MessageBox.Show("Left mouse button is proteced.", "Mouse Protect");
				}
			}
			else if(e.Button == MouseButtons.Right) 
			{
                if (cViewMode == EnumViewMode.DIGITAL_SELECT && bMultiSelectFlag)
                {
                    bMultiSelectFlag = false;
                    this.Capture = false;
                    bMouseCaptureFlag = false;

                    return;

                }
                
                if(ConfigViewMain.bResponseMouseRightOnGraphic)	  
				{
					if(await objectGraphic.WmRightButtonDown(this, e) == false) 
					{
                        if (ConfigViewMain.bResponseMouseRightGraphicContextMenu)
                        {
                            Point point = new Point(e.X, e.Y);

                            contextMenuMain.Show(this, point);
                        }
					}
					else 
					{
						sSelectedTagName = objectGraphic.sSelectedTagName;
						eSelectedTagType = objectGraphic.eSelectedTagType;
						//nSelectedTagPos  = objectGraphic.nSelectedTagPos;

                        if (ConfigViewMain.bResponseMouseRightGraphicContextMenu)
                        {
                            if (sSelectedTagName.Length > 0)
                            {
                                Point point = new Point(e.X, e.Y);
                                if (eSelectedTagType == EnumTagType.AI)
                                    contextMenuAnalog.Show(this, point);
                                else if (eSelectedTagType == EnumTagType.DI)
                                    contextMenuDigital.Show(this, point);
                            }
                        }
					}
				}
				else 
				{
					if(Tools.IsLangKorean()) 
						MessageBox.Show("오른쪽 마우스 버튼 사용이 금지되어 있습니다.", "사용 제한");
					else if(Tools.IsLangChinese()) 
						MessageBox.Show("禁止使用鼠标右按钮。", "使用限制");
					else
						MessageBox.Show("Right mouse button is proteced.", "Mouse Protect");

				}
			}
		}

        private async void FormGraphic_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (cViewMode == EnumViewMode.PANNING)
                {
                    this.Capture = false;
                    bMouseCaptureFlag = false;


                    // 저장된 임시 이동 값을 사용하여 스크롤 위치 변경  //20250314 PSU , 드래그(패닝) 시 화면깜빡임이 심해 마우스UP 시 적용.
                    //Point point = this.AutoScrollPosition;
                    //point.X = tempMoveX - nCaptureScrollX;
                    //point.Y = tempMoveY - nCaptureScrollY;

                    //this.AutoScrollPosition = point;

                    // 스크롤바 값 동기화
                    if (vScrollBar != null && hScrollBar != null)
                    {
                        hScrollBar.Value = Math.Min(hScrollBar.Maximum, Math.Max(0, scrollPosition.X));
                        vScrollBar.Value = Math.Min(vScrollBar.Maximum, Math.Max(0, scrollPosition.Y));
                    }

                    UpdateNavigatorDisplay();
                    // 화면 갱신
                    Invalidate();
                    return;
                }
                if (cViewMode == EnumViewMode.ZOOM_IN)
                {
                    ZoomIn();

                    this.Capture = false;
                    bMouseCaptureFlag = false;
                    return;
                }
                if (cViewMode == EnumViewMode.ZOOM_OUT)
                {
                    return;
                }
                //25-02-05 Digital Input Select Mode 추가
                if (cViewMode == EnumViewMode.DIGITAL_SELECT)
                {
                    this.Capture = false; //20250317 PSU 추가.
                    bMouseCaptureFlag = false;  //20250317 PSU 추가.

                    if (bMultiSelectFlag)
                    {
                        bMultiSelectFlag = false;

                        if (RecurseSelectIfIncluded(objectGraphic.groupRoot, nCaptureX, nCaptureY, nOldMX, nOldMY) > 0)
                        {
                            ControlBoxDigitalInputSelectGo dialog = new ControlBoxDigitalInputSelectGo();

                            dialog.Go(this, SelectedTagNameList);
                        }
                    }

                    return;
                }

                await objectGraphic.WmLeftButtonUp(this, e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                await objectGraphic.WmRightButtonUp(this, e);
            }

            // 휠 버튼 클릭 처리 추가 20250212 PSU
            else if (e.Button == MouseButtons.Middle)
            {
                if (cViewMode == EnumViewMode.CONTROL)
                    //cViewMode = EnumViewMode.PANNING;
                    SetViewMode(EnumViewMode.PANNING); //20250312 PSU
                else //cViewMode = EnumViewMode.CONTROL;
                    SetViewMode(EnumViewMode.CONTROL);  //20250312 PSU
            }
        }

        private async void FormGraphic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            nOldMX = e.X;
            nOldMY = e.Y;

            if (cViewMode == EnumViewMode.ZOOM_IN)
            {
                this.Cursor = cursorZoomIn;
                if (bMouseCaptureFlag)
                {
                    Invalidate();
                }
            }
            else if (cViewMode == EnumViewMode.ZOOM_OUT)
            {
                this.Cursor = cursorZoomOut;
            }
            else if (cViewMode == EnumViewMode.PANNING)
            {
                this.Cursor = cursorPanning;

                if (bMouseCaptureFlag)
                {	// mouse capture 중이다.
                    int newX = nCaptureScrollX + (nCaptureX - e.X);
                    int newY = nCaptureScrollY + (nCaptureY - e.Y);

                    // 스크롤 위치 업데이트
                    scrollPosition.X = Math.Max(0, Math.Min(newX, hScrollBar.Maximum - hScrollBar.LargeChange));
                    scrollPosition.Y = Math.Max(0, Math.Min(newY, vScrollBar.Maximum - vScrollBar.LargeChange));

                    // 스크롤바 값 동기화
                    hScrollBar.Value = Math.Min(hScrollBar.Maximum, Math.Max(0, scrollPosition.X));
                    vScrollBar.Value = Math.Min(vScrollBar.Maximum, Math.Max(0, scrollPosition.Y));

                    Invalidate();
                }
            }
            //25-02-05 Digital Input Select Mode 추가
            else if (cViewMode == EnumViewMode.DIGITAL_SELECT)
            {
                this.Cursor = cursorCross;

                if (bMouseCaptureFlag)
                {   // mouse capture 중이다.

                }

            }
            else
            {
                this.Cursor = Cursors.Arrow;
                await objectGraphic.WmMouseMove(this, e);
            }
        }

        private void FormGraphic_MouseWheel(object sender, MouseEventArgs e)   //20250212 PSU 추가
        {
            //  Control + 휠: 줌 인/아웃
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                nOldMX = e.X;
                nOldMY = e.Y;

                if (e.Delta > 0)
                {
                    int rate = objectGraphic.GetOpticRate();
                    rate = GetNextZoomInRate(rate);
                    int pic_x = nOldMX;
                    int pic_y = nOldMY;
                    GetPicturePosition(ref pic_x, ref pic_y);
                    ZoomingGo(rate);
                    SetPicturePosition(pic_x, pic_y);
                }
                else
                {
                    ZoomOut();
                }
                return;
            }
            // Shift + 휠: 가로 스크롤만  20250317 PSU 추가
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (hScrollBar != null && hScrollBar.Visible)
                {
                    int scrollAmount = hScrollBar.SmallChange * (e.Delta > 0 ? -1 : 1);
                    int newValue = hScrollBar.Value + scrollAmount;

                    // 값 범위 제한
                    newValue = Math.Max(hScrollBar.Minimum, Math.Min(newValue, hScrollBar.Maximum - hScrollBar.LargeChange + 1));
                    hScrollBar.Value = newValue;
                    scrollPosition.X = newValue;

                    // 화면 갱신
                    Invalidate();
                }
                return;
            }

            // 일반 휠: 세로 스크롤만  20250317 PSU 추가
            if (vScrollBar != null && vScrollBar.Visible)
            {
                int scrollAmount = vScrollBar.SmallChange * (e.Delta > 0 ? -1 : 1);
                int newValue = vScrollBar.Value + scrollAmount;

                // 값 범위 제한
                newValue = Math.Max(vScrollBar.Minimum, Math.Min(newValue, vScrollBar.Maximum - vScrollBar.LargeChange + 1));
                vScrollBar.Value = newValue;
                scrollPosition.Y = newValue;

                // 화면 갱신
                Invalidate();
            }
        }

        #endregion mouse events

        void GetPicturePosition(ref int x, ref int y)
		{
			int rate = objectGraphic.GetOpticRate();

			//Point point = this.AutoScrollPosition;
            //x = (Math.Abs(point.X) + x) * 100 / rate;
            //y = (Math.Abs(point.Y) + y) * 100 / rate;

            x = (scrollPosition.X + x) * 100 / rate; //20250317 PSU
            y = (scrollPosition.Y + y) * 100 / rate;			
		}

		void SetPicturePosition(int x, int y)
		{
			Rectangle rect;
			int rate = objectGraphic.GetOpticRate();

			rect = this.ClientRectangle;

			x = x*rate/100-(rect.Right/2);
			y = y*rate/100-(rect.Bottom/2);

			if(x < 0)	x = 0;
			if(y < 0)	y = 0;

            //Point point = new Point(x, y);
            //this.AutoScrollPosition = point;

            scrollPosition.X = x;
            scrollPosition.Y = y;

            // 스크롤바 값 동기화
            if (vScrollBar != null && hScrollBar != null)
            {
                hScrollBar.Value = Math.Min(hScrollBar.Maximum, Math.Max(0, scrollPosition.X));
                vScrollBar.Value = Math.Min(vScrollBar.Maximum, Math.Max(0, scrollPosition.Y));
            }

			UpdateNavigatorDisplay();
		}

        #region Zooming
        private int GetNextZoomInRate(int currentRate) //20250212 PSU 추가
        {
            if (currentRate < 100)
            {
                if (currentRate <= 10) return 25;
                if (currentRate <= 25) return 33;
                if (currentRate <= 33) return 50;
                if (currentRate <= 50) return 66;
                if (currentRate <= 66) return 75;
                if (currentRate <= 75) return 80;
                if (currentRate <= 80) return 100;
            }
            else
            {
                if (currentRate <= 100) return 125;
                if (currentRate <= 125) return 150;
                if (currentRate <= 150) return 200;
                if (currentRate <= 200) return 300;
                if (currentRate <= 300) return 400;
                if (currentRate <= 400) return 500;
                if (currentRate <= 500) return 600;
                if (currentRate <= 600) return 700;
                if (currentRate <= 700) return 800;
                if (currentRate <= 800) return 900;
                if (currentRate <= 900) return 1000;
            }
            return currentRate;
        }

        private int GetNextZoomOutRate(int currentRate)  //20250212 PSU 추가
        {
            if (currentRate > 100)
            {
                if (currentRate >= 1000) return 900;
                if (currentRate >= 900) return 800;
                if (currentRate >= 800) return 700;
                if (currentRate >= 700) return 600;
                if (currentRate >= 600) return 500;
                if (currentRate >= 500) return 400;
                if (currentRate >= 400) return 300;
                if (currentRate >= 300) return 200;
                if (currentRate >= 200) return 150;
                if (currentRate >= 150) return 125;
                if (currentRate >= 125) return 100;
            }
            else
            {
                if (currentRate >= 100) return 80;
                if (currentRate >= 80) return 75;
                if (currentRate >= 75) return 66;
                if (currentRate >= 66) return 50;
                if (currentRate >= 50) return 33;
                if (currentRate >= 33) return 25;
                if (currentRate >= 25) return 10;
            }
            return currentRate;
        }

        void ZoomOut()
        {
            int rate = objectGraphic.GetOpticRate();
            int pic_x, pic_y;

            //rate = (int)(rate*0.7);
            rate = GetNextZoomOutRate(rate);  //20250212 PSU 수정

            pic_x = nOldMX;
            pic_y = nOldMY;

            GetPicturePosition(ref pic_x, ref pic_y);
            ZoomingGo(rate);
            SetPicturePosition(pic_x, pic_y);
        }

		void ZoomIn()
		{
			int rate;

			int x1 = nOldMX;
			int y1 = nOldMY;
			int x2 = nCaptureX;
			int y2 = nCaptureY;
			int pic_x, pic_y;

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);
	
			if(Math.Abs(x2-x1) < 10 && Math.Abs(y2-y1) < 10) 
			{
				rate = objectGraphic.nOpticRate;
				//rate = (int)(rate*1.7);
                rate = GetNextZoomInRate(rate);  //20250212 PSU 수정
				pic_x = nOldMX;
				pic_y = nOldMY;
				GetPicturePosition(ref pic_x, ref pic_y);
				ZoomingGo(rate);
				SetPicturePosition(pic_x, pic_y);
				return;
			}
	
			int width, height;
			int size;
			Rectangle rect;

			rect = this.ClientRectangle;

			objectGraphic.GetModuleSize(out width, out height);

			rate = objectGraphic.GetOpticRate();

			size = Math.Abs(nOldMX-nCaptureX)*100/rate;	// 선택한 실제 길이를 구한다.

			if(size == 0)
				rate = rect.Right*100;
			else
				rate = rect.Right*100/size;

			pic_x = x1+(x2-x1)/2;
			pic_y = y1+(y2-y1)/2;
			GetPicturePosition(ref pic_x, ref pic_y);
			ZoomingGo(rate);
			SetPicturePosition(pic_x, pic_y);
		}

        public void ZoomingGo(int rate)
        {
            if (rate > 1000) rate = 1000;
            if (rate < 10) rate = 10;

            objectGraphic.SetOpticRate(rate);
            ScrollUpdate();
            Invalidate();

            SetGraphicWindowTitle();
            UpdateNavigatorDisplay();
        }

        #endregion Zooming


        #region Digital Input Select Mode
        //25-02-05 Digital Input Select Mode 추가
        void DrawMutiSelectZone(Graphics g)
        {
            int x1, y1, x2, y2;

            x2 = nOldMX;
            y2 = nOldMY;
            x1 = nCaptureX;
            y1 = nCaptureY;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Pen pen = new Pen(Color.FromArgb(128, 255, 255, 255), 1);
            g.DrawRectangle(pen, x1, y1, x2 - x1, y2 - y1);

            pen = new Pen(Color.Black);
            g.DrawRectangle(pen, x1 - 1, y1 - 1, x2 - x1 + 2, y2 - y1 + 2);
            g.DrawRectangle(pen, x1 + 1, y1 + 1, x2 - x1 - 2, y2 - y1 - 2);

        }

        //25-02-05 Digital Input Select Mode 추가
        int RecurseSelectIfIncluded(ObjectPublicGroupLayer gl, int px1, int py1, int px2, int py2)
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int count = 0;
            int l;

            if (px1 > px2) Tools.Temp(ref px1, ref px2);
            if (py1 > py2) Tools.Temp(ref py1, ref py2);

            for (l = 0; l < gl.GetObjectHap(); l++)
            {
                ObjectExpand type = (ObjectExpand)gl.GetPoint(l);
                if (type.enumObjectType == EnumObjectType.Layer || type.enumObjectType == EnumObjectType.Group)
                {
                    count += RecurseSelectIfIncluded((ObjectPublicGroupLayer)type, px1, py1, px2, py2);
                }
                else
                {
                    type.GetZone(ref x1, ref y1, ref x2, ref y2);

                    x1 = type.GetViewPosX(x1);
                    y1 = type.GetViewPosY(y1);
                    x2 = type.GetViewPosX(x2);
                    y2 = type.GetViewPosY(y2);

                    if (x1 > x2) Tools.Temp(ref x1, ref x2);
                    if (y1 > y2) Tools.Temp(ref y1, ref y2);

                    if (x1 >= px1 && y1 >= py1 && x2 <= px2 && y2 <= py2)
                    {
                        if (type.enumObjectType == EnumObjectType.DigitalAnimation || type.enumObjectType == EnumObjectType.DigitalCircle ||
                            type.enumObjectType == EnumObjectType.DigitalRectangle || type.enumObjectType == EnumObjectType.DigitalString)
                        {
                            ObjectTag type_tag = (ObjectTag)type;

                            string buf = type_tag.sTagName;
                            if (!SelectedTagNameList.Contains(buf) && type_tag.GetCheckResponseOnVisible())
                            {
                                SelectedTagNameList.Add(buf);
                                count++;
                            }
                        }
                    }
                }
            }

            return count;
        }
        #endregion Digital Input Select Mode


        #region menuItem events
        private void menuItemClose_Click(object sender, System.EventArgs e)
        {

        }
        private void menuItemViewControl_Click(object sender, System.EventArgs e)
		{
			cViewMode = EnumViewMode.CONTROL;
		}

		private void menuItemViewZoomIn_Click(object sender, System.EventArgs e)
		{
			cViewMode = EnumViewMode.ZOOM_IN;
		}

		private void menuItemViewZoomOut_Click(object sender, System.EventArgs e)
		{
			cViewMode = EnumViewMode.ZOOM_OUT;
		}

		private void menuItemViewPanning_Click(object sender, System.EventArgs e)
		{
			cViewMode = EnumViewMode.PANNING;
		}

        private void menuItemViewDigitalInput_Click(object sender, EventArgs e)
        {
            cViewMode = EnumViewMode.DIGITAL_SELECT;
        }

		private void menuItemModuleReal_Click(object sender, System.EventArgs e)
		{
			ChangeModuleOpticMethod(0);
		}

		private void menuItemModuleFitWindow_Click(object sender, System.EventArgs e)
		{
			cViewMode = EnumViewMode.CONTROL;
			ChangeModuleOpticMethod(1);
		}

		private void menuItemModuleFitWindowXY_Click(object sender, System.EventArgs e)
		{
			cViewMode = EnumViewMode.CONTROL;
			ChangeModuleOpticMethod(2);
		}




        public void ChangeModuleOpticMethod(int method)
		{
			objectGraphic.SetObjectOpticMethod(method);
			ScrollUpdate();

            objectGraphic.SetScreenSize(ClientSize.Width, ClientSize.Height);   // 확대모드를 왔다갔다 할때 스크롤바가 없는 경우 이전화면을 계속가지고 있어서 갱신하기 위해서 추가. 2020-9-15 추가.

			this.Invalidate();
			this.Update();
		}


        private void menuItemViewPercent75_Click(object sender, System.EventArgs e)
        {
            ZoomingGo(75);
        }

        private void menuItemViewPercent80_Click(object sender, EventArgs e)
        {
            ZoomingGo(80); //2024-12-09 PSU 80% 추가
        }

        private void menuItemViewPercent100_Click(object sender, System.EventArgs e)
        {
            ZoomingGo(100);
        }

        private void menuItemViewPercent125_Click(object sender, EventArgs e)
        {
            ZoomingGo(125); //2024-12-09 PSU 125% 추가
        }

        private void menuItemViewPercent150_Click(object sender, System.EventArgs e)
        {
            ZoomingGo(150);
        }

        private void menuItemViewPercent200_Click(object sender, System.EventArgs e)
        {
            ZoomingGo(200);
        }

        private void menuItemViewPercent300_Click(object sender, System.EventArgs e)
        {
            ZoomingGo(300);
        }

        private void menuItemAnalogDetail_Click(object sender, System.EventArgs e)
        {
            // 간접태그는 직접태그가 있는 경우 그 태그를 사용한다. 2024-12-09 PSU
            TagAiClass ai;
            int[] pos = new int[1];
            ai = TagLib.GetStructAI(sSelectedTagName, ref pos);
            ai = TagLib.GetDirectTag(ai);
            sSelectedTagName = ai.tag;

            ViewAnalogInputDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDetailMain(sSelectedTagName), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        void ViewAnalogTrend(int hour)
        {
            ArrayList arr = new ArrayList();
            multiTrendTagStruct multi = new multiTrendTagStruct();

            // 간접태그는 직접태그가 있는 경우 그 태그를 사용한다. 2024-12-09 PSU
            TagAiClass ai;
            int[] pos = new int[1];
            ai = TagLib.GetStructAI(sSelectedTagName, ref pos);
            ai = TagLib.GetDirectTag(ai);
            multi.tag = ai.tag;

            //multi.tag = sSelectedTagName;
            multi.color = Color.Black;
            multi.edge = eTrendEdgeType.NONE;
            arr.Add(multi);

            ViewAnalogInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputTrendMain(hour, arr, Color.White, 30), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        private void menuItemAnalogTrend1_Click(object sender, System.EventArgs e)
        {
            ViewAnalogTrend(1);
        }

        private void menuItemAnalogTrend8_Click(object sender, System.EventArgs e)
        {
            ViewAnalogTrend(8);

        }

        private void menuItemAnalogTrend24_Click(object sender, System.EventArgs e)
        {
            ViewAnalogTrend(24);
        }

        private void menuItemAnalogTrend48_Click(object sender, System.EventArgs e)
        {
            ViewAnalogTrend(48);
        }

        private void menuItemAnalogTrend72_Click(object sender, System.EventArgs e)
        {
            ViewAnalogTrend(72);
        }

        private void menuItemAnalogTrend30_Click(object sender, System.EventArgs e)
        {
            ViewAnalogTrend(720);
        }

        void ViewAnalogData(eDataTime datatime)
        {
            ArrayList arr = new ArrayList();
            multiTrendTagStruct da = new multiTrendTagStruct();

            // 간접태그는 직접태그가 있는 경우 그 태그를 사용한다. 2024-12-09 PSU
            TagAiClass ai;
            int[] pos = new int[1];
            ai = TagLib.GetStructAI(sSelectedTagName, ref pos);
            ai = TagLib.GetDirectTag(ai);
            da.tag = ai.tag;

            //da.tag = sSelectedTagName;
            da.color = Color.Black;
            da.edge = eTrendEdgeType.NONE;
            arr.Add(da);

            ViewAnalogInputDataMain.ringViewAnalogInputDataMain.CreateMdi(TotalConfig.formMain, new ViewAnalogInputDataMain(datatime, arr, Color.White, 0), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        private void menuItemAnalogDataMin_Click(object sender, System.EventArgs e)
        {
            ViewAnalogData(eDataTime.MIN);
        }

        private void menuItemAnalogDataHour_Click(object sender, System.EventArgs e)
        {
            ViewAnalogData(eDataTime.HOUR);
        }

        private void menuItemAnalogDataDay_Click(object sender, System.EventArgs e)
        {
            ViewAnalogData(eDataTime.DAY);
        }

        private void menuItemAnalogWeek_Click(object sender, System.EventArgs e)
        {
            ViewAnalogData(eDataTime.WEEK);
        }

        private void menuItemAnalogMonth_Click(object sender, System.EventArgs e)
        {
            ViewAnalogData(eDataTime.MONTH);
        }

        private void menuItemAnalogTagList_Click(object sender, System.EventArgs e)
        {
            ViewAnalogInputMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewAnalogInputMain(sSelectedTagName), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        private void menuItemAnalogSetValue_Click(object sender, System.EventArgs e)
        {
            ControlBoxAnalogInputGo dialog = new ControlBoxAnalogInputGo();
            dialog.Go(this, sSelectedTagName);
        }

        private void menuItemDigitalDetail_Click(object sender, System.EventArgs e)
        {
            // 간접태그는 직접태그가 있는 경우 그 태그를 사용한다. 2024-12-09 PSU
            TagDiClass di;
            int[] pos = new int[1];
            di = TagLib.GetStructDI(sSelectedTagName, ref pos);
            di = TagLib.GetDirectTag(di);
            sSelectedTagName = di.tag;

            ViewDigitalInputDetailMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputDetailMain(sSelectedTagName), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        void ViewDigitalTrend(int hour)
        {
            ArrayList arr = new ArrayList();
            multiTrendTagStruct multi = new multiTrendTagStruct();

            // 간접태그는 직접태그가 있는 경우 그 태그를 사용한다. 2024-12-09 PSU
            TagDiClass di;
            int[] pos = new int[1];
            di = TagLib.GetStructDI(sSelectedTagName, ref pos);
            di = TagLib.GetDirectTag(di);
            multi.tag = di.tag;

            //multi.tag = sSelectedTagName;
            multi.color = Color.Black;
            multi.edge = eTrendEdgeType.NONE;
            arr.Add(multi);

            ViewDigitalInputTrendMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputTrendMain(hour, arr, Color.White), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        private void menuItemDigitalTrend1_Click(object sender, System.EventArgs e)
        {
            ViewDigitalTrend(1);
        }

        private void menuItemDigitalTrend8_Click(object sender, System.EventArgs e)
        {
            ViewDigitalTrend(8);
        }

        private void menuItemDigitalTrend24_Click(object sender, System.EventArgs e)
        {
            ViewDigitalTrend(24);
        }

        private void menuItemDigitalTrend48_Click(object sender, System.EventArgs e)
        {
            ViewDigitalTrend(48);
        }

        private void menuItemDigitalTrend72_Click(object sender, System.EventArgs e)
        {
            ViewDigitalTrend(72);
        }

        private void menuItemDigitalTrend30_Click(object sender, System.EventArgs e)
        {
            ViewDigitalTrend(720);
        }

        void ViewDigitalData(eDataTime datatime)
        {
            ArrayList arr = new ArrayList();
            multiTrendTagStruct da = new multiTrendTagStruct();

            // 간접태그는 직접태그가 있는 경우 그 태그를 사용한다. 2024-12-09 PSU
            TagDiClass di;
            int[] pos = new int[1];
            di = TagLib.GetStructDI(sSelectedTagName, ref pos);
            di = TagLib.GetDirectTag(di);
            da.tag = di.tag;

            //da.tag = sSelectedTagName;
            da.color = Color.Black;
            da.edge = eTrendEdgeType.NONE;
            arr.Add(da);

            ViewDigitalInputDataMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputDataMain(datatime, arr, SharedData.colorTotal.BACK, 0), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        private void menuItemDigitalDataMin_Click(object sender, System.EventArgs e)
        {
            ViewDigitalData(eDataTime.MIN);
        }

        private void menuItemDigitalDataHour_Click(object sender, System.EventArgs e)
        {
            ViewDigitalData(eDataTime.HOUR);
        }

        private void menuItemDigitalDataDay_Click(object sender, System.EventArgs e)
        {
            ViewDigitalData(eDataTime.DAY);
        }

        private void menuItemDigitalDataWeek_Click(object sender, System.EventArgs e)
        {
            ViewDigitalData(eDataTime.WEEK);
        }

        private void menuItemDigitalDataMonth_Click(object sender, System.EventArgs e)
        {
            ViewDigitalData(eDataTime.MONTH);
        }

        private void menuItemDigitalTagList_Click(object sender, System.EventArgs e)
        {
            ViewDigitalInputMain.ringForm.CreateMdi(TotalConfig.formMain, new ViewDigitalInputMain(sSelectedTagName), ConfigViewMain.nMdiCountOnBasicScreen);
        }

        private void menuItemDigitalSetValue_Click(object sender, System.EventArgs e)
        {
            ControlBoxDigitalInputGo dialog = new ControlBoxDigitalInputGo();

            dialog.Go(this, sSelectedTagName);
        }

        private void menuItemViewPercent50_Click(object sender, System.EventArgs e)
        {
            ZoomingGo(50);
        }

        private void menuItemScreenPrint_Click(object sender, System.EventArgs e)
        {
            Rectangle r = this.ClientRectangle;
            Bitmap bitmap = new Bitmap(r.Width, r.Height, CreateGraphics());

            Graphics g = Graphics.FromImage(bitmap);

            //objectGraphic.Display(g, r, r, this.AutoScrollPosition);
            objectGraphic.Display(g, r, r, scrollPosition); //20250317 PSU

            DialogClipBoardPrint.FormClipBoardPrint dialog = new DialogClipBoardPrint.FormClipBoardPrint();

            dialog.prepareBitmap = bitmap;

            dialog.ShowDialog(this);
        }


        void ChangeProtectFlagAI(EnumProtectFlag mask)
        {
            int[] tag_pos = new int[1];

            TagAiClass ai = TagLib.GetStructAI(sSelectedTagName, ref tag_pos);
            ai = TagLib.GetDirectTag(ai);   // 간접태그는직접태그가있는경우그태그를사용한다. PSU

            if ((ai.wProtectFlags & mask) > 0) ai.wProtectFlags &= (EnumProtectFlag)(0xFFFF - (int)mask);
            else ai.wProtectFlags |= mask;

            LibComNetServer.SendCommandProtectFlagChange(ai.tag, (int)ai.wProtectFlags);

            TagLib.bChangedByLocalMain = true;
        }

        private void menuItemAnalogProtectScan_Click(object sender, System.EventArgs e)
        {
            int[] tag_pos = new int[1];

            TagAiClass ai = TagLib.GetStructAI(sSelectedTagName, ref tag_pos);
            ai = TagLib.GetDirectTag(ai);   // 간접태그는직접태그가있는경우그태그를사용한다. PSU

            BasicScreen.ViewAnalogInputHandInputDlg dialog = new BasicScreen.ViewAnalogInputHandInputDlg(ai);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void menuItemAnalogProtectControl_Click(object sender, System.EventArgs e)
        {
            ChangeProtectFlagAI(EnumProtectFlag.CONTROL);
        }

        private void menuItemAnalogProtectAlarmEvent_Click(object sender, System.EventArgs e)
        {
            ChangeProtectFlagAI(EnumProtectFlag.ALARM_EVENT);
        }

        private void menuItemAnalogProtectAlarmData_Click(object sender, System.EventArgs e)
        {
            ChangeProtectFlagAI(EnumProtectFlag.ALARM_DATA);
        }

        private void menuItemDigitalProtectScan_Click(object sender, System.EventArgs e)
        {
            int[] tag_pos = new int[1];

            TagDiClass di = TagLib.GetStructDI(sSelectedTagName, ref tag_pos);
            di = TagLib.GetDirectTag(di);   // 간접태그는직접태그가있는경우그태그를사용한다. PSU

            BasicScreen.kdymain.ViewDigitalInputHandInputDlg dialog = new ViewDigitalInputHandInputDlg(di);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        void ChangeProtectFlagDI(EnumProtectFlag mask)
        {
            int[] tag_pos = new int[1];

            TagDiClass di = TagLib.GetStructDI(sSelectedTagName, ref tag_pos);
            di = TagLib.GetDirectTag(di);   // 간접태그는직접태그가있는경우그태그를사용한다. PSU

            if ((di.wProtectFlags & mask) > 0) di.wProtectFlags &= (EnumProtectFlag)(0xFFFF - (int)mask);
            else di.wProtectFlags |= mask;

            LibComNetServer.SendCommandProtectFlagChange(di.tag, (int)di.wProtectFlags);

            TagLib.bChangedByLocalMain = true;
        }

        private void menuItemDigitalProtectControl_Click(object sender, System.EventArgs e)
        {
            ChangeProtectFlagDI(EnumProtectFlag.CONTROL);
        }

        private void menuItemDigitalProtectAlarmEvent_Click(object sender, System.EventArgs e)
        {
            ChangeProtectFlagDI(EnumProtectFlag.ALARM_EVENT);
        }

        private void menuItemDigitalProtectAlarmData_Click(object sender, System.EventArgs e)
        {
            ChangeProtectFlagDI(EnumProtectFlag.ALARM_DATA);
        }

        #endregion menuItem events




        public void ScrollUpdate()
        {
            if (this.IsDisposed) return;

            // 스크롤바가 초기화되지 않았을 경우 예외 방지
            if (vScrollBar == null || hScrollBar == null) return;

            if (objectGraphic.cObjectOpticMethod == 0)
            {
                // 정확한 그래픽 크기 계산 - 정수 연산 오차 방지를 위해 더 정밀하게 계산
                int graphicWidth = (int)Math.Ceiling(this.ClientSize.Width * (double)objectGraphic.nOpticRate / 100);
                int graphicHeight = (int)Math.Ceiling(this.ClientSize.Height * (double)objectGraphic.nOpticRate / 100);

                // 정확한 클라이언트 영역 - 폼의 테두리와 제목 표시줄 제외
                int clientWidth = this.ClientSize.Width;
                int clientHeight = this.ClientSize.Height;

                // 디버깅용 - 실제 값 확인
               // Debug.WriteLine(string.Format("Rate: {0}%, GraphicSize: {1}x{2}, ClientSize: {3}x{4}",
               //     objectGraphic.nOpticRate, graphicWidth, graphicHeight, clientWidth, clientHeight));

                if (objectGraphic.nModuleSizeX > this.ClientSize.Width) graphicWidth = (int)Math.Ceiling(objectGraphic.nModuleSizeX * (double)objectGraphic.nOpticRate / 100);
                if (objectGraphic.nModuleSizeY > this.ClientSize.Height) graphicHeight = (int)Math.Ceiling(objectGraphic.nModuleSizeY * (double)objectGraphic.nOpticRate / 100);

                // 디버깅용 - 실제 값 확인
                //Debug.WriteLine(string.Format("2nd // Rate: {0}%, GraphicSize: {1}x{2}, ClientSize: {3}x{4}",
                //    objectGraphic.nOpticRate, graphicWidth, graphicHeight, clientWidth, clientHeight));

                // 스크롤바 필요 여부 - 1픽셀이라도 크면 스크롤바 표시
                bool needHScroll = graphicWidth > clientWidth;
                bool needVScroll = graphicHeight > clientHeight;

                // 하나의 스크롤바가 표시되면 다른 쪽 영역이 줄어듦
                if (needHScroll)
                {
                    clientHeight -= hScrollBar.Height;
                    // 가로 스크롤바가 표시되면 세로 스크롤바 필요 여부 다시 계산
                    needVScroll = graphicHeight > clientHeight;
                }

                if (needVScroll)
                {
                    clientWidth -= vScrollBar.Width;
                    // 세로 스크롤바가 표시되면 가로 스크롤바 필요 여부 다시 계산
                    needHScroll = graphicWidth > clientWidth;
                }

                // 스크롤바 설정
                vScrollBar.Minimum = 0;
                hScrollBar.Minimum = 0;

                // 페이지 크기 설정
                // 화면 비율(rate)을 고려한 LargeChange 계산
                vScrollBar.LargeChange = Math.Max(1, (int)(clientHeight * 100.0 / objectGraphic.nOpticRate));
                hScrollBar.LargeChange = Math.Max(1, (int)(clientWidth * 100.0 / objectGraphic.nOpticRate));

                // 스크롤 증가량 설정
                vScrollBar.SmallChange = Math.Max(1, clientHeight / 10);
                hScrollBar.SmallChange = Math.Max(1, clientWidth / 10);

                // Maximum 설정 - 전체 크기 - 1
                vScrollBar.Maximum = Math.Max(0, graphicHeight - 1);
                hScrollBar.Maximum = Math.Max(0, graphicWidth - 1);

                // 스크롤 위치 제한
                int maxScrollX = Math.Max(0, graphicWidth - clientWidth);
                int maxScrollY = Math.Max(0, graphicHeight - clientHeight);

                scrollPosition.X = Math.Min(scrollPosition.X, maxScrollX);
                scrollPosition.Y = Math.Min(scrollPosition.Y, maxScrollY);

                // 스크롤바 가시성 - 확인용 추가 로그
                //Debug.WriteLine(string.Format("NeedScroll V:{0}, H:{1}, MaxScroll:{2}x{3}",
                //    needVScroll, needHScroll, maxScrollX, maxScrollY));

                // 값 설정 전 유효범위 확인
                if (needHScroll)
                {
                    int validValue = Math.Min(Math.Max(0, scrollPosition.X), Math.Max(0, hScrollBar.Maximum - hScrollBar.LargeChange));
                    hScrollBar.Value = validValue;
                }
                else
                {
                    scrollPosition.X = 0;
                }

                if (needVScroll)
                {
                    int validValue = Math.Min(Math.Max(0, scrollPosition.Y), Math.Max(0, vScrollBar.Maximum - vScrollBar.LargeChange));
                    vScrollBar.Value = validValue;
                }
                else
                {
                    scrollPosition.Y = 0;
                }

                // 스크롤바 가시성 최종 설정
                vScrollBar.Visible = needVScroll;
                hScrollBar.Visible = needHScroll;
            }
            else
            {
                // 확대 모드가 실제 크기가 아닐 경우 스크롤바 숨김
                vScrollBar.Visible = false;
                hScrollBar.Visible = false;
                scrollPosition = new Point(0, 0);
            }

            // 화면 갱신 - 스크롤바 변경 사항 반영
            this.Invalidate();
        }



		/// <summary>
		/// 모듈 파일명의 그래픽폴더를 제외한 Sub폴더와 파일명을 가져온다.
		/// 작업 폴더의 파일이 아닌 경우는 전체 경로명을 보여준다.
		/// </summary>
		/// <returns></returns>
		public string GetModuleSubDirAndName()
		{
			string dir = AutoLib.MakeFilePath.GetProjectDirectory()+"\\Graphic";
			// 작업폴더 밑에 있는 파일이다.
			if(String.Compare(sFileName, 0, dir, 0, dir.Length, true) == 0)
				return sFileName.Substring(dir.Length+1);
			else
				return sFileName;
		}

        public bool bUseUserDefinedTitle = false;  // 사용자가 지정한 타이틀을 사용한다.

		void SetGraphicWindowTitle()
		{
            if (bUseUserDefinedTitle) return;

			if(this.ParentForm.Name != "FormGraphicFrame")	return;

			string title;

            if (TotalConfig.eOemType == EnumOemType.ZIoT)
            {
                title = "";
            }
            else
            {
                //20251030 PSU 팝업모듈 제목X + 시스템메뉴 O 시 제목 숨기기.
                EnumWindowStyleFlags flags = (EnumWindowStyleFlags)objectGraphic.GetModuleWindowStyleFlag();
                if (!flags.HasFlag(EnumWindowStyleFlags.WS_CAPTION) &&
                    flags.HasFlag(EnumWindowStyleFlags.WS_SYSMENU))
                {
                    title = "";
                }
                else
                {
                    title = GetModuleSubDirAndName();
                    title += String.Format(" ({0}%)", objectGraphic.nOpticRate);
                }
            }

            this.ParentForm.Text = title;
		}




		public void NavigatorChangePosSize(RECT r)
		{
			ChangeModuleOpticMethod(0);

			int x, y;
			Rectangle rect = ClientRectangle;

			objectGraphic.GetModuleSize(out x, out y);

			int rate_x, rate_y;
			int pic_x, pic_y;

			if((r.right-r.left) <= 0)
				rate_x = rect.Right*100;
			else
				rate_x = rect.Right*100/(r.right-r.left);

			if((r.bottom-r.top) <= 0) 
				rate_y = rect.Bottom*100;
			else
				rate_y = rect.Bottom*100/(r.bottom-r.top);

			pic_x = r.left+(r.right-r.left)/2;
			pic_y = r.top+(r.bottom-r.top)/2;

			ZoomingGo(rate_x < rate_y ? rate_x : rate_y);

			SetPicturePosition(pic_x, pic_y);

		}

		public void IdmPublicMdiSetScrollPos(int direction)
		{
            //20250317 PSU 수정
            if (direction == 1)
                scrollPosition.Y -= 10;
            else if (direction == 2)
                scrollPosition.X += 10;
            else if (direction == 3)
                scrollPosition.Y += 10;
            else
                scrollPosition.X -= 10;

            // 범위 검사 추가
            scrollPosition.X = Math.Max(0, Math.Min(scrollPosition.X, hScrollBar.Maximum - hScrollBar.LargeChange));
            scrollPosition.Y = Math.Max(0, Math.Min(scrollPosition.Y, vScrollBar.Maximum - vScrollBar.LargeChange));

            // 스크롤바 값 동기화
            if (vScrollBar != null && hScrollBar != null)
            {
                hScrollBar.Value = Math.Min(hScrollBar.Maximum, Math.Max(0, scrollPosition.X));
                vScrollBar.Value = Math.Min(vScrollBar.Maximum, Math.Max(0, scrollPosition.Y));
            }

			UpdateNavigatorDisplay();
		}


        #region context menu


        private void contextMenuMain_Popup(object sender, System.EventArgs e)
        {
            menuItemModuleReal.Checked = objectGraphic.cObjectOpticMethod == 0 ? true : false;
            menuItemModuleFitWindow.Checked = objectGraphic.cObjectOpticMethod == 1 ? true : false;
            menuItemModuleFitWindowXY.Checked = objectGraphic.cObjectOpticMethod == 2 ? true : false;

            menuItemViewControl.Checked = cViewMode == EnumViewMode.CONTROL ? true : false;
            menuItemViewZoomIn.Checked = cViewMode == EnumViewMode.ZOOM_IN ? true : false;
            menuItemViewZoomOut.Checked = cViewMode == EnumViewMode.ZOOM_OUT ? true : false;
            menuItemViewPanning.Checked = cViewMode == EnumViewMode.PANNING ? true : false;
            menuItemViewDigitalInputSelect.Checked = cViewMode == EnumViewMode.DIGITAL_SELECT ? true : false; //25-02-05 hsjeong

            menuItemViewPercent50.Checked = objectGraphic.nOpticRate == 50 ? true : false;
            menuItemViewPercent75.Checked = objectGraphic.nOpticRate == 75 ? true : false;
            menuItemViewPercent80.Checked = objectGraphic.nOpticRate == 80 ? true : false;  //2024-12-09 PSU
            menuItemViewPercent100.Checked = objectGraphic.nOpticRate == 100 ? true : false;
            menuItemViewPercent125.Checked = objectGraphic.nOpticRate == 125 ? true : false;  //2024-12-09 PSU
            menuItemViewPercent150.Checked = objectGraphic.nOpticRate == 150 ? true : false;
            menuItemViewPercent200.Checked = objectGraphic.nOpticRate == 200 ? true : false;
            menuItemViewPercent300.Checked = objectGraphic.nOpticRate == 300 ? true : false;

            bool flag = false;
            if (objectGraphic.cObjectOpticMethod != 0) flag = false;
            else flag = true;

            //DigitalInputSelectMode 추가로 인한 조작모드 선택 가능하게 변경 필요 2025-02-06 hsjeong
            //menuItemViewControl.Enabled = flag;
            menuItemViewZoomIn.Enabled = flag;
            menuItemViewZoomOut.Enabled = flag;
            menuItemViewPanning.Enabled = flag;


            menuItemViewPercent50.Enabled = flag;
            menuItemViewPercent75.Enabled = flag;
            menuItemViewPercent80.Enabled = flag; //2024-12-09 PSU
            menuItemViewPercent100.Enabled = flag;
            menuItemViewPercent125.Enabled = flag; //2024-12-09 PSU
            menuItemViewPercent150.Enabled = flag;
            menuItemViewPercent200.Enabled = flag;
            menuItemViewPercent300.Enabled = flag;
        }



        private void contextMenuAnalog_Popup(object sender, System.EventArgs e)
		{
			int[] tag_pos = new int[1];

			TagAiClass ai = TagLib.GetStructAI(sSelectedTagName, ref tag_pos);
            ai = TagLib.GetDirectTag(ai);   // 간접태그는직접태그가있는경우그태그를사용한다. 2024-12-09 PSU

			this.menuItemAnalogProtectAlarmData.Checked = (ai.wProtectFlags & EnumProtectFlag.ALARM_DATA) > 0;
			this.menuItemAnalogProtectAlarmEvent.Checked = (ai.wProtectFlags & EnumProtectFlag.ALARM_EVENT) > 0;
			this.menuItemAnalogProtectControl.Checked = (ai.wProtectFlags & EnumProtectFlag.CONTROL) > 0;
			this.menuItemAnalogProtectScan.Checked = (ai.wProtectFlags & EnumProtectFlag.SCAN) > 0;

            this.menuItemAnalogProtectAlarmData.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemAnalogProtectAlarmEvent.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemAnalogProtectControl.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemAnalogProtectScan.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemAnalogProtectSeparator.Visible = ConfigViewMain.bUseProtectMenu;
		}

		private void contextMenuDigital_Popup(object sender, System.EventArgs e)
		{
			int[] tag_pos = new int[1];

			TagDiClass di = TagLib.GetStructDI(sSelectedTagName, ref tag_pos);
            di = TagLib.GetDirectTag(di);   // 간접태그는직접태그가있는경우그태그를사용한다. 2024-12-09 PSU

			this.menuItemDigitalProtectAlarmData.Checked = (di.wProtectFlags & EnumProtectFlag.ALARM_DATA) > 0;
			this.menuItemDigitalProtectAlarmEvent.Checked = (di.wProtectFlags & EnumProtectFlag.ALARM_EVENT) > 0;
			this.menuItemDigitalProtectControl.Checked = (di.wProtectFlags & EnumProtectFlag.CONTROL) > 0;
			this.menuItemDigitalProtectScan.Checked = (di.wProtectFlags & EnumProtectFlag.SCAN) > 0;

            this.menuItemDigitalProtectAlarmData.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemDigitalProtectAlarmEvent.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemDigitalProtectControl.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemDigitalProtectScan.Visible = ConfigViewMain.bUseProtectMenu;
            this.menuItemDigitalProtectSeparator.Visible = ConfigViewMain.bUseProtectMenu;
		}

        #endregion context menu


        private void FormGraphicChild_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			
		}

        private async void FormGraphicChild_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 모듈 종료 시 프로그램을 시작한다.
            await PlayScriptWhenModStartEnd("ModEnd");

            SharedViewMain.EventListTagChanged -= new SharedViewMain.OnEventTagChanged(FormGraphic_EventTag);
            SharedViewMain.EventListTimer -= timerHandler; // 같은 참조로 해제
            objectGraphic.Close();
        }
	}
}
