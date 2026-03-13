using System;
using System.Windows.Forms;
using System.Drawing;
using GraphicModule;
using System.Collections;
using NetTools.OldDefine;

namespace Studio
{
	

	/// <summary>
	/// Summary description for ClassEditObjectCurve.
	/// </summary>
	public class ClassEditObjectCurve : ClassMainTool
	{
		public ClassEditObjectCurve()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		bool bMouseCaptureFlag = false;
		ArrayList blockCurve = new ArrayList();
		Point pStart = new Point();
		Point pOld = new Point();

		public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{
			CURVE_STRUCT curve = new CURVE_STRUCT();
			Point p = new Point();
			Graphics g = form.CreateGraphics();

			if(bMouseCaptureFlag == false) 
			{
				form.ConvertMousePointByGuideLine(form.workThis, ref p, e.X, e.Y);
				
				blockCurve = new ArrayList();
		
				bMouseCaptureFlag = true;
				form.Capture = true;

				pStart = pOld = p;

				curve.x[0] = pStart.X;
				curve.y[0] = pStart.Y;
				curve.type = EnumCurveType.START;
				
				blockCurve.Add(curve);
			}
			else 
			{
				if(Math.Abs(pOld.X-pStart.X) <= 1 && Math.Abs(pOld.Y-pStart.Y) <= 1) 
				{
				}
				else 
				{
					pStart = pOld;

					curve.x[0] = pStart.X;
					curve.y[0] = pStart.Y;
					curve.type = EnumCurveType.LINE;

					blockCurve.Add(curve);

					bIgnoreMove = true;		// 마우스를 누르고 나면 MouseMove도 함께 발생되어서 Ctrl키를 누른상태에서 더블클릭을 하게되면 원하지 않는 점이 찍히게 된다.
											// 마우스 누르고 난 후 첫번째 Move는 무시한다.
				}
			}
		}

		static bool bIgnoreMove = false;

		// Ctrl키가 눌러 졌을때는 큰 것을 기준으로 크기를 맞춘다. 15도씩 할당한다.
		public static void CalcLineOnPressedCtrl(Form form, int startx, int starty, ref int x, ref int y)
		{
			int gabx = x-startx;
			int gaby = y-starty;

			double radian;
			double degree;

			if(gabx == 0) 
			{
				radian = Math.PI*90/180;
				degree = 90;
			}
			else 
			{
				radian = Math.Atan((double)Math.Abs(gaby)/(double)Math.Abs(gabx));
				degree = radian*180/Math.PI;
			}

			if(gabx >= 0 && gaby >= 0) // 270~360
			{
				degree = 360-degree;
			}
			else if(gabx >= 0 && gaby <= 0)		// 0~90
			{
				//degree = degree;
			}
			else if(gabx <= 0 && gaby <= 0)		// 90~180
			{
				degree = 180-degree;
			}
			else		// 180~270
			{
				degree = 180+degree;
			}

			// 15도 단위에 가까운 위치를 찾아준다.
			int new_degree = (((int)(degree+7.5))/15)*15;
			double sin = Math.Sin(radian);
			double length;
			
			if(sin == 0)	length = Math.Abs(gabx);
			else			length = (double)Math.Abs(gaby)/sin;

			double newx, newy;
			double new_radian;
			
			if(new_degree >= 0 && new_degree <= 90) 
			{
				new_radian = new_degree*Math.PI/180;
				newx = Math.Cos(new_radian)*length;
				newy = -Math.Sin(new_radian)*length;
			}
			else if(new_degree >= 90 && new_degree <= 180) 
			{
				new_radian = (180-new_degree)*Math.PI/180;
				newx = -Math.Cos(new_radian)*length;
				newy = -Math.Sin(new_radian)*length;
			}
			else if(new_degree >= 180 && new_degree <= 270) 
			{
				new_radian = (new_degree-180)*Math.PI/180;
				newx = -Math.Cos(new_radian)*length;
				newy = Math.Sin(new_radian)*length;
			}
			else 
			{
				new_radian = (360-new_degree)*Math.PI/180;
				newx = Math.Cos(new_radian)*length;
				newy = Math.Sin(new_radian)*length;
			}

			x = (int)(startx + (int)newx);	// (int)newx를 하지 않으면 45도일때 약간 기울어 진다.
			y = (int)(starty + (int)newy);	// (int)newx를 하지 않으면 45도일때 약간 기울어 진다.
		}

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			form.Cursor = Cursors.Cross;
	
