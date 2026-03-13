using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using NetTools.OldDefine;
using GraphicModule;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditObjectPoly.
	/// </summary>
	public class ClassEditObjectPoly : ClassMainTool
	{
		public ClassEditObjectPoly() 
		{
			//
			// TODO: Add constructor logic here
			//
		}

		ArrayList blockPoly = new ArrayList();
		bool bStartPolyFlag = false;

		Point pMouseStart = new Point();
		Point pMouseOld = new Point();

		public bool bFillFlag = false;

		public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{
            if (e.Clicks == 2) return;      // 더블 클릭 시는 더 이상 선을 추가하지 않는다.

			Point poly;

            if (bStartPolyFlag == false)
            {
				blockPoly = new ArrayList();	// 리스트를 새로 만드는것이 좋다.

                bStartPolyFlag = true;
				form.Capture = true;
				form.ConvertMousePointByGuideLine(form.workThis, ref pMouseStart, e.X, e.Y);
				pMouseOld = pMouseStart;

				poly = new Point();
				poly.X = pMouseStart.X;
				poly.Y = pMouseStart.Y;

				blockPoly.Add(poly);

				form.Invalidate();
			}
			else {
				if(pMouseStart == pMouseOld) {
				
				}
				else {
                        poly = new Point();
                        poly.X = pMouseOld.X;
                        poly.Y = pMouseOld.Y;

                        pMouseStart = pMouseOld;

                        blockPoly.Add(poly);

                        form.Invalidate();

                        bIgnoreMove = true;

                        // 마우스를 누르고 나면 MouseMove도 함께 발생되어서 Ctrl키를 누른상태에서 더블클릭을 하게되면 원하지 않는 점이 찍히게 된다.
                        // 마우스 누르고 난 후 첫번째 Move는 무시한다.
				}
			}		
		}

		static bool bIgnoreMove = false;

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			form.Cursor = Cursors.Cross;

            if (bStartPolyFlag == false) return;

			if(bIgnoreMove)		
			{
				bIgnoreMove = false;
				return;
			}

			// 확대 되었을 때는 위치가 정확하지 않으므로 실제로 위치할 점을 찾는다.
			form.ConvertMousePointByGuideLine(form.workThis, ref pMouseOld, e.X, e.Y);
			
			// Shift 키가 눌러졌을때는 큰것을 기준으로 크기를 맞춘다.
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
			
		}

		void DrawPoly(Graphics g)
		{
			if(blockPoly.Count == 0)	return;
			
			Point poly;
			bool move_flag = false;
			int  move_x=0, move_y=0;

			Pen pen = new Pen(Color.Black);

			for(int l = 0; l < blockPoly.Count; l++) 
			{
				poly = (Point)blockPoly[l];
				if(!move_flag) 
				{
					move_flag = true;
					move_x = poly.X;
					move_y = poly.Y;
				}
				else 
				{
					g.DrawLine(pen, move_x, move_y, poly.X, poly.Y);
					move_x = poly.X;
					move_y = poly.Y;
				}
			}

			g.DrawLine(pen, move_x, move_y, pMouseOld.X, pMouseOld.Y);
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
            if (bStartPolyFlag)
				DrawPoly(g);
		}

		public override void DoubleClick(FormEditGraphic form)
		{
			Point poly;

            if (bStartPolyFlag == false) return;

			form.Capture = false;
            bStartPolyFlag = false;

			if(blockPoly.Count > 2) 
			{
				WORK_MODULE_STRUCT work = form.workThis;
				int x, y;

				for(int l = 0; l < blockPoly.Count; l++) 
				{
					poly = (Point)blockPoly[l];
					x = form.GetPicturePosX(poly.X);
					y = form.GetPicturePosY(poly.Y);
					blockPoly[l] = new Point(x, y);	// Point는 스트럭쳐이므로 다시 할당해야 값을 바꿀 수 있다.
				}

                BrushSolid bs = new BrushSolid(Color.White);
                if (bFillFlag) bs.brush_type = 1;
                else bs.brush_type = 0;

                object obj = new ObjectPoly(work.obj.objCommonProperty, form, null, null, new ObjectGeneral("Poly1"), Color.Black, bs, 1, 1, blockPoly);

                ClassEditInsert.InsertPublic(form, obj, "Insert Poly");
			}
			form.Invalidate();
		}
	}
}

