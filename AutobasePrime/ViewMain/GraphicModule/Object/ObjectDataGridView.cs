using System;
using AutoLibLocal;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using System.Net.Sockets;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

// 10.2.3.8 부터 이 기능을 사용할 수 있음 레지스트리에서 NextVersion/DataGridView=true로 설정할 것

namespace GraphicModule
{

	[Serializable]
	public class ObjectArgsDataGridView
	{
		public string dsn;
        public bool bAllowUserToAddRows = false;
        public bool bAllowUserToDeleteRows = false;
		public Color lColorText;
		public BrushPublic lColorBack = new BrushPublic();

        public ScriptClass scriptEventCellClick;
        public ScriptClass scriptEventCellPainting;
        public ScriptClass scriptEventCellValueChanged;

        // 새로 추가된 속성들 20250304 PSU
        public bool bHideBorder = false;
        public bool bHideRowHeaders = false;
        public bool bUseAlternatingRowColors = false;
        public bool bHideGridLines = false;
        public Color lAlternatingRowBackColor = Color.FromArgb(240, 240, 240);
        public Color lAlternatingRowForeColor = Color.Black;
        public Color lSelectionBackColor = Color.LightBlue;
        public Color lSelectionForeColor = Color.Black;
        public Color lColumnHeadersForeColor = Color.Black;
        public Color lColumnHeadersDefaultBackColor = Color.LightGray;
        public Color lRowHeadersDefaultForeColor = Color.Black;
        public Color lRowHeadersDefaultBackColor = Color.LightGray;
        //public Color lCellForeColor = Color.Black;
        public Color lCellBackColor = Color.White;
        public int nAutoSizeRowsMode = 0; // DataGridViewAutoSizeRowsMode.None
        public int nAutoSizeColumnsMode = 1; // DataGridViewAutoSizeColumnsMode.None
        public int nColumnHeaderFontSize = 10;
	}

	/// <summary>
	/// Summary description for ObjectDatabase.
	/// </summary>
	
	[Serializable]
	public class ObjectDataGridView : ObjectExpand
	{
		ObjectArgsDataGridView objArgs;

		[NonSerialized]
		DataGridView wndChild;

        [NonSerialized]
        Form formParent; // 20250312 PSU

		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        public ObjectArgsDataGridView ObjectArgs 
		{
			set 
			{
				objArgs = value;
			}
			get 
			{
				return objArgs;
			}
		}

        public ObjectDataGridView(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsDataGridView args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.DataGridView;
            bSupportObjectOnCE = false;
			objArgs = args;

            formParent = form; //20250312 PSU

			//if(objArgs.nUpdateTime < 2)	objArgs.nUpdateTime = 2;	// 최소 2초

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				wndChild = new DataGridView();
                /*
				wndChild.SetClassName(general.sClassName);
				wndChild.sFileName = objArgs.filename;
				wndChild.sTableName = objArgs.table;
				wndChild.sDsnName = objArgs.dsn;
				wndChild.m_bUseNo = (objArgs.bUseNo == 1);
				wndChild.bMdbOrString = objArgs.nConnectionType;
				wndChild.bAutoUpdate = (objArgs.bAutoUpdate == 1);
				wndChild.bUseGrid = objArgs.bUseGrid;
				wndChild.bUseFullCursor = (objArgs.bUseFullCursor == 1);
				wndChild.nUpdateTime = objArgs.nUpdateTime;
                wndChild.bReverseNo = objArgs.bReverseNo;

				wndChild.TopLevel = false;
				wndChild.FormBorderStyle = FormBorderStyle.None;

                wndChild.Show();                // form.Coltrols.Add 후에 Show하면 초기 크기가 맞지 않는다.
                 */
				form.Controls.Add(wndChild);
	                
				int x1=0, y1=0, x2=0, y2=0;
				GetViewZone(ref x1, ref y1, ref x2, ref y2);
				wndChild.Left = x1;
				wndChild.Top = y1;
				wndChild.Width = x2-x1;
				wndChild.Height = y2-y1;
                
                //wndChild.SetTextColor(objArgs.lColorText);
                //wndChild.SetBackColor(objArgs.lColorBack.basic_color);
                
				wndChild.Font = MakeFont();
                wndChild.ColumnHeadersDefaultCellStyle.Font = new Font(wndChild.DefaultCellStyle.Font.FontFamily, objArgs.nColumnHeaderFontSize, wndChild.DefaultCellStyle.Font.Style);
                //wndChild.DefaultCellStyle.Font = MakeFont();
                //wndChild.AutoSize = true;
                //wndChild.ColumnHeadersHeight = 60;
                //wndChild.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // 20250304 PSU 추가
                if (objArgs.bHideBorder)
                {
                    wndChild.BorderStyle = BorderStyle.None;
                }
                // 행 헤더 표시 여부 설정
                wndChild.RowHeadersVisible = !objArgs.bHideRowHeaders;

                // 번갈아가며 행 색상 설정
                if (objArgs.bUseAlternatingRowColors)
                {
                    wndChild.AlternatingRowsDefaultCellStyle.BackColor = objArgs.lAlternatingRowBackColor;
                    wndChild.AlternatingRowsDefaultCellStyle.ForeColor = objArgs.lAlternatingRowForeColor;
                }

                // 그리드 라인 설정
                wndChild.CellBorderStyle = objArgs.bHideGridLines ?
                    DataGridViewCellBorderStyle.None : DataGridViewCellBorderStyle.Single;

                // 색상 설정
                wndChild.BackgroundColor = objArgs.lColorBack.basic_color;
                wndChild.DefaultCellStyle.ForeColor = objArgs.lColorText;
                wndChild.DefaultCellStyle.BackColor = objArgs.lCellBackColor;
                wndChild.DefaultCellStyle.SelectionBackColor = objArgs.lSelectionBackColor;
                wndChild.DefaultCellStyle.SelectionForeColor = objArgs.lSelectionForeColor;
                wndChild.ColumnHeadersDefaultCellStyle.ForeColor = objArgs.lColumnHeadersForeColor;
                wndChild.ColumnHeadersDefaultCellStyle.BackColor = objArgs.lColumnHeadersDefaultBackColor;
                wndChild.RowHeadersDefaultCellStyle.ForeColor = objArgs.lRowHeadersDefaultForeColor;
                wndChild.RowHeadersDefaultCellStyle.BackColor = objArgs.lRowHeadersDefaultBackColor;

                // 자동 크기 조정 모드 설정
                wndChild.AutoSizeRowsMode = (DataGridViewAutoSizeRowsMode)objArgs.nAutoSizeRowsMode;
                wndChild.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)objArgs.nAutoSizeColumnsMode;
                wndChild.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

				arrayClassList.Add(this);

                wndChild.CellClick += new DataGridViewCellEventHandler(wndChild_CellClick);
                wndChild.CellPainting += new DataGridViewCellPaintingEventHandler(wndChild_CellPainting);
                wndChild.CellValueChanged += new DataGridViewCellEventHandler(wndChild_CellValueChanged);
                wndChild.AllowUserToAddRows = objArgs.bAllowUserToAddRows;
                wndChild.AllowUserToDeleteRows = objArgs.bAllowUserToDeleteRows;
                wndChild.MultiSelect = false;


				//base.SetToolTipOnChildWindow(wndChild.m_list);
                OnVisible(ExpandCalcVisible());
            }
			
