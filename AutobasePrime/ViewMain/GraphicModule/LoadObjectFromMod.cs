using System;
using System.Drawing;
using NetTools.OldDefine;
using System.IO;
using NetTools;
using System.Collections;
using AutoLibLocal;

namespace GraphicModule 
{
	/// <summary>
	/// Summary description for LoadObject.
	/// </summary>
	public class LoadObjectFromMod
	{
		//readonly int  ON = 1;
		//readonly int  OFF = 0;

		public ArrayList graphMember = new ArrayList();
		public ArrayList graphMemberXY = new ArrayList();
		//Block	 demandMember;
		public ArrayList	 blockPoint = new ArrayList();		// Point, 를 합쳐놓음.
		public Color lTextColor;
        public BrushPublic lBackColor = new BrushPublic();
		public Color lLineColor;
        public BrushPublic lFillColor = new BrushPublic();
		public Color lGuideLineColor;
		public Color lOnColor;
		public Color lOffColor;
		public string 	sTagName;
		//string	sTagNameHeight;
		public RECT	rRect = new RECT();
		public RECT	rTagRect = new RECT();
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
		public ushort	wFlags;
		public int		nBackBox;			// 밑에 배경으로 까는 박스의 형태.

		public string 	sFileName;
		public string 	sFileNameOn;
		public string 	sFileNameOff;
		public string 	sString;
		public ObjectGeneral objGeneral = new ObjectGeneral();
		public string  sStringOption;	// 이속에 각각의 Object에 특화된 Option이 들어있다. 

		public int		nOverlayMethod;
		//public int		nTagDigitalOutputMethod;
		//public int		nTagDigitalOutputDelayTime;

		public EXPAND_ID_STRUCT eID;
		public MOUSE_RESPONSE_STRUCT mouseResponse = new MOUSE_RESPONSE_STRUCT();

		public ushort	wLineOption;
		//public ushort	wFillOption;
		public ushort	wLineThick;
		public ushort	wGraphPointSize;
		public EnumWindowStyleFlags 	dwWindowStyle;

		public ArrayList blockAnalogStatus = new ArrayList();
		public LOGFONT fontStruct;
		public TEXT_ALIGN align = new TEXT_ALIGN();
		public ArrayList blockButtonDoutMember = new ArrayList();
		public ArrayList blockListData = new ArrayList();	
		public ArrayList blockCurve = new ArrayList();
		public ArrayList blockTwoTagMember = new ArrayList();

		public bool bReadedTagRectFlag = false;

		public int		nLevelDisplaySize = 7;

		public LoadObjectFromMod()
		{
			//
			// TODO: Add constructor logic here
			//
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
			rRect.left = 100;
			rRect.top = 100;
			rRect.right = 200;
			rRect.bottom = 200;
			rTagRect.left = 150;
			rTagRect.top = 150;
			rTagRect.right = 250;
			rTagRect.bottom = 250;
			nOverlayMethod = 0;
			sFileNameOn = "";
			sFileNameOff = "";
			sFileName = "";
			sString = "";
			nBackBox = 0;
			eID = new EXPAND_ID_STRUCT();
			eID.mod_type = EnumModType.mod;
			wLevelDevide = 2;
			wTimeSelectOption = 0;
			wGraphPointSize = 10;
			align.x = 2;	// right
			align.y = 1;	// vcenter
			wFlags = 0;
			dwWindowStyle = 0;

			fontStruct = new LOGFONT();
			sStringOption = "";
		}

