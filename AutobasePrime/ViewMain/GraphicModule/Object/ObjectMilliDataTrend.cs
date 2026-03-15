using AutoLib;



using AutoLibLocal;



using DialogControl;



using NetTools;



using NetTools.OldDefine;



using Npgsql;



using System;



using System.Collections;



using System.Collections.Generic;



using System.Data;



using System.Diagnostics;



using System.Drawing;



using System.Drawing.Drawing2D;



using System.Drawing.Text;



using System.Globalization;



using System.IO;



using System.Runtime.Serialization;



using System.Runtime.Serialization.Formatters.Binary;



using System.Runtime.Serialization.Json;



using System.Threading.Tasks;



using System.Windows.Forms;







namespace GraphicModule



{



    [Serializable]



    public class ObjectArgsMilliDataTrend



    {



        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();







        // 범위 설정 20251210 PSU



        private int _wLevelDevide;



        public int wLevelDevide



        {



            get => _wLevelDevide;



            set => _wLevelDevide = Math.Max(2, Math.Min(100, value));



        }







        private int _wTimeDevide;



        public int wTimeDevide



        {



            get => _wTimeDevide;



            set => _wTimeDevide = Math.Max(1, Math.Min(10000, value));



        }







        private int _nDataCycle;



        public int nDataCycle



        {



            get => _nDataCycle;



            set => _nDataCycle = Math.Max(1, Math.Min(1000, value));



        }







        private int _wShowUnit;



        public int wShowUnit



        {



            get => _wShowUnit;



            set => _wShowUnit = Math.Max(2, Math.Min(44640, value));



        }







        public int wTimeSelectOption;







        public BrushPublic lColorBack = new BrushPublic();



        public Color lColorGuideLine;



        public Color lColorText;



        public BrushPublic lColorFill = new BrushPublic();



        //public int wPointSize;		// y 크기의 10/1000 만한 포인트 크기











        //public EnumDisplayFlag wDisplayFlags;



        //public int nLevelDisplaySize = 7;







        public string sDsn;



        //public string sTable;



        //public string sColumnTime;			// 시간 컬럼











        //public string sColumnMilli;			// Milli Sec Column



        //public int nDateColumnType = 0;		// 0 = Datetime, 1 = yyyymmddhhmmss



        public int nBasicSpaceLeft = 0;		// 왼쪽의 기본적인 준비 공간



        public int nBasicSpaceRight = 0;	// 오른쪽의 기본적인 준비 공간



        public bool bAutoUpdate = false;    // 자동으로 갱신하는 모드 (속도 문제가 있을 수 있으므로 기본을 false) 9.5.2 부터 지원







        public LogarithmicScale logarithmicScale = new LogarithmicScale();







        public bool bDontUseConfigDialog;       // 설정대화상자 사용안함. 20250306 PSU 없어서 추가.



        public bool bUseToolBar;     //20250306 PSU



        public int nToolBarPos = 0;



        public int nTooolBarBtnColor = 0; //0 : white, 1 : black



        public bool bHideLabelDataRange;







        public int nToolBarButtonSize = 16; // 기본 버튼 크기 16 20250317 PSU



        public int nToolBarTextSize = 9; // 기본 글자 크기 9







    }







    [Serializable]



    public class MD_TREND_MEMBER



    {



        public string tag;



        public string column;



        // multi graph 에서 사용할 멤버



        public EnumTagType nType;		// tag type



        public int[] nPos;			// tag pos



        public Color color;			// 각 태그의 사용색



        public int nValueType;	// 0 - 평균치



        public int nGraphType;	// 0 = Line, 1 = Bar, 2 = Area, 3 = Spline, 4 = StepLine,



        public int nPointType;	// 점의 형태, 0 - none, 



        public int nLineThick;	// 선굵기.



        public int nAxisPosition;	// 좌표의 위치, 0 - 표시안함, 1 - 왼쪽에 표시, 2 - 오른쪽에 표시,



        public int nLevelFrom;	// 그라프 표시의 위치 부터



        public int nLevelTo;		// 그라프 표시의 위치 까지



        public int nAxisCalcPos;	// 내부 계산에 필요한 부분 미리 그래프가 위치할 부분을 계산해 놓는다.



        public ONE_POINT_STRUCT[] point;// 각 그래프에서 사용하는 그라프 메모리.



        public double min_value;		// 읽은 값 중에서 최소값



        public double max_value;		// 읽은 값 중에서 최대값.



        public sbyte visible;		// 현재 활성화중인가? ON=보임, OFF = 안보임



        public int nTagDisplaySize = 10;// 태그이름을 표시할 때 크기가 10으로 고정되면 모두 안보일 수 있으므로 사용자가 설정할 수 있게 하였다.



        public double view_full;



        public double view_base;



        public int nReverseY;		// Y축의 좌표를 반대로 한다.



        public ushort wFlags = 0;



        //public string	 sTable;		// 각 멤버에서 사용하는 테이블



        //public string	 sWhereString;



        public string sDescription;	// 개별적으로 사용할 설명.



        public int column_pos = -1;     // DB에서 컬럼을 빨리 찾을 수 있게 위치를 찾아 놓는다.



    }







    [DataContract]



    public class PatternLineItem



    {



        [DataMember]



        public Color color;



        [DataMember]



        public int thick;



        [DataMember]



        public DateTime tStart;



        [DataMember]



        public double pattern_cycle;



        [DataMember]



        public int line_count;



        [DataMember]



        public int start_number;



        [DataMember]



        public int number_position = 0;   // 0=left, 1=right



        [DataMember]



        public bool visible = true;



    }







    /// <summary>



    /// Summary description for ObjectMultiTrend.



    /// </summary>



    [Serializable]



    public class ObjectMilliDataTrend : ObjectExpand



    {



        ArrayList blockMember;







        int nHapLeftDisplay;	// 왼쪽 눈금의 개수



        int nHapRightDisplay;	// 왼쪽 눈금의 개수



        int nCharWidth;



        int nCharHeight;







        bool bDisplayPointDate;	// 자료시점의 날짜를 표시해 준다.



        bool bAutoViewRange;



        bool bAutoGuideLine;



        bool bUseLocalRange;







        sbyte cGraphStartTimeMethod;







        int dwFirstLevelDisplay;



        int nDataGab;



        int nCursorX1;



        int nCursorX2;



        bool bMouseCapture;



        int nBarGraphCount = 0;



        bool bShowToolTip;







        int nMovePeriod = 1; //20250306 PSU 앞주기,뒤주기 이동간격







        [NonSerialized]



        private PictureBox[] toolbarButtons;



        private int originalShowUnit = 0;



        private DateTime originalStartTime;



        private int originalCursorX1;



        private int originalCursorX2;



        private bool toolbarVisible = true;



        [NonSerialized]



        private ToolTip toolTipToolbar; // ToolTip 객체 추가



        [NonSerialized]



        private Label lblInfo;







        private string[] toolTipToolbars = Tools.IsLangKorean()



    ? new string[] { "자동/수동", "축소", "확대", "이전프레임", "이전주기", "다음주기", "다음프레임", "드래그 확대", "설정" }



    : new string[] { "Auto/Manual", "Zoom Out", "Zoom In", "Prev Frame", "Prev Cycle", "Next Cycle", "Next Frame", "Drag Zoom", "Settings" };











        ObjectArgsMilliDataTrend objArgs;







        DateTime dtStartTime = DateTimeServer.Now;







        [NonSerialized]



        Form formParent;







        // 등록된 클래스 리스트



        [NonSerialized]



        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();







        ArrayList arrayGuideDisplay = new ArrayList();











        // 캐시 이미지 관련 필드 추가 20250701 PSU



        [NonSerialized]



        private Bitmap cachedGraphImage = null;



        [NonSerialized]



        private Size cachedGraphSize = Size.Empty;



        [NonSerialized]



        private DateTime lastDataUpdateTime = DateTime.MinValue;



        [NonSerialized]



        private bool graphCacheValid = false;











        public ObjectArgsMilliDataTrend ObjectArgs



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







        public ArrayList GraphMember



        {



            set



            {



                this.SetBlock(value);



            }



            get



            {



                return blockMember;



            }



        }







        [NonSerialized]



        List<PatternLineItem> arrayPatternLine = null;







        public ObjectMilliDataTrend(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsMilliDataTrend args, ArrayList block)



            : base(ocp, rect, eid, lf, general)



        {



            //



            // TODO: Add constructor logic here



            //



            enumObjectType = EnumObjectType.MilliDataTrend;



            bSupportObjectOnCE = false;



            objArgs = args;



            formParent = form;







            TextColor = args.lColorText;



            BackColor = args.lColorBack;



            FillColor = args.lColorFill;







            bDisplayPointDate = false;



            bAutoViewRange = false;



            bAutoGuideLine = false;



            bUseLocalRange = false;







            cGraphStartTimeMethod = 0;			// AUTO







            SetPointSize(args.pub.wPointSize);		// y 크기의 10/1000 만한 포인트 크기



            dwFirstLevelDisplay = 0;			// 첫번째 Level디스프레이는 사용자가 바꿀 수 있다.



            nDataGab = 1;







            if (objArgs.wLevelDevide < 2) objArgs.wLevelDevide = 2;



            if (objArgs.wLevelDevide > 100) objArgs.wLevelDevide = 100;



            if (objArgs.nDataCycle < 1) objArgs.nDataCycle = 1;







            SetBlock(block);







            if (objArgs.wTimeDevide < 1) objArgs.wTimeDevide = 10;







            CalcStartTime();







            CalcCharSize();



            nCursorX1 = objArgs.wShowUnit / 2;



            nCursorX2 = objArgs.wShowUnit / 2;







            LoadConfigAndExcute();



            LoadMultiTrendMemberConfig();







            _ = ReadAllPoint();







            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



            {



                arrayClassList.Add(this);



            }







            LoadPatternLine();







            // ToolTip 객체 초기화  20250306 PSU



            toolTipToolbar = new ToolTip();



            toolTipToolbar.InitialDelay = 500;



            toolTipToolbar.ReshowDelay = 100;



        }







        public override void Close()



        {



            arrayClassList.Remove(this);







            // 그래프 캐시 정리



            InvalidateGraphCache();







            // 툴바 버튼 제거  20250306 PSU 추가



            if (toolbarButtons != null)



            {



                foreach (PictureBox btn in toolbarButtons)



                {



                    if (btn != null && !btn.IsDisposed)



                    {



                        btn.Dispose();



                    }



                }



                toolbarButtons = null;



            }



            // ToolTip 제거



            if (toolTipToolbar != null)



            {



                toolTipToolbar.Dispose();



                toolTipToolbar = null;



            }







            // 라벨 제거



            if (lblInfo != null && !lblInfo.IsDisposed)



            {



                lblInfo.Dispose();



                lblInfo = null;



            }



        }







        void SeekCalcPos(ArrayList array, MD_TREND_MEMBER seek)



        {



            MD_TREND_MEMBER member;







            int fit_pos = 0;







            for (int l = 0; l < array.Count; l++)



            {



                member = (MD_TREND_MEMBER)array[l];







                // 영역에 걸린다.



                if (seek.nLevelFrom >= member.nLevelTo || seek.nLevelTo <= member.nLevelFrom)



                {



                }



                else



                {



                    fit_pos++;



                }



            }







            seek.nAxisCalcPos = fit_pos;







            array.Add(seek);



        }







        void AddGuideDisplay(int from, int to)



        {



            GUIDE_DISPLAY guide;







            for (int i = 0; i < arrayGuideDisplay.Count; i++)



            {



                guide = (GUIDE_DISPLAY)arrayGuideDisplay[i];







                if (guide.from == from && guide.to == to) return;



            }







            guide = new GUIDE_DISPLAY();



            guide.from = from;



            guide.to = to;



            arrayGuideDisplay.Add(guide);



        }







        void CalcAxisPosition()



        {



            int l;



            MD_TREND_MEMBER member;



            ArrayList arrayL = new ArrayList();



            ArrayList arrayR = new ArrayList();







            nBarGraphCount = 0;







            arrayGuideDisplay.Clear();







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                AddGuideDisplay(member.nLevelFrom, member.nLevelTo);







                if (member.nAxisPosition == 1)



                {	// left position



                    SeekCalcPos(arrayL, member);







                }



                else if (member.nAxisPosition == 2)



                {	// right position



                    SeekCalcPos(arrayR, member);



                }



                else { }







                if (member.nGraphType == 1) nBarGraphCount++;



            }







            nHapLeftDisplay = 0;







            for (l = 0; l < arrayL.Count; l++)



            {



                member = (MD_TREND_MEMBER)arrayL[l];







                if (member.nAxisCalcPos > nHapLeftDisplay) nHapLeftDisplay = member.nAxisCalcPos;



            }







            if (arrayL.Count > 0) nHapLeftDisplay++;







            nHapRightDisplay = 0;







            for (l = 0; l < arrayR.Count; l++)



            {



                member = (MD_TREND_MEMBER)arrayR[l];







                if (member.nAxisCalcPos > nHapRightDisplay) nHapRightDisplay = member.nAxisCalcPos;



            }







            if (arrayR.Count > 0) nHapRightDisplay++;







            // 기본 좌우 공간을 확보한다.



            if (nHapLeftDisplay < objArgs.nBasicSpaceLeft) nHapLeftDisplay = objArgs.nBasicSpaceLeft;



            if (nHapRightDisplay < objArgs.nBasicSpaceRight) nHapRightDisplay = objArgs.nBasicSpaceRight;







        }