			TextColor = objArgs.lColorText;
			BackColor = objArgs.lColorBack;
		}

        bool bSkipCellValueChanged = false;

        async void wndChild_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 컬럼명이 바뀔때는 실행하지 않고 오직 데이타 부분일때만 호출해 준다.
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;

            if (bSkipCellValueChanged) return;  // 데이터 로딩중에도 발생한다.

            nSystemValueColumnIndex = e.ColumnIndex;
            nSystemValueRowIndex = e.RowIndex;

            await RunScript(objArgs.scriptEventCellValueChanged, "Cell Value Changed Script");
        }

        async Task RunScript(ScriptClass script, string scriptname)
        {
            if (script != null)
            {
                script.SetHandOperation();	// 수동으로 출력한다.
                await script.RunAsync(objCommonProperty.form, this);

                if (script.IsError())
                {
                    string message;
                    message = script.GetError();

                    if (Tools.IsLangKorean())
                        MessageBox.Show("스크립트 오류\n\n" + message, scriptname);
                    else if (Tools.IsLangChinese())
                        MessageBox.Show("脚本错误\n\n" + message, scriptname);
                    else
                        MessageBox.Show("Script Error\n\n" + message, scriptname);
                }
            }
        }

        async void wndChild_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            nSystemValueColumnIndex = e.ColumnIndex;
            nSystemValueRowIndex = e.RowIndex;

            await RunScript(objArgs.scriptEventCellPainting, "Cell Painting Script");
            
        }

        public static int nSystemValueColumnIndex;
        public static int nSystemValueRowIndex;

        async void wndChild_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            nSystemValueColumnIndex = e.ColumnIndex;
            nSystemValueRowIndex = e.RowIndex;

            await RunScript(objArgs.scriptEventCellClick, "Cell Click Script");
        }

		public override void Close()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				//wndChild.SaveClassConfig();
				arrayClassList.Remove(this);
			}
		}

        //public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        //{
        //    if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
        //    {
        //        if(x1 > x2)	Tools.Temp(ref x1, ref x2);
        //        if(y1 > y2)	Tools.Temp(ref y1, ref y2);

        //        Font font = MakeFont();

        //        int cyChar = (int)font.GetHeight()+1;
        //        int cxChar = (int)(font.GetHeight()/2);

        //        RECT r = new RECT();

        //        Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
        //        DrawClass.PopBox2(g, x1, y1, x2, y2, brushback);

        //        Rectangle rect = new Rectangle(x1, y1, x2-x1, y2-y1);
        //        StringFormat format = new StringFormat();
        //        format.Alignment = StringAlignment.Center;
        //        format.LineAlignment = StringAlignment.Center;

        //        SafeException.SafeDrawString(g, this.objGeneral.sClassName, MakeFont(), Brushes.Black, rect, format);
        //    }
        //}

        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                // 클리핑 영역 설정
                Region originalClip = g.Clip;
                Rectangle clipRect = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                g.SetClip(clipRect);

                // 배경 그리기
                using (Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2))
                    DrawClass.PopBox2(g, x1, y1, x2, y2, brushback);

                Font font = MakeFont();
                Brush textBrush = new SolidBrush(RunColorText); // disposed below

                int padding = 0; //padding 필요없음.
                int lineHeight = (int)font.GetHeight() + 3;
                StringFormat infoFormat = new StringFormat(); // disposed below

                // 실제 그리드 영역 (전체 영역 사용)
                int gridY = y1 + padding;
                int gridHeight = y2 - gridY - padding;
                Rectangle gridRect = new Rectangle(x1, gridY, x2 - x1, gridHeight);

                // 컬럼 헤더 텍스트 (샘플)
                string[] headerTexts = { " No", " Name", " Value", " Status" };

                // 행 헤더 너비
                int rowHeaderWidth = 30;
                int contentX = x1;

                // 행 헤더 표시 여부 
                bool showRowHeaders = !objArgs.bHideRowHeaders;

                if (showRowHeaders)
                {
                    contentX += rowHeaderWidth;
                }

                int colWidth = (x2 - contentX) / headerTexts.Length;

                // 샘플 데이터 행 그리기 (3개 정도)
                string[][] sampleData = new string[][]
        {
            new string[] { "1", "Item 1", "100", "Active" },
            new string[] { "2", "Item 2", "200", "(Selected Row)" },
            new string[] { "3", "Item 3", "300", "Pending" },
            new string[] { "4", "Item 4", "300", "" },
            new string[] { "5", "Item 5", "300", "" }
        };

                // 헤더 행 높이 계산
                //int headerHeight = lineHeight;
                Font columnHeaderFont;
                int columnFontHeight = GetViewSize((int)objArgs.nColumnHeaderFontSize);

                if (columnFontHeight == 0) columnFontHeight = 1;

                try
                {
                    columnHeaderFont = new Font(logFont.lfFaceName, columnFontHeight, logFont.style);
                }
                catch
                {
                    columnHeaderFont = new Font("Arial", columnFontHeight, logFont.style);
                }
                // Note: columnHeaderFont is disposed at end of DisplayObject
                int headerHeight = (int)columnHeaderFont.GetHeight() + 3;

                // 데이터 영역 총 높이 계산
                int dataAreaHeight = sampleData.Length * lineHeight;

                // 헤더 행 (컬럼명) 배경 그리기
                Rectangle headerRowRect = new Rectangle(x1, gridY, x2 - x1, headerHeight);
                using (Brush columnHeaderBrush = new SolidBrush(objArgs.lColumnHeadersDefaultBackColor))
                {
                    g.FillRectangle(columnHeaderBrush, headerRowRect);
                }

                // 행 헤더 배경 그리기 - 데이터 행 수만큼만 그리기
                if (showRowHeaders)
                {
                    // 헤더 영역
                    Rectangle rowHeaderHeaderRect = new Rectangle(x1, gridY, rowHeaderWidth, headerHeight);
                    using (Brush rowHeaderBrush = new SolidBrush(objArgs.lRowHeadersDefaultBackColor))
                    {
                        g.FillRectangle(rowHeaderBrush, rowHeaderHeaderRect);
                    }

                    // 데이터 영역 행 헤더 (데이터 행 수에 맞게)
                    Rectangle rowHeaderDataRect = new Rectangle(x1, gridY + headerHeight, rowHeaderWidth, dataAreaHeight);
                    using (Brush rowHeaderBrush = new SolidBrush(objArgs.lRowHeadersDefaultBackColor))
                    {
                        g.FillRectangle(rowHeaderBrush, rowHeaderDataRect);
                    }
                }

                // 모든 셀 배경색 그리기 (번갈아가는 행 색상 적용)
                for (int row = 0; row < sampleData.Length; row++)
                {
                    int rowY = gridY + headerHeight + (row * lineHeight);

                    // 번갈아가는 행 색상 옵션이 켜져 있으면 적용
                    Color rowColor = (objArgs.bUseAlternatingRowColors && row % 2 == 1)
                        ? objArgs.lAlternatingRowBackColor : objArgs.lCellBackColor;
                    using (Brush rowBrush = new SolidBrush(rowColor))
                    {
                        Rectangle rowRect = new Rectangle(contentX, rowY, x2 - contentX, lineHeight);
                        g.FillRectangle(rowBrush, rowRect);
                    }

                    // 선택된 행 효과 (두 번째 행은 선택된 것처럼 표시)
                    if (row == 1)
                    {
                        Rectangle selectedRect = new Rectangle(contentX, rowY, x2 - contentX, lineHeight);
                        using (Brush selectionBrush = new SolidBrush(objArgs.lSelectionBackColor))
                        {
                            g.FillRectangle(selectionBrush, selectedRect);
                        }
                    }
                }

                // 헤더 텍스트 그리기
                for (int i = 0; i < headerTexts.Length; i++)
                {
                    Rectangle colRect = new Rectangle(contentX + (i * colWidth), gridY + 3, colWidth, headerHeight);
                    using (Brush headerTextBrush = new SolidBrush(objArgs.lColumnHeadersForeColor))
                    {
                        g.DrawString(headerTexts[i], columnHeaderFont, headerTextBrush, colRect, infoFormat);
                    }
                }

                // 행 헤더와 데이터 텍스트 그리기
                for (int row = 0; row < sampleData.Length; row++)
                {
                    int rowY = gridY + headerHeight + (row * lineHeight) + 2;

                    // 행 헤더 표시
                    if (showRowHeaders)
                    {
                        Rectangle rowHeaderRect = new Rectangle(x1, rowY, rowHeaderWidth, lineHeight);

                        // 선택된 행에는 선택 표시자 그리기
                        if (row == 1) // 선택된 행
                        {
                            using (StringFormat headerFormat = new StringFormat())
                            using (Brush rowHeaderFgBrush = new SolidBrush(objArgs.lRowHeadersDefaultForeColor))
                            {
                                headerFormat.Alignment = StringAlignment.Center;
                                headerFormat.LineAlignment = StringAlignment.Center;
                                g.DrawString("▶", font, rowHeaderFgBrush, rowHeaderRect, headerFormat);
                            }
                        }
                    }

                    // 행 텍스트 색상 결정
                    // 행 텍스트 색상 결정
                    Color cellTextColor;
                    bool disposeCellBrush;
                    if (row == 1) { cellTextColor = objArgs.lSelectionForeColor; disposeCellBrush = true; }
                    else if (objArgs.bUseAlternatingRowColors && row % 2 == 1) { cellTextColor = objArgs.lAlternatingRowForeColor; disposeCellBrush = true; }
                    else { cellTextColor = RunColorText; disposeCellBrush = false; }

                    Brush cellTextBrush = disposeCellBrush ? new SolidBrush(cellTextColor) : textBrush;
                    try
                    {
                        for (int col = 0; col < sampleData[row].Length; col++)
                        {
                            Rectangle cellRect = new Rectangle(contentX + (col * colWidth), rowY, colWidth, lineHeight);
                            g.DrawString(sampleData[row][col], font, cellTextBrush, cellRect, infoFormat);
                        }
                    }
                    finally
                    {
                        if (disposeCellBrush) cellTextBrush.Dispose();
                    }
                }

                // 모든 셀과 텍스트를 그린 후 마지막으로 그리드 라인 그리기 (bShowGridLines 옵션 사용)
                if (!objArgs.bHideGridLines)
                {
                    // 헤더 행 구분선
                    g.DrawLine(Pens.Gray, x1, gridY + headerHeight, x2, gridY + headerHeight);

                    // 컬럼 구분선
                    for (int i = 1; i < headerTexts.Length; i++)
                    {
                        g.DrawLine(Pens.Gray, contentX + (i * colWidth), gridY,
                            contentX + (i * colWidth), gridY + headerHeight + dataAreaHeight);
                    }

                    // 행 헤더와 내용 구분선
                    if (showRowHeaders)
                    {
                        g.DrawLine(Pens.Gray, contentX, gridY, contentX, gridY + headerHeight + dataAreaHeight);
                    }

                    // 행 구분선
                    for (int row = 0; row < sampleData.Length; row++)
                    {
                        int rowY = gridY + headerHeight + (row * lineHeight);
                        g.DrawLine(Pens.Gray, x1, rowY, x2, rowY);
                    }

                    // 데이터 영역 끝 구분선
                    g.DrawLine(Pens.Gray, x1, gridY + headerHeight + dataAreaHeight, x2, gridY + headerHeight + dataAreaHeight);
                }

                // 테두리 그리기 (bHideBorder 옵션에 따라)
                if (!objArgs.bHideBorder)
                {
                    using (Pen borderPen = new Pen(Color.Black, 1))
                    {
                        g.DrawRectangle(borderPen, gridRect);
                    }
                }

                // 설정 정보 박스 표시 (그리드 위에 겹쳐서)
                int infoBoxWidth = (x2 - x1) - 20;
                int infoBoxHeight = lineHeight * 11; // 추가된 옵션으로 인해 높이 조정

                if (infoBoxHeight < gridHeight - 50)  // 정보박스보다 datagrid가 작으면 그리지 않음.
                {

                    // 박스 위치 - 그리드 중앙 하단에 위치
                    int infoBoxX = x1 + 10; // ((x2 - x1) - infoBoxWidth);
                    int infoBoxY = gridY + gridHeight - infoBoxHeight - 10;



                    Rectangle infoBoxRect = new Rectangle(infoBoxX, infoBoxY, infoBoxWidth, infoBoxHeight);

                    // 정보 박스 배경 (약간 투명하게)
                    using (Brush infoBrush = new SolidBrush(Color.FromArgb(220, Color.WhiteSmoke)))
                    {
                        g.FillRectangle(infoBrush, infoBoxRect);
                    }

                    // 정보 박스 테두리
                    g.DrawRectangle(Pens.Gray, infoBoxRect);

                    // 언어에 따른 설정 정보 표시
                    string[] settings;
                    string titleText;

                    if (Tools.IsLangKorean())
                    {
                        titleText = "DataGridView 설정 정보";
                        settings = new string[]
                        {
                            String.Format("클래스명: {0}", this.objGeneral.sClassName),
                            String.Format("DSN: {0}", string.IsNullOrEmpty(objArgs.dsn) ? "(없음)" : objArgs.dsn),
                            String.Format("테두리 표시: {0}", objArgs.bHideBorder ? "숨김" : "표시"),
                            String.Format("행 자동크기: {0}", GetAutoSizeRowsModeText(objArgs.nAutoSizeRowsMode)),
                            String.Format("열 자동크기: {0}", GetAutoSizeColumnsModeText(objArgs.nAutoSizeColumnsMode)),
                            String.Format("행 헤더 표시: {0}", objArgs.bHideRowHeaders ? "숨김" : "표시"),
                            String.Format("번갈아가는 행 색상: {0}", objArgs.bUseAlternatingRowColors ? "사용" : "사용안함"),                
                            String.Format("그리드 라인 표시: {0}", objArgs.bHideGridLines ? "숨김" : "표시"),
                            String.Format("사용자 열 추가/삭제: {0} / {1}", objArgs.bAllowUserToAddRows ? "가능" : "불가", 
                                objArgs.bAllowUserToDeleteRows ? "가능" : "불가")
                        };
                    }
                    else
                    {
                        titleText = "DataGridView Settings";
                        settings = new string[]
                        {
                            String.Format("Class Name: {0}", this.objGeneral.sClassName),
                            String.Format("DSN: {0}", string.IsNullOrEmpty(objArgs.dsn) ? "(None)" : objArgs.dsn),
                            String.Format("Border Display: {0}", objArgs.bHideBorder ? "Hidden" : "Shown"),
                            String.Format("Auto Size Rows: {0}", GetAutoSizeRowsModeText(objArgs.nAutoSizeRowsMode)),
                            String.Format("Auto Size Columns: {0}", GetAutoSizeColumnsModeText(objArgs.nAutoSizeColumnsMode)),
                            String.Format("Row Headers: {0}", objArgs.bHideRowHeaders ? "Hidden" : "Shown"),
                            String.Format("Alternating Row Colors: {0}", objArgs.bUseAlternatingRowColors ? "Used" : "Not Used"),
                            String.Format("Grid Lines: {0}", objArgs.bHideGridLines ? "Hidden" : "Shown"),
                            String.Format("User Add/Delete Rows: {0} / {1}", objArgs.bAllowUserToAddRows ? "Allowed" : "Not Allowed", 
                                objArgs.bAllowUserToDeleteRows ? "Allowed" : "Not Allowed")
                        };
                    }

                    // 정보 박스 제목
                    using (Font titleFont = new Font(font.FontFamily, 10, FontStyle.Bold))
                    using (Font contentFont = new Font(font.FontFamily, 9, FontStyle.Regular))
                    {
                        g.DrawString(titleText, titleFont, Brushes.DarkBlue,
                            infoBoxX + padding * 2, infoBoxY + padding * 2 + 2);

                        // 설정 정보 내용 (여백 추가)
                        for (int i = 0; i < settings.Length; i++)
                        {
                            g.DrawString(settings[i], contentFont, Brushes.Black,
                                infoBoxX + padding * 2,
                                infoBoxY + padding * 3 + lineHeight + (i * (lineHeight + 2)));
                        }
                    }
                }

                textBrush.Dispose();
                infoFormat.Dispose();
                columnHeaderFont.Dispose();
                g.Clip = originalClip;
            }

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                UpdateControlState(wndChild, formParent);//20250312 PSU
            }
        }


        // 자동 크기 조정 모드 텍스트 반환 헬퍼 메서드
        private string GetAutoSizeRowsModeText(int mode)
        {
            switch (mode)
            {
                case 0: return (Tools.IsLangKorean() ? "사용안함" : "None");
                //case 1: return "전체 컬럼";
                //case 2: return "헤더만";
                //case 3: return "표시된 컬럼";
                // 4: return "마지막 컬럼";
                case 7: return (Tools.IsLangKorean() ? "사용" : "Used");
                default: return (Tools.IsLangKorean() ? "알수없음" : "Unknown");
            }
        }

        private string GetAutoSizeColumnsModeText(int mode)
        {
            switch (mode)
            {
                case 1: return (Tools.IsLangKorean() ? "사용안함" : "None");
                //case 1: return "전체 컬럼";
                //case 2: return "헤더만";
                //case 3: return "표시된 컬럼";
                //case 4: return "마지막 컬럼";
                case 6: return (Tools.IsLangKorean() ? "사용" : "Used");
                default: return (Tools.IsLangKorean() ? "알수없음" : "Unknown");
            }
        }

		public override void OnMove(int x1, int y1, int x2, int y2)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
                if (wndChild != null)
                {
                    // 컨트롤 업데이트 일시 중지 //20250312 PSU
                    wndChild.SuspendLayout();
                    try
                    {
                        Font font = MakeFont();
                        wndChild.Font = font;

                        wndChild.Left = x1;
                        wndChild.Top = y1;
                        wndChild.Width = Math.Abs(x2 - x1);
                        wndChild.Height = Math.Abs(y2 - y1);
                    }
                    finally
                    {
                        // 컨트롤 업데이트 재개
                        wndChild.ResumeLayout(false);
                        wndChild.PerformLayout(); // 레이아웃 강제 수행, datagrieview는 필요.
                    }

				}
			}
		}

        DataSet rstHistory;
        string sSelectText = "";

        async Task ReLoad()
        {
            if (dtDataGridSetDataTable != null)
            {
                bSkipCellValueChanged = true;
                this.wndChild.DataSource = dtDataGridSetDataTable;
                bSkipCellValueChanged = false;

                dtDataGridSetDataTable = null;
            }
            else if (objArgs.dsn.Length == 0)
            {
                this.wndChild.DataSource = null;
            }
            else
            {
                rstHistory = new DataSet();

                string query = sSelectText;

                DataGate gate = new DataGate();
                string error;
                (rstHistory, error) = await gate.GetDataSetFromDsn(objArgs.dsn, query);

                if (rstHistory == null)
                {
                    MessageDisplay.Show("Dsn={0}\n{1}", objArgs.dsn, error);
                    return;
                }

                bSkipCellValueChanged = true;
                this.wndChild.DataSource = rstHistory.Tables[0];
                bSkipCellValueChanged = false;
            }
        }


        DataGridViewColumn GetViewColumn(string name)
        {
            for (int i = 0; i < wndChild.Columns.Count; i++)
            {
                if (wndChild.Columns[i].Name == name)
                {
                    return wndChild.Columns[i];
                }
            }

            return null;
        }

        // 헬퍼 메서드
        private Task<object> ExecuteOnUIThreadAsync(Func<Task<object>> action)
        {
            var tcs = new TaskCompletionSource<object>();

            if (wndChild.InvokeRequired)
            {
                wndChild.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        var result = await action();
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }));
            }
            else
            {
                // 이미 UI 스레드라면 그냥 호출
                return action();
            }

            return tcs.Task;
        }

        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
                   // 헬퍼 메서드로 전체 로직을 UI 스레드에서 실행
            return await ExecuteOnUIThreadAsync(async() =>
            {
                //20250304 PSU try catch 추가
                try
                {
                    string className = (string)args[0];

                    if (command == "DataGridAddButtonColumn")
                    {
                        DataGridViewButtonColumn column = new DataGridViewButtonColumn();
                        column.Name = (string)args[1];
                        column.HeaderText = (string)args[2];
                        column.DataPropertyName = (string)args[3];
                        column.Text = (string)args[4];
                        column.UseColumnTextForButtonValue = true;
                        wndChild.Columns.Add(column);
                        return 1;
                    }
                    else if (command == "DataGridAddCheckBoxColumn")
                    {
                        DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();

                        column.Name = (string)args[1];
                        column.HeaderText = (string)args[2];
                        column.DataPropertyName = (string)args[3];

                        wndChild.Columns.Add(column);
                        return 1;
                    }
                    else if (command == "DataGridAddTextBoxColumn")
                    {
                        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();

                        column.Name = (string)args[1];
                        column.HeaderText = (string)args[2];
                        column.DataPropertyName = (string)args[3];

                        wndChild.Columns.Add(column);
                        return 1;
                    }
                    else if (command == "DataGridAddComboBoxColumn")
                    {
                        DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();

                        column.Name = (string)args[1];
                        column.HeaderText = (string)args[2];
                        column.DataPropertyName = (string)args[3];

                        wndChild.Columns.Add(column);
                        return 1;
                    }
                    else if (command == "DataGridAddRow")
                    {
                        try
                        {
                            return wndChild.Rows.Add();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error");
                        }
                        return 1; //20250719 PSU 추가
                    }

                    else if (command == "DataGridClearColumn")
                    {
                        wndChild.Columns.Clear();
                        return 1;
                    }
                    else if (command == "DataGridDeleteRow")
                    {
                        int row_pos = (int)args[1];

                        try
                        {
                            wndChild.Rows.RemoveAt(row_pos);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error");
                        }
                        return 1; //20250719 PSU 추가
                    }
                    else if (command == "DataGridGetCellData")
                    {
                        int cell_x = (int)args[1];
                        int cell_y = (int)args[2];

                        if (cell_x >= wndChild.Columns.Count ||
                            cell_y >= wndChild.Rows.Count)
                            return "";

                        DataGridViewRow row = wndChild.Rows[cell_y];
                        DataGridViewCell cell = row.Cells[cell_x];

                        if (cell.Value == null)
                            return "";
                        else
                            return cell.Value.ToString();
                    }
                    else if (command == "DataGridGetCellSel")
                    {
                        //int cell_x = (int)args[1];
                        //int cell_y = (int)args[2];

                        if (wndChild.SelectedCells.Count == 0)
                            return 0;

                        args[1] = wndChild.SelectedCells[0].ColumnIndex;
                        args[2] = wndChild.SelectedCells[0].RowIndex;

                        return 1;
                    }
                    else if (command == "DataGridGetColumnName")
                    {
                        int cell_x = (int)args[1];

                        if (cell_x < 0 || cell_x >= wndChild.Columns.Count) return "";

                        return wndChild.Columns[cell_x].Name;
                    }
                    else if (command == "DataGridGetCurSel")
                    {
                        if (wndChild.SelectedRows.Count == 0) return -1;

                        return wndChild.SelectedRows[0].Index;
                    }
                    // 언제부터인지 제외되어 있었다. 2020-2-24 발견하고 다시 유효화하였다.
                    else if (command == "DataGridSetCurSel")
                    {
                        // -1 이하의 값을 주면 일치되는 것이 없어서 자동으로 선택이 취소된다.
                        int pos = (int)args[1];

                        for (int i = 0; i < wndChild.Rows.Count; i++)
                        {
                            if (pos != i && wndChild.Rows[i].Selected)
                                wndChild.Rows[i].Selected = false;
                        }

                        for (int i = 0; i < wndChild.Rows.Count; i++)
                        {
                            if (pos == i)
                            {
                                wndChild.CurrentCell = wndChild.Rows[i].Cells[0];   // 이것을 하지 않으면 처음에 있는 화살표가 이동하지 않는다.
                                wndChild.Rows[i].Selected = true;
                            }
                        }

                        return 1;
                    }
                    else if (command == "DataGridGetRowCount")
                    {
                        return wndChild.Rows.Count;
                    }
                    else if (command == "DataGridGetRowData")  //20250304 PSU 체크박스인 경우 수정.
                    {
                        string column_name = (string)args[1];
                        int row_pos = (int)args[2];

                        if (row_pos < 0 || row_pos >= wndChild.Rows.Count) return "";

                        DataGridViewRow row = wndChild.Rows[row_pos];
                        DataGridViewCell cell;

                        try
                        {
                            cell = row.Cells[column_name];
                        }
                        catch
                        {
                            return "";
                        }


                        if (cell is DataGridViewCheckBoxCell)  // 체크박스 셀인 경우 개선된 처리
                        {
                            bool isChecked = false;

                            // 여러 속성을 확인하여 체크박스 상태 파악
                            if (cell.Value != null)
                            {
                                isChecked = Convert.ToBoolean(cell.Value);
                            }
                            else if (cell.EditedFormattedValue != null)
                            {
                                isChecked = Convert.ToBoolean(cell.EditedFormattedValue);
                            }

                            // 현재 편집 모드인 경우 종료하여 값 확정
                            if (wndChild.IsCurrentCellInEditMode && wndChild.CurrentCell == cell)
                            {
                                try
                                {
                                    wndChild.EndEdit();
                                    if (cell.Value != null)
                                    {
                                        isChecked = Convert.ToBoolean(cell.Value);
                                    }
                                }
                                catch
                                {
                                    // 예외 무시
                                }
                            }

                            return isChecked ? "1" : "0";
                        }
                        return cell.Value == null ? "" : cell.Value.ToString();
                    }
                    else if (command == "DataGridInsertRow")
                    {
                        int row_pos = (int)args[1];
                        try
                        {
                            wndChild.Rows.Insert(row_pos, 1);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error");
                        }
                        return 1;
                    }
                    else if (command == "DataGridReLoad")
                    {
                        await ReLoad();
                        return 1; //20250719 PSU 추가
                    }
                    else if (command == "DataGridSetCellData")
                    {
                        int cell_x = (int)args[1];
                        int cell_y = (int)args[2];

                        if (cell_x >= wndChild.Columns.Count ||
                            cell_y >= wndChild.Rows.Count)
                            return "";

                        DataGridViewRow row = wndChild.Rows[cell_y];
                        DataGridViewCell cell = row.Cells[cell_x];
                        DataGridViewColumn dgvc = wndChild.Columns[cell_x];

                        string data = (string)args[3];

                        try
                        {
                            if (dgvc.GetType() == typeof(DataGridViewCheckBoxColumn))
                            {
                                if (data == "1" ||
                                    String.Compare(data, "TRUE", true) == 0 ||
                                    String.Compare(data, "ON", true) == 0)
                                {
                                    cell.Value = true;
                                }
                                else
                                {
                                    cell.Value = false;
                                }
                            }
                            else
                            {
                                cell.Value = args[3];
                            }
                            return 1; //20250719 PSU 추가
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error");
                        }
                    }
                    else if (command == "DataGridSetColumnIndex")
                    {
                        DataGridViewColumn column = GetViewColumn((string)args[1]);
                        if (column != null)
                        {
                            int index = (int)args[2];
                            column.DisplayIndex = index;
                        }
                        return 1;
                    }
                    else if (command == "DataGridSetColumnReadOnly")
                    {
                        DataGridViewColumn column = GetViewColumn((string)args[1]);
                        if (column != null)
                        {
                            bool flag = (bool)args[2];
                            column.ReadOnly = flag;
                        }
                        return 1;
                    }
                    else if (command == "DataGridSetColumnText")
                    {
                        DataGridViewColumn column = GetViewColumn((string)args[1]);
                        if (column != null)
                        {
                            string text = (string)args[2];

                            column.HeaderText = text;
                        }
                        return 1;
                    }
                    else if (command == "DataGridSetColumnVisible")
                    {
                        DataGridViewColumn column = GetViewColumn((string)args[1]);
                        if (column != null)
                        {
                            bool flag = (bool)args[2];
                            column.Visible = flag;
                        }
                        return 1;
                    }

                    else if (command == "DataGridSetCellBackColor")
                    {
                        string column_name = (string)args[1];
                        int row_pos = (int)args[2];
                        int color = (int)args[3];

                        if (row_pos < 0 || row_pos >= wndChild.Rows.Count) return 1;

                        DataGridViewRow row = wndChild.Rows[row_pos];
                        row.Cells[column_name].Style.BackColor = Color.FromArgb(color);

                        return 1;
                    }
                    else if (command == "DataGridSetCellTextColor")
                    {
                        string column_name = (string)args[1];
                        int row_pos = (int)args[2];
                        int color = (int)args[3];

                        if (row_pos < 0 || row_pos >= wndChild.Rows.Count) return 1;

                        DataGridViewRow row = wndChild.Rows[row_pos];
                        row.Cells[column_name].Style.ForeColor = Color.FromArgb(color);

                        return 1;
                    }

                    else if (command == "DataGridSetDsn")
                    {
                        string dsn = (string)args[1];
                        objArgs.dsn = dsn;

                        return 1;
                    }

                    else if (command == "DataGridSetRowData")
                    {
                        string column_name = (string)args[1];
                        int row_pos = (int)args[2];

                        if (row_pos < 0 || row_pos >= wndChild.Rows.Count) return 0;

                        DataGridViewRow row = wndChild.Rows[row_pos];
                        DataGridViewCell cell;

                        try
                        {
                            cell = row.Cells[column_name];
                        }
                        catch
                        {
                            return 0;
                        }

                        DataGridViewColumn dgvc = wndChild.Columns[cell.ColumnIndex];

                        string data = (string)args[3];

                        try
                        {
                            if (dgvc.GetType() == typeof(DataGridViewCheckBoxColumn))
                            {
                                if (data == "1" ||
                                    String.Compare(data, "TRUE", true) == 0 ||
                                    String.Compare(data, "ON", true) == 0)
                                {
                                    cell.Value = true;
                                }
                                else
                                {
                                    cell.Value = false;
                                }
                            }
                            else
                            {
                                cell.Value = args[3];
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Error");
                        }

                        return 1;
                    }

                    else if (command == "DataGridSetSelect")
                    {
                        string text = (string)args[1];
                        sSelectText = text;

                        return 1;
                    }

                    else if (command == "DataGridSetColumnWidth")
                    {
                        int cell_x = (int)args[1];
                        int width = (int)args[2];

                        if (cell_x < 0 || cell_x >= wndChild.Columns.Count) return 0;

                        wndChild.Columns[cell_x].Width = width;

                        return 1;
                    }
                    else if (command == "DataGridSetRowHeight")
                    {
                        int cell_y = (int)args[1];
                        int height = (int)args[2];

                        if (cell_y < 0 || cell_y >= wndChild.Rows.Count) return 0;

                        DataGridViewRow row = wndChild.Rows[cell_y];

                        row.Height = height;

                        return 1;
                    }

                    else if (command == "DataGridComboBoxColumnItemAdd")
                    {
                        string column_name = (string)args[1];
                        string item = (string)args[2];

                        DataGridViewColumn dgvc = wndChild.Columns[column_name];

                        if (dgvc.GetType() == typeof(DataGridViewComboBoxColumn))
                        {
                            DataGridViewComboBoxColumn gdvcbc = (DataGridViewComboBoxColumn)dgvc;
                            gdvcbc.Items.Add(item);
                        }
                        else
                        {

                        }

                        return 1;
                    }
                    else if (command == "DataGridSetColumnHeadersHeight") // 폰트 크기가 바뀌면 컬럼헤더의 높이를 변경할 필요가 있다. 폰트에 따라서 자동으로 크기가 변경되지 않는다.
                    {
                        int height = (int)args[1];

                        wndChild.ColumnHeadersHeight = height;

                        return 1;
                    }
                    else if (command == "DataGridSetDataTable")
                    {
                        if (args[1].GetType() == typeof(DataTable))
                            dtDataGridSetDataTable = (DataTable)args[1];
                        else
                            dtDataGridSetDataTable = null;

                        return 1;
                    }
                    else if (command == "DataGridSetColumnAlignment")
                    {
                        DataGridViewColumn column = GetViewColumn((string)args[1]);
                        if (column != null)
                        {
                            int alignment = (int)args[2];

                            try
                            {
                                column.DefaultCellStyle.Alignment = (DataGridViewContentAlignment)alignment;
                                // NotSet = 0, TopLeft = 1, TopCenter = 2, TopRight = 4, MiddleLeft = 16, MiddleCenter = 32, MiddleRight = 64, BottomLeft = 256, BottomCenter = 512, BottomRight = 1024,
                            }
                            catch
                            {
                            }
                        }
                        return 1;
                    }

                    //20250304 PSU 추가
                    else if (command == "DataGridGetColumnCount")
                    {
                        return GetColumnCount();
                    }
                    else if (command == "DataGridSetBorder")
                    {
                        bool hideBorder = (bool)args[1];
                        objArgs.bHideBorder = hideBorder;
                        wndChild.BorderStyle = hideBorder ? BorderStyle.None : BorderStyle.FixedSingle;
                        return 1;
                    }
                    else if (command == "DataGridSetRowHeadersVisible")
                    {
                        bool hideRowHeaders = (bool)args[1];
                        objArgs.bHideRowHeaders = hideRowHeaders;
                        if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                        {
                            wndChild.RowHeadersVisible = !hideRowHeaders;
                        }
                        return 1;
                    }
                    else if (command == "DataGridSetSelectionBackColor")
                    {
                        int color = (int)args[1];
                        objArgs.lSelectionBackColor = Color.FromArgb(color);
                        wndChild.DefaultCellStyle.SelectionBackColor = objArgs.lSelectionBackColor;
                        return 1;
                    }
                    else if (command == "DataGridSetSelectionTextColor")  //20250402 PSU 수정 Fore -> Text
                    {
                        int color = (int)args[1];
                        objArgs.lSelectionForeColor = Color.FromArgb(color);
                        wndChild.DefaultCellStyle.SelectionForeColor = objArgs.lSelectionForeColor;
                        return 1;
                    }
                    else if (command == "DataGridSetColumnHeadersTextColor")  //20250402 PSU 수정 Fore -> Text
                    {
                        int color = (int)args[1];
                        objArgs.lColumnHeadersForeColor = Color.FromArgb(color);
                        wndChild.ColumnHeadersDefaultCellStyle.ForeColor = objArgs.lColumnHeadersForeColor;
                        return 1;
                    }
                    else if (command == "DataGridSetColumnHeadersBackColor")
                    {
                        int color = (int)args[1];
                        objArgs.lColumnHeadersDefaultBackColor = Color.FromArgb(color);
                        wndChild.ColumnHeadersDefaultCellStyle.BackColor = objArgs.lColumnHeadersDefaultBackColor;
                        return 1;
                    }
                    else if (command == "DataGridSetRowHeadersTextColor")  //20250402 PSU 수정 Fore -> Text
                    {
                        int color = (int)args[1];
                        objArgs.lRowHeadersDefaultForeColor = Color.FromArgb(color);
                        wndChild.RowHeadersDefaultCellStyle.ForeColor = objArgs.lRowHeadersDefaultForeColor;
                        return 1;
                    }
                    else if (command == "DataGridSetRowHeadersBackColor")
                    {
                        int color = (int)args[1];
                        objArgs.lRowHeadersDefaultBackColor = Color.FromArgb(color);
                        wndChild.RowHeadersDefaultCellStyle.BackColor = objArgs.lRowHeadersDefaultBackColor;
                        return 1;
                    }
                    else if (command == "DataGridSetDefaultCellBackColor")  //기본 셀 배경색상 변경.
                    {
                        int color = (int)args[1];
                        objArgs.lCellBackColor = Color.FromArgb(color);
                        wndChild.DefaultCellStyle.BackColor = objArgs.lCellBackColor;
                        return 1;
                    }
                    else if (command == "DataGridSetDefaultCellTextColor")  //기본 셀 글자 색상 변경.  //20250402 PSU 수정 Back -> Text
                    {
                        int color = (int)args[1];
                        objArgs.lColorText = Color.FromArgb(color);
                        wndChild.DefaultCellStyle.ForeColor = objArgs.lColorText;
                        return 1;
                    }
                    else if (command == "DataGridSetAutoSizeRowsMode")
                    {
                        int mode = (int)args[1];
                        objArgs.nAutoSizeRowsMode = mode;
                        wndChild.AutoSizeRowsMode = (DataGridViewAutoSizeRowsMode)mode;
                        return 1;
                    }
                    else if (command == "DataGridSetAutoSizeColumnsMode")
                    {
                        int mode = (int)args[1];
                        objArgs.nAutoSizeColumnsMode = mode;
                        wndChild.AutoSizeColumnsMode = (DataGridViewAutoSizeColumnsMode)mode;
                        return 1;
                    }
                    else if (command == "DataGridSetUseAlternatingRowColors")
                    {
                        bool useAlternatingColors = (bool)args[1];
                        objArgs.bUseAlternatingRowColors = useAlternatingColors;
                        if (useAlternatingColors)
                        {
                            wndChild.AlternatingRowsDefaultCellStyle.BackColor = objArgs.lAlternatingRowBackColor;
                            wndChild.AlternatingRowsDefaultCellStyle.ForeColor = objArgs.lAlternatingRowForeColor;
                        }
                        else
                        {
                            wndChild.AlternatingRowsDefaultCellStyle.BackColor = wndChild.DefaultCellStyle.BackColor; //20250402 PSU 수정 else 누락.
                            wndChild.AlternatingRowsDefaultCellStyle.ForeColor = wndChild.DefaultCellStyle.ForeColor;
                        }
                        return 1;
                    }
                    else if (command == "DataGridSetAlternatingRowTextColor")  //20250402 PSU 수정 Fore -> Text
                    {
                        int color = (int)args[1];
                        objArgs.lAlternatingRowForeColor = Color.FromArgb(color);
                        wndChild.AlternatingRowsDefaultCellStyle.ForeColor = objArgs.lAlternatingRowForeColor;
                        return 1;
                    }
                    else if (command == "DataGridSetAlternatingRowBackColor") //20250328 PSU else 누락.
                    {
                        int color = (int)args[1];
                        objArgs.lAlternatingRowBackColor = Color.FromArgb(color);
                        wndChild.AlternatingRowsDefaultCellStyle.BackColor = objArgs.lAlternatingRowBackColor;
                        return 1;
                    }
                    else if (command == "DataGridSetHideGridLines")
                    {
                        bool hide = (bool)args[1];
                        objArgs.bHideGridLines = hide;
                        wndChild.CellBorderStyle = objArgs.bHideGridLines ?
                            DataGridViewCellBorderStyle.None : DataGridViewCellBorderStyle.Single;

                        return 1;
                    }
                    else if (command == "DataGridSortColumn")
                    {
                        int columnIndex = (int)args[1];
                        int direction = (int)args[2]; // 0: Ascending, 1: Descending

                        if (columnIndex >= 0 && columnIndex < wndChild.Columns.Count)
                        {
                            wndChild.Sort(wndChild.Columns[columnIndex],
                                direction == 0 ? ListSortDirection.Ascending : ListSortDirection.Descending);
                        }

                        return 1;
                    }
                    else if (command == "DataGridClearRows")   //20250328 PSU else 누락.
                    {
                        ClearRows();
                        return 1;
                    }
                    // CSV 가져오기 (파일 경로 지정)
                    else if (command == "DataGridImportCsv")
                    {
                        if (args.Length < 2) return 0;
                        string filePath = (string)args[1];
                        bool hasHeader = args.Length > 2 ? Convert.ToBoolean(args[2]) : true;
                        char delimiter = args.Length > 3 ? Convert.ToChar(args[3]) : ',';
                        return ImportFromCSV(filePath, hasHeader, delimiter) ? 1 : 0;
                    }
                    // CSV 내보내기 (파일 경로 지정)
                    else if (command == "DataGridExportCsv")
                    {
                        if (args.Length < 2) return 0;
                        string filePath = (string)args[1];
                        bool includeHeaders = args.Length > 2 ? Convert.ToBoolean(args[2]) : true;
                        char delimiter = args.Length > 3 ? Convert.ToChar(args[3]) : ',';
                        Encoding encoding = Encoding.UTF8; // 기본값은 UTF-8
                        if (args.Length > 4 && args[4] is string)
                        {
                            string encodingName = (string)args[4];
                            switch (encodingName.ToUpper())
                            {
                                case "UTF8":
                                case "UTF-8":
                                    encoding = Encoding.UTF8;
                                    break;
                                case "ASCII":
                                    encoding = Encoding.ASCII;
                                    break;
                                case "UNICODE":
                                    encoding = Encoding.Unicode;
                                    break;
                                case "BIGENDIANUNICODE":
                                    encoding = Encoding.BigEndianUnicode;
                                    break;
                                case "UTF32":
                                case "UTF-32":
                                    encoding = Encoding.UTF32;
                                    break;
                                case "DEFAULT":
                                    encoding = Encoding.Default;
                                    break;
                                default:
                                    // 알 수 없는 인코딩은 기본값 사용
                                    encoding = Encoding.Default;
                                    break;
                            }
                        }

                        return ExportToCSV(filePath, includeHeaders, delimiter, encoding) ? 1 : 0;
                    }
                    else if (command == "DataGridSetColumnHeaderFontSize")
                    {
                        objArgs.nColumnHeaderFontSize = (int)args[1];
                        wndChild.ColumnHeadersDefaultCellStyle.Font = new Font(wndChild.DefaultCellStyle.Font.FontFamily, objArgs.nColumnHeaderFontSize, wndChild.DefaultCellStyle.Font.Style);
                        return 1; //20250719 PSU 추가
                    }
                    else if (command == "DataGridSetDefaultFontSize")
                    {
                        int defaultFontSize = (int)args[1];
                        wndChild.Font = new Font(wndChild.Font.FontFamily, defaultFontSize, wndChild.Font.Style);
                        return 1; //20250719 PSU 추가
                    }
                    else
                    {
                        string msg = String.Format("This function is not yet supported. Method={0} in {1}.ExcuteClassName", command, this.ToString());
                        MessageDisplay.Show(msg);
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    // 예외 발생 시 오류 메시지 표시
                    if (ConfigViewMain.bScriptErrorMessageShow)
                    {
                        string className = (string)args[0];
                        MessageDisplay.Show(String.Format("DataGrid object, ClassName: {0} \nError: ", className) + ex.Message);
                    }
                    return 0;
                }
            });
        }

        DataTable dtDataGridSetDataTable = null;

        /*
        public override string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            

            return "";
        }*/

		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);

			//SaveObjectItem.FileName(writer, objArgs.filename);
			SaveObjectItem.BackColor(writer, GetBackColor());
			SaveObjectItem.TextColor(writer, GetTextColor());
			
            writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.dsn);
            writer.Write("{0},", objArgs.bAllowUserToAddRows);
            writer.Write("{0},", objArgs.bAllowUserToDeleteRows);

            writer.Write("{0},", objArgs.bHideBorder);  //20250304 PSU 추가
            writer.Write("{0},", objArgs.bHideRowHeaders);
            writer.Write("{0},", objArgs.bHideGridLines);
            writer.Write("{0},", objArgs.bUseAlternatingRowColors);
            writer.Write("{0},", objArgs.lAlternatingRowForeColor.ToArgb());
            writer.Write("{0},", objArgs.lAlternatingRowBackColor.ToArgb());
            writer.Write("{0},", objArgs.lSelectionBackColor.ToArgb());
            writer.Write("{0},", objArgs.lSelectionForeColor.ToArgb());
            writer.Write("{0},", objArgs.lColumnHeadersForeColor.ToArgb());
            writer.Write("{0},", objArgs.lColumnHeadersDefaultBackColor.ToArgb());
            writer.Write("{0},", objArgs.lRowHeadersDefaultForeColor.ToArgb());
            writer.Write("{0},", objArgs.lRowHeadersDefaultBackColor.ToArgb());
            writer.Write("{0},", objArgs.lCellBackColor.ToArgb());
            writer.Write("{0},", objArgs.nAutoSizeRowsMode);
            writer.Write("{0},", objArgs.nAutoSizeColumnsMode);
            writer.Write("{0},", objArgs.nColumnHeaderFontSize);

			writer.WriteLine();

            if(objArgs.scriptEventCellClick != null) 
            {
                objArgs.scriptEventCellClick.SaveScript(writer, "ScriptEventCellClick");
            }
            if (objArgs.scriptEventCellPainting != null)
            {
                objArgs.scriptEventCellPainting.SaveScript(writer, "ScriptEventCellPainting");
            }
            if (objArgs.scriptEventCellValueChanged != null)
            {
                objArgs.scriptEventCellValueChanged.SaveScript(writer, "ScriptEventCellValueChanged");
            }
		}

        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            //wndChild.SetTextColor(this.RunColorText);
            wndChild.DefaultCellStyle.ForeColor = this.RunColorText; //20250304 PSU
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            //wndChild.SetBackColor(this.RunColorBack.basic_color);

            wndChild.BackgroundColor = this.RunColorBack.basic_color; //20250304 PSU
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                this.wndChild.Visible = flag;
            }

        }


        public void ClearRows()  //모든 행 삭제 20250304 PSU
        {
            if (wndChild != null && wndChild.Rows.Count > 0)
            {
                bSkipCellValueChanged = true;
                try
                {
                    wndChild.Rows.Clear();
                }
                finally
                {
                    bSkipCellValueChanged = false;
                }
            }
        }
        public int GetColumnCount()  //열 개수 반환 20250304 PSU
        {
            return wndChild != null ? wndChild.Columns.Count : 0;
        }

        /// <summary>
        /// CSV 파일을 DataGridView로 가져오기
        /// </summary>
        /// <param name="filePath">CSV 파일 경로</param>
        /// <param name="hasHeaderRow">CSV 파일의 첫 번째 행이 헤더인지 여부</param>
        /// <param name="delimiter">구분자</param>
        /// <returns>성공 여부</returns>
        public bool ImportFromCSV(string filePath, bool hasHeaderRow, char delimiter)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("파일이 존재하지 않습니다: " + filePath, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else MessageBox.Show("File does not exist: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                // 파일 인코딩 탐지
                Encoding encoding = DetectEncoding(filePath);

                // 먼저 최대 열 수 확인
                int maxColumnCount = 0;
                string firstLine = null;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoding))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (firstLine == null)
                        {
                            firstLine = line;
                        }

                        string[] columns = line.Split(delimiter);
                        if (columns.Length > maxColumnCount)
                        {
                            maxColumnCount = columns.Length;
                        }
                    }
                }

                if (string.IsNullOrEmpty(firstLine) || maxColumnCount == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("CSV 파일이 비어 있습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else MessageBox.Show("CSV file is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // CSV 파일을 DataTable로 변환
                DataTable dt = new DataTable();

                // 최대 열 수에 맞게 열 생성
                if (hasHeaderRow)
                {
                    string[] headers = firstLine.Split(delimiter);

                    // 헤더 행의 열 개수만큼 먼저 열 추가
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dt.Columns.Add(headers[i].Trim('"').Trim());
                    }

                    // 헤더 행보다 더 많은 열이 있는 경우 추가 열 생성
                    for (int i = headers.Length; i < maxColumnCount; i++)
                    {
                        dt.Columns.Add("Column" + (i + 1));
                    }
                }
                else
                {
                    // 헤더가 없는 경우 최대 열 수만큼 기본 열 이름 생성
                    for (int i = 0; i < maxColumnCount; i++)
                    {
                        dt.Columns.Add("Column" + (i + 1));
                    }
                }

                // 데이터 행 추가
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoding))
                {
                    // 헤더 행이 있으면 첫 행 건너뛰기
                    if (hasHeaderRow)
                    {
                        sr.ReadLine();
                    }

                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            dt.Rows.Add(ConvertCSVRowToDataRow(line, delimiter, dt));
                        }
                    }
                }

                // DataGridView에 DataTable 바인딩
                bSkipCellValueChanged = true; // 이벤트 임시 비활성화
                wndChild.DataSource = dt;
                bSkipCellValueChanged = false; // 이벤트 재활성화

                // DataTable 저장
                dtDataGridSetDataTable = dt;
                return true;
            }
            catch (Exception ex)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("CSV 파일 가져오기 오류: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show("CSV File Import Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// CSV 파일의 인코딩 탐지
        /// </summary>
        private Encoding DetectEncoding(string filePath)
        {
            // 바이트 순서 표시(BOM) 확인
            byte[] bom = new byte[4];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Read(bom, 0, 4);
            }

            // UTF-8 BOM (EF BB BF)
            if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                return Encoding.UTF8;

            // UTF-16 LE BOM (FF FE)
            if (bom[0] == 0xFF && bom[1] == 0xFE)
                return Encoding.Unicode;

            // UTF-16 BE BOM (FE FF)
            if (bom[0] == 0xFE && bom[1] == 0xFF)
                return Encoding.BigEndianUnicode;

            // UTF-32 BOM
            if (bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
                return Encoding.UTF32;

            // 기본값으로 UTF-8 또는 시스템 기본 인코딩 사용
            return Encoding.Default;
        }

        /// <summary>
        /// CSV 행을 DataRow로 변환
        /// </summary>
        private object[] ConvertCSVRowToDataRow(string csvLine, char delimiter, DataTable dt)
        {
            List<string> values = new List<string>();
            int startIndex = 0;
            bool inQuotes = false;
            int columnCount = dt.Columns.Count;

            for (int i = 0; i < csvLine.Length; i++)
            {
                if (csvLine[i] == '"')
                    inQuotes = !inQuotes;

                else if (csvLine[i] == delimiter && !inQuotes)
                {
                    values.Add(csvLine.Substring(startIndex, i - startIndex).Trim('"').Trim());
                    startIndex = i + 1;
                }
            }

            // 마지막 값 추가
            values.Add(csvLine.Substring(startIndex).Trim('"').Trim());

            // 열 수만큼 값 배열 생성
            object[] rowValues = new object[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                if (i < values.Count)
                    rowValues[i] = values[i];
                else
                    rowValues[i] = DBNull.Value;
            }

            return rowValues;
        }

        /// <summary>
        /// DataGridView의 데이터를 CSV 파일로 내보내기
        /// </summary>
        /// <param name="filePath">저장할 CSV 파일 경로</param>
        /// <param name="includeHeaders">헤더 포함 여부</param>
        /// <param name="delimiter">구분자</param>
        /// <param name="encoding">파일 인코딩</param>
        /// <returns>성공 여부</returns>
        public bool ExportToCSV(string filePath, bool includeHeaders, char delimiter, Encoding encoding)
        {
            try
            {
                if (wndChild.Rows.Count == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("내보낼 데이터가 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("No data to export", "notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                using (StreamWriter sw = new StreamWriter(filePath, false, encoding))
                {
                    // 헤더 작성
                    if (includeHeaders)
                    {
                        List<string> headerValues = new List<string>();
                        foreach (DataGridViewColumn column in wndChild.Columns)
                        {
                            if (column.Visible) // 보이는 열만 내보내기
                            {
                                string value = FormatCSVField(column.HeaderText, delimiter);
                                headerValues.Add(value);
                            }
                        }
                        sw.WriteLine(string.Join(delimiter.ToString(), headerValues.ToArray()));
                    }

                    // 데이터 행 작성
                    foreach (DataGridViewRow row in wndChild.Rows)
                    {
                        if (!row.IsNewRow) // 새 행이 아닌 경우에만 내보내기
                        {
                            List<string> rowValues = new List<string>();
                            foreach (DataGridViewColumn column in wndChild.Columns)
                            {
                                if (column.Visible) // 보이는 열만 내보내기
                                {
                                    object value = row.Cells[column.Index].Value;
                                    string formattedValue = FormatCSVField(value != null ? value.ToString() : string.Empty, delimiter);
                                    rowValues.Add(formattedValue);
                                }
                            }
                            sw.WriteLine(string.Join(delimiter.ToString(), rowValues.ToArray()));
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("CSV 파일 내보내기 오류: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show("CSV File Export Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        /// <summary>
        /// CSV 필드값 포맷팅
        /// </summary>
        private string FormatCSVField(string value, char delimiter)
        {
            // 값이 비어있으면 빈 문자열 반환
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // 값에 구분자, 쌍따옴표, 줄바꿈이 포함되어 있으면 쌍따옴표로 감싸기
            bool needQuotes = value.Contains(delimiter.ToString()) ||
                              value.Contains("\"") ||
                              value.Contains("\r") ||
                              value.Contains("\n");

            if (needQuotes)
            {
                // 쌍따옴표를 두 개로 이스케이프
                value = value.Replace("\"", "\"\"");
                return "\"" + value + "\"";
            }

            return value;
        }

		
	}
}

