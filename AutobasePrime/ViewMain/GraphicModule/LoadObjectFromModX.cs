using System;
using System.Drawing;
using NetTools.OldDefine;
using System.IO;
using NetTools;
using System.Collections;
using AutoLibLocal;
using AutoLib;

namespace GraphicModule 
{
	/// <summary>
	/// Summary description for LoadObject.
	/// </summary>
	public class LoadObjectFromModX
	{
		public ArrayList graphMember = new ArrayList();
		public ArrayList dbTrendMember = null;
        public ArrayList mdTrendMember = null;
		public ArrayList	 blockPoint = new ArrayList();		// Point, 를 합쳐놓음.
		public Color lTextColor;
        public BrushPublic lBackColor = new BrushPublic();
		public Color lLineColor;
		public BrushPublic lFillColor = new BrushPublic();
		public Color lGuideLineColor;
        public Color lCursorColor;
		public Color lOnColor;
		public Color lOffColor;
		public string 	sTagName = "";
		//string	sTagNameHeight; 사용하지 않는것 같아서 없앴다.
		public RECT	rRect = new RECT();
		//public RECT	rTagRect = new RECT();	MODX에서는 사용하지 않는다.
		public float	fStartAngle;
		public float	fEndAngle;
		public int		nAngleDirection;
		public int	nLocalMethod;
		//public int	nTagMouseResponse;
		int		nRealUnit;
		public int		nShowUnit;
		public ushort	wTimeDevide;
		public ushort	wLevelDevide;
		public ushort	wTimeSelectOption;
		//public ushort	wFlags;
		public int		nBackBox;			// 밑에 배경으로 까는 박스의 형태.

		public string 	sFileName;
		public string 	sFileNameOn;
		public string 	sFileNameOff;

        public bool     bMultiString = false;   // 이 클래스를 부르기 전에 true로 하면 여러줄을 붙여서 읽는다.
		public string 	sString;
        bool            bStringReaded = false;  // 여러줄일 때 첫번째 줄을 읽었느냐?

		public ObjectGeneral objGeneral = new ObjectGeneral();
		public string  sStringOption;	// 이속에 각각의 Object에 특화된 Option이 들어있다. 

		public int		nOverlayMethod;
        public int      nRotateFlip = 0;

        // CE 호환 Bitmap 병합 데이터 (ObjectBitmap으로 저장된 MergedBitmapSimple에서 사용)
        public int nMergedOriginalCount = 0;
        public byte[] mergedOriginalData = null;
		//public int		nTagDigitalOutputMethod;
		//public int		nTagDigitalOutputDelayTime;
		public MOUSE_RESPONSE_STRUCT mouseResponse = new MOUSE_RESPONSE_STRUCT();

		public EXPAND_ID_STRUCT eID;

		public ushort	wLineOption;
		//public ushort	wFillOption;
		public ushort	wLineThick;
		public bool		bLineThickLoaded;
		public int		nButtonDesignType;
		public int		nButtonRadius;
		//public ushort	wGraphPointSize;
		public EnumWindowStyleFlags dwWindowStyle;

		public ArrayList blockAnalogStatus = new ArrayList();
		public LOGFONT fontStruct;
		public TEXT_ALIGN align = new TEXT_ALIGN();
		//public ArrayList blockButtonDoutMember = new ArrayList();
		public ArrayList blockListData = new ArrayList();	
		public ArrayList blockCurve = new ArrayList();

		public ArrayList blockTwoTagMember = new ArrayList();
        public ArrayList blockTwoTagMemberList = new ArrayList();   // TestGraph에서는 이것이 0이면 위의 blockTwoTagMember를 사용한다.

        public ArrayList blockDonutChartMember = new ArrayList(); // DonutChart 용 member hsjeong 25-02-04

		//public bool bReadedTagRectFlag = false;
		public ScriptClass scriptLocal;

		//public int		nLevelDisplaySize = 7;

        public ObjectArgsGraphPublic argsGraphPublic = new ObjectArgsGraphPublic();

        ObjectCommonProperty ocp;

