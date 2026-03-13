using System;
using System.Collections.Generic;

using System.Text;
using GraphicModule;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using NetTools.OldDefine;
using NetTools;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using AutoLibLocal;

namespace Studio
{
    public class WebLibraryModFull : WebLibraryPublic
    {
        public ObjectRoot groupTemp;

        //public const string sLibExt = "LibM";

        public WebLibraryModFull()
        {
            sLibExt = "LibM";
            webgate = new WebLibraryGate("ModFull");
            nIconSize = 3;
            nTempPreviewSplitter = 300;
        }

        MemoryStream RootToStream(ObjectRoot group, string source_directory, string keywords)
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

            SaveRoot(s, group);
            WebLibraryUtil.SaveInformation(s, keywords);
            SaveTagList(s, group);

            s.Finish();
            s.Close();

            return stream;
        }

        void SaveTagList(ZipOutputStream s, ObjectRoot group)
        {
            if (TotalConfig.LoadRegAutoBaseConfig("Config", "Library", "IncludeTagWhenRegisterToLibrary", true) == false) return;

            ArrayList block = new ArrayList();
            group.GetMultiSelectTagList(block, "library.modx");

            WebLibraryUtil.SaveTagListPublic(s, block);
        }

        void SaveRoot(ZipOutputStream s, ObjectRoot group)
        {
            MemoryStream stream = new MemoryStream();

            group.ObjectSaveToStream(stream);
            
            byte[] buffer = stream.ToArray();

            stream.Close();

            string dir = "Group.modx";
            WebLibraryUtil.AddOneEntry(s, buffer, dir, DateTime.Now, FileAttributes.Archive);
        }

        public override MemoryStream ObjectToStream(string source_directory, string keywords)
        {
            return RootToStream(groupTemp, source_directory, keywords);
        }

        public override object PreviewItemNew()
        {
            PreviewItemModFull item = new PreviewItemModFull();

            //item.ocp.bLoadByZipStream = true;

            return item;
        }

        void Make비례축소(int tx, int ty, int sx, int sy, out int width, out int height)
        {
            if (sx < tx && sy < ty)
            { // 영역속에 들어간다.
                width = sx;
                height = sy;
            }
            else
            {
                width = tx;
                height = tx * sy / sx;

                if (height > ty)
                {
                    height = ty;
                    width = ty * sx / sy;
                }
            }
        }

        public override void PreviewItemLoad(Form form, PreviewItemPublic pip, byte[] buffer, int nViewBoxX, int nViewBoxY, int nBigPreviewSizeX, int nBigPreviewSizeY)
        {
            if (buffer == null) return;

            MemoryStream stream = new MemoryStream(buffer);
            if (stream == null) return;

            pip.sKeywords = GetKeywords(stream);

            PreviewItemModFull item = (PreviewItemModFull)pip.group;

            item.ocp.bLoadByZipStream = true;
            item.ocp.streamZip = stream;
            item.ocp.bLoadOnLibraryView = true;

            ObjectRoot root = new ObjectRoot();

            root.objCommonProperty.bLoadOnLibraryView = true;
            root.objCommonProperty.sModuleName = pip.name;      // 별로 사용하지는 않지만 이름을 알면 참고가 될 듯

            if (root.Load(form, stream) == 0) return;

            stream.Close();

            int pic_sizex;
            int pic_sizey;

            root.GetModuleSize(out pic_sizex, out pic_sizey);

            Make비례축소(nViewBoxX, nViewBoxY, pic_sizex, pic_sizey, out pip.width, out pip.height);
            
            Bitmap bitmap = new Bitmap(pic_sizex, pic_sizey);
            Graphics g = Graphics.FromImage(bitmap);
            Rectangle r = new Rectangle(0, 0, pic_sizex, pic_sizey);
            Point p = new Point(0, 0);
            root.Display(g, r, r, p);
            //g.Flush();

            // 각 아이템의 Preview
            Bitmap bitmap2 = new Bitmap(pip.width, pip.height);
            g = Graphics.FromImage(bitmap2);
            //r = new Rectangle(0, 0, pip.width, pip.height);
            //p = new Point(0, 0);
            g.DrawImage(bitmap, 0, 0, pip.width, pip.height);

            item.bitmap = bitmap2;


            int width, height;
            Make비례축소(nBigPreviewSizeX, nBigPreviewSizeY, pic_sizex, pic_sizey, out width, out height);
            Bitmap bitmap3 = new Bitmap(width, height);
            g = Graphics.FromImage(bitmap3);
            g.DrawImage(bitmap, 0, 0, width, height);

            item.bitmapInfo = bitmap3;

            item.tree = new TreeNode();
            root.groupRoot.AddObjectInfo(item.tree);
        }

        public override void PreviewItemDisplay(PreviewItemPublic pip, Graphics g, int x, int y, Rectangle cliprect)
        {
            PreviewItemModFull item = (PreviewItemModFull)pip.group;

            if(item.bitmap != null)
                g.DrawImageUnscaled(item.bitmap, x, y);
        }

        //ObjectRoot objPlay = null;
        Bitmap objPlay = null;

