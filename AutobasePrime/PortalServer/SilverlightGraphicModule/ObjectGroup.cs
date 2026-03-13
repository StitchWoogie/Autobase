using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using NetTools.OldDefine;
using System.Collections.Generic;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public enum EnumNotType
    {
        NOT_TYPE_RECT,			// 사각형과 8개의 사각 포인트를 그린다.
        NOT_TYPE_LINE,			// 두점만 그린다.
        NOT_TYPE_POINT8,		// 사각형은 그리지 않고 8개의 사각 포인트만 그린다.
        NOT_TYPE_POLY,
        NOT_TYPE_GROUP,			// Group 형태의 Not Type
    }

    public class FAMILY_FILE_STRUCT
    {
        public string filename;	// 얻어올 때의 파일 명
        public string change;		// Change 할 때 파일 명
    }

    /*
    public class MULTI_SELECT_TAG_STRUCT
    {
        public string tagSource;		//
        public string tagTarget;
        public EnumTagType tag_type;			// 태그의 종류.
    }*/

    /// <summary>
    /// Summary description for ObjectGroup.
    /// </summary>
    /// 
    //[Serializable]
    public class ObjectGroup : ObjectPublicGroupLayer
    {
        public SIZE sizeGroup = new SIZE();		// 그룹의 실제 사이즈

        public ObjectGroup(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general) :
            base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Group;

            sizeGroup.cx = 0;
            sizeGroup.cy = 0;
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }

        public int Load(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string filename, int depth, EnumModType load_type)
        {
            int retn = 0;

            Canvas child = new Canvas();
            //child.Background = new SolidColorBrush(Colors.Yellow);

            ObjectGroupLoadModX load = new ObjectGroupLoadModX();
            retn = load.Load(ocp, child, this, form, reader, filename, depth, load_type);

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);

            ((TransformGroup)child.RenderTransform).Children.Insert(0, transformScale);   // Transform의 순서가 중요하다. 회전후 Scale하면 계산하기가 너무 어려워서 Scale후 Rotate를 하는 것이 좋다.

            MoveShape();

            return retn;
        }

        ScaleTransform transformScale = new ScaleTransform();

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            // base.OnMove(x1, y1, x2, y2);
            FrameworkElement shape = GetShapeOriginal();

            if (shape == null) return;

            if (x1 > x2) NetTools.Tools.Temp(ref x1, ref x2);
            if (y1 > y2) NetTools.Tools.Temp(ref y1, ref y2);

            Canvas.SetLeft(shape, x1);
            Canvas.SetTop(shape, y1);

            double width = x2 - x1 + 1;
            double height = y2 - y1 + 1;

            if (width == sizeGroup.cx && height == sizeGroup.cy)
            {
                //shape.RenderTransform = null;
                //shape.Width = width;
                //shape.Height = height;
                transformScale.ScaleX = 1;
                transformScale.ScaleY = 1;
                return;
            }

            // 그룹의 요소가 하나도 없으면 sizeGroup.cx/cy 가 0이된다.
            double fOpticX = (sizeGroup.cx == 0) ? width : width / sizeGroup.cx;
            double fOpticY = (sizeGroup.cy == 0) ? height : height / sizeGroup.cy;

            ScaleTransform transform = transformScale;// new ScaleTransform();
            transform.ScaleX = fOpticX;
            transform.ScaleY = fOpticY;

            //shape.RenderTransform = transform;

            //shape.Width = width * fOpticX;
            //shape.Height = height * fOpticY;
        }

        // Plan Load한 후 한번 불러준다.
        public void UpdateGroupRealSize(int x, int y)
        {
            sizeGroup.cx = x;
            sizeGroup.cy = y;
        }

        /*
        public override void UpdateZone(int x1, int y1, int x2, int y2)
        {
            base.UpdateZone(x1, y1, x2, y2);

            Canvas.SetLeft(canvasThis, x1);
            Canvas.SetTop(canvasThis, y1);
        }*/

        /*
        public void GetGroupRealSize(ref int x, ref int y)
        {
            x = sizeGroup.cx;
            y = sizeGroup.cy;
        }	// Plan Load한 후 한번 불러준다. 


        public void UpdateZone(System.Windows.Forms.Form form, int pos, int x1, int y1, int x2, int y2)
        {
            if (pos >= objectList.Count)
            {
                MessageBox.Show("pos >= ObjectHap", "UpdateObjectZone");
                return;
            }

            object obj;
            obj = objectList[pos];

            ((ObjectExpand)obj).UpdateZone(x1, y1, x2, y2);
        }
        */

        //--------------------------------------------------------
        // 내가 속한 그룹의 화면상의 좌표를 구한다.
        //--------------------------------------------------------

        public void SetZoneAtPercent100(RECT rView)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];
                switch (((ObjectType)obj).enumObjectType)
                {
                    case EnumObjectType.Group:
                        {
                            ObjectGroup group = (ObjectGroup)obj;

                            RECT r = new RECT();
                            int p_sizex, p_sizey;

                            group.GetZone(ref r);

                            p_sizex = rView.right - rView.left + 1;
                            p_sizey = rView.bottom - rView.top + 1;

                            r.left = r.left * p_sizex / sizeGroup.cx + rView.left;
                            r.top = r.top * p_sizey / sizeGroup.cy + rView.top;
                            r.right = r.right * p_sizex / sizeGroup.cx + rView.left;
                            r.bottom = r.bottom * p_sizey / sizeGroup.cy + rView.top;

                            group.SetZoneAtPercent100(r);

                            ((ObjectExpand)obj).SetZoneAtPercent100(sizeGroup, rView);
                        }
                        break;
                    default:
                        ((ObjectExpand)obj).SetZoneAtPercent100(sizeGroup, rView);
                        break;
                }
            }
        }

        /*
        //-----------------------------------------------------------------------------------------
        // 6.20 이후 버전부터는 Group 특성이 바뀌었으므로 이전 버전들은 모두 속성을 바꿔주어야 한다.
        //-----------------------------------------------------------------------------------------

        public void MakeGroupRect(System.Windows.Forms.Form form)
        {
            if (objectList.Count == 0) return;

            RECT r = new RECT(), rMax = new RECT();
            int l;

            for (l = 0; l < objectList.Count; l++)
            {
                GetZone(l, ref r);
                if (r.left > r.right) Tools.Temp(ref r.left, ref r.right);
                if (r.top > r.bottom) Tools.Temp(ref r.top, ref r.bottom);

                if (l == 0)
                {
                    rMax.left = r.left;
                    rMax.top = r.top;
                    rMax.right = r.right;
                    rMax.bottom = r.bottom;
                }
                else
                {
                    if (r.left < rMax.left) rMax.left = r.left;
                    if (r.right > rMax.right) rMax.right = r.right;
                    if (r.top < rMax.top) rMax.top = r.top;
                    if (r.bottom > rMax.bottom) rMax.bottom = r.bottom;
                }
            }

            nLeft = rMax.left;
            nTop = rMax.top;
            nRight = rMax.right;
            nBottom = rMax.bottom;
            sizeGroup.cx = nRight - nLeft + 1;
            sizeGroup.cy = nBottom - nTop + 1;

            RECT rLeft = new RECT();

            for (l = 0; l < objectList.Count; l++)
            {
                GetZone(l, ref rLeft);

                rLeft.left -= rMax.left;
                rLeft.top -= rMax.top;
                rLeft.right -= rMax.left;
                rLeft.bottom -= rMax.top;

                UpdateZone(form, l, rLeft);
            }
        }

        int GetZone(int pos, ref RECT r)
        {
            int x1, y1, x2, y2;

            x1 = r.left;
            y1 = r.top;
            x2 = r.right;
            y2 = r.bottom;

            GetZone(pos, ref x1, ref y1, ref x2, ref y2);

            r.left = x1;
            r.top = y1;
            r.right = x2;
            r.bottom = y2;

            return 1;
        }

        void UpdateZone(System.Windows.Forms.Form form, int pos, RECT r)
        {
            int x1, y1, x2, y2;

            x1 = r.left;
            y1 = r.top;
            x2 = r.right;
            y2 = r.bottom;

            UpdateZone(form, pos, x1, y1, x2, y2);
        }

        public static FAMILY_FILE_STRUCT GetMatchFamilyFile(string filename, ArrayList block)
        {
            int l;
            FAMILY_FILE_STRUCT family;

            for (l = 0; l < block.Count; l++)
            {
                family = (FAMILY_FILE_STRUCT)block[l];
                if (String.Compare(family.filename, filename, true) == 0)
                {
                    if (String.Compare(family.filename, family.change, true) == 0)	// 바꿀 필요가 없다.
                        return null;
                    return family;
                }
            }
            return null;
        }


        
        public static void AddTagList(List<object> block, string tag, EnumTagType tag_type)
        {
            MULTI_SELECT_TAG_STRUCT list;
            int l;

            for (l = 0; l < block.Count; l++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)block[l];
                if (String.Compare(list.tagSource, tag, StringComparison.CurrentCultureIgnoreCase) == 0) return;	// same tag already exist
            }

            list = new MULTI_SELECT_TAG_STRUCT();
            list.tagSource = tag;
            list.tagTarget = tag;
            list.tag_type = tag_type;
            block.Add(list);
        }*/
        
        public static bool IsNeedUpdateTag(List<object> block, ref string tag)
        {
            MULTI_SELECT_TAG_STRUCT list;
            int l;

            for (l = 0; l < block.Count; l++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)block[l];
                if (String.Compare(list.tagSource, tag, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (String.Compare(list.tagSource, list.tagTarget, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        return false;	// no need update
                    }
                    tag = list.tagTarget;
                    return true;	// need to update
                }
            }

            return false;	// can't seek match tag = 
        }

        protected override void CalcRotationCenter(out int cx, out int cy)
        {
            //base.CalcRotationCenter(out cx, out cy);
            int x1 = nViewX1;
            int y1 = nViewY1;
            int x2 = nViewX2;
            int y2 = nViewY2;

            if (x1 > x2) NetTools.Tools.Temp(ref x1, ref x2);
            if (y1 > y2) NetTools.Tools.Temp(ref y1, ref y2);

            ScaleTransform transform = transformScale;// new ScaleTransform();

            double width = sizeGroup.cx * transform.ScaleX;
            double height = sizeGroup.cy * transform.ScaleY;    

            x2 = (int)(x1 + width - 1);
            y2 = (int)(y1 + height - 1);

            if (this.nBasicExpandOptionRotate == 1) // 회전축이 Left,Top
            {
                cx = x1;
                cy = y1;
            }
            else if (this.nBasicExpandOptionRotate == 2) // 회전축이 RIght,Top 
            {
                cx = x2;
                cy = y1;
            }
            else if (this.nBasicExpandOptionRotate == 3)// 회전축이 Left,Bottom
            {
                cx = x1;
                cy = y2;
            }
            else if (this.nBasicExpandOptionRotate == 4)// 회전축이 RIght,Bottom 
            {
                cx = x2;
                cy = y2;
            }
            else // 회전축이 중심
            {
                cx = (int)(x1 + width / 2);
                cy = (int)(y1 + height / 2);
            }
        }
    }
}
