using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using GraphicModule;
using System.Drawing;

namespace Studio
{
    class ClassEditObjectSpuit : ClassMainTool
    {
        Cursor hCursorSpuit = new Cursor(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Studio.Cursor.Spuit2.cur"));

        public override void MouseDownLeft(FormEditGraphic form, MouseEventArgs e)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                ClassEditObjectMove.MainToolLButtonDownMoveWithKeyShift(form, e);
                return;
            }

            if (work.nSelectCount == 0) return; // 선택된 오브젝트가 없다.

            SELECT_LIST list;
            ObjectExpand obj;

            ObjectExpand obj2 = (ObjectExpand)work.obj.groupRoot.GetObjectOfMouseZone(e);

            if (obj2 == null) return;

            ClassStudioEditUndo.UndoSave_Selected(form, "Color Picker");

            Color lcolor = obj2.GetLineColor();
            BrushPublic fcolor = obj2.GetFillColor();
            BrushPublic bcolor = obj2.GetBackColor();
            Color tcolor = obj2.GetTextColor();

            bool use_backcolor = obj2.IsUseBackColor;
            bool use_fillcolor = obj2.IsUseFillColor;
            bool use_textcolor = obj2.IsUseTextColor;
            bool use_linecolor = obj2.IsUseLineColor;

            for (int j = 0; j < work.nSelectCount; j++)
            {
                list = work.selectList[j];

                if (((ObjectType)list.obj).enumObjectType == EnumObjectType.Group) continue;

                obj = (ObjectExpand)list.obj;

                if (obj == obj2) continue;  // 자기 자신

                if(use_linecolor == true)
                    obj.SetLineColor(lcolor);

                // 원본이 배경색을 사용하면 
                if (use_backcolor == true)
                    obj.SetBackColor((BrushPublic)NetTools.Tools.CopyObject(bcolor));
                else
                {
                    if (use_fillcolor == true)
                        obj.SetBackColor((BrushPublic)NetTools.Tools.CopyObject(fcolor));
                }

                // 원본이 채움색을 사용하면 
                if (use_fillcolor == true)
                    obj.SetFillColor((BrushPublic)NetTools.Tools.CopyObject(fcolor));
                else
                {
                    if (use_backcolor == true)
                        obj.SetFillColor((BrushPublic)NetTools.Tools.CopyObject(bcolor));
                }

                if (use_textcolor == true)
                    obj.SetTextColor(tcolor);

                obj.InvalidateObject(form);
                obj.previewOnStudio = null;
            }

            ClassEditProperty.SelectChanged(form);
            Layer.FormLayer.InvalidateDisplay();
        }

        public override void MouseMove(FormEditGraphic form, MouseEventArgs e)
        {
            WORK_MODULE_STRUCT work = form.workThis;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                form.Cursor = Cursors.Arrow;
            }
            else 
                form.Cursor = hCursorSpuit;

            base.MouseMove(form, e);
        }
    }
}
