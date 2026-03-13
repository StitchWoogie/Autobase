using System;
using System.Drawing;
using NetTools.OldDefine;
using NetTools;
using AutoLib;


namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectUnknown.
	/// </summary>
	[Serializable]
	public class ObjectUnknown : ObjectExpand
	{
		string sCommand;
        
		public ObjectUnknown(ObjectCommonProperty ocp, string command, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general)
			: base(ocp, rect, eid, null, general)
		{
			sCommand = command;
		}

        bool bNotSupported = false;
        //AutoLibLocal.EnumOemTypeRight saveOemr;

        public void SetNotSupported()
        {
            bNotSupported = true;
            //saveOemr = oemr;
        }

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

            if(bNotSupported)
                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.Blue);
            else
                DrawClass.PopBox2(g, x1, y1, x2, y2, Color.Red);

			Font font = this.MakeFont();
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

            string text;

            if (bNotSupported)
                text = sCommand + " Not supported";
            else
                text = sCommand;

            DrawClass.WinDrawText(g, x1, y1, x2 - x1 + 1, y2 - y1 + 1, text, Color.White, Color.Red, font, format);
		}

		public override void ObjectSave(CommaTextWriter writer)
		{

		} 
	}
}

/*

*/








