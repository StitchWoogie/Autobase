using System;
using NetTools;
using AutoLibLocal;
using AutoLib;

namespace GraphicModule
{
    /// <summary>
    /// Summary description for ScriptFunctionTag.
    /// </summary>
    public class ScriptFunctionSystem
    {
        /*
        public ScriptFunctionSystem()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        static EnumSystemValue SeekSystemName(string item)
        {
            EnumSystemValue e;

            if (item == "LocalMainSharedDatabaseSaveAlarmTable")
            {
                e = EnumSystemValue.LocalMainSharedDatabaseSaveAlarmTable;
            }
            else if (item == "LocalMainSharedDatabaseSaveTagTable")
            {
                e = EnumSystemValue.LocalMainSharedDatabaseSaveTagTable;
            }
            else if (item == "LocalMainSharedDatabaseActive")
            {
                e = EnumSystemValue.LocalMainSharedDatabaseActive;
            }
            else if (item == "LocalMainScheduleActive")
            {
                e = EnumSystemValue.LocalMainScheduleActive;
            }
            else if (item == "LocalMainScriptActive")
            {
                e = EnumSystemValue.LocalMainScriptActive;
            }

            else if (item == "UserControlBoxDescription")
            {
                e = EnumSystemValue.UserControlBoxDescription;
            }
            else if (item == "UserControlBoxMaxValue")
            {
                e = EnumSystemValue.UserControlBoxMaxValue;
            }
            else if (item == "UserControlBoxMinValue")
            {
                e = EnumSystemValue.UserControlBoxMinValue;
            }
            else if (item == "UserControlBoxTitle")
            {
                e = EnumSystemValue.UserControlBoxTitle;
            }
            else if (item == "UserControlBoxTag")
            {
                e = EnumSystemValue.UserControlBoxTag;
            }

            else
            {
                try
                {
                    e = (EnumSystemValue)Enum.Parse(typeof(EnumSystemValue), item);
                }
                catch
                {
                    e = EnumSystemValue.NotFound;
                }

            }

            return e;
        }

        public delegate object DeleSystemValueGet(EnumSystemValue item);
        public static DeleSystemValueGet procSystemValueGet = null;

        public delegate int DeleSystemValueSet(EnumSystemValue item, object data);
        public static DeleSystemValueSet procSystemValueSet = null;

        static int Function_SystemValueGet(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string item;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out item)) return -1;

            EnumSystemValue e = SeekSystemName(item);

            if (e == EnumSystemValue.NotFound)
            {
                scriptClass.ErrorMessage(String.Format("SystemValueGet에서 존재하지 않는 item입니다.({0})", item));
                return -1;
            }

            if (procSystemValueGet != null)
            {
                value = procSystemValueGet(e);
            }

            return 1;
        }

        static int Function_SystemValueSet(ScriptClass scriptClass, string command, string argument, out object value)
        {
            ScriptArgumentString arg = new ScriptArgumentString();
            string buf;
            string item;
            object data;

            value = 0;

            arg.Set(argument);

            arg.GetArgument(out buf);
            if (!scriptClass.GetArgumentString(buf, out item)) return -1;

            arg.GetArgument(out buf);
            if (!scriptClass.GetValueRecurse(buf, out data)) return -1;

            EnumSystemValue e = SeekSystemName(item);

            if (e == EnumSystemValue.NotFound)
            {
                scriptClass.ErrorMessage(String.Format("SystemValueSet에서 존재하지 않는 item입니다.({0})", item));
                return -1;
            }

            if (procSystemValueGet != null)
            {
                value = procSystemValueSet(e, data);
            }

            return 1;
        }

        public static int Function_System(ScriptClass scriptClass, string command, string argument, out object value)
        {
            if (String.Compare(command, "SystemValueGet") == 0)
            {
                return Function_SystemValueGet(scriptClass, command, argument, out value);
            }
            else if (String.Compare(command, "SystemValueSet") == 0)
            {
                return Function_SystemValueSet(scriptClass, command, argument, out value);
            }
            else
            {
                if (Tools.IsLangKorean())
                {
                    scriptClass.ErrorMessage(String.Format("지원되지 않는 System?? 함수입니다.({0})", command));
                }
                else if (Tools.IsLangChinese())
                {
                    scriptClass.ErrorMessage(String.Format("不支持的 System 函数。({0})", command));
                }
                else
                {
                    scriptClass.ErrorMessage(String.Format("Undefined System function ({0})", command));
                }
                value = 0;
                return -1;
            }
        }*/
        static EnumSystemValue SeekSystemName(string item)
        {
            EnumSystemValue e;

            if (item == "LocalMainSharedDatabaseSaveAlarmTable")
            {
                e = EnumSystemValue.LocalMainSharedDatabaseSaveAlarmTable;
            }
            else if (item == "LocalMainSharedDatabaseSaveTagTable")
            {
                e = EnumSystemValue.LocalMainSharedDatabaseSaveTagTable;
            }
            else if (item == "LocalMainSharedDatabaseActive")
            {
                e = EnumSystemValue.LocalMainSharedDatabaseActive;
            }
            else if (item == "LocalMainScheduleActive")
            {
                e = EnumSystemValue.LocalMainScheduleActive;
            }
            else if (item == "LocalMainScriptActive")
            {
                e = EnumSystemValue.LocalMainScriptActive;
            }

            else if (item == "UserControlBoxDescription")
            {
                e = EnumSystemValue.UserControlBoxDescription;
            }
            else if (item == "UserControlBoxMaxValue")
            {
                e = EnumSystemValue.UserControlBoxMaxValue;
            }
            else if (item == "UserControlBoxMinValue")
            {
                e = EnumSystemValue.UserControlBoxMinValue;
            }
            else if (item == "UserControlBoxTitle")
            {
                e = EnumSystemValue.UserControlBoxTitle;
            }
            else if (item == "UserControlBoxTag")
            {
                e = EnumSystemValue.UserControlBoxTag;
            }

            else
            {
                try
                {
                    e = (EnumSystemValue)Enum.Parse(typeof(EnumSystemValue), item);
                }
                catch
                {
                    e = EnumSystemValue.NotFound;
                }

            }

            return e;
        }

