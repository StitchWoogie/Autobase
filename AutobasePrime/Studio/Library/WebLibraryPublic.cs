using System;
using System.Collections.Generic;

using System.Text;
using AutoLibLocal;
using System.IO;
using GraphicModule;
using NetTools;
using System.Windows.Forms;
using System.Drawing;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Collections;
using DialogTag.TagEditor;

namespace Studio
{
    public abstract class WebLibraryPublic
    {
        public WebLibraryGate webgate;
        public int nIconSize = 1;

        // 라이브러리 삽입 시 위치 보관
        public int nTempInsertType = 0;
        public int nTempInsertTheme = 0;
        public int nTempInsertGroup = 0;
        public string sTempInsertItem = "";

        // 라이브러리 등록 시 위치 보관
        public int nTempRegistType = 0;
        public int nTempRegistTheme = 0;
        public int nTempRegistGroup = 0;
        //public string sTempRegistItem = "";

        public int nTempSearchInsertKeyword = 0;
        public string sTempSearchInsertItem = "";
        public string sLibExt;

        public int nTempPreviewSplitter = 0;

        public virtual MemoryStream ObjectToStream(string source_directory, string keywords)
        {
            return null;
        }

        public virtual object PreviewItemNew()
        {
            return null;
        }

        public virtual void PreviewItemLoad(Form form, PreviewItemPublic pip, byte[] buffer, int nViewBoxX, int nViewBoxY, int nBigPreviewSizeX, int nBigPreviewSizeY)
        {

        }

        public virtual void PreviewItemDisplay(PreviewItemPublic pip, Graphics g, int x, int y, Rectangle cliprect)
        {
            
        }

        protected void LoadInformation(MemoryStream stream, PreviewItemPublic item)
        {
            MemoryStream sinfo = ObjectAnimation.RestoreFromZipStream(stream, "info.txt");

            if (sinfo == null)
            {
                return; // 없을 수도 있으므로
            }

            TextReader reader = new StreamReader(sinfo);
            string one_line;
            CommaTextReader comma = new CommaTextReader();
            string buf = "";
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                comma.Set(one_line);
                comma.GetString(ref buf);
                if (buf == "Keywords")
                {
                    comma.GetString(ref item.sKeywords);
                }
            }

            reader.Close();

            sinfo.Close();
        }

        public virtual void PreviewPrePlaySetObject(PreviewItemPublic pip, TreeView treeViewInfo)
        {

        }

        public virtual void PreviewPrePlayTimer(Form form, Panel parent)
        {
            
        }

        public virtual void PreviewPrePlayPaint(Graphics g, Rectangle r)
        {
            
        }

        public virtual object InsertSelection(PreviewItemPublic pip, byte[] buffer)
        {
            return null;
        }

        /*
        public virtual string InsertSelectionWithModule(PreviewItemPublic pip, byte[] buffer)
        {
            return null;
        }*/

        protected ArrayList GetTagsFromLibrary(MemoryStream stream, string item_name)
        {
            if (TotalConfig.LoadRegAutoBaseConfig("Config", "Library", "IncludeTagWhenInsertFromLibrary", true) == false) return null;

            MemoryStream sinfo = ObjectAnimation.RestoreFromZipStream(stream, "Group.tagx");

            if (sinfo == null)
            {
                return null; // 없을 수도 있으므로
            }

            TagFile file = new TagFile();
            TagGrClass gr = new TagGrClass();

            if (!file.LoadTag(sinfo, gr, false))
            {
                sinfo.Close();
                return null;
            }

            sinfo.Close();

            if (gr.arrayTag.Count == 0) return null; // 태그가 없다.

            // 먼저 태그의 리스트를 만든다.
            TagListStruct[] list = TagLib.MakeTagList(gr);

            ArrayList arrayTag = new ArrayList();

            for (int i = 0; i < list.Length; i++)
            {
                TagUtil.AddTagList(arrayTag, list[i].tag, list[i].type, "Library", EnumTagUsedType.None, null, "WebLibraryPublic");
            }

            int new_number;
            string group_name = "";
            TagPublicClass tp;

            // 속한 멤버가 1개이고 그 태그가 그룹 태그이면 그룹의 이름만 바꾸어서 사용한다.
            if (gr.arrayTag.Count == 1 && ((TagPublicClass)gr.arrayTag[0]).enumTagType == EnumTagType.GR)
            {
                tp = (TagPublicClass)gr.arrayTag[0];

                tp.name = FormTagEditor.MakeNewTagName(TagLib.groupRoot.arrayTag, tp.name, out new_number);
            }
            else
            {
                // 그렇지 않은 경우는 유일한 새로운 그룹을 만들어서 그룹속에 포함하여 사용한다.
                tp = gr;

                tp.act = 1;
                tp.name = FormTagEditor.MakeNewTagName(TagLib.groupRoot.arrayTag, "Library", out new_number);
            }

            if (group_name.Length == 0)
                tp.tag = tp.name;
            else
                tp.tag = group_name + "." + tp.name;

            TagLib.groupRoot.AddTag(tp, false);

            if (tp.enumTagType == EnumTagType.GR)
            {
                FormTagEditor.RecurseCalcTagName((TagGrClass)tp); // 그룹속에 있는 태그들도 그룹명에 맞추어 다시 명명해야 한다. 2007.9.18
            }

            list = TagLib.MakeTagList((TagGrClass)tp);
            MULTI_SELECT_TAG_STRUCT tag_list;

            for (int i = 0; i < list.Length; i++)
            {
                tag_list = (MULTI_SELECT_TAG_STRUCT)arrayTag[i];
                tag_list.tagTarget = list[i].tag;
            }

            TagLib.ChangeTagNotFound();
            AutoLib.TerminalClass.SaveTag(TagLib.groupRoot);

            return arrayTag;
        }

        protected string GetKeywords(MemoryStream stream)
        {
            MemoryStream sk = ObjectAnimation.RestoreFromZipStream(stream, "info.txt");

            if (sk == null) return "";

            TextReader reader = new StreamReader(sk);
            CommaTextReader comma = new CommaTextReader();
            string one_line;
            string command = "";
            string keywords = "";
            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref command);

                if (command == "Keywords")
                {
                    comma.GetString(ref keywords);
                }
            }

            return keywords;
        }

        /// <summary>
        /// 오브젝트의 형식을 개략적으로 알 수 있는 샘플 오브젝트를 보내주어 DrawAndDrop에서 활용할 수 있도록 한다.
        /// </summary>
        /// <returns></returns>
        public virtual object GetSampleObject(PreviewItemPublic pip)
        {
            return null;
        }
    }

    public class PreviewItemPublic
    {
        public string name;
        public object group; // //public objectbjectGroup group = new ObjectGroup(null, null, null, null, null);
        public int width;
        public int height;
        //public ObjectCommonProperty ocp = new ObjectCommonProperty();
        public string sKeywords = "";
        public bool bLoaded = false;
        public int nPrice;
        public int nDownLoad;
        public string username;
        public DateTime tCreate;    // 등록한 날짜
        public string itemname = "";    // 일반적으로 name과 같지만 템플릿 라이브러리 구매의 경우 이름이 숫자이므로 실제적인 itemname으로 사용하면 오리지널 파일명으로 템플릿을 만들 수 있다.
        public bool web_share;      // 웹상에 공유 상태인가?
        public float score;     // 평점 0 은 아직 평가가 없는 뜻
        public string comment;  // 제작자의 코멘트
        public DateTime tUpdate;    // 마지막 수정된 날짜
    }
}