		public LoadObjectFromModX(ObjectCommonProperty o)
		{
			//
			// TODO: Add constructor logic here
			//
            ocp = o;

			fStartAngle = 0.0f;
			fEndAngle = 360.0f;
			nAngleDirection = 0;	// 시계방향.
			nLocalMethod = 0;
			//nTagMouseResponse = 0;
			lTextColor = Color.Black;
			lBackColor.basic_color = Color.White;
			lLineColor = Color.Black;
			lFillColor.basic_color = Color.Red;
			lOnColor = Color.Blue;
			lOffColor = Color.Black;
			lGuideLineColor = Color.DarkGray;
            lCursorColor = Color.DarkGray;
			rRect.left = 100;
			rRect.top = 100;
			rRect.right = 200;
			rRect.bottom = 200;
			//rTagRect.left = 150;
			//rTagRect.top = 150;
			//rTagRect.right = 250;
			//rTagRect.bottom = 250;
			nOverlayMethod = 0;
			sFileNameOn = "";
			sFileNameOff = "";
			sFileName = "";
			sString = "";
			nBackBox = 0;
			eID = new EXPAND_ID_STRUCT();
			eID.mod_type = EnumModType.modx;
			wLevelDevide = 2;
			wTimeSelectOption = 0;
			//wGraphPointSize = 10;
			align.x = 2;	// right
			align.y = 1;	// vcenter
			//wFlags = 0;
			dwWindowStyle = 0;

			fontStruct = new LOGFONT();
			sStringOption = "";
		}

        // 2007.6.26
        // 자신만의 클래스에서 사용하는 것들은 따로 클래스를 만들어서 사용하는 것이 속도와 구조적으로 나을 듯하다.
        protected virtual bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            return false;
        }

