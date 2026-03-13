using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using System.Collections;
using NetTools.OldDefine;
using System.Reflection;

namespace Studio.Layer
{
    public partial class FormLayer : Form
    {
        static ObjectGroup rootGroup = null;
        static public FormLayer formThis = null;
        static FormEditGraphic formWork = null;

        static bool bReverseLayer = true;            // photo shop 처럼 레이어를 꺼꾸로 디스프레이 한다.

        public FormLayer()
        {
            InitializeComponent();

            // 20250728 PSU
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            InitializePanelDraw();  // 20250728 PSU
        }

        private void InitializePanelDraw()
        {
            // panelDraw가 Panel이라면
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panelDraw, new object[] { true });
        }

        int fonty = 0;
        int gaby = 0;

        void DrawPlusMinus(Graphics g, Rectangle r, bool flag)
        {
            int cx, cy;

            r.Width--;
            r.Height--;

            cx = r.Left + r.Width / 2;
            cy = r.Top  + r.Height / 2;

            DrawClass.grect(g, cx - 4, cy - 4, cx + 4, cy + 4, Color.DarkGray);

            DrawClass.gline(g, cx-2, cy, cx+2, cy, Color.Black);

            if(flag == false)
                DrawClass.gline(g, cx, cy-2, cx, cy+2, Color.Black);
        }

        int RecurseGetListCount(ObjectPublicGroupLayer group)
        {
            int count = 0;

            ObjectType type;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectType)group.GetPoint(i);

