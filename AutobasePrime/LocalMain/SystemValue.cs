using System;
using System.Collections.Generic;
using System.Text;
using GraphicModule;
using NetTools;

namespace LocalMain
{
    class SystemValue
    {
        public static void Init()
        {
            GraphicModule.ScriptFunctionSystem.procSystemValueGet = new GraphicModule.ScriptFunctionSystem.DeleSystemValueGet(SystemValueGet);
            GraphicModule.ScriptFunctionSystem.procSystemValueSet = new GraphicModule.ScriptFunctionSystem.DeleSystemValueSet(SystemValueSet);
        }

        public static object SystemValueGet(EnumSystemValue item)
        {
            if (item == EnumSystemValue.LocalMainSharedDatabaseActive)
                return SharedDatabase.configWebServer.bActive;
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveAlarmTable)
                return SharedDatabase.configWebServer.bTableAlarmFile;
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveTagTable)
                return SharedDatabase.configWebServer.bTableTagExchange;

            else if (item == EnumSystemValue.LocalMainScheduleActive)
                return CheckEngineSchedule.bActiveSchedule;
            else if (item == EnumSystemValue.LocalMainScriptActive)
                return CheckEngineAlwaysScript.bActiveScript;

            else if (item == EnumSystemValue.UserControlBoxDescription)
                return GraphicTool.sTempUserControlBoxDescription;
            else if (item == EnumSystemValue.UserControlBoxMaxValue)
                return GraphicTool.sTempUserControlBoxMaxValue;
            else if (item == EnumSystemValue.UserControlBoxMinValue)
                return GraphicTool.sTempUserControlBoxMinValue;
            else if (item == EnumSystemValue.UserControlBoxTitle)
                return GraphicTool.sTempUserControlBoxTitle;
            else if (item == EnumSystemValue.UserControlBoxTag)
                return GraphicTool.sTempUserControlBoxTag;

            else if (item == EnumSystemValue.TagEventTag)
                return Script.TagEventScript.sTagEventTag;

            else if (item == EnumSystemValue.ColumnIndex)
                return ObjectDataGridView.nSystemValueColumnIndex;
            else if (item == EnumSystemValue.RowIndex)
                return ObjectDataGridView.nSystemValueRowIndex;

            else
                return 0;
        }

        public static int SystemValueSet(EnumSystemValue item, object data)
        {
            int retn = 1;

            if (item == EnumSystemValue.LocalMainSharedDatabaseActive)
                SharedDatabase.configWebServer.bActive = ConvertTool.ToBoolean(data);
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveAlarmTable)
                SharedDatabase.configWebServer.bTableAlarmFile = ConvertTool.ToBoolean(data);
            else if (item == EnumSystemValue.LocalMainSharedDatabaseSaveTagTable)
                SharedDatabase.configWebServer.bTableTagExchange = ConvertTool.ToBoolean(data);

            else if (item == EnumSystemValue.LocalMainScheduleActive)
            {
                bool old_value = CheckEngineSchedule.bActiveSchedule;

                CheckEngineSchedule.bActiveSchedule = ConvertTool.ToBoolean(data);

                FormLocalMain.formMain.Invalidate(true);

                if (old_value == false && CheckEngineSchedule.bActiveSchedule == true)
                {
                    FormSchedule.OnScheduleStructChanged();
                }
            }
            else if (item == EnumSystemValue.LocalMainScriptActive)
            {
                CheckEngineAlwaysScript.bActiveScript = ConvertTool.ToBoolean(data);
                FormAlwaysScript.SetTitle();
            }

            else
                retn = 0;

            return retn;
        }
    }
}