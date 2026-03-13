using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using NetTools.OldDefine;
using GraphicModule;
using NetTools;
using System.Drawing.Drawing2D;

namespace Studio
{
	enum EnumMouseCapture
	{
		CONTROL,	// curve의 콘트롤 포인트 선택
		NODE,		// 각 node의 포인트 선택
		MULTI,
		SHIFT,
	}

	class CONTROL_POINT_STRUCT
	{
		public  bool flag;
		public  int	 ps_x, ps_y;
		public  int	 pe_x, pe_y;
		public  int	 node_pos;
		public  int	 cent_pos;
		public  EnumCurveType	mode;
	}

	/// <summary>
	/// Summary description for ClassEditObjectPointMove.
	/// </summary>
	public class ClassEditObjectPointMove : ClassMainTool
	{
		public ClassEditObjectPointMove()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		ArrayList arraySelectNode = new ArrayList();

		bool bCaptureFlag;
		Point pointStart = new Point();
		Point pMouseOld = new Point();

		ArrayList blockCurve = new ArrayList();
		ArrayList blockCurveStart = new ArrayList();
		ObjectCurve objectSelect;
		CONTROL_POINT_STRUCT[] controlPoint = new CONTROL_POINT_STRUCT[4] { new CONTROL_POINT_STRUCT(), new CONTROL_POINT_STRUCT(), new CONTROL_POINT_STRUCT(), new CONTROL_POINT_STRUCT() };
		CONTROL_POINT_STRUCT[] controlPointSave = new CONTROL_POINT_STRUCT[4] { new CONTROL_POINT_STRUCT(), new CONTROL_POINT_STRUCT(), new CONTROL_POINT_STRUCT(), new CONTROL_POINT_STRUCT() };
		EnumMouseCapture cMouseCaptureForm;	
		int  nCaptureControlPoint;
		int nMouseCaptureNode;	// 여러개의 노드를 선택했을 때 마우스가 캡쳐된 노드를 알아둔다. 이것은 격자 맞춤에서 필요하다.
		Cursor hCursorCurveMain = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Studio.Cursor.CurveMain.cur"));
		Cursor hCursorCurveMove = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Studio.Cursor.CurveMove.cur"));

		public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			if(work.nSelectCount != 1)		return;

			SELECT_LIST list = work.selectList[0];

			if(IsPolyObjectSelected(form)) 
			{
				ClassEditObjectPointMovePoly.MouseDownLeft(form, e);
				return;
			}

			if(IsAnotherObjectSelected(form)) 
			{
				form.editMove.MouseDownLeft(form, e);
				return;
			}

			ObjectCurve obj = (ObjectCurve)list.obj;
			ArrayList block = new ArrayList();

			obj.GetCurveBlock(block);

			CONTROL_POINT_STRUCT control;
			Graphics g = form.CreateGraphics();
			int l;
			POINT pBase = new POINT();
			int dx, dy;

			work.obj.GetBasePoint(pBase);

			for(l = 0; l < 4; l++) 
			{
				control = controlPoint[l];
				if(control.flag == false)	continue;

				dx = form.GetDisplayPosX(control.pe_x);
				dy = form.GetDisplayPosY(control.pe_y);

				if(e.X >= dx-4 && e.X <= dx+4 && e.Y >= dy-4 && e.Y <= dy+4) 
				{	// control point 선택.

					if(l == 0) 
					{
						SelectedNodeNot(form, work.obj, g, block);
						arraySelectNode.Clear();
						SelectNodeAdd(block, control.cent_pos);

						SelectedNodePrepare(form);
						SelectedNodeNot(form, work.obj, g, block);

						nCaptureControlPoint = 2;
					}
					else if(l == 3) 
					{
						SelectedNodeNot(form, work.obj, g, block);
						arraySelectNode.Clear();
						SelectNodeAdd(block, control.cent_pos);

						SelectedNodePrepare(form);
						SelectedNodeNot(form, work.obj, g, block);
						nCaptureControlPoint = 1;
					}
					else 
					{
						nCaptureControlPoint = l;
						SelectedNodePrepare(form);
					}

					pointStart.X = e.X;
					pointStart.Y = e.Y;
					pMouseOld.X = e.X;
					pMouseOld.Y = e.Y;

					objectSelect = obj;
					cMouseCaptureForm = EnumMouseCapture.CONTROL;

					bCaptureFlag = true;
					form.Capture = true;
					XorCurve(form, g, blockCurve);

					return;
				}
			}

			CURVE_STRUCT curve;
			Point p = new Point();

			for(l = 0; l < block.Count; l++) 
			{
				curve = (CURVE_STRUCT)block[l];

				GetCurveMainPoint(curve, ref p);

				dx = form.GetDisplayPosX(p.X);
				dy = form.GetDisplayPosY(p.Y);

				if(e.X  >= dx-4 && e.X <= dx+4 &&
					e.Y >= dy-4 && e.Y <= dy+4) 
				{

					pointStart.X = e.X;
					pointStart.Y = e.Y;
					pMouseOld.X = e.X;
					pMouseOld.Y = e.Y;

					objectSelect = obj;
			
					if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) 
					{	// shift key를 누른상태
						if(IsSelected(l)) 
						{
							SelectNodeRemove(l);
						}
						else 
						{
							SelectNodeAdd(block, l);
						}
						SelectedNodePrepare(form);
						form.Invalidate();

						bCaptureFlag = true;
						form.Capture = true;
						cMouseCaptureForm = EnumMouseCapture.SHIFT;
						return;
					}
			
					if(arraySelectNode.Count == 0) 
					{
						SelectNodeAdd(block, l);
						SelectedNodePrepare(form);
						SelectedNodeNot(form, work.obj, g, block);
					}
					else if(IsSelected(l)) 
					{
						SelectedNodePrepare(form);
					}
					else 
					{
						SelectedNodeNot(form, work.obj, g, block);
						arraySelectNode.Clear();
						SelectNodeAdd(block, l);
						SelectedNodePrepare(form);
						SelectedNodeNot(form, work.obj, g, block);
					}

					bCaptureFlag = true;
					form.Capture = true;
					XorCurve(form, g, blockCurve);

					cMouseCaptureForm = EnumMouseCapture.NODE;
					nMouseCaptureNode = l;
		
					return;
				}
			}

			bCaptureFlag = true;
			form.Capture = true;
			pointStart.X = e.X;
			pointStart.Y = e.Y;
			pMouseOld.X = e.X;
			pMouseOld.Y = e.Y;
			cMouseCaptureForm = EnumMouseCapture.MULTI;
			ClearAllNodeSelection();
			SelectedNodePrepare(form);

