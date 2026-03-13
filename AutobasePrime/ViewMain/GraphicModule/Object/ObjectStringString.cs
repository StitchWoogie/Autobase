using System;
using NetTools;
using NetTools.OldDefine;
using System.Drawing;
using AutoLib;
using AutoLibLocal;
using System.IO;
using System.Collections;
using System.Threading.Tasks;

namespace GraphicModule
{
	[Serializable]
	public class TEXT_ALIGN
	{
		public byte x;	// 0 - left, 1 - center, 2 - right
		public byte y;	// 0 - top,  1 - vcenter, 2 - bottom
	}

	[Serializable]
	public class ObjectArgsStringString
	{
		public int nBoxUse;
		public Color colorText;
        public BrushPublic colorBack = new BrushPublic();
	}

	/// <summary>
	/// Summary description for ObjectAnalogString.
	/// </summary>
	[Serializable]
	public class ObjectStringString : ObjectTag
	{
		ObjectArgsStringString objArgs;
		TEXT_ALIGN textAlign;

		public ObjectArgsStringString ObjectArgs 
		{
			get 
			{
				return objArgs;
			}
			set 
			{
				objArgs = value;
			}
		}

		public TEXT_ALIGN TextAlign 
		{
			get 
			{
				return textAlign;
			}
			set 
			{
				textAlign = value;
			}
		}

		public ObjectStringString(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, string tag, MOUSE_RESPONSE_STRUCT mouse_response, RECT rMouse, ObjectArgsStringString args, TEXT_ALIGN align)
			: base(ocp, rect, eid, general, lf, tag, mouse_response, rMouse)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.StringString;
			objArgs = args;
			textAlign = align;
			SetTextColor(args.colorText);
			SetBackColor(args.colorBack);
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			string buf;

			TagStClass st = TagLib.GetStructST(sTagName, ref nTagPos);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
				buf = sTagName;
			else
				buf = st.curr;

            Brush brushback = ObjectRectangle.MakePublicBrush(RunColorBack, x1, y1, x2, y2);

			if(objArgs.nBoxUse == 0)
				DrawClass.gcls(g, x1, y1, x2, y2, brushback);
			else if(objArgs.nBoxUse == 1)
				DrawClass.PopBox2(g, x1, y1, x2, y2, brushback);
			else if(objArgs.nBoxUse == 2)
				DrawClass.PushBox2(g, x1, y1, x2, y2, brushback);
			else {}

			Font font = MakeFont();
			StringFormat format = new StringFormat();

			if(textAlign.x == 0)
				format.Alignment = StringAlignment.Near;
			else if(textAlign.x == 1)
				format.Alignment = StringAlignment.Center;
			else
				format.Alignment = StringAlignment.Far;

			if(textAlign.y == 0)
				format.LineAlignment = StringAlignment.Near;
			else if(textAlign.y == 1)
				format.LineAlignment = StringAlignment.Center;
			else
				format.LineAlignment = StringAlignment.Far;

			format.FormatFlags |= StringFormatFlags.NoWrap;

			DrawClass.WinDrawText(g, x1, y1, x2-x1+1, y2-y1+1, buf, RunColorText, RunColorBack.basic_color, font, format);

			DisplayInactive(g, x1, y1, x2, y2);
		}

		public override async Task EventTimerObject(System.Windows.Forms.Form form)
		{
			TagStClass st = TagLib.GetStructST(sTagName, ref nTagPos);

			st.NeedDataCurr = true;

			await Task.CompletedTask;
		}

        public override void EventTag(System.Windows.Forms.Form form, TagPublicClass tagevent)
		{
			if(tagevent.enumTagType != EnumTagType.ST)	return;
			if(String.Compare(tagevent.tag, sTagName) != 0)		return;

			InvalidateObject(form);
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveTag(writer);
			SaveObjectItem.TextColor(writer, GetTextColor());
			SaveObjectItem.BackColor(writer, GetBackColor());
			SaveObjectItem.BackBox(writer, objArgs.nBoxUse);
			SaveObjectItem.TextAlign(writer, textAlign);
			ObjectSaveFont(writer);
		} 


	}
}



