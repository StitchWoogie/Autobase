using System;
using System.Collections.Generic;

using System.Text;
using GraphicModule;
using System.IO;
using NetTools.OldDefine;
using System.Drawing;
using System.Windows.Forms;
using NetTools;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using AutoLibLocal;

namespace Studio
{
    public class WebLibraryModCut : WebLibraryPublic
    {
        public ObjectGroup groupTemp;

        //public const string sLibExt = "LibG";

        public WebLibraryModCut()
        {
            sLibExt = "LibG";
            webgate = new WebLibraryGate("ModCut");
            nIconSize = 2;
        }

        public override MemoryStream ObjectToStream(string source_directory, string keywords)
        {
            return WebLibraryModUtil.GroupToStream(groupTemp, source_directory, keywords);
        }

        public override object PreviewItemNew()
        {
            PreviewItemModCut item = new PreviewItemModCut();

            item.ocp.bLoadByZipStream = true;

            return item;
        }

        public override void PreviewItemLoad(Form form, PreviewItemPublic pip, byte[] buffer, int nViewBoxX, int nViewBoxY, int nBigPreviewSizeX, int nBigPreviewSizeY)
        {
            if (buffer == null) return;

            MemoryStream stream = new MemoryStream(buffer);
            if (stream == null) return;

            pip.sKeywords = GetKeywords(stream);

            PreviewItemModCut item = (PreviewItemModCut)pip.group;

            item.ocp.bLoadByZipStream = true;
            item.ocp.streamZip = stream;
            item.ocp.bLoadOnLibraryView = true;

            MemoryStream smod = ObjectAnimation.RestoreFromZipStream(stream, "group.modx");
            if (smod == null)
            {
                stream.Close();
                return;
            }

            if (!item.group.LoadModX(item.ocp, form, smod)) return;

            smod.Close();

            LoadInformation(stream, pip);

            stream.Close();

            int pic_sizex;
            int pic_sizey;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            item.group.GetZone(ref x1, ref y1, ref x2, ref y2);
            pic_sizex = x2 - x1 + 1;
            pic_sizey = y2 - y1 + 1;
            item.group.UpdateZone(form, 0, 0, nViewBoxX - 1, nViewBoxY - 1);
            item.group.SetModuleSize(pic_sizex, pic_sizey);

            if (pic_sizex < nViewBoxX && pic_sizey < nViewBoxY)
            { // 영역속에 들어간다.
                pip.width = pic_sizex;
                pip.height = pic_sizey;
            }
            else
            {
                pip.width = nViewBoxX;
                pip.height = nViewBoxX * pic_sizey / pic_sizex;

                if (pip.height > nViewBoxY)
                {
                    pip.height = nViewBoxY;
                    pip.width = nViewBoxY * pic_sizex / pic_sizey;
                }
            }

            item.group.SetScreenSize(nViewBoxX, nViewBoxY);

            RECT r = new RECT();
            r.left = 0;
            r.top = 0;
            r.right = pip.width - 1;
            r.bottom = pip.height - 1;
            item.group.SetZoneAtPercent100(item.group.sizeGroup, r);
        }

        public override void PreviewItemDisplay(PreviewItemPublic pip, Graphics g, int x, int y, Rectangle cliprect)
        {
            PreviewItemModCut item = (PreviewItemModCut)pip.group;

            item.group.SetBasePoint(x, y);
            item.group.Display(g, cliprect, 0, 0);
        }

        ObjectGroup objPlay = null;

