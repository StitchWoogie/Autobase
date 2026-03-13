using AutoLibLocal;
using BasicScreen.kdymain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicModule
{
    public class ScriptFunctionLang
    {
        static int Run_SetLang(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = 0;

            string langCode = (string)args[0];

            //if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            //{
            //}

            LanguageManager.Instance.SetLang(langCode);

            return 1;
        }


        static int Run_GetLang(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";

            val = LanguageManager.Instance.GetLang();

            return 1;
        }

        static int Run_Lang(ScriptClass scriptClass, string method_name, out object val, object[] args)
        {
            val = "";

            string key = (string)args[0];

            val = LanguageManager.Instance.Lang(key);

            return 1;
        }
        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "Lang";

            prepare.AddMethod(prename, "LangSet", "void", new ScriptExternalRun.DeleMethod(Run_SetLang), "in:string:langCode");

            prepare.AddMethod(prename, "LangGet", "string", new ScriptExternalRun.DeleMethod(Run_GetLang));
            prepare.AddMethod(prename, "Lang", "string", new ScriptExternalRun.DeleMethod(Run_Lang), "in:string:key");

        }

    }
}
