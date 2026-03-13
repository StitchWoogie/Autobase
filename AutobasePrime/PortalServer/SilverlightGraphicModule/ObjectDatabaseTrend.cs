using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using AutoLibLocal;
using NetTools.OldDefine;
using System.IO;

namespace SilverlightGraphicModule
{
    public class ObjectArgsDatabaseTrend
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();

        public int wTimeDevide;
        public int wTimeSelectOption;

        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorGuideLine;
        public Color lColorText;
        public BrushPublic lColorFill = new BrushPublic();
        public int wPointSize;		// y 크기의 10/1000 만한 포인트 크기
        public int wShowUnit;
        public int wLevelDevide;

        // public EnumDisplayFlag wDisplayFlags;
        // public int nLevelDisplaySize = 7;

        public string sDsn;
        public string sTable;
        public string sColumnTime;			// 시간 컬럼
        public int nDataCycle;			    // 
        public string sColumnMilli;			// Milli Sec Column
        public int nDateColumnType = 0;		// 0 = Datetime, 1 = yyyymmddhhmmss
        public int nBasicSpaceLeft = 0;		// 왼쪽의 기본적인 준비 공간
        public int nBasicSpaceRight = 0;	// 오른쪽의 기본적인 준비 공간
    }

    public class DB_TREND_MEMBER : PUBLIC_GRAPH_MEMBER
    {
        public string column;

        public int nGraphType;	// 0 = Line, 1 = Bar,

        public string sTable;		// 각 멤버에서 사용하는 테이블
        public string sWhereString;
        public string sDescription;	// 개별적으로 사용할 설명.
    }

    public class ObjectDatabaseTrend : ObjectExpand
    {
        ObjectArgsDatabaseTrend objArgs;
        static public List<object> arrayClassList = new List<object>();

        ControlDatabaseTrend formChild;

        public ObjectDatabaseTrend(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsDatabaseTrend args, List<object> block)
            : base(ocp, rect, eid, lf, general)
        {
            enumObjectType = EnumObjectType.DatabaseTrend;
            objArgs = args;
            //formParent = form;

            TextColor = args.lColorText;
            BackColor = args.lColorBack;
            FillColor = args.lColorFill;

            //bAutoViewRange = bAutoViewRange;
            //dwFirstLevelDisplay = dwFirstLevelDisplay;

            //bDisplayPointDate = false;
            //bAutoViewRange = false;
            //bAutoGuideLine = false;
            //bUseLocalRange = false;

            //cGraphStartTimeMethod = 0;			// AUTO

            //SetPointSize(args.wPointSize);		// y 크기의 10/1000 만한 포인트 크기
            //dwFirstLevelDisplay = 0;			// 첫번째 Level디스프레이는 사용자가 바꿀 수 있다.
            //nDataGab = 1;

            if (objArgs.wLevelDevide < 2) objArgs.wLevelDevide = 2;
            if (objArgs.wLevelDevide > 100) objArgs.wLevelDevide = 100;
            if (objArgs.nDataCycle < 1) objArgs.nDataCycle = 1;

            //SetBlock(block);

            if (objArgs.wTimeDevide < 1) objArgs.wTimeDevide = 10;

            /*
            CalcStartTime();
            //Graphics g = CreateGraphics();
            CalcCharSize();
            nCursorX1 = objArgs.wShowUnit / 2;
            nCursorX2 = objArgs.wShowUnit / 2;

            LoadConfigAndExcute();
            LoadMultiTrendMemberConfig();

            ReadAllPoint();*/

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }

            ControlDatabaseTrend child = new ControlDatabaseTrend(objArgs.wShowUnit, block, objArgs.pub.wPointSize, objArgs.wTimeDevide, objArgs.wLevelDevide, DateTime.Now, objArgs.wTimeSelectOption, args.lColorBack, args.lColorFill, args.lColorGuideLine, args.pub.wDisplayFlags, false, 1, args);

            formChild = child;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "DbTrendReLoad")
            {
                formChild.ReadAllPoint();
            }
            else if (command == "DbTrendSetStartTime")
            {
                int limit_day = DateTime.DaysInMonth((int)args[0], (int)args[1]);
                int day = (int)args[2];
                if (day > limit_day)
                    day = limit_day;

                DateTime t = new DateTime((int)args[0], (int)args[1], day, (int)args[3], (int)args[4], 0);
                formChild.SetStartTime(t);
                formChild.ReadAllPoint();
            }
            else if (command == "DbTrendSetShowSize")
            {
                objArgs.wShowUnit = (int)args[0];
                if (objArgs.wShowUnit < 2) objArgs.wShowUnit = 2;
                formChild.SetShowUnit(objArgs.wShowUnit);
            }
            else if (command == "DbTrendGetShowSize")
            {
                return objArgs.wShowUnit;
            }
            else if (command == "DbTrendSetDataType")
            {
                objArgs.wTimeSelectOption = (int)args[0];
                objArgs.nDataCycle = (int)args[1];
            }
            else if (command == "DbTrendSetDsn")
            {
                objArgs.sDsn = (string)args[0];
            }
            else if (command == "DbTrendSetTable")
            {
                objArgs.sTable = (string)args[0];
            }
            else if (command == "DbTrendShiftTime")
            {
                DbTrendShiftTime((int)args[0]);
                formChild.UpdateGraph();
            }
            else if (command == "DbTrendGetMemberFlags")
            {
                return formChild.GetMemberFlags((int)args[0]);
            }
            else if (command == "DbTrendSetMemberFlags")
            {
                int retn = formChild.SetMemberFlags((int)args[0], (ushort)(int)args[1]);
                formChild.UpdateGraph();
                return retn;
            }
            else if (command == "DbTrendSetMemberTable")
            {
                int pos = (int)args[0];

                if (pos >= formChild.blockMember.Count) return 0;
                if (pos < 0) return 0;

                DB_TREND_MEMBER member = (DB_TREND_MEMBER)formChild.blockMember[pos];
                member.sTable = (string)args[1];
            }
            else if (command == "DbTrendClear")
            {
                formChild.MemberClear();
                /*
                this.blockMember.Clear();
                CalcAxisPosition();
                InvalidateObject(formParent);*/
            }
            else if (command == "DbTrendAddMember")
            {
                formChild.AddTag((PUBLIC_GRAPH_MEMBER)args[0]);
                /*
                DB_TREND_MEMBER member = (DB_TREND_MEMBER)args[0];
                CheckOneMember(member);
                blockMember.Add(member);
                CalcAxisPosition();
                InvalidateObject(formParent);*/
            }
            else if (command == "DbTrendRemoveAt")
            {
                int pos = (int)args[0];

                if (pos >= formChild.blockMember.Count) return 0;
                if (pos < 0) return 0;

                formChild.blockMember.RemoveAt(pos);
                //CalcAxisPosition();
                //InvalidateObject(formParent);
            }
            else if (command == "DbTrendSaveToCsv")
            {
                return SaveToCsv((string)args[0]);
            }
            else if (command == "DbTrendGetRealPos")
            {
                int left_right = (int)args[0];
                int pos = (int)args[1];
                int level = (int)args[2];

                for (int i = 0; i < formChild.blockMember.Count; i++)
                {
                    DB_TREND_MEMBER member = (DB_TREND_MEMBER)formChild.blockMember[i];
                    if (left_right != member.nAxisPosition) continue;
                    if (pos != member.nAxisCalcPos) continue;
                    if (level < member.nLevelFrom) continue;
                    if (level > member.nLevelTo) continue;
                    return i;
                }

                return -1;
            }
            else if (command == "DbTrendGetMin")
            {
                int pos = (int)args[0];

                if (pos >= formChild.blockMember.Count) return 0;
                if (pos < 0) return 0;

                DB_TREND_MEMBER member = (DB_TREND_MEMBER)formChild.blockMember[pos];
                return member.min_value;
            }
            else if (command == "DbTrendGetMax")
            {
                int pos = (int)args[0];

                if (pos >= formChild.blockMember.Count) return 0;
                if (pos < 0) return 0;

                DB_TREND_MEMBER member = (DB_TREND_MEMBER)formChild.blockMember[pos];
                return member.max_value;
            }
            else if (command == "DbTrendSetMin")
            {
                int pos = (int)args[0];

                if (pos >= formChild.blockMember.Count) return 0;
                if (pos < 0) return 0;

                DB_TREND_MEMBER member = (DB_TREND_MEMBER)formChild.blockMember[pos];
                member.min_value = (double)args[1];
                //InvalidateObject(formParent);
            }
            else if (command == "DbTrendSetMax")
            {
                int pos = (int)args[0];

                if (pos >= formChild.blockMember.Count) return 0;
                if (pos < 0) return 0;

                DB_TREND_MEMBER member = (DB_TREND_MEMBER)formChild.blockMember[pos];
                member.max_value = (double)args[1];
                //InvalidateObject(formParent);
            }
            else { }

            return 0;
        }

        int SaveToCsv(string filename)
        {
            /*
            TextWriter writer = new StreamWriter(filename, false, System.Text.Encoding.Default);
            if (writer == null) return 0;

            int l;
            DB_TREND_MEMBER member;

            writer.Write("시간,");

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (DB_TREND_MEMBER)blockMember[l];

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
                    member = (DB_TREND_MEMBER)blockMember[l];
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
            */
            return 1;
        }

        void DbTrendShiftTime(int shift)
        {
            int data_cycle = shift * objArgs.nDataCycle;
            DateTime t;

            if (objArgs.wTimeSelectOption == 0)
            {	// milli data
                t = formChild.dtStartTime.AddMilliseconds(data_cycle);
            }
            else if (objArgs.wTimeSelectOption == 1)
            {	// sec data
                t = formChild.dtStartTime.AddSeconds(data_cycle);
            }
            else if (objArgs.wTimeSelectOption == 2)
            {	// min data
                t = formChild.dtStartTime.AddMinutes(data_cycle);
            }
            else if (objArgs.wTimeSelectOption == 3)
            {	// hour data
                t = formChild.dtStartTime.AddHours(data_cycle);
            }
            else if (objArgs.wTimeSelectOption == 4)
            {	// day data
                t = formChild.dtStartTime.AddDays(data_cycle);
            }
            else if (objArgs.wTimeSelectOption == 5)
            {	// month data
                t = formChild.dtStartTime.AddMonths(data_cycle);
            }
            else
            {	// year data
                t = formChild.dtStartTime.AddYears(data_cycle);
            }

            formChild.SetStartTime(t);
        }
    }
}