                count++;

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    if (((ObjectPublicGroupLayer)type).bOnStudioGroupOpen)
                    {
                        count += RecurseGetListCount((ObjectPublicGroupLayer)type);
                    }
                }
            }

            return count;
        }

        // child중 하나라도 선택되어 있으면 true를 반환한다.
        bool RecurseIsChildAnySelected(ObjectExpand obj)
        {
            if (obj.enumObjectType == EnumObjectType.Group || obj.enumObjectType == EnumObjectType.Layer)
            {
                
            }
            else
            {
                return false;
            }

            ObjectType type;
            ObjectPublicGroupLayer group = (ObjectPublicGroupLayer)obj;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectType)group.GetPoint(i);

                if (type.bOnStudioSelected) return true;

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    if (RecurseIsChildAnySelected((ObjectPublicGroupLayer)type)) return true;
                }
            }

            return false;
        }

        void RecursePaint(Graphics g, ObjectPublicGroupLayer group, ref int y, int depth, bool parent_visible, bool parent_lock, ref int list_pos)
        {
            depth++;

            ObjectExpand type;

            bool draw;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                list_pos++;

                if (list_pos < this.vScrollBar1.Value)
                {
                    draw = false;
                }
                else
                {
                    draw = true;
                }
                
                type = (ObjectExpand)group.GetPoint(bReverseLayer ? group.GetObjectHap()-1-i : i);

                Rectangle r = new Rectangle();

                if (draw)
                {
                    if (type.bOnStudioSelected)
                    {
                        DrawClass.gcls(g, depth * 10 + gaby * 3 - 4, y, (int)g.VisibleClipBounds.Right, y + gaby - 2, Color.RoyalBlue);
                    }
                    else
                    {   // 선택되지 않았을 경우 Child 중 하나가 선택되어 있으면 약하게 표시한다. 선택된 오브젝트를 쉽게 찾기 위해서 필요하다.
                        if (RecurseIsChildAnySelected(type))
                        {
                            DrawClass.gcls(g, (int)g.VisibleClipBounds.Right - 4, y, (int)g.VisibleClipBounds.Right, y + gaby - 2, Color.RoyalBlue);
                        }
                    }

                    if (type.enumObjectType == EnumObjectType.Layer)
                    {
                        DrawClass.PopBox2(g, 0, y, gaby * 2 - 2, y + gaby - 1, ((ObjectLayer)type).LayerColor);
                    }
                    else
                    {
                        DrawClass.PopBox2(g, 0, y, gaby * 2 - 2, y + gaby - 1, Color.LightGray);
                    }
                    
                    r.X = 2;
                    r.Y = y + 2;
                    r.Width = gaby - 4;
                    r.Height = gaby - 4;

                    DrawClass.PushBox2(g, r, Color.LightGray);
                    if (parent_visible && type.objGeneral.bOnStudioVisible)
                        g.DrawImageUnscaled(this.imageList1.Images[1], r.X + 1, r.Y + 1);
                    else if (!parent_visible && type.objGeneral.bOnStudioVisible)
                        g.DrawImageUnscaled(this.imageList1.Images[0], r.X + 1, r.Y + 1);
                    else { }

                    r.X = gaby + 1;
                    DrawClass.PushBox2(g, r, Color.LightGray);

                    if (parent_lock && type.objGeneral.bOnStudioLocked)
                        g.DrawImageUnscaled(this.imageList1.Images[3], r.X + 1, r.Y + 1);
                    else if (parent_lock && !type.objGeneral.bOnStudioLocked)
                        g.DrawImageUnscaled(this.imageList1.Images[2], r.X + 1, r.Y + 1);
                    else if (!parent_lock && type.objGeneral.bOnStudioLocked)
                        g.DrawImageUnscaled(this.imageList1.Images[3], r.X + 1, r.Y + 1);
                    else { }

                    if (type.enumObjectType == EnumObjectType.Layer)
                    {
                        g.DrawImageUnscaled(this.imageList2.Images[0], depth * 10 + gaby * 3 - 2, y);
                    }
                    else if (type.enumObjectType == EnumObjectType.Group)
                    {
                        g.DrawImageUnscaled(this.imageList2.Images[2], depth * 10 + gaby * 3 - 2, y);
                    }
                    else
                    {
                        Rectangle r2 = new Rectangle(depth * 10 + gaby * 3 - 2, y, gaby - 1, gaby - 2);
                        g.DrawRectangle(Pens.LightGray, r2);
                        if (type.previewOnStudio == null)
                            type.MakeLayerPreview(gaby - 2, gaby - 3);

                        g.DrawImageUnscaled(type.previewOnStudio, depth * 10 + gaby * 3 - 1, y + 1);
                    }

                    if (bMovingStart && nCaptureListPos == list_pos)
                    {
                        if (nCaptureChildPosBeforeAfter == -1)
                            DrawClass.gcls(g, 0, y, (int)g.VisibleClipBounds.Right, y + 2, Color.FromArgb(128, Color.DarkGray));
                        else if (nCaptureChildPosBeforeAfter == 1)
                            DrawClass.gcls(g, 0, y + gaby, (int)g.VisibleClipBounds.Right, y + gaby + 2, Color.FromArgb(128, Color.DarkGray));
                        else
                            DrawClass.gcls(g, 0, y, (int)g.VisibleClipBounds.Right, y + gaby - 2, Color.FromArgb(128, Color.DarkGray));
                    }
                }

                string title = ((ObjectExpand)type).objGeneral.sOnStudioTitle.Length == 0 ? type.enumObjectType.ToString() : ((ObjectExpand)type).objGeneral.sOnStudioTitle;

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    ObjectPublicGroupLayer grp = (ObjectPublicGroupLayer)type;

                    if (draw)
                    {
                        r.X = depth * 10 + gaby * 2;

                        if (grp.GetObjectHap() > 0)  // 안에 요소가 있을때만 +-를 그린다.
                            DrawPlusMinus(g, r, grp.bOnStudioGroupOpen);

                        if (type.enumObjectType == EnumObjectType.Group)
                        {
                            SafeException.SafeDrawString(g, title, this.Font, type.bOnStudioSelected ? Brushes.White : Brushes.Black, depth * 10 + gaby * 4 - 2, y + 2);
                        }
                        else
                        {
                            SafeException.SafeDrawString(g, title, this.Font, type.bOnStudioSelected ? Brushes.White : Brushes.Black, depth * 10 + gaby * 4 - 2, y + 2);
                        }
                        
                        y += gaby;
                    }

                    if (grp.bOnStudioGroupOpen)
                    {
                        RecursePaint(g, (ObjectPublicGroupLayer)type, ref y, depth, parent_visible && type.objGeneral.bOnStudioVisible, parent_lock || type.objGeneral.bOnStudioLocked, ref list_pos);
                    }
                }
                else
                {
                    if (draw)
                    {
                        string obj_title = type.GetObjectMainTitle();

                        if (obj_title.Length > 0)
                        {
                            title += String.Format(" '{0}'", obj_title);
                        }

                        SafeException.SafeDrawString(g, title, this.Font, type.bOnStudioSelected ? Brushes.White : Brushes.Black, depth * 10 + gaby * 4 - 2, y + 2);
                        y += gaby;
                    }
                }

            }
        }

        private void panelDraw_Paint(object sender, PaintEventArgs e)
        {
            if (rootGroup == null) return;

            int y = 0;
            int depth = -1;

            int list_pos = -1;
            RecursePaint(e.Graphics, rootGroup, ref y, depth, true, false, ref list_pos);
        }

        public static void MdiActivate(Form form, object group)
        {
            if(group == rootGroup)  return; // same object

            lastSelectedParent = null;
            lastSelectedObject = null;
            formWork = null;
            rootGroup = null;

            //System.NullReferenceException: 개체 참조가 개체의 인스턴스로 설정되지 않았습니다.
            //위치: Studio.Layer.FormLayer.MdiActivate(Form form, Object group) 파일 D:\NET2008\AutobasePrime\Studio\Layer\FormLayer.cs:줄 285
            if (formThis == null)   // 레이어 속성창이 이미 닫혔거나 아직 열리지 않은 경우 10.0.3.0 에서 formThis가 0인 경우가 있어 이부분을 추가했다. 2009.9.28 
            {
                return;
            }

            if (form == null)
            {
                formThis.UpdateObjectCount(); //250731 PSU
                formThis.Invalidate(true);
                return;  // 그래픽 모듈이 닫힐 때다.
            }

            if (form.GetType() != typeof(FormEditGraphic))
            {
                formThis.Invalidate(true);
                return;  // 그래픽 모듈이 아니다.
            }

            formWork = (FormEditGraphic)form;

            if (group == null)
            {
                if (formThis == null) return;
                formThis.CalcScroll();
                formThis.Invalidate(true);
                return;
            }

            if (group.GetType() == typeof(GraphicModule.ObjectGroup))
                rootGroup = (ObjectGroup)group;
            else
                rootGroup = null;

            if (formThis == null)
            {
                formThis.Invalidate(true);
                return;
            }
            formThis.UpdateObjectCount(); //250731 PSU
            formThis.CalcScroll();
            formThis.Invalidate(true);
        }

        int nSaveListCount = -1;

        public void CalcScroll()
        {
            if (rootGroup == null) return;  // 초기에 SizeChanged가 발생하면 할당하기 전에 발생한다.

            int count = RecurseGetListCount(rootGroup);

            if (count != nSaveListCount)
            {
                nSaveListCount = count;

                int limit = (panelDraw.ClientRectangle.Height) / gaby;
                if (count > limit)
                {
                    this.vScrollBar1.Visible = true;
                    this.vScrollBar1.Minimum = 0;
                    this.vScrollBar1.Maximum = count - 1;
                    if (this.vScrollBar1.Value > this.vScrollBar1.Maximum)
                    {
                        this.vScrollBar1.Value = this.vScrollBar1.Maximum;
                    }
                }
                else
                {
                    this.vScrollBar1.Value = 0;
                    this.vScrollBar1.Visible = false;
                }
            }
        }

        public static void SelectedChanged()
        {
            if (formThis != null)
            {
                formThis.CalcScroll();
                // 선택된 오브젝트로 스크롤 이동 250731 PSU
                formThis.ScrollToSelectedObject(); 
                formThis.Invalidate(true);
            }
        }

        public static void InvalidateDisplay()
        {
            if (formThis != null)
            {
                formThis.Invalidate(true);
            }
        }

        private void FormLayer_Load(object sender, EventArgs e)
        {
            formThis = this;

            fonty = (int)this.Font.GetHeight()+1;
            gaby = fonty + 3;

            if (gaby < 17) gaby = 17;
        }

        private void FormLayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            formThis = null;
        }

        void AllSelectionOff()
        {
            if (rootGroup == null) return;
            RecurseSelectionOff(rootGroup);
        }

        void RecurseSelectionOff(ObjectType obj)
        {
            obj.bOnStudioSelected = false;

            if (obj.enumObjectType == EnumObjectType.Layer || obj.enumObjectType == EnumObjectType.Group)
            {
                ObjectType type;
                ObjectPublicGroupLayer group = (ObjectPublicGroupLayer)obj;

                for (int i = 0; i < group.GetObjectHap(); i++)
                {
                    type = (ObjectType)group.GetPoint(i);

                    type.bOnStudioSelected = false;

                    if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                    {
                        RecurseSelectionOff((ObjectPublicGroupLayer)type);
                    }

                }
            }
        }

        static ObjectPublicGroupLayer lastSelectedParent = null;
        static ObjectType lastSelectedObject = null;

        void UpdateSelectList()
        {
            FormEditGraphic form = (FormEditGraphic)formWork;

            form.SelectListReMake();

            form.workThis.parentSelectList = lastSelectedParent;
        }

        void WorkAfterSelectChanged()
        {
            UpdateSelectList();
            formWork.DisplayAfterSelectedChangedCallByLayer();
            // 선택된 오브젝트로 스크롤 이동
            formWork.ScrollToSelectedObject(); //250731 PSU
            this.panelDraw.Invalidate();
            formWork.Invalidate();
        }

        void RecurseSelect(ObjectType type, bool flag)
        {
            type.bOnStudioSelected = flag;

            if (type.enumObjectType == EnumObjectType.Layer)
            {
                ObjectLayer layer = (ObjectLayer)type;
                ObjectType obj;

                for (int i = 0; i < layer.GetObjectHap(); i++)
                {
                    obj = (ObjectType)layer.GetPoint(i);

                    obj.bOnStudioSelected = flag;

                    if (obj.enumObjectType == EnumObjectType.Layer)
                    {
                        RecurseSelect((ObjectLayer)obj, flag);
                    }
                }
            }
        }

        int nCapturePosX, nCapturePosY;
        bool bCaptureFlag = false;  // Form.Capture 만으로는 정확한 마우스 캡쳐를 알 수 없다. Capture를 true로 하지 않아도 마우스캡쳐가 되는 경우가 있다. 그래서 CaptureFlag를 선언해서 사용했다.
        int nCaptureStartListPos;
        int nCaptureListPos;

        bool RecurseMouseDown(MouseEventArgs e, ObjectPublicGroupLayer group, ref int y, int depth, bool parent_visible, bool parent_lock, ref int list_pos)
        {
            depth++;

            ObjectExpand type;

            bool draw;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                list_pos++;

                if (list_pos < this.vScrollBar1.Value)
                {
                    draw = false;
                }
                else
                {
                    draw = true;
                }

                type = (ObjectExpand)group.GetPoint(bReverseLayer ? group.GetObjectHap()-1-i : i);

                if (draw)
                {
                    if (e.Y >= y && e.Y < y + gaby)
                    {
                        if (e.X < gaby) // visible 영역
                        {
                            type.objGeneral.bOnStudioVisible = !type.objGeneral.bOnStudioVisible;
                            this.panelDraw.Invalidate();
                            formWork.Invalidate();
                            formWork.SetChangeFlag();

                            return true;
                        }
                        else if (e.X <= (gaby * 2 - 2)) // Lock 영역 검사
                        {
                            type.objGeneral.bOnStudioLocked = !type.objGeneral.bOnStudioLocked;

                            if (type.objGeneral.bOnStudioLocked)
                            {
                                type.bOnStudioSelected = false;
                                RecurseSelectionOff(type);

                                WorkAfterSelectChanged();
                            }

                            this.panelDraw.Invalidate();
                            formWork.SetChangeFlag();

                            return true;
                        }
                        else if (e.X < (depth * 10 + gaby * 3 - 2)) // Expand icon +- 아이콘
                        {
                            if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                            {
                                ObjectPublicGroupLayer grp = (ObjectPublicGroupLayer)type;
                                grp.bOnStudioGroupOpen = !grp.bOnStudioGroupOpen;
                                this.CalcScroll();
                                this.panelDraw.Invalidate();

                            }

                            return true;
                        }
                        else
                        {
                            if (!type.objGeneral.bOnStudioLocked && !parent_lock)
                            {
                                bTitleClicked = true;
                                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                {
                                    if (group != lastSelectedParent)    // 다른 구역
                                    {
                                        AllSelectionOff();
                                    }

                                    type.bOnStudioSelected = !type.bOnStudioSelected;

                                    RecurseSelect(type, type.bOnStudioSelected);
                                }
                                else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                                {
                                    if (group != lastSelectedParent)    // 다른 구역
                                    {
                                        AllSelectionOff();
                                        type.bOnStudioSelected = true;
                                    }
                                    else
                                    {
                                        AllSelectionOff();

                                        bool flag = false;
                                        ObjectType o;

                                        for (int j = 0; j < group.GetObjectHap(); j++)
                                        {
                                            o = (ObjectType)group.GetPoint(j);
                                            if (flag)
                                            {
                                                RecurseSelect(o, true);

                                                if (o == lastSelectedObject || o == type) break;
                                            }
                                            else
                                            {
                                                if (o == lastSelectedObject || o == type)
                                                {
                                                    flag = true;
                                                    RecurseSelect(o, true);

                                                    if (o == lastSelectedObject && o == type)
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (type.bOnStudioSelected) // 이미 선택되어 있다.
                                    {

                                    }
                                    else
                                    {
                                        AllSelectionOff();
                                        type.bOnStudioSelected = true;
                                    }

                                    lastSelectedParent = group;
                                    lastSelectedObject = type;

                                    RecurseSelect(type, true);

                                    WorkAfterSelectChanged();

                                    bCaptureFlag = true;
                                    this.panelDraw.Capture = true;
                                    nCapturePosX = e.X;
                                    nCapturePosY = e.Y;
                                    nCaptureListPos = list_pos;
                                    nCaptureStartListPos = list_pos;
                                    oCaptureObject = type;
                                    return true;
                                }

                                lastSelectedParent = group;
                                lastSelectedObject = type;

                                //RecurseSelect(type, true); 이것이 있으니 Control키를 누르고 선택을 취소할 때 되지 않는다.

                                WorkAfterSelectChanged();
                            }   // if ! locked
                        } // else
                    }//if (e.Y >= y && e.Y < y + gaby)
                } // if draw

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    ObjectPublicGroupLayer grp = (ObjectPublicGroupLayer)type;

                    if (draw)
                    {
                        y += gaby;
                    }
                    
                    if (grp.bOnStudioGroupOpen)
                    {
                        if (RecurseMouseDown(e, (ObjectPublicGroupLayer)type, ref y, depth, parent_visible && type.objGeneral.bOnStudioVisible, parent_lock || type.objGeneral.bOnStudioLocked, ref list_pos)) return true;
                    }
                }
                else
                {
                    if (draw)
                    {
                        y += gaby;
                    }
                }
            }

            return false;
        }

        bool bTitleClicked = false; // 각 오브젝트의 제목 부분이 클릭되었다.

        private void panelDraw_MouseDown(object sender, MouseEventArgs e)
        {
            if (rootGroup == null) return;
            if (e.Button != MouseButtons.Left) return;

            int y = 0;
            int depth = -1;
            int list_pos = -1;
            bTitleClicked = false;

            RecurseMouseDown(e, rootGroup, ref y, depth, true, false, ref list_pos);
        }

        ObjectPublicGroupLayer oCaptureParent;      // 이동 위치 커서의 잡을 위치의 Parent
        ObjectType oCaptureObject;                  // 이동 위치 커서의 오브젝트
        int nCaptureChildPos;                       // 그룹이나 레이어 내에서의 배열 위치
        int nCaptureChildPosBeforeAfter = 0;        // 해당 위치의 앞(-1)이냐 뒤(1)냐? 아니면 중간이냐(0)?

        bool RecurseMouseMove(MouseEventArgs e, ObjectPublicGroupLayer group, ref int y, int depth, bool parent_visible, bool parent_lock, ref int list_pos, bool parent_selected)
        {
            depth++;

            ObjectExpand type;

            bool draw;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                list_pos++;

                if (list_pos < this.vScrollBar1.Value)
                {
                    draw = false;
                }
                else
                {
                    draw = true;
                }

                type = (ObjectExpand)group.GetPoint(bReverseLayer ? group.GetObjectHap()-1-i: i);

                if (draw)
                {
                    if (e.Y >= y && e.Y < y + gaby)
                    {
                        if (!type.objGeneral.bOnStudioLocked && !parent_lock && !parent_selected)
                        {
                            int before_after = 0;

                            if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                            {
                                if (e.Y <= y + 2)
                                {
                                    before_after = -1;
                                }
                                else if (e.Y >= y + gaby - 3)
                                {
                                    before_after = 1;
                                }
                                else
                                {
                                    before_after = 0;
                                }
                            }
                            else
                            {
                                if (e.Y <= y + gaby / 2)
                                {
                                    before_after = -1;
                                }
                                else
                                {
                                    before_after = 1;
                                }
                            }

                            if (list_pos != nCaptureListPos || before_after != nCaptureChildPosBeforeAfter)
                            {
                                nCaptureListPos = list_pos;
                                nCaptureChildPosBeforeAfter = before_after;

                                panelDraw.Invalidate();
                                oCaptureParent = group;
                                oCaptureObject = type;
                                nCaptureChildPos = bReverseLayer ? group.GetObjectHap() - 1 - i : i;
                            }
                        } 
                    }
                }

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    ObjectPublicGroupLayer grp = (ObjectPublicGroupLayer)type;

                    if (draw)
                    {
                        y += gaby;
                    }

                    if (grp.bOnStudioGroupOpen)
                    {
                        if (RecurseMouseMove(e, (ObjectPublicGroupLayer)type, ref y, depth, parent_visible && type.objGeneral.bOnStudioVisible, parent_lock || type.objGeneral.bOnStudioLocked, ref list_pos, parent_selected || type.bOnStudioSelected)) return true;
                    }
                }
                else
                {
                    if (draw)
                    {
                        y += gaby;
                    }
                }
            }

            return false;
        }

        bool bMovingStart = false;
        int  nMouseOutPosition = 0;

        private void panelDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (!bCaptureFlag) return;

            if (bMovingStart)   // 이동 모드 시작
            {
                if (e.Y < 0)
                {
                    nMouseOutPosition = 1;  // 전체 페이지 위로 스크롤 모드
                }
                else if(e.Y > panelDraw.ClientRectangle.Bottom)
                {
                    nMouseOutPosition = 2;  // 전체 페이지 아래로 스크롤
                }
                else
                {
                    nMouseOutPosition = 0;

                    int y = 0;
                    int depth = -1;
                    int list_pos = -1;

                    RecurseMouseMove(e, rootGroup, ref y, depth, true, false, ref list_pos, false);
                }
            }
            else
            {
                if (Math.Abs(e.Y - nCapturePosY) >= 2)  // 캡춰한 위치에서 2칸이상 움직으면 이동 모드가 된다.
                {
                    this.panelDraw.Cursor = Cursors.HSplit;
                    bMovingStart = true;
                }
            }
        }

        // 실제 스크린에서의 좌표로 바꾸어 준다.
        bool RecurseCalcScreenPos(ObjectPublicGroupLayer pgroup, SELECT_LIST list)
        {
            ObjectExpand obj;

            for (int i = 0; i < pgroup.GetObjectHap(); i++)
            {
                obj = (ObjectExpand)pgroup.GetPoint(i);

                if (obj == list.obj)
                {
                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

                    obj.GetZone(ref x1, ref y1, ref x2, ref y2);

                    x1 = obj.GetViewPosXOn100Percent(x1);
                    y1 = obj.GetViewPosYOn100Percent(y1);
                    x2 = obj.GetViewPosXOn100Percent(x2);
                    y2 = obj.GetViewPosYOn100Percent(y2);

                    obj.UpdateZone(formWork, x1, y1, x2, y2);

                    return true;
                }

                if (obj.enumObjectType == EnumObjectType.Group) 
                {
                    if (RecurseCalcScreenPos((ObjectPublicGroupLayer)obj, list)) return true;
                }
                else if (obj.enumObjectType == EnumObjectType.Layer) 
                {
                    if (RecurseCalcScreenPos((ObjectPublicGroupLayer)obj, list)) return true;
                }
            }

            return false;
        }

        void SelectedObjectToArray(ObjectPublicGroupLayer gl, ArrayList array, bool calc_location, ObjectPublicGroupLayer target_group)
        {
            WORK_MODULE_STRUCT work = formWork.workThis;

            SELECT_LIST[] selectList;
            
            if(target_group.enumObjectType == EnumObjectType.Group && target_group != work.obj.groupRoot)     // 그룹에 끌어 넣기 할 때는 레이어는 필요없다. 단 루트인 경우는 Group이라고 할 수 없다.
                selectList = formWork.SelectListMakeOnlyChild();
            else
                selectList = formWork.SelectListMakeOnlyParent();       // 그룹이 아닌 경우는 레이어도 이동하는 것이 편리하다.

            if (selectList == null) return;

            SELECT_LIST list;

            for (int i = 0; i < selectList.Length; i++)
            {
                list = selectList[i];
                ObjectExpand p = (ObjectExpand)list.obj;

                if(calc_location) 
                    RecurseCalcScreenPos(work.obj.groupRoot, list);

                array.Add(p);
            }
        }

        // 루트 화면상의 오브젝트가 그룹속에 위치 했을 때 위치할 좌표를 찾는다.
        bool RecurseFitArrayToTarget(ObjectPublicGroupLayer gl, object p, int x1, int y1, int x2, int y2)
        {
            ObjectExpand obj;

            for (int i = 0; i < gl.GetObjectHap(); i++)
            {
                obj = (ObjectExpand)gl.GetPoint(i);

                if (obj == p)
                {
                    obj.UpdateZone(formWork, x1, y1, x2, y2);

                    return true;
                }

                // 그룹속으로 들어 갈때는 그륩에 해당되는 좌표로 바꾸어서 들어간다.
                if (obj.enumObjectType == EnumObjectType.Group)
                {
                    RECT r = new RECT();
                    SIZE size = new SIZE();

                    ((ObjectGroup)obj).GetGroupRealSize(ref size.cx, ref size.cy);  // 그룹의 실제 크기
                    ((ObjectGroup)obj).GetZone(ref r);                              // 그룹의 크기

                    int smallx = r.left < r.right ? r.left : r.right;
                    int smally = r.top < r.bottom ? r.top : r.bottom;

                    int xx1, xx2, yy1, yy2;

                    xx1 = x1-smallx;
                    yy1 = y1-smally;
                    xx2 = x2-smallx;
                    yy2 = y2-smally;

                    int sizex = Math.Abs(r.right-r.left)+1;
                    int sizey = Math.Abs(r.bottom-r.top)+1;

                    xx1 = xx1 * size.cx/sizex;
                    yy1 = yy1 * size.cy/sizey;
                    xx2 = xx2 * size.cx / sizex;
                    yy2 = yy2 * size.cy / sizey;

                    if (RecurseFitArrayToTarget((ObjectPublicGroupLayer)obj, p, xx1, yy1, xx2, yy2)) return true;
                }
                else if (obj.enumObjectType == EnumObjectType.Layer)
                {
                    if (RecurseFitArrayToTarget((ObjectPublicGroupLayer)obj, p, x1, y1, x2, y2)) return true;
                }
            }

            return false;
        }

        void FitArrayToTarget(ArrayList array, bool calc_location)
        {
            if (!calc_location) return;

            object obj;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            for (int i = 0; i < array.Count; i++)
            {
                obj = array[i];

                RECT r = new RECT();
                formWork.workThis.obj.groupRoot.GetZone(ref r);
                SIZE size = new SIZE();
                formWork.workThis.obj.groupRoot.GetGroupRealSize(ref size.cx, ref size.cy);

                ((ObjectExpand)obj).GetZone(ref x1, ref y1, ref x2, ref y2);

                RecurseFitArrayToTarget(formWork.workThis.obj.groupRoot, obj, x1, y1, x2, y2);
            }
        }

        void DeleteSelectedObject(ObjectPublicGroupLayer gl)
        {
            int i;
            ObjectExpand obj;

            // 지울때는 위에부터 지워야 한다.
            for (i = gl.GetObjectHap() - 1; i >= 0; i--)
            {
                obj = (ObjectExpand)gl.GetPoint(i);

                if (obj.bOnStudioSelected)
                {
                    gl.DeleteOneObject(i);
                }
            }
        }

        void MoveObject()
        {
            ClassStudioEditUndo.UndoSave_All(formWork, "Move object");

            ObjectPublicGroupLayer source_group = lastSelectedParent;
            ObjectPublicGroupLayer target_group = oCaptureParent;

            if (oCaptureObject.enumObjectType == EnumObjectType.Layer && nCaptureChildPosBeforeAfter == 0)  // 레이어에 Drop하면 맨 나중에 추가한다.
            {
                target_group = (ObjectPublicGroupLayer)oCaptureObject;

                target_group.bOnStudioSelected = false;

                ArrayList array = new ArrayList();

                SelectedObjectToArray(source_group, array, true, target_group);
                DeleteSelectedObject(source_group);

                for (int i = 0; i < array.Count; i++)
                {
                    object p = array[i];
                    target_group.AddObject(p);
                }

                FitArrayToTarget(array, true);

                target_group.bOnStudioGroupOpen = true;
            }
            else if (oCaptureObject.enumObjectType == EnumObjectType.Group && nCaptureChildPosBeforeAfter == 0) // 그룹 Drop하면 맨 나중에 추가한다.
            {
                target_group = (ObjectPublicGroupLayer)oCaptureObject;

                target_group.bOnStudioSelected = false;

                ArrayList array = new ArrayList();

                SelectedObjectToArray(source_group, array, true, target_group);
                DeleteSelectedObject(source_group);

                for (int i = 0; i < array.Count; i++)
                {
                    object p = array[i];
                    target_group.AddObject(p);
                }

                FitArrayToTarget(array, true);

                target_group.bOnStudioGroupOpen = true;
            }
            else
            {
                int insert_pos = nCaptureChildPos;

                if (bReverseLayer)
                {
                    if (nCaptureChildPosBeforeAfter == -1)
                        insert_pos++;
                }
                else
                {
                    if (nCaptureChildPosBeforeAfter == 1)
                        insert_pos++;
                }

                bool calc_location = true;                    

                if (source_group == target_group)   // 같은 Parent에서 이동할 때
                {
                    calc_location = false;  // 같은 Parent 에서의 이동은 위치 계산을 할 필요가 없다.
                    int limit_pos = insert_pos;

                    for (int i = 0; i < source_group.GetObjectHap() && i < limit_pos; i++)
                    {
                        if (((ObjectType)source_group.GetPoint(i)).bOnStudioSelected)
                        {
                            insert_pos--;
                        }
                    }
                }

                ArrayList array = new ArrayList();

                SelectedObjectToArray(source_group, array, calc_location, target_group);
                DeleteSelectedObject(source_group);

                for (int i = 0; i < array.Count; i++)
                {
                    object p = array[i];
                    target_group.InsertObject(p, insert_pos+i);
                }

                FitArrayToTarget(array, calc_location);

                target_group.bOnStudioGroupOpen = true;
            }

            WORK_MODULE_STRUCT work = formWork.workThis;

            work.obj.SetBasePoint(-work.nScrollHorPos, -work.nScrollVerPos);
            work.obj.EditReCalcGroupSize();
            work.obj.SetZoneAtPercent100();
            
            WorkAfterSelectChanged();
            formWork.SetChangeFlag();
        }

        private void panelDraw_MouseUp(object sender, MouseEventArgs e)
        {
            if (!bCaptureFlag) return;

            this.panelDraw.Cursor = Cursors.Default;
            bMovingStart = false;
            bCaptureFlag = false;
            this.panelDraw.Capture = false;

            if (nCaptureListPos != nCaptureStartListPos)    
            {
                MoveObject();           
            }

            this.panelDraw.Invalidate();
        }

        void LayerAdd()
        {
            if (formWork == null) return;

            AllSelectionOff();

            FormEditGraphic form = (FormEditGraphic)formWork;

            ObjectLayer obj = new ObjectLayer(form.workThis.obj.objCommonProperty, null, null, null, null);

            obj.bOnStudioSelected = true;

            ObjectPublicGroupLayer parent;

            if (lastSelectedParent == null)
            {
                parent = form.workThis.obj.groupRoot;
            }
            else
            {
                parent = (ObjectPublicGroupLayer)lastSelectedParent;
                while (true)
                {
                    if (parent.enumObjectType == EnumObjectType.Layer) break;
                    if (parent.parentGroupLayer == null) break;
                    parent = parent.parentGroupLayer;
                }
            }

            int[] opos = null;
            ClassEditInsert.RecurseMakePos(form.workThis.obj.groupRoot, parent, ref opos);
            ClassStudioEditUndo.UndoSave_Add(form, "Add Layer", opos, ((ObjectPublicGroupLayer)parent).GetObjectHap(), 1);

            parent.AddObject(obj);

            lastSelectedParent = parent;
            lastSelectedObject = obj;

            WorkAfterSelectChanged();
            formWork.SetChangeFlag();
        }

        void AddSubLayer()
        {
            if (lastSelectedObject == null) return;

            AllSelectionOff();

            FormEditGraphic form = (FormEditGraphic)formWork;

            ObjectLayer obj = new ObjectLayer(form.workThis.obj.objCommonProperty, null, null, null, null);

            obj.bOnStudioSelected = true;

            ObjectPublicGroupLayer parent;

            if (lastSelectedObject.enumObjectType == EnumObjectType.Layer)
            {
                parent = (ObjectPublicGroupLayer)lastSelectedObject;
            }
            else
            {
                if (lastSelectedParent == null)
                {
                    parent = form.workThis.obj.groupRoot;
                }
                else
                {
                    parent = (ObjectPublicGroupLayer)lastSelectedParent;
                    while (true)
                    {
                        if (parent.enumObjectType == EnumObjectType.Layer) break;
                        if (parent.parentGroupLayer == null) break;
                        parent = parent.parentGroupLayer;
                    }
                }
            }

            int[] opos = null;
            ClassEditInsert.RecurseMakePos(form.workThis.obj.groupRoot, parent, ref opos);
            ClassStudioEditUndo.UndoSave_Add(form, "Add Sub Layer", opos, ((ObjectPublicGroupLayer)parent).GetObjectHap(), 1);

            parent.AddObject(obj);

            lastSelectedParent = parent;
            lastSelectedObject = obj;

            if (parent.GetObjectHap() == 1) // 레이어에서 처음으로 생성된 오브젝트
            {
                parent.bOnStudioGroupOpen = true;
            }

            WorkAfterSelectChanged();
            formWork.SetChangeFlag();
        }

        bool IsAbleDeleteLayer()
        {
            if (lastSelectedObject == null) return false;
            if (lastSelectedObject.enumObjectType != EnumObjectType.Layer)
            {
                return false;
            }

            return true;
        }

        void DeleteLayer()
        {
            if (!IsAbleDeleteLayer()) return;

            if (MessageBox.Show("선택한 레이어를 삭제할까요?", "레이어 삭제", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            ObjectPublicGroupLayer parent;
            FormEditGraphic form = (FormEditGraphic)formWork;

            if (lastSelectedParent == null)
            {
                parent = form.workThis.obj.groupRoot;
            }
            else
            {
                parent = (ObjectPublicGroupLayer)lastSelectedParent;
                while (true)
                {
                    if (parent.enumObjectType == EnumObjectType.Layer) break;
                    if (parent.parentGroupLayer == null) break;
                    parent = parent.parentGroupLayer;
                }
            }

            ClassStudioEditUndo.UndoSave_All(form, "Delete Layer");

            parent.Remove(lastSelectedObject);

            lastSelectedParent = parent;
            lastSelectedObject = null;

            WorkAfterSelectChanged();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            this.panelDraw.Invalidate();
        }

        private void FormLayer_SizeChanged(object sender, EventArgs e)
        {
            CalcScroll();
            Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LayerAdd();
        }

        private void toolStripButtonAddSubLayer_Click(object sender, EventArgs e)
        {
            AddSubLayer();
        }

        private void toolStripButtonDeleteLayer_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditDelete(formWork);         
        }

        bool IsAbleLayerProperties()
        {
            if (lastSelectedObject == null) return false;
            if (lastSelectedObject.enumObjectType != EnumObjectType.Layer) return false;

            return true;
        }

        private void panelDraw_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!bTitleClicked) return; // 오브젝트의 제목 부분이 클릭되었을 때만 더블클릭을 허용한다. 트리를 확장/축소 했을 경우도 생길 수 있으므로

            if (IsAbleLayerProperties())
            {
                LayerPropertyGo();
            }
            else
            {
                ObjectPropertyGo();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (formWork == null)
            {
                e.Cancel = true;
                return;
            }

            this.toolStripMenuItemLayerProperties.Enabled = IsAbleLayerProperties();
            this.toolStripMenuItemObjectProerties.Enabled = IsAbleObjectProperties();

            this.toolStripMenuItemHide.Enabled = (formWork.workThis.nSelectCount > 0);
            this.toolStripMenuItemLock.Enabled = ClassStudioEdit.IsPossibleEditLock(formWork);
            this.toolStripMenuItemShow.Enabled = (formWork.workThis.nSelectCount > 0);
            this.toolStripMenuItemUnLock.Enabled = ClassStudioEdit.IsPossibleEditUnLock(formWork);

            this.toolStripMenuItemGroup.Enabled = ClassStudioEdit.IsPossibleEditGroup(formWork);
            this.toolStripMenuItemUnGroup.Enabled = ClassStudioEdit.IsPossibleEditUnGroup(formWork);
        }

        private void toolStripMenuItemAddLayer_Click(object sender, EventArgs e)
        {
            LayerAdd();
        }

        private void toolStripMenuItemAddSubLayer_Click(object sender, EventArgs e)
        {
            AddSubLayer();
        }

        void LayerPropertyGo()
        {
            if (!IsAbleLayerProperties()) return;

            FormConfigLayer dialog = new FormConfigLayer();

            dialog.Set((ObjectLayer)lastSelectedObject);
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ClassStudioEditUndo.UndoSave_All(formWork, "Change Layer Properties");
                dialog.Get((ObjectLayer)lastSelectedObject);
                this.panelDraw.Invalidate();
            }
        }

        private void toolStripMenuItemLayerProperties_Click(object sender, EventArgs e)
        {
            LayerPropertyGo();
        }

        bool IsAbleObjectProperties()
        {
            if (lastSelectedObject == null) return false;

            return true;
        }

        void ObjectPropertyGo()
        {
            if (!IsAbleObjectProperties()) return;

            FormEditGraphic form = (FormEditGraphic)formWork;

            ClassEditProperty.OnUserPropertyClick(form);
        }

        private void toolStripMenuItemObjectProerties_Click(object sender, EventArgs e)
        {
            ObjectPropertyGo();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!bCaptureFlag)              return;
            if (!bMovingStart)              return;
            if (nMouseOutPosition == 0)     return;
            if (!vScrollBar1.Visible) return;

            if (nMouseOutPosition == 1)
            {
                if (this.vScrollBar1.Value <= 0) return;

                this.vScrollBar1.Value--;
                this.panelDraw.Invalidate();
            }
            else
            {
                if (this.vScrollBar1.Value >= this.vScrollBar1.Maximum) return;

                this.vScrollBar1.Value++;
                this.panelDraw.Invalidate();
            }
        }

        void RecurseSelectionLock(ObjectPublicGroupLayer group, bool flag)
        {
            ObjectExpand type;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectExpand)group.GetPoint(i);

                if (type.bOnStudioSelected)
                {
                    type.objGeneral.bOnStudioLocked = flag;
                }

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    RecurseSelectionLock((ObjectPublicGroupLayer)type, flag);
                }
            }
        }

        private void toolStripMenuItemLock_Click(object sender, EventArgs e)
        {
            RecurseSelectionLock(formWork.workThis.obj.groupRoot, true);

            this.panelDraw.Invalidate();
        }

        private void toolStripMenuItemUnLock_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditUnLock(formWork);
        }

        void RecurseSelectionShow(ObjectPublicGroupLayer group, bool flag)
        {
            ObjectExpand type;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectExpand)group.GetPoint(i);

                if (type.bOnStudioSelected)
                {
                    type.objGeneral.bOnStudioVisible = flag;
                }

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    RecurseSelectionShow((ObjectPublicGroupLayer)type, flag);
                }

            }
        }

        private void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            RecurseSelectionShow(formWork.workThis.obj.groupRoot, true);

            this.panelDraw.Invalidate();
            formWork.Invalidate();
        }

        private void toolStripMenuItemHide_Click(object sender, EventArgs e)
        {
            RecurseSelectionShow(formWork.workThis.obj.groupRoot, false);

            this.panelDraw.Invalidate();
            formWork.Invalidate();
        }

        private void toolStripMenuItemGroup_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditGroup(formWork);
        }

        private void toolStripMenuItemUnGroup_Click(object sender, EventArgs e)
        {
            ClassStudioEdit.EditUnGroup(formWork);
        }

        public static void UpdateObjectCountIfVisible()
        {
            if (formThis != null)
            {
                formThis.UpdateObjectCount();
            }
        }

        public void UpdateObjectCount()
        {
            if (rootGroup == null)
            {
                this.toolStripLabelCount.Text = "0";
                return;
            }

            int totalCount = RecurseGetTotalObjectCount(rootGroup);
            this.toolStripLabelCount.Text = totalCount.ToString();
        }

        //  전체 오브젝트 개수를 구하는 메서드
        int RecurseGetTotalObjectCount(ObjectPublicGroupLayer group)
        {
            int count = 0;
            ObjectType type;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                type = (ObjectType)group.GetPoint(i);
                count++; // 모든 오브젝트 카운트 (열림/닫힘 상관없이)

                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    count += RecurseGetTotalObjectCount((ObjectPublicGroupLayer)type);
                }
            }

            return count;
        }

        // 선택된 오브젝트가 보이도록 스크롤 조정 250731 PSU
        private void ScrollToSelectedObject()
        {
            if (!this.vScrollBar1.Visible) return; // 스크롤바가 없으면 불필요

            int selectedPosition = FindSelectedObjectPosition();
            if (selectedPosition == -1) return; // 선택된 항목이 없음

            int visibleItemCount = panelDraw.ClientRectangle.Height / gaby;
            int currentTopPosition = this.vScrollBar1.Value;
            int currentBottomPosition = currentTopPosition + visibleItemCount - 1;

            // 이미 보이는 영역에 있는지 확인
            if (selectedPosition >= currentTopPosition && selectedPosition <= currentBottomPosition)
            {
                return; // 이미 보임
            }

            int newScrollValue;

            // 선택된 항목이 현재 보이는 영역 위에 있는 경우
            if (selectedPosition < currentTopPosition)
            {
                newScrollValue = selectedPosition;
            }
            // 선택된 항목이 현재 보이는 영역 아래에 있는 경우
            else
            {
                newScrollValue = selectedPosition - visibleItemCount + 1;
            }

            // 스크롤 범위 제한
            newScrollValue = Math.Max(0, Math.Min(newScrollValue, this.vScrollBar1.Maximum));

            this.vScrollBar1.Value = newScrollValue;
            this.panelDraw.Invalidate();
        }

        // 선택된 오브젝트의 리스트 위치를 찾는 메서드
        private int FindSelectedObjectPosition()
        {
            if (rootGroup == null) return -1;

            int position = -1;
            RecurseFindSelectedPosition(rootGroup, ref position);
            return position;
        }

        // 재귀적으로 선택된 오브젝트의 위치를 찾음
        private bool RecurseFindSelectedPosition(ObjectPublicGroupLayer group, ref int position)
        {
            ObjectExpand type;

            for (int i = 0; i < group.GetObjectHap(); i++)
            {
                position++;

                type = (ObjectExpand)group.GetPoint(bReverseLayer ? group.GetObjectHap() - 1 - i : i);

                // 선택된 오브젝트를 찾으면 true 반환
                if (type.bOnStudioSelected)
                {
                    return true;
                }

                // 그룹/레이어인 경우 하위 항목도 검사
                if (type.enumObjectType == EnumObjectType.Group || type.enumObjectType == EnumObjectType.Layer)
                {
                    ObjectPublicGroupLayer grp = (ObjectPublicGroupLayer)type;

                    if (grp.bOnStudioGroupOpen)
                    {
                        if (RecurseFindSelectedPosition(grp, ref position))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


    }
}