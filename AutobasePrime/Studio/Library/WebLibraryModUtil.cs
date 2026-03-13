using System;
using System.Collections.Generic;

using System.Text;
using GraphicModule;
using System.IO;
using NetTools.OldDefine;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Collections;
using AutoLibLocal;
using NetTools;

namespace Studio
{
    public class WebLibraryModUtil
    {
        public static ObjectGroup LoadLocalOldGroup(System.Windows.Forms.Form form, string item_directory, int viewx, int viewy, ref int width, ref int height)
        {
            string filename = item_directory + "\\Group.modx";

            if (!File.Exists(filename))
            {
                filename = item_directory + "\\Group.mod";
                if (!File.Exists(filename))
                {
                    return null;
                }
            }

            ObjectCommonProperty ocp = new ObjectCommonProperty();

            GraphicTool.bLoadOnLibrary = true;

            GraphicTool.sLoadOnLibraryDir = item_directory;

            ocp.sModuleName = filename;

            ObjectGroup group = new ObjectGroup(null, null, null, null, null);
          
            LoadToGroup(form, ocp, group, filename, viewx,viewy, ref width, ref height);

            GraphicTool.bLoadOnLibrary = false;

            return group;
        }

        public static void LoadToGroup(System.Windows.Forms.Form form, ObjectCommonProperty ocp, ObjectGroup group, string filename, int nViewBoxX, int nViewBoxY, ref int rx, ref int ry)
        {
            if (!group.Load(ocp, form, filename)) return;

            int pic_sizex;
            int pic_sizey;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            group.GetZone(ref x1, ref y1, ref x2, ref y2);
            pic_sizex = x2 - x1 + 1;
            pic_sizey = y2 - y1 + 1;
            //group.UpdateZone(0, 0, nViewBoxX - 1, nViewBoxY - 1);
            group.SetModuleSize(pic_sizex, pic_sizey);

            if (pic_sizex < nViewBoxX && pic_sizey < nViewBoxY)
            { // 영역속에 들어간다.
                rx = pic_sizex;
                ry = pic_sizey;
            }
            else
            {
                rx = nViewBoxX;
                ry = nViewBoxX * pic_sizey / pic_sizex;

                if (ry > nViewBoxY)
                {
                    ry = nViewBoxY;
                    rx = nViewBoxY * pic_sizex / pic_sizey;
                }
            }

            group.SetScreenSize(nViewBoxX, nViewBoxY);

            RECT r = new RECT();
            r.left = 0;
            r.top = 0;
            r.right = rx - 1;
            r.bottom = ry - 1;
            group.SetZoneAtPercent100(group.sizeGroup, r);
        }

        public static MemoryStream GroupToStream(ObjectGroup group, string source_directory, string keywords)
        {
            MemoryStream stream = new MemoryStream();
            ZipOutputStream s = new ZipOutputStream(stream);

            s.SetLevel(6); // 0 - store only to 9 - means best compression

            ArrayList family_file = new ArrayList();
            group.GetFamilyFile(family_file);

            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();
            string source_path;

            for (int l = 0; l < family_file.Count; l++)
            {
                family = (FAMILY_FILE_STRUCT)family_file[l];
                source_path = String.Format("{0}\\{1}", source_directory, family.filename);

                if (File.Exists(source_path))
                {
                    byte[] buffer = File.ReadAllBytes(source_path);
                    WebLibraryUtil.AddOneEntry(s, buffer, family.filename, File.GetLastWriteTime(source_path), File.GetAttributes(source_path));
                }
            }

            SaveGroup(s, group);
            WebLibraryUtil.SaveInformation(s, keywords);
            SaveTagList(s, group);

            s.Finish();
            s.Close();

            return stream;
        }

        static void SaveTagList(ZipOutputStream s, ObjectGroup group)
        {
            if (TotalConfig.LoadRegAutoBaseConfig("Config", "Library", "IncludeTagWhenRegisterToLibrary", true) == false) return;

            ArrayList block = new ArrayList();
            group.GetMultiSelectTagList(block);

            WebLibraryUtil.SaveTagListPublic(s, block);
        }

        static void SaveGroup(ZipOutputStream s, ObjectGroup group)
        {
            MemoryStream stream = new MemoryStream();
            CommaTextWriter writer = new CommaTextWriter(stream);

            group.ObjectSave(writer);

            writer.Close();

            byte[] buffer = stream.ToArray();

            string dir = "Group.modx";
            WebLibraryUtil.AddOneEntry(s, buffer, dir, DateTime.Now, FileAttributes.Archive);

            stream.Close();
        }

        
    }
}
