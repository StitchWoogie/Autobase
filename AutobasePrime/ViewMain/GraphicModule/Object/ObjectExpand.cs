using System;
using System.Drawing;
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using AutoLibLocal;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectPublic.
	/// </summary>
	///

	[Serializable]
	public class  EXPAND_ID_STRUCT 
	{
		public sbyte	active; 
		public uint		id;
		public EnumModType mod_type;
		public ExpandScript expand = new ExpandScript();
		public bool		bLineColorToTextColor = false;	// MOD 파일에서는 TextColor 스크립트가 없고 LineColor 스크립트를 대신 사용했으므로 SingleText Object는 변환이 필요하다.
	}

	[Serializable]
	public class EXPAND_SLIDER_STRUCT {
		public string sTag;
		public int nZoneMin;
        public int nZoneMax;
        public float fValueMin;
        public float fValueMax;
		public sbyte bUseFullBase;

		public int  nWriteTimer;				// Write시 값을 변경시킨 후 
		public int  nWriteTimerOldSec;
		public double fWriteValue;				// 변경한 값이 timer가 0이 될때까지 화면상에는 이값을 보여준다.

		[NonSerialized] public int[]  nTagPos = new int[1];	

		public void LoadFromMODX(TextReader reader, string command)
		{
            CommaTextReader comma = new CommaTextReader();
			string buf="";

			nZoneMin = 0;
			nZoneMax = 100;
			fValueMin = 0;
			fValueMax = 100;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				comma.Set(buf);
				comma.GetString(ref buf);
				if(buf == "TagName") 
				{
					comma.GetString(ref sTag);
					sTag = sTag.Trim();
				}
				else if(buf == "ZoneMin") 
				{
					comma.GetInt(ref nZoneMin);
				}
				else if(buf == "ZoneMax") 
				{
                    comma.GetInt(ref nZoneMax);
				}
				else if(buf == "ValueMin") 
				{
                    comma.GetFloat(ref fValueMin);
				}
				else if(buf == "ValueMax") 
				{
                    comma.GetFloat(ref fValueMax);
				}
				else if(buf == "bUseFullBase") 
				{
					comma.GetChar(ref bUseFullBase);
				}
				else if(buf == command) 
				{
					break;
				}
			}
		}

		public void SaveFile(CommaTextWriter writer)
		{
			writer.WriteLine("TagName,{0}",  sTag);
			writer.WriteLine("ZoneMin,{0}",  nZoneMin);
			writer.WriteLine("ZoneMax,{0}",  nZoneMax);
			writer.WriteLine("ValueMin,{0}", fValueMin);
			writer.WriteLine("ValueMax,{0}", fValueMax);
			writer.WriteLine("bUseFullBase,{0}", bUseFullBase);
		}
	}

	[Serializable]
	public class EXPAND_MOUSE_ZONE
	{
		public int	x1 = 5;
		public int  y1 = 5;
		public int  x2 = 5;
		public int  y2 = 5;
		public sbyte bLock = 1;	// 이것이 체크되어 있으면 모두가 같게 반응한다.

		public void LoadFromMODX(TextReader reader, string command)
		{
            CommaTextReader comma = new CommaTextReader();
			string buf="";

			bLock = 1;
			x1 = 5;
			y1 = 5;
			x2 = 5;
			y2 = 5;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				comma.Set(buf);
				comma.GetString(ref buf);
				if(buf == "Zone") 
				{
					comma.GetInt(ref x1);
					comma.GetInt(ref y1);
					comma.GetInt(ref x2);
					comma.GetInt(ref y2);
				}
				else if(buf == "Lock") 
				{
					comma.GetChar(ref bLock);
				}
				else if(buf == command)	break;
				else {}
			}
		}

		public void SaveFile(CommaTextWriter writer)
		{
			writer.WriteLine("Zone,{0},{1},{2},{3},", x1, y1, x2, y2);
			writer.WriteLine("Lock,{0},", bLock);
		}
	}

    public enum EnumExpandType
    {
        Basic = 0,
        Script = 1,
    }

    [Serializable]
    public class ExpandBasicConversion
    {
        public string sTag;
        public int nBasicConversionType;
        public float fBasicSourceMin;
        public float fBasicSourceMax;
        public float fBasicTargetMin;
        public float fBasicTargetMax;

        public int[] tag_pos;
    }

    [Serializable]
    public class ExpandBasicVisible
    {
        public string sTag;
        public int condition;
        public string value;

        public int[] tag_pos;
        public EnumTagType tag_type;
    }

    [Serializable]
    public class ExpandBasicBlinking
    {
        public string sTag;
        public int condition;
        public string value;
        public int cycle;

        public int[] tag_pos;
        public EnumTagType tag_type;
    }

    [Serializable]
    public class ExpandBasicOnlyTag
    {
        public string sTag;

        public int[] tag_pos;
        public EnumTagType tag_type;
    }

    [Serializable]
    public class ExpandBasicColorMember
    {
        public bool active;
        public string sTag;
        public int condition;
        public string value="0";
        public Color color;

        public int[] tag_pos;
        public EnumTagType tag_type;
    }

    [Serializable]
    public class ExpandBasicColor
    {
        public ArrayList member = new ArrayList();
    }

    [Serializable]
    public class ExpandBasicRotate
    {
        public string sTag;
        public int nRotateAxis; // 회전축
        public float fBasicSourceMin;
        public float fBasicSourceMax;
        public float fBasicTargetMin;
        public float fBasicTargetMax;
        public int nDirection;  // 0 = 시계방향, 1 = 반시계방향

        public int[] tag_pos;
    }

    [Serializable]
    public class ExpandBasicAndScript
    {
        public EnumExpandType eExpandType = EnumExpandType.Script;  // 9 에서는 기본이 Script이므로 기본을 Script로 지정
        public object basic;
        public ScriptClass script;
    }

	[Serializable]
	public class ExpandScript 
	{
        public ExpandBasicAndScript scriptSizeWidth;
        public ExpandBasicAndScript scriptSizeHeight;
        public ExpandBasicAndScript scriptLocationX;
        public ExpandBasicAndScript scriptLocationY;
        public ExpandBasicAndScript scriptVisible;
		public ScriptClass scriptMouseLeftDown;
        public ScriptClass scriptMouseLeftUp;
        public ScriptClass scriptMouseRightDown;
        public ScriptClass scriptMouseRightUp;
        public ScriptClass scriptMouseEnter;
        public ScriptClass scriptMouseLeave;
        public ScriptClass scriptMouseMove;

        public ExpandBasicAndScript scriptAnimationSpeed;
		public EXPAND_SLIDER_STRUCT sliderHorz;
		public EXPAND_SLIDER_STRUCT sliderVert;
		public EXPAND_MOUSE_ZONE	structMouseZone;
        public ExpandBasicAndScript scriptColorLine;
        public ExpandBasicAndScript scriptColorFill;
        public ExpandBasicAndScript scriptColorText;
        public ExpandBasicAndScript scriptColorBack;
        public ExpandBasicAndScript scriptThickLine;
        public ExpandBasicAndScript scriptBlinking;
		public ScriptClass scriptEventKeyDown;	    // control
		public ScriptClass scriptEventSelChange;	// control	
        public ExpandBasicAndScript scriptRotate;   // 10.0 부터 추가된 기능
	}

    [Serializable]
    public class BrushPublic
    {
        public int brush_type = 1;      // 0 - null, 1 = solid, 2 = linear gradation, 3 = radial gradation
        public Color basic_color = Color.Black;   
                                        // 기존에 한가지 색상만을 사용했으므로 기존 버전과 호환성을 유지하기 위해서 기본색상을 지정해 주면 이전 버전에서도 비슷한 색상을 읽어올 수 있다.
                                        // 완전 투명한 색보다는 아예 검은 색이 좋을 듯 하다. 투명한 색은 윈도우 컨트롤의 배경으로 사용 시 다운된다.
        public Color org_basic_color = Color.Black;
                                        // Runtime시 basic색상이 바뀐다. original을 기억해서 확장기능에서 이외의 색상일 경우를 대비해서 기억해 둔다. 10.1.1 부터 추가
    }

    [Serializable]
    public class BrushSolid : BrushPublic
    {
        // basic_color 를 사용한다.
        public BrushSolid(Color color)
        {
            basic_color = color;
        }

        public BrushSolid()
        {
            
        }
    }

    [Serializable]
    public class BrushGradient : BrushPublic
    {
        public List<BrushGradientStop> stops;
        public int spread_method;
    }

    [Serializable]
    public class BrushLinearGradient : BrushGradient
    {
        public PointF pStart;
        public PointF pEnd;

        void ex()
        {
            
        }
    }

    /* 나중에 지원하기로 한다. WPF의 RadialGradientBrush에 적당한 클래스가 PathGradient가 있으나 기능이 부족함.
    [Serializable]
    public class BrushRadialGradient : BrushGradient
    {
        public PointF pOrigin;
        public PointF pCenter;
        public float radiusx;
        public float radiusy;
    }*/

    [Serializable]
    public class BrushGradientStop
    {
        public Color color;
        public float offset;
    }

	[Serializable]
	public class ObjectExpand : ObjectFont
	{
		[NonSerialized] 
		bool bMouseCapture;
		
		protected EXPAND_ID_STRUCT eID = new EXPAND_ID_STRUCT();

		public ExpandScript expandScript
        {
            set
            {
                eID.expand = value;
            }
            get
            {
                return eID.expand;
            }
        }

		[NonSerialized] int nSliderGabX;
		[NonSerialized] int nSliderGabY;

		[NonSerialized] int  nRunLocationX;			// run 중일 때 위치값.
		[NonSerialized] int  nRunLocationY;			// run 중일 때 위치값.
		[NonSerialized] float fRunPercentWidth;     //
		[NonSerialized] float fRunPercentHeight;    //
		[NonSerialized] protected bool bRunVisible; //
		[NonSerialized] int  nRunAnimationSpeed;	// animation speed
		int	 nRunThickLine;			                // 현재 화면상에 계산되어 있는 object의 borderThick
		[NonSerialized] int  nBlinkingCycle;		// 점멸주기 
		[NonSerialized] bool bVisibleByBlinking;
		
		[NonSerialized] int nViewX1;		// 현재 화면상에 계산되어 있는 object의 위치
		[NonSerialized] int nViewY1;		// 현재 화면상에 계산되어 있는 object의 위치
		[NonSerialized] int nViewX2;		// 현재 화면상에 계산되어 있는 object의 위치
		[NonSerialized] int nViewY2;		// 현재 화면상에 계산되어 있는 object의 위치
		[NonSerialized] int nViewLineThick;	// 현재 화면상에 계산되어 있는 object의 Line thick
		
		Color lLineColor;
		BrushPublic lFillColor;
		Color lTextColor;
        BrushPublic lBackColor;
		int wBorderThick;			// 테두리의 두께.

        // 2009.7.24  
        public bool IsUseBackColor = false;      // 오브젝트가 배경색을 사용하면 ture 사용하지 않으면 false  스포이드에서 Back Fill을 적당히 응용하면 적당한 색을 검출할 수 있다. 2009.7.24 SetBackColor에서 초기화 된다.
        public bool IsUseFillColor = false;      // 오브젝트가 채움색을 사용하면 ture 사용하지 않으면 false  SetFillColor에서 초기화 된다.
        public bool IsUseTextColor = false;      // 오브젝트가 배경색을 사용하면 ture 사용하지 않으면 false
        public bool IsUseLineColor = false;      // 오브젝트가 채움색을 사용하면 ture 사용하지 않으면 false

		public int	 nLineOption;
        public int nFillOption
        {
            get 
            { 
                return lFillColor.brush_type; 
            }
            set 
            { 
                lFillColor.brush_type = value; 
            }
        }

		[NonSerialized] protected bool bOnMouseZone;					// ON이면 마우스가 Object영역속에 들어왔다는 의미이다.
		protected int nLeft, nTop, nRight, nBottom;  	// object 좌표
				
		[NonSerialized] Color lRunColorLine;		    // run 중 line color
        //[NonSerialized] BrushPublic lRunColorFill;		// run 중 fill color
		[NonSerialized] Color lRunColorText;		    // run 중 fill color
        //[NonSerialized] BrushPublic lRunColorBack;		// run 중 fill color

        [NonSerialized]
        float fRunRotationAngle;		                // run 중 회전각

        public float RotationAngle
        {
            get
            {
                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                {
                    if (eID.expand.scriptRotate == null)
                    {
                        return objGeneral.fRotateAngle;
                    }
                    else
                    {
                        return fRunRotationAngle;
                    }
                }
                else
                    return objGeneral.fRotateAngle;
            }
            set
            {
                objGeneral.fRotateAngle = value;
                fRunRotationAngle = value;
            }
        }

        [NonSerialized]
        public bool bSetOriginalSizeOnStudio = false;	// 원형크기로 복구 기능을 사용한 경우 true이면 ClassName 속성 상자에서 사이즈를 변경하지 않는다.

		public virtual void Close()
		{
			
		}

		public Color RunColorLine
		{
			get 
			{
				if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
					return lRunColorLine;
				else
					return lLineColor;
			}
		}

        public BrushPublic RunColorFill
		{
			get 
			{
				//if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
					//return lRunColorFill;
				//else
					return lFillColor;
			}
		}

		public Color RunColorText
		{
			get 
			{
				if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
					return lRunColorText;
				else
					return lTextColor;
			}
		}

        public BrushPublic RunColorBack
		{
			get 
			{
				//if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
					//return lRunColorBack;
				//else
					return lBackColor;
			}
		}

		public virtual void ObjectSave(CommaTextWriter writer)
		{
			MessageBox.Show("상속클래스에서 ObjectSave를 override 해야 합니다.\n(Class="+this.ToString()+")", "프로그램 오류");
		}

		public bool ExpandActive 
		{
			get 
			{
				return (eID.active == 1);
			}
			set 
			{
				if(value)	eID.active = 1;
				else		eID.active = 0;
			}
		}

        public void SetExpandIdStruct(EXPAND_ID_STRUCT eid)
        {
            eID = eid;
        }

		public Color LineColor 
		{
			set 
			{
				SetLineColor(value);
			}
		}

		public BrushPublic FillColor
		{
			set 
			{
				SetFillColor(value);
			}
		}

		public Color TextColor
		{
			set 
			{
				SetTextColor(value);
			}
		}

        public BrushPublic BackColor
		{
			set 
			{
				SetBackColor(value);
			}
		}

		public int LineThick
		{
			set 
			{
				SetBorderThick(value);
			}
		}

		public ObjectGeneral objGeneral = new ObjectGeneral();

		public ObjectExpand(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
			: base(ocp, lf)
		{
			if(general == null)
				objGeneral = new ObjectGeneral();
			else
				objGeneral = general;
	
			if(rect == null) 
			{
				nLeft   = 0;
				nTop    = 0;
				nRight  = 100;
				nBottom = 100;
			}
			else 
			{
				nLeft   = rect.left;
				nTop    = rect.top;
				nRight  = rect.right;
				nBottom = rect.bottom;
			}

			if(eid != null) 
			{
				eID = eid;
			}
			else 
			{
				eID.active = 0;
				eID.id = 0;
				eID.mod_type = EnumModType.mod;
			}

			bMouseCapture = false;

			nRunLocationX = 0;			    // run 중일 때 위치 값.
			nRunLocationY = 0;
			fRunPercentWidth = 0;
			fRunPercentHeight = 0;
			bRunVisible = true;
			lRunColorLine = Color.Black;
			//lRunColorFill = new BrushSolid(Color.White);
            //lRunColorBack = new BrushSolid(Color.White);
			lRunColorText = Color.Black;
			nRunThickLine = 1;
			nBlinkingCycle = 0;	            // 점멸주기 
			bVisibleByBlinking = true;	    // 점멸옵션을 사용할 때 상태.
			bOnMouseZone = false;			// ON이면 마우스가 Object영역속에 들어왔다는 의미이다.

			wBorderThick = 1;
	
			lLineColor = Color.Black;
			lFillColor = new BrushSolid(Color.Red);
            lBackColor = new BrushSolid(Color.White);

			if(eID.mod_type == EnumModType.mod) 
			{
				eID.expand.scriptSizeWidth  = null;
                eID.expand.scriptSizeHeight = null;
                eID.expand.scriptLocationX = null;
                eID.expand.scriptLocationY = null;
                eID.expand.scriptVisible = null;
                eID.expand.scriptMouseLeftDown = null;
                eID.expand.scriptMouseLeftUp = null;
                eID.expand.scriptMouseRightDown = null;
                eID.expand.scriptMouseRightUp = null;
                eID.expand.scriptMouseEnter = null;
                eID.expand.scriptMouseLeave = null;
                eID.expand.scriptMouseMove = null;

                eID.expand.structMouseZone = null;
                eID.expand.scriptAnimationSpeed = null;
                eID.expand.sliderHorz = null;
                eID.expand.sliderVert = null;
                eID.expand.scriptColorLine = null;
                eID.expand.scriptColorFill = null;
                eID.expand.scriptColorText = null;
                eID.expand.scriptColorBack = null;
                eID.expand.scriptThickLine = null;
                eID.expand.scriptBlinking = null;
                eID.expand.scriptEventKeyDown = null;
                eID.expand.scriptEventSelChange = null;
                eID.expand.scriptRotate = null;

				if(eID.active == 1) 
				{
					if(eID.id > 0) 
					{
						Profile profile = new Profile();
						string inifile = "";
						MakeScriptPath(ref inifile, "INI", eID.id);
						profile.LoadByEncodingDefault(inifile);

                        eID.expand.scriptSizeWidth = LoadExpandBasicAndScriptFromMod(profile, "SIW", eID.id);
                        eID.expand.scriptSizeHeight = LoadExpandBasicAndScriptFromMod(profile, "SIH", eID.id);
                        eID.expand.scriptLocationX = LoadExpandBasicAndScriptFromMod(profile, "LOX", eID.id);
                        eID.expand.scriptLocationY = LoadExpandBasicAndScriptFromMod(profile, "LOY", eID.id);
                        eID.expand.scriptVisible = LoadExpandBasicAndScriptFromMod(profile, "VIS", eID.id);
                        eID.expand.scriptMouseLeftDown = LoadExpandScriptFromMod(profile, "MDN", eID.id);
                        eID.expand.scriptMouseLeftUp = LoadExpandScriptFromMod(profile, "MUP", eID.id);
                        eID.expand.scriptAnimationSpeed = LoadExpandBasicAndScriptFromMod(profile, "ANS", eID.id);
                        eID.expand.sliderHorz = LoadExpandSlider(profile, "SLH", eID.id);
                        if (eID.expand.sliderHorz != null && eID.expand.sliderHorz.nTagPos[0] == TagLib.TAG_NOT_FOUND) 
						{
                            eID.expand.sliderHorz = null;
						}
                        eID.expand.sliderVert = LoadExpandSlider(profile, "SLV", eID.id);
                        if (eID.expand.sliderVert != null && eID.expand.sliderVert.nTagPos[0] == TagLib.TAG_NOT_FOUND) 
						{
                            eID.expand.sliderVert = null;
						}
                        eID.expand.structMouseZone = LoadExpandMouseZone(profile, "MZN", eID.id);

						if(eid.bLineColorToTextColor)
                            eID.expand.scriptColorText = LoadExpandBasicAndScriptFromMod(profile, "COL", eID.id);
						else
                            eID.expand.scriptColorLine = LoadExpandBasicAndScriptFromMod(profile, "COL", eID.id);

                        eID.expand.scriptColorFill = LoadExpandBasicAndScriptFromMod(profile, "COF", eID.id);

                        eID.expand.scriptThickLine = LoadExpandBasicAndScriptFromMod(profile, "THL", eID.id);
                        eID.expand.scriptBlinking = LoadExpandBasicAndScriptFromMod(profile, "BLK", eID.id);
                        eID.expand.scriptEventKeyDown = LoadExpandScriptFromMod(profile, "KDN", eID.id);
                        eID.expand.scriptEventSelChange = LoadExpandScriptFromMod(profile, "SLC", eID.id);
					}
				}
			}


			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
                SetUseScriptLocationX((eID.active == 1 && eID.expand.scriptLocationX != null) ? true : false);
                SetUseScriptLocationY((eID.active == 1 && eID.expand.scriptLocationY != null) ? true : false);
			}

			nViewX1 = 0;
			nViewY1 = 0;
			nViewX2 = 0;
			nViewY2 = 0;
			nViewLineThick = 1;

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
                //EnumRunChangedType changed_type;
                //ExpandRun(null, out changed_type);
                //EnumRunChangedType changed_type;
                _ = ExpandRunAsync(null);
            }

			ExpandCalcObjectRect();
		}

		public Color GetLineColor()  { return lLineColor; }
		public BrushPublic GetFillColor()  { return lFillColor; }
		public Color GetTextColor()  { return lTextColor; }
        public BrushPublic GetBackColor() { return lBackColor; }
		public int GetBorderThick()  { return wBorderThick; }

		int ExpandCalcSizeWidth(int width)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(eID.active == 0)		return width;
                if(eID.expand.scriptSizeWidth == null) return width;

				return (int)((long)width*fRunPercentWidth/100);
			}
			else 
			{
				return width;
			}
		}

		int ExpandCalcSizeHeight(int height)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(eID.active == 0)		return height;
                if (eID.expand.scriptSizeHeight == null) return height;

				return (int)((long)height*fRunPercentHeight/100);
			}
			else 
			{
				return height;
			}
		}

		int ExpandCalcLocationX(int x)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(eID.active == 0)		return x;
                if (eID.expand.scriptLocationX == null && eID.expand.sliderHorz == null) return x;
				return nRunLocationX;
			}
			else 
			{
				return x;
			}
		}

		int ExpandCalcLocationY(int y)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(eID.active == 0)		return y;
                if (eID.expand.scriptLocationY == null && eID.expand.sliderVert == null) return y;

				return nRunLocationY;
			}
			else 
			{
				return y;
			}
		}

		protected bool ExpandCalcVisible()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(bRunVisible && bVisibleByBlinking)	return true;

				return false;
			}
			else 
			{
				return true;
			}
		}

        /*
		int ExpandCalcThickLine()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(eID.active == 0)		return wBorderThick;
				if(eID.expand.scriptThickLine == null)	return wBorderThick;

				return nRunThickLine;
			}
			else 
			{
				return wBorderThick;
			}
		}*/

		//------------------------------------------------------------------------------
		//	각 object에서 직접 이 함수를 부른다.
		//	내부에서는 사용하지 않는다.
		//  사용자 script에서는 rpm 값으로 온다.
		//------------------------------------------------------------------------------

		public int ExpandGetAnimationSpeed()
		{
			if(eID.active == 0)		return 60;
            if (eID.expand.scriptAnimationSpeed == null) return 60;	// 1초에 1회전 한다.

			return nRunAnimationSpeed; 
		}

		//------------------------------------------------------------------------------
		//	각 object에서 직접 이 함수를 부른다.
		//	내부에서는 사용하지 않는다.
		// 애니매이션 속도를 사용하는가 ?.
		//------------------------------------------------------------------------------

		public bool ExpandIsUseAnimationSpeed()
		{
			if(eID.active == 0)		return false;

            if (eID.expand.scriptAnimationSpeed != null) return true;
			else								return false;
		}

        double GetPercent(double curr, double v_base, double v_full, double fBase, double fFull)
		{
            double val;

			if((v_full-v_base) == 0) 
			{
				if(fBase < fFull) 	val = fBase;
				else				val = fFull;
			}
			else 
			{
				if(fBase < fFull) 
				{
					val = (((double)(curr-v_base)*(fFull-fBase))/(v_full-v_base));
					val += fBase;
				}
				else 
				{	// 결과값이 반대로 꺼꾸로 되어 있을 때
					val = (((double)(curr-v_base)*(fBase-fFull))/(v_full-v_base));
					val = (fBase-fFull)-val;
					val += fFull;
				}
			}

			// 범위를 초과하는 값을 잘라낸다.
			if(fBase < fFull) 
			{
				if(val < fBase)	val = fBase;
				if(val > fFull)	val = fFull;
			}
			else 
			{
				if(val > fBase)	val = fBase;
				if(val < fFull)	val = fFull;
			}

			return val;
		}

        protected enum EnumRunChangedType
        {
            None        = 0x0000,
            Size        = 0x0001,
            Location    = 0x0002,
            Thick       = 0x0004,
            Rotation    = 0x0008,   // 10.0 부터 추가
        }

        protected virtual void TextColorChanged() { }
        protected virtual void BackColorChanged() { }

        double GetTagValue(string tag, ref int[] tag_pos)
        {
            double val = 0;
            TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

            if (tp.enumTagType == EnumTagType.AI)
            {
                TagAiClass ai = (TagAiClass)tp;
                ai.NeedDataCurr = true; //20241111 PSU ViewMain 갱신
                val = ai.curr;
            }
            else if (tp.enumTagType == EnumTagType.DI)
            {
                TagDiClass di = (TagDiClass)tp;   //20241111 PSU ViewMain 갱신
                di.NeedDataCurr = true;
                val = di.curr;
            }
            else { }

            return val;
        }

        async Task<(bool success, double retn, int conversion_type)> RunExpandBasicAndScriptAsync(System.Windows.Forms.Form form, ExpandBasicAndScript expand, int conversion_type = 0)
        {
            double retn = 0;

            if (expand == null) return (false, retn, conversion_type);

            if (expand.eExpandType == EnumExpandType.Basic)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion conv = (ExpandBasicConversion)expand.basic;

                    double val = GetTagValue(conv.sTag, ref conv.tag_pos);

                    retn = (val - conv.fBasicSourceMin) * (conv.fBasicTargetMax - conv.fBasicTargetMin) / (conv.fBasicSourceMax - conv.fBasicSourceMin) + conv.fBasicTargetMin;

                    conversion_type = conv.nBasicConversionType;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible conv = (ExpandBasicVisible)expand.basic;

                    if(ObjectTagAnimation.IsCondition(conv.sTag, ref conv.tag_pos, conv.condition, conv.value))
                        retn = 1;
                    else
                        retn = 0;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking conv = (ExpandBasicBlinking)expand.basic;

                    if (ObjectTagAnimation.IsCondition(conv.sTag, ref conv.tag_pos, conv.condition, conv.value))
                        retn = conv.cycle;
                    else
                        retn = 0;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag conv = (ExpandBasicOnlyTag)expand.basic;

                    retn = GetTagValue(conv.sTag, ref conv.tag_pos);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate conv = (ExpandBasicRotate)expand.basic;

                    double val = GetTagValue(conv.sTag, ref conv.tag_pos);

                    retn = (val - conv.fBasicSourceMin) * (conv.fBasicTargetMax - conv.fBasicTargetMin) / (conv.fBasicSourceMax - conv.fBasicSourceMin) + conv.fBasicTargetMin;

                    conversion_type = conv.nRotateAxis;

                    if (conv.nDirection == 1) retn *= -1;   // 반시계방향
                }
                else
                {
                    //MessageBox.Show("ObjectExpand.RunExpandBasicAndScript 에서 알수 없는 Type", expand.basic.ToString());    //250828 PSU 
                    if (form.InvokeRequired)
                        form.Invoke(new Action(() => MessageBox.Show("ObjectExpand.RunExpandBasicAndScript 에서 알수 없는 Type", expand.basic.ToString())));
                    else
                        MessageBox.Show("ObjectExpand.RunExpandBasicAndScript 에서 알수 없는 Type", expand.basic.ToString());
                }
            }
            else
            {
                if (expand.script == null) return (false, retn, conversion_type);
                await expand.script.RunAsync(form, this).ConfigureAwait(false);
                retn = (double)expand.script.GetReturnValue();
            }

            return (true, retn, conversion_type);
        }

        async Task<(bool success, Color color)> RunExpandBasicAndScriptAsync(System.Windows.Forms.Form form, ExpandBasicAndScript expand,  Color retn)
        {
            if (expand == null) return (false, retn);

            if (expand.eExpandType == EnumExpandType.Basic)
            {
                if (expand.basic == null) return (false, retn);

                if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor conv = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;

                    for (int i = 0; i < conv.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)conv.member[i];

                        if (member.active == false) continue;

                        if (ObjectTagAnimation.IsCondition(member.sTag, ref member.tag_pos, member.condition, member.value))
                        {
                            retn = member.color;
                            return (true, retn);
                        }
                    }

                    return (true, retn);    // false가 아니다 넘어온 기본 색상값을 변경하지 않고 true를 반환한다.
                }
                else
                {
                    MessageBox.Show("ObjectExpand.RunExpandBasicAndScript에서 알수 없는 Type", expand.basic.ToString());
                }
            }
            else
            {
                if (expand.script == null) return (false, retn);
                await expand.script.RunAsync(form, this).ConfigureAwait(false);
                retn = expand.script.GetReturnValueColor();
            }

            return (true, retn);
        }

        [NonSerialized] int nBasicExpandOptionSizeWidth = 0;
        [NonSerialized] int nBasicExpandOptionSizeHeight = 0;
        [NonSerialized] int nBasicExpandOptionLocationX = 0;
        [NonSerialized] int nBasicExpandOptionLocationY = 0;
        [NonSerialized] int nBasicExpandOptionRotate = 0;   // 0 = 중심이 회전축, 1=LT, 2=RT, 3=LB, 4=RB

        [NonSerialized] private DateTime _lastExpandRun = DateTime.MinValue;
        [NonSerialized] private readonly TimeSpan _expandRunThrottle = TimeSpan.FromMilliseconds(16); // ~60fps

        private bool IsHighPriorityUpdate()
        {
            // 사용자 상호작용이나 중요한 상태 변경이 있는 경우
            return bMouseCapture || bRightMouseCapture ||
                   (eID.expand.sliderHorz?.nWriteTimer > 0) ||
                   (eID.expand.sliderVert?.nWriteTimer > 0);
        }

        async Task<(bool changeFlag, EnumRunChangedType changedType)> ExpandRunAsync(System.Windows.Forms.Form form)
        {
            var now = DateTime.Now;

            // 고주파 업데이트 방지 (선택적 스로틀링)
            if (now - _lastExpandRun < _expandRunThrottle && !IsHighPriorityUpdate())
            {
                return (false, EnumRunChangedType.None);
            }
            _lastExpandRun = now;

            bool change_flag = false;
            EnumRunChangedType changed_type = EnumRunChangedType.None;

            if (eID.active == 1)
            {	// 확장 옵션을 사용한다.
                //double retn;
                int option = 0; // 옵션이 없는 경우

                var resultVisible = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptVisible, option).ConfigureAwait(false);
                if (resultVisible.success)
                {
                    bool visible = resultVisible.retn == 0 ? false : true;

                    if (bRunVisible != visible)
                    {
                        bRunVisible = visible;
                        change_flag = true;
                    }

                    bool bSkipWithVisible = true; // visible이 false인 경우는 점멸만 계산한다. 

                    // 보이지 않을 때는 필수적이지 않은 계산들을 스킵
                    if (!bRunVisible && bSkipWithVisible)
                    {
                        var resultBlinking2 = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptBlinking, option).ConfigureAwait(false);
                        if (resultBlinking2.success)
                        {
                            if (nBlinkingCycle != (int)resultBlinking2.retn)
                            {
                                nBlinkingCycle = (int)resultBlinking2.retn;
                                change_flag = true;

                                if (nBlinkingCycle == 0)
                                {   // 점멸을 사용하지 않는다.
                                    bVisibleByBlinking = true;
                                }
                            }
                        }
                        return (change_flag, changed_type);
                    }
                }


                var resultSizeWidth = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptSizeWidth, nBasicExpandOptionSizeWidth).ConfigureAwait(false);
                if (resultSizeWidth.success)
                {
                    if (fRunPercentWidth != (float)resultSizeWidth.retn)
                    {
                        fRunPercentWidth = (float)resultSizeWidth.retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Size;
                    }
                    nBasicExpandOptionSizeWidth = resultSizeWidth.conversion_type;
                }

                var resultSizeHeight = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptSizeHeight, nBasicExpandOptionSizeHeight).ConfigureAwait(false);
                if (resultSizeHeight.success)
                {
                    if (fRunPercentHeight != (float)resultSizeHeight.retn)
                    {
                        fRunPercentHeight = (float)resultSizeHeight.retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Size;
                    }
                    nBasicExpandOptionSizeHeight = resultSizeHeight.conversion_type;
                }

                var resultLocationX = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptLocationX, nBasicExpandOptionLocationX).ConfigureAwait(false);
                if (resultLocationX.success)
                {
                    if (nRunLocationX != (int)resultLocationX.retn)
                    {
                        nRunLocationX = (int)resultLocationX.retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Location;
                    }
                    nBasicExpandOptionLocationX = resultLocationX.conversion_type;
                }

                var resultLocationY = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptLocationY, nBasicExpandOptionLocationY).ConfigureAwait(false);
                if (resultLocationY.success)
                {
                    if (nRunLocationY != (int)resultLocationY.retn)
                    {
                        nRunLocationY = (int)resultLocationY.retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Location;
                    }
                    nBasicExpandOptionLocationY = resultLocationY.conversion_type;
                }

                //var resultVisible = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptVisible, option).ConfigureAwait(false);
                //if (resultVisible.success)
                //{
                //    bool visible = resultVisible.retn == 0 ? false : true;

                //    if (bRunVisible != visible)
                //    {
                //        bRunVisible = visible;
                //        change_flag = true;
                //    }
                //}

                var resultBlinking = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptBlinking, option).ConfigureAwait(false);
                if (resultBlinking.success)
                {
                    if (nBlinkingCycle != (int)resultBlinking.retn)
                    {
                        nBlinkingCycle = (int)resultBlinking.retn;
                        change_flag = true;

                        if (nBlinkingCycle == 0)
                        {   // 점멸을 사용하지 않는다.
                            bVisibleByBlinking = true;
                        }
                    }
                }

                var resultAnimationSpeed = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptAnimationSpeed, option).ConfigureAwait(false);
                if (resultAnimationSpeed.success)
                {
                    nRunAnimationSpeed = (int)resultAnimationSpeed.retn;
                }

                if (eID.expand.sliderHorz != null)
                {
                    if (eID.expand.sliderHorz.nTagPos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderHorz.sTag, ref eID.expand.sliderHorz.nTagPos);

                        double val = ai.curr;

                        if (eID.expand.sliderHorz.nWriteTimer > 0)
                        {
                            if (val == eID.expand.sliderHorz.fWriteValue)
                            {
                                eID.expand.sliderHorz.nWriteTimer = 0;  // 값이 확인되었다.
                            }
                            else
                            {
                                DateTime t = DateTime.Now;
                                if (t.Second != eID.expand.sliderHorz.nWriteTimerOldSec)
                                {
                                    eID.expand.sliderHorz.nWriteTimerOldSec = t.Second;
                                    eID.expand.sliderHorz.nWriteTimer--;
                                }
                                if (eID.expand.sliderHorz.nWriteTimer > 0)
                                    val = eID.expand.sliderHorz.fWriteValue;
                            }
                        }

                        if (eID.expand.sliderHorz.bUseFullBase == 1)
                        {
                            val = GetPercent((float)val, ai.fBase, ai.fFull,
                                (float)(-eID.expand.sliderHorz.nZoneMin), (float)eID.expand.sliderHorz.nZoneMax);
                        }
                        else
                        {
                            val = GetPercent((float)val, (float)eID.expand.sliderHorz.fValueMin, (float)eID.expand.sliderHorz.fValueMax,
                                (float)(-eID.expand.sliderHorz.nZoneMin), (float)eID.expand.sliderHorz.nZoneMax);
                        }

                        val += (nLeft < nRight ? nLeft : nRight);

                        if (nRunLocationX != (int)val) //20250724 int 캐스팅 추가
                        {
                            nRunLocationX = (int)val;
                            change_flag = true;
                            changed_type |= EnumRunChangedType.Location;
                        }
                    }
                }
                if (eID.expand.sliderVert != null)
                {
                    if (eID.expand.sliderVert.nTagPos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderVert.sTag, ref eID.expand.sliderVert.nTagPos);

                        double val = ai.curr;

                        if (eID.expand.sliderVert.nWriteTimer > 0)
                        {
                            if (val == eID.expand.sliderVert.fWriteValue)
                            {
                                eID.expand.sliderVert.nWriteTimer = 0;  // 값이 확인되었다.
                            }
                            else
                            {
                                DateTime t = DateTime.Now;
                                if (t.Second != eID.expand.sliderVert.nWriteTimerOldSec)
                                {
                                    eID.expand.sliderVert.nWriteTimerOldSec = t.Second;
                                    eID.expand.sliderVert.nWriteTimer--;
                                }
                                if (eID.expand.sliderVert.nWriteTimer > 0)
                                    val = eID.expand.sliderVert.fWriteValue;
                            }
                        }

                        if (eID.expand.sliderVert.bUseFullBase == 1)
                        {
                            val = GetPercent((float)val, ai.fBase, ai.fFull,
                                (float)(-eID.expand.sliderVert.nZoneMin), (float)eID.expand.sliderVert.nZoneMax);
                        }
                        else
                        {
                            val = GetPercent((float)val, (float)eID.expand.sliderVert.fValueMin, (float)eID.expand.sliderVert.fValueMax,
                                (float)(-eID.expand.sliderVert.nZoneMin), (float)eID.expand.sliderVert.nZoneMax);
                        }
                        val += (nTop < nBottom ? nTop : nBottom);

                        if (nRunLocationY != (int)val)  //20250724 int 캐스팅 추가
                        {
                            nRunLocationY = (int)val;
                            change_flag = true;
                            changed_type |= EnumRunChangedType.Location;
                        }
                    }
                }

                var resultThickLine = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptThickLine, option).ConfigureAwait(false);
                if (resultThickLine.success)
                {
                    if (nRunThickLine != (int)resultThickLine.retn)
                    {
                        nRunThickLine = (int)resultThickLine.retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Thick;
                    }
                }

                Color color;

                color = lLineColor; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                var resultColorLine = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptColorLine, color).ConfigureAwait(false);
                if (resultColorLine.success)
                {
                    Color newColor = resultColorLine.color;
                    if (lRunColorLine != newColor)
                    {
                        lRunColorLine = newColor;
                        change_flag = true;
                    }
                }
                else
                {
                    lRunColorLine = color;
                }

                color = lFillColor.org_basic_color; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                var resultColorFill = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptColorFill, color).ConfigureAwait(false);
                if (resultColorFill.success)
                {
                    Color newColor = resultColorFill.color;
                    if (lFillColor.basic_color != newColor)
                    {
                        lFillColor.basic_color = newColor;
                        change_flag = true;
                    }
                }
                else
                {
                    lFillColor.basic_color = color;
                }

                color = lTextColor; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                var resultColorText = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptColorText, color).ConfigureAwait(false);
                if (resultColorText.success)
                {
                    Color newColor = resultColorText.color;
                    if (lRunColorText != newColor)
                    {
                        lRunColorText = newColor;
                        change_flag = true;

                        // TextColorChanged();   //250728 PSU

                        if (form != null && form.InvokeRequired)
                            form?.Invoke(new Action(TextColorChanged));  //form.Invoke(new Action(() => TextColorChanged())); 
                        else
                            TextColorChanged();
                    }
                }
                else
                {
                    lRunColorText = color;
                }

                color = lBackColor.org_basic_color; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                var resultColorBack = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptColorBack, color).ConfigureAwait(false);
                if (resultColorBack.success)
                {
                    Color newColor = resultColorBack.color;
                    if (lBackColor.basic_color != newColor)
                    {
                        lBackColor.basic_color = newColor;
                        change_flag = true;

                        // BackColorChanged();   250728 PSU
                        if (form != null && form.InvokeRequired)
                            form.Invoke(new Action(BackColorChanged));
                        else
                            BackColorChanged();
                    }
                }
                else
                {
                    lBackColor.basic_color = color;
                }

                var resultRotate = await RunExpandBasicAndScriptAsync(form, eID.expand.scriptRotate, nBasicExpandOptionRotate).ConfigureAwait(false);
                if (resultRotate.success)
                {
                    if (fRunRotationAngle != (float)resultRotate.retn)
                    {
                        fRunRotationAngle = (float)resultRotate.retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Rotation;
                    }
                    nBasicExpandOptionRotate = resultRotate.conversion_type;
                }
            }

            return (change_flag, changed_type);
        }

        // Edit모드에서도 Original size 로 돌아 갈때도 이 함수를 사용한다.
        // 
        protected bool ExpandCalcObjectRect()
		{
			int x1, y1, x2, y2;
			int oldx1 = nViewX1;
			int oldy1 = nViewY1;
			int oldx2 = nViewX2;
			int oldy2 = nViewY2;
            int oldthick = nViewLineThick;

            // LocationX, Width 계산
            x1 = nLeft;
            x2 = nRight;
            y1 = nTop;
            y2 = nBottom;

            // RUN시 실제 오브젝트가 위치할 위치와 크기를 결정한다.
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                int width = Math.Abs(nRight - nLeft) + 1;
                int height = Math.Abs(nBottom - nTop) + 1;

                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                // LocationX
                if (nBasicExpandOptionLocationX == 1)   // 오른쪽 방향
                {
                    x1 = x1 + ExpandCalcLocationX(x1);
                    x2 = x1 + (width - 1);
                }
                else if (nBasicExpandOptionLocationX == 2)   // 왼쪽 방향
                {
                    x1 = x1 - ExpandCalcLocationX(x1);
                    x2 = x1 + (width - 1);
                }
                else // 화면 좌표
                {
                    x1 = ExpandCalcLocationX(x1);
                    x2 = x1 + (width - 1);
                }

                // Size Width
                if (nBasicExpandOptionSizeWidth == 1)   // 중간에서
                {
                    int calc_width = ExpandCalcSizeWidth(width);
                    x1 = x1 + width / 2 - calc_width / 2;
                    x2 = x1 + (calc_width - 1);
                }
                else if (nBasicExpandOptionSizeWidth == 2)   // 오른쪽에서
                {
                    x1 = x2 - (ExpandCalcSizeWidth(width) - 1);
                }
                else // 왼쪽에서 - 기본 모드
                {
                    x2 = x1 + (ExpandCalcSizeWidth(width) - 1);
                }

                // LocationY
                if (nBasicExpandOptionLocationY == 1)   // 아래쪽 방향
                {
                    y1 = y1 + ExpandCalcLocationY(y1);
                    y2 = y1 + (height - 1);
                }
                else if (nBasicExpandOptionLocationY == 2)   // 위쪽 방향
                {
                    y1 = y1 - ExpandCalcLocationY(y1);
                    y2 = y1 + (height - 1);
                }
                else // 화면 좌표
                {
                    y1 = ExpandCalcLocationY(y1);
                    y2 = y1 + (height - 1);
                }

                // Size Height
                if (nBasicExpandOptionSizeHeight == 1)   // 중간에서
                {
                    int calc_height = ExpandCalcSizeHeight(height);
                    y1 = y1 + height / 2 - calc_height / 2;
                    y2 = y1 + (calc_height - 1);
                }
                else if (nBasicExpandOptionSizeHeight == 2)   // 밑에서
                {
                    y1 = y2 - (ExpandCalcSizeHeight(height) - 1);
                }
                else // 위에서 - 기본 모드
                {
                    y2 = y1 + (ExpandCalcSizeHeight(height) - 1);
                }

                if (nLeft > nRight) Tools.Temp(ref x1, ref x2);
                if (nTop > nBottom) Tools.Temp(ref y1, ref y2);
            }

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                {
                    if (eID.active == 1 || bItIsGroup_CallByObjectRect)
                    {
                        // 해당 스크립트가 있는 경우만
                        if (eID.expand.scriptLocationX != null || eID.expand.scriptLocationY != null ||
                            eID.expand.scriptSizeHeight != null || eID.expand.scriptSizeWidth != null ||
                            eID.expand.sliderHorz != null || eID.expand.sliderVert != null || bItIsGroup_CallByObjectRect)
                        {
                            ObjectGroup grp = (ObjectGroup)this;

                            RECT r = new RECT();

                            r.left = x1 + rGroupZone.left;
                            r.top = y1 + rGroupZone.top;
                            r.right = x2 + rGroupZone.left;
                            r.bottom = y2 + rGroupZone.top;

                            grp.SetZoneAtPercent100(grp.sizeGroup, r);
                        }
                    }
                }
            }

			x1 = GetViewPosX(x1);
			y1 = GetViewPosY(y1);
			x2 = GetViewPosX(x2);
			y2 = GetViewPosY(y2);

			nViewX1 = x1;
			nViewY1 = y1;
			nViewX2 = x2;
			nViewY2 = y2;
			nViewLineThick = GetViewSize(nRunThickLine);

			// 변한것이 있으면 OnMove메세지를 날려준다.
            if ((oldx1 != nViewX1) || (oldy1 != nViewY1) || (oldx2 != nViewX2) || (oldy2 != nViewY2) || oldthick != nViewLineThick)
			{
                nViewOldX1 = oldx1;
                nViewOldY1 = oldy1;
                nViewOldX2 = oldx2;
                nViewOldY2 = oldy2;
                nViewOldLineThick = oldthick;
				OnMove(nViewX1, nViewY1, nViewX2, nViewY2);

                return true;
			}

            return false;
		}

        [NonSerialized]
        int nViewOldX1 = 0, nViewOldY1 = 0, nViewOldX2 = 0, nViewOldY2 = 0, nViewOldLineThick = 0; // 좌표가 바뀌기 전의 값

		public virtual void OnMove(int x1, int y1, int x2, int y2) 
		{
		
		}

        /// <summary>
        /// 원래 ObjectTag에 있는 함수인데 9.3.5 부터 이쪽으로 이전
        /// </summary>
        /// <returns></returns>
        protected bool CheckResponseOnVisible()
        {
            if (objGeneral.bResponseOnVisible == false) return true;

            if (bRunVisible) return true;

            return false;
        }

        // 회전을 사용 시 마우스 위치를 회전한 값 만큼 꺼꾸로 돌리면 정상적인 모양에서 마우스 체크를 하는 것과 같다.
        protected void GetMousePositionByRotate(MouseEventArgs e, out int mx, out int my)
        {
            float angle = RotationAngle;
            mx = e.X;
            my = e.Y;

            if(angle == 0) {
                return;
            }

            int x1, x2, y1, y2;

			x1 = nViewX1;
			y1 = nViewY1;
			x2 = nViewX2;
			y2 = nViewY2;

            int cx, cy;

            CalcRotationCenter(out cx, out cy);

            float degree = (float)MathLib.MathGradientToDegree(mx - cx, my - cy);
            degree += RotationAngle;

            double r = MathLib.MathGetHypotenuse(mx - cx, my - cy);

            MathLib.MathGetEllipsePoint(cx, cy, (float)r, (float)r, degree, out mx, out my);

            return;
        }

        public virtual async Task<bool> WmLeftButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
		{
            if (!CheckResponseOnVisible()) return false;

			int sx, sy;
			int   vx1, vx2, vy1, vy2;

            GetMousePositionByRotate(e, out sx, out sy);
			
			vx1 = nViewX1;
			vy1 = nViewY1;
			vx2 = nViewX2;
			vy2 = nViewY2;

			if(vx1 > vx2)	Tools.Temp(ref vx1, ref vx2);
			if(vy1 > vy2)	Tools.Temp(ref vy1, ref vy2);

			if(sx < vx1 || sy < vy1 || sx > vx2 || sy > vy2) 	return false;

			if(eID.active == 1 && eID.expand.scriptMouseLeftDown != null) 
			{
                eID.expand.scriptMouseLeftDown.SetHandOperation();  // 수동으로 출력한다.
                await eID.expand.scriptMouseLeftDown.RunAsync(form, this);
                if (eID.expand.scriptMouseLeftDown.IsError()) 
				{
					string message;
                    message = eID.expand.scriptMouseLeftDown.GetError();
					MessageDisplay.Show(message);
				}

                if (eID.expand.scriptMouseLeftUp != null) 
				{	  // 마우스 Up 스크립트가 있을 때만.
					form.Capture = true;
					bMouseCapture = true;
				}
				return true;
			}
			else 
			{
                if (eID.active == 1 && eID.expand.scriptMouseLeftUp != null) 
				{
					form.Capture = true;
					bMouseCapture = true;
					return true;
				}
			}

			if(eID.active == 1 && (eID.expand.sliderHorz != null || eID.expand.sliderVert != null)) 
			{
				nSliderGabX = sx-vx1;
				nSliderGabY = sy-vy1;

				form.Capture = true;
				bMouseCapture = true;
				return true;
			}

			return false;
		}

		public virtual async Task<bool> WmLeftButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			if(bMouseCapture == false)	return false;

            if (eID.active == 1 && eID.expand.scriptMouseLeftUp != null) 
			{
                eID.expand.scriptMouseLeftUp.SetHandOperation();	// 수동으로 출력한다.
                await eID.expand.scriptMouseLeftUp.RunAsync(form, this);
                if (eID.expand.scriptMouseLeftUp.IsError())
				{
					string message;
                    message = eID.expand.scriptMouseLeftUp.GetError();
					MessageDisplay.Show(message);
				}
			}

			bMouseCapture = false;
			form.Capture = false;

			return true;
		}

		static bool bToolTipCheckStart = false;
		static object pToolTipOwner = null;
		static bool bToolTipViewStatus = false;
		static ToolTip oToolTip = new ToolTip();
		static Form pToolTipForm = null;

		public static void ToolTipCheckStart()
		{
			if(!ConfigViewMain.bDisplayToolTip)	return;

			bToolTipCheckStart = false;
		}

		public static void ToolTipCheckEnd()
		{
			if(!ConfigViewMain.bDisplayToolTip)	return;

			if(bToolTipCheckStart == false) // 툴팁 디스프레이가 해당되는 오브젝트가 없다.
			{
				if(pToolTipForm != null) 
				{
					oToolTip.SetToolTip(pToolTipForm, "");
				}
				bToolTipViewStatus = false;
			}
		}

        static GraphicModule.ScriptClass scriptCalc = new GraphicModule.ScriptClass();

		public virtual async Task<string> GetToolTipString()
		{
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (this.objGeneral.sObjectDescription.Length > 0 && this.objGeneral.sObjectDescription[0] == '=')
                {
                    (bool success, object val) = await scriptCalc.GetValueRecurse(this.objGeneral.sObjectDescription.Substring(1)).ConfigureAwait(false);

                    return scriptCalc.GetValueString(val);
                }
                else
                {
                    return this.objGeneral.sObjectDescription;
                }
            }
            else
            {
                return this.objGeneral.sObjectDescription;
            }
		}

		/// <summary>
		/// 자체윈도우가 있는 요소는 해당되는 차일드 폼을 연결해 준다.
		/// </summary>
		public async Task SetToolTipOnChildWindow(Control child)
		{
			oToolTip.InitialDelay = 0;
			oToolTip.ShowAlways = true;

			oToolTip.SetToolTip(child, await GetToolTipString());
		}

		protected async Task ToolTipCheck(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			if(!ConfigViewMain.bDisplayToolTip)	return;
            if (!CheckResponseOnVisible()) return;

			if(bToolTipCheckStart == false && this.objGeneral.bUseToolTip == 1)	// 툴팁 보여주기
			{
				int   vx1, vx2, vy1, vy2;

				vx1 = nViewX1;
				vy1 = nViewY1;
				vx2 = nViewX2;
				vy2 = nViewY2;

				if(vx1 > vx2)	Tools.Temp(ref vx1, ref vx2);
				if(vy1 > vy2)	Tools.Temp(ref vy1, ref vy2);

				if(e.X >= vx1 && e.Y >= vy1 && e.X <= vx2 && e.Y <= vy2) 
				{
					if(!bToolTipViewStatus) 
					{
						bToolTipViewStatus = true;
						oToolTip.SetToolTip(form, await GetToolTipString());
						oToolTip.InitialDelay = 0;
						oToolTip.ShowAlways = true;
						pToolTipOwner = this;
						pToolTipForm = form;
						//nToolTipX = e.X;
						//nToolTipY = e.Y;
					}
					else 
					{
						if(pToolTipOwner != this)	// 오브젝트가 바뀌었다.
						{
							oToolTip.SetToolTip(form, await this.GetToolTipString());
							pToolTipOwner = this;
							pToolTipForm = form;
						}
					}

					bToolTipCheckStart = true;
				}
				else 
				{
					/*
					if(bToolTipViewStatus) 
					{
						bToolTipViewStatus = false;
						//InvalidateObject(form);
					}
					*/
				}
			}
		}

        protected virtual string GetTagName()
        {
            return "";
        }

        // 같은 태그로 설정된 오브젝트의 선택영역을 함께 반전해 준다.
        // 2012.7.23 엠네스텍의 요구로 지원
        void CheckSameTagMouseZone(bool visible)
        {
            if (!ConfigViewMain.bDisplayMouseZoneWhenSameTagSelected) return;

            string tag = GetTagName();

            if (tag.Length == 0) return;    // 태그가 없을 경우 해당 사항이 없다.

            FormGraphicChild child = (FormGraphicChild)objCommonProperty.form;

            child.objectGraphic.groupRoot.SetSameTagMouseZone(tag, visible);

            objCommonProperty.form.Invalidate();
        }

        // 다른 오브젝트에서 마우스 영역 표시가 되면 연결된 태그도 영영표시를 할 경우
        protected bool bSameTagMouseZoneDisplay = false;

        // 오브젝트에서 마우스 영역 표시가 될 때 다른 오브젝트도 영역표시를 함께한다. 2012.7.23 지원
        public virtual void SetSameTagMouseZone(string tag, bool visible)
        {

        }

        async Task PlayExpandScript(System.Windows.Forms.Form form, ScriptClass script)
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

		public virtual async Task<bool> WmMouseMove(System.Windows.Forms.Form form, MouseEventArgs e)
		{
			await ToolTipCheck(form, e);	// ToolTip 체크
			
			if(eID.active == 1) 
			{
                if (eID.expand.scriptMouseMove != null)
                {
                    await PlayExpandScript(form, eID.expand.scriptMouseMove);
                }

				int   vx1, vx2, vy1, vy2;

				vx1 = nViewX1;
				vy1 = nViewY1;
				vx2 = nViewX2;
				vy2 = nViewY2;

				if(vx1 > vx2)	Tools.Temp(ref vx1, ref vx2);
				if(vy1 > vy2)	Tools.Temp(ref vy1, ref vy2);

                int sx, sy;

                GetMousePositionByRotate(e, out sx, out sy);

                if (sx >= vx1 && sy >= vy1 && sx <= vx2 && sy <= vy2) 
				{
					if(!bOnMouseZone) 
					{
						bOnMouseZone = true;
                        if (eID.expand.scriptMouseEnter != null)
                        {
                            await PlayExpandScript(form, eID.expand.scriptMouseEnter);
                        }

                        if (eID.expand.structMouseZone != null)
                        {
                            InvalidateObject(form);
                            CheckSameTagMouseZone(bOnMouseZone);
                        }
					}
				}
				else 
				{
					if(bOnMouseZone) 
					{
						bOnMouseZone = false;
                        if (eID.expand.scriptMouseLeave != null)
                        {
                            await PlayExpandScript(form, eID.expand.scriptMouseLeave);
                        }

                        if (eID.expand.structMouseZone != null)
                        {
                            InvalidateObject(form);
                            CheckSameTagMouseZone(bOnMouseZone);
                        }
					}
				}
			}

			if(bMouseCapture == false)	return false;

			if(eID.active == 1 && eID.expand.sliderHorz != null) 
			{
				if(eID.expand.sliderHorz.nTagPos[0] != TagLib.TAG_NOT_FOUND) 
				{
					int left = nLeft < nRight ? nLeft : nRight;
					int x1 = GetViewPosX(left-(int)eID.expand.sliderHorz.nZoneMin)+nSliderGabX;
                    int x2 = GetViewPosX(left + (int)eID.expand.sliderHorz.nZoneMax) + nSliderGabX;

					double val;

					TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderHorz.sTag, ref eID.expand.sliderHorz.nTagPos);

					if(eID.expand.sliderHorz.bUseFullBase == 1) 
						val = GetPercent((float)e.X, (float)x1, (float)x2, (float)ai.fBase, (float)ai.fFull);
					else
						val = GetPercent((float)e.X, (float)x1, (float)x2, (float)eID.expand.sliderHorz.fValueMin, (float)eID.expand.sliderHorz.fValueMax);

					if(SharedData.userInfo.HaveRightsHandOperationAndMsgAtScript(ai)) 
					{
					
						await TagWrite.WriteCurrAI(eID.expand.sliderHorz.sTag, ai, val, true);

						DateTime t = DateTime.Now;
						eID.expand.sliderHorz.nWriteTimer = 5;
						eID.expand.sliderHorz.nWriteTimerOldSec = t.Second;
						eID.expand.sliderHorz.fWriteValue = val;

                        await EventTimerAsync(form);	// 메모리 태그일 경우 바의 위치를 바로 결정하기 위해서 한번 실행
					
					}
				}
			}

			if(eID.active == 1 && eID.expand.sliderVert != null) 
			{
				if(eID.expand.sliderVert.nTagPos[0] != TagLib.TAG_NOT_FOUND) 
				{
					int top = nTop < nBottom ? nTop : nBottom;
                    int y1 = GetViewPosY(top - (int)eID.expand.sliderVert.nZoneMin) + nSliderGabY;
                    int y2 = GetViewPosY(top + (int)eID.expand.sliderVert.nZoneMax) + nSliderGabY;

					double val;

					TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderVert.sTag, ref eID.expand.sliderVert.nTagPos);

					if(eID.expand.sliderVert.bUseFullBase == 1) 
						val = GetPercent((float)e.Y, (float)y1, (float)y2, (float)ai.fBase,  (float)ai.fFull);
					else
						val = GetPercent((float)e.Y, (float)y1, (float)y2, (float)eID.expand.sliderVert.fValueMin,  (float)eID.expand.sliderVert.fValueMax);

					if(SharedData.userInfo.HaveRightsHandOperationAndMsgAtScript(ai)) 
					{
					
						await TagWrite.WriteCurrAI(eID.expand.sliderVert.sTag, ai, val, true);

						DateTime t = DateTime.Now;
						eID.expand.sliderVert.nWriteTimer = 5;
						eID.expand.sliderVert.nWriteTimerOldSec = t.Second;
						eID.expand.sliderVert.fWriteValue = val;

						await EventTimerAsync(form);	// 메모리 태그일 경우 바의 위치를 바로 결정하기 위해서 한번 실행
					}
				}
			}

			return true;
		}

        bool bRightMouseCapture = false;

		public virtual async Task<bool> WmRightButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            if (!CheckResponseOnVisible()) return false;

            int sx, sy;
            int vx1, vx2, vy1, vy2;

            GetMousePositionByRotate(e, out sx, out sy);

            vx1 = nViewX1;
            vy1 = nViewY1;
            vx2 = nViewX2;
            vy2 = nViewY2;

            if (vx1 > vx2) Tools.Temp(ref vx1, ref vx2);
            if (vy1 > vy2) Tools.Temp(ref vy1, ref vy2);

            if (sx < vx1 || sy < vy1 || sx > vx2 || sy > vy2) return false;

            if (eID.active == 1 && eID.expand.scriptMouseRightDown != null)
            {
                eID.expand.scriptMouseRightDown.SetHandOperation();	// 수동으로 출력한다.
                await eID.expand.scriptMouseRightDown.RunAsync(form, this);
                if (eID.expand.scriptMouseRightDown.IsError())
                {
                    string message;
                    message = eID.expand.scriptMouseRightDown.GetError();
                    MessageDisplay.Show(message);
                }

                if (eID.expand.scriptMouseRightUp != null)
                {	  // 마우스 Up 스크립트가 있을 때만.
                    form.Capture = true;
                    bRightMouseCapture = true;
                }
                return true;
            }
            else
            {
                if (eID.active == 1 && eID.expand.scriptMouseRightUp != null)
                {
                    form.Capture = true;
                    bRightMouseCapture = true;
                    return true;
                }
            }

            return false;
		}

		public virtual async Task<bool> WmRightButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
		{
            if (bRightMouseCapture == false) return false;

            if (eID.active == 1 && eID.expand.scriptMouseRightUp != null)
            {
                eID.expand.scriptMouseRightUp.SetHandOperation();	// 수동으로 출력한다.
                await eID.expand.scriptMouseRightUp.RunAsync(form, this);
                if (eID.expand.scriptMouseRightUp.IsError())
                {
                    string message;
                    message = eID.expand.scriptMouseRightUp.GetError();
                    MessageDisplay.Show(message);
                }
            }

            bRightMouseCapture = false;
            form.Capture = false;

            return true;
		}

        public void GetZone(ref RECT r)
        {
            r.left = nLeft;
            r.top = nTop;
            r.right = nRight;
            r.bottom = nBottom;
        }

		public virtual void GetZone(ref int x1, ref int y1, ref int x2, ref int y2)
		{
			x1 = nLeft;
			y1 = nTop;
			x2 = nRight;
			y2 = nBottom;
		}

		public void GetViewZone(ref int x1, ref int y1, ref int x2, ref int y2)
		{
            x1 = nViewX1;
            y1 = nViewY1;
            x2 = nViewX2;
            y2 = nViewY2;
		}

        // 회전이 적용된 ViewZone
        protected void GetViewZoneWithRotation(ref int x1, ref int y1, ref int x2, ref int y2)  
        {
            if (RotationAngle == 0)
            {
                x1 = nViewX1 - nDisplayOriginX;
                y1 = nViewY1 - nDisplayOriginY;
                x2 = nViewX2 - nDisplayOriginX;
                y2 = nViewY2 - nDisplayOriginY;
            }
            else
            {
                x1 = nViewX1 - nRotationCenterX;
                y1 = nViewY1 - nRotationCenterY;
                x2 = nViewX2 - nRotationCenterX;
                y2 = nViewY2 - nRotationCenterY;
            }
        }

        // 이 Form 은 오직 SingleText를 위해서 필요하지만 이렇게 사용하는 것이 좋을 듯 하다. Form을 저장해서 하는것 보다 좋을 듯
		public virtual void UpdateZone(Form form, int x1, int y1, int x2, int y2)
		{
			nLeft = x1;
			nTop  = y1;
			nRight = x2;
			nBottom = y2;

			ExpandCalcObjectRect();	// 보여줄 사각형을 미리 계산해 놓는다.
		}

        bool bNeedInvalidateOnTimer = false;    // 타이머에서 Invalidate해주어야 한다.

		public virtual void OnVisible(bool flag) {} // 이것은 파생 클래스가 Child윈도우로 구성될 때 Window를 Show/Hide 하기위해서 필요하다.

        public virtual async Task EventTimerAsync(System.Windows.Forms.Form form)
        {
            if (bNeedInvalidateOnTimer)
            {
                form.Invalidate();
                bNeedInvalidateOnTimer = false;
            }

        	if(eID.active == 1) 
        	{
        		bool old_visible = ExpandCalcVisible();
                      EnumRunChangedType changed_type;

                var result = await ExpandRunAsync(form);
                if (result.changeFlag)
                {   // 변화가 있었다.
                    changed_type = result.changedType;

                    if ((changed_type & EnumRunChangedType.Rotation) > 0)
                    {
                        form.Invalidate();  // 일단 회전일 경우는 전체를 Invalidate한다.
                    }
                    else
                    {
                        // 각 변화별로 속도를 개선하기 위해서 필요한 동작만 한다.
                        if ((changed_type & EnumRunChangedType.Location) > 0 ||
                            (changed_type & EnumRunChangedType.Size) > 0 ||
                            (changed_type & EnumRunChangedType.Thick) > 0)
                        {
                            if (ExpandCalcObjectRect()) // 변화가 있으면
                            {
                                InvalidateOldObject(form);  // 오브젝트의 이전 위치를 갱신

                                if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                                {
                                    form.Invalidate();  // 부분적인 Invalidate가 잘 되지 않는다.
                                }

                            }
                        }

                        InvalidateObject(form);
                    }
                }

        		if(old_visible != ExpandCalcVisible()) 
        		{	// 이것은 파생클래스가 Child윈도우를 가질 때 사용 (ObjectActiveX)
        			OnVisible(ExpandCalcVisible());
        		}

        		if(eID.expand.scriptBlinking != null) 
        		{	// 점멸 옵션을 사용중일때는 각 주기가 되면 점멸을 시도한다.
        			if(nBlinkingCycle > 0 && bRunVisible) 
        			{
        				DateTime time = new DateTime();
        				int mili;
        				bool visible;

        				time = DateTime.Now;

        				mili = time.Millisecond+time.Second*1000+time.Minute*1000*60+time.Hour*1000*60*60;
        				mili %= nBlinkingCycle;

        				if((int)mili < nBlinkingCycle/2)	visible = true;
        				else								visible = false;

        				if(bVisibleByBlinking != visible) 
        				{
        					bVisibleByBlinking = visible;
        					InvalidateObject(form);
        					OnVisible(ExpandCalcVisible());
        				}
        			}
        		}
        	}

        	await EventTimerObject(form);
        }


        public virtual async Task EventTimerObject(System.Windows.Forms.Form form) { await Task.CompletedTask; }

		public async Task OnEventKeyDown(int code)
		{
			if(eID.active == 1 && eID.expand.scriptEventKeyDown != null) 
			{	
				eID.expand.scriptEventKeyDown.nKeyValue = code;
				eID.expand.scriptEventKeyDown.SetHandOperation();	// 수동으로 출력한다.
                await eID.expand.scriptEventKeyDown.RunAsync(objCommonProperty.form, this);
				if(eID.expand.scriptEventKeyDown.IsError()) 
				{
					string msg = eID.expand.scriptEventKeyDown.GetError();
					MessageBox.Show(msg, "Error:EventKeyDown");	
				}
			}
		}

		public async Task OnEventSelChange()
		{
			if(eID.active == 1 && eID.expand.scriptEventSelChange != null) 
			{	
				eID.expand.scriptEventSelChange.SetHandOperation();	// 수동으로 출력한다.
                await eID.expand.scriptEventSelChange.RunAsync(objCommonProperty.form, this);
				if(eID.expand.scriptEventSelChange.IsError()) 
				{
					string msg = eID.expand.scriptEventSelChange.GetError();
					MessageBox.Show(msg, "Error:EventSelChange");	
				}
			}
		}

		void DrawMouseZone(Graphics g, int x1, int y1, int x2, int y2)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			x1 -= eID.expand.structMouseZone.x1;
			y1 -= eID.expand.structMouseZone.y1;
			x2 += eID.expand.structMouseZone.x2;
			y2 += eID.expand.structMouseZone.y2;

			using (Pen hPenWhite = new Pen(Color.White, 1))
			using (Pen hPenBlack = new Pen(Color.Black, 1))
			{
				g.DrawLine(hPenWhite, x1, y1, x2, y1);
				g.DrawLine(hPenWhite, x2, y1, x2, y2);
				g.DrawLine(hPenWhite, x2, y2, x1, y2);
				g.DrawLine(hPenWhite, x1, y2, x1, y1);

				g.DrawLine(hPenBlack, x1-1, y1-1, x2+1, y1-1);
				g.DrawLine(hPenBlack, x2+1, y1-1, x2+1, y2+1);
				g.DrawLine(hPenBlack, x2+1, y2+1, x1-1, y2+1);
				g.DrawLine(hPenBlack, x1-1, y2+1, x1-1, y1-1);

				g.DrawLine(hPenBlack, x1+1, y1+1, x2-1, y1+1);
				g.DrawLine(hPenBlack, x2-1, y1+1, x2-1, y2-1);
				g.DrawLine(hPenBlack, x2-1, y2-1, x1+1, y2-1);
				g.DrawLine(hPenBlack, x1+1, y2-1, x1+1, y1+1);
			}
		}

        public bool IsMouseInViewZone(MouseEventArgs e)
        {
            int x1 = nViewX1;
            int x2 = nViewX2;
            int y1 = nViewY1;
            int y2 = nViewY2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (e.X < x1)  return false;
            if (e.Y < y1) return false;
            if (e.X > x2)   return false;
            if (e.Y > y2)    return false;

            return true;
        }

        protected virtual bool IsNeedPaint(Rectangle r, int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (x1 > r.Right)   return false;
            if (y1 > r.Bottom)  return false;
            if (x2 < r.Left)    return false;
            if (y2 < r.Top)     return false;

            return true;
        }

        [NonSerialized] private int nRotationCenterX = 0;
        [NonSerialized] private int nRotationCenterY = 0;
        [NonSerialized] private int nDisplayOriginX = 0;
        [NonSerialized] private int nDisplayOriginY = 0;

        public void CalcRotationCenter(out int cx, out int cy)
        {
            if (this.nBasicExpandOptionRotate == 1) // 회전축이 Left,Top
            {
                cx = (nViewX1 < nViewX2) ? nViewX1 : nViewX2;
                cy = (nViewY1 < nViewY2) ? nViewY1 : nViewY2;
            }
            else if (this.nBasicExpandOptionRotate == 2) // 회전축이 RIght,Top 
            {
                cx = (nViewX1 < nViewX2) ? nViewX2 : nViewX1;
                cy = (nViewY1 < nViewY2) ? nViewY1 : nViewY2;
            }
            else if (this.nBasicExpandOptionRotate == 3)// 회전축이 Left,Bottom
            {
                cx = (nViewX1 < nViewX2) ? nViewX1 : nViewX2;
                cy = (nViewY1 < nViewY2) ? nViewY2 : nViewY1;
            }
            else if (this.nBasicExpandOptionRotate == 4)// 회전축이 RIght,Bottom 
            {
                cx = (nViewX1 < nViewX2) ? nViewX2 : nViewX1;
                cy = (nViewY1 < nViewY2) ? nViewY2 : nViewY1;
            }
            else // 회전축이 중심
            {
                int w = Math.Abs(nViewX2 - nViewX1) + 1;
                int h = Math.Abs(nViewY2 - nViewY1) + 1;

                cx = (nViewX1 > nViewX2) ? nViewX2 + w / 2 : nViewX1 + w / 2;
                cy = (nViewY1 > nViewY2) ? nViewY2 + h / 2 : nViewY1 + h / 2;
            }
        }

		public virtual void Display(Graphics g, Rectangle rcPaint, int originx, int originy)
		{
			if(!ExpandCalcVisible())	return;

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE &&
                bSupportObjectOnCE == false)
            {
                nDisplayOriginX = originx;
                nDisplayOriginY = originy;

                int x1 = nViewX1 - originx;
                int y1 = nViewY1 - originy;
                int x2 = nViewX2 - originx;
                int y2 = nViewY2 - originy;

                if(x1 > x2) Tools.Temp(ref x1, ref x2);
                if(y1 > y2) Tools.Temp(ref y1, ref y2);

                Rectangle r = new Rectangle(x1, y1, x2-x1, y2-y1);

                DrawClass.gcls(g, x1, y1, x2, y2, Color.FromArgb(180, Color.Blue));

                string object_name = Path.GetExtension(this.ToString()).Substring(1);
                string text;
                
                if(Tools.IsLangKorean())
                    text = object_name+"\nCE에서 미 지원";
                else
                    text = object_name + "\nObject not supported on CE";

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                SafeException.SafeDrawString(g, text, new Font("Gulim", 9), Brushes.White, r, format);

                return;
            }

			//if(!IsNeedPaint(rcPaint, nViewX1, nViewY1, nViewX2, nViewY2))	return;     그룹 회전인 경우 BasePoint가 틀려져서 영역 검사가 잘 안된다.

            if (RotationAngle != 0) // 회전이 적용 되었다.
            {
                int cx;
                int cy;

                CalcRotationCenter(out cx, out cy);

                nRotationCenterX = cx;
                nRotationCenterY = cy;

                g.TranslateTransform(cx, cy);
                g.RotateTransform(RotationAngle);

                DisplayObject(g, nViewX1-cx, nViewY1-cy, nViewX2-cx, nViewY2-cy, nViewLineThick);

                if (eID.active == 1 && eID.expand.structMouseZone != null && (bOnMouseZone || bSameTagMouseZoneDisplay))
                {
                    DrawMouseZone(g, nViewX1-cx, nViewY1-cy, nViewX2-cx, nViewY2-cy);
                }

                g.ResetTransform();
            }
            else
            {
                nDisplayOriginX = originx;
                nDisplayOriginY = originy;

                DisplayObject(g, nViewX1 - originx, nViewY1 - originy, nViewX2 - originx, nViewY2 - originy, nViewLineThick);

                if (eID.active == 1 && eID.expand.structMouseZone != null && (bOnMouseZone || bSameTagMouseZoneDisplay))
                {
                    DrawMouseZone(g, nViewX1 - originx, nViewY1 - originy, nViewX2 - originx, nViewY2 - originy);
                }
            }
		}

		public override void SetBasePoint(int x, int y)
		{
			base.SetBasePoint(x, y);

			ExpandCalcObjectRect();
		}

		public override void SetScreenSize(int x, int y)
		{
			base.SetScreenSize(x, y);
	
			ExpandCalcObjectRect();
		}

		public override void SetOpticRate(int rate)
		{
			base.SetOpticRate(rate);
	
			ExpandCalcObjectRect();
		}

		public override void SetZoneAtPercent100Expand(SIZE size, RECT r)
		{
            base.SetZoneAtPercent100Expand(size, r);
	
			ExpandCalcObjectRect();
		}

		public virtual void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{

		}

		public void SetBorderThick(int thick)
		{
			wBorderThick  = thick;
			nRunThickLine = thick;
			ExpandCalcObjectRect();
		}

        public void InvalidateOldObject(System.Windows.Forms.Form form)
        {
            int gab = nViewOldLineThick / 2;
            int x1, y1, x2, y2;

            x1 = nViewOldX1;
            y1 = nViewOldY1;
            x2 = nViewOldX2;
            y2 = nViewOldY2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            x1 -= gab;
            y1 -= gab;
            x2 += gab;
            y2 += gab;

            if (eID.active == 1 && eID.expand.structMouseZone != null)
            {
                x1 -= (eID.expand.structMouseZone.x1 + 1);
                y1 -= (eID.expand.structMouseZone.y1 + 1);
                x2 += (eID.expand.structMouseZone.x2 + 1);
                y2 += (eID.expand.structMouseZone.y2 + 1);
            }

            Rectangle rc = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

            form.Invalidate(rc);
        }

		public void InvalidateObject(System.Windows.Forms.Form form)
		{
            if (TotalConfig.threadMain != System.Threading.Thread.CurrentThread)
            {
                bNeedInvalidateOnTimer = true;  // 스레드가 다르므로 메인스레드에서 무효화 한다.
                return;
            }

            if (RotationAngle == 0)
            {
                int gab = nViewLineThick / 2;
                int x1, y1, x2, y2;

                x1 = nViewX1;
                y1 = nViewY1;
                x2 = nViewX2;
                y2 = nViewY2;

                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                x1 -= gab;
                y1 -= gab;
                x2 += gab;
                y2 += gab;

                if (eID.active == 1 && eID.expand.structMouseZone != null)
                {
                    x1 -= (eID.expand.structMouseZone.x1 + 1);
                    y1 -= (eID.expand.structMouseZone.y1 + 1);
                    x2 += (eID.expand.structMouseZone.x2 + 1);
                    y2 += (eID.expand.structMouseZone.y2 + 1);
                }

                Rectangle rc = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

                form.Invalidate(rc);
            }
            else
            {
                form.Invalidate();  // 전체를 한다.
            }
		}

        public void InvalidateObject()
        {
            InvalidateObject(objCommonProperty.form);
        }

		public void SetLineColor(Color color)
		{
			lLineColor = color;
			lRunColorLine = color;
            IsUseLineColor = true;
		}

		public void SetFillColor(BrushPublic color)
		{
			lFillColor = color;
            lFillColor.org_basic_color = lFillColor.basic_color;    // run time시 basic_color가 바뀌므로 기본색상을 기억해 둔다.
			//lRunColorFill = color;
            IsUseFillColor = true;
		}

		public void SetTextColor(Color color)
		{
			lTextColor = color;
			lRunColorText = color;
            IsUseTextColor = true;
		}

        public void SetBackColor(BrushPublic color)
		{
			lBackColor = color;
            lBackColor.org_basic_color = lBackColor.basic_color;    // run time시 basic_color가 바뀌므로 기본색상을 기억해 둔다.
			//lRunColorBack = color;
            IsUseBackColor = true;
		}

		ScriptClass LoadExpandScriptFromMod(Profile profile, string ext, uint id)
		{
			if(id == 0)	return null;

			ScriptClass control;

			control = null;

			if(profile.GetIntFromReadyMemory(ext, "Active", 0) == 0)	return null;

			string scriptfile = "";

			MakeScriptPath(ref scriptfile, ext, id);

			if(!File.Exists(scriptfile))	return null;

			control = new ScriptClass();
			if(control == null)	return null;

			control.LoadFromFile(scriptfile);

			return control;
		}

        // MOD 파일은 스크립트 밖에 없다.
        ExpandBasicAndScript LoadExpandBasicAndScriptFromMod(Profile profile, string ext, uint id)
        {
            ScriptClass script = LoadExpandScriptFromMod(profile, ext, id);

            if (script == null) return null;

            ExpandBasicAndScript expand = new ExpandBasicAndScript();

            expand.eExpandType = EnumExpandType.Script; // MOD 파일은 항상 스크립트 형이다.
            expand.script = script;
            
            return expand;
        }

		EXPAND_SLIDER_STRUCT LoadExpandSlider(Profile profile, string ext, uint id)
		{
			if(id == 0)	return null;

			//string inifile="";
			EXPAND_SLIDER_STRUCT slider;

			slider = null;

			//MakeScriptPath(nTerminal, ref inifile, "INI", id);

			//if(Profile.GetPrivateProfileInt(ext, "Active", 0, inifile) == 0)	return null;
			if(profile.GetIntFromReadyMemory(ext, "Active", 0) == 0)	return null;

			string filename="";

			MakeScriptPath(ref filename, ext, id);

			if(!File.Exists(filename))	return null;

			slider = new EXPAND_SLIDER_STRUCT();
			if(slider == null)	return null;

			ExpandLoadSliderFile(filename, slider);

			return slider;
		}

		EXPAND_MOUSE_ZONE LoadExpandMouseZone(Profile profile, string ext, uint id)
		{
			if(id == 0)	return null;

			//string inifile="";
			EXPAND_MOUSE_ZONE zone;

			zone = null;

			//MakeScriptPath(nTerminal, ref inifile, "INI", id);

			//if(Profile.GetPrivateProfileInt(ext, "Active", 0, inifile) == 0)	return null;
			if(profile.GetIntFromReadyMemory(ext, "Active", 0) == 0)	return null;

			string filename="";

			MakeScriptPath(ref filename, ext, id);

			if(!File.Exists(filename))	return null;

			zone = new EXPAND_MOUSE_ZONE();
			if(zone == null)	return null;

			ExpandLoadMouseZoneFile(filename, zone);

			return zone;
		}

		// MOD버전에서.
		// 태그를 사용하는 오브젝트에서 Mouse 반응을 하기위해서 존재한다.
		protected void AddMouseZone(int x1, int y1, int x2, int y2)
		{
			eID.expand.structMouseZone = new EXPAND_MOUSE_ZONE();
			eID.expand.structMouseZone.x1 = x1;
			eID.expand.structMouseZone.y1 = y1;
			eID.expand.structMouseZone.x2 = x2;
			eID.expand.structMouseZone.y2 = y2;
			eID.active = 1;
		}

        public virtual void EventTag(System.Windows.Forms.Form form, TagPublicClass tagevent)
		{

		}

		void MakeScriptPath(ref string path, string ext, uint id)
		{
			string file;
			file = String.Format("OBJ{0:00000}.{1}", id, ext);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				if(GraphicTool.bLoadOnLibrary) 
				{
					path = GraphicTool.sLoadOnLibraryDir+"\\"+file;
				}
				else 
				{
					
					path = MakeFilePath.Graphic_ObjScr(file);
				}
			}
			else 
			{
				
				path = MakeFilePath.Graphic_ObjScr(file);
			}
		}

		void ExpandLoadMouseZoneFile(string filename, EXPAND_MOUSE_ZONE zone)
		{

            CommaTextReader comma = new CommaTextReader();
			string buf="";

			zone.bLock = 1;
			zone.x1 = 5;
			zone.y1 = 5;
			zone.x2 = 5;
			zone.y2 = 5;

			TextReader reader = new StreamReader(filename, System.Text.Encoding.Default);
			if(reader == null)	return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				comma.Set(buf);
				comma.GetString(ref buf);
				if(buf == "Zone") 
				{
					comma.GetInt(ref zone.x1);
					comma.GetInt(ref zone.y1);
					comma.GetInt(ref zone.x2);
					comma.GetInt(ref zone.y2);
				}
				else if(buf == "Lock") 
				{
					comma.GetChar(ref zone.bLock);
				}
				else {}
			}

			reader.Close();
		}

		void ExpandLoadSliderFile(string filename, EXPAND_SLIDER_STRUCT slider)
		{
            CommaTextReader comma = new CommaTextReader();
			string buf="";

			slider.nZoneMin = 0;
			slider.nZoneMax = 100;
			slider.fValueMin = 0;
			slider.fValueMax = 100;
			//slider.nTagPos = -1;

			TextReader reader = new StreamReader(filename, System.Text.Encoding.Default);
			if(reader == null)	return;

			while(true) 
			{
				buf = reader.ReadLine();
				if(buf == null)	break;

				comma.Set(buf);
				comma.GetString(ref buf);
				if(buf == "TagName") 
				{
					comma.GetString(ref slider.sTag);
					slider.sTag = slider.sTag.Trim();
				}
				else if(buf == "ZoneMin") 
				{
					comma.GetInt(ref slider.nZoneMin);
				}
				else if(buf == "ZoneMax") 
				{
                    comma.GetInt(ref slider.nZoneMax);
				}
				else if(buf == "ValueMin") 
				{
                    comma.GetFloat(ref slider.fValueMin);
				}
				else if(buf == "ValueMax") 
				{
                    comma.GetFloat(ref slider.fValueMax);
				}
			}

			reader.Close();

			EnumTagType tag_type = EnumTagType.none;

			if(TagLib.GetTagTypeAndPos(slider.sTag, ref tag_type, ref slider.nTagPos)) 
			{
			}
		}

        /// <summary>
        /// 기본 구현에서 async를 제거.
        /// sync -> 그대로 override
        /// async -> async override 
        /// </summary>
        public virtual Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
		{
            //return 0;
            return Task.FromResult<object>(0);
        }

        protected virtual bool ExecuteClassName_Signage(string command, out object retn_value, params object[] args)
        {
            retn_value = 0;
            return false;
        }

        bool bItIsGroup_CallByObjectRect = false;   // 2023-8-10추가 ObjectSetRect 함수를 사용할 때 그룹은 적용되지 않았다. 그룹도 지원하면서 ObjectExpand를 사용하거나 이 플래그가 ON되어 있으면 그룹크기를 변경할 수 있도록 한다.

        // retn_value 가 있으면 true를 반환 그렇지 않으면 false를 반환한다.
        // 현재는 ObjectGet() 함수만 true를 반환한다.
		public bool ExecuteClassNameOnlyObject(string command, out object retn_value, params object[] args)
		{
            if (String.Compare(command, 0, "Object", 0, 6) == 0)
            {
                if (command == "ObjectSetBackColor")
                {
                    BrushSolid brush = new BrushSolid(Color.FromArgb((int)args[1]));
                    SetBackColor(brush);
                    BackColorChanged();
                }
                else if (command == "ObjectSetTextColor")
                {
                    Color color = Color.FromArgb((int)args[1]);
                    SetTextColor(color);
                    TextColorChanged();
                }
                else if (command == "ObjectSetLineColor")
                {
                    Color color = Color.FromArgb((int)args[1]);
                    SetLineColor(color);
                }
                else if (command == "ObjectSetFillColor")
                {
                    Color color = Color.FromArgb((int)args[1]);
                    BrushPublic bp = new BrushPublic();
                    bp.basic_color = color;
                    SetFillColor(bp);
                }
                else if (command == "ObjectSetLineThick")
                {
                    int thick = (int)args[1];
                    SetBorderThick(thick);
                }
                else if (command == "ObjectSetLineOption")
                {
                    int option = (int)args[1];
                    nLineOption = option;
                }
                else if (command == "ObjectSetFillOption")
                {
                    int option = (int)args[1];
                    nFillOption = option;
                }
                else if (command == "ObjectSetToolTipText")
                {
                    string option = (string)args[1];

                    objGeneral.sObjectDescription = option;
                }
                else if (command == "ObjectSetText")
                {
                    string option = (string)args[1];

                    OnObjectSetText(option);
                }
                else if (command == "ObjectSetTag")
                {
                    string option = (string)args[1];

                    OnObjectSetTag(option);
                }
                else if (command == "ObjectGetTag")
                {
                    retn_value = GetTagName();
                    // return값이 있다.
                    return true;
                }
                else if (command == "ObjectSetRotationAxis")
                {
                    int rotation_axis = (int)args[1];

                    this.nBasicExpandOptionRotate = rotation_axis;
                }
                else if (command == "ObjectSetRect")
                {
                    nLeft = (int)args[1];
                    nTop = (int)args[2];
                    nRight = (int)args[3];
                    nBottom = (int)args[4];

                    if(enumObjectType == EnumObjectType.Group)
                        bItIsGroup_CallByObjectRect = true;

                    if (ExpandCalcObjectRect()) // 변화가 있으면
                    {
                        InvalidateOldObject(objCommonProperty.form);  // 오브젝트의 이전 위치를 갱신

                        if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                        {
                            objCommonProperty.form.Invalidate();  // 부분적인 Invalidate가 잘 되지 않는다.
                        }
                    }
                    InvalidateObject(objCommonProperty.form);

                    bItIsGroup_CallByObjectRect = true;
                }
                else if (command == "ObjectSetVisible")
                {
                    int flag = (int)args[1];

                    bRunVisible = (flag == 1);

                    InvalidateObject(objCommonProperty.form);

                    OnVisible(ExpandCalcVisible());
                }
                else if (command == "ObjectSetFont")
                {
                    string name = (string)args[1];
                    float size = (float)args[2];
                    int style = (int)args[3];

                    logFont.lfFaceName = name;
                    logFont.lfHeight = size;
                    logFont.style = FontStyle.Regular;
                    if ((style & 0x1) > 0)
                        logFont.style |= FontStyle.Bold;
                    if ((style & 0x2) > 0)
                        logFont.style |= FontStyle.Italic;
                    if ((style & 0x4) > 0)
                        logFont.style |= FontStyle.Underline;
                    if ((style & 0x8) > 0)
                        logFont.style |= FontStyle.Strikeout;

                    OnObjectSetFont();
                }
                else if (command == "ObjectSetFontName")
                {
                    string name = (string)args[1];

                    logFont.lfFaceName = name;

                    OnObjectSetFont();
                }
                else if (command == "ObjectSetFontSize")
                {
                    float size = (float)args[1];

                    logFont.lfHeight = size;

                    OnObjectSetFont();
                }
                else if (command == "ObjectSetFontStyle")
                {
                    int style = (int)args[1];

                    logFont.style = FontStyle.Regular;
                    if ((style & 0x1) > 0)
                        logFont.style |= FontStyle.Bold;
                    if ((style & 0x2) > 0)
                        logFont.style |= FontStyle.Italic;
                    if ((style & 0x4) > 0)
                        logFont.style |= FontStyle.Underline;
                    if ((style & 0x8) > 0)
                        logFont.style |= FontStyle.Strikeout;

                    OnObjectSetFont();
                }
                else if (command == "ObjectGetRect")
                {
                    // public virtual void GetZone(ref int x1, ref int y1, ref int x2, ref int y2)
                    args[1] = nLeft;
                    args[2] = nTop;
                    args[3] = nRight;
                    args[4] = nBottom;
                }
                else if (command == "ObjectGetPosition") //251113 PSU 추가.
                {
                    args[1] = nRunLocationX;
                    args[2] = nRunLocationY;
                }
                else if (command == "ObjectSetPosition") 
                {
                    nRunLocationX = (int)args[1];
                    nRunLocationY = (int)args[2];

                    if (enumObjectType == EnumObjectType.Group)
                        bItIsGroup_CallByObjectRect = true;

                    if (ExpandCalcObjectRect()) // 변화가 있으면
                    {
                        InvalidateOldObject(objCommonProperty.form);  // 오브젝트의 이전 위치를 갱신

                        if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                        {
                            objCommonProperty.form.Invalidate();  // 부분적인 Invalidate가 잘 되지 않는다.
                        }
                    }
                }
                else if (command == "ObjectGetSize") //251113 PSU 추가.
                {
                    args[1] = fRunPercentWidth;
                    args[2] = fRunPercentHeight;
                }
                else if (command == "ObjectSetSize")
                {
                    fRunPercentWidth = (int)args[1];
                    fRunPercentHeight = (int)args[2];

                    if (enumObjectType == EnumObjectType.Group)
                        bItIsGroup_CallByObjectRect = true;

                    if (ExpandCalcObjectRect()) // 변화가 있으면
                    {
                        InvalidateOldObject(objCommonProperty.form);  // 오브젝트의 이전 위치를 갱신

                        if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                        {
                            objCommonProperty.form.Invalidate();  // 부분적인 Invalidate가 잘 되지 않는다.
                        }
                    }
                }
                else if (command == "ObjectSetRotationAngle")
                {
                    fRunRotationAngle = (float)args[1];
                }
                else if (command == "ObjectGetRotationAngle")
                {
                   args[1] =  fRunRotationAngle ;
                }
                else
                {
                    retn_value = 0;
                    return false;
                }
            }
            else if (String.Compare(command, 0, "Signage", 0, 7) == 0)
            {
                return ExecuteClassName_Signage(command, out retn_value, args);
            }
            else
            {
                retn_value = 0;
                return false;
            }

            retn_value = 1;
			return false;

            // return true/false 가 성공실패가 아니라 return값이 있다/없다 이다.
		}

        protected virtual void OnObjectSetFont() 
        {
            InvalidateObject();
        }

        protected virtual void OnObjectSetText(string text) { }
        protected virtual void OnObjectSetTag(string tag) { }
        //protected virtual string OnObjectGetTag() { return ""; }

        /*
		public virtual string ExecuteClassNameStringReturn(string command, params object[] args)
		{
			return "";
		}*/

		public void GetExpandIdStruct(EXPAND_ID_STRUCT eid) 
		{ 
			eid.active = eID.active;
			eid.id = eID.id;
		}

		public static void SaveScriptToModX(CommaTextWriter writer, string name, ScriptClass script)
		{
			if(script == null)	return;

			writer.WriteLine("\t{0},BEGIN", name);
			script.SaveFile(writer);
			writer.WriteLine("\t{0},END", name);
		}

		void SaveScriptToModX(CommaTextWriter writer, string name, EXPAND_MOUSE_ZONE script)
		{
			if(script == null)	return;

			writer.WriteLine("\t{0},BEGIN", name);
			script.SaveFile(writer);
			writer.WriteLine("\t{0},END", name);
		}

		void SaveScriptToModX(CommaTextWriter writer, string name, EXPAND_SLIDER_STRUCT script)
		{
			if(script == null)	return;

			writer.WriteLine("\t{0},BEGIN", name);
			script.SaveFile(writer);
			writer.WriteLine("\t{0},END", name);
		}

        void SaveExpandToModX(CommaTextWriter writer, string name, ExpandBasicAndScript expand)
        {
            if (expand == null) return;

            if (expand.basic != null)   // 설정되어 있을때만 사용한다.
            {
                writer.WriteLine("\t{0},BEGIN", "Expand" + name);
                writer.WriteLine("\t\tExpandType,{0},", (int)expand.eExpandType);
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion con = (ExpandBasicConversion)expand.basic;
                    writer.WriteLine("\t\tExpandBasicConversion,{0},{1},{2},{3},{4},{5},", con.sTag, con.nBasicConversionType, con.fBasicSourceMin, con.fBasicSourceMax, con.fBasicTargetMin, con.fBasicTargetMax);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    writer.WriteLine("\t\tExpandBasicVisible,{0},{1},{2},", con.sTag, con.condition, con.value);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    writer.WriteLine("\t\tExpandBasicBlinking,{0},{1},{2},{3}", con.sTag, con.condition, con.value, con.cycle);
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
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate con = (ExpandBasicRotate)expand.basic;
                    writer.WriteLine("\t\tExpandBasicRotate,{0},{1},{2},{3},{4},{5},{6},", con.sTag, con.nRotateAxis, con.fBasicSourceMin, con.fBasicSourceMax, con.fBasicTargetMin, con.fBasicTargetMax, con.nDirection);
                }
                else
                {
                    MessageBox.Show("unknown type at ObjectExpand.SaveExpandToModX", expand.basic.ToString());
                }
                writer.WriteLine("\t{0},END", "Expand" + name);
            }

            SaveScriptToModX(writer, "Script" + name, expand.script);
        }

		public void SaveExpandScript(CommaTextWriter writer)
		{
			writer.WriteLine("\tExpandScript,BEGIN");
			writer.WriteLine("\t\tActive,{0}", eID.active);
			//SaveScriptToModX(writer, "ScriptSizeWidth", eID.expand.scriptSizeWidth);
            SaveExpandToModX(writer, "SizeWidth", eID.expand.scriptSizeWidth);
            SaveExpandToModX(writer, "SizeHeight", eID.expand.scriptSizeHeight);
            SaveExpandToModX(writer, "LocationX", eID.expand.scriptLocationX);
            SaveExpandToModX(writer, "LocationY", eID.expand.scriptLocationY);
			SaveScriptToModX(writer, "ScriptEventKeyDown", eID.expand.scriptEventKeyDown);
			SaveScriptToModX(writer, "ScriptEventSelChange", eID.expand.scriptEventSelChange);
            SaveScriptToModX(writer, "ScriptMouseDown", eID.expand.scriptMouseLeftDown);
            SaveScriptToModX(writer, "ScriptMouseUp", eID.expand.scriptMouseLeftUp);
            SaveScriptToModX(writer, "ScriptMouseRightDown", eID.expand.scriptMouseRightDown);
            SaveScriptToModX(writer, "ScriptMouseRightUp", eID.expand.scriptMouseRightUp);
            SaveScriptToModX(writer, "ScriptMouseEnter", eID.expand.scriptMouseEnter);
            SaveScriptToModX(writer, "ScriptMouseLeave", eID.expand.scriptMouseLeave);
            SaveScriptToModX(writer, "ScriptMouseMove", eID.expand.scriptMouseMove);

			SaveScriptToModX(writer, "ScriptMouseZone", eID.expand.structMouseZone);
            SaveExpandToModX(writer, "Visible", eID.expand.scriptVisible);
            SaveExpandToModX(writer, "Blinking", eID.expand.scriptBlinking);
            SaveExpandToModX(writer, "AnimationSpeed", eID.expand.scriptAnimationSpeed);
			SaveScriptToModX(writer, "ScriptSliderHorz", eID.expand.sliderHorz);
			SaveScriptToModX(writer, "ScriptSliderVert", eID.expand.sliderVert);
            SaveExpandToModX(writer, "ColorLine", eID.expand.scriptColorLine);
            SaveExpandToModX(writer, "ColorFill", eID.expand.scriptColorFill);
            SaveExpandToModX(writer, "ColorText", eID.expand.scriptColorText);
            SaveExpandToModX(writer, "ColorBack", eID.expand.scriptColorBack);
            SaveExpandToModX(writer, "ThickLine", eID.expand.scriptThickLine);
            SaveExpandToModX(writer, "Rotate", eID.expand.scriptRotate);
			writer.WriteLine("\tExpandScript,END");
		}

		public virtual void GetFamilyFile(ArrayList block)
		{

		}

		public virtual void ChangeFamilyFile(ArrayList block)
		{

		}

		void GetAllTagListOfOneScript(ArrayList block, ScriptClass script, string used_position)
		{
			if(script == null)	return;
			script.GetMultiSelectTagList(block, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, used_position);
		}

        void GetAllTagListOfOneScript(ArrayList block, EXPAND_SLIDER_STRUCT script, string used_position)
		{
			if(script == null)	return;
            TagUtil.AddTagList(block, script.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, used_position);
		}

        void GetAllTagListOfOneExpand(ArrayList block, ExpandBasicAndScript expand, string position)
        {
            if (expand == null) return;

            if (expand.basic != null)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion con = (ExpandBasicConversion)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    TagLib.GetTagTypeAndPos(con.sTag, ref con.tag_type, ref con.tag_pos);
                    TagUtil.AddTagList(block, con.sTag, con.tag_type, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    TagLib.GetTagTypeAndPos(con.sTag, ref con.tag_type, ref con.tag_pos);
                    TagUtil.AddTagList(block, con.sTag, con.tag_type, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag con = (ExpandBasicOnlyTag)expand.basic;
                    TagLib.GetTagTypeAndPos(con.sTag, ref con.tag_type, ref con.tag_pos);
                    TagUtil.AddTagList(block, con.sTag, con.tag_type, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;
                    for (int i = 0; i < con.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)con.member[i];
                        TagLib.GetTagTypeAndPos(member.sTag, ref member.tag_type, ref member.tag_pos);
                        TagUtil.AddTagList(block, member.sTag, member.tag_type, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                    }
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate con = (ExpandBasicRotate)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else
                {
                }

            }

            GetAllTagListOfOneScript(block, expand.script, position);
        }

		public virtual void GetMultiSelectTagList(ArrayList block)
		{
			GetAllTagListOfOneExpand(block, eID.expand.scriptSizeWidth, "Expand:Width");
            GetAllTagListOfOneExpand(block, eID.expand.scriptSizeHeight, "Expand:Height");
            GetAllTagListOfOneExpand(block, eID.expand.scriptLocationX, "Expand:X");
            GetAllTagListOfOneExpand(block, eID.expand.scriptLocationY, "Expand:Y");
			GetAllTagListOfOneScript(block, eID.expand.scriptEventKeyDown, "Expand:EventKeyDown");
            GetAllTagListOfOneScript(block, eID.expand.scriptEventSelChange, "Expand:EventSelChange");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseLeftDown, "Expand:MouseDown");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseLeftUp, "Expand:MouseUp");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseRightDown, "Expand:MouseRightDown");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseRightUp, "Expand:MouseRightUp");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseEnter, "Expand:MouseEnter");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseLeave, "Expand:MouseLeave");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseMove, "Expand:MouseMove");

            GetAllTagListOfOneExpand(block, eID.expand.scriptVisible, "Expand:Visible");
            GetAllTagListOfOneExpand(block, eID.expand.scriptBlinking, "Expand:Blinking");
            GetAllTagListOfOneExpand(block, eID.expand.scriptAnimationSpeed, "Expand:AnimationSpeed");
			GetAllTagListOfOneScript(block, eID.expand.sliderHorz, "Expand:SliderHorz");
            GetAllTagListOfOneScript(block, eID.expand.sliderVert, "Expand:SliderVert");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorLine, "Expand:ColorLine");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorFill, "Expand:ColorFill");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorText, "Expand:ColorText");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorBack, "Expand:ColorBack");
            GetAllTagListOfOneExpand(block, eID.expand.scriptThickLine, "Expand:LineThick");
            GetAllTagListOfOneExpand(block, eID.expand.scriptRotate, "Expand:Rotate");
		}

		void SetAllTagListOfOneScript(ArrayList block, ScriptClass script)
		{
			if(script == null)	return;
			script.SetMultiSelectTagList(block);
		}

		void SetAllTagListOfOneScript(ArrayList block, EXPAND_SLIDER_STRUCT script)
		{
			if(script == null)	return;

			string tag = script.sTag;
			if(ObjectGroup.IsNeedUpdateTag(block, ref tag))
				script.sTag = tag;
		}

        void SetAllTagListOfOneExpand(ArrayList block, ExpandBasicAndScript expand)
        {
            if (expand == null) return;

            string tag;
            if (expand.basic != null)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion con = (ExpandBasicConversion)expand.basic;

                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag con = (ExpandBasicOnlyTag)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;
                    for (int i = 0; i < con.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)con.member[i];
                        tag = member.sTag;
                        if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                            member.sTag = tag;
                    }
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate con = (ExpandBasicRotate)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else
                {
                }
            }
            
            SetAllTagListOfOneScript(block, expand.script);
        }

		public virtual void SetMultiSelectTagList(ArrayList block)
		{
			SetAllTagListOfOneExpand(block, eID.expand.scriptSizeWidth);
            SetAllTagListOfOneExpand(block, eID.expand.scriptSizeHeight);
            SetAllTagListOfOneExpand(block, eID.expand.scriptLocationX);
            SetAllTagListOfOneExpand(block, eID.expand.scriptLocationY);
			SetAllTagListOfOneScript(block, eID.expand.scriptEventKeyDown);
			SetAllTagListOfOneScript(block, eID.expand.scriptEventSelChange);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseLeftDown);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseLeftUp);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseRightDown);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseRightUp);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseEnter);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseLeave);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseMove);

            SetAllTagListOfOneExpand(block, eID.expand.scriptVisible);
            SetAllTagListOfOneExpand(block, eID.expand.scriptBlinking);
            SetAllTagListOfOneExpand(block, eID.expand.scriptAnimationSpeed);
			SetAllTagListOfOneScript(block, eID.expand.sliderHorz);
			SetAllTagListOfOneScript(block, eID.expand.sliderVert);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorLine);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorFill);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorText);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorBack);
            SetAllTagListOfOneExpand(block, eID.expand.scriptThickLine);
            SetAllTagListOfOneExpand(block, eID.expand.scriptRotate);
		}

		public virtual async Task EventTimerObjectOnPreview(System.Windows.Forms.Form form) 
		{
			await EventTimerObject(form);
		}

		[NonSerialized] protected bool bPreviewMode = false;

		public virtual async Task EventTimerOnPreview(System.Windows.Forms.Form form)
		{
			bPreviewMode = true;
			await EventTimerObjectOnPreview(form);
		}

		public virtual void AddObjectInfo(TreeNode parent)
		{
			TreeNode node = new TreeNode(Path.GetExtension(this.ToString()).Substring(1));
			//parent.Nodes.Add(node);
            parent.Nodes.Insert(0, node);
		}

		public delegate void DelegateCallBackObject(object obj);

        public virtual void EditModeOnDelete() { }

        //public virtual void Dispose()
        //{

        //}

        // 레이어 창에서 사용하는 조그만 미리보기 비트맵을 만든다.
        public void MakeLayerPreview(int width, int height)
        {
            previewOnStudio = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(previewOnStudio);
            MakeLayerPreviewObject(g, width, height, nViewLineThick > 3 ? 3 : nViewLineThick);
        }

        protected virtual void MakeLayerPreviewObject(Graphics g, int width, int height, int thick)
        {
            int vw = Math.Abs(nViewX2 - nViewX1)+1;
            int vh = Math.Abs(nViewY2 - nViewY1)+1;

            int ry = width * vh / vw;
            int rx = height * vw / vh;

            int x1, y1, x2, y2;

            if (vw == 1) rx = 1;    // size가 0 이라면 계산할 결과도 0 이다
            if (vh == 1) ry = 1;    // size가 0 이라면 계산할 결과도 0 이다

            if (rx <= width)    // 속에 들어온다.
            {
                ry = height;
                if (vw != 1 && rx < 3) rx = 3;    // 너무 작은 경우  모양을 알 수 있도록 적당히 키워준다.
            }
            else
            {
                rx = width;
                if (vh != 1 && ry < 3) ry = 3;    // 너무 작은 경우  모양을 알 수 있도록 적당히 키워준다.
            }

            if (rx == 0) rx = 1;
            if (ry == 0) ry = 1;

            if (rx == width)
            {
                x1 = 0;
                x2 = width - 1;
            }
            else
            {
                x1 = width / 2 - rx / 2;
                x2 = x1 + rx - 1;
            }
            if (ry == height)
            {
                y1 = 0;
                y2 = height - 1;
            }
            else
            {
                y1 = height / 2 - ry / 2;
                y2 = y1 + ry - 1;
            }

            DisplayPreviewObject(g, x1, y1, x2, y2, thick);
        }

        protected virtual void DisplayPreviewObject(Graphics g, int x1, int y1, int x2, int y2, int thick)
        {
            DisplayObject(g, x1, y1, x2, y2, thick);
        }

        public virtual void EditFlipHorz() 
        {

        }

        public virtual void EditFlipVert() 
        {
            
        }

        // 크기와 회전이 함께 적용되므로 UpdataZone도 함께 처리한다. Curve나 Poly 때문에 함께 처리 해야 함
        public virtual void EditRotateRight(int nx1, int ny1, int nx2, int ny2) 
        {
            UpdateZone(objCommonProperty.form, nx1, ny1, nx2, ny2);
        }

        public virtual void EditRotateLeft(int nx1, int ny1, int nx2, int ny2) 
        {
            UpdateZone(objCommonProperty.form, nx1, ny1, nx2, ny2);
        }

        public virtual async Task MakeWebPublishFile(string target_dir)
        {
            await Task.CompletedTask;
        }

        public virtual void EditReCalcGroupSize()
        {

        }

        // autobase 11에서는 Version3 형식으로 바꾸어주는 것이 좋다. 저장 시 Version2 인 경우는 새로 저장하도록 한다.
        public virtual void SaveAnimationFileToVersion3()
        {
            
        }

        [NonSerialized]
        protected bool bLastEnabledState = true; // 기본 상태 저장

        // 컨트롤 상태를 업데이트하는 메서드
        protected void UpdateControlState(Control control, Form formParent)
        {
            bool shouldBeEnabled = IsViewModeControl(formParent);
            if (bLastEnabledState != shouldBeEnabled)
            {
                control.Enabled = shouldBeEnabled;
                bLastEnabledState = shouldBeEnabled;
            }
        }

        // 뷰 모드를 확인하는 메서드
        protected bool IsViewModeControl(Form formParent)
        {
            // FormGraphicChild의 viewMode 확인
            if (formParent != null && formParent is GraphicModule.FormGraphicChild)
            {
                GraphicModule.FormGraphicChild parentForm = (GraphicModule.FormGraphicChild)formParent;
                return parentForm.cViewMode == EnumViewMode.CONTROL;
            }
            return true; // 기본값은 컨트롤 모드임
        }
	}

}