        bool IsDesX() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.DESX) == 0 ? false : true); } // X축의 설명이 있는냐?



        bool IsDesY() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.DESY) == 0 ? false : true); } // Y축의 설명이 있는냐?



        bool IsGuideLine() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.GUIDELINE) == 0 ? false : true); }	// Guide Line이 있느냐.



        bool IsTagColor() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_COLOR) == 0 ? false : true); }	// 태그 안내판 표시.



        bool IsTagCurr() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_CURR) == 0 ? false : true); }



        bool IsTagOldCurr() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_OLD_CURR) == 0 ? false : true); }



        bool IsTagMinMax() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_MIN_MAX) == 0 ? false : true); }



        bool IsBackBorder() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.BACK_BORDER) == 0 ? false : true); }



        bool IsByDescription() { return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.BY_DESCRIPTION) == 0 ? false : true); }







        void GetGraphZone(ref int x1, ref int y1, ref int x2, ref int y2)



        {



            GetViewZone(ref x1, ref y1, ref x2, ref y2);







            x1 += 3;



            y1 += 3;



            x2 -= 3;



            y2 -= 3;







            if (IsDesX())



            {



                y2 -= (int)(nCharHeight * 2);



            }



            else



            {



                y2 -= nCharHeight / 2 - 2;



            }







            if (IsDesY())



            {



                x1 += (nCharWidth * objArgs.pub.nLevelDisplaySize) * nHapLeftDisplay;



                x2 -= (nCharWidth * objArgs.pub.nLevelDisplaySize) * nHapRightDisplay;



            }







            if (IsTagColor()) y2 -= (nCharHeight + 2) * 1;



            if (IsTagCurr()) y2 -= (nCharHeight + 2) * 1;



            if (IsTagOldCurr()) y2 -= (nCharHeight + 2) * 1;



            if (IsTagMinMax()) y2 -= (nCharHeight + 2) * 2;







            if (bDisplayPointDate) y1 += nCharHeight;



        }







        void CalcCharSize()



        {



            Font font = MakeFont();







            nCharHeight = (int)font.GetHeight() + 1;



            nCharWidth = (int)font.SizeInPoints;







            //GetTextCharSize(hdc, nCharWidth, nCharHeight);



            //g.Get



        }







        DateTime FitStartTime(DateTime dt)



        {



            if (objArgs.wTimeSelectOption == 0)



            {	// milli data



                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);



            }



            else if (objArgs.wTimeSelectOption == 1)



            {	// sec data



                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);



            }



            else if (objArgs.wTimeSelectOption == 2)



            {	// min data



                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);



            }



            else if (objArgs.wTimeSelectOption == 3)



            {	// hour data



                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);



            }



            else if (objArgs.wTimeSelectOption == 4)



            {	// day data



                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);



            }



            else if (objArgs.wTimeSelectOption == 5)



            {	// month data



                return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);



            }



            else



            {	// year data



                return new DateTime(dt.Year, 1, 1, 0, 0, 0);



            }



        }







        void CalcStartTime()



        {



            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



            {



                if (cGraphStartTimeMethod == 0)



                {



                    DateTime dt = DateTimeServer.Now;







                    dt = FitStartTime(dt);







                    int data_cycle = objArgs.wShowUnit * objArgs.nDataCycle;







                    if (objArgs.wTimeSelectOption == 0)



                    {	// milli data



                        dtStartTime = dt.AddMilliseconds(-data_cycle);



                    }



                    else if (objArgs.wTimeSelectOption == 1)



                    {	// sec data



                        dtStartTime = dt.AddSeconds(-data_cycle);



                    }



                    else if (objArgs.wTimeSelectOption == 2)



                    {	// min data



                        dtStartTime = dt.AddMinutes(-data_cycle);



                    }



                    else if (objArgs.wTimeSelectOption == 3)



                    {	// hour data



                        dtStartTime = dt.AddHours(-data_cycle);



                    }



                    else if (objArgs.wTimeSelectOption == 4)



                    {	// day data



                        dtStartTime = dt.AddDays(-data_cycle);



                    }



                    else if (objArgs.wTimeSelectOption == 5)



                    {	// month data



                        dtStartTime = dt.AddMonths(-data_cycle);



                    }



                    else



                    {	// year data



                        dtStartTime = dt.AddYears(-data_cycle);



                    }



                }



                else



                {



                    dtStartTime = FitStartTime(dtStartTime);



                }



            }



            else



            {



                DateTime dt = DateTimeServer.Now;







                dt = FitStartTime(dt);







                int data_cycle = objArgs.wShowUnit * objArgs.nDataCycle;







                if (objArgs.wTimeSelectOption == 0)



                {	// milli data



                    dtStartTime = dt.AddMilliseconds(-data_cycle);



                }



                else if (objArgs.wTimeSelectOption == 1)



                {	// sec data



                    dtStartTime = dt.AddSeconds(-data_cycle);



                }



                else if (objArgs.wTimeSelectOption == 2)



                {	// min data



                    dtStartTime = dt.AddMinutes(-data_cycle);



                }



                else if (objArgs.wTimeSelectOption == 3)



                {	// hour data



                    dtStartTime = dt.AddHours(-data_cycle);



                }



                else if (objArgs.wTimeSelectOption == 4)



                {	// day data



                    dtStartTime = dt.AddDays(-data_cycle);



                }



                else if (objArgs.wTimeSelectOption == 5)



                {	// month data



                    dtStartTime = dt.AddMonths(-data_cycle);



                }



                else



                {	// year data



                    dtStartTime = dt.AddYears(-data_cycle);



                }



            }







            // 시간이 변경되었으므로 캐시 무효화 20250701



            InvalidateGraphCache();



        }







        void SetPointSize(int size)



        {



            if (size <= 0) size = 10;



            if (size >= 500) size = 500;



            objArgs.pub.wPointSize = (ushort)size;



        }







        DateTime GetDateTimeFromRow(DataRow row, int column_time, int column_milli)



        {



            DateTime t;



            int milli;







            if (objArgs.wTimeSelectOption == 0) // milli sec



            {



                try



                {



                    if (column_milli == -1)



                        milli = 0;



                    else



                        milli = ConvertTool.ToInt32(row[column_milli].ToString());







                    t = ConvertTool.ToDateTime(row[column_time]);







                    t = t.AddMilliseconds(milli);



                }



                catch



                {



                    t = new DateTime(1, 1, 1, 0, 0, 0);



                }



            }



            else



            {



                try



                {



                    t = ConvertTool.ToDateTime(row[column_time]);



                }



                catch



                {



                    t = new DateTime(1, 1, 1, 0, 0, 0);



                }



            }







            return t;



        }







        bool FilenameToDateTime(string filename, out DateTime t)



        {



            if (filename.Length != 19)



            {



                t = new DateTime(2000, 1, 1);



                return false;



            }







            int year;



            int month;



            int day;



            int hour;



            int minute;



            int second;







            year = ConvertTool.ToInt32(filename.Substring(0, 4));



            month = ConvertTool.ToInt32(filename.Substring(4, 2));



            day = ConvertTool.ToInt32(filename.Substring(6, 2));







            hour = ConvertTool.ToInt32(filename.Substring(9, 2));



            minute = ConvertTool.ToInt32(filename.Substring(11, 2));



            second = ConvertTool.ToInt32(filename.Substring(13, 2));







            try



            {



                t = new DateTime(year, month, day, hour, minute, second);



            }



            catch   // 파일명이 날짜 형식이 아닌것 같다. invalid file



            {



                t = new DateTime(2000, 1, 1);



                return false;



            }







            return true;



        }







        bool ReadOneFile(FileInfo fi)



        {



            DateTime t_filestart;







            //int year;



            //int month;



            //int day;



            //int hour;



            //int minute;



            //int second;







            bool file_end = false;







            if (!FilenameToDateTime(fi.Name, out t_filestart))



                return file_end;







            //year = ConvertTool.ToInt32(fi.Name.Substring(0, 4));



            //month = ConvertTool.ToInt32(fi.Name.Substring(4, 2));



            //day = ConvertTool.ToInt32(fi.Name.Substring(6, 2));







            //hour = ConvertTool.ToInt32(fi.Name.Substring(9, 2));



            //minute = ConvertTool.ToInt32(fi.Name.Substring(11, 2));



            //second = ConvertTool.ToInt32(fi.Name.Substring(13, 2));







            //try



            //{



            //    t_filestart = new DateTime(year, month, day, hour, minute, second);



            //}



            //catch   // 파일명이 날짜 형식이 아닌것 같다. invalid file



            //{



            //    return file_end;



            //}







            int data_cycle = objArgs.wShowUnit * objArgs.nDataCycle;



            DateTime t_to;







            if (objArgs.wTimeSelectOption == 0)



            {



                t_to = dtStartTime.AddMilliseconds(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 1)



            {



                t_to = dtStartTime.AddSeconds(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 2)



            {



                t_to = dtStartTime.AddMinutes(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 3)



            {



                t_to = dtStartTime.AddHours(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 4)



            {



                t_to = dtStartTime.AddHours(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 5)



            {



                t_to = dtStartTime.AddDays(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 6)



            {



                t_to = dtStartTime.AddDays(data_cycle);



            }



            else



            {



                return file_end;



            }







            if (t_filestart >= t_to) return file_end; // 범위 밖의 파일 (뒤쪽 부분의)







            if (t_filestart <= dtStartTime)



                file_end = true;







            string dsn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};", fi.FullName);







            CommonDbConnection conn = new CommonDbConnection(EnumDbConnectionType.OleDb, dsn, false);







            try



            {



                conn.Open();



            }



            catch



            {



                return file_end;



            }







            if (conn.State != ConnectionState.Open) return file_end;







            string command;







            // 데이터가 많을때는 필요한 부분만 읽으면 훨씬 더 빨라진다.



            //command = String.Format("SELECT * FROM Data ORDER BY [Data Time],MilliSec");



            command = String.Format("SELECT * FROM Data WHERE [Data Time]>={0} AND [Data Time]<={1} ORDER BY [Data Time],MilliSec",



                DbTool.MakeDateTimeString(EnumDbType.MDB, dtStartTime), DbTool.MakeDateTimeString(EnumDbType.MDB, t_to));







            CommonDbDataAdapter adapter = new CommonDbDataAdapter(command, conn);







            DataSet ds = new DataSet();







            Stopwatch sw = new Stopwatch();







            sw.Start();



            try



            {



                adapter.Fill(ds);



                sw.Stop();



                TimeSpan ts = sw.Elapsed;



            }



            catch



            {



                conn.Close();



                return file_end;



            }







            conn.Close();







            int column_time = ds.Tables[0].Columns.IndexOf("Data Time");



            if (column_time == -1)



            {



                return file_end;



            }



            int column_milli = ds.Tables[0].Columns.IndexOf("MilliSec");







            DateTime dbt;



            DateTime t;



            int k = 0;



            DataRow row;



            MD_TREND_MEMBER member;







            t = dtStartTime;







            for (int j = 0; j < blockMember.Count; j++)



            {



                member = (MD_TREND_MEMBER)blockMember[j];



                member.column_pos = ds.Tables[0].Columns.IndexOf(member.column);



            }







            for (int i = 0; i < objArgs.wShowUnit; i++)



            {



                while (true)



                {



                    if (k >= ds.Tables[0].Rows.Count) break;







                    row = ds.Tables[0].Rows[k];







                    dbt = GetDateTimeFromRow(row, column_time, column_milli);







                    if (t == dbt)



                    {



                        for (int j = 0; j < blockMember.Count; j++)



                        {



                            member = (MD_TREND_MEMBER)blockMember[j];







                            if (member.column_pos != -1)



                            {



                                member.point[i].read_flag = true;



                                string s = row[member.column_pos].ToString();



                                member.point[i].val = ConvertTool.ToDouble(s);



                            }



                        }







                        k++;







                        break;



                    }







                    else if (dbt < t)	// 다음 레코드를 읽어야 한다.



                    {



                        k++;



                    }



                    else	// 



                    {



                        break;



                    }



                }







                if (objArgs.wTimeSelectOption == 0) t = t.AddMilliseconds(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 1) t = t.AddSeconds(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 2) t = t.AddMinutes(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 3) t = t.AddHours(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 4) t = t.AddDays(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 5) t = t.AddMonths(objArgs.nDataCycle);



                else t = t.AddYears(objArgs.nDataCycle);



            }







            return file_end;



        }







        public static void SortFis(FileInfo[] array)



        {



            int i, j;







            FileInfo temp;







            int big;



            for (i = 0; i < array.Length; i++)



            {



                big = i;



                for (j = i + 1; j < array.Length; j++)



                {



                    if (String.Compare(array[j].Name, array[big].Name) > 0) big = j;



                }







                if (big != i)



                {



                    temp = array[i];



                    array[i] = array[big];



                    array[big] = temp;



                }



            }



        }







        async Task<bool> ReadAllPointFromLocal(ONE_POINT_STRUCT[][] tempPoints)



        {



            // tempPoints는 이미 초기화되어 있음 (read_flag = false)







            if (objArgs.sDsn.Length == 0) return false;







            try



            {



                // PostgreSQL에서 MilliData 조회



                return await ReadAllPointFromPostgreSQL(tempPoints);



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"PostgreSQL 데이터 조회 오류: {ex.Message}");



                MessageDisplay.Show("MilliData 데이터 조회 오류: {0}", ex.Message);



                return false;



            }



        }







        async Task<bool> ReadAllPointFromPostgreSQL(ONE_POINT_STRUCT[][] tempPoints)



        {



            MD_TREND_MEMBER member;







            // 1. objArgs.sDsn에서 테이블명 패턴 추출



            // sDsn은 MilliData 설정의 title 또는 식별자로 가정



            string milliDataTitle = objArgs.sDsn;







            // 2. 시간 범위 계산



            DateTime startTime = dtStartTime;



            DateTime endTime = CalculateEndTime(startTime);







            // 3. 해당 기간의 테이블 찾기



            List<string> tableNames = await FindMilliDataTables(milliDataTitle, startTime, endTime);







            if (tableNames.Count == 0)



            {



                Debug.WriteLine($"해당 기간의 MilliData 테이블을 찾을 수 없습니다: {milliDataTitle}");



                return false;



            }







            // 각 멤버의 컬럼 위치 초기화



            for (int j = 0; j < blockMember.Count; j++)



            {



                member = (MD_TREND_MEMBER)blockMember[j];



                member.column_pos = -1;



            }







            // 테이블별로 데이터 조회 및 병합



            bool hasData = false;



            foreach (string tableName in tableNames)



            {



                bool result = await ReadDataFromTable(tableName, startTime, endTime, tempPoints);



                if (result) hasData = true;



            }







            return true;



        }







        DateTime CalculateEndTime(DateTime startTime)



        {



            int data_cycle = objArgs.wShowUnit * objArgs.nDataCycle;







            switch (objArgs.wTimeSelectOption)



            {



                case 0: // millisecond



                    return startTime.AddMilliseconds(data_cycle);



                case 1: // second



                    return startTime.AddSeconds(data_cycle);



                case 2: // minute



                    return startTime.AddMinutes(data_cycle);



                case 3: // hour



                    return startTime.AddHours(data_cycle);



                case 4: // day



                    return startTime.AddDays(data_cycle);



                case 5: // month



                    return startTime.AddMonths(data_cycle);



                case 6: // year



                    return startTime.AddYears(data_cycle);



                default:



                    return startTime.AddHours(data_cycle);



            }



        }







        public class MilliDataTableInfo



        {



            public string TableName { get; set; }



            public DateTime StartTime { get; set; }



            public int TimeInterval { get; set; }



            public int CutMethod { get; set; }



            public int SizeCut { get; set; }



            public DateTime EstimatedEndTime { get; set; }



        }







        async Task<List<string>> FindMilliDataTables(string milliDataTitle, DateTime startTime, DateTime endTime)



        {



            List<string> tables = new List<string>();







            try



            {



                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))



                {



                    await connection.OpenAsync().ConfigureAwait(false);







                    // Local Time을 UTC로 변환하여 비교



                    DateTime startTimeUtc = startTime.ToUniversalTime();



                    DateTime endTimeUtc = endTime.ToUniversalTime();







                    // 메타데이터에서 해당 title의 모든 테이블 검색



                    string query = @"



                SELECT table_name, start_time, time_interval, cut_method, size_cut



                FROM history.millidata_metadata



                WHERE title = @title



                AND EXISTS (



                    SELECT 1 FROM information_schema.tables 



                    WHERE table_schema = 'history' 



                    AND table_name = millidata_metadata.table_name



                )



                ORDER BY start_time DESC";







                    using (var command = new NpgsqlCommand(query, connection))



                    {



                        command.Parameters.AddWithValue("title", milliDataTitle);







                        using (var reader = await command.ExecuteReaderAsync())



                        {



                            while (reader.Read())



                            {



                                string tableName = reader.GetString(0);



                                DateTime tableStartTimeUtc = reader.GetDateTime(1);



                                int timeInterval = reader.GetInt32(2);



                                int cutMethod = reader.GetInt32(3);



                                int sizeCut = reader.GetInt32(4);







                                // 테이블의 예상 종료 시간 계산



                                DateTime tableStartTime = tableStartTimeUtc.ToLocalTime();



                                DateTime tableEndTime = CalculateTableEndTime(tableStartTime, cutMethod, sizeCut);







                                // 시간 범위가 겹치는지 확인



                                if (tableEndTime >= startTime && tableStartTime <= endTime)



                                {



                                    tables.Add(tableName);



                                }



                            }



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"테이블 검색 오류: {ex.Message}");



            }







            return tables;



        }







        DateTime CalculateTableEndTime(DateTime tableStartTime, int cutMethod, int sizeCut)



        {



            // MilliData 테이블의 cut_method와 size_cut에 따라 종료 시간 계산



            // checkEngineMilliData.cs의 CuttingCheck 메서드 로직 참고







            switch (cutMethod)



            {



                case 0: // 초 단위



                    return tableStartTime.AddSeconds(sizeCut);







                case 1: // 분 단위



                    return tableStartTime.AddMinutes(sizeCut);







                case 2: // 시간 단위



                    return tableStartTime.AddHours(sizeCut);







                case 3: // 일 단위



                    return tableStartTime.AddDays(sizeCut);







                case 4: // 주 단위 (무조건 1주)



                    return tableStartTime.AddDays(7);







                case 5: // 월 단위



                    return tableStartTime.AddMonths(sizeCut);







                case 6: // 년 단위



                    return tableStartTime.AddYears(sizeCut);







                default:



                    return tableStartTime.AddHours(1); // 기본값



            }



        }







        async Task<bool> ReadDataFromTable(string tableName, DateTime startTime, DateTime endTime, ONE_POINT_STRUCT[][] tempPoints)



        {



            try



            {



                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))



                {



                    await connection.OpenAsync().ConfigureAwait(false);







                    // 1. 먼저 컬럼 정보 확인



                    Dictionary<string, string> columnMapping = await GetColumnMapping(connection, tableName);







                    // Local Time을 UTC로 변환



                    DateTime startTimeUtc = startTime.ToUniversalTime();



                    DateTime endTimeUtc = endTime.ToUniversalTime();







                    // 2. 데이터 조회



                    string query = $@"



                SELECT data_time, interval_ms, *



                FROM history.{tableName}



                WHERE data_time >= @startTime



                AND data_time <= @endTime



                ORDER BY data_time, created_at";







                    using (var command = new NpgsqlCommand(query, connection))



                    {



                        command.Parameters.AddWithValue("startTime", startTimeUtc);



                        command.Parameters.AddWithValue("endTime", endTimeUtc);



                        command.CommandTimeout = 60; // 타임아웃 설정







                        using (var reader = await command.ExecuteReaderAsync())



                        {



                            return ProcessDataRows(reader, columnMapping, tempPoints);



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"테이블 데이터 읽기 오류 ({tableName}): {ex.Message}");



                return false;



            }



        }







        async Task<Dictionary<string, string>> GetColumnMapping(NpgsqlConnection connection, string tableName)



        {



            Dictionary<string, string> mapping = new Dictionary<string, string>();







            try



            {



                string query = @"



            SELECT tag_name, column_name



            FROM history.millidata_tag_metadata



            WHERE table_name = @tableName";







                using (var command = new NpgsqlCommand(query, connection))



                {



                    command.Parameters.AddWithValue("tableName", tableName);







                    using (var reader = await command.ExecuteReaderAsync())



                    {



                        while (await reader.ReadAsync())



                        {



                            string tagName = reader.GetString(0);



                            string columnName = reader.GetString(1);



                            mapping[tagName] = columnName;



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"컬럼 매핑 조회 오류: {ex.Message}");



            }







            return mapping;



        }







        bool ProcessDataRows(NpgsqlDataReader reader, Dictionary<string, string> columnMapping, ONE_POINT_STRUCT[][] tempPoints)



        {



            DateTime currentTime = dtStartTime;



            int currentIndex = 0;



            bool hasData = false;







            while (reader.Read() && currentIndex < objArgs.wShowUnit)



            {



                try



                {



                    // UTC로 저장된 시간을 읽어서 Local Time으로 변환



                    DateTime dataTimeUtc = reader.GetDateTime(reader.GetOrdinal("data_time"));



                    DateTime dataTime = dataTimeUtc.ToLocalTime();







                    // interval_ms 컬럼에서 시간 간격 정보 가져오기



                    int intervalMs = objArgs.nDataCycle; // 기본값



                    try



                    {



                        intervalMs = reader.GetInt32(reader.GetOrdinal("interval_ms"));



                    }



                    catch



                    {



                        // interval_ms 컬럼이 없으면 기본값 사용



                    }







                    // 현재 시간 슬롯과 매칭되는 데이터 찾기



                    while (currentIndex < objArgs.wShowUnit)



                    {



                        if (IsTimeMatch(currentTime, dataTime, intervalMs))



                        {



                            // 데이터 매칭 - 태그별로 값 저장



                            SaveDataToMembers(reader, columnMapping, currentIndex, tempPoints);



                            hasData = true;







                            currentTime = IncrementTime(currentTime);



                            currentIndex++;



                            break;



                        }



                        else if (dataTime > currentTime.Add(GetTimeToleranceSpan(intervalMs)))



                        {



                            // 데이터가 현재 시간보다 너무 미래이므로 시간 슬롯을 앞으로 이동



                            currentTime = IncrementTime(currentTime);



                            currentIndex++;



                        }



                        else



                        {



                            // 데이터가 과거 시점이므로 다음 데이터를 읽음



                            break;



                        }



                    }



                }



                catch (Exception ex)



                {



                    Debug.WriteLine($"데이터 행 처리 오류: {ex.Message}");



                    continue; // 오류가 있는 행은 건너뛰고 계속 처리



                }



            }







            return hasData;



        }











        DateTime IncrementTime(DateTime time)



        {



            switch (objArgs.wTimeSelectOption)



            {



                case 0: return time.AddMilliseconds(objArgs.nDataCycle);



                case 1: return time.AddSeconds(objArgs.nDataCycle);



                case 2: return time.AddMinutes(objArgs.nDataCycle);



                case 3: return time.AddHours(objArgs.nDataCycle);



                case 4: return time.AddDays(objArgs.nDataCycle);



                case 5: return time.AddMonths(objArgs.nDataCycle);



                case 6: return time.AddYears(objArgs.nDataCycle);



                default: return time.AddSeconds(objArgs.nDataCycle);



            }



        }







        bool IsTimeMatch(DateTime expectedTime, DateTime dataTime, int intervalMs)



        {



            // 시간 단위에 따라 비교 정밀도 조정



            switch (objArgs.wTimeSelectOption)



            {



                case 0: // 밀리초



                    // 밀리초 단위에서는 더 정확한 매칭 필요



                    double toleranceMs = Math.Max(intervalMs * 0.5, 50); // 최소 50ms 허용



                    return Math.Abs((dataTime - expectedTime).TotalMilliseconds) <= toleranceMs;







                case 1: // 초



                    return Math.Abs((dataTime - expectedTime).TotalSeconds) <= intervalMs * 0.5;







                case 2: // 분



                    return Math.Abs((dataTime - expectedTime).TotalMinutes) <= intervalMs * 0.5;







                case 3: // 시간



                    return Math.Abs((dataTime - expectedTime).TotalHours) <= intervalMs * 0.5;







                case 4: // 일



                    return Math.Abs((dataTime - expectedTime).TotalDays) <= intervalMs * 0.5;







                case 5: // 월



                    return dataTime.Year == expectedTime.Year &&



                           Math.Abs(dataTime.Month - expectedTime.Month) <= intervalMs * 0.5;







                case 6: // 년



                    return Math.Abs(dataTime.Year - expectedTime.Year) <= intervalMs * 0.5;







                default:



                    return Math.Abs((dataTime - expectedTime).TotalSeconds) <= intervalMs * 0.5;



            }



        }







        // 허용 오차 시간 계산



        TimeSpan GetTimeToleranceSpan(int intervalMs)



        {



            switch (objArgs.wTimeSelectOption)



            {



                case 0: return TimeSpan.FromMilliseconds(intervalMs * 0.5);



                case 1: return TimeSpan.FromSeconds(intervalMs * 0.5);



                case 2: return TimeSpan.FromMinutes(intervalMs * 0.5);



                case 3: return TimeSpan.FromHours(intervalMs * 0.5);



                case 4: return TimeSpan.FromDays(intervalMs * 0.5);



                case 5: return TimeSpan.FromDays(intervalMs * 15); // 월 근사값



                case 6: return TimeSpan.FromDays(intervalMs * 182); // 년 근사값



                default: return TimeSpan.FromSeconds(intervalMs * 0.5);



            }



        }







        //태그별 데이터 저장 메서드



        void SaveDataToMembers(NpgsqlDataReader reader, Dictionary<string, string> columnMapping, int index, ONE_POINT_STRUCT[][] tempPoints)



        {



            for (int j = 0; j < blockMember.Count; j++)



            {



                MD_TREND_MEMBER member = (MD_TREND_MEMBER)blockMember[j];







                try



                {



                    string columnName = GetColumnNameForMember(member, columnMapping);







                    if (!string.IsNullOrEmpty(columnName))



                    {



                        int columnIndex = reader.GetOrdinal(columnName);







                        if (!reader.IsDBNull(columnIndex))



                        {



                            switch (member.nType)



                            {



                                case EnumTagType.AI:



                                    tempPoints[j][index].val = Convert.ToSingle(reader.GetDouble(columnIndex));



                                    tempPoints[j][index].read_flag = true;



                                    break;







                                case EnumTagType.DI:



                                    tempPoints[j][index].val = reader.GetInt32(columnIndex);



                                    tempPoints[j][index].read_flag = true;



                                    break;







                                case EnumTagType.ST:



                                    string strValue = reader.GetString(columnIndex);



                                    if (double.TryParse(strValue, out double numValue))



                                    {



                                        tempPoints[j][index].val = (float)numValue;



                                        tempPoints[j][index].read_flag = true;



                                    }



                                    break;



                            }



                        }



                    }



                }



                catch (Exception ex)



                {



                    // 개별 컬럼 오류는 디버그 로그만 남기고 계속 진행



                    Debug.WriteLine($"멤버 데이터 저장 오류 ({member.tag}): {ex.Message}");



                }



            }



        }







        //  멤버에 해당하는 컬럼명 찾기



        string GetColumnNameForMember(MD_TREND_MEMBER member, Dictionary<string, string> columnMapping)



        {



            // 1. 태그명으로 컬럼명 찾기



            if (columnMapping.ContainsKey(member.tag))



            {



                return columnMapping[member.tag];



            }







            // 2. 컬럼명 직접 사용



            if (!string.IsNullOrEmpty(member.column))



            {



                return member.column;



            }







            // 3. 태그명을 컬럼명으로 변환해서 시도



            string convertedColumnName = ConvertToValidColumnName(member.tag);



            if (columnMapping.ContainsValue(convertedColumnName))



            {



                return convertedColumnName;



            }







            return null;



        }







        // 9. 비동기 컬럼 매핑 가져오기 메서드



        async Task<Dictionary<string, string>> GetColumnMappingAsync(NpgsqlConnection connection, string tableName)



        {



            Dictionary<string, string> mapping = new Dictionary<string, string>();







            try



            {



                string query = @"



            SELECT tag_name, column_name



            FROM history.millidata_tag_metadata



            WHERE table_name = @tableName";







                using (var command = new NpgsqlCommand(query, connection))



                {



                    command.Parameters.AddWithValue("tableName", tableName);







                    using (var reader = await command.ExecuteReaderAsync())



                    {



                        while (reader.Read())



                        {



                            string tagName = reader.GetString(0);



                            string columnName = reader.GetString(1);



                            mapping[tagName] = columnName;



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"컬럼 매핑 조회 오류: {ex.Message}");



            }







            return mapping;



        }







        // 10. ConvertToValidColumnName 메서드 (기존 메서드와 동일)



        static string ConvertToValidColumnName(string tagName)



        {



            if (string.IsNullOrEmpty(tagName)) return "";







            string result = tagName.ToLower()



                .Replace(" ", "_")



                .Replace("-", "_")



                .Replace(".", "_")



                .Replace("(", "_")



                .Replace(")", "_")



                .Replace("[", "_")



                .Replace("]", "_")



                .Replace("/", "_")



                .Replace("\\", "_");







            if (char.IsDigit(result[0]))



            {



                result = "tag_" + result;



            }







            return result;



        }















        bool ReadAllPointFromWeb(ONE_POINT_STRUCT[][] tempPoints)



        {



            // 10.3.2.5 부터 지원



            if (!ConfigVarTotal.IsWebServerVersionEqualOrHigher(10, 3, 2, 5)) return false;







            MD_TREND_MEMBER member;







            // tempPoints는 이미 초기화되어 있음 (read_flag = false)







            string columns = "";







            // 읽기전에 read flag를 Clear한다.



            for (int i = 0; i < blockMember.Count; i++)



            {



                member = (MD_TREND_MEMBER)blockMember[i];







                columns += String.Format("{0},", member.column);



            }







            ServiceLibDataGate sldg = new ServiceLibDataGate();







            sldg.PrepareArg1(columns);







            int retn = sldg.Command("MilliDataTrend", objArgs.sDsn, ConvertTool.ToDateTimeString(dtStartTime), objArgs.wShowUnit, objArgs.nDataCycle, objArgs.wTimeSelectOption);







            if (retn != 1)



            {



                MessageDisplay.Show(sldg.sErrorMessage);



                return false;



            }







            string data = sldg.GetResultString(0);







            DataSet ds = new DataSet();







            ds.ReadXml(new StringReader(data));







            if (ds.Tables.Count == 0) return false;







            DataRow row;



            object obj;



            int index;







            for (int j = 0; j < blockMember.Count; j++)



            {



                member = (MD_TREND_MEMBER)blockMember[j];



                member.column_pos = ds.Tables[0].Columns.IndexOf(member.column);



            }







            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {



                row = ds.Tables[0].Rows[i];







                index = ConvertTool.ToInt32(row["Order"].ToString());







                for (int j = 0; j < blockMember.Count; j++)



                {



                    member = (MD_TREND_MEMBER)blockMember[j];







                    if (member.column_pos == -1) continue;







                    obj = row[member.column_pos].ToString();



                    if (obj == null || obj.ToString().Length == 0)



                        tempPoints[j][index].read_flag = false;



                    else
                    {



                        tempPoints[j][index].read_flag = true;



                        tempPoints[j][index].val = ConvertTool.ToDouble(obj.ToString());



                    }



                }



            }







            return true;



        }







        // 캐시 무효화 메서드 20250701 PSU



        private void InvalidateGraphCache()



        {



            graphCacheValid = false;



            if (cachedGraphImage != null)



            {



                cachedGraphImage.Dispose();



                cachedGraphImage = null;



            }



        }











        [NonSerialized]



        private bool _isLoading = false;







        async Task ReadAllPoint()



        {



            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return;



            if (_isLoading) return; // 이미 로딩 중이면 중복 실행 방지







            _isLoading = true; // 로딩 시작







            try



            {



                // 임시 배열 생성 (더블버퍼링 - 기존 데이터 유지하면서 새 데이터를 임시 배열에 저장)



                int showUnit = objArgs.wShowUnit;



                ONE_POINT_STRUCT[][] tempPoints = new ONE_POINT_STRUCT[blockMember.Count][];



                for (int i = 0; i < blockMember.Count; i++)



                {



                    tempPoints[i] = new ONE_POINT_STRUCT[showUnit];



                    // read_flag는 기본값 false



                }







                bool dataLoaded = false;







                if (ConfigVarTotal.bLocalFlag)



                {



                    dataLoaded = await ReadAllPointFromLocal(tempPoints);



                }



                else



                {



                    dataLoaded = ReadAllPointFromWeb(tempPoints);



                }







                if (dataLoaded)



                {



                    // DB 완료 후 한 번에 교체 (atomic swap)



                    for (int i = 0; i < blockMember.Count; i++)



                    {



                        MD_TREND_MEMBER member = (MD_TREND_MEMBER)blockMember[i];



                        member.point = tempPoints[i];



                    }







                    // 캐시 무효화



                    InvalidateGraphCache();



                    lastDataUpdateTime = DateTime.Now;







                    // 기존 후처리 로직 (범위 계산 등)



                    ProcessMemberData();



                }







            }



            catch (Exception ex)



            {



                Debug.WriteLine($"데이터 읽기 오류: {ex.Message}");



                if (Tools.IsLangKorean())



                    MessageDisplay.Show("미세자료 데이터를 읽는 중 오류가 발생했습니다: " + ex.Message);



                else



                    MessageDisplay.Show("MilliData Read Error: " + ex.Message);



            }



            finally



            {



                _isLoading = false;



            }



        }







        // 12. 멤버 데이터 후처리



        void ProcessMemberData()



        {



            for (int i = 0; i < blockMember.Count; i++)



            {



                MD_TREND_MEMBER member = (MD_TREND_MEMBER)blockMember[i];







                if (member.nType == EnumTagType.AI)



                {



                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                    GetViewFullBase(ai, member, out member.max_value, out member.min_value);



                }



                else



                {



                    member.min_value = 0;



                    member.max_value = 100;



                }







                // 자동 범위 또는 로그 범위 계산 (기존 로직 유지)



                if (bAutoViewRange)



                {



                    CalculateAutoViewRange(member);



                }



                else if (objArgs.logarithmicScale.bUse && member.min_value == 0)



                {



                    CalculateLogarithmicViewRange(member);



                }



            }



        }







        // 13. 자동 범위 계산



        void CalculateAutoViewRange(MD_TREND_MEMBER member)



        {



            int readCount = 0;







            for (int j = 0; j < objArgs.wShowUnit; j++)



            {



                if (member.point[j].read_flag)



                {



                    double val = member.point[j].val;







                    if (objArgs.logarithmicScale.bUse && val == 0)



                        continue;







                    if (readCount == 0)



                    {



                        member.min_value = val;



                        member.max_value = val;



                    }



                    else



                    {



                        if (val < member.min_value) member.min_value = val;



                        if (val > member.max_value) member.max_value = val;



                    }



                    readCount++;



                }



            }



        }







        // 14. 로그 범위 계산



        void CalculateLogarithmicViewRange(MD_TREND_MEMBER member)



        {



            int readCount = 0;







            for (int j = 0; j < objArgs.wShowUnit; j++)



            {



                if (member.point[j].read_flag)



                {



                    double val = member.point[j].val;







                    if (val == 0) continue;







                    double baseValue = 1000000000;



                    for (int k = 0; k < 20; k++)



                    {



                        if (val >= baseValue)



                        {



                            val = baseValue;



                            break;



                        }



                        baseValue /= 10;



                    }







                    if (readCount == 0)



                    {



                        member.min_value = val;



                    }



                    else



                    {



                        if (val < member.min_value)



                            member.min_value = val;



                    }



                    readCount++;



                }



            }



        }







        // 캐시된 그래프 이미지 생성 메서드



        private Bitmap CreateCachedGraphImage(int gx1, int gy1, int gx2, int gy2)



        {



            int width = gx2 - gx1 + 1;



            int height = gy2 - gy1 + 1;







            if (width <= 0 || height <= 0) return null;







            Bitmap bitmap = new Bitmap(width, height);



            using (Graphics g = Graphics.FromImage(bitmap))



            {



                // 고품질 렌더링 설정



                g.SmoothingMode = SmoothingMode.AntiAlias;



                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;







                // 배경 그리기



                Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, 0, 0, width, height);



                if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)



                    DrawClass.PushBox2(g, 0, 0, width, height, RunColorFill.basic_color);



                else if (IsBackBorder())



                    DrawClass.PushBox2(g, 0, 0, width, height, brush);



                else



                    DrawClass.gcls(g, 0, 0, width, height, brush);







                // 가이드라인 그리기



                DrawGraphGuideLine(g, 0, 0, width - 1, height - 1, gx1, gy1, gx2, gy2);







                // 패턴라인 그리기



                DrawPatternLine(g, 0, 0, width - 1, height - 1, gx1, gy1, gx2, gy2);







                // 실제 그래프 데이터 그리기



                DrawGraphData(g, 0, 0, width - 1, height - 1, gx1, gy1, gx2, gy2);



            }







            return bitmap;



        }















        // 가이드라인 그리기 메서드 (좌표 오프셋 적용)



        //   void DrawGraphGuideLine(Graphics g, int dx1, int dy1, int dx2, int dy2, int gx1, int gy1, int gx2, int gy2)



        //   {



        //       int pos;



        //       int i;



        //       int time_devide = GetGuideTimeDivide(gx1, gx2);



        //       GUIDE_DISPLAY guide;



        //       int real_y, real_size;







        //       Pen pen = new Pen(objArgs.lColorGuideLine, 1);







        //       for (int j = 0; j < arrayGuideDisplay.Count; j++)



        //       {



        //           guide = (GUIDE_DISPLAY)arrayGuideDisplay[j];







        //           for (i = 0; i <= objArgs.wLevelDevide; i++)



        //           {



        //               if (i == 0 && guide.from == 0) continue;



        //               if (i == objArgs.wLevelDevide && guide.to == 100) continue;







        //               real_size = (guide.to - guide.from) * (gy2 - gy1) / 100;



        //               real_y = gy2 - (guide.from) * (gy2 - gy1) / 100;



        //               pos = real_y - (real_size) * i / (objArgs.wLevelDevide);







        //               // 좌표 오프셋 적용



        //               int drawY = pos - gy1 + dy1;



        //               g.DrawLine(pen, dx1 + 1, drawY, dx2, drawY);



        //           }



        //       }







        //       ushort w;



        //       DateTime t = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, dtStartTime.Second);











        //       if (objArgs.wTimeSelectOption == 0)



        //       {   // 밀리 데이터



        //           int lastDrawX = -1;



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMilliseconds(objArgs.nDataCycle))



        //           {



        //               if ((t.Second % time_devide) != 0 || t.Millisecond != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               // 좌표 오프셋 적용



        //               int drawX = pos - gx1 + dx1;







        //               // 🔥 픽셀 간격 보장 (핵심)



        //               if (lastDrawX != -1 &&



        //   drawX - lastDrawX < MIN_GUIDE_PIXEL_SPACING)



        //                   continue;







        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);







        //               lastDrawX = drawX;



        //           }



        //       }



        //       else if (objArgs.wTimeSelectOption == 1)



        //       {   // 초 데이터



        //           int lastDrawX = -1;



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddSeconds(objArgs.nDataCycle))



        //           {



        //               if (((t.Second + t.Minute * 60) % time_devide) != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               int drawX = pos - gx1 + dx1;







        //               // 🔥 픽셀 간격 보장 (핵심)



        //               if (lastDrawX != -1 &&



        //drawX - lastDrawX < MIN_GUIDE_PIXEL_SPACING)



        //                   continue;







        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);







        //               lastDrawX = drawX;



        //           }



        //       }



        //       else if (objArgs.wTimeSelectOption == 2)



        //       {   // 분 데이터



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMinutes(objArgs.nDataCycle))



        //           {



        //               if (((t.Minute + t.Hour * 60) % time_devide) != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               int drawX = pos - gx1 + dx1;



        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



        //           }



        //       }



        //       else if (objArgs.wTimeSelectOption == 3)



        //       {   // 시간 데이터



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddHours(objArgs.nDataCycle))



        //           {



        //               if (((t.Hour + (t.Day - 1) * 24) % time_devide) != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               int drawX = pos - gx1 + dx1;



        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



        //           }



        //       }



        //       else if (objArgs.wTimeSelectOption == 4)



        //       {   // 일일 데이터



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddDays(objArgs.nDataCycle))



        //           {



        //               if (((t.Day - 1) % time_devide) != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               int drawX = pos - gx1 + dx1;



        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



        //           }



        //       }



        //       else if (objArgs.wTimeSelectOption == 5)



        //       {   // 월간 데이터.



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMonths(objArgs.nDataCycle))



        //           {



        //               if (((t.Month - 1) % time_devide) != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               int drawX = pos - gx1 + dx1;



        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



        //           }



        //       }



        //       else if (objArgs.wTimeSelectOption == 6)



        //       {   // 연간 데이터.



        //           for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddYears(objArgs.nDataCycle))



        //           {



        //               if (((t.Year) % time_devide) != 0) continue;







        //               if (objArgs.wShowUnit - 1 == 0) continue;



        //               pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //               int drawX = pos - gx1 + dx1;



        //               g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



        //           }



        //       }



        //   }







        // 패턴라인 그리기 메서드 (좌표 오프셋 적용)







        void DrawGraphGuideLine(Graphics g,



            int dx1, int dy1, int dx2, int dy2,



            int gx1, int gy1, int gx2, int gy2)



        {



            int time_devide = GetDisplayTimeDevide(gx1, gx2);







            Pen pen = new Pen(objArgs.lColorGuideLine, 1);







            DateTime t = dtStartTime;







            int lastDrawX = -1;



            int width = gx2 - gx1;







            for (ushort w = 0; w < objArgs.wShowUnit; w++)



            {



                if (!IsDivideMatch(t, time_devide))



                {



                    t = AddStep(t);



                    continue;



                }







                int pos = gx1 + (int)((long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));



                int drawX = pos - gx1 + dx1;







                //  픽셀 간격 보장



                if (lastDrawX >= 0 &&



                    drawX - lastDrawX < MIN_GUIDE_PIXEL_SPACING)



                {



                    t = AddStep(t);



                    continue;



                }







                g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);







                lastDrawX = drawX;



                t = AddStep(t);



            }



            pen.Dispose();
        }







        void DrawPatternLine(Graphics g, int dx1, int dy1, int dx2, int dy2, int gx1, int gy1, int gx2, int gy2)



        {



            if (!bVisiblePatternLine) return;



            if (arrayPatternLine == null) return;



            if (arrayPatternLine.Count == 0) return;







            PatternLineItem pli;







            for (int i = 0; i < arrayPatternLine.Count; i++)



            {



                pli = arrayPatternLine[i];



                DrawPatternLineOne(pli, g, dx1, dy1, dx2, dy2, gx1, gy1, gx2, gy2);



            }



        }







        // 패턴라인 개별 그리기 (좌표 오프셋 적용 - 기존 메서드 수정 필요)



        void DrawPatternLineOne(PatternLineItem pli, Graphics g, int dx1, int dy1, int dx2, int dy2, int gx1, int gy1, int gx2, int gy2)



        {



            if (!pli.visible) return;







            DateTime tPatternEnd = pli.tStart.AddSeconds(pli.pattern_cycle * (pli.line_count - 1));   // 마지막 시간







            if (tPatternEnd < dtStartTime) return;  // 이전 시간대이다.







            TimeSpan ts = dtStartTime - pli.tStart;



            long ms_StartTime = ts.Ticks / TimeSpan.TicksPerMillisecond;



            long cycle = (int)(pli.pattern_cycle * 1000);



            long remain = 0;



            int shift = 0;



            remain = Math.Abs(ms_StartTime % cycle);







            if (objArgs.wTimeSelectOption == 0)



            {



                shift = (int)(remain);



            }



            else if (objArgs.wTimeSelectOption == 1)



            {



                shift = (int)(remain / 1000);



            }



            else if (objArgs.wTimeSelectOption == 2)



            {



                shift = (int)(remain / (1000 * 60));



            }



            else if (objArgs.wTimeSelectOption == 3)



            {



                shift = (int)(remain / (1000 * 60 * 60));



            }



            else if (objArgs.wTimeSelectOption == 4)



            {



                shift = (int)(remain / (1000 * 60 * 60 * 24));



            }







            if (ms_StartTime < 0)



                shift *= -1;







            DateTime t = dtStartTime;







            Pen pen = new Pen(pli.color, pli.thick);



            ushort w;



            int pos;



            int p_i = shift;







            if (objArgs.wShowUnit <= 1) return; // 숫자가 적어서 계산할 수 없다.







            long ms_PatternStart = pli.tStart.Ticks / TimeSpan.TicksPerMillisecond; // tStart를 ms로 환산한 값



            long ms_t = ms_StartTime;   // 밀리초로 환산



            string buf;



            int x;



            int number;



            Font font = MakeFont();



            if (objArgs.wTimeSelectOption == 0)



            {   // 밀리 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddMilliseconds(objArgs.nDataCycle), ms_t += objArgs.nDataCycle)



                {



                    if (ms_t % cycle != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    // 좌표 오프셋 적용



                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen1 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen1, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush1 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush1, x, dy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 1)



            {   // 초 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddSeconds(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000)



                {



                    if (ms_t % cycle != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen2 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen2, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush2 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush2, x, dy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 2)



            {   // 분 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddMinutes(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000 * 60)



                {



                    if (ms_t % cycle != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen3 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen3, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush3 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush3, x, dy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 3)



            {   // 시간 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddHours(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000 * 60 * 60)



                {



                    if (ms_t % cycle != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen4 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen4, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush4 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush4, x, dy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 4)



            {   // 일일 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddDays(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000 * 60 * 60 * 24)



                {



                    if (ms_t % cycle != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen5 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen5, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush5 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush5, x, dy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 5)



            {   // 월간 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddMonths(objArgs.nDataCycle))



                {



                    // 월간 데이터는 ms 계산이 복잡하므로 단순 방식 사용



                    TimeSpan tsDiff = t - pli.tStart;



                    double totalMonths = (t.Year - pli.tStart.Year) * 12 + (t.Month - pli.tStart.Month);







                    if ((totalMonths % pli.pattern_cycle) != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    number = (int)(totalMonths / pli.pattern_cycle);







                    if (number == 0)



                    {



                        using (Pen linePen6 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen6, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush6 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush6, x, dy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 6)



            {   // 년간 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddYears(objArgs.nDataCycle))



                {



                    double totalYears = t.Year - pli.tStart.Year;







                    if ((totalYears % pli.pattern_cycle) != 0) continue;







                    if (t > tPatternEnd) break;



                    if (t < pli.tStart) continue;







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    int drawX = pos - gx1 + dx1;



                    if (drawX < dx1 || drawX > dx2) continue;







                    number = (int)(totalYears / pli.pattern_cycle);







                    if (number == 0)



                    {



                        using (Pen linePen7 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen7, drawX, dy1 + 1, drawX, dy2);



                    }



                    else



                    {



                        g.DrawLine(pen, drawX, dy1 + 1, drawX, dy2);



                    }







                    buf = String.Format("{0}", number + pli.start_number);



                    x = CalcPatternLineNumberPosition(g, pli, drawX, font, buf, number);



                    using (Brush textBrush7 = new SolidBrush(pli.color))
                        g.DrawString(buf, font, textBrush7, x, dy1);



                }



            }



            pen.Dispose();
        }











        // DrawGraphData 메서드 완성 (좌표 오프셋 적용)



        void DrawGraphData(Graphics g, int dx1, int dy1, int dx2, int dy2, int gx1, int gy1, int gx2, int gy2)



        {



            int sizex, sizey;



            int x, y = 0;



            double gaby;



            ushort j;



            int l;



            ushort pos;



            MD_TREND_MEMBER member;



            TagAiClass ai = null;



            int point_radios;



            int view_y1;



            int view_y2;



            int view_sizey;



            bool moveto_flag = false;



            int move_x = 0, move_y = 0, line_y;



            Font font = MakeFont();



            // Brush brush removed - was never used (GDI leak)







            point_radios = (gy2 - gy1 + 1) * objArgs.pub.wPointSize / 1000;



            if (point_radios < 1) point_radios = 1;







            sizex = gx2 - gx1;



            sizey = gy2 - gy1 + 1;







            int bar = 0;







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                if (member.nGraphType == 1) bar++;







                if (member.visible == 0) continue;







                if (member.nType == EnumTagType.AI)



                {



                    ai = GetRealTagAI(member.tag, ref member.nPos);



                    gaby = member.max_value - member.min_value;



                }



                else if (member.nType == EnumTagType.DI)



                {



                    gaby = 100;



                }



                else



                {



                    gaby = member.view_full - member.view_base;



                }







                view_y1 = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;



                view_y2 = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;



                view_sizey = view_y2 - view_y1 + 1;







                // AI 태그일 때만 경계치 표시



                if (member.nType == EnumTagType.AI)



                {



                    Pen hPenLimit = new Pen(member.color, 1);



                    hPenLimit.DashStyle = DashStyle.Dot;







                    if ((member.wFlags & 0x0001) > 0)



                    {   // hihi



                        if (gaby == 0)



                            y = 0;



                        else



                            y = (int)((double)view_sizey * (ai.hihi - member.min_value) / gaby);



                        if (y < 0) y = 0;



                        if (y >= view_sizey) y = view_sizey - 1;







                        if (member.nReverseY != 0)



                            y = view_y1 + y;



                        else



                            y = view_y2 - y;







                        hPenLimit.Color = objArgs.pub.colorHiHi;







                        // 좌표 오프셋 적용



                        int drawY = y - gy1 + dy1;



                        g.DrawLine(hPenLimit, dx1 + 1, drawY, dx2, drawY);



                    }







                    if ((member.wFlags & 0x0002) > 0)



                    {   // high



                        if (gaby == 0)



                            y = 0;



                        else



                            y = (int)((double)view_sizey * (ai.high - member.min_value) / gaby);



                        if (y < 0) y = 0;



                        if (y >= view_sizey) y = view_sizey - 1;







                        if (member.nReverseY != 0)



                            y = view_y1 + y;



                        else



                            y = view_y2 - y;







                        hPenLimit.Color = objArgs.pub.colorHigh;







                        int drawY = y - gy1 + dy1;



                        g.DrawLine(hPenLimit, dx1 + 1, drawY, dx2, drawY);



                    }







                    if ((member.wFlags & 0x0004) > 0)



                    {   // low



                        if (gaby == 0)



                            y = 0;



                        else



                            y = (int)((double)view_sizey * (ai.low - member.min_value) / gaby);



                        if (y < 0) y = 0;



                        if (y >= view_sizey) y = view_sizey - 1;







                        if (member.nReverseY != 0)



                            y = view_y1 + y;



                        else



                            y = view_y2 - y;







                        hPenLimit.Color = objArgs.pub.colorLow;







                        int drawY = y - gy1 + dy1;



                        g.DrawLine(hPenLimit, dx1 + 1, drawY, dx2, drawY);



                    }







                    if ((member.wFlags & 0x0008) > 0)



                    {   // lolo



                        if (gaby == 0)



                            y = 0;



                        else



                            y = (int)((double)view_sizey * (ai.lolo - member.min_value) / gaby);



                        if (y < 0) y = 0;



                        if (y >= view_sizey) y = view_sizey - 1;







                        if (member.nReverseY != 0)



                            y = view_y1 + y;



                        else



                            y = view_y2 - y;







                        hPenLimit.Color = objArgs.pub.colorLoLo;







                        int drawY = y - gy1 + dy1;



                        g.DrawLine(hPenLimit, dx1 + 1, drawY, dx2, drawY);



                    }



                    hPenLimit.Dispose();
                }







                Pen pen = new Pen(member.color, member.nLineThick);



                pos = 0;



                moveto_flag = false;



                List<Point> continuousPoints = new List<Point>();







                for (j = 0; j < objArgs.wShowUnit; j++, pos++)



                {



                    if (objArgs.wShowUnit - 1 == 0)



                        x = 0;



                    else



                        x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));







                    if (gaby == 0)



                        y = 0;



                    else



                    {



                        if (member.point[j].read_flag)



                        {



                            if (member.nType == EnumTagType.AI)



                            {



                                if (objArgs.logarithmicScale.bUse)



                                {



                                    double log_full, log_base;







                                    if (member.max_value == 0)



                                        log_full = 0;



                                    else



                                        log_full = Math.Log(member.max_value, objArgs.logarithmicScale.fBase);







                                    if (member.min_value == 0)



                                        log_base = 0;



                                    else



                                        log_base = Math.Log(member.min_value, objArgs.logarithmicScale.fBase);







                                    double log_val = Math.Log(TagUtil.GetDisplayValue(ai, member.point[j].val), objArgs.logarithmicScale.fBase);



                                    double log_gab = log_full - log_base;







                                    if (log_gab == 0)



                                        y = 0;



                                    else



                                        y = (int)((double)view_sizey * (log_val - log_base) / log_gab);



                                }



                                else



                                {



                                    y = (int)((double)view_sizey * (TagUtil.GetDisplayValue(ai, member.point[j].val) - member.min_value) / gaby);



                                }



                            }



                            else if (member.nType == EnumTagType.DI)



                            {



                                if (member.point[j].val > 0)



                                    y = (int)(view_sizey * 0.9);



                                else



                                    y = (int)(view_sizey * 0.1);



                            }



                            else



                            {



                                y = (int)((double)view_sizey * (member.point[j].val - member.min_value) / gaby);



                            }



                        }



                    }







                    if (y < 0) y = 0;



                    if (y >= view_sizey) y = view_sizey - 1;







                    if ((j % nDataGab) == 0 || j == objArgs.wShowUnit - 1)



                    {



                        switch (member.nGraphType)



                        {



                            case 1: // Bar (enhanced)



                                if (member.point[pos].read_flag)



                                {



                                    int thick = (sizex - objArgs.wShowUnit) / (objArgs.wShowUnit);



                                    if (thick < 1) thick = 1;



                                    if (member.nLineThick < thick) thick = member.nLineThick;



                                    int left_thick = (thick - 1) / 2;



                                    int right_thick = (thick) / 2;



                                    int x1b, x2b;







                                    if (objArgs.wShowUnit - 1 == 0) x = 0;



                                    else x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));



                                    move_x = x + dx1;







                                    if (j == 0) { x1b = move_x; x2b = move_x + right_thick; }



                                    else if (j == objArgs.wShowUnit - 1) { x1b = move_x - left_thick; x2b = move_x; }



                                    else { x1b = move_x - left_thick; x2b = move_x + right_thick; }







                                    using (Brush barBrush = new SolidBrush(Color.FromArgb(180, member.color)))



                                    using (Pen borderPen = new Pen(member.color, 1))



                                    {



                                        if (member.nReverseY == 1)



                                        {



                                            int ry = dy1 + (view_y1 - gy1) + y;



                                            int by = dy1 + (view_y1 - gy1);



                                            Rectangle barRect = new Rectangle(x1b, by, x2b - x1b, ry - by);



                                            if (ry - by > 0)

                                            {

                                                DrawClass.gcls(g, x1b, by, x2b, ry, barBrush);

                                                if (x2b - x1b > 1) g.DrawRectangle(borderPen, x1b, by, x2b - x1b, ry - by);

                                            }



                                        }



                                        else



                                        {



                                            int ry = dy1 + (view_y2 - gy1) - y;



                                            int by = dy1 + (view_y2 - gy1);



                                            Rectangle barRect = new Rectangle(x1b, ry, x2b - x1b, by - ry);



                                            if (by - ry > 0)

                                            {

                                                DrawClass.gcls(g, x1b, ry, x2b, by, barBrush);

                                                if (x2b - x1b > 1) g.DrawRectangle(borderPen, x1b, ry, x2b - x1b, by - ry);

                                            }



                                        }



                                    }



                                }



                                break;







                            case 2: // Area



                            case 3: // Spline



                            case 4: // StepLine



                                if (member.point[j].read_flag)



                                {



                                    int ry;



                                    if (member.nReverseY == 1) ry = dy1 + (view_y1 - gy1) + y;



                                    else ry = dy1 + (view_y2 - gy1) - y;



                                    continuousPoints.Add(new Point(x + dx1, ry));



                                }



                                else



                                {



                                    if (continuousPoints.Count > 0)



                                    {



                                        int baseY = member.nReverseY == 1 ? dy1 + (view_y1 - gy1) : dy1 + (view_y2 - gy1);



                                        FlushMilliGraphSegment(g, pen, continuousPoints, member.nGraphType, baseY, member.color, member.nPointType, point_radios);



                                        continuousPoints.Clear();



                                    }



                                }



                                break;







                            default: // Line (0)



                                if (member.point[j].read_flag)



                                {



                                    if (member.nReverseY == 1)



                                        line_y = dy1 + (view_y1 - gy1) + y;



                                    else



                                        line_y = dy1 + (view_y2 - gy1) - y;







                                    if (moveto_flag == false)



                                    { move_x = x + dx1; move_y = line_y; moveto_flag = true; }



                                    else



                                    { g.DrawLine(pen, move_x, move_y, x + dx1, line_y); move_x = x + dx1; move_y = line_y; }







                                    if (member.nPointType == 0)



                                        DrawClass.gcls(g, x + dx1, line_y, x + dx1, line_y, member.color);



                                    else



                                        DisplayPoint(g, x + dx1, line_y, member.color, member.nPointType, point_radios);



                                }



                                else



                                {



                                    moveto_flag = false;



                                }



                                break;



                        }



                    }



                }







                // Flush remaining points for Area/Spline/StepLine



                if (continuousPoints.Count > 0 && member.nGraphType >= 2 && member.nGraphType <= 4)



                {



                    int baseY = member.nReverseY == 1 ? dy1 + (view_y1 - gy1) : dy1 + (view_y2 - gy1);



                    FlushMilliGraphSegment(g, pen, continuousPoints, member.nGraphType, baseY, member.color, member.nPointType, point_radios);



                    continuousPoints.Clear();



                }



                pen.Dispose();
            }



        }







        private void FlushMilliGraphSegment(Graphics g, Pen pen, List<Point> points, int graphType, int baseY, Color color, int pointType, int point_radios)



        {



            if (points.Count < 1) return;







            if (graphType == 2) // Area



            {



                if (points.Count >= 2)



                {



                    Point[] polygon = new Point[points.Count + 2];



                    for (int i = 0; i < points.Count; i++) polygon[i] = points[i];



                    polygon[points.Count] = new Point(points[points.Count - 1].X, baseY);



                    polygon[points.Count + 1] = new Point(points[0].X, baseY);



                    using (Brush areaBrush = new SolidBrush(Color.FromArgb(80, color)))



                    { g.FillPolygon(areaBrush, polygon); }



                    g.DrawLines(pen, points.ToArray());



                }



            }



            else if (graphType == 3) // Spline



            {



                if (points.Count >= 3) g.DrawCurve(pen, points.ToArray(), 0.5f);



                else if (points.Count == 2) g.DrawLine(pen, points[0], points[1]);



                else if (points.Count == 1) g.DrawLine(pen, points[0].X, points[0].Y, points[0].X, points[0].Y);



            }



            else if (graphType == 4) // StepLine



            {



                if (points.Count >= 2)



                {



                    List<Point> expanded = new List<Point>(points.Count * 2);



                    expanded.Add(points[0]);



                    for (int i = 1; i < points.Count; i++)



                    {



                        expanded.Add(new Point(points[i].X, points[i - 1].Y));



                        expanded.Add(points[i]);



                    }



                    g.DrawLines(pen, expanded.ToArray());



                }



            }







            if (pointType > 0)



            {



                foreach (Point pt in points)



                    DisplayPoint(g, pt.X, pt.Y, color, pointType, point_radios);



            }



        }











        CONFIG_DATABASE_TREND LoadTrendConfig()



        {



            CONFIG_DATABASE_TREND cfg = new CONFIG_DATABASE_TREND();







            string path = ConfigVarTotal.sDirConfigUser + "\\DatabaseTrend";



            string filename = path + "\\" + objGeneral.GetClassName();







            if (File.Exists(filename))



            {



                Stream s = File.OpenRead(filename);



                BinaryFormatter format = new BinaryFormatter();







                try



                {



                    cfg = (CONFIG_DATABASE_TREND)format.Deserialize(s);



                }



                catch



                {



                    cfg = new CONFIG_DATABASE_TREND();



                }



                s.Close();



            }







            return cfg;



        }







        void SaveMultiTrendConfig(CONFIG_DATABASE_TREND cfg)



        {



            string path = ConfigVarTotal.sDirConfigUser + "\\DatabaseTrend";







            Directory.CreateDirectory(path);







            string filename;







            if (objGeneral.sClassName.Length == 0)



                filename = path + "\\" + "NoName";



            else



                filename = path + "\\" + objGeneral.sClassName;







            Stream s = File.OpenWrite(filename);



            s.Flush();



            BinaryFormatter format = new BinaryFormatter();



            format.Serialize(s, cfg);



            s.Close();



        }







        void LoadConfigAndExcute()



        {



            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



            {



                CONFIG_DATABASE_TREND cfg = LoadTrendConfig();







                //objArgs.wTimeSelectOption = cfg.cTimeZone; 



                //SetShowUnit(cfg.wShowUnit);



                cGraphStartTimeMethod = cfg.cStartMethod;



                bAutoViewRange = cfg.bAutoRange;



                bAutoGuideLine = cfg.bAutoGuideLine;



                bDisplayPointDate = cfg.bDisplayPointDate;



                bUseLocalRange = cfg.bUseLocalRange;



                //nDataGab = cfg.nDataGab;



                //memcpy(&dStartTime, &cfg.dStart, sizeof(dStartTime));



                //memcpy(&tStartTime, &cfg.tStart, sizeof(tStartTime));



                dtStartTime = cfg.dtStart;



                bShowToolTip = cfg.bShowToolTip;







                if (cfg.nMovePeriod == 0) cfg.nMovePeriod = 1;



                nMovePeriod = cfg.nMovePeriod; //20250306 PSU



            }



            else



            {



                DateTime t = DateTimeServer.Now;



            }







            CalcStartTime();



            //ReadAllPoint();



        }







        TagAiClass GetRealTagAI(string tag, ref int[] tag_pos)



        {



            TagAiClass ai = TagLib.GetStructAI(tag, ref tag_pos);







            //if(ai.cTagType == 3) 



            //{	// 간접태그



            //	if(ai.assign.pos != -1) 



            //	{



            //		return &terminalStruct[0].analogInput[ai.assign.pos];



            //	}



            //}







            return ai;



        }







        TagDiClass GetRealTagDI(string tag, ref int[] tag_pos)



        {



            TagDiClass di = TagLib.GetStructDI(tag, ref tag_pos);







            //if(ai.cTagType == 3) 



            //{	// 간접태그



            //	if(ai.assign.pos != -1) 



            //	{



            //		return &terminalStruct[0].analogInput[ai.assign.pos];



            //	}



            //}







            return di;



        }







        void GetViewFullBase(TagAiClass ai, MD_TREND_MEMBER member, out double view_full, out double view_base)



        {



            if (bUseLocalRange)



            {



                view_full = member.view_full;



                view_base = member.view_base;



            }



            else



            {



                view_full = ai.view_full;



                view_base = ai.view_base;



            }



        }







        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)



        {



            if (x1 > x2) Tools.Temp(ref x1, ref x2);



            if (y1 > y2) Tools.Temp(ref y1, ref y2);







            int gx1 = 0, gy1 = 0, gx2 = 0, gy2 = 0;







            GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);







            //CE 소스 업데이트 시 삭제 20250206 PSU



            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)



            {



                objArgs.pub.colorPanelBack = Color.FromArgb(0xc0, 0xc0, 0xc0);



                objArgs.pub.colorPanelText = Color.Black;



            }







            if (objArgs.bUseToolBar) //툴바 사용 여부. 20250306 PSU 추가



            {



                // 툴바 버튼이 초기화되지 않았고 formParent가 있으면 초기화



                if (toolbarButtons == null && formParent != null && TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



                {



                    CreateToolbarButtons(formParent);



                }







                // 그래픽 객체 표시 후 툴바 위치 업데이트



                // 그래픽 객체 표시 후 툴바 위치 업데이트 (런타임 모드일 때만)



                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



                {



                    //formParent.SuspendLayout();



                    UpdateToolbarPosition(g, x1, y1, x2, y2);



                    UpdateControlState(); //20250312 PSU



                    //formParent.ResumeLayout(false);



                }



                else



                {// 편집 모드일 때 툴바 영역에 이미지만 그리기



                    DrawToolbarPlaceholder(g, x1, y1, x2, y2);



                }



            }







            if (IsBackBorder())



            {



                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);



                DrawClass.PopBox2(g, x1, y1, x2, y2, brushback);



            }







            DisplayDescriptionX(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);



            DisplayDescriptionY(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);



            DisplayTagColor(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);







            // PatternLine 때문에 보호 영역이 필요하다.



            //g.SetClip(new Rectangle(gx1, gy1, gx2 - gx1, gy2 - gy1));



            g.SetClip(new Rectangle(gx1 - 1, gy1 - 1, gx2 - gx1 + 2, gy2 - gy1 + 2)); //20250207 PSU 그래프영역 잘려서 1px 확장.



            DisplayGraph(g, gx1, gy1, gx2, gy2);



            g.ResetClip();







            DisplayCursor(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);



        }







        // 수정된 DisplayGraph 메서드



        void DisplayGraph(Graphics g, int gx1, int gy1, int gx2, int gy2)



        {



            Size currentSize = new Size(gx2 - gx1 + 1, gy2 - gy1 + 1);







            // 캐시가 유효한지 확인



            bool needsUpdate = !graphCacheValid ||



                               cachedGraphImage == null ||



                               cachedGraphSize != currentSize ||



                               (objArgs.bAutoUpdate && DateTime.Now.Subtract(lastDataUpdateTime).TotalSeconds > 1);







            if (needsUpdate)



            {



                // 기존 캐시 정리



                if (cachedGraphImage != null)



                {



                    cachedGraphImage.Dispose();



                    cachedGraphImage = null;



                }







                // 새 캐시 이미지 생성



                cachedGraphImage = CreateCachedGraphImage(gx1, gy1, gx2, gy2);



                cachedGraphSize = currentSize;



                graphCacheValid = true;



            }







            // 캐시된 이미지 그리기



            if (cachedGraphImage != null)



            {



                g.DrawImage(cachedGraphImage, gx1, gy1);



            }



        }







        [NonSerialized]



        private bool _lastEnabledState = true; // 기본 상태 저장







        private void UpdateControlState()



        {



            bool shouldBeEnabled = IsViewModeControl();







            if (_lastEnabledState != shouldBeEnabled)



            {



                for (int i = 0; i < toolbarButtons.Length; i++)



                {



                    toolbarButtons[i].Enabled = shouldBeEnabled;



                }



                _lastEnabledState = shouldBeEnabled;



            }



        }







        private bool IsViewModeControl()



        {



            // FormGraphicChild의 viewMode 확인



            if (formParent != null && formParent is GraphicModule.FormGraphicChild)



            {



                GraphicModule.FormGraphicChild parentForm = (GraphicModule.FormGraphicChild)formParent;



                return parentForm.cViewMode == EnumViewMode.CONTROL;



            }



            return true; // 기본값은 컨트롤 모드임



        }











        //void DisplayDescriptionX(Graphics g, int x1, int y1, int x2, int y2, int gx1, int gy1, int gx2, int gy2)



        //{



        //    if (!IsDesX()) return;







        //    int pos;



        //    ushort w;



        //    string buf;



        //    RECT r = new RECT();



        //    SizeF size;



        //    int time_devide = GetGuideTimeDivide(gx1, gx2);



        //    int displayed_pos_up = -1;



        //    int displayed_pos_down = -1;







        //    DateTime t = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, dtStartTime.Second);



        //    Brush brush = new SolidBrush(RunColorText);



        //    Font font = MakeFont();



        //    StringFormat format = new StringFormat();







        //    if (objArgs.wTimeSelectOption == 0)



        //    {	// 밀리초 데이터.



        //        int save_sec = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMilliseconds(objArgs.nDataCycle))



        //        {



        //            // 분할은 초단위로 한다.



        //            if ((t.Second % time_devide) != 0 || t.Millisecond != 0) continue;



        //            //continue;







        //            //if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //            //buf = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", t.Hour, t.Minute, t.Second, t.Millisecond);   // 너무 크다



        //            buf = String.Format("{0:00}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);







        //            size = g.MeasureString(buf, font);







        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_up = r.right;



        //            }







        //            if (save_sec != t.Second)



        //            {



        //                save_sec = t.Second;



        //                buf = String.Format("{0}/{1}", t.Month, t.Day);



        //                size = g.MeasureString(buf, font);



        //                r.left = (int)(pos - size.Width / 2);



        //                if (r.left <= x1) r.left = x1 + 1;



        //                r.right = (int)(r.left + size.Width);



        //                r.top = gy2 + 1 + nCharHeight;



        //                r.bottom = r.top + nCharHeight;







        //                if (r.right >= x2)



        //                {



        //                    r.right = x2;



        //                    r.left = (int)(r.right - size.Width);



        //                }







        //                if (r.left > displayed_pos_down)



        //                {



        //                    format.Alignment = StringAlignment.Center;



        //                    format.LineAlignment = StringAlignment.Center;







        //                    DrawClass.DrawText(g, buf, font, brush, r, format);



        //                    displayed_pos_down = r.right;



        //                }



        //            }



        //        }



        //    }



        //    else if (objArgs.wTimeSelectOption == 1)



        //    {	// 초 데이터.



        //        int save_min = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddSeconds(objArgs.nDataCycle))



        //        {



        //            //if ((t.Second % time_devide) != 0) continue;



        //            if (((t.Second + t.Minute * 60) % time_devide) != 0) continue;    // 범위를 넘어서면 선이 그려지지 않는다. MultiTrend와 같은 방식 사용 2019-8-19







        //            if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //            buf = String.Format("{0:00}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);







        //            size = g.MeasureString(buf, font);







        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_up = r.right;



        //            }







        //            if (save_min != t.Minute)



        //            {



        //                save_min = t.Minute;



        //                buf = String.Format("{0}/{1}", t.Month, t.Day);



        //                size = g.MeasureString(buf, font);



        //                r.left = (int)(pos - size.Width / 2);



        //                if (r.left <= x1) r.left = x1 + 1;



        //                r.right = (int)(r.left + size.Width);



        //                r.top = gy2 + 1 + nCharHeight;



        //                r.bottom = r.top + nCharHeight;







        //                if (r.right >= x2)



        //                {



        //                    r.right = x2;



        //                    r.left = (int)(r.right - size.Width);



        //                }







        //                if (r.left > displayed_pos_down)



        //                {



        //                    format.Alignment = StringAlignment.Center;



        //                    format.LineAlignment = StringAlignment.Center;







        //                    DrawClass.DrawText(g, buf, font, brush, r, format);



        //                    displayed_pos_down = r.right;



        //                }



        //            }



        //        }



        //    }



        //    else if (objArgs.wTimeSelectOption == 2)



        //    {	// 분 데이터.



        //        int save_hour = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMinutes(objArgs.nDataCycle))



        //        {



        //            //if ((t.Minute % time_devide) != 0) continue;



        //            if (((t.Minute + t.Hour * 60) % time_devide) != 0) continue;    // 범위를 넘어서면 선이 그려지지 않는다. MultiTrend와 같은 방식 사용 2019-8-19







        //            if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //            buf = String.Format("{0:00}:{1:00}", t.Hour, t.Minute);







        //            size = g.MeasureString(buf, font);







        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_up = r.right;



        //            }







        //            if (save_hour != t.Hour)



        //            {



        //                save_hour = t.Hour;



        //                buf = String.Format("{0}/{1}", t.Month, t.Day);



        //                size = g.MeasureString(buf, font);



        //                r.left = (int)(pos - size.Width / 2);



        //                if (r.left <= x1) r.left = x1 + 1;



        //                r.right = (int)(r.left + size.Width);



        //                r.top = gy2 + 1 + nCharHeight;



        //                r.bottom = r.top + nCharHeight;







        //                if (r.right >= x2)



        //                {



        //                    r.right = x2;



        //                    r.left = (int)(r.right - size.Width);



        //                }







        //                if (r.left > displayed_pos_down)



        //                {



        //                    format.Alignment = StringAlignment.Center;



        //                    format.LineAlignment = StringAlignment.Center;







        //                    DrawClass.DrawText(g, buf, font, brush, r, format);



        //                    displayed_pos_down = r.right;



        //                }



        //            }



        //        }



        //    }



        //    else if (objArgs.wTimeSelectOption == 3)



        //    {	// 시간 데이터.



        //        int save_day = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddHours(objArgs.nDataCycle))



        //        {



        //            //if ((t.Hour % time_devide) != 0) continue;



        //            if (((t.Hour + (t.Day - 1) * 24) % time_devide) != 0) continue; // 범위를 넘어서면 선이 그려지지 않는다. MultiTrend와 같은 방식 사용 2019-8-19







        //            if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //            buf = String.Format("{0:00}:{1:00}", t.Hour, 0);







        //            size = g.MeasureString(buf, font);







        //            size = g.MeasureString(buf, font);



        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_up = r.right;



        //            }







        //            if (save_day != t.Day)



        //            {



        //                save_day = t.Day;







        //                buf = String.Format("{0}/{1}", t.Month, t.Day);



        //                size = g.MeasureString(buf, font);



        //                r.left = (int)(pos - size.Width / 2);



        //                if (r.left <= x1) r.left = x1 + 1;



        //                r.right = (int)(r.left + size.Width);



        //                r.top = gy2 + 1 + nCharHeight;



        //                r.bottom = r.top + nCharHeight;







        //                if (r.right >= x2)



        //                {



        //                    r.right = x2;



        //                    r.left = (int)(r.right - size.Width);



        //                }







        //                if (r.left > displayed_pos_down)



        //                {



        //                    format.Alignment = StringAlignment.Center;



        //                    format.LineAlignment = StringAlignment.Center;







        //                    DrawClass.DrawText(g, buf, font, brush, r, format);



        //                    displayed_pos_down = r.right;



        //                }



        //            }



        //        }



        //    }



        //    else if (objArgs.wTimeSelectOption == 4)



        //    {	// 일별 데이터.



        //        int save_month = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddDays(objArgs.nDataCycle))



        //        {



        //            if (((t.Day - 1) % time_devide) != 0) continue;







        //            if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));











        //            buf = t.Day.ToString();



        //            size = g.MeasureString(buf, font);



        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);







        //                displayed_pos_up = r.right;



        //            }







        //            if (save_month != t.Month)



        //            {



        //                save_month = t.Month;



        //                buf = String.Format("{0}/{1}", t.Year, t.Month);



        //                size = g.MeasureString(buf, font);



        //                r.left = (int)(pos - size.Width / 2);



        //                if (r.left <= x1) r.left = x1 + 1;



        //                r.right = (int)(r.left + size.Width);



        //                r.top = gy2 + 1 + nCharHeight;



        //                r.bottom = r.top + nCharHeight;







        //                if (r.right >= x2)



        //                {



        //                    r.right = x2;



        //                    r.left = (int)(r.right - size.Width);



        //                }







        //                if (r.left > displayed_pos_down)



        //                {



        //                    format.Alignment = StringAlignment.Center;



        //                    format.LineAlignment = StringAlignment.Center;







        //                    DrawClass.DrawText(g, buf, font, brush, r, format);



        //                    displayed_pos_down = r.right;



        //                }



        //            }



        //        }



        //    }



        //    else if (objArgs.wTimeSelectOption == 5)



        //    {		// 월별 데이터.



        //        int save_year = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMonths(objArgs.nDataCycle))



        //        {



        //            if (((t.Month - 1) % time_devide) != 0) continue;







        //            if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //            buf = t.Month.ToString();



        //            size = g.MeasureString(buf, font);



        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_up = r.right;



        //            }







        //            if (save_year != t.Year)



        //            {



        //                save_year = t.Year;



        //                buf = t.Year.ToString();



        //                size = g.MeasureString(buf, font);



        //                r.left = (int)(pos - size.Width / 2);



        //                if (r.left <= x1) r.left = x1 + 1;



        //                r.right = (int)(r.left + size.Width);



        //                r.top = gy2 + 1 + nCharHeight;



        //                r.bottom = r.top + nCharHeight;







        //                if (r.right >= x2)



        //                {



        //                    r.right = x2;



        //                    r.left = (int)(r.right - size.Width);



        //                }







        //                if (r.left > displayed_pos_down)



        //                {



        //                    format.Alignment = StringAlignment.Center;



        //                    format.LineAlignment = StringAlignment.Center;







        //                    DrawClass.DrawText(g, buf, font, brush, r, format);



        //                    displayed_pos_down = r.right;



        //                }



        //            }



        //        }



        //    }



        //    else if (objArgs.wTimeSelectOption == 6)



        //    {		// 년별 데이터.



        //        //int save_year = 0;







        //        for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddYears(objArgs.nDataCycle))



        //        {



        //            if (((t.Year) % time_devide) != 0) continue;







        //            if (objArgs.wShowUnit - 1 == 0) continue;



        //            pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







        //            buf = t.Year.ToString();



        //            size = g.MeasureString(buf, font);



        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_up)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_up = r.right;



        //            }







        //            buf = t.Year.ToString();



        //            size = g.MeasureString(buf, font);



        //            r.left = (int)(pos - size.Width / 2);



        //            if (r.left <= x1) r.left = x1 + 1;



        //            r.right = (int)(r.left + size.Width);



        //            r.top = gy2 + 1 + nCharHeight;



        //            r.bottom = r.top + nCharHeight;







        //            if (r.right >= x2)



        //            {



        //                r.right = x2;



        //                r.left = (int)(r.right - size.Width);



        //            }







        //            if (r.left > displayed_pos_down)



        //            {



        //                format.Alignment = StringAlignment.Center;



        //                format.LineAlignment = StringAlignment.Center;







        //                DrawClass.DrawText(g, buf, font, brush, r, format);



        //                displayed_pos_down = r.right;



        //            }



        //        }



        //    }



        //    else



        //    {







        //    }



        //}







        void DisplayDescriptionX(Graphics g,



            int x1, int y1, int x2, int y2,



            int gx1, int gy1, int gx2, int gy2)



        {



            if (!IsDesX()) return;



            if (objArgs.wShowUnit <= 1) return;







            int time_devide = GetDisplayTimeDevide(gx1, gx2);







            // int displayedUp = -1;



            //int displayedDown = -1;







            DateTime t = dtStartTime;







            Brush brush = new SolidBrush(RunColorText);



            Font font = MakeFont();







            int displayedPair = -1; // 위/아래 공통 겹침 기준 (중요)







            for (ushort w = 0; w < objArgs.wShowUnit; w++)



            {



                if (!IsDivideMatch(t, time_devide))



                {



                    t = AddStep(t);



                    continue;



                }







                int pos = gx1 + (int)((long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                string upper = GetUpperText(t);



                string lower = GetLowerText(t);







                DrawCenteredTextPair(g, font, brush,



                    upper, lower,



                    pos,



                    gy2 + 1,



                    gy2 + 1 + nCharHeight,



                    x1, x2,



                    nCharHeight,



                    ref displayedPair);







                t = AddStep(t);



            }



            brush.Dispose();
        }







        bool DrawCenteredTextPair(Graphics g,



            Font font, Brush brush,



            string upper, string lower,



            int pos, int yUpper, int yLower,



            int x1, int x2, int charHeight,



            ref int lastRight)



        {



            // 1) 폭 측정 (둘 다 있는 경우 maxWidth로 clamp)



            float wUp = !string.IsNullOrEmpty(upper) ? g.MeasureString(upper, font).Width : 0f;



            float wDn = !string.IsNullOrEmpty(lower) ? g.MeasureString(lower, font).Width : 0f;







            float maxW = Math.Max(wUp, wDn);



            if (maxW <= 0) return false;







            int centerX = pos;







            // 2) maxW 기준으로 center clamp (여기서 위/아래 중심이 "같이" 결정됨)



            int leftMax = (int)(centerX - maxW / 2);



            int rightMax = (int)(centerX + maxW / 2);







            if (leftMax < x1) centerX += (x1 - leftMax);



            if (rightMax > x2) centerX -= (rightMax - x2);







            leftMax = (int)(centerX - maxW / 2);



            rightMax = (int)(centerX + maxW / 2);







            // 3) 겹침 방지 (maxW 기준)



            if (leftMax <= lastRight)



                return false;







            // 4) 실제 그리기 (Rectangle 기반 + Center 정렬)



            using (var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })



            {



                if (!string.IsNullOrEmpty(upper))



                {



                    var rcUp = new RectangleF(centerX - wUp / 2f, yUpper, wUp, charHeight);



                    g.DrawString(upper, font, brush, rcUp, fmt);



                }







                if (!string.IsNullOrEmpty(lower))



                {



                    var rcDn = new RectangleF(centerX - wDn / 2f, yLower, wDn, charHeight);



                    g.DrawString(lower, font, brush, rcDn, fmt);



                }



            }







            lastRight = rightMax;



            return true;



        }







        DateTime AddStep(DateTime t)



        {



            switch (objArgs.wTimeSelectOption)



            {



                case 0: return t.AddMilliseconds(objArgs.nDataCycle);



                case 1: return t.AddSeconds(objArgs.nDataCycle);



                case 2: return t.AddMinutes(objArgs.nDataCycle);



                case 3: return t.AddHours(objArgs.nDataCycle);



                case 4: return t.AddDays(objArgs.nDataCycle);



                case 5: return t.AddMonths(objArgs.nDataCycle);



                case 6: return t.AddYears(objArgs.nDataCycle);



                default: return t;



            }



        }







        bool IsDivideMatch(DateTime t, int divide)



        {



            switch (objArgs.wTimeSelectOption)



            {



                case 0: return t.Second % divide == 0 && t.Millisecond == 0;



                case 1: return (t.Second + t.Minute * 60) % divide == 0;



                case 2: return (t.Minute + t.Hour * 60) % divide == 0;



                case 3: return (t.Hour + (t.Day - 1) * 24) % divide == 0;



                case 4: return (t.Day - 1) % divide == 0;



                case 5: return (t.Month - 1) % divide == 0;



                case 6: return t.Year % divide == 0;



                default: return true;



            }



        }







        string GetUpperText(DateTime t)



        {



            switch (objArgs.wTimeSelectOption)



            {



                case 0:



                case 1:



                    return t.ToString("HH:mm:ss");







                case 2:



                case 3:



                    return t.ToString("HH:mm");







                case 4:



                    return t.Day.ToString();







                case 5:



                    return t.Month.ToString();







                case 6:



                    return t.Year.ToString();



            }



            return null;



        }







        string GetLowerText(DateTime t)



        {



            switch (objArgs.wTimeSelectOption)



            {



                case 0:



                case 1:



                case 2:



                case 3:



                    return t.ToString("M/d", CultureInfo.InvariantCulture);







                case 4:



                    return t.ToString("yyyy/M", CultureInfo.InvariantCulture);







                case 5:



                case 6:



                    return t.Year.ToString();



            }



            return null;



        }







        void DisplayDescriptionY(Graphics g, int x1, int y1, int x2, int y2, int gx1, int gy1, int gx2, int gy2)



        {



            if (!IsDesY()) return;







            MD_TREND_MEMBER member;



            MD_TREND_MEMBER memberDisplay;	// 이것은 태그의 위치에서 오른쪽버튼을 누르면



            // 첫번째의 레벨값을 해당 태그로 바꾸어 주는 기능때문에 필요하다.



            int l;



            double view_full = 0, view_base = 0;







            double val;



            int pos;



            int i;



            string buf;



            RECT r = new RECT();



            int real_y;



            int real_size;



            RECT rZone = new RECT();



            TagAiClass ai = null;



            TagDiClass di = null;



            Font font = MakeFont();







            StringFormat format = new StringFormat();







            format.Alignment = StringAlignment.Far;



            format.LineAlignment = StringAlignment.Center;



            format.FormatFlags = StringFormatFlags.FitBlackBox;







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];



                memberDisplay = (MD_TREND_MEMBER)blockMember[l];







                if (member.nAxisPosition == 0) continue;







                if (l == 0)



                {



                    if (l != dwFirstLevelDisplay && dwFirstLevelDisplay < blockMember.Count)



                    {



                        memberDisplay = (MD_TREND_MEMBER)blockMember[dwFirstLevelDisplay];



                    }



                }







                if (member.nType == 0)



                {



                    ai = GetRealTagAI(memberDisplay.tag, ref memberDisplay.nPos);



                    //GetViewFullBase(ai, memberDisplay, out view_full, out view_base);



                    view_full = memberDisplay.max_value;



                    view_base = memberDisplay.min_value;



                }



                else if (member.nType == EnumTagType.DI)



                {	// DIGITAL INPUT tag



                    di = GetRealTagDI(memberDisplay.tag, ref memberDisplay.nPos);



                    view_full = memberDisplay.max_value;



                    view_base = memberDisplay.min_value;



                }



                else



                {



                    //GetViewFullBase(ai, memberDisplay, out view_full, out view_base);







                    view_full = memberDisplay.view_full;



                    view_base = memberDisplay.view_base;



                }







                if (member.nAxisPosition == 1)



                {



                    rZone.right = gx1 - (member.nAxisCalcPos * (nCharWidth * objArgs.pub.nLevelDisplaySize)) - 2;



                    rZone.left = rZone.right - (nCharWidth * objArgs.pub.nLevelDisplaySize) + 2;



                }



                else



                {



                    rZone.left = gx2 + (member.nAxisCalcPos * (nCharWidth * objArgs.pub.nLevelDisplaySize)) + 2;



                    rZone.right = rZone.left + (nCharWidth * objArgs.pub.nLevelDisplaySize) - 2;



                }







                rZone.top = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;



                rZone.bottom = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;







                //DrawClass.PushBox2(g, rZone.left, rZone.top, rZone.right, rZone.bottom, RunColorFill.basic_color);







                Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, rZone.left, rZone.top, rZone.right, rZone.bottom);



                if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE) DrawClass.PushBox2(g, rZone.left, rZone.top, rZone.right, rZone.bottom, RunColorFill.basic_color); //CE업데이트시 삭제.



                else if (IsBackBorder()) DrawClass.PushBox2(g, rZone.left, rZone.top, rZone.right, rZone.bottom, brush); //20250206 PSU 배경판 옵션 사용 추가



                else { DrawClass.gcls(g, rZone.left - 2, rZone.top, rZone.right + 2, rZone.bottom, brush); } //배경판 미사용 시 pushbox 제거. 좌우 2씩 추가해야 요소 뒤 색상이 안보임.







                brush.Dispose();
                brush = new SolidBrush(memberDisplay.color);







                for (i = 0; i < objArgs.wLevelDevide + 1; i++)



                {



                    val = (view_full - view_base) * i / objArgs.wLevelDevide + view_base;







                    real_size = (member.nLevelTo - member.nLevelFrom) * (gy2 - gy1) / 100;



                    real_y = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;







                    if (member.nReverseY == 1)



                    {



                        real_y = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;



                        pos = real_y + (real_size) * i / (objArgs.wLevelDevide);



                    }



                    else



                    {



                        real_y = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;



                        pos = real_y - (real_size) * i / (objArgs.wLevelDevide);



                    }







                    if (member.nType == 0)



                    {



                        if (objArgs.logarithmicScale.bUse)



                        {



                            double log_full, log_base;







                            if (view_full == 0)



                                log_full = 0;



                            else



                                log_full = Math.Log(view_full, objArgs.logarithmicScale.fBase);







                            if (view_base == 0)



                                log_base = 0;



                            else



                                log_base = Math.Log(view_base, objArgs.logarithmicScale.fBase);







                            double log_val = (log_full - log_base) * i / objArgs.wLevelDevide + log_base;







                            val = Math.Pow(objArgs.logarithmicScale.fBase, log_val);







                            buf = TagUtil.AiValueToStringOnlyPoint(ai, TagUtil.GetDisplayValue(ai, val));



                        }



                        else



                            buf = TagUtil.AiValueToStringOnlyPoint(ai, TagUtil.GetDisplayValue(ai, val));



                    }



                    else if (member.nType == EnumTagType.DI)



                    {



                        if (i == 0) buf = TagUtil.GetDesOFF(di);



                        else if (i == objArgs.wLevelDevide) buf = TagUtil.GetDesON(di);



                        else buf = "";	// empty



                    }



                    else



                    {



                        buf = val.ToString();



                    }







                    r.left = rZone.left + 1;



                    r.right = rZone.right - 1;







                    if (member.nReverseY == 1)



                    {



                        if (i == 0)	// 처음



                            r.top = rZone.top + 1;



                        else if (i == objArgs.wLevelDevide) // 끝



                            r.top = rZone.bottom - nCharHeight;



                        else



                            r.top = pos - nCharHeight / 2;



                    }



                    else



                    {



                        if (i == 0)	// 처음



                            r.top = rZone.bottom - nCharHeight;



                        else if (i == objArgs.wLevelDevide) // 끝



                            r.top = rZone.top + 1;



                        else



                            r.top = pos - nCharHeight / 2;



                    }







                    r.bottom = r.top + nCharHeight;







                    Rectangle rt = new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);







                    SafeException.SafeDrawString(g, buf, font, brush, rt, format);



                }



            }



            format.Dispose();
            brush.Dispose();
        }







        int GetDisplayTimeDevide(int gx1, int gx2)



        {



            int devide = objArgs.wTimeDevide;







            if (bAutoGuideLine)



            {



                devide = (gx2 - gx1) / (nCharWidth * 7);



                if (devide == 0) devide = objArgs.wTimeDevide;



                else devide = objArgs.wShowUnit / devide;







                if (objArgs.wTimeSelectOption == 0)



                {	// milli data



                    if (devide > 500) devide = 1000;



                }



                else if (objArgs.wTimeSelectOption == 1)



                {	// second data



                    if (devide > 30) devide = 60;



                }



                else if (objArgs.wTimeSelectOption == 2)



                {	// min data



                    if (devide > 30) devide = 60;



                }



                else if (objArgs.wTimeSelectOption == 3)



                {	// hour data



                    if (devide > 12) devide = 24;



                }



                else if (objArgs.wTimeSelectOption == 4)



                {	// day data



                    if (devide > 15) devide = 31;



                }



                else if (objArgs.wTimeSelectOption == 5)



                {	// hour data



                    if (devide > 6) devide = 12;



                }



                else if (objArgs.wTimeSelectOption == 6)



                {	// hour data



                    if (devide > 6) devide = 12;



                }



            }







            if (devide <= 0) devide = 1;



            return devide;



        }







        const int MIN_GUIDE_PIXEL_SPACING = 20; // 30px 이하로는 안 그림











        void CheckOneMember(MD_TREND_MEMBER member)



        {



            TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);



            if (member.nLevelFrom < 0) member.nLevelFrom = 0;



            if (member.nLevelTo > 100) member.nLevelTo = 100;



            if (member.nLevelFrom >= member.nLevelTo)



            {



                member.nLevelFrom = 0;



                member.nLevelTo = 100;



            }



            member.visible = 1;



            if (member.nTagDisplaySize < 1 || member.nTagDisplaySize > 40)



            {



                member.nTagDisplaySize = 10;



            }







            if (member.nType == EnumTagType.AI)



            {



                TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);



                member.max_value = ai.view_full;



                member.min_value = ai.view_base;



                member.view_base = ai.view_base;



                member.view_full = ai.view_full;



            }



            else



            {



                member.max_value = 100;



                member.min_value = 0;



                member.view_base = 0;



                member.view_full = 100;



            }







            MallocOneBuf(member);



        }







        void SetBlock(ArrayList block)



        {



            if (block == null)



                blockMember = new ArrayList();



            else



                blockMember = block;







            int l;



            MD_TREND_MEMBER member;







            if (block != null)



            {



                for (l = 0; l < block.Count; l++)



                {



                    member = (MD_TREND_MEMBER)block[l];



                    CheckOneMember(member);



                }



            }







            // 블록이 변경되었으므로 캐시 무효화



            InvalidateGraphCache();







            CalcAxisPosition();



        }







        static int nSignGraphNo = 0;







        void MallocOneBuf(MD_TREND_MEMBER member)



        {



            member.point = new ONE_POINT_STRUCT[objArgs.wShowUnit];







            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)



            {



                int gab;







                nSignGraphNo++;



                nSignGraphNo %= 20;







                if (member.nType == 0)



                {



                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);



                    gab = (int)((ai.fFull - ai.fBase) / 2);



                    if (gab <= 0) gab = 50;



                }



                else



                    gab = 50;







                for (int j = 0; j < objArgs.wShowUnit; j++)



                {



                    member.point[j].val = (float)(gab + Math.Sin(3.14 * ((720.0 * (j + nSignGraphNo * 5)) / objArgs.wShowUnit) / 360.0) * gab);



                    if (member.nType == EnumTagType.DI)



                    {



                        if (member.point[j].val > gab / 2) member.point[j].val = 1;



                        else member.point[j].val = 0;



                    }



                    member.point[j].read_flag = true;



                }



            }



        }







        void MallocAllBuf()



        {



            MD_TREND_MEMBER member;



            for (int i = 0; i < blockMember.Count; i++)



            {



                member = (MD_TREND_MEMBER)blockMember[i];



                MallocOneBuf(member);



            }



        }







        void DisplayTagColor(Graphics g, int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2)



        {



            if (!IsTagColor() && !IsTagMinMax() && !IsTagCurr() && !IsTagOldCurr()) return;







            int l;



            MD_TREND_MEMBER member;



            RECT r = new RECT();



            int x;



            int y;



            int x1, y1, x2, y2;



            string buf;



            double val;



            double min, max;







            x = ox1 + 5;



            x1 = x;



            x2 = x + nCharWidth * 8 - 1;







            if (IsDesX()) y = gy2 + nCharHeight * 2;



            else y = gy2 + 3;







            r.left = x1 + 1;



            r.right = x2;







            Font font = MakeFont();



            Brush brush = new SolidBrush(objArgs.pub.colorPanelText); //20250204 PSU 수정



            StringFormat format = new StringFormat();



            format.Alignment = StringAlignment.Center;



            format.LineAlignment = StringAlignment.Center;







            // 현재SmoothingMode 상태 저장, 고품질모드에서 antialias 적용으로 popbox2 사이에 빈공간이 생겨서 추가.  20250206 PSU 



            SmoothingMode prevMode = g.SmoothingMode;



            g.SmoothingMode = SmoothingMode.None;







            if (IsTagColor())



            {



                y1 = y;



                y2 = y + nCharHeight + 1;



                DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack); //20250204 PSU 수정



                r.top = y1 + 1;



                r.bottom = y2;







                if (Tools.IsLangKorean())



                    buf = "태그명";



                else if (Tools.IsLangJapanese())



                    buf = "タグ";



                else if (Tools.IsLangChinese())



                    buf = "标记";



                else



                    buf = "TAG";







                DrawClass.DrawText(g, buf, font, brush, r, format);



                y += nCharHeight + 2;



            }



            if (IsTagMinMax())



            {



                y1 = y;



                y2 = y + nCharHeight + 1;



                DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                r.top = y1 + 1;



                r.bottom = y2;



                if (Tools.IsLangKorean())



                    buf = "최대값";



                else if (Tools.IsLangJapanese())



                    buf = "最大値";



                else if (Tools.IsLangChinese())



                    buf = "最大值";



                else



                    buf = "MAX";







                DrawClass.DrawText(g, buf, font, brush, r, format);



                y += nCharHeight + 2;







                y1 = y;



                y2 = y + nCharHeight + 1;



                DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                r.top = y1 + 1;



                r.bottom = y2;







                if (Tools.IsLangKorean())



                    buf = "최소값";



                else if (Tools.IsLangJapanese())



                    buf = "最小値";



                else if (Tools.IsLangChinese())



                    buf = "最小值";



                else



                    buf = "MIN";







                DrawClass.DrawText(g, buf, font, brush, r, format);



                y += nCharHeight + 2;



            }



            if (IsTagCurr())



            {



                y1 = y;



                y2 = y + nCharHeight + 1;



                DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                r.top = y1 + 1;



                r.bottom = y2;







                if (Tools.IsLangKorean())



                    buf = "현재값";



                else if (Tools.IsLangJapanese())



                    buf = "現在値";



                else if (Tools.IsLangChinese())



                    buf = "现在值";



                else



                    buf = "Curr";







                DrawClass.DrawText(g, buf, font, brush, r, format);



                y += nCharHeight + 2;



            }



            if (IsTagOldCurr())



            {



                y1 = y;



                y2 = y + nCharHeight + 1;



                DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                r.top = y1 + 1;



                r.bottom = y2;







                if (Tools.IsLangKorean())



                {



                    if (nCursorX1 == nCursorX2) buf = "자료값";



                    else buf = "평균값";



                }



                else if (Tools.IsLangJapanese())



                {



                    if (nCursorX1 == nCursorX2) buf = "データ";



                    else buf = "平均値";



                }



                else if (Tools.IsLangChinese())



                {



                    if (nCursorX1 == nCursorX2) buf = "平均值";



                    else buf = "资料值";



                }



                else



                {



                    if (nCursorX1 == nCursorX2) buf = "Data";



                    else buf = "Ave";



                }







                DrawClass.DrawText(g, buf, font, brush, r, format);



                y += nCharHeight + 2;



            }







            x += nCharWidth * 8;







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                if (x + nCharWidth * member.nTagDisplaySize >= ox2 - 1) break;







                if (IsDesX()) y = gy2 + nCharHeight * 2;



                else y = gy2 + 3;







                x1 = x;



                x2 = x + nCharWidth * member.nTagDisplaySize - 1;







                if (IsTagColor())



                {



                    y1 = y;



                    y2 = y + nCharHeight + 1;



                    if (member.visible == 0)



                    {



                        DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                    }



                    else



                    {



                        DrawClass.PopBox2(g, x1, y1, x2, y2, RunColorFill.basic_color);



                    }



                    DisplayPoint(g, x1 + 2 + nCharWidth / 2, y1 + 1 + nCharHeight / 2, member.color, member.nPointType, nCharWidth / 2);



                    if (member.visible == 0)



                    {



                        brush.Dispose();
                        brush = new SolidBrush(objArgs.pub.colorPanelText);



                    }



                    else



                    {



                        brush.Dispose();
                        brush = new SolidBrush(member.color);



                    }







                    r.left = x1 + nCharWidth * 2;



                    r.right = x2;



                    r.top = y1 + 1;



                    r.bottom = y2;







                    if (member.sDescription.Length > 0)



                    {



                        buf = member.sDescription;



                    }



                    else



                    {







                        if (IsByDescription())



                        {



                            if (member.nType == EnumTagType.AI)



                            {



                                TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                                if (ai.description.Length == 0)



                                    buf = ai.tag;



                                else



                                    buf = ai.description;



                            }



                            else if (member.nType == EnumTagType.DI)



                            {



                                TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);



                                if (di.description.Length == 0)



                                    buf = di.tag;



                                else



                                    buf = di.description;



                            }



                            else



                            {



                                buf = member.tag;



                            }



                        }



                        else



                        {



                            if (member.nType == EnumTagType.AI)



                            {



                                TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                                buf = ai.tag;



                            }



                            else if (member.nType == EnumTagType.DI)



                            {



                                TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);



                                buf = di.tag;



                            }



                            else



                            {



                                buf = member.tag;



                            }



                        }



                    }







                    format.Alignment = StringAlignment.Near;



                    format.LineAlignment = StringAlignment.Center;



                    //format.FormatFlags |= StringFormatFlags.NoWrap;



                    //format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;







                    DrawClass.DrawText(g, buf, font, brush, r, format);



                    y += nCharHeight + 2;



                }



                if (IsTagMinMax())



                {



                    y1 = y;



                    y2 = y + nCharHeight + 1;



                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                    brush.Dispose();
                    brush = new SolidBrush(objArgs.pub.colorPanelText);







                    r.left = x1 + 1;



                    r.right = x2;



                    r.top = y1 + 1;



                    r.bottom = y2;



                    if (member.nType == 0)



                    {



                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                        if (nCursorX1 == nCursorX2)



                        {



                            GetViewFullBase(ai, member, out max, out min);



                            buf = TagUtil.AiValueToStringOnlyPoint(ai, max);



                        }



                        else



                        {



                            GetCursorValue(member, out val, out min, out max);



                            buf = TagUtil.AiValueToStringOnlyPoint(ai, max);



                        }



                    }



                    else



                    {



                        buf = "";



                    }







                    format.Alignment = StringAlignment.Far;



                    format.LineAlignment = StringAlignment.Center;







                    DrawClass.DrawText(g, buf, font, brush, r, format);



                    y += nCharHeight + 2;







                    y1 = y;



                    y2 = y + nCharHeight + 1;



                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                    brush.Dispose();
                    brush = new SolidBrush(objArgs.pub.colorPanelText);







                    r.left = x1 + 1;



                    r.right = x2;



                    r.top = y1 + 1;



                    r.bottom = y2;



                    if (member.nType == 0)



                    {



                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                        if (nCursorX1 == nCursorX2)



                        {



                            GetViewFullBase(ai, member, out max, out min);



                            buf = TagUtil.AiValueToStringOnlyPoint(ai, min);



                        }



                        else



                        {



                            GetCursorValue(member, out val, out min, out max);



                            buf = TagUtil.AiValueToStringOnlyPoint(ai, min);



                        }



                    }



                    else



                    {



                        buf = "";



                    }







                    format.Alignment = StringAlignment.Far;



                    format.LineAlignment = StringAlignment.Center;







                    DrawClass.DrawText(g, buf, font, brush, r, format);



                    y += nCharHeight + 2;



                }



                if (IsTagCurr())



                {



                    y1 = y;



                    y2 = y + nCharHeight + 1;



                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                    brush.Dispose();
                    brush = new SolidBrush(objArgs.pub.colorPanelText);







                    r.left = x1 + 1;



                    r.right = x2;



                    r.top = y1 + 1;



                    r.bottom = y2;



                    if (member.nType == EnumTagType.AI)



                    {



                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                        buf = TagUtil.AiValueToStringOnlyPoint(ai, ai.curr);







                        format.Alignment = StringAlignment.Far;



                        format.LineAlignment = StringAlignment.Center;







                        DrawClass.DrawText(g, buf, font, brush, r, format);



                    }



                    else if (member.nType == EnumTagType.DI)



                    {



                        TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);



                        if (di.curr == 1) buf = TagUtil.GetDesON(di);



                        else buf = TagUtil.GetDesOFF(di);



                        DrawClass.DrawText(g, buf, font, brush, r, format);



                    }



                    else



                    {



                        buf = "???";



                        DrawClass.DrawText(g, buf, font, brush, r, format);



                    }







                    y += nCharHeight + 2;



                }



                if (IsTagOldCurr())



                {



                    y1 = y;



                    y2 = y + nCharHeight + 1;



                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);



                    brush.Dispose();
                    brush = new SolidBrush(objArgs.pub.colorPanelText);







                    r.left = x1 + 1;



                    r.right = x2;



                    r.top = y1 + 1;



                    r.bottom = y2;







                    if (GetCursorValue(member, out val, out min, out max))



                    {



                        if (member.nType == EnumTagType.AI)



                        {



                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                            buf = TagUtil.AiValueToStringOnlyPoint(ai, val);



                        }



                        else if (member.nType == EnumTagType.DI)



                        {



                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);



                            if (val > 0.5) buf = TagUtil.GetDesON(di);



                            else buf = TagUtil.GetDesOFF(di);



                        }



                        else



                        {



                            buf = val.ToString();



                        }



                    }



                    else



                    {



                        buf = "***";



                    }







                    if (member.nType == 0)



                    {



                        format.Alignment = StringAlignment.Far;



                        format.LineAlignment = StringAlignment.Center;







                        DrawClass.DrawText(g, buf, font, brush, r, format);



                    }



                    else



                    {



                        format.Alignment = StringAlignment.Center;



                        format.LineAlignment = StringAlignment.Center;







                        DrawClass.DrawText(g, buf, font, brush, r, format);



                    }



                    y += nCharHeight + 2;



                }







                x += nCharWidth * member.nTagDisplaySize;



            }







            // 이전 상태로 복원  20250206 PSU



            g.SmoothingMode = prevMode;



            format.Dispose();
            brush.Dispose();
        }







        public static void DisplayPoint(Graphics g, int x, int y, Color color, int cPointType, int r)



        {



            if (cPointType == 0) return;







            Point[] poly = new Point[4];







            Pen pen = new Pen(color, 1);



            Brush brush = new SolidBrush(color);







            int width = r * 2;



            int height = r * 2;







            switch (cPointType)



            {



                case 1:	// rectangle



                    g.FillRectangle(brush, x - r, y - r, width, height);



                    g.DrawRectangle(pen, x - r, y - r, width, height);



                    break;



                case 2:	// cirdle.



                    g.FillEllipse(brush, x - r, y - r, width, height);



                    g.DrawEllipse(pen, x - r, y - r, width, height);



                    break;



                case 3:	// triangle



                    poly = new Point[3];



                    poly[0].X = x - r;



                    poly[0].Y = y + r;



                    poly[1].X = x + r;



                    poly[1].Y = y + r;



                    poly[2].X = x;



                    poly[2].Y = y - r;



                    g.FillPolygon(brush, poly);



                    g.DrawPolygon(pen, poly);



                    break;



                case 4:	// diamond



                    poly = new Point[4];



                    poly[0].X = x;



                    poly[0].Y = y - r;



                    poly[1].X = x + r;



                    poly[1].Y = y;



                    poly[2].X = x;



                    poly[2].Y = y + r;



                    poly[3].X = x - r;



                    poly[3].Y = y;



                    g.FillPolygon(brush, poly);



                    g.DrawPolygon(pen, poly);



                    break;



                case 5:	// X



                    g.DrawLine(pen, x - r, y - r, x + r + 1, y + r + 1);



                    g.DrawLine(pen, x - r, y + r, x + r + 1, y - r - 1);



                    //MoveToEx(hdc, x-r, y-r, NULL);



                    //LineTo(hdc, x+r+1, y+r+1);



                    //MoveToEx(hdc, x-r, y+r, NULL);



                    //LineTo(hdc, x+r+1, y-r-1);



                    break;



                case 6:	// +



                    g.DrawLine(pen, x - r, y, x + r + 1, y);



                    g.DrawLine(pen, x, y - r, x, y + r + 1);







                    //MoveToEx(hdc, x-r, y, NULL);



                    //LineTo(hdc,   x+r+1, y);



                    //MoveToEx(hdc, x, y-r, NULL);



                    //LineTo(hdc,   x, y+r+1);



                    break;



                case 7:	// *



                    g.DrawLine(pen, x - r, y - r, x + r + 1, y + r + 1);



                    g.DrawLine(pen, x - r, y + r, x + r + 1, y - r - 1);



                    g.DrawLine(pen, x - r, y, x + r + 1, y);



                    g.DrawLine(pen, x, y - r, x, y + r + 1);







                    //MoveToEx(hdc, x-r, y-r, NULL);



                    //LineTo(hdc, x+r+1, y+r+1);



                    //MoveToEx(hdc, x-r, y+r, NULL);



                    //LineTo(hdc, x+r+1, y-r-1);







                    //MoveToEx(hdc, x-r, y, NULL);



                    //LineTo(hdc,   x+r+1, y);



                    //MoveToEx(hdc, x, y-r, NULL);



                    //LineTo(hdc,   x, y+r+1);



                    break;



            }



            pen.Dispose();
            brush.Dispose();
        }







        bool GetCursorValue(MD_TREND_MEMBER member, out double curr, out double min, out double max)



        {



            curr = 0;



            min = 0;



            max = 100;







            //if(member.point == null)			return false;







            int x1, x2;







            x1 = nCursorX1;



            x2 = nCursorX2;







            if (x1 > x2) Tools.Temp(ref x1, ref x2);



            if (x1 < 0 || x1 >= objArgs.wShowUnit) return false;



            if (x2 < 0 || x2 >= objArgs.wShowUnit) return false;







            return GetZoneValue(member, out curr, out min, out max, x1, x2);



        }







        bool GetZoneValue(MD_TREND_MEMBER member, out double ave, out double min, out double max, int from, int to)



        {



            ave = 0;



            min = 0;



            max = 100;







            if (member.nType == EnumTagType.AI)



            {



                TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);



                GetViewFullBase(ai, member, out max, out min);



            }







            bool retn = false;







            if (from == to)



            {



                int pos;







                pos = from;







                if (pos < 0 || pos >= objArgs.wShowUnit) return false;







                ave = member.point[pos].val;



                retn = member.point[pos].read_flag;



            }



            else



            {



                int count = 0;



                int i;







                if (from > to) Tools.Temp(ref from, ref to);



                if (from < 0 || from >= objArgs.wShowUnit) return false;



                if (to < 0 || to >= objArgs.wShowUnit) return false;







                float val;







                for (i = from; i <= to; i++)



                {



                    if (!member.point[i].read_flag) continue;



                    val = (float)member.point[i].val;



                    if (count == 0)



                    {



                        ave = val;



                        min = val;



                        max = val;



                    }



                    else



                    {







                        ave += val;



                        if (val < min) min = val;



                        if (val > max) max = val;



                    }



                    count++;



                }







                if (count > 0)



                {



                    ave = ave / count;



                    retn = true;



                }



            }







            return retn;



        }







        int CalcPatternLineNumberPosition(Graphics g, PatternLineItem pli, int pos, Font font, string text, int number)



        {



            int x;







            if (pli.number_position == 1)



            {



                if (number == 0)



                    x = (int)(pos + (pli.thick));



                else



                    x = (int)(pos + (pli.thick + 1) / 2);



            }



            else



            {



                SizeF size = g.MeasureString(text, font);







                if (number == 0)



                    x = (int)(pos - size.Width - (pli.thick));



                else



                    x = (int)(pos - size.Width - (pli.thick + 1) / 2);



            }







            return x;



        }







        void DisplayPatternLineOne(PatternLineItem pli, Graphics g, int gx1, int gy1, int gx2, int gy2)



        {



            if (!pli.visible) return;







            DateTime tPatternEnd = pli.tStart.AddSeconds(pli.pattern_cycle * (pli.line_count - 1));   // 마지막 시간







            if (tPatternEnd < dtStartTime) return;  // 이전 시간대이다.







            TimeSpan ts = dtStartTime - pli.tStart;







            long ms_StartTime = ts.Ticks / TimeSpan.TicksPerMillisecond;







            long cycle = (int)(pli.pattern_cycle * 1000);



            long remain = 0;



            int shift = 0;







            remain = Math.Abs(ms_StartTime % cycle);







            if (objArgs.wTimeSelectOption == 0)



            {



                //cycle = (int)(pli.pattern_cycle * 1000);//pli.pattern_cycle;







                //remain = Math.Abs(ms_StartTime % cycle);



                shift = (int)(remain);



            }



            else if (objArgs.wTimeSelectOption == 1)



            {



                //cycle = (int)(pli.pattern_cycle * 1000);//pli.pattern_cycle * 1000;







                //remain = Math.Abs(ms_StartTime % cycle);



                shift = (int)(remain / 1000);



            }



            else if (objArgs.wTimeSelectOption == 2)



            {



                //cycle = (int)(pli.pattern_cycle * 1000);//pli.pattern_cycle * (1000 * 60);







                //remain = Math.Abs(ms_StartTime % cycle);



                shift = (int)(remain / (1000 * 60));



            }







            else if (objArgs.wTimeSelectOption == 3)



            {



                //cycle = (int)(pli.pattern_cycle * 1000);//pli.pattern_cycle * (1000 * 60 * 60);







                //remain = Math.Abs(ms_StartTime % cycle);



                shift = (int)(remain / (1000 * 60 * 60));



            }







            else if (objArgs.wTimeSelectOption == 4)



            {



                //cycle = (int)(pli.pattern_cycle * 1000);//pli.pattern_cycle * (1000 * 60 * 60 * 24);







                //remain = Math.Abs(ms_StartTime % cycle);



                shift = (int)(remain / (1000 * 60 * 60 * 24));



            }



            else



            {







            }







            if (ms_StartTime < 0)



                shift *= -1;







            DateTime t = dtStartTime;







            Pen pen = new Pen(pli.color, pli.thick);



            ushort w;



            int pos;



            int p_i = shift;







            if (objArgs.wShowUnit <= 1) return; // 숫자가 적어서 계산할 수 없다.







            long ms_PatternStart = pli.tStart.Ticks / TimeSpan.TicksPerMillisecond; // tStart를 ms로 환산한 값



            long ms_t = ms_StartTime;   // 밀리초로 환산



            string buf;



            int x;



            int number;



            //SizeF size;



            Font font = MakeFont();







            if (objArgs.wTimeSelectOption == 0)



            {	// 밀리 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddMilliseconds(objArgs.nDataCycle), ms_t += objArgs.nDataCycle)



                {



                    if (ms_t % cycle != 0) continue; //  







                    if (t > tPatternEnd) break;     // 끝났다.



                    if (t < pli.tStart) continue;   // 아직 시작점이 되지 않았다.







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    if (pos < gx1 || pos > gx2) continue;  // over







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen8 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen8, pos, gy1 + 1, pos, gy2);



                        //x = pos - (pli.thick + nCharWidth);



                    }



                    else



                    {



                        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                        //x = pos - ((pli.thick + 1) / 2 + nCharWidth);



                    }







                    buf = String.Format("{0}", number + pli.start_number);







                    x = CalcPatternLineNumberPosition(g, pli, pos, font, buf, number);



                    using (Brush textBrush8 = new SolidBrush(pli.color))
                        g.DrawString(buf, MakeFont(), textBrush8, x, gy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 1)



            {	// 초 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddSeconds(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000)



                {



                    if (ms_t % cycle != 0) continue; //  







                    if (t > tPatternEnd) break;     // 끝났다.



                    if (t < pli.tStart) continue;   // 아직 시작점이 되지 않았다.







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    if (pos < gx1 || pos > gx2) continue;  // over







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen9 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen9, pos, gy1 + 1, pos, gy2);



                        //x = pos - (pli.thick+nCharWidth);



                    }



                    else



                    {



                        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                        //x = pos - ((pli.thick + 1) / 2 + nCharWidth);



                    }







                    buf = String.Format("{0}", number + pli.start_number);







                    x = CalcPatternLineNumberPosition(g, pli, pos, font, buf, number);



                    using (Brush textBrush9 = new SolidBrush(pli.color))
                        g.DrawString(buf, MakeFont(), textBrush9, x, gy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 2)



            {	// 분 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddMinutes(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000 * 60)



                {



                    if (ms_t % cycle != 0) continue; //  







                    if (t > tPatternEnd) break;     // 끝났다.



                    if (t < pli.tStart) continue;   // 아직 시작점이 되지 않았다.







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    if (pos < gx1 || pos > gx2) continue;  // over







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen10 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen10, pos, gy1 + 1, pos, gy2);



                        //x = pos - (pli.thick + nCharWidth);



                    }



                    else



                    {



                        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                        //x = pos - ((pli.thick + 1) / 2 + nCharWidth);



                    }







                    buf = String.Format("{0}", number + pli.start_number);







                    x = CalcPatternLineNumberPosition(g, pli, pos, font, buf, number);



                    using (Brush textBrush10 = new SolidBrush(pli.color))
                        g.DrawString(buf, MakeFont(), textBrush10, x, gy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 3)



            {	// 시간 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddHours(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000 * 60 * 60)



                {



                    if (ms_t % cycle != 0) continue; //  







                    if (t > tPatternEnd) break;     // 끝났다.



                    if (t < pli.tStart) continue;   // 아직 시작점이 되지 않았다.







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    if (pos < gx1 || pos > gx2) continue;  // over







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen11 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen11, pos, gy1 + 1, pos, gy2);



                        //x = pos - (pli.thick + nCharWidth);



                    }



                    else



                    {



                        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                        //x = pos - ((pli.thick + 1) / 2 + nCharWidth);



                    }







                    buf = String.Format("{0}", number + pli.start_number);







                    x = CalcPatternLineNumberPosition(g, pli, pos, font, buf, number);



                    using (Brush textBrush11 = new SolidBrush(pli.color))
                        g.DrawString(buf, MakeFont(), textBrush11, x, gy1);



                }



            }



            else if (objArgs.wTimeSelectOption == 4)



            {	// 일일 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddDays(objArgs.nDataCycle), ms_t += objArgs.nDataCycle * 1000 * 60 * 60 * 24)



                {



                    if (ms_t % cycle != 0) continue; //  







                    if (t > tPatternEnd) break;     // 끝났다.



                    if (t < pli.tStart) continue;   // 아직 시작점이 되지 않았다.







                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    if (pos < gx1 || pos > gx2) continue;  // over







                    ts = t - pli.tStart;



                    number = (int)(ts.TotalMilliseconds / cycle);







                    if (number == 0)



                    {



                        using (Pen linePen12 = new Pen(pli.color, pli.thick * 2f))
                            g.DrawLine(linePen12, pos, gy1 + 1, pos, gy2);



                        //x = pos - (pli.thick + nCharWidth);



                    }



                    else



                    {



                        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                        //x = pos - ((pli.thick + 1) / 2 + nCharWidth);



                    }







                    buf = String.Format("{0}", number + pli.start_number);







                    x = CalcPatternLineNumberPosition(g, pli, pos, font, buf, number);



                    using (Brush textBrush12 = new SolidBrush(pli.color))
                        g.DrawString(buf, MakeFont(), textBrush12, x, gy1);



                }



            }



            //else if (objArgs.wTimeSelectOption == 5)



            //{	// 월간 데이터.



            //    for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddMonths(objArgs.nDataCycle))



            //    {



            //        if (((p_i * objArgs.nDataCycle) % pli.pattern_cycle) != 0) continue;







            //        if (t < pli.tStart) continue;  // 아직 시작점이 되지 않았다.







            //        pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







            //        if (pos < gx1 || pos > gx2) continue;  // over







            //        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



            //    }



            //}



            //else if (objArgs.wTimeSelectOption == 6)



            //{	// 연간 데이터.



            //    for (w = 0; w < objArgs.wShowUnit; w++, p_i++, t = t.AddYears(objArgs.nDataCycle))



            //    {



            //        if (((p_i * objArgs.nDataCycle) % pli.pattern_cycle) != 0) continue;







            //        if (t < pli.tStart) continue;  // 아직 시작점이 되지 않았다.







            //        pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







            //        if (pos < gx1 || pos > gx2) continue;  // over







            //        g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



            //    }



            //}



            else



            {	// 분 데이터.







            }



            pen.Dispose();
        }







        void DisplayPatternLine(Graphics g, int gx1, int gy1, int gx2, int gy2)



        {



            if (!bVisiblePatternLine) return;







            if (arrayPatternLine == null) return;







            if (arrayPatternLine.Count == 0) return;







            PatternLineItem pli;







            for (int i = 0; i < arrayPatternLine.Count; i++)



            {



                pli = arrayPatternLine[i];



                DisplayPatternLineOne(pli, g, gx1, gy1, gx2, gy2);



            }



        }







        #region DisplayGraphBeforeCache







        //void DisplayGraph(Graphics g, int gx1, int gy1, int gx2, int gy2)



        //{







        //    //dtStartTime = new DateTime(2019, 4, 3, 15, 59, 30);







        //    //DrawClass.PushBox2(g, gx1, gy1, gx2, gy2, RunColorFill.basic_color);







        //    Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, gx1, gy1, gx2, gy2);



        //    if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE) DrawClass.PushBox2(g, gx1, gy1, gx2, gy2, RunColorFill.basic_color); //CE 소스 수정시 제거.



        //    else if (IsBackBorder()) DrawClass.PushBox2(g, gx1, gy1, gx2, gy2, brush);  //20250206 PSU 배경판 옵션 추가



        //    else { DrawClass.gcls(g, gx1, gy1, gx2, gy2, brush); } //배경판 미사용 시 pushbox 제거.







        //    DisplayGraphGuideLine(g, gx1, gy1, gx2, gy2);



        //    DisplayPatternLine(g, gx1, gy1, gx2, gy2);







        //    int sizex, sizey;



        //    int x, y = 0;



        //    double gaby;



        //    ushort j;



        //    int l;



        //    ushort pos;



        //    MD_TREND_MEMBER member;



        //    TagAiClass ai = null;



        //    int point_radios;



        //    int view_y1;



        //    int view_y2;



        //    int view_sizey;



        //    bool moveto_flag = false;



        //    int move_x = 0, move_y = 0, line_y;



        //    Font font = MakeFont();



        //    brush = new SolidBrush(Color.Black);







        //    point_radios = (gy2 - gy1 + 1) * objArgs.pub.wPointSize / 1000;



        //    if (point_radios < 1) point_radios = 1;







        //    sizex = gx2 - gx1; // +1을 안하는 것이 정확한 값이다. 10.2.1



        //    //sizex = gx2-gx1+1;



        //    sizey = gy2 - gy1 + 1;







        //    int bar = 0;







        //    for (l = 0; l < blockMember.Count; l++)



        //    {



        //        member = (MD_TREND_MEMBER)blockMember[l];







        //        if (member.nGraphType == 1) bar++;







        //        if (member.visible == 0) continue;







        //        if (member.nType == EnumTagType.AI)



        //        {



        //            ai = GetRealTagAI(member.tag, ref member.nPos);



        //            gaby = member.max_value - member.min_value;



        //        }



        //        else if (member.nType == EnumTagType.DI)



        //        {



        //            gaby = 100;



        //        }



        //        else



        //        {



        //            gaby = member.view_full - member.view_base;



        //        }







        //        view_y1 = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;



        //        view_y2 = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;



        //        view_sizey = view_y2 - view_y1 + 1;







        //        if (member.nType == EnumTagType.AI)



        //        {	// AI 일때만 경계치 표시



        //            Pen hPenLimit = new Pen(member.color, 1);



        //            hPenLimit.DashStyle = DashStyle.Dot;







        //            if ((member.wFlags & 0x0001) > 0)



        //            {	// hihi



        //                if (gaby == 0)	// protect divide by zero



        //                    y = 0;



        //                else



        //                    y = (int)((double)view_sizey * (ai.hihi - member.min_value) / gaby);



        //                if (y < 0) y = 0;



        //                if (y >= view_sizey) y = view_sizey - 1;







        //                if (member.nReverseY != 0)



        //                    y = view_y1 + y;



        //                else



        //                    y = view_y2 - y;







        //                hPenLimit.Color = objArgs.pub.colorHiHi;







        //                g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);



        //            }



        //            if ((member.wFlags & 0x0002) > 0)



        //            {	// hihi



        //                if (gaby == 0)	// protect divide by zero



        //                    y = 0;



        //                else



        //                    y = (int)((double)view_sizey * (ai.high - member.min_value) / gaby);



        //                if (y < 0) y = 0;



        //                if (y >= view_sizey) y = view_sizey - 1;







        //                if (member.nReverseY != 0)



        //                    y = view_y1 + y;



        //                else



        //                    y = view_y2 - y;







        //                hPenLimit.Color = objArgs.pub.colorHigh;







        //                g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);



        //            }



        //            if ((member.wFlags & 0x0004) > 0)



        //            {	// hihi



        //                if (gaby == 0)	// protect divide by zero



        //                    y = 0;



        //                else



        //                    y = (int)((double)view_sizey * (ai.low - member.min_value) / gaby);



        //                if (y < 0) y = 0;



        //                if (y >= view_sizey) y = view_sizey - 1;







        //                if (member.nReverseY != 0)



        //                    y = view_y1 + y;



        //                else



        //                    y = view_y2 - y;







        //                hPenLimit.Color = objArgs.pub.colorLow;







        //                g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);



        //            }



        //            if ((member.wFlags & 0x0008) > 0)



        //            {	// hihi



        //                if (gaby == 0)	// protect divide by zero



        //                    y = 0;



        //                else



        //                    y = (int)((double)view_sizey * (ai.lolo - member.min_value) / gaby);



        //                if (y < 0) y = 0;



        //                if (y >= view_sizey) y = view_sizey - 1;







        //                if (member.nReverseY != 0)



        //                    y = view_y1 + y;



        //                else



        //                    y = view_y2 - y;







        //                hPenLimit.Color = objArgs.pub.colorLoLo;







        //                g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);



        //            }



        //        }







        //        Pen pen = new Pen(member.color, member.nLineThick);







        //        pos = 0;



        //        moveto_flag = false;







        //        //dt = member.ds.Tables[member.tag];







        //        for (j = 0; j < objArgs.wShowUnit; j++, pos++)



        //        {



        //            //row = dt.Rows[j];







        //            if (objArgs.wShowUnit - 1 == 0)	// protect device by zero



        //                x = 0;



        //            else



        //                x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));







        //            if (gaby == 0)	// protect divide by zero



        //                y = 0;



        //            else



        //            {



        //                if (member.point[j].read_flag)



        //                {	// 값을 읽었을 때만



        //                    if (member.nType == EnumTagType.AI)



        //                    {	// AI



        //                        if (objArgs.logarithmicScale.bUse)



        //                        {



        //                            double log_full, log_base;







        //                            if (member.max_value == 0)



        //                                log_full = 0;



        //                            else



        //                                log_full = Math.Log(member.max_value, objArgs.logarithmicScale.fBase);







        //                            if (member.min_value == 0)



        //                                log_base = 0;



        //                            else



        //                                log_base = Math.Log(member.min_value, objArgs.logarithmicScale.fBase);







        //                            double log_val = Math.Log(TagUtil.GetDisplayValue(ai, member.point[j].val), objArgs.logarithmicScale.fBase);







        //                            double log_gab = log_full - log_base;







        //                            if (log_gab == 0)



        //                                y = 0;



        //                            else



        //                                y = (int)((double)view_sizey * (log_val - log_base) / log_gab);



        //                        }



        //                        else



        //                        {



        //                            y = (int)((double)view_sizey * (TagUtil.GetDisplayValue(ai, member.point[j].val) - member.min_value) / gaby);



        //                        }



        //                    }



        //                    else if (member.nType == EnumTagType.DI)



        //                    {



        //                        if (member.point[j].val > 0)



        //                            y = (int)(view_sizey * 0.9);



        //                        else



        //                            y = (int)(view_sizey * 0.1);



        //                    }



        //                    else



        //                    {



        //                        y = (int)((double)view_sizey * (member.point[j].val - member.min_value) / gaby);



        //                    }



        //                }



        //            }







        //            if (y < 0) y = 0;



        //            if (y >= view_sizey) y = view_sizey - 1;







        //            if ((j % nDataGab) == 0 || j == objArgs.wShowUnit - 1)



        //            {	// 마지막이거나 해당 분일 때



        //                if (member.nGraphType == 1) // bar graph



        //                {



        //                    if (member.point[pos].read_flag)



        //                    {



        //                        brush = new SolidBrush(member.color);







        //                        int thick = (sizex - objArgs.wShowUnit) / (objArgs.wShowUnit);  // 바의 크기는 그래프 갯수의 반은되어야 각 바만큼의 공간을 확보할 수 있다.



        //                        if (thick < 1) thick = 1;







        //                        if (member.nLineThick < thick)



        //                            thick = member.nLineThick;



        //                        int left_thick = (thick - 1) / 2;



        //                        int right_thick = (thick) / 2;



        //                        int x1, x2;







        //                        if (objArgs.wShowUnit - 1 == 0)	// protect device by zero



        //                            x = 0;



        //                        else



        //                            x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));







        //                        move_x = x + gx1;







        //                        if (j == 0)



        //                        {



        //                            x1 = move_x;



        //                            x2 = move_x + right_thick;



        //                        }



        //                        else if (j == objArgs.wShowUnit - 1)



        //                        {



        //                            x1 = move_x - left_thick;



        //                            x2 = move_x;



        //                        }



        //                        else



        //                        {



        //                            x1 = move_x - left_thick;



        //                            x2 = move_x + right_thick;



        //                        }







        //                        int result_y;



        //                        if (member.nReverseY == 1)



        //                        {



        //                            result_y = view_y1 + y;



        //                            DrawClass.gcls(g, x1, view_y1, x2, result_y, brush);



        //                        }



        //                        else



        //                        {



        //                            result_y = view_y2 - y;



        //                            DrawClass.gcls(g, x1, result_y, x2, view_y2, brush);



        //                        }



        //                    }



        //                }







        //                else 



        //                { // Line Graph



        //                    if (member.point[j].read_flag)



        //                    {



        //                        if (member.nReverseY == 1) line_y = view_y1 + y;



        //                        else line_y = view_y2 - y;







        //                        if (moveto_flag == false)



        //                        {



        //                            move_x = x + gx1;



        //                            move_y = line_y;







        //                            //MoveToEx(hdc, x+gx1, view_y2-y, null);



        //                            moveto_flag = true;



        //                        }



        //                        else



        //                        {



        //                            g.DrawLine(pen, move_x, move_y, x + gx1, line_y);



        //                            move_x = x + gx1;



        //                            move_y = line_y;



        //                        }







        //                        if (member.nPointType == 0)



        //                        {



        //                            DrawClass.gcls(g, x + gx1, line_y, x + gx1, line_y, member.color);



        //                        }



        //                        else



        //                        {



        //                            DisplayPoint(g, x + gx1, line_y, member.color, member.nPointType, point_radios);



        //                        }



        //                    }



        //                    else



        //                    {



        //                        moveto_flag = false;



        //                    }



        //                }



        //                /*



        //                else	// Bar graph



        //                {



        //                    if (member.point[j].read_flag)



        //                    {



        //                        int bar_size = ((gx2 - gx1) / objArgs.wShowUnit) - 1 - nBarGraphCount;



        //                        if (bar_size > 10) bar_size = 10;



        //                        if (bar_size < 1) bar_size = 1;



        //                        bar_size /= 2;



        //                        line_x = x + gx1 - nBarGraphCount / 2 + (bar - 1);







        //                        if (member.nReverseY == 1)



        //                        {



        //                            line_y = view_y1 + y;



        //                            DrawClass.gcls(g, line_x - bar_size, view_y1, line_x + bar_size, line_y, member.color);



        //                        }



        //                        else



        //                        {



        //                            line_y = view_y2 - y;



        //                            DrawClass.gcls(g, line_x - bar_size, line_y, line_x + bar_size, view_y2, member.color);



        //                        }



        //                    }



        //                }*/



        //            }



        //        }



        //    }



        //}







        #endregion







        string GetDateString(int pos, out DateTime dt)



        {



            string buf;







            dt = new DateTime(dtStartTime.Ticks);







            int data_gab = pos * objArgs.nDataCycle;







            if (objArgs.wTimeSelectOption == 0)



            {	// min data



                dt = dt.AddMilliseconds(data_gab);



                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}.{6:000}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);



            }



            else if (objArgs.wTimeSelectOption == 1)



            {	// min data



                dt = dt.AddSeconds(data_gab);



                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);



            }



            else if (objArgs.wTimeSelectOption == 2)



            {	// min data



                dt = dt.AddMinutes(data_gab);



                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);



            }



            else if (objArgs.wTimeSelectOption == 3)



            {	// hour data



                dt = dt.AddHours(data_gab);



                if (Tools.IsLangKorean())



                {



                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}시", dt.Year, dt.Month, dt.Day, dt.Hour);



                }



                else if (Tools.IsLangJapanese())



                {



                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}時", dt.Year, dt.Month, dt.Day, dt.Hour);



                }



                else if (Tools.IsLangChinese())



                {



                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}时", dt.Year, dt.Month, dt.Day, dt.Hour);



                }



                else



                {



                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}H", dt.Year, dt.Month, dt.Day, dt.Hour);



                }



            }



            else if (objArgs.wTimeSelectOption == 4)



            {	// day data



                dt = dt.AddDays(data_gab);



                buf = String.Format("{0:0000}/{1:00}/{2:00}", dt.Year, dt.Month, dt.Day);



            }



            else if (objArgs.wTimeSelectOption == 5)



            {	// mon data



                dt = dt.AddMonths(data_gab);



                if (Tools.IsLangKorean())



                {



                    buf = String.Format("{0:0000}년 {1:00}월", dt.Year, dt.Month);



                }



                else if (Tools.IsLangJapanese())



                {



                    buf = String.Format("{0:0000}年 {1:00}月", dt.Year, dt.Month);



                }



                else if (Tools.IsLangChinese())



                {



                    buf = String.Format("{0:0000}年 {1:00}月", dt.Year, dt.Month);



                }



                else



                {



                    buf = String.Format("{0:0000}/{1:00}", dt.Year, dt.Month);



                }



            }



            else if (objArgs.wTimeSelectOption == 6)



            {	// mon data



                dt = dt.AddYears(data_gab);



                if (Tools.IsLangKorean())



                {



                    buf = String.Format("{0:0000}년", dt.Year);



                }



                else if (Tools.IsLangJapanese())



                {



                    buf = String.Format("{0:0000}年", dt.Year);



                }



                else if (Tools.IsLangChinese())



                {



                    buf = String.Format("{0:0000}年", dt.Year);



                }



                else



                {



                    buf = String.Format("{0:0000}", dt.Year);



                }



            }



            else



            {



                buf = "Time type unknown";



            }



            return buf;



        }







        void DisplayCursor(Graphics g, int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2)



        {



            RECT r = new RECT();







            r.top = gy1 + 1;



            r.bottom = gy2;







            if (nCursorX1 == nCursorX2)



            {



                r.left = nCursorX1 * (gx2 - gx1) / (objArgs.wShowUnit - 1) + gx1;



                r.right = r.left + 2;



            }



            else



            {



                int x1, x2;



                x1 = nCursorX1;



                x2 = nCursorX2;



                if (x1 > x2) Tools.Temp(ref x1, ref x2);



                r.left = x1 * (gx2 - gx1) / (objArgs.wShowUnit - 1) + gx1;



                r.right = x2 * (gx2 - gx1) / (objArgs.wShowUnit - 1) + gx1;



            }







            Brush brush_fill = new SolidBrush(Color.FromArgb(0x80, objArgs.lColorGuideLine));







            g.FillRectangle(brush_fill, r.left, r.top, r.right - r.left, r.bottom - r.top);



            //DrawClass.InvertRect(g, r.left, r.top, r.right, r.bottom);







            if (bDisplayPointDate)



            {



                string buf;



                DateTime t;







                //buf = GetDateString(nCursorX1, out t);







                int x1, x2;  //20250306 PSU 수정. 앞쪽 커서위치로 표시



                x1 = nCursorX1;



                x2 = nCursorX2;



                if (x1 > x2) Tools.Temp(ref x1, ref x2);



                buf = GetDateString(x1, out t);







                Font font = MakeFont();



                SizeF size;



                size = g.MeasureString(buf, font);







                r.bottom = gy1;



                r.top = r.bottom - nCharHeight;



                r.left = (int)(r.left - size.Width / 2);



                r.right = (int)(r.left + size.Width);







                if (r.left <= ox1)



                {



                    r.left = ox1 + 1;



                    r.right = (int)(r.left + size.Width);



                }



                if (r.right >= ox2)



                {



                    r.right = ox2 - 1;



                    r.left = (int)(r.right - size.Width);



                }







                Brush brush = new SolidBrush(RunColorText);



                //Brush brush = new SolidBrush(Color.Black);



                StringFormat format = new StringFormat();



                format.Alignment = StringAlignment.Near;



                format.LineAlignment = StringAlignment.Far;







                DrawClass.DrawText(g, buf, font, brush, r, format);



                //DrawText(hdc, buf, strlen(buf), &r, DT_LEFT|DT_SINGLELINE|DT_BOTTOM);



            }



            brush_fill.Dispose();
            brush.Dispose();
            format.Dispose();
        }







        void DisplayGraphGuideLine(Graphics g, int gx1, int gy1, int gx2, int gy2)



        {



            int pos;



            int i;



            int time_devide = GetDisplayTimeDevide(gx1, gx2);



            GUIDE_DISPLAY guide;



            int real_y, real_size;







            Pen pen = new Pen(objArgs.lColorGuideLine, 1);







            for (int j = 0; j < arrayGuideDisplay.Count; j++)



            {



                guide = (GUIDE_DISPLAY)arrayGuideDisplay[j];







                for (i = 0; i <= objArgs.wLevelDevide; i++)



                {



                    if (i == 0 && guide.from == 0) continue;



                    if (i == objArgs.wLevelDevide && guide.to == 100) continue;







                    real_size = (guide.to - guide.from) * (gy2 - gy1) / 100;



                    real_y = gy2 - (guide.from) * (gy2 - gy1) / 100;



                    pos = real_y - (real_size) * i / (objArgs.wLevelDevide);







                    //pos = gy2-(gy2-gy1)*i/objArgs.wLevelDevide;



                    g.DrawLine(pen, gx1 + 1, pos, gx2, pos);



                }



            }







            ushort w;



            DateTime t = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, dtStartTime.Second);







            if (objArgs.wTimeSelectOption == 0)



            {	// 밀리 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMilliseconds(objArgs.nDataCycle))



                {



                    if ((t.Second % time_devide) != 0 || t.Millisecond != 0) continue;







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else if (objArgs.wTimeSelectOption == 1)



            {	// 초 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddSeconds(objArgs.nDataCycle))



                {



                    //if ((t.Second % time_devide) != 0) continue;



                    if (((t.Second + t.Minute * 60) % time_devide) != 0) continue;    // 범위를 넘어서면 선이 그려지지 않는다. MultiTrend와 같은 방식 사용 2019-8-19







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else if (objArgs.wTimeSelectOption == 2)



            {	// 분 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMinutes(objArgs.nDataCycle))



                {



                    //if ((t.Minute % time_devide) != 0) continue;



                    if (((t.Minute + t.Hour * 60) % time_devide) != 0) continue;    // 범위를 넘어서면 선이 그려지지 않는다. MultiTrend와 같은 방식 사용 2019-8-19







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else if (objArgs.wTimeSelectOption == 3)



            {	// 시간 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddHours(objArgs.nDataCycle))



                {



                    //if ((t.Hour % time_devide) != 0) continue;



                    if (((t.Hour + (t.Day - 1) * 24) % time_devide) != 0) continue; // 범위를 넘어서면 선이 그려지지 않는다. MultiTrend와 같은 방식 사용 2019-8-19







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else if (objArgs.wTimeSelectOption == 4)



            {	// 일일 데이터



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddDays(objArgs.nDataCycle))



                {



                    if (((t.Day - 1) % time_devide) != 0) continue;







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else if (objArgs.wTimeSelectOption == 5)



            {	// 월간 데이터.



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddMonths(objArgs.nDataCycle))



                {



                    if (((t.Month - 1) % time_devide) != 0) continue;







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else if (objArgs.wTimeSelectOption == 6)



            {	// 연간 데이터.



                for (w = 0; w < objArgs.wShowUnit; w++, t = t.AddYears(objArgs.nDataCycle))



                {



                    if (((t.Year) % time_devide) != 0) continue;







                    if (objArgs.wShowUnit - 1 == 0) continue;



                    pos = (int)(gx1 + (long)(gx2 - gx1) * w / (objArgs.wShowUnit - 1));







                    g.DrawLine(pen, pos, gy1 + 1, pos, gy2);



                }



            }



            else



            {	// 분 데이터.







            }



            pen.Dispose();
        }







        int nOldSec = -1;







        async Task CheckTimeChange()



        {



            if (objArgs.bAutoUpdate == false) return;



            if (cGraphStartTimeMethod != 0) return;	// auto가 아니면 return;



            DateTime t = DateTimeServer.Now;







            if (t.Second == nOldSec) return;



            nOldSec = t.Second;







            if (objArgs.wTimeSelectOption == 0) // milli sec



            {



                //if ((t.Second % objArgs.nDataCycle) != 0) return;



            }



            else if (objArgs.wTimeSelectOption == 1)    // sec



            {



                if ((t.Second % objArgs.nDataCycle) != 0) return;



            }



            else if (objArgs.wTimeSelectOption == 2)    // minutes



            {



                if ((t.Minute % objArgs.nDataCycle) != 0) return;



            }



            else if (objArgs.wTimeSelectOption == 3)    // hour



            {



                if ((t.Hour % objArgs.nDataCycle) != 0) return;



            }



            else



            {



                return;



            }











            CalcStartTime();



            // await ReadAllPoint();  // 기존: 완료까지 대기



            _ = Task.Run(async () =>    // 변경: 백그라운드 실행



            {



                await ReadAllPoint();



                formParent?.BeginInvoke((Action)(() => InvalidateObject(formParent)));



            });



            // InvalidateObject(formParent);  // Task.Run 내부로 이동







            /*



            nCursorX1--;	// 지시선도 같이 흘러준다.



            nCursorX2--;



            if (nCursorX1 < 0) nCursorX1 = 0;



            if (nCursorX2 < 0) nCursorX2 = 0;







            CalcStartTime();



            ReadAllPoint();



            InvalidateObject(formParent);*/



            await Task.CompletedTask;



        }







        public override async Task EventTimerObject(System.Windows.Forms.Form form)



        {



            TagPublicClass pub;







            MD_TREND_MEMBER member;



            int i;







            for (i = 0; i < blockMember.Count; i++)



            {



                member = (MD_TREND_MEMBER)blockMember[i];



                pub = TagLib.GetStructPublic(member.tag, ref member.nPos);



                pub.NeedDataCurr = true;



            }







            await CheckTimeChange();



            // await Task.CompletedTask;



        }







        public override void EventTag(System.Windows.Forms.Form form, TagPublicClass tagevent)



        {



            TagPublicClass pub;







            MD_TREND_MEMBER member;



            int i;







            for (i = 0; i < blockMember.Count; i++)



            {



                member = (MD_TREND_MEMBER)blockMember[i];



                pub = TagLib.GetStructPublic(member.tag, ref member.nPos);



                if (member.tag == tagevent.tag)



                {



                    InvalidateObject(form);



                    return;



                }



            }



        }







        byte bMouseLeftOrRight = 0;







        [NonSerialized]



        ToolTip toolTip = new ToolTip();



        int nToolTipX, nToolTipY;







        void ShowToolTip(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e, int posx, int gy1, int gy2)



        {



            if (!bShowToolTip) return;



            if (blockMember.Count == 0) return;		// 멤버가 없다.



            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN) return;







            int sizey;



            int y = 0;



            double gaby;



            int l;



            MD_TREND_MEMBER member;



            TagAiClass ai = null;



            int view_y1;



            int view_y2;



            int view_sizey;



            int small_gab = Math.Abs(gy2 - gy1);



            int small_pos = 0;



            int line_y;



            int gab;







            sizey = gy2 - gy1 + 1;







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                if (member.visible == 0) continue;







                if (member.nType == EnumTagType.AI)



                {



                    ai = GetRealTagAI(member.tag, ref member.nPos);



                    gaby = member.max_value - member.min_value;



                }



                else if (member.nType == EnumTagType.DI)



                {



                    gaby = 100;



                }



                else



                {



                    gaby = member.view_full - member.view_base;



                }







                view_y1 = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;



                view_y2 = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;



                view_sizey = view_y2 - view_y1 + 1;







                if (gaby == 0)	// protect divide by zero



                    y = 0;



                else



                {



                    if (member.point[posx].read_flag)



                    {	// 값을 읽었을 때만



                        if (member.nType == EnumTagType.AI)



                        {	// AI



                            y = (int)((double)view_sizey * (TagUtil.GetDisplayValue(ai, member.point[posx].val) - member.min_value) / gaby);



                        }



                        else if (member.nType == EnumTagType.DI)



                        {



                            if (member.point[posx].val > 0)



                                y = (int)(view_sizey * 0.9);



                            else



                                y = (int)(view_sizey * 0.1);



                        }



                        else



                        {



                            y = (int)((double)view_sizey * (member.point[posx].val - member.min_value) / gaby);



                        }



                    }



                }







                if (y < 0) y = 0;



                if (y >= view_sizey) y = view_sizey - 1;







                if (member.nReverseY == 1) line_y = view_y1 + y;



                else line_y = view_y2 - y;







                gab = Math.Abs(e.Y - line_y);



                if (gab < small_gab)



                {



                    small_gab = gab;



                    small_pos = l;



                }



            }











            string des;







            member = (MD_TREND_MEMBER)blockMember[small_pos];







            if (member.nType == EnumTagType.AI)



            {



                ai = GetRealTagAI(member.tag, ref member.nPos);



                des = ai.description;



            }



            else if (member.nType == EnumTagType.DI)



            {



                TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);



                des = di.description;



            }



            else



            {



                des = member.tag;



            }







            DateTime t;



            string msg = String.Format("{0}\nData={1}\nTime={2}", des, member.point[posx].val, GetDateString(posx, out t));







            toolTip.SetToolTip(form, msg);



            toolTip.InitialDelay = 0;



            nToolTipX = e.X;



            nToolTipY = e.Y;



        }







        public override async Task<bool> WmLeftButtonDown(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)



        {



            if (bMouseCapture) return false;



            if (!CheckResponseOnVisible()) return false;



            if (!IsViewModeControl()) return false; //20250312 PSU 







            int mx = e.X;



            int my = e.Y;







            int gx1 = 0, gy1 = 0, gx2 = 0, gy2 = 0;



            int ox1 = 0, oy1 = 0, ox2 = 0, oy2 = 0;







            GetViewZone(ref ox1, ref oy1, ref ox2, ref oy2);



            GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);







            if (mx >= gx1 && my >= gy1 && mx <= gx2 && my <= gy2)



            {



                int pos = (int)((float)(objArgs.wShowUnit - 1) * (mx - gx1) / (gx2 - gx1) + 0.5);







                ShowToolTip(form, e, pos, gy1, gy2);







                nCursorX1 = pos;



                nCursorX2 = pos;



                InvalidateObject(form);



                form.Capture = true;



                bMouseCapture = true;



                bMouseLeftOrRight = 0;







                return true;



            }







            if (LeftButtonCheckTagZone(ox1, oy1, ox2, oy2, gx1, gy1, gx2, gy2, mx, my))



            {



                InvalidateGraphCache(); //250805 PSU 태그명 클릭 시 다시그리기.



                InvalidateObject(form);



                return true;



            }







            if (!objArgs.bDontUseConfigDialog && mx >= ox1 && my >= oy1 && mx <= ox2 && my <= oy2)



            {



                CONFIG_DATABASE_TREND config;







                config = LoadTrendConfig();







                // 설정대화상자의 시간도 함께 따라간다. 2019-9-20



                config.cStartMethod = this.cGraphStartTimeMethod;



                config.dtStart = this.dtStartTime;



                config.bAutoRange = this.bAutoViewRange;







                config.nMovePeriod = this.nMovePeriod; //20250306 PSU







                ConfigDatabaseTrend dialog = new ConfigDatabaseTrend(objGeneral.sClassName, config);







                if (Tools.IsLangKorean())



                {



                    dialog.Text = "미세자료 트랜드 설정";



                }



                else



                {



                    dialog.Text = "MilliData Trend Config";



                }







                if (dialog.ShowDialog() == DialogResult.OK)



                {



                    SaveMultiTrendConfig(config);







                    LoadConfigAndExcute();



                    await ReadAllPoint();







                    InvalidateObject(form);



                }







                return true;



            }



            await Task.CompletedTask;



            return false;







        }







        public override async Task<bool> WmLeftButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)



        {



            await Task.Yield(); // 경고 해결용







            if (!bMouseCapture) return false;







            form.Capture = false;



            bMouseCapture = false;







            return true;



        }







        int nFirstX = 0, nFirstY = 0;



        int nOldGab = 0;







        public override async Task<bool> WmMouseMove(System.Windows.Forms.Form form, MouseEventArgs e)



        {



            if (Math.Abs(nToolTipX - e.X) > 10 || Math.Abs(nToolTipY - e.Y) > 10)



                toolTip.SetToolTip(form, "");







            if (!bMouseCapture)



            {



                //if(!bShowToolTip)	// 마우스를 누를때 선택한 시점의 시간과 자료값을 보여주는 툴팁을 사용하지 않을때만 클래스 툴팁을 보여준다.



                await base.ToolTipCheck(form, e);



                return false;



            }







            int mx = e.X;



            int my = e.Y;







            int gx1 = 0, gy1 = 0, gx2 = 0, gy2 = 0;



            int ox1 = 0, oy1 = 0, ox2 = 0, oy2 = 0;







            GetViewZone(ref ox1, ref oy1, ref ox2, ref oy2);



            GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);







            if (bMouseLeftOrRight == 0)



            {	// left button pressing



                int pos = (int)((float)(objArgs.wShowUnit - 1) * (mx - gx1) / (gx2 - gx1) + 0.5);



                if (pos < 0) pos = 0;



                if (pos >= objArgs.wShowUnit) pos = objArgs.wShowUnit - 1;



                nCursorX2 = pos;



                InvalidateObject(form);



            }



            else



            {



                int calc_gab = (int)((float)(objArgs.wShowUnit - 1) * (mx - nFirstX) / (gx2 - gx1));



                int gab;



                int i;







                if (calc_gab != nOldGab)



                {



                    gab = calc_gab - nOldGab;



                    nOldGab = calc_gab;











                    if (objArgs.wTimeSelectOption == 0)



                    {	// milli data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddMilliseconds(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddMilliseconds(objArgs.nDataCycle);



                        }



                    }



                    else if (objArgs.wTimeSelectOption == 1)



                    {	// sec data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddSeconds(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddSeconds(objArgs.nDataCycle);



                        }



                    }



                    else if (objArgs.wTimeSelectOption == 2)



                    {	// min data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddMinutes(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddMinutes(objArgs.nDataCycle);



                        }



                    }



                    else if (objArgs.wTimeSelectOption == 3)



                    {	// hour data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddHours(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddHours(objArgs.nDataCycle);



                        }



                    }



                    else if (objArgs.wTimeSelectOption == 4)



                    {	// day data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddDays(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddDays(objArgs.nDataCycle);



                        }



                    }



                    else if (objArgs.wTimeSelectOption == 5)



                    {	// mon data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddMonths(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddMonths(objArgs.nDataCycle);



                        }



                    }



                    else if (objArgs.wTimeSelectOption == 6)



                    {	// mon data



                        if (gab > 0)



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddYears(-objArgs.nDataCycle);



                        }



                        else



                        {



                            for (i = 0; i < Math.Abs(gab); i++) dtStartTime = dtStartTime.AddYears(objArgs.nDataCycle);



                        }



                    }











                    if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



                    {



                        cGraphStartTimeMethod = 1;	// 수동.



                    }







                    if ((objArgs.wShowUnit * blockMember.Count) <= 60)  //20250711 PSU 데이터개수*멤버가 60 개 이상이면 드래그 종료 후 갱신



                    {



                        await ReadAllPoint();



                        InvalidateObject(form);



                    }



                }



            }







            return true;



        }







        public override async Task<bool> WmRightButtonDown(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)



        {



            await Task.CompletedTask;// 경고 해결용







            if (bMouseCapture) return false;



            if (!CheckResponseOnVisible()) return false;







            int mx = e.X;



            int my = e.Y;







            int gx1 = 0, gy1 = 0, gx2 = 0, gy2 = 0;



            int ox1 = 0, oy1 = 0, ox2 = 0, oy2 = 0;







            GetViewZone(ref ox1, ref oy1, ref ox2, ref oy2);



            GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);







            if (mx >= gx1 && my >= gy1 && mx <= gx2 && my <= gy2)



            {



                form.Capture = true;



                bMouseCapture = true;



                bMouseLeftOrRight = 1;



                nFirstX = mx;



                nFirstY = my;



                nOldGab = 0;



                //				captureTrend = this;







                return true;



            }







            if (RightButtonCheckTagZone(form, ox1, oy1, ox2, oy2, gx1, gy1, gx2, gy2, mx, my))



            {



                InvalidateObject(form);



                return true;



            }







            return false;



        }







        public override async Task<bool> WmRightButtonUp(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)



        {



            if (!bMouseCapture) return false;







            form.Capture = false;



            bMouseCapture = false;







            if ((objArgs.wShowUnit * blockMember.Count) > 60)  //20250711 PSU 데이터개수*멤버가  60 개 이상이면 드래그 종료 후 갱신



            {



                await ReadAllPoint();



                InvalidateObject(form);



            }



            await Task.CompletedTask;



            return true;



        }







        bool LeftButtonCheckTagZone(int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2, int mx, int my)



        {



            if (!IsTagColor() && !IsTagMinMax() && !IsTagCurr() && !IsTagOldCurr()) return false;







            int l;



            MD_TREND_MEMBER member;



            RECT r = new RECT();



            int x;



            int y;



            int x1, y1, x2, y2;







            x = ox1 + 5;



            x1 = x;



            x2 = x + nCharWidth * 8 - 1;







            if (IsDesX()) y = gy2 + nCharHeight * 2;



            else y = gy2 + 3;







            r.left = x1 + 1;



            r.right = x2;







            x += nCharWidth * 8;







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                if (x + nCharWidth * member.nTagDisplaySize >= ox2 - 1) break;







                if (IsDesX()) y = gy2 + nCharHeight * 2;



                else y = gy2 + 3;







                x1 = x;



                x2 = x + nCharWidth * member.nTagDisplaySize - 1;



                y1 = y;







                if (IsTagColor())



                {



                    y += nCharHeight + 2;



                }



                if (IsTagMinMax())



                {







                    y += (nCharHeight + 2) * 2;



                }



                if (IsTagCurr())



                {



                    y += nCharHeight + 2;



                }



                if (IsTagOldCurr())



                {



                    y += nCharHeight + 2;



                }







                y2 = y;







                if (mx >= x1 && mx <= x2 && my >= y1 && my <= y2)



                {



                    member.visible = member.visible == 1 ? (sbyte)0 : (sbyte)1;



                    return true;



                }







                x += nCharWidth * member.nTagDisplaySize;



            }







            return false;



        }







        bool RightButtonCheckTagZone(System.Windows.Forms.Form form, int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2, int mx, int my)



        {



            if (!IsTagColor() && !IsTagMinMax() && !IsTagCurr() && !IsTagOldCurr()) return false;







            int l;



            MD_TREND_MEMBER member;



            RECT r = new RECT();



            int x;



            int y;



            int x1, y1, x2, y2;







            x = ox1 + 5;



            x1 = x;



            x2 = x + nCharWidth * 8 - 1;







            if (IsDesX()) y = gy2 + nCharHeight * 2;



            else y = gy2 + 3;







            r.left = x1 + 1;



            r.right = x2;







            x += nCharWidth * 8;







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                if (x + nCharWidth * member.nTagDisplaySize >= ox2 - 1) break;







                if (IsDesX()) y = gy2 + nCharHeight * 2;



                else y = gy2 + 3;







                x1 = x;



                x2 = x + nCharWidth * member.nTagDisplaySize - 1;



                y1 = y;







                if (IsTagColor())



                {



                    y += nCharHeight + 2;



                }



                if (IsTagMinMax())



                {



                    if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



                    {



                        if (bUseLocalRange)



                        {



                            if (mx >= x1 && mx <= x2 && my >= y && my < y + (nCharHeight + 2) * 2)



                            {







                                ConfigMultiTrendRange dialog = new ConfigMultiTrendRange();







                                if (Tools.IsLangKorean())



                                    dialog.Text = "미세자료 트랜드 그래프 범위설정";



                                else if (Tools.IsLangChinese())



                                    dialog.Text = "设置微细资料倾向范围";



                                else



                                    dialog.Text = "MilliData Trend Graph Range Setting";







                                dialog.fViewFull = member.view_full;



                                dialog.fViewBase = member.view_base;







                                if (dialog.ShowDialog() == DialogResult.OK)



                                {



                                    member.view_full = dialog.fViewFull;



                                    member.view_base = dialog.fViewBase;







                                    SaveMultiTrendMemberConfig();



                                    //LoadConfigAndExcute();



                                    InvalidateObject(form);



                                }







                                return true;



                            }



                        }



                    }



                    y += (nCharHeight + 2) * 2;



                }



                if (IsTagCurr())



                {



                    y += nCharHeight + 2;



                }



                if (IsTagOldCurr())



                {



                    y += nCharHeight + 2;



                }







                y2 = y;







                if (mx >= x1 && mx <= x2 && my >= y1 && my <= y2)



                {



                    if (dwFirstLevelDisplay != l)



                    {



                        dwFirstLevelDisplay = l;



                        InvalidateObject(form);



                    }



                    return true;



                }







                x += nCharWidth * member.nTagDisplaySize;



            }







            return false;



        }







        public override void SetObjectOpticMethod(int method)



        {



            base.SetObjectOpticMethod(method);



            CalcCharSize();



        }







        public override void SetOpticRate(int rate)



        {



            base.SetOpticRate(rate);



            CalcCharSize();



        }







        public override void OnMove(int x1, int y1, int x2, int y2)



        {



            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



            {



                CalcCharSize();



            }



        }







        void SetShowUnit(int unit)



        {



            if (unit == objArgs.wShowUnit) return;



            if (unit < 10) unit = 10;



            objArgs.wShowUnit = unit;



            if (nCursorX1 < 0) nCursorX1 = 0;



            if (nCursorX1 >= objArgs.wShowUnit) nCursorX1 = objArgs.wShowUnit - 1;



            if (nCursorX2 < 0) nCursorX2 = 0;



            if (nCursorX2 >= objArgs.wShowUnit) nCursorX2 = objArgs.wShowUnit - 1;







            // 캐시 무효화  20250701 PSU



            InvalidateGraphCache();







            MallocAllBuf();



        }







        void SaveMultiTrendMemberConfig()



        {



            string path = ConfigVarTotal.sDirConfigUser + "\\DbTrend\\Member";







            Directory.CreateDirectory(path);







            string filename = path + "\\" + objGeneral.sClassName;







            MD_TREND_MEMBER member;



            int i;







            FileStream s = File.Open(filename, FileMode.Create);



            CommaTextWriter writer = new CommaTextWriter(s);







            for (i = 0; i < blockMember.Count; i++)



            {



                member = (MD_TREND_MEMBER)blockMember[i];



                writer.WriteLine("{0},{1},{2},", member.column, member.view_base, member.view_full);



            }



            writer.Close();



        }







        void LoadMultiTrendMemberConfig()



        {



            string path = ConfigVarTotal.sDirConfigUser + "\\DbTrend\\Member";







            string filename = path + "\\" + objGeneral.sClassName;







            if (!File.Exists(filename)) return;







            MD_TREND_MEMBER member;



            string one_line;



            string column = "";



            int i;







            FileStream s = File.OpenRead(filename);



            TextReader reader = new StreamReader(s);



            CommaBlockString comma = new CommaBlockString();







            while (true)



            {



                one_line = reader.ReadLine();







                if (one_line == null) break;



                comma.Set(one_line);



                comma.GetString(ref column);







                for (i = 0; i < blockMember.Count; i++)



                {



                    member = (MD_TREND_MEMBER)blockMember[i];



                    if (member.column == column)



                    {



                        comma.GetDouble(ref member.view_base);



                        comma.GetDouble(ref member.view_full);



                        break;



                    }



                }



            }







            reader.Close();



        }







        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)



        {



            if (command == "MilliTrendReLoad")



            {



                await ReadAllPoint();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendSetStartTime")



            {



                int year = (int)args[1];



                int month = (int)args[2];



                int day = (int)args[3];



                int hour = (int)args[4];



                int minute = (int)args[5];



                int second = (int)args[6];







                if (year < 1) year = 1;



                if (year > 9999) year = 9999;



                if (month < 1) month = 1;



                if (month > 12) month = 12;



                if (day < 1) day = 1;



                if (day > 31) day = 31;



                if (hour < 0) hour = 0;



                if (hour > 23) hour = 23;



                if (minute < 0) minute = 0;



                if (minute > 59) minute = 59;



                if (second < 0) second = 0;



                if (second > 59) second = 59;







                int limit_day = DateTime.DaysInMonth(year, month);



                if (day > limit_day)



                    day = limit_day;







                dtStartTime = new DateTime(year, month, day, hour, minute, second);







                this.cGraphStartTimeMethod = 1; // 수동으로 변경 2019-9-20 추가







                // 시간이 변경되었으므로 캐시 무효화



                InvalidateGraphCache();







                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendSetStartTimeAuto")   // 2020-7-16 추가



            {



                this.cGraphStartTimeMethod = 0;







                // 자동 모드로 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



            }



            else if (command == "MilliTrendSetShowSize")



            {



                objArgs.wShowUnit = (int)args[1];



                if (objArgs.wShowUnit < 2) objArgs.wShowUnit = 2;



                MallocAllBuf();



                ResetCursorPosition(); // 커서 위치 초기화



                // 표시 크기가 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendGetShowSize")



            {



                return objArgs.wShowUnit;



            }



            else if (command == "MilliTrendSetLevelDevide")    // TEST 24-03-22



            {



                if ((int)args[1] == 0)



                {



                    objArgs.wTimeDevide = (int)args[2];



                    if (objArgs.wTimeDevide < 1) objArgs.wTimeDevide = 1;



                    else if (objArgs.wTimeDevide > 10000) objArgs.wTimeDevide = 10000;



                    InvalidateObject(formParent);



                }



                else if ((int)args[1] == 1)



                {



                    objArgs.wLevelDevide = (int)args[2];



                    if (objArgs.wLevelDevide < 1) objArgs.wLevelDevide = 1;



                    else if (objArgs.wLevelDevide > 100) objArgs.wLevelDevide = 100;



                    InvalidateObject(formParent);



                }







            }



            else if (command == "MilliTrendGetLevelDevide") // TEST 24-03-22



            {



                if ((int)args[1] == 0)



                {



                    return objArgs.wTimeDevide;



                }



                else if ((int)args[1] == 1)



                {



                    return objArgs.wLevelDevide;



                }











            }



            else if (command == "MilliTrendSetDataType")



            {



                objArgs.wTimeSelectOption = (int)args[1];



                objArgs.nDataCycle = (int)args[2];



                // 데이터 타입이 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



                UpdateLabel();  //20250306 PSU 추가 .



            }



            else if (command == "MilliTrendSetData")



            {



                objArgs.sDsn = (string)args[1];



                // 데이터 소스가 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



            }



            else if (command == "MilliTrendShiftTime")



            {



                DbTrendShiftTime((int)args[1]);



                // 시간이 이동했으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendGetMemberFlags")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[pos];



                return member.wFlags;



            }



            else if (command == "MilliTrendSetMemberFlags")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[pos];



                member.wFlags = (ushort)(int)args[2];



                // 멤버 플래그가 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendClear")



            {



                this.blockMember.Clear();



                CalcAxisPosition();



                // 멤버가 클리어되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendAddMember")



            {



                MD_TREND_MEMBER member = new MD_TREND_MEMBER();







                member.tag = (string)args[1];



                member.column = (string)args[2];



                int color = (int)args[3];



                int a = (color >> 24 & 0xFF);



                int r = (color >> 16 & 0xFF);



                int g = (color >> 8 & 0xFF);



                int b = (color >> 0 & 0xFF);



                member.color = System.Drawing.Color.FromArgb(a, r, g, b);



                member.nValueType = (int)args[4];



                member.nPointType = (int)args[5];



                member.nLineThick = (int)args[6];



                member.nAxisPosition = (int)args[7];



                member.nLevelFrom = (int)args[8];



                member.nLevelTo = (int)args[9];



                member.nTagDisplaySize = (int)args[10];



                member.nReverseY = (int)args[11];



                member.wFlags = (ushort)(int)args[12];



                member.nGraphType = (int)args[13];



                //member.sTable = (string)args[14];



                //member.sWhereString = (string)args[15];



                member.sDescription = (string)args[16];







                CheckOneMember(member);



                blockMember.Add(member);



                CalcAxisPosition();



                // 멤버가 추가되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendRemoveAt")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                this.blockMember.RemoveAt(pos);



                CalcAxisPosition();



                // 멤버가 제거되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendSaveToCsv")



            {



                return SaveToCsv((string)args[1]);



            }



            else if (command == "MilliTrendGetRealPos")



            {



                int left_right = (int)args[1];



                int pos = (int)args[2];



                int level = (int)args[3];







                for (int i = 0; i < blockMember.Count; i++)



                {



                    MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[i];



                    if (left_right != member.nAxisPosition) continue;



                    if (pos != member.nAxisCalcPos) continue;



                    if (level < member.nLevelFrom) continue;



                    if (level > member.nLevelTo) continue;



                    return i;



                }







                return -1;



            }



            else if (command == "MilliTrendGetMin")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[pos];



                return member.min_value;



            }



            else if (command == "MilliTrendGetMax")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[pos];



                return member.max_value;



            }



            else if (command == "MilliTrendSetMin")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[pos];



                member.min_value = (double)args[2];



                // 범위가 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendSetMax")



            {



                int pos = (int)args[1];







                if (pos >= this.blockMember.Count) return 0;



                if (pos < 0) return 0;







                MD_TREND_MEMBER member = (MD_TREND_MEMBER)this.blockMember[pos];



                member.max_value = (double)args[2];



                // 범위가 변경되었으므로 캐시 무효화



                InvalidateGraphCache();



                InvalidateObject(formParent);



            }



            else if (command == "MilliTrendGetCursorData")



            {



                return GetCursorData((int)args[1]);



            }



            else if (command == "MilliTrendGetCursorTime")



            {



                DateTime t;



                // GetDateString(nCursorX1, out t);







                int x1 = nCursorX1;  //20250306 PSU 앞쪽 커서위치로 수정.



                int x2 = nCursorX2;







                if (x1 > x2) Tools.Temp(ref x1, ref x2);



                GetDateString(x1, out t);







                args[1] = t.Year;



                args[2] = t.Month;



                args[3] = t.Day;



                args[4] = t.Hour;



                args[5] = t.Minute;



                args[6] = t.Second;







                return 1;



            }



            else if (command == "MilliTrendGetCursorSize")



            {



                return Math.Abs(this.nCursorX2 - this.nCursorX1) + 1;



            }



            else if (command == "MilliTrendGetTimeType")



            {



                return (int)objArgs.wTimeSelectOption;



            }



            else if (command == "MilliTrendSetTimeType")



            {



                int type = (int)args[1];



                this.objArgs.wTimeSelectOption = type;







                InvalidateGraphCache();



            }



            else if (command == "MilliTrendSetAutoUpdate")



            {



                int type = (int)args[1];



                this.objArgs.bAutoUpdate = (type == 1);







                if (type == 1)



                    this.cGraphStartTimeMethod = 0; // AutotoUpdate가 1인 경우는 자동으로 변경 2020-7-16 추가







                InvalidateGraphCache();



            }



            else if (command == "MilliTrendGetAutoRange")



            {



                return this.bAutoViewRange ? 1 : 0;



            }



            else if (command == "MilliTrendSetAutoRange")



            {



                int range = ((int)args[1]);



                this.bAutoViewRange = (range == 1);







                InvalidateGraphCache();



            }



            else if (command == "MilliTrendGetStartTime")



            {



                DateTime t = dtStartTime;



                args[1] = t.Year;



                args[2] = t.Month;



                args[3] = t.Day;



                args[4] = t.Hour;



                args[5] = t.Minute;



                args[6] = t.Second;







                return 1;



            }



            else if (command == "MilliTrendGetScreenData")



            {



                return GetScreenData((int)args[1], (int)args[2]);



            }



            else if (command == "MilliTrendSetLogarithmicScale")



            {



                objArgs.logarithmicScale.bUse = ((int)args[1] == 1);



                objArgs.logarithmicScale.fBase = (double)args[2];



                if (objArgs.logarithmicScale.fBase < 2)



                    objArgs.logarithmicScale.fBase = 2;







                InvalidateGraphCache();



            }



            else if (command == "MilliTrendSetPatternLine")



            {



                PatternLineItem pli = new PatternLineItem();







                bool active = ((int)args[1] == 1);



                pli.color = Color.FromArgb((int)args[2]);



                pli.thick = (int)args[3];



                pli.pattern_cycle = (double)args[4];



                try



                {



                    pli.tStart = (DateTime)args[5];



                }



                catch   // 혹시 오류가 날수 있어서 미리 막음.



                {



                    pli.tStart = new DateTime(2000, 1, 1);  // 2023-5-25 추가.



                }



                pli.line_count = (int)args[6];



                pli.start_number = (int)args[7];



                pli.number_position = (int)args[8];



                if (pli.pattern_cycle <= 0) pli.pattern_cycle = 1;  // 0으로 나누는 것을 방지. 2023-5-25 추가.



                if (active)



                {



                    if (this.arrayPatternLine == null)



                        this.arrayPatternLine = new List<PatternLineItem>();







                    // 같은시간 같은 주기를 사용해서 글자를 왼쪽 오른쪽에 같이 표시할 수 도 있다. 2019-7-31



                    //for (int i = 0; i < this.arrayPatternLine.Count; i++)



                    //{



                    //    if (arrayPatternLine[i].tStart == pli.tStart &&



                    //        arrayPatternLine[i].pattern_cycle == pli.pattern_cycle &&



                    //        arrayPatternLine[i].line_count == pli.line_count)



                    //        return true;    // 같은 주기/크기라면 표시해도 무의미하다.



                    //}







                    // 일단 1000개만 등록해 주도록 한다. 2020-3-25 업체요청으로 1000->2000개로 변경



                    if (this.arrayPatternLine.Count >= 2000)



                        this.arrayPatternLine.RemoveAt(0);







                    this.arrayPatternLine.Add(pli);



                }



                else



                {



                    if (this.arrayPatternLine != null)



                    {



                        for (int i = 0; i < this.arrayPatternLine.Count; i++)



                        {



                            if (arrayPatternLine[i].tStart == pli.tStart)



                            {



                                this.arrayPatternLine.RemoveAt(i);



                                i--;



                                //break; 여러개가 등록될 수 있다.



                            }



                        }



                    }



                }







                SavePatternLine();



                InvalidateGraphCache();



                await Task.CompletedTask;



            }



            else if (command == "MilliTrendClearPatternLine")



            {



                this.arrayPatternLine = null;







                SavePatternLine();



                InvalidateGraphCache();



            }



            else if (command == "MilliTrendSetPatternLineVisible")



            {



                bool visible = ((int)args[1] == 1);







                bVisiblePatternLine = visible;



                InvalidateGraphCache();



            }



            else if (command == "MilliTrendSetPatternLineVisibleByColor")



            {



                Color color = Color.FromArgb((int)args[1]);



                bool visible = ((int)args[2] == 1);







                for (int i = 0; i < this.arrayPatternLine.Count; i++)



                {



                    if (arrayPatternLine[i].color == color)



                    {



                        arrayPatternLine[i].visible = visible;



                        // 같은 색상이 여래개 있을 수 있어서 계속한다.



                    }



                }



                SavePatternLine();



                InvalidateGraphCache();



            }







            //if (command == "MilliTrendSetToolbarVisible")



            //{



            //    bool visible = (bool)args[0];



            //    SetToolbarVisible(visible);



            //    return null;



            //}







            else { }







            return 0;



        }







        bool bVisiblePatternLine = true;







        void LoadPatternLine()



        {



            string dir = TotalConfig.GetProjectDataDirectory() + "\\PatternLine";







            string filename = String.Format("{0}\\{1}_{2}.txt", dir, Path.GetFileName(objCommonProperty.sModuleName), objGeneral.sClassName);







            if (!File.Exists(filename)) return;







            byte[] buffer = File.ReadAllBytes(filename);



            MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length);



            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<PatternLineItem>));







            try



            {



                arrayPatternLine = ser.ReadObject(ms) as List<PatternLineItem>;



            }



            catch (Exception exception)



            {



                string msg;



                msg = String.Format("filename={0}\nMessage={1}", filename, exception.Message);



                MessageBox.Show(msg, "LoadPatternLine");



            }



            ms.Close();



        }







        void SavePatternLine()



        {



            string dir = TotalConfig.GetProjectDataDirectory() + "\\PatternLine";







            if (!Directory.Exists(dir))



                Directory.CreateDirectory(dir);







            string filename = String.Format("{0}\\{1}_{2}.txt", dir, Path.GetFileName(objCommonProperty.sModuleName), objGeneral.sClassName);







            if ((arrayPatternLine == null || arrayPatternLine.Count == 0))



            {



                if (File.Exists(filename))



                    File.Delete(filename);







                return;



            }







            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(arrayPatternLine.GetType());



            MemoryStream ms = new MemoryStream();



            serializer.WriteObject(ms, arrayPatternLine);



            byte[] data = ms.ToArray();







            File.WriteAllBytes(filename, data);



        }







        double GetCursorData(int pos)



        {



            if (pos < 0) return 0;



            if (pos >= (int)blockMember.Count) return 0;







            MD_TREND_MEMBER member;



            double value, min, max;







            member = (MD_TREND_MEMBER)blockMember[pos];







            if (GetCursorValue(member, out value, out min, out max))



            {



                return value;



            }



            else



            {



                return 0;



            }



        }







        double GetScreenData(int pos, int type)



        {



            if (pos < 0) return 0;



            if (pos >= (int)blockMember.Count) return 0;







            MD_TREND_MEMBER member;



            double ave, min, max;







            member = (MD_TREND_MEMBER)blockMember[pos];







            bool retn = GetZoneValue(member, out ave, out min, out max, 0, objArgs.wShowUnit - 1);







            if (retn)



            {



                if (type == 1)



                    return min;



                else if (type == 2)



                    return max;



                else



                    return ave;



            }



            else



            {



                return 0;



            }



        }







        int SaveToCsv(string filename)



        {



            TextWriter writer = new StreamWriter(filename, false, System.Text.Encoding.Default);



            if (writer == null) return 0;







            int l;



            MD_TREND_MEMBER member;







            writer.Write("시간,");







            for (l = 0; l < blockMember.Count; l++)



            {



                member = (MD_TREND_MEMBER)blockMember[l];







                if (member.tag.Length > 0)



                {



                    TagPublicClass tp = TagLib.GetStructPublic(member.tag, ref member.nPos);



                    writer.Write("{0},", tp.description);



                }



                else



                {



                    writer.Write("{0},", member.column);



                }



            }







            writer.WriteLine();







            DateTime t = this.FitStartTime(dtStartTime);







            for (int w = 0; w < objArgs.wShowUnit; w++)



            {



                writer.Write("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00},", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);







                for (l = 0; l < blockMember.Count; l++)



                {



                    member = (MD_TREND_MEMBER)blockMember[l];



                    if (member.point[w].read_flag)



                        writer.Write("{0},", member.point[w].val);



                    else



                        writer.Write(",");



                }



                writer.WriteLine();







                if (objArgs.wTimeSelectOption == 0) t = t.AddMilliseconds(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 1) t = t.AddSeconds(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 2) t = t.AddMinutes(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 3) t = t.AddHours(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 4) t = t.AddDays(objArgs.nDataCycle);



                else if (objArgs.wTimeSelectOption == 5) t = t.AddMonths(objArgs.nDataCycle);



                else t = t.AddYears(objArgs.nDataCycle);



            }







            writer.Close();







            return 1;



        }







        void DbTrendShiftTime(int shift)



        {



            int data_cycle = shift * objArgs.nDataCycle;







            if (objArgs.wTimeSelectOption == 0)



            {	// milli data



                dtStartTime = dtStartTime.AddMilliseconds(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 1)



            {	// sec data



                dtStartTime = dtStartTime.AddSeconds(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 2)



            {	// min data



                dtStartTime = dtStartTime.AddMinutes(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 3)



            {	// hour data



                dtStartTime = dtStartTime.AddHours(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 4)



            {	// day data



                dtStartTime = dtStartTime.AddDays(data_cycle);



            }



            else if (objArgs.wTimeSelectOption == 5)



            {	// month data



                dtStartTime = dtStartTime.AddMonths(data_cycle);



            }



            else



            {	// year data



                dtStartTime = dtStartTime.AddYears(data_cycle);



            }



        }







        public override void ObjectSave(CommaTextWriter writer)



        {



            ObjectSaveFont(writer);







            SaveObjectItem.BackColor(writer, GetBackColor());



            SaveObjectItem.TextColor(writer, GetTextColor());



            SaveObjectItem.FillColor(writer, GetFillColor());



            SaveObjectItem.GuideLineColor(writer, objArgs.lColorGuideLine);



            SaveObjectItem.LogarithmicScale(writer, objArgs.logarithmicScale);







            writer.WriteLine("\tShowUnit,{0},", objArgs.wShowUnit);



            //writer.WriteLine("\twFlags,{0},", (int)objArgs.wDisplayFlags);



            writer.WriteLine("\twTimeDevide,{0},", objArgs.wTimeDevide);



            writer.WriteLine("\twLevelDevide,{0},", objArgs.wLevelDevide);



            SaveObjectItem.MdTrendMember(writer, blockMember);



            SaveObjectItem.TimeSelectOption(writer, objArgs.wTimeSelectOption);



            //SaveObjectItem.GraphPointSize(writer, objArgs.wPointSize);



            //SaveObjectItem.LevelDisplaySize(writer, objArgs.nLevelDisplaySize);



            SaveObjectItem.GraphPublicArgs(writer, objArgs.pub);







            writer.Write("\tStringOption,");



            writer.Write("{0},", objArgs.sDsn);



            writer.Write("{0},", "");   //objArgs.sTable);



            writer.Write("{0},", "");   //objArgs.sColumnTime);



            writer.Write("{0},", objArgs.nDataCycle);



            writer.Write("{0},", "");   //objArgs.sColumnMilli);



            writer.Write("{0},", 0);    //objArgs.nDateColumnType);



            writer.Write("{0},", objArgs.nBasicSpaceLeft);



            writer.Write("{0},", objArgs.nBasicSpaceRight);



            writer.Write("{0},", objArgs.bAutoUpdate);



            writer.Write("{0},", objArgs.bDontUseConfigDialog);



            writer.Write("{0},", objArgs.bUseToolBar);   //20250306 PSU 추가 



            writer.Write("{0},", objArgs.nToolBarPos);



            writer.Write("{0},", objArgs.nTooolBarBtnColor);



            writer.Write("{0},", objArgs.bHideLabelDataRange);



            writer.Write("{0},", objArgs.nToolBarButtonSize); //20250317 PSU 추가



            writer.Write("{0},", objArgs.nToolBarTextSize);



            writer.WriteLine();



        }











        // 툴바 버튼 생성 메서드



        private void CreateToolbarButtons(Form form)



        {



            // 9개 버튼 생성



            if (!objArgs.bDontUseConfigDialog) toolbarButtons = new PictureBox[9];  //설정창 사용 안하면 설정버튼 숨기기



            else toolbarButtons = new PictureBox[8];











            // 버튼 이미지 설정과 생성



            for (int i = 0; i < toolbarButtons.Length; i++)



            {







                toolbarButtons[i] = new PictureBox();



                toolbarButtons[i].Tag = i; // 인덱스 저장



                toolbarButtons[i].SizeMode = PictureBoxSizeMode.StretchImage;



                if (objArgs.nTooolBarBtnColor == 0) toolbarButtons[i].BackColor = Color.White;



                else toolbarButtons[i].BackColor = Color.Black;



                //toolbarButtons[i].BackColor = Color.FromArgb(50,RunColorFill.basic_color);



                toolbarButtons[i].Cursor = Cursors.Hand;



                toolbarButtons[i].BorderStyle = BorderStyle.None; //모듈 확대/축소 시 테두리 잔상으로 직접 그림.











                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN) toolbarButtons[i].Click += async (sender, e) => await ToolbarButton_Click(sender, e);







                // // 마우스 이벤트 추가 (hover 효과)



                // toolbarButtons[i].MouseEnter += ToolbarButton_MouseEnter;



                // toolbarButtons[i].MouseLeave += ToolbarButton_MouseLeave;







                // 이미지 설정



                SetButtonImage(toolbarButtons[i], i);



                toolTipToolbar?.SetToolTip(toolbarButtons[i], toolTipToolbars[i]);







                form.Controls.Add(toolbarButtons[i]);



            }







            // 버튼 위치 초기화



            //UpdateToolbarPosition();







            if (!objArgs.bHideLabelDataRange)



            {



                // 정보 라벨 추가



                lblInfo = new Label();



                lblInfo.AutoSize = false;



                lblInfo.TextAlign = ContentAlignment.MiddleCenter;



                lblInfo.BackColor = Color.White;



                if (objArgs.nTooolBarBtnColor == 1)



                {



                    lblInfo.BackColor = Color.Black;



                }



                lblInfo.ForeColor = objArgs.nTooolBarBtnColor == 1 ? Color.White : Color.Black;







                UpdateLabel(); // 라벨 텍스트 업데이트







                form.Controls.Add(lblInfo);



            }



        }







        // 버튼 이미지 설정 메서드



        private void SetButtonImage(PictureBox button, int index)



        {



            // hover 상태에 따라 다른 이미지 설정



            if (objArgs.nTooolBarBtnColor == 0)



            {



                switch (index)



                {



                    case 0: // 트렌드 모드



                        bool isAutoUpdateOn = this.objArgs.bAutoUpdate;



                        if (isAutoUpdateOn)



                        {



                            button.Image = Properties.Resources.stop;



                        }



                        else



                        {



                            button.Image = Properties.Resources.start;



                        }



                        break;



                    case 1: // 축소



                        button.Image = Properties.Resources.zoom_out;



                        break;







                    case 2: // 확대



                        button.Image = Properties.Resources.zoom_in;



                        break;







                    case 3: // 이전 프레임



                        button.Image = Properties.Resources.double_arrow_left;



                        break;







                    case 4: // 앞 주기



                        button.Image = Properties.Resources.arrow_left;



                        break;







                    case 5: // 뒤 주기



                        button.Image = Properties.Resources.arrow_right;



                        break;







                    case 6: // 뒤 프레임



                        button.Image = Properties.Resources.double_arrow_right;



                        break;







                    case 7: // 마우스 확대/축소



                        bool isOriginalSize = originalShowUnit > 0;



                        if (isOriginalSize)



                        {



                            button.Image = Properties.Resources.pinch_zoom_out;



                        }



                        else



                        {



                            button.Image = Properties.Resources.pinch_zoom_in;



                        }



                        break;







                    case 8: // 설정



                        button.Image = Properties.Resources.config;



                        break;



                }



            }



            else



            {







                switch (index)



                {



                    case 0: // 트렌드 모드



                        bool isAutoUpdateOn = this.objArgs.bAutoUpdate;



                        if (isAutoUpdateOn)



                        {



                            button.Image = Properties.Resources.stop_w;



                        }



                        else



                        {



                            button.Image = Properties.Resources.start_w;



                        }



                        break;



                    case 1: // 축소



                        button.Image = Properties.Resources.zoom_out_w;



                        break;







                    case 2: // 확대



                        button.Image = Properties.Resources.zoom_in_w;



                        break;







                    case 3: // 이전 프레임



                        button.Image = Properties.Resources.double_arrow_left_w;



                        break;







                    case 4: // 앞 주기



                        button.Image = Properties.Resources.arrow_left_w;



                        break;







                    case 5: // 뒤 주기



                        button.Image = Properties.Resources.arrow_right_w;



                        break;







                    case 6: // 뒤 프레임



                        button.Image = Properties.Resources.double_arrow_right_w;



                        break;







                    case 7: // 마우스 확대/축소



                        bool isOriginalSize = originalShowUnit > 0;



                        if (isOriginalSize)



                        {



                            button.Image = Properties.Resources.pinch_zoom_out_w;



                        }



                        else



                        {



                            button.Image = Properties.Resources.pinch_zoom_in_w;



                        }



                        break;







                    case 8: // 설정



                        button.Image = Properties.Resources.config_w;



                        break;



                }



            }



        }







        // 툴바 버튼 위치 업데이트 메서드



        private void UpdateToolbarPosition(Graphics g, int x1, int y1, int x2, int y2)



        {



            if (toolbarButtons == null || !toolbarVisible) return;







            // 그래프 크기 계산



            int graphWidth = x2 - x1;



            int graphHeight = y2 - y1;







            // 브러시 생성 - RunColorBack 사용



            Brush brushToolbar = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);







            double opticRateX, opticRateY; //20250317 PSU



            GetOpticRate(out opticRateX, out opticRateY);



            // 사용자 설정 버튼 크기 사용



            int buttonWidth = (int)(opticRateX * objArgs.nToolBarButtonSize);



            int buttonHeight = buttonWidth;







            // 버튼 크기를 그래프 크기에 비례하게 계산



            int minButtonSize = 2;  // 최소 버튼 크기



            int maxButtonSize = 64;  // 최대 버튼 크기







            buttonWidth = Math.Max(minButtonSize, Math.Min(maxButtonSize, buttonWidth));



            buttonHeight = buttonWidth;











            // 버튼 간격도 크기에 비례하게 조정



            int buttonGap = buttonWidth / 3;







            // 전체 툴바 크기 계산



            int totalWidth = toolbarButtons.Length * (buttonWidth + buttonGap) - buttonGap;



            int totalHeight = buttonHeight;



            int availableWidth = graphWidth;



            int availableHeight = graphHeight;







            int labelWidth = buttonWidth * 3;







            // 좌측/우측 툴바일 때 2열 배치를 위한 변수



            // int columns = 1;



            //int rows;







            if (objArgs.nToolBarPos == 0)   // Top



            {



                // 버튼 시작 위치 - 트렌드 그래프 바로 위



                int startX = x1 + 3;



                int startY = y1 - buttonHeight - 5; // 그래프 상단







                int padding = 2;







                if (totalWidth + labelWidth > graphWidth) labelWidth = buttonWidth * 3;



                else labelWidth = (int)(graphWidth - totalWidth) - buttonGap - padding - 2;











                if (IsBackBorder())



                {



                    if (objArgs.bHideLabelDataRange)



                        DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalWidth + padding, startY + buttonHeight + padding, brushToolbar);



                    else DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalWidth + labelWidth + buttonGap + padding, startY + buttonHeight + padding, brushToolbar);







                }







                // 버튼 위치 설정



                for (int i = 0; i < toolbarButtons.Length; i++)



                {



                    int x = startX + i * (buttonWidth + buttonGap);



                    // 크기가 변경되었을 때만 Size 속성 업데이트



                    if (toolbarButtons[i].Width != buttonWidth || toolbarButtons[i].Height != buttonHeight)



                        toolbarButtons[i].Size = new Size(buttonWidth, buttonHeight);



                    // 위치가 변경되었을 때만 Location 속성 업데이트



                    if (toolbarButtons[i].Left != x || toolbarButtons[i].Top != startY)



                        toolbarButtons[i].Location = new Point(x, startY);



                    toolbarButtons[i].Visible = toolbarVisible;







                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, x, startY, buttonWidth, buttonHeight);



                    }



                }



            }



            // 나머지 방향(Bottom, Left, Right)에 대한 코드도 유사하게 수정...



            else if (objArgs.nToolBarPos == 1) // Bottom



            {



                // 버튼 시작 위치 - 트렌드 그래프 바로 아래



                int startX = x1 + 3;



                int startY = y2 + 5; // 그래프 하단







                int padding = 2;







                if (totalWidth + labelWidth > graphWidth) labelWidth = buttonWidth * 3;



                else labelWidth = (int)(graphWidth - totalWidth) - buttonGap - padding - 2;







                if (IsBackBorder())



                {



                    if (objArgs.bHideLabelDataRange)



                        DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalWidth + padding, startY + buttonHeight + padding, brushToolbar);



                    else DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalWidth + labelWidth + buttonGap + padding, startY + buttonHeight + padding, brushToolbar);



                }











                // 버튼 위치 설정



                for (int i = 0; i < toolbarButtons.Length; i++)



                {



                    int x = startX + i * (buttonWidth + buttonGap);



                    // 크기가 변경되었을 때만 Size 속성 업데이트



                    if (toolbarButtons[i].Width != buttonWidth || toolbarButtons[i].Height != buttonHeight)



                        toolbarButtons[i].Size = new Size(buttonWidth, buttonHeight);



                    // 위치가 변경되었을 때만 Location 속성 업데이트



                    if (toolbarButtons[i].Left != x || toolbarButtons[i].Top != startY)



                        toolbarButtons[i].Location = new Point(x, startY);



                    toolbarButtons[i].Visible = toolbarVisible;







                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, x, startY, buttonWidth, buttonHeight);



                    }



                }



            }











            else if (objArgs.nToolBarPos == 2 || objArgs.nToolBarPos == 3) // Left 또는 Right



            {



                // 세로 방향일 때 버튼 크기 계산



                buttonWidth = buttonHeight;



                buttonGap = buttonHeight / 3;







                // 2열 배치 설정



                int columns = 2;



                int rows = (int)Math.Ceiling(toolbarButtons.Length / (double)columns);







                // 시작 위치 계산



                int startX = (objArgs.nToolBarPos == 2) ?



                    x1 - (buttonWidth * columns + buttonGap) - 6 : // Left



                    x2 + 8;                                      // Right



                int startY = y1 + 4;







                // 패널 크기 계산



                int panelWidth = buttonWidth * columns + buttonGap;



                int panelHeight = rows * buttonHeight + (rows - 1) * buttonGap;







                // 패널 배경 그리기



                int padding = 2;



                if (IsBackBorder())



                    DrawClass.PopBox2(g, startX - padding - 2, startY - 4,



                                     startX + panelWidth + 3, startY + panelHeight + padding, brushToolbar);







                // 왼쪽/오른쪽 위치일 때 버튼 순서 재정렬



                // 버튼 인덱스: 0=자동/수동, 1=축소, 2=확대, 3=이전프레임, 4=앞주기, 5=뒤주기, 6=다음프레임, 7=드래그확대, 8=설정



                int[] buttonOrder = null;







                // 설정 버튼이 있는 경우와 없는 경우를 구분



                if (!objArgs.bDontUseConfigDialog)



                {



                    // 설정 버튼 있음 (9개 버튼)



                    buttonOrder = new int[] {



                0,  // 자동/수동



                7,  // 드래그확대



                1,  // 축소



                2,  // 확대



                3,  // 이전프레임



                6,  // 다음프레임



                4,  // 앞주기



                5,  // 뒤주기



                8   // 설정



            };



                }



                else



                {



                    // 설정 버튼 없음 (8개 버튼)



                    buttonOrder = new int[] {



                0,  // 자동/수동



                7,  // 드래그확대



                1,  // 축소



                2,  // 확대



                3,  // 이전프레임



                6,  // 다음프레임



                4,  // 앞주기



                5   // 뒤주기



            };



                }







                // 버튼 위치 설정 (2열 그리드로 배치, 재정렬된 순서 적용)



                for (int i = 0; i < buttonOrder.Length; i++)



                {



                    int buttonIndex = buttonOrder[i];



                    int col = i % columns;



                    int row = i / columns;







                    int x = startX + col * (buttonWidth + buttonGap);



                    int y = startY + row * (buttonHeight + buttonGap);







                    // 크기가 변경되었을 때만 Size 속성 업데이트



                    if (toolbarButtons[buttonIndex].Width != buttonWidth || toolbarButtons[buttonIndex].Height != buttonHeight)



                        toolbarButtons[buttonIndex].Size = new Size(buttonWidth, buttonHeight);



                    // 위치가 변경되었을 때만 Location 속성 업데이트



                    if (toolbarButtons[buttonIndex].Left != x || toolbarButtons[buttonIndex].Top != y)



                        toolbarButtons[buttonIndex].Location = new Point(x, y);



                    toolbarButtons[buttonIndex].Visible = toolbarVisible;







                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, x, y, buttonWidth, buttonHeight);



                    }



                }



            }







            // 툴바 버튼 위치 설정 이후에 라벨 위치도 업데이트



            if (lblInfo != null)



            {



                int buttonSize = toolbarButtons[0].Width;



                int totalButtonWidth = toolbarButtons.Length * (buttonSize + buttonGap) - buttonGap;



                Font sizedFont = new Font(lblInfo.Font.FontFamily, (float)(objArgs.nToolBarTextSize * opticRateX));







                Font sizedFont2;



                // if (objArgs.nTooolBarBtnColor == 0)



                sizedFont2 = new Font(lblInfo.Font.FontFamily, (float)(objArgs.nToolBarTextSize * opticRateX));



                // else sizedFont2 = new Font(lblInfo.Font.FontFamily, (float)(objArgs.nToolBarTextSize * opticRateX));







                // 툴바 위치에 따라 라벨 위치 설정



                switch (objArgs.nToolBarPos)



                {



                    case 0: // Top



                        lblInfo.Size = new Size(labelWidth, buttonHeight);



                        lblInfo.Location = new Point(x1 + totalButtonWidth + buttonGap, toolbarButtons[0].Top);



                        lblInfo.Font = sizedFont;



                        break;







                    case 1: // Bottom



                        lblInfo.Size = new Size(labelWidth, buttonHeight);



                        lblInfo.Location = new Point(x1 + totalButtonWidth + buttonGap, toolbarButtons[0].Top);



                        lblInfo.Font = sizedFont;



                        break;







                    case 2: // Left



                        // 세로 배치일 때는 맨 아래쪽에 표시



                        lblInfo.Size = new Size(buttonWidth * 2 + buttonGap + 10, buttonHeight * 2 + 10);



                        if (!objArgs.bDontUseConfigDialog)



                        {



                            lblInfo.Location = new Point(toolbarButtons[0].Left - 5,



                                toolbarButtons[toolbarButtons.Length - 1].Top + buttonHeight + buttonGap);



                        }



                        else



                        {



                            lblInfo.Location = new Point(toolbarButtons[0].Left - 5,



                               toolbarButtons[5].Top + buttonHeight + buttonGap);



                        }







                        lblInfo.Font = sizedFont2;



                        break;



                    case 3: // Right



                        // 세로 배치일 때는 맨 아래쪽에 표시



                        lblInfo.Size = new Size(buttonWidth * 2 + buttonGap + 10, buttonHeight * 2 + 10);







                        if (!objArgs.bDontUseConfigDialog)



                        {



                            lblInfo.Location = new Point(toolbarButtons[0].Left - 5,



                                toolbarButtons[toolbarButtons.Length - 1].Top + buttonHeight + buttonGap);



                        }



                        else



                        {



                            lblInfo.Location = new Point(toolbarButtons[0].Left - 5,



                               toolbarButtons[5].Top + buttonHeight + buttonGap);



                        }



                        lblInfo.Font = sizedFont2;



                        break;



                }







                lblInfo.Visible = toolbarVisible;







                // 라벨에 테두리 그리기



                using (Pen borderPen = new Pen(Color.DarkGray, 1))



                {



                    g.DrawRectangle(borderPen, lblInfo.Left, lblInfo.Top, lblInfo.Width, lblInfo.Height);



                }



            }











        }







        // UpdateLabel 메서드 추가



        private void UpdateLabel()



        {



            if (objArgs.bHideLabelDataRange) return;







            int wShowUnit = objArgs.wShowUnit;



            int nDataCycle = objArgs.nDataCycle;



            int wTimeSelectOption = objArgs.wTimeSelectOption;



            int totalUnits = wShowUnit * nDataCycle;



            double seconds = totalUnits; // 기본적으로 초 기준







            switch (wTimeSelectOption)



            {



                case 0: // 밀리초



                    seconds = totalUnits / 1000.0;



                    break;



                case 1: // 초



                    seconds = totalUnits;



                    break;



                case 2: // 분



                    seconds = totalUnits * 60;



                    break;



                case 3: // 시간



                    seconds = totalUnits * 60 * 60;



                    break;



                case 4: // 일



                    seconds = totalUnits * 60 * 60 * 24;



                    break;



                case 5: // 월 (30일 기준)



                    seconds = totalUnits * 60 * 60 * 24 * 30;



                    break;



                case 6: // 년 (365일 기준)



                    seconds = totalUnits * 60 * 60 * 24 * 365;



                    break;



            }







            string formattedTime;



            bool isKorean = Tools.IsLangKorean();







            if (seconds >= 86400) // 1일 이상



            {



                double days = seconds / 86400;



                formattedTime = isKorean



                    ? string.Format("{0:F1}일", days)



                    : string.Format("{0:F1}d", days);



            }



            else if (seconds >= 3600) // 1시간 이상



            {



                double hours = seconds / 3600;



                formattedTime = isKorean



                    ? string.Format("{0:F1}시간", hours)



                    : string.Format("{0:F1}h", hours);



            }



            else if (seconds >= 60) // 1분 이상



            {



                double minutes = seconds / 60;



                formattedTime = isKorean



                    ? string.Format("{0:F1}분", minutes)



                    : string.Format("{0:F1}m", minutes);



            }



            else



            {



                formattedTime = isKorean



                    ? string.Format("{0:F1}초", seconds)



                    : string.Format("{0:F1}s", seconds);



            }







            if (lblInfo != null)



            {



                if (objArgs.nToolBarPos == 2 || objArgs.nToolBarPos == 3)



                {



                    lblInfo.Text = isKorean



                        ? string.Format("데이터범위\n{0}개\n({1})", wShowUnit, formattedTime)



                        : string.Format("DataRange\n{0}\n({1})", wShowUnit, formattedTime);



                }



                else



                {



                    lblInfo.Text = isKorean



                        ? string.Format("데이터범위 {0}개 ({1})", wShowUnit, formattedTime)



                        : string.Format("DataRange {0} ({1})", wShowUnit, formattedTime);



                }



            }



        }







        private readonly int[] meaningfulDataCounts = new int[] {



    10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 44640



};







        // 시간 단위별 의미 있는 데이터 개수 배열 정의



        private readonly Dictionary<int, int[]> timeBasedDataCounts = new Dictionary<int, int[]>() {



    // 밀리초 데이터 (0)



    {0, new int[] {10, 20, 50, 100, 200, 500, 600, 1000, 2000, 5000, 6000, 10000, 12000, 20000, 44640}},



    // 초 데이터 (1)



    {1, new int[] {10, 30, 60, 120, 300, 600, 900, 1800, 3600, 7200, 14400, 21600, 43200}},



    // 분 데이터 (2)



    {2, new int[] {10, 15, 30, 60, 90, 120, 180, 240, 360, 720,1440, 2880, 10080, 40320, 41760, 43200, 44640}},



    // 시간 데이터 (3)



    {3, new int[] {12, 24, 48, 72, 168, 336, 672, 696,  720 , 744}},



    // 일 데이터 (4)



    {4, new int[] {7, 14, 30, 60, 90, 180, 365}},



    // 월 데이터 (5)



    {5, new int[] {3, 6, 12, 24, 36, 60}},



    // 년 데이터 (6)



    {6, new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20}}



};











        // 시간 타입을 고려한 의미 있는 데이터 개수를 찾는 메서드 수정



        private int GetNextMeaningfulDataCount(int currentCount, bool increase)



        {



            // 시간 타입에 맞는 배열 가져오기



            int timeType = objArgs.wTimeSelectOption;



            int[] meaningfulCounts = timeBasedDataCounts.ContainsKey(timeType)



                ? timeBasedDataCounts[timeType]



                : meaningfulDataCounts; // 기본 배열로 폴백







            if (increase) // 축소 (더 많은 데이터 표시)



            {



                for (int i = 0; i < meaningfulCounts.Length; i++)



                {



                    if (meaningfulCounts[i] > currentCount)



                        return meaningfulCounts[i];



                }



                return meaningfulCounts[meaningfulCounts.Length - 1]; // 최대값



            }



            else // 확대 (더 적은 데이터 표시)



            {



                for (int i = meaningfulCounts.Length - 1; i >= 0; i--)



                {



                    if (meaningfulCounts[i] < currentCount)



                        return meaningfulCounts[i];



                }



                return meaningfulCounts[0]; // 최소값



            }



        }







        private void DrawToolbarPlaceholder(Graphics g, int x1, int y1, int x2, int y2)



        {



            // 그래픽 객체의 영역 가져오기



            // 그래프 크기 계산



            int graphWidth = x2 - x1;



            int graphHeight = y2 - y1;







            // 브러시 생성 - RunColorBack 사용



            Brush brushToolbar = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);







            double opticRateX, opticRateY; //20250317 PSU



            GetOpticRate(out opticRateX, out opticRateY);







            // 사용자 설정 버튼 크기 사용



            int buttonWidth = (int)(objArgs.nToolBarButtonSize * opticRateX);



            int buttonHeight = buttonWidth;







            int minButtonSize = 2;  // 최소 버튼 크기



            int maxButtonSize = 64;  // 최대 버튼 크기







            buttonWidth = Math.Max(minButtonSize, Math.Min(maxButtonSize, buttonWidth));



            buttonHeight = buttonWidth;



            // 높이는 너비와 동일하게 (정사각형 버튼)







            // 버튼 간격도 크기에 비례하게 조정



            int buttonGap = buttonWidth / 3;







            // 버튼 수 설정 (설정창 사용 여부에 따라 다름)



            int buttonLength = 9;



            if (objArgs.bDontUseConfigDialog) buttonLength = buttonLength - 1;







            if (objArgs.nToolBarPos == 0)   // Top



            {



                // 버튼 시작 위치 - 트렌드 그래프 바로 위



                int startX = x1 + 3;



                int startY = y1 - buttonHeight - 5; // 그래프 상단







                int padding = 2;







                int totalButtonWidth = buttonLength * (buttonWidth + buttonGap) - buttonGap;



                int labelWidth = buttonWidth * 3;



                if (totalButtonWidth + labelWidth > graphWidth) labelWidth = buttonWidth * 3;



                else labelWidth = (int)(graphWidth - totalButtonWidth) - buttonGap - padding - 2;







                if (IsBackBorder())



                {



                    if (objArgs.bHideLabelDataRange)



                        DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalButtonWidth + padding, startY + buttonHeight + padding, brushToolbar);



                    else DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalButtonWidth + labelWidth + buttonGap + padding, startY + buttonHeight + padding, brushToolbar);











                }







                // Top 위치에서 데이터 범위 라벨 추가



                if (!objArgs.bHideLabelDataRange)



                {



                    if (labelWidth > 0) //라벨공간이 없으면 그리지 않음.



                    {







                        int labelHeight = buttonHeight;



                        int labelX = startX + totalButtonWidth + buttonGap;



                        int labelY = startY;







                        // 라벨 배경 그리기



                        Color labelBackColor = objArgs.nTooolBarBtnColor == 0 ? Color.White : Color.Black;



                        Color labelTextColor = objArgs.nTooolBarBtnColor == 0 ? Color.Black : Color.White;







                        using (Brush fillBrush = new SolidBrush(labelBackColor))
                            g.FillRectangle(fillBrush, labelX, labelY, labelWidth, labelHeight);







                        // 라벨 테두리 그리기



                        using (Pen borderPen = new Pen(Color.DarkGray, 1))



                        {



                            g.DrawRectangle(borderPen, labelX, labelY, labelWidth, labelHeight);



                        }







                        // 라벨 텍스트 계산



                        int wShowUnit = objArgs.wShowUnit;



                        int nDataCycle = objArgs.nDataCycle;



                        int wTimeSelectOption = objArgs.wTimeSelectOption;



                        double seconds = wShowUnit * nDataCycle; // 기본적으로 초 기준







                        if (wTimeSelectOption == 0) // 밀리초



                            seconds = wShowUnit * nDataCycle / 1000.0;



                        else if (wTimeSelectOption == 2) // 분



                            seconds = wShowUnit * nDataCycle * 60;



                        else if (wTimeSelectOption == 3) // 시간



                            seconds = wShowUnit * nDataCycle * 60 * 60;



                        else if (wTimeSelectOption == 4) // 일



                            seconds = wShowUnit * nDataCycle * 60 * 60 * 24;







                        string formattedTime;



                        bool isKorean = Tools.IsLangKorean();







                        if (seconds >= 86400) // 1일 이상



                        {



                            double days = seconds / 86400;



                            formattedTime = isKorean



                                ? String.Format("{0:F1}일", days)



                                : String.Format("{0:F1}d", days);



                        }



                        else if (seconds >= 3600) // 1시간 이상



                        {



                            double hours = seconds / 3600;



                            formattedTime = isKorean



                                ? String.Format("{0:F1}시간", hours)



                                : String.Format("{0:F1}h", hours);



                        }



                        else if (seconds >= 60) // 1분 이상



                        {



                            double minutes = seconds / 60;



                            formattedTime = isKorean



                                ? String.Format("{0:F1}분", minutes)



                                : String.Format("{0:F1}m", minutes);



                        }



                        else



                        {



                            formattedTime = isKorean



                                ? String.Format("{0:F1}초", seconds)



                                : String.Format("{0:F1}s", seconds);



                        }







                        // 라벨 텍스트 표시



                        //Font labelFont = new Font(SystemFonts.DefaultFont.FontFamily, (float)(buttonHeight * 0.4));



                        Font labelFont = new Font(SystemFonts.DefaultFont.FontFamily, (float)(objArgs.nToolBarTextSize * opticRateX));



                        string labelText = isKorean



                            ? String.Format("데이터범위 {0}개 ({1})", wShowUnit, formattedTime)



                            : String.Format("DataRange {0} ({1})", wShowUnit, formattedTime);







                        using (Brush lblBrush = new SolidBrush(labelTextColor))
                        using (StringFormat lblFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                            g.DrawString(labelText, labelFont, lblBrush,
                            new Rectangle(labelX, labelY, labelWidth, labelHeight),
                            lblFormat);











                    }



                }







                // 버튼 위치에 이미지 표시



                for (int i = 0; i < buttonLength; i++)



                {



                    int x = startX + i * (buttonWidth + buttonGap);



                    Rectangle rect = new Rectangle(x, startY, buttonWidth, buttonHeight);







                    // 버튼 테두리 그리기



                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, rect);



                    }







                    // 버튼 이미지 그리기 - 컬러 옵션 추가



                    DrawButtonImage(g, i, rect);



                }



            }



            else if (objArgs.nToolBarPos == 1) // Bottom



            {



                // 버튼 시작 위치 - 트렌드 그래프 바로 아래



                int startX = x1 + 3;



                int startY = y2 + 5; // 그래프 하단







                int padding = 2;







                int totalButtonWidth = buttonLength * (buttonWidth + buttonGap) - buttonGap;



                int labelWidth = buttonWidth * 3;



                if (totalButtonWidth + labelWidth > graphWidth) labelWidth = buttonWidth * 3;



                else labelWidth = (int)(graphWidth - totalButtonWidth) - buttonGap - padding - 2;















                //if (IsBackBorder()) DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + graphWidth - 3, startY + buttonHeight + padding + 1, brushToolbar);



                if (IsBackBorder())



                {



                    if (objArgs.bHideLabelDataRange)



                        DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalButtonWidth + padding, startY + buttonHeight + padding + 1, brushToolbar);



                    else DrawClass.PopBox2(g, startX - padding - 1, startY - padding - 1, startX + totalButtonWidth + labelWidth + buttonGap + padding, startY + buttonHeight + padding, brushToolbar);



                }







                // Bottom 위치에서 데이터 범위 라벨 추가



                if (!objArgs.bHideLabelDataRange)



                {



                    if (labelWidth > 0)



                    {







                        int labelHeight = buttonHeight;



                        int labelX = startX + totalButtonWidth + buttonGap;



                        int labelY = startY;







                        // 라벨 배경 그리기



                        Color labelBackColor = objArgs.nTooolBarBtnColor == 0 ? Color.White : Color.Black;



                        Color labelTextColor = objArgs.nTooolBarBtnColor == 0 ? Color.Black : Color.White;







                        using (Brush fillBrush = new SolidBrush(labelBackColor))
                            g.FillRectangle(fillBrush, labelX, labelY, labelWidth, labelHeight);







                        // 라벨 테두리 그리기



                        using (Pen borderPen = new Pen(Color.DarkGray, 1))



                        {



                            g.DrawRectangle(borderPen, labelX, labelY, labelWidth, labelHeight);



                        }







                        // 라벨 텍스트 계산



                        int wShowUnit = objArgs.wShowUnit;



                        int nDataCycle = objArgs.nDataCycle;



                        int wTimeSelectOption = objArgs.wTimeSelectOption;



                        double seconds = wShowUnit * nDataCycle; // 기본적으로 초 기준







                        if (wTimeSelectOption == 0) // 밀리초



                            seconds = wShowUnit * nDataCycle / 1000.0;



                        else if (wTimeSelectOption == 2) // 분



                            seconds = wShowUnit * nDataCycle * 60;



                        else if (wTimeSelectOption == 3) // 시간



                            seconds = wShowUnit * nDataCycle * 60 * 60;



                        else if (wTimeSelectOption == 4) // 일



                            seconds = wShowUnit * nDataCycle * 60 * 60 * 24;







                        string formattedTime;



                        bool isKorean = Tools.IsLangKorean();







                        if (seconds >= 86400) // 1일 이상



                        {



                            double days = seconds / 86400;



                            formattedTime = isKorean



                                ? String.Format("{0:F1}일", days)



                                : String.Format("{0:F1}d", days);



                        }



                        else if (seconds >= 3600) // 1시간 이상



                        {



                            double hours = seconds / 3600;



                            formattedTime = isKorean



                                ? String.Format("{0:F1}시간", hours)



                                : String.Format("{0:F1}h", hours);



                        }



                        else if (seconds >= 60) // 1분 이상



                        {



                            double minutes = seconds / 60;



                            formattedTime = isKorean



                                ? String.Format("{0:F1}분", minutes)



                                : String.Format("{0:F1}m", minutes);



                        }



                        else



                        {



                            formattedTime = isKorean



                                ? String.Format("{0:F1}초", seconds)



                                : String.Format("{0:F1}s", seconds);



                        }







                        // 라벨 텍스트 표시



                        //Font labelFont = new Font(SystemFonts.DefaultFont.FontFamily, (float)(buttonHeight * 0.4));



                        Font labelFont = new Font(SystemFonts.DefaultFont.FontFamily, (float)(objArgs.nToolBarTextSize * opticRateX));



                        string labelText = isKorean



                            ? String.Format("데이터범위 {0}개 ({1})", wShowUnit, formattedTime)



                            : String.Format("DataRange {0} ({1})", wShowUnit, formattedTime);







                        using (Brush lblBrush = new SolidBrush(labelTextColor))
                        using (StringFormat lblFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                            g.DrawString(labelText, labelFont, lblBrush,
                            new Rectangle(labelX, labelY, labelWidth, labelHeight),
                            lblFormat);











                    }



                }







                // 버튼 위치에 이미지 표시



                for (int i = 0; i < buttonLength; i++)



                {



                    int x = startX + i * (buttonWidth + buttonGap);



                    Rectangle rect = new Rectangle(x, startY, buttonWidth, buttonHeight);







                    // 버튼 테두리 그리기



                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, rect);



                    }







                    // 버튼 이미지 그리기



                    DrawButtonImage(g, i, rect);



                }



            }



            else if (objArgs.nToolBarPos == 2 || objArgs.nToolBarPos == 3) // Left 또는 Right



            {



                // 세로 방향일 때 버튼 크기 계산



                buttonGap = buttonHeight / 3;







                // 2열 배치 설정



                int columns = 2;



                int rows = (int)Math.Ceiling(buttonLength / (double)columns);







                // 시작 위치 계산



                int startX = (objArgs.nToolBarPos == 2) ?



                    x1 - (buttonWidth * columns + buttonGap) - 6 : // Left



                    x2 + 8;                                         // Right



                int startY = y1 + 4;







                // 패널 크기 계산



                int panelWidth = buttonWidth * columns + buttonGap;



                int panelHeight = rows * buttonHeight + (rows - 1) * buttonGap;







                // 패널 배경 그리기



                int padding = 2;



                if (IsBackBorder())



                    DrawClass.PopBox2(g, startX - padding - 2, startY - 4,



                                     startX + panelWidth + 3, startY + panelHeight + padding, brushToolbar);







                // 왼쪽/오른쪽 위치일 때 버튼 순서 재정렬



                int[] buttonOrder = null;







                // 설정 버튼이 있는 경우와 없는 경우를 구분



                if (!objArgs.bDontUseConfigDialog)



                {



                    // 설정 버튼 있음 (9개 버튼)



                    buttonOrder = new int[] {



                0,  // 자동/수동



                7,  // 드래그확대



                1,  // 축소



                2,  // 확대



                3,  // 이전프레임



                6,  // 다음프레임



                4,  // 앞주기



                5,  // 뒤주기



                8   // 설정



            };



                }



                else



                {



                    // 설정 버튼 없음 (8개 버튼)



                    buttonOrder = new int[] {



                0,  // 자동/수동



                7,  // 드래그확대



                1,  // 축소



                2,  // 확대



                3,  // 이전프레임



                6,  // 다음프레임



                4,  // 앞주기



                5   // 뒤주기



            };



                }







                // 사용할 버튼 순서 배열 길이 결정 (buttonLength와 buttonOrder.Length 중 작은 값)



                int orderLength = Math.Min(buttonLength, buttonOrder.Length);







                // 버튼 위치에 이미지 표시 (2열 그리드로 배치, 재정렬된 순서 적용)



                for (int i = 0; i < orderLength; i++)



                {



                    int buttonIndex = buttonOrder[i];



                    int col = i % columns;



                    int row = i / columns;







                    int x = startX + col * (buttonWidth + buttonGap);



                    int y = startY + row * (buttonHeight + buttonGap);







                    Rectangle rect = new Rectangle(x, y, buttonWidth, buttonHeight);







                    // 버튼 테두리 그리기



                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, rect);



                    }







                    // 먼저 흰색 배경을 그립니다



                    g.FillRectangle(Brushes.White, rect);







                    // 그 위에 이미지를 그립니다



                    DrawButtonImage(g, buttonIndex, rect);



                }







                // 데이터 범위 라벨 추가 (UpdateToolbarPosition과 동일하게)



                if (!objArgs.bHideLabelDataRange)



                {



                    int labelWidth, labelHeight;







                    // 세로 배치일 때는 버튼 너비의 2배 + 간격만큼 너비 설정



                    labelWidth = buttonWidth * 2 + buttonGap + 10;



                    labelHeight = buttonHeight * 2 + 10;







                    int labelX = startX - 5;



                    int labelY = startY + rows * (buttonHeight + buttonGap);







                    // 라벨 배경 그리기



                    Color labelBackColor = objArgs.nTooolBarBtnColor == 0 ? Color.White : Color.Black;



                    Color labelTextColor = objArgs.nTooolBarBtnColor == 0 ? Color.Black : Color.White;







                    Rectangle labelRect = new Rectangle(labelX, labelY, labelWidth, labelHeight);







                    // 라벨 배경 그리기



                    using (Brush fillBrush = new SolidBrush(labelBackColor))
                        g.FillRectangle(fillBrush, labelRect);







                    // 라벨 테두리 그리기



                    using (Pen borderPen = new Pen(Color.DarkGray, 1))



                    {



                        g.DrawRectangle(borderPen, labelRect);



                    }







                    // 라벨 텍스트 계산



                    int wShowUnit = objArgs.wShowUnit;



                    int nDataCycle = objArgs.nDataCycle;



                    int wTimeSelectOption = objArgs.wTimeSelectOption;



                    double seconds = wShowUnit * nDataCycle; // 기본적으로 초 기준







                    if (wTimeSelectOption == 0) // 밀리초



                        seconds = wShowUnit * nDataCycle / 1000.0;



                    else if (wTimeSelectOption == 2) // 분



                        seconds = wShowUnit * nDataCycle * 60;



                    else if (wTimeSelectOption == 3) // 시간



                        seconds = wShowUnit * nDataCycle * 60 * 60;



                    else if (wTimeSelectOption == 4) // 일



                        seconds = wShowUnit * nDataCycle * 60 * 60 * 24;







                    string formattedTime;



                    bool isKorean = Tools.IsLangKorean();







                    if (seconds >= 86400) // 1일 이상



                    {



                        double days = seconds / 86400;



                        formattedTime = isKorean



                            ? String.Format("{0:F1}일", days)



                            : String.Format("{0:F1}d", days);



                    }



                    else if (seconds >= 3600) // 1시간 이상



                    {



                        double hours = seconds / 3600;



                        formattedTime = isKorean



                            ? String.Format("{0:F1}시간", hours)



                            : String.Format("{0:F1}h", hours);



                    }



                    else if (seconds >= 60) // 1분 이상



                    {



                        double minutes = seconds / 60;



                        formattedTime = isKorean



                            ? String.Format("{0:F1}분", minutes)



                            : String.Format("{0:F1}m", minutes);



                    }



                    else



                    {



                        formattedTime = isKorean



                            ? String.Format("{0:F1}초", seconds)



                            : String.Format("{0:F1}s", seconds);



                    }







                    // 라벨 텍스트 표시



                    Font labelFont = new Font(SystemFonts.DefaultFont.FontFamily, (float)(objArgs.nToolBarTextSize * opticRateX));



                    string labelText = isKorean



                        ? String.Format("데이터범위\n{0}개\n({1})", wShowUnit, formattedTime)



                        : String.Format("DataRange\n{0}\n({1})", wShowUnit, formattedTime);











                    using (Brush lblBrush = new SolidBrush(labelTextColor))
                    using (StringFormat lblFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        g.DrawString(labelText, labelFont, lblBrush,
                        new Rectangle(labelX + 5, labelY + 5, labelWidth - 10, labelHeight - 10),
                        lblFormat);











                }



            }



        }







        // DrawButtonImage 도우미 메서드 추가



        private void DrawButtonImage(Graphics g, int index, Rectangle rect)



        {



            Image buttonImage = null;







            if (objArgs.nTooolBarBtnColor == 0)



            {



                switch (index)



                {



                    case 0: // 트렌드 모드



                        buttonImage = objArgs.bAutoUpdate ?



                            Properties.Resources.stop : Properties.Resources.start;



                        break;



                    case 1: // 축소



                        buttonImage = Properties.Resources.zoom_out;



                        break;



                    case 2: // 확대



                        buttonImage = Properties.Resources.zoom_in;



                        break;







                    case 3: // 이전 프레임



                        buttonImage = Properties.Resources.double_arrow_left;



                        break;



                    case 4: // 앞 주기



                        buttonImage = Properties.Resources.arrow_left;



                        break;



                    case 5: // 뒤 주기



                        buttonImage = Properties.Resources.arrow_right;



                        break;



                    case 6: // 뒤 프레임



                        buttonImage = Properties.Resources.double_arrow_right;



                        break;







                    case 7: // 마우스 확대/축소



                        buttonImage = originalShowUnit > 0 ?



                            Properties.Resources.pinch_zoom_out : Properties.Resources.pinch_zoom_in;



                        break;



                    case 8: // 설정



                        buttonImage = Properties.Resources.config;



                        break;



                }







                if (buttonImage != null)



                {



                    // 먼저 흰색 배경을 그립니다



                    g.FillRectangle(Brushes.White, rect);



                    // 그 위에 이미지를 그립니다



                    g.DrawImage(buttonImage, rect);



                }



            }



            else



            {



                switch (index)



                {



                    case 0: // 트렌드 모드



                        buttonImage = objArgs.bAutoUpdate ?



                            Properties.Resources.stop_w : Properties.Resources.start_w;



                        break;



                    case 1: // 축소



                        buttonImage = Properties.Resources.zoom_out_w;



                        break;



                    case 2: // 확대



                        buttonImage = Properties.Resources.zoom_in_w;



                        break;







                    case 3: // 이전 프레임



                        buttonImage = Properties.Resources.double_arrow_left_w;



                        break;



                    case 4: // 앞 주기



                        buttonImage = Properties.Resources.arrow_left_w;



                        break;



                    case 5: // 뒤 주기



                        buttonImage = Properties.Resources.arrow_right_w;



                        break;



                    case 6: // 뒤 프레임



                        buttonImage = Properties.Resources.double_arrow_right_w;



                        break;







                    case 7: // 마우스 확대/축소



                        buttonImage = originalShowUnit > 0 ?



                            Properties.Resources.pinch_zoom_out : Properties.Resources.pinch_zoom_in_w;



                        break;



                    case 8: // 설정



                        buttonImage = Properties.Resources.config_w;



                        break;



                }







                if (buttonImage != null)



                {



                    // 먼저 흰색 배경을 그립니다



                    g.FillRectangle(Brushes.Black, rect);



                    // 그 위에 이미지를 그립니다



                    g.DrawImage(buttonImage, rect);



                }



            }



        }







        private bool isProcessing = false; // 처리중 플래그







        // 버튼 클릭 이벤트 핸들러



        private async Task ToolbarButton_Click(object sender, EventArgs e)



        {



            // 이미 처리 중이면 무시



            if (isProcessing) return;







            try



            {



                isProcessing = true;







                PictureBox pb = sender as PictureBox;



                if (pb == null) return;







                int index = (int)pb.Tag;







                switch (index)



                {







                    case 0: // 트렌드 모드 변경



                        if (objArgs.bAutoUpdate)



                        {



                            await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 0);



                        }



                        else



                        {



                            await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 1);



                        }



                        SetButtonImage(pb, 0); // 이미지 업데이트



                        break;



                    case 1: // 축소



                        int currentShowUnit1 = objArgs.wShowUnit;



                        //int newShowUnit1 = Math.Min(currentShowUnit1 * 2, 44640); // 최대 44640



                        if (currentShowUnit1 >= 44640) return;



                        int newShowUnit1 = GetNextMeaningfulDataCount(currentShowUnit1, true);



                        await ExecuteClassName(false, "MilliTrendSetShowSize", null, newShowUnit1);



                        await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                        ResetCursorPosition(); // 커서 위치 초기화



                        UpdateLabel();



                        break;







                    case 2: // 확대



                        int currentShowUnit2 = objArgs.wShowUnit;



                        // int newShowUnit2 = Math.Max(currentShowUnit2 / 2, 10); // 최소 10



                        int newShowUnit2 = GetNextMeaningfulDataCount(currentShowUnit2, false);



                        await ExecuteClassName(false, "MilliTrendSetShowSize", null, newShowUnit2);



                        await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                        ResetCursorPosition(); // 커서 위치 초기화



                        UpdateLabel();



                        break;















                    case 3: // 이전 프레임



                        await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 0);



                        SetButtonImage(toolbarButtons[0], 0); // 트렌드 모드 버튼 이미지 업데이트



                        await ExecuteClassName(false, "MilliTrendShiftTime", null, -objArgs.wShowUnit);



                        await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                        break;







                    case 4: // 앞 주기



                        await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 0);



                        SetButtonImage(toolbarButtons[0], 0); // 트렌드 모드 버튼 이미지 업데이트



                        //int dataCycle = objArgs.n;



                        await ExecuteClassName(false, "MilliTrendShiftTime", null, -nMovePeriod);//-10 * dataCycle



                        await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                        break;







                    case 5: // 뒤 주기



                        await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 0);



                        SetButtonImage(toolbarButtons[0], 0); // 트렌드 모드 버튼 이미지 업데이트



                        //int dataCycle2 = objArgs.nDataCycle;



                        await ExecuteClassName(false, "MilliTrendShiftTime", null, nMovePeriod);



                        await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                        break;







                    case 6: // 뒤 프레임



                        await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 0);



                        SetButtonImage(toolbarButtons[0], 0); // 트렌드 모드 버튼 이미지 업데이트



                        await ExecuteClassName(false, "MilliTrendShiftTime", null, objArgs.wShowUnit);



                        await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                        break;







                    case 7: // 마우스 확대/축소



                        await ExecuteClassName(false, "MilliTrendSetAutoUpdate", null, 0);



                        SetButtonImage(pb, 7); // 이미지 업데이트



                        int cursorSize = (int)await ExecuteClassName(false, "MilliTrendGetCursorSize", null);



                        if (cursorSize > 1 && originalShowUnit == 0)



                        {



                            // 확대 모드



                            originalShowUnit = objArgs.wShowUnit;



                            originalStartTime = dtStartTime; // 현재 시작 시간 저장



                            originalCursorX1 = nCursorX1;



                            originalCursorX2 = nCursorX2;







                            // 커서 시간 가져오기



                            int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;



                            object[] timeParams = new object[] { null, year, month, day, hour, minute, second };



                            await ExecuteClassName(false, "MilliTrendGetCursorTime", timeParams);



                            // 참조 매개변수로부터 값 읽기



                            year = (int)timeParams[1];



                            month = (int)timeParams[2];



                            day = (int)timeParams[3];



                            hour = (int)timeParams[4];



                            minute = (int)timeParams[5];



                            second = (int)timeParams[6];







                            // 시작 시간 설정



                            await ExecuteClassName(false, "MilliTrendSetStartTime", null, year, month, day, hour, minute, second);



                            await ExecuteClassName(false, "MilliTrendSetShowSize", null, cursorSize);







                            await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                            ResetCursorPosition(); // 커서 위치 초기화







                        }



                        else if (originalShowUnit > 0)



                        {



                            // 원래 크기로 복원



                            await ExecuteClassName(false, "MilliTrendSetShowSize", null, originalShowUnit);



                            // 원래 시작 시간으로 복원



                            await ExecuteClassName(false, "MilliTrendSetStartTime", null,



                                originalStartTime.Year, originalStartTime.Month, originalStartTime.Day,



                                originalStartTime.Hour, originalStartTime.Minute, originalStartTime.Second);



                            await ExecuteClassName(false, "MilliTrendReLoad", null, 0);



                            originalShowUnit = 0;



                            //ResetCursorPosition(); // 커서 위치 초기화



                            nCursorX1 = originalCursorX1; //커서 원위치



                            nCursorX2 = originalCursorX2;



                        }



                        SetButtonImage(pb, 7); // 이미지 업데이트



                        UpdateLabel();



                        break;



                    case 8: // 설정



                        if (objArgs.bDontUseConfigDialog) return;







                        CONFIG_DATABASE_TREND config = LoadTrendConfig();



                        ConfigDatabaseTrend dialog = new ConfigDatabaseTrend(objGeneral.sClassName, config);







                        if (Tools.IsLangKorean())



                        {



                            dialog.Text = "미세자료 트랜드 설정";



                        }



                        else



                        {



                            dialog.Text = "MilliData Trend Config";



                        }







                        if (dialog.ShowDialog() == DialogResult.OK)



                        {



                            SaveMultiTrendConfig(config);



                            LoadConfigAndExcute();



                            await ReadAllPoint();



                            InvalidateObject(formParent);



                        }



                        break;



                }







                // 화면 갱신



                InvalidateObject(formParent);



            }



            finally



            {



                // 항상 플래그 해제



                isProcessing = false;



                //SetToolbarEnabled(true);



                //this.Cursor = Cursors.Default;



            }



        }







        //private void SetToolbarEnabled(bool enabled)



        //{



        //    foreach (var button in toolbarButtons)



        //    {



        //        button.Enabled = enabled;



        //    }



        //}











        // 툴바 표시/숨김 설정 메서드 추가



        public void SetToolbarVisible(bool visible)



        {



            toolbarVisible = visible;



            if (toolbarButtons != null)



            {



                foreach (PictureBox btn in toolbarButtons)



                {



                    btn.Visible = visible;



                }



            }



        }







        public override void OnVisible(bool flag) //20250407 PSU 추가



        {



            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)



            {



                this.toolbarVisible = flag;







                SetToolbarVisible(flag);



                if (lblInfo != null) lblInfo.Visible = flag;



            }



        }







        // 커서 위치 초기화 메서드



        private void ResetCursorPosition()



        {



            // 커서 위치를 그래프 중앙으로 초기화



            nCursorX1 = objArgs.wShowUnit / 2;



            nCursorX2 = nCursorX1;



        }



    }







    /// <summary>



    /// MilliDataTrendReader - PostgreSQL에서 MilliData를 읽어 DataSet으로 반환



    /// LocalMain WCF, PortalServer에서 호출하여 웹 클라이언트에 데이터 전달



    /// </summary>



    public class MilliDataTrendReader



    {



        int wShowUnit;



        int nDataCycle;



        int wTimeSelectOption;



        DateTime dtStartTime;







        public DataSet ReadData(string columns, string sDsn, DateTime start_time, int show_unit, int data_cycle, int time_select_option)



        {



            wShowUnit = show_unit;



            nDataCycle = data_cycle;



            wTimeSelectOption = time_select_option;



            dtStartTime = start_time;







            List<MD_TREND_MEMBER> blockMember = new List<MD_TREND_MEMBER>();







            CommaTextReader comma = new CommaTextReader();



            comma.Set(columns);







            while (true)



            {



                if (comma.IsEOS()) break;



                MD_TREND_MEMBER member = new MD_TREND_MEMBER();



                member.column = comma.GetString();



                member.point = new ONE_POINT_STRUCT[wShowUnit];



                blockMember.Add(member);



            }







            // PostgreSQL에서 데이터 읽기



            try



            {



                ReadFromPostgreSQL(blockMember, sDsn);



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"MilliDataTrendReader PostgreSQL 읽기 오류: {ex.Message}");



            }







            // DataSet으로 변환하여 반환



            DataSet ds = new DataSet();



            DataTable dt = new DataTable();







            DataColumn dc;



            dc = new DataColumn("Order", typeof(int));



            dt.Columns.Add(dc);



            for (int i = 0; i < blockMember.Count; i++)



            {



                dc = new DataColumn(blockMember[i].column, typeof(double));



                dt.Columns.Add(dc);



            }







            DataRow row;



            for (int i = 0; i < wShowUnit; i++)



            {



                row = dt.NewRow();



                row["Order"] = i;



                for (int c = 0; c < blockMember.Count; c++)



                {



                    if (blockMember[c].point[i].read_flag)



                        row[blockMember[c].column] = blockMember[c].point[i].val;



                }







                dt.Rows.Add(row);



            }







            ds.Tables.Add(dt);







            return ds;



        }







        /// <summary>



        /// PostgreSQL에서 MilliData 테이블을 찾아 데이터를 읽는다.



        /// </summary>



        void ReadFromPostgreSQL(List<MD_TREND_MEMBER> blockMember, string sDsn)



        {



            string milliDataTitle = sDsn;







            DateTime startTime = dtStartTime;



            DateTime endTime = CalculateEndTime(startTime);







            // 해당 기간의 테이블 찾기



            List<string> tableNames = FindMilliDataTables(milliDataTitle, startTime, endTime);







            if (tableNames.Count == 0)



            {



                Debug.WriteLine($"해당 기간의 MilliData 테이블을 찾을 수 없습니다: {milliDataTitle}");



                return;



            }







            // 각 멤버의 컬럼 위치 초기화



            for (int j = 0; j < blockMember.Count; j++)



            {



                blockMember[j].column_pos = -1;



            }







            // 테이블별로 데이터 읽기



            foreach (string tableName in tableNames)



            {



                ReadDataFromTable(blockMember, tableName, startTime, endTime);



            }



        }







        DateTime CalculateEndTime(DateTime startTime)



        {



            int data_cycle = wShowUnit * nDataCycle;







            switch (wTimeSelectOption)



            {



                case 0: return startTime.AddMilliseconds(data_cycle);



                case 1: return startTime.AddSeconds(data_cycle);



                case 2: return startTime.AddMinutes(data_cycle);



                case 3: return startTime.AddHours(data_cycle);



                case 4: return startTime.AddDays(data_cycle);



                case 5: return startTime.AddMonths(data_cycle);



                case 6: return startTime.AddYears(data_cycle);



                default: return startTime.AddHours(data_cycle);



            }



        }







        List<string> FindMilliDataTables(string milliDataTitle, DateTime startTime, DateTime endTime)



        {



            List<string> tables = new List<string>();







            try



            {



                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))



                {



                    connection.Open();







                    string query = @"



                SELECT table_name, start_time, time_interval, cut_method, size_cut



                FROM history.millidata_metadata



                WHERE title = @title



                AND EXISTS (



                    SELECT 1 FROM information_schema.tables



                    WHERE table_schema = 'history'



                    AND table_name = millidata_metadata.table_name



                )



                ORDER BY start_time DESC";







                    using (var command = new NpgsqlCommand(query, connection))



                    {



                        command.Parameters.AddWithValue("title", milliDataTitle);







                        using (var reader = command.ExecuteReader())



                        {



                            while (reader.Read())



                            {



                                string tableName = reader.GetString(0);



                                DateTime tableStartTimeUtc = reader.GetDateTime(1);



                                int cutMethod = reader.GetInt32(3);



                                int sizeCut = reader.GetInt32(4);







                                DateTime tableStartTime = tableStartTimeUtc.ToLocalTime();



                                DateTime tableEndTime = CalculateTableEndTime(tableStartTime, cutMethod, sizeCut);







                                if (tableEndTime >= startTime && tableStartTime <= endTime)



                                {



                                    tables.Add(tableName);



                                }



                            }



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"MilliDataTrendReader 테이블 검색 오류: {ex.Message}");



            }







            return tables;



        }







        DateTime CalculateTableEndTime(DateTime tableStartTime, int cutMethod, int sizeCut)



        {



            switch (cutMethod)



            {



                case 0: return tableStartTime.AddSeconds(sizeCut);



                case 1: return tableStartTime.AddMinutes(sizeCut);



                case 2: return tableStartTime.AddHours(sizeCut);



                case 3: return tableStartTime.AddDays(sizeCut);



                case 4: return tableStartTime.AddDays(7);



                case 5: return tableStartTime.AddMonths(sizeCut);



                case 6: return tableStartTime.AddYears(sizeCut);



                default: return tableStartTime.AddHours(1);



            }



        }







        void ReadDataFromTable(List<MD_TREND_MEMBER> blockMember, string tableName, DateTime startTime, DateTime endTime)



        {



            try



            {



                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))



                {



                    connection.Open();







                    // 컬럼 매핑 조회



                    Dictionary<string, string> columnMapping = GetColumnMapping(connection, tableName);







                    // Local Time을 UTC로 변환



                    DateTime startTimeUtc = startTime.ToUniversalTime();



                    DateTime endTimeUtc = endTime.ToUniversalTime();







                    string query = $@"



                SELECT data_time, interval_ms, *



                FROM history.""{tableName}""



                WHERE data_time >= @startTime



                AND data_time <= @endTime



                ORDER BY data_time, created_at";







                    using (var command = new NpgsqlCommand(query, connection))



                    {



                        command.Parameters.AddWithValue("startTime", startTimeUtc);



                        command.Parameters.AddWithValue("endTime", endTimeUtc);



                        command.CommandTimeout = 60;







                        using (var reader = command.ExecuteReader())



                        {



                            ProcessDataRows(blockMember, reader, columnMapping);



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"MilliDataTrendReader 테이블 데이터 읽기 오류 ({tableName}): {ex.Message}");



            }



        }







        Dictionary<string, string> GetColumnMapping(NpgsqlConnection connection, string tableName)



        {



            Dictionary<string, string> mapping = new Dictionary<string, string>();







            try



            {



                string query = @"



            SELECT tag_name, column_name



            FROM history.millidata_tag_metadata



            WHERE table_name = @tableName";







                using (var command = new NpgsqlCommand(query, connection))



                {



                    command.Parameters.AddWithValue("tableName", tableName);







                    using (var reader = command.ExecuteReader())



                    {



                        while (reader.Read())



                        {



                            string tagName = reader.GetString(0);



                            string columnName = reader.GetString(1);



                            mapping[tagName] = columnName;



                        }



                    }



                }



            }



            catch (Exception ex)



            {



                Debug.WriteLine($"MilliDataTrendReader 컬럼 매핑 조회 오류: {ex.Message}");



            }







            return mapping;



        }







        void ProcessDataRows(List<MD_TREND_MEMBER> blockMember, NpgsqlDataReader reader, Dictionary<string, string> columnMapping)



        {



            DateTime currentTime = dtStartTime;



            int currentIndex = 0;







            while (reader.Read() && currentIndex < wShowUnit)



            {



                try



                {



                    DateTime dataTimeUtc = reader.GetDateTime(reader.GetOrdinal("data_time"));



                    DateTime dataTime = dataTimeUtc.ToLocalTime();







                    int intervalMs = nDataCycle;



                    try



                    {



                        intervalMs = reader.GetInt32(reader.GetOrdinal("interval_ms"));



                    }



                    catch { }







                    while (currentIndex < wShowUnit)



                    {



                        if (IsTimeMatch(currentTime, dataTime, intervalMs))



                        {



                            SaveDataToMembers(blockMember, reader, columnMapping, currentIndex);



                            currentTime = IncrementTime(currentTime);



                            currentIndex++;



                            break;



                        }



                        else if (dataTime > currentTime.Add(GetTimeToleranceSpan(intervalMs)))



                        {



                            currentTime = IncrementTime(currentTime);



                            currentIndex++;



                        }



                        else



                        {



                            break;



                        }



                    }



                }



                catch (Exception ex)



                {



                    Debug.WriteLine($"MilliDataTrendReader 데이터 행 처리 오류: {ex.Message}");



                    continue;



                }



            }



        }







        void SaveDataToMembers(List<MD_TREND_MEMBER> blockMember, NpgsqlDataReader reader, Dictionary<string, string> columnMapping, int index)



        {



            for (int j = 0; j < blockMember.Count; j++)



            {



                MD_TREND_MEMBER member = blockMember[j];







                try



                {



                    string columnName = GetColumnNameForMember(member, columnMapping);







                    if (!string.IsNullOrEmpty(columnName))



                    {



                        int columnIndex = reader.GetOrdinal(columnName);







                        if (!reader.IsDBNull(columnIndex))



                        {



                            member.point[index].read_flag = true;



                            member.point[index].val = Convert.ToSingle(reader.GetDouble(columnIndex));



                        }



                    }



                }



                catch (Exception ex)



                {



                    Debug.WriteLine($"MilliDataTrendReader 멤버 데이터 저장 오류 ({member.column}): {ex.Message}");



                }



            }



        }







        string GetColumnNameForMember(MD_TREND_MEMBER member, Dictionary<string, string> columnMapping)



        {



            // 1. tag 이름으로 매핑에서 찾기



            if (!string.IsNullOrEmpty(member.tag) && columnMapping.ContainsKey(member.tag))



            {



                return columnMapping[member.tag];



            }







            // 2. column 이름으로 매핑에서 찾기



            if (!string.IsNullOrEmpty(member.column) && columnMapping.ContainsKey(member.column))



            {



                return columnMapping[member.column];



            }







            // 3. column 이름을 직접 사용 (매핑 없이)



            if (!string.IsNullOrEmpty(member.column))



            {



                return member.column;



            }







            return null;



        }







        DateTime IncrementTime(DateTime time)



        {



            switch (wTimeSelectOption)



            {



                case 0: return time.AddMilliseconds(nDataCycle);



                case 1: return time.AddSeconds(nDataCycle);



                case 2: return time.AddMinutes(nDataCycle);



                case 3: return time.AddHours(nDataCycle);



                case 4: return time.AddDays(nDataCycle);



                case 5: return time.AddMonths(nDataCycle);



                case 6: return time.AddYears(nDataCycle);



                default: return time.AddSeconds(nDataCycle);



            }



        }







        bool IsTimeMatch(DateTime expectedTime, DateTime dataTime, int intervalMs)



        {



            switch (wTimeSelectOption)



            {



                case 0:



                    double toleranceMs = Math.Max(intervalMs * 0.5, 50);



                    return Math.Abs((dataTime - expectedTime).TotalMilliseconds) <= toleranceMs;



                case 1:



                    return Math.Abs((dataTime - expectedTime).TotalSeconds) <= intervalMs * 0.5;



                case 2:



                    return Math.Abs((dataTime - expectedTime).TotalMinutes) <= intervalMs * 0.5;



                case 3:



                    return Math.Abs((dataTime - expectedTime).TotalHours) <= intervalMs * 0.5;



                case 4:



                    return Math.Abs((dataTime - expectedTime).TotalDays) <= intervalMs * 0.5;



                case 5:



                    return dataTime.Year == expectedTime.Year &&



                           Math.Abs(dataTime.Month - expectedTime.Month) <= intervalMs * 0.5;



                case 6:



                    return Math.Abs(dataTime.Year - expectedTime.Year) <= intervalMs * 0.5;



                default:



                    return Math.Abs((dataTime - expectedTime).TotalSeconds) <= intervalMs * 0.5;



            }



        }







        TimeSpan GetTimeToleranceSpan(int intervalMs)



        {



            switch (wTimeSelectOption)



            {



                case 0: return TimeSpan.FromMilliseconds(intervalMs * 0.5);



                case 1: return TimeSpan.FromSeconds(intervalMs * 0.5);



                case 2: return TimeSpan.FromMinutes(intervalMs * 0.5);



                case 3: return TimeSpan.FromHours(intervalMs * 0.5);



                case 4: return TimeSpan.FromDays(intervalMs * 0.5);



                case 5: return TimeSpan.FromDays(intervalMs * 15);



                case 6: return TimeSpan.FromDays(intervalMs * 182);



                default: return TimeSpan.FromSeconds(intervalMs * 0.5);



            }



        }



    }



}







