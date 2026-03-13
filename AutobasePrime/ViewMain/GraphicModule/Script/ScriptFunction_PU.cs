using System;
using AutoLib;
using AutoLibLocal;
using NetTools;
using System.IO;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptFunctionComboBox.
	/// </summary>
	public class ScriptFunction_PU
	{
        public static int Run_PlaySound(ScriptClass script, string name, out object retn_value, object[] args)
        {
            string filename = (string)args[0];

            retn_value = 0;

            string path;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                path = MakeFilePath.Sound(filename);

                if (!File.Exists(path))
                {
                    if (Tools.IsLangKorean())
                    {
                        script.ErrorMessage(String.Format("PlaySound에서 [{0}] 파일을 찾을 수 없습니다.", path));
                    }
                    else
                    {
                        script.ErrorMessage(String.Format("Can't found [{0}] file at PlaySound function", path));
                    }
                    return -1;
                }
                else
                {
                    Win32Function.PlaySound(null, IntPtr.Zero, EnumPlaySound.SND_ASYNC);	// 1 = SND_ASYNC
                    Win32Function.PlaySound(path, IntPtr.Zero, EnumPlaySound.SND_ASYNC);	// 1 = SND_ASYNC
                }
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "P";

            prepare.AddMethod(prename, "PlaySound", "void", new ScriptExternalRun.DeleMethod(Run_PlaySound), "in:string:filename");
        }
	}
}
