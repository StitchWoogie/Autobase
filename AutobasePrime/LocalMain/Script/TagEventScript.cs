using System;
using System.Collections.Generic;
using System.Text;
using GraphicModule;
using System.IO;
using AutoLibLocal;
using System.Threading.Tasks;

namespace LocalMain.Script
{
    public class TagEventScript
    {
        static List<TagEventScriptItem> listTagEvent = new List<TagEventScriptItem>();

        static ScriptClass LoadScript(string filename)
        {
            for (int i = 0; i < listTagEvent.Count; i++)
            {
                if (String.Compare(listTagEvent[i].filename, filename, true) == 0)
                {
                    return listTagEvent[i].script;
                }
            }

            TagEventScriptItem item = new TagEventScriptItem();

            item.filename = filename;
            item.script = null;

            listTagEvent.Add(item);

            if (!File.Exists(filename)) return null;

            ScriptClass script = new ScriptClass();
            if (script.LoadFromFile(filename) == 1)
            {
                item.script = script;
            }

            return item.script;
        }

        public static void InitOne(TagPublicClass tp)
        {
            tp.scriptTagEvent = null;       // 감시 프로그램에서 스크립트를 새로 설정하면 이전의 스크립트는 지워준다.
            if (tp.sScriptTagEvent == null) return;
            if (tp.sScriptTagEvent.Length > 0)
            {
                string filename = String.Format("{0}\\Control\\TagEvent\\{1}", TotalConfig.sDirWorkProject, tp.sScriptTagEvent);
                tp.scriptTagEvent = TagEventScript.LoadScript(filename);
            }
        }

        public static void Init()
        {
            TagPublicClass tp;

            for (int i = 0; i < TagLib.tagListAll.Length; i++)
            {
                tp = TagLib.GetStructPublic(TagLib.tagListAll[i]);

                InitOne(tp);
            }
        }

        public static string sTagEventTag = "";

        public static async Task Run(TagPublicClass tp)
        {
            if (tp.scriptTagEvent == null) return;

            ScriptClass script = (ScriptClass)tp.scriptTagEvent;

            sTagEventTag = tp.tag;
            await script.RunAsync(FormLocalMain.formMain, null);
            if (script.IsError())
            {
                string message;
                message = script.GetError();
                MessageDisplay.Show("Tag Event Script Error: Filename={0}\n{1}", tp.sScriptTagEvent, message);
            }
        }
    }

    class TagEventScriptItem
    {
        public string filename;		// filename
        public ScriptClass script;	// control class point
    }
}
