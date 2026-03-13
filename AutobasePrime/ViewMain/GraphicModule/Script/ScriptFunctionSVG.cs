using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.Drawing;
using System.Threading.Tasks;

namespace GraphicModule
{
    public class ScriptFunctionSVG
    {
        //20241024 PSU SVG 
        static async Task<(int, object val)> Run_ObjectPublic(ScriptClass scriptClass, string method_name,  object[] args)
        {
             object retn_value = 0;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, (string)args[0], method_name, args);
            }

            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementColor(ScriptClass scriptClass, string method_name, object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                int color = (int)args[2];
                int flagFill = (int)args[3];
                int flagStroke = (int)args[4];
                
                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementColor", className, elementId, color, flagFill, flagStroke);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementVisible(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                int flag = (int)args[2];

                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementVisible", className, elementId, flag);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementRotate(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                float angle = ((float)args[2]);

                retn_value =  await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementRotate", className, elementId, angle);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementMove(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                float x = ((float)args[2]);
                float y = ((float)args[3]);

                retn_value =  await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementMove", className, elementId, x, y);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementText(ScriptClass scriptClass, string method_name, object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                string newText = (string)args[2];

                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementText", className, elementId, newText);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementTextSize(ScriptClass scriptClass, string method_name,  object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                float size = ((float)args[2]);
                string unit = (string)args[3];

                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementTextSize", className, elementId, size, unit);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementTransform(ScriptClass scriptClass, string method_name, object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];

                // 기본값 설정
                float rotate = 0f;
                float scaleX = 1f;
                float scaleY = 1f;
                float translateX = 0f;
                float translateY = 0f;

                // args 길이에 따라 파라미터 설정
                if (args.Length > 2) // rotate
                {
                    rotate = (float)args[2];
                }
                if (args.Length > 4) // scale
                {
                    scaleX = (float)args[3];
                    scaleY = (float)args[4];
                }
                if (args.Length > 6) // translate
                {
                    translateX = (float)args[5];
                    translateY = (float)args[6];
                }

                retn_value =  await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementTransform",
                    className, elementId, rotate, scaleX, scaleY, translateX, translateY);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementScale(ScriptClass scriptClass, string method_name, object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];
                float scaleX = ((float)args[2]);
                float scaleY = ((float)args[3]);

                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementScale", className, elementId, scaleX, scaleY);
            }
            return (1, retn_value);
        }

        static async Task<(int, object val)> Run_SVGElementReset(ScriptClass scriptClass, string method_name, object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                string className = (string)args[0];
                string elementId = (string)args[1];

                retn_value = await scriptClass.ExecuteClassName(ObjectSVG.arrayClassList, className, "SVGElementReset", className, elementId);
            }
            return (1, retn_value);
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "SVG";

            prepare.AddMethod(prename, "SVGSetFile", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectPublic), "in:string:classname", "in:string:filename");
            prepare.AddMethod(prename, "SVGElementReset", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementReset), "in:string:classname", "in:string:elementId");
            prepare.AddMethod(prename, "SVGElementColor", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementColor), "in:string:classname", "in:string:elementId", "in:int:color", "in:int:flagFill", "in:int:flagStroke");
            prepare.AddMethod(prename, "SVGElementVisible", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementVisible), "in:string:classname", "in:string:elementId", "in:int:flag");
            prepare.AddMethod(prename, "SVGElementRotate", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementRotate), "in:string:classname", "in:string:elementId", "in:float:angle");
            prepare.AddMethod(prename, "SVGElementMove", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementMove), "in:string:classname", "in:string:elementId", "in:float:x", "in:float:y");
            prepare.AddMethod(prename, "SVGElementText", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementText), "in:string:classname", "in:string:elementId", "in:string:newText");
            prepare.AddMethod(prename, "SVGElementScale", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementScale),
        "in:string:classname", "in:string:elementId", "in:float:scaleX", "in:float:scaleY");
            prepare.AddMethod(prename, "SVGElementTextSize", "void", new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementTextSize),
    "in:string:classname", "in:string:elementId", "in:float:size", "in:string:unit");

            prepare.AddMethod(prename, "SVGElementTransform", "void",
       new ScriptExternalRun.AsyncDeleMethod(Run_SVGElementTransform),
       "in:string:classname",
       "in:string:elementId",
       "in:float:rotate",      // 회전 각도
       "in:float:scaleX",      // X축 스케일
       "in:float:scaleY",      // Y축 스케일
       "in:float:translateX",  // X축 이동
       "in:float:translateY"); // Y축 이동
        }
	
    }
}
