using System;
using System.Drawing;
using System.Windows.Forms;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using System.Collections;
using AutoLibLocal;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GraphicModule
{
	[Serializable]
	public class ObjectArgsGraphPublic
	{
		public EnumDisplayFlag	wDisplayFlags;
		public int			    wPointSize = 10;
		public int				nLevelDisplaySize = 7;

        public Color colorHiHi = Color.FromArgb(0x80, 0, 0);
        public Color colorHigh = Color.FromArgb(255, 0, 0);
        public Color colorLow = Color.FromArgb(0, 0, 255);
        public Color colorLoLo = Color.FromArgb(0, 0, 0x80);

        public Color colorPanelText = Color.Black;  //20250204 PSU 추가
        public Color colorPanelBack = Color.FromArgb(0xF0, 0xF0, 0xF0);  //F0F0F0   << Color.FromArgb(0xc0, 0xc0, 0xc0)

        public int nThickHiHi = 1;
        public int nThickHigh = 1;
        public int nThickLow = 1;
        public int nThickLoLo = 1;

        public int nStyleHiHi = 2;  // Dot
        public int nStyleHigh = 2;  // Dot
        public int nStyleLow = 2;   // Dot
        public int nStyleLoLo = 2;  // Dot
	}
	
	[Serializable]
	public class ObjectArgsMultiGraph
	{
		public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();
		public BrushPublic		lColorFill = new BrushPublic();
		public Color		lColorText;
        public BrushPublic lColorBack = new BrushPublic();
		public Color		lColorGuideLine;

		private int _wShowUnit;
		public int wShowUnit
		{
            get => _wShowUnit;
            set => _wShowUnit = Math.Max(10, Math.Min(36000, value));
        }

		private int _nDataTime;
		public int	nDataTime       // milli sec
		{
            get => _nDataTime;
            set => _nDataTime = Math.Max(1, Math.Min(60000, value));
        }
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

        public string 		sTitle;
		public sbyte		bTimeDirToLeft;
		public sbyte 		bDisplayByTime;

		// public int nBasicSpaceLeft = 0;		// 왼쪽의 기본적인 준비 공간
		// public int nBasicSpaceRight = 0;	    // 오른쪽의 기본적인 준비 공간

        public LogarithmicScale logarithmicScale = new LogarithmicScale(); // 2020-5-6 추가
	}
	/// <summary>
	/// Summary description for ObjectMultiGraph.
	/// </summary>
	[Serializable]
	public class ObjectMultiGraph : ObjectExpand
	{
		ArrayList blockMember;
		
		int   nHapLeftDisplay;	// 왼쪽 눈금의 개수
		int   nHapRightDisplay;	// 왼쪽 눈금의 개수
		int   nCharWidth;
		int   nCharHeight;

		bool  bDisplayPointDate;	// 자료시점의 날짜를 표시해 준다.
		// bool  bAutoViewRange;
		bool  bAutoGuideLine;
		bool  bUseLocalRange;
		//sbyte cGraphStartTimeMethod;
		int   dwFirstLevelDisplay;
		//int   nDataGab;
		int   nCursorX1;
		int   nCursorX2;
		int   nBufPos = 0;
		bool  bMouseCapture = false;
		bool  bRunFlag = true;
        
		ObjectArgsMultiGraph objArgs;

        DateTime dtStartTime = DateTimeServer.Now;
		[NonSerialized]
		Form formParent;

		// 등록된 클래스 리스트
		[NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        int nOldMilliSec;
		int nRemainMilliSec = 0;
		DateTime dtLastData;

		public ObjectArgsMultiGraph ObjectArgs 
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

		public ObjectMultiGraph(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsMultiGraph args, ArrayList block)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.MultiGraph;
			objArgs = args;
			formParent = form; 

			bDisplayPointDate = false;
			//bAutoViewRange = false;
			bAutoGuideLine = false;
			bUseLocalRange = false;
			//cGraphStartTimeMethod = 0;			// AUTO

			bRunFlag = true;

			SetPointSize(args.pub.wPointSize);		// y 크기의 10/1000 만한 포인트 크기
			dwFirstLevelDisplay = 0;			// 첫 번째 Level디스프레이는 사용자가 바꿀 수 있다.

			if(objArgs.wLevelDevide < 2)		objArgs.wLevelDevide = 2;
			if(objArgs.wLevelDevide > 100)		objArgs.wLevelDevide = 100;

			if(objArgs.pub.nLevelDisplaySize < 2)	objArgs.pub.nLevelDisplaySize = 2;
			if(objArgs.pub.nLevelDisplaySize > 50)	objArgs.pub.nLevelDisplaySize = 7;

			if(block == null)
				blockMember = new ArrayList();
			else
				blockMember = block;

			SetBlock(blockMember);

			if(objArgs.wTimeDevide < 1)	 objArgs.wTimeDevide = 10;

			CalcCharSize();
			nCursorX1 = objArgs.wShowUnit/2;
			nCursorX2 = objArgs.wShowUnit/2;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				arrayClassList.Add(this);
			}

			nOldMilliSec = -1;
            dtLastData = DateTimeServer.Now;

			if(objArgs.nDataTime < 1)	objArgs.nDataTime = 1000;

			TextColor = args.lColorText;
			BackColor = args.lColorBack;
			FillColor = args.lColorFill;
		}

		public override void Close()
		{
			arrayClassList.Remove(this);
		}

        TagAiClass GetRealTagAI(string tag, ref int[] tag_pos)
        {
            TagAiClass ai = TagLib.GetStructAI(tag, ref tag_pos);

            if (ai.cTagLinkType == 3)
            {	// 간접태그
                if (ai.assign != null && ai.assign.pos[0] != TagLib.TAG_NOT_FOUND)
                {	// assign tag가 있을 때
                    TagAiClass ai2 = TagLib.GetStructAI(ai.assign.tag, ref ai.assign.pos);
                    return ai2;
                }
            }

            return ai;
        }

        TagDiClass GetRealTagDI(string tag, ref int[] tag_pos)
        {
            TagDiClass di = TagLib.GetStructDI(tag, ref tag_pos);

            if (di.cTagLinkType == 3)
            {	// 간접태그
                if (di.assign != null && di.assign.pos[0] != TagLib.TAG_NOT_FOUND)
                {	// assign tag가 있을 때
                    TagDiClass di2 = TagLib.GetStructDI(di.assign.tag, ref di.assign.pos);
                    return di2;
                }
            }

            return di;
        }

		void SetBasicLevel(int pos)
		{
			if(pos < 0)	return;
			if(pos >= (int)blockMember.Count)	return;
			if(dwFirstLevelDisplay != pos) 
			{
				dwFirstLevelDisplay = pos;
				InvalidateObject(formParent);
			}	
		}

		double GetCursorData(int pos)
		{
			if(pos < 0)	return 0;
			if(pos >= (int)blockMember.Count)	return 0;

			ANALOG_GRAPH_MEMBER member;
			double value, min, max;

			member = (ANALOG_GRAPH_MEMBER)blockMember[pos];

			if(GetCursorValue(member, out value, out min, out max)) 
			{
				return value;
			}
			else 
			{
				return 0;
			}
		}

		void AddTag(ANALOG_GRAPH_MEMBER member)
		{
			member.point = null;
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
			if(member.nAxisPosition > 2 || member.nAxisPosition < 0)
				member.nAxisPosition = 0;

			blockMember.Add(member);

			MallocAllBuf();
			CalcAxisPosition();
		}

        void DeleteTag(string tag)
        {
            ANALOG_GRAPH_MEMBER member;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (ANALOG_GRAPH_MEMBER)blockMember[i];

                if (tag == member.tag)
                {
                    blockMember.RemoveAt(i);
                    CalcAxisPosition();
                    return;
                }
            }
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
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				CalcCharSize();
			}
		}

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

		public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
            await Task.CompletedTask; // 경고 해결용

            if (command == "MultiGraphSetBackColor")
			{
                GetBackColor().basic_color = Color.FromArgb((int)args[1]);
				InvalidateObject(formParent);
			}
			else if(command == "MultiGraphSetBasicLevel")
			{
				SetBasicLevel((int)args[1]);
			}
			else if(command == "MultiGraphGetCursorData")
			{
				return GetCursorData((int)args[1]);
			}
			else if(command == "MultiGraphClear")
			{
				blockMember.Clear();
                CalcAxisPosition();     // 멤버를 삭제만 하고 아무것도 안하는 경우도 있어 영역을 재 조정해준다. 2016-1-21
			}
			else if(command == "MultiGraphAddTag")
			{
                ANALOG_GRAPH_MEMBER member = new ANALOG_GRAPH_MEMBER();

                member.tag = (string)args[1];
                member.color = Color.FromArgb((int)args[2]);
                member.nValueType = (int)args[3];
                member.nPointType = (int)args[4];
                member.nLineThick = (int)args[5];
                member.nAxisPosition = (int)args[6];
                member.nLevelFrom = (int)args[7];
                member.nLevelTo = (int)args[8];
                member.nTagDisplaySize = (int)args[9];
                member.bReverseY = (sbyte)(int)args[10];
                member.nGraphType = (int)args[11];
                member.wFlags = (ushort)(int)args[12];

                AddTag(member);		
			}
            else if (command == "MultiGraphDeleteTag")
            {
                DeleteTag((string)args[1]);
            }
			else if(command == "MultiGraphSetDataSize")
			{
				this.SetShowUnit((int)args[1]);
			}
			else if(command == "MultiGraphGetDataSize")
			{
				return this.objArgs.wShowUnit;
			}
            else if (command == "MultiGraphSetVisible")
            {
                VisibleTag((string)args[1], (int)args[2]);
            }
            else if (command == "MultiGraphSetPanelTextColor")
            {
                int color = ((int)args[1]);
                objArgs.pub.colorPanelText = Color.FromArgb(color);
                return 1;
            }
            else if (command == "MultiGraphSetPanelBackColor")
            {
                int color = ((int)args[1]);
                objArgs.pub.colorPanelBack = Color.FromArgb(color);
                return 1;
            }
            else if (command == "MultiGraphGetCursor1Data")
            {
                return GetCursor12Value((int)args[1], false);
            }
            else if (command == "MultiGraphGetCursor2Data")
            {
                return GetCursor12Value((int)args[1], true);
            }
            else if (command == "MultiGraphSetAlarmLineThick")
            {
                int type = (int)args[1];
                int thick = (int)args[2];
                if (type == 0)
                    objArgs.pub.nThickLoLo = thick;
                else if (type == 1)
                    objArgs.pub.nThickLow = thick;
                else if (type == 2)
                    objArgs.pub.nThickHigh = thick;
                else if (type == 3)
                    objArgs.pub.nThickHiHi = thick;

                InvalidateObject(formParent);

                return 1;
            }
            else if (command == "MultiGraphSetAlarmLineStyle")
            {
                int type = (int)args[1];
                int style = (int)args[2];
                if (type == 0)
                    objArgs.pub.nStyleLoLo = style;
                else if (type == 1)
                    objArgs.pub.nStyleLow = style;
                else if (type == 2)
                    objArgs.pub.nStyleHigh = style;
                else if (type == 3)
                    objArgs.pub.nStyleHiHi = style;

                InvalidateObject(formParent);

                return 1;
            }
            else if (command == "MultiGraphSetLogarithmicScale")
            {
                objArgs.logarithmicScale.bUse = ((int)args[1] == 1);
                objArgs.logarithmicScale.fBase = (double)args[2];
                if (objArgs.logarithmicScale.fBase < 2)
                    objArgs.logarithmicScale.fBase = 2;
            }
            else if (command == "MultiGraphSetDataCycle")
            {
                int cycle = (int)args[1];   //20251204 PSU 수정 
                if (cycle < 1) cycle = 1000;
                if (cycle > 60000) cycle = 1000;
                objArgs.nDataTime = (int)args[1];
            }
            else if (command == "MultiGraphSetGridLine")
            {
                objArgs.wTimeDevide = (int)args[1];
                objArgs.wLevelDevide = (int)args[2];
            }
            else if (command == "MultiGraphSetStart")
            {
                int flag = (int)args[1];

                bRunFlag = (flag == 1) ? true : false;  // (flag == 1);
            }
			else 
			{
				return 0;
			}

			return 1;
		}

        double GetCursor12Value(int pos, bool flag_cursor2)
        {
            double curr = 0;

            if (pos < 0) return curr;
            if (pos >= (int)blockMember.Count) return curr;

            ANALOG_GRAPH_MEMBER member;
            double min, max;

            member = (ANALOG_GRAPH_MEMBER)blockMember[pos];

            int x1, x2;

            if (flag_cursor2)
            {
                x1 = nCursorX2;
                x2 = nCursorX2;
            }
            else
            {
                x1 = nCursorX1;
                x2 = nCursorX1;
            }

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (x1 < 0 || x1 >= objArgs.wShowUnit) return curr;
            if (x2 < 0 || x2 >= objArgs.wShowUnit) return curr;

            GetZoneValue(member, out curr, out min, out max, x1, x2);

            return curr;
        }

        //Color colorPanelText = Color.Black;  //20250204 PSU 제거
        //Color colorPanelBack = Color.FromArgb(0xc0, 0xc0, 0xc0);

        void VisibleTag(string tag, int visible)
        {
            ANALOG_GRAPH_MEMBER member;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (ANALOG_GRAPH_MEMBER)blockMember[i];

                if (tag == member.tag)
                {
                    member.visible = (sbyte)visible;
                    InvalidateObject();
                    return;
                }
            }
        }

		void SetPointSize(int size)
		{
			if(size <= 0)	size = 10;
			if(size >= 500)	size = 500;
			objArgs.pub.wPointSize = (ushort)size;
		}

		void SetBlock(ArrayList block)
		{
			int l;
			ANALOG_GRAPH_MEMBER member;

			if(block == null)
				blockMember = new ArrayList();
			else
				blockMember = block;

			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];
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
			}

			MallocAllBuf();
			CalcAxisPosition();
		}

		void CalcCharSize()
		{
			Font font = MakeFont();

			nCharHeight = (int)font.GetHeight()+1;
			nCharWidth = (int)font.SizeInPoints;
			
			//GetTextCharSize(hdc, nCharWidth, nCharHeight);
			//g.Get
		}

		static int nSignGraphNo = 0;

        public static void MakeSampleData(ANALOG_GRAPH_MEMBER member, int showunit)
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

            for (int j = 0; j < showunit; j++)
            {
                member.point[j].val = (float)(gab + Math.Sin(3.14 * ((720.0 * (j + nSignGraphNo * 5)) / showunit) / 360.0) * gab);
                if (member.nType == EnumTagType.DI)
                {
                    if (member.point[j].val > gab / 2) member.point[j].val = 1;
                    else member.point[j].val = 0;
                }
                member.point[j].read_flag = true;
            }
        }

		void MallocOneBuf(ANALOG_GRAPH_MEMBER member)
		{
			member.point = new ONE_POINT_STRUCT[objArgs.wShowUnit];

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
                /*
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
				}*/
                MakeSampleData(member, objArgs.wShowUnit);
			}
		}

		void MallocAllBuf()
		{
			ANALOG_GRAPH_MEMBER member;
			for(int i = 0; i < blockMember.Count; i++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[i];
				MallocOneBuf(member);
			}
		}


		/*
		void MallocBuf()
		{
			int l;
			ANALOG_GRAPH_MEMBER member;
	
			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];

				member.point = new ONE_POINT_STRUCT[objArgs.wShowUnit];
			}
		}
		*/

		void CalcAxisPosition()
		{
			int lstart = 0, lend = 0;
			int rstart = 0, rend = 0;
			int lpos = 0;
			int rpos = 0;
			int l;
			ANALOG_GRAPH_MEMBER member;

			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];

				if(member.nAxisPosition == 1) 
				{	// left position
					if(lstart == 0 && lend == 0) 
					{
						member.nAxisCalcPos = lpos;
						lstart = member.nLevelFrom;
						lend = member.nLevelTo;
					}
					else 
					{
						if(member.nLevelFrom >= lend || member.nLevelTo <= lstart) 
						{
							member.nAxisCalcPos = lpos;	
							lstart = 0;
							lend = 0;
							lpos++;
						}
						else 
						{
							lpos++;
							member.nAxisCalcPos = lpos;	
							lstart = member.nLevelFrom;
							lend = member.nLevelTo;
						}
					}
				}
				else if(member.nAxisPosition == 2) 
				{	// right position
					if(rstart == 0 && rend == 0) 
					{
						member.nAxisCalcPos = rpos;
						rstart = member.nLevelFrom;
						rend = member.nLevelTo;
					}
					else 
					{
						if(member.nLevelFrom >= rend || member.nLevelTo <= rstart) 
						{
							member.nAxisCalcPos = rpos;
							rstart = 0;
							rend = 0;
							rpos++;
						}
						else 
						{
							rpos++;
							member.nAxisCalcPos = rpos;	
							rstart = member.nLevelFrom;
							rend = member.nLevelTo;
						}
					}
				}
			}

			if(lstart == 0 && lend == 0 && lpos == 0) 
			{
				nHapLeftDisplay  = 0;	// 왼쪽 눈금의 개수
			}
			else 
			{
				nHapLeftDisplay  = lpos+1;	// 왼쪽 눈금의 개수
			}
			if(rstart == 0 && rend == 0 && rpos == 0) 
			{
				nHapRightDisplay  = 0;		// 오른쪽 눈금의 개수
			}
			else 
			{
				nHapRightDisplay  = rpos+1;	// 오른쪽 눈금의 개수
			}
		}

		bool  IsDesX() {		return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.DESX) == 0 ? false : true); }      // X축의 설명이 있는냐?
		bool  IsDesY() {		return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.DESY) == 0 ? false : true); }      // Y축의 설명이 있는냐?
		bool  IsGuideLine() {	return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.GUIDELINE) == 0 ? false : true); }	// Guide Line이 있느냐.
		bool  IsTagColor()  {	return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_COLOR) == 0 ? false : true); }	// 태그 안내판 표시.
		bool  IsTagCurr()    {	return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_CURR) == 0 ? false : true); }
		bool  IsTagOldCurr() {	return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_OLD_CURR) == 0 ? false : true); }
		bool  IsTagMinMax()  {	return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.TAG_MIN_MAX) == 0 ? false : true); }
		bool  IsBackBorder()  {	return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.BACK_BORDER) == 0 ? false : true); }
		bool  IsByDescription(){return ((objArgs.pub.wDisplayFlags & EnumDisplayFlag.BY_DESCRIPTION) == 0 ? false : true); }

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
				x1 += (nCharWidth*objArgs.pub.nLevelDisplaySize)*nHapLeftDisplay;
				x2 -= (nCharWidth*objArgs.pub.nLevelDisplaySize)*nHapRightDisplay;
			}

			if(IsTagColor())	y2 -= (nCharHeight+2)*1;
			if(IsTagCurr())		y2 -= (nCharHeight+2)*1;
			if(IsTagOldCurr())	y2 -= (nCharHeight+2)*1;
			if(IsTagMinMax())	y2 -= (nCharHeight+2)*2;

			if(bDisplayPointDate)	y1 += nCharHeight;
		}

		void GetViewFullBase(TagAiClass ai, ANALOG_GRAPH_MEMBER member, out double view_full, out double view_base)
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

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			int gx1=0, gy1=0, gx2=0, gy2=0;

			GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);

            //CE 소스 업데이트 시 삭제 20250206 PSU
            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE)
            {
                objArgs.pub.colorPanelBack = Color.FromArgb(0xc0, 0xc0, 0xc0);
                objArgs.pub.colorPanelText = Color.Black;
            }

            if (IsBackBorder())
            {
                Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);
                DrawClass.PopBox2(g, x1, y1, x2, y2, brushback);
            }
			
			DisplayDescriptionX(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);
			DisplayDescriptionY(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);
			DisplayTagColor(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);
			DisplayGraph(g, gx1, gy1, gx2, gy2);
			DisplayCursor(g, x1, y1, x2, y2, gx1, gy1, gx2, gy2);
		}

		void DisplayGraph(Graphics g, int gx1, int gy1, int gx2, int gy2)
		{
            Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, gx1, gy1, gx2, gy2);
            //DrawClass.PushBox2(g, gx1, gy1, gx2, gy2, brush);

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE) DrawClass.PushBox2(g, gx1, gy1, gx2, gy2, brush); //CE 소스 수정시 제거.
            else if (IsBackBorder()) DrawClass.PushBox2(g, gx1, gy1, gx2, gy2, brush);  //20250206 PSU 배경판 옵션 추가
            else { DrawClass.gcls(g, gx1, gy1, gx2, gy2, brush); } //배경판 미사용 시 pushbox 제거.

			DisplayGraphGuideLine(g, gx1, gy1, gx2, gy2);

			int sizex, sizey;
			int x, y=0;
			double gaby;
			ushort j;
			int  l;
			int  pos;
			ANALOG_GRAPH_MEMBER member;
			TagAiClass ai = null;
			int point_radios;
			int view_y1;
			int view_y2;
			int view_sizey;
			bool moveto_flag = false;
			int move_x=0, move_y=0;
			Font font = MakeFont();
			int result_y;

			point_radios = (gy2-gy1+1)*objArgs.pub.wPointSize/1000;
			if(point_radios < 1)	point_radios = 1;

            sizex = gx2 - gx1; // +1을 안하는 것이 정확한 값이다. 10.2.1
			//sizex = gx2-gx1+1;
			sizey = gy2-gy1+1;

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (ANALOG_GRAPH_MEMBER)blockMember[l];
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
                    string str;

                    if (Tools.IsLangKorean())
                        str = "태그 없음";
                    else if (Tools.IsLangJapanese())
                        str = "タグない";
                    else if (Tools.IsLangChinese())
                        str = "没有标记";
                    else
                        str = "Tag not found.";

                    SafeException.SafeDrawString(g, str, font, brush, gx1 + 1, gy1 + 1);
                    continue;
                }

                view_y1 = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;
                view_y2 = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;
                view_sizey = view_y2 - view_y1 + 1;

                if (member.nType == EnumTagType.AI)
                {	// AI 일때만 경계치 표시
                    using (Pen hPenLimit = new Pen(member.color))
                    {

                    if ((member.wFlags & 0x0001) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.hihi - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        hPenLimit.Color = objArgs.pub.colorHiHi;
                        hPenLimit.Width = objArgs.pub.nThickHiHi;
                        hPenLimit.DashStyle = ObjectRectangle.GetDashStyle(objArgs.pub.nStyleHiHi);

                        g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    if ((member.wFlags & 0x0002) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.high - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        hPenLimit.Color = objArgs.pub.colorHigh;
                        hPenLimit.Width = objArgs.pub.nThickHigh;
                        hPenLimit.DashStyle = ObjectRectangle.GetDashStyle(objArgs.pub.nStyleHigh);

                        g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    if ((member.wFlags & 0x0004) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.low - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        hPenLimit.Color = objArgs.pub.colorLow;
                        hPenLimit.Width = objArgs.pub.nThickLow;
                        hPenLimit.DashStyle = ObjectRectangle.GetDashStyle(objArgs.pub.nStyleLow);

                        g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    if ((member.wFlags & 0x0008) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.lolo - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        hPenLimit.Color = objArgs.pub.colorLoLo;
                        hPenLimit.Width = objArgs.pub.nThickLoLo;
                        hPenLimit.DashStyle = ObjectRectangle.GetDashStyle(objArgs.pub.nStyleLoLo);

                        g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    } // using hPenLimit
                }

                using (Pen pen = new Pen(member.color, member.nLineThick))
                {

                pos = nBufPos + 1;
                moveto_flag = false;
                List<Point> continuousPoints = new List<Point>();

                for (j = 0; j < objArgs.wShowUnit; j++, pos++)
                {
                    pos %= objArgs.wShowUnit;

                    if (gaby == 0)	// protect divide by zero
                        y = 0;
                    else
                    {
                        if (member.point[pos].read_flag)
                        {	// 값을 읽었을 때만
                            if (member.nType == 0)
                            {	// AI
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

                                    double log_val = Math.Log(TagUtil.GetDisplayValue(ai, member.point[pos].val), objArgs.logarithmicScale.fBase);

                                    double log_gab = log_full - log_base;

                                    if (log_gab == 0)
                                        y = 0;
                                    else
                                        y = (int)((double)view_sizey * (log_val - log_base) / log_gab);
                                }
                                else
                                {
                                    y = (int)((double)view_sizey * (TagUtil.GetDisplayValue(ai, member.point[pos].val) - member.min_value) / gaby);
                                }
                            }
                            else if (member.nType == EnumTagType.DI)
                            {
                                if (member.point[pos].val > 0) y = (int)(view_sizey * 0.9);
                                else y = (int)(view_sizey * 0.1);
                            }
                            else
                            {
                                y = 1;
                            }
                        }
                    }

                    if (y < 0) y = 0;
                    if (y >= view_sizey) y = view_sizey - 1;

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
                                int x1, x2;

                                if (objArgs.wShowUnit - 1 == 0) x = 0;
                                else x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));

                                if (objArgs.bTimeDirToLeft == 1) move_x = x + gx1;
                                else move_x = gx2 - x;

                                if (j == 0)
                                {
                                    if (objArgs.bTimeDirToLeft == 1) { x1 = move_x; x2 = move_x + right_thick; }
                                    else { x1 = move_x - left_thick; x2 = move_x; }
                                }
                                else if (j == objArgs.wShowUnit - 1)
                                {
                                    if (objArgs.bTimeDirToLeft == 0) { x1 = move_x; x2 = move_x + right_thick; }
                                    else { x1 = move_x - left_thick; x2 = move_x; }
                                }
                                else { x1 = move_x - left_thick; x2 = move_x + right_thick; }

                                using (Brush barBrush = new SolidBrush(Color.FromArgb(180, member.color)))
                                using (Pen borderPen = new Pen(member.color, 1))
                                {
                                    if (member.bReverseY == 1)
                                    {
                                        result_y = view_y1 + y;
                                        if (result_y - view_y1 > 0)
                                        {
                                            DrawClass.gcls(g, x1, view_y1, x2, result_y, barBrush);
                                            if (x2 - x1 > 1) g.DrawRectangle(borderPen, x1, view_y1, x2 - x1, result_y - view_y1);
                                        }
                                    }
                                    else
                                    {
                                        result_y = view_y2 - y;
                                        if (view_y2 - result_y > 0)
                                        {
                                            DrawClass.gcls(g, x1, result_y, x2, view_y2, barBrush);
                                            if (x2 - x1 > 1) g.DrawRectangle(borderPen, x1, result_y, x2 - x1, view_y2 - result_y);
                                        }
                                    }
                                }
                            }
                            break;

                        case 2: // Area
                        case 3: // Spline
                        case 4: // StepLine
                            if (member.point[pos].read_flag)
                            {
                                if (member.bReverseY == 1) result_y = view_y1 + y;
                                else result_y = view_y2 - y;

                                if (objArgs.wShowUnit - 1 == 0) x = 0;
                                else x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));

                                int px;
                                if (objArgs.bTimeDirToLeft == 1) px = x + gx1;
                                else px = gx2 - x;

                                continuousPoints.Add(new Point(px, result_y));
                            }
                            else
                            {
                                if (continuousPoints.Count > 0)
                                {
                                    int baseY = member.bReverseY == 1 ? view_y1 : view_y2;
                                    FlushGraphSegment(g, pen, continuousPoints, member.nGraphType, baseY, member.color, member.nPointType, point_radios);
                                    continuousPoints.Clear();
                                }
                            }
                            break;

                        default: // Line (0)
                            if (member.point[pos].read_flag)
                            {
                                if (member.bReverseY == 1) result_y = view_y1 + y;
                                else result_y = view_y2 - y;

                                if (objArgs.wShowUnit - 1 == 0) x = 0;
                                else x = (int)((long)sizex * j / (objArgs.wShowUnit - 1));

                                if (objArgs.bTimeDirToLeft == 1)
                                {
                                    if (moveto_flag == false)
                                    { move_x = x + gx1; move_y = result_y; moveto_flag = true; }
                                    else
                                    { g.DrawLine(pen, move_x, move_y, x + gx1, result_y); move_x = x + gx1; move_y = result_y; }

                                    if (member.nPointType == 0) g.DrawLine(pen, x + gx1, result_y, x + gx1, result_y);
                                    else ObjectMultiTrend.DisplayPoint(g, x + gx1, result_y, member.color, member.nPointType, point_radios);
                                }
                                else
                                {
                                    if (moveto_flag == false)
                                    { move_x = gx2 - x; move_y = result_y; moveto_flag = true; }
                                    else
                                    { g.DrawLine(pen, move_x, move_y, gx2 - x, result_y); move_x = gx2 - x; move_y = result_y; }

                                    if (member.nPointType == 0) g.DrawLine(pen, gx2 - x, result_y, gx2 - x, result_y);
                                    else ObjectMultiTrend.DisplayPoint(g, gx2 - x, result_y, member.color, member.nPointType, point_radios);
                                }
                            }
                            else
                            {
                                moveto_flag = false;
                            }
                            break;
                    }
                }

                // Flush remaining points for Area/Spline/StepLine
                if (continuousPoints.Count > 0 && (member.nGraphType >= 2 && member.nGraphType <= 4))
                {
                    int baseY = member.bReverseY == 1 ? view_y1 : view_y2;
                    FlushGraphSegment(g, pen, continuousPoints, member.nGraphType, baseY, member.color, member.nPointType, point_radios);
                    continuousPoints.Clear();
                }
                } // using pen
            }
		}

        // Area/Spline/StepLine segment rendering helper
        private void FlushGraphSegment(Graphics g, Pen pen, List<Point> points, int graphType, int baseY, Color color, int pointType, int point_radios)
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
                if (points.Count >= 3)
                    g.DrawCurve(pen, points.ToArray(), 0.5f);
                else if (points.Count == 2)
                    g.DrawLine(pen, points[0], points[1]);
                else if (points.Count == 1)
                    g.DrawLine(pen, points[0].X, points[0].Y, points[0].X, points[0].Y);
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

            // Render point symbols
            if (pointType > 0)
            {
                foreach (Point pt in points)
                    ObjectMultiTrend.DisplayPoint(g, pt.X, pt.Y, color, pointType, point_radios);
            }
        }

		void DisplayCursor(Graphics g, int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2)
		{
			RECT r = new RECT();

			r.top = gy1+1;
			r.bottom = gy2;

			if(nCursorX1 == nCursorX2) 
			{
				r.left = nCursorX1*(gx2-gx1)/(objArgs.wShowUnit-1)+gx1;
				r.right = r.left+3;
			}
			else 
			{
				int x1, x2;
				x1 = nCursorX1;
				x2 = nCursorX2;
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				r.left  = x1*(gx2-gx1)/(objArgs.wShowUnit-1)+gx1;
				r.right = x2*(gx2-gx1)/(objArgs.wShowUnit-1)+gx1;
			}

			using (Brush brush_fill = new SolidBrush(Color.FromArgb(0x80, objArgs.lColorGuideLine)))
				g.FillRectangle(brush_fill, r.left, r.top, r.right-r.left, r.bottom-r.top);
			//DrawClass.InvertRect(g, r.left, r.top, r.right, r.bottom);

			if(bDisplayPointDate) 
			{
			}
		}

		void DisplayDescriptionX(Graphics g, int x1, int y1, int x2, int y2, int gx1, int gy1, int gx2, int gy2)
		{
			if(!IsDesX())	return;

			int   pos;
			ushort  w;
			string buf;
			RECT  r = new RECT();
			SizeF  size;
			int   time_devide = GetDisplayTimeDevide(gx1, gx2);
			int   displayed_pos_up = -1;
			//int   displayed_pos_down = -1;

			Font font = MakeFont();
			using (Brush brush = new SolidBrush(RunColorText))
			using (StringFormat format = new StringFormat())
			{

			DateTime dt = new DateTime(dtLastData.Ticks);

			if(objArgs.bTimeDirToLeft == 0) 
			{
				displayed_pos_up = -1;

				for(w = 0; w < objArgs.wShowUnit; w++) 
				{
					if((w%time_devide) != 0)		continue;

					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					if(objArgs.bDisplayByTime == 1) 
					{
						buf = String.Format("{0}:{1:00}:{2:00}", dt.Hour, dt.Minute, dt.Second);
						dt = dt.AddMilliseconds(-time_devide*objArgs.nDataTime);
					}
					else 
					{
						buf = String.Format("{0}", (w*objArgs.nDataTime)/1000);
					}

					// GetTextExtentPoint32(hdc, buf, strlen(buf), &size);
					size = g.MeasureString(buf, font);

					r.left  = (int)(pos-size.Width/2);
					if(r.left <= x1)	r.left = x1+1;
					r.right = (int)(r.left+size.Width);
					r.top   = gy2+1;
					r.bottom = r.top+nCharHeight;

					if(r.right >= x2) 
					{
						r.right = x2;
						r.left = (int)(r.right-size.Width);
					}

					if(r.left > displayed_pos_up) 
					{
						//DrawText(hdc, buf, strlen(buf), &r, DT_VCENTER | DT_SINGLELINE | DT_CENTER);
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Center;

						DrawClass.DrawText(g, buf, font, brush, r, format);
						displayed_pos_up = r.right;
					}
				}
			}
			else 
			{
				displayed_pos_up = x2+1;

				for(w = 0; w < objArgs.wShowUnit; w++) 
				{
					if((w%time_devide) != 0)		continue;

					pos = (int)(gx2-(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					if(objArgs.bDisplayByTime == 1) 
					{
						buf = String.Format("{0}:{1:00}:{2:00}", dt.Hour, dt.Minute, dt.Second);
						dt = dt.AddMilliseconds(-time_devide*objArgs.nDataTime);
					}
					else 
					{
						buf = String.Format("{0}", (w*objArgs.nDataTime)/1000);
					}

					size = g.MeasureString(buf, font);
					//GetTextExtentPoint32(hdc, buf, strlen(buf), &size);
					r.right  = (int)(pos+size.Width/2);
					if(r.right >= x2)	r.right = x2-1;
					r.left = (int)(r.right-size.Width);
					r.top   = gy2+1;
					r.bottom = r.top+nCharHeight;

					if(r.left <= x1) 
					{
						r.left = x1;
						r.right = (int)(r.right+size.Width);
					}

					if(r.right < displayed_pos_up) 
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Center;

						DrawClass.DrawText(g, buf, font, brush, r, format);
						displayed_pos_up = r.left;
					}
				}
			}
			} // using brush + format
		}

		void DisplayDescriptionY(Graphics g, int x1, int y1, int x2, int y2, int gx1, int gy1, int gx2, int gy2)
		{
			if(!IsDesY())	return;

			ANALOG_GRAPH_MEMBER member;
			ANALOG_GRAPH_MEMBER memberDisplay;	// 이것은 태그의 위치에서 오른쪽버튼을 누르면
			// 첫번째의 레벨값을 해당 태그로 바꾸어 주는 기능때문에 필요하다.
			int l;
			double view_full = 0, view_base = 0;

			double val;
			int   pos;
			int   i;
			string buf;
			RECT  r = new RECT();
			int   real_y;
			int	  real_size;
			RECT  rZone = new RECT();
			TagAiClass ai = null;
			TagDiClass di = null;
			Font font = MakeFont();

			using (StringFormat format = new StringFormat())
			{
			format.Alignment = StringAlignment.Far;
			format.LineAlignment = StringAlignment.Center;

			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];
				memberDisplay = (ANALOG_GRAPH_MEMBER)blockMember[l];

				if(member.nType != EnumTagType.AI && member.nType != EnumTagType.DI)		continue;
				if(member.nAxisPosition == 0)												continue;

				if(l == 0) 
				{
					if(l != dwFirstLevelDisplay && dwFirstLevelDisplay < blockMember.Count) 
					{
						memberDisplay = (ANALOG_GRAPH_MEMBER)blockMember[dwFirstLevelDisplay];
					}
				}

				if(member.nType == 0) 
				{
                    ai = GetRealTagAI(memberDisplay.tag, ref memberDisplay.nPos);
					view_full = memberDisplay.max_value;
					view_base = memberDisplay.min_value;
				}
				else 
				{	// DIGITAL INPUT tag
                    di = GetRealTagDI(memberDisplay.tag, ref memberDisplay.nPos);
					view_full = memberDisplay.max_value;
					view_base = memberDisplay.min_value;
				}

				if(member.nAxisPosition == 1) 
				{
					rZone.right = gx1-(member.nAxisCalcPos*(nCharWidth*objArgs.pub.nLevelDisplaySize))-2;
					rZone.left  = rZone.right-(nCharWidth*objArgs.pub.nLevelDisplaySize)+2;
				}
				else 
				{
					rZone.left   = gx2+(member.nAxisCalcPos*(nCharWidth*objArgs.pub.nLevelDisplaySize))+2;
					rZone.right  = rZone.left+(nCharWidth*objArgs.pub.nLevelDisplaySize)-2;
				}

				rZone.top = gy2-(member.nLevelTo)*(gy2-gy1)/100;
				rZone.bottom = gy2-(member.nLevelFrom)*(gy2-gy1)/100;

                using (Brush fillBrush = ObjectRectangle.MakePublicBrush(RunColorFill, rZone.left, rZone.top, rZone.right, rZone.bottom))
                {
                //DrawClass.PushBox2(g, rZone.left, rZone.top, rZone.right, rZone.bottom, brush); 

                if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE) DrawClass.PushBox2(g, rZone.left, rZone.top, rZone.right, rZone.bottom, fillBrush); //CE업데이트시 삭제.
                else if (IsBackBorder()) DrawClass.PushBox2(g, rZone.left, rZone.top, rZone.right, rZone.bottom, fillBrush); //20250206 PSU 배경판 옵션 사용 추가
                else { DrawClass.gcls(g, rZone.left - 2, rZone.top, rZone.right + 2, rZone.bottom, fillBrush); }
                } //배경판 미사용 시 pushbox 제거. 좌우 2씩 추가해야 요소 뒤 색상이 안보임.

				using (Brush brush = new SolidBrush(memberDisplay.color))
				{

				real_size = (member.nLevelTo-member.nLevelFrom)*(gy2-gy1)/100;
				if(member.bReverseY == 1)
					real_y = gy1+(member.nLevelFrom)*(gy2-gy1)/100;
				else
					real_y = gy2-(member.nLevelFrom)*(gy2-gy1)/100;

				for(i = 0; i < objArgs.wLevelDevide+1; i++) 
				{
					val = (view_full-view_base)*i/objArgs.wLevelDevide+view_base;

					if(member.bReverseY == 1)
						pos = real_y+(real_size)*i/(objArgs.wLevelDevide);
					else
						pos = real_y-(real_size)*i/(objArgs.wLevelDevide);

					if(member.nType == 0) 
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
					else 
					{
						if(i == 0)							buf = TagUtil.GetDesOFF(di);
						else if(i == objArgs.wLevelDevide)	buf = TagUtil.GetDesON(di);
						else								buf = "";	// empty
					}
			
					r.left  = rZone.left+1;
					r.right = rZone.right-1;

					if(member.bReverseY == 1) 
					{
						if(i == 0)	
							r.top = rZone.top+1;
						else if(i == objArgs.wLevelDevide) 
							r.top = rZone.bottom-nCharHeight;
						else
							r.top = pos-nCharHeight/2;
					}
					else 
					{
						if(i == 0)	
							r.top = rZone.bottom-nCharHeight;
						else if(i == objArgs.wLevelDevide) 
							r.top = rZone.top+1;
						else
							r.top = pos-nCharHeight/2;
					}
					r.bottom = r.top+nCharHeight;

					Rectangle rt = new Rectangle(r.left, r.top, r.right-r.left, r.bottom-r.top);

                    SafeException.SafeDrawString(g, buf, font, brush, rt, format);
				}
				} // using brush
			}
			} // using format
		}

		int GetDisplayTimeDevide(int gx1, int gx2)
		{
			if(bAutoGuideLine == false)		return objArgs.wTimeDevide;

			int devide = objArgs.wTimeDevide;
	
			devide = (gx2-gx1)/(nCharWidth*7);
			if(devide == 0)	devide = objArgs.wTimeDevide;
			else			devide = objArgs.wShowUnit/devide;

			if(devide > 30)	devide = 60;
	
			if(devide <= 0)	devide = 1;
			return devide;
		}

		void DisplayGraphGuideLine(Graphics g, int gx1, int gy1, int gx2, int gy2)
		{
			int   pos;
			int   i;
			int   time_devide = GetDisplayTimeDevide(gx1, gx2);

			using (Pen pen = new Pen(objArgs.lColorGuideLine, 1))
			{
				for(i = 1; i < objArgs.wLevelDevide; i++) 
				{
					pos = gy2-(gy2-gy1)*i/objArgs.wLevelDevide;
					g.DrawLine(pen, gx1+1, pos, gx2, pos);
				}

			int w;

			if(objArgs.bTimeDirToLeft == 0) 
			{
				for(w = 0; w < objArgs.wShowUnit; w++) 
				{
					if((w%time_devide) != 0)		continue;

					pos = (int)(gx1+(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			else 
			{
				for(w = 0; w < objArgs.wShowUnit; w++) 
				{
					if((w%time_devide) != 0)		continue;

					pos = (int)(gx2-(long)(gx2-gx1)*w/(objArgs.wShowUnit-1));

					g.DrawLine(pen, pos, gy1+1, pos, gy2);
				}
			}
			} // using pen
		}

		void DisplayTagColor(Graphics g, int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2)
		{
			if(!IsTagColor() && !IsTagMinMax() && !IsTagCurr() && !IsTagOldCurr())	return;
	
			int l;
			ANALOG_GRAPH_MEMBER member;
			RECT r = new RECT();
			int x;
			int y;
			int x1, y1, x2, y2;
			string buf;
			double val;
			double min, max;

			x = ox1+5;
			x1 = x;
			x2 = x+nCharWidth*8-1;

			if(IsDesX())	y = gy2+nCharHeight*2;
			else			y = gy2+3;

			r.left =  x1+1;
			r.right = x2;

			Font font = MakeFont();
            using (Brush brush = new SolidBrush(objArgs.pub.colorPanelText))
			using (StringFormat format = new StringFormat())
			{
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

            // 현재SmoothingMode 상태 저장, 고품질모드에서 antialias 적용으로 popbox2 사이에 빈공간이 생겨서 추가.  20250206 PSU 
            SmoothingMode prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            if (x2 < ox2)   // 판이 범위가 넘어서지 않도록 
            {
                if (IsTagColor())
                {
                    y1 = y;
                    y2 = y + nCharHeight + 1;
                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);
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
                        if (nCursorX1 == nCursorX2) buf = "资料值";
                        else buf = "平均值";
                    }
                    else
                    {
                        if (nCursorX1 == nCursorX2) buf = "Data";
                        else buf = "Ave";
                    }

                    DrawClass.DrawText(g, buf, font, brush, r, format);
                    y += nCharHeight + 2;
                }
            }

			x += nCharWidth*8;
	
			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];

				if(x+nCharWidth*member.nTagDisplaySize >= ox2-1)	break;

				if(IsDesX())	y = gy2+nCharHeight*2;
				else			y = gy2+3;

				x1 = x;
				x2 = x+nCharWidth*member.nTagDisplaySize-1;
		
				if(IsTagColor()) 
				{
					y1 = y;
					y2 = y+nCharHeight+1;
					if(member.visible == 0) 
					{
                        DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);
					}
					else 
					{
						DrawClass.PopBox2(g, x1, y1, x2, y2, RunColorFill.basic_color);// objArgs.lColorFill.basic_color); 250904 PSU 수정
                    }
					ObjectMultiTrend.DisplayPoint(g, x1+2+nCharWidth/2, y1+1+nCharHeight/2, member.color, member.nPointType, nCharWidth/2);
					if(member.visible == 0) 
					{
                        brush = new SolidBrush(objArgs.pub.colorPanelText);
					}
					else 
					{
						brush = new SolidBrush(member.color);
					}

					r.left = x1+nCharWidth*2;
					r.right = x2;
					r.top   = y1+1;
					r.bottom = y2;

					if(IsByDescription()) 
					{
						if(member.nType == EnumTagType.AI) 
						{
                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
							if(ai.description.Length == 0)
								buf = ai.tag;
							else
								buf = ai.description;
						}
						else if(member.nType == EnumTagType.DI) 
						{
                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
							if(di.description.Length == 0)
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
						if(member.nType == EnumTagType.AI) 
						{
                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
							buf = ai.tag;
						}
						else if(member.nType == EnumTagType.DI) 
						{
                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
							buf = di.tag;
						}
						else 
						{
							buf = member.tag;
						}
					}

					format.Alignment = StringAlignment.Near;
					format.LineAlignment = StringAlignment.Center;
					//format.FormatFlags |= StringFormatFlags.NoWrap;
					//format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

					DrawClass.DrawText(g, buf, font, brush, r, format);
					y += nCharHeight+2;
				}
				if(IsTagMinMax()) 
				{
					y1 = y;
					y2 = y+nCharHeight+1;
                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);
                    brush = new SolidBrush(objArgs.pub.colorPanelText);

					r.left = x1+1;
					r.right = x2;
					r.top   = y1+1;
					r.bottom = y2;
					if(member.nType == 0) 
					{
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
						if(nCursorX1 == nCursorX2) 
						{
							GetViewFullBase(ai, member, out max, out min);
							buf = TagUtil.AiValueToString(ai, max);
						}
						else 
						{
							GetCursorValue(member, out val, out min, out max); 
							buf = TagUtil.AiValueToString(ai, max);
						}
					}
					else 
					{
						buf = "";
					}

					format.Alignment = StringAlignment.Far;
					format.LineAlignment = StringAlignment.Center;

					DrawClass.DrawText(g, buf, font, brush, r, format);
					y += nCharHeight+2;

					y1 = y;
					y2 = y+nCharHeight+1;
                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);

                    brush = new SolidBrush(objArgs.pub.colorPanelText);
					r.left = x1+1;
					r.right = x2;
					r.top   = y1+1;
					r.bottom = y2;
					if(member.nType == 0) 
					{
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
						if(nCursorX1 == nCursorX2) 
						{
							GetViewFullBase(ai, member, out max, out min);
							buf = TagUtil.AiValueToString(ai, min);
						}
						else 
						{
							GetCursorValue(member, out val, out min, out max);
							buf = TagUtil.AiValueToString(ai, min);
						}
					}
					else 
					{
						buf = "";
					}

					format.Alignment = StringAlignment.Far;
					format.LineAlignment = StringAlignment.Center;

					DrawClass.DrawText(g, buf, font, brush, r, format);
					y += nCharHeight+2;
				}
				if(IsTagCurr()) 
				{
					y1 = y;
					y2 = y+nCharHeight+1;
                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);
                    brush = new SolidBrush(objArgs.pub.colorPanelText);

					r.left = x1+1;
					r.right = x2;
					r.top   = y1+1;
					r.bottom = y2;
					if(member.nType == EnumTagType.AI) 
					{
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
						buf = TagUtil.AiValueToString(ai, ai.curr);

						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Center;

						DrawClass.DrawText(g, buf, font, brush, r, format);
					}
					else if(member.nType == EnumTagType.DI) 
					{
                        TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
						if(di.curr == 1)	buf = TagUtil.GetDesON(di);
						else				buf = TagUtil.GetDesOFF(di);
						DrawClass.DrawText(g, buf, font, brush, r, format);
					}
					else 
					{
						buf = "???";
						DrawClass.DrawText(g, buf, font, brush, r, format);
					}

					y += nCharHeight+2;
				}
				if(IsTagOldCurr()) 
				{
					y1 = y;
					y2 = y+nCharHeight+1;
                    DrawClass.PopBox2(g, x1, y1, x2, y2, objArgs.pub.colorPanelBack);
                    brush = new SolidBrush(objArgs.pub.colorPanelText);

					r.left = x1+1;
					r.right = x2;
					r.top   = y1+1;
					r.bottom = y2;

					if(GetCursorValue(member, out val, out min, out max)) 
					{
						if(member.nType == EnumTagType.AI) 
						{
                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
							buf = TagUtil.AiValueToString(ai, val);
						}
						else 
						{
                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
							if(val > 0.5)		buf = TagUtil.GetDesON(di);
							else				buf = TagUtil.GetDesOFF(di);
						}
					}
					else 
					{
						buf = "***";
					}

					if(member.nType == 0) 
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
					y += nCharHeight+2;
				}

				x += nCharWidth*member.nTagDisplaySize;
			}

            // 이전 상태로 복원  20250206 PSU
            g.SmoothingMode = prevMode;
			} // using brush + format
		}

		bool GetCursorValue(ANALOG_GRAPH_MEMBER member, out double curr, out double min, out double max)
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

		bool GetZoneValue(ANALOG_GRAPH_MEMBER member, out double curr, out double min, out double max, int from, int to)
		{
			curr = 0;
			min  = 0;
			max  = 100;

			if(member.nType == EnumTagType.AI) 
			{
                TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
				GetViewFullBase(ai, member, out max, out min);
			}

			if(member.point == null)			return false;
	
			bool retn = false;

			if(from == to) 
			{
				int pos;
		
				if(objArgs.bTimeDirToLeft == 0) 
				{
					pos = (nBufPos-from);
					if(pos < 0)	pos += objArgs.wShowUnit;
				}
				else 
				{
					pos = (nBufPos+from+1);
					pos %= objArgs.wShowUnit;
				}

				if(pos < 0 || pos >= objArgs.wShowUnit)		return false;

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
				
				float val;
				int pos = nBufPos;

				for(i = from; i <= to; i++) 
				{
					if(objArgs.bTimeDirToLeft == 0) 
					{
						pos = (nBufPos-i);
						if(pos < 0)	pos += objArgs.wShowUnit;
					}
					else 
					{
						pos = (nBufPos+i+1);
						pos %= objArgs.wShowUnit;
					}

					if(!member.point[pos].read_flag)	continue;
					val = (float)member.point[pos].val;
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

		public override async Task EventTimerObject(System.Windows.Forms.Form form)
		{
			TagPublicClass pub;

			ANALOG_GRAPH_MEMBER member;
			int i;

			for(i = 0; i < blockMember.Count; i++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[i];
				pub = TagLib.GetStructPublic(member.tag, ref member.nPos);
				pub.NeedDataCurr = true;
			}

			if(!bRunFlag)		return;

            DateTime dt = DateTimeServer.Now;

			if(nOldMilliSec == -1) 
			{	// 최초 시작
				dtLastData = new DateTime(dt.Ticks);
				nOldMilliSec = dt.Second*1000+dt.Millisecond;
				nRemainMilliSec = 0;
				nBufPos = 0;
				//ReadFlagClear();
				FillData();
				InvalidateObject(form);
				return;
			}

			int curMilliSec = dt.Second*1000+dt.Millisecond;

			if(curMilliSec < nOldMilliSec) 
			{	// 분이 바뀌었다.
				nRemainMilliSec += (curMilliSec+60000)-nOldMilliSec;
			}
			else 
			{
				nRemainMilliSec += curMilliSec-nOldMilliSec;
			}

			nOldMilliSec = curMilliSec;

			bool data_flag = false;

			while(true) 
			{
				if(nRemainMilliSec >= objArgs.nDataTime) 
				{
					dtLastData = new DateTime(dt.Ticks);
					FillData();
					nRemainMilliSec -= objArgs.nDataTime;
					data_flag = true;
				}
				else 
				{ 
					break;
				}
			}

			if(data_flag) 
			{
				InvalidateObject(form);
			}

            await Task.CompletedTask;
        }

		void FillData()
		{
			nBufPos++;
			nBufPos %= objArgs.wShowUnit;
	
			int l;
			ANALOG_GRAPH_MEMBER member;
//			int read_count = 0;

			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];

				if(member.nType == 0) 
				{ 	// AI TAG
                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
					GetViewFullBase(ai, member, out member.max_value, out member.min_value);
				}
				else 
				{
					member.min_value = 0;
					member.max_value = 100;
				}

				if(member.point != null)	
				{
//					read_count = 0;

					if(member.nType == 0) 
					{ 	// AI TAG
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
						member.point[nBufPos].read_flag = true;
						member.point[nBufPos].val = ai.curr;
					}
					else 
					{
                        TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
						member.point[nBufPos].read_flag = true;
						member.point[nBufPos].val = di.curr;
					}

                    // 2020-5-6 multigraph에도 LogScale 추가
                    if (objArgs.logarithmicScale.bUse && member.min_value == 0) 
                    {
                        // 단위를 10의 배수에 맞게 잘라준다.
                        int read_count = 0;
                        int j;

                        for (j = 0; j < objArgs.wShowUnit; j++)
                        {
                            if (member.point[j].read_flag)
                            {
                                double val = member.point[j].val;

                                if (val == 0)
                                {

                                }
                                else
                                {
                                    double base_value = 1000000000;

                                    for (int k = 0; k < 20; k++)
                                    {
                                        if (val >= base_value)
                                        {
                                            val = base_value;
                                            break;
                                        }
                                        base_value /= 10;
                                    }

                                    if (read_count == 0)
                                    {
                                        member.min_value = val;
                                        //member.max_value = val;
                                    }
                                    else
                                    {
                                        if (val < member.min_value)
                                            member.min_value = val;
                                        //if (val > member.max_value) member.max_value = val;
                                    }
                                    read_count++;
                                }
                            }
                        }
                    }


				}
			}
	
		}

        async Task RunMouseScript(Form form, ScriptClass script)
        {
            script.SetHandOperation();	// 수동으로 출력한다.
            await script.RunAsync(form, this);
            if (script.IsError())
            {
                string message;
                message = script.GetError();
                MessageDisplay.Show(message);
            }
        }

		byte bMouseLeftOrRight = 0;

		public override async Task<bool> WmLeftButtonDown(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
		{
			if(bMouseCapture)	return false;
            if (!CheckResponseOnVisible()) return false;
	
			int mx = e.X;
			int my = e.Y;

			int gx1=0, gy1=0, gx2=0, gy2=0;
			int ox1=0, oy1=0, ox2=0, oy2=0;
	
			GetViewZoneWithRotation(ref ox1, ref oy1, ref ox2, ref oy2);
			GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);

			if(mx >= gx1 && my >= gy1 && mx <= gx2 && my <= gy2) 
			{
				int pos = (int)((float)(objArgs.wShowUnit-1)*(mx-gx1)/(gx2-gx1)+0.5);
				nCursorX1 = pos;
				nCursorX2 = pos;
				InvalidateObject(form);
				form.Capture = true;
				bMouseCapture = true;
				bMouseLeftOrRight = 0;

                // 일단 그래프 구역에서 클릭했을때만 스크립트가 동작하도록 했는데 전체영역을 지원하려면 각 구역마다 변수를 두는것도 좋을듯 하다. 2019-1-8
                if (eID.active == 1 && expandScript.scriptMouseLeftDown != null)
                {
                    await RunMouseScript(form, expandScript.scriptMouseLeftDown);
                }

				return true;
			}

			if(LeftButtonCheckTagZone(ox1, oy1, ox2, oy2, gx1, gy1, gx2, gy2, mx, my)) 
			{
				InvalidateObject(form);
				return true;
			}

			if(mx >= ox1 && my >= oy1 && mx <= ox2 && my <= oy2) 
			{
				/*
				CONFIG_MULTI_TREND config;

				config = LoadMultiTrendConfig();

				ConfigMultiTrend dialog = new ConfigMultiTrend(sClassName, config);
				if(dialog.ShowDialog() == DialogResult.OK) 
				{
					SaveMultiTrendConfig(config);

					LoadConfigAndExcute();
					ReadAllPoint();
					InvalidateObject(form);
				}
				*/

				return true;
			}

			return false;
		}

		public override async Task< bool> WmLeftButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			if(!bMouseCapture)	return false;

			form.Capture = false;
			bMouseCapture = false;

            if (eID.active == 1 && expandScript.scriptMouseLeftUp != null)
            {
                await RunMouseScript(form, expandScript.scriptMouseLeftUp);
                InvalidateObject(form);
            }

			return true;
		}

		//int nFirstX=0, nFirstY=0;
		//int nOldGab = 0;

		public override async Task<bool> WmMouseMove(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			if(!bMouseCapture) 
			{
				await base.ToolTipCheck(form, e);
				return false;
			}

			int mx = e.X;
			int my = e.Y;

			int gx1=0, gy1=0, gx2=0, gy2=0;
			int ox1=0, oy1=0, ox2=0, oy2=0;

            GetViewZoneWithRotation(ref ox1, ref oy1, ref ox2, ref oy2);
			GetGraphZone(ref gx1, ref gy1, ref gx2, ref gy2);

			if(bMouseLeftOrRight == 0) 
			{	// left button pressing
				int pos = (int)((float)(objArgs.wShowUnit-1)*(mx-gx1)/(gx2-gx1)+0.5);
				if(pos < 0)	pos = 0;
				if(pos >= objArgs.wShowUnit)	pos = objArgs.wShowUnit-1;
				nCursorX2 = pos;
				InvalidateObject(form);
			}
			else 
			{
				/*
				int calc_gab = (int)((float)(objArgs.wShowUnit-1)*(mx-nFirstX)/(gx2-gx1));
				int gab;
				int i;

				if(calc_gab != nOldGab) 
				{
					gab = calc_gab-nOldGab;
					nOldGab = calc_gab;

					if(objArgs.wTimeSelectOption == 0) 
					{	// min data
						if(gab > 0) 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddMinutes(-1);
						}
						else 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddMinutes(1);
						}
					}
					else if(objArgs.wTimeSelectOption == 1) 
					{	// hour data
						if(gab > 0) 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddHours(-1);
						}
						else 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddHours(1);
						}
					}
					else if(objArgs.wTimeSelectOption == 2) 
					{	// day data
						if(gab > 0) 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddDays(-1);
						}
						else 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddDays(1);
						}
					}
					else if(objArgs.wTimeSelectOption == 3) 
					{	// mon data
						if(gab > 0) 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddMonths(-1);
						}
						else 
						{
							for(i = 0; i < Math.Abs(gab); i++)	dtStartTime = dtStartTime.AddMonths(1);
						}
					}


					cGraphStartTimeMethod = 1;	// 수동.
					ReadAllPoint();

					InvalidateObject(form);
				}
				*/
			}

			return true;
		}

		bool LeftButtonCheckTagZone(int ox1, int oy1, int ox2, int oy2, int gx1, int gy1, int gx2, int gy2, int mx, int my)
		{
			if(!IsTagColor() && !IsTagMinMax() && !IsTagCurr() && !IsTagOldCurr())	return false;
	
			int  l;
			ANALOG_GRAPH_MEMBER member;
			RECT r = new RECT();
			int x;
			int y;
			int x1, y1, x2, y2;

			x = ox1+5;
			x1 = x;
			x2 = x+nCharWidth*8-1;

			if(IsDesX())	y = gy2+nCharHeight*2;
			else			y = gy2+3;

			r.left =  x1+1;
			r.right = x2;

			x += nCharWidth*8;
	
			for(l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];

				if(x+nCharWidth*member.nTagDisplaySize >= ox2-1)	break;

				if(IsDesX())	y = gy2+nCharHeight*2;
				else			y = gy2+3;

				x1 = x;
				x2 = x+nCharWidth*member.nTagDisplaySize-1;
				y1 = y;
		
				if(IsTagColor()) 
				{
					y += nCharHeight+2;
				}
				if(IsTagMinMax()) 
				{

					y += (nCharHeight+2)*2;
				}
				if(IsTagCurr()) 
				{
					y += nCharHeight+2;
				}
				if(IsTagOldCurr()) 
				{
					y += nCharHeight+2;
				}

				y2 = y;

				if(mx >= x1 && mx <= x2 && my >= y1 && my <= y2) 
				{
					member.visible = member.visible == 1 ? (sbyte)0 : (sbyte)1;
					return true;
				}

				x += nCharWidth*member.nTagDisplaySize;
			}

			return false;
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
			//writer.WriteLine("\twFlags,{0},", (int)objArgs.pub.wDisplayFlags);
			writer.WriteLine("\twTimeDevide,{0},", objArgs.wTimeDevide);
			writer.WriteLine("\twLevelDevide,{0},", objArgs.wLevelDevide);
			SaveObjectItem.GraphMember(writer, blockMember);
			//SaveObjectItem.GraphPointSize(writer, objArgs.pub.wPointSize);
			//SaveObjectItem.LevelDisplaySize(writer, objArgs.pub.nLevelDisplaySize);
            SaveObjectItem.GraphPublicArgs(writer, objArgs.pub);

			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.nDataTime);
			writer.Write("{0},", objArgs.bTimeDirToLeft);
			writer.Write("{0},", objArgs.bDisplayByTime);
			writer.WriteLine();
		} 


		public override void GetMultiSelectTagList(ArrayList block)
		{
			base.GetMultiSelectTagList(block);

			ANALOG_GRAPH_MEMBER member;
			for(int l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];
                TagUtil.AddTagList(block, member.tag, member.nType, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, "Graph Member");
			}
		}

		public override void SetMultiSelectTagList(ArrayList block)
		{
			base.SetMultiSelectTagList(block);

			ANALOG_GRAPH_MEMBER member;
			string tag;
			for(int l = 0; l < blockMember.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)blockMember[l];
				tag = member.tag;
				if(ObjectGroup.IsNeedUpdateTag(block, ref tag))
					member.tag = tag;
			}
		}
	}
}




 