			if(bMouseCaptureFlag == false)	return;

			if(bIgnoreMove)
			{
				bIgnoreMove = false;
				return;
			}

			/*
			int x = e.X;
			int y = e.Y;

			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				CalcLineOnPressedCtrl(form, pStart.X, pStart.Y, ref x, ref y);
			}

			form.ConvertMousePointByGuideLine(form.workThis, ref pOld, x, y);
			*/

			// 확대 되었을 때는 위치가 정확하지 않으므로 실제로 위치할 점을 찾는다.
			form.ConvertMousePointByGuideLine(form.workThis, ref pOld, e.X, e.Y);
			
			// Ctrl키가 눌러졌을때는 큰것을 기준으로 크기를 맞춘다.
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				int x = pOld.X;
				int y = pOld.Y;
				ClassEditObjectCurve.CalcLineOnPressedCtrl(form, pStart.X, pStart.Y, ref x, ref y);
				pOld.X = x;
				pOld.Y = y;
			}

			form.DrawMakingObjectSize(pStart.X, pStart.Y, pOld.X, pOld.Y);
			form.Invalidate();
		}

		public override void MouseUpLeft(FormEditGraphic form, MouseEventArgs e)
		{

		}

		void DrawXorCurve(Graphics g)
		{
			if(blockCurve.Count == 0)	return;
			
			CURVE_STRUCT curve;
			bool move_flag = false;
			int  move_x=0, move_y=0;

			Pen pen = new Pen(Color.Black);

			for(int l = 0; l < blockCurve.Count; l++) 
			{
				curve = (CURVE_STRUCT)blockCurve[l];
				if(!move_flag) 
				{
					move_flag = true;
					move_x = curve.x[0];
					move_y = curve.y[0];
				}
				else 
				{
					
					g.DrawLine(pen, move_x, move_y, curve.x[0], curve.y[0]);
					move_x = curve.x[0];
					move_y = curve.y[0];
				}
			}

			g.DrawLine(pen, move_x, move_y, pOld.X, pOld.Y);
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
			if(bMouseCaptureFlag)
				DrawXorCurve(g);
		}

		public override void DoubleClick(FormEditGraphic form)
		{
			CURVE_STRUCT curve;

			if(bMouseCaptureFlag == false)	return;

			form.Capture = false;
			bMouseCaptureFlag = false;

			if(blockCurve.Count > 2) 
			{
				WORK_MODULE_STRUCT work = form.workThis;


				for(int l = 0; l < blockCurve.Count; l++) 
				{
					curve = (CURVE_STRUCT)blockCurve[l];
					curve.x[0] = form.GetPicturePosX(curve.x[0]);
					curve.y[0] = form.GetPicturePosY(curve.y[0]);
					curve.x[1] = form.GetPicturePosX(curve.x[1]);
					curve.y[1] = form.GetPicturePosY(curve.y[1]);
					curve.x[2] = form.GetPicturePosX(curve.x[2]);
					curve.y[2] = form.GetPicturePosY(curve.y[2]);
				}

				//int fill_flag = 1;
                object obj = new ObjectCurve(work.obj.objCommonProperty, form, null, null, new ObjectGeneral("Curve1"), Color.Black, new BrushSolid(Color.White), 1, 1, blockCurve);

                ClassEditInsert.InsertPublic(form, obj, "Insert Curve");
			}
			form.Invalidate();
		}
	}
}