		public bool run(TextReader reader, string command)
		{
			string buf="";
			string one_line;
			int r=0, g=0, b=0, a=0;
			CommaTextReader comma = new CommaTextReader();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	return false;		// file end. file destroyed
				if(one_line.Length == 0)	continue;

				comma.Set(one_line);
				comma.GetString(ref buf);	

				if(String.Compare(buf, command) == 0) 
				{
					comma.GetString(ref buf);
					if(String.Compare(buf, "END") != 0)	return false;	// file destroyed

                    // ObjectTag에서 사용하는 것을 전체 클래스에서 사용하도록 이동함 2007.6.13
                    if (mouseResponse.bImsiResponseOnVisible == 1)
                    {
                        objGeneral.bResponseOnVisible = true;
                    }
					return true;
				}
                else if (ChildCheck(reader, buf, comma))
                {
                    // 해당 명령어가 있다.
                }
                else if (String.Compare(buf, "TextColor") == 0)
                {
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lTextColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "BackColor") == 0)
                {
                    /*
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lBackColor = Color.FromArgb(a, r, g, b);
                     */
                    lBackColor = LoadBrush(comma);
                }
                else if (String.Compare(buf, "LineColor") == 0)
                {
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lLineColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "FillColor") == 0)
                {
                    /*
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lFillColor = Color.FromArgb(a, r, g, b);
                     */
                    lFillColor = LoadBrush(comma);
                }
                else if (String.Compare(buf, "GuideLineColor") == 0)
                {
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lGuideLineColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "CursorColor") == 0)
                {
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lCursorColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "OnColor") == 0)
                {
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lOnColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "OffColor") == 0)
                {
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    comma.GetInt(ref a);
                    lOffColor = Color.FromArgb(a, r, g, b);
                }

                    // 아래는 태그 관련 object에서 사용하는 명령.
                else if (String.Compare(buf, "TagName") == 0)
                {
                    comma.GetString(ref sTagName);
                    sTagName = sTagName.Trim();
                }
                    /*
                else if (String.Compare(buf, "TagNameHeight") == 0)
                {
                    comma.GetString(ref sTagNameHeight);
                }*/
                
                else if (String.Compare(buf, "TagMouseResponse") == 0)
                {
                    comma.GetInt(ref mouseResponse.mouse_response);
                    comma.GetString(ref mouseResponse.scriptFilename);
                    comma.GetChar(ref mouseResponse.bImsiResponseOnVisible);

                    comma.GetChar(ref mouseResponse.cUserControlBoxUse);
                    comma.GetString(ref mouseResponse.sUserControlBoxModuleFile);
                    comma.GetString(ref mouseResponse.sUserControlBoxTitle);
                    comma.GetString(ref mouseResponse.sUserControlBoxDescription);
                }
                else if (String.Compare(buf, "TagDigitalOutputMethod") == 0)
                {
                    comma.GetInt(ref mouseResponse.do_method);
                }
                else if (String.Compare(buf, "TagDigitalOutputDelayTime") == 0)
                {
                    comma.GetInt(ref mouseResponse.do_delaytime);
                }
                else if (String.Compare(buf, "Rect") == 0)
                {
                    comma.GetInt(ref rRect.left);
                    comma.GetInt(ref rRect.top);
                    comma.GetInt(ref rRect.right);
                    comma.GetInt(ref rRect.bottom);
                }

                else if (String.Compare(buf, "StartAngle") == 0)
                {
                    comma.GetFloat(ref fStartAngle);
                }
                else if (String.Compare(buf, "EndAngle") == 0)
                {
                    comma.GetFloat(ref fEndAngle);
                }
                else if (String.Compare(buf, "AngleDirection") == 0)
                {
                    comma.GetInt(ref nAngleDirection);
                }
                else if (String.Compare(buf, "LocalMethod") == 0)
                {
                    comma.GetInt(ref nLocalMethod);
                }
                else if (String.Compare(buf, "FileName") == 0)
                {
                    comma.GetString(ref sFileName);
                }
                else if (String.Compare(buf, "GraphMember") == 0)
                {
                    ANALOG_GRAPH_MEMBER member = new ANALOG_GRAPH_MEMBER();
                    int cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    comma.GetInt(ref cr);
                    comma.GetInt(ref cg);
                    comma.GetInt(ref cb);
                    comma.GetInt(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);
                    comma.GetInt(ref member.nValueType);
                    comma.GetInt(ref member.nPointType);
                    comma.GetInt(ref member.nLineThick);
                    if (member.nLineThick < 1) member.nLineThick = 1;

                    comma.GetInt(ref member.nAxisPosition);
                    comma.GetInt(ref member.nLevelFrom);
                    comma.GetInt(ref member.nLevelTo);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetChar(ref member.bReverseY);
                    comma.GetHexWORD(ref member.wFlags);
                    comma.GetInt(ref member.nTimeShift);
                    comma.GetInt(ref member.nGraphType);
                    //	GraphMember,AI_0000,0,0,255,255,0,0,1,1,0,100,10,0,0000,0,2,0,
                    graphMember.Add(member);
                }
                
                else if (String.Compare(buf, "DbTrendMember") == 0)
                {
                    DB_TREND_MEMBER member = new DB_TREND_MEMBER();
                    int cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    comma.GetString(ref member.column);
                    member.column = member.column.Trim();

                    comma.GetInt(ref cr);
                    comma.GetInt(ref cg);
                    comma.GetInt(ref cb);
                    comma.GetInt(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);
                    comma.GetInt(ref member.nValueType);
                    comma.GetInt(ref member.nPointType);
                    comma.GetInt(ref member.nLineThick);
                    if (member.nLineThick < 1) member.nLineThick = 1;

                    comma.GetInt(ref member.nAxisPosition);
                    comma.GetInt(ref member.nLevelFrom);
                    comma.GetInt(ref member.nLevelTo);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetInt(ref member.nReverseY);
                    comma.GetHexWORD(ref member.wFlags);
                    comma.GetInt(ref member.nGraphType);
                    comma.GetString(ref member.sTable);
                    comma.GetString(ref member.sWhereString);
                    comma.GetString(ref member.sDescription);

                    if (dbTrendMember == null) dbTrendMember = new ArrayList();

                    dbTrendMember.Add(member);
                }
                else if (String.Compare(buf, "MdTrendMember") == 0) // Milli Data Trend Member
                {
                    MD_TREND_MEMBER member = new MD_TREND_MEMBER();
                    int cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    comma.GetString(ref member.column);
                    member.column = member.column.Trim();

                    comma.GetInt(ref cr);
                    comma.GetInt(ref cg);
                    comma.GetInt(ref cb);
                    comma.GetInt(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);
                    comma.GetInt(ref member.nValueType);
                    comma.GetInt(ref member.nPointType);
                    comma.GetInt(ref member.nLineThick);
                    if (member.nLineThick < 1) member.nLineThick = 1;

                    comma.GetInt(ref member.nAxisPosition);
                    comma.GetInt(ref member.nLevelFrom);
                    comma.GetInt(ref member.nLevelTo);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetInt(ref member.nReverseY);
                    comma.GetHexWORD(ref member.wFlags);
                    comma.GetInt(ref member.nGraphType);
                    comma.Skip();//.GetString(ref member.sTable);
                    comma.Skip();//.GetString(ref member.sWhereString);
                    comma.GetString(ref member.sDescription);

                    if (mdTrendMember == null) mdTrendMember = new ArrayList();

                    mdTrendMember.Add(member);
                }
                else if (String.Compare(buf, "MemberAnalogStatus") == 0)
                {
                    ANALOG_STATUS_STRUCT status = new ANALOG_STATUS_STRUCT();
                    comma.GetInt(ref status.type);
                    comma.GetString(ref status.filename);
                    blockAnalogStatus.Add(status);
                }
                else if (String.Compare(buf, "RealUnit") == 0)
                {
                    comma.GetInt(ref nRealUnit);
                }
                else if (String.Compare(buf, "ShowUnit") == 0)
                {
                    comma.GetInt(ref nShowUnit);
                }
                else if (String.Compare(buf, "wFlags") == 0)
                {
                    ushort flags = 0;
                    comma.GetWORD(ref flags);
                    argsGraphPublic.wDisplayFlags = (EnumDisplayFlag)flags;
                }
                else if (String.Compare(buf, "wTimeDevide") == 0)
                {
                    comma.GetWORD(ref wTimeDevide);
                }
                else if (String.Compare(buf, "wLevelDevide") == 0)
                {
                    comma.GetWORD(ref wLevelDevide);
                }
                else if (String.Compare(buf, "wTimeSelectOption") == 0)
                {
                    comma.GetWORD(ref wTimeSelectOption);
                }
                else if (String.Compare(buf, "FileNameOn") == 0)
                {
                    comma.GetString(ref sFileNameOn);
                }
                else if (String.Compare(buf, "FileNameOff") == 0)
                {
                    comma.GetString(ref sFileNameOff);
                }
                else if (String.Compare(buf, "OverlayMethod") == 0)
                {
                    comma.GetInt(ref nOverlayMethod);
                    comma.GetInt(ref nRotateFlip);
                }
                else if (String.Compare(buf, "MergedOriginalCount") == 0)
                {
                    comma.GetInt(ref nMergedOriginalCount);
                }
                else if (String.Compare(buf, "MergedOriginalData") == 0)
                {
                    string base64 = "";
                    comma.GetString(ref base64);
                    if (!string.IsNullOrEmpty(base64))
                    {
                        try { mergedOriginalData = Convert.FromBase64String(base64); }
                        catch { mergedOriginalData = null; }
                    }
                }
                else if (String.Compare(buf, "LogFont") == 0)
                {
                    int imsi = 0;
                    comma.GetString(ref fontStruct.lfFaceName);
                    comma.GetFloat(ref fontStruct.lfHeight);

                    fontStruct.style = 0;

                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Bold;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Italic;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Strikeout;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Underline;
                }
                else if (String.Compare(buf, "BackBox") == 0)
                {
                    comma.GetInt(ref nBackBox);
                }
                else if (String.Compare(buf, "ExpandScript") == 0)
                {
                    eID = LoadExpandScript(reader, buf);
                }
                else if (String.Compare(buf, "LocalScript") == 0)
                {
                    scriptLocal = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "TagScript") == 0)
                {
                    mouseResponse.scriptInclude = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "LineOption") == 0)
                {
                    comma.GetWORD(ref wLineOption);
                }
                else if (String.Compare(buf, "FillOption") == 0)
                {
                    if(!bBrushVersion)  // 10.0.2 이전 버전인 경우만 채움을 확인한다.
                        comma.GetInt(ref lFillColor.brush_type);
                }
                else if (String.Compare(buf, "LineThick") == 0)
                {
                    comma.GetWORD(ref wLineThick);
                    bLineThickLoaded = true;
                }
                else if (String.Compare(buf, "String") == 0)
                {
                    if (bMultiString)
                    {
                        if (this.bStringReaded)
                        {
                            string add = comma.GetStringWithSpace();
                            sString += "\r\n" + add;
                        }
                        else
                        {
                            sString = comma.GetStringWithSpace();
                            this.bStringReaded = true;
                        }
                    }
                    else
                    {
                        sString = comma.GetStringWithSpace();
                    }
                }
                else if (String.Compare(buf, "Point") == 0)
                {
                    int x = 0;
                    int y = 0;

                    comma.GetInt(ref x);
                    comma.GetInt(ref y);

                    Point point = new Point(x, y);

                    blockPoint.Add(point);
                }
                else if (String.Compare(buf, "Curve") == 0)
                {
                    CURVE_STRUCT curve = new CURVE_STRUCT();

                    ushort type = 0;

                    comma.GetHexWORD(ref type);
                    curve.type = (EnumCurveType)type;
                    comma.GetInt(ref curve.x[0]);
                    comma.GetInt(ref curve.y[0]);
                    comma.GetInt(ref curve.x[1]);
                    comma.GetInt(ref curve.y[1]);
                    comma.GetInt(ref curve.x[2]);
                    comma.GetInt(ref curve.y[2]);

                    blockCurve.Add(curve);
                }
                else if (String.Compare(buf, "wGraphPointSize") == 0)
                {
                    comma.GetInt(ref argsGraphPublic.wPointSize);
                }
                else if (String.Compare(buf, "TextAlign") == 0)
                {
                    comma.GetBYTE(ref align.x);
                    comma.GetBYTE(ref align.y);
                }
                else if (String.Compare(buf, "StringOption") == 0)
                {
                    comma.GetStringTotalRemain(ref sStringOption);
                }
                else if (String.Compare(buf, "ButtonDesignType") == 0)
                {
                    comma.GetInt(ref nButtonDesignType);
                }
                else if (String.Compare(buf, "ButtonRadius") == 0)
                {
                    comma.GetInt(ref nButtonRadius);
                }
                else if (String.Compare(buf, "ClassName") == 0)
                {
                    LoadObjectItem.LoadClassName(comma, objGeneral, ocp);
                }
                else if (String.Compare(buf, "Rotation") == 0)
                {
                    LoadObjectItem.LoadRotation(comma, objGeneral);
                }
                /* 2015-10-5 LoadobjectFromButtonDigitalOut으로 이동. MODX에서 한줄을 줄이는 것은 로딩속도를 전체적으로 올릴 수 있다.
                else if (String.Compare(buf, "ButtonDoutMember") == 0)
                {
                    BUTTON_DOUT_STRUCT member = new BUTTON_DOUT_STRUCT();

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    blockButtonDoutMember.Add(member);
                }*/
                // 9.2.0 이전에 RealTimeTestGraph에서 한개만 기준 그래프 한개만 사용할 때
                else if (String.Compare(buf, "TwoTagMember") == 0)
                {
                    TWO_TAG_MEMBER member = new TWO_TAG_MEMBER();

                    comma.GetString(ref member.sTag1);
                    comma.GetString(ref member.sTag2);
                    blockTwoTagMember.Add(member);
                }
                // 9.2.0 이후에 사용하는 형식 
                else if (String.Compare(buf, "TwoTagMemberList") == 0)
                {
                    TWO_TAG_MEMBER_LIST list = new TWO_TAG_MEMBER_LIST();

                    comma.GetColorFromARGB(ref list.color);
                    comma.GetInt(ref list.nLineThick);
                    comma.GetInt(ref list.nPointType);
                    comma.GetInt(ref list.nLevelMemberPos); //comma.Skip(); 이었다가 9.5.2 부터 nLevelMemberPos로 바뀜
                    sbyte flag = 0;
                    comma.GetChar(ref flag); //comma.Skip(); 이었다가 9.5.3 부터 bDisplayPointValue로 바뀜
                    list.bDisplayPointValue = (flag == 1);

                    comma.GetInt(ref list.nBasicGraphType); //reserved 였던 것 BasicGraph로 사용 24-06-27 hsjeong
                    comma.Skip();

                    while (true)
                    {
                        if (comma.IsEOS()) break;

                        TWO_TAG_MEMBER member = new TWO_TAG_MEMBER();

                        comma.GetString(ref member.sTag1);
                        if (member.sTag1 == "") break;  // end of x,y member 뒤에 추가할 수 있으므로 일단 끝을 만들어 놓았다.
                        comma.GetString(ref member.sTag2);
                        list.arrayMember.Add(member);
                    }
                    blockTwoTagMemberList.Add(list);
                }
                else if (String.Compare(buf, "WindowStyle") == 0)
                {
                    uint flags = 0;
                    comma.GetHexDWORD(ref flags);
                    dwWindowStyle = (EnumWindowStyleFlags)flags;
                }
                else if (String.Compare(buf, "ListData") == 0)
                {
                    string data = "";
                    comma.GetString(ref data);
                    blockListData.Add(data);
                }
                else if (String.Compare(buf, "LevelDisplaySize") == 0)
                {
                    comma.GetInt(ref argsGraphPublic.nLevelDisplaySize);
                }
                else if (String.Compare(buf, "GraphPublicArgs") == 0)
                {
                    ushort flags = 0;
                    comma.GetWORD(ref flags);
                    argsGraphPublic.wDisplayFlags = (EnumDisplayFlag)flags;
                    comma.GetInt(ref argsGraphPublic.wPointSize);
                    comma.GetInt(ref argsGraphPublic.nLevelDisplaySize);

                    uint argb = 0;
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorHiHi = Color.FromArgb((int)argb);
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorHigh = Color.FromArgb((int)argb);
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorLow = Color.FromArgb((int)argb);
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorLoLo = Color.FromArgb((int)argb);

                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorPanelBack = Color.FromArgb((int)argb);  //20250204 PSU 추가
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorPanelText = Color.FromArgb((int)argb);

                    if (argsGraphPublic.colorPanelBack.ToArgb() == 0 && argsGraphPublic.colorPanelText.ToArgb() == 0)
                    {
                        // 0값으로 같다면(설정이 안되어 있다면) 기본값으로 재설정
                        argsGraphPublic.colorPanelBack = Color.FromArgb(0xF0, 0xF0, 0xF0);
                        argsGraphPublic.colorPanelText = Color.Black;
                    }


                    //writer.WriteLine("\tGraphPublicArgs,{0},{1},{2},{3:X08},{4:X08},{5:X08},{6:X08}", (int)pub.wDisplayFlags, pub.wPointSize, pub.nLevelDisplaySize, pub.colorHiHi, pub.colorHigh, pub.colorLow, pub.colorLoLo);
                    
                }
                else
                {

                }
			}
		}

        bool bBrushVersion = false;

        BrushPublic LoadBrush(CommaTextReader comma)
        {
            int r=0, g=0, b=0, a = 0;
            int type = 1;

            comma.GetInt(ref r);
            comma.GetInt(ref g);
            comma.GetInt(ref b);
            comma.GetInt(ref a);

            if (comma.IsEOS())
            {
                BrushSolid brush = new BrushSolid();
                brush.basic_color = Color.FromArgb(a, r, g, b);
                brush.brush_type = type;
                return brush;
            }

            bBrushVersion = true;
            comma.GetInt(ref type);

            if (type == 2)
            {
                /*
                for (int i = 0; i < brush.stops.Count; i++)
                {
                    members += string.Format("{0}:{1:X08}:", brush.stops[i].offset, brush.stops[i].color.ToArgb());
                }
                writer.Write("{0},{1},{2},{3},{4},{5},{6},{7},", brush.spread_method, brush.stops.Count, members, brush.pStart.X, brush.pStart.Y, brush.pEnd.X, brush.pEnd.Y);
                */

                BrushLinearGradient brush = new BrushLinearGradient();
                brush.basic_color = Color.FromArgb(a, r, g, b);
                brush.brush_type = type;

                comma.GetInt(ref brush.spread_method);
                int count = 0;
                comma.GetInt(ref count);
                string members = "";
                comma.GetString(ref members);

                float x = 0, y = 0;

                comma.GetFloat(ref x);
                comma.GetFloat(ref y);
                brush.pStart = new PointF(x, y);
                comma.GetFloat(ref x);
                comma.GetFloat(ref y);
                brush.pEnd = new PointF(x, y);

                brush.stops = new System.Collections.Generic.List<BrushGradientStop>();

                CommaTextReader commastop = new CommaTextReader();
                commastop.SetBlockCode(':');
                commastop.Set(members);
                
                for (int i = 0; i < count; i++)
                {
                    BrushGradientStop stop = new BrushGradientStop();
                    commastop.GetFloat(ref stop.offset);
                    uint argb = 0;
                    commastop.GetHexDWORD(ref argb);
                    stop.color = Color.FromArgb((int)argb);

                    brush.stops.Add(stop);
                }

                return brush;
            }
            else
            {
                BrushSolid brush = new BrushSolid();
                brush.basic_color = Color.FromArgb(a, r, g, b);
                brush.brush_type = type;
                return brush;
            }
        }

		public static EXPAND_ID_STRUCT LoadExpandScript(TextReader reader, string command)
		{
            EXPAND_ID_STRUCT eID;

            eID = new EXPAND_ID_STRUCT();
            eID.mod_type = EnumModType.modx;

			string one_line;
			string buf="";
            CommaTextReader comma = new CommaTextReader();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				
				comma.Set(one_line);
				comma.GetString(ref buf);
				if(String.Compare(command, buf, true) == 0) // END
				{
					break;
				}
				else if(String.Compare(buf, "Active", true) == 0) 
				{
					comma.GetChar(ref eID.active);
				}
				else if(String.Compare(buf, "ScriptSizeWidth", true) == 0) 
				{
                    if (eID.expand.scriptSizeWidth == null)
                    {
                        eID.expand.scriptSizeWidth = new ExpandBasicAndScript();
                    }

					eID.expand.scriptSizeWidth.script = LoadOneScript(reader, buf);
				}
                else if (String.Compare(buf, "ScriptSizeHeight", true) == 0)
                {
                    if (eID.expand.scriptSizeHeight == null)
                    {
                        eID.expand.scriptSizeHeight = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptSizeHeight.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptLocationX", true) == 0)
                {
                    if (eID.expand.scriptLocationX == null)
                    {
                        eID.expand.scriptLocationX = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptLocationX.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptLocationY", true) == 0)
                {
                    if (eID.expand.scriptLocationY == null)
                    {
                        eID.expand.scriptLocationY = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptLocationY.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ExpandSizeWidth", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptSizeWidth);
                }
                else if (String.Compare(buf, "ExpandSizeHeight", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptSizeHeight);
                }
                else if (String.Compare(buf, "ExpandLocationX", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptLocationX);
                }
                else if (String.Compare(buf, "ExpandLocationY", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptLocationY);
                }
				
				else if(String.Compare(buf, "ScriptEventKeyDown", true) == 0) 
				{
					eID.expand.scriptEventKeyDown = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptEventSelChange", true) == 0) 
				{
					eID.expand.scriptEventSelChange = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptMouseDown", true) == 0) 
				{
					eID.expand.scriptMouseLeftDown = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptMouseUp", true) == 0) 
				{
					eID.expand.scriptMouseLeftUp = LoadOneScript(reader, buf);
				}
                else if (String.Compare(buf, "ScriptMouseRightDown", true) == 0)
                {
                    eID.expand.scriptMouseRightDown = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseRightUp", true) == 0)
                {
                    eID.expand.scriptMouseRightUp = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseEnter", true) == 0)
                {
                    eID.expand.scriptMouseEnter = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseLeave", true) == 0)
                {
                    eID.expand.scriptMouseLeave = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseMove", true) == 0)
                {
                    eID.expand.scriptMouseMove = LoadOneScript(reader, buf);
                }

				else if(String.Compare(buf, "ScriptMouseZone", true) == 0) 
				{
					eID.expand.structMouseZone = LoadOneMouseZone(reader, buf);
				}
				else if(String.Compare(buf, "ScriptVisible", true) == 0) 
				{
                    if (eID.expand.scriptVisible == null)
                    {
                        eID.expand.scriptVisible = new ExpandBasicAndScript();
                    }
					eID.expand.scriptVisible.script = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptBlinking", true) == 0) 
				{
                    if (eID.expand.scriptBlinking == null)
                    {
                        eID.expand.scriptBlinking = new ExpandBasicAndScript();
                    }
					eID.expand.scriptBlinking.script = LoadOneScript(reader, buf);
				}
                else if (String.Compare(buf, "ExpandVisible", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptVisible);
                }
                else if (String.Compare(buf, "ExpandBlinking", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptBlinking);
                }

				else if(String.Compare(buf, "ScriptSliderHorz", true) == 0) 
				{
					eID.expand.sliderHorz = LoadOneSlider(reader, buf);
				}
				else if(String.Compare(buf, "ScriptSliderVert", true) == 0) 
				{
					eID.expand.sliderVert = LoadOneSlider(reader, buf);
				}

				else if(String.Compare(buf, "ScriptColorLine", true) == 0) 
				{
                    if (eID.expand.scriptColorLine == null)
                    {
                        eID.expand.scriptColorLine = new ExpandBasicAndScript();
                    }
					eID.expand.scriptColorLine.script = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptColorFill", true) == 0)
                {
                    if (eID.expand.scriptColorFill == null)
                    {
                        eID.expand.scriptColorFill = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorFill.script = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptColorText", true) == 0)
                {
                    if (eID.expand.scriptColorText == null)
                    {
                        eID.expand.scriptColorText = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorText.script = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptColorBack", true) == 0)
                {
                    if (eID.expand.scriptColorBack == null)
                    {
                        eID.expand.scriptColorBack = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorBack.script = LoadOneScript(reader, buf);
				}
				else if(String.Compare(buf, "ScriptThickLine", true) == 0)
                {
                    if (eID.expand.scriptThickLine == null)
                    {
                        eID.expand.scriptThickLine = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptThickLine.script = LoadOneScript(reader, buf);
				}
                else if (String.Compare(buf, "ScriptAnimationSpeed", true) == 0)
                {
                    if (eID.expand.scriptAnimationSpeed == null)
                    {
                        eID.expand.scriptAnimationSpeed = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptAnimationSpeed.script = LoadOneScript(reader, buf);
                }

                else if (String.Compare(buf, "ExpandColorLine", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorLine);
                }
                else if (String.Compare(buf, "ExpandColorFill", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorFill);
                }
                else if (String.Compare(buf, "ExpandColorText", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorText);
                }
                else if (String.Compare(buf, "ExpandColorBack", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorBack);
                }
                else if (String.Compare(buf, "ExpandThickLine", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptThickLine);
                }
                else if (String.Compare(buf, "ExpandAnimationSpeed", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptAnimationSpeed);
                }

                else if (String.Compare(buf, "ScriptRotate", true) == 0)
                {
                    if (eID.expand.scriptRotate == null)
                    {
                        eID.expand.scriptRotate = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptRotate.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ExpandRotate", true) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptRotate);
                }

				else 
				{
					
				}
			}

            return eID;
		}

        static void LoadOneConversion(TextReader reader, string command, ref ExpandBasicAndScript expand)
        {
            if (expand == null)
            {
                expand = new ExpandBasicAndScript();
            }

            string one_line;
            string buf = "";
            CommaTextReader comma = new CommaTextReader();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref buf);

                if (String.Compare(command, buf, true) == 0) // END
                {
                    break;
                }
                else if (String.Compare(buf, "ExpandType", true) == 0)
                {
                    int type = 0;
                    comma.GetInt(ref type);

                    if (type == 0) expand.eExpandType = EnumExpandType.Basic;
                    else
                    {
                        expand.eExpandType = EnumExpandType.Script;
                    }
                }
                else if (String.Compare(buf, "ExpandBasicConversion", true) == 0)
                {
                    ExpandBasicConversion con = new ExpandBasicConversion();

                    comma.GetString(ref con.sTag);

                    if (con.sTag.Length > 1000)
                    {
                        con.sTag = con.sTag.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    comma.GetInt(ref con.nBasicConversionType);
                    comma.GetFloat(ref con.fBasicSourceMin);
                    comma.GetFloat(ref con.fBasicSourceMax);
                    comma.GetFloat(ref con.fBasicTargetMin);
                    comma.GetFloat(ref con.fBasicTargetMax);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicVisible", true) == 0)
                {
                    ExpandBasicVisible con = new ExpandBasicVisible();

                    comma.GetString(ref con.sTag);

                    if (con.sTag.Length > 1000)
                    {
                        con.sTag = con.sTag.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    comma.GetInt(ref con.condition);
                    comma.GetString(ref con.value);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicBlinking", true) == 0)
                {
                    ExpandBasicBlinking con = new ExpandBasicBlinking();

                    comma.GetString(ref con.sTag);

                    if (con.sTag.Length > 1000)
                    {
                        con.sTag = con.sTag.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    comma.GetInt(ref con.condition);
                    comma.GetString(ref con.value);
                    comma.GetInt(ref con.cycle);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicOnlyTag", true) == 0)
                {
                    ExpandBasicOnlyTag con = new ExpandBasicOnlyTag();

                    comma.GetString(ref con.sTag);

                    if (con.sTag.Length > 1000)
                    {
                        con.sTag = con.sTag.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicColorMember", true) == 0)
                {
                    if (expand.basic == null)
                    {
                        expand.basic = new ExpandBasicColor();
                    }

                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member = new ExpandBasicColorMember();
                    byte flag = 0;

                    comma.GetBYTE(ref flag);
                    member.active = (flag == 1);
                    comma.Skip();   // bDefault
                    comma.GetString(ref member.sTag);

                    if (member.sTag.Length > 1000)
                    {
                        member.sTag = member.sTag.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    comma.GetInt(ref member.condition);
                    comma.GetString(ref member.value);

                    if (member.value.Length > 1000)
                    {
                        member.value = member.value.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    int cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetInt(ref cr);
                    comma.GetInt(ref cg);
                    comma.GetInt(ref cb);
                    comma.GetInt(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);

                    con.member.Add(member);
                }
                else if (String.Compare(buf, "ExpandBasicRotate", true) == 0)
                {
                    ExpandBasicRotate con = new ExpandBasicRotate();

                    comma.GetString(ref con.sTag);

                    if (con.sTag.Length > 1000)
                    {
                        con.sTag = con.sTag.Substring(0, 1000); // CommaTextReader를 사용해야 하는데 CommaBlockString을 사용해서 ," 이 들어있는경우 저장할 때 기하급수적으로 늘어난다. 2015-8-28 추가
                    }

                    comma.GetInt(ref con.nRotateAxis);
                    comma.GetFloat(ref con.fBasicSourceMin);
                    comma.GetFloat(ref con.fBasicSourceMax);
                    comma.GetFloat(ref con.fBasicTargetMin);
                    comma.GetFloat(ref con.fBasicTargetMax);
                    comma.GetInt(ref con.nDirection);

                    expand.basic = con;
                }
                /*
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    writer.WriteLine("\t\tExpandBasicVisible,{0},{1},{2},", con.sTag, con.condition, con.value);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    writer.WriteLine("\t\tExpandBasicVisible,{0},{1},{2},{3}", con.sTag, con.condition, con.value, con.cycle);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag con = (ExpandBasicOnlyTag)expand.basic;
                    writer.WriteLine("\t\tExpandBasicOnlyTag,{0},", con.sTag);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;
                    for (int i = 0; i < con.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)con.member[i];
                        writer.Write("\t\tExpandBasicColorMember,{0},", member.active ? 1 : 0);
                        writer.Write("{0},", "");   // imsi tag 애니매이션과 일치하기 위해서
                        writer.Write("{0},", member.sTag);
                        writer.Write("{0},", member.condition);
                        writer.Write("{0},", member.value);
                        writer.Write("{0},{1},{2},{3},", member.color.R, member.color.G, member.color.B, member.color.A);
                        writer.WriteLine();
                    }
                }*/
                else
                {

                }
            }
        }

		public static ScriptClass LoadOneScript(TextReader reader, string command)
		{
			ScriptClass script = new ScriptClass();

			script.LoadFromMODX(reader, command);

			if(script.Script.Length == 0)
				return null;

			return script;
		}

		static EXPAND_MOUSE_ZONE LoadOneMouseZone(TextReader reader, string command)
		{
			EXPAND_MOUSE_ZONE script = new EXPAND_MOUSE_ZONE();

			script.LoadFromMODX(reader, command);

			return script;
		}

		static EXPAND_SLIDER_STRUCT LoadOneSlider(TextReader reader, string command)
		{
			EXPAND_SLIDER_STRUCT script = new EXPAND_SLIDER_STRUCT();

			script.LoadFromMODX(reader, command);

			return script;
		}
	}
}


