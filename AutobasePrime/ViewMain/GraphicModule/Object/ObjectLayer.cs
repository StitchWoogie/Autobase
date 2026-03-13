using System;
using System.Collections;
using System.Drawing;
using System.IO;
using NetTools;
using NetTools.OldDefine;
using System.Windows.Forms;
using AutoLib;
using AutoLibLocal;

namespace GraphicModule
{
	[Serializable]
	public class ObjectLayer : ObjectPublicGroupLayer
	{
        Color colorLayer;

        public Color LayerColor
        {
            get
            {
                return colorLayer;
            }
            set
            {
                colorLayer = value;
            }
        }

        public ObjectLayer(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
            :
            base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Layer;

            colorLayer = Color.LightGray;  // 레이어 색상
		}

        public override void ObjectSave(CommaTextWriter writer)
        {
            int l;
            string name;
            ObjectType obj;
            int left = 0, top = 0, right = 0, bottom = 0;

            Color color = colorLayer;
            int flag_lock = 0;
            int flag_show = 1;

            if (AutoLib.ConfigStudio.bSaveLayerLockStatus)  // 
            {
                flag_lock = objGeneral.bOnStudioLocked ? 1 : 0;
            }
            if (AutoLib.ConfigStudio.bSaveLayerShowStatus)
            {
                flag_show = objGeneral.bOnStudioVisible ? 1 : 0;
            }

            writer.WriteLine("ObjectLayer,BEGIN,{0},{1},{2},{3},{4},{5},{6},{7},", 0, objGeneral.sOnStudioTitle, color.R, color.G, color.B, color.A, flag_lock, flag_show);  // 0 = studio flags

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
        }

        public int Load(ObjectCommonProperty ocp, Form form, TextReader reader, int depth, EnumModType load_type)
        {
            int retn = 0;
            ObjectGroupLoadModX load = new ObjectGroupLoadModX();
            retn = load.Load(ocp, this, form, reader, depth, load_type);

            return retn;
        }

        public override void Display(Graphics g, Rectangle rcPaint, int originx, int originy)
        {
            int l;
            object obj;

            for (l = 0; l < objectList.Count; l++)
            {
                obj = objectList[l];

                // 편집 상태에서는 StudioVisible이 아니면 그리지 않는다.
                if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT && !((ObjectExpand)obj).objGeneral.bOnStudioVisible) continue;

                ((ObjectExpand)obj).Display(g, rcPaint, originx, originy);
            }
        }

        public void GetLayerZone(ref int lx1, ref int ly1, ref int lx2, ref int ly2)
        {
            int i;
            ObjectExpand obj;
            int x1 = -1, y1 = -1, x2 = -1, y2 = -1;

            for (i = 0; i < objectList.Count; i++)
            {
                obj = (ObjectExpand)objectList[i];

                if (obj.enumObjectType == EnumObjectType.Layer)
                {
                    x1 = -1;
                    y1 = -1;
                    x2 = -1;
                    y2 = -1;
                    ((ObjectLayer)obj).GetLayerZone(ref x1, ref y1, ref x2, ref y2);

                    // 오브젝트가 하나도 없다.
                    if (x1 == -1 && y1 == -1 && x2 == -1 && y2 == -1)
                    {
                        continue;
                    }
                }
                else
                {
                    obj.GetZone(ref x1, ref y1, ref x2, ref y2);
                    if (x1 > x2) Tools.Temp(ref x1, ref x2);
                    if (y1 > y2) Tools.Temp(ref y1, ref y2);
                }

                if (lx1 == -1 && ly1 == -1 && lx2 == -1 && ly2 == -1)   // 최초일 때
                {
                    lx1 = x1;
                    ly1 = y1;
                    lx2 = x2;
                    ly2 = y2;
                }
                else
                {
                    if (x1 < lx1) lx1 = x1;
                    if (y1 < ly1) ly1 = y1;
                    if (x2 > lx2) lx2 = x2;
                    if (y2 > ly2) ly2 = y2;
                }
            }
        }

        /*
        public override void GetZone(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            // 레이어에서 영역을 얻는다면 child의 전체 영역을 보내준다.

            GetLayerZone(ref x1, ref y1, ref x2, ref y2);
        }*/


    }
}





