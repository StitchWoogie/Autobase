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
		public string 	filename;	// 얻어올 때의 파일 명
		public string   change;		// Change 할 때 파일 명
	}

    

	/// <summary>
	/// Summary description for ObjectGroup.
	/// </summary>
	/// 
	[Serializable]
	public class ObjectGroup : ObjectPublicGroupLayer
	{
		//ArrayList objectList;

		//public RECT rGroupZone = new RECT();	// 그룹의 현재 좌표값
		public SIZE sizeGroup = new SIZE();		// 그룹의 실제 사이즈

        public ObjectGroup(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general) : 
            base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.Group;

			//rGroupZone.left = 0;
			//rGroupZone.top = 0;
			//rGroupZone.right = 0;
			//rGroupZone.bottom = 0;
			sizeGroup.cx = 0;
			sizeGroup.cy = 0;
		}

        public int Load(ObjectCommonProperty ocp, Form form, TextReader reader, int depth, EnumModType load_type)
        {
            int retn = 0;
            if (load_type == EnumModType.mod)
            {
                ObjectGroupLoadMod load = new ObjectGroupLoadMod();
                retn = load.Load(ocp, this, form, reader, depth, load_type);
            }
            else if (load_type == EnumModType.modx)
            {
                ObjectGroupLoadModX load = new ObjectGroupLoadModX();
                retn = load.Load(ocp, this, form, reader, depth, load_type);
            }
            else
            {

            }

            return retn;
        }

		// Plan Load한 후 한번 불러준다.
		public void UpdateGroupRealSize(int x, int y) 
		{ 
			sizeGroup.cx = x;
			sizeGroup.cy = y; 
		}	

		public void GetGroupRealSize(ref int x, ref int y) 
		{ 
			x = sizeGroup.cx;
			y = sizeGroup.cy; 
		}	// Plan Load한 후 한번 불러준다. 
		
        /*
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
        }*/

		//-----------------------------------------------------------------------------------------
		// 6.20 이후 버전부터는 Group 특성이 바뀌었으므로 이전 버전들은 모두 속성을 바꿔주어야 한다.
		//-----------------------------------------------------------------------------------------

		public void MakeGroupRect(System.Windows.Forms.Form form)
		{
			if(objectList.Count == 0)	return;

			RECT  r=new RECT(), rMax=new RECT();
			int l;

			for(l = 0; l < objectList.Count; l++) 
			{
				GetZone(l, ref r);
				if(r.left > r.right)	Tools.Temp(ref r.left, ref r.right);
				if(r.top  > r.bottom)	Tools.Temp(ref r.top,  ref r.bottom);

				if(l == 0) 
				{
					rMax.left  = r.left;
					rMax.top = r.top;
					rMax.right = r.right;
					rMax.bottom = r.bottom;
				}
				else 
				{
					if(r.left < rMax.left)		rMax.left  = r.left;
					if(r.right > rMax.right)	rMax.right = r.right;
					if(r.top < rMax.top)		rMax.top  = r.top;
					if(r.bottom > rMax.bottom)	rMax.bottom  = r.bottom;
				}
			}

            nLeft = rMax.left;
            nTop = rMax.top;
            nRight = rMax.right;
            nBottom = rMax.bottom;
            sizeGroup.cx = nRight - nLeft + 1;
            sizeGroup.cy = nBottom - nTop + 1;

			RECT rLeft=new RECT();

			for(l = 0; l < objectList.Count; l++) 
			{
				GetZone(l, ref rLeft);
		
				rLeft.left -= rMax.left;
				rLeft.top  -= rMax.top;
				rLeft.right -= rMax.left;
				rLeft.bottom  -= rMax.top;

                ((ObjectExpand)objectList[l]).UpdateZone(form, rLeft.left, rLeft.top, rLeft.right, rLeft.bottom);
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

			r.left  = x1;
			r.top   = y1;
			r.right = x2;
			r.bottom= y2;

			return 1;
		}

		public override void ObjectSave(CommaTextWriter writer)
		{
			int l;
			string name;
			ObjectType obj;
			int left=0, top=0, right=0, bottom=0;

			writer.WriteLine("ObjectGroup,BEGIN,");
			writer.WriteLine("\tGroupRect,{0},{1},{2},{3},", nLeft, nTop, nRight, nBottom);
			writer.WriteLine("\tGroupSize,{0},{1},", sizeGroup.cx, sizeGroup.cy);

			for(l = 0; l < objectList.Count; l++) 
			{
				obj = (ObjectType)objectList[l];
				if(obj.enumObjectType == EnumObjectType.Group || obj.enumObjectType == EnumObjectType.Layer) 
				{
					((ObjectExpand)obj).ObjectSave(writer);
				}
				else 
				{
					name = Path.GetExtension(objectList[l].ToString()).Substring(1);

					if(name == "ObjectButtonProgram")	// 9.0.9 부터는 ObjectButtonProgram으로 바뀌었으나 호환성을 유지하기 위해서 m을 당분간은 하나 더 붙여준다.
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

            SaveObjectItem.ClassName(writer, objGeneral);
            SaveExpandScript(writer);

			writer.WriteLine("ObjectGroup,END,");
		}

		public static FAMILY_FILE_STRUCT GetMatchFamilyFile(string filename, ArrayList block)
		{
			int l;
			FAMILY_FILE_STRUCT family;

			for(l = 0; l < block.Count; l++) 
			{
				family = (FAMILY_FILE_STRUCT)block[l];
				if(String.Compare(family.filename, filename, true) == 0) 
				{
                    if (family.change == null)  // 바꿀 필요가 없다.
                        return null;
					if(String.Compare(family.filename, family.change, true) == 0)	// 바꿀 필요가 없다.
						return null;
					return family;
				}
			}
			return null;
		}

        

		public static bool IsNeedUpdateTag(ArrayList block, ref string tag)
		{
			MULTI_SELECT_TAG_STRUCT list;
			int l;

			for(l = 0; l < block.Count; l++) 
			{
				list = (MULTI_SELECT_TAG_STRUCT)block[l];
				if(String.Compare(list.tagSource, tag, true) == 0) 
				{
					if(String.Compare(list.tagSource, list.tagTarget, true) == 0) 
					{
						return false;	// no need update
					}
					tag = list.tagTarget;
					return true;	// need to update
				}
			}

			return false;	// can't seek match tag = 
		}

        public bool LoadModX(ObjectCommonProperty ocp, Form form, Stream stream)
        {
            if (stream == null) return false;

            TextReader reader;

            reader = new StreamReader(stream);

            if (reader == null) return false;

            string one_line;
            CommaTextReader comma = new CommaTextReader();
            string command = "";

            one_line = reader.ReadLine();
            comma.Set(one_line);
            comma.GetString(ref command);

            if (command == "ObjectGroup")
            {
                Load(ocp, form, reader, 1, EnumModType.modx);
            }

            reader.Close();

            return true;
        }

		public bool Load(ObjectCommonProperty ocp, Form form, string filename)
		{
			if(!File.Exists(filename))	return false;

			EnumModType mod_type;

			if(String.Compare(Path.GetExtension(filename), ".modx", true) == 0)
				mod_type = EnumModType.modx;
			else
				mod_type = EnumModType.mod;

			TextReader reader;
			
			if(mod_type == EnumModType.modx)
				reader = new StreamReader(filename);
			else
				reader = new StreamReader(filename, System.Text.Encoding.Default);

			if(reader == null)	return false;

			string one_line;
			CommaBlockString comma = new CommaBlockString();
			string command="";

			one_line = reader.ReadLine();
			comma.Set(one_line);
			comma.GetString(ref command);

			if(mod_type == EnumModType.modx) 
			{
				if(command == "ObjectGroup") 
				{
					Load(ocp, form, reader, 1, mod_type);
				}
			}
			else 
			{
				if(command == "Group") 
				{
					Load(ocp, form, reader, 1, mod_type);
				}
			}

			reader.Close();

			return true;
		}

        public override void Display(Graphics g, Rectangle rcPaint, int originx, int originy)
        {
            // 그룹인 경우 먼저 체크하고
            {
                if (!ExpandCalcVisible()) return;   // 그룹 오브젝트인 경우는 Visible상태를 검사한다.

                base.Display(g, rcPaint, originx, originy);                // 마우스 영역존을 그려야 한다.
            }

            if (RotationAngle == 0)
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
            else
            {
                int l;
                object obj;

                int cx, cy;

                CalcRotationCenter(out cx, out cy);

                g.TranslateTransform(cx, cy);
                g.RotateTransform(RotationAngle);

                originx = cx;
                originy = cy;

                for (l = 0; l < objectList.Count; l++)
                {
                    obj = objectList[l];

                    // 편집 상태에서는 StudioVisible이 아니면 그리지 않는다.
                    if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT && !((ObjectExpand)obj).objGeneral.bOnStudioVisible) continue;

                    ((ObjectExpand)obj).Display(g, rcPaint, originx, originy);
                }

                g.ResetTransform();
            }
        }
        
        public override void UpdateZone(Form form, int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);        // 그룹인 경우 좌표가 이상하면 전체적인 화면 디스프레이에서 이상해질 수 있으므로 미리 막는다.
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            base.UpdateZone(form, x1, y1, x2, y2);
        }

        public override void EditFlipHorz()
        {
            int i;
            ObjectExpand obj;
            int x1=0, y1=0, x2=0, y2=0;
            int bx1, bx2;

            bx1 = 0;
            bx2 = sizeGroup.cx - 1;

            for (i = 0; i < objectList.Count; i++)
            {
                obj = (ObjectExpand)objectList[i];

                obj.GetZone(ref x1, ref y1, ref x2, ref y2);
                
                x1 = bx2 - (x1 - bx1);
                x2 = bx2 - (x2 - bx1);

                obj.UpdateZone(objCommonProperty.form, x1, y1, x2, y2);

                ((ObjectExpand)obj).EditFlipHorz();
            }
        }

        public override void EditFlipVert()
        {
            int i;
            ObjectExpand obj;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int by1, by2;

            by1 = 0;
            by2 = sizeGroup.cy - 1;

            for (i = 0; i < objectList.Count; i++)
            {
                obj = (ObjectExpand)objectList[i];

                obj.GetZone(ref x1, ref y1, ref x2, ref y2);

                y1 = by2 - (y1 - by1);
                y2 = by2 - (y2 - by1);

                obj.UpdateZone(objCommonProperty.form, x1, y1, x2, y2);

                ((ObjectExpand)obj).EditFlipVert();
            }
        }

        public override void EditRotateRight(int nx1, int ny1, int nx2, int ny2)
        {
            int i;
            ObjectExpand obj;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int bx1, bx2, by1, by2;

            bx1 = 0;
            bx2 = sizeGroup.cx - 1;
            by1 = 0;
            by2 = sizeGroup.cy - 1;

            int gabx, gaby;
            int width = bx2 - bx1;
            int height = by2 - by1;

            for (i = 0; i < objectList.Count; i++)
            {
                obj = (ObjectExpand)objectList[i];

                obj.GetZone(ref x1, ref y1, ref x2, ref y2);

                gabx = x1 - bx1;
                gaby = y1 - by1;

                x1 = bx1 + height - gaby;
                y1 = by1 + gabx;

                gabx = x2 - bx1;
                gaby = y2 - by1;

                x2 = bx1 + height - gaby;
                y2 = by1 + gabx;

                ((ObjectExpand)obj).EditRotateRight(x1, y1, x2, y2);
            }

            sizeGroup.cx = by2 + 1;
            sizeGroup.cy = bx2 + 1;

            UpdateZone(objCommonProperty.form, nx1, ny1, nx2, ny2);
        }

        public override void EditRotateLeft(int nx1, int ny1, int nx2, int ny2)
        {
            int i;
            ObjectExpand obj;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            int bx1, bx2, by1, by2;

            bx1 = 0;
            bx2 = sizeGroup.cx - 1;
            by1 = 0;
            by2 = sizeGroup.cy - 1;

            int gabx, gaby;
            int width = bx2 - bx1;
            int height = by2 - by1;

            for (i = 0; i < objectList.Count; i++)
            {
                obj = (ObjectExpand)objectList[i];

                obj.GetZone(ref x1, ref y1, ref x2, ref y2);

                gabx = x1 - bx1;
                gaby = y1 - by1;

                x1 = bx1 + gaby;
                y1 = by1 + width - gabx;

                gabx = x2 - bx1;
                gaby = y2 - by1;

                x2 = bx1 + gaby;
                y2 = by1 + width - gabx;

                ((ObjectExpand)obj).EditRotateLeft(x1, y1, x2, y2);
            }

            sizeGroup.cx = by2 + 1;
            sizeGroup.cy = bx2 + 1;

            UpdateZone(objCommonProperty.form, nx1, ny1, nx2, ny2);
        }

        public void EditReCalcGroupRect(System.Windows.Forms.Form form)
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

            int new_sizex, new_sizey;

            new_sizex = rMax.right - rMax.left + 1;
            new_sizey = rMax.bottom - rMax.top + 1;

            if (new_sizex == sizeGroup.cx && new_sizey == sizeGroup.cy) return; // 그룹의 크기는 변함이 없으므로 그냥 돌아간다.

            int old_w = nRight - nLeft + 1;
            int old_h = nBottom - nTop + 1;

            int old_gw = sizeGroup.cx;
            int old_gh = sizeGroup.cy;

            sizeGroup.cx = new_sizex;
            sizeGroup.cy = new_sizey;

            nLeft = nLeft + rMax.left;
            nTop = nTop + rMax.top;
            nRight = nLeft + (new_sizex * old_w / old_gw) - 1;
            nBottom = nTop + (new_sizey * old_h / old_gh) - 1;

            RECT rLeft = new RECT();

            for (l = 0; l < objectList.Count; l++)
            {
                GetZone(l, ref rLeft);

                rLeft.left -= rMax.left;
                rLeft.top -= rMax.top;
                rLeft.right -= rMax.left;
                rLeft.bottom -= rMax.top;

                ((ObjectExpand)objectList[l]).UpdateZone(form, rLeft.left, rLeft.top, rLeft.right, rLeft.bottom);
            }
        }
	}
}





