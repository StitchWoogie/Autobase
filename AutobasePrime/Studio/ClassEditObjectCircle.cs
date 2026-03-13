using System;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using GraphicModule;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditObjectCircle.
	/// </summary>
	public class ClassEditObjectCircle : ClassMainTool
	{
		public ClassEditObjectCircle()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static bool bMouseCaptureFlag = false;

		static Point pMouseStart = new Point();
		static Point pMouseOld = new Point();

		public bool bFillFlag = false;

		public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{
			if(bMouseCaptureFlag == true)		return;

			bMouseCaptureFlag = true;
			form.Capture = true;
			form.ConvertMousePointByGuideLine(form.workThis, ref pMouseStart, e.X, e.Y);
			pMouseOld = pMouseStart;

			form.Invalidate();
		}

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			form.Cursor = Cursors.Cross;
	
			if(bMouseCaptureFlag == false)	return;

			Graphics g = form.CreateGraphics();

			// 확대 되었을 때는 위치가 정확하지 않으므로 실제로 위치할 점을 찾는다.
			form.ConvertMousePointByGuideLine(form.workThis, ref pMouseOld, e.X, e.Y);
			
			// Ctrl키가 눌러졌을때는 큰것을 기준으로 크기를 맞춘다.
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				int x = pMouseOld.X;
				int y = pMouseOld.Y;
				ClassEditObjectRect.CalcRectOnPressedCtrl(pMouseStart.X, pMouseStart.Y, ref x, ref y);
				pMouseOld.X = x;
				pMouseOld.Y = y;
			}

			form.DrawMakingObjectSize(pMouseStart.X, pMouseStart.Y, pMouseOld.X, pMouseOld.Y);
			form.Invalidate();
			
			
		}

		public override void MouseUpLeft(FormEditGraphic form, MouseEventArgs e)
		{
			if(bMouseCaptureFlag == false)	return;
	
			bMouseCaptureFlag = false;
			form.Capture = false;

			//Graphics g = form.CreateGraphics();
			//XorLine(g, pMouseStart.X, pMouseStart.Y, pMouseOld.X, pMouseOld.Y);

			if(pMouseStart.X == pMouseOld.X && pMouseStart.Y == pMouseOld.Y)	return;	// Line start & end position is same

			WORK_MODULE_STRUCT work = form.workThis;
			RECT rect = new RECT();

			rect.left =   form.GetPicturePosX(pMouseStart.X);
			rect.top  =   form.GetPicturePosY(pMouseStart.Y);
			rect.right =  form.GetPicturePosX(pMouseOld.X);
			rect.bottom = form.GetPicturePosY(pMouseOld.Y);

            BrushSolid bs = new BrushSolid(Color.White);
            if (bFillFlag)  bs.brush_type = 1;
            else            bs.brush_type = 0;

			object obj = new ObjectCircle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Circle1"), Color.Black, bs, 1, 1, null);

            ClassEditInsert.InsertPublic(form, obj, "Insert Circle");
		}

		static void XorCircle(Graphics g, int x1, int y1, int x2, int y2)
		{
			/*
			hPen = CreatePen(PS_SOLID, 1, WHITE_COLOR);
			int save = SetROP2(hdc, R2_XORPEN);
			hPenOld = (HPEN)SelectObject(hdc, hPen);
			MoveToEx(hdc, x1, y1, NULL);
			LineTo(hdc, x2, y2);
			SelectObject(hdc, hPenOld);
			SetROP2(hdc, save);
			DeleteObject(hPen);
			*/
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			Pen pen = new Pen(Color.Black);
			g.DrawEllipse(pen, x1, y1, x2-x1, y2-y1);
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
			if(bMouseCaptureFlag)
				XorCircle(g, pMouseStart.X, pMouseStart.Y, pMouseOld.X, pMouseOld.Y);
		}
	}
}