        public override void PreviewPrePlaySetObject(PreviewItemPublic pip, TreeView treeViewInfo)
        {
            PreviewItemModFull item = (PreviewItemModFull)pip.group;

            objPlay = item.bitmapInfo;
            
            treeViewInfo.Nodes.Clear();
            string msg;

            if (Tools.IsLangKorean()) msg = String.Format("템플릿명:{0}", pip.name);
            else if (Tools.IsLangJapanese()) msg = String.Format("Templete名:{0}", pip.name);
            else if (Tools.IsLangChinese()) msg = String.Format("Templete名:{0}", pip.name);
            else msg = String.Format("Templete Name:{0}", pip.name);

            if (pip.nDownLoad > 0)
                msg += String.Format(" Down:{0}", pip.nDownLoad);

            msg += String.Format(" Owner:{0}", pip.username);
            msg += String.Format(" Create:{0}", pip.tCreate);

            TreeNode root = new TreeNode(msg);
            treeViewInfo.Nodes.Add(root);

            string buf = String.Format("Price:{0} {1}  Score:{2:F1}", FormLibraryGroupPreview.sPriceUnit, pip.nPrice, pip.score);
            TreeNode ni = new TreeNode(buf);
            root.Nodes.Add(ni);

            if (pip.comment != null && pip.comment.Length > 0)
            {
                buf = String.Format("Comment:{0}", pip.comment);
                ni = new TreeNode(buf);
                root.Nodes.Add(ni);
            }

            if (pip.sKeywords.Length > 0)
            {
                buf = String.Format("Keywords:{0}", pip.sKeywords);
                ni = new TreeNode(buf);
                root.Nodes.Add(ni);
            }

            TreeNode node = new TreeNode("Object");

            if (item.tree != null)
            {
                item.tree.Text = "Object";
                node = item.tree;
            }

            treeViewInfo.Nodes.Add(node);

            treeViewInfo.ExpandAll();
            root.EnsureVisible();

            /*
            TreeNode root = new TreeNode(msg);
            treeViewInfo.Nodes.Add(root);

            string buf = String.Format("Price:{0} {1}", FormLibraryGroupPreview.sPriceUnit, pip.nPrice);
            TreeNode ni = new TreeNode(buf);
            root.Nodes.Add(ni);

            TreeNode node = new TreeNode("Object");
            treeViewInfo.Nodes.Add(node);

            objPlay.AddObjectInfo(node);

            treeViewInfo.ExpandAll();
            root.EnsureVisible();
            */
        }

        public override void PreviewPrePlayPaint(Graphics g, Rectangle r)
        {
            if (objPlay != null)
            {
                g.DrawImageUnscaled(objPlay, 0, 0);
            }
        }

        public override object InsertSelection(PreviewItemPublic pip, byte[] buffer)
        {
            if (buffer == null) return null;

            MemoryStream stream = new MemoryStream(buffer);
            if (stream == null) return null;

            PreviewItemModFull item = (PreviewItemModFull)pip.group;

            ObjectRoot root = new ObjectRoot();
            root.objCommonProperty.bLoadOnLibraryView = true;

            if (root.Load(SharedStudio.formMain, stream) == 0) return null;

            ArrayList family_file = new ArrayList();
            root.GetFamilyFile(family_file);

            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            string target_dir = String.Format("{0}\\Graphic", AutoLibLocal.TotalConfig.sDirWorkProject);

            for (int l = 0; l < family_file.Count; l++)
            {
                family = (FAMILY_FILE_STRUCT)family_file[l];

                MemoryStream s = ObjectAnimation.RestoreFromZipStream(stream, family.filename);
                ClassStudioEditCopyFile.CopyFileToGraphicDirectoryFamilyFromStream(target_dir, s, family);
            }

            root.ChangeFamilyFile(family_file);

            ArrayList array = GetTagsFromLibrary(stream, pip.name);

            if (array != null)
            {
                root.SetMultiSelectTagList(array);
            }

            return root;
        }

        /*
        // Control Box 전용일 경우 계속 추가하다보면 같은 내용이 여러파일로 생기는 것을 방지하기 위해 모듈도 같이 해당 폴더에 직접 복사한다. (즉 변하기 전에는 또 복사하는 것을 방지하기 위하여)
        public override string InsertSelectionWithModule(PreviewItemPublic pip, byte[] buffer)
        {
            if (buffer == null) return null;

            MemoryStream stream = new MemoryStream(buffer);
            if (stream == null) return null;

            PreviewItemModFull item = (PreviewItemModFull)pip.group;

            ObjectRoot root = new ObjectRoot();
            root.objCommonProperty.bLoadOnLibraryView = true;

            if (root.Load(SharedStudio.formMain, stream) == 0) return null;

            ArrayList family_file = new ArrayList();
            root.GetFamilyFile(family_file);

            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            string target_dir = String.Format("{0}\\Graphic", AutoLibLocal.TotalConfig.sDirWorkProject);
            MemoryStream s;

            for (int l = 0; l < family_file.Count; l++)
            {
                family = (FAMILY_FILE_STRUCT)family_file[l];

                s = ObjectAnimation.RestoreFromZipStream(stream, family.filename);
                ClassStudioEditCopyFile.CopyFileToGraphicDirectoryFamilyFromStream(target_dir, s, family);
            }

            root.ChangeFamilyFile(family_file);

            // Control Box는 태그를 사용하지 않으므로 태그를 복사할 필요가 없다.
            //ArrayList array = GetTagsFromLibrary(stream, pip.name);

            //if (array != null)
            //{
            //    root.SetMultiSelectTagList(array);
            //}

            s = ObjectAnimation.RestoreFromZipStream(stream, "Group.modx");
            family = new FAMILY_FILE_STRUCT();
            family.filename = pip.name;
            ClassStudioEditCopyFile.CopyFileToGraphicDirectoryFamilyFromStream(target_dir, s, family);
            
            return family.change;
        }*/
    }

    class PreviewItemModFull
    {
        public Bitmap bitmap;                   // 각 아이템에 나타나는 미리보기
        public Bitmap bitmapInfo;               // 비교적 큰 미리보기 
        public ObjectCommonProperty ocp = new ObjectCommonProperty();
        public TreeNode tree;
    }
}
