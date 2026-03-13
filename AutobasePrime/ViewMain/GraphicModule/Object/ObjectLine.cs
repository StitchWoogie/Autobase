using System;
using System.Drawing;
using AutoLib;
using NetTools.OldDefine;
using NetTools;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectLine.
	/// </summary>
	[Serializable]
	public class ObjectLine : ObjectExpand
	{
		//int 		wLineOption;
		//int 		wFillOption;

		//public int GetLineOption() { return wLineOption; }
		//public int GetFillOption() { return wFillOption; }

        public ObjectLine(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick)
			: base(ocp, rect, eid, null, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Line;
			SetLineColor(lcolor);
			SetFillColor(fcolor);		
			nLineOption = loption;
			SetBorderThick(lthick);
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			DrawClass draw = new DrawClass();
	
			Pen pen = new Pen(RunColorLine, bthick);

			pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);

			g.DrawLine(pen, x1, y1, x2, y2);
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.LineColor(writer, GetLineColor());
			//SaveObjectItem.FillColor(writer, GetBackColor());
			SaveObjectItem.LineThick(writer, GetBorderThick());
			SaveObjectItem.LineOption(writer, nLineOption);
			//SaveObjectItem.FillOption(writer, nFillOption);
		}

        protected override void MakeLayerPreviewObject(Graphics g, int width, int height, int thick)
        {
            int vx1 = 0, vy1 = 0, vx2 = 0, vy2 = 0;

            GetViewZone(ref vx1, ref vy1, ref vx2, ref vy2);

            int vw = Math.Abs(vx2 - vx1) + 1;
            int vh = Math.Abs(vy2 - vy1) + 1;

            int ry = width * vh / vw;
            int rx = height * vw / vh;
            int x1, y1, x2, y2;

            if (vw == 1) rx = 1;    // size가 0 이라면 계산할 결과도 0이다
            if (vh == 1) ry = 1;    // size가 0 이라면 계산할 결과도 0이다

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

            if (vx1 > vx2) Tools.Temp(ref x1, ref x2);
            if (vy1 > vy2) Tools.Temp(ref y1, ref y2);

            DisplayObject(g, x1, y1, x2, y2, thick);
        }
	}
}

