using System;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using GraphicModule;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditObjectLine.
	/// </summary>
	public class ClassEditObjectLine : ClassMainTool
	{
		public ClassEditObjectLine()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		bool bMouseCaptureFlag = false;

		Point pMouseStart = new Point();
		Point pMouseOld = new Point();

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

			/*
			int x = e.X;
			int y = e.Y;
			
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				ClassEditObjectCurve.CalcLineOnPressedCtrl(form, pMouseStart.X, pMouseStart.Y, ref x, ref y);
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
				ClassEditObjectCurve.CalcLineOnPressedCtrl(form, pMouseStart.X, pMouseStart.Y, ref x, ref y);
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

			if(pMouseStart.X == pMouseOld.X && pMouseStart.Y == pMouseOld.Y)	return;	// Line start & end position is same

			WORK_MODULE_STRUCT work = form.workThis;
			RECT rect = new RECT();

			
			rect.left =   form.GetPicturePosX(pMouseStart.X);
			rect.top  =   form.GetPicturePosY(pMouseStart.Y);
			rect.right =  form.GetPicturePosX(pMouseOld.X);
			rect.bottom = form.GetPicturePosY(pMouseOld.Y);

            object obj = new ObjectLine(work.obj.objCommonProperty, rect, null, new ObjectGeneral("Line1"), Color.Black, new BrushSolid(Color.White), 1, 1);

            ClassEditInsert.InsertPublic(form, obj, "Insert Line");
		}

		static void XorLine(Graphics g, int x1, int y1, int x2, int y2)
		{
			Pen pen = new Pen(Color.Black);
			g.DrawLine(pen, x1, y1, x2, y2);
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
			if(bMouseCaptureFlag)
				XorLine(g, pMouseStart.X, pMouseStart.Y, pMouseOld.X, pMouseOld.Y);
		}
	}
}
