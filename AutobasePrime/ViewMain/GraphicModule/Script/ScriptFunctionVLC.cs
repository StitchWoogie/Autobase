using System;
using System.Threading.Tasks;
using AutoLib;
using AutoLibLocal;


namespace GraphicModule
{
    class ScriptFunctionVLC
    {
        //static int Run_ObjectVLC(ScriptClass scriptClass, string method_name, out object retn_value, object[] args)
        //{
        //    retn_value = 0;

        //    if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
        //    {
        //        retn_value = scriptClass.ExecuteClassName(ObjectVLCAx.arrayClassList, (string)args[0], method_name, args);

        //    }
        //    return 1;
        //}


        //VLC 오디오 명령 연속 실행 시 응답없음 방지 추가 20241010 PSU
        private static DateTime lastAudioCommandTime = DateTime.MinValue;
        private const int MIN_COMMAND_INTERVAL_MS = 200; // 최소 명령 간격 (밀리초)

        static async Task<(int, object val)> Run_ObjectVLC(ScriptClass scriptClass, string method_name, object[] args)
        {
            object retn_value = 0;
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (method_name == "VLCAudioMute" || method_name == "VLCAudioVolume")
                {
                    DateTime now = DateTime.Now;
                    if ((now - lastAudioCommandTime).TotalMilliseconds < MIN_COMMAND_INTERVAL_MS)
                    {
                        // 최소 간격 내에 다시 명령이 들어오면 무시
                        return (1, retn_value);
                    }
                    lastAudioCommandTime = now;
                }

                retn_value = await scriptClass.ExecuteClassName(ObjectVLCAx.arrayClassList, (string)args[0], method_name, args);
            }
            return (1, retn_value);
        }


        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "VLC";

            prepare.AddMethod(prename, "VLCPlaylistAdd", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname", "in:string:mrl", "in:string:options");
            prepare.AddMethod(prename, "VLCPlaylistPlay", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname");
            prepare.AddMethod(prename, "VLCPlaylistStop", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname");
            prepare.AddMethod(prename, "VLCPlaylistClear", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname");
            prepare.AddMethod(prename, "VLCAudioMute", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname", "in:int:mute");
            prepare.AddMethod(prename, "VLCAudioVolume", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname", "in:int:volume");
            prepare.AddMethod(prename, "VLCAutoLoop", "void", new ScriptExternalRun.AsyncDeleMethod(Run_ObjectVLC), "in:string:classname", "in:int:autoloop");
        }

    }
}