		public bool run(TextReader reader, string command)
		{
			string buf="";
			int r=0, g=0, b=0;
			CommaBlockString comma = new CommaBlockString();

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	return false;		// file end
				if(buf.Length == 0)	continue;

				comma.Set(buf);
				comma.GetString(ref buf);	

				if(String.Compare(buf, command) == 0) 
				{
					comma.GetString(ref buf);
					if(String.Compare(buf, "END") != 0)	return false;	// file destroyed
					return true;
				}
				else if(String.Compare(buf, "TextColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lTextColor = Color.FromArgb(r, g, b);
				}
				else if(String.Compare(buf, "BackColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lBackColor.basic_color = Color.FromArgb(r, g, b);
				}
				else if(String.Compare(buf, "LineColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lLineColor = Color.FromArgb(r, g, b);
				}
				else if(String.Compare(buf, "FillColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lFillColor.basic_color = Color.FromArgb(r, g, b);
				}
				else if(String.Compare(buf, "GuideLineColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lGuideLineColor = Color.FromArgb(r, g, b);
				}
				else if(String.Compare(buf, "OnColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lOnColor = Color.FromArgb(r, g, b);
				}
				else if(String.Compare(buf, "OffColor") == 0) 
				{
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					lOffColor = Color.FromArgb(r, g, b);
				}

					// 아래는 태그 관련 object에서 사용하는 명령.
				else if(String.Compare(buf, "TagName") == 0) 
				{
					comma.GetString(ref sTagName);
					sTagName = sTagName.Trim();
				}
                    /*
				else if(String.Compare(buf, "TagNameHeight") == 0) 
				{
					comma.GetString(ref sTagNameHeight);
				}*/
				else if(String.Compare(buf, "TagRect") == 0) 
				{
					comma.GetInt(ref rTagRect.left); 
					comma.GetInt(ref rTagRect.top);
					comma.GetInt(ref rTagRect.right);
					comma.GetInt(ref rTagRect.bottom);

					bReadedTagRectFlag = true;
				}
				else if(String.Compare(buf, "TagMouseResponse") == 0) 
				{
					comma.GetInt(ref mouseResponse.mouse_response);
					comma.GetString(ref mouseResponse.scriptFilename);
				}
				else if(String.Compare(buf, "TagDigitalOutputMethod") == 0) 
				{
					comma.GetInt(ref mouseResponse.do_method);
				}
				else if(String.Compare(buf, "TagDigitalOutputDelayTime") == 0) 
				{
					comma.GetInt(ref mouseResponse.do_delaytime);
				}
				else if(String.Compare(buf, "Rect") == 0) 
				{
					comma.GetInt(ref rRect.left);
					comma.GetInt(ref rRect.top);
					comma.GetInt(ref rRect.right);
					comma.GetInt(ref rRect.bottom);
				}

				else if(String.Compare(buf, "StartAngle") == 0) 
				{
					comma.GetFloat(ref fStartAngle);
				}
				else if(String.Compare(buf, "EndAngle") == 0) 
				{
					comma.GetFloat(ref fEndAngle);
				}
				else if(String.Compare(buf, "AngleDirection") == 0) 
				{
					comma.GetInt(ref nAngleDirection);
				}
				else if(String.Compare(buf, "LocalMethod") == 0) 
				{
					comma.GetInt(ref nLocalMethod);
				}
				else if(String.Compare(buf, "FileName") == 0) 
				{
					comma.GetString(ref sFileName);
				}
				else if(String.Compare(buf, "GraphMember") == 0)
				{		
					ANALOG_GRAPH_MEMBER member = new ANALOG_GRAPH_MEMBER();
					int cr=0, cg=0, cb=0;

					comma.GetString(ref member.tag);
					member.tag = member.tag.Trim();
					comma.GetInt(ref cr);
					comma.GetInt(ref cg);
					comma.GetInt(ref cb);
					member.color = Color.FromArgb(cr, cg, cb);
					comma.GetInt(ref member.nValueType);
					comma.GetInt(ref member.nPointType);
					comma.GetInt(ref member.nLineThick);
					if(member.nLineThick < 1)	member.nLineThick = 1;

					comma.GetInt(ref member.nAxisPosition);
					comma.GetInt(ref member.nLevelFrom);
					comma.GetInt(ref member.nLevelTo);
					comma.GetInt(ref member.nTagDisplaySize);
					comma.GetChar(ref member.bReverseY);
					comma.GetHexWORD(ref member.wFlags);

					graphMember.Add(member);
				}
				else if(String.Compare(buf, "GraphMemberXY") == 0) 
				{
					XY_GRAPH_MEMBER member = new XY_GRAPH_MEMBER();
					comma.GetString(ref member.tagX);
					comma.GetString(ref member.tagY);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					member.color = Color.FromArgb(r, g, b);
					comma.Skip();//comma.GetChar(ref member.n.cValueType);
					comma.GetChar(ref member.cPointType);
					comma.GetChar(ref member.cLineThick);
					if(member.cLineThick < 1)	member.cLineThick = 1;

					comma.GetChar(ref member.cAxisPositionX);
					comma.GetInt(ref member.nLevelFromX);
					comma.GetInt(ref member.nLevelToX);
					comma.GetChar(ref member.cAxisPositionY);
					comma.GetInt(ref member.nLevelFromY);
					comma.GetInt(ref member.nLevelToY);
					comma.GetInt(ref member.nTagDisplaySize);
					comma.GetChar(ref member.bReverseX);
					comma.GetChar(ref member.bReverseY);

					graphMemberXY.Add(member);
				}
				else if(String.Compare(buf, "DemandMember") == 0) 
				{	
					/*
					ANALOG_GRAPH_MEMBER member;
					memset(&member, 0, sizeof(ANALOG_GRAPH_MEMBER));
					comma.GetString(member.tag, sizeof(member.tag));
					comma.GetInt(r);
					comma.GetInt(g);
					comma.GetInt(b);
					member.color = RGB(r, g, b);
					comma.GetChar(member.cValueType);
					comma.GetChar(member.cPointType);
					comma.GetChar(member.cLineThick);

					if(member.cLineThick < 1)	member.cLineThick = 1;

					demandMember->AddBlock((BYTE*)&member);
					*/
				}
				else if(String.Compare(buf, "MemberAnalogStatus") == 0) 
				{
					ANALOG_STATUS_STRUCT status = new ANALOG_STATUS_STRUCT();
					comma.GetInt(ref status.type);
					comma.GetString(ref status.filename);
					blockAnalogStatus.Add(status);
				}
				else if(String.Compare(buf, "RealUnit") == 0) 
				{
					comma.GetInt(ref nRealUnit);
				}
				else if(String.Compare(buf, "ShowUnit") == 0) 
				{
					comma.GetInt(ref nShowUnit);
				}
				else if(String.Compare(buf, "wFlags") == 0) 
				{
					comma.GetWORD(ref wFlags);
				}
				else if(String.Compare(buf, "wTimeDevide") == 0) 
				{
					comma.GetWORD(ref wTimeDevide);
				}
				else if(String.Compare(buf, "wLevelDevide") == 0) 
				{
					comma.GetWORD(ref wLevelDevide);
				}
				else if(String.Compare(buf, "wTimeSelectOption") == 0) 
				{
					comma.GetWORD(ref wTimeSelectOption);
				}
				else if(String.Compare(buf, "FileNameOn") == 0) 
				{
					comma.GetString(ref sFileNameOn);
				}
				else if(String.Compare(buf, "FileNameOff") == 0) 
				{
					comma.GetString(ref sFileNameOff);
				}
				else if(String.Compare(buf, "OverlayMethod") == 0) 
				{
					comma.GetInt(ref nOverlayMethod);
				}
				else if(String.Compare(buf, "Font") == 0) 
				{	
					// 3.07 이전의 방식

					long imsi=0;

					comma.GetString(ref fontStruct.lfFaceName);
					comma.GetLong(ref imsi);

					// 다시 복원
					//size = ::MulDiv(-lf->lfHeight, 72, ::GetDeviceCaps(hdc, LOGPIXELSY));
                    //fontStruct.lfHeight = (int)(-imsi*72/96);
					fontStruct.lfHeight = (int)((-imsi*(Double)72.0/96.0)+0.5);

					fontStruct.style = 0;

					comma.GetLong(ref imsi);	// comma.GetLong(fontStruct->lfWeight);
					if(imsi == 700)
						fontStruct.style |= FontStyle.Bold;
					comma.GetLong(ref imsi);	// comma.GetBYTE(fontStruct->lfItalic);
					if(imsi == 1)
						fontStruct.style |= FontStyle.Italic;
					comma.GetLong(ref imsi);	// comma.GetBYTE(fontStruct->lfUnderline);
					if(imsi == 1)
						fontStruct.style |= FontStyle.Underline;

					//comma.GetString(fontStruct->lfFaceName, sizeof(fontStruct->lfFaceName));
					//comma.GetLong(fontStruct->lfHeight);
					//comma.GetLong(fontStruct->lfWeight);
					//comma.GetBYTE(fontStruct->lfItalic);
					//comma.GetBYTE(fontStruct->lfUnderline);
				}
				else if(String.Compare(buf, "LogFont") == 0) 
				{	// LOGFONT를 전부 포함하는 방식
					long imsi=0;

					comma.GetString(ref fontStruct.lfFaceName);
					comma.GetLong(ref imsi);

					// 다시 복원
					//size = ::MulDiv(-lf->lfHeight, 72, ::GetDeviceCaps(hdc, LOGPIXELSY));
					//fontStruct.lfHeight = (int)(-imsi*72/96);
					fontStruct.lfHeight = (int)((-imsi*(Double)72.0/96.0)+0.5);

					fontStruct.style = 0;

					comma.GetLong(ref imsi);	// comma.GetLong(fontStruct->lfWidth);
					comma.GetLong(ref imsi);	// comma.GetLong(fontStruct->lfEscapement);
					comma.GetLong(ref imsi);	// comma.GetLong(fontStruct->lfOrientation);
					comma.GetLong(ref imsi);	// comma.GetLong(fontStruct->lfWeight);
					if(imsi == 700)
						fontStruct.style |= FontStyle.Bold;
					comma.GetLong(ref imsi);	// comma.GetBYTE(fontStruct->lfItalic);
					if(imsi == 1)
						fontStruct.style |= FontStyle.Italic;
					comma.GetLong(ref imsi);	// comma.GetBYTE(fontStruct->lfUnderline);
					if(imsi == 1)
						fontStruct.style |= FontStyle.Underline;
					comma.GetLong(ref imsi);	// comma.GetBYTE(fontStruct->lfStrikeOut);
					if(imsi == 1)
						fontStruct.style |= FontStyle.Strikeout;
					
					/*
					comma.GetString(fontStruct->lfFaceName, sizeof(fontStruct->lfFaceName));
					comma.GetLong(fontStruct->lfHeight);
					comma.GetLong(fontStruct->lfWidth);
					comma.GetLong(fontStruct->lfEscapement);
					comma.GetLong(fontStruct->lfOrientation);
					comma.GetLong(fontStruct->lfWeight);
					comma.GetBYTE(fontStruct->lfItalic);
					comma.GetBYTE(fontStruct->lfUnderline);
					comma.GetBYTE(fontStruct->lfStrikeOut);
					comma.GetBYTE(fontStruct->lfCharSet);
					comma.GetBYTE(fontStruct->lfOutPrecision);
					comma.GetBYTE(fontStruct->lfClipPrecision);
					comma.GetBYTE(fontStruct->lfQuality);
					comma.GetBYTE(fontStruct->lfPitchAndFamily);
					*/
				}
				else if(String.Compare(buf, "BackBox") == 0) 
				{
					comma.GetInt(ref nBackBox);
				}
				else if(String.Compare(buf, "Script") == 0) 
				{
					comma.GetChar(ref eID.active);
					comma.GetDWORD(ref eID.id);
				}
				else if(String.Compare(buf, "LineOption") == 0) 
				{
					comma.GetWORD(ref wLineOption);
				}
				else if(String.Compare(buf, "FillOption") == 0) 
				{
					comma.GetInt(ref lFillColor.brush_type);
				}
				else if(String.Compare(buf, "LineThick") == 0) 
				{
					comma.GetWORD(ref wLineThick);
				}
				else if(String.Compare(buf, "String") == 0) 
				{
					comma.GetString(ref sString);
				}
				else if(String.Compare(buf, "Point") == 0) 
				{
					int x = 0;
					int y = 0;

					comma.GetInt(ref x);
					comma.GetInt(ref y);

					Point point = new Point(x, y);

					blockPoint.Add(point);
				}
				else if(String.Compare(buf, "Curve") == 0) 
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
				else if(String.Compare(buf, "wGraphPointSize") == 0) 
				{
					comma.GetWORD(ref wGraphPointSize);
				}
				else if(String.Compare(buf, "TextAlign") == 0) 
				{
					comma.GetBYTE(ref align.x);
					comma.GetBYTE(ref align.y);
				}
				else if(String.Compare(buf, "StringOption") == 0) 
				{
					comma.GetStringTotalRemain(ref sStringOption);
				}
				else if(String.Compare(buf, "ClassName") == 0) 
				{
					comma.GetString(ref objGeneral.sClassName);
				}
				else if(String.Compare(buf, "ButtonDoutMember") == 0) 
				{
					BUTTON_DOUT_STRUCT member = new BUTTON_DOUT_STRUCT();

					comma.GetString(ref member.tag);
					member.tag = member.tag.Trim();
					blockButtonDoutMember.Add(member);
				}
				else if(String.Compare(buf, "TwoTagMember") == 0) 
				{
					TWO_TAG_MEMBER member = new TWO_TAG_MEMBER();

					comma.GetString(ref member.sTag1);
					comma.GetString(ref member.sTag2);
					blockTwoTagMember.Add(member);
				}
				else if(String.Compare(buf, "WindowStyle") == 0) 
				{
					uint flags=0;
					comma.GetHexDWORD(ref flags);
					dwWindowStyle = (EnumWindowStyleFlags)flags;
				}
				else if(String.Compare(buf, "ListData") == 0) 
				{
					string data="";
					comma.GetString(ref data);
					blockListData.Add(data);
				}
				else if(String.Compare(buf, "LevelDisplaySize") == 0) 
				{
					comma.GetInt(ref nLevelDisplaySize);
				}
				else 
				{

				}
			}
		}
	}
}


