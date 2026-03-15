using System;
using System.Drawing;
using AutoLib;
using NetTools.OldDefine;
using NetTools;
using System.Collections;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectPoly.
	/// </summary>
	[Serializable]
	public class ObjectPoly : ObjectExpand
	{
		ArrayList   blockPoint;
		RECT rPolySize = new RECT();

        public ObjectPoly(ObjectCommonProperty ocp, System.Windows.Forms.Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, Color lcolor, BrushPublic fcolor, int loption, int lthick, ArrayList block)
			: base(ocp, rect, eid, null, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Poly;
			SetLineColor(lcolor);
			SetFillColor(fcolor);		
			nLineOption = loption;
			SetBorderThick(lthick);

			SetPointBlock(form, block);
		}

		public void SetPointBlock(System.Windows.Forms.Form form, ArrayList block)
		{
			blockPoint = (ArrayList)Tools.CopyObject(block);

			CalcPolyRect(ref rPolySize);
			UpdateZone(form, rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
		}

		void CalcPolyRect(ref RECT rect)
		{
			Point poly;
			int i;

			poly = (Point)blockPoint[0];
	
			rect.left = poly.X;
			rect.top  = poly.Y;
			rect.right = poly.X;
			rect.bottom = poly.Y;

			for(i = 0; i < blockPoint.Count; i++) 
			{
				poly = (Point)blockPoint[i];
				if(poly.X < rect.left)		rect.left = poly.X;
				if(poly.Y < rect.top)		rect.top  = poly.Y;
				if(poly.X > rect.right)		rect.right= poly.X;
				if(poly.Y > rect.bottom)	rect.bottom= poly.Y;
			}
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(nLineOption == 0 && nFillOption == 0) 
			{
				nLineOption = 1;
				nFillOption = 1;
			}

			Point[] polygon = new Point[blockPoint.Count];

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			int org_sizex = rPolySize.right -rPolySize.left;
			int org_sizey = rPolySize.bottom-rPolySize.top;
			int tar_sizex = x2-x1;
			int tar_sizey = y2-y1;
			int l;
			Point p;

			for(l = 0; l < blockPoint.Count; l++) 
			{
				p = (Point)blockPoint[l];
				polygon[l].X = p.X;
				polygon[l].Y = p.Y;

				if(org_sizex == 0)
					polygon[l].X = x1;
				else
					polygon[l].X = x1+((polygon[l].X-rPolySize.left)*(tar_sizex)/(org_sizex));

				if(org_sizey == 0) 
					polygon[l].Y = y1;
				else
					polygon[l].Y = y1+((polygon[l].Y-rPolySize.top )*(tar_sizey)/(org_sizey));
			}

			if(nFillOption > 0)
			{
				using (Brush brush = ObjectRectangle.MakePublicBrush(RunColorFill, x1, y1, x2, y2))
					g.FillPolygon(brush, polygon);
			}

			if(nLineOption > 0)
			{
				using (Pen pen = new Pen(RunColorLine, bthick))
				{
					pen.DashStyle = ObjectRectangle.GetDashStyle(nLineOption);
					g.DrawPolygon(pen, polygon);
				}
			}
		}

		public void GetPolyBlock(ArrayList block)
		{
			Point poly = new Point();
			Point copy = new Point();

			block.Clear();

			for(int l = 0; l < blockPoint.Count; l++) 
			{
				poly = (Point)blockPoint[l];
				copy = new Point();
				copy.X = poly.X;
				copy.Y = poly.Y;
				block.Add(poly);
			}		
		}

		public override void UpdateZone(System.Windows.Forms.Form form, int x1, int y1, int x2, int y2)
		{
			int org_sizex = rPolySize.right -rPolySize.left;
			int org_sizey = rPolySize.bottom-rPolySize.top;
			int tar_sizex = x2-x1;
			int tar_sizey = y2-y1;
			int l;
			Point p;
			int x, y;

			for(l = 0; l < blockPoint.Count; l++) 
			{
				p = (Point)blockPoint[l];

				if(org_sizex == 0)
					x = x1;
				else
					x = x1+((p.X-rPolySize.left)*(tar_sizex)/(org_sizex));

				if(org_sizey == 0) 
					y = y1;
				else
					y = y1+((p.Y-rPolySize.top )*(tar_sizey)/(org_sizey));

				// p.X, p.Y 에 바로 대입하면 struct라서 그런지 대입해도 이전값을 유지한다.
				blockPoint[l] = new Point(x, y);
			}

			CalcPolyRect(ref rPolySize);
            base.UpdateZone(form, rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			SaveObjectItem.LineColor(writer, GetLineColor());
			SaveObjectItem.FillColor(writer, GetFillColor());
			SaveObjectItem.LineThick(writer, GetBorderThick());
			SaveObjectItem.LineOption(writer, nLineOption);
			SaveObjectItem.FillOption(writer, nFillOption);

			Point poly;

			for(int l = 0; l < blockPoint.Count; l++) 
			{
				poly = (Point)blockPoint[l];
				writer.WriteLine("\tPoint,{0},{1},", poly.X, poly.Y);
			}		
		}

        public override void EditRotateRight(int nx1, int ny1, int nx2, int ny2)
        {
            int x, y;
            int bx1, bx2, by1, by2;

            bx1 = rPolySize.left;
            bx2 = rPolySize.right;
            by1 = rPolySize.top;
            by2 = rPolySize.bottom;

            if (bx1 > bx2) Tools.Temp(ref bx1, ref bx2);
            if (by1 > by2) Tools.Temp(ref by1, ref by2);

            if (nx1 > nx2) Tools.Temp(ref nx1, ref nx2);
            if (ny1 > ny2) Tools.Temp(ref ny1, ref ny2);

            int gabx, gaby;
            int width = bx2 - bx1;
            int height = by2 - by1;

            int i;
            Point p;

            for (i = 0; i < blockPoint.Count; i++)
            {
                p = (Point)blockPoint[i];

                x = p.X;
                y = p.Y;

                gabx = x - bx1;
                gaby = y - by1;

                x = nx1 + height - gaby;
                y = ny1 + gabx;

                blockPoint[i] = new Point(x, y);
            }

            CalcPolyRect(ref rPolySize);
            
            base.UpdateZone(objCommonProperty.form, rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
        }

        public override void EditRotateLeft(int nx1, int ny1, int nx2, int ny2)
        {
            int x, y;
            int bx1, bx2, by1, by2;

            bx1 = rPolySize.left;
            bx2 = rPolySize.right;
            by1 = rPolySize.top;
            by2 = rPolySize.bottom;

            if (bx1 > bx2) Tools.Temp(ref bx1, ref bx2);
            if (by1 > by2) Tools.Temp(ref by1, ref by2);

            if (nx1 > nx2) Tools.Temp(ref nx1, ref nx2);
            if (ny1 > ny2) Tools.Temp(ref ny1, ref ny2);

            int gabx, gaby;
            int width = bx2 - bx1;
            int height = by2 - by1;

            int i;
            Point p;

            for (i = 0; i < blockPoint.Count; i++)
            {
                p = (Point)blockPoint[i];

                x = p.X;
                y = p.Y;

                gabx = x - bx1;
                gaby = y - by1;

                x = nx1 + gaby;
                y = ny1 + width-gabx;

                blockPoint[i] = new Point(x, y);
            }

            CalcPolyRect(ref rPolySize);

            base.UpdateZone(objCommonProperty.form, rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
        }
	}
}

/*
 * 
void ObjectPoly :: UpdateZone(int x1, int y1, int x2, int y2)
{
	int org_sizex = rPolySize.right -rPolySize.left;
	int org_sizey = rPolySize.bottom-rPolySize.top;
	int tar_sizex = x2-x1;
	int tar_sizey = y2-y1;
	DWORD l;
	POINT p;

	for(l = 0; l < blockPoint->GetCount(); l++) {
		blockPoint->GetBlock(&p, l);

		if(org_sizex == 0)
			p.x = x1;
		else
			p.x = x1+((p.x-rPolySize.left)*(tar_sizex)/(org_sizex));

		if(org_sizey == 0) 
			p.y = y1;
		else
			p.y = y1+((p.y-rPolySize.top )*(tar_sizey)/(org_sizey));

		blockPoint->SetBlock(&p, l);
	}

	CalcPolyRect(&rPolySize);
	ObjectExpand :: UpdateZone(rPolySize.left, rPolySize.top, rPolySize.right, rPolySize.bottom);
}


*/