        public override void PreviewPrePlaySetObject(PreviewItemPublic pip, TreeView treeViewInfo)
        {
            PreviewItemModCut item = (PreviewItemModCut)pip.group;

			objPlay = null;
            objPlay = (ObjectGroup)Tools.CopyObject(item.group);

            objPlay.SetBasePoint(0, 0); // 디스프레이가 1*1로 나와서 SetBasePoint를 해주니 정상으로 나온다.
			
			treeViewInfo.Nodes.Clear();
			string msg;

            if (Tools.IsLangKorean()) msg = String.Format("아이템명:{0}", pip.name);
            else if (Tools.IsLangJapanese()) msg = String.Format("アイテム名:{0}", pip.name);
            else if (Tools.IsLangChinese()) msg = String.Format("项名:{0}",  pip.name);
            else msg = String.Format("Item Name:{0}",  pip.name);

            if (Tools.IsLangKorean())
            {
                if (pip.nDownLoad > 0)
                    msg += String.Format(" 다운로드:{0}", pip.nDownLoad);

                if (pip.username != null && pip.username.Length > 0)
                    msg += String.Format(" 소유자:{0}", pip.username);

                msg += String.Format(" 생성일:{0}", pip.tCreate);
            }
            else
            {
                if (pip.nDownLoad > 0)
                    msg += String.Format(" Down:{0}", pip.nDownLoad);

                if (pip.username != null && pip.username.Length > 0)
                    msg += String.Format(" Owner:{0}", pip.username);

                msg += String.Format(" Create:{0}", pip.tCreate);
            }

			TreeNode root = new TreeNode(msg);
            treeViewInfo.Nodes.Add(root);

            string buf;
            
            if(Tools.IsLangKorean()) 
                buf = String.Format("가격:{0} {1}  평가:{2:F1}", FormLibraryGroupPreview.sPriceUnit, pip.nPrice, pip.score);
            else
                buf = String.Format("Price:{0} {1}  Score:{2:F1}", FormLibraryGroupPreview.sPriceUnit, pip.nPrice, pip.score);

            TreeNode ni = new TreeNode(buf);
            root.Nodes.Add(ni);

            if (pip.comment != null && pip.comment.Length > 0)
            {
                if(Tools.IsLangKorean())
                    buf = String.Format("설명:{0}", pip.comment);
                else
                    buf = String.Format("Comment:{0}", pip.comment);

                ni = new TreeNode(buf);
                root.Nodes.Add(ni);
            }

            if (pip.sKeywords.Length > 0)
            {
                if(Tools.IsLangKorean())
                    buf = String.Format("검색어:{0}", pip.sKeywords);
                else
                    buf = String.Format("Keywords:{0}", pip.sKeywords);

                ni = new TreeNode(buf);
                root.Nodes.Add(ni);
            }

            if (TotalConfig.eOemType == EnumOemType.UYeG_GS)
            {

            }
            else
            {
                TreeNode node = new TreeNode("Object");
                treeViewInfo.Nodes.Add(node);
                objPlay.AddObjectInfo(node);
            }

			treeViewInfo.ExpandAll();
            root.EnsureVisible();
        }

        public override void PreviewPrePlayTimer(Form form, Panel parent)
        {
			if(objPlay != null) 
			{
				objPlay.EventTimerOnPreview(form);
                parent.Invalidate();
			}
        }

        public override void PreviewPrePlayPaint(Graphics g, Rectangle r)
        {
            if (objPlay != null)
            {
                objPlay.Display(g, r, 0, 0);
            }
        }

        public override object InsertSelection(PreviewItemPublic pip, byte[] buffer)
        {
            PreviewItemModCut item = (PreviewItemModCut)pip.group;

            ObjectGroup group = (ObjectGroup)Tools.CopyObject(item.group);

            Form child = SharedStudio.formMain.ActiveMdiChild;

            if (child.Name != "FormEditGraphicFrame")
            {
                return null;
            }

            ArrayList family_file = new ArrayList();
            group.GetFamilyFile(family_file);

            MemoryStream rstream = null;

            rstream = new MemoryStream(buffer);

            FAMILY_FILE_STRUCT family = new FAMILY_FILE_STRUCT();

            FormEditGraphicFrame form = (FormEditGraphicFrame)child;

            for (int l = 0; l < family_file.Count; l++)
            {
                family = (FAMILY_FILE_STRUCT)family_file[l];

                MemoryStream stream = ObjectAnimation.RestoreFromZipStream(rstream, family.filename);
                ClassStudioEditCopyFile.CopyFileToGraphicDirectoryFamilyFromStream(Path.GetDirectoryName(form.formChild.workThis.obj.objCommonProperty.sModuleName), stream, family);
            }

            group.ChangeFamilyFile(family_file);

            ArrayList array = GetTagsFromLibrary(rstream, pip.name);

            if (array != null)
            {
                group.SetMultiSelectTagList(array);
            }

            return group;
        }

        public override object GetSampleObject(PreviewItemPublic pip)
        {
            PreviewItemModCut item = (PreviewItemModCut)pip.group;

            ObjectGroup group = (ObjectGroup)Tools.CopyObject(item.group);

            return group;
        }
    }

    class PreviewItemModCut
    {
        public ObjectGroup group = new ObjectGroup(null, null, null, null, null);
        public ObjectCommonProperty ocp = new ObjectCommonProperty();
    }
}