/*

namespace GraphicModule
{
	public class GUIDE_DISPLAY 
	{
		public int from;
		public int to;
	}

	/// <summary>
	/// Summary description for ObjectMultiTrend.
	/// </summary>
	[Serializable]
	public class ObjectDatabaseTrend : ObjectExpand
	{
		ArrayList blockMember;
		
		int   nHapLeftDisplay;	// 왼쪽 눈금의 개수
		int   nHapRightDisplay;	// 왼쪽 눈금의 개수
		int   nCharWidth;
		int   nCharHeight;

		bool  bDisplayPointDate;	// 자료시점의 날짜를 표시해 준다.
		bool  bAutoViewRange;
		bool  bAutoGuideLine;
		bool  bUseLocalRange;

		sbyte cGraphStartTimeMethod;

		int   dwFirstLevelDisplay;
		int   nDataGab;
		int   nCursorX1;
		int   nCursorX2;
		bool  bMouseCapture;
		int	  nBarGraphCount = 0;
		bool  bShowToolTip;
        
		ObjectArgsDatabaseTrend objArgs;

		DateTime dtStartTime = DateTime.Now;
		
		[NonSerialized]
		Form formParent;

		// 등록된 클래스 리스트
		[NonSerialized]
		static public ArrayList arrayClassList = new ArrayList();
		
		ArrayList arrayGuideDisplay = new ArrayList();

		public ObjectArgsDatabaseTrend ObjectArgs 
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

		void SeekCalcPos(ArrayList array, DB_TREND_MEMBER seek)
		{
			DB_TREND_MEMBER member;

			int fit_pos = 0;

			for(int l = 0; l < array.Count; l++) 
			{
				member = (DB_TREND_MEMBER)array[l];
				
				// 영역에 걸린다.
				if(seek.nLevelFrom >= member.nLevelTo || seek.nLevelTo <= member.nLevelFrom) 
				{
				}
				else {
					fit_pos++;
				}
			}

			seek.nAxisCalcPos = fit_pos;

			array.Add(seek);
		}

		void AddGuideDisplay(int from, int to)
		{
			GUIDE_DISPLAY guide;
			
			for(int i = 0; i < arrayGuideDisplay.Count; i++) 
			{
				guide = (GUIDE_DISPLAY)arrayGuideDisplay[i];
	
				if(guide.from == from && guide.to == to)	return;
			}

			guide = new GUIDE_DISPLAY();
			guide.from = from;
			guide.to = to;
			arrayGuideDisplay.Add(guide);
		}

		void CalcAxisPosition()
		{
			int l;
			DB_TREND_MEMBER member;
			ArrayList arrayL = new ArrayList();
			ArrayList arrayR = new ArrayList();

			nBarGraphCount = 0;

			arrayGuideDisplay.Clear();

			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (DB_TREND_MEMBER)blockMember[l];

				AddGuideDisplay(member.nLevelFrom, member.nLevelTo);

				if(member.nAxisPosition == 1) 
				{	// left position
					SeekCalcPos(arrayL, member);
					
				}
				else if(member.nAxisPosition == 2) 
				{	// right position
					SeekCalcPos(arrayR, member);
				}
				else {}

				if(member.nGraphType == 1)	nBarGraphCount++;
			}

			nHapLeftDisplay = 0;
            
			for(l = 0; l < arrayL.Count; l++) 
			{
				member = (DB_TREND_MEMBER)arrayL[l];

				if(member.nAxisCalcPos > nHapLeftDisplay)	nHapLeftDisplay = member.nAxisCalcPos;
			}

			if(arrayL.Count > 0)	nHapLeftDisplay++;

			nHapRightDisplay = 0;
            
			for(l = 0; l < arrayR.Count; l++) 
			{
				member = (DB_TREND_MEMBER)arrayR[l];

				if(member.nAxisCalcPos > nHapRightDisplay)	nHapRightDisplay = member.nAxisCalcPos;
			}

			if(arrayR.Count > 0)	nHapRightDisplay++;

			// 기본 좌우 공간을 확보한다.
			if(nHapLeftDisplay < objArgs.nBasicSpaceLeft)	nHapLeftDisplay = objArgs.nBasicSpaceLeft;
			if(nHapRightDisplay < objArgs.nBasicSpaceRight)	nHapRightDisplay = objArgs.nBasicSpaceRight;

		}

		bool  IsDesX() {		return ((objArgs.wDisplayFlags & EnumDisplayFlag.DESX) == 0 ? false : true); } // X축의 설명이 있는냐?
		bool  IsDesY() {		return ((objArgs.wDisplayFlags & EnumDisplayFlag.DESY) == 0 ? false : true); } // Y축의 설명이 있는냐?
		bool  IsGuideLine() {	return ((objArgs.wDisplayFlags & EnumDisplayFlag.GUIDELINE) == 0 ? false : true); }	// Guide Line이 있느냐.
		bool  IsTagColor()  {	return ((objArgs.wDisplayFlags & EnumDisplayFlag.TAG_COLOR) == 0 ? false : true); }	// 태그 안내판 표시.
		bool  IsTagCurr()    {	return ((objArgs.wDisplayFlags & EnumDisplayFlag.TAG_CURR) == 0 ? false : true); }
		bool  IsTagOldCurr() {	return ((objArgs.wDisplayFlags & EnumDisplayFlag.TAG_OLD_CURR) == 0 ? false : true); }
		bool  IsTagMinMax()  {	return ((objArgs.wDisplayFlags & EnumDisplayFlag.TAG_MIN_MAX) == 0 ? false : true); }
		bool  IsBackBorder()  {	return ((objArgs.wDisplayFlags & EnumDisplayFlag.BACK_BORDER) == 0 ? false : true); }
		bool  IsByDescription(){return ((objArgs.wDisplayFlags & EnumDisplayFlag.BY_DESCRIPTION) == 0 ? false : true); }

		void GetGraphZone(ref int x1, ref int y1, ref int x2, ref int y2)
		{
			GetViewZoneWithRotation(ref x1, ref y1, ref x2, ref y2);

			x1 += 3;
			y1 += 3;
			x2 -= 3;
			y2 -= 3;

			if(IsDesX()) 
			{
				y2 -= (int)(nCharHeight*2);
			}
			else 
			{
				y2 -= nCharHeight/2-2;
			}

			if(IsDesY()) 
			{
				x1 += (nCharWidth*objArgs.nLevelDisplaySize)*nHapLeftDisplay;
				x2 -= (nCharWidth*objArgs.nLevelDisplaySize)*nHapRightDisplay;
			}

			if(IsTagColor())	y2 -= (nCharHeight+2)*1;
			if(IsTagCurr())		y2 -= (nCharHeight+2)*1;
			if(IsTagOldCurr())	y2 -= (nCharHeight+2)*1;
			if(IsTagMinMax())	y2 -= (nCharHeight+2)*2;

			if(bDisplayPointDate)	y1 += nCharHeight;
		}

		void CalcCharSize()
		{
			Font font = MakeFont();

			nCharHeight = (int)font.GetHeight()+1;
			nCharWidth = (int)font.SizeInPoints;
			
			//GetTextCharSize(hdc, nCharWidth, nCharHeight);
			//g.Get
		}

		void SetPointSize(int size)
		{
			if(size <= 0)	size = 10;
			if(size >= 500)	size = 500;
			objArgs.wPointSize = size;
		}

		string MakeDateTimeString(EnumDbType dbtype, int year, int mon, int day, int hour, int min, int sec)
		{
			string command;
			if(objArgs.nDateColumnType == 1) 
			{
				command = String.Format("'{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}'", year, mon, day, hour, min, sec);
			}
			else 
			{
				command = DbTool.MakeDateTimeString(dbtype, year, mon, day, hour, min, sec);
			}

			return command;
		}

		DateTime GetDateTimeFromRow(DataSet ds, DataRow row, int column_time)
		{
			DateTime t;
			
			if(objArgs.nDateColumnType == 1) 
			{
				int year, mon, day, hour, min, sec;
				string s = row[column_time].ToString();
				string imsi;
				if(s.Length < 14) 
				{
					t = new DateTime(1,1,1,0,0,0);
				}
				else 
				{
					imsi = s.Substring(0, 4);
					year = ConvertTool.ToInt32(imsi);
					imsi = s.Substring(4, 2);
					mon = ConvertTool.ToInt32(imsi);
					imsi = s.Substring(6, 2);
					day = ConvertTool.ToInt32(imsi);
					imsi = s.Substring(8, 2);
					hour = ConvertTool.ToInt32(imsi);
					imsi = s.Substring(10, 2);
					min = ConvertTool.ToInt32(imsi);
					imsi = s.Substring(12, 2);
					sec = ConvertTool.ToInt32(imsi);

					if(hour == 24)	// 24시로 되어있으면 0으로 해주지만 실제로 데이타를 읽어오지 못할수도 있다. 소팅순서가 틀리므로 만약 24시가 다음날 00시라면 날짜를 하루 더해야 한다.
					{
						hour = 0;
					}

					try 
					{
						t = new DateTime(year, mon, day, hour, min, sec);
					}
					catch 
					{
						t = new DateTime(1,1,1,0,0,0);
					}
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
					t = new DateTime(1,1,1,0,0,0);
				}
			}
			
			return t;
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

		void GetViewFullBase(TagAiClass ai, DB_TREND_MEMBER member, out double view_full, out double view_base)
		{
			if(bUseLocalRange) 
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

		int GetDisplayTimeDevide(int gx1, int gx2)
		{
			if(!bAutoGuideLine)		return objArgs.wTimeDevide;

			int devide = objArgs.wTimeDevide;
	
			devide = (gx2-gx1)/(nCharWidth*7);
			if(devide == 0)	devide = objArgs.wTimeDevide;
			else			devide = objArgs.wShowUnit/devide;

			if(objArgs.wTimeSelectOption == 0) 
			{	// milli data
				if(devide > 500)	devide = 1000;
			}
			else if(objArgs.wTimeSelectOption == 1) 
			{	// second data
				if(devide > 30)	devide = 60;
			}
			else if(objArgs.wTimeSelectOption == 2) 
			{	// min data
				if(devide > 30)	devide = 60;
			}
			else if(objArgs.wTimeSelectOption == 3) 
			{	// hour data
				if(devide > 12)	devide = 24;
			}
			else if(objArgs.wTimeSelectOption == 4) 
			{	// day data
				if(devide > 15)	devide = 31;
			}
			else if(objArgs.wTimeSelectOption == 5) 
			{	// hour data
				if(devide > 6)	devide = 12;
			}
			else if(objArgs.wTimeSelectOption == 6) 
			{	// hour data
				if(devide > 6)	devide = 12;
			}
	
			if(devide <= 0)	devide = 1;
			return devide;
		}
        
		void CheckOneMember(DB_TREND_MEMBER member)
		{
			TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);
			if(member.nLevelFrom < 0)	member.nLevelFrom = 0;
			if(member.nLevelTo > 100)	member.nLevelTo = 100;
			if(member.nLevelFrom >= member.nLevelTo) 
			{
				member.nLevelFrom = 0;
				member.nLevelTo = 100;
			}
			member.visible = 1;
			if(member.nTagDisplaySize < 1 || member.nTagDisplaySize > 40) 
			{
				member.nTagDisplaySize = 10;
			}

			if(member.nType == EnumTagType.AI) 
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
			if(block == null)
				blockMember = new ArrayList();
			else
				blockMember = block;

			int l;
			DB_TREND_MEMBER member;

			if(block != null) 
			{
				for(l = 0; l < block.Count; l++) 
				{
					member = (DB_TREND_MEMBER)block[l];
					CheckOneMember(member);
				}
			}

			CalcAxisPosition();
		}

		static int nSignGraphNo = 0;

		void MallocOneBuf(DB_TREND_MEMBER member)
		{
			member.point = new ONE_POINT_STRUCT[objArgs.wShowUnit];

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) {
				int gab;

				nSignGraphNo ++;
				nSignGraphNo %= 20;

				//if(member.nType == 0) 	// AI TAG
				//	gab = (ter->analogInput[member.nPos].file.full-ter->analogInput[member.nPos].file.fbase)/2;
				//else
					gab = 50;

				for(int j = 0; j < objArgs.wShowUnit; j++) 
				{
					member.point[j].val = (float)(gab+Math.Sin(3.14*((720.0*(j+nSignGraphNo*5))/objArgs.wShowUnit)/360.0)*gab);
					if(member.nType == EnumTagType.DI) 
					{
						if(member.point[j].val > gab/2)	member.point[j].val = 1;
						else							member.point[j].val = 0;
					}
					member.point[j].read_flag = true;
				}
			}
		}

		void MallocAllBuf()
		{
			DB_TREND_MEMBER member;
			for(int i = 0; i < blockMember.Count; i++) 
			{
				member = (DB_TREND_MEMBER)blockMember[i];
				MallocOneBuf(member);
			}
		}

		bool GetCursorValue(DB_TREND_MEMBER member, out double curr, out double min, out double max)
		{
			curr = 0;
			min  = 0;
			max  = 100;

			//if(member.point == null)			return false;
	
			int x1, x2;

			x1 = nCursorX1;
			x2 = nCursorX2;

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(x1 < 0 || x1 >= objArgs.wShowUnit)		return false;
			if(x2 < 0 || x2 >= objArgs.wShowUnit)		return false;

			return GetZoneValue(member, out curr, out min, out max, x1, x2);
		}

		bool GetZoneValue(DB_TREND_MEMBER member, out double curr, out double min, out double max, int from, int to)
		{
			curr = 0;
			min  = 0;
			max  = 100;

			if(member.nType == EnumTagType.AI) 
			{
				TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
				GetViewFullBase(ai, member, out max, out min);
			}

			// if(member.point == null)			return false;
			//if(dsData.Tables.IndexOf(member.tag) == -1)	return;
	
			bool retn = false;

			if(from == to) 
			{
				int pos;
		
				pos = from;

				if(pos < 0 || pos >= objArgs.wShowUnit)		return false;

				//DataRow row = member.ds.Tables[0].Rows[pos];

				curr = member.point[pos].val;
				retn = member.point[pos].read_flag;
			}
			else 
			{
				int count = 0;
				int i;

				if(from > to)	Tools.Temp(ref from, ref to);
				if(from < 0 || from >= objArgs.wShowUnit)	return false;
				if(to < 0   || to >= objArgs.wShowUnit)		return false;
				
				//DataRow row;
				float val;

				for(i = from; i <= to; i++) 
				{
					//row = member.ds.Tables[0].Rows[i];

					if(!member.point[i].read_flag)	continue;
					val = (float)member.point[i].val;
					if(count == 0) 
					{
						curr = val;
						min = curr;
						max = curr;
					}
					else 
					{
						
						curr += val;
						if(val < min)	min = val;
						if(val > max)	max = val;
					}
					count++;
				}
		
				if(count > 0) 
				{
					curr = curr/count;
					retn = true;
				}
			}

			return retn;
		}


		string GetDateString(int pos)
		{
			string buf;
		
			DateTime dt = new DateTime(dtStartTime.Ticks);

			int data_gab = pos*objArgs.nDataCycle;
                
			if(objArgs.wTimeSelectOption == 0) 
			{	// min data
				dt = dt.AddMilliseconds(data_gab);
				buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}.{6:000}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
			}
			else if(objArgs.wTimeSelectOption == 1) 
			{	// min data
				dt = dt.AddSeconds(data_gab);
				buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
			}
			else if(objArgs.wTimeSelectOption == 2) 
			{	// min data
				dt = dt.AddMinutes(data_gab);
				buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
			}
			else if(objArgs.wTimeSelectOption == 3) 
			{	// hour data
				dt = dt.AddHours(data_gab);
				if(Tools.IsLangKorean()) 
				{
					buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}시", dt.Year, dt.Month, dt.Day, dt.Hour);
				}
				else if(Tools.IsLangJapanese()) 
				{
					buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}時", dt.Year, dt.Month, dt.Day, dt.Hour);
				}
				else if(Tools.IsLangChinese()) 
				{
					buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}时", dt.Year, dt.Month, dt.Day, dt.Hour);
				}
				else 
				{
					buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}H", dt.Year, dt.Month, dt.Day, dt.Hour);
				}
			}
			else if(objArgs.wTimeSelectOption == 4) 
			{	// day data
				dt = dt.AddDays(data_gab);
				buf = String.Format("{0:0000}/{1:00}/{2:00}", dt.Year, dt.Month, dt.Day);
			}
			else if(objArgs.wTimeSelectOption == 5) 
			{	// mon data
				dt = dt.AddMonths(data_gab);
				if(Tools.IsLangKorean()) 
				{
					buf = String.Format("{0:0000}년 {1:00}월", dt.Year, dt.Month);
				}
				else if(Tools.IsLangJapanese()) 
				{
					buf = String.Format("{0:0000}年 {1:00}月", dt.Year, dt.Month);
				}
				else if(Tools.IsLangChinese()) 
				{
					buf = String.Format("{0:0000}年 {1:00}月", dt.Year, dt.Month);
				}
				else 
				{
					buf = String.Format("{0:0000}/{1:00}", dt.Year, dt.Month);
				}
			}
			else if(objArgs.wTimeSelectOption == 6) 
			{	// mon data
				dt = dt.AddYears(data_gab);
				if(Tools.IsLangKorean()) 
				{
					buf = String.Format("{0:0000}년", dt.Year);
				}
				else if(Tools.IsLangJapanese()) 
				{
					buf = String.Format("{0:0000}年", dt.Year);
				}
				else if(Tools.IsLangChinese()) 
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

		void DisplayGraphGuideLine(Graphics g, int gx1, int gy1, int gx2, int gy2)
		{
			int   pos;
			int   i;
			int   time_devide = GetDisplayTimeDevide(gx1, gx2);
			GUIDE_DISPLAY guide;
			int real_y, real_size;

			Pen pen = new Pen(objArgs.lColorGuideLine, 1);

			for(int j = 0; j < arrayGuideDisplay.Count; j++) 
			{
				guide = (GUIDE_DISPLAY)arrayGuideDisplay[j];

				for(i = 0; i <= objArgs.wLevelDevide; i++) 
				{
					if(i == 0 && guide.from == 0)	continue;
					if(i == objArgs.wLevelDevide && guide.to == 100)	continue;

					real_size = (guide.to-guide.from)*(gy2-gy1)/100;
					real_y = gy2-(guide.from)*(gy2-gy1)/100;
					pos = real_y-(real_size)*i/(objArgs.wLevelDevide);

					//pos = gy2-(gy2-gy1)*i/objArgs.wLevelDevide;
					g.DrawLine(pen, gx1+1, pos, gx2, pos);
				}
			}

			ushort w;
			DateTime t = new DateTime(dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, dtStartTime.Second);

			if(objArgs.wTimeSelectOption == 0) 
			{	// 밀리 데이터
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddMilliseconds(objArgs.nDataCycle)) 
				{
					if((t.Millisecond%time_devide) != 0)		continue;

					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else if(objArgs.wTimeSelectOption == 1) 
			{	// 초 데이터
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddSeconds(objArgs.nDataCycle)) 
				{
					if((t.Second%time_devide) != 0)		continue;

					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else if(objArgs.wTimeSelectOption == 2) 
			{	// 분 데이터
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddMinutes(objArgs.nDataCycle)) 
				{
					if((t.Minute%time_devide) != 0)		continue;

					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else if(objArgs.wTimeSelectOption == 3) 
			{	// 시간 데이터
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddHours(objArgs.nDataCycle)) 
				{
					if((t.Hour%time_devide) != 0)		continue;

					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else if(objArgs.wTimeSelectOption == 4) 
			{	// 일일 데이터
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddDays(objArgs.nDataCycle)) 
				{
					if(((t.Day-1)%time_devide) != 0)		continue;
			
					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else if(objArgs.wTimeSelectOption == 5) 
			{	// 월간 데이터.
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddMonths(objArgs.nDataCycle)) 
				{
					if(((t.Month-1)%time_devide) != 0)		continue;

					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else if(objArgs.wTimeSelectOption == 6) 
			{	// 연간 데이터.
				for(w = 0; w < objArgs.wShowUnit; w++, t = t.AddYears(objArgs.nDataCycle)) 
				{
					if(((t.Year)%time_devide) != 0)		continue;

					if(objArgs.wShowUnit-1 == 0)	continue;
					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else 
			{	// 분 데이터.

			}
		}

		public override void EventTimerObject(System.Windows.Forms.Form form)
		{
			TagPublicClass pub;

			DB_TREND_MEMBER member;
			int i;

			for(i = 0; i < blockMember.Count; i++) 
			{
				member = (DB_TREND_MEMBER)blockMember[i];
				pub = TagLib.GetStructPublic(member.tag, ref member.nPos);
				pub.NeedDataCurr = true;
			}
		}

		public override void EventTag(System.Windows.Forms.Form form, COMM_EVENT_STRUCT tagevent)
		{
			TagPublicClass pub;

			DB_TREND_MEMBER member;
			int i;

			for(i = 0; i < blockMember.Count; i++) 
			{
				member = (DB_TREND_MEMBER)blockMember[i];
				pub = TagLib.GetStructPublic(member.tag, ref member.nPos);
				if(member.tag == tagevent.tag) 
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
			if(!bShowToolTip)	return;
			if(blockMember.Count == 0)	return;		// 멤버가 없다.
			if(TotalConfig.defineMode != EnumDefineMode.MODE_RUN)	return;

			int sizey;
			int y=0;
			double gaby;
			int  l;
			DB_TREND_MEMBER member;
			TagAiClass ai = null;
			int view_y1;
			int view_y2;
			int view_sizey;
			int small_gab = Math.Abs(gy2-gy1);
			int small_pos = 0;
			int line_y;
			int gab;

			sizey = gy2-gy1+1;

			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (DB_TREND_MEMBER)blockMember[l];

				if(member.visible == 0)	continue;

				if(member.nType == EnumTagType.AI) 
				{
					ai = GetRealTagAI(member.tag, ref member.nPos);
					gaby = member.max_value-member.min_value;
				}
				else if(member.nType == EnumTagType.DI) 
				{
					gaby = 100;
				}
				else 
				{
					gaby = member.view_full-member.view_base;
				}

				view_y1 = gy2-(member.nLevelTo)*(gy2-gy1)/100;
				view_y2 = gy2-(member.nLevelFrom)*(gy2-gy1)/100; 
				view_sizey = view_y2-view_y1+1;

				if(gaby == 0)	// protect divide by zero
					y = 0;
				else 
				{
					if(member.point[posx].read_flag) 
					{	// 값을 읽었을 때만
						if(member.nType == EnumTagType.AI) 
						{	// AI
							y = (int)((double)view_sizey*(TagUtil.GetDisplayValue(ai, member.point[posx].val) - member.min_value)/gaby);
						}
						else if(member.nType == EnumTagType.DI) 
						{
							if(member.point[posx].val > 0)	
								y = (int)(view_sizey*0.9);
							else										
								y = (int)(view_sizey*0.1);
						}
						else 
						{
							y = (int)((double)view_sizey*(member.point[posx].val - member.min_value)/gaby);
						}
					}
				}

				if(y < 0)				y = 0;
				if(y >= view_sizey)		y = view_sizey-1;

				if(member.nReverseY == 1)	line_y = view_y1+y;
				else						line_y = view_y2-y;

				gab = Math.Abs(e.Y-line_y);
				if(gab < small_gab) 
				{
					small_gab = gab;
					small_pos = l;
				}
			}
			
			
			string des;

			member = (DB_TREND_MEMBER)blockMember[small_pos];

			if(member.nType == EnumTagType.AI) 
			{
				ai = GetRealTagAI(member.tag, ref member.nPos);
				des = ai.description;
			}
			else if(member.nType == EnumTagType.DI) 
			{
				TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
				des = di.description;
			}
			else 
			{
				des = member.tag;
			}

			string msg = String.Format("{0}\nData={1}\nTime={2}", des, member.point[posx].val, GetDateString(posx));

			toolTip.SetToolTip(form, msg);
			toolTip.InitialDelay = 0;
			nToolTipX = e.X;
			nToolTipY = e.Y;
		}
 * 
		void SetShowUnit(int unit)
		{
			if(unit == objArgs.wShowUnit)	return;
			if(unit < 10)	unit = 10;
			objArgs.wShowUnit = unit;
			if(nCursorX1 < 0)			nCursorX1 = 0;
			if(nCursorX1 >= objArgs.wShowUnit)	nCursorX1 = objArgs.wShowUnit-1;
			if(nCursorX2 < 0)			nCursorX2 = 0;
			if(nCursorX2 >= objArgs.wShowUnit)	nCursorX2 = objArgs.wShowUnit-1;
			MallocAllBuf();
		}
	}
}
*/