			form.Invalidate();
		}

		void CurveCopy(CURVE_STRUCT cur_t, CURVE_STRUCT cur_s)
		{
			cur_t.type = cur_s.type;
			for(int j = 0; j < 3; j++) 
			{
				cur_t.x[j] = cur_s.x[j];
				cur_t.y[j] = cur_s.y[j];
			}
		}

		void BlockCopy(ArrayList target, ArrayList source)
		{
			CURVE_STRUCT cur_s;
			CURVE_STRUCT cur_t;

			target.Clear();

			for(int i = 0; i < source.Count; i++) 
			{
				cur_s = (CURVE_STRUCT)source[i];
				cur_t = new CURVE_STRUCT();

				CurveCopy(cur_t, cur_s);

				target.Add(cur_t);
			}
		}

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			if(IsPolyObjectSelected(form)) 
			{
				ClassEditObjectPointMovePoly.MouseMove(form, e);
				return;
			}
			
			if(IsAnotherObjectSelected(form)) 
			{
				form.editMove.MouseMove(form, e);
				return;
			}
			
			
			int gabx, gaby;

			if(bCaptureFlag && cMouseCaptureForm == EnumMouseCapture.CONTROL) 
			{
				Graphics g = form.CreateGraphics();
				CONTROL_POINT_STRUCT control;
				CONTROL_POINT_STRUCT controlSave;
				CURVE_STRUCT curve;

				int ctrl_calc_x = e.X;
				int ctrl_calc_y = e.Y;

				ClassEditObjectMove.CalcMoveOnPressedShift(pointStart.X, pointStart.Y, ref ctrl_calc_x, ref ctrl_calc_y);

				gabx = ctrl_calc_x-pointStart.X;
				gaby = ctrl_calc_y-pointStart.Y;

				XorCurve(form, g, blockCurve);
				SelectedNodeNot(form, form.workThis.obj, g, blockCurve);

				controlSave = controlPointSave[nCaptureControlPoint];
				control = controlPoint[nCaptureControlPoint];

				form.ConvertX(form.workThis, controlSave.pe_x, ref gabx);
				form.ConvertY(form.workThis, controlSave.pe_y, ref gaby);

				control.pe_x = controlSave.pe_x+gabx;
				control.pe_y = controlSave.pe_y+gaby;

				if(nCaptureControlPoint == 1 && ((control.mode & EnumCurveType.SYMMETRICAL) == EnumCurveType.SYMMETRICAL)) 
				{
					CONTROL_POINT_STRUCT control2 = controlPoint[2];
					control2.pe_x = control.ps_x-(control.pe_x-control.ps_x);
					control2.pe_y = control.ps_y-(control.pe_y-control.ps_y);
				}
				else if(nCaptureControlPoint == 2 && ((control.mode & EnumCurveType.SYMMETRICAL) == EnumCurveType.SYMMETRICAL)) 
				{
					CONTROL_POINT_STRUCT control2 = controlPoint[1];
					control2.pe_x = control.ps_x-(control.pe_x-control.ps_x);
					control2.pe_y = control.ps_y-(control.pe_y-control.ps_y);
				}
				else if(nCaptureControlPoint == 1 && ((control.mode & EnumCurveType.SMOOTH) == EnumCurveType.SMOOTH)) 
				{
					ControlPointMoveBySmooth(control, controlPointSave[2], controlPoint[2]);
				}
				else if(nCaptureControlPoint == 2 && ((control.mode & EnumCurveType.SMOOTH) == EnumCurveType.SMOOTH)) 
				{
					ControlPointMoveBySmooth(control, controlPointSave[1], controlPoint[1]);
				}
				else {}

				for(int i = 0; i < 4; i++) 
				{
					control = controlPoint[i];
					if(control.flag == false)	continue;
			
					curve = (CURVE_STRUCT)blockCurve[control.node_pos];
					if(i == 0 || i == 2) 
					{
						curve.x[0] = control.pe_x;
						curve.y[0] = control.pe_y;
					}
					else 
					{
						curve.x[1] = control.pe_x;
						curve.y[1] = control.pe_y;
					}
				}	

				SelectedNodeNot(form, form.workThis.obj, g, blockCurve);
				XorCurve(form, g, blockCurve);

				pMouseOld.X = ctrl_calc_x;
				pMouseOld.Y = ctrl_calc_y;

				form.Invalidate();

				return;
			}
			else if(bCaptureFlag && cMouseCaptureForm == EnumMouseCapture.NODE) 
			{
				CURVE_STRUCT curve, curve_s, curve_e;
				Point p = new Point();
				Graphics g = form.CreateGraphics();

				curve = (CURVE_STRUCT)blockCurveStart[nMouseCaptureNode];
				GetCurveMainPoint(curve, ref p);

				int ctrl_calc_x = e.X;
				int ctrl_calc_y = e.Y;

				ClassEditObjectMove.CalcMoveOnPressedShift(pointStart.X, pointStart.Y, ref ctrl_calc_x, ref ctrl_calc_y);

				gabx = ctrl_calc_x-pointStart.X;
				gaby = ctrl_calc_y-pointStart.Y;

				form.ConvertX(form.workThis, p.X, ref gabx);
				form.ConvertY(form.workThis, p.Y, ref gaby);
		
				int pos_s, pos_e;
				int pos;
				
				XorCurve(form, g, blockCurve);

				SelectedNodeNot(form, form.workThis.obj, g, blockCurve);

				BlockCopy(blockCurve, blockCurveStart);
		
				for(int i = 0; i < arraySelectNode.Count; i++) 
				{
					pos = (int)arraySelectNode[i];
			
					curve = (CURVE_STRUCT)blockCurve[pos];
					GetNodeStartEnd(blockCurve, out curve_s, out curve_e, out pos_s, out pos_e, pos);

					if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
					{
						curve.x[1] += gabx;
						curve.y[1] += gaby;
						curve.x[2] += gabx;
						curve.y[2] += gaby;
					}
					else 
					{
						curve.x[0] += gabx;
						curve.y[0] += gaby;
					}

					if(pos == pos_s) 
					{	// node의 시작.
						if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)		// 닫힌 노드
						{
							if((curve_e.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
							{
								curve_e.x[1] += gabx;
								curve_e.y[1] += gaby;
								curve_e.x[2] += gabx;
								curve_e.y[2] += gaby;
							}
							else 
							{
								curve_e.x[0] += gabx;
								curve_e.y[0] += gaby;
							}
						}

						curve = (CURVE_STRUCT)blockCurve[pos+1];
						if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							curve.x[0] += gabx;
							curve.y[0] += gaby;
						}
					}
					else if(pos == pos_e) 
					{	// node의 끝 

					}
					else 
					{
						curve = (CURVE_STRUCT)blockCurve[pos+1];
						if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							curve.x[0] += gabx;
							curve.y[0] += gaby;
						}
					}
				}

				if(arraySelectNode.Count == 1) 
				{
					AddControlPoint(blockCurve, (int)arraySelectNode[0]);
				}
				
				SelectedNodeNot(form, form.workThis.obj, g, blockCurve);

				pMouseOld.X = ctrl_calc_x;
				pMouseOld.Y = ctrl_calc_y;

				form.Invalidate();

				return;
			}
			else if(bCaptureFlag && cMouseCaptureForm == EnumMouseCapture.MULTI) 
			{
				Graphics g = form.CreateGraphics();

				XorRect(g, pointStart.X, pointStart.Y, pMouseOld.X, pMouseOld.Y);

				int ctrl_calc_x = e.X;
				int ctrl_calc_y = e.Y;

				ClassEditObjectMove.CalcMoveOnPressedShift(pointStart.X, pointStart.Y, ref ctrl_calc_x, ref ctrl_calc_y);
				pMouseOld.X = ctrl_calc_x;
				pMouseOld.Y = ctrl_calc_y;

				XorRect(g, pointStart.X, pointStart.Y, pMouseOld.X, pMouseOld.Y);

				form.Invalidate();
				return;
			}

			if(bCaptureFlag == false) 
			{
				MouseCursorStatus(form, e);
			}
		}

		public override void MouseUpLeft(FormEditGraphic form, MouseEventArgs e)
		{
			if(IsPolyObjectSelected(form)) 
			{
				ClassEditObjectPointMovePoly.MouseUpLeft(form, e);
				return;
			}
			
			if(IsAnotherObjectSelected(form)) 
			{
				form.editMove.MouseUpLeft(form, e);
				return;
			}

			if(bCaptureFlag == false) 
			{
				SelectAnotherObject(form, e);
				return;
			}

			bCaptureFlag = false;
			form.Capture = false;

			if(cMouseCaptureForm == EnumMouseCapture.NODE) 
			{
				if(pointStart != pMouseOld) 
				{
					ClassStudioEditUndo.UndoSave_Selected(form, "Curve node move");
					objectSelect.SetCurveBlock(form, blockCurve);
					form.SelectListUpdateFirstItem();
                    ClassEditProperty.ObjectPosSizeChanged(form);   // 크기가 변경되었으므로 속성상자에 크기변경을 알려준다.
					form.SetChangeFlag();
				}
				form.Invalidate(); 
				return;
			}

			if(cMouseCaptureForm == EnumMouseCapture.CONTROL) 
			{
				if(pointStart != pMouseOld) 
				{
					ClassStudioEditUndo.UndoSave_Selected(form, "Curve control move");
					objectSelect.SetCurveBlock(form, blockCurve);
					form.SelectListUpdateFirstItem();
                    ClassEditProperty.ObjectPosSizeChanged(form);   // 크기가 변경되었으므로 속성상자에 크기변경을 알려준다.
					form.SetChangeFlag();
				}
				form.Invalidate();
				return;
			}

			if(cMouseCaptureForm == EnumMouseCapture.SHIFT) 
			{
				return;
			}

			if(pointStart == pMouseOld) 
			{	// 마우스가 움직이지 않았을 때 
				SelectAnotherObject(form, e);
			}
			else 
			{	// 마우스가 움직였을 때는 멀티 선택모드로 들어간다.
				arraySelectNode.Clear();

				int l;
				CURVE_STRUCT curve;
				Point p = new Point();
				int x1, y1, x2, y2;
				POINT pBase = new POINT();

				form.workThis.obj.GetBasePoint(pBase);

				x1 = pointStart.X;
				y1 = pointStart.Y;
				x2 = pMouseOld.X;
				y2 = pMouseOld.Y;

				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

				x1 = form.GetPicturePosX(x1);
				y1 = form.GetPicturePosY(y1);
				x2 = form.GetPicturePosX(x2);
				y2 = form.GetPicturePosY(y2);

				for(l = 0; l < blockCurve.Count; l++) 
				{
					curve = (CURVE_STRUCT)blockCurve[l];
					GetCurveMainPoint(curve, ref p);
					if(p.X >= x1 && p.X <= x2 && p.Y >= y1 && p.Y <= y2) 
					{
						if((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)
						{}
						else 
						{
							SelectNodeAdd(blockCurve, l);
						}
					}
				}

				SelectedNodePrepare(form);
			}

			form.Invalidate();
		}

		bool IsPolyObjectSelected(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			if(work.nSelectCount != 1)		return false;

			SELECT_LIST list = work.selectList[0];

			EnumObjectType type = ((ObjectType)list.obj).enumObjectType;

			if(type == EnumObjectType.Poly)	return true;

			return false;
		}

		
		bool IsAnotherObjectSelected(FormEditGraphic form)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			if(work.nSelectCount != 1)		return false;

			SELECT_LIST list = work.selectList[0];

            EnumObjectType type = ((ObjectType)list.obj).enumObjectType;

			if(type == EnumObjectType.Curve)	return false;
			if(type == EnumObjectType.Poly)		return false;

			return true;
		}
		

		public override void Paint(FormEditGraphic form, Graphics g)
		{
			if(IsPolyObjectSelected(form))
			{
				ClassEditObjectPointMovePoly.Paint(form, g);
				return;
			}
			else if(IsAnotherObjectSelected(form))
			{
				int i;

				for(i = 0; i < form.workThis.nSelectCount; i++) 
				{
					form.ObjectNotOne(g, form.workThis.selectList[i], i == 0);
				}
			}
			else {
				ObjectNotPointMove(form, form.workThis, g);
			
				if(bCaptureFlag) 
				{
					XorCurve(form, g, blockCurve);
					if(cMouseCaptureForm == EnumMouseCapture.MULTI)
						XorRect(g, pointStart.X, pointStart.Y, pMouseOld.X, pMouseOld.Y);
				}
			}
		}

		void SelectedNodeNot(FormEditGraphic form, ObjectRoot plan, Graphics g, ArrayList block)
		{
			if(arraySelectNode.Count == 0)		return;

			int node;

			for(int i = 0; i < arraySelectNode.Count; i++) 
			{
				node = (int)arraySelectNode[i];
				ObjectSelectPointNot(form, plan, g, block, node);
			}
		}

		void ObjectSelectPointNot(FormEditGraphic form, ObjectRoot plan, Graphics g, ArrayList block, int pos)
		{
			if(pos == -1)	return;							// zone over
			if(pos >= block.Count)	return;		// zone over

			Point p = new Point();
			CURVE_STRUCT curve;
			POINT pBase = new POINT();

			plan.GetBasePoint(pBase);

			curve = (CURVE_STRUCT)block[pos];
			GetCurveMainPoint(curve, ref p);

			NotFillRectangleBig(g, form.GetDisplayPosX(p.X), form.GetDisplayPosY(p.Y));

			if(arraySelectNode.Count >= 2)	return;	// multi select

			CONTROL_POINT_STRUCT control;
	
			for(int i = 0; i < 4; i++) 
			{
				control = controlPoint[i];
				if(control.flag)
					NotCurveMember(g,	form.GetDisplayPosX(control.ps_x), 
										form.GetDisplayPosY(control.ps_y), 
										form.GetDisplayPosX(control.pe_x), 
										form.GetDisplayPosY(control.pe_y));
			}
		}

		void SelectNodeAdd(ArrayList block, int pos)
		{
			CURVE_STRUCT curve_s, curve_e;
			int pos_s, pos_e;
	
			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

			if(pos == pos_e && ((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)) 
			{
				pos = pos_s;
			}

			arraySelectNode.Add(pos);
		}

		void CopyControlPointBlock(ref CONTROL_POINT_STRUCT[] target, ref CONTROL_POINT_STRUCT[] source)
		{
			for(int i = 0; i < 4; i++) 
			{
				target[i].cent_pos = source[i].cent_pos;	
				target[i].flag = source[i].flag;
				target[i].mode = source[i].mode;
				target[i].node_pos = source[i].node_pos;
				target[i].pe_x = source[i].pe_x;
				target[i].pe_y = source[i].pe_y;
				target[i].ps_x = source[i].ps_x;
				target[i].ps_y = source[i].ps_y;
			}
		}

		public void SelectedNodePrepare(FormEditGraphic form)
		{
			for(int i = 0; i < 4; i++) 
			{
				AddOneControlPointOFF(i);
			}

			WORK_MODULE_STRUCT work = form.workThis;

			if(work.nSelectCount != 1)		return;

			SELECT_LIST list = work.selectList[0];

            EnumObjectType type = ((ObjectType)list.obj).enumObjectType;

			if(type != EnumObjectType.Curve)	return;

			ObjectCurve obj = (ObjectCurve)list.obj;

			obj.GetCurveBlock(blockCurve);
			BlockCopy(blockCurveStart, blockCurve);

			if(arraySelectNode.Count == 1) 
			{
				AddControlPoint(blockCurve, (int)arraySelectNode[0]);
			}

			CopyControlPointBlock(ref controlPointSave, ref controlPoint);
		}

		void XorCurve(FormEditGraphic form, Graphics g, ArrayList blockPoint)
		{
			int hap = 0;
			CURVE_STRUCT curve;
			int l;

			for(l = 0; l < blockPoint.Count; l++) 
			{
				curve = (CURVE_STRUCT)blockPoint[l];
				if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)	
					hap+=3;
				else 
					hap+=1;
			}

			Point[] polygon = new Point[hap];
			byte[]  polyatr = new byte[hap];

			POINT pBase = new POINT();

			form.workThis.obj.GetBasePoint(pBase);

			hap = 0;
			for(l = 0; l < blockPoint.Count; l++) 
			{
				curve = (CURVE_STRUCT)blockPoint[l];

				if((curve.type & EnumCurveType.START) == EnumCurveType.START) 
				{
					polygon[hap].X = form.GetDisplayPosX(curve.x[0]);
					polygon[hap].Y = form.GetDisplayPosY(curve.y[0]);

					polyatr[hap] = (byte)PathPointType.Start;//PT_MOVETO;
					hap++;
				}
				else if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
				{
					for(int j = 0; j < 3; j++) 
					{
						polygon[hap].X = form.GetDisplayPosX(curve.x[j]);
						polygon[hap].Y = form.GetDisplayPosY(curve.y[j]);

						polyatr[hap] = (byte)PathPointType.Bezier;//PT_BEZIERTO;

						if(j == 2 && ((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)) 
						{
							polyatr[hap] |= (byte)PathPointType.CloseSubpath;//PT_CLOSEFIGURE;
						}

						hap++;
					}
				}
				else 
				{
					polygon[hap].X = form.GetDisplayPosX(curve.x[0]);
					polygon[hap].Y = form.GetDisplayPosY(curve.y[0]);

					polyatr[hap] = (byte)PathPointType.Line;//PT_LINETO;

					if((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
					{
						polyatr[hap] |= (byte)PathPointType.CloseSubpath;//PT_CLOSEFIGURE;
					}
	
					hap++;
				}
			}

			GraphicsPath path = new GraphicsPath(polygon, polyatr);

			Pen pen = new Pen(Color.Black, 1);
			g.DrawPath(pen, path);
		}

		// main point 는 곡선은 x[2], y[2] 이외는 x[0],y[0]의 값이다.
		static void GetCurveMainPoint(CURVE_STRUCT curve, ref Point point)
		{
			if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
			{
				point.X = curve.x[2];
				point.Y = curve.y[2];
			}
			else 
			{
				point.X = curve.x[0];
				point.Y = curve.y[0];
			}
		}

		bool IsSelected(int pos)
		{
			int i;
			int  val;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				val = (int)arraySelectNode[i];
				if(val == pos)	return true;
			}

			return false;
		}

		void SelectNodeRemove(int pos)
		{
			for(int i = 0; i < arraySelectNode.Count; i++) 
			{
				if(pos == (int)arraySelectNode[i]) 
				{
					arraySelectNode.RemoveAt(i);
					return;
				}
			}
		}

		void ClearAllNodeSelection()
		{
			arraySelectNode.Clear();
			for(int i = 0; i < 4; i++) 
			{
				AddOneControlPointOFF(i);
			}
		}

		void AddOneControlPointOFF(int pos)
		{
			CONTROL_POINT_STRUCT control = controlPoint[pos];

			//ZeroMemory(control, sizeof(CONTROL_POINT_STRUCT));

			control.flag = false;
		}

		

		static void ControlPointMoveBySmooth(CONTROL_POINT_STRUCT control, CONTROL_POINT_STRUCT control2, CONTROL_POINT_STRUCT control3)
		{
			double x = (control.pe_x-control.ps_x);
			double y = (control.pe_y-control.ps_y);
			double sgak;
			double tgak;

			int buhox=-1, buhoy=-1;
			if(x < 0)	buhox = 1;	
			if(y < 0)	buhoy = 1;	

			x = Math.Abs(x);
			y = Math.Abs(y);

			if(x == 0)	sgak = Math.Atan(y);
			else		sgak = Math.Atan(y/x);	// 소스의 각도

			x = Math.Abs(control2.pe_x-control2.ps_x);
			y = Math.Abs(control2.pe_y-control2.ps_y);

			double tlen;

			if(x == 0)		tlen = y;
			else if(y == 0)	tlen = x;
			else 
			{
				tgak = Math.Atan(y/x);					// 타겟의 각도
				double costgak = Math.Cos(tgak);
	
				if(costgak == 0)	tlen = x;		// 타겟의 길이
				else				tlen = x/costgak;
			}

			x = Math.Cos(sgak)*tlen;
			y = Math.Sin(sgak)*tlen;

			control3.pe_x = (int)(control.ps_x+buhox*x);
			control3.pe_y = (int)(control.ps_y+buhoy*y);
		}

		static void GetNodeStartEnd(ArrayList block, out CURVE_STRUCT curve_s, out CURVE_STRUCT curve_e, out int pos_s, out int pos_e, int pos_default)
		{
			int i;

			pos_s = pos_default;
			pos_e = pos_default;
			curve_s = (CURVE_STRUCT)block[pos_default];
			curve_e = (CURVE_STRUCT)block[pos_default];

			for(i = pos_default; i >= 0; i--) 
			{
				pos_s = i;
				curve_s = (CURVE_STRUCT)block[i];
				if((curve_s.type & EnumCurveType.START) == EnumCurveType.START)		break;
			}

			for(i = pos_default; i < block.Count; i++) 
			{
				pos_e = i;
				curve_e = (CURVE_STRUCT)block[i];			
				if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)	return;	// 이미 close 되어있다.
				if(i > pos_default) 
				{
					if((curve_e.type & EnumCurveType.START) == EnumCurveType.START)	
					{
						curve_e = (CURVE_STRUCT)block[i-1];
						pos_e = i-1;
						return;
					}
				}
			}
		}

		void AddControlPoint(ArrayList block, int pos)
		{
			for(int i = 0; i < 4; i++) 
			{		// 모든 콘트롤 포인트를 OFF 시킨다.
				AddOneControlPointOFF(i);
			}

			if(pos == -1)	return;					// zone over
			if(pos >= (int)block.Count)	return;		// zone over

			Point p = new Point();
			Point p_n = new Point();
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_n;
			CURVE_STRUCT curve_s, curve_e;
			int pos_s, pos_e;
	
			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);
			curve = (CURVE_STRUCT)block[pos];
			GetCurveMainPoint(curve, ref p);

			// 마지막 포인트가 Close되어 있으면 첫 포인트를 선택한것과 같은 동작을 한다.
			//	if(pos == pos_e && ((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)) {
			//		pos = pos_s;
			//	}

			if(pos == pos_s) 
			{	// 시작 포인터
				if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
				{
					if((curve_e.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
					{
						curve_n = (CURVE_STRUCT)block[pos_e-1];
						GetCurveMainPoint(curve_n, ref p_n);
						AddOneControlPointON(0, p_n.X, p_n.Y, curve_e.x[0], curve_e.y[0], pos_e, pos_e-1, 0);
						AddOneControlPointON(1, curve_e.x[2], curve_e.y[2], curve_e.x[1], curve_e.y[1], pos_e, pos, curve_e.type);
					}
					else 
					{
						AddOneControlPointOFF(0);
						AddOneControlPointOFF(1);
					}
				}
				else 
				{
					AddOneControlPointOFF(0);
					AddOneControlPointOFF(1);
				}
			}
			else 
			{
				if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
				{
					curve_n = (CURVE_STRUCT)block[pos-1];
					GetCurveMainPoint(curve_n, ref p_n);
					AddOneControlPointON(0, p_n.X, p_n.Y, curve.x[0], curve.y[0], pos, pos-1, 0);
					AddOneControlPointON(1, curve.x[2], curve.y[2], curve.x[1], curve.y[1], pos, pos, curve.type);
				}
				else 
				{
					AddOneControlPointOFF(0);
					AddOneControlPointOFF(1);
				}
			}

			if(pos < pos_e) 
			{
				curve_n = (CURVE_STRUCT)block[pos+1];
				if((curve_n.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
				{
					GetCurveMainPoint(curve_n, ref p_n);
					AddOneControlPointON(2, p.X, p.Y, curve_n.x[0], curve_n.y[0], pos+1, pos, curve.type);
					AddOneControlPointON(3, curve_n.x[2], curve_n.y[2], curve_n.x[1], curve_n.y[1], pos+1, pos+1, 0);
				}
				else 
				{
					AddOneControlPointOFF(2);
					AddOneControlPointOFF(3);
				}
			}
			else 
			{
				AddOneControlPointOFF(2);
				AddOneControlPointOFF(3);
			}
		}

		void MouseCursorStatus(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			if(work.nSelectCount != 1) 
			{
				form.Cursor = hCursorCurveMain;
				return;
			}

			SELECT_LIST list = work.selectList[0];

            EnumObjectType type = ((ObjectType)list.obj).enumObjectType;

			if(type != EnumObjectType.Curve)	
			{
				form.Cursor = hCursorCurveMain;
				return;
			}

			ObjectCurve obj = (ObjectCurve)list.obj;
			ArrayList block = new ArrayList();

			obj.GetCurveBlock(block);

			CONTROL_POINT_STRUCT control;
			Graphics g = form.CreateGraphics();
			int l;
			int dx, dy;

			for(l = 0; l < 4; l++) 
			{
				control = controlPoint[l];
				if(control.flag == false)	continue;

				dx = form.GetDisplayPosX(control.pe_x);
				dy = form.GetDisplayPosY(control.pe_y);

				if(e.X >= dx-4 && e.X <= dx+4 && e.Y >= dy-4 && e.Y <= dy+4) 
				{	// control point 선택.
					form.Cursor = hCursorCurveMove;
					return;
				}
			}

			CURVE_STRUCT curve;
			Point p = new Point();

			for(l = 0; l < block.Count; l++) 
			{
				curve = (CURVE_STRUCT)block[l];

				GetCurveMainPoint(curve, ref p);

				dx = form.GetDisplayPosX(p.X);
				dy = form.GetDisplayPosY(p.Y);

				if(e.X  >= dx-4 && e.X <= dx+4 && e.Y >= dy-4 && e.Y <= dy+4) 
				{
					form.Cursor = hCursorCurveMove;
					return;
				}
			}

			form.Cursor = hCursorCurveMain;
		}

		static void XorRect(Graphics g, int x1, int y1, int x2, int y2)
		{
			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			Pen pen = new Pen(Color.Black, 1);

			g.DrawRectangle(pen, x1, y1, x2-x1, y2-y1);
		}

		void SelectAnotherObject(FormEditGraphic form, MouseEventArgs e)
		{
			// 마우스를 떼었을 때 mouse 캡쳐가 되지 않았으면 다른 Curve Object를 선택하도록 한다.
			// 하나가 마우스영역에 포함되면 그것을 선택하고 돌아간다.

			WORK_MODULE_STRUCT work = form.workThis;

            object obj;
            if(ClassEditObjectMove.RecurseMouseCheckZone(form, work.obj.groupRoot, e, out obj, false))
            {
				form.SelectListClear(work);
				form.SelectListAdd(work, obj);

                form.DisplayAfterSelectedChanged();
				ClearAllNodeSelection();
				form.Invalidate();
				return;
			}
		}

		static void NotFillRectangleSmall(Graphics g, int x, int y)
		{
			RECT r = new RECT();

			r.left = x-3;
			r.top  = y-3;
			r.right= x+4;
			r.bottom=y+4;

			DrawClass.InvertRect(g, r);

			Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
			g.FillRectangle(brush, x-2, y-2, 5, 5);
			Pen pen = new Pen(Color.Black, 1);
			g.DrawRectangle(pen, x-3, y-3, 6, 6);
		}

		static void NotFillRectangleBig(Graphics g, int x, int y)
		{
			Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
			g.FillRectangle(brush, x-3, y-3, 7, 7);
			Pen pen = new Pen(Color.Black, 1);
			g.DrawRectangle(pen, x-4, y-4, 8, 8);
		}

		static void NotRectangle(Graphics g, int x, int y, int size)
		{
			Brush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
			g.FillRectangle(brush, x-size+1, y-size+1, size*2-1, size*2-1);
			Pen pen = new Pen(Color.Black, 1);
			g.DrawRectangle(pen, x-size, y-size, size*2, size*2);
		}

		static void NotRectangleSmall(Graphics g, int x, int y)
		{
			NotRectangle(g, x, y, 2);
		}

		static void NotRectangleBig(Graphics g, int x, int y)
		{
			NotRectangle(g, x, y, 3);
		}

		static void NotCurveMember(Graphics g, int x1, int y1, int x2, int y2)
		{
			/*
			HPEN hPen, hPenOld;

			hPen = CreatePen(PS_SOLID, 1, RGB(0,255,0));
			hPenOld = (HPEN)SelectObject(hdc, hPen);

			int save_rop = SetROP2(hdc, R2_XORPEN);
			MoveToEx(hdc, x1, y1, NULL);
			LineTo(hdc, x2, y2);
			SetROP2(hdc, save_rop);

			SelectObject(hdc, hPenOld);
			DeleteObject(hPen);
			*/

			Pen pen = new Pen(Color.Black, 1);
			g.DrawLine(pen, x1, y1, x2, y2);

			NotFillRectangleSmall(g, x2, y2);
		}

		void AddOneControlPointON(int pos, int x1, int y1, int x2, int y2, int node_pos, int cent_pos, EnumCurveType mode)
		{
			/*
			CONTROL_POINT_STRUCT control = controlPoint[pos];

			//ZeroMemory(control, sizeof(CONTROL_POINT_STRUCT));

			control.flag = true;
			control.ps_x = x1;
			control.ps_y = y1;
			control.pe_x = x2;
			control.pe_y = y2;
			control.node_pos = node_pos;
			control.cent_pos = cent_pos;
			control.mode = mode;
			*/

			controlPoint[pos].flag = true;
			controlPoint[pos].ps_x = x1;
			controlPoint[pos].ps_y = y1;
			controlPoint[pos].pe_x = x2;
			controlPoint[pos].pe_y = y2;
			controlPoint[pos].node_pos = node_pos;
			controlPoint[pos].cent_pos = cent_pos;
			controlPoint[pos].mode = mode;
		}

		void ObjectNotPointMove(FormEditGraphic form, WORK_MODULE_STRUCT work, Graphics g)
		{
			if(work.nSelectCount != 1)			return;

			SELECT_LIST list = work.selectList[0];

            EnumObjectType type = ((ObjectType)list.obj).enumObjectType;

			if(type != EnumObjectType.Curve)	return;

			ObjectCurve obj = (ObjectCurve)list.obj;

			ArrayList block = new ArrayList();
			CURVE_STRUCT curve;
			POINT pBase = new POINT();

			Point p = new Point();

			obj.GetCurveBlock(block);

			work.obj.GetBasePoint(pBase);
	
			for(int l = 0; l < block.Count; l++) 
			{
				curve = (CURVE_STRUCT)block[l];

				GetCurveMainPoint(curve, ref p);

				if((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)	continue;

				if(IsSelected(l)) 
				{
					ObjectSelectPointNot(form, work.obj, g, block, l);
				}
				else 
				{
					if((curve.type & EnumCurveType.START) == EnumCurveType.START) 
					{
						NotRectangleBig(g, form.GetDisplayPosX(p.X), form.GetDisplayPosY(p.Y));
					}
					else 
					{
						if((curve.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)
						{}
						else
							NotRectangleSmall(g, form.GetDisplayPosX(p.X), form.GetDisplayPosY(p.Y));
					}
				}
			}
		}

		public override void MouseDownRight(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			if(work.nSelectCount != 1)		return;

			SELECT_LIST list = work.selectList[0];

            EnumObjectType type = ((ObjectType)list.obj).enumObjectType;

			if(type != EnumObjectType.Curve) 
			{
				form.contextMenuStripMove.Show(form, new Point(e.X, e.Y));
				return;
			}

			ObjectCurve obj = (ObjectCurve)list.obj;

			ArrayList block = new ArrayList();
			CURVE_STRUCT curve;
			Graphics g = form.CreateGraphics();
			Point p = new Point();
			POINT pBase = new POINT();

			work.obj.GetBasePoint(pBase);

			obj.GetCurveBlock(block);

			int dx, dy;

			for(int l = 0; l < block.Count; l++) 
			{
				curve = (CURVE_STRUCT)block[l];

				GetCurveMainPoint(curve, ref p);

				dx = form.GetDisplayPosX(p.X);
				dy = form.GetDisplayPosY(p.Y);

				if(e.X  >= dx-4 && e.X <= dx+4 && e.Y >= dy-4 && e.Y <= dy+4) 
				{

					if(IsSelected(l)) 
					{
				
					}
					else 
					{
						arraySelectNode.Clear();
						SelectNodeAdd(block, l);
					}

					objectSelect = obj;

					SelectedNodePrepare(form);
					form.Invalidate();

					form.contextMenuStripPointMove.Show(form, new Point(e.X, e.Y));

					return;
				}
			}
			ClearAllNodeSelection();
			form.Invalidate();
			return;
		}

		public bool IsPossibleAdd(FormEditGraphic form)
		{
			if(arraySelectNode.Count != 1)	return false;

			ArrayList block = new ArrayList();	

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve_s, curve_e;
			int pos_s, pos_e;
			int pos = (int)arraySelectNode[0];

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

			if(pos == pos_e)	return false;	// 끝 포인터에서는 추가를 할 수 없다.

			return true;
		}

		public void IdmCurvePointAdd(FormEditGraphic form)
		{
			if(!IsPossibleAdd(form))	return;

			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			Point p = new Point();
			CURVE_STRUCT curve_s, curve_e;
			int pos_s, pos_e;
			int pos = (int)arraySelectNode[0];

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

			curve = (CURVE_STRUCT)block[pos];
			GetCurveMainPoint(curve, ref p);

			if(pos == pos_e)	return;	// 끝 포인터에서는 추가를 할 수 없다.

			curve = new CURVE_STRUCT();
			curve.type = EnumCurveType.LINE;
			curve.x[0] = p.X+10;
			curve.y[0] = p.Y+10;

			block.Insert(pos+1, curve);

			ClassStudioEditUndo.UndoSave_Selected(form, "Curve point add");
			form.SetChangeFlag();

			objectSelect.SetCurveBlock(form, block);
			form.SelectListUpdateFirstItem();

			arraySelectNode.Clear();
			SelectNodeAdd(block, pos+1);

			SelectedNodePrepare(form);

			form.Invalidate();
		}

		public bool IsPossibleDelete(FormEditGraphic form)
		{
			if(arraySelectNode.Count != 1)	return false;
			return true;
		}

		public void IdmCurvePointDelete(FormEditGraphic form)
		{
			if(!IsPossibleDelete(form))		return;

			ArrayList block = new ArrayList();	

			objectSelect.GetCurveBlock(block);

			if(block.Count <= 2) 
			{
				ClearAllNodeSelection();

				ClassStudioEdit.EditDelete(form);
				return;
			}

			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
			int pos_s = 0, pos_e = 0;
			Point p = new Point();
			int pos = (int)arraySelectNode[0];

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

			int hap = pos_e-pos_s+1;
			int i;

			if(hap <= 2) 
			{
				for(i = 0; i < hap; i++) 
				{
					block.RemoveAt(pos_s);
				}

				ClassStudioEditUndo.UndoSave_Selected(form, "Curve point delete");
				form.SetChangeFlag();

				objectSelect.SetCurveBlock(form, block);
				form.SelectListUpdateFirstItem();
				ClearAllNodeSelection();
				form.Invalidate();
				return;
			}

			if(pos == pos_s) 
			{	// START node
				curve = (CURVE_STRUCT)block[pos_s+1];
				GetCurveMainPoint(curve, ref p);
				curve.type = EnumCurveType.START;
				curve.x[0] = p.X;
				curve.y[0] = p.Y;
				//block.SetBlock(&curve, pos_s+1);

				if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
				{
					if((curve_e.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
					{
						curve_e.x[2] = p.X;
						curve_e.y[2] = p.Y;
						//block.SetBlock(&curve_e, pos_e);
					}
					else 
					{
						curve_e.x[0] = p.X;
						curve_e.y[0] = p.Y;
						//block.SetBlock(&curve_e, pos_e);
					}
				}

				block.RemoveAt(pos_s);

				ClassStudioEditUndo.UndoSave_Selected(form, "Curve point delete");
				form.SetChangeFlag();

				objectSelect.SetCurveBlock(form, block);
				form.SelectListUpdateFirstItem();
				ClearAllNodeSelection();
				form.Invalidate();

				return;
			}
			else 
			{
				block.RemoveAt(pos);

				ClassStudioEditUndo.UndoSave_Selected(form, "Curve point delete");
				form.SetChangeFlag();

				objectSelect.SetCurveBlock(form, block);
				form.SelectListUpdateFirstItem();
				ClearAllNodeSelection();
				form.Invalidate();
			}
		}

		public bool IsPossibleToCurve(FormEditGraphic form)
		{
			if(arraySelectNode.Count == 0)	return false;

			ArrayList block = new ArrayList();	

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
	
			int pos_s, pos_e;
			int pos;
			int i;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos = (int)arraySelectNode[i];

				GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

				if(pos == pos_s) 
				{
					if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
					{
						if((curve_e.type & EnumCurveType.LINE) == EnumCurveType.LINE)	return true;
					}
					else 
					{
						continue;
					}
				}
				else 
				{
					curve = (CURVE_STRUCT)block[pos];
					if((curve.type & EnumCurveType.LINE) == EnumCurveType.LINE) 
					{
						return true;
					}
				}
			}

			return false;
		}

		public void IdmCurvePointToCurve(FormEditGraphic form)
		{
			if(!IsPossibleToCurve(form))	return;

			ArrayList block = new ArrayList();	

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_pre;
			CURVE_STRUCT curve_s, curve_e;
	
			int pos_s, pos_e;
			Point p1 = new Point();
			Point p2 = new Point();
			int pos;
			int i;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos = (int)arraySelectNode[i];

				GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

				if(pos == pos_s) 
				{
					if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
					{
						curve = (CURVE_STRUCT)block[pos_e];
						curve_pre = (CURVE_STRUCT)block[pos_e-1];
					}
					else 
					{
						continue;
					}
				}
				else 
				{
					curve = (CURVE_STRUCT)block[pos];
					curve_pre = (CURVE_STRUCT)block[pos-1];
				}

				GetCurveMainPoint(curve_pre, ref p1);
				GetCurveMainPoint(curve,     ref p2);

				if((curve.type & EnumCurveType.LINE) == EnumCurveType.LINE) 
				{
					CURVE_STRUCT curve_imsi = new CURVE_STRUCT();
					CurveCopy(curve_imsi, curve);

					curve_imsi.type &= ~EnumCurveType.LINE;
					curve_imsi.type |= EnumCurveType.BEZIER;
					curve_imsi.x[0] = p1.X+(p2.X-p1.X)/3;
					curve_imsi.y[0] = p1.Y+(p2.Y-p1.Y)/3;
					curve_imsi.x[1] = p1.X+(p2.X-p1.X)*2/3;
					curve_imsi.y[1] = p1.Y+(p2.Y-p1.Y)*2/3;
					curve_imsi.x[2] = p2.X;
					curve_imsi.y[2] = p2.Y;

					if(pos == pos_s) 
					{
						block[pos_e] = curve_imsi;
					}
					else 
					{
						block[pos] = curve_imsi;
					}
				}
			}

			ClassStudioEditUndo.UndoSave_Selected(form, "Curve point to curve");
			form.SetChangeFlag();

			objectSelect.SetCurveBlock(form, block);
			form.SelectListUpdateFirstItem();
			SelectedNodePrepare(form);
			form.Invalidate();
		}

		public bool IsPossibleToLine(FormEditGraphic form)
		{
			if(arraySelectNode.Count == 0)	return false;

			ArrayList block = new ArrayList();	

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
	
			int pos_s, pos_e;
			int pos;
			int i;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos = (int)arraySelectNode[i];

				GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

				if(pos == pos_s) 
				{
					if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
					{
						if((curve_e.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)	return true;
					}
					else 
					{
						continue;
					}
				}
				else 
				{
					curve = (CURVE_STRUCT)block[pos];
					if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
					{
						return true;
					}
				}
			}

			return false;
		}

		public void IdmCurvePointToLine(FormEditGraphic form)
		{
			if(!IsPossibleToLine(form))			return;

			ArrayList block = new ArrayList();	

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
			int pos_s, pos_e;
			int pos;
			int i;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos = (int)arraySelectNode[i];

				GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);
				curve = (CURVE_STRUCT)block[pos];

				if(pos == pos_s) 
				{
					if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
					{
						curve = (CURVE_STRUCT)block[pos_e];
					}
					else 
					{
						continue;
					}
				}
				else 
				{
					curve = (CURVE_STRUCT)block[pos];
				}

				if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
				{
					CURVE_STRUCT curve_imsi = new CURVE_STRUCT();
					CurveCopy(curve_imsi, curve);

					curve_imsi.type &= ~EnumCurveType.BEZIER;
					curve_imsi.type |= EnumCurveType.LINE;
					curve_imsi.x[0] = curve.x[2];
					curve_imsi.y[0] = curve.y[2];

					if(pos == pos_s) 
					{
						block[pos_e] = curve_imsi;
					}
					else 
					{
						block[pos] = curve_imsi;
					}
				}
			}

			ClassStudioEditUndo.UndoSave_Selected(form, "Curve point to line");
			form.SetChangeFlag();

			objectSelect.SetCurveBlock(form, block);
			form.SelectListUpdateFirstItem();
			SelectedNodePrepare(form);
			form.Invalidate();
		}

		static void SetCurveMode(ref EnumCurveType mode, EnumCurveType flag)
		{
			mode &= ~(EnumCurveType.SMOOTH | EnumCurveType.SYMMETRICAL);
			mode |= flag;
		}

		public void ModeChangePublic(FormEditGraphic form, EnumCurveType mode, string title_undo)
		{
			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
			CURVE_STRUCT curve_n;
			int pos_s = 0, pos_e = 0;
			//Point p = new Point();
			int i;
			int pos_c;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos_c = (int)arraySelectNode[i];

				curve = (CURVE_STRUCT)block[pos_c];

				GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos_c);

				if(pos_c == pos_s) 
				{	// 시작 포인트 일 때
					if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
					{	// close curve
						if((curve_e.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							SetCurveMode(ref curve_e.type, mode);
							//block.SetBlock(&curve_e, pos_e);
							SetCurveMode(ref curve.type, mode);
							//block.SetBlock(&curve, pos_c);
						}

						curve_n = (CURVE_STRUCT)block[pos_c+1];
						if((curve_n.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							SetCurveMode(ref curve.type, mode);
							//block.SetBlock(&curve, pos_c);
						}
					}
				}
				else 
				{
					if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER)
					{
						SetCurveMode(ref curve.type, mode);
						//block.SetBlock(&curve, pos_c);
					}
					if(pos_c < pos_e) 
					{
						curve_n = (CURVE_STRUCT)block[pos_c+1];
						if((curve_n.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							SetCurveMode(ref curve.type, mode);
							//block.SetBlock(&curve, pos_c);
						}
					}
				}
			}

			ClassStudioEditUndo.UndoSave_Selected(form, title_undo);
			form.SetChangeFlag();
			objectSelect.SetCurveBlock(form, block);
			form.SelectListUpdateFirstItem();
			SelectedNodePrepare(form);
			form.Invalidate();
		}

		/// <summary>
		/// 선택된 노드들의 날카롭게 
		/// </summary>
		/// <returns>0은 여러종류, 1-날카롭게, 2-부드럽게,3-대칭</returns>
		public int GetCurveModeBySelectedNodes()
		{
			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			int i;
			int pos_c;
			int mode = -1;
			int m;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos_c = (int)arraySelectNode[i];

				curve = (CURVE_STRUCT)block[pos_c];

				if((curve.type & EnumCurveType.SMOOTH) > 0) 
				{
					m = 2;
				}
				else if((curve.type & EnumCurveType.SYMMETRICAL) > 0) 
				{
					m = 3;
				}
				else 
				{
					m = 1;
				}

				if(mode == -1)	// 처음노드
				{
					mode = m;
				}
				else 
				{
					if(mode != m) 
					{
						return 0;
					}
				}
			}

			return mode;
		}

		public bool IsPossibleCusp(FormEditGraphic form)
		{
			if(arraySelectNode.Count == 0)	return false;
			return true;
		}

		public void IdmCurvePointCusp(FormEditGraphic form)
		{
			if(!IsPossibleCusp(form))	return;
			ModeChangePublic(form, 0, "Curve point to cusp");
		}

		public bool IsPossibleSmooth(FormEditGraphic form)
		{
			if(arraySelectNode.Count == 0)	return false;
			return true;
		}

		public void IdmCurvePointSmooth(FormEditGraphic form)
		{
			if(!IsPossibleSmooth(form))	return;
			ModeChangePublic(form, EnumCurveType.SMOOTH, "Curve point to smooth");
		}

		public bool IsPossibleSymmetrical(FormEditGraphic form)
		{
			if(arraySelectNode.Count == 0)	return false;
			return true;
		}

		public void IdmCurvePointSymmetrical(FormEditGraphic form)
		{
			if(!IsPossibleSymmetrical(form))	return;
			ModeChangePublic(form, EnumCurveType.SYMMETRICAL, "Curve point to symmetrical");
		}

		public bool IsPossibleAutoClose(FormEditGraphic form)
		{
			if(arraySelectNode.Count != 1)	return false;

			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve_s, curve_e;
			int pos_s = 0, pos_e = 0;
			int pos = (int)arraySelectNode[0];

			if(block.Count <= 2)	return false;	// node의 개수가 너무적다

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

			if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)	return false;	// 이미 close 되어있다.

			return true;
		}

		public void IdmCurvePointAutoClose(FormEditGraphic form)
		{
			if(!IsPossibleAutoClose(form))	return;

			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
			int pos_s = 0, pos_e = 0;
			//Point p = new Point();
			int pos = (int)arraySelectNode[0];

			if(block.Count <= 2)	return;	// node의 개수가 너무적다

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos);

			if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE)	return;	// 이미 close 되어있다.

			curve = new CURVE_STRUCT();
			curve.type = EnumCurveType.LINE | EnumCurveType.CLOSE;
			curve.x[0] = curve_s.x[0];
			curve.y[0] = curve_s.y[0];

			block.Insert(pos_e+1, curve);

			ClassStudioEditUndo.UndoSave_Selected(form, "Curve point auto close");
			form.SetChangeFlag();

			objectSelect.SetCurveBlock(form, block);
			form.SelectListUpdateFirstItem();
			SelectedNodePrepare(form);

			form.Invalidate();
		}

		public bool IsPossibleBreakApart(FormEditGraphic form)
		{
			if(arraySelectNode.Count != 1)	return false;

			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve_s, curve_e;
			int pos_s = 0, pos_e = 0;
			int pos_c = (int)arraySelectNode[0];

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos_c);

			if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
			{	// node 가 close되어 있다.
			}
			else 
			{	// node가 open 되어 있다.
				if(pos_s == pos_c)	return false;
				if(pos_e == pos_c)	return false;
			}

			return true;
		}

		public void IdmCurvePointBreakApart(FormEditGraphic form)
		{
			if(!IsPossibleBreakApart(form))	return;

			ArrayList block = new ArrayList();

			objectSelect.GetCurveBlock(block);
	
			CURVE_STRUCT curve;
			CURVE_STRUCT curve_s, curve_e;
			int pos_s = 0, pos_e = 0;
			int i;
			int pos_c = (int)arraySelectNode[0];
			Point p = new Point();

			curve = (CURVE_STRUCT)block[pos_c];

			GetNodeStartEnd(block, out curve_s, out curve_e, out pos_s, out pos_e, pos_c);

			if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) // node 가 close되어 있다.
			{	
				if(pos_s == pos_c || pos_e == pos_c) 
				{	// start or end node
					curve_e.type &= ~EnumCurveType.CLOSE;
					//block.SetBlock(&curve_e, pos_e);

					ClassStudioEditUndo.UndoSave_Selected(form, "Curve point break apart");
					form.SetChangeFlag();

					objectSelect.SetCurveBlock(form, block);
					form.SelectListUpdateFirstItem();
					SelectedNodePrepare(form);
					form.Invalidate();
					return;		
				}
		
				ArrayList blockTemp = new ArrayList();
				int hap = pos_e-pos_s+1;
				int pos;

				GetCurveMainPoint(curve, ref p);
				curve = new CURVE_STRUCT();			// 새로 만들지 않으면 이래에서 if(..== EnumCurveType.START)에 걸린다.
				curve.type = EnumCurveType.START;
				curve.x[0] = p.X;
				curve.y[0] = p.Y;
				blockTemp.Add(curve);

				for(i = 0; i < hap; i++) 
				{
					pos = i+pos_c+1;
					pos %= hap;
					curve = (CURVE_STRUCT)block[pos+pos_s];
					if((curve.type & EnumCurveType.START) == EnumCurveType.START) {
					
					}
					else 
					{
						curve.type &= ~EnumCurveType.CLOSE;
						curve.type &= ~EnumCurveType.START;
						blockTemp.Add(curve);
					}
				}

				ClassStudioEditUndo.UndoSave_Selected(form, "Curve point break apart");
				form.SetChangeFlag();

				objectSelect.SetCurveBlock(form, blockTemp);
				form.SelectListUpdateFirstItem();

				arraySelectNode.Clear();
				SelectNodeAdd(blockTemp, 0);
				SelectedNodePrepare(form);

				form.Invalidate();
			}
			else 
			{	// node가 open 되어 있다.
				if(pos_s == pos_c)	return;
				if(pos_e == pos_c)	return;

				GetCurveMainPoint(curve, ref p);
				curve = new CURVE_STRUCT();
				curve.type = EnumCurveType.START;
				curve.x[0] = p.X;
				curve.y[0] = p.Y;
				block.Insert(pos_c+1, curve);

				ClassStudioEditUndo.UndoSave_Selected(form, "Curve point break apart");
				form.SetChangeFlag();

				objectSelect.SetCurveBlock(form, block);
				form.SelectListUpdateFirstItem();

				arraySelectNode.Clear();
				SelectNodeAdd(block, pos_c+1);

				SelectedNodePrepare(form);
				form.Invalidate();
				return;
			}
		}

		public bool IsPossibleJoin(FormEditGraphic form)
		{
			if(arraySelectNode.Count != 2) 
			{
				return false;
			}

			CURVE_STRUCT curve_s, curve_e;
			int pos_s = 0, pos_e = 0;
			int pos;
			int i;

			for(i = 0; i < arraySelectNode.Count; i++) 
			{
				pos = (int)arraySelectNode[i];
				GetNodeStartEnd(blockCurve, out curve_s, out curve_e, out pos_s, out pos_e, pos);
				if((curve_e.type & EnumCurveType.CLOSE) == EnumCurveType.CLOSE) 
				{
					return false;
				}
				if(pos == pos_s || pos == pos_e) 
				{

				}
				else 
				{
					return false;
				}
			}

			return true;
		}

		public void IdmCurvePointJoin(FormEditGraphic form)
		{
			if(!IsPossibleJoin(form))	return;

			ClassStudioEditUndo.UndoSave_Selected(form, "Curve point Join");
			form.SetChangeFlag();

			CURVE_STRUCT curve, curve_p, curve_new;
			CURVE_STRUCT curve_s1, curve_e1, curve_s2, curve_e2;
			int pos_s1 = 0, pos_e1 = 0, pos_s2 = 0, pos_e2 = 0;
			int pos1, pos2;
			Point p = new Point();
			int i;

			pos1 = (int)arraySelectNode[0];
			GetNodeStartEnd(blockCurve, out curve_s1, out curve_e1, out pos_s1, out pos_e1, pos1);
			pos2 = (int)arraySelectNode[1];
			GetNodeStartEnd(blockCurve, out curve_s2, out curve_e2, out pos_s2, out pos_e2, pos2);

			if(pos_s1 == pos_s2 && pos_e1 == pos_e2)	// 한Path의 시작과 끝을 선택했다.
			{	
				if(pos1 == pos_s1) 
				{	// 처음에 선택한 것이 시작점이다.	시작점 위치에 끝을 붙인다.
					if((curve_e1.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
					{
						curve_e1.type |= EnumCurveType.CLOSE;
						curve_e1.x[2] = curve_s1.x[0];
						curve_e1.y[2] = curve_s1.y[0];
						SetCurveMode(ref curve_e1.type, 0);	// 뾰족하게 만든다.
					}
					else 
					{
						curve_e1.type |= EnumCurveType.CLOSE;
						curve_e1.x[0] = curve_s1.x[0];
						curve_e1.y[0] = curve_s1.y[0];
						SetCurveMode(ref curve_e1.type, 0);	// 뾰족하게 만든다.
					}
				}
				else // 나중에 선택한 것이 시작점이다.  끝점에 시작점을 붙인다.
				{
					if((curve_e1.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
					{
						curve_e1.type |= EnumCurveType.CLOSE;
						curve_s1.x[0] = curve_e1.x[2];
						curve_s1.y[0] = curve_e1.y[2];
						SetCurveMode(ref curve_e1.type, 0);	// 뾰족하게 만든다.
					}
					else 
					{
						curve_e1.type |= EnumCurveType.CLOSE;
						curve_s1.x[0] = curve_e1.x[0];
						curve_s1.y[0] = curve_e1.y[0];
						SetCurveMode(ref curve_e1.type, 0);	// 뾰족하게 만든다.
					}
				}

				objectSelect.SetCurveBlock(form, blockCurve);
				form.SelectListUpdateFirstItem();
				arraySelectNode.Clear();
				SelectNodeAdd(blockCurve, pos_s1);
			}
			else									// 두개 Path에 걸쳐서 선택되었다.
			{	
				ArrayList blockTemp = new ArrayList();

				// 첫번째 곡선을 해석한다.

				if(pos1 == pos_s1)	// 역 방향으로 진행해야 한다.
				{	
					curve = (CURVE_STRUCT)blockCurve[pos_e1];
					GetCurveMainPoint(curve, ref p);

					curve = new CURVE_STRUCT();
					curve.type = EnumCurveType.START;
					curve.x[0] = p.X;
					curve.y[0] = p.Y;
					blockTemp.Add(curve);

					for(i = pos_e1; i > pos_s1; i--) 
					{
						curve = (CURVE_STRUCT)blockCurve[i];
						curve_p = (CURVE_STRUCT)blockCurve[i-1];
						GetCurveMainPoint(curve_p, ref p);

						if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							curve_new = new CURVE_STRUCT();
							curve_new.type = curve.type;
							curve_new.x[0] = curve.x[1];
							curve_new.y[0] = curve.y[1];
							curve_new.x[1] = curve.x[0];
							curve_new.y[1] = curve.y[0];
							curve_new.x[2] = p.X;
							curve_new.y[2] = p.Y;
						}
						else 
						{
							curve_new = new CURVE_STRUCT();
							curve_new.type = curve.type;
							curve_new.x[0] = p.X;
							curve_new.y[0] = p.Y;
						}

						if(i == pos_s1+1)
							SetCurveMode(ref curve_new.type, 0);	// 뾰족하게 만든다.

						blockTemp.Add(curve_new);
					}
				}
				else 
				{
					for(i = pos_s1; i <= pos_e1; i++) 
					{
						curve = (CURVE_STRUCT)blockCurve[i];
						curve_new = new CURVE_STRUCT();
						CurveCopy(curve_new, curve);
						if(i == pos_e1)
							SetCurveMode(ref curve_new.type, 0);	// 뾰족하게 만든다.
						blockTemp.Add(curve_new);
					}
				}

				// 두번째 곡선을 해석한다.

				if(pos2 != pos_s2)	// 역 방향으로 진행해야 한다.
				{	
					for(i = pos_e2; i > pos_s2; i--) 
					{
						curve = (CURVE_STRUCT)blockCurve[i];
						curve_p = (CURVE_STRUCT)blockCurve[i-1];
						GetCurveMainPoint(curve_p, ref p);

						if((curve.type & EnumCurveType.BEZIER) == EnumCurveType.BEZIER) 
						{
							curve_new = new CURVE_STRUCT();
							curve_new.type = curve.type;
							curve_new.x[0] = curve.x[1];
							curve_new.y[0] = curve.y[1];
							curve_new.x[1] = curve.x[0];
							curve_new.y[1] = curve.y[0];
							curve_new.x[2] = p.X;
							curve_new.y[2] = p.Y;
						}
						else 
						{
							curve_new = new CURVE_STRUCT();
							curve_new.type = curve.type;
							curve_new.x[0] = p.X;
							curve_new.y[0] = p.Y;
						}

						blockTemp.Add(curve_new);
					}
				}
				else 
				{
					for(i = pos_s2; i <= pos_e2; i++) 
					{
						curve = (CURVE_STRUCT)blockCurve[i];
						if((curve.type & EnumCurveType.START) == EnumCurveType.START)
						{}
						else 
						{
							curve_new = new CURVE_STRUCT();
							CurveCopy(curve_new, curve);
							blockTemp.Add(curve_new);
						}
					}
				}

				// 나머지 Path를 이어서 등록한다.
				for(i = 0; i < blockCurve.Count; i++) 
				{
					if(i >= pos_s1 && i <= pos_e1)	continue;	// 첫번째 노드에 속해있는 path
					if(i >= pos_s2 && i <= pos_e2)	continue;	// 두번째 노드에 속해있는 path

					CURVE_STRUCT curve_s3, curve_e3;
					int pos_s3 = 0, pos_e3 = 0;

					GetNodeStartEnd(blockCurve, out curve_s3, out curve_e3, out pos_s3, out pos_e3, i);

					for(int j = pos_s3; j <= pos_e3; j++) 
					{
						curve = (CURVE_STRUCT)Tools.CopyObject(blockCurve[j]);
						blockTemp.Add(curve);
					}

					i = pos_e3;
				}


				objectSelect.SetCurveBlock(form, blockTemp);
				form.SelectListUpdateFirstItem();
				arraySelectNode.Clear();
				//SelectNodeAdd(&blockTemp, pos_s1);
			}

			SelectedNodePrepare(form);
			form.Invalidate();
		}

		public void SelectNodeClear()
		{
			arraySelectNode.Clear();
		}

		public override void DoubleClick(FormEditGraphic form)
		{
			ClassEditProperty.OnUserPropertyClick(form);
		}
	}
}


