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
using NetTools.OldDefine;
using NetTools;
using System.IO;

namespace SilverlightGraphicModule
{
    public class ObjectLayer : ObjectPublicGroupLayer
    {
        public ObjectLayer(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
            :
            base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Layer;
        }

        /*
        public override void ObjectSave(CommaTextWriter writer)
        {
            int l;
            string name;
            ObjectType obj;
            int left = 0, top = 0, right = 0, bottom = 0;

            writer.WriteLine("ObjectLayer,BEGIN,");

            for (l = 0; l < objectList.Count; l++)
            {
                obj = (ObjectType)objectList[l];
                if (obj.enumObjectType == EnumObjectType.Group || obj.enumObjectType == EnumObjectType.Layer)
                {
                    ((ObjectExpand)obj).ObjectSave(writer);
                }
                else
                {
                    name = Path.GetExtension(objectList[l].ToString()).Substring(1);

                    if (name == "ObjectButtonProgram")	// 9.0.9 부터는 ObjectButtonProgram으로 바뀌었으나 호환성을 유지하기 위해서 m을 당분간은 하나 더 붙여준다.
                        name = "ObjectButtonProgramm";

                    writer.WriteLine("{0},BEGIN", name);
                    ((ObjectExpand)obj).GetZone(ref left, ref top, ref right, ref bottom);

                    SaveObjectItem.Rect(writer, left, top, right, bottom);

                    ((ObjectExpand)obj).ObjectSave(writer);
                    SaveObjectItem.ClassName(writer, ((ObjectExpand)obj).objGeneral);
                    ((ObjectExpand)obj).SaveExpandScript(writer);

                    writer.WriteLine("{0},END", name);
                }
            }
            writer.WriteLine("ObjectLayer,END,");
        }*/

        public int Load(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string filename, int depth, EnumModType load_type)
        {
            int retn = 0;
            ObjectGroupLoadModX load = new ObjectGroupLoadModX();
            retn = load.Load(ocp, parent_canvas, this, form, reader, filename, depth, load_type);

            return retn;
        }

        /*
        public override void Display(Graphics g, Rectangle rcPaint, int originx, int originy)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                // 편집 상태에서는 StudioVisible이 아니면 그리지 않는다.
                if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT && !((ObjectType)obj).bOnStudioVisible) continue;

                ((ObjectExpand)obj).Display(g, rcPaint, originx, originy);
            }
        }*/


    }
}