        public delegate object DeleSystemValueGet(EnumSystemValue item);
        public static DeleSystemValueGet procSystemValueGet = null;

        public delegate int DeleSystemValueSet(EnumSystemValue item, object data);
        public static DeleSystemValueSet procSystemValueSet = null;

        static int Run_SystemValueGet(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            string item = (string)args[0];

            value = 0;

            EnumSystemValue e = SeekSystemName(item);

            if (e == EnumSystemValue.NotFound)
            {
                scriptClass.ErrorMessage(String.Format("SystemValueGet에서 존재하지 않는 item입니다.({0})", item));
                return -1;
            }

            if (procSystemValueGet != null)
            {
                value = procSystemValueGet(e);
            }

            return 1;
        }

        static int Run_SystemValueSet(ScriptClass scriptClass, string method_name, out object value, object[] args)
        {
            string item = (string)args[0];
            object data = args[1];

            value = 0;

            EnumSystemValue e = SeekSystemName(item);

            if (e == EnumSystemValue.NotFound)
            {
                scriptClass.ErrorMessage(String.Format("SystemValueSet에서 존재하지 않는 item입니다.({0})", item));
                return -1;
            }

            if (procSystemValueGet != null)
            {
                value = procSystemValueSet(e, data);
            }

            return 1;
        }

        public static void PrepareMethod(ScriptExternalRun prepare)
        {
            string prename = "System";

            prepare.AddMethod(prename, "SystemValueGet", "object", new ScriptExternalRun.DeleMethod(Run_SystemValueGet), "in:string:item");
            prepare.AddMethod(prename, "SystemValueSet", "void", new ScriptExternalRun.DeleMethod(Run_SystemValueSet), "in:string:item", "in:object:data");
        }
    }

    public enum EnumSystemValue
    {
        NotFound = -1,
        LocalMainSharedDatabaseSaveAlarmTable = 1,  // 공유 데이터베이스 경보 테이블 저장
        LocalMainSharedDatabaseSaveTagTable,        // 공유 데이터베이스 태그값 테이블 저장
        LocalMainSharedDatabaseActive,              // 공유 데이터베이스 활성
        LocalMainScheduleActive,                    // 스케쥴 제어 활성
        LocalMainScriptActive,                      // 스크립트 활성
        UserControlBoxTitle,
        UserControlBoxDescription,
        UserControlBoxMinValue,
        UserControlBoxMaxValue,
        UserControlBoxTag,

        TagEventTag,                                // 태그 이벤트 발생시의 태그이름

        ColumnIndex,    // int
        RowIndex,       // int
    }
}