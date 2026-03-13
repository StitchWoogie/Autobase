using System;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using GraphicModule;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditObjectRect.
	/// </summary>
	public class ClassEditObjectRect : ClassMainTool
	{
		public ClassEditObjectRect()
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
			//Graphics g = form.CreateGraphics();
			//XorLine(g, pMouseStart.X, pMouseStart.Y, pMouseOld.X, pMouseOld.Y);

			form.Invalidate();			

		}

		// Ctrl키가 눌러졌을때는 큰것을 기준으로 크기를 맞춘다.
		public static void CalcRectOnPressedCtrl(int startx, int starty, ref int x, ref int y)
		{
			int gabx = Math.Abs(startx-x);
			int gaby = Math.Abs(starty-y);

			if(gabx > gaby) 
			{
				y = starty+ ((y-starty) > 0 ? gabx : -gabx);
			}
			else 
			{
				x = startx+ ((x-startx) > 0 ? gaby : -gaby);
			}
		}

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			form.Cursor = Cursors.Cross;
	
			if(bMouseCaptureFlag == false)	return;

			Graphics g = form.CreateGraphics();
			
			/*
			int x = e.X;
			int y = e.Y;
			
			// Ctrl키가 눌러졌을때는 큰것을 기준으로 크기를 맞춘다.
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				ClassEditObjectRect.CalcRectOnPressedCtrl(pMouseStart.X, pMouseStart.Y, ref x, ref y);
			}
			
			
			form.ConvertMousePointByGuideLine(form.workThis, ref pMouseOld, x, y);
			*/

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
            if (bFillFlag) bs.brush_type = 1;
            else bs.brush_type = 0;

            object obj = new ObjectRectangle(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Rect1"), Color.Black, bs, 1, 1, new ObjectArgsRectangle());

            ClassEditInsert.InsertPublic(form, obj, "Insert Rectangle");
		}

		static void XorRect(Graphics g, int x1, int y1, int x2, int y2)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			Pen pen = new Pen(Color.Black);
			g.DrawRectangle(pen, x1, y1, x2-x1, y2-y1);
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
			if(bMouseCaptureFlag)
				XorRect(g, pMouseStart.X, pMouseStart.Y, pMouseOld.X, pMouseOld.Y);
		}
	}
}
