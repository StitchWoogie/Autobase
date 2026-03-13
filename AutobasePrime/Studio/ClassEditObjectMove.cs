using System;
using System.Drawing;
using System.Windows.Forms;
using NetTools.OldDefine;
using GraphicModule;
using System.Collections;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for ClassEditObjectMove.
	/// </summary>
	public class ClassEditObjectMove : ClassMainTool
	{
		public bool bMouseLeftFlag = false;
		bool bMouseRightFlag = false;

		int nOldMX, nOldMY;

		int nStartX, nStartY;

		object oMouseSelectedObject;

		EnumSelect  cSelectConner;	// 현재 마우스로 이동중인 코너는 ?

		public ClassEditObjectMove()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static bool RecurseMouseCheckZone(FormEditGraphic form, ObjectPublicGroupLayer gl, MouseEventArgs e, out object obj_result, bool check_only_selected)
        {
            int i;
            ObjectExpand obj;
            obj_result = null;

            for (i = gl.GetObjectHap() - 1; i >= 0; i--)
            {
                obj = (ObjectExpand)gl.GetPoint(i);
                if (obj.objGeneral.bOnStudioLocked) continue;	// LOCK되어 있다.
                if (!obj.objGeneral.bOnStudioVisible) continue;   // 보이지 않는 오브젝트는 검사할 필요가 없다.

                if (obj.enumObjectType == EnumObjectType.Layer)
                {
                    if (RecurseMouseCheckZone(form, (ObjectLayer)obj, e, out obj_result, check_only_selected)) return true;
                }
                else
                {
                    if (check_only_selected)    // 선택되어 있는 오브젝트만 검사하는 경우
                    {
                        if (!obj.bOnStudioSelected) continue;
                    }

                    if (CheckZone(form, (ObjectExpand)obj, i, e))
                    {
                        obj_result = obj;
                        return true;
                    }
                }
            }

            return false;
        }

		public static void MainToolLButtonDownMoveWithKeyShift(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

            object obj;

            if (RecurseMouseCheckZone(form, work.obj.groupRoot, e, out obj, false))
            {
                ObjectType type = (ObjectType)obj;

                if (type.bOnStudioSelected)
                {
                    form.SelectListDel(work, type);
                }
                else
                {
                    // 이전에 선택되어 있는것이 그룹속에 포함된 것이라면
                    bool group_child = false;
                    
                    for (int i = 0; i < work.nSelectCount; i++)
                    {
                        if (((ObjectType)work.selectList[i].obj).parentGroupLayer.enumObjectType == EnumObjectType.Group    // 부모가 그룹이고
                            && ((ObjectType)work.selectList[i].obj).parentGroupLayer != work.obj.groupRoot                  // 루트 멤버가 아니고
                            && ((ObjectType)work.selectList[i].obj).parentGroupLayer != type.parentGroupLayer)              // 현재 선택하는 오브젝트와 같은 부모가 아닌경우
                        {
                            group_child = true;
                        }
                    }

                    if (group_child) form.SelectListClear(work);

                    form.SelectListAdd(work, type);
                }
                form.DisplayAfterSelectedChanged();
                                
                form.Invalidate();

            }
			return;
		}

		public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
		{
			if(bMouseLeftFlag || bMouseRightFlag)	return;

			form.SelectNodeClear();	// 이동 시 노드 편집은 무효화 된다.

			// Move 도구일때만 다중 선택
			if (form.formFrame.enumMainTool == EnumMainTool.ARROW && (Control.ModifierKeys & Keys.Shift) == Keys.Shift) 
			{
				MainToolLButtonDownMoveWithKeyShift(form, e);
				return;
			}

			WORK_MODULE_STRUCT work = form.workThis;
			Graphics g;
			int mx, my;

			mx = e.X;
			my = e.Y;

			g = form.CreateGraphics();

            object obj;
            object selectedObj;

            // X 선택된것이 있을 때는 순서에 상관없이 선택된것이 우선한다.
			// 20260311 PSU PowerPointer처럼 상위 오브젝트가 클릭되도록 변경. 
            // 선택된 오브젝트가 있어도, 클릭 위치에 다른 최상단 오브젝트가 있으면 그 오브젝트를 우선 선택한다.
            // 단, 선택된 오브젝트의 핸들을 클릭했거나 해당 오브젝트가 최상단이면 기존 선택을 유지한다.
            if (work.nSelectCount > 0) 
			{
                if (RecurseMouseCheckZone(form, work.obj.groupRoot, e, out selectedObj, true))
                {
                    bool selectedCorner = CheckCornner(form, (ObjectExpand)selectedObj, e);
                    bool hasTopObject = RecurseMouseCheckZone(form, work.obj.groupRoot, e, out obj, false);

					if (selectedCorner || !hasTopObject || obj == selectedObj)
					{
						form.Capture = true;
						bMouseLeftFlag = true;
						oMouseSelectedObject = obj;
						nStartX = nOldMX = mx;
						nStartY = nOldMY = my;

                        if (selectedCorner)
                        {

                        }
                        else
                        {
                            cSelectConner = EnumSelect.SELECT_MID;

                            // form.DoDragDrop("DragAndDrop Object Move", DragDropEffects.Copy);   // Drop 이 있어서 그런지 Drop후 가 이상하다.
                        }

                        form.ObjectNotAll(g, work);
                        SelectListGetZone(work);

                        SaveSelectedList(work);

                        return;
                    }
                }
			}

			// 하나가 마우스영역에 포함되면 그것을 선택하고 돌아간다.

            if (RecurseMouseCheckZone(form, work.obj.groupRoot, e, out obj, false))
            {
                if (work.nSelectCount > 0)
                {
                    form.ObjectNotAll(g, work);
                    form.SelectListClear(work);
                }

                form.SelectListAdd(work, obj);

                form.DisplayAfterSelectedChanged();
                form.SelectNodeClear();

                form.Invalidate();

                return;
            }

			if(work.nSelectCount > 0) 
			{
				form.ObjectNotAll(g, work);
				form.SelectListClear(work);

                form.DisplayAfterSelectedChanged();
				form.Invalidate();	// 바로 밑에서 return이 되면 무효화가 안된다.
			}

			// Move도구일때만 다중 선택을 할수 있다. PointMove에서도 이함수를 사용한다.
			if(form.formFrame.enumMainTool != EnumMainTool.ARROW)	return;

			// 여러 obj를 선택하는 모드로 들어간다.
			form.Capture = true;
			bMouseLeftFlag = true;
			bMultiSelectFlag = true;
			nStartX = nOldMX = mx;
			nStartY = nOldMY = my;

			form.Invalidate();
	
			SaveSelectedList(work);

			return;
		}

		//------------------------------------------------------------------------------
		//	마우스가 움직일 때마다 마우스 모양을 바꾸어 준다.
		//------------------------------------------------------------------------------

		void WorkCursorStatus(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;
			int mx, my;

			mx = e.X;
			my = e.Y;

            object obj;

            if (RecurseMouseCheckZone(form, work.obj.groupRoot, e, out obj, true)) {

                if (CheckCornner(form, (ObjectExpand)obj, e)) 
				{

				}
				else 
				{
					cSelectConner = EnumSelect.SELECT_MID;
				}

				switch(cSelectConner) 
				{
					case EnumSelect.SELECT_MID:	
						form.Cursor = Cursors.SizeAll;
						break;
					case EnumSelect.SELECT_MID_LEFT:	
					case EnumSelect.SELECT_MID_RIGHT:	
						form.Cursor = Cursors.SizeWE;
						break;
					case EnumSelect.SELECT_MID_TOP:	
					case EnumSelect.SELECT_MID_BOTTOM:	
						form.Cursor = Cursors.SizeNS;
						break;
					case EnumSelect.SELECT_LEFT_TOP:	
					case EnumSelect.SELECT_RIGHT_BOTTOM: 
	                    {
                            RECT r = new RECT();
                            ((ObjectExpand)obj).GetZone(ref r);
						    form.Cursor = Cursors.SizeNWSE;
                        }
						break;
					case EnumSelect.SELECT_RIGHT_TOP:	
					case EnumSelect.SELECT_LEFT_BOTTOM:	
						form.Cursor = Cursors.SizeNESW;
						break;

					default:
						form.Cursor = Cursors.Arrow;
						break;
				}
				return;
			}

			form.Cursor = Cursors.Arrow;
		}

        // 10 부터는 Shift로 바뀌었다.
		// 이동중 Shift키가 눌러졌을때는 큰것을 기준으로 수평,수직으로 이동할 수 있도록 한다.
		public static void CalcMoveOnPressedShift(int startx, int starty, ref int x, ref int y)
		{
			//if ((Control.ModifierKeys & Keys.Control) != Keys.Control)	return;
            if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift) return;

			int gabx = Math.Abs(startx-x);
			int gaby = Math.Abs(starty-y);

			if(gabx > gaby)
			{
				y = starty;
			}
			else 
			{
				x = startx;
			}
		}

        // 이동중 키가 눌러졌을 때는 큰것을 기준으로 수평,수직으로 이동할 수 있도록 한다.
        void CalcMoveOnPressedControl(int startx, int starty, ref int x, ref int y)
        {
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)	return;

            // 사각 코너를 선택했을 때만 비율적으로 조절한다.
            if (cSelectConner == EnumSelect.SELECT_LEFT_TOP || cSelectConner == EnumSelect.SELECT_LEFT_BOTTOM ||
                cSelectConner == EnumSelect.SELECT_RIGHT_BOTTOM || cSelectConner == EnumSelect.SELECT_RIGHT_TOP)
            {
                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                ((ObjectExpand)oMouseSelectedObject).GetZone(ref x1, ref y1, ref x2, ref y2);

                x1 = ((ObjectExpand)oMouseSelectedObject).GetViewPosX(x1);
                y1 = ((ObjectExpand)oMouseSelectedObject).GetViewPosY(y1);
                x2 = ((ObjectExpand)oMouseSelectedObject).GetViewPosX(x2);
                y2 = ((ObjectExpand)oMouseSelectedObject).GetViewPosY(y2);

                int width = Math.Abs(x2 - x1)+1;
                int height = Math.Abs(y2 - y1)+1;

                int gabx = Math.Abs(startx - x);
                int gaby = Math.Abs(starty - y);

                int centerx, centery;

                if (cSelectConner == EnumSelect.SELECT_LEFT_TOP || cSelectConner == EnumSelect.SELECT_LEFT_BOTTOM)
                {
                    if (x1 < x2)    centerx = x2;
                    else            centerx = x1;
                }
                else 
                {
                    if (x1 < x2)    centerx = x1;
                    else            centerx = x2;
                }

                if (cSelectConner == EnumSelect.SELECT_LEFT_TOP || cSelectConner == EnumSelect.SELECT_RIGHT_TOP)
                {
                    if (y1 < y2)    centery = y2;
                    else            centery = y1;
                }
                else
                {
                    if (y1 < y2)    centery = y1;
                    else            centery = y2;
                }

                gabx = Math.Abs(centerx - x)+1;
                gaby = Math.Abs(centery - y)+1;

                double ratex = (double)gabx / width;
                double ratey = (double)gaby / height;

                if (ratex > ratey)
                {
                    if(y < centery)
                        y = centery - (int)(ratex * height);
                    else
                        y = centery + (int)(ratex * height);
                }
                else
                {
                    if (x < centerx)
                        x = centerx - (int)(ratey * width);
                    else
                        x = centerx + (int)(ratey * width);
                }
            }
        }

		public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
		{
			if(bMouseLeftFlag == false && bMouseRightFlag == false)	
			{
				WorkCursorStatus(form, e);	
				return;
			}

			int gabx, gaby;
	
			WORK_MODULE_STRUCT work = form.workThis;
			int  mx, my;

			mx = e.X;
			my = e.Y;

			if(bMultiSelectFlag == true)
			{
				nOldMX = mx;
				nOldMY = my;
				form.Invalidate();
				return;
			}

			int moveX1 = 0;
			int moveY1 = 0;
			int moveX2 = 0;
			int moveY2 = 0;

			CalcMoveOnPressedShift(nStartX, nStartY, ref mx, ref my);
            CalcMoveOnPressedControl(nStartX, nStartY, ref mx, ref my);

			gabx = mx-nStartX;
			gaby = my-nStartY;

			CalcMovePointerFitToGuideLine(form, work, ref gabx, ref gaby);

			if(cSelectConner == EnumSelect.SELECT_MID) 
			{
				moveX1 += gabx;
				moveY1 += gaby;
				moveX2 += gabx;
				moveY2 += gaby;
			}
			else if(cSelectConner == EnumSelect.SELECT_LEFT_TOP) 
			{
				moveX1 += gabx;
				moveY1 += gaby;
			}
			else if(cSelectConner == EnumSelect.SELECT_RIGHT_TOP) 
			{
				moveX2 += gabx;
				moveY1 += gaby;
			}
			else if(cSelectConner == EnumSelect.SELECT_LEFT_BOTTOM) 
			{
				moveX1 += gabx;
				moveY2 += gaby;
			}
			else if(cSelectConner == EnumSelect.SELECT_RIGHT_BOTTOM) 
			{
				moveX2 += gabx;
				moveY2 += gaby;
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_LEFT) 
			{
				moveX1 += gabx;
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_RIGHT) 
			{
				moveX2 += gabx;
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_TOP) 
			{
				moveY1 += gaby;
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_BOTTOM) 
			{
				moveY2 += gaby;
			}
			else {}

			SelectedObjectListMove(form, work, moveX1, moveY1, moveX2, moveY2);

			nOldMX = mx;
			nOldMY = my;

            form.Invalidate();
		}

        void CopyByControlKey(FormEditGraphic form)
        {
            WORK_MODULE_STRUCT work = form.workThis;
            object obj;
            object block;

            ArrayList array = new ArrayList();

            for (int i = 0; i < work.nSelectCount; i++)
            {
                obj = work.selectList[i].obj;
                block = ClassStudioEdit.CreateSameObject(form, obj, 0, 0);
                array.Add(block);
            }

            ObjectPublicGroupLayer parent = ClassStudioEdit.GetTargetRoot(form);

            ClassStudioEditUndo.UndoSave_All(form, "Object Copy");

            for (int l = 0; l < array.Count; l++)
            {
                block = (object)array[l];
                obj = block;
                ((ObjectPublic)(work.selectList[l].obj)).bOnStudioSelected = false;
                parent.AddObject(obj);

                work.selectList[l].obj = obj;
                ClassEditInsert.RecurseMakePos(work.obj.groupRoot, obj, ref work.selectList[l].opos);   // parent.AddObject를 먼저하고 Pos를 얻어야 얻을 수 있다.
            }

            // 2014-1-8 복사후 회전을 적용하면 원본에 적용되어서 아래 두개를 추가했다.
            work.selectListOnlyParent = form.SelectListMakeOnlyParent();
            work.selectListOnlyChild = form.SelectListMakeOnlyChild();

            work.obj.SetOpticRate(work.obj.nOpticRate);
            work.obj.SetBasePoint(-work.nScrollHorPos, -work.nScrollVerPos);
            work.obj.SetZoneAtPercent100();
            UpdateSelectedObject(form, work);

            form.SetChangeFlag();
            
            ClassEditProperty.SelectChanged(form);
            Layer.FormLayer.InvalidateDisplay();

            form.Invalidate();
        }

		public override void MouseUpLeft(FormEditGraphic form, MouseEventArgs e)
		{
			if(bMouseLeftFlag == false)	return;

			form.Capture = false;
			bMouseLeftFlag = false;

			WORK_MODULE_STRUCT work = form.workThis;

			if(bMultiSelectFlag) 
			{
				bMultiSelectFlag = false;

                if (RecurseSelectIfIncluded(form, work.obj.groupRoot, nStartX, nStartY, nOldMX, nOldMY) > 0)
                {
                    form.DisplayAfterSelectedChanged();
                }

				form.Invalidate();
			
				return;
			}

			if(nStartX == nOldMX && nStartY == nOldMY) 
			{	// 변화가 없었다.
				form.Invalidate();
			}
			else 
			{
                if (cSelectConner == EnumSelect.SELECT_MID && (Control.ModifierKeys & Keys.Control) > 0)
                {
                    CopyByControlKey(form);
                }
                else
                {
                    ClassStudioEditUndo.UndoSave_Selected(form, "Move Object");
                    UpdateSelectedObject(form, work);
                    form.SelectListUpdate();	// Single 텍스트는 다시 좌표를 정리해야 한다.
                    ClassEditProperty.ObjectPosSizeChanged(form);
                    form.Invalidate();
                    Layer.FormLayer.InvalidateDisplay();
                }
			}
		}

		public override void Paint(FormEditGraphic form, Graphics g)
		{
			if(bMultiSelectFlag)
				DrawMutiSelectZone(g);

            // Control키를 누른 상태에서는 복사 그림을 그려준다.
            if (cSelectConner == EnumSelect.SELECT_MID && (Control.ModifierKeys & Keys.Control) > 0 && bMouseLeftFlag)
            {
                Point p = form.PointToClient(Control.MousePosition);

                int x = p.X+15;
                int y = p.Y+15;
                
                g.DrawLine(Pens.Black, x - 7, y, x + 7, y);
                g.DrawLine(Pens.Black, x, y-7, x, y+7);
            }
		}

		bool bMultiSelectFlag = false;	// 여러개의 Object를 선택하는 중.

		void DrawMutiSelectZone(Graphics g)
		{
			int x1, y1, x2, y2;

			x1 = nOldMX;
			y1 = nOldMY;
			x2 = nStartX;
			y2 = nStartY;

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			Pen pen = new Pen(Color.FromArgb(128, 255, 255, 255), 1);
			g.DrawRectangle(pen, x1, y1, x2-x1, y2-y1);

			pen = new Pen(Color.Black);
			g.DrawRectangle(pen, x1-1, y1-1, x2-x1+2, y2-y1+2);
			g.DrawRectangle(pen, x1+1, y1+1, x2-x1-2, y2-y1-2);

		}

        static void RotateAngle(int x1, int y1, int x2, int y2, ref int mx, ref int my, float angle)
        {
            int cx, cy;

            int w = Math.Abs(x2 - x1) + 1;
            int h = Math.Abs(y2 - y1) + 1;

            cx = (x1 > x2) ? x2 + w / 2 : x1 + w / 2;
            cy = (y1 > y2) ? y2 + h / 2 : y1 + h / 2;

            float degree = (float)MathLib.MathGradientToDegree(mx - cx, my - cy);
            degree += angle;

            double r = MathLib.MathGetHypotenuse(mx - cx, my - cy);

            MathLib.MathGetEllipsePoint(cx, cy, (float)r, (float)r, (float)degree, out mx, out my); 
        }

		//------------------------------------------------------------------------------
		//	마우스가 해당 오브젝트의 영역속에 있는가를 검사한다.
		//------------------------------------------------------------------------------

        static bool CheckZone(FormEditGraphic form, ObjectExpand obj, int pos, System.Windows.Forms.MouseEventArgs e)
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int mx, my;

            mx = e.X;
            my = e.Y;

            obj.GetZone(ref x1, ref y1, ref x2, ref y2);

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            x1 = obj.GetViewPosX(x1);
            y1 = obj.GetViewPosY(y1);
            x2 = obj.GetViewPosX(x2);
            y2 = obj.GetViewPosY(y2);

            // 회전된 경우 마우스 포인트만 회전된 각 만큼 꺼꾸로 돌린다. 
            if (obj.RotationAngle != 0)
            {
                RotateAngle(x1, y1, x2, y2, ref mx, ref my, obj.RotationAngle);
            }

            if (mx >= x1 - 3 && mx <= x2 + 3 && my >= y1 - 3 && my <= y2 + 3) return true;

            return false;
        }

		bool CheckCornner(FormEditGraphic form, ObjectExpand obj, System.Windows.Forms.MouseEventArgs e)
		{
			int mx, my;
			int x1=0, y1=0, x2=0, y2=0;

			obj.GetZone(ref x1, ref y1, ref x2, ref y2);

			EnumNotType not_type = obj.GetNotType();

			mx = e.X;
			my = e.Y;

			x1 = obj.GetViewPosX(x1);
            y1 = obj.GetViewPosY(y1);
            x2 = obj.GetViewPosX(x2);
            y2 = obj.GetViewPosY(y2);

			int width = Math.Abs(x2-x1);
			int height = Math.Abs(y2-y1);
			int smallx = x1 < x2 ? x1 : x2;	// smallx
			int smally = y1 < y2 ? y1 : y2;	// smally

            // 회전된 경우 마우스 포인트만 회전된 각 만큼 꺼꾸로 돌린다. 
            if (obj.RotationAngle != 0)
            {
                RotateAngle(x1, y1, x2, y2, ref mx, ref my, obj.RotationAngle);
            }

			if(not_type == EnumNotType.NOT_TYPE_LINE)	// 라인일때는 두군데만 체크하여야 한다. 다른곳을 체크하면 수평수직선일때는 다른 제어점이 선택될 수 있다.
			{
				if(mx >= x1-3 && my >= y1-3 && mx <= x1+3 && my <= y1+3) 
				{
					cSelectConner = EnumSelect.SELECT_LEFT_TOP;
				}
				else if(mx >= x2-3 && my >= y2-3 && mx <= x2+3 && my <= y2+3) 
				{
					cSelectConner = EnumSelect.SELECT_RIGHT_BOTTOM;
				}
				else 
				{
					return false;
				}
			}
			else 
			{
				if(mx >= x1-3 && my >= y1-3 && mx <= x1+3 && my <= y1+3) 
				{
					cSelectConner = EnumSelect.SELECT_LEFT_TOP;
				}
				else if(mx >= x2-3 && my >= y1-3 && mx <= x2+3 && my <= y1+3) 
				{
					cSelectConner = EnumSelect.SELECT_RIGHT_TOP;
				}
				else if(mx >= x1-3 && my >= y2-3 && mx <= x1+3 && my <= y2+3) 
				{
					cSelectConner = EnumSelect.SELECT_LEFT_BOTTOM;
				}
				else if(mx >= x2-3 && my >= y2-3 && mx <= x2+3 && my <= y2+3) 
				{
					cSelectConner = EnumSelect.SELECT_RIGHT_BOTTOM;
				}
				else if(mx >= x1-3 && my >= smally+(height)/2-3 && mx <= x1+3 && my <= smally+(height)/2+3) 
				{
					cSelectConner = EnumSelect.SELECT_MID_LEFT;
				}
				else if(mx >= x2-3 && my >= smally+(height)/2-3 && mx <= x2+3 && my <= smally+(height)/2+3) 
				{
					cSelectConner = EnumSelect.SELECT_MID_RIGHT;
				}
				else if(mx >= smallx+(width)/2-3 && my >= y1-3 && mx <= smallx+(width)/2+3 && my <= y1+3) 
				{
					cSelectConner = EnumSelect.SELECT_MID_TOP;
				}
				else if(mx >= smallx+(width)/2-3 && my >= y2-3 && mx <= smallx+(width)/2+3 && my <= y2+3) 
				{
					cSelectConner = EnumSelect.SELECT_MID_BOTTOM;
				}
				else 
				{
					return false;
				}
			}

			return true;
		}

		public static void SelectListGetZone(WORK_MODULE_STRUCT work)
		{
			int i;

			for(i = 0; i < work.nSelectCount; i++) 
			{
				//list = work.selectList[i]; 이것을 사용하면 복사가 되어서 얻은값이 소용없다. struct
                ((ObjectExpand)work.selectList[i].obj).GetZone(ref work.selectList[i].x1, ref work.selectList[i].y1, ref work.selectList[i].x2, ref work.selectList[i].y2);
			}
		}

		void SaveSelectedList(WORK_MODULE_STRUCT work)
		{
			if(work.selectListOld != null) 
			{
				work.selectListOld = null;
			}

			if(work.nSelectCount == 0)	return;

			work.selectListOld = new SELECT_LIST[work.nSelectCount];

			if(work.selectListOld == null)	return;		// 메모리를 할당할 수 없다.

			int i;

			for(i = 0; i < work.nSelectCount; i++) 
			{
				work.selectListOld[i].x1 = work.selectList[i].x1;
				work.selectListOld[i].y1 = work.selectList[i].y1;
				work.selectListOld[i].x2 = work.selectList[i].x2;
				work.selectListOld[i].y2 = work.selectList[i].y2;
                work.selectListOld[i].opos = work.selectList[i].opos;
                work.selectListOld[i].obj = work.selectList[i].obj;
			}
		}

		void SelectedObjectListMove(FormEditGraphic form, WORK_MODULE_STRUCT work, int gx1, int gy1, int gx2, int gy2)
		{
			int i;

			for(i = 0; i < work.nSelectCount; i++) 
			{
				work.selectList[i].x1 = work.selectListOld[i].x1+gx1;
				work.selectList[i].y1 = work.selectListOld[i].y1+gy1;
				work.selectList[i].x2 = work.selectListOld[i].x2+gx2;
				work.selectList[i].y2 = work.selectListOld[i].y2+gy2;
			}

			form.DrawSelectedObjectSize(work);

			
		}

        int RecurseSelectIfIncluded(FormEditGraphic form, ObjectPublicGroupLayer gl, int px1, int py1, int px2, int py2)
        {
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int count = 0;
            int l;

            if (px1 > px2) Tools.Temp(ref px1, ref px2);
            if (py1 > py2) Tools.Temp(ref py1, ref py2);

            for (l = 0; l < gl.GetObjectHap(); l++)
            {
                ObjectExpand type = (ObjectExpand)gl.GetPoint(l);

                if (type.objGeneral.bOnStudioLocked) continue;	    // LOCK되어 있다.
                if (!type.objGeneral.bOnStudioVisible) continue;	// 보이지 않는 오브젝트

                if (type.enumObjectType == EnumObjectType.Layer)
                {
                    count += RecurseSelectIfIncluded(form, (ObjectPublicGroupLayer)type, px1, py1, px2, py2);
                }
                else
                {
                    type.GetZone(ref x1, ref y1, ref x2, ref y2);

                    x1 = type.GetViewPosX(x1);
                    y1 = type.GetViewPosY(y1);
                    x2 = type.GetViewPosX(x2);
                    y2 = type.GetViewPosY(y2);

                    if (x1 > x2) Tools.Temp(ref x1, ref x2);
                    if (y1 > y2) Tools.Temp(ref y1, ref y2);

                    if (x1 >= px1 && y1 >= py1 && x2 <= px2 && y2 <= py2)
                    {
                        form.SelectListAdd(form.workThis, type);
                        count++;
                    }
                }
            }

            return count;
        }

		//----------------------------------------------------------------------------------------
		//	현재 선택되어 있는 object의 좌표를 마우스가 이동된 만큼 갱신한다.
		//----------------------------------------------------------------------------------------

		public static void UpdateSelectedObject(FormEditGraphic form, WORK_MODULE_STRUCT work)
		{
			int i;
			SELECT_LIST list;
	
			for(i = 0; i < work.nSelectCount; i++) 
			{
				list = work.selectList[i];

                ((ObjectExpand)list.obj).UpdateZone(form, list.x1, list.y1, list.x2, list.y2);

                ((ObjectExpand)list.obj).previewOnStudio = null;
			}

			work.obj.SetZoneAtPercent100();

			form.SetChangeFlag();
		}

		public void CalcMovePointerFitToGuideLine(FormEditGraphic form, WORK_MODULE_STRUCT work, ref int gabx, ref int gaby)
		{
            if (AutoLib.ConfigStudio.bGuideLineFit == false) 
			{
				gabx = form.GetPictureSize(gabx);
				gaby = form.GetPictureSize(gaby);
				return;
			}

			int ox = 0, oy = 0;
			int i;

			if(cSelectConner == EnumSelect.SELECT_MID) 
			{
				ox = work.selectListOld[0].x1;
				oy = work.selectListOld[0].y1;

				for(i = 0; i < work.nSelectCount; i++) 
				{
					if(work.selectListOld[i].x1 < ox)	ox = work.selectListOld[i].x1;
					if(work.selectListOld[i].y1 < oy)	oy = work.selectListOld[i].y1;
					if(work.selectListOld[i].x2 < ox)	ox = work.selectListOld[i].x2;
					if(work.selectListOld[i].y2 < oy)	oy = work.selectListOld[i].y2;
				}

				form.ConvertX(work, ox, ref gabx);
				form.ConvertY(work, oy, ref gaby);
			}
			else if(cSelectConner == EnumSelect.SELECT_LEFT_TOP) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
					if(work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						ox = work.selectListOld[i].x1;
						oy = work.selectListOld[i].y1;
						form.ConvertX(work, ox, ref gabx);
						form.ConvertY(work, oy, ref gaby);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_RIGHT_TOP) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						ox = work.selectListOld[i].x2;
						oy = work.selectListOld[i].y1;
						form.ConvertX(work, ox, ref gabx);
						form.ConvertY(work, oy, ref gaby);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_LEFT_BOTTOM) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						ox = work.selectListOld[i].x1;
						oy = work.selectListOld[i].y2;
						form.ConvertX(work, ox, ref gabx);
						form.ConvertY(work, oy, ref gaby);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_RIGHT_BOTTOM) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						ox = work.selectListOld[i].x2;
						oy = work.selectListOld[i].y2;
						form.ConvertX(work, ox, ref gabx);
						form.ConvertY(work, oy, ref gaby);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_LEFT) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						ox = work.selectListOld[i].x1;
						form.ConvertX(work, ox, ref gabx);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_RIGHT) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						ox = work.selectListOld[i].x2;
						form.ConvertX(work, ox, ref gabx);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_TOP) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						oy = work.selectListOld[i].y1;
						form.ConvertY(work, oy, ref gaby);
						break;
					}
				}		
			}
			else if(cSelectConner == EnumSelect.SELECT_MID_BOTTOM) 
			{
				for(i = 0; i < work.nSelectCount; i++) 
				{
                    if (work.selectListOld[i].obj == oMouseSelectedObject) 
					{
						oy = work.selectListOld[i].y2;
						form.ConvertY(work, oy, ref gaby);
						break;
					}
				}		
			}
			else {}
		}

		public override void MouseDownRight(FormEditGraphic form, MouseEventArgs e)
		{
			WORK_MODULE_STRUCT work = form.workThis;

			form.contextMenuStripMove.Show(form, new Point(e.X, e.Y));
		}

		public override void DoubleClick(FormEditGraphic form)
		{
			ClassEditProperty.OnUserPropertyClick(form);
		}

		
	}
}
