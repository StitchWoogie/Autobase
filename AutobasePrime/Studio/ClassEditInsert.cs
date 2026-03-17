using System;
using GraphicModule;
using NetTools.OldDefine;
using System.Drawing;
using DialogTag;
using System.Windows.Forms;
using System.Collections;
using NetTools;
using System.IO;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditInsert.
	/// </summary>
	public class ClassEditInsert
	{
		public ClassEditInsert()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static int nNewPosX = 10;
		static int nNewPosY = 50;

		static int GetNewPosX(FormEditGraphic form)
		{
			nNewPosX += 10;
			if(nNewPosX >= 100)	nNewPosX = 10;

			return form.GetPicturePosX(nNewPosX);
		}

		static int GetNewPosY(FormEditGraphic form)
		{
			nNewPosY += 10;
			if(nNewPosY >= 100)	nNewPosY = 10;

			return form.GetPicturePosY(nNewPosY);
		}

		public static LOGFONT GetNewFont()
		{
            LOGFONT logFont = new LOGFONT();

            if (NetTools.Tools.IsLangKorean())
            {
                // 6 = Vista
                if (Environment.OSVersion.Version.Major >= 6)
                    logFont.lfFaceName = "Malgun Gothic";
                else
                    logFont.lfFaceName = "Gulim";

                logFont.lfHeight = 10;
            }
            else if (NetTools.Tools.IsLangJapanese())
            {
                logFont.lfFaceName = "MS UI Gothic";
                logFont.lfHeight = 9;
            }
            else if (NetTools.Tools.IsLangChinese())
            {
                logFont.lfFaceName = "SimSun";
                logFont.lfHeight = 9;
            }
            else
            {
                logFont.lfFaceName = "Tahoma";
                logFont.lfHeight = 9;
            }

			return logFont;
		}

        /// <summary>
        /// 주어진 오브젝트의 위치배열을 찾는다
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="seek_obj"></param>
        /// <param name="opos">root 일때는 null이 아닌 int[0]를 반납한다.</param>
        /// <returns></returns>

        public static bool RecurseMakePos(ObjectPublicGroupLayer gl, object seek_obj, ref int[] opos)
        {
            if (gl == seek_obj)     // root 오브젝트이다 
            {
                opos = new int[0];
                return true;
            }

            int depth;

            if (opos == null)   depth = 0;
            else                depth = opos.Length;

            int[] opos2 = new int[depth + 1];

            for (int i = 0; i < depth; i++)
            {
                opos2[i] = opos[i];
            }

            ObjectType obj;
            for (int i = 0; i < gl.GetObjectHap(); i++)
            {
                opos2[depth] = i;

                obj = (ObjectType)gl.GetPoint(i);
                if (obj == seek_obj)
                {
                    opos = opos2;
                    return true;
                }

                if (obj.enumObjectType == EnumObjectType.Group || obj.enumObjectType == EnumObjectType.Layer)
                {
                    if (RecurseMakePos((ObjectPublicGroupLayer)obj, seek_obj, ref opos2))
                    {
                        opos = opos2;
                        return true;
                    }
                }
            }

            return false;
        }

        public static void InsertPublic(FormEditGraphic form, object obj, string undo_title)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            ObjectPublic parent;

            parent = ClassStudioEdit.GetTargetRoot(form);

            int[] opos = null;

            RecurseMakePos(work.obj.groupRoot, parent, ref opos);
            ClassStudioEditUndo.UndoSave_Add(form, undo_title, opos, ((ObjectPublicGroupLayer)parent).GetObjectHap(), 1);

            ((ObjectPublicGroupLayer)parent).AddObject(obj);
            ((ObjectPublicGroupLayer)parent).bOnStudioGroupOpen = true; // 추가한 레이어의 트리를 오픈한다.

            form.workThis.obj.SetCommonProperty();
            form.workThis.obj.SetOpticRate(form.workThis.obj.nOpticRate);       // 이 부분이 없으니 확대해서 그룹을 만들면 그룹의 선택위치가 엉뚱한 곳으로 간다. 2009-12-3
            form.workThis.obj.SetBasePoint(-form.workThis.nScrollHorPos, -form.workThis.nScrollVerPos);
            form.SetChangeFlag();	// ON
            form.SelectListClear(form.workThis);
            form.SelectListAdd(form.workThis, obj);

            form.DisplayAfterSelectedChanged();
            form.SelectNodeClear();
            form.Invalidate();
        }

		public static void EditInsertButtonModule3D(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+150;
			rect.bottom = rect.top+30;

			ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
			args.tcolor = Color.Black;
            //args.bcolor = new BrushSolid(Color.LightGray);
            args.bcolor = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0)); //20250204 PSU 기본배경색상 변경
            args.thick = 1; // 기본이 2이지만 10부터는 1칸짜리로 스마트하게 사용한다.
            args.designType = ButtonDesignType.Modern;
            args.radius = 4;

            args.sText = "ModuleButton3D";

			object obj = new ObjectButtonModule3D(work.obj.objCommonProperty, rect, null, GetNewFont(), new ObjectGeneral("ButtonModule3D1"), args, "noname.modx");

            InsertPublic(form, obj, "Insert Module Button");
		}

		public static void EditInsertButtonModuleHide(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+150;
			rect.bottom = rect.top+150;

		
			object obj = new ObjectButtonModuleHide(work.obj.objCommonProperty, rect, null, new ObjectGeneral("ButtonModuleHide1"), "noname.modx");

            InsertPublic(form, obj, "Insert Module Button Hide");
		}

		public static void EditInsertButtonProgram(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+150;
			rect.bottom = rect.top+30;

			ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
			args.tcolor = Color.Black;
            args.bcolor = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
            args.thick = 1; // 기본이 2이지만 10부터는 1칸짜리로 스마트하게 사용한다.
            args.designType = ButtonDesignType.Modern;
            args.radius = 4;
            args.sText = "ButtonProgram";
			
			object obj = new ObjectButtonProgram(work.obj.objCommonProperty, rect, null, GetNewFont(), new ObjectGeneral("ButtonProgram1"), args, 1, "noname.ctlx", null);

            InsertPublic(form, obj, "Insert Program Button");
		}

		public static void EditInsertButtonDigitalOut(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+150;
			rect.bottom = rect.top+30;

			ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
			args.tcolor = Color.Black;
            args.bcolor = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
            args.thick = 1; // 기본이 2이지만 10부터는 1칸짜리로 스마트하게 사용한다.
            args.designType = ButtonDesignType.Modern;
            args.radius = 4;
            args.sText = "DO ON/OFF";
		
			object obj = new ObjectButtonDigitalOut(work.obj.objCommonProperty, rect, null, GetNewFont(), new ObjectGeneral("ButtonDigitalOut1"), args, 0, null, 0);

            InsertPublic(form, obj, "Insert DigitalOut Button");
		}

		public static void EditInsertDigitalAnimation(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagDI = true;
			if(tag.Run(form) != DialogResult.OK)	return;

			FormSelectDigitalAnimationFromLibrary dialog = new FormSelectDigitalAnimationFromLibrary();

			string on_file;
			string off_file;
			if(dialog.ShowDialog(form) != DialogResult.OK)	return;

			ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, dialog.sFileNameON, out on_file);
			ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, dialog.sFileNameOFF, out off_file);
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+30;
			rect.bottom = rect.top+30;

			ObjectArgsDigitalAnimation args = new ObjectArgsDigitalAnimation();
			args.nOverlayMethod = 0;
			args.sFileOn = on_file;
			args.sFileOff = off_file;

			ObjectDigitalAnimation obj = new ObjectDigitalAnimation(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("DigitalAnimation1"), tag.sTag, null, null, args);
			obj.SetOriginalSize();

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;

            InsertPublic(form, obj, "Insert Digital Animation");
		}

		public static void EditInsertDigitalCircle(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagDI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+25;
			rect.bottom = rect.top+25;

			ObjectArgsDigitalCircle args = new ObjectArgsDigitalCircle();
			args.colorOn = Color.Red;
			args.colorOff = Color.Black;

			ObjectDigitalCircle obj = new ObjectDigitalCircle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("DigitalCircle1"), tag.sTag, null, null, args);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;

            InsertPublic(form, obj, "Insert Digital Circle");
		}

		public static void EditInsertDigitalRectangle(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagDI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+25;
			rect.bottom = rect.top+25;

			ObjectArgsDigitalRectangle args = new ObjectArgsDigitalRectangle();
			args.colorOn = Color.Red;
			args.colorOff = Color.Black;

			ObjectDigitalRectangle obj = new ObjectDigitalRectangle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("DigitalRect1"), tag.sTag, null, null, args);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Digital Rectangle");
		}

		public static void EditInsertDigitalString(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagDI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+70;
			rect.bottom = rect.top+25;

			ObjectArgsDigitalString args = new ObjectArgsDigitalString();
			args.colorOn = Color.Red;
			args.colorOff = Color.DarkGray;
			args.colorBack = new BrushSolid(Color.Black);


			ObjectDigitalString obj = new ObjectDigitalString(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("DigitalString1"), GetNewFont(), tag.sTag, null, null, args);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Digital String");
		}

		public static void EditInsertAnalogMeter(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagAI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+150;

			ObjectArgsAnalogMeter args = new ObjectArgsAnalogMeter();
			args.colorBack = new BrushSolid(Color.White);
			args.colorBorder = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.colorGuide = Color.Black;
			args.colorHand = Color.Red;
			args.colorText = Color.LightGray;
			args.thickHand = 3;

			ObjectAnalogMeter obj = new ObjectAnalogMeter(work.obj.objCommonProperty, rect, null, new ObjectGeneral("AnalogMeter1"), GetNewFont(), tag.sTag, null, null, args);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Analog Meter");
		}

		public static void EditInsertAnalogRectangle(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagAI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+60;
			rect.bottom = rect.top+200;

			ObjectArgsAnalogRectangle args = new ObjectArgsAnalogRectangle();
			args.colorOff = new BrushSolid(Color.Black);
			args.colorOn = new BrushSolid(Color.Red);
			args.nBarDir = 0;
            args.cornerRadius = 0; //250204 PSU 추가

            VIEW_RANGE_STRUCT view = new VIEW_RANGE_STRUCT();
			view.fBase = 0;
			view.fFull = 100;
			view.flag = 0;

			GUIDE_LINE_STRUCT guide = new GUIDE_LINE_STRUCT();
			guide.bLevelString = 0;
			guide.colorBig = Color.Black;
			guide.colorSmall = Color.Black;
			guide.devideBig = 2;
			guide.devideSmall = 10;
			guide.line_length = 7;
			guide.method = 1;

			
			ObjectAnalogRectangle obj = new ObjectAnalogRectangle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("AnalogRect1"), GetNewFont(), tag.sTag, null, null, args, view, guide);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Analog Rectangle");
		}

		public static void EditInsertAnalogRotate(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagAI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+100;

			ObjectArgsAnalogRotate args = new ObjectArgsAnalogRotate();
			args.bAngleDirection = 1;
			args.lBackColor = new BrushSolid(Color.White);
			args.nEndAngle = 360;
			args.nStartAngle = 0;
            args.nMethod = 1;           // Default를 원형배경으로 변경함 2016-9-2
            args.sFileName = "";

			
			ObjectAnalogRotate obj = new ObjectAnalogRotate(work.obj.objCommonProperty, rect, null, new ObjectGeneral("AnalogRotate1"), tag.sTag, null, null, args);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Analog Rotate");
		}

		public static void EditInsertAnalogStatus(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagAI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+50;
			rect.bottom = rect.top+50;

			ObjectArgsAnalogStatus args = new ObjectArgsAnalogStatus();

			ArrayList array = new ArrayList();
			ANALOG_STATUS_STRUCT status;

			for(int i = 0; i < 16; i++) 
			{
				status = new ANALOG_STATUS_STRUCT();
				status.type = 0;
				array.Add(status);
			}

			
			ObjectAnalogStatus obj = new ObjectAnalogStatus(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("AnalogStatus1"), tag.sTag, null, null, args, array);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Analog Status");
		}

		public static void EditInsertAnalogString(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagAI = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+25;

			ObjectArgsAnalogString args = new ObjectArgsAnalogString();

			args.colorBack = new BrushSolid(Color.White);
			args.colorText = Color.Black;
			args.nBoxUse = 2;
			args.nDisplayValue = 0;
			args.sDisplayFormat = "";

			
			ObjectAnalogString obj = new ObjectAnalogString(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("AnalogString1"), GetNewFont(), tag.sTag, null, null, args);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Analog String");
		}

        //아날로그게이지 추가 hsjeong 25-02-04
        public static void EditInsertAnalogGauge(FormEditGraphic form)
        {
            SelectTag tag = new SelectTag();
            tag.bUseTagAI = true;
            if (tag.Run(form) != DialogResult.OK) return;

            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 200;

            ObjectArgsAnalogGauge args = new ObjectArgsAnalogGauge();
            args.colorOff = new BrushSolid(Color.Black);
            args.colorOn = new BrushSolid(Color.Red);
            args.nBarDir = 0;
            args.nLineThick = 10;

            args.fStartAngle = 0;
            args.fSweepAngle = 120;

            VIEW_RANGE_STRUCT2 view = new VIEW_RANGE_STRUCT2();
            view.fBase = 0;
            view.fFull = 100;
            view.flag = 0;

            GUIDE_LINE_STRUCT2 guide = new GUIDE_LINE_STRUCT2();
            guide.bLevelString = 0;
            guide.colorBig = Color.Black;
            guide.colorSmall = Color.Black;
            guide.devideBig = 2;
            guide.devideSmall = 10;
            guide.line_length = 7;
            guide.method = 1;
            guide.nSpaceGuideLine = 0;
            guide.nSpaceLevelString = 20;

            ANALOG_GAGUE_POINT_MEMBER point = new ANALOG_GAGUE_POINT_MEMBER();
            point.nPointType = 0;
            point.color = Color.Black;
            point.nPointSize = 0;


            ObjectAnalogGauge obj = new ObjectAnalogGauge(work.obj.objCommonProperty, rect, null, new ObjectGeneral("AnalogGauge1"), GetNewFont(), tag.sTag, null, null, args, view, guide, point);

            obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
            obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Analog Gauge");
        }

		public static void EditInsertStringString(FormEditGraphic form)
		{
			SelectTag tag = new SelectTag();
			tag.bUseTagST = true;
			if(tag.Run(form) != DialogResult.OK)	return;
			
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+25;

			ObjectArgsStringString args = new ObjectArgsStringString();

			args.colorBack = new BrushSolid(Color.White);
			args.colorText = Color.Black;
			args.nBoxUse = 2;

			TEXT_ALIGN align = new TEXT_ALIGN();
			align.x = 0;
			align.y = 1;

			
			ObjectStringString obj = new ObjectStringString(work.obj.objCommonProperty, rect, null, new ObjectGeneral("StringTag1"), GetNewFont(), tag.sTag, null, null, args, align);

			obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
			obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert String Tag");
		}

        public static void EditInsertTagAnimation(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 100;
            rect.bottom = rect.top + 100;

            ObjectArgsTagAnimation args = new ObjectArgsTagAnimation();
            args.nOverlayMethod = 0;

            for (int i = 0; i < 4; i++)
            {
                TagAnimationMember member = new TagAnimationMember();
                member.active = true;
                member.condition = 5;   // On 조건

                if (i == 3)
                {
                    member.bDefault = true;
                }
                args.member.Add(member);
            }

            ObjectTagAnimation obj = new ObjectTagAnimation(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("TagAnimation1"), "", null, args);
            //obj.SetOriginalSize();

            
            //obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
            //obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Tag Animation");
        }

		public static void EditInsertModule(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+150;

			ObjectArgsModule args = new ObjectArgsModule();

			args.filename = "noname.modx";

			object obj = new ObjectModule(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Module1"), args);

            InsertPublic(form, obj, "Insert Module");
		}

		public static void EditInsertControlCheckBox(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+30;

			ObjectArgsControlCheckBox args = new ObjectArgsControlCheckBox();

			args.rgbColor = Color.Black;
			args.sTitle = "CheckBox";

			
			object obj = new ObjectControlCheckBox(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("CheckBox1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Check Box");
		}

		public static void EditInsertControlComboBox(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+150;

			ObjectArgsControlComboBox args = new ObjectArgsControlComboBox();

			
			object obj = new ObjectControlComboBox(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("ComboBox1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert ComboBox");
		}

		public static void EditInsertControlEditBox(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+30;

			ObjectArgsControlEditBox args = new ObjectArgsControlEditBox();

			
			object obj = new ObjectControlEditBox(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("EditBox1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert EditBox");
		}

		public static void EditInsertControlListBox(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+150;

			ObjectArgsControlListBox args = new ObjectArgsControlListBox();

			object obj = new ObjectControlListBox(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("ListBox1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert ListBox");
		}

		public static void EditInsertControlRadioButton(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+50;

			ObjectArgsControlRadioButton args = new ObjectArgsControlRadioButton();

			//args.rgbColor = Color.Black;
			//args.sTitle = "CheckBox";
			args.arrayListData = new ArrayList();
			args.arrayListData.Add("Radio Item1");
			args.arrayListData.Add("Radio Item2");
			args.rgbColor = Color.Black;

			
			object obj = new ObjectControlRadioButton(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Radio1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Radio Button");
		}

        public static void EditInsertControlDatePicker(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 30;

            ObjectArgsControlDatePicker args = new ObjectArgsControlDatePicker();

            args.sFormat = "yyyy-MM-dd";

            object obj = new ObjectControlDatePicker(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("DatePicker1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert DatePicker");
        }

        public static void EditInsertControlTabControl(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 200;

            ObjectArgsControlTabControl args = new ObjectArgsControlTabControl();

            args.sFormat = "yyyy-MM-dd";

            object obj = new ObjectControlTabControl(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("TabControl1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert TabControl");
        }

        public static void EditInsertControlTreeView(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 200;

            ObjectArgsControlTreeView args = new ObjectArgsControlTreeView();

            //args.sFormat = "yyyy-MM-dd";

            object obj = new ObjectControlTreeView(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("TreeView1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Tree View");
        }

		public static void EditInsertBitmap(FormEditGraphic form)
		{
			string filename = "";

			if(!PropertyPageObjectBitmap.SelectBitmapFile(ref filename))	return;

			ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, filename, out filename);

			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+50;

			ObjectArgsBitmap args = new ObjectArgsBitmap();

			args.sBitmapFile = filename;
			args.nOverlayMethod = 0;

			ObjectBitmap obj = new ObjectBitmap(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Bitmap1"), args);
            obj.SetOriginalSize();

            InsertPublic(form, obj, "Insert Bitmap");
		}

        //250917 PSU
        public static void EditInsertBitmapByDrag(FormEditGraphic form, string sourceFilename, Point dropPosition)
        {
            try
            {
                string filename = sourceFilename;

                // 기존과 동일하게 파일을 그래픽 디렉토리로 복사
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, filename, out filename);

                WORK_MODULE_STRUCT work = form.workThis;
                RECT rect = new RECT();

                // 드롭 위치를 기준으로 좌표 설정
                rect.left = dropPosition.X;
                rect.top = dropPosition.Y;
                rect.right = rect.left + 200;
                rect.bottom = rect.top + 50;

                ObjectArgsBitmap args = new ObjectArgsBitmap();
                args.sBitmapFile = filename;
                args.nOverlayMethod = 0;

                ObjectBitmap obj = new ObjectBitmap(work.obj.objCommonProperty, form, rect, null,
                                                  new ObjectGeneral("Bitmap1"), args);
                obj.SetOriginalSize();

                InsertPublic(form, obj, "Insert Bitmap by Drag");
            }
            catch (System.Exception ex)
            {
                string errorMsg = Tools.IsLangKorean() ?
                                 "비트맵 파일을 삽입할 수 없습니다: " + ex.Message :
                                 "Unable to insert bitmap file: " + ex.Message;
                string title = Tools.IsLangKorean() ? "오류" : "Error";

                System.Windows.Forms.MessageBox.Show(errorMsg, title,
                               System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        // Animation 드래그 앤 드롭 삽입 메서드
        public static void EditInsertAnimationByDrag(FormEditGraphic form, string sourceFilename, Point dropPosition)
        {
            try
            {
                string filename = sourceFilename;
                // 파일을 그래픽 디렉토리로 복사
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, filename, out filename);
                WORK_MODULE_STRUCT work = form.workThis;
                RECT rect = new RECT();
                // 드롭 위치를 기준으로 좌표 설정
                rect.left = dropPosition.X;
                rect.top = dropPosition.Y;
                rect.right = rect.left + 200;
                rect.bottom = rect.top + 50;
                ObjectArgsAnimation args = new ObjectArgsAnimation();
                args.sAnimationFile = filename;
                args.nOverlayMethod = 0;
                ObjectAnimation obj = new ObjectAnimation(work.obj.objCommonProperty, form, rect, null,
                                                          new ObjectGeneral("Animation1"), args);
                obj.SetOriginalSize();
                InsertPublic(form, obj, "Insert Animation by Drag");
            }
            catch (System.Exception ex)
            {
                string errorMsg = Tools.IsLangKorean() ?
                                 "Animation 파일을 삽입할 수 없습니다: " + ex.Message :
                                 "Unable to insert Animation file: " + ex.Message;
                string title = Tools.IsLangKorean() ? "오류" : "Error";
                System.Windows.Forms.MessageBox.Show(errorMsg, title,
                               System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // SVG 드래그 앤 드롭 삽입 메서드
        public static void EditInsertSVGByDrag(FormEditGraphic form, string sourceFilename, Point dropPosition)
        {
            try
            {
                string filename = sourceFilename;
                // 파일을 그래픽 디렉토리로 복사
                ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, filename, out filename);
                WORK_MODULE_STRUCT work = form.workThis;
                RECT rect = new RECT();
                // 드롭 위치를 기준으로 좌표 설정
                rect.left = dropPosition.X;
                rect.top = dropPosition.Y;
                rect.right = rect.left + 200;
                rect.bottom = rect.top + 50;
                ObjectArgsSVG args = new ObjectArgsSVG();
                args.sSvgFile = filename;
                args.nOverlayMethod = 0;
                ObjectSVG obj = new ObjectSVG(work.obj.objCommonProperty, form, rect, null,
                                              new ObjectGeneral("SVG1"), args);
                obj.SetOriginalSize();
                InsertPublic(form, obj, "Insert SVG by Drag");
            }
            catch (System.Exception ex)
            {
                string errorMsg = Tools.IsLangKorean() ?
                                 "SVG 파일을 삽입할 수 없습니다: " + ex.Message :
                                 "Unable to insert SVG file: " + ex.Message;
                string title = Tools.IsLangKorean() ? "오류" : "Error";
                System.Windows.Forms.MessageBox.Show(errorMsg, title,
                               System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void EditInsertAnimation(FormEditGraphic form)
		{
			string filename = "";

			FormSelectAnimationFromLibrary dialog = new FormSelectAnimationFromLibrary();

			if(dialog.ShowDialog(form) != DialogResult.OK)	return;

			ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, dialog.sFileNameON, out filename);

			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+50;

			ObjectArgsAnimation args = new ObjectArgsAnimation();

			args.sAnimationFile = filename;
			args.nOverlayMethod = 0;

			ObjectAnimation obj = new ObjectAnimation(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Animation1"), args);
			obj.SetOriginalSize();

            InsertPublic(form, obj, "Insert Animation");
		}

		public static void EditInsertSingleText(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+50;

			ObjectArgsSingleText args = new ObjectArgsSingleText();

			args.text = "SingleText";
			args.textColor = Color.Black;

			ObjectSingleText obj = new ObjectSingleText(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("SingleText1"), GetNewFont(), args);

			obj.RecalcRectSize(form.CreateGraphics());

            InsertPublic(form, obj, "Insert Single Text");
		}


		public static void EditInsertRectangle(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+100;

			
			ObjectRectangle obj = new ObjectRectangle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Rectangle1"), Color.Black, new BrushSolid(Color.White), 1, 1, new ObjectArgsRectangle());



            InsertPublic(form, obj, "Insert Rectangle");
		}

		public static void EditInsertRoundRectangle(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+100;

			ObjectArgsRoundRectangle args = new ObjectArgsRoundRectangle();

			args.round_x = 10;
			args.round_y = 10;

			ObjectRoundRectangle obj = new ObjectRoundRectangle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("RoundRect1"), Color.Black, new BrushSolid(Color.White), 1, 1, args);

			
            InsertPublic(form, obj, "Insert Round Rectangle");
		}

		public static void EditInsertCircle(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+100;

			ObjectCircle obj = new ObjectCircle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Circle1"), Color.Black, new BrushSolid(Color.White), 1, 1, null);

            InsertPublic(form, obj, "Insert Circle");
		}

		public static void EditInsertLine(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+50;
			
			ObjectLine obj = new ObjectLine(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Line1"), Color.Black, new BrushSolid(Color.White), 1, 1);

            InsertPublic(form, obj, "Insert Line");
		}

		public static void EditInsertText(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+200;
			rect.bottom = rect.top+50;

			ObjectArgsText args = new ObjectArgsText();

			args.text = "Text";
			args.textColor = Color.Black;
			
			ObjectText obj = new ObjectText(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Text1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Text");
		}

		public static void EditInsertDate(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+30;

			ObjectArgsDate args = new ObjectArgsDate();

			args.backColor = new BrushSolid(Color.White);
			args.textColor = Color.Black;
			args.type = 0;

			
			ObjectDate obj = new ObjectDate(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Date1"), GetNewFont(), args);



            InsertPublic(form, obj, "Insert Date");
		}

		public static void EditInsertChangeValueDisplay(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+300;
			rect.bottom = rect.top+100;

			ObjectArgsChangeValueDisplay args = new ObjectArgsChangeValueDisplay();

            args.backColor = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.textColor = Color.Black;
			args.nListCount = 3;

			
			ObjectChangeValueDisplay obj = new ObjectChangeValueDisplay(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Display1"), GetNewFont(), args);



            InsertPublic(form, obj, "Insert Change Value Display");
		}

		public static void EditInsertClock(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+100;
			rect.bottom = rect.top+30;

			ObjectArgsClock args = new ObjectArgsClock();

			args.backColor = new BrushSolid(Color.White);
			args.textColor = Color.Black;
			args.type = 0;

			
			ObjectClock obj = new ObjectClock(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Clock1"), GetNewFont(), args);



            InsertPublic(form, obj, "Insert Clock");
		}

		public static void EditInsertDatabaseTrend(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+400;
			rect.bottom = rect.top+300;

			ObjectArgsDatabaseTrend args = new ObjectArgsDatabaseTrend();

			args.lColorFill = new BrushSolid(Color.White);
            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.lColorGuideLine = Color.DarkGray;
			args.lColorText = Color.Black;
            args.pub.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
			args.wLevelDevide = 3;
            args.pub.wPointSize = 10;
			args.wShowUnit = 60;
			args.wTimeDevide = 60;
            args.wTimeSelectOption = 2;  //20250306 PSU 시에서 분으로 수정.
            args.bUseToolBar = true;

			
			ObjectDatabaseTrend obj = new ObjectDatabaseTrend(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("DbTrend1"), GetNewFont(), args, null);



            InsertPublic(form, obj, "Insert Database Trend");
		}

		public static void EditInsertMultiGraph(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+400;
			rect.bottom = rect.top+300;

			ObjectArgsMultiGraph args = new ObjectArgsMultiGraph();

			args.lColorFill = new BrushSolid(Color.White);
            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.lColorGuideLine = Color.DarkGray;
			args.lColorText = Color.Black;
			args.pub.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
			args.wLevelDevide = 3;
			args.pub.wPointSize = 10;
			args.wShowUnit = 60;
			args.wTimeDevide = 60;
			//args.wTimeSelectOption = 3;

			
			ObjectMultiGraph obj = new ObjectMultiGraph(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("MultiGraph1"), GetNewFont(), args, null);



            InsertPublic(form, obj, "Insert Multi Graph");
		}


		public static void EditInsertMultiTrend(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+400;
			rect.bottom = rect.top+300;

			ObjectArgsMultiTrend args = new ObjectArgsMultiTrend();

			args.lColorFill = new BrushSolid(Color.White);
            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.lColorGuideLine = Color.DarkGray;
			args.lColorText = Color.Black;
			args.pub.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
			args.wLevelDevide = 3;
			args.pub.wPointSize = 10;
			args.wShowUnit = 60;
			args.wTimeDevide = 60;
            args.wTimeSelectOption = 0; //20250306 PSU 시에서 분으로 변경.
            args.bUseToolBar = true;

			
			ObjectMultiTrend obj = new ObjectMultiTrend(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("MultiTrend1"), GetNewFont(), args, null);



            InsertPublic(form, obj, "Insert Multi Trend");
		}

        //=== Chart 컨트롤 기반 오브젝트 삽입 (25-02-24) ===
        public static void EditInsertCustomChart(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;
            RECT rect = new RECT();
            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 500;
            rect.bottom = rect.top + 350;

            ObjectArgsChartCustom args = new ObjectArgsChartCustom();
            args.lColorFill = new BrushSolid(Color.White);
            args.lColorBack = new BrushSolid(Color.White);
            args.lColorGuideLine = Color.DarkGray;
            args.lColorText = Color.Black;
            args.pub.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
            args.wLevelDevide = 5;
            args.pub.wPointSize = 10;
            args.wShowUnit = 100;
            args.wTimeDivide = 10;
            args.bSupportPie = true;
            args.bSupportDoughnut = true;
            args.bSupportRadar = true;
            args.chart.bShowLegend = true;
            args.chart.bShowGrid = true;
            args.chart.bAntiAlias = true;
            args.chart.bUseMouseButtonAsZoom = true;

            CustomChart obj = new CustomChart(work.obj.objCommonProperty, form, rect,
                null, new ObjectGeneral("CustomChart1"), GetNewFont(), args, null);
            InsertPublic(form, obj, "Insert Custom Chart");
        }

		public static void EditInsertDatabase(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+300;
			rect.bottom = rect.top+100;

			ObjectArgsDatabase args = new ObjectArgsDatabase();

            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.lColorText = Color.Black;
			args.table = "Table1";
			args.nUpdateTime = 10;
			args.bUseNo = 1;
			args.nConnectionType = 1;
			args.bUseFullCursor = 1;

			
			object obj = new ObjectDatabase(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Database1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Database");
		}

		public static void EditInsertWindowAlarm(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+300;
			rect.bottom = rect.top+100;

			ObjectArgsWindowAlarm args = new ObjectArgsWindowAlarm();

			args.cIncludeMethod = 0;
			args.bFlagWindowCaption = 1;
			args.bFlagUseColumnHeader = 1;

			
			object obj = new ObjectWindowAlarm(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("Alarm1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Alarm Window");
		}

		public static void EditInsertDemandWindow(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+830;
			rect.bottom = rect.top+400;

			ObjectArgsDemandWindow args = new ObjectArgsDemandWindow();

			args.thick_target = 1;
            args.lColorBack.basic_color = Color.White;
            args.lColorFill.basic_color = Color.White;
		
			object obj = new ObjectDemandWindow(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("DemandWindow1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Demand Window");
		}

		public static void EditInsertMilliData(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+600;
			rect.bottom = rect.top+400;

			ObjectArgsMilliData args = new ObjectArgsMilliData();
			args.sTitle = "";

			
			object obj = new ObjectMilliData(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("MilliData1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Milli Data Window");
		}

        public static void EditInsertMilliDataTrend(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 600;
            rect.bottom = rect.top + 400;

            ObjectArgsMilliDataTrend args = new ObjectArgsMilliDataTrend();

            args.lColorFill = new BrushSolid(Color.White);
            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
            args.lColorGuideLine = Color.DarkGray;
            args.lColorText = Color.Black;
            args.pub.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
            args.wLevelDevide = 3;
            args.pub.wPointSize = 10;
            args.wShowUnit = 60;
            args.wTimeDevide = 60;
            args.wTimeSelectOption = 1; //20250306 PSU 시에서 초로 변경.
            args.bUseToolBar = true;

            
            object obj = new ObjectMilliDataTrend(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("MilliDataTrend1"), GetNewFont(), args, null);

            InsertPublic(form, obj, "Insert Milli Data Trend");
        }

		public static void EditInsertRealTimeTestGraph(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+400;
			rect.bottom = rect.top+300;

			ObjectArgsRealTimeTestGraph args = new ObjectArgsRealTimeTestGraph();
			args.wShowUnit = 60;
			args.nDataTime = 1000;
			args.pub.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
			args.lColorFill = new BrushSolid(Color.White);
            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.lColorGuideLine = Color.FromArgb(0x80, 0x80, 0x80);
			args.lColorText = Color.Black;

			
			object obj = new ObjectRealTimeTestGraph(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("TestGraph1"), GetNewFont(), args, null, null);

            InsertPublic(form, obj, "Insert RealTime Graph");
		}

		public static void EditInsertXYGraph(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+300;
			rect.bottom = rect.top+100;

			ObjectArgsXYGraph args = new ObjectArgsXYGraph();
			args.showunit = 60;
			args.nDataTime = 1000;
			args.wDisplayFlags = (EnumDisplayFlag)0xFFFF;
			args.bcolor = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
			args.gcolor = Color.FromArgb(0x80, 0x80, 0x80);
			args.lColorFill = new BrushSolid(Color.White);

			
			object obj = new ObjectXYGraph(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("XYGraph1"), GetNewFont(), args, null);

            InsertPublic(form, obj, "Insert RealTime Graph");
		}

        public static void EditInsertWebBrowser(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 400;
            rect.bottom = rect.top + 300;

            ObjectArgsWebBrowser args = new ObjectArgsWebBrowser();

            args.url = "http://www.autobase.biz";
            
            object obj = new ObjectWebBrowser(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("WebBrowser1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Web Browser");
        }

        //webview 20240627 PSU
        public static void EditInsertWebView(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 400;
            rect.bottom = rect.top + 300;

            ObjectArgsWebView args = new ObjectArgsWebView();

            args.url = "https://www.autobase.biz";

            object obj = new ObjectWebView(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("WebView1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Web View");
        }

        //vlc 20240627 PSU
        public static void EditInsertVLCAx(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 400;
            rect.bottom = rect.top + 300;

            ObjectArgsVLCAx args = new ObjectArgsVLCAx();

            args.mrl = "rtsp://";
            args.options = ":rtsp-tcp";

            object obj = new ObjectVLCAx(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("VLCPlayer1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert VLC Media Player");
        }

        //SVG 20241024 PSU 
        public static void EditInsertSVG(FormEditGraphic form)
        {
            string filename = "";

            if (!PropertyPageObjectSVG.SelectSVGFile(ref filename)) return;

            ClassStudioEditCopyFile.CopyFileToGraphicDirectory(form, filename, out filename);

            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 50;

            ObjectArgsSVG args = new ObjectArgsSVG();

            args.sSvgFile = filename;
            args.nOverlayMethod = 0;

            ObjectSVG obj = new ObjectSVG(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral("SVG1"), args);
            obj.SetOriginalSize();

            InsertPublic(form, obj, "Insert SVG");
        }


        static WebLibraryModCut weblibraryModCut = new WebLibraryModCut(); // 화면을 닫고 다시 띄울 때 이전상태를 유지하기 위해서 외부에 선언

        public static void EditInsertFromLibrary(FormEditGraphic form)
        {
            FormInsertFromLibrary dialog = new FormInsertFromLibrary(weblibraryModCut);

            dialog.Owner = SharedStudio.formMain;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.Show();  //250828 PSU SharedStudio.formMain 제거
        }

        public static void EditInsertFrom_LibraryInsertClick(FormEditGraphic form, ObjectGroup groupCopy)
		{
			ObjectGroup group = (ObjectGroup)Tools.CopyObject(groupCopy);

			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

			int width=0, height=0;
			group.GetGroupRealSize(ref width, ref height);

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+width-1;
			rect.bottom = rect.top+height-1;

            group.UpdateZone(form, rect.left, rect.top, rect.right, rect.bottom);

            InsertPublic(form, group, "Insert From Library");

            work.obj.groupRoot.UpdateParentGroupLayer();    // 하지 않으면 삽입된 그룹의 오브젝트를 선택한 후 다시 삽입하면 다운된다. 2010.1.12

            work.obj.SetZoneAtPercent100();
		}

		public static void EditInsertFrom_LibraryDragAndDrop(FormEditGraphic form, ObjectGroup groupCopy, int mx, int my)
		{
			ObjectGroup group = (ObjectGroup)Tools.CopyObject(groupCopy);

			WORK_MODULE_STRUCT work = form.workThis;


			RECT rect = new RECT();

			int width=0, height=0;
			group.GetGroupRealSize(ref width, ref height);

			rect.left = form.GetPicturePosX(mx)-width/2;
			rect.top  = form.GetPicturePosY(my)-height/2;
			rect.right = rect.left+width-1;
			rect.bottom = rect.top+height-1;


            group.UpdateZone(form, rect.left, rect.top, rect.right, rect.bottom);

            InsertPublic(form, group, "Insert From Library");

            work.obj.groupRoot.UpdateParentGroupLayer();    // 하지 않으면 삽입된 그룹의 오브젝트를 선택한 후 다시 삽입하면 다운된다. 2010.1.12

            work.obj.SetZoneAtPercent100();
		}

		public static void EditImport(FormEditGraphic form)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.Filter = "Vecter Files (*.WMF;*.EMF)|*.wmf;*.emf";

			ObjectGroup group = null;
            double scalex = 1, scaley = 1;

			if(dialog.ShowDialog(form) == DialogResult.OK) 
			{
				string ext = System.IO.Path.GetExtension(dialog.FileName);

				try 
				{
					if(String.Compare(ext, ".WMF", true) == 0) 
					{
						ClassEditImportWMF import = new ClassEditImportWMF();
						group = import.Import(form, dialog.FileName);
					}
					else if(String.Compare(ext, ".EMF", true) == 0) 
					{
						ClassEditImportEMF import = new ClassEditImportEMF();
						group = import.Import(form, dialog.FileName, out scalex, out scaley);
					}
					else
					{
						group = null;
					}
				}
				catch (Exception exception)
				{
					MessageBox.Show("Error=:\n"+exception.Message, dialog.FileName);	
					group = null;
				}
			}

			if(group == null)	return;

			WORK_MODULE_STRUCT work = form.workThis;

			RECT rect = new RECT();

            int width = 0, height = 0;

			group.GetGroupRealSize(ref width, ref height);

            width = (int)(width/scalex);
            height = (int)(height/scaley);

			int csizex, csizey;

			csizex = 1000;
			csizey = 1000;

			if(width > csizex) 
			{
				height = height*csizex/width;
				width = csizex;
			}

			if(height > csizey) 
			{
				width = width*csizey/height;
				height = csizey;
			}

			rect.left = GetNewPosX(form);
			rect.top  = GetNewPosY(form);
			rect.right = rect.left+width-1;
			rect.bottom = rect.top+height-1;

            group.UpdateZone(form, rect.left, rect.top, rect.right, rect.bottom);

            InsertPublic(form, group, "Import");

			work.obj.SetZoneAtPercent100();
		}

        public static void EditInsertFromLibraryFile(FormEditGraphicFrame form)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            WebLibraryModCut weblibrary = new WebLibraryModCut();

            dialog.Filter = String.Format("Library files (*.{0})|*.{0}", weblibrary.sLibExt);

            if (dialog.ShowDialog(form) == DialogResult.OK)
            {
                PreviewItemPublic item = new PreviewItemPublic();
                item.group = weblibrary.PreviewItemNew();

                byte[] buffer = File.ReadAllBytes(dialog.FileName);
                WebLibraryUtil.HashBuffer(buffer);

                weblibrary.PreviewItemLoad(form.formChild, item, buffer, 10, 10, 100, 100);

                object obj = weblibrary.InsertSelection(item, buffer);

                form.InsertLibrary_FromInsertClick((ObjectGroup)obj);
            }
        }

        static string GetUniqueClassName(WORK_MODULE_STRUCT work, string header)
        {
            string class_name;

            ObjectExpand obj;
            int count = 0;

            while (true)
            {
            next_name:
                count++;
                class_name = header + count.ToString();

                for (int i = 0; i < work.obj.groupRoot.GetObjectHap(); i++)
                {
                    class_name = header + count.ToString();
                    obj = (ObjectExpand)work.obj.groupRoot.GetPoint(i);
                    if (obj.objGeneral.sClassName == class_name) goto next_name;
                }
                break;
            }

            return class_name;
        }

        public static void EditInsertDataGridView(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 560;
            rect.bottom = rect.top + 320;

            ObjectArgsDataGridView args = new ObjectArgsDataGridView();

            args.lColorBack = new BrushSolid(Color.FromArgb(0xF0, 0xF0, 0xF0));
            args.lColorText = Color.Black;
            //args.table = "Table1";
            //args.nUpdateTime = 10;
            //args.bUseNo = 1;
            //args.nConnectionType = 1;
            //args.bUseFullCursor = 1;


            object obj = new ObjectDataGridView(work.obj.objCommonProperty, form, rect, null, new ObjectGeneral(GetUniqueClassName(work, "DataGrid")), GetNewFont(), args);

            InsertPublic(form, obj, "Insert DataGridView");
        }

        public static void EditInsertDonutChart(FormEditGraphic form)
        {

            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();

            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 200;

            ObjectArgsDonutChart args = new ObjectArgsDonutChart();

            args.nBarDir = 0;
            args.nLineThick = 10;

            args.fStartAngle = 0;
            args.fSpaceAngle = 0;

            args.nTagNameOption = 0;
            args.nValueOption = 0;
            args.nPercentOption = 0;

            args.nSpaceOutside = 50;
            args.nSpaceInside = 50;

            args.nTagNameSize = 12;
            args.nValueSize = 10;
            args.nPercentSize = 8;

            args.color_total = Color.Black;




            object obj = new ObjectDonutChart(work.obj.objCommonProperty, rect, null, new ObjectGeneral("DonutChart1"), GetNewFont(), args, null);

            //obj.expandScript.structMouseZone = new EXPAND_MOUSE_ZONE();
            //obj.ExpandActive = true;



            InsertPublic(form, obj, "Insert Donut Chart");
        }

        // DemandChart 객체 생성
        public static void EditInsertDemandChart(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();
            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 830;
            rect.bottom = rect.top + 400;

            ObjectArgsDemandChart args = new ObjectArgsDemandChart();

            object obj = new ObjectDemandChart(work.obj.objCommonProperty, form, rect, null,
                new ObjectGeneral("DemandChart1"), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Demand Chart");
        }

        public static void EditInsertBarcodeDisplay(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();
            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 200;
            rect.bottom = rect.top + 200;

            ObjectArgsBarcodeDisplay args = new ObjectArgsBarcodeDisplay();

            object obj = new ObjectBarcodeDisplay(work.obj.objCommonProperty, form, rect, null,
                new ObjectGeneral("BarcodeDisplay1"), args);

            InsertPublic(form, obj, "Insert Barcode Display");
        }

        public static void EditInsertBarcodeScanner(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();
            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 250;
            rect.bottom = rect.top + 80;

            ObjectArgsBarcodeScanner args = new ObjectArgsBarcodeScanner();

            object obj = new ObjectBarcodeScanner(work.obj.objCommonProperty, form, rect, null,
                new ObjectGeneral("BarcodeScanner1"), args);

            InsertPublic(form, obj, "Insert Barcode Scanner");
        }

        //26-03-17 테이블 오브젝트 삽입
        public static void EditInsertTable(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            RECT rect = new RECT();
            rect.left = GetNewPosX(form);
            rect.top = GetNewPosY(form);
            rect.right = rect.left + 400;
            rect.bottom = rect.top + 200;

            ObjectArgsTable args = new ObjectArgsTable();
            args.InitializeCells();

            object obj = new ObjectTable(work.obj.objCommonProperty, form, rect, null,
                new ObjectGeneral(GetUniqueClassName(work, "Table")), GetNewFont(), args);

            InsertPublic(form, obj, "Insert Table");
        }

    }
}

