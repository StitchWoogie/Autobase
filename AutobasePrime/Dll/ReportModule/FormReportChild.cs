using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using NetTools.OldDefine;
using System.Drawing.Drawing2D;
using System.IO;
using AutoLibLocal;
using DialogTag;
using System.Drawing.Printing;
using ReportBasicLib;
using System.Runtime.Serialization.Formatters.Binary;
using AutoLib;
using NetTools.Hash;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormMainEdit.
	/// </summary>
	public class FormReportChild : System.Windows.Forms.Form
	{
        private sealed class BufferedCanvasPanel : Panel
        {
            public BufferedCanvasPanel()
            {
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.StandardDoubleClick |
                              ControlStyles.UserPaint |
                              ControlStyles.OptimizedDoubleBuffer |
                              ControlStyles.ResizeRedraw, true);
                this.UpdateStyles();
            }
        }

        private static readonly Dictionary<string, Font> s_fontCache = new Dictionary<string, Font>();
        private static readonly Dictionary<int, SolidBrush> s_brushCache = new Dictionary<int, SolidBrush>();
        private static readonly StringFormat[,] s_cellFormats = CreateCellFormats();
        private static readonly Dictionary<string, ParsedCellDisplay> s_cellDisplayCache = new Dictionary<string, ParsedCellDisplay>();
        private const int TABLE_HEADER_WIDTH = 40;
        private const int TABLE_HEADER_HEIGHT = 20;
        private const int TABLE_HEADER_GAP = 2;

        private sealed class ParsedCellDisplay
        {
            public EnumCommand CommandId = EnumCommand.COMMAND_UNKNOWN;
            public string CommandDescription = "";
            public string TagText = "";
            public string TimeText = "";
        }

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public int nHorScrollPos = 0; 
		public int nVerScrollPos = 0;
		public int nHorScrollHap = 0;
		public int nVerScrollHap = 0;
		public readonly int START_X = 20;
		public readonly int START_Y = 20;
		public WORK_VIEW_STRUCT workView = new WORK_VIEW_STRUCT();
		public REPORT_STRUCT reportEdit;
		REPORT_STRUCT reportRun;
		//REPORT_STRUCT reportUndo;
		public string sFilename;
		Form formParent;	// parent는 pannel에 걸려있기 때문에 실제 Parent를 알아야 한다.

		public Cursor cursorRight = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ReportModule.Cursor.right_arrow.cur"));
		public Cursor cursorDown = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ReportModule.Cursor.bottom_arrow.cur"));
		public Cursor cursorRightDown = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ReportModule.Cursor.right_down_arrow.cur"));
		private System.Windows.Forms.Label label1;
        private BufferedCanvasPanel panelCanvas;
        private Panel panelQuickCellEdit;
        private Panel panelQuickCellHeader;
        private Panel panelQuickCellNameHost;
        private Label labelQuickCellEdit;
        private Label labelQuickCellSummary;
        private TextBox textBoxQuickCellName;
        private TextBox textBoxQuickCellEdit;
        private Panel panelQuickCellTextHost;
        private bool bQuickCellTextSync = false;
        private bool bQuickCellTextDirty = false;
        private bool bQuickCellNameSync = false;
        private bool bQuickCellPreviewActive = false;
        private bool bQuickCellPreviewUndoSaved = false;
        private string sQuickCellPreviewOriginalText = "";

		StatusBarPanel statusBarPanelMain = null;

		public int nUndoPos = 0;
		public ArrayList undoList = new ArrayList();
        /// <summary>
        /// 리포터 파일을 만들 때 오류메시지를 넣어서 만든다.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static REPORT_STRUCT MakeReportWithErrorMessage(string msg)
        {
            REPORT_STRUCT report = ReportLib.MakeNewReport();

            InsertNewTable(report, 0, 1, 1);

            TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[0];
            CELL_STRUCT cell = (CELL_STRUCT)table.cellBuf[0];
            cell.width = 300;
            cell.height = 100;
            cell.cAlignVert = 3;
            cell.text = msg;

            return report;
        }

		public static async Task<REPORT_STRUCT> MakeRunReportPublic(REPORT_STRUCT source, EnumHandAuto hand_auto, string filename)
		{
			REPORT_STRUCT target;

            if (ConfigVarTotal.bLocalFlag)
            {
                ReportRuntimeFacade facade = new ReportRuntimeFacade();
                ReportExecutionResult result = await facade.ExecuteFileAsync(
                    CreateExecutionRequest(filename, hand_auto)).ConfigureAwait(false);

                if (!result.Success || result.Report == null)
                {
                    target = MakeReportWithErrorMessage(Path.GetFileName(filename) + " : " + result.ErrorMessage);
                    return target;
                }

                target = result.Report;
            }
            else
            {
                if (ConfigVarTotal.eServiceType == EnumServiceType.WcfService)
                {
                    /*
                    AutoLib.ServiceReferenceDataGate.ServiceDataGateClient service = ServiceLibDataGate.GetServiceDataGate();

                    string data = null;

                    try
                    {
                        MakeHashCrc ht = new MakeHashCrc();
                        string hash = ht.ComputeHash("GetLogFile" + ServiceLibDataGate.nConnectionID.ToString() + log_name);

                        data = service.GetReportFile(ServiceLibDataGate.nConnectionID, Path.GetFileName(filename), , hash);
                    }
                    catch (Exception ex)
                    {
                        data = null;
                        WebCommInfo.SetError(ex.Message);
                    }

                    DataSet ds = new DataSet();

                    if (data != null)
                    {
                        ds.ReadXml(new XmlTextReader(new StringReader(data)));
                    }

                    return ds;*/
                    target = MakeReportWithErrorMessage(Path.GetFileName(filename) + " : WCF 모드에서는 아직 미지원");
                    return target;
                }
                else
                {
                    string report_path = String.Format("{0}\\Report\\", AutoLib.MakeFilePath.GetProjectDirectory());
                    string report_subname;

                    if(filename.Length < report_path.Length) 
                        report_subname= Path.GetFileName(filename);
                    else 
                        report_subname = filename.Substring(report_path.Length); // drive:\path\report\ 를 제외한 이름.
                       

                    if (ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 3, 2, 1))
                    {
                        AutoLib.ServiceReferenceReport.ServiceReportClient service = ServiceLib.GetServiceReport();

                        string[] keys;
                        string[] values;

                        ServiceLib.DicToString(out keys, out values);

                        byte[] b = service.GetReportStructWithDic(
                            ServiceLib.StringToReportString(keys),
                            ServiceLib.StringToReportString(values),
                            report_subname, ReportConfig.tHandReportTime, ReportConfig.tAutoReportTime, ReportConfig.GetMinListTimeFr(), ReportConfig.GetMinListTimeTo(), (int)hand_auto);
                        if (b == null)
                        {
                            target = MakeReportWithErrorMessage(report_subname + " : 파일이 없거나 로딩 중 오류 발생");
                            return target;
                        }

                        MemoryStream m = new MemoryStream(b);
                        BinaryFormatter f = new BinaryFormatter();
                        target = (REPORT_STRUCT)f.Deserialize(m);
                        m.Close();
                    }
                    else
                    {
                        ReportBasicLib.ServiceReferenceReport.ServiceReportClient service = new ReportBasicLib.ServiceReferenceReport.ServiceReportClient();
                        //service.Url = AutoLibLocal.ConfigVarTotal.GetServicePath("ServiceReport.svc);
                        //service.CookieContainer = ConfigVarTotal.cookieContainer;

                        byte[] b = service.GetReportStruct(report_subname, ReportConfig.tHandReportTime, ReportConfig.tAutoReportTime, ReportConfig.GetMinListTimeFr(), ReportConfig.GetMinListTimeTo(), (int)hand_auto);
                        if (b == null)
                        {
                            target = MakeReportWithErrorMessage(report_subname + " : 파일이 없거나 로딩 중 오류 발생");
                            return target;
                        }

                        MemoryStream m = new MemoryStream(b);
                        BinaryFormatter f = new BinaryFormatter();
                        target = (REPORT_STRUCT)f.Deserialize(m);
                        m.Close();
                    }

                }

            }

			return target;
		}

        private static ReportExecutionRequest CreateExecutionRequest(string filename, EnumHandAuto hand_auto)
        {
            ReportExecutionRequest request = new ReportExecutionRequest
            {
                TemplateFile = filename,
                HandAuto = hand_auto,
                HandTime = ReportConfig.tHandReportTime,
                AutoTime = ReportConfig.tAutoReportTime,
                MinListFrom = ReportConfig.GetMinListTimeFr(),
                MinListTo = ReportConfig.GetMinListTimeTo()
            };

            if (ConfigVarTotal.varKeys == null || ConfigVarTotal.varValues == null)
            {
                return request;
            }

            int max = Math.Min(ConfigVarTotal.varKeys.Length, ConfigVarTotal.varValues.Length);
            if (max <= 0)
            {
                return request;
            }

            request.StringVariables = new System.Collections.Generic.Dictionary<string, string>();
            for (int i = 0; i < max; i++)
            {
                string key = ConfigVarTotal.varKeys[i];
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                request.StringVariables[key] = ConfigVarTotal.varValues[i];
            }

            return request;
        }

		public FormReportChild(Form parent, string filename, StatusBarPanel statusbarpanel, EnumViewMode mode)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            InitializeQuickCellEditBar();

            // 더블 버퍼링 활성화 깜빡임 제거  20250701 PSU 추가
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            workView.nViewMode = mode;

			statusBarPanelMain = statusbarpanel;
			reportEdit = null;//ReportLib.MakeNewReport();
			reportRun = null;

			string path = Path.GetPathRoot(filename);

			if(path.Length == 0)	// 파일 이름만 존재
			{
                //sFilename = String.Format("{0}\\Report\\{1}", TotalConfig.sDirWorkProject, filename);
                sFilename = String.Format("{0}\\Report\\{1}", AutoLib.MakeFilePath.GetProjectDirectory(), filename); //24-06-03 @RportCloseName 위해 경로읽어오는 함수 변경
			}
			else 
			{
				sFilename = filename;
			}

			if(mode == EnumViewMode.EDIT)
			{
				reportEdit = ReportFile.ReportLoad(sFilename, true);

				if(reportEdit == null)	reportEdit = ReportLib.MakeNewReport();
			}
			else 
			{
				if(ConfigVarTotal.bLocalFlag) 
				{
					reportEdit = ReportFile.ReportLoad(sFilename, true);
				}

				// 파일을 없거나 해서 읽지 못했을 때 문제가 될수 있으므로 기본 리포터를 만들어 준다.
				if(reportEdit == null)	reportEdit = ReportLib.MakeNewReport();

				//reportRun =  MakeRunReportPublic(reportEdit, 0, sFilename);
                this.Load += async (s, e) =>
                {
                    reportRun = await MakeRunReportPublic(reportEdit, 0, sFilename);
                    SetTitle();
                    ScrollUpdate();
                };
            }

			formParent = parent;

			this.label1.ForeColor = Color.FromArgb(0,0,0,0);
			this.label1.BackColor = Color.FromArgb(0,0,0,0);
			this.label1.Left = -this.label1.Width;	// 라벨을 영역밖으로 옮기지 않으면 기본화면의 마우스 응답을 Label이 먼저 응답한다.
            UpdateQuickCellEditBar(GetReportStruct());
		}

        public new Point AutoScrollPosition
        {
            get
            {
                if (panelCanvas != null)
                {
                    return panelCanvas.AutoScrollPosition;
                }

                return base.AutoScrollPosition;
            }
            set
            {
                if (panelCanvas != null)
                {
                    panelCanvas.AutoScrollPosition = value;
                    return;
                }

                base.AutoScrollPosition = value;
            }
        }

        public new Rectangle ClientRectangle
        {
            get
            {
                if (panelCanvas != null)
                {
                    return panelCanvas.ClientRectangle;
                }

                return base.ClientRectangle;
            }
        }

        public new bool Capture
        {
            get
            {
                if (panelCanvas != null)
                {
                    return panelCanvas.Capture;
                }

                return base.Capture;
            }
            set
            {
                if (panelCanvas != null)
                {
                    panelCanvas.Capture = value;
                }

                base.Capture = value;
            }
        }

        public new Cursor Cursor
        {
            get
            {
                if (panelCanvas != null && panelCanvas.Cursor != null)
                {
                    return panelCanvas.Cursor;
                }

                return base.Cursor;
            }
            set
            {
                if (panelCanvas != null)
                {
                    panelCanvas.Cursor = value;
                }

                base.Cursor = value;
            }
        }

        public new void Invalidate()
        {
            base.Invalidate();
            if (panelCanvas != null)
            {
                panelCanvas.Invalidate();
            }
        }

        public string GetReportSubDirAndName()
        {
            string dir = AutoLib.MakeFilePath.GetProjectDirectory() + "\\Report";
            // 작업폴더 밑에 있는 파일이다.
            if (String.Compare(sFilename, 0, dir, 0, dir.Length, true) == 0)
                return sFilename.Substring(dir.Length + 1);
            else
                return sFilename;
        }

		public void SetTitle()
		{
			REPORT_STRUCT report = GetReportStruct();
			if (report == null) return;

			string title;

			string dir = AutoLib.MakeFilePath.GetProjectDirectory()+"\\Report";
			// 작업폴더 밑에 있는 파일이다.
			if(String.Compare(sFilename, 0, dir, 0, dir.Length, true) == 0)
				title = sFilename.Substring(dir.Length+1);
			else
				title = sFilename;

			title += String.Format(" ({0}%)", report.wOpticRate);
			formParent.Text = title;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportChild));
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // FormReportChild
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = null;
            this.KeyPreview = true;
            this.Name = "FormReportChild";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormMainEdit_Paint);
            this.DoubleClick += new System.EventHandler(this.FormReportChild_DoubleClick);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormReportChild_MouseUp);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormReportChild_Closing);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormReportChild_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormReportChild_MouseDown);
            this.Load += new System.EventHandler(this.FormMainEdit_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormMainEdit_Load(object sender, System.EventArgs e)
		{
			SetTitle();
			ScrollUpdate();
            UpdateQuickCellEditBar(GetReportStruct());
		}

        private void InitializeQuickCellEditBar()
        {
            Panel panelQuickCellHeaderGap;
            Panel panelQuickCellEditGap;
            panelCanvas = new BufferedCanvasPanel();
            panelQuickCellEdit = new Panel();
            panelQuickCellHeader = new Panel();
            panelQuickCellNameHost = new Panel();
            labelQuickCellEdit = new Label();
            labelQuickCellSummary = new Label();
            textBoxQuickCellName = new TextBox();
            textBoxQuickCellEdit = new TextBox();
            panelQuickCellTextHost = new Panel();

            panelCanvas.Dock = DockStyle.Fill;
            panelCanvas.AutoScroll = true;
            panelCanvas.BackColor = SystemColors.AppWorkspace;
            panelCanvas.TabStop = true;
            panelCanvas.Paint += new PaintEventHandler(this.FormMainEdit_Paint);
            panelCanvas.DoubleClick += new EventHandler(this.FormReportChild_DoubleClick);
            panelCanvas.MouseUp += new MouseEventHandler(this.FormReportChild_MouseUp);
            panelCanvas.MouseMove += new MouseEventHandler(this.FormReportChild_MouseMove);
            panelCanvas.MouseDown += new MouseEventHandler(this.FormReportChild_MouseDown);
            this.Controls.Add(panelCanvas);
            panelCanvas.SendToBack();

            panelQuickCellEdit.Dock = DockStyle.Top;
            panelQuickCellEdit.Height = 58;
            panelQuickCellEdit.Padding = new Padding(6, 4, 6, 4);
            panelQuickCellEdit.BackColor = SystemColors.Control;

            panelQuickCellHeader.Dock = DockStyle.Top;
            panelQuickCellHeader.Height = 24;
            panelQuickCellHeader.Padding = new Padding(2, 0, 0, 0);
            panelQuickCellHeaderGap = new Panel();
            panelQuickCellEditGap = new Panel();

            labelQuickCellEdit.Dock = DockStyle.Left;
            labelQuickCellEdit.Width = 38;
            labelQuickCellEdit.TextAlign = ContentAlignment.MiddleLeft;
            labelQuickCellEdit.Text = "주소";
            labelQuickCellEdit.Padding = new Padding(0, 0, 6, 0);

            panelQuickCellNameHost.Dock = DockStyle.Left;
            panelQuickCellNameHost.Width = 92;
            panelQuickCellNameHost.BackColor = SystemColors.Window;
            panelQuickCellNameHost.BorderStyle = BorderStyle.FixedSingle;
            panelQuickCellNameHost.Resize += delegate { AlignQuickEditControls(); };

            textBoxQuickCellName.Dock = DockStyle.None;
            textBoxQuickCellName.Width = 92;
            textBoxQuickCellName.Height = TextRenderer.MeasureText("A", textBoxQuickCellName.Font).Height + 2;
            textBoxQuickCellName.Multiline = false;
            textBoxQuickCellName.BorderStyle = BorderStyle.None;
            textBoxQuickCellName.KeyDown += textBoxQuickCellName_KeyDown;
            textBoxQuickCellName.Leave += textBoxQuickCellName_Leave;
            panelQuickCellNameHost.Controls.Add(textBoxQuickCellName);

            panelQuickCellHeaderGap.Dock = DockStyle.Left;
            panelQuickCellHeaderGap.Width = 10;

            panelQuickCellEditGap.Dock = DockStyle.Top;
            panelQuickCellEditGap.Height = 4;

            labelQuickCellSummary.Dock = DockStyle.Fill;
            labelQuickCellSummary.TextAlign = ContentAlignment.MiddleLeft;
            labelQuickCellSummary.AutoEllipsis = true;
            labelQuickCellSummary.Text = "명령: 없음";
            labelQuickCellSummary.Padding = new Padding(2, 0, 0, 0);

            textBoxQuickCellEdit.Dock = DockStyle.None;
            textBoxQuickCellEdit.Height = TextRenderer.MeasureText("A", textBoxQuickCellEdit.Font).Height + 2;
            textBoxQuickCellEdit.Multiline = false;
            textBoxQuickCellEdit.BorderStyle = BorderStyle.None;
            textBoxQuickCellEdit.KeyDown += textBoxQuickCellEdit_KeyDown;
            textBoxQuickCellEdit.Leave += textBoxQuickCellEdit_Leave;
            textBoxQuickCellEdit.TextChanged += textBoxQuickCellEdit_TextChanged;

            panelQuickCellTextHost.Dock = DockStyle.Fill;
            panelQuickCellTextHost.BackColor = SystemColors.Window;
            panelQuickCellTextHost.BorderStyle = BorderStyle.FixedSingle;
            panelQuickCellTextHost.Resize += delegate { AlignQuickEditControls(); };
            panelQuickCellTextHost.Controls.Add(textBoxQuickCellEdit);

            panelQuickCellHeader.Controls.Add(labelQuickCellSummary);
            panelQuickCellHeader.Controls.Add(panelQuickCellHeaderGap);
            panelQuickCellHeader.Controls.Add(panelQuickCellNameHost);
            panelQuickCellHeader.Controls.Add(labelQuickCellEdit);
            panelQuickCellEdit.Controls.Add(panelQuickCellTextHost);
            panelQuickCellEdit.Controls.Add(panelQuickCellEditGap);
            panelQuickCellEdit.Controls.Add(panelQuickCellHeader);
            this.Controls.Add(panelQuickCellEdit);
            panelQuickCellEdit.BringToFront();
            AlignQuickEditControls();
        }

        private void AlignQuickEditControls()
        {
            if (textBoxQuickCellName != null && panelQuickCellNameHost != null)
            {
                int nameHeight = Math.Min(textBoxQuickCellName.Height, Math.Max(16, panelQuickCellNameHost.ClientSize.Height - 2));
                textBoxQuickCellName.Width = Math.Max(20, panelQuickCellNameHost.ClientSize.Width - 4);
                textBoxQuickCellName.Height = nameHeight;
                textBoxQuickCellName.Left = 2;
                textBoxQuickCellName.Top = Math.Max(0, (panelQuickCellNameHost.ClientSize.Height - nameHeight) / 2);
            }

            if (textBoxQuickCellEdit != null && panelQuickCellTextHost != null)
            {
                int editHeight = Math.Min(textBoxQuickCellEdit.Height, Math.Max(16, panelQuickCellTextHost.ClientSize.Height - 2));
                textBoxQuickCellEdit.Width = Math.Max(20, panelQuickCellTextHost.ClientSize.Width - 4);
                textBoxQuickCellEdit.Height = editHeight;
                textBoxQuickCellEdit.Left = 2;
                textBoxQuickCellEdit.Top = Math.Max(0, (panelQuickCellTextHost.ClientSize.Height - editHeight) / 2);
            }
        }

        private void UpdateQuickCellEditBar(REPORT_STRUCT report)
        {
            if (panelQuickCellEdit == null || textBoxQuickCellEdit == null || labelQuickCellEdit == null)
            {
                return;
            }

            bool bEditMode = workView.nViewMode == EnumViewMode.EDIT;
            bool bHasReport = report != null && report.TableCount > 0;
            bool bSingleCell = false;
            string cellId = "";
            string text = "";
            string summaryText = "명령: 없음";

            if (bEditMode && bHasReport)
            {
                bSingleCell = report.cursor_x1 == report.cursor_x2 && report.cursor_y1 == report.cursor_y2;
                if (bSingleCell)
                {
                    MakeCellIdString(out cellId, report.cursor_table, report.cursor_x1, report.cursor_y1);
                    SelectedCell.SelectedCellGetText(report, ref text);
                    summaryText = MakeQuickCellSummary(text, report);
                }
                else
                {
                    MakeCellRangeString(out cellId, report);
                    SelectedCell.SelectedCellGetText(report, ref text);
                    summaryText = "여러 셀 선택";
                }
            }

            panelQuickCellEdit.Visible = true;
            panelQuickCellEdit.Enabled = true;
            if (textBoxQuickCellName != null)
            {
                if (!textBoxQuickCellName.Focused)
                {
                    bQuickCellNameSync = true;
                    try
                    {
                        textBoxQuickCellName.Enabled = bEditMode && bHasReport;
                        textBoxQuickCellName.Text = bEditMode && bHasReport ? cellId : "";
                    }
                    finally
                    {
                        bQuickCellNameSync = false;
                    }
                }
                else
                {
                    textBoxQuickCellName.Enabled = bEditMode && bHasReport;
                }
            }
            labelQuickCellSummary.Text = summaryText;

            if (!(bQuickCellTextDirty && textBoxQuickCellEdit.Focused))
            {
                bQuickCellTextSync = true;
                try
                {
                    textBoxQuickCellEdit.Enabled = bEditMode && bHasReport;
                    textBoxQuickCellEdit.Text = bEditMode && bHasReport ? text : "";
                }
                finally
                {
                    bQuickCellTextSync = false;
                    bQuickCellTextDirty = false;
                }
            }
            else
            {
                textBoxQuickCellEdit.Enabled = bEditMode && bHasReport;
            }
        }

        private string MakeQuickCellSummary(string text, REPORT_STRUCT report)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "명령: 없음";
            }

            if (text[0] != '=')
            {
                return "텍스트 셀";
            }

            string formulaText = text.Substring(1).Trim();
            if (formulaText.StartsWith("@"))
            {
                return "함수: " + formulaText;
            }

            try
            {
                CommaBlockString comma = new CommaBlockString();
                string command = "";
                string description = "";
                string tag = "";

                comma.Set(text);
                comma.GetString(ref command);
                EnumCommand id = ReportLib.ChangeCommandStringToId(command.Substring(1));
                ReportLib.ChangeCommandIdToDes(out description, id);
                SelectedCell.SelectedCellGetTag(report, ref tag);

                if (string.IsNullOrEmpty(description) || id == EnumCommand.COMMAND_UNKNOWN)
                {
                    description = formulaText;
                }

                if (string.IsNullOrEmpty(tag))
                {
                    return "명령: " + description;
                }

                return string.Format("명령: {0} / 태그: {1}", description, tag);
            }
            catch
            {
                return "명령: 해석 불가";
            }
        }

        private void CommitQuickCellEdit()
        {
            if (bQuickCellTextSync || !bQuickCellTextDirty)
            {
                return;
            }

            REPORT_STRUCT report = GetReportStruct();
            if (report == null || report.TableCount == 0)
            {
                return;
            }

            if (workView.nViewMode != EnumViewMode.EDIT)
            {
                return;
            }

            ApplyQuickCellEditText(report);
        }

        private bool HandleUndoRedoShortcut(Keys keyData)
        {
            if ((keyData & Keys.Control) != Keys.Control)
            {
                return false;
            }

            Keys keyCode = keyData & Keys.KeyCode;
            bool bShift = (keyData & Keys.Shift) == Keys.Shift;

            if (keyCode != Keys.Z && keyCode != Keys.Y)
            {
                return false;
            }

            bQuickCellTextDirty = false;
            EndQuickCellPreview();

            if (keyCode == Keys.Z && bShift)
            {
                ReportEditorUndo.menuItemEditRedo_Click(this);
            }
            else if (keyCode == Keys.Y)
            {
                ReportEditorUndo.menuItemEditRedo_Click(this);
            }
            else
            {
                ReportEditorUndo.menuItemEditUndo_Click(this);
            }

            UpdateQuickCellEditBar(GetReportStruct());
            Invalidate();
            return true;
        }

        private void SaveUndoForQuickCellEdit(REPORT_STRUCT report)
        {
            if (report == null)
            {
                return;
            }

            if(Tools.IsLangKorean()) 
            {
                ReportEditorUndo.UndoSave(this, report.cursor_x1 == report.cursor_x2 && report.cursor_y1 == report.cursor_y2 ? "셀 텍스트 바꿈" : "선택 셀 채움");
            } 
            else if(Tools.IsLangChinese()) 
            {
                ReportEditorUndo.UndoSave(this, report.cursor_x1 == report.cursor_x2 && report.cursor_y1 == report.cursor_y2 ? "更改单元格文本" : "填充选定单元格");
            } 
            else 
            {
                ReportEditorUndo.UndoSave(this, report.cursor_x1 == report.cursor_x2 && report.cursor_y1 == report.cursor_y2 ? "Change Cell Text" : "Fill Selected Cells");
            }
        }

        private void StartQuickCellPreview(REPORT_STRUCT report)
        {
            if (bQuickCellPreviewActive || report == null)
            {
                return;
            }

            SelectedCell.SelectedCellGetText(report, ref sQuickCellPreviewOriginalText);
            bQuickCellPreviewActive = true;
            bQuickCellPreviewUndoSaved = false;
        }

        private void EndQuickCellPreview()
        {
            bQuickCellPreviewActive = false;
            bQuickCellPreviewUndoSaved = false;
            sQuickCellPreviewOriginalText = "";
        }

        private void PreviewQuickCellEditText(REPORT_STRUCT report)
        {
            if (bQuickCellTextSync || textBoxQuickCellEdit == null || report == null)
            {
                return;
            }

            StartQuickCellPreview(report);

            string current = "";
            SelectedCell.SelectedCellGetText(report, ref current);
            string next = textBoxQuickCellEdit.Text ?? "";

            if (current == next)
            {
                DrawCellInfo(report);
                return;
            }

            if (!bQuickCellPreviewUndoSaved)
            {
                SaveUndoForQuickCellEdit(report);
                bQuickCellPreviewUndoSaved = true;
            }

            SelectedCell.SelectedCellSetText(report, next);
            SetChangeFlag();
            Invalidate();
            DrawCellInfo(report);
        }

        private void CancelQuickCellPreview(REPORT_STRUCT report)
        {
            if (!bQuickCellPreviewActive || report == null || textBoxQuickCellEdit == null)
            {
                return;
            }

            bQuickCellTextSync = true;
            try
            {
                textBoxQuickCellEdit.Text = sQuickCellPreviewOriginalText;
            }
            finally
            {
                bQuickCellTextSync = false;
            }

            SelectedCell.SelectedCellSetText(report, sQuickCellPreviewOriginalText);
            bQuickCellTextDirty = false;
            Invalidate();
            DrawCellInfo(report);
            EndQuickCellPreview();
        }

        private void ApplyQuickCellEditText(REPORT_STRUCT report)
        {
            if (report == null || textBoxQuickCellEdit == null)
            {
                return;
            }

            string current = "";
            SelectedCell.SelectedCellGetText(report, ref current);

            if (current == textBoxQuickCellEdit.Text)
            {
                bQuickCellTextDirty = false;
                EndQuickCellPreview();
                return;
            }

            if (!bQuickCellPreviewUndoSaved)
            {
                SaveUndoForQuickCellEdit(report);
            }

            SelectedCell.SelectedCellSetText(report, textBoxQuickCellEdit.Text);
            bQuickCellTextDirty = false;
            SetChangeFlag();
            Invalidate();
            DrawCellInfo(report);
            EndQuickCellPreview();
        }

        private void textBoxQuickCellEdit_TextChanged(object sender, EventArgs e)
        {
            if (bQuickCellTextSync)
            {
                return;
            }

            bQuickCellTextDirty = true;
            PreviewQuickCellEditText(GetReportStruct());
        }

        private void textBoxQuickCellEdit_Leave(object sender, EventArgs e)
        {
            CommitQuickCellEdit();
        }

        private void textBoxQuickCellName_Leave(object sender, EventArgs e)
        {
            CommitQuickCellNameSelection();
        }

        private void textBoxQuickCellEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleUndoRedoShortcut(e.KeyData))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                CommitQuickCellEdit();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                REPORT_STRUCT report = GetReportStruct();
                CancelQuickCellPreview(report);
                UpdateQuickCellEditBar(report);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxQuickCellName_KeyDown(object sender, KeyEventArgs e)
        {
            if (HandleUndoRedoShortcut(e.KeyData))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                CommitQuickCellNameSelection();
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                UpdateQuickCellEditBar(GetReportStruct());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void CommitQuickCellNameSelection()
        {
            if (bQuickCellNameSync || textBoxQuickCellName == null)
            {
                return;
            }

            REPORT_STRUCT report = GetReportStruct();
            if (workView.nViewMode != EnumViewMode.EDIT || report == null || report.TableCount <= 0)
            {
                return;
            }

            int tableNo;
            int x1;
            int y1;
            int x2;
            int y2;
            if (!TryParseNameBoxSelection(textBoxQuickCellName.Text, report, out tableNo, out x1, out y1, out x2, out y2))
            {
                UpdateQuickCellEditBar(report);
                return;
            }

            if (x1 == x2 && y1 == y2)
            {
                SetCursorPosition(report, tableNo, x1, y1);
            }
            else
            {
                SetCursorPosition(report, tableNo, x1, y1, x2, y2);
            }

            ScrollUpdate();
            Invalidate();
        }

        private bool TryParseNameBoxSelection(string text, REPORT_STRUCT report, out int tableNo, out int x1, out int y1, out int x2, out int y2)
        {
            tableNo = 0;
            x1 = y1 = x2 = y2 = 0;

            if (string.IsNullOrWhiteSpace(text) || report == null || report.TableCount <= 0)
            {
                return false;
            }

            string[] parts = text.Trim().Split(':');
            if (parts.Length == 1)
            {
                if (!TryParseNameBoxCell(parts[0], report, report.cursor_table, out tableNo, out x1, out y1))
                {
                    return false;
                }

                x2 = x1;
                y2 = y1;
                return true;
            }

            if (parts.Length != 2)
            {
                return false;
            }

            int tableNo2;
            if (!TryParseNameBoxCell(parts[0], report, report.cursor_table, out tableNo, out x1, out y1))
            {
                return false;
            }

            if (!TryParseNameBoxCell(parts[1], report, tableNo, out tableNo2, out x2, out y2))
            {
                return false;
            }

            return tableNo == tableNo2;
        }

        private static bool TryParseNameBoxCell(string text, REPORT_STRUCT report, int defaultTableNo, out int tableNo, out int cellX, out int cellY)
        {
            tableNo = defaultTableNo;
            cellX = 0;
            cellY = 0;

            if (string.IsNullOrWhiteSpace(text) || report == null)
            {
                return false;
            }

            string value = text.Trim().ToUpperInvariant();
            int index = 0;
            int parsedTableNo = 0;
            bool hasTableNo = false;

            while (index < value.Length && char.IsDigit(value[index]))
            {
                hasTableNo = true;
                parsedTableNo = (parsedTableNo * 10) + (value[index] - '0');
                index++;
            }

            if (index >= value.Length || !char.IsLetter(value[index]))
            {
                if (hasTableNo)
                {
                    return false;
                }

                parsedTableNo = defaultTableNo;
                index = 0;
            }

            int columnStart = index;
            while (index < value.Length && char.IsLetter(value[index]))
            {
                index++;
            }

            if (columnStart == index || index >= value.Length)
            {
                return false;
            }

            int parsedCellX = 0;
            for (int i = columnStart; i < index; i++)
            {
                parsedCellX = (parsedCellX * 26) + (value[i] - 'A' + 1);
            }

            int parsedCellY = 0;
            int rowStart = index;
            while (index < value.Length && char.IsDigit(value[index]))
            {
                parsedCellY = (parsedCellY * 10) + (value[index] - '0');
                index++;
            }

            if (rowStart == index || index != value.Length)
            {
                return false;
            }

            if (parsedTableNo < 0 || parsedTableNo >= report.TableCount)
            {
                return false;
            }

            TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[parsedTableNo];
            parsedCellX -= 1;
            if (parsedCellX < 0 || parsedCellX >= table.cell_x || parsedCellY < 0 || parsedCellY >= table.cell_y)
            {
                return false;
            }

            tableNo = parsedTableNo;
            cellX = parsedCellX;
            cellY = parsedCellY;
            return true;
        }

		public REPORT_STRUCT GetReportStruct()
		{
			if(workView.nViewMode == EnumViewMode.RUN)
				return reportRun;
			else 
			{
		
				return reportEdit;
			}
		}

        public int GetCanvasOriginX()
        {
            return START_X;
        }

        public int GetCanvasOriginY()
        {
            int y = START_Y;
            if (panelQuickCellEdit != null && panelQuickCellEdit.Visible)
            {
                y = Math.Max(y, panelQuickCellEdit.Height + 6);
            }

            return y;
        }

        public Rectangle GetCanvasPaintBounds()
        {
            if (panelCanvas != null)
            {
                return panelCanvas.ClientRectangle;
            }

            return this.ClientRectangle;
        }

		private void FormMainEdit_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			nHorScrollPos = -this.AutoScrollPosition.X;
			nVerScrollPos = -this.AutoScrollPosition.Y;

			REPORT_STRUCT report = GetReportStruct();

			// TODO: add draw code for native data here
			TABLE_STRUCT table;
			int l;
			int current_y = -nVerScrollPos + GetCanvasOriginY();
			Rectangle rect;
			Graphics g = e.Graphics;

			rect = GetCanvasPaintBounds();

			if(report == null) 
			{
				g.FillRectangle(Brushes.White, rect);
                SafeException.SafeDrawString(g, "Report Struct == NULL", GetCachedFont("Gulim", 9f, FontStyle.Regular), Brushes.Black, rect, GetCellFormat(1, 1));
				return;
			}

			g.FillRectangle(GetCachedBrush(report.lColorPaper), rect);

			for(l = 0; l < report.TableCount; l++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[l];

				DrawOneTable(g, rect, report, table, ref current_y);
			}

			ReportEditorMouse.Paint(this, e.Graphics);
		}

		public static int GetRealView(REPORT_STRUCT report, int org)
		{
			return org*report.wOpticRate/100;
		}

		public static double GetRealView(REPORT_STRUCT report, double org)
		{
			return org*report.wOpticRate/100.0;
		}

		public int GetRealView(int org)
		{
			return GetRealView(GetReportStruct(), org);
		}

		public static int GetCellSizeWidth(REPORT_STRUCT report, TABLE_STRUCT table, CELL_STRUCT cell)
		{
			if(cell.cGroup == 0) 
				return GetRealView(report, cell.width);
			else if(cell.cGroup == 1) 
			{	// group
				CELL_STRUCT imsi;
				int width = 0;
				int x;

				for(x = cell.x; x <= cell.nGroupX; x++) 
				{
					imsi = (CELL_STRUCT)table.cellBuf[x+cell.y*table.cell_x];
					width += GetRealView(report, imsi.width);
				}
				return width;
			}
			else 
			{
				return 0;
			}
		}

		public static int GetCellSizeHeight(REPORT_STRUCT report, TABLE_STRUCT table, CELL_STRUCT cell)
		{
			if(cell.cGroup == 0) 
				return GetRealView(report, cell.height);
			else if(cell.cGroup == 1) 
			{	// group
				CELL_STRUCT imsi;
				int height = 0;
				int y;

				for(y = cell.y; y <= cell.nGroupY; y++) 
				{
					imsi = (CELL_STRUCT)table.cellBuf[cell.x+y*table.cell_x];
					height += GetRealView(report, imsi.height);
				}
				return height;
			}
			else 
			{
				return 0;
			}
		}

		int GetMaxHeight(REPORT_STRUCT report, TABLE_STRUCT table, int posy)
		{
			int max = 0;
			int cell_pos = table.cell_x*posy;
			int height = 0;
			CELL_STRUCT cell;
			int x;
	
			for(x = 0; x < table.cell_x; x++, cell_pos++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[cell_pos];
				height = GetCellSizeHeight(report, table, cell);
				if(height > max)	max = height;
			}

			return max;
		}

        public static void BuildCellViewSizeCache(REPORT_STRUCT report, TABLE_STRUCT table, int[] widthCache, int[] heightCache, int[] rowMaxHeightCache)
        {
            if (table == null)
            {
                return;
            }

            int count = table.cell_x * table.cell_y;
            for (int pos = 0; pos < count; pos++)
            {
                CELL_STRUCT cell = (CELL_STRUCT)table.cellBuf[pos];
                int width = GetCellSizeWidth(report, table, cell);
                int height = GetCellSizeHeight(report, table, cell);

                if (widthCache != null && pos < widthCache.Length)
                {
                    widthCache[pos] = width;
                }

                if (heightCache != null && pos < heightCache.Length)
                {
                    heightCache[pos] = height;
                }

                if (rowMaxHeightCache != null && cell.y < rowMaxHeightCache.Length && height > rowMaxHeightCache[cell.y])
                {
                    rowMaxHeightCache[cell.y] = height;
                }
            }
        }

        static ParsedCellDisplay GetParsedCellDisplay(CELL_STRUCT cell)
        {
            if (cell == null || string.IsNullOrEmpty(cell.text) || cell.text[0] != '=')
            {
                return null;
            }

            ParsedCellDisplay parsed;
            if (s_cellDisplayCache.TryGetValue(cell.text, out parsed))
            {
                return parsed;
            }

            parsed = ParseCellDisplay(cell.text);
            s_cellDisplayCache[cell.text] = parsed;
            return parsed;
        }

        static ParsedCellDisplay ParseCellDisplay(string cellText)
        {
            ParsedCellDisplay parsed = new ParsedCellDisplay();
            string command = "";

            CommaBlockString comma = new CommaBlockString();
            comma.Set(cellText);
            comma.GetString(ref command);

            if (command.Length <= 1)
            {
                parsed.CommandDescription = cellText;
                return parsed;
            }

            parsed.CommandId = ReportLib.ChangeCommandStringToId(command.Substring(1));
            if (parsed.CommandId == 0)
            {
                parsed.CommandDescription = cellText;
            }
            else
            {
                ReportLib.ChangeCommandIdToDes(out parsed.CommandDescription, parsed.CommandId);
                parsed.TagText = ParseCellTagText(cellText, parsed.CommandId);
                parsed.TimeText = ParseCellTimeText(cellText, parsed.CommandId);
            }

            return parsed;
        }

        static string ParseCellTagText(string cellText, EnumCommand id)
        {
            switch(id) 
            {
                case EnumCommand.COMMAND_AI_CURR:
                {
                    OBJECT_AI_CURR obj = new OBJECT_AI_CURR();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_AI_AVE:
                case EnumCommand.COMMAND_AI_MIN:
                case EnumCommand.COMMAND_AI_MAX:
                case EnumCommand.COMMAND_AI_SUM:
                case EnumCommand.COMMAND_AI_SUB:
                case EnumCommand.COMMAND_AI_MIN_TIME:
                case EnumCommand.COMMAND_AI_MAX_TIME:
                {
                    OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_AI_MULTI_AVE:
                case EnumCommand.COMMAND_AI_MULTI_MIN:
                case EnumCommand.COMMAND_AI_MULTI_MAX:
                case EnumCommand.COMMAND_AI_MULTI_SUM:
                case EnumCommand.COMMAND_AI_MULTI_SUB:
                {
                    OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_AI_MOMENT:
                case EnumCommand.COMMAND_AI_MULTI_MOMENT:
                case EnumCommand.COMMAND_DI_MOMENT:
                case EnumCommand.COMMAND_DI_MULTI_MOMENT:
                {
                    OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_AI_MAX_SUM:
                case EnumCommand.COMMAND_AI_MULTI_MAX_SUM:
                {
                    OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_AI_MIN_LIST:
                {
                    OBJECT_AI_MIN_LIST obj = new OBJECT_AI_MIN_LIST();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_DI_CURR:
                {
                    OBJECT_DI_CURR obj = new OBJECT_DI_CURR();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_DI_ONTIME:
                case EnumCommand.COMMAND_DI_OFFTIME:
                case EnumCommand.COMMAND_DI_ONCOUNT:
                {
                    OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_DI_MULTI_ONTIME:
                case EnumCommand.COMMAND_DI_MULTI_OFFTIME:
                case EnumCommand.COMMAND_DI_MULTI_ONCOUNT:
                {
                    OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_DI_ONOFF_LIST:
                {
                    OBJECT_DI_ONOFF_LIST obj = new OBJECT_DI_ONOFF_LIST();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_DI_ONOFF_LIST_SUM:
                case EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM:
                {
                    OBJECT_DI_ONOFF_LIST_SUM obj = new OBJECT_DI_ONOFF_LIST_SUM();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                case EnumCommand.COMMAND_ST_CURR:
                {
                    OBJECT_ST_CURR obj = new OBJECT_ST_CURR();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    return obj.tag;
                }
                default:
                    if(Tools.IsLangKorean()) return "없음";
                    if(Tools.IsLangJapanese()) return "タグない";
                    if(Tools.IsLangChinese()) return "没有标记";
                    return "Tag none";
            }
        }

        static string ParseCellTimeText(string cellText, EnumCommand id)
        {
            CELL_TIME time = null;

            switch(id)
            {
                case EnumCommand.COMMAND_AI_CURR:
                {
                    break;
                }
                case EnumCommand.COMMAND_AI_AVE:
                case EnumCommand.COMMAND_AI_MIN:
                case EnumCommand.COMMAND_AI_MAX:
                case EnumCommand.COMMAND_AI_SUM:
                case EnumCommand.COMMAND_AI_SUB:
                case EnumCommand.COMMAND_AI_MIN_TIME:
                case EnumCommand.COMMAND_AI_MAX_TIME:
                {
                    OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_AI_MULTI_AVE:
                case EnumCommand.COMMAND_AI_MULTI_MIN:
                case EnumCommand.COMMAND_AI_MULTI_MAX:
                case EnumCommand.COMMAND_AI_MULTI_SUM:
                case EnumCommand.COMMAND_AI_MULTI_SUB:
                {
                    OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_AI_MOMENT:
                case EnumCommand.COMMAND_AI_MULTI_MOMENT:
                case EnumCommand.COMMAND_DI_MOMENT:
                case EnumCommand.COMMAND_DI_MULTI_MOMENT:
                {
                    OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_AI_MAX_SUM:
                case EnumCommand.COMMAND_AI_MULTI_MAX_SUM:
                {
                    OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_AI_MIN_LIST:
                {
                    break;
                }
                case EnumCommand.COMMAND_DI_CURR:
                {
                    break;
                }
                case EnumCommand.COMMAND_DI_ONTIME:
                case EnumCommand.COMMAND_DI_OFFTIME:
                case EnumCommand.COMMAND_DI_ONCOUNT:
                {
                    OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_DI_MULTI_ONTIME:
                case EnumCommand.COMMAND_DI_MULTI_OFFTIME:
                case EnumCommand.COMMAND_DI_MULTI_ONCOUNT:
                {
                    OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_DI_ONOFF_LIST:
                {
                    OBJECT_DI_ONOFF_LIST obj = new OBJECT_DI_ONOFF_LIST();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_DI_ONOFF_LIST_SUM:
                case EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM:
                {
                    OBJECT_DI_ONOFF_LIST_SUM obj = new OBJECT_DI_ONOFF_LIST_SUM();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
                case EnumCommand.COMMAND_ETC_MULTI_COUNT:
                {
                    OBJECT_ETC_MULTI_COUNT obj = new OBJECT_ETC_MULTI_COUNT();
                    ReportLib.ObjectStringToStruct(ref obj, cellText);
                    time = obj.time;
                    break;
                }
            }

            if (time == null)
            {
                if(Tools.IsLangKorean()) return "없음";
                return "None";
            }

            string buf;
            MakeCellTimeInfoString(time, out buf);
            return buf;
        }

		static void DrawCellAsCommand(Graphics g, Font font, Brush brush, CELL_STRUCT cell, Rectangle r, StringFormat uFormat)
		{
			ParsedCellDisplay parsed = GetParsedCellDisplay(cell);

			if(parsed == null || parsed.CommandId == 0) 
			{
                SafeException.SafeDrawString(g, cell.text, font, brush, r, uFormat);
			}
			else 
			{
                SafeException.SafeDrawString(g, parsed.CommandDescription, font, brush, r, uFormat);
			}

		}

		static void DrawCellAsTag(Graphics g, Font font, Brush brush, CELL_STRUCT cell, Rectangle r, StringFormat uFormat)
		{
			ParsedCellDisplay parsed = GetParsedCellDisplay(cell);
            SafeException.SafeDrawString(g, parsed == null ? "" : parsed.TagText, font, brush, r, uFormat);
		}

		static void MakeCellTimeToText(out string buf, int time, int shift, int zone)
		{
			if(Tools.IsLangKorean()) 
			{
				char[] sWeekDay = { '일', '월', '화', '수', '목', '금', '토' };

				switch(zone) 
				{
					case 0:
						if(shift < 0)		buf = String.Format("이전{0}시 {1:00}분", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}시 {1:00}분", shift, time);
						else 				buf = String.Format("지정시 {0:00}분", time);
						break;
					case 1:
						if(shift < 0)		buf = String.Format("이전{0}일 {1:00}시", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}일 {1:00}시", shift, time);
						else 				buf = String.Format("지정일 {0:00}시", time);
						break;
					case 2:
						if(shift < 0)		buf = String.Format("이전{0}달 {1:00}일", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}달 {1:00}일", shift, time);
						else 				buf = String.Format("지정달 {0:00}일", time);
						break;
					case 3:
						if(shift < 0)		buf = String.Format("이전{0}주 {1}요일", shift, sWeekDay[time]);
						else if(shift > 0)	buf = String.Format("다음+{0}주 {1}요일", shift, sWeekDay[time]);
						else 				buf = String.Format("지정주 {0}요일", sWeekDay[time]);
						break;
					case 4:
						if(shift < 0)		buf = String.Format("이전{0}년 {1:00}월", shift, time);
						else if(shift > 0)	buf = String.Format("다음+{0}년 {1:00}월", shift, time);
						else 				buf = String.Format("지정년 {0:00}월", time);
						break;
					default:
						buf = String.Format("새로운 시간범위");
						break;
				}
			}
			else if(Tools.IsLangJapanese()) 
			{
				char[] sWeekDay = { '日', '月', '火', '水', '木', '金', '土' };

				switch(zone) 
				{
					case 0:
						if(shift < 0)		buf = String.Format("以前{0}時 {1:00}分", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}時 {1:00}分", shift, time);
						else 				buf = String.Format("指定時 {0:00}分", time);
						break;
					case 1:
						if(shift < 0)		buf = String.Format("以前{0}日 {1:00}時", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}日 {1:00}時", shift, time);
						else 				buf = String.Format("指定日 {0:00}時", time);
						break;
					case 2:
						if(shift < 0)		buf = String.Format("以前{0}月 {1:00}日", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}月 {1:00}日", shift, time);
						else 				buf = String.Format("指定月 {0:00}日", time);
						break;
					case 3:
						if(shift < 0)		buf = String.Format("以前{0}週 {1}曜日", shift, sWeekDay[time]);
						else if(shift > 0)	buf = String.Format("以後+{0}週 {1}曜日", shift, sWeekDay[time]);
						else 				buf = String.Format("指定週 {0}曜日", sWeekDay[time]);
						break;
					case 4:
						if(shift < 0)		buf = String.Format("以前{0}年 {1:00}月", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}年 {1:00}月", shift, time);
						else 				buf = String.Format("指定年 {0:00}月", time);
						break;
					default:
						buf = String.Format("Unknown time zone");
						break;
				}
			}
				/*
			else if(Tools.IsLangChinese()) 
			{
				char[] sWeekDay = { '日', '月', '火', '水', '木', '金', '土' };

				switch(zone) 
				{
					case 0:
						if(shift < 0)		buf = String.Format("以前{0}時 {1:00}分", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}時 {1:00}分", shift, time);
						else 				buf = String.Format("指定時 {0:00}分", time);
						break;
					case 1:
						if(shift < 0)		buf = String.Format("以前{0}日 {1:00}時", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}日 {1:00}時", shift, time);
						else 				buf = String.Format("指定日 {0:00}時", time);
						break;
					case 2:
						if(shift < 0)		buf = String.Format("以前{0}月 {1:00}日", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}月 {1:00}日", shift, time);
						else 				buf = String.Format("指定月 {0:00}日", time);
						break;
					case 3:
						if(shift < 0)		buf = String.Format("以前{0}週 {1}曜日", shift, sWeekDay[time]);
						else if(shift > 0)	buf = String.Format("以後+{0}週 {1}曜日", shift, sWeekDay[time]);
						else 				buf = String.Format("指定週 {0}曜日", sWeekDay[time]);
						break;
					case 4:
						if(shift < 0)		buf = String.Format("以前{0}年 {1:00}月", shift, time);
						else if(shift > 0)	buf = String.Format("以後+{0}年 {1:00}月", shift, time);
						else 				buf = String.Format("指定年 {0:00}月", time);
						break;
					default:
						buf = String.Format("Unknown time zone");
						break;
				}
			}
			*/
			else 
			{
				string[] sWeekDay = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

				switch(zone) 
				{
					case 0:
						if(shift < 0)		buf = String.Format("Prev {0}H {1:00}M", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}H {1:00}M", shift, time);
						else 				buf = String.Format("Set Hour {0:00}M", time);
						break;
					case 1:
						if(shift < 0)		buf = String.Format("Prev {0}D {1:00}H", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}D {1:00}H", shift, time);
						else 				buf = String.Format("Set Day {0:00}H", time);
						break;
					case 2:
						if(shift < 0)		buf = String.Format("Prev {0}M {1:00}D", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}M {1:00}D", shift, time);
						else 				buf = String.Format("Set Month {0:00}D", time);
						break;
					case 3:
						if(shift < 0)		buf = String.Format("Prev {0}W {1}Day", shift, sWeekDay[time]);
						else if(shift > 0)	buf = String.Format("Next +{0}W {1}Day", shift, sWeekDay[time]);
						else 				buf = String.Format("Set Week {0}Day", sWeekDay[time]);
						break;
					case 4:
						if(shift < 0)		buf = String.Format("Prev {0}Y {1:00}M", shift, time);
						else if(shift > 0)	buf = String.Format("Next +{0}Y {1:00}M", shift, time);
						else 				buf = String.Format("Set Year {0:00}M", time);
						break;
					default:
						buf = String.Format("Unknown time zone");
						break;
				}
			}
		}

		static void MakeCellTimeToText(out string buf, string zone, int time, int shift)
		{
			int type = -1;

			if(String.Compare(zone, "Min", true) == 0)		type = 0;
			else if(String.Compare(zone, "Hour", true) == 0)	type = 1;
			else if(String.Compare(zone, "Day", true) == 0)	type = 2;
			else if(String.Compare(zone, "Week", true) == 0)	type = 3;
			else if(String.Compare(zone, "Mon", true) == 0)	type = 4;
			else								type = -1;

			MakeCellTimeToText(out buf, time, shift, type);
		}

		static void MakeCellTimeInfoString(CELL_TIME time, out string buf)
		{
			string from;
			string to;

			MakeCellTimeToText(out from, time.zone, time.from, time.shift_from);
			MakeCellTimeToText(out to, time.zone, time.to, time.shift_to);
			buf = String.Format("{0}~{1}", from, to);
		}

		static void DrawCellAsTime(Graphics g, Font font, Brush brush, CELL_STRUCT cell, Rectangle r, StringFormat uFormat)
		{
			ParsedCellDisplay parsed = GetParsedCellDisplay(cell);
            SafeException.SafeDrawString(g, parsed == null ? "" : parsed.TimeText, font, brush, r, uFormat);
		}

		static void DrawCellText(Graphics g, Font font, Brush brush, REPORT_STRUCT report, CELL_STRUCT cell, Rectangle r, StringFormat uFormat, EnumViewMode viewmode)
		{
			if(cell.text == null)	return;
			if(cell.text.Length == 0)	return;

			if(cell.text[0] != '=') 
			{
                SafeException.SafeDrawString(g, cell.text, font, brush, r, uFormat);
				return;
			}

			if(viewmode == EnumViewMode.EDIT) 
			{
                SafeException.SafeDrawString(g, cell.text, font, brush, r, uFormat);
				return;
			}
			if(viewmode == EnumViewMode.RUN) 
			{
                SafeException.SafeDrawString(g, cell.text, font, brush, r, uFormat);
				return;
			}

			if(viewmode == EnumViewMode.COMMAND) 
			{
				DrawCellAsCommand(g, font, brush, cell, r, uFormat);
			}
			else if(viewmode == EnumViewMode.TAG) 
			{
				DrawCellAsTag(g,  font, brush, cell, r, uFormat);
			}
			else if(viewmode == EnumViewMode.TIME) 
			{
				DrawCellAsTime(g,  font, brush, cell, r, uFormat);
			}
			else 
			{
                SafeException.SafeDrawString(g, cell.text, font, brush, r, uFormat);
			}
		}

		static void DrawBorder(Graphics g, BORDER_STRUCT border, int x1, int y1, int x2, int y2, EnumViewMode viewmode, int pos)
		{
			System.Drawing.Color color;
			int thick;
			DashStyle type;
			Pen pen;

			if(border.type == 0) 
			{
				if(viewmode == EnumViewMode.RUN)	return;	// Run Mode 일때는 회색선을 그리지 않는다. 
				if(x1 != x2 && y1 != y2)	return;	// 사선
				color = System.Drawing.Color.LightGray;
				thick = 1;
				type = DashStyle.Dot;
			}
			else if(border.type == 6) 
			{
				pen = new Pen(border.color, 1);
				if(pos == 0 || pos == 2) 
				{	// left or right
					g.DrawLine(pen, x1-1, y1, x2-1, y2);
					g.DrawLine(pen, x1+1, y1, x2+1, y2);
				}
				else if(pos == 1 || pos == 3) 
				{	// top or bottom
					g.DrawLine(pen, x1, y1-1, x2, y2-1);
					g.DrawLine(pen, x1, y1+1, x2, y2+1);
				}
				else 
				{	// 사선
					g.DrawLine(pen, x1, y1, x2, y2);
				}
				return;
			}
			else 
			{
				color = border.color;
				thick = border.thick;
				if(border.type == 1)		type = DashStyle.Solid;
				else if(border.type == 2)	type = DashStyle.Dot;
				else 						type = DashStyle.Solid;
			}
	
			pen = new Pen(color, thick);
			pen.DashStyle = type;

			g.DrawLine(pen, x1, y1, x2, y2);
		}

		public static void DrawOneCell(Graphics g, REPORT_STRUCT report, int x1, int y1, int x2, int y2, CELL_STRUCT cell, EnumViewMode viewmode, bool cursor_zone)
		{
			Rectangle r = new Rectangle(x1, y1, x2-x1, y2-y1);
			Font font;
			StringFormat uFormat;

			if(viewmode == EnumViewMode.RUN)
				DrawClass.gcls(g, x1, y1, x2, y2, cell.bcolor);
			else
				DrawClass.gcls(g, x1, y1, x2, y2, cursor_zone ? cell.tcolor : cell.bcolor);

			float font_size = (float)GetRealView(report, cell.fontSize);
			font = GetCachedFont(cell.fontName, font_size, cell.fontStyle);
            uFormat = GetCellFormat(cell.cAlignHorz, cell.cAlignVert);

			Brush brush;
			
			if(viewmode == EnumViewMode.RUN)
				brush = GetCachedBrush(cell.tcolor);
			else
				brush = GetCachedBrush(cursor_zone ? cell.bcolor : cell.tcolor);

			DrawCellText(g, font, brush, report, cell, r, uFormat, viewmode);

			DrawBorder(g, cell.border[0], x1, y1, x1, y2, viewmode, 0);
			DrawBorder(g, cell.border[1], x1, y1, x2, y1, viewmode, 1);
			DrawBorder(g, cell.border[2], x2, y1, x2, y2, viewmode, 2);
			DrawBorder(g, cell.border[3], x1, y2, x2, y2, viewmode, 3);
			DrawBorder(g, cell.border[4], x1, y1, x2, y2, viewmode, 4);
			DrawBorder(g, cell.border[5], x2, y1, x1, y2, viewmode, 5);
		}

		void DrawOneTable(Graphics g, Rectangle rectPaper, REPORT_STRUCT report, TABLE_STRUCT table, ref int current_y)
		{
			int x;
			int posx, posy;
			CELL_STRUCT cell = null;
			int cell_pos;
			int x2, y2;
			//Rectangle r = new Rectangle();
			int max_height;
            int[] widthCache = new int[table.cellBuf.Count];
            int[] heightCache = new int[table.cellBuf.Count];
            int[] rowMaxHeightCache = new int[table.cell_y];

            BuildCellViewSizeCache(report, table, widthCache, heightCache, rowMaxHeightCache);

			cell_pos = 0;
            int tableStartY = current_y + GetRealView(table.gab_top);
			current_y = tableStartY;
			for(posy = 0; posy < table.cell_y; posy++) 
			{
		
				if(current_y >= rectPaper.Bottom) 
					return;

				max_height = rowMaxHeightCache[posy];
				if(current_y+max_height < 0) 
				{
					cell = (CELL_STRUCT)table.cellBuf[posy*table.cell_x];
					current_y+=GetRealView(cell.height);
					continue;
				}

				cell_pos = posy*table.cell_x;
		
				for(posx = 0, x = GetCanvasOriginX()+GetRealView(table.gab_left)-nHorScrollPos; posx < table.cell_x; posx++, cell_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[cell_pos];
					x2 = x+widthCache[cell_pos];
					y2 = current_y+heightCache[cell_pos];
					if(x2 >= 0 && cell.cGroup != 2) 
					{
						DrawOneCell(g, report, x, current_y, x2, y2, cell, workView.nViewMode, ReportLib.IsCursorZone(report, table.no, posx, posy));
					}

					x+=GetRealView(cell.width);
					if(x > rectPaper.Right)	break;
				}

				current_y+=GetRealView(cell.height);
			}
		}

        private static string MakeColumnHeaderText(int cell_x)
        {
            if (cell_x < 26)
            {
                return String.Format("{0}", (char)(cell_x + 'A'));
            }

            return String.Format("{0}{1}", (char)((cell_x / 26) - 1 + 'A'), (char)((cell_x % 26) + 'A'));
        }

        private void DrawTableHeaders(Graphics g, Rectangle rectPaper, REPORT_STRUCT report, TABLE_STRUCT table, int tableStartY, int[] widthCache, int[] heightCache)
        {
        }

        private bool TrySelectByHeaderClick(MouseEventArgs e)
        {
            return false;
        }

        private static Font GetCachedFont(string fontName, float fontSize, FontStyle fontStyle)
        {
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = "Gulim";
            }

            string key = string.Format("{0}|{1}|{2}", fontName, fontSize, (int)fontStyle);
            Font font;
            if (s_fontCache.TryGetValue(key, out font))
            {
                return font;
            }

            font = new Font(fontName, fontSize <= 0 ? 8f : fontSize, fontStyle);
            s_fontCache[key] = font;
            return font;
        }

        private static SolidBrush GetCachedBrush(Color color)
        {
            SolidBrush brush;
            if (s_brushCache.TryGetValue(color.ToArgb(), out brush))
            {
                return brush;
            }

            brush = new SolidBrush(color);
            s_brushCache[color.ToArgb()] = brush;
            return brush;
        }

        private static StringFormat[,] CreateCellFormats()
        {
            StringFormat[,] formats = new StringFormat[3, 4];
            for (int horz = 0; horz < 3; horz++)
            {
                for (int vert = 0; vert < 4; vert++)
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = horz == 0 ? StringAlignment.Near : (horz == 1 ? StringAlignment.Center : StringAlignment.Far);
                    if (vert == 0)
                    {
                        format.LineAlignment = StringAlignment.Near;
                        format.FormatFlags |= StringFormatFlags.NoWrap;
                    }
                    else if (vert == 1)
                    {
                        format.LineAlignment = StringAlignment.Center;
                        format.FormatFlags |= StringFormatFlags.NoWrap;
                    }
                    else if (vert == 2)
                    {
                        format.LineAlignment = StringAlignment.Far;
                        format.FormatFlags |= StringFormatFlags.NoWrap;
                    }
                    formats[horz, vert] = format;
                }
            }

            return formats;
        }

        private static StringFormat GetCellFormat(int horzAlign, int vertAlign)
        {
            int horz = horzAlign;
            int vert = vertAlign;
            if (horz < 0 || horz > 2) horz = 0;
            if (vert < 0 || vert > 3) vert = 0;
            return s_cellFormats[horz, vert];
        }

		public static bool InsertNewTable(REPORT_STRUCT report, int insert_pos, int cell_x, int cell_y)
		{
			TABLE_STRUCT new_table = new TABLE_STRUCT();

			ReportLib.FillDefaultTable(new_table, cell_x, cell_y);
			new_table.no = insert_pos;

			report.tableBuf.Insert(insert_pos, new_table);

			TABLE_STRUCT table = new TABLE_STRUCT();

			// 테이블 번호를 다시지정한다.
			for(int i = insert_pos; i < report.tableBuf.Count; i++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[i];
				table.no = i;
			}

			return true;
		}

		public static void MakeCellIdString(out string str, int table_no, int cell_x, int cell_y)
		{
			string imsi;

			if(cell_x < 26) 
			{
				imsi = String.Format("{0}", (char)(cell_x+'A'));
			}
			else 
			{
				imsi = String.Format("{0}{1}", (char)((cell_x/26)-1+'A'), (char)((cell_x%26)+'A'));
			}

			str = String.Format("{0}{1}{2}", table_no, imsi, cell_y);
		}

        public static void MakeCellRangeString(out string str, REPORT_STRUCT report)
        {
            int x1, y1, x2, y2;
            ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

            string startId;
            string endId;
            MakeCellIdString(out startId, report.cursor_table, x1, y1);
            MakeCellIdString(out endId, report.cursor_table, x2, y2);

            if (startId == endId)
            {
                str = startId;
            }
            else
            {
                str = startId + ":" + endId;
            }
        }

		public void DrawCellInfo(REPORT_STRUCT report)
		{
			string buf;
			string imsi;
			string imsi2;

			if(report.TableCount == 0) 
			{
				buf = "Empty report";
			}
			else 
			{
				TABLE_STRUCT table;
				CELL_STRUCT cell;

				table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

				cell = (CELL_STRUCT)table.cellBuf[report.cursor_y1*table.cell_x+report.cursor_x1];
				if(report.cursor_x1 == report.cursor_x2 &&
					report.cursor_y1 == report.cursor_y2) 
				{
					MakeCellIdString(out imsi, report.cursor_table, report.cursor_x1, report.cursor_y1);
					buf = String.Format("{0}[{1}]", imsi, cell.text);
				}
				else 
				{
					MakeCellIdString(out imsi, report.cursor_table, report.cursor_x1, report.cursor_y1);
					MakeCellIdString(out imsi2, report.cursor_table, report.cursor_x2, report.cursor_y2);
					buf = String.Format("{0}:{1}", imsi2, imsi);
				}
			}
	
			if(statusBarPanelMain != null)
				statusBarPanelMain.Text = buf;

            UpdateQuickCellEditBar(report);
		}

		public void SetCursorPosition(REPORT_STRUCT report, int table_no, int x, int y)
		{
			report.cursor_table = table_no;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[table_no];
			if(x < 0)	x = 0;
			if(y < 0)	y = 0;
		
			if(x >= table.cell_x)	x = table.cell_x-1;
			if(y >= table.cell_y)	y = table.cell_y-1;

			report.cursor_x1 = x;
			report.cursor_y1 = y;
			report.cursor_x2 = x;
			report.cursor_y2 = y;

			DrawCellInfo(report);	
		}

		public void SetCursorPosition(REPORT_STRUCT report, int table_no, int x1, int y1, int x2, int y2)
		{
			report.cursor_table = table_no;
			report.cursor_x1 = x1;
			report.cursor_y1 = y1;
			report.cursor_x2 = x2;
			report.cursor_y2 = y2;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[table_no];
			if(report.cursor_x1 < 0)	report.cursor_x1 = 0;
			if(report.cursor_y1 < 0)	report.cursor_y1 = 0;
			if(report.cursor_x2 < 0)	report.cursor_x2 = 0;
			if(report.cursor_y2 < 0)	report.cursor_y2 = 0;

			if(report.cursor_x1 >= table.cell_x)	report.cursor_x1 = table.cell_x-1;
			if(report.cursor_y1 >= table.cell_y)	report.cursor_y1 = table.cell_y-1;
			if(report.cursor_x2 >= table.cell_x)	report.cursor_x2 = table.cell_x-1;
			if(report.cursor_y2 >= table.cell_y)	report.cursor_y2 = table.cell_y-1;

			DrawCellInfo(report);
		}

		public bool bChangedFlag = false;

		public void SetChangeFlag()
		{
			bChangedFlag = true;
		}

		int GetScrollHorzSize(REPORT_STRUCT report)
		{
			int x;
			int posx;
			CELL_STRUCT cell;
			TABLE_STRUCT table;
			int table_no;
			int max = 100;

			if(report == null)	return max;

			for(table_no = 0; table_no < report.TableCount; table_no++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];

				x = GetRealView(table.gab_left) + GetCanvasOriginX() + START_X;

				for(posx = 0; posx < table.cell_x; posx++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[posx];
					x+=GetRealView(cell.width);
				}

				if(x > max)	max = x;
			}

			return max;
		}

		int GetScrollVertSize(REPORT_STRUCT report)
		{
			int y = GetCanvasOriginY() + START_Y;
			int posy;
			CELL_STRUCT cell;
			TABLE_STRUCT table;
			int table_no;
	
			if(report == null)	return 100;

			for(table_no = 0; table_no < report.TableCount; table_no++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[table_no];

				y += GetRealView(table.gab_top);

				for(posy = 0; posy < table.cell_y; posy++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[posy*table.cell_x];
					y+=GetRealView(cell.height);
				}
			}

			return y;
		}

		public void ScrollUpdate()
		{
			REPORT_STRUCT report = GetReportStruct();
			if (report == null) return;

			nHorScrollHap = GetScrollHorzSize(report);
			nVerScrollHap = GetScrollVertSize(report);

			if(nHorScrollHap < 0)		nHorScrollHap = 0;
			if(nVerScrollHap < 0)		nVerScrollHap = 0;
			if(nHorScrollHap > 30000)	nHorScrollHap = 30000;
			if(nVerScrollHap > 30000)	nVerScrollHap = 30000;

			if(nHorScrollPos > nHorScrollHap)	nHorScrollPos = nHorScrollHap;
			if(nVerScrollPos > nVerScrollHap)	nVerScrollPos = nVerScrollHap;

            if (panelCanvas != null)
            {
                panelCanvas.AutoScrollMinSize = new Size(nHorScrollHap, nVerScrollHap);
            }
            else
            {
			    this.AutoScrollMargin = new Size(nHorScrollHap, nVerScrollHap);
            }
		}

		public void menuItemInsertTable_Click()
		{
			FormDialogInsertTable dialog = new FormDialogInsertTable();

			if(dialog.ShowDialog(this) != DialogResult.OK)	return;
	
			REPORT_STRUCT report = GetReportStruct();
			int insert_pos;
            
			ReportEditorUndo.UndoSave(this, "표 삽입");

			if(dialog.m_where == 0) 
			{	// 처음에 삽입
				insert_pos = 0;
			}
			else if(dialog.m_where == 1) 
			{	// 중간에 삽입
				insert_pos = report.cursor_table;
			}
			else 
			{	// 마지막에 삽입
				insert_pos = report.TableCount;
			}

			InsertNewTable(report, insert_pos, ConvertTool.ToInt32(dialog.numericUpDownCountX.Value), ConvertTool.ToInt32(dialog.numericUpDownCountY.Value));

			SetCursorPosition(report, insert_pos, 0, 0);
			this.Invalidate();
			SetChangeFlag();
			ScrollUpdate();
		}

		void MessageBoxCannotRunMode()
		{
			if(Tools.IsLangKorean()) 
			{
				MessageBox.Show("실행 모드에서는 사용할 수 없는 메뉴입니다.", "RUN Mode");
			}
			else if(Tools.IsLangChinese()) 
			{
				MessageBox.Show("在运行模式不可使用的菜单。", "运行模式");
			}
			else 
			{
				MessageBox.Show("Can't use This menu on RUN mode.", "RUN Mode");
			}
		}

		void DeleteTable()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				MessageBoxCannotRunMode();
				return;
			}

			REPORT_STRUCT report = GetReportStruct();

			if(report.cursor_table >= report.TableCount)	return;

			if(Tools.IsLangKorean()) 
			{
				ReportEditorUndo.UndoSave(this, "표 삭제");
			}
			else if(Tools.IsLangChinese()) 
			{
				ReportEditorUndo.UndoSave(this, "删除表格");
			}
			else 
			{
				ReportEditorUndo.UndoSave(this, "Delete Table");
			}

			report.tableBuf.RemoveAt(report.cursor_table);

			TABLE_STRUCT table;
			// 테이블 번호를 다시 지정한다. 테이블 번호를 다시 지정하지 않으면 삭제 후 표의 선택이 이상해 진다.
			for(int i = 0; i < report.tableBuf.Count; i++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[i];
				table.no = i;
			}

			if(report.cursor_table < report.TableCount) 
			{
				//SetCursorPosition(report, report.cursor_table, report.cursor_x1, report.cursor_y1, report.cursor_x2, report.cursor_y2);
				SetCursorPosition(report, report.cursor_table, 0, 0);
			}

			this.Invalidate();
			SetChangeFlag();
			ScrollUpdate();
		}

		public void menuItemDeleteTable_Click()
		{
			DeleteTable();
		}	

		public void menuItemInsertRow_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				MessageBoxCannotRunMode();
				return;
			}

			REPORT_STRUCT report = GetReportStruct();

			if(report.cursor_table >= report.TableCount)	return;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
			ArrayList cell_new = new ArrayList(); //CELL_STRUCT[table.cell_x*(table.cell_y+1)];

			ReportEditorUndo.UndoSave(this, "행 삽입");

			int source_pos = 0;
			int target_pos = 0;
			int x, y;
			CELL_STRUCT cell;

			int x1, y1, x2, y2;
			ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

			table.cell_y++;

			for(y = 0; y < y1; y++) 
			{
				for(x = 0; x < table.cell_x; x++, source_pos++, target_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
					cell_new.Add(cell);
				}
			}

			for(x = 0; x < table.cell_x; x++, target_pos++) 
			{
				cell = (CELL_STRUCT)Tools.CopyObject(table.cellBuf[target_pos]);
				cell.x = x;
				cell.y = y;
				cell.text = "";
				cell_new.Add(cell);
			}

			for(; y < table.cell_y-1; y++) 
			{
				for(x = 0; x < table.cell_x; x++, source_pos++, target_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y+1;
					cell.nGroupY += 1;
					cell_new.Add(cell);
				}
			}

			table.cellBuf = cell_new;

			this.Invalidate();
			SetChangeFlag();
			DrawCellInfo(report);
			ScrollUpdate();
		}

		public void menuItemDeleteRow_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN)
			{
				MessageBoxCannotRunMode();
				return;
			}

			REPORT_STRUCT report = GetReportStruct();

			if(report.cursor_table >= report.TableCount)	return;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			int x1, y1, x2, y2;
			ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

			if(y1 <= 0 && y2 >= table.cell_y-1) 
			{	// 모든 행을 선택했다. 테이블을 삭제한다.
				DeleteTable();
				return;			
			}

			int gab = y2-y1+1;

			ArrayList cell_new = new ArrayList();

			if(Tools.IsLangKorean()) 
			{
				ReportEditorUndo.UndoSave(this, "행 삭제");
			}
			else if(Tools.IsLangChinese()) 
			{
				ReportEditorUndo.UndoSave(this, "删除行");
			}
			else 
			{
				ReportEditorUndo.UndoSave(this, "Delete Record");
			}

			table.cell_y-=gab;

			int source_pos = 0;
			int target_pos = 0;
			int x, y;
			CELL_STRUCT cell;

			for(y = 0; y < y1; y++) 
			{
				for(x = 0; x < table.cell_x; x++, source_pos++, target_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
                    if (cell.nGroupY >= table.cell_y)
                    {
                        cell.nGroupY = table.cell_y - 1;
                    }
					cell_new.Add(cell);
				}
			}

			source_pos+=(table.cell_x*gab);

			for(; y < table.cell_y; y++) 
			{
				for(x = 0; x < table.cell_x; x++, source_pos++, target_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
					cell.nGroupY -= gab;
                    if (cell.nGroupY >= table.cell_y)
                    {
                        cell.nGroupY = table.cell_y - 1;
                    }
					cell_new.Add(cell);
					
				}
			}

			table.cellBuf = cell_new;

			SetCursorPosition(report, report.cursor_table, x1, y1);

			Invalidate();
			ScrollUpdate();
			SetChangeFlag();	
		}

		public void menuItemInsertColumn_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				MessageBoxCannotRunMode();
				return;
			}

			REPORT_STRUCT report = GetReportStruct();

			if(report.cursor_table >= report.TableCount)	return;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			ArrayList cell_new = new ArrayList();

			if(Tools.IsLangKorean()) 
			{
				ReportEditorUndo.UndoSave(this, "열 삽입");
			}
			else if(Tools.IsLangChinese()) 
			{
				ReportEditorUndo.UndoSave(this, "插入列");
			}
			else 
			{
				ReportEditorUndo.UndoSave(this, "Insert Field");
			}

			int source_pos = 0;
			int x, y;
			CELL_STRUCT cell;

			int x1, y1, x2, y2;
			ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

			table.cell_x++;

			for(y = 0; y < table.cell_y; y++) 
			{
				for(x = 0; x < x1; x++, source_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
					cell_new.Add(cell);
				}
				cell = (CELL_STRUCT)Tools.CopyObject(table.cellBuf[source_pos]);
				//if(cell.cGroup == 1) 
				//{
				//	cell.cGroup = 0;
				//}

				cell.text = ""; 
				cell.x = x;
				cell.y = y;
				cell_new.Add(cell);
				x++;

				for(; x < table.cell_x; x++, source_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
					cell.nGroupX += 1;
					cell_new.Add(cell);
				}
			}

			table.cellBuf = cell_new;

			Invalidate();
			SetChangeFlag();
			DrawCellInfo(report);	
			ScrollUpdate();
		}

		public void menuItemDeleteColumn_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				MessageBoxCannotRunMode();
				return;
			}

			REPORT_STRUCT report = GetReportStruct();

			if(report.cursor_table >= report.TableCount)	return;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			int x1, y1, x2, y2;
			ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

			if(x1 <= 0 && x2 >= table.cell_x-1) 
			{	// 모든 열을 선택했다. 테이블을 삭제한다.
				DeleteTable();
				return;			
			}

			int gab = x2-x1+1;

			//CELL_STRUCT cell_new = new CELL_STRUCT[(table.cell_x-gab)*table.cell_y];
			ArrayList cell_new = new ArrayList();

			if(Tools.IsLangKorean()) 
			{
				ReportEditorUndo.UndoSave(this, "열 삭제");
			}
			else if(Tools.IsLangChinese()) 
			{
				ReportEditorUndo.UndoSave(this, "删除列");
			}
			else 
			{
				ReportEditorUndo.UndoSave(this, "Delete field");
			}

			table.cell_x-=gab;

			int source_pos = 0;
			int target_pos = 0;
			int x, y;
			CELL_STRUCT cell;

			for(y = 0; y < table.cell_y; y++) 
			{
				for(x = 0; x < x1; x++, source_pos++, target_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
                    if (cell.nGroupX >= table.cell_x)
                    {
                        cell.nGroupX = table.cell_x - 1;
                    }
					cell_new.Add(cell);
				}
				source_pos+=gab;
				for(; x < table.cell_x; x++, source_pos++, target_pos++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[source_pos];
					cell.x = x;
					cell.y = y;
					cell.nGroupX -= gab;
                    if (cell.nGroupX >= table.cell_x)
                    {
                        cell.nGroupX = table.cell_x - 1;
                    }
					cell_new.Add(cell);
				}
			}
	
			table.cellBuf = cell_new;

			SetCursorPosition(report, report.cursor_table, x1, y1);

			Invalidate();
			SetChangeFlag();	
			ScrollUpdate();
		}

		public void menuItemAlignToLeft_Click()
		{
            ReportEditorUndo.UndoSave(this, "Align Left");
			SelectedCell.SelectedCellSetAlignHorz(GetReportStruct(), 0);
			this.Invalidate();
		}

		public void menuItemAlignToCenter_Click()
		{
            ReportEditorUndo.UndoSave(this, "Align Center");
			SelectedCell.SelectedCellSetAlignHorz(GetReportStruct(), 1);
			this.Invalidate();
		}

		public void menuItemAlignToRight_Click()
		{
            ReportEditorUndo.UndoSave(this, "Align Right");
			SelectedCell.SelectedCellSetAlignHorz(GetReportStruct(), 2);
			this.Invalidate();
		}

		private void FormReportChild_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (panelCanvas != null)
            {
                panelCanvas.Focus();
            }
            this.Select();	// 이 부분이 없으면 MDI창이 하나일 때 솔루션을 선택한 후 키보드 포커스를 MDI로 위치할 수가 없다.

			if(e.Button != MouseButtons.Left)	return;
            if (TrySelectByHeaderClick(e)) return;
			ReportEditorMouse.MouseDown(this, e);
		}

		private void FormReportChild_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ReportEditorMouse.MouseMove(this, e);
		}

		private void FormReportChild_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)	return;
			ReportEditorMouse.MouseUp(this, e);
		}

		private void FormReportChild_DoubleClick(object sender, System.EventArgs e)
		{
			ReportEditorMouse.OnLButtonDblClk(this);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) 
		{ 

		} 

		public void menuItemInsertBasicData_Click()
		{
			ReportInsert.InsertAutoData(this);	
		}

		public void menuItemInsertFunction_Click()
		{
			ReportInsert.InsertFunctionData(this);
		}

		public void menuItemEditCut_Click()
		{
			ReportCopyPaste.OnEditCut(this);
		}

		public void menuItemEditCopy_Click()
		{
			ReportCopyPaste.OnEditCopy(this);
		}

		public void menuItemEditPaste_Click()
		{
			ReportCopyPaste.OnEditPaste(this);
		}

		public void menuItemEditPasteAsNewTable_Click()
		{
			ReportCopyPaste.OnEditPasteNewTable(this);
		}

		public void menuItemEditDelete_Click()
		{
			ReportCopyPaste.OnEditDel(this);
		}

		public void menuItemEditHeader_Click()
		{
			FormHeaderEditor head = new FormHeaderEditor(reportEdit, 0);

			head.ShowDialog(this);
		}

		public void menuItemEditFooter_Click()
		{
			FormHeaderEditor head = new FormHeaderEditor(reportEdit, 1);

			head.ShowDialog(this);
		}

		public void menuItemFormCellTextColor_Click()
		{
			ColorDialog dialog = new ColorDialog();
			REPORT_STRUCT report = GetReportStruct();
            
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				SelectedCell.SelectedCellSetTextColor(report, dialog.Color);
				Invalidate();
			}	
		}

		public void menuItemFormCellBackColor_Click()
		{
			ColorDialog dialog = new ColorDialog();
			REPORT_STRUCT report = GetReportStruct();
            
			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				SelectedCell.SelectedCellSetBackColor(report, dialog.Color);
				Invalidate();
			}	
		}

		public void menuItemFormCellBorder_Click()
		{
			FormDialogConfigCellBorder dialog = new FormDialogConfigCellBorder();
			REPORT_STRUCT report = GetReportStruct();

			dialog.border = SelectedCell.SelectedCellGetBorder(report);

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				SelectedCell.SelectedCellSetBorder(report, dialog.border);
				this.Invalidate();
			}
		}

		public void menuItemFormCellFont_Click()
		{
			FontDialog dialog = new FontDialog();
			REPORT_STRUCT report = GetReportStruct();

			dialog.Font = SelectedCell.SelectedCellGetFont(report);

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				SelectedCell.SelectedCellSetFont(report, dialog.Font);
				this.Invalidate();
			}
		}

		public void menuItemFormCellSize_Click()
		{
			FormDialogConfigCellSize dialog = new FormDialogConfigCellSize();

			dialog.SetItem(GetReportStruct());

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				dialog.GetItem(this);
			}
		}

		public void menuItemFormCellAlign_Click()
		{
			FormDialogConfigCellAlign dialog = new FormDialogConfigCellAlign();

			dialog.SetItem(this);

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				dialog.GetItem(this);
			}
		}

		public void menuItemFormCellDisplayFormat_Click()
		{
			FormDialogConfigCellFormat dialog = new FormDialogConfigCellFormat();

			dialog.SetItem(this);

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				dialog.GetItem(this);
			}
		}

		public void menuItemFormCellGroup_Click()
		{
			ReportGroup.CellGroup(this);			
		}

		public void menuItemFormCellUngroup_Click()
		{
			ReportGroup.CellUnGroup(this);			
		}

		public void menuItemFormCellChangeCommand_Click()
		{
			FormDialogInsertAutoData dialog = new FormDialogInsertAutoData();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				if(Tools.IsLangKorean()) 
				{
					ReportEditorUndo.UndoSave(this, "셀 명령어 바꿈");
				}
				else if(Tools.IsLangChinese()) 
				{
					ReportEditorUndo.UndoSave(this, "更改单元格命令");
				}
				else 
				{
					ReportEditorUndo.UndoSave(this, "Change of Cell Command");
				}

				SelectedCell.SelectedCellSetCommand(GetReportStruct(), dialog.sSelectedCommand);
				this.SetChangeFlag();
				Invalidate();
			}
		}

		public void menuItemFormCellChangeTag_Click()
		{
			FormSelectTag dialog = new FormSelectTag();
			REPORT_STRUCT report = GetReportStruct();

			dialog.bUseTagAI = true;
			dialog.bUseTagDI = true;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				if(Tools.IsLangKorean()) 
				{
					ReportEditorUndo.UndoSave(this, "셀 태그 바꿈");
				}
				else if(Tools.IsLangChinese()) 
				{
					ReportEditorUndo.UndoSave(this, "更改单元格标记");
				}
				else 
				{
					ReportEditorUndo.UndoSave(this, "Change of Cell Tag");
				}

				SelectedCell.SelectedCellSetTag(report, dialog.sTag);
				SetChangeFlag();
				Invalidate();
				DrawCellInfo(report);
			}
		}

		public void menuItemFormCellChangeTime_Click()
		{
			PropertySheetPublic sheet = new PropertySheetPublic();

			PropertySelectTime time = new PropertySelectTime();
			
			CELL_TIME t = new CELL_TIME();

			time.SetTime(t);
			REPORT_STRUCT report = GetReportStruct();

			sheet.AddPage(time);

			if(sheet.ShowDialog(this) == DialogResult.OK)
			{
				if(Tools.IsLangKorean()) 
				{
					ReportEditorUndo.UndoSave(this, "셀 시간 바꿈");
				}
				else if(Tools.IsLangChinese()) 
				{
					ReportEditorUndo.UndoSave(this, "更改单元格时间");
				}
				else 
				{
					ReportEditorUndo.UndoSave(this, "Change of Cell Time");
				}

				time.GetTime(t);
				SelectedCell.SelectedCellSetTime(report, t);
				SetChangeFlag();
				Invalidate();
				DrawCellInfo(report);
			}
		}

		public void menuItemFormCellChangeText_Click()
		{
			REPORT_STRUCT report = GetReportStruct();
			string text="";

			SelectedCell.SelectedCellGetText(report, ref text);

			FormDialogTextEdit dialog = new FormDialogTextEdit();

			dialog.textBoxCellText.Text = text;

			if(dialog.ShowDialog(this) != DialogResult.OK)	return;

			if(Tools.IsLangKorean()) 
			{
				ReportEditorUndo.UndoSave(this, "셀 텍스트 바꿈");
			} 
			else if(Tools.IsLangChinese()) 
			{
				ReportEditorUndo.UndoSave(this, "更改单元格文本");
			}
			else 
			{
				ReportEditorUndo.UndoSave(this, "Change Cell Text");
			}

			text = dialog.textBoxCellText.Text;

			SelectedCell.SelectedCellSetText(report, text);
			SetChangeFlag();
			Invalidate();
			DrawCellInfo(report);	
		}

		public void menuItemViewZoomIn_Click()
		{
			REPORT_STRUCT report;
			if(this.reportEdit != null)	report = reportEdit;
			else						report = reportRun;

			int Rate = report.wOpticRate; 
	
			if(Rate < 10)		Rate = 10;	
			else if(Rate < 25)	Rate = 25;	
			else if(Rate < 50)	Rate = 50;
			else if(Rate < 75)	Rate = 75;	
			else if(Rate < 100)	Rate = 100;
			else if(Rate < 150)	Rate = 150;
			else if(Rate < 200)	Rate = 200;
			else								return;

			report.wOpticRate = Rate;
			if(this.reportRun != null)	reportRun.wOpticRate = Rate;

			SetTitle();
			ScrollUpdate();

			Invalidate();
		}

		public void menuItemViewZoomOut_Click()
		{
			REPORT_STRUCT report;
			if(this.reportEdit != null)	report = reportEdit;
			else						report = reportRun;

			int Rate = report.wOpticRate;

			if(Rate > 200)		Rate = 200;
			else if(Rate > 150)	Rate = 150;
			else if(Rate > 100)	Rate = 100;
			else if(Rate > 75)	Rate = 75;
			else if(Rate > 50)	Rate = 50;
			else if(Rate > 25)	Rate = 25;
			else if(Rate > 10)	Rate = 10;
			else				return;

			report.wOpticRate = Rate;
			if(reportRun != null)	reportRun.wOpticRate = Rate;

			SetTitle();
			ScrollUpdate();

			Invalidate();
		}

		public void menuItemViewAsEditMode_Click()
		{
			this.workView.nViewMode = EnumViewMode.EDIT;
			ScrollUpdate();
			Invalidate();
		}

		public async void menuItemViewAsRunMode_Click()
		{
			MakeRunReport make = new MakeRunReport();
			reportRun = await make.Make(reportEdit, 0);
			this.workView.nViewMode = EnumViewMode.RUN;
			ScrollUpdate();
			Invalidate();
		}

		public void menuItemViewAsCommandMode_Click()
		{
			this.workView.nViewMode = EnumViewMode.COMMAND;
			ScrollUpdate();
			Invalidate();
		}

		public void menuItemViewAsTagMode_Click()
		{
			this.workView.nViewMode = EnumViewMode.TAG;
			ScrollUpdate();
			Invalidate();
		}

		public void menuItemViewAsTimeMode_Click()
		{
			this.workView.nViewMode = EnumViewMode.TIME;
			ScrollUpdate();
			Invalidate();
		}


		public void menuItemConfigPaperColor_Click()
		{
			REPORT_STRUCT report = GetReportStruct();

			ColorDialog dialog = new ColorDialog();
			
			dialog.Color = report.lColorPaper;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				report.lColorPaper = dialog.Color;
				SetChangeFlag();
				Invalidate();
			}	
		}

		public void menuItemConfigPrintTime_Click()
		{
			FormDialogConfigPrintTime dialog = new FormDialogConfigPrintTime();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				ReCalcRunReport();
			}
		}

		public void menuItemConfigPrintCycle_Click()
		{
			
		}

		public void menuItemConfigMinListTime_Click()
		{
			FormDialogConfigMinListTime dialog = new FormDialogConfigMinListTime();

            dialog.ShowDialog(this);
		}


		public bool menuItemFileSave_Click()
		{
			return FileSave(reportEdit);
		}

		bool FileSave(REPORT_STRUCT report, string filename)
		{
			if(!ReportFile.Save(report, filename)) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("파일을 저장할 수 없습니다.", filename);
				else
					MessageBox.Show("Can't save the file.", filename);

				return false;
			}
			else 
			{
				this.bChangedFlag = false;
				this.sFilename = filename;//Path.GetFileName(filename);
				SetTitle(); // title을 다시 그린다.
			}

			return true;
		}

		bool FileSave(REPORT_STRUCT report)
		{
			string ext = Path.GetExtension(this.sFilename);
			if(String.Compare(ext, ".rpt", true) == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("파일을 RPTX 형태로 저장해야 합니다.", "파일변환");
				else
					MessageBox.Show("This file will be save to RPTX format.", "File conversion");

				return SaveAs(report, Path.ChangeExtension(this.sFilename, ".rptx"));
			}

			string name = Path.GetFileNameWithoutExtension(this.sFilename);
			if(String.Compare(name, 0, "noname", 0, 6, true) == 0) 
			{
				return SaveAs(report, Path.ChangeExtension(this.sFilename, ".rptx"));
			}
						
			string path = this.sFilename;
			return FileSave(report, path);
		}

		public delegate void DelegateFormSolutionReLoad();
		public static DelegateFormSolutionReLoad lpfnFormSolutionReLoad = null;

		bool SaveAs(REPORT_STRUCT report, string init_file)
		{
			SaveFileDialog dialog = new SaveFileDialog();

			dialog.Filter = "Report files (*.rptx)|*.rptx";
			string init = TotalConfig.sDirWorkProject+"\\Report";
			if(!Directory.Exists(init))
			{
				Directory.CreateDirectory(init);
			}
			dialog.InitialDirectory = init;

			if(init_file != null)
			{
				dialog.FileName = init_file;
			}

            if (dialog.ShowDialog(this) == DialogResult.OK) 
			{
				bool retn = FileSave(report, dialog.FileName);
				if(retn)	// 파일 이름이 바뀌었을 수도 있다.
				{
					if(lpfnFormSolutionReLoad != null)
						lpfnFormSolutionReLoad();
				}
				return retn;
			}

			return false;
		}

		public void menuItemFileSaveAs_Click()
		{
			SaveAs(GetReportStruct(), null);
		}

		public void menuItemFileSaveResult_Click()
		{
			if(workView.nViewMode != EnumViewMode.RUN)
			{
				string msg;
				
				if(Tools.IsLangKorean()) 
					msg = String.Format("실행 모드에서만 [결과를 파일로 저장] 기능을 사용할 수 있습니다.");
				else if(Tools.IsLangChinese()) 
					msg = String.Format("[结果保存为文件]功能只能在运行模式使用。");
				else
					msg = String.Format("You can save to file only RUN mode.");

				MessageBox.Show(msg, "Cannot save");
				return;
			}

			SaveFileDialog dialog = new SaveFileDialog();

			dialog.Filter = "Report Files (*.rptx)|*.rptx|CSV (쉼표로 분리)|*.csv";

            dialog.FilterIndex = TotalConfig.LoadRegAutoBaseConfig(Application.UserAppDataRegistry, "ViewMain", "Report", "FilterIndex", 1);

			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
                string ext = Path.GetExtension(dialog.FileName);

                if (String.Compare(ext, ".rptx", true) == 0)
                {
                    ReportFile.Save(reportRun, dialog.FileName);
                }
                else
                {
                    ReportFile.SaveToCSV(reportRun, dialog.FileName);
                }

                TotalConfig.SaveRegAutoBaseConfig(Application.UserAppDataRegistry, "ViewMain", "Report", "FilterIndex", dialog.FilterIndex);
			}

            
		}

		public async void menuItemFilePrint_Click()
		{
			ReportPrint print = new ReportPrint();
			
			REPORT_STRUCT run = await MakeRunReportPublic(reportEdit, 0, this.sFilename);
			print.PrintGo(run, this.sFilename);
		}

		public async void menuItemFilePreview_Click()
		{
			ReportPrint print = new ReportPrint();

			REPORT_STRUCT run = await MakeRunReportPublic(reportEdit, 0, this.sFilename);
			print.PreviewGo(run, this.sFilename);
		}

		public void menuItemFileConfigPaper_Click()
		{
			FormDialogPaperSetup dialog = new FormDialogPaperSetup();
	
			dialog.SetItem(reportEdit);
			if(dialog.ShowDialog(this) == DialogResult.OK)
			{
				dialog.GetItem(reportEdit);
			}
		}

		public void menuItemFilePrintMultiDate_Click()
		{
			FormDialogConfigPrintMultiDate dialog = new FormDialogConfigPrintMultiDate();

			dialog.ShowDialog(this);
		}

		public void menuItemFileInformation_Click()
		{
			FormDialogFileInformation dialog = new FormDialogFileInformation();

			dialog.textBoxDescription.Text = reportEdit.description;
			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				reportEdit.description = dialog.textBoxDescription.Text;
				this.SetChangeFlag();
			}
		}

		private void FormReportChild_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(bChangedFlag) 
			{

			}
		}

		public async void ReCalcRunReport() 
		{
			if(workView.nViewMode != EnumViewMode.RUN)	return;

			reportRun = await MakeRunReportPublic(reportEdit, 0, sFilename);

			this.ScrollUpdate();
			this.Invalidate();
		}

		//-----------------------------------------------------------------
		//	주어진 리포트에서 가로 GAB이 가장 작은 TABLE의 size를 얻는다.
		//-----------------------------------------------------------------

		int GetMinTableGabX(REPORT_STRUCT report)
		{
			if(report.TableCount == 0)	return 0;

			int size = 0;
			int min = 0;
			int i;
			TABLE_STRUCT table;

			for(i = 0; i < report.TableCount; i++) 
			{
		
				table = (TABLE_STRUCT)report.tableBuf[i];
				size = table.gab_left;

				if(i == 0)	min = size;
				else 
				{
					if(size < min)	min = size;
				}
			}

			return min;
		}

		public void menuItemTableAlignToLeft_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN)	return;
			REPORT_STRUCT report = GetReportStruct();

			if(report.TableCount <= 1)	return;
	
			int gab = GetMinTableGabX(report);
			TABLE_STRUCT table;

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
	
			if(gab != table.gab_left) 
			{
				if(Tools.IsLangKorean()) 
				{
					ReportEditorUndo.UndoSave(this, "표 왼쪽으로 정렬");
				}
				else if(Tools.IsLangChinese()) 
				{
					ReportEditorUndo.UndoSave(this, "居左排列");
				}
				else 
				{
					ReportEditorUndo.UndoSave(this, "Align table to left");
				}
				table.gab_left = gab;
				SetChangeFlag();
				this.Invalidate();
			}
		}

		//-----------------------------------------------------------------
		//	주어진 테이블의 가로크기를 얻는다.
		//-----------------------------------------------------------------

		int GetTableWidth(TABLE_STRUCT table)
		{
			int size = 0;
			int j;
			CELL_STRUCT cell;

			for(j = 0; j < table.cell_x; j++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[j];
				size += cell.width;
			}

			return size;
		}

		public void menuItemTableAlignToCenter_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN)	return;
			REPORT_STRUCT report = GetReportStruct();

			if(report.TableCount <= 1)	return;
	
			int max = ReportPrint.GetMaxTableWidth(report);
			TABLE_STRUCT table;
			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
			int size = GetTableWidth(table);
			int gab;

			gab = max/2-size/2;
			if(gab < 0)	gab = 0;
	
			if(gab != table.gab_left) 
			{
				if(Tools.IsLangKorean()) 
				{
					ReportEditorUndo.UndoSave(this, "표 중간으로 정렬");
				}
				else if(Tools.IsLangChinese()) 
				{
					ReportEditorUndo.UndoSave(this, "居中排列");
				}
				else 
				{
					ReportEditorUndo.UndoSave(this, "Align table to center");
				}
				table.gab_left = gab;
				SetChangeFlag();
				this.Invalidate();
			}
		}

		public void menuItemTableAlignToRight_Click()
		{
			if(workView.nViewMode == EnumViewMode.RUN)	return;
			REPORT_STRUCT report = GetReportStruct();

			if(report.TableCount <= 1)	return;
	
			int max = ReportPrint.GetMaxTableWidth(report);
			TABLE_STRUCT table;
			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
			int size = GetTableWidth(table);
			int gab;

			gab = max-size;
			if(gab < 0)	gab = 0;
	
			if(gab != table.gab_left) 
			{
				if(Tools.IsLangKorean()) 
				{
					ReportEditorUndo.UndoSave(this, "표 오른쪽으로 정렬");
				} 
				else if(Tools.IsLangChinese()) 
				{
					ReportEditorUndo.UndoSave(this, "居右排列");
				}
				else 
				{
					ReportEditorUndo.UndoSave(this, "Align table to right");
				}

				table.gab_left = gab;
				SetChangeFlag();
				this.Invalidate();
			}	
		}

		CELL_STRUCT GetCellStruct(TABLE_STRUCT table, int x, int y)
		{
			return (CELL_STRUCT)table.cellBuf[y*table.cell_x+x];
		}

		bool IsClientZoneCursorX(REPORT_STRUCT report)
		{
			if(report.TableCount == 0)	return true;

			TABLE_STRUCT table;
			CELL_STRUCT cell;

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
	
			int x = -nHorScrollPos + GetCanvasOriginX();
			x += GetRealView(table.gab_left);

			cell = (CELL_STRUCT)table.cellBuf[table.cell_x*report.cursor_y1];

			for(int l = 0; l < report.cursor_x1; l++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[table.cell_x*report.cursor_y1+l];
				x+=GetRealView(cell.width);
			}

			if(x < 0)	return false;
			if(x+GetRealView(cell.width) > GetCanvasPaintBounds().Right)	return false;

			return true;
		}

		bool IsClientZoneCursorY(REPORT_STRUCT report)
		{
			if(report.TableCount == 0)	return true;

			TABLE_STRUCT table;
			CELL_STRUCT cell = null;

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
	
			int y = -nVerScrollPos + GetCanvasOriginY();
			int t;

			for(t = 0; t <= report.cursor_table; t++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[t];
				y += GetRealView(table.gab_top);

				for(int l = 0; l < table.cell_y; l++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[table.cell_x*l];
					if(t == report.cursor_table && l == report.cursor_y1)	break;
					y+=GetRealView(cell.height);
				}
			}

			if(y < 0)	return false;
			if(y+GetRealView(cell.height) > GetCanvasPaintBounds().Bottom)	return false;

			return true;
		}

		//-----------------------------------------------------------------------------------
		//	해당 CELL이 화면상에서 위치하는 값을 구한다.
		//-----------------------------------------------------------------------------------

		int GetViewPosX(TABLE_STRUCT table, int cursorx)
		{
			CELL_STRUCT cell;

			int x = 0;
			x += GetRealView(table.gab_left);

			for(int l = 0; l < cursorx; l++) 
			{
				cell = (CELL_STRUCT)table.cellBuf[l];
				x+=GetRealView(cell.width);
			}

			return x;
		}

		//-----------------------------------------------------------------------------------
		//	해당 CELL이 화면상에서 위치하는 값을 구한다.
		//-----------------------------------------------------------------------------------

		int GetViewPosY(REPORT_STRUCT report, int table_no, int cursory)
		{
			if(report.TableCount == 0)	return 0;

			TABLE_STRUCT table;
			CELL_STRUCT cell;

			int y = 0;
			int t;

			for(t = 0; t <= table_no; t++) 
			{
				table = (TABLE_STRUCT)report.tableBuf[t];
				y += GetRealView(table.gab_top);

				for(int l = 0; l < table.cell_y; l++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[table.cell_x*l];
					if(t == table_no && l == cursory)	break;
					y+=GetRealView(cell.height);
				}
			}

			return y;
		}

		void KeyCheckLeft()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				int x = Math.Abs(this.AutoScrollPosition.X);
				if(x <= 0)	return;

				x -= 20;
				if(x < 0)	x = 0;

				Point point = new Point(x, Math.Abs(AutoScrollPosition.Y));
				this.AutoScrollPosition = point;

				return;
			}

			if(Capture)	return;
	
			REPORT_STRUCT report = GetReportStruct();
			TABLE_STRUCT table;

			if(report.TableCount == 0)	return;
		
			bool shift_flag = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			if(shift_flag) 
			{	// most sig set
				if(report.cursor_x1 > 0) 
				{
					report.cursor_x1--;
				}
			}
			else 
			{
				if(report.cursor_x1 <= 0)	return;

				CELL_STRUCT cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);
		
				if(cell.cGroup == 2) 
				{
					SetCursorPosition(report, report.cursor_table, cell.nGroupX-1, report.cursor_y1);
				}
				else
					SetCursorPosition(report, report.cursor_table, report.cursor_x1-1, report.cursor_y1);
			}

			if(!IsClientZoneCursorX(report)) 
			{
				nHorScrollPos = GetViewPosX(table, report.cursor_x1);
				if(nHorScrollPos < 0)	nHorScrollPos = 0;
				
				Point point = new Point(nHorScrollPos, Math.Abs(AutoScrollPosition.Y));
				this.AutoScrollPosition = point;
			}

			Invalidate();
		}

		void KeyCheckRight()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				int x = Math.Abs(this.AutoScrollPosition.X);

				x += 20;

				Point point = new Point(x, Math.Abs(AutoScrollPosition.Y));
				this.AutoScrollPosition = point;

				return;
			}

			if(Capture)	return;
	
			REPORT_STRUCT report = GetReportStruct();
			TABLE_STRUCT table;

			if(report.TableCount == 0)	return;

			bool shift_flag = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			if(shift_flag) 
			{	// most sig set
				if(report.cursor_x1 < table.cell_x-1) 
				{
					report.cursor_x1++;
				}	
			}
			else 
			{
				if(report.cursor_x1 >= table.cell_x-1)		return;

				CELL_STRUCT cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

				if(cell.cGroup == 1)
					SetCursorPosition(report, report.cursor_table, cell.nGroupX+1, report.cursor_y1);
				else if(cell.cGroup == 2) 
				{
					CELL_STRUCT cell2 = GetCellStruct(table, cell.nGroupX, cell.nGroupY);
					SetCursorPosition(report, report.cursor_table, cell2.nGroupX+1, report.cursor_y1);
				}
				else
					SetCursorPosition(report, report.cursor_table, report.cursor_x1+1, report.cursor_y1);
			}

			if(!IsClientZoneCursorX(report)) 
			{
				CELL_STRUCT cell;
				cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

				nHorScrollPos = GetViewPosX(table, report.cursor_x1);
				nHorScrollPos -= GetCanvasPaintBounds().Right;

				nHorScrollPos += GetRealView(cell.width);
				nHorScrollPos += GetCanvasOriginX() + START_X;

				if(nHorScrollPos < 0)	nHorScrollPos = 0;

				Point point = new Point(nHorScrollPos, Math.Abs(AutoScrollPosition.Y));
				this.AutoScrollPosition = point;
			}

			Invalidate();
		}

		void KeyCheckUp()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				int y = Math.Abs(this.AutoScrollPosition.Y);
				if(y <= 0)	return;

				y -= 20;
				if(y < 0)	y = 0;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), y);
				this.AutoScrollPosition = point;

				return;
			}

			if(Capture)	return;
	
			REPORT_STRUCT report = GetReportStruct();
			TABLE_STRUCT table;

			if(report.TableCount == 0)	return;
		
			bool shift_flag = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			if(shift_flag) 
			{	
				if(report.cursor_y1 > 0) 
				{
					report.cursor_y1--;
				}
			}
			else 
			{
				if(report.cursor_y1 <= 0) 
				{
					if(report.cursor_table <= 0) return;
					report.cursor_table--;
					table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
					SetCursorPosition(report, report.cursor_table, report.cursor_x1, table.cell_y-1);
				}
				else 
				{
					CELL_STRUCT cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);
			
					if(cell.cGroup == 2) 
					{
						SetCursorPosition(report, report.cursor_table, report.cursor_x1, cell.nGroupY-1);
					}
					else
						SetCursorPosition(report, report.cursor_table, report.cursor_x1, report.cursor_y1-1);	
				}
			}

			if(!IsClientZoneCursorY(report)) 
			{
				nVerScrollPos = GetViewPosY(report, report.cursor_table, report.cursor_y1);
				if(nVerScrollPos < 0)	nVerScrollPos = 0;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), nVerScrollPos);
				this.AutoScrollPosition = point;
			}

			Invalidate();	
		}

		void KeyCheckDown()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				int y = Math.Abs(this.AutoScrollPosition.Y);

				y += 20;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), y);
				this.AutoScrollPosition = point;

				return;
			}

			if(Capture)	return;
	
			REPORT_STRUCT report = GetReportStruct();

			TABLE_STRUCT table;

			if(report.TableCount == 0)	return;

			bool shift_flag = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			if(shift_flag) 
			{	
				if(report.cursor_y1 < table.cell_y-1) 
				{
					report.cursor_y1++;
				}
			}
			else 
			{
				if(report.cursor_y1 >= table.cell_y-1) 
				{
					if(report.cursor_table >= report.TableCount-1)	return;	// last table
					report.cursor_table++;
					table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
					SetCursorPosition(report, report.cursor_table, report.cursor_x1, 0);
				} 
				else 
				{
					CELL_STRUCT cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

					if(cell.cGroup == 1) 
						SetCursorPosition(report, report.cursor_table, report.cursor_x1, cell.nGroupY+1);
					else if(cell.cGroup == 2) 
					{
						CELL_STRUCT cell2 = GetCellStruct(table, cell.nGroupX, cell.nGroupY);
						SetCursorPosition(report, report.cursor_table, report.cursor_x1, cell2.nGroupY+1);
					}
					else 
						SetCursorPosition(report, report.cursor_table, report.cursor_x1, report.cursor_y1+1);
				}
			}

			if(!IsClientZoneCursorY(report)) 
			{
				CELL_STRUCT cell;
				cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

				nVerScrollPos = GetViewPosY(report, report.cursor_table, report.cursor_y1);
				nVerScrollPos -= GetCanvasPaintBounds().Bottom;

				nVerScrollPos += GetRealView(cell.height);
				nVerScrollPos += GetCanvasOriginY() + START_Y;

				if(nVerScrollPos < 0)	nVerScrollPos = 0;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), nVerScrollPos);
				this.AutoScrollPosition = point;
			}

			Invalidate();
		}

		void KeyCheckPgUp()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				int y = Math.Abs(this.AutoScrollPosition.Y);
				if(y <= 0)	return;

				y -= (int)(GetCanvasPaintBounds().Height*0.8);
				if(y < 0)	y = 0;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), y);
				this.AutoScrollPosition = point;

				return;
			}

			if(Capture)	return;

			REPORT_STRUCT report = GetReportStruct();

			TABLE_STRUCT table;
			CELL_STRUCT cell;
			int  size_y = 0;

			if(report.TableCount == 0)	return;

			while(true) 
			{
				table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
				cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

				if(report.cursor_y1 <= 0) 
				{
					if(report.cursor_table <= 0)	break;	// top table
			
			
					size_y += GetRealView(table.gab_top);

					report.cursor_table--;
					table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
					report.cursor_y1 = table.cell_y-1;
					cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);
					size_y += GetRealView(cell.height);
				} 
				else 
				{
					report.cursor_y1--;
					size_y += GetRealView(cell.height);
				}

				if(size_y >= GetCanvasPaintBounds().Height)	break;
			}

			if(size_y == 0)	return;	// 움직인 것이 없다.

			SetCursorPosition(report, report.cursor_table, report.cursor_x1, report.cursor_y1);

			if(!IsClientZoneCursorY(report)) 
			{
				nVerScrollPos = GetViewPosY(report, report.cursor_table, report.cursor_y1);
				if(nVerScrollPos < 0)	nVerScrollPos = 0;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), nVerScrollPos);
				this.AutoScrollPosition = point;
			}

			Invalidate();
		}

		void KeyCheckPgDn()
		{
			if(workView.nViewMode == EnumViewMode.RUN) 
			{
				int y = Math.Abs(this.AutoScrollPosition.Y);

				y += (int)(GetCanvasPaintBounds().Height*0.8);

				Point point = new Point(Math.Abs(AutoScrollPosition.X), y);
				this.AutoScrollPosition = point;

				return;
			}

			if(Capture)	return;

			REPORT_STRUCT report = GetReportStruct();

			TABLE_STRUCT table;
			CELL_STRUCT cell;
			int  size_y = 0;

			if(report.TableCount == 0)	return;

			while(true) 
			{
				table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
				cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

				if(report.cursor_y1 >= table.cell_y-1) 
				{
					if(report.cursor_table >= report.TableCount-1)	break;	// last table
			
					size_y += GetRealView(cell.height);

					report.cursor_table++;
					report.cursor_y1 = 0;

					table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

					size_y += GetRealView(table.gab_top);
				} 
				else 
				{
					size_y += GetRealView(cell.height);
			
					report.cursor_y1++;
				}

				cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);
				if(size_y+GetRealView(cell.height) >= GetCanvasPaintBounds().Height)	break;
			}

			if(size_y == 0)	return;	// 움직인 것이 없다.

			SetCursorPosition(report, report.cursor_table, report.cursor_x1, report.cursor_y1);

			if(!IsClientZoneCursorY(report)) 
			{
				cell = GetCellStruct(table, report.cursor_x1, report.cursor_y1);

				nVerScrollPos = GetViewPosY(report, report.cursor_table, report.cursor_y1);
				nVerScrollPos -= GetCanvasPaintBounds().Bottom;

				nVerScrollPos += GetRealView(cell.height);
				nVerScrollPos += GetCanvasOriginY() + START_Y;

				if(nVerScrollPos < 0)	nVerScrollPos = 0;

				Point point = new Point(Math.Abs(AutoScrollPosition.X), nVerScrollPos);
				this.AutoScrollPosition = point;
			}

			Invalidate();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
            if (HandleUndoRedoShortcut(keyData))
            {
                return true;
            }

            if ((textBoxQuickCellEdit != null && textBoxQuickCellEdit.Focused) ||
                (textBoxQuickCellName != null && textBoxQuickCellName.Focused))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

			if(msg.Msg == 0x100)	// key Down, 방향키는 ProcessCmdKey 에서만 들어온다
			{
				if(msg.WParam.ToInt32() == (int)Keys.Left)	KeyCheckLeft();
				if(msg.WParam.ToInt32() == (int)Keys.Up)	KeyCheckUp();
				if(msg.WParam.ToInt32() == (int)Keys.Right) KeyCheckRight();
				if(msg.WParam.ToInt32() == (int)Keys.Down)	KeyCheckDown();
				if(msg.WParam.ToInt32() == (int)Keys.PageDown)	KeyCheckPgDn();
				if(msg.WParam.ToInt32() == (int)Keys.PageUp)	KeyCheckPgUp();
				if(msg.WParam.ToInt32() == (int)Keys.Enter) 
				{
					WORK_VIEW_STRUCT work = workView;
				
					if(work.nViewMode != EnumViewMode.RUN) 
					{
						ReportEditorProperty.ChangeProperty(this);
                        return true;
					}
				}
			}
			
			return base.ProcessCmdKey (ref msg, keyData);
		}

        protected override Point ScrollToControl(Control activeControl)
        {
            if (activeControl != null)
            {
                Control current = activeControl;
                while (current != null)
                {
                    if (current == panelQuickCellEdit || current == textBoxQuickCellEdit || current == textBoxQuickCellName)
                    {
                        return this.DisplayRectangle.Location;
                    }

                    current = current.Parent;
                }
            }

            return base.ScrollToControl(activeControl);
        }


	}
}